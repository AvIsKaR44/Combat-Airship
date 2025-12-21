using UnityEngine;

    public class MovementController : MonoBehaviour
    {
        public enum ControlMode
        {
            Desktop,
            Mobile,
            Both
        }

        [SerializeField] private ControlMode m_ControlMode;
        [SerializeField] private AirShip m_TargetShip;
        [SerializeField] private VirtualJoystick m_MobileJoystick;
        [SerializeField] private PointerClickHold m_MobileFirePrimary;
        [SerializeField] private PointerClickHold m_MobileFireSecondary;
        [SerializeField] private PointerClickHold m_MobileFireShot;
        [SerializeField] private PointerClickHold m_MobileIceShot;

        [SerializeField] private float m_ThrustSensitivity = 1.0f;
        [SerializeField] private float m_TorqueSensitivity = 1.0f;

        private void Start()
        {
            if (m_TargetShip == null)
                m_TargetShip = GetComponent<AirShip>();

            // Активация джойстика с проверкой на null
            if (m_MobileJoystick != null)
            {
                m_MobileJoystick.gameObject.SetActive(
                    m_ControlMode == ControlMode.Mobile ||
                    m_ControlMode == ControlMode.Both
                );
            }

            // Активация кнопок через единый метод
            SetMobileButtonActive(m_MobileFirePrimary);
            SetMobileButtonActive(m_MobileFireSecondary);
            SetMobileButtonActive(m_MobileFireShot);
            SetMobileButtonActive(m_MobileIceShot);
        }

        private void Update()
        {
            // Обработка ввода для разных режимов
            if (m_ControlMode == ControlMode.Desktop)
            {
                GetDesktopInput();
            }
            else if (m_ControlMode == ControlMode.Mobile)
            {
                GetMobileInput();
            }
            else if (m_ControlMode == ControlMode.Both)
            {
                GetDesktopInput();
                GetMobileInput();
            }
        }

        private void SetMobileButtonActive(PointerClickHold button)
        {
            if (button != null)
            {
                button.gameObject.SetActive(
                    m_ControlMode == ControlMode.Mobile ||
                    m_ControlMode == ControlMode.Both
                );
            }
        }

        private void GetDesktopInput()
        {
            if (m_TargetShip == null) return;

            // Управление движением
            float thrust = Input.GetAxis("Vertical");
            float torque = -Input.GetAxis("Horizontal");
            m_TargetShip.ThrustControl = thrust;
            m_TargetShip.TorqueControl = torque;

            // Стрельба
            if (Input.GetKey(KeyCode.Space))
                m_TargetShip.Fire(TurretMode.Primary);

            if (Input.GetKey(KeyCode.X))
                m_TargetShip.Fire(TurretMode.Secondary);

            if (Input.GetKey(KeyCode.V))
                m_TargetShip.Fire(TurretMode.FireShot);

            if (Input.GetKey(KeyCode.C))
                m_TargetShip.Fire(TurretMode.IceShot);
        }

        private (float, float) GetMobileInput()
        {
            // Безопасное получение направления джойстика
            Vector3 dir = m_MobileJoystick != null ?
                m_MobileJoystick.Value :
                Vector3.zero;

            float thrust = 0f;
            float torque = 0f;

            if (m_TargetShip != null)
            {
                thrust = Mathf.Max(0, Vector2.Dot(dir, m_TargetShip.transform.up));
                torque = Vector2.Dot(dir, m_TargetShip.transform.right);
            }

            // Обработка стрельбы с проверкой на null
            if (m_MobileFirePrimary != null && m_MobileFirePrimary.IsHold)
                m_TargetShip?.Fire(TurretMode.Primary);

            if (m_MobileFireSecondary != null && m_MobileFireSecondary.IsHold)
                m_TargetShip?.Fire(TurretMode.Secondary);

            if (m_MobileFireShot != null && m_MobileFireShot.IsHold)
                m_TargetShip?.Fire(TurretMode.FireShot);

            if (m_MobileIceShot != null && m_MobileIceShot.IsHold)
                m_TargetShip?.Fire(TurretMode.IceShot);

            return (thrust, torque);
        }

        public void SetTargetShip(AirShip ship)
        {
            m_TargetShip = ship;
        }

        public float TorqueSensitivity
        {
            get => m_TorqueSensitivity;
            set => m_TorqueSensitivity = value;
        }
        public float ThrustSensitivity
        {
            get => m_ThrustSensitivity;
            set => m_ThrustSensitivity = value;
        }
    }

