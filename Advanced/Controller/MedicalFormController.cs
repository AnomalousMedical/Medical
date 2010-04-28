using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    public interface MedicalFormController
    {
        void stop();

        void newScene();

        void saveMedicalState(string p);

        void openStates(string p);

        void createNewMedicalStates();

        void createMedicalState(string p);

        void saveSequence(string p);

        void loadSequence(string p);

        void createNewSequence();

        void cloneActiveWindow();

        void setFourWindowLayout();

        void setThreeWindowLayout();

        void setTwoWindowLayout();

        void setOneWindowLayout();

        void showOptions();

        void applyDistortion(String filename);

        DrawingWindowController DrawingWindowController
        {
            get;
        }

        void changeRenderingMode(RenderingMode renderingMode);
    }
}
