using Cinemachine;
using LobbyRelaySample;
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
    [SerializeField] float cameraOffsetY = 10;
    [SerializeField] Transform playerOneTransform;
    [SerializeField] Transform playerTwoTransform;
    [SerializeField] ChatMessageWidget chatMessageObject;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] ScrollRect chatScrollRect;
    [SerializeField] UnityEngine.UI.Image leftPlayerImage;
    [SerializeField] UnityEngine.UI.Image rightPlayerImage;
    [SerializeField] TextMeshProUGUI leftPlayerName;
    [SerializeField] TextMeshProUGUI rightPlayerName;
    [SerializeField] UnityEngine.UI.Slider progressSlider;
    [SerializeField] GameObject HudObject;
    [SerializeField] InputManager inputManager;
    [SerializeField] EmoticonWidget leftPlayerEmoteUIObject;
    [SerializeField] EmoticonWidget rightPlayerEmoteUIObject;
    [SerializeField] GameObject loadingScreenObject;
    [SerializeField] float loadingScreenWaitSeconds = 4;

    public Sprite notSelectedSprite;
    public Sprite selectedSprite;

    int activeQuestionIndex = 0;

	private void FixedUpdate()
	{
		cameraFollowPoint.position = new Vector2(cameraFollowPoint.position.x, 
                                                playerOneTransform.position.y < playerTwoTransform.position.y ? playerOneTransform.position.y + cameraOffsetY : 
                                                playerTwoTransform.position.y + cameraOffsetY);
	}

	public void SetupPlayersUI()
    {
        for (int i = 0; i < GameManager.Instance.LocalLobby.PlayerCount; i++)
        {
            var player = GameManager.Instance.LocalLobby.GetLocalPlayer(i);
            if (player == null)
                continue;
            SetupPlayer(player);
        }
        StartCoroutine(LoadingWait());
    }

    public void ExitCurrentGame()
    {
        GameManager.Instance.ClientQuitGame();
    }

    IEnumerator LoadingWait()
    {
        yield return new WaitForSecondsRealtime(loadingScreenWaitSeconds);
        progressSlider.maxValue = questionManager.questionsPerRun;
        HudObject.SetActive(true);
        inputManager.EnableControls();
        loadingScreenObject.SetActive(false);
    }

    void SetupPlayer(LocalPlayer player)
    {
        if (player.IsHost.Value)
        {
            leftPlayerName.text = player.DisplayName.Value;
            if (NetworkManager.Singleton.LocalClientId == 0)
            {
				rightPlayerEmoteUIObject.gameObject.SetActive(false);
			}
        }
        else
        {
            rightPlayerName.text = player.DisplayName.Value;
			if (NetworkManager.Singleton.LocalClientId != 0)
			{
				leftPlayerEmoteUIObject.gameObject.SetActive(false);
			} 
        }
    }

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
        progressSlider.value++;
    }

    [ClientRpc]  
    public void FinalAnswersShow_ClientRpc(QuestionManager.FinalAnswerData finalAnswers)
    {
        answerScrollRectObject.SetActive(true);
        HudObject.SetActive(false);
        GameObject newAnswer = Instantiate(finalAnswerObject.gameObject, finalAnswersParent.transform, false);
        newAnswer.GetComponent<FinalAnswerWidget>().SetupAnswerWidget(finalAnswers.Question, finalAnswers.answerPlayerOne, finalAnswers.answerPlayerTwo, finalAnswers.isSameAnswer);
    }

    public void InitiateChatMessage()
    {
        SendChatMessage_ServerRpc(inputField.text, NetworkManager.Singleton.LocalClientId);
		inputField.text = "";
	}

	[ServerRpc(RequireOwnership = false)]
	void SendChatMessage_ServerRpc(FixedString512Bytes message, ulong senderID)
	{
        for (int i = 0; i < GameManager.Instance.LocalLobby.PlayerCount; i++)
        {
            var player = GameManager.Instance.LocalLobby.GetLocalPlayer(i);
            if (player == null)
                continue;
            if (player.Index.Value == (int)senderID)
            {
                bool isPlayerOne = player.Index.Value == 0;
                ChatMessageShow_ClientRpc(message, player.DisplayName.Value, isPlayerOne);
            }
        }
	}

	[ClientRpc]
    public void ChatMessageShow_ClientRpc(FixedString512Bytes message, FixedString512Bytes playerName, bool isPlayerOne)
    {
        GameObject newMessage = Instantiate(chatMessageObject.gameObject, finalAnswersParent.transform, false);
        newMessage.GetComponent<ChatMessageWidget>().SetupChatObject(message.ToString(), playerName.ToString(), isPlayerOne);
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        float progress = 1 - chatScrollRect.verticalNormalizedPosition;
        float startPos = chatScrollRect.verticalNormalizedPosition;
        while (progress < 1f)
        {
            progress += Time.unscaledDeltaTime;
            float percentageDone = progress / 1;
            chatScrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, 0, percentageDone);
            yield return null;
        }
        chatScrollRect.velocity = Vector2.zero;
    }

    public void RegisterAnswer(int choosenAnswer)
    {
        questionManager.RecieveQuestionAnswer_ServerRpc(NetworkManager.Singleton.LocalClientId, activeQuestionIndex - 1, choosenAnswer);
        questionUIObject.SetActive(false);
    }
}
