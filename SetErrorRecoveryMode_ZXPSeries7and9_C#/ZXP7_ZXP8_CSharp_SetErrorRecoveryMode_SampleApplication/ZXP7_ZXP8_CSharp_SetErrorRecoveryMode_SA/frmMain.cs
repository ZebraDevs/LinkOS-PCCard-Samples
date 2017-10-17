using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ZMOTIFPRINTERLib;

namespace ZXP7_ZXP8_CSharp_SetErrorRecoveryMode_SA
{
    public partial class frmMain : Form
    {
        private string _cardTypes = string.Empty,
                       _samples = string.Empty;

        // Array list containing all card types for a printer

        private ArrayList _cardTypeList = null;

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
        
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Job job = null;
            ZMTGraphics.ZMotifGraphics graphics = null;
            StringBuilder sb = null;
            try
            {
                //Get the Printer and Graphics SDK versions and display them on the title bar
                byte major = 0;
                byte minor = 0;
                byte rev = 0;
                byte build = 0;

                job = new Job();

                graphics = new ZMTGraphics.ZMotifGraphics();

                sb = new StringBuilder();

                job.GetSDKVersion(out major, out minor, out build, out rev);

                sb.Append("     Printer SDK version: ");
                sb.Append(major.ToString() + ".");
                sb.Append(minor.ToString() + ".");
                sb.Append(build.ToString() + ".");
                sb.Append(rev.ToString() + "   ");

                major = 0;
                minor = 0;
                rev = 0;
                build = 0;

                graphics.GetSDKVersion(out major, out minor, out build, out rev);

                sb.Append("Graphics SDK version: ");
                sb.Append(major.ToString() + ".");
                sb.Append(minor.ToString() + ".");
                sb.Append(build.ToString() + ".");
                sb.Append(rev.ToString());

                //Text += " " + sb.ToString(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                job = null;
                graphics = null;
                sb = null;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            ZXPSampleCode z = null;
            try
            {
                // Get a list of USB ZXP devices
                if (cboPrnInit(true))
                {
                    // Displays versions
                    z = new ZXPSampleCode(this.cboPrn.Text);
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit();
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

        private void btnConnectToPrinter_Click(object sender, EventArgs e)
        {
            ZXPSampleCode z = null;
            Job job = null;
            try
            {
                Cursor = Cursors.WaitCursor;

                btnRun.Enabled = false;

                lbStatus.Visible = true;
                lbStatus.Text = "Connecting to printer";
                Refresh();
                Application.DoEvents();

                job = new Job();

                z = new ZXPSampleCode(this.cboPrn.Text);

                if (z.Connect(ref job))
                {
                    // Displays versions
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit();
                    }
                    else
                        cboCardType.Enabled = false;

                    btnRun.Enabled = true;
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
                if (z != null)
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
                Refresh();
                Application.DoEvents();

                // Get a list of ZXP devices
                if (cboPrnInit(rbUSB.Checked))
                {
                    // Displays versions
                    z = new ZXPSampleCode(this.cboPrn.Text);
                    this.lblVersions.Text = z.GetVersions();

                    if (!z.IsZXP7)
                    {
                        // Loads card types based on type of sample type
                        _cardTypeList = new ArrayList();

                        CardTypesInit(ref z, this.cboPrn.Text);

                        cboCardTypeInit();
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ZXPSampleCode zxp = null;
            try
            {
                zxp = new ZXPSampleCode(this.cboPrn.Text);

                btnRun.Enabled = false;

                // Sets the card type
                //zxp.CardType = this.cboCardType.Text;

                lbStatus.Text = "Active Job: Monochrome Print";
                lbStatus.Visible = true;
                Refresh();
                Application.DoEvents();

                // Runs the specified sample code
                PerformPrintTest(ref zxp, "Set Error Recovery Mode", cboCardType.Text);
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

        #region Combo Boxes

        // Initializes cboCardType combo box
        //     from the _cardTypess ArrayList based on sample type
        // --------------------------------------------------------------------------------------------------

        private void cboCardTypeInit()
        {
            cboCardType.Items.Clear();

            InitializeCardTypeList();
        }
        #endregion

        #region Initializations

        private void InitializeCardListForPrintingTests()
        {
            try
            {
                if (_cardTypes != "Print")
                {
                    cboCardType.Items.Clear();

                    InitializeCardTypeList();

                    _cardTypes = "Print";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeCardListForPrintingTests threw exception: " + ex.Message);
            }
        }

        private void InitializeCardTypeList()
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
                        AddPrintCardType(ref cboCardType, cardType, ref first);
                        break;
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

                if (!ConnectToPrinter(ref z, ref job, deviceName))
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

        #endregion //Initialization

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
                    case "Set Error Recovery Mode":
                        zxp.Perform_Set_Error_Recovery_Mode_Print(cardType);
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
