using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup dialoguePanel;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject skipPrompt;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float autoAdvanceDelay = 3f;

    [Header("Speaker Portraits")]
    [SerializeField] private Sprite dispatcherPortrait;
    [SerializeField] private Sprite enemyBossPortrait;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private Sprite defaultPortrait;

    [Header("Name Colors")]
    [SerializeField] private Color dispatcherColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color enemyBossColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color playerColor = new Color(0.8f, 1f, 0.4f);

    // Синглтон
    public static DialogueUI Instance { get; private set; }

    // Приватные поля
    private Dialogue currentDialogue;
    private int currentLineIndex;
    private bool isTyping = false;
    private bool isActive = false;
    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeUI();
    }

    void InitializeUI()
    {
        dialoguePanel.alpha = 0;
        dialoguePanel.blocksRaycasts = false;
        dialoguePanel.interactable = false;

        continueButton.onClick.AddListener(AdvanceDialogue);

        if (skipPrompt != null)
            skipPrompt.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKey(KeyCode.Space))
        {
            ShowSkipPrompt(true);

            if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
            {
                SkipToEnd();
            }
        }
        else
        {
            ShowSkipPrompt(false);
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !isTyping)
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning("Attempted to start empty dialogue!");
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isActive = true;

        Time.timeScale = 0f;
        ShowDialoguePanel(true);

        DisplayLine(currentDialogue.lines[currentLineIndex]);
        GameEvents.InvokeDialogueStarted(dialogue);
    }

    private void DisplayLine(Dialogue.DialogueLine line)
    {
        if (line == null) return;

        SetSpeakerInfo(line.speaker);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line.text));

        if (line.voiceLine != null)
            PlayVoiceLine(line.voiceLine);

        if (autoAdvanceCoroutine != null)
            StopCoroutine(autoAdvanceCoroutine);

        autoAdvanceCoroutine = StartCoroutine(AutoAdvance());
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char character in text.ToCharArray())
        {
            dialogueText.text += character;

            if (character == '.' || character == '!' || character == '?')
                yield return new WaitForSecondsRealtime(textSpeed * 5);
            else if (character == ',')
                yield return new WaitForSecondsRealtime(textSpeed * 3);
            else
                yield return new WaitForSecondsRealtime(textSpeed);

            if (!isTyping) break;
        }

        isTyping = false;
    }

    private IEnumerator AutoAdvance()
    {
        yield return new WaitForSecondsRealtime(autoAdvanceDelay);

        if (isTyping)
        {
            while (isTyping)
                yield return null;
        }

        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            isTyping = false;
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            DisplayLine(currentDialogue.lines[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void SkipToEnd()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentDialogue.lines[currentLineIndex].text;
        isTyping = false;
        AdvanceDialogue();
    }

    private void SetSpeakerInfo(Dialogue.Speaker speaker)
    {
        switch (speaker)
        {
            case Dialogue.Speaker.Dispatcher:
                speakerNameText.text = "Леди Алисия Вэнс";
                speakerNameText.color = dispatcherColor;
                speakerPortrait.sprite = dispatcherPortrait ?? defaultPortrait;
                break;

            case Dialogue.Speaker.EnemyBoss:
                speakerNameText.text = "Лорд Морток";
                speakerNameText.color = enemyBossColor;
                speakerPortrait.sprite = enemyBossPortrait ?? defaultPortrait;
                break;

            case Dialogue.Speaker.Player:
                speakerNameText.text = "Капитан Рейдер";
                speakerNameText.color = playerColor;
                speakerPortrait.sprite = playerPortrait ?? defaultPortrait;
                break;
        }

        StartCoroutine(PortraitTransition());
    }

    private IEnumerator PortraitTransition()
    {
        Color originalColor = speakerPortrait.color;
        Color transparent = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        speakerPortrait.color = transparent;

        float elapsed = 0;
        while (elapsed < fadeDuration / 2)
        {
            elapsed += Time.unscaledDeltaTime;
            speakerPortrait.color = Color.Lerp(transparent, originalColor, elapsed / (fadeDuration / 2));
            yield return null;
        }
    }

    private void PlayVoiceLine(AudioClip clip)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 0.8f;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
        Destroy(tempAudio, clip.length + 0.1f);
    }

    private void ShowDialoguePanel(bool show)
    {
        StartCoroutine(FadePanel(show));
    }

    private IEnumerator FadePanel(bool show)
    {
        float targetAlpha = show ? 1 : 0;
        float startAlpha = dialoguePanel.alpha;
        float elapsed = 0;

        dialoguePanel.blocksRaycasts = show;
        dialoguePanel.interactable = show;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            dialoguePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        dialoguePanel.alpha = targetAlpha;
    }

    private void ShowSkipPrompt(bool show)
    {
        if (skipPrompt != null)
            skipPrompt.SetActive(show && !isTyping);
    }

    private void EndDialogue()
    {
        ShowDialoguePanel(false);
        isActive = false;
        Time.timeScale = 1f;
        GameEvents.InvokeDialogueCompleted();

        if (currentDialogue.nextDialogue != null)
            StartCoroutine(StartNextDialogueAfterDelay(0.5f));
    }

    private IEnumerator StartNextDialogueAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        StartDialogue(currentDialogue.nextDialogue);
    }

    public bool IsDialogueActive()
    {
        return isActive;
    }
}