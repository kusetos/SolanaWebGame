using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
 
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public Button button;
 
    private Queue<DialogueLine> lines;
    
    public bool isDialogueActive = false;
 
    public float typingSpeed = 0.2f;

    [Header("Panel Settings")]
    [SerializeField] private RectTransform panel;  
    [SerializeField] private float moveDistance = 800f; // How far it moves down (in pixels)
    [SerializeField] private float duration = 0.5f;   // Animation duration
    private Vector2 _originalPosition;
    private bool _isVisible = false;
    private Tween _currentTween;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
        button.onClick.AddListener(() => DisplayNextDialogueLine());
    }
    public void ForceStopDialogue()
    {
        Debug.Log("Dialogue force-stopped");

        StopAllCoroutines();

        // Clear all remaining dialogue lines
        lines.Clear();

        // Hide UI immediately (skip tween)
        _currentTween?.Kill();
        panel.anchoredPosition = _originalPosition - new Vector2(0, moveDistance);
        _isVisible = false;

        // Reset states
        isDialogueActive = false;
        // Optionally clear UI text
        characterName.text = "";
        dialogueArea.text = "";
    }

    public void StartDialogue(Dialogue dialogue)
    {
        CharacterStateManager.Instance.SetCharacterState(CharacterState.Wait);

        isDialogueActive = true;

        Show();
 
        lines.Clear();
 
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
        DisplayNextDialogueLine();

    }
     public void Show()
    {
        if (_isVisible) return;
        _isVisible = true;

        _currentTween?.Kill();

        panel.anchoredPosition = _originalPosition - new Vector2(0, moveDistance);
        _currentTween = panel.DOAnchorPos(_originalPosition, duration)
            .SetEase(Ease.InOutFlash);
    }

    public void Hide()
    {
        if (!_isVisible) return;
        _isVisible = false;

        _currentTween?.Kill();

        _currentTween = panel.DOAnchorPos(_originalPosition - new Vector2(0, moveDistance), duration)
            .SetEase(Ease.InOutFlash);
        CharacterStateManager.Instance.SetCharacterState(CharacterState.Chais);
        
    }

    public void Toggle()
    {
        if (_isVisible) Hide();
        else Show();
    }
    public void DisplayNextDialogueLine()
    {
        Debug.Log("Next dialogue Line");
        if (lines.Count == 0)
        {
            Hide();
            return;
        }
 
        DialogueLine currentLine = lines.Dequeue();
 
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;
 
        StopAllCoroutines();
 
        StartCoroutine(TypeSentence(currentLine));
    }
 
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            SoundManager.Instance.Play(dialogueLine.soundName);
            yield return new WaitForSeconds(typingSpeed);
        }
    }
 
}
 