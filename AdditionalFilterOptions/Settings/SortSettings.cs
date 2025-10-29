using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdditionalFilterOptions.Patches;

namespace AdditionalFilterOptions.Settings
{
    internal class SortSettings
    {
        public SortType PrimarySort
        {
            get
            {
                if (Sorts.Count > 0)
                {
                    return Sorts[0];
                }
                else
                {
                    return SortType.Order;
                }
            }
        }
        public List<SortType> Sorts { get; set; } = new List<SortType>();
        public SortSettings()
        {
            Sorts = new List<SortType>();
        }

        public void ResetValues()
        {
            Sorts.Clear();
        }
    }
}
