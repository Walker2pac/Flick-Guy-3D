using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperPunch : MonoBehaviour
{
    [SerializeField] public SettingsProject settingsProject;
    public float needDeath;
    public float currentDeath;
    public Image processing;
    public AudioSource sound;
    public HandsController hand;

    float step;
    [HideInInspector]
    public bool punch;
    [HideInInspector]
    public bool boom;
    bool use;

    private void Start()
    {
        hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<HandsController>();
        gameObject.SetActive(false);
        needDeath = settingsProject.DeathForSuperPunch;
        step = 1 / needDeath;
    }

    private void Update()
    {
        processing.fillAmount = currentDeath * step;

        if (currentDeath >= needDeath) currentDeath = needDeath;

        if (punch)
        {
            Mathf.Clamp(currentDeath -= 50 * Time.deltaTime, 0, needDeath);
            Time.timeScale = 0.078f;
            Time.fixedDeltaTime = Time.deltaTime;
        }
        else if(!boom)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.deltaTime;
        }

        if(use && !punch)
        {
            currentDeath = 0;
        }
    }

    public void Punch()
    {
        if (currentDeath < needDeath) return;
        Invoke("OffUse", 2f);
        punch = true;
        use = true;
        GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().superPunch = true;
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        Invoke("Sound", 0f);
    }

    void Sound()
    {
        sound.Play();
    }

    void OffUse()
    {
        use = false;
    }
}
