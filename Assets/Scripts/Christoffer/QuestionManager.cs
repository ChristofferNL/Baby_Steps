using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class QuestionManager : NetworkBehaviour
{
    public struct QuestionAnswerData : INetworkSerializable
    {
        public ulong PlayerID;
        public int QuestionIndex;
        public int AnswerIndex;

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref QuestionIndex);
            serializer.SerializeValue(ref AnswerIndex);
		}
	}

    public NetworkVariable<QuestionAnswerData> questionAnswer = new NetworkVariable<QuestionAnswerData>(new QuestionAnswerData { PlayerID = 10, QuestionIndex = 70, AnswerIndex = 10}, 
                                                                                                NetworkVariableReadPermission.Everyone, 
                                                                                                NetworkVariableWritePermission.Server);

    [SerializeField] List<Question_SO> allQuestions = new();
    [SerializeField] float questionTimerSeconds = 15f;
    [SerializeField] int questionsPerRun = 0;
    [SerializeField] List<Question_SO> selectedQuestions = new();
    [SerializeField] int distanceToSpawnQuestion;
    [SerializeField] UIGamePlayManager uiGamePlayManager;
    [SerializeField] Transform playerTransform;
    [SerializeField] TextMeshProUGUI debugText;

    public bool SpawnQuestion;

    int questionsAnswered;
    int nextQuestionHeightTarget;
    
    public override void OnNetworkSpawn()
    {
        GenerateGameQuestions();
        SetNewTargetHeight();
        questionAnswer.OnValueChanged += (QuestionAnswerData previousValue, QuestionAnswerData newValue) =>
        {
            RecordAnswer(newValue);
        };
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
        if (questionsAnswered >= questionsPerRun) return;

        uiGamePlayManager.NewQuestionShow(selectedQuestions[questionsAnswered]);
        questionsAnswered++;
        SetNewTargetHeight();
        SpawnQuestion = false;
    }

    public void SendQuestionAnswer(ulong playerID, int questionIndex, int answerIndex)
    {
        questionAnswer.Value = new QuestionAnswerData
        {
            PlayerID = playerID,
            QuestionIndex = questionIndex,
            AnswerIndex = answerIndex
        };
    }

    public void RecordAnswer(QuestionAnswerData newValue)
    {
        debugText.text = $"{newValue.PlayerID} {newValue.QuestionIndex} {newValue.AnswerIndex}";
    }

    public void ShowQuestionAnswers()
    {
        //foreach (var item in answersTest)
        //{
        //    Debug.Log($"player: {item.Item1} question: {item.Item2} answer: {item.Item3}");
        //}
    }

    void GenerateGameQuestions()
    {
        selectedQuestions.Clear();
        while (selectedQuestions.Count < questionsPerRun && selectedQuestions.Count < allQuestions.Count)
        {
            int value = Random.Range(0, allQuestions.Count);
            selectedQuestions.Add(allQuestions[value]);
            allQuestions.RemoveAt(value);
        }
    }
}
