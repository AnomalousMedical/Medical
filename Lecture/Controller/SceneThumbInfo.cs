using Engine;
using Engine.Attributes;
using Engine.Saving;
using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SceneThumbInfo : Saveable, IDisposable
    {
        [DoNotSave]
        private FreeImageBitmap sceneThumb;

        private int includeX;
        private int includeY;
        private Color color;

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
        /// Returns a copy of this object, it will also create a copy of its image
        /// and the caller is responsible for disposing the created copy.
        /// </summary>
        /// <returns></returns>
        public SceneThumbInfo copy()
        {
            SceneThumbInfo copy = CopySaver.Default.copy(this);
            copy.SceneThumb = new FreeImageBitmap(sceneThumb);
            return copy;
        }

        /// <summary>
        /// Set the scene thumb for this thumb info. This class will take control of the bitmap,
        /// so if you dispose this it will dispose the set scene thumb and if you set this property
        /// to a new value the old thumb will be disposed.
        /// </summary>
        public FreeImageBitmap SceneThumb
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

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
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
