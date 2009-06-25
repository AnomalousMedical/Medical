using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

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

        public MuscleControl()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 30.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 30.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 30.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 30.0f);
            //TeethController.setTeethLoose(true);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 50.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 0.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 0.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 50.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 0.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 0.0f);
            //TeethController.setTeethLoose(true);
        }

        private void clenchButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 100.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 100.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 100.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 100.0f);
            //TeethController.setTeethLoose(true);
        }

        private void neutralButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 0.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 0.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 0.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 0.0f);
            //TeethController.setTeethLoose(true);
        }
    }
}
