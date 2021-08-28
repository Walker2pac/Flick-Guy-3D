using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chicken : MonoBehaviour
{
    public bool death;
    [HideInInspector]
    public Rigidbody rb;

    public Animator anim;
    public BoxCollider boxCollider;
    public Material deathMaterial;
    public AudioSource audioAttack;
    public AudioSource audioDeath;
    Transform trans;

    bool attack1;
    bool back;
    bool firstAttack;
    Transform mainCamera;

    [Space]
    public GameObject warning;
    GameObject warningObj;
    bool createMsg;

    private Plane[] planes;
    [HideInInspector]

    Vector3 lastPos;

    void Start()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        trans = GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        Invoke("OffTrigger", 0.25f);
        Invoke("AttackFirst", 1.75f);
    }

    private void Update()
    {
        if(death)
        {
            boxCollider.isTrigger = true;
            back = false;
            anim.enabled = false;
            trans.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = deathMaterial;
        }

        if (attack1 && !death)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            trans.position = new Vector3(trans.position.x, trans.position.y + 3 * Time.deltaTime, trans.position.z);
            
            Quaternion OriginalRot = trans.rotation;
            trans.LookAt(mainCamera);
            Quaternion NewRot = trans.rotation;
            trans.rotation = OriginalRot;
            trans.rotation = Quaternion.RotateTowards(trans.rotation, NewRot, 5 * (Time.deltaTime * 20));

            Invoke("AttackSecond", 0.4f);
        }

        if(back)
        {
            trans.LookAt(mainCamera);
            trans.rotation = Quaternion.Euler(0, trans.localEulerAngles.y, 0);
            anim.enabled = true;
            //trans.Translate(-Vector3.forward * 1 * Time.deltaTime);
            if(lastPos != null) trans.position = Vector3.MoveTowards(trans.position, lastPos, 1.5f * Time.deltaTime);
            Invoke("AttackFirst", 4f);
        }

        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<CapsuleCollider>().bounds))//Проверяем вошел обьект в зону обзора камеры или нет
        {
            if (warningObj != null)
            {
                Destroy(warningObj);
                createMsg = false;
                Camera.main.GetComponent<EnemiesWaypoint>().imgEnemy.Remove(warningObj.GetComponent<Image>());
                Camera.main.GetComponent<EnemiesWaypoint>().transEnemy.Remove(trans);
            }
        }
        else
        {
            if (!death)
            {
                if (!createMsg)
                {
                    GameObject img = Instantiate(warning, GameObject.FindGameObjectWithTag("UiManager").transform);
                    warningObj = img;
                    createMsg = true;
                    Camera.main.GetComponent<EnemiesWaypoint>().imgEnemy.Add(warningObj.GetComponent<Image>());
                    Camera.main.GetComponent<EnemiesWaypoint>().transEnemy.Add(trans);
                }
            }
            else
            {
                if (warningObj != null)
                {
                    Destroy(warningObj);
                    createMsg = false;
                    Camera.main.GetComponent<EnemiesWaypoint>().imgEnemy.Remove(warningObj.GetComponent<Image>());
                    Camera.main.GetComponent<EnemiesWaypoint>().transEnemy.Remove(trans);
                }
            }
        }
    }

    void OffTrigger()
    {
        GetComponent<CapsuleCollider>().isTrigger = false;
    }

    void AttackFirst()
    {
        CancelInvoke("AttackFirst");
        back = false;
        anim.enabled = false;

        rb.velocity = new Vector3(0, 0, 0);
        attack1 = true;
    }

    void AttackSecond()
    {
        if (death)
        {
            CancelInvoke("AttackSecond");
            return;
        }

        attack1 = false;
        CancelInvoke("AttackSecond");
        anim.enabled = true;

        firstAttack = true;
        trans.LookAt(mainCamera);
        rb.freezeRotation = true;
        rb.velocity = new Vector3(0, 0, 0);
        lastPos = trans.position;
        rb.AddForce(((mainCamera.position - trans.position) * 1) + (Vector3.up * 3.75f), ForceMode.Impulse);
        boxCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" && GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().attackChicken == false && !death)
        {
            GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>().Damage(1);
            back = true;
            boxCollider.isTrigger = true;
            audioAttack.Play();
        }
        else if(collision.transform.tag == "Player" && death)
        {
            Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
        }
        else
        {
            if (firstAttack && collision.gameObject.layer == 2 && !death && !back)
            {
                if (collision.gameObject.GetComponent<BoxCollider>() != null)
                {
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                }
                if (collision.gameObject.GetComponent<SphereCollider>() != null)
                {
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<SphereCollider>());
                    Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<SphereCollider>());
                }
                if (collision.gameObject.GetComponent<CapsuleCollider>() != null)
                {
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
                    Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
                }
            }
        }

        if (death)
        {
            switch (collision.transform.tag)
            {
                //case "UsuallyEnemy":
                //    collision.transform.GetComponent<UsuallyEnemy>().death = true;
                //    collision.transform.GetComponent<UsuallyEnemy>().rb.velocity = rb.velocity;
                //    break;

                //case "DoubleEnemy":
                //    collision.transform.GetComponent<DoubleEnemy>().death = true;
                //    collision.transform.GetComponent<DoubleEnemy>().rb.velocity = rb.velocity;
                //    break;

                //case "CupEnemy":
                //    collision.transform.GetComponent<CupEnemy>().death = true;
                //    collision.transform.GetComponent<CupEnemy>().rb.velocity = rb.velocity;
                //    break;

                //case "UsuallyBoss":
                //    collision.transform.GetComponent<UsuallyBoss>().death = true;
                //    collision.transform.GetComponent<UsuallyBoss>().rb.velocity = rb.velocity;
                //    break;

                //case "ChickenBoss":
                //    if (trans.parent != collision.transform.transform.parent) break;
                //    collision.transform.GetComponent<ChickenBoss>().death = true;
                //    collision.transform.GetComponent<ChickenBoss>().rb.velocity = rb.velocity;
                //    break;

                case "Chicken":
                    if (!death || collision.transform.GetComponent<BoxCollider>().isTrigger == true)
                    {
                        rb.AddForce(-(collision.transform.position - trans.position) * 2f, ForceMode.Impulse);
                        break;
                    }
                    collision.transform.GetComponent<Chicken>().death = true;
                    collision.transform.GetComponent<Chicken>().rb.velocity = rb.velocity;
                    break;

                default:
                    break;
            }
        }
    }
}
