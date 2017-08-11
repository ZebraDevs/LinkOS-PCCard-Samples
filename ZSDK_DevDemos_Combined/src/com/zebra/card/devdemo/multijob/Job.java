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

package com.zebra.card.devdemo.multijob;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.util.HashMap;
import java.util.Map;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JDialog;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.filechooser.FileFilter;

import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.common.card.enumerations.CardDestination;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.CoercivityType;
import com.zebra.sdk.common.card.enumerations.PrintOptimizationMode;
import com.zebra.sdk.common.card.enumerations.PrintType;

public class Job {

	private JDialog demoDialog;

	public Map<PrintType, String> frontImageInfo;
	public Map<PrintType, String> backImageInfo;

	private JPanel panel;

	private JButton frontSubmitButton;
	private JButton backSubmitButton;

	public JCheckBox frontCheckBox;
	public JCheckBox backCheckBox;
	private JCheckBox frontOverlayCheckBox;
	private JCheckBox backOverlayCheckBox;
	public JCheckBox magEncodeCheckBox;

	public JComboBox sourceComboBox;
	public JComboBox destinationComboBox;
	public JComboBox printOptimizationComboBox;
	public JComboBox quantityComboBox;
	public JComboBox coercivityTypeComboBox;
	private JComboBox frontPrintTypeComboBox;
	private JComboBox backPrintTypeComboBox;

	public JLabel printOptimizationLabel;
	public JLabel jobStatusLabel;

	public JTextField jobStatus;
	private JTextField frontFileNameTextField;
	private JTextField backFileNameTextField;
	public JTextField track1DataTextField;
	public JTextField track2DataTextField;
	public JTextField track3DataTextField;

	private ActionListener frontSubmitButtonListener;
	private ActionListener frontPrintTypeComboBoxListener;
	private ActionListener backSubmitButtonListener;
	private ActionListener backPrintTypeComboBoxListener;

	private ChangeListener frontCheckBoxListener;
	private ChangeListener frontOverlayCheckBoxListener;
	private ChangeListener backCheckBoxListener;
	private ChangeListener backOverlayCheckBoxListener;
	private ChangeListener magEncodeCheckBoxListener;

	public JPanel createJob(JDialog dialog) {
		demoDialog = dialog;
		panel = new JPanel();

		frontImageInfo = new HashMap<PrintType, String>();
		backImageInfo = new HashMap<PrintType, String>();

		frontFileNameTextField = new JTextField();
		backFileNameTextField = new JTextField();
		track1DataTextField = new JTextField();
		track2DataTextField = new JTextField();
		track3DataTextField = new JTextField();

		sourceComboBox = new JComboBox();
		destinationComboBox = new JComboBox();
		printOptimizationComboBox = new JComboBox();
		quantityComboBox = new JComboBox();
		frontPrintTypeComboBox = new JComboBox();
		backPrintTypeComboBox = new JComboBox();
		coercivityTypeComboBox = new JComboBox();

		frontSubmitButton = new JButton("Add Image");
		backSubmitButton = new JButton("Add Image");

		frontCheckBox = new JCheckBox();
		backCheckBox = new JCheckBox();
		frontOverlayCheckBox = new JCheckBox();
		backOverlayCheckBox = new JCheckBox();
		magEncodeCheckBox = new JCheckBox();

		setUpPrintPanels();
		return panel;
	}

	public void cleanUp() {
		frontCheckBox.setSelected(false);
		backCheckBox.setSelected(false);
		frontOverlayCheckBox.setSelected(false);
		backOverlayCheckBox.setSelected(false);
		magEncodeCheckBox.setSelected(false);

		frontFileNameTextField.setText("");
		backFileNameTextField.setText("");
		track1DataTextField.setText("");
		track2DataTextField.setText("");
		track3DataTextField.setText("");

		frontImageInfo.clear();
		backImageInfo.clear();

		frontSubmitButton.removeActionListener(frontSubmitButtonListener);
		backSubmitButton.removeActionListener(backSubmitButtonListener);
		frontPrintTypeComboBox.removeActionListener(frontPrintTypeComboBoxListener);
		backPrintTypeComboBox.removeActionListener(backPrintTypeComboBoxListener);

		frontCheckBox.removeChangeListener(frontCheckBoxListener);
		backCheckBox.removeChangeListener(backCheckBoxListener);
		frontOverlayCheckBox.removeChangeListener(frontOverlayCheckBoxListener);
		backOverlayCheckBox.removeChangeListener(backOverlayCheckBoxListener);
		magEncodeCheckBox.removeChangeListener(magEncodeCheckBoxListener);
	}

	public void setUpPrintPanels(JobSettingOptions printOptions) {
		panel.removeAll();
		JPanel lowerPart = createLowerPanel(printOptions);
		panel.add(lowerPart, BorderLayout.PAGE_END);
	}

	private void setUpPrintPanels() {
		JPanel lowerPart = createLowerPanel(new JobSettingOptions());
		panel.add(lowerPart, BorderLayout.PAGE_END);
	}

	private JPanel createLowerPanel(JobSettingOptions printOptions) {
		JPanel lowerPart = new JPanel();
		lowerPart.setLayout(new BoxLayout(lowerPart, BoxLayout.PAGE_AXIS));
		lowerPart.add(createJobSettingPanel(printOptions));
		lowerPart.add(createFrontOfCardPanel(printOptions));
		lowerPart.add(createBackOfCardPanel(printOptions));
		lowerPart.add(createMagEncodingPanel(printOptions.showMagEncoding));
		lowerPart.setBorder(new EmptyBorder(10, 10, 10, 10));
		return lowerPart;
	}

	private JPanel createJobSettingPanel(JobSettingOptions printOptions) {
		JPanel jobSettingPanel = new JPanel();
		jobSettingPanel.setLayout(new BoxLayout(jobSettingPanel, BoxLayout.PAGE_AXIS));
		jobSettingPanel.setBorder(new TitledBorder("Job Settings"));

		JPanel jobSettingsArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel sourceLabel = new JLabel("Source");
		jobSettingsArea.add(sourceLabel);

		sourceComboBox.removeAllItems();
		for (CardSource source : CardSource.values()) {
			if (!printOptions.cardSourceRange.isEmpty() && printOptions.cardSourceRange.contains(source.name())) {
				sourceComboBox.addItem(source.name());
			}
		}

		sourceComboBox.setSelectedItem(CardSource.Feeder.name());
		jobSettingsArea.add(sourceComboBox);

		JLabel destinationLabel = new JLabel("Destination");
		jobSettingsArea.add(destinationLabel);

		destinationComboBox.removeAllItems();
		for (CardDestination destination : CardDestination.values()) {
			if (!printOptions.cardDestinationRange.isEmpty() && printOptions.cardDestinationRange.contains(destination.name())) {
				if (!destination.name().contains("Laminator") || printOptions.showLamDestinations) {
					destinationComboBox.addItem(destination.name());
				}
			}
		}

		destinationComboBox.setSelectedItem(CardDestination.Eject.name());
		jobSettingsArea.add(destinationComboBox);

		printOptimizationLabel = new JLabel("Print Optimization");
		jobSettingsArea.add(printOptimizationLabel);
		printOptimizationLabel.setVisible(printOptions.showPrintOptimization);

		printOptimizationComboBox.removeAllItems();
		for (PrintOptimizationMode mode : PrintOptimizationMode.values()) {
			printOptimizationComboBox.addItem(mode.name());
		}

		printOptimizationComboBox.setSelectedItem(PrintOptimizationMode.Speed.name());
		printOptimizationComboBox.setVisible(printOptions.showPrintOptimization);
		jobSettingsArea.add(printOptimizationComboBox);

		JLabel quantityLabel = new JLabel("Quantity");
		jobSettingsArea.add(quantityLabel);
		quantityComboBox.removeAllItems();

		quantityComboBox.addItem(1);
		quantityComboBox.addItem(2);
		quantityComboBox.addItem(3);
		quantityComboBox.addItem(4);
		quantityComboBox.addItem(5);
		jobSettingsArea.add(quantityComboBox);

		jobSettingPanel.add(jobSettingsArea);
		return jobSettingPanel;
	}

	private JPanel createFrontOfCardPanel(JobSettingOptions printOptions) {
		JPanel frontOfCardArea = new JPanel();
		frontOfCardArea.setLayout(new BoxLayout(frontOfCardArea, BoxLayout.PAGE_AXIS));
		frontOfCardArea.setBorder(new TitledBorder("Front"));

		JPanel selectPrintFrontArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		selectPrintFrontArea.add(frontCheckBox);
		frontOfCardArea.add(selectPrintFrontArea);

		JLabel selectPrintFrontLabel = new JLabel("Print front side");
		selectPrintFrontArea.add(selectPrintFrontLabel);

		JPanel frontImageFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Image File");
		frontImageFileArea.add(fileNameLabel);

		frontFileNameTextField.setPreferredSize(new Dimension(350, 25));
		frontImageFileArea.add(frontFileNameTextField);

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
					frontFileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
				}
			}
		});

		JPanel printTypeArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel printTypeLabel = new JLabel("Type");
		printTypeArea.add(printTypeLabel);

		frontPrintTypeComboBox.removeAllItems();

		if (printOptions.allowsColorOption) {
			frontPrintTypeComboBox.addItem(PrintType.Color.name());
		}

		if (printOptions.allowsMonoOption) {
			frontPrintTypeComboBox.addItem(PrintType.MonoK.name());
		}

		if (printOptions.allowsOverlayOption) {
			frontPrintTypeComboBox.addItem(PrintType.Overlay.name());
		}

		printTypeArea.add(frontPrintTypeComboBox);

		frontOverlayCheckBox = new JCheckBox("Full Overlay");
		printTypeArea.add(frontOverlayCheckBox);
		printTypeArea.add(frontSubmitButton);
		frontSubmitButton.addActionListener(frontSubmitButtonListener = new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				String type = frontPrintTypeComboBox.getSelectedItem().toString();
				String imageLocation = frontFileNameTextField.getText();
				boolean successfullyAdded = false;

				File file = new File(imageLocation);
				if (file.exists() && !file.isDirectory()) {
					if (!imageLocation.isEmpty()) {
						frontImageInfo.put(PrintType.valueOf(type), imageLocation);
						successfullyAdded = true;
					}
				} else if (type.equals(PrintType.Overlay.name()) && frontOverlayCheckBox.isSelected()) {
					frontImageInfo.put(PrintType.valueOf(type), null);
					successfullyAdded = true;
				}

				if (successfullyAdded) {
					PrinterModel.showInformationDialog("Front Image", "The " + type + " image was added successfully.");
				} else {
					PrinterModel.showInformationDialog("Front Image", "Unable to add the " + type + " image.\nCheck your filepath and try again.");
				}
			}
		});

		frontOfCardArea.add(printTypeArea);

		frontFileNameTextField.setEditable(false);
		frontImageBrowseButton.setEnabled(false);
		frontPrintTypeComboBox.setEnabled(false);
		frontOverlayCheckBox.setVisible(false);
		frontSubmitButton.setEnabled(false);

		frontCheckBox.addChangeListener(frontCheckBoxListener = new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				boolean shouldEnable = frontCheckBox.isSelected();
				frontFileNameTextField.setEditable(shouldEnable);
				frontImageBrowseButton.setEnabled(shouldEnable);
				frontOverlayCheckBox.setEnabled(shouldEnable);
				frontPrintTypeComboBox.setEnabled(shouldEnable);
				frontSubmitButton.setEnabled(shouldEnable);
			}
		});

		frontPrintTypeComboBox.addActionListener(frontPrintTypeComboBoxListener = new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				String selectedItem = frontPrintTypeComboBox.getSelectedItem().toString();
				if (selectedItem != null) {
					if (selectedItem.equalsIgnoreCase(PrintType.Overlay.name())) {
						frontOverlayCheckBox.setVisible(true);
						if (frontOverlayCheckBox.isSelected()) {
							frontFileNameTextField.setText("");
							frontFileNameTextField.setEditable(false);
							frontImageBrowseButton.setEnabled(false);
						}
					} else {
						frontOverlayCheckBox.setVisible(false);
						frontFileNameTextField.setEditable(true);
						frontImageBrowseButton.setEnabled(true);
					}
				}
			}
		});

		frontOverlayCheckBox.addChangeListener(frontOverlayCheckBoxListener = new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				if (frontOverlayCheckBox.isSelected()) {
					frontFileNameTextField.setText("");
					frontFileNameTextField.setEditable(false);
					frontImageBrowseButton.setEnabled(false);
				} else {
					frontFileNameTextField.setEditable(true);
					frontImageBrowseButton.setEnabled(true);
				}
			}
		});

		return frontOfCardArea;
	}

	private JPanel createBackOfCardPanel(JobSettingOptions printOptions) {
		JPanel backOfCardPanel = new JPanel();

		if (printOptions.showBackSidePrint) {
			backOfCardPanel.setLayout(new BoxLayout(backOfCardPanel, BoxLayout.PAGE_AXIS));
			backOfCardPanel.setBorder(new TitledBorder("Back"));

			JPanel selectPrintBackArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));

			selectPrintBackArea.add(backCheckBox);
			backOfCardPanel.add(selectPrintBackArea);

			JLabel selectPrintBackLabel = new JLabel("Print back side");
			selectPrintBackArea.add(selectPrintBackLabel);

			JPanel backImageFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			JLabel fileNameLabel = new JLabel("Image File");
			backImageFileArea.add(fileNameLabel);

			backFileNameTextField.setPreferredSize(new Dimension(350, 25));
			backImageFileArea.add(backFileNameTextField);

			final JButton browseButton = new JButton("Browse");
			backImageFileArea.add(browseButton);
			backOfCardPanel.add(backImageFileArea);

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
						backFileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
					}
				}
			});

			JPanel printTypeArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			JLabel printTypeLabel = new JLabel("Type");
			printTypeArea.add(printTypeLabel);

			backPrintTypeComboBox.removeAllItems();
			if (printOptions.allowsColorOption) {
				backPrintTypeComboBox.addItem(PrintType.Color.name());
			}

			if (printOptions.allowsMonoOption) {
				backPrintTypeComboBox.addItem(PrintType.MonoK.name());
			}

			if (printOptions.allowsOverlayOption) {
				backPrintTypeComboBox.addItem(PrintType.Overlay.name());
			}

			printTypeArea.add(backPrintTypeComboBox);

			backOverlayCheckBox = new JCheckBox("Full Overlay");
			printTypeArea.add(backOverlayCheckBox);
			printTypeArea.add(backSubmitButton);

			backSubmitButton.addActionListener(backSubmitButtonListener = new ActionListener() {

				@Override
				public void actionPerformed(ActionEvent e) {
					String type = backPrintTypeComboBox.getSelectedItem().toString();
					String imageLocation = backFileNameTextField.getText();
					boolean successfullyAdded = false;

					File file = new File(imageLocation);
					if (file.exists() && !file.isDirectory()) {
						if (!imageLocation.isEmpty()) {
							backImageInfo.put(PrintType.valueOf(type), imageLocation);
							successfullyAdded = true;
						}
					} else if (type.equals(PrintType.Overlay.name()) && backOverlayCheckBox.isSelected()) {
						backImageInfo.put(PrintType.valueOf(type), null);
						successfullyAdded = true;
					}

					if (successfullyAdded) {
						PrinterModel.showInformationDialog("Back Image", "The " + type + " image was added successfully.");
					} else {
						PrinterModel.showInformationDialog("Back Image", "Unable to add the " + type + " image.\nCheck your filepath and try again.");
					}
				}
			});

			backOfCardPanel.add(printTypeArea);

			backFileNameTextField.setEditable(false);
			browseButton.setEnabled(false);
			backPrintTypeComboBox.setEnabled(false);
			backOverlayCheckBox.setVisible(false);
			backSubmitButton.setEnabled(false);

			backCheckBox.addChangeListener(backCheckBoxListener = new ChangeListener() {

				@Override
				public void stateChanged(ChangeEvent e) {
					boolean shouldEnable = backCheckBox.isSelected();
					backFileNameTextField.setEditable(shouldEnable);
					browseButton.setEnabled(shouldEnable);
					backPrintTypeComboBox.setEnabled(shouldEnable);
					backOverlayCheckBox.setEnabled(shouldEnable);
					backSubmitButton.setEnabled(shouldEnable);
				}
			});

			backPrintTypeComboBox.addActionListener(backPrintTypeComboBoxListener = new ActionListener() {

				@Override
				public void actionPerformed(ActionEvent e) {
					String selectedItem = backPrintTypeComboBox.getSelectedItem().toString();
					if (selectedItem != null) {
						if (backPrintTypeComboBox.getSelectedItem().toString().equalsIgnoreCase(PrintType.Overlay.name())) {
							backOverlayCheckBox.setVisible(true);
							if (backOverlayCheckBox.isSelected()) {
								backFileNameTextField.setText("");
								backFileNameTextField.setEditable(false);
								browseButton.setEnabled(false);
							}
						} else {
							backOverlayCheckBox.setVisible(false);
							backFileNameTextField.setEditable(true);
							browseButton.setEnabled(true);
						}
					}
				}
			});

			backOverlayCheckBox.addChangeListener(backOverlayCheckBoxListener = new ChangeListener() {

				@Override
				public void stateChanged(ChangeEvent e) {
					if (backOverlayCheckBox.isSelected()) {
						backFileNameTextField.setText("");
						backFileNameTextField.setEditable(false);
						browseButton.setEnabled(false);
					} else {
						backFileNameTextField.setEditable(true);
						browseButton.setEnabled(true);
					}
				}
			});
		}

		return backOfCardPanel;
	}

	private JPanel createMagEncodingPanel(boolean showMagEncoding) {
		JPanel magEncodeArea = new JPanel();
		if (showMagEncoding) {
			magEncodeArea.setLayout(new BoxLayout(magEncodeArea, BoxLayout.PAGE_AXIS));
			magEncodeArea.setBorder(new TitledBorder("Mag - Encode"));

			JPanel selectMagArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			selectMagArea.add(magEncodeCheckBox);
			magEncodeArea.add(selectMagArea);

			JLabel selectPrintMagLabel = new JLabel("Magnetic Encode");
			selectMagArea.add(selectPrintMagLabel);

			selectMagArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 120, 10)));
			JLabel selectCoercivityTypeLabel = new JLabel("Coercivity Type");
			selectMagArea.add(selectCoercivityTypeLabel);

			coercivityTypeComboBox.removeAllItems();
			coercivityTypeComboBox.addItem(CoercivityType.High.name());
			coercivityTypeComboBox.addItem(CoercivityType.Low.name());
			coercivityTypeComboBox.setEnabled(false);
			selectMagArea.add(coercivityTypeComboBox);

			JPanel track1FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			JLabel track1DataLabel = new JLabel("Track 1 data");
			track1FileArea.add(track1DataLabel);

			track1DataTextField.setPreferredSize(new Dimension(350, 25));
			track1FileArea.add(track1DataTextField);
			magEncodeArea.add(track1FileArea);

			JPanel track2FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			JLabel track2DataLabel = new JLabel("Track 2 data");
			track2FileArea.add(track2DataLabel);

			track2DataTextField.setPreferredSize(new Dimension(350, 25));
			track2FileArea.add(track2DataTextField);
			magEncodeArea.add(track2FileArea);

			JPanel track3FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
			JLabel track3DataLabel = new JLabel("Track 3 data");
			track3FileArea.add(track3DataLabel);

			track3DataTextField.setPreferredSize(new Dimension(350, 25));
			track3FileArea.add(track3DataTextField);
			magEncodeArea.add(track3FileArea);

			track1DataTextField.setEnabled(false);
			track2DataTextField.setEnabled(false);
			track3DataTextField.setEnabled(false);

			magEncodeCheckBox.addChangeListener(magEncodeCheckBoxListener = new ChangeListener() {

				@Override
				public void stateChanged(ChangeEvent e) {
					boolean shouldEnable = magEncodeCheckBox.isSelected();
					coercivityTypeComboBox.setEnabled(shouldEnable);
					track1DataTextField.setEnabled(shouldEnable);
					track2DataTextField.setEnabled(shouldEnable);
					track3DataTextField.setEnabled(shouldEnable);
				}
			});
		}

		return magEncodeArea;
	}
}
