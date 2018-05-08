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
using DeveloperDemo_Card_Desktop.Discovery;
using DeveloperDemo_Card_Desktop.UserControls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zebra.Sdk.Card.Comm;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace DeveloperDemo_Card_Desktop.Utils {

    public class PrinterManager {

        private const int PrinterResetWaitTime = 5000;
        private const long NetworkReestablishmentTimeout = 30000;

        private DiscoveredPrinter printer;

        public delegate void PrinterSelectedHandler(DiscoveredPrinter printer);
        public delegate void ConnectionReestablishedHandler();

        public event PrinterSelectedHandler PrinterSelected;
        public event ConnectionReestablishedHandler ConnectionReestablished;

        public DiscoveredPrinter Printer {
            get => printer;
            set {
                printer = value;

                if (PrinterSelected != null) {
                    PrinterSelected.Invoke(printer);
                }
            }
        }

        public async Task PerformAction(string progressMessage, Action<ZebraCardPrinter, Connection> onConnectionOpened, Action<Exception> onException) {
            await PerformAction(progressMessage, onConnectionOpened, onException, null);
        }

        public async Task PerformAction(string progressMessage, Action<ZebraCardPrinter, Connection> onConnectionOpened, Action<Exception> onException, Action onFinish) {
            IndeterminateProgressDialog indeterminateProgressDialog = new IndeterminateProgressDialog(progressMessage);

            new Thread(() => {
                Application.Current.Dispatcher.Invoke(() => {
                    indeterminateProgressDialog.ShowDialog();
                });
            }).Start();

            await Task.Run(() => {
                Connection connection = null;
                ZebraCardPrinter zebraCardPrinter = null;

                try {
                    connection = printer.GetConnection();
                    connection.Open();

                    zebraCardPrinter = ZebraCardPrinterFactory.GetInstance(connection);

                    if (onConnectionOpened != null) {
                        onConnectionOpened.Invoke(zebraCardPrinter, connection);
                    }
                } catch (Exception exception) {
                    if (onException != null) {
                        onException.Invoke(exception);
                    }
                } finally {
                    ConnectionHelper.CleanUpQuietly(zebraCardPrinter, connection);

                    Application.Current.Dispatcher.Invoke(() => {
                        indeterminateProgressDialog.Close();
                    });
                }
            });

            if (onFinish != null) {
                onFinish.Invoke();
            }
        }

        public async Task ReestablishConnection(bool resetPrinter) {
            ZebraPrinterZmotif zebraPrinterZmotif = null;

            await PerformAction("Reconnecting to printer...", (zebraCardPrinter, connection) => {
                DiscoveredPrinter oldPrinter = Printer;

                Application.Current.Dispatcher.Invoke(() => {
                    Printer = null;
                });

                zebraPrinterZmotif = ZebraCardPrinterFactory.GetZmotifPrinter(connection);

                if (resetPrinter) {
                    zebraPrinterZmotif.Reset();
                } else {
                    zebraPrinterZmotif.ResetNetwork();
                }

                Thread.Sleep(PrinterResetWaitTime);

                ReconnectionHandler handler = new ReconnectionHandler(this, oldPrinter);
                handler.ReconnectionFinished += (printer) => {
                    Application.Current.Dispatcher.Invoke(() => {
                        Printer = printer;

                        if (ConnectionReestablished != null) {
                            ConnectionReestablished.Invoke();
                        }
                    });
                };

                CardConnectionReestablisher connectionReestablisher = connection.GetConnectionReestablisher(NetworkReestablishmentTimeout) as CardConnectionReestablisher;
                connectionReestablisher.ReestablishConnection(handler);
            }, (exception) => {
                MessageBoxHelper.ShowError($"Error reestablishing connection: {exception.Message}");
            }, () => {
                try {
                    if (zebraPrinterZmotif != null) {
                        zebraPrinterZmotif.Destroy();
                    }
                } catch (Exception) {
                    // Do nothing
                }
            });
        }

        public bool IsPrinterReady(ZebraCardPrinter zebraCardPrinter, JobStatusControl jobStatusControl) {
            PrinterStatusInfo statusInfo = zebraCardPrinter.GetPrinterStatus();

            jobStatusControl.UpdateLog("Checking printer status...");

            if (statusInfo.ErrorInfo.Value > 0) {
                jobStatusControl.UpdateLog($"{statusInfo.Status} ({statusInfo.ErrorInfo.Description}). Please fix the issue and try again.");
                return false;
            } else if (statusInfo.AlarmInfo.Value > 0) {
                jobStatusControl.UpdateLog($"{statusInfo.Status} ({statusInfo.AlarmInfo.Description}). Please fix the issue and try again.");
                return false;
            }

            return true;
        }
    }
}
