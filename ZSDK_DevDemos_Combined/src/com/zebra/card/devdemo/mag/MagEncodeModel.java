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
import java.util.Map;
import java.util.concurrent.TimeoutException;

import javax.swing.JTextArea;

import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.MagTrackData;
import com.zebra.sdk.common.card.enumerations.DataSource;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.settings.SettingsException;

public class MagEncodeModel extends PrinterModel {

	public void MagEncode(ZebraCardPrinter zebraCardPrinter, MagEncodeContainer container, JTextArea jobStatusArea) throws ConnectionException, SettingsException, ZebraCardException {
		Map<String, String> jobSettings = new HashMap<String, String>();

		jobSettings.put(ZebraCardJobSettingNames.CARD_SOURCE, container.cardSource);
		jobSettings.put(ZebraCardJobSettingNames.CARD_DESTINATION, container.cardDestination);
		jobSettings.put(ZebraCardJobSettingNames.MAG_COERCIVITY, container.coercivityType);

		if (container.verify) {
			jobSettings.put(ZebraCardJobSettingNames.MAG_VERIFY, "yes");
		} else {
			jobSettings.put(ZebraCardJobSettingNames.MAG_VERIFY, "no");
		}

		zebraCardPrinter.setJobSettings(jobSettings);
		int jobId = zebraCardPrinter.magEncode(container.quantity, container.track1Data, container.track2Data, container.track3Data);

		if (container.cardSource.equalsIgnoreCase("atm")) {
			showInformationDialog("Mag Encode Demo", "Please place a card into the ATM slot and click Okay.");
		}

		pollJobStatus(zebraCardPrinter, jobId, jobStatusArea);
	}

	public MagTrackData MagRead(ZebraCardPrinter zebraCardPrinter, EnumSet<DataSource> tracksToRead, String source, String destination)
			throws ConnectionException, SettingsException, ZebraCardException, TimeoutException {
		Map<String, String> jobSettings = new HashMap<String, String>();

		jobSettings.put(ZebraCardJobSettingNames.CARD_SOURCE, source);
		jobSettings.put(ZebraCardJobSettingNames.CARD_DESTINATION, destination);

		zebraCardPrinter.setJobSettings(jobSettings);
		return zebraCardPrinter.readMagData(tracksToRead, true);
	}
}
