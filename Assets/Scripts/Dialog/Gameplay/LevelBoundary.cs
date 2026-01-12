using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Dialogue nextAreaDialogue;
    [SerializeField] private bool disableOnTrigger = true;
    [SerializeField] private bool showDebug = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (showDebug) Debug.Log($"[LevelBoundary] Player entered boundary: {gameObject.name}");

        // Запускаем диалог для следующей области
        if (nextAreaDialogue != null)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(nextAreaDialogue);
            }
            else if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.StartDialogue(nextAreaDialogue);
            }
        }

        // Деактивируем коллайдер если нужно
        if (disableOnTrigger)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
        }
    }

    public void ResetBoundary()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showDebug) return;

        Collider collider = GetComponent<Collider>();
        if (collider == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f);

        if (collider is BoxCollider boxCollider)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
        else if (collider is SphereCollider sphereCollider)
        {
            Vector3 scale = transform.lossyScale;
            float maxScale = Mathf.Max(scale.x, scale.y, scale.z);
            Gizmos.DrawSphere(transform.position + sphereCollider.center, sphereCollider.radius * maxScale);
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        UnityEditor.Handles.Label(transform.position, $"Level Boundary\nNext Dialogue: {nextAreaDialogue?.name ?? "None"}", style);
    }
#endif
}