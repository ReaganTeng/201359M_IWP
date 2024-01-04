using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
    Dialogue currentDialogue;

    int currentSentenceidx;

    [HideInInspector]
    public MenuManager MM;
    
    void Awake()
    {
        MM = GetComponent<MenuManager>();



        dialoguePanel = MM.dialoguePanel;
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        dialogueText.text = "";
        dialoguePanelCG = dialoguePanel.GetComponent<CanvasGroup>();


        currentSentenceidx = 0;
        sentences = new Queue<string>();
        AS = GetComponent<AudioSource>();
        
        currentDialogue = null;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        MM.togglePanel(dialoguePanel);

        sentences.Clear();
        currentDialogue = dialogue;

        //Character[] charactersInScene = FindObjectsOfType<Character>();
        //foreach (Character character in charactersInScene)
        //{
        //    character.disabled = true;
        //}

        foreach (Dialogue.Sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence.text);
        }
        StartCoroutine(DisplayNextSentence());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && dialoguePanelCG.interactable
            && dialoguePanelCG.alpha == 1
            && dialoguePanelCG.blocksRaycasts)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

                if (clickedObject != null && clickedObject.GetComponent<Button>() != null)
                {
                    // The mouse is over a UI button, so we don't want to process game logic.
                    return;
                }
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
                typingSpeed = 0.0f;
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

    void DisplayOptions()
    {
        // Get the current sentence
        Dialogue.Sentence currentSentence = currentDialogue.sentences[currentSentenceidx];

        // Create buttons for each option
        for (int i = 0; i < currentSentence.options.Count; i++)
        {
            Debug.Log("OPTION");
            Dialogue.DialogueOption option = currentSentence.options[i];
            // Create a new button
            GameObject buttonGO = new GameObject("OptionButton" + i);
            buttonGO.transform.SetParent(dialoguePanel.transform);
            // Add a Canvas component to the button
            //Canvas canvas = buttonGO.AddComponent<Canvas>();
            //canvas.overrideSorting = true;
            //canvas.sortingOrder = 10;
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
            // Assign the click event to the option's action
            //Debug.Log("Adding click listener for option: " + option.optionText);
            button.onClick.AddListener(() => //Debug.Log("LOM"));
            OnOptionSelected(option));
            // Position the button
            RectTransform rectTransform = buttonGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(-i * 200, dialoguePanel.transform.position.y); // Negative to arrange from top to bottom
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

    void OnOptionSelected(Dialogue.DialogueOption option)
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
        MM.togglePanel(dialoguePanel);
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
