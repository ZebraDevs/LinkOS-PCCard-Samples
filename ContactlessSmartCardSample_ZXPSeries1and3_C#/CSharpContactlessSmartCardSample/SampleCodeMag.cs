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
File: SampleCodeMag.cs
Description: Example code showing how to apply magnetic encoding.
$Revision: 1 $
$Date: 2011/8/10 $
*******************************************************************************/

using System;
using System.Text;
using System.Windows.Forms;

namespace CSharpContactlessSmartCardSample
{
    class SampleCodeMag
    {
        ZBRPrinter _thePrinterSDK = null;
        #region Constructor

        // Class Initialization 

        public SampleCodeMag()
        {
            _thePrinterSDK = new ZBRPrinter();
        }

        #endregion

        #region Get SDK Version

        // Gets the DLL version for the SDK -----------------------------------------------------------------
        public string GetSDKVersion()
        {
            ZBRPrinter prn = null; 

            string version = "";

            try
            {
                prn = new ZBRPrinter();

                version = prn.GetPrinterSDKVersion(); 
            }
            catch (Exception ex)
            {
                version = ex.ToString();
            }
            finally
            {
                prn = null;
            }
            return version;
        }

        #endregion

        #region Magnetic Example Code


        #region Magnetic Encoding Example
        /**************************************************************************************************
        * Function Name: PerformMagneticEncodeJob
        * 
        * Purpose: Wraps required functionality to magnetically encode a card and display job result.
        * 
        * Parameters: errMsg = string containing an error message if an error occurs   
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 01/17/2011       ACT             Function creation.
        ***************************************************************************************************/
        public void PerformMagneticEncodeJob(string deviceName, out string errMsg)
        {
            string track1 = "ABCDEFGHIJKLMNOPQRS";
            string track2 = "0123456789987654321";
            string track3 = "1122334455667788990";

            string outTrk1 = string.Empty;
            string outTrk2 = string.Empty;
            string outTrk3 = string.Empty;

            errMsg = string.Empty;
             
            Application.DoEvents();

            try
            {

                if (!OpenConnectionToPrinter(deviceName))
                {
                    errMsg = "failed to open connection to printer";
                    return;
                }

                // Encode tracks 1, 2, 3
                // If track data is ""; that track is not encoded
                int trkToWrite = 0;
                if (track1 != "") trkToWrite = 1;
                if (track2 != "") trkToWrite |= 2;
                if (track3 != "") trkToWrite |= 4;

                _thePrinterSDK.WriteMagneticTracks(trkToWrite, track1, track2, track3, out errMsg);

                if (errMsg != string.Empty)
                {
                    return;
                }

                // Read all three tracks
                int trkToRead = 7;

                _thePrinterSDK.ReadMagneticTracks(trkToRead, out outTrk1, out outTrk2,
                                                  out outTrk3, out errMsg);

                if (errMsg != string.Empty)
                {
                    return;
                }

                // Verify that data read back equals data encoded 
                if (track1 != outTrk1 || track2 != outTrk2 || track3 != outTrk3)
                {
                    errMsg = "Magnetic Encode Error: data read back does not equal data encoded.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "PerformMagneticEncodeJob threw exception");
            }
        }

        #endregion //Magnetic Encoding Example




        /**************************************************************************************************
        * Function Name: OpenConnectionToPrinter
        * 
        * Purpose: Gets a handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = Handle retrieved
        *          False = Failed to open a handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool OpenConnectionToPrinter(string deviceName)
        {
            try
            {
                string errMsg = string.Empty;

                if (deviceName.Length <= 0)
                {
                    MessageBox.Show("No printer selected");
                    return false;
                }

                _thePrinterSDK.Open(deviceName, out errMsg);

                if (errMsg == string.Empty)
                {
                    return true;
                }
                MessageBox.Show("Unable to open device [" + deviceName + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OPenConnectionToPrinter threw exception; Unable to open device: " + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: CloseConnectionToPrinter
        * 
        * Purpose: Releases a handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = Handle released
        *          False = Failed to release a handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool CloseConnectionToPrinter(string deviceName)
        {
            try
            {
                string errMsg = string.Empty;

                _thePrinterSDK.Close(out errMsg);

                if (errMsg == string.Empty)
                    return true;
                else
                    MessageBox.Show("Unable to close device [" + deviceName + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CloseConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }



        #endregion
    }
}
