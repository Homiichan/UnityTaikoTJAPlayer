using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("SongSelect GameObject")]
    public ScrollRectSnap SongSelect;

    private AudioSource MainAudioPlayer;
    private bool KantanDifFound = false;
    private bool FutsuDifFound = false;
    private bool MuzukashiiDifFound = false;
    private bool OniDifFound = false;

    private TaikoGameInstance TGI;

    private bool SelectorIsMoving = false;

    public List<GameObject> CompToFade; 

    // Start is called before the first frame update
    void Start()
    {
        MainAudioPlayer = TaikoStaticExtension.GetMainSongPlayer();
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        SetupSong();
        GetAllCompToFade();
        StartCoroutine(LerpAlphaFloat(1, 0, 0));
    }



    void GetAllCompToFade()
    {
        for(int i = 0; i <= transform.childCount - 1; i++)
        {
            if(transform.GetChild(i).tag != "FadeIgnoreFlag")
            {
                CompToFade.Add(transform.GetChild(i).gameObject);
            }
        }
    }
    public void SetupSong()
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

    private void OnEnable()
    {
        SetupSong();
    }
    public void StartFadeIn(float timeToFade)
    {
        StartCoroutine(LerpAlphaFloat(0, 1, timeToFade));
    }

    public void StartFadeOut(float timeToFade)
    {
        StartCoroutine(LerpAlphaFloat(1, 0, timeToFade));
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

    public void OnBackToSongSelect()
    {
        SongSelect.SwitchToFullScreenMode(false);
        //SongSelect.gameObject.SetActive(true);
        //this.gameObject.SetActive(false);
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

    public IEnumerator LerpAlphaFloat(float Origin, float targetAlpha, float timeToMove)
    {
        var currentPos = Origin;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            SelectorIsMoving = true;
            float tmpLerpedFloat = Mathf.Lerp(Origin, targetAlpha, t);
            SetFadeValueToComp(tmpLerpedFloat);
            yield return null;
        }
        SelectorIsMoving = false;
    }

    void SetFadeValueToComp(float AlphaValue)
    {
        foreach(GameObject go in CompToFade)
        {
            if(go.GetComponent<Image>())
            {
                go.GetComponent<Image>().color = new Color(go.GetComponent<Image>().color.r, go.GetComponent<Image>().color.g, go.GetComponent<Image>().color.b, AlphaValue);
            }
            else if(go.GetComponent<TextMeshProUGUI>())
            {
                go.GetComponent<TextMeshProUGUI>().color = new Color(go.GetComponent<TextMeshProUGUI>().color.r, go.GetComponent<TextMeshProUGUI>().color.g, go.GetComponent<TextMeshProUGUI>().color.b, AlphaValue);
            }
        }
    }
    public void OnSubmitDiff(int SelectedDif)
    {
        TGI.FindSongAndDifficulty(0, TaikoStaticExtension.GetTaikoDiffByInt(SelectedDif));
        TaikoStaticExtension.SetFadeState(true);
        Debug.Log("selected dif = " + TaikoStaticExtension.GetTaikoDiffByInt(SelectedDif));
        StartCoroutine(WaitBeforeFade());

    }

    void LoadLevel()
    {
        SceneManager.LoadScene(2);
    }

    public IEnumerator WaitBeforeFade()
    {
        yield return new WaitForSeconds(1.5f);
        LoadLevel();

    }
}
