using FreeImageAPI;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    public class RocketTextWriter : ImageTextWriter
    {

        const String DocumentStart = @"<rml>
	<head>
		<link type=""text/template"" href=""~/Medical.GUI.RocketTextWriter.RocketTextWriterTemplate.trml"" />
	</head>
	<body template=""RocketTextWriterTemplate"" style=""background-color:{0}00;color:#ffffff;font-size:{1}px;font-weight:bold;"">";
        const String DocumentEnd = "</body></rml>";

        public void writeText(FreeImageBitmap bitmap, string p, int fontSize)
        {
            p = p.Replace("\n", "<br/>");

            int width = Math.Min(bitmap.Width, 2048);
            int height = Math.Min(bitmap.Height, 256);

            var imageBox = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, width, height, Align.Default, "Overlapped", "TempImageBox");
            try
            {
                imageBox.Visible = false;
                using (RocketWidget rocketWidget = new RocketWidget(imageBox, true))
                {
                    var cornerColor = Engine.Color.FromARGB(bitmap.GetPixel(0, 0).ToArgb());
                    rocketWidget.resized();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat(DocumentStart, cornerColor.ToHexString(), fontSize);
                    sb.AppendFormat("<p>{0}</p>", p);
                    sb.Append(DocumentEnd);

                    var resourceLoader = new RocketAssemblyResourceLoader(this.GetType().Assembly);
                    RocketInterface.Instance.FileInterface.addExtension(resourceLoader);

                    var document = rocketWidget.Context.LoadDocumentFromMemory(sb.ToString());
                    if (document != null)
                    {
                        document.Show(ElementDocument.FocusFlags.NONE);
                    }

                    RocketInterface.Instance.FileInterface.removeExtension(resourceLoader);

                    using (FreeImageBitmap copyrightBitmap = new FreeImageBitmap(imageBox.Width, imageBox.Height, PixelFormat.Format32bppArgb))
                    {
                        rocketWidget.writeToGraphics(copyrightBitmap, new Rectangle(0, 0, copyrightBitmap.Width, copyrightBitmap.Height));
                        using (FreeImageBitmap background = bitmap.Copy(0, 0, Math.Min(copyrightBitmap.Width, bitmap.Width), Math.Min(copyrightBitmap.Height, bitmap.Height)))
                        {
                            background.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP);
                            copyrightBitmap.Composite(false, null, background);
                            bitmap.Paste(copyrightBitmap, 0, 0, int.MaxValue);
                        }
                    }
                }
            }
            finally
            {
                Gui.Instance.destroyWidget(imageBox);
            }
        }
    }
}
