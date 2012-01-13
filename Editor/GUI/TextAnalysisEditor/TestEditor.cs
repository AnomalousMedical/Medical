using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TestEditor : AnalysisEditorComponent
    {
        private const String GREATER_THAN = ">";
        private const String GREATER_THAN_EQUAL = ">=";
        private const String LESS_THAN = "<";
        private const String LESS_THAN_EQUAL = "<=";
        private const String EQUAL = "=";
        private const String NOT_EQUAL = "!=";
        private const String IS_TRUE = "Is True";

        private Button variableButton;
        private StaticText successText;
        private StaticText failText;
        private DataFieldInfo fieldInfo;
        private ComboBox conditionCombo;
        private Edit testValueEdit;

        private ActionBlockEditor successEditor;
        private ActionBlockEditor failEditor;

        private int baseHeight;
        private int minTestValueFieldWidth;

        public TestEditor(AnalysisEditorComponentParent parent)
            :base("Medical.GUI.TextAnalysisEditor.TestEditor.layout", parent)
        {
            baseHeight = widget.Height;

            variableButton = (Button)widget.findWidget("VariableButton");
            variableButton.MouseButtonClick += new MyGUIEvent(variableButton_MouseButtonClick);

            successText = (StaticText)widget.findWidget("SuccessText");
            failText = (StaticText)widget.findWidget("FailText");
            testValueEdit = (Edit)widget.findWidget("TestValueEdit");
            minTestValueFieldWidth = testValueEdit.Width;

            conditionCombo = (ComboBox)widget.findWidget("ConditionCombo");
            conditionCombo.addItem(GREATER_THAN);
            conditionCombo.addItem(GREATER_THAN_EQUAL);
            conditionCombo.addItem(LESS_THAN);
            conditionCombo.addItem(LESS_THAN_EQUAL);
            conditionCombo.addItem(EQUAL);
            conditionCombo.addItem(NOT_EQUAL);
            conditionCombo.addItem(IS_TRUE);

            successEditor = new ActionBlockEditor("Success", this);
            failEditor = new ActionBlockEditor("Failure", this);
        }

        public TestEditor(AnalysisEditorComponentParent parent, TestAction action)
            :this(parent)
        {
            if (action is GreaterThanTest)
            {
                GreaterThanTest test = (GreaterThanTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, GREATER_THAN, test.TestValue.ToString());
            }
            else if (action is GreaterThanEqualTest)
            {
                GreaterThanEqualTest test = (GreaterThanEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, GREATER_THAN_EQUAL, test.TestValue.ToString());
            }
            else if (action is LessThanTest)
            {
                LessThanTest test = (LessThanTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, LESS_THAN, test.TestValue.ToString());
            }
            else if (action is LessThanEqualTest)
            {
                LessThanEqualTest test = (LessThanEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, LESS_THAN_EQUAL, test.TestValue.ToString());
            }
            else if (action is DecimalEqualTest)
            {
                DecimalEqualTest test = (DecimalEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, EQUAL, test.TestValue.ToString());
            }
            else if (action is StringEqualTest)
            {
                StringEqualTest test = (StringEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, EQUAL, test.TestValue);
            }
            else if (action is DecimalNotEqualTest)
            {
                DecimalNotEqualTest test = (DecimalNotEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, NOT_EQUAL, test.TestValue.ToString());
            }
            else if (action is StringNotEqualTest)
            {
                StringNotEqualTest test = (StringNotEqualTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, NOT_EQUAL, test.TestValue);
            }
            else if (action is TrueTest)
            {
                TrueTest test = (TrueTest)action;
                DataRetriever dataRetriever = test.Data;
                setInfo(dataRetriever, IS_TRUE, "");
            }
            ActionBlock successAction = action.SuccessAction as ActionBlock;
            if (successAction != null)
            {
                successEditor.createFromAnalyzer(successAction);
            }
            ActionBlock failureAction = action.FailureAction as ActionBlock;
            if (failureAction != null)
            {
                failEditor.createFromAnalyzer(failureAction);
            }
        }

        public override void Dispose()
        {
            successEditor.Dispose();
            failEditor.Dispose();
            base.Dispose();
        }

        public override void layout(int left, int top, int width)
        {
            int variableButtonWidth = (int)variableButton.getTextSize().Width;
            if (variableButtonWidth + conditionCombo.Width + minTestValueFieldWidth > width)
            {
                variableButtonWidth = width - (conditionCombo.Width + minTestValueFieldWidth);
            }
            variableButton.setSize(variableButtonWidth, variableButton.Height);
            conditionCombo.setPosition(variableButton.Right, conditionCombo.Top);
            testValueEdit.setCoord(conditionCombo.Right, testValueEdit.Top, width - conditionCombo.Right, testValueEdit.Height);

            int indent = left + 15;
            int indentWidth = width - indent;
            successEditor.layout(indent, successText.Bottom, indentWidth);
            failText.setPosition(failText.Left, successEditor.Bottom);
            failEditor.layout(indent, failText.Bottom, indentWidth);
            widget.setCoord(left, top, width, baseHeight + successEditor.Height + failEditor.Height);
        }

        public override AnalysisAction createAction()
        {
            if (fieldInfo == null)
            {
                throw new AnalysisCompilationError("Field not specified for test.");
            }
            DataRetriever dataRetriever = fieldInfo.createDataRetriever();
            switch (conditionCombo.SelectedItemName)
            {
                case GREATER_THAN:
                    return new GreaterThanTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                case GREATER_THAN_EQUAL:
                    return new GreaterThanEqualTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                case LESS_THAN:
                    return new LessThanTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                case LESS_THAN_EQUAL:
                    return new LessThanEqualTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                case EQUAL:
                    try
                    {
                        return new DecimalEqualTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                    }
                    catch (Exception)
                    {
                        return new StringEqualTest(successEditor.createAction(), failEditor.createAction(), testValueEdit.OnlyText, dataRetriever, "");
                    }
                case NOT_EQUAL:
                    try
                    {
                        return new DecimalNotEqualTest(successEditor.createAction(), failEditor.createAction(), decimal.Parse(testValueEdit.OnlyText), dataRetriever, 0);
                    }
                    catch (Exception)
                    {
                        return new StringNotEqualTest(successEditor.createAction(), failEditor.createAction(), testValueEdit.OnlyText, dataRetriever, "");
                    }
                case IS_TRUE:
                    return new TrueTest(successEditor.createAction(), failEditor.createAction(), dataRetriever, false);
            }
            throw new NotImplementedException();
        }

        void variableButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Parent.openVariableBrowser(delegate(DataFieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;
                variableButton.Caption = fieldInfo.FullName;
                requestLayout();
            });
        }

        private void setInfo(DataRetriever dataRetriever, String fieldSelection, String testValue)
        {
            conditionCombo.SelectedIndex = conditionCombo.findItemIndexWith(fieldSelection);
            StringBuilder sb = new StringBuilder();
            foreach (String section in dataRetriever.ExamSections)
            {
                sb.Append(".");
                sb.Append(section);
            }
            sb.Remove(0, 1);
            fieldInfo = new DataFieldInfo(sb.ToString(), dataRetriever.DataPoint);
            variableButton.Caption = fieldInfo.FullName;
            testValueEdit.Caption = testValue;
        }
    }
}
