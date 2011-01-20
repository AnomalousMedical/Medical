using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Platform;
using Engine.Resources;
using System.Xml;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical.Controller
{
    public delegate void NavigationControllerEvent(NavigationController controller);

    public class NavigationController : IDisposable
    {
        public event NavigationControllerEvent NavigationStateSetChanged;

        private NavigationStateSet navigationSet;
        private String currentCameraFile;

        public NavigationController()
        {

        }

        public void Dispose()
        {
            if (navigationSet != null)
            {
                navigationSet.Dispose();
            }
        }

        public bool loadNavigationSetIfDifferent(String cameraFile)
        {
            if (cameraFile != currentCameraFile)
            {
                loadNavigationSet(cameraFile);
                return true;
            }
            return false;
        }

        public void loadNavigationSet(String cameraFile)
        {
            currentCameraFile = cameraFile;
            NavigationSet = loadNavSet(cameraFile);
        }

        public void mergeNavigationSet(String cameraFile)
        {
            using (NavigationStateSet navSet = loadNavSet(cameraFile))
            {
                if (navSet != null)
                {
                    if (NavigationSet == null)
                    {
                        currentCameraFile = cameraFile;
                        NavigationSet = navSet;
                    }
                    else
                    {
                        NavigationSet.mergeStates(navSet);
                    }
                }
            }
        }

        private NavigationStateSet loadNavSet(String cameraFile)
        {
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            if (archive.exists(cameraFile))
            {
                try
                {
                    using (XmlTextReader textReader = new XmlTextReader(archive.openStream(cameraFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                    {
                        return NavigationSerializer.readNavigationStateSet(textReader);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error loading navigation file.\n{0}", ex.Message);
                }
            }
            return null;
        }

        public void saveNavigationSet(String cameraFile)
        {
            using (XmlTextWriter textWriter = new XmlTextWriter(cameraFile, Encoding.Default))
            {
                textWriter.Formatting = Formatting.Indented;
                NavigationSerializer.writeNavigationStateSet(navigationSet, textWriter);
            }
        }

        public NavigationState getState(String name)
        {
            if (navigationSet != null)
            {
                return navigationSet.getState(name);
            }
            return null;
        }

        public void setNavigationState(String name, SceneViewWindow window)
        {
            NavigationState state = getState(name);
            if (state != null)
            {
                window.setPosition(state.Translation, state.LookAt);
            }
        }

        public NavigationState findClosestNonHiddenState(Vector3 position)
        {
            if (navigationSet != null)
            {
                return navigationSet.findClosestNonHiddenState(position);
            }
            return null;
        }

        public String CurrentCameraFile
        {
            get
            {
                return currentCameraFile;
            }
        }

        public NavigationStateSet NavigationSet
        {
            get
            {
                return navigationSet;
            }
            set
            {
                if (navigationSet != value)
                {
                    if (navigationSet != null)
                    {
                        navigationSet.Dispose();
                    }
                    navigationSet = value;
                    if (NavigationStateSetChanged != null)
                    {
                        NavigationStateSetChanged.Invoke(this);
                    }
                }
            }
        }
    }
}
