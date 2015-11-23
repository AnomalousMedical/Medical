using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Editing;

namespace Medical
{
    public class AnatomyManager
    {
        public enum CustomQueries
        {
            BuildBrowser
        }

        private AnatomyManager() { }

        private static List<AnatomyIdentifier> anatomyList = new List<AnatomyIdentifier>();
        private static AnatomyOrganizer anatomyOrganizer;

        public static void addAnatomy(AnatomyIdentifier anatomy)
        {
            anatomyList.Add(anatomy);
        }

        public static void removeAnatomy(AnatomyIdentifier anatomy)
        {
            anatomyList.Remove(anatomy);
        }

        public static IEnumerable<AnatomyIdentifier> AnatomyList
        {
            get
            {
                return anatomyList;
            }
        }

        public static AnatomyOrganizer AnatomyOrganizer
        {
            get
            {
                return anatomyOrganizer;
            }
            internal set
            {
                if (anatomyOrganizer != null)
                {
                    Log.Warning("A second Anatomy Organizer has been set. Only one is supported per scene.");
                }
                anatomyOrganizer = value;
            }
        }

        public static SortedAnatomyClickResults findAnatomy(Ray3 ray)
        {
            float distance = 0.0f;
            SortedAnatomyClickResults results = new SortedAnatomyClickResults();
            foreach (AnatomyIdentifier anatomy in anatomyList)
            {
                if (anatomy.ShowInClickSearch && anatomy.CurrentAlpha > 0.0f && anatomy.checkCollision(ray, ref distance))
                {
                    results.add(new AnatomyClickResult(anatomy, distance));
                }
            }
            results.sort();
            return results;
        }

        public static Browser buildBrowser()
        {
            var browser = new Browser("Anatomy", "Choose Anatomy");
            foreach (var anatomy in anatomyList)
            {
                try
                {
                    browser.addNode("", new BrowserNode(anatomy.AnatomicalName, anatomy.AnatomicalName));
                }
                catch(Exception ex)
                {
                    //Exceptions are ok, this is just a list, prevents duplicates from being an issue
                }
            }
            return browser;
        }
    }
}
