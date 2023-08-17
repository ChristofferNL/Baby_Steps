using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class QuestionManager : NetworkBehaviour
{
    public struct QuestionAnswerData
    {
        public int playerID;
        public int questionIndex;
        public int questionAnswerIndex;
    }

    [SerializeField] List<Question_SO> allQuestions = new();
    [SerializeField] float questionTimerSeconds = 15f;
    [SerializeField] int questionsPerRun = 0;
    [SerializeField] List<Question_SO> selectedQuestions = new();
    [SerializeField] int distanceToSpawnQuestion;
    [SerializeField] UIGamePlayManager uiGamePlayManager;
    [SerializeField] Transform playerTransform;

    public bool SpawnQuestion;

    int questionsAnswered;
    int nextQuestionHeightTarget;

    List<QuestionAnswerData> answersData;

    public override void OnNetworkSpawn()
    {
        GenerateGameQuestions();
        SetNewTargetHeight();
    }

    private void FixedUpdate()
    {
        if (playerTransform.position.y >= nextQuestionHeightTarget)
        {
            SpawnQuestion = true;
        }
    }

    void SetNewTargetHeight()
    {
        nextQuestionHeightTarget = (int)playerTransform.position.y + distanceToSpawnQuestion;
    }

    public void OpenNextQuestion()
    {
        uiGamePlayManager.NewQuestionShow(selectedQuestions[questionsAnswered]);
        questionsAnswered++;
        SetNewTargetHeight();
        SpawnQuestion = false;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SendQuestionAnswer_ServerRpc(ulong userID, int questionAnswer)
    {
        if (!IsOwner) return;
        QuestionAnswerData answerData = new QuestionAnswerData()
        {
            playerID = (int)userID,
            questionIndex = questionsAnswered - 1,
            questionAnswerIndex = questionAnswer
        };
        answersData.Add(answerData);
    }

    [ClientRpc]
    public void ShowQuestionAnswers_ClientRpc()
    {
        if(!IsOwner) return;
        foreach (var item in answersData)
        {
            Debug.Log($"player: {item.playerID} question: {item.questionIndex} answer: {item.questionAnswerIndex}");
        }
    }

    List<Question_SO> GenerateGameQuestions()
    {
        selectedQuestions.Clear();
        List<Question_SO> questions = new();
        while (selectedQuestions.Count < questionsPerRun && selectedQuestions.Count < allQuestions.Count)
        {
            int value = Random.Range(0, allQuestions.Count);
            selectedQuestions.Add(allQuestions[value]);
            allQuestions.RemoveAt(value);
        }
        return questions;
    }
}
