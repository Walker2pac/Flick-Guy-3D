using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CupEnemy : MonoBehaviour
{
    public bool attack;// Атакует враг или нет
    public bool death;// Мёрт враг или нет
    public int idSkin;// id материала кожи
    public float timeAttack;// Время его атаки
    public float timeThrow;// Время его атаки
    [HideInInspector]
    public float powerThrow;// Сила броска
    public AnimationClip animIdle;// Анимация простоя
    public AnimationClip animThrow;// Анимация броска
    public AnimationClip animBeforeThrow;// Анимация перед броском
    public GameObject eyes;// Обычные глаза
    public GameObject crossEyes;// Глаза крестиком
    public GameObject skin;// Кожа
    public GameObject cup;// объект кружки
    public Transform pointSpawnCup;// объект кружки
    public Transform transformHead;// Голова
    public AudioSource audioDamage;
    public AudioSource audioLaughter;
    [Header("Ragdroll")]
    //public Rigidbody[] skeletonRd;// Набор костей для Ragdroll
    public List<Rigidbody> GetRigidbodies = new List<Rigidbody>();
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

    bool cupAttack;
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
        attack = false;
        death = false;
        oldColor = skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color;
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        anim.clip = animIdle;
        anim.Play();
        trans = transform;
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
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
        GetComponent<CapsuleCollider>().radius = GetComponent<CapsuleCollider>().radius / 2;
        GetComponent<CapsuleCollider>().height = GetComponent<CapsuleCollider>().height / 2;
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
        if (death == true && !crossEyes.activeSelf)
        {
            eyes.SetActive(false);
            crossEyes.SetActive(true);
            skin.GetComponent<SkinnedMeshRenderer>().materials[idSkin].color = Color.gray;
            trans.position = GetRigidbodies[1].position;
        }
        else if(!death)
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
            GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().currentDeath += 1;
            anim.clip = animIdle;
            anim.Play();
            rb.freezeRotation = false;
            rag = true;
            //GetComponent<CapsuleCollider>().isTrigger = true;
            anim.enabled = false;
            //Invoke("OnRag", 0.1f);
            Invoke("OnRag", 0);
            if (vent) Invoke("VentDeath", gp.settingsProject.TimeDeathVent);
        }

        if (attack && !death)
        {
            Invoke("Attack", timeAttack);
        }


        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<CapsuleCollider>().bounds) || otherOG)//Проверяем вошел обьект в зону обзора камеры или нет
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

        GetComponent<CapsuleCollider>().radius = GetComponent<CapsuleCollider>().radius / 2;
        GetComponent<CapsuleCollider>().height = GetComponent<CapsuleCollider>().height / 2;
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
    }

    void Attack()
    {
        gp = GameObject.FindGameObjectWithTag("GamePlay").GetComponent<GamePlay>();
        if (death || gp.death) return;
        CancelInvoke("Attack");
        attack = false;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        trans.LookAt(playerPosition);
        anim.clip = animBeforeThrow;
        anim.Play();
    }

    public void OffAttackAnim()
    {
        anim.clip = animIdle;
        anim.Play();
        cupAttack = false;
        Invoke("Attack", timeAttack);
    }

    public void OnAttackAnim()
    {
        anim.clip = animThrow;
        anim.Play();
    }

    public void ThrowCup()
    {
        cupAttack = true;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        GameObject cupObj = Instantiate(cup, pointSpawnCup.position, Quaternion.identity);
        cupObj.transform.SetParent(trans);
        cupObj.GetComponent<Rigidbody>().AddForce(((playerPosition - pointSpawnCup.position) * powerThrow) + (Vector3.up * (4.25f + Vector3.Distance(playerPosition, trans.position) / 10)), ForceMode.Impulse);
        cupObj.GetComponent<CupByEnemy>().headEnemy = transformHead;
        cupObj.GetComponent<CupByEnemy>().power = powerThrow;
        Invoke("Laughter", 0.2f);
    }

    private void Laughter()
    {
        if(!death) audioLaughter.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (death)
        {
            if (other.transform.name.Contains("wall") || other.transform.name.Contains("door"))
            {
                Invoke("StopCollider", 0.05f);
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
        if (collision.transform.tag == "CupByEnemy" && cupAttack == false)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 3f, ForceMode.Impulse);
            death = true;
        }

        if (collision.gameObject.name.Contains("piece"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<MeshCollider>());
        }

        switch (collision.gameObject.tag)
        {
            case "Door":
                if (collision.gameObject.GetComponent<Door>() != null && collision.gameObject.GetComponent<Door>().danger == true)
                {
                    death = true;
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

        if ((collision.gameObject.name.Contains("door") || collision.gameObject.name.Contains("wall")) && death)
        {
            rb.velocity = new Vector3(0, 0, 0);
            trans.position = trans.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 2 && collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - trans.position) * 0.5f, ForceMode.Impulse);
        }

        switch (collision.transform.tag)
        {
            case "Hats":
                Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;

            case "Chicken":
                if (collision.gameObject.GetComponent<Chicken>().death == false) break;
                Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());
                Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;

            default:
                break;
        }
    }
}
