using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour {
    [SerializeField] private Text _dirText;

    public void ChageDirText(Vector2 direction) => _dirText.text = $"{direction.x} : {direction.y}";
    }