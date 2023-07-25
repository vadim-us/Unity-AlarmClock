namespace UnityEngine.UI
{
    /// <summary>
    /// Ограничение интерфейса до безопасной зоны
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class SafeArea : MonoBehaviour
    {
        private DrivenRectTransformTracker m_Tracker;
        private Canvas canvas;
        public RectTransform safeAreaTransform;

        protected void OnEnable()
        {
            canvas = GetComponent<Canvas>();
            UpdateRect();
        }

        protected void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        protected void OnDisable()
        {
            m_Tracker.Clear();
        }

        private void UpdateRect()
        {
            if (!isActiveAndEnabled)
                return;

            safeAreaTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            safeAreaTransform.sizeDelta = new Vector2(0, 0);
            safeAreaTransform.anchorMin = new Vector2(0, 0);
            safeAreaTransform.anchorMax = new Vector2(1, 1);
            safeAreaTransform.localScale = new Vector3(1, 1, 1);

            m_Tracker.Clear();
            m_Tracker.Add(this, safeAreaTransform, DrivenTransformProperties.All);

            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
        }
    }
}