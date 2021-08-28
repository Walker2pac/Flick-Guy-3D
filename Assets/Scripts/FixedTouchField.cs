using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 TouchDis;
    Vector2 PointerOld;
    int PointerId;
    public bool Pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }

    void Update()
    {
        if(Pressed == true)
        {
            if(PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDis = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDis = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDis = new Vector2();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
