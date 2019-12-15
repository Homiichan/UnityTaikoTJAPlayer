using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class songDifficulty : MonoBehaviour
{
    // Start is called before the first frame update
    public TaikoSongStruc diffContained;

    public Image[] Stars;

    void Start()
    {
        for(int i = 0; i <= diffContained.StarCount - 1 ; i++)
        {
            Stars[i].enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
