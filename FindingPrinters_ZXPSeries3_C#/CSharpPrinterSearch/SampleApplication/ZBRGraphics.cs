/**************************************************************************************************
 * Name space: nsZBRGraphics
 * 
 * Purpose: Application Framework for ZBRGraphics SDK 
 * 
 * Printers Supported: ZXP 3 Series
 * 
 * Zebra Technologies LLC, Copyright (c) 2010-2011 All Rights Reserved
 * 
 * Redistribution and use in of this sample application in source and binary forms, 
 * with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of Zebra Technologies LLC nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY ZEBRA TECHNOLOGIES LLC "AS IS" AND ANY EXPRESS
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE ZEBRA TECHNOLOGIES LLC 
 * OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
 * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * File History:
 * Date             Who             Comment
 * 12/9/2010        ACT             File creation.
 * 12/14/2010       ACT             Added new version of function IsPrinterReady
 * 12/14/2010       ACT             Added new version of function InitGraphics
 **************************************************************************************************/

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace nsZBRGraphics
{
	public class ZBRGraphics
    {
        #region Constants
        
        private const int FONT_BOLD = 0x01;
        private const int FONT_ITALIC = 0x02;
        private const int FONT_UNDERLINE = 0x04;
        private const int FONT_STRIKETHRU = 0x08;
        #endregion //Constants 

        #region Private Variables

        private IntPtr _hDC;

        ASCIIEncoding _asciiEncoder = null;

        #endregion //Private variables

        #region Constructor/Destructor

        /**************************************************************************************************
        * Function Name: ZBRGraphics
        * 
        * Purpose: Class constructor 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public ZBRGraphics()
        {
            _asciiEncoder = new ASCIIEncoding();
        }

        /**************************************************************************************************
        * Function Name: ~ZBRGraphics
        * 
        * Purpose: Class Destructor 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        ~ZBRGraphics()
        {
            _asciiEncoder = null;
            _hDC = IntPtr.Zero;
        }

        #endregion //Constructor/Destructor

        #region ZBRGraphics DLLImports

        /**************************************************************************************************
        * Purpose: Importation of required ZBRGraphics.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Functions creation.
        ***************************************************************************************************/
        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIIsPrinterReady", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIIsPrinterReady(byte[] strPrinterName, out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIGetSDKVer", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern void ZBRGDIGetSDKVer(out int major, out int minor, out int engLevel);

        [DllImport(	"ZBRGraphics.dll",EntryPoint="ZBRGDIInitGraphics",CharSet=CharSet.Auto,
            SetLastError=true)]
        private static extern int ZBRGDIInitGraphics(byte[] strPrinterName, out IntPtr hDC, out int err);

		[DllImport(	"ZBRGraphics.dll",EntryPoint="ZBRGDICloseGraphics",CharSet=CharSet.Auto,
            SetLastError=true)]
        private static extern int ZBRGDICloseGraphics(IntPtr hDC, out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIClearGraphics", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIClearGraphics(out int err);
        
        [DllImport(	"ZBRGraphics.dll",EntryPoint="ZBRGDIPrintGraphics",CharSet=CharSet.Auto,
            SetLastError=true)]
        private static extern int ZBRGDIPrintGraphics(IntPtr hDC, out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIDrawText", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIDrawText(int x, int y, byte[] text, byte[] font, int fontSize, int fontStyle,
            int color, out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIDrawLine", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIDrawLine(int x1, int y1, int x2, int y2, int color, float thickness, 
                                         out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIDrawImageRect", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIDrawImageRect(byte[] fileName, int x, int y, int sizeX, int sizeY, out int err);

        [DllImport("ZBRGraphics.dll", EntryPoint = "ZBRGDIDrawBarCode", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRGDIDrawBarCode(int x, int y, int rotation, int barcodeType, int widthRatio,
            int mutiplier, int height, int textUnder, byte[] data, out int err);

        #endregion //ZBRGraphics DLLImports
                
        #region Print Spooler

        /**************************************************************************************************
        * Function Name: IsPrinterReady
        * 
        * Purpose: To create a public wrapper method for ZBRGDIIsPrinterReady API. API checks print spooler 
        *           for active print jobs for selected printer. 
        *           
        * 
        * Parameters:   drvName = byte array containing name of selected printer
        *              errValue = integer containing the error code if printer is in error mode  
        *                 
        * Returns: 1 = Print spooler has no print jobs for selected printer
        *          0 = Print spooler has print job(s) for selected printer
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int IsPrinterReady(byte[] drvName, out int errValue)
        {
            return ZBRGDIIsPrinterReady(drvName, out errValue);
        }

       /**************************************************************************************************
       * Function Name: IsPrinterReady
       * 
       * Purpose: To create a public wrapper method for ZBRGDIIsPrinterReady API. API checks print spooler 
       *           for active print jobs for selected printer. 
       *           
       * 
       * Parameters:   drvName = string containing name of selected printer
       *                errMsg = string containing error message if printer is in error mode  
       *                 
       * Returns: 1 = Print spooler has no print jobs for selected printer
       *          0 = Print spooler has print job(s) for selected printer
       *         -1 = Exception caught 
       * 
       * History:
       * Date             Who             Comment
       * 12/14/2010       ACT             Function creation.
       ***************************************************************************************************/
        public int IsPrinterReady(string drvName, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                int errValue = 0;

                int result = ZBRGDIIsPrinterReady(_asciiEncoder.GetBytes(drvName), out errValue);
                if (errValue > 0)
                    errMsg = "Error: " + Convert.ToString(errValue);

                return result;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return -1;
        }

       /**************************************************************************************************
       * Function Name: IsPrinterReady
       * 
       * Purpose: To check printer status for a pre-defined amount of time to determine 
       *            if printer is available for new print job. 
       *           
       * 
       * Parameters:   drvName = string containing name of selected printer
       *               seconds = number of seconds to check printer availability
       *                errMsg = string containing error message if printer is in error mode  
       *                 
       * Returns:  True = Print is available
       *          False = Print is unavailable or error occurred
       * 
       * History:
       * Date             Who             Comment
       * 12/9/2010        ACT             Function creation.
       ***************************************************************************************************/
        public bool IsPrinterReady(string drvName, int seconds, out string errMsg)
        {
            bool ready = false;
            int errValue = 0;

            errMsg = string.Empty;
            try
            {
                for (int i = 0; i < seconds; i++)
                {
                    if (IsPrinterReady(_asciiEncoder.GetBytes(drvName), out errValue) != 0)
                    {
                        ready = true;
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                ready = false;
                errMsg = "IsPrinterReady threw exception " + ex.Message;
            }
            return ready;
        }

        #endregion //Print Spooler

        #region SDK DLL Version

        /**************************************************************************************************
        * Function Name: GetSDKVer
        * 
        * Purpose: To create a public wrapper method for ZBRGDIGetSDKVer API. 
        *           API returns the ZBRGraphics.dll's version. 
        *           
        * 
        * Parameters:  major = integer containing the major build version number
        *              minor = integer containing the minor build version number
        *           engLevel = integer containing the engineering release level version number  
        *                 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void GetSDKVer (out int major, out int minor, out int engLevel)
        {
            ZBRGDIGetSDKVer(out major, out minor, out engLevel);
        }

        /**************************************************************************************************
        * Function Name: GetGraphicsSDKVersion
        * 
        * Purpose: To return the ZBRGraphics.dll's version. 
        *           
        * 
        * Parameters: None   
        *                 
        * Returns: Success = string containing the ZBRGraphics.dll's version
        *             Fail = error or exception message   
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public string GetGraphicsSDKVersion()
        {
            string version = string.Empty;

            try
            {
                int engLevel = 0;
                int major = 0;
                int minor = 0;

                GetSDKVer(out major, out minor, out engLevel);

                if ((major + minor + engLevel) > 0)
                    version = major.ToString() + "." + minor.ToString() + "." + engLevel.ToString();
                else
                    version = "Failed to retrieve Graphics SDK version";
            }
            catch (Exception ex)
            {
                version = "GetGraphicsSDKVersion threw exception " + ex.ToString();
            }
            return version;
        }

        #endregion //SDK DLL Version

        #region Graphics Buffer & Device Context Initialization & Management

        /**************************************************************************************************
        * Function Name: InitGraphics
        * 
        * Purpose: To create a public wrapper method for ZBRGDIInitGraphics API. 
        *           API initializes the graphics buffer, and creates a handle to a device context. 
        *           
        * 
        * Parameters:   drvName = byte array containing name of selected printer
        *              errValue = integer containing the error code if error occurs
        *                 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int InitGraphics(byte[] drvName, out int errValue)
		{
            return ZBRGDIInitGraphics(drvName, out _hDC, out errValue);
		}

        /**************************************************************************************************
        * Function Name: InitGraphics
        * 
        * Purpose: To create a public wrapper method for ZBRGDIInitGraphics API. 
        *           API initializes the graphics buffer, and creates a handle to a device context. 
        *           
        * 
        * Parameters:  major = integer containing the major build version number
        *              minor = integer containing the minor build version number
        *           engLevel = integer containing the engineering release level version number  
        *                 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/14/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int InitGraphics(string drvName, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                int errValue = 0;
                
                int result = ZBRGDIInitGraphics(_asciiEncoder.GetBytes(drvName), out _hDC, out errValue);
                
                if (errValue > 0)
                    errMsg = "Error: " + Convert.ToString(errValue);

                return result;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return -1;
        }

        /**************************************************************************************************
        * Function Name: PrintGraphics
        * 
        * Purpose: To create a public wrapper method for ZBRGDIPrintGraphics API. 
        *           API sends data in the graphics buffer to the printer. 
        *           
        * 
        * Parameters:  errValue = integer containing the error code if error occurs
        *                 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int PrintGraphics(out int errValue)
        {
            return ZBRGDIPrintGraphics(_hDC, out errValue);
        }

        /**************************************************************************************************
        * Function Name: CloseGraphics
        * 
        * Purpose: To create a public wrapper method for ZBRGDICloseGraphics API. 
        *           API releases the graphics buffer, and device context. 
        *           
        * 
        * Parameters:  errValue = integer containing the error code if error occurs
        *                 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int CloseGraphics(out int errValue)
		{
            return ZBRGDICloseGraphics(_hDC, out errValue);
		}

        /**************************************************************************************************
        * Function Name: ClearGraphics
        * 
        * Purpose: To create a public wrapper method for ZBRGDIClearGraphics API. 
        *           API clears the graphics buffer. 
        *           
        * 
        * Parameters:  errValue = integer containing the error code if error occurs
        *                 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int ClearGraphics(out int errValue)
        {
            return ZBRGDIClearGraphics(out errValue);
        }
        
        #endregion //Graphics Buffer & Device Context Initialization & Management

        #region Drawing Methods

        /**************************************************************************************************
        * Function Name: DrawText
        * 
        * Purpose: To create a public wrapper method for ZBRGDIDrawText API. 
        *           API draws text into the graphics buffer. 
        *           
        * 
        * Parameters:  x = integer containing x coordinate: top-left corner of text
        *              y = integer containing y coordinate: top-left corner of text 
        *           text = byte array containing text to print
        *           font = byte array containing font name
        *       fontSize = integer containing the font size
        *      fontStyle = integer containing the font style: multiple styles can be OR'd together to define the style 
        *                  0x01 = bold
        *                  0x02 = italic
        *                  0x04 = underline
        *                  0x08 = strikethrough 
        *      textColor = integer containing the text color: RGB value
        *      errValue = integer containing the error code if error occurs
        *                 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int DrawText(int x, int y, byte[] text, byte[] font, int fontSize, int fontStyle,
                            int textColor, out int errValue)
		{
            return ZBRGDIDrawText(x, y, text, font, fontSize, fontStyle, textColor, out errValue);
		}

        /**************************************************************************************************
        * Function Name: DrawLine
        * 
        * Purpose: To create a public wrapper method for ZBRGDIDrawLine API. 
        *           API draws a line into the graphics buffer. 
        *           
        * Parameters:  x1 = integer containing x coordinate of the starting position of the line in dots
        *              y1 = integer containing y coordinate of the starting position of the line in dots
        *              x2 = integer containing x coordinate of the ending position of the line in dots
        *              y2 = integer containing y coordinate of the ending position of the line in dots
        *           color = integer containing the line color: RGB value
        *       thickness = float containing the line thickness in dots
        *        errValue = integer containing the error code if error occurs
        *                 
        *       Note: printer uses 300 dots per inch
        * 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int DrawLine(int x1, int y1, int x2, int y2, int color, float thickness, out int errValue)
        {
            return ZBRGDIDrawLine(x1, y1, x2, y2, color, thickness, out errValue);
        }

        /**************************************************************************************************
        * Function Name: DrawImage
        * 
        * Purpose: To create a public wrapper method for ZBRGDIDrawImageRect API. 
        *           API draws an image into the graphics buffer within a rectangular region. 
        *           
        * Parameters:  filename = byte array containing the image to place in the graphics buffer
        *              x = integer containing x coordinate: top-left corner of rectangle
        *              y = integer containing y coordinate: top-left corner of rectangle
        *          width = integer containing the width of the rectangle in dots
        *         height = integer containing the height of the rectangle in dots
        *        errValue = integer containing the error code if error occurs
        *                 
        *       Note: printer uses 300 dots per inch
        *                  
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int DrawImage(byte[] filename, int x, int y, int width, int height, out int errValue) 
		{
            return ZBRGDIDrawImageRect(filename, x, y, width, height, out errValue);
		}

        /**************************************************************************************************
        * Function Name: DrawBarcode
        * 
        * Purpose: To create a public wrapper method for ZBRGDIDrawBarCode API. 
        *           API draws a barcode into the monochrome graphics buffer. 
        *           
        * Parameters:  x1 = integer containing x coordinate of the starting position of the barcode in dots
        *              y1 = integer containing y coordinate of the starting position of the barcode in dots
        * 
        *        rotation = integer containing the type of rotation for barcode:
        *                   0 = origin lower left no rotation
        *                   1 = origin lower left 90 degrees
        *                   2 = origin lower left 180 degrees
        *                   3 = origin lower left 270 degrees
        *                   4 = origin center no rotation
        *                   5 = origin center 90 degrees
        *                   6 = origin center 180 degrees
        *                   7 = origin center 270 degrees 
        * 
        *     barcodeType = integer containing the barcode type:
        *                   0 = code 39 (3 of 9 alphanumeric)
        *                   1 = 2/5 interleave (numeric, even, no count)
        *                   2 = 2/5 industrial (numeric, no check digit)
        *                   3 = EAN8 (numeric 12 digits encoded)
        *                   4 = EAN13 (numeric 12 digits encoded)
        *                   5 = UPC – A (numeric 12 digits encoded)
        *                   6 = reserved for MONARCH
        *                   7 = code 128 C w/o check digits (numeric only, even number printed)
        *                   8 = code 128 B w/o check digits (numeric)
        *                   107 = code 128 C with check digits (numeric only, even number printed)
        *                   108 = code 128 B with check digits (numeric)
        * 
        *      widthRatio = integer containing the width ratio of narrow to wide bars:
        *                     0 = narrow bar = 1 dot, wide bar = 2 dots
        *                     1 = narrow bar = 1 dot, wide bar = 3 dots
        *                     2 = narrow bar = 2 dots, wide bar = 5 dots
        *    
        *      multiplier = integer containing the barcode multiplier
        *          height = integer containing barcode height in dots
        * 
        *       textUnder = integer containing the underline barcode text instruction
        *                     0 = do not underline 
        *                     1 = underline  
        * 
        *     barcodeData = byte array containing the data for the barcode   
        
        *        errValue = integer containing the error code if error occurs
        *                 
        *       Note: printer uses 300 dots per inch
        * 
        * Returns: 1 = Success
        *          0 = Failure 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int DrawBarcode(int x, int y, int rotation, int barcodeType, int widthRatio, int multiplier,
                               int height, int textUnder, byte[] barcodeData, out int errValue)
        {
            return ZBRGDIDrawBarCode(x, y, rotation, barcodeType, widthRatio, multiplier, height, textUnder,
                                     barcodeData, out errValue);
        }

        #endregion //Drawing Methods

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
        * 12/10/2010       ACT             Function creation.
        ***************************************************************************************************/
        public void PrintDualSideJob(string drvName, string frontText, string backText,
                                     string imgPath, out string msg)
        {
            int errValue;   // value of 0 indicates no errors

            msg = string.Empty;
            try
            {
                if (InitGraphics(_asciiEncoder.GetBytes(drvName), out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                int fontStyle = FONT_BOLD | FONT_ITALIC | FONT_UNDERLINE | FONT_STRIKETHRU;

                if (DrawText(35, 575, _asciiEncoder.GetBytes(frontText), 
                    _asciiEncoder.GetBytes("Arial"), 12, fontStyle, 0xFF0000, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Draws a line into the Graphics Buffer
                if (DrawLine(35, 300, 300, 300, 0xFF0000, (float)5.0, out errValue) == 0)
                {
                    msg = "Printing DrawLine Error: " + errValue.ToString();
                    return;
                }

                // Places an Image from a File into the Graphics Buffer
                if (DrawImage(_asciiEncoder.GetBytes(imgPath + "\\Zebra.bmp"), 30, 30, 200, 150, out errValue) == 0)
                {
                    msg = "Printing DrawImage Error: " + errValue.ToString();
                    return;
                }

                // Sends Barcode Data to the Monochrome Buffer
                if (DrawBarcode(35, 500, 0, 0, 1, 3, 30, 1, _asciiEncoder.GetBytes("123456789"), out errValue) == 0)
                {
                    msg = "Printing DrawBarcode Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Front Side)
                if (PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }

                // Clears the Graphics Buffer
                if (ClearGraphics(out errValue) == 0)
                {
                    msg = "Printing ClearGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                if (DrawText(30, 575, _asciiEncoder.GetBytes(backText), _asciiEncoder.GetBytes("Arial"), 
                    12, fontStyle, 0, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Back Side)
                if (PrintGraphics(out errValue) == 0)
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
                if (CloseGraphics(out errValue) == 0)
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
        * 12/10/2010       ACT             Function creation.
        ***************************************************************************************************/
        public void PrintSingleSideJob(string drvName, string text, string imgPath, out string msg)
        {
            int errValue;    // value of 0 indicates no errors

            IntPtr hDC = IntPtr.Zero;
            msg = string.Empty;
            try
            {
                if (InitGraphics(_asciiEncoder.GetBytes(drvName), out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                int fontStyle = FONT_BOLD | FONT_ITALIC | FONT_UNDERLINE | FONT_STRIKETHRU;

                if (DrawText(35, 575, _asciiEncoder.GetBytes(text), _asciiEncoder.GetBytes("Arial"), 12, 
                             fontStyle, 0xFF0000, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Draws a line into the Graphics Buffer
                if (DrawLine(35, 300, 300, 300, 0xFF0000, (float)5.0, out errValue) == 0)
                {
                    msg = "Printing DrawLine Error: " + errValue.ToString();
                    return;
                }

                // Places an Image from a file into the Graphics Buffer
                if (DrawImage(_asciiEncoder.GetBytes(imgPath + "\\Zebra.bmp"), 30, 30, 200, 150, out errValue) == 0)
                {
                    msg = "Printing DrawImage Error: " + errValue.ToString();
                    return;
                }

                // Sends Barcode data to the Monochrome Buffer
                if (DrawBarcode(35, 500, 0, 0, 1, 3, 30, 1, _asciiEncoder.GetBytes("123456789"), out errValue) == 0)
                {
                    msg = "Printing DrawBarcode Error: " + errValue.ToString();
                    return;
                }

                // Prints data from the Graphics and Monochrome Buffers (Front Side)
                if (PrintGraphics(out errValue) == 0)
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
                if (CloseGraphics(out errValue) == 0)
                {
                    msg = "Printing CloseGraphics Error: " + errValue.ToString();
                }
            }
        }
        #endregion //Printing Facade
    }
}
