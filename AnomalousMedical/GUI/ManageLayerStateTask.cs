using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class ManageLayerStateTask : Task, IDisposable
    {
        private PopupMenu popupMenu;
        private LayerController layerController;

        public ManageLayerStateTask(LayerController layerController)
            : base("Medical.ManageLayerStateTask", "Manage Layer State", CommonResources.NoIcon, "Navigation")
        {
            this.layerController = layerController;

            popupMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            popupMenu.Visible = false;

            MenuItem sequenceItem = popupMenu.addItem("Undo", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => layerController.undo();

            sequenceItem = popupMenu.addItem("Redo", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => layerController.redo();
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(popupMenu);
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            popupMenu.setVisibleSmooth(true);
            LayerManager.Instance.upLayerItem(popupMenu);
            IntVector2 loc = taskPositioner.findGoodWindowPosition(popupMenu.Width, popupMenu.Height);
            popupMenu.setPosition(loc.x, loc.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }
    }
}
