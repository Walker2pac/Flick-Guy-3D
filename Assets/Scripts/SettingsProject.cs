using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

[CreateAssetMenu(fileName = "AllSettingsProject", menuName = "SO/Settings")]
public class SettingsProject : ScriptableObject
{
    [Tooltip("Скорость игрока")]
    [SerializeField] float playerSpeed;
    [Tooltip("Скорость поворота игрока при управлении персонажем")]
    [SerializeField] float playerSpeedRotation;
    [Tooltip("Скорость поворота персонажа без управления игроком")]
    [SerializeField] float toTargetSpeedRotation;
    [Tooltip("Сила отталкивания врага")]
    [SerializeField] float dropForce;
    [Tooltip("Сила отталкивания врага вверх")]
    [SerializeField] float upDropForce;
    [Tooltip("Сила удара игрока")]
    [SerializeField] float playerAttackDistance;
    [Tooltip("Количество жизней у игрока")]
    [SerializeField] int playerHealth;
    [Tooltip("Сколько времени висит враг над вентилятором живой")]
    [SerializeField] float timeDeathVent;
    [Tooltip("Как расстояние от окна до игрокад олжно быть, чтобы слышать звуки")]
    [SerializeField] float glassplateSoundDistance;
    [Tooltip("Сколько смертей для суперудара")]
    [SerializeField] int deathForSuperPunch;
    [Tooltip("Сила суперудара")]
    [SerializeField] float forceForSuperPunch;

    public float PlayerSpeed
    {
        get {
            return playerSpeed;
        }
    }

    public float PlayerSpeedRotation
    {
        get
        {
            return playerSpeedRotation;
        }
    }

    public float ToTargetSpeedRotation
    {
        get
        {
            return toTargetSpeedRotation;
        }
    }

    public float DropForce
    {
        get
        {
            return dropForce;
        }
    }

    public float UpDropForce
    {
        get
        {
            return upDropForce;
        }
    }

    public float PlayerAttackDistance
    { 
        get
        {
            return playerAttackDistance;
        }
    }

    public int PlayerHealth
    {
        get
        {
            return playerHealth;
        }
    }

    public float TimeDeathVent
    {
        get
        {
            return timeDeathVent;
        }
    }

    public float GlassplateSoundDistance
    {
        get
        {
            return glassplateSoundDistance;
        }
    }

    public int DeathForSuperPunch
    {
        get
        {
            return deathForSuperPunch;
        }
    }

    public float ForceForSuperPunch
    {
        get
        {
            return forceForSuperPunch;
        }
    }
}
