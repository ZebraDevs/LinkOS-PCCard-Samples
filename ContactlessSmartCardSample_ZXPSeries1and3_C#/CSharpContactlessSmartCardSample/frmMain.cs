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
File: frmMain.cs
Description: Form allowing users to select printing and encoding options.
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/

using System;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.Collections.Generic;

namespace CSharpContactlessSmartCardSample
{
    public partial class frmMain : Form
    {
        #region Local Variables

        // Versions -----------------------------------------------------------------------------------------

        private string _graphicsSDKVersion, 
                        _prnSDKVersion;
        #endregion


        ZBRPrinter _thePrinterSDK = null;
        SampleCodeContactless Contactless = null;

        #region Local Functions

      
        // Configures the Form based on present dlls --------------------------------------------------------
        private void FormConfig()
        {
            string msg = "";
            try
            {
                this.lblStatus.Text = "";

                // Printing (Graphics)
                if (_graphicsSDKVersion != "")
                {
                    this.gbPrint.Enabled = true;
                    this.gbSmartCard.Enabled = true;
                    msg = "Graphics: " + _graphicsSDKVersion + "; ";
                }

                // Magnetic Encoding (Print)
                if (_prnSDKVersion != "")
                {
                    this.gbMag.Enabled = true;
                    this.gbSmartCard.Enabled = true;
                    msg += "Printer: " + _prnSDKVersion + "; ";
                }

 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FormConfig threw exception");
            }
            finally
            {
                this.lblVersions.Text = msg;
            }
        }

        // Gets the versions of the SDK's DLLs
        //     if the version == "" then the supporting dll is not present ----------------------------------
        private void GetSDKVersions()
        {
            SampleCodeGraphics g; 
            SampleCodeMag p;
            try
            {
                g = new SampleCodeGraphics();
                _graphicsSDKVersion = g.GetSDKVersion();
                 
                p = new SampleCodeMag();
                _prnSDKVersion = p.GetSDKVersion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GetSDKVersions threw exception");
            }
            finally
            {
                g = null; 
                p = null;
            }
        }
         
        #endregion

        #region Form

        // Form Initialization ------------------------------------------------------------------------------

        public frmMain()
        {
            InitializeComponent();
        }

        // Form Load Event ----------------------------------------------------------------------------------

        private void frmMain_Load(object sender, EventArgs e)
        {
            _thePrinterSDK = new ZBRPrinter();
            LocatePrinters();
            cboPrn.Focus();
            lblStatus.Text = "";
            //GetSDKVersions();
            //FormConfig();
        }


        #endregion

        #region Buttons

        // Exits the Application ----------------------------------------------------------------------------

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Submit Button
        //     Starts the example code based on Form selections ---------------------------------------------

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region Variables

            bool eject = false;
            string msg = "";

            SampleCodeMag mag = null;
            SampleCodeGraphics prn = null; 

            #endregion

            #region Check Selections

            // Verifies that a printer has been selected
            try
            {
                if (cboPrn.SelectedIndex < 0)
                {
                    msg = "Error: A Printer has not been selected";
                    return;
                }

                // Verifies that at least one selection is made
                if (!this.cbBack.Checked && !this.cbFront.Checked && !this.cbMag.Checked && !this.cbContactless.Checked)
                {
                    msg = "Error: No Selections";
                    return;
                }

            #endregion



            #region Smart Cards 
            {


                // the eject variable is set to true if magnetic encoding is not selected
                //  and neither Front or Back is selected

                eject = (!this.cbMag.Checked && !this.cbBack.Checked && !this.cbFront.Checked);

                // Determines which smart card sample code to run

                if (this.cbContactless.Checked)
                {
                    Contactless = new SampleCodeContactless();
                    string _ContactlessReader = "";
                    string _ContactReader = "";
                    string errorMsg = "";

                    WinSCard.GetPCSCReaders(out _ContactlessReader, out _ContactReader, out errorMsg);
                    if (_ContactlessReader != null && _ContactlessReader.Length > 0)
                    {
                        Contactless.ContactlessEncode(this.cboPrn.Text, _ContactlessReader, Convert.ToInt16(eject), out msg);
                        if (msg == "")
                            this.lblStatus.Text = "Contactless : No Errors";
                        else
                        {
                            this.lblStatus.Text = msg;
                        }
                    }
                    else

                        this.lblStatus.Text = "Error: No Contactless Reader found";
                }
            }

            #endregion

            #region Magnetic Encoding

            if (cbMag.Checked)
            {
                // the eject variable is set to true if neither Front or Back is selected

                eject = (!this.cbBack.Checked && !this.cbFront.Checked);

                // Encodes and Verifies all three Tracks

                mag = new SampleCodeMag();
                mag.PerformMagneticEncodeJob(this.cboPrn.Text,  out msg);
                if (msg != "")
                    return;
                this.lblStatus.Text = "Magnetic Encoding : No Errors";
            }

            #endregion

            #region Printing

            // Initialize the Print Side Class

            prn = new SampleCodeGraphics();

            // Determines the printing type

            if (this.cbFront.Checked && !this.cbBack.Checked)
            {
                prn.PrintFrontSideOnly(this.cboPrn.Text, "Front Side Text", Application.StartupPath, out msg);
                if (msg == "") this.lblStatus.Text = "No Errors : Front Side Only Printing";
            }
            else if (this.cbFront.Checked && this.cbBack.Checked)
            {
                prn.PrintBothSides(this.cboPrn.Text, "Front Side Text", "Back Side Text", Application.StartupPath, out msg);
                if (msg == "") this.lblStatus.Text = "No Errors : Both Side Printing";
            }
        }
        catch (Exception ex)
        {
            msg += ex.Message;
            MessageBox.Show(ex.ToString(), "btnSubmit_Click threw exception");
        }
        finally
        {
            if (msg != "")
                this.lblStatus.Text = msg;

            mag = null;
            prn = null; 
        }

            #endregion


        }

        #endregion

        #region Check Boxes

        private void cbBack_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbBack.Checked)
                this.cbFront.Checked = true;
        }

        private void cbFront_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.cbFront.Checked)
                this.cbBack.Checked = false;
        }

        #endregion


        #region Printer Setup

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
        private void ConfigurePrinter()
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
        private void DecipherConfigurationCode(ref byte[] options)
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


                cbMag.Enabled = false; 
                if (MagOption == 'M') //magnetic encoder
                {
                    cbMag.Enabled = true;
                    gbMag.Enabled = true;
                }

                cbFront.Enabled = true;
                gbPrint.Enabled = true;
                if (sides == '2')
                    cbBack.Enabled = true;
                else
                    cbBack.Enabled = false;


                if (SCOption == 'E' || SCOption == 'A') //internal or external 
                { 
                    // Contactless Readers
                    cbContactless.Enabled = true;
                    gbSmartCard.Enabled = true;
                }
                
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
        private bool LocatePrinters()
        {
            Cursor.Current = Cursors.WaitCursor;
            List<string> printers = null;
            try
            {
                printers = new List<string>();

                
                LocateUSBPrinters(ref printers);
            
                LocateEthernetPrinters(ref printers);
                

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


        #endregion //Locate Printers 

       
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
 
                  ConfigurePrinter(); 
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



        #endregion //Configure application for selected printer

    }
}