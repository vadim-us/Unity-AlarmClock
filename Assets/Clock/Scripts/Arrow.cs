using UnityEngine;
using UnityEngine.EventSystems;

namespace Clock
{
    /// <summary>
    /// ������� �����
    /// </summary>
    public class Arrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region filds
        /// <summary>
        /// ������ �� �������� ����� �����
        /// </summary>
        [SerializeField]
        private Clock clock;

        /// <summary>
        /// ���������� �� �����������
        /// </summary>
        [SerializeField]
        private bool interactable = false;

        public bool SetInteractable
        {
            set { interactable = value; }
        }

        /// <summary>
        /// ��� �������
        /// </summary>
        [SerializeField]
        private EClockArrow type;
        public EClockArrow Type { get { return type; } }

        /// <summary>
        /// ������� �������
        /// </summary>
        private int curentSegment;

        /// <summary>
        /// ������� �������� � �������� �� ����������
        /// </summary>
        private int segmentDegree;
        #endregion

        #region metods
        public void Start()
        {
            // ����������� ������� �������� �� ������ ���� �������
            segmentDegree = type == EClockArrow.hour ? Clock.segmentDegree.segment12 : Clock.segmentDegree.segment60;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (interactable)
            {
                // ��������� �������� � ������ �������� �������
                curentSegment = GetSegment();

                // ������������ ����������� ����������� ������ �������
                if (type == EClockArrow.hour)
                    clock.GetArrow.min.SetInteractable = false;

                if (type == EClockArrow.min)
                    clock.GetArrow.hour.SetInteractable = false;
            }
        }

        /// <summary>
        /// ��������� ������ ��������
        /// </summary>
        private int GetSegment()
        {
            // ����� �������� (�� 0 �� 12 ��� 60)
            int segment = Mathf.FloorToInt((360 - transform.eulerAngles.z) / segmentDegree);

            if (type == EClockArrow.hour)
                // ���� ������� � ����� ����� 12 �������� �� 0 
                segment = segment == 12 ? 0 : segment;

            if (type == EClockArrow.min)
                // ���� ������� � ����� ����� 60 �������� �� 0
                segment = segment == 60 ? 0 : segment;

            return segment;
        }

        /// <summary>
        /// ������ ����������� �������
        /// </summary>
        private void DragArrow()
        {
            // ������ �������� ��������
            Vector3 segmentArrowDirection = Quaternion.Euler(0, 0, 360 - (curentSegment * segmentDegree)) * Vector3.up;
            Debug.DrawRay(transform.position, segmentArrowDirection.normalized * 500, Color.red);

            // ������ � ������� ������� (���� ��� ������)
            Vector3 CursorDirection = Input.touchCount > 0 ?
                (Vector3)Input.GetTouch(0).position - transform.position
                : Input.mousePosition - transform.position;
            Debug.DrawRay(transform.position, CursorDirection.normalized * 500, Color.blue);

            // ������� ������� ����� �� ��������
            transform.rotation = Quaternion.LookRotation(transform.forward, CursorDirection);

            switch (type)
            {
                case EClockArrow.hour: // ��� ����������� ������� �������

                    // ����������� �������� ������� ������������ ��������� ������� �������
                    clock.GetArrow.min.transform.rotation = Quaternion.Euler(0, 0, (transform.eulerAngles.z % segmentDegree) * 12);

                    int newMin = Mathf.FloorToInt(60 - (transform.eulerAngles.z % segmentDegree) * 2);

                    // ���������� ������� �������� ������ ������������ ��������� ������� �������
                    clock.AddAlarmTime(EClockArrow.min, newMin - clock.GetAlarmTime.Minute);

                    //clock.alarmTime = clock.alarmTime.AddMinutes(newMin - clock.alarmTime.Minute);
                    //clock.SetAlarmText();
                    break;
                case EClockArrow.min: // ��� ����������� �������� �������

                    // ����������� ������� ������� ������������ ��������� �������� �������
                    clock.GetArrow.hour.transform.rotation = Quaternion.Euler(0, 0, 360 -
                        (clock.GetAlarmTime.Hour * Clock.segmentDegree.segment12 + clock.GetAlarmTime.Minute * Clock.segmentDegree.segment12 / 60)
                        );
                    break;
            }

            // ���� ����� ��������� �������
            float degree = Vector3.Angle(segmentArrowDirection.normalized, CursorDirection.normalized);

            // �� ����� ������� ��������� ����� ��������� �������
            int newSegment = GetSegment();

            // ���������� ������� �������� ������ ������ � ������ ��������� � ����� �������
            if (curentSegment != newSegment)
            {
                curentSegment = newSegment;

                // � ����� ������� ������� �������
                float leftOrRight = Vector3.Cross(segmentArrowDirection.normalized, CursorDirection.normalized).z;

                // ������ �������
                if (leftOrRight > 0)
                {
                    // ������� � ���������� ��������� ����� ������� � ����� ���������
                    int segmentDifference = Mathf.CeilToInt(degree / segmentDegree);

                    clock.AddAlarmTime(type, -segmentDifference);
                }

                // �� �������
                if (leftOrRight < 0)
                {
                    // ������� � ���������� ��������� ����� ������� � ����� ���������
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