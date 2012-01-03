using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Engine.Saving;

namespace Medical
{
    public class NotesDataField : DataField
    {
        public NotesDataField(String name)
            :base(name)
        {
            StartingValue = "";
            NumberOfLines = 10;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        [Editable]
        public String StartingValue { get; set; }

        [Editable]
        public int NumberOfLines { get; set; }

        public override string Type
        {
            get
            {
                return "Notes";
            }
        }

        protected NotesDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
