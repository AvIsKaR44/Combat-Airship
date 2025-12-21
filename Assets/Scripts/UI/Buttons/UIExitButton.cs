using UnityEngine;

public class UIExitButton: MonoBehaviour
{
    [SerializeField] private GameObject exitPanel;
        
    public void ExitGame()
    {
        if (exitPanel != null) 
            exitPanel.SetActive(false);

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }       
}
