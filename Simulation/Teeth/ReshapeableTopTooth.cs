using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletPlugin;
using OgreWrapper;
using Engine;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    class ToothEditRenderer : EditInterfaceRenderer
    {
        public void frameUpdate(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {
            
        }

        public void interfaceDeselected(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {

        }

        public void interfaceSelected(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {
            drawingSurface.begin("ToothEditor", Engine.Renderer.DrawingType.LineList);
            drawingSurface.setColor(Color.Red);
            drawingSurface.drawLine(Vector3.Zero, Vector3.UnitX);
            drawingSurface.end();
        }

        public void propertiesChanged(Engine.Renderer.DebugDrawingSurface drawingSurface)
        {
            
        }
    }

    class ReshapeableTopTooth : TopTooth
    {
        [DoNotSave]
        private List<ToothSection> toothSections = new List<ToothSection>();

        [Editable]
        private ToothSection mainToothSection = new ToothSection("MainTooth");

        protected override void constructed()
        {
            base.constructed();
            using (MeshPtr meshPtr = entity.getMesh())
            {
                SubMesh subMesh = meshPtr.Value.getSubMesh(0);

                VertexData vertexData = subMesh.vertexData;
                IndexData indexData = subMesh.indexData;
                if (subMesh.UseSharedVertices)
                {
                    vertexData = meshPtr.Value.SharedVertexData;
                }

                VertexDeclaration vertexDeclaration = vertexData.vertexDeclaration;
                VertexElement positionElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_POSITION);

                VertexBufferBinding vertexBinding = vertexData.vertexBufferBinding;
                using (HardwareVertexBufferSharedPtr vertexBuffer = vertexBinding.getBuffer(positionElement.getSource()))
                {
                    using (HardwareIndexBufferSharedPtr indexBuffer = indexData.IndexBuffer)
                    {
                        uint vertexSize = vertexBuffer.Value.getVertexSize();

                        uint numVertices = vertexBuffer.Value.getNumVertices();
                        uint positionOffset = positionElement.getOffset();

                        uint numIndices = indexBuffer.Value.getNumIndexes();
                        uint numTriangles = numIndices / 3;
                        unsafe
                        {
                            Vector3[] verticesArray = new Vector3[vertexBuffer.Value.getNumVertices()];
                            uint[] indicesArray = new uint[indexBuffer.Value.getNumIndexes()];

                            // Get vertex data
                            byte* vertexBufferData = (byte*)vertexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                            float* elemStart;
                            for (int i = 0; i < numVertices; ++i)
                            {
                                positionElement.baseVertexPointerToElement(vertexBufferData, &elemStart);
                                verticesArray[i].x = *elemStart++;
                                verticesArray[i].y = *elemStart++;
                                verticesArray[i].z = *elemStart++;
                                vertexBufferData += vertexSize;
                            }
                            vertexBuffer.Value.unlock();

                            // Get index data
                            if (indexBuffer.Value.getType() == HardwareIndexBuffer.IndexType.IT_16BIT)
                            {
                                ushort* indexBufferData = (ushort*)indexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                                for (int i = 0; i < numIndices; ++i)
                                {
                                    indicesArray[i] = indexBufferData[i];
                                }
                                indexBuffer.Value.unlock();
                            }
                            else if (indexBuffer.Value.getType() == HardwareIndexBuffer.IndexType.IT_32BIT)
                            {
                                uint* indexBufferData = (uint*)indexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                                for (int i = 0; i < numIndices; ++i)
                                {
                                    indicesArray[i] = indexBufferData[i];
                                }
                                indexBuffer.Value.unlock();
                            }

                            ReshapeableRigidBody body = (ReshapeableRigidBody)actorElement;
                            mainToothSection.checkTriangles(verticesArray, indicesArray);
                            mainToothSection.createSection(verticesArray, body);
                            body.recomputeMassProps();                            
                        }
                    }
                }
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.Renderer = new ToothEditRenderer();
        }
    }
}
