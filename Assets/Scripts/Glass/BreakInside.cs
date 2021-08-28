using UnityEngine;
using System.Collections;

public class BreakInside : MonoBehaviour 
{
	public Transform brokenObject;
    public AudioSource sound;

    bool use;

    void OnCollisionEnter(Collision collision)//
	{
        //Destroy(gameObject);
        //Instantiate(brokenObject, transform.position, transform.rotation);
        if (use) return;
        use = true;
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        sound.Play();
        Destroy(gameObject, 8f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.transform.tag == "DoubleEnemy")
        //{
        //    other.gameObject.GetComponent<DoubleEnemy>().death = true;
        //    other.gameObject.GetComponent<DoubleEnemy>().deathFull = true;
        //}
    }
}
