/**********************************************
* CONFIDENTIAL AND PROPRIETARY
*
* The source code and other information contained herein is the confidential and the exclusive property of
* ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
* This source code, and any other information contained herein, shall not be copied, reproduced, published,
* displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
* expressly permitted under such license agreement.
*
* Copyright ZIH Corp. 2011
*
* ALL RIGHTS RESERVED
***********************************************
File: SampleCodeDrawConfiguration.cs
Description: Example code used to store configuration for the data to be passed to the Graphics
$Revision: 1 $
$Date: 2011/08/15 $
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CSharpCardIDPrintApplicationSample
{
    public class SampleCodeDrawConfiguration
    { 
        public string StringLabelText { get; set; } // The text to be printed
        public Point LabelLocation { get; set; }  // The location on the card

        public byte[] ImageData { get; set; } // The image byte array
        public Rectangle ImageLocationRect { get; set; } // the image location 

        public byte[] SignatureImageData { get; set; } // Signature image byte array 
        public Rectangle SignatureImageRect { get; set; } // the signature image location
 
        public byte[] BackgroundImageData { get; set; } // Background image data 
        public Rectangle BackgroundImageRect { get; set; } // Background location 
    }
}
