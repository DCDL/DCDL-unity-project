using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PlayerInSet
{
    public string playerId { get; set; }
    public string response { get; set; }
    public string victory { get; set; }
    public string score { get; set; }
}

public class Set
{
    public string setId { get; set; }
    public string date { get; set; }
    public string mode { get; set; }
    public string question { get; set; }
    public string finished { get; set; }
    public List<PlayerInSet> playersInSet { get; set; }
}

public class Action
{
    public string playerId { get; set; }
    public string setId { get; set; }
    public string response { get; set; }
}

public class RoomAPI
{
    public List<string> playerIds { get; set; }
    public string roomId { get; set; }
}

public class PlayerAPI
{
    public string playerId { get; set; }
    public string playerName { get; set; }
    public string ELO { get; set; }
    public string gameCount { get; set; }
    public string victoriesCount { get; set; }
}