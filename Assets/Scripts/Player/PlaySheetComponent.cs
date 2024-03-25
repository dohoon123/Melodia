using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlaySheetComponent : MonoBehaviour
{
    public int completeSheetNumber = 0;

    int correctMelody = 0;
    int correctRhythm = 0;

    bool isPlayingSheet = false;
    bool isContinuePlayingSheet = true;
    int currentNote = 0;
    readonly int noteCount = 8;

    int[] completeSheetArray = {0, 0, 0};

    [Header("Complete/Empty Sheet Sprite")]
    [SerializeField] Transform completeSheetPanel;
    [SerializeField] Sprite completeSheetSprite;
    [SerializeField] Sprite emptySheetSprite;

    [Header("Note Sprite")]
    public SheetDataSO[] Sheets;
    [SerializeField] Transform sheetPanel;
    [SerializeField] Sprite baseNoteSprite;
    [SerializeField] Sprite targetNoteSprite;
    [SerializeField] Sprite failureNoteSprite;

    [Header("Arrow Sprite")]
    [SerializeField] Transform arrowPanel;
    [SerializeField] Sprite rhythmArrow;
    [SerializeField] Sprite emptyArrow;

    [Header("Success/Failure Sprite")]
    [SerializeField] Image SF_Image;
    [SerializeField] Sprite successImage;
    [SerializeField] Sprite failureImage;

    private SheetDataSO currentSheet;

    Queue<Note> notesQ = new Queue<Note>();

    //<note index, true/false>
    Stack<int> successCount = new Stack<int>();

    void SetSheet() {
        isPlayingSheet = true;
        sheetPanel.gameObject.SetActive(true);

        Image[] noteImages;
        noteImages = sheetPanel.GetComponentsInChildren<Image>();

        int sheetNumber = UnityEngine.Random.Range(0, Sheets.Count());
        currentSheet = Sheets[sheetNumber];

        foreach (Image image in noteImages) {
            image.sprite = baseNoteSprite;
        }

        for (int i = 0; i < currentSheet.sheet.notes.Count; i++) {
            Note tempNote = currentSheet.sheet.notes[i];
            notesQ.Enqueue(tempNote);
            noteImages[tempNote.rhythm].sprite = targetNoteSprite;
        }

        correctMelody = notesQ.Peek().melody;
        correctRhythm = notesQ.Peek().rhythm;
        notesQ.Dequeue();
    }

    void FinishPlayingSheet() {
        sheetPanel.gameObject.SetActive(false);
        SF_Image.sprite = emptyArrow;
        
        notesQ.Clear();
        successCount.Clear();
        
        isPlayingSheet = false;

        if (isContinuePlayingSheet) { 
            completeSheetNumber++;
            completeSheetArray[completeSheetNumber - 1] = 1;

            UpdateCompleteSheetUI();
        }

        isContinuePlayingSheet = true;
    }

    public void StartPlayingSheet() {
        if (completeSheetNumber < 3) {
            StartCoroutine(RunningSheet());
        }
    }

    IEnumerator RunningSheet() {
        
        SetSheet();

        Image[] arrowImages;
        arrowImages = arrowPanel.GetComponentsInChildren<Image>();

        for (int i = 0; i < noteCount; i++) {
            currentNote = i;
            arrowImages[i].sprite = rhythmArrow;

            yield return new WaitForSeconds(currentSheet.playSpeed);

            if (!isContinuePlayingSheet) { break; }

            if (currentNote == correctRhythm) {
                if (successCount.Count == 0 || successCount.Peek() != currentNote) {
                    SF_Image.sprite = failureImage;
                    isContinuePlayingSheet = false;
                    break;
                }

                if (notesQ.Count > 0) {
                    correctMelody = notesQ.Peek().melody;
                    correctRhythm = notesQ.Peek().rhythm;        
                    notesQ.Dequeue();
                }
            }

            arrowImages[i].sprite = emptyArrow;
        }

        foreach (Image img in arrowImages) {
            img.sprite = emptyArrow;
        }

        yield return new WaitForSeconds(currentSheet.playSpeed);
        FinishPlayingSheet();
    }

    public void ContinuePlayingSheet() {
        if (!isContinuePlayingSheet) {
            return;
        }

        if (correctRhythm == currentNote) {
            SF_Image.sprite = successImage;
            successCount.Push(currentNote);
        }else {
            SF_Image.sprite = failureImage;
            isContinuePlayingSheet = false;
        }
    }

    void UpdateCompleteSheetUI() {
        Image[] CSImages;
        CSImages = completeSheetPanel.GetComponentsInChildren<Image>();
        Debug.Log(CSImages.Count());
        for (int i = 0; i < completeSheetArray.Count(); i++) {
            if (completeSheetArray[i] == 1) {
                CSImages[i].sprite = completeSheetSprite;
            }else {
               CSImages[i].sprite = emptySheetSprite; 
            }
        }
    }

    public bool IsPlayingSheet() {
        return isPlayingSheet;
    }

    public int GetCompleteSheet() {
        return completeSheetNumber;
    }

    public void UseCompleteSheet() {
        if (completeSheetNumber > 0) {
            completeSheetArray[completeSheetNumber - 1] = 0;
            completeSheetNumber--;
        }
        UpdateCompleteSheetUI();
    }
}
