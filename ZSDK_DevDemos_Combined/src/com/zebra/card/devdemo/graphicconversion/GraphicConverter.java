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

import javax.imageio.ImageIO;
import javax.swing.JOptionPane;
import javax.swing.JTextArea;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;

import com.zebra.sdk.common.card.enumerations.OrientationType;
import com.zebra.sdk.common.card.enumerations.PrintType;
import com.zebra.sdk.common.card.graphics.ZebraCardGraphics;
import com.zebra.sdk.common.card.graphics.ZebraGraphics;
import com.zebra.sdk.common.card.graphics.enumerations.MonochromeConversion;
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
	private final PrinterModelInfo printerModelInfo;
	private final String format;
	private final String widthString;
	private final String heightString;
	private final String xoffsetString;
	private final String yoffsetString;

	private final DimensionOption dimensionOption;
	private final JTextArea outputArea;

	protected enum DimensionOption {
		original, crop, resize;
	}

	public GraphicConverter(GraphicsContainer graphicsContainer, JTextArea outputArea) {
		this.outputArea = outputArea;

		this.inputFilePath = graphicsContainer.inputFilePath;
		this.outputFilePath = graphicsContainer.outputFilePath;
		this.printerModelInfo = graphicsContainer.printerModelInfo;
		this.format = graphicsContainer.format;
		this.widthString = graphicsContainer.widthString;
		this.heightString = graphicsContainer.heightString;
		this.dimensionOption = graphicsContainer.dimensionOption;
		this.xoffsetString = graphicsContainer.xoffsetString;
		this.yoffsetString = graphicsContainer.yoffsetString;
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

	private int constrainDimension(int value, int maxValue, String maxExceededMessage, String minExceededMessage) {
		if (value > maxValue) {
			verbosePrint(maxExceededMessage);
			showConversionErrorDialog(maxExceededMessage);
			outputArea.setText("");

			value = maxValue;
		} else if (value < 1) {
			verbosePrint(minExceededMessage);
			showConversionErrorDialog(minExceededMessage);
			outputArea.setText("");

			value = 1;
		}
		return value;
	}

	public void processImage() throws IOException, Exception {
		ZebraGraphics graphics = null;
		FileOutputStream fileOutputStream = null;

		try {
			graphics = new ZebraCardGraphics(null);
			graphics.setPrinterModel(printerModelInfo.getPrinterModel());

			int[] imageSize = getImageSize(inputFilePath);

			int inputWidth = widthString.isEmpty() ? 0 : Integer.parseInt(widthString);
			int inputHeight = heightString.isEmpty() ? 0 : Integer.parseInt(heightString);

			String widthGreaterThanMaxMessage = String.format("Specified width %s is greater than the maximum width %d. Setting width to maximum width...\n", widthString, printerModelInfo.getMaxWidth());
			String widthNotPositiveMessage = "Width must be positive. Setting width to 1...";
			String heightGreaterThanMaxMessage = String.format("Specified height %s is greater than the maximum height %d. Setting height to maximum height...\n", heightString, printerModelInfo.getMaxHeight());
			String heightNotPositiveMessage = "Height must be positive. Setting height to 1...";

			byte[] imageData = FileUtils.readFileToByteArray(new File(inputFilePath));

			int width; // Width of final output image
			int height; // Height of final output image

			switch (dimensionOption) {
				case crop:
					int croppedWidth = constrainDimension(inputWidth, printerModelInfo.getMaxWidth(), widthGreaterThanMaxMessage, widthNotPositiveMessage);
					int croppedHeight = constrainDimension(inputHeight, printerModelInfo.getMaxHeight(), heightGreaterThanMaxMessage, heightNotPositiveMessage);
					imageData = cropImage(graphics, imageData, croppedWidth, croppedHeight);

					width = croppedWidth;
					height = croppedHeight;
					break;
				case resize:
					width = constrainDimension(inputWidth, printerModelInfo.getMaxWidth(), widthGreaterThanMaxMessage, widthNotPositiveMessage);
					height = constrainDimension(inputHeight, printerModelInfo.getMaxHeight(), heightGreaterThanMaxMessage, heightNotPositiveMessage);

					verboseFormatPrint("Resizing image to %dx%d...\n", width, height);
					break;
				case original:
				default:
					width = constrainDimension(imageSize[1], printerModelInfo.getMaxWidth(), widthGreaterThanMaxMessage, widthNotPositiveMessage);
					height = constrainDimension(imageSize[0], printerModelInfo.getMaxHeight(), heightGreaterThanMaxMessage, heightNotPositiveMessage);

					verbosePrint("Keeping current image dimensions unless they exceed the maximum model-specific width and height...\n");
					break;
			}

			fileOutputStream = new FileOutputStream(outputFilePath);

			MonochromeConversion monochromeConversionType = getMonochromeConversionType();
			PrintType printType = getPrintType();
			OrientationType orientationType = OrientationType.Landscape;

			verboseFormatPrint("Setting orientation to %s...\n", orientationType);

			graphics.initialize(width, height, orientationType, printType, Color.WHITE);
			graphics.drawImage(imageData, 0, 0, width, height, RotationType.RotateNoneFlipNone);
			applyMonochromeConversion(graphics, printType, monochromeConversionType);

			verboseFormatPrint("Writing graphic file to path %s...\n", outputFilePath);

			fileOutputStream.write(graphics.createImage().getImageData());

			verbosePrint("Finished converting graphic\n");
		} finally {
			if (graphics != null) {
				graphics.close();
			}

			IOUtils.closeQuietly(fileOutputStream);
		}
	}

	private void showConversionErrorDialog(String errorMessage) {
		Object[] options = { "OK" };
		JOptionPane.showOptionDialog(null, errorMessage, "Graphic Conversion Error", JOptionPane.PLAIN_MESSAGE, JOptionPane.QUESTION_MESSAGE, null, options, options[0]);
	}

	private byte[] cropImage(ZebraGraphics graphics, byte[] imageData, int croppedWidth, int croppedHeight) throws IOException {
		int xoffset = xoffsetString.isEmpty() ? 0 : Integer.parseInt(xoffsetString);
		int yoffset = yoffsetString.isEmpty() ? 0 : Integer.parseInt(yoffsetString);

		verboseFormatPrint("Cropping image from xOffset:%d yOffset:%d with width:%d and height:%d...\n", xoffset, yoffset, croppedWidth, croppedHeight);

		byte[] croppedImage = graphics.cropImage(imageData, xoffset, yoffset, croppedWidth, croppedHeight);

		verbosePrint("Finished cropping image");

		return croppedImage;
	}

	private void applyMonochromeConversion(ZebraGraphics graphics, PrintType printType, MonochromeConversion monochromeConversionType) {
		verbosePrint("Converting graphic...\n");

		if (monochromeConversionType == MonochromeConversion.Diffusion) {
			if (printType != PrintType.MonoK && printType != PrintType.GrayDye) {
				verbosePrint("Ignoring diffusion option for non-mono/gray format type...\n");
			} else {
				graphics.monochromeConversionType(MonochromeConversion.Diffusion);
				verbosePrint("Applying diffusion algorithm...\n");
			}
		} else if (monochromeConversionType == MonochromeConversion.HalfTone_6x6 || monochromeConversionType == MonochromeConversion.HalfTone_8x8) {
			if (printType != PrintType.MonoK && printType != PrintType.GrayDye) {
				verbosePrint("Ignoring halftone option for non-mono/gray format type...\n");
			} else {
				if (monochromeConversionType == MonochromeConversion.HalfTone_6x6) {
					graphics.monochromeConversionType(MonochromeConversion.HalfTone_6x6);
					verbosePrint("Applying 6x6 halftone algorithm...\n");
				} else if (monochromeConversionType == MonochromeConversion.HalfTone_8x8) {
					graphics.monochromeConversionType(MonochromeConversion.HalfTone_8x8);
					verbosePrint("Applying 8x8 halftone algorithm...\n");
				}
			}
		}
	}

	private PrintType getPrintType() {
		PrintType printType = PrintType.Color;
		if (format.equalsIgnoreCase(GraphicConverter.GRAPHICS_FORMAT_COLOR)) {
			printType = PrintType.Color;
		} else if (format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_8X8) || format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_HALFTONE_6X6) || format.equalsIgnoreCase(GRAPHICS_FORMAT_MONO_DIFFUSION)) {
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
}
