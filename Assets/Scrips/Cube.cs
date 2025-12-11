using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class Cube : MonoBehaviour
{
    public static Cube instance {  get; private set; }

    public enum FACE { L, l, R, r, U, u, D, d, F, f, B, b, M, E, S, X, Y, Z, NONE };

    private const int FaceCount = 18;
    private List<Vector3Int>[] blocksInFace;

    public int[,,] ID;
    public Block[] blockOfColor;
    public Block[,,] blocks;
    public Block blockPrefab;

    private bool isRotating;
    private int multiple;
    private bool isDoubled;
    private int direction;

    public static Vector3 pivot;
    public static Vector3Int pivotInt;

    private float rotateDuration_Slow = 0.2f;
    private float rotateDuration_Medium = 0.17f;
    private float rotateDuration_Fast = 0.15f;

    private int slowBound = 2;
    private int mediumBound = 5;

    private int scrambleMoves = 17;

    private Queue<Rotation> rotationQueue;

    private CrossStateTable crossTable;
    private F2LStateTable F2LTable;
    private OLLStateTable OLLTable;
    private PLLStateTable PLLTable;

    private void Awake()
    {
        if (instance == null) {

            instance = this;
            DontDestroyOnLoad(gameObject);

            gameObject.transform.position = Vector3.zero;

            DataInitiate();
            SetColor();
            SetTable();
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Scramble();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            if (rotationQueue.Count == 0) {
                StartCoroutine(SolveCFOP());
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) ||  Input.GetKeyDown(KeyCode.RightShift)) {
            direction = -1;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
            direction = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            multiple = 2;
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            multiple = 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            isDoubled = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
            isDoubled = false;
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            AddRotation(isDoubled ? FACE.l : FACE.L, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            AddRotation(isDoubled ? FACE.r : FACE.R, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.U)) {
            AddRotation(isDoubled ? FACE.u : FACE.U, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            AddRotation(isDoubled ? FACE.d : FACE.D, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.F)) {
            AddRotation(isDoubled ? FACE.f : FACE.F, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.B)) {
            AddRotation(isDoubled ? FACE.b : FACE.B, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.M)) {
            AddRotation(FACE.M, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            AddRotation(FACE.E, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            AddRotation(FACE.S, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.X)) {
            AddRotation(FACE.X, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.Y)) {
            AddRotation(FACE.Y, direction, 90 * multiple);
        }
        else if (Input.GetKeyDown(KeyCode.Z)) {
            AddRotation(FACE.Z, direction, 90 * multiple);
        }

        if (!isRotating && rotationQueue.Count > 0) {
            StartCoroutine(StartRotation(rotationQueue.Count, rotationQueue.Dequeue()));
        }
    }

    private void AddRotation(FACE face, int direction, int degree)
    {
        Vector3 pivot = Vector3.zero;
        Vector3Int pivotInt = Vector3Int.zero;

        switch (face) {
            case FACE.R:
            case FACE.X:
            case FACE.r:
                pivot = Vector3.right;
                pivotInt = Vector3Int.up;
                break;
            case FACE.L:
            case FACE.l:
            case FACE.M:
                pivot = Vector3.left;
                pivotInt = Vector3Int.down;
                break;
            case FACE.U:
            case FACE.Y:
            case FACE.u:
                pivot = Vector3.up;
                pivotInt = Vector3Int.forward;
                break;
            case FACE.D:
            case FACE.d:
            case FACE.E:
                pivot = Vector3.down;
                pivotInt = Vector3Int.back;
                break;
            case FACE.F:
            case FACE.S:
            case FACE.Z:
            case FACE.f:
                pivot = Vector3.back;
                pivotInt = Vector3Int.right;
                break;
            case FACE.B:
            case FACE.b:
                pivot = Vector3.forward;
                pivotInt = Vector3Int.left;
                break;
        }

        pivot *= direction;
        pivotInt *= direction;

        rotationQueue.Enqueue(new(face, degree, pivot, pivotInt));
    }

    private void Scramble()
    {
        int multiple, direction, face;
        for (int i = 0; i < scrambleMoves; ++i) {
            multiple = UnityEngine.Random.Range(1, 3);
            direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
            face = UnityEngine.Random.Range(0, FaceCount - 3);
            AddRotation((FACE)face, direction, multiple * 90); 
        }
    }

    private IEnumerator SolveCFOP()
    {
        while (rotationQueue.Count > 0 || isRotating) {
            yield return null;
        }
        SolveCross();
        for (int i = 0; i < 4; ++i) {
            while (rotationQueue.Count > 0 || isRotating) {
                yield return null;
            }
            SolveF2L(i);
            if (i < 3) AddRotation(FACE.Y, 1, 90);
        }
        for (int i = 0; i < 4; ++i) {
            while (rotationQueue.Count > 0 || isRotating) {
                yield return null;
            }
            if (SolveOLL()) break;
            if (i < 3) AddRotation(FACE.U, 1, 90);
        }
        for (int i = 0; i < 4; ++i) {
            bool solved = false;
            for (int j = 0; j < 4; ++j) {
                while (rotationQueue.Count > 0 || isRotating) {
                    yield return null;
                }
                if (solved = SolvePLL()) break;
                AddRotation(FACE.U, 1, 90);
            }
            if (solved) break;
            AddRotation(FACE.Y, 1, 90);
        }
    }

    private void SolveCross()
    {
        Block cur;
        long state = 0;
        int color = 0, xor = 0, id = 0;
        int[] pos = new int[8]
        {
            2, 1,   1, 2,   0, 1,   1, 0
        };

        for (int i = 0, j = 0; i < 8; i += 2, j ^= 1) {
            color = blocks[1, 1, 0].color + blocks[pos[i], pos[i + 1], 1].color;
            cur = blockOfColor[color];
            id = ID[cur.pos[0] + 1, cur.pos[1] + 1, cur.pos[2] + 1];
            xor = cur.orientation ^ j ^ blocks[1, 1, 1].orientation;
            state += (long)((xor << 3) + (i / 2 + 1)) << ((id - 1) * 4);
        }
        if (crossTable.TryGetSolution(state, out List<int> solution)) {
            foreach (var code in solution) {
                AddRotation((FACE)(code >> 1), (code % 2 == 0 ? 1 : -1 ), 90);
            }
        }
    }

    private void SolveF2L(int phase)
    {
        int edgeColor = blocks[2, 1, 1].color + blocks[1, 2, 1].color;
        int cornerColor = blocks[1, 1, 0].color + edgeColor;

        Block edge = blockOfColor[edgeColor], corner = blockOfColor[cornerColor];

        long edgeOri = edge.orientation ^ blocks[1, 1, 1].orientation, cornerOri = 0;
        int id = (int)(Mathf.Log(blocks[1, 1, 0].color, 2) + 0.1);
        int cross = (corner.pos[0] * corner.rot[id].y - corner.pos[1] * corner.rot[id].x) * (corner.pos[2] == -1 ? -1 : 1);
        if (cross > 0) cornerOri = 1;
        else if (cross < 0) cornerOri = 2;

        long state = ((edgeOri << 1 | 1) << ((ID[edge.pos[0] + 1, edge.pos[1] + 1, edge.pos[2] + 1] - 1) * 2 + 8)) +
            ((cornerOri << 1 | 1) << ((ID[corner.pos[0] + 1, corner.pos[1] + 1, corner.pos[2] + 1] - 13) * 3 + 32));

        if (F2LTable.TryGetSolution(phase, state, out List<int> solution)) {
            foreach (var code in solution) {
                AddRotation((FACE)(code >> 1), code % 2 == 0 ? 1 : -1, 90);
            }
        }
    }

    private bool SolveOLL()
    {
        long state = 0L;
        int id = (int)(Mathf.Log(blocks[1, 1, 2].color, 2) + 0.1);
        for (int i = 2; i >= 0; --i) {
            for (int j = 0; j < 3; ++j) {
                var block = blocks[i, j, 2];
                var rot = block.rot[id];
                state = (state << 4) + Mathf.Abs(rot.x) * (-rot.x + 2) + Mathf.Abs(rot.y) * (-rot.y + 3) + rot.z * 5;
            }
        }

        if (OLLTable.TryGetSolution(state, out List<int> solution)) {
            foreach (var s in solution) {
                AddRotation((FACE)(s >> 2), s % 2 == 0 ? 1 : -1, s % 4 > 1 ? 180 : 90);
            }
            return true;
        }

        return false;
    }

    private bool SolvePLL()
    {
        int[,] order = new int[4, 2]
        {
            { 2, 1 }, { 1, 2 }, { 0, 1 }, { 1, 0 }
        };

        long state = 0;
        for (int i = 0, j = 3; i < 4; ++i, j = i - 1) {
            var corner = blockOfColor[blocks[1, 1, 2].color + blocks[order[i, 0], order[i, 1], 1].color + blocks[order[j, 0], order[j, 1], 1].color];
            state = (state << 4) + ID[corner.pos[0] + 1, corner.pos[1] + 1, corner.pos[2] + 1] - 16;
        }
        for (int i = 0; i < 4; ++i) {
            var edge = blockOfColor[blocks[1, 1, 2].color + blocks[order[i, 0], order[i, 1], 1].color];
            state = (state << 4) + ID[edge.pos[0] + 1, edge.pos[1] + 1, edge.pos[2] + 1] - 8;
        }

        if (PLLTable.TryGetSolution(state, out List<int> solution)) {
            foreach (var s in solution) {
                AddRotation((FACE)(s >> 2), s % 2 == 0 ? 1 : -1, s % 4 > 1 ? 180 : 90);
            }
            return true;
        }

        return false;
    }

    private IEnumerator StartRotation(int count, Rotation rotation)
    {
        isRotating = true;
        Tie(rotation.face);

        float percentage = 0.0f;
        int degree = rotation.degree;
        float rotateDuration = count <= slowBound ? rotateDuration_Slow : count <= mediumBound ? rotateDuration_Medium : rotateDuration_Fast;

        Quaternion startRotation = gameObject.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(degree - 0.1f, rotation.pivot);
        Quaternion finalRotation = startRotation * Quaternion.AngleAxis(degree, rotation.pivot);

        while (percentage < 1.0f) {
            percentage += Time.deltaTime / rotateDuration;
            gameObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, percentage);
            yield return null;
        }
        gameObject.transform.rotation = finalRotation;

        ReLocate(rotation.face, degree == 180, rotation.pivotInt);
        UnTie(rotation.face);

        isRotating = false;
    }

    private void Tie(FACE face)
    {
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var pos = blocksInFace[(int)face][i];
            blocks[pos.x, pos.y, pos.z].transform.parent = transform;
        }
    }

    private void UnTie(FACE face)
    {
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var pos = blocksInFace[(int)face][i];
            blocks[pos.x, pos.y, pos.z].transform.parent = null;
        }
        transform.rotation = Quaternion.identity;
    }

    private void ReLocate(FACE face, bool doubled, Vector3Int pivotInt)
    {
        Block cur;
        List<Block> tmp = new();
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var pos = blocksInFace[(int)face][i];
            cur = blocks[pos.x, pos.y, pos.z];
            cur.UpdateLocation(doubled, pivotInt);
            tmp.Add(cur);
        }

        for (int i = 0; i < tmp.Count; ++i) {
            tmp[i].Locate();
        }
    }

    private void SetTable()
    {
        crossTable = Resources.Load<CrossStateTable>("StateTables/CrossStateTable");
        if (crossTable == null) {
            Debug.Log("crossTable not set");
        }

        F2LTable = Resources.Load<F2LStateTable>("StateTables/F2LStateTables");
        if (F2LTable == null) {
            Debug.Log("F2LTable not set");
        }

        OLLTable = Resources.Load<OLLStateTable>("StateTables/OLLStateTable");
        if (OLLTable == null) {
            Debug.Log("OLLTable not set");
        }

        PLLTable = Resources.Load<PLLStateTable>("StateTables/PLLStateTable");
        if (PLLTable == null) {
            Debug.Log("PLLTable not set");
        }
    }

    private void SetColor()
    {
        for (int i = 0; i < blocksInFace[(int)FACE.L].Count; ++i) {
            var pos = blocksInFace[(int)FACE.L][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 4;
            block.rot[4] = new(0, -1, 0);
            var mr = block.transform.Find("Left").GetComponent<MeshRenderer>();
            mr.material.color = new(1, 0.5f, 0);
        }
        for (int i = 0; i < blocksInFace[(int)FACE.R].Count; ++i) {
            var pos = blocksInFace[(int)FACE.R][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 2;
            block.rot[2] = new(0, 1, 0);
            var mr = block.transform.Find("Right").GetComponent<MeshRenderer>();
            mr.material.color = Color.red;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.U].Count; ++i) {
            var pos = blocksInFace[(int)FACE.U][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 5;
            block.rot[5] = new(0, 0, 1);
            var mr = block.transform.Find("Up").GetComponent<MeshRenderer>();
            mr.material.color = Color.yellow;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.D].Count; ++i) {
            var pos = blocksInFace[(int)FACE.D][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 0;
            block.rot[0] = new(0, 0, -1);
            var mr = block.transform.Find("Down").GetComponent<MeshRenderer>();
            mr.material.color = Color.white;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.F].Count; ++i) {
            var pos = blocksInFace[(int)FACE.F][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 1;
            block.rot[1] = new(1, 0, 0);
            var mr = block.transform.Find("Front").GetComponent<MeshRenderer>();
            mr.material.color = Color.blue;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.B].Count; ++i) {
            var pos = blocksInFace[(int)FACE.B][i];
            var block = blocks[pos.x, pos.y, pos.z];
            block.color |= 1 << 3;
            block.rot[3] = new(-1, 0, 0);
            var mr = block.transform.Find("Back").GetComponent<MeshRenderer>();
            mr.material.color = Color.green;
        }

        blockOfColor = new Block[1 << 6];
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                for (int k = 0; k < 3; ++k)
                    blockOfColor[blocks[i, j, k].color] = blocks[i, j, k];
    }

    private void DataInitiate()
    {
        isRotating = false;
        direction = 1;
        isDoubled = false;
        multiple = 1;

        rotationQueue = new Queue<Rotation>();

        int node = 0;
        int[] pos = new int[3 * 20]
        {
            2, 1, 0,
            1, 2, 0,
            0, 1, 0,
            1, 0, 0,
            2, 0, 1,
            2, 2, 1,
            0, 2, 1,
            0, 0, 1,
            2, 1, 2,
            1, 2, 2,
            0, 1, 2,
            1, 0, 2,

            2, 0, 0,
            2, 2, 0,
            0, 2, 0,
            0, 0, 0,
            2, 0, 2,
            2, 2, 2,
            0, 2, 2,
            0, 0, 2
        };
        ID = new int[3, 3, 3];
        for (int i = 0; i < 3 * 20; i += 3) {
            ID[pos[i], pos[i + 1], pos[i + 2]] = ++node;
        }

        blocks = new Block[3, 3, 3];
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                for (int k = 0; k < 3; ++k) {
                    blocks[i, j, k] = Instantiate(blockPrefab);
                    blocks[i, j, k].gameObject.transform.position = new(j - 1, k - 1, 1 - i);
                    blocks[i, j, k].pos[0] = i - 1;
                    blocks[i, j, k].pos[1] = j - 1;
                    blocks[i, j, k].pos[2] = k - 1;
                }
            }
        }

        blocksInFace = new List<Vector3Int>[FaceCount];
        for (int i = 0; i < FaceCount; ++i) {
            blocksInFace[i] = new();
        }

        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                blocksInFace[(int)FACE.L].Add(new(i, 0, j));
                blocksInFace[(int)FACE.M].Add(new(i, 1, j));
                blocksInFace[(int)FACE.R].Add(new(i, 2, j));
                
                blocksInFace[(int)FACE.D].Add(new(i, j, 0));
                blocksInFace[(int)FACE.E].Add(new(i, j, 1));
                blocksInFace[(int)FACE.U].Add(new(i, j, 2));

                blocksInFace[(int)FACE.B].Add(new(0, i, j));
                blocksInFace[(int)FACE.S].Add(new(1, i, j));
                blocksInFace[(int)FACE.F].Add(new(2, i, j));

                blocksInFace[(int)FACE.X].Add(new(0, i, j));
                blocksInFace[(int)FACE.X].Add(new(1, i, j));
                blocksInFace[(int)FACE.X].Add(new(2, i, j));

                blocksInFace[(int)FACE.Y].Add(new(0, i, j));
                blocksInFace[(int)FACE.Y].Add(new(1, i, j));
                blocksInFace[(int)FACE.Y].Add(new(2, i, j));

                blocksInFace[(int)FACE.Z].Add(new(0, i, j));
                blocksInFace[(int)FACE.Z].Add(new(1, i, j));
                blocksInFace[(int)FACE.Z].Add(new(2, i, j));
            }
        }

        blocksInFace[(int)FACE.l].AddRange(blocksInFace[(int)FACE.L]);
        blocksInFace[(int)FACE.l].AddRange(blocksInFace[(int)FACE.M]);

        blocksInFace[(int)FACE.r].AddRange(blocksInFace[(int)FACE.R]);
        blocksInFace[(int)FACE.r].AddRange(blocksInFace[(int)FACE.M]);

        blocksInFace[(int)FACE.u].AddRange(blocksInFace[(int)FACE.U]);
        blocksInFace[(int)FACE.u].AddRange(blocksInFace[(int)FACE.E]);

        blocksInFace[(int)FACE.b].AddRange(blocksInFace[(int)FACE.B]);
        blocksInFace[(int)FACE.b].AddRange(blocksInFace[(int)FACE.S]);

        blocksInFace[(int)FACE.f].AddRange(blocksInFace[(int)FACE.F]);
        blocksInFace[(int)FACE.f].AddRange(blocksInFace[(int)FACE.S]);
    }
}
