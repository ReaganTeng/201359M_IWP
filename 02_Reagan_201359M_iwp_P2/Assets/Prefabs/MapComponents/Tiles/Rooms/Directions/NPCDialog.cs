using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NPCDialog : MonoBehaviour
{
    public string[] dialogSentences;
    public Text dialogText;
    public float letterDelay = 0.1f;
    public AudioClip beepSound;

    private int currentSentenceIndex = 0;
    private AudioSource audioSource;

    private void Start()
    {
        dialogText.text = "";
        audioSource = GetComponent<AudioSource>();
    }

    public void StartDialog()
    {
        if (currentSentenceIndex < dialogSentences.Length)
        {
            StartCoroutine(DisplaySentence(dialogSentences[currentSentenceIndex]));
            currentSentenceIndex++;
        }
    }

    IEnumerator DisplaySentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence)
        {
            dialogText.text += letter;
            if (beepSound != null)
            {
                audioSource.PlayOneShot(beepSound);
            }
            yield return new WaitForSeconds(letterDelay);
        }
    }
}
