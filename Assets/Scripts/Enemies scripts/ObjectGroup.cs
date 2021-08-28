using UnityEngine;

public class ObjectGroup : MonoBehaviour
{
    public int allEnemy;// Всего врагов в ObjectGroup
    public int deadEnemy;// Умерло всего врагов в ObjectGroup
    public bool allEnemyIsDead;// Галочка, если все враги мертвы

    Transform trans;
    [HideInInspector]
    public GamePlay gp;

    [HideInInspector]
    public float dopTime;

    private void Awake()
    {
        trans = transform;
        Invoke("CheakEnemy", 1f);
        //allEnemy = trans.childCount;

        for (int i = 0; i < trans.childCount; i++)
        {
            switch (trans.GetChild(i).tag)// Проверяем тип
            {
                case "UsuallyEnemy":
                    if (trans.GetChild(i).GetComponent<UsuallyEnemy>() != null) allEnemy += 1;
                    break;

                case "DoubleEnemy":
                    if (trans.GetChild(i).GetComponent<DoubleEnemy>() != null) allEnemy += 1;
                    break;

                case "CupEnemy":
                    if (trans.GetChild(i).GetComponent<CupEnemy>() != null) allEnemy += 1;
                    break;

                case "UsuallyBoss":
                    if (trans.GetChild(i).GetComponent<UsuallyBoss>() != null) allEnemy += 1;
                    break;

                case "ChickenBoss":
                    if (trans.GetChild(i).GetComponent<ChickenBoss>() != null) allEnemy += 1;
                    break;

                default:
                    break;
            }
        }  
    }

    private void Update()
    {
        if (deadEnemy >= allEnemy || trans.childCount <= 0)// Если все враги мертвы
        {
            if (gp != null && !allEnemyIsDead) Invoke("OffAttack", 0.5f + dopTime);
            allEnemyIsDead = true;// Галочка. что все мертвы
        }
    }

    void OffAttack()
    {
        gp.AttackOff();// Выключаем атаку у игрока
        dopTime = 0;
    }

    void CheakEnemy()// Метод проверки все-ли мертвы
    {
        if (allEnemyIsDead) return;
        deadEnemy = 0;

        for (int i = 0; i < trans.childCount; i++)
        {
            switch (trans.GetChild(i).tag)// Проверяем тип
            {
                case "UsuallyEnemy":
                    if (trans.GetChild(i).GetComponent<UsuallyEnemy>() != null && trans.GetChild(i).GetComponent<UsuallyEnemy>().death == true) deadEnemy += 1;
                    break;

                case "DoubleEnemy":
                    if (trans.GetChild(i).GetComponent<DoubleEnemy>() != null && trans.GetChild(i).GetComponent<DoubleEnemy>().deathFull == true) deadEnemy += 1;
                    break;

                case "CupEnemy":
                    if (trans.GetChild(i).GetComponent<CupEnemy>() != null && trans.GetChild(i).GetComponent<CupEnemy>().death == true) deadEnemy += 1;
                    break;

                case "UsuallyBoss":
                    if (trans.GetChild(i).GetComponent<UsuallyBoss>() != null && trans.GetChild(i).GetComponent<UsuallyBoss>().death == true) deadEnemy += 1;
                    break;

                case "ChickenBoss":
                    if (trans.GetChild(i).GetComponent<ChickenBoss>() != null && trans.GetChild(i).GetComponent<ChickenBoss>().death == true) deadEnemy += 1;
                    break;

                //case "Chicken":
                //    if (trans.GetChild(i).GetComponent<Chicken>() != null && trans.GetChild(i).GetComponent<Chicken>().death == true) deadEnemy += 1;
                //    break;

                default:
                    break;
            }
        }

        Invoke("CheakEnemy", 1f);
    }
}
