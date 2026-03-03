using UnityEngine;
using System;
public static class IdUtil
{
    public static string NewId(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }
}