using System.Collections.Generic;
using UnityEngine;

namespace Heat___Complete_Modern_UI.Scripts.Localization
{
    [CreateAssetMenu(fileName = "New Localization Table", menuName = "Heat UI/Localization/New Localization Table")]
    public class LocalizationTable : ScriptableObject
    {
        public string tableID;
        public LocalizationSettings localizationSettings;
        public List<TableContent> tableContent = new List<TableContent>();

        [System.Serializable]
        public class TableContent
        {
            public string key;
        }
    }
}