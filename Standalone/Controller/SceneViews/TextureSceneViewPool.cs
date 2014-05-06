using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    public class PooledSceneView : PooledObject
    {
        public void finished()
        {
            this.returnToPool();
        }

        protected override void reset()
        {
            
        }

        public TextureSceneView SceneView { get; set; }
    }

    public class TextureSceneViewPool : IDisposable
    {
        private SceneViewController sceneViewController;
        private LifecycleObjectPool<PooledSceneView> pool;
        private IntSize2 size;
        private String baseName;

        public event Action<PooledSceneView> SceneViewDestroyed;

        public TextureSceneViewPool(SceneViewController sceneViewController, String baseName, IntSize2 size)
        {
            pool = new LifecycleObjectPool<PooledSceneView>(createSceneViewWrapper, destroySceneViewWrapper);
            this.sceneViewController = sceneViewController;
            this.size = size;
            this.baseName = baseName;
        }

        public void Dispose()
        {
            pool.Dispose();
        }

        public PooledSceneView getSceneView(Vector3 translation, Vector3 lookAt, LayerState layers)
        {
            PooledSceneView pooledView = pool.getPooledObject();
            if(pooledView.SceneView == null)
            {
                pooledView.SceneView = sceneViewController.createTextureSceneView(baseName + Guid.NewGuid().ToString(), translation, lookAt, size.Width, size.Height);
            }
            else
            {
                pooledView.SceneView.immediatlySetPosition(new CameraPosition()
                {
                    Translation = translation,
                    LookAt = lookAt,
                    UseIncludePoint = false
                });
            }

            layers.instantlyApplyTo(pooledView.SceneView.CurrentTransparencyState);

            return pooledView;
        }

        public int? MaxPoolSize
        {
            get
            {
                return pool.MaxPoolSize;
            }
            set
            {
                pool.MaxPoolSize = value;
            }
        }

        private PooledSceneView createSceneViewWrapper()
        {
            return new PooledSceneView();
        }

        void destroySceneViewWrapper(PooledSceneView pooledView)
        {
            if(SceneViewDestroyed != null)
            {
                SceneViewDestroyed.Invoke(pooledView);
            }
            sceneViewController.destroyWindow(pooledView.SceneView);
        }
    }
}
