using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "New Question")]
public class Question_SO : ScriptableObject
{
    public string QuestionText;
    public string[] QuestionAnswers = new string[4];
}
