using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DrumGameObjectStruc
{
    public GameObject LeftKa;
    public GameObject RightKa;
    public GameObject LeftDon;
    public GameObject RightDon;
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
    void Start()
    {
        TGS = GameObject.FindObjectOfType<TaikoSongPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check For Player In tick because of unity (u_u)

        //Checking For Left Don
        if(Input.GetButtonDown("LeftDon"))
        {
            DrumGameObjectList.LeftDon.GetComponent<MeshRenderer>().enabled = true;
            TGS.RegisterInput(DrumInputType.LeftDon);
            DrumGameObjectList.LeftDon.GetComponent<AudioSource>().Play();
        }
        if(Input.GetButtonUp("LeftDon"))
        {
            DrumGameObjectList.LeftDon.GetComponent<MeshRenderer>().enabled = false;
        }

        //Checking For Right Don
        if (Input.GetButtonDown("RightDon"))
        {
            DrumGameObjectList.RightDon.GetComponent<MeshRenderer>().enabled = true;
            TGS.RegisterInput(DrumInputType.RightDon);
            DrumGameObjectList.RightDon.GetComponent<AudioSource>().Play();
        }
        if (Input.GetButtonUp("RightDon"))
        {
            DrumGameObjectList.RightDon.GetComponent<MeshRenderer>().enabled = false;
        }

        //Checking For Left Ka
        if (Input.GetButtonDown("LeftKa"))
        {
            DrumGameObjectList.LeftKa.GetComponent<MeshRenderer>().enabled = true;
            TGS.RegisterInput(DrumInputType.LeftKa);
            DrumGameObjectList.LeftKa.GetComponent<AudioSource>().Play();
        }
        if (Input.GetButtonUp("LeftKa"))
        {
            DrumGameObjectList.LeftKa.GetComponent<MeshRenderer>().enabled = false;
        }

        //Checking For Right Ka
        if (Input.GetButtonDown("RightKa"))
        {
            DrumGameObjectList.RightKa.GetComponent<MeshRenderer>().enabled = true;
            TGS.RegisterInput(DrumInputType.RightKa);
            DrumGameObjectList.RightKa.GetComponent<AudioSource>().Play();
        }
        if (Input.GetButtonUp("RightKa"))
        {
            DrumGameObjectList.RightKa.GetComponent<MeshRenderer>().enabled = false;
        }


    }
}
