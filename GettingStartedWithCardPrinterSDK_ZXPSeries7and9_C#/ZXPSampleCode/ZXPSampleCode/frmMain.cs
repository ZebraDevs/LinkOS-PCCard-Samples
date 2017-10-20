using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// ZXP Namespaces

using ZMOTIFPRINTERLib;

namespace ZXPSampleCode
{
    public partial class frmMain : Form
    {
        #region Declarations

        // Declarations
        // --------------------------------------------------------------------------------------------------

        // Available samples

        private string[] _lbSamples = { "Print 1",      // image & shapes
                                        "Print 2",      // image with adjustments, saved to profile
                                        "Print 3",      // image adjusted by profile
                                        "Print 4",      // gray scale conversion
                                        "Print 5",      // image with transparency
                                        "Print 6",      // border
                                        "Print With Contactless Smart Card",// encode smart card/then print images
                                        "Print With Contact Smart Card",
                                        "Smart Card Process",
                                        "Multi Print",
                                        "Write Mag Data",
                                        "Read Mag Data",
                                        "Print With Magnetic Encoding",
                                        "Position Card",
                                        "Eject Card",
                                        "Reject Card"};
        
        // Local variables

        private string _cardTypes = string.Empty,
                       _samples   = string.Empty;

        // Array list containing all card types for a printer

        private ArrayList _cardTypeList = null;

        // Test type classification

        private enum SAMPLE_TYPE
        {
            PRINTING,
            MAG,
            SMART_CARD
        }
        #endregion

        private string _connectionType = string.Empty;
        public string ConnectionType
        {
            get
            {
                if (rbEthernet.Checked)
                        _connectionType = GetIPAddrr();
                else
                        _connectionType = cboPrn.Text;

                return _connectionType;
            }
        }

        public string GetIPAddrr()
        {
            string temp = cboPrn.Text;
            if (temp.Contains(","))
            {
                int index = temp.IndexOf(",");
                temp = temp.Substring(index + 1);
            }
            temp = temp.Trim();
            if (!string.IsNullOrEmpty(temp))
            {
                int index = temp.IndexOf(":");
                if (index >= 0)
                    temp = temp.Substring(0, index);
                temp = temp.Trim();
            }
            return temp;
        }

        #region Form

        // Form class initialization
        // --------------------------------------------------------------------------------------------------

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            ZXPSampleCode z = null;
            try
            {
                // Get a list of USB ZXP devices
                if (cboPrnInit(true))
                {
                    // Loads list box with samples
                    for (int i = 0; i < _lbSamples.Length; i++)
                        this.lbSamples.Items.Add(_lbSamples[i]);

                    this.lbSamples.Text = _lbSamples[0];

                    // Displays versions
                    z = new ZXPSampleCode(this.cboPrn.Text);
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit(SAMPLE_TYPE.PRINTING);
                    }
                    else
                        cboCardType.Enabled = false;
                }
                else //do not close the program
                {
                    MessageBox.Show("Could not locate a ZXP Printer on USB ports", "Warning");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmMain_Load threw exception: " + ex.Message);
            }
            finally
            {
                z = null;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cardTypeList != null)
                _cardTypeList.Clear();
            _cardTypeList = null;
        }

        #endregion

        #region Buttons

        private void btnConnectToPrinter_Click(object sender, EventArgs e)
        {
            ZXPSampleCode z = null;
            Job job = null;
            try
            {
                Cursor = Cursors.WaitCursor;

                lbStatus.Visible = true;
                lbStatus.Text = "Connecting to printer";
                lbSamples.Items.Clear();
                Refresh();
                Application.DoEvents();

                job = new Job();

                z = new ZXPSampleCode(this.cboPrn.Text);

                if (z.Connect(ref job))
                {
                    // Loads list box with samples
                    for (int i = 0; i < _lbSamples.Length; i++)
                        this.lbSamples.Items.Add(_lbSamples[i]);

                    this.lbSamples.Text = _lbSamples[0];

                    // Displays versions
                    
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit(SAMPLE_TYPE.PRINTING);
                    }
                    else
                        cboCardType.Enabled = false;
                }
                else 
                {
                    MessageBox.Show("Could not open connection to printer " + cboPrn.Text, "Warning");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("btnConnectToPrinter_Click exception: " + ex.Message);
            }
            finally
            {
                if(z != null)
                    z.Disconnect(ref job);
                z = null;
                lbStatus.Visible = false;
                Cursor = Cursors.Default;
            }
        }

        private void btnLocatePrinters_Click(object sender, EventArgs e)
        {
            ZXPSampleCode z = null;
            try
            {
                Cursor = Cursors.WaitCursor;

                lbStatus.Visible = true;
                lbJobStatus.Text = "Searching for printers";
                lbSamples.Items.Clear();
                Refresh();
                Application.DoEvents();

                // Get a list of ZXP devices
                if (cboPrnInit(rbUSB.Checked))
                {
                    // Loads list box with samples
                    for (int i = 0; i < _lbSamples.Length; i++)
                        this.lbSamples.Items.Add(_lbSamples[i]);

                    this.lbSamples.Text = _lbSamples[0];

                    // Displays versions
                    z = new ZXPSampleCode(this.cboPrn.Text);
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit(SAMPLE_TYPE.PRINTING);
                    }
                    else
                        cboCardType.Enabled = false;
                }
                else //do not close the program
                {
                    string temp = rbUSB.Checked ? " USB" : " Ethernet";
                    MessageBox.Show("Could not locate a ZXP Printer via " + temp, "Warning");
                 }
            }
            catch (Exception ex)
            {
                MessageBox.Show("btnLocatePrinters_Click exception: " + ex.Message);
            }
            finally
            {
                z = null;
                lbStatus.Visible = false;
                Cursor = Cursors.Default;
            }
        }

        // Exits the application
        // --------------------------------------------------------------------------------------------------

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Starts a sample routine
        // --------------------------------------------------------------------------------------------------

        private void btnRun_Click(object sender, EventArgs e)
        {
            ZXPSampleCode zxp = null;
            try
            {
                zxp = new ZXPSampleCode(this.cboPrn.Text);

                btnRun.Enabled = false;

                SetDestinations(ref zxp, this.cboSrc.Text, this.cboDest.Text);

                // Sets the card type
                //zxp.CardType = this.cboCardType.Text;

                lbStatus.Text = "Active Job: " + lbSamples.Text;
                lbStatus.Visible = true;
                Refresh();
                Application.DoEvents();

                // Runs the specified sample code
                if (lbSamples.Text.Contains("Smart Card"))
                {
                    if (IsValidIP(ConnectionType))
                        MessageBox.Show("Smartcard Encoding over Ethernet not supported by this application", "Warning");
                    else 
                        PerformSmartCardTest(ref zxp, lbSamples.Text);
                }
                else if (lbSamples.Text.Contains("Card"))
                {
                    PerformPositionCardTest(ref zxp, lbSamples.Text);
                }
                else if(lbSamples.Text.Contains("Multi"))
                {
                    PerformMultiPrintTest(ref zxp, cboPrn.Text);
                }
                else if(lbSamples.Text.Contains("Mag"))
                {
                    PerformMagneticPrintTest(ref zxp, lbSamples.Text);
                }
                else if(lbSamples.Text.Contains("Print")) 
                {
                    PerformPrintTest(ref zxp, lbSamples.Text, cboCardType.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("btnRun_Click threw exception: " + ex.Message);
            }
            finally
            {
                zxp = null;
                btnRun.Enabled = true;
                lbStatus.Visible = false;
                Refresh();
                Application.DoEvents();
            }
        }
        #endregion

        #region Combo Boxes

        // Initializes cboCardType combo box
        //     from the _cardTypess ArrayList based on sample type
        // --------------------------------------------------------------------------------------------------

        private void cboCardTypeInit(SAMPLE_TYPE sampleType)
        {
            cboCardType.Items.Clear();

            InitializeCardTypeList(sampleType);
        }
        #endregion

        #region List Boxes

        // Selected Index Change Event for lbSamples
        //     updates the tbDescr text box control
        //     deterimes if source and destination combo boxes should be enabled
        // --------------------------------------------------------------------------------------------------

        private void lbSamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.lbSamples.Text == _samples)
                    return;

                _samples = this.lbSamples.Text;

                this.cboDest.Text = "Output Bin";
                this.cboSrc.Text = "Feeder";

                switch (_samples)
                {
                    case "Print 1":
                    case "Print 2":
                    case "Print 3":
                    case "Print 4":
                    case "Print 5":
                    case "Print 6":
                        InitializeCardListForPrintingTests(SAMPLE_TYPE.PRINTING);
                        InitializePrintTestSelections(_samples);
                        break;

                    case "Print With Contactless Smart Card":
                    case "Print With Contact Smart Card":
                    case "Smart Card Process":
                        InitializeCardListForSmartcardTests(SAMPLE_TYPE.SMART_CARD);
                        InitializeSmartCardTestSelections(_samples);
                        break;

                    case "Multi Print":
                        InitializeMultipleTestSelections();
                        break;

                    case "Write Mag Data":
                    case "Read Mag Data":
                    case "Print With Magnetic Encoding":
                        InitializeCardListForMagneticTests(SAMPLE_TYPE.MAG);
                        InitializeMagneticTestSelections(_samples);
                        break;

                    case "Position Card":
                    case "Eject Card":
                    case "Reject Card":
                        InitializeCardListForCardMovement(_samples, ref _cardTypes);
                        InitializeCardMovementTestSelections(_samples);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("lbSamples_SelectedIndexChanged threw exception: " + ex.Message);
            }
            finally
            {
                btnRun.Enabled = lbSamples.Items.Count > 0 ? true : false;
            }
        }

        private void InitializePrintTestSelections(string test)
        {
            try
            {
                switch (test)
                {
                    case "Print 1":
                        this.tbDescr.Text = "Print 1:\r\n" +
                            "   Draws images, shapes and text into the graphic buffer\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;

                    case "Print 2":
                        this.tbDescr.Text = "Print 2:\r\n" +
                            "   Draws an image into the graphic buffer\r\n" +
                            "   Applies adjustments\r\n" +
                            "   Stores profile to a file\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;

                    case "Print 3":
                        this.tbDescr.Text = "Print 3\r\n" +
                            "   Draws an image into the graphic buffer\r\n" +
                            "   Applies saved profile from file\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;

                    case "Print 4":
                        this.tbDescr.Text = "Print 4\r\n" +
                            "   Draws an image into the graphic buffer\r\n" +
                            "   Applies gray scale conversion\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;

                    case "Print 5":
                        this.tbDescr.Text = "Print 5\r\n" +
                            "   Draws a backgound image into the graphic buffer\r\n" +
                            "   Draws an image containing transparency data\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;

                    case "Print 6":
                        this.tbDescr.Text = "Print 6\r\n" +
                            "   Draws an image into the graphic buffer\r\n" +
                            "   Prints grapic buffer data\r\n\r\n";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializePrintTestSelections threw exception: " + ex.Message);
            }
        }

        private void InitializeSmartCardTestSelections(string test)
        {
            try
            {
                switch (test)
                {
                    case "Print With Contactless Smart Card":
                        this.tbDescr.Text = "Smart Card Process\r\n" +
                        "   Starts a smart card job\r\n" +
                        "   Card is moved to the smart card station\r\n" +
                        "   Job is suspended\r\n" +
                        "   Note: smart card encoding is independent of the printer\r\n" +
                        "   A JobResume or JobAbort is sent to finish Job Operation\r\n\r\n" +
                        "   Print job is performed after message box is dismissed";
                        break;

                    case "Print With Contact Smart Card":
                        this.tbDescr.Text = "Smart Card Process\r\n" +
                        "   Starts a smart card job\r\n" +
                        "   Card is moved to the smart card station\r\n" +
                        "   Job is suspended\r\n" +
                        "   Note: smart card encoding is independent of the printer\r\n" +
                        "   A JobResume or JobAbort is sent to finish Job Operation\r\n\r\n" +
                        "   Print job is performed after message box is dismissed";
                        break;

                    case "Smart Card Process":
                        this.tbDescr.Text = "Smart Card Process\r\n" +
                        "   Starts a smart card job\r\n" +
                        "   Card is moved to the smart card station\r\n" +
                        "   Job is suspended\r\n" +
                        "   Note: smart card encoding is independent of the printer\r\n" +
                        "   A JobResume or JobAbort is sent to finish Job Operation\r\n\r\n";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeSmartCardTestSelections threw exception: " + ex.Message);
            }
        }

        private void InitializeMagneticTestSelections(string test)
        {
            try
            {
                switch (test)
                {
                    case "Write Mag Data":
                        this.tbDescr.Text = "Write Mag Data\r\n" +
                        "   Encodes all three tracks\r\n\r\n";
                        break;

                    case "Read Mag Data":
                        this.tbDescr.Text = "Read Mag Data\r\n" +
                        "   Reads all three tracks\r\n\r\n";
                        break;

                    case "Print With Magnetic Encoding":
                        this.tbDescr.Text = "Print With Magnetic Encoding\r\n" +
                        "   Draws into the graphic buffer front and back side images\r\n" +
                        "   Runs a print job with magnetic encoding\r\n\r\n";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeMagneticTestSelections threw exception: " + ex.Message);
            }
        }

        private void InitializeMultipleTestSelections()
        {
            try
            {
                _cardTypes = "Multi Print";

                EnableSrcDest(false);

                this.tbDescr.Text = "Multi Print\r\n" +
                    "   Opens a form for multiple card printing\r\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeMultipleTestSelections threw exception: " + ex.Message);
            }
        }

        private void InitializeCardMovementTestSelections(string test)
        {
            try
            {
                switch (test)
                {
                    case "Position Card":
                        this.tbDescr.Text = "Position Card\r\n" +
                        "   Moves a card from the specified source to destination\r\n\r\n";
                        break;

                    case "Eject Card":
                        this.tbDescr.Text = "Eject Card\r\n" +
                        "   Warning: Card must be in the HOLD or INTERNAL location\r\n" +
                        "   Moves a card to the OUTPUT BIN\r\n\r\n";
                        break;

                    case "Reject Card":
                        this.tbDescr.Text = "Reject Card\r\n" +
                        "   Warning: Card must be in the HOLD or INTERNAL location\r\n" +
                        "   Moves a card to the REJECT BIN\r\n\r\n";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardMovementTestSelections threw exception: " + ex.Message);
            }
        }
        #endregion

        #region "Initializations"
        private void InitializeCardListForPrintingTests(SAMPLE_TYPE sampleType)
        {
            try
            {
                if (_cardTypes != "Print")
                {
                    cboCardType.Items.Clear();

                    InitializeCardTypeList(sampleType);

                    _cardTypes = "Print";
                }

                EnableSrcDest(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardListForPrintingTests threw exception: " + ex.Message);
            }
        }

        private void InitializeCardListForSmartcardTests(SAMPLE_TYPE sampleType)
        {
            try
            {
                cboCardType.Items.Clear();

                InitializeCardTypeList(sampleType);

                _cardTypes = "Smart Card";

                EnableSrcDest(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardListForSmartcardTests threw exception: " + ex.Message);
            }
        }

        private void InitializeCardListForMagneticTests(SAMPLE_TYPE sampleType)
        {
            try
            {
                cboCardType.Items.Clear();

                InitializeCardTypeList(sampleType);

                _cardTypes = "Mag";

                EnableSrcDest(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardListForMagneticTests threw exception: " + ex.Message);
            }
        }

        private void InitializeCardListForCardMovement(string samples, ref string cardTypes)
        {
            try
            {
                switch (samples)
                {
                    case "Position Card":
                        if (cardTypes != "")
                        {
                            this.cboCardType.Items.Clear();
                            this.cboCardType.Text = "";
                            cardTypes = "";
                        }
                        EnableSrcDest(true);
                        break;

                    case "Eject Card":
                    case "Reject Card":
                        this.cboCardType.Items.Clear();
                        this.cboCardType.Text = "";
                        cardTypes = "";
                        EnableSrcDest(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardListForCardMovement threw exception: " + ex.Message);
            }
        }

        private void InitializeCardTypeList(SAMPLE_TYPE sampleType)
        {
            bool first = true;
            string cardType = string.Empty;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (_cardTypeList == null)
                    return;
                
                for (int i = 0; i < _cardTypeList.Count; i++)
                {
                    cardType = _cardTypeList[i].ToString();

                    if (cardType.ToLower().Contains("custom"))
                    {
                        this.cboCardType.Items.Add(cardType);
                    }
                    else
                    {
                        switch (sampleType)
                        {
                            case SAMPLE_TYPE.SMART_CARD:
                                AddSmartCardType(ref cboCardType, cardType, ref first);
                                break;

                            case SAMPLE_TYPE.MAG:
                                AddMagneticCardType(ref cboCardType, cardType, ref first);
                                break;

                            case SAMPLE_TYPE.PRINTING:
                                AddPrintCardType(ref cboCardType, cardType, ref first);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardTypeList threw exception: " + ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void AddSmartCardType(ref ComboBox cbo, string cardType, ref bool first)
        {
            try
            {
                string item = Convert.ToString(lbSamples.SelectedItem);
                if (item.Contains("Contactless"))
                {
                    if( cardType.ToLower().Contains("mifare"))
                        AddCardType(ref cbo, cardType, ref first);
                }
                else if (item.Contains("Contact"))
                {
                    if (cardType.ToLower().Contains("pvc,sle"))
                        AddCardType(ref cbo, cardType, ref first);
                }
                else 
                {
                    if (cardType.ToLower().Contains("pvc,sle") || cardType.ToLower().Contains("mifare"))
                    {
                        cbo.Items.Add(cardType);
                        if (first)
                        {
                            cbo.Text = cardType;
                            first = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddSmartCardType threw exception: " + ex.Message);
            }
        }

        private void AddCardType(ref ComboBox cbo, string cardType, ref bool first)
        {
            try
            {
                cbo.Items.Add(cardType);
                if (first)
                {
                    cbo.Text = cardType;
                    first = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddCardType threw exception: " + ex.Message);
            }
        }

        private void AddMagneticCardType(ref ComboBox cbo, string cardType, ref bool first)
        {
            try
            {
                if (cardType.ToLower().Contains("loco") || cardType.ToLower().Contains("hico"))
                {
                    cbo.Items.Add(cardType);
                    if (first)
                    {
                        cbo.Text = cardType;
                        first = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddMagneticCardType threw exception: " + ex.Message);
            }
        }

        private void AddPrintCardType(ref ComboBox cbo, string cardType, ref bool first)
        {
            try
            {
                switch (cardType.ToLower())
                {
                    case "pvc,re-xfer rdy":
                    case "pvc":
                    case "polycarbonate":
                    case "pvc,composite":
                    case "pvc,composite,z6":
                    case "teslin,composite":
                    case "pet":
                    case "petg":
                    case "abs":
                        cbo.Items.Add(cardType);
                        if (first)
                        {
                            cbo.Text = cardType;
                            first = false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddPrintCardType threw exception: " + ex.Message);
            }
        }

        // Builds the _cardTypes ArrayList
        //     from the selected printer
        // --------------------------------------------------------------------------------------------------
        private int CardTypesInit(ref ZXPSampleCode z, string deviceName)
        {
            Job job = null;
            try
            {
                _cardTypeList.Clear();

                job = new Job();

                if( !ConnectToPrinter(ref z, ref job, deviceName) )
                    return 0;
                
                z.GetCardTypeList(ref job);
                if (z.CardTypeList != null)
                {
                    Array array = (Array)z.CardTypeList;
                    for (int i = 0; i < array.GetLength(0); i++)
                        _cardTypeList.Add((string)array.GetValue(i));
                }
                else
                {
                    MessageBox.Show("No ZMotif devices found");
                    this.Refresh();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("cboCardTypeInit Exception: " + e.Message);
            }
            finally
            {
                DisconnectFromPrinter(ref z, ref job);
                job = null;
            }
            return _cardTypeList.Count;
        }

        // Initializes the printer selection combo box
        //     based on printer discovery from GetDeviceList
        // --------------------------------------------------------------------------------------------------

        private bool cboPrnInit(bool usb)
        {
            Cursor.Current = Cursors.WaitCursor;

            this.cboPrn.Text = string.Empty;
            this.cboPrn.Items.Clear();
            this.cboPrn.Refresh();

            ZXPSampleCode z = null;
            bool bRet = true;
            try
            {
                if (!string.IsNullOrEmpty(cboPrn.Text))
                {
                    return true;
                }
                else //search for printers
                {
                    z = new ZXPSampleCode();
                    //----------------------------------------------------------------------------------------------------
                    //note: pass true for searching for usb-connected printers, false to search for ethernet printers
                    //for ethernert printers, only those printers on the same subnet as the sample application will be
                    //located
                    //----------------------------------------------------------------------------------------------------
                    z.GetDeviceList(usb);
                    if (z.DeviceList != null)
                    {
                        Array array = (Array)z.DeviceList;

                        for (int i = 0; i < array.GetLength(0); i++)
                            this.cboPrn.Items.Add((string)array.GetValue(i));

                        this.cboPrn.Text = (string)array.GetValue(0);
                    }
                    else
                    {
                        bRet = false;
                    }
                }
            }
            catch
            {
                bRet = false;
            }
            finally
            {
                z = null;
                Cursor.Current = Cursors.Default;
            }
            return bRet;
        }

        // Enables / Disables
        //     source & destination labels and combo boxes
        // --------------------------------------------------------------------------------------------------

        private void EnableSrcDest(bool enable)
        {
            this.lblSrc.Enabled = enable;
            this.lblDest.Enabled = enable;

            this.cboSrc.Enabled = enable;
            this.cboDest.Enabled = enable;
        }
        #endregion //Initializiations
        
        #region Support

        public bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

            //create our Regular Expression object
            Regex check = new Regex(pattern);

            //boolean variable to hold the status
            bool valid = false;

            //check to make sure an ip address was provided
            if (string.IsNullOrEmpty(addr))
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }
            //return the results
            return valid;
        }
        
        private void SetDestinations(ref ZXPSampleCode zxp, string source, string destination)
        {
            try
            {
                // Sets the card source location
                switch (source)
                {
                    case "ATM Slot":
                        zxp.Feeder = FeederSourceEnum.ATMSlot;
                        break;

                    case "Internal":
                        zxp.Feeder = FeederSourceEnum.Internal;
                        break;

                    default:
                        zxp.Feeder = FeederSourceEnum.CardFeeder;
                        break;
                }

                // Sets the card destination location
                switch (destination)
                {
                    case "Feeder":
                        zxp.Destination = DestinationTypeEnum.Feeder;
                        break;

                    case "Hold":
                        zxp.Destination = DestinationTypeEnum.Hold;
                        break;

                    case "Reject Bin":
                        zxp.Destination = DestinationTypeEnum.Reject;
                        break;

                    default:
                        zxp.Destination = DestinationTypeEnum.Eject;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SetDestinations threw exception: " + ex.Message);
            }
        }

        private void PerformPrintTest(ref ZXPSampleCode zxp, string test, string cardType)
        {
            try
            {
                lbJobStatus.Visible = true;
                lbJobStatus.Text = "Waiting for " + test + " to complete";
                Refresh();
                Application.DoEvents();

                switch (test)
                {
                    case "Print 1":
                        zxp.Print_1(cardType);
                        break;

                    case "Print 2":
                        zxp.Print_2(cardType);
                        break;

                    case "Print 3":
                        zxp.Print_3(cardType);
                        break;

                    case "Print 4":
                        zxp.Print_4(cardType);
                        break;

                    case "Print 5":
                        zxp.Print_5(cardType);
                        break;

                    case "Print 6":
                        zxp.Print_6(cardType);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PerformPrintTest exception: " + ex.Message);
            }
            finally
            {
                lbJobStatus.Text = "Job Result = " + zxp.Msg;
            }
        }

        private void PerformPositionCardTest(ref ZXPSampleCode zxp, string test)
        {
            try
            {
                switch (test)
                {
                    case "Position Card":
                        zxp.PositionCard();
                        break;

                    case "Eject Card":
                        zxp.EjectCard();
                        break;

                    case "Reject Card":
                        zxp.RejectCard();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PerformPositionCardTest threw exception: " + ex.Message);
            }
        }

        private void PerformSmartCardTest(ref ZXPSampleCode zxp, string test)
        {
            try
            {
                switch (test)
                {
                    case "Print With Contactless Smart Card":
                        zxp.ContactlessSmartCardAndPrint();
                        break;

                    case "Print With Contact Smart Card":
                        zxp.ContactSmartCardAndPrint();
                        break;

                    case "Smart Card Process":
                        zxp.SmartCard();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PerformSmartCardTest threw exception: " + ex.Message);
            }
        }

        private void PerformMultiPrintTest(ref ZXPSampleCode zxp, string printer)
        {
            try
            {
                using (frmMultiPrint f = new frmMultiPrint(printer))
                {
                    f.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PerformMultiPrintTest threw exception: " + ex.Message);
            }
        }

        private void PerformMagneticPrintTest(ref ZXPSampleCode zxp, string test)
        {
            try
            {
                switch (test)
                {
                    case "Print With Magnetic Encoding":
                        zxp.PrintWithMag();
                        break;

                    case "Write Mag Data":
                        zxp.MagneticDataOnly();
                        break;

                    case "Read Mag Data":
                        zxp.Tracks = DataSourceEnum.Track1Data | DataSourceEnum.Track2Data |
                                     DataSourceEnum.Track3Data;

                        // Reads and displays all 3 magnetic tracks
                        if (zxp.MagRead())
                        {
                            MessageBox.Show("Track 1 = " + zxp.Track1Data + "\r\n" +
                                            "Track 2 = " + zxp.Track2Data + "\r\n" +
                                            "Track 3 = " + zxp.Track3Data);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PerformMagneticPrintTest threw exception: " + ex.Message);
            }
        }

        private bool ConnectToPrinter(ref ZXPSampleCode z, ref Job job, string deviceName)
        {
            try
            {
                if (!z.Connect(ref job))
                {
                    Cursor.Current = Cursors.Default;
                    
                    MessageBox.Show("Unable to open device [" + deviceName + "]");

                    DisconnectFromPrinter(ref z, ref job);
                    
                }
                else return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConnectToPrinter threw exception: " + ex.Message);
            }
            return false;
        }

        private void DisconnectFromPrinter(ref ZXPSampleCode z, ref Job job)
        {
            try
            {
                z.Disconnect(ref job);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DisconnectFromPrinter threw exception: " + ex.Message);
            }
        }
        #endregion //Support

        

       

        

        
    }
}