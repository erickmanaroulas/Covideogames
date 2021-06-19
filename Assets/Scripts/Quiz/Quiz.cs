using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Quiz : MonoBehaviour
{
    public Transform acertoPos;
    public Sprite[] acertoErro;
    public GameObject acertoObject;
    public QuizPerguntas[] perguntas;
    public QuizPerguntas perguntaAtual, ultimaPergunta;
    public Text perguntaTexto;
    public GameObject quizHolder, comecarHolder;
    public GameObject vitoria,derrota;
    public GameObject popup;
    public int acertos;
    public int erros;
    public AudioSource _audio;
    public AudioClip acerto, erro;
    void SelecionarAleatoria(){
        perguntaAtual = perguntas[Random.Range(0, perguntas.Length - 1)];
        if(perguntaAtual == ultimaPergunta){
          perguntaAtual = perguntas[Random.Range(0, perguntas.Length - 1)];  
        }
    }
    void RefreshUI(){
        perguntaTexto.text = perguntaAtual.pergunta;
    }
    void ChecarAcertosErros(){
        if(acertos == 3){
            vitoria.SetActive(true);
        }
        if(erros == 2){
            derrota.SetActive(true);
        }
    }
    public void ChecarEscolha(bool value){
        if(value != perguntaAtual.correta){
            var _erro = Instantiate(acertoObject, acertoPos);
            _erro.GetComponent<Image>().sprite = acertoErro[1];
            Destroy(_erro, 1.25f);
            _audio.PlayOneShot(erro);
            erros++;
        }else{
            var _acerto = Instantiate(acertoObject, acertoPos);
            _acerto.GetComponent<Image>().sprite = acertoErro[0];
            Destroy(_acerto, 1.25f);
            _audio.PlayOneShot(acerto);
            acertos++;
        }
        var _popup = Instantiate(popup, this.transform);
        _popup.GetComponent<Popup>().Initialize(perguntaAtual.explicacao);
        ChecarAcertosErros();
        ultimaPergunta = perguntaAtual;
        SelecionarAleatoria();
        RefreshUI();
    }
    public void Voltar(){
        SceneManager.LoadScene(0);
    }
    public void Comecar(){
        comecarHolder.SetActive(false);
        quizHolder.SetActive(true);
        SelecionarAleatoria();
        RefreshUI();
    }
}
