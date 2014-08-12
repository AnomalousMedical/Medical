using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using System.IO;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;
using MyGUIPlugin;
using System.Diagnostics;
using System.Reflection;

namespace Medical
{
    class AnomalousController : StandaloneApp
    {
        private StandaloneController controller;
        private SplashScreen splashScreen;
        private BorderLayoutChainLink editorBorder;
        private BorderLayoutChainLink contentArea;
        private LicenseDisplayManager licenseDisplay = new LicenseDisplayManager();

        public override bool OnInit()
        {
            return startApplication();
        }

        public override int OnExit()
        {
            bool applyingUpdate = UpdateController.promptForUpdate();
            controller.Dispose();
            if (restartOnShutdown && !applyingUpdate)
            {
                OtherProcessManager.restart(restartAsAdmin);
            }
            return 0;
        }

        public override bool OnIdle()
        {
            controller.onIdle();
            return MainWindow.Instance.Active;
        }

        public bool startApplication()
        {
            CertificateStoreManager.CertificateStoreLoaded += CertificateStoreManager_CertificateStoreLoaded;
            CertificateStoreManager.CertificateStoreLoadError += CertificateStoreManager_CertificateStoreLoadError;

            //Core
            controller = new StandaloneController(this);
            controller.BeforeSceneLoadProperties += new SceneEvent(controller_BeforeSceneLoadProperties);
            MyGUIInterface.Instance.CommonResourceGroup.addResource(this.GetType().AssemblyQualifiedName, "EmbeddedResource", true);
            splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, 100, "Medical.Resources.SplashScreen.SplashScreen.layout", "Medical.Resources.SplashScreen.SplashScreen.xml");
            splashScreen.Hidden += new EventHandler(splashScreen_Hidden);

            UpdateController.CurrentVersion = Assembly.GetAssembly(typeof(AnomalousMainPlugin)).GetName().Version;

            splashScreen.updateStatus(5, "Loading Files from Server");
            ResourceManager.Instance.load("Medical.Resources.AnomalousBootstrapImagesets.xml");

            return true;
        }

        void CertificateStoreManager_CertificateStoreLoaded()
        {
            LicenseManager = new LicenseManager(MedicalConfig.LicenseFile);
            LicenseManager.getKey(processKeyResults);

            controller.IdleHandler.runTemporaryIdle(runSplashScreen());
        }

        void CertificateStoreManager_CertificateStoreLoadError()
        {
            MessageBox.show("Anomalous Medical must connect to the Internet at least one time to download essential files.\nIf you can connect to the Internet, please do so now and click Retry.\nIf you cannot connect please click Cancel and try again when you have an Internet connection.", "Internet Connection Issue", MessageBoxStyle.IconQuest | MessageBoxStyle.Retry | MessageBoxStyle.Cancel, (result) =>
                {
                    if (result == MessageBoxStyle.Retry)
                    {
                        controller.retryLoadingCertificateStore();
                    }
                    else
                    {
                        controller.exit();
                    }
                });
        }

        private IEnumerable<IdleStatus> runSplashScreen()
        {
			splashScreen.updateStatus(10, String.Format("Initializing Core{0}", PlatformConfig.InitializingCoreMessage));
            yield return IdleStatus.Ok;

            //Configure the filesystem
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            //Add primary archive
            archive.addArchive(this.PrimaryArchive);

            controller.addWorkingArchive();

            controller.initializeControllers(createBackground());
            licenseDisplay.setSceneViewController(controller.SceneViewController);

            //Setup shader resources
            var resourceManager = PluginManager.Instance.createLiveResourceManager("GlobalShaders"); //We don't ever unload the shaders, so this can be garbage collected after its done here.
            var ogreResources = resourceManager.getSubsystemResource("Ogre");
            var shaderGroup = ogreResources.addResourceGroup("Medical.Shaders");
            shaderGroup.addResource("Shaders/Articulometrics", "EngineArchive", true);
            resourceManager.initializeResources();

            //GUI
            splashScreen.updateStatus(20, "Creating GUI");
            yield return IdleStatus.Ok;

            //Layout Chain
            LayoutChain layoutChain = new LayoutChain();
            layoutChain.addLink(new SingleChildChainLink(GUILocationNames.Notifications, controller.NotificationManager.LayoutContainer), true);
            layoutChain.addLink(new PopupAreaChainLink(GUILocationNames.FullscreenPopup), true);
            layoutChain.SuppressLayout = true;
            editorBorder = new BorderLayoutChainLink(GUILocationNames.EditorBorderLayout, controller.MedicalController.MainTimer);
            layoutChain.addLink(editorBorder, true);
            layoutChain.addLink(new MDIChainLink(GUILocationNames.MDI, controller.MDILayout), true);
            layoutChain.addLink(new PopupAreaChainLink(GUILocationNames.ContentAreaPopup), true);
            contentArea = new BorderLayoutChainLink(GUILocationNames.ContentArea, controller.MedicalController.MainTimer);
            layoutChain.addLink(contentArea, true);
            layoutChain.addLink(new FinalChainLink("SceneViews", controller.MDILayout.DocumentArea), true);
            layoutChain.SuppressLayout = false;
            layoutChain.layout();

            controller.createGUI(layoutChain);
            controller.GUIManager.setMainInterfaceEnabled(false);
            controller.GUIManager.Disposing += GUIManager_Disposing;

            //Scene Load
            splashScreen.updateStatus(30, "Loading Scene");
            yield return IdleStatus.Ok;
            controller.openNewScene(DefaultScene);

            splashScreen.updateStatus(70, "Waiting for License");
            yield return IdleStatus.Ok;
        }

        public void saveCrashLog()
        {
            if (controller != null)
            {
                controller.saveCrashLog();
            }
        }

        void controller_BeforeSceneLoadProperties(SimScene scene)
        {
            if (splashScreen != null)
            {
                splashScreen.updateStatus(60, "Loading Scene Properties");
            }
        }

        public override string WindowTitle
        {
            get 
            {
                return "Anomalous Medical";
            }
        }

        public override String PrimaryArchive
        {
            get
            {
                return Path.Combine(FolderFinder.ExecutableFolder, "AnomalousMedical.dat");
            }
        }

        public override String DefaultScene
        {
            get
            {
                String scene = Path.Combine(MedicalConfig.SceneDirectory, MedicalConfig.DefaultScene);
                if (!VirtualFileSystem.Instance.exists(scene))
                {
                    scene = Path.Combine(MedicalConfig.SceneDirectory, MedicalConfig.FallbackScene);
                }
                return scene;
            }
        }

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private BackgroundScene createBackground()
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(this.GetType().AssemblyQualifiedName, "EmbeddedResource", "Background", true);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            BackgroundScene background = new BackgroundScene("SourceBackground", "BodyAtlasBackground", 900, 5000, 5000, 50, 50);
            background.setVisible(true);
            return background;
        }

        void splashScreen_Hidden(object sender, EventArgs e)
        {
            splashScreen.Dispose();
            splashScreen = null;
            controller.sceneRevealed();
        }

        private void addPlugins()
        {
            //DEPENDENCY_HACK
            MedicalConfig.PluginConfig.addAdditionalDependencyFile("Utilities.dat");
            MedicalConfig.PluginConfig.addAdditionalPluginFile("IntroductionTutorial.dat");
            controller.AtlasPluginManager.addPlugin(new AnomalousMainPlugin(LicenseManager, this));
            MedicalConfig.PluginConfig.findRegularPluginsAndDependencies();
            foreach(String dependency in MedicalConfig.PluginConfig.Dependencies)
            {
                controller.AtlasDependencyManager.addDependency(dependency);
            }
            foreach (String plugin in MedicalConfig.PluginConfig.Plugins)
            {
                controller.AtlasPluginManager.addPlugin(plugin);
            }
        }

        void processKeyResults(bool valid)
        {
            if (valid)
            {
                keyValid();
            }
            else
            {
                showKeyDialog();
            }
        }

        void keyValid()
        {
            if (splashScreen != null && splashScreen.Visible)
            {
                splashScreen.updateStatus(85, "Loading Plugins");
            }

            MedicalConfig.setUserDirectory(LicenseManager.User);

            controller.GUIManager.setMainInterfaceEnabled(true);
            licenseDisplay.setLicenseText(String.Format("Licensed to: {0}", LicenseManager.LicenseeName));
            addPlugins();
            controller.initializePlugins();

            if (splashScreen != null && splashScreen.Visible)
            {
                splashScreen.updateStatus(100, "");
                splashScreen.hide();
            }
            else
            {
                //Let plugins know the scene has been revealed, if the license dialog had to be opened.
                controller.sceneRevealed();
            }

            controller.MedicalController.MainTimer.resetLastTime();
        }

        void showKeyDialog()
        {
            MvcLoginController mvcLogin = new MvcLoginController(controller, LicenseManager, keyValid);
            mvcLogin.showContext();

            splashScreen.updateStatus(100, "");
            splashScreen.hide();
        }

        void GUIManager_Disposing()
        {
            IDisposableUtil.DisposeIfNotNull(editorBorder);
            IDisposableUtil.DisposeIfNotNull(contentArea);
        }
    }
}
