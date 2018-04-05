using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace WindowsFormsApplication1
{
    public class MyPorts
    {
        SerialPort _serialPort = null;
       
        public MyPorts()
        {
            _serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
            
        }
        public SerialPort PORT
        {
            get
            {
                return _serialPort;
            }
        }
    }
}
