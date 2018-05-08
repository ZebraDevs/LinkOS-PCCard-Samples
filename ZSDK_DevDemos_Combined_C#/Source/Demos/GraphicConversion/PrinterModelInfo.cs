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

using Zebra.Sdk.Card.Graphics.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.GraphicConversion {

    public class PrinterModelInfo {

        private PrinterModel printerModel;
        private string displayName;
        private int maxWidth;
        private int maxHeight;

        public PrinterModelInfo(PrinterModel printerModel, string displayName, int maxWidth, int maxHeight) {
            this.printerModel = printerModel;
            this.displayName = displayName;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }

        public PrinterModel PrinterModel {
            get => printerModel;
        }

        public int MaxHeight {
            get => maxHeight;
        }

        public int MaxWidth {
            get => maxWidth;
        }

        public new string ToString() {
            return displayName;
        }
    }
}
