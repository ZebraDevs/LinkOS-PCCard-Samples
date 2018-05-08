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

namespace DeveloperDemo_Card_Desktop.Dialogs {

    public class MessageDialogViewModel : ViewModelBase {

        private string message = "";
        private string positiveButtonText = "OK";
        private string negativeButtonText = "Cancel";
        private bool hasNegativeButton = true;

        public string Message {
            get => message;
            set {
                message = value;
                OnPropertyChanged();
            }
        }

        public string PositiveButtonText {
            get => positiveButtonText;
            set {
                positiveButtonText = value;
                OnPropertyChanged();
            }
        }

        public string NegativeButtonText {
            get => negativeButtonText;
            set {
                negativeButtonText = value;
                OnPropertyChanged();
            }
        }

        public bool HasNegativeButton {
            get => hasNegativeButton;
            set {
                hasNegativeButton = value;
                OnPropertyChanged();
            }
        }
    }
}
