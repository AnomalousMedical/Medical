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
        private NumberLine numberLine;

        public NumberLineNumber(StaticText text, Widget hashMark, NumberLine numberLine)
        {
            this.text = text;
            this.hashMark = hashMark;
            this.numberLine = numberLine;
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
                int min = (int)(time / 60);
                int sec = (int)(time % 60);
                if (sec < 10)
                {
                    text.Caption = String.Format("{0}:0{1}", min, sec);
                }
                else
                {
                    text.Caption = String.Format("{0}:{1}", min, sec);
                }
                Size2 textSize = text.getTextSize();
                text.setSize((int)textSize.Width, (int)textSize.Height);
                text.setPosition((int)((numberLine.PixelsPerSecond * time) - text.Width / 2), text.Top);
                hashMark.setPosition((int)(numberLine.PixelsPerSecond * time), hashMark.Top);
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
                hashMark.Visible = value;
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
            actionView.PixelsPerSecondChanged += new EventHandler(actionView_PixelsPerSecondChanged);
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
                Logging.Log.Debug(pixelsPerSecond.ToString());
                if (pixelsPerSecond == 1)
                {
                    numberSeparationDuration = 40.0f;
                }
                else if (pixelsPerSecond <= 3)
                {
                    numberSeparationDuration = 20.0f;
                }
                else if (pixelsPerSecond <= 7)
                {
                    numberSeparationDuration = 10.0f;
                }
                else if (pixelsPerSecond <= 10)
                {
                    numberSeparationDuration = 5.0f;
                }
                else if (pixelsPerSecond <= 20)
                {
                    numberSeparationDuration = 3.0f;
                }
                else if (pixelsPerSecond <= 40)
                {
                    numberSeparationDuration = 2.0f;
                }
                else
                {
                    numberSeparationDuration = 1.0f;
                }
                foreach(NumberLineNumber number in activeNumbers)
                {
                    returnNumberToPool(number);
                }
                activeNumbers.Clear();
                canvasModified();
            }
        }

        void actionView_CanvasPositionChanged(CanvasEventArgs info)
        {
            numberlineScroller.CanvasPosition = new Vector2(-info.Left, 0.0f);
            canvasModified();
        }

        void actionView_PixelsPerSecondChanged(object sender, EventArgs e)
        {
            ActionView actionView = (ActionView)sender;
            this.PixelsPerSecond = actionView.PixelsPerSecond;
        }

        void actionView_CanvasWidthChanged(float newSize)
        {
            numberlineScroller.CanvasSize = new Size2(newSize > numberlineScroller.Width ? newSize : numberlineScroller.Width, numberlineScroller.Height);
            canvasModified();
        }

        void canvasModified()
        {
            float leftSide = numberlineScroller.CanvasPosition.x - pixelsPerSecond * numberSeparationDuration;
            float rightSide = leftSide + numberlineScroller.ClientCoord.width + pixelsPerSecond * numberSeparationDuration;

            //Remove inactive numbers
            for (int i = 0; i < activeNumbers.Count; ++i)
            {
                NumberLineNumber number = activeNumbers[i];
                if (number.Right < leftSide)
                {
                    activeNumbers.RemoveAt(i--);
                    returnNumberToPool(number);
                }
                if (number.Left > rightSide)
                {
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
                    NumberLineNumber number = getPooledNumber();
                    number.Time = i;
                    activeNumbers.Insert(0, number);
                }

                //Add numbers to the back of the list
                startingPoint = activeNumbers[activeNumbers.Count - 1].Time + numberSeparationDuration;
                for (float i = startingPoint; i * pixelsPerSecond < rightSide; i += numberSeparationDuration)
                {
                    NumberLineNumber number = getPooledNumber();
                    number.Time = i;
                    activeNumbers.Add(number);
                }
            }
            //If there are currently no active numbers
            else
            {
                //NEED TO COMPUTE THE STARTING VALUE CORRECTLY SO IT STAYS ALIGNED
                float startingPoint = leftSide / pixelsPerSecond;
                NumberLineNumber number = null;
                for (float i = startingPoint; i * pixelsPerSecond < rightSide; i += numberSeparationDuration)
                {
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
                    numberlineScroller.createWidgetT("Widget", "Separator1", 0, 0, 1, 8, Align.Left | Align.Top, ""), this);
            }
            return number;
        }

        private void returnNumberToPool(NumberLineNumber number)
        {
            number.Visible = false;
            inactiveNumbers.Add(number);
        }
    }
}
