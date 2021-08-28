using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    public static UIManager Default => _default;
    private static UIManager _default;


    public Panel panelProgress;

    public Panel panelLevelFailed;

    public Panel panelLevelWin;

    public Panel panelMainMenu;

    public GameObject superBtn;

    public Canvas mainCanvas;


    public enum State { None, MainMenu, Play, Failed, Win }

    public State CurState
    {
        get => curState;
        set
        {
            if (curState == value ||
            !statesMap.ContainsKey((int)value) ||
            !statesMap[(int)value].Condition((int)curState))
                return;
            statesMap[(int)curState].OnEnd((int)value);
            statesMap[(int)value].OnStart();
            curState = value;
        }
    }
    private State curState;
    private Dictionary<int, StateDefault> statesMap = new Dictionary<int, StateDefault>();
    public UIManager()
    {
        _default = this;

        statesMap.AddState((int)State.None, () => { }, (a) => { });

        SetupStateMainMenu();
        SetupStatePlay();
        SetupStateFailed();
        SetupStateWin();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SetHighlightUIElement(GameObject go, bool arg)
    {

        Canvas canvas = null;
        if (arg)
        {
            canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
        }
        else
        {
            canvas = go.GetComponent<Canvas>();
            GameObject.Destroy(canvas);
        }
    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().start = true;
    }

    public void Restart()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("WarningEnemy")) Destroy(transform.GetChild(i).gameObject);
        }
    }
}

