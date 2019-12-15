using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongDiffSelectorScreenController : MonoBehaviour
{



    public Image DiffSelector;

    [Header("Star Bar Ref")]
    public Image KantanBar;
    public Image FutsuBar;
    public Image MuzukashiiBar;
    public Image OniBar;

    [Header("DifButtonBar")]
    public Button KantanButton;
    public Button FutsuButton;
    public Button MuzukashiiButton;
    public Button OniButton;

    [Header("Song var")]
    public TaikoSongContainer CurrentSong;

    [Header("SongInfo")]
    public TextMeshProUGUI SongName;

    private AudioSource MainAudioPlayer;
    private bool KantanDifFound = false;
    private bool FutsuDifFound = false;
    private bool MuzukashiiDifFound = false;
    private bool OniDifFound = false;

    private TaikoGameInstance TGI;

    private bool SelectorIsMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        MainAudioPlayer = TaikoStaticExtension.GetMainSongPlayer();
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        SetupSong();
    }


    void SetupSong()
    {
        KantanDifFound = false;
        FutsuDifFound = false;
        MuzukashiiDifFound = false;
        OniDifFound = false;
        foreach (TaikoSongStruc tmpSong in CurrentSong.AllSongDifficulty)
        {
            switch (tmpSong.Difficulty)
            {
                case Taiko_Difficulty.Dif_Easy:
                    KantanBar.fillAmount = (float)tmpSong.StarCount / 10;
                    KantanDifFound = true;
                    break;
                case Taiko_Difficulty.Dif_Normal:
                    FutsuBar.fillAmount = (float)tmpSong.StarCount / 10;
                    FutsuDifFound = true;
                    break;
                case Taiko_Difficulty.Dif_Hard:
                    MuzukashiiBar.fillAmount = (float)tmpSong.StarCount / 10;
                    MuzukashiiDifFound = true;
                    break;
                case Taiko_Difficulty.Dif_Oni:
                    OniBar.fillAmount = (float)tmpSong.StarCount / 10;
                    OniDifFound = true;
                    break;
            }
        }

        SongName.text = CurrentSong.TitleName;
        CheckForAvailableDif();

        StartCoroutine(TaikoStaticExtension.LoadAndPlaySong(MainAudioPlayer, CurrentSong));
    }

    void CheckForAvailableDif()
    {
        if(!KantanDifFound)
        {
            KantanButton.interactable = false;
        }

        if (!FutsuDifFound)
        {
            FutsuButton.interactable = false;
        }

        if (!MuzukashiiDifFound)
        {
            MuzukashiiButton.interactable = false;
        }

        if (!OniDifFound)
        {
            OniButton.interactable = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

        
    }


    public void OnDiffSelected(Button SelectedButton)
    {
        if(!SelectorIsMoving)
        {
            StopCoroutine(MoveSelector(0, 0));
            StartCoroutine(MoveSelector(SelectedButton.transform.position.x, .5f));
        }
            
    }
    public IEnumerator MoveSelector(float NewX, float timeToMove)
    {
        var currentPos = DiffSelector.transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            SelectorIsMoving = true;
            DiffSelector.transform.position = Vector3.Lerp(currentPos, new Vector3(NewX, DiffSelector.transform.position.y, DiffSelector.transform.position.z), t);
            //DiffSelector.transform.position = Vector3.MoveTowards(currentPos, new Vector3(NewX, DiffSelector.transform.position.y, DiffSelector.transform.position.z),10 * Time.deltaTime);
            yield return null;
        }
        SelectorIsMoving = false;
    }

    public void OnSubmitDiff(int SelectedDif)
    {
        TaikoStaticExtension.SetFadeState(true);
    }

}
