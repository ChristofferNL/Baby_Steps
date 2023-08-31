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


    [Header("Settings Things")]
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject audioPopup;
    [SerializeField] GameObject howToPlayPopup;
    [SerializeField] GameObject quitPopup;
    [SerializeField] List<GraphicRaycaster> settingsButtonsRaycasters = new();

    [Header("SmoothPopupVariables")]
    [SerializeField] float popupSpeed = 8;
    [SerializeField] float popupStartSize = 0.3f;
    [SerializeField] AnimationCurve popupCurve;
    [SerializeField] bool SizeChangeIsRunning = false;

    public Sprite notSelectedSprite;
    public Sprite selectedSprite;

    int activeQuestionIndex = 0;

	private void FixedUpdate()
	{
		cameraFollowPoint.position = new Vector2(cameraFollowPoint.position.x, 
                                                playerOneTransform.position.y < playerTwoTransform.position.y ? playerOneTransform.position.y + cameraOffsetY : 
                                                playerTwoTransform.position.y + cameraOffsetY);
	}

    public void OpenSettings()
    {
        if(!SizeChangeIsRunning) StartCoroutine(OpenSettingsSmoothly());
    }

    IEnumerator OpenSettingsSmoothly()
    {
        SizeChangeIsRunning = true;
        Vector3 endScale = settingsMenu.transform.localScale * 2;   // the evaluate curve ends at 0.5 to allow overshoot before (for the bouncy feeling)
        settingsMenu.transform.localScale = settingsMenu.transform.localScale * popupStartSize;
        Vector3 startScale = settingsMenu.transform.localScale;
        settingsMenu.SetActive(true);
        for (float i = 0; i <= 1;)
        {
            settingsMenu.transform.localScale = Vector3.Lerp(startScale, endScale, popupCurve.Evaluate(i));
            i += Time.unscaledDeltaTime * popupSpeed;
            yield return 0;
        }
        settingsMenu.transform.localScale = endScale / 2;
        SizeChangeIsRunning = false;
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void ClosePopups()
    {
        audioPopup.SetActive(false);
        howToPlayPopup.SetActive(false);
        quitPopup.SetActive(false);
        foreach (var raycaster in settingsButtonsRaycasters)
        {
            raycaster.enabled = true;
        }
    }

    public void OpenAudioPopup()
    {
        audioPopup.SetActive(true);
        foreach (var raycaster in settingsButtonsRaycasters)
        {
            raycaster.enabled = false;
        }
    }

    public void OpenHowToPlayPopup()
    {
        howToPlayPopup.SetActive(true);
        foreach (var raycaster in settingsButtonsRaycasters)
        {
            raycaster.enabled = false;
        }
    }

    public void OpenQuitGamePopup()
    {
        quitPopup.SetActive(true);
        foreach (var raycaster in settingsButtonsRaycasters)
        {
            raycaster.enabled = false;
        }
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
        questionManager.SetTimeScale_ServerRpc(1);
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
        if (!SizeChangeIsRunning) StartCoroutine(ShowQuestionSmooth());

    }

    private IEnumerator ShowQuestionSmooth()
    {
        SizeChangeIsRunning = true;
        Vector3 endScale = questionUIObject.transform.localScale * 2;   // the evaluate curve ends at 0.5 to allow overshoot before (for the bouncy feeling)
        questionUIObject.transform.localScale = questionUIObject.transform.localScale * popupStartSize;
        Vector3 startScale = questionUIObject.transform.localScale;
        questionUIObject.SetActive(true);
        for (float i = 0; i <= 1;)
        {
            questionUIObject.transform.localScale = Vector3.Lerp(startScale, endScale, popupCurve.Evaluate(i));
            i += Time.unscaledDeltaTime * popupSpeed;
            yield return 0;
        }
        questionUIObject.transform.localScale = endScale / 2;
        SizeChangeIsRunning = false;
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
        if(inputField.text.Length > 0)
        {
			SendChatMessage_ServerRpc(inputField.text, NetworkManager.Singleton.LocalClientId);
			inputField.text = "";
		}
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
