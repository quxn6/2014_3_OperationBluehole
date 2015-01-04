using UnityEngine;
using System.Collections;
using System.Linq;

public class StatusManager : MonoBehaviour
{
	public GameObject[] statUpButton;
	public GameObject applyButton;
	public GameObject BonusStatPointLabel;

	public int BonusStatPoint { get; set; }
	public int[] increasedStats { get; set; }

	private UILabel BonusStatPointLabelText;

	static private StatusManager instance = null;
	static public StatusManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}
		instance = this;

		increasedStats = new int[(int)OperationBluehole.Content.StatType.StatCount];
		BonusStatPointLabelText = BonusStatPointLabel.GetComponent<UILabel>();
		InitButtons();
	}

	public void InitButtons()
	{
		for ( int i = 1 ; i < increasedStats.Length ; ++i )
		{
			statUpButton[i - 1].GetComponent<IncreaseStatButton>().StatusID = i;
			statUpButton[i - 1].SetActive( false );
		}
		applyButton.SetActive( false );
	}

	public void CalculateStatPoint()
	{
		// Calculate bonus stat
		int totalStats = ( DataManager.Instance.clientPlayerData.Stat[(int)OperationBluehole.Content.StatType.Lev] - 1 ) * OperationBluehole.Content.Config.BONUS_STAT_PER_LEVEL + OperationBluehole.Content.Config.CHARACTER_BASE_STATS.Skip( 1 ).Take( 6 ).Sum( i => i );
		int curStats = DataManager.Instance.clientPlayerData.Stat.Skip( 1 ).Take( 6 ).Sum( i => i );
		BonusStatPoint = totalStats - curStats;

		if ( BonusStatPoint > 0 )
		{
			// Change text
			BonusStatPointLabelText.text = BonusStatPoint.ToString();

			// Show Statup Button
			for ( int i = 1 ; i < increasedStats.Length ; ++i )
			{
				statUpButton[i - 1].SetActive( true );
			}
			applyButton.SetActive( true );
		}
		else
		{
			BonusStatPointLabelText.text = "";
		}
	}

	public void UseBonusStatPoint()
	{
		if ( --BonusStatPoint <= 0 )
		{
			for ( int i = 1 ; i < increasedStats.Length ; ++i )
			{
				statUpButton[i - 1].SetActive( false );
			}
		}

		// refresh view
		BonusStatPointLabelText.text = BonusStatPoint.ToString();
	}

	public void ApplyStatUpResult()
	{
		applyButton.SetActive( false );
		NetworkManager.Instance.ChangeStatsRequest( increasedStats );
	}
}
