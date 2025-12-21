using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIGameButton : UISelectableButton,IScriptableObjectProperty
{
    [SerializeField] private GameInfo gameInfo;

    [SerializeField] private Image icon;
    [SerializeField] private Text title;

    private void Start()
    {
        ApplyProperty(gameInfo);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (gameInfo == null) return;

        SceneManager.LoadScene(gameInfo.SceneName);
    }

    public void ApplyProperty(ScriptableObject property)
    {
        if (property == null) return;           

        if (property is GameInfo == false) return;
        gameInfo = property as GameInfo;

        icon.sprite = gameInfo.Icon;
        title.text = gameInfo.Title;
    }

}
