using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct NoteMaterials
{
    public Taiko_Notes Notes;
    public Sprite CloseTexture;
    public Sprite OpenTexture;
    public Sprite AngryTexture;
    public NoteMaterials(NoteMaterials Template) { 
        this = Template;
    }
}

public class Note : MonoBehaviour
{
    //Pure Note Variable
    public Taiko_Notes CurrentNoteType = Taiko_Notes.Blank;
    public NoteData CurrentNoteData;
    public Sprite Bareline;
    float MeasureNumber;
    public float NoteBPM;
    Taiko_NoteState CurrentNoteState;
    bool IsHitted = false;
    float SpawnNoteTime;
    public float HitTime;
    public float CorrectHitTime;
    TaikoSongPlayer TGS;
    float startPosY;
    bool Move = false;
    float NoteSpeed = 0;
    float ScrollSpeed = 2;
    public float NoteEndTime;
    float Tick = 0;
    float lastpostion;


    // Start is called before the first frame update


    Renderer noterender;
    float SecondsToTravel = 0;
    Vector3 EndPosition;
    public List<NoteMaterials> MaterialsList = new List<NoteMaterials>();
    public NoteMaterials CurrentMaterialList = new NoteMaterials();
    public Renderer ChildRender;
    float timer;

    void Start()
    {
       
    }

    public void OnSpawn(float secondsToTravel, Vector3 targetEndPoint, Taiko_Notes targetNoteType, float BPM, float NoteTime, float elapsedTime)
    {
        TGS = GameObject.FindObjectOfType<TaikoSongPlayer>();
        SecondsToTravel = secondsToTravel;
        CurrentNoteType = targetNoteType;
        CurrentNoteData.NoteType = targetNoteType;
        CurrentNoteData.NoteDurationMS = secondsToTravel;
        NoteBPM = BPM;
        startPosY = transform.position.x;
        if (CurrentNoteType == Taiko_Notes.bareline)
        {
            GetComponent<Image>().sprite = Bareline;
            GetComponent<Image>().SetNativeSize();
        }
        else
        {
            FindCorrectNoteMaterial();
        }
        
        if (CurrentNoteType == Taiko_Notes.Blank)
        {
            DestroyNote(false);
        }
        EndPosition = targetEndPoint;
        float EndDistance = Vector3.Distance(transform.position, EndPosition);
        float HitDistance = Vector3.Distance(transform.position, TGS.HitPoint.transform.position);
        SpawnNoteTime = NoteTime;
        NoteSpeed = EndDistance / (60000 / NoteBPM);
        //Debug.Log(EndDistance + "   " + NoteSpeed);
        NoteEndTime = EndDistance / (NoteSpeed / Time.deltaTime);
        //
        //Debug.Log(NoteEndTime);
        NoteEndTime = (NoteEndTime * (1 / ScrollSpeed)) + SpawnNoteTime;
        HitTime = HitDistance / (NoteSpeed / Time.deltaTime);
        HitTime = (HitTime * (1 / ScrollSpeed)) + SpawnNoteTime;
        //NoteEndTime = NoteTime + (2.12f * 2);
        //Debug.Log("Note Speed = " + NoteSpeed + "PredictedTime = " + NoteEndTime);
        Move = true;
        PredictNoteEndTime();
        //Debug.Log(Time.deltaTime);

    }

    void PredictNoteEndTime()
    {

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
        Tick += Time.deltaTime;
        //TMP.text = timer.ToString();
        //timer -= Time.deltaTime;
        
        if (Move)
        {
            transform.position -= new Vector3((NoteSpeed * ScrollSpeed), 0,0);
            //transform.position -= new Vector3(NoteBPM / 60, 0, 0);
           
             
            if (TGS.tick >= NoteEndTime)
            {
                DestroyNote(false);
                //Debug.Log("destroy");
            }


        }
    }
    
    void SetNoteTexture(Sprite targetTexture)
    {
        GetComponent<Image>().sprite = targetTexture;
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
        if(!IsHitted)
            DestroyNote(false);

    }

    public void DestroyNote(bool Hiited)
    {
        TGS = GameObject.FindObjectOfType<TaikoSongPlayer>();
        if (CurrentNoteType != Taiko_Notes.bareline && CurrentNoteType != Taiko_Notes.Blank)
        {
            //Debug.Log("Note Index = " + TGS.CurrentNote.IndexOf(this.transform.parent.gameObject));
            if(TGS.CurrentNote.IndexOf(this.transform.gameObject) != -1)
            {
                int NoteIndex = TGS.CurrentNote.IndexOf(this.gameObject);
                if (Hiited)
                {
                    if (this.GetComponent<AudioSource>() != null)
                    {
                        this.GetComponent<AudioSource>().Play();
                    }

                    //StopCoroutine(MoveToPosition(this.transform, EndPosition, SecondsToTravel));
                    
                    IsHitted = true;
                    Move = false;
                    TGS.CurrentNote.RemoveAt(NoteIndex);
                    GetComponent<RectTransform>().anchoredPosition = GameObject.FindGameObjectWithTag("AnimStartPoint").GetComponent<RectTransform>().anchoredPosition;
                    this.transform.GetComponent<Animation>().Play();
                    StartCoroutine(DestroyAfterSeconds());
                    //Debug.Log("coucou");

                }
                else
                {
                    TGS.CurrentNote.RemoveAt(NoteIndex);
                    //Debug.Log("try to destroy = + " + HitTime + CurrentNoteType.ToString());
                    Destroy(this.gameObject);
                }
                
            }
            else
            {
                //Debug.Log("Bonsoir PARIIIS");
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("destry");
        Destroy(this.gameObject);
    }
    bool Contains(List<GameObject> list, Note nameClass)
    {
        foreach (GameObject n in list)
        {
            if (n.GetComponentInChildren<Note>().HitTime == nameClass.HitTime)
            { return true; }
        }
        return false;
    }
    
    float UnitsToMovePerMs()
    {
        float beatsPerSeconds = 60 / NoteBPM;
        float milliSecondsPerBeat = beatsPerSeconds / 1000;
        float distToTravel = Vector3.Distance(new Vector3(59, 0, 51), new Vector3(-59, 0, 51));
        //Debug.Log(distToTravel);
        return distToTravel / beatsPerSeconds;

    }
    
}
