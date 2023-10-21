using UnityEngine;

public class Segment // отрезок
    {
    private Vector2 _start, _end;

    public Vector2 Start => _start;
    public Vector2 End => _end;

    public float XComponent => _end.x - _start.x;
    public float YComponent => _end.y - _start.y;

    public Segment(Vector2 start, Vector2 end)
        {
        _start = start;
        _end = end;
        }
    }