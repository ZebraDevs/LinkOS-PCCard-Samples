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

package com.zebra.card.devdemo.smartcard;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.FlowLayout;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterDemo;
import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.common.card.containers.SmartCardInfo;
import com.zebra.sdk.common.card.enumerations.CardDestination;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.SmartCardEncoderType;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;

public class SmartCardDemo extends PrinterDemoBase implements PrinterDemo {

	public static final String NO_ENCODER = "None";

	private JComboBox cardSourceComboBox;
	private JComboBox cardDestinationComboBox;
	private JComboBox cardTypeComboBox;

	private ZebraCardPrinter zebraCardPrinter = null;

	@Override
	public void createDemoDialog(JFrame owner) {
		demoDialog = new JDialog(owner, "Zebra Multi Platform SDK - Developer Demo", true);

		Container mainPane = demoDialog.getContentPane();
		mainPane.add(createPanelHeader("Smart Card"), BorderLayout.PAGE_START);
		mainPane.add(createSelectPrinterPanel());

		connectionTypeDropdown.removeItemAt(0);

		JPanel lowerPart = createLowerPanel();
		mainPane.add(lowerPart, BorderLayout.PAGE_END);

		addressDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				additionalPostDiscoveryAction();
			}
		});

		demoDialog.pack();
		demoDialog.setResizable(false);
		demoDialog.setLocation((Toolkit.getDefaultToolkit().getScreenSize().width / 2) - (mainPane.getWidth() / 2), 0);
		demoDialog.setVisible(true);
		demoDialog.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
	}

	private JPanel createLowerPanel() {
		JPanel lowerPanel = new JPanel();
		lowerPanel.setLayout(new BoxLayout(lowerPanel, BoxLayout.PAGE_AXIS));
		lowerPanel.add(createJobSettingPanel());

		JPanel jobStatusPanel = new JPanel();
		jobStatusPanel.setLayout(new BoxLayout(jobStatusPanel, BoxLayout.PAGE_AXIS));
		jobStatusPanel.setBorder(new EmptyBorder(10, 10, 10, 10));

		JPanel jobStatusArea = createJobStatusPanel(10, 92);
		jobStatusPanel.add(jobStatusArea);

		lowerPanel.add(jobStatusPanel);
		lowerPanel.add(createSmartCardEncodeButton());
		return lowerPanel;
	}

	private JPanel createSmartCardEncodeButton() {
		JPanel buttonPanel = new JPanel();
		buttonPanel.setLayout(new FlowLayout(FlowLayout.RIGHT));

		actionButton = new JButton("Start Job");
		enableActionButton(false);
		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						enableActionButton(false);
						enableSmartCard(false);
						enableSelectedPrinterOptions(false);
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();

						String source = (String) cardSourceComboBox.getSelectedItem();
						String destination = (String) cardDestinationComboBox.getSelectedItem();
						String cardType = (String) cardTypeComboBox.getSelectedItem();

						new SmartCardModel().runSmartCardOperation(printer, source, destination, cardType, statusTextArea);

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						enableActionButton(true);
						enableSmartCard(true);
						enableSelectedPrinterOptions(true);
					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(actionButton, BorderLayout.LINE_END);
		buttonPanel.add(buttonHolder, BorderLayout.LINE_END);

		return buttonPanel;
	}

	private JPanel createJobSettingPanel() {
		JPanel jobSettingArea = new JPanel();
		jobSettingArea.setLayout(new BoxLayout(jobSettingArea, BoxLayout.PAGE_AXIS));
		jobSettingArea.setBorder(new TitledBorder("Job Settings"));

		JPanel jobSettingsInnerArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JPanel leftSpacer = new JPanel(new FlowLayout(FlowLayout.LEFT, 15, 10));
		jobSettingsInnerArea.add(leftSpacer);

		JLabel sourceLabel = new JLabel("Source");
		jobSettingsInnerArea.add(sourceLabel);

		cardSourceComboBox = new JComboBox();
		cardSourceComboBox.setEnabled(false);
		jobSettingsInnerArea.add(cardSourceComboBox);

		JPanel middleSpacer = new JPanel(new FlowLayout(FlowLayout.LEFT, 50, 10));
		jobSettingsInnerArea.add(middleSpacer);

		JLabel destinationLabel = new JLabel("Destination");
		jobSettingsInnerArea.add(destinationLabel);

		cardDestinationComboBox = new JComboBox();
		cardDestinationComboBox.setEnabled(false);
		jobSettingsInnerArea.add(cardDestinationComboBox);

		JPanel rightSpacer = new JPanel(new FlowLayout(FlowLayout.LEFT, 50, 10));
		jobSettingsInnerArea.add(rightSpacer);

		JLabel cardTypeLabel = new JLabel("Card Type");
		jobSettingsInnerArea.add(cardTypeLabel);

		cardTypeComboBox = new JComboBox();
		cardTypeComboBox.setEnabled(false);
		jobSettingsInnerArea.add(cardTypeComboBox);

		jobSettingArea.add(jobSettingsInnerArea);

		JPanel jobSettingPanel = new JPanel();
		jobSettingPanel.setLayout(new BoxLayout(jobSettingPanel, BoxLayout.PAGE_AXIS));
		jobSettingPanel.setBorder(new EmptyBorder(10, 10, 10, 10));
		jobSettingPanel.add(jobSettingArea);

		return jobSettingPanel;
	}

	@Override
	protected void additionalPostDiscoveryAction() {
		if (addressDropdown.getItemCount() > 0) {
			DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();
			Connection connection = null;

			String cardSourceRange = null;
			String cardDestinationRange = null;
			boolean hasLaminator = false;
			SmartCardInfo smartCardInfo = null;

			try {
				connection = printer.getConnection();
				connection.open();

				zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);
				smartCardInfo = zebraCardPrinter.getSmartCardConfiguration();
				cardSourceRange = zebraCardPrinter.getJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
				cardDestinationRange = zebraCardPrinter.getJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
				hasLaminator = zebraCardPrinter.hasLaminator();
			} catch (Exception e) {
				JOptionPane.showMessageDialog(null, "Error encountered getting setting ranges: " + e.getLocalizedMessage());
			} finally {
				PrinterModel.cleanUpQuietly(zebraCardPrinter, connection);
			}

			if (hasSmartCardEncoder(smartCardInfo)) {
				enableSmartCard(true);
				enableActionButton(true);

				cardSourceComboBox.removeAllItems();
				for (CardSource source : CardSource.values()) {
					if (cardSourceRange != null && cardSourceRange.contains(source.name())) {
						cardSourceComboBox.addItem(source.name());
					}
				}
				cardSourceComboBox.setSelectedItem(CardSource.Feeder.name());

				cardDestinationComboBox.removeAllItems();
				for (CardDestination destination : CardDestination.values()) {
					if (!hasLaminator && destination.getValue().contains("lam")) {
						continue;
					}
					if (cardDestinationRange != null && cardDestinationRange.contains(destination.name())) {
						cardDestinationComboBox.addItem(destination.name());
					}
				}
				cardDestinationComboBox.setSelectedItem(CardDestination.Eject.name());

				cardTypeComboBox.removeAllItems();
				if (!smartCardInfo.contactEncoder.equals(SmartCardEncoderType.None)) {
					cardTypeComboBox.addItem("Contact");
				}

				if (!smartCardInfo.contactlessEncoder.equals(SmartCardEncoderType.None)) {
					cardTypeComboBox.addItem("Contactless");
				}

			} else {
				enableSmartCard(false);
				enableActionButton(false);
				JOptionPane.showMessageDialog(null, "The selected printer does not have a smart card encoder.\nPlease choose a different printer from the list.");
			}
		}
	}

	private boolean hasSmartCardEncoder(SmartCardInfo smartCardInfo) {
		if (smartCardInfo.contactEncoder != SmartCardEncoderType.None || smartCardInfo.contactlessEncoder != SmartCardEncoderType.None) {
			return true;
		}
		return false;
	}

	private void enableSmartCard(boolean enabled) {
		cardSourceComboBox.setEnabled(enabled);
		cardDestinationComboBox.setEnabled(enabled);
		cardTypeComboBox.setEnabled(enabled);
	}

	private void enableSelectedPrinterOptions(boolean enabled) {
		addressDropdown.setEnabled(enabled);
		connectionTypeDropdown.setEnabled(enabled);
		discoveryButton.setEnabled(enabled);
	}
}
