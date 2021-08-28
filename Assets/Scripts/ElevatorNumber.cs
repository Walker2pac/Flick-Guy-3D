using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorNumber : MonoBehaviour
{
    bool completed;

    private void Update()
    {
        if(!completed)
        {
            LevelManagement lm = GameObject.FindGameObjectWithTag("LevelManagement").GetComponent<LevelManagement>();
            GetComponent<TextMesh>().text = lm.completedLevels.ToString();
            Invoke("Completed", 0.1f);
        }
    }

    void Completed()
    {
        completed = true;
    }
}
