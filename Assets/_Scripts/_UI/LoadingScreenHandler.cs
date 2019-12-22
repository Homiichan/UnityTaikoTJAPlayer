using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI SongName;

    private TaikoGameInstance TGI;

    Animator LoadingAnimator;
    void Start()
    {
        DontDestroyOnLoad(this);
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        LoadingAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeIn()
    {
        SongName.text = TGI.CurrentSelectedSong.TitleName;
        LoadingAnimator.SetBool("FadeIN", true);
        //StartCoroutine(DebugTest());
;    }

    public void FadeOut()
    {
        LoadingAnimator.SetBool("FadeIN", false);
    }

    public IEnumerator DebugTest()
    {
        yield return new WaitForSeconds(5);
        FadeOut();
    }
}
