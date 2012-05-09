using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    partial class NavigationModel : Saveable
    {
        private List<NavigationLink> links = new List<NavigationLink>();
        private int currentIndex = 0;

        public NavigationModel()
        {
            Name = "DefaultNavigation";
        }

        public void addNavigationLink(NavigationLink link)
        {
            links.Add(link);
            addLinkDefinition(link);
        }

        public void removeNavigationLink(NavigationLink link)
        {
            links.Remove(link);
            removeLinkDefinition(link);
        }

        public void insertNavigationLink(int index, NavigationLink item)
        {
            links.Insert(index, item);
            refreshLinkDefinitions();
        }

        public void resetIndex()
        {
            currentIndex = 0;
        }

        public NavigationLink getNext()
        {
            if (++currentIndex < links.Count)
            {
                return links[currentIndex];
            }
            --currentIndex;
            return null;
        }

        public NavigationLink getPrevious()
        {
            if (--currentIndex > -1 && links.Count > 0)
            {
                return links[currentIndex];
            }
            ++currentIndex;
            return null;
        }

        public NavigationLink getCurrent()
        {
            if (links.Count > 0)
            {
                return links[currentIndex];
            }
            return null;
        }

        public bool HasNext
        {
            get
            {
                return currentIndex + 1 < links.Count;
            }
        }

        public bool HasPrevious
        {
            get
            {
                return currentIndex - 1 > -1;
            }
        }

        [Editable]
        public String Name { get; set; }

        protected NavigationModel(LoadInfo info)
        {
            Name = info.GetString("Name");
            info.RebuildList<NavigationLink>("Link", links);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.ExtractList<NavigationLink>("Link", links);
        }
    }

    partial class NavigationModel
    {
        private EditInterface editInterface;
        private EditInterfaceManager<NavigationLink> linkEdits;

        public EditInterface getEditInterface(String name)
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name, null);
                editInterface.addCommand(new EditInterfaceCommand("Add Link", addLink));

                linkEdits = new EditInterfaceManager<NavigationLink>(editInterface);
                linkEdits.addCommand(new EditInterfaceCommand("Remove", removeLink));
                linkEdits.addCommand(new EditInterfaceCommand("Move Up", moveUp));
                linkEdits.addCommand(new EditInterfaceCommand("Move Down", moveDown));

                foreach (NavigationLink link in links)
                {
                    addLinkDefinition(link);
                }
            }
            return editInterface;
        }

        private void addLink(EditUICallback callback, EditInterfaceCommand command)
        {
            addNavigationLink(new NavigationLink());
        }

        private void removeLink(EditUICallback callback, EditInterfaceCommand command)
        {
            NavigationLink link = linkEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeNavigationLink(link);
        }

        private void moveUp(EditUICallback callback, EditInterfaceCommand command)
        {
            NavigationLink link = linkEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = links.IndexOf(link) - 1;
            if (index >= 0)
            {
                removeNavigationLink(link);
                insertNavigationLink(index, link);
            }
        }

        private void moveDown(EditUICallback callback, EditInterfaceCommand command)
        {
            NavigationLink link = linkEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = links.IndexOf(link) + 1;
            if (index < links.Count)
            {
                removeNavigationLink(link);
                insertNavigationLink(index, link);
            }
        }

        private void addLinkDefinition(NavigationLink link)
        {
            if (editInterface != null)
            {
                linkEdits.addSubInterface(link, link.getEditInterface());
            }
        }

        private void removeLinkDefinition(NavigationLink link)
        {
            if (editInterface != null)
            {
                linkEdits.removeSubInterface(link);
            }
        }

        private void refreshLinkDefinitions()
        {
            if (editInterface != null)
            {
                linkEdits.clearSubInterfaces();
                foreach (NavigationLink link in links)
                {
                    link.getEditInterface().clearCommands();
                    linkEdits.addSubInterface(link, link.getEditInterface());
                }
            }
        }
    }
}
