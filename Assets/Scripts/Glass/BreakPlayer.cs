using UnityEngine;
using System.Collections;

public class BreakPlayer : MonoBehaviour 
{
	public Transform brokenObject;
	public bool brokenByPlayer;
	public AudioSource sound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<MeshRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Transform trans = transform.GetChild(0);
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).GetComponent<Rigidbody>().AddForce(-(other.transform.position - trans.GetChild(i).position) * 1.5f, ForceMode.Impulse);
            }
            sound.Play();
            Destroy(gameObject, 3f);
        }
    }
}
