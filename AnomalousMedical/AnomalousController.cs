﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using System.IO;
using Engine.ObjectManagement;
using OgrePlugin;
using MyGUIPlugin;
using System.Reflection;
using Anomalous.OSPlatform;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;

namespace Medical
{
    class AnomalousController : App
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
        private AnomalousMainPlugin mainPlugin;

        public AnomalousController()
        {
            Title = "Anomalous Medical";
        }

        public override bool OnInit()
        {
            CertificateStoreManager.CertificateStoreLoaded += CertificateStoreManager_CertificateStoreLoaded;
            CertificateStoreManager.CertificateStoreLoadError += CertificateStoreManager_CertificateStoreLoadError;

            //Cusom Config
            MyGUIInterface.BeforeMainResourcesLoaded += MyGUIInterface_BeforeMainResourcesLoaded;

            //Core
            controller = new StandaloneController(this);
            controller.BeforeSceneLoadProperties += new SceneEvent(controller_BeforeSceneLoadProperties);
            splashScreen = new SplashScreen(controller.MainWindow, FinishedPosition, "Medical.Resources.SplashScreen.SplashScreen.layout", "Medical.Resources.SplashScreen.SplashScreen.xml");
            splashScreen.Hidden += splashScreen_Hidden;
            splashScreen.StatusUpdated += splashScreen_StatusUpdated;

            UpdateController.CurrentVersion = Assembly.GetAssembly(typeof(AnomalousMainPlugin)).GetName().Version;

            splashScreen.updateStatus(LoadingServerFilesPosition, "Loading Files from Server");
            ResourceManager.Instance.load("Medical.Resources.AnomalousBootstrapImagesets.xml");

            return true;
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

        public override void OnIdle()
        {
            controller.onIdle();
        }

        void MyGUIInterface_BeforeMainResourcesLoaded(MyGUIInterface obj)
        {
            MyGUIInterface.Instance.CommonResourceGroup.addResource(this.GetType().AssemblyQualifiedName, "EmbeddedResource", true);
            MyGUIInterface.LayerFile = "Medical.Resources.AnomalousMedical_MyGUI_Layer.xml";
            MyGUIInterface.BeforeMainResourcesLoaded -= MyGUIInterface_BeforeMainResourcesLoaded;
        }

        void CertificateStoreManager_CertificateStoreLoaded()
        {
            CertificateStoreManager.CertificateStoreLoaded -= CertificateStoreManager_CertificateStoreLoaded;
            CertificateStoreManager.CertificateStoreLoadError -= CertificateStoreManager_CertificateStoreLoadError;

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

            controller.initializeControllers(createBackground(), new LicenseManager(MedicalConfig.LicenseFile));
            controller.LicenseManager.getKey(processKeyResults);
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

        public String PrimaryArchive
        {
            get
            {
                return Path.Combine(FolderFinder.ExecutableFolder, "AnomalousMedical.dat");
            }
        }

        public String DefaultScene
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

        void splashScreen_Hidden(SplashScreen sender)
        {
            splashScreen.Dispose();
            splashScreen = null;
            if (mainPlugin != null)
            {
                mainPlugin.sceneRevealed();
            }
        }

        void splashScreen_StatusUpdated(SplashScreen sender)
        {
            OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget.update();
        }

        private IEnumerable<PluginLoadStatus> addPlugins()
        {
            PluginLoadStatus loadStatus = new PluginLoadStatus();
            loadStatus.Total = MedicalConfig.PluginConfig.findPlugins();

            loadStatus.Total += 2;

            MedicalConfig.PluginConfig.addAdditionalPluginFile("IntroductionTutorial.dat");
            loadStatus.Current++;
            yield return loadStatus;
            mainPlugin = new AnomalousMainPlugin(controller.LicenseManager, this);
            controller.AtlasPluginManager.addPlugin(mainPlugin);
            loadStatus.Current++;
            yield return loadStatus;

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
            MvcLoginController mvcLogin = new MvcLoginController(controller, controller.LicenseManager);
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
                            mainPlugin.sceneRevealed();
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
            MedicalConfig.setUserDirectory(controller.LicenseManager.User);

            controller.GUIManager.setMainInterfaceEnabled(true);
            licenseDisplay.setLicenseText(String.Format("Licensed to: {0}", controller.LicenseManager.LicenseeName));

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
