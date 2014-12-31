using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Attributes;
using System.Reflection;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    public class PropertiesForm : PropertyEditor, IDisposable
    {
        public event Action<PropertiesForm> LayoutChanged;

        private List<PropertiesFormComponent> components = new List<PropertiesFormComponent>();
        protected StretchLayoutContainer flowLayout = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, 3, new IntVector2(0, 0));
        private Widget widget;
        private MedicalUICallback uiCallback;
        private EditInterface currentEditInterface;
        private EditablePropertyInfo currentPropInfo;
        private int rightPadding = ScaleHelper.Scaled(2);

        //These control the advanced button, it will appear if there are advanced properties in the 
        //edit interface, once the user clicks on it it will disappear and all advanced properties will
        //be displayed after that.
        private bool showAdvancedProperties = false;
        private LinkedList<EditableProperty> hiddenAdvancedProperties = new LinkedList<EditableProperty>();
        private PropertiesFormButton showAdvancedButton = null;

        public PropertiesForm(Widget widget, MedicalUICallback uiCallback)
        {
            this.uiCallback = uiCallback;
            this.widget = widget;
        }

        public virtual void Dispose()
        {
            clear();
        }

        public void clear()
        {
            if (currentEditInterface != null)
            {
                currentEditInterface.OnPropertyAdded -= new PropertyAdded(currentEditInterface_OnPropertyAdded);
                currentEditInterface.OnPropertyRemoved -= new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                currentEditInterface.OnDataNeedsRefresh -= new EditInterfaceEvent(currentEditInterface_OnDataNeedsRefresh);
            }
            flowLayout.clearChildren();
            foreach (PropertiesFormComponent component in components)
            {
                component.Dispose();
            }
            components.Clear();
        }

        public virtual void layout()
        {
            int height = flowLayout.DesiredSize.Height;
            int width = widget.Width;
            flowLayout.WorkingSize = new IntSize2(width - rightPadding, height);
            flowLayout.layout();
            if (LayoutChanged != null)
            {
                LayoutChanged.Invoke(this);
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditInterface;
            }
            set
            {
                clear();
                if (value != null)
                {
                    currentEditInterface = value;
                    currentEditInterface.OnPropertyAdded += new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved += new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                    currentEditInterface.OnDataNeedsRefresh += new EditInterfaceEvent(currentEditInterface_OnDataNeedsRefresh);
                    currentPropInfo = value.getPropertyInfo();
                    foreach (EditInterfaceCommand command in currentEditInterface.getCommands())
                    {
                        PropertiesFormComponent component = new PropertiesFormButton(currentEditInterface, command, uiCallback, widget);
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

        public int Height
        {
            get
            {
                return flowLayout.WorkingSize.Height;
            }
        }

        public int DesiredHeight
        {
            get
            {
                return flowLayout.DesiredSize.Height;
            }
        }

        public int RightPadding
        {
            get
            {
                return rightPadding;
            }
            set
            {
                rightPadding = value;
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
            flowLayout.SuppressLayout = true;
            if (showAdvancedButton != null)
            {
                flowLayout.removeChild(showAdvancedButton.Container);
            }
            if (showAdvancedProperties || !property.Advanced)
            {
                PropertiesFormComponent component = createComponenet(property);
                components.Add(component);
                flowLayout.addChild(component.Container);
            }
            else
            {
                hiddenAdvancedProperties.AddLast(property);
                if (showAdvancedButton == null)
                {
                    showAdvancedButton = new PropertiesFormButton(currentEditInterface, new EditInterfaceCommand("Show Advanced", addHiddenProperties), uiCallback, widget);
                    components.Add(showAdvancedButton);
                }
            }
            if (showAdvancedButton != null)
            {
                flowLayout.addChild(showAdvancedButton.Container);
            }
            flowLayout.SuppressLayout = false;
        }

        private void addHiddenProperties()
        {
            showAdvancedProperties = true;
            foreach (EditableProperty property in hiddenAdvancedProperties)
            {
                addProperty(property);
            }
            hiddenAdvancedProperties.Clear();
            components.Remove(showAdvancedButton);
            flowLayout.removeChild(showAdvancedButton.Container);
            showAdvancedButton.Dispose();
            showAdvancedButton = null;
            layout();
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
                    return;
                }
            }
            hiddenAdvancedProperties.Remove(property);
        }

        private PropertiesFormComponent createComponenet(EditableProperty property)
        {
            Type propertyType = property.getPropertyType(1);
            //Have to do this because we cannot "out" a method using trygetvalue
            if (FormCreationMethods.ContainsKey(propertyType))
            {
                return FormCreationMethods[propertyType].Invoke(property, widget);
            }
            else if (propertyType.IsEnum)
            {
                if (propertyType.GetCustomAttributes(typeof(SingleEnumAttribute), true).Length > 0)
                {
                    PropertiesFormComboBox editorCell = new PropertiesFormComboBox(property, widget, propertyType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(fieldInfo => new Pair<String, Object>(fieldInfo.Name, Enum.ToObject(propertyType, fieldInfo.GetRawConstantValue()))));
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
                return new PropertiesFormTextBoxBrowser(property, widget, uiCallback);
            }
            return new PropertiesFormTextBox(property, widget);
        }

        public delegate PropertiesFormComponent CreateComponent(EditableProperty property, Widget parent);
        private static Dictionary<Type, CreateComponent> FormCreationMethods = new Dictionary<Type, CreateComponent>();
        static PropertiesForm()
        {
            FormCreationMethods.Add(typeof(bool), (property, parent) =>
            {
                return new PropertiesFormCheckBox(property, parent);
            });

            FormCreationMethods.Add(typeof(Vector3), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormVector3(property, parent));
            });

            FormCreationMethods.Add(typeof(Quaternion), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormEulerQuat(property, parent));
            });

            FormCreationMethods.Add(typeof(IntVector2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormIntVector2(property, parent));
            });

            FormCreationMethods.Add(typeof(Vector2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormVector2(property, parent));
            });

            FormCreationMethods.Add(typeof(Size2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSize2(property, parent));
            });

            FormCreationMethods.Add(typeof(IntSize2), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormIntSize2(property, parent));
            });

            FormCreationMethods.Add(typeof(Byte), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormByte(property, parent));
            });

            FormCreationMethods.Add(typeof(Decimal), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormDecimal(property, parent));
            });

            FormCreationMethods.Add(typeof(Double), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormDouble(property, parent));
            });

            FormCreationMethods.Add(typeof(Int16), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt16(property, parent));
            });

            FormCreationMethods.Add(typeof(Int32), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt32(property, parent));
            });

            FormCreationMethods.Add(typeof(Int32?), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt32Nullable(property, parent));
            });

            FormCreationMethods.Add(typeof(Int64), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormInt64(property, parent));
            });

            FormCreationMethods.Add(typeof(SByte), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSByte(property, parent));
            });

            FormCreationMethods.Add(typeof(Single), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormSingle(property, parent));
            });

            FormCreationMethods.Add(typeof(UInt16), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt16(property, parent));
            });

            FormCreationMethods.Add(typeof(UInt32), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt32(property, parent));
            });

            FormCreationMethods.Add(typeof(UInt64), (property, parent) =>
            {
                return buildConstrainableForm(property, new PropertiesFormUInt64(property, parent));
            });

            FormCreationMethods.Add(typeof(Color), (property, parent) =>
            {
                return new PropertiesFormColor(property, parent);
            });

            FormCreationMethods.Add(typeof(Color?), (property, parent) =>
            {
                return new PropertiesFormColorNullable(property, parent);
            });

            FormCreationMethods.Add(typeof(ChoiceObject), (property, parent) =>
            {
                return new PropertiesFormComboBox(property, parent, ((ChoiceObject)property.getRealValue(1)).Choices);
            });
        }

        public static void addFormCreationMethod(Type type, CreateComponent creationMethod)
        {
            FormCreationMethods.Add(type, creationMethod);
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
