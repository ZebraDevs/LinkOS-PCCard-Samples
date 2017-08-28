using System;
using System.Collections.Generic;
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
File: ZBRUtil.cs
Description: A utility class to provide descriptive error messages
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/

namespace CSharpCardIDPrintApplicationSample
{
   public class ZBRUtil
    {
        public enum Orientation { POTRAIT = 0, LANDSCAPE = 1 };


        public enum Rotation { NONE = 0, DEG180 = 1 };

       public enum ZEBRA_ERROR_CODES
       {
           // Error Codes
           ZBR_ERROR_NO_ERROR = 0,		// No errors
           ZBR_640_SDK_ERROR_BASE = 4000,	// Error code base

           // Zebra Common SDK Errors
           ZBR_ERROR_PRINTER_NOT_SUPPORTED = 60,		// Printer not supported by SDK
           ZBR_ERROR_CANNOT_GET_PRINTER_HANDLE = 61,		// Unable to open handle to printer
           // (check printer name)
           ZBR_ERROR_CANNOT_GET_PRINTER_DRIVER = 62,		// Unable to open print driver
           // (check driver installation)
           ZBR_ERROR_GETPRINTERDATA_ERROR = 63,		// Error communicating with print driver
           ZBR_ERROR_INVALID_PRINTER_HANDLE = 65,		// Invalid or NULL print driver handle
           ZBR_ERROR_CLOSEPRINTER_FAILURE = 66,		// Failed to close handle to print driver
           ZBR_SDK_ERROR_COMMUNICATION_ERROR = 67,     // Command failed due to communication error
           ZBR_SDK_ERROR_BUFFER_OVERFLOW = 68,     // Response too large for buffer
           ZBR_SDK_ERROR_READ_DATA_ERROR = 69,     // Error reading data
           ZBR_SDK_ERROR_WRITE_DATA_ERROR = 70,     // Error writing data
           ZBR_SDK_ERROR_LOAD_LIBRARY_ERROR = 71,     // Error loading SDK
           ZBR_SDK_ERROR_INVALID_STRUCT_ALIGNMENT = 72,    // Invalid structure alignment
           ZBR_SDK_ERROR_GETTING_DEVICE_CONTEXT = 73,    // Unable to create the device context for the driver
           ZBR_SDK_ERROR_SPOOLER_ERROR = 74,    // Print spooler error
           ZBR_SDK_ERROR_OUT_OF_MEMORY = 75,    // Operating system is out of memory
           ZBR_SDK_ERROR_OUT_OF_DISK_SPACE = 76,    // Operating system is out of disk space
           ZBR_SDK_ERROR_USER_ABORT = 77,    // Print job aborted by the user
           ZBR_SDK_ERROR_APPLICATION_ABORT = 78,    // Application aborted
           ZBR_SDK_ERROR_CREATE_FILE_ERROR = 79,    // Error creating file
           ZBR_SDK_ERROR_WRITE_FILE_ERROR = 80,    // Error writing file
           ZBR_SDK_ERROR_READ_FILE_ERROR = 81,    // Error reading file
           ZBR_SDK_ERROR_INVALID_MEDIA = 82,    // Invalid media
           ZBR_ERROR_BUFFER_TOO_SMALL = 4011,	// Insufficient allocated memory	
           ZBR_ERROR_PRINT_IMAGE = 4018,	// Error while printing image
           ZBR_ERROR_PARAMETERS_ERROR = 4019,	// Inavlid parameters supplied to SDK
           ZBR_ERROR_UNKNOWN_ERROR = 65535,	// Unknown error has occured

           // Printing Errors
           ZBR_ERROR_GETTING_DEVICE_CONTEXT = 4020,	// Unable to create the device context for the driver
           ZBR_ERROR_SPOOLER_ERROR = 4021,	// Print spooler error
           ZBR_ERROR_OUT_OF_MEMORY = 4022,	// System out of memory
           ZBR_ERROR_OUT_OF_DISK_SPACE = 4023,	// Out of disk space
           ZBR_ERROR_USER_ABORT = 4024,	// User aborted job
           ZBR_ERROR_APPLICATION_ABORT = 4025,	// Application aborted job
           ZBR_ERROR_CREATE_FILE_ERROR = 4026, // Create file error
           ZBR_ERROR_WRITE_FILE_ERROR = 4027,	// Write to file error
           ZBR_ERROR_READ_FILE_ERROR = 4028,	// Read from file error
           // Printer Error Codes
           ZBR_ERROR_INVALID_FILE_EXT = 4062,	// Invalid file extension
           ZBR_ERROR_DIAGNOSTIC_FAILURE = 4063,	// Diagnostic failure
           ZBR_ERROR_RECEPTOR_OUT = 4064,	// Receptor out
           ZBR_ERROR_MEDIA_JAM = 4065,	// Media jam
           ZBR_ERROR_RIBBON_OUT = 4066,	// Ribbon out
           ZBR_ERROR_FRONT_PANEL_OPEN = 4067,	// Front Panel open
           ZBR_ERROR_PRINTHEAD_JAM = 4068,	// Printhead jam
           ZBR_ERROR_IMAGE_CONVERT = 4069,	// Image conversion error
           ZBR_ERROR_CLEAN_TAPE_OUT = 4070,	// Cleaning Tape out
           ZBR_ERROR_LOST_HOME = 4071,	// Lost home position
           ZBR_ERROR_FLIP_JAM = 4072,	// Flip Station jam
           ZBR_ERROR_CARD_LOST_FLIP = 4073,	// Card lost after flip
           ZBR_ERROR_MAG_WRITE = 4076,	// Mag. Encoder write error
           ZBR_ERROR_WAIT = 4085,	// Waiting for command to continue
           ZBR_ERROR_PRINTER_BUSY = 4088,	// Printer is busy
           ZBR_ERROR_ILLEGAL_REQUEST = 4089,	// Illegal request
           ZBR_ERROR_TEMP_SHUTDOWN = 4090,	// Printhead temperature too high, 
           // printing will resume when printhead cools.
           ZBR_ERROR_DATA_COMM_TIMEOUT = 4092,	// Printer timed out when receiving or sending data
           ZBR_ERROR_LAM_OUT = 4093,	// Laminate out
           ZBR_ERROR_HEATER_ERROR = 4104,	// Heater error
           ZBR_ERROR_WRITE_DATA_ERROR = 4105,	// Error sending data to printer
           ZBR_ERROR_READ_DATA_ERROR = 4106,	// Error reading data from printer
           ZBR_ERROR_BOTTOM_NO_GAP = 4107,	// No gap detected in bottom laminate
           ZBR_ERROR_TOP_NO_GAP = 4108,	// No gap detected in top laminate
           ZBR_ERROR_TEMP_RANGE = 4109,	// CJ temp out of range
           ZBR_ERROR_CARD_NOT_SEATED = 4110,	// Card not properly seated in truck
           ZBR_ERROR_CARD_CARRIER_STALL = 4111,	// Lost steps driving to print position
           ZBR_ERROR_MAG_FLUX_READBACK = 4112,	// No flux transitions detected during readback
           ZBR_ERROR_MAG_FLUX_VERIFY = 4113,	// No flux transitions detected during verify
           ZBR_ERROR_EIN_DATA_ERROR = 4114,	// Readback of EIN data failed
           ZBR_ERROR_CARD_NOT_INSERTED = 4115,	// Card failed to insert into laminator
           ZBR_ERROR_TOP_LAM_OUT = 4116,	// Top laminate out
           ZBR_ERROR_BOTTOM_LAM_OUT = 4117,	// Bottom laminate out
           ZBR_ERROR_LAM_CARD_JAM = 4118,	// Card jammed in laminator
           ZBR_ERROR_LAM_CARD_EXIT = 4119,	// Card didn't exit laminator
           ZBR_ERROR_CARD_DOOR_OPEN = 4120,	// Card hopper door open
           ZBR_ERROR_READING_EIN = 4122,	// Reading EIN
           ZBR_ERROR_MISSING_DONGLE = 4137,	// Dongle is missing or has wrong serial number
           ZBR_ERROR_NO_RFID_RESPONSE = 4138,	// No response from RFID Board
           ZBR_ERROR_NO_RFID_RIBBON = 4139,	// No RFID Ribbon present
           ZBR_ERROR_DEC_PANEL_COUNT_ERROR = 4140	// Error decrementing panel counter
       };

        //utility method for translating error codes:
        public static string TranslateErrorCode(int err)
        {
            ZEBRA_ERROR_CODES error = (ZEBRA_ERROR_CODES)err;
            string msg = "";
            try
            {
                switch (error)
                {
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_NO_ERROR: msg = "No Error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_640_SDK_ERROR_BASE: msg = "Base Error Code"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_PRINTER_NOT_SUPPORTED: msg = "Printer not supported by SDK"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CANNOT_GET_PRINTER_HANDLE: msg = "Unable to open handle to printer"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CANNOT_GET_PRINTER_DRIVER: msg = "Unable to open print driver"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_GETPRINTERDATA_ERROR: msg = "Error communicating with print driver"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_INVALID_PRINTER_HANDLE: msg = "Invalid or NULL print driver handle"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CLOSEPRINTER_FAILURE: msg = "Failed to close handle to print driver"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_COMMUNICATION_ERROR: msg = "Command failed due to communication error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_BUFFER_OVERFLOW: msg = "Response too large for buffer"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_READ_DATA_ERROR: msg = "Error reading data"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_WRITE_DATA_ERROR: msg = "Error writing data"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_LOAD_LIBRARY_ERROR: msg = "Error loading SDK"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_INVALID_STRUCT_ALIGNMENT: msg = "Invalid structure alignment"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_GETTING_DEVICE_CONTEXT: msg = "Unable to create the device context for the driver"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_SPOOLER_ERROR: msg = "Print spooler error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_OUT_OF_MEMORY: msg = "Operating system is out of memory"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_OUT_OF_DISK_SPACE: msg = "Operating system is out of disk space"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_USER_ABORT: msg = "Print job aborted by the user"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_APPLICATION_ABORT: msg = "Application aborted"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_CREATE_FILE_ERROR: msg = "Error creating file"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_WRITE_FILE_ERROR: msg = "Error writing file"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_READ_FILE_ERROR: msg = "Error reading file"; break;
                    case ZEBRA_ERROR_CODES.ZBR_SDK_ERROR_INVALID_MEDIA: msg = "Invalid media"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_BUFFER_TOO_SMALL: msg = "Insufficient allocated memory"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_PRINT_IMAGE: msg = "Error while printing image"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_PARAMETERS_ERROR: msg = "Inavlid parameters supplied to SDK"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_UNKNOWN_ERROR: msg = "Unknown error has occured"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_GETTING_DEVICE_CONTEXT: msg = "Unable to create the device context for the driver"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_SPOOLER_ERROR: msg = "Print spooler error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_OUT_OF_MEMORY: msg = "System out of memory"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_OUT_OF_DISK_SPACE: msg = "Out of disk space"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_USER_ABORT: msg = "User aborted job"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_APPLICATION_ABORT: msg = "Application aborted job"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CREATE_FILE_ERROR: msg = "Create file error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_WRITE_FILE_ERROR: msg = "Write to file error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_READ_FILE_ERROR: msg = "Read from file error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_INVALID_FILE_EXT: msg = "Invalid file extension"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_DIAGNOSTIC_FAILURE: msg = "Diagnostic failure"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_RECEPTOR_OUT: msg = "Receptor out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_MEDIA_JAM: msg = "Media jam"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_RIBBON_OUT: msg = "Ribbon out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_FRONT_PANEL_OPEN: msg = "Front Panel open"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_PRINTHEAD_JAM: msg = "Printhead jam"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_IMAGE_CONVERT: msg = "Image conversion error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CLEAN_TAPE_OUT: msg = "Cleaning Tape out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_LOST_HOME: msg = "Lost home position"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_FLIP_JAM: msg = "Flip Station jam"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CARD_LOST_FLIP: msg = "Card lost after flip"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_MAG_WRITE: msg = "Mag. Encoder write error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_WAIT: msg = "Waiting for command to continue"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_PRINTER_BUSY: msg = "Printer is busy"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_ILLEGAL_REQUEST: msg = "Illegal request"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_TEMP_SHUTDOWN: msg = "Printhead temperature too high; printing will resume when printhead cools."; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_DATA_COMM_TIMEOUT: msg = "Printer timed out when receiving or sending data"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_LAM_OUT: msg = "Laminate out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_HEATER_ERROR: msg = "Heater error"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_WRITE_DATA_ERROR: msg = "Error sending data to printer"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_READ_DATA_ERROR: msg = "Error reading data from printer"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_BOTTOM_NO_GAP: msg = "No gap detected in bottom laminate"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_TOP_NO_GAP: msg = "No gap detected in top laminate"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_TEMP_RANGE: msg = "CJ temp out of range"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CARD_NOT_SEATED: msg = "Card not properly seated in truck"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CARD_CARRIER_STALL: msg = "Lost steps driving to print position"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_MAG_FLUX_READBACK: msg = "No flux transitions detected during readback"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_MAG_FLUX_VERIFY: msg = "No flux transitions detected during verify"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_EIN_DATA_ERROR: msg = "Readback of EIN data failed"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CARD_NOT_INSERTED: msg = "Card failed to insert into laminator"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_TOP_LAM_OUT: msg = "Top laminate out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_BOTTOM_LAM_OUT: msg = "Bottom laminate out"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_LAM_CARD_JAM: msg = "Card jammed in laminator"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_LAM_CARD_EXIT: msg = "Card didn't exit laminator"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_CARD_DOOR_OPEN: msg = "Card hopper door open"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_READING_EIN: msg = "Reading EIN"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_MISSING_DONGLE: msg = "Dongle is missing or has wrong serial number"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_NO_RFID_RESPONSE: msg = "No response from RFID Board"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_NO_RFID_RIBBON: msg = "No RFID Ribbon present"; break;
                    case ZEBRA_ERROR_CODES.ZBR_ERROR_DEC_PANEL_COUNT_ERROR: msg = "Error decrementing panel counter"; break;

                    default: msg = "Uknown Error Code"; break;
                }
            }
            catch (Exception ex)
            {
                msg = ex.StackTrace + ex.Message;
            }
            return msg;
        }

    }
}
