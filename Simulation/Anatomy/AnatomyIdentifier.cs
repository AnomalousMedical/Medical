using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;

namespace Medical
{
    public partial class AnatomyIdentifier : Interface, Anatomy
    {
        [Editable]
        private String anatomicalName;

        [DoNotSave]
        private List<AnatomyTag> tags = new List<AnatomyTag>();

        protected override void constructed()
        {
            AnatomyManager.addAnatomy(this);
        }

        protected override void destroy()
        {
            AnatomyManager.removeAnatomy(this);
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
            if (tagEditInterface != null)
            {
                tagEditInterface.addEditableProperty(tag);
            }
        }

        public void removeTag(AnatomyTag tag)
        {
            tags.Remove(tag);
            if (tagEditInterface != null)
            {
                tagEditInterface.removeEditableProperty(tag);
            }
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList<AnatomyTag>("AnatomyTag", tags);
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList<AnatomyTag>("AnatomyTag", tags);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addSubInterface(createTagEditInterface());
        }

        #region Tag EditInterface

        [DoNotCopy]
        [DoNotSave]
        private EditInterface tagEditInterface;

        /// <summary>
        /// Get the EditInterface.
        /// </summary>
        /// <returns>The EditInterface.</returns>
        private EditInterface createTagEditInterface()
        {
            tagEditInterface = new EditInterface("Tags", addTag, removeTag, validateTag);
            tagEditInterface.setPropertyInfo(AnatomyTag.Info);
            foreach (AnatomyTag tag in tags)
            {
                tagEditInterface.addEditableProperty(tag);
            }
            return tagEditInterface;
        }

        /// <summary>
        /// Callback to add a resource.
        /// </summary>
        /// <param name="callback"></param>
        private void addTag(EditUICallback callback)
        {
            addTag(new AnatomyTag());
        }

        /// <summary>
        /// Callback to remove a resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="property"></param>
        private void removeTag(EditUICallback callback, EditableProperty property)
        {
            removeTag((AnatomyTag)property);
        }

        /// <summary>
        /// Callback to validate the resources.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool validateTag(out String message)
        {
            foreach (AnatomyTag tag in tags)
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

        #endregion Tag EditInterface
    }
}
