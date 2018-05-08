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

package com.zebra.card.devdemo.print;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;

import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.filechooser.FileFilter;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.sdk.common.card.enumerations.PrintType;

public class PrintDemo extends PrinterDemoBase<PrintModel> {

	private static final String PRINT_TYPE_COLOR = "Color";
	private static final String PRINT_TYPE_MONO = "MonoK";

	private JTextField frontImageFileNameTextField;
	private JTextField overlayImageFileNameTextField;
	private JTextField backImageFileNameTextField;

	private JCheckBox selectPrintFrontCheckBox;
	private JCheckBox selectPrintOverlayCheckBox;
	private JCheckBox selectPrintBackCheckBox;

	private JComboBox printTypeComboBox;
	private JComboBox printQuantityComboBox;

	public PrintDemo() {
		super(new PrintModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Print YMCKO / Mono"), BorderLayout.PAGE_START);
		container.add(createSelectPrinterPanel(true));
		container.add(createLowerPanel(), BorderLayout.PAGE_END);
	}

	@Override
	protected void onConnectionEstablished() {
		boolean atLeastOneImageSpecified = selectPrintFrontCheckBox.isSelected() || selectPrintOverlayCheckBox.isSelected() || selectPrintBackCheckBox.isSelected();
		setActionButtonEnabled(atLeastOneImageSpecified);
	}

	private JPanel createLowerPanel() {
		actionButton = new JButton("Print");
		setActionButtonEnabled(false);

		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						setActionButtonEnabled(false);
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						PrintJobOptions printJobOptions = new PrintJobOptions();
						String frontSideImageFile = null;
						PrintType printType = PrintType.MonoK;

						if (selectPrintFrontCheckBox.isSelected()) {
							frontSideImageFile = frontImageFileNameTextField.getText();
							printType = PrintType.valueOf((String) printTypeComboBox.getSelectedItem());
							printJobOptions.frontImageInfo.put(printType, frontSideImageFile);
						}

						String overlaySideImageFile = null;
						if (selectPrintOverlayCheckBox.isSelected()) {
							overlaySideImageFile = overlayImageFileNameTextField.getText();
							printJobOptions.frontImageInfo.put(PrintType.Overlay, overlaySideImageFile);
						}

						String backSideImageFile = null;
						if (selectPrintBackCheckBox.isSelected()) {
							backSideImageFile = backImageFileNameTextField.getText();
							printJobOptions.backImageInfo.put(PrintType.MonoK, backSideImageFile);
						}

						printJobOptions.copies = Integer.parseInt((String) printQuantityComboBox.getSelectedItem());

						try {
							demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));
							getPrinterModel().print(printJobOptions, statusTextArea);
						} catch (Exception e) {
							JOptionPane.showMessageDialog(null, "Error printing card : " + e.getLocalizedMessage());
						}

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						setActionButtonEnabled(true);

					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(actionButton, BorderLayout.LINE_END);

		JPanel lowerPart = new JPanel();
		lowerPart.setLayout(new BoxLayout(lowerPart, BoxLayout.PAGE_AXIS));
		lowerPart.add(createFrontOfCardPanel());
		lowerPart.add(createFrontOverlayPanel());
		lowerPart.add(createBackMonoPanel());
		lowerPart.add(createPrintOptionsPanel());
		lowerPart.add(createJobStatusPanel(10, 92));
		lowerPart.add(Box.createVerticalStrut(10));
		lowerPart.add(buttonHolder, BorderLayout.PAGE_END);
		lowerPart.setBorder(new EmptyBorder(10, 10, 10, 10));
		return lowerPart;
	}

	private JPanel createFrontOfCardPanel() {
		JPanel frontOfCardArea = new JPanel();
		frontOfCardArea.setLayout(new BoxLayout(frontOfCardArea, BoxLayout.PAGE_AXIS));
		frontOfCardArea.setBorder(new TitledBorder("Front"));

		JPanel selectPrintFrontArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));

		selectPrintFrontCheckBox = new JCheckBox();
		selectPrintFrontArea.add(selectPrintFrontCheckBox);
		frontOfCardArea.add(selectPrintFrontArea);

		JLabel selectPrintFrontLabel = new JLabel("Print front side");
		selectPrintFrontArea.add(selectPrintFrontLabel);

		JPanel frontImageFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Image File");
		frontImageFileArea.add(fileNameLabel);

		frontImageFileNameTextField = new JTextField();
		frontImageFileNameTextField.setPreferredSize(new Dimension(350, 25));
		frontImageFileArea.add(frontImageFileNameTextField);

		final JButton frontImageBrowseButton = new JButton("Browse");
		frontImageFileArea.add(frontImageBrowseButton);
		frontOfCardArea.add(frontImageFileArea);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Image Files (*.bmp)";
			}

			@Override
			public boolean accept(File f) {
				String fileExt = f.getName().toLowerCase();
				return fileExt.endsWith(".bmp") || f.isDirectory();
			}
		});

		frontImageBrowseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					System.out.println("Front panel image selected");
					frontImageFileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
				}
			}
		});

		JPanel printTypeArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel printTypeLabel = new JLabel("Type");
		printTypeArea.add(printTypeLabel);
		printTypeComboBox = new JComboBox(new String[] { PRINT_TYPE_COLOR, PRINT_TYPE_MONO });
		printTypeArea.add(printTypeComboBox);
		frontOfCardArea.add(printTypeArea);

		frontImageFileNameTextField.setEditable(false);
		frontImageBrowseButton.setEnabled(false);
		printTypeComboBox.setEnabled(false);
		selectPrintFrontCheckBox.addChangeListener(new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				boolean shouldEnable = selectPrintFrontCheckBox.isSelected();
				frontImageFileNameTextField.setEditable(shouldEnable);
				frontImageBrowseButton.setEnabled(shouldEnable);
				printTypeComboBox.setEnabled(shouldEnable);
				checkIfImageSpecified();
			}
		});
		return frontOfCardArea;
	}

	private void checkIfImageSpecified() {
		boolean atLeastOneImageSpecified = selectPrintFrontCheckBox.isSelected() || selectPrintOverlayCheckBox.isSelected() || selectPrintBackCheckBox.isSelected();
		boolean printerListNotEmpty = getPrinterModel().getConnection() != null;
		setActionButtonEnabled(atLeastOneImageSpecified && printerListNotEmpty);
	}

	private JPanel createFrontOverlayPanel() {
		JPanel frontOverlayArea = new JPanel();
		frontOverlayArea.setLayout(new BoxLayout(frontOverlayArea, BoxLayout.PAGE_AXIS));
		frontOverlayArea.setBorder(new TitledBorder("Front - Overlay"));

		JPanel selectPrintFrontArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));

		selectPrintOverlayCheckBox = new JCheckBox();
		selectPrintFrontArea.add(selectPrintOverlayCheckBox);
		frontOverlayArea.add(selectPrintFrontArea);

		JLabel selectPrintFrontLabel = new JLabel("Print front side overlay");
		selectPrintFrontArea.add(selectPrintFrontLabel);

		JPanel overlayImageFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Image File");
		overlayImageFileArea.add(fileNameLabel);

		overlayImageFileNameTextField = new JTextField();
		overlayImageFileNameTextField.setPreferredSize(new Dimension(350, 25));
		overlayImageFileArea.add(overlayImageFileNameTextField);

		final JButton browseButton = new JButton("Browse");
		overlayImageFileArea.add(browseButton);
		frontOverlayArea.add(overlayImageFileArea);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Image Files (*.bmp)";
			}

			@Override
			public boolean accept(File f) {
				String fileExt = f.getName().toLowerCase();
				return fileExt.endsWith(".bmp") || f.isDirectory();
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					overlayImageFileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
				}
			}
		});

		overlayImageFileNameTextField.setEditable(false);
		browseButton.setEnabled(false);
		selectPrintOverlayCheckBox.addChangeListener(new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				boolean shouldEnable = selectPrintOverlayCheckBox.isSelected();
				overlayImageFileNameTextField.setEditable(shouldEnable);
				browseButton.setEnabled(shouldEnable);
				checkIfImageSpecified();
			}
		});

		return frontOverlayArea;
	}

	private JPanel createBackMonoPanel() {
		JPanel backMonoArea = new JPanel();
		backMonoArea.setLayout(new BoxLayout(backMonoArea, BoxLayout.PAGE_AXIS));
		backMonoArea.setBorder(new TitledBorder("Back - Mono"));

		JPanel selectPrintFrontArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));

		selectPrintBackCheckBox = new JCheckBox();
		selectPrintFrontArea.add(selectPrintBackCheckBox);
		backMonoArea.add(selectPrintFrontArea);

		JLabel selectPrintFrontLabel = new JLabel("Print back side (mono only)");
		selectPrintFrontArea.add(selectPrintFrontLabel);

		JPanel backImageFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Image File");
		backImageFileArea.add(fileNameLabel);

		backImageFileNameTextField = new JTextField();
		backImageFileNameTextField.setPreferredSize(new Dimension(350, 25));
		backImageFileArea.add(backImageFileNameTextField);

		final JButton browseButton = new JButton("Browse");
		backImageFileArea.add(browseButton);
		backMonoArea.add(backImageFileArea);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Image Files (*.bmp)";
			}

			@Override
			public boolean accept(File f) {
				String fileExt = f.getName().toLowerCase();
				return fileExt.endsWith(".bmp") || f.isDirectory();
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					backImageFileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
				}
			}
		});

		backImageFileNameTextField.setEditable(false);
		browseButton.setEnabled(false);
		selectPrintBackCheckBox.addChangeListener(new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				boolean shouldEnable = selectPrintBackCheckBox.isSelected();
				backImageFileNameTextField.setEditable(shouldEnable);
				browseButton.setEnabled(shouldEnable);
				checkIfImageSpecified();
			}
		});

		return backMonoArea;
	}

	private JPanel createPrintOptionsPanel() {
		JPanel printOptionsArea = new JPanel();
		printOptionsArea.setLayout(new BoxLayout(printOptionsArea, BoxLayout.PAGE_AXIS));
		printOptionsArea.setBorder(new TitledBorder("Print Options"));

		JPanel printQuantityArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel printTypeLabel = new JLabel("Quantity");
		printQuantityArea.add(printTypeLabel);
		printQuantityComboBox = new JComboBox(new String[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
		printQuantityArea.add(printQuantityComboBox);
		printOptionsArea.add(printQuantityArea);

		return printOptionsArea;
	}

}
