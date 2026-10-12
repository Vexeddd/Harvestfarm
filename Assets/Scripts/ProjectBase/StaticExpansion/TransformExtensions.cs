using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindDirect(this Transform parent, string targetName)
    {
        foreach (Transform trs in parent)
        {
            if (trs.name == targetName)
            {
                return trs;
            }
        }

        return null;
    }

    public static Transform FindRecursive(this Transform parent, string targetName)
    {
        foreach (Transform trs in parent)
        {
            if (trs.name == targetName)
            {
                return trs;
            }

            Transform found = trs.FindRecursive(targetName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
