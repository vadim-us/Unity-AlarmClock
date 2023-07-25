using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Scaler", 150)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]

    /// <summary>
    /// Изманение масштабирования до наибольшего размера вписанного в родительские рамки
    /// </summary>
    public class Scaler : UIBehaviour//, ILayoutSelfController
    {
        public RectTransform scaleObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateRect();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            //Debug.Log('0');
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;

            Rect rect = GetComponent<RectTransform>().rect;

            Rect clockRect = scaleObject.rect;

            float scale = 1;

            if(rect.width > rect.height)
            {
                scale = rect.height / clockRect.height;
            }
            else
            {
                scale = rect.width / clockRect.width;
            }

            scaleObject. localScale = new Vector3(1, 1, 1) * scale;
        }
    }
}