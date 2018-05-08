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

import java.util.EnumSet;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeoutException;

import javax.swing.JTextArea;
import javax.swing.SwingUtilities;

import com.zebra.card.devdemo.JobInfo;
import com.zebra.card.devdemo.PollJobStatusWorker;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.MagTrackData;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.DataSource;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.settings.SettingsException;

public class MagEncodeModel extends PrinterModel {

	public void magEncode(MagEncodeContainer container, final JTextArea jobStatusArea) throws ConnectionException, SettingsException, ZebraCardException {
		jobStatusArea.setText("");

		Map<String, String> jobSettings = new HashMap<String, String>();

		jobSettings.put(ZebraCardJobSettingNames.CARD_SOURCE, container.cardSource);
		jobSettings.put(ZebraCardJobSettingNames.CARD_DESTINATION, container.cardDestination);
		jobSettings.put(ZebraCardJobSettingNames.MAG_COERCIVITY, container.coercivityType);

		if (container.verify) {
			jobSettings.put(ZebraCardJobSettingNames.MAG_VERIFY, "yes");
		} else {
			jobSettings.put(ZebraCardJobSettingNames.MAG_VERIFY, "no");
		}

		getZebraCardPrinter().setJobSettings(jobSettings);

		int jobId = getZebraCardPrinter().magEncode(container.quantity, container.track1Data, container.track2Data, container.track3Data);
		new PollJobStatusWorker(getZebraCardPrinter(), new JobInfo(jobId, CardSource.fromString(container.cardSource))) {
			@Override
			protected void process(final List<StatusUpdateInfo> updateList) {
				SwingUtilities.invokeLater(new Runnable() {

					@Override
					public void run() {
						StatusUpdateInfo update = updateList.get(updateList.size() - 1);
						jobStatusArea.append(update.getMessage());
					}
				});
			};
		}.execute();
	}

	public MagTrackData magRead(EnumSet<DataSource> tracksToRead, String source, String destination) throws ConnectionException, SettingsException, ZebraCardException, TimeoutException {
		Map<String, String> jobSettings = new HashMap<String, String>();

		jobSettings.put(ZebraCardJobSettingNames.CARD_SOURCE, source);
		jobSettings.put(ZebraCardJobSettingNames.CARD_DESTINATION, destination);

		getZebraCardPrinter().setJobSettings(jobSettings);
		return getZebraCardPrinter().readMagData(tracksToRead, true);
	}
}
