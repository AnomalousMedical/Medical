using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Models
{
    public class UserStoresModel : Saveable
    {
        private List<StoreModel> stores = new List<StoreModel>();

        public UserStoresModel()
        {

        }

        public UserStoresModel(IEnumerable<StoreModel> stores)
        {
            this.stores.AddRange(stores);
        }

        public IEnumerable<StoreModel> Stores
        {
            get
            {
                return stores;
            }
        }

        protected UserStoresModel(LoadInfo info)
        {
            info.RebuildList("Store", stores);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList("Store", stores);
        }
    }
}
