using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class WifiDevices
    {
        private const int m_timeout = 1000;
        private List<IPAddress> m_devices;
        private Dictionary<IPAddress, String> m_heaters;
        private Dictionary<String, double> m_temperatures;

        public WifiDevices()
        {
            m_devices = new List<IPAddress>();
            m_heaters = new Dictionary<IPAddress, String>();
            m_temperatures = new Dictionary<string, double>();
        }

        /// <summary>
        /// Sends schedule to all heaters on list.
        /// Starts in new thread.
        /// </summary>
        /// <param name="heaters">heaters to change</param>
        public void SendSchedules(List<Heater> heaters, List<Schedule> schedules)
        {
            Thread t = new Thread(delegate ()
            {
                bool found;
                foreach (Heater h in heaters)
                {
                    found = false;
                    lock (m_heaters)
                    {
                        foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                        {
                            if (elem.Value.Equals(h.Id))
                            {
                                WiFiClient wc = new WiFiClient(elem.Key.ToString());
                                foreach (Schedule s in schedules)
                                {
                                    if (s.GetName().Equals(h.Name))
                                    {
                                        wc.SendSchedule(s);
                                    }
                                }
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                        continue;

                    FindHeaters();

                    lock (m_heaters)
                    {
                        foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                        {
                            if (elem.Value.Equals(h.Id))
                            {
                                WiFiClient wc = new WiFiClient(elem.Key.ToString());
                                foreach (Schedule s in schedules)
                                {
                                    if (s.GetName().Equals(h.Name))
                                    {
                                        wc.SendSchedule(s);
                                    }
                                }
                                found = true;
                                break;
                            }
                        }
                    }
                }
            });
            t.Start();
        }

        /// <summary>
        /// Sends schedule to all heaters on list.
        /// Starts in new thread.
        /// </summary>
        /// <param name="heaters">heaters to change</param>
        public void SendAlternativeSchedules(List<Heater> heaters, List<Schedule> schedules)
        {
            Thread t = new Thread(delegate ()
            {
                bool found;
                foreach (Heater h in heaters)
                {
                    found = false;
                    lock (m_heaters)
                    {
                        foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                        {
                            if (elem.Value.Equals(h.Id))
                            {
                                WiFiClient wc = new WiFiClient(elem.Key.ToString());
                                foreach (Schedule s in schedules)
                                {
                                    if (s.GetName().Equals(h.Name))
                                    {
                                        wc.SendAlternativeSchedule(s);
                                    }
                                }
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                        continue;

                    FindHeaters();

                    lock (m_heaters)
                    {
                        foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                        {
                            if (elem.Value.Equals(h.Id))
                            {
                                WiFiClient wc = new WiFiClient(elem.Key.ToString());
                                foreach (Schedule s in schedules)
                                {
                                    if (s.GetName().Equals(h.Name))
                                    {
                                        wc.SendAlternativeSchedule(s);
                                    }
                                }
                                found = true;
                                break;
                            }
                        }
                    }
                }
            });
            t.Start();
        }

        /// <summary>
        /// Changes id of new heater to new id.
        /// Only one new heater must be connected.
        /// Starts in new thread.
        /// </summary>
        /// <param name="newId"></param>
        public void ChangeIdfromStartId(String newId)
        {
            Thread t = new Thread(delegate ()
            {
                lock (m_heaters)
                {
                    foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                    {
                        if (elem.Value.Equals(newId))
                        {
                            WiFiClient wc = new WiFiClient(elem.Key.ToString());
                            wc.ChangeId(newId);
                            m_heaters.Remove(elem.Key);
                            return;
                        }
                    }
                }
                FindHeaters();

                lock (m_heaters)
                {
                    foreach (KeyValuePair<IPAddress, String> elem in m_heaters)
                    {
                        if (elem.Value.Equals(newId))
                        {
                            WiFiClient wc = new WiFiClient(elem.Key.ToString());
                            wc.ChangeId(newId);
                            m_heaters.Remove(elem.Key);
                            return;
                        }
                    }
                }
            });
            t.Start();
        }

        /// <summary>
        /// Sends date of return and temperature to all devices.
        /// Starts in new thread
        /// </summary>
        /// <param name="date">Date of return</param>
        /// <param name="temp">Desired temperature</param>
        public void SendReturnDateAndTemperature(DateTime date, int temp)
        {
            Thread t = new Thread(delegate ()
            {
                Dictionary<IPAddress, String> devices = FindHeaters();

                foreach (IPAddress address in devices.Keys)
                {
                    SendReturnDateAndTemperatureToDevice(date, temp, address);
                }
            });
            t.Start();
        }

        /// <summary>
        /// Starts new thread
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, double> GetTemperature()
        {
            Dictionary<IPAddress, String> devices = FindHeaters();
            List<Thread> workers = new List<Thread>();

            foreach (KeyValuePair<IPAddress, String> keyValuePair in devices)
            {
                workers.Add(new Thread(() => GetTemperatureFromDevice(keyValuePair.Key, keyValuePair.Value)));
                workers.ElementAt(workers.Count - 1).Start();
            }

            foreach (Thread worker in workers)
            {
                worker.Join();
            }

            Dictionary<String, double> res;
            lock (m_temperatures)
            {
                // copy of dictionary
                res = m_temperatures.ToDictionary(entry => entry.Key,
                                                  entry => entry.Value);
            }

            return res;
        }

        /// <summary>
        /// Finds all heaters on network.
        /// Waits for searching to finish.
        /// </summary>
        /// <returns>New instance of dictionary</returns>
        public Dictionary<IPAddress, String> FindHeaters()
        {
            PingDevices();
            List<Thread> workers = new List<Thread>();
            foreach (IPAddress a in m_devices)
            {
                workers.Add(new Thread(() => AuthenticateDevice(a)));
                workers.ElementAt(workers.Count - 1).Start();
            }

            // wait for workers
            foreach (Thread worker in workers)
            {
                worker.Join();
            }

            Dictionary<IPAddress, String> res;
            lock (m_heaters)
            {
                // copy of dictionary
                res = m_heaters.ToDictionary(entry => entry.Key,
                                             entry => entry.Value);
            }
            
            return res;
        }

        /// <summary>
        /// Pings all devices on network
        /// Waits for pinging to finish
        /// </summary>
        /// <returns>new list of found devices</returns>
        public List<IPAddress> PingDevices()
        {
            IPAddress local = GetLocalIpAddress();
            IPAddress mask = GetSubnetMask(local);
            IPAddress net = NetAddress(local, mask);

            byte[] devices = new byte[local.GetAddressBytes().Length];
            byte[] bytes = new byte[local.GetAddressBytes().Length];
            // mask negation
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i] = (byte) (mask.GetAddressBytes()[i] ^ 255);
            }

            // iterate over all devices
            List<Thread> workers = new List<Thread>();

            IPAddress device;
            Decrese(devices);
            while (IsBiggerThanZero(devices))
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (byte) (net.GetAddressBytes()[i] | devices[i]);
                }
                device = new IPAddress(bytes);
                //PingDevice(device);
                workers.Add(new Thread(() => PingDevice(device)));
                workers.ElementAt(workers.Count - 1).Start();

                Decrese(devices);
            }

            // wait for workers
            foreach (Thread worker in workers)
            {
                worker.Join();
            }

            return m_devices;
        }

        /// <summary>
        /// Sends request to heater, to get temperature.
        /// Writes temperature to m_temperatures.
        /// </summary>
        /// <param name="address">IP address of heater</param>
        /// <param name="name">Name of heater</param>
        private void GetTemperatureFromDevice(IPAddress address, String name)
        {
            WiFiClient wc = new WiFiClient(address.ToString());
            double res = wc.GetTemperature();

            lock (m_temperatures)
            {
                if (m_temperatures.ContainsKey(name))
                {
                    m_temperatures.Remove(name);
                }
                m_temperatures.Add(name, res);
            }
        }

        /// <summary>
        /// Sends return date and temperature to specified device.
        /// Creates new thread.
        /// </summary>
        /// <param name="date">Return date</param>
        /// <param name="temp">Desired temperature</param>
        /// <param name="address">IP address of device</param>
        private void SendReturnDateAndTemperatureToDevice(DateTime date, int temp, IPAddress address)
        {
            Thread t = new Thread(delegate ()
            {
                WiFiClient wc = new WiFiClient(address.ToString());
                wc.SendReturnDateAndTemperature(date, temp);
            });
            t.Start();
        }
        /// <summary>
        /// Creates new thread which pings device
        /// </summary>
        /// <param name="address">device to ping</param>
        private void PingDevice(IPAddress address)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(address, m_timeout);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            if (pingable)
            {
                lock (m_devices)
                {
                    if (!m_devices.Contains(address))
                        m_devices.Add(address);
                }
            }
        }

        /// <summary>
        /// Starts in new thread
        /// Adds address and id to m_heaters
        /// </summary>
        /// <param name="address">Adress to communicate with</param>
        private void AuthenticateDevice(IPAddress address)
        {
            Debug.WriteLine("Contacting: " + address.ToString()); 
            String id;
            try
            {
                WiFiClient wc = new WiFiClient(address.ToString());
                id = wc.IdentifyHeater();
            }
            catch
            {
                return;
            }
            lock (m_heaters)
            {
                if (!m_heaters.ContainsKey(address))
                    m_heaters.Add(address, id);
            }
            Debug.WriteLine("Found ID " + id + " that belongs to " + address.ToString());
        }

        /// <summary>
        /// Checks if number represented in byte array is bigger than zero
        /// </summary>
        /// <param name="bytes">Number in byte form</param>
        /// <returns>True if any byte is bigger than zero</returns>
        private static bool IsBiggerThanZero(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                if (b > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Decreses byte[] as binary number
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static void Decrese(byte[] bytes)
        {
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] > 0)
                {
                    bytes[i]--;
                    for (int j = i + 1; j < bytes.Length; j++)
                    {
                        bytes[j] |= (byte)255;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Calculates address of network
        /// </summary>
        /// <param name="ip">IP of device on network</param>
        /// <param name="mask">subnet mask</param>
        /// <returns>Address of network</returns>
        private static IPAddress NetAddress(IPAddress ip, IPAddress mask)
        {
            byte[] netBytes = new byte[ip.GetAddressBytes().Length];
            for (int i = 0; i < ip.GetAddressBytes().Length; i++)
            {
                netBytes[i] = (byte)(ip.GetAddressBytes()[i] & mask.GetAddressBytes()[i]);
            }

            return new IPAddress(netBytes);
        }
        
        /// <summary>
        /// Finds subnet mask from given IP
        /// </summary>
        /// <param name="address">IP of device</param>
        /// <returns>subnet mask</returns>
        private static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        /// <summary>
        /// Tries avoid selecting IP address from wrong adapter
        /// Virtual box etc..
        /// throws if no suitable address is found
        /// </summary>
        /// <returns>local ip address</returns>
        private static IPAddress GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                var properties = network.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    // adress from stateless configuration
                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }

                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }

                    return address.Address;
                }
            }

            if (mostSuitableIp == null)
                throw new Exception("No IP adress");
            return mostSuitableIp.Address;
        }
    }
}
