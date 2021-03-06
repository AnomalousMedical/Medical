﻿using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;
using Engine.Attributes;
using Engine.Threads;

namespace Medical
{
    public class VolumeCalculator : BehaviorInterface
    {
        [Editable]
        private String nodeName = "Node";
        [Editable]
        private String entityName = "Entity";
        [Editable]
        private String name;
        [Editable]
        private String prettyName;
        [Editable]
        private String category;

        [DoNotCopy]
        [DoNotSave]
        private float initialVolume;

        Entity entity;

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find Node '{0}'", nodeName);
            }

            entity = sceneNode.getNodeObject(entityName) as Entity;

            if (entity == null)
            {
                if (entityName == null)
                {
                    blacklist("entityName is Null");
                }
                else
                {
                    blacklist("Cannot find entity named '{0}' in node '{1}'.", entityName, nodeName);
                }
            }

            if(String.IsNullOrEmpty(name))
            {
                blacklist("Name is empty");
            }

            VolumeController.addVolume(this);

            base.constructed();

            //VERY hacky and bad, figure out another way to do this
            ThreadManager.invoke(() =>
            {
                initialVolume = CurrentVolume;
            });
        }

        protected override void destroy()
        {
            VolumeController.removeVolume(this);
            base.destroy();
        }

        public float CurrentVolume
        {
            get
            {
                return entity.calculateVolume();
            }
        }

        public float InitialVolume
        {
            get
            {
                return initialVolume;
            }
        }

        public String VolumeName
        {
            get
            {
                return name;
            }
        }

        public String PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        public String Category
        {
            get
            {
                return category;
            }
        }
    }
}
