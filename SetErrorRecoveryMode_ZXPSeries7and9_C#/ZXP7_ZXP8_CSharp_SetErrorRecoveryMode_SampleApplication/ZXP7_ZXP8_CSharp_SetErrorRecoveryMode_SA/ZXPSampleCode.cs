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

namespace ZXP7_ZXP8_CSharp_SetErrorRecoveryMode_SA
{
    class ZXPSampleCode
    {
        #region Declarations

        // Local declarations
        // --------------------------------------------------------------------------------------------------

        private bool   _isZXP7           = false;
        private short  _alarm            = 0;
        private string _cardType         = string.Empty,
                       _deviceName       = string.Empty,
                       _msg              = string.Empty,
                       _nameFrontImage   = string.Empty,
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

        #region Print Graphic Samples

        public bool Perform_Set_Error_Recovery_Mode_Print(string cardType)
        {
            bool bRet = true;

            byte[] img = null;
            byte[] bmp = null;
            byte[] monoBmp = null;

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

                img = g.ImageFileToByteArray(FrontImage);

                // Builds the image to print
                // -------------------------

                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.Color);

                g.DrawImage(ref img, 50, 50, 924, 548, 0);
                g.DrawTextString(50.0f, 580.0f, "Color + Set Error Recovery Mode", "Arial", 10.0f,
                                 ZMotifGraphics.FontTypeEnum.Regular,
                                 g.IntegerFromColor(System.Drawing.Color.Black));

                int dataLen = 0;
                bmp = g.CreateBitmap(out dataLen);

                g.ClearGraphics();

                //re-init graphics for MonoK panel
                g.InitGraphics(0, 0, ZMotifGraphics.ImageOrientationEnum.Landscape,
                               ZMotifGraphics.RibbonTypeEnum.MonoK);

                //load image to be printed as MonoK panel:
                img = g.ImageFileToByteArray("Back.bmp");
                g.DrawImage(ref img, 50, 50, 924, 548, 0);

                dataLen = 0;
                monoBmp = g.CreateBitmap(out dataLen);

                // Print image on both sides
                // -------------------------

                if (!_isZXP7)
                    job.JobControl.CardType = cardType;

                job.JobControl.FeederSource = FeederSourceEnum.CardFeeder;

                if (job.Device.HasLaminator)
                    job.JobControl.Destination = DestinationTypeEnum.LaminatorAny;
                else
                    job.JobControl.Destination = DestinationTypeEnum.Eject;

                //SET ERROR RECOVERY MODE FOR PRINTER:
                job.Device.ErrorControlLevel = ErrorControlLevelEnum.EC_None;

                job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1, GraphicTypeEnum.BMP, bmp);

                job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1, GraphicTypeEnum.BMP, monoBmp);

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
