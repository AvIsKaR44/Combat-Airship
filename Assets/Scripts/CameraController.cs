using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_InterpolationLinear = 5f;
    [SerializeField] private Vector3 m_Offset = new Vector3(0, 0, -10); // Смещение камеры

    private void FixedUpdate()
    {
        if (m_Target == null) return;

        Vector3 targetPosition = m_Target.position + m_Offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_InterpolationLinear * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        m_Target = newTarget;
    }
}