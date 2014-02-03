using Engine.Attributes;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SceneThumbInfo : Saveable, IDisposable
    {
        [DoNotSave]
        private Bitmap sceneThumb;

        private int includeX;
        private int includeY;

        public SceneThumbInfo()
        {

        }

        public SceneThumbInfo(int includeX, int includeY)
        {
            this.includeX = includeX;
            this.includeY = includeY;
        }

        public void Dispose()
        {
            if (sceneThumb != null)
            {
                sceneThumb.Dispose();
                sceneThumb = null;
            }
        }

        /// <summary>
        /// Set the scene thumb for this thumb info. This class will take control of the bitmap,
        /// so if you dispose this it will dispose the set scene thumb and if you set this property
        /// to a new value the old thumb will be disposed.
        /// </summary>
        public Bitmap SceneThumb
        {
            get
            {
                return sceneThumb;
            }
            set
            {
                if (sceneThumb != null)
                {
                    sceneThumb.Dispose();
                }
                sceneThumb = value;
            }
        }

        public int IncludeX
        {
            get
            {
                return includeX;
            }
            set
            {
                includeX = value;
            }
        }

        public int IncludeY
        {
            get
            {
                return includeY;
            }
            set
            {
                includeY = value;
            }
        }

        protected SceneThumbInfo(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }
}
