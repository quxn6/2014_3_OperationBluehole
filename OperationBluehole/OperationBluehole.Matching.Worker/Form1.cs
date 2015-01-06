using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationBluehole.Matching.Worker
{
	public partial class Form_Worker : Form
	{
		public Form_Worker()
		{
			InitializeComponent();
		}

		static Matching matchingWorker;
		private void Btn_Start_Click( object sender, EventArgs e )
		{
			Btn_Start.Enabled = false;

			ushort minLev = Convert.ToUInt16( TextBox_MinLev.Text );
			ushort maxLev = Convert.ToUInt16( TextBox_MaxLev.Text );
			string lobby = TextBox_Lobby.Text;
			string simulation = TextBox_Simulation.Text;

			if ( minLev > maxLev || maxLev == 0 || minLev == 0 )
			{
				Console.WriteLine( "Wrong Level Range." );
				Btn_Start.Enabled = true;
				return;
			}
			
			matchingWorker = new Matching( minLev, maxLev, simulation );

			var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };

			var TMRun = new Thread( () => TaskManager.Run( factory, matchingWorker, lobby ) );
			TMRun.Start();

			this.Text += "_" + TaskManager.workerNum;

			TextBox_MinLev.Text = "";
			TextBox_MaxLev.Text = "";
			TextBox_Lobby.Enabled = false;
			TextBox_Simulation.Enabled = false;

			Btn_Apply.Enabled = true;
		}

		private void Btn_Apply_Click( object sender, EventArgs e )
		{
			Btn_Apply.Enabled = false;

			ushort minLev = Convert.ToUInt16( TextBox_MinLev.Text );
			ushort maxLev = Convert.ToUInt16( TextBox_MaxLev.Text );

			TextBox_MinLev.Text = "";
			TextBox_MaxLev.Text = "";

			Btn_Apply.Enabled = true;
		}

		public void UpdateMinLev( ushort minLev )
		{
			Label_MinLev.Text = minLev.ToString();
		}

		public void UpdateMaxLev( ushort maxLev )
		{
			Label_MaxLev.Text = maxLev.ToString();
		}

		public void UpdateWaitingPlayers( int waitingPlayers )
		{
			Label_WaitingPlayers.Text = waitingPlayers.ToString();
		}

		public void UpdateInPartyPlayers( int inPartyPlayers )
		{
			Label_InPartyPlayers.Text = inPartyPlayers.ToString();
		}

		public void UpdateWaitingParties( int waitingParties )
		{
			Label_WaitingParties.Text = waitingParties.ToString();
		}
	}
}
