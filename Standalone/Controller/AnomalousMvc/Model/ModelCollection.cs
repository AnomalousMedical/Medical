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
                callback.runCustomQuery(CustomQueries.ShowModelBrowser, delegate(Object result, ref string errorPrompt)
                {
                    ModelCreationInfo returnedTypeInfo = result as ModelCreationInfo;
                    if (returnedTypeInfo != null)
                    {
                        //Try to add with the default name.
                        if (!hasItem(returnedTypeInfo.DefaultName))
                        {
                            add(returnedTypeInfo.createModel(returnedTypeInfo.DefaultName));
                        }
                        else
                        {
                            //Default name in use (second instance of model). Ask user for new name.
                            callback.getInputString("Enter a name.", delegate(String input, ref String err)
                            {
                                if (!hasItem(input))
                                {
                                    add(returnedTypeInfo.createModel(input));
                                    return true;
                                }
                                err = String.Format("An item named {0} already exists. Please input another name.", input);
                                return false;
                            });
                        }
                        return true;
                    }
                    return false;
                });
            }));
        }

        public enum CustomQueries
        {
            ShowModelBrowser
        }

        protected ModelCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
