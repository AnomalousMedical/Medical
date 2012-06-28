using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Editing;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using System.Reflection;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    public class ViewCollection : SaveableEditableItemCollection<View>
    {
        public delegate View CreateView(String name);

        private static readonly Type[] constructorArgs = { typeof(String) };

        public ViewCollection()
        {
            
        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<View> itemEdits)
        {
            editInterface.IconReferenceTag = "MvcContextEditor/OverallViewIcon";
            editInterface.addCommand(new EditInterfaceCommand("Add View", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                callback.runCustomQuery(CustomQueries.CreateViewBrowser, delegate(Browser browser, ref string errorPrompt)
                {
                    callback.showInputBrowser(browser, delegate(CreateView createView, String name, ref string err)
                    {
                        if (!hasItem(name))
                        {
                            add(createView(name));
                            return true;
                        }
                        err = String.Format("A view named {0} already exists. Please input another name.", name);
                        return false;
                    });
                    return true;
                });
            }));
        }

        public enum CustomQueries
        {
            CreateViewBrowser
        }

        protected ViewCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
