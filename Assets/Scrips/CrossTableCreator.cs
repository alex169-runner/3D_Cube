using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CrossTableCreator : MonoBehaviour
{
    const long start = 0xC3A1L;

    Queue<long> q = new();
    SortedSet<long> vis = new();
    SortedDictionary<long, long> pre = new();
    SortedDictionary<long, int> back = new();

    int[,] edgesInFace = new int[6, 4]
    {
        { 4, 8, 12, 5 },
        { 2, 6, 10, 7 },
        { 9, 12, 11, 10 },
        { 1, 2, 3, 4 },
        { 1, 5, 9, 6 },
        { 3, 7, 11, 8 },
    };

    public CrossStateTable CreateTableAsset()
    {
        Debug.Log("Start Creating Cross State Table...");

        CrossStateTable table = ScriptableObject.CreateInstance<CrossStateTable>();

        q.Enqueue(start);
        vis.Add(start);

        while (q.Count > 0) {
            long cur = q.Dequeue();

            for (int i = 0; i <= 10; i += 2) {
                long cw = Rotate(i, cur, false);
                if (!vis.Contains(cw)) {
                    vis.Add(cw);
                    pre.Add(cw, cur);
                    back.Add(cw, i << 1 | 1);
                    q.Enqueue(cw);
                }

                long ccw = Rotate(i, cur, true);
                if (!vis.Contains(ccw)) {
                    vis.Add(ccw);
                    pre.Add(ccw, cur);
                    back.Add(ccw, i << 1);
                    q.Enqueue(ccw);
                }
            }
        }
        
        Debug.Log("BFS ended, start building solution routes...");

        int validStates = 0;
        foreach (var state in vis) {
            List<int> route = new();
            long cur = state;
            while (cur != start) {
                route.Add(back[cur]);
                cur = pre[cur];
            }
            table.stateSolutions.Add(new() { stateHash = state, solutionMoves = route });
            ++validStates;
        }

        Debug.Log($"{validStates} valid states found");

        table.totalStates = validStates;
        table.buildDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Debug.Log("Cross State Table built sucessfully£¡");
        return table;
    }

    long Rotate(int face, long hash, bool reversed)
    {
        long core = GetCore(face, hash);

        for (int i = 0; i < 4; ++i)
            if (((core >> (i * 4)) & 0xFL) != 0)
                core ^= 0x8L << (i * 4);
        
        if (reversed) core = (core >> 4) + ((core & 0xFL) << 12);
        else core = ((core & 0xFFFL) << 4) + (core >> 12);

        return WrapCore(face, core, hash);
    }

    long GetCore(int face, long hash)
    {
        long core = 0;
        for (int i = 0; i < 4; ++i) {
            core += ((hash >> ((edgesInFace[face / 2, i] - 1) * 4)) & 0xFL) << (i * 4);
        }
        return core;
    }

    long WrapCore(int face, long core, long origin)
    {
        long hash = 0;
        long mask = 0xFFFFFFFFFFFFL;
        for (int i = 0; i < 4; ++i) {
            hash += ((core >> (i * 4)) & 0xFL) << ((edgesInFace[face / 2, i] - 1) * 4);
            mask -= 0xFL << ((edgesInFace[face / 2, i] - 1) * 4);
        }
        return hash + (mask & origin);
    }
}