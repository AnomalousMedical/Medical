using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;
using Engine.Attributes;

namespace Medical
{
    public partial class MultipleChoiceField : DataField
    {
        private MultipleChoiceOptions options;

        public MultipleChoiceField(String name)
            :base(name)
        {
            options = new MultipleChoiceOptions();
            StartingOptionIndex = 0;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(options.getEditInterface());
        }

        [Editable]
        public int StartingOptionIndex { get; set; }

        public string DefaultValue
        {
            get
            {
                if (StartingOptionIndex < options.Count)
                {
                    return options[StartingOptionIndex].OptionText;
                }
                return null;
            }
        }

        public MultipleChoiceOptions Options
        {
            get
            {
                return options;
            }
        }

        public override string Type
        {
            get
            {
                return "Multiple Choice";
            }
        }

        protected MultipleChoiceField(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
