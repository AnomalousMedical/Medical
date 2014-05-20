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

	void stopRunLoop();

protected:
	// Window event handlers.
	void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
	void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
	void OnWindowClosed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::CoreWindowEventArgs^ args);

private:
	WinRTApp* anomalousApp;
	static AnomalousRTFramework^ instance;
	bool runningLoop;
	bool windowVisible;
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