using System.Collections.Generic;
using UnityEngine;

public static class SortUtils
{
    public static int ByHierarchy(Transform a, Transform b)
    {
        if (a == b) 
            return 0;
        var pathRootA = PathToRoot(a);
        var pathRootB = PathToRoot(b);
        
        int n = Mathf.Min(pathRootA.Count, pathRootB.Count);
        for (int i = 0; i < n; i++)
        {
            if (pathRootA[i] != pathRootB[i])
                return pathRootA[i].GetSiblingIndex().CompareTo(pathRootB[i].GetSiblingIndex());
        }
        // Chelou si ça arrive
        return 0;
    }

    private static List<Transform> PathToRoot(Transform transform)
    {
        var list = new List<Transform>();
        while (transform != null) 
        { 
            list.Add(transform); 
            transform = transform.parent; 
        }
        list.Reverse();
        return list;
    }
}

class ComparerHierarchy : IComparer<Transform>
{
    public int Compare(Transform a, Transform b)
    {
        return (SortUtils.ByHierarchy(a, b));
    }
}
