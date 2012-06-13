using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    public class PropertiesForm : PropertyEditor, IDisposable
    {
        private List<PropertiesFormComponent> components = new List<PropertiesFormComponent>();
        private StretchLayoutContainer flowLayout = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, 0, new IntVector2(0, 0));
        private ScrollView scrollView;
        private EditUICallback uiCallback;
        private EditInterface currentEditInterface;
        private bool allowValidation = true;
        private EditablePropertyInfo currentPropInfo;

        public PropertiesForm(ScrollView scrollView, EditUICallback uiCallback)
        {
            this.uiCallback = uiCallback;
            this.scrollView = scrollView;
            //propertiesTable.CellValidating += new EventHandler<TableCellValidationEventArgs>(propertiesTable_CellValidating);
            //propertiesTable.CellValueChanged += new EventHandler(propertiesTable_CellValueChanged);
            //addRemoveButtons = buttons;
            //if (addRemoveButtons != null)
            //{
            //    addRemoveButtons.AddButtonClicked += addRemoveButtons_AddButtonClicked;
            //    addRemoveButtons.RemoveButtonClicked += addRemoveButtons_RemoveButtonClicked;
            //}
        }

        public void Dispose()
        {
            clear();
        }

        public void clear()
        {
            flowLayout.clearChildren();
            foreach (PropertiesFormComponent component in components)
            {
                component.Dispose();
            }
            components.Clear();
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditInterface;
            }
            set
            {
                allowValidation = false;
                if (currentEditInterface != null)
                {
                    currentEditInterface.OnPropertyAdded -= new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved -= new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                }
                clear();
                if (value != null)
                {
                    currentEditInterface = value;
                    currentEditInterface.OnPropertyAdded += new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved += new PropertyRemoved(currentEditInterface_OnPropertyRemoved);

                    //if (addRemoveButtons != null)
                    //{
                    //    addRemoveButtons.Visible = currentEditInterface.canAddRemoveProperties();
                    //}
                    currentPropInfo = value.getPropertyInfo();
                    if (currentPropInfo != null)
                    {
                        //foreach (EditablePropertyColumn column in currentPropInfo.getColumns())
                        //{
                        //    TableColumn dgvColumn = new TableColumn(column.Header);
                        //    dgvColumn.ReadOnly = column.ReadOnly;
                        //    propertiesTable.Columns.add(dgvColumn);
                        //}
                    }
                    if (value.hasEditableProperties())
                    {
                        foreach (EditableProperty editProp in value.getEditableProperties())
                        {
                            addProperty(editProp);
                        }
                    }
                    layout();
                }
                else
                {
                    currentPropInfo = null;
                    currentEditInterface = null;
                }
                allowValidation = true;
            }
        }

        void currentEditInterface_OnPropertyRemoved(EditableProperty property)
        {
            removeProperty(property);
            layout();
        }

        void currentEditInterface_OnPropertyAdded(EditableProperty property)
        {
            addProperty(property);
            layout();
        }

        private void addProperty(EditableProperty property)
        {
            PropertiesFormComponent component = createComponenet(property);
            components.Add(component);
            flowLayout.addChild(component.Container);
        }

        private void removeProperty(EditableProperty property)
        {
            foreach (PropertiesFormComponent component in components)
            {
                if (component.Property == property)
                {
                    components.Remove(component);
                    flowLayout.removeChild(component.Container);
                    component.Dispose();
                    break;
                }
            }
        }

        private PropertiesFormComponent createComponenet(EditableProperty property)
        {
            Type propertyType = property.getPropertyType(1);
            if (propertyType == typeof(bool))
            {
                return new PropertiesFormCheckBox(property, scrollView);
            }
            else if (propertyType == typeof(Vector3))
            {
                return new PropertiesFormVector3(property, scrollView);
            }
            else if (propertyType == typeof(Quaternion))
            {
                return new PropertiesFormEulerQuat(property, scrollView);
            }

            //No match, create an appropriate text box
            else if (property.hasBrowser(1))
            {
                return new PropertiesFormTextBoxBrowser(property, scrollView, uiCallback);
            }
            return new PropertiesFormTextBox(property, scrollView);
        }

        public void layout()
        {
            int width = scrollView.ViewCoord.width;
            int height = flowLayout.DesiredSize.Height;
            flowLayout.WorkingSize = new IntSize2(width, height);
            flowLayout.layout();
            scrollView.CanvasSize = flowLayout.WorkingSize;
        }
    }
}
