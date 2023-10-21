using System;
using UnityEngine;

public class InputController : MonoBehaviour {
    public static Action OnDrag;
    public static Action OnPointerDown;
    public static Action OnPointerUp;
    public static Action OnPointerEnter;
    public static Action OnPointerExit;

    private static Vector3 _mousePosition, _worldMousePosition;

    private bool _active = false, _inBoard = false;
    private Camera _cam;

    public static Vector3 MousePosition => _mousePosition;
    public static Vector3 WorldMousePosition => _worldMousePosition;

    public void Start() {
        _cam = Camera.main;
        }

    public void Drag() {
        if (!_inBoard)
            return;
        UpdateMousePosition();
        OnDrag?.Invoke();
        }

    public void PointerEnter() {
        if (!_active)
            return;
        _inBoard = true;
        UpdateMousePosition();
        OnPointerEnter?.Invoke();
        }

    public void PointerExit() {
        if (!_active)
            return;
        _inBoard = false;
        UpdateMousePosition();
        OnPointerExit?.Invoke();
        }

    public void PointerDown() => ActiveBoard();
    public void PointerUp() => ActiveBoard(false);

    private void ActiveBoard(bool enable = true) {
        _active = enable;
        _inBoard = enable;
        UpdateMousePosition();
        if (enable)
            OnPointerDown?.Invoke();
        else
            OnPointerUp?.Invoke();
        }
    private void UpdateMousePosition() {
        _mousePosition = Input.mousePosition;
        _worldMousePosition = _cam.ScreenToWorldPoint(_mousePosition);
        _worldMousePosition.z = 0;
        }
    }