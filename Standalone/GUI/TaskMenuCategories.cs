using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Anomalous.GuiFramework
{
    public static class TaskMenuCategories
    {
        const int DynamicGroupBase = 3;

        public const String Explore = "Explore";
        public const String Patient = "Patient";
        public const String System = "System";
        public const String Create = "Create";
        public const String Developer = "Developer";

        class MedicalTaskSorter : TaskMenuGroupSorter
        {
            private Dictionary<String, int> groupOrder = new Dictionary<string, int>();
            private List<String> externalGroups = new List<string>();

            public MedicalTaskSorter()
            {
                groupOrder.Add(TaskMenu.SearchResultsCategory, 0);
                groupOrder.Add(Explore, 1);
                groupOrder.Add(Create, 2);
                groupOrder.Add(Patient, 3);
                groupOrder.Add(Developer, int.MaxValue - 1);
                groupOrder.Add(System, int.MaxValue);
            }

            public int compareGroups(string x, string y)
            {
                return groupOrder[x] - groupOrder[y];
            }

            public void groupAdded(string name)
            {
                if(!groupOrder.ContainsKey(name))
                {
                    //Remove all dynamic groups
                    foreach(var group in externalGroups)
                    {
                        groupOrder.Remove(group);
                    }

                    //Add new group and sort
                    externalGroups.Add(name);
                    externalGroups.Sort(NaturalSortAlgorithm.CompareFunc);
                    
                    //Readd all dynamic groups
                    int i = DynamicGroupBase;
                    foreach (var group in externalGroups)
                    {
                        groupOrder.Add(group, i++);
                    }
                }
            }
        }

        private static TaskMenuGroupSorter sorter = new MedicalTaskSorter();

        public static TaskMenuGroupSorter Sorter
        {
            get
            {
                return sorter;
            }
        }
    }
}
