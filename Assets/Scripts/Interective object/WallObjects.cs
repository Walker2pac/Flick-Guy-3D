using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObjects : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "UsuallyEnemy":
            case "CupEnemy":
            case "DoubleEnemy":
            case "UsuallyBoss":
                GetComponent<BoxCollider>().isTrigger = false;
                GetComponent<Rigidbody>().useGravity = true;
                break;

            default:
                break;
        }
    }
}
