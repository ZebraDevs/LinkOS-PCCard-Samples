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

package com.zebra.card.devdemo.graphicconversion;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.HashMap;

import javax.imageio.ImageIO;
import javax.swing.JOptionPane;
import javax.swing.JTextArea;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;

import com.zebra.sdk.common.card.enumerations.OrientationType;
import com.zebra.sdk.common.card.enumerations.PrintType;
import com.zebra.sdk.common.card.graphics.ZebraCardGraphics;
import com.zebra.sdk.common.card.graphics.ZebraCardImageI;
import com.zebra.sdk.common.card.graphics.ZebraGraphics;
import com.zebra.sdk.common.card.graphics.enumerations.MonochromeConversion;
import com.zebra.sdk.common.card.graphics.enumerations.PrinterModel;
import com.zebra.sdk.common.card.graphics.enumerations.RotationType;

public class GraphicConverter {

	public static final String GRAY_HALFTONE_8X8 = "Gray - halftone 8x8";
	public static final String GRAY_HALFTONE_6X6 = "Gray - halftone 6x6";
	public static final String GRAPHICS_FORMAT_GRAY_DIFFUSION = "Gray - diffusion";
	public static final String GRAPHICS_FORMAT_MONO_HALFTONE_8X8 = "Mono - halftone 8x8";
	public static final String GRAPHICS_FORMAT_MONO_HALFTONE_6X6 = "Mono - halftone 6x6";
	public static final String GRAPHICS_FORMAT_MONO_DIFFUSION = "Mono - diffusion";
	public static final String GRAPHICS_FORMAT_COLOR = "Color";

	private final String inputFilePath;
	private final String outputFilePath;
	private final String printerModelString;
	private final String format;
	private final String widthString;
	private final String heightString;
	private final String xoffsetString;
	private final String yoffsetString;

	private final DimensionOption dimensionOption;
	private final JTextArea outputArea;

	private final HashMap<PrinterModel, Integer> maxHeightForLandscape = new HashMap<PrinterModel, Integer>();
	private final HashMap<PrinterModel, Integer> maxWidthForLandscape = new HashMap<PrinterModel, Integer>();

	protected enum DimensionOption {
		original, crop, resize;
	}

	public GraphicConverter(GraphicsContainer graphicsContainer, JTextArea outputArea) {
		this.outputArea = outputArea;

		this.inputFilePath = graphicsContainer.inputFilePath;
		this.outputFilePath = graphicsContainer.outputFilePath;
		this.printerModelString = graphicsContainer.printerModelString;
		this.format = graphicsContainer.format;
		this.widthString = graphicsContainer.widthString;
		this.heightString = graphicsContainer.heightString;
		this.dimensionOption = graphicsContainer.dimensionOption;
		this.xoffsetString = graphicsContainer.xoffsetString;
		this.yoffsetString = graphicsContainer.yoffsetString;

		maxHeightForLandscape.put(PrinterModel.ZXPSeries1, 640);
		maxHeightForLandscape.put(PrinterModel.ZXPSeries3, 640);
		maxHeightForLandscape.put(PrinterModel.ZXPSeries7, 640);
		maxHeightForLandscape.put(PrinterModel.ZXPSeries8, 648);
		maxHeightForLandscape.put(PrinterModel.ZXPSeries9, 648);

		maxWidthForLandscape.put(PrinterModel.ZXPSeries1, 1024);
		maxWidthForLandscape.put(PrinterModel.ZXPSeries3, 1024);
		maxWidthForLandscape.put(PrinterModel.ZXPSeries7, 1006);
		maxWidthForLandscape.put(PrinterModel.ZXPSeries8, 1024);
		maxWidthForLandscape.put(PrinterModel.ZXPSeries9, 1024);
	}

	protected void verboseFormatPrint(String format, Object... args) {
		String message = String.format(format, args);
		outputArea.append(message);
	}

	protected void verbosePrint(String message) {
		outputArea.append(message);
	}

	public static int[] getImageSize(String imageFilePath) throws IOException {
		int[] heightAndWidth = new int[2];
		BufferedImage img = ImageIO.read(new File(imageFilePath));
		heightAndWidth[0] = img.getHeight();
		heightAndWidth[1] = img.getWidth();
		return heightAndWidth;
	}

	public void processImage() throws IOException, Exception {
		ZebraGraphics graphics = null;
		FileOutputStream fileOutputStream = null;

		try {
			graphics = new ZebraCardGraphics(null);

			int width = widthString.isEmpty() ? 0 : Integer.parseInt(widthString);
			int height = heightString.isEmpty() ? 0 : Integer.parseInt(heightString);

			if (height == 0 || width == 0) {
				verbosePrint("Keeping current image dimensions unless they exceed the maximum model specific height and width\n");
			}

			if (width < 0) {
				String errorMessage = "";
				if (height < 0) {
					errorMessage = String.format("Error converting graphic: Width(%d) and height(%d) must be > 0.", width, height);
				} else {
					errorMessage = String.format("Error converting graphic: Width(%d) must be > 0.", width);
				}
				showConversionErrorDialog(errorMessage);
				outputArea.setText("");
			} else if (height < 0) {
				String errorMessage = String.format("Error converting graphic: Height(%d) must be > 0.", height);
				showConversionErrorDialog(errorMessage);
				outputArea.setText("");
			} else {
				byte[] imageData = FileUtils.readFileToByteArray(new File(inputFilePath));
				fileOutputStream = new FileOutputStream(outputFilePath);

				if (dimensionOption == DimensionOption.original || dimensionOption == DimensionOption.resize) {
					PrinterModel printerModel = getPrinterModelType(printerModelString);
					graphics.setPrinterModel(printerModel);

					if (dimensionOption != DimensionOption.original) {
						int maxWidth = maxWidthForLandscape.get(printerModel);
						int maxHeigth = maxHeightForLandscape.get(printerModel);

						if (width > maxWidth) {
							verboseFormatPrint("Specified width %s is greater than the maximum width %d\n", widthString, maxWidth);
							width = maxWidth;
						}

						if (height > maxHeigth) {
							verboseFormatPrint("Specified height %s is greater than the maximum height %d\n", heightString, maxHeigth);
							height = maxHeigth;
						}

						if (width != 0 && height != 0) {
							verboseFormatPrint("Resizing image to %dx%d\n", width, height);
						}
					} else {
						height = 0;
						width = 0;
					}

					OrientationType orientationType = OrientationType.Landscape;
					verboseFormatPrint("Setting orientation to %s\n", orientationType);

					PrintType printType = getPrintType();
					graphics.initialize(width, height, orientationType, printType, Color.WHITE);
					graphics.drawImage(imageData, 0, 0, width, height, RotationType.RotateNoneFlipNone);

					verbosePrint("Converting graphic\n");
					MonochromeConversion monochromeConversionType = getMonochromeConversionType();
					applyMonochromeConversion(graphics, printType, monochromeConversionType);

					ZebraCardImageI zci = graphics.createImage(null);
					fileOutputStream.write(zci.getImageData());
					verbosePrint("Finished converting graphic\n");
				} else {
					int xoffset = xoffsetString.isEmpty() ? 0 : Integer.parseInt(xoffsetString);
					int yoffset = yoffsetString.isEmpty() ? 0 : Integer.parseInt(yoffsetString);

					verboseFormatPrint("Cropping image from xOffset:%d yOffset:%d with width:%d and height:%d\n", xoffset, yoffset, width, height);
					imageData = graphics.cropImage(imageData, xoffset, yoffset, width, height);
					fileOutputStream.write(imageData);

					verbosePrint("Finished cropping image\n");
				}
			}
		} finally {
			if (graphics != null) {
				graphics.close();
			}

			IOUtils.closeQuietly(fileOutputStream);
		}
	}

	private void showConversionErrorDialog(String errorMessage) {
		Object[] options = { "Okay" };
		JOptionPane.showOptionDialog(null, errorMessage, "Graphic Conversion Error", JOptionPane.PLAIN_MESSAGE, JOptionPane.QUESTION_MESSAGE, null, options, options[0]);
	}

	private void applyMonochromeConversion(ZebraGraphics graphics, PrintType printType, MonochromeConversion monochromeConversionType) {
		if (monochromeConversionType == MonochromeConversion.Diffusion) {
			if (printType != PrintType.MonoK && printType != PrintType.GrayDye) {
				verbosePrint("Ignoring diffusion option for non-mono/gray format type\n");
			} else {
				graphics.monochromeConversionType(MonochromeConversion.Diffusion);
				verbosePrint("Applying diffusion algorithm\n");
			}
		} else if (monochromeConversionType == MonochromeConversion.HalfTone_6x6 || monochromeConversionType == MonochromeConversion.HalfTone_8x8) {
			if (printType != PrintType.MonoK && printType != PrintType.GrayDye) {
				verbosePrint("Ignoring halftone option for non-mono/gray format type\n");
			} else {
				if (monochromeConversionType == MonochromeConversion.HalfTone_6x6) {
					graphics.monochromeConversionType(MonochromeConversion.HalfTone_6x6);
					verbosePrint("Applying 6x6 halftone algorithm\n");
				} else if (monochromeConversionType == MonochromeConversion.HalfTone_8x8) {
					graphics.monochromeConversionType(MonochromeConversion.HalfTone_8x8);
					verbosePrint("Applying 8x8 halftone algorithm\n");
				}
			}
		}
	}

	private PrintType getPrintType() {
		PrintType printType = PrintType.Color;
		if (format.equalsIgnoreCase(GraphicConverter.GRAPHICS_FORMAT_COLOR)) {
			printType = PrintType.Color;
		} else if (format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_8X8) || format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_6X6)
				|| format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_DIFFUSION)) {
			printType = PrintType.MonoK;
		} else if (format.equalsIgnoreCase(GRAY_HALFTONE_8X8) || format.equalsIgnoreCase(GRAY_HALFTONE_6X6) || format.equalsIgnoreCase(GRAPHICS_FORMAT_GRAY_DIFFUSION)) {
			printType = PrintType.GrayDye;
		}
		return printType;
	}

	private MonochromeConversion getMonochromeConversionType() {
		MonochromeConversion conversionType = MonochromeConversion.None;
		if (format.equalsIgnoreCase(GraphicConverter.GRAPHICS_FORMAT_COLOR)) {
			conversionType = MonochromeConversion.None;
		} else if (format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_DIFFUSION) || format.equalsIgnoreCase(GRAPHICS_FORMAT_GRAY_DIFFUSION)) {
			conversionType = MonochromeConversion.Diffusion;
		} else if (format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_6X6) || format.equalsIgnoreCase(GRAY_HALFTONE_6X6)) {
			conversionType = MonochromeConversion.HalfTone_6x6;
		} else if (format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_8X8) || format.equalsIgnoreCase(GRAY_HALFTONE_8X8)) {
			conversionType = MonochromeConversion.HalfTone_8x8;
		}
		return conversionType;
	}

	public static PrinterModel getPrinterModelType(String printerModelString) throws IllegalArgumentException {
		PrinterModel printerModel = PrinterModel.ZXPSeries7;
		if (printerModelString.equalsIgnoreCase("zxp1")) {
			printerModel = PrinterModel.ZXPSeries1;
		} else if (printerModelString.equalsIgnoreCase("zxp3")) {
			printerModel = PrinterModel.ZXPSeries3;
		} else if (printerModelString.equalsIgnoreCase("zxp7")) {
			printerModel = PrinterModel.ZXPSeries7;
		} else if (printerModelString.equalsIgnoreCase("zxp8")) {
			printerModel = PrinterModel.ZXPSeries8;
		} else if (printerModelString.equalsIgnoreCase("zxp9")) {
			printerModel = PrinterModel.ZXPSeries9;
		}
		return printerModel;
	}
}
