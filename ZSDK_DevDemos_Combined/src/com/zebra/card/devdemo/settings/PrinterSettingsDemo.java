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

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Component;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.HashMap;
import java.util.Map;

import javax.swing.DefaultComboBoxModel;
import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTable;
import javax.swing.border.EmptyBorder;
import javax.swing.table.DefaultTableCellRenderer;
import javax.swing.table.DefaultTableModel;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.settings.PrinterSettingsModel.SettingsGroup;

public class PrinterSettingsDemo extends PrinterDemoBase<PrinterSettingsModel> {

	private final Map<SettingsGroup, JTable> settingsTables = new HashMap<SettingsGroup, JTable>();

	public PrinterSettingsDemo() {
		super(new PrinterSettingsModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Printer Settings"), BorderLayout.PAGE_START);
		container.add(createSelectPrinterPanel(true));
		container.add(createLowerPanel(), BorderLayout.PAGE_END);
	}

	@Override
	protected void onConnectionEstablished() {
		setActionButtonEnabled(getPrinterModel().getConnection() != null);
		refreshPrinterSettings();
	}

	public void refreshPrinterSettings() {
		Map<SettingsGroup, String[][]> printerSettingsDataMap = getPrinterModel().getPrinterSettings();
		for (SettingsGroup group : SettingsGroup.values()) {
			updateTableModel(printerSettingsDataMap.get(group), group);
		}
	}

	public void resetTable(SettingsGroup settingsGroup) {
		JTable table = settingsTables.get(settingsGroup);
		DefaultTableModel tableModel = (DefaultTableModel) table.getModel();
		tableModel.setRowCount(0);
		table.invalidate();
	}

	public void resetDemo() {
		((DefaultComboBoxModel) addressDropdown.getModel()).removeAllElements();
		manualIpAddress.setText("");
		actionButton.setEnabled(false);
		resetTable(SettingsGroup.device);
		resetTable(SettingsGroup.print);
	}

	private JPanel createLowerPanel() {
		JTabbedPane tabbedPane = new JTabbedPane();
		tabbedPane.setPreferredSize(new Dimension(600, 500));
		tabbedPane.addTab("Device", createSettingPanel(SettingsGroup.device));
		tabbedPane.addTab("Print", createSettingPanel(SettingsGroup.print));

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

						refreshPrinterSettings();

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

	private JComponent createSettingPanel(SettingsGroup group) {
		String[] headerLabels = group == SettingsGroup.device ? new String[] { "Setting", "Value", "Action" } : new String[] { "Setting", "Value" };

		final JTable table = new JTable();
		table.addMouseListener(new java.awt.event.MouseAdapter() {
			@Override
			public void mouseClicked(java.awt.event.MouseEvent evt) {
				int row = table.rowAtPoint(evt.getPoint());
				int col = table.columnAtPoint(evt.getPoint());
				if (row >= 0 && col == 2) {
					String settingName = table.getModel().getValueAt(row, 0).toString();

					// Only device settings can be set in this demo
					new ChangeSettingDialog(demoDialog, PrinterSettingsDemo.this, SettingsGroup.device, settingName, getPrinterModel()).setVisible(true);
				}
			}
		});
		table.setModel(new DefaultTableModel(new String[0][headerLabels.length], headerLabels) {
			private static final long serialVersionUID = 4646379300019834951L;

			@Override
			public boolean isCellEditable(int row, int column) {
				return column == 2;
			}
		});
		table.removeAll();
		table.invalidate();
		settingsTables.put(group, table);

		JScrollPane settingListScroller = new JScrollPane();
		settingListScroller.getViewport().add(table);
		return settingListScroller;
	}

	private void updateTableModel(String[][] settingsData, SettingsGroup group) {
		JTable table = settingsTables.get(group);
		DefaultTableModel tableModel = (DefaultTableModel) table.getModel();
		tableModel.setRowCount(0);
		for (int i = 0; i < settingsData.length; i++) {
			tableModel.addRow(settingsData[i]);
		}
		table.invalidate();

		table.getColumnModel().getColumn(0).setPreferredWidth(110);
		table.getColumnModel().getColumn(1).setPreferredWidth(110);
		if (table.getColumnModel().getColumnCount() > 2) {
			table.getColumnModel().getColumn(2).setPreferredWidth(45);
			table.getColumnModel().getColumn(2).setMaxWidth(45);
			table.getColumnModel().getColumn(2).setCellRenderer(new ColoredBackgroundRenderer());
		}
	}

	class ColoredBackgroundRenderer extends DefaultTableCellRenderer {
		private static final long serialVersionUID = -7516038561585633605L;

		@Override
		public Component getTableCellRendererComponent(JTable table, Object value, boolean isSelected, boolean hasFocus, int row, int column) {
			Component cell = super.getTableCellRendererComponent(table, value, isSelected, hasFocus, row, column);
			cell.setBackground(new Color(240, 240, 240));
			return cell;
		}
	}
}
