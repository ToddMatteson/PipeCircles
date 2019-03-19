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
		[SerializeField] float timeRemaining = 40f;

		bool timerRunning = false;
		bool speedUp = false;

		void Update()
		{
			StartTimer();
			UpdateTimerText();
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

		public void StartTimer()
		{
			timerRunning = true;
		}

		public void StopTimer()
		{
			timerRunning = false;
		}

		public void ApplyTimerPenalty()
		{
			timeRemaining = Mathf.Max(timeRemaining - timePenalty,0);
		}

		public void SwitchSpeed()
		{
			speedUp = !speedUp;
		}
	}
}
