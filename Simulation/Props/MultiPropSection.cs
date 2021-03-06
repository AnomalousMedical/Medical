﻿using BulletPlugin;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class MultiPropSection
    {
        private SceneNode node;
        private Entity entity;
        private String name;
        private String collision;
        private String mesh;
        private ReshapeableRigidBodySection rigidBodySection;

        public MultiPropSection(String name, String mesh, String collision, Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            this.name = name;
            this.Translation = translation;
            this.Rotation = rotation;
            this.Scale = scale;
            this.collision = collision;
            this.mesh = mesh;            
        }

        internal void create(MultiProp multiProp)
        {
            node = multiProp.OgreSceneManager.SceneManager.createSceneNode(String.Format("{0}_MultiPropNode_{1}", multiProp.Owner.Name, name));
            node.setPosition(Translation);
            node.setOrientation(Rotation);
            node.setScale(Scale);
            multiProp.MainNode.addChild(node);

            entity = multiProp.OgreSceneManager.SceneManager.createEntity(String.Format("{0}_MultiPropEntity_{1}", multiProp.Owner.Name, name), mesh);
            node.attachObject(entity);

            rigidBodySection = multiProp.RigidBody.createSection(collision, Translation, Rotation, Scale);

            if (rigidBodySection == null)
            {
                Logging.Log.Error("Cannot find collision shape '{0}'", collision);
            }
        }

        internal void destroy(MultiProp multiProp)
        {
            entity.detachFromParent();
            multiProp.OgreSceneManager.SceneManager.destroyEntity(entity);
            multiProp.MainNode.removeChild(node);
            multiProp.OgreSceneManager.SceneManager.destroySceneNode(node);
            multiProp.RigidBody.destroySection(rigidBodySection);
        }

        public void updatePosition()
        {
            node.setPosition(Translation);
            node.setOrientation(Rotation);
            node.setScale(Scale);

            rigidBodySection.moveOrigin(Translation, Rotation);
            rigidBodySection.setScaling(Scale);
        }

        public Vector3 Translation { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
