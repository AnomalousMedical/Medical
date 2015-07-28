using Anomalous.GuiFramework;
using Engine;
using Medical;
using MyGUIPlugin;
using OgrePlugin;
using OgrePlugin.VirtualTexture;
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
        VirtualTextureManager virtualTextureManager;

        public VirtualTextureDebugger(VirtualTextureManager virtualTextureManager)
            : base("Developer.GUI.VirtualTextureDebugger.VirtualTextureDebugger.layout")
        {
            this.virtualTextureManager = virtualTextureManager;

            textureCombo = window.findWidget("TextureCombo") as ComboBox;
            textureCombo.EventComboAccept += textureCombo_EventComboAccept;
            foreach (String textureName in virtualTextureManager.TextureNames)
            {
                textureCombo.addItem(textureName);
            }

            textureImage = window.findWidget("TextureImage") as ImageBox;

            Button save = window.findWidget("SaveButton") as Button;
            save.MouseButtonClick += save_MouseButtonClick;

            Button reset = window.findWidget("ResetButton") as Button;
            reset.MouseButtonClick += reset_MouseButtonClick;
        }

        void textureCombo_EventComboAccept(Widget source, EventArgs e)
        {
            if (textureCombo.SelectedIndex != ComboBox.Invalid)
            {
                textureImage.setImageTexture(textureCombo.SelectedItemName);
            }
        }

        unsafe void save_MouseButtonClick(Widget source, EventArgs e)
        {
            if (textureCombo.SelectedIndex != ComboBox.Invalid)
            {
                String selectedTexture = textureCombo.SelectedItemName;
                using (var tex = TextureManager.getInstance().getByName(selectedTexture))
                {
                    int numMips = tex.Value.NumMipmaps + 1;
                    int width = (int)tex.Value.Width;
                    int height = (int)tex.Value.Height;
                    for (int mip = 0; mip < numMips; ++mip)
                    {
                        using (var buffer = tex.Value.getBuffer(0, (uint)mip))
                        {
                            using (var blitBitmap = new FreeImageAPI.FreeImageBitmap(width, height, FreeImageAPI.PixelFormat.Format32bppArgb))
                            {
                                using (var blitBitmapBox = new PixelBox(0, 0, width, height, OgreDrawingUtility.getOgreFormat(blitBitmap.PixelFormat), blitBitmap.GetScanlinePointer(0).ToPointer()))
                                {
                                    buffer.Value.blitToMemory(blitBitmapBox);
                                }

                                blitBitmap.RotateFlip(FreeImageAPI.RotateFlipType.RotateNoneFlipY);
                                using (var stream = System.IO.File.Open(selectedTexture + "_mip_" + mip + ".bmp", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                                {
                                    blitBitmap.Save(stream, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_BMP);
                                }
                            }
                            width >>= 1;
                            height >>= 1;
                        }
                    }
                }
            }
        }

        void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            virtualTextureManager.reset();
        }
    }
}
