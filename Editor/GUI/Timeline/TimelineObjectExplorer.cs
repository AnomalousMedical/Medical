using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineObjectExplorer : MDIDialog
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        public TimelineObjectExplorer(MedicalUICallback medicalUICallback)
            : base("Medical.GUI.Timeline.TimelineObjectExplorer.layout")
        {
            tree = new Tree((ScrollView)window.findWidget("ScrollView"));
            editTreeView = new EditInterfaceTreeView(tree, medicalUICallback);

            this.Resized += new EventHandler(TimelineObjectExplorer_Resized);
        }

        public override void Dispose()
        {
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        public EditInterfaceTreeView EditInterfaceTree
        {
            get
            {
                return editTreeView;
            }
        }

        public bool Enabled
        {
            get
            {
                return tree.Enabled;
            }
            set
            {
                tree.Enabled = value;
            }
        }

        void TimelineObjectExplorer_Resized(object sender, EventArgs e)
        {
            tree.layout();
        }
    }
}
