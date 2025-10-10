using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnFromDetect : MonoBehaviour
{
    [Header("Data source")]
    public Listener listener;        

    [Header("Reference")]
    public Transform reference;                
    public Vector3 direction = Vector3.forward;

    [Header("Prefabs")]
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject greenPrefab;

    [Header("Scaling")]
    [Tooltip("Multiply incoming meters by this to convert to Unity units.")]
    public float metersToUnity = 1.0f;

    [Header("Spawn/Place")]
    public Transform parent;                   // optional
    public bool updateContinuously = true;     // move each frame as new distances arrive
    public float LerpSpeed = 12f; 

    [Tooltip("If distance_m <= 0 (missing), use this fallback (meters).")]
    public float fallbackMeters = 0.5f;
    
    private readonly Dictionary<string, GameObject> _spawned = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);


    void Awake()
    {
        listener = FindAnyObjectByType<Listener>();
    }

    void Update()
    {
        // var pairs = _listener.GetLatestPairs();
        // if (pairs.Count > 0)
        // {
        //     var line = string.Join(" | ",
        //         pairs.Select(p => $"{p.cube_label}: {p.distance_m:F3} m"));
        //     Debug.Log(line);
        // }

        if (listener == null) return;

        var pairs = listener.GetLatestPairs();
        if (pairs == null || pairs.Count == 0) return;

        var red = GetNearestByColor(pairs, "red");
        var blue = GetNearestByColor(pairs, "blue");
        var green = GetNearestByColor(pairs, "green");

        TrySpawnOrMove("red", red, redPrefab);
        TrySpawnOrMove("blue", blue, bluePrefab);
        TrySpawnOrMove("green", green, greenPrefab);
    }

    Pair GetNearestByColor(IReadOnlyList<Pair> pairs, string color)
    {
        Pair best = null;
        float bestD = float.PositiveInfinity;
        foreach (var p in pairs)
        {
            if (!LabelIs(p.cube_label, color)) continue;
            var d = p.distance_m > 0f ? p.distance_m : fallbackMeters; // use fallback when missing
            if (d < bestD) { bestD = d; best = p; }
        }
        return best;
    }

    void TrySpawnOrMove(string key, Pair p, GameObject prefab)
    {
        if (p == null || prefab == null) return;

        // Spawn once
        if (!_spawned.TryGetValue(key, out var go) || go == null)
        {
            go = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
            _spawned[key] = go;
            Debug.Log($"Spawned {key} prefab");
        }

        // Desired world position: reference.position + (reference-space direction) * meters
        float meters = p.distance_m > 0f ? p.distance_m : fallbackMeters;
        Vector3 worldDir = reference.TransformDirection(direction.normalized);
        Vector3 targetPos = reference.position + worldDir * meters;

        if (!updateContinuously || LerpSpeed <= 0f)
        {
            go.transform.position = targetPos;
        }
        else
        {
            go.transform.position = Vector3.Lerp(go.transform.position, targetPos, 1f - Mathf.Exp(-LerpSpeed * Time.deltaTime));
        }
    }
    
    static bool LabelIs(string label, string color)
    {
        if (string.IsNullOrEmpty(label)) return false;
        return label.Equals(color, StringComparison.OrdinalIgnoreCase)
            || label.StartsWith(color, StringComparison.OrdinalIgnoreCase)
            || label.IndexOf(color, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}