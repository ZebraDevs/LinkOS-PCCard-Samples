/***************************************************************************************************************
*	OspreyTestApp.cpp : Defines the entry point for the console application.
*
*	This program demonstrates how to use the ZMotif Printer & Graphics SDKs 
*   to perform dual sided printing with the ZXP 8 series printer.	
*
*	Copyright 2010 Zebra Technologies LLC, All rights reserved.
****************************************************************************************************************/


#include "stdafx.h"

#include "ZMotifPrinterSDK.h" //ZMotifPrinter SDK import, function prototypes, etc.

#include "ZMotifGraphicsSDK.h" //ZMotifGraphics SDK import, function prototypes, etc.

#include "Utilities.h" //generic functions


int main(int argc, char* argv[])
{
	
	char errMsg[25] = "";
	
	CoInitialize(NULL);

	IJobPtr pJob = GetZMotifPrinterInterface();

	if (!GetZMotifGraphicsInterface())
	{
		char c;
		printf("Failed to initialize ZMotifGraphics SDK. Press any key to continue\n");
		scanf(&c);

		CoUninitialize();
		return 0;
	}
	
	if (pJob == NULL)
	{
		char c;
		printf("Failed to initialize ZMotifPrinter SDK. Press any key to continue\n");
		scanf(&c);

		CoUninitialize();
		return 0;
	}
	
	_variant_t list;

	char* pPrinterSerialNo = NULL;
	char* PrinterName = NULL;

	try
	{
		printf("Searching for printers via USB\n");

		pJob->GetPrinters(ConnectionTypeEnum(USB), &list);
		
		if ( list.vt != VT_EMPTY && list.parray->cDims >= 1 )
		{
			SAFEARRAY* psaPrinters;
			BSTR* pPrinters;
			BSTR PrinterDriverName;

			psaPrinters = list.parray;
			if ( psaPrinters->rgsabound[0].cElements != 0 )
			{
				SafeArrayAccessData(psaPrinters, (void**)&pPrinters);
                try
                {
					pPrinterSerialNo = BSTRtoChar(pPrinters[0]);
					
					PrinterDriverName = pJob->GetDriverName(pPrinterSerialNo);
					
					PrinterName = BSTRtoChar(PrinterDriverName);

					printf("Connecting to USB printer %s\n", pPrinterSerialNo);
					if (strcmp(PrinterName, "Unknown") == 0)
						printf("No printer driver found\n");
					else
						printf("Printer Driver Name = %s\n", PrinterName);
					 
					pJob->Open(pPrinters[0]);
					if (pJob->IsOpen)
					{
						
						//define print job:
						pJob->JobControl->FeederSource = CardFeeder;
						pJob->JobControl->Destination = Eject;
						pJob->JobControl->DeleteAfter = false;

						printf("Checking printer's status\n");

						if (IsPrinterReady(pJob))
						{

							printf("Printer %s is ready\n", pPrinterSerialNo);

							BSTR junk, model;
							pJob->Device->GetDeviceInfo(&junk, &model, &junk, &junk, &junk, &junk, &junk, &junk, &junk, &junk);

							if (wcsstr(model, L"7") == NULL && wcsstr(model,L"9") == NULL) // ZXP Series 7 printers do not have card type
								pJob->JobControl->CardType = "PVC,RE-XFER RDY";
							char* msg = new char[100];

							char imagePath[255];
							char FrontImagePath[255];
							char BackImagePath[255];
							if (GetImagePath(imagePath))
							{
								printf(imagePath);
								strcpy(FrontImagePath, imagePath);
								strcpy(BackImagePath, imagePath);

								GetFrontImagePath(FrontImagePath);
								GetBackImagePath(BackImagePath);
								printf(FrontImagePath);
								printf(BackImagePath);
								unsigned long imgLen = 0;
								unsigned char* tmpImg = NULL;
								bool result1 = false;

								//build the image using the ZMotifGraphics SDK:
								tmpImg = BuildColorImage(msg, FrontImagePath, &imgLen);
										
								if (tmpImg != NULL)
								{
									_variant_t vtGrData = pJob->Utilities->BytePtrToVariantArray( (unsigned char*)tmpImg, imgLen );
												
									pJob->BuildGraphicsLayers(SideEnum(Front), 
										                      PrintTypeEnum(Color), 
															  0, 0, 0, -1,
															  GraphicTypeEnum(BMP), vtGrData);

									result1 = true;
									delete tmpImg;
								}

								unsigned long imgLen1 = 0;
								unsigned char* tmpImg1 = NULL;
								bool result2 = false;
								
								//build the image using the ZMotifGraphics SDK:
								tmpImg1 = BuildMonoKImage(msg, BackImagePath, &imgLen1);
										
								if (tmpImg1 != NULL)
								{
									_variant_t vtGrData1 = pJob->Utilities->BytePtrToVariantArray( (unsigned char*)tmpImg1, imgLen1 );

									pJob->BuildGraphicsLayers(SideEnum(Back), 
										                      PrintTypeEnum(MonoK), 
															  0, 0, 0, -1,
															  GraphicTypeEnum(BMP), vtGrData1);

									result2 = true;
									delete tmpImg1;
								}
										
								if( (result1) && (result2) )
								{
									//perform print job
									printf("Printing image via ZMotif Printer & Graphics SDKs\n");

									long actionID = 0;
									pJob->PrintGraphicsLayers(1, &actionID);
										
									wchar_t status = 0;
									short alarm = 0;

									JobWait(pJob, actionID, 60, &status, &alarm);
								}
							}

							printf("\nPrinting complete\n");

							//Clear graphics buffer: 
							printf("Clearing graphics buffer\n");
										
							pJob->ClearGraphicsLayers(); 
						}
						else //printer not ready
						{
							printf("\nPrinter %s is not ready\n", pPrinterSerialNo);
						}
					}
					else
					{
						printf("Failed to open connection to %s\n", pPrinterSerialNo);
					}
				}
				catch (_com_error ex)
				{
					pJob->JobCancel(0); 
					pJob->EjectCard();
					printf("\nException inner try block of main1: %s", BSTRtoChar(ex.Description()));
				}
				SafeArrayUnaccessData(psaPrinters);
			}
		}
	}
	catch (_com_error ex)
	{
      pJob->JobCancel(0); 
	  pJob->EjectCard();
	  printf("\nException outer try block of main2: %s", BSTRtoChar(ex.Description()));
  	}

	if ( pJob->IsOpen )
            pJob->Close();

	ReleaseZMotifPrinterInterface();
	
	ReleaseZMotifGraphicsInterface();

	if (pPrinterSerialNo != NULL)
		free(pPrinterSerialNo);
	
	if (PrinterName != NULL)
		free(PrinterName);

	pJob.Release();

	char c;
	printf("Press any key to continue\n");
	scanf(&c);

	CoUninitialize();
		
	return 0;
}






