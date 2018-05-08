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

package com.zebra.card.devdemo.print;

import java.awt.Color;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.swing.JOptionPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;

import org.apache.commons.io.FileUtils;

import com.zebra.card.devdemo.JobInfo;
import com.zebra.card.devdemo.PollJobStatusWorker;
import com.zebra.card.devdemo.PrinterModel;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.common.card.containers.GraphicsInfo;
import com.zebra.sdk.common.card.enumerations.CardSide;
import com.zebra.sdk.common.card.enumerations.CardSource;
import com.zebra.sdk.common.card.enumerations.GraphicType;
import com.zebra.sdk.common.card.enumerations.OrientationType;
import com.zebra.sdk.common.card.enumerations.PrintType;
import com.zebra.sdk.common.card.exceptions.ZebraCardException;
import com.zebra.sdk.common.card.graphics.ZebraCardGraphics;
import com.zebra.sdk.common.card.graphics.ZebraCardImageI;
import com.zebra.sdk.common.card.graphics.enumerations.RotationType;
import com.zebra.sdk.settings.SettingsException;

public class PrintModel extends PrinterModel {

	public void print(PrintJobOptions printJobOptions, final JTextArea jobStatusArea) throws IOException, SettingsException, ZebraCardException, ConnectionException {
		ZebraCardGraphics graphics = null;

		jobStatusArea.setText("");

		try {
			if (getConnection() == null) {
				throw new ConnectionException("No printer selected");
			}

			getConnection().open();

			graphics = new ZebraCardGraphics(getZebraCardPrinter()); // Initialize graphics for ZXP Series printers

			byte[] frontSideImageData = null;
			byte[] backSideImageData = null;
			byte[] overlayImageData = null;

			PrintType printType = null;
			boolean overlaySelected = printJobOptions.frontImageInfo.containsKey(PrintType.Overlay);

			if (printJobOptions.frontImageInfo.containsKey(PrintType.Color)) {
				frontSideImageData = FileUtils.readFileToByteArray(new File(printJobOptions.frontImageInfo.get(PrintType.Color)));
				printType = PrintType.Color;
			} else if (printJobOptions.frontImageInfo.containsKey(PrintType.MonoK)) {
				frontSideImageData = FileUtils.readFileToByteArray(new File(printJobOptions.frontImageInfo.get(PrintType.MonoK)));
				printType = PrintType.MonoK;
			}

			if (overlaySelected) {
				String overlaySideImageFile = printJobOptions.frontImageInfo.get(PrintType.Overlay);
				if (!overlaySideImageFile.isEmpty()) {
					overlayImageData = FileUtils.readFileToByteArray(new File(overlaySideImageFile));
				}
			}

			if (printJobOptions.backImageInfo.containsKey(PrintType.MonoK)) {
				backSideImageData = FileUtils.readFileToByteArray(new File(printJobOptions.backImageInfo.get(PrintType.MonoK)));
			}

			List<GraphicsInfo> graphicsData = new ArrayList<GraphicsInfo>();

			if (frontSideImageData != null) {
				graphics.initialize(0, 0, OrientationType.Landscape, printType, Color.WHITE);
				graphics.drawImage(frontSideImageData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
				graphicsData.add(buildGraphicsInfo(graphics.createImage(), CardSide.Front, printType));
				graphics.clear();
			}

			if (overlayImageData != null) {
				graphics.initialize(0, 0, OrientationType.Landscape, PrintType.Overlay, Color.WHITE);
				graphics.drawImage(overlayImageData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
				graphicsData.add(buildGraphicsInfo(graphics.createImage(), CardSide.Front, PrintType.Overlay));
				graphics.clear();
			} else if (overlaySelected) {
				graphicsData.add(buildGraphicsInfo(null, CardSide.Front, PrintType.Overlay));
				graphics.clear();
			}

			if (backSideImageData != null) {
				graphics.initialize(0, 0, OrientationType.Landscape, PrintType.MonoK, Color.WHITE);
				graphics.drawImage(backSideImageData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
				graphicsData.add(buildGraphicsInfo(graphics.createImage(), CardSide.Back, PrintType.MonoK));
				graphics.clear();
			}

			int jobId = getZebraCardPrinter().print(printJobOptions.copies, graphicsData);
			new PollJobStatusWorker(getZebraCardPrinter(), new JobInfo(jobId, CardSource.Feeder)) {
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
		} catch (ConnectionException e) {
			JOptionPane.showMessageDialog(null, e.getMessage());
		} finally {
			cleanUpQuietly();
		}
	}

	protected GraphicsInfo buildGraphicsInfo(ZebraCardImageI zebraCardImage, CardSide side, PrintType printType) throws IOException {
		GraphicsInfo grInfo = new GraphicsInfo();
		if (zebraCardImage != null) {
			grInfo.graphicData = zebraCardImage;
			grInfo.graphicType = GraphicType.BMP;
		} else {
			grInfo.graphicType = GraphicType.NA;
		}
		grInfo.side = side;
		grInfo.printType = printType;
		return grInfo;
	}
}
