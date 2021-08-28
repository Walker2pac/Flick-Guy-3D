using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleEnemy : MonoBehaviour
{
    public bool attack;// Атакует враг или нет
    public bool death;// Мёрт враг или нет
    int countDeath;
    public bool deathFull;// Мёрт враг или нет
    public int idSkin;// id материала кожи
    public float timeKnockout;// Время в нокауте
    public float timeAttack;// Время его атаки
    public float timePunch;// Время его атаки
    bool punch;// есть удар или нет
    public float speed;// Скорость
    public AnimationClip animIdle;// Анимация простоя
    public AnimationClip animRun;// Анимация бега
    public AnimationClip animPunch;// Анимация бега
    public GameObject eyes;// Обычные глаза
    public GameObject crossEyes;// Глаза крестиком
    public GameObject skin;// Кожа
    public Transform targetPoint;// Точка куда бужать
    public AudioSource audioDamage;
    public AudioSource audioFly;
    Vector3 targetPointPosition;
    [Header("Ragdroll")]
    //public Rigidbody[] skeletonRd;// Набор костей для Ragdroll
    public List<Rigidbody> GetRigidbodies = new List<Rigidbody>();
    bool rag;// Проищошёл Ragdroll или нет
    [Header("Hat settings")]
    public Transform headTrans;
    public GameObject hat;
    GameObject hatObj;
    [Header("Body")]
    public Transform leftArm;
    public Transform rigthArm;
    public Transform leftLeg;
    public Transform rigthLeg;

    Color oldColor;// Старый цвет
    Animation anim;// Анимация
    [HideInInspector]
    public Rigidbody rb;// Физика
    
    Transform trans;
    Transform playerTrans;
    Vector3 playerPosition;

    GamePlay gp;
    bool go;

    [HideInInspector]
    public bool electro;
    [HideInInspector]
    public bool vent;

    bool resurrection;

    [Space]
    public GameObject warning;
    GameObject warningObj;
    bool createMsg;

    private Plane[] planes;
    [HideInInspector]
    public bool otherOG;

    [HideInInspector]
    public bool floor;

    private void Awake()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        if (targetPoint != null) targetPointPosition = targetPoint.position;
        oldColor = skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color;
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        anim.clip = animIdle;
        anim.Play();
        trans = transform;
        gp = GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>();

        GetRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        RigidbodyIsKinematicOn();

        if (hat != null)
        {
            hatObj = Instantiate(hat, headTrans);
            hatObj.transform.localPosition = new Vector3(0, 0, 0);
            hatObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            hatObj.transform.localScale = new Vector3(1, 1, 1);
            Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }



    /// <summary>
    /// //////////////////////////
    /// </summary>

    void RigidbodyIsKinematicOn()
    {
        for (int i = 0; i < GetRigidbodies.Count; i++)
        {
            if (GetRigidbodies[i].gameObject == gameObject) continue;
            GetRigidbodies[i].isKinematic = true;
            GetRigidbodies[i].gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }

    public void RigidbodyIsKinematicOff()
    {
        for (int i = 0; i < GetRigidbodies.Count; i++)
        {
            if (GetRigidbodies[i].gameObject == gameObject) continue;
            if (hat != null) Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetRigidbodies[i].GetComponent<Collider>());
            Physics.IgnoreCollision(GetRigidbodies[i].gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            GetRigidbodies[i].isKinematic = false;
            GetRigidbodies[i].velocity = rb.velocity;
            GetRigidbodies[i].gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }




    private void Update()
    {
        if(deathFull == true && !crossEyes.activeSelf)
        {
            eyes.SetActive(false);
            crossEyes.SetActive(true);
            skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color = Color.gray;
            GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().currentDeath += 1;
            if (floor && !electro) trans.position = GetRigidbodies[1].position;
        }
        else if(deathFull == false)
        {
            eyes.SetActive(true);
            crossEyes.SetActive(false);
            skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color = oldColor;
        }

        if ((death || vent) && !rag)
        {
            if (warningObj != null)
            {
                Destroy(warningObj);
                createMsg = false;
                Camera.main.GetComponent<EnemiesWaypoint>().imgEnemy.Remove(warningObj.GetComponent<Image>());
                Camera.main.GetComponent<EnemiesWaypoint>().transEnemy.Remove(trans);
            }

            if (Time.timeScale >= 1) audioDamage.Play();
            go = false;
            CancelInvoke("Go");
            if (countDeath >= 1)
            {
                deathFull = true;
                GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size / 2;
            }
            anim.clip = animIdle;
            anim.Play();
            rb.useGravity = true;
            rb.freezeRotation = false;
            rag = true;
            anim.enabled = false;
            //Invoke("OnRag", 0.1f);
            Invoke("OnRag", 0);
            if (vent) Invoke("VentDeath", gp.settingsProject.TimeDeathVent);
        }



        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<BoxCollider>().bounds) || otherOG)//Проверяем вошел обьект в зону обзора камеры или нет
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
            if (!death && attack)
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

            if (electro) gp.electro = false;
        }
    }

    void VentDeath()
    {
        death = true;
        deathFull = true;
    }

    void OnRag()
    {
        if (hat != null && countDeath == 0)
        {
            Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetComponent<Collider>());
            hatObj.GetComponent<Rigidbody>().isKinematic = false;
            hatObj.GetComponent<Rigidbody>().velocity = rb.velocity;
            hatObj.GetComponent<Rigidbody>().AddForce(Vector3.up * 1f, ForceMode.Impulse);
            hatObj.GetComponent<BoxCollider>().isTrigger = false;
            //hatObj.transform.parent = gp.transform;
            hatObj.transform.parent = GameObject.FindGameObjectWithTag("GamePlay").transform;
        }

        for (int i = 0; i < GetRigidbodies.Count; i++)
        {
            if (GetRigidbodies[i].gameObject == gameObject) continue;
            if (hat != null) Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetRigidbodies[i].GetComponent<Collider>());
            Physics.IgnoreCollision(GetRigidbodies[i].gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            GetRigidbodies[i].isKinematic = false;
            GetRigidbodies[i].velocity = rb.velocity;
            GetRigidbodies[i].gameObject.GetComponent<Collider>().isTrigger = false;
        }

        Invoke("RigidbodyIsKinematicOff", 0.1f);

        if (countDeath == 0) countDeath += 1;
        if (!deathFull) Invoke("Resurrection", timeKnockout);
    }

    void Resurrection()
    {
        if (deathFull == true) return;
        CancelInvoke("Resurrection");
        resurrection = true;
    }

    void EndResurrection()
    {
        if (deathFull == true) return;
        trans.position = GetRigidbodies[1].transform.position - new Vector3(0, 1, 0);
        resurrection = false;
        RigidbodyIsKinematicOn();
        ////GetComponent<BoxCollider>().isTrigger = false;
        CancelInvoke("EndResurrection");
        if (deathFull || electro) return;
        //for (int i = 0; i < skeletonRd.Length; i++)
        //{
        //    skeletonRd[i].isKinematic = true;
        //    if (skeletonRd[i].GetComponent<BoxCollider>() != null) skeletonRd[i].GetComponent<BoxCollider>().enabled = false;
        //    if (skeletonRd[i].GetComponent<SphereCollider>() != null) skeletonRd[i].GetComponent<SphereCollider>().enabled = false;
        //    if (skeletonRd[i].GetComponent<CapsuleCollider>() != null) skeletonRd[i].GetComponent<CapsuleCollider>().enabled = false;
        //}
        anim.enabled = true;
        anim.clip = animIdle;
        anim.Play();
        rb.freezeRotation = true;
        death = false;
        rag = false;
        if (targetPoint != null) trans.LookAt(targetPoint);
        else trans.LookAt(playerPosition);
        trans.localEulerAngles = new Vector3(0, trans.localEulerAngles.y, 0);
        timeAttack = 0;
    }

    private void FixedUpdate()
    {
        if (attack && !death)
        {
            Invoke("Go", timeAttack);
        }

        if(resurrection && !electro)
        {
            Transform skeletonTrans = GetRigidbodies[3].transform;
            skeletonTrans.position = new Vector3(skeletonTrans.position.x, skeletonTrans.position.y + 33 * Time.deltaTime, skeletonTrans.position.z);
            //transform.position = skeletonTrans.position;
            Invoke("EndResurrection", 0.45f);
        }

        if (go && !death)
        {
            if (targetPoint != null)
            {
                if (Vector3.Distance(trans.position, targetPointPosition) <= 2.225f)
                {
                    if (Vector3.Distance(trans.position, playerPosition) <= 2.225f)
                    {
                        trans.LookAt(playerPosition);
                        trans.rotation = Quaternion.Euler(0, trans.localEulerAngles.y, 0);
                        if (!punch)
                        {
                            anim.clip = animIdle;
                            anim.Play();
                            if (!gp.death) Invoke("Fight", timePunch);
                            punch = true;
                        }
                        return;
                    }
                    else if (Vector3.Distance(trans.position, targetPointPosition) <= 0.25f)
                    {
                        anim.clip = animIdle;
                        anim.Play();
                        //attack = false;
                        //CancelInvoke("Go");
                        return;
                    }
                    else
                    {
                        anim.clip = animRun;
                        anim.Play();
                        float step = speed * Time.deltaTime;
                        trans.position = Vector3.MoveTowards(trans.position, targetPointPosition, step);
                        return;
                    }
                }
                else
                {
                    anim.clip = animRun;
                    anim.Play();
                }
            }
            else
            {
                if (Vector3.Distance(trans.position, playerPosition) <= 2.225f)
                {
                    trans.LookAt(playerPosition);
                    trans.rotation = Quaternion.Euler(0, trans.localEulerAngles.y, 0);
                    if (!punch)
                    {
                        anim.clip = animIdle;
                        anim.Play();
                        if (!gp.death) Invoke("Fight", timePunch);
                        punch = true;
                    }
                    return;
                }
                else
                {
                    if (!gp.death)
                    {
                        anim.clip = animRun;
                        anim.Play();
                    }
                    else
                    {
                        anim.clip = animIdle;
                        anim.Play();
                    }
                }
            }

            if (targetPoint != null)
            {
                float step = speed * Time.deltaTime;
                trans.LookAt(targetPoint);
                trans.rotation = Quaternion.Euler(0, trans.localEulerAngles.y, 0);
                trans.position = Vector3.MoveTowards(trans.position, targetPointPosition, step);
            }
            else
            {
                float step = speed * Time.deltaTime;
                trans.LookAt(playerPosition);
                trans.rotation = Quaternion.Euler(0, trans.localEulerAngles.y, 0);
                trans.position = Vector3.MoveTowards(trans.position, playerPosition, step);
            }
        }
    }

    void Go()
    {
        if (go) return;
        gp = GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        anim = GetComponent<Animation>();
        go = true;
    }

    void Fight()
    {
        if (Vector3.Distance(trans.position, playerPosition) > 2.225f) return;
        if (gp.damage >= gp.playerHealth) return;

        CancelInvoke("Fight");
        anim.clip = animPunch;
        anim.Play();
        Invoke("Fight", timePunch + 1f);
        Invoke("OffPunchAnim", 1f);
    }

    void OffPunchAnim()
    {
        CancelInvoke("OffPunchAnim");
        anim.clip = animIdle;
        anim.Play();
        punch = false;
    }

    public void Attack()
    {
        gp.Damage(1);
        if (gp.death) gp.LookToward(trans);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Door":
                if (collision.gameObject.GetComponent<Door>() != null && collision.gameObject.GetComponent<Door>().danger == true)
                {
                    death = true;
                    deathFull = true;
                    Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
                    rb.velocity = new Vector3(0, 0, 0);
                }
                else if (!death)
                {
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    for (int i = 0; i < GetRigidbodies.Count; i++)
                    {
                        Physics.IgnoreCollision(GetRigidbodies[i].GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
                    }
                    rb.velocity = new Vector3(0, 0, 0);
                }
                break;

            default:
                break;
        }

        if (collision.gameObject.name.Contains("piece"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<MeshCollider>());
        }

        if (death && !floor)
        {
            if (floor) return;
            if (collision.transform.name.Contains("Enemy"))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
                switch (collision.transform.tag)
                {
                    case "UsuallyEnemy":
                        collision.transform.GetComponent<UsuallyEnemy>().death = true;
                        collision.transform.GetComponent<UsuallyEnemy>().rb.velocity = rb.velocity;
                        break;

                    case "DoubleEnemy":
                        collision.transform.GetComponent<DoubleEnemy>().death = true;
                        collision.transform.GetComponent<DoubleEnemy>().rb.velocity = rb.velocity;
                        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
                        break;

                    case "CupEnemy":
                        collision.transform.GetComponent<CupEnemy>().death = true;
                        collision.transform.GetComponent<CupEnemy>().rb.velocity = rb.velocity;
                        break;

                    case "UsuallyBoss":
                        collision.transform.GetComponent<UsuallyBoss>().death = true;
                        collision.transform.GetComponent<UsuallyBoss>().rb.velocity = rb.velocity;
                        break;

                    default:
                        break;
                }
            }
        }
        else if (death && floor && collision.transform.name.Contains("Enemy")) Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        
        if (collision.gameObject.GetComponent<CharacterJoint>() != null)
        {
            UsuallyEnemy ue = collision.gameObject.GetComponentInParent<UsuallyEnemy>();
            DoubleEnemy de = collision.gameObject.GetComponentInParent<DoubleEnemy>();

            if (ue != null && ue.death && !ue.floor)
            {
                death = true;
                rb.velocity = ue.rb.velocity;
            }
            else if (de != null && de.death && !de.floor)
            {
                death = true;
                rb.velocity = de.rb.velocity;
            }
        }

        if ((collision.gameObject.name.Contains("door") || collision.gameObject.name.Contains("wall")) && deathFull && !electro)
        {
            rb.velocity = new Vector3(0, 0, 0);
            trans.position = trans.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.gameObject.layer == 2 && collision.gameObject.GetComponent<Rigidbody>() != null)
            || (collision.gameObject.GetComponent<Rigidbody>() != null && !collision.gameObject.name.Contains("Enemy")))
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - (trans.position - new Vector3(0, 0.3f, 0))) * 0.6f, ForceMode.Impulse);
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

        switch (collision.transform.tag)
        {
            case "Hats":
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;

            case "Chicken":
                if (collision.gameObject.GetComponent<Chicken>().death == false) break;
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;

            default:
                break;
        }

        if (death && collision.gameObject.name.Contains("Cube")) floor = true;
        else if (death && !collision.gameObject.name.Contains("Cube")) floor = false;
    }
}
