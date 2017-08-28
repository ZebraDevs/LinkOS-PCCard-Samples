using System;
using System.Runtime.InteropServices;
using System.Text;
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
File: ZBRPrinter.cs
Description: A wrapper class for the ZXP Series 3's Printing and Magenetic Encoding functionality.
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/
namespace CSharpCardIDPrintApplicationSample
{
    public class ZBRPrinter
    {
        #region Private Constants

        private const int LOW_COERCIVITY = 0;
        private const int HIGH_COERCIVITY = 1;

        #endregion //Private Constants

        #region Private Variables

        private IntPtr _handle;    // device context
        private int _prnType;

        private ASCIIEncoding _theAsciiEncoder = null;

        #endregion //Private Variables

        #region Printer DLLImports
        /**************************************************************************************************
        * Purpose: Importation of required ZBRPrinter.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Functions creation.
        ***************************************************************************************************/
        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetSDKVer", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void ZBRPRNGetSDKVer(out int major, out int minor, out int engLevel);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRGetHandle", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRGetHandle(out IntPtr _handle, byte[] drvName, out int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRCloseHandle", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRCloseHandle(IntPtr _handle, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetIPAddress", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetIPAddress([MarshalAs(UnmanagedType.LPStr)]string PrinterName, byte[] IPAddress);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNEjectCard", CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern int ZBRPRNEjectCard(IntPtr _handle, int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSetEncoderCoercivity", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNSetEncoderCoercivity(IntPtr _handle, int prn_type, int coercivity, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNReadMag", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNReadMag(IntPtr _handle, int prn_type, int trksToRead, byte[] trk1Buf,
            out int trk1BytesNeeded, byte[] trk2Buf, out int trk2BytesNeeded, byte[] trk3Buf,
            out int trk3BytesNeeded, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNPrintPrnFile", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNPrintPrnFile(IntPtr _handle, int prn_type, [MarshalAs(UnmanagedType.LPStr)]string fileName, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNReadMagByTrk", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNReadMagByTrk(IntPtr _handle, int prn_type, int trkNumb,
                                             byte[] trkBuf, out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNWriteMag", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNWriteMag(IntPtr _handle, int prn_type, int trksToWrite, byte[] trk1Data,
            byte[] trk2Data, byte[] trk3Data, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterSerialNumber", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPrinterSerialNumber(IntPtr _handle, int prn_type, byte[] serialNo,
            out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrintHeadSerialNumber", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPrintHeadSerialNumber(IntPtr _handle, int prn_type, byte[] serialNo,
            out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSendCmdEx", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNSendCmdEx(IntPtr _handle, int prn_type, byte[] cmd, byte[] response,
            out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNResetPrinter", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNResetPrinter(IntPtr _handle, int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNClrErrStatusLn", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNClrErrStatusLn(IntPtr _handle, int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetCleaningParam", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetCleaningParam(IntPtr _handle, int prn_type, out int ribbonPanelCounter,
            out int cleanCounter, out int cleanCardCounter, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSetCleaningParam", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNSetCleaningParam(IntPtr _handle, int prn_type, int ribbonPanelCounter,
            int cleanCardCounter, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterOptions", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPrinterOptions(IntPtr _handle, int prn_type, byte[] options,
            out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNIsPrinterReady", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNIsPrinterReady(IntPtr _handle, int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterStatus", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPrinterStatus(out int statusCode);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNUpgradeFirmware", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNUpgradeFirmware(IntPtr _handle, int prn_type, [MarshalAs(UnmanagedType.LPStr)]string filename, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPanelsRemaining", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPanelsRemaining(IntPtr _handle, int prn_type, out int panelsRemaining, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPanelsPrinted", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPanelsPrinted(IntPtr _handle, int prn_type, out int panelsPrinted, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrintCount", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNGetPrintCount(IntPtr _handle, int prn_type, out int printCount, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMovePrintReady", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNMovePrintReady(IntPtr _handle, int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMoveCardFwd", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNMoveCardFwd(IntPtr _handle, int prn_type, int steps, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMoveCardBkwd", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNMoveCardBkwd(IntPtr _handle, int prn_type, int steps, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNPrintTestCard", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNPrintTestCard(IntPtr _handle, int prn_type, int cardType, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNStartSmartCard", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNStartSmartCard(IntPtr _handle, int printerType, int cardType, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNEndSmartCard", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNEndSmartCard(IntPtr _handle, int printerType, int cardType, int movement, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSetCardFeedingMode", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ZBRPRNSetCardFeedingMode(IntPtr _handle, int printerType, int mode, out int err);

        #endregion //Printer DLLImports

        #region Constructor
        /**************************************************************************************************
        * Function Name: ZBRPrinter
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
        public ZBRPrinter()
        {
            _prnType = 0;
            _theAsciiEncoder = new ASCIIEncoding();
        }

        /**************************************************************************************************
        * Function Name: ~ZBRPrinter
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
        ~ZBRPrinter()
        {
            _prnType = 0;
            _theAsciiEncoder = null;
        }

        #endregion

        #region "Properties"

        /**************************************************************************************************
        * Property Name: IsHandleValid
        * 
        * Purpose: Determines if the current printer driver handle is valid 
        * 
        * Parameters: None
        * 
        * Returns:  true = valid handle
        *          false = invalid handle
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public bool IsHandleValid
        {
            get
            {
                return (_handle != IntPtr.Zero);
            }
        }

        /**************************************************************************************************
        * Property Name: IsHandleValid
        * 
        * Purpose: returns the current printer driver handle 
        * 
        * Parameters: None
        * 
        * Returns:  IntPtr containing the handle to printer driver
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public IntPtr Handle
        {
            get { return _handle; }
        }

        /**************************************************************************************************
        * Property Name: PrinterType
        * 
        * Purpose: returns an integer value representing the current printer model 
        * 
        * Parameters: None
        * 
        * Returns:  integer value containing the current printer model 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int PrinterType
        {
            get { return _prnType; }
        }
        #endregion //Properties

        #region SDK Wrapper functions - Public interface

        #region Get Printer SDK version
        /**************************************************************************************************
        * Function Name: GetPrinterSDKVersion
        * 
        * Purpose: To create a public wrapper method for ZBRPRNGetSDKVer API. 
        *           API returns the ZBRPrinter.dll's version. 
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
        private void GetPrinterSDKVersion(out int major, out int minor, out int engLevel)
        {
            ZBRPRNGetSDKVer(out major, out minor, out engLevel);
        }

        /**************************************************************************************************
        * Function Name: GetPrinterSDKVersion
        * 
        * Purpose: To return the ZBRPrinter.dll's version. 
        *           
        * 
        * Parameters: None   
        *                 
        * Returns: Success = string containing the ZBRPrinter.dll's version
        *             Fail = error or exception message   
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public string GetPrinterSDKVersion()
        {
            string version = string.Empty;
            try
            {
                int engLevel = 0;
                int major = 0;
                int minor = 0;

                GetPrinterSDKVersion(out major, out minor, out engLevel);

                if ((major + minor + engLevel) > 0)
                    version = major.ToString() + "." + minor.ToString() + "." + engLevel.ToString();
                else
                    version = "Failed to retrieve Printer SDK version";
            }
            catch (Exception ex)
            {
                version = "GetPrinterSDKVersion threw exception: " + ex.ToString();
            }
            return version;
        }
        #endregion //Get Printer SDK version

        #region Handle
        /**************************************************************************************************
        * Function Name: Open
        * 
        * Purpose: To retrieve a handle to the printer driver for a specific printer. 
        *           
        * 
        * Parameters:  printerName = string containing the printer name
        *                   errMsg = string containing an error message if error encountered
        *                 
        * Returns: true = handle retrieved
        *         false = failed to retrieve handle 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public bool Open(string printerName, out string errMsg)
        {
            byte[] drvName = null;
            int errValue = 0;
            errMsg = string.Empty;
            try
            {
                drvName = _theAsciiEncoder.GetBytes(printerName);

                int result = ZBRGetHandle(out _handle, drvName, out _prnType, out errValue);

                if ((result == 1) && (errValue == 0))
                    return true;
                else
                    errMsg = "Open failed. Error = " + Convert.ToString(errValue);
            }
            catch (Exception ex)
            {
                errMsg = "Open threw exception: " + ex.Message;
            }
            finally
            {
                drvName = null;
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: Close
        * 
        * Purpose: To release a handle to the printer driver for a specific printer. 
        *           
        * 
        * Parameters:  errMsg = string containing an error message if error encountered
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public void Close(out string errMsg)
        {
            int errValue = 0;
            errMsg = string.Empty;
            try
            {
                if (_handle != IntPtr.Zero)
                {
                    int result = ZBRCloseHandle(_handle, out errValue);

                    if ((result == 0) || (errValue > 0))
                        errMsg = "Close failed. Error = " + Convert.ToString(errValue);
                }
            }
            finally
            {
                _prnType = 0;
                _handle = IntPtr.Zero;
            }
        }
        #endregion //Handle

        #region GetIPAddres
        /**************************************************************************************************
        * Function Name: GetIPAddress
        * 
        * Purpose: To retrieve the IP Address for a specific ethernet-connected printer. 
        *           
        * 
        * Parameters:  printerName = string containing the printer name
        *                ipAddress = string containing the ip address
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        * 02/09/2011       ACT             Increased buffer size to 50;
        ***************************************************************************************************/
        public int GetIPAddress(string printerName, out string ipAddress)
        {
            ipAddress = "";
            byte[] ip = null;
            System.Text.ASCIIEncoding a = null;
            try
            {
                ip = new byte[50];

                int result = ZBRPRNGetIPAddress(printerName, ip);
                if (result == 1)
                {
                    a = new ASCIIEncoding();
                    ipAddress = a.GetString(ip, 0, ip.Length - 1);

                    int indx = ipAddress.IndexOf('\0');
                    if (indx > 0)
                        ipAddress = ipAddress.Substring(0, indx);
                }
                return result;
            }
            catch
            {
            }
            finally
            {
                ip = null;
                a = null;
            }
            return 0;
        }
        #endregion //GetIPAddres

        #region Send Commands
        /**************************************************************************************************
        * Function Name: SendCmdEx
        * 
        * Purpose: To send a low-level command to a specific printer. 
        *           
        * 
        * Parameters:      cmd = byte array containing the command
        *             errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int SendCmdEx(byte[] cmd, out int errValue)
        {
            errValue = 0;
            byte[] response = null;
            try
            {
                response = new byte[50];
                int respSize = 0;

                return ZBRPRNSendCmdEx(_handle, _prnType, cmd, response, out respSize, out errValue);
            }
            catch
            {
            }
            finally
            {
                response = null;
            }
            return 0;
        }

        /**************************************************************************************************
        * Function Name: SendCommand
        * 
        * Purpose: To send a low-level command to a specific printer & return its response. 
        *           
        * 
        * Parameters:      cmd = byte array containing the command
        *             response = byte array containing the response to the command
        *             respSize = integer containing the response size in bytes   
        *             errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int SendCommand(byte[] cmd, out byte[] response, out int respSize, out int errValue)
        {
            errValue = 0;
            try
            {
                response = new byte[50];
                respSize = 0;

                return ZBRPRNSendCmdEx(_handle, _prnType, cmd, response, out respSize, out errValue);
            }
            catch
            {
                response = null;
                respSize = 0;
            }
            return 0;
        }

        /**************************************************************************************************
        * Function Name: SendCommandToPrinter
        * 
        * Purpose: To send a low-level command to a specific printer & return its response. 
        *           
        * 
        * Parameters:      cmd = byte array containing the command
        *               errMsg = int containing the error code if error encountered
        *                 
        * Returns: Success = string containing the command response
        *           Failed = empty string
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private string SendCommandToPrinter(string cmd, out string errMsg)
        {
            string Response = string.Empty;
            errMsg = string.Empty;

            byte[] response = null;
            System.Text.ASCIIEncoding asc = null;

            try
            {
                int responseSize = 0;
                int err = 0;

                response = new byte[50];
                asc = new System.Text.ASCIIEncoding();

                ZBRPRNSendCmdEx(_handle, _prnType, asc.GetBytes(cmd), response, out responseSize, out err);
                if (err == 0)
                {
                    Response = asc.GetString(response, 0, responseSize - 1);
                }
                else errMsg = "Error code: " + Convert.ToString(err);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                Response = string.Empty;
            }
            finally
            {
                asc = null;
                response = null;
            }
            return Response;
        }
        #endregion //Send Commands

        #region Update Firmware
        /**************************************************************************************************
        * Function Name: UpdateFirmware
        * 
        * Purpose: To update a printer's firmware. 
        *           
        * 
        * Parameters:    fName = binary file containing the new firmware 
        *             errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int UpdateFirmware(string fName, out int errValue)
        {
            errValue = 0;
            try
            {
                return ZBRPRNUpgradeFirmware(_handle, _prnType, fName, out errValue);
            }
            catch
            {
            }
            return 0;
        }
        #endregion //Update Firmware

        #region Print Prn File
        /**************************************************************************************************
        * Function Name: PrintPrnFile
        * 
        * Purpose: To print a .prn file. 
        *           
        * 
        * Parameters:    fName = .prn file to print 
        *             errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int PrintPrnFile(string fName, out int err)
        {
            return ZBRPRNPrintPrnFile(_handle, _prnType, fName, out err);
        }
        #endregion //Print Prn File

        #region Reset Printer
        /**************************************************************************************************
        * Function Name: ResetPrinter
        * 
        * Purpose: To send the reset command to a printer. 
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int ResetPrinter(out int errValue)
        {
            return ZBRPRNResetPrinter(_handle, _prnType, out errValue);
        }
        #endregion // Reset Printer

        #region ClearError
        /**************************************************************************************************
        * Function Name: ClearError
        * 
        * Purpose: To clear an error from a printer's display. 
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int ClearError(out int errValue)
        {
            return ZBRPRNClrErrStatusLn(_handle, _prnType, out errValue);
        }
        #endregion //ClearError

        #region Command Facade
        /**************************************************************************************************
        * Function Name: SetMagneticCoercivityHigh
        * 
        * Purpose: To set a printer's magnetic encoder to high coercivity level  
        *           
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 01/27/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int SetMagneticCoercivityHigh(out int error)
        {
            return ZBRPRNSetEncoderCoercivity(_handle, _prnType, HIGH_COERCIVITY, out error);
        }

        /**************************************************************************************************
        * Function Name: SetMagneticCoercivityLow
        * 
        * Purpose: To set a printer's magnetic encoder to low coercivity level  
        *           
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 01/27/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int SetMagneticCoercivityLow(out int error)
        {
            return ZBRPRNSetEncoderCoercivity(_handle, _prnType, LOW_COERCIVITY, out error);
        }

        /**************************************************************************************************
        * Function Name: DrawCardFromHopper
        * 
        * Purpose: To place a card into printer from the hopper 
        *           
        * 
        * Parameters:  errMsg = string containing an error message if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public string DrawCardFromHopper(out string errMsg)
        {
            try
            {
                return SendCommandToPrinter("MI", out errMsg);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return string.Empty;
        }
        #endregion //Command Facade

        #region Card Movement
        /**************************************************************************************************
        * Function Name: EjectCard
        * 
        * Purpose: To eject a card from the printer 
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public int EjectCard(out int errValue)
        {
            return ZBRPRNEjectCard(_handle, _prnType, out errValue);
        }

        /**************************************************************************************************
        * Function Name: MoveCardForward
        * 
        * Purpose: To move a card within the printer a number of steps forward 
        *           
        * 
        * Parameters:     steps = int containing the number of steps to move forward
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveCardForward(int steps, out int errValue)
        {
            return ZBRPRNMoveCardFwd(_handle, _prnType, steps, out errValue);
        }

        /**************************************************************************************************
        * Function Name: MoveCardBackward
        * 
        * Purpose: To move a card within the printer a number of steps backward 
        *           
        * 
        * Parameters:     steps = int containing the number of steps to move backward
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveCardBackward(int steps, out int errValue)
        {
            return ZBRPRNMoveCardBkwd(_handle, _prnType, steps, out errValue);
        }

        /**************************************************************************************************
        * Function Name: MoveCardToPrintReadyPosition
        * 
        * Purpose: To position a card for printing 
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveCardToPrintReadyPosition(out int errValue)
        {
            return ZBRPRNMovePrintReady(_handle, _prnType, out errValue);
        }

        /**************************************************************************************************
        * Function Name: MoveCardToMagEncoderPosition
        * 
        * Purpose: To position a card for magnetic encoding 
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveCardToMagEncoderPosition(out int err)
        {
            return ZBRPRNMovePrintReady(_handle, _prnType, out err);
        }

        /**************************************************************************************************
        * Function Name: MoveCardToSmartCardEncoder
        * 
        * Purpose: To position a card for smartcard encoding
        *           
        * 
        * Parameters:  cardType = int containing contact or contactless card type
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveCardToSmartCardEncoder(int cardType, out int err)
        {
            return ZBRPRNStartSmartCard(_handle, _prnType, cardType, out err);
        }

        /**************************************************************************************************
        * Function Name: MoveSmartCardToPrintReadyPosition
        * 
        * Purpose: To position a card for printing after smartcard encoding
        *           
        * 
        * Parameters:  cardType = int containing contact or contactless card type
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 08/01/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int MoveSmartCardToPrintReadyPosition(int cardType, out int error)
        {
            return ZBRPRNEndSmartCard(_handle, _prnType, cardType, 0, out error);
        }
        #endregion //Card Movement

        #region Ribbon Panel Status
        /**************************************************************************************************
        * Function Name: GetPanelsRemaining
        * 
        * Purpose: To return the number of remaining panels for a printer's ribbon
        *           
        * 
        * Parameters:  panels = int containing remaining panels count
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetPanelsRemaining(out int panels, out int errValue)
        {
            return ZBRPRNGetPanelsRemaining(_handle, _prnType, out panels, out errValue);
        }

        /**************************************************************************************************
        * Function Name: GetPanelsPrinted
        * 
        * Purpose: To return the number of printed panels for a printer's ribbon
        *           
        * 
        * Parameters:  panels = int containing printed panels count
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetPanelsPrinted(out int panels, out int errValue)
        {
            return ZBRPRNGetPanelsPrinted(_handle, _prnType, out panels, out errValue);
        }
        #endregion //Ribbon Panel Status

        #region Get/Set Cleaning Parameters
        /**************************************************************************************************
        * Function Name: GetCleaningParameters
        * 
        * Purpose: To return the printer's current cleaning parameters
        *           
        * 
        * Parameters:  ImageCounter = int containing the number of images which have passed 
        *                             the printhead (image passes) while it was in the "head down" position
        *              CleanCounter = int containing the number of image passes before a cleaning
        *                             alert is sent 
        *               CardCounter = int containing the number of card passes during a cleaning  
        *                  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetCleaningParameters(out int ImageCounter, out int CleanCounter,
                                         out int CardCounter, out int errValue)
        {
            return ZBRPRNGetCleaningParam(_handle, _prnType, out ImageCounter, out CleanCounter,
                                          out CardCounter, out errValue);
        }

        /**************************************************************************************************
        * Function Name: SetCleaningParameters
        * 
        * Purpose: To assign the printer's current cleaning parameters
        *           
        * 
        * Parameters:  RibbonPanelCounter = int containing the number of panels printed before
        *                                   a cleaning is required
        *                   CleanCardPass = int containing the number of card passes during a cleaning  
        *                        errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int SetCleaningParameters(int RibbonPanelCounter, int CardCounter, out int errValue)
        {
            return ZBRPRNSetCleaningParam(_handle, _prnType, RibbonPanelCounter, CardCounter, out errValue);
        }
        #endregion //Get/Set Cleaning Parameters

        #region Cards Printed
        /**************************************************************************************************
        * Function Name: GetCardsPrintedCount
        * 
        * Purpose: To return the number of cards printed
        *           
        * 
        * Parameters:  CardsPrinted = int containing the number of card printed
        *                  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetCardsPrintedCount(out int CardsPrinted, out int err)
        {
            return ZBRPRNGetPrintCount(_handle, _prnType, out CardsPrinted, out err);
        }
        #endregion //Cards Printed

        #region Print Test Card
        /**************************************************************************************************
        * Function Name: PrintStandardTestCard
        * 
        * Purpose: To print the Zebra Standard Test Card
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int PrintStandardTestCard(out int errValue)
        {
            const int STANDARD_CARD = 0;
            errValue = 0;

            return ZBRPRNPrintTestCard(_handle, _prnType, STANDARD_CARD, out errValue);
        }
        #endregion //Print Test Card

        #region Card Feeder Mode
        /**************************************************************************************************
        * Function Name: SetCardfeedMode
        * 
        * Purpose: To assign a printer's card feeding mode.
        *           Supported modes: hopper & atm  
        *           
        * 
        * Parameters:      mode = int containing the feeder mode 
        *              errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed 
        * 
        * History:
        * Date             Who             Comment
        * 12/12/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int SetCardfeedMode(int mode, out int errValue)
        {
            return ZBRPRNSetCardFeedingMode(_handle, _prnType, mode, out errValue);
        }
        #endregion //Card Feeder Mode

        #region Printer Status
        /**************************************************************************************************
        * Function Name: IsPrinterReady
        * 
        * Purpose: To determine if a printer is available to receive a job  
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns:  true = printer is available
        *          false = printer is busy 
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public bool IsPrinterReady(out int errValue)
        {
            int result = ZBRPRNIsPrinterReady(_handle, _prnType, out errValue);
            if ((result > 0) && (errValue == 0))
                return true;
            return false;
        }

        /**************************************************************************************************
        * Function Name: IsPrinterInErrorMode
        * 
        * Purpose: To determine if a printer is in an error state  
        *           
        * 
        * Parameters:  errValue = int containing the error code if error encountered
        *                 
        * Returns:  true = printer is in error state
        *          false = printer is not in error state  
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public bool IsPrinterInErrorMode(out int errValue)
        {
            ZBRPRNGetPrinterStatus(out errValue);
            if (errValue == 0)
                return false;
            return true;
        }
        #endregion //Printer Status

        #region Magnetic Encoding
        /**************************************************************************************************
        * Function Name: SetMagneticCoercivity
        * 
        * Purpose: To set a printer's magnetic encoder's coercivity level  
        *          Supported levels: high & low coercivity
        * 
        * Parameters:  coercivity = int containing the coercivity level
        *                errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int SetMagneticCoercivity(int coercivity, out int error)
        {
            return ZBRPRNSetEncoderCoercivity(_handle, _prnType, coercivity, out error);
        }

        /**************************************************************************************************
        * Function Name: ReadMagneticTracks
        * 
        * Purpose: To read 1 or more magnetic tracks from a card.
        * 
        * Parameters:  trksToRead = int containing the track(s) to read
        *                 trkBuf1 = byte array containing the data read from track 1 
        *                 trkBuf2 = byte array containing the data read from track 2
        *                 trkBuf3 = byte array containing the data read from track 3   
        *                errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int ReadMagneticTracks(int trksToRead, byte[] trkBuf1, byte[] trkBuf2, byte[] trkBuf3, out int errValue)
        {
            int size1, size2, size3;
            return ZBRPRNReadMag(_handle, _prnType, trksToRead, trkBuf1, out size1, trkBuf2, out size2,
                                 trkBuf3, out size3, out errValue);
        }

        /**************************************************************************************************
        * Function Name: ReadMagneticTracks
        * 
        * Purpose: To read data from 1 or more magnetic tracks of a card.
        * 
        * Parameters:  trksToRead = int containing the track(s) to read
        *                   track1 = string containing the data to be read from track 1 
        *                   track2 = string containing the data to be read from track 2 
        *                   track3 = string containing the data to be read from track 3  
        *                   errMsg = string containing an error message if error encountered
        * 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 01/17/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int ReadMagneticTracks(int trksToRead, out string track1, out string track2, out string track3, out string errMsg)
        {
            byte[] trkBuf1 = null;
            byte[] trkBuf2 = null;
            byte[] trkBuf3 = null;

            track1 = string.Empty;
            track2 = string.Empty;
            track3 = string.Empty;
            errMsg = string.Empty;
            try
            {
                int errValue = 0;
                int trkBuf1Size = 0;
                int trkBuf2Size = 0;
                int trkBuf3Size = 0;

                trkBuf1 = new byte[50];
                trkBuf2 = new byte[50];
                trkBuf3 = new byte[50];

                int result = ZBRPRNReadMag(_handle, _prnType, trksToRead,
                                           trkBuf1, out trkBuf1Size,
                                           trkBuf2, out trkBuf2Size,
                                           trkBuf3, out trkBuf3Size, out errValue);
                if ((result == 1) || (errValue == 0))
                {
                    track1 = _theAsciiEncoder.GetString(trkBuf1, 0, trkBuf1Size);
                    track2 = _theAsciiEncoder.GetString(trkBuf2, 0, trkBuf2Size);
                    track3 = _theAsciiEncoder.GetString(trkBuf3, 0, trkBuf3Size);
                }
                else
                {
                    errMsg = "ReadMagnetic tracks failed. Error = " + Convert.ToString(errValue);
                }
                return result;
            }
            catch (Exception ex)
            {
                track1 = string.Empty;
                track2 = string.Empty;
                track3 = string.Empty;
                errMsg = "ReadMagneticTracks threw exception: " + ex.Message;
            }
            finally
            {
                trkBuf1 = null;
                trkBuf2 = null;
                trkBuf3 = null;
            }
            return 0;
        }

        /**************************************************************************************************
        * Function Name: ReadMagneticTrack
        * 
        * Purpose: To read 1 magnetic track from a card.
        * 
        * Parameters:        trackNo = int containing the track to read
        *                  trackData = byte array containing the data read from the track 
        *              trackDataSize = int containing the number of bytes read from the track  
        *                   errValue = int containing the error code if error encountered
        *                 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int ReadMagneticTrack(int trackNo, byte[] trackData, out int trackDataSize, out int errValue)
        {
            return ZBRPRNReadMagByTrk(_handle, _prnType, trackNo, trackData, out trackDataSize, out errValue);
        }

        /**************************************************************************************************
        * Function Name: WriteMagneticTracks
        * 
        * Purpose: To write data to 1 or more magnetic tracks of a card.
        * 
        * Parameters:  trksToWrite = int containing the track(s) to write
        *                  trkBuf1 = byte array containing the data to be written to track 1 
        *                  trkBuf2 = byte array containing the data to be written to track 2
        *                  trkBuf3 = byte array containing the data to be written to track 3   
        *                 errValue = int containing the error code if error encountered
        * 
        * Note: if data for a track is set to null or "", data is not writen to the track                
        *            
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int WriteMagneticTracks(int trksToWrite, byte[] trkBuf1, byte[] trkBuf2, byte[] trkBuf3, out int errValue)
        {
            return ZBRPRNWriteMag(_handle, _prnType, trksToWrite, trkBuf1, trkBuf2, trkBuf3, out errValue);
        }

        /**************************************************************************************************
        * Function Name: WriteMagneticTracks
        * 
        * Purpose: To write data to 1 or more magnetic tracks of a card.
        * 
        * Parameters:  trksToWrite = int containing the track(s) to write
        *                   track1 = string containing the data to be written to track 1 
        *                   track2 = string containing the data to be written to track 2 
        *                   track3 = string containing the data to be written to track 3   
        *                   errMsg = string containing an error message if error encountered
        * 
        * Note: if data for a track is set to null or "", data is not writen to the track                
        *            
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 01/17/2011       ACT             Function creation.
        ***************************************************************************************************/
        public int WriteMagneticTracks(int trksToWrite, string track1, string track2, string track3, out string errMsg)
        {
            errMsg = string.Empty;
            byte[] trkBuf1 = null;
            byte[] trkBuf2 = null;
            byte[] trkBuf3 = null;
            try
            {
                int errValue = 0;

                trkBuf1 = _theAsciiEncoder.GetBytes(track1);
                trkBuf2 = _theAsciiEncoder.GetBytes(track2);
                trkBuf3 = _theAsciiEncoder.GetBytes(track3);

                int result = ZBRPRNWriteMag(_handle, _prnType, trksToWrite, trkBuf1, trkBuf2, trkBuf3, out errValue);
                if ((result != 1) || (errValue > 0))
                {
                    errMsg = "WriteMagnetic tracks failed. Error = " + Convert.ToString(errValue);
                }
                return result;
            }
            catch (Exception ex)
            {
                errMsg = "WriteMagneticTracks threw exception: " + ex.Message;
            }
            finally
            {
                trkBuf1 = null;
                trkBuf2 = null;
                trkBuf3 = null;
            }
            return 0;
        }
        #endregion //Magnetic Encoding

        #region Printer/Printer Head Serial Number
        /**************************************************************************************************
        * Function Name: GetPrinterSerialNumber
        * 
        * Purpose: To retrieve a printer's serial number.
        * 
        * Parameters:  errMsg = string containing error message if error encountered
        * 
        * Returns: success = string containing printer serial number
        *           failed = empty string   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public string GetPrinterSerialNumber(out string errMsg)
        {
            string serialNo = string.Empty;

            GetPrinterSerialNumber(ref serialNo, out errMsg);

            return serialNo;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterSerialNumber
        * 
        * Purpose: To retrieve a printer's serial number.
        * 
        * Parameters:  SerialNo = string containing printer serial number   
        *                errMsg = string containing error message if error encountered
        * 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetPrinterSerialNumber(ref string SerialNo, out string errMsg)
        {
            byte[] serialNo = null;
            System.Text.ASCIIEncoding asc = null;
            int err = 0;
            errMsg = string.Empty;
            try
            {
                int serialNoSize = 0;

                serialNo = new byte[50];

                ZBRPRNGetPrinterSerialNumber(_handle, _prnType, serialNo, out serialNoSize, out err);
                if (err == 0)
                {
                    asc = new System.Text.ASCIIEncoding();
                    SerialNo = asc.GetString(serialNo, 0, serialNoSize - 1);
                }
                else errMsg = "Error code: " + Convert.ToString(err);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SerialNo = string.Empty;
            }
            finally
            {
                asc = null;
                serialNo = null;
            }
            return err;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterHeadSerialNumber
        * 
        * Purpose: To retrieve a printer's print-head serial number.
        * 
        * Parameters:  errValue = string containing error message if error encountered
        * 
        * Returns: success = string containing print-head serial number
        *           failed = empty string   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public string GetPrinterHeadSerialNumber(out string errMsg)
        {
            string serialNo = string.Empty;

            GetPrinterHeadSerialNumber(ref serialNo, out errMsg);

            return serialNo;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterHeadSerialNumber
        * 
        * Purpose: To retrieve a printer's print-head serial number.
        * 
        * Parameters:  SerialNo = string containing print-head serial number   
        *                errMsg = string containing error message if error encountered
        * 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetPrinterHeadSerialNumber(ref string SerialNo, out string errMsg)
        {
            byte[] serialNo = null;
            System.Text.ASCIIEncoding asc = null;
            int err = 0;
            errMsg = string.Empty;
            try
            {
                int serialNoSize = 0;

                serialNo = new byte[50];

                ZBRPRNGetPrintHeadSerialNumber(_handle, _prnType, serialNo, out serialNoSize, out err);
                if (err == 0)
                {
                    asc = new System.Text.ASCIIEncoding();
                    SerialNo = asc.GetString(serialNo, 0, serialNoSize - 1);
                }
                else errMsg = "Error code: " + Convert.ToString(err);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SerialNo = string.Empty;
            }
            finally
            {
                asc = null;
                serialNo = null;
            }
            return err;
        }
        #endregion //Printer/Printer Head Serial Number

        #region Printer Configuration
        /**************************************************************************************************
        * Function Name: GetPrinterConfiguration
        * 
        * Purpose: To retrieve a printer's configuration 
        *           
        * 
        * Parameters:  errMsg = string containing an error message if error encountered
        *                 
        * Returns: Success = string containing the printer's configuration
        *           Failed = empty string
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        public string GetPrinterConfiguration(out string errMsg)
        {
            string ConfigInfo = string.Empty;
            errMsg = string.Empty;

            try
            {
                ConfigInfo = SendCommandToPrinter("V", out errMsg);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                ConfigInfo = string.Empty;
            }
            return ConfigInfo;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterConfiguration
        * 
        * Purpose: To retrieve a printer's configuration.
        * 
        * Parameters:  options = byte array containing the printer's configuration code   
        *                errMsg = string containing error message if error encountered
        * 
        * Returns: 1 = success
        *          0 = failed   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        public int GetPrinterConfiguration(byte[] options, out string errMsg)
        {
            errMsg = string.Empty;
            int err = -1;
            int size = 0;
            int result = -1;
            try
            {
                result = ZBRPRNGetPrinterOptions(_handle, _prnType, options, out size, out err);
                if (err > 0)
                    errMsg = "Failed to get printer's configuration. Error = " + Convert.ToString(err);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                result = -1;
            }
            return result;
        }
        #endregion //Printer Configuration

        #endregion //SDK Wrapper functions - Public interface

        #region "Private Utility Methods"
        /**************************************************************************************************
        * Function Name: AllocateUnmanagedArray
        * 
        * Purpose: To allocate memory for an array outside of .net's managed resources.
        * 
        * Parameters:  Size = int containing the number of bytes to allocate    
        * 
        * Returns: success = pointer to the allocated memory
        *           failed = null pointer   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        private System.IntPtr AllocateUnmanagedArray(int Size)
        {
            System.IntPtr pointer = Marshal.AllocHGlobal(Size);
            return pointer;
        }

        /**************************************************************************************************
        * Function Name: FreeUnmanagedMemory
        * 
        * Purpose: To release allocated memory.
        * 
        * Parameters:  pointer = IntPtr containing the address of the allocated memory    
        * 
        * Returns: None   
        * 
        * History:
        * Date             Who             Comment
        * 12/11/2010       ACT             Function creation.
        ***************************************************************************************************/
        private void FreeUnmanagedMemory(System.IntPtr pointer)
        {
            Marshal.FreeHGlobal(pointer);
        }
        #endregion //Private Utility Methods

    }

}