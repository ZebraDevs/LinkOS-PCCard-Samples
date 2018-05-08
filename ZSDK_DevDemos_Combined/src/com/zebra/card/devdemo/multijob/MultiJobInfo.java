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

package com.zebra.card.devdemo.multijob;

import com.zebra.card.devdemo.JobInfo;
import com.zebra.sdk.common.card.enumerations.CardSource;

public class MultiJobInfo extends JobInfo {
	private final Job job;

	public MultiJobInfo(CardSource cardSource, Integer jobId, Job job) {
		super(jobId, cardSource);
		this.job = job;
	}

	public Job getJob() {
		return job;
	}
}
