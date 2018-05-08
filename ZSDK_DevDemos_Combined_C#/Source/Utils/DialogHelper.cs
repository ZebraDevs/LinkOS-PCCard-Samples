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

using DeveloperDemo_Card_Desktop.Dialogs;
using System.Windows;

namespace DeveloperDemo_Card_Desktop.Utils {

    public static class DialogHelper {

        public static Microsoft.Win32.OpenFileDialog CreateOpenImageFileDialog() {
            return new Microsoft.Win32.OpenFileDialog {
                Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png;"
            };
        }

        public static Microsoft.Win32.SaveFileDialog CreateSaveImageFileDialog() {
            return new Microsoft.Win32.SaveFileDialog {
                Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png;",
                OverwritePrompt = false
            };
        }

        public static Microsoft.Win32.OpenFileDialog CreateOpenTemplateFileDialog() {
            return new Microsoft.Win32.OpenFileDialog {
                Filter = "Template Files (*.xml)|*.xml;"
            };
        }

        public static MessageDialog ShowInsertCardDialog() {
            MessageDialog messageDialog = null;
            Application.Current.Dispatcher.Invoke(() => {
                messageDialog = new MessageDialog {
                    Title = "Insert Card",
                    Message = "Please insert a card into the ATM slot.",
                    HasNegativeButton = false
                };
                messageDialog.Show();
            });
            return messageDialog;
        }
    }
}
