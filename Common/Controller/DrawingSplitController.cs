using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;

namespace Medical.Controller
{
    public class DrawingSplitController
    {
        DrawingWindow camera1 = new DrawingWindow();
        DrawingWindow camera2 = new DrawingWindow();
        DrawingWindow camera3 = new DrawingWindow();
        DrawingWindow camera4 = new DrawingWindow();
        private MedicalController medicalController;
        
        public DrawingSplitController()
        {

        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;

            //CameraSection cameras = AnomalyConfig.CameraSection;
            camera1.initialize("UpperLeft", this);
            camera1.Dock = DockStyle.Fill;
            medicalController.MedicalForm.addDockContent(camera1);

            camera2.initialize("UpperRight", this);
            camera2.Dock = DockStyle.Fill;
            medicalController.MedicalForm.addDockContent(camera2);

            camera3.initialize("BottomLeft", this);
            camera3.Dock = DockStyle.Fill;
            medicalController.MedicalForm.addDockContent(camera3);

            camera4.initialize("BottomRight", this);
            camera4.Dock = DockStyle.Fill;
            medicalController.MedicalForm.addDockContent(camera4);
        }
    }
}
