using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class NumberLineNumber
    {
        private StaticText text;
        private Widget hashMark;
        private float time;
        private int pixelsPerSecond;

        public NumberLineNumber(StaticText text, Widget hashMark, int pixelsPerSecond)
        {
            this.text = text;
            this.hashMark = hashMark;
            this.pixelsPerSecond = pixelsPerSecond;
        }

        public int Left
        {
            get
            {
                return text.Left;
            }
        }

        public int Right
        {
            get
            {
                return text.Right;
            }
        }

        public float Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                text.Caption = time.ToString();
                Size2 textSize = text.getTextSize();
                text.setSize((int)textSize.Width, (int)textSize.Height);
                text.setPosition((int)((pixelsPerSecond * time) - text.Width / 2), text.Top);
                hashMark.setPosition((int)(pixelsPerSecond * time), hashMark.Top);
            }
        }

        public int PixelsPerSecond
        {
            get
            {
                return pixelsPerSecond;
            }
            set
            {
                pixelsPerSecond = value;
                text.setPosition((int)((pixelsPerSecond * time) - text.Width / 2), text.Top);
                hashMark.setPosition((int)(pixelsPerSecond * time), hashMark.Top);
            }
        }

        public bool Visible
        {
            get
            {
                return text.Visible;
            }
            set
            {
                text.Visible = value;
            }
        }
    }

    class NumberLine
    {
        private ScrollView numberlineScroller;
        private int pixelsPerSecond;
        private float numberSeparationDuration = 1.0f;

        private List<NumberLineNumber> activeNumbers = new List<NumberLineNumber>();
        private List<NumberLineNumber> inactiveNumbers = new List<NumberLineNumber>();

        public NumberLine(ScrollView numberlineScroller, ActionView actionView)
        {
            this.numberlineScroller = numberlineScroller;
            pixelsPerSecond = actionView.PixelsPerSecond;
            actionView.CanvasPositionChanged += new CanvasPositionChanged(actionView_CanvasPositionChanged);
            actionView.CanvasWidthChanged += new CanvasSizeChanged(actionView_CanvasWidthChanged);

            canvasModified();
        }

        void actionView_CanvasWidthChanged(float newSize)
        {
            numberlineScroller.CanvasSize = new Size2(newSize > numberlineScroller.Width ? newSize : numberlineScroller.Width, numberlineScroller.Height);
            canvasModified();
        }

        public int PixelsPerSecond
        {
            get
            {
                return pixelsPerSecond;
            }
            set
            {
                pixelsPerSecond = value;
            }
        }

        void actionView_CanvasPositionChanged(CanvasEventArgs info)
        {
            numberlineScroller.CanvasPosition = new Vector2(-info.Left, 0.0f);
            canvasModified();
        }

        void canvasModified()
        {
            float leftSide = numberlineScroller.CanvasPosition.x - pixelsPerSecond * numberSeparationDuration;
            float rightSide = leftSide + numberlineScroller.ClientCoord.width + pixelsPerSecond * numberSeparationDuration;

            Logging.Log.Debug("{0} {1}", leftSide, rightSide);

            //Remove inactive numbers
            for (int i = 0; i < activeNumbers.Count; ++i)
            {
                NumberLineNumber number = activeNumbers[i];
                if (number.Right < leftSide)
                {
                    Logging.Log.Debug("Removing number at index {0}", i);
                    activeNumbers.RemoveAt(i--);
                    returnNumberToPool(number);
                }
                if (number.Left > rightSide)
                {
                    Logging.Log.Debug("Removing number at index {0}", i);
                    activeNumbers.RemoveAt(i--);
                    returnNumberToPool(number);
                }
            }

            //If there are any active numbers
            if (activeNumbers.Count > 0)
            {
                //See if any numbers need to be added to the front of the list
                float startingPoint = activeNumbers[0].Time - numberSeparationDuration;
                for (float i = startingPoint; i * pixelsPerSecond > leftSide; i -= numberSeparationDuration)
                {
                    Logging.Log.Debug("Adding FRONT number at time {0}", i);
                    NumberLineNumber number = getPooledNumber();
                    number.Time = i;
                    activeNumbers.Insert(0, number);
                }

                //Add numbers to the back of the list
                startingPoint = activeNumbers[activeNumbers.Count - 1].Time + numberSeparationDuration;
                for (float i = startingPoint; i * pixelsPerSecond < rightSide; i += numberSeparationDuration)
                {
                    Logging.Log.Debug("Adding REAR number at time {0}", i);
                    NumberLineNumber number = getPooledNumber();
                    number.Time = i;
                    activeNumbers.Add(number);
                }
            }
            //If there are currently no active numbers
            else
            {
                float startingPoint = leftSide / pixelsPerSecond;
                NumberLineNumber number = null;
                for (float i = startingPoint; i * pixelsPerSecond < rightSide; i += numberSeparationDuration)
                {
                    Logging.Log.Debug("Adding NEW number at time {0}", i);
                    number = getPooledNumber();
                    number.Time = i;
                    activeNumbers.Add(number);
                }
                numberlineScroller.CanvasSize = new Size2(number.Right, numberlineScroller.Height);
            }
        }

        private NumberLineNumber getPooledNumber()
        {
            NumberLineNumber number;
            if (inactiveNumbers.Count > 0)
            {
                number = inactiveNumbers[0];
                inactiveNumbers.RemoveAt(0);
                number.Visible = true;
            }
            else
            {
                number = new NumberLineNumber(numberlineScroller.createWidgetT("StaticText", "StaticText", 0, 9, 10, 15, Align.Left | Align.Top, "") as StaticText,
                    numberlineScroller.createWidgetT("Widget", "Separator1", 0, 0, 1, 8, Align.Left | Align.Top, ""), pixelsPerSecond);
            }
            return number;
        }

        private void returnNumberToPool(NumberLineNumber text)
        {
            text.Visible = false;
            inactiveNumbers.Add(text);
        }
    }
}
