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
import java.awt.Container;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.border.EmptyBorder;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.settings.PrinterSettingsModel.SettingsGroup;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.settings.SettingsException;

public class ChangeSettingDialog extends JDialog {

	private static final long serialVersionUID = 7477092684017834672L;

	public static final int DIALOG_HEIGHT = 200;
	public static final int DIALOG_WIDTH = 500;

	private final DiscoveredPrinterForDevDemo printer;
	private final SettingsGroup group;
	private final String settingName;

	private JTextField settingValueTextField;

	public ChangeSettingDialog(JFrame owner, DiscoveredPrinterForDevDemo printer, SettingsGroup group, String settingName) {
		super(owner, "Set printer setting value", true);
		this.setSize(DIALOG_WIDTH, DIALOG_HEIGHT);
		this.setResizable(false);
		this.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
		this.printer = printer;
		this.group = group;
		this.settingName = settingName;

		Container mainPane = this.getContentPane();

		JPanel outerArea = new JPanel(new BorderLayout(20, 20));
		outerArea.setBorder(new EmptyBorder(20, 20, 20, 20));
		outerArea.add(createTopArea(settingName), BorderLayout.PAGE_START);
		outerArea.add(createBottomLine(), BorderLayout.PAGE_END);

		mainPane.add(outerArea);
		this.pack();
	}

	private JPanel createTopArea(String settingName) {
		JPanel topArea = new JPanel(new BorderLayout(10, 10));
		JLabel ipAddressLabel = new JLabel("Set the following setting on the selected printer:");

		topArea.add(ipAddressLabel, BorderLayout.PAGE_START);
		topArea.add(createMiddleArea(settingName), BorderLayout.PAGE_END);

		return topArea;
	}

	private JPanel createMiddleArea(String settingName) {
		JLabel settingLabel = new JLabel(settingName);

		JPanel newValueArea = new JPanel(new FlowLayout(FlowLayout.LEFT));
		newValueArea.add(settingLabel);

		settingValueTextField = new JTextField();
		settingValueTextField.setPreferredSize(new Dimension(110, 25));
		newValueArea.add(settingValueTextField);

		String allowedValues = "";
		try {
			allowedValues = new PrinterSettingsModel().getSettingRange(printer, group, settingName);
		} catch (ConnectionException e) {
			JOptionPane.showMessageDialog(null, "Unable to get range for " + settingName + " : " + e.getLocalizedMessage());
		} catch (SettingsException e) {
			JOptionPane.showMessageDialog(null, "Unable to get range for " + settingName + " : " + e.getLocalizedMessage());
		}

		String settingType = "";
		try {
			settingType = new PrinterSettingsModel().getSettingType(printer, group, settingName);
		} catch (ConnectionException e) {
			JOptionPane.showMessageDialog(null, "Unable to get type for " + settingName + " : " + e.getLocalizedMessage());
		} catch (SettingsException e) {
			JOptionPane.showMessageDialog(null, "Unable to get type for " + settingName + " : " + e.getLocalizedMessage());
		}

		JLabel settingInfoLabel = new JLabel("Type: " + settingType + " Range: " + allowedValues);
		newValueArea.add(settingInfoLabel);
		return newValueArea;
	}

	private JPanel createBottomLine() {
		JPanel buttonArea = new JPanel(new FlowLayout(FlowLayout.RIGHT));

		JButton setValueButton = new JButton("Set Value");
		buttonArea.add(setValueButton);

		JButton cancelButton = new JButton("Cancel");
		buttonArea.add(cancelButton);

		cancelButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				dispose();
			}
		});

		setValueButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				final String settingValue = settingValueTextField.getText();
				new Thread(new Runnable() {

					@Override
					public void run() {
						try {
							new PrinterSettingsModel().setSettingValue(printer, group, settingName, settingValue);
						} catch (ConnectionException e) {
							JOptionPane.showMessageDialog(null, "Unable to set value for printer at " + printer + " : " + e.getLocalizedMessage());
						} catch (SettingsException e) {
							JOptionPane.showMessageDialog(null, "Unable to set value for printer at " + printer + " : " + e.getLocalizedMessage());
						}
					}
				}).start();
				dispose();
			}
		});

		return buttonArea;
	}

	public static void showErrorDialog(JDialog parent, String message, String title) {
		JOptionPane.showMessageDialog(parent, message, title, JOptionPane.ERROR_MESSAGE);
	}
}
