using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;

namespace Medical
{
    public abstract partial class DDPluginTask : Task, Saveable
    {
        private DDAtlasPlugin plugin;

        public DDPluginTask(String uniqueName, String name, String iconName, String category)
            : base(uniqueName, name, iconName, category)
        {
            this.Name = "";
            TaskUniqueName = uniqueName;
        }

        public DDAtlasPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        [Editable]
        public String TaskName
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        [Editable]
        public String TaskIcon
        {
            get
            {
                return IconName;
            }
            set
            {
                IconName = value;
            }
        }

        [Editable]
        public String TaskCategory
        {
            get
            {
                return Category;
            }
            set
            {
                Category = value;
            }
        }

        [Editable]
        public String TaskUniqueName { get; set; }

        /// <summary>
        /// Set the plugin for this task. DO NOT TOUCH if you are not DDAtlasPlugin.
        /// </summary>
        /// <param name="plugin">The DDAtlasPlugin that loaded this task.</param>
        internal void _setPlugin(DDAtlasPlugin plugin)
        {
            this.plugin = plugin;
            UniqueName = String.Format("DDPlugin.{0}.{1}", plugin.PluginNamespace, TaskUniqueName);
        }

        protected DDPluginTask(LoadInfo info)
            : base(info.GetString("UniqueName"),
            info.GetString("Name"),
            info.GetString("IconName"),
            info.GetString("Category"))
        {
            TaskUniqueName = info.GetString("TaskUniqueName", UniqueName);
            ShowOnTaskbar = info.GetBoolean("ShowOnTaskbar", ShowOnTaskbar);
            ShowOnTimelineTaskbar = info.GetBoolean("ShowOnTimelineTaskbar", ShowOnTimelineTaskbar);
            ShowOnTaskMenu = info.GetBoolean("ShowOnTaskMenu", ShowOnTaskMenu);
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue("UniqueName", UniqueName);
            info.AddValue("Name", Name);
            info.AddValue("IconName", IconName);
            info.AddValue("Category", Category);
            info.AddValue("TaskUniqueName", TaskUniqueName);
            info.AddValue("ShowOnTaskbar", ShowOnTaskbar);
            info.AddValue("ShowOnTimelineTaskbar", ShowOnTimelineTaskbar);
            info.AddValue("ShowOnTaskMenu", ShowOnTaskMenu);
        }
    }

    partial class DDPluginTask
    {
        private static DDPluginScanner scanner = new DDPluginScanner();

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, scanner, String.Format("{0} - {1}", UniqueName, GetType().Name), null);
                }
                return editInterface;
            }
        }

        /// <summary>
        /// A scanner for DDPluginTasks, needed because the base class isn't marked up as editable, but we want to be able
        /// to include these anyway.
        /// </summary>
        class DDPluginScanner : MemberScanner
        {
            public IEnumerable<MemberWrapper> getMatchingMembers(Type type)
            {
                foreach(var wrapper in ReflectedEditInterface.DefaultScanner.getMatchingMembers(type))
                {
                    yield return wrapper;
                }

                yield return new PropertyMemberWrapper(type.GetProperty("ShowOnTaskbar"));
                yield return new PropertyMemberWrapper(type.GetProperty("ShowOnTimelineTaskbar"));
                yield return new PropertyMemberWrapper(type.GetProperty("ShowOnTaskMenu"));
                yield return new PropertyMemberWrapper(type.GetProperty("Dragable"));
            }
        }
    }
}
