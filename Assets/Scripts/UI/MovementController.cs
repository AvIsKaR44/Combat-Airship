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

    [Header("AirShip movement")]
    [SerializeField] private float m_ForwardThrust = 8f;
    [SerializeField] private float m_BackwardThrust = 4f;
    [SerializeField] private float m_VerticalThrust = 5f;
    [SerializeField] private float m_MaxForwardSpeed = 8f;
    [SerializeField] private float m_MaxBackwardSpeed = 4f;
    [SerializeField] private float m_BuoyancyForce = 5f;

    [Header("Inertion")]
    [SerializeField] private float m_ThrustSmooth = 2f;
    [SerializeField] private float m_VerticalSmooth = 2f;
    [SerializeField] private float m_AirResistance = 0.98f;

    private AirShip m_TargetShip;
    private VirtualGamePad m_VirtualGamePad;
    private Rigidbody2D m_Rigidbody2D;

    private float currentHorizontal;
    private float currentVertical;
    private float targetHorizontal;
    private float targetVertical;

    public void Construct(VirtualGamePad virtualGamePad)
    {
        m_VirtualGamePad = virtualGamePad;
    }

    private void Start()
    {
        if (m_TargetShip == null)
            m_TargetShip = GetComponent<AirShip>();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.gravityScale = 1f;
            m_Rigidbody2D.linearDamping = 0.5f;
            m_Rigidbody2D.angularDamping = 0f;
            m_Rigidbody2D.mass = 5f;
            m_Rigidbody2D.freezeRotation = true; // Вращение только через TorqueControl
        }

        // Активация джойстика
        if (m_VirtualGamePad != null)
        {
            if (m_VirtualGamePad.VirtualJoystick != null)
            {
                m_VirtualGamePad.VirtualJoystick.gameObject.SetActive(
                    m_ControlMode == ControlMode.Mobile ||
                    m_ControlMode == ControlMode.Both);
            }

            SetMobileButtonActive(m_VirtualGamePad.MobileFirePrimary);
            SetMobileButtonActive(m_VirtualGamePad.MobileFireSecondary);
        }

        targetHorizontal = 0f;
        currentHorizontal = 0f;
        targetVertical = 0f;
        currentVertical = 0f;
    }

    private void Update()
    {
        // Получаем ввод
        switch (m_ControlMode)
        {
            case ControlMode.Desktop:
                GetDesktopInput();
                break;
            case ControlMode.Mobile:
                GetMobileInput();
                break;
            case ControlMode.Both:
                GetDesktopInput();
                GetMobileInput();
                break;
        }

        // Плавная интерполяция
        currentHorizontal = Mathf.Lerp(currentHorizontal, targetHorizontal, Time.deltaTime * m_ThrustSmooth);
        currentVertical = Mathf.Lerp(currentVertical, targetVertical, Time.deltaTime * m_VerticalSmooth);
    }

    private void FixedUpdate()
    {
        if (m_TargetShip == null || m_Rigidbody2D == null) return;

        // Подъемная сила (компенсация гравитации)
        m_Rigidbody2D.AddForce(Vector2.up * m_BuoyancyForce, ForceMode2D.Force);

        // Горизонтальное движение
        float currentThrust = currentHorizontal > 0 ? m_ForwardThrust : m_BackwardThrust;
        m_Rigidbody2D.AddForce(Vector2.right * (currentHorizontal * currentThrust), ForceMode2D.Force);

        // Вертикальное движение
        m_Rigidbody2D.AddForce(Vector2.up * (currentVertical * m_VerticalThrust), ForceMode2D.Force);

        // Ограничение скорости
        float maxSpeedX = currentHorizontal > 0 ? m_MaxForwardSpeed : m_MaxBackwardSpeed;
        Vector2 velocity = m_Rigidbody2D.linearVelocity;
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX, maxSpeedX);
        velocity.y = Mathf.Clamp(velocity.y, -m_MaxForwardSpeed * 0.7f, m_MaxForwardSpeed * 0.7f);
        m_Rigidbody2D.linearVelocity = velocity;

        // Сопротивление воздуха
        m_Rigidbody2D.linearVelocity *= m_AirResistance;

        // Границы уровня
        if (LevelBoundary.Instance != null)
        {
            m_Rigidbody2D.position = LevelBoundary.Instance.ClampPosition(m_Rigidbody2D.position);
        }

        // Передаём управление в AirShip
        m_TargetShip.ThrustControl = currentHorizontal;
        m_TargetShip.TorqueControl = currentVertical; // Крен при подъёме/спуске
    }

    private void LateUpdate()
    {
        // Дополнительная защита от выхода за границы (трансформ)
        if (LevelBoundary.Instance != null)
        {
            transform.position = LevelBoundary.Instance.ClampPosition(transform.position);
        }
    }

    private void GetDesktopInput()
    {
        if (m_TargetShip == null) return;

        targetHorizontal = Input.GetAxisRaw("Horizontal");
        targetVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space))
            m_TargetShip.Fire(TurretMode.Primary);

        if (Input.GetKey(KeyCode.B))
            m_TargetShip.Fire(TurretMode.Secondary);
    }

    private void GetMobileInput()
    {
        Vector3 dir = m_VirtualGamePad?.VirtualJoystick?.Value ?? Vector3.zero;

        if (m_TargetShip != null)
        {
            targetHorizontal = dir.x;
            targetVertical = dir.y;
        }

        if (m_VirtualGamePad?.MobileFirePrimary?.IsHold == true)
            m_TargetShip?.Fire(TurretMode.Primary);

        if (m_VirtualGamePad?.MobileFireSecondary?.IsHold == true)
            m_TargetShip?.Fire(TurretMode.Secondary);
    }

    private void SetMobileButtonActive(PointerClickHold button)
    {
        if (button != null)
        {
            button.gameObject.SetActive(
                m_ControlMode == ControlMode.Mobile ||
                m_ControlMode == ControlMode.Both);
        }
    }

    public void SetTargetShip(AirShip ship)
    {
        m_TargetShip = ship;
        if (m_TargetShip != null)
            m_Rigidbody2D = m_TargetShip.GetComponent<Rigidbody2D>();
    }
}