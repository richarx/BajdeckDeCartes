using UnityEngine;


[ExecuteAlways]
public class GridLayout2D : MonoBehaviour
{
    public int Height { get; private set; }
    [SerializeField, Min(1)] private int _width = 5;
    [SerializeField] private float _spacingX = 1f;
    [SerializeField] private float _spacingY = 1f;

    public int Width { get { return (_width); } }

    public int Count { get { return (transform.childCount); } }


    // Recalcule la grille à chaque changement utile en Éditeur et en Play
    private void OnValidate()
    {
        _width = Mathf.Max(1, _width);
        Rebuild();
    }

    private void OnTransformChildrenChanged() => Rebuild();

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying) Rebuild();
    }
#endif
    
    public void Rebuild()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            child.localPosition = GetLocalPositionForIndex(i);
            i++;
        }

        var count = transform.childCount;
        Height = Mathf.FloorToInt((count - 1) / _width) + 1;
    }

    public Vector3 GetLocalPositionForIndex(int index)
    {
        int x = index % _width;
        int y = index / _width;
        return new Vector3(x * _spacingX,  -y * _spacingY, 0f);
    }
}