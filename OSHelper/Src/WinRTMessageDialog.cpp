#include "StdAfx.h"
#include "NativeDialog.h"
#include "NativeOSWindow.h"
#include "AnomalousRTFramework.h"

extern "C" _AnomalousExport void MessageDialog_showErrorDialog(NativeOSWindow* parent, String msg, String cap)
{
	auto dispatcher = AnomalousRTFramework::getSingleton()->getWindow()->Dispatcher;
	auto pMsg = ref new Platform::String(msg);

	dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
		ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		Windows::UI::Popups::MessageDialog^ dlg = ref new Windows::UI::Popups::MessageDialog(pMsg);
		dlg->ShowAsync();
	}));
}

extern "C" _AnomalousExport NativeDialogResult MessageDialog_showQuestionDialog(NativeOSWindow* parent, String msg, String cap)
{
	auto dispatcher = AnomalousRTFramework::getSingleton()->getWindow()->Dispatcher;
	auto pMsg = ref new Platform::String(msg);

	dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
		ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		Windows::UI::Popups::MessageDialog^ dlg = ref new Windows::UI::Popups::MessageDialog(pMsg);
		dlg->Commands->Append(ref new Windows::UI::Popups::UICommand("Yes"));
		dlg->Commands->Append(ref new Windows::UI::Popups::UICommand("No"));
		dlg->ShowAsync();
	}));

	return NativeDialogResult::CANCEL;
}