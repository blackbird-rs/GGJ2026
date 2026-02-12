using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public static class Extensions
{
    public static string Capitalize(this string str)
    {
        if (str.IsNullOrWhitespace()) return str;
        if (str.Length == 1) return str.ToUpper();
        return char.ToUpper(str[0]) + str[1..];
    }

    public static bool IsNullOrWhitespace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static void ClearChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static void SetListener(this UnityEvent unityEvent, UnityAction action)
    {
        unityEvent.RemoveAllListeners();
        unityEvent.AddListener(action);
    }

    public static void Shuffle<T>(this IList<T> list, string seed)
    {
        int? seedInt = null;
        if (seed != null)
        {
            var md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(seed));
            seedInt = BitConverter.ToInt32(hashed, 0);
        }
        
        list.Shuffle(seedInt);
    }

    public static void Shuffle<T>(this IList<T> list, int? seedInt = null)
    {
        var rng = seedInt.HasValue ? new Random(seedInt.Value) : new Random();
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}