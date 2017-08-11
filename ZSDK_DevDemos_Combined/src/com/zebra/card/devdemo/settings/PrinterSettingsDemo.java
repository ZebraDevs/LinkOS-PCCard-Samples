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
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.HashMap;
import java.util.Map;

import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTable;
import javax.swing.border.EmptyBorder;
import javax.swing.table.DefaultTableCellRenderer;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableModel;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterDemo;
import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.settings.PrinterSettingsModel.SettingsGroup;

public class PrinterSettingsDemo extends PrinterDemoBase implements PrinterDemo {

	private final Map<SettingsGroup, JTable> settingsTables = new HashMap<SettingsGroup, JTable>();
	private JFrame owner = null;

	public PrinterSettingsDemo() {
	}

	@Override
	public void createDemoDialog(JFrame owner) {
		this.owner = owner;
		demoDialog = new JDialog(owner, "Zebra Multi Platform SDK - Developer Demo", true);

		Container mainPane = demoDialog.getContentPane();
		mainPane.add(createPanelHeader("Printer Settings"), BorderLayout.PAGE_START);
		mainPane.add(createSelectPrinterPanel());

		JPanel lowerPart = createLowerPanel();
		mainPane.add(lowerPart, BorderLayout.PAGE_END);

		demoDialog.pack();
		demoDialog.setResizable(false);
		demoDialog.setLocation((Toolkit.getDefaultToolkit().getScreenSize().width / 2) - (mainPane.getWidth() / 2), 0);
		demoDialog.setVisible(true);
		demoDialog.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
	}

	@Override
	protected void additionalPostDiscoveryAction() {
		boolean printerListNotEmpty = addressDropdown.getItemCount() > 0;
		enableActionButton(printerListNotEmpty);
	}

	private JPanel createLowerPanel() {
		JTabbedPane tabbedPane = new JTabbedPane();
		tabbedPane.setPreferredSize(new Dimension(600, 500));

		JComponent deviceSettingsTab = createSettingPanel(SettingsGroup.device);
		tabbedPane.addTab("Device", deviceSettingsTab);

		JComponent printSettingsTab = createSettingPanel(SettingsGroup.print);
		tabbedPane.addTab("Print", printSettingsTab);

		actionButton = new JButton("Refresh");
		enableActionButton(false);
		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						enableActionButton(false);
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();
						Map<SettingsGroup, Object[][]> printerSettingssDataMap = new PrinterSettingsModel().getPrinterSettings(printer);
						updatePrinterSettings(printerSettingssDataMap);

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						enableActionButton(true);
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
		JTable table = new JTable();
		settingsTables.put(group, table);

		JScrollPane settingListScroller = new JScrollPane();
		settingListScroller.getViewport().add(settingsTables.get(group));

		initializeTableModel(getHeaderLabels(group), table);
		return settingListScroller;
	}

	private void updatePrinterSettings(Map<SettingsGroup, Object[][]> statusDataMap) {
		for (SettingsGroup group : SettingsGroup.values()) {
			updateSettingsGroup(group, statusDataMap.get(group));
		}
	}

	private void updateTableModel(Object[][] settingsData, String[] headerLabels, JTable table) {
		DefaultTableModel tableModel = (DefaultTableModel) table.getModel();
		tableModel.setRowCount(0);
		for (int ix = 0; ix < settingsData.length; ix++) {
			tableModel.addRow(settingsData[ix]);
		}
		table.invalidate();
	}

	private void initializeTableModel(String[] headerLabels, JTable table) {
		String[][] settingsData = new String[0][headerLabels.length];

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

	private void updateSettingsGroup(final SettingsGroup group, Object[][] settingsData) {
		boolean settable = group == SettingsGroup.device;
		final JTable table = settingsTables.get(group);
		String[] headerLabels = getHeaderLabels(group);

		updateTableModel(settingsData, headerLabels, table);

		table.getColumnModel().getColumn(0).setPreferredWidth(110);
		table.getColumnModel().getColumn(1).setPreferredWidth(110);

		if (settable) {
			table.getColumnModel().getColumn(2).setPreferredWidth(45);
			table.getColumnModel().getColumn(2).setMaxWidth(45);
			table.getColumnModel().getColumn(2).setCellRenderer(new ColoredBackgroundRendered());
			final TableModel tableModel = table.getModel();

			table.addMouseListener(new java.awt.event.MouseAdapter() {

				@Override
				public void mouseClicked(java.awt.event.MouseEvent evt) {
					int row = table.rowAtPoint(evt.getPoint());
					int col = table.columnAtPoint(evt.getPoint());
					if (row >= 0 && col == 2) {
						String settingName = tableModel.getValueAt(row, 0).toString();
						DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();
						new ChangeSettingDialog(owner, printer, group, settingName).setVisible(true);
					}
				}
			});
		}
	}

	class ColoredBackgroundRendered extends DefaultTableCellRenderer {
		private static final long serialVersionUID = -7516038561585633605L;

		@Override
		public Component getTableCellRendererComponent(JTable table, Object value, boolean isSelected, boolean hasFocus, int row, int column) {
			Component cell = super.getTableCellRendererComponent(table, value, isSelected, hasFocus, row, column);
			cell.setBackground(new Color(240, 240, 240));
			return cell;
		}
	}

	private String[] getHeaderLabels(SettingsGroup group) {
		boolean settable = group == SettingsGroup.device;
		String[] headerLabels;

		if (settable) {
			headerLabels = new String[] { "Setting", "Value", "Action" };
		} else {
			headerLabels = new String[] { "Setting", "Value" };
		}

		return headerLabels;
	}
}
