using UnityEngine;
using UnityEngine.EventSystems;

namespace Clock
{
    /// <summary>
    /// стрелка часов
    /// </summary>
    public class Arrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region filds
        /// <summary>
        /// ссылка на основной класс часов
        /// </summary>
        [SerializeField]
        private Clock clock;

        /// <summary>
        /// разрешение на перемешение
        /// </summary>
        [SerializeField]
        private bool interactable = false;

        public bool SetInteractable
        {
            set { interactable = value; }
        }

        /// <summary>
        /// тип стрелки
        /// </summary>
        [SerializeField]
        private EClockArrow type;
        public EClockArrow Type { get { return type; } }

        /// <summary>
        /// текущий сегмент
        /// </summary>
        private int curentSegment;

        /// <summary>
        /// размера сегмента в градусах на циферблате
        /// </summary>
        private int segmentDegree;
        #endregion

        #region metods
        public void Start()
        {
            // определение размера сегмента на основе типа стрелки
            segmentDegree = type == EClockArrow.hour ? Clock.segmentDegree.segment12 : Clock.segmentDegree.segment60;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (interactable)
            {
                // получение сегмента в начале двишения стрелки
                curentSegment = GetSegment();

                // блокирование возможности перемещения другой стрелки
                if (type == EClockArrow.hour)
                    clock.GetArrow.min.SetInteractable = false;

                if (type == EClockArrow.min)
                    clock.GetArrow.hour.SetInteractable = false;
            }
        }

        /// <summary>
        /// получения номера сегмента
        /// </summary>
        private int GetSegment()
        {
            // номер сегмента (от 0 до 12 или 60)
            int segment = Mathf.FloorToInt((360 - transform.eulerAngles.z) / segmentDegree);

            if (type == EClockArrow.hour)
                // если сегмент у часов равен 12 сбросить до 0 
                segment = segment == 12 ? 0 : segment;

            if (type == EClockArrow.min)
                // если сегмент у минут равен 60 сбросить до 0
                segment = segment == 60 ? 0 : segment;

            return segment;
        }

        /// <summary>
        /// ручное перемешения стрелки
        /// </summary>
        private void DragArrow()
        {
            // вектор текушего сегмента
            Vector3 segmentArrowDirection = Quaternion.Euler(0, 0, 360 - (curentSegment * segmentDegree)) * Vector3.up;
            Debug.DrawRay(transform.position, segmentArrowDirection.normalized * 500, Color.red);

            // вектор в сторону курсора (мыши или пальца)
            Vector3 CursorDirection = Input.touchCount > 0 ?
                (Vector3)Input.GetTouch(0).position - transform.position
                : Input.mousePosition - transform.position;
            Debug.DrawRay(transform.position, CursorDirection.normalized * 500, Color.blue);

            // поворот стрелки вслед за курсором
            transform.rotation = Quaternion.LookRotation(transform.forward, CursorDirection);

            switch (type)
            {
                case EClockArrow.hour: // при перемешении часовой стрелки

                    // перемешение минутной стрелки относительно положения часовой стрелки
                    clock.GetArrow.min.transform.rotation = Quaternion.Euler(0, 0, (transform.eulerAngles.z % segmentDegree) * 12);

                    int newMin = Mathf.FloorToInt(60 - (transform.eulerAngles.z % segmentDegree) * 2);

                    // Обновление времени минутной стреки относительно положения часовой стрелки
                    clock.AddAlarmTime(EClockArrow.min, newMin - clock.GetAlarmTime.Minute);

                    //clock.alarmTime = clock.alarmTime.AddMinutes(newMin - clock.alarmTime.Minute);
                    //clock.SetAlarmText();
                    break;
                case EClockArrow.min: // при перемешении минутной стрелки

                    // перемешение часовой стрелки относительно положения минутной стрелки
                    clock.GetArrow.hour.transform.rotation = Quaternion.Euler(0, 0, 360 -
                        (clock.GetAlarmTime.Hour * Clock.segmentDegree.segment12 + clock.GetAlarmTime.Minute * Clock.segmentDegree.segment12 / 60)
                        );
                    break;
            }

            // угол между векторами стрелки
            float degree = Vector3.Angle(segmentArrowDirection.normalized, CursorDirection.normalized);

            // на какой сегмент указывает новое положение стрелки
            int newSegment = GetSegment();

            // Обновление времени выбраной стреки только в случаи поподания в новый сегмент
            if (curentSegment != newSegment)
            {
                curentSegment = newSegment;

                // в какую сторону двигали стрелку
                float leftOrRight = Vector3.Cross(segmentArrowDirection.normalized, CursorDirection.normalized).z;

                // против часовой
                if (leftOrRight > 0)
                {
                    // разница в количестве сегментов между текущим и новым сегментом
                    int segmentDifference = Mathf.CeilToInt(degree / segmentDegree);

                    clock.AddAlarmTime(type, -segmentDifference);
                }

                // по часовой
                if (leftOrRight < 0)
                {
                    // разница в количестве сегментов между текущим и новым сегментом
                    int segmentDifference = Mathf.FloorToInt(degree / segmentDegree);

                    clock.AddAlarmTime(type, segmentDifference);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (interactable)
                DragArrow();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (interactable)
            {
                clock.SetTimeOnClockFace(clock.GetAlarmTime);

                clock.GetArrow.min.SetInteractable = true;
                clock.GetArrow.hour.SetInteractable = true;
            }
        }
        #endregion
    }
}