using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cube : MonoBehaviour
{
    public static Cube instance {  get; private set; }

    public enum FACE { L, l, R, r, U, u, D, d, F, f, B, b, M, E, S, X, Y, Z, NONE };

    private const int FaceCount = 18;
    private List<Vector3Int>[] blocksInFace;

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


    private void Awake()
    {
        if (instance == null) {

            instance = this;
            DontDestroyOnLoad(gameObject);

            gameObject.transform.position = Vector3.zero;

            DataInitiate();
            SetColor();
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Scramble();
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift) ||  Input.GetKeyDown(KeyCode.RightShift)) {
            direction = -1;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
            direction = 1;
        }

        else if (Input.GetKeyDown(KeyCode.Space)) {
            multiple = 2;
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            multiple = 1;
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            isDoubled = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
            isDoubled = false;
        }

        else if (Input.GetKeyDown(KeyCode.L)) {
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
            case FACE.M:
            case FACE.X:
            case FACE.r:
                pivot = Vector3.right * direction;
                pivotInt = Vector3Int.up * direction;
                break;
            case FACE.L:
            case FACE.l:
                pivot = Vector3.left * direction;
                pivotInt = Vector3Int.down * direction;
                break;
            case FACE.U:
            case FACE.E:
            case FACE.Y:
            case FACE.u:
                pivot = Vector3.up * direction;
                pivotInt = Vector3Int.forward * direction;
                break;
            case FACE.D:
            case FACE.d:
                pivot = Vector3.down * direction;
                pivotInt = Vector3Int.back * direction;
                break;
            case FACE.F:
            case FACE.S:
            case FACE.Z:
            case FACE.f:
                pivot = Vector3.back * direction;
                pivotInt = Vector3Int.right * direction;
                break;
            case FACE.B:
            case FACE.b:
                pivot = Vector3.forward * direction;
                pivotInt = Vector3Int.left * direction;
                break;
        }

        rotationQueue.Enqueue(new(face, degree, pivot, pivotInt));
    }

    private void Scramble()
    {
        int multiple, direction, face;
        for (int i = 0; i < scrambleMoves - 3; ++i) {
            multiple = Random.Range(1, 3);
            direction = Random.Range(0, 2) * 2 - 1;
            face = Random.Range(0, FaceCount);
            AddRotation((FACE)face, direction, multiple * 90);
        }
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
            blocks[pos.x, pos.y, pos.z].gameObject.transform.parent = gameObject.transform;
        }
    }

    private void UnTie(FACE face)
    {
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var pos = blocksInFace[(int)face][i];
            blocks[pos.x, pos.y, pos.z].gameObject.transform.parent = null;
        }
        gameObject.transform.rotation = Quaternion.identity;
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

    private void SetColor()
    {
        for (int i = 0; i < blocksInFace[(int)FACE.L].Count; ++i) {
            var pos = blocksInFace[(int)FACE.L][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Left").GetComponent<MeshRenderer>();
            mr.material.color = new(1, 0.5f, 0);
        }
        for (int i = 0; i < blocksInFace[(int)FACE.R].Count; ++i) {
            var pos = blocksInFace[(int)FACE.R][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Right").GetComponent<MeshRenderer>();
            mr.material.color = Color.red;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.U].Count; ++i) {
            var pos = blocksInFace[(int)FACE.U][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Up").GetComponent<MeshRenderer>();
            mr.material.color = Color.yellow;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.D].Count; ++i) {
            var pos = blocksInFace[(int)FACE.D][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Down").GetComponent<MeshRenderer>();
            mr.material.color = Color.white;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.F].Count; ++i) {
            var pos = blocksInFace[(int)FACE.F][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Front").GetComponent<MeshRenderer>();
            mr.material.color = Color.blue;
        }
        for (int i = 0; i < blocksInFace[(int)FACE.B].Count; ++i) {
            var pos = blocksInFace[(int)FACE.B][i];
            var block = blocks[pos.x, pos.y, pos.z];
            var mr = block.transform.Find("Back").GetComponent<MeshRenderer>();
            mr.material.color = Color.green;
        }
    }

    private void DataInitiate()
    {
        isRotating = false;
        direction = 1;
        isDoubled = false;
        multiple = 1;

        rotationQueue = new Queue<Rotation>();

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
