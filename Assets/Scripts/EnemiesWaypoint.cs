using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesWaypoint : MonoBehaviour
{
    public List<Image> imgEnemy = new List<Image>();
    public List<Transform> transEnemy = new List<Transform>();

    private void Update()
    {
        if (imgEnemy.Count > 0)
        {
            for (int i = 0; i < imgEnemy.Count; i++)
            {
                float minX = imgEnemy[i].GetPixelAdjustedRect().width / 2;
                float maxX = Screen.width - minX;

                float minY = imgEnemy[i].GetPixelAdjustedRect().height / 2;
                float maxY = Screen.width - minY;

                Vector2 pos = Camera.main.WorldToScreenPoint(transEnemy[i].position + new Vector3(0, 1.25f, 0));

                if (Vector3.Dot((transEnemy[i].position - transform.position), transform.forward) < 0)
                {
                    if (pos.x < Screen.width / 2)
                    {
                        pos.x = maxX;
                    }
                    else
                    {
                        pos.x = minX;
                    }
                }

                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                imgEnemy[i].transform.position = pos;
            }
        }
    }
}
