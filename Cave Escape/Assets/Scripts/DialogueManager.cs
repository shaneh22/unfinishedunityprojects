using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    public AudioClip voice;
    public Text dialogueText;

    public Animator animator;
    void Awake()
    {
        sentences = new Queue<string>();
    }
    public void StartDialogue(Dialogue dialogue)
    {
        GameManager.instance.doingSetup = true;
        animator.SetBool("IsOpen",true);
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
        //StartCoroutine(NextSentence());
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            if (letter != ' ') SoundManager.instance.RandomizeSfx(voice);
            dialogueText.text += letter;
            if (dialogueText.text.Equals(sentence))
            {
                StartCoroutine(NextSentence());
            }
            yield return null;
        }
    }
    public IEnumerator NextSentence()
    {
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                DisplayNextSentence();
            }
            yield return null;
        }
    }
    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        GameManager.instance.doingSetup = false;
    }
}
