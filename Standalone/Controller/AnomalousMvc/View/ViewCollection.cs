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
        private static readonly Type[] constructorArgs = { typeof(String) };

        public ViewCollection()
        {
            
        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<View> itemEdits)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add View", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                callback.runCustomQuery(CustomQueries.CreateViewBrowser, delegate(Browser browser, ref string errorPrompt)
                {
                    callback.showInputBrowser("Choose a view", browser, delegate(Type returnedTypeInfo, String name, ref string err)
                    {
                        if (!hasItem(name))
                        {
                            add((View)Activator.CreateInstance(returnedTypeInfo, name));
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
            /// <summary>
            /// Input: no args
            /// Output: 
            /// </summary>
            CreateViewBrowser
        }

        protected ViewCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
