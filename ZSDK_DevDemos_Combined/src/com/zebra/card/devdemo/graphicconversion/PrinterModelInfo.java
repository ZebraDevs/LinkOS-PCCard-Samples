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

package com.zebra.card.devdemo.graphicconversion;

import com.zebra.sdk.common.card.graphics.enumerations.PrinterModel;

public enum PrinterModelInfo {
	ZXP1(PrinterModel.ZXPSeries1, "ZXP1", 1024, 640),
	ZXP3(PrinterModel.ZXPSeries3, "ZXP3", 1024, 640),
	ZXP7(PrinterModel.ZXPSeries7, "ZXP7", 1006, 640),
	ZXP8(PrinterModel.ZXPSeries8, "ZXP8", 1024, 648),
	ZXP9(PrinterModel.ZXPSeries9, "ZXP9", 1024, 648),
	ZC100(PrinterModel.ZC100, "ZC100", 1006, 640),
	ZC150(PrinterModel.ZC100, "ZC150", 1006, 640),
	ZC300(PrinterModel.ZC300, "ZC300", 1006, 640),
	ZC350(PrinterModel.ZC300, "ZC350", 1006, 640);

	private PrinterModel printerModel;
	private String displayName;
	private int maxWidth;
	private int maxHeight;

	private PrinterModelInfo(PrinterModel printerModel, String displayName, int maxWidth, int maxHeight) {
		this.printerModel = printerModel;
		this.displayName = displayName;
		this.maxWidth = maxWidth;
		this.maxHeight = maxHeight;
	}

	public PrinterModel getPrinterModel() {
		return printerModel;
	}

	public int getMaxWidth() {
		return maxWidth;
	}

	public int getMaxHeight() {
		return maxHeight;
	}

	@Override
	public String toString() {
		return displayName;
	}
}
