using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

// ZXP Namespaces

using ZMOTIFPRINTERLib;
using ZMTGraphics;

namespace ZXPSampleCode
{
    class ZXPSampleCode
    {
        #region Declarations

        // Local declarations
        // --------------------------------------------------------------------------------------------------

        private bool   _isContact        = false,
                       _isContactless    = false,
                       _isMag            = false,
                       _isZXP7           = false;
        private short  _alarm            = 0;
        private string _cardType         = string.Empty,
                       _deviceName       = string.Empty,
                       _msg              = string.Empty,
                       _nameBackImage    = "Back.bmp",
                       _nameFrontImage   = string.Empty,
                       _nameImageProfile = "ImageProfile.xml",
                       _nameZebraImage   = "Zebra.bmp",
                       _track1Data       = string.Empty,
                       _track2Data       = string.Empty,
                       _track3Data       = string.Empty;

        private object _cardTypeList     = null,
                       _deviceList       = null;

        private DestinationTypeEnum _destination;
        private FeederSourceEnum    _feeder;
        private DataSourceEnum      _tracks;

        private struct JobStatusStruct
        {
            public int    copiesCompleted,
                          copiesRequested,
                          errorCode;
            public string cardPosition,
                          contactlessStatus,
                          contactStatus,
                          magStatus,
                          printingStatus,
                          uuidJob;
        }

        #endregion

        #region Properties

        // Properties
        // --------------------------------------------------------------------------------------------------
        public string FrontImage
        {
            get { return _nameFrontImage; }
            set { _nameFrontImage = value; }
        }

        public bool IsZXP7
        {
            get { return _isZXP7; }
        }

        public object CardTypeList
        {
            get { return _cardTypeList; }
            set { _cardTypeList = value; }
        }

        public object DeviceList
        {
            get { return _deviceList; }
            set { _deviceList = value; }
        }

        public short Alarm
        {
            get { return _alarm; }
        }

        public string CardType
        {
            get { return _cardType; }
            set { _cardType = value; }
        }

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public string Track1Data
        {
            get { return _track1Data; }
            set { _track1Data = value; }
        }

        public string Track2Data
        {
            get { return _track2Data; }
            set { _track2Data = value; }
        }

        public string Track3Data
        {
            get { return _track3Data; }
            set { _track3Data = value; }
        }

        public DataSourceEnum Tracks
        {
            get { return _tracks; }
            set { _tracks = value; }
        }

        public DestinationTypeEnum Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        public FeederSourceEnum Feeder
        {
            get { return _feeder; }
            set { _feeder = value; }
        }

        #endregion

        #region Class Initialize

        // Class Initialization
        // --------------------------------------------------------------------------------------------------

        public ZXPSampleCode()
        {
            _deviceName = string.Empty;
        }

        public ZXPSampleCode(string deviceName)
        {
            _deviceName = deviceName;
        }
        #endregion

        #region ZMotif Device Connect

        // Connects to a ZMotif device
        // --------------------------------------------------------------------------------------------------

        public bool Connect(ref Job j)
        {
            bool bRet = true;

            try
            {
                if (j == null)
                    return false;

                if (!j.IsOpen)
                {
                    _alarm = j.Open(_deviceName);

                    IdentifyZMotifPrinter(ref j);
                }
            }
            catch(Exception e)
            {
                _msg = e.Message;
                bRet = false;
            }
            return bRet;
        }

        // Disconnects from a ZMotif device
        // --------------------------------------------------------------------------------------------------

        public bool Disconnect(ref Job j)
        {
            bool bRet = true;

            try
            {
                if (j == null)
                    return false;

                if (j.IsOpen)
                {
                    j.Close();

                    do
                    {
                        Thread.Sleep(10);
                    } while (Marshal.FinalReleaseComObject(j) != 0);
                }
            }
            catch
            {
                bRet = false;
            }
            finally
            {
                j = null;
                GC.Collect();
            }
            return bRet;
        }
        #endregion

        #region Identify ZXP Printer Type

        private void IdentifyZMotifPrinter(ref Job job)
        {
            try
            {
                string vendor = string.Empty;
                string model = string.Empty;
                string serialNo = string.Empty;
                string MAC = string.Empty;
                string headSerialNo = string.Empty;
                string OemCode = string.Empty;
                string fwVersion = string.Empty;
                string mediaVersion = string.Empty;
                string heaterVersion = string.Empty;
                string zmotifVer = string.Empty;

                GetDeviceInfo(ref job, out vendor, out model, out serialNo, out MAC,
                              out headSerialNo, out OemCode, out fwVersion,
                              out mediaVersion, out heaterVersion, out zmotifVer);

                if (model.Contains("7"))
                    _isZXP7 = true;
                else
                    _isZXP7 = false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private short GetDeviceInfo(ref Job job, out string vender, out string model, out string serialNo, out string MAC,
                                    out string headSerialNo, out string OemCode, out string fwVersion, out string mediaVersion,
                                    out string heaterVersion, out string zmotifVersion)
        {
            vender = string.Empty;
            model = string.Empty;
            serialNo = string.Empty;
            MAC = string.Empty;
            headSerialNo = string.Empty;
            OemCode = string.Empty;
            fwVersion = string.Empty;
            mediaVersion = string.Empty;
            heaterVersion = string.Empty;
            zmotifVersion = string.Empty;

            try
            {
                return job.Device.GetDeviceInfo(out vender, out model, out serialNo, out MAC,
                                                 out headSerialNo, out OemCode, out fwVersion,
                                                 out mediaVersion, out heaterVersion, out zmotifVersion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Identify ZXP Printer Type

        #region Card Movement

        // Sends a card to the Output Bin
        //     from the Internal Hold Position
        // --------------------------------------------------------------------------------------------------

        public bool EjectCard()
        {
            bool bRet = true;
            
            Job job = null;

            try
            {
                job = new Job();

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in Alarm State";
                    return false;
                }
                _alarm = job.EjectCard();
            }
            catch (Exception e)
            {
                _msg = e.Message;
                bRet = false;
            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }

        // Positions a card from the 
        //     specified soure location
        //     to the specified destination location
        // --------------------------------------------------------------------------------------------------

        public bool PositionCard()
        {
            bool  bRet     = true;
            int   actionID = 0;
            
            Job job = null;

            try
            {
                job = new Job();

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in Alarm State";
                    return false;
                }

                // Sets the job source and destination

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                _alarm = job.PositionCard(out actionID);

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                _msg = e.Message;
                bRet = false;
            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }

        // Sends a card to the Reject Bin
        //     from the Internal Hold Position
        // --------------------------------------------------------------------------------------------------

        public bool RejectCard()
        {
            bool bRet = true;
            
            Job job = null;

            try
            {
                job = new Job();

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in Alarm State";
                    return false;
                }
                _alarm = job.RejectCard();
            }
            catch (Exception e)
            {
                _msg = e.Message;
                bRet = false;
            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }
        #endregion

        #region Magnetic

        // Magnetic Encoding
        //     no printing
        // --------------------------------------------------------------------------------------------------

        public bool MagneticDataOnly()
        {
            bool bRet = true;
            
            Job job = null;

            try
            {
                job = new Job();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in Alarm State";
                    return false;
                }

                // Determines if the ZXP device supports magnetic encoding
                // -------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    return false;
                }

                if (!_isMag)
                {
                    _msg = "Printer is not configured for Magnetic Encoding\r\n";
                    return false;
                }

                // Enocdes all three tracks
                //     holds the card inside the printer at the end of the job
                // -----------------------------------------------------------

                int actionID = 0;
                string track1 = "ABCDEFGH",
                       track2 = "12345678",
                       track3 = "87654321";

                if (!_isZXP7)
                    job.JobControl.CardType = GetMagneticCardType(ref job);
                
                // Set card start and end location
                // -------------------------------

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                // Runs magnetic encode only job
                // -----------------------------

                job.MagDataOnly(1, track1, track2, track3, out actionID);

                // Waits for the magnetic encoding job to complete
                // -----------------------------------------------

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);

                if (status != "cleaning_up" && status != "done_ok")
                {
                    _msg = "Magnetic Data Only Error: " + status + "\r\n";
                    return false;
                }
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }

        // Reads Magnetic Tracks
        // --------------------------------------------------------------------------------------------------

        public bool MagRead()
        {
            bool bRet = true;

            Job job = null;

            _track1Data = string.Empty;
            _track2Data = string.Empty;
            _track3Data = string.Empty;

            try
            {
                job = new Job();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in alarm condition";
                    return false;
                }

                // Determines if the ZXP device supports magnetic encoding
                // -------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    return false;
                }

                if (!_isMag)
                {
                    _msg = "Printer is not configured for Magnetic Encoding";
                    return false;
                }

                if(!_isZXP7)
                    job.JobControl.CardType = GetMagneticCardType(ref job);
                
                // Sets the source and destination positions
                // -----------------------------------------

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                // Runs a magnetic read job - no need to wait for job to complete. Job will be completed when function returns
                // ------------------------
                int actionID;
                job.ReadMagData(_tracks, out _track1Data, out _track2Data, out _track3Data, out actionID);
            }
            catch (Exception e)
            {
                _msg = e.Message;
                bRet = false;

            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }

        // Magnetic encodes then prints
        //     all in one job
        // --------------------------------------------------------------------------------------------------

        public bool PrintWithMag()
        {
            bool bRet = true;

            byte[] imgFront = null;
            byte[] imgBack = null;
            byte[] bmpFront = null;
            byte[] bmpBack = null;

            Job job = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                    return false;

                if (_alarm != 0)
                {
                    _msg = "Device is in alarm condition";
                    return false;
                }

                // Determines if the ZXP device supports magnetic encoding
                // -------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    _msg = "Unable to get Printer Configuration";
                    return false;
                }

                if (!_isMag)
                {
                    _msg = "Printer is not configured for Magnetic Encoding";
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Get images from files
                // ---------------------

                imgFront = g.ImageFileToByteArray(_nameFrontImage);
                imgBack = g.ImageFileToByteArray(_nameBackImage);

                // Initializes front side graphic buffer
                //     and draws images and text
                // -------------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref imgFront, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print with Mag: Front Side: Color Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                int dataLen = 0;
                bmpFront = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Initializes back side graphic buffer
                //     and draws images and text
                // ------------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.MonoK);

                g.DrawImage(ref imgBack, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print with Mag: Back Side Monochrome Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Black"));

                bmpBack = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Sets the card type
                // ------------------
                if (!_isZXP7)
                    job.JobControl.CardType = GetMagneticCardType(ref job);

                // Sets the card source and destination locations
                // ----------------------------------------------

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                // Loads the image data to print
                // -----------------------------

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1,
                                        GraphicTypeEnum.BMP, bmpFront);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1,
                                        GraphicTypeEnum.BMP, bmpBack);

                // Magnetic encodes and prints
                // ---------------------------

                int actionID = 0;
                string track1 = "ABCDEFGH",
                       track2 = "12345678",
                       track3 = "87654321";

                job.PrintGraphicsLayersWithMagData(1, track1, track2, track3, out actionID);
                job.ClearGraphicsLayers();

                // Waits for the magnetic encoding job to complete
                // -----------------------------------------------

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);

                if (status != "cleaning_up" && status != "done_ok")
                {
                    bRet = false;
                    _msg = "Print with Magnetic Encoding Error: " + status;
                    return false;
                }
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                imgFront = null;
                imgBack = null;
                bmpFront = null;
                bmpBack = null;

                Disconnect(ref job);
            }
            return bRet;
        }
        #endregion

        #region Print Graphic Samples

        // Print_1
        //     images, shapes, text, print
        // --------------------------------------------------------------------------------------------------

        public bool Print_1(string cardType)
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmpFront = null;
            byte[] bmpBack = null;

            Job  job  = null;
            ZMotifGraphics g = null;
                       
            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]\r\n";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition\r\n" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Builds the front side image (color)
                // -----------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,  ZMotifGraphics.RibbonTypeEnum.Color);

                img = g.ImageFileToByteArray(_nameFrontImage);
                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 400, 350, 0);

                g.DrawLine(160, 70, 250, 70, g.IntegerFromColor(System.Drawing.Color.Red), 5.0f);
                g.DrawRectangle(300, 20, 100, 100, 5.0f, g.IntegerFromColorName("Green"));
                g.DrawEllipse(450, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Blue));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Front Side: Image, Shapes, Text", "Arial", 10.0f,
                    ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColor(System.Drawing.Color.Navy));

                int dataLen;

                bmpFront = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Builds the back side image (monochrome)
                // ---------------------------------------
                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape, ZMotifGraphics.RibbonTypeEnum.MonoK);

                img = g.ImageFileToByteArray(_nameZebraImage);
                g.DrawImage(ref img, 50.0f, 50.0f, 275, 200, 0);

                g.DrawLine(350, 50, 475, 120, g.IntegerFromColor(System.Drawing.Color.Black), 5.0f);
                g.DrawRectangle(500, 20, 100, 100, 5.0f, g.IntegerFromColorName("Black"));
                g.DrawEllipse(650, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Black));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Back Side: Image, Shapes, Text", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColor(System.Drawing.Color.DarkBlue));

                bmpBack = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Print the images
                // ----------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1, GraphicTypeEnum.BMP, bmpFront);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1, GraphicTypeEnum.BMP, bmpBack);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);
                
                job.ClearGraphicsLayers(); 

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();

                g = null;
                img = null;
                bmpFront = null;
                bmpBack = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        // Print_2
        //     create image, apply adjustments, save profile, print
        // --------------------------------------------------------------------------------------------------

        public bool Print_2(string cardType)
        {
            bool bRet = true;

            byte[] bmpNormal = null;
            byte[] img = null;
            byte[] bmpAdjusted = null;

            Job job  = null;
            ZMotifGraphics g = null;
            
            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Create an image, adjust it, save to profile for the front side
                // --------------------------------------------------------------

                img = g.ImageFileToByteArray(_nameFrontImage);

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print 2: Front Side: Color Image", "Arial", 10,
                    ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                int dataLen;
                bmpNormal = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);

                g.AdjustBrightness(10);
                g.AdjustColorScale(10, 10, 10);
                g.AdjustContrast(10);
                g.AdjustSaturation(10);
                g.RotateHue(10);

                g.DrawTextString(50, 580, "Print 2: Back Side: Adjusted Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                // Saves the adjusted image to a profile file
                // ------------------------------------------
                g.SaveImageProfile(_nameImageProfile);

                bmpAdjusted = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Prints the adjusted image on front side, normal image back side
                // ---------------------------------------------------------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpNormal);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpAdjusted);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers(); 

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();

                g = null;
                bmpNormal = null;
                img = null;
                bmpAdjusted = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        // Print 3
        //     create image, apply adjustment from profile, print
        // ------------------------------------------------------

        public bool Print_3(string cardType)
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmpNormal = null;
            byte[] bmpAdjusted = null;

            Job  job  = null;
            ZMotifGraphics g = null;
                        
            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition\r\n" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Creates an image, applies a saved profile
                // -----------------------------------------

                img = g.ImageFileToByteArray(_nameFrontImage);

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print 3: Front Side: Color Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                int dataLen;
                bmpNormal = g.CreateBitmap(out dataLen);

                g.ClearGraphics();


                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print 3: Back Side: Adjusted Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                g.LoadImageProfile(_nameImageProfile);

                bmpAdjusted = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                // Print the adjusted image on both sides
                // --------------------------------------

                if(!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpNormal);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpAdjusted);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                img = null;
                bmpNormal = null;
                bmpAdjusted = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        // Print 4
        //     create image, apply gray scale conversion, print
        // --------------------------------------------------------------------------------------------------

        public bool Print_4(string cardType)
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmpNormal = null;
            byte[] bmpGray = null;

            Job job = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Creates an image, applies gray scale conversion
                // -----------------------------------------------

                img = g.ImageFileToByteArray(_nameFrontImage);

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print 4: Front Side: Color Image", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                int dataLen;
                bmpNormal = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);
                
                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 1000, 620, 0);
                g.DrawTextString(50, 580, "Print 4: Back Side: Gray Scale", "Arial", 10,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColorName("Navy"));

                g.ConvertToGrayScale(0.5f);

                bmpGray = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                // Print gray scale image on back side
                // ------------------------------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpNormal);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpGray);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                img = null;
                bmpNormal = null;
                bmpGray = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        // Print 5
        //     create an image with transparency, print
        // --------------------------------------------------------------------------------------------------

        public bool Print_5(string cardType)
        {
            bool bRet = true;

            byte[] fishByteArray = null;
            byte[] logoByteArray = null;
            byte[] bmp = null;
            
            Job  job  = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Background image
                // ----------------

                fishByteArray = g.ImageFileToByteArray(_nameFrontImage);

                // Transparent Image
                // -----------------

                logoByteArray = g.ImageFileToByteArray("Zebra_Transparent.gif");

                // Builds the image to print
                // -------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref fishByteArray, 0, 0, 1024, 648, 0);

                // all color between transparentLow and transparentHigh will be transparent
                // ------------------------------------------------------------------------

                int transparentLow = 0xc0c0c0;
                int transparentHigh = 0xfe0000;

                g.DrawImage(ref logoByteArray, 325, 325, 400, 50, 0, transparentLow, transparentHigh);

                g.DrawTextString(50.0f, 580.0f, "Print 5: Transparent", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular,
                                 g.IntegerFromColor(System.Drawing.Color.DarkBlue));

                int dataLen;
                bmp = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                // Prints the build image on both sides
                // ------------------------------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmp);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmp);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                fishByteArray = null;
                logoByteArray = null;
                bmp = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        // Print 6
        //     print image with a color background
        // --------------------------------------------------------------------------------------------------

        public bool Print_6(string cardType)
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmp = null;

            Job  job = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition" + "Error: " + job.Device.GetStatusMessageString(_alarm);
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                img = g.ImageFileToByteArray(_nameFrontImage);

                // Builds the image to print
                // -------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color, 0xCCCCCC);

                g.DrawImage(ref img, 50, 50, 924, 548, 0);
                g.DrawTextString(50.0f, 580.0f, "Print 6: Background fill, then Image", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular,
                                 g.IntegerFromColor(System.Drawing.Color.DarkBlue));

                int dataLen;
                bmp = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                // Print image on both sides
                // -------------------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmp);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmp);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                string status = string.Empty;
                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                img = null;
                bmp = null;

                Disconnect(ref job);
            }
            return bRet;
        }
        #endregion

        #region Smart Cards

        // Smart Card only
        // --------------------------------------------------------------------------------------------------

        public bool SmartCard()
        {
            bool bRet = true;
            
            Job  job = null;

            try
            {
                job = new Job();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition";
                    return false;
                }

                // Determines if the ZXP device supports smart card encoding
                // ---------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    _msg = "Unable to get printer configuration";
                    return false;
                }

                if (!_isContact && !_isContactless)
                {
                    _msg = "Printer is not configured for Contact or Contactless Encoding";
                    return false;
                }

                // Starts a smart card job
                // -----------------------

                if (!_isZXP7)
                    job.JobControl.CardType = GetContactCardType(ref job);

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.JobControl.SmartCardConfiguration(SideEnum.Front, SmartCardTypeEnum.Contact, true);

                int actionID = 0;
                job.SmartCardDataOnly(1, out actionID);

                // Waits for the card to reach the smart card station
                // --------------------------------------------------

                string status = string.Empty;
                AtStation(ref job, actionID, 30, out status);

                // Once the card reaches the station, the job is suspended
                //     note, the printer does not magange the actual smart card encoding or reading
                // --------------------------------------------------------------------------------

                // ***** Smart Card Code goes here *****

                // At the completion of smart card process
                //     if the smart card encoding was successful JobResume
                //     if the smart card encoding was Unsuccessful JobAbort
                // --------------------------------------------------------

                DialogResult result = MessageBox.Show(
                    "Job has been suspended\r\n" +
                    "   Smart Card Code is Independent of Printer Jobs\r\n" +
                    "   Yes to Resume Job\r\n" +
                    "   No to Abort Job",
                    "Smart Card Example",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);

                if (result == DialogResult.Yes)
                    JobResume(ref job);
                else
                    JobAbort(ref job, true);

                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                Disconnect(ref job);
            }
            return bRet;
        }

        public bool ContactlessSmartCardAndPrint()
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmpFront = null;
            byte[] bmpBack = null;
            
            Job job = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]\r\n";
                    return false;
                }

                // Determines if the ZXP device supports smart card encoding
                // ---------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    _msg = "Unable to get printer configuration";
                    return false;
                }

                if (!_isContact && !_isContactless)
                {
                    _msg = "Printer is not configured for Contact or Contactless Encoding";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition\r\n";
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Builds the front side image (color)
                // -----------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                img = g.ImageFileToByteArray(_nameFrontImage);
                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 400, 350, 0);

                g.DrawLine(160, 70, 250, 70, g.IntegerFromColor(System.Drawing.Color.Red), 5.0f);
                g.DrawRectangle(300, 20, 100, 100, 5.0f, g.IntegerFromColorName("Green"));
                g.DrawEllipse(450, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Blue));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Front Side: Image, Shapes, Text", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColor(System.Drawing.Color.Navy));

                int dataLen = 0;
                bmpFront = g.CreateBitmap(out dataLen);
                g.ClearGraphics();
                
                // Builds the back side image (monochrome)
                // ---------------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.MonoK);

                img = g.ImageFileToByteArray(_nameZebraImage);
                g.DrawImage(ref img, 50.0f, 50.0f, 275, 200, 0);

                g.DrawLine(350, 50, 475, 120, g.IntegerFromColor(System.Drawing.Color.Black), 5.0f);
                g.DrawRectangle(500, 20, 100, 100, 5.0f, g.IntegerFromColorName("Black"));
                g.DrawEllipse(650, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Black));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Back Side: Image, Shapes, Text", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular,
                                 g.IntegerFromColor(System.Drawing.Color.DarkBlue));

                bmpBack = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Start a contactless smart card job
                // -----------------------

                if (!_isZXP7)
                    job.JobControl.CardType = GetContactlessCardType(ref job);

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.JobControl.SmartCardConfiguration(SideEnum.Front, SmartCardTypeEnum.MIFARE, true);

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpFront);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpBack);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                // Waits for the card to reach the smart card station
                // --------------------------------------------------
                string status = string.Empty;
                AtStation(ref job, actionID, 30, out status);

                // ***** Smart Card Code goes here *****


                // At the completion of smart card process
                //     if the smart card encoding was successful JobResume
                //     if the smart card encoding was Unsuccessful JobAbort
                // --------------------------------------------------------

                DialogResult result = MessageBox.Show(
                    "Job has been suspended\r\n" +
                    "   Smart Card Code is Independent of Printer Jobs\r\n" +
                    "   Yes to Resume Job\r\n" +
                    "   No to Abort Job",
                    "Smart Card Example",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);

                if (result == DialogResult.Yes)
                    JobResume(ref job);
                else
                    JobAbort(ref job, true);

                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                img = null;
                bmpFront = null;
                bmpBack = null;

                Disconnect(ref job);
            }
            return bRet;
        }

        public bool ContactSmartCardAndPrint()
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmpFront = null;
            byte[] bmpBack = null;

            Job job = null;
            ZMotifGraphics g = null;

            try
            {
                job = new Job();
                g = new ZMotifGraphics();

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!Connect(ref job))
                {
                    _msg = "Unable to open device [" + _deviceName + "]\r\n";
                    return false;
                }

                // Determines if the ZXP device supports smart card encoding
                // ---------------------------------------------------------

                if (!GetPrinterConfiguration(ref job))
                {
                    _msg = "Unable to get printer configuration";
                    return false;
                }

                if (!_isContact && !_isContactless)
                {
                    _msg = "Printer is not configured for Contact or Contactless Encoding";
                    return false;
                }

                if (_alarm != 0)
                {
                    _msg = "Printer is in alarm condition\r\n";
                    return false;
                }

                if (_isZXP7)
                    FrontImage = "ZXP7Front.bmp";
                else
                    FrontImage = "ZXP8Front.bmp";


                // Builds the front side image (color)
                // -----------------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                img = g.ImageFileToByteArray(_nameFrontImage);
                g.DrawImage(ref img, ZMotifGraphics.ImagePositionEnum.Centered, 400, 350, 0);

                g.DrawLine(160, 70, 250, 70, g.IntegerFromColor(System.Drawing.Color.Red), 5.0f);
                g.DrawRectangle(300, 20, 100, 100, 5.0f, g.IntegerFromColorName("Green"));
                g.DrawEllipse(450, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Blue));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Front Side: Image, Shapes, Text", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular, g.IntegerFromColor(System.Drawing.Color.Navy));

                int dataLen = 0;
                bmpFront = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Builds the back side image (monochrome)
                // ---------------------------------------
                
                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.MonoK);

                img = g.ImageFileToByteArray(_nameZebraImage);
                g.DrawImage(ref img, 50.0f, 50.0f, 275, 200, 0);

                g.DrawLine(350, 50, 475, 120, g.IntegerFromColor(System.Drawing.Color.Black), 5.0f);
                g.DrawRectangle(500, 20, 100, 100, 5.0f, g.IntegerFromColorName("Black"));
                g.DrawEllipse(650, 20, 100, 100, 5.0f, g.IntegerFromColor(System.Drawing.Color.Black));

                g.DrawTextString(50.0f, 580.0f, "Print 1: Back Side: Image, Shapes, Text", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular,
                                 g.IntegerFromColor(System.Drawing.Color.DarkBlue));

                bmpBack = g.CreateBitmap(out dataLen);
                g.ClearGraphics();

                // Start a contactless smart card job
                // -----------------------

                if (!_isZXP7)
                    job.JobControl.CardType = GetContactCardType(ref job);

                job.JobControl.FeederSource = _feeder;
                job.JobControl.Destination = _destination;

                job.JobControl.SmartCardConfiguration(SideEnum.Front, SmartCardTypeEnum.Contact, true);

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpFront);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0,
                                        -1, GraphicTypeEnum.BMP, bmpBack);

                int actionID = 0;
                job.PrintGraphicsLayers(1, out actionID);

                job.ClearGraphicsLayers();

                // Waits for the card to reach the smart card station
                // --------------------------------------------------
                string status;
                AtStation(ref job, actionID, 30, out status);

                // ***** Smart Card Code goes here *****


                // At the completion of smart card process
                //     if the smart card encoding was successful JobResume
                //     if the smart card encoding was Unsuccessful JobAbort
                // --------------------------------------------------------

                DialogResult result = MessageBox.Show(
                    "Job has been suspended\r\n" +
                    "   Smart Card Code is Independent of Printer Jobs\r\n" +
                    "   Yes to Resume Job\r\n" +
                    "   No to Abort Job",
                    "Smart Card Example",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);

                if (result == DialogResult.Yes)
                    JobResume(ref job);
                else
                    JobAbort(ref job, true);

                JobWait(ref job, actionID, 180, out status);
            }
            catch (Exception e)
            {
                bRet = false;
                _msg = e.Message;
            }
            finally
            {
                g.CloseGraphics();
                g = null;

                img = null;
                bmpFront = null;
                bmpBack = null;

                Disconnect(ref job);
            }
            return bRet;
        }
        #endregion

        #region Versions

        public string GetVersions()
        {
            string versions = "";

            Job            j = new Job();
            ZMotifGraphics g = new ZMotifGraphics();

            try
            {
                byte major, minor, build, revision;

                if (!Connect(ref j))
                {
                    _msg = "Unable to open device [" + _deviceName + "]";
                    return "";
                }

                if ((_alarm != 0) && (_alarm != 4016))
                {
                    _msg = "Printer is in alarm condition";
                    Disconnect(ref j);
                    return "";
                }

                g.GetSDKVersion(out major, out minor, out build, out revision);
                versions = "Graphic SDK = " + major.ToString() + "." +
                    minor.ToString() + "." +
                    build.ToString() + "." +
                    revision.ToString() + ";  ";

                j.GetSDKVersion(out major, out minor, out build, out revision);
                versions += "Printer SDK = " + major.ToString() + "." +
                    minor.ToString() + "." +
                    build.ToString() + "." +
                    revision.ToString() + ";  ";


                string fwVersion, junk;
                j.Device.GetDeviceInfo(out junk, out junk, out junk, out junk, out junk, out junk,
                    out fwVersion, out junk, out junk, out junk);

                versions += "Firmware = " + fwVersion;
            }
            catch (Exception e)
            {
                versions = "Exception: " + e.Message;
            }
            finally
            {
                g = null;
                Disconnect(ref j);
            }

            return versions;
        }

        #endregion

        #region Support

        // Waits for a smart card to be at the smart card programming station
        // --------------------------------------------------------------------------------------------------

        private void AtStation(ref Job job, int actionID, int loops, out string status)
        {
            bool timedOut = true;

            JobStatusStruct js = new JobStatusStruct();

            status = "";
            
            for (int i = 0; i < loops; i++)
            {
                try
                {
                    _alarm = job.GetJobStatus(actionID, out js.uuidJob, out js.printingStatus,
                                out js.cardPosition, out js.errorCode, out js.copiesCompleted,
                                out js.copiesRequested, out js.magStatus, out js.contactStatus,
                                out js.contactlessStatus);
                }
                catch (Exception e)
                {
                    status = "At Station Exception: " + e.Message;
                    break;
                }

                if (js.printingStatus.Contains("error") || js.printingStatus == "at_station" ||
                    js.contactStatus == "at_station" || js.contactlessStatus == "at_station")
                {
                    timedOut = false;
                    break;
                }
                Thread.Sleep(1000);
            }
            if (timedOut)
                status = "At Station Timed Out";
        }

        // Gets the card types that the printer supports
        // --------------------------------------------------------------------------------------------------

        public bool GetCardTypeList(ref Job job)
        {
            _cardTypeList = null;

            try
            {
                job.JobControl.GetAvailableCardTypes(out _cardTypeList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetContactlessCardType(ref Job job)
        {
            try
            {
                object cardTypes = null;

                job.JobControl.GetAvailableCardTypes(out cardTypes);

                System.Collections.ArrayList arrCardTypes = System.Collections.ArrayList.Adapter((string[])cardTypes);

                foreach (string card in arrCardTypes)
                {
                    if (card.ToUpper().Contains("MIFARE"))
                        return card;
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        private string GetContactCardType(ref Job job)
        {
            try
            {
                object cardTypes = null;

                job.JobControl.GetAvailableCardTypes(out cardTypes);

                System.Collections.ArrayList arrCardTypes = System.Collections.ArrayList.Adapter((string[])cardTypes);

                foreach (string card in arrCardTypes)
                {
                    if (card.Contains("SLE"))
                        return card;
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        private string GetMagneticCardType(ref Job job)
        {
            try
            {
                object cardTypes = null;

                job.JobControl.GetAvailableCardTypes(out cardTypes);

                System.Collections.ArrayList arrCardTypes = System.Collections.ArrayList.Adapter((string[])cardTypes);

                foreach (string card in arrCardTypes)
                {
                    if (card.ToUpper().Contains("HICO"))
                        return card;
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        // Gets a list of ZMotif devices
        //     ConnectionTypeEnum { USB, Ethernet, All }
        // --------------------------------------------------------------------------------------------------

        public bool GetDeviceList(bool USB)
        {
            bool bRet = true;
            Job  job  = new Job();

            try
            {
                if (USB)
                    job.GetPrinters(ConnectionTypeEnum.USB, out _deviceList);
                else
                    job.GetPrinters(ConnectionTypeEnum.Ethernet, out _deviceList); 
            }
            catch
            {
                _deviceList = null;
                bRet = false;
            }

            Disconnect(ref job);
            return bRet;
        }

        // Gets the printer configuration
        // --------------------------------------------------------------------------------------------------

        private bool GetPrinterConfiguration(ref Job j)
        {
            bool bRet = true;

            _isContact = _isContactless = _isMag = false;

            try
            {
                string headType, stripeLocation;
                j.Device.GetMagneticEncoderConfiguration(out headType, out stripeLocation);
                if (headType != "none" && headType != "")
                    _isMag = true;

                string commChannel, contact, contactless;
                j.Device.GetSmartCardConfiguration(out commChannel, out contact, out contactless);

                if (contact != "" && contact != "none")
                    _isContact = true;

                if (contactless != "" && contactless != "none")
                    _isContactless = true;
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

        // Loads a byte array with image data from a file
        // --------------------------------------------------------------------------------------------------

        private byte[] ImageToByteArray(string filename)
        {
            Image img = System.Drawing.Image.FromFile(filename);

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        // Aborts a suspended job
        // --------------------------------------------------------------------------------------------------

        private bool JobAbort(ref Job job, bool eject)
        {
            try
            {
                job.JobAbort(eject);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Waits for a job to complete
        // --------------------------------------------------------------------------------------------------

        public void JobWait(ref Job job, int actionID, int loops, out string status)
        {
            status = string.Empty;

            try
            {
                JobStatusStruct js = new JobStatusStruct();

                while (loops > 0)
                {
                    try
                    {
                        _alarm = job.GetJobStatus(actionID, out js.uuidJob, out js.printingStatus,
                                    out js.cardPosition, out js.errorCode, out js.copiesCompleted,
                                    out js.copiesRequested, out js.magStatus, out js.contactStatus,
                                    out js.contactlessStatus);

                        if (js.printingStatus == "done_ok" || js.printingStatus == "cleaning_up" )
                        {
                            status = js.printingStatus + ": " + "Indicates a job completed successfully";
                            break;
                        }
                        else if (js.printingStatus.Contains("cancelled"))
                        {
                            status = js.printingStatus;
                            break;
                        }

                        if (js.contactStatus.ToLower().Contains("error"))
                        {
                            status = js.contactStatus;
                            break;
                        }

                        if (js.printingStatus.ToLower().Contains("error"))
                        {
                            status = "Printing Status Error";
                            break;
                        }

                        if (js.contactlessStatus.ToLower().Contains("error"))
                        {
                            status = js.contactlessStatus;
                            break;
                        }

                        if (js.magStatus.ToLower().Contains("error"))
                        {
                            status = js.magStatus;
                            break;
                        }

                        if (_alarm != 0 && _alarm != 4016) //no error or out of cards
                        {
                            status = "Error: " + job.Device.GetStatusMessageString(_alarm);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        status = "Job Wait Exception: " + e.Message;
                        break;
                    }

                    if (_alarm == 0)
                    {
                        if (--loops <= 0)
                        {
                            status = "Job Status Timeout";
                            break;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            finally
            {
                _msg = status;
            }
        }


        // Resumes a suspended job
        // --------------------------------------------------------------------------------------------------

        private bool JobResume(ref Job job)
        {
            try
            {
                job.JobResume();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
