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

package com.zebra.card.devdemo;

import com.zebra.sdk.common.card.enumerations.CardSource;

public class JobInfo {
	private Integer jobId;
	private final CardSource cardSource;

	public JobInfo(Integer jobId, CardSource cardSource) {
		this.jobId = jobId;
		this.cardSource = cardSource;
	}

	public Integer getJobId() {
		return jobId;
	}

	public CardSource getCardSource() {
		return cardSource;
	}

	public void setJobId(Integer jobId) {
		this.jobId = jobId;
	}
}
