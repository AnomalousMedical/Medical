using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class SceneErrorWindow : MDIDialog
    {
        ImageBox rmlImage;
        RocketWidget rocketWidget;
        GUIManager guiManager;

        public SceneErrorWindow(GUIManager guiManager)
            : base("Medical.GUI.SceneErrorWindow.SceneErrorWindow.layout")
        {
            rmlImage = (ImageBox)window.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage, false);

            window.WindowChangedCoord += window_WindowChangedCoord;
            this.guiManager = guiManager;
            guiManager.addManagedDialog(this);
            guiManager.autoDisposeDialog(this);

            rocketWidget.Context.UnloadAllDocuments();

            StringBuilder htmlString = new StringBuilder();

            htmlString.Append(DocumentStart);

            foreach (var error in SimObjectErrorManager.Errors)
            {
                htmlString.AppendFormat(ErrorLine, error.Subsystem, error.SimObject, error.Type, error.ElementName, error.Message);
            }

            htmlString.Append(DocumentEnd);

            var resourceLoader = new RocketAssemblyResourceLoader(this.GetType().Assembly);
            RocketInterface.Instance.FileInterface.addExtension(resourceLoader);
            using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(htmlString.ToString()))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
                    rocketWidget.renderOnNextFrame();
                }
            }
            RocketInterface.Instance.FileInterface.removeExtension(resourceLoader);
        }

        public override void Dispose()
        {
            guiManager.removeManagedDialog(this);
            rocketWidget.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }

        const String DocumentStart = @"<rml>
	<head>
		<link type=""text/template"" href=""Medical.GUI.SceneErrorWindow.ErrorTemplate.trml"" />
	</head>
	<body template=""ErrorTemplate"">";

        const String DocumentEnd = "</body></rml>";

        const String ErrorLine = "<p><span class=\"Subsystem\">{0}</span>&nbsp;<span class=\"SimObject\">{1}</span>&nbsp;<span class=\"Type\">{2}</span>&nbsp;<span class=\"ElementName\">{3}</span>&nbsp;<span class=\"Reason\">{4}</span></p>";
    }
}
