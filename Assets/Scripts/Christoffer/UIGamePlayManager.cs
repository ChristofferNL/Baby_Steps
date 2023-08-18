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
    [SerializeField] GameObject answerScrollRectObject;
    [SerializeField] FinalAnswerWidget finalAnswerObject;
    [SerializeField] GameObject finalAnswersParent;

    int activeQuestionIndex = 0;

    [ClientRpc]
	public void NewQuestionShow_ClientRpc(QuestionManager.QuestionData questionData)
    {
        questionUIObject.SetActive(true);
        questionText.text = questionData.Question;
        answerTexts[0].text = questionData.answerOne;
        answerTexts[1].text = questionData.answerTwo;
        answerTexts[2].text = questionData.answerThree;
        answerTexts[3].text = questionData.answerFour;
        activeQuestionIndex++;
    }

    [ClientRpc]  
    public void FinalAnswersShow_ClientRpc(QuestionManager.FinalAnswerData finalAnswers)
    {
        answerScrollRectObject.SetActive(true);

        GameObject newAnswer = Instantiate(finalAnswerObject.gameObject, finalAnswersParent.transform, false);
        newAnswer.GetComponent<FinalAnswerWidget>().SetupAnswerWidget(finalAnswers.Question, finalAnswers.answerPlayerOne, finalAnswers.answerPlayerTwo);

        //finalAnswersText.text += $"{finalAnswers.Question} {finalAnswers.answerPlayerOne} {finalAnswers.answerPlayerTwo}";
    }

    public void RegisterAnswer(int choosenAnswer)
    {
        questionManager.RecieveQuestionAnswer_ServerRpc(NetworkManager.Singleton.LocalClientId, activeQuestionIndex - 1, choosenAnswer);
        questionUIObject.SetActive(false);
    }
}
