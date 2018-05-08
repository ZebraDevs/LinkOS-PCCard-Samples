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

using System.Collections.ObjectModel;
using Zebra.Sdk.Card.Containers;

namespace DeveloperDemo_Card_Desktop.Demos.PrinterStatus {

    public class PrinterStatusDemoViewModel : ViewModelBase {

        private ObservableCollection<PrinterSetting> printerSettings = new ObservableCollection<PrinterSetting>();
        private ObservableCollection<PrinterSetting> generalSettings = new ObservableCollection<PrinterSetting>();
        private ObservableCollection<MediaInfo> mediaSettings = new ObservableCollection<MediaInfo>();
        private ObservableCollection<PrinterSetting> sensorSettings = new ObservableCollection<PrinterSetting>();

        public ObservableCollection<PrinterSetting> PrinterSettings {
            get => printerSettings;
        }

        public ObservableCollection<PrinterSetting> GeneralSettings {
            get => generalSettings;
        }

        public ObservableCollection<MediaInfo> MediaSettings {
            get => mediaSettings;
        }

        public ObservableCollection<PrinterSetting> SensorSettings {
            get => sensorSettings;
        }
    }
}
