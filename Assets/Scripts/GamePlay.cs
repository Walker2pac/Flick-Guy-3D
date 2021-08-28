using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    [SerializeField] public SettingsProject settingsProject;
    public SplineFollower splineFollower;// Следователь сплайна
    public GameObject player;// Объект игрока
    public Transform transPlayer;// Трансформ игрока
    public Transform transCamera;// Трансформ камеры
    public CameraController cameraController;// Контроллер камеры
    public FixedTouchField fixedTouchField;// Контролль нажат палец на экран или нет
    public Transform target;// Трансформ следователя сплайна0
    public float speedRot;// Скорость поворота
    public Animator animHead;
    public bool attackAnim;
    float dropForce;// Сила отталкивания врага
    float upDropForce;// Сила отталкивания врага вверх
    float playerAttackDistance;// Сила удара игрока
    [HideInInspector]
    public int playerHealth;// Количество жизней у игрока
    [HideInInspector]
    public int damage;// Сколько урона получил игрок
    bool punch;
    [Space]
    public bool followPosition;// Отслеживать позицию или нет

    bool rotate;// Поварачивать камеру к объекту или нет
    bool rotateToSpline;// Поварачивать камеру по ходу движения сплайна или нет
    Transform lookTarget;// Цель поворота

    int countEnemiesGroup;// Кол-во групп, которые напали
    public bool attack;// Если на игрока напали
    bool finish;

    [HideInInspector]
    public bool electro;
    bool doubleElectro;

    [HideInInspector]
    public bool door;
    [HideInInspector]
    public Transform doorTrans;

    [HideInInspector]
    public bool attackCup;
    bool minusCup;
    int forKillEnemies;

    [HideInInspector]
    public Transform attackCupObj;

    private Plane[] planes;
    [HideInInspector]
    public bool superPunch;
    public float forceForSuperPunch;

    [HideInInspector]
    public bool start;
    [HideInInspector]
    public bool death;

    Transform currentOG;

    [HideInInspector]
    public bool attackChicken;

    private void Awake()// В самом старте игры
    {
        fixedTouchField = GameObject.FindGameObjectWithTag("TouchPanel").GetComponent<FixedTouchField>();// Ищем объект на сцене с тегом TouchPanel и получаем его компонент

        //transCamera = Camera.main.transform;
        //cameraController = Camera.main.GetComponent<CameraController>();

        start = false;
        GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelMainMenu.gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelLevelFailed.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelLevelWin.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelProgress.gameObject.SetActive(false);

        splineFollower.followSpeed = settingsProject.PlayerSpeed;// Опеределяем настройки скорости игрока
        cameraController.speedRotate = settingsProject.PlayerSpeedRotation;// Опеределяем настройки вращения игрока игроком
        speedRot = settingsProject.ToTargetSpeedRotation;// Опеределяем настройки вращения игрока компьюетром
        dropForce = settingsProject.DropForce;// Опеределяем силу отталкивания врага
        upDropForce = settingsProject.UpDropForce;// Опеределяем силу отталкивания врага вверх
        playerAttackDistance = settingsProject.PlayerAttackDistance;// Опеределяем сила удара игрока
        playerHealth = settingsProject.PlayerHealth;// Опеределяем количество жизней у игрока
        forceForSuperPunch = settingsProject.ForceForSuperPunch;

        death = false;
        damage = 0;
        Damage(0);

        splineFollower.startPosition = 0f;
    }

    private void Update()
    {
        if (countEnemiesGroup > 0) attack = true;
        else
        {
            attack = false;
            if(!door && !electro && start && damage < playerHealth && !finish) splineFollower.follow = true;// Продолжаем идти
            else if(door && !electro)
            {
                cameraController.playerLook = false;
                Quaternion OriginalRot = transCamera.rotation;
                transCamera.LookAt(doorTrans);
                Quaternion NewRot = transCamera.rotation;
                transCamera.rotation = OriginalRot;
                transCamera.rotation = Quaternion.RotateTowards(transCamera.rotation, NewRot, speedRot * (Time.deltaTime * 20));
            }
            else if ((!door && (electro && !doubleElectro)) || !start || finish)
            {
                if (electro) doubleElectro = true;
                splineFollower.follow = false;
            }
        }

        if ((attack || forKillEnemies > 0) && !death)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));
            
            if (Physics.Raycast(ray, out hit, playerAttackDistance))
            {
                Transform hitTrans = hit.transform;

                Debug.DrawLine(ray.origin, hit.point, Color.red);

                switch (hitTrans.tag)
                {
                    case "UsuallyEnemy":
                        if (!attackAnim && hitTrans.GetComponent<UsuallyEnemy>().death == false)
                        {
                            if (animHead.GetInteger("IndexAnim") != 1) animHead.SetInteger("IndexAnim", 1);
                            attackAnim = true;
                        }                        
                        break;

                    case "DoubleEnemy":
                        if (!attackAnim && hitTrans.GetComponent<DoubleEnemy>().death == false)
                        {
                            if (animHead.GetInteger("IndexAnim") != 1) animHead.SetInteger("IndexAnim", 1);
                            attackAnim = true;
                        }
                        break;

                    case "UsuallyBoss":
                        if (!attackAnim && hitTrans.GetComponent<UsuallyBoss>().death == false)
                        {
                            if (animHead.GetInteger("IndexAnim") != 1) animHead.SetInteger("IndexAnim", 1);
                            attackAnim = true;
                        }
                        break;

                    case "ChickenBoss":
                        if (!attackAnim && hitTrans.GetComponent<ChickenBoss>().death == false)
                        {
                            if (animHead.GetInteger("IndexAnim") != 1) animHead.SetInteger("IndexAnim", 1);
                            attackAnim = true;
                        }
                        break;

                    case "Chicken":
                        if (!attackAnim && hitTrans.GetComponent<Chicken>().death == false)
                        {
                            attackChicken = true;
                            if (animHead.GetInteger("IndexAnim") != 1) animHead.SetInteger("IndexAnim", 1);
                            attackAnim = true;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        if(attackCup && !minusCup && !death)
        {
            animHead.SetInteger("IndexAnim", 1);
            minusCup = true;
        }

        if (superPunch && !death)
        {
            animHead.SetInteger("IndexAnim", 6);
        }

        if (damage >= playerHealth)
        {
            death = true;
            GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelLevelFailed.gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().superBtn.SetActive(false);
            cameraController.playerLook = false;
            splineFollower.follow = false;
        }
    }

    public void Damage(int count)
    {
        damage += count;
        if (damage >= playerHealth) death = true;
        Image di = GameObject.FindGameObjectWithTag("DamageIndicator").GetComponent<Image>();
        float health = playerHealth;
        float alpha = (1 / health) * damage;
        di.color = new Color(1, 1, 1, alpha);
    }

    public void OffAttackAnim()
    {
        if(punch)
        {
            animHead.SetInteger("IndexAnim", Random.Range(3, 6));
            punch = false;
        } else animHead.SetInteger("IndexAnim", 3);
        
        attackChicken = false;
        attackAnim = false;
    }

    public void Attack()
    {
        if (death)
        {
            OffAttackAnim();
            return;
        }

        if(attackCup)
        {
            CupByEnemy cbe = attackCupObj.GetComponent<CupByEnemy>();
            attackCupObj.GetComponent<Rigidbody>().velocity = new Vector2(0, 0);
            attackCupObj.GetComponent<Rigidbody>().AddForce(((cbe.headEnemy.position - attackCupObj.position) * cbe.power * 2) + (Vector3.up * 3f), ForceMode.Impulse);
            player.GetComponent<PlayerLogic>().audioAttackCup.Play();
            attackCup = false;
            punch = true;
            attackAnim = false;
            minusCup = false;
        }

        if(superPunch)
        {
            GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            superPunch = false;
            GameObject.FindGameObjectWithTag("SuperPunch").GetComponent<SuperPunch>().punch = false;
            Time.timeScale = 1;

            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            var cols = Physics.OverlapSphere(transform.position, 100);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                if (col.GetComponent<Collider>() == null) return;

                if (GeometryUtility.TestPlanesAABB(planes, col.GetComponent<Collider>().bounds))//Проверяем вошел обьект в зону обзора камеры или нет
                {
                    if (col.name != "Camera" && col.name != "Player")
                    {
                        RaycastHit hit2 = new RaycastHit();
                        Vector3 pos = (col.gameObject.transform.position - transCamera.position).normalized;
                        if (Physics.Raycast(transCamera.position, pos, out hit2))
                        { 
                            if (hit2.transform.name.Contains("wall") || hit2.transform.name.Contains(".")
                                || hit2.transform.name.Contains("door") || hit2.transform.name.Contains("column")
                                || hit2.transform.name.Contains("Electro") || hit2.transform.name == "Door")
                            {
                                Debug.DrawLine(transPlayer.position, hit2.point, Color.red);
                                continue;
                            }
                            else if (hit2.transform.name.Contains("Enemy"))
                            {
                                hit2.transform.gameObject.layer = 2;
                                bool enem = true;
                                int def = cols.Length;
                                while (enem)
                                {
                                    RaycastHit hit3 = new RaycastHit();
                                    if (Physics.Raycast(transform.position, pos, out hit3))
                                    {
                                        if (hit3.transform.name.Contains("Enemy") && hit3.transform.gameObject == col.gameObject)
                                        {
                                            hit3.transform.gameObject.layer = 0;
                                            enem = false;
                                            switch (col.tag)
                                            {
                                                case "UsuallyEnemy":
                                                    col.transform.GetComponent<UsuallyEnemy>().death = true;
                                                    break;

                                                case "DoubleEnemy":
                                                    col.transform.GetComponent<DoubleEnemy>().death = true;
                                                    col.transform.GetComponent<DoubleEnemy>().deathFull = true;
                                                    break;

                                                case "CupEnemy":
                                                    col.transform.GetComponent<CupEnemy>().death = true;
                                                    break;

                                                case "UsuallyBoss":
                                                    col.transform.GetComponent<UsuallyBoss>().death = true;
                                                    break;

                                                case "ChickenBoss":
                                                    col.transform.GetComponent<ChickenBoss>().death = true;
                                                    break;

                                                default:
                                                    break;
                                            }
                                            continue;

                                            //if (hit3.transform.name.Contains("wall") || hit3.transform.name.Contains(".")
                                            //|| hit3.transform.name.Contains("door") || hit3.transform.name.Contains("column")
                                            //|| hit3.transform.name.Contains("Electro") || hit3.transform.gameObject != col.gameObject)
                                            //{
                                            //    //Debug.DrawLine(transform.parent.position, hit.point, Color.red);

                                            //}
                                        }
                                        else
                                        {
                                            hit3.transform.gameObject.layer = 2;
                                        }
                                    }
                                    def--;
                                    if (def <= 0) enem = false;
                                }
                            }
                        }

                        switch (col.tag)
                        {
                            case "UsuallyEnemy":
                                col.transform.GetComponent<UsuallyEnemy>().death = true;
                                break;

                            case "DoubleEnemy":
                                col.transform.GetComponent<DoubleEnemy>().death = true;
                                col.transform.GetComponent<DoubleEnemy>().deathFull = true;
                                break;

                            case "CupEnemy":
                                col.transform.GetComponent<CupEnemy>().death = true;
                                break;

                            case "UsuallyBoss":
                                col.transform.GetComponent<UsuallyBoss>().death = true;
                                break;

                            case "ChickenBoss":
                                col.transform.GetComponent<ChickenBoss>().death = true;
                                break;

                            default:
                                break;
                        }

                        if (col.tag.Contains("Enemy")) col.gameObject.layer = 0;

                        if (col.tag == "Door") continue;

                        if (col.gameObject.GetComponent<CharacterJoint>() != null) continue;

                        if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                        {
                            rigidbodies.Add(col.attachedRigidbody);
                        }
                    }
                }
            }

            foreach (var rb in rigidbodies)
            {
                rb.AddExplosionForce(forceForSuperPunch, transPlayer.position, 100, 1 * 1, ForceMode.Impulse);
            }
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, playerAttackDistance))
        {
            Transform hitTrans = hit.transform;

            switch (hitTrans.tag)
            {
                case "UsuallyEnemy":
                    UsuallyEnemy ue = hitTrans.GetComponent<UsuallyEnemy>();
                    ue.death = true;
                    ue.audioDamage.pitch = Random.Range(0.8f, 1.21f);
                    ue.audioDamage.Play();
                    ue.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
                    //ue.rb.AddForce(((hitTrans.position - player.transform.position) * dropForce) + (Vector3.up * upDropForce), ForceMode.Impulse);
                    ue.rb.AddForce((transCamera.forward * dropForce) + (Vector3.up * upDropForce), ForceMode.Impulse);
                    ue.RigidbodyIsKinematicOff();
                    if (ue.targetPoint != null) forKillEnemies -= 1;
                    punch = true;
                    player.GetComponent<PlayerLogic>().audioAttack.Play();
                    Invoke("OffAttackAnim", 0.18333f);
                    break;

                case "DoubleEnemy":
                    DoubleEnemy de = hitTrans.GetComponent<DoubleEnemy>();
                    de.death = true;
                    de.audioDamage.pitch = Random.Range(0.8f, 1.21f);
                    de.audioDamage.Play();
                    //de.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    de.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
                    de.rb.AddForce((transCamera.forward * dropForce) + (Vector3.up * upDropForce), ForceMode.Impulse);
                    de.RigidbodyIsKinematicOff();
                    if (de.targetPoint != null && de.deathFull == true) forKillEnemies -= 1;
                    punch = true;
                    player.GetComponent<PlayerLogic>().audioAttack.Play();
                    Invoke("OffAttackAnim", 0.18333f);
                    break;

                case "UsuallyBoss":
                    UsuallyBoss ub = hitTrans.GetComponent<UsuallyBoss>();
                    ub.death = true;
                    ub.audioDamage.pitch = Random.Range(0.8f, 1.21f);
                    ub.audioDamage.Play();
                    ub.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
                    ub.rb.AddForce((transCamera.forward * dropForce) + (Vector3.up * upDropForce), ForceMode.Impulse);
                    if (ub.targetPoint != null) forKillEnemies -= 1;
                    punch = true;
                    player.GetComponent<PlayerLogic>().audioAttack.Play();
                    Invoke("OffAttackAnim", 0.18333f);
                    break;

                case "ChickenBoss":
                    ChickenBoss chb = hitTrans.GetComponent<ChickenBoss>();
                    chb.death = true;
                    chb.audioDamage.pitch = Random.Range(0.8f, 1.21f);
                    chb.audioDamage.Play();
                    chb.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
                    chb.rb.AddForce((transCamera.forward * dropForce * 1.5f) + (Vector3.up * upDropForce), ForceMode.Impulse);
                    if (chb.targetPoint != null) forKillEnemies -= 1;
                    punch = true;
                    player.GetComponent<PlayerLogic>().audioAttack.Play();
                    attackChicken = false;
                    Invoke("OffAttackAnim", 0.18333f);
                    break;

                case "Chicken":
                    Chicken c = hitTrans.GetComponent<Chicken>();
                    c.death = true;
                    c.audioDeath.Play();
                    c.rb.velocity = new Vector3(0, 0, 0);
                    c.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
                    c.rb.AddForce((transCamera.forward * dropForce * 1.5f) + (Vector3.up * -1f), ForceMode.Impulse);
                    c.rb.freezeRotation = false;
                    punch = true;
                    player.GetComponent<PlayerLogic>().audioAttack.Play();
                    Invoke("OffAttackAnim", 0.18333f);
                    break;

                default:
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (followPosition) transPlayer.position = target.position;// Если следовать, то следовать за сплайном

        /// Если есть поворот к цели, то вычисляем куда поварачиваться
        /// и плавно поварачиваемся
        /// Через 1.5 секунд выключаем поворот и даём управление игроку
        if (rotate)
        {
            if (cameraController.playerLook == false)
            {
                Quaternion OriginalRot = transCamera.rotation;
                if(lookTarget.gameObject.name.Contains("Enemy")) transCamera.LookAt(lookTarget.position + new Vector3(0, 1.5f, 0));
                else transCamera.LookAt(lookTarget);
                Quaternion NewRot = transCamera.rotation;
                transCamera.rotation = OriginalRot;
                transCamera.rotation = Quaternion.RotateTowards(transCamera.rotation, NewRot, speedRot * (Time.deltaTime * 10));
            }

            if(attack) Invoke("OffRotate", 0.25f);
            else Invoke("OffRotate", 1.5f);
        }

        /// Если есть поворот к сплайну, то вычисляем куда поварачиваться
        /// и плавно поварачиваемся
        /// /// Через 1.5 секунд выключаем поворот и даём управление игроку
        if (rotateToSpline)
        {
            CancelInvoke("OffRotate");// Отменяем вызов метода
            transCamera.rotation = Quaternion.Lerp(transCamera.rotation, target.rotation, speedRot * (Time.deltaTime / 2));// Поварачиваем игрока в сторону ходьбы
            Invoke("OffRotateToSpline", 1.5f);
        }
    }

    public void AttackOff()// Метод выключение атаки
    {
        countEnemiesGroup -= 1;
        damage = 0;
        Damage(0);
    }

    void OffRotate()
    {
        if (fixedTouchField.Pressed == true)// Если игрок не касается экрана
        {
            cameraController.playerLook = true;// Включаем управление у игрока
            rotate = false;
        }
    }

    void OffRotateToSpline()// Выключаем поворот и даём управление игроку
    {
        cameraController.playerLook = true;
        rotateToSpline = false;
    }


    /// Метод делает поворот к цели
    /// 
    /// обозначаем цель
    /// включаем управление у игрока
    /// включаем поворот к цели
    public void LookToward(Transform lookPoint)
    {
        if(!attack) CancelInvoke("OffRotate");
        cameraController.playerLook = false;
        lookTarget = lookPoint;
        rotate = true;
    }


    /// Метод делает поворот к сплайну
    /// 
    /// Выключаем управление у игрока
    /// выключаем поворот к цели
    /// включаем поворот к сплайну
    public void StopLook()
    {
        CancelInvoke("OffRotate");// Отменяем вызов метода
        cameraController.playerLook = false;
        rotate = false;
        rotateToSpline = true;
    }

    /// Метод, когда враги (если живы) нападают на игрока
    /// 
    /// Останавливаем движение сплайна, если враги напали (кто-то жив)
    /// 
    public void WaitForKill(Transform objectGroup)
    {
        doubleElectro = false;
        door = false;

        if (currentOG != null)
        {
            for (int i = 0; i < currentOG.childCount; i++)
            {
                switch (currentOG.GetChild(i).tag)// Проверяем типы
                {
                    case "UsuallyEnemy":// Если тип UsuallyEnemy
                        if (currentOG.GetChild(i).GetComponent<UsuallyEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyEnemy>().otherOG = true;
                        break;

                    case "DoubleEnemy":// Если тип DoubleEnemy
                        if (currentOG.GetChild(i).GetComponent<DoubleEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<DoubleEnemy>().otherOG = true;
                        break;

                    case "CupEnemy":// Если тип CupEnemy
                        if (currentOG.GetChild(i).GetComponent<CupEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<CupEnemy>().otherOG = true;
                        break;

                    case "UsuallyBoss":// Если тип UsuallyBoss
                        if (currentOG.GetChild(i).GetComponent<UsuallyBoss>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyBoss>().otherOG = true;
                        break;

                    case "ChickenBoss":// Если тип ChickenBoss
                        if (currentOG.GetChild(i).GetComponent<ChickenBoss>() != null)
                            currentOG.GetChild(i).GetComponent<ChickenBoss>().otherOG = true;
                        break;

                    default:
                        break;
                }
            }
        }

        ObjectGroup og = objectGroup.GetComponent<ObjectGroup>();// Получаем объект
        if (og.allEnemyIsDead)// Если все мертвы
        {
            return;// Если все враги мертвы выходим из метода
        }
        else
        {
            countEnemiesGroup += 1;
            splineFollower.follow = false;// Прекращаем идти
            animHead.SetInteger("IndexAnim", 3);// Вкл анимации Idle
        }

        og.gp = this;// Если кто-то жив, передаём этот скрипт
        Transform ogTrans = og.transform;
        currentOG = og.transform;

        for (int i = 0; i < ogTrans.childCount; i++)
        {
            objectGroup.GetChild(i).gameObject.layer = 0;
            switch (ogTrans.GetChild(i).tag)// Проверяем типы
            {
                case "UsuallyEnemy":// Если тип UsuallyEnemy
                    if (objectGroup.GetChild(i).GetComponent<UsuallyEnemy>() != null)
                        objectGroup.GetChild(i).GetComponent<UsuallyEnemy>().attack = true;// Включаем им атаку
                    break;

                case "DoubleEnemy":// Если тип DoubleEnemy
                    if (objectGroup.GetChild(i).GetComponent<DoubleEnemy>() != null)
                        objectGroup.GetChild(i).GetComponent<DoubleEnemy>().attack = true;// Включаем им атаку
                    break;

                case "CupEnemy":// Если тип CupEnemy
                    if (objectGroup.GetChild(i).GetComponent<CupEnemy>() != null)
                        objectGroup.GetChild(i).GetComponent<CupEnemy>().attack = true;// Включаем им атаку
                    break;

                case "UsuallyBoss":// Если тип UsuallyBoss
                    if (objectGroup.GetChild(i).GetComponent<UsuallyBoss>() != null)
                        objectGroup.GetChild(i).GetComponent<UsuallyBoss>().attack = true;// Включаем им атаку
                    break;

                case "ChickenBoss":// Если тип ChickenBoss
                    if (objectGroup.GetChild(i).GetComponent<ChickenBoss>() != null)
                        objectGroup.GetChild(i).GetComponent<ChickenBoss>().attack = true;// Включаем им атаку
                    break;

                default:
                    break;
            }
        }
    }

    /// Метод в котором нападают враги (до WaitForKill)
    /// 
    /// Нападаем на игрока (если кто-то жив)
    /// 
    //public void ForKill(GameObject objectGroup)// Получаем трансформ группы врагов
    public void ForKill(Transform objectGroup)// Получаем трансформ группы врагов
    {
        doubleElectro = false;
        door = false;

        if (currentOG != null)
        {
            for (int i = 0; i < currentOG.childCount; i++)
            {
                switch (currentOG.GetChild(i).tag)// Проверяем типы
                {
                    case "UsuallyEnemy":// Если тип UsuallyEnemy
                        if (currentOG.GetChild(i).GetComponent<UsuallyEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyEnemy>().otherOG = true;
                        break;

                    case "DoubleEnemy":// Если тип DoubleEnemy
                        if (currentOG.GetChild(i).GetComponent<DoubleEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<DoubleEnemy>().otherOG = true;
                        break;

                    case "CupEnemy":// Если тип CupEnemy
                        if (currentOG.GetChild(i).GetComponent<CupEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<CupEnemy>().otherOG = true;
                        break;

                    case "UsuallyBoss":// Если тип UsuallyBoss
                        if (currentOG.GetChild(i).GetComponent<UsuallyBoss>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyBoss>().otherOG = true;
                        break;

                    case "ChickenBoss":// Если тип ChickenBoss
                        if (currentOG.GetChild(i).GetComponent<ChickenBoss>() != null)
                            currentOG.GetChild(i).GetComponent<ChickenBoss>().otherOG = true;
                        break;

                    default:
                        break;
                }
            }
        }

        ObjectGroup og = objectGroup.GetComponent<ObjectGroup>();// Получаем объект
        if (og.allEnemyIsDead == false)// Если все мертвы
        {
            Transform ogTrans = og.transform;
            currentOG = og.transform;

            for (int i = 0; i < ogTrans.childCount; i++)
            {
                objectGroup.GetChild(i).gameObject.layer = 0;
                switch (ogTrans.GetChild(i).tag)// Проверяем типы
                {
                    case "UsuallyEnemy":// Если тип UsuallyEnemy
                        if (objectGroup.GetChild(i).GetComponent<UsuallyEnemy>() != null)
                        {
                            objectGroup.GetChild(i).GetComponent<UsuallyEnemy>().attack = true;// Включаем им атаку
                            forKillEnemies += 1;
                        }
                        break;

                    case "DoubleEnemy":// Если тип UsuallyEnemy
                        if (objectGroup.GetChild(i).GetComponent<DoubleEnemy>() != null)
                        {
                            objectGroup.GetChild(i).GetComponent<DoubleEnemy>().attack = true;// Включаем им атаку
                            forKillEnemies += 1;
                        }
                        break;

                    case "CupEnemy":// Если тип CupEnemy
                        if (objectGroup.GetChild(i).GetComponent<CupEnemy>() != null)
                            objectGroup.GetChild(i).GetComponent<CupEnemy>().attack = true;// Включаем им атаку
                        break;

                    case "UsuallyBoss":// Если тип UsuallyBoss
                        if (objectGroup.GetChild(i).GetComponent<UsuallyBoss>() != null)
                        {
                            objectGroup.GetChild(i).GetComponent<UsuallyBoss>().attack = true;// Включаем им атаку
                            forKillEnemies += 1;
                        }
                        break;

                    case "ChickenBoss":// Если тип ChickenBoss
                        if (objectGroup.GetChild(i).GetComponent<ChickenBoss>() != null)
                        {
                            objectGroup.GetChild(i).GetComponent<ChickenBoss>().attack = true;// Включаем им атаку
                            forKillEnemies += 1;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void ChangeSpeed(float speed)
    {
        splineFollower.followSpeed = speed;
    }

    ///
    /// Метод при достижении конца
    public void Finish()
    {
        finish = true;
        GameObject.FindGameObjectWithTag("SuperPunch").gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("UiManager").GetComponent<UIManager>().panelLevelWin.gameObject.SetActive(true);
        if (currentOG != null)
        {
            for (int i = 0; i < currentOG.childCount; i++)
            {
                switch (currentOG.GetChild(i).tag)// Проверяем типы
                {
                    case "UsuallyEnemy":// Если тип UsuallyEnemy
                        if (currentOG.GetChild(i).GetComponent<UsuallyEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyEnemy>().otherOG = true;
                        break;

                    case "DoubleEnemy":// Если тип DoubleEnemy
                        if (currentOG.GetChild(i).GetComponent<DoubleEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<DoubleEnemy>().otherOG = true;
                        break;

                    case "CupEnemy":// Если тип CupEnemy
                        if (currentOG.GetChild(i).GetComponent<CupEnemy>() != null)
                            currentOG.GetChild(i).GetComponent<CupEnemy>().otherOG = true;
                        break;

                    case "UsuallyBoss":// Если тип UsuallyBoss
                        if (currentOG.GetChild(i).GetComponent<UsuallyBoss>() != null)
                            currentOG.GetChild(i).GetComponent<UsuallyBoss>().otherOG = true;
                        break;

                    case "ChickenBoss":// Если тип ChickenBoss
                        if (currentOG.GetChild(i).GetComponent<ChickenBoss>() != null)
                            currentOG.GetChild(i).GetComponent<ChickenBoss>().otherOG = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
