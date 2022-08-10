using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPIP_ServerClient
{
    public class TCPIPClientMain
    {
        #region Public Variables
        public string ServerIP;
        public bool IsConnectedToServer { get; set; }
        public string ClientMessage { get; set; }


        public char ClientCommand { get; set; }


        public string ClientMessageReturn { get; set; }
        public int TargetPosition { get; set; }

        private string _clientMessage;
        private int _targetPosition;
        private bool _isConnectedToServer;

        private object Lck_LedColor = new object();
        private object Lck_ClientMessage = new object();
        private object Lck_TargetPosition = new object();
        private object Lck_IsConnectedToServer= new object();

        #endregion

        #region Parameters 

        //public readonly int Port = 38000;


        private readonly byte StartByte = (byte)'J';


        private int CommmunicationFrequency = 50;       /// Hz

        #endregion

        #region Private Variables

        public TCPIPClient Client;
        private Thread CommunicationThread;
        private double CommunicationPeriod;
        private bool ThreadEnabled = true;
        #endregion

        #region Indexes

        #region Client To Server
        private readonly int Index_ClientMessage = 0;
        #endregion
        #region Server To Client
        private readonly int Index_TargetPosition = 0;
        #endregion

        #endregion


        public TCPIPClientMain()
        {
            ServerIP = Constants.Constants.Ip;
        }
        private bool ConnectToServer()
        {
            Client = new TCPIPClient(port: Constants.Constants.Port, ip: ServerIP, StartByte: StartByte);
            ServerIP = Client.ConnectToServer();

            if (ServerIP == "")
            {
                return false;
            }
            else
                return true;
            Debug.WriteLine(String.Format("Client is connected to IP : {0}",ServerIP));
        }
        public void StartCommunicationThread()
        {
            ThreadEnabled = true;
            CommunicationPeriod = 1.0 / CommmunicationFrequency;
            CoreFcn(ClientCommand);
        }
        public void StopCommunication()
        {
            Client.DisconnectFromServer();
            Client = null;
            ServerIP = "";
            if (CommunicationThread != null)
            {
                ThreadEnabled = false;
                if (CommunicationThread.IsAlive)
                    CommunicationThread.Abort();
                CommunicationThread = null;
            }

        }
        private void CoreFcn(char clientCommand)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = ConnectToServer();

            if (success == false)
            {
                StopCommunication();
                return;
            }

            SendServerData(clientCommand);

            //byte[] a = new byte[239];
            //Client.Client.GetStream().Read(a,0,239);

            while (ThreadEnabled)
            {
                GetServerData();
                IsConnectedToServer = Client.IsConnectedToServer;
                while (watch.Elapsed.TotalSeconds < CommunicationPeriod)
                {
                    Thread.Sleep(1);
                }
            watch.Restart();
            }
        }
        private byte[] PrepareDataToBeSent(char clientcommand)
        {
            byte[] messageBytes = PrepareMessageBytes(clientcommand);
            return messageBytes;
        }

        private byte[] PrepareMessageBytes(char clientcommand)
        {
            int messageLen;
            byte[] messageData;
            if (string.IsNullOrEmpty(ClientMessage))
            {
                messageLen = 0;
                messageData = new byte[messageLen + 12];
            }
            else
            {
                byte[] messageBytes = Encoding.ASCII.GetBytes(ClientMessage);
                messageLen = messageBytes.Length;
                messageData = new byte[messageLen + 12]; 
                messageBytes.CopyTo(messageData, 12);
            }
            byte[] len1 = BitConverter.GetBytes((int)0);
            byte[] len2 = BitConverter.GetBytes((long) messageLen);

            len1.CopyTo(messageData, 0);
            len2.CopyTo(messageData, 4);

            //messageData[0] = (byte)0;

            //messageData[0] = (byte)clientcommand;
            //messageData[1] = (byte)(messageLen & 0xff);
            //messageData[2] = (byte)((messageLen >> 8) & 0xff);

            return messageData;
        }
        private void SendServerData(char clientcommand)
        {
            byte[] data = PrepareDataToBeSent(clientcommand);

            string test1 = Encoding.ASCII.GetString(data);

            Client.SendDataServer(data);
        }


        private void GetServerData()
        {
            byte[] data = Client.GetData();
            if (data != null)
            {
                AnalyzeReceivedData(data);

            }
            else
                IsConnectedToServer = false;
        }


        private void AnalyzeReceivedData(byte[] receivedData)
        {
            if (receivedData.Length > 5) /////////////
            {
                ThreadEnabled = false;
                AnalyzeReceivedData2(receivedData);


            }
        }
        private void AnalyzeReceivedData2(byte[] receivedData2)
        {
            byte[] MessageBytes;
            //int LenMessage = receivedData2[Index_TargetPosition] | (receivedData2[Index_TargetPosition + 1] << 8);
            if (true)
            {
                MessageBytes = new byte[receivedData2.Length];
                Array.Copy(receivedData2, 0, MessageBytes, 0, receivedData2.Length);
                ClientMessageReturn = Encoding.ASCII.GetString(MessageBytes);
            }
            else
                ClientMessageReturn = "";
        }
    }

}
