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

import java.util.Map;

import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.printer.discovery.DiscoveredPrinter;

public class DiscoveredPrinterForDevDemo {
	private DiscoveredPrinter printer;

	public DiscoveredPrinterForDevDemo(DiscoveredPrinter printer) {
		this.printer = printer;
	}

	public Connection getConnection() {
		return printer.getConnection();
	}

	@Override
	public String toString() {
		return printer.address + " : " + getModel();
	}

	private String getModel() {
		Map<String, String> discoveryDataMap = printer.getDiscoveryDataMap();
		String model = discoveryDataMap.get("MODEL");
		return model;
	}
}
