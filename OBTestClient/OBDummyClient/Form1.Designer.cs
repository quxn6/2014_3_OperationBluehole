namespace OperationBluehole.DummyClient
{
	partial class DummyClient
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
            this.Btn_start = new System.Windows.Forms.Button();
            this.TextBox_host = new System.Windows.Forms.TextBox();
            this.TextBox_dummyConnections = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_RegisterDelay = new System.Windows.Forms.TextBox();
            this.textBox_UpdateResultDelay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_MaxSimulationCount = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Btn_start
            // 
            this.Btn_start.Location = new System.Drawing.Point(310, 25);
            this.Btn_start.Name = "Btn_start";
            this.Btn_start.Size = new System.Drawing.Size(75, 62);
            this.Btn_start.TabIndex = 0;
            this.Btn_start.Text = "Start";
            this.Btn_start.UseVisualStyleBackColor = true;
            this.Btn_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextBox_host
            // 
            this.TextBox_host.Location = new System.Drawing.Point(62, 25);
            this.TextBox_host.Name = "TextBox_host";
            this.TextBox_host.Size = new System.Drawing.Size(211, 21);
            this.TextBox_host.TabIndex = 1;
            this.TextBox_host.Text = "http://project06.codetalks.kr:3579";
            // 
            // TextBox_dummyConnections
            // 
            this.TextBox_dummyConnections.Location = new System.Drawing.Point(142, 66);
            this.TextBox_dummyConnections.Name = "TextBox_dummyConnections";
            this.TextBox_dummyConnections.Size = new System.Drawing.Size(75, 21);
            this.TextBox_dummyConnections.TabIndex = 2;
            this.TextBox_dummyConnections.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Dummy Connections";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Register delay";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Update result delay";
            // 
            // textBox_RegisterDelay
            // 
            this.textBox_RegisterDelay.Location = new System.Drawing.Point(142, 93);
            this.textBox_RegisterDelay.Name = "textBox_RegisterDelay";
            this.textBox_RegisterDelay.Size = new System.Drawing.Size(75, 21);
            this.textBox_RegisterDelay.TabIndex = 7;
            this.textBox_RegisterDelay.Text = "500";
            // 
            // textBox_UpdateResultDelay
            // 
            this.textBox_UpdateResultDelay.Location = new System.Drawing.Point(142, 120);
            this.textBox_UpdateResultDelay.Name = "textBox_UpdateResultDelay";
            this.textBox_UpdateResultDelay.Size = new System.Drawing.Size(75, 21);
            this.textBox_UpdateResultDelay.TabIndex = 8;
            this.textBox_UpdateResultDelay.Text = "2000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Max simulation count";
            // 
            // textBox_MaxSimulationCount
            // 
            this.textBox_MaxSimulationCount.Location = new System.Drawing.Point(142, 147);
            this.textBox_MaxSimulationCount.Name = "textBox_MaxSimulationCount";
            this.textBox_MaxSimulationCount.Size = new System.Drawing.Size(75, 21);
            this.textBox_MaxSimulationCount.TabIndex = 10;
            this.textBox_MaxSimulationCount.Text = "4";
            // 
            // DummyClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 256);
            this.Controls.Add(this.textBox_MaxSimulationCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_UpdateResultDelay);
            this.Controls.Add(this.textBox_RegisterDelay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_dummyConnections);
            this.Controls.Add(this.TextBox_host);
            this.Controls.Add(this.Btn_start);
            this.Name = "DummyClient";
            this.Text = "Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Btn_start;
		private System.Windows.Forms.TextBox TextBox_host;
		private System.Windows.Forms.TextBox TextBox_dummyConnections;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_RegisterDelay;
        private System.Windows.Forms.TextBox textBox_UpdateResultDelay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_MaxSimulationCount;
	}
}

