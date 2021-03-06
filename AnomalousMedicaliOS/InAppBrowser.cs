﻿using System;
using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Medical.Controller;
using Anomalous.OSPlatform;

namespace AnomalousMedicaliOS
{
	public class InAppBrowser : IDisposable
	{
		private const double AnimationDuration = 0.5;

		UIView view;
		UIButton closeButton;
		UIWebView webView;
		UIActivityIndicatorView indicatorView;
		AnimationCompletedDelegate animationComplete;
		TouchMouseGuiForwarder touchForwarder;

		public InAppBrowser(UIView parentView, String url, TouchMouseGuiForwarder touchForwarder)
		{
			this.touchForwarder = touchForwarder;
			touchForwarder.Enabled = false;

			var parentBounds = parentView.Bounds;

			view = new UIView(parentView.Bounds);
			view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			closeButton = new UIButton(new CGRect(parentBounds.Left, parentBounds.Top, 100, 44));
			closeButton.SetTitle("Close", UIControlState.Normal);
			view.AddSubview(closeButton);
			CGRect frame = view.Frame;
			frame.Y = frame.Height;
			view.Frame = frame;

			var buttonBounds = closeButton.Bounds;

			indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			indicatorView.Frame = new CGRect(buttonBounds.Right, buttonBounds.Top, 44, 44);
			view.AddSubview(indicatorView);
			indicatorView.StartAnimating();

			webView = new UIWebView(new CGRect(parentBounds.Left, buttonBounds.Bottom, parentBounds.Width, parentView.Bounds.Height - buttonBounds.Height));
			webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			view.AddSubview(webView);
			webView.LoadFinished += HandleLoadFinished;
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			closeButton.TouchUpInside += HandleTouchUpInside;

			parentView.AddSubview(view);

			UIView.BeginAnimations("slideAnimation");
			UIView.SetAnimationDuration(AnimationDuration);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseOut);
			UIView.SetAnimationRepeatCount(0);
			UIView.SetAnimationRepeatAutoreverses(false);

			frame.Y = 0;
			view.Frame = frame;

			UIView.CommitAnimations();
		}

		public void Dispose()
		{
			touchForwarder.Enabled = true;

			closeButton.TouchUpInside -= HandleTouchUpInside;

			if(animationComplete != null)
			{
				animationComplete.Dispose();
				animationComplete = null;
			}

			indicatorView.RemoveFromSuperview();
			indicatorView.Dispose();
			webView.LoadFinished -= HandleLoadFinished;
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
			closeButton.TouchUpInside -= HandleTouchUpInside;

			CGRect frame = view.Frame;
			frame.Y = frame.Height;

			UIView.BeginAnimations("slideAnimation");
			UIView.SetAnimationDuration(AnimationDuration);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseIn);
			UIView.SetAnimationRepeatCount(0);
			UIView.SetAnimationRepeatAutoreverses(false);
			animationComplete = new AnimationCompletedDelegate(this);
			UIView.SetAnimationDelegate(animationComplete);
			UIView.SetAnimationDidStopSelector(new Selector("slideAnimationFinished"));

			view.Frame = frame;

			UIView.CommitAnimations();
		}

		void HandleLoadFinished (object sender, EventArgs e)
		{
			indicatorView.StopAnimating();
		}

		class AnimationCompletedDelegate : NSObject
		{
			private InAppBrowser browser;

			public AnimationCompletedDelegate(InAppBrowser browser)
			{
				this.browser = browser;
			}

			protected override void Dispose(bool disposing)
			{
				browser = null;
				base.Dispose(disposing);
			}

			[Export("slideAnimationFinished")]
			void SlideStopped()
			{
				browser.Dispose();
			}
		}
	}
}

