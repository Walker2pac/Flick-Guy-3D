using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVent : MonoBehaviour
{
    public Vent[] vent;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CharacterJoint>() != null)
        {
            foreach (Vent item in vent)
            {
                item.work = true;
            }
        }
    }
}
