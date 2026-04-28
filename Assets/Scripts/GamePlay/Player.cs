using Unity.Cinemachine;
using UnityEngine;


public class Player : SingletonBase<Player>
{
    public static AirShip SelectedAirShip;

    [SerializeField] private int m_NumLives;

    [SerializeField] private AirShip m_PlayerShipPrefab;

    public AirShip ActiveShip => m_Ship;

    private MovementController m_MovementController;
    private Transform m_SpawnPoint;

    public void Construct(MovementController movementController, Transform spawnPoint)
    {
        m_MovementController = movementController;
        m_SpawnPoint = spawnPoint;
    }

    private AirShip m_Ship;

    private int m_Score;
    private int m_NumKills;

    public int Score => m_Score;
    public int NumKills => m_NumKills;
    public int NumLives => m_NumLives;

    public AirShip ShipPrefab
    {
        get
        {
            if (SelectedAirShip == null)
            {
                return m_PlayerShipPrefab;
            }
            else
            {
                return SelectedAirShip;
            }
        }
    }

    private void Start()
    {
        Respawn();
    }

    private void OnShipDeath()
    {
        m_NumLives--;

        if (m_NumLives > 0)
            Respawn();
    }

    private void Respawn()
    {
        var newPlayerShip = Instantiate(ShipPrefab);

        m_Ship = newPlayerShip.GetComponent<AirShip>();

        m_Ship.EventOnDeath.AddListener(OnShipDeath);

        m_MovementController.SetTargetShip(m_Ship);

        UpdateCinemachineCamera();
    }

    private void UpdateCinemachineCamera()
    {
        var virtualCamera = FindFirstObjectByType<CinemachineVirtualCameraBase>();

        if (virtualCamera != null)
        {
            virtualCamera.Follow = m_Ship.transform;
        }
    }

    public void AddKill()
    {
        m_NumKills += 1;
    }

    public void AddScore(int num)
    {
        m_Score += num;
    }
}

