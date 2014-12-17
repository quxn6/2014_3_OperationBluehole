using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationBluehole.DummyClient
{
	public partial class DummyClient : Form
	{
		public DummyClient()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Network.Init(this.TextBox_host.Text);
			
			int numOfUsers = Convert.ToInt32(this.TextBox_dummyConnections.Text);

            Random random = new Random();

            TestResult.totalUsers = numOfUsers;
			for (int i = 1; i <= numOfUsers; ++i)
			{
                var user = new User( "testUser" + i, "testPw" + i, random );
				user.Start();
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}
