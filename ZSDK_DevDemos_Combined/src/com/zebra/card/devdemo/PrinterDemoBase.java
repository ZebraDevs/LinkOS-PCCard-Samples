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
import java.awt.Component;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.net.URL;

import javax.swing.BoxLayout;
import javax.swing.DefaultComboBoxModel;
import javax.swing.DefaultListCellRenderer;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JProgressBar;
import javax.swing.JRootPane;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.text.DefaultCaret;

import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.comm.TcpConnection;
import com.zebra.sdk.common.card.printer.discovery.NetworkCardDiscoverer;
import com.zebra.sdk.common.card.printer.discovery.ZebraCardPrinterFilter;
import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.printer.discovery.DiscoveredUsbPrinter;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;
import com.zebra.sdk.printer.discovery.UsbDiscoverer;

public abstract class PrinterDemoBase<T extends PrinterModel> {

	protected JButton actionButton = null;
	protected JButton discoveryButton = null;
	protected JComboBox connectionTypeDropdown;
	protected JDialog demoDialog;
	protected static JComboBox addressDropdown;
	protected JTextField manualIpAddress;
	protected JButton manuallyConnectButton;
	protected JTextArea statusTextArea;
	private final T printerModel;
	private JDialog progressDialog;

	public PrinterDemoBase(T printerModel) {
		this.printerModel = printerModel;
	}

	protected void createDemoDialog(JFrame owner) {
		demoDialog = new JDialog(owner, "Zebra Multiplatform SDK - Developer Demo", true);
		addDemoDialogContent(demoDialog.getContentPane());
		demoDialog.pack();
		demoDialog.setResizable(false);
		demoDialog.setLocationRelativeTo(null);
		demoDialog.setVisible(true);
		demoDialog.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
	};

	protected abstract void addDemoDialogContent(Container container);

	protected abstract void onConnectionEstablished();

	protected void onDiscoveryStart() {
	}

	protected JPanel createPanelHeader(String demoTitle) {
		JPanel panelHeaderArea = new JPanel(new BorderLayout());
		panelHeaderArea.setBorder(new EmptyBorder(10, 10, 10, 10));
		panelHeaderArea.setBackground(Color.BLACK);

		JLabel textLabel = adjustLabelFontSize(new JLabel(demoTitle));
		textLabel.setForeground(Color.WHITE);
		panelHeaderArea.add(textLabel, BorderLayout.LINE_START);

		ImageIcon zebraHead = loadIcon("resources/zebra-logo-50px.png");
		panelHeaderArea.add(new JLabel(zebraHead, JLabel.CENTER), BorderLayout.LINE_END);

		return panelHeaderArea;
	}

	protected JPanel createSelectPrinterPanel(boolean shouldAddManualConnection) {
		JPanel selectedPrinterPanel = new JPanel(new BorderLayout());
		selectedPrinterPanel.setBorder(new TitledBorder("Selected Printer"));
		selectedPrinterPanel.add(createConnectionArea(), BorderLayout.WEST);
		selectedPrinterPanel.add(createPrinterAddressArea(), BorderLayout.CENTER);
		if (shouldAddManualConnection) {
			selectedPrinterPanel.add(createManualSelectionArea(), BorderLayout.SOUTH);
		}

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
		addressDropdown.setPreferredSize(new Dimension(600, 25));
		addressDropdown.setRenderer(new DefaultListCellRenderer() {

			private static final long serialVersionUID = -6124994881797463668L;

			@Override
			public Component getListCellRendererComponent(JList list, Object value, int index, boolean isSelected, boolean cellHasFocus) {
				super.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus);
				if (value instanceof DiscoveredPrinter) {
					DiscoveredPrinter printer = (DiscoveredPrinter) value;
					setText(printer.getDiscoveryDataMap().get("MODEL") + " (" + printer.getDiscoveryDataMap().get("ADDRESS") + ")");
				}
				return this;
			}
		});
		addressDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				Object selectedItem = addressDropdown.getSelectedItem();
				if (selectedItem != null && selectedItem instanceof DiscoveredPrinter) {
					printerModel.setConnection(((DiscoveredPrinter) selectedItem).getConnection(), false);
					if (printerModel.getConnection() != null && manualIpAddress != null) {
						manualIpAddress.setText("");
					}
					onConnectionEstablished();
				}
			}
		});
		addressArea.add(addressDropdown);

		ImageIcon reloadIcon = loadIcon("resources/ic_refresh_16px.png");
		Image resizedImage = reloadIcon.getImage().getScaledInstance(15, 15, Image.SCALE_DEFAULT);
		reloadIcon = new ImageIcon(resizedImage);

		discoveryButton = new JButton();
		discoveryButton.setIcon(reloadIcon);
		discoveryButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						onDiscoveryStart();
						setConnectionButtonsEnabled(false);
						discoverPrinters();
						setConnectionButtonsEnabled(true);
					}
				}).start();
			}
		});
		addressArea.add(discoveryButton);

		return addressArea;
	}

	private JPanel createManualSelectionArea() {
		JPanel selectionArea = new JPanel();
		selectionArea.add(new JLabel("IP Address"));

		manualIpAddress = new JTextField();
		manualIpAddress.setPreferredSize(new Dimension(350, 25));
		selectionArea.add(manualIpAddress);

		manuallyConnectButton = new JButton("Connect");
		manuallyConnectButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						try {
							setConnectionButtonsEnabled(false);
							connectManuallyToPrinter();
							if (printerModel.getConnection() != null) {
								addressDropdown.removeAllItems();
							}
						} catch (ConnectionException e) {
							JOptionPane.showMessageDialog(null, "Unable to connect to printer : " + e.getLocalizedMessage());
						} finally {
							setConnectionButtonsEnabled(true);
						}
					}
				}).start();
			}
		});
		selectionArea.add(manuallyConnectButton);

		return selectionArea;
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

	public void clearDiscoveredPrinters() {
		addressDropdown.setModel(new DefaultComboBoxModel());
	}

	protected void discoverPrinters() {
		final DefaultComboBoxModel addressComboBoxModel = new DefaultComboBoxModel();

		DiscoveryHandler discoveryHandler = new DiscoveryHandler() {

			@Override
			public void foundPrinter(DiscoveredPrinter printer) {
				if (printer.getDiscoveryDataMap().get("MODEL").contains("ZXP") || printer.getDiscoveryDataMap().get("MODEL").contains("ZC")) {
					addressComboBoxModel.addElement(printer);
				}
			}

			@Override
			public void discoveryFinished() {
				addressDropdown.setModel(addressComboBoxModel);
				demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
				if (addressDropdown.getItemCount() > 0) {
					printerModel.setConnection(((DiscoveredPrinter) addressDropdown.getSelectedItem()).getConnection(), false);
					if (printerModel.getConnection() != null && manualIpAddress != null) {
						manualIpAddress.setText("");
					}
					onConnectionEstablished();
				} else {
					JOptionPane.showMessageDialog(null, "Discovery finished without finding any printers.");
				}
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
				for (DiscoveredUsbPrinter discoPrinter : UsbDiscoverer.getZebraUsbPrinters(new ZebraCardPrinterFilter())) {
					discoveryHandler.foundPrinter(discoPrinter);
				}
				discoveryHandler.discoveryFinished();
			}
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "Unable to discover printers : " + e.getLocalizedMessage());
		}
	}

	private void connectManuallyToPrinter() throws ConnectionException {
		String ipAddress = manualIpAddress.getText();
		int port = 9100;
		if (manualIpAddress.getText().contains(":")) {
			ipAddress = manualIpAddress.getText().substring(0, manualIpAddress.getText().indexOf(":"));
			port = Integer.parseInt(manualIpAddress.getText().substring(manualIpAddress.getText().indexOf(":") + 1));
		}
		Connection connection = new TcpConnection(ipAddress, port);
		testPrinterConnection(connection);
		printerModel.setConnection(connection, true);
		onConnectionEstablished();
	}

	public void testPrinterConnection(Connection connection) throws ConnectionException {
		try {
			connection.open();
		} finally {
			try {
				connection.close();
			} catch (ConnectionException e) {
				// Do nothing
			}
		}
	}

	private JLabel adjustLabelFontSize(JLabel label) {
		label.setFont(new Font(label.getFont().getName(), Font.PLAIN, 30));
		return label;
	}

	public void setActionButtonEnabled(final boolean enabled) {
		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				actionButton.setEnabled(enabled);
			}
		});
	}

	protected void setConnectionButtonsEnabled(final boolean enabled) {
		SwingUtilities.invokeLater(new Runnable() {

			@Override
			public void run() {
				discoveryButton.setEnabled(enabled);
				if (manuallyConnectButton != null) {
					manuallyConnectButton.setEnabled(enabled);
				}
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

	public void showProgressDialog(String message) {
		if (progressDialog != null) {
			progressDialog.dispose();
		}

		progressDialog = new JDialog(demoDialog, null, true);

		JPanel progressPanel = new JPanel();
		progressPanel.setLayout(new BoxLayout(progressPanel, BoxLayout.Y_AXIS));
		progressPanel.setBorder(new EmptyBorder(10, 15, 10, 15));

		JProgressBar progressBar = new JProgressBar();
		progressBar.setIndeterminate(true);
		progressBar.setAlignmentX(Component.CENTER_ALIGNMENT);
		progressPanel.add(progressBar);

		JLabel progressMessage = new JLabel(message);
		progressMessage.setAlignmentX(Component.CENTER_ALIGNMENT);
		progressPanel.add(progressMessage);

		progressDialog.getContentPane().add(progressPanel);
		progressDialog.setUndecorated(true);
		progressDialog.getRootPane().setWindowDecorationStyle(JRootPane.NONE);
		progressDialog.pack();
		progressDialog.setResizable(false);
		progressDialog.setLocationRelativeTo(null);
		progressDialog.setVisible(true);
		progressDialog.setModal(false);
	}

	public void dismissProgressDialog() {
		if (progressDialog != null) {
			progressDialog.dispose();
			progressDialog = null;
		}
	}

	public JDialog getDemoDialog() {
		return demoDialog;
	}

	public T getPrinterModel() {
		return printerModel;
	}

	public JComboBox getAddressComboBox() {
		return addressDropdown;
	}

	public String getSelectedConnectionType() {
		return (String) connectionTypeDropdown.getSelectedItem();
	}

	public void setConnectionType(String connectionType) {
		connectionTypeDropdown.setSelectedItem(connectionType);
	}

	public void setManualConnectionText(String ipAddress) {
		manualIpAddress.setText(ipAddress);
	}
}
