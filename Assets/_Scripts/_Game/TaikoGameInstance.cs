using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum Taiko_NoteState
{
    Mounth_Open,
    Mouth_Open_Angry,
    Mouth_Close
}

public class TaikoGameInstance : MonoBehaviour
{
    public List<TaikoSongContainer> TJAFileAvailable;
    public TaikoSongContainer CurrentSelectedSong;
    public int SongDiffIndex = 0;
    float CurrentSongBPM = 0;
    public Taiko_NoteState CurrentNoteState;
    public ScrollRectSnap Temp;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        if (SongDataSaver.DoesFilesExist())
        {
            CheckForMissingSongs();
        }
        
    }


    void CheckForMissingSongs()
    {
        TJAFileAvailable = SongDataSaver.LoadData().tja;
        foreach(TaikoSongContainer tmpSong in TJAFileAvailable)
        {

        }

        string[] tmpfile;
        tmpfile = Directory.GetFiles(SongDataSaver.GetDefaultTJAPath(), "*.tja", SearchOption.AllDirectories);
        for(int i = 0; i <=  tmpfile.Length - 1; i++)
        {
            string tmpFilepath = tmpfile[i];
            foreach (TaikoSongContainer tmpSong in TJAFileAvailable)
            {
                if(tmpSong.SongPath == tmpFilepath)
                {
                    break;
                }
            }
        }
        StartCoroutine(Test());

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTJAFileParsed(List<TaikoSongContainer> ListToAdd)
    {
        TJAFileAvailable = new List<TaikoSongContainer>(ListToAdd);
    }

    public void FindSongAndDifficulty(int SongIndex, Taiko_Difficulty DiffToFind)
    {
        SongDiffIndex = -1;
        if (SongIndex <= TJAFileAvailable.Count - 1)
        {

            CurrentSelectedSong = TJAFileAvailable[SongIndex];
            for (int i = 0; i <= CurrentSelectedSong.AllSongDifficulty.Count - 1; i++)
            {
                if (CurrentSelectedSong.AllSongDifficulty[i].Difficulty == DiffToFind)
                {
                    SongDiffIndex = i;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("song index not valid");
        }
    }

    public void SetGiBPM(float targetBPM)
    {
        CurrentSongBPM = targetBPM;
        StartCoroutine(SwitchNoteTexture());
    }


    public IEnumerator SwitchNoteTexture()
    {

        yield return new WaitForSeconds((60000 / CurrentSongBPM));
        if(CurrentNoteState == Taiko_NoteState.Mounth_Open)
        {
            CurrentNoteState = Taiko_NoteState.Mouth_Close;
        }
        else
        {
            CurrentNoteState = Taiko_NoteState.Mounth_Open;
        }
        StartCoroutine(SwitchNoteTexture());

    }


    public IEnumerator Test()
    {
        yield return new WaitForSeconds(1);
        Temp.ConstructUI();

    }
    public void SaveSongDb()
    {
        SongDataSaver.SaveData(this);
    }

    public void LoadData()
    {
        TJAFileAvailable = SongDataSaver.LoadData().tja;
    }
}
