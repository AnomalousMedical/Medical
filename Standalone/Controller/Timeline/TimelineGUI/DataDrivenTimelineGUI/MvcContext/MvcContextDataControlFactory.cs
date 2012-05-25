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
            throw new NotImplementedException();
        }

        public void addField(MenuItemField field)
        {
            if(field.Timelines.Count() > 1)
            {
                throw new Exception("Field timlines > 1, convert manually");
            }
            String otherControllerName = Path.GetFileNameWithoutExtension(field.Timelines.Single().Timeline);
            String rml = @"
            <li>
                <a onclick='{0}'>{1}</a>
            </li>";
            rmlStringBuilder.AppendFormat(rml, String.Format("{0}/Show", otherControllerName), field.Name);
        }

        public void addField(MultipleChoiceField field)
        {
            throw new NotImplementedException();
        }

        public void addField(NotesDataField field)
        {
            throw new NotImplementedException();
        }

        public void addField(NumericDataField field)
        {
            throw new NotImplementedException();
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
            String rml;
            if (field.FontHeight == 30)
            {
                rml = @"
            <h1>{0}</h1>";
            }
            else if (String.IsNullOrWhiteSpace(field.Text))
            {
                rml = @"
            <br/>";
            }
            else if (field.Text.StartsWith("#707070"))
            {
                field.Text = field.Text.Replace("#707070", "");
                rml = @"
            <p class=""Citation"">
                {0}
            </p>";
            }
            else
            {
                rml = @"
            <p>
                {0}
            </p>";
            }
            rmlStringBuilder.AppendFormat(rml, field.Text);
        }

        public void addField(CloseGUIPlayTimelineField field)
        {
            throw new NotImplementedException();
        }

        public void addField(DoActionsDataField field)
        {
            throw new NotImplementedException();
        }
    }
}
