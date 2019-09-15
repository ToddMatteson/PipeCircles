using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResults : MonoBehaviour
{
    const int maxLevels = 80;
    LevelResult[] levelResults = new LevelResult[maxLevels];

    private void Start()
    {
        for (int i = 0; i < maxLevels; i++)
        {
            levelResults[i] = LevelResult.Locked;
        }

        for (int i = 0; i < 3; i++)
        {
            levelResults[i] = LevelResult.Unlocked;
        }
    }

    public void ChangeLevelResult(int levelToChange, LevelResult levelToSet)
    {
        if (levelToChange >= 1 && levelToChange <= maxLevels)
        {
            levelResults[levelToChange - 1] = levelToSet;
        }
    }

    public LevelResult GetLevelResult(int levelToGetResult)
    {
        if (levelToGetResult >= 1 && levelToGetResult <= maxLevels)
        {
            return levelResults[levelToGetResult - 1];
        } else
        {
            Debug.LogError("Asked for an out of range level number, returning LevelResult.Locked");
            return LevelResult.Locked;
        }
    }
}

public enum LevelResult { Locked, Unlocked, Done1Star, Done2Star, Done3Star }
