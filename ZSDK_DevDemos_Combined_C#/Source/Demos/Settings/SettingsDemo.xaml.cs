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
using DeveloperDemo_Card_Desktop.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DeveloperDemo_Card_Desktop.Demos.Settings {

    /// <summary>
    /// Interaction logic for SettingsDemo.xaml
    /// </summary>
    public partial class SettingsDemo : UserControl {

        private SettingsDemoViewModel viewModel;
        private PrinterManager printerManager;

        public SettingsDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as SettingsDemoViewModel;
            this.printerManager = printerManager;

            RetrieveSettings();
        }

        public async void RetrieveSettings() {
            viewModel.DeviceSettings.Clear();
            viewModel.PrintSettings.Clear();

            await printerManager.PerformAction("Retrieving settings...", (zebraCardPrinter, connection) => {
                Dictionary<string, string> deviceSettings = zebraCardPrinter.GetAllSettingValues();
                Dictionary<string, string> printSettings = zebraCardPrinter.GetAllJobSettingValues();

                Application.Current.Dispatcher.Invoke(() => {
                    foreach (var entry in deviceSettings) { viewModel.DeviceSettings.Add(new PrinterSetting(entry.Key, entry.Value)); }
                    foreach (var entry in printSettings) { viewModel.PrintSettings.Add(new PrinterSetting(entry.Key, entry.Value)); }
                });
            }, (exception) => {
                MessageBoxHelper.ShowError($"Error retrieving settings: {exception.Message}");

                Application.Current.Dispatcher.Invoke(() => {
                    viewModel.DeviceSettings.Clear();
                    viewModel.PrintSettings.Clear();
                });
            }, null);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            RetrieveSettings();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e) {
            PrinterSetting printerSetting = ((FrameworkElement)sender).DataContext as PrinterSetting;
            new SetSettingDialog(this, printerManager, printerSetting).ShowDialog();
        }
    }
}
