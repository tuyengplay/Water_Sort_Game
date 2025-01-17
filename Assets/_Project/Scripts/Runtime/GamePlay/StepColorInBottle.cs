using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class StepColorInBottle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer colorShow;
        [SerializeField] private AnimationCurve curvePos;

        public void SetColor(ItemID _idColor)
        {
            DataColor color = DataColorManager.Instance.GetData(_idColor);
            colorShow.color = color.ColorMain;
        }

        public void SetAngle(float _angle)
        {
            Vector3 pos = transform.localPosition;
            pos.y = curvePos.Evaluate(_angle);
            transform.localPosition = pos;
        }
    }
}