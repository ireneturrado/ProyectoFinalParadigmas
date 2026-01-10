using System.Collections.Generic;
using UnityEngine;

public class NavGraph : MonoBehaviour
{
    [Header("Nodes (usa los waypoints existentes)")]
    public Transform[] nodes;

    [Header("Conexión automática")]
    public LayerMask obstacleMask;
    public float maxLinkDistance = 12f;
    public float rayHeight = 1.0f;

    private Dictionary<Transform, List<Transform>> adj = new();

    void Awake()
    {
        Build();
    }

    public void Build()
    {
        adj.Clear();
        if (nodes == null || nodes.Length == 0) return;

        foreach (var n in nodes)
            if (n != null) adj[n] = new List<Transform>();

        for (int i = 0; i < nodes.Length; i++)
            for (int j = i + 1; j < nodes.Length; j++)
            {
                var a = nodes[i];
                var b = nodes[j];
                if (a == null || b == null) continue;

                float d = Vector3.Distance(a.position, b.position);
                if (d > maxLinkDistance) continue;

                Vector3 o = a.position + Vector3.up * rayHeight;
                Vector3 dir = b.position - a.position;
                dir.y = 0f;

                if (Physics.Raycast(o, dir.normalized, d, obstacleMask))
                    continue;

                adj[a].Add(b);
                adj[b].Add(a);
            }
    }

    public IEnumerable<Transform> Neighbors(Transform n)
    {
        if (n == null || !adj.ContainsKey(n)) yield break;
        foreach (var x in adj[n]) yield return x;
    }

    public Transform GetClosestNode(Vector3 pos)
    {
        Transform best = null;
        float bestD = float.PositiveInfinity;

        foreach (var n in nodes)
        {
            if (n == null) continue;
            float d = Vector3.Distance(pos, n.position);
            if (d < bestD)
            {
                bestD = d;
                best = n;
            }
        }
        return best;
    }

    public float Cost(Transform a, Transform b)
    {
        return Vector3.Distance(a.position, b.position);
    }
}
