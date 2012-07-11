using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Presentation;
using MyGUIPlugin;

namespace Medical.GUI
{
    class SlideIndex : LayoutComponent
    {
        private PresentationIndex presentation;
        private ButtonGrid buttonGrid;
        private int lastWidth = -1;
        private int lastHeight = -1;

        public SlideIndex(MyGUIViewHost viewHost, SlideIndexView view)
            : base("Medical.GUI.SlideIndex.SlideIndex.layout", viewHost)
        {
            this.presentation = view.Presentation;
            presentation.EntryAdded += presentation_EntryAdded;
            presentation.EntryRemoved += presentation_EntryRemoved;

            buttonGrid = new ButtonGrid((ScrollView)widget, new ButtonGridListLayout());

            foreach (PresentationEntry entry in presentation.Entries)
            {
                addEntryToButtonGrid(entry);
            }
        }

        public override void Dispose()
        {
            presentation.EntryAdded -= presentation_EntryAdded;
            presentation.EntryRemoved -= presentation_EntryRemoved;
            buttonGrid.Dispose();
            base.Dispose();
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            //Layout only if size changes
            if (widget.Width != lastWidth || widget.Height != lastHeight)
            {
                lastWidth = widget.Width;
                lastHeight = widget.Height;
                buttonGrid.layout();
            }
        }

        void presentation_EntryAdded(PresentationEntry obj)
        {
            addEntryToButtonGrid(obj);
        }

        void presentation_EntryRemoved(PresentationEntry obj)
        {
            removeEntryFromButtonGrid(obj);
        }

        private void addEntryToButtonGrid(PresentationEntry entry)
        {
            ButtonGridItem item = buttonGrid.addItem("", entry.UniqueName);
            item.UserObject = entry;
        }

        private void removeEntryFromButtonGrid(PresentationEntry entry)
        {
            ButtonGridItem item = buttonGrid.findItemByUserObject(entry);
            buttonGrid.removeItem(item);
        }
    }
}
