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

using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Exceptions;
using Zebra.Sdk.Card.Graphics;
using Zebra.Sdk.Card.Graphics.Enumerations;
using Zebra.Sdk.Card.Job;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Settings;
using static DeveloperDemo_Card_Desktop.UserControls.JobStatusControl;

namespace DeveloperDemo_Card_Desktop.Demos.MultiJob {

    /// <summary>
    /// Interaction logic for MultiJobDemo.xaml
    /// </summary>
    public partial class MultiJobDemo : UserControl {

        private const string ColorOption = "ymc";
        private static readonly string[] MonoRibbonOptions = { "k", "mono", "black", "white", "red", "blue", "silver", "gold" };
        private static readonly string[] OverlayRibbonOptions = { "ymcko", "kro", "kdo" };

        private MultiJobDemoViewModel viewModel;
        private PrinterManager printerManager;

        public MultiJobDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as MultiJobDemoViewModel;
            this.printerManager = printerManager;

            Job1Control.ViewModel.JobNumber = MultiJobNumber.One;
            Job2Control.ViewModel.JobNumber = MultiJobNumber.Two;
            Job3Control.ViewModel.JobNumber = MultiJobNumber.Three;
            Job4Control.ViewModel.JobNumber = MultiJobNumber.Four;

            JobStatusControl.MultiJobStatusUpdated += JobStatusControl_MultiJobStatusUpdated;

            RetrieveSettingsRanges();
        }

        private void JobStatusControl_MultiJobStatusUpdated(MultiJobNumber jobNumber, int jobId, JobStatusInfo jobStatusInfo) {
            if (jobStatusInfo != null) {
                string message = jobStatusInfo.PrintStatus;
                if (jobStatusInfo.PrintStatus.Equals("done_ok")) {
                    message = "Completed";
                } else if (jobStatusInfo.PrintStatus.Equals("done_error")) {
                    message = "Completed with error";
                } else if (jobStatusInfo.PrintStatus.Contains("cancelled")) {
                    message = jobStatusInfo.ErrorInfo.Value > 0 ? "Cancelled with error" : "Cancelled";
                }

                UpdateJobStatus(jobNumber, $"Job ID {jobId}: {message}");
            }
        }

        private void UpdateJobStatus(MultiJobNumber jobNumber, string status) {
            switch (jobNumber) {
                case MultiJobNumber.One:
                    viewModel.JobStatus1 = status;
                    break;
                case MultiJobNumber.Two:
                    viewModel.JobStatus2 = status;
                    break;
                case MultiJobNumber.Three:
                    viewModel.JobStatus3 = status;
                    break;
                case MultiJobNumber.Four:
                    viewModel.JobStatus4 = status;
                    break;
            }
        }

        private bool IsPrintTypeSupported(string installedRibbon, string[] ribbonTypeOptions) {
            bool isSupported = true;
            foreach (string option in ribbonTypeOptions) {
                if (installedRibbon.Contains(option)) {
                    isSupported = true;
                    break;
                } else {
                    isSupported = false;
                }
            }
            return isSupported;
        }

        private async void RetrieveSettingsRanges() {
            viewModel.Sources.Clear();
            viewModel.Destinations.Clear();
            viewModel.PrintOptimizations.Clear();
            viewModel.PrintTypes.Clear();
            viewModel.CoercivityTypes.Clear();

            await printerManager.PerformAction("Retrieving settings ranges...", (zebraCardPrinter, connection) => {
                string installedRibbon = zebraCardPrinter.GetSettingValue(ZebraCardSettingNames.RIBBON_DESCRIPTION);
                if (!string.IsNullOrEmpty(installedRibbon)) {
                    installedRibbon = installedRibbon.ToLower();

                    if (installedRibbon.Contains(ColorOption)) {
                        Application.Current.Dispatcher.Invoke(() => {
                            viewModel.PrintTypes.Add(PrintType.Color);
                        });
                    }

                    if (IsPrintTypeSupported(installedRibbon, MonoRibbonOptions)) {
                        Application.Current.Dispatcher.Invoke(() => {
                            viewModel.PrintTypes.Add(PrintType.MonoK);
                        });
                    }

                    if (IsPrintTypeSupported(installedRibbon, OverlayRibbonOptions)) {
                        Application.Current.Dispatcher.Invoke(() => {
                            viewModel.PrintTypes.Add(PrintType.Overlay);
                        });
                    }

                    string cardSourceRange = zebraCardPrinter.GetJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
                    if (cardSourceRange != null) {
                        foreach (CardSource source in Enum.GetValues(typeof(CardSource))) {
                            if (cardSourceRange.Contains(source.ToString())) {
                                Application.Current.Dispatcher.Invoke(() => {
                                    viewModel.Sources.Add(source.ToString());
                                });
                            }
                        }
                    }

                    string cardDestinationRange = zebraCardPrinter.GetJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
                    if (cardDestinationRange != null) {
                        foreach (CardDestination destination in Enum.GetValues(typeof(CardDestination))) {
                            if (cardDestinationRange.Contains(destination.ToString())) {
                                if (!destination.ToString().Contains("Laminator") || zebraCardPrinter.HasLaminator()) {
                                    Application.Current.Dispatcher.Invoke(() => {
                                        viewModel.Destinations.Add(destination.ToString());
                                    });
                                }
                            }
                        }
                    }

                    bool hasMagneticEncoder = zebraCardPrinter.HasMagneticEncoder();
                    if (hasMagneticEncoder) {
                        string coercivityTypeRange = zebraCardPrinter.GetJobSettingRange(ZebraCardJobSettingNames.MAG_COERCIVITY);
                        if (coercivityTypeRange != null) {
                            foreach (CoercivityType coercivityType in Enum.GetValues(typeof(CoercivityType))) {
                                if (coercivityTypeRange.Contains(coercivityType.ToString())) {
                                    Application.Current.Dispatcher.Invoke(() => {
                                        viewModel.CoercivityTypes.Add(coercivityType.ToString());
                                    });
                                }
                            }
                        }
                    }

                    Application.Current.Dispatcher.Invoke(() => {
                        viewModel.IsPrintOptimizationAvailable = zebraCardPrinter.GetJobSettings().Contains(ZebraCardJobSettingNames.PRINT_OPTIMIZATION);
                        viewModel.HasDualSidedPrintCapability = zebraCardPrinter.GetPrintCapability() == TransferType.DualSided;
                        viewModel.HasMagneticEncoder = hasMagneticEncoder;
                    });
                } else {
                    throw new ZebraCardException("No ribbon installed. Please install a ribbon and try again.");
                }
            }, (exception) => {
                string errorMessage = $"Error retrieving settings ranges: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.Sources.Clear();
                    viewModel.Destinations.Clear();
                    viewModel.PrintOptimizations.Clear();
                    viewModel.PrintTypes.Clear();
                    viewModel.CoercivityTypes.Clear();
                });
            }, null);
        }

        private bool IsJobValid(MultiJobControlViewModel jobViewModel) {
            // Check if print optimization mode is set if print optimization is available on the printer
            bool hasValidPrintOptimization = viewModel.IsPrintOptimizationAvailable ? jobViewModel.SelectedPrintOptimization != null : true;

            return jobViewModel.SelectedSource != null &&
                jobViewModel.SelectedDestination != null &&
                hasValidPrintOptimization &&
                jobViewModel.SelectedQuantity > 0 &&
                jobViewModel.HasValidJob;
        }

        private bool AreAnyJobsValid(List<MultiJobControlViewModel> jobViewModels) {
            foreach (MultiJobControlViewModel jobViewModel in jobViewModels) {
                if (IsJobValid(jobViewModel)) {
                    return true;
                }
            }
            return false;
        }

        private void SetJobSettings(ZebraCardPrinter zebraCardPrinter, MultiJobControlViewModel jobViewModel) {
            zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, jobViewModel.SelectedSource.ToString());
            zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, jobViewModel.SelectedDestination.ToString());

            if (viewModel.IsPrintOptimizationAvailable) {
                zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.PRINT_OPTIMIZATION, jobViewModel.SelectedPrintOptimization.ToString());
            }

            if (viewModel.HasMagneticEncoder) {
                zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.MAG_ENCODING_TYPE, MagEncodingType.ISO.ToString());

                if (jobViewModel.MagneticEncode) {
                    zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.MAG_COERCIVITY, jobViewModel.SelectedCoercivityType);
                }
            }
        }

        private GraphicsInfo BuildGraphicsInfo(ZebraCardImageI zebraCardImage, CardSide side, PrintType printType) {
            GraphicsInfo graphicsInfo = new GraphicsInfo {
                Side = side,
                PrintType = printType,
                GraphicType = zebraCardImage != null ? GraphicType.BMP : GraphicType.NA
            };

            if (zebraCardImage != null) {
                graphicsInfo.GraphicData = zebraCardImage;
            }
            return graphicsInfo;
        }

        private List<GraphicsInfo> CreateGraphicsInfo(ZebraCardGraphics graphics, Dictionary<PrintType, string> imageInfo, CardSide side) {
            List<GraphicsInfo> graphicsInfoList = new List<GraphicsInfo>();
            foreach (PrintType type in imageInfo.Keys) {
                graphics.Initialize(0, 0, OrientationType.Landscape, type, -1);

                if (type == PrintType.Overlay && imageInfo[type] == null) {
                    GraphicsInfo graphicsInfo = new GraphicsInfo {
                        Side = side,
                        PrintType = type,
                        GraphicType = GraphicType.NA
                    };
                    graphicsInfoList.Add(graphicsInfo);
                } else {
                    byte[] imageData = File.ReadAllBytes(imageInfo[type]);
                    graphics.DrawImage(imageData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
                    graphicsInfoList.Add(BuildGraphicsInfo(graphics.CreateImage(), side, type));
                }

                graphics.Clear();
            }

            return graphicsInfoList;
        }

        private int PrintAndMagEncode(ZebraCardPrinter zebraCardPrinter, MultiJobControlViewModel jobViewModel) {
            int jobId;

            ZebraCardGraphics graphics = null;
            try {
                graphics = new ZebraCardGraphics(zebraCardPrinter);

                List<GraphicsInfo> graphicsInfoList = new List<GraphicsInfo>();

                if (jobViewModel.HasValidFrontSide) {
                    foreach (GraphicsInfo graphicsInfo in CreateGraphicsInfo(graphics, jobViewModel.FrontSideGraphicFilenames, CardSide.Front)) {
                        graphicsInfoList.Add(graphicsInfo);
                    }
                }

                if (jobViewModel.HasValidBackSide) {
                    foreach (GraphicsInfo graphicsInfo in CreateGraphicsInfo(graphics, jobViewModel.BackSideGraphicFilenames, CardSide.Back)) {
                        graphicsInfoList.Add(graphicsInfo);
                    }
                }

                if (jobViewModel.HasValidMagEncodeJob) {
                    jobId = zebraCardPrinter.PrintAndMagEncode(jobViewModel.SelectedQuantity, graphicsInfoList, jobViewModel.Track1Data, jobViewModel.Track2Data, jobViewModel.Track3Data);
                } else {
                    jobId = zebraCardPrinter.Print(jobViewModel.SelectedQuantity, graphicsInfoList);
                }
            } finally {
                if (graphics != null) {
                    graphics.Close();
                }
            }

            return jobId;
        }

        private int MagEncode(ZebraCardPrinter zebraCardPrinter, MultiJobControlViewModel jobViewModel) {
            return zebraCardPrinter.MagEncode(jobViewModel.SelectedQuantity, jobViewModel.Track1Data, jobViewModel.Track2Data, jobViewModel.Track3Data);
        }

        private void SetUpAndSendJob(ZebraCardPrinter zebraCardPrinter, MultiJobControlViewModel jobViewModel) {
            SetJobSettings(zebraCardPrinter, jobViewModel);

            int? jobId = null;
            if (jobViewModel.HasValidFrontSide || jobViewModel.HasValidBackSide) {
                jobId = PrintAndMagEncode(zebraCardPrinter, jobViewModel);
            } else if (jobViewModel.HasValidMagEncodeJob) {
                jobId = MagEncode(zebraCardPrinter, jobViewModel);
            }

            if (jobId.HasValue) {
                jobViewModel.JobId = jobId.Value;
            }
        }

        private MultiJobControlViewModel SetSelectedFullOverlays(MultiJobControlViewModel viewModel) {
            if (viewModel.IsFrontSideFullOverlay) {
                if (viewModel.FrontSideGraphicFilenames.ContainsKey(PrintType.Overlay)) {
                    viewModel.FrontSideGraphicFilenames.Remove(PrintType.Overlay);
                }

                viewModel.FrontSideGraphicFilenames.Add(PrintType.Overlay, null);
            } else if (viewModel.FrontSideGraphicFilenames.ContainsKey(PrintType.Overlay) && viewModel.FrontSideGraphicFilenames[PrintType.Overlay] == null) {
                viewModel.FrontSideGraphicFilenames.Remove(PrintType.Overlay);
            }

            if (viewModel.IsBackSideFullOverlay) {
                if (viewModel.BackSideGraphicFilenames.ContainsKey(PrintType.Overlay)) {
                    viewModel.BackSideGraphicFilenames.Remove(PrintType.Overlay);
                }

                viewModel.BackSideGraphicFilenames.Add(PrintType.Overlay, null);
            } else if (viewModel.BackSideGraphicFilenames.ContainsKey(PrintType.Overlay) && viewModel.BackSideGraphicFilenames[PrintType.Overlay] == null) {
                viewModel.BackSideGraphicFilenames.Remove(PrintType.Overlay);
            }
            return viewModel;
        }

        private async void SendJobsButton_Click(object sender, RoutedEventArgs e) {
            JobStatusControl.ClearLog();
            viewModel.IsSendJobsButtonEnabled = false;

            bool success = true;
            List<MultiJobControlViewModel> jobViewModels = new List<MultiJobControlViewModel> {
                SetSelectedFullOverlays(Job1Control.ViewModel),
                SetSelectedFullOverlays(Job2Control.ViewModel),
                SetSelectedFullOverlays(Job3Control.ViewModel),
                SetSelectedFullOverlays(Job4Control.ViewModel)
            };

            jobViewModels = jobViewModels.Where((viewModel) => {
                if (IsJobValid(viewModel)) {
                    return true;
                } else {
                    UpdateJobStatus(viewModel.JobNumber, "Not configured");
                    return false;
                }
            }).ToList();

            await printerManager.PerformAction("Sending jobs...", (zebraCardPrinter, connection) => {
                if (AreAnyJobsValid(jobViewModels)) {
                    if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                        JobStatusControl.UpdateLog("Setting up jobs...");

                        foreach (MultiJobControlViewModel jobViewModel in jobViewModels) {
                            SetUpAndSendJob(zebraCardPrinter, jobViewModel);
                        }
                    } else {
                        success = false;
                    }
                } else {
                    throw new ZebraCardException("No jobs configured");
                }
            }, (exception) => {
                success = false;

                string errorMessage = $"Error sending jobs: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            });

            if (success) {
                List<JobInfo> jobInfoList = new List<JobInfo>();
                foreach (MultiJobControlViewModel jobViewModel in jobViewModels) {
                    if (jobViewModel.JobId.HasValue) {
                        CardSource cardSource = (CardSource)Enum.Parse(typeof(CardSource), jobViewModel.SelectedSource);
                        jobInfoList.Add(new JobInfo(jobViewModel.JobNumber, jobViewModel.JobId.Value, cardSource));
                    }
                }

                await JobStatusControl.StartPolling(printerManager.Printer, jobInfoList);
            }

            viewModel.IsSendJobsButtonEnabled = true;
        }

        private async void CancelJobButton_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(viewModel.CancellableJobId)) {
                MessageBoxHelper.ShowError("No job ID specified");
                return;
            }

            bool success = true;

            await printerManager.PerformAction($"Cancelling job ID {viewModel.CancellableJobId}...", (zebraCardPrinter, connection) => {
                zebraCardPrinter.Cancel(int.Parse(viewModel.CancellableJobId));
            }, (exception) => {
                success = false;

                string errorMessage = $"Error cancelling job ID {viewModel.CancellableJobId}: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            }, () => {
                if (success) {
                    JobStatusControl.UpdateLog($"Cancel job ID {viewModel.CancellableJobId} command sent successfully.");
                }
            });
        }

        private async void CancelAllButton_Click(object sender, RoutedEventArgs e) {
            bool success = true;

            await printerManager.PerformAction("Cancelling all jobs...", (zebraCardPrinter, connection) => {
                zebraCardPrinter.Cancel(0);
            }, (exception) => {
                success = false;

                string errorMessage = $"Error cancelling all jobs: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            }, () => {
                if (success) {
                    JobStatusControl.UpdateLog("Cancel all jobs command sent successfully.");
                }
            });
        }
    }
}
