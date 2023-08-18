using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ClientMainGame : SingletonMonoBehaviour<ClientMainGame>
{
    [Header("Data Fields")]
    public GameObject dataFieldsObj;
    public TextMeshProUGUI nameMesh;
    public ClientDataField banked;
    public ClientDataField atRisk;
    public ClientDataField potential;

    [Header("Main Game Area")]
    public Animator mainGameAreaAnim;
    public Animator timerAppearanceAnim;

    public Animator questionMetaDataAnim;
    public TextMeshProUGUI questionMetaDataMesh;

    public Animator questionStrapAnim;
    public TextMeshProUGUI questionStrapMesh;

    [Header("R1 Buttons")]
    public AnswerButton[] r1AnswerButtons;
    public AnswerDisplay[] r1AnswerDisplays;

    [Header("Leaderboard")]
    public ClientLeaderboardManager leaderboardManager;

    [Header("Fixed Message")]
    public GameObject fixedMessageObj;
    public TextMeshProUGUI fixedMessageMesh;

    private void Update()
    {
        /*submitButton.interactable = answerInput.text.Length <= 0 ? false : true;

        if (enterSubmits && answerInput.text.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            OnSubmitAnswer();*/
    }

    public void Initialise(string[] data)
    {
        leaderboardManager.gameObject.SetActive(true);
        leaderboardManager.RefreshScrollRect();

        dataFieldsObj.SetActive(true);
        nameMesh.text = data[0];
        banked.mesh.text = data[1];
        atRisk.mesh.text = data[2];
    }

    public void DisplayCountdown()
    {

    }

    public void DisplayQuestion()
    {

    }

    public void OnSubmitR1(int pressedOption)
    {
        for(int i = 0; i < r1AnswerButtons.Length; i++)
            r1AnswerButtons[i].PlayerHasAnswered(i == pressedOption);

        ClientManager.Get.SendPayloadToHost("Get answer from index pushed and loaded question (which will be sent down in the payload)", EventLibrary.ClientEventType.Answer);
    }

    public void DisplayResponse(string[] data)
    {

    }

    public void DisplayAnswer(string data)
    {

    }

    public void UpdateCurrentScore(string data)
    {

    }

    public void ResetForNewQuestion()
    {
        
    }

    public void NewInstanceOpened()
    {
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE TWITCH ACCOUNT ASSOCIATED WITH THIS CONTROLLER HAS BEEN ASSOCIATED WITH ANOTHER CONTROLLER.\n\n" +
            "THIS CONTROLLER WILL NOT RECEIVE FURTHER DATA FROM THE GAME AND CAN NOW BE CLOSED.\n\n" +
            "IF YOU DID  NOT VALIDATE A NEW CONTROLLER, PLEASE CONTACT THE HOST.";
    }

    public void EndOfGameAlert(string pennyValue)
    {
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE GAME HAS NOW CONCLUDED AND THIS CONTROLLER CAN BE CLOSED.";
    }

    public void WrongApp()
    {
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "YOU ARE ATTEMPTING TO PLAY THE GAME USING THE WRONG CONTROLLER APP.\n\n" +
            "PLEASE CHECK THE GAME PAGE FOR THE CORRECT LINK TO THE CURRENT GAME.";
    }

    public void UpdateLeaderboard(string data)
    {
        string[] players = data.Split('¬');
        for (int i = 0; i < players.Length; i++)
        {
            //[0] = Name
            //[1] = Bank
            //[2] = Risk
            string[] splitData = players[i].Split('|');
            leaderboardManager.PopulateStrap(splitData, i);
        }
        //leaderboardManager.RefreshScrollRect();
    }
}
