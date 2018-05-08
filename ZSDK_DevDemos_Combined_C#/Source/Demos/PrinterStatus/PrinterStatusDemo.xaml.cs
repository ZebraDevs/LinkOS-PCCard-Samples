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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;

namespace DeveloperDemo_Card_Desktop.Demos.PrinterStatus {

    /// <summary>
    /// Interaction logic for PrinterStatusDemo.xaml
    /// </summary>
    public partial class PrinterStatusDemo : UserControl {

        private PrinterStatusDemoViewModel viewModel;
        private PrinterManager printerManager;

        public PrinterStatusDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as PrinterStatusDemoViewModel;
            this.printerManager = printerManager;

            RetrieveSettings();
        }

        private async void RetrieveSettings() {
            viewModel.PrinterSettings.Clear();
            viewModel.GeneralSettings.Clear();
            viewModel.MediaSettings.Clear();
            viewModel.SensorSettings.Clear();

            await printerManager.PerformAction("Retrieving settings...", (zebraCardPrinter, connection) => {
                List<PrinterSetting> printerSettingList = BuildPrinterSettingList(zebraCardPrinter.GetPrinterInformation());
                List<PrinterSetting> generalSettingList = BuildGeneralSettingList(zebraCardPrinter.GetPrinterStatus());
                List<MediaInfo> mediaInfoList = zebraCardPrinter.GetMediaInformation();
                Dictionary<string, string> sensorStateList = zebraCardPrinter.GetSensorStates();
                Dictionary<string, string> sensorValueList = zebraCardPrinter.GetSensorValues();

                Application.Current.Dispatcher.Invoke(() => {
                    foreach (PrinterSetting printerSetting in printerSettingList) { viewModel.PrinterSettings.Add(printerSetting); }
                    foreach (PrinterSetting printerSetting in generalSettingList) { viewModel.GeneralSettings.Add(printerSetting); }
                    foreach (MediaInfo mediaInfo in mediaInfoList) { viewModel.MediaSettings.Add(mediaInfo); }
                    foreach (var entry in sensorStateList) { viewModel.SensorSettings.Add(new PrinterSetting(entry.Key, entry.Value)); }
                    foreach (var entry in sensorValueList) { viewModel.SensorSettings.Add(new PrinterSetting(entry.Key, entry.Value)); }
                });
            }, (exception) => {
                MessageBoxHelper.ShowError($"Error retrieving settings: {exception.Message}");

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.PrinterSettings.Clear();
                    viewModel.GeneralSettings.Clear();
                    viewModel.MediaSettings.Clear();
                    viewModel.SensorSettings.Clear();
                });
            }, null);
        }

        private List<PrinterSetting> BuildPrinterSettingList(PrinterInfo printerInfo) {
            return new List<PrinterSetting> {
                new PrinterSetting("Vendor", printerInfo.Vendor),
                new PrinterSetting("Model", printerInfo.Model),
                new PrinterSetting("Serial Number", printerInfo.SerialNumber),
                new PrinterSetting("MAC Address", printerInfo.MacAddress),
                new PrinterSetting("Printhead Serial Number", printerInfo.PrintheadSerialNumber),
                new PrinterSetting("OEM Code", printerInfo.OemCode),
                new PrinterSetting("Firmware Version", printerInfo.FirmwareVersion),
                new PrinterSetting("Media Version", printerInfo.MediaVersion),
                new PrinterSetting("Heater Version", printerInfo.HeaterVersion),
                new PrinterSetting("ZMotif Version", printerInfo.ZMotifVersion)
            };
        }

        private List<PrinterSetting> BuildGeneralSettingList(PrinterStatusInfo printerStatusInfo) {
            return new List<PrinterSetting> {
                new PrinterSetting("Status", printerStatusInfo.Status),
                new PrinterSetting("Jobs Pending", printerStatusInfo.JobsPending),
                new PrinterSetting("Jobs Active", printerStatusInfo.JobsActive),
                new PrinterSetting("Jobs Complete", printerStatusInfo.JobsComplete),
                new PrinterSetting("Job Errors", printerStatusInfo.JobErrors),
                new PrinterSetting("Jobs Cancelled", printerStatusInfo.JobsCancelled),
                new PrinterSetting("Jobs Total", printerStatusInfo.JobsTotal),
                new PrinterSetting("Next Job ID", printerStatusInfo.NextJobId),
                new PrinterSetting("Alarm Info", printerStatusInfo.AlarmInfo.Value != 0 ? printerStatusInfo.AlarmInfo.Value + ":" + printerStatusInfo.AlarmInfo.Description : "None"),
                new PrinterSetting("Error Info", printerStatusInfo.ErrorInfo.Value != 0 ? printerStatusInfo.ErrorInfo.Value + ":" + printerStatusInfo.ErrorInfo.Description : "None"),
            };
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            RetrieveSettings();
        }
    }
}
