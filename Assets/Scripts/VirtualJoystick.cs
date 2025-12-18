using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image m_JoyBack;
    [SerializeField] private Image m_JoyStick;
        
    public Vector3 Value { get; private set; }
        
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_JoyBack.rectTransform, 
            eventData.position, eventData.pressEventCamera, out position);

        position.x = (position.x / m_JoyBack.rectTransform.sizeDelta.x);
        position.y = (position.y / m_JoyBack.rectTransform.sizeDelta.y);

        position.x = position.x * 2 - 1;
        position.y = position.y * 2 - 1;

        Value = new Vector3(position.x, position.y, 0);

        if (Value.magnitude > 1)
            Value = Value.normalized;

        float offsetX = m_JoyBack.rectTransform.sizeDelta.x / 2 - m_JoyStick.rectTransform.sizeDelta.x / 2;
        float offsetY = m_JoyBack.rectTransform.sizeDelta.y / 2 - m_JoyStick.rectTransform.sizeDelta.y / 2;

        m_JoyStick.rectTransform.anchoredPosition = new Vector2(Value.x * offsetX, Value.y * offsetY);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector3.zero;
        m_JoyStick.rectTransform.anchoredPosition = Vector3.zero;
    }
}


