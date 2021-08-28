using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupByEnemy : MonoBehaviour
{
    [HideInInspector]
    public Transform headEnemy;
    [HideInInspector]
    public GamePlay gp;
    [HideInInspector]
    public float power;

    bool back;

    private void Start()
    {
        gp = GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>();
        Invoke("OffTrigger", 0.2f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "MainCamera" && !back)
        {
            back = true;
            gp.attackCup = true;
            gp.attackCupObj = transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" && gp.attack == true && !back)
        {
            gp.Damage(1);

            if (gp.death)
            {
                CupEnemy ce = headEnemy.GetComponentInParent<CupEnemy>();
                if (ce != null) gp.LookToward(headEnemy);
            }

            back = true;
        }
    }

    void OffTrigger()
    {
        GetComponent<CapsuleCollider>().isTrigger = false;
    }
}
