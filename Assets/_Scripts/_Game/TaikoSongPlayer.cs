using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TaikoSongPlayer : MonoBehaviour
{
    public TaikoGameInstance TGI;
    public TaikoSongContainer CurrentPlayingSong;
    public int DiffToPlay = 0;
    AudioSource AS;
    public string Filepath;
    public AudioType CurrentAudioType;
    AudioClip myClip;

    //ReadLineVar
    public int CurrentLine = 0;
    public int CurrentChar = 0;
    public string CurrentLineToRead;
    public float MeasureLenght = 0;
    float fullMeasureLenght = 0;
    public float measure = 0;
    public float noteduration = 0;
    public int NoteNumsInMeasure = 0;

    public GameObject SpawnPoint;
    public GameObject EndPoint;
    public GameObject ObjectToSpawn;
    /// <summary>
    /// parse data before or after parsing and convert course to custom struc circle with the spawn time in seconds or ms 
    /// To calculate that we will be using measure and measurelenght etc
    /// </summary>
    public List<NoteData> SongNoteData = new List<NoteData>();
    public List<string> tmpListData;


    // Start is called before the first frame update
    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        AS = GetComponent<AudioSource>();
        
        if(TGI != null)
        {
            CurrentPlayingSong = TGI.CurrentSelectedSong;
            DiffToPlay = TGI.SongDiffIndex;
            if(DiffToPlay != -1)
            {
                CurrentAudioType = GetAudioTypeBasedOnPath(CurrentPlayingSong.SoundWavePath);
                //StartCoroutine(LoadAlbumAudio(AS));
                ParseNoteData();
            }
        }
    }

    void StartSong()
    {
        StartCoroutine(StartSound(Mathf.Abs(CurrentPlayingSong.Offset)));
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartSound(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReCalculateMeasureVar();
        StartCoroutine(CaclReadLineBPM());
    }

    IEnumerator CaclReadLineBPM()
    {
        //Debug.Log(CurrentPlayingSong.BPM  / 60000 / 1000);
        //yield return new WaitForSeconds(CurrentPlayingSong.BPM / 60000 / 1000);
        yield return new WaitForSeconds((60000 / CurrentPlayingSong.BPM)/1000);
        ReadLine();
        //Debug.Log((60000 / CurrentPlayingSong.BPM) / 1000);
        StartCoroutine(CaclReadLineBPM());
    }
    private IEnumerator LoadAlbumAudio(AudioSource player)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + CurrentPlayingSong.OggPath, CurrentAudioType))
        {
            //yield return www.Send();
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (player.clip != null)
                {
                    // Remove from memory
                    Destroy(player.clip);
                    //yield return null;
                }
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                player.clip = clip;
                player.Play();
                StartSong();
                Debug.Log(clip.length);
            }
        }
    }

    Taiko_Notes stringToTaikoNote(char strToCheck)
    {
        //Debug.Log("Char Checked = " + strToCheck);
        string check = strToCheck.ToString();
        switch(check)
        {
            case "0":
                return Taiko_Notes.Blank;
            case "1":
                return Taiko_Notes.Don;
            case "2":
                return Taiko_Notes.Ka;
            case "3":
                return Taiko_Notes.BigDon;
            case "4":
                return Taiko_Notes.BigKa;
            case "5":
                return Taiko_Notes.DrumRoll;
            case "6":
                return Taiko_Notes.BigDrumRoll;
            case "7":
                return Taiko_Notes.startBalloon;
            case "8":
                return Taiko_Notes.endBalloon;
            case "9":
                return Taiko_Notes.Kusudama;
            case "10":
                return Taiko_Notes.endKusudama;
        }
        return Taiko_Notes.Blank;
    }

    AudioType GetAudioTypeBasedOnPath(string path)
    {
        string filename = path.Split('.')[0];
        string extension = path.Split('.')[1];
        switch(extension)
        {
            case "ogg":
                return AudioType.OGGVORBIS;
            case "mp3":
                return AudioType.MPEG;
            case "wav":
                return AudioType.WAV;
        }
        path.Split('.');
        return AudioType.OGGVORBIS;
    }

    void ReCalculateMeasureVar()
    {
        if(DiffToPlay != -1)
        {
            MeasureLenght = 240 * ( CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Length / CurrentPlayingSong.BPM);
            NoteNumsInMeasure = CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Length;
            measure = 240 * (4 * 4) / CurrentPlayingSong.BPM * 1000;
        }
    }

    void ReadLine()
    {
        string tmplinetoread;
        //Debug.Log(CurrentChar)
        if (DiffToPlay != -1)
        {
            if(CurrentLine <= CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Length -1)
            {
                if (CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Contains("#"))
                {
                    CurrentLine++;
                    CurrentChar = 0;
                    Debug.Log("Next line cause of commands");
                }
                else if (CurrentChar <= CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Length - 1)
                {
                    noteduration = measure / NoteNumsInMeasure;
                    tmplinetoread = CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine];
                    SpawnNote(stringToTaikoNote(CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine][CurrentChar]));
                    //Debug.Log(CurrentNoteMS);
                    CurrentChar++;
                }
                else
                {
                    CurrentChar = 0;
                    CurrentLine++;
                    ReCalculateMeasureVar();
                    Debug.Log("Next Line");
                }
            }
        }
        
    }

    void ParseNoteData()
    {
        CalculateMeasureDataRework(0,0);
        if (DiffToPlay != -1)
        {
            tmpListData = new List<string>(CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData);
            for(int i = 0; i <= tmpListData.Count -1; i++)
            {
                string tmpLine = tmpListData[i];
                //Debug.Log("Currenly Parsing this = " + tmpListData[i] + "  index of line = " + i);
                for(int z = 0; z <= tmpLine.Length -1; z++)
                {
                    char tmpChr = tmpLine[z];
                    //char[] charfilter = new char[] { '#',  };
                    //Debug.Log("Currently Parsing = " + tmpLine[z]);
                    if(!IsEnglishLetter(tmpChr) || tmpChr == '#')
                    {
                        if(tmpChr == ',')
                        {
                            CalculateMeasureDataRework(i, 0);
                        }
                        else
                        {
                            NoteData tmpNoteDate = new NoteData();
                            tmpNoteDate.NoteType = stringToTaikoNote(tmpChr);
                            tmpNoteDate.NoteDurationMS = MeasureLenght;
                            Debug.Log(MeasureLenght.ToString() + NoteNumsInMeasure.ToString());
                            tmpNoteDate.MeasureLenght = MeasureLenght / NoteNumsInMeasure;
                            SongNoteData.Add(new NoteData(tmpChr.ToString(), tmpNoteDate.MeasureLenght, tmpNoteDate.NoteDurationMS, tmpNoteDate.NoteType, NoteNumsInMeasure));
                        }
                    }
                    else
                    {
                        //Debug.Log("this is letter or command  = " + tmpChr);
                    }
                }
                /*
                if (i <= tmpListData.Count - 1)
                {
                    
                    if (tmpLine.Contains("#"))
                    {
                        tmpCurrentLine++;
                        tmpCurrentChar = 0;
                        //Debug.Log(tmpLine + tmpCurrentLine);
                        //Debug.Log("Next line cause of commands");
                    }
                    else if (tmpCurrentChar <= tmpListData[i].Length - 1)
                    {
                        if(tmpListData[i][tmpCurrentChar] == ',')
                        {
                            tmpCurrentChar = 0;
                            tmpCurrentLine++;
                            CalculateMeasureDataRework(tmpCurrentLine, 0);
                            //Debug.Log("Next Line cause of ::,");
                        }
                        else
                        {
                            NoteData tmpNoteDate = new NoteData();
                            tmpNoteDate.NoteSpawnMS = 0;
                            tmpNoteDate.NoteType = stringToTaikoNote(tmpListData[i][tmpCurrentChar]);
                            /////
                            noteduration = measure / NoteNumsInMeasure;
                            tmpNoteDate.NoteDurationMS = measure / NoteNumsInMeasure;
                            //tmplinetoread = CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine];
                            //SpawnNote(stringToTaikoNote(CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine][CurrentChar]));
                            SongNoteData.Add(new NoteData(tmpNoteDate.Notedata, tmpNoteDate.NoteSpawnMS, tmpNoteDate.NoteDurationMS, tmpNoteDate.NoteType));
                            //Debug.Log(CurrentNoteMS);
                            tmpCurrentChar++;
                        }
                        
                    }
                    else
                    {
                        tmpCurrentChar = 0;
                        tmpCurrentLine++;
                        CalculateMeasureDataRework(i, 0);
                        //Debug.Log("Next Line");
                    }
                }
                else
                {
                    Debug.Log("End Of The File");
                    break;
                }
                */
            }
            
            
        }
    }

    void CalculateMeasureDataRework(int targetLine, int targetChar)
    {
        if (targetLine <= CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[targetLine].Length - 1)
        {
            MeasureLenght = 240 * (4 * 4) / CurrentPlayingSong.BPM * 1000;
            fullMeasureLenght = fullMeasureLenght + MeasureLenght;
            NoteNumsInMeasure = CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[targetLine].Length;

        }
    }

    bool IsEnglishLetter(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    void SpawnNote(Taiko_Notes NoteToSpawn)
    {
        GameObject tmpObject = Instantiate(ObjectToSpawn, SpawnPoint.transform.position, Quaternion.identity);
        tmpObject.transform.parent = this.transform;
        tmpObject.transform.position = SpawnPoint.transform.position;
        tmpObject.GetComponentInChildren<Note>().OnSpawn(noteduration / 1000, EndPoint.transform.position, NoteToSpawn);
        Debug.Log(noteduration.ToString() + NoteToSpawn);
        


    }

}
