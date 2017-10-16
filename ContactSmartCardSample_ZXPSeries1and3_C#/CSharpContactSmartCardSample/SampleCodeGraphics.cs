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
File: SampleCodeGraphics.cs
Description: Example code showing how to print graphics and text to a card.
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/

using System;
using System.Text;
using System.Threading;

namespace CSharpContactSmartCardSample
{
    class SampleCodeGraphics
    {
        #region Declarations

        // Constants ----------------------------------------------------------------------------------------

        const int FONT_BOLD = 0x01;
        const int FONT_ITALIC = 0x02;
        const int FONT_UNDERLINE = 0x04;
        const int FONT_STRIKETHRU = 0x08;

        #endregion

         // Private Variables --------------------------------------------------------------------------------
        #region Declarations


        
        #endregion

        #region Constructor 

        // Class Initialization
        //     gets the ZBRGraphics.dll version -------------------------------------------------------------

        public SampleCodeGraphics()
        {
        }

        #endregion

        #region Get SDL DLL Version

        // Gets the DLL version for the SDK -----------------------------------------------------------------

        public string GetSDKVersion()
        {
            ZBRGraphics graphics = null; 

            string version = "";

            try
            {
                graphics = new ZBRGraphics();

                version =  graphics.GetGraphicsSDKVersion(); 
            }
            catch (Exception ex)
            {
                version = ex.ToString();
            }
            finally
            {
                graphics = null;
            }
            return version;
        }

        #endregion


        #region Printing Facade

        /**************************************************************************************************
        * Function Name: PrintDualSideJob
        * 
        * Purpose: Performs the necessary tasks to define a dual-sided print job, and send the job
        *           to the selected printer.
        * 
        * Parameters: drvName = string containing name of selected printer
        *           fronttext = string containing the text to be printed on front-side of card
        *            backtext = string containing the text to be printed on back-side of card
        *             imgPath = string containing the path to the location of the image file to be printed
        *                 msg = string containing an error message if an error occurs.  
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public void PrintBothSides(string drvName, string frontText, string backText,
                                     string imgPath, out string msg)
        {
            int errValue;   // value of 0 indicates no errors
            ZBRGraphics graphics = null; 

            msg = string.Empty;
            try
            {
                graphics = new ZBRGraphics();
                if (graphics.InitGraphics(graphics.AsciiEncoder.GetBytes(drvName), out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                int fontStyle = FONT_BOLD | FONT_ITALIC | FONT_UNDERLINE | FONT_STRIKETHRU;

                if (graphics.DrawText(35, 575, graphics.AsciiEncoder.GetBytes(frontText),
                    graphics.AsciiEncoder.GetBytes("Arial"), 12, fontStyle, 0xFF0000, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Draws a line into the Graphics Buffer
                if (graphics.DrawLine(35, 300, 300, 300, 0xFF0000, (float)5.0, out errValue) == 0)
                {
                    msg = "Printing DrawLine Error: " + errValue.ToString();
                    return;
                }

                // Places an Image from a File into the Graphics Buffer
                if (graphics.DrawImage(graphics.AsciiEncoder.GetBytes(imgPath + "\\images\\Zebra-Logo.png"), 30, 30, 200, 150, out errValue) == 0)
                {
                    msg = "Printing DrawImage Error: " + errValue.ToString();
                    return;
                }

                // Sends Barcode Data to the Monochrome Buffer
                if (graphics.DrawBarcode(35, 500, 0, 0, 1, 3, 30, 1, graphics.AsciiEncoder.GetBytes("123456789"), out errValue) == 0)
                {
                    msg = "Printing DrawBarcode Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Front Side)
                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }

                // Clears the Graphics Buffer
                if (graphics.ClearGraphics(out errValue) == 0)
                {
                    msg = "Printing ClearGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                if (graphics.DrawText(30, 575, graphics.AsciiEncoder.GetBytes(backText), graphics.AsciiEncoder.GetBytes("Arial"),
                    12, fontStyle, 0, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Back Side)
                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                msg += "PrintBothSides threw exception " + ex.ToString();
            }
            finally
            {
                // Starts the printing process and releases the Graphics Buffer
                if (graphics.CloseGraphics(out errValue) == 0)
                {
                    msg = "Printing CloseGraphics Error: " + errValue.ToString();
                }
            }
        }

        /**************************************************************************************************
        * Function Name: PrintSingleSideJob
        * 
        * Purpose: Performs the necessary tasks to define a single-sided print job, and send the job
        *           to the selected printer.
        * 
        * Parameters: drvName = string containing name of selected printer
        *                text = string containing the text to be printed
        *             imgPath = string containing the complete path to the image file to be printed
        *                 msg = string containing an error message if an error occurs.  
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public void PrintFrontSideOnly(string drvName, string text, string imgPath, out string msg)
        {
            int errValue;    // value of 0 indicates no errors
            ZBRGraphics graphics = new ZBRGraphics();
            IntPtr hDC = IntPtr.Zero;
            msg = string.Empty;
            try
            {
                if (graphics.InitGraphics(graphics.AsciiEncoder.GetBytes(drvName), out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                int fontStyle = FONT_BOLD | FONT_ITALIC | FONT_UNDERLINE | FONT_STRIKETHRU;

                if (graphics.DrawText(35, 575, graphics.AsciiEncoder.GetBytes(text), graphics.AsciiEncoder.GetBytes("Arial"), 12,
                             fontStyle, 0xFF0000, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Draws a line into the Graphics Buffer
                if (graphics.DrawLine(35, 300, 300, 300, 0xFF0000, (float)5.0, out errValue) == 0)
                {
                    msg = "Printing DrawLine Error: " + errValue.ToString();
                    return;
                }

                //// Places an Image from a file into the Graphics Buffer
                if (graphics.DrawImage(graphics.AsciiEncoder.GetBytes(imgPath + "\\images\\Zebra-Logo.png"), 30, 30, 200, 150, out errValue) == 0)
                {
                    msg = "Printing DrawImage Error: " + errValue.ToString();
                    return;
                }

                // Sends Barcode data to the Monochrome Buffer
                if (graphics.DrawBarcode(35, 500, 0, 0, 1, 3, 30, 1, graphics.AsciiEncoder.GetBytes("123456789"), out errValue) == 0)
                {
                    msg = "Printing DrawBarcode Error: " + errValue.ToString();
                    return;
                }

                // Prints data from the Graphics and Monochrome Buffers (Front Side)
                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                msg += "PrintFrontSideOnly threw exception " + ex.ToString();
            }
            finally
            {
                // Starts the printing process and releases the Graphics Buffer
                if (graphics.CloseGraphics(out errValue) == 0)
                {
                    msg = "Printing CloseGraphics Error: " + errValue.ToString();
                }
            }
        }
        #endregion //Printing Facade
    }
}
