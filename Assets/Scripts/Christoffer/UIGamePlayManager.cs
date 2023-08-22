using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIGamePlayManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4];
    [SerializeField] GameObject questionUIObject;
    [SerializeField] QuestionManager questionManager;
    [SerializeField] GameObject answerScrollRectObject;
    [SerializeField] FinalAnswerWidget finalAnswerObject;
    [SerializeField] GameObject finalAnswersParent;
    [SerializeField] Transform cameraFollowPoint;
    [SerializeField] Transform playerOneTransform;
    [SerializeField] ChatMessageWidget chatMessageObject;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] ScrollRect chatScrollRect;

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
        newAnswer.GetComponent<FinalAnswerWidget>().SetupAnswerWidget(finalAnswers.Question, finalAnswers.answerPlayerOne, finalAnswers.answerPlayerTwo, finalAnswers.isSameAnswer);
    }

    public void InitiateChatMessage()
    {
        SendChatMessage_ServerRpc(inputField.text);
		inputField.text = "";
	}

	[ServerRpc(RequireOwnership = false)]
	void SendChatMessage_ServerRpc(FixedString512Bytes message)
	{
        ChatMessageShow_ClientRpc(message);
	}

	[ClientRpc]
    public void ChatMessageShow_ClientRpc(FixedString512Bytes message)
    {
        GameObject newMessage = Instantiate(chatMessageObject.gameObject, finalAnswersParent.transform, false);
        newMessage.GetComponent<ChatMessageWidget>().SetupChatObject(message.ToString(), "player");
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        chatScrollRect.verticalNormalizedPosition = -0.2f;
    }

    public void RegisterAnswer(int choosenAnswer)
    {
        cameraFollowPoint.position = new Vector2(cameraFollowPoint.position.x, playerOneTransform.position.y + 14);
        questionManager.RecieveQuestionAnswer_ServerRpc(NetworkManager.Singleton.LocalClientId, activeQuestionIndex - 1, choosenAnswer);
        questionUIObject.SetActive(false);
    }
}
