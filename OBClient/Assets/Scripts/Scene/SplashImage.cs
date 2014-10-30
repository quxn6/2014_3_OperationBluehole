using UnityEngine;
using System.Collections;

public class SplashImage : MonoBehaviour {
	IEnumerator Start()
	{
		yield return new WaitForSeconds( GameConfig.SPLASH_IMAGE_PLAY_TIME );
		SpriteRenderer splashImage = gameObject.GetComponent<SpriteRenderer>();
		float newColorValue = 1.0f;
		while ( splashImage.color.b > 0.0f )
		{
			newColorValue -= 0.01f;
			splashImage.color = new Color( newColorValue , newColorValue , newColorValue );
			yield return null;
		}
		Application.LoadLevel( "title" );
	}
}
