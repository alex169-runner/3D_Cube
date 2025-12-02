using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLLTableCreator : MonoBehaviour
{
    // https://www.bilibili.com/video/BV1jq4y1Q7fE/?spm_id_from=333.1387.search.video_card.click&vd_source=68bcb935ef3c4f70475cc30669ee4457

    string[] algorithms = new string[22]
    {
        // 1
        "R0,U',R0,U0,R0,U0,R0,U',R',U',R2",
        "R2,U0,R0,U0,R',U',R',U',R',U0,R'",
        "M2,U0,M2,U2,M2,U0,M2",
        "M',U0,M2,U0,M2,U0,M',U2,M2,U'",
        "X',R2,D2,R',U',R0,D2,R',U0,R'",
        // 6
        "X',R0,U',R0,D2,R',U0,R0,D2,R2",
        "X',R0,U',R',D0,R0,U0,R',D',R0,U0,R',D0,R0,U',R',D'",
        "R0,U0,R',U',R',F0,R2,U',R',U',R0,U0,R',F'",
        "R',U',F',R0,U0,R',U',R',F0,R2,U',R',U',R0,U0,R',U0,R0",
        "R',U0,R',d',R',F',R2,U',R',U0,R',F0,R0,F0",
        // 11
        "F0,R0,U',R',U',R0,U0,R',F',R0,U0,R',U',R',F0,R0,F'",
        "Z0,U',R0,D',R2,U0,R',U',R2,D0,U0,R'",
        "R0,U0,R',F',R0,U0,R',U',R',F0,R2,U',R',U'",
        "R',U2,R0,U2,R',F0,R0,U0,R',U',R',F',R2,U'",
        "R0,U',R',U',R0,U0,R0,D0,R',U',R0,D',R',U2,R',U'",
        // 16
        "R2,u',R0,U',R0,U0,R',u0,R2,Y0,R0,U',R'",
        "R0,U0,R',Y',R2,u',R0,U',R',U0,R',u0,R2",
        "R2,u0,R',U0,R',U',R0,u',R2,F',U0,F0",
        "R',U',R0,Y0,R2,u0,R',U0,R0,U',R0,u',R2",
        "R',U0,R0,U',R',F',U',F0,R0,U0,R',F0,R',F',R0,U',R0",
        // 21
        "R0,U0,R',U0,R0,U0,R',F',R0,U0,R',U',R',F0,R2,U',R',U2,R0,U',R'",
        "",
    };

    long[] hashCode = new long[22]
    {
        // 1
        0x1234_4132L,
        0x1234_2431L,
        0x1234_3142L,
        0x1234_4321L,
        0x2314_1234L,
        // 6
        0x3124_1234L,
        0x4321_1234L,
        0x1324_1432L,
        0x1324_3214L,
        0x1432_1324L,
        // 11
        0x1432_1243L,
        0x2134_2134L,
        0x1324_2134L,
        0x1243_2134L,
        0x1324_1243L,
        // 16
        0x4132_2431L,
        0x3241_4213L,
        0x3241_1423L,
        0x4132_3241L,
        0x1432_1432L,
        // 21
        0x3214_1432L,
        0x1234_1234L,
    };

    public PLLStateTable CreateTableAsset()
    {
        PLLStateTable table = ScriptableObject.CreateInstance<PLLStateTable>();

        int[] alphaToFace = new int[255];
        alphaToFace['R'] = (int)Cube.FACE.R;
        alphaToFace['r'] = (int)Cube.FACE.r;
        alphaToFace['L'] = (int)Cube.FACE.L;
        alphaToFace['l'] = (int)Cube.FACE.l;
        alphaToFace['U'] = (int)Cube.FACE.U;
        alphaToFace['u'] = (int)Cube.FACE.u;
        alphaToFace['D'] = (int)Cube.FACE.D;
        alphaToFace['d'] = (int)Cube.FACE.d;
        alphaToFace['F'] = (int)Cube.FACE.F;
        alphaToFace['f'] = (int)Cube.FACE.f;
        alphaToFace['M'] = (int)Cube.FACE.M;
        alphaToFace['X'] = (int)Cube.FACE.X;
        alphaToFace['Y'] = (int)Cube.FACE.Y;
        alphaToFace['Z'] = (int)Cube.FACE.Z;

        for (int i = 0; i < 22; ++i) {
            List<int> route = new();
            for (int j = 0; j < algorithms[i].Length; j += 3) {
                int code = (alphaToFace[algorithms[i][j]] << 2);
                if (algorithms[i][j + 1] == '2') code += 2;
                else if (algorithms[i][j + 1] == '\'') code += 1;
                route.Add(code);
            }
            table.stateSolutions.Add(new() { stateHash = hashCode[i], solutionMoves = route });
        }

        return table;
    }
}
