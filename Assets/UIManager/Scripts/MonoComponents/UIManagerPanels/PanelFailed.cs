using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;



public class PanelFailed : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private Button buttonRestart;

    private void OnEnable()
    {
        buttonRestart.gameObject.SetActive(true);
    }


    public void Start()
    {
        buttonRestart.onClick.AddListener(HandleButtonRestartClick);
    }

    private void HandleButtonRestartClick()
    {

    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

