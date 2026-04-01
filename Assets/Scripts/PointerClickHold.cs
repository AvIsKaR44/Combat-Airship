using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool m_Hold;

    public bool IsHold => m_Hold;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        m_Hold = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}


