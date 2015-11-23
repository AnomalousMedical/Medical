using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Anomalous.GuiFramework
{
    public static class TaskMenuCategories
    {
        const int DynamicGroupBase = 10;

        public const String Explore = "Explore";
        public const String Patient = "Patient";
        public const String System = "System";
        public const String Create = "Create";
        public const String Developer = "Developer";

        class MedicalTaskSorter : TaskMenuGroupSorter
        {
            private Dictionary<String, int> groupOrder = new Dictionary<string, int>();
            private List<String> externalGroups = new List<string>();
            private String highlightGroup;

            public MedicalTaskSorter()
            {
                groupOrder.Add(TaskMenu.SearchResultsCategory, 0);
                //1 is the highlight group
                groupOrder.Add(Explore, 2);
                groupOrder.Add(Create, 3);
                groupOrder.Add(Patient, 4);
                groupOrder.Add(Developer, int.MaxValue - 1);
                groupOrder.Add(System, int.MaxValue);
            }

            public int compareGroups(string x, string y)
            {
                //Treat not found items as loweset value

                int xVal;
                int yVal;
                if(!groupOrder.TryGetValue(x, out xVal))
                {
                    xVal = 1;
                }
                if(!groupOrder.TryGetValue(y, out yVal))
                {
                    yVal = 1;
                }

                return xVal - yVal;
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
