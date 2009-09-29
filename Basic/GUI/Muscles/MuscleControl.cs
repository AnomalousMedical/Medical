using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Medical.Properties;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Medical.Muscles;
using Engine.Resources;
using System.Xml;
using Engine.Platform;

namespace Medical.GUI
{
    public partial class MuscleControl : GUIElement
    {
        private const String RightTemporalisDynamic = "RightTemporalisDynamic";
        private const String RightMasseterDynamic = "RightMasseterDynamic";
        private const String RightMedialPterygoidDynamic = "RightMedialPterygoidDynamic";
        private const String RightLateralPterygoidDynamic = "RightLateralPterygoidDynamic";
        private const String RightDigastricDynamic = "RightDigastricDynamic";

        private const String LeftTemporalisDynamic = "LeftTemporalisDynamic";
        private const String LeftMasseterDynamic = "LeftMasseterDynamic";
        private const String LeftMedialPterygoidDynamic = "LeftMedialPterygoidDynamic";
        private const String LeftLateralPterygoidDynamic = "LeftLateralPterygoidDynamic";
        private const String LeftDigastricDynamic = "LeftDigastricDynamic";

        private const float CP_MOVE_SPEED = 0.25f;

        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;

        private XmlSaver xmlSaver = new XmlSaver();
        private MovementSequence currentSequence;
        private float time;

        public MuscleControl()
        {
            InitializeComponent();
            muscleSequenceView.LargeImageList = new ImageList();
            muscleSequenceView.LargeImageList.ImageSize = new Size(108, 120);
            muscleSequenceView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            muscleSequenceView.LargeImageList.Images.Add("OpenIcon", Resources.openmuscle);
            muscleSequenceView.LargeImageList.Images.Add("CloseIcon", Resources.clenchedmuscle);
            muscleSequenceView.SequenceActivated += new MuscleSequenceActivated(muscleSequenceView_SequenceActivated);
            playbackTrackBar.TimeChanged += new TimeChanged(playbackTrackBar_TimeChanged);
        }

        void playbackTrackBar_TimeChanged(TimeTrackBar trackBar, double currentTime)
        {
            if (currentSequence != null)
            {
                currentSequence.setPosition((float)currentTime);
            }
        }

        void muscleSequenceView_SequenceActivated(string sequenceText, string sequenceFile)
        {
            using (Archive archive = FileSystem.OpenArchive(sequenceFile))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(sequenceFile, FileMode.Open, FileAccess.Read)))
                {
                    currentSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                }
            }
            playbackPanel.Enabled = currentSequence != null;
            if(playbackPanel.Enabled)
            {
                playbackTrackBar.MaximumTime = currentSequence.Duration;
                playbackTrackBar.CurrentTime = 0;
                time = 0.0f;
            }
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            this.Enabled = leftCP != null && rightCP != null;
            SimSubScene defaultScene = scene.getDefaultSubScene();
            if (defaultScene != null)
            {
                SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                muscleSequenceView.initializeSequences(medicalScene, MedicalController.CurrentSceneDirectory);
            }
        }

        protected override void sceneUnloading()
        {
            leftCP = null;
            rightCP = null;
            muscleSequenceView.clearSequences();
        }

        protected override void fixedLoopUpdate(Clock time)
        {
            base.fixedLoopUpdate(time);
            this.time += (float)time.Seconds;
            playbackTrackBar.CurrentTime = this.time % playbackTrackBar.MaximumTime;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            time = 0.0f;
            subscribeToUpdates();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            unsubscribeFromUpdates();
        }
    }
}
