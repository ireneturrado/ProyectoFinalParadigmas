using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Transform> FindPath(NavGraph g, Transform start, Transform goal)
    {
        var open = new List<Transform>();
        var cameFrom = new Dictionary<Transform, Transform>();
        var gScore = new Dictionary<Transform, float>();
        var fScore = new Dictionary<Transform, float>();

        open.Add(start);
        gScore[start] = 0f;
        fScore[start] = Heuristic(start, goal);

        while (open.Count > 0)
        {
            Transform current = LowestF(open, fScore);
            if (current == goal)
                return Reconstruct(cameFrom, current);

            open.Remove(current);

            foreach (var nb in g.Neighbors(current))
            {
                float tentative = gScore[current] + g.Cost(current, nb);

                if (!gScore.ContainsKey(nb) || tentative < gScore[nb])
                {
                    cameFrom[nb] = current;
                    gScore[nb] = tentative;
                    fScore[nb] = tentative + Heuristic(nb, goal);

                    if (!open.Contains(nb))
                        open.Add(nb);
                }
            }
        }

        return new List<Transform>();
    }

    static float Heuristic(Transform a, Transform b)
    {
        return Vector3.Distance(a.position, b.position);
    }

    static Transform LowestF(List<Transform> open, Dictionary<Transform, float> f)
    {
        Transform best = open[0];
        float bestV = f.ContainsKey(best) ? f[best] : float.PositiveInfinity;

        for (int i = 1; i < open.Count; i++)
        {
            var n = open[i];
            float v = f.ContainsKey(n) ? f[n] : float.PositiveInfinity;
            if (v < bestV)
            {
                bestV = v;
                best = n;
            }
        }
        return best;
    }

    static List<Transform> Reconstruct(Dictionary<Transform, Transform> cameFrom, Transform current)
    {
        var path = new List<Transform> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
