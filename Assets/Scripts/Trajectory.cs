using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class Trajectory : MonoBehaviour
	{
		public ThetaReturn FindLaunchAngleToTarget(Vector2 startPos, Vector2 endPos, float launchSpeed, float g)
		{
			float v = launchSpeed;

			//Wikipedia formula below requires a starting position of (0, 0) so need to offset vectors
			Vector2 endingPos = endPos - startPos;

			//Calculating the angle theta required to hit coordinate(x, y) when starting at (0,0)
			//theta = arctan[(v^2 +- sqrt[v^4 - x^2 * g^2 - 2 * v^2 * y * g)]) / (x * g)]
			float sqrtPortion1 = v * v * v * v;
			float sqrtPortion2 = endingPos.x * endingPos.x * g * g;
			float sqrtPortion3 = 2f * v * v * endingPos.y * g;
			float sqrtCombined = sqrtPortion1 - sqrtPortion2 - sqrtPortion3;

			float sqrtPortion;
			if (sqrtCombined < 0)
			{
				return new ThetaReturn(false, 0, 0); //Negative square roots are a no-go
			} else
			{
				sqrtPortion = Mathf.Sqrt(sqrtCombined);
			}

			float theta1 = Mathf.Atan((v * v + sqrtPortion) / (endingPos.x * g));
			float theta2 = Mathf.Atan((v * v - sqrtPortion) / (endingPos.x * g));
			ThetaReturn answer = new ThetaReturn(true, theta1, theta2);
			return answer;
		}

		public float FindTotalFlightTime(Vector2 startPos, Vector2 endPos, float launchAngle, float launchSpeed, float g)
		{
			//Using the formula found on the wikipedia page on projectile motion
			//t = (1 / g) * [vSinTheta + sqrt( [vSinTheta]^2 + 2gy0)]
			//where y0 is the amount the starting point is above the ending point

			float y0 = startPos.y - endPos.y;
			float vSinTheta = launchSpeed * Mathf.Sin(launchAngle);
			float sqrtPortion = vSinTheta * vSinTheta + 2f * g * y0;
			if(sqrtPortion < 0)
			{
				return 0;
			} else
			{
				sqrtPortion = Mathf.Sqrt(sqrtPortion);
			}
			return ((vSinTheta + sqrtPortion) / g);
		}

		public Vector2 FindPosition(Vector2 startPos, float launchAngle, float launchSpeed, float timeSinceLaunch, float g)
		{
			float xDisplacement = timeSinceLaunch * launchSpeed * Mathf.Cos(launchAngle);
			float yDisplacement = timeSinceLaunch * launchSpeed * Mathf.Sin(launchAngle) - 0.5f * g * timeSinceLaunch * timeSinceLaunch;
			return new Vector2(startPos.x + xDisplacement, startPos.y + yDisplacement);
		}
	}

	public struct ThetaReturn
	{
		public bool success;
		public float theta1;
		public float theta2;

		public ThetaReturn(bool succeeds, float thetaOne, float thetaTwo)
		{
			success = succeeds;
			theta1 = thetaOne;
			theta2 = thetaTwo;
		}
	}
}
