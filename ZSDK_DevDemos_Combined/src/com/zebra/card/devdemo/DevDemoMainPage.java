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
import java.awt.Dimension;
import java.awt.Font;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.net.URL;

import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;

import com.zebra.card.devdemo.graphicconversion.GraphicConversionDemo;
import com.zebra.card.devdemo.mag.MagEncodeDemo;
import com.zebra.card.devdemo.multijob.MultiJobDemo;
import com.zebra.card.devdemo.print.PrintDemo;
import com.zebra.card.devdemo.printerstatus.PrinterStatusDemo;
import com.zebra.card.devdemo.settings.PrinterSettingsDemo;
import com.zebra.card.devdemo.smartcard.SmartCardDemo;
import com.zebra.card.devdemo.template.TemplateDemo;

public class DevDemoMainPage {

	public DevDemoMainPage() {
		JFrame devDemoFrame = new JFrame("Zebra Multiplatform SDK - Developer Demo");

		Container mainPane = devDemoFrame.getContentPane();

		mainPane.add(createPanelHeader(), BorderLayout.PAGE_START);

		JPanel boxOfButtons = new JPanel();
		boxOfButtons.setLayout(new BoxLayout(boxOfButtons, BoxLayout.PAGE_AXIS));
		boxOfButtons.setBorder(new EmptyBorder(10, 10, 10, 10));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Graphic Conversion", new GraphicConversionDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Magnetic Encoding", new MagEncodeDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Print YMCKO / Mono", new PrintDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Settings", new PrinterSettingsDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Smartcard", new SmartCardDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Status", new PrinterStatusDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Template", new TemplateDemo(), devDemoFrame));
		boxOfButtons.add(createOneDevDemoStartButtonArea("Multi-job", new MultiJobDemo(), devDemoFrame));
		mainPane.add(boxOfButtons, BorderLayout.PAGE_END);

		devDemoFrame.pack();
		devDemoFrame.setVisible(true);
		devDemoFrame.setLocation((Toolkit.getDefaultToolkit().getScreenSize().width - devDemoFrame.getSize().width) / 2, 0);
		devDemoFrame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	}

	private Component createOneDevDemoStartButtonArea(String demoTitle, final PrinterDemoBase<? extends PrinterModel> demo, final JFrame devDemoFrame) {
		JPanel area = new JPanel(new BorderLayout());
		area.setBorder(new EmptyBorder(10, 10, 10, 10));
		area.add(adjustLabelFontSize(new JLabel(demoTitle), 20), BorderLayout.WEST);

		JButton runDemoButton = new JButton("Run demo");
		runDemoButton.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent arg0) {
				demo.createDemoDialog(devDemoFrame);
			}

		});
		area.add(runDemoButton, BorderLayout.EAST);

		JPanel spacerForButtonArea = new JPanel(new BorderLayout());
		spacerForButtonArea.setBorder(BorderFactory.createLineBorder(new Color(135, 201, 222))); // Set light blue border
		spacerForButtonArea.add(area);

		return spacerForButtonArea;
	}

	private JPanel createPanelHeader() {
		JPanel panelHeaderArea = new JPanel(new BorderLayout());
		panelHeaderArea.setBorder(new EmptyBorder(10, 10, 10, 10));
		panelHeaderArea.setBackground(Color.BLACK);

		JPanel leftHandSideText = new JPanel();
		leftHandSideText.setLayout(new BorderLayout());
		leftHandSideText.setBackground(Color.BLACK);

		JLabel textLabel = adjustLabelFontSize(new JLabel("Developer Demos"), 30);
		textLabel.setForeground(Color.WHITE);
		leftHandSideText.add(textLabel, BorderLayout.NORTH);

		JLabel lowerTextLabel = adjustLabelFontSize(new JLabel("FOR ZEBRA CARD PRINTERS"), 10);
		lowerTextLabel.setForeground(Color.WHITE);
		leftHandSideText.add(lowerTextLabel, BorderLayout.SOUTH);

		panelHeaderArea.add(leftHandSideText, BorderLayout.LINE_START);

		ImageIcon zebraHead = loadIcon("resources/zebra-logo-50px.png");
		panelHeaderArea.add(new JLabel(zebraHead, JLabel.CENTER), BorderLayout.LINE_END);

		panelHeaderArea.add(Box.createRigidArea(new Dimension(50, 0)), BorderLayout.CENTER);
		return panelHeaderArea;
	}

	private JLabel adjustLabelFontSize(JLabel label, int newSize) {
		label.setFont(new Font(label.getFont().getName(), Font.PLAIN, newSize));
		return label;
	}

	private ImageIcon loadIcon(String logo_path) {
		ImageIcon icon = new ImageIcon(logo_path);
		URL logoContentURL = getClass().getClassLoader().getResource(logo_path);
		if (logoContentURL != null) {
			icon = new ImageIcon(logoContentURL);
		}
		return icon;
	}
}
