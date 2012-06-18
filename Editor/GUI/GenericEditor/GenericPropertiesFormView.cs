using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.GUI
{
    class GenericPropertiesFormView : MyGUIView
    {
        [DoNotSave]
        private List<Tuple<Type, PropertiesForm.CreateComponent>> customCreationMethods = new List<Tuple<Type, PropertiesForm.CreateComponent>>();

        public GenericPropertiesFormView(String name, EditInterface editInterface, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Right;
        }

        public void addCustomForm(Type type, PropertiesForm.CreateComponent creationMethod)
        {
            customCreationMethods.Add(Tuple.Create(type, creationMethod));
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        public IEnumerable<Tuple<Type, PropertiesForm.CreateComponent>> CustomCreationMethods
        {
            get
            {
                return customCreationMethods;
            }
        }

        protected GenericPropertiesFormView(LoadInfo info)
            : base(info)
        {

        }
    }
}
