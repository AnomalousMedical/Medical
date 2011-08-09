using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Renderer;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;
using Logging;
using Medical;
using OgrePlugin;
using OgreWrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using MyGUIPlugin;
using Medical.GUI;
using Medical.Controller;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Medical
{
    public delegate void SceneEvent(SimScene scene);

    public class StandaloneController : IDisposable
    {
        //Events
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;
        public event SceneEvent BeforeSceneLoadProperties;

        //Controller
        private MedicalController medicalController;
        private WindowListener windowListener;
        private MedicalStateController medicalStateController;
        private TemporaryStateBlender tempStateBlender;
        private MovementSequenceController movementSequenceController;
        private SimObjectMover teethMover;
        private ImageRenderer imageRenderer;
        private TimelineController timelineController;
        private PropFactory propFactory;
        private ExamController examController;
        private TaskController taskController;

        //GUI
        private GUIManager guiManager;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private SceneViewWindowPresetController windowPresetController;
        private HtmlHelpController htmlHelpController;
        private AbstractTimelineGUIManager abstractTimelineGUIManager;

        //Platform
        private MainWindow mainWindow;
        private StandaloneApp app;
        private AtlasPluginManager atlasPluginManager;
		private bool shuttingDown = false;

        //Touch
        private TouchController touchController;

        public StandaloneController(StandaloneApp app)
        {
            this.app = app;

            MedicalConfig config = new MedicalConfig(FolderFinder.AnomalousMedicalRoot, app.ProgramFolder);
            atlasPluginManager = new AtlasPluginManager(this);
            guiManager = new GUIManager(this);

            MyGUIInterface.Theme = PlatformConfig.ThemeFile;

            //Engine core
            medicalController = new MedicalController();
            mainWindow = new MainWindow(app.WindowTitle);
            Medical.Controller.WindowFunctions.setWindowIcon(mainWindow, app.Icon);
            medicalController.initialize(app, mainWindow, createWindow);
            mainWindow.setPointerManager(PointerManager.Instance);

            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);
        }

        public void Dispose()
        {
            DocumentController.saveRecentDocuments();
            if (touchController != null)
            {
                touchController.Dispose();
            }
            htmlHelpController.Dispose();
            atlasPluginManager.Dispose();
            guiManager.Dispose();
            watermark.Dispose();
            measurementGrid.Dispose();
            movementSequenceController.Dispose();
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            mdiLayout.Dispose();
            medicalController.Dispose();
            mainWindow.Dispose();

            //Stop any waiting background threads last.
            ThreadManager.cancelAll();
        }

        public void initializeControllers(ViewportBackground background)
        {
            //Help
            htmlHelpController = new HtmlHelpController(mainWindow);
            app.addHelpDocuments(htmlHelpController);

            //Documents
            DocumentController = new DocumentController(MedicalConfig.RecentDocsFile);

            //Setup MyGUI listeners
            MyGUIInterface myGUI = MyGUIInterface.Instance;
            myGUI.RenderEnded += new EventHandler(myGUI_RenderEnded);
            myGUI.RenderStarted += new EventHandler(myGUI_RenderStarted);

            windowListener = new WindowListener(this);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);
            OgreInterface.Instance.OgrePrimaryWindow.OgreRenderWindow.DeactivateOnFocusChange = false;

            //MDI Layout
            mdiLayout = new MDILayoutManager();
            medicalController.MainTimer.addFixedUpdateListener(new MDIUpdate(medicalController.EventManager, mdiLayout));

            //SceneView
            MyGUIInterface myGui = MyGUIInterface.Instance;
            sceneViewController = new SceneViewController(mdiLayout, medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow, myGui.OgrePlatform.getRenderManager());

            //Watermark
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

            //Background
            this.background = background;
            backgroundController = new BackgroundController(background, sceneViewController);

            //Measurement grid
            measurementGrid = new MeasurementGrid("MeasurementGrid", medicalController, sceneViewController);
            SceneUnloading += measurementGrid.sceneUnloading;
            SceneLoaded += measurementGrid.sceneLoaded;

            //Image Renderer
            imageRenderer = new ImageRenderer(medicalController, sceneViewController);
            imageRenderer.Watermark = watermark;
            imageRenderer.Background = background;
            imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;

            //Medical states
            medicalStateController = new MedicalStateController(imageRenderer, medicalController);
            tempStateBlender = new TemporaryStateBlender(medicalController.MainTimer, medicalStateController);

            //Movement sequences
            movementSequenceController = new MovementSequenceController(medicalController);

            //Teeth mover
            teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
            this.SceneLoaded += teethMover.sceneLoaded;
            this.SceneUnloading += teethMover.sceneUnloading;
            TeethController.TeethMover = teethMover;
            medicalController.FixedLoopUpdate += teethMover.update;
            imageRenderer.ImageRenderStarted += TeethController.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += TeethController.ScreenshotRenderCompleted;

            //Props
            propFactory = new PropFactory(this);
            Arrow.createPropDefinition(propFactory);
            Ruler.createPropDefinition(propFactory);
            PointingHand.createPropDefinition(propFactory);
            Doppler.createPropDefinition(propFactory);
            Syringe.createPropDefinition(propFactory);
            JVAProp.createPropDefinition(propFactory);
            Mustache.createPropDefinition(propFactory);
            CircularHighlight.createPropDefinition(propFactory);
            PoseableHand.createPropDefinition(propFactory);
            BiteStick.createPropDefinition(propFactory);
            RangeOfMotionScale.createPropDefinition(propFactory);
            Pen.createPropDefinition(propFactory);
            Caliper.createPropDefinition(propFactory);
            SplintDefiner.createPropDefinition(propFactory);
            DentalFloss.createPropDefinition(propFactory);

            //Timeline
            TimelineGUIFactory = new TimelineGUIFactory();
            timelineController = new TimelineController(this);
            timelineController.PlaybackStarted += new EventHandler(timelineController_PlaybackStarted);
            timelineController.PlaybackStopped += new EventHandler(timelineController_PlaybackStopped);

            abstractTimelineGUIManager = new AbstractTimelineGUIManager(medicalController.MainTimer, guiManager);

            //Exams
            examController = new ExamController();

            //Tasks
            taskController = new TaskController();

            //MultiTouch
            if (MedicalConfig.EnableMultitouch && MultiTouch.IsAvailable)
            {
                touchController = new TouchController(mainWindow, medicalController.MainTimer, sceneViewController);
            }
            else
            {
                Log.Info("MultiTouch not available");
            }
        }

        public void createGUI()
        {
            windowPresetController = new SceneViewWindowPresetController();
            app.createWindowPresets(windowPresetController);

            //GUI
            guiManager.createGUI(mdiLayout);
            guiManager.giveGUIsToTimelineController(timelineController);
            medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);

            //Create scene view windows
            sceneViewController.createFromPresets(windowPresetController.getPresetSet("Primary"));
        }

        public void initializePlugins()
        {
            Taskbar taskbar = GUIManager.Taskbar;
            TaskMenu taskMenu = GUIManager.TaskMenu;
            taskbar.SuppressLayout = true;
            taskMenu.SuppressLayout = true;
            atlasPluginManager.initialzePlugins();
            taskbar.SuppressLayout = false;
            taskMenu.SuppressLayout = false;
            taskbar.layout();
            guiManager.loadSavedUI();

            if (PlatformConfig.CreateMenu)
            {
                guiManager.createMenuBar(mainWindow.MenuBar);
            }

            //Load recent documents here, this way the document handlers are all loaded
            DocumentController.loadRecentDocuments();
        }

        public void onIdle()
        {
            medicalController.MainTimer.OnIdle();
        }

        public void openHelpTopic(int index)
        {
            htmlHelpController.Display(index);
        }

        public void exit()
        {
			if(!shuttingDown)
			{
				shuttingDown = true;
	            if (PlatformConfig.CloseMainWindowOnShutdown)
	            {
	                mainWindow.close();
	            }
            	app.exit();
			}
        }

        /// <summary>
        /// Opens a scene as a "new" scene by opening the given file and clearing the states.
        /// </summary>
        /// <param name="filename"></param>
        public bool openNewScene(String filename)
        {
            medicalStateController.clearStates();
            bool success = changeScene(filename);
            medicalStateController.createNormalStateFromScene();
            examController.clear();
            return success;
        }

        public void saveMedicalState(PatientDataFile dataFile)
        {
            if (medicalStateController.getNumStates() == 0)
            {
                medicalStateController.createNormalStateFromScene();
            }
            dataFile.PatientData.MedicalStates = medicalStateController.getSavedState(medicalController.CurrentSceneFile);
            examController.addExamsToData(dataFile.PatientData);
            dataFile.save();
        }

        public void openPatientFile(PatientDataFile dataFile)
        {
            if (dataFile.loadData())
            {
                SavedMedicalStates states = dataFile.PatientData.MedicalStates;
                if (states != null)
                {
                    changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName);
                    medicalStateController.setStates(states);
                    medicalStateController.blend(0.0f);
                    guiManager.changeLeftPanel(null);
                }
                else
                {
                    MessageBox.show(String.Format("Error loading file {0}.\nCould not read state information.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
                examController.setExamsFromData(dataFile.PatientData);
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.\nCould not load patient data.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void sceneRevealed()
        {
            atlasPluginManager.sceneRevealed();
        }

        public void setWatermarkText(String text)
        {
            ((SideLogoWatermark)watermark).addText(text);
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public TemporaryStateBlender TemporaryStateBlender
        {
            get
            {
                return tempStateBlender;
            }
        }

        public SceneViewController SceneViewController
        {
            get
            {
                return sceneViewController;
            }
        }

        public MedicalStateController MedicalStateController
        {
            get
            {
                return medicalStateController;
            }
        }

        public MovementSequenceController MovementSequenceController
        {
            get
            {
                return movementSequenceController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public SceneViewWindowPresetController PresetWindows
        {
            get
            {
                return windowPresetController;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return measurementGrid;
            }
        }

        public MDILayoutManager MDILayout
        {
            get
            {
                return mdiLayout;
            }
        }

        public PropFactory PropFactory
        {
            get
            {
                return propFactory;
            }
        }

        public AtlasPluginManager AtlasPluginManager
        {
            get
            {
                return atlasPluginManager;
            }
        }

        public GUIManager GUIManager
        {
            get
            {
                return guiManager;
            }
        }

        public StandaloneApp App
        {
            get
            {
                return app;
            }
        }

        public TimelineController TimelineController
        {
            get
            {
                return timelineController;
            }
        }

        public ExamController ExamController
        {
            get
            {
                return examController;
            }
        }

        public TaskController TaskController
        {
            get
            {
                return taskController;
            }
        }

        public TimelineGUIFactory TimelineGUIFactory { get; private set; }

        public DocumentController DocumentController { get; private set; }

        public void recreateMainWindow()
        {
            //sceneViewController.destroyCameras();
            //MyGUIInterface.Instance.destroyViewport();
            //medicalController.destroyInputHandler();

            //RendererWindow window = OgreInterface.Instance.recreatePrimaryWindow();

            //medicalController.recreateInputHandler(window.Handle);
            //MyGUIInterface.Instance.recreateViewport(window);
            //sceneViewController.changeRendererWindow(window);
            //sceneViewController.createCameras(medicalController.CurrentScene);
            //window.Handle.addListener(windowListener);
            //basicGUI.windowChanged(window.Handle);

            MessageBox.show("You will need to restart the program to apply your settings.\nWould you like to shut down now?", "Apply Changes?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, displayParameterChangeCallback);
        }

        public void saveCrashLog()
        {
            if (medicalController != null)
            {
                medicalController.saveCrashLog();
            }
        }

        void displayParameterChangeCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                this.exit();
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
        {
            bool success = false;
            sceneViewController.resetAllCameraPositions();
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            sceneViewController.destroyCameras();
            background.destroyBackground();
            backgroundController.sceneUnloading();
            if (medicalController.openScene(file))
            {
                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (BeforeSceneLoadProperties != null)
                {
                    BeforeSceneLoadProperties.Invoke(medicalController.CurrentScene);
                }
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();
                    backgroundController.sceneLoaded(ogreScene);
                    background.createBackground(ogreScene);

                    sceneViewController.createCameras(medicalController.CurrentScene);
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();

                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }
                }
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out WindowInfo defaultWindow)
        {
            defaultWindow = new WindowInfo(mainWindow, "Primary");
            defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
            defaultWindow.MonitorIndex = 0;

            if (MedicalConfig.EngineConfig.Fullscreen)
            {
                mainWindow.showFullScreen();
                mainWindow.setSize(MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            }
            else
            {
                mainWindow.Maximized = true;
                mainWindow.show();
            }
        }

        /// <summary>
        /// Called before MyGUI renders.
        /// </summary>
        void myGUI_RenderStarted(object sender, EventArgs e)
        {
            watermark.Visible = false;
            measurementGrid.HideCaption = true;
        }

        /// <summary>
        /// Called after MyGUI renders.
        /// </summary>
        void myGUI_RenderEnded(object sender, EventArgs e)
        {
            watermark.Visible = true;
            measurementGrid.HideCaption = false;
        }

        void medicalController_FullSpeedLoopUpdate(Clock time)
        {
            ThreadManager.doInvoke();
        }

        /// <summary>
        /// This matches ogre perfectly, leave it alone and calculate the correct orientation for input.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        Matrix4x4 makeViewMatrix(Vector3 position, Quaternion orientation) 
		    //,const Matrix4x4 reflectMatrix)
	    {
		    Matrix4x4 viewMatrix;

		    // View matrix is:
		    //
		    //  [ Lx  Uy  Dz  Tx  ]
		    //  [ Lx  Uy  Dz  Ty  ]
		    //  [ Lx  Uy  Dz  Tz  ]
		    //  [ 0   0   0   1   ]
		    //
		    // Where T = -(Transposed(Rot) * Pos)

		    // This is most efficiently done using 3x3 Matrices
		    Matrix3x3 rot = orientation.toRotationMatrix();

		    // Make the translation relative to new axes
		    Matrix3x3 rotT = rot.transpose();
		    Vector3 trans = -rotT * position;

		    // Make final matrix
		    viewMatrix = Matrix4x4.Identity;
		    viewMatrix.setRotation(rotT); // fills upper 3x3
		    viewMatrix.m03 = trans.x;
            viewMatrix.m13 = trans.y;
            viewMatrix.m23 = trans.z;

		    // Deal with reflections
            //if (reflectMatrix)
            //{
            //    viewMatrix = viewMatrix * (*reflectMatrix);
            //}

		    return viewMatrix;
	    }

        public Quaternion shortestArcQuatFixedYaw(ref Vector3 v0, ref Vector3 v1)
        {
            throw new NotImplementedException();
        }

        void medicalController_FixedLoopUpdate(Clock time)
        {
            //Log.Debug("Matrix test");
            //Matrix3x3 matrix = new Matrix3x3(0, 1, 2, 3, 4, 5, 6, 7, 8);
            //for (int i = 0; i < 3; ++i)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        Log.Debug("Matrix index {0}, {1} is {2}", i, j, matrix[i, j]);
            //    }
            //}

            Log.Debug("------------------------------------");

            Vector3 localTrans = new Vector3(15, -17, 29);
            SceneViewWindow sceneWindow = sceneViewController.ActiveWindow;
            float aspect = sceneWindow.Camera.getAspectRatio();
            float fovy = sceneWindow.Camera.getFOVy() * 0.5f;

            Vector3 direction = sceneWindow.LookAt - sceneWindow.Translation;
            direction.normalize();
            Log.Debug("My direction {0} length {1}", direction, direction.length());

            Quaternion viewUpdateOrientation = sceneWindow.Camera.getOrientationForViewUpdate();
            Log.Debug("Ogre direction {0}", Quaternion.quatRotate(viewUpdateOrientation, Vector3.Forward));

            //Figure out direction, must use ogre fixed yaw calculation
            Vector3 zAdjustVec = -direction;
            zAdjustVec.normalize();
            Vector3 mYawFixedAxis = Vector3.Up;
            Vector3 xVec = mYawFixedAxis.cross(ref zAdjustVec);
            xVec.normalize();

            Vector3 yVec = zAdjustVec.cross(ref xVec);
            yVec.normalize();

            Quaternion targetWorldOrientation = new Quaternion();
            targetWorldOrientation.fromAxes(xVec, yVec, zAdjustVec);


            Log.Debug("My orientation {0}", targetWorldOrientation);// Quaternion.shortestArcQuat(ref Vector3.Forward, ref direction).normalize());//Vector3.Forward.getRotationTo(direction));
            Log.Debug("Ogre orientatin {0}", viewUpdateOrientation);

            //Log.Debug("My ogre view matrix\n {0}\n ogre\n {1}", makeViewMatrix(sceneWindow.Translation, viewUpdateOrientation).DisplayString, sceneWindow.Camera.getViewMatrix().DisplayString);

            //Log.Debug("My Matrix      --   {0}\n{1}", myMatrix * localTrans, myMatrix.DisplayString);
            //Log.Debug("Ogre Matrix    --   {0}\n{1}", viewMatrix * localTrans, viewMatrix.DisplayString);
            //Log.Debug("My Result   {0}", SceneViewWindow.computeOffsetToIncludePoint(myMatrix, localTrans, aspect, fovy));
            //Log.Debug("Ogre Result {0}", SceneViewWindow.computeOffsetToIncludePoint(viewMatrix, localTrans, aspect, fovy));

            //Log.Debug("My Right   {0,10}, {1,10}, {2,10}", myMatrix.m00, myMatrix.m10, myMatrix.m20);
            //Log.Debug("Ogre Right {0,10}, {1,10}, {2,10}", viewMatrix.m00, viewMatrix.m10, viewMatrix.m20);

            //Log.Debug("My Up   {0,10}, {1,10}, {2,10}", myMatrix.m01, myMatrix.m11, myMatrix.m21);
            //Log.Debug("Ogre Up {0,10}, {1,10}, {2,10}", viewMatrix.m01, viewMatrix.m11, viewMatrix.m21);

            //Log.Debug("My Forward   {0,10}, {1,10}, {2,10}", myMatrix.m02, myMatrix.m12, myMatrix.m22);
            //Log.Debug("Ogre Forward {0,10}, {1,10}, {2,10}", viewMatrix.m02, viewMatrix.m12, viewMatrix.m22);

            //Log.Debug("My CamPos   {0,10}, {1,10}, {2,10}", myMatrix.m03, myMatrix.m13, myMatrix.m23);
            //Log.Debug("Ogre CamPos {0,10}, {1,10}, {2,10}", viewMatrix.m03, viewMatrix.m13, viewMatrix.m23);

            //Log.Debug("My LookAtDirection {0}", ((sceneWindow.LookAt - sceneWindow.Translation).normalized()));
            //Log.Debug("Ogre LookAtDirection {0}", Quaternion.quatRotate(sceneWindow.Camera.getDerivedOrientation(), Vector3.Forward));
        }

        void timelineController_PlaybackStopped(object sender, EventArgs e)
        {
            guiManager.setMainInterfaceEnabled(true);
            timelineController.ResourceProvider = null;
        }

        void timelineController_PlaybackStarted(object sender, EventArgs e)
        {
            guiManager.setMainInterfaceEnabled(false);
        }
    }
}