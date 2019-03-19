using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PipeCircles
{
	public class LevelLoader : MonoBehaviour
	{
		[SerializeField] int timeToWait = 4;

		int currentSceneIndex;

		void Start()
		{
			currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
			if (currentSceneIndex == 0)
			{
				StartCoroutine(WaitForTime());
			}
		}

		IEnumerator WaitForTime()
		{
			yield return new WaitForSeconds(timeToWait);
			LoadNextScene();
		}

		public void LoadNextScene()
		{
			SceneManager.LoadScene(currentSceneIndex + 1);
		}

		public void LoadMainMenu()
		{
			SceneManager.LoadScene("Main Menu");
		}

		public void LoadOptions()
		{
			SceneManager.LoadScene("Main Menu"); //TODO replace with Options menu once built
		}
	}
}