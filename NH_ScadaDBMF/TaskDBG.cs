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
			// �s�u�q��			
		}

		public void Test()
		{
			this.ReceiveData();
		}

		protected override void ReceiveData()
		{
			lock(this)  //�T�O�@���B�z�@�����
			{
				MSG msg = new MSG();
				msg = (MSG)this.ClientTask.MsgReadBufferDequeue();
				
				string str = msg.MsgToString();  //Msg�ন�r��				
				this.OnShowDBG_ReceiveData(str, "InQueue");				
				this.OnReceiveMsg(msg);   //�e��Queue���A�A��TimerĲ�o�e���ݳB�z(�N���줧FRAWC½Ķ��GENCODE)
				this.OnShowDBG_ReceiveData(str, "DeQueue");
			}//lock
		}

		protected override void DisconnectNotice()
		{
			// �s�u���_�B�z
			this.OnDisplay_StringMessage("CCI"+ " " + "INFO    - " + "�PTBS���_�s�u");
		}
	}
}
