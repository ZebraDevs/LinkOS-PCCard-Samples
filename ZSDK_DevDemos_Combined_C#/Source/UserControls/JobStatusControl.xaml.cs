/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2018
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using DeveloperDemo_Card_Desktop.Demos.MultiJob;
using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace DeveloperDemo_Card_Desktop.UserControls {

    /// <summary>
    /// Interaction logic for JobStatusControl.xaml
    /// </summary>
    public partial class JobStatusControl : UserControl {

        private const int CardFeedTimeoutMilliseconds = 60 * 1000;
        private Dictionary<int, string> jobIdLastMessageMap = new Dictionary<int, string>();

        public delegate void MultiJobStatusUpdatedHandler(MultiJobNumber jobNumber, int jobId, JobStatusInfo jobStatusInfo);
        public event MultiJobStatusUpdatedHandler MultiJobStatusUpdated;

        public JobStatusControl() {
            InitializeComponent();
        }

        public async Task StartPolling(DiscoveredPrinter printer, JobInfo jobInfo) {
            List<JobInfo> jobInfoList = new List<JobInfo> {
                jobInfo
            };

            await StartPolling(printer, jobInfoList);
        }

        public async Task StartPolling(DiscoveredPrinter printer, List<JobInfo> jobInfoList) {
            await Task.Run(() => {
                Connection connection = null;
                ZebraCardPrinter zebraCardPrinter = null;
                bool showAtmDialog = true;
                bool isFeeding = false;

                try {
                    connection = printer.GetConnection();
                    connection.Open();

                    zebraCardPrinter = ZebraCardPrinterFactory.GetInstance(connection);

                    Stopwatch stopwatch = Stopwatch.StartNew();

                    foreach (JobInfo jobInfo in jobInfoList) {
                        UpdateLog(jobInfo.JobId, $"Polling job status for job ID {jobInfo.JobId}...");
                    }

                    while (jobInfoList.Count > 0) {
                        foreach (JobInfo jobInfo in jobInfoList.ToList()) {
                            JobStatusInfo jobStatusInfo = zebraCardPrinter.GetJobStatus(jobInfo.JobId);

                            if (!isFeeding) {
                                stopwatch = Stopwatch.StartNew();
                            }

                            bool isAlarmInfoPresent = jobStatusInfo.AlarmInfo.Value > 0;
                            bool isErrorInfoPresent = jobStatusInfo.ErrorInfo.Value > 0;
                            isFeeding = jobStatusInfo.CardPosition.Contains("feeding");

                            string alarmInfo = isAlarmInfoPresent ? $"{jobStatusInfo.AlarmInfo.Value} ({jobStatusInfo.AlarmInfo.Description})" : jobStatusInfo.AlarmInfo.Value.ToString();
                            string errorInfo = isErrorInfoPresent ? $"{jobStatusInfo.ErrorInfo.Value} ({jobStatusInfo.ErrorInfo.Description})" : jobStatusInfo.ErrorInfo.Value.ToString();

                            string jobStatusMessage = $"Job ID {jobInfo.JobId}: status:{jobStatusInfo.PrintStatus}, position:{jobStatusInfo.CardPosition}, contact:{jobStatusInfo.ContactSmartCardStatus}, " +
                                $"contactless:{jobStatusInfo.ContactlessSmartCardStatus}, alarm:{alarmInfo}, error:{errorInfo}";

                            UpdateLog(jobInfo.JobId, jobStatusMessage, jobInfo.JobNumber, jobStatusInfo);

                            if (jobStatusInfo.PrintStatus.Equals("done_ok")) {
                                UpdateLog(jobInfo.JobId, $"Job ID {jobInfo.JobId} completed.", jobInfo.JobNumber, jobStatusInfo);

                                showAtmDialog = true;
                                stopwatch = Stopwatch.StartNew();
                                jobInfoList.Remove(jobInfo);
                            } else if (jobStatusInfo.PrintStatus.Equals("done_error")) {
                                UpdateLog(jobInfo.JobId, $"Job ID {jobInfo.JobId} completed with error: {jobStatusInfo.ErrorInfo.Description}", jobInfo.JobNumber, jobStatusInfo);

                                showAtmDialog = true;
                                stopwatch = Stopwatch.StartNew();
                                jobInfoList.Remove(jobInfo);
                            } else if (jobStatusInfo.PrintStatus.Contains("cancelled")) {
                                if (isErrorInfoPresent) {
                                    UpdateLog(jobInfo.JobId, $"Job ID {jobInfo.JobId} cancelled with error: {jobStatusInfo.ErrorInfo.Description}", jobInfo.JobNumber, jobStatusInfo);
                                } else {
                                    UpdateLog(jobInfo.JobId, $"Job ID {jobInfo.JobId} cancelled.", jobInfo.JobNumber, jobStatusInfo);
                                }

                                showAtmDialog = true;
                                stopwatch = Stopwatch.StartNew();
                                jobInfoList.Remove(jobInfo);
                            } else if (isAlarmInfoPresent) {
                                MessageBoxResult messageBoxResult = MessageBox.Show($"Job ID {jobInfo.JobId} encountered alarm [{jobStatusInfo.AlarmInfo.Description}].\r\n" +
                                    $"Either fix the alarm and click OK once the job begins again or click Cancel to cancel the job.", "Alarm Encountered", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                if (messageBoxResult == MessageBoxResult.Cancel) {
                                    zebraCardPrinter.Cancel(jobInfo.JobId);
                                }
                            } else if (isErrorInfoPresent) {
                                zebraCardPrinter.Cancel(jobInfo.JobId);
                            } else if (jobStatusInfo.ContactSmartCardStatus.Contains("at_station") || jobStatusInfo.ContactlessSmartCardStatus.Contains("at_station")) {
                                MessageBoxResult messageBoxResult = MessageBox.Show("Please click OK to resume the job or Cancel to cancel the job.", "Card at Station", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                                if (messageBoxResult == MessageBoxResult.Cancel) {
                                    zebraCardPrinter.Cancel(jobInfo.JobId);
                                } else {
                                    zebraCardPrinter.Resume();
                                }
                            } else if (isFeeding) {
                                if (showAtmDialog && jobInfo.CardSource == CardSource.ATM) {
                                    DialogHelper.ShowInsertCardDialog();
                                    showAtmDialog = false;
                                } else if (stopwatch.ElapsedMilliseconds > CardFeedTimeoutMilliseconds) {
                                    UpdateLog(jobInfo.JobId, $"Job ID {jobInfo.JobId} timed out waiting for a card and was cancelled.", jobInfo.JobNumber, jobStatusInfo);
                                    zebraCardPrinter.Cancel(jobInfo.JobId);
                                }
                            }

                            Thread.Sleep(500);
                        }
                    }
                } catch (Exception exception) {
                    MessageBoxHelper.ShowError($"Error polling job status: {exception.Message}");
                } finally {
                    ConnectionHelper.CleanUpQuietly(zebraCardPrinter, connection);
                }
            });
        }

        public void SetText(string text) {
            JobStatusLog.Text = text;
        }

        public void ClearLog() {
            JobStatusLog.Text = "";
        }

        public void UpdateLog(string message) {
            WriteToLog(message);
        }

        public void UpdateLog(int jobId, string message) {
            UpdateLog(jobId, message, null, null);
        }

        private void UpdateLog(int jobId, string message, MultiJobNumber? jobNumber, JobStatusInfo jobStatusInfo) {
            string lastMessageForJobId = jobIdLastMessageMap.ContainsKey(jobId) ? jobIdLastMessageMap[jobId] : null;
            if (lastMessageForJobId == null || !lastMessageForJobId.Equals(message)) {
                WriteToLog(message);
                jobIdLastMessageMap[jobId] = message;
            }

            if (MultiJobStatusUpdated != null && jobNumber.HasValue) {
                MultiJobStatusUpdated.Invoke(jobNumber.Value, jobId, jobStatusInfo);
            }
        }

        private void WriteToLog(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                if (!string.IsNullOrEmpty(JobStatusLog.Text)) {
                    JobStatusLog.AppendText("\n");
                }

                JobStatusLog.AppendText(GetCurrentTimestamp() + " " + message);
                JobStatusLog.ScrollToEnd();
            });
        }

        private static string GetCurrentTimestamp() {
            return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]");
        }

        public class JobInfo {
            public MultiJobNumber? JobNumber {
                get; set;
            }

            public int JobId {
                get; set;
            }

            public CardSource CardSource {
                get; set;
            }

            public JobInfo(int jobId, CardSource cardSource) {
                JobId = jobId;
                CardSource = cardSource;
            }

            public JobInfo(MultiJobNumber jobNumber, int jobId, CardSource cardSource) {
                JobNumber = jobNumber;
                JobId = jobId;
                CardSource = cardSource;
            }
        }
    }
}
