using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using OgreWrapper;

namespace Medical
{
    /// <summary>
    /// This class defines how an image is rendered.
    /// </summary>
    public class ImageRendererProperties
    {
        public delegate void CustomizeCameraPositionDelegate(Camera camera, Viewport viewport); 

        public ImageRendererProperties()
        {
            Width = 100;
            Height = 100;
            MaxGridWidth = 2048;
            MaxGridHeight = 2048;
            AntiAliasingMode = 1;
            TransparentBackground = false;
            ShowWatermark = true;
            UseWindowBackgroundColor = true;
            CustomBackgroundColor = Color.Black;
            UseActiveViewportLocation = true;
            CameraPosition = Vector3.Zero;
            CameraLookAt = Vector3.Zero;
            OverrideLayers = false;
            LayerState = null;
            ShowBackground = true;
            ShowUIUpdates = true;
            UseIncludePoint = false;
            IncludePoint = Vector3.Zero;
            CustomizeCameraPosition = null;
        }

        /// <summary>
        /// The width of the final image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the final image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The maximum width one element of the grid can be.
        /// </summary>
        public int MaxGridWidth { get; set; }

        /// <summary>
        /// The maximum height one element of the grid can be.
        /// </summary>
        public int MaxGridHeight { get; set; }

        /// <summary>
        /// Compute the number of tiles in the grid.
        /// </summary>
        public int NumGridTiles
        {
            get
            {
                int largeWidth = Width * AntiAliasingMode;
                int largeHeight = Height * AntiAliasingMode;
                int widthCompute = largeWidth % MaxGridWidth == 0 ? largeWidth / MaxGridWidth : largeWidth / MaxGridWidth + 1;
                int heightCompute = largeHeight % MaxGridHeight == 0 ? largeHeight / MaxGridHeight : largeHeight / MaxGridHeight + 1;
                if (widthCompute > heightCompute)
                {
                    return widthCompute;
                }
                return heightCompute;
            }
        }

        /// <summary>
        /// A scalar applied to the width and height to achieve some anti
        /// aliasing. This is done by multiplying the width and height by this
        /// number and then scaling the image back down to the desired size
        /// before it is returned. This will effectivly create a smoothing
        /// effect, but larger numbers will require more time and memory to
        /// process.
        /// </summary>
        public int AntiAliasingMode { get; set; }

        /// <summary>
        /// If this is true the color of the background of the image will be
        /// replaced with alpha.
        /// </summary>
        public bool TransparentBackground { get; set; }

        /// <summary>
        /// If this is true any watermark assigned to the ImageRenderer will be
        /// used.
        /// </summary>
        public bool ShowWatermark { get; set; }

        /// <summary>
        /// If this is true any background assigned to the ImageRenderer will be
        /// used.
        /// </summary>
        public bool ShowBackground { get; set; }

        /// <summary>
        /// If this is true the window color of the active drawing window will
        /// be used as the backgound color. If this is false the color defined
        /// by CustomBackgroundColor will be used.
        /// </summary>
        public bool UseWindowBackgroundColor { get; set; }

        /// <summary>
        /// The custom background color to use if UseWindowBackgroundColor is
        /// false.
        /// </summary>
        public Color CustomBackgroundColor { get; set; }

        /// <summary>
        /// Use the location of the camera in the active viewport as the
        /// location to render from. If this is true it will override the
        /// setting for UseCustomPostion and UseNavigationStatePosition. So this
        /// must be set to false if one of those is going to be used.
        /// </summary>
        public bool UseActiveViewportLocation { get; set; }

        /// <summary>
        /// The positon of the camera for UseCustomPosition.
        /// </summary>
        public Vector3 CameraPosition { get; set; }

        /// <summary>
        /// The look at position for UseCustomPosition.
        /// </summary>
        public Vector3 CameraLookAt { get; set; }

        /// <summary>
        /// If this is true the layers will be set to the given layer state.
        /// </summary>
        public bool OverrideLayers { get; set; }

        /// <summary>
        /// The layer state to use with OverrideLayers.
        /// </summary>
        public LayerState LayerState { get; set; }

        /// <summary>
        /// Set this to true to enable the ImageRendererProgress and show updates. Set it to false to keep this UI hidden.
        /// </summary>
        public bool ShowUIUpdates { get; set; }

        /// <summary>
        /// Set to true to use an include point to calculate the camera position
        /// </summary>
        public bool UseIncludePoint { get; set; }

        /// <summary>
        /// Set the include point you wish to use.
        /// </summary>
        public Vector3 IncludePoint { get; set; }

        /// <summary>
        /// Callback to customize or further manipulate the camera, this is called after
        /// the include point has been corrected for if one is set.
        /// </summary>
        public CustomizeCameraPositionDelegate CustomizeCameraPosition { get; set; }
    }
}
