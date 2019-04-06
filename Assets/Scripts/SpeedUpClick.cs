using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class SpeedUpClick : MonoBehaviour
	{
		public void OnMouseUp()
		{
			FindObjectOfType<Timer>().SwitchSpeed();
		}
	}
}