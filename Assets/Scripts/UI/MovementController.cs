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
    [SerializeField] private float m_ForwardThrust = 8f;        // Сила вперед
    [SerializeField] private float m_BackwardThrust = 4f;       // Сила назад
    [SerializeField] private float m_VerticalThrust = 5f;       // Сила вертикали
    [SerializeField] private float m_MaxForwardSpeed = 8f;      // Макс скорость вперед
    [SerializeField] private float m_MaxBackwardSpeed = 4f;      // Макс скорость назад
    [SerializeField] private float m_BuoyancyForce = 5f;        // Подъемная сила

    [Header("Roll of the AirShip")]
    [SerializeField] private float m_MaxTiltAngle = 15f;        // Макс угол крена
    [SerializeField] private float m_TiltSpeed = 5f;            // Скорость крена
    [SerializeField] private float m_TiltOnForward = 3f;        // Крен при движении вперед
    [SerializeField] private float m_TiltOnBackward = 5f;       // Крен при движении назад
    [SerializeField] private float m_TiltOnVertical = 10f;      // Крен при подъеме/спуске

    [Header("Inertion")]
    [SerializeField] private float m_ThrustSmooth = 2f;         // Плавность горизонтали 
    [SerializeField] private float m_VerticalSmooth = 2f;       // Плавность вертикали
    [SerializeField] private float m_AirResistance = 0.98f;     // Сопротивление воздуха 

    private AirShip m_TargetShip;
    private VirtualGamePad m_VirtualGamePad;
    private Rigidbody2D m_Rigidbody2D;
    private Transform m_ShipVisual;

    private float currentHorizontal;
    private float currentVertical;
    private float targetHorizontal;
    private float targetVertical;
    private float currentTilt;

    public void Construct(VirtualGamePad virtualGamePad)
    {
        m_VirtualGamePad = virtualGamePad;
    }

    private void Start()
    {
        if (m_TargetShip == null)
            m_TargetShip = GetComponent<AirShip>();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        
        if(transform.childCount > 0)
            m_ShipVisual = transform.GetChild(0);
        else
            m_ShipVisual = transform;

        if(m_Rigidbody2D != null)
        {
            m_Rigidbody2D.gravityScale = 1f;
            m_Rigidbody2D.linearDamping = 0.5f;
            m_Rigidbody2D.angularDamping = 0f;
            m_Rigidbody2D.mass = 5f;
            m_Rigidbody2D.freezeRotation = true;
        }
          
        // Активация джойстика с проверкой на null
        if (m_VirtualGamePad != null)
        {
            if (m_VirtualGamePad.VirtualJoystick != null)
            {
                m_VirtualGamePad.VirtualJoystick.gameObject.SetActive(
                    m_ControlMode == ControlMode.Mobile ||
                    m_ControlMode == ControlMode.Both
                    );
            }

            SetMobileButtonActive(m_VirtualGamePad.MobileFirePrimary);
            SetMobileButtonActive(m_VirtualGamePad.MobileFireSecondary);
        }

        targetHorizontal = 1f;
        currentHorizontal = 1f;
    }

    private void Update()
    {
        // Получаем ввод
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
        // Плавная интерполяция
        currentHorizontal = Mathf.Lerp(currentHorizontal, targetHorizontal, Time.deltaTime * m_ThrustSmooth);
        currentVertical = Mathf.Lerp(currentVertical, targetVertical, Time.deltaTime * m_VerticalSmooth);

        // Расчет крена (спрайт направлен вправо по движению вперед)
        float targetTilt = 0f;

        // Крен от вертикали (подъем/спуск)
        targetTilt += -targetVertical * m_TiltOnVertical;

        // Крен от горизонтали (вперед/назад)
        if (targetHorizontal < 0) // Движение назад
            targetTilt += m_TiltOnBackward;
        else if (targetHorizontal > 0) // Движение вперед
            targetTilt += -m_TiltOnForward;

        // Ограничение максимального крена
        targetTilt = Mathf.Clamp(targetTilt, -m_MaxTiltAngle, m_MaxTiltAngle);

        // Плавно применяем крен
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * m_TiltSpeed);

        // Применяем визуальный крен
        if (m_ShipVisual != null)
        {
            m_ShipVisual.localEulerAngles = new Vector3 (0, 0, currentTilt);
        }
    }

    private void FixedUpdate()
    {
        if (m_TargetShip == null || m_Rigidbody2D == null) return;

        // Подъемная сила
        m_Rigidbody2D.AddForce(Vector2.up * m_BuoyancyForce, ForceMode2D.Force);

        // Горизонтальное движение с разной силой для вперед/назад
        float currentThrust = currentHorizontal > 0 ? m_ForwardThrust : m_BackwardThrust;
        Vector2 horizontalForce = Vector2.right * (currentHorizontal * currentThrust);
        m_Rigidbody2D.AddForce(horizontalForce, ForceMode2D.Force);

        // Вертикальное движение
        Vector2 verticalForce = Vector2.up * (currentVertical * m_VerticalThrust);
        m_Rigidbody2D.AddForce(verticalForce, ForceMode2D.Force);

        // Ограничение скорости (разное для вперед/назад)
        float maxSpeedX = currentHorizontal > 0 ? m_MaxForwardSpeed : m_MaxBackwardSpeed;
        Vector2 velocity = m_Rigidbody2D.linearVelocity;
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX, maxSpeedX);
        velocity.y = Mathf.Clamp(velocity.y, -m_MaxForwardSpeed * 0.7f, m_MaxForwardSpeed * 0.7f);
        m_Rigidbody2D.linearVelocity = velocity;

        // Сопротивление воздуха
        m_Rigidbody2D.linearVelocity *= m_AirResistance;

        m_TargetShip.ThrustControl = currentHorizontal;
        m_TargetShip.ThrustControl = currentVertical;
    }

    private void GetDesktopInput()
    {
        if (m_TargetShip == null) return;

        // Управление движением
        targetHorizontal = Input.GetAxisRaw("Horizontal");
        targetVertical = Input.GetAxisRaw("Vertical");

        // Стрельба
        if (Input.GetKey(KeyCode.Space))
            m_TargetShip.Fire(TurretMode.Primary);

        if (Input.GetKey(KeyCode.X))
            m_TargetShip.Fire(TurretMode.Secondary);
    }

    private void GetMobileInput()
    {
        // Безопасное получение направления джойстика
        Vector3 dir = m_VirtualGamePad?.VirtualJoystick?.Value ?? Vector3.zero;

        if (m_TargetShip != null)
        {
            targetHorizontal = dir.x;
            targetVertical = dir.y;
        }

        // Обработка стрельбы с проверкой на null
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
                m_ControlMode == ControlMode.Both
            );
        }
    }

    public void SetTargetShip(AirShip ship)
    {
        m_TargetShip = ship;
        if (m_TargetShip != null) 
            m_Rigidbody2D = m_TargetShip.GetComponent<Rigidbody2D>();
    }
}
