using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLLTableCreator : MonoBehaviour
{
    //https://www.bilibili.com/video/BV1Sq4y177d1/?spm_id_from=333.337.search-card.all.click

    string[] algorithms = new string[58]
    {
        // 1
        "R0,U2,R',U',R0,U',R',",
        "R0,U0,R',U0,R0,U2,R'",
        "R0,U2,R',U',R0,U0,R',U',R0,U',R'",
        "R0,U2,R2,U',R2,U',R2,U2,R0",
        "r0,U0,R',U',r',F0,R0,F'",
        // 6
        "R2,D',R0,U2,R',D0,R0,U2,R0",
        "F',r0,U0,R',U',r',F0,R0",
        "R0,U2,R2,F0,R0,F',U2,R',F0,R0,F'",
        "F0,R0,U0,R',U',F',f0,R0,U0,R',U',f'",
        "f0,R0,U0,R',U',f',U',F0,R0,U0,R',U',F'",
        // 11
        "f0,R0,U0,R',U',f',U0,F0,R0,U0,R',U',F'",
        "R0,U0,R',U0,R',F0,R0,F',U2,R',F0,R0,F'",
        "r0,U0,R',U0,R0,U2,r',r',U',R0,U',R',U2,r0",
        "r',R0,U0,R0,U0,R',U',r0,R2,F0,R0,F'",
        "r0,U0,R',U',M2,U0,R0,U',R',U',M'",
        // 16
        "f0,R0,U0,R',U',R0,U0,R',U',f'",
        "R',F',U',F0,U',R0,U0,R',U0,R0",
        "r0,U0,r',U0,R0,U',R',U0,R0,U',R',r0,U',r'",
        "R',F0,R0,U0,R0,U',R2,F',R2,U',R',U0,R0,U0,R'",
        "r0,U',r',U',r0,U0,r',F',U0,F0",
        // 21
        "r',U',r0,R',U',R0,U0,r',U0,r0",
        "R',F0,R0,U0,R',F',R0,F0,U',F'",
        "r0,U0,r',R0,U0,R',U',r0,U',r'",
        "R0,U0,R',U',R',F0,R0,F'",
        "F0,R0,U0,R',U',F'",
        // 26
        "R0,U0,R',U',M',U0,R0,U',r'",
        "r0,U0,R',U',r',R0,U0,R0,U',R'",
        "R0,U0,R',F',U',F0,U0,R0,U2,R'",
        "R',F0,R0,U0,R',U',F',U0,R0",
        "R0,U0,R2,U',R',F0,R0,U0,R0,U',F'",
        // 31
        "R',U',R',F0,R0,F',U0,R0",
        "R',U',F0,U0,R0,U',R',F',R0",
        "R0,U0,B',U',R',U0,R0,B0,R'",
        "R',U',F',U0,F0,R0",
        "f0,R0,U0,R',U',f'",
        // 36
        "F0,R0,U',R',U',R0,U0,R',F'",
        "R0,U2,R2,F0,R0,F',R0,U2,R'",
        "r',U2,R0,U0,R',U0,r0",
        "r0,U2,R',U',R0,U',r'",
        "r',U',R0,U',R',U2,r0",
        // 41
        "r0,U0,R',U0,R0,U2,r'",
        "r',R2,U0,R',U0,R0,U2,R',U0,M'",
        "F0,R0,U0,R',U',F',U0,F0,R0,U0,R',U',F'",
        "R0,U0,R',U',R',F0,R2,U0,R',U',F'",
        "R0,U0,R',U0,R',F0,R0,F',R0,U2,R'",
        // 46
        "r0,U2,R',U',R0,U0,R',U',R0,U',r'",
        "F',L',U',L0,U0,L',U',L0,U0,F0",
        "r',U2,R0,U0,R',U',R0,U0,R',U0,r0",
        "F0,R0,U0,R',U',R0,U0,R',U',F'",
        "R',F0,R2,B',R2,F',R2,B0,R'",
        // 51
        "R0,B',R2,F0,R2,B0,R2,F',R0",
        "f0,R0,U0,R2,U',R',U0,R2,U',R',f'",
        "R',F0,R0,F',R0,U2,R',U',F',U',F0",
        "R',U',R0,U',R',U2,R0,F0,R0,U0,R',U',F'",
        "R0,U0,R',U0,R0,U2,R',F0,R0,U0,R',U',F'",
        // 56
        "R0,U0,R',U0,R0,U',R',U',R',F0,R0,F'",
        "R',U',R0,U',R',U0,R0,U0,l0,U',R',U0",
        "",
    };

    long[] hashCodes = new long[58]
    {
        // 1
        0x152_555_455L,
        0x551_555_352L,
        0x151_555_353L,
        0x451_555_453L,
        0x155_555_355L,
        // 6
        0x555_555_353L,
        0x551_555_455L,
        0x412_452_432L,
        0x411_452_433L,
        0x415_452_332L,
        // 11
        0x112_452_435L,
        0x415_452_533L,
        0x111_452_535L,
        0x412_452_535L,
        0x515_452_535L,
        // 16
        0x411_555_433L,
        0x451_452_453L,
        0x412_555_432L,
        0x111_555_333L,
        0x511_555_332L,
        // 21
        0x415_555_332L,
        0x115_555_433L,
        0x112_555_435L,
        0x115_555_335L,
        0x415_555_435L,
        // 26
        0x515_555_535L,
        0x515_552_555L,
        0x511_555_435L,
        0x415_555_533L,
        0x515_555_432L,
        // 31
        0x552_452_552L,
        0x115_455_355L,
        0x155_455_335L,
        0x111_552_555L,
        0x455_455_435L,
        // 36
        0x115_552_552L,
        0x155_455_532L,
        0x455_455_332L,
        0x112_455_455L,
        0x152_552_533L,
        // 41
        0x511_552_352L,
        0x551_455_332L,
        0x152_455_435L,
        0x115_552_453L,
        0x451_552_335L,
        // 46
        0x111_552_353L,
        0x112_455_352L,
        0x151_552_333L,
        0x411_552_453L,
        0x112_552_352L,
        // 51
        0x152_552_332L,
        0x452_455_535L,
        0x515_455_452L,
        0x151_552_535L,
        0x515_552_353L,
        // 56
        0x512_552_355L,
        0x155_552_532L,
        0x555_555_555L,
    };

    public OLLStateTable CreateTableAsset()
    {
        OLLStateTable table = ScriptableObject.CreateInstance<OLLStateTable>();

        int[] alphaToFace = new int[255];
        alphaToFace['R'] = (int)Cube.FACE.R;
        alphaToFace['r'] = (int)Cube.FACE.r;
        alphaToFace['L'] = (int)Cube.FACE.L;
        alphaToFace['l'] = (int)Cube.FACE.l;
        alphaToFace['U'] = (int)Cube.FACE.U;
        alphaToFace['D'] = (int)Cube.FACE.D;
        alphaToFace['F'] = (int)Cube.FACE.F;
        alphaToFace['f'] = (int)Cube.FACE.f;
        alphaToFace['B'] = (int)Cube.FACE.B;
        alphaToFace['M'] = (int)Cube.FACE.M;

        for (int i = 0; i < 58; ++i) {
            List<int> route = new();
            for (int j = 0; j < algorithms[i].Length; j += 3) {
                int code = (alphaToFace[algorithms[i][j]] << 2);
                if (algorithms[i][j + 1] == '2') code += 2;
                else if (algorithms[i][j + 1] == '\'') code += 1;
                route.Add(code);
            }
            table.stateSolutions.Add(new() { stateHash = hashCodes[i], solutionMoves = route });
        }

        return table;
    }
}
