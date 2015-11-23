using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving;

namespace Medical.GUI
{
    class ShowTasksAnatomyCommand : AnatomyCommand
    {
        private AnatomyContextWindowManager windowManager;
        private Anatomy anatomy;

        public ShowTasksAnatomyCommand(AnatomyContextWindowManager windowManager, Anatomy anatomy)
        {
            this.windowManager = windowManager;
            this.anatomy = anatomy;
        }

        public bool BooleanValue
        {
            get
            {
                return false;
            }

            set
            {
                
            }
        }

        public bool DisplayInGroup
        {
            get
            {
                return false;
            }
        }

        public float NumericValue
        {
            get
            {
                return 0;
            }

            set
            {
                
            }
        }

        public float NumericValueMax
        {
            get
            {
                return 0;
            }
        }

        public float NumericValueMin
        {
            get
            {
                return 0;
            }
        }

        public bool ShowAnatomyFinder
        {
            get
            {
                return false;
            }
        }

        public string UIText
        {
            get
            {
                return "Show Tasks";
            }
        }

        public AnatomyCommandUIType UIType
        {
            get
            {
                return AnatomyCommandUIType.Executable;
            }
        }

        public event AnatomyBooleanValueChanged BooleanValueChanged;
        public event AnatomyNumericValueChanged NumericValueChanged;

        public bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            return true;
        }

        public EditInterface createEditInterface()
        {
            return null;
        }

        public void destroy()
        {
            
        }

        public void execute()
        {
            windowManager.showTaskMenuFor(anatomy);
        }

        public void getInfo(SaveInfo info)
        {
            
        }

        public bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref string errorMessage)
        {
            return true;
        }
    }
}
