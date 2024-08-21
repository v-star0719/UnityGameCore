using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

namespace GameCore.Common
{
    public class PolygonRect : Polygon
    {
        public void Init(Vector2 l, Vector2 r, Vector2 t, Vector2 b)
        {
            Init(new List<Vector2>() { l, r, t, b }, Vector2.zero);
        }

        public void UpdatePoints(Vector2 l, Vector2 r, Vector2 t, Vector2 b)
        {
            vertices[0].pos = l;
            vertices[1].pos = r;
            vertices[2].pos = t;
            vertices[3].pos = b;
        }
    }
}