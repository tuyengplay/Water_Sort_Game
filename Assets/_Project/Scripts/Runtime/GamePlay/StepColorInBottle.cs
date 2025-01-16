using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class StepColorInBottle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer colorShow;

        public void SetColor(ItemID _idColor)
        {
            DataColor color = DataColorManager.Instance.GetData(_idColor);
            colorShow.color = color.ColorMain;
        }
    }
}