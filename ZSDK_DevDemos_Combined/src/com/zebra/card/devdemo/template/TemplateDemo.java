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

package com.zebra.card.devdemo.template;

import java.awt.BorderLayout;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.imageio.ImageIO;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JDialog;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.filechooser.FileFilter;
import javax.swing.table.DefaultTableModel;

import org.apache.commons.io.IOUtils;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterDemo;
import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.sdk.common.card.containers.GraphicsInfo;

public class TemplateDemo extends PrinterDemoBase implements PrinterDemo {

	private static final int VARIABLES_TAB_INDEX = 0;

	private JTextField templateFileNameTextField;
	private JTextField imageDirectoryTextField;
	private JTable variableDataTable;
	private final JScrollPane[] previewScrollers = new JScrollPane[4];
	private DefaultTableModel variablesTableModel = null;

	public TemplateDemo() {
	}

	@Override
	public void createDemoDialog(JFrame owner) {
		demoDialog = new JDialog(owner, "Zebra Multi Platform SDK - Developer Demo", true);

		JPanel mainPane = (JPanel) demoDialog.getContentPane();

		JPanel wrapper = new JPanel(new BorderLayout());

		wrapper.add(createTopPanel(), BorderLayout.PAGE_START);
		wrapper.add(createMiddlePanel(), BorderLayout.CENTER);
		wrapper.add(createLowerPanel(), BorderLayout.PAGE_END);

		mainPane.add(wrapper);

		demoDialog.pack();
		demoDialog.setLocation((Toolkit.getDefaultToolkit().getScreenSize().width / 2) - (mainPane.getWidth() / 2), 0);
		demoDialog.setVisible(true);
		demoDialog.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
	}

	@Override
	protected void additionalPostDiscoveryAction() {
		enablePrintButtonWhenReady();
	}

	private void enablePrintButtonWhenReady() {
		boolean printerListNotEmpty = addressDropdown.getItemCount() > 0;
		String templateFileName = templateFileNameTextField.getText();
		boolean templateSpecified = (templateFileName != null) && (templateFileName.isEmpty() == false);
		enableActionButton(printerListNotEmpty && templateSpecified);
	}

	private JPanel createTopPanel() {
		JPanel topPanel = new JPanel();
		topPanel.setLayout(new BoxLayout(topPanel, BoxLayout.PAGE_AXIS));
		topPanel.add(createPanelHeader("Template"), BorderLayout.PAGE_START);
		topPanel.add(createSelectPrinterPanel());
		topPanel.add(createSelectTemplatePanel());
		topPanel.add(createSelectImageDirectoryPanel());
		return topPanel;
	}

	private JPanel createMiddlePanel() {
		JPanel middlePanel = new JPanel();
		middlePanel.setLayout(new BoxLayout(middlePanel, BoxLayout.PAGE_AXIS));
		middlePanel.add(createTabPanel());
		return middlePanel;
	}

	private JPanel createLowerPanel() {
		actionButton = new JButton("Print");
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
						try {
							TemplateModel templateModel = new TemplateModel(templateFileNameTextField.getText(), imageDirectoryTextField.getText(), statusTextArea);
							Map<String, String> variableData = getVariablesTabData();
							templateModel.print(printer, variableData, statusTextArea);
						} catch (Exception e) {
							JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
						} finally {
							demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
							enableActionButton(true);
						}
					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(actionButton, BorderLayout.LINE_END);

		JPanel lowerPart = new JPanel();
		lowerPart.setLayout(new BoxLayout(lowerPart, BoxLayout.PAGE_AXIS));
		lowerPart.add(Box.createVerticalStrut(10));
		lowerPart.add(createJobStatusPanel(10, 92));
		lowerPart.add(buttonHolder, BorderLayout.PAGE_END);
		lowerPart.setBorder(new EmptyBorder(10, 10, 10, 10));
		return lowerPart;
	}

	private JPanel createSelectTemplatePanel() {
		JPanel selectTemplateArea = new JPanel();
		selectTemplateArea.setLayout(new BoxLayout(selectTemplateArea, BoxLayout.PAGE_AXIS));
		selectTemplateArea.setBorder(new TitledBorder("Selected Template"));

		JPanel templateFileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Template");
		templateFileArea.add(fileNameLabel);

		templateFileNameTextField = new JTextField();
		templateFileNameTextField.setPreferredSize(new Dimension(350, 25));
		templateFileArea.add(templateFileNameTextField);

		final JButton browseButton = new JButton("Browse");
		templateFileArea.add(browseButton);
		selectTemplateArea.add(templateFileArea);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Template File (*.xml)";
			}

			@Override
			public boolean accept(File f) {
				String fileExt = f.getName().toLowerCase();
				return fileExt.endsWith(".xml") || f.isDirectory();
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					String templateFilePath = fileChooser.getSelectedFile().getAbsolutePath();
					templateFileNameTextField.setText(templateFilePath);

					if (imageDirectoryTextField.getText().isEmpty()) {
						String imageFileDirectory = new File(templateFilePath).getParent();
						imageDirectoryTextField.setText(imageFileDirectory);
					}

					enablePrintButtonWhenReady();

					try {
						TemplateModel templateModel = new TemplateModel(templateFilePath, imageDirectoryTextField.getText(), statusTextArea);
						List<String> fieldNames = templateModel.getTemplateFields();
						addFieldsToVariablesPane(fieldNames);
					} catch (Exception e) {
						JOptionPane.showMessageDialog(null, "Error reading template : " + templateFilePath + " : " + e.getLocalizedMessage());
					}
				}
			}
		});

		templateFileNameTextField.setEditable(true);
		browseButton.setEnabled(true);

		JPanel selectTemplatePanel = new JPanel();
		selectTemplatePanel.setLayout(new BoxLayout(selectTemplatePanel, BoxLayout.PAGE_AXIS));
		selectTemplatePanel.add(selectTemplateArea);
		selectTemplatePanel.setBorder(new EmptyBorder(0, 10, 10, 10));
		return selectTemplatePanel;
	}

	private JPanel createSelectImageDirectoryPanel() {
		JPanel selectImageDirectoryArea = new JPanel();
		selectImageDirectoryArea.setLayout(new BoxLayout(selectImageDirectoryArea, BoxLayout.PAGE_AXIS));
		selectImageDirectoryArea.setBorder(new TitledBorder("Image Files"));

		JPanel imageDirectoryArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel fileNameLabel = new JLabel("Image Directory");
		imageDirectoryArea.add(fileNameLabel);

		imageDirectoryTextField = new JTextField();
		imageDirectoryTextField.setPreferredSize(new Dimension(350, 25));
		imageDirectoryArea.add(imageDirectoryTextField);

		final JButton browseButton = new JButton("Browse");
		imageDirectoryArea.add(browseButton);
		selectImageDirectoryArea.add(imageDirectoryArea);

		final JFileChooser fileChooser = new JFileChooser();
		fileChooser.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
		fileChooser.setFileFilter(new FileFilter() {

			@Override
			public String getDescription() {
				return "Image File Directory";
			}

			@Override
			public boolean accept(File f) {
				return f.isDirectory();
			}
		});

		browseButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent arg0) {
				if (fileChooser.showOpenDialog(demoDialog) == JFileChooser.APPROVE_OPTION) {
					String imageDirectoryPath = fileChooser.getSelectedFile().getAbsolutePath();
					imageDirectoryTextField.setText(imageDirectoryPath);
				}
			}

		});

		templateFileNameTextField.setEditable(true);
		browseButton.setEnabled(true);

		JPanel selectImageDirectoryPanel = new JPanel();
		selectImageDirectoryPanel.setLayout(new BoxLayout(selectImageDirectoryPanel, BoxLayout.PAGE_AXIS));
		selectImageDirectoryPanel.setBorder(new EmptyBorder(0, 10, 0, 10));
		selectImageDirectoryPanel.add(selectImageDirectoryArea);
		return selectImageDirectoryPanel;
	}

	private void addFieldsToVariablesPane(List<String> fieldNames) {
		DefaultTableModel model = (DefaultTableModel) variableDataTable.getModel();

		int numRows = model.getRowCount();
		for (int ix = 0; ix < numRows; ix++) {
			model.removeRow(0);
		}

		for (String fieldName : fieldNames) {
			model.addRow(new Object[] { fieldName, "" });
		}
	}

	private JTabbedPane createTabPanel() {
		final JTabbedPane tabbedPane = new JTabbedPane();
		tabbedPane.setPreferredSize(new Dimension(600, 300));

		JComponent variableDataTab = createVariableDataPanel();
		tabbedPane.addTab("Variable Data", variableDataTab);

		JComponent mergedPreviewTab = createPreviewPanel();
		tabbedPane.addTab("Merged Preview", mergedPreviewTab);

		tabbedPane.addChangeListener(new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				if (tabbedPane.getSelectedIndex() != VARIABLES_TAB_INDEX) {
					generatePreview();
				}
			}
		});
		return tabbedPane;
	}

	protected void generatePreview() {
		if (templateFileNameTextField.getText().isEmpty()) {
			JOptionPane.showMessageDialog(null, "Please specify a template name");
		} else {
			try {
				TemplateModel templateModel = new TemplateModel(templateFileNameTextField.getText(), imageDirectoryTextField.getText(), statusTextArea);

				Map<String, String> variableData = getVariablesTabData();
				List<GraphicsInfo> graphicsData = templateModel.generatePreview(variableData);

				JPanel imageHolder = new JPanel();
				imageHolder.setLayout(new BoxLayout(imageHolder, BoxLayout.PAGE_AXIS));

				for (GraphicsInfo info : graphicsData) {
					JLabel imageLabel = new JLabel(info.side + " " + info.printType);
					imageHolder.add(imageLabel);

					byte[] loopImageData = info.graphicData.getImageData();
					JLabel loopImage = labelFromImageData(loopImageData);
					imageHolder.add(loopImage);
				}

				previewScrollers[0].getViewport().add(imageHolder);
				demoDialog.pack();
			} catch (Exception e) {
				JOptionPane.showMessageDialog(null, "Could not generate preview : " + e.getLocalizedMessage());
			}
		}
	}

	private JLabel labelFromImageData(byte[] imageData) throws IOException {
		InputStream in = new ByteArrayInputStream(imageData);
		try {
			BufferedImage bi = ImageIO.read(in);
			ImageIcon icon = new ImageIcon(bi);
			return new JLabel(icon, JLabel.CENTER);
		} finally {
			IOUtils.closeQuietly(in);
		}
	}

	private Map<String, String> getVariablesTabData() {
		Map<String, String> data = new HashMap<String, String>();
		if (variablesTableModel != null) {
			for (int ix = 0; ix < variablesTableModel.getRowCount(); ix++) {
				data.put((String) variablesTableModel.getValueAt(ix, 0), (String) variablesTableModel.getValueAt(ix, 1));
			}
		}
		return data;
	}

	private JComponent createVariableDataPanel() {
		JScrollPane settingListScroller = new JScrollPane();

		variableDataTable = new JTable();
		settingListScroller.getViewport().add(variableDataTable);
		initializeTableModel(getHeaderLabels(), variableDataTable);
		return settingListScroller;
	}

	private String[] getHeaderLabels() {
		return new String[] { "Variable", "Value" };
	}

	private void initializeTableModel(String[] headerLabels, JTable table) {
		String[][] settingsData = new String[0][headerLabels.length];
		variablesTableModel = new DefaultTableModel(settingsData, headerLabels) {
			private static final long serialVersionUID = 5140988482895869527L;

			@Override
			public boolean isCellEditable(int row, int column) {
				return column == 1;
			}
		};

		table.removeAll();
		table.setModel(variablesTableModel);
		table.invalidate();
	}

	private JComponent createPreviewPanel() {
		String zebra_logo_path = "resources/zebra-logo-50px.png";
		ImageIcon zebraHead = loadIcon(zebra_logo_path);

		JScrollPane settingListScroller = new JScrollPane();
		previewScrollers[0] = settingListScroller;

		JLabel image = new JLabel(zebraHead, JLabel.CENTER);
		settingListScroller.getViewport().add(image);
		return settingListScroller;
	}
}
