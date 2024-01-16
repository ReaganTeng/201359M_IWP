using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
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
    Queue<string> sentences;

    [HideInInspector]
    public AudioSource AS;

    public AudioClip beepClip;

    [HideInInspector]
    public bool dialogueMode;
    bool isTyping = false;

    [HideInInspector]
    public DialogueRealTime currentDialogue;

    int currentSentenceidx;

    [HideInInspector]
    public MenuManager MM;

    [HideInInspector]
    public HubWorldMenuManager hubWorldMM;

    void Awake()
    {
        MM = GetComponent<MenuManager>();
        hubWorldMM = GetComponent<HubWorldMenuManager>();

        currentSentenceidx = 0;
        sentences = new Queue<string>();
        AS = GetComponent<AudioSource>();
        currentDialogue = null;
    }

    public void StartDialogue(DialogueRealTime dialogue)
    {
        if (currentDialogue == null 
            //&& dialoguePanel != null
            && !dialoguePanel.GetComponent<CanvasGroup>().interactable)
        {
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
                sentences.Enqueue(sentence.text);
            }
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
        }


        if (Input.GetMouseButtonDown(0)
            && dialoguePanelCG.interactable
            && dialoguePanelCG.alpha == 1
            && dialoguePanelCG.blocksRaycasts)
        {

            Button buttonComponent = dialoguePanel.GetComponentInChildren<Button>();

            // if there is choice button, then we don't want to process game logic. user must click the option to continue
            if (buttonComponent != null)
            {
                //GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

                //if (clickedObject != null && clickedObject.GetComponent<Button>() != null)
                //{
                    return;
                //}
            }

            if (!isTyping)
            {
                if (currentSentenceidx < currentDialogue.sentences.Count - 1)
                {
                    currentSentenceidx++;
                    StartCoroutine(DisplayNextSentence());
                }
                else
                {
                    //Debug.Log("CLOSE");
                    CloseDialogue();
                }
            }
            else
            {
                typingSpeed = -1.0f;
            }
        }
    }

    IEnumerator DisplayNextSentence()
    {
        typingSpeed = 0.05f;
        // Clear existing buttons
        ClearButtons();

        if (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            isTyping = true;
            yield return TypeSentenceOrOption(sentence);
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
        Debug.Log($"Selected option: {option.optionText}");
        // Perform any action associated with the selected option
        if (option.onOptionSelected != null)
        {
            option.onOptionSelected.Invoke();
        }
        // Move to the next sentence or close the dialogue
        StartCoroutine(DisplayNextSentence());
    }

    public void CloseDialogue()
    {
        currentSentenceidx = 0;
        Scene currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "HubWorld":
                {
                    //hubWorldMM.togglePanel(dialoguePanel);
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    hubWorldMM.dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
                    //Debug.Log("CLOSE FROM HUB WORLD");
                    break;
                }
            case "GameScene":
                {
                    //MM.togglePanel(dialoguePanel);
                    MM.dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
                    MM.dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    MM.dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
                    //Debug.Log("CLOSE FROM GAME WORLD");
                    break;
                }
            default:
                break;
        }

        currentDialogue = null; // Reset currentDialogue
    }

    IEnumerator TypeSentenceOrOption(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
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
