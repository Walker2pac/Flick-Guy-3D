using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    GamePlay gp;
    GameObject leg;
    public Rigidbody rb;
    public AudioSource sound;
    public Transform transf;

    bool use;

    [HideInInspector]
    public bool danger;

    Transform trans;
    Transform playerTrans;
    Vector3 pos;
    bool time;

    void Awake()
    {
        trans = transform;
        //rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.velocity != new Vector3(0, 0, 0) || trans.rotation.x == 0 || trans.rotation.x == 180 || trans.rotation.x == -180 || !time) danger = true;
        else danger = false;

        if(use) Debug.DrawRay(transform.position, pos, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !use)
        {
            pos = (other.gameObject.transform.position - transform.position).normalized;
            gp = GameObject.FindGameObjectWithTag("GamePlay").gameObject.GetComponent<GamePlay>();
            use = true;
            rb.isKinematic = false;
            playerTrans = other.transform;
            gp.door = true;
            gp.splineFollower.follow = false;
            gp.cameraController.playerLook = false;
            gp.doorTrans = transform;
            if(gp == null) Destroy(gameObject);
            leg = other.transform.GetChild(0).GetChild(2).gameObject;
            leg.SetActive(true);
            leg.GetComponent<Animator>().SetInteger("IndexAnim", 1);
            Invoke("PushDoor", 0.25f);
            Invoke("Time", 1.25f);
        }
        else if(other.tag == "Player" && use) Physics.IgnoreCollision(GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if((collision.gameObject.GetComponent<CharacterJoint>() != null || collision.gameObject.GetComponent<Rigidbody>() != null) && !use || (use && !danger))
        //{
        //    //PushDoorEnemy();
        //    if (collision.gameObject.GetComponent<Rigidbody>() != null) collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //    rb.isKinematic = true;
        //    Invoke("PushDoorEnemy", 0.1f);
        //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        //    rb.velocity = new Vector3(0, 0, 0);
        //}

        //if (collision.gameObject.GetComponent<CharacterJoint>() != null)
        //{
        //    if (collision.gameObject.GetComponent<Rigidbody>() != null) collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //    rb.isKinematic = true;
        //    Invoke("PushDoorEnemy", 0.1f);
        //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        //    rb.velocity = new Vector3(0, 0, 0);
        //}

        //if(!use && !collision.gameObject.name.Contains("Cube"))
        //{
        //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        //    rb.velocity = new Vector3(0, 0, 0);
        //}

        //if (!use && collision.gameObject.name.Contains("Enemy"))
        //{
        //    Debug.Log(1);
        //    if (collision.gameObject.GetComponent<Rigidbody>() != null) collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //    rb.isKinematic = true;
        //    Invoke("PushDoorEnemy", 0.1f);
        //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        //    rb.velocity = new Vector3(0, 0, 0);
        //}

        //if (collision.gameObject.name.Contains("piece") && !use)
        //{
        //    rb.isKinematic = true;
        //    Invoke("PushDoorEnemy", 1f);
        //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        //    rb.velocity = new Vector3(0, 0, 0);
        //}
    } 

    void Time()
    {
        time = true;
    }

    public void PushDoor()
    {
        leg.GetComponent<Animator>().SetInteger("IndexAnim", 0);
        Invoke("End", 0.3f);
        gp.cameraController.playerLook = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.freezeRotation = false;
        //rb.AddForce(-(Camera.main.transform.position - transform.position) * 7, ForceMode.Impulse);
        rb.velocity = new Vector3(0, 0, 0);
        GetComponent<BoxCollider>().isTrigger = true;
        //rb.AddForce((-(Camera.main.transform.position - transf.position).normalized + new Vector3(0, 0.5f, 0)) * 10f, ForceMode.VelocityChange);
        rb.AddForce(-pos * 10f, ForceMode.VelocityChange);
        sound.Play();
        gp.door = false;
        gp.splineFollower.follow = true;
    }

    void End()
    {
        GetComponent<BoxCollider>().isTrigger = false;
        leg.SetActive(false);
        gp.door = false;
        gp.splineFollower.follow = true;
    }

    void PushDoorEnemy()
    {
        rb.isKinematic = false;
    }
}
