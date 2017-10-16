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
File: SmartCardSupport.cs
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSharpContactSmartCardSample
{
    /**************************************************************************************************
    * Class Name: PCSCHelperVersion
    * Function Name: GetVersion
    * 
    * Purpose: To return the Appication Framework version information. 
    *           
    * 
    * Parameters:  None   
    *                 
    * Returns: Application Framework version information.
    * 
    * History:
    * Date             Who             Comment
    * 12/12/2010       ACT             Function creation.
    ***************************************************************************************************/
    public static class PCSCHelperVersion
    {
        public static String GetVersion()
        {
            return "1.0.0.0";
        }
    }

    #region Enumerations

    public enum ATTRIB_TYPE
    {
        TYPE = 0x00,
        PROTOCOL = 0x01,
        FEATURES = 0x02,
        MEMORY = 0x03,
        PIN = 0x04,
        CR = 0x05,
        COUNTERS = 0x06,
        CLOCK = 0x07,
        BIT_ORDER = 0x08,
        CONFIGURATION = 0x09
    }

    public enum SMART_CARD_TYPE
    {
        UNKNOWN = 0,
        SLE4406 = 0x01,
        SLE4418 = 0x02,
        SLE4428 = 0x03,
        SLE4432 = 0x04,
        SLE4436 = 0x05,
        SLE4442 = 0x06,
        SLE5536 = 0x07,
        AT24C01ASC = 0x08,
        AT24C02SC = 0x09,
        AT24C04SC = 0x0A,
        AT24C08SC = 0x0B,
        AT24C16SC = 0x0C,
        AT24C32SC = 0x0D,
        AT24C64SC = 0x0E,
        AT24C128SC = 0x0F,
        AT24C256SC = 0x10,
        AT24C512SC = 0x11,
        AT88SC153 = 0x12,
        AT88SC1608 = 0x13,
        MCARDTYPE_SLE6636 = 0x19
    }
    #endregion

    /**************************************************************************************************
    * Class Name: WinSCard
    *  
    * Purpose: To provide a wrapper class for the winscard.dll functionality. 
    *           
    ***************************************************************************************************/
    public class WinSCard
    {
        #region Data Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct SCARD_IO_REQUEST
        {
            public uint dwProtocol;
            public int cbPciLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APDURec
        {
            public byte bCLA;
            public byte bINS;
            public byte bP1;
            public byte bP2;
            public byte bP3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] SW;
            public bool IsSend;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCARD_READERSTATE
        {
            public string RdrName;
            public int UserData;
            public int RdrCurrState;
            public int RdrEventState;
            public int ATRLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
            public byte ATRValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VERSION_CONTROL
        {
            public int SmclibVersion;
            public byte DriverMajor;
            public byte DriverMinor;
            public byte FirmwareMajor;
            public byte FirmwareMinor;
            public byte UpdateKey;
        }

        #endregion //Data Structures

        #region Constants
        
        public const int SCARD_S_SUCCESS = 0;
        public const int SCARD_ATR_LENGTH = 33;

        #region Memory Card Types
        
        public const int CT_MCU = 0x00;                   // MCU
        public const int CT_IIC_Auto = 0x01;               // IIC (Auto Detect Memory Size)
        public const int CT_IIC_1K = 0x02;                 // IIC (1K)
        public const int CT_IIC_2K = 0x03;                 // IIC (2K)
        public const int CT_IIC_4K = 0x04;                 // IIC (4K)
        public const int CT_IIC_8K = 0x05;                 // IIC (8K)
        public const int CT_IIC_16K = 0x06;                // IIC (16K)
        public const int CT_IIC_32K = 0x07;                // IIC (32K)
        public const int CT_IIC_64K = 0x08;                // IIC (64K)
        public const int CT_IIC_128K = 0x09;               // IIC (128K)
        public const int CT_IIC_256K = 0x0A;               // IIC (256K)
        public const int CT_IIC_512K = 0x0B;               // IIC (512K)
        public const int CT_IIC_1024K = 0x0C;              // IIC (1024K)
        public const int CT_AT88SC153 = 0x0D;              // AT88SC153
        public const int CT_AT88SC1608 = 0x0E;             // AT88SC1608
        public const int CT_SLE4418 = 0x0F;                // SLE4418
        public const int CT_SLE4428 = 0x10;                // SLE4428
        public const int CT_SLE4432 = 0x11;                // SLE4432
        public const int CT_SLE4442 = 0x12;                // SLE4442
        public const int CT_SLE4406 = 0x13;                // SLE4406
        public const int CT_SLE4436 = 0x14;                // SLE4436
        public const int CT_SLE5536 = 0x15;                // SLE5536
        public const int CT_MCUT0 = 0x16;                  // MCU T=0
        public const int CT_MCUT1 = 0x17;                  // MCU T=1
        public const int CT_MCU_Auto = 0x18;               // MCU Autodetect
        
        #endregion //Memory Card Types

        #region SCard Commands
        
        public const int SCARD_ATTR_VENDOR_NAME = 65792;
        public const int SCARD_ATTR_VENDOR_IFD_TYPE = 65793;
        public const int SCARD_ATTR_VENDOR_IFD_VERSION = 65794;
        public const int SCARD_ATTR_CHANNEL_ID = 131344;
        public const int SCARD_ATTR_DEFAULT_CLK = 196897;
        public const int SCARD_ATTR_DEFAULT_DATA_RATE = 196899;
        public const int SCARD_ATTR_MAX_CLK = 196898;
        public const int SCARD_ATTR_MAX_DATA_RATE = 196900;
        public const int SCARD_ATTR_MAX_IFSD = 196901;
        public const int SCARD_ATTR_ICC_PRESENCE = 590592;
        public const int SCARD_ATTR_ATR_STRING = 590595;
        public const int SCARD_ATTR_CURRENT_CLK = 524802;
        public const int SCARD_ATTR_CURRENT_F = 524803;
        public const int SCARD_ATTR_CURRENT_D = 524804;
        public const int SCARD_ATTR_CURRENT_N = 524805;
        public const int SCARD_ATTR_CURRENT_CWT = 524810;
        public const int SCARD_ATTR_CURRENT_BWT = 524809;
        public const int SCARD_ATTR_CURRENT_IFSC = 524807;
        #endregion //SCard Commands

        #region Context Scope

        /*===============================================================	
        ' Note: The context is a user context, and any database operations 
        '       are performed within the domain of the user.
        ===============================================================	*/
        public const int SCARD_SCOPE_USER = 0;
        
        /*===============================================================
        ' The context is that of the current terminal, and any database 
        'operations are performed within the domain of that terminal.  
        '(The calling application must have appropriate access permissions 
        'for any database actions.)
        '===============================================================*/
        public const int SCARD_SCOPE_TERMINAL = 1;
        
        /*===============================================================
        ' The context is the system context, and any database operations 
        ' are performed within the domain of the system.  (The calling
        ' application must have appropriate access permissions for any 
        ' database actions.)
        '===============================================================*/
        public const int SCARD_SCOPE_SYSTEM = 2;

        #endregion //Context Scope   
        
        #region State

        /*===============================================================
        ' The application is unaware of the current state, and would like 
        ' to know. The use of this value results in an immediate return
        ' from state transition monitoring services. This is represented
        ' by all bits set to zero.
        '===============================================================*/
        public const int SCARD_STATE_UNAWARE = 0x00;
        
        /*===============================================================
        ' The application requested that this reader be ignored. No other
        ' bits will be set.
        '===============================================================*/
        public const int SCARD_STATE_IGNORE = 0x01;
        
        /*===============================================================
        ' This implies that there is a difference between the state 
        ' believed by the application, and the state known by the Service
        ' Manager.When this bit is set, the application may assume a
        ' significant state change has occurred on this reader.
        '===============================================================*/
        public const int SCARD_STATE_CHANGED = 0x02;
        
        /*===============================================================
        ' This implies that the given reader name is not recognized by
        ' the Service Manager. If this bit is set, then SCARD_STATE_CHANGED
        ' and SCARD_STATE_IGNORE will also be set.
        '===============================================================*/
        public const int SCARD_STATE_UNKNOWN = 0x04;
        
        /*===============================================================
        ' This implies that the actual state of this reader is not
        ' available. If this bit is set, then all the following bits are
        ' clear.
        '===============================================================*/
        public const int SCARD_STATE_UNAVAILABLE = 0x08;
        
        /*===============================================================
        '  This implies that there is no card in the reader. If this bit
        '  is set, all the following bits will be clear.
         ===============================================================*/
        public const int SCARD_STATE_EMPTY = 0x10;
        
        /*===============================================================
        '  This implies that there is a card in the reader.
         ===============================================================*/
        public const int SCARD_STATE_PRESENT = 0x20;
        
        /*===============================================================
        '  This implies that there is a card in the reader with an ATR
        '  matching one of the target cards. If this bit is set,
        '  SCARD_STATE_PRESENT will also be set.  This bit is only returned
        '  on the SCardLocateCard() service.
         ===============================================================*/
        public const int SCARD_STATE_ATRMATCH = 0x40;
        
        /*===============================================================
        '  This implies that the card in the reader is allocated for 
        '  exclusive use by another application. If this bit is set,
        '  SCARD_STATE_PRESENT will also be set.
         * ===============================================================*/
        public const int SCARD_STATE_EXCLUSIVE = 0x80;
        
        /*===============================================================
        '  This implies that the card in the reader is in use by one or 
        '  more other applications, but may be connected to in shared mode. 
        '  If this bit is set, SCARD_STATE_PRESENT will also be set.
         ===============================================================*/
        public const int SCARD_STATE_INUSE = 0x100;

        /*===============================================================
        '  This implies that the card in the reader is unresponsive or not
        '  supported by the reader or software.
        ' ===============================================================*/
        public const int SCARD_STATE_MUTE = 0x200;

        /*===============================================================
        '  This implies that the card in the reader has not been powered up.
        ' ===============================================================*/
        public const int SCARD_STATE_UNPOWERED = 0x400;
                
        /*===============================================================
        ' This application is not willing to share this card with other 
        'applications.
        '===============================================================*/
        public const int SCARD_SHARE_EXCLUSIVE = 1;
        
        /*===============================================================
        ' This application is willing to share this card with other 
        'applications.
        '===============================================================*/
        public const int SCARD_SHARE_SHARED = 2;
        
        /*===============================================================
        ' This application demands direct control of the reader, so it 
        ' is not available to other applications.
        '===============================================================*/
        public const int SCARD_SHARE_DIRECT = 3;
        
        #endregion //State

        #region Disposition
        
        public const int SCARD_LEAVE_CARD = 0;   // Don't do anything special on close
        public const int SCARD_RESET_CARD = 1;   // Reset the card on close
        public const int SCARD_UNPOWER_CARD = 2;   // Power down the card on close
        public const int SCARD_EJECT_CARD = 3;   // Eject the card on close

        #endregion //Disposition

        #region ACS IOCTL 
        
        public const long FILE_DEVICE_SMARTCARD = 0x310000; // Reader action IOCTLs

        public const long IOCTL_SMARTCARD_DIRECT = FILE_DEVICE_SMARTCARD + 2050 * 4;
        public const long IOCTL_SMARTCARD_SELECT_SLOT = FILE_DEVICE_SMARTCARD + 2051 * 4;
        public const long IOCTL_SMARTCARD_DRAW_LCDBMP = FILE_DEVICE_SMARTCARD + 2052 * 4;
        public const long IOCTL_SMARTCARD_DISPLAY_LCD = FILE_DEVICE_SMARTCARD + 2053 * 4;
        public const long IOCTL_SMARTCARD_CLR_LCD = FILE_DEVICE_SMARTCARD + 2054 * 4;
        public const long IOCTL_SMARTCARD_READ_KEYPAD = FILE_DEVICE_SMARTCARD + 2055 * 4;
        public const long IOCTL_SMARTCARD_READ_RTC = FILE_DEVICE_SMARTCARD + 2057 * 4;
        public const long IOCTL_SMARTCARD_SET_RTC = FILE_DEVICE_SMARTCARD + 2058 * 4;
        public const long IOCTL_SMARTCARD_SET_OPTION = FILE_DEVICE_SMARTCARD + 2059 * 4;
        public const long IOCTL_SMARTCARD_SET_LED = FILE_DEVICE_SMARTCARD + 2060 * 4;
        public const long IOCTL_SMARTCARD_LOAD_KEY = FILE_DEVICE_SMARTCARD + 2062 * 4;
        public const long IOCTL_SMARTCARD_READ_EEPROM = FILE_DEVICE_SMARTCARD + 2065 * 4;
        public const long IOCTL_SMARTCARD_WRITE_EEPROM = FILE_DEVICE_SMARTCARD + 2066 * 4;
        public const long IOCTL_SMARTCARD_GET_VERSION = FILE_DEVICE_SMARTCARD + 2067 * 4;
        public const long IOCTL_SMARTCARD_GET_READER_INFO = FILE_DEVICE_SMARTCARD + 2051 * 4;
        public const long IOCTL_SMARTCARD_SET_CARD_TYPE = FILE_DEVICE_SMARTCARD + 2060 * 4;

        #endregion //ACS IOCTL

        #region Error Codes
 
        public const int SCARD_F_INTERNAL_ERROR = -2146435071;
        public const int SCARD_E_CANCELLED = -2146435070;
        public const int SCARD_E_INVALID_HANDLE = -2146435069;
        public const int SCARD_E_INVALID_PARAMETER = -2146435068;
        public const int SCARD_E_INVALID_TARGET = -2146435067;
        public const int SCARD_E_NO_MEMORY = -2146435066;
        public const int SCARD_F_WAITED_TOO_LONG = -2146435065;
        public const int SCARD_E_INSUFFICIENT_BUFFER = -2146435064;
        public const int SCARD_E_UNKNOWN_READER = -2146435063;


        public const int SCARD_E_TIMEOUT = -2146435062;
        public const int SCARD_E_SHARING_VIOLATION = -2146435061;
        public const int SCARD_E_NO_SMARTCARD = -2146435060;
        public const int SCARD_E_UNKNOWN_CARD = -2146435059;
        public const int SCARD_E_CANT_DISPOSE = -2146435058;
        public const int SCARD_E_PROTO_MISMATCH = -2146435057;


        public const int SCARD_E_NOT_READY = -2146435056;
        public const int SCARD_E_INVALID_VALUE = -2146435055;
        public const int SCARD_E_SYSTEM_CANCELLED = -2146435054;
        public const int SCARD_F_COMM_ERROR = -2146435053;
        public const int SCARD_F_UNKNOWN_ERROR = -2146435052;
        public const int SCARD_E_INVALID_ATR = -2146435051;
        public const int SCARD_E_NOT_TRANSACTED = -2146435050;
        public const int SCARD_E_READER_UNAVAILABLE = -2146435049;
        public const int SCARD_P_SHUTDOWN = -2146435048;
        public const int SCARD_E_PCI_TOO_SMALL = -2146435047;

        public const int SCARD_E_READER_UNSUPPORTED = -2146435046;
        public const int SCARD_E_DUPLICATE_READER = -2146435045;
        public const int SCARD_E_CARD_UNSUPPORTED = -2146435044;
        public const int SCARD_E_NO_SERVICE = -2146435043;
        public const int SCARD_E_SERVICE_STOPPED = -2146435042;

        public const int SCARD_W_UNSUPPORTED_CARD = -2146435041;
        public const int SCARD_W_UNRESPONSIVE_CARD = -2146435040;
        public const int SCARD_W_UNPOWERED_CARD = -2146435039;
        public const int SCARD_W_RESET_CARD = -2146435038;
        public const int SCARD_W_REMOVED_CARD = -2146435037;

        #endregion //Error Codes

        #region Protocol

        public const int SCARD_PROTOCOL_UNDEFINED = 0x00;          // There is no active protocol.
        public const int SCARD_PROTOCOL_T0 = 0x01;                 // T=0 is the active protocol.
        public const int SCARD_PROTOCOL_T1 = 0x02;                 // T=1 is the active protocol.
        public const int SCARD_PROTOCOL_RAW = 0x04;
        public const int SCARD_PROTOCOL_T15 = 0x0008;
        //public const int SCARD_PROTOCOL_RAW = 0x00010000;         // Raw is the active protocol.
        public const uint SCARD_PROTOCOL_DEFAULT = 0x80000000;      // Use implicit PTS.

       #endregion //Protocol

       #region Reader State

        /*===============================================================
        ' This value implies the driver is unaware of the current 
        ' state of the reader.
        '===============================================================*/
        public const int SCARD_UNKNOWN = 0;

        /*===============================================================
        ' This value implies there is no card in the reader.
        '===============================================================*/
        public const int SCARD_ABSENT = 1;

        /*===============================================================
        ' This value implies there is a card is present in the reader, 
        'but that it has not been moved into position for use.
        '===============================================================*/
        public const int SCARD_PRESENT = 2;

        /*===============================================================
        ' This value implies there is a card in the reader in position 
        ' for use.  The card is not powered.
        '===============================================================*/
        public const int SCARD_SWALLOWED = 3;

        /*===============================================================
        ' This value implies there is power is being provided to the card, 
        ' but the Reader Driver is unaware of the mode of the card.
        '===============================================================*/
        public const int SCARD_POWERED = 4;

        /*===============================================================
        ' This value implies the card has been reset and is awaiting 
        ' PTS negotiation.
        '===============================================================*/
        public const int SCARD_NEGOTIABLE = 5;

        /*===============================================================
        ' This value implies the card has been reset and specific 
        ' communication protocols have been established.
        '===============================================================*/
        public const int SCARD_SPECIFIC = 6;
 
        #endregion //Reader State

 #endregion //Constants

        #region Kernel32 Dll Imports
        /**************************************************************************************************
        * Purpose: Importation of required kernel32.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll")]
        private extern static void FreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr handle, string functionName);

        #endregion //Kernel32 Dll Imports

        #region WinSCard Dll Imports
        /**************************************************************************************************
        * Purpose: Importation of required winscard.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
        [DllImport("winscard.dll")]
        public static extern int SCardEstablishContext(uint dwScope, int pvReserved1, int pvReserved2, ref int phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardReleaseContext(int phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardConnect(int hContext, string szReaderName, uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref uint ActiveProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardReconnect(int hContext, uint dwShareMode, uint dwPrefProtocols, uint dwInitialization, ref uint ActiveProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardBeginTransaction(int hCard);

        [DllImport("winscard.dll")]
        public static extern int SCardDisconnect(int hCard, int Disposition);

        [DllImport("winscard.dll")]
        public static extern int SCardListReaderGroups(int hContext, ref string mzGroups, ref int pcchGroups);

        [DllImport("winscard.DLL", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(int hContext, byte[] Groups, byte[] Readers,
                                                  ref int pcchReaders);

        [DllImport("winscard.dll")]
        public static extern int SCardStatus(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, ref byte ATR, ref int ATRLen);

        [DllImport("winscard.dll")]
        private static extern int SCardStatus(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref uint Protocol, IntPtr ATR, ref int ATRLen);

        /**************************************************************************************************
        * Function Name: SCardStatusEx
        * 
        * Purpose: To create a public wrapper method for SCardStatus API. This method also provides 
        *          unmanaged-to-managed & managed-to-unmanaged marshalling.
        *           
        * 
        * Parameters:       hCard = int containing the handle to the smartcard
        *            szReaderName = string containing the smartcard reader name
        *           pcchReaderLen = int containing the length of the reader name in bytes
        *                   state = int containing the reader connection state returned
        *                Protocol = unsigned int containing the reader connection protocol returned
        *                     ATR = byte array containing the answer to reset response returned
        *                  ATRLen = int containing the length in bytes of the answer to reset response  
        *                 
        * Returns: 0 = Success
        *          A non-Zero responses is a PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int SCardStatusEx(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref uint Protocol, ref byte[] ATR, ref int ATRLen)
        {
            IntPtr pnt = IntPtr.Zero;
            int result = -1;
            try
            {
                // Initialize unmanaged memory to hold the array.
                int size = Marshal.SizeOf(ATR[0]) * ATR.Length;

                pnt = Marshal.AllocHGlobal(size);

                // Copy the array to unmanaged memory.
                Marshal.Copy(ATR, 0, pnt, ATR.Length);

                result = SCardStatus(hCard, szReaderName, ref pcchReaderLen, ref State, ref Protocol, pnt, ref size);

                // Copy the unmanaged array back.
                Marshal.Copy(pnt, ATR, 0, size);
                ATRLen = size;

            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
            return result;
        }

        [DllImport("winscard.dll")]
        public static extern int SCardEndTransaction(int hCard, int Disposition);

        [DllImport("winscard.dll")]
        public static extern int SCardState(int hCard, ref uint State, ref uint Protocol, ref byte ATR, ref uint ATRLen);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);

        [DllImport("winscard.dll")]
        private static extern int SCardTransmit(int hCard, ref SCARD_IO_REQUEST pioSendRequest, IntPtr SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, IntPtr RecvBuff, ref int RecvBuffLen);

        /**************************************************************************************************
        * Function Name: SCardTransmitEx
        * 
        * Purpose: To create a public wrapper method for SCardTransmit API. This method also provides 
        *          unmanaged-to-managed & managed-to-unmanaged marshalling.
        *           
        * 
        * Parameters:        hCard = int containing the handle to the smartcard
        *           pioSendRequest = struct containing the required parameters for the transmit request
        *                 SendBuff = byte array containing the data to be transmitted to the card
        *              SendBuffLen = int containing the number of bytes in SendBuff
        *           pioRecvRequest = struct containing the response to the transmit request
        *                 RecvBuff = byte array containing the response to be transmitted data
        *              RecvBuffLen = int containing the number of bytes in RecvBuff  
        *                 
        * Returns: 0 = Success
        *          A non-Zero responses is a PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int SCardTransmitEx(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte[] SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte[] RecvBuff, ref int RecvBuffLen)
        {
            int SendSize = 0;
            int RecSize = 0;
            IntPtr Sendpnt = IntPtr.Zero;
            IntPtr Recpnt = IntPtr.Zero;

            int result = -1;

            try
            {
                if (SendBuff != null)
                {
                    SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                }
                Sendpnt = Marshal.AllocHGlobal(SendSize);

                if (RecvBuff != null)
                {
                    RecSize = Marshal.SizeOf(RecvBuff[0]) * RecvBuffLen;
                }
                Recpnt = Marshal.AllocHGlobal(RecSize);


                // Copy the array to unmanaged memory.
                if (SendBuff != null)
                    Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);

                if (RecvBuff != null)
                    Marshal.Copy(RecvBuff, 0, Recpnt, RecvBuffLen);

                result = SCardTransmit(hCard, ref pioSendRequest, Sendpnt, SendSize, ref pioRecvRequest,
                                       Recpnt, ref RecSize);

                // Copy the unmanaged array back.
                Marshal.Copy(Recpnt, RecvBuff, 0, RecSize);
                RecvBuffLen = RecSize;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(Sendpnt);
                Marshal.FreeHGlobal(Recpnt);
            }
            return result;
        }

        [DllImport("winscard.dll")]
        private static extern int SCardControl(int hCard, uint dwControlCode, IntPtr SendBuff, int SendBuffLen, ref VERSION_CONTROL RecvBuff, int RecvBuffLen, ref int pcbBytesReturned);

        /**************************************************************************************************
        * Function Name: SCardControlEx
        * 
        * Purpose: To create a public wrapper method for SCardControl API. This method also provides 
        *          unmanaged-to-managed & managed-to-unmanaged marshalling.
        *           
        * 
        * Parameters:        hCard = int containing the handle to the smartcard
        *            dwControlCode = unsigned int containing the control code
        *                 SendBuff = byte array containing the data to be transmitted to the card
        *              SendBuffLen = int containing the number of bytes in SendBuff
        *                 RecvBuff = struct containing the smartcard reader version information
        *              RecvBuffLen = int containing the number of bytes in RecvBuff  
        *         pcbBytesReturned = int containing the number of bytes returned 
        *                 
        * Returns: 0 = Success
        *          A non-Zero responses is a PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int SCardControlEx(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref VERSION_CONTROL RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
        {
            int SendSize = 0;
            int RecSize = 0;
            IntPtr Sendpnt = IntPtr.Zero;

            int result = -1;

            try
            {
                if (SendBuff != null)
                {
                    SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                }
                Sendpnt = Marshal.AllocHGlobal(SendSize);

                // Copy the array to unmanaged memory.
                if (SendBuff != null)
                    Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);


                result = SCardControl(hCard, dwControlCode, Sendpnt, SendSize,
                                      ref RecvBuff, RecvBuffLen, ref RecSize);

                pcbBytesReturned = RecSize;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(Sendpnt);
            }
            return result;
        }

        [DllImport("winscard.dll")]
        public static extern int SCardControl(int hCard, uint dwControlCode, IntPtr SendBuff, int SendBuffLen, IntPtr RecvBuff, int RecvBuffLen, ref int pcbBytesReturned);

        /**************************************************************************************************
        * Function Name: SCardTransmitEx
        * 
        * Purpose: To create a public wrapper method for SCardTransmit API. This method also provides 
        *          unmanaged-to-managed & managed-to-unmanaged marshalling.
        *           
        * 
        * Parameters:        hCard = int containing the handle to the smartcard
        *            dwControlCode = unsigned int containing the control code
        *                 SendBuff = byte array containing the data to be transmitted to the card
        *              SendBuffLen = int containing the number of bytes in SendBuff
        *                 RecvBuff = byte array containing the response to be transmitted data
        *              RecvBuffLen = int containing the number of bytes in RecvBuff  
        *         pcbBytesReturned = int containing the number of bytes returned
        *                 
        * Returns: 0 = Success
        *          A non-Zero responses is a PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int SCardControlEx(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref byte[] RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
        {
            int SendSize = 0;
            int RecSize = 0;
            IntPtr Sendpnt = IntPtr.Zero;
            IntPtr Recpnt = IntPtr.Zero;
            int result = -1;
            try
            {
                if (SendBuff != null)
                {
                    SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                }
                Sendpnt = Marshal.AllocHGlobal(SendSize);

                if (RecvBuff != null)
                {
                    RecSize = Marshal.SizeOf(RecvBuff[0]) * RecvBuffLen;
                }
                Recpnt = Marshal.AllocHGlobal(RecSize);

                // Copy the array to unmanaged memory.
                if (SendBuff != null)
                    Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);

                if (RecvBuff != null)
                    Marshal.Copy(RecvBuff, 0, Recpnt, RecvBuffLen);

                result = SCardControl(hCard, dwControlCode, Sendpnt, SendBuffLen,
                                      Recpnt, RecvBuffLen, ref  RecSize);

                // Copy the unmanaged array back.
                Marshal.Copy(Recpnt, RecvBuff, 0, RecSize);
                pcbBytesReturned = RecSize;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(Sendpnt);
                Marshal.FreeHGlobal(Recpnt);
            }
            return result;
        }

        [DllImport("winscard.dll")]
        private static extern int SCardGetAttrib(int hCard, uint dwAttrId, IntPtr pbAttr, ref int pcbAttrLen);

        /**************************************************************************************************
        * Function Name: SCardGetAttribEx
        * 
        * Purpose: To create a public wrapper method for SCardGetAttrib API. This method also provides 
        *          unmanaged-to-managed & managed-to-unmanaged marshalling.
        *           
        * 
        * Parameters:        hCard = int containing the handle to the smartcard
        *               dwAttribId = unsigned int containing the attribute code
        *                   pbAttr = byte array containing the attribute returend
        *               pcbAttrLen = int containing the number of bytes in pbAttr
        *                                
        * Returns: 0 = Success
        *          A non-Zero responses is a PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int SCardGetAttribEx(int hCard, uint dwAttrId, ref byte[] pbAttr, ref int pcbAttrLen)
        {
            int size = 0;
            int result = -1;
            IntPtr pnt = IntPtr.Zero;

            try
            {
                size = Marshal.SizeOf(pbAttr[0]) * pcbAttrLen;
                pnt = Marshal.AllocHGlobal(size);

                // Copy the array to unmanaged memory.
                Marshal.Copy(pbAttr, 0, pnt, pcbAttrLen);

                result = SCardGetAttrib(hCard, dwAttrId, pnt, ref size);

                // Copy the unmanaged array back.
                Marshal.Copy(pnt, pbAttr, 0, size);
                pcbAttrLen = size;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
            return result;
        }
        #endregion WinSCard Dll Imports

        #region WinsCard.dll Wrapper Functions
        /**************************************************************************************************
        * Purpose: Public wrappers for imported winscard.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
        public int SCardEstablishContextWrapper(uint dwScope, int pvReserved1, int pvReserved2, ref int phContext)
        {
            return SCardEstablishContext(dwScope, pvReserved1, pvReserved2, ref phContext);
        }

        public int SCardReleaseContextWrapper(int phContext)
        {
            return SCardReleaseContext(phContext);
        }

        public int SCardConnectWrapper(int hContext, string szReaderName, uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref uint ActiveProtocol)
        {
            return SCardConnect(hContext, szReaderName, dwShareMode, dwPrefProtocol, ref phCard, ref ActiveProtocol);
        }

        public int SCardBeginTransactionWrapper(int hCard)
        {
            return SCardBeginTransaction(hCard);
        }

        public int SCardDisconnectWrapper(int hCard, int Disposition)
        {
            return SCardDisconnect(hCard, Disposition);
        }

        public int SCardListReaderGroupsWrapper(int hContext, ref string mzGroups, ref int pcchGroups)
        {
            return SCardListReaderGroups(hContext, ref mzGroups, ref pcchGroups);
        }

        public int SCardListReadersWrapper(int hContext, byte[] Groups, byte[] Readers,
                                           ref int pcchReaders)
        {
            return SCardListReaders(hContext, Groups, Readers, ref pcchReaders);
        }

        public int SCardStatusWrapper(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, ref byte ATR, ref int ATRLen)
        {
            return SCardStatus(hCard, szReaderName, ref pcchReaderLen, ref State, ref Protocol, ref ATR, ref ATRLen);
        }

        public int SCardEndTransactionWrapper(int hCard, int Disposition)
        {
            return SCardEndTransaction(hCard, Disposition);
        }

        public int SCardStateWrapper(int hCard, ref uint State, ref uint Protocol, ref byte ATR, ref uint ATRLen)
        {
            return SCardState(hCard, ref State, ref Protocol, ref ATR, ref ATRLen);
        }

        public int SCardTransmitWrapper(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen)
        {
            return SCardTransmit(hCard, ref pioSendRequest, ref SendBuff, SendBuffLen, ref pioRecvRequest, ref RecvBuff, ref RecvBuffLen);
        }

        public int SCardControlWrapper(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref byte[] RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
        {
            return SCardControlEx(hCard, dwControlCode, ref SendBuff, SendBuffLen, ref RecvBuff, RecvBuffLen, ref pcbBytesReturned);
        }

        public int SCardGetAttribWrapper(int hCard, uint dwAttrId, ref byte[] pbAttr, ref int pcbAttrLen)
        {
            return SCardGetAttribEx(hCard, dwAttrId, ref pbAttr, ref pcbAttrLen);
        }
        #endregion //WinsCard.dll Wrapper Functions

        #region PCI Address

        /**************************************************************************************************
        * Function Name: GetPciT0
        * 
        * Purpose: To retrieve the T0 PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   None
        *                                
        * Returns: Success = Address to PCI
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        private static IntPtr GetPciT0()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr pci = IntPtr.Zero;
            try
            {
                handle = LoadLibrary("Winscard.dll");
                pci = GetProcAddress(handle, "g_rgSCardT0Pci");
            }
            catch
            {
                pci = IntPtr.Zero;
            }
            finally
            {
                FreeLibrary(handle);
            }
            return pci;
        }

        /**************************************************************************************************
        * Function Name: GetPciT1
        * 
        * Purpose: To retrieve the T1 PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   None
        *                                
        * Returns: Success = Address to PCI
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        private static IntPtr GetPciT1()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr pci = IntPtr.Zero;
            try
            {
                handle = LoadLibrary("Winscard.dll");
                pci = GetProcAddress(handle, "g_rgSCardT1Pci");
            }
            catch
            {
                pci = IntPtr.Zero;
            }
            finally
            {
                FreeLibrary(handle);
            }
            return pci;
        }

        /**************************************************************************************************
        * Function Name: GetPciRaw
        * 
        * Purpose: To retrieve the Raw PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   None
        *                                
        * Returns: Success = Address to PCI
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        private static IntPtr GetPciRaw()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr pci = IntPtr.Zero;
            try
            {
                handle = LoadLibrary("Winscard.dll");
                pci = GetProcAddress(handle, "g_rgSCardRawPci");
            }
            catch
            {
                pci = IntPtr.Zero;
            }
            finally
            {
                FreeLibrary(handle);
            }
            return pci;
        }

        #endregion //PCI Address

        #region Get Protocol
        /**************************************************************************************************
        * Function Name: GetProtocolStructure
        * 
        * Purpose: To retrieve the appropriate PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   protocol = int containing the protocol in use
        *                                
        * Returns: Success = Address to PCI for the protocol in use
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static IntPtr GetProtocolStructure(int protocol)
        {
            try
            {
                if (protocol == 1)
                    return GetPciT0();

                else if ((protocol == 2) || (protocol == 3))
                    return GetPciT1();

                else
                    return GetPciRaw();
            }
            catch
            {
            }
            return IntPtr.Zero;
        }

        /**************************************************************************************************
        * Function Name: GetProtocolStructureEx
        * 
        * Purpose: To provide a public wrapper for the static function GetProtocolStructure.
        *           
        * 
        * Parameters:   protocol = int containing the protocol in use
        *                                
        * Returns: Success = Address to PCI for the protocol in use
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public IntPtr GetProtocolStructureEx(int protocol)
        {
            return GetProtocolStructure(protocol);
        }
        #endregion //Get Protocol

        #region Public Methods

        /**************************************************************************************************
        * Function Name: GetPCSReaders
        * 
        * Purpose: To retrieve the contactless & contact smartcard reader names from windows.
        *           
        * 
        * Parameters:   contactlessReader = string containing the name of the contactless reader
        *                   contactReader = string containing the name of the contact reader
        *                          errMsg = string containing an error message if an error is encountered 
        *                                
        * Returns: Success = 0
        *          Failure = PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static int GetPCSCReaders(out string contactlessReader, out string contactReader,
                                         out string errMsg)
        {
            contactlessReader = contactReader = errMsg = "";
            byte[] readersList = null;
            try
            {
                int context = 0;
                int ret = SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, 0, 0, ref context);
                if (ret != 0)
                {
                    errMsg = "WinSCard: GetPCSCReader: EstablishContext Error: " + ret.ToString();
                    return ret;
                }

                int byteCnt = 0;
                ret = WinSCard.SCardListReaders(context, null, null, ref byteCnt);
                if (ret != SCARD_S_SUCCESS)
                {
                    errMsg = "WinSCard: GetPCSCReader: ListReaders Error: " + ret.ToString();
                    return ret;
                }

                readersList = new byte[byteCnt];
                ret = WinSCard.SCardListReaders(context, null, readersList, ref byteCnt);
                if (ret != SCARD_S_SUCCESS)
                {
                    errMsg = "WinSCard: GetPCSCReader: ListReaders Error: " + ret.ToString();
                    return ret;
                }

                int indx = 0;
                string readerName = "";

                while (readersList[indx] != 0)
                {
                    while (readersList[indx] != 0)
                    {
                        readerName = readerName + (char)readersList[indx++];
                    }

                    if (readerName.Contains("ACR38U") || readerName.Contains("ACS") || readerName.Contains("SDI010 Smart Card Reader"))
                        contactReader = readerName;
                    
                    else if (readerName.Contains("SDI010 Contactless"))
                        contactlessReader = readerName;

                    readerName = "";
                    indx++;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                readersList = null;
            }
            return 0;
        }

        /**************************************************************************************************
        * Function Name: GetSCardErrMsg
        * 
        * Purpose: To retrieve the error message string for a specific error code.
        *      
        * 
        * Parameters:   ReturnCode = int containing the error code
        *                    
        *                                
        * Returns: Success = string containing the error message
        *          Failure = Exception message
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
        public static string GetSCardErrMsg(int ReturnCode)
        {
            string errMsg = string.Empty;
            try
            {
                switch (ReturnCode)
                {
                    case SCARD_E_CANCELLED:
                        return ("The action was canceled by an SCardCancel request.");
                    case SCARD_E_CANT_DISPOSE:
                        return ("The system could not dispose of the media in the requested manner.");
                    case SCARD_E_CARD_UNSUPPORTED:
                        return ("The smart card does not meet minimal requirements for support.");
                    case SCARD_E_DUPLICATE_READER:
                        return ("The reader driver didn't produce a unique reader name.");
                    case SCARD_E_INSUFFICIENT_BUFFER:
                        return ("The data buffer for returned data is too small for the returned data.");
                    case SCARD_E_INVALID_ATR:
                        return ("An ATR string obtained from the registry is not a valid ATR string.");
                    case SCARD_E_INVALID_HANDLE:
                        return ("The supplied handle was invalid.");
                    case SCARD_E_INVALID_PARAMETER:
                        return ("One or more of the supplied parameters could not be properly interpreted.");
                    case SCARD_E_INVALID_TARGET:
                        return ("Registry startup information is missing or invalid.");
                    case SCARD_E_INVALID_VALUE:
                        return ("One or more of the supplied parameter values could not be properly interpreted.");
                    case SCARD_E_NOT_READY:
                        return ("The reader or card is not ready to accept commands.");
                    case SCARD_E_NOT_TRANSACTED:
                        return ("An attempt was made to end a non-existent transaction.");
                    case SCARD_E_NO_MEMORY:
                        return ("Not enough memory available to complete this command.");
                    case SCARD_E_NO_SERVICE:
                        return ("The smart card resource manager is not running.");
                    case SCARD_E_NO_SMARTCARD:
                        return ("The operation requires a smart card, but no smart card is currently in the device.");
                    case SCARD_E_PCI_TOO_SMALL:
                        return ("The PCI receive buffer was too small.");
                    case SCARD_E_PROTO_MISMATCH:
                        return ("The requested protocols are incompatible with the protocol currently in use with the card.");
                    case SCARD_E_READER_UNAVAILABLE:
                        return ("The specified reader is not currently available for use.");
                    case SCARD_E_READER_UNSUPPORTED:
                        return ("The reader driver does not meet minimal requirements for support.");
                    case SCARD_E_SERVICE_STOPPED:
                        return ("The smart card resource manager has shut down.");
                    case SCARD_E_SHARING_VIOLATION:
                        return ("The smart card cannot be accessed because of other outstanding connections.");
                    case SCARD_E_SYSTEM_CANCELLED:
                        return ("The action was canceled by the system, presumably to log off or shut down.");
                    case SCARD_E_TIMEOUT:
                        return ("The user-specified timeout value has expired.");
                    case SCARD_E_UNKNOWN_CARD:
                        return ("The specified smart card name is not recognized.");
                    case SCARD_E_UNKNOWN_READER:
                        return ("The specified reader name is not recognized.");
                    case SCARD_F_COMM_ERROR:
                        return ("An internal communications error has been detected.");
                    case SCARD_F_INTERNAL_ERROR:
                        return ("An internal consistency check failed.");
                    case SCARD_F_UNKNOWN_ERROR:
                        return ("An internal error has been detected, but the source is unknown.");
                    case SCARD_F_WAITED_TOO_LONG:
                        return ("An internal consistency timer has expired.");
                    case SCARD_S_SUCCESS:
                        return ("No error was encountered.");
                    case SCARD_W_REMOVED_CARD:
                        return ("The smart card has been removed, so that further communication is not possible.");
                    case SCARD_W_RESET_CARD:
                        return ("The smart card has been reset, so any shared state information is invalid.");
                    case SCARD_W_UNPOWERED_CARD:
                        return ("Power has been removed from the smart card, so that further communication is not possible.");
                    case SCARD_W_UNRESPONSIVE_CARD:
                        return ("The smart card is not responding to a reset.");
                    case SCARD_W_UNSUPPORTED_CARD:
                        return ("The reader cannot communicate with the card, due to ATR string configuration conflicts.");
                    default:
                        return ("?");
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }
        #endregion //Public Methods
    }

    /**************************************************************************************************
    * Class Name: ContactlessSupport
    *  
    * Purpose: To provide contactless smartcard functionality. 
    *           
    ***************************************************************************************************/
    public class ContactlessSupport
    {
        #region Constants & Variables

        private uint _activeProtocol = 0;
        private int _context = 0;
        private int _cardHandle = 0;
        private int _previousCardHandle = 0;

        private byte[] _sessionKey = null;

        // Base Mifare ATR for 1K, 4K and Ultralight
        private byte[] bufMifare = { 0x3B, 0x8F, 0x80, 0x01, 0x80, 0x4F, 0x0C, 0xA0, 0x00, 0x00, 0x03, 0x06,
                                     0x03, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF};

        // Desfire ATR
        private byte[] bufDesfireATR = { 0x3B, 0x81, 0x80, 0x01, 0x80, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private const int IOCTL_CCID_ESCAPE = 0x3136B0;

        #endregion //Constants & Variables

        #region Constructor & Destructor

        public ContactlessSupport()
        {
            _sessionKey = new byte[16];
        }

        ~ContactlessSupport()
        {
            _sessionKey = null;
        }

        #endregion //Constructor & Destructor

        #region Utility Methods
        // Block To Bytes - converts the block integer to a byte array
        private byte[] BlockToBytes(int block)
        {
            byte[] blk = { (byte)((block & 0xFF00) >> 8), (byte)(block & 0x00FF) };
            return blk;
        }
        #endregion //Utility Methods

        #region Desfire Support

        private bool DesfireTest(ref string errMsg)
        {
            bool passed;
            byte[] dataIn = null;
            byte[] dataOut = null;
            try
            {
                byte[] key = { 0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00 };

                passed = DesfireAuthenticate(0, ref key, ref errMsg);

                if (errMsg != "")
                    return false;

                passed = DesfireFormatCard(ref errMsg);

                if (errMsg != "")
                    return false;

                byte[] aid = { 0xAA, 0xAA, 0xAA };
                passed = DesfireCreateApplication(ref aid, ref errMsg);

                if (errMsg != "")
                    return false;

                passed = DesfireSelectApplication(ref aid, ref errMsg);

                if (errMsg != "")
                    return false;

                byte fileNumber = 0x0F;
                byte comMode = 0x00;
                int accessRights = 0x0EEEE;
                int fileSize = 0x0F3C;

                passed = DesfireCreateFile(fileNumber, comMode, accessRights, fileSize, ref errMsg);

                if (errMsg != "")
                    return false;

                dataIn = new byte[32];
                for (int i = 0; i < dataIn.Length; i++)
                    dataIn[i] = (byte)(i + 0x41);

                passed = DesfireWrite(15, 0, ref dataIn, ref errMsg);

                if (errMsg != "")
                    return false;

                dataOut = new byte[dataIn.Length];

                passed = DesfireRead(15, 0, ref dataOut, ref errMsg);

                if (errMsg != "")
                    return false;

                for (int i = 0; i < dataIn.Length; i++)
                {
                    if (dataIn[i] != dataOut[i])
                    {
                        errMsg = "Read does not equal Written for Address [" + i.ToString() + "]";
                        passed = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                passed = false;
            }
            finally
            {
                dataIn = null;
                dataOut = null;
            }
            return passed;
        }

        private bool DesfireTest(string dataToEncode, ref string errMsg)
        {
            bool passed;
            byte[] dataIn = null;
            byte[] dataOut = null;
            try
            {
                byte[] key = { 0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00,
                               0x00, 0x00, 0x00, 0x00 };

                passed = DesfireAuthenticate(0, ref key, ref errMsg);

                if (errMsg != "")
                    return false;

                passed = DesfireFormatCard(ref errMsg);

                if (errMsg != "")
                    return false;

                byte[] aid = { 0xAA, 0xAA, 0xAA };
                passed = DesfireCreateApplication(ref aid, ref errMsg);

                if (errMsg != "")
                    return false;

                passed = DesfireSelectApplication(ref aid, ref errMsg);

                if (errMsg != "")
                    return false;

                byte fileNumber = 0x0F;
                byte comMode = 0x00;
                int accessRights = 0x0EEEE;
                int fileSize = 0x0F3C;

                passed = DesfireCreateFile(fileNumber, comMode, accessRights, fileSize, ref errMsg);

                if (errMsg != "")
                    return false;

                dataIn = new byte[32];
                byte data = Convert.ToByte(dataToEncode);

                for (int i = 0; i < dataIn.Length; i++)
                    dataIn[i] = data;

                passed = DesfireWrite(15, 0, ref dataIn, ref errMsg);

                if (errMsg != "")
                    return false;

                dataOut = new byte[dataIn.Length];

                passed = DesfireRead(15, 0, ref dataOut, ref errMsg);

                if (errMsg != "")
                    return false;

                for (int i = 0; i < dataIn.Length; i++)
                {
                    if (dataIn[i] != dataOut[i])
                    {
                        errMsg = "Read does not equal Written for Address [" + i.ToString() + "]";
                        passed = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                passed = false;
            }
            finally
            {
                dataIn = null;
                dataOut = null;
            }
            return passed;
        }

        private bool DesfireAuthenticate(byte keyNumber, ref byte[] key, ref string errMsg)
        {
            byte[] respBuf = null;
            byte[] tempBuf8 = null;
            byte[] rndB = null;
            byte[] rndA = null;
            Random rand = null;
            byte[] bothRnd = null;
            byte[] tempBuf16 = null;
            DesfireEncryption des = null;
            try
            {
                respBuf = new byte[4096];
                int respSize = respBuf.Length;

                byte[] cmd = { 0x0A, keyNumber };

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "DesfireAuthenticate Error Getting RndB: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0xAF)
                {
                    errMsg = "DesfireAuthenticate Error Getting RndB: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                //Copy encrypted RndB into temp buffer

                tempBuf8 = new byte[8];
                Array.Copy(respBuf, 1, tempBuf8, 0, 8);

                //Decode RndB
                des = new DesfireEncryption();
                des.Desfire_xDes_Recv(tempBuf8, 1, key);

                rndB = new byte[8];
                Array.Copy(tempBuf8, 1, rndB, 0, 7);
                rndB[7] = tempBuf8[0];

                //Generate random numbers
                rand = new Random();
                rndA = new byte[8];
                rand.NextBytes(rndA);

                //RndA + RndB
                bothRnd = new byte[16];
                Array.Copy(rndA, 0, bothRnd, 0, 8);
                Array.Copy(rndB, 0, bothRnd, 8, 8);

                //Encrypt both Rnd
                des.Desfire_xDES_Send(bothRnd, 2, key);

                //reinitialize variables
                Array.Clear(respBuf, 0, respBuf.Length);
                respSize = respBuf.Length;

                cmd = null;
                cmd = new byte[17];
                Array.Clear(cmd, 0, cmd.Length);
                cmd[0] = 0xAF;
                Array.Copy(bothRnd, 0, cmd, 1, 16);

                ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "DesfireAuthenticate Error Getting RndA: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "DesfireAuthenticate Error Getting RndA: Card Status = " +
                                DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                //Place returned RndA into tempBuf and decode

                tempBuf16 = new byte[16];
                Array.Clear(tempBuf16, 0, tempBuf16.Length);
                Array.Copy(respBuf, 1, tempBuf16, 0, 8);

                des.Desfire_xDes_Recv(tempBuf16, 2, key);

                //Check to see if sent equals returned
                bool match = true;

                for (int i = 0; i < 8; i++)
                {
                    if (i == 7)
                    {
                        if (tempBuf16[i] != rndA[0])
                        {
                            match = false;
                            break;
                        }
                    }
                    else if (tempBuf16[i] != rndA[i + 1])
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    errMsg = "DesfireAuthenticate Returned RndA did not Match Sent";
                    return false;
                }

                Array.Copy(rndA, _sessionKey, 4);
                Array.Copy(rndB, 0, _sessionKey, 4, 4);

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
                tempBuf8 = null;
                rndB = null;
                rndA = null;
                rand = null;
                bothRnd = null;
                tempBuf16 = null;
                des = null;
            }
            return false;
        }

        private bool DesfireCreateApplication(ref byte[] aid, ref string errMsg)
        {
            byte[] respBuf = null;
            byte[] cmd = { 0xCA, aid[0], aid[1], aid[2], 0xFF, 0x00 };
            try
            {
                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "Create Application Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "Create Application Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
                cmd = null;
            }
            return false;
        }

        private bool DesfireCreateFile(byte fileNumber, byte comMode, int accessRights, int fileSize,
                                        ref string errMsg)
        {
            byte[] respBuf = null;

            byte[] accRights = { (byte)((accessRights & 0xFF00) >> 8),
                                 (byte)(accessRights & 0x00FF) };

            byte[] fSize = { (byte)((fileSize & 0xFF0000) >> 16),
                             (byte)((fileSize & 0x00FF00) >> 8),
                             (byte)(fileSize & 0x0000FF) };

            byte[] cmd = { 0xCD, fileNumber, comMode, accRights[1], accRights[0],
                           fSize[2], fSize[1], fSize[0] };
            try
            {
                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);
                if (ret != 0)
                {
                    errMsg = "Create File Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "Create File Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
                accRights = null;
                fSize = null;
                cmd = null;
            }
            return false;
        }

        private bool DesfireFormatCard(ref string errMsg)
        {
            byte[] respBuf = null;
            byte[] cmd = { 0xFC };
            try
            {
                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);
                if (ret != 0)
                {
                    errMsg = "DesfireFormatCard Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "DesfireFormatCard Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
                cmd = null;
            }
            return false;
        }

        private bool DesfireRead(byte fileNumber, int addr, ref byte[] dataOut, ref string errMsg)
        {
            byte[] cmd = null;
            byte[] respBuf = null;
            try
            {
                int dataSize = dataOut.Length;
                if (dataSize > 59)
                    dataSize = 59;

                cmd = new byte[8];
                cmd[0] = 0xBD;
                cmd[1] = fileNumber;
                cmd[2] = (byte)(addr & 0x0000FF);
                cmd[3] = (byte)((addr & 0x00FF00) >> 8);
                cmd[4] = (byte)((addr & 0xFF0000) >> 16);
                cmd[5] = (byte)(dataSize & 0x0000FF);
                cmd[6] = (byte)((dataSize & 0x00FF00) >> 8);
                cmd[7] = (byte)((dataSize & 0xFF0000) >> 16);

                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "Read Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "Read Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }

                for (int i = 0; i < dataSize; i++)
                    dataOut[i] = respBuf[i + 1];

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                cmd = null;
                respBuf = null;
            }
            return false;
        }

        private bool DesfireSelectApplication(ref byte[] aid, ref string errMsg)
        {

            byte[] respBuf = null;
            byte[] cmd = { 0x5A, aid[0], aid[1], aid[2] };
            try
            {
                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "DesfireIsAID Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "DesfireIsAID Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
                cmd = null;
            }
            return false;
        }

        private bool DesfireWrite(byte fileNumber, int addr, ref byte[] dataIn, ref string errMsg)
        {
            byte[] cmd = null;
            byte[] respBuf = null;
            try
            {
                int dataInSize = dataIn.Length;
                if (dataInSize > 52)
                    dataInSize = 52;

                cmd = new byte[dataInSize + 8];

                cmd[0] = 0x3D;
                cmd[1] = fileNumber;
                cmd[2] = (byte)(addr & 0x0000FF);
                cmd[3] = (byte)((addr & 0x00FF00) >> 8);
                cmd[4] = (byte)((addr & 0xFF0000) >> 16);
                cmd[5] = (byte)(dataInSize & 0x0000FF);
                cmd[6] = (byte)((dataInSize & 0x00FF00) >> 8);
                cmd[7] = (byte)((dataInSize & 0xFF0000) >> 16);

                for (int i = 0; i < dataInSize; i++)
                    cmd[i + 8] = dataIn[i];

                respBuf = new byte[128];
                int respSize = respBuf.Length;

                int ret = SendDesfireCommand(ref cmd, ref respBuf, ref respSize);

                if (ret != 0)
                {
                    errMsg = "Write Error: SCardTransmit Return = " + ret.ToString();
                    return false;
                }

                if (respBuf[0] != 0x00)
                {
                    errMsg = "Write Error: Card Status = " + DesfireStatusMsg(respBuf[0]);
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                cmd = null;
                respBuf = null;
            }
            return false;
        }

        private int SendDesfireCommand(ref byte[] cmd, ref byte[] respBuf, ref int respSize)
        {
            byte[] apdu = null;
            try
            {
                apdu = new byte[cmd.Length + 5];
                apdu[0] = 0xFF;
                apdu[1] = 0xDE;
                apdu[2] = 0x00;
                apdu[3] = 0x00;
                apdu[4] = (byte)(cmd.Length & 0x00FF);

                for (int i = 0; i < cmd.Length; i++)
                    apdu[i + 5] = cmd[i];

                return SCardTransmit(ref apdu, apdu.Length, ref respBuf, ref respSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                apdu = null;
            }
        }

        //Returns a Desfire Status Message
        private string DesfireStatusMsg(byte status)
        {
            string msg = "Unknown Status";
            try
            {
                switch (status)
                {
                    case 0x00:
                        msg = "Operation OK";
                        break;

                    case 0x0C:
                        msg = "No Changes";
                        break;

                    case 0x0E:
                        msg = "Out of EEPROM";
                        break;

                    case 0x1C:
                        msg = "Illegal Command Code";
                        break;

                    case 0x1E:
                        msg = "Integrity Error";
                        break;

                    case 0x40:
                        msg = "No Such Key";
                        break;

                    case 0x7E:
                        msg = "Invalid Command Length";
                        break;

                    case 0x9D:
                        msg = "Permission Denied";
                        break;

                    case 0x9E:
                        msg = "Invalid Parameter";
                        break;

                    case 0xA0:
                        msg = "Application Not Found";
                        break;

                    case 0xA1:
                        msg = "Application Integrity Error";
                        break;

                    case 0xAE:
                        msg = "Authentication Error";
                        break;

                    case 0xAF:
                        msg = "Additional Data";
                        break;

                    case 0xBE:
                        msg = "Boundary Error";
                        break;

                    case 0xC1:
                        msg = "PICC Integrity Error";
                        break;

                    case 0xCA:
                        msg = "Command Aborted";
                        break;

                    case 0xCD:
                        msg = "PICC Disabled";
                        break;

                    case 0xCE:
                        msg = "Application Count Error";
                        break;

                    case 0xDE:
                        msg = "Duplication Error";
                        break;

                    case 0xEE:
                        msg = "EEPROM Power Error";
                        break;

                    case 0xF0:
                        msg = "File Not Found";
                        break;

                    case 0xF1:
                        msg = "File Integrity Error";
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
    
        #endregion //Desire Tests

        #region Mifare Test

        public bool MifareTest(ref string smartCardType, string readerName, byte[] key, bool fullTest,
                               out string errMsg)
        {
            errMsg = string.Empty;

            byte[] dataIn = null;
            byte[] dataOut = null;

            bool passed = false;
            try
            {
                dataIn = new byte[8];
                dataOut = new byte[8];

                int blockEnd = 0;

                int ret = EstablishContext(out errMsg);
                if (ret != 0)
                    return false;

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);

                    ret = Connect(readerName);
                    if (ret == 0)
                    {
                        _previousCardHandle = _cardHandle;
                        passed = true;
                        break;
                    }
                    else
                    {
                        DisconnectPreviousHandle();
                    }
                    ReleaseContext();
                    Thread.Sleep(500);
                    EstablishContext(out errMsg);
                }

                if (!passed)
                {
                    errMsg = "Card Connection Failed [" + ret.ToString() + "]";
                }
                else if (SetCardType("TypeA") != 0)
                {
                    errMsg = "Mifare Set Card Type Failed";
                    passed = false;
                }
                else if (SetPUPI(true) != 0)
                {
                    errMsg = "Mifare Set PUPI Failed";
                    passed = false;
                }
                else if (GetCardTypeFromATR(readerName, ref smartCardType, out errMsg) != 0)
                {
                    errMsg = "Mifare " + errMsg;
                    passed = false;
                }

                if (smartCardType == "MIFARE DESFIRE")
                {
                    if (fullTest)
                        passed = DesfireTest(ref errMsg);

                    return passed;
                }

                byte baseByte = 0x41;
                int dataSize = 4;

                if (errMsg == "" && smartCardType == "MIFARE ULTRALIGHT")
                {
                    dataSize = 4;

                    if (fullTest)
                        blockEnd = 15;
                    else
                        blockEnd = 4;

                    for (int block = 4; block <= blockEnd; block++)
                    {
                        for (int i = 0; i < dataSize; i++)
                            dataIn[i] = baseByte++;

                        ret = MifareBlockWrite(block, dataIn, dataSize, out errMsg);
                        if (ret != 0)
                        {
                            errMsg = "MIFARE ULTRALIGHT Write Error: " + ret.ToString();
                            passed = false;
                            break;
                        }
                    }

                    if (passed)
                    {
                        baseByte = 0x41;

                        for (int block = 4; block <= blockEnd; block++)
                        {
                            ret = MifareBlockRead(block, ref dataOut, ref dataSize, out errMsg);
                            if (ret != 0 || dataSize != 4)
                            {
                                errMsg = "MIFARE ULTRALIGHT Read Error: " + ret.ToString();
                                passed = false;
                                break;
                            }

                            if (dataOut[0] != baseByte || dataOut[1] != baseByte + 1 ||
                                dataOut[2] != baseByte + 2 || dataOut[3] != baseByte + 3)
                            {
                                errMsg = "MIFARE ULTRALIGHT Block [" + block.ToString() +
                                            "] Read does not equal Written";
                                passed = false;
                                break;
                            }
                            baseByte += 4;
                        }
                    }
                }
                else if (errMsg == "")
                {
                    if (LoadKey("KeyA", key.Length, key, out errMsg) != 0)
                    {
                        errMsg = "Mifare Load Key Failed";
                        passed = false;
                    }
                    else
                    {
                        passed = MifareTestAllBlocks(smartCardType, "KeyA", 1, fullTest, out errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                passed = false;
                errMsg = ex.Message;
            }
            finally
            {
                dataIn = null;
                dataOut = null;

                Disconnect();
                ReleaseContext();
            }
            return passed;
        }

        public bool MifareTest(ref string smartCardType, string readerName, byte[] key, bool fullTest,
                               string dataToEncode, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] dataIn = null;
            byte[] dataOut = null;

            bool passed = false;
            try
            {
                dataIn = new byte[8];
                dataOut = new byte[8];
                int blockEnd = 0;

                int ret = EstablishContext(out errMsg);
                if (ret != 0)
                    return false;

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);

                    ret = Connect(readerName);
                    if (ret == 0)
                    {
                        passed = true;
                        break;
                    }

                    ReleaseContext();
                    Thread.Sleep(500);
                    EstablishContext(out errMsg);
                }

                if (!passed)
                {
                    errMsg = "Card Connection Failed [" + ret.ToString() + "]";
                }
                else if (SetCardType("TypeA") != 0)
                {
                    errMsg = "Mifare Set Card Type Failed";
                    passed = false;
                }
                else if (SetPUPI(true) != 0)
                {
                    errMsg = "Mifare Set PUPI Failed";
                    passed = false;
                }
                else if (GetCardTypeFromATR(readerName, ref smartCardType, out errMsg) != 0)
                {
                    errMsg = "Mifare " + errMsg;
                    passed = false;
                }

                if (smartCardType == "MIFARE DESFIRE")
                {
                    if (fullTest)
                        passed = DesfireTest(dataToEncode, ref errMsg);

                    return passed;
                }

                byte baseByte = Convert.ToByte(dataToEncode);
                int dataSize = 4;

                for (int i = 0; i < dataSize; i++)
                    dataIn[i] = baseByte;

                if (errMsg == "" && smartCardType == "MIFARE ULTRALIGHT")
                {
                    dataSize = 4;

                    if (fullTest)
                        blockEnd = 15;
                    else
                        blockEnd = 4;

                    for (int block = 4; block <= blockEnd; block++)
                    {
                        ret = MifareBlockWrite(block, dataIn, dataSize, out errMsg);
                        if (ret != 0)
                        {
                            errMsg = "MIFARE ULTRALIGHT Write Error: " + ret.ToString();
                            passed = false;
                            break;
                        }
                    }

                    if (passed)
                    {
                        for (int block = 4; block <= blockEnd; block++)
                        {
                            ret = MifareBlockRead(block, ref dataOut, ref dataSize, out errMsg);
                            if (ret != 0 || dataSize != 4)
                            {
                                errMsg = "MIFARE ULTRALIGHT Read Error: " + ret.ToString();
                                passed = false;
                                break;
                            }

                            if (dataOut[0] != baseByte || dataOut[1] != baseByte ||
                                dataOut[2] != baseByte || dataOut[3] != baseByte)
                            {
                                errMsg = "MIFARE ULTRALIGHT Block [" + block.ToString() +
                                        "] Read does not equal Written";
                                passed = false;
                                break;
                            }
                        }
                    }
                }
                else if (errMsg == "")
                {
                    if (LoadKey("KeyA", key.Length, key, out errMsg) != 0)
                    {
                        errMsg = "Mifare Load Key Failed";
                        passed = false;
                    }
                    else
                    {
                        passed = MifareTestAllBlocks(smartCardType, "KeyA", 1, fullTest, dataToEncode, out errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                passed = false;
                errMsg = ex.Message;
            }
            finally
            {
                dataIn = null;
                dataOut = null;

                Disconnect();
                ReleaseContext();
            }
            return passed;
        }

        private bool MifareTestAllBlocks(string smartCardType, string keyType, byte keyNumber, bool fullTest,
                                        out string errMsg)
        {
            errMsg = string.Empty;
            bool passed = true;
            try
            {
                byte data = 0x00;
                int blockEnd = 0;

                switch (smartCardType)
                {
                    case "MIFARE 1K":

                        if (fullTest)
                            blockEnd = 60;
                        else
                            blockEnd = 4;

                        for (int block = 4; block <= blockEnd; block += 4)
                        {
                            passed = MifareSectorTest(keyType, keyNumber, block, 3, data, out errMsg);
                            if (!passed)
                                break;

                            data++;
                        }
                        break;

                    case "MIFARE 4K":

                        if (fullTest)
                            blockEnd = 127; //60
                        else
                            blockEnd = 4;

                        //sectors 0 - 31 have 0 - 3 blocks
                        for (int block = 4; block <= blockEnd; block += 4)
                        {
                            passed = MifareSectorTest(keyType, keyNumber, block, 3, data, out errMsg);
                            if (!passed)
                                break;

                            data++;
                        }

                        if (passed && fullTest)
                        {
                            //sectors 32 - 39 have 0 - 15 blocks
                            //block starts at 128 => block 0 of sector 32
                            blockEnd = 255; //block 15 of sector 39
                            for (int block = 128; block <= blockEnd; block += 16)
                            {
                                passed = MifareSectorTest(keyType, keyNumber, block, 15, data, out errMsg);
                                if (!passed)
                                    break;

                                data++;
                            }
                        }
                        break;
                }

                return passed;
            }
            catch (Exception ex)
            {
                passed = false;
                errMsg = ex.Message;
            }
            return passed;
        }

        private bool MifareTestAllBlocks(string smartCardType, string keyType, byte keyNumber, bool fullTest,
                                        string dataToEncode, out string errMsg)
        {
            errMsg = string.Empty;

            bool passed = true;
            try
            {
                byte data = Convert.ToByte(dataToEncode);
                int blockEnd = 0;

                switch (smartCardType)
                {
                    case "MIFARE 1K":

                        if (fullTest)
                            blockEnd = 60;
                        else
                            blockEnd = 4;

                        for (int block = 4; block <= blockEnd; block += 4)
                        {
                            passed = MifareSectorTest(keyType, keyNumber, block, 3, data, out errMsg);
                            if (!passed)
                                break;
                        }
                        break;

                    case "MIFARE 4K":

                        if (fullTest)
                            blockEnd = 127; //60
                        else
                            blockEnd = 4;

                        //sectors 0 - 31 have 0 - 3 blocks
                        for (int block = 4; block <= blockEnd; block += 4)
                        {
                            passed = MifareSectorTest(keyType, keyNumber, block, 3, data, out errMsg);
                            if (!passed)
                                break;
                        }

                        if (passed && fullTest)
                        {
                            //sectors 32 - 39 have 0 - 15 blocks
                            //block starts at 128 => block 0 of sector 32
                            blockEnd = 255; //block 15 of sector 39
                            for (int block = 128; block <= blockEnd; block += 16)
                            {
                                passed = MifareSectorTest(keyType, keyNumber, block, 15, data, out errMsg);
                                if (!passed)
                                    break;
                            }
                        }
                        break;
                }

                return passed;
            }
            catch (Exception ex)
            {
                passed = false;
                errMsg = ex.Message;
            }
            return passed;
        }

        private bool MifareSectorTest(string keyType, byte keyNumber, int blockStart, int blockCount, byte baseData, out string errMsg)
        {
            byte[] dataIn = null;
            byte[] dataOut = null;
            bool passed = true;
            
            errMsg = string.Empty;
            try
            {
                byte data = baseData;
                dataIn = new byte[20];
                dataOut = new byte[20];

                int dataSize = 16;
                int ret = 0;

                for (int block = blockStart; block < blockStart + blockCount; block++)
                {
                    ret = MifareAuthenticate(block, keyType, keyNumber, out errMsg);
                    if (ret != 0)
                    {
                        errMsg = "Mifare Authentication Failed";
                        passed = false;
                        break;
                    }

                    dataSize = 16;

                    for (int i = 0; i < 16; i++)
                        dataIn[i] = data++;

                    if (MifareBlockWrite(block, dataIn, 16, out errMsg) != 0)
                    {
                        errMsg = "Mifare Write Error";
                        passed = false;
                    }
                    else if (MifareBlockRead(block, ref dataOut, ref dataSize, out errMsg) != 0)
                    {
                        errMsg = "Mifare Read Error";
                        passed = false;
                    }

                    if (passed)
                    {
                        for (int i = 0; i < dataSize; i++)
                        {
                            if (dataIn[i] != dataOut[i])
                            {
                                errMsg = "Mifare Read does not equal Written";
                                passed = false;
                                break;
                            }
                        }
                    }
                    if (!passed)
                        break;
                }
                return passed;
            }
            catch (Exception ex)
            {
                passed = false;
                errMsg = ex.Message;
            }
            finally
            {
                dataIn = null;
                dataOut = null;
            }
            return passed;
        }

        private int MifareAuthenticate(int block, string keyType, byte keyNumber, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] respBuf = null;
            int ret = -1;
            try
            {
                byte kType = 0x60;
                switch (keyType.ToLower())
                {
                    case "keyb":
                    case "key b":
                        kType = 0x61;
                        break;
                }

                byte[] blk = BlockToBytes(block);
                byte[] apdu = { 0xFF, 0x86, 0x00, 0x00, 0x05, 0x01, blk[0], blk[1], kType, keyNumber };

                respBuf = new byte[1024];
                int respSize = respBuf.Length;

                int sendLen = 10;

                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref apdu[0], sendLen, ref sIO, ref respBuf[0],
                                                 ref respSize);

                if (ret != 0)
                {
                    errMsg = WinSCard.GetSCardErrMsg(ret);
                    return ret;
                }

                if (respBuf[0] != 0x90 && respBuf[1] != 0x00)
                {
                    errMsg = "Invalid Key";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
            }
            return ret;
        }

        private int MifareBlockRead(int block, ref byte[] dataOut, ref int dataSize, out string errMsg)
        {
            errMsg = string.Empty;
            byte[] respBuf = null;
            int ret = -1;
            try
            {
                byte[] blk = BlockToBytes(block);
                byte[] cmd = { 0xFF, 0xB0, blk[0], blk[1], 0x00 };

                respBuf = new byte[128];
                int cmdLength = cmd.Length;
                int respSize = respBuf.Length;

                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref cmd[0], cmdLength, ref sIO,
                                             ref respBuf[0], ref respSize);

                if (ret != 0)
                {
                    errMsg = WinSCard.GetSCardErrMsg(ret);
                    return ret;
                }

                if (respSize < 2)
                {
                    errMsg = "Data Read Size = 0";
                    return -1;
                }

                if (respSize > dataSize + 2)
                {
                    errMsg = "Data Read is to Large for Buffer";
                    return -2;
                }

                if (respBuf[respSize - 2] != 0x90 || respBuf[respSize - 1] != 0x00)
                {
                    errMsg = "Invalid Read Process";
                    return -3;
                }

                for (int i = 0; i < respSize - 2; i++)
                    dataOut[i] = respBuf[i];

                dataSize = respSize - 2;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                respBuf = null;
            }
            return ret;
        }

        private int MifareBlockWrite(int block, byte[] data, int dataSize, out string errMsg)
        {
            errMsg = string.Empty;
            byte[] cmd = null;
            byte[] respBuf = null;
            int ret = -1;
            try
            {
                byte[] blk = BlockToBytes(block);
                
                cmd = new byte[dataSize + 5];
                int cmdLength = cmd.Length;

                cmd[0] = 0xFF;
                cmd[1] = 0xD6;
                cmd[2] = blk[0];
                cmd[3] = blk[1];
                cmd[4] = (byte)dataSize;

                for (int i = 0; i < dataSize; i++)
                    cmd[i + 5] = data[i];

                int respSize = 8;
                respBuf = new byte[respSize];

                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref cmd[0], cmdLength, ref sIO,
                                             ref respBuf[0], ref respSize);

                if (ret != 0)
                {
                    errMsg = WinSCard.GetSCardErrMsg(ret);
                    return -1;
                }

                if (respSize != 2)
                {
                    errMsg = "Invalid Response Size";
                    return -2;
                }

                if (respBuf[0] != 0x90 || respBuf[1] != 0x00)
                {
                    errMsg = "Write Block Operation Failed";
                    return -3;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                cmd = null;
                respBuf = null;
            }
            return ret;
        }

        // Get ATR: check the returned ATR against MIFARE ATRs 
        private bool CheckMifareATR(string cardType, byte[] atr)
        {
            bool found = true;
            byte[] buf = null;
            try
            {
                switch (cardType)
                {
                    case "1K":
                        buf = bufMifare;
                        buf[0x0E] = 0x01;
                        buf[0x13] = 0x6A;
                        break;

                    case "4K":
                        buf = bufMifare;
                        buf[0x0E] = 0x02;
                        buf[0x13] = 0x69;
                        break;

                    case "Ultralight":
                        buf = bufMifare;
                        buf[0x0E] = 0x03;
                        buf[0x13] = 0x68;
                        break;

                    case "Desfire":
                        buf = bufDesfireATR;
                        break;
                }

                for (int i = 0; i < buf.Length; i++)
                {
                    if (buf[i] != atr[i])
                    {
                        found = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                found = false;
                throw ex;
            }
            return found;
        }

        private int GetCardTypeFromATR(string readerName, ref string cardName, out string errMsg)
        {
            errMsg = string.Empty;
            
            byte[] atr = null;
            int ret = -1;
            try
            {
                int atrLen = 33,
                    protocol = 0,
                    readerLen = 0,
                    state = 0;

                atr = new byte[atrLen];

                ret = WinSCard.SCardStatus(_cardHandle, readerName, ref readerLen, ref state, ref protocol,
                                           ref atr[0], ref atrLen);

                if (ret != 0)
                {
                    errMsg = "Get ATR Failed";
                    return ret;
                }

                if (CheckMifareATR("1K", atr))
                {
                    cardName = "MIFARE 1K";
                }
                else if (CheckMifareATR("4K", atr))
                {
                    cardName = "MIFARE 4K";
                }
                else if (CheckMifareATR("Ultralight", atr))
                {
                    cardName = "MIFARE ULTRALIGHT";
                }
                else if (CheckMifareATR("Desfire", atr))
                {
                    cardName = "MIFARE DESFIRE";
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                atr = null;
            }
            return ret;
        }
        
        #endregion //Mifare Tests

        #region Context Support

        private int EstablishContext(out string errMsg)
        {
            errMsg = string.Empty;
            int ret = -1;
            try
            {
                ret = WinSCard.SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, 0, 0, ref _context);

                if (ret != 0)
                    errMsg = WinSCard.GetSCardErrMsg(ret);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return ret;
        }

        private int ReleaseContext()
        {
            try
            {
                return WinSCard.SCardReleaseContext(_context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion //Context

        #region Connection Support

        //connect to a smart card
        public int Connect(string readerName)
        {
            int ret = 0;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    ret = WinSCard.SCardConnect(_context, readerName, WinSCard.SCARD_SHARE_SHARED,
                                                WinSCard.SCARD_PROTOCOL_T0 | WinSCard.SCARD_PROTOCOL_T1,
                                                ref _cardHandle, ref _activeProtocol);
                    if (ret == 0)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        //disconnect from a card
        public int Disconnect()
        {
            try
            {
                return WinSCard.SCardDisconnect(_cardHandle, WinSCard.SCARD_UNPOWER_CARD);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DisconnectPreviousHandle()
        {
            try
            {
                if (_previousCardHandle != 0)
                    return WinSCard.SCardDisconnect(_previousCardHandle, WinSCard.SCARD_UNPOWER_CARD);
                else return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion //Connect

        #region SmartCard Reader Methods

        // Get RF Switch - determines if the RF is on
        public int GetRFSwitch(out bool enabled)
        {
            enabled = false;

            byte[] rcvBuf = null;
            byte[] cmd = { 0x96, 0xFF };
            try
            {
                int rcvBufLen = 262;
                int bytesRet = 0;

                rcvBuf = new byte[262];
                Array.Clear(rcvBuf, 0, 262);

                int ret = WinSCard.SCardControlEx(_cardHandle, IOCTL_CCID_ESCAPE, ref cmd, 2, ref rcvBuf,
                                                   rcvBufLen, ref bytesRet);

                if (ret == 0 && rcvBuf[0] == 0x00)
                    enabled = true;

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rcvBuf = null;
                cmd = null;
            }
        }

        // Load Key - loads a key into a reader
        public int LoadKey(string keyType, int keyLength, byte[] key, out string errMsg)
        {
            errMsg = string.Empty;
            byte[] cmd = null;
            byte[] respBuf = null;
            try
            {
                byte kType = 0x00;
                switch (keyType.ToLower())
                {
                    case "keya":
                    case "key a":
                        kType = 0x60;
                        break;

                    case "keyb":
                    case "key b":
                        kType = 0x61;
                        break;
                }

                cmd = new byte[5 + keyLength];
                cmd[0] = 0xFF;
                cmd[1] = 0x82;
                cmd[2] = 0x00;
                cmd[3] = kType;
                cmd[4] = (byte)keyLength;

                for (int i = 0; i < keyLength; i++)
                    cmd[i + 5] = key[i];

                int cmdLength = keyLength + 5;

                int respSize = 128;
                respBuf = new byte[respSize];

                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                int ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref cmd[0], cmdLength, ref sIO,
                                                    ref respBuf[0], ref respSize);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd = null;
                respBuf = null;
            }
        }

        // Set Card Type - configures the type of card the reader will support
        public int SetCardType(string cardType)
        {
            byte[] rcvBuf = null;
            try
            {
                byte cType = 0x02;

                switch (cardType.ToLower())
                {
                    case "keya":
                    case "key a":
                        cType = 0x00;
                        break;

                    case "keyb":
                    case "key b":
                        cType = 0x01;
                        break;
                }

                byte[] cmd = { 0x95, cType };

                int rcvBufLen = 262;
                int bytesRet = 0;

                rcvBuf = new byte[262];
                Array.Clear(rcvBuf, 0, 262);

                return WinSCard.SCardControlEx(_cardHandle, IOCTL_CCID_ESCAPE, ref cmd, 2, ref rcvBuf,
                                               rcvBufLen, ref bytesRet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rcvBuf = null;
            }
        }

        // Set PUPI - enables or disables PUPI
        public int SetPUPI(bool enable)
        {
            byte[] rcvBuf = null;
            try
            {
                byte pupiState = 0x00;
                if (enable) pupiState = 0x01;

                byte[] cmd = { 0x9B, pupiState };

                int rcvBufLen = 262;
                int bytesRet = 0;

                rcvBuf = new byte[262];
                Array.Clear(rcvBuf, 0, 262);

                return WinSCard.SCardControlEx(_cardHandle, IOCTL_CCID_ESCAPE, ref cmd, 2, ref rcvBuf,
                                                rcvBufLen, ref bytesRet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rcvBuf = null;
            }
        }

        // Sends a command to a Reader
        private int SCardTransmit(ref byte[] apdu, int sendLen, ref byte[] respBuf, ref int respSize)
        {
            try
            {
                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                return WinSCard.SCardTransmit(_cardHandle, ref sIO, ref apdu[0], sendLen, ref sIO,
                                              ref respBuf[0], ref respSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion //SmartCard Reader Methods
    }

    /**************************************************************************************************
    * Class Name: DesfireEncryption
    *  
    * Purpose: To provide encryption support for Desfire contactless smartcards. 
    *           
    ***************************************************************************************************/
    public class DesfireEncryption
    {
#region Structures

        private struct _des_ctx
        {
            public UInt32[] encrypt_subkeys;// = new long[32];
            public UInt32[] decrypt_subkeys;// = new long[32];
        }
        _des_ctx ctx;

#endregion //Structures

#region Variables
        /*
         * The s-box values are permuted according to the 'primitive function P'
         * and are rotated one bit to the left.
         */
        private UInt32[] sbox1 = new UInt32[64]
        {
          0x01010400, 0x00000000, 0x00010000, 0x01010404, 0x01010004, 0x00010404, 0x00000004, 0x00010000,
          0x00000400, 0x01010400, 0x01010404, 0x00000400, 0x01000404, 0x01010004, 0x01000000, 0x00000004,
          0x00000404, 0x01000400, 0x01000400, 0x00010400, 0x00010400, 0x01010000, 0x01010000, 0x01000404,
          0x00010004, 0x01000004, 0x01000004, 0x00010004, 0x00000000, 0x00000404, 0x00010404, 0x01000000,
          0x00010000, 0x01010404, 0x00000004, 0x01010000, 0x01010400, 0x01000000, 0x01000000, 0x00000400,
          0x01010004, 0x00010000, 0x00010400, 0x01000004, 0x00000400, 0x00000004, 0x01000404, 0x00010404,
          0x01010404, 0x00010004, 0x01010000, 0x01000404, 0x01000004, 0x00000404, 0x00010404, 0x01010400,
          0x00000404, 0x01000400, 0x01000400, 0x00000000, 0x00010004, 0x00010400, 0x00000000, 0x01010004
        };

        private UInt32[] sbox2 = new UInt32[64]
        {
          0x80108020, 0x80008000, 0x00008000, 0x00108020, 0x00100000, 0x00000020, 0x80100020, 0x80008020,
          0x80000020, 0x80108020, 0x80108000, 0x80000000, 0x80008000, 0x00100000, 0x00000020, 0x80100020,
          0x00108000, 0x00100020, 0x80008020, 0x00000000, 0x80000000, 0x00008000, 0x00108020, 0x80100000,
          0x00100020, 0x80000020, 0x00000000, 0x00108000, 0x00008020, 0x80108000, 0x80100000, 0x00008020,
          0x00000000, 0x00108020, 0x80100020, 0x00100000, 0x80008020, 0x80100000, 0x80108000, 0x00008000,
          0x80100000, 0x80008000, 0x00000020, 0x80108020, 0x00108020, 0x00000020, 0x00008000, 0x80000000,
          0x00008020, 0x80108000, 0x00100000, 0x80000020, 0x00100020, 0x80008020, 0x80000020, 0x00100020,
          0x00108000, 0x00000000, 0x80008000, 0x00008020, 0x80000000, 0x80100020, 0x80108020, 0x00108000
        };

        private UInt32[] sbox3 = new UInt32[64]
        {
          0x00000208, 0x08020200, 0x00000000, 0x08020008, 0x08000200, 0x00000000, 0x00020208, 0x08000200,
          0x00020008, 0x08000008, 0x08000008, 0x00020000, 0x08020208, 0x00020008, 0x08020000, 0x00000208,
          0x08000000, 0x00000008, 0x08020200, 0x00000200, 0x00020200, 0x08020000, 0x08020008, 0x00020208,
          0x08000208, 0x00020200, 0x00020000, 0x08000208, 0x00000008, 0x08020208, 0x00000200, 0x08000000,
          0x08020200, 0x08000000, 0x00020008, 0x00000208, 0x00020000, 0x08020200, 0x08000200, 0x00000000,
          0x00000200, 0x00020008, 0x08020208, 0x08000200, 0x08000008, 0x00000200, 0x00000000, 0x08020008,
          0x08000208, 0x00020000, 0x08000000, 0x08020208, 0x00000008, 0x00020208, 0x00020200, 0x08000008,
          0x08020000, 0x08000208, 0x00000208, 0x08020000, 0x00020208, 0x00000008, 0x08020008, 0x00020200
        };

        private UInt32[] sbox4 = new UInt32[64]
        {
          0x00802001, 0x00002081, 0x00002081, 0x00000080, 0x00802080, 0x00800081, 0x00800001, 0x00002001,
          0x00000000, 0x00802000, 0x00802000, 0x00802081, 0x00000081, 0x00000000, 0x00800080, 0x00800001,
          0x00000001, 0x00002000, 0x00800000, 0x00802001, 0x00000080, 0x00800000, 0x00002001, 0x00002080,
          0x00800081, 0x00000001, 0x00002080, 0x00800080, 0x00002000, 0x00802080, 0x00802081, 0x00000081,
          0x00800080, 0x00800001, 0x00802000, 0x00802081, 0x00000081, 0x00000000, 0x00000000, 0x00802000,
          0x00002080, 0x00800080, 0x00800081, 0x00000001, 0x00802001, 0x00002081, 0x00002081, 0x00000080,
          0x00802081, 0x00000081, 0x00000001, 0x00002000, 0x00800001, 0x00002001, 0x00802080, 0x00800081,
          0x00002001, 0x00002080, 0x00800000, 0x00802001, 0x00000080, 0x00800000, 0x00002000, 0x00802080
        };

        private UInt32[] sbox5 = new UInt32[64]
        {
          0x00000100, 0x02080100, 0x02080000, 0x42000100, 0x00080000, 0x00000100, 0x40000000, 0x02080000,
          0x40080100, 0x00080000, 0x02000100, 0x40080100, 0x42000100, 0x42080000, 0x00080100, 0x40000000,
          0x02000000, 0x40080000, 0x40080000, 0x00000000, 0x40000100, 0x42080100, 0x42080100, 0x02000100,
          0x42080000, 0x40000100, 0x00000000, 0x42000000, 0x02080100, 0x02000000, 0x42000000, 0x00080100,
          0x00080000, 0x42000100, 0x00000100, 0x02000000, 0x40000000, 0x02080000, 0x42000100, 0x40080100,
          0x02000100, 0x40000000, 0x42080000, 0x02080100, 0x40080100, 0x00000100, 0x02000000, 0x42080000,
          0x42080100, 0x00080100, 0x42000000, 0x42080100, 0x02080000, 0x00000000, 0x40080000, 0x42000000,
          0x00080100, 0x02000100, 0x40000100, 0x00080000, 0x00000000, 0x40080000, 0x02080100, 0x40000100
        };

        private UInt32[] sbox6 = new UInt32[64]
        {
          0x20000010, 0x20400000, 0x00004000, 0x20404010, 0x20400000, 0x00000010, 0x20404010, 0x00400000,
          0x20004000, 0x00404010, 0x00400000, 0x20000010, 0x00400010, 0x20004000, 0x20000000, 0x00004010,
          0x00000000, 0x00400010, 0x20004010, 0x00004000, 0x00404000, 0x20004010, 0x00000010, 0x20400010,
          0x20400010, 0x00000000, 0x00404010, 0x20404000, 0x00004010, 0x00404000, 0x20404000, 0x20000000,
          0x20004000, 0x00000010, 0x20400010, 0x00404000, 0x20404010, 0x00400000, 0x00004010, 0x20000010,
          0x00400000, 0x20004000, 0x20000000, 0x00004010, 0x20000010, 0x20404010, 0x00404000, 0x20400000,
          0x00404010, 0x20404000, 0x00000000, 0x20400010, 0x00000010, 0x00004000, 0x20400000, 0x00404010,
          0x00004000, 0x00400010, 0x20004010, 0x00000000, 0x20404000, 0x20000000, 0x00400010, 0x20004010
        };

        private UInt32[] sbox7 = new UInt32[64]
        {
          0x00200000, 0x04200002, 0x04000802, 0x00000000, 0x00000800, 0x04000802, 0x00200802, 0x04200800,
          0x04200802, 0x00200000, 0x00000000, 0x04000002, 0x00000002, 0x04000000, 0x04200002, 0x00000802,
          0x04000800, 0x00200802, 0x00200002, 0x04000800, 0x04000002, 0x04200000, 0x04200800, 0x00200002,
          0x04200000, 0x00000800, 0x00000802, 0x04200802, 0x00200800, 0x00000002, 0x04000000, 0x00200800,
          0x04000000, 0x00200800, 0x00200000, 0x04000802, 0x04000802, 0x04200002, 0x04200002, 0x00000002,
          0x00200002, 0x04000000, 0x04000800, 0x00200000, 0x04200800, 0x00000802, 0x00200802, 0x04200800,
          0x00000802, 0x04000002, 0x04200802, 0x04200000, 0x00200800, 0x00000000, 0x00000002, 0x04200802,
          0x00000000, 0x00200802, 0x04200000, 0x00000800, 0x04000002, 0x04000800, 0x00000800, 0x00200002
        };

        private UInt32[] sbox8 = new UInt32[64]
        {
          0x10001040, 0x00001000, 0x00040000, 0x10041040, 0x10000000, 0x10001040, 0x00000040, 0x10000000,
          0x00040040, 0x10040000, 0x10041040, 0x00041000, 0x10041000, 0x00041040, 0x00001000, 0x00000040,
          0x10040000, 0x10000040, 0x10001000, 0x00001040, 0x00041000, 0x00040040, 0x10040040, 0x10041000,
          0x00001040, 0x00000000, 0x00000000, 0x10040040, 0x10000040, 0x10001000, 0x00041040, 0x00040000,
          0x00041040, 0x00040000, 0x10041000, 0x00001000, 0x00000040, 0x10040040, 0x00001000, 0x00041040,
          0x10001000, 0x00000040, 0x10000040, 0x10040000, 0x10040040, 0x10000000, 0x00040000, 0x10001040,
          0x00000000, 0x10041040, 0x00040040, 0x10000040, 0x10040000, 0x10001000, 0x10001040, 0x00000000,
          0x10041040, 0x00041000, 0x00041000, 0x00001040, 0x00001040, 0x00040040, 0x10000000, 0x10041000
        };

        /*
         * These two tables are part of the 'permuted choice 1' function.
         * In this implementation several speed improvements are done.
         */
        private UInt32[] leftkey_swap = new UInt32[16]
        {
          0x00000000, 0x00000001, 0x00000100, 0x00000101,
          0x00010000, 0x00010001, 0x00010100, 0x00010101,
          0x01000000, 0x01000001, 0x01000100, 0x01000101,
          0x01010000, 0x01010001, 0x01010100, 0x01010101
        };

        private UInt32[] rightkey_swap = new UInt32[16]
        {
          0x00000000, 0x01000000, 0x00010000, 0x01010000,
          0x00000100, 0x01000100, 0x00010100, 0x01010100,
          0x00000001, 0x01000001, 0x00010001, 0x01010001,
          0x00000101, 0x01000101, 0x00010101, 0x01010101,
        };

        /*
         * Numbers of left shifts per round for encryption subkeys.
         * To calculate the decryption subkeys we just reverse the
         * ordering of the calculated encryption subkeys. So their
         * is no need for a decryption rotate tab.
         */
        private byte[] encrypt_rotate_tab = new byte[16]
        {
          1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };
#endregion //Variables

#region Send & Receive

        public void Desfire_xDes_Recv(byte[] data, int blocks, byte[] key)
        {
            try
            {
                bool tripleDES = false;

                for (int i = 0; i < key.Length / 2; i++)
                {
                    if (key[i] != key[i + 8])
                        tripleDES = true;
                }
                if (tripleDES)
                    Desfire_3DES_Recv(data, blocks, key);
                else
                    Desfire_DES_Recv(data, blocks, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* Encipher data to be sent to the DESFire */
        public void Desfire_xDES_Send(byte[] data, int blocks, byte[] key)
        {
            try
            {
                bool tripleDES = false;

                for (int i = 0; i < key.Length / 2; i++)
                {
                    if (key[i] != key[i + 8])
                        tripleDES = true;
                }
                if (tripleDES)
                    Desfire_3DES_Send(data, blocks, key);
                else
                    Desfire_DES_Send(data, blocks, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Desfire_3DES_Recv(byte[] data, int blocks, byte[] key)
        {
        }

        private void Desfire_DES_Recv(byte[] data, int blocks, byte[] key)
        {
            byte[] carry = null;
            byte[] buffer = null;
            byte[] tempData = null;
            try
            {
                int i = 0;
                byte j = 0;
                carry = new byte[8];
                buffer = new byte[8];

                DES_Init(key);
                Array.Clear(buffer, 0, 8);
                for (i = 0; i < blocks; i++)
                {
                    Array.Copy(buffer, carry, 8);
                    Array.Copy(data, 8 * i, buffer, 0, 8);

                    tempData = new byte[8];
                    Array.Copy(data, 8 * i, tempData, 0, 8);

                    des_ecb_crypt(ctx, buffer, tempData, true);
                    Array.Copy(tempData, 0, data, 8 * i, 8);

                    for (j = 0; j < 8; j++)
                        data[8 * i + j] ^= carry[j];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                carry = null;
                buffer = null;
                tempData = null;
            }
        }

        private void Desfire_3DES_Send(byte[] data, int blocks, byte[] key)
        {
        }

        /* DES decryption in CBC send mode */
        private void Desfire_DES_Send(byte[] data, int blocks, byte[] key)
        {
            byte[] carry = null;
            byte[] tempData = null;
            int i;
            byte j;
            try
            {
                carry = new byte[8];

                DES_Init(key);
                Array.Clear(carry, 0, 8);

                for (i = 0; i < blocks; i++)
                {
                    for (j = 0; j < 8; j++)
                        data[8 * i + j] ^= carry[j];    /* P  <- P XOR IV  */

                    tempData = new byte[8];
                    Array.Copy(data, 8 * i, tempData, 0, 8);

                    des_ecb_crypt(ctx, tempData, carry, true);  /* IV <- iDES(P)   */
                    Array.Copy(carry, 0, data, 8 * i, 8);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                carry = null;
                tempData = null;
            }
        }

#endregion //Send & Receive

#region Encrypt/Decrypt Methods
        // Macro to swap bits across two words.
        private void DO_PERMUTATION(ref UInt32 a, out UInt32 temp, ref UInt32 b, UInt32 offset,
                                    UInt32 mask)
        {
            try
            {
                temp = ((a >> (int)offset) ^ b) & mask;
                b ^= temp;
                a ^= temp << (int)offset;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // This performs the 'initial permutation' of the data to be encrypted
        // or decrypted. Additionally the resulting two words are rotated one bit to the left.
        private void INITIAL_PERMUTATION(ref UInt32 left, out UInt32 temp, ref UInt32 right)
        {
            try
            {
                DO_PERMUTATION(ref left, out temp, ref right, 4, 0x0f0f0f0f);
                DO_PERMUTATION(ref left, out temp, ref right, 16, 0x0000ffff);
                DO_PERMUTATION(ref right, out temp, ref left, 2, 0x33333333);
                DO_PERMUTATION(ref right, out temp, ref left, 8, 0x00ff00ff);

                right = (right << 1) | (right >> 31);
                temp = (left ^ right) & 0xaaaaaaaa;
                right ^= temp;
                left ^= temp;
                left = (left << 1) | (left >> 31);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // The 'inverse initial permutation'.
        private void FINAL_PERMUTATION(ref UInt32 left, out UInt32 temp, ref UInt32 right)
        {
            try
            {
                left = (left << 31) | (left >> 1);
                temp = (left ^ right) & 0xaaaaaaaa;
                left ^= temp;
                right ^= temp;
                right = (right << 31) | (right >> 1);

                DO_PERMUTATION(ref right, out temp, ref left, 8, 0x00ff00ff);
                DO_PERMUTATION(ref right, out temp, ref left, 2, 0x33333333);
                DO_PERMUTATION(ref left, out temp, ref right, 16, 0x0000ffff);
                DO_PERMUTATION(ref left, out temp, ref right, 4, 0x0f0f0f0f);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // A full DES round including 'expansion function', 'sbox substitution'
        // and 'primitive function P' but without swapping the left and right word.
        // Note: The data in 'from' and 'to' is already rotated one bit to
        //       the left, done in the initial permutation.
        private void DES_ROUND(UInt32 from, ref UInt32 to, out UInt32 work, UInt32[] subkey, ref int index)
        {
            try
            {
                work = from ^ subkey[index++];
                to ^= sbox8[work & 0x3f];
                to ^= sbox6[(work >> 8) & 0x3f];
                to ^= sbox4[(work >> 16) & 0x3f];
                to ^= sbox2[(work >> 24) & 0x3f];

                work = ((from << 28) | (from >> 4)) ^ subkey[index++];
                to ^= sbox7[work & 0x3f];
                to ^= sbox5[(work >> 8) & 0x3f];
                to ^= sbox3[(work >> 16) & 0x3f];
                to ^= sbox1[(work >> 24) & 0x3f];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endregion //Encrypt/Decrypt Methods

#region Macro Converters
        // Macros to convert 8 bytes from/to 32bit words.
        
        private void READ_64BIT_DATA(byte[] data, out UInt32 left, out UInt32 right)
        {
            try
            {
                left = (UInt32)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
                right = (UInt32)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WRITE_64BIT_DATA(byte[] data, UInt32 left, UInt32 right)
        {
            try
            {
                data[0] = Convert.ToByte((left >> 24) & 0xff);
                data[1] = Convert.ToByte((left >> 16) & 0xff);
                data[2] = Convert.ToByte((left >> 8) & 0xff);
                data[3] = Convert.ToByte(left & 0xff);
                data[4] = Convert.ToByte((right >> 24) & 0xff);
                data[5] = Convert.ToByte((right >> 16) & 0xff);
                data[6] = Convert.ToByte((right >> 8) & 0xff);
                data[7] = Convert.ToByte(right & 0xff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endregion //Macro Converters

#region Initialization

        /* Fill a DES context with subkeys calculated from a 64bit key.
         * Does not check parity bits, simply ignores them.
         * Does not check for weak keys.
         */
        private int DES_Init(byte[] key)
        {
            try
            {
                int i = 0;

                ctx.encrypt_subkeys = null;
                ctx.decrypt_subkeys = null;

                ctx = new _des_ctx();
                ctx.encrypt_subkeys = new UInt32[32];
                ctx.decrypt_subkeys = new UInt32[32];

                des_key_schedule(key, ctx.encrypt_subkeys);

                for (i = 0; i < 32; i += 2)
                {
                    ctx.decrypt_subkeys[i] = ctx.encrypt_subkeys[30 - i];
                    ctx.decrypt_subkeys[i + 1] = ctx.encrypt_subkeys[31 - 1];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        /* des_key_schedule():
         * Calculate 16 subkeys pairs (even/odd) for
         *			  16 encryption rounds.
         *			  To calculate subkeys for decryption the caller
         *			  have to reorder the generated subkeys.
         *
         *    rawkey: 8 Bytes of key data
         *    subkey: Array of at least 32 u32s. Will be filled
         *		      with calculated subkeys.
         *
        */
        private void des_key_schedule(byte[] rawkey, UInt32[] subkey)
        {
            UInt32 left, right, work;
            int round;
            try
            {
                READ_64BIT_DATA(rawkey, out left, out right);
                DO_PERMUTATION(ref right, out work, ref left, 4, 0x0f0f0f0f);
                DO_PERMUTATION(ref right, out work, ref left, 0, 0x10101010);

                left = (leftkey_swap[(left >> 0) & 0xf] << 3) | (leftkey_swap[(left >> 8) & 0xf] << 2)
                        | (leftkey_swap[(left >> 16) & 0xf] << 1) | (leftkey_swap[(left >> 24) & 0xf])
                        | (leftkey_swap[(left >> 5) & 0xf] << 7) | (leftkey_swap[(left >> 13) & 0xf] << 6)
                        | (leftkey_swap[(left >> 21) & 0xf] << 5) | (leftkey_swap[(left >> 29) & 0xf] << 4);

                left &= 0x0fffffff;

                right = (rightkey_swap[(right >> 1) & 0xf] << 3) | (rightkey_swap[(right >> 9) & 0xf] << 2)
                        | (rightkey_swap[(right >> 17) & 0xf] << 1) | (rightkey_swap[(right >> 25) & 0xf])
                        | (rightkey_swap[(right >> 4) & 0xf] << 7) | (rightkey_swap[(right >> 12) & 0xf] << 6)
                        | (rightkey_swap[(right >> 20) & 0xf] << 5) | (rightkey_swap[(right >> 28) & 0xf] << 4);

                right &= 0x0fffffff;

                int keyIndex = 0;
                for (round = 0; round < 16; ++round)
                {
                    left = ((left << encrypt_rotate_tab[round]) | (left >> (28 - encrypt_rotate_tab[round]))) & 0x0fffffff;
                    right = ((right << encrypt_rotate_tab[round]) | (right >> (28 - encrypt_rotate_tab[round]))) & 0x0fffffff;

                    subkey[keyIndex] = ((left << 4) & 0x24000000)
                                        | ((left << 28) & 0x10000000)
                                        | ((left << 14) & 0x08000000)
                                        | ((left << 18) & 0x02080000)
                                        | ((left << 6) & 0x01000000)
                                        | ((left << 9) & 0x00200000)
                                        | ((left >> 1) & 0x00100000)
                                        | ((left << 10) & 0x00040000)
                                        | ((left << 2) & 0x00020000)
                                        | ((left >> 10) & 0x00010000)
                                        | ((right >> 13) & 0x00002000)
                                        | ((right >> 4) & 0x00001000)
                                        | ((right << 6) & 0x00000800)
                                        | ((right >> 1) & 0x00000400)
                                        | ((right >> 14) & 0x00000200)
                                        | (right & 0x00000100)
                                        | ((right >> 5) & 0x00000020)
                                        | ((right >> 10) & 0x00000010)
                                        | ((right >> 3) & 0x00000008)
                                        | ((right >> 18) & 0x00000004)
                                        | ((right >> 26) & 0x00000002)
                                        | ((right >> 24) & 0x00000001);

                    keyIndex++;
                    subkey[keyIndex] = ((left << 15) & 0x20000000)
                                        | ((left << 17) & 0x10000000)
                                        | ((left << 10) & 0x08000000)
                                        | ((left << 22) & 0x04000000)
                                        | ((left >> 2) & 0x02000000)
                                        | ((left << 1) & 0x01000000)
                                        | ((left << 16) & 0x00200000)
                                        | ((left << 11) & 0x00100000)
                                        | ((left << 3) & 0x00080000)
                                        | ((left >> 6) & 0x00040000)
                                        | ((left << 15) & 0x00020000)
                                        | ((left >> 4) & 0x00010000)
                                        | ((right >> 2) & 0x00002000)
                                        | ((right << 8) & 0x00001000)
                                        | ((right >> 14) & 0x00000808)
                                        | ((right >> 9) & 0x00000400)
                                        | ((right) & 0x00000200)
                                        | ((right << 7) & 0x00000100)
                                        | ((right >> 7) & 0x00000020)
                                        | ((right >> 3) & 0x00000011)
                                        | ((right << 2) & 0x00000004)
                                        | ((right >> 21) & 0x00000002);

                    keyIndex++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endregion Initialization

#region Mode-based encrytion/decryption
        /* Electronic Codebook Mode DES encryption/decryption of data according to 'mode'. */
        private int des_ecb_crypt(_des_ctx ctx, byte[] from, byte[] to, bool mode)
        {
            UInt32 left, right, work;
            UInt32[] keys;

            keys = mode ? ctx.decrypt_subkeys : ctx.encrypt_subkeys;
            READ_64BIT_DATA(from, out left, out right);
            INITIAL_PERMUTATION(ref left, out work, ref right);

            int index = 0;
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);
            DES_ROUND(right, ref left, out work, keys, ref index); DES_ROUND(left, ref right, out work, keys, ref index);

            FINAL_PERMUTATION(ref right, out work, ref left);
            WRITE_64BIT_DATA(to, right, left);

            return 0;
        }
#endregion //Mode-based encrytion/decryption
   }

   /**************************************************************************************************
   * Class Name: ContactSupport
   *  
   * Purpose: To provide contact smartcard functionality. 
   *           
   ***************************************************************************************************/
    public class ContactSupport
    {
        #region "Constants"

        //M Card API error & warning codes:
        
        // Memory card specific error codes
        public const uint SCARD_CODE_BASE = 0x80100000;
        public const int COSTOMER_CODE_FLAG = 0x20000000;
        public const int MCARD_OFFSET = 0x800;

        // Offset for error codes		
        public const int MCARD_E_OFFSET = 0x0;

        //Offset for warning codes [SCARD_W_xx uses same]
        public const int MCARD_W_OFFSET = 0x65;

        public const uint MCARD_CODE_BASE = ((SCARD_CODE_BASE | COSTOMER_CODE_FLAG) + MCARD_OFFSET);
        public const uint MCARD_E_CODE_BASE = (MCARD_CODE_BASE + MCARD_E_OFFSET); // errors start here
        public const uint MCARD_W_CODE_BASE = (MCARD_CODE_BASE + MCARD_W_OFFSET); // warning start here

        public const int MCARD_S_SUCCESS = 0x00;

        // 0x90100801: an internal error has occured
        public const uint MCARD_E_INTERNAL_ERROR = MCARD_E_CODE_BASE + 0x1;
        
        // 0x90100802: function not implemented
        public const uint MCARD_E_NOT_IMPLEMENTED = MCARD_E_CODE_BASE + 0x2;

        // 0x90100803: MCardInitialize not called
        public const uint MCARD_E_NOT_INITIALIZED = MCARD_E_CODE_BASE + 0x3;

        // 0x90100804: this DLL does not work with the specified reader
        public const uint MCARD_E_INCOMPATIBLE_READER = MCARD_E_CODE_BASE + 0x4;

        // 0x90100805: could not identify card
        public const uint MCARD_E_UNKNOWN_CARD = MCARD_E_CODE_BASE + 0x05;

        // 0x90100811: the buffer for return daa is too small
        public const uint MCARD_E_BUFFER_TOO_SMALL = MCARD_E_CODE_BASE + 0x11;

        // 0x90100812: one or more parameters are invalid
        public const uint MCARD_E_INVALID_PARAMETER = MCARD_E_CODE_BASE + 0x12;

        // 0x90100821: protocoll error while connecting to card
        public const uint MCARD_E_PROTO_MISMATCH = MCARD_E_CODE_BASE + 0x21;

        // 0x90100822: protocol error during card access
        public const uint MCARD_E_PROTOCOL_ERROR = MCARD_E_CODE_BASE + 0x22;

        // 0x90100827: Challenge Response Failed
        public const uint MCARD_E_CHAL_RESP_FAILED = MCARD_E_CODE_BASE + 0x23;

        // 0x90100826: Invalid memory range
        public const uint MCARD_E_INVALID_MEMORY_RANGE = MCARD_E_CODE_BASE + 0x24;

        // 0x90100831: specified memory zone ID is invalid for current card
        public const uint MCARD_E_INVALID_MEMORY_ZONE_ID = MCARD_E_CODE_BASE + 0x31;

        // 0x90100832: specified PIN ID is invalid for current card
        public const uint MCARD_E_INVALID_PIN_ID = MCARD_E_CODE_BASE + 0x32;

        // 0x90100833: spezcified challenge/response ID is invalid for current card
        public const uint MCARD_E_INVALID_CHAL_RESP_ID = MCARD_E_CODE_BASE + 0x33;

        // MCARD_W_xxx warning codes *************************************************
        // Note: these codes indicate a problem occured; the application must decide how serious it is

        // 0x90100866: could not read all data from card
        public const uint MCARD_W_NOT_ALL_DATA_READ = MCARD_W_CODE_BASE + 0x1;

        // 0x90100867: could not write all data to card
        public const uint MCARD_W_NOT_ALL_DATA_WRITTEN = MCARD_W_CODE_BASE + 0x2;

        // 0x90100876: PIN must be verified before access is possible
        public const uint MCARD_W_PIN_VERIFY_NEEDED = MCARD_W_CODE_BASE + 0x11;

        // 0x90100877: PIN verification failed
        public const uint MCARD_W_PIN_VERIFY_FAILED = MCARD_W_CODE_BASE + 0x12;

        // 0x90100878: no PIN verification attempts left, card probably locked
        public const uint MCARD_W_NO_PIN_ATTEMPTS_LEFT = MCARD_W_CODE_BASE + 0x13;

        //no Units to Decrement in the counter
        public const uint MCARD_W_NO_UNITS_TO_DECREMENT = MCARD_W_CODE_BASE + 0x14;

        // 0xA0100886: The card has been removed
        public const uint MCARD_W_REMOVED_CARD = MCARD_W_CODE_BASE + 0x21;

        public static string GetMCardErrMsg(uint ReturnCode)
        {
            switch (ReturnCode)
            {
                case MCARD_E_INTERNAL_ERROR:
                    return ("Internal error.");
                case MCARD_E_NOT_IMPLEMENTED:
                    return ("Not implemented.");
                case MCARD_E_NOT_INITIALIZED:
                    return ("Not initialized");
                case MCARD_E_INCOMPATIBLE_READER:
                    return ("Incompatible reader.");
                case MCARD_E_UNKNOWN_CARD:
                    return ("Unknown card.");
                case MCARD_E_BUFFER_TOO_SMALL:
                    return ("Buffer is too small.");
                case MCARD_E_INVALID_PARAMETER:
                    return ("Invalid parameter.");
                case MCARD_E_PROTOCOL_ERROR:
                    return ("Protocol Error.");
                case MCARD_E_CHAL_RESP_FAILED:
                    return ("Challenge response failed.");
                case MCARD_E_INVALID_MEMORY_RANGE:
                    return ("Invalid memory range.");
                case MCARD_E_INVALID_MEMORY_ZONE_ID:
                    return ("Invalid memory zone ID.");
                case MCARD_E_INVALID_PIN_ID:
                    return ("Invalide PIN ID.");
                case MCARD_E_INVALID_CHAL_RESP_ID:
                    return ("Invalid challenge response ID.");
                case MCARD_W_NOT_ALL_DATA_READ:
                    return ("Could not read all the data.");
                case MCARD_W_NOT_ALL_DATA_WRITTEN:
                    return ("Could not write all the data.");
                case MCARD_W_PIN_VERIFY_NEEDED:
                    return ("PIN verification required.");
                case MCARD_E_PROTO_MISMATCH:
                    return ("The requested protocols are incompatible with the protocol currently in use with the card.");
                case MCARD_W_PIN_VERIFY_FAILED:
                    return ("PIN verification failed.");
                case MCARD_W_NO_PIN_ATTEMPTS_LEFT:
                    return ("No PIN verification attempts remain.");
                case MCARD_W_NO_UNITS_TO_DECREMENT:
                    return ("No units to decrement.");
                case MCARD_W_REMOVED_CARD:
                    return ("The smart card has been removed.");

                default:
                    return ("?");
            }
        }
        #endregion

        #region Variables

        private byte _smartCardType = (byte)SMART_CARD_TYPE.UNKNOWN;
        private int _cardContext = 0;
        private int _cardHandle = 0;
        private int _context = 0;

        #endregion //Variables

        #region Enumerations

        private enum CONNECTION_MODE
        {
            FORCED = 0,
            INTELLIGENT = 1
        }

        #endregion //Enumerations

        #region MCSCM DllImports - MCard API

        [DllImport("MCSCM.dll")]
        extern static int MCardConnect(int cardContext, int connectMode, byte cardType,
                                       out int cardHandle);

        [DllImport("MCSCM.dll")]
        extern static int MCardDisconnect(int cardHandle, int disposition);

        [DllImport("MCSCM.dll")]
        extern static int MCardGetAttrib(int cardHandle, int attribType, byte[] attrib,
                                         out int attribSize);

        [DllImport("MCSCM.dll")]
        extern static int MCardInitialize(int context, string readerName, out int cardContext,
                                          out int version);

        [DllImport("MCSCM.dll")]
        extern static int MCardReadMemory(int cardHandle, byte memZone, int offset, byte[] readBuffer,
                                          ref int readLen);

        [DllImport("MCSCM.dll")]
        extern static int MCardShutdown(int cardContext);

        [DllImport("MCSCM.dll")]
        extern static int MCardVerifyPIN(int cardHandle, byte pinNumb, byte[] pinBuf, byte pinLen);

        [DllImport("MCSCM.dll")]
        extern static int MCardWriteMemory(int cardHandle, byte memZone, int offset, byte[] writeBuffer,
                                           ref int writeLen);

        #endregion //MCSCM DllImports - MCard API

        #region Constructor

        public ContactSupport()
        {
        }
        #endregion //Constructor

        #region Tests

        public int ContactTest(ref string smartCardType, string readerName, byte[] key, bool fullTest,
                               out string errMsg)
        {
            errMsg = string.Empty;
            int ret = 0;
            byte[] dataIn = null;
            byte[] dataOut = null;
            try
            {
                byte baseData = 0x00;
                byte data = 0x00;

                dataIn = new byte[20];
                dataOut = new byte[20];

                bool passed = false;
                int addrEnd = 0;
                int connectionMode = (int)CONNECTION_MODE.INTELLIGENT;
                int dataInLen = 0;
                int dataOutLen = 0;
                int version = 0;

                ret = EstablishContext();
                if (ret != 0)
                {
                    errMsg = "SCM: Establish Context Failed";
                    return ret;
                }

                ret = Initialize(readerName, out version);
                if (ret != 0)
                {
                    errMsg = "SCM: Contact Smart Card Initialization Error: " + ret.ToString();
                    return ret;
                }

                DateTime timeStart = DateTime.Now;
                DateTime timeNow;

                while (true)
                {
                    ret = Connect(_smartCardType, connectionMode);
                    if (ret == 0)
                        break;

                    timeNow = DateTime.Now;
                    TimeSpan ts = timeNow - timeStart;
                    if (ts.Seconds > 3)
                        break;

                    Thread.Sleep(500);
                }

                if (ret != 0)
                {
                    errMsg = "SCM: Contact Smart Card Connection Error: " + ret.ToString();
                    return ret;
                }

                _smartCardType = (byte)SMART_CARD_TYPE.UNKNOWN;

                ret = GetCardType();
                if (ret != 0)
                {
                    errMsg = "Contact Smart Card Get Card Type Error: " + ret.ToString();
                    return ret;
                }

                switch (_smartCardType)
                {
                    case (byte)SMART_CARD_TYPE.SLE4428:
                        smartCardType = "SLExx28";

                        ret = Authenticate(0, key, 2);
                        if (ret != 0)
                        {
                            errMsg = "SCM: Contact Smart Card SLExx28: Verify Pin Error: " + ret.ToString();
                            break;
                        }

                        baseData = data = 0x41;

                        if (fullTest)
                            addrEnd = 1007;
                        else
                            addrEnd = 31;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            if (addr == 1007)
                                dataInLen = 13;
                            else
                                dataInLen = 16;

                            for (int i = 0; i < dataInLen; i++)
                                dataIn[i] = data++;

                            ret = MemoryWrite(0, addr, dataIn, ref dataInLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx28: Write Memory Address [" +
                                         addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }
                            data = ++baseData;
                        }

                        baseData = data = 0x41;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            if (addr == 1007)
                                dataOutLen = 13;
                            else
                                dataOutLen = 16;

                            for (int i = 0; i < dataOutLen; i++)
                                dataIn[i] = data++;

                            ret = MemoryRead(0, addr, dataOut, ref dataOutLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Read Memory Address [" +
                                        addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }

                            if (addr == 1007 && dataOutLen != 13)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Incorrect data out length" +
                                        " for Address [" + addr.ToString() + "] Length is " + dataOutLen.ToString();
                                return ret;
                            }
                            else if (addr != 1007 && dataOutLen != 16)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Incorrect data out length" +
                                        " for Address [" + addr.ToString() + "] Length is " + dataOutLen.ToString();
                                return ret;
                            }

                            passed = true;
                            for (int i = 0; i < dataOutLen; i++)
                            {
                                if (dataIn[i] != dataOut[i])
                                {
                                    passed = false;
                                    break;
                                }
                            }
                            if (!passed)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Address [" + addr.ToString() +
                                        " ] Written vs. Read Failed";
                                return ret;
                            }
                            data = ++baseData;
                        }
                        break;

                    case (byte)SMART_CARD_TYPE.SLE4442:

                        smartCardType = "SLExx42";

                        ret = Authenticate(0, key, 3);
                        if (ret != 0)
                        {
                            errMsg = "SCM: Contact Smart Card SLExx42: Verify Pin Error: " + ret.ToString();
                            break;
                        }

                        baseData = data = 0x41;

                        if (fullTest)
                            addrEnd = 239;
                        else
                            addrEnd = 31;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            for (int i = 0; i < 16; i++)
                                dataIn[i] = data++;

                            dataInLen = 16;

                            ret = MemoryWrite(0, addr, dataIn, ref dataInLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Write Memory Address [" +
                                        addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }
                            data = ++baseData;
                        }

                        baseData = data = 0x41;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            for (int i = 0; i < 16; i++)
                                dataIn[i] = data++;

                            dataOutLen = 16;

                            ret = MemoryRead(0, addr, dataOut, ref dataOutLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Read Memory Address [" +
                                        addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }

                            passed = true;
                            for (int i = 0; i < dataIn.Length; i++)
                            {
                                if (dataIn[i] != dataOut[i])
                                {
                                    passed = false;
                                    break;
                                }
                            }
                            if (!passed)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Address [" + addr.ToString() +
                                        " ] Written vs. Read Failed";
                                return ret;
                            }
                            data = ++baseData;
                        }
                        break;

                    default:
                        errMsg = "SCM: Contact Smart Card: Unknown Card";
                        passed = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                errMsg = "ContactTest threw exception: " + ex.Message;
            }
            finally
            {
                dataIn = null;
                dataOut = null;

                Disconnect();
                Shutdown();
                ReleaseContext();
            }
            return ret;
        }

        public int ContactTest(ref string smartCardType, string readerName, byte[] key, bool fullTest,
                               string dataToEncode, out string errMsg)
        {
            errMsg = "";
            byte[] dataIn = null;
            byte[] dataOut = null;
            int ret = 0;
            try
            {
                dataIn = new byte[20];
                dataOut = new byte[20];

                byte data = Convert.ToByte(dataToEncode);

                bool passed = false;
                int addrEnd = 0;
                int connectionMode = (int)CONNECTION_MODE.INTELLIGENT;
                int dataInLen = 0;
                int dataOutLen = 0;
                int version = 0;

                ret = EstablishContext();
                if (ret != 0)
                {
                    errMsg = "SCM: Establish Context Failed";
                    return ret;
                }

                ret = Initialize(readerName, out version);
                if (ret != 0)
                {
                    errMsg = "SCM: Contact Smart Card Initialization Error: " + ret.ToString();
                    return ret;
                }

                DateTime timeStart = DateTime.Now;
                DateTime timeNow;

                while (true)
                {
                    ret = Connect(_smartCardType, connectionMode);
                    if (ret == 0)
                        break;

                    timeNow = DateTime.Now;
                    TimeSpan ts = timeNow - timeStart;
                    if (ts.Seconds > 3)
                        break;

                    Thread.Sleep(500);
                }

                if (ret != 0)
                {
                    errMsg = "SCM: Contact Smart Card Connection Error: " + ret.ToString();
                    return ret;
                }

                _smartCardType = (byte)SMART_CARD_TYPE.UNKNOWN;

                ret = GetCardType();
                if (ret != 0)
                {
                    errMsg = "Contact Smart Card Get Card Type Error: " + ret.ToString();
                    return ret;
                }

                switch (_smartCardType)
                {
                    case (byte)SMART_CARD_TYPE.SLE4428:

                        smartCardType = "SLExx28";

                        ret = Authenticate(0, key, 2);
                        if (ret != 0)
                        {
                            errMsg = "SCM: Contact Smart Card SLExx28: Verify Pin Error: " + ret.ToString();
                            break;
                        }

                        if (fullTest)
                            addrEnd = 1007;
                        else
                            addrEnd = 31;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            if (addr == 1007)
                                dataInLen = 13;
                            else
                                dataInLen = 16;

                            for (int i = 0; i < dataInLen; i++)
                                dataIn[i] = data;

                            ret = MemoryWrite(0, addr, dataIn, ref dataInLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx28: Write Memory Address [" +
                                            addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }
                        }

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            if (addr == 1007)
                                dataOutLen = 13;
                            else
                                dataOutLen = 16;

                            for (int i = 0; i < dataOutLen; i++)
                                dataIn[i] = data;

                            ret = MemoryRead(0, addr, dataOut, ref dataOutLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Read Memory Address [" +
                                            addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }

                            if (addr == 1007 && dataOutLen != 13)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Incorrect data out length" +
                                        " for Address [" + addr.ToString() + "] Length is " + dataOutLen.ToString();
                                return ret;
                            }
                            else if (addr != 1007 && dataOutLen != 16)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Incorrect data out length" +
                                        " for Address [" + addr.ToString() + "] Length is " + dataOutLen.ToString();
                                return ret;
                            }

                            passed = true;
                            for (int i = 0; i < dataOutLen; i++)
                            {
                                if (dataIn[i] != dataOut[i])
                                {
                                    passed = false;
                                    break;
                                }
                            }
                            if (!passed)
                            {
                                errMsg = "SCM: Contact Smart Card SLE44xx: Address [" + addr.ToString() +
                                        " ] Written vs. Read Failed";
                                return ret;
                            }

                        }
                        break;

                    case (byte)SMART_CARD_TYPE.SLE4442:

                        smartCardType = "SLExx42";

                        ret = Authenticate(0, key, 3);
                        if (ret != 0)
                        {
                            errMsg = "SCM: Contact Smart Card SLExx42: Verify Pin Error: " + ret.ToString();
                            return ret;
                        }

                        if (fullTest)
                            addrEnd = 239;
                        else
                            addrEnd = 31;

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            for (int i = 0; i < 16; i++)
                                dataIn[i] = data;

                            dataInLen = 16;

                            ret = MemoryWrite(0, addr, dataIn, ref dataInLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Write Memory Address [" +
                                        addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }

                        }

                        for (int addr = 31; addr <= addrEnd; addr += 16)
                        {
                            for (int i = 0; i < 16; i++)
                                dataIn[i] = data;

                            dataOutLen = 16;

                            ret = MemoryRead(0, addr, dataOut, ref dataOutLen);
                            if (ret != 0)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Read Memory Address [" +
                                        addr.ToString() + "] Error: " + ret.ToString();
                                return ret;
                            }

                            passed = true;
                            for (int i = 0; i < dataIn.Length; i++)
                            {
                                if (dataIn[i] != dataOut[i])
                                {
                                    passed = false;
                                    break;
                                }
                            }
                            if (!passed)
                            {
                                errMsg = "SCM: Contact Smart Card SLExx42: Address [" + addr.ToString() +
                                            " ] Written vs. Read Failed";
                                return ret;
                            }
                        }
                        break;

                    default:
                        errMsg = "SCM: Contact Smart Card: Unknown Card";
                        passed = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                dataIn = null;
                dataOut = null;
                Disconnect();
                Shutdown();
                ReleaseContext();
            }
            return ret;
        }
        #endregion //Tests

        #region Context
        
        private int EstablishContext()
        {
            return WinSCard.SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, 0, 0, ref _context);
        }

        private int ReleaseContext()
        {
            return WinSCard.SCardReleaseContext(_context);
        }
        #endregion //Context

        #region Connect/Disconnect

        public int Initialize(string readerName, out int version)
        {
            int ret = 0;
            version = 0;

            for (int i = 0; i < 5; i++)
            {
                ret = MCardInitialize(_context, readerName, out _cardContext, out version);
                if (ret == 0) break;
            }
            return ret;
        }

        public int Shutdown()
        {
            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardShutdown(_cardContext);
                if (ret == 0) break;
            }
            return ret;
        }

        // Connects with a Memory Card ----------------------------------------------------------------------
        public int Connect(byte cardType, int connectionMode)
        {
            int ret = 0;
            for (int i = 0; i < 3; i++)
            {
                ret = MCardConnect(_cardContext, connectionMode, cardType, out _cardHandle);
                if (ret == 0) break;
            }
            return ret;
        }

        // Disconnects from a Memory Card -------------------------------------------------------------------

        public int Disconnect()
        {
            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardDisconnect(_cardHandle, 0);
                if (ret == 0) break;
            }
            return ret;
        }
        #endregion //Connect/Disconnect

        #region Card Methods

        // Authentication -----------------------------------------------------------------------------------
        public int Authenticate(byte keyNumber, byte[] key, byte keyLength)
        {
            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardVerifyPIN(_cardHandle, keyNumber, key, keyLength);
                if (ret == 0) break;
            }
            return ret;
        }

        // Gets Memroy Card Type ----------------------------------------------------------------------------
        public int GetCardType()
        {
            byte[] attrib = new byte[128];
            int attribSize = 0;

            _smartCardType = (byte)SMART_CARD_TYPE.UNKNOWN;

            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardGetAttrib(_cardHandle, (int)ATTRIB_TYPE.TYPE, attrib, out attribSize);
                if (ret == 0) break;
            }
            if (ret != 0) return ret;

            if (attribSize != 1) return 1;

            _smartCardType = attrib[0];
            return 0;
        }

        // Memory Card - Read Memory ------------------------------------------------------------------------
        public int MemoryRead(byte memZone, int offset, byte[] readBuffer, ref int readLen)
        {
            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardReadMemory(_cardHandle, memZone, offset, readBuffer, ref readLen);
                if (ret == 0) break;
            }
            return ret;
        }

        // Memory Card - Write Memory -----------------------------------------------------------------------
        public int MemoryWrite(byte memZone, int offset, byte[] writeBuffer, ref int writeLen)
        {
            int ret = 0;
            for (int i = 0; i < 5; i++)
            {
                ret = MCardWriteMemory(_cardHandle, memZone, offset, writeBuffer, ref writeLen);
                if (ret == 0) break;
            }
            return ret;
        }
        #endregion //Card Methods
    }
}
