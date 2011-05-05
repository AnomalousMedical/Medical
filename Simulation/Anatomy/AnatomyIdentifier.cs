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
    public partial class AnatomyIdentifier : Interface, Anatomy
    {
        [Editable]
        private String anatomicalName;

        [Editable]
        private bool allowGroupSelection = true;

        [Editable]
        private String nodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [DoNotSave]
        private List<AnatomyTag> tags = new List<AnatomyTag>();

        [DoNotSave]
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        [DoNotCopy]
        [DoNotSave]
        private AnatomyIdentifierEditInterface anatomyIdentifierEditInterface;

        [DoNotCopy]
        [DoNotSave]
        private TransparencyChanger transparencyChanger = null;

        [DoNotCopy]
        [DoNotSave]
        private Entity entity;

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
                command.Dispose();
            }
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
            if (anatomyIdentifierEditInterface != null)
            {
                anatomyIdentifierEditInterface.addTagProperty(tag);
            }
        }

        public void removeTag(AnatomyTag tag)
        {
            tags.Remove(tag);
            if (anatomyIdentifierEditInterface != null)
            {
                anatomyIdentifierEditInterface.removeTagProperty(tag);
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

        /// <summary>
        /// This is the TransparencyChanger for this anatomy. It can be null if
        /// no TransparencyChanger is assigned.
        /// </summary>
        [DoNotCopy]
        public TransparencyChanger TransparencyChanger
        {
            get
            {
                return transparencyChanger;
            }
            internal set
            {
                this.transparencyChanger = value;
            }
        }

        public void addCommand(AnatomyCommand command)
        {
            commands.Add(command);
            if (anatomyIdentifierEditInterface != null)
            {
                anatomyIdentifierEditInterface.createCommandEditInterface(command);
            }
        }

        public void removeCommand(AnatomyCommand command)
        {
            commands.Remove(command);
            if (anatomyIdentifierEditInterface != null)
            {
                anatomyIdentifierEditInterface.removeCommandEditInterface(command);
            }
        }

        internal bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return entity.raycastPolygonLevel(ray, ref distanceOnRay);
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList<AnatomyTag>("AnatomyTag", tags);
            info.ExtractList<AnatomyCommand>("AnatomyCommand", commands);
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList<AnatomyTag>("AnatomyTag", tags);
            info.RebuildList<AnatomyCommand>("AnatomyCommand", commands);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            anatomyIdentifierEditInterface = new AnatomyIdentifierEditInterface(this, editInterface);
        }
    }

    class AnatomyIdentifierEditInterface
    {
        private static AnatomyCommandBrowser anatomyCommandBrowser = null;

        private EditInterface mainEditInterface;
        private AnatomyIdentifier anatomyIdentifier;

        public AnatomyIdentifierEditInterface(AnatomyIdentifier anatomyIdentifier, EditInterface mainEditInterface)
        {
            this.mainEditInterface = mainEditInterface;
            this.anatomyIdentifier = anatomyIdentifier;

            //Tags
            tagEditInterface = new EditInterface("Tags", addTag, removeTag, validateTag);
            tagEditInterface.setPropertyInfo(AnatomyTag.Info);
            foreach (AnatomyTag tag in anatomyIdentifier.Tags)
            {
                addTagProperty(tag);
            }
            mainEditInterface.addSubInterface(tagEditInterface);

            //Commands
            mainEditInterface.addCommand(new EditInterfaceCommand("Add Command", createCommand));
            commandEditInterfaces = new EditInterfaceManager<AnatomyCommand>(mainEditInterface);
            foreach (AnatomyCommand command in anatomyIdentifier.Commands)
            {
                createCommandEditInterface(command);
            }
        }

        #region Command EditInterface

        private EditInterfaceManager<AnatomyCommand> commandEditInterfaces;

        private void createCommand(EditUICallback callback, EditInterfaceCommand caller)
        {
            if (anatomyCommandBrowser == null)
            {
                anatomyCommandBrowser = new AnatomyCommandBrowser();
            }
            Object objResult;
            bool result = callback.showBrowser(anatomyCommandBrowser, out objResult);
            Type commandType = objResult as Type;
            if (result && commandType != null)
            {
                anatomyIdentifier.addCommand((AnatomyCommand)Activator.CreateInstance(commandType));
            }
        }

        private void destroyCommand(EditUICallback callback, EditInterfaceCommand caller)
        {
            anatomyIdentifier.removeCommand(commandEditInterfaces.resolveSourceObject(callback.getSelectedEditInterface()));
        }

        public void createCommandEditInterface(AnatomyCommand command)
        {
            EditInterface commandEdit = command.createEditInterface();
            commandEdit.addCommand(new EditInterfaceCommand("Remove", destroyCommand));
            commandEditInterfaces.addSubInterface(command, commandEdit);
        }

        public void removeCommandEditInterface(AnatomyCommand command)
        {
            commandEditInterfaces.removeSubInterface(command);
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
            anatomyIdentifier.addTag(new AnatomyTag());
        }

        /// <summary>
        /// Callback to remove a resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="property"></param>
        private void removeTag(EditUICallback callback, EditableProperty property)
        {
            anatomyIdentifier.removeTag((AnatomyTag)property);
        }

        /// <summary>
        /// Callback to validate the resources.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool validateTag(out String message)
        {
            foreach (AnatomyTag tag in anatomyIdentifier.Tags)
            {
                if (tag.Tag == null || tag.Tag == String.Empty)
                {
                    message = "Cannot accept empty tags. Please remove any blank entries.";
                    return false;
                }
            }
            message = null;
            return true;
        }

        internal void addTagProperty(AnatomyTag tag)
        {
            tagEditInterface.addEditableProperty(tag);
        }

        internal void removeTagProperty(AnatomyTag tag)
        {
            tagEditInterface.removeEditableProperty(tag);
        }

        #endregion Tag EditInterface
    }
}
