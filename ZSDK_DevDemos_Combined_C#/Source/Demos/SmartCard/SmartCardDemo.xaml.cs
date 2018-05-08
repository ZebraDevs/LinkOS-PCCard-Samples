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
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Job;
using static DeveloperDemo_Card_Desktop.UserControls.JobStatusControl;

namespace DeveloperDemo_Card_Desktop.Demos.SmartCard {

    /// <summary>
    /// Interaction logic for SmartCardDemo.xaml
    /// </summary>
    public partial class SmartCardDemo : UserControl {

        private SmartCardDemoViewModel viewModel;
        private PrinterManager printerManager;

        public SmartCardDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as SmartCardDemoViewModel;
            this.printerManager = printerManager;

            RetrieveSettingsRanges();
        }

        private async void RetrieveSettingsRanges() {
            viewModel.Sources.Clear();
            viewModel.Destinations.Clear();
            viewModel.EncoderTypes.Clear();

            await printerManager.PerformAction("Retrieving settings ranges...", (zebraCardPrinter, connection) => {
                bool hasLaminator = zebraCardPrinter.HasLaminator();

                if (zebraCardPrinter.HasSmartCardEncoder()) {
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

                    Dictionary<string, string> smartCardConfigurations = zebraCardPrinter.GetSmartCardConfigurations();
                    foreach (string encoderType in smartCardConfigurations.Keys) {
                        Application.Current.Dispatcher.Invoke(() => {
                            viewModel.EncoderTypes.Add(encoderType);
                        });
                    }
                } else {
                    MessageBoxHelper.ShowError("Unable to proceed with demo because no smart card encoder was found.");
                }
            }, (exception) => {
                string errorMessage = $"Error retrieving settings ranges: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.Sources.Clear();
                    viewModel.Destinations.Clear();
                    viewModel.EncoderTypes.Clear();
                });
            }, null);
        }

        private async void StartJobButton_Click(object sender, RoutedEventArgs e) {
            int? jobId = null;
            CardSource? cardSource = null;

            JobStatusControl.ClearLog();

            await printerManager.PerformAction("Starting smart card operation...", (zebraCardPrinter, connection) => {
                if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                    cardSource = (CardSource)Enum.Parse(typeof(CardSource), viewModel.SelectedSource);

                    zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, cardSource.ToString());
                    zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, viewModel.SelectedDestination.ToString());

                    bool isEncoderTypeContact = viewModel.SelectedEncoderType.Equals("contact", StringComparison.OrdinalIgnoreCase) || viewModel.SelectedEncoderType.Equals("contact_station", StringComparison.OrdinalIgnoreCase);
                    string settingName = isEncoderTypeContact ? ZebraCardJobSettingNames.SMART_CARD_CONTACT : ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS;
                    string settingValue = isEncoderTypeContact ? "yes" : viewModel.SelectedEncoderType;

                    zebraCardPrinter.SetJobSetting(settingName, settingValue);
                    jobId = zebraCardPrinter.SmartCardEncode(1);
                }
            }, (exception) => {
                string errorMessage = $"Error sending smart card job: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            });

            if (jobId.HasValue && cardSource.HasValue) {
                await JobStatusControl.StartPolling(printerManager.Printer, new JobInfo(jobId.Value, cardSource.Value));
            }
        }
    }
}
