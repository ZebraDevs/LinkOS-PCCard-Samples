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

import org.apache.commons.io.FileUtils;

import com.zebra.card.devdemo.print.PrintJobOptions;
import com.zebra.card.devdemo.print.PrintModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.GraphicsInfo;
import com.zebra.sdk.common.card.enumerations.CardSide;
import com.zebra.sdk.common.card.enumerations.GraphicType;
import com.zebra.sdk.common.card.enumerations.OrientationType;
import com.zebra.sdk.common.card.enumerations.PrintType;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.graphics.ZebraCardGraphics;
import com.zebra.sdk.common.card.graphics.enumerations.RotationType;
import com.zebra.sdk.common.card.printer.ZebraCardPrinter;
import com.zebra.sdk.settings.SettingsException;

public class MultiJobModel extends PrintModel {

	public int magEncode(ZebraCardPrinter zebraCardPrinter, PrintJobOptions printJobOptions) throws ConnectionException, SettingsException, ZebraCardException {
		return zebraCardPrinter.magEncode(printJobOptions.copies, printJobOptions.track1Data, printJobOptions.track2Data, printJobOptions.track3Data);
	}

	public int printAndMagEncode(ZebraCardPrinter zebraCardPrinter, PrintJobOptions printJobOptions) throws IOException, ConnectionException, SettingsException, ZebraCardException {
		int jobId = -1;

		ZebraCardGraphics graphics = null;
		try {
			List<GraphicsInfo> graphicsInfo = new ArrayList<GraphicsInfo>();

			// Initialize graphics for printer object
			graphics = new ZebraCardGraphics(zebraCardPrinter);

			if (printJobOptions.frontSelected) {
				graphicsInfo.addAll(createGraphicsInfo(graphics, printJobOptions.frontImageInfo, CardSide.Front));
			}

			if (printJobOptions.backSelected) {
				graphicsInfo.addAll(createGraphicsInfo(graphics, printJobOptions.backImageInfo, CardSide.Back));
			}

			if (jobContainsTrackData(printJobOptions)) {
				jobId = zebraCardPrinter.printAndMagEncode(printJobOptions.copies, graphicsInfo, printJobOptions.track1Data, printJobOptions.track2Data, printJobOptions.track3Data);
			} else {
				jobId = zebraCardPrinter.print(printJobOptions.copies, graphicsInfo);
			}
		} finally {
			if (graphics != null) {
				graphics.close();
			}
		}
		return jobId;
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
}
