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
			this.SuspendLayout();
			// 
			// Btn_start
			// 
			this.Btn_start.Location = new System.Drawing.Point(240, 25);
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
			this.TextBox_host.Size = new System.Drawing.Size(155, 21);
			this.TextBox_host.TabIndex = 1;
			// 
			// TextBox_dummyConnections
			// 
			this.TextBox_dummyConnections.Location = new System.Drawing.Point(142, 66);
			this.TextBox_dummyConnections.Name = "TextBox_dummyConnections";
			this.TextBox_dummyConnections.Size = new System.Drawing.Size(75, 21);
			this.TextBox_dummyConnections.TabIndex = 2;
			this.TextBox_dummyConnections.Text = "1";
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
			// TestClient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(336, 102);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TextBox_dummyConnections);
			this.Controls.Add(this.TextBox_host);
			this.Controls.Add(this.Btn_start);
			this.Name = "TestClient";
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
	}
}

