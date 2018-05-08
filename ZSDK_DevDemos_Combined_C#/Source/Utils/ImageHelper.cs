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

using System;
using System.Drawing;

namespace DeveloperDemo_Card_Desktop.Utils {

    public static class ImageHelper {

        public static Image CreateImageFromFile(string filename) {
            try {
                return Image.FromFile(filename);
            } catch (Exception exception) {
                throw new ArgumentException($"Could not create image object from file {filename}", exception);
            }
        }

        public static byte[] ConvertImage(Image image) {
            return (byte[])new ImageConverter().ConvertTo(image, typeof(byte[]));
        }
    }
}
