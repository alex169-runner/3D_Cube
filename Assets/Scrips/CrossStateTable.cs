using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrossStateTable", menuName = "CubeSolver/CrossStateTable")]
public class CrossStateTable : ScriptableObject
{
    [System.Serializable]
    public class StateSolution
    {
        public long stateHash;
        public List<int> solutionMoves;
    }

    public List<StateSolution> stateSolutions = new();
    public int totalStates;
    public string buildDate;

    [System.NonSerialized]
    private SortedDictionary<long, List<int>> solutionCache;

    private void OnEnable()
    {
        if (solutionCache == null) {
            BuildCache();
        }
    }

    public void BuildCache()
    {
        solutionCache = new();
        foreach (var solution in stateSolutions) {
            solutionCache.Add(solution.stateHash, solution.solutionMoves);
        }
    }

    public bool TryGetSolution(long stateHash, out List<int> solution)
    {
        if (solutionCache != null && solutionCache.TryGetValue(stateHash, out solution)) {
            return true;
        } else {
            Debug.Log("cross state not found");
        }

        solution = null;
        return false;
    }
}