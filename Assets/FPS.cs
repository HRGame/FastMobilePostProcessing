using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	Text text;
	double lastInterval;
	int frames = 0;

	void Start()
	{
		text = GetComponent<Text>();

		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}

	void Update()
	{
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if( timeNow > lastInterval + updateInterval )
		{
			float fps = (float)( frames / ( timeNow - lastInterval ) );
			frames = 0;
			lastInterval = timeNow;

			text.text = Mathf.RoundToInt( fps ).ToString();
		}
	}
}
