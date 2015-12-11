using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ClientTCP
{
    public partial class Client : Form
    {
        private Socket m_clientSocket;
        private byte[] m_receiveBuffer = new byte[1024];
        public Client()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Client_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            string serverIP = txtIP.Text;
            int serverPort = Int32.Parse(txtPort.Text);
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            try
            {
                m_clientSocket.Connect(remoteEndPoint);
                if (m_clientSocket.Connected)
                {
                    m_clientSocket.BeginReceive(m_receiveBuffer, 0, m_receiveBuffer.Length, 0, new AsyncCallback(ReceiveCallBack), null);
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    this.AddRunningInfo(">>" + DateTime.Now.ToString() + "Client connect server success.");
                }
            }
            catch (Exception)
            {
                this.AddRunningInfo(">>" + DateTime.Now.ToString() + "Client connect server fail.");
                m_clientSocket = null;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (m_clientSocket != null)
            {
                m_clientSocket.Close();
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
                this.AddRunningInfo(">>" + DateTime.Now.ToString() + "Client disconnected.");
            }
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        private void btnSend_Click(object sender, EventArgs e)
        {
            string strSendData = txtSend.Text;
            byte[] sendBuffer = new byte[1024];
            sendBuffer = Encoding.Unicode.GetBytes(strSendData);
            if (m_clientSocket != null)
            {
                m_clientSocket.Send(sendBuffer);
            }
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int REnd = m_clientSocket.EndReceive(ar);
                string strReceiveData = Encoding.Unicode.GetString(m_receiveBuffer, 0, REnd);
                this.HandleMessage(strReceiveData);
                m_clientSocket.BeginReceive(m_receiveBuffer, 0, m_receiveBuffer.Length, 0, new AsyncCallback(ReceiveCallBack), null);
            }
            catch (Exception ex)
            {
              //  throw new Exception(ex.Message);
                this.AddRunningInfo(">>Exception :" +ex. Message);
            }
        }
        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// 
        private void HandleMessage(string message)
        {
            message = message.Replace("/0", "");
            if (!string.IsNullOrEmpty(message))
            {
                this.AddRunningInfo(">>Receive Data from server:" + message);
            }
        }

        private void AddRunningInfo(string message)
        {
            lstRunningInfo.BeginUpdate();
            lstRunningInfo.Items.Insert(0, message);
            if (lstRunningInfo.Items.Count > 100)
            {
                lstRunningInfo.Items.RemoveAt(100);
            }
            lstRunningInfo.EndUpdate();
        }
       

      
    }
}