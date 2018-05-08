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

package com.zebra.card.devdemo.graphicconversion;

import java.awt.BorderLayout;
import java.awt.Component;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;
import java.io.File;
import java.io.IOException;

import javax.swing.BoxLayout;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.filechooser.FileFilter;

import org.apache.commons.io.FilenameUtils;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.graphicconversion.GraphicConverter.DimensionOption;

public class GraphicConversionDemo extends PrinterDemoBase<GraphicConversionModel> {

	protected JTextField fileNameTextField;
	protected JTextField convertedFileNameTextField;

	private JTextArea outputTextArea;

	private JComboBox graphicsFormatComboBox;
	private JComboBox printerModelComboBox;

	private JTextField widthTextField;
	private JTextField heightTextField;
	private JTextField xOffsetTextField;
	private JTextField yOffsetTextField;

	private JRadioButton originalSizeButton;
	private JRadioButton resizeButton;
	private JRadioButton cropButton;

	public GraphicConversionDemo() {
		super(new GraphicConversionModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Graphic Conversion"), BorderLayout.PAGE_START);
		container.add(createLowerPanel(), BorderLayout.PAGE_END);
	}

	private JPanel createLowerPanel() {
		JPanel lowerPanel = new JPanel();
		lowerPanel.setLayout(new BoxLayout(lowerPanel, BoxLayout.PAGE_AXIS));
		lowerPanel.setBorder(new EmptyBorder(10, 10, 10, 10));

		lowerPanel.add(createFilenamePanel());
		lowerPanel.add(createOptionsPanel());
		lowerPanel.add(createDimensionsPanel());
		lowerPanel.add(createOutputFilenamePanel());
		lowerPanel.add(createOutputPanel());

		JButton convertButton = new JButton("Convert");
		convertButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));
						outputTextArea.setText("");

						try {
							GraphicsContainer graphicsContainer = buildGraphicsContainer();
							GraphicConverter converter = new GraphicConverter(graphicsContainer, outputTextArea);
							converter.processImage();
						} catch (Exception e) {
							JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
						}

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(convertButton, BorderLayout.LINE_END);
		lowerPanel.add(buttonHolder);
		return lowerPanel;
	}

	private GraphicsContainer buildGraphicsContainer() {
		GraphicsContainer graphicsContainer = new GraphicsContainer();

		graphicsContainer.format = (String) graphicsFormatComboBox.getSelectedItem();
		graphicsContainer.outputFilePath = convertedFileNameTextField.getText() + ".bmp";
		graphicsContainer.inputFilePath = fileNameTextField.getText();
		graphicsContainer.printerModelInfo = (PrinterModelInfo) printerModelComboBox.getSelectedItem();
		graphicsContainer.widthString = widthTextField.getText();
		graphicsContainer.heightString = heightTextField.getText();
		graphicsContainer.xoffsetString = xOffsetTextField.getText();
		graphicsContainer.yoffsetString = yOffsetTextField.getText();
		graphicsContainer.dimensionOption = getDimensionOption();

		return graphicsContainer;
	}

	private DimensionOption getDimensionOption() {
		DimensionOption option = DimensionOption.original;
		if (originalSizeButton.isSelected()) {
			option = DimensionOption.original;
		} else if (resizeButton.isSelected()) {
			option = DimensionOption.resize;
		} else if (cropButton.isSelected()) {
			option = DimensionOption.crop;
		}
		return option;
	}

	private JPanel createOutputFilenamePanel() {
		JPanel fileNameArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		fileNameArea.setBorder(new TitledBorder("Output File"));

		JLabel fileNameLabel = new JLabel("Filename of Converted Graphic");
		fileNameArea.add(fileNameLabel);

		JPanel convertedFileNameArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 0, 0));

		convertedFileNameTextField = new JTextField();
		convertedFileNameTextField.setPreferredSize(new Dimension(350, 25));
		convertedFileNameArea.add(convertedFileNameTextField);
		convertedFileNameArea.add(new JLabel(".bmp"));

		fileNameArea.add(convertedFileNameArea);

		JButton browseButton = new JButton("Browse");
		fileNameArea.add(browseButton);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "all";
			}

			@Override
			public boolean accept(File f) {
				return true;
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					convertedFileNameTextField.setText(FilenameUtils.removeExtension(fileChooser.getSelectedFile().getAbsolutePath()));
				}
			}
		});

		return fileNameArea;
	}

	private JPanel createFilenamePanel() {
		JPanel fileNameArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		fileNameArea.setBorder(new TitledBorder("Input"));

		JLabel fileNameLabel = new JLabel("Filename of Graphic");
		fileNameArea.add(fileNameLabel);

		fileNameTextField = new JTextField();
		fileNameTextField.setPreferredSize(new Dimension(350, 25));
		fileNameArea.add(fileNameTextField);

		JButton browseButton = new JButton("Browse");
		fileNameArea.add(browseButton);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Image Files (*.bmp, *.jpg, *.png)";
			}

			@Override
			public boolean accept(File f) {
				String fileExt = f.getName().toLowerCase();
				return fileExt.endsWith(".bmp") || fileExt.endsWith(".png") || fileExt.endsWith(".jpg") || f.isDirectory();
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					fileNameTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
				}
			}
		});

		return fileNameArea;
	}

	private Component createOutputPanel() {
		JPanel outputPanel = new JPanel();
		outputPanel.setBorder(new TitledBorder("Output"));
		outputTextArea = new JTextArea(15, 55);
		outputPanel.add(outputTextArea);
		return outputPanel;
	}

	private Component createDimensionsPanel() {
		JPanel outerPanel = new JPanel(new BorderLayout());
		outerPanel.setBorder(new TitledBorder("Dimensions"));

		JPanel dimensionsAlignmentWrapper = new JPanel(new BorderLayout());
		JPanel radioButtonsArea = new JPanel();

		ItemListener radiobuttonStateListener = new ItemListener() {

			@Override
			public void itemStateChanged(ItemEvent evt) {
				if (widthTextField != null && heightTextField != null && xOffsetTextField != null && yOffsetTextField != null) {
					if (originalSizeButton.isSelected()) {
						widthTextField.setEditable(false);
						heightTextField.setEditable(false);
						xOffsetTextField.setEditable(false);
						yOffsetTextField.setEditable(false);
					} else if (resizeButton.isSelected()) {
						widthTextField.setEditable(true);
						heightTextField.setEditable(true);
						xOffsetTextField.setEditable(false);
						yOffsetTextField.setEditable(false);
					} else {
						widthTextField.setEditable(true);
						heightTextField.setEditable(true);
						xOffsetTextField.setEditable(true);
						yOffsetTextField.setEditable(true);
					}

					if (resizeButton.isSelected() || cropButton.isSelected()) {
						String inputFilePath = fileNameTextField.getText();
						if (!inputFilePath.isEmpty()) {
							if ((heightTextField.getText().isEmpty()) && (widthTextField.getText().isEmpty())) {
								try {
									int[] heightAndWidth = GraphicConverter.getImageSize(inputFilePath);
									heightTextField.setText(Integer.toString(heightAndWidth[0]));
									widthTextField.setText(Integer.toString(heightAndWidth[1]));
								} catch (IOException e) {
									JOptionPane.showMessageDialog(null, "Error accessing file : " + inputFilePath + ":" + e.getLocalizedMessage());
								}
							}
						}
					}
				}
			}
		};

		originalSizeButton = new JRadioButton("Original");
		resizeButton = new JRadioButton("Resize");
		cropButton = new JRadioButton("Crop");

		radioButtonsArea.add(originalSizeButton);
		radioButtonsArea.add(resizeButton);
		radioButtonsArea.add(cropButton);

		ButtonGroup dimensonsButtonGroup = new ButtonGroup();
		dimensonsButtonGroup.add(originalSizeButton);
		dimensonsButtonGroup.add(resizeButton);
		dimensonsButtonGroup.add(cropButton);

		originalSizeButton.setSelected(true);

		originalSizeButton.addItemListener(radiobuttonStateListener);
		resizeButton.addItemListener(radiobuttonStateListener);
		cropButton.addItemListener(radiobuttonStateListener);

		dimensionsAlignmentWrapper.add(radioButtonsArea, BorderLayout.WEST);
		outerPanel.add(dimensionsAlignmentWrapper, BorderLayout.NORTH);
		outerPanel.add(createPositionFields(), BorderLayout.SOUTH);
		return outerPanel;
	}

	private JPanel createPositionFields() {
		JPanel positionFields = new JPanel(new GridLayout(2, 2));

		JPanel widthArea = new JPanel(new GridLayout(1, 2));
		widthArea.setBorder(new EmptyBorder(10, 10, 10, 10));

		JLabel width = new JLabel("Width (in pixels)");
		widthArea.add(width);

		widthTextField = new JTextField();
		widthTextField.setEditable(false);
		widthArea.add(widthTextField);

		JPanel heightArea = new JPanel(new GridLayout(1, 2));
		heightArea.setBorder(new EmptyBorder(10, 10, 10, 10));

		JLabel height = new JLabel("Height (in pixels)");
		heightArea.add(height);

		heightTextField = new JTextField();
		heightTextField.setEditable(false);
		heightArea.add(heightTextField);

		JPanel xOffsetArea = new JPanel(new GridLayout(1, 2));
		xOffsetArea.setBorder(new EmptyBorder(10, 10, 10, 10));

		JLabel xOffset = new JLabel("X-Offset (in pixels)");
		xOffsetArea.add(xOffset);

		xOffsetTextField = new JTextField();
		xOffsetTextField.setEditable(false);
		xOffsetArea.add(xOffsetTextField);

		JPanel yOffsetArea = new JPanel(new GridLayout(1, 2));
		yOffsetArea.setBorder(new EmptyBorder(10, 10, 10, 10));

		JLabel yOffset = new JLabel("Y-Offset (in pixels)");
		yOffsetArea.add(yOffset);

		yOffsetTextField = new JTextField();
		yOffsetTextField.setEditable(false);
		yOffsetArea.add(yOffsetTextField);

		positionFields.add(widthArea);
		positionFields.add(heightArea);
		positionFields.add(xOffsetArea);
		positionFields.add(yOffsetArea);
		return positionFields;
	}

	private Component createOptionsPanel() {
		JPanel optionsAreaWrapper = new JPanel(new BorderLayout());
		optionsAreaWrapper.setBorder(new TitledBorder("Options"));

		JPanel optionsArea = new JPanel(new GridLayout(2, 2, 10, 10));
		optionsArea.setBorder(new EmptyBorder(10, 10, 10, 10));

		JLabel grsphicsFormatLabel = new JLabel("Graphics Format");
		optionsArea.add(grsphicsFormatLabel);

		graphicsFormatComboBox = new JComboBox(new String[] { GraphicConverter.GRAPHICS_FORMAT_COLOR, GraphicConverter.GRAPHICS_FORMAT_MONO_DIFFUSION, GraphicConverter.GRAPHICS_FORMAT_MONO_HALFTONE_6X6, GraphicConverter.GRAPHICS_FORMAT_MONO_HALFTONE_8X8,
				GraphicConverter.GRAPHICS_FORMAT_GRAY_DIFFUSION, GraphicConverter.GRAY_HALFTONE_6X6, GraphicConverter.GRAY_HALFTONE_8X8 });
		optionsArea.add(graphicsFormatComboBox);

		JLabel printerModelLabelLabel = new JLabel("Printer Model");
		optionsArea.add(printerModelLabelLabel);

		printerModelComboBox = new JComboBox(
				new PrinterModelInfo[] { PrinterModelInfo.ZXP1, PrinterModelInfo.ZXP3, PrinterModelInfo.ZXP7, PrinterModelInfo.ZXP8, PrinterModelInfo.ZXP9, PrinterModelInfo.ZC100, PrinterModelInfo.ZC150, PrinterModelInfo.ZC300, PrinterModelInfo.ZC350 });
		optionsArea.add(printerModelComboBox);

		optionsAreaWrapper.add(optionsArea, BorderLayout.WEST);
		return optionsAreaWrapper;
	}

	@Override
	protected void onConnectionEstablished() {
		// Do nothing
	}
}
