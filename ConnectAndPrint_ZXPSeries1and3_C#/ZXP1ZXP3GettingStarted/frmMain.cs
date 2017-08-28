/**************************************************************************************************
 * Name space: ZXP1ZXP3GettingStarted
 * 
 * Purpose: C# Sample Application 
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
 * 04/25/2011        JL             File creation.
 * 08/13/2015		 SP				Modified for single and dual-sided printing
 ***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using nsZBRPrinter;         //printer SDK
using nsZBRGraphics;        //graphics SDK

namespace ZXP1ZXP3GettingStarted
{
    public partial class frmMain : Form
    {
        #region Local Variables

        private ZBRPrinter _thePrinterSDK = null;
        private ZBRGraphics _theGraphicsSDK = null;

        private bool _dualSided = false;

        #endregion //Local Variables

        /**************************************************************************************************
        * Function Name: frmMain
        * 
        * Purpose: Class constructor 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        public frmMain()
        {
            InitializeComponent();

            _thePrinterSDK = new ZBRPrinter();
            _theGraphicsSDK = new ZBRGraphics();
        }

        /**************************************************************************************************
        * Function Name: frmMain_Load
        * 
        * Purpose: form load initialization 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
 		* 08/13/2015		SP			   Removed unused function call
        ***************************************************************************************************/
        private void frmMain_Load(object sender, EventArgs e)
        {
            RefreshUSB();
        }

        #region Print Button event handler

        /**************************************************************************************************
        * Function Name: btnPrint_Click
        * 
        * Purpose: Event handler for start print job button click event, determines what type of job(s) to
        *          send to the printer based upon user selection(s). 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
 		* 08/13/2015		SP			   Removed unused function calls
        ***************************************************************************************************/
        private void btnPrint_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            string cardType = string.Empty;

            try
            {
                Cursor = Cursors.WaitCursor;

                lblPrintStatus.Text = string.Empty;

                //Perform print job 
                PerformPrintJob();
            }
            catch (Exception ex)
            {
                msg += ex.Message;
                MessageBox.Show(ex.ToString(), "btnPrint_Click threw exception");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }
        #endregion //Command Button event handlers

        /**************************************************************************************************
        * Function Name: cboPrn_SelectedIndexChanged
        * 
        * Purpose: Event handler for installed printers combo box selection event. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *   
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
 		* 08/13/2015		SP			   Check to see if printer supports dual-sided printing
        ***************************************************************************************************/
        private void cboPrinterSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                OpenConnectionToPrinter();
                ConfigureApp(cboPrinterSelection.Text);
                CloseConnectionToPrinter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "cboPrn_SelectedIndexChanged threw exception");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        #region Printer Setup

        /**************************************************************************************************
        * Function Name: DecipherConfigurationCode
        * 
        * Purpose: Determine a printer's configuration from its configuration code. 
        * 
        * Parameters: options = string value containing the configuration code
        *                 usb = boolean value which defines the type of printer interface
        *                        True = usb-connected printer
        *                       False = ethernet-connected printer   
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        * 01/17/2011       ACT             Added enabling magnetic encoder checkbox if printer includes mag encoder.
 		* 08/13/2015		SP			   Removed unused variables
        ***************************************************************************************************/
        private void DecipherConfigurationCode(ref byte[] options)
        {
            string errMsg = string.Empty;
            string config = string.Empty;
            try
            {

                char sides = Convert.ToChar(options[4]);

                config = Convert.ToString(sides);

                btnPrint.Enabled = true;
                if (sides == '2')
                    _dualSided = true;
                else
                    _dualSided = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("DecipherConfigurationCode threw exception: " + ex.Message);
            }
        }

        /**************************************************************************************************
        * Function Name: ConfigureApp
        * 
        * Purpose: Configures the application's option based upon the selected printer's configuration. 
        * 
        * Parameters: deviceName = string value containing the selected printer's name
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ConfigureApp(string deviceName)
        {
            byte[] options = null;
            try
            {
                string errMsg = string.Empty;

                options = new byte[50];

                _thePrinterSDK.GetPrinterConfiguration(options, out errMsg);
                if (errMsg == "")
                {
                    DecipherConfigurationCode(ref options);

                }
                else MessageBox.Show("ConfigureApp failed: " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConfigureApp threw exception: " + ex.Message);
            }
            finally
            {
                options = null;
            }
        }

        #endregion

        #region Locate Printers

        /**************************************************************************************************
        * Function Name: LocatePrinters
        * 
        * Purpose: Determines how to search for installed printer(s): via usb or ethernet, 
        *           initialize printer combo box with installed printer name(s). 
        * 
        * Parameters: usbConnection = boolean which defines type of search to conduct.
        *                             True = usb search
        *                            False = ethernet search 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private bool LocatePrinters()
        {
            Cursor.Current = Cursors.WaitCursor;
            List<string> printers = null;
            try
            {
                printers = new List<string>();
                LocateUSBPrinters(ref printers);
                cboPrinterSelectionInit(ref printers);
                if (cboPrinterSelection.Items.Count > 0)
                {
                    cboPrinterSelection.Enabled = true;
                    return true;
                }
                else
                    cboPrinterSelection.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocatePrinters threw exception:" + ex.Message);
            }
            finally
            {
                if (printers != null)
                    printers.Clear();
                printers = null;
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: LocateUSBPrinters
        * 
        * Purpose: Determines how to search for installed printer(s) via usb, 
        *           initialize printer list control with installed printer name(s). 
        * 
        * Parameters: printers = string list to hold names of installed printers 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private bool LocateUSBPrinters(ref List<string> printers)
        {
            try
            {
                foreach (String strPrn in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    string name = strPrn.ToUpper();

                    if (name.Contains("ZEBRA"))
                        if ((name.Contains("ZXP SERIES 3") && !name.Contains("NETWORK"))
                            || (name.Contains("ZXP S3") && !name.Contains("ZXP S3 NETWORK")))
                            printers.Add(strPrn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocateUSBPrinters threw exception:" + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: cboPrinterSelectionInit
        * 
        * Purpose: Initialize printer combo box with installed printer name(s). 
        * 
        * Parameters: printers = string list of installed printer name(s) 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private void cboPrinterSelectionInit(ref List<string> printers)
        {
            try
            {
                cboPrinterSelection.Text = "";
                cboPrinterSelection.Items.Clear();
                cboPrinterSelection.Refresh();

                foreach (string printer in printers)
                {
                    cboPrinterSelection.Items.Add(printer);
                }

                cboPrinterSelection.Focus();
                cboPrinterSelection.SelectedIndex = -1;

                Refresh();
            }
            catch (System.Exception ex)
            {
                cboPrinterSelection.Items.Clear();
                MessageBox.Show("cboPrinterSelection threw exception: Could not locate printers on USB port: " + ex.Message);
            }
        }

        #endregion //Locate Printers

        #region Open/Close/Reset Printer Connection

        /**************************************************************************************************
        * Function Name: OpenConnectionToPrinter
        * 
        * Purpose: Gets a handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = Successfully got handle
        *          False = Failed to get a handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 08/13/2011        SP             Function creation.
        ***************************************************************************************************/
        private bool OpenConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                _thePrinterSDK.Open(cboPrinterSelection.Text, out errMsg);

                if (errMsg == string.Empty)
                    return true;
                else
                    MessageBox.Show("Unable to open device [" + cboPrinterSelection.Text + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OpenConnectionToPrinter threw exception: " + ex.Message);
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
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private bool CloseConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                _thePrinterSDK.Close(out errMsg);

                if (errMsg == string.Empty)
                    return true;
                else
                    MessageBox.Show("Unable to close device [" + cboPrinterSelection.Text + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CloseConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }

        #endregion //Open/Close/Reset Printer Connection

        #region Print Spooler Status

        /**************************************************************************************************
        * Function Name: IsPrinterReady
        * 
        * Purpose: Checks the printer spooler to determine if it contains print jobs for the selected printer
        *           for a pre-determined length of time. If spooler contains no jobs for the selected printer, 
        *           printer is ready to accept a new job.
        * 
        * Parameters: drvName = string containing name of selected printer
        *               seconds = integer containing the pre-determined length of time in seconds to check spooler
        *                errMsg = string containing error mesage is error occurs  
        *                 
        * Returns:  True = Print spooler has no print jobs for printer: printer is ready for new job
        *          False = Print spooler is still busy with jobs for selected printer 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private bool IsPrinterReady(string drvName, int seconds, out string errMsg)
        {
            errMsg = string.Empty;

            bool ready = false;
            try
            {
                for (int i = 0; i < seconds; i++)
                {
                    if (_theGraphicsSDK.IsPrinterReady(drvName, out errMsg) != 0)
                    {
                        if (errMsg == string.Empty)
                        {
                            ready = true;
                        }
                        break;
                    }
                    Thread.Sleep(1000);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                ready = false;
                MessageBox.Show(ex.ToString(), "IsPrinterReady exception");
            }
            return ready;
        }
        #endregion //Print Spooler Status

        #region Poll Current Job Status

        /**************************************************************************************************
        * Function Name: WaitForJobToComplete
        * 
        * Purpose: Checks the printer spooler to determine if it contains a print job for the selected printer,
        *           and checks the printer for any error codes. 
        *           If spooler contains no jobs for the selected printer, job has completed.
        * 
        * Parameters: status = string containing success message or error message 
        *                              
        * Returns:  None 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
 		* 08/13/2015		SP			   Removed unused function calls
        ***************************************************************************************************/
        private void WaitForJobToComplete(out string status)
        {
            status = string.Empty;
            try
            {
                while (true)
                {
                    bool ready = IsPrinterReady(cboPrinterSelection.Text, 1, out status);

                    if (!ready && status != string.Empty)
                    {
                        status = "Error " + status;
                        break;
                    }
                    else if (ready && status == string.Empty) //print job completed
                    {
                        status = "Success";
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                status = "WaitForJobToComplete exception: " + e.Message;
            }
        }
        #endregion //Poll Current Job Status

        #region USB / Ethernet Refresh

        /**************************************************************************************************
        * Function Name: RefreshUSB
        * 
        * Purpose: Resets UI controls and searches for usb-connected printers.
        * 
        * Parameters: None  
        *                 
        * Returns: None  
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        ***************************************************************************************************/
        private void RefreshUSB()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                cboPrinterSelection.Visible = true;
                cboPrinterSelection.Focus();

                LocatePrinters();
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        #endregion //USB / Ethernet Refresh

        #region Printing Example

        /**************************************************************************************************
        * Function Name: PerformPrintJob
        * 
        * Purpose: Wraps required functionality to print a card, monitor the printer's progress,
        *           and display job result.
        * 
        * Parameters: None  
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 04/25/2011        JL             Function creation.
        * 08/13/2015        SP             Modified for both single and dual-sided print jobs
        ***************************************************************************************************/
        private void PerformPrintJob()
        {
            try
            {
                string msg = string.Empty;

                lblPrintStatus.Text = "Performing Print Job";

                _theGraphicsSDK.PrintJob(cboPrinterSelection.Text, "Print Color: " + txtCardTextFront.Text, _dualSided, "Print Monochrome", out msg);

                if (msg == "")
                {
                    WaitForJobToComplete(out msg);
                    lblPrintStatus.Text = "Print Job: " + msg;
                }
                else
                    lblPrintStatus.Text = "Print Job: " + msg;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "PerformPrintJob exception");
            }
        }
        #endregion //Dual-Sided Printing Example
    }
}
