using UnityEngine;
using System.Collections;

    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceShip : Destructible
    {
        [SerializeField] private Sprite m_PreviewImage;
          
        [Header("Space ship")]
        [SerializeField] private float m_Mass;

        /// <summary>
        /// ��������� ������ ����.
        /// </summary>
        [SerializeField] private float m_Thrust;

        /// <summary>
        /// ��������� ����.
        /// </summary>
        [SerializeField] private float m_Mobility;

        /// <summary>
        /// ������������ �������� ��������.
        /// </summary>
        [SerializeField] private float m_MaxLinearVelocity;

        /// <summary>
        /// ������������ ������������ ��������. � ��������/���.
        /// </summary>
        [SerializeField] private float m_MaxAngularVelocity;

        [Header("VFX Settings")]
        [SerializeField] private ParticleSystem m_ExplosionEffect; // ������ ������� ������

        /// <summary>
        /// ����������� ������ �� �����.
        /// </summary>
        private Rigidbody2D m_Rigid;

        public float MaxLianerVelocity => m_MaxLinearVelocity;
        public float MaxAngularVelocity => m_MaxAngularVelocity;
        public Sprite PreviewImage => m_PreviewImage;

        #region Public API

        /// <summary>
        /// ���������� �������� �����. -1.0 �� +1.0
        /// </summary>
        public float ThrustControl { get; set; }

        /// <summary>
        /// ���������� ������������ �����. -1.0 �� +1.0
        /// </summary>
        public float TorqueControl { get; set; }

        #endregion

        #region Unity Event

        [Header("Invulnerability Settings")]
        [SerializeField] private GameObject m_ShieldEffect; // ������ �� ���������� ������ ����
        [SerializeField] private float m_ShieldEffectScale = 1.5f; // ������� ������� ������������ �������

        private bool m_IsInvulnerable;
        private float m_InvulnerabilityTimer;
        private GameObject m_ActiveShield; // �������� ��������� ����

        [Header("Weapon Settings")]
        [SerializeField] private float m_DefaultProjectileSpeed = 15f; // �������� ��������


        public Vector3 Velocity
        {
            get
            {
                if (m_Rigid == null) return Vector3.zero;
                return m_Rigid.linearVelocity;
            }
        }

        protected override void Start()
        {
            base.Start();

            m_Rigid = GetComponent<Rigidbody2D>();
            m_Rigid.mass = m_Mass;
            m_Rigid.inertia = 1;

            if (m_ShieldEffect != null)
            {
                m_ActiveShield = Instantiate(m_ShieldEffect, transform);
                m_ActiveShield.transform.localPosition = Vector3.zero;
                m_ActiveShield.transform.localScale = Vector3.one * m_ShieldEffectScale;
                m_ActiveShield.SetActive(false);
            }


            InitOffensive();
        }

        public void SetInvulnerable(float duration)
        {
            m_IsInvulnerable = true;
            m_InvulnerabilityTimer = duration;

            // ���������� ���
            if (m_ActiveShield != null)
            {
                m_ActiveShield.SetActive(true);
            }

            // ���������� ��������� (����������������)
            ToggleShipVisual(false);
        }

        private void Update()
        {
            
            // ��������� ������� ������������
            if (m_IsInvulnerable)
            {
                m_InvulnerabilityTimer -= Time.deltaTime;
                if (m_InvulnerabilityTimer <= 0)
                {
                    DisableInvulnerability();
                }
            }
        }

        private void DisableInvulnerability()
        {
            m_IsInvulnerable = false;

            // ������������ ���
            if (m_ActiveShield != null)
            {
                m_ActiveShield.SetActive(false);
            }

            // ���������� ���������� ������������
            ToggleShipVisual(true);
        }

        private void ToggleShipVisual(bool normalState)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var rend in renderers)
            {
                // ���������� ��� � �����
                if (rend.gameObject == m_ActiveShield || rend.gameObject == m_ExplosionEffect) continue;

                Color color = rend.color;
                color.a = normalState ? 1f : 0.5f; // 50% ������������ ��� ������������
                rend.color = color;
            }
        }

        public override void ApplyDamage(int damage)
        {
            if (m_IsInvulnerable) return; // ���������� ���� ��� ������������

            base.ApplyDamage(damage);
        }

        private void FixedUpdate()
        {
            UpdateRigidBody();

            UpdateEnergyRegen();
        }

        #endregion

        /// <summary>
        /// ����� ���������� ��� ������� ��� ��������.
        /// </summary>
        private void UpdateRigidBody()
        {
            if (float.IsNaN(TorqueControl))
            {
                Debug.LogError("TorqueControl is NaN!");
                TorqueControl = 0f;
            }

            if (m_Rigid == null) return;

            m_Rigid.AddForce(ThrustControl * m_Thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
            m_Rigid.AddForce(-m_Rigid.linearVelocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
            m_Rigid.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);
            m_Rigid.AddTorque(-m_Rigid.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            if (m_MaxAngularVelocity > Mathf.Epsilon)
            {
                float damping = m_Mobility / m_MaxAngularVelocity;
                m_Rigid.AddTorque(-m_Rigid.angularVelocity * damping * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }

        protected override void OnDeath()
        {
            // �������� ������� ������
            if (m_ExplosionEffect != null)
            {
                var effect = Instantiate(m_ExplosionEffect, transform.position, transform.rotation);

                // ����� ��������� � �������������� ������
                var mainModule = effect.main;
                effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                effect.Play();

                Destroy(effect.gameObject, mainModule.duration + mainModule.startLifetime.constantMax);
            }

            // ���������� ���������� ���� �����������
            Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in allColliders)
            {
                col.enabled = false;
            }

            // ���������� ���������� ������������
            Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in allRenderers)
            {
                rend.enabled = false;
            }

            // ��������� ������ (���� ���� Rigidbody)
            if (TryGetComponent(out Rigidbody2D rb))
            {
                rb.simulated = false;
            }

            // ������ �������� �����������
            StartCoroutine(DelayedDestruction(1.0f));
        }

        private IEnumerator DelayedDestruction(float delay)
        {
            // ���� ���������� �������
            yield return new WaitForSeconds(delay);

            // �������� ������� ������ �����������
            base.OnDeath();
        }

        [SerializeField] private Turret[] m_Turrets;

        public void Fire(TurretMode mode)
        {
            for (int i = 0; i < m_Turrets.Length; i++)
            {
                if (m_Turrets[i].Mode == mode)
                {
                    m_Turrets[i].Fire();
                }
            }
        }

        [SerializeField] private int m_MaxEnergy;
        [SerializeField] private int m_MaxAmmo;
        [SerializeField] private int m_EnergyRegenPerSecond;

        private float m_PrimaryEnergy;
        private float m_SecondaryAmmo;

        public void AddEnergy(int e)
        {
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy + e, 0, m_MaxEnergy);
        }

        public void AddAmmo(int ammo)
        {
            m_SecondaryAmmo = Mathf.Clamp(m_SecondaryAmmo + ammo, 0, m_MaxAmmo);
        }

        private void InitOffensive()
        {
            m_PrimaryEnergy = m_MaxEnergy;
            m_SecondaryAmmo = m_MaxAmmo;
        }

        private void UpdateEnergyRegen()
        {
            m_PrimaryEnergy += (float)m_EnergyRegenPerSecond * Time.fixedDeltaTime;
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy, 0, m_MaxEnergy);
        }

        public bool DrawEnergy(int count)
        {
            if (count == 0) return true;

            if (m_PrimaryEnergy >= count)
            {
                m_PrimaryEnergy -= count;
                return true;
            }

            return false;
        }

        public bool DrawAmmo(int count)
        {
            if (count == 0) return true;

            if (m_SecondaryAmmo >= count)
            {
                m_SecondaryAmmo -= count;
                return true;
            }

            return false;
        }

        public void AssignWeapon(TurretProperties props)
        {
            for (int i = 0; i < m_Turrets.Length; i++)
            {
                m_Turrets[i].AssignLoadout(props);
            }
        }

        public float GetProjectileSpeed()
        {
            if (m_Turrets == null || m_Turrets.Length == 0)
                return m_DefaultProjectileSpeed;

            // ���� ������ primary turret
            foreach (var turret in m_Turrets)
            {
                if (turret != null && turret.Mode == TurretMode.Primary)
                    return turret.GetProjectileSpeed();
            }

            return m_DefaultProjectileSpeed;
        }
    }


