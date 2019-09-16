using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResults : MonoBehaviour
{
    //Hard coded as 3 for max stars per level
    //If changing this, please look carefully through the code

    const int MAX_LEVELS = 80;
    const int NUM_UNLOCKED_LEVELS_AT_START = 3;
    LevelResult[] levelResults = new LevelResult[MAX_LEVELS];
    bool[] starEarned = new bool[MAX_LEVELS * 3];

    [SerializeField] Sprite[] levelResultsSprites = null;  //1st sprite needs to be locked, 2nd is unlocked, 3rd is completed
    [SerializeField] Sprite[] starEarnedSprites = null;    //1st sprite needs to be unearned, 2nd is earned
    [SerializeField] Transform[] levelIconTransforms = null; //The icon holders in the menu system
    [SerializeField] Transform[] starIconTransforms = null;  //The icon holders in the menu system

    private void Start()
    {
        for (int i = 0; i < MAX_LEVELS; i++)
        {
            levelResults[i] = LevelResult.Locked;
        }

        for (int i = 0; i < NUM_UNLOCKED_LEVELS_AT_START; i++)
        {
            levelResults[i] = LevelResult.Unlocked;
        }

        for (int i = 0; i < MAX_LEVELS * 3; i++)
        {
            starEarned[i] = false;
        }

        if(levelResultsSprites.Length != 3)
        {
            Debug.LogWarning("There should be 3 sprites for level results");
        }

        if(starEarnedSprites.Length != 2)
        {
            Debug.LogWarning("There should be 2 sprites for stars earned");
        }
    }

    public LevelResult GetLevelResult(int levelToGetResult)
    {
        if (levelToGetResult >= 1 && levelToGetResult <= MAX_LEVELS)
        {
            return levelResults[levelToGetResult - 1];
        }
        else
        {
            Debug.LogError("Asked for an out of range level number, returning LevelResult.Locked");
            return LevelResult.Locked;
        }
    }

    public bool GetStarEarned(int indexToCheck)
    {
        if (indexToCheck >= 0 && indexToCheck < MAX_LEVELS * 3)
        {
            return starEarned[indexToCheck];
        }
        else
        {
            Debug.LogError("Asked for an out of range index number, returning false");
            return false;
        }
    }

    public void ChangeLevelResult(int levelToChange, LevelResult resultToSet)
    {
        if (levelToChange < 1 || levelToChange > MAX_LEVELS) { return; }  //Improper inputs, no need for this method
        if (GetLevelResult(levelToChange) == resultToSet) { return; } //Nothing is changing, no need for this method

        //********** Level Results ************
        LevelResult currentLevelResult = GetLevelResult(levelToChange);
        levelResults[levelToChange - 1] = resultToSet;
        ChangeLevelResultImageIfNeeded(levelToChange, currentLevelResult, resultToSet);
        
        //********** Stars Earned *************
        StarImages currentStarEarned = new StarImages(starEarned[3 * levelToChange - 3],
                                                      starEarned[3 * levelToChange - 2],
                                                      starEarned[3 * levelToChange - 1]);
        SetNewStarsEarnedArray(levelToChange, resultToSet);
        ChangeStarImagesIfNeeded(levelToChange, currentStarEarned, resultToSet);
    }

    private void ChangeLevelResultImageIfNeeded(int levelToChange, LevelResult currentLevelResult, LevelResult resultToSet)
    {
        if (DoesLevelImageNeedChanging(currentLevelResult, resultToSet))
        {
            ChangeLevelResultImage(levelToChange, resultToSet);
        }
    }

    static private bool DoesLevelImageNeedChanging(LevelResult currentLevelResult, LevelResult resultToSet)
    {
        if (currentLevelResult == LevelResult.Locked || currentLevelResult == LevelResult.Unlocked)
        {
            return true; //Any change in state means the image has to change
        } else //One of the Done states
        {
            if (resultToSet == LevelResult.Locked || resultToSet == LevelResult.Unlocked)
            {
                return true; //This is in case of a reset feature
            }
            else 
            { 
                return false; //One of the other Done states, so the image stays the same
            }
        }
    }

    private void ChangeLevelResultImage(int levelToChange, LevelResult resultToSet)
    {
        switch(resultToSet)
        {
            case LevelResult.Locked:
                levelIconTransforms[levelToChange - 1].GetComponent<SpriteRenderer>().sprite = levelResultsSprites[0];
                return;
            case LevelResult.Unlocked:
                levelIconTransforms[levelToChange - 1].GetComponent<SpriteRenderer>().sprite = levelResultsSprites[1];
                return;
            case LevelResult.Done1Star:
                levelIconTransforms[levelToChange - 1].GetComponent<SpriteRenderer>().sprite = levelResultsSprites[2];
                return;
            case LevelResult.Done2Star:
                levelIconTransforms[levelToChange - 1].GetComponent<SpriteRenderer>().sprite = levelResultsSprites[2];
                return;
            case LevelResult.Done3Star:
                levelIconTransforms[levelToChange - 1].GetComponent<SpriteRenderer>().sprite = levelResultsSprites[2];
                return;
            default:
                return;
        }
    }

    private void SetNewStarsEarnedArray(int levelToChange, LevelResult resultToSet)
    {
        switch (resultToSet)
        {
            case LevelResult.Locked:
                starEarned[3 * levelToChange - 3] = false;
                starEarned[3 * levelToChange - 2] = false;
                starEarned[3 * levelToChange - 1] = false;
                return;
            case LevelResult.Unlocked:
                starEarned[3 * levelToChange - 3] = false;
                starEarned[3 * levelToChange - 2] = false;
                starEarned[3 * levelToChange - 1] = false;
                return;
            case LevelResult.Done1Star:
                starEarned[3 * levelToChange - 3] = true;
                starEarned[3 * levelToChange - 2] = false;
                starEarned[3 * levelToChange - 1] = false;
                return;
            case LevelResult.Done2Star:
                starEarned[3 * levelToChange - 3] = true;
                starEarned[3 * levelToChange - 2] = true;
                starEarned[3 * levelToChange - 1] = false;
                return;
            case LevelResult.Done3Star:
                starEarned[3 * levelToChange - 3] = true;
                starEarned[3 * levelToChange - 2] = true;
                starEarned[3 * levelToChange - 1] = true;
                return;
            default:
                return;
        }
    }

    private void ChangeStarImagesIfNeeded(int levelToChange, StarImages currentStarEarned, LevelResult resultToSet)
    {
        //*** First star ***
        if(currentStarEarned.starImage1 != starEarned[3 * levelToChange - 3])
        {
            switch (resultToSet)
            {
                case LevelResult.Locked:
                    ChangeStarImage(3 * levelToChange - 3, false);
                    break;
                case LevelResult.Unlocked:
                    ChangeStarImage(3 * levelToChange - 3, false);
                    break;
                case LevelResult.Done1Star:
                    ChangeStarImage(3 * levelToChange - 3, true);
                    break;
                case LevelResult.Done2Star:
                    ChangeStarImage(3 * levelToChange - 3, true);
                    break;
                case LevelResult.Done3Star:
                    ChangeStarImage(3 * levelToChange - 3, true);
                    break;
                default:
                    break;
            }
        }

        //*** Second star ***
        if (currentStarEarned.starImage2 != starEarned[3 * levelToChange - 2])
        {
            switch (resultToSet)
            {
                case LevelResult.Locked:
                    ChangeStarImage(3 * levelToChange - 2, false);
                    break;
                case LevelResult.Unlocked:
                    ChangeStarImage(3 * levelToChange - 2, false);
                    break;
                case LevelResult.Done1Star:
                    ChangeStarImage(3 * levelToChange - 2, false);
                    break;
                case LevelResult.Done2Star:
                    ChangeStarImage(3 * levelToChange - 2, true);
                    break;
                case LevelResult.Done3Star:
                    ChangeStarImage(3 * levelToChange - 2, true);
                    break;
                default:
                    break;
            }
        }

        //*** Third star ***
        if (currentStarEarned.starImage3 != starEarned[3 * levelToChange - 1])
        {
            switch (resultToSet)
            {
                case LevelResult.Locked:
                    ChangeStarImage(3 * levelToChange - 1, false);
                    break;
                case LevelResult.Unlocked:
                    ChangeStarImage(3 * levelToChange - 1, false);
                    break;
                case LevelResult.Done1Star:
                    ChangeStarImage(3 * levelToChange - 1, false);
                    break;
                case LevelResult.Done2Star:
                    ChangeStarImage(3 * levelToChange - 1, false);
                    break;
                case LevelResult.Done3Star:
                    ChangeStarImage(3 * levelToChange - 1, true);
                    break;
                default:
                    break;
            }
        }
    }

    private void ChangeStarImage(int starToChange, bool isEarned)
    {
        if(isEarned)
        {
            starIconTransforms[starToChange].GetComponent<SpriteRenderer>().sprite = starEarnedSprites[1];
        } else
        {
            starIconTransforms[starToChange].GetComponent<SpriteRenderer>().sprite = starEarnedSprites[0];
        }
    }
}

public struct StarImages
{
    public bool starImage1;
    public bool starImage2;
    public bool starImage3;

    public StarImages(bool starImages1, bool starImages2, bool starImages3)
    {
        starImage1 = starImages1;
        starImage2 = starImages2;
        starImage3 = starImages3;
    }
}

public enum LevelResult { Locked, Unlocked, Done1Star, Done2Star, Done3Star }