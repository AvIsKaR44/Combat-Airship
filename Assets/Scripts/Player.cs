using UnityEngine;

    public class Player : SingletonBase<Player>
    {
        public static AirShip SelectedSpaceShip;
        
        [SerializeField] private int m_NumLives;
        
        [SerializeField] private AirShip m_PlayerShipPrefab;

        public AirShip ActiveShip => m_Ship;

        [SerializeField] private CameraController m_CameraController;
        [SerializeField] private MovementController m_MovementController;

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
                if(SelectedSpaceShip == null)
                {
                    return m_PlayerShipPrefab;
                }
                else
                {
                    return SelectedSpaceShip;
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
            var newPlayerShip = Instantiate(m_PlayerShipPrefab);

            m_Ship = newPlayerShip.GetComponent<AirShip>();

            m_Ship.EventOnDeath.AddListener(OnShipDeath);

            m_CameraController.SetTarget(m_Ship.transform);
            m_MovementController.SetTargetShip(m_Ship);
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

