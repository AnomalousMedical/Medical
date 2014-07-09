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
    class MaxExport : MDIDialog
    {
        private MedicalController medicalController;

        public MaxExport(StandaloneController standaloneController)
            : base("Developer.GUI.MaxExport.MaxExport.layout")
        {
            this.medicalController = standaloneController.MedicalController;

            Button saveAll = (Button)window.findWidget("SaveAll");
            saveAll.MouseButtonClick += saveAll_MouseButtonClick;

            Button saveLeftTeethAsRight = (Button)window.findWidget("SaveLeftTeethAsRight");
            saveLeftTeethAsRight.MouseButtonClick += saveLeftTeethAsRight_MouseButtonClick;

            Button saveRightTeethAsLeft = (Button)window.findWidget("SaveRighteethAsLeft");
            saveRightTeethAsLeft.MouseButtonClick += saveRightTeethAsLeft_MouseButtonClick;
        }

        public override void Dispose()
        {
            
        }

        void saveAll_MouseButtonClick(Widget source, EventArgs e)
        {
            FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Dump Positions to 3ds Max", Environment.CurrentDirectory, "AnomalousMedicalSimObjects.ms", "MaxScript (*.ms)|*.ms");
            saveDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    using (MaxWriter maxWriter = new MaxWriter(path))
                    {
                        maxWriter.write(medicalController.SimObjects.Select(so => new MaxWriterInfo(so)));
                    }
                }
            });
        }

        void saveLeftTeethAsRight_MouseButtonClick(Widget source, EventArgs e)
        {
            Dictionary<String, MaxWriterInfo> transforms = new Dictionary<String, MaxWriterInfo>();
            buildLeftToRightTransform(transforms);
            finishTransformedSave(transforms);
        }

        void saveRightTeethAsLeft_MouseButtonClick(Widget source, EventArgs e)
        {
            Dictionary<String, MaxWriterInfo> transforms = new Dictionary<String, MaxWriterInfo>();
            buildRightToLeftTransform(transforms);
            finishTransformedSave(transforms);
        }

        private void finishTransformedSave(Dictionary<String, MaxWriterInfo> transforms)
        {
            FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Dump Positions to 3ds Max", Environment.CurrentDirectory, "AnomalousMedicalSimObjects.ms", "MaxScript (*.ms)|*.ms");
            saveDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    using (MaxWriter maxWriter = new MaxWriter(path))
                    {
                        maxWriter.write(from so in medicalController.SimObjects
                                        where transforms.ContainsKey(so.Name)
                                        select transformWriter(new MaxWriterInfo(so), transforms));
                    }
                }
            });
        }

        private void buildLeftToRightTransform(Dictionary<String, MaxWriterInfo> transforms)
        {
            transforms.Clear();

            //Top
            for (int i = 0; i < 8; ++i)
            {
                var src = 16 - i;
                var dest = 1 + i;
                transforms.Add(String.Format("Tooth{0:D2}", src), new MaxWriterInfo()
                {
                    Name = String.Format("Tooth{0:D2}", dest),
                    MeshName = String.Format("PerfTooth{0:D2}", dest),
                    Translation = new Vector3(-1, 1, 1),
                    Rotation = new Vector3(1, -1, -1)
                });
            }

            //Bottom
            for (int i = 0; i < 8; ++i)
            {
                var src = 17 + i;
                var dest = 32 - i;
                transforms.Add(String.Format("Tooth{0:D2}", src), new MaxWriterInfo()
                {
                    Name = String.Format("Tooth{0:D2}", dest),
                    MeshName = String.Format("PerfTooth{0:D2}", dest),
                    Translation = new Vector3(-1, 1, 1),
                    Rotation = new Vector3(1, -1, -1)
                });
            }
        }

        private void buildRightToLeftTransform(Dictionary<String, MaxWriterInfo> transforms)
        {
            transforms.Clear();

            //Top
            for (int i = 0; i < 8; ++i)
            {
                var src = 1 + i;
                var dest = 16 - i;
                transforms.Add(String.Format("Tooth{0:D2}", src), new MaxWriterInfo()
                {
                    Name = String.Format("Tooth{0:D2}", dest),
                    MeshName = String.Format("PerfTooth{0:D2}", dest),
                    Translation = new Vector3(-1, 1, 1),
                    Rotation = new Vector3(1, -1, -1)
                });
            }

            //Bottom
            for (int i = 0; i < 8; ++i)
            {
                var src = 32 - i;
                var dest = 17 + i;
                transforms.Add(String.Format("Tooth{0:D2}", src), new MaxWriterInfo()
                {
                    Name = String.Format("Tooth{0:D2}", dest),
                    MeshName = String.Format("PerfTooth{0:D2}", dest),
                    Translation = new Vector3(-1, 1, 1),
                    Rotation = new Vector3(1, -1, -1)
                });
            }
        }

        private MaxWriterInfo transformWriter(MaxWriterInfo original, Dictionary<String, MaxWriterInfo> transforms)
        {
            MaxWriterInfo transform;
            if (transforms.TryGetValue(original.Name, out transform))
            {
                original.Name = transform.Name;
                original.MeshName = transform.MeshName;
                original.Translation *= transform.Translation;
                original.Rotation *= transform.Rotation;
            }
            return original;
        }
    }
}
