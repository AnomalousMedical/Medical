using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using System.IO;

namespace Medical
{
    class PluginBrandingResourceItemTemplate : ProjectItemTemplate
    {
        public PluginBrandingResourceItemTemplate()
        {
            BrandingImageName = "BrandingImage.png";
            IconName = "Icon.png";
        }

        public string TypeName
        {
            get
            {
                return "Plugin Branding Resources";
            }
        }

        public string ImageName
        {
            get
            {
                return "StandaloneIcons/NoIcon";
            }
        }

        public string Group
        {
            get
            {
                return "Plugin";
            }
        }

        [Editable]
        public String BrandingImageName { get; set; }

        [Editable]
        public String IconName { get; set; }

        [Editable]
        public String PluginNamespace { get; set; }

        public void createItem(string path, EditorController editorController)
        {
            String fullPath = Path.Combine(path, "Imagesets.xml");
            using (StreamWriter imagesetsStream = new StreamWriter(editorController.ResourceProvider.openWriteStream(fullPath)))
            {
                imagesetsStream.Write(String.Format(ImagesetsText, PluginNamespace, BrandingImageName, IconName));
            }
        }

        public bool isValid(out string errorMessage)
        {
            errorMessage = null;

            if (String.IsNullOrEmpty(PluginNamespace))
            {
                errorMessage += "Please enter a value for the Plugin Namespace\n";
            }

            if (String.IsNullOrEmpty(BrandingImageName))
            {
                errorMessage += "Please enter a value for the Branding Image Name\n";
            }

            if (String.IsNullOrEmpty(IconName))
            {
                errorMessage += "Please enter a value for the Icon Name\n";
            }

            return errorMessage == null;
        }

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
                }
                return editInterface;
            }
        }

        //0 - Plugin Namespace
        //1 - Branding image name
        //2 - Icon name
        private static String ImagesetsText = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<MyGUI type=""Resource"" version=""1.1"">
    <Resource type=""ResourceImageSet"" name=""{0}/BrandingImage"">
      <Group name=""Icons"" texture=""{0}/Resources/{1}"" size=""100 100"">
        <Index name=""Icon"">
          <Frame point=""0 0""/>
        </Index>
      </Group>
      </Resource>
      <Resource type=""ResourceImageSet"" name=""{0}/Icon"">
      <Group name=""Icons"" texture=""{0}/Resources/{2}"" size=""32 32"">
        <Index name=""Icon"">
          <Frame point=""0 0""/>
        </Index>
      </Group>
    </Resource>
</MyGUI>
";
    }
}
