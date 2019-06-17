using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NoteMaterials
{
    public Taiko_Notes Notes;
    public Texture2D CloseTexture;
    public Texture2D OpenTexture;
    public Texture2D AngryTexture;
    public NoteMaterials(NoteMaterials Template) { 
        this = Template;
    }
}

public class Note : MonoBehaviour
{
    //Pure Note Variable
    public Taiko_Notes CurrentNoteType = Taiko_Notes.Blank;
    public NoteData CurrentNoteData;
    public float MeasureNumber;
    public float NoteBPM;
    Taiko_NoteState CurrentNoteState;
    bool IsHitted = false;



    // Start is called before the first frame update


    Renderer noterender;
    float SecondsToTravel = 0;
    Vector3 EndPosition;
    public List<NoteMaterials> MaterialsList = new List<NoteMaterials>();
    public NoteMaterials CurrentMaterialList = new NoteMaterials();
    public Renderer ChildRender;

    void Start()
    {

       
    }

    public void OnSpawn(float secondsToTravel, Vector3 targetEndPoint, Taiko_Notes targetNoteType, float BPM)
    {
        SecondsToTravel = secondsToTravel;
        CurrentNoteType = targetNoteType;
        CurrentNoteData.NoteType = targetNoteType;
        CurrentNoteData.NoteDurationMS = secondsToTravel;
        NoteBPM = BPM;
        if (CurrentNoteType == Taiko_Notes.bareline)
        {
            Renderer rend;
            rend = GetComponent<Renderer>();
            rend.enabled = false;
            ChildRender.enabled = true;
        }
        else
        {
            FindCorrectNoteMaterial();
        }
        
        if(CurrentNoteType == Taiko_Notes.Blank)
        {
            Destroy(this.transform.parent.gameObject);
        }
        if(CurrentNoteType == Taiko_Notes.bareline)
        {
            this.GetComponent<Renderer>().enabled = false;
            this.GetComponentInChildren<Renderer>().enabled = true;
        }
        EndPosition = targetEndPoint;
        StartCoroutine(MoveToPosition(this.transform, EndPosition, SecondsToTravel));
        
    }
    void SetNoteColorAndSize()
    {
        SetNoteTexture(CurrentMaterialList.CloseTexture);
        switch (CurrentNoteType)
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
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
            case Taiko_Notes.BigKa:
                SetColor(Color.cyan);
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
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
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
        StartCoroutine(SwitchNoteTexture());

    }


    void FindCorrectNoteMaterial()
    {
        foreach(NoteMaterials Nm in MaterialsList)
        {
            if(Nm.Notes == CurrentNoteType)
            {

                CurrentMaterialList = Nm;
                SetNoteColorAndSize();
                break;
            }
        }
    }
    void SetColor(Color ColorToSet)
    {

        //noterender = this.GetComponent<Renderer>();
        //Material mat = GetComponent<Renderer>().material;
        //mat.SetColor("_Color", ColorToSet);
    }
    // Update is called once per frame
    void Update()
    {
        if(CurrentNoteType != Taiko_Notes.bareline)
            SwitchTextureOnBPM();
    }
    
    void SetNoteTexture(Texture targetTexture)
    {
        noterender = this.GetComponent<Renderer>();
        Material mat = GetComponent<Renderer>().material;
        mat.SetTexture("_NoteTexture", targetTexture);
    }
    public IEnumerator SwitchNoteTexture()
    {
        yield return new WaitForSeconds((60000 / NoteBPM) / 1000);
        //SwitchTextureOnBPM();
    }

    void SwitchTextureOnBPM()
    {
        TaikoGameInstance TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        if(CurrentNoteState != TGI.CurrentNoteState)
        {
            switch (TGI.CurrentNoteState)
            {
                case Taiko_NoteState.Mounth_Open:
                    SetNoteTexture(CurrentMaterialList.OpenTexture);
                    break;
                case Taiko_NoteState.Mouth_Close:
                    SetNoteTexture(CurrentMaterialList.CloseTexture);
                    break;
                case Taiko_NoteState.Mouth_Open_Angry:
                    SetNoteTexture(CurrentMaterialList.AngryTexture);
                    break;
            }
            CurrentNoteState = TGI.CurrentNoteState;
        }
        

    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
            
        }
        Destroy(this.transform.parent.gameObject);
    }
    
}
