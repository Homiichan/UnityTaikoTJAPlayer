using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    // Start is called before the first frame update
    public Taiko_Notes CurrentNoteType = Taiko_Notes.Blank;
    public NoteData CurrentNoteData;
    Renderer noterender;
    float SecondsToTravel = 0;
    Vector3 EndPosition;

    void Start()
    {
        
        
    }

    public void OnSpawn(float secondsToTravel, Vector3 targetEndPoint, Taiko_Notes targetNoteType)
    {
        SecondsToTravel = secondsToTravel;
        CurrentNoteType = targetNoteType;
        CurrentNoteData.NoteType = targetNoteType;
        CurrentNoteData.NoteDurationMS = secondsToTravel;
        Debug.Log(CurrentNoteType);
        SetNoteColorAndSize();
        EndPosition = targetEndPoint;
        StartCoroutine(MoveToPosition(this.transform, EndPosition, SecondsToTravel));
    }
    void SetNoteColorAndSize()
    {
        switch(CurrentNoteType)
        {
            case Taiko_Notes.Blank:
                Destroy(this.gameObject);
                break;
            case Taiko_Notes.Don:
                SetColor(Color.red);
                break;
            case Taiko_Notes.Ka:
                SetColor(Color.cyan);
                break;
            case Taiko_Notes.BigDon:
                SetColor(Color.red);
                this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case Taiko_Notes.BigKa:
                SetColor(Color.cyan);
                this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case Taiko_Notes.DrumRoll:
                SetColor(Color.yellow);
                break;
            case Taiko_Notes.startBalloon:
                SetColor(Color.yellow);
                break;
            case Taiko_Notes.endBalloon:
                SetColor(Color.yellow);
                break;
            case Taiko_Notes.Kusudama:
                SetColor(Color.yellow);
                break;
            case Taiko_Notes.endKusudama:
                SetColor(Color.yellow);
                break;
            case Taiko_Notes.BigDrumRoll:
                SetColor(Color.yellow);
                this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
        }
    }

    void SetColor(Color ColorToSet)
    {
        noterender = this.GetComponent<Renderer>();
        Material mat = GetComponent<Renderer>().material;
        mat.SetColor("_Color", ColorToSet);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
            
        }
        Destroy(this.transform.parent.gameObject);
    }
}
