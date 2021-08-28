using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class ExplosionPhysicsForce : MonoBehaviour
    {
        public float explosionForce = 4;
        public AudioSource soundDamage;

        private IEnumerator Start()
        {
            // wait one frame because some explosions instantiate debris which should then
            // be pushed by physics force
            yield return null;

            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;

            float r = 10*multiplier;
            var cols = Physics.OverlapSphere(transform.position, r);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                RaycastHit hit = new RaycastHit();
                Vector3 pos = (col.gameObject.transform.position - transform.parent.position).normalized;
                if (Physics.Raycast(transform.parent.position, pos, out hit))
                {
                    //Debug.Log(col.gameObject.name + " - " + hit2.transform.name + " - " + col.transform.parent.name);
                    if (hit.transform.name.Contains("wall") || hit.transform.name.Contains(".")
                        || hit.transform.name.Contains("door") || hit.transform.name.Contains("column")
                        || hit.transform.name.Contains("Electro"))
                    {
                        if(hit.transform.gameObject.layer == 2)
                        {
                            if (Vector3.Distance(transform.parent.position, hit.point) <
                            Vector3.Distance(transform.parent.position, col.transform.position)) continue;
                        }
                        else continue;
                    }
                    else if (hit.transform.name.Contains("Enemy"))
                    {
                        bool enem = true;
                        int def = cols.Length;
                        while (enem)
                        {
                            if (Physics.Raycast(transform.position, pos, out hit))
                            {
                                if (!hit.transform.name.Contains("Enemy") || hit.transform.gameObject == col.gameObject) enem = false;
                                else
                                {
                                    hit.transform.gameObject.layer = 2;

                                    switch (hit.transform.tag)
                                    {
                                        case "UsuallyEnemy":
                                            hit.transform.GetComponent<UsuallyEnemy>().death = true;
                                            break;

                                        case "DoubleEnemy":
                                            hit.transform.GetComponent<DoubleEnemy>().death = true;
                                            hit.transform.GetComponent<DoubleEnemy>().deathFull = true;
                                            break;

                                        case "CupEnemy":
                                            hit.transform.GetComponent<CupEnemy>().death = true;
                                            break;

                                        case "UsuallyBoss":
                                            hit.transform.GetComponent<UsuallyBoss>().death = true;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            def--;
                            if (def <= 0) enem = false;
                        }

                        if (hit.transform != null)
                        {
                            if (hit.transform.name.Contains("wall") || hit.transform.name.Contains(".")
                                || hit.transform.name.Contains("door") || hit.transform.name.Contains("column")
                                || hit.transform.name.Contains("Electro"))
                            {
                                //Debug.DrawLine(transform.parent.position, hit.point, Color.red);
                                continue;
                            }
                        }
                    }
                }

                if (col.tag.Contains("Enemy")) col.gameObject.layer = 0;

                if (col.tag == "Door") continue;

                if (col.GetComponent<WallObjects>() != null)
                {
                    col.GetComponent<BoxCollider>().isTrigger = false;
                    col.GetComponent<Rigidbody>().useGravity = true;
                }

                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                {
                    rigidbodies.Add(col.attachedRigidbody);
                }

                soundDamage.pitch = UnityEngine.Random.Range(0.8f, 1.21f);
                soundDamage.Play();
            }
            foreach (var rb in rigidbodies)
            {
                rb.AddExplosionForce(explosionForce*multiplier, transform.position, r, 1*multiplier, ForceMode.Impulse);
            }
        }
    }
}
