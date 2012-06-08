using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    class EditorInfoBarView : MyGUIView
    {
        private List<EditorInfoBarAction> actions = new List<EditorInfoBarAction>();

        public EditorInfoBarView(String name, String caption, String closeAction = null)
            :base(name)
        {
            this.Caption = caption;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Top;
            this.CloseAction = closeAction;
        }

        public void addAction(EditorInfoBarAction action)
        {
            actions.Add(action);
        }

        public void removeAction(EditorInfoBarAction action)
        {
            actions.Remove(action);
        }

        public String Caption { get; set; }

        public String CloseAction { get; set; }

        public IEnumerable<EditorInfoBarAction> Actions
        {
            get
            {
                return actions;
            }
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList("Action", actions);
        }

        protected EditorInfoBarView(LoadInfo info)
            : base(info)
        {
            info.RebuildList("Action", actions);
        }
    }
}
