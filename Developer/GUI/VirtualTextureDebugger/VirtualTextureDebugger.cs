using Anomalous.GuiFramework;
using Engine;
using Medical;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developer.GUI
{
    class VirtualTextureDebugger : MDIDialog
    {
        ComboBox textureCombo;
        ImageBox textureImage;

        public VirtualTextureDebugger(VirtualTextureManager virtualTextureManager)
            : base("Developer.GUI.VirtualTextureDebugger.VirtualTextureDebugger.layout")
        {
            textureCombo = window.findWidget("TextureCombo") as ComboBox;
            textureCombo.EventComboAccept += textureCombo_EventComboAccept;
            foreach(String textureName in virtualTextureManager.TextureNames)
            {
                textureCombo.addItem(textureName);
            }

            textureImage = window.findWidget("TextureImage") as ImageBox;
        }

        void textureCombo_EventComboAccept(Widget source, EventArgs e)
        {
            if (textureCombo.SelectedIndex != ComboBox.Invalid)
            {
                textureImage.setImageTexture(textureCombo.SelectedItemName);
            }
        }
    }
}
