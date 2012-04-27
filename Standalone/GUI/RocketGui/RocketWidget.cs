using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using libRocketPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class RocketWidget : IDisposable
    {
        private SceneManager sceneManager;
        private Camera camera;
        private Viewport vp;
        private TexturePtr texture;
        private HardwarePixelBufferSharedPtr pixelBuffer;

        private Context context;

        private ImageBox imageBox;

        public RocketWidget(String name, EventManager eventManager)
        {
            int width = 1024;
            int height = 1024;

            //Create ogre stuff
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, "__libRocketScene_" + name);
            camera = sceneManager.createCamera("libRocketCamera");

            texture = TextureManager.getInstance().createManual("__RocketRTT", "Rocket", TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);

            pixelBuffer = texture.Value.getBuffer();
            vp = pixelBuffer.Value.getRenderTarget().addViewport(camera);
            vp.setBackgroundColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
            vp.setOverlaysEnabled(false);
            vp.clear();

            //Create context
            context = Core.CreateContext(name, new Vector2i(width, height));

            using (ElementDocument document = context.LoadDocument("assets/demo.rml"))
            {
                if (document != null)
                {
                    document.Show();
                }
            }

            sceneManager.addRenderQueueListener(new RocketRenderQueueListener(context, (RenderInterfaceOgre3D)Core.GetRenderInterface()));

            imageBox = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, width, height, MyGUIPlugin.Align.Default, "Overlapped", name);
            imageBox.setImageTexture("__RocketRTT");
            imageBox.NeedKeyFocus = true;
            imageBox.NeedMouseFocus = true;

            imageBox.MouseButtonPressed += new MyGUIEvent(imageBox_MouseButtonPressed);
            imageBox.MouseButtonReleased += new MyGUIEvent(imageBox_MouseButtonReleased);
            imageBox.MouseMove += new MyGUIEvent(imageBox_MouseMove);
            imageBox.MouseDrag += new MyGUIEvent(imageBox_MouseDrag);
            imageBox.MouseWheel += new MyGUIEvent(imageBox_MouseWheel);
            imageBox.KeyButtonPressed += new MyGUIEvent(imageBox_KeyButtonPressed);
            imageBox.KeyButtonReleased += new MyGUIEvent(imageBox_KeyButtonReleased);
        }

        public void Dispose()
        {
            if (imageBox != null)
            {
                Gui.Instance.destroyWidget(imageBox);
            }
            if (context != null)
            {
                context.Dispose();
            }
            if (vp != null)
            {
                pixelBuffer.Value.getRenderTarget().destroyViewport(vp);
            }
            if (pixelBuffer != null)
            {
                pixelBuffer.Dispose();
            }
            if (texture != null)
            {
                texture.Dispose();
            }
            if (camera != null)
            {
                sceneManager.destroyCamera(camera);
            }
            if (sceneManager != null)
            {
                Root.getSingleton().destroySceneManager(sceneManager);
            }
        }

        void imageBox_MouseMove(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            context.ProcessMouseMove(me.Position.x, me.Position.y, 0);
        }

        void imageBox_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            context.ProcessMouseMove(me.Position.x, me.Position.y, 0);
        }

        void imageBox_MouseWheel(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            context.ProcessMouseWheel(me.RelativeWheelPosition / -120, 0);
        }

        void imageBox_MouseButtonReleased(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            context.ProcessMouseButtonUp((int)me.Button, 0);
        }

        void imageBox_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            context.ProcessMouseButtonDown((int)me.Button, 0);
        }

        void imageBox_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            context.ProcessKeyUp(InputMap.GetKey(ke.Key), 0);
        }

        void imageBox_KeyButtonPressed(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            KeyIdentifier key = InputMap.GetKey(ke.Key);
            char keyChar = ke.Char;

            context.ProcessKeyDown(key, 0);

            switch (key)
            {
                case KeyIdentifier.KI_DELETE:
                    keyChar = (char)0;
                    break;
                case KeyIdentifier.KI_RETURN:
                    keyChar = (char)0;
                    context.ProcessTextInput('\n');
                    break;
            }
            if (keyChar >= 32)
            {
                context.ProcessTextInput(keyChar);
            }
        }
    }
}
