using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class FireExtinguisher : MonoBehaviour
{
    public float forceExplosion;
    public float timeSlowMotion;
    public GameObject effectExplosion;
    public MeshRenderer meshRenderer;
    public AudioSource sound;

    bool boom;

    private void Start()
    {
        effectExplosion.GetComponent<ExplosionPhysicsForce>().explosionForce = forceExplosion;
    }

    private void OnTriggerEnter(Collider other)
    {
        //switch (other.tag)
        //{
        //    case "UsuallyEnemy":
        //        Time.timeScale = 0.25f;
        //        meshRenderer.enabled = false;
        //        effectExplosion.SetActive(true);
        //        Invoke("Off", timeSlowMotion);
        //        break;

        //    default:
        //        break;
        //}
    }

    private void Update()
    {
        if (boom)
        {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.25f * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "UsuallyEnemy":
            case "DoubleEnemy":
            case "CupEnemy":
            case "CupByEnemy":
            case "UsuallyBoss":
            case "ChikenBoss":
                if (boom) break;
                boom = true;
                if (GameObject.FindGameObjectWithTag("SuperPunch") != null) GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().boom = true;
                meshRenderer.enabled = false;
                effectExplosion.SetActive(true);
                Invoke("Off", timeSlowMotion);
                if(collision.gameObject.tag == "DoubleEnemy") collision.gameObject.GetComponent<DoubleEnemy>().deathFull = true;
                sound.Play();
                break;

            default:
                break;
        }

        if(collision.gameObject.GetComponent<CharacterJoint>() != null && !boom)
        {
            GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().boom = true;
            sound.Play();
            meshRenderer.enabled = false;
            effectExplosion.SetActive(true);
            Invoke("Off", timeSlowMotion);
            boom = true;
        }
    }

    void Off()
    {
        GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().boom = false;
        boom = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.deltaTime;
        effectExplosion.SetActive(false);
        Destroy(gameObject);
    }
}
