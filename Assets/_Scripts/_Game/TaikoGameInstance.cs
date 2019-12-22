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

    GameObject SongLoadingScreen;
    // Start is called before the first frame update
    TJAParser Parser;
    void Start()
    {

        SongLoadingScreen = GameObject.FindGameObjectWithTag("SongLoadingScreen");
        Parser = GetComponent<TJAParser>();
        DontDestroyOnLoad(this);
        if (SongDataSaver.DoesFilesExist())
        {
            CheckForMissingSongs();
        }
        else
        {
            CreateTJADataBase();
        }
        
    }


    void CheckForMissingSongs()
    {
        List<string> MissingFileToPase = new List<string>();
        TJAFileAvailable = SongDataSaver.LoadData().tja;

        List<string> tmpfile;

        List<TaikoSongContainer> SongIndexToRemove = new List<TaikoSongContainer>();
        tmpfile =  new List<string>(Directory.GetFiles(SongDataSaver.GetDefaultTJAPath(), "*.tja", SearchOption.AllDirectories));
        foreach(string songName in tmpfile)
        {
            bool found = false;
            foreach (TaikoSongContainer tmpSong in TJAFileAvailable)
            {
                if (tmpSong.SongPath == songName)
                {
                    found = true;
                }
            }

            if(!found)
            {
                Debug.Log("file not found = " + songName);
                MissingFileToPase.Add(songName);
            }
        }


        for(int i = 0; i <= TJAFileAvailable.Count - 1; i++)
        {
            TaikoSongContainer tmpSong = TJAFileAvailable[i];
            bool IsInSongDB = tmpfile.Contains(tmpSong.SongPath);
            if (!IsInSongDB)
            {
                Debug.Log("Missing file at " + tmpSong.TitleName + "Index is " + i);
                SongIndexToRemove.Add(tmpSong);
                
            }

        }


        foreach (string tmpMissingFile in MissingFileToPase)
        {
            TaikoSongContainer tmpstruc = Parser.ReadTJAFileRework(tmpMissingFile);

            TJAFileAvailable.Add(tmpstruc);
            Debug.Log("parsing " + tmpMissingFile + " plus " +tmpstruc.SongPath);
        }

        foreach(TaikoSongContainer indexToRemove in SongIndexToRemove)
        {
            Debug.Log("removing index = " + indexToRemove);
            TJAFileAvailable.Remove(indexToRemove);
        }

        SaveSongDb();
        StartCoroutine(Test());

        
    }

    void CreateTJADataBase()
    {
        List<TaikoSongContainer> SongIndexToRemove = new List<TaikoSongContainer>();
        List<string> tmpfile = new List<string>(Directory.GetFiles(SongDataSaver.GetDefaultTJAPath(), "*.tja", SearchOption.AllDirectories));
        foreach (string tmpMissingFile in tmpfile)
        {
            TaikoSongContainer tmpstruc = Parser.ReadTJAFileRework(tmpMissingFile);

            TJAFileAvailable.Add(tmpstruc);
            Debug.Log("parsing " + tmpMissingFile + " plus " + tmpstruc.SongPath);
        }

        SaveSongDb();
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
        if (true)
        {

            //CurrentSelectedSong = TJAFileAvailable[SongIndex];
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

    public void SetSelectedSong(int selectedSong)
    {
        if(selectedSong <= TJAFileAvailable.Count - 1)
        {
            Debug.Log("Selected song = " + TJAFileAvailable[selectedSong].TitleName);
            CurrentSelectedSong = TJAFileAvailable[selectedSong];
        }
        else
        {
            Debug.Log("Bad INDEX");
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
        yield return new WaitForSeconds(0);
        SongLoadingScreen.SetActive(false);
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
