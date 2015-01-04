using UnityEngine;
using System.Collections;

public class IncreaseStatButton : MonoBehaviour 
{
	public int StatusID { get; set; }

	public void PushButton()
	{
		StatusManager.Instance.increasedStats[StatusID]++;
		StatusManager.Instance.UseBonusStatPoint();
	}
}
