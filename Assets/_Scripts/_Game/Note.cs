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
    bool IsGoGoTime = false;
   

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

    public void OnSpawn(Vector3 targetEndPoint, Taiko_Notes targetNoteType, float BPM, float NoteTime, float elapsedTime, NoteData NoteData)
    {
        TGS = GameObject.FindObjectOfType<TaikoSongPlayer>();
        CurrentNoteType = targetNoteType;
        CurrentNoteData.NoteType = targetNoteType;
        NoteBPM = NoteData.NoteBPM;
        ScrollSpeed = NoteData.ScrollSpeed;
        IsGoGoTime = NoteData.IsGoGoTime;
        startPosY = transform.position.x;
        EndPosition = targetEndPoint;
        SpawnNoteTime = NoteTime;
        Debug.Log(NoteTime + targetNoteType.ToString());
        if(CurrentNoteType == Taiko_Notes.bareline)
        {
            GetComponent<Image>().sprite = Bareline;
            GetComponent<Image>().SetNativeSize();
        }
        if(CurrentNoteType == Taiko_Notes.Blank)
        {
            DestroyNote(false);
        }
        /*
        switch (CurrentNoteType)
        {
            case Taiko_Notes.bareline:
                GetComponent<Image>().sprite = Bareline;
                GetComponent<Image>().SetNativeSize();
                break;

            case Taiko_Notes.Blank:
                DestroyNote(false);
                break;
        } 
        */
        if(CurrentNoteType != Taiko_Notes.bareline)
        {
            FindCorrectNoteMaterial();
        }
        PredictNoteTime();
        Move = true;

    }

    void PredictNoteTime()
    {
        float EndDistance = Vector3.Distance(transform.position, EndPosition);
        float HitDistance = Vector3.Distance(transform.position, EndPosition);
        NoteSpeed = EndDistance / (60000 / NoteBPM);
        NoteEndTime = EndDistance / (NoteSpeed / Time.deltaTime);
        NoteEndTime = (NoteEndTime * (1 / ScrollSpeed)) + SpawnNoteTime;
        HitTime = HitDistance / (NoteSpeed / Time.deltaTime);
        HitTime = (HitTime * (1 / ScrollSpeed)) + SpawnNoteTime;

    }
    void SetNoteColorAndSize()
    {
        SetNoteTexture(CurrentMaterialList.CloseTexture);
        switch (CurrentNoteType)
        {
            case Taiko_Notes.BigKa:
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
            case Taiko_Notes.BigDrumRoll:
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }

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
    // Update is called once per frame
    void Update()
    {
        Tick += Time.deltaTime;
        SwitchNoteTexture();
        if (Move)
        {
            transform.position -= new Vector3((NoteSpeed * ScrollSpeed), 0,0);
            //transform.position -= new Vector3(NoteBPM / 60, 0, 0);
           
             
            if (TGS.tick >= NoteEndTime)
            {
                DestroyNote(false);
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
                    IsHitted = true;
                    Move = false;
                    TGS.CurrentNote.RemoveAt(NoteIndex);
                    GetComponent<RectTransform>().anchoredPosition = GameObject.FindGameObjectWithTag("AnimStartPoint").GetComponent<RectTransform>().anchoredPosition;
                    this.transform.GetComponent<Animation>().Play();
                    StartCoroutine(DestroyAfterSeconds());

                }
                else
                {
                    TGS.CurrentNote.RemoveAt(NoteIndex);;
                    Destroy(this.gameObject);
                }
                
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
    
    
}
