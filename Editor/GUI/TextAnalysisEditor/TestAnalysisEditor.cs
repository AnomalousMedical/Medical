﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TestAnalysisEditor : AnalysisEditorComponent
    {
        private const String GREATER_THAN = ">";
        private const String GREATER_THAN_EQUAL = ">=";
        private const String LESS_THAN = "<";
        private const String LESS_THAN_EQUAL = "<=";
        private const String EQUAL = "=";
        private const String IS_TRUE = "Is True";

        private StaticText variableName;
        private StaticText successText;
        private StaticText failText;
        private DataFieldInfo fieldInfo;
        private ComboBox conditionCombo;
        private Edit testValueEdit;

        private ActionBlockEditor successEditor;
        private ActionBlockEditor failEditor;

        private int baseHeight;

        public TestAnalysisEditor(AnalysisEditorComponentParent parent)
            :base("Medical.GUI.TextAnalysisEditor.TestAnalysisEditor.layout", parent)
        {
            baseHeight = widget.Height;

            Button choose = (Button)widget.findWidget("Choose");
            choose.MouseButtonClick += new MyGUIEvent(choose_MouseButtonClick);

            variableName = (StaticText)widget.findWidget("VariableName");
            successText = (StaticText)widget.findWidget("SuccessText");
            failText = (StaticText)widget.findWidget("FailText");
            testValueEdit = (Edit)widget.findWidget("TestValueEdit");

            conditionCombo = (ComboBox)widget.findWidget("ConditionCombo");
            conditionCombo.addItem(GREATER_THAN);
            conditionCombo.addItem(GREATER_THAN_EQUAL);
            conditionCombo.addItem(LESS_THAN);
            conditionCombo.addItem(LESS_THAN_EQUAL);
            conditionCombo.addItem(EQUAL);
            conditionCombo.addItem(IS_TRUE);

            successEditor = new ActionBlockEditor(this);
            successEditor.Removeable = false;
            failEditor = new ActionBlockEditor(this);
            failEditor.Removeable = false;
        }

        public override void Dispose()
        {
            successEditor.Dispose();
            failEditor.Dispose();
            base.Dispose();
        }

        public override void layout(int left, int top, int width)
        {
            int indent = left + 15;
            int indentWidth = width - indent;
            successEditor.layout(indent, successText.Bottom, indentWidth);
            failText.setPosition(failText.Left, successEditor.Bottom);
            failEditor.layout(indent, failText.Bottom, indentWidth);
            widget.setCoord(left, top, width, baseHeight + successEditor.Height + failEditor.Height);
        }

        public override AnalysisAction createAction()
        {
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
                case IS_TRUE:
                    return new TrueTest(successEditor.createAction(), failEditor.createAction(), dataRetriever, false);
            }
            throw new NotImplementedException();
        }

        void choose_MouseButtonClick(Widget source, EventArgs e)
        {
            Parent.openVariableBrowser(delegate(DataFieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;
                variableName.Caption = fieldInfo.FullName;
            });
        }
    }
}
