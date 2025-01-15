using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.Serialization;

namespace WaterSort
{
    [ExecuteAlways]
    public class ControlBottle : MonoBehaviour
    {
        #region ControlFill

        [Header("Fill"), Range(0f, 1f), SerializeField]
        private float fillWater;

        [SerializeField] private AnimationCurve fillAngle;
        private float angle;
        [SerializeField] private Transform[] posColor;
        [SerializeField] private Transform posTop;
        [SerializeField] private Transform top;
        [SerializeField] private float distanceFill = 0;
        [SerializeField] private float maskIgnore;

        [SerializeField] private AnimationCurve curveScaleFill;

        [SerializeField] private AnimationCurve curvePos;
        [SerializeField] private Transform bottleFront;
        [SerializeField] private AnimationCurve curveScaleX;
        [SerializeField] private AnimationCurve curveScaleY;
        [SerializeField] private AnimationCurve curveScaleAllNode;
        [SerializeField] private AnimationCurve curveMovePosTop;

        [Header("Game Object")] [SerializeField]
        private GameObject gStatic;

        [SerializeField] private GameObject gPour;
        [SerializeField] private GameObject gFill;

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CalculationAngle();
            }
#endif
        }

        private void OnValidate()
        {
            if (distanceFill <= 0)
            {
                distanceFill = top.position.y - posTop.parent.transform.position.y;
            }
        }

        private void Start()
        {
            CalculationAngle();
        }

        private void CalculationAngle()
        {
            angle = Mathf.Abs(bottleFront.transform.localEulerAngles.z > 180 ? 360 - bottleFront.transform.localEulerAngles.z : bottleFront.transform.localEulerAngles.z);
            fillWater = fillAngle.Evaluate(angle);
            CalculationFill();
        }

        private void CalculationFill()
        {
            int total = posColor.Length;
            float percent = 1f / (float)total;
            float a = 0 * percent;
            int index = 0;
            Vector3 colorTemp = Vector3.one;
            for (int i = total - 1; i >= 0; i--)
            {
                posColor[i].gameObject.SetActive(true);
                colorTemp = posColor[i].transform.localScale;
                colorTemp.x = curveScaleAllNode.Evaluate(angle);
                posColor[i].transform.localScale = colorTemp;
                if (i * percent < fillWater && (i + 1) * percent >= fillWater)
                {
                    index = i;
                }
                else if (i * percent > fillWater)
                {
                    posColor[i].gameObject.SetActive(false);
                }

                int tempIndex = index - 1;
                if (tempIndex >= 0)
                {
                    Vector3 scaleCurrent = posColor[tempIndex].transform.localScale;
                    scaleCurrent.y = 1;
                    posColor[tempIndex].transform.localScale = scaleCurrent;
                }
            }

            Transform currentScale = posColor[index];
            float remain = fillWater - (index * percent);
            float percentInNote = remain / percent;
            float scale = maskIgnore + (1 - maskIgnore) * percentInNote;
            Vector3 scaleV3 = currentScale.transform.localScale;
            scaleV3.y = scale;
            currentScale.transform.localScale = scaleV3;
            Vector3 posCurrent = posTop.localPosition;
            if (bottleFront.transform.localEulerAngles.z >= 180)
            {
                posCurrent.x = -1 * curveMovePosTop.Evaluate(angle);
            }
            else
            {
                posCurrent.x = curveMovePosTop.Evaluate(angle);
            }

            posCurrent.y = distanceFill * curvePos.Evaluate(fillWater);
            posTop.localPosition = posCurrent;
            Vector3 scaleTop = posTop.localScale;
            if (angle == 0)
            {
                scaleTop.y = curveScaleFill.Evaluate(fillWater);
                gStatic.SetActive(true);
                gPour.SetActive(false);
            }
            else
            {
                scaleTop.y = curveScaleY.Evaluate(angle);
                gStatic.SetActive(false);
                gPour.SetActive(true);
            }

            scaleTop.x = curveScaleX.Evaluate(angle);
            posTop.localScale = scaleTop;
            //Scale Mat Nuoc
        }

        #endregion
    }
}