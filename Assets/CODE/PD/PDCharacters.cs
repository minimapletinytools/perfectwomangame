using UnityEngine;
using System.Collections;

public class PDCharacters
{
    public static PDCharacterStats[] characters = new PDCharacterStats[]
    {
        new PDCharacterStats() //FETUS 0-0
        {
            Title = "Fetus"
        },   
        new PDCharacterStats()      //05-1
        {
            EDUCATION = 0,
            EXPRESSION = 5,
            FAMILY = 0,
            HEALTH = -1,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 5,
            WISDOM = 0,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Princess"
        },
        new PDCharacterStats()      //05-2
        {
            EDUCATION = 0,
            EXPRESSION = 100,
            FAMILY = -5,
            HEALTH = -100,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = -5,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Child Star"
        },
        new PDCharacterStats()      //05-3
        {
            EDUCATION = -100,
            EXPRESSION = 5,
            FAMILY = -5,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 100,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Street Kid"
        },
        new PDCharacterStats()      //05-4
        {
            EDUCATION = 5,
            EXPRESSION = 5,
            FAMILY = 100,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 5,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "1 of 12"
        },
        new PDCharacterStats()      //16-1
        {
            EDUCATION = -100,
            EXPRESSION = -5,
            FAMILY = 5,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = -5,
            ROMANCE = 0,
            WISDOM = 100,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Responsible Sister"
        },
        new PDCharacterStats()      //16-2
        {
            EDUCATION = 100,
            EXPRESSION = 5,
            FAMILY = -5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 0,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Nerd"
        },
        new PDCharacterStats()      //16-3
        {
            EDUCATION = 0,
            EXPRESSION = 5,
            FAMILY = 0,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 0,

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Sexy Lady"
        },
        new PDCharacterStats()      //16-4
        {
            EDUCATION = -5,
            EXPRESSION = 100,
            FAMILY = 0,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Band Member"
        },
        new PDCharacterStats()      //27-1
        {
            EDUCATION = 5,
            EXPRESSION = 0,
            FAMILY = 100,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Mother"
        },
        new PDCharacterStats()      //27-2
        {
            EDUCATION = 100,
            EXPRESSION = 5,
            FAMILY = -100,
            HEALTH = -100,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = -5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Drug User"
        },
        new PDCharacterStats()      //27-3
        {
            EDUCATION = -100,
            EXPRESSION = 5,
            FAMILY = 0,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = -5,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Oktoberfest Bartender"
        },
        new PDCharacterStats()      //27-4
        {
            EDUCATION = -100,
            EXPRESSION = 0,
            FAMILY = -5,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Pot Head"
        },
        new PDCharacterStats()      //34-1
        {
            EDUCATION = -5,
            EXPRESSION = -5,
            FAMILY = 100,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Soccer Mom in Danville"
        },
        new PDCharacterStats()      //34-2
        {
            EDUCATION = 100,
            EXPRESSION = 5,
            FAMILY = -100,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 100,
            ROMANCE = 0,
            WISDOM = -100,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "MIT Professor giving a TED talk"
        },
        new PDCharacterStats()      //34-3
        {
            EDUCATION = 0,
            EXPRESSION = -100,
            FAMILY = -5,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = 100,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Sad Person"
        },
        new PDCharacterStats()      //34-4
        {
            EDUCATION = -5,
            EXPRESSION = 100,
            FAMILY = 100,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Foster Parent in Costa Rica"
        },
        new PDCharacterStats()      //45-1
        {
            EDUCATION = 5,
            EXPRESSION = 5,
            FAMILY = 5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = -5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Rich Married Woman"
        },
        new PDCharacterStats()      //45-2
        {
            EDUCATION = 0,
            EXPRESSION = -5,
            FAMILY = -5,
            HEALTH = -100,
            MONEY = 0,
            RESPECT = -5,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Stressed Out Lady"
        },
        new PDCharacterStats()      //45-3
        {
            EDUCATION = 5,
            EXPRESSION = 100,
            FAMILY = -5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 100,
            ROMANCE = 0,
            WISDOM = 5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Famosu Actress"
        },
        new PDCharacterStats()      //45-4
        {
            EDUCATION = 0,
            EXPRESSION = 100,
            FAMILY = 5,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Belly Dance Instructor"
        },
        new PDCharacterStats()      //60-1
        {
            EDUCATION = -5,
            EXPRESSION = -5,
            FAMILY = 0,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Upset Mother"
        },
        new PDCharacterStats()      //60-2
        {
            EDUCATION = 100,
            EXPRESSION = 5,
            FAMILY = -100,
            HEALTH = -5,
            MONEY = 0,
            RESPECT = 100,
            ROMANCE = 0,
            WISDOM = 5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Foreign Minister"
        },
        new PDCharacterStats()      //60-3
        {
            EDUCATION = -5,
            EXPRESSION = 5,
            FAMILY = 5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 5,
            ROMANCE = 0,
            WISDOM = -5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Charity Speaker"
        },
        new PDCharacterStats()      //60-4
        {
            EDUCATION = 5,
            EXPRESSION = 5,
            FAMILY = 5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 5,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Intellectual"
        },
        new PDCharacterStats()      //80-1
        {
            EDUCATION = -5,
            EXPRESSION = 5,
            FAMILY = 5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = 0,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Follower of God"
        },
        new PDCharacterStats()      //80-2
        {
            EDUCATION = 0,
            EXPRESSION = 100,
            FAMILY = -100,
            HEALTH = -100,
            MONEY = 0,
            RESPECT = -100,
            ROMANCE = 0,
            WISDOM = -100,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Crazy Cat Lady"
        },
        new PDCharacterStats()      //80-3
        {
            EDUCATION = -100,
            EXPRESSION = 0,
            FAMILY = 5,
            HEALTH = 0,
            MONEY = 0,
            RESPECT = -5,
            ROMANCE = 0,
            WISDOM = 0,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Senile Senior"
        },
        new PDCharacterStats()      //80-4
        {
            EDUCATION = 5,
            EXPRESSION = 5,
            FAMILY = 100,
            HEALTH = 5,
            MONEY = 0,
            RESPECT = 100,
            ROMANCE = 0,
            WISDOM = 100,
            
            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0),
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0),
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0),
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0),
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0),
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0),
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0),
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0),

            Title = "Wise Grandma"
        },
        new PDCharacterStats()      //999
        {

            Title = "Burried"
        }

    };
}
