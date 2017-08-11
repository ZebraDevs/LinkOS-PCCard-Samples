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

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Cursor;
import java.awt.Font;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.net.URL;
import java.util.HashMap;

import javax.swing.BoxLayout;
import javax.swing.DefaultComboBoxModel;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JDialog;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.text.DefaultCaret;

import com.zebra.sdk.common.card.printer.discovery.NetworkCardDiscoverer;
import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.printer.discovery.DiscoveredUsbPrinter;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;
import com.zebra.sdk.printer.discovery.UsbDiscoverer;

public class PrinterDemoBase {

	public PrinterDemoBase() {
		super();
	}

	protected JButton actionButton = null;
	protected JButton discoveryButton = null;
	protected JComboBox connectionTypeDropdown;
	protected JDialog demoDialog;
	protected static JComboBox addressDropdown;
	protected JTextArea statusTextArea;

	protected JPanel createPanelHeader(String demoTitle) {
		JPanel panelHeaderArea = new JPanel(new BorderLayout());
		panelHeaderArea.setBorder(new EmptyBorder(10, 10, 10, 10));
		panelHeaderArea.setBackground(Color.BLACK);

		final JLabel textLabel = adjustLabelFontSize(new JLabel(demoTitle));
		textLabel.setForeground(Color.WHITE);
		panelHeaderArea.add(textLabel, BorderLayout.LINE_START);

		String zebra_logo_path = "resources/zebra-logo-50px.png";
		ImageIcon zebraHead = loadIcon(zebra_logo_path);

		panelHeaderArea.add(new JLabel(zebraHead, JLabel.CENTER), BorderLayout.LINE_END);
		return panelHeaderArea;
	}

	protected JPanel createSelectPrinterPanel() {
		JPanel selectedPrinterPanel = new JPanel(new BorderLayout());
		selectedPrinterPanel.setBorder(new TitledBorder("Selected Printer"));
		selectedPrinterPanel.add(createConnectionArea(), BorderLayout.WEST);
		selectedPrinterPanel.add(createPrinterAddressArea(), BorderLayout.CENTER);

		JPanel spacerForSelectedPrinter = new JPanel(new BorderLayout());
		spacerForSelectedPrinter.setBorder(new EmptyBorder(10, 10, 10, 10));
		spacerForSelectedPrinter.add(selectedPrinterPanel);
		return spacerForSelectedPrinter;
	}

	private JPanel createConnectionArea() {
		JPanel connectionArea = new JPanel();
		connectionArea.add(new JLabel("Connection"));

		connectionTypeDropdown = new JComboBox(new String[] { PrinterModel.NETWORK_SELECTION, PrinterModel.USB_SELECTION });
		connectionTypeDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				clearDiscoveredPrinters();
			}
		});

		connectionArea.add(connectionTypeDropdown);
		return connectionArea;
	}

	private JPanel createPrinterAddressArea() {
		JPanel addressArea = new JPanel();
		addressArea.add(new JLabel("Printer"));

		addressDropdown = new JComboBox();
		addressDropdown.setPrototypeDisplayValue(new DiscoveredPrinterForDevDemo(createPrototypeForCombobox()));
		addressArea.add(addressDropdown);

		String reloadIconPath = "resources/ic_refresh_16px.png";
		ImageIcon reloadIcon = loadIcon(reloadIconPath);

		Image resizedImage = reloadIcon.getImage().getScaledInstance(15, 15, Image.SCALE_DEFAULT);
		reloadIcon = new ImageIcon(resizedImage);

		discoveryButton = new JButton();
		discoveryButton.setIcon(reloadIcon);
		addressArea.add(discoveryButton);

		discoveryButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						additionalPreDiscoveryAction();
						enableDiscoveryButton(false);
						discoverPrinters();
						enableDiscoveryButton(true);
					}
				}).start();
			}
		});

		return addressArea;
	}

	public JPanel createJobStatusPanel(int height, int width) {
		JPanel jobStatusArea = new JPanel();
		jobStatusArea.setBorder(new TitledBorder("Job Status"));
		jobStatusArea.setLayout(new BoxLayout(jobStatusArea, BoxLayout.PAGE_AXIS));
		jobStatusArea.add(createJobStatusArea(height, width), BorderLayout.AFTER_LAST_LINE);
		return jobStatusArea;
	}

	public JScrollPane createJobStatusArea(int height, int width) {
		statusTextArea = new JTextArea(height, width);
		DefaultCaret caret = (DefaultCaret) statusTextArea.getCaret();

		caret.setUpdatePolicy(DefaultCaret.ALWAYS_UPDATE);
		statusTextArea.setEditable(false);

		return new JScrollPane(statusTextArea);
	}

	public static void clearDiscoveredPrinters() {
		addressDropdown.setModel(new DefaultComboBoxModel());
	}

	protected void discoverPrinters() {
		final DefaultComboBoxModel addressComboBoxModel = new DefaultComboBoxModel();

		DiscoveryHandler discoveryHandler = new DiscoveryHandler() {

			@Override
			public void foundPrinter(DiscoveredPrinter printer) {
				if (printer.getDiscoveryDataMap().get("MODEL").contains("ZXP")) {
					addressComboBoxModel.addElement(new DiscoveredPrinterForDevDemo(printer));
				}
			}

			@Override
			public void discoveryFinished() {
				addressDropdown.setModel(addressComboBoxModel);
				demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
				additionalPostDiscoveryAction();
			}

			@Override
			public void discoveryError(String message) {
				JOptionPane.showMessageDialog(null, "Unable to discover printers : " + message);
			}
		};

		try {
			demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

			if (connectionTypeDropdown.getSelectedItem().equals(PrinterModel.NETWORK_SELECTION)) {
				NetworkCardDiscoverer.findPrinters(discoveryHandler);
			} else {
				for (DiscoveredUsbPrinter discoPrinter : UsbDiscoverer.getZebraUsbPrinters()) {
					discoveryHandler.foundPrinter(discoPrinter);
				}
				discoveryHandler.discoveryFinished();
			}
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "Unable to discover printers : " + e.getLocalizedMessage());
		}
	}

	private DiscoveredUsbPrinter createPrototypeForCombobox() {
		HashMap<String, String> attributes = new HashMap<String, String>();
		attributes.put("MODEL", "");

		StringBuffer eightyChars = new StringBuffer(); // Wide enough to hold Card Printer usb address
		for (int ix = 1; ix <= 8; ix++) {
			eightyChars.append("HHHHHHHHHH");
		}

		return new DiscoveredUsbPrinter(eightyChars.toString(), attributes);
	}

	protected void additionalPreDiscoveryAction() {
	}

	protected void additionalPostDiscoveryAction() {
	}

	private JLabel adjustLabelFontSize(JLabel label) {
		Font labelFont = label.getFont();
		label.setFont(new Font(labelFont.getName(), Font.PLAIN, 30));
		return label;
	}

	protected void enableActionButton(final boolean b) {
		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				actionButton.setEnabled(b);
			}
		});
	}

	protected void enableDiscoveryButton(final boolean b) {
		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				discoveryButton.setEnabled(b);
			}
		});
	}

	protected ImageIcon loadIcon(String logo_path) {
		ImageIcon icon = new ImageIcon(logo_path);
		URL logoContentURL = getClass().getClassLoader().getResource(logo_path);
		if (logoContentURL != null) {
			icon = new ImageIcon(logoContentURL);
		}
		return icon;
	}
}
