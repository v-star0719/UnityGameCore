using System.Collections;
using System.Collections.Generic;
using GameCore.Common;
using UnityEngine;

[ExecuteAlways]
public class PolygonTest : MonoBehaviour
{
    public Transform polygonRoot1;
    public Transform polygonRoot2;
    public float gizmosSize = 0.1f;

    private Polygon polygon1;
    private Polygon polygon2;

    // Use this for initialization
    void Start()
    {
        polygon1 = new Polygon();
        polygon2 = new Polygon();
    }

    void OnEnable()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
        polygon1.PolygonIntersect(polygon2);
    }

    public void Refresh()
    {
        RefreshPolygon(polygon1, polygonRoot1);
        RefreshPolygon(polygon2, polygonRoot2);
    }

    private void RefreshPolygon(Polygon polygon, Transform polygonRoot)
    {
        if (polygonRoot == null)
        {
            return;
        }

        var points = new List<Vector2>();
        Vector2 center = Vector2.zero;
        for (int i = 0; i < polygonRoot.childCount; i++)
        {
            var point = polygonRoot.GetChild(i);
            points.Add(point.position);
            center += new Vector2(point.position.x, point.position.y);
        }
        center /= polygonRoot.childCount;
        polygon.Init(points, center);
    }

    private void OnDrawGizmos()
    {
        polygon1.DrawGizmos(gizmosSize);
        polygon2.DrawGizmos(gizmosSize);
    }
}