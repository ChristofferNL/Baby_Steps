using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public struct QuestionData : INetworkSerializable
    {
        public string Question;
        public string answerOne;
        public string answerTwo;
        public string answerThree;
        public string answerFour;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Question);
            serializer.SerializeValue(ref answerOne);
            serializer.SerializeValue(ref answerTwo);
            serializer.SerializeValue(ref answerThree);
            serializer.SerializeValue(ref answerFour);
        }
    }

    public struct FinalAnswerData : INetworkSerializable
    {
        public string Question;
        public string answerPlayerOne;
        public string answerPlayerTwo;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Question);
            serializer.SerializeValue(ref answerPlayerOne);
            serializer.SerializeValue(ref answerPlayerTwo);
        }
    }

    public NetworkVariable<QuestionAnswerData> questionAnswer = new NetworkVariable<QuestionAnswerData>(new QuestionAnswerData { PlayerID = 10, QuestionIndex = 70, AnswerIndex = 10}, 
                                                                                                NetworkVariableReadPermission.Everyone, 
                                                                                                NetworkVariableWritePermission.Server);

    public NetworkVariable<QuestionData> activeQuestion = new NetworkVariable<QuestionData>(new QuestionData() { Question = "", answerOne = "", answerTwo = "", answerThree = "", answerFour = ""}, 
                                                                                                NetworkVariableReadPermission.Everyone, 
                                                                                                NetworkVariableWritePermission.Server);

    public NetworkVariable<FinalAnswerData> finalAnswer = new NetworkVariable<FinalAnswerData>(new FinalAnswerData() { Question = "", answerPlayerOne = "", answerPlayerTwo = "" }, 
                                                                                                NetworkVariableReadPermission.Everyone, 
                                                                                                NetworkVariableWritePermission.Owner);

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

    [SerializeField] List<(ulong, int, int)> savedAnswers = new();

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

    [ServerRpc(RequireOwnership = false)]
    public void OpenNextQuestion_ServerRpc()
    {
        if (questionsAnswered >= questionsPerRun) return;

        activeQuestion.Value = new QuestionData 
        {
            Question = selectedQuestions[questionsAnswered].QuestionText,
            answerOne = selectedQuestions[questionsAnswered].QuestionAnswers[0],
            answerTwo = selectedQuestions[questionsAnswered].QuestionAnswers[1],
            answerThree = selectedQuestions[questionsAnswered].QuestionAnswers[2],
            answerFour = selectedQuestions[questionsAnswered].QuestionAnswers[3],
        };

        uiGamePlayManager.NewQuestionShow_ClientRpc(activeQuestion.Value);
        questionsAnswered++;
        SetNewTargetHeight();
        SpawnQuestion = false;
        SetTimeScale_ClientRpc(0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RecieveQuestionAnswer_ServerRpc(ulong playerID, int questionIndex, int answerIndex)
    {
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
        if (savedAnswers.Count == questionsAnswered * 2)
        {
            SetTimeScale_ClientRpc(1);
        }
        if (savedAnswers.Count / 2 == questionsPerRun)
        {
            ShowAnswer(); // end game
            SetTimeScale_ClientRpc(0);
        }
    }

    [ClientRpc]
    void SetTimeScale_ClientRpc(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    public void ShowAnswer()
    {
        string questionTemp = "";
        string answerOneTemp = "";
        string answerTwoTemp = "";

        for (int i = 0; i < selectedQuestions.Count; i++)
        {
            questionTemp = $"Question: { selectedQuestions[i].QuestionText}\n";
            var answers = (from ans in savedAnswers where ans.Item2 == i select ans).ToList();
            foreach (var item in answers)
            {
                if (item.Item1 == 0)
                {
                    answerOneTemp = $"Player {item.Item1}  Answer: {selectedQuestions[i].QuestionAnswers[item.Item3]}";
                }
                else
                {
                    answerTwoTemp = $"Player {item.Item1}  Answer: {selectedQuestions[i].QuestionAnswers[item.Item3]}";
                }
            }
            finalAnswer.Value = new FinalAnswerData
            {
                Question = questionTemp,
                answerPlayerOne = answerOneTemp,
                answerPlayerTwo = answerTwoTemp
            };
            uiGamePlayManager.FinalAnswersShow_ClientRpc(finalAnswer.Value);
        }
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
