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
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Map;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.sdk.common.card.enumerations.CardDestination;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;

public class SmartCardDemo extends PrinterDemoBase<SmartCardModel> {

	public static final String NO_ENCODER = "None";

	private JComboBox cardSourceComboBox;
	private JComboBox cardDestinationComboBox;
	private JComboBox cardTypeComboBox;

	public SmartCardDemo() {
		super(new SmartCardModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Smart Card"), BorderLayout.PAGE_START);
		container.add(createSelectPrinterPanel(false));
		container.add(createLowerPanel(), BorderLayout.PAGE_END);

		connectionTypeDropdown.removeItemAt(0);
	}

	private JPanel createLowerPanel() {
		JPanel lowerPanel = new JPanel();
		lowerPanel.setLayout(new BoxLayout(lowerPanel, BoxLayout.PAGE_AXIS));
		lowerPanel.add(createJobSettingPanel());

		JPanel jobStatusPanel = new JPanel();
		jobStatusPanel.setLayout(new BoxLayout(jobStatusPanel, BoxLayout.PAGE_AXIS));
		jobStatusPanel.setBorder(new EmptyBorder(10, 10, 10, 10));
		jobStatusPanel.add(createJobStatusPanel(10, 92));

		lowerPanel.add(jobStatusPanel);
		lowerPanel.add(createSmartCardEncodeButton());
		return lowerPanel;
	}

	private JPanel createSmartCardEncodeButton() {
		JPanel buttonPanel = new JPanel();
		buttonPanel.setLayout(new FlowLayout(FlowLayout.RIGHT));

		actionButton = new JButton("Start Job");
		setActionButtonEnabled(false);
		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						setActionButtonEnabled(false);
						setSmartCardOptionsEnabled(false);
						setConnectionOptionsEnabled(false);
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						String source = (String) cardSourceComboBox.getSelectedItem();
						String destination = (String) cardDestinationComboBox.getSelectedItem();
						String cardType = (String) cardTypeComboBox.getSelectedItem();

						getPrinterModel().runSmartCardOperation(source, destination, cardType, statusTextArea);

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						setActionButtonEnabled(true);
						setSmartCardOptionsEnabled(true);
						setConnectionOptionsEnabled(true);
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
		jobSettingsInnerArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 15, 10))); // Left spacer

		jobSettingsInnerArea.add(new JLabel("Source"));
		cardSourceComboBox = new JComboBox();
		cardSourceComboBox.setEnabled(false);
		jobSettingsInnerArea.add(cardSourceComboBox);

		jobSettingsInnerArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 50, 10))); // Middle spacer

		jobSettingsInnerArea.add(new JLabel("Destination"));
		cardDestinationComboBox = new JComboBox();
		cardDestinationComboBox.setEnabled(false);
		jobSettingsInnerArea.add(cardDestinationComboBox);

		jobSettingsInnerArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 50, 10))); // Right spacer

		jobSettingsInnerArea.add(new JLabel("Card Type"));
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
	protected void onConnectionEstablished() {
		String cardSourceRange = null;
		String cardDestinationRange = null;
		Map<String, String> smartCardConfigurations = null;
		boolean hasLaminator = false;
		boolean hasSmartCardEncoder = false;

		try {
			getPrinterModel().getConnection().open();

			cardSourceRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
			cardDestinationRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
			smartCardConfigurations = getPrinterModel().getZebraCardPrinter().getSmartCardConfigurations();
			hasSmartCardEncoder = getPrinterModel().getZebraCardPrinter().hasSmartCardEncoder();
			hasLaminator = getPrinterModel().getZebraCardPrinter().hasLaminator();
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "Error encountered getting setting ranges: " + e.getLocalizedMessage());
		} finally {
			getPrinterModel().cleanUpQuietly();
		}

		if (hasSmartCardEncoder) {
			setSmartCardOptionsEnabled(true);
			setActionButtonEnabled(true);

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
			for (String encoderType : smartCardConfigurations.keySet()) {
				cardTypeComboBox.addItem(encoderType);
			}
		} else {
			setSmartCardOptionsEnabled(false);
			setActionButtonEnabled(false);
			JOptionPane.showMessageDialog(null, "The selected printer does not have a smart card encoder.\nPlease choose a different printer from the list.");
		}
	}

	private void setSmartCardOptionsEnabled(boolean enabled) {
		cardSourceComboBox.setEnabled(enabled);
		cardDestinationComboBox.setEnabled(enabled);
		cardTypeComboBox.setEnabled(enabled);
	}

	private void setConnectionOptionsEnabled(boolean enabled) {
		addressDropdown.setEnabled(enabled);
		connectionTypeDropdown.setEnabled(enabled);
		discoveryButton.setEnabled(enabled);
	}
}
