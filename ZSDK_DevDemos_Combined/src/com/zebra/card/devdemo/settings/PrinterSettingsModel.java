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

package com.zebra.card.devdemo.settings;

import java.util.HashMap;
import java.util.Map;

import javax.swing.JOptionPane;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;
import com.zebra.sdk.settings.SettingsException;

public class PrinterSettingsModel extends PrinterModel {

	public enum SettingsGroup {
		device, print
	}

	public Map<SettingsGroup, Object[][]> getPrinterSettings(DiscoveredPrinterForDevDemo printer) {
		Map<SettingsGroup, Object[][]> statusDataMap = new HashMap<SettingsGroup, Object[][]>();
		ZebraCardPrinter zebraCardPrinter = null;

		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
			for (SettingsGroup group : SettingsGroup.values()) {
				statusDataMap.put(group, getSettingsData(zebraCardPrinter, group));
			}
		} catch (ConnectionException e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		} finally {
			cleanUpQuietly(zebraCardPrinter, connection);
		}

		return statusDataMap;
	}

	public void setSettingValue(DiscoveredPrinterForDevDemo printer, SettingsGroup group, String settingName, String settingValue) throws ConnectionException, SettingsException {
		ZebraCardPrinter zebraCardPrinter = null;
		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
			if (group == SettingsGroup.print) {
				zebraCardPrinter.setJobSetting(settingName, settingValue);
			} else {
				zebraCardPrinter.setSetting(settingName, settingValue);
			}
		} finally {
			cleanUpQuietly(zebraCardPrinter, connection);
		}
	}

	public String getSettingRange(DiscoveredPrinterForDevDemo printer, SettingsGroup group, String settingName) throws ConnectionException, SettingsException {
		String range = "";
		ZebraCardPrinter zebraCardPrinter = null;
		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
			switch (group) {
				case device:
					range = zebraCardPrinter.getSettingRange(settingName);
					break;
				case print:
					range = zebraCardPrinter.getJobSettingRange(settingName);
					break;
			}
		} finally {
			cleanUpQuietly(zebraCardPrinter, connection);
		}

		return range;
	}

	public String getSettingType(DiscoveredPrinterForDevDemo printer, SettingsGroup group, String settingName) throws ConnectionException, SettingsException {
		String settingType = "";
		ZebraCardPrinter zebraCardPrinter = null;
		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
			switch (group) {
				case device:
					settingType = zebraCardPrinter.getSettingType(settingName);
					break;
				case print:
					settingType = zebraCardPrinter.getJobSettingType(settingName);
					break;
			}
		} finally {
			cleanUpQuietly(zebraCardPrinter, connection);
		}

		return settingType;
	}

	private Object[][] getSettingsData(ZebraCardPrinter zebraCardPrinter, SettingsGroup group) {
		Object[][] result = null;
		try {
			switch (group) {
				case device:
					result = mapToArrayOfArrays(group, zebraCardPrinter.getAllSettingValues());
					break;
				case print:
					result = mapToArrayOfArrays(group, zebraCardPrinter.getAllJobSettingValues());
					break;
			}
		} catch (SettingsException e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		}

		return result;
	}

	private Object[][] mapToArrayOfArrays(SettingsGroup group, Map<String, String> allSettingValues) {
		boolean isSettable = group == SettingsGroup.device;
		String[][] result = new String[allSettingValues.keySet().size()][isSettable ? 3 : 2];

		int counter = 0;
		for (String key : allSettingValues.keySet()) {
			if (isSettable) {
				result[counter++] = new String[] { key, allSettingValues.get(key), "    Set" };
			} else {
				result[counter++] = new String[] { key, allSettingValues.get(key) };
			}
		}

		return result;
	}
}
