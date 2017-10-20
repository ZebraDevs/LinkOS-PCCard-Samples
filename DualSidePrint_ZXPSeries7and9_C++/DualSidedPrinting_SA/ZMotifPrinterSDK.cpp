/***************************************************************************************************************
*	ZMotifPrinterSDK.cpp : Defines ZMotif Printer SDK entry points.
*
*	This modules provides functions which use ZMotif Printer SDK features.	
*
*	Copyright 2010 Zebra Technologies LLC, All rights reserved.
****************************************************************************************************************/

#include "stdafx.h"

#include "ZMotifPrinterSDK.h"

#include "Utilities.h"

//ZMotifPrinter's Job Interface pointer:
IJobPtr pJob;

IJobPtr GetZMotifPrinterInterface()
{
	try
	{
		HRESULT hr = CoCreateInstance(CLSID_Job, NULL, CLSCTX_INPROC_SERVER, IID_IJob, (void**)&pJob);
		
		if (SUCCEEDED(hr))
			return pJob;
	}
	catch(_com_error ex)
	{
		printf("\nGetZMotifPrinterInterface failed: &s", BSTRtoChar(ex.Description()));
	}
	return NULL;
}

bool ReleaseZMotifPrinterInterface()
{
	bool result = false;
	try
	{
		pJob.Release(); //release via smart pointer

		result = true;

	}
	catch(_com_error ex)
	{
		result = false;
		printf("\nReleaseZMotifPrinterInterface failed: &s", BSTRtoChar(ex.Description()));
	}
	return result;
}

bool IsPrinterReady(IJobPtr pJob)
{
	long err, jobsPending, jobsActive, jobsComplete, jobErrors, jobsTotal, nextActionID;

	bool printerReady = false;

	try
    {
		BSTR status;
		
		short alarm = pJob->Device->GetPrinterStatus(&status, &err, &jobsPending, &jobsActive, 
		                                             &jobsComplete, &jobErrors, &jobsTotal, &nextActionID);
        
        if (alarm == 0)
		{
			if( (wcsstr(status, L"idle") != NULL) ||
				(wcsstr(status, L"standby") != NULL) )
				printerReady = true;
		}
	}
    catch (char* e)
    {
		printf("Is Printer Ready Exception: %s", e);
	}

	return printerReady;
}

void JobWait(IJobPtr pJob, int actionID, int loops, wchar_t* status, short* alarm)
{
	*alarm = 0;

	long copiesCompleted = 0,
        copiesRequested = 0,
        errorCode = 0;
    
	BSTR contactStatus[25];
	BSTR contactlessStatus[25];
    BSTR magStatus[25];
    BSTR printingStatus[25];
    BSTR cardPosition[25];
	BSTR uuidJob[25];

    bool jobStatusGood = false;

    while (loops > 0)
	{
		try
        {
			Sleep(250);
			
			*alarm = pJob->GetJobStatus(actionID, uuidJob, printingStatus, cardPosition, &errorCode,
                                        &copiesCompleted, &copiesRequested, magStatus,
                                        contactStatus, contactlessStatus);
        }
        catch (_com_error ex)
        {
			wcscpy(status, L"Job Wait Exception: ");
			wcscat(status, ex.Description());
     	    break;
        }

		_bstr_t tmp(*printingStatus, FALSE);   
		CString printMsg(static_cast<const char*>(tmp)); 

		_bstr_t tmp1(*cardPosition, FALSE);  
		CString cardPos(static_cast<const char*>(tmp1)); 

		if(printMsg == "done_ok")
        {
			jobStatusGood = true;
            break;
        }

        if(printMsg == "error")
        {
			wcscpy(status, L"Printing Status Error");
			break;
        }

        if(cardPos == "ejecting_eject")
		{
			wcscpy(status, L"card exiting to eject bin");
            break;
        }

        if(cardPos == "ejecting_reject")
		{
			wcscpy(status, L"card exiting to reject bin");
            break;
        }

        if(cardPos == "ejecting_feeder")
		{
			wcscpy(status, L"card exiting to feeder");
            break;
        }

		loops--;
        //Sleep(1000);
    }

    if (!jobStatusGood && (wcsstr(status, L"") != NULL))
		wcscpy(status, L"Job Status Timedout");
}

LONG GetCardCount(IJobPtr pJob)
{
	try
	{
		LONG cardCount = 0;
        SHORT alarm = pJob->Device->GetTotalCardCount(&cardCount);

		return cardCount;
	}
	catch (_com_error ex)
	{
		OutputDebugString(ex.Description());
	}

	return 0L;
}

short CheckAlarm(IJobPtr pJob)
{
	int alarm = 0;
	char* errMsg = "";
    BSTR junk;
    
	short result;
    try
    {
		alarm = pJob->Device->GetDeviceInfo(&junk, &junk, &junk, &junk, &junk, &junk,
											&junk, &junk, &junk, &junk); 
	    return alarm;
    }
    catch (char* ex)
    {
		MessageBox(NULL, ex, "CheckAlarm threw exception. Defaulting to no alarm condition", MB_OK);
		result = 0;
    }
    return result;
}