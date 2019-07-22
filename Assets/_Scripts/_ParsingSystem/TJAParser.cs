using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum Taiko_Difficulty
{
    Dif_Easy,
    Dif_Normal,
    Dif_Hard,
    Dif_Oni,
    Dif_Edit
}

public enum Taiko_Notes
{ 
    Blank,
    Don,
    Ka,
    BigDon,
    BigKa,
    DrumRoll,
    BigDrumRoll,
    startBalloon,
    endBalloon,
    Kusudama,
    endKusudama,
    bareline,
    BPMChange,
    MeasureChange,
    GoGoStart,
    GoGoEnd
}

[System.Serializable]
public struct NoteData
{
    public string Notedata;
    public float MeasureLenght;
    public float NoteDurationMS;
    public Taiko_Notes NoteType;
    public int NoteNumber;
    public float Time;
    public float MeasureNumber;
    public bool HasBeenSpawned;
    public float NoteBPM;
    public bool IsGoGoTime;
    public float ScrollSpeed;
    public bool IsSliderNote;
    public bool IsBalloon;
    public float DrumRollEndTime;

    public NoteData(NoteData Template)
    {
        /*
        this.Notedata = targetNoteDate;
        this.MeasureLenght = targetNoteSpawnMS;
        this.NoteDurationMS = targetNoteDurationMS;
        this.NoteType = targetNoteType;
        this.NoteNumber = targetNoteNumbers;
        */
        this = Template;
    }
}

[System.Serializable]
public struct TaikoSongContainer
{
    public string TitleName;

    public List<TaikoSongStruc> AllSongDifficulty;

    public List<string> AllSongData;

    public float BPM;

    public string SoundWavePath;

    public float Offset;

    public float SongVolume;

    public float SEVOL;

    public float DEMOSTART;

    public int SCOREMODE;

    public string Genre;

    public string SongPath;

    public string OggPath;

    
    public TaikoSongContainer(string TitleName, List<TaikoSongStruc> AllSongDifficultyInside,List<string> AllSongData, float BPM, string SoundWavePath, float Offset, float SongVolume, float SEVOL, float DEMOSTART, int SCOREMOD, string Genre, string SongPath, string OggPath)
    {
        this.TitleName = TitleName;
        this.AllSongDifficulty = AllSongDifficultyInside;
        this.AllSongData = AllSongData;
        this.BPM = BPM;
        this.SoundWavePath = SoundWavePath;
        this.Offset = Offset;
        this.SongVolume = SongVolume;
        this.SEVOL = SEVOL;
        this.DEMOSTART = DEMOSTART;
        this.SCOREMODE = SCOREMOD;
        this.Genre = Genre;
        this.SongPath = SongPath;
        this.OggPath = OggPath;
    }
    
    
}

[System.Serializable]
public struct TaikoSongStruc
{

    public Taiko_Difficulty Difficulty;

    public int StarCount;

    public float Balloon;

    public List<string> SongData;
    public TaikoSongStruc(Taiko_Difficulty Diff, int StarCount,int Balloon, List<string> songdata)
    {
        this.Difficulty = Diff;
        this.StarCount = StarCount;
        this.Balloon = Balloon;
        this.SongData = new List<string>();
    }

};


public class TJAParser : MonoBehaviour
{
    public string folderPath;
    List<string> TJAFileInDir = new List<string>();
    List<string> TJAFileInDirPath = new List<string>();
    public List<TaikoSongContainer> TJAFileAvailable;
    public TaikoSongContainer CurrentSongData;
    int FileToCheck;
    TaikoGameInstance TGI;
    SongLoader SL;
    // Start is called before the first frame update




    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        SL = GameObject.FindObjectOfType<SongLoader>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetAllTJAFileInFolder(string targetPath)
    {
        if(targetPath[targetPath.Length - 1] != '\\')
        {
            targetPath = targetPath + "\\";
        }
        string[] tmpfile;
        tmpfile = Directory.GetFiles(targetPath, "*.tja", SearchOption.AllDirectories);
        foreach (string FilePath in tmpfile)
        {
            TJAFileInDir.Add(FilePath);

            ReadTJAFile(FilePath);

        }
        //We Finished Parsing All The File Now We Are Pushing All Data To The GameInstance
        if(TGI != null)
        {
            TGI.OnTJAFileParsed(TJAFileAvailable);
            TJAFileAvailable.Clear();
        }
        SL.CreateUI();
    }


    void ReadTJAFile(string FilePath)
    {
        //
        //ReadingAllLineOfTJAFile
        //
        //Preparating lines for the parsing u_u
        string[] tmplines;
        if(File.Exists(FilePath))
        {
            tmplines = File.ReadAllLines(FilePath, System.Text.Encoding.GetEncoding("shift_jis"));
            for(int i = 0; i <= tmplines.Length -1; i++)
            {
                string currentline = tmplines[i];
                if (currentline != "")
                {
                    currentline.Replace("//", "").Trim();
                    if (currentline.Contains(","))
                    {
                        currentline = currentline.Split(',')[0];
                        currentline = currentline + ",";
                        CurrentSongData.AllSongData.Add(currentline);
                    }
                    else
                        CurrentSongData.AllSongData.Add(currentline);
                }
                
            }
        }

        if(CurrentSongData.AllSongData.Count != 0)
        {
            ParseMainData(CurrentSongData.AllSongData, FilePath);
        }

    }

    void ParseMainData(List<string> DataToParse, string filePath)
    {
        int tmpindex = filePath.IndexOf('\\');
        CurrentSongData.SongPath = filePath;
        bool inSong = false;
        bool isMultipleCourse = false;
        int CurrentCourseMeta = 0;

        TaikoSongStruc CurrentStruc = new TaikoSongStruc(Taiko_Difficulty.Dif_Oni, 0, 0, new List<string>());
        TaikoSongContainer CUrrentParsingStuff = new TaikoSongContainer("", new List<TaikoSongStruc>(), new List<string>(), 0, "", 0, 0, 0, 0, 0, "", "", "");
        List<string> TmpCourseData = new List<string>();
        //Parsing MetaData of the song 
        for (int i = 0; i <= DataToParse.Count - 1; i++)
        {
            string CurrentLine = DataToParse[i];
            int SongStart;
            int SongEnd;

            
            if (CurrentLine.Contains("COURSE"))
            {
                isMultipleCourse = true;
            }
            
            if (CurrentLine.Contains(":"))
            {
                string ParamName = CurrentLine.Split(':')[0];

                string ParamInfo = CurrentLine.Split(':')[1];

                if (ParamName == "TITLE" || ParamName == "GENRE" || ParamName == "WAVE")
                {
                    switch (ParamName)
                    {

                        case "TITLE":
                            CurrentSongData.TitleName = ParamInfo;
                            break;

                        case "GENRE":
                            CurrentSongData.Genre = ParamInfo;
                            break;

                        case "WAVE":
                            CurrentSongData.SoundWavePath = ParamInfo;
                            CurrentSongData.OggPath = GetOGGPathName(filePath, ParamInfo);

                            break;
                        default:

                            break;
                    }
                }
                if (ParamName == "BPM" || ParamName == "OFFSET" || ParamName == "SONGVOL" || ParamName == "SEVOL" || ParamName == "DEMOSTART" || ParamName == "SCOREMODE")
                {
                    switch (ParamName)
                    {

                        case "BPM":
                            CurrentSongData.BPM = tryToParseFloatCheck(ParamInfo, ParamName);
                            break;

                        case "OFFSET":
                            //Debug.Log(CurrentSongData.TitleName + ParamName);
                            CurrentSongData.Offset = tryToParseFloatCheck(ParamInfo, ParamName);
                            
                            break;

                        case "SONGVOL":
                            CurrentSongData.SongVolume = tryToParseFloatCheck(ParamInfo, ParamName);
                            break;

                        case "SEVOL":
                            CurrentSongData.SEVOL = tryToParseFloatCheck(ParamInfo, ParamName);
                            break;

                        case "DEMOSTART":
                            CurrentSongData.DEMOSTART = tryToParseFloatCheck(ParamInfo, ParamName);
                            break;

                        case "SCOREMODE":
                            CurrentSongData.SCOREMODE = tryToParseIntCheck(ParamInfo);
                            break;

                        default:

                            break;
                    }
                }
                //Parsing Course Data
                if (ParamName == "COURSE")
                {
                    if (CurrentSongData.AllSongDifficulty.Count >= CurrentCourseMeta)
                    {
                        CurrentSongData.AllSongDifficulty.Add(new TaikoSongStruc(Taiko_Difficulty.Dif_Oni, 0, 0, new List<string>(TmpCourseData)));
                        var Dif = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];
                        Dif.Difficulty = ParseTaikoDif(ParamInfo);
                        CurrentSongData.AllSongDifficulty[CurrentCourseMeta] = Dif;
                        isMultipleCourse = true;

                    }
                }
                if (ParamName == "LEVEL")
                {
                    if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                    {
                        var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];
                        Level.StarCount = tryToParseIntCheck(ParamInfo);
                        CurrentSongData.AllSongDifficulty[CurrentCourseMeta] = Level;
                    }
                    else if (!isMultipleCourse)
                    {
                        if (CurrentSongData.AllSongDifficulty.Count >= 1)
                        {
                            var Level = CurrentSongData.AllSongDifficulty[0];
                            Level.StarCount = tryToParseIntCheck(ParamInfo);
                            CurrentSongData.AllSongDifficulty[0] = Level;
                        }
                        else if (CurrentSongData.AllSongDifficulty.Count == 0)
                        {
                            CurrentSongData.AllSongDifficulty.Add(new TaikoSongStruc(Taiko_Difficulty.Dif_Oni, 0, 0, new List<string>(TmpCourseData)));
                            var Level = CurrentSongData.AllSongDifficulty[0];
                            Level.StarCount = tryToParseIntCheck(ParamInfo);
                            CurrentSongData.AllSongDifficulty[0] = Level;
                        }
                    }
                }
                if (ParamName == "BALLOON")
                {
                    if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                    {
                        var Balloon = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];
                        Balloon.Balloon = tryToParseFloatCheck(ParamInfo, ParamName);
                        CurrentSongData.AllSongDifficulty[CurrentCourseMeta] = Balloon;
                    }
                    else if (!isMultipleCourse)
                    {
                        if (CurrentSongData.AllSongDifficulty.Count >= 1)
                        {
                            var Balloon = CurrentSongData.AllSongDifficulty[0];
                            Balloon.Balloon = tryToParseIntCheck(ParamInfo);
                            CurrentSongData.AllSongDifficulty[0] = Balloon;
                        }
                        else if (CurrentSongData.AllSongDifficulty.Count == 0)
                        {
                            CurrentSongData.AllSongDifficulty.Add(new TaikoSongStruc(Taiko_Difficulty.Dif_Oni, 0, 0, new List<string>(TmpCourseData)));
                            var Balloon = CurrentSongData.AllSongDifficulty[0];
                            Balloon.Balloon = tryToParseIntCheck(ParamInfo);
                            CurrentSongData.AllSongDifficulty[0] = Balloon;
                        }
                    }

                }
            }

                if (CurrentLine[0] == '#')
                {
                    if (CurrentLine.Substring(1) == "START")
                    {
                        inSong = true;
                        SongStart = i;
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            CurrentStruc.SongData.Add(CurrentLine);
                            TmpCourseData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                        }

                    }
                    if (CurrentLine.Substring(1) == "END")
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            inSong = false;
                            SongEnd = i;
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];
                            CurrentCourseMeta++;
                            CurrentStruc.Balloon = 0;
                            CurrentStruc.StarCount = 0;
                        }

                    }

                    if (CurrentLine.Substring(1).Contains("MEASURE"))
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];
                        }
                    }
                    if (CurrentLine.Substring(1).Contains("BPMCHANGE"))
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];

                        }
                    }
                    if (CurrentLine.Substring(1).Contains("SCROLL"))
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];

                        }
                    }
                    if (CurrentLine.Substring(1).Contains("GOGOSTART"))
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];

                        }
                    }
                    if (CurrentLine.Substring(1).Contains("GOGOEND"))
                    {
                        if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                        {
                            TmpCourseData.Add(CurrentLine);
                            CurrentStruc.SongData.Add(CurrentLine);
                            CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                            var Level = CurrentSongData.AllSongDifficulty[CurrentCourseMeta];

                        }
                    }
            }
                if (inSong && CurrentLine[0] != '#')
                {
                    if (CurrentCourseMeta <= CurrentSongData.AllSongDifficulty.Count - 1)
                    {
                        CurrentStruc.SongData.Add(CurrentLine);
                        CurrentSongData.AllSongDifficulty[CurrentCourseMeta].SongData.Add(CurrentLine);
                        TmpCourseData.Add(CurrentLine);
                    }
                }


            }
            AddToAllTJAData(CUrrentParsingStuff);

        }

    void AddToAllTJAData(TaikoSongContainer tmptest)
    {
        TaikoSongContainer CurrentParsingSong = new TaikoSongContainer(CurrentSongData.TitleName,new List<TaikoSongStruc>(CurrentSongData.AllSongDifficulty), new List<string>(CurrentSongData.AllSongData),CurrentSongData.BPM, CurrentSongData.SoundWavePath, CurrentSongData.Offset, CurrentSongData.SongVolume, CurrentSongData.SEVOL,CurrentSongData.DEMOSTART,CurrentSongData.SCOREMODE, CurrentSongData.Genre,CurrentSongData.SongPath,CurrentSongData.OggPath);
        TJAFileAvailable.Add(CurrentParsingSong);
        ReInitStruc();

    }
    void ReInitStruc()
    {
        CurrentSongData.Offset = 0;
        CurrentSongData.Genre = "";
        CurrentSongData.OggPath = "";
        CurrentSongData.SCOREMODE = 0;
        CurrentSongData.SongVolume = 0;
        CurrentSongData.SoundWavePath = "";
        CurrentSongData.TitleName = "";
        CurrentSongData.AllSongData.Clear();
        CurrentSongData.AllSongDifficulty.Clear();
        CurrentSongData.BPM = 0;
        CurrentSongData.DEMOSTART = 0;

    }



    string GetOGGPathName(string Path, string OggFileName)
    {
        string combinedPath = Path.Substring(0, Path.Length - OggFileName.Length);
        if(combinedPath[combinedPath.Length - 1] != '\\')
        {
            combinedPath = combinedPath + "\\";
        }
        combinedPath = combinedPath + OggFileName;
        return combinedPath;
    }

    float tryToParseFloatCheck(string StrToParse, string paramName)
    {
        float convertedInt = 0;
        string tmpstr = StrToParse;

        if (float.TryParse(tmpstr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out convertedInt))
        {
            return convertedInt;
        }
        else
        {
            Debug.Log("can't parse " + CurrentSongData.TitleName + "   " + tmpstr);
            return 0;
        }
    }


    int tryToParseIntCheck(string StrToParse)
    {
        int convertedInt = 0;
        if (int.TryParse(StrToParse, out convertedInt))
        {
            return convertedInt;
        }
        else
        {
            Debug.Log("can't parse " + CurrentSongData.TitleName);
            return 0;
        }

    }

    Taiko_Difficulty ParseTaikoDif(string DiffToCheck)
    {
        Taiko_Difficulty Difficulty = Taiko_Difficulty.Dif_Oni;
        if (DiffToCheck == "Easy" || DiffToCheck == "0")
        {
            Difficulty = Taiko_Difficulty.Dif_Easy;
        }
        else if (DiffToCheck == "Normal" || DiffToCheck == "1")
        {
            Difficulty = Taiko_Difficulty.Dif_Normal;
        }
        else if (DiffToCheck == "Hard" || DiffToCheck == "2")
        {
            Difficulty = Taiko_Difficulty.Dif_Hard;
        }
        else if (DiffToCheck == "Oni" || DiffToCheck == "3")
        {
            Difficulty = Taiko_Difficulty.Dif_Oni;
        }
        else if (DiffToCheck == "Edit" || DiffToCheck == "4")
        {
            Difficulty = Taiko_Difficulty.Dif_Edit;
        }
        //Debug.Log(Difficulty);
        return Difficulty;
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(10);
        ReInitStruc();
    }
}
