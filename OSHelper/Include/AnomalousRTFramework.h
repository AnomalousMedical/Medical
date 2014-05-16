#pragma once

class WinRTApp;

ref class AnomalousRTFramework sealed : public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
	AnomalousRTFramework(WinRTApp* anomalousApp);

	static AnomalousRTFramework^ getSingleton()
	{
		return instance;
	}

public:
	virtual ~AnomalousRTFramework();

	// IFrameworkView Methods.
	virtual void Initialize(Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
	virtual void SetWindow(Windows::UI::Core::CoreWindow^ window);
	virtual void Load(Platform::String^ entryPoint);
	virtual void Run();
	virtual void Uninitialize();

private:
	WinRTApp* anomalousApp;
	static AnomalousRTFramework^ instance;
};


// This class creates the AnomalousRTFramework instance.
ref class AnomalousFrameworkSource sealed : Windows::ApplicationModel::Core::IFrameworkViewSource
{
internal:
	AnomalousFrameworkSource(WinRTApp* anomalousApp);

public:
	virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();

private:
	WinRTApp* anomalousApp;
};