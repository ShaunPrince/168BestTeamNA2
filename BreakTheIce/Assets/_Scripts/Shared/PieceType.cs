using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceType
{
    public enum PType { Plus, Square, Line, L, Slash }
    public const int Cross = 0;
    public const int L = 1;
    public const int Square = 2;
    public const int Z = 3;

    public static string ToType(PType num)
    {
        if (num == PType.Plus)
        {
            return "Plus";
        }
        else if (num == PType.L)
        {
            return "L";
        }
        else if(num == PType.Square)
        {
            return "Square";
        }
        else if(num == PType.Slash)
        {
            return "Slash";
        }
        else if(num == PType.Line)
        {
            return "Line";
        }
        else
        {
            return "NULL";
        }

    }
}
