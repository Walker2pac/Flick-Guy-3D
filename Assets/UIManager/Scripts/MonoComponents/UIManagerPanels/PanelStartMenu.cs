using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using DG.Tweening;


public class PanelStartMenu : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private Button buttonStart;


    private void OnEnable()
    {

    }

    public void Start()
    {
        buttonStart.onClick.AddListener(HandleButtonStartClick);
    }


    private void HandleButtonStartClick()
    {
        UIManager.Default.CurState = UIManager.State.Play;

    }

    private void OnDisable()
    {

    }
}


