using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public AudioSource sound;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenElevator()
    {
        sound.Play();
        anim.SetBool("Open", true);
        anim.SetBool("Close", false);
    }

    public void CloseElevator()
    {
        sound.Play();
        anim.SetBool("Close", true);
        anim.SetBool("Open", false);
    }
}
