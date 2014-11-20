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
        private String structure;

        [Editable]
        private String nodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private Vector3 previewCameraDirection = Vector3.Backward;

        [DoNotSave]
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        [DoNotSave]
        private List<CallbackAnatomyCommand> externalCommands = new List<CallbackAnatomyCommand>();

        [DoNotSave]
        private List<String> tags = new List<String>();

        [DoNotSave]
        private List<String> systems = new List<String>();

        [DoNotSave]
        private List<String> connections = new List<String>();

        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        [DoNotCopy]
        [DoNotSave]
        private Entity entity;

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

        public String AnatomicalName
        {
            get
            {
                return anatomicalName;
            }
        }

        public void addTag(String tag)
        {
            tags.Add(tag);
            editInterface.safeAlertSubInterfaceDataContentsChanged(tags);
        }

        public void removeTag(String tag)
        {
            tags.Remove(tag);
            editInterface.safeAlertSubInterfaceDataContentsChanged(tags);
        }

        [DoNotCopy]
        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                foreach(var command in externalCommands)
                {
                    yield return command;
                }
                foreach (var command in commands)
                {
                    yield return command;
                }
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

        /// <summary>
        /// The systems this anatomy is part of.
        /// </summary>
        public IEnumerable<String> Systems
        {
            get
            {
                return systems;
            }
        }

        /// <summary>
        /// The connections for this anatomy.
        /// </summary>
        public IEnumerable<String> Connections
        {
            get
            {
                return connections;
            }
        }

        /// <summary>
        /// The tags for this anatomy.
        /// </summary>
        public IEnumerable<String> Tags
        {
            get
            {
                return tags;
            }
        }

        /// <summary>
        /// The region for this anatomy.
        /// </summary>
        public String Region
        {
            get
            {
                return region;
            }
        }

        /// <summary>
        /// The structure for this anatomy.
        /// </summary>
        public String Structure
        {
            get
            {
                return structure;
            }
        }

        /// <summary>
        /// The classificaiton for this anatomy.
        /// </summary>
        public String Classification
        {
            get
            {
                return classification;
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
            editInterface.safeAddSubInterfaceForObject(commands, command);
        }

        public void removeCommand(AnatomyCommand command)
        {
            commands.Remove(command);
            editInterface.safeRemoveSubInterfaceForObject(commands, command);
        }

        /// <summary>
        /// Add an external command, these commands are not saved.
        /// </summary>
        /// <param name="command"></param>
        public void addExternalCommand(CallbackAnatomyCommand command)
        {
            externalCommands.Add(command);
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
            info.Version = 1;
            base.customSave(info);
            info.ExtractList("AnatomyTag", tags);
            info.ExtractList("System", systems);
            info.ExtractList("Connection", connections);
            info.ExtractList("AnatomyCommand", commands);
        }

        //Custom conversion code from tags to new style, remove this after updating
        public static readonly String[] classificationUpgrades = { "Bone", "Ligament", "Muscle" };
        public static readonly String[] autoClassifications = { "Artery", "Vein", "Tooth" };
        public static readonly String[] autoStructures = { "Foot", "Leg", "Arm", "Hand", "Spine" };
        public static readonly String[] regions = { "Arm", "Leg" };

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            //Can remove after everything is converted.
            if (info.Version < 1)
            {
                List<AnatomyTag> anatomyTags = new List<AnatomyTag>();
                info.RebuildList<AnatomyTag>("AnatomyTag", anatomyTags);
                List<AnatomyTag> toRemove = new List<AnatomyTag>();
                foreach (var tag in anatomyTags)
                {
                    if (tag.Tag.Contains("System") && tag.Tag != "Masticatory System")
                    {
                        systems.Add(tag.Tag);
                        toRemove.Add(tag);
                    }
                    if (classification == null)
                    {
                        if (classificationUpgrades.Contains(tag.Tag))
                        {
                            classification = tag.Tag;
                            toRemove.Add(tag);
                        }
                        else
                        {
                            var query = from q in autoClassifications
                                        where AnatomicalName.Contains(q)
                                        select q;

                            classification = query.FirstOrDefault();
                        }
                    }
                    if (region == null && regions.Contains(tag.Tag))
                    {
                        //region = tag.Tag;
                        toRemove.Add(tag);
                    }
                    if(structure == null)
                    {
                        var query = from q in anatomyTags
                                    where autoStructures.Any(i => q.Tag.Contains(i))
                                    select q;

                        AnatomyTag result = query.FirstOrDefault();
                        if (result != null)
                        {
                            toRemove.Add(result);
                            structure = makeLeftRight(result.Tag);
                        }
                    }
                }
                
                //We can assume most things are in the head and neck region
                if (region == null)
                {
                    if (classification != "Bone" || anatomyTags.Any(i => i.Tag.Contains("C-Spine")) || AnatomicalName == "Mandible" || AnatomicalName.Contains("Skull"))
                    {
                        region = "Head and Neck";
                    }
                    else if (anatomyTags.Any(i => i.Tag.Contains("Hand") || i.Tag.Contains("Arm") || i.Tag.Contains("Shoulder Blade") || i.Tag.Contains("Clavicle")))
                    {
                        region = "Upper Limb";
                    }
                    else if (anatomyTags.Any(i => i.Tag.Contains("Foot") || i.Tag.Contains("Leg") ))
                    {
                        region = "Lower Limb";
                    }
                    else if (anatomyTags.Any(i => i.Tag.Contains("T-Spine") || i.Tag.Contains("Chest")))
                    {
                        region = "Thorax";
                    }
                    else if (anatomyTags.Any(i => i.Tag.Contains("L-Spine") || AnatomicalName == "Pelvis" || AnatomicalName == "Sacrum"))
                    {
                        region = "Abdomen";
                    }
                }

                tags.AddRange(anatomyTags.Where(i => !toRemove.Contains(i)).Select(t => t.Tag));
            }
            else
            {
                info.RebuildList("AnatomyTag", tags);
                info.RebuildList("System", systems);
                info.RebuildList("Connection", connections);
            }
            info.RebuildList("AnatomyCommand", commands);
        }

        private String makeLeftRight(String structureName)
        {
            if(structureName == null)
            {
                return structureName;
            }

            if (anatomicalName.Contains("Left"))
            {
                return String.Format("Left {0}", structureName);
            }
            else if(anatomicalName.Contains("Right"))
            {
                return String.Format("Right {0}", structureName);
            }
            return structureName;
        }

        private static AnatomyCommandBrowser anatomyCommandBrowser = null;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);

            this.editInterface = editInterface;

            editInterface.addSubInterfaceForObject(tags, new StringListlikeEditInterface(tags, "Tags",
                validateCallback: () =>
                {
                    if (this.Tags.Any(t => String.IsNullOrWhiteSpace(t)))
                    {
                        throw new ValidationException("Cannot accept blank tags. Please remove any blank entries.");
                    }
                }));
            editInterface.addSubInterfaceForObject(systems, new StringListlikeEditInterface(systems, "Systems",
                validateCallback: () =>
                {
                    if (this.systems.Any(t => String.IsNullOrWhiteSpace(t)))
                    {
                        throw new ValidationException("Cannot accept blank systems. Please remove any blank entries.");
                    }
                }));
            editInterface.addSubInterfaceForObject(connections, new StringListlikeEditInterface(connections, "Connections",
                validateCallback: () =>
                {
                    if (this.connections.Any(t => String.IsNullOrWhiteSpace(t)))
                    {
                        throw new ValidationException("Cannot accept blank connections. Please remove any blank entries.");
                    }
                }));

            //Commands
            EditInterface commandsEditInterface = new EditInterface("Commands");

            commandsEditInterface.addCommand(new EditInterfaceCommand("Add Command", cb =>
            {
                if (anatomyCommandBrowser == null)
                {
                    anatomyCommandBrowser = new AnatomyCommandBrowser();
                }
                cb.showBrowser(anatomyCommandBrowser, delegate(Object result, ref String errorMessage)
                {
                    Type commandType = result as Type;
                    if (commandType != null)
                    {
                        this.addCommand((AnatomyCommand)Activator.CreateInstance(commandType));
                        return true;
                    }
                    return false;
                });
            }));

            var commandEditInterfaces = commandsEditInterface.createEditInterfaceManager<AnatomyCommand>(i => i.createEditInterface(), Commands);
            commandEditInterfaces.addCommand(new EditInterfaceCommand("Remove", cb => this.removeCommand(commandsEditInterface.resolveSourceObject<AnatomyCommand>(cb.getSelectedEditInterface()))));
            editInterface.addSubInterfaceForObject(commands, commandsEditInterface);
        }
    }
}
