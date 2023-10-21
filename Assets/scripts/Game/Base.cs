using System;
using UnityEngine;

public class Base : MonoBehaviour {
    public static Action OnBaseMoved;
    public static Action OnStartMoved;
    public static Action OnBallDestroy;

    private readonly float _speed = 8;// �������� �������� ���� � ����� �������

    private Vector3 _startMovePosition;// ������ ��������� ����
    private Vector3 _destination, _direction;// ���� ������� ���� � � ����� ����������� ��� ���������
    private bool _IsFirstBall = false;// ����� �� ������� ����, ������ �� ��� ��������
    private int _counterMovedBalls;// ���������� ����������� �����

    public Vector3 StartMovePosition => _startMovePosition;// ������ ��������� ����
    public bool IsAllowedMove => _counterMovedBalls <= 0;
    public bool IsBaseMoved => _direction != Vector3.zero;

    private void OnEnable() {
        Ball.OnBallDestroy += BallDisable;
        }
    private void OnDisable() {
        Ball.OnBallDestroy -= BallDisable;
        }

    private void Update() {
        Vector3 pos = transform.position;

        if (_direction == Vector3.zero) // ���� �� �����
            {
            if (!Data.DEBUG_MODE)
                return;

            if (IsAllowedMove.Not())
                return;

            Vector3 dir = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
                dir = Vector3.left;
            else if (Input.GetKey(KeyCode.D))
                dir = Vector3.right;

            if (Input.GetKey(KeyCode.W))
                dir = Vector3.up;
            else if (Input.GetKey(KeyCode.S))
                dir = Vector3.down;

            if (dir != Vector3.zero) {
                pos += _speed * Time.deltaTime * dir;

                if (dir == Vector3.left && pos.x <= Data.LEFT_BORDER)
                    pos.x = Data.LEFT_BORDER;
                else if (dir == Vector3.right && pos.x >= Data.RIGHT_BORDER)
                    pos.x = Data.RIGHT_BORDER;

                if (dir == Vector3.down && pos.y <= Data.DOWN_BORDER)
                    pos.y = Data.DOWN_BORDER;
                else if (dir == Vector3.up && pos.y >= Data.UP_BORDER)
                    pos.y = Data.UP_BORDER;
                }

            transform.position = pos;
            OnBaseMoved?.Invoke();
            return;
            }

        pos += _speed * Time.deltaTime * _direction;

        if ((_direction == Vector3.right && pos.x >= _destination.x) || (_direction == Vector3.left && pos.x <= _destination.x)) {
            pos = _destination;
            _direction = Vector3.zero;
            }

        transform.position = pos;
        OnBaseMoved?.Invoke();
        }

    public void StartMove(int counterBalls) {
        _IsFirstBall = false;
        _counterMovedBalls = counterBalls;
        _startMovePosition = transform.position;
        OnStartMoved?.Invoke();
        }

    private void MoveToNewPosition(float x) {
        _direction = (x > transform.position.x) ? Vector3.right : Vector3.left;
        _destination = new Vector3(x, transform.position.y, transform.position.z);
        }

    private void BallDisable(float x) {
        _counterMovedBalls--;
        OnBallDestroy?.Invoke();

        if (!_IsFirstBall) {
            _IsFirstBall = true;
            if (Data.DEBUG_MODE && Data.MOVE_BASE_FROM_FIRSTBALL.Not())
                return;
            MoveToNewPosition(x);
            }
        }
    }