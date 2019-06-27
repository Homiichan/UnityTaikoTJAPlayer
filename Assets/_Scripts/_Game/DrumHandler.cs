using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DrumGameObjectStruc
{
    public GameObject LeftKa;
    public GameObject RightKa;
    public GameObject LeftDon;
    public GameObject RightDon;
}

[System.Serializable]
public struct LaneEffectSpriteStruc
{
    public Sprite KaSprite;
    public Sprite DonSprite;
}

public enum DrumInputType
{
    LeftKa,
    RightKa,
    LeftDon,
    RightDon
}
public class DrumHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public DrumGameObjectStruc DrumGameObjectList;
    TaikoSongPlayer TGS;
    GameObject LaneEffect;
    public LaneEffectSpriteStruc LESS;
    bool lerp = false;
    float t = 0.0f;
    void Start()
    {
        TGS = GameObject.FindObjectOfType<TaikoSongPlayer>();
        LaneEffect = GameObject.FindGameObjectWithTag("PlayerLane");
    }

    // Update is called once per frame
    void Update()
    {
        //Check For Player In tick because of unity (u_u)


        //Checking For Left Don
        if(Input.GetButtonDown("LeftDon"))
        {
            DrumGameObjectList.LeftDon.GetComponent<Image>().enabled = true;
            TGS.RegisterInput(DrumInputType.LeftDon);
            DrumGameObjectList.LeftDon.GetComponent<AudioSource>().Play();
            LaneEffect.GetComponent<Image>().sprite = LESS.DonSprite;
            LaneEffect.GetComponent<Animation>().Stop();
            LaneEffect.GetComponent<Animation>().Play();
        }
        if(Input.GetButtonUp("LeftDon"))
        {
            DrumGameObjectList.LeftDon.GetComponent<Image>().enabled = false;
        }

        //Checking For Right Don
        if (Input.GetButtonDown("RightDon"))
        {
            DrumGameObjectList.RightDon.GetComponent<Image>().enabled = true;
            TGS.RegisterInput(DrumInputType.RightDon);
            DrumGameObjectList.RightDon.GetComponent<AudioSource>().Play();
            LaneEffect.GetComponent<Image>().sprite = LESS.DonSprite;
            LaneEffect.GetComponent<Animation>().Stop();
            LaneEffect.GetComponent<Animation>().Play();
        }
        if (Input.GetButtonUp("RightDon"))
        {
            DrumGameObjectList.RightDon.GetComponent<Image>().enabled = false;
        }

        //Checking For Left Ka
        if (Input.GetButtonDown("LeftKa"))
        {
            DrumGameObjectList.LeftKa.GetComponent<Image>().enabled = true;
            TGS.RegisterInput(DrumInputType.LeftKa);
            DrumGameObjectList.LeftKa.GetComponent<AudioSource>().Play();
            LaneEffect.GetComponent<Image>().sprite = LESS.KaSprite;
            LaneEffect.GetComponent<Animation>().Stop();
            LaneEffect.GetComponent<Animation>().Play();
        }
        if (Input.GetButtonUp("LeftKa"))
        {
            DrumGameObjectList.LeftKa.GetComponent<Image>().enabled = false;
        }

        //Checking For Right Ka
        if (Input.GetButtonDown("RightKa"))
        {
            DrumGameObjectList.RightKa.GetComponent<Image>().enabled = true;
            TGS.RegisterInput(DrumInputType.RightKa);
            LaneEffect.GetComponent<Image>().sprite = LESS.KaSprite;
            LaneEffect.GetComponent<Animation>().Stop();
            LaneEffect.GetComponent<Animation>().Play();
            DrumGameObjectList.RightKa.GetComponent<AudioSource>().Play();
        }
        if (Input.GetButtonUp("RightKa"))
        {
            DrumGameObjectList.RightKa.GetComponent<Image>().enabled = false;
        }


    }
}
