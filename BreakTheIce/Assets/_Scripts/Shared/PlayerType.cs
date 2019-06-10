using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerType
{
    public const int PolarBear = 1;
    public const int Penguin = 2;

    public static string ToType(int num)
    {
        if (num == 1) return "PolarBear";
        if (num == 2) return "Penguin";
        return "NULL";
    }
}
