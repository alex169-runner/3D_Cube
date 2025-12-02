using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "F2LStateTable", menuName = "CubeSolver/F2LStateTable")]
public class F2LStateTable : ScriptableObject
{
    [System.Serializable]
    public class StateSolution
    {
        public long stateHash;
        public List<int> solutionMoves;
    }

    public string buildDate;
    [SerializeField] private List<StateSolution> stateSolutions0 = new();
    [SerializeField] private List<StateSolution> stateSolutions1 = new();
    [SerializeField] private List<StateSolution> stateSolutions2 = new();
    [SerializeField] private List<StateSolution> stateSolutions3 = new();

    public List<StateSolution> stateSolutions(int layer)
    {
        return layer switch
        {
            0 => stateSolutions0,
            1 => stateSolutions1,
            2 => stateSolutions2,
            3 => stateSolutions3,
            _ => null
        };
    }

    [System.NonSerialized]
    private SortedDictionary<long, List<int>>[] solutionCache;

    private void OnEnable()
    {
        if (solutionCache == null) {
            BuildCache();
        }
    }

    private void BuildCache()
    {
        solutionCache = new SortedDictionary<long, List<int>>[4];
        for (int i = 0; i < 4; ++i) {
            solutionCache[i] = new();
            var solutions = stateSolutions(i);
            foreach (var s in solutions) {
                solutionCache[i].Add(s.stateHash, s.solutionMoves);
            }
            Debug.Log($"{i}-th Cache built with {solutionCache[i].Count} valid states");
        }
    }

    public bool TryGetSolution(int layer, long stateHash, out List<int> solution)
    {
        if (solutionCache != null && solutionCache[layer].TryGetValue(stateHash, out solution)) {
            return true;
        }
        else {
            Debug.Log($"{layer}-th F2L state not found");
        }

        solution = null;
        return false;
    }
}