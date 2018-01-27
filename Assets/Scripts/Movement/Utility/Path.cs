using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Defines a path for platforms to follow
/// </summary>
public class Path : MonoBehaviour
{
    /// the array of points that make up the path
    public Transform[] points;

    public IEnumerator<Transform> GetPathEnumerator()
    {
        if (points == null || points.Length < 1)
        {
            yield break;
        }

        var direction = 1;
        var index = 0;
        while (true)
        {
            yield return points[index];

            if (points.Length == 1)
                continue;

            if (index <= 0)
                direction = 1;

            else if (index >= points.Length - 1)
                direction = -1;

            index = index + direction;
        }
    }

    public void OnDrawGizmos()
    {

        if (this.points == null || this.points.Length < 2)
            return;

        var localPoints = points.Where(t => t != null).ToList();

        if (localPoints.Count < 2)
            return;

        for (var i = 1; i < localPoints.Count; i++)
        {
            Gizmos.DrawLine(localPoints[i - 1].position, localPoints[i].position);
        }

    }
}
