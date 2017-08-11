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

import java.util.HashMap;
import java.util.Map;

import com.zebra.sdk.common.card.enumerations.PrintType;

public class PrintJobOptions {

	public boolean frontSelected = false;
	public boolean backSelected = false;
	public Map<PrintType, String> frontImageInfo = new HashMap<PrintType, String>();
	public Map<PrintType, String> backImageInfo = new HashMap<PrintType, String>();
	public String track1Data = "";
	public String track2Data = "";
	public String track3Data = "";
	public int copies = 1;
}
