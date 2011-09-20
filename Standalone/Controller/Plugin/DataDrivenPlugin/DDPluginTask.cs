using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public abstract partial class DDPluginTask : Task, Saveable
    {
        private DDAtlasPlugin plugin;

        public DDPluginTask(String uniqueName, String name, String iconName, String category)
            : base(uniqueName, name, iconName, category)
        {
            
        }

        public DDAtlasPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        /// <summary>
        /// Set the plugin for this task. DO NOT TOUCH if you are not DDAtlasPlugin.
        /// </summary>
        /// <param name="plugin">The DDAtlasPlugin that loaded this task.</param>
        internal void _setPlugin(DDAtlasPlugin plugin)
        {
            this.plugin = plugin;
        }

        protected DDPluginTask(LoadInfo info)
            : base(info.GetString("UniqueName"),
            info.GetString("Name"),
            info.GetString("IconName"),
            info.GetString("Category"))
        {

        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue("UniqueName", UniqueName);
            info.AddValue("Name", Name);
            info.AddValue("IconName", IconName);
            info.AddValue("Category", Category);
        }
    }

    partial class DDPluginTask
    {
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, String.Format("{0} - {1}", UniqueName, GetType().Name), null);
                }
                return editInterface;
            }
        }
    }
}
