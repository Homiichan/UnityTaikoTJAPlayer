using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class TaikoStaticExtension
{
    // Start is called before the first frame update
    public static IEnumerator LoadAndPlaySong(AudioSource player, TaikoSongContainer CurrentPlayingSong)
    {

        AudioType CurrentAudioType = GetAudioTypeBasedOnPath(CurrentPlayingSong.OggPath);
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
                    //yield return null;
                }
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                player.clip = clip;
                player.Play();
            }
        }
    }

    public static AudioSource GetMainSongPlayer()
    {
        AudioSource tmpAudio = GameObject.FindGameObjectWithTag("SongAudioSource").GetComponent<AudioSource>();
        if (tmpAudio)
        {
            return tmpAudio;
        }
        else
        {
            return null;
        }
    }


    public static AudioType GetAudioTypeBasedOnPath(string path)
    {
        string filename = path.Split('.')[0];
        string extension = path.Split('.')[1];
        switch (extension)
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

    public static Taiko_Difficulty GetTaikoDiffByInt(int DiffIndex)
    {
        switch (DiffIndex)
        {
            case 0:
                return Taiko_Difficulty.Dif_Easy;
            case 1:
                return Taiko_Difficulty.Dif_Normal;
            case 2:
                return Taiko_Difficulty.Dif_Hard;
            case 3:
                return Taiko_Difficulty.Dif_Oni;
            case 4:
                return Taiko_Difficulty.Dif_Edit;
        }
        return Taiko_Difficulty.Dif_Easy;
    }

    public static void SetFadeState(bool FadeState)
    {
        LoadingScreenHandler LCH = GameObject.FindObjectOfType<LoadingScreenHandler>();
        if(LCH)
        {
            if(FadeState)
            {
                LCH.FadeIn();
            }
            else
            {
                LCH.FadeOut();
            }
        }
    }
}

