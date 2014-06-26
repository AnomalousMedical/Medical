using Engine;
using Engine.ObjectManagement;
using Medical;
using Medical.GUI;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Developer
{
    class DumpTeethToMax : Task
    {
        private MedicalController medicalController;

        private Dictionary<String, MaxWriterInfo> transforms = new Dictionary<String, MaxWriterInfo>();

        public DumpTeethToMax(MedicalController medicalController)
            : base("Developer.DumpTeethToMax", "Dump Teeth Positions to 3ds Max", CommonResources.NoIcon, "Developer")
        {
            this.ShowOnTaskbar = false;
            this.medicalController = medicalController;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            buildLeftToRightTransform();
            FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Dump Positions to 3ds Max", Environment.CurrentDirectory, "AnomalousMedicalSimObjects.ms", "MaxScript (*.ms)|*.ms");
            saveDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    using (MaxWriter maxWriter = new MaxWriter(path))
                    {
                        maxWriter.write(from so in medicalController.SimObjects
                                        where transforms.ContainsKey(so.Name)
                                        select transformWriter(new MaxWriterInfo(so)));
                    }
                }
                fireItemClosed();
            });
        }

        private void buildLeftToRightTransform()
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

        private void buildRightToLeftTransform()
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

        private MaxWriterInfo transformWriter(MaxWriterInfo original)
        {
            MaxWriterInfo transform;
            if(transforms.TryGetValue(original.Name, out transform))
            {
                original.Name = transform.Name;
                original.MeshName = transform.MeshName;
                original.Translation *= transform.Translation;
                original.Rotation *= transform.Rotation;
            }
            return original;
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }
    }
}
