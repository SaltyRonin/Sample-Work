using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Data
{
    public static GameController GameController { get; set; }
    public static ShipBehavior Player { get; set; }

    public static int LevelScore { get; set; }
    public static int TotalScore { get; set; }

    public static int Level { get; set; }
    public static string LevelName
    {
        get
        {
            switch(Level)
            {
                case 0: return "Briefing";
                case 1: return "Level1";
                case 2: return "Briefing";
                case 3: return "Level2";
                case 4: return "Level3";
                default: return "Menu";
            }
        }
    }

    public static int ShipSelection { get; set; }
    public static string ShipName
    {
        get
        {
            switch (ShipSelection)
            {
                case 0: return "modern_ship";
                case 1: return "classic_ship";
                case 2: return "retro_ship";
                default: return "";
            }
        }
    }

    public static bool Retry { get; set; }
    public static string PlayerName { get; set; }
    public static string One { get; set; }
    public static string Two { get; set; }

    public static int EventLength { get { return dialog[Level].Length; } }

    public static void Reset()
    {
        ShipSelection = 0;
        LevelScore = 0;
        TotalScore = 0;
        Level = 0;
        Retry = false;
        PlayerName = "Player";
        One = "Kate";
        Two = "Dan";
    }

    public static void LoadNextLevel()
    {
        TotalScore += LevelScore;
        LevelScore = 0;
        Retry = false;
        Level++;
        SceneManager.LoadScene(LevelName);
    }

    public static void RestartLevel()
    {
        LevelScore = 0;
        Retry = true;
        SceneManager.LoadScene(LevelName);
    }

    public enum Name
    {
        Player,
        One,
        Two,
        Commander,
        // Memelord,
        Error
    }

    public struct DialogEvent
    {
        public Name Character;
        public string Name;
        public string Text;
        public float Time;

        public DialogEvent(Name n, string text, float time = 2f)
        {
            Character = n;
            Name = "";
            Text = text;
            Time = time;
        }
        public DialogEvent(string n, string text, float time)
        {
            Character = Data.Name.Error;
            Name = n;
            Text = text;
            Time = time;
        }
    }

    private static DialogEvent[][] dialog = new DialogEvent[][]
    {
        // Mission 1 Breifing
        new DialogEvent[]
        { 
            new DialogEvent(Name.Commander, "Our world is being overrun with those not-so-funny Internet picture things!<pause=0.5> The uh…<pause=0.5> <size=16><speed=0.07>what do you call them...</speed></size>", 1f),
            new DialogEvent(Name.One, "They're called memes, Commander.", 1f),
            new DialogEvent(Name.Commander, "Is that how you say it?<pause=0.5> Meme?<pause=1> <size=16><speed=0.07>I always thought it was me-me</speed><speed=0.05>...</speed></size><pause=0.5> Anyway<pause=0.25>, they’ve somehow escaped the Internet and are attacking our cities left and right!<pause=0.5> They must be stopped!", 1f),
            new DialogEvent(Name.Player, "You can count on me, Commander!", 1f),
            new DialogEvent(Name.Commander, "You're our top pilot, so if anyone can stop them, it's you.<pause=0.5> Our two best Internet analysts are working on locating the source of the memes as we speak.", 1f),
            new DialogEvent(Name.Two, "We'll keep you up to date with any information you need during your mission.", 1f),
            new DialogEvent(Name.Player, "Let's get to work!", 1f),
        },

        // Mission 1
        new DialogEvent[]
        {
            new DialogEvent(Name.One, "Look out for collapsing buildings! Press L and R trigger to brake and boost.", 3f),
            new DialogEvent(Name.Two, "There's a meme! Shoot it with A!", 2f),
            new DialogEvent(Name.One, "Grab that laser upgrade!", 2f),
            new DialogEvent(Name.Two, "Don't get too close to those, they're like mines!", 3f),
            new DialogEvent(Name.Player, "What was that?", 1f),
            new DialogEvent(Name.One, "Uh oh...", 1f),
            new DialogEvent(Name.Two, "Quick! Do a barrel roll! (Double-press L or R)", 3f),
            new DialogEvent(Name.Player, "Looks like the city is clear.", 2f),
        },

        // Mission 2 Breifing
        new DialogEvent[]
        {
            new DialogEvent(Name.Commander, "We've received new intel about the source of the memes.", 1f),
            new DialogEvent(Name.One, "We picked up a strange reading coming from just outside the atmosphere.", 1f),
            new DialogEvent(Name.Two, "It seems the memes are coming out of some kind of energy anomaly.", 1f),
            new DialogEvent(Name.Commander, "Get up there and see what you can find!", 1f),
            new DialogEvent(Name.Player, "Yes sir!", 1f),
        },

        // Mission 2
        new DialogEvent[]
        {
            new DialogEvent(Name.One, "Look around for anything suspicious.", 2f),
            new DialogEvent(Name.Two, "Watch out!", 1f),
            new DialogEvent(Name.One, "We must be getting close. The memes concentration is increasing!", 2f),
            new DialogEvent(Name.Two, "That must be the anomaly! Is that a portal to the Internet?", 2f),
            new DialogEvent(Name.Player, "I'm going in!", 1f),
        },

        // Mission 3
        new DialogEvent[]
        {
            new DialogEvent(Name.Player, "I made it!<pause=1.5> Guys?<pause=1.5> Guess I'm on my own this time.<pause=0.5> And the Internet made me all pixelated..."),
            new DialogEvent(Name.Player, "Guess we won't be seeing any memes anymore!"),
        },
    };

	public static DialogEvent GetDialog(int index)
    {
        if(index >= dialog[Level].Length)
        {
            return new DialogEvent("ERROR", string.Format("INVALID DIALOG EVENT\nLEVEL: {0} INDEX: {1}", Level, index), 5f);
        }

        DialogEvent e = dialog[Level][index];

        switch(e.Character)
        {
            case Name.Player:
                e.Name = PlayerName.ToUpper();
                break;
            case Name.One:
                e.Name = One.ToUpper();
                break;
            case Name.Two:
                e.Name = Two.ToUpper();
                break;
            case Name.Commander:
                e.Name = "COMMANDER";
                break;
            //case Name.Memelord:
                //e.Name = "MEMELORD";
                //break;
        }

        return e;
    }
}
