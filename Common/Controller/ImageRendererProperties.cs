using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class defines how an image is rendered.
    /// </summary>
    public class ImageRendererProperties
    {
        public ImageRendererProperties()
        {
            Width = 100;
            Height = 100;
            AntiAliasingMode = 1;
            TransparentBackground = false;
            ShowWatermark = true;
            UseWindowBackgroundColor = true;
            CustomBackgroundColor = Color.Black;
            UseActiveViewportLocation = true;
            UseCustomPosition = false;
            CameraPosition = Vector3.Zero;
            CameraLookAt = Vector3.Zero;
            UseNavigationStatePosition = false;
            NavigationStateName = null;
            OverrideLayers = false;
            LayerState = null;
            ShowBackground = true;
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
        /// Use a navigation state as the position to render from. This will
        /// only get used if UseActiveViewportLocation is false. This will
        /// override UseCustomPosition so make sure this is false if that is set
        /// to true.
        /// </summary>
        public bool UseNavigationStatePosition { get; set; }

        /// <summary>
        /// The name of the navigation state to render from. Used by
        /// UseNavigationStatePosition.
        /// </summary>
        public String NavigationStateName { get; set; }

        /// <summary>
        /// Use the position defined by CameraPosition and CameraLookAt as the
        /// position to render from.
        /// </summary>
        public bool UseCustomPosition { get; set; }

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
        public String LayerState { get; set; }
    }
}
