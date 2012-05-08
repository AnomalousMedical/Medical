using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Editing;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using System.Reflection;

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
                ConstructorInfo constructor = null;
                callback.runCustomQuery(CustomQueries.ShowViewBrowser, delegate(Object result, ref string errorPrompt)
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
                                    add((View)constructor.Invoke(new Object[] { input }));
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
            ShowViewBrowser
        }

        protected ViewCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
