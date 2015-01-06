namespace OperationBluehole.Matching.Worker
{
	partial class Form_Worker
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.TextBox_MinLev = new System.Windows.Forms.TextBox();
			this.Btn_Start = new System.Windows.Forms.Button();
			this.Label_MinLev = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.Label_MaxLev = new System.Windows.Forms.Label();
			this.TextBox_MaxLev = new System.Windows.Forms.TextBox();
			this.Btn_Apply = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.Label_WaitingPlayers = new System.Windows.Forms.Label();
			this.Label_InPartyPlayers = new System.Windows.Forms.Label();
			this.Label_WaitingParties = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TextBox_Lobby = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.TextBox_Simulation = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "MinLev";
			// 
			// TextBox_MinLev
			// 
			this.TextBox_MinLev.Location = new System.Drawing.Point(115, 92);
			this.TextBox_MinLev.Name = "TextBox_MinLev";
			this.TextBox_MinLev.Size = new System.Drawing.Size(36, 21);
			this.TextBox_MinLev.TabIndex = 1;
			// 
			// Btn_Start
			// 
			this.Btn_Start.Location = new System.Drawing.Point(17, 200);
			this.Btn_Start.Name = "Btn_Start";
			this.Btn_Start.Size = new System.Drawing.Size(62, 28);
			this.Btn_Start.TabIndex = 5;
			this.Btn_Start.Text = "Start";
			this.Btn_Start.UseVisualStyleBackColor = true;
			this.Btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
			// 
			// Label_MinLev
			// 
			this.Label_MinLev.AutoSize = true;
			this.Label_MinLev.BackColor = System.Drawing.SystemColors.Window;
			this.Label_MinLev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label_MinLev.Location = new System.Drawing.Point(89, 95);
			this.Label_MinLev.Name = "Label_MinLev";
			this.Label_MinLev.Size = new System.Drawing.Size(13, 14);
			this.Label_MinLev.TabIndex = 0;
			this.Label_MinLev.Text = "0";
			this.Label_MinLev.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 122);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "MaxLev";
			// 
			// Label_MaxLev
			// 
			this.Label_MaxLev.AutoSize = true;
			this.Label_MaxLev.BackColor = System.Drawing.SystemColors.Window;
			this.Label_MaxLev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label_MaxLev.Location = new System.Drawing.Point(89, 122);
			this.Label_MaxLev.Name = "Label_MaxLev";
			this.Label_MaxLev.Size = new System.Drawing.Size(13, 14);
			this.Label_MaxLev.TabIndex = 0;
			this.Label_MaxLev.Text = "0";
			this.Label_MaxLev.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// TextBox_MaxLev
			// 
			this.TextBox_MaxLev.Location = new System.Drawing.Point(115, 119);
			this.TextBox_MaxLev.Name = "TextBox_MaxLev";
			this.TextBox_MaxLev.Size = new System.Drawing.Size(36, 21);
			this.TextBox_MaxLev.TabIndex = 2;
			// 
			// Btn_Apply
			// 
			this.Btn_Apply.Enabled = false;
			this.Btn_Apply.Location = new System.Drawing.Point(89, 200);
			this.Btn_Apply.Name = "Btn_Apply";
			this.Btn_Apply.Size = new System.Drawing.Size(62, 28);
			this.Btn_Apply.TabIndex = 6;
			this.Btn_Apply.Text = "Apply";
			this.Btn_Apply.UseVisualStyleBackColor = true;
			this.Btn_Apply.Click += new System.EventHandler(this.Btn_Apply_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 10);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 12);
			this.label5.TabIndex = 0;
			this.label5.Text = "WaitingPlayers";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 65);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(84, 12);
			this.label6.TabIndex = 0;
			this.label6.Text = "WaitingParties";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 39);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(87, 12);
			this.label7.TabIndex = 0;
			this.label7.Text = "PlayersInParty";
			// 
			// Label_WaitingPlayers
			// 
			this.Label_WaitingPlayers.AutoSize = true;
			this.Label_WaitingPlayers.BackColor = System.Drawing.SystemColors.Window;
			this.Label_WaitingPlayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label_WaitingPlayers.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Label_WaitingPlayers.Location = new System.Drawing.Point(138, 10);
			this.Label_WaitingPlayers.Name = "Label_WaitingPlayers";
			this.Label_WaitingPlayers.Size = new System.Drawing.Size(13, 14);
			this.Label_WaitingPlayers.TabIndex = 0;
			this.Label_WaitingPlayers.Text = "0";
			this.Label_WaitingPlayers.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// Label_InPartyPlayers
			// 
			this.Label_InPartyPlayers.AutoSize = true;
			this.Label_InPartyPlayers.BackColor = System.Drawing.SystemColors.Window;
			this.Label_InPartyPlayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label_InPartyPlayers.Location = new System.Drawing.Point(138, 37);
			this.Label_InPartyPlayers.Name = "Label_InPartyPlayers";
			this.Label_InPartyPlayers.Size = new System.Drawing.Size(13, 14);
			this.Label_InPartyPlayers.TabIndex = 0;
			this.Label_InPartyPlayers.Text = "0";
			this.Label_InPartyPlayers.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// Label_WaitingParties
			// 
			this.Label_WaitingParties.AutoSize = true;
			this.Label_WaitingParties.BackColor = System.Drawing.SystemColors.Window;
			this.Label_WaitingParties.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label_WaitingParties.Location = new System.Drawing.Point(138, 63);
			this.Label_WaitingParties.Name = "Label_WaitingParties";
			this.Label_WaitingParties.Size = new System.Drawing.Size(13, 14);
			this.Label_WaitingParties.TabIndex = 0;
			this.Label_WaitingParties.Text = "0";
			this.Label_WaitingParties.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 149);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "Lobby";
			// 
			// TextBox_Lobby
			// 
			this.TextBox_Lobby.Location = new System.Drawing.Point(80, 146);
			this.TextBox_Lobby.Name = "TextBox_Lobby";
			this.TextBox_Lobby.Size = new System.Drawing.Size(71, 21);
			this.TextBox_Lobby.TabIndex = 3;
			this.TextBox_Lobby.Text = "localhost";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(15, 176);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "Simulation";
			// 
			// TextBox_Simulation
			// 
			this.TextBox_Simulation.Location = new System.Drawing.Point(80, 173);
			this.TextBox_Simulation.Name = "TextBox_Simulation";
			this.TextBox_Simulation.Size = new System.Drawing.Size(71, 21);
			this.TextBox_Simulation.TabIndex = 4;
			this.TextBox_Simulation.Text = "localhost";
			// 
			// Form_Worker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(168, 241);
			this.Controls.Add(this.Btn_Apply);
			this.Controls.Add(this.Btn_Start);
			this.Controls.Add(this.TextBox_Simulation);
			this.Controls.Add(this.TextBox_Lobby);
			this.Controls.Add(this.TextBox_MaxLev);
			this.Controls.Add(this.TextBox_MinLev);
			this.Controls.Add(this.Label_WaitingParties);
			this.Controls.Add(this.Label_InPartyPlayers);
			this.Controls.Add(this.Label_MaxLev);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.Label_WaitingPlayers);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.Label_MinLev);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Name = "Form_Worker";
			this.Text = "Worker";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TextBox_MinLev;
		private System.Windows.Forms.Button Btn_Start;
		private System.Windows.Forms.Label Label_MinLev;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label Label_MaxLev;
		private System.Windows.Forms.TextBox TextBox_MaxLev;
		private System.Windows.Forms.Button Btn_Apply;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label Label_WaitingPlayers;
		private System.Windows.Forms.Label Label_InPartyPlayers;
		private System.Windows.Forms.Label Label_WaitingParties;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TextBox_Lobby;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox TextBox_Simulation;
	}
}

