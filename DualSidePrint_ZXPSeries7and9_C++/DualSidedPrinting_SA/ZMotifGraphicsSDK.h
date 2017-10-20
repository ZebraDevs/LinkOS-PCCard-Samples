#ifndef _ZMOTIF_GRAPHICS_SDK_H
#define _ZMOTIF_GRAPHICS_SDK_H

#include "ZMotifPrinterSDK.h" //Zmotif dll import, SDK+ function prototypes, etc.

#import "ZMotifGraphics.tlb" no_namespace named_guids

extern IZMotifGraphicsPtr pGraphics;

bool BuildColorImage(char* msg, char* theImage, SAFEARRAY* image);
unsigned char* BuildColorImage(char* msg, char* theImage, unsigned long* pimgLen);

bool BuildMonoKImage(char* msg, char* theImage, SAFEARRAY* image);
unsigned char* BuildMonoKImage(char* msg,  char* theImage, unsigned long* pimgLen);

bool GetImagePath(char* path);

bool GetZMotifGraphicsInterface();

bool ReleaseZMotifGraphicsInterface();

#endif