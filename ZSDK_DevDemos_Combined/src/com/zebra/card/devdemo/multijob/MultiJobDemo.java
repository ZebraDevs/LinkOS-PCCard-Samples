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

import java.awt.*;
import java.awt.event.*;
import java.io.IOException;
import java.util.*;
import java.util.List;

import javax.swing.*;
import javax.swing.border.TitledBorder;
import javax.swing.event.*;

import com.zebra.card.devdemo.*;
import com.zebra.card.devdemo.print.PrintJobOptions;
import com.zebra.sdk.comm.*;
import com.zebra.sdk.common.card.containers.*;
import com.zebra.sdk.common.card.enumerations.*;
import com.zebra.sdk.common.card.errors.ZebraCardErrors;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.common.card.printer.*;
import com.zebra.sdk.common.card.settings.ZebraCardSettingNames;
import com.zebra.sdk.settings.SettingsException;

public class MultiJobDemo extends PrinterDemoBase implements PrinterDemo {

	private static final String ZXP_SERIES_9 = "ZXP Series 9";
	private static final String COLOR_OPTION = "ymc";
	private static final List<String> monoRibbonOptions = Arrays.asList("k", "mono", "black", "white", "red", "blue", "silver", "gold");
	private static final List<String> overlayRibbonOptions = Arrays.asList("ymcko", "kro", "kdo");

	private static final Integer CARD_FEED_TIMEOUT = 30000;

	private ZebraCardPrinter zebraCardPrinter;
	private Connection connection;
	private JobSettingOptions printOptions;
	private List<Integer> jobIdsList;
	private Map<Integer, Job> jobMap;

	private JTextField jobNumberCanceled;

	private JTabbedPane mainTab;
	private JTabbedPane mainJobsTab;

	private JButton sendJobsButton;
	private JButton cancelJobButton;
	private JButton cancelAllJobsButton;

	@Override
	public void createDemoDialog(JFrame owner) {
		jobIdsList = new ArrayList<Integer>();
		demoDialog = new JDialog(owner, "Zebra Multi Platform SDK - Developer Demo", true);
		Container mainPane = demoDialog.getContentPane();
		mainTab = new JTabbedPane();

		mainPane.add(createPanelHeader("Multi-Job Printing Demo"), BorderLayout.PAGE_START);

		JPanel mainPrintPanel = new JPanel();
		mainTab.addTab("Print Jobs", null, mainPrintPanel, null);

		mainJobsTab = new JTabbedPane(JTabbedPane.TOP);
		mainTab.addTab("Setup Jobs", null, mainJobsTab, null);
		mainTab.setEnabled(false);

		jobMap = new LinkedHashMap<Integer, Job>();

		Job job = new Job();
		mainJobsTab.addTab("Job 1", null, job.createJob(demoDialog), null);
		jobMap.put(1, job);

		job = new Job();
		mainJobsTab.addTab("Job 2", null, job.createJob(demoDialog), null);
		jobMap.put(2, job);

		job = new Job();
		mainJobsTab.addTab("Job 3", null, job.createJob(demoDialog), null);
		jobMap.put(3, job);

		job = new Job();
		mainJobsTab.addTab("Job 4", null, job.createJob(demoDialog), null);
		jobMap.put(4, job);

		mainPrintPanel.setPreferredSize(new Dimension(1200, 675));
		mainPrintPanel.add(createSelectPrinterPanel(), BorderLayout.NORTH);
		mainPrintPanel.add(setUpJobStatusPanel(), BorderLayout.AFTER_LAST_LINE);
		mainPrintPanel.add(createJobStatusArea(30, 96), BorderLayout.AFTER_LAST_LINE);

		JPanel BottomPanel = setUpJobActionsPanel();
		mainPane.add(BottomPanel, BorderLayout.SOUTH);
		mainPane.add(mainTab);

		setUpMainPaneListeners();

		demoDialog.setResizable(false);
		demoDialog.pack();
		demoDialog.setLocation((Toolkit.getDefaultToolkit().getScreenSize().width / 2) - (mainPane.getWidth() / 2), 0);
		demoDialog.setVisible(true);
		demoDialog.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
	}

	private void setUpMainPaneListeners() {
		mainTab.addChangeListener(new ChangeListener() {

			@Override
			public void stateChanged(ChangeEvent e) {
				if (addressDropdown.getSelectedItem() != null) {
					String address = addressDropdown.getSelectedItem().toString();
					if (!address.isEmpty()) {
						boolean shouldView = address.contains(ZXP_SERIES_9);
						for (int jobNumber : jobMap.keySet()) {
							jobMap.get(jobNumber).printOptimizationComboBox.setVisible(shouldView);
							jobMap.get(jobNumber).printOptimizationLabel.setVisible(shouldView);
						}
					}
				}
			}
		});

		addressDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				statusTextArea.setText("");
				additionalPostDiscoveryAction();
			}
		});

		connectionTypeDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				PrinterDemoBase.clearDiscoveredPrinters();
				mainTab.setEnabled(false);
			}
		});
	}

	private JPanel setUpJobStatusPanel() {
		JPanel jobStatusPanel = new JPanel();
		jobStatusPanel.setLayout(new FlowLayout(FlowLayout.LEFT, 10, 10));
		jobStatusPanel.setBorder(new TitledBorder("Job Status"));

		for (int jobNumber : jobMap.keySet()) {
			jobMap.get(jobNumber).jobStatusLabel = new JLabel("Status " + jobNumber);
			jobMap.get(jobNumber).jobStatus = new JTextField();
			jobMap.get(jobNumber).jobStatus.setEditable(false);
			jobMap.get(jobNumber).jobStatus.setPreferredSize(new Dimension(222, 25));

			JPanel jobPanel = new JPanel(new FlowLayout(FlowLayout.LEFT, 1, 10));
			jobPanel.add(jobMap.get(jobNumber).jobStatusLabel);
			jobPanel.add(jobMap.get(jobNumber).jobStatus);
			jobStatusPanel.add(jobPanel);
		}

		return jobStatusPanel;
	}

	private JPanel setUpJobActionsPanel() {
		JPanel jobActionsPanel = new JPanel();
		jobActionsPanel.setLayout(new BoxLayout(jobActionsPanel, BoxLayout.PAGE_AXIS));
		jobActionsPanel.setBorder(new TitledBorder("Job Actions"));

		jobNumberCanceled = new JTextField();
		jobNumberCanceled.setEnabled(false);
		jobNumberCanceled.setPreferredSize(new Dimension(40, 25));

		JPanel jobCancelPanel = new JPanel();
		JLabel jobCancelLabel = new JLabel("Job ID");
		jobCancelPanel.add(jobCancelLabel);
		jobCancelPanel.add(jobNumberCanceled);

		cancelJobButton = new JButton("Cancel");
		cancelJobButton.setEnabled(false);
		jobCancelPanel.add(cancelJobButton);
		jobCancelPanel.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 45, 10)));

		cancelAllJobsButton = new JButton("Cancel All Jobs");
		cancelAllJobsButton.setEnabled(false);
		jobCancelPanel.add(cancelAllJobsButton);

		JPanel cancelPanel = new JPanel(new FlowLayout(FlowLayout.LEFT, 40, 10));
		cancelPanel.setLayout(new BoxLayout(cancelPanel, BoxLayout.PAGE_AXIS));
		cancelPanel.setBorder(new TitledBorder("Cancel Jobs"));
		cancelPanel.add(jobCancelPanel);

		JPanel jobActionsArea = new JPanel();
		jobActionsArea.add(cancelPanel, BorderLayout.WEST);
		jobActionsArea.add(new JPanel(new FlowLayout(FlowLayout.LEFT, 300, 10)), BorderLayout.CENTER);

		sendJobsButton = new JButton("Send Jobs");
		jobActionsArea.add(sendJobsButton, BorderLayout.EAST);
		jobActionsPanel.add(jobActionsArea);

		JPanel bottomPanel = new JPanel();
		bottomPanel.add(jobActionsPanel, BorderLayout.CENTER);

		setUpSendJobsButtonHandler();
		setUpCancelJobButtonHandler();
		setUpCancelAllJobsButtonHandler();

		return bottomPanel;
	}

	private void setUpJobPanels() {
		for (Job job : jobMap.values()) {
			job.setUpPrintPanels(printOptions);
		}
	}

	private void setUpSendJobsButtonHandler() {
		sendJobsButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				if (checkIfAnyJobsAreSetUp()) {
					mainTab.setSelectedIndex(0);
					enableJobActionButtons(false);

					jobIdsList = new ArrayList<Integer>();
					demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

					DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();

					try {
						connection = printer.getConnection();
						connection.open();

						zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);

						if (isPrinterReady(zebraCardPrinter)) {
							statusTextArea.setText("Setting up jobs to send to " + connection.toString() + "\n");

							for (int jobNumber : jobMap.keySet()) {
								Job job = jobMap.get(jobNumber);
								if (job.frontCheckBox.isSelected() || job.backCheckBox.isSelected() || job.magEncodeCheckBox.isSelected()) {
									setUpAndSendJob(jobMap.get(jobNumber));
								} else {
									job.jobStatus.setText("Not configured.");
								}
							}
						}
					} catch (Exception ex) {
						JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
					}

					disableJobActionsAfterJobsComplete();
				} else {
					JOptionPane.showMessageDialog(null, "No jobs were configured. Please configure a job and try again.");
				}
			}
		});
	}

	private void setUpCancelJobButtonHandler() {
		cancelJobButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				String jobId = jobNumberCanceled.getText();
				if (!jobId.isEmpty()) {
					try {
						zebraCardPrinter.cancel(Integer.parseInt(jobId));
					} catch (NumberFormatException ex) {
						statusTextArea.setText(statusTextArea.getText() + "Invalid Entry. JobId must be a whole number.\n");
					} catch (ConnectionException ex) {
						JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
					} catch (ZebraCardException ex) {
						statusTextArea.setText(statusTextArea.getText() + "Job ID " + jobId + " not found.\n");
					}
				}
			}
		});
	}

	private void setUpCancelAllJobsButtonHandler() {
		cancelAllJobsButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				try {
					zebraCardPrinter.cancel(0);
				} catch (Exception ex) {
					JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
				}
			}
		});
	}

	private void setUpAndSendJob(final Job job) {
		try {
			setJobSetting(job);
			if (sendPrintJob(job)) {
				new Thread(new Runnable() {

					@Override
					public void run() {
						try {
							statusTextArea.setText(statusTextArea.getText() + "Polling status for Job ID " + jobIdsList.get(jobIdsList.size() - 1) + "\n");
							pollJobStatus(jobIdsList.get(jobIdsList.size() - 1), job);
						} catch (Exception ex) {
							JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
						}
					}
				}).start();
			}
		} catch (Exception ex) {
			JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
		}
	}

	private void setJobSetting(Job job) throws SettingsException {
		CardSource cardSource = CardSource.valueOf(job.sourceComboBox.getSelectedItem().toString());
		zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, cardSource.name());
		if (printOptions.showMagEncoding) {
			zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.MAG_ENCODING_TYPE, MagEncodingType.ISO.name());
			if (job.magEncodeCheckBox.isSelected()) {
				zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.MAG_COERCIVITY, job.coercivityTypeComboBox.getSelectedItem().toString());
			}
		}

		CardDestination cardDestination = CardDestination.valueOf(job.destinationComboBox.getSelectedItem().toString());
		zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, cardDestination.name());
		if (printOptions.showPrintOptimization) {
			PrintOptimizationMode printOptimizationMode = PrintOptimizationMode.valueOf(job.printOptimizationComboBox.getSelectedItem().toString());
			zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.PRINT_OPTIMIZATION, printOptimizationMode.name());
		}
	}

	private void disableJobActionsAfterJobsComplete() {
		new Thread(new Runnable() {

			@Override
			public void run() {
				try {
					while (!jobIdsList.isEmpty()) {
						sleep(150);
					}

					enableJobActionButtons(true);
					demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
				} finally {
					PrinterModel.cleanUpQuietly(zebraCardPrinter, connection);
				}
			}
		}).start();
	}

	private void enableJobActionButtons(boolean enable) {
		sendJobsButton.setEnabled(enable);

		jobNumberCanceled.setEnabled(!enable);
		cancelJobButton.setEnabled(!enable);
		cancelAllJobsButton.setEnabled(!enable);
	}

	@Override
	protected void additionalPreDiscoveryAction() {
		mainTab.setEnabled(false);
	};

	@Override
	protected void additionalPostDiscoveryAction() {
		mainTab.setEnabled(false);

		for (Job job : jobMap.values()) {
			job.cleanUp();
		}

		DiscoveredPrinterForDevDemo printer = (DiscoveredPrinterForDevDemo) addressDropdown.getSelectedItem();
		if (printer != null) {
			try {
				connection = printer.getConnection();
				connection.open();

				zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);

				printOptions = new JobSettingOptions();

				printOptions.showMagEncoding = zebraCardPrinter.hasMagneticEncoder();
				if (zebraCardPrinter.getJobSettings().contains(ZebraCardJobSettingNames.PRINT_OPTIMIZATION)) {
					printOptions.showPrintOptimization = true;
				}

				if (zebraCardPrinter.getPrintCapability() == TransferType.DualSided) {
					printOptions.showBackSidePrint = true;
				}

				String installedRibbon = zebraCardPrinter.getSettingValue(ZebraCardSettingNames.RIBBON_DESCRIPTION);
				if (installedRibbon != null && !installedRibbon.isEmpty()) {
					installedRibbon = installedRibbon.toLowerCase(Locale.US);

					if (installedRibbon.contains(COLOR_OPTION)) {
						printOptions.allowsColorOption = true;
					}

					if (isPrintTypeSupported(installedRibbon, monoRibbonOptions)) {
						printOptions.allowsMonoOption = true;
					}

					if (isPrintTypeSupported(installedRibbon, overlayRibbonOptions)) {
						printOptions.allowsOverlayOption = true;
					}

					printOptions.cardSourceRange = zebraCardPrinter.getJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
					printOptions.cardDestinationRange = zebraCardPrinter.getJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
					printOptions.showLamDestinations = zebraCardPrinter.hasLaminator();

					setUpJobPanels();
					mainTab.setEnabled(true);
				} else {
					statusTextArea.setText(statusTextArea.getText() + addressDropdown.getSelectedItem().toString() + " - No ribbon installed.  Please install a ribbon and try again.\n");
				}
			} catch (ConnectionException ex) {
				statusTextArea.setText(statusTextArea.getText() + "Error talking to the printer.  Please power cycle your printer and try again.\n");
			} catch (Exception e) {
				JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
			} finally {
				PrinterModel.cleanUpQuietly(zebraCardPrinter, connection);
			}
		}
	}

	private boolean sendPrintJob(Job job) throws IOException, SettingsException, ZebraCardException, ConnectionException {
		boolean printJobSent = false;
		boolean frontCheckBox = job.frontCheckBox.isSelected();
		boolean backCheckBox = job.backCheckBox.isSelected();
		boolean magEncodeCheckBox = job.magEncodeCheckBox.isSelected();

		if (frontCheckBox || backCheckBox || magEncodeCheckBox) {
			PrintJobOptions printJobOptions = new PrintJobOptions();
			printJobOptions.track1Data = job.track1DataTextField.getText();
			printJobOptions.track2Data = job.track2DataTextField.getText();
			printJobOptions.track3Data = job.track3DataTextField.getText();

			printJobOptions.copies = (Integer) job.quantityComboBox.getSelectedItem();

			if (frontCheckBox || backCheckBox) {
				printJobOptions.frontImageInfo = job.frontImageInfo;
				printJobOptions.backImageInfo = job.backImageInfo;
				printJobOptions.frontSelected = job.frontCheckBox.isSelected();
				printJobOptions.backSelected = job.backCheckBox.isSelected();

				jobIdsList.add(new MultiJobModel().printAndMagEncode(zebraCardPrinter, printJobOptions));
				printJobSent = true;
			} else {
				jobIdsList.add(new MultiJobModel().magEncode(zebraCardPrinter, printJobOptions));
				printJobSent = true;
			}
		}

		return printJobSent;
	}

	protected void pollJobStatus(int jobId, Job job) {
		String status = "";
		JobStatusInfo jobStatus = null;

		long start = System.currentTimeMillis();
		boolean done = false;
		boolean showAtmDialog = true;
		try {

			while (!done) {
				jobStatus = zebraCardPrinter.getJobStatus(jobId);
				status = jobId + ": " + jobStatus.printStatus;

				if (!job.jobStatus.getText().equals(status)) {
					job.jobStatus.setText(status);
				}

				if (jobStatus.printStatus.contains("done_ok")) {
					done = true;
				} else if (jobStatus.printStatus.contains("error") || jobStatus.printStatus.contains("cancelled")) {
					if (jobStatus.errorInfo.value > 0) {
						statusTextArea.append("Job ID " + jobId + " encountered an error [" + jobStatus.errorInfo.description + "] and was cancelled.\n");
					}
					done = true;
				} else if (jobStatus.errorInfo.value > 0) {
					zebraCardPrinter.cancel(jobId);
				} else if ((jobStatus.printStatus.contains("in_progress") && jobStatus.cardPosition.contains("feeding")) // ZMotif printers
						|| (jobStatus.printStatus.contains("alarm_handling") && jobStatus.alarmInfo.value == ZebraCardErrors.MEDIA_OUT_OF_CARDS)) { // ZXP printers
					if (showAtmDialog && job.sourceComboBox.getSelectedItem().toString().equalsIgnoreCase("atm")) {
						PrinterModel.showInformationDialog("Multi-job Demo", "Please place a card into the ATM slot and click Okay.");
						showAtmDialog = false;
					} else if (System.currentTimeMillis() > start + CARD_FEED_TIMEOUT) {
						statusTextArea.append("Job ID " + jobId + " timed out waiting for a card and was cancelled.\n");
						zebraCardPrinter.cancel(jobId);
					}
				}

				if (!done) {
					// If not the active job reset the start time.
					if (jobIdsList.indexOf(jobId) != 0) {
						start = System.currentTimeMillis();
					}
					sleep(1000);
				}
			}
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, e.getLocalizedMessage());
		}

		jobIdsList.remove(jobIdsList.indexOf(jobId));

		if (job.jobStatus.getText().contains("cancelled")) {
			job.jobStatus.setText("Cancelled Job ID " + jobId);
		} else {
			job.jobStatus.setText("Completed Job ID " + jobId);
		}
	}

	private boolean isPrinterReady(ZebraCardPrinter zebraCardPrinter) throws ConnectionException, SettingsException, ZebraCardException {
		boolean printerReady = true;

		statusTextArea.append("Checking printer status...\n");
		PrinterStatusInfo statusInfo = zebraCardPrinter.getPrinterStatus();

		int errorCode = statusInfo.errorInfo.value;
		int alarmCode = statusInfo.alarmInfo.value;

		if (alarmCode > 0 || errorCode > 0) {
			printerReady = false;

			String errorDesc = "";
			if (alarmCode > 0) {
				errorDesc = statusInfo.alarmInfo.value + " " + statusInfo.alarmInfo.description;
			} else if (errorCode > 0) {
				errorDesc = statusInfo.errorInfo.value + " " + statusInfo.errorInfo.description;
			}

			statusTextArea.append(String.format("%s (%s), please correct and try again.", statusInfo.status, errorDesc));
		}

		return printerReady;
	}

	private void sleep(long milliseconds) {
		try {
			Thread.sleep(milliseconds);
		} catch (InterruptedException e) {
		}
	}

	private boolean checkIfAnyJobsAreSetUp() {
		for (Job job : jobMap.values()) {
			if (isFrontImageSetUp(job) || isBackImageSetUp(job) || isMagDataSetUp(job)) {
				return true;
			}
		}
		return false;
	}

	private boolean isBackImageSetUp(Job job) {
		return (job.backCheckBox.isSelected() && !job.backImageInfo.isEmpty());
	}

	private boolean isFrontImageSetUp(Job job) {
		return (job.frontCheckBox.isSelected() && !job.frontImageInfo.isEmpty());
	}

	private boolean isMagDataSetUp(Job job) {
		boolean hasMagData = !job.track1DataTextField.getText().isEmpty() || !job.track2DataTextField.getText().isEmpty() || !job.track3DataTextField.getText().isEmpty();
		return (job.magEncodeCheckBox.isSelected() && hasMagData);
	}

	private boolean isPrintTypeSupported(String installedRibbon, List<String> ribbonTypeOptions) {
		boolean isSupported = true;
		for (String option : ribbonTypeOptions) {
			if (installedRibbon.indexOf(option) == -1) {
				isSupported = false;
			} else {
				isSupported = true;
				break;
			}
		}
		return isSupported;
	}
}
