using UnityEditor.Build.Reporting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int orientation = 0;
    public int color = 0;
    public int[] pos = new int[3];
    public Vector3Int[] rot = new Vector3Int[6];

    public void Locate()
    {
        Cube.instance.blocks[pos[0] + 1, pos[1] + 1, pos[2] + 1] = this;
    }

    public void UpdateLocation(bool doubled, Vector3Int pivotInt)
    {
        if (doubled) {
            if (pivotInt.x != 0) {
                pos[1] = -pos[1];
                pos[2] = -pos[2];
                for (int i = 0; i < 6; ++i) {
                    if (rot[i] != null) {
                        rot[i].y = -rot[i].y;
                        rot[i].z = -rot[i].z;
                    }
                }
            }
            else if (pivotInt.y != 0) {
                pos[0] = -pos[0];
                pos[2] = -pos[2];
                for (int i = 0; i < 6; ++i) {
                    if (rot[i] != null) {
                        rot[i].x = -rot[i].x;
                        rot[i].z = -rot[i].z;
                    }
                }
            }
            else if (pivotInt.z != 0) {
                pos[0] = -pos[0];
                pos[1] = -pos[1];
                for (int i = 0; i < 6; ++i) {
                    if (rot[i] != null) {
                        rot[i].y = -rot[i].y;
                        rot[i].x = -rot[i].x;
                    }
                }
            }
        }
        else {
            orientation ^= 1;
            if (pivotInt.x != 0) {
                if (pivotInt.x > 0) {
                    (pos[1], pos[2]) = (pos[2], -pos[1]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].y, rot[i].z) = (rot[i].z, -rot[i].y);
                }
                else {
                    (pos[1], pos[2]) = (-pos[2], pos[1]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].y, rot[i].z) = (-rot[i].z, rot[i].y);
                }
            }
            else if (pivotInt.y != 0) {
                if (pivotInt.y > 0) {
                    (pos[0], pos[2]) = (-pos[2], pos[0]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].x, rot[i].z) = (-rot[i].z, rot[i].x);
                }
                else {
                    (pos[0], pos[2]) = (pos[2], -pos[0]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].x, rot[i].z) = (rot[i].z, -rot[i].x);
                }
            }
            else if (pivotInt.z != 0) {
                if (pivotInt.z > 0) {
                    (pos[0], pos[1]) = (pos[1], -pos[0]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].x, rot[i].y) = (rot[i].y, -rot[i].x);
                }
                else {
                    (pos[0], pos[1]) = (-pos[1], pos[0]);
                    for (int i = 0; i < 6; ++i)
                        if (rot[i] != null)
                            (rot[i].x, rot[i].y) = (-rot[i].y, rot[i].x);
                }
            }
        }
    }
}
