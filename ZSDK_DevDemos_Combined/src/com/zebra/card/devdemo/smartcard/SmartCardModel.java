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

import java.util.List;

import javax.swing.JOptionPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;

import com.zebra.card.devdemo.JobInfo;
import com.zebra.card.devdemo.PollJobStatusWorker;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.jobSettings.ZebraCardJobSettingNames;

public class SmartCardModel extends PrinterModel {
	public void runSmartCardOperation(String cardSource, String cardDestination, String cardType, final JTextArea jobStatus) {
		jobStatus.setText("");

		try {
			getConnection().open();

			getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.CARD_SOURCE, cardSource);
			getZebraCardPrinter().setJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION, cardDestination);

			boolean isEncoderTypeContact = cardType.equalsIgnoreCase("contact") || cardType.equalsIgnoreCase("contact_station");
			String settingName = isEncoderTypeContact ? ZebraCardJobSettingNames.SMART_CARD_CONTACT : ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS;
			String settingValue = isEncoderTypeContact ? "yes" : cardType;

			getZebraCardPrinter().setJobSetting(settingName, settingValue);

			int jobId = getZebraCardPrinter().smartCardEncode(1);
			new PollJobStatusWorker(getZebraCardPrinter(), new JobInfo(jobId, CardSource.fromString(cardSource))) {
				@Override
				protected void process(final List<StatusUpdateInfo> updateList) {
					SwingUtilities.invokeLater(new Runnable() {

						@Override
						public void run() {
							StatusUpdateInfo update = updateList.get(updateList.size() - 1);
							jobStatus.append(update.getMessage());
						}
					});
				};
			}.execute();
		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "Error encoding card : " + e.getLocalizedMessage());
		} finally {
			cleanUpQuietly();
		}
	}
}
