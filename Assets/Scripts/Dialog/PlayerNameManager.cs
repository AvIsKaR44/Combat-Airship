using UnityEngine;
using TMPro;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    public void SavePlayerName()
    {
        if (!string.IsNullOrWhiteSpace(nameInputField.text))
        {
            PlayerPrefs.SetString("PilotName", nameInputField.text);
            PlayerPrefs.Save();
        }
    }
}
