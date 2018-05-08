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

using DeveloperDemo_Card_Desktop.Dialogs;
using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Job;
using static DeveloperDemo_Card_Desktop.UserControls.JobStatusControl;

namespace DeveloperDemo_Card_Desktop.Demos.MagEncode {

    /// <summary>
    /// Interaction logic for MagEncodeDemo.xaml
    /// </summary>
    public partial class MagEncodeDemo : UserControl {

        private MagEncodeDemoViewModel viewModel;
        private PrinterManager printerManager;

        public MagEncodeDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as MagEncodeDemoViewModel;
            this.printerManager = printerManager;

            JobTypesDropdown.SelectionChanged += JobTypesDropdown_SelectionChanged;

            RetrieveSettingsRanges();
        }

        private void JobTypesDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Track1DataInput.Text = "";
            Track2DataInput.Text = "";
            Track3DataInput.Text = "";
        }

        private async void RetrieveSettingsRanges() {
            viewModel.Sources.Clear();
            viewModel.Destinations.Clear();
            viewModel.CoercivityTypes.Clear();

            await printerManager.PerformAction("Retrieving settings ranges...", (zebraCardPrinter, connection) => {
                bool hasMagEncoder = zebraCardPrinter.HasMagneticEncoder();
                bool hasLaminator = zebraCardPrinter.HasLaminator();

                if (hasMagEncoder) {
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
                                if (!destination.ToString().Contains("Laminator") || hasLaminator) {
                                    Application.Current.Dispatcher.Invoke(() => {
                                        viewModel.Destinations.Add(destination.ToString());
                                    });
                                }
                            }
                        }
                    }

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
                } else {
                    MessageBoxHelper.ShowError("Unable to proceed with demo because no magnetic encoder was found.");
                }
            }, (exception) => {
                string errorMessage = $"Error retrieving settings ranges: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.Sources.Clear();
                    viewModel.Destinations.Clear();
                    viewModel.CoercivityTypes.Clear();
                });
            }, null);
        }

        private async Task ReadMagEncodeData() {
            MessageDialog insertCardDialog = null;

            viewModel.Track1Data = "";
            viewModel.Track2Data = "";
            viewModel.Track3Data = "";
            JobStatusControl.ClearLog();

            Console.SetOut(new TextBoxTextWriter(JobStatusControl.JobStatusLog));

            await printerManager.PerformAction("Reading mag encode data...", (zebraCardPrinter, connection) => {
                if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                    Console.WriteLine(); // Start logging on new line after printer ready check

                    CardSource cardSource = (CardSource)Enum.Parse(typeof(CardSource), viewModel.SelectedSource);

                    Dictionary<string, string> jobSettings = new Dictionary<string, string>();
                    jobSettings.Add(ZebraCardJobSettingNames.CARD_SOURCE, cardSource.ToString());
                    jobSettings.Add(ZebraCardJobSettingNames.CARD_DESTINATION, viewModel.SelectedDestination.ToString());

                    zebraCardPrinter.SetJobSettings(jobSettings);

                    if (cardSource == CardSource.ATM) {
                        insertCardDialog = DialogHelper.ShowInsertCardDialog();
                    }

                    MagTrackData magTrackData = zebraCardPrinter.ReadMagData(DataSource.Track1 | DataSource.Track2 | DataSource.Track3, true);

                    if (string.IsNullOrEmpty(magTrackData.Track1) && string.IsNullOrEmpty(magTrackData.Track2) && string.IsNullOrEmpty(magTrackData.Track3)) {
                        Console.WriteLine("No data read from card.");
                    }

                    Application.Current.Dispatcher.Invoke(() => {
                        viewModel.Track1Data = magTrackData.Track1;
                        viewModel.Track2Data = magTrackData.Track2;
                        viewModel.Track3Data = magTrackData.Track3;
                    });
                }
            }, (exception) => {
                string errorMessage = $"Error reading mag encode data: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                Console.WriteLine(errorMessage);
            });

            if (insertCardDialog != null) {
                insertCardDialog.Close();
            }

            StreamWriter streamWriter = new StreamWriter(Console.OpenStandardOutput());
            streamWriter.AutoFlush = true;
            Console.SetOut(streamWriter);
        }

        private async Task WriteMagEncodeData() {
            int? jobId = null;
            CardSource? cardSource = null;

            JobStatusControl.ClearLog();

            await printerManager.PerformAction("Writing mag encode data...", (zebraCardPrinter, connection) => {
                if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                    cardSource = (CardSource)Enum.Parse(typeof(CardSource), viewModel.SelectedSource);

                    Dictionary<string, string> jobSettings = new Dictionary<string, string>();
                    jobSettings.Add(ZebraCardJobSettingNames.CARD_SOURCE, cardSource.ToString());
                    jobSettings.Add(ZebraCardJobSettingNames.CARD_DESTINATION, viewModel.SelectedDestination.ToString());
                    jobSettings.Add(ZebraCardJobSettingNames.MAG_COERCIVITY, viewModel.SelectedCoercivityType.ToString());
                    jobSettings.Add(ZebraCardJobSettingNames.MAG_VERIFY, viewModel.VerifyEncoding ? "yes" : "no");

                    zebraCardPrinter.SetJobSettings(jobSettings);

                    jobId = zebraCardPrinter.MagEncode(1, viewModel.Track1Data, viewModel.Track2Data, viewModel.Track3Data);
                }
            }, (exception) => {
                string errorMessage = $"Error writing mag encode data: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            });

            if (jobId.HasValue && cardSource.HasValue) {
                await JobStatusControl.StartPolling(printerManager.Printer, new JobInfo(jobId.Value, cardSource.Value));
            }
        }

        private async void ReadWriteButton_Click(object sender, RoutedEventArgs e) {
            if (viewModel.SelectedJobType == MagEncodeJobType.Read) {
                await ReadMagEncodeData();
            } else if (viewModel.SelectedJobType == MagEncodeJobType.Write) {
                await WriteMagEncodeData();
            }
        }
    }
}
