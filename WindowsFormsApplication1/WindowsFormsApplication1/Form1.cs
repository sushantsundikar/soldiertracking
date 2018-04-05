using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics; //for debug.writeline
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MySqlConnection con = new MyConn().CON;

        //create a serial port object
        SerialPort _serialPort;
        // delegate is used to write to a UI control from a non-UI thread
        private delegate void SetTextDeleg(string text);
        string soldierid = "";

        public Form1()
        {
            InitializeComponent();
        }
       
        public Form1(string sid)
        {
            InitializeComponent();
            soldierid = sid;
            txtSoldierId.Text = soldierid;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
           
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string lat="43.23";
            string lng="114.08";
            webBrowser1.Navigate("http://maps.google.com/maps?q="+lat+","+lng);
            // Makes sure serial port is open before trying to write
            try
            {

                // all of the options for a serial device
                // can be sent through the constructor of the SerialPort class
                // PortName = "COM1", Baud Rate = 19200, Parity = None, 
                // Data Bits = 8, Stop Bits = One, Handshake = None
                _serialPort = new MyPorts().PORT;
                _serialPort.Handshake = Handshake.None;
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
                _serialPort.Open();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening/writing to serial port :: " + ex.Message, "Error!");
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(1000);
            string data = _serialPort.ReadExisting();
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
            _serialPort.Close();
        }

        private void si_DataReceived(string data)
        {
            txtBPM.Text = "";
            string bpmdata=data.Trim();
            Debug.Write(bpmdata);
            string[] array = bpmdata.Split(' ');
            txtBPM.Text = array[2];
            //foreach (string s in array)
            //{
            //    listBox1.Items.Add(s);
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serialPort.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool exists = true;
            try
            {
                 string Url="";
                string select_query = "select * from soldier_tracking where soldierid = '" + txtSoldierId.Text + "'";
                MySqlCommand cmd = new MySqlCommand(select_query, con);
                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exists = false;
                    double lat = double.Parse(rdr["lat"].ToString());
                    double lng = double.Parse(rdr["lng"].ToString());
                    Url = "http://maps.google.com/maps?q=" + lat + "," + lng;
                   
                }
                con.Close();
                if (exists)
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                    MessageBox.Show("No Location Detected");
                }
                webBrowser1.Navigate(Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.ToString());
            }

            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Makes sure serial port is open before trying to write
            try
            {
                timer1.Enabled = true;
                timer1.Start();
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                _serialPort.Write("SI\r\n");
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening/writing to serial port :: " + ex.Message, "Error!");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Makes sure serial port is open before trying to write
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                else
                {
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();

                    _serialPort.Write("SI\r\n");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening/writing to serial port :: " + ex.Message, "Error!");
            }
        }
    }
}
