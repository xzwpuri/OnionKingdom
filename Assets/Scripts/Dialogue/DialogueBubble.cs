using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject textPanel;
    [SerializeField] GameObject choicePanel;
    [SerializeField] Transform choiceContainer;
    [SerializeField] Transform boggleAnchor;
    [SerializeField] GameObject bogglePanel;
    [SerializeField] Button closeBoggleButton;
    [SerializeField] ChoiceButton choiceButtonPrefab;
    [SerializeField] BoggleDirector boggleDirectorPrefab;

    [Header("Typing Effect")]
    [SerializeField] float typingSpeed = 0.05f;

    public Action OnDialogueEnd;

    DialogueData currentData;
    int currentPageIndex = 0;
    bool isTyping = false;
    string fullText = "";
    Coroutine typingCoroutine;
    BoggleDirector currentBoggle = null;
    Action<BoggleDirector> onBoggleCreated;

    private void Awake()
    {
        gameObject.SetActive(false);
        if (closeBoggleButton != null)
            closeBoggleButton.onClick.AddListener(CloseBoggle);
    }

    public void StartDialogue(DialogueData data, BoggleDirector savedBoggle, Action<BoggleDirector> onBoggleCreated = null)
    {
        currentData = data;
        currentPageIndex = 0;
        currentBoggle = savedBoggle;
        this.onBoggleCreated = onBoggleCreated;
        gameObject.SetActive(true);
        StartCoroutine(AnimateOpen());
        ShowPage(currentPageIndex);
    }

    public void EndDialogue()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(AnimateClose(() => OnDialogueEnd?.Invoke()));
    }

    private void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= currentData.pages.Length)
        {
            EndDialogue();
            return;
        }

        currentPageIndex = pageIndex;
        DialoguePage page = currentData.pages[pageIndex];

        textPanel.SetActive(false);
        choicePanel.SetActive(false);
        bogglePanel.SetActive(false);

        switch (page.pageType)
        {
            case PageType.Text:
                ShowTextPage(page);
                break;
            case PageType.Choice:
                ShowChoicePage(page);
                break;
            case PageType.Boggle:
                ShowBogglePage(page);
                break;
        }
    }

    private void ShowTextPage(DialoguePage page)
    {
        textPanel.SetActive(true);
        fullText = page.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(fullText));

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (isTyping)
                SkipTyping();
            else
                ShowPage(page.nextPageIndex);
        });
    }

    private void ShowChoicePage(DialoguePage page)
    {
        choicePanel.SetActive(true);

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var choice in page.choices)
        {
            ChoiceButton btn = Instantiate(choiceButtonPrefab, choiceContainer);
            int nextIndex = choice.nextPageIndex;
            btn.Setup(choice.choiceText, () => ShowPage(nextIndex));
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void ShowBogglePage(DialoguePage page)
    {
        bogglePanel.SetActive(true);
        boggleAnchor.gameObject.SetActive(true);

        if (currentBoggle == null)
        {
            currentBoggle = Instantiate(boggleDirectorPrefab, boggleAnchor);

            RectTransform boggleRect = currentBoggle.GetComponent<RectTransform>();
            boggleRect.anchoredPosition = Vector2.zero;
            boggleRect.anchorMin = new Vector2(0.5f, 0.5f);
            boggleRect.anchorMax = new Vector2(0.5f, 0.5f);
            boggleRect.sizeDelta = new Vector2(1002, 672);

            RectTransform panelRect = bogglePanel.GetComponent<RectTransform>();
            float scaleX = panelRect.rect.width / 1002f;
            float scaleY = panelRect.rect.height / 672f;
            float scale = Mathf.Min(scaleX, scaleY);
            boggleRect.localScale = new Vector3(scale, scale, 1f);

            currentBoggle.Init(page.boggleData.GetBoard(), page.boggleData.answers);
            currentBoggle.OnAllWordsAcquired += () => ShowPage(currentPageIndex + 1);

            // NPC에 보글 인스턴스 저장
            onBoggleCreated?.Invoke(currentBoggle);
        }
        else
        {
            // 기존 보글 재활용 - 부모만 다시 설정
            currentBoggle.transform.SetParent(boggleAnchor);
            currentBoggle.gameObject.SetActive(true);
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void CloseBoggle()
    {
        bogglePanel.SetActive(false);
        ShowPage(currentPageIndex + 1);
    }

    private IEnumerator TypeText(string text, Action onComplete = null)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        isTyping = false;
        dialogueText.text = fullText;
    }

    private IEnumerator AnimateOpen()
    {
        transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            float scale = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateClose(Action onComplete)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float scale = Mathf.SmoothStep(1f, 0f, t);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}