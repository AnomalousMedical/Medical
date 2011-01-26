#pragma once
class NativeStringEnumerator
{
public:
	NativeStringEnumerator();

	virtual ~NativeStringEnumerator();

	wxArrayString& getArrayString();

	wxString& getCurrent();

	bool moveNext();

	void reset();

private:
	wxArrayString arrayString;
	int currentIndex;
};