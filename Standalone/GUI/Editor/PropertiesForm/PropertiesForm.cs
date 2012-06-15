using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Attributes;
using System.Reflection;

namespace Medical.GUI
{
    public class PropertiesForm : PropertyEditor, IDisposable
    {
        private List<PropertiesFormComponent> components = new List<PropertiesFormComponent>();
        private StretchLayoutContainer flowLayout = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, 3, new IntVector2(0, 0));
        private ScrollView scrollView;
        private EditUICallback uiCallback;
        private EditInterface currentEditInterface;
        private EditablePropertyInfo currentPropInfo;

        public PropertiesForm(ScrollView scrollView, EditUICallback uiCallback)
        {
            this.uiCallback = uiCallback;
            this.scrollView = scrollView;
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

        public void layout()
        {
            //Adjust the scrollbar height first to get any scrolbars active
            int height = flowLayout.DesiredSize.Height;
            scrollView.CanvasSize = new IntSize2(0, height);

            int width = scrollView.ViewCoord.width;
            flowLayout.WorkingSize = new IntSize2(width, height);
            flowLayout.layout();
            scrollView.CanvasSize = flowLayout.WorkingSize;
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditInterface;
            }
            set
            {
                if (currentEditInterface != null)
                {
                    currentEditInterface.OnPropertyAdded -= new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved -= new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                    currentEditInterface.OnDataNeedsRefresh -= new EditInterfaceEvent(currentEditInterface_OnDataNeedsRefresh);
                }
                clear();
                if (value != null)
                {
                    currentEditInterface = value;
                    currentEditInterface.OnPropertyAdded += new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved += new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                    currentEditInterface.OnDataNeedsRefresh += new EditInterfaceEvent(currentEditInterface_OnDataNeedsRefresh);
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
                    foreach (EditInterfaceCommand command in currentEditInterface.getCommands())
                    {
                        PropertiesFormComponent component = new PropertiesFormButton(command, uiCallback, scrollView);
                        components.Add(component);
                        flowLayout.addChild(component.Container);
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
            }
        }

        void currentEditInterface_OnDataNeedsRefresh(EditInterface editInterface)
        {
            foreach (PropertiesFormComponent component in components)
            {
                component.refreshData();
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
            //Have to do this because we cannot "out" a method using trygetvalue
            if (DefaultCreationMethods.ContainsKey(propertyType))
            {
                return DefaultCreationMethods[propertyType].Invoke(property, scrollView);
            }
            else if (propertyType.IsEnum)
            {
                if (propertyType.GetCustomAttributes(typeof(SingleEnumAttribute), true).Length > 0)
                {
                    PropertiesFormComboBox editorCell = new PropertiesFormComboBox(property, scrollView, propertyType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(fieldInfo => fieldInfo.Name));
                    return editorCell;
                }
                //else if (propertyType.GetCustomAttributes(typeof(MultiEnumAttribute), true).Length > 0)
                //{
                //    MultiEnumEditorCell editorCell = new MultiEnumEditorCell();
                //    editorCell.EnumType = propType;
                //    return editorCell;
                //}
            }
            
            //No match, create an appropriate text box
            if (property.hasBrowser(1))
            {
                return new PropertiesFormTextBoxBrowser(property, scrollView, uiCallback);
            }
            return new PropertiesFormTextBox(property, scrollView);
        }

        private delegate PropertiesFormComponent CreateComponent(EditableProperty property, Widget parent);
        private static Dictionary<Type, CreateComponent> DefaultCreationMethods = new Dictionary<Type, CreateComponent>();
        static PropertiesForm()
        {
            DefaultCreationMethods.Add(typeof(bool), (property, parent) =>
            {
                return new PropertiesFormCheckBox(property, parent);
            });

            DefaultCreationMethods.Add(typeof(Vector3), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormVector3(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Quaternion), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormEulerQuat(property, parent));
            });

            DefaultCreationMethods.Add(typeof(IntVector2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormIntVector2(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Vector2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormVector2(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Size2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSize2(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Byte), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormByte(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Decimal), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormDecimal(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Double), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormDouble(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Int16), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt16(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Int32), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt32(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Int64), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt64(property, parent));
            });

            DefaultCreationMethods.Add(typeof(SByte), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSByte(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Single), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSingle(property, parent));
            });

            DefaultCreationMethods.Add(typeof(UInt16), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt16(property, parent));
            });

            DefaultCreationMethods.Add(typeof(UInt32), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt32(property, parent));
            });

            DefaultCreationMethods.Add(typeof(UInt64), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt64(property, parent));
            });

            DefaultCreationMethods.Add(typeof(Color), (property, parent) =>
            {
                return new PropertiesFormColor(property, parent);
            });
        }

        private static PropertiesFormComponent buildConstrainableForm(EditableProperty property, ConstrainableFormComponent component)
        {
            if (property is ReflectedMinMaxEditableProperty)
            {
                component.setConstraints((ReflectedMinMaxEditableProperty)property);
            }

            return component;
        }
    }
}
