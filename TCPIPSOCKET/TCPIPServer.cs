using System.Net.Sockets;
using System;
using System.Diagnostics;
using System.Net;

namespace TCPIP_Server
{
    public class TCPIPServer
    {
        private TcpListener Listener;
        private TcpClient Client;
        private int Port;
        private string IP;
        private int BufferSize;
        private byte StartByte;
        public bool IsClientConnected = false;

        public TCPIPServer(int port = 38000, string ip = "", int bufferSize = 64 * 1024, byte startByte = (byte)'A')
        {
            Port = port;
            IP = ip;
            BufferSize = bufferSize;
            StartByte = startByte;
        }

        public string setupserver()
        {
            try
            {
                IPAddress localaddr = null;
                int sayac = 0;

                if (string.IsNullOrEmpty(IP))
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var myip in host.AddressList)
                    {
                        if (myip.AddressFamily == AddressFamily.InterNetwork && sayac < 1)
                        {
                            localaddr = myip;
                            sayac = sayac + 1;
                        }
                    }
                }
                else
                {
                    localaddr = IPAddress.Parse(IP);
                }

                Listener = new TcpListener(localaddr, Port);
                Listener.Start();

                IP = Listener.LocalEndpoint.ToString();


                Debug.WriteLine("Server is Ready");
                return localaddr.ToString();

            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed To Start Server:" + ex.ToString());
                return null;
            }
        }       

        public string StartListener()
        {
            try
            {
                if(Listener == null)
                {
                    return null;
                }
                Debug.WriteLine("Listener is Started IP:  " + IP + "  Port: " + Port);

                Client = Listener.AcceptTcpClient(); //blocking here
                IsClientConnected = true;
                IPEndPoint endPoint = (IPEndPoint)Client.Client.RemoteEndPoint;
                var ipAddress = endPoint.Address;
                Client.ReceiveBufferSize = BufferSize;
                Client.SendBufferSize = BufferSize;



                Debug.WriteLine(ipAddress + " is connected");
                return ipAddress.ToString();



            }
            catch(Exception e)
            {
                Debug.WriteLine("Error occured on Listener: " + e.ToString());
                return null;
            }




        }

        public void CloseServer()
        {
            try
            {
                if (Client != null)
                {
                    Client.Close();
                    Client = null;
                    IsClientConnected = false;
                }
                if (Listener == null)
                {
                    return;
                }
                Listener.Stop();
                Listener = null;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Failed to close Server: " + e.ToString());

            }
        }


        public bool SendDataToClient(byte[] data)
        {
            ///start byte
            ///datalength uzunluğunda bir header
            
            byte[] headerBytes = PrepareDataHeader(data.Length);

            int dataLength = headerBytes.Length + data.Length;

            byte[] dataBytesToSend = new byte[dataLength];

            headerBytes.CopyTo(dataBytesToSend, 0);
            data.CopyTo(dataBytesToSend, headerBytes.Length);

            bool success = false;


            try
            {
               if(Client == null)
                {
                    return false;
                }
                if (Client.Connected)
                {
                    NetworkStream stream = Client.GetStream();
                    if (dataLength < BufferSize)
                    {
                        stream.Write(dataBytesToSend, 0, dataLength);
                    }
                    else
                    {
                        int numOfBytesRemaining = dataLength;
                        int totalBytesSent = 0;
                        int temp_len = BufferSize;
                        byte[] tempData;
                        while (numOfBytesRemaining > 0)
                        {
                            tempData = new byte[temp_len];
                            Array.Copy(dataBytesToSend, totalBytesSent, tempData, 0, temp_len);
                            stream.Write(tempData, 0, temp_len);
                            numOfBytesRemaining -= temp_len;
                            totalBytesSent += temp_len;
                            temp_len = Math.Min(numOfBytesRemaining, BufferSize);
                        }

                    }
                    success = true;
                }
                else
                    success = false;
                

            }
            catch(Exception ex)
            {
                success = false;
                Debug.WriteLine("Failed to send data : " + ex.ToString());
                IsClientConnected = false;
                
                if(Client != null)
                {
                    Client.Close();
                    Client = null;
                }
                var t = Task.Run(() => StartListener());
            }
            return success;
        }

        private byte[] PrepareDataHeader(int len)
        {
            byte[] header = new byte[5];

            header[0] = StartByte;

            byte[] lengthBytes = BitConverter.GetBytes(len);

            lengthBytes.CopyTo(header, 1);

            return header;

        }

        public byte[] GetData()
        {
            try
            {
                if(Client == null)
                {
                    return null;
                }
                else
                {
                    NetworkStream stream = Client.GetStream();
                    byte[] tempData = new byte[BufferSize];
                    byte[] dataHeader = new byte[5];

                    using(MemoryStream mStream = new MemoryStream())
                    {
                        int numBytesRead = 0;
                        int TotalBytesRead = 0;
                        bool isFirstSampleRecieved = false;

                        int dataLength = 0;


                        while (true)
                        {
                            if (!isFirstSampleRecieved)
                            {
                                numBytesRead = stream.Read(dataHeader, 0, dataHeader.Length);
                                if(numBytesRead == dataHeader.Length)
                                {
                                    if (dataHeader[0] == StartByte)
                                    {
                                        dataLength = BitConverter.ToInt32(dataHeader, 1);
                                        isFirstSampleRecieved = true;
                                    }

                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if(dataLength < BufferSize)
                                {
                                    numBytesRead = stream.Read(tempData, 0, dataLength);
                                    TotalBytesRead += numBytesRead;

                                    mStream.Write(tempData, 0, dataLength);
                                }
                                else
                                {
                                    int temp_len = BufferSize;
                                    while(TotalBytesRead < dataLength)
                                    {
                                        numBytesRead = stream.Read(tempData, 0, temp_len);
                                        mStream.Write(tempData,0,numBytesRead);
                                        TotalBytesRead += numBytesRead;

                                        temp_len = Math.Min(dataLength - TotalBytesRead, BufferSize);
                                    }
                                }
                            }
                            if (TotalBytesRead >= dataLength)
                                break;
                        }
                        
                        if(TotalBytesRead == dataLength)
                        {
                            byte[] recievedBytes = mStream.ToArray();
                            return recievedBytes;
                        }
                        else
                        {
                            Debug.WriteLine("Recieved Bytes Are Not Correct!!!");
                            return null;
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error : " + ex.ToString());
                if (Client != null)
                {
                    Client.Close();
                    Client = null;
                }
                var t = Task.Run(() => StartListener());
                return null;
            }
        }
    }
}