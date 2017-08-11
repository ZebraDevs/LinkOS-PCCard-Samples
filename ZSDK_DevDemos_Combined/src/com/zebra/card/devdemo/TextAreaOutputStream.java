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

package com.zebra.card.devdemo;

import java.io.*;

import javax.swing.JTextArea;

public class TextAreaOutputStream extends OutputStream {
	private final JTextArea statusTextArea;

	public TextAreaOutputStream(JTextArea control) {
		statusTextArea = control;
	}

	@Override
	public void write(int b) throws IOException {
		statusTextArea.append(String.valueOf((char) b));
	}
}
