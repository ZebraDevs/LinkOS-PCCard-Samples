using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// ZXP Namespaces

using ZMOTIFPRINTERLib;
using ZMTGraphics;

namespace ZXPSampleCode
{
    public partial class frmMultiPrint : Form
    {
        #region Declarations

        // Local variables
        // --------------------------------------------------------------------------------------------------

        byte[]         _bmpBack          = null,
                       _bmpFront         = null;
        private short  _alarm            = 0;
        private string _cardType         = "PVC",
                       _deviceName       = string.Empty,
                       _nameBackImage    = "Back.bmp",
                       _nameFrontImage   = string.Empty;
        
        private bool   _isZXP7           = false;
        
        #endregion

        private string FrontImage
        {
            set {_nameFrontImage = value;}
            get {return _nameFrontImage; }
        }

        private bool IsZXP7
        {
            get { return _isZXP7; }
        }
        
        #region Form

        // Form class initialization
        // --------------------------------------------------------------------------------------------------

        public frmMultiPrint(string deviceName)
        {
            InitializeComponent();
            
            _deviceName = deviceName;
        }

        // Form load event
        // --------------------------------------------------------------------------------------------------

        private void frmMultiPrint_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region Buttons

        // Clears the results text box
        // --------------------------------------------------------------------------------------------------

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbResults.Text = string.Empty;
        }

        // Closes the form
        // --------------------------------------------------------------------------------------------------

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        // Starts the printing process
        // --------------------------------------------------------------------------------------------------

        private void btnRun_Click(object sender, EventArgs e)
        {
            Job job = null;
            ZXPSampleCode z = null;

            int actionID = 0;
            short alarm = 0;
            
            string msg = string.Empty;
            string status = string.Empty;
            
            // Ensures that count is a number
            if (!IsInteger(this.tbCount.Text))
            {
                MessageBox.Show("Count is not an integer value");
                this.tbCount.Focus();
                return;
            }

            try
            {
                job = new Job();
                z = new ZXPSampleCode(_deviceName);

                // Opens a connection with a ZXP printer

                z.Connect(ref job);

                _isZXP7 = z.IsZXP7;

                // Sets the card source and destination
                job.JobControl.FeederSource = FeederSourceEnum.CardFeeder;
                job.JobControl.Destination = DestinationTypeEnum.Eject;

                int count = Convert.ToInt16(this.tbCount.Text);

                this.tbResults.AppendText(DisplayTime() + ": Start\r\n");

                if (!BuildBitmaps())
                {
                    MessageBox.Show("Unable to build bitmaps");
                    this.Close();
                }

                // Prints (count) number of cards
                for (int cardCount = 1; cardCount <= count; )
                {
                    // Checks to see if the printer is in alarm condition

                    alarm = GetAlarm(ref job, out msg);

                    if (!string.IsNullOrEmpty(msg))
                    {
                        this.tbResults.AppendText(DisplayTime() + ": " + cardCount.ToString() + ": " + msg);
                    }

                    // If the printer is not in alarm condition
                    //     starts a print job and increments cardCount

                    if (alarm == 0)
                    {
                        // Sets the card type
                        if (!_isZXP7)
                            job.JobControl.CardType = _cardType;
                        
                        // Builds and prints the graphic layers

                        job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                                -1, GraphicTypeEnum.BMP, _bmpFront);

                        job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0,
                                                -1, GraphicTypeEnum.BMP, _bmpBack);

                        job.PrintGraphicsLayers(1, out actionID);

                        this.tbResults.AppendText(DisplayTime() + ": " + cardCount.ToString() +
                                                  ": Print Job Processed with no Errors\r\n");

                        // Waits for the job status
                        //     "cleaning_up" or "done_ok" or is in error state

                        z.JobWait(ref job, actionID, 180, out status);

                        this.tbResults.AppendText(DisplayTime() + ": " + cardCount.ToString() +
                                                  ": Job Wait Status = " + status + "\r\n");

                        cardCount++;
                    }
                }

                // Make sure the last job is truly finished

                while (true)
                {
                    z.JobWait(ref job, actionID, 180, out status);

                    if (status == "done_ok" || status.Contains("error"))
                        break;
                }
                this.tbResults.AppendText(DisplayTime() + ": Finished\r\n");
            }
            catch (Exception exc)
            {
                this.tbResults.AppendText("Exception: " + exc.Message);
            }
            finally
            {
                z.Disconnect(ref job);
                z = null;
            }
        }
        #endregion

        #region Support

        // Builds the front and back side bitmaps
        // --------------------------------------------------------------------------------------------------

        private bool BuildBitmaps()
        {
            ZMotifGraphics g = new ZMotifGraphics();

            bool builtOk = true;

            try
            {
                // Gets images from files

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";

                byte[] imgFront = g.ImageFileToByteArray(_nameFrontImage);
                byte[] imgBack  = g.ImageFileToByteArray(_nameBackImage);

                // Initializes the graphic buffer for the front side

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                    ZMotifGraphics.RibbonTypeEnum.Color);

                // Draws text and image into the graphic buffer for front side

                g.DrawImage(ref imgFront, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Front Side: Color Image", "Arial", 10,
                    ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                // Creates the front side bitmap

                int dataLen;
                _bmpFront = g.CreateBitmap(out dataLen);

                // Clears the graphics buffer

                g.ClearGraphics();

                // Initializes the graphic buffer for the back side

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                    ZMotifGraphics.RibbonTypeEnum.MonoK);

                // Draws text an image into the graphic buffer for the back side

                g.DrawImage(ref imgBack, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Back Side Monochrome Image", "Arial", 10,
                    ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Black"));

                // Creates the back side bitmap

                _bmpBack = g.CreateBitmap(out dataLen);

                g.ClearGraphics();
            }
            catch
            {
                builtOk = false;
            }
            return builtOk;
        }

        // Returns a string containing hours:minutes:seconds
        // --------------------------------------------------------------------------------------------------

        private string DisplayTime()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString("HH:mm:ss");
        }

        // Gets printer alarm condition and displays if alarm condition has changed
        // --------------------------------------------------------------------------------------------------

        private short GetAlarm(ref Job job, out string msg)
        {
            short  alarm = 0;
            string junk  = "";

            msg = "";

            try
            {
                alarm = job.Device.GetDeviceInfo(out junk, out junk, out junk, out junk, out junk, out junk,
                    out junk, out junk, out junk, out junk);

                if (alarm == 0 && _alarm != 0)
                {
                    msg = "Alarm has been cleared\r\n";
                    _alarm = 0;
                }
                else if (alarm != 0 && alarm != _alarm)
                {
                    msg = "Alarm ("
                             + alarm.ToString() + ") "
                             + job.Device.GetStatusMessageString(alarm);
                    _alarm = alarm;
                }
            }
            catch (Exception e)
            {
                msg = "Get Alarm Exception: " + e.Message;
            }
            return alarm;
        }

        // Determines if the value is an integer
        // --------------------------------------------------------------------------------------------------

        private bool IsInteger(string v)
        {
            bool isNumeric = true;

            try
            {
                int i = Convert.ToInt16(v);
            }
            catch
            {
                isNumeric = false;
            }
            return isNumeric;
        }
        #endregion
    }
}