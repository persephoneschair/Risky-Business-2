using Control;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerObject
{
    public string playerClientID;
    public Player playerClientRef;
    public Podium podium;
    public string otp;
    public string playerName;

    public GlobalLeaderboardStrap strap;
    public GlobalLeaderboardStrap cloneStrap;

    public string twitchName;
    public Texture profileImage;

    public bool eliminated;

    public int bankedPoints;
    public int riskPoints;
    public bool chosenToRisk;
    public int totalCorrect;
    public bool wasCorrect;
    public bool attemptedQ;
    public int pointsForNextQ;

    public PlayerObject(Player pl, string name)
    {
        playerClientRef = pl;
        otp = OTPGenerator.GenerateOTP();
        playerName = name;
        bankedPoints = 0;
        riskPoints = 0;
        pointsForNextQ = 1;
        //podium = Podiums.GetPodiums.podia.FirstOrDefault(x => x.containedPlayer == null);
        //podium.containedPlayer = this;
    }

    public void ApplyProfilePicture(string name, Texture tx, bool bypassSwitchAccount = false)
    {
        //Player refreshs and rejoins the same game
        if(PlayerManager.Get.players.Count(x => (!string.IsNullOrEmpty(x.twitchName)) && x.twitchName.ToLowerInvariant() == name.ToLowerInvariant()) > 0 && !bypassSwitchAccount)
        {
            PlayerObject oldPlayer = PlayerManager.Get.players.FirstOrDefault(x => x.twitchName.ToLowerInvariant() == name.ToLowerInvariant());
            if (oldPlayer == null)
                return;

            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.SecondInstance, "");

            oldPlayer.playerClientID = playerClientID;
            oldPlayer.playerClientRef = playerClientRef;
            oldPlayer.playerName = playerName;
            //oldPlayer.podium.playerNameMesh.text = playerName;

            otp = "";
            //podium.containedPlayer = null;
            //podium = null;
            playerClientRef = null;
            playerName = "";

            if (PlayerManager.Get.pendingPlayers.Contains(this))
                PlayerManager.Get.pendingPlayers.Remove(this);

            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.Validated, $"{oldPlayer.playerName}|<color=#F8A3A3>RISK: {oldPlayer.riskPoints.ToString()}</color>\nBANK: {oldPlayer.bankedPoints.ToString()}|{oldPlayer.twitchName}");
            //HostManager.Get.UpdateClientLeaderboards();
            return;
        }
        otp = "";
        twitchName = name.ToLowerInvariant();
        profileImage = tx;

        //podium.avatarRend.material.mainTexture = profileImage;
        /*if(!LobbyManager.Get.lateEntry)
        {
            //podium.InitialisePodium();
        }
        else
        {
            bankedPoints = 0;
            eliminated = true;
        }*/

        HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.Validated, $"{playerName}|<color=#F8A3A3>RISK: {riskPoints.ToString()}</color>\nBANK: {bankedPoints.ToString()}|{twitchName}");
        PlayerManager.Get.players.Add(this);
        PlayerManager.Get.pendingPlayers.Remove(this);
        LeaderboardManager.Get.PlayerHasJoined(this);
        AudioManager.Get.PlayPop();
    }

    public void HandlePlayerScoring(string[] submittedAnswers = null)
    {
        switch(GameplayManager.Get.currentRound)
        {
            case GameplayManager.Round.FavourableOdds:
                int[] pointsArray = new int[4] { 2, 3, 5, 0 };
                int pointsThisQ = pointsArray[QuestionManager.currentQuestionIndex % 4];

                //Wipeout for wrong attempts
                if (!wasCorrect && attemptedQ)
                {
                    DebugLog.Print($"{playerName} wiped out losing {riskPoints} points of risk!", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                    AudioManager.Get.Play(AudioManager.OneShotClip.Boing);
                    strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                    cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                    strap.SetStrapScore(riskPoints, 0, false);
                    cloneStrap.SetStrapScore(riskPoints, 0, false);
                    riskPoints = 0;
                }                    

                else if (wasCorrect && pointsThisQ != 0)
                {
                    DebugLog.Print($"{playerName} was correct!", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
                    AudioManager.Get.Play(AudioManager.OneShotClip.Ding);
                    strap.SetStrapScore(riskPoints, riskPoints + pointsThisQ, false);
                    cloneStrap.SetStrapScore(riskPoints, riskPoints + pointsThisQ, false);
                    strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    riskPoints += pointsThisQ;
                }                    

                else if (wasCorrect && pointsThisQ == 0)
                {
                    DebugLog.Print($"{playerName} doubled up!", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
                    AudioManager.Get.Play(AudioManager.OneShotClip.Ding);
                    strap.SetStrapScore(riskPoints, riskPoints * 2, false);
                    cloneStrap.SetStrapScore(riskPoints, riskPoints * 2, false);
                    strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    riskPoints *= 2;
                }
                HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {riskPoints}</color>\nBank: {bankedPoints}");

                string wereOrWas = QuestionManager.currentQuestionIndex % 4 == 3 ? "answer was" : "answers were";

                if (wasCorrect)
                    HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.SingleAndMultiResult, $"WELL PLAYED!\nThe correct {wereOrWas} {string.Join(", ", GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|CORRECT");
                else 
                    HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.SingleAndMultiResult, $"BAD LUCK!\nThe correct {wereOrWas} {string.Join(", ", GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|INCORRECT");
                break;

            case GameplayManager.Round.TheBoardGame:
                AudioManager.Get.Play(AudioManager.OneShotClip.Ding);

                strap.SetStrapScore(riskPoints, submittedAnswers.Length * 5, false);
                cloneStrap.SetStrapScore(riskPoints, submittedAnswers.Length * 5, false);
                strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
                cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);

                riskPoints = submittedAnswers.Length * 5;

                HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {riskPoints}</color>\nBank: {bankedPoints}");

                List<string> correctAnswers = GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToList();
                wasCorrect = submittedAnswers.All(x => correctAnswers.Contains(x));
                DebugLog.Print(wasCorrect ? $"{playerName} played a safe {submittedAnswers.Length} answers!" : $"{playerName} wiped out!", wasCorrect ? DebugLog.StyleOption.Italic : DebugLog.StyleOption.Bold, wasCorrect ? DebugLog.ColorOption.Green : DebugLog.ColorOption.Red);
                break;

            case GameplayManager.Round.ThisOrThat:

                //Correct
                if (attemptedQ && wasCorrect)
                {
                    DebugLog.Print($"{playerName} earned {pointsForNextQ} points that Q!", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
                    AudioManager.Get.Play(AudioManager.OneShotClip.Ding);
                    strap.SetStrapScore(riskPoints, riskPoints + pointsForNextQ, false);
                    cloneStrap.SetStrapScore(riskPoints, riskPoints + pointsForNextQ, false);
                    strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                    riskPoints += pointsForNextQ;
                    pointsForNextQ++;
                    HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.SingleAndMultiResult, $"WELL PLAYED!\nThe correct answer was {string.Join(", ", GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|CORRECT");
                }
                //Incorrect
                else if (attemptedQ && !wasCorrect)
                {
                    DebugLog.Print($"{playerName} wiped out losing {riskPoints} points of risk!", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                    AudioManager.Get.Play(AudioManager.OneShotClip.Boing);
                    strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                    cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                    HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.SingleAndMultiResult, $"BAD LUCK!\nThe correct answer was {string.Join(", ", GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|INCORRECT");
                }
                HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {riskPoints}</color>\nBank: {bankedPoints}");
                break;
        }
    }

}
