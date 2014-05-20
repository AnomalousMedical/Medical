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

	Windows::UI::Core::CoreWindow^ getWindow();

protected:
	// Application lifecycle event handlers.
	void OnActivated(Windows::ApplicationModel::Core::CoreApplicationView^ applicationView, Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);
	void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ args);
	void OnResuming(Platform::Object^ sender, Platform::Object^ args);
	void OnVisibilityChanged(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::VisibilityChangedEventArgs ^args);

private:
	WinRTApp* anomalousApp;
	static AnomalousRTFramework^ instance;
	bool runningLoop;
	bool windowVisible;
	Windows::UI::Core::CoreWindow^ mainWindow;
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