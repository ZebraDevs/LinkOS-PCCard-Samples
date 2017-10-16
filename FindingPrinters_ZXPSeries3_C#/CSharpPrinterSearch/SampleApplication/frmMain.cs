/**************************************************************************************************
 * Name space: SampleApplication
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
 * 12/8/2010        ACT             File creation.
 * 12/8/2010        ACT             Added UI Controls & Event Handlers
 * 12/9/2010        ACT             Added functions for locating USB/Ethernet printers
 * 12/9/2010        ACT             Added functions for single & dual sided printing 
 * 12/10/2010       ACT             Added functions for USB-connected smart card encoding
 * 12/10/2010       ACT             Began adding support for Ethernet-connected smart card encoding
 * 12/14/2010       ACT             Finished adding support for Ethernet-connected smart card encoding
 * 01/28/2011       ACT             Updated alll smartcard encoding over ethernet namespace/object declarations.
 ***************************************************************************************************/

using System;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.Collections.Generic;

using nsZBRPrinter;         //printer SDK
using nsZBRGraphics;        //graphics SDK

namespace SampleApplication
{
    public partial class frmMain : Form
    {
        #region Constants

        private const int FONT_BOLD = 0x01;
        private const int FONT_ITALIC = 0x02;
        private const int FONT_UNDERLINE = 0x04;
        private const int FONT_STRIKETHRU = 0x08;

        private const int CONTACT_CARD = 0x01;
        private const int CONTACTLESS_CARD = 0x02;

        #endregion //Constants

        #region Local Variables

        private string  _graphicsSDKVersion = string.Empty;
        private string _prnSDKVersion = string.Empty;

        private bool _dualSided = false;

        private ZBRPrinter _thePrinterSDK = null;
        private ZBRGraphics _theGraphicsSDK = null;
                
        #endregion //Variables

        #region Static Variables

        private static string _PrinterIpAddress = string.Empty;

        #endregion //Static Variables

        #region Form Properties
        
        #endregion

        #region Form functions

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
        * 12/8/2010        ACT             Function creation.
        * 12/9/2010        ACT             Added initialization of printer & graphics class objects
        * 01/28/2011       ACT             Renamed Smartcard encoding framework object to _SCEncodeEthernet
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
        * 12/8/2010        ACT             Function creation.
        * 12/9/2010        ACT             Added retrieving printer & graphics SDK versions
        * 12/9/2010        ACT             Added call to FormConfig
        * 12/9/2010        ACT             Added call to InitializeDelegates
        ***************************************************************************************************/
        private void frmMain_Load(object sender, EventArgs e)
        {
            rbUSB.Checked = false;
            rbEthernet.Checked = false;

            cboPrn.Focus();

            //GetSDKVersions();

            //FormConfig();
        }

        /**************************************************************************************************
        * Function Name: FormConfig
        * 
        * Purpose: Initialize select UI controls & setup form title display 
        * 
        * Parameters: None
        * 
        * Returns: None
        *   
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void FormConfig()
        {
            string msg = string.Empty;
            try
            {
                lblStatus.Text = string.Empty;

                if (_prnSDKVersion != "")
                {
                    msg = "Printer: " + _prnSDKVersion + "; ";
                }

                if (_graphicsSDKVersion != "")
                {
                    msg += "Graphics: " + _graphicsSDKVersion + "; ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FormConfig threw exception");
            }
            finally
            {
                lblVersions.Text = msg;
            }
        }

        /**************************************************************************************************
        * Function Name: RefreshForm
        * 
        * Purpose: Refresh form & UI controls 
        * 
        * Parameters: None
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void RefreshForm()
        {
            Refresh();
            Application.DoEvents();
        }
        #endregion //Form functions

        #region Application Exit

        /**************************************************************************************************
        * Function Name: btnExit_Click
        * 
        * Purpose: Event handler for Exit button click event, program cleanup, & signal application to exit. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *   
        * History:
        * Date             Who             Comment
        * 12/8/2010        ACT             Function creation.
        * 01/28/2011       ACT             Renamed Smartcard encoding framework object to _SCEncodeEthernet
        ***************************************************************************************************/
        private void btnExit_Click(object sender, EventArgs e)
        {
            _thePrinterSDK = null;
            _theGraphicsSDK = null;
            
            Dispose(true); 

            Application.Exit();
        }
        #endregion //Application Exit

        #region Checkbox event handlers
        private void cbMagEncode_CheckedChanged(object sender, EventArgs e)
        {
            CanEnableRunButton();
        }
        #endregion //Checkbox event handlers

        #region Radio Button event handlers
        /**************************************************************************************************
        * Function Name: rbUSB_CheckedChanged
        * 
        * Purpose: Event handler for USB radio button selection/de-selection event, 
        *          reset selected UI controls, & locate USB-connected printers. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *   
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void rbUSB_CheckedChanged(object sender, EventArgs e)
        {
            ResetPrintControls();

            if (rbUSB.Checked)
            {
                RefreshUSB();

                CanEnableRunButton();
            }
        }

        /**************************************************************************************************
        * Function Name: rbEthernet_CheckedChanged
        * 
        * Purpose: Event handler for Ethernet radio button selection/de-selection event, 
        *          reset selected UI controls, & locate Ethernet-connected printers. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void rbEthernet_CheckedChanged(object sender, EventArgs e)
        {
            ResetPrintControls();

            if (rbEthernet.Checked)
            {
                RefreshEthernet();

                CanEnableRunButton();
            }
        }

        /**************************************************************************************************
        * Function Name: rbSingleSide_CheckedChanged
        * 
        * Purpose: Event handler for Single-Sided Print Job radio button selection/de-selection event. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/09/2010       ACT             Function creation.
        ***************************************************************************************************/
        private void rbSingleSide_CheckedChanged(object sender, EventArgs e)
        {
            CanEnableRunButton();
        }

        /**************************************************************************************************
        * Function Name: rbDualSide_CheckedChanged
        * 
        * Purpose: Event handler for Dual-Sided Print Job radio button selection/de-selection event. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/09/2010       ACT             Function creation.
        ***************************************************************************************************/
        private void rbDualSided_CheckedChanged(object sender, EventArgs e)
        {
            CanEnableRunButton();
        }
        #endregion //Radio Buttons event handlers

        #region Combobox event handlers

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
        * 12/8/2010        ACT             Function creation.
        * 01/17/2011       ACT             Added call to function ResetMagneticEncoderControls(). 
        ***************************************************************************************************/
        private void cboPrn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                ResetPrintControls();

                RefreshForm();

                if (rbUSB.Checked)
                {
                    ConfigureForUsbPrinter();
                }
                else if (rbEthernet.Checked)
                {
                    ConfigureForEthernetPrinter();
                }

                CanEnableRunButton();
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

        /**************************************************************************************************
        * Function Name: cboCardTypes_SelectedIndexChanged
        * 
        * Purpose: Event handler for smart card types combo box selection event. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/8/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void cboCardTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CanEnableRunButton();
        }
        #endregion //Combobox event handlers

        #region Command Button event handlers

        /**************************************************************************************************
        * Function Name: btnRun_Click
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
        * 12/8/2010        ACT             Function creation.
        * 01/28/2011       ACT             Renamed Smartcard encoding framework object to SCEncodeEthernet
        ***************************************************************************************************/
        private void btnRun_Click(object sender, EventArgs e)
        {
            bool eject = false;
            
            string msg = string.Empty;
            string cardType = string.Empty;
            try
            {
                Cursor = Cursors.WaitCursor;

                lblStatus.Text = string.Empty;

                //Perform the selected print job 
                if (rbSingleSide.Checked)
                {
                    PerformSingleSidePrintJob();
                }
                else if (rbDualSided.Checked)
                {
                    PerformDualSidePrintJob();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
                MessageBox.Show(ex.ToString(), "btnSubmit_Click threw exception");
            }
            finally
            {
                if (eject)
                    EjectCard();

                if (rbEthernet.Checked)
                {
                    string temp = string.Empty;
                    msg += " " + temp;
                }

                CloseConnectionToPrinter();

                Cursor = Cursors.Arrow;
            }
        }
        #endregion //Command Button event handlers

        #region UI Controls Enable/Disable

        /**************************************************************************************************
        * Function Name: ResetPrintControls
        * 
        * Purpose: Returns printer-related UI controls to their initialized state. 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ResetPrintControls()
        {
            rbDualSided.Enabled = false;
            rbSingleSide.Enabled = false;

            lblStatus.Text = string.Empty;
        }

        /**************************************************************************************************
        * Function Name: CanEnableRunButton
        * 
        * Purpose: Determines whether or not to enable Start Printer Job button based upon 
        *           printer and smartcard-related UI control selections. 
        * 
        * Parameters: None
        * 
        * Returns: None
        *  
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        * 01/17/2011       ACT             Added checking state of cbMagEncode control.
        ***************************************************************************************************/
        private void CanEnableRunButton()
        {
            try
            {
                btnRun.Enabled = false;

                if (rbUSB.Checked || rbEthernet.Checked)
                {
                    if (cboPrn.Text.Length > 0)
                    {
                        if(rbSingleSide.Checked || rbDualSided.Checked)
                        {
                            btnRun.Enabled = true; //print job defined
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "CanEnableRunButton threw exception");
            }
        }

        #endregion //UI Controls Enable/Disable

        #region SDK Version Info

        /**************************************************************************************************
        * Function Name: GetSDKVersions
        * 
        * Purpose: Retrieves the versions of the Printer and Graphics SDKs. 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/8/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void GetSDKVersions()
        {
            try
            {
                _graphicsSDKVersion = _theGraphicsSDK.GetGraphicsSDKVersion();

                _prnSDKVersion = _thePrinterSDK.GetPrinterSDKVersion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GetSDKVersions threw exception");
            }
        }
        #endregion SDK Version Info

        #region Locate Printers

        /**************************************************************************************************
        * Function Name: GetPrinterDesignation
        * 
        * Purpose: Retrieve the name of the currently selected printer. 
        * 
        * Parameters: None
        * 
        * Returns: String value containing printer name
        *  
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private String GetPrinterDesignation()
        {
            string temp = cboPrn.Text;
            if (temp.Contains(","))
            {
                int index = temp.IndexOf(",");
                temp = temp.Substring(index + 1);
            }
            temp = temp.Trim();
            return temp;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterDesignation
        * 
        * Purpose: Retrieve the name of the currently selected printer. 
        * 
        * Parameters: deviceName = string to hold printer name 
        * 
        * Returns:  True = Printer name retrieved successfully
        *          False = failed to retrieve printer name 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool GetPrinterDesignation(ref string deviceName)
        {
            deviceName = GetPrinterDesignation();
            if (deviceName.Length == 0)
            {
                MessageBox.Show("Printer ID cannot be blank");
                return false;
            }
            return true;
        }

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
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool LocatePrinters(bool usbConnection)
        {
            Cursor.Current = Cursors.WaitCursor;
            List<string> printers = null;
            try
            {
                printers = new List<string>();

                if (usbConnection)
                {
                    LocateUSBPrinters(ref printers);
                }
                else //Ethernet
                {
                    LocateEthernetPrinters(ref printers);
                }

                cboPrnInit(ref printers);
                if (cboPrn.Items.Count > 0)
                {
                    cboPrn.Enabled = true;
                    return true;
                }
                else
                    cboPrn.Enabled = false;
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
        * 12/9/2010        ACT             Function creation.
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
        * Function Name: LocateEthernetPrinters
        * 
        * Purpose: Determines how to search for installed printer(s) via ethernet, 
        *           initialize printer list control with installed printer name(s). 
        * 
        * Parameters: printers = string list to hold names of installed printers 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool LocateEthernetPrinters(ref List<string> printers)
        {
            try
            {
                foreach (String strPrn in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    string name = strPrn.ToUpper();

                    if (name.Contains("ZEBRA"))
                        if (name.Contains("ZXP SERIES 3 NETWORK")
                            || name.Contains("ZXP S3 NETWORK"))
                            printers.Add(strPrn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocateEthernetPrinters threw exception:" + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: cboPrnInit
        * 
        * Purpose: Initialize printer combo box with installed printer name(s). 
        * 
        * Parameters: printers = string list of installed printer name(s) 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void cboPrnInit(ref List<string> printers)
        {
            try
            {
                cboPrn.Text = "";
                cboPrn.Items.Clear();
                cboPrn.Refresh();

                foreach (string printer in printers)
                {
                    cboPrn.Items.Add(printer);
                }

                cboPrn.Focus();
                cboPrn.SelectedIndex = -1;

                Refresh();
            }
            catch (System.Exception ex)
            {
                cboPrn.Items.Clear();
                MessageBox.Show("cboPrnInit threw exception: Could not locate printers on USB port: " + ex.Message);
            }
        }

        #endregion //Locate Printers 

        #region Configure application for selected printer

        /**************************************************************************************************
        * Function Name: ConfigureForUsbPrinter
        * 
        * Purpose: Configures the program for a usb-connected printer. 
        * 
        * Parameters: None 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ConfigureForUsbPrinter()
        {
            try
            {
                if (!RefreshConnectionToPrinter())
                    return;

                string deviceName = cboPrn.Text;

                ConfigureApp(deviceName);

                rbDualSided.Enabled = _dualSided;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConfigureForUsbPrinter threw exception: " + ex.Message);
            }
            finally
            {
                CloseConnectionToPrinter();
            }
        }

        /**************************************************************************************************
        * Function Name: ConfigureForEthernetPrinter
        * 
        * Purpose: Configures the program for an ethernet-connected printer. 
        * 
        * Parameters: None 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ConfigureForEthernetPrinter()
        {
            try
            {
                string deviceName = string.Empty;
                if (!GetPrinterDesignation(ref deviceName))
                    return;

                if (!RefreshConnectionToPrinter())
                    return;

                cboPrn.Visible = true;

                ConfigureApp(deviceName);

                rbDualSided.Enabled = _dualSided;

                Refresh();
                Application.DoEvents();
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConfigureForEthernetPrinter threw exception: " + ex.Message);
            }
            finally
            {
                CloseConnectionToPrinter();
            }
        }

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
        ***************************************************************************************************/
        private void DecipherConfigurationCode(ref byte[] options, bool usb)
        {
            string errMsg = string.Empty;
            string config = string.Empty;
            try
            {
                char sides = Convert.ToChar(options[4]);
                char SCOption = Convert.ToChar(options[5]);
                char MagOption = Convert.ToChar(options[6]);
                char Interface = Convert.ToChar(options[8]);

                config = Convert.ToString(sides);

                if (sides == '2')
                    _dualSided = true;
                
                rbSingleSide.Enabled = true;
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
                    DecipherConfigurationCode(ref options, rbUSB.Checked);

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

        #endregion //Configure application for selected printer

        #region Open/Close/Reset Printer Connection

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
        private bool OpenConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                if (cboPrn.Text.Length <= 0)
                {
                    MessageBox.Show("No printer selected");
                    return false;
                }
                                
                _thePrinterSDK.Open(cboPrn.Text, out errMsg);

                if (errMsg == string.Empty)
                {
                    return true;
                }
                MessageBox.Show("Unable to open device [" + cboPrn.Text + "]. " + errMsg);
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
        private bool CloseConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                _thePrinterSDK.Close(out errMsg);

                if (errMsg == string.Empty)
                    return true;
                else
                    MessageBox.Show("Unable to close device [" + cboPrn.Text + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CloseConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: RefreshConnectionToPrinter
        * 
        * Purpose: Releases current handle and gets a new handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = New handle retrieved
        *          False = Failed to open a new handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool RefreshConnectionToPrinter()
        {
            try
            {
                CloseConnectionToPrinter();

                return OpenConnectionToPrinter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }

        #endregion //Open/Close/Reset Printer Connection

        #region Eject Card From Printer

        /**************************************************************************************************
        * Function Name: EjectCard
        * 
        * Purpose: Orders printer to remove card from printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  None  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void EjectCard()
        {
            try
            {
                int err = 0;

                _thePrinterSDK.EjectCard(out err);
                if (err > 0)
                {
                    MessageBox.Show("EjectCard failed: Error code = " + Convert.ToString(err));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EjectCard exception");
            }
        }

        #endregion //Eject Card From Printer

        #region Print Spooler Status
        
        /**************************************************************************************************
        * Function Name: WaitForSpoolerClear
        * 
        * Purpose: Waits pre-determined length of time for the printer spooler to become clear of jobs. 
        * 
        * Parameters: prnDriver = string containing name of selected printer
        *               seconds = integer containing the pre-determined length of time in seconds to wait  
        *                 
        * Returns:  True = Print spooler became clear during the pre-determined length of time
        *          False = Print spooler is still busy  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool WaitForSpoolerClear(string prnDriver, int seconds)
        {
            try
            {
                string errMsg = string.Empty;

                return _theGraphicsSDK.IsPrinterReady(prnDriver, seconds, out errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "WaitForSpoolerClear threw exception");
            }
            return false;
        }

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
        * 12/9/2010        ACT             Function creation.
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
        #endregion Print Spooler Status

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
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void WaitForJobToComplete(out string status)
        {
            status = string.Empty;
            try
            {
                int errValue = 0;
                
                while (true)
                {
                    bool ready = IsPrinterReady(cboPrn.Text, 1, out status);

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
        #endregion Poll Current Job Status

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
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void RefreshUSB()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                rbUSB.Checked = true;
                cboPrn.Visible = true;
                cboPrn.Focus();

                LocatePrinters(true); 
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        /**************************************************************************************************
        * Function Name: RefreshEthernet
        * 
        * Purpose: Resets UI controls and searches for ethernet-connected printers.
        * 
        * Parameters: None  
        *                 
        * Returns: None  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void RefreshEthernet()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                rbEthernet.Checked = true;
                cboPrn.Focus();

                LocatePrinters(false); 
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        #region Static methods
        /**************************************************************************************************
        * Function Name: GetPrinterIPAddress
        * 
        * Purpose: Retrieves the IP Address for the selected ethernet-connected printer., 
        *           
        * Parameters: driverName = string containing name of selected printer
        *                 IPAddr = string to hold the selected printer's IP Address  
        *                 
        * Returns: None   
        * 
        * History:
        * Date             Who             Comment
        * 12/10/2010       ACT             Function creation.
        * 12/29/2010       ACT             Added using SDK GetIPAddress function. 
        ***************************************************************************************************/
        private static void GetPrinterIPAddress(string driverName, ref string IPAddr)
        {
            ZBRPrinter prn = null;
            try
            {
                string errMsg = string.Empty;

                prn = new ZBRPrinter();
                prn.GetIPAddress(driverName, out IPAddr);
            }
            catch (Exception ex)
            {
                IPAddr = string.Empty;
                MessageBox.Show("GetPrinterIPAddress threw exception: " + ex.Message);
            }
            finally
            {
                prn = null;
            }
        }

        #endregion //Static methods

        #endregion //USB / Ethernet Refresh
     
        #region Single-Sided Printing Example

        /**************************************************************************************************
        * Function Name: PerformSingleSidePrintJob
        * 
        * Purpose: Wraps required functionality to print a single-sided card, monitor the printer's progress,
        *           and display job result.
        * 
        * Parameters: None  
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/10/2010       ACT             Function creation.
        ***************************************************************************************************/
        private void PerformSingleSidePrintJob()
        {
            try
            {
                string msg = string.Empty;

                lblStatus.Text = "Performing Single Side Print Job";
                Refresh();
                Application.DoEvents();

                _theGraphicsSDK.PrintSingleSideJob(cboPrn.Text, "Front Side Text", Application.StartupPath, out msg);

                if (msg == "")
                {
                    WaitForJobToComplete(out msg);
                    lblStatus.Text = "Single Side Print Job: " + msg;
                }
                else
                    lblStatus.Text = "Single Side Print Job";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "PerformSingleSidePrintJob exception");
            }
        }
        #endregion Single-Sided Printing Example 

        #region Dual-Sided Printing Example 

        /**************************************************************************************************
        * Function Name: PerformDualSidePrintJob
        * 
        * Purpose: Wraps required functionality to print a dual-sided card, monitor the printer's progress,
        *           and display job result.
        * 
        * Parameters: None  
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/10/2010       ACT             Function creation.
        ***************************************************************************************************/
        private void PerformDualSidePrintJob()
        {
            try
            {
                string msg = string.Empty;

                lblStatus.Text = "Performing Dual Side Print Job";

                _theGraphicsSDK.PrintDualSideJob(cboPrn.Text, "Front Side Text", "Back Side Text", Application.StartupPath, out msg);

                if (msg == "")
                {
                    WaitForJobToComplete(out msg);
                    lblStatus.Text = "Dual Side Print Job: " + msg;
                }
                else
                    lblStatus.Text = "Dual Side Print Job";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "PerformDualSidePrintJob exception");
            }
        }
        #endregion //Dual-Sided Printing Example 
   }
}