using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;
using OgreWrapper;
using OgrePlugin;

namespace Medical
{
    public partial class AnatomyIdentifier : BehaviorInterface, Anatomy
    {
        [Editable]
        private String anatomicalName;

        [Editable]
        private bool allowGroupSelection = true;

        [Editable]
        bool showInBasicVersion = false;

        [Editable]
        bool showInTextSearch = true;

        [Editable]
        bool showInClickSearch = true;

        [Editable]
        bool showInTree = false;

        [Editable]
        private String region;

        [Editable]
        private String classification;

        [Editable]
        private String nodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private Vector3 previewCameraDirection = Vector3.Backward;

        [DoNotSave]
        private List<AnatomyTag> tags = new List<AnatomyTag>();

        [DoNotSave]
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        [DoNotSave]
        private List<String> systems = new List<String>();

        [DoNotSave]
        private List<String> connectedTo = new List<String>();

        [DoNotCopy]
        [DoNotSave]
        private EditInterface mainEditInterface;

        [DoNotCopy]
        [DoNotSave]
        private Entity entity;

        [DoNotCopy]
        [DoNotSave]
        private LinkedList<Anatomy> relatedAnatomy = new LinkedList<Anatomy>();

        [DoNotCopy]
        [DoNotSave]
        private TransparencyAnatomyCommand transparencyCommand;

        protected override void constructed()
        {
            
        }

        protected override void link()
        {
            String errorMessage = null;
            foreach (AnatomyCommand command in commands)
            {
                if (!command.link(Owner, this, ref errorMessage))
                {
                    blacklist("SimObject {0} AnatomyIdentifier {1} failed to link command {2}. Reason: {3}", Owner.Name, AnatomicalName, command.UIText, errorMessage);
                }
            }
            SceneNodeElement nodeElement = Owner.getElement(nodeName) as SceneNodeElement;
            if (nodeElement == null)
            {
                blacklist("SimObject {0} AnatomyIdentifier {1} cannot find node named {2}", Owner.Name, AnatomicalName, nodeName);
            }
            entity = nodeElement.getNodeObject(entityName) as Entity;
            if (entity == null)
            {
                blacklist("SimObject {0} AnatomyIdentifier {1} Node {2} cannot find entity named {3}", Owner.Name, AnatomicalName, nodeName, entityName);
            }
            AnatomyManager.addAnatomy(this);
        }

        protected override void destroy()
        {
            AnatomyManager.removeAnatomy(this);
            foreach (AnatomyCommand command in commands)
            {
                command.destroy();
            }
        }

        public void addRelatedAnatomy(Anatomy anatomy)
        {
            relatedAnatomy.AddLast(anatomy);
        }

        public String AnatomicalName
        {
            get
            {
                return anatomicalName;
            }
        }

        [DoNotCopy]
        public IEnumerable<AnatomyTag> Tags
        {
            get
            {
                return tags;
            }
        }

        public void addTag(AnatomyTag tag)
        {
            tags.Add(tag);
            if (mainEditInterface != null)
            {
                this.addTagProperty(tag);
            }
        }

        public void removeTag(AnatomyTag tag)
        {
            tags.Remove(tag);
            if (mainEditInterface != null)
            {
                this.removeTagProperty(tag);
            }
        }

        [DoNotCopy]
        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                return commands;
            }
        }

        [DoNotCopy]
        public IEnumerable<Anatomy> RelatedAnatomy
        {
            get
            {
                return relatedAnatomy;
            }
        }

        [DoNotCopy]
        public IEnumerable<Anatomy> SelectableAnatomy
        {
            get
            {
                yield return this;
            }
        }

        public AxisAlignedBox WorldBoundingBox
        {
            get
            {
                return entity.getWorldBoundingBox();
            }
        }

        /// <summary>
        /// Override the ability to select as a group. This is useful for stuff
        /// that is itself a top level object that isn't part of a group.
        /// </summary>
        public bool AllowGroupSelection
        {
            get
            {
                return allowGroupSelection;
            }
        }

        public bool ShowInTextSearch
        {
            get
            {
                return showInTextSearch;
            }
        }

        public bool ShowInClickSearch
        {
            get
            {
                return showInClickSearch;
            }
        }

        public bool ShowInTree
        {
            get
            {
                return showInTree;
            }
        }

        public bool ShowInBasicVersion
        {
            get
            {
                return showInBasicVersion;
            }
        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            if (transparencyCommand != null)
            {
                transparencyCommand.smoothBlend(alpha, duration, easingFunction);
            }
        }

        public float CurrentAlpha
        {
            get
            {
                if(transparencyCommand != null)
                {
                    return transparencyCommand.CurrentAlpha;
                }
                return 0.0f;
            }
        }

        public IEnumerable<String> TransparencyNames
        {
            get
            {
                if(transparencyCommand != null)
                {
                    yield return transparencyCommand.TransparencyInterfaceName;
                }
                yield break;
            }
        }

        [DoNotCopy]
        public Vector3 PreviewCameraDirection
        {
            get
            {
                return previewCameraDirection;
            }
            set
            {
                previewCameraDirection = value;
            }
        }

        public void addCommand(AnatomyCommand command)
        {
            commands.Add(command);
            createCommandEditInterface(command);
        }

        public void removeCommand(AnatomyCommand command)
        {
            commands.Remove(command);
            removeCommandEditInterface(command);
        }

        internal bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return entity.raycastPolygonLevel(ray, ref distanceOnRay);
        }

        internal void _setTransparencyCommand(TransparencyAnatomyCommand command)
        {
            this.transparencyCommand = command;
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList<AnatomyTag>("AnatomyTag", tags);
            info.ExtractList<AnatomyCommand>("AnatomyCommand", commands);
            info.ExtractList("System", systems);
            info.ExtractList("ConnectedTo", connectedTo);
        }

        //Erase this static property when finished upgrading
        static String[] classificationUpgrades = { "Bone", "Ligament", "Muscle" };
        static String[] regions = { "Arm", "Leg" };
        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList<AnatomyTag>("AnatomyTag", tags);
            info.RebuildList<AnatomyCommand>("AnatomyCommand", commands);
            info.RebuildList("System", systems);
            info.RebuildList("ConnectedTo", connectedTo);

            
            //Custom conversion code from tags to new style, remove this after updating
            bool updateSystems = systems.Count == 0;
            List<AnatomyTag> toRemove = new List<AnatomyTag>();
            foreach(var tag in tags)
            {
                if (updateSystems && tag.Tag.Contains("System") && !systems.Contains(tag.Tag))
                {
                    systems.Add(tag.Tag);
                    toRemove.Add(tag);
                }
                if(classification == null && classificationUpgrades.Contains(tag.Tag))
                {
                    classification = tag.Tag;
                }
                if(region == null && regions.Contains(tag.Tag))
                {
                    region = tag.Tag;
                }
            }
            tags.RemoveAll(i => toRemove.Contains(i));
        }

        private static AnatomyCommandBrowser anatomyCommandBrowser = null;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);

            this.mainEditInterface = editInterface;

            //Tags
            tagEditInterface = new EditInterface("Tags", addTag, removeTag, () =>
            {
                if (this.Tags.Any(t => t.Tag == null || t.Tag == String.Empty))
                {
                    throw new ValidationException("Cannot accept empty tags. Please remove any blank entries.");
                }
            });
            tagEditInterface.setPropertyInfo(AnatomyTag.Property.Info);
            foreach (AnatomyTag tag in this.Tags)
            {
                addTagProperty(tag);
            }
            mainEditInterface.addSubInterface(tagEditInterface);

            editInterface.addSubInterfaceForObject(systems, new StringListlikeEditInterface(systems, "Systems"));
            editInterface.addSubInterfaceForObject(connectedTo, new StringListlikeEditInterface(connectedTo, "Connected To"));

            //Commands
            mainEditInterface.addCommand(new EditInterfaceCommand("Add Command", createCommand));
            var commandEditInterfaces = mainEditInterface.createEditInterfaceManager<AnatomyCommand>();
            foreach (AnatomyCommand command in this.Commands)
            {
                createCommandEditInterface(command);
            }
        }

        #region Command EditInterface

        private void createCommand(EditUICallback callback)
        {
            if (anatomyCommandBrowser == null)
            {
                anatomyCommandBrowser = new AnatomyCommandBrowser();
            }
            callback.showBrowser(anatomyCommandBrowser, delegate(Object result, ref String errorMessage)
            {
                Type commandType = result as Type;
                if (commandType != null)
                {
                    this.addCommand((AnatomyCommand)Activator.CreateInstance(commandType));
                    return true;
                }
                return false;
            });
        }

        private void destroyCommand(EditUICallback callback)
        {
            this.removeCommand(mainEditInterface.resolveSourceObject<AnatomyCommand>(callback.getSelectedEditInterface()));
        }

        private void createCommandEditInterface(AnatomyCommand command)
        {
            if (mainEditInterface != null)
            {
                EditInterface commandEdit = command.createEditInterface();
                commandEdit.addCommand(new EditInterfaceCommand("Remove", destroyCommand));
                mainEditInterface.addSubInterface(command, commandEdit);
            }
        }

        private void removeCommandEditInterface(AnatomyCommand command)
        {
            if (mainEditInterface != null)
            {
                mainEditInterface.removeSubInterface(command);
            }
        }

        #endregion

        #region Tag EditInterface

        [DoNotCopy]
        [DoNotSave]
        private EditInterface tagEditInterface;

        /// <summary>
        /// Callback to add a resource.
        /// </summary>
        /// <param name="callback"></param>
        private void addTag(EditUICallback callback)
        {
            this.addTag(new AnatomyTag());
        }

        /// <summary>
        /// Callback to remove a resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="property"></param>
        private void removeTag(EditUICallback callback, EditableProperty property)
        {
            this.removeTag(tagEditInterface.getKeyObjectForProperty<AnatomyTag>(property));
        }

        private void addTagProperty(AnatomyTag tag)
        {
            tagEditInterface.addEditableProperty(tag, new AnatomyTag.Property(tag));
        }

        private void removeTagProperty(AnatomyTag tag)
        {
            tagEditInterface.removeEditableProperty(tag);
        }

        #endregion Tag EditInterface
    }
}
