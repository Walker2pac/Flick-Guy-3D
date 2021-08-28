using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenBoss : MonoBehaviour
{
    public bool attack;// Атакует враг или нет
    public bool death;// Мёртв враг или нет
    public int idSkin;// id материала кожи
    [Space]
    public float timeWave;// Время маха
    bool wave;
    public float timeAfterLastChicken;// Время идти после последней курицы
    public GameObject chicken;// Объект курицы
    public Transform chickenPointSpawn;// Объект курицы
    public float timeAttack;// Время его атаки
    public int countChicken;// Количество куриц, сколько запустить
    public float powerDropForward;
    public float powerDropUp;
    [Space]
    public float timePunch;// Время его атаки
    bool punch;// есть удар или нет
    public float speed;// Скорость
    public AnimationClip animIdle;// Анимация простоя
    public AnimationClip animRun;// Анимация бега
    public AnimationClip animPunch;// Анимация удара
    public AnimationClip animWave;// Анимация маха
    public GameObject eyes;// Обычные глаза
    public GameObject crossEyes;// Глаза крестиком
    public GameObject skin;// Кожа
    public Transform targetPoint;// Точка куда бужать
    public AudioSource audioDamage;
    public AudioSource audioFly;
    Vector3 targetPointPosition;
    [Header("Ragdroll")]
    public Rigidbody[] skeletonRd;// Набор костей для Ragdroll
    bool rag;// Проищошёл Ragdroll или нет
    [Header("Hat settings")]
    public Transform headTrans;
    public GameObject hat;
    GameObject hatObj;

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
    public bool vent;

    [Space]
    public GameObject warning;
    GameObject warningObj;
    bool createMsg;

    private Plane[] planes;
    [HideInInspector]
    public bool otherOG;

    private void Start()
    {
        if (targetPoint != null) targetPointPosition = targetPoint.position;
        oldColor = skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color;
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        anim.clip = animIdle;
        anim.Play();
        trans = transform;
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        gp = GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>();

        if (hat != null)
        {
            hatObj = Instantiate(hat, headTrans);
            hatObj.transform.localPosition = new Vector3(0, 0, 0);
            hatObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            hatObj.transform.localScale = new Vector3(1, 1, 1);
            Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    private void Update()
    {
        if (death == true)
        {
            eyes.SetActive(false);
            crossEyes.SetActive(true);
            skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color = Color.gray;
        }
        else
        {
            eyes.SetActive(true);
            crossEyes.SetActive(false);
            skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color = oldColor;
        }

        if ((death || vent) && !rag)
        {
            if (Time.timeScale >= 1) audioDamage.Play();
            GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().currentDeath += 1;
            go = false;
            CancelInvoke("Go");
            anim.clip = animIdle;
            anim.Play();
            rb.freezeRotation = false;
            rag = true;
            GetComponent<BoxCollider>().isTrigger = true;
            anim.enabled = false;
            Invoke("OnRag", 0.1f);
            if (vent) Invoke("VentDeath", gp.settingsProject.TimeDeathVent);

            for (int i = 0; i < trans.parent.childCount; i++)
            {
                if (trans.parent.GetChild(i).tag == "Chicken") trans.parent.GetChild(i).GetComponent<Chicken>().death = true;
            }
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
        }
    }

    void VentDeath()
    {
        death = true;
    }

    void OnRag()
    {
        if (hat != null)
        {
            Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), GetComponent<Collider>());
            hatObj.GetComponent<Rigidbody>().isKinematic = false;
            hatObj.GetComponent<Rigidbody>().velocity = rb.velocity;
            hatObj.GetComponent<Rigidbody>().AddForce(Vector3.up * 1f, ForceMode.Impulse);
            hatObj.GetComponent<BoxCollider>().isTrigger = false;
            //hatObj.transform.parent = gp.transform;
            hatObj.transform.parent = GameObject.FindGameObjectWithTag("GamePlay").transform;
        }

        for (int i = 0; i < skeletonRd.Length; i++)
        {
            skeletonRd[i].isKinematic = false;
            skeletonRd[i].velocity = rb.velocity;
            if (hat != null) Physics.IgnoreCollision(hatObj.GetComponent<Collider>(), skeletonRd[i].GetComponent<Collider>());
            skeletonRd[i].GetComponent<Collider>().enabled = true;
            skeletonRd[i].GetComponent<Collider>().isTrigger = false;
        }
    }

    private void FixedUpdate()
    {
        if (attack && !death)
        {
            if (warningObj != null)
            {
                Destroy(warningObj);
                createMsg = false;
                Camera.main.GetComponent<EnemiesWaypoint>().imgEnemy.Remove(warningObj.GetComponent<Image>());
                Camera.main.GetComponent<EnemiesWaypoint>().transEnemy.Remove(trans);
            }

            if (!wave) Invoke("Wave", timeAttack);
            wave = true;

            if (countChicken <= 0) Invoke("Go", timeAfterLastChicken);
        }

        if (go)
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
                            if (gp.damage < gp.playerHealth) Invoke("Fight", timePunch);
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
                        if (gp.damage < gp.playerHealth) Invoke("Fight", timePunch);
                        punch = true;
                    }
                    return;
                }
                else
                {
                    anim.clip = animRun;
                    anim.Play();
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
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        go = true;
    }

    void Wave()
    {
        if(countChicken > 0)
        {
            anim.clip = animWave;
            anim.Play();
            Invoke("Wave", timeWave);
            countChicken -= 1;
        }
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

    public void SpawnChicken()
    {
        GameObject chickenObj = Instantiate(chicken, chickenPointSpawn.position, Quaternion.Euler(Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        chickenObj.GetComponent<CapsuleCollider>().isTrigger = true;
        chickenObj.GetComponent<Rigidbody>().AddForce((transform.forward * powerDropForward) + (Vector3.up * powerDropUp) + (transform.right * Random.Range(-1, 2)), ForceMode.Impulse);
        chickenObj.transform.SetParent(trans.parent);
        //trans.parent.GetComponent<ObjectGroup>().allEnemy += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (death)
        {
            if (other.transform.name.Contains("Enemy"))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
                switch (other.tag)
                {
                    case "UsuallyEnemy":
                        other.GetComponent<UsuallyEnemy>().death = true;
                        other.GetComponent<UsuallyEnemy>().rb.velocity = rb.velocity;
                        break;

                    case "DoubleEnemy":
                        other.GetComponent<DoubleEnemy>().death = true;
                        other.GetComponent<DoubleEnemy>().rb.velocity = rb.velocity;
                        break;

                    case "CupEnemy":
                        other.GetComponent<CupEnemy>().death = true;
                        other.GetComponent<CupEnemy>().rb.velocity = rb.velocity;
                        break;

                    case "UsuallyBoss":
                        other.GetComponent<UsuallyBoss>().death = true;
                        other.GetComponent<UsuallyBoss>().rb.velocity = rb.velocity;
                        break;

                    case "ChickenBoss":
                        other.GetComponent<ChickenBoss>().death = true;
                        other.GetComponent<ChickenBoss>().rb.velocity = rb.velocity;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                if (other.transform.name.Contains("wall") || other.transform.name.Contains("door"))
                {
                    Invoke("StopCollider", 0.05f);
                }
            }
        }
    }

    void StopCollider()
    {
        trans.position = trans.position;
        rb.velocity = new Vector3(0, 0, 0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Door":
                if (collision.gameObject.GetComponent<Door>() != null && collision.gameObject.GetComponent<Door>().danger == true)
                {
                    death = true;
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    //for (int i = 0; i < skeletonRd.Length; i++)
                    //{
                    //    if (skeletonRd[i].GetComponent<BoxCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    //    if (skeletonRd[i].GetComponent<SphereCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<SphereCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    //    if (skeletonRd[i].GetComponent<CapsuleCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    //}
                    rb.velocity = new Vector3(0, 0, 0);
                }
                else if (!death)
                {
                    Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    for (int i = 0; i < skeletonRd.Length; i++)
                    {
                        if (skeletonRd[i].GetComponent<BoxCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                        if (skeletonRd[i].GetComponent<SphereCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<SphereCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                        if (skeletonRd[i].GetComponent<CapsuleCollider>() != null) Physics.IgnoreCollision(skeletonRd[i].GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                    }
                    rb.velocity = new Vector3(0, 0, 0);
                }
                break;

            case "Chicken":
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
                break;

            default:
                break;
        }

        if (collision.gameObject.name.Contains("piece"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.gameObject.GetComponent<MeshCollider>());
        }

        if (collision.gameObject.GetComponent<CharacterJoint>() != null || collision.transform.name == "pelvis")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

        if ((collision.gameObject.name.Contains("door") || collision.gameObject.name.Contains("wall")) && death)
        {
            rb.velocity = new Vector3(0, 0, 0);
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterJoint>() != null || collision.transform.name == "pelvis")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            for (int i = 0; i < skeletonRd.Length; i++)
            {
                if (hat != null) Physics.IgnoreCollision(collision.collider, skeletonRd[i].GetComponent<Collider>());
            }
        }

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
    }
}

