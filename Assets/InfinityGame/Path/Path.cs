using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityGame.Path
{
    [System.Serializable]
    public class Path
    {
        [SerializeField, HideInInspector] private List<Vector2> points;

        public Path(Vector2 center)
        {
            points = new List<Vector2>
            {
                center + Vector2.left,
                center + (Vector2.left + Vector2.up) * 0.5f,
                center + (Vector2.right + Vector2.down) * 0.5f,
                center + Vector2.right,
            };
        }

        public void AddSegment(Vector2 anchorPos)
        {
            points.Add(points[^1] * 2 - points[^2]);
            points.Add((points[^1] + anchorPos) / 2f);
            points.Add(anchorPos);
        }
    }
}