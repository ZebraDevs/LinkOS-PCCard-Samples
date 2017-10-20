/***************************************************************************************************************
*	ZMotifGraphicsSDK.cpp : Defines ZMotif Graphic SDK entry points.
*
*	This modules provides functions which use ZMotif Graphics SDK features.	
*
*	Copyright 2010 Zebra Technologies LLC, All rights reserved.
****************************************************************************************************************/
#include "stdafx.h"

#include "ZMotifGraphicsSDK.h"

#include "Utilities.h" 

#include <string.h>

//ZMotifGraphics Interface pointer:
IZMotifGraphicsPtr pGraphics;

bool GetZMotifGraphicsInterface()
{
	bool result = false;
	try
	{
		HRESULT hr = CoCreateInstance(CLSID_ZMotifGraphics, NULL, CLSCTX_INPROC_SERVER, IID_IZMotifGraphics, (void**)&pGraphics);
		
		if (SUCCEEDED(hr))
			result = true;
	}
	catch(_com_error ex)
	{
		result = false;
		printf("\nGetZMotifGraphicsInterface failed: &s", BSTRtoChar(ex.Description()));
	}
	return result;
}

bool ReleaseZMotifGraphicsInterface()
{
	bool result = false;
	try
	{
		HRESULT hr = pGraphics->CloseGraphics();
		if (SUCCEEDED(hr))
			hr = pGraphics->Release();
		
		if (SUCCEEDED(hr))
			result = true;

	}
	catch(_com_error ex)
	{
		result = false;
		printf("\nReleaseZMotifGraphicsInterface failed: &s", BSTRtoChar(ex.Description()));
	}
	return result;
}

unsigned char* BuildColorImage(char* msg, char* theImage, unsigned long* pimgLen)
{
	strcpy(msg, "");

	ImageOrientationEnum orient = ImageOrientationEnum_Landscape;
	ImagePositionEnum pos = ImagePositionEnum_Centered;
	RibbonTypeEnum ribbon = RibbonTypeEnum_Color;

	unsigned char* tmpImg = NULL;

	try
	{
		long dataLen = 0;

		pGraphics->InitGraphics_2(0, 0, orient, ribbon);

		SAFEARRAY* image = pGraphics->ImageFileToByteArray(theImage);
				
		pGraphics->DrawImage_3(&image, pos, 1100, 700, 0);

		if(image != NULL)
		{
			image = pGraphics->CreateBitmap(&dataLen);

			unsigned short dims = image->cDims;
			*pimgLen = image->rgsabound[0].cElements;

			tmpImg = new unsigned char[*pimgLen];
			memcpy( tmpImg, (char*)image->pvData, *pimgLen );
		}
			
		pGraphics->ClearGraphics();
	}
	catch(_com_error ex)
	{
		tmpImg = NULL;
		printf("\nBuildColorImage threw exception: &s", BSTRtoChar(ex.Description()));
	}
	catch(char* exp)
	{
		tmpImg = NULL;
		strcpy(msg, exp);
		printf("\nBuildColorImage threw exception: &s", exp);
		
	}
	return tmpImg;
}

unsigned char* BuildMonoKImage(char* msg, char* theImage, unsigned long* pimgLen)
{
	strcpy(msg, "");

	ImageOrientationEnum orient = ImageOrientationEnum_Landscape;
	ImagePositionEnum pos = ImagePositionEnum_Centered;
	RibbonTypeEnum ribbon = RibbonTypeEnum_MonoK;

	unsigned char* tmpImg = NULL;

	try
	{
		long dataLen = 0;

		pGraphics->InitGraphics_2(0, 0, orient, ribbon);

		SAFEARRAY* image = pGraphics->ImageFileToByteArray(theImage);

		pGraphics->DrawImage_3(&image, pos, 1100, 700, 0);

		if(image != NULL)
		{
			image = pGraphics->CreateBitmap(&dataLen);

			unsigned short dims = image->cDims;
			*pimgLen = image->rgsabound[0].cElements;

			tmpImg = new unsigned char[*pimgLen];
		
			memcpy( tmpImg, (char*)image->pvData, *pimgLen );
		}
		
		pGraphics->ClearGraphics();
	}
	catch(_com_error ex)
	{
		tmpImg = NULL;
		printf("\nBuildMonoKImage threw exception: &s", BSTRtoChar(ex.Description()));
	}
	catch(char* exp)
	{
		strcpy(msg, exp);
		printf("\nBuildMonoKImage threw exception: &s", exp);
		tmpImg = NULL;
	}
	return tmpImg;
}


