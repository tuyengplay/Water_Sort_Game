using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace WaterSort
{
    [ExecuteAlways]
    public class ControlBottle : MonoBehaviour
    {
        #region Logic

        [SerializeField] private SortingGroup group;
        [SerializeField] private Collider2D coll;
        private Stack<ItemID> colorInBottle = new Stack<ItemID>();
        private Vector3 posRoot;

        public void SetData(DataSpawn _data)
        {
            group.sortingOrder = 1000;
            posRoot = transform.position;
            foreach (ItemID temp in _data.dataInBottle)
            {
                AddColor(temp);
            }

            fillWater = CountColor * 0.25f;
        }

        public ControlBottle Target
        {
            set
            {
                if (value != null)
                {
                    int countAdd = 0;
                    ItemID top = ColorTop;
                    for (int i = 0; i < colorInBottle.Count; i++)
                    {
                        if (top == colorInBottle.Peek() && value.CountCanReceive > 0)
                        {
                            value.AddColor(colorInBottle.Pop());
                            countAdd++;
                            i--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    MoveTarget(value, 4 - CountColor);
                }
            }
        }

        public ItemID ColorTop
        {
            get
            {
                if (CountColor > 0)
                {
                    return colorInBottle.Peek();
                }
                else
                {
                    return ItemID.None;
                }
            }
        }

        private int CountColor
        {
            get => colorInBottle.Count;
        }

        public int CountCanReceive
        {
            get => 4 - CountColor;
        }

        private void AddColor(ItemID _id)
        {
            posColor[CountColor].SetColor(_id);
            colorInBottle.Push(_id);
            fillWater = CountColor * RhythmManager.Percent;
            CalculationFill(false);
            SetColorTop(_id);
        }

        private void MoveTarget(ControlBottle _target, int _countAdd)
        {
            Vector3 posTarget = _target.transform.position + new Vector3(0.5f, 1, 0);
            float timeMove = RhythmManager.TimeMove(transform.position, posTarget);
            transform.DOMove(posTarget, timeMove).OnComplete(
                () => { StartCoroutine(IE_PourWater(_countAdd)); });
        }

        public void OnSelect()
        {
            coll.enabled = false;
            group.sortingOrder = 1500;
            Vector3 pos = transform.localPosition;
            pos.y += 0.5f;
            float timeMove = RhythmManager.TimeMove(transform.position, pos);
            transform.DOLocalMoveY(pos.y, timeMove);
        }

        public void OnNoSelect()
        {
            float timeMove = RhythmManager.TimeMove(transform.position, posRoot);
            transform.DOMove(posRoot, timeMove).OnComplete(() =>
            {
                coll.enabled = true;
                group.sortingOrder = 1000;
            });
        }

        public ItemID CanReceive
        {
            get
            {
                if (CountColor >= 4)
                {
                    return ItemID.None;
                }
                else if (CountColor <= 0)
                {
                    return ItemID.ColorEnd;
                }
                else
                {
                    return colorInBottle.Peek();
                }
            }
        }

        #endregion

        #region Pour Nuoc

        private int[] angleValues = new[] { 90, 80, 70, 54, 30, 0 };

        private IEnumerator IE_PourWater(int _count)
        {
            float time = 0;
            float lerpValue = 0;
            float angleValue = 0;
            float timeNeedPour = RhythmManager.TimePour(_count);
            int angleLimit = angleValues[CountColor];
            float fillCurrent = fillWater;
            while (time < timeNeedPour)
            {
                lerpValue = time / timeNeedPour;
                angleValue = Mathf.Lerp(0, angleLimit, lerpValue);
                bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                CalculationAngle(fillCurrent, false);
            }

            if (CountColor > 0)
            {
                SetColorTop(colorInBottle.Peek());
            }

            angleValue = angleLimit;
            bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
            CalculationAngle(fillCurrent, false);
            yield return IE_Waterback(timeNeedPour, angleLimit);
            yield break;
        }

        private IEnumerator IE_Waterback(float _timePour, float _angle)
        {
            float time = 0;
            float lerpValue = 0;
            float angleValue = 0;
            Vector3 posCurrent = transform.position;
            while (time < _timePour)
            {
                lerpValue = time / _timePour;
                angleValue = Mathf.Lerp(_angle, 0, lerpValue);
                transform.position = Vector3.Lerp(posCurrent, posRoot, lerpValue);
                bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                CalculationAngle(CountColor * 0.25f, true);
            }

            angleValue = 0;
            bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
            transform.position = posRoot;
            CalculationAngle(CountColor * 0.25f, true);
            coll.enabled = true;
            group.sortingOrder = 1000;
            yield break;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CalculationAngle(1, true);
            }
#endif
        }

        #endregion

        #region ControlFill

        [Header("Fill"), Range(0f, 1f), SerializeField]
        private float fillWater;

        [SerializeField] private AnimationCurve fillAngle;
        private float angle;
        [SerializeField, ReadOnly] private StepColorInBottle[] posColor;
        [SerializeField] private Transform posTop;
        [SerializeField] private float maskIgnore;

        [SerializeField] private AnimationCurve curveScaleFill;

        [SerializeField] private AnimationCurve curvePosY;
        [SerializeField] private AnimationCurve curvePosYAngle025;
        [SerializeField] private AnimationCurve curvePosYAngle05;
        [SerializeField] private AnimationCurve curvePosYAngle075;
        [SerializeField] private AnimationCurve curvePosYAngle1;
        [SerializeField] private Transform bottleFront;
        [SerializeField] private AnimationCurve curveScaleX;
        [SerializeField] private AnimationCurve curveScaleY;
        [SerializeField] private AnimationCurve curveScaleAllNode;
        [SerializeField] private AnimationCurve curveMovePosTop;

        private AnimationCurve curvePosCurrent;
        private bool isFillOrAngle;

        public AnimationCurve CurvePosCurrent
        {
            get => curvePosCurrent;
            set
            {
                if (value != curvePosCurrent)
                {
                    curvePosCurrent = value;
                }
            }
        }

        [Header("Game Object")] [SerializeField]
        private GameObject gStatic;

        [SerializeField] private GameObject gPour;
        [SerializeField] private GameObject gFill;

        private void OnValidate()
        {
            coll = gameObject.GetComponent<Collider2D>();
            if (coll == null)
            {
                coll = gameObject.AddComponent<Collider2D>();
            }

            coll.isTrigger = true;
            group = GetComponentInChildren<SortingGroup>();
            group.sortingOrder = 1000;
        }

        private void CalculationAngle(float _limit, bool _isBack)
        {
            angle = Mathf.Abs(bottleFront.transform.localEulerAngles.z > 180 ? 360 - bottleFront.transform.localEulerAngles.z : bottleFront.transform.localEulerAngles.z);
            if (Application.isPlaying)
            {
                float a = fillAngle.Evaluate(angle);

                if (a <= _limit)
                {
                    fillWater = a;
                }
                else
                {
                    fillWater = _limit;
                }
            }

            foreach (StepColorInBottle temp in posColor)
            {
                temp.SetAngle(angle);
            }

            CalculationFill(_isBack);
        }

        private void CalculationFill(bool _isBack)
        {
            int total = posColor.Length;
            int index = 0;
            Vector3 colorTemp = Vector3.one;
            Vector3 tempScaleCurrent;
            for (int i = total - 1; i >= 0; i--)
            {
                posColor[i].gameObject.SetActive(true);
                colorTemp = posColor[i].transform.localScale;
                colorTemp.x = curveScaleAllNode.Evaluate(angle);
                posColor[i].transform.localScale = colorTemp;
                if ((i + 1) * RhythmManager.Percent == 1 && fillWater == 1)
                {
                    index = total - 1;
                }
                else if (i * RhythmManager.Percent < fillWater && (i + 1) * RhythmManager.Percent >= fillWater)
                {
                    index = i;
                }
                else if (i * RhythmManager.Percent >= fillWater)
                {
                    posColor[i].gameObject.SetActive(false);
                }
                else
                {
                    tempScaleCurrent = posColor[i].transform.localScale;
                    tempScaleCurrent.y = 1;
                    posColor[i].transform.localScale = tempScaleCurrent;
                }

                int tempIndex = index - 1;
                if (tempIndex >= 0)
                {
                    Vector3 scaleCurrent = posColor[tempIndex].transform.localScale;
                    scaleCurrent.y = 1;
                    posColor[tempIndex].transform.localScale = scaleCurrent;
                }
            }

            Transform currentScale = posColor[index].transform;
            float remain = fillWater - (index * RhythmManager.Percent);
            float percentInNote = remain / RhythmManager.Percent;
            float maskIgnoreTemp = 1;
            if (angle == 0)
            {
                maskIgnoreTemp = maskIgnore;
            }
            else
            {
                switch (index)
                {
                    case 0:
                        maskIgnoreTemp = maskIgnore;
                        break;
                    case 1:
                        maskIgnoreTemp = 0.9f;
                        break;
                    case 2:
                        maskIgnoreTemp = 0.83f;
                        break;
                    case 3:
                        maskIgnoreTemp = 0.85f;
                        break;
                    default:
                        maskIgnoreTemp = maskIgnore;
                        break;
                }
            }

            float scale = maskIgnoreTemp + (1 - maskIgnoreTemp) * percentInNote;
            Vector3 scaleV3 = currentScale.transform.localScale;
            scaleV3.y = scale;
            currentScale.transform.localScale = scaleV3;
            Vector3 posCurrent = posTop.localPosition;
            posTop.gameObject.SetActive(fillWater != 0);
            if (bottleFront.transform.localEulerAngles.z > 180)
            {
                posCurrent.x = -1 * curveMovePosTop.Evaluate(angle);
                float fillWaterTemp = Mathf.Ceil(fillWater * 100) / 100;
                switch (fillWaterTemp)
                {
                    case 0.25f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle025;
                        break;
                    case 0.5f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle05;
                        break;
                    case 0.75f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle075;
                        break;
                    case 1f:
                        isFillOrAngle = true;
                        CurvePosCurrent = curvePosYAngle1;
                        break;
                    case 0f:
                        isFillOrAngle = true;
                        CurvePosCurrent = curvePosYAngle1;
                        break;
                }
            }
            else if (bottleFront.transform.localEulerAngles.z > 0)
            {
                posCurrent.x = curveMovePosTop.Evaluate(angle);
                float fillWaterTemp = Mathf.Ceil(fillWater * 100) / 100;
                switch (fillWaterTemp)
                {
                    case 0.25f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle025;
                        break;
                    case 0.5f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle05;
                        break;
                    case 0.75f:
                        isFillOrAngle = false;
                        CurvePosCurrent = curvePosYAngle075;
                        break;
                    case 1f:
                        isFillOrAngle = true;
                        CurvePosCurrent = curvePosYAngle1;
                        break;
                    case 0f:
                        isFillOrAngle = true;
                        CurvePosCurrent = curvePosYAngle1;
                        break;
                }
            }
            else
            {
                posCurrent.x = 0;
                isFillOrAngle = true;
                CurvePosCurrent = curvePosY;
            }

            if (isFillOrAngle)
            {
                posCurrent.y = CurvePosCurrent.Evaluate(fillWater);
            }
            else
            {
                posCurrent.y = CurvePosCurrent.Evaluate(angle);
            }

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

        #region Control_Color

        private void SetColorTop(ItemID _id)
        {
            DataColor color = DataColorManager.Instance.GetData(_id);
            colorStatic.color = color.ColorShadow;

            colorPour.material.SetColor("_Color1", color.ColorMain);
            colorPour.material.SetColor("_Color2", color.ColorShadow);

            colorFill.material.SetColor("_Color1", color.ColorMain);
            colorFill.material.SetColor("_Color2", color.ColorShadow);
        }

        [SerializeField] private SpriteRenderer colorStatic;
        [SerializeField] private SpriteRenderer colorPour;
        [SerializeField] private SpriteRenderer colorFill;

        #endregion
    }
}