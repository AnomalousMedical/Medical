using Anomalous.libRocketWidget;
using Anomalous.OSPlatform;
using Engine;
using Engine.Platform;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    public class TouchMouseGuiForwarder
    {
        private int currentFingerId = int.MinValue;
        private IntVector2 gestureStartPos;

        private Touches touches; //Ghetto
        private NativeOSWindow window;
        private RocketWidget currentRocketWidget;
        private NativeInputHandler inputHandler;
		private bool enabled = true;
		private bool keyboardVisible = false;

        public TouchMouseGuiForwarder(EventManager eventManager, NativeInputHandler inputHandler, NativeOSWindow window)
        {
            this.touches = eventManager.Touches;
            this.touches.FingerStarted += HandleFingerStarted;
            this.inputHandler = inputHandler;
            this.window = window;
            InputManager.Instance.ChangeKeyFocus += HandleChangeKeyFocus;
            RocketWidget.ElementFocused += HandleElementFocused;
			RocketWidget.ElementBlurred += HandleElementBlurred;
			RocketWidget.RocketWidgetDisposing += HandleRocketWidgetDisposing;

			eventManager[EventLayers.Last].Keyboard.KeyPressed += HandleKeyPressed;
			eventManager[EventLayers.Last].Keyboard.KeyReleased += HandleKeyReleased;
        }

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				if(!enabled && currentFingerId == int.MinValue)
				{
					stopTrackingFinger();
				}
			}
		}

        void HandleElementFocused(RocketWidget rocketWidget, Element element)
        {
            if (element != null)
            {
                currentRocketWidget = rocketWidget;
                String tag = element.TagName;
				switch (tag) 
				{
					case "input":
						String type = element.GetAttributeString ("type");
						keyboardVisible = type == "text" || type == "password";
						break;
					case "textarea":
						keyboardVisible = true;
						break;
					default:
						keyboardVisible = false;
						break;
				}
			}
		}

		void HandleElementBlurred (RocketWidget widget, Element element)
		{
			if(widget == currentRocketWidget)
			{
				keyboardVisible = false;
			}
		}

		void HandleRocketWidgetDisposing(RocketWidget widget)
		{
			if(widget == currentRocketWidget)
			{
				currentRocketWidget = null;
				keyboardVisible = false;
				//Handle these for keyboard toggle right away or it won't work
				toggleKeyboard();
			}
		}

        void HandleChangeKeyFocus(Widget widget)
        {
            if (currentRocketWidget == null || !currentRocketWidget.isHostWidget(widget))
            {
                keyboardVisible = widget != null && widget is EditBox;
            }
        }

        void HandleFingerStarted(Finger obj)
        {
            if (currentFingerId == int.MinValue && enabled)
            {
                var finger = touches.Fingers[0];
                currentFingerId = finger.Id;
                touches.FingerEnded += fingerEnded;
                touches.FingerMoved += HandleFingerMoved;
				touches.FingersCanceled += HandleFingersCanceled;
                gestureStartPos = new IntVector2(finger.PixelX, finger.PixelY);
                inputHandler.injectMoved(finger.PixelX, finger.PixelY);
                inputHandler.injectButtonDown(MouseButtonCode.MB_BUTTON0);
            }
        }

        void HandleFingerMoved(Finger obj)
        {
            if (obj.Id == currentFingerId)
            {
                inputHandler.injectMoved(obj.PixelX, obj.PixelY);
            }
        }

        void fingerEnded(Finger obj)
        {
            if (obj.Id == currentFingerId)
            {
				stopTrackingFinger();
                inputHandler.injectMoved(obj.PixelX, obj.PixelY);
                inputHandler.injectButtonUp(MouseButtonCode.MB_BUTTON0);
				toggleKeyboard();
            }
		}

		void HandleFingersCanceled()
		{
			if(currentFingerId != int.MinValue)
			{
				stopTrackingFinger();
				inputHandler.injectButtonUp(MouseButtonCode.MB_BUTTON0);
				toggleKeyboard();
			}
		}

		void stopTrackingFinger()
		{
			touches.FingerEnded -= fingerEnded;
			touches.FingerMoved -= HandleFingerMoved;
			currentFingerId = int.MinValue;
		}

		void toggleKeyboard()
		{
			if(keyboardVisible != window.OnscreenKeyboardVisible)
			{
				window.OnscreenKeyboardVisible = keyboardVisible;
			}
		}

		void HandleKeyReleased(KeyboardButtonCode keyCode)
		{
			toggleKeyboard();
		}

		void HandleKeyPressed(KeyboardButtonCode keyCode, uint keyChar)
		{
			toggleKeyboard();
		}
    }
}
