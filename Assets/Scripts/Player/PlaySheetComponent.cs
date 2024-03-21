using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlaySheetComponent : MonoBehaviour
{
    int nextMelody = 0;
    int nextRhythm = 0;

    bool isPlayingSheet = false;
    int currentNote = 0;
    readonly int noteCount = 8;

    [Header("Note Sprite")]
    public SheetDataSO[] Sheets;
    [SerializeField] Transform arrowArea;
    [SerializeField] Sprite baseNoteSprite;
    [SerializeField] Sprite targetNoteSprite;
    [SerializeField] Sprite failureNoteSprite;

    [Header("Arrow Sprite")]
    [SerializeField] Transform panelSheet;
    [SerializeField] Sprite rhythmArrow;
    [SerializeField] Sprite emptyArrow;

    [Header("Success/Failure Sprite")]
    [SerializeField] Image SF_Image;
    [SerializeField] Sprite successImage;
    [SerializeField] Sprite failureImage;

    private SheetDataSO currentSheet;

    Queue<Note> notesQ = new Queue<Note>();

    void SetSheet() {
        isPlayingSheet = true;
        panelSheet.gameObject.SetActive(true);

        Image[] noteImages;
        noteImages = panelSheet.GetComponentsInChildren<Image>();

        int sheetNumber = Random.Range(0, Sheets.Count());
        currentSheet = Sheets[sheetNumber];

        foreach (Image image in noteImages) {
            image.sprite = baseNoteSprite;
        }

        for (int i = 0; i < currentSheet.sheet.notes.Count; i++) {
            Note tempNote = currentSheet.sheet.notes[i];
            notesQ.Enqueue(tempNote);
            noteImages[tempNote.rhythm].sprite = targetNoteSprite;
            nextMelody = notesQ.Peek().melody;
            nextRhythm = notesQ.Peek().rhythm;
        }
    }

    void FinishSheet() {
        panelSheet.gameObject.SetActive(false);
        SF_Image.sprite = emptyArrow;
        isPlayingSheet = false;
    }

    public void StartPlayingSheet() {
        StartCoroutine(RunningSheet());
    }

    IEnumerator RunningSheet() {
        
        SetSheet();

        Image[] arrowImages;
        arrowImages = arrowArea.GetComponentsInChildren<Image>();

        for (int i = 0; i < noteCount; i++) {
            currentNote = i;

            if (currentNote > nextRhythm) {
                if (notesQ.Count > 0) {
                    Note tempNote = notesQ.Dequeue();
                    nextMelody = tempNote.melody;
                    nextRhythm = tempNote.rhythm;        
                }
            }

            arrowImages[i].sprite = rhythmArrow;

            yield return new WaitForSeconds(currentSheet.playSpeed);

            arrowImages[i].sprite = emptyArrow;
        }

        FinishSheet();
    }

    public void ContinuePlayingSheet() {
        if (nextRhythm == currentNote) {
            SF_Image.sprite = successImage;
        }else {
            SF_Image.sprite = failureImage;
        }
    }

    public bool IsPlayingSheet() {
        return isPlayingSheet;
    }
}
