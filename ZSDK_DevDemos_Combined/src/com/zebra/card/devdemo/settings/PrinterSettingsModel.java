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

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.swing.DefaultComboBoxModel;
import javax.swing.JOptionPane;
import javax.swing.SwingUtilities;

import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.CardConnectionReestablisher;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;
import com.zebra.sdk.common.card.printer.ZebraPrinterZmotif;
import com.zebra.sdk.common.card.settings.ZebraCardSettingNames;
import com.zebra.sdk.printer.CardPrinterReconnectionHandler;
import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.settings.SettingsException;
import com.zebra.sdk.zmotif.settings.ZebraCardSettingNamesZmotif;

public class PrinterSettingsModel extends PrinterModel {

	private static final List<String> SGD_LIST_NETWORK_RESET_REQUIRED = Arrays.asList(ZebraCardSettingNamesZmotif.WIRED_SNMP, ZebraCardSettingNames.WIRED_DHCP, ZebraCardSettingNames.WIRED_ADDRESS, ZebraCardSettingNames.WIRED_SUBNET, ZebraCardSettingNames.WIRED_GATEWAY,
			ZebraCardSettingNamesZmotif.WIRED_DNS_NAME, ZebraCardSettingNamesZmotif.WIRELESS_SNMP, ZebraCardSettingNamesZmotif.WIRELESS_DHCP, ZebraCardSettingNamesZmotif.WIRELESS_ADDRESS, ZebraCardSettingNamesZmotif.WIRELESS_SUBNET, ZebraCardSettingNamesZmotif.WIRELESS_GATEWAY);
	private static final List<String> SGD_LIST_PRINTER_RESET_REQUIRED = Arrays.asList(ZebraCardSettingNamesZmotif.OCP_LANGUAGE_TYPE, ZebraCardSettingNamesZmotif.OCP_LANGUAGE_NAME, ZebraCardSettingNamesZmotif.STANDBY_TIMEOUT);
	private static final List<String> SGD_LIST_DISABLE_DHCP_ETHERNET = Arrays.asList(ZebraCardSettingNamesZmotif.WIRED_ADDRESS, ZebraCardSettingNamesZmotif.WIRED_SUBNET, ZebraCardSettingNamesZmotif.WIRED_GATEWAY);
	private static final List<String> SGD_LIST_DISABLE_DHCP_WIRELESS = Arrays.asList(ZebraCardSettingNamesZmotif.WIRELESS_ADDRESS, ZebraCardSettingNamesZmotif.WIRELESS_SUBNET, ZebraCardSettingNamesZmotif.WIRELESS_GATEWAY);

	public enum SettingsGroup {
		device, print
	}

	public Map<SettingsGroup, String[][]> getPrinterSettings() {
		Map<SettingsGroup, String[][]> statusDataMap = new HashMap<SettingsGroup, String[][]>();

		try {
			getConnection().open();

			for (SettingsGroup group : SettingsGroup.values()) {
				statusDataMap.put(group, getSettingsData(group));
			}
		} catch (Exception e) {
			showErrorDialog("Error retrieving printer settings: " + e.getLocalizedMessage());
		} finally {
			cleanUpQuietly();
		}

		return statusDataMap;
	}

	public void setSettingValue(final PrinterSettingsDemo printerSettingsDemo, final ChangeSettingDialog changeSettingDialog, final String changedSetting, String settingValue) throws ConnectionException, SettingsException {
		final boolean isResetRequired = SGD_LIST_NETWORK_RESET_REQUIRED.contains(changedSetting) || SGD_LIST_PRINTER_RESET_REQUIRED.contains(changedSetting);
		final boolean resetPrinter = SGD_LIST_PRINTER_RESET_REQUIRED.contains(changedSetting);

		final boolean isManualConnection = isManualConnection();
		final String selectedConnectionType = printerSettingsDemo.getSelectedConnectionType();
		final DiscoveredPrinter selectedPrinter = (DiscoveredPrinter) printerSettingsDemo.getAddressComboBox().getSelectedItem();

		try {
			getConnection().open();

			if (SGD_LIST_DISABLE_DHCP_ETHERNET.contains(changedSetting)) {
				getZebraCardPrinter().setSetting(ZebraCardSettingNames.WIRED_DHCP, "disabled");
			}

			if (SGD_LIST_DISABLE_DHCP_WIRELESS.contains(changedSetting)) {
				getZebraCardPrinter().setSetting(ZebraCardSettingNamesZmotif.WIRELESS_DHCP, "disabled");
			}

			getZebraCardPrinter().setSetting(changedSetting, settingValue);
		} catch (Exception e) {
			showErrorDialog("Error setting value for setting " + changedSetting + ": " + e.getLocalizedMessage());
		} finally {
			cleanUpQuietly();
		}

		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				changeSettingDialog.dispose();
				printerSettingsDemo.refreshPrinterSettings();

				if (isResetRequired) {
					String resetButtonName = "Reset";
					String[] options = { resetButtonName };
					String resetTypeString = resetPrinter ? "printer" : "network";
					String message = "The setting " + changedSetting + " requires a " + resetTypeString + " reset for any changes to take effect. Click " + resetButtonName + " to reset the " + resetTypeString + ".";
					int result = JOptionPane.showOptionDialog(printerSettingsDemo.getDemoDialog(), message, "Reset Required", JOptionPane.OK_CANCEL_OPTION, JOptionPane.WARNING_MESSAGE, null, options, options[0]);
					if (result == JOptionPane.OK_OPTION) {
						printerSettingsDemo.resetDemo();

						new Thread(new Runnable() {

							@Override
							public void run() {
								reestablishConnection(printerSettingsDemo, resetPrinter, isManualConnection, selectedConnectionType, selectedPrinter);
							}
						}).start();

						printerSettingsDemo.showProgressDialog("Reconnecting...");
					}
				}
			}
		});
	}

	public void reestablishConnection(final PrinterSettingsDemo printerSettingsDemo, boolean resetPrinter, final boolean isManualConnection, final String connectionType, final DiscoveredPrinter selectedPrinter) {
		ZebraPrinterZmotif zebraPrinterZmotif = null;

		try {
			zebraPrinterZmotif = ZebraCardPrinterFactory.getZmotifPrinter(getConnection());

			if (resetPrinter) {
				zebraPrinterZmotif.reset();
			} else {
				zebraPrinterZmotif.resetNetwork();
			}

			Thread.sleep(5000);

			ReconnectionHandler reconnectionHandler = new ReconnectionHandler();
			CardConnectionReestablisher connectionReestablisher = (CardConnectionReestablisher) getConnection().getConnectionReestablisher(60000L);
			connectionReestablisher.reestablishConnection(reconnectionHandler);

			try {
				if (zebraPrinterZmotif != null) {
					zebraPrinterZmotif.destroy();
				}
			} catch (Exception e) {
				// Do nothing
			}

			cleanUpQuietly();

			while (!reconnectionHandler.isPrinterOnline()) {
				Thread.sleep(100);
			}

			setConnection(reconnectionHandler.getZebraCardPrinter().getConnection(), isManualConnection);
			final String address = getConnection().getSimpleConnectionName();

			SwingUtilities.invokeAndWait(new Runnable() {

				@Override
				public void run() {
					if (isManualConnection) {
						printerSettingsDemo.setManualConnectionText(address);
						printerSettingsDemo.setActionButtonEnabled(true);
						printerSettingsDemo.refreshPrinterSettings();
					} else {
						printerSettingsDemo.setConnectionType(connectionType);
						selectedPrinter.getDiscoveryDataMap().put("ADDRESS", address);
						((DefaultComboBoxModel) printerSettingsDemo.getAddressComboBox().getModel()).addElement(selectedPrinter);
						printerSettingsDemo.setActionButtonEnabled(true);
						printerSettingsDemo.refreshPrinterSettings();
					}
				}
			});
		} catch (Exception e) {
			showErrorDialog("Error reestablishing connection: " + e.getLocalizedMessage());
		} finally {
			SwingUtilities.invokeLater(new Runnable() {

				@Override
				public void run() {
					printerSettingsDemo.dismissProgressDialog();
				}
			});
		}
	}

	public String getSettingRange(SettingsGroup group, String settingName) throws ConnectionException, SettingsException {
		String range = "";

		try {
			getConnection().open();

			switch (group) {
				case device:
					range = getZebraCardPrinter().getSettingRange(settingName);
					break;
				case print:
					range = getZebraCardPrinter().getJobSettingRange(settingName);
					break;
			}
		} catch (Exception e) {
			showErrorDialog("Error retrieving setting range for setting " + settingName + ": " + e.getLocalizedMessage());
		} finally {
			cleanUpQuietly();
		}

		return range;
	}

	public String getSettingType(SettingsGroup group, String settingName) throws ConnectionException, SettingsException {
		String settingType = "";

		try {
			getConnection().open();

			switch (group) {
				case device:
					settingType = getZebraCardPrinter().getSettingType(settingName);
					break;
				case print:
					settingType = getZebraCardPrinter().getJobSettingType(settingName);
					break;
			}
		} catch (Exception e) {
			showErrorDialog("Error retrieving setting type for setting " + settingName + ": " + e.getLocalizedMessage());
		} finally {
			cleanUpQuietly();
		}

		return settingType;
	}

	private String[][] getSettingsData(SettingsGroup group) throws ConnectionException, SettingsException {
		String[][] result = null;
		switch (group) {
			case device:
				result = mapToArrayOfArrays(group, getZebraCardPrinter().getAllSettingValues());
				break;
			case print:
				result = mapToArrayOfArrays(group, getZebraCardPrinter().getAllJobSettingValues());
				break;
		}

		return result;
	}

	private String[][] mapToArrayOfArrays(SettingsGroup group, Map<String, String> allSettingValues) {
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

	class ReconnectionHandler implements CardPrinterReconnectionHandler {

		private ZebraCardPrinter zebraCardPrinter;
		private boolean isPrinterOnline = false;

		@Override
		public void progressUpdate(String status, int percentComplete) {
			// Do nothing
		}

		@Override
		public void printerOnline(ZebraCardPrinter printer, String firmwareVersion) {
			zebraCardPrinter = printer;
			isPrinterOnline = true;
		}

		public ZebraCardPrinter getZebraCardPrinter() {
			return zebraCardPrinter;
		}

		public boolean isPrinterOnline() {
			return isPrinterOnline;
		}
	}
}
