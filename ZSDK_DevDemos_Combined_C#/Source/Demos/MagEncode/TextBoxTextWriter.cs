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

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DeveloperDemo_Card_Desktop.Demos.MagEncode {

    public class TextBoxTextWriter : TextWriter {

        private TextBox textBox;

        public TextBoxTextWriter(TextBox textBox) {
            this.textBox = textBox;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value) {
            base.Write(value);

            Application.Current.Dispatcher.Invoke(() => {
                textBox.AppendText(value.ToString());
                textBox.ScrollToEnd();
            });
        }
    }
}
