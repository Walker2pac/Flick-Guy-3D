using UnityEngine;
using System.Collections;

public class Break : MonoBehaviour 
{
    [SerializeField] public SettingsProject settingsProject;
    public Transform brokenObject;
    public AudioSource sound;
    public AudioSource soundFly;
    public AudioSource soundCity;
    public BoxCollider bc;
    public BoxCollider bcT;

    bool use;
    bool fly;
    float distanceSound;

    private void Start()
    {
        soundCity.maxDistance = settingsProject.GlassplateSoundDistance;
    }

    void OnCollisionEnter(Collision collision)//
	{
        //Destroy(gameObject);
        //Instantiate(brokenObject, transform.position, transform.rotation);
        if (use) return;
        use = true;
        fly = true;
        Invoke("OffFly", 0.1f);
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
        Physics.IgnoreCollision(collision.collider, bc);
        transform.GetChild(0).gameObject.SetActive(true);
        sound.Play();
        soundCity.Play();
        Destroy(transform.GetChild(0).gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterJoint>() != null)
        {
            DoubleEnemy de = other.gameObject.GetComponentInParent<DoubleEnemy>();

            if (de != null)
            {
                de.death = true;
                de.deathFull = true;
            }
        }

        if (other.gameObject.name == "pelvis" && use && !fly)
        {
            Physics.IgnoreCollision(other, bc);
            Physics.IgnoreCollision(other, bcT);
            soundFly.Play();
        }
    }

    void OffFly()
    {
        fly = false;
    }
}
