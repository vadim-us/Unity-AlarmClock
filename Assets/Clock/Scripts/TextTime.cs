using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Clock
{
    /// <summary>
    /// Ввод-вывод времени в числовом виде
    /// </summary>
    public class TextTime : MonoBehaviour
    {
        /// <summary>
        /// ссылка на основной класс часов
        /// </summary>
        [SerializeField]
        private Clock clock;

        [SerializeField]
        private GameObject hourText;
        [SerializeField]
        private GameObject minuteText;

        [SerializeField]
        private bool isAlarm;

        private TMP_InputField inputHour;
        private TMP_InputField inputMinute;

        private int prevNum;

        private void Start()
        {
            var click = GetComponentInParent<Button>().onClick;

            inputHour = hourText.GetComponent<TMP_InputField>();
            if (inputHour != null)
                AddListener(click, inputHour, 24);

            inputMinute = minuteText.GetComponent<TMP_InputField>();
            if (inputMinute != null)
                AddListener(click, inputMinute, 60);

            void AddListener(Button.ButtonClickedEvent click, TMP_InputField inputFild, int maxNumber)
            {
                inputFild.onSelect.AddListener((text) => { click.Invoke(); InputStart(inputFild); });
                inputFild.onValueChanged.AddListener((text) => { InputTime(text, inputFild, maxNumber); });
                inputFild.onEndEdit.AddListener((text) => { InputEnd(inputFild); });
            }
        }

        /// <summary>
        /// начало ввода
        /// </summary>
        private void InputStart(TMP_InputField input)
        {
            prevNum = int.Parse(input.text);
        }

        /// <summary>
        /// Ввод времени в текстовое поле
        /// </summary>
        private void InputTime(string text, TMP_InputField input, int maxNumber)
        {
            if (text != "")
            {
                int num = int.Parse(text);
                if (num >= 0 && num < maxNumber)
                    prevNum = num;
                else
                    input.text = prevNum.ToString();
            }
        }

        /// <summary>
        /// Завершение ввода времени в текстовое поле
        /// </summary>
        private void InputEnd(TMP_InputField input)
        {
            // правка отображение времени
            input.text = prevNum.ToString("00");

            clock.SetAlarmTime(int.Parse(inputHour.text), int.Parse(inputMinute.text));
        }

        /// <summary>
        /// Установка времени в текстовое поле
        /// </summary>
        public void SetText(DateTime time)
        {
            if (isAlarm)
                SetText(hourText.GetComponent<TMP_InputField>(), minuteText.GetComponent<TMP_InputField>(), time);
            else
                SetText(hourText.GetComponent<TMP_Text>(), minuteText.GetComponent<TMP_Text>(), time);
        }

        /// <summary>
        /// Установка времени будильника в текстовое поле
        /// </summary>
        private void SetText(TMP_InputField hours, TMP_InputField minutes, DateTime time)
        {
            hours.text = time.Hour.ToString("00");
            minutes.text = time.Minute.ToString("00");
        }

        /// <summary>
        /// Установка текущего времени в текстовое поле
        /// </summary>
        private void SetText(TMP_Text hours, TMP_Text minutes, DateTime time)
        {
            hours.text = time.Hour.ToString("00");
            minutes.text = time.Minute.ToString("00");
        }
    }
}
