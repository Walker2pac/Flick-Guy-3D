using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public AudioSource audioAttack;
    public AudioSource audioAttackCup;
    public AudioSource audioStart;

    Rigidbody rb;

    private void Start()
    {
        audioStart.Play();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
    }
}
