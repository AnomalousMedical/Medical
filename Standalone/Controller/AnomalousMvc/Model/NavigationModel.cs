﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    public delegate void NavigationModelEvent(NavigationModel navModel);

    public partial class NavigationModel : Model
    {
        public const String DefaultName = "DefaultNavigation";

        [DoNotSave]
        private List<NavigationLink> links = new List<NavigationLink>();

        [DoNotSave]
        private int currentIndex = 0;

        [DoNotSave]
        public event NavigationModelEvent CurrentIndexChanged;

        public NavigationModel(String name)
            :base(name)
        {
            
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
            if (HasNext)
            {
                return links[++CurrentIndex];
            }
            return null;
        }

        public NavigationLink getPrevious()
        {
            if (HasPrevious)
            {
                return links[--CurrentIndex];
            }
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
                return currentIndex - 1 > -1 && links.Count > 0;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = value;
                if (CurrentIndexChanged != null)
                {
                    CurrentIndexChanged.Invoke(this);
                }
            }
        }

        protected NavigationModel(LoadInfo info)
            :base(info)
        {
            info.RebuildList<NavigationLink>("Link", links);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<NavigationLink>("Link", links);
        }

        public IEnumerable<NavigationLink> Links
        {
            get
            {
                return links;
            }
        }
    }

    partial class NavigationModel
    {
        private EditInterfaceManager<NavigationLink> linkEdits;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
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
            if (linkEdits != null)
            {
                linkEdits.addSubInterface(link, link.getEditInterface());
            }
        }

        private void removeLinkDefinition(NavigationLink link)
        {
            if (linkEdits != null)
            {
                linkEdits.removeSubInterface(link);
            }
        }

        private void refreshLinkDefinitions()
        {
            if (linkEdits != null)
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
