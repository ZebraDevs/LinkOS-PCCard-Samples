#ifndef _UTILITIES_H
#define _UTILITIES_H

bool get_module_directory(TCHAR *obuf, size_t osize);
bool GetAppPath(char* path);
bool ReplaceSubstring(const char *s, const char *oldOne, const char *newOne, char* ret);

bool GetImagePath(char* path);
bool GetFrontImagePath(char* path);
bool GetBackImagePath(char* path);

char* BSTRtoChar(BSTR String);

#endif