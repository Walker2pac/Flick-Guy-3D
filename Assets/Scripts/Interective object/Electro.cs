using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electro : MonoBehaviour
{
    public float timeEnd;
    public GameObject sparkEffect;
    public AudioSource audioStart;
    public AudioSource audioElectro;
    GameObject sparkEnemy;
    Transform transSparkEnemy;
    bool use;

    private void FixedUpdate()
    {
        if(sparkEnemy != null)
        {
            switch (sparkEnemy.tag)
            {
                case "UsuallyEnemy":
                    UsuallyEnemy ue = sparkEnemy.GetComponent<UsuallyEnemy>();
                    
                    ue.leftArm.GetComponent<Rigidbody>().isKinematic = false;
                    ue.leftArm.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    ue.rigthArm.GetComponent<Rigidbody>().isKinematic = false;
                    ue.rigthArm.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    ue.leftLeg.GetComponent<Rigidbody>().isKinematic = false;
                    ue.leftLeg.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    ue.rigthLeg.GetComponent<Rigidbody>().isKinematic = false;
                    ue.rigthLeg.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    break;

                case "DoubleEnemy":
                    DoubleEnemy de = sparkEnemy.GetComponent<DoubleEnemy>();

                    de.leftArm.GetComponent<Rigidbody>().isKinematic = false;
                    de.leftArm.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    de.rigthArm.GetComponent<Rigidbody>().isKinematic = false;
                    de.rigthArm.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    de.leftLeg.GetComponent<Rigidbody>().isKinematic = false;
                    de.leftLeg.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);

                    de.rigthLeg.GetComponent<Rigidbody>().isKinematic = false;
                    de.rigthLeg.Translate(new Vector2(Random.Range(-4, 5), Random.Range(-4, 5)) * Time.deltaTime);
                    break;

                default:
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "UsuallyEnemy":
                if(collision.gameObject.GetComponent<UsuallyEnemy>().death == true && !use)
                {
                    sparkEffect.SetActive(true);
                    UsuallyEnemy ue = collision.gameObject.GetComponent<UsuallyEnemy>();
                    ue.rb.isKinematic = true;
                    ue.electro = true;
                    sparkEnemy = collision.gameObject;
                    transSparkEnemy = collision.transform;
                    audioStart.Play();
                    audioElectro.Play();
                    for (int i = 0; i < ue.GetRigidbodies.Count; i++)
                    {
                        ue.GetRigidbodies[i].isKinematic = true;
                    }
                    use = true;
                    GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = true;
                    Invoke("End", timeEnd);
                }
                break;

            case "DoubleEnemy":
                if (collision.gameObject.GetComponent<DoubleEnemy>().death == true && !use)
                {
                    sparkEffect.SetActive(true);
                    DoubleEnemy de = collision.gameObject.GetComponent<DoubleEnemy>();
                    de.rb.isKinematic = true;
                    de.electro = true;
                    sparkEnemy = collision.gameObject;
                    transSparkEnemy = collision.transform;
                    audioStart.Play();
                    audioElectro.Play();
                    for (int i = 0; i < de.GetRigidbodies.Count; i++)
                    {
                        de.GetRigidbodies[i].isKinematic = true;
                    }
                    use = true;
                    GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = true;
                    Invoke("End", timeEnd);
                }
                break;

            default:
                break;
        }

        if(collision.gameObject.GetComponent<CharacterJoint>() != null)
        {
            UsuallyEnemy ue = collision.gameObject.GetComponentInParent<UsuallyEnemy>();
            DoubleEnemy de = collision.gameObject.GetComponentInParent<DoubleEnemy>();

            if (ue != null)
            {
                if (ue.death == true && !use)
                {
                    sparkEffect.SetActive(true);
                    ue.rb.isKinematic = true;
                    ue.electro = true;
                    sparkEnemy = ue.gameObject;
                    transSparkEnemy = ue.transform;
                    audioStart.Play();
                    audioElectro.Play();
                    for (int i = 0; i < ue.GetRigidbodies.Count; i++)
                    {
                        ue.GetRigidbodies[i].isKinematic = true;
                    }
                    use = true;
                    GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = true;
                    Invoke("End", timeEnd);
                }
            }
            else if (de != null)
            {
                if (de.death == true && !use)
                {
                    sparkEffect.SetActive(true);
                    de.rb.isKinematic = true;
                    de.electro = true;
                    sparkEnemy = de.gameObject;
                    transSparkEnemy = de.transform;
                    audioStart.Play();
                    audioElectro.Play();
                    for (int i = 0; i < de.GetRigidbodies.Count; i++)
                    {
                        de.GetRigidbodies[i].isKinematic = true;
                    }
                    use = true;
                    GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = true;
                    Invoke("End", timeEnd);
                }
            }
        }
    }

    void End()
    {
        switch (sparkEnemy.tag)
        {
            case "UsuallyEnemy":
                sparkEffect.SetActive(false);
                UsuallyEnemy ue = sparkEnemy.GetComponent<UsuallyEnemy>();
                ue.rb.isKinematic = false;
                audioElectro.Stop();
                for (int i = 0; i < ue.GetRigidbodies.Count; i++)
                {
                    ue.GetRigidbodies[i].isKinematic = false;
                    ue.GetRigidbodies[i].AddForce(-Vector3.forward * 0.2f, ForceMode.Impulse);
                }
                GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = false;
                sparkEnemy = null;
                break;

            case "DoubleEnemy":
                sparkEffect.SetActive(false);
                DoubleEnemy de = sparkEnemy.GetComponent<DoubleEnemy>();
                de.rb.isKinematic = false;
                de.deathFull = true;
                audioElectro.Stop();
                for (int i = 0; i < de.GetRigidbodies.Count; i++)
                {
                    de.GetRigidbodies[i].isKinematic = false;
                    de.GetRigidbodies[i].AddForce(-Vector3.forward * 0.2f, ForceMode.Impulse);
                }
                GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().electro = false;
                sparkEnemy = null;
                break;

            default:
                break;
        }
    }
}
