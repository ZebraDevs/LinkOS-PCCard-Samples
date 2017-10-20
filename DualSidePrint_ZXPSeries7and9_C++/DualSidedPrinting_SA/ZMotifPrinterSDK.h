#ifndef _ZMOTIF_PRINTER_SDK
#define _ZMOTIF_PRINTER_SDK

#import "ZMotifPrinter.dll" no_namespace named_guids

const int CARD_NOT_DETECTED = 4014;
const int OUT_OF_CARDS = 4016;
const int EP_SCRIPT_ERROR = 7014;
const int ACTION_ID_NOT_FOUND = 13003;

IJobPtr GetZMotifPrinterInterface();
bool ReleaseZMotifPrinterInterface();

void AtStation(IJobPtr pJob, int actionID, int loops, char* status, short* alarm);
LONG GetCardCount(IJobPtr pJob);

bool IsPrinterReady(IJobPtr pJob);
short GetPrinterStatus(IJobPtr pJob, unsigned short* status);

void JobWait(IJobPtr pJob, int actionID, int loops, wchar_t* status, short* alarm);

void DetermineMagneticTestType(IJobPtr pJob, char* testType, char* cardType, int* actionID,
                               char* errMsg, short* alarm);

short CheckAlarm(IJobPtr pJob);

#endif