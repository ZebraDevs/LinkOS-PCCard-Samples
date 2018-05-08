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

using DeveloperDemo_Card_Desktop.Demos.GraphicConversion;
using DeveloperDemo_Card_Desktop.Demos.MagEncode;
using DeveloperDemo_Card_Desktop.Demos.MultiJob;
using DeveloperDemo_Card_Desktop.Demos.Print;
using DeveloperDemo_Card_Desktop.Demos.PrinterStatus;
using DeveloperDemo_Card_Desktop.Demos.Settings;
using DeveloperDemo_Card_Desktop.Demos.SmartCard;
using DeveloperDemo_Card_Desktop.Demos.Template;
using DeveloperDemo_Card_Desktop.Discovery;
using DeveloperDemo_Card_Desktop.UserControls;
using DeveloperDemo_Card_Desktop.Utils;
using System.Windows;
using System.Windows.Input;
using Zebra.Sdk.Printer.Discovery;

namespace DeveloperDemo_Card_Desktop {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private MainWindowViewModel viewModel;
        private PrinterManager printerManager = new PrinterManager();
        private NoDemoSelectedControl noDemoSelectedControl = new NoDemoSelectedControl();

        public MainWindow() {
            InitializeComponent();

            viewModel = DataContext as MainWindowViewModel;
            printerManager.PrinterSelected += PrinterManager_PrinterSelected;
            printerManager.ConnectionReestablished += PrinterManager_ConnectionReestablished;
        }

        private void PrinterManager_PrinterSelected(DiscoveredPrinter printer) {
            Application.Current.Dispatcher.Invoke(() => {
                DemoContentControl.Content = noDemoSelectedControl;
                viewModel.SelectedPrinter = printer;
            });
        }

        private void PrinterManager_ConnectionReestablished() {
            DemoContentControl.Content = new SettingsDemo(printerManager);
        }

        private void DiscoverButton_Click(object sender, RoutedEventArgs e) {
            new PrinterDiscoveryDialog(printerManager).ShowDialog();
        }

        private void ManualConnectButton_Click(object sender, RoutedEventArgs e) {
            new ManualConnectionDialog(printerManager).ShowDialog();
        }

        private void GraphicConversionDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new GraphicConversionDemo(printerManager);
        }

        private void MagEncodeDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new MagEncodeDemo(printerManager);
        }

        private void MultiJobDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new MultiJobDemo(printerManager);
        }

        private void PrintDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new PrintDemo(printerManager);
        }

        private void PrinterStatusDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new PrinterStatusDemo(printerManager);
        }

        private void SettingsDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new SettingsDemo(printerManager);
        }

        private void SmartCardDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new SmartCardDemo(printerManager);
        }

        private void TemplateDemoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            DemoContentControl.Content = new TemplateDemo(printerManager);
        }
    }
}
