using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Engine.ObjectManagement;
using Engine.Resources;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Medical.Properties;

namespace Medical.GUI
{
    class SequencesGUIController
    {
        private MedicalController medicalController;
        private KryptonRibbonTab sequenceTab;

        public SequencesGUIController(BasicForm form, BasicController basicController)
        {
            this.medicalController = basicController.MedicalController;
            this.sequenceTab = form.sequencesTab;

            basicController.SceneLoaded += new SceneEvent(basicController_SceneLoaded);
        }

        void basicController_SceneLoaded(SimScene scene)
        {
            SimSubScene defaultScene = scene.getDefaultSubScene();
            sequenceTab.Groups.Clear();
            if (defaultScene != null)
            {
                SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                String sequenceDir = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;
                using (Archive archive = FileSystem.OpenArchive(sequenceDir))
                {
                    foreach (String directory in archive.listDirectories(sequenceDir, false, false))
                    {
                        KryptonRibbonGroup group = new KryptonRibbonGroup();
                        group.TextLine1 = archive.getFileInfo(directory).Name;
                        sequenceTab.Groups.Add(group);
                        KryptonRibbonGroupTriple triple = null;
                        foreach (String file in archive.listFiles(directory, false))
                        {
                            if (triple == null || triple.Items.Count > 2)
                            {
                                triple = new KryptonRibbonGroupTriple();
                                group.Items.Add(triple);
                            }
                            String fileName = archive.getFileInfo(file).Name;
                            KryptonRibbonGroupButton button = new KryptonRibbonGroupButton();
                            button.TextLine1 = fileName.Substring(0, fileName.Length - 4);
                            button.Tag = archive.getFullPath(file);
                            button.ImageLarge = Resources.SequenceIconLarge;
                            button.ImageSmall = Resources.SequenceIconSmall;
                            triple.Items.Add(button);
                        }

                    }
                }
            }
        }
    }
}
