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
    class TouchMouseGuiForwarder
    {
        private int currentFingerId = int.MinValue;
        private IntVector2 gestureStartPos;

        private Touches touches; //Ghetto
        private NativeOSWindow window;
        private RocketWidget currentRocketWidget;

        public TouchMouseGuiForwarder(EventManager eventManager, NativeOSWindow window)
        {
            this.touches = eventManager.Touches;
            this.touches.FingerStarted += HandleFingerStarted;
            this.window = window;
            InputManager.Instance.ChangeKeyFocus += HandleChangeKeyFocus;
            RocketWidget.ElementFocused += HandleElementFocused;
        }

        void HandleElementFocused(RocketWidget rocketWidget, Element element)
        {
            if (element != null)
            {
                currentRocketWidget = rocketWidget;
                String tag = element.TagName;
				bool makeKeyboardVisible = false;
				switch (tag) 
				{
					case "input":
						String type = element.GetAttributeString ("type");
						makeKeyboardVisible = type == "text" || type == "password";
						break;
					case "textarea":
						makeKeyboardVisible = true;
						break;
				}
				window.setOnscreenKeyboardVisible(makeKeyboardVisible);
            }
        }

        void HandleChangeKeyFocus(Widget widget)
        {
            if (currentRocketWidget == null || !currentRocketWidget.isHostWidget(widget))
            {
                window.setOnscreenKeyboardVisible(widget != null && widget is EditBox);
            }
        }

        void HandleFingerStarted(Finger obj)
        {
            if (currentFingerId == int.MinValue)
            {
                var finger = touches.Fingers[0];
                currentFingerId = finger.Id;
                touches.FingerEnded += fingerEnded;
                touches.FingerMoved += HandleFingerMoved;
                gestureStartPos = new IntVector2(finger.PixelX, finger.PixelY);
                InputManager.Instance.injectMousePress(obj.PixelX, obj.PixelY, MouseButtonCode.MB_BUTTON0);
            }
        }

        void HandleFingerMoved(Finger obj)
        {
            if (obj.Id == currentFingerId)
            {
                InputManager.Instance.injectMouseMove(obj.PixelX, obj.PixelY, 0);
            }
        }

        void fingerEnded(Finger obj)
        {
            if (obj.Id == currentFingerId)
            {
                touches.FingerEnded -= fingerEnded;
                touches.FingerMoved -= HandleFingerMoved;
                currentFingerId = int.MinValue;
                InputManager.Instance.injectMouseRelease(obj.PixelX, obj.PixelY, MouseButtonCode.MB_BUTTON0);
            }
        }
    }
}
