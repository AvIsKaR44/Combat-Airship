using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 m_Offset = new Vector3(0, 0, -10);
    [SerializeField] private float m_FollowSpeed = 8f;

    [Header("Camera Bounds (отдельно от LevelBoundary)")]
    [SerializeField] private Bounds m_CameraBounds; // Свои границы для камеры
    [SerializeField] private bool m_AutoFindBounds = true; // Автопоиск границ фона

    private void Start()
    {
        if (m_AutoFindBounds)
        {
            // Ищем все фоны и объединяем их границы
            SpriteRenderer[] backgrounds = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            if (backgrounds.Length > 0)
            {
                Bounds combined = backgrounds[0].bounds;
                for (int i = 1; i < backgrounds.Length; i++)
                {
                    combined.Encapsulate(backgrounds[i].bounds);
                }
                m_CameraBounds = combined;
                Debug.Log($"Auto bounds: {m_CameraBounds}");
            }
        }
    }

    private void LateUpdate()
    {
        if (m_Target == null) return;

        // Куда хочет двигаться камера
        Vector3 targetPosition = m_Target.position + m_Offset;
        targetPosition.z = transform.position.z;

        // Ограничиваем камеру пределами фона (с учётом размера камеры)
        if (m_CameraBounds.size != Vector3.zero)
        {
            float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float cameraHalfHeight = Camera.main.orthographicSize;

            float minX = m_CameraBounds.min.x + cameraHalfWidth;
            float maxX = m_CameraBounds.max.x - cameraHalfWidth;
            float minY = m_CameraBounds.min.y + cameraHalfHeight;
            float maxY = m_CameraBounds.max.y - cameraHalfHeight;

            // Если фон меньше экрана — центрируем
            if (minX > maxX) minX = maxX = (minX + maxX) / 2;
            if (minY > maxY) minY = maxY = (minY + maxY) / 2;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Плавное движение камеры
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_FollowSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        m_Target = newTarget;
    }

    // Опционально: визуализация границ в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(m_CameraBounds.center, m_CameraBounds.size);
    }
}