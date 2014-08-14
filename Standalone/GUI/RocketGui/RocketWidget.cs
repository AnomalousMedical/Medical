using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using libRocketPlugin;
using Engine;
using Engine.Platform;
using FreeImageAPI;

namespace Medical.GUI
{
    public class RocketWidget : IDisposable
    {
        private const FreeImageAPI.PixelFormat BitmapFormat = FreeImageAPI.PixelFormat.Format32bppArgb;
        private static readonly Engine.Color ClearColor = new Engine.Color(0.0f, 0.0f, 0.0f, 0.0f);

        private const String RTT_BASE_NAME = "__RocketRTT{0}_{1}";
        private const int MAX_TEXTURE_SIZE_POW2 = 4096;
        private const int MIN_TEXTURE_SIZE_POW2 = 2;

        private SceneManager sceneManager;
        private Camera camera;
        private Viewport vp;
        private TexturePtr texture;
        private HardwarePixelBufferSharedPtr pixelBuffer;
        private RenderTexture renderTexture;
        private int currentTextureWidth;
        private int currentTextureHeight;
        private String textureName;
        private String name;
        private byte textureRenameIndex = 0;
        private OgreWrapper.PixelFormat ogreTextureFormat = OgreWrapper.PixelFormat.PF_X8R8G8B8;

        private RocketRenderQueueListener renderQueueListener;
        private Context context;

        private ImageBox imageBox;

        //Conditions to turn on rendering
        bool renderingEnabled = true; //This one has to be true to use the others

        //Any one (or combo) of these has to be on
        bool hasKeyFocus = false;
        bool mouseOver = false;
        bool renderOneFrame = true;
        bool alwaysRender = false;

        public RocketWidget(ImageBox imageBox, bool transparent)
        {
            this.imageBox = imageBox;
            if (transparent)
            {
                ogreTextureFormat = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            }
            this.name = RocketWidgetManager.generateRocketWidgetName(this);
            generateTextureName();

            currentTextureWidth = computeSize(imageBox.Width);
            currentTextureHeight = computeSize(imageBox.Height);

            //Create ogre stuff
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, "__libRocketScene_" + name);
            camera = sceneManager.createCamera("libRocketCamera");

            texture = TextureManager.getInstance().createManual(textureName, RocketInterface.Instance.CommonResourceGroup.FullName, TextureType.TEX_TYPE_2D, (uint)currentTextureWidth, (uint)currentTextureHeight, 1, 1, ogreTextureFormat, TextureUsage.TU_RENDERTARGET, false, 0);

            pixelBuffer = texture.Value.getBuffer();
            renderTexture = pixelBuffer.Value.getRenderTarget();
            vp = renderTexture.addViewport(camera);
            vp.setBackgroundColor(ClearColor);
            vp.setOverlaysEnabled(false);
            vp.clear();

            //Create context
            context = Core.CreateContext(name, new Vector2i(imageBox.Width, imageBox.Height));

            renderQueueListener = new RocketRenderQueueListener(context, (RenderInterfaceOgre3D)Core.GetRenderInterface(), renderTexture.requiresTextureFlipping());
            renderQueueListener.FrameCompleted += new Action(renderQueueListener_FrameCompleted);
            renderQueueListener.RenderDimensions = new IntSize2(currentTextureWidth, currentTextureHeight);
            sceneManager.addRenderQueueListener(renderQueueListener);

            imageBox.setImageTexture(textureName);
            imageBox.setImageCoord(new IntCoord(0, 0, imageBox.Width, imageBox.Height));

            imageBox.NeedKeyFocus = true;
            imageBox.NeedMouseFocus = true;

            imageBox.MouseButtonPressed += imageBox_MouseButtonPressed;
            imageBox.MouseButtonReleased += imageBox_MouseButtonReleased;
            imageBox.MouseMove += imageBox_MouseMove;
            imageBox.MouseDrag += imageBox_MouseMove;
            imageBox.MouseWheel += imageBox_MouseWheel;
            imageBox.KeyButtonPressed += imageBox_KeyButtonPressed;
            imageBox.KeyButtonReleased += imageBox_KeyButtonReleased;
            imageBox.EventScrollGesture += imageBox_EventScrollGesture;

            //In mygui lost/got focus is mouse entered / left
            imageBox.MouseSetFocus += imageBox_MouseSetFocus;
            imageBox.MouseLostFocus += imageBox_MouseLostFocus;
            imageBox.RootKeyChangeFocus += imageBox_RootKeyChangeFocus;

            determineRenderingActive();
        }

        public void Dispose()
        {
            RocketWidgetManager.rocketWidgetDisposed(name);
            imageBox.MouseButtonPressed -= imageBox_MouseButtonPressed;
            imageBox.MouseButtonReleased -= imageBox_MouseButtonReleased;
            imageBox.MouseMove -= imageBox_MouseMove;
            imageBox.MouseDrag -= imageBox_MouseMove;
            imageBox.MouseWheel -= imageBox_MouseWheel;
            imageBox.KeyButtonPressed -= imageBox_KeyButtonPressed;
            imageBox.KeyButtonReleased -= imageBox_KeyButtonReleased;
            imageBox.EventScrollGesture -= imageBox_EventScrollGesture;

            //In mygui lost/got focus is mouse entered / left
            imageBox.MouseSetFocus -= imageBox_MouseSetFocus;
            imageBox.MouseLostFocus -= imageBox_MouseLostFocus;
            imageBox.RootKeyChangeFocus -= imageBox_RootKeyChangeFocus;

            if (context != null)
            {
                context.Dispose();
            }
            if (vp != null)
            {
                renderTexture.destroyViewport(vp);
            }
            if (pixelBuffer != null)
            {
                pixelBuffer.Dispose();
            }
            if (texture != null)
            {
                texture.Dispose();
                RenderManager.Instance.destroyTexture(textureName);
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
            //Only resize if enabled.
            if (renderingEnabled)
            {
                //Compute texture size
                int textureWidth = computeSize(imageBox.Width);
                int textureHeight = computeSize(imageBox.Height);

                if (textureWidth != currentTextureWidth || textureHeight != currentTextureHeight)
                {
                    currentTextureWidth = textureWidth;
                    currentTextureHeight = textureHeight;

                    //Destroy old render target
                    renderTexture.destroyViewport(vp);
                    pixelBuffer.Dispose();
                    texture.Dispose();
                    RenderManager.Instance.destroyTexture(textureName);

                    generateTextureName();

                    texture = TextureManager.getInstance().createManual(textureName, RocketInterface.Instance.CommonResourceGroup.FullName, TextureType.TEX_TYPE_2D, (uint)textureWidth, (uint)textureHeight, 1, 1, ogreTextureFormat, TextureUsage.TU_RENDERTARGET, false, 0);

                    pixelBuffer = texture.Value.getBuffer();
                    renderTexture = pixelBuffer.Value.getRenderTarget();
                    vp = renderTexture.addViewport(camera);
                    vp.setBackgroundColor(ClearColor);
                    vp.setOverlaysEnabled(false);
                    vp.clear();

                    renderQueueListener.RenderDimensions = new IntSize2(textureWidth, textureHeight);
                }

                int imageWidth = imageBox.Width;
                if (imageWidth < 0)
                {
                    imageWidth = 0;
                }
                int imageHeight = imageBox.Height;
                if (imageHeight < 0)
                {
                    imageHeight = 0;
                }
                context.Dimensions = new Vector2i(imageWidth, imageHeight);
                imageBox.setImageInfo(textureName, new IntCoord(0, 0, imageWidth, imageHeight), new IntSize2(imageWidth, imageHeight));
                renderOneFrame = true;
                determineRenderingActive();
            }
        }

        public void removeFocus()
        {
            Element element = context.GetFocusElement();
            if (element != null)
            {
                element.Blur();
                renderOnNextFrame();
            }
        }

        public void setScale(float newScale)
        {
            if (Context.ZoomLevel != newScale)
            {
                Context.ZoomLevel = newScale;
                foreach (ElementDocument document in Context.Documents)
                {
                    document.MakeDirtyForScaleChange();
                }
                renderOnNextFrame();
            }
        }

        /// <summary>
        /// Write to the given graphics, if the widget texture is not large enough it will be
        /// resized temporarily and then sized back. So be careful with large destrects.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="destRect"></param>
        public void writeToGraphics(FreeImageBitmap g, Rectangle destRect)
        {
            bool changedSize = false;
            IntSize2 originalSize = new IntSize2(imageBox.Width, imageBox.Height);
            float cropRatio = (float)imageBox.Width / destRect.Width;
            Rectangle srcRect = new Rectangle(0, 0, imageBox.Width, (int)(destRect.Height * cropRatio));

            //Make sure the source image is large enough, if not resize.
            if (originalSize.Width < srcRect.Width || originalSize.Height < srcRect.Height)
            {
                imageBox.setSize(srcRect.Width, srcRect.Height);
                changedSize = true;
                resized();
                renderTexture.update();
            }
            else if (renderOneFrame)
            {
                renderTexture.update();
            }

            using (FreeImageBitmap fullBitmap = new FreeImageBitmap(currentTextureWidth, currentTextureHeight, BitmapFormat))
            {
                fullBitmap.copyFromRenderTarget(renderTexture, ogreTextureFormat);
                //Remove alpha
                //BitmapDataExtensions.SetAlpha(bmpData, 255);

                if (srcRect.Height > fullBitmap.Height)
                {
                    srcRect.Height = fullBitmap.Height;
                }

                using (FreeImageBitmap cropped = fullBitmap.Copy(srcRect))
                {
                    cropped.Rescale(destRect.Width, destRect.Height, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                    g.Paste(cropped, destRect.X, destRect.Y, int.MaxValue);
                }
            }

            if (changedSize)
            {
                imageBox.setSize(originalSize.Width, originalSize.Height);
                resized();
                renderTexture.update();
            }
        }

        internal void setFocus()
        {
            InputManager.Instance.setKeyFocusWidget(imageBox);
        }

        /// <summary>
        /// The RocketWidget attempts to only render when it needs to (such as
        /// the mouse being over it or having focus. If it is in the background
        /// it will just use the last rendered frame. However, sometimes this
        /// can cause old data to be shown, such as if a document was loaded
        /// when the widget did not have focus. In that case call this function
        /// to cause the widget to render on the next frame.
        /// </summary>
        public void renderOnNextFrame()
        {
            renderOneFrame = true;
            determineRenderingActive();
        }

        /// <summary>
        /// This can be set to true to override the attempt to only render the
        /// RocketWidget when it needs updates and force it to render every
        /// frame. This is still overridden by Enabled.
        /// </summary>
        public bool AlwaysRender
        {
            get
            {
                return alwaysRender;
            }
            set
            {
                alwaysRender = value;
                determineRenderingActive();
            }
        }

        public bool InputEnabled
        {
            get
            {
                return imageBox.Enabled;
            }
            set
            {
                imageBox.Enabled = value;
            }
        }

        /// <summary>
        /// This is the master switch to enable / disable rendering. If this is
        /// false the widget will never render. If it is true if will render
        /// depending on the other conditions.
        /// </summary>
        public bool RenderingEnabled
        {
            get
            {
                return renderingEnabled;
            }
            set
            {
                renderingEnabled = value;
                if (renderingEnabled)
                {
                    renderOneFrame = true;
                }
                determineRenderingActive();
            }
        }

        public Context Context
        {
            get
            {
                return context;
            }
        }

        public int AbsoluteLeft
        {
            get
            {
                return imageBox.AbsoluteLeft;
            }
        }

        public int AbsoluteTop
        {
            get
            {
                return imageBox.AbsoluteTop;
            }
        }

        void imageBox_MouseMove(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mousePos = me.Position;
                mousePos.x -= imageBox.AbsoluteLeft;
                mousePos.y -= imageBox.AbsoluteTop;
                context.ProcessMouseMove(mousePos.x, mousePos.y, 0);
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_MouseWheel(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                MouseEventArgs me = (MouseEventArgs)e;
                int wheelDelta = 0;
                if (me.RelativeWheelPosition > 0.0f)
                {
                    wheelDelta = -1;
                }
                if (me.RelativeWheelPosition < 0.0f)
                {
                    wheelDelta = 1;
                }
                context.ProcessMouseWheel(wheelDelta, 0);
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_MouseButtonReleased(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                MouseEventArgs me = (MouseEventArgs)e;
                context.ProcessMouseButtonUp((int)me.Button, 0);
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_MouseButtonPressed(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                MouseEventArgs me = (MouseEventArgs)e;
                context.ProcessMouseButtonDown((int)me.Button, 0);
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_KeyButtonReleased(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                KeyEventArgs ke = (KeyEventArgs)e;
                context.ProcessKeyUp(InputMap.GetKey(ke.Key), buildModifier());
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_KeyButtonPressed(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
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
                    case KeyIdentifier.KI_LEFT:
                        keyChar = (char)0;
                        break;
                    case KeyIdentifier.KI_RIGHT:
                        keyChar = (char)0;
                        break;
                    case KeyIdentifier.KI_UP:
                        keyChar = (char)0;
                        break;
                    case KeyIdentifier.KI_DOWN:
                        keyChar = (char)0;
                        break;
                    case KeyIdentifier.KI_BACK:
                        keyChar = (char)0;
                        break;
                }

                if (keyChar >= 32)
                {
                    context.ProcessTextInput(keyChar);
                }
            }
            finally
            {
                context.removeReference();
            }
        }

        void imageBox_EventScrollGesture(Widget source, EventArgs e)
        {
            context.addReference(); //Keep context alive even if this widget is disposed during this event
            try
            {
                Element element = context.GetFocusElement();
                //Find the parent element that scrolls
                while (element != null && element.ScrollHeight <= element.ClientHeight && element.ScrollWidth <= element.ClientWidth)
                {
                    element = element.ParentNode;
                }
                if (element != null)
                {
                    ScrollGestureEventArgs sgea = (ScrollGestureEventArgs)e;
                    element.ScrollLeft -= sgea.DeltaX;
                    element.ScrollTop -= sgea.DeltaY;
                }
            }
            finally
            {
                context.removeReference();
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

        void imageBox_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs fe = (RootFocusEventArgs)e;
            if (!fe.Focus)
            {
                renderOneFrame = true;
                removeFocus();
            }
            hasKeyFocus = fe.Focus;
            determineRenderingActive();
        }

        void imageBox_MouseSetFocus(Widget source, EventArgs e)
        {
            mouseOver = true;
            determineRenderingActive();
        }

        void imageBox_MouseLostFocus(Widget source, EventArgs e)
        {
            //Move the mouse offscreen to keep it from staying over stuff when the widget is not in focus
            context.ProcessMouseMove(-100, -100, 0);
            mouseOver = false;
            renderOneFrame = true;
            determineRenderingActive();
        }

        void renderQueueListener_FrameCompleted()
        {
            if (renderOneFrame)
            {
                renderOneFrame = false;
                determineRenderingActive();
            }
        }

        void determineRenderingActive()
        {
            renderTexture.setAutoUpdated(renderingEnabled && (alwaysRender || hasKeyFocus || mouseOver || renderOneFrame));
        }
    }
}
