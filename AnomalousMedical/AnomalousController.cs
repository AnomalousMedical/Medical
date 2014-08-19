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
        private const int LoadingServerFilesPosition = 5;
        private const int InitializingCorePosition = 10;
        private const int CreatingGuiPosition = 15;
        private const int LoadingScenePosition = 20;
        private const int LoadingScenePropertiesPosition = 75;
        private const int WaitingForLicensePosition = 80;
        private const int LoadingPluginsPosition = 80;
        private const int InitializingPluginsPosition = 90;
        private const int FinishedPosition = 100;

        private const int LoadingSceneDelta = LoadingScenePropertiesPosition - LoadingScenePosition;
        private const int LoadingPluginsDelta = InitializingPluginsPosition - LoadingPluginsPosition;
        private const int InitializingPluginsDelta = FinishedPosition - InitializingPluginsPosition;

        private delegate uint StatusUpdateFunc(uint currentPosition, uint startPos, uint totalPositionGain, PluginLoadStatus status);

        private StandaloneController controller;
        private SplashScreen splashScreen;
        private BorderLayoutChainLink editorBorder;
        private BorderLayoutChainLink contentArea;
        private LicenseDisplayManager licenseDisplay = new LicenseDisplayManager();
        private Engine.Resources.ResourceGroup commonResources;

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
            splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, FinishedPosition, "Medical.Resources.SplashScreen.SplashScreen.layout", "Medical.Resources.SplashScreen.SplashScreen.xml");
            splashScreen.Hidden += new EventHandler(splashScreen_Hidden);

            UpdateController.CurrentVersion = Assembly.GetAssembly(typeof(AnomalousMainPlugin)).GetName().Version;

            splashScreen.updateStatus(LoadingServerFilesPosition, "Loading Files from Server");
            ResourceManager.Instance.load("Medical.Resources.AnomalousBootstrapImagesets.xml");

            return true;
        }

        void CertificateStoreManager_CertificateStoreLoaded()
        {
            CertificateStoreManager.CertificateStoreLoaded -= CertificateStoreManager_CertificateStoreLoaded;
            CertificateStoreManager.CertificateStoreLoadError -= CertificateStoreManager_CertificateStoreLoadError;

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
            splashScreen.updateStatus(InitializingCorePosition, "Initializing Core");
            yield return IdleStatus.Ok;

            //Configure the filesystem
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            //Add primary archive
            archive.addArchive(this.PrimaryArchive);

            controller.addWorkingArchive();

            var resourceManager = PluginManager.Instance.createLiveResourceManager("AnomalousMedical"); //We don't ever unload the shaders, so this can be garbage collected after its done here.
            var ogreResources = resourceManager.getSubsystemResource("Ogre");
            commonResources = ogreResources.addResourceGroup("Common");

            controller.initializeControllers(createBackground());
            licenseDisplay.setSceneViewController(controller.SceneViewController);

            //Setup shader resources
            var shaderGroup = ogreResources.addResourceGroup("Shaders");
            shaderGroup.addResource("Shaders/Articulometrics", "EngineArchive", true);
            shaderGroup.initialize();

            //GUI
            splashScreen.updateStatus(CreatingGuiPosition, "Creating GUI");
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
            uint currentPosition = LoadingScenePosition;
            String message = "Loading Scene";
            bool updateStatus = true;
            bool firstOgre = true;

            splashScreen.updateStatus(currentPosition, message);
            yield return IdleStatus.Ok;

            foreach (var status in controller.openNewSceneStatus(DefaultScene))
            {
                switch (status.Subsystem)
                {
                    case OgreInterface.PluginName:
                        if (status.NumItems != 0)
                        {
                            currentPosition = LoadingScenePosition + (uint)(status.CurrentPercent * LoadingSceneDelta);
                            message = "Loading Artwork";
                            updateStatus = firstOgre || currentPosition > splashScreen.Position;
                            firstOgre = false;
                        }
                        break;
                    default:
                        updateStatus = true;
                        if (status.Message != null)
                        {
                            message = status.Message;
                        }
                        else
                        {
                            message = "Loading Scene";
                        }
                        break;
                }

                if (updateStatus)
                {
                    splashScreen.updateStatus(currentPosition, message);
                }

                yield return IdleStatus.Ok;
            }

            splashScreen.updateStatus(WaitingForLicensePosition, "Waiting for License");
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
                splashScreen.updateStatus(LoadingScenePropertiesPosition, "Loading Scene Properties");
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
            commonResources.addResource(this.GetType().AssemblyQualifiedName, "EmbeddedResource", true);
            commonResources.initialize();
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

        private IEnumerable<PluginLoadStatus> addPlugins()
        {
            PluginLoadStatus loadStatus = new PluginLoadStatus();
            loadStatus.Total = MedicalConfig.PluginConfig.findRegularPluginsAndDependencies();

            loadStatus.Total += 3;

            //DEPENDENCY_HACK
            MedicalConfig.PluginConfig.addAdditionalDependencyFile("Utilities.dat");
            loadStatus.Current++;
            yield return loadStatus;
            MedicalConfig.PluginConfig.addAdditionalPluginFile("IntroductionTutorial.dat");
            loadStatus.Current++;
            yield return loadStatus;
            controller.AtlasPluginManager.addPlugin(new AnomalousMainPlugin(LicenseManager, this));
            loadStatus.Current++;
            yield return loadStatus;

            foreach (String dependency in MedicalConfig.PluginConfig.Dependencies)
            {
                controller.AtlasDependencyManager.addDependency(dependency);
                loadStatus.Current++;
                yield return loadStatus;
            }

            foreach(int dependencyId in MedicalConfig.PluginConfig.ForceLoadDependencies)
            {
                controller.AtlasDependencyManager.initializeDependency(dependencyId);
                yield return loadStatus;
            }

            foreach (String plugin in MedicalConfig.PluginConfig.Plugins)
            {
                controller.AtlasPluginManager.addPlugin(plugin);
                loadStatus.Current++;
                yield return loadStatus;
            }
        }

        void processKeyResults(bool valid)
        {
            //This is fired when the idle function returns to the normal thread processor and the ThreadUtilities invokes
            //its invoke functions, which will contain the results of the license check.
            if (valid)
            {
                //Key was valid and the splash screen is still showing.
                splashScreen.updateStatus(LoadingPluginsPosition, "Loading Plugins");

                keyValid(splashScreen.Position, 
                (currentPosition, startPos, totalPositionGain, status) =>
                    {
                        if (status.Total != 0)
                        {
                            currentPosition = startPos + (uint)(status.PercentComplete * totalPositionGain);
                            if (currentPosition > splashScreen.Position)
                            {
                                splashScreen.updateStatus(currentPosition, "Loading Plugins");
                            }
                        }
                        return currentPosition;
                    }, 
                () =>
                    {
                        splashScreen.updateStatus(FinishedPosition, "");
                        splashScreen.hide();
                    });
            }
            else
            {
                //Key was not valid, show the key dialog
                showKeyDialog();
            }
        }

        void showKeyDialog()
        {
            MvcLoginController mvcLogin = new MvcLoginController(controller, LicenseManager);
            mvcLogin.LoginSucessful += () =>
                {
                    Root.getSingleton()._updateAllRenderTargets();
                    keyValid(0,
                        (currentPosition, startPos, totalPositionGain, status) =>
                        {
                            return currentPosition;
                        },
                        () =>
                        {
                            mvcLogin.close();
                            //Let plugins know the scene has been revealed, since the license dialog had to be opened.
                            controller.sceneRevealed();
                        });
                };
            mvcLogin.showContext();

            splashScreen.updateStatus(FinishedPosition, "");
            splashScreen.hide();
        }

        void keyValid(uint currentPosition, StatusUpdateFunc statusUpdateFunc, Action finishFunc)
        {
            controller.IdleHandler.runTemporaryIdle(keyValidIdleFunc(currentPosition, statusUpdateFunc, finishFunc));
        }

        private IEnumerable<IdleStatus> keyValidIdleFunc(uint currentPosition, StatusUpdateFunc statusUpdateFunc, Action finishFunc)
        {
            MedicalConfig.setUserDirectory(LicenseManager.User);

            controller.GUIManager.setMainInterfaceEnabled(true);
            licenseDisplay.setLicenseText(String.Format("Licensed to: {0}", LicenseManager.LicenseeName));

            foreach (var status in addPlugins())
            {
                currentPosition = statusUpdateFunc(currentPosition, LoadingPluginsPosition, LoadingPluginsDelta, status);
                yield return IdleStatus.Ok;
            }

            foreach (var status in controller.initializePlugins())
            {
                currentPosition = statusUpdateFunc(currentPosition, InitializingPluginsPosition, InitializingPluginsDelta, status);
                yield return IdleStatus.Ok;
            }

            finishFunc();

            controller.MedicalController.MainTimer.resetLastTime();
            yield return IdleStatus.Ok;
        }

        void GUIManager_Disposing()
        {
            IDisposableUtil.DisposeIfNotNull(editorBorder);
            IDisposableUtil.DisposeIfNotNull(contentArea);
        }
    }
}
