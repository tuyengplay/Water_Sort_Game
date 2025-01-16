using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSort;

namespace WaterSort
{
    public class GameManager : Singleton<GameManager>
    {
        private int layerBottle;
        [SerializeField] private Camera cam;
        private ControlBottle bottleCurrent;
        [SerializeField] private DataSpawn[] dataTemp;

        public ControlBottle BottleCurrent
        {
            get => bottleCurrent;
            set
            {
                bottleCurrent = value;
                if (bottleCurrent != null)
                {
                    bottleCurrent.OnSelect();
                }
            }
        }
        private void OnNoFirst()
        {
            RaycastHit2D hit = Physics2D.Raycast(posWorld, Vector2.zero, 10, layerBottle);
            if (hit.collider != null)
            {
                ControlBottle temp = hit.collider.GetComponent<ControlBottle>();
                if (temp != null && temp.ColorTop != ItemID.None)
                {
                    BottleCurrent = temp;
                }
                else
                {
                    ClearCache();
                }
            }
            else
            {
                ClearCache();
            }
        }

        private void OnFirst()
        {
            RaycastHit2D hit = Physics2D.Raycast(posWorld, Vector2.zero, 10, layerBottle);
            if (hit.collider != null)
            {
                ControlBottle temp = hit.collider.GetComponent<ControlBottle>();
                if (temp != null && (temp.CanReceive == bottleCurrent.ColorTop || temp.CanReceive == ItemID.ColorEnd))
                {
                    bottleCurrent.Target = temp;
                    bottleCurrent = null;
                }
                else
                {
                    ClearCache();
                }
            }
            else
            {
                ClearCache();
            }
        }
        private void ClearCache()
        {
            if (BottleCurrent != null)
            {
                BottleCurrent.OnNoSelect();
                BottleCurrent = null;
            }
        }
        #region Input

        private void Start()
        {
            layerBottle = LayerMask.GetMask("Bottle");
            cam.allowDynamicResolution = true;
#if UNITY_EDITOR
            Application.targetFrameRate = 1000;
            var resolution = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen, 1000);
            Application.targetFrameRate = 1000;
#else
            Application.targetFrameRate = 120;
            var resolution = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen, 120);
            Application.targetFrameRate = 120;
#endif
        }
        private Touch onlyTouch;
        private Vector2 posWorld;

        private void Update()
        {
            if (Input.touchCount <= 0)
            {
                return;
            }

            onlyTouch = Input.GetTouch(0);
            posWorld = cam.ScreenToWorldPoint(onlyTouch.position);
            if (onlyTouch.phase == TouchPhase.Began)
            {
                if (BottleCurrent != null)
                {
                    OnFirst();
                }
                else
                {
                    OnNoFirst();
                }
            }
        }

        #endregion
    }

    [System.Serializable]
    public class DataSpawn
    {
        public ItemID[] dataInBottle;
    }
}