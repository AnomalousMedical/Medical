using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Globalization;

namespace Medical.GUI
{
    class PatientBindingSource : BindingList<PatientDataFile>
    {
        private bool isSorted = false;

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            isSorted = false;
            List<PatientDataFile> items = this.Items as List<PatientDataFile>;
            if (items != null)
            {
                if (prop.Name == "FirstName")
                {
                    items.Sort(new FirstNameComparer(direction));
                    isSorted = true;
                }
                else if (prop.Name == "LastName")
                {
                    items.Sort(new LastNameComparer(direction));
                    isSorted = true;
                }
                else if (prop.Name == "DateModified")
                {
                    items.Sort(new DateModifiedComparer(direction));
                    isSorted = true;
                }
            }
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
        }

        protected override bool IsSortedCore
        {
            get
            {
                return isSorted;
            }
        }

        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                PatientDataFile data = (PatientDataFile)Items[i];
                if (data != null && data.LastName != null 
                    && data.LastName.StartsWith(key.ToString(), true, CultureInfo.CurrentCulture))
                {
                    return i;
                }
            }
            return -1;
        }

        class LastNameComparer : IComparer<PatientDataFile>
        {
            private ListSortDirection direction;

            public LastNameComparer(ListSortDirection direction)
            {
                this.direction = direction;
            }

            public int Compare(PatientDataFile x, PatientDataFile y)
            {
                if (direction == ListSortDirection.Ascending)
                {
                    return NaturalSortAlgorithm.CompareFunc(x.LastName, y.LastName);
                }
                else
                {
                    return NaturalSortAlgorithm.CompareFunc(y.LastName, x.LastName);
                }
            }
        }

        class FirstNameComparer : IComparer<PatientDataFile>
        {
            private ListSortDirection direction;

            public FirstNameComparer(ListSortDirection direction)
            {
                this.direction = direction;
            }

            public int Compare(PatientDataFile x, PatientDataFile y)
            {
                if (direction == ListSortDirection.Ascending)
                {
                    return NaturalSortAlgorithm.CompareFunc(x.FirstName, y.FirstName);
                }
                else
                {
                    return NaturalSortAlgorithm.CompareFunc(y.FirstName, x.FirstName);
                }
            }
        }

        class DateModifiedComparer : IComparer<PatientDataFile>
        {
            private ListSortDirection direction;

            public DateModifiedComparer(ListSortDirection direction)
            {
                this.direction = direction;
            }

            public int Compare(PatientDataFile x, PatientDataFile y)
            {
                if (direction == ListSortDirection.Ascending)
                {
                    if (x.DateModified.Ticks == y.DateModified.Ticks)
                    {
                        return 0;
                    }
                    if (x.DateModified.Ticks < y.DateModified.Ticks)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (y.DateModified.Ticks == x.DateModified.Ticks)
                    {
                        return 0;
                    }
                    if (y.DateModified.Ticks < x.DateModified.Ticks)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }
    }
}
