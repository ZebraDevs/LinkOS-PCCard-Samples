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
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Printer.Discovery;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace DeveloperDemo_Card_Desktop.Discovery {

    public class ReconnectionHandler : CardPrinterReconnectionHandler {

        private PrinterManager printerManager;
        private DiscoveredPrinter oldPrinter;

        public delegate void ReconnectionFinishedHandler(DiscoveredPrinter printer);
        public event ReconnectionFinishedHandler ReconnectionFinished;

        public ReconnectionHandler(PrinterManager printerManager, DiscoveredPrinter oldPrinter) {
            this.printerManager = printerManager;
            this.oldPrinter = oldPrinter;
        }

        public void PrinterOnline(ZebraCardPrinter zebraCardPrinter, string firmwareVersion) {
            Connection newConnection = null;

            try {
                DiscoveredPrinter newPrinter = null;
                newConnection = zebraCardPrinter.Connection;
                newConnection.Open();

                Dictionary<string, string> discoveryData = DiscoveryUtilCard.GetDiscoveryDataMap(newConnection);

                if (oldPrinter is DiscoveredUsbPrinter) {
                    newPrinter = new DiscoveredUsbPrinter((newConnection as UsbConnection).SimpleConnectionName, discoveryData);
                } else if (oldPrinter is DiscoveredCardPrinterNetwork) {
                    newPrinter = new DiscoveredCardPrinterNetwork(discoveryData);
                } else {
                    throw new ArgumentException("Not a reconnectable printer type");
                }

                ReconnectionFinished(newPrinter);
            } catch (Exception e) {
                MessageBoxHelper.ShowError("Could not reconnect to printer: " + e.Message);
            } finally {
                ConnectionHelper.CleanUpQuietly(zebraCardPrinter, newConnection);
            }
        }

        public void ProgressUpdate(string status, int percentComplete) { }
    }
}
