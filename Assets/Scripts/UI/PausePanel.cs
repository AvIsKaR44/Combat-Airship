using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Panel;

    private void Start()
    {
        m_Panel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowPause()
    {
        m_Panel.SetActive(true);
        Time.timeScale = 0;
    }

    public void HidePause()
    {
        m_Panel.SetActive(false);
        Time.timeScale = 1;
    }

    public void SwitchPause()
    {
        if (m_Panel.activeSelf)
        {
            HidePause();
        }
        else
        {
            ShowPause();
        }
    }

    public void LoadMainMenu()
    {
        m_Panel.SetActive(false);
        Time.timeScale = 1;

        SceneManager.LoadScene(0);
    }
}