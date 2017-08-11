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

package com.zebra.card.devdemo.smartcard;

import javax.swing.JOptionPane;
import javax.swing.JTextArea;

import com.zebra.card.devdemo.DiscoveredPrinterForDevDemo;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.JobStatusInfo;
import com.zebra.sdk.common.card.errors.ZebraCardErrors;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.common.card.printer.ZebraCardPrinterFactory;

public class SmartCardModel {

	public static final Integer RESUME_OPTION = 0;
	private static final Integer CARD_FEED_TIMEOUT = 30000;

	public void pollJobStatus(ZebraCardPrinter zebraCardPrinter, int jobId, JTextArea jobStatusArea) throws ConnectionException, ZebraCardException {
		boolean done = false;
		long start = System.currentTimeMillis();

		jobStatusArea.setText("Polling status for job id " + jobId + "...\n");

		while (!done) {
			JobStatusInfo jobStatus = zebraCardPrinter.getJobStatus(jobId);

			String alarmDesc = jobStatus.alarmInfo.value > 0 ? " (" + jobStatus.alarmInfo.description + ")" : "";
			String errorDesc = jobStatus.errorInfo.value > 0 ? " (" + jobStatus.errorInfo.description + ")" : "";

			jobStatusArea.append(String.format("Job %d: status:%s, position:%s, contact:%s, contactless:%s, alarm:%d%s, error:%d%s%n", jobId, jobStatus.printStatus, jobStatus.cardPosition,
					jobStatus.contactSmartCard, jobStatus.contactlessSmartCard, jobStatus.alarmInfo.value, alarmDesc, jobStatus.errorInfo.value, errorDesc));

			if (jobStatus.printStatus.contains("done_ok")) {
				done = true;
			} else if (jobStatus.contactSmartCard.contains("at_station") || jobStatus.contactlessSmartCard.contains("at_station")) {
				waitForUserInput(zebraCardPrinter, jobId);
			} else if (jobStatus.printStatus.contains("error") || jobStatus.printStatus.contains("cancelled")) {
				done = true;
			} else if (jobStatus.errorInfo.value > 0) {
				jobStatusArea.append("The job encountered an error [" + jobStatus.errorInfo.description + "] and was cancelled.");
				zebraCardPrinter.cancel(jobId);
				done = true;
			} else if (jobStatus.alarmInfo.value > 0) {
				done = PrinterModel.waitForUserInput(zebraCardPrinter, jobId, jobStatus.alarmInfo.description);
			} else if ((jobStatus.printStatus.contains("in_progress") && jobStatus.cardPosition.contains("feeding")) // ZMotif printers
					|| (jobStatus.printStatus.contains("alarm_handling") && jobStatus.alarmInfo.value == ZebraCardErrors.MEDIA_OUT_OF_CARDS)) { // ZXP printers
				if (System.currentTimeMillis() > start + CARD_FEED_TIMEOUT) {
					zebraCardPrinter.cancel(jobId);
					jobStatusArea.append("Job ID " + jobId + " was cancelled.%n");
					done = true;
				}
			}

			if (!done) {
				PrinterModel.sleep(750);
			}
		}
	}

	private void waitForUserInput(ZebraCardPrinter zebraCardPrinter, int jobId) throws ZebraCardException, ConnectionException {
		Object[] options = { "Resume", "Cancel" };
		int option = PrinterModel.showInformationDialog(options, "At Station", "Click Resume to continue or cancel to cancel the job");

		if (option == RESUME_OPTION) {
			zebraCardPrinter.resume();
		} else {
			zebraCardPrinter.cancel(jobId);
		}
	}

	public void runSmartCardOperation(DiscoveredPrinterForDevDemo printer, String cardSource, String cardDestination, String cardType, JTextArea jobStatus) {
		ZebraCardPrinter zebraCardPrinter = null;
		Connection connection = null;

		try {
			connection = printer.getConnection();
			connection.open();

			zebraCardPrinter = ZebraCardPrinterFactory.getInstance(connection);

			zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, cardSource);
			zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, cardDestination);

			if (cardType.equalsIgnoreCase("Contact")) {
				zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACT, "yes");
			} else {
				zebraCardPrinter.setJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS, "MIFARE");
			}

			int jobId = zebraCardPrinter.smartCardEncode(1);

			if (cardSource.equalsIgnoreCase("atm")) {
				PrinterModel.showInformationDialog("Smart Card Demo", "Please place a card into the ATM slot and click Okay.");
			}

			pollJobStatus(zebraCardPrinter, jobId, jobStatus);
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "Error encoding card : " + e.getLocalizedMessage());
		} finally {
			PrinterModel.cleanUpQuietly(zebraCardPrinter, connection);
		}
	}
}
