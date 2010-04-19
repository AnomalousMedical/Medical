using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using BulletPlugin;
using Engine;

namespace Medical
{
    class ReshapeableBottomTooth : BottomTooth
    {
        protected override void constructed()
        {
            try
            {
                base.constructed();
            }
            catch (Exception)
            {

            }

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

                            fixed (float* verts = &verticesArray[0].x)
                            {
                                fixed (uint* idxs = &indicesArray[0])
                                {
                                    ConvexDecompositionDesc decompDesc = new ConvexDecompositionDesc();
                                    decompDesc.mVcount = vertexBuffer.Value.getNumVertices();
                                    decompDesc.mVertices = verts;
                                    decompDesc.mTcount = indexBuffer.Value.getNumIndexes() / 3;
                                    decompDesc.mIndices = idxs;
                                    decompDesc.mDepth = 2;
                                    decompDesc.mCpercent = 5;
                                    decompDesc.mPpercent = 15;
                                    decompDesc.mMaxVertices = 16;
                                    decompDesc.mSkinWidth = 0.0f;

                                    ReshapeableRigidBody reshape = (ReshapeableRigidBody)actorElement;
                                    reshape.createHullRegion("Tooth", decompDesc);
                                    reshape.recomputeMassProps();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
