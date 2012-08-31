#include "stdafx.h"
#include "NativeStringEnumerator.h"
#include "NativeString.h"

#ifdef USE_WXWIDGETS

NativeStringEnumerator::NativeStringEnumerator()
	:currentIndex(-1)
{

}

NativeStringEnumerator::~NativeStringEnumerator()
{

}

wxArrayString& NativeStringEnumerator::getArrayString()
{
	return arrayString;
}

wxString& NativeStringEnumerator::getCurrent()
{
	return arrayString[currentIndex];
}

bool NativeStringEnumerator::moveNext()
{
	return ++currentIndex < arrayString.Count();
}

void NativeStringEnumerator::reset()
{
	currentIndex = -1;
}

//PInvoke

extern "C" _AnomalousExport NativeStringEnumerator* NativeStringEnumerator_new()
{
	return new NativeStringEnumerator();
}

extern "C" _AnomalousExport void NativeStringEnumerator_delete(NativeStringEnumerator* enumerator)
{
	delete enumerator;
}

extern "C" _AnomalousExport NativeString* NativeStringEnumerator_getCurrent(NativeStringEnumerator* enumerator)
{
	return new NativeString(enumerator->getCurrent());
}

extern "C" _AnomalousExport bool NativeStringEnumerator_moveNext(NativeStringEnumerator* enumerator)
{
	return enumerator->moveNext();
}

extern "C" _AnomalousExport void NativeStringEnumerator_reset(NativeStringEnumerator* enumerator)
{
	enumerator->reset();
}

#endif