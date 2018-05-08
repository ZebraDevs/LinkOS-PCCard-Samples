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

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.HashMap;
import java.util.Map;

import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTable;
import javax.swing.border.EmptyBorder;
import javax.swing.table.DefaultTableModel;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.printerstatus.PrinterStatusModel.StatusGroup;

public class PrinterStatusDemo extends PrinterDemoBase<PrinterStatusModel> {

	private final Map<StatusGroup, JTable> statusTables = new HashMap<StatusGroup, JTable>();

	public PrinterStatusDemo() {
		super(new PrinterStatusModel());
	}

	@Override
	protected void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Printer Status"), BorderLayout.PAGE_START);
		container.add(createSelectPrinterPanel(true));
		container.add(createLowerPanel(), BorderLayout.PAGE_END);
	}

	@Override
	protected void onConnectionEstablished() {
		setActionButtonEnabled(getPrinterModel().getConnection() != null);
	}

	private JPanel createLowerPanel() {
		JTabbedPane tabbedPane = new JTabbedPane();
		tabbedPane.setPreferredSize(new Dimension(600, 400));

		JComponent printerStatusTab = createStatusPanel(StatusGroup.printer);
		tabbedPane.addTab("Printer", printerStatusTab);

		JComponent generalStatusTab = createStatusPanel(StatusGroup.general);
		tabbedPane.addTab("General", generalStatusTab);

		JComponent mediaStatusTab = createStatusPanel(StatusGroup.media);
		tabbedPane.addTab("Media", mediaStatusTab);

		JComponent sensorStatusTab = createStatusPanel(StatusGroup.sensors);
		tabbedPane.addTab("Sensors", sensorStatusTab);

		actionButton = new JButton("Refresh");
		setActionButtonEnabled(false);
		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						setActionButtonEnabled(false);
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						Map<StatusGroup, String[][]> printerStatusDataMap = getPrinterModel().getPrinterStatus();
						updatePrinterStatus(printerStatusDataMap);

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						setActionButtonEnabled(true);
					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(actionButton, BorderLayout.LINE_END);

		JPanel lowerPart = new JPanel(new BorderLayout(10, 10));
		lowerPart.add(tabbedPane, BorderLayout.PAGE_START);
		lowerPart.add(buttonHolder, BorderLayout.PAGE_END);
		lowerPart.setBorder(new EmptyBorder(10, 10, 10, 10));
		return lowerPart;
	}

	private JComponent createStatusPanel(StatusGroup group) {
		statusTables.put(group, new JTable());

		JScrollPane settingListScroller = new JScrollPane();
		settingListScroller.getViewport().add(statusTables.get(group));

		return settingListScroller;
	}

	private void updatePrinterStatus(Map<StatusGroup, String[][]> statusDataMap) {
		for (PrinterStatusModel.StatusGroup group : StatusGroup.values()) {
			updateStatusGroup(group, statusDataMap.get(group));
		}
	}

	private void updateTableModel(String[][] settingsData, String[] headerLabels, JTable table) {
		DefaultTableModel tableModel = new DefaultTableModel(settingsData, headerLabels) {
			private static final long serialVersionUID = 4646379300019834951L;

			@Override
			public boolean isCellEditable(int row, int column) {
				return column == 2;
			}
		};

		table.removeAll();
		table.setModel(tableModel);
		table.invalidate();
	}

	private void updateStatusGroup(StatusGroup group, String[][] settingsData) {
		JTable table = statusTables.get(group);
		String[] headerLabels = getHeaderLabels(group);
		updateTableModel(settingsData, headerLabels, table);
	}

	private String[] getHeaderLabels(StatusGroup group) {
		String[] headerLabels;
		switch (group) {
			case media:
				headerLabels = new String[] { "Type", "InitialSize", "Panels Remaining", "Description", "OEM", "Part Number" };
				break;
			default:
				headerLabels = new String[] { "Setting", "Value" };
		}

		return headerLabels;
	}
}
