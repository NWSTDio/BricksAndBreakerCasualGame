using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    public enum Mirror { NONE, UP, RIGHT, DOWN, LEFT }
    public static Action<float> OnBallDestroy;

    [SerializeField] private float _speed = 10f;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private bool _showPath = false;

    private bool _sleep = true;

    private readonly float _lineLength = 1f;
    private Vector3 _direction;

    [SerializeField] private List<Vector3> _path;

    private void Start() {
        _path = new();
        }

    private void FixedUpdate() {

        }

    private void Update() {
        //if (_sleep)
        //    return;

        //Vector3 pos = transform.position;// текущая позиция
        //Utils.DrawCross(pos, .2f, Color.gray);
        //pos += _speed * Time.deltaTime * _direction;// предварительно двигаем вперед
        //Utils.DrawCross(pos, .2f, Color.magenta);

        //CheckCollision(transform.position, pos, out pos);

        ////// Debug.Break();

        ////for (int i = 0; i < 20; i++) // попытки проверки колизии для одного шара
        ////    {
        ////    if (CheckCollision(transform.position, pos, out pos).Not()) // если колизии нету
        ////        break;
        ////    Debug.Log($"попытка номер {i} проверки столкновения для обьекта {name}");
        ////    }

        //_path.Add(pos);

        //transform.position = pos;// двигаем в указанную точку
        //transform.right = _direction;
        }

    private bool CheckCollision(Vector3 start, Vector3 current, out Vector3 pos) {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(current, .05f, _mask);

        if (colliders.Length > 0) {
            if (colliders.Length == 1) {
                Box box = colliders[0].GetComponent<Box>();
                box.TakeDamage();
                if (box.IsLive.Not()) // если обьект уже не жив
                {
                    pos = current;
                    return false;
                    }
                Debug.Log("ONE");
                return CheckMirror(start, current, box, out pos);
                }
            foreach (Collider2D collider in colliders) {
                Box box = collider.GetComponent<Box>();
                box.TakeDamage();
                Debug.Log("BOX: " + box.name);
                }
            }



        //// точечная коллизия
        //Collider2D collider = Physics2D.OverlapPoint(current, _mask);
        //if (collider != null)
        //    {
        //    Box box = collider.GetComponent<Box>();

        //    box.TakeDamage();

        //    if (box.IsLive.Not()) // если обьект уже не жив
        //        {
        //        pos = current;
        //        return false;
        //        }
        //    Debug.Log("COLLISION!");
        //    Debug.Break();
        //    return CheckMirror(start, current, box, out pos);
        //    }

        // пересечения линий
        //Collider2D[] colliders = Physics2D.OverlapArea

        // проверка столкновений с границами по
        if (current.x < Data.LEFT_BORDER) // левой стороне
            return ApplyMirror(current, Data.LEFT_BORDER, true, out pos);

        if (current.x > Data.RIGHT_BORDER) // правой стороне
            return ApplyMirror(current, Data.RIGHT_BORDER, true, out pos);

        if (current.y > Data.UP_BORDER) // потолку
            return ApplyMirror(current, Data.UP_BORDER, false, out pos);

        if (current.y < Data.DOWN_BORDER) // если пол
        {
            OnBallDestroy?.Invoke(current.x);// сообщим что удаляем шар
            _sleep = true;
            if (Data.DEBUG_MODE.Not() || (Data.DEBUG_MODE && Data.HIDE_BALL))
                gameObject.SetActive(false);// скрываем шар
            }

        pos = current;
        return false;
        }

    public void Move(Vector3 startMovePosition, Vector3 direction) {
        transform.position = startMovePosition;
        _direction = direction;
        _sleep = false;
        _path.Clear();
        transform.right = _direction;
        }

    public bool ApplyMirror(Vector3 line1B, Box box, out Vector3 pos, Mirror mirror) {
        Vector3 neighbor = box.Position;
        if (mirror == Mirror.LEFT)
            neighbor.x -= (box.Width / 2) + Data.BLOCK_WIDTH / 2;
        else if (mirror == Mirror.RIGHT)
            neighbor.x += (box.Width / 2) + Data.BLOCK_WIDTH / 2;
        else if (mirror == Mirror.UP)
            neighbor.y += (box.Height / 2) + Data.BLOCK_HEIGHT / 2;
        else if (mirror == Mirror.DOWN)
            neighbor.y -= (box.Height / 2) + Data.BLOCK_HEIGHT / 2;

        Collider2D collider = Physics2D.OverlapPoint(neighbor, _mask);
        if (collider == null) {
            if (mirror == Mirror.LEFT)
                return ApplyMirror(line1B, box.MinX, true, out pos, CheckCollision(mirror, box));
            else if (mirror == Mirror.RIGHT)
                return ApplyMirror(line1B, box.MaxX, true, out pos, CheckCollision(mirror, box));
            else if (mirror == Mirror.UP)
                return ApplyMirror(line1B, box.MaxY, true, out pos, CheckCollision(mirror, box));
            else if (mirror == Mirror.DOWN)
                return ApplyMirror(line1B, box.MinY, true, out pos, CheckCollision(mirror, box));
            }

        Box nBox = collider.GetComponent<Box>();

        List<Vector3> points = CheckLineCollision(nBox, transform.position, line1B);

        if (points.Count == 0) {
            pos = line1B;
            return false;
            }

        Vector3 point;
        if (points.Count == 1)
            point = points[1];
        else
            point = GetNearestPoint(transform.position, points);

        float distance = Vector3.Distance(line1B, point);



        if (points.Count > 1)
            _direction *= -1;
        else {
            // инвертируем направление движения, взависимости от поверхности столкновения
            if (mirror == Mirror.LEFT || mirror == Mirror.RIGHT)
                _direction.x *= -1;
            else
                _direction.y *= -1;
            }

        pos = point + (distance * _direction);

        Collider2D coll = Physics2D.OverlapPoint(pos, _mask);

        if (IsOutOfBorder(pos) || coll != null) // новая позиция за границами поля или мы в обьекте
        {
            Debug.Log("СЮДА НЕЛЬЗЯ!");
            transform.position = point;
            return true;
            }

        return false;
        }

    public bool ApplyMirror(Vector3 line1B, float limit, bool horizontal, out Vector3 pos, bool back = false) {
        Vector2 line1A = transform.position;

        // позиции граници с которой пересекаемся
        var line2A = (horizontal) ? new Vector2(limit, line1A.y - 1) : new Vector2(line1A.x - 1, limit);
        var line2B = (horizontal) ? new Vector2(limit, line1B.y + 1) : new Vector2(line1B.x + 1, limit);

        //Debug.DrawLine(line1A, line1B, Color.yellow);
        //Debug.DrawLine(line2A, line2B, Color.yellow);

        Vector3 pos2 = Utils.GetIntersectionPosition(line1A, line1B, line2A, line2B);// получим координаты пересечения двух точек

        Debug.Log("POS2" + pos2);

        //Debug.DrawLine(pos2 + Vector3.left, pos2 + Vector3.right, Color.yellow);
        //Debug.DrawLine(pos2 + Vector3.down, pos2 + Vector3.up, Color.yellow);

        float distance = Vector3.Distance(line1B, pos2);

        if (Data.DEBUG_MODE) {
            Debug.DrawLine(pos2, pos2 + (_lineLength * distance * _direction), Color.red);// направление куда двигаемся от точки пересечения
            Debug.DrawLine(pos2, pos2 - (_lineLength * distance * _direction), Color.green);// направление куда двигаемся до точки пересечения
            }

        if (back)
            _direction *= -1;
        else {
            // инвертируем направление движения, взависимости от поверхности столкновения
            if (horizontal)
                _direction.x *= -1;
            else
                _direction.y *= -1;
            }

        if (Data.DEBUG_MODE) // направление куда двигатся
            Debug.DrawLine(pos2, pos2 + (_lineLength * distance * _direction), Color.blue);

        pos = pos2 + (distance * _direction);

        if (back)
            return false;

        //Debug.DrawLine(pos + Vector3.left, pos + Vector3.right, Color.magenta);
        //Debug.DrawLine(pos + Vector3.down, pos + Vector3.up, Color.magenta);
        Debug.Log("POS" + pos);

        Collider2D collider = Physics2D.OverlapPoint(pos, _mask);

        if (IsOutOfBorder(pos) || collider != null) // новая позиция за границами поля или мы в обьекте
        {
            Debug.Log("СЮДА НЕЛЬЗЯ!");
            transform.position = pos2;
            return true;
            }

        return false;// передвинем на то расстояние которое за барьером
        }

    private bool IsOutOfBorder(Vector3 pos) => (pos.x < Data.LEFT_BORDER || pos.x > Data.RIGHT_BORDER || pos.y > Data.UP_BORDER);

    private bool CheckMirror(Vector3 start, Vector3 current, Box box, out Vector3 pos) {
        bool IsRight = _direction.x > 0;
        bool IsLeft = _direction.x < 0;
        bool IsUp = _direction.y > 0;
        bool IsDown = _direction.y < 0;
        bool IsVerticalStop = _direction.y == 0;

        pos = current;

        if (start.x < box.MinX) // c лева
        {
            Debug.Log("LEFT BORDER BOX");
            if ((IsRight && IsUp) || (IsRight && IsDown) || (IsRight && IsVerticalStop))
                return ApplyMirror(pos, box, out pos, Mirror.LEFT);
            }
        else if (start.x > box.MaxX) {
            Debug.Log("RIGHT BORDER BOX");
            if ((IsLeft && IsUp) || (IsLeft && IsDown) || (IsLeft && IsVerticalStop)) {
                Debug.Log("YES");
                return ApplyMirror(pos, box, out pos, Mirror.RIGHT);
                }
            }
        else {
            if (start.y < box.MinY) {
                Debug.Log("DOWN BORDER BOX");
                return ApplyMirror(pos, box, out pos, Mirror.DOWN);
                }
            else {
                Debug.Log("UP BORDER BOX");
                return ApplyMirror(pos, box, out pos, Mirror.UP);
                }
            }

        Debug.Log("NOT" + transform.position.ToString());

        return false;
        }
    private bool CheckCollision(Mirror mirror, Box box) {
        Vector3 pos = box.Position;

        switch (mirror) {
            case Mirror.UP:
            pos.y += (box.Height / 2) + Data.BLOCK_HEIGHT / 2;
            break;
            case Mirror.RIGHT:
            pos.x += (box.Width / 2) + Data.BLOCK_WIDTH / 2;
            break;
            case Mirror.DOWN:
            pos.y -= (box.Height / 2) + Data.BLOCK_HEIGHT / 2;
            break;
            case Mirror.LEFT:
            pos.x -= (box.Width / 2) + Data.BLOCK_WIDTH / 2;
            break;
            case Mirror.NONE:
            return false;
            }

        if (Physics2D.OverlapBox(pos, new Vector2(.4f, .4f), 0, _mask) != null) {
            Debug.Log("COLLISION!!!!!");
            return true;
            }

        Debug.Log("NO COLLISION!!!!!");
        return false;
        }
    private void OnDrawGizmos() {
        if (Data.DEBUG_MODE.Not() && Data.RUNNING.Not())
            return;
        if (_path.Count <= 1)
            return;
        Gizmos.color = Color.cyan;
        Vector3 start = _path[0];
        foreach (var p in _path) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p, .05f);
            Gizmos.color = Color.cyan;
            if (start == p)
                continue;
            Gizmos.DrawLine(start, p);
            start = p;
            }
        }
    private List<Vector3> CheckLineCollision(Box box, Vector3 line1A, Vector3 line1B) {
        var points = new List<Vector3>();

        float limit = box.MinX;
        var line2A = new Vector2(limit, line1A.y - 1);
        var line2B = new Vector2(limit, line1B.y + 1);
        Debug.DrawLine(line2A, line2B, Color.cyan);
        points.Add(Utils.GetIntersectionPosition(line1A, line1B, line2A, line2B));

        limit = box.MaxX;
        line2A = new Vector2(limit, line1A.y - 1);
        line2B = new Vector2(limit, line1B.y + 1);

        Debug.DrawLine(line2A, line2B, Color.cyan);
        points.Add(Utils.GetIntersectionPosition(line1A, line1B, line2A, line2B));

        limit = box.MinY;
        line2A = new Vector2(line1A.x - 1, limit);
        line2B = new Vector2(line1B.x + 1, limit);

        Debug.DrawLine(line2A, line2B, Color.cyan);
        points.Add(Utils.GetIntersectionPosition(line1A, line1B, line2A, line2B));

        limit = box.MaxY;
        line2A = new Vector2(line1A.x - 1, limit);
        line2B = new Vector2(line1B.x + 1, limit);

        Debug.DrawLine(line2A, line2B, Color.cyan);
        points.Add(Utils.GetIntersectionPosition(line1A, line1B, line2A, line2B));

        Debug.DrawLine(line1A, line1B, Color.cyan);

        points.Log();
        Debug.Break();

        return points;
        }

    private Vector3 GetNearestPoint(Vector3 line1A, List<Vector3> points) {
        float distance = Vector3.Distance(line1A, points[0]);

        int id = 0, counter = 0;

        foreach (var p in points) {
            float dist = Vector3.Distance(line1A, p);

            if (dist < distance) {
                id = counter;
                distance = dist;
                }

            counter++;
            }

        return points[id];
        }
    }