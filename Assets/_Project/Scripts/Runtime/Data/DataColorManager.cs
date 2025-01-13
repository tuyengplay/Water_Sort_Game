using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WaterSort
{
    [CreateAssetMenu(fileName = "DataColorManager", menuName = "Data/DataColorManager", order = 1)]
    public class DataColorManager : ScriptableSingleton<DataColorManager>
    {
        public static DataColorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<DataColorManager>("Managers/DataColorManager");
                }

                return instance;
            }
        }
        
        [SerializeField, TableList, Searchable]
        private DataColor[] colors;

        public DataColor GetData(ItemID _id)
        {
            foreach (DataColor temp in colors)
            {
                if (temp.ID == _id)
                {
                    return temp;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DataColor
    {
        [SerializeField] private ItemID idColor;
        public ItemID ID => idColor;
        [SerializeField] private Color colorMain;
        public Color ColorMain => colorMain;
        [SerializeField] private Color colorShadow;
        public Color ColorShadow => colorShadow;
    }
}