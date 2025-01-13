using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace WaterSort
{
    public class ControlBottle : MonoBehaviour
    {
        #region ControlFill

        [Header("Fill")] [SerializeField] private float fillWater;
        [SerializeField] private Transform[] posColor;
        [SerializeField] private Transform posScale;
        [SerializeField] private Transform top;
        [SerializeField]private float distanceFill;
        [SerializeField] private float maskIgnore;

        public float FillWater
        {
            get => fillWater;
            set
            {
                fillWater = value;
                CalculationFill();
            }
        }

        private void OnValidate()
        {
            distanceFill = top.position.y - posScale.parent.transform.position.y;
        }

        private void Start()
        {
            CalculationFill();
                //DOVirtual.DelayedCall(2, () => { DOVirtual.Float(1, 0, 10, (_value) => { FillWater = _value; }); });
        }


        private void CalculationFill()
        {
            int total = posColor.Length;
            float percent = 1f / (float)total;
            float a = 0 * percent;
            int index = 0;
            for (int i = total - 1; i >= 0; i--)
            {
                posColor[i].gameObject.SetActive(true);
                if (i * percent <= fillWater && (i + 1) * percent > fillWater)
                {
                    index = i;
                }
                else if (i * percent > fillWater)
                {
                    posColor[i].gameObject.SetActive(false);
                }
                else
                {
                    posColor[i].transform.localScale = Vector3.one;
                }
            }

            Transform currentScale = posColor[index];
            float remain = fillWater - (index * percent);
            float percentInNote = remain / percent;
            float scale = maskIgnore + (1 - maskIgnore) * percentInNote;
            Vector3 scaleV3 = currentScale.transform.localScale;
            scaleV3.y = scale;
            currentScale.transform.localScale = scaleV3;
            Vector3 posCurrent = posScale.localPosition;
            posCurrent.y = fillWater * distanceFill;
            posScale.localPosition = posCurrent;
        }

        #endregion
    }
}