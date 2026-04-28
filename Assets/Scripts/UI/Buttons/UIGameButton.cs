using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIGameButton : UISelectableButton,IScriptableObjectProperty
{
    [SerializeField] private LevelProperties m_LevelProperties;

    [SerializeField] private Image icon;
    [SerializeField] private Text title;

    private void Start()
    {
        ApplyProperty(m_LevelProperties);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (m_LevelProperties == null) return;

        SceneManager.LoadScene(m_LevelProperties.SceneName);
    }

    public void ApplyProperty(ScriptableObject property)
    {
        if (property == null) return;           

        if (property is LevelProperties == false) return;
        m_LevelProperties = property as LevelProperties;

        icon.sprite = m_LevelProperties.PreviewImage;
        title.text = m_LevelProperties.Title;
    }

}
