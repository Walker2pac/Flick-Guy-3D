using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float xRot;
    float yRot;
    public float speedRotate;
    public FixedTouchField fixedTouchField;
    public bool playerLook;

    Transform trans;

    private void Awake()// В самом старте игры
    {
        fixedTouchField = GameObject.FindGameObjectWithTag("TouchPanel").GetComponent<FixedTouchField>();// Ищем объект на сцене с тегом TouchPanel и получаем его компонент
    }

    private void Start()
    {
        trans = transform;
    }

    void Update()
    {
        // Rotate camera

        if(playerLook)
        {
            xRot = fixedTouchField.TouchDis.x / 20;
            yRot = fixedTouchField.TouchDis.y / 20;

            Vector3 camRotation = new Vector3(yRot, 0, 0) * speedRotate;
            Vector3 rotation = new Vector3(0, xRot, 0) * speedRotate;

            transform.Rotate(-camRotation);
            transform.Rotate(rotation);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        }        
    }
}
