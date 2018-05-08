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

using DeveloperDemo_Card_Desktop.Demos.PrinterStatus;
using DeveloperDemo_Card_Desktop.Dialogs;
using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zebra.Sdk.Card.Settings;
using Zebra.Sdk.Card.Zmotif.Settings;

namespace DeveloperDemo_Card_Desktop.Demos.Settings {

    /// <summary>
    /// Interaction logic for SetSettingDialog.xaml
    /// </summary>
    public partial class SetSettingDialog : Window {

        private static readonly string[] NetworkResetRequiredSettingsList = new string[] {
                ZebraCardSettingNamesZmotif.WIRED_SNMP,
                ZebraCardSettingNames.WIRED_DHCP,
                ZebraCardSettingNames.WIRED_ADDRESS,
                ZebraCardSettingNames.WIRED_SUBNET,
                ZebraCardSettingNames.WIRED_GATEWAY,
                ZebraCardSettingNamesZmotif.WIRED_DNS_NAME,
                ZebraCardSettingNamesZmotif.WIRELESS_SNMP,
                ZebraCardSettingNamesZmotif.WIRELESS_DHCP,
                ZebraCardSettingNamesZmotif.WIRELESS_ADDRESS,
                ZebraCardSettingNamesZmotif.WIRELESS_SUBNET,
                ZebraCardSettingNamesZmotif.WIRELESS_GATEWAY
        };

        private static readonly string[] PrinterResetRequiredSettingsList = new string[] {
                ZebraCardSettingNamesZmotif.OCP_LANGUAGE_TYPE,
                ZebraCardSettingNamesZmotif.OCP_LANGUAGE_NAME,
                ZebraCardSettingNamesZmotif.STANDBY_TIMEOUT
        };

        private static readonly string[] DisableWiredDhcpSettingsList = new string[] {
                ZebraCardSettingNames.WIRED_ADDRESS,
                ZebraCardSettingNames.WIRED_SUBNET,
                ZebraCardSettingNames.WIRED_GATEWAY
        };

        private static readonly string[] DisableWirelessDhcpSettingsList = new string[] {
                ZebraCardSettingNamesZmotif.WIRELESS_ADDRESS,
                ZebraCardSettingNamesZmotif.WIRELESS_SUBNET,
                ZebraCardSettingNamesZmotif.WIRELESS_GATEWAY
        };

        private SetSettingsDialogViewModel viewModel;
        private SettingsDemo settingsDemo;
        private PrinterManager printerManager;

        public SetSettingDialog(SettingsDemo settingsDemo, PrinterManager printerManager, PrinterSetting printerSetting) {
            InitializeComponent();

            viewModel = DataContext as SetSettingsDialogViewModel;
            this.settingsDemo = settingsDemo;
            this.printerManager = printerManager;
            viewModel.SettingName = printerSetting.Setting;
            viewModel.OldSettingValue = printerSetting.Value;
            viewModel.NewSettingValue = printerSetting.Value;

            UpdateRangeAndType();
        }

        public async void UpdateRangeAndType() {
            await printerManager.PerformAction($"Getting range and type for setting {viewModel.SettingName}...", (zebraCardPrinter, connection) => {
                // Settings can only be set on device settings within the settings demo, so no need to call GetJobSettingRange() and GetJobSettingType() for print settings
                string range = zebraCardPrinter.GetSettingRange(viewModel.SettingName);
                string type = zebraCardPrinter.GetSettingType(viewModel.SettingName);

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.Range = range;
                    viewModel.Type = type;
                });
            }, (exception) => {
                MessageBoxHelper.ShowError($"Error getting range and type for setting {exception.Message}");
            });
        }

        private async void SetButton_Click(object sender, RoutedEventArgs e) {
            string changedSetting = viewModel.SettingName;
            bool isResetRequired = NetworkResetRequiredSettingsList.Contains(changedSetting) || PrinterResetRequiredSettingsList.Contains(changedSetting);
            bool resetPrinter = PrinterResetRequiredSettingsList.Contains(changedSetting);
            bool success = true;

            await printerManager.PerformAction($"Setting value for setting {changedSetting}...", (zebraCardPrinter, connection) => {
                if (DisableWiredDhcpSettingsList.Contains(changedSetting)) {
                    zebraCardPrinter.SetSetting(ZebraCardSettingNames.WIRED_DHCP, "disabled");
                }

                if (DisableWirelessDhcpSettingsList.Contains(changedSetting)) {
                    zebraCardPrinter.SetSetting(ZebraCardSettingNamesZmotif.WIRELESS_DHCP, "disabled");
                }

                zebraCardPrinter.SetSetting(changedSetting, viewModel.NewSettingValue);
            }, (exception) => {
                success = false;
                
                MessageBoxHelper.ShowError($"Error setting value for setting {viewModel.SettingName}: {exception.Message}");
            }, () => {
                if (success) {
                    Application.Current.Dispatcher.Invoke(() => {
                        Close();
                    });

                    if (isResetRequired) {
                        Application.Current.Dispatcher.Invoke(() => {
                            string resetButtonName = "Reset";
                            string resetTypeString = resetPrinter ? "printer" : "network";
                            MessageDialog resetDialog = new MessageDialog {
                                Title = "Reset Required",
                                Message = $"The setting {viewModel.SettingName} requires a {resetTypeString} reset for any changes to take effect. Click {resetButtonName} to reset the {resetTypeString}.",
                                PositiveButtonText = resetButtonName
                            };
                            resetDialog.PositiveButtonClicked += async () => {
                                await printerManager.ReestablishConnection(resetPrinter);
                            };
                            resetDialog.ShowDialog();
                        });
                    } else {
                        settingsDemo.RetrieveSettings();
                    }
                }
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
