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
    bool SongHasStarted = false;
    float tick;

    //ReadLineVar
    public int CurrentLine = 0;
    public int CurrentChar = 0;
    public string CurrentLineToRead;
    public float MeasureLenght = 0;
    float fullMeasureLenght = 0;
    public float measure = 0;
    public float noteduration = 0;
    public int NoteNumsInMeasure = 0;
    public float LasSpawnedNote;
    public float LastSpawnedBareline;
    Taiko_Notes LastSpawnNote;

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
        if(SongHasStarted)
        {
            tick += Time.deltaTime;
            //Debug.Log(tick);
            GetClosesNotes(tick);
        }
    }

    IEnumerator StartSound(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReCalculateMeasureVar();
        tick = Mathf.Abs(CurrentPlayingSong.Offset);
        SongHasStarted = true;
        TGI.SetGiBPM(CurrentPlayingSong.BPM);
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
                    //Debug.Log("Next line cause of commands");
                }
                else if (CurrentChar <= CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine].Length - 1)
                {
                    noteduration = measure / NoteNumsInMeasure;
                    tmplinetoread = CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine];
                    //SpawnNote(stringToTaikoNote(CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData[CurrentLine][CurrentChar]));
                    //Debug.Log(CurrentNoteMS);
                    CurrentChar++;
                }
                else
                {
                    CurrentChar = 0;
                    CurrentLine++;
                    ReCalculateMeasureVar();
                    //Debug.Log("Next Line");
                }
            }
        }
        
    }

    void ParseNoteData()
    {
        if (DiffToPlay != -1)
        {
            tmpListData = new List<string>(CurrentPlayingSong.AllSongDifficulty[DiffToPlay].SongData);
            
            float CurrentMeasure = CalculateMeasureDataRework(4, 4);
            Debug.Log(CurrentMeasure);
            var measureCount = 0;
            var nowTime = (long)(CurrentPlayingSong.Offset * 1000 * 1000.0) * -1;
            Debug.Log(nowTime);
            for (int i = 0; i <= tmpListData.Count -1; i++)
            {
                string tmpLine = tmpListData[i];
                var timePerNotes = (long)(CurrentMeasure / CurrentPlayingSong.BPM / tmpLine.Length * 1000 * 1000.0);
                /*
                var measureChip = new NoteData();
                measureChip.ChipType = Chips.Measure;
                measureChip.IsHitted = false;
                measureChip.IsGoGoTime = gogoTime;
                measureChip.CanShow = true;
                measureChip.Scroll = nowScroll;
                measureChip.Branch = nowBranch;
                measureChip.Branching = branching;
                measureChip.Time = nowTime;
                measureChip.Scroll = nowScroll;
                measureChip. = nowBPM;
                measureChip.MeasureNumber = measureCount;
                // Listへ
                list.Add(measureChip);
                */
                if (!tmpLine.StartsWith("#") && !tmpLine.StartsWith(","))
                {
                    // 音符
                    foreach (var note in tmpLine)
                    {
                        var chip = new NoteData();
                        //chip.ChipType = Chips.Note;
                        chip.NoteType = stringToTaikoNote(note);
                        chip.Notedata = note.ToString();
                        //chip.IsHitted = false;
                        chip.HasBeenSpawned = false;
                        //chip.IsGoGoTime = gogoTime;
                        //chip.CanShow = true;
                        //chip.Scroll = nowScroll;
                        //chip.Branch = nowBranch;
                        //chip.Branching = branching;
                        chip.Time = nowTime / 1000000f;
                        //chip.Scroll = nowScroll;
                        chip.MeasureNumber = measureCount;

                        nowTime += timePerNotes;
                        //var msPerMeasure = 60000 * measure / note.bpm
                        //check 3

                        // Listへ
                        SongNoteData.Add(new NoteData(chip));
                        //list.Add(chip);
                    }
                }
                var chipbareline = new NoteData();
                //chip.ChipType = Chips.Note;
                chipbareline.NoteType = Taiko_Notes.bareline;
                chipbareline.Notedata = "bareline";
                //chip.IsHitted = false;
                chipbareline.HasBeenSpawned = false;
                //chip.IsGoGoTime = gogoTime;
                //chip.CanShow = true;
                //chip.Scroll = nowScroll;
                //chip.Branch = nowBranch;
                //chip.Branching = branching;
                chipbareline.Time = nowTime / 1000000f;
                //chip.Scroll = nowScroll;
                //chip.BPM = nowBPM;
                chipbareline.MeasureNumber = measureCount;

                // ひとつ進める
                //nowTime += timePerNotes;
                //var msPerMeasure = 60000 * measure / note.bpm
                //check 3

                // Listへ
                SongNoteData.Add(new NoteData(chipbareline));
                measureCount++;
            }
            StartCoroutine(LoadAlbumAudio(AS));
        }
    }


    float CalculateMeasureDataRework(float Part, float Beat)
    {
        return 240 * (Part / Beat);
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
        tmpObject.GetComponentInChildren<Note>().OnSpawn(.5f, EndPoint.transform.position, NoteToSpawn, CurrentPlayingSong.BPM);
        //Debug.Log(noteduration.ToString() + NoteToSpawn);
        


    }

    void GetClosesNotes(float tick)
    {
        //Debug.Log(tick);
        /*
        foreach(NoteData test in SongNoteData)
        {
            if(RoughlyEqual(test.Time, tick, 0.01f) && !test.HasBeenSpawned)
            {
                NoteData ND = test;
                Debug.Log("ok " + test.NoteType.ToString() + test.Time + " note data = " + test.Notedata + "and tick = " + tick);
                ND.HasBeenSpawned = true;
                test = ND;
            }
        }
        */
        for (int i = 0; i < SongNoteData.Count - 1; i++)
        {
            NoteData tmpNoteData = SongNoteData[i];
            if (RoughlyEqual(tmpNoteData.Time, tick, 0.01f))
            {
                if(LasSpawnedNote != tmpNoteData.Time && tmpNoteData.NoteType != Taiko_Notes.bareline)
                {
                    //Debug.Log("ok " + tmpNoteData.NoteType.ToString() + tmpNoteData.Time + " note data = " + tmpNoteData.Notedata + "and tick = " + tick + tmpNoteData.HasBeenSpawned);
                    SpawnNote(tmpNoteData.NoteType);
                    tmpNoteData.HasBeenSpawned = true;
                    LasSpawnedNote = tmpNoteData.Time;
                    LastSpawnNote = tmpNoteData.NoteType;
                }
                else if(LastSpawnedBareline != tmpNoteData.Time && tmpNoteData.NoteType == Taiko_Notes.bareline)
                {
                    SpawnNote(tmpNoteData.NoteType);
                    tmpNoteData.HasBeenSpawned = true;
                    LastSpawnedBareline = tmpNoteData.Time;
                    //LastSpawnNote = tmpNoteData.NoteType;
                }
                
            }
        }
    }

    static bool RoughlyEqual(float a, float b, float treshold)
    {
        //Debug.Log(Mathf.Abs(a - b));
        return (Mathf.Abs(a - b) < treshold);
    }

}
