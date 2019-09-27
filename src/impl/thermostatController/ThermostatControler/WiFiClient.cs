using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

namespace ThermostatControler
{
    public class WiFiClient
    {
        private const int m_port = 1111;
        private const int TIMEOUT = 1000;
        private TcpClient m_client;
        private NetworkStream m_nwStream;

        /// <summary>
        /// Creates new client
        /// </summary>
        /// <param name="ipAdress"></param>
        public WiFiClient(String ipAdress)
        {
            try
            {
                m_client = new TcpClient(ipAdress, m_port);
                m_nwStream = m_client.GetStream();
            }
            catch
            {
                throw new SocketException();
            }
        }

        ~WiFiClient()
        {
            if (m_nwStream != null)
                m_nwStream.Close();
            if (m_client != null)
                m_client.Close();
        }

        /// <summary>
        /// Sends schedule to server in compact form
        /// Day = "0" | "1" | "2" | "3" | "4" | "5" | "6"
        /// Num 4 = "0" | "1" | "2" | "3"
        /// Num 5 = "0" | "1" | "2" | "3" | "4"
        /// Num 9 = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Num 10 = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Minutes = Num 10 | Num 9, Num 10 | "1", Num 4, Num 10 | | "1", "4", Num 5
        /// Temperature = Num 10 | Num 9, Num 10 | "1", Num 10, Num 10 | "200"
        /// Node = Day, Minutes, Temperature
        /// Message = "s", {Node}, "7"
        /// </summary>
        /// <param name="sch">Schedule to send</param>
        public void SendSchedule(Schedule sch)
        {
            SendScheduleWithPrefix(sch, 's');
        }

        /// <summary>
        /// Sends schedule to server in compact form
        /// Day = "0" | "1" | "2" | "3" | "4" | "5" | "6"
        /// Num 4 = "0" | "1" | "2" | "3"
        /// Num 5 = "0" | "1" | "2" | "3" | "4"
        /// Num 9 = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Num 10 = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Minutes = Num 10 | Num 9, Num 10 | "1", Num 4, Num 10 | | "1", "4", Num 5
        /// Temperature = Num 10 | Num 9, Num 10 | "1", Num 10, Num 10 | "200"
        /// Node = Day, Minutes, Temperature
        /// Message = "v", {Node}, "7"
        /// </summary>
        /// <param name="sch">Schedule to send</param>
        public void SendAlternativeSchedule(Schedule sch)
        {
            SendScheduleWithPrefix(sch, 'v');
        }

        /// <summary>
        /// Sends schedule to server in compact form
        /// Day = "0" | "1" | "2" | "3" | "4" | "5" | "6"
        /// Num 4 = "0" | "1" | "2" | "3"
        /// Num 5 = "0" | "1" | "2" | "3" | "4"
        /// Num 9 = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Num 10 = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// Minutes = Num 10 | Num 9, Num 10 | "1", Num 4, Num 10 | | "1", "4", Num 5
        /// Temperature = Num 10 | Num 9, Num 10 | "1", Num 10, Num 10 | "200"
        /// Node = Day, Minutes, Temperature
        /// Message = prefix, {Node}, "7"
        /// </summary>
        /// <param name="sch">Schedule to send.</param>
        /// <param name="prefix">Message prefix.</param>
        private void SendScheduleWithPrefix(Schedule sch, char prefix)
        {
            byte[] msg = new byte[sch.getNumOfNodes() * 3 + 2];
            int idx = 0;
            msg[idx++] = Convert.ToByte(prefix);
            for (int i = 0; i < Schedule.NUM_DAYS; i++)
            {
                foreach (TempNode n in sch.GetDay(i))
                {
                    msg[idx++] = (byte)i;
                    msg[idx++] = n.GetNumOfMinutes();
                    msg[idx++] = n.GetTemperature();
                }
            }
            msg[idx] = (byte)7;

            m_nwStream.Write(msg, 0, msg.Length);
        }

        /// <summary>
        /// Send message = "hDAY,MONTH,YEAR,TEMPERATURE"
        /// Day, month, temperature always have 2 digits
        /// </summary>
        /// <param name="date">Date of return</param>
        /// <param name="temp">Desired temperature</param>
        public void SendReturnDateAndTemperature(DateTime date, int temp)
        {
            StringBuilder stringBuilder = new StringBuilder("h");
            if (date.Day < 10)
                stringBuilder.Append('0');
            stringBuilder.Append(date.Day.ToString());
            stringBuilder.Append(',');

            if (date.Month < 10)
                stringBuilder.Append('0');
            stringBuilder.Append(date.Month.ToString());
            stringBuilder.Append(',');

            stringBuilder.Append(date.Year.ToString());
            stringBuilder.Append(',');

            if (temp < 10)
                stringBuilder.Append('0');
            stringBuilder.Append(temp.ToString());

            SendData(stringBuilder.ToString());
        }

        /// <summary>
        /// Send message = "a"
        /// Expects "OK" + number
        /// Can throw wrong response
        /// </summary>
        /// <returns>device authentication number</returns>
        public String IdentifyHeater()
        {
            SendData("a");
            Thread.Sleep(TIMEOUT);
            String res;
            try
            {
                res = ReadData();
            }
            catch
            {
                throw new InvalidDataException("Device didn't respond");
            }
            if (!res.StartsWith("OK") || res.Length < 5)
                throw new InvalidDataException("Device responded with wrong message");
            return res.Substring(2,2);
        }

        /// <summary>
        /// Sends message = "c" + newId
        /// </summary>
        /// <param name="newId">the new id</param>
        public void ChangeId(String newId)
        {
            SendData("c" + newId);
        }

        /// <summary>
        /// Gets temperature from client
        /// Sends message = "t"
        /// </summary>
        /// <returns>Measured temperature</returns>
        public double GetTemperature()
        {
            SendData("t");
            Thread.Sleep(TIMEOUT);

            String response = ReadData();
            double res;
            try
            {
                // fix for decimal separator "."
                res = Double.Parse(response.Substring(0, 4), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            catch
            {
                res = Double.NaN;
            }

            return res;
        }

        /// <summary>
        /// Sends data to server
        /// </summary>
        /// <param name="data"></param>
        private void SendData(String data)
        {
            m_nwStream.Write(ASCIIEncoding.ASCII.GetBytes(data), 0, ASCIIEncoding.ASCII.GetBytes(data).Length);
        }

        /// <summary>
        /// Reads data from server
        /// </summary>
        /// <returns></returns>
        private String ReadData()
        {
            byte[] data = new byte[m_client.ReceiveBufferSize];
            int bytesRead = m_nwStream.Read(data, 0, m_client.ReceiveBufferSize);
            return Encoding.ASCII.GetString(data);
        }
    }
}
