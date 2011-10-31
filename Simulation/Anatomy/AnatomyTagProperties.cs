using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public partial class AnatomyTagProperties : Saveable
    {
        bool showInBasicVersion = true;
        bool showInTextSearch = true;
        bool showInClickSearch = true;
        bool showInTree = true;
        String name;

        public AnatomyTagProperties()
        {

        }

        public AnatomyTagProperties(String name)
        {
            this.name = name;
        }

        [DoNotCopy]
        [Editable]
        public bool ShowInTextSearch
        {
            get
            {
                return showInTextSearch;
            }
            set
            {
                showInTextSearch = value;
            }
        }

        [DoNotCopy]
        [Editable]
        public bool ShowInClickSearch
        {
            get
            {
                return showInClickSearch;
            }
            set
            {
                showInClickSearch = value;
            }
        }

        [DoNotCopy]
        [Editable]
        public bool ShowInTree
        {
            get
            {
                return showInTree;
            }
            set
            {
                showInTree = value;
            }
        }

        [DoNotCopy]
        [Editable]
        public bool ShowInBasicVersion
        {
            get
            {
                return showInBasicVersion;
            }
            set
            {
                showInBasicVersion = value;
            }
        }

        [DoNotCopy]
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #region Saveable Members

        protected AnatomyTagProperties(LoadInfo info)
        {
            showInTextSearch = info.GetBoolean("ShowInTextSearch");
            showInClickSearch = info.GetBoolean("ShowInClickSearch");
            showInTree = info.GetBoolean("ShowInTree");
            showInBasicVersion = info.GetBoolean("ShowInBasicVersion");
            name = info.GetString("Name");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("ShowInTextSearch", showInTextSearch);
            info.AddValue("ShowInClickSearch", showInClickSearch);
            info.AddValue("ShowInTree", showInTree);
            info.AddValue("ShowInBasicVersion", showInBasicVersion);
            info.AddValue("Name", name);
        }

        #endregion
    }

    partial class AnatomyTagProperties
    {
        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name, null);
                }
                return editInterface;
            }
        }
    }
}
