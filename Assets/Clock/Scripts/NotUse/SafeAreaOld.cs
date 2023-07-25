using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    //[AddComponentMenu("Layout/SafeArea", 151)]
    //[ExecuteAlways]
    //[RequireComponent(typeof(RectTransform))]
    //[DisallowMultipleComponent]

    /// <summary>
    /// Ограничение интерфейса до безопасной зоны
    /// </summary>
    public class SafeAreaOld : UIBehaviour
    {
        enum orient
        {
            Portrait,
            Landscape
        }

        private orient orientation;

        private DrivenRectTransformTracker m_Tracker;

        public RectTransform safeAreaObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateRect();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();

            m_Tracker.Add(this, safeAreaObject, DrivenTransformProperties.SizeDelta);
            m_Tracker.Add(this, safeAreaObject, DrivenTransformProperties.AnchoredPosition);

            Rect safeArea = Screen.safeArea;
            Debug.Log(safeArea);

            float width = safeArea.width;
            float height = safeArea.height;


            RectTransform rect = safeAreaObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(safeArea.x, safeArea.y);
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}