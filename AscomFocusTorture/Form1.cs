using System;
using System.Windows.Forms;

namespace ASCOM.FocuserDebouncer
{
    public partial class Form1 : Form
    {

        private ASCOM.DriverAccess.Focuser driver;

        private int start_position;
        private int count_ticks;

        private int total_calls;

        public Form1()
        {
            InitializeComponent();
            SetUIState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnected)
                driver.Connected = false;

            Properties.Settings.Default.Save();
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DriverId = ASCOM.DriverAccess.Focuser.Choose(Properties.Settings.Default.DriverId);
            SetUIState();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                driver.Connected = false;
            }
            else
            {
                driver = new ASCOM.DriverAccess.Focuser(Properties.Settings.Default.DriverId);
                driver.Connected = true;
            }
            SetUIState();
        }

        private void SetUIState()
        {
            buttonConnect.Enabled = !string.IsNullOrEmpty(Properties.Settings.Default.DriverId);
            buttonChoose.Enabled = !IsConnected;
            buttonConnect.Text = IsConnected ? "Disconnect" : "Connect";
        }

        private bool IsConnected
        {
            get
            {
                return ((this.driver != null) && (driver.Connected == true));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.IsConnected)
            {
                // Start stress test
                if (this.timer1.Enabled == false)
                {
                    this.listBox1.Items.Clear();
                    this.total_calls = 0;
                    this.listBox1.Items.Add("Started at " + DateTime.Now.ToShortTimeString());
                    this.listBox1.Items.Add(this.driver.Name);

                    this.start_position = this.driver.Position;
                    this.timer1.Enabled = true;
                    this.button1.Text = "Disable Stresstest"; 
                } else
                {
                    this.timer1.Enabled = false;
                    this.button1.Text = "Enable Stresstest";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // send the commands
            if (this.IsConnected)
            {
                count_ticks++;
                try
                {


                    var tmp = this.driver.IsMoving;
                    var tmp2 = this.driver.Temperature;

                    if (count_ticks == 10)
                    {
                        // Move position
                        if (this.driver.Position > this.start_position)
                        {
                            this.driver.Move(this.start_position - 100);
                        }
                        else
                        {
                            this.driver.Move(this.start_position + 100);
                        }
                        count_ticks = 0;
                        this.listBox1.Items.Add(DateTime.Now.ToShortTimeString() + ": Position: " + this.driver.Position.ToString() + ", IsMoving: " + tmp.ToString() + ", Temperature: " + tmp2.ToString() + " ( done 10 calls in between)");
                    }
                    this.total_calls++;
                } catch (Exception ex)
                {
                    this.timer1.Enabled = false;
                    this.listBox1.Items.Add(ex.Message + " (total calls " + this.total_calls.ToString() + ")");
                }

                
                
                // Move position up a little
            }
        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }
    }
}
