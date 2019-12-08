using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class TestSave : MonoBehaviour
{
    public List<TaikoSongContainer> tja;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SaveArray()
    {
        //SongDataSaver.SaveData(this);
    }

    public void loadData()
    {
        tja = SongDataSaver.LoadData().tja;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
