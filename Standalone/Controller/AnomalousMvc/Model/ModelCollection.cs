using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using System.Reflection;

namespace Medical.Controller.AnomalousMvc
{
    public class ModelCollection : SaveableEditableItemCollection<MvcModel>
    {
        private static readonly Type[] constructorArgs = { typeof(String) };

        public ModelCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<MvcModel> itemEdits)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Model", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                callback.runCustomQuery(CustomQueries.CreateModelBrowser, delegate(Browser modelBrowser, ref string errorPrompt)
                {
                    callback.showInputBrowser(modelBrowser, delegate(Type returnedTypeInfo, String name, ref string error)
                    {
                        if (!hasItem(name))
                        {
                            add((MvcModel)Activator.CreateInstance(returnedTypeInfo, name));
                            return true;
                        }
                        error = String.Format("A model named {0} already exists. Please input another name.", name);
                        return false;
                    });
                    return true;
                });
            }));
        }

        public enum CustomQueries
        {
            CreateModelBrowser
        }

        protected ModelCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
