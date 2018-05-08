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

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import javax.swing.JOptionPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;

import org.apache.commons.io.FileUtils;

import com.zebra.card.devdemo.JobInfo;
import com.zebra.card.devdemo.PollJobStatusWorker;
import com.zebra.card.devdemo.print.PrintJobOptions;
import com.zebra.card.devdemo.print.PrintModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.GraphicsInfo;
import com.zebra.sdk.common.card.containers.JobStatusInfo;
import com.zebra.sdk.common.card.containers.PrinterStatusInfo;
import com.zebra.sdk.common.card.enumerations.CardDestination;
import com.zebra.sdk.common.card.enumerations.CardSide;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.GraphicType;
import com.zebra.sdk.common.card.enumerations.MagEncodingType;
import com.zebra.sdk.common.card.enumerations.OrientationType;
import com.zebra.sdk.common.card.enumerations.PrintOptimizationMode;
import com.zebra.sdk.common.card.enumerations.PrintType;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.graphics.ZebraCardGraphics;
import com.zebra.sdk.common.card.graphics.enumerations.RotationType;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.settings.SettingsException;

public class MultiJobModel extends PrintModel {

	private final List<JobInfo> jobInfoList = new ArrayList<JobInfo>();

	public boolean isPrinterReady(JTextArea statusTextArea) throws ConnectionException, SettingsException, ZebraCardException {
		boolean printerReady = true;

		statusTextArea.append("Checking printer status...\n");

		PrinterStatusInfo statusInfo = getZebraCardPrinter().getPrinterStatus();

		int errorCode = statusInfo.errorInfo.value;
		int alarmCode = statusInfo.alarmInfo.value;

		if (alarmCode > 0 || errorCode > 0) {
			printerReady = false;

			String errorDesc = "";
			if (alarmCode > 0) {
				errorDesc = alarmCode + " " + statusInfo.alarmInfo.description;
			} else if (errorCode > 0) {
				errorDesc = errorCode + " " + statusInfo.errorInfo.description;
			}

			statusTextArea.append(String.format("%s (%s), please correct and try again.", statusInfo.status, errorDesc));
		}

		return printerReady;
	}

	public void setUpAndSendJobs(final JTextArea statusTextArea, Map<Integer, Job> jobMap, JobSettingOptions printOptions) throws ConnectionException, SettingsException, ZebraCardException {
		statusTextArea.setText("");

		try {
			getConnection().open();

			if (isPrinterReady(statusTextArea)) {
				statusTextArea.setText("Setting up jobs...\n");

				for (int jobNumber : jobMap.keySet()) {
					Job job = jobMap.get(jobNumber);
					if (job.frontCheckBox.isSelected() || job.backCheckBox.isSelected() || job.magEncodeCheckBox.isSelected()) {
						setUpAndSendJob(job, printOptions);
					} else {
						job.jobStatus.setText("Not configured");
					}
				}

				new PollJobStatusWorker(getZebraCardPrinter(), jobInfoList) {
					@Override
					protected void process(final List<StatusUpdateInfo> updateList) {
						SwingUtilities.invokeLater(new Runnable() {

							@Override
							public void run() {
								StatusUpdateInfo update = updateList.get(updateList.size() - 1);
								JobStatusInfo jobStatusInfo = update.getJobStatusInfo();

								statusTextArea.append(update.getMessage());

								if (jobStatusInfo != null) {
									String message = jobStatusInfo.printStatus;
									if (jobStatusInfo.printStatus.equals("done_ok")) {
										message = "Completed";
									} else if (jobStatusInfo.printStatus.equals("done_error")) {
										message = "Completed with error";
									} else if (jobStatusInfo.printStatus.contains("cancelled")) {
										if (jobStatusInfo.errorInfo.value > 0) {
											message = "Cancelled with error";
										} else {
											message = "Cancelled";
										}
									}
									((MultiJobInfo) update.getJobInfo()).getJob().jobStatus.setText("Job ID " + update.getJobInfo().getJobId() + ": " + message);
								}
							}
						});
					};
				}.execute();
			}
		} finally {
			cleanUpQuietly();
		}
	}

	private void setUpAndSendJob(Job job, JobSettingOptions printOptions) {
		try {
			setJobSetting(job, printOptions);
			sendPrintJob(job);
		} catch (Exception ex) {
			JOptionPane.showMessageDialog(null, ex.getLocalizedMessage());
		}
	}

	private void setJobSetting(Job job, JobSettingOptions printOptions) throws SettingsException, ConnectionException {
		CardSource cardSource = CardSource.fromString(job.sourceComboBox.getSelectedItem().toString());
		getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, cardSource.name());

		if (printOptions.showMagEncoding) {
			getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.MAG_ENCODING_TYPE, MagEncodingType.ISO.name());
			if (job.magEncodeCheckBox.isSelected()) {
				getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.MAG_COERCIVITY, job.coercivityTypeComboBox.getSelectedItem().toString());
			}
		}

		CardDestination cardDestination = CardDestination.valueOf(job.destinationComboBox.getSelectedItem().toString());
		getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, cardDestination.name());

		if (printOptions.showPrintOptimization) {
			PrintOptimizationMode printOptimizationMode = PrintOptimizationMode.valueOf(job.printOptimizationComboBox.getSelectedItem().toString());
			getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.PRINT_OPTIMIZATION, printOptimizationMode.name());
		}
	}

	private void sendPrintJob(Job job) throws IOException, SettingsException, ZebraCardException, ConnectionException {
		boolean frontCheckBox = job.frontCheckBox.isSelected();
		boolean backCheckBox = job.backCheckBox.isSelected();
		boolean magEncodeCheckBox = job.magEncodeCheckBox.isSelected();
		int jobId;

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

				jobId = printAndMagEncode(printJobOptions);
			} else {
				jobId = magEncode(printJobOptions);
			}

			jobInfoList.add(new MultiJobInfo(CardSource.fromString(job.sourceComboBox.getSelectedItem().toString()), jobId, job));
		}
	}

	public int magEncode(PrintJobOptions printJobOptions) throws ConnectionException, SettingsException, ZebraCardException {
		try {
			return getZebraCardPrinter().magEncode(printJobOptions.copies, printJobOptions.track1Data, printJobOptions.track2Data, printJobOptions.track3Data);
		} finally {
			cleanUpQuietly();
		}
	}

	public int printAndMagEncode(PrintJobOptions printJobOptions) throws IOException, ConnectionException, SettingsException, ZebraCardException {
		ZebraCardGraphics graphics = null;
		try {
			List<GraphicsInfo> graphicsInfo = new ArrayList<GraphicsInfo>();

			// Initialize graphics for printer object
			graphics = new ZebraCardGraphics(getZebraCardPrinter());

			if (printJobOptions.frontSelected) {
				graphicsInfo.addAll(createGraphicsInfo(graphics, printJobOptions.frontImageInfo, CardSide.Front));
			}

			if (printJobOptions.backSelected) {
				graphicsInfo.addAll(createGraphicsInfo(graphics, printJobOptions.backImageInfo, CardSide.Back));
			}

			if (jobContainsTrackData(printJobOptions)) {
				return getZebraCardPrinter().printAndMagEncode(printJobOptions.copies, graphicsInfo, printJobOptions.track1Data, printJobOptions.track2Data, printJobOptions.track3Data);
			} else {
				return getZebraCardPrinter().print(printJobOptions.copies, graphicsInfo);
			}
		} finally {
			if (graphics != null) {
				graphics.close();
			}
			cleanUpQuietly();
		}
	}

	private boolean jobContainsTrackData(PrintJobOptions printJobOptions) {
		return !printJobOptions.track1Data.isEmpty() || !printJobOptions.track2Data.isEmpty() || !printJobOptions.track3Data.isEmpty();
	}

	private List<GraphicsInfo> createGraphicsInfo(ZebraCardGraphics graphics, Map<PrintType, String> imageInfo, CardSide side) throws IOException {
		List<GraphicsInfo> graphicsInfo = new ArrayList<GraphicsInfo>();
		for (PrintType type : imageInfo.keySet()) {
			graphics.initialize(0, 0, OrientationType.Landscape, type, -1);

			if (type.equals(PrintType.Overlay) && imageInfo.get(type) == null) {
				GraphicsInfo grInfo = new GraphicsInfo();
				grInfo.side = side;
				grInfo.printType = type;
				grInfo.graphicType = GraphicType.NA;
				graphicsInfo.add(grInfo);
			} else {
				byte[] imageData = FileUtils.readFileToByteArray(new File(imageInfo.get(type)));
				graphics.drawImage(imageData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
				graphicsInfo.add(buildGraphicsInfo(graphics.createImage(), side, type));
			}

			graphics.clear();
		}

		return graphicsInfo;
	}

	public List<JobInfo> getJobInfoList() {
		return jobInfoList;
	}
}
