using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace AnomalousMedicaliOS
{
	public class InAppBrowser : IDisposable
	{
		UIView view;
		UIButton closeButton;
		UIWebView webView;

		public InAppBrowser(UIView parentView, String url)
		{
			var parentBounds = parentView.Bounds;

			view = new UIView(parentView.Bounds);
			closeButton = new UIButton(new CGRect(parentBounds.Left, parentBounds.Top, 100, 44));
			closeButton.SetTitle("Close", UIControlState.Normal);
			view.AddSubview(closeButton);
			CGRect frame = view.Frame;
			frame.Y = frame.Height;
			view.Frame = frame;

			var buttonBounds = closeButton.Bounds;

			webView = new UIWebView(new CGRect(parentBounds.Left, buttonBounds.Bottom, parentBounds.Width, parentView.Bounds.Height - buttonBounds.Height));
			view.AddSubview(webView);
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			closeButton.TouchUpInside += HandleTouchUpInside;

			parentView.AddSubview(view);

			UIView.BeginAnimations("slideAnimation");
			UIView.SetAnimationDuration(0.8);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseOut);
			UIView.SetAnimationRepeatCount(0);
			UIView.SetAnimationRepeatAutoreverses(false);

			frame.Y = 0;
			view.Frame = frame;

			UIView.CommitAnimations();
		}

		public void Dispose()
		{
			closeButton.TouchUpInside -= HandleTouchUpInside;

			webView.RemoveFromSuperview();
			webView.Dispose();
			webView = null;
			closeButton.RemoveFromSuperview();
			closeButton.Dispose();
			closeButton = null;
			view.RemoveFromSuperview();
			view.Dispose();
			view = null;
		}

		void HandleTouchUpInside (object sender, EventArgs e)
		{
			this.Dispose();
		}
	}
}

