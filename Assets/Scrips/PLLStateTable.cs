using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLLStateTable : ScriptableObject
{
    [System.Serializable]
    public class StateSolution
    {
        public long stateHash;
        public List<int> solutionMoves;
    }

    public string buildDate;
    public List<StateSolution> stateSolutions = new();

    [System.NonSerialized]
    private SortedDictionary<long, List<int>> solutionCache;

    private void OnEnable()
    {
        if (solutionCache == null) {
            BuildCache();
        }
    }

    private void BuildCache()
    {
        solutionCache = new();
        foreach (var s in stateSolutions) {
            solutionCache.Add(s.stateHash, s.solutionMoves);
        }
        Debug.Log($"PLL Cache built with {solutionCache.Count} valid states");
    }

    public bool TryGetSolution(long stateHash, out List<int> solution)
    {
        if (solutionCache != null && solutionCache.TryGetValue(stateHash, out solution)) {
            return true;
        }
        else {
            Debug.Log("PLL state not found in dictionary");
        }

        solution = null;
        return false;
    }
}
