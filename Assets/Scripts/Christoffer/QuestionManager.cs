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

    [SerializeField] List<(ulong, int, int)> savedAnswers = new();

    public override void OnNetworkSpawn()
    {
        GenerateGameQuestions();
        SetNewTargetHeight();
        //questionAnswer.OnValueChanged += (QuestionAnswerData previousValue, QuestionAnswerData newValue) =>
        //{
        //    RecordAnswer(newValue);
        //};
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

    [ServerRpc(RequireOwnership = false)]
    public void RecieveQuestionAnswer_ServerRpc(ulong playerID, int questionIndex, int answerIndex)
    {
        if (!IsHost) return;
        questionAnswer.Value = new QuestionAnswerData
        {
            PlayerID = playerID,
            QuestionIndex = questionIndex,
            AnswerIndex = answerIndex
        };
        RecordAnswer(questionAnswer.Value);
    }

    public void RecordAnswer(QuestionAnswerData newValue)
    {
        (ulong, int, int) answer = new();
        answer.Item1 = newValue.PlayerID;
        answer.Item2 = newValue.QuestionIndex;
        answer.Item3 = newValue.AnswerIndex;
        savedAnswers.Add(answer);
        //debugText.text = savedAnswers.Count.ToString();
        ShowAnswer_ClientRpc(savedAnswers.Count);
    }

    [ClientRpc]
    public void ShowAnswer_ClientRpc(int questionsAnswered)
    {
        debugText.text = questionsAnswered.ToString();
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
