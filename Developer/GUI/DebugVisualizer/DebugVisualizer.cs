using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developer.GUI
{
    class DebugVisualizer : MDIDialog
    {
        private PluginManager pluginManager;
        private bool firstShow = true;
        private SimScene currentScene;

        private MedicalUICallback uiCallback;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        public DebugVisualizer(StandaloneController standaloneController)
            : base("Developer.GUI.DebugVisualizer.DebugVisualizer.layout")
        {
            this.pluginManager = standaloneController.MedicalController.PluginManager;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            uiCallback = new MedicalUICallback();

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            this.Resized += DebugVisualizer_Resized;
        }

        public override void Dispose()
        {
            objectEditor.Dispose();
            propTable.Dispose();
            table.Dispose();
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            if(firstShow)
            {
                firstShow = false;
                EditInterface editInterface = new EditInterface("Debug Visualizers");
                foreach(var debugInterface in pluginManager.getDebugInterfaces())
                {
                    EditInterface debugEditInterface = new EditInterface(debugInterface.Name);
                    EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                    propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                    propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                    debugEditInterface.setPropertyInfo(propertyInfo);

                    debugEditInterface.addEditableProperty(new GenericEditableProperty<bool>("Enabled",
                        () => debugInterface.Enabled, v => debugInterface.Enabled = v, canParseBool, bool.Parse));

                    debugEditInterface.addEditableProperty(new GenericEditableProperty<bool>("Depth Testing",
                        () => debugInterface.DepthTesting, v => debugInterface.DepthTesting = v, canParseBool, bool.Parse));

                    foreach(var entry in debugInterface.getEntries())
                    {
                        debugEditInterface.addEditableProperty(new GenericEditableProperty<bool>(entry.Text,
                            () => entry.Enabled, value => entry.Enabled = value, canParseBool, bool.Parse));
                    }
                    
                    editInterface.addSubInterface(debugEditInterface);
                }

                objectEditor.EditInterface = editInterface;
            }
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            currentScene = scene;
            if (!firstShow && currentScene != null)
            {
                foreach (DebugInterface debugInterface in pluginManager.getDebugInterfaces())
                {
                    debugInterface.createDebugInterface(pluginManager.RendererPlugin, currentScene.getDefaultSubScene());
                }
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            currentScene = null;
            if (!firstShow)
            {
                foreach (DebugInterface debugInterface in pluginManager.getDebugInterfaces())
                {
                    debugInterface.destroyDebugInterface(pluginManager.RendererPlugin, scene.getDefaultSubScene());
                }
            }
        }

        void DebugVisualizer_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }

        bool canParseBool(String str)
        {
            bool bVal;
            return bool.TryParse(str, out bVal);
        }

        //probably move this out to the engine
        class GenericEditableProperty<T> : EditableProperty
        {
            private String name;
            private Func<T> getGenericValue;
            private Action<T> setGenericValue;
            private Func<String, bool> canParseStringToValue;
            private Func<String, T> parseStringToValue;

            public GenericEditableProperty(String name, Func<T> getGenericValue, Action<T> setGenericValue, Func<String, bool> canParseStringToValue, Func<String, T> parseStringToValue)
            {
                this.name = name;
                this.getGenericValue = getGenericValue;
                this.setGenericValue = setGenericValue;
                this.canParseStringToValue = canParseStringToValue;
                this.parseStringToValue = parseStringToValue;
            }

            public bool Advanced
            {
                get
                {
                    return false;
                }
            }

            public bool canParseString(int column, string value, out string errorMessage)
            {
                if(canParseStringToValue(value))
                {
                    errorMessage = null;
                    return true;
                }
                errorMessage = "Cannot parse.";
                return false;
            }

            public Browser getBrowser(int column, EditUICallback uiCallback)
            {
                return null;
            }

            public Type getPropertyType(int column)
            {
                switch (column)
                {
                    case 0:
                        return typeof(String);
                    case 1:
                        return typeof(T);
                    default:
                        return typeof(String);
                }
            }

            public object getRealValue(int column)
            {
                switch (column)
                {
                    case 0:
                        return name;
                    case 1:
                        return getGenericValue();
                    default:
                        return null;
                }
            }

            public string getValue(int column)
            {
                switch (column)
                {
                    case 0:
                        return name;
                    case 1:
                        return getGenericValue().ToString();
                    default:
                        return null;
                }
            }

            public bool hasBrowser(int column)
            {
                return false;
            }

            public bool readOnly(int column)
            {
                return column == 0;
            }

            public void setValue(int column, object value)
            {
                if (column == 1)
                {
                    setGenericValue((T)value);
                }
            }

            public void setValueStr(int column, string value)
            {
                if (column == 1)
                {
                    setGenericValue(parseStringToValue(value));
                }
            }
        }
    }
}
