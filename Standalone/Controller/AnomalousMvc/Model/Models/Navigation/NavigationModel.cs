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

    public partial class NavigationModel : MvcModel
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

        public void reset()
        {
            if (StartIndex < links.Count)
            {
                currentIndex = StartIndex;
            }
            else if (links.Count > 0)
            {
                currentIndex = links.Count - 1;
            }
            else
            {
                currentIndex = 0;
            }
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

        public NavigationLink getNamed(String name)
        {
            for (int i = 0; i < links.Count; ++i)
            {
                if (links[i].Name == name)
                {
                    CurrentIndex = i;
                    return links[i];
                }
            }
            return null;
        }

        public NavigationLink getAt(int index)
        {
            if (index < links.Count)
            {
                CurrentIndex = index;
                return links[index];
            }
            return null;
        }

        public NavigationLink getFirst()
        {
            if (links.Count > 0)
            {
                CurrentIndex = 0;
                return links[0];
            }
            return null;
        }

        public NavigationLink getLast()
        {
            int linkCount = links.Count;
            if (linkCount > 0)
            {
                CurrentIndex = linkCount - 1;
                return links[CurrentIndex];
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

        [Editable]
        public int StartIndex { get; set; }

        public IEnumerable<NavigationLink> Links
        {
            get
            {
                return links;
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
    }

    partial class NavigationModel
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Link", addLink));

            var linkEdits = editInterface.createEditInterfaceManager<NavigationLink>();
            linkEdits.addCommand(new EditInterfaceCommand("Remove", removeLink));
            linkEdits.addCommand(new EditInterfaceCommand("Move Up", moveUp));
            linkEdits.addCommand(new EditInterfaceCommand("Move Down", moveDown));
            linkEdits.addCommand(new EditInterfaceCommand("Insert", insert));

            foreach (NavigationLink link in links)
            {
                addLinkDefinition(link);
            }
        }

        private void addLink(EditUICallback callback)
        {
            addNavigationLink(new NavigationLink());
        }

        private void removeLink(EditUICallback callback)
        {
            NavigationLink link = editInterface.resolveSourceObject<NavigationLink>(callback.getSelectedEditInterface());
            removeNavigationLink(link);
        }

        private void moveUp(EditUICallback callback)
        {
            NavigationLink link = editInterface.resolveSourceObject<NavigationLink>(callback.getSelectedEditInterface());
            int index = links.IndexOf(link) - 1;
            if (index >= 0)
            {
                removeNavigationLink(link);
                insertNavigationLink(index, link);
            }
        }

        private void moveDown(EditUICallback callback)
        {
            NavigationLink link = editInterface.resolveSourceObject<NavigationLink>(callback.getSelectedEditInterface());
            int index = links.IndexOf(link) + 1;
            if (index < links.Count)
            {
                removeNavigationLink(link);
                insertNavigationLink(index, link);
            }
        }

        private void insert(EditUICallback callback)
        {
            NavigationLink link = editInterface.resolveSourceObject<NavigationLink>(callback.getSelectedEditInterface());
            int index = links.IndexOf(link);
            if (index < links.Count)
            {
                insertNavigationLink(index, new NavigationLink());
            }
        }

        private void addLinkDefinition(NavigationLink link)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(link, link.getEditInterface());
            }
        }

        private void removeLinkDefinition(NavigationLink link)
        {
            if (editInterface != null)
            {
                editInterface.removeSubInterface(link);
            }
        }

        private void refreshLinkDefinitions()
        {
            if (editInterface != null)
            {
                editInterface.getEditInterfaceManager<NavigationLink>().clearSubInterfaces();
                foreach (NavigationLink link in links)
                {
                    link.getEditInterface().clearCommands();
                    editInterface.addSubInterface(link, link.getEditInterface());
                }
            }
        }
    }
}
