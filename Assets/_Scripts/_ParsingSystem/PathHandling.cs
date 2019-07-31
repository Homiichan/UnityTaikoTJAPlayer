using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using SFB;
using UnityEngine.UI.ScrollSnaps;

public class PathHandling : MonoBehaviour
{
    public TMP_InputField FileInputField;
    public TJAParser TJAParser;
    public Button BrowseFolderButton;
    string TJASongPath = "";
    public DirectionalScrollSnap DSS;
    // Start is called before the first frame update
    LevelTemplate CurrentSelectedLevel;
    //SongLoader SL;
    void Start()
    {
        TJAParser = GameObject.FindObjectOfType<TJAParser>();
        GetDefaultSongPath();
        FileInputField.onValueChanged.AddListener(delegate { OnValueChange(FileInputField.text); });
        DSS.snappedToItem.AddListener(delegate { ItemSelected(DSS.closestSnapPositionIndex); });
    }

    void GetDefaultSongPath()
    {
        string tmpPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        tmpPath = tmpPath + "\\TaikoSim\\";
        if (!Directory.Exists(tmpPath))
        {
            //if it doesn't, create it

            Directory.CreateDirectory(tmpPath);
            if (!Directory.Exists(tmpPath + @"\Songs\"))
            {
                Directory.CreateDirectory(tmpPath + @"\Songs\");
            }

        }
        FileInputField.text = tmpPath + @"Songs\";
        TJASongPath = tmpPath + @"Songs\";
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("RightDon"))
        {
            DSS.OnForward();
        }
        if (Input.GetButtonDown("LeftDon"))
        {
            DSS.OnBack();
        }
    }
    public void ItemSelected(int selected)
    {
        Debug.Log(selected);
        RectTransform temp;
        DSS.GetChildAtSnapIndex(selected, out temp);
        Debug.Log(temp.GetComponent<LevelTemplate>().text.text);
        temp.GetComponent<LevelTemplate>().OnDeselected();
        CurrentSelectedLevel = temp.GetComponent<LevelTemplate>();
        CurrentSelectedLevel.OnSelected();

    }
    public void OnValueChange(string Path)
    {
        TJASongPath = Path;
    }
    public void OnPathSelected(Button Button)
    {
        TJAParser.GetAllTJAFileInFolder(TJASongPath);
        Destroy(FileInputField.gameObject);
        Destroy(Button.gameObject);
        Destroy(BrowseFolderButton.gameObject);
    }

    public void OpenFilePath()
    {
        TJASongPath = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true)[0];
        if (TJASongPath[TJASongPath.Length - 1] != '\\')
        {
            TJASongPath = TJASongPath + "\\";
        }
        FileInputField.text = TJASongPath;
    }
}
