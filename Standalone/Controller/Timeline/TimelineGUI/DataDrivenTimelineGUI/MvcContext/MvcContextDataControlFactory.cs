using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using System.IO;

namespace Medical
{
    class MvcContextDataControlFactory : DataControlFactory
    {
        private AnomalousMvcContext context;
        private StringBuilder rmlStringBuilder;
        private MvcController controller;

        public MvcContextDataControlFactory(AnomalousMvcContext context, StringBuilder rmlStringBuilder, MvcController controller)
        {
            this.context = context;
            this.rmlStringBuilder = rmlStringBuilder;
            this.controller = controller;
        }

        public void pushColumnLayout()
        {
            
        }

        public void popColumnLayout()
        {
            
        }

        public void addField(BooleanDataField field)
        {
            
        }

        public void addField(MenuItemField field)
        {
            //rmlStringBuilder.AppendFormat("<a onclick='{0}'>{1}</a>", field.
        }

        public void addField(MultipleChoiceField field)
        {
            
        }

        public void addField(NotesDataField field)
        {
            
        }

        public void addField(NumericDataField field)
        {
            
        }

        public void addField(PlayExampleDataField field)
        {
            String actionName = Path.GetFileNameWithoutExtension(field.Timeline);
            RunCommandsAction runAction = new RunCommandsAction(actionName);
            runAction.addCommand(new PlayTimelineCommand(field.Timeline));
            controller.Actions.add(runAction);

            String rml = @"
            <li>
                <a onclick='{0}'>{1}</a>
            </li>";
            rmlStringBuilder.AppendFormat(rml, String.Format("{0}/{1}", controller.Name, actionName), field.Name);
        }

        public void addField(StaticTextDataField field)
        {
            
        }

        public void addField(CloseGUIPlayTimelineField field)
        {
            
        }

        public void addField(DoActionsDataField field)
        {
            
        }
    }
}
