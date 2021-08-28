using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLevelMsgText : MonoBehaviour
{
    public bool start;
    public bool next;
    public bool restart;

    private void Update()
    {
        LevelManagement lm = GameObject.FindGameObjectWithTag("LevelManagement").GetComponent<LevelManagement>();
        if(start && !next && !restart) GetComponent<TextMeshProUGUI>().text = "LEVEL " + lm.completedLevels.ToString();
        else if(!start && next && !restart) GetComponent<TextMeshProUGUI>().text = "LEVEL " + lm.completedLevels.ToString() + " PASSED!";
        else if(!start && !next && restart) GetComponent<TextMeshProUGUI>().text = "LEVEL " + lm.completedLevels.ToString() + " FAILED!";
    }
}
