namespace UnityEngine.UI
{
    [ExecuteAlways]
    public class ChangeOrient : MonoBehaviour
    {

        protected void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            DestroyImmediate(gameObject.GetComponent<HorizontalLayoutGroup>());
            DestroyImmediate(gameObject.GetComponent<VerticalLayoutGroup>());

            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                Component component = gameObject.AddComponent<VerticalLayoutGroup>();
                //vertical.ApplyTo(component);
            }

            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                Component component = gameObject.AddComponent<HorizontalLayoutGroup>();
                //horizontal.ApplyTo(component);
            }
        }
    }
}
