using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public float tick;

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
    public Text text;

    public List<NoteData> SongNoteData = new List<NoteData>();
    public List<string> tmpListData;
    public List<GameObject> CurrentNote = new List<GameObject>();
    public GameObject HitPoint;


    // Start is called before the first frame update
    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        AS = GetComponent<AudioSource>();
       // Time.timeScale = .5f;
        
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
    void FixedUpdate()
    {
        //Debug.Log("start pos = " + SpawnPoint.transform.position + "end point = " + EndPoint.transform.position);
        text.text = tick.ToString();
        if(SongHasStarted)
        {
            tick += Time.fixedDeltaTime;
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
            float CurrentScroll = 2;
            bool IsGoGoTime = false;
            var measureCount = 0;
            var nowTime = (long)(CurrentPlayingSong.Offset * 1000.0) * -1;
            bool InScroll = false;
            bool InBalloon = false;
            Debug.Log(nowTime);
            float CurrentBPM = CurrentPlayingSong.BPM;
            for (int i = 0; i <= tmpListData.Count -1; i++)
            {
                string tmpLine = tmpListData[i];
                //tmpLine = tmpLine.Replace(",", string.Empty);
                var nowMeasureNote = 0;
                if(!tmpLine.StartsWith("#"))
                {
                    nowMeasureNote = tmpLine.Length;
                }
                Debug.Log(tmpLine);
                var timePerNotes = (long)(CurrentMeasure / CurrentBPM / nowMeasureNote * 1000.0);
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
                if (!tmpLine.StartsWith("#") && !tmpLine.StartsWith("="))
                {
                    // 音符
                    foreach (var note in tmpLine)
                    {
                        var tmpNote = new NoteData();
                        //chip.ChipType = Chips.Note;
                        switch(note)
                        {
                            case '5' :
                                InScroll = true;
                                break;

                            case '6':
                                InScroll = true;
                                break;

                            case '7' :
                                InBalloon = true;
                                break;

                            case '8' :
                                InBalloon = false;
                                InScroll = false;
                                break;

                            case '9':
                                InBalloon = true;
                                break;
                        }
                        tmpNote.NoteType = stringToTaikoNote(note);
                        tmpNote.Notedata = note.ToString();
                        tmpNote.HasBeenSpawned = false;
                        tmpNote.NoteBPM = CurrentBPM;
                        tmpNote.ScrollSpeed = CurrentScroll;
                        tmpNote.IsGoGoTime = IsGoGoTime;
                        tmpNote.Time = nowTime / 1000f;
                        tmpNote.IsBalloon = InBalloon;
                        tmpNote.IsSliderNote = InScroll;
                        tmpNote.MeasureNumber = measureCount;
                        //Debug.Log(nowTime);
                        nowTime = nowTime + timePerNotes;
                        //Debug.Log("measure = " + CurrentMeasure + "bpm = " + CurrentBPM + "line lenght = " + tmpLine.Length + "calculation = " + (tmpLine.Length * 1000) + "now time = " + nowTime + "Timer per note  = " + timePerNotes);
                        SongNoteData.Add(new NoteData(tmpNote));
                        //Debug.Log(nowTime);
                    }
                }
                else if(tmpLine.StartsWith("#") && !tmpLine.StartsWith("="))
                {
                    Debug.Log("command = " + tmpLine);
                    string tmpcommand = tmpLine.Substring(1);
                    if(tmpcommand.Contains("MEASURE"))
                    {
                        string[] tempsMeasureSplit = tmpcommand.Split(' ');
                        if (tempsMeasureSplit.Length > 1)
                        {
                            string[] tmpMeasureValue = tempsMeasureSplit[1].Split('/');
                            //Debug.Log(tmpMeasureValue[0] + " " + tmpMeasureValue[1] + " " + CurrentMeasure);
                            CurrentMeasure = CalculateMeasureDataRework(tryToParseFloatCheck(tmpMeasureValue[0]),tryToParseFloatCheck(tmpMeasureValue[1]));
                        }    
                    }

                    if (tmpcommand.Contains("BPMCHANGE"))
                    {
                        string[] tempsBPMSplit = tmpcommand.Split(' ');
                        if (tempsBPMSplit.Length > 1)
                        {
                            CurrentBPM = tryToParseFloatCheck(tempsBPMSplit[1]);
                            //Debug.Log(CurrentBPM);
                        }
                    }

                    if (tmpcommand.Contains("SCROLL"))
                    {
                        string[] tempScrollSplit = tmpcommand.Split(' ');
                        if (tempScrollSplit.Length > 1)
                        {
                            CurrentScroll = tryToParseFloatCheck(tempScrollSplit[1]);
                            Debug.Log(CurrentScroll);
                        }
                    }

                    if (tmpcommand.Contains("SCROLL"))
                    {
                        string[] tempScrollSplit = tmpcommand.Split(' ');
                        if (tempScrollSplit.Length > 1)
                        {
                            CurrentScroll = tryToParseFloatCheck(tempScrollSplit[1]);
                            Debug.Log(CurrentScroll);
                        }
                    }

                    if (tmpcommand.Contains("GOGOSTART"))
                    {
                        IsGoGoTime = true;
                    }

                    if (tmpcommand.Contains("GOGOEND"))
                    {
                        IsGoGoTime = false;
                    }

                }
                var chipbareline = new NoteData();
                chipbareline.NoteType = Taiko_Notes.bareline;
                chipbareline.Notedata = "bareline";
                chipbareline.HasBeenSpawned = false;
                //chip.IsGoGoTime = gogoTime;
                //chip.CanShow = true;
                chipbareline.NoteBPM = CurrentBPM;
                //chip.Scroll = nowScroll;
                //chip.Branch = nowBranch;
                //chip.Branching = branching;
                chipbareline.Time = nowTime / 1000f;
                chipbareline.ScrollSpeed = CurrentScroll;
                //chip.Scroll = nowScroll;
                //chip.BPM = nowBPM;
                chipbareline.MeasureNumber = measureCount;
                //chipbareline.Time = nowTime / 1000000f;
                //nowTime += timePerNotes;
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
        Debug.Log("Recalculating Measure = " + (240 * (Part / Beat)));
        return 240 * (Part / Beat);
    }

    bool IsEnglishLetter(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    float NowMeasureNote(string str)
    {
        if(!str.StartsWith("#"))
        {
            str.Replace(',', '\0');
            str.Trim();
            Debug.Log(str);
            return str.Length;
        }
        else
        {
            return 0;
        }
    }

    void SpawnNote(Taiko_Notes NoteToSpawn, float spawnTime, NoteData NoteDataToSpawn)
    {
        GameObject tmpObject = Instantiate(ObjectToSpawn, SpawnPoint.transform.position, Quaternion.identity);
        tmpObject.transform.SetParent(this.transform, false);
        
        tmpObject.transform.position = SpawnPoint.transform.position;
        if(NoteToSpawn != Taiko_Notes.bareline && NoteToSpawn != Taiko_Notes.Blank)
        {
            CurrentNote.Add(tmpObject);
        }
        tmpObject.GetComponent<Note>().OnSpawn(EndPoint.transform.position, NoteToSpawn, CurrentPlayingSong.BPM, spawnTime, tick, NoteDataToSpawn);
        //tmpObject.name = tmpObject.GetComponent<Note>().HitTime.ToString();




    }

    void GetClosesNotes(float tick)
    {
        for (int i = 0; i < SongNoteData.Count - 1; i++)
        {
            NoteData tmpNoteData = SongNoteData[i];
            if (RoughlyEqual(tmpNoteData.Time, tick, .01f))
            {
                if(LasSpawnedNote != tmpNoteData.Time && tmpNoteData.NoteType != Taiko_Notes.bareline)
                {
                    SpawnNote(tmpNoteData.NoteType, tmpNoteData.Time, tmpNoteData);
                    tmpNoteData.HasBeenSpawned = true;
                    LasSpawnedNote = tmpNoteData.Time;
                    LastSpawnNote = tmpNoteData.NoteType;
                    SongNoteData.RemoveAt(i);
                    break;
                }
                
                else if(LastSpawnedBareline != tmpNoteData.Time && tmpNoteData.NoteType == Taiko_Notes.bareline)
                {
                    SpawnNote(tmpNoteData.NoteType, tmpNoteData.Time, tmpNoteData);
                    tmpNoteData.HasBeenSpawned = true;
                    LastSpawnedBareline = tmpNoteData.Time;
                    SongNoteData.RemoveAt(i);
                    break;
                }
                
                
            }
        }
    }

    static bool RoughlyEqual(float a, float b, float treshold)
    {
        return (Mathf.Abs(a - b) < treshold);
    }

    public void RegisterInput(DrumInputType NotePressed)
    {
        FindClosedNote(NotePressed);
    }

    void FindClosedNote(DrumInputType NotePressed)
    {
        float HittedTime = tick;
        //Debug.Log("Hitted time = " + tick);
        for(int i = 0; i < CurrentNote.Count -1;i++)
        {
            Note TmpForNote = CurrentNote[i].GetComponent<Note>();
            if (TmpForNote.CurrentNoteType != Taiko_Notes.bareline && TmpForNote.CurrentNoteType != Taiko_Notes.Blank)
            {
                if (TmpForNote.HitTime -  HittedTime < .4f && CanHitThisNote(NotePressed, TmpForNote.CurrentNoteType))
                {
                    //Debug.Log("Hitt time = " + TmpForNote.HitTime + "dif  =  " + (TmpForNote.HitTime - HittedTime));
                    TmpForNote.DestroyNote(true);
                }
            }
        }
    }

    bool CanHitThisNote(DrumInputType NotePressed, Taiko_Notes NoteToCheck)
    {
        if(NotePressed == DrumInputType.LeftKa || NotePressed == DrumInputType.RightKa && NoteToCheck == Taiko_Notes.Ka || NoteToCheck == Taiko_Notes.BigKa)
        {
            return true;
        }
        if (NotePressed == DrumInputType.RightDon || NotePressed == DrumInputType.LeftDon && NoteToCheck == Taiko_Notes.Don || NoteToCheck == Taiko_Notes.BigDon)
        {
            return true;
        }

        return false;
    }

    float tryToParseFloatCheck(string StrToParse)
    {
        float convertedFloat = 0;
        string tmpstr = StrToParse;

        if (float.TryParse(tmpstr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out convertedFloat))
        {
            return convertedFloat;
        }
        else
        {
            Debug.Log("can;t parse = " + StrToParse);
            return 0;
        }
    }




}
