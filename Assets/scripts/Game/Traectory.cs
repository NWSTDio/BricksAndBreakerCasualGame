using System.Collections.Generic;
using UnityEngine;

public class Traectory : MonoBehaviour {
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private float _normalScale, _bigScale;

    private readonly List<Transform> _dots = new();
    private Vector3 _direction;
    private float _y;

    public Vector3 Direction => _direction;

    public void Start() {
        for (int i = 0; i < Data.TRAECTORY_DOTS; i++)
            _dots.Add(CreateDot());
        _y = transform.position.y;
        }

    public void RefreshDots(Vector3 point) {
        Vector3 direction = (point - transform.position).normalized;

        CalculateDots(direction);
        }

    public void RefreshDots() // �������� ���
        {
        Vector3 direction = (InputController.WorldMousePosition - transform.position).normalized;

        if (InputController.WorldMousePosition.y < _y + _bigScale / 2)
            return;
        CalculateDots(direction);
        }
    private void CalculateDots(Vector3 direction) {
        _direction = direction;// �������� ��������� �����������

        Vector3 position = transform.position;// ������ �������� ����������

        bool border = false, finish = false;// ���� �� ������� ��� ����� ����

        int counter = 0;// ������� ��� 6 ��������� ����� �� �������

        Vector2 scale;// ����������� ������

        foreach (var dot in _dots) {
            if (!finish) // ���� ��� ����� ��������� �����
                {
                position += direction * .2f;// ������� ����� ����

                scale = new Vector2(_normalScale, _normalScale);

                if (CollisionHorizontalBorder(position, out position)) {
                    direction.x *= -1;
                    border = true;
                    scale = new Vector2(_bigScale, _bigScale);
                    }

                if (CollisionVerticalBorder(position, out position)) {
                    direction.y *= -1;
                    border = true;
                    scale = new Vector2(_bigScale, _bigScale);
                    }

                if (border)
                    counter++;

                if (counter > 7) {
                    dot.gameObject.SetActive(false);
                    finish = true;
                    }
                else {
                    dot.gameObject.SetActive(true);
                    dot.transform.position = position;
                    dot.transform.localScale = scale;
                    }
                }
            else // �������� ��� �� ������ ���������� �����
                {
                if (!dot.gameObject.activeSelf) // ���� ����� ���������
                    break;
                dot.gameObject.SetActive(false);// �������� �����
                }
            }
        }

    public void ResetDots() {
        foreach (var dot in _dots)
            dot.gameObject.SetActive(false);
        }

    private Transform CreateDot() {
        GameObject obj = Instantiate(_dotPrefab, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        return obj.transform;
        }
    private bool CollisionHorizontalBorder(Vector3 position, out Vector3 pos) {
        pos = position;
        if (pos.x < Data.LEFT_BORDER) {
            pos.x = Data.LEFT_BORDER;
            return true;
            }
        else if (pos.x > Data.RIGHT_BORDER) {
            pos.x = Data.RIGHT_BORDER;
            return true;
            }
        return false;
        }
    private bool CollisionVerticalBorder(Vector3 position, out Vector3 pos) {
        pos = position;
        if (pos.y > Data.UP_BORDER) {
            pos.y = Data.UP_BORDER;
            return true;
            }
        return false;
        }
    }
