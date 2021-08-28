using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsController : MonoBehaviour
{
    public GamePlay gp;

    public void Attack()
    {
        gp.Attack();
    }

    public void OffAttackHim()
    {
        gp.OffAttackAnim();
    }
}
