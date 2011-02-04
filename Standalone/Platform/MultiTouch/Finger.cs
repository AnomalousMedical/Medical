using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class Finger : PooledObject
    {
        private int id;
        private float xPos;
        private float yPos;
        private float lastXPos;
        private float lastYPos;
        private float deltaX;
        private float deltaY;

        public Finger()
        {

        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public float X
        {
            get
            {
                return xPos;
            }
        }

        public float Y
        {
            get
            {
                return yPos;
            }
        }

        public float LastX
        {
            get
            {
                return lastXPos;
            }
        }

        public float LastY
        {
            get
            {
                return lastYPos;
            }
        }

        public float DeltaX
        {
            get
            {
                return deltaX;
            }
        }

        public float DeltaY
        {
            get
            {
                return deltaY;
            }
        }

        internal void setCurrentPosition(float xPos, float yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            deltaX = xPos - lastXPos;
            deltaY = yPos - lastYPos;
        }

        internal void captureCurrentPositionAsLast()
        {
            lastXPos = xPos;
            lastYPos = yPos;
            deltaX = 0;
            deltaY = 0;
        }

        internal void finished()
        {
            this.returnToPool();
        }

        internal void setInfoOutOfPool(int id, float xPos, float yPos)
        {
            this.id = id;
            this.xPos = lastXPos = xPos;
            this.yPos = lastYPos = yPos;
        }

        protected override void reset()
        {
            id = -1;
        }
    }
}
