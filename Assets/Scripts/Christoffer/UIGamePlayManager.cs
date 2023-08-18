using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class UIGamePlayManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4];
    [SerializeField] GameObject questionUIObject;
    [SerializeField] QuestionManager questionManager;

    int activeQuestionIndex = 0;

    QuestionManager.QuestionAnswerData answerData;

	public override void OnNetworkSpawn()
	{
        answerData = new();
	}

	public void NewQuestionShow(Question_SO question_SO)
    {
        questionUIObject.SetActive(true);
        questionText.text = question_SO.QuestionText;
        for (int i = 0; i < question_SO.QuestionAnswers.Length; i++)
        {
            answerTexts[i].text = question_SO.QuestionAnswers[i];
        }
        activeQuestionIndex++;
    }

    public void RegisterAnswer(int choosenAnswer)
    {
        questionManager.RecieveQuestionAnswer_ServerRpc(NetworkManager.Singleton.LocalClientId, activeQuestionIndex - 1, choosenAnswer);
        questionUIObject.SetActive(false);
    }
}
