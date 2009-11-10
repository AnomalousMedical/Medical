using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Engine;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using Logging;

namespace Medical
{
    class NavigationSerializer
    {
        private const String NAVIGATION_STATE_SET = "NavigationStateSet";
        private const String NAVIGATION_STATE = "NavigationState";
        private const String LOOK_AT = "LookAt";
        private const String TRANSLATION = "Translation";
        private const String NAME = "Name";
        private const String LINK = "Link";
        private const String BUTTON = "Button";
        private const String DESTINATION = "Destination";
        private const String SOURCE = "Source";
        private const String VISUAL_RADIUS = "VisualRadius";
        private const String HIDDEN = "IsHidden";
        private const String SHORTCUT_KEY = "ShortcutKey";
        private const String THUMBNAIL = "Thumbnail";

        private const String NAVIGATION_MENU = "NavigationMenu";
        private const String NAVIGATION_MENU_ENTRY = "NavigationMenuEntry";
        private const String TEXT = "Text";
        private const String BITMAP_SIZE = "Size";

        public static void writeNavigationStateSet(NavigationStateSet set, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(NAVIGATION_STATE_SET);
            foreach(NavigationState state in set.States)
            {
                xmlWriter.WriteStartElement(NAVIGATION_STATE);
                xmlWriter.WriteElementString(NAME, state.Name);
                xmlWriter.WriteElementString(TRANSLATION, state.Translation.ToString());
                xmlWriter.WriteElementString(LOOK_AT, state.LookAt.ToString());
                xmlWriter.WriteElementString(HIDDEN, state.Hidden.ToString());
                xmlWriter.WriteElementString(SHORTCUT_KEY, state.ShortcutKey.ToString());
                xmlWriter.WriteEndElement();
            }

            foreach (NavigationState state in set.States)
            {
                foreach (NavigationLink link in state.AdjacentStates)
                {
                    xmlWriter.WriteStartElement(LINK);
                    xmlWriter.WriteElementString(SOURCE, state.Name);
                    xmlWriter.WriteElementString(DESTINATION, link.Destination.Name);
                    xmlWriter.WriteElementString(BUTTON, link.Button.ToString());
                    xmlWriter.WriteElementString(VISUAL_RADIUS, link.VisualRadius.ToString());
                    xmlWriter.WriteEndElement();
                }
            }
            xmlWriter.WriteStartElement(NAVIGATION_MENU);
            foreach (NavigationMenuEntry entry in set.Menus.ParentEntries)
            {
                WriteNavMenuEntry(xmlWriter, entry);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public static void WriteNavMenuEntry(XmlWriter xmlWriter, NavigationMenuEntry entry)
        {
            xmlWriter.WriteStartElement(NAVIGATION_MENU_ENTRY);
            xmlWriter.WriteElementString(TEXT, entry.Text);
            if (entry.Thumbnail != null)
            {
                xmlWriter.WriteStartElement(THUMBNAIL);
                using (MemoryStream memStream = new MemoryStream())
                {
                    entry.Thumbnail.Save(memStream, ImageFormat.Png);
                    byte[] buffer = memStream.GetBuffer();
                    xmlWriter.WriteAttributeString(BITMAP_SIZE, buffer.Length.ToString());
                    xmlWriter.WriteBinHex(buffer, 0, buffer.Length);
                }
                xmlWriter.WriteEndElement();
            }
            if (entry.SubEntries != null)
            {
                foreach (NavigationMenuEntry subEntry in entry.SubEntries)
                {
                    WriteNavMenuEntry(xmlWriter, subEntry);
                }
            }
            if (entry.States != null)
            {
                foreach (NavigationState state in entry.States)
                {
                    xmlWriter.WriteElementString(NAVIGATION_STATE, state.Name);
                }
            }
            xmlWriter.WriteEndElement();
        }

        public static NavigationStateSet readNavigationStateSet(XmlReader xmlReader)
        {
            NavigationStateSet set = new NavigationStateSet();
            while (!isEndElement(xmlReader, NAVIGATION_STATE_SET) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAVIGATION_STATE)
                    {
                        readNavigationState(set, xmlReader);
                    }
                    else if (xmlReader.Name == LINK)
                    {
                        readNavigationLink(set, xmlReader);
                    }
                    else if (xmlReader.Name == NAVIGATION_MENU)
                    {
                        readNavigationMenu(set, xmlReader);
                    }
                }
            }
            return set;
        }

        private static void readNavigationState(NavigationStateSet set, XmlReader xmlReader)
        {
            String name = null;
            Vector3 position = Vector3.UnitZ;
            Vector3 lookAt = Vector3.Zero;
            bool hidden = false;
            KeyCodes shortcutKey = KeyCodes.None;
            while (!isEndElement(xmlReader, NAVIGATION_STATE) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAME)
                    {
                        name = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == TRANSLATION)
                    {
                        position = new Vector3(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == LOOK_AT)
                    {
                        lookAt = new Vector3(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == HIDDEN)
                    {
                        hidden = bool.Parse(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == SHORTCUT_KEY)
                    {
                        shortcutKey = (KeyCodes)Enum.Parse(typeof(KeyCodes), xmlReader.ReadElementContentAsString());
                    }
                }
            }
            if (name != null)
            {
                set.addState(new NavigationState(name, lookAt, position, hidden, shortcutKey));
            }
        }

        private static void readNavigationLink(NavigationStateSet set, XmlReader xmlReader)
        {
            String source = "";
            String destination = "";
            NavigationButtons button = NavigationButtons.Up;
            float radius = 10.0f;
            while (!isEndElement(xmlReader, LINK) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == DESTINATION)
                    {
                        destination = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == SOURCE)
                    {
                        source = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == BUTTON)
                    {
                        button = (NavigationButtons)Enum.Parse(typeof(NavigationButtons), xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == VISUAL_RADIUS)
                    {
                        radius = xmlReader.ReadElementContentAsFloat();
                    }
                }
            }
            NavigationState sourceState = set.getState(source);
            NavigationState destState = set.getState(destination);
            if (sourceState != null && destState != null)
            {
                sourceState.addAdjacentState(destState, button, radius);
            }
        }

        public static void readNavigationMenu(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            while (!isEndElement(xmlReader, NAVIGATION_MENU) && xmlReader.Read())
            {
                if(isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAVIGATION_MENU_ENTRY)
                    {
                        readNavigationMenuParentEntry(navStateSet, xmlReader);
                    }
                }
            }
        }

        public static void readNavigationMenuParentEntry(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            navStateSet.Menus.addParentEntry(readNavMenuEntryData(navStateSet, xmlReader));
        }

        private static NavigationMenuEntry readNavMenuEntryData(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            NavigationMenuEntry menuEntry = new NavigationMenuEntry("");
            while (!isEndElement(xmlReader, NAVIGATION_MENU_ENTRY) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == TEXT)
                    {
                        menuEntry.Text = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == THUMBNAIL)
                    {
                        int size = int.Parse(xmlReader.GetAttribute(BITMAP_SIZE));
                        byte[] buffer = new byte[size];
                        xmlReader.ReadElementContentAsBinHex(buffer, 0, size);
                        using (MemoryStream memStream = new MemoryStream(buffer))
                        {
                            menuEntry.Thumbnail = new Bitmap(memStream);
                        }
                    }
                    else if (xmlReader.Name == NAVIGATION_MENU_ENTRY)
                    {
                        menuEntry.addSubEntry(readNavMenuEntryData(navStateSet, xmlReader));
                    }
                    else if (xmlReader.Name == NAVIGATION_STATE)
                    {
                        NavigationState state = navStateSet.getState(xmlReader.ReadElementContentAsString());
                        if (state != null)
                        {
                            menuEntry.addNavigationState(state);
                        }
                        else
                        {
                            Log.Error("Cannot load state {0} into menu {1} because the state was not found.", xmlReader.ReadElementContentAsString(), menuEntry.Text);
                        }
                    }
                }
            }
            xmlReader.Read();
            return menuEntry;
        }

        private static bool isEndElement(XmlReader xmlReader, String elementName)
        {
            return xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.EndElement;
        }

        private static bool isValidElement(XmlReader xmlReader)
        {
            return xmlReader.NodeType == XmlNodeType.Element && !xmlReader.IsEmptyElement;
        }
    }
}
