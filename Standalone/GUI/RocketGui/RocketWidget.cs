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
    public class RocketWidget : IDisposable
    {
        private const String RTT_BASE_NAME = "__RocketRTT{0}{1}";
        private const int MAX_TEXTURE_SIZE_POW2 = 4096;
        private const int MIN_TEXTURE_SIZE_POW2 = 2;

        private SceneManager sceneManager;
        private Camera camera;
        private Viewport vp;
        private TexturePtr texture;
        private HardwarePixelBufferSharedPtr pixelBuffer;
        private int currentTextureWidth;
        private int currentTextureHeight;
        private String textureName;
        private String name;
        private byte textureRenameIndex = 0;

        private RocketRenderQueueListener renderQueueListener;
        private Context context;

        private ImageBox imageBox;

        public RocketWidget(String name, String doc, ImageBox imageBox)
        {
            this.imageBox = imageBox;
            this.name = name;
            generateTextureName();

            currentTextureWidth = computeSize(imageBox.Width);
            currentTextureHeight = computeSize(imageBox.Height);

            //Create ogre stuff
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, "__libRocketScene_" + name);
            camera = sceneManager.createCamera("libRocketCamera");

            texture = TextureManager.getInstance().createManual(textureName, "Rocket", TextureType.TEX_TYPE_2D, (uint)currentTextureWidth, (uint)currentTextureHeight, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);

            pixelBuffer = texture.Value.getBuffer();
            vp = pixelBuffer.Value.getRenderTarget().addViewport(camera);
            vp.setBackgroundColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
            vp.setOverlaysEnabled(false);
            vp.clear();

            //Create context
            context = Core.CreateContext(name, new Vector2i(imageBox.Width, imageBox.Height));
            //Debugger.Initialise(context);

            using (ElementDocument document = context.LoadDocument(doc))
            {
                if (document != null)
                {
                    document.Show();
                }
            }

            renderQueueListener = new RocketRenderQueueListener(context, (RenderInterfaceOgre3D)Core.GetRenderInterface());
            renderQueueListener.RenderDimensions = new IntSize2(currentTextureWidth, currentTextureHeight);
            sceneManager.addRenderQueueListener(renderQueueListener);

            imageBox.setImageTexture(textureName);
            imageBox.setImageCoord(new IntCoord(0, 0, imageBox.Width, imageBox.Height));
            
            imageBox.NeedKeyFocus = true;
            imageBox.NeedMouseFocus = true;

            imageBox.MouseButtonPressed += new MyGUIEvent(imageBox_MouseButtonPressed);
            imageBox.MouseButtonReleased += new MyGUIEvent(imageBox_MouseButtonReleased);
            imageBox.MouseMove += new MyGUIEvent(imageBox_MouseMove);
            imageBox.MouseDrag += new MyGUIEvent(imageBox_MouseMove);
            imageBox.MouseWheel += new MyGUIEvent(imageBox_MouseWheel);
            imageBox.KeyButtonPressed += new MyGUIEvent(imageBox_KeyButtonPressed);
            imageBox.KeyButtonReleased += new MyGUIEvent(imageBox_KeyButtonReleased);

            //In mygui lost/got focus is mouse entered / left
            imageBox.MouseLostFocus += new MyGUIEvent(imageBox_MouseLostFocus);
        }

        void imageBox_MouseLostFocus(Widget source, EventArgs e)
        {
            //Move the mouse offscreen to keep it from staying over stuff when the widget is not in focus
            context.ProcessMouseMove(-100, -100, 0);
        }

        public void changeDocument(String newDocumentName)
        {
            context.UnloadAllDocuments();
            using (ElementDocument document = context.LoadDocument(newDocumentName))
            {
                if (document != null)
                {
                    document.Show();
                }
            }
        }

        public void Dispose()
        {
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

        public void resized()
        {
            //Compute texture size
            int textureWidth = computeSize(imageBox.Width);
            int textureHeight = computeSize(imageBox.Height);

            if (textureWidth != currentTextureWidth || textureHeight != currentTextureHeight)
            {
                currentTextureWidth = textureWidth;
                currentTextureHeight = textureHeight;

                //Destroy old render target
                pixelBuffer.Value.getRenderTarget().destroyViewport(vp);
                pixelBuffer.Dispose();
                texture.Dispose();
                RenderManager.Instance.destroyTexture(textureName);

                generateTextureName();

                texture = TextureManager.getInstance().createManual(textureName, "Rocket", TextureType.TEX_TYPE_2D, (uint)textureWidth, (uint)textureHeight, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);

                pixelBuffer = texture.Value.getBuffer();
                vp = pixelBuffer.Value.getRenderTarget().addViewport(camera);
                vp.setBackgroundColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
                vp.setOverlaysEnabled(false);
                vp.clear();

                renderQueueListener.RenderDimensions = new IntSize2(textureWidth, textureHeight);
            }
            context.Dimensions = new Vector2i(imageBox.Width, imageBox.Height);
            imageBox.setImageInfo(textureName, new IntCoord(0, 0, imageBox.Width, imageBox.Height), new IntSize2(imageBox.Width, imageBox.Height));
        }

        public bool Enabled
        {
            get
            {
                return pixelBuffer.Value.getRenderTarget().isActive(); 
            }
            set
            {
                pixelBuffer.Value.getRenderTarget().setActive(value);
            }
        }

        void imageBox_MouseMove(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 mousePos = me.Position;
            mousePos.x -= imageBox.AbsoluteLeft;
            mousePos.y -= imageBox.AbsoluteTop;
            context.ProcessMouseMove(mousePos.x, mousePos.y, 0);
        }

        void imageBox_MouseWheel(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
			int wheelDelta = 0;
			if(me.RelativeWheelPosition > 0.0f)
			{
				wheelDelta = -1;
			}
			if(me.RelativeWheelPosition < 0.0f)
			{
				wheelDelta = 1;
			}
            context.ProcessMouseWheel(wheelDelta, 0);
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
            context.ProcessKeyUp(InputMap.GetKey(ke.Key), buildModifier());
        }

        void imageBox_KeyButtonPressed(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            KeyIdentifier key = InputMap.GetKey(ke.Key);
            char keyChar = ke.Char;

            context.ProcessKeyDown(key, buildModifier());

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

        int buildModifier()
        {
            int value = 0;
            if (InputManager.Instance.isShiftPressed())
            {
                value += (int)KeyModifier.KM_SHIFT;
            }
            if (InputManager.Instance.isControlPressed())
            {
                value += (int)KeyModifier.KM_CTRL;
            }
            return value;
        }

        //MyGUI caches the textures to determine size, this hack of renaming the texture when it is remade
        //gets us around that problem
        private void generateTextureName()
        {
            textureName = String.Format(RTT_BASE_NAME, name, textureRenameIndex++);
        }

        private static int computeSize(int dimension)
        {
            if (dimension > MAX_TEXTURE_SIZE_POW2)
            {
                return MAX_TEXTURE_SIZE_POW2;
            }
            else if (dimension < MIN_TEXTURE_SIZE_POW2)
            {
                return MIN_TEXTURE_SIZE_POW2;
            }
            else
            {
                return NumberFunctions.computeClosestLargerPow2(dimension, 256);
            }
        }
    }
}
