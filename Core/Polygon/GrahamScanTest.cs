using System.Collections;
using System.Collections.Generic;
using GameCore.Common;
using UnityEngine;

[ExecuteAlways]
public class GrahamScanTest : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    void OnEnable()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        List<Vector2> points = new List<Vector2>();
        for(int i = 0; i < transform.childCount; i++)
        {
            var point = transform.GetChild(i).position;
            points.Add(new Vector2(point.x, point.y));
            Gizmos.DrawSphere(point, 0.1f);
        }
        var rt = GrahamScan.Calculate(points);
        for (int i = 0; i < rt.Count - 1; i++)
        {
            Gizmos.DrawLine(new Vector3(rt[i].x, rt[i].y), new Vector3(rt[i + 1].x, rt[i + 1].y, 0));
        }

        if (rt.Count > 1)
        {
            Gizmos.DrawLine(new Vector3(rt[^1].x, rt[^1].y), new Vector3(rt[0].x, rt[0].y, 0));
        }
    }
}