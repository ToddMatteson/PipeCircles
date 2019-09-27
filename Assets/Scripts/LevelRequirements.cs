using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
    public class LevelRequirements : MonoBehaviour
    {
        //This is mainly a helper class for RoundOver to reduce clutter

        const int MAX_NUM_LEVELS = 80;
        const int MAX_REQUIREMENT = 4;

        int tempScoreVariable = 200;
        int tempLoopsVariable = 4;
        bool tempEndAtTheEndVariable = true;
        int tempSpecialFeaturesAchieved = 3;

        public bool AreAllLevelRequirementsMet(int levelToCheck)
        {
            if (levelToCheck < 1 || levelToCheck > MAX_NUM_LEVELS)
            {
                Debug.LogWarning("Requested level does not exist");
                return false;
            }

            bool requirementsMet = true;
            for (int i = 0; i < MAX_REQUIREMENT; i++)
            {
                requirementsMet = requirementsMet && IsLevelRequirementMet(levelToCheck, i + 1);
            }
            return requirementsMet;

        }

        public bool IsLevelRequirementMet(int level, int requirement)
        {
            if (level < 1 || level > MAX_NUM_LEVELS)
            {
                Debug.LogWarning("Invalid level passed in");
                return false;
            }

            if (requirement < 1 || requirement > MAX_REQUIREMENT)
            {
                Debug.LogWarning("Invalid requirement passed in");
                return false;
            }

            switch (level)
            {
                case 1:
                    switch (requirement)
                    {
                        case 1:
                            return false; //(tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 2:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 3:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 4:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 5:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 6:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 7:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 8:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 9:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 10:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 11:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 12:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 13:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 14:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 15:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 16:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 17:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 18:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 19:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 20:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 21:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 22:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 23:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 24:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 25:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 26:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 27:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 28:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 29:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 30:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 31:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 32:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 33:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 34:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 35:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 36:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 37:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 38:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 39:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 40:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 41:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 42:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 43:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 44:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 45:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 46:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 47:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 48:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 49:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 50:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 51:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 52:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 53:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 54:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 55:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 56:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 57:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 58:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 59:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 60:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 61:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 62:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 63:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 64:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 65:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 66:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 67:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 68:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 69:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 70:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 71:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 72:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 73:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 74:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 75:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 76:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 77:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 78:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 79:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                case 80:
                    switch (requirement)
                    {
                        case 1:
                            return (tempScoreVariable > 10);
                        case 2:
                            return (tempLoopsVariable > 1);
                        case 3:
                            return tempEndAtTheEndVariable;
                        case 4:
                            return (tempSpecialFeaturesAchieved > 1);
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool DoesLevelRequirementExist(int level, int requirement)
        {
            if (level < 1 || level > MAX_NUM_LEVELS)
            {
                Debug.LogWarning("Invalid level passed in");
                return false;
            }

            if (requirement < 1 || requirement > MAX_REQUIREMENT)
            {
                Debug.LogWarning("Invalid requirement passed in");
                return false;
            }

            switch (level)
            {
                case 1:
                    switch (requirement)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        default:
                            return false;
                    }
                case 2:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 3:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 4:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 5:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 6:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 7:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 8:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 9:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 10:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 11:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 12:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 13:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 14:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 15:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 16:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 17:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 18:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 19:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 20:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 21:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 22:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 23:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 24:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 25:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 26:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 27:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 28:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 29:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 30:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 31:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 32:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 33:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 34:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 35:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 36:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 37:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 38:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 39:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 40:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 41:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 42:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 43:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 44:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 45:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 46:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 47:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 48:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 49:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 50:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 51:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 52:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 53:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 54:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 55:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 56:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 57:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 58:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 59:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 60:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 61:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 62:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 63:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 64:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 65:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 66:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 67:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 68:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 69:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 70:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 71:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 72:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 73:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 74:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 75:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 76:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 77:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 78:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 79:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                case 80:
                    switch (requirement)
                    {
                        case 1:
                            return true;
                        case 2:
                            return true;
                        case 3:
                            return true;
                        case 4:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;

            }
        }
    }
}
