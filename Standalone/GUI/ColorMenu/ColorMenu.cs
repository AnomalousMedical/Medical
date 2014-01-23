using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Logging;

namespace Medical.GUI
{
    public class ColorMenu : PopupContainer
    {
        private static bool colorsLoaded = false;
        public delegate void ColorChosenDelegate(Color color);

        public static ColorMenu ShowColorMenu(int left, int top, ColorChosenDelegate colorChosenCallback, Action colorClearedCallback = null)
        {
            ColorMenu colorMenu = new ColorMenu(colorChosenCallback, colorClearedCallback);
            colorMenu.show(left, top);
            return colorMenu;
        }

        private ColorChosenDelegate colorChosenCallback;
        private Action colorClearedCallback;
        private SingleSelectButtonGrid colorGrid;
        private Color customColor = Color.Black;

        private ColorMenu(ColorChosenDelegate colorChosenCallback, Action colorClearedCallback)
            :base("Medical.GUI.ColorMenu.ColorMenu.layout")
        {
            this.colorChosenCallback = colorChosenCallback;
            this.colorClearedCallback = colorClearedCallback;

            colorGrid = new SingleSelectButtonGrid(widget.findWidget("ColorGrid") as ScrollView);
            for (int i = 0; i < 77; ++i)
            {
                ButtonGridItem item = colorGrid.addItem("Main", "", "Colors/" + i);
                item.UserObject = i;
            }
            colorGrid.SelectedValueChanged += new EventHandler(colorGrid_SelectedValueChanged);

            Button moreColorsButton = widget.findWidget("MoreColorsButton") as Button;
            moreColorsButton.MouseButtonClick += new MyGUIEvent(moreColorsButton_MouseButtonClick);

            this.Hidden += new EventHandler(ColorMenu_Hidden);

            Button clearColorButton = (Button)widget.findWidget("ClearColorButton");
            clearColorButton.Visible = colorClearedCallback != null;
            clearColorButton.MouseButtonClick += clearColorButton_MouseButtonClick;
        }

        protected override void loadResources()
        {
            if (!colorsLoaded)
            {
                ResourceManager.Instance.load("Medical.GUI.ColorMenu.Colors.xml");
                colorsLoaded = true;
            }
        }

        public Color SelectedColor
        {
            get
            {
                if (colorGrid.SelectedItem != null)
                {
                    return colorTable[(int)colorGrid.SelectedItem.UserObject];
                }
                return customColor;
            }
        }

        void colorGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            if (colorChosenCallback != null)
            {
                colorChosenCallback.Invoke(SelectedColor);
            }
            this.hide();
        }

        void moreColorsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Color selectedColor = SelectedColor;
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = selectedColor;
            colorDialog.showModal((result, color) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    colorGrid.SelectedItem = null;
                    customColor = color;
                    colorGrid_SelectedValueChanged(this, EventArgs.Empty);
                }
            });
        }

        void clearColorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (colorClearedCallback != null)
            {
                colorClearedCallback.Invoke();
            }
            this.hide();
        }

        void ColorMenu_Hidden(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private static Color[] colorTable = {
                                         //Row 1
                                         new Color(1, 1, 1),
                                         new Color(0, 0, 0),
                                         new Color(238 / 255f, 236 / 255f, 225 / 255f),
                                         new Color(31 / 255f, 73 / 255f, 125 / 255f),
                                         new Color(79 / 255f, 129 / 255f, 189 / 255f),
                                         new Color(192 / 255f, 80 / 255f, 77 / 255f),
                                         new Color(155 / 255f, 187 / 255f, 89 / 255f),
                                         new Color(128 / 255f, 100 / 255f, 162 / 255f),
                                         new Color(128 / 255f, 0 / 255f, 255 / 255f),
                                         new Color(75 / 255f, 172 / 255f, 198 / 255f),
                                         new Color(247 / 255f, 150 / 255f, 70 / 255f),

                                         //Row 2
                                         new Color(242 / 255f, 242 / 255f, 242 / 255f),
                                         new Color(127 / 255f, 127 / 255f, 127 / 255f),
                                         new Color(221 / 255f, 217 / 255f, 195 / 255f),
                                         new Color(198 / 255f, 217 / 255f, 240 / 255f),
                                         new Color(219 / 255f, 229 / 255f, 241 / 255f),
                                         new Color(242 / 255f, 220 / 255f, 219 / 255f),
                                         new Color(235 / 255f, 241 / 255f, 221 / 255f),
                                         new Color(229 / 255f, 224 / 255f, 236 / 255f),
                                         new Color(238 / 255f, 221 / 255f, 255 / 255f),
                                         new Color(219 / 255f, 238 / 255f, 243 / 255f),
                                         new Color(253 / 255f, 234 / 255f, 218 / 255f),

                                         //Row 3
                                         new Color(216 / 255f, 216 / 255f, 216 / 255f),
                                         new Color(89 / 255f, 89 / 255f, 89 / 255f),
                                         new Color(196 / 255f, 189 / 255f, 151 / 255f),
                                         new Color(141 / 255f, 179 / 255f, 226 / 255f),
                                         new Color(184 / 255f, 204 / 255f, 228 / 255f),
                                         new Color(229 / 255f, 185 / 255f, 183 / 255f),
                                         new Color(215 / 255f, 227 / 255f, 188 / 255f),
                                         new Color(204 / 255f, 193 / 255f, 217 / 255f),
                                         new Color(221 / 255f, 187 / 255f, 255 / 255f),
                                         new Color(183 / 255f, 221 / 255f, 232 / 255f),
                                         new Color(251 / 255f, 213 / 255f, 181 / 255f),

                                         //Row 4
                                         new Color(191 / 255f, 191 / 255f, 191 / 255f),
                                         new Color(63 / 255f, 63 / 255f, 63 / 255f),
                                         new Color(147 / 255f, 137 / 255f, 83 / 255f),
                                         new Color(84 / 255f, 141 / 255f, 212 / 255f),
                                         new Color(149 / 255f, 179 / 255f, 215 / 255f),
                                         new Color(217 / 255f, 150 / 255f, 148 / 255f),
                                         new Color(195 / 255f, 214 / 255f, 155 / 255f),
                                         new Color(178 / 255f, 162 / 255f, 199 / 255f),
                                         new Color(209 / 255f, 164 / 255f, 255 / 255f),
                                         new Color(146 / 255f, 205 / 255f, 220 / 255f),
                                         new Color(250 / 255f, 192 / 255f, 143 / 255f),

                                         //Row 5
                                         new Color(165 / 255f, 165 / 255f, 165 / 255f),
                                         new Color(38 / 255f, 38 / 255f, 38 / 255f),
                                         new Color(73 / 255f, 68 / 255f, 41 / 255f),
                                         new Color(23 / 255f, 54 / 255f, 93 / 255f),
                                         new Color(54 / 255f, 96 / 255f, 146 / 255f),
                                         new Color(149 / 255f, 55 / 255f, 52 / 255f),
                                         new Color(118 / 255f, 146 / 255f, 60 / 255f),
                                         new Color(95 / 255f, 73 / 255f, 122 / 255f),
                                         new Color(111 / 255f, 0 / 255f, 221 / 255f),
                                         new Color(49 / 255f, 133 / 255f, 155 / 255f),
                                         new Color(227 / 255f, 108 / 255f, 9 / 255f),

                                         //Row 6
                                         new Color(128 / 255f, 128 / 255f, 128 / 255f),
                                         new Color(12 / 255f, 12 / 255f, 12 / 255f),
                                         new Color(29 / 255f, 27 / 255f, 16 / 255f),
                                         new Color(15 / 255f, 36 / 255f, 62 / 255f),
                                         new Color(36 / 255f, 64 / 255f, 97 / 255f),
                                         new Color(99 / 255f, 36 / 255f, 35 / 255f),
                                         new Color(79 / 255f, 97 / 255f, 40 / 255f),
                                         new Color(63 / 255f, 49 / 255f, 81 / 255f),
                                         new Color(70 / 255f, 0 / 255f, 140 / 255f),
                                         new Color(32 / 255f, 88 / 255f, 103 / 255f),
                                         new Color(151 / 255f, 72 / 255f, 6 / 255f),

                                         //Row 7
                                         new Color(192 / 255f, 0 / 255f, 0 / 255f),
                                         new Color(255 / 255f, 0 / 255f, 0 / 255f),
                                         new Color(255 / 255f, 192 / 255f, 0 / 255f),
                                         new Color(255 / 255f, 255 / 255f, 0 / 255f),
                                         new Color(146 / 255f, 208 / 255f, 80 / 255f),
                                         new Color(0 / 255f, 176 / 255f, 80 / 255f),
                                         new Color(0 / 255f, 176 / 255f, 240 / 255f),
                                         new Color(0 / 255f, 112 / 255f, 192 / 255f),
                                         new Color(255 / 255f, 0 / 255f, 255 / 255f),
                                         new Color(0 / 255f, 32 / 255f, 96 / 255f),
                                         new Color(112 / 255f, 48 / 255f, 160 / 255f),
                                     };
    }
}
