using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F2LTableCreator : MonoBehaviour
{
    const long start = (1L << 18) + (1L << 35);

    int[,] edgesInFace = new int[6, 4]
    {
        { 4, 8, 12, 5 },
        { 2, 6, 10, 7 },
        { 9, 12, 11, 10 },
        { 1, 2, 3, 4 },
        { 1, 5, 9, 6 },
        { 3, 7, 11, 8 },
    };

    int[,] cornersInFace = new int[6, 4]
    {
        { 16, 20, 17, 13 },
        { 14, 18, 19, 15 },
        { 17, 20, 19, 18 },
        { 13, 14, 15, 16 },
        { 13, 17, 18, 14 },
        { 15, 19, 20, 16 },
    };

    int[,,] faceLock = new int[4, 4, 2]
    {
        { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
        { { 1, 0 }, { 0, 0 }, { 0, 0 }, { 0, 1 } },
        { { 1, 0 }, { 0, 0 }, { 0, 1 }, { 1, 1 } },
        { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, 1 } },
    };

    int[] faces = new int[5]
    {
        8, 2, 10, 0, 4
    };

    int[] faceToID = new int[6]
    {
        3, 1, 4, -1, 0, 2
    };

    public F2LStateTable CreateTableAsset()
    {
        Debug.Log("Start creating F2L table asset");

        F2LStateTable table = ScriptableObject.CreateInstance<F2LStateTable>();

        for (int t = 0; t < 4; ++t) {
            Debug.Log($"{t}-th bfs starts");

            var stateSolutions = table.stateSolutions(t);

            SortedSet<long> vis = new();
            SortedDictionary<long, long> pre = new();
            SortedDictionary<long, int> back = new();

            Queue<long> q = new();
            vis.Add(start);
            q.Enqueue(start);

            while (q.Count > 0) {
                long cur = q.Dequeue();

                for (int i = 0; i < 5; ++i) {
                    if (i < 4) {
                        long baseOri = (cur >> (i * 2)) & 3L;
                        long leftOri = (cur >> (((i + 3) % 4) * 2)) & 3L;
                        long rightOri = (cur >> (((i + 1) % 4) * 2)) & 3L;
                        if (baseOri == 0L) {
                            if (leftOri == 3L || rightOri == 1L) continue;
                            for (int j = 0; j < 2; ++j) {
                                if (faceLock[t, i, j] == 0 && (faceLock[t, i, 1 - j] == 0 || (j == 0 ? rightOri : leftOri) == 0L)) {
                                    long next = Rotate(faces[i], cur, j == 1);
                                    if (!vis.Contains(next)) {
                                        q.Enqueue(next);
                                        vis.Add(next);
                                        pre[next] = cur;
                                        back[next] = (faces[i] << 1) + (1 - j); 
                                    }
                                }
                            }
                        } else if (baseOri == 3L) {
                            if (leftOri != 3L) {
                                long next = Rotate(faces[i], cur, false);
                                if (!vis.Contains(next)) {
                                    q.Enqueue(next);
                                    vis.Add(next);
                                    pre[next] = cur;
                                    back[next] = faces[i] << 1 | 1;
                                }
                            }
                        } else if (baseOri == 1L) {
                            if (rightOri != 1L) {
                                long next = Rotate(faces[i], cur, true);
                                if (!vis.Contains(next)) {
                                    q.Enqueue(next);
                                    vis.Add(next);
                                    pre[next] = cur;
                                    back[next] = faces[i] << 1;
                                }
                            }
                        }
                    } else {
                        for (int j = 0; j < 2; ++j) {
                            long next = Rotate(faces[i], cur, j == 1);
                            if (!vis.Contains(next)) {
                                q.Enqueue(next);
                                vis.Add(next);
                                pre[next] = cur;
                                back[next] = (faces[i] << 1) + (1 - j);
                            }
                        }
                    }
                }
            }

            Debug.Log($"{t}-th bfs ends");

            int totLength = 0;
            foreach (var state in vis) {
                for (int j = 0; j < 4; ++j) {
                    if (((state >> (j * 2)) & 3L) == 2) {
                        Debug.Log("2 appeared");
                    }
                }
                if ((state & 255L) > 0) continue;
                if ((state >> 16 & 1) != 0) {
                    Debug.Log("edge's position crooked");
                }
                if ((state >> 32 & 1) != 0) {
                    Debug.Log("cornor's position crooked");
                }
                List<int> route = new();
                long cur = state;
                while (cur != start) {
                    route.Add(back[cur]);
                    cur = pre[cur];
                }
                totLength += route.Count;
                stateSolutions.Add(new() { stateHash = state, solutionMoves = route });
            }

            Debug.Log($"{t}-th table with {stateSolutions.Count} valid states and {(float)totLength / stateSolutions.Count} as the average path length built");
        }

        table.buildDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("All 4 tables are successfully set");

        return table;
    }

    long Rotate(int face, long hash, bool reversed)
    {
        long core = GetCore(face, hash);

        for (int i = 0; i < 4; ++i)
            if (((core >> (i * 2 + 8)) & 3L) != 0)
                core ^= 2L << (i * 2 + 8);

        if (face != 4) {
            for (int i = 0; i < 4; ++i) {
                if (((core >> (i * 3 + 16)) & 7L) != 0) {
                    long turns = ((core >> (i * 3 + 16)) & 7L) >> 1;
                    long addition = (i % 2 == 0 ? -1 : 1);
                    turns = (turns + addition + 3) % 3;
                    core -= ((core >> (i * 3 + 16)) & 7L) << (i * 3 + 16);
                    core += ((turns << 1) | 1) << (i * 3 + 16);
                }
            }
        }

        if (faceToID[face / 2] < 4) {
            long cur = (core >> (faceToID[face / 2] * 2)) & 3L;
            core ^= cur << (faceToID[face / 2] * 2);
            cur = (cur + (reversed ? -1 : 1) + 4) % 4;
            core += cur << (faceToID[face / 2] * 2);
        }

        long edges = (core & (255L << 8)) >> 8, corners = core >> 16;

        if (reversed) {
            edges = (edges >> 2) + ((edges & 3L) << 6);
            corners = (corners >> 3) + ((corners & 7L) << 9);
        } else {
            edges = ((edges & 63L) << 2) + (edges >> 6);
            corners = ((corners & 511L) << 3) + (corners >> 9);
        }

        core = (edges << 8) + (corners << 16) + (core & 255L);

        return WrapCore(face, core, hash);
    }

    long GetCore(int face, long hash)
    {
        long core = 0;
        for (int i = 0; i < 4; ++i) {
            core += ((hash >> ((edgesInFace[face / 2, i] - 1) * 2 + 8)) & 3L) << (i * 2 + 8);
            core += ((hash >> ((cornersInFace[face / 2, i] - 13) * 3 + 32)) & 7L) << (i * 3 + 16);
        }
        return core + (255L & hash);
    }

    long WrapCore(int face, long core, long origin)
    {
        long hash = core & 255L;
        long mask = ((1L << 56) - 1) - 255L;
        for (int i = 0; i < 4; ++i) {
            hash += ((core >> (i * 2 + 8)) & 3L) << ((edgesInFace[face / 2, i] - 1) * 2 + 8);
            hash += ((core >> (i * 3 + 16)) & 7L) << ((cornersInFace[face / 2, i] - 13) * 3 + 32);
            mask -= 3L << ((edgesInFace[face / 2, i] - 1) * 2 + 8);
            mask -= 7L << ((cornersInFace[face / 2, i] - 13) * 3 + 32);
        }
        return hash + (mask & origin);
    }
}
