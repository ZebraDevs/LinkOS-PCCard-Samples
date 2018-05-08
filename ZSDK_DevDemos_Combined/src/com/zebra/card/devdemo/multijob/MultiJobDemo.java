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
import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;
import javax.swing.JTextField;
import javax.swing.border.TitledBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;

import com.zebra.card.devdemo.PrinterDemoBase;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.enumerations.TransferType;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.common.card.settings.ZebraCardSettingNames;

public class MultiJobDemo extends PrinterDemoBase<MultiJobModel> {

	private static final String ZXP_SERIES_9 = "ZXP Series 9";
	private static final String COLOR_OPTION = "ymc";
	private static final List<String> monoRibbonOptions = Arrays.asList("k", "mono", "black", "white", "red", "blue", "silver", "gold");
	private static final List<String> overlayRibbonOptions = Arrays.asList("ymcko", "kro", "kdo");

	private JobSettingOptions printOptions;
	private final Map<Integer, Job> jobMap = new LinkedHashMap<Integer, Job>();

	private JTextField jobNumberCanceled;

	private JTabbedPane mainTab;

	private JButton sendJobsButton;
	private JButton cancelJobButton;
	private JButton cancelAllJobsButton;

	public MultiJobDemo() {
		super(new MultiJobModel());
	}

	@Override
	public void addDemoDialogContent(Container container) {
		container.add(createPanelHeader("Multi-Job Printing Demo"), BorderLayout.PAGE_START);
		container.add(setUpJobActionsPanel(), BorderLayout.SOUTH);
		container.add(createMainTabPanel());
	}

	private JComponent createMainTabPanel() {
		JPanel mainPrintPanel = new JPanel();
		mainPrintPanel.setPreferredSize(new Dimension(1200, 675));

		mainTab = new JTabbedPane();
		mainTab.addTab("Print Jobs", null, mainPrintPanel, null);

		JTabbedPane mainJobsTab = new JTabbedPane(JTabbedPane.TOP);
		mainTab.addTab("Setup Jobs", null, mainJobsTab, null);
		mainTab.setEnabled(false);

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

		mainPrintPanel.add(createSelectPrinterPanel(true), BorderLayout.NORTH);
		mainPrintPanel.add(setUpJobStatusPanel(), BorderLayout.AFTER_LAST_LINE);
		mainPrintPanel.add(createJobStatusArea(30, 96), BorderLayout.AFTER_LAST_LINE);

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

		connectionTypeDropdown.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				clearDiscoveredPrinters();
				mainTab.setEnabled(false);
			}
		});

		return mainTab;
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

					demoDialog.setCursor(new Cursor(Cursor.WAIT_CURSOR));

					try {
						getPrinterModel().setUpAndSendJobs(statusTextArea, jobMap, printOptions);
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
						getPrinterModel().getConnection().open();
						getPrinterModel().getZebraCardPrinter().cancel(Integer.parseInt(jobId));
					} catch (NumberFormatException ex) {
						statusTextArea.setText(statusTextArea.getText() + "Invalid Entry. JobId must be a whole number.\n");
					} catch (ConnectionException ex) {
						JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
					} catch (ZebraCardException ex) {
						statusTextArea.setText(statusTextArea.getText() + "Job ID " + jobId + " not found.\n");
					} finally {
						if (getPrinterModel().getJobInfoList().isEmpty()) {
							getPrinterModel().cleanUpQuietly();
						}
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
					getPrinterModel().getConnection().open();
					getPrinterModel().getZebraCardPrinter().cancel(0);
				} catch (Exception ex) {
					JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
				} finally {
					if (getPrinterModel().getJobInfoList().isEmpty()) {
						getPrinterModel().cleanUpQuietly();
					}
				}
			}
		});
	}

	private void disableJobActionsAfterJobsComplete() {
		new Thread(new Runnable() {

			@Override
			public void run() {
				while (!getPrinterModel().getJobInfoList().isEmpty()) {
					sleep(150);
				}

				enableJobActionButtons(true);
				demoDialog.setCursor(new Cursor(Cursor.DEFAULT_CURSOR));
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
	protected void onDiscoveryStart() {
		mainTab.setEnabled(false);
	};

	@Override
	protected void onConnectionEstablished() {
		mainTab.setEnabled(false);

		for (Job job : jobMap.values()) {
			job.cleanUp();
		}
		statusTextArea.setText("");

		try {
			getPrinterModel().getConnection().open();

			printOptions = new JobSettingOptions();

			printOptions.showMagEncoding = getPrinterModel().getZebraCardPrinter().hasMagneticEncoder();
			if (getPrinterModel().getZebraCardPrinter().getJobSettings().contains(ZebraCardJobSettingNames.PRINT_OPTIMIZATION)) {
				printOptions.showPrintOptimization = true;
			}

			if (getPrinterModel().getZebraCardPrinter().getPrintCapability() == TransferType.DualSided) {
				printOptions.showBackSidePrint = true;
			}

			String installedRibbon = getPrinterModel().getZebraCardPrinter().getSettingValue(ZebraCardSettingNames.RIBBON_DESCRIPTION);
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

				printOptions.cardSourceRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_SOURCE);
				printOptions.cardDestinationRange = getPrinterModel().getZebraCardPrinter().getJobSettingRange(ZebraCardJobSettingNames.CARD_DESTINATION);
				printOptions.showLamDestinations = getPrinterModel().getZebraCardPrinter().hasLaminator();

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
			getPrinterModel().cleanUpQuietly();
		}
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
