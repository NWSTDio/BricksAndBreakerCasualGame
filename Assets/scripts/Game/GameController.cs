using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController Instance;

    [SerializeField] private Base _base;
    [SerializeField] private Traectory _traectory;
    [SerializeField] private LevelController _level;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private Status _status;

    private bool _active = false, _exit = false;
    private float _pointerSpeed = 1f;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        }

    private void Start() {
        Data.LEFT_BORDER = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        Data.RIGHT_BORDER = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        Data.DOWN_BORDER = _traectory.transform.position.y;
        Data.RUNNING = true;
        }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.R))
            SceneManager.LoadScene(0);

        if (Data.DEBUG_MODE) {
            if (Input.GetKeyUp(KeyCode.Q)) {
                _pointer.SetActive(true);
                _status.gameObject.SetActive(true);
                Data.MOUSE_OFF = true;
                Debug.Log("����� ���� ��������!");
                _traectory.RefreshDots();
                }
            else if (Input.GetKeyUp(KeyCode.E)) {
                _pointer.SetActive(false);
                _status.gameObject.SetActive(false);
                Data.MOUSE_OFF = false;
                Debug.Log("����� ���� �������!");
                _traectory.ResetDots();
                }

            if (Input.GetKeyUp(KeyCode.I)) {
                Data.MANUAL_MODE = !Data.MANUAL_MODE;
                Debug.Log("������ ����� <color=green>" + (Data.MANUAL_MODE ? "�������" : "��������") + "</color>");
                }

            if (Data.MOUSE_OFF) {
                if (Input.GetKeyUp(KeyCode.T) && _base.IsAllowedMove) {
                    _level.StartMove();
                    _traectory.ResetDots();
                    return;
                    }
                if (_base.IsAllowedMove) {
                    Vector3 pos = _pointer.transform.position;
                    Vector3 direction = Vector3.zero;

                    if (Input.GetKey(KeyCode.LeftArrow))
                        direction = Vector3.left;
                    else if (Input.GetKey(KeyCode.RightArrow))
                        direction = Vector3.right;

                    if (Input.GetKey(KeyCode.DownArrow))
                        direction = Vector3.down;
                    else if (Input.GetKey(KeyCode.UpArrow))
                        direction = Vector3.up;


                    if (direction != Vector3.zero) {
                        pos += _pointerSpeed * Time.deltaTime * direction;

                        if (pos.x <= Data.LEFT_BORDER)
                            pos.x = Data.LEFT_BORDER;
                        else if (pos.x >= Data.RIGHT_BORDER)
                            pos.x = Data.RIGHT_BORDER;

                        if (pos.y <= Data.DOWN_BORDER + .2f)
                            pos.y = Data.DOWN_BORDER + .2f;
                        else if (pos.y >= Data.UP_BORDER)
                            pos.y = Data.UP_BORDER;
                        _pointer.transform.position = pos;
                        _traectory.RefreshDots(pos);
                        _status.ChageDirText(_traectory.Direction);
                        }
                    }
                }
            if (Data.MANUAL_MODE) {
                if (Input.GetKeyUp(KeyCode.O) && _base.IsAllowedMove) {
                    Debug.Log("<color=red>STEP ZIPP</color>");
                    _level.StartMove();
                    }
                }
            }
        }
    private void OnEnable() {
        InputController.OnPointerDown += PointerDown;
        InputController.OnPointerUp += PointerUp;
        InputController.OnDrag += Drag;
        InputController.OnPointerExit += PointerExit;
        InputController.OnPointerEnter += PointerEnter;
        }

    private void OnDisable() {
        InputController.OnPointerDown -= PointerDown;
        InputController.OnPointerUp -= PointerUp;
        InputController.OnDrag -= Drag;
        InputController.OnPointerExit -= PointerExit;
        InputController.OnPointerEnter -= PointerEnter;
        }

    private void PointerDown() {
        if (Data.DEBUG_MODE && Data.MANUAL_MODE)
            return;
        if (Data.DEBUG_MODE && Data.MOUSE_OFF)
            return;

        if (!_base.IsAllowedMove)
            return;

        _active = true;
        _traectory.RefreshDots();
        }

    private void PointerUp() {
        if (Data.MANUAL_MODE)
            return;

        if (_active && _base.IsAllowedMove && !_exit) {
            _level.StartMove();
            Debug.Log("DIRECTION" + _traectory.Direction);
            Debug.Log("POSITION" + _base.transform.position);
            }

        if (_exit)
            _exit = false;

        _active = false;
        _traectory.ResetDots();
        }

    private void PointerExit() {
        _exit = true;
        _traectory.ResetDots();
        }

    private void PointerEnter() {
        if (_exit)
            _exit = false;
        }

    private void Drag() {
        if (!_base.IsAllowedMove)
            return;
        _traectory.RefreshDots();
        }
    }