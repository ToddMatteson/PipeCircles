using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PipeCircles
{
	public class Timer : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI timerText;
		[SerializeField] float timePenalty = 3f;
		[SerializeField] float startingTime = 40f;

		float timeRemaining;
		bool timerRunning = false;
		bool speedUp = false;

		private void Awake()
		{
			SingletonPattern();
		}

		private void SingletonPattern()
		{
			int classCount = FindObjectsOfType<Timer>().Length;
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
			ResetTimer();
		}

		void Update()
		{
			StartTimer();
			UpdateTimerText();
		}

		public void ResetTimer()
		{
			timeRemaining = startingTime;
		}

		public void StartTimer()
		{
			timerRunning = true;
		}

		public void StopTimer()
		{
			timerRunning = false;
		}

		private void UpdateTimerText()
		{
			if (timerRunning && timeRemaining > 0)
			{
				if (speedUp)
				{
					timeRemaining -= Time.deltaTime * 3;
				} else
				{
					timeRemaining -= Time.deltaTime;
				}
			}
			timerText.text = Mathf.Round(timeRemaining).ToString();
		}

		public void ApplyTimerPenalty()
		{
			timeRemaining = Mathf.Max(timeRemaining - timePenalty,0);
		}

		public void SwitchSpeed()
		{
			speedUp = !speedUp;
		}

		public float GetTimeRemaining()
		{
			return timeRemaining;
		}
	}
}
