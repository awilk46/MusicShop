using MusicShop.Models;
using MusicShop.ViewModels;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    class DataModelLoader
    {
        List<RecommendViewModel> list;
        string UserIdFld;
        string ItemIdFld;
        string PrefValFld;

        public DataModelLoader(List<RecommendViewModel> dataList, string userIdField, string itemIdField, string prefValueField = null)
        {
            list = dataList;
            UserIdFld = userIdField;
            ItemIdFld = itemIdField;
            PrefValFld = prefValueField;
        }
        public IDataModel Load()
        {
            var hasPrefVal = !String.IsNullOrEmpty(PrefValFld);

            FastByIDMap<IList<IPreference>> data = new FastByIDMap<IList<IPreference>>();

            foreach (var item in list)
            {
                if (item.UserID != null)
                { 
                long userID = Convert.ToInt64(item.UserID.GetHashCode());
            
                long itemID = Convert.ToInt64(item.AlbumID);

                var userPrefs = data.Get(userID);
                if (userPrefs == null)
                {
                    userPrefs = new List<IPreference>(3);
                    data.Put(userID, userPrefs);
                }

                if (hasPrefVal)
                {
                    var prefVal = Convert.ToSingle(PrefValFld);
                    userPrefs.Add(new GenericPreference(userID, itemID, prefVal));
                }
                else
                {
                    userPrefs.Add(new BooleanPreference(userID, itemID));
                }
                }
            }



            var newData = new FastByIDMap<IPreferenceArray>(data.Count());
            foreach (var entry in data.EntrySet())
            {
                var prefList = (List<IPreference>)entry.Value;
                newData.Put(entry.Key, hasPrefVal ?
                    (IPreferenceArray)new GenericUserPreferenceArray(prefList) :
                    (IPreferenceArray)new BooleanUserPreferenceArray(prefList));
            }
            return new GenericDataModel(newData);
        }
    }
}