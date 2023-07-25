using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Clock
{
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// ������ �� �������� ����� �����
        /// </summary>
        [SerializeField]
        private Clock clock;

        [SerializeField]
        private Animator settingsAnimator;

        private bool showSettings = false;

        [SerializeField]
        private GameObject hideSettings;

        [SerializeField]
        private TMP_Dropdown clockFaceSkins;

        [SerializeField]
        private TMP_Dropdown arrowColors;

        [SerializeField]
        private Image clockFace;

        [SerializeField]
        private Texture2D textureArrow;

        /// <summary>
        /// ������ ����� � ���������� ������
        /// </summary>
        private Color indexSelectedColor;

        /// <summary>
        /// ������� ������
        /// </summary>
        Dictionary<string, Color> colorDictionary = new()
        {
            { "white", Color.white },
            { "orange", new Color(1, 0.7051305f, 0) },
            { "green", Color.green },
            { "blue", Color.blue },
            { "red", Color.red }
        };

        #region metods
        private void Awake()
        {
            FillColorList();
            ChangeColorArrows(arrowColors.value);
        }

        private void FillColorList()
        {
            foreach (var item in colorDictionary)
            {
                CreateSprite(item.Key, item.Value);
            }
        }

        private void CreateSprite(string colorName, Color color)
        {
            var texture = new Texture2D(textureArrow.width, textureArrow.height);

            Color[] colors = textureArrow.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] *= color;
            }

            texture.SetPixels(colors);
            texture.Apply();

            var item = new TMP_Dropdown.OptionData(colorName, Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 1000));
            arrowColors.options.Add(item);
        }

        /// <summary>
        /// ������ �������� ����������
        /// </summary>
        public void ChangeSkin(int i)
        {
            clockFace.sprite = clockFaceSkins.options[i].image;
        }

        /// <summary>
        /// ������ �������� �������
        /// </summary>
        public void ChangeArrows(int i)
        {
            Switch(clock.GetArrow.hour, clock.hourArrows[i]);
            Switch(clock.GetArrow.min, clock.minuteArrows[i]);
            Switch(clock.GetArrow.sec, clock.secondArrows[i]);

            void Switch(Arrow arrow, GameObject arrowImage)
            {
                // ������ �� ������������ ����� ������� � ���������
                var animTransform = arrow.transform.GetChild(0);

                Destroy(animTransform.GetChild(0).gameObject);
                var newArrow = Instantiate(arrowImage, animTransform);

                ChangeColor(arrow, newArrow.GetComponentInChildren<Image>());
            }
        }

        /// <summary>
        /// ����� ����� �������
        /// </summary>
        public void ChangeColorArrows(int i)
        {
            Color color = colorDictionary[arrowColors.options[i].text];
            indexSelectedColor = color;

            ChangeColor(clock.GetArrow.hour);
            ChangeColor(clock.GetArrow.min);
        }

        /// <summary>
        /// ����� ����� �������
        /// </summary>
        private void ChangeColor(Arrow arrow, Image image = null)
        {
            // �� �������� ���� ��������� �������
            if (arrow.Type == EClockArrow.sec) { return; }

            image = image == null ? arrow.GetComponentInChildren<Image>() : image;

            image.color = indexSelectedColor;
        }

        /// <summary>
        /// �����������/������� ��������
        /// </summary>
        public void ShowHideSettings()
        {
            showSettings = !showSettings;
            hideSettings.SetActive(showSettings);

            settingsAnimator.SetBool("show", showSettings);
        }

        #endregion
    }
}
