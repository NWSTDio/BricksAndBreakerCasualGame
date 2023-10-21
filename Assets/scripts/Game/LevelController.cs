using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Traectory _traectory;
    [SerializeField] private Base _base;
    [SerializeField] private Vector3 _pos, _dir;

    private readonly List<Ball> _balls = new();
    private int id = 0;

    private void Start() {
        for (int i = 0; i < Data.COUNT_BALL; i++)
            _balls.Add(AddBall());
        }

    private IEnumerator Step() {
        _base.StartMove(_balls.Count);// �������� ��� �� �����

        var wait = new WaitForSeconds(.05f);

        foreach (var ball in _balls) {
            if (Data.DEBUG_MODE.Not() || (Data.DEBUG_MODE && Data.HIDE_BALL))
                ball.gameObject.SetActive(true);
            if (Data.DEBUG_MODE && Data.MANUAL_MODE)
                ball.Move(_pos, _dir);
            else
                ball.Move(_base.StartMovePosition, _traectory.Direction);
            yield return wait;
            }
        }

    private Ball AddBall() {
        GameObject ball = Instantiate(_ballPrefab, _traectory.transform.position, Quaternion.identity);
        if (Data.DEBUG_MODE.Not() || (Data.DEBUG_MODE && Data.HIDE_BALL))
            ball.SetActive(false);
        ball.name = "ball_" + (id++);
        return ball.GetComponent<Ball>();
        }

    public void StartMove() => StartCoroutine(Step());
    }