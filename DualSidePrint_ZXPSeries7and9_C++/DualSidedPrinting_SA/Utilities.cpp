/***************************************************************************************************************
*	Utilities.cpp : Defines generic functions not associated with the Zmotif SDKs.
*
*	This modules provides general-purpose functions for convenience.	
*
*	Copyright 2010 Zebra Technologies LLC, All rights reserved.
****************************************************************************************************************/

#include "stdafx.h"

#include "Utilities.h"

#include <olectl.h>
#include <ole2.h>

#define COUNTOF(x)  ( sizeof(x) / sizeof((x)[0]) )   
    
bool get_module_directory(TCHAR *obuf, size_t osize)   
{   
    if ( ! GetModuleFileName(0, obuf, osize) )   
    {   
        *obuf = '\0';// insure it's NUL terminated   
        return FALSE;   
    }   
    
    // run through looking for the *last* slash in this path.   
    // if we find it, NUL it out to truncate the following   
    // filename part.   
    
    TCHAR*lastslash = 0;   
    
    for ( ; *obuf; obuf ++)   
    {   
        if ( *obuf == '\\' || *obuf == '/' )   
            lastslash = obuf;   
    }   
    
    if ( lastslash ) *lastslash = '\0';   
    
    return TRUE;   
}  

void WcharToChar(char* str,TCHAR* pTchar)
{ 

	while( (*str++ = (char)*pTchar++) != '\0') ;
}
 
bool GetAppPath(char* path)
{
	TCHAR filebuf[256];

	if ( get_module_directory(filebuf, COUNTOF(filebuf)) )   
    {   
		
		WcharToChar(path, filebuf);
		return true;   
    }   
    else   
    {   
        return false;   
    } 
}

bool ReplaceSubstring(const char *s, const char *oldOne, const char *newOne, char* ret)
{
	int i, count = 0;
	size_t newlen = strlen(newOne);
	size_t oldlen = strlen(oldOne);

	for (i = 0; s[i] != '\0'; i++) 
	{
		if (strstr(&s[i], oldOne) == &s[i]) 
		{
			count++;
			i += oldlen - 1;
		}
	}

	i = 0;
	while (*s) 
	{
		if (strstr(s, oldOne) == s) 
		{
			strcpy(&ret[i], newOne);
			i += newlen;
			s += oldlen;
		} 
		else
			ret[i++] = *s++;
	}
	ret[i] = '\0';

	return true;
}

bool GetImagePath(char* path)
{
	char imgPath[255];
	char CleanImagePath[255];
	
	if (!GetAppPath(imgPath))
	{
		printf("\nGetAppPath failed");

        return false;
    }
				
	char* pos = strstr(imgPath, "Debug");

	if (pos != NULL)
	{
		ReplaceSubstring(imgPath, "Debug", "", CleanImagePath);
				
	}
	else 
	{
		pos = strstr(imgPath, "Release");
		if (pos != NULL)
		{	
			ReplaceSubstring(imgPath, "Release", "", CleanImagePath);
		}
	}
		        
	strcpy(path, CleanImagePath);
	
	return true;
}

bool GetFrontImagePath(char* path)
{
	strcat(path, "ZXPFront.bmp");
	return true;
}

bool GetBackImagePath(char* path)
{
	strcat(path, "ZXPBack.bmp");
	return true;
}


char* BSTRtoChar(BSTR String)
{
    int n, i;
    char* FinalChar;
    
	n = SysStringLen(String); // length of input
    
	FinalChar = (char*) malloc(n+1);
    
	for (i = 0; i < n; i++)
    {
        FinalChar[i] = (char) String[i];
    }
    
	FinalChar[i] = 0;
    
	return FinalChar;
}