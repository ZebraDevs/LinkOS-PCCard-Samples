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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Printer.Discovery;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace DeveloperDemo_Card_Desktop.Discovery {

    /// <summary>
    /// Interaction logic for PrinterDiscoveryDialog.xaml
    /// </summary>
    public partial class PrinterDiscoveryDialog : Window {

        private PrinterDiscoveryDialogViewModel viewModel;
        private PrinterManager printerManager;
        private CountdownEvent countdownEvent = new CountdownEvent(1);

        public PrinterDiscoveryDialog(PrinterManager printerManager) {
            InitializeComponent();

            this.printerManager = printerManager;
            viewModel = DataContext as PrinterDiscoveryDialogViewModel;

            DiscoverPrinters();
        }

        private void DiscoverPrinters() {
            viewModel.DiscoveredPrinters.Clear();
            viewModel.IsDiscovering = true;

            Task.Run(() => {
                try {
                    List<DiscoveredUsbPrinter> usbPrinters = UsbDiscoverer.GetZebraUsbPrinters(new ZebraCardPrinterFilter());
                    foreach (DiscoveredUsbPrinter printer in usbPrinters) {
                        Application.Current.Dispatcher.Invoke(() => {
                            viewModel.DiscoveredPrinters.Add(printer);
                        });
                    }

                    NetworkCardDiscoverer.FindPrinters(new NetworkCardDiscoveryHandler(this));
                    countdownEvent.Wait();
                } catch (Exception e) {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageBoxHelper.ShowError($"Error discovering printers: {e.Message}");
                    });
                } finally {
                    Application.Current.Dispatcher.Invoke(() => {
                        viewModel.IsDiscovering = false;
                    });
                }
            });
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            DiscoveredPrinter printer = viewModel.SelectedPrinter;
            string address = printer.Address;

            IndeterminateProgressDialog indeterminateProgressDialog = new IndeterminateProgressDialog($"Connecting to {address}...");

            Task.Run(() => {
                Application.Current.Dispatcher.Invoke(() => {
                    indeterminateProgressDialog.ShowDialog();
                });
            });

            Task.Run(() => {
                Connection connection = null;
                ZebraCardPrinter zebraCardPrinter = null;

                try {
                    connection = printer.GetConnection();
                    connection.Open();

                    zebraCardPrinter = ZebraCardPrinterFactory.GetInstance(connection);
                    printerManager.Printer = printer;

                    Application.Current.Dispatcher.Invoke(() => {
                        Close();
                    });
                } catch (Exception exception) {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageBoxHelper.ShowError($"Error connecting to printer {address}: {exception.Message}");
                    });
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

        class NetworkCardDiscoveryHandler : DiscoveryHandler {

            private PrinterDiscoveryDialog printerDiscoveryDialog;

            public NetworkCardDiscoveryHandler(PrinterDiscoveryDialog printerDiscoveryDialog) {
                this.printerDiscoveryDialog = printerDiscoveryDialog;
            }

            public void DiscoveryError(string message) {
                printerDiscoveryDialog.countdownEvent.Signal();
            }

            public void DiscoveryFinished() {
                printerDiscoveryDialog.countdownEvent.Signal();
            }

            public void FoundPrinter(DiscoveredPrinter printer) {
                Application.Current.Dispatcher.Invoke(() => {
                    printerDiscoveryDialog.viewModel.DiscoveredPrinters.Add(printer);
                });
            }
        }
    }
}
