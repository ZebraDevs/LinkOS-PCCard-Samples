/***********************************************
 * CONFIDENTIAL AND PROPRIETARY
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2016
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

package com.zebra.card.devdemo;

import javax.swing.JOptionPane;
import javax.swing.SwingUtilities;

import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;

public class PrinterModel {

	public static final String USB_SELECTION = "USB";
	public static final String NETWORK_SELECTION = "Network";

	private ZebraCardPrinter zebraCardPrinter;
	private Connection connection;
	private boolean isManualConnection = false;

	public static void showInformationDialog(String title, String message) {
		Object[] options = { "OK" };
		showInformationDialog(options, title, message);
	}

	public static int showInformationDialog(Object[] options, String title, String message) {
		return JOptionPane.showOptionDialog(null, message, title, JOptionPane.DEFAULT_OPTION, JOptionPane.INFORMATION_MESSAGE, null, options, null);
	}

	public void showErrorDialog(final String message) {
		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				JOptionPane.showMessageDialog(null, message, "Error", JOptionPane.ERROR_MESSAGE);
			}
		});
	}

	public void cleanUpQuietly() {
		try {
			if (zebraCardPrinter != null) {
				zebraCardPrinter.destroy();
			}
		} catch (ZebraCardException e) {
		} finally {
			zebraCardPrinter = null;
		}

		try {
			if (connection != null) {
				connection.close();
			}
		} catch (ConnectionException e) {
		}
	}

	public ZebraCardPrinter getZebraCardPrinter() throws ConnectionException {
		if (zebraCardPrinter == null) {
			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
		}
		return zebraCardPrinter;
	}

	public boolean isManualConnection() {
		return isManualConnection;
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection, boolean isManualConnection) {
		this.connection = connection;
		this.isManualConnection = isManualConnection;
		zebraCardPrinter = null; // Set to null so that next getZebraCardPrinter() call creates a new instance of ZebraCardPrinter with the new connection
	}
}