using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    internal class LiveThumbnailHostInfo
    {
        public LiveThumbnailHostInfo()
        {
            Visible = false;
        }

        public LayerState Layers { get; set; }

        public Vector3 Translation { get; set; }

        public Vector3 LookAt { get; set; }

        public bool Visible { get; set; }

        public PooledSceneView CurrentSceneView { get; set; }

        public SceneViewWindowEvent WindowCreatedCallback { get; set; }

        public LiveThumbnailHost Host { get; set; }
    }
}
