using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaikoGameInstance : MonoBehaviour
{
    public List<TaikoSongContainer> TJAFileAvailable;
    public TaikoSongContainer CurrentSelectedSong;
    public int SongDiffIndex = 0;
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

}
