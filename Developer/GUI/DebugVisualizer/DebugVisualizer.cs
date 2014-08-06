using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
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
        private MedicalController medicalController;

        private MedicalUICallback uiCallback;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private PropertiesForm propertiesForm;

        private ObjectEditor objectEditor;

        public DebugVisualizer(StandaloneController standaloneController)
            : base("Developer.GUI.DebugVisualizer.DebugVisualizer.layout")
        {
            this.medicalController = standaloneController.MedicalController;
            this.pluginManager = medicalController.PluginManager;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            uiCallback = new MedicalUICallback();

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            propertiesForm = new ScrollablePropertiesForm((ScrollView)window.findWidget("TableScroller"), uiCallback);

            objectEditor = new ObjectEditor(editTreeView, propertiesForm, uiCallback);

            this.Resized += DebugVisualizer_Resized;

            currentScene = standaloneController.MedicalController.CurrentScene;
        }

        public override void Dispose()
        {
            objectEditor.Dispose();
            propertiesForm.Dispose();
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
                foreach(var debugInterface in pluginManager.DebugInterfaces)
                {
                    EditInterface debugEditInterface = new EditInterface(debugInterface.Name);
                    EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                    propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                    propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                    debugEditInterface.setPropertyInfo(propertyInfo);

                    debugEditInterface.addEditableProperty(new CallbackEditableProperty<bool>("Enabled",
                        () => debugInterface.Enabled, v => debugInterface.Enabled = v, canParseBool, bool.Parse));

                    debugEditInterface.addEditableProperty(new CallbackEditableProperty<bool>("Depth Testing",
                        () => debugInterface.DepthTesting, v => debugInterface.DepthTesting = v, canParseBool, bool.Parse));

                    foreach(var entry in debugInterface.Entries)
                    {
                        debugEditInterface.addEditableProperty(new CallbackEditableProperty<bool>(entry.Text,
                            () => entry.Enabled, value => entry.Enabled = value, canParseBool, bool.Parse));
                    }
                    
                    editInterface.addSubInterface(debugEditInterface);
                }

                objectEditor.EditInterface = editInterface;

                createDebugVisualizers();
            }
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            currentScene = scene;
            createDebugVisualizers();
        }

        private void createDebugVisualizers()
        {
            if (!firstShow && currentScene != null)
            {
                medicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
                foreach (DebugInterface debugInterface in pluginManager.DebugInterfaces)
                {
                    debugInterface.createDebugInterface(pluginManager.RendererPlugin, currentScene.getDefaultSubScene());
                }
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            medicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            currentScene = null;
            if (!firstShow)
            {
                foreach (DebugInterface debugInterface in pluginManager.DebugInterfaces)
                {
                    debugInterface.destroyDebugInterface(pluginManager.RendererPlugin, scene.getDefaultSubScene());
                }
            }
        }

        void DebugVisualizer_Resized(object sender, EventArgs e)
        {
            tree.layout();
            propertiesForm.layout();
        }

        bool canParseBool(String str)
        {
            bool bVal;
            return bool.TryParse(str, out bVal);
        }

        void MedicalController_OnLoopUpdate(Clock time)
        {
            //This is only active if the scene is not null and the debug visualizers are setup
            SimSubScene subScene = currentScene.getDefaultSubScene();
            foreach (DebugInterface debugInterface in pluginManager.DebugInterfaces)
            {
                debugInterface.renderDebug(subScene);
            }
        }
    }
}
