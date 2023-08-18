﻿using Control;
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
    public int totalCorrect;
    public string submission;
    public float submissionTime;
    public bool flagForCondone;
    public bool wasCorrect;

    public PlayerObject(Player pl, string name)
    {
        playerClientRef = pl;
        otp = OTPGenerator.GenerateOTP();
        playerName = name;
        bankedPoints = 0;
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

            PlayerManager.Get.players.Remove(this);
            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.Validated, $"{oldPlayer.playerName}|{oldPlayer.bankedPoints.ToString()}");
            HostManager.Get.UpdateClientLeaderboards();
            return;
        }
        otp = "";
        twitchName = name.ToLowerInvariant();
        profileImage = tx;
        //podium.avatarRend.material.mainTexture = profileImage;
        if(!LobbyManager.Get.lateEntry)
        {

        }
            //podium.InitialisePodium();
        else
        {
            bankedPoints = 0;
            eliminated = true;
        }
        HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.Validated, $"{playerName}|{bankedPoints.ToString()}|{riskPoints.ToString()}");
        HostManager.Get.UpdateClientLeaderboards();
    }
}
