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
    class DumpToMax : Task
    {
        private MedicalController medicalController;

        public DumpToMax(MedicalController medicalController)
            : base("Developer.DumpToMax", "Dump Positions to 3ds Max", CommonResources.NoIcon, "Developer")
        {
            this.ShowOnTaskbar = false;
            this.medicalController = medicalController;
        }

        public override void clicked(TaskPositioner taskPositioner)
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
                fireItemClosed();
            });
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
