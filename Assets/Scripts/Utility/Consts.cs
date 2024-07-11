using System;

public class Consts
{
    public const float BurstRadius = 1f;
    public const float BurstWallRadius = 5f;
    public const float CurrentRadius = .5f;
    public const float CurrentDist = 2f;
    public const float ImmunizeRadius = .75f;

    // names
    public const string PeopleGo = "People";
    public const string EdgesGo = "Edges";
    public const string ScoreGo = "Score";

    // time
    public const float minSickSec = 20;
    public const float maxSickSec = 40;
    public const float immunityCooldownSec = 20;

    public const float expandDuration = .03f;

    public const float sicknessRecoverySec = 1;
    //public const float immunityCooldownSec = 3;
    //public const float minPressDuration = 2;

    // probabilities
    public const float contagionProb = .07f;
    public const float immunityProb = .25f;
    public const float fatalityProb = .7f;
    public const float spreadImmunityProb = .2f;

    // tags
    public const string PersonTag = "Person";
    public const string RoomTag = "Room";
}

