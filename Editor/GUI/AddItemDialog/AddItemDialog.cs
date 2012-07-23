using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class AddItemDialog : Dialog
    {
        public static void AddItem(String path, EditorController editorController, MedicalUICallback uiCallback)
        {
            AddItemDialog addItemDialog = new AddItemDialog(uiCallback);

            foreach (EditorTypeController typeController in editorController.TypeControllers)
            {
                ProjectItemTemplate itemTemplate = typeController.createItemTemplate();
                if (itemTemplate != null)
                {
                    addItemDialog.addItemTemplate(itemTemplate);
                }
            }

            addItemDialog.CreateItem += (itemTemplate) =>
            {
                itemTemplate.createItem(path, editorController);
            };

            addItemDialog.Closed += (sender, e) =>
            {
                addItemDialog.Dispose();
            };

            addItemDialog.center();
            addItemDialog.ensureVisible();
            addItemDialog.open(true);
        }

        private ScrollingExpandingEditInterfaceViewer expandingView;
        private ButtonGrid itemGrid;
        public event Action<ProjectItemTemplate> CreateItem;

        protected AddItemDialog(MedicalUICallback uiCallback)
            :base("Medical.GUI.AddItemDialog.AddItemDialog.layout")
        {
            itemGrid = new ButtonGrid((ScrollView)window.findWidget("ItemList"), new ButtonGridListLayout());
            itemGrid.SelectedValueChanged += new EventHandler(itemGrid_SelectedValueChanged);

            expandingView = new ScrollingExpandingEditInterfaceViewer((ScrollView)window.findWidget("ItemProperties"), uiCallback);

            Button add = (Button)window.findWidget("Add");
            add.MouseButtonClick += new MyGUIEvent(add_MouseButtonClick);

            Button cancel = (Button)window.findWidget("Cancel");
            cancel.MouseButtonClick += new MyGUIEvent(cancel_MouseButtonClick);
        }

        public override void Dispose()
        {
            expandingView.Dispose();
            base.Dispose();
        }

        public void addItemTemplate(ProjectItemTemplate itemTemplate)
        {
            ButtonGridItem item = itemGrid.addItem(itemTemplate.Group, itemTemplate.TypeName, itemTemplate.ImageName);
            item.UserObject = itemTemplate;
            if (itemGrid.SelectedItem == null)
            {
                itemGrid.SelectedItem = item;
            }
        }

        void itemGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            expandingView.EditInterface = ((ProjectItemTemplate)itemGrid.SelectedItem.UserObject).EditInterface;
            expandingView.layout();
        }

        void add_MouseButtonClick(Widget source, EventArgs e)
        {
            ProjectItemTemplate itemTemplate = (ProjectItemTemplate)itemGrid.SelectedItem.UserObject;
            String error;
            if (itemTemplate.isValid(out error))
            {
                if (CreateItem != null)
                {
                    CreateItem.Invoke(itemTemplate);
                }
                this.close();
            }
            else
            {
                MessageBox.show(error, "Add Item Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void cancel_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }
    }
}
