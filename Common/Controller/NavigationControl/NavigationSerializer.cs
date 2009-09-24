using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Engine;

namespace Medical
{
    public class NavigationSerializer
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

        public static void writeNavigationStateSet(NavigationStateSet set, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(NAVIGATION_STATE_SET);
            foreach(NavigationState state in set.States)
            {
                xmlWriter.WriteStartElement(NAVIGATION_STATE);
                xmlWriter.WriteElementString(NAME, state.Name);
                xmlWriter.WriteElementString(TRANSLATION, state.Translation.ToString());
                xmlWriter.WriteElementString(LOOK_AT, state.LookAt.ToString());
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
                }
            }
            return set;
        }

        private static void readNavigationState(NavigationStateSet set, XmlReader xmlReader)
        {
            String name = null;
            Vector3 position = Vector3.UnitZ;
            Vector3 lookAt = Vector3.Zero;
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
                }
            }
            if (name != null)
            {
                set.addState(new NavigationState(name, lookAt, position));
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
