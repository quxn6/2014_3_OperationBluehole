using UnityEngine;
using System.Collections;


static public class InputManager 
{
	static private float beginTime = 0.0f;
	static private float endTime = 0.0f;
	static private Vector2 beginPosition = Vector2.zero;
	static private Vector2 endPosition = Vector2.zero;

	static public int GetInputType()
	{
		int directionValue = (int)InputType.NONE;
		if ( Input.GetButtonDown("Fire1"))			
		{
			beginTime = Time.time;
			beginPosition = Input.mousePosition;
			directionValue |= (int)InputType.TAP;
		}

		if ( Input.GetButtonUp("Fire1"))
		{
			endTime = Time.time;
			endPosition = Input.mousePosition;

			if ( endTime - beginTime > GameConfig.SWIPE_TIME)
			{
				float xGap = endPosition.x - beginPosition.x;
				float yGap = endPosition.y - beginPosition.y;
				if ( xGap > 0 )
				{
					directionValue |= (int)InputType.SWIPE_EAST;
				}
				else
				{
					directionValue |= (int)InputType.SWIPE_WEST;
				}

				if ( yGap > 0 )
				{
					directionValue |= (int)InputType.SWIPE_NORTH;
				}
				else
				{
					directionValue |= (int)InputType.SWIPE_SOUTH;
				}
			}
			
		}

		return directionValue;
	}

	static public Vector2 GetInputPosition()
	{
		return beginPosition;
	}
}
