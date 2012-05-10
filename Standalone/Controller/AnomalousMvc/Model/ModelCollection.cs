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
    public class ModelCollection : SaveableEditableItemCollection<Model>
    {
        private static readonly Type[] constructorArgs = { typeof(String) };

        public ModelCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<Model> itemEdits)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Model", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                ConstructorInfo constructor = null;
                callback.runCustomQuery(CustomQueries.ShowModelBrowser, delegate(Object result, ref string errorPrompt)
                {
                    Type returnedTypeInfo = result as Type;
                    if (returnedTypeInfo != null)
                    {
                        constructor = returnedTypeInfo.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, constructorArgs, null);
                        if (constructor != null)
                        {
                            callback.getInputString("Enter a name.", delegate(String input, ref String err)
                            {
                                if (!hasItem(input))
                                {
                                    add((Model)constructor.Invoke(new Object[] { input }));
                                    return true;
                                }
                                err = String.Format("An item named {0} already exists. Please input another name.", input);
                                return false;
                            });
                            return true;
                        }
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
