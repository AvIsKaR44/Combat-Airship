using UnityEngine;



public class UISelectableButtonContainer : MonoBehaviour
{
    [SerializeField] private Transform buttonsContainer;

    public bool Interasctable = true;
    public void SetInterasctable(bool interasctable) => Interasctable = interasctable;

    private UISelectableButton[] buttons;

    private int selectButtonIndex = 0;

    private void Start()
    {
        buttons = buttonsContainer.GetComponentsInChildren<UISelectableButton>();

        if (buttons == null)
            Debug.LogError("Button list is empty!");

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].PointerEnter += OnPointerEnter;
        }

        if (Interasctable ==  false) return;

        buttons[selectButtonIndex].SetFocuse();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].PointerEnter -= OnPointerEnter;
        }
    }

    private void OnPointerEnter(UIButton button)
    {
        SelectButton(button);
    }

    private void SelectButton(UIButton button)
    {
        if (Interasctable == false) return;

        buttons[selectButtonIndex].SetUnFocuse();

        for (int i = 0; i < buttons.Length; i++)
        { 
            if(button == buttons[i])
            {
                selectButtonIndex = i;
                button.SetFocuse();
                break;
            }
        }
    }

    public void SelectNext() {}

    public void SelectPrecious() {}        
}
