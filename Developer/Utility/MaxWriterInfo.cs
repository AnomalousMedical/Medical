using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developer
{
    class MaxWriterInfo
    {
        public MaxWriterInfo()
        {

        }

        public MaxWriterInfo(SimObject simObject)
        {
            Name = simObject.Name;
            MeshName = FindMeshName(simObject);
            Translation = EngineTransToMax(simObject.Translation);
            Rotation = EngineRotToMax(simObject.Rotation);
        }

        /// <summary>
        /// The name of the object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The name of the mesh.
        /// </summary>
        public String MeshName { get; set; }

        /// <summary>
        /// The translation in 3ds max coords, use the helper to convert.
        /// </summary>
        public Vector3 Translation { get; set; }

        /// <summary>
        /// The rotation in 3ds max coords, use the helper to convert.
        /// </summary>
        public Vector3 Rotation { get; set; }

        public static Vector3 EngineTransToMax(Vector3 trans)
        {
            return new Vector3(trans.x,
                               trans.z * -1.0f,
                               trans.y);
        }

        public static Vector3 EngineRotToMax(Quaternion rot)
        {
            Vector3 euler = rot.getEuler();
            return new Vector3((Degree)new Radian(euler.z),
                               (Degree)new Radian(euler.y * -1.0f),
                               (Degree)new Radian(euler.x));
        }

        public static String FindMeshName(SimObject simObject)
        {
            String name = null;
            foreach (var element in simObject.getElementIter())
            {
                SceneNodeElement sceneElement = element as SceneNodeElement;
                if (sceneElement != null)
                {
                    foreach (var movable in sceneElement.MovableObjects)
                    {
                        Entity entity = movable as Entity;
                        if (entity != null)
                        {
                            using (MeshPtr mesh = entity.getMesh())
                            {
                                String meshName = mesh.Value.getName();
                                if (meshName != null)
                                {
                                    name = Path.GetFileNameWithoutExtension(meshName);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return name;
        }
    }
}
