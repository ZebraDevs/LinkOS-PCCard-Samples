/***********************************************
 * CONFIDENTIAL AND PROPRIETARY
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2018
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

package com.zebra.card.devdemo;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import javax.swing.SwingWorker;

import com.zebra.card.devdemo.PollJobStatusWorker.StatusUpdateInfo;
import com.zebra.sdk.common.card.containers.JobStatusInfo;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;

public class PollJobStatusWorker extends SwingWorker<Void, StatusUpdateInfo> {

	private static final int CARD_FEED_TIMEOUT = 60 * 1000;

	private final ZebraCardPrinter zebraCardPrinter;
	private List<JobInfo> jobInfoList = new ArrayList<JobInfo>();

	public PollJobStatusWorker(ZebraCardPrinter zebraCardPrinter, JobInfo jobInfo) {
		this.zebraCardPrinter = zebraCardPrinter;
		jobInfoList.add(jobInfo);
	}

	public PollJobStatusWorker(ZebraCardPrinter zebraCardPrinter, List<JobInfo> jobInfoList) {
		this.zebraCardPrinter = zebraCardPrinter;
		this.jobInfoList = jobInfoList;
	}

	@Override
	protected Void doInBackground() throws Exception {
		long startTime = System.currentTimeMillis();
		boolean showAtmDialog = true;
		boolean isFeeding = false;

		for (JobInfo jobInfo : jobInfoList) {
			publish(new StatusUpdateInfo(null, jobInfo, "Polling status for job id " + jobInfo.getJobId() + "...\n"));
		}

		while (!jobInfoList.isEmpty()) {
			for (JobInfo jobInfo : new ArrayList<JobInfo>(jobInfoList)) {
				JobStatusInfo jobStatus = zebraCardPrinter.getJobStatus(jobInfo.getJobId());

				if (!isFeeding) {
					startTime = System.currentTimeMillis();
				}

				boolean isAlarmInfoPresent = jobStatus.alarmInfo.value > 0;
				boolean isErrorInfoPresent = jobStatus.errorInfo.value > 0;
				isFeeding = jobStatus.cardPosition.contains("feeding");

				String alarmDesc = isAlarmInfoPresent ? " (" + jobStatus.alarmInfo.description + ")" : "";
				String errorDesc = isErrorInfoPresent ? " (" + jobStatus.errorInfo.description + ")" : "";

				publish(new StatusUpdateInfo(jobStatus, jobInfo, String.format("Job %d: status:%s, position:%s, contact:%s, contactless:%s, alarm:%d%s, error:%d%s\n", jobInfo.getJobId(), jobStatus.printStatus, jobStatus.cardPosition, jobStatus.contactSmartCard,
						jobStatus.contactlessSmartCard, jobStatus.alarmInfo.value, alarmDesc, jobStatus.errorInfo.value, errorDesc)));

				if (jobStatus.printStatus.equals("done_ok")) {
					publish(new StatusUpdateInfo(jobStatus, jobInfo, "Job ID " + jobInfo.getJobId() + " completed.\n"));

					showAtmDialog = true;
					startTime = System.currentTimeMillis();
					jobInfoList.remove(jobInfo);
				} else if (jobStatus.printStatus.equals("done_error")) {
					publish(new StatusUpdateInfo(jobStatus, jobInfo, "Job ID " + jobInfo.getJobId() + " completed with error: " + jobStatus.errorInfo.description + ".\n"));

					showAtmDialog = true;
					startTime = System.currentTimeMillis();
					jobInfoList.remove(jobInfo);
				} else if (jobStatus.printStatus.contains("cancelled")) {
					if (isErrorInfoPresent) {
						publish(new StatusUpdateInfo(jobStatus, jobInfo, "Job ID " + jobInfo.getJobId() + " cancelled with error: " + jobStatus.errorInfo.description + ".\n"));
					} else {
						publish(new StatusUpdateInfo(jobStatus, jobInfo, "Job ID " + jobInfo.getJobId() + " cancelled.\n"));
					}

					showAtmDialog = true;
					startTime = System.currentTimeMillis();
					jobInfoList.remove(jobInfo);
				} else if (isAlarmInfoPresent) {
					String positiveButtonText = "OK";
					String negativeButtonText = "Cancel";
					String[] options = { positiveButtonText, negativeButtonText };
					int result = PrinterModel.showInformationDialog(options, "Alarm Encountered", "The job encountered an alarm [" + jobStatus.alarmInfo.description + "].\nEither fix the alarm and click " + positiveButtonText + " once the job begins again,\nor select "
							+ negativeButtonText + " to cancel the job.");
					if (result == Arrays.asList(options).indexOf(negativeButtonText)) {
						zebraCardPrinter.cancel(jobInfo.getJobId());
					}
				} else if (isErrorInfoPresent) {
					zebraCardPrinter.cancel(jobInfo.getJobId());
				} else if (jobStatus.contactSmartCard.contains("at_station") || jobStatus.contactlessSmartCard.contains("at_station")) {
					String positiveButtonText = "Resume";
					String negativeButtonText = "Cancel";
					String[] options = { positiveButtonText, negativeButtonText };
					int result = PrinterModel.showInformationDialog(options, "Card at Station", "Click " + positiveButtonText + " to resume the job or click " + negativeButtonText + " to cancel the job.");
					if (result == Arrays.asList(options).indexOf(negativeButtonText)) {
						zebraCardPrinter.cancel(jobInfo.getJobId());
					} else {
						zebraCardPrinter.resume();
					}
				} else if (isFeeding) {
					if (showAtmDialog && jobInfo.getCardSource() == CardSource.ATM) {
						PrinterModel.showInformationDialog("Insert Card", "Please place a card into the ATM slot and click OK.");
						showAtmDialog = false;
					} else if (System.currentTimeMillis() > startTime + CARD_FEED_TIMEOUT) {
						publish(new StatusUpdateInfo(jobStatus, jobInfo, "Job ID " + jobInfo.getJobId() + " timed out waiting for a card and was cancelled.\n"));
						zebraCardPrinter.cancel(jobInfo.getJobId());
					}
				}

				try {
					Thread.sleep(500);
				} catch (InterruptedException e) {
					// Do nothing
				}
			}
		}
		return null;
	}

	public class StatusUpdateInfo {
		private final JobStatusInfo jobStatusInfo;
		private final JobInfo jobInfo;
		private final String message;

		public StatusUpdateInfo(JobStatusInfo jobStatusInfo, JobInfo jobInfo, String message) {
			this.jobStatusInfo = jobStatusInfo;
			this.jobInfo = jobInfo;
			this.message = message;
		}

		public JobStatusInfo getJobStatusInfo() {
			return jobStatusInfo;
		}

		public JobInfo getJobInfo() {
			return jobInfo;
		}

		public String getMessage() {
			return message;
		}
	}
}
