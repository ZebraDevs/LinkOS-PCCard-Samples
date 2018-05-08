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

package com.zebra.card.devdemo.mag;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.PrintStream;
import java.util.EnumSet;
import java.util.concurrent.TimeoutException;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.border.TitledBorder;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.card.devdemo.TextAreaOutputStream;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.MagTrackData;
import com.zebra.sdk.common.card.enumerations.CardDestination;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.CoercivityType;
import com.zebra.sdk.common.card.enumerations.DataSource;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.settings.SettingsException;

public class MagEncodeDemo extends PrinterDemoBase<MagEncodeModel> {

	private JTextField track1DataTextField;
	private JTextField track2DataTextField;
	private JTextField track3DataTextField;

	private JComboBox cardSourceComboBox;
	private JComboBox cardDestinationComboBox;
	private JComboBox coercivityTypeComboBox;
	private JComboBox magEncodeJob;
	private JComboBox quantityComboBox;

	private JCheckBox verifyCheckBox;
	private JCheckBox track1CheckBox;
	private JCheckBox track2CheckBox;
	private JCheckBox track3CheckBox;

	public MagEncodeDemo() {
		super(new MagEncodeModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Magnetic Encode"), BorderLayout.PAGE_START);
		container.add(createLowerPanel());
	}

	private JPanel createLowerPanel() {
		JPanel lowerPanel = new JPanel();
		lowerPanel.setLayout(new BoxLayout(lowerPanel, BoxLayout.PAGE_AXIS));
		lowerPanel.add(createSelectPrinterPanel(true));
		lowerPanel.add(createJobSettingPanel());
		lowerPanel.add(createMagEncodingPanel());
		lowerPanel.add(createJobStatusPanel(10, 86));
		lowerPanel.add(createMagEncodeButton(), BorderLayout.LINE_END);
		return lowerPanel;
	}

	private JPanel createMagEncodeButton() {
		JPanel buttonPanel = new JPanel();
		buttonPanel.setLayout(new FlowLayout(FlowLayout.RIGHT));

		actionButton = new JButton("Write");
		setActionButtonEnabled(false);

		actionButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						enableMagEncoding(false);
						setActionButtonEnabled(false);

						demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

						try {
							getPrinterModel().getConnection().open();

							if (magEncodeJob.getSelectedItem().equals("Write")) {
								MagEncodeContainer magEncodeContainer = buildMagEncodeContainer();
								getPrinterModel().magEncode(magEncodeContainer, statusTextArea);
							} else {
								clearData();

								EnumSet<DataSource> tracksToRead = getTracksToRead();
								if (!tracksToRead.isEmpty()) {
									readMagData(tracksToRead);
								} else {
									JOptionPane.showMessageDialog(null, "No tracks were selected to be read.\nPlease select a track to read and try again.");
								}
							}
						} catch (Exception e) {
							JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
						} finally {
							getPrinterModel().cleanUpQuietly();
						}

						demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
						setActionButtonEnabled(true);
						enableMagEncoding(true);
					}
				}).start();
			}
		});

		JPanel buttonHolder = new JPanel(new BorderLayout());
		buttonHolder.add(actionButton, BorderLayout.LINE_END);
		buttonPanel.add(buttonHolder, BorderLayout.LINE_END);
		return buttonPanel;
	}

	private void readMagData(EnumSet<DataSource> tracksToRead) throws ConnectionException, SettingsException, ZebraCardException, TimeoutException {
		try {
			statusTextArea.setText("");

			PrintStream printStream = new PrintStream(new TextAreaOutputStream(statusTextArea));
			System.setOut(printStream);

			String source = (String) cardSourceComboBox.getSelectedItem();
			String destination = (String) cardDestinationComboBox.getSelectedItem();

			MagTrackData data = getPrinterModel().magRead(tracksToRead, source, destination);

			boolean dataRead = false;

			if (tracksToRead.contains(DataSource.Track1) && !data.track1Data.isEmpty()) {
				track1DataTextField.setText(data.track1Data);
				dataRead = true;
			}

			if (tracksToRead.contains(DataSource.Track2) && !data.track2Data.isEmpty()) {
				track2DataTextField.setText(data.track2Data);
				dataRead = true;
			}

			if (tracksToRead.contains(DataSource.Track3) && !data.track3Data.isEmpty()) {
				track3DataTextField.setText(data.track3Data);
				dataRead = true;
			}

			if (!dataRead) {
				System.out.println("No data read from card.");
			}
		} finally {
			getPrinterModel().cleanUpQuietly();
			System.setOut(System.out);
		}
	}

	private JPanel createJobSettingPanel() {
		JPanel jobSettingPanel = new JPanel();
		jobSettingPanel.setLayout(new BoxLayout(jobSettingPanel, BoxLayout.PAGE_AXIS));
		jobSettingPanel.setBorder(new TitledBorder("Job Settings"));

		JPanel jobSettingsArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel magEncodeJobLabel = new JLabel("Mag Encode Job");
		jobSettingsArea.add(magEncodeJobLabel);

		magEncodeJob = new JComboBox();
		magEncodeJob.addItem("Write");
		magEncodeJob.addItem("Read");
		magEncodeJob.setEnabled(false);
		jobSettingsArea.add(magEncodeJob);

		magEncodeJob.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				setUpJobSettings();
			}
		});

		JLabel sourceLabel = new JLabel("Source");
		jobSettingsArea.add(sourceLabel);

		cardSourceComboBox = new JComboBox();
		cardSourceComboBox.setEnabled(false);
		jobSettingsArea.add(cardSourceComboBox);

		JLabel destinationLabel = new JLabel("Destination");
		jobSettingsArea.add(destinationLabel);

		cardDestinationComboBox = new JComboBox();
		cardDestinationComboBox.setEnabled(false);
		jobSettingsArea.add(cardDestinationComboBox);

		JLabel coercivityLabel = new JLabel("Coercivity Type");
		jobSettingsArea.add(coercivityLabel);

		coercivityTypeComboBox = new JComboBox();
		coercivityTypeComboBox.setEnabled(false);
		jobSettingsArea.add(coercivityTypeComboBox);

		verifyCheckBox = new JCheckBox("Verify Encoding");
		verifyCheckBox.setEnabled(true);
		jobSettingsArea.add(verifyCheckBox);

		JLabel quantityLabel = new JLabel("Quantity");
		jobSettingsArea.add(quantityLabel);

		quantityComboBox = new JComboBox();
		quantityComboBox.removeAllItems();
		quantityComboBox.addItem(1);
		quantityComboBox.addItem(2);
		quantityComboBox.addItem(3);
		quantityComboBox.addItem(4);
		quantityComboBox.addItem(5);
		quantityComboBox.setEnabled(false);
		jobSettingsArea.add(quantityComboBox);

		jobSettingPanel.add(jobSettingsArea);
		return jobSettingPanel;
	}

	private JPanel createMagEncodingPanel() {
		track1DataTextField = new JTextField();
		track2DataTextField = new JTextField();
		track3DataTextField = new JTextField();

		JPanel magEncodeArea = new JPanel();
		magEncodeArea.setLayout(new BoxLayout(magEncodeArea, BoxLayout.PAGE_AXIS));
		magEncodeArea.setBorder(new TitledBorder("Mag - Encode"));

		JPanel selectPrintFrontArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		magEncodeArea.add(selectPrintFrontArea);

		JLabel selectPrintFrontLabel = new JLabel("Magnetic Encode");
		selectPrintFrontArea.add(selectPrintFrontLabel);

		JPanel track1FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel track1DataLabel = new JLabel("Track 1 data");
		track1FileArea.add(track1DataLabel);
		track1DataTextField.setPreferredSize(new Dimension(600, 25));
		track1FileArea.add(track1DataTextField);

		track1CheckBox = new JCheckBox("Read Track 1 Data");
		track1CheckBox.setEnabled(false);
		track1FileArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 75, 10)));
		track1FileArea.add(track1CheckBox);
		magEncodeArea.add(track1FileArea);

		JPanel track2FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel track2DataLabel = new JLabel("Track 2 data");
		track2FileArea.add(track2DataLabel);

		track2DataTextField.setPreferredSize(new Dimension(600, 25));
		track2FileArea.add(track2DataTextField);

		track2CheckBox = new JCheckBox("Read Track 2 Data");
		track2CheckBox.setEnabled(false);
		track2FileArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 75, 10)));
		track2FileArea.add(track2CheckBox);
		magEncodeArea.add(track2FileArea);

		JPanel track3FileArea = new JPanel(new FlowLayout(FlowLayout.LEFT, 20, 10));
		JLabel track3DataLabel = new JLabel("Track 3 data");
		track3FileArea.add(track3DataLabel);

		track3DataTextField.setPreferredSize(new Dimension(600, 25));
		track3FileArea.add(track3DataTextField);

		track3CheckBox = new JCheckBox("Read Track 3 Data");
		track3CheckBox.setEnabled(false);
		track3FileArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 75, 10)));
		track3FileArea.add(track3CheckBox);
		magEncodeArea.add(track3FileArea);

		return magEncodeArea;
	}

	@Override
	protected void onConnectionEstablished() {
		clearData();
		setUpMagDemo();
	}

	private void setUpMagDemo() {
		String cardSourceRange = null;
		String cardDestinationRange = null;
		String coercivityTypeRange = null;
		boolean hasMagEncoder = false;
		boolean hasLaminator = false;

		try {
			getPrinterModel().getConnection().open();

			hasMagEncoder = getPrinterModel().getZebraCardPrinter().hasMagneticEncoder();
			hasLaminator = getPrinterModel().getZebraCardPrinter().hasLaminator();
			if (hasMagEncoder) {
				cardSourceRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
				cardDestinationRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
				coercivityTypeRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.MAG_COERCIVITY);
			}
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		} finally {
			getPrinterModel().cleanUpQuietly();
		}

		if (hasMagEncoder) {
			cardSourceComboBox.removeAllItems();
			for (CardSource source : CardSource.values()) {
				if (cardSourceRange != null && cardSourceRange.contains(source.name())) {
					cardSourceComboBox.addItem(source.name());
				}
			}
			cardSourceComboBox.setSelectedItem(CardSource.Feeder.name());

			cardDestinationComboBox.removeAllItems();
			for (CardDestination destination : CardDestination.values()) {
				if (cardDestinationRange != null && cardDestinationRange.contains(destination.name())) {
					if (!destination.name().contains("Laminator") || hasLaminator) {
						cardDestinationComboBox.addItem(destination.name());
					}
				}
			}
			cardDestinationComboBox.setSelectedItem(CardDestination.Eject.name());

			coercivityTypeComboBox.removeAllItems();
			for (CoercivityType coercivity : CoercivityType.values()) {
				if (coercivityTypeRange != null && coercivityTypeRange.contains(coercivity.name())) {
					coercivityTypeComboBox.addItem(coercivity.name());
				}
			}
			coercivityTypeComboBox.setSelectedItem(CoercivityType.High.name());

			enableMagEncoding(true);
			setUpJobSettings();
		} else {
			enableMagEncoding(false);
			JOptionPane.showMessageDialog(null, "The selected printer does not have a Magnetic Encoder.\nPlease choose a different printer from the list.");
		}
	}

	private void setUpJobSettings() {
		String magJob = magEncodeJob.getSelectedItem().toString();
		actionButton.setText(magJob);

		boolean isRead = magJob.equalsIgnoreCase("read");
		track1CheckBox.setSelected(isRead);
		track2CheckBox.setSelected(isRead);
		track3CheckBox.setSelected(isRead);
		track1CheckBox.setEnabled(isRead);
		track2CheckBox.setEnabled(isRead);
		track3CheckBox.setEnabled(isRead);

		verifyCheckBox.setEnabled(!isRead);
		verifyCheckBox.setSelected(!isRead);
		quantityComboBox.setEnabled(!isRead);

		if (isRead) {
			track1DataTextField.setText("");
			track2DataTextField.setText("");
			track3DataTextField.setText("");
		}
	}

	private void enableMagEncoding(boolean enabled) {
		track1DataTextField.setEnabled(enabled);
		track2DataTextField.setEnabled(enabled);
		track3DataTextField.setEnabled(enabled);

		cardSourceComboBox.setEnabled(enabled);
		cardDestinationComboBox.setEnabled(enabled);
		coercivityTypeComboBox.setEnabled(enabled);

		magEncodeJob.setEnabled(enabled);
		actionButton.setEnabled(enabled);

		if (!enabled || magEncodeJob.getSelectedItem().toString().equalsIgnoreCase("write")) {
			quantityComboBox.setEnabled(enabled);
			verifyCheckBox.setEnabled(enabled);
		}
	}

	private EnumSet<DataSource> getTracksToRead() {
		EnumSet<DataSource> tracksToRead = EnumSet.noneOf(DataSource.class);

		if (track1CheckBox.isSelected()) {
			tracksToRead.add(DataSource.Track1);
		}

		if (track2CheckBox.isSelected()) {
			tracksToRead.add(DataSource.Track2);
		}

		if (track3CheckBox.isSelected()) {
			tracksToRead.add(DataSource.Track3);
		}

		return tracksToRead;
	}

	private void clearData() {
		track1DataTextField.setText("");
		track2DataTextField.setText("");
		track3DataTextField.setText("");
	}

	private MagEncodeContainer buildMagEncodeContainer() {
		MagEncodeContainer container = new MagEncodeContainer();
		container.cardSource = (String) cardSourceComboBox.getSelectedItem();
		container.cardDestination = (String) cardDestinationComboBox.getSelectedItem();
		container.coercivityType = (String) coercivityTypeComboBox.getSelectedItem();
		container.verify = verifyCheckBox.isSelected();
		container.track1Data = track1DataTextField.getText();
		container.track2Data = track2DataTextField.getText();
		container.track3Data = track3DataTextField.getText();
		container.quantity = (Integer) quantityComboBox.getSelectedItem();
		return container;
	}
}
