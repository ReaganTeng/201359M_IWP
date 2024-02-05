using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class DialogueManager : MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI dialogueText;
    [HideInInspector]
    public GameObject dialoguePanel;
    [HideInInspector]
    public CanvasGroup dialoguePanelCG;

    [HideInInspector]
    public float typingSpeed = 0.05f;
    List<string> sentences = new List<string>();

    [HideInInspector]
    public AudioSource AS;

    public AudioClip beepClip;

    [HideInInspector]
    public bool dialogueMode;
    bool isTyping = false;

    [HideInInspector]
    public DialogueRealTime currentDialogue;

    [HideInInspector]
    public int currentSentenceidx;

    [HideInInspector]
    public MenuManager MM;

    [HideInInspector]
    public HubWorldMenuManager hubWorldMM;


    Coroutine typeSentenceCoroutine;


    void Awake()
    {
        MM = GetComponent<MenuManager>();
        hubWorldMM = GetComponent<HubWorldMenuManager>();
        currentSentenceidx = 0;
        AS = GetComponent<AudioSource>();
        currentDialogue = null;
    }

    public void StartDialogue(DialogueRealTime dialogue)
    {
        //Debug.Log("Dialogue STARTED");
        if (currentDialogue == null 
            && dialoguePanel != null
            && !dialoguePanel.GetComponent<CanvasGroup>().interactable)
        {
            //Debug.Log("DIALOGUE STARTED");
            Scene currentScene = SceneManager.GetActiveScene();
            switch (currentScene.name)
            {
                case "HubWorld":
                    hubWorldMM.togglePanel(dialoguePanel);
                    break;
                case "GameScene":
                    MM.togglePanel(dialoguePanel);
                    break;
                default:
                    break;
            }

            sentences.Clear();
            currentDialogue = dialogue;

            foreach (DialogueRealTime.Sentence sentence in dialogue.sentences)
            {
                sentences.Add(sentence.text); // Change from Enqueue to Add
            }
            //if (currentSentenceidx + 1 >= sentences.Count
            //         || currentSentenceidx < 0
            //         || sentences[currentSentenceidx] == null)
            //{
            //    return;
            //}

            StartCoroutine(DisplayNextSentence());

        }
    }

    void Update()
    {
        if (dialoguePanel == null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            switch (currentScene.name)
            {
                case "HubWorld":
                    {
                        dialoguePanel = hubWorldMM.dialoguePanel;
                        break;
                    }
                case "GameScene":
                    {
                        dialoguePanel = MM.dialoguePanel;
                        break;
                    }
                default:
                    break;
            }

            dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
            dialogueText.text = "";
            dialoguePanelCG = dialoguePanel.GetComponent<CanvasGroup>();
            return;
        }

        //PRESS ENTER TO SKIP DIALOGUE
        if (Input.GetKeyDown(KeyCode.Return)
            && dialoguePanelCG.interactable
            && dialoguePanelCG.alpha == 1
            && dialoguePanelCG.blocksRaycasts
            )
        {
            
            //IF NO OPTIONS
            if (currentDialogue.sentences[currentSentenceidx].options == null
                || currentDialogue.sentences.Count <= 1)
            {
                return;
            }

            if (currentSentenceidx < currentDialogue.sentences.Count - 1)
            {
                StopCoroutine(typeSentenceCoroutine);
                StopCoroutine(DisplayNextSentence());
                dialogueText.text = "";
                dialogueText.text = sentences[currentSentenceidx];
                currentSentenceidx++;
                if (currentSentenceidx + 1 >= sentences.Count
                     || currentSentenceidx < 0
                     || sentences[currentSentenceidx] == null)
                {
                    return;
                }
                StartCoroutine(DisplayNextSentence());

            }
            else
            {
                //Debug.Log("CLOSE DIALOGUE");
                CloseDialogue();
            }
        }

        //SPEED UP ANIMATION
        if ((Input.GetMouseButtonDown(0)
            ||Input.GetKeyDown(KeyCode.E))
            && dialoguePanelCG.interactable
            && dialoguePanelCG.alpha == 1
            && dialoguePanelCG.blocksRaycasts)
        {

            Button buttonComponent = dialoguePanel.GetComponentInChildren<Button>();

            // if there is choice button, then we don't want to process game logic. user must click the option to continue
            if (buttonComponent != null)
            {
                return;   
            }

            if (!isTyping)
            {
                if (currentSentenceidx < currentDialogue.sentences.Count - 1)
                {
                    StopCoroutine(typeSentenceCoroutine);
                    StopCoroutine(DisplayNextSentence());
                    dialogueText.text = "";
                    dialogueText.text = sentences[currentSentenceidx];
                    currentSentenceidx++;
                    if (currentSentenceidx + 1 >= sentences.Count
                      || currentSentenceidx < 0
                      || sentences[currentSentenceidx] == null)
                    {
                        return;

                    }
                    StartCoroutine(DisplayNextSentence());

                }
                else
                {
                    //Debug.Log("CLOSE DIALOGUE");
                    CloseDialogue();
                }
            }
            else
            {
                typingSpeed = -1.0f;
            }
        }
    }

   

    //DISPLAY OPTION BUTTONS
    void DisplayOptions()
    {
        // Get the current sentence
        DialogueRealTime.Sentence currentSentence = currentDialogue.sentences[currentSentenceidx];

        // Create buttons for each option
        for (int i = 0; i < currentSentence.options.Count; i++)
        {
            //Debug.Log("OPTION");
            DialogueRealTime.DialogueOption option = currentSentence.options[i];
            // Create a new button
            GameObject buttonGO = new GameObject("OptionButton" + i);
            buttonGO.transform.SetParent(dialoguePanel.transform);
            
            // Add a TextMeshProUGUI component to the button
            TextMeshProUGUI buttonText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            buttonText.transform.SetParent(buttonGO.transform);
            buttonText.text = option.optionText;
            buttonText.alignment = TextAlignmentOptions.Center;
            // Add a Button component to the button
            Button button = buttonGO.AddComponent<Button>();
            Image ImageButton = buttonGO.AddComponent<Image>();
            button.targetGraphic = ImageButton;
            button.interactable = true;
            // Customize the appearance of the button
            CustomizeButton(button);
           
            button.onClick.AddListener(() => 
            OnOptionSelected(option));
            // Position the button
            RectTransform rectTransform = buttonGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(-i * 250, dialoguePanel.transform.position.y - 200); // Negative to arrange from top to bottom
            // Set the button's size
            rectTransform.sizeDelta = new Vector2(100, 50); // Adjust size as needed
        }
    }

    void CustomizeButton(Button button)
    {
        // Customize the appearance of the button
        ColorBlock colors = button.colors;
        // Set normal color
        colors.normalColor = new Color(1.0f, .0f, .0f);
        // Set highlighted color
        colors.highlightedColor = new Color(.0f, 1.0f, 0.0f);
        // Set pressed color
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        // Set disabled color
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f);
        // Set button colors
        button.colors = colors;
        // Add a border by adjusting the transition
        ColorBlock highlightColors = button.colors;
        highlightColors.colorMultiplier = 10.0f; // Adjust the multiplier to control border intensity
        button.colors = highlightColors;
    }



    void ClearButtons()
    {
        // Destroy all existing buttons
        foreach (Transform child in dialoguePanel.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void OnOptionSelected(DialogueRealTime.DialogueOption option)
    {
        // Handle the selected option
        //Debug.Log($"Selected option: {option.optionText}");
        // Perform any action associated with the selected option
        if (option.onOptionSelected != null)
        {
            option.onOptionSelected.Invoke();
        }

        StopCoroutine(typeSentenceCoroutine);
        StopCoroutine(DisplayNextSentence());
        dialogueText.text = "";
        dialogueText.text = sentences[currentSentenceidx];
        currentSentenceidx++;

        if (currentSentenceidx + 1 >= sentences.Count
            || currentSentenceidx < 0
            || sentences[currentSentenceidx] == null)
        {

            //Debug.Log("RETURN NULL");
            return;
            // Move to the next sentence or close the dialogue
        }
        StartCoroutine(DisplayNextSentence());

    }

    public void CloseDialogue()
    {
        StopCoroutine(typeSentenceCoroutine);
        StopCoroutine(DisplayNextSentence());

        currentSentenceidx = 0;
        Scene currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "HubWorld":
                {
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
                    //Debug.Log("CLOSE FROM HUB WORLD");
                    break;
                }
            case "GameScene":
                {
                    MM.dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
                    MM.dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    MM.dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
                    //Debug.Log("CLOSE FROM GAME WORLD");
                    break;
                }
            default:
                break;
        }

        //AS.clip = null;
        //Debug.Log("SET TO NULL");
        currentDialogue = null; // Reset currentDialogue
    }


    IEnumerator DisplayNextSentence()
    {
        dialogueText.text = "";
        typingSpeed = 0.05f;
        // Clear existing buttons
        ClearButtons();
        //Debug.Log($"SENTENCE IS {sentences.Dequeue()}");

        

        if (sentences.Count > 0)
        {
            string sentence = $"{sentences[currentSentenceidx]}";
            Debug.Log($"SENTENCE IS {sentence}");
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentenceOrOption(sentence));
            //yield return TypeSentenceOrOption(sentence);
            yield return typeSentenceCoroutine;
        }
        else if (currentDialogue != null
            && currentDialogue.sentences != null
            && currentDialogue.sentences.Count > 0)
        {
            DisplayOptions();
        }
        else
        {
            //Debug.Log("CLOSE");
            CloseDialogue();
        }
    }


    IEnumerator TypeSentenceOrOption(string text)
    {
        //dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            //Debug.Log($"LETTER IS {text}");
            AS.clip = beepClip;
            AS.Play();
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        //if(currentDialogue.sentences.op)
        DisplayOptions();
        isTyping = false;
    }
}
