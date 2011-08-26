using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using System.Collections;

namespace Medical
{
    public partial class MultipleChoiceOptions : IEnumerable<MultipleChoiceOption>, Saveable
    {
        private List<MultipleChoiceOption> options = new List<MultipleChoiceOption>();

        public MultipleChoiceOptions()
        {

        }

        public void addOption(MultipleChoiceOption option)
        {
            options.Add(option);
            onOptionAdded(option);
        }

        public void removeOption(MultipleChoiceOption option)
        {
            options.Remove(option);
            onOptionRemove(option);
        }

        public IEnumerator<MultipleChoiceOption> GetEnumerator()
        {
            return options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return options.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return options.Count;
            }
        }

        public MultipleChoiceOption this[int index]
        {
            get
            {
                return options[index];
            }
        }

        protected MultipleChoiceOptions(LoadInfo info)
        {
            info.RebuildList("Option", options);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList("Option", options);
        }
    }

    partial class MultipleChoiceOptions
    {
        [DoNotSave]
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Options", addOption, removeOption, validate);
                EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                propertyInfo.addColumn(new EditablePropertyColumn("Text", false));
                editInterface.setPropertyInfo(propertyInfo);

                foreach (MultipleChoiceOption option in options)
                {
                    editInterface.addEditableProperty(option);
                }
            }
            return editInterface;
        }

        private void addOption(EditUICallback callback)
        {
            addOption(new MultipleChoiceOption());
        }

        private void removeOption(EditUICallback callback, EditableProperty property)
        {
            MultipleChoiceOption options = property as MultipleChoiceOption;
            removeOption(options);
        }

        private bool validate(out String message)
        {
            message = null;
            return true;
        }

        private void onOptionAdded(MultipleChoiceOption entry)
        {
            if (editInterface != null)
            {
                editInterface.addEditableProperty(entry);
            }
        }

        private void onOptionRemove(MultipleChoiceOption entry)
        {
            if (editInterface != null)
            {
                editInterface.removeEditableProperty(entry);
            }
        }
    }
}
