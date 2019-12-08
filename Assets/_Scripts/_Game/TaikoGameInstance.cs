using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
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

    public void SaveSongDb()
    {
        SongDataSaver.SaveData(this);
    }

    public void LoadData()
    {
        TJAFileAvailable = SongDataSaver.LoadData().tja;
    }
}
