using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pergunta", menuName = "Covideogames/QuizPerguntas", order = 0)]
public class QuizPerguntas : ScriptableObject
{
    public string pergunta;
    public string explicacao;
    public bool correta;
}
