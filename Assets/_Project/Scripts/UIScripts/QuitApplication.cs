using UnityEngine;

public class QuitApplication : MonoBehaviour
{
	public void Quit()
	{
#if UNITY_STANDALONE_WIN
		Application.Quit();
#endif
#if UNITY_WEBGL
		Application.OpenURL("https://solanafightclub.xyz");
#endif
	}
}
