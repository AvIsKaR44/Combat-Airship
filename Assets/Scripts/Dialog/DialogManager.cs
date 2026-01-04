using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text dialogText;
    [SerializeField] private Image speakerIcon;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private KeyCode continueKey = KeyCode.Space;

    [Header("Speaker Icons")]
    [SerializeField] private Sprite dispatcherIcon;
    [SerializeField] private Sprite mortokIcon;
    [SerializeField] private Sprite pilotIcon;

    private Queue<DialogDataSO.DialogLine> dialogQueue = new Queue<DialogDataSO.DialogLine>();
    private DialogDataSO currentDialog;
    private Coroutine currentTextCoroutine;
    private bool isTyping = false;
    private bool waitingForInput = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        dialogPanel.SetActive(false);
    }

    public void StartDialog(DialogDataSO dialog)
    {
        if (dialog == null || dialog.dialogLines.Length == 0)
            return;

        currentDialog = dialog;
        dialogQueue.Clear();

        foreach (var line in dialog.dialogLines)
        {
            dialogQueue.Enqueue(line);
        }

        dialogPanel.SetActive(true);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (dialogQueue.Count == 0)
        {
            EndDialog();
            return;
        }

        var line = dialogQueue.Dequeue();

        // Установка иконки говорящего
        SetSpeakerIcon(line.speakerName);

        // Установка имени говорящего
        speakerNameText.text = FormatSpeakerName(line.speakerName);

        // Отображение текста
        if (currentTextCoroutine != null)
            StopCoroutine(currentTextCoroutine);

        currentTextCoroutine = StartCoroutine(TypeText(line.lineText, line.displayDuration, line.waitForPlayerInput));
    }

    private IEnumerator TypeText(string text, float displayDuration, bool waitForInput)
    {
        isTyping = true;
        waitingForInput = false;
        dialogText.text = "";

        // Постепенный вывод текста
        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        // Ожидание ввода или таймер
        if (waitForInput)
        {
            waitingForInput = true;
            yield return new WaitUntil(() => Input.GetKeyDown(continueKey));
            waitingForInput = false;
            ShowNextLine();
        }
        else if (displayDuration > 0)
        {
            yield return new WaitForSeconds(displayDuration);
            ShowNextLine();
        }
        // Если duration = 0 и нет waitForInput - диалог зависнет
    }

    private void SetSpeakerIcon(string speakerName)
    {
        if (speakerIcon == null) return;

        switch (speakerName.ToLower())
        {
            case "леди алисия":
            case "алисия":
            case "диспетчер":
                speakerIcon.sprite = dispatcherIcon;
                break;

            case "лорд морток":
            case "морток":
            case "захватчик":
                speakerIcon.sprite = mortokIcon;
                break;

            case "пилот":
            case "рейдер":
            case "игрок":
                speakerIcon.sprite = pilotIcon;
                break;

            default:
                speakerIcon.sprite = null;
                break;
        }
    }

    private string FormatSpeakerName(string rawName)
    {
        // Здесь можно добавить замену имени пилота на кастомное
        if (rawName.ToLower().Contains("пилот") || rawName.ToLower().Contains("рейдер"))
        {
            string pilotName = PlayerPrefs.GetString("PilotName", "Капитан Рейдер");
            return pilotName;
        }
        return rawName;
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
        currentDialog = null;

        // Можно добавить событие окончания диалога
        // EventBus.Publish(new DialogEndedEvent());
    }

    private void Update()
    {
        // Пропуск текущей строки по нажатию
        if (Input.GetKeyDown(continueKey) && dialogPanel.activeSelf)
        {
            if (isTyping)
            {
                // Пропустить анимацию набора текста
                StopCoroutine(currentTextCoroutine);
                dialogText.text = currentDialog.dialogLines[currentDialog.dialogLines.Length - dialogQueue.Count - 1].lineText;
                isTyping = false;
            }
            else if (waitingForInput)
            {
                // Продолжить диалог
                ShowNextLine();
            }
        }
    }
}
