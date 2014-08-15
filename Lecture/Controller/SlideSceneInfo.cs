using Engine.Saving;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class SlideSceneInfo : IDisposable
    {
        private SlideAction startupAction;
        private SceneThumbInfo sceneThumbInfo = null;

        public SlideSceneInfo()
        {

        }

        public SlideSceneInfo(Slide slide, SceneThumbInfo sceneThumbInfo)
        {
            startupAction = CopySaver.Default.copy(slide.StartupAction);
            this.sceneThumbInfo = sceneThumbInfo;
        }

        public void Dispose()
        {
            if (sceneThumbInfo != null)
            {
                sceneThumbInfo.Dispose();
                sceneThumbInfo = null;
            }
        }

        public void applyToSlide(Slide slide)
        {
            slide.StartupAction = CopySaver.Default.copy(startupAction);
        }

        /// <summary>
        /// Set the scene thumb info for this class. The object will be taken over by this class
        /// and will be disposed when it is or when the SceneThumbInfo is changed with this property.
        /// You should make copies of stuff you put in here (if applicable) and take out (if applicable).
        /// </summary>
        public SceneThumbInfo SceneThumbInfo
        {
            get
            {
                return sceneThumbInfo;
            }
            set
            {
                if (sceneThumbInfo != null)
                {
                    sceneThumbInfo.Dispose();
                }
                sceneThumbInfo = value;
            }
        }
    }
}
