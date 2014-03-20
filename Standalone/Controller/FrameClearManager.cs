using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    /// <summary>
    /// This class provides a way for the program to clear its entire frame buffer each frame. Due to the way ogre
    /// works, it must have a viewport that covers the screen completely in order to clear it effectivly. We have to
    /// clear using this viewport in order for the scene to not flicker when rendering images. This is likely symptomatic
    /// of a larger problem, but doing our clears this way solves the problem.
    /// 
    /// The primary cause of this issue is the result of a discrepency between opengl and d3d11. In d3d11 clearing anything
    /// causes the entire screen to clear, in opengl it will clear only the active viewport. By clearing with the method done
    /// in this class we can ensure that both render systems work the same way.
    /// </summary>
    class FrameClearManager : IDisposable
    {
        private RenderTarget renderTarget;
        private SceneManager sceneManager;
        private Camera camera;
        private Viewport viewport;

        public FrameClearManager(RenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
            renderTarget.PreRenderTargetUpdate += OgreRenderWindow_PreRenderTargetUpdate;
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC);
            camera = sceneManager.createCamera("ClearCamera");
            viewport = renderTarget.addViewport(camera, 999, 0.0f, 0.0f, 1.0f, 1.0f);
            viewport.AutoUpdated = false;
        }

        public void Dispose()
        {
            renderTarget.destroyViewport(viewport);
            sceneManager.destroyCamera(camera);
            Root.getSingleton().destroySceneManager(sceneManager);
            renderTarget.PreRenderTargetUpdate -= OgreRenderWindow_PreRenderTargetUpdate;
        }

        void OgreRenderWindow_PreRenderTargetUpdate()
        {
            viewport.clear(FrameBufferType.FBT_COLOUR | FrameBufferType.FBT_DEPTH | FrameBufferType.FBT_STENCIL, Engine.Color.Black);
        }
    }
}
