using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoBehaviour
{
    public Text texto;
    public Button closeButton;
    private void Start() {
        closeButton.onClick.AddListener(Close);
    }
    public void Initialize(string _texto){
        texto.text = _texto;
    }
    void Close(){
        Destroy(this.gameObject);
    }
}
