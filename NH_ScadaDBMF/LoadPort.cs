using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Timers;
using SYS;

namespace SCADA_RX
{
	public class CF_Count
	{
		public DateTime Time;
		public ushort CFstatus;

		public CF_Count()
		{
			this.Time = DateTime.Now;
			this.CFstatus = 0x0000;
		}
	}

	public class LoadPort
	{
		public event CCI_Form.ValidateReceiveComMessage_Handler OnValidateReceiveComMessage;
		public event CCI_Form.FB_MessageToVPI_Handler			OnFB_MessageToVPI;
		public event CCI_Form.FC_MessageToVPI_Handler			OnFC_MessageToVPI;
		public event CCI_Form.Judge_CF_Handler					OnJudge_CF;
		public event CCI_Form.ShowGEN_ReceiveData_Handler		OnShowGEN_ReceiveData;

		// �d��  G01 31 COM3 1200 8 NONE 1 NO 30
		internal string LocName;   // G01
		internal int LocNum;       // 31
		internal int ComPort;      // COM3
		internal int BaudRate;     // 1200
		internal byte ByteSize;    // 8
		internal byte Parity;      // NONE
		internal byte StopBits;    // 1
		internal byte FlowControl; // NO
		internal int ReadTimeout;  // 30	

		//SCADA
		internal string strEncodingCode;
		internal string strBuf;
		//SCADA
        
		internal CommPort hnd2com;
		internal ArrayList ByteAL;     //�x�sŪ���줧byte
		internal Queue ControlFcQue;  //�x�s�n�e��{��������T�����e
		internal string PortStatus;
		
		internal System.Timers.Timer AutoRecevieMessage_Timer;  //�۰ʱ����UComPort��Timer
		internal System.Timers.Timer AutoPolling_Timer;  //�۰ʽ��ߤ�Timer		
		internal System.Timers.Timer AutoJudgeCF_Timer;  //�P�_�O�_CF��Timer		
		internal System.TimeSpan myTimeSpan;  //�s��ɶ��ܼ�



		//SCADA
		public string myBuf
		{
			get{ return this.strBuf;}
			set{this.strBuf=value;}
		}

		public string EncodingCode
		{
			get{ return this.strEncodingCode;}
			set{this.strEncodingCode=value;}
		}



		//SCADA


		public LoadPort()
		{	
			//�w�]�غc�l
		}

		//�ۭq�غc�l
		public LoadPort(string LocNameSetting, int LocNumSetting, int PortsSetting, int RateSetting, byte ByteSizeSetting, byte ParitySetting, byte StopBitsSetting, byte FlowControlSetting, int ReadTimeoutSetting, string vstrEncodingCode)
		{
			this.LocName = LocNameSetting;
			this.LocNum = LocNumSetting;			
			this.ComPort = PortsSetting;
			this.BaudRate = RateSetting;
			this.ByteSize = ByteSizeSetting;
			this.Parity = ParitySetting;
			this.StopBits = StopBitsSetting;
			this.FlowControl = FlowControlSetting;
			this.ReadTimeout = ReadTimeoutSetting;
			this.hnd2com = new CommPort(this.ComPort, this.BaudRate, this.ByteSize, this.Parity, this.StopBits, this.FlowControl);
			this.ByteAL = new ArrayList();
			this.ControlFcQue = Queue.Synchronized(new Queue());
			this.PortStatus = "Close";
			this.myBuf="";
			this.EncodingCode=vstrEncodingCode;

			try
			{
				// �UComPort������ƪ�Timer				
				this.AutoRecevieMessage_Timer = new System.Timers.Timer();   //�C�@��Comport�|���@�ӹ���ŪTimer��Ū����Comport�����
				this.AutoRecevieMessage_Timer.Interval = Int32.Parse(SYS.Profile.GetValue("ScadaDBMF.ini", "Param", "AutoReceive"));
				this.AutoRecevieMessage_Timer.Enabled = true;				
				this.AutoRecevieMessage_Timer.Elapsed += new System.Timers.ElapsedEventHandler(this.AutoReceviceMessage_Tick);

				// �UComPort��{��Polling��Timer
				this.AutoPolling_Timer = new System.Timers.Timer();  //�C�@��Comport�|���@�ӹ���ŪTimer�ӹ�{���@Polling���ʧ@
				this.AutoPolling_Timer.Interval = Int32.Parse(SYS.Profile.GetValue("ScadaDBMF.ini", "Param", "AutoPolling"));
				this.AutoPolling_Timer.Enabled = false;			
				this.AutoPolling_Timer.Elapsed += new System.Timers.ElapsedEventHandler(AutoPolling_Tick);

				// �UComPort�p��CF��Timer
				this.AutoJudgeCF_Timer = new System.Timers.Timer();
				this.AutoJudgeCF_Timer.Interval = Int32.Parse(SYS.Profile.GetValue("ScadaDBMF.ini", "Param", "AutoCFJudge"));
				this.AutoJudgeCF_Timer.Enabled = true;				
				this.AutoJudgeCF_Timer.Elapsed += new System.Timers.ElapsedEventHandler(AutoJudgeCF_Tick);			
			}
			catch
			{
				this.AutoRecevieMessage_Timer.Interval = 800;
				this.AutoPolling_Timer.Interval = 1000;
				this.AutoJudgeCF_Timer.Interval = 5000;
			}
		}

		#region AutoReceviceMessage �ƥ�
		private void AutoReceviceMessage_Tick(object sender, System.Timers.ElapsedEventArgs e)
		{			
			byte[] bCommRead =  this.hnd2com.Read();  //�� ComPort �� InQueue ��Ū�����
			if( bCommRead != null )
			{
				this.OnShowGEN_ReceiveData(this, "InQueue");
				this.OnValidateReceiveComMessage(bCommRead, this);  //�ˬd���쪺���
				this.OnShowGEN_ReceiveData(this, "DeQueue");
			}
		}
		#endregion

		#region AutoPolling �ƥ�
		private void AutoPolling_Tick(object sender, System.Timers.ElapsedEventArgs e)  //��{���@Polling�����e
		{
			if(this.ControlFcQue.Count == 0)  //�S����ƭn�e�X��
			{
				this.OnFB_MessageToVPI(this);				
			}			
			else  //����ƭn�e�X��
			{
				this.OnFC_MessageToVPI(this);
			}			
		}
		#endregion

		#region AutoJudgeCF �ƥ�
		public void AutoJudgeCF_Tick(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.OnJudge_CF(this);
		}
		#endregion

	}//class LoadPort
}//namespace CCI