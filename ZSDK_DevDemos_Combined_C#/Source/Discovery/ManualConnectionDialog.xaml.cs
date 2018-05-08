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
using System.Threading.Tasks;
using System.Windows;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Printer.Discovery;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Settings;

namespace DeveloperDemo_Card_Desktop.Discovery {

    /// <summary>
    /// Interaction logic for ManualConnectionDialog.xaml
    /// </summary>
    public partial class ManualConnectionDialog : Window {

        private PrinterManager printerManager;

        public ManualConnectionDialog(PrinterManager printerManager) {
            InitializeComponent();

            this.printerManager = printerManager;
        }

        private TcpConnection GetTcpConnection(string connectionText) {
            int colonIndex = connectionText.IndexOf(":");
            if (colonIndex != -1) {
                string ipAddress = connectionText.Substring(0, colonIndex);
                int portNumber = int.Parse(connectionText.Substring(colonIndex + 1));
                return new TcpConnection(ipAddress, portNumber);
            } else {
                return new TcpConnection(connectionText, 9100);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            string ipAddressText = IpAddressInput.Text;

            IndeterminateProgressDialog indeterminateProgressDialog = new IndeterminateProgressDialog($"Connecting to {ipAddressText}...");

            Task.Run(() => {
                Application.Current.Dispatcher.Invoke(() => {
                    indeterminateProgressDialog.ShowDialog();
                });
            });

            Task.Run(() => {
                TcpConnection connection = null;
                ZebraCardPrinter zebraCardPrinter = null;

                try {
                    connection = GetTcpConnection(ipAddressText);
                    connection.Open();

                    zebraCardPrinter = ZebraCardPrinterFactory.GetInstance(connection);

                    string model = zebraCardPrinter.GetPrinterInformation().Model;
                    if (model != null) {
                        if (!model.ToLower().Contains("zxp1") && !model.ToLower().Contains("zxp3")) {
                            printerManager.Printer = new DiscoveredCardPrinterNetwork(DiscoveryUtilCard.GetDiscoveryDataMap(connection));

                            Application.Current.Dispatcher.Invoke(() => {
                                Close();
                            });
                        } else {
                            throw new ConnectionException("Printer model not supported");
                        }
                    } else {
                        throw new SettingsException("No printer model found");
                    }
                } catch (Exception exception) {
                    MessageBoxHelper.ShowError($"Error connecting to printer {ipAddressText}: {exception.Message}");
                } finally {
                    ConnectionHelper.CleanUpQuietly(zebraCardPrinter, connection);

                    Application.Current.Dispatcher.Invoke(() => {
                        indeterminateProgressDialog.Close();
                    });
                }
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
