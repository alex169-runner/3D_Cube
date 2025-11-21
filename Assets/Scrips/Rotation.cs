using UnityEngine;

public struct Rotation
{
    public Cube.FACE face;
    public int degree;
    public Vector3 pivot;
    public Vector3Int pivotInt;

    public Rotation(Cube.FACE face,
             int degree,
             Vector3 pivot,
             Vector3Int pivotInt
        )
    {
        this.face = face;
        this.degree = degree;
        this.pivot = pivot;
        this.pivotInt = pivotInt;
    }
}
