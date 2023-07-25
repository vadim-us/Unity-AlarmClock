using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Clock
{
    /// <summary>
    /// класс дл€ десериализации json
    /// </summary>
    public class JsonDate
    {
        public long time;
        public long timestamp;
    }

    #region enum
    /// <summary>
    /// –ежим часов
    /// </summary>
    public enum EClockMode
    {
        Clock,
        Alarm
    }

    /// <summary>
    /// “ипы стрелок
    /// </summary>
    public enum EClockArrow
    {
        hour,
        min,
        sec
    }
    #endregion

    /// <summary>
    /// ќсновной класс часов
    /// </summary>
    public class Clock : MonoBehaviour
    {
        #region filds
        [Header("ѕрефабы стрелок")]
        public List<GameObject> hourArrows;
        public List<GameObject> minuteArrows;
        public List<GameObject> secondArrows;

        [Space, Header("ћеста вывода времени")]
        [SerializeField]
        private TextTime clockText;
        [SerializeField]
        private TextTime alarmText;

        [Space, Header("стрелки на циферблате")]
        [SerializeField]
        private Arrow hours;
        [SerializeField]
        private Arrow minutes;
        [SerializeField]
        private Arrow seconds;

        public (Arrow hour, Arrow min, Arrow sec) GetArrow
        {
            get { return (hours, minutes, seconds); }
        }

        [Space]
        [SerializeField]
        private Animator clockAnimator;

        /// <summary>
        /// текушее врем€
        /// </summary>
        private DateTime currentTime;

        /// <summary>
        /// врем€ будильника
        /// </summary>
        private DateTime alarmTime;

        /// <summary>
        /// ѕереключатель будильника
        /// </summary>
        [Space, SerializeField]
        private Toggle alarmB;

        /// <summary>
        /// ќкно звон€щего будильника
        /// </summary>
        [SerializeField]
        private GameObject alarm;

        public DateTime GetAlarmTime
        {
            get { return alarmTime; }
        }

        /// <summary>        
        /// градус дилени€ у часов и минут
        /// </summary>
        public static readonly (int segment12, int segment60) segmentDegree = (30, 6);

        private bool tick = true;

        private bool loadTime;
        #endregion

        #region metods
        public void Start()
        {
            Application.targetFrameRate = 60;
            StartCoroutine(UpdateTime());
            StartCoroutine(Tick());
        }

        /// <summary>
        ///  оректировка времени каждый час
        /// </summary>
        private IEnumerator UpdateTime()
        {
            while (true)
            {
                StartCoroutine(GetTimeFromUrl());
                yield return new WaitForSeconds(3600);
            }
        }

        /// <summary>
        /// ѕолучение времени с интернет-сервиса
        /// </summary>
        private IEnumerator GetTimeFromUrl()
        {
            loadTime = false;
            yield return StartCoroutine(GetRequest("https://yandex.com/time/sync.json"));
            if (!loadTime) yield return StartCoroutine(GetRequest("https://tools.aimylogic.com/api/now"));

            if (!loadTime) // если не удалось получить врем€ по сети вз€ть врем€ устройства
                currentTime = DateTime.Now;

            if (alarmTime.Year == 1) //если небыло установлено врем€ дл€ будильника
                //устанавливает будильник на текушее дату
                alarmTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);

            SetTimeOnClockFace(currentTime);
            clockText.SetText(currentTime);
        }

        /// <summary>
        /// «апрос времени
        /// </summary>
        private IEnumerator GetRequest(string uri)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);

                    loadTime = true;

                    string json = webRequest.downloadHandler.text;
                    JsonDate date = JsonUtility.FromJson<JsonDate>(json);

                    long time = date.time != 0 ? date.time : date.timestamp;

                    currentTime = new DateTime(1970, 1, 1).AddMilliseconds(time).ToLocalTime();
                    break;
            }
        }

        /// <summary>
        /// ƒобавление секунды к текушему времени
        /// </summary>
        private IEnumerator Tick()
        {
            while (true)
            {
                currentTime = currentTime.AddSeconds(1);

                if (currentTime.Second == 0)
                {
                    clockText.SetText(currentTime);
                    Alarm();
                }

                AnimArrow();

                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// ”становка времени будильника
        /// </summary>
        public void SetAlarmTime(int h, int m)
        {
            alarmTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, h, m, 0);

            SetTimeOnClockFace(alarmTime);
        }

        /// <summary>
        /// ƒобавление времени будильнику
        /// </summary>
        public void AddAlarmTime(EClockArrow type, int dif)
        {
            if (type == EClockArrow.hour)
                alarmTime = alarmTime.AddHours(dif);

            if (type == EClockArrow.min)
                alarmTime = alarmTime.AddMinutes(dif);

            SetAlarmText();
        }

        /// <summary>
        /// ”становка времени на циферблате
        /// </summary>
        public void SetTimeOnClockFace(DateTime time)
        {
            hours.transform.rotation = Quaternion.Euler(0, 0, GetDegree(EClockArrow.hour));
            minutes.transform.rotation = Quaternion.Euler(0, 0, GetDegree(EClockArrow.min));
            seconds.transform.rotation = Quaternion.Euler(0, 0, GetDegree(EClockArrow.sec));

            float GetDegree(EClockArrow type)
            {
                switch (type)
                {
                    case EClockArrow.hour:
                        return -(segmentDegree.segment12 * time.Hour + (float)segmentDegree.segment12 / 60 * time.Minute);
                    case EClockArrow.min:
                        return -segmentDegree.segment60 * time.Minute;
                    case EClockArrow.sec:
                        return -segmentDegree.segment60 * time.Second;
                }
                return 0;
            }
        }

        /// <summary>
        /// ”становка времени в текстовое поле
        /// </summary>
        public void SetAlarmText()
        {
            alarmText.SetText(alarmTime);
        }

        /// <summary>
        /// Ѕудильник (учитыват только врем€)
        /// </summary>
        public void Alarm()
        {
            if (alarmB.isOn)
            {
                // текущее врем€ 
                var time1 = new DateTime(1, 1, 1, currentTime.Hour, currentTime.Minute, 0);
                // врем€ будильника
                var time2 = new DateTime(1, 1, 1, alarmTime.Hour, alarmTime.Minute, 0);

                if (time1 == time2)
                {
                    alarm.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Ѕудильник остановка
        /// </summary>
        public void AlarmStop()
        {
            alarmB.isOn = false;
            alarm.SetActive(false);
        }

        /// <summary>
        /// јнимаци€ движени€ стрелок
        /// </summary>
        private void AnimArrow()
        {
            if (tick)
            {
                // движение секундной стрелки
                seconds.transform.Rotate(0, 0, -segmentDegree.segment60);
                clockAnimator.SetTrigger("Sec");

                if (currentTime.Second == 0)
                {
                    //движение минутной стрелки когда секундна€ равна 0
                    minutes.transform.Rotate(0, 0, -segmentDegree.segment60);
                    clockAnimator.SetTrigger("Min");

                    //движение часовой стрелки относительно положению минутной
                    hours.transform.Rotate(0, 0, -(segmentDegree.segment12 / 60f));
                }
            }
        }

        /// <summary>        
        /// ѕереключение режимов часы\будильник
        /// </summary>
        public void ToggleClockMode(int clockMode)
        {
            switch ((EClockMode)clockMode)
            {
                case EClockMode.Clock:
                    Toggle(true, currentTime);
                    break;
                case EClockMode.Alarm:
                    Toggle(false, alarmTime);
                    break;
            }

            void Toggle(bool b, DateTime time)
            {
                seconds.gameObject.SetActive(b);

                SetTimeOnClockFace(time);
                tick = b;

                hours.SetInteractable = !b;
                minutes.SetInteractable = !b;
            }
        }
        #endregion
    }
}