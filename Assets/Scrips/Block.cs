using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int[] pos = new int[3];

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
            }
            else if (pivotInt.y != 0) {
                pos[0] = -pos[0];
                pos[2] = -pos[2];
            }
            else if (pivotInt.z != 0) {
                pos[0] = -pos[0];
                pos[1] = -pos[1];
            }
        }
        else {
            if (pivotInt.x != 0) {
                if (pivotInt.x > 0) {
                    (pos[1], pos[2]) = (pos[2], -pos[1]);
                }
                else {
                    (pos[1], pos[2]) = (-pos[2], pos[1]);
                }
            }
            else if (pivotInt.y != 0) {
                if (pivotInt.y > 0) {
                    (pos[0], pos[2]) = (-pos[2], pos[0]);
                }
                else {
                    (pos[0], pos[2]) = (pos[2], -pos[0]);
                }
            }
            else if (pivotInt.z != 0) {
                if (pivotInt.z > 0) {
                    (pos[0], pos[1]) = (pos[1], -pos[0]);
                }
                else {
                    (pos[0], pos[1]) = (-pos[1], pos[0]);
                }
            }
        }
    }
}
