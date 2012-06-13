using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class EditInterfaceHandler : MvcModel
    {
        public const String DefaultName = "EditInterfaceHandler";

        [DoNotSave]
        private EditInterfaceConsumer consumer;

        public EditInterfaceHandler(String name = DefaultName)
            :base(name)
        {

        }

        public void setEditInterfaceConsumer(EditInterfaceConsumer consumer)
        {
            this.consumer = consumer;
        }

        public void changeEditInterface(EditInterface editInterface)
        {
            if (consumer != null)
            {
                consumer.CurrentEditInterface = editInterface;
            }
        }

        protected EditInterfaceHandler(LoadInfo info)
            :base(info)
        {

        }
    }
}
