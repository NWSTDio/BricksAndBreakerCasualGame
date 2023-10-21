using UnityEngine;
using UnityEngine.UI;

public class CounterBalls : MonoBehaviour
    {
    [SerializeField] private Base _base;

    private Text _text;

    private int _counter;

    private void Start()
        {
        _text = GetComponent<Text>();

        transform.position = _base.transform.position + new Vector3(0, .15f, 0);
        }

    private void StartMoved()
        {
        _counter = 0;
        UpdateText();
        }

    private void BallDestroy()
        {
        _counter++;
        UpdateText();
        }
    private void UpdateText() => _text.text = $"X {_counter}";
    private void UpdatePosition() => transform.position = new Vector3(_base.transform.position.x, transform.position.y, transform.position.z);
    
    private void OnEnable()
        {
        Base.OnBaseMoved += UpdatePosition;
        Base.OnStartMoved += StartMoved;
        Base.OnBallDestroy += BallDestroy;
        }
    private void OnDisable()
        {
        Base.OnBaseMoved -= UpdatePosition;
        Base.OnStartMoved -= StartMoved;
        Base.OnBallDestroy -= BallDestroy;
        }
    }