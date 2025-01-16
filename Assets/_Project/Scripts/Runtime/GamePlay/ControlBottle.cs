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

        private void Start()
        {
            if (Application.isPlaying)
            {
                group.sortingOrder = 1000;
                posRoot = transform.position;
                AddColor(ItemID.ColorBlue);
                AddColor(ItemID.ColorRed);
                fillWater = CountColor * 0.25f;
            }
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
            SetColorTop(_id);
        }

        private void MoveTarget(ControlBottle _target, int _countAdd)
        {
            Vector3 posTarget = _target.transform.position + new Vector3(0.5f, 1, 0);
            float timeMove = RhythmManager.TimeMove(transform.position, posTarget);
            transform.DOMove(posTarget, timeMove).OnComplete(
                () => { StartCoroutine(IE_PourWater(_target, _countAdd)); });
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

        private int[] angleValues = new[] { 90,80, 70, 54, 30, 0 };

        private IEnumerator IE_PourWater(ControlBottle _bottle, int _count)
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
                CalculationAngle(fillCurrent);
            }

            angleValue = angleLimit;
            bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
            CalculationAngle(fillCurrent);
            yield return IE_Waterback(timeNeedPour, angleLimit);
            yield break;
        }

        private IEnumerator IE_Waterback(float _timePour, float _angle)
        {
            float time = 0;
            float lerpValue = 0;
            float angleValue = 0;
            Vector3 posCurrent = transform.position;
            float fillCurrent = (float)CountColor * 0.25f;
            while (time < _timePour)
            {
                lerpValue = time / _timePour;
                angleValue = Mathf.Lerp(_angle, 0, lerpValue);
                transform.position = Vector3.Lerp(posCurrent, posRoot, lerpValue);
                bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                CalculationAngle(CountColor * 0.25f);
            }

            angleValue = 0;
            bottleFront.eulerAngles = new Vector3(0, 0, angleValue);
            transform.position = posRoot;
            CalculationAngle(CountColor * 0.25f);
            coll.enabled = true;
            group.sortingOrder = 1000;
            yield break;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CalculationAngle(1);
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

        private void OnValidate()
        {
            if (distanceFill <= 0)
            {
                distanceFill = top.position.y - posTop.parent.transform.position.y;
            }

            coll = gameObject.GetComponent<Collider2D>();
            if (coll == null)
            {
                coll = gameObject.AddComponent<Collider2D>();
            }

            coll.isTrigger = true;
            group = GetComponentInChildren<SortingGroup>();
            group.sortingOrder = 1000;
        }

        private void CalculationAngle(float _limit)
        {
            angle = Mathf.Abs(bottleFront.transform.localEulerAngles.z > 180 ? 360 - bottleFront.transform.localEulerAngles.z : bottleFront.transform.localEulerAngles.z);
            if (Application.isPlaying)
            {
                float a = fillAngle.Evaluate(angle);

                if (a <= _limit)
                {
                    fillWater = fillAngle.Evaluate(angle);
                }
            }
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

            Transform currentScale = posColor[index].transform;
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