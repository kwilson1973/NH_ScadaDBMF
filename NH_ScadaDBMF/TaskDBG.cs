using System;
using TBC;
using HDR;
using SYS;

namespace SCADA_RX
{	
	public class TaskDBGForm : DBGForm
	{
		public event CCI_Form.ReceiveMsg_Handler				OnReceiveMsg;
		public event CCI_Form.ShowDBG_ReceiveData_Handler		OnShowDBG_ReceiveData;
		public event CCI_Form.Display_StringMessage_Handler		OnDisplay_StringMessage;

		public TaskDBGForm(string TaskName):base(TaskName)
		{
		}

		protected override void OnlineNotice()
		{
			// 連線通知			
		}

		public void Test()
		{
			this.ReceiveData();
		}

		protected override void ReceiveData()
		{
			lock(this)  //確保一次處理一筆資料
			{
				MSG msg = new MSG();
				msg = (MSG)this.ClientTask.MsgReadBufferDequeue();
				
				string str = msg.MsgToString();  //Msg轉成字串				
				this.OnShowDBG_ReceiveData(str, "InQueue");				
				this.OnReceiveMsg(msg);   //送至Queue內，再由Timer觸發送到後端處理(將收到之FRAWC翻譯成GENCODE)
				this.OnShowDBG_ReceiveData(str, "DeQueue");
			}//lock
		}

		protected override void DisconnectNotice()
		{
			// 連線中斷處理
			this.OnDisplay_StringMessage("CCI"+ " " + "INFO    - " + "與TBS中斷連線");
		}
	}
}
