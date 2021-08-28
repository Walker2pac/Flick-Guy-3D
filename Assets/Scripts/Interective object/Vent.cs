using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
    public bool work;
    public float speed;
    public Transform vint;
    public GameObject effects;
    public float timeWork;

    bool addDopTime;

    private void Update()
    {
        if(work)
        {
            effects.SetActive(true);
            vint.Rotate(0, speed * Time.deltaTime, 0);
            Invoke("OffWork", timeWork);
        }
    }

    void OffWork()
    {
        work = false;
        effects.SetActive(false);
        CancelInvoke("OffWork");
        addDopTime = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(work)
        {
            switch (other.tag)
            {
                case "UsuallyEnemy":
                    UsuallyEnemy ue = other.GetComponent<UsuallyEnemy>();
                    ue.rb.velocity = new Vector3(0, ue.rb.velocity.y, 0);
                    ue.vent = true;
                    ue.attack = false;
                    if (!addDopTime)
                    {
                        addDopTime = true;
                        other.transform.parent.GetComponent<ObjectGroup>().dopTime += 1f;
                    }
                    break;

                case "CupEnemy":
                    CupEnemy ce = other.GetComponent<CupEnemy>();
                    ce.rb.velocity = new Vector3(0, ce.rb.velocity.y, 0);
                    ce.vent = true;
                    ce.attack = false;
                    if (!addDopTime)
                    {
                        addDopTime = true;
                        other.transform.parent.GetComponent<ObjectGroup>().dopTime += 1f;
                    }
                    break;

                case "DoubleEnemy":
                    DoubleEnemy de = other.GetComponent<DoubleEnemy>();
                    de.rb.velocity = new Vector3(0, de.rb.velocity.y, 0);
                    de.vent = true;
                    de.attack = false;
                    if (!addDopTime)
                    {
                        addDopTime = true;
                        other.transform.parent.GetComponent<ObjectGroup>().dopTime += 1f;
                    }
                    break;

                case "UsuallyBoss":
                    UsuallyBoss ub = other.GetComponent<UsuallyBoss>();
                    ub.rb.velocity = new Vector3(0, ub.rb.velocity.y, 0);
                    ub.vent = true;
                    ub.attack = false;
                    if (!addDopTime)
                    {
                        addDopTime = true;
                        other.transform.parent.GetComponent<ObjectGroup>().dopTime += 1f;
                    }
                    break;

                default:
                    break;
            }

            if(other.GetComponent<CharacterJoint>() != null)
            {
                other.GetComponent<Rigidbody>().velocity = new Vector3(0, other.GetComponent<Rigidbody>().velocity.y, 0);
                other.GetComponent<Rigidbody>().AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (work)
        {
            if (other.GetComponent<CharacterJoint>() != null)
            {
                other.GetComponent<Rigidbody>().AddForce(Vector3.up * 1.75f, ForceMode.Impulse);
            }
        }
    }
}
