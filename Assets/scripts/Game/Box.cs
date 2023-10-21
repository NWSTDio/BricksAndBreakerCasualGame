using UnityEngine;

public class Box : MonoBehaviour {
    [SerializeField] private Animator _animator;

    private float _width, _height;
    private int _hp = 100;

    public float MinX => transform.position.x - _width / 2;
    public float MaxX => transform.position.x + _width / 2;
    public float MinY => transform.position.y - _height / 2;
    public float MaxY => transform.position.y + _height / 2;

    public bool IsLive => _hp > 0;

    public float Width => _width;
    public float Height => _height;

    public Vector3 Position => transform.position;

    private void Start() {
        _width = transform.localScale.x;
        _height = transform.localScale.y;
        }

    public void TakeDamage() {
        if (--_hp <= 0) {
            Destroy(gameObject);
            return;
            }
        _animator.SetTrigger("damage");
        }
    }