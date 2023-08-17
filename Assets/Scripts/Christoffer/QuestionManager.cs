using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class QuestionManager : NetworkBehaviour
{
    [SerializeField] List<Question_SO> allQuestions = new();
    [SerializeField] float questionTimerSeconds = 15f;
    [SerializeField] Question_SO[] selectedQuestions = new Question_SO[10];

}
