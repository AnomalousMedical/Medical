using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Saving;
using Engine.Renderer;
using Engine.Attributes;
using BulletPlugin;

namespace Medical
{
    class ToothSectionEditRenderer : EditInterfaceRenderer
    {
        private ToothSection section;

        public ToothSectionEditRenderer(ToothSection section)
        {
            this.section = section;
        }

        public void frameUpdate(DebugDrawingSurface drawingSurface)
        {

        }

        public void interfaceDeselected(DebugDrawingSurface drawingSurface)
        {

        }

        public void interfaceSelected(DebugDrawingSurface drawingSurface)
        {
            drawBoundingBox(drawingSurface);
        }

        public void propertiesChanged(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {
            drawBoundingBox(drawingSurface);
        }

        private void drawBoundingBox(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {
            drawingSurface.begin(String.Format("ToothSection{0}", section.Name), Engine.Renderer.DrawingType.LineList);

            //Origin
            drawingSurface.setColor(Color.Red);
            drawingSurface.drawLine(section.BoundsOrigin, section.BoundsOrigin + Vector3.Right * 0.1f);
            drawingSurface.setColor(Color.Blue);
            drawingSurface.drawLine(section.BoundsOrigin, section.BoundsOrigin + Vector3.Up * 0.1f);
            drawingSurface.setColor(Color.Green);
            drawingSurface.drawLine(section.BoundsOrigin, section.BoundsOrigin + Vector3.Backward * 0.1f);

            drawingSurface.setColor(Color.Red);

            Vector3[] vertices = new Vector3[8];
            section._getBoundsVertices(vertices);

            //-Z ccw
            drawingSurface.drawLine(vertices[0], vertices[1]);
            drawingSurface.drawLine(vertices[1], vertices[2]);
            drawingSurface.drawLine(vertices[2], vertices[3]);
            drawingSurface.drawLine(vertices[3], vertices[0]);

            //+z ccw
            drawingSurface.drawLine(vertices[4], vertices[5]);
            drawingSurface.drawLine(vertices[5], vertices[6]);
            drawingSurface.drawLine(vertices[6], vertices[7]);
            drawingSurface.drawLine(vertices[7], vertices[4]);

            //Sides
            drawingSurface.drawLine(vertices[0], vertices[4]);
            drawingSurface.drawLine(vertices[1], vertices[5]);
            drawingSurface.drawLine(vertices[2], vertices[6]);
            drawingSurface.drawLine(vertices[3], vertices[7]);

            drawingSurface.end();
        }
    }

    class ToothSection : BehaviorObject
    {
        private String name;

        [DoNotCopy]
        [DoNotSave]
        private Box3 boundingBox;

        [DoNotCopy]
        [DoNotSave]
        private uint[] indexBuffer; //The buffer of indices to include in this section

        [Editable]
        uint decompDepth = 2;

        [Editable]
        int concavityThreshold = 5;

        [Editable]
        int volumeConservationThreshold = 15;

        [Editable]
        uint maxVertices = 16;

        [Editable]
        float skinWidth = 0.0f;

        /// <summary>
        /// Constructor for copying, do not call this directly.
        /// </summary>
        public ToothSection()
        {
            boundingBox = new Box3();
            Vector3[] axes = boundingBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Backward;
        }

        public ToothSection(String name)
        {
            this.name = name;
            boundingBox = new Box3();
            Vector3[] axes = boundingBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Backward;
            boundingBox.setExtents(new Vector3(2, 2, 2));
        }

        public void checkTriangles(Vector3[] vertices, uint[] indices)
        {
            int numIndices = 0;
            uint[] tempTriangleBuffer = new uint[indices.Length];
            for (int i = 0; i < indices.Length; i += 3)
            {
                if (boundingBox.isInside(vertices[indices[i]]) && boundingBox.isInside(vertices[indices[i + 1]]) && boundingBox.isInside(vertices[indices[i + 2]]))
                {
                    tempTriangleBuffer[numIndices++] = indices[i];
                    tempTriangleBuffer[numIndices++] = indices[i + 1];
                    tempTriangleBuffer[numIndices++] = indices[i + 2];
                }
            }
            //Copy the results into indexBuffer
            indexBuffer = new uint[numIndices];
            Buffer.BlockCopy(tempTriangleBuffer, 0, indexBuffer, 0, sizeof(uint) * numIndices);
        }

        public unsafe void createSection(Vector3[] vertices, ReshapeableRigidBody body)
        {
            fixed (float* verts = &vertices[0].x)
            {
                fixed (uint* idxs = &indexBuffer[0])
                {
                    ConvexDecompositionDesc decompDesc = new ConvexDecompositionDesc();
                    decompDesc.mVcount = (uint)vertices.Length;
                    decompDesc.mVertices = verts;
                    decompDesc.mTcount = (uint)indexBuffer.Length / 3;
                    decompDesc.mIndices = idxs;
                    decompDesc.mDepth = decompDepth;
                    decompDesc.mCpercent = concavityThreshold;
                    decompDesc.mPpercent = volumeConservationThreshold;
                    decompDesc.mMaxVertices = maxVertices;
                    decompDesc.mSkinWidth = skinWidth;

                    //temp
                    if (decompDesc.mDepth == 6 && decompDesc.mCpercent == 0)
                    {
                        decompDesc.mDepth = 3;
                    }
                    //endtemp

                    body.createHullRegion(name, decompDesc);
                }
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.Renderer = new ToothSectionEditRenderer(this);
        }

        /// <summary>
        /// Get the vertices of the bounding box, used to render in the editor.
        /// </summary>
        /// <param name="verts"></param>
        internal void _getBoundsVertices(Vector3[] verts)
        {
            boundingBox.computeVertices(verts);
        }

        public void drawBoundsWorld(DebugDrawingSurface drawingSurface, Vector3 worldTrans, Quaternion worldRot)
        {
            Box3 worldBox = new Box3();
            worldBox.setCenter(Quaternion.quatRotate(worldRot, boundingBox.getCenter()) + worldTrans);
            worldBox.setExtents(boundingBox.getExtents());
            Vector3[] worldAxis = worldBox.getAxes();
            Vector3[] boundAxis = boundingBox.getAxes();
            worldAxis[0] = Quaternion.quatRotate(worldRot, boundAxis[0]);
            worldAxis[1] = Quaternion.quatRotate(worldRot, boundAxis[1]);
            worldAxis[2] = Quaternion.quatRotate(worldRot, boundAxis[2]);

            Vector3 origin = worldBox.getCenter();

            //Origin
            drawingSurface.setColor(Color.Red);
            drawingSurface.drawLine(origin, origin + Vector3.Right * 0.1f);
            drawingSurface.setColor(Color.Blue);
            drawingSurface.drawLine(origin, origin + Vector3.Up * 0.1f);
            drawingSurface.setColor(Color.Green);
            drawingSurface.drawLine(origin, origin + Vector3.Backward * 0.1f);

            drawingSurface.setColor(Color.Red);

            Vector3[] vertices = new Vector3[8];
            worldBox.computeVertices(vertices);

            //-Z ccw
            drawingSurface.drawLine(vertices[0], vertices[1]);
            drawingSurface.drawLine(vertices[1], vertices[2]);
            drawingSurface.drawLine(vertices[2], vertices[3]);
            drawingSurface.drawLine(vertices[3], vertices[0]);

            //+z ccw
            drawingSurface.drawLine(vertices[4], vertices[5]);
            drawingSurface.drawLine(vertices[5], vertices[6]);
            drawingSurface.drawLine(vertices[6], vertices[7]);
            drawingSurface.drawLine(vertices[7], vertices[4]);

            //Sides
            drawingSurface.drawLine(vertices[0], vertices[4]);
            drawingSurface.drawLine(vertices[1], vertices[5]);
            drawingSurface.drawLine(vertices[2], vertices[6]);
            drawingSurface.drawLine(vertices[3], vertices[7]);
        }

        [Editable]
        public Vector3 BoundsOrigin
        {
            get
            {
                return boundingBox.getCenter();
            }
            set
            {
                boundingBox.setCenter(value);
            }
        }

        [Editable]
        public Vector3 BoundsExtents
        {
            get
            {
                return boundingBox.getExtents();
            }
            set
            {
                boundingBox.setExtents(value);
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        #region Save

        protected ToothSection(LoadInfo info)
            : base(info)
        {
            boundingBox = new Box3();
            Vector3[] axes = boundingBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Forward;

            BoundsOrigin = info.GetVector3("BoundsOrigin");
            BoundsExtents = info.GetVector3("BoundsExtents");
        }

        protected override void customizeSave(SaveInfo info)
        {
            info.AddValue("BoundsOrigin", BoundsOrigin);
            info.AddValue("BoundsExtents", BoundsExtents);
        }

        #endregion

        internal bool intersects(Ray3 localRay)
        {
            return boundingBox.testIntersection(ref localRay);
        }
    }
}
