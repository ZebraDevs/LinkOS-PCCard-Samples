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

package com.zebra.card.devdemo.printerstatus;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.swing.JOptionPane;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.MediaInfo;
import com.zebra.sdk.common.card.containers.PrinterInfo;
import com.zebra.sdk.common.card.containers.PrinterStatusInfo;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;

public class PrinterStatusModel extends PrinterModel {

	public enum StatusGroup {
		printer, general, media, sensors
	}

	public Map<StatusGroup, Object[][]> getPrinterStatus(DiscoveredPrinterForDevDemo printer) {
		Map<StatusGroup, Object[][]> statusDataMap = new HashMap<StatusGroup, Object[][]>();
		ZebraCardPrinter zebraCardPrinter = null;

		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);

			for (StatusGroup group : StatusGroup.values()) {
				statusDataMap.put(group, getStatusData(zebraCardPrinter, group));
			}
		} catch (ConnectionException e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		} finally {
			cleanUpQuietly(zebraCardPrinter, connection);
		}
		return statusDataMap;
	}

	private Object[][] getStatusData(ZebraCardPrinter zebraCardPrinter, StatusGroup group) {
		Object[][] result = null;
		try {
			switch (group) {
				case printer:
					result = mapPrinterInfoToNameValueList(zebraCardPrinter.getPrinterInformation());
					break;
				case general:
					result = mapPrinterStatusToNameValueList(zebraCardPrinter.getPrinterStatus());
					break;
				case media:
					result = mapMediaInfoToNameValueList(zebraCardPrinter.getMediaInformation());
					break;
				case sensors:
					result = mapSensorStatesToNameValueList(zebraCardPrinter.getSensorStates(), zebraCardPrinter.getSensorValues());
					break;
			}
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		}
		return result;
	}

	private Object[][] mapPrinterStatusToNameValueList(PrinterStatusInfo printerStatus) {
		final String[][] result = new String[9][2];

		int counter = 0;
		result[counter++] = new String[] { "status", printerStatus.status };
		result[counter++] = new String[] { "jobsPending", Integer.toString(printerStatus.jobsPending) };
		result[counter++] = new String[] { "jobsActive", Integer.toString(printerStatus.jobsActive) };
		result[counter++] = new String[] { "jobsComplete", Integer.toString(printerStatus.jobsComplete) };
		result[counter++] = new String[] { "jobErrors", Integer.toString(printerStatus.jobErrors) };
		result[counter++] = new String[] { "jobsTotal", Integer.toString(printerStatus.jobsTotal) };
		result[counter++] = new String[] { "nextJobID", Integer.toString(printerStatus.nextJobID) };
		result[counter++] = new String[] { "alarmInfo", Integer.toString(printerStatus.alarmInfo.value) + ":" + printerStatus.alarmInfo.description };
		result[counter++] = new String[] { "errorInfo", Integer.toString(printerStatus.errorInfo.value) + ":" + printerStatus.errorInfo.description };
		return result;
	}

	private Object[][] mapPrinterInfoToNameValueList(PrinterInfo printerInfo) {
		final String[][] result = new String[10][2];

		int counter = 0;
		result[counter++] = new String[] { "vendor", printerInfo.vendor };
		result[counter++] = new String[] { "model", printerInfo.model };
		result[counter++] = new String[] { "serialNumber", printerInfo.serialNumber };
		result[counter++] = new String[] { "macAddress", printerInfo.macAddress };
		result[counter++] = new String[] { "printheadSerialNumber", printerInfo.printheadSerialNumber };
		result[counter++] = new String[] { "oemCode", printerInfo.oemCode };
		result[counter++] = new String[] { "firmwareVersion", printerInfo.firmwareVersion };
		result[counter++] = new String[] { "mediaVersion", printerInfo.mediaVersion };
		result[counter++] = new String[] { "heaterVersion", printerInfo.heaterVersion };
		result[counter++] = new String[] { "zmotifVersion", printerInfo.zmotifVersion };
		return result;
	}

	private Object[][] mapMediaInfoToNameValueList(List<MediaInfo> mediaInfo) {
		final String[][] result = new String[mediaInfo.size()][6];

		int counter = 0;
		for (MediaInfo info : mediaInfo) {
			String[] row = new String[] { info.type.name(), Integer.toString(info.initialSize), Integer.toString(info.panelsRemaining), info.description, info.oemCode, info.partNumber };
			result[counter++] = row;
		}

		return result;
	}

	private Object[][] mapSensorStatesToNameValueList(Map<String, String> sensorStates, Map<String, String> sensorValues) {
		final String[][] result = new String[sensorStates.size() + sensorValues.size()][2];

		int counter = 0;
		for (String key : sensorStates.keySet()) {
			result[counter++] = new String[] { key, sensorStates.get(key) };
		}

		for (String key : sensorValues.keySet()) {
			result[counter++] = new String[] { key, sensorValues.get(key) };
		}

		return result;
	}
}
