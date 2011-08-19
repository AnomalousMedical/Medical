using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ExamAnalyzerMenu : IDisposable
    {
        public event EventDelegate<ExamAnalyzerMenu, ExamAnalyzer> RunExamAnalyzer;

        public event EventDelegate<ExamAnalyzerMenu> Closed;

        private PopupMenu popupMenu;

        public ExamAnalyzerMenu(ExamAnalyzerCollection analyzers)
        {
            popupMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 10, 10, Align.Default, "Overlapped", "");
            foreach (ExamAnalyzer analyzer in analyzers.Analyzers)
            {
                MenuItem item = popupMenu.addItem(analyzer.Name);
                item.UserObject = analyzer;
            }
            popupMenu.ItemAccept += new MyGUIEvent(popupMenu_ItemAccept);
            popupMenu.Closed += new MyGUIEvent(popupMenu_Closed);
        }

        public void Dispose()
        {
            if (popupMenu != null)
            {
                Gui.Instance.destroyWidget(popupMenu);
                popupMenu = null;
            }
        }

        public void addHistory(Exam mostRecent)
        {
            Exam next = mostRecent.PreviousExam;
            while (next != null)
            {
                MenuItem item = popupMenu.addItem(String.Format("{0} {1}", next.PrettyName, next.Date.ToString()));
                item.UserObject = RawDataAnalyzer.Instance;
                next = next.PreviousExam;
            }
        }

        public void show(int left, int top)
        {
            popupMenu.setPosition(left, top);
            LayerManager.Instance.upLayerItem(popupMenu);
            popupMenu.Visible = true;
        }

        void popupMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcaea = (MenuCtrlAcceptEventArgs)e;
            if (RunExamAnalyzer != null)
            {
                RunExamAnalyzer.Invoke(this, (ExamAnalyzer)mcaea.Item.UserObject);
            }
        }

        void popupMenu_Closed(Widget source, EventArgs e)
        {
            if (Closed != null)
            {
                Closed.Invoke(this);
            }
        }
    }
}
