using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PipeCircles
{
	public class Scoring : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI scoreText = null;
		[SerializeField] TextMeshProUGUI loopsText = null;

		[SerializeField] int loopScore = 100;
		[SerializeField] int loopTierBonus = 200;
		[SerializeField] int pieceTraveledScore = 20;
		[SerializeField] int specialItemScore = 50;
		[SerializeField] int endReachedScore = 100;
		[SerializeField] int unusedPieceScore = -10;

		int levelScore = 0;
		int totalScore = 0;
		int levelLoops = 0;
		int totalLoops = 0;

		private void Awake()
		{
			SingletonPattern();
		}

		private void SingletonPattern()
		{
			int classCount = FindObjectsOfType<Scoring>().Length;
			if (classCount > 1)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			} else
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		private void Start()
		{
			UpdateScoreAndLoopsText();
		}

		public int GetLevelScore() { return levelScore; }

		public int GetTotalScore() { return totalScore; }

		public void AddToLevelScore(int scoreToAdd)
		{
			if (scoreToAdd < 0)
			{
				levelScore -= Mathf.Min(levelScore,-scoreToAdd);
			} else
			{
				levelScore += scoreToAdd;
			}

			UpdateScoreAndLoopsText();
		}

		private void UpdateScoreAndLoopsText()
		{
			scoreText.text = levelScore.ToString();
			loopsText.text = levelLoops.ToString();
		}

		public void AddToTotalScore(int scoreToAdd)
		{
			if (scoreToAdd < 0)
			{
				totalScore -= Mathf.Min(totalScore,-scoreToAdd);
			} else
			{
				totalScore += scoreToAdd;
			}
		}

		public void LoopCompleted()
		{
			levelLoops++;
			totalLoops++;
			AddToLevelScore(loopScore);
			AddToTotalScore(loopScore);

			ApplyLoopTierBonus();
		}

		private void ApplyLoopTierBonus()
		{
			if (levelLoops > 0 && levelLoops % 4 == 0)
			{
				AddToLevelScore(loopTierBonus);
				AddToTotalScore(loopTierBonus);
			}
		}

		public void PieceTraveled()
		{
			AddToLevelScore(pieceTraveledScore);
			AddToTotalScore(pieceTraveledScore);
		}

		public void SpecialItemUsed()
		{
			AddToLevelScore(specialItemScore);
			AddToTotalScore(specialItemScore);
		}

		public void EndReached()
		{
			AddToLevelScore(endReachedScore);
			AddToTotalScore(endReachedScore);
		}

		public void UnusedPiecePenalty()
		{
			AddToLevelScore(unusedPieceScore);
			AddToTotalScore(unusedPieceScore);
		}
	}
}
