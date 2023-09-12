using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;
using NaughtyAttributes;
using System.Linq;
using TMPro;

public class GameplayPennys : SingletonMonoBehaviour<GameplayPennys>
{

    private PlayerShell playerList;
    private MedalTableObject medalList;
    private List<LeaderboardObject> leaderboardList;
    private readonly string path = @"D:\Unity Projects\HerokuPennyData\PennyStorage";

    public int authorPennys;
    [Range (1, 20)] public int multiplyFactor;
    public string gameName;


    [Button]
    public void UpdatePennysAndMedals()
    {
        AwardPennys();
        if(!string.IsNullOrEmpty(gameName))
            AwardMedals();
        WriteNewFile();
    }

    private void LoadJSON()
    {
        playerList = JsonConvert.DeserializeObject<PlayerShell>(File.ReadAllText(path + @"\NewPennys.txt"));
    }

    private void LoadMedalJSON()
    {
        medalList = JsonConvert.DeserializeObject<MedalTableObject>(File.ReadAllText(path + $@"\{gameName}.txt"));
    }

    private void LoadLeaderboardJSON()
    {
        leaderboardList = JsonConvert.DeserializeObject<List<LeaderboardObject>>(File.ReadAllText(path + $@"\{gameName}Leaderboard.txt"));
    }

    private void AwardPennys()
    {
        List<PlayerObject> list = PlayerManager.Get.players.OrderBy(p => p.twitchName).Where(x => x.bankedPoints > 0).ToList();
        PlayerPennyData ppd;

        LoadJSON();
        foreach (PlayerObject p in list)
        {
            ppd = playerList.playerList.FirstOrDefault(x => x.PlayerName.ToLowerInvariant() == p.twitchName.ToLowerInvariant());
            if (ppd == null)
                CreateNewPlayer(p);
            else
            {
                ppd.CurrentSeasonPennys += (p.bankedPoints * multiplyFactor);
                ppd.AllTimePennys += (p.bankedPoints * multiplyFactor);
            }
        }

        ppd = null;
        ppd = playerList.playerList.FirstOrDefault(x => x.PlayerName.ToLowerInvariant() == QuestionManager.currentPack.author.ToLowerInvariant());
        if (ppd == null)
            CreateNewAuthor(QuestionManager.currentPack.author.ToLowerInvariant());
        else
        {
            ppd.CurrentSeasonPennys += authorPennys;
            ppd.AllTimePennys += authorPennys;
            ppd.AuthorCredits++;
        }
    }

    private void AwardMedals()
    {
        List<PlayerObject> orderedByPoints = PlayerManager.Get.players.OrderByDescending(x => x.bankedPoints).ThenBy(x => x.twitchName).ToList();
        LoadMedalJSON();
        int medalToAward = 0;

        for(int i = 0; i < orderedByPoints.Count; i++)
        {
            if (i == 0)
                medalList.goldMedallists.Add(orderedByPoints[i].twitchName);

            else
            {
                if (orderedByPoints[i].bankedPoints != orderedByPoints[i - 1].bankedPoints)
                    medalToAward++;

                if (medalToAward == 3)
                    break;

                else
                {
                    switch(medalToAward)
                    {
                        case 0:
                            medalList.goldMedallists.Add(orderedByPoints[i].twitchName);
                            break;

                        case 1:
                            medalList.silverMedallists.Add(orderedByPoints[i].twitchName);
                            break;

                        case 2:
                            medalList.bronzeMedallists.Add(orderedByPoints[i].twitchName);
                            break;
                    }
                }
            }
        }
    }

    private void CreateNewPlayer(PlayerObject p)
    {
        PlayerPennyData newP = new PlayerPennyData()
        {
            PlayerName = p.twitchName.ToLowerInvariant(),
            CurrentSeasonPennys = (p.bankedPoints * multiplyFactor),
            AllTimePennys = (p.bankedPoints * multiplyFactor)
        };
        playerList.playerList.Add(newP);
    }

    private void CreateNewAuthor(string p)
    {
        PlayerPennyData newP = new PlayerPennyData()
        {
            PlayerName = p,
            CurrentSeasonPennys = authorPennys,
            AllTimePennys = authorPennys,
            AuthorCredits = 1
        };
        playerList.playerList.Add(newP);
    }

    private void WriteNewFile()
    {
        string pennyPath = Operator.Get.testMode ? path + @"\NewPennysTest.txt" : path + @"\NewPennys.txt";
        string medalPath = Operator.Get.testMode ? path + $@"\{gameName}Test.txt" : path + $@"\{gameName}.txt";

        string newDataContent = JsonConvert.SerializeObject(playerList);
        File.WriteAllText(pennyPath, newDataContent);

        if (!string.IsNullOrEmpty(gameName))
        {
            newDataContent = JsonConvert.SerializeObject(medalList);
            File.WriteAllText(medalPath, newDataContent);
        }

        if (Operator.Get.testMode)
            DebugLog.Print("TEST DATA WRITTEN TO DRIVE", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Orange);
        else
            DebugLog.Print("DATA WRITTEN TO DRIVE", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Green);
    }
}
