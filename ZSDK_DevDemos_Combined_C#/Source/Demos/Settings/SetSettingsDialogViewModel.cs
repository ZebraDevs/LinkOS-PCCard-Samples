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

namespace DeveloperDemo_Card_Desktop.Demos.Settings {

    public class SetSettingsDialogViewModel : ViewModelBase {

        private string settingName;
        private string oldSettingValue;
        private string newSettingValue;
        private string range;
        private string type;

        public string SettingName {
            get => settingName;
            set {
                settingName = value;
                OnPropertyChanged();
            }
        }

        public string OldSettingValue {
            get => oldSettingValue;
            set {
                oldSettingValue = value;
                OnPropertyChanged();
            }
        }

        public string NewSettingValue {
            get => newSettingValue;
            set {
                newSettingValue = value;
                OnPropertyChanged();
            }
        }

        public string Range {
            get => range;
            set {
                range = value;
                OnPropertyChanged();
            }
        }

        public string Type {
            get => type;
            set {
                type = value;
                OnPropertyChanged();
            }
        }
    }
}
