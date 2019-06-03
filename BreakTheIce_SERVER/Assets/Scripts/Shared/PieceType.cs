using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceType
{
    public const int Cross = 0;
    public const int L = 1;
    public const int Square = 2;
    public const int Z = 3;

    public static string ToType(int num)
    {
        if (num == 0) return "Cross";
        if (num == 1) return "L";
        if (num == 2) return "Square";
        if (num == 3) return "Z";
        return "NULL";
    }
}
