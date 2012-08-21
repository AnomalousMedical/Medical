using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AddItemDialog : Dialog
    {
        public static void AddItem(IEnumerable<AddItemTemplate> itemTemplates, Action<AddItemTemplate> createItemCallback)
        {
            AddItemDialog addItemDialog = new AddItemDialog();

            foreach (AddItemTemplate itemTemplate in itemTemplates)
            {
                addItemDialog.addItemTemplate(itemTemplate);
            }

            addItemDialog.CreateItem += (itemTemplate) =>
            {
                createItemCallback(itemTemplate);
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
        public event Action<AddItemTemplate> CreateItem;

        protected AddItemDialog()
            :base("Medical.GUI.Editor.AddItemDialog.AddItemDialog.layout")
        {
            itemGrid = new ButtonGrid((ScrollView)window.findWidget("ItemList"), new ButtonGridListLayout());
            itemGrid.SelectedValueChanged += new EventHandler(itemGrid_SelectedValueChanged);

            expandingView = new ScrollingExpandingEditInterfaceViewer((ScrollView)window.findWidget("ItemProperties"), null);

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

        public void addItemTemplate(AddItemTemplate itemTemplate)
        {
            itemTemplate.reset();
            ButtonGridItem item = itemGrid.addItem(itemTemplate.Group, itemTemplate.TypeName, itemTemplate.ImageName);
            item.UserObject = itemTemplate;
            if (itemGrid.SelectedItem == null)
            {
                itemGrid.SelectedItem = item;
            }
        }

        void itemGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            expandingView.EditInterface = ((AddItemTemplate)itemGrid.SelectedItem.UserObject).EditInterface;
            expandingView.layout();
        }

        void add_MouseButtonClick(Widget source, EventArgs e)
        {
            AddItemTemplate itemTemplate = (AddItemTemplate)itemGrid.SelectedItem.UserObject;
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
