using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SongData
{
    public List<TaikoSongContainer> tja;

    public SongData(TaikoGameInstance TS)
    {
        tja = new List<TaikoSongContainer>(TS.TJAFileAvailable);
    }
}
