using Anomalous.GuiFramework.Cameras;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class TextureSceneViewPoolExtensions
    {
        public static PooledSceneView getSceneView(this TextureSceneViewPool pool, Vector3 translation, Vector3 lookAt, LayerState layers)
        {
            PooledSceneView pooledView = pool.getSceneView(translation, lookAt);

            layers.instantlyApplyTo(pooledView.SceneView.CurrentTransparencyState);

            return pooledView;
        }
    }
}
