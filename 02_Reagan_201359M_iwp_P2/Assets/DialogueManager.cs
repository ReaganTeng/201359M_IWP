using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;
    Queue<string> sentences;
    public AudioSource AS;
    public AudioClip beepCLip;

    void Awake()
    {
        sentences = new Queue<string>();

        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        StartCoroutine(DisplayNextSentence());
    }

    IEnumerator DisplayNextSentence()
    {
        dialoguePanel.SetActive(true);
        while (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            yield return TypeSentence(sentence);
        }
        dialoguePanel.SetActive(false);
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            AS.clip = beepCLip;
            AS.Play();
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
