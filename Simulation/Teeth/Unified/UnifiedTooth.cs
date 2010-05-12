using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;
using OgreWrapper;
using Engine.Attributes;
using BulletPlugin;
using Engine.Renderer;
using Engine.Saving;
using Engine.ObjectManagement;

namespace Medical
{
    public abstract class UnifiedTooth : Tooth
    {
        [Editable]
        private String sceneNodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private String transparencyInterface = "Alpha";

        [Editable]
        private bool extracted = false;

        [Editable]
        private String unifiedBodyObject = "";

        [Editable]
        private String unifiedBodyActor = "Actor";

        protected bool loose = false;

        protected SceneNodeElement sceneNodeElement;
        protected Entity entity;

        private TransparencyInterface transparency;
        protected ReshapeableRigidBody unifiedActorElement;
        protected SimObject unifiedObject;
        protected Vector3 startingLocation;
        protected Quaternion startingRotation;
        private Vector3 offset = Vector3.Zero;
        private Quaternion rotationOffset = Quaternion.Identity;
        private int adaptTeeth = 0;

        private bool toolHighlight = false;

        [DoNotCopy]
        [DoNotSave]
        protected List<Tooth> collidingTeeth = new List<Tooth>(5);

        [DoNotCopy]
        [DoNotSave]
        private bool showTools = false;

        //Dynamic teeth stuff
        [DoNotSave]
        private List<UnifiedToothSection> toothSections = new List<UnifiedToothSection>();

        [Editable]
        private UnifiedToothSection mainToothSection = new UnifiedToothSection("MainTooth");

        [DoNotCopy]
        [DoNotSave]
        Vector3[] verticesArray;

        [DoNotCopy]
        [DoNotSave]
        uint[] indicesArray;

        protected override void constructed()
        {
            //Scene Node and entity
            sceneNodeElement = Owner.getElement(sceneNodeName) as SceneNodeElement;
            validate(sceneNodeElement != null, "Could not find SceneNodeElement {0}.", sceneNodeName);
            entity = sceneNodeElement.getNodeObject(entityName) as Entity;
            validate(entity != null, "Could not find Entity {0}.", entityName);

            //ReshapeableRigidBody
            unifiedObject = Owner.getOtherSimObject(unifiedBodyObject);
            validate(unifiedObject != null, "Could not find unifiedBodyObject {0}", unifiedBodyObject);
            unifiedActorElement = unifiedObject.getElement(unifiedBodyActor) as ReshapeableRigidBody;
            validate(unifiedActorElement != null, "Could not find Actor {0}.", unifiedBodyActor);
            //unifiedActorElement.MaxContactDistance = 0.05f;
            //unifiedActorElement.setActivationState(ActivationState.DisableDeactivation);

            //Create convex hulls
            createConvexHulls();

            //Transparency Interface
            transparency = Owner.getElement(transparencyInterface) as TransparencyInterface;
            validate(transparency != null, "Could not find TransparencyInterface {0}", transparencyInterface);

            //Add to Teeth Controller if everything is ok
            TeethController.addTooth(Owner.Name, this);
        }

        private void createConvexHulls()
        {
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
                            verticesArray = new Vector3[vertexBuffer.Value.getNumVertices()];
                            indicesArray = new uint[indexBuffer.Value.getNumIndexes()];

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

                            bool inWorld = unifiedActorElement.isInWorld();
                            if (inWorld)
                            {
                                unifiedActorElement.removeFromWorld();
                            }
                            ReshapeableRigidBody body = (ReshapeableRigidBody)unifiedActorElement;
                            mainToothSection.checkTriangles(verticesArray, indicesArray);
                            foreach (UnifiedToothSection section in toothSections)
                            {
                                section.checkTriangles(verticesArray, indicesArray);
                            }

                            Vector3 anchorOrigin = Owner.Translation - unifiedObject.Translation;
                            Quaternion anchorOrientation = unifiedObject.Rotation.inverse() * Owner.Rotation;
                            mainToothSection.createSection(verticesArray, unifiedActorElement, anchorOrigin, anchorOrientation, Owner.Name);
                            foreach (UnifiedToothSection section in toothSections)
                            {
                                section.createSection(verticesArray, unifiedActorElement, anchorOrigin, anchorOrientation, Owner.Name);
                            }
                            body.recomputeMassProps();
                            if (inWorld)
                            {
                                unifiedActorElement.addToWorld();
                            }
                        }
                    }
                }
            }
        }

        protected override void destroy()
        {
            TeethController.removeTooth(Owner.Name);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            bool highlight = false;
            if (TeethController.HighlightContacts)
            {
                highlight = MakingContact;
            }
            if (adaptTeeth > 0)
            {
                switch (adaptTeeth)
                {
                    case 1:
                        applyAdaptation(ToothType.Top, true);
                        break;
                    case 2:
                        applyAdaptation(ToothType.Top, false);
                        break;
                    case 3:
                        applyAdaptation(ToothType.Bottom, true);
                        break;
                    case 4:
                        applyAdaptation(ToothType.Bottom, false);
                        break;
                    default:
                        adaptTeeth = 0;
                        break;
                }
                adaptTeeth++;
            }

            if (toolHighlight)
            {
                HighlightColor = Color.Red;
            }
            else
            {
                HighlightColor = highlight ? Color.Blue : Color.White;
            }
        }

        protected abstract void looseChanged(bool loose);

        protected abstract void applyAdaptation(ToothType type, bool adapt);

        [DoNotCopy]
        public override bool Adapt
        {
            get
            {
                return adaptTeeth > 0;
            }
            set
            {
                //Start adaptation or do nothing if started.
                if (value)
                {
                    if (adaptTeeth == 0)
                    {
                        adaptTeeth = 1;
                    }
                }
                //Stop adaptation or do nothing if stopped.
                else
                {
                    if (adaptTeeth > 0)
                    {
                        adaptTeeth = 0;
                        applyAdaptation(ToothType.Top, false);
                        applyAdaptation(ToothType.Bottom, false);
                    }
                }
            }
        }

        [DoNotCopy]
        private Color HighlightColor
        {
            set
            {
                entity.getSubEntity(0).setCustomParameter(0, new Quaternion(value.r, value.g, value.b, value.a));
            }
        }

        [DoNotCopy]
        public override bool Extracted
        {
            get
            {
                return extracted;
            }
            set
            {
                //Put the tooth back if extracted
                if (this.extracted && !value)
                {
                    extracted = false;
                    unifiedActorElement.clearCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(true);
                    transparency.DisableOnHidden = true;
                }
                //Extract the tooth if it is in the scene
                else if (!this.extracted && value)
                {
                    extracted = true;
                    unifiedActorElement.raiseCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(false);
                    transparency.DisableOnHidden = false;
                }
            }
        }

        [DoNotCopy]
        public override bool Loose
        {
            get
            {
                return loose;
            }
            set
            {
                loose = value;
                looseChanged(loose);
            }
        }

        [DoNotCopy]
        public override Vector3 Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        [DoNotCopy]
        public override Quaternion Rotation
        {
            get
            {
                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
            }
        }

        [DoNotCopy]
        public override bool MakingContact
        {
            get
            {
                foreach (Tooth tooth in collidingTeeth)
                {
                    if (!tooth.Extracted)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool rayIntersects(Ray3 worldRay, out float distance, out uint vertexNumber)
        {
            Ray3 localRay = getLocalRay(ref worldRay);

            //Find the closest section that actually hits the tooth.
            Vector3 hitLocation;
            float closestSectionDistance = float.MaxValue;
            UnifiedToothSection closestSection = null;

            if (mainToothSection.intersects(localRay, out hitLocation))
            {
                closestSectionDistance = (hitLocation - localRay.Origin).length2();
                closestSection = mainToothSection;
            }
            foreach (UnifiedToothSection section in toothSections)
            {
                if (section.intersects(localRay, out hitLocation))
                {
                    float distance2 = (hitLocation - localRay.Origin).length2();
                    if (distance2 < closestSectionDistance)
                    {
                        closestSectionDistance = distance2;
                        closestSection = section;
                    }
                }
            }

            //Check the triangles in the closest section
            if (closestSection != null)
            {
                return closestSection.checkTriangleCollision(verticesArray, localRay, out distance, out vertexNumber);
            }
            else
            {
                vertexNumber = 0;
                distance = float.MaxValue;
                return false;
            }
        }

        private Ray3 getLocalRay(ref Ray3 worldRay)
        {
            Ray3 localRay = worldRay;
            Quaternion rotationDir = Owner.Rotation.inverse();
            localRay.Direction = Quaternion.quatRotate(rotationDir, worldRay.Direction);
            localRay.Origin = localRay.Origin - Owner.Translation;
            localRay.Origin = Quaternion.quatRotate(rotationDir, localRay.Origin);
            return localRay;
        }

        private Vector3 slowNormalRecompute(uint vertexNumber)
        {
            //search the whole index array for all triangles that hold this vertex
            List<uint> triangleBases = new List<uint>();
            for (uint i = 0; i < indicesArray.Length; i += 3)
            {
                if (indicesArray[i] == vertexNumber || indicesArray[i + 1] == vertexNumber || indicesArray[i + 2] == vertexNumber)
                {
                    triangleBases.Add(i);
                }
            }
            Vector3 normalTotal = Vector3.Zero;
            foreach (uint baseIndex in triangleBases)
            {
                Vector3 v0 = verticesArray[indicesArray[baseIndex]];
                Vector3 v1 = verticesArray[indicesArray[baseIndex + 1]];
                Vector3 v2 = verticesArray[indicesArray[baseIndex + 2]];
                Vector3 edge1 = v1 - v0;
                Vector3 edge2 = v2 - v0;
                normalTotal += edge1.cross(ref edge2).normalize();
            }
            return (normalTotal /= triangleBases.Count).normalize();
        }

        public unsafe void moveVertex(uint vertex, Ray3 worldRay)
        {
            Ray3 localRay = getLocalRay(ref worldRay);

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

                VertexElement normalElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_NORMAL);
                VertexElement binormalElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_BINORMAL);
                VertexElement tangentElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_TANGENT);

                VertexBufferBinding vertexBinding = vertexData.vertexBufferBinding;
                using (HardwareVertexBufferSharedPtr vertexBuffer = vertexBinding.getBuffer(positionElement.getSource()))
                {
                    uint vertexSize = vertexBuffer.Value.getVertexSize();

                    // Modify vertex data
                    byte* vertexBufferData = (byte*)vertexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_NORMAL);
                    vertexBufferData += vertex * vertexSize;
                    float* position;
                    float* normal;
                    float* tangent;
                    float* binormal;
                    normalElement.baseVertexPointerToElement(vertexBufferData, &normal);
                    positionElement.baseVertexPointerToElement(vertexBufferData, &position);
                    Vector3 posVec = new Vector3(position[0], position[1], position[2]);
                    Vector3 normalVec = new Vector3(normal[0], normal[1], normal[2]);
                    //                    posVec += localRay.Direction * 0.005f;
                    posVec += -normalVec * 0.001f;

                    position[0] = posVec.x;
                    position[1] = posVec.y;
                    position[2] = posVec.z;

                    tangentElement.baseVertexPointerToElement(vertexBufferData, &tangent);
                    binormalElement.baseVertexPointerToElement(vertexBufferData, &binormal);

                    //This WILL NOT WORK for nonparity models
                    meshPtr.Value.buildTangentVectors(VertexElementSemantic.VES_TANGENT, 0, 0, false, false, true);

                    Vector3 normalNewVal = slowNormalRecompute(vertex);
                    normal[0] = normalNewVal.x;
                    normal[1] = normalNewVal.y;
                    normal[2] = normalNewVal.z;

                    Vector3 tangentVec = new Vector3(tangent[0], tangent[1], tangent[2]);
                    Vector3 newBinormal = tangentVec.cross(ref normalNewVal);

                    binormal[0] = newBinormal.x;
                    binormal[1] = newBinormal.y;
                    binormal[2] = newBinormal.z;

                    vertexBuffer.Value.unlock();
                    verticesArray[vertex] = posVec;
                }
            }
        }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            //debugDrawing.begin("ToothRay" + Owner.Name, DrawingType.LineList);

            //mainToothSection.drawBoundsWorld(debugDrawing, Owner.Translation, Owner.Rotation);

            //foreach (UnifiedToothSection section in toothSections)
            //{
            //    section.drawBoundsWorld(debugDrawing, Owner.Translation, Owner.Rotation);
            //}

            //debugDrawing.end();

            debugDrawing.begin("ToothContacts" + Owner.Name, DrawingType.LineList);

            debugDrawing.setColor(new Color(1.0f, 0.0f, 1.0f));

            Vector3 delta = Vector3.ScaleIdentity * 0.01f;
            foreach (Vector3 pos in debugContactPoints)
            {
                debugDrawing.drawLine(pos, pos + delta);
            }

            debugDrawing.end();
        }

        List<Vector3> debugContactPoints = new List<Vector3>();

        void actorElement_ContactContinues(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            if (debugContactPoints.Count > 1000)
            {

                debugContactPoints.Clear();
            }
            ManifoldPoint manifoldPt = new ManifoldPoint();
            TopTooth otherTooth = otherBody.Owner.getElement("Behavior") as TopTooth;
            if (otherTooth != null)
            {
                int numContacts = contact.getNumContacts();
                for (int i = 0; i < numContacts; ++i)
                {
                    contact.getContactPoint(i, manifoldPt);
                    if (isBodyA)
                    {
                        debugContactPoints.Add(manifoldPt.getPositionWorldOnA());
                    }
                    else
                    {
                        debugContactPoints.Add(manifoldPt.getPositionWorldOnB());
                    }
                }
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            //debugContactPoints.Clear();
        }

        protected override void customLoad(LoadInfo info)
        {
            info.RebuildList<UnifiedToothSection>("ToothSections", toothSections);
        }

        protected override void customSave(SaveInfo info)
        {
            info.ExtractList<UnifiedToothSection>("ToothSections", toothSections);
        }

        #region MovableObject Members

        [DoNotCopy]
        public override Vector3 ToolTranslation
        {
            get
            {
                return Owner.Translation;
            }
        }

        public override void move(Vector3 offset)
        {
            Offset += offset;
        }

        [DoNotCopy]
        public override Quaternion ToolRotation
        {
            get
            {
                return rotationOffset;
            }
        }

        public override void rotate(ref Quaternion newRot)
        {
            Rotation = newRot;
        }

        [DoNotCopy]
        public override bool ShowTools
        {
            get
            {
                return showTools && !extracted;
            }
            set
            {
                showTools = value;
            }
        }

        public override void alertToolHighlightStatus(bool highlighted)
        {
            toolHighlight = highlighted;
        }

        #endregion

        #region EditInterface

        [DoNotCopy]
        [DoNotSave]
        private EditInterfaceManager<UnifiedToothSection> sectionManager;

        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            this.editInterface = editInterface;
            sectionManager = new EditInterfaceManager<UnifiedToothSection>(editInterface);
            sectionManager.addCommand(new EditInterfaceCommand("Remove", removeSectionCallback));
            ToothEditRenderer toothEditRenderer = new ToothEditRenderer();
            editInterface.Renderer = toothEditRenderer;
            toothEditRenderer.addSubRenderer(mainToothSection.getEditInterface(null, null).Renderer);//dont need args as this will already be created
            editInterface.addCommand(new EditInterfaceCommand("Add Tooth Section", addSectionCallback));
            foreach (UnifiedToothSection section in toothSections)
            {
                onToothSectionAdded(section);
            }
        }

        private void addSectionCallback(EditUICallback callback, EditInterfaceCommand command)
        {
            String name;
            bool accept = callback.getInputString("Enter a name for the section.", out name, validateSectionCreate);
            if (accept)
            {
                UnifiedToothSection section = new UnifiedToothSection(name);
                toothSections.Add(section);
                onToothSectionAdded(section);
            }
        }

        private bool validateSectionCreate(String input, out String errorPrompt)
        {
            if (input == null || input == "")
            {
                errorPrompt = "Please enter a non empty name.";
                return false;
            }
            foreach (UnifiedToothSection section in toothSections)
            {
                if (section.Name == input)
                {
                    errorPrompt = "That name is already in use. Please provide another.";
                    return false;
                }
            }
            errorPrompt = "";
            return true;
        }

        private void removeSectionCallback(EditUICallback callback, EditInterfaceCommand command)
        {
            EditInterface edit = callback.getSelectedEditInterface();
            UnifiedToothSection section = sectionManager.resolveSourceObject(edit);
            toothSections.Remove(section);
            onToothSectionRemoved(section, edit);
        }

        private void onToothSectionAdded(UnifiedToothSection section)
        {
            EditInterface edit = section.getEditInterface(section.Name, BehaviorEditMemberScanner.Scanner);
            ((ToothEditRenderer)editInterface.Renderer).addSubRenderer(edit.Renderer);
            sectionManager.addSubInterface(section, edit);
        }

        private void onToothSectionRemoved(UnifiedToothSection section, EditInterface edit)
        {
            ((ToothEditRenderer)editInterface.Renderer).removeSubRenderer(edit.Renderer);
            sectionManager.removeSubInterface(section);
        }

        #endregion
    }
}
