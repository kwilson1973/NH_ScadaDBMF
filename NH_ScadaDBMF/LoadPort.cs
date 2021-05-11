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

		// 範例  G01 31 COM3 1200 8 NONE 1 NO 30
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
		internal ArrayList ByteAL;     //儲存讀取到之byte
		internal Queue ControlFcQue;  //儲存要送到現場的控制訊號內容
		internal string PortStatus;
		
		internal System.Timers.Timer AutoRecevieMessage_Timer;  //自動接收各ComPort之Timer
		internal System.Timers.Timer AutoPolling_Timer;  //自動輪詢之Timer		
		internal System.Timers.Timer AutoJudgeCF_Timer;  //判斷是否CF之Timer		
		internal System.TimeSpan myTimeSpan;  //存放時間變數



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
			//預設建構子
		}

		//自訂建構子
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
				// 各ComPort接收資料的Timer				
				this.AutoRecevieMessage_Timer = new System.Timers.Timer();   //每一個Comport會有一個對應讀Timer來讀取該Comport之資料
				this.AutoRecevieMessage_Timer.Interval = Int32.Parse(SYS.Profile.GetValue("ScadaDBMF.ini", "Param", "AutoReceive"));
				this.AutoRecevieMessage_Timer.Enabled = true;				
				this.AutoRecevieMessage_Timer.Elapsed += new System.Timers.ElapsedEventHandler(this.AutoReceviceMessage_Tick);

				// 各ComPort對現場Polling的Timer
				this.AutoPolling_Timer = new System.Timers.Timer();  //每一個Comport會有一個對應讀Timer來對現場作Polling的動作
				this.AutoPolling_Timer.Interval = Int32.Parse(SYS.Profile.GetValue("ScadaDBMF.ini", "Param", "AutoPolling"));
				this.AutoPolling_Timer.Enabled = false;			
				this.AutoPolling_Timer.Elapsed += new System.Timers.ElapsedEventHandler(AutoPolling_Tick);

				// 各ComPort計算CF的Timer
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

		#region AutoReceviceMessage 事件
		private void AutoReceviceMessage_Tick(object sender, System.Timers.ElapsedEventArgs e)
		{			
			byte[] bCommRead =  this.hnd2com.Read();  //到 ComPort 的 InQueue 內讀取資料
			if( bCommRead != null )
			{
				this.OnShowGEN_ReceiveData(this, "InQueue");
				this.OnValidateReceiveComMessage(bCommRead, this);  //檢查收到的資料
				this.OnShowGEN_ReceiveData(this, "DeQueue");
			}
		}
		#endregion

		#region AutoPolling 事件
		private void AutoPolling_Tick(object sender, System.Timers.ElapsedEventArgs e)  //對現場作Polling之內容
		{
			if(this.ControlFcQue.Count == 0)  //沒有資料要送出時
			{
				this.OnFB_MessageToVPI(this);				
			}			
			else  //有資料要送出時
			{
				this.OnFC_MessageToVPI(this);
			}			
		}
		#endregion

		#region AutoJudgeCF 事件
		public void AutoJudgeCF_Tick(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.OnJudge_CF(this);
		}
		#endregion

	}//class LoadPort
}//namespace CCI