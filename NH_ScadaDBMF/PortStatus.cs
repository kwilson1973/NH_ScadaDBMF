using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SCADA_RX
{
	/// <summary>
	/// PortStatus 的摘要描述。
	/// </summary>
	public class PortStatus : System.Windows.Forms.Form
	{
		public event CCI_Form.Reload_PortStatus_Handler OnReload_PortStatus;

		private System.Windows.Forms.ColumnHeader LocName;
		private System.Windows.Forms.ColumnHeader LocNum;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ColumnHeader ComStatus;
		private System.Windows.Forms.ListView LV_PortStatus;
		private System.Windows.Forms.Button btn_Close;
		private System.Windows.Forms.ColumnHeader ComNo;
		private System.Windows.Forms.ColumnHeader BaudRate;
		private System.Windows.Forms.ColumnHeader ByteSize;
		private System.Windows.Forms.ColumnHeader Parity;
		private System.Windows.Forms.ColumnHeader StopBits;
		private System.Windows.Forms.ColumnHeader FlowControl;
		private System.Windows.Forms.ColumnHeader ReadTimeout;
		private System.Windows.Forms.Button btn_Reload;
		
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PortStatus()
		{
			//
			// Windows Form 設計工具支援的必要項
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 呼叫之後加入任何建構函式程式碼
			//
		}

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// 此為設計工具支援所必需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
			this.LV_PortStatus = new System.Windows.Forms.ListView();
			this.LocName = new System.Windows.Forms.ColumnHeader();
			this.LocNum = new System.Windows.Forms.ColumnHeader();
			this.ComNo = new System.Windows.Forms.ColumnHeader();
			this.ComStatus = new System.Windows.Forms.ColumnHeader();
			this.BaudRate = new System.Windows.Forms.ColumnHeader();
			this.ByteSize = new System.Windows.Forms.ColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btn_Close = new System.Windows.Forms.Button();
			this.Parity = new System.Windows.Forms.ColumnHeader();
			this.StopBits = new System.Windows.Forms.ColumnHeader();
			this.FlowControl = new System.Windows.Forms.ColumnHeader();
			this.ReadTimeout = new System.Windows.Forms.ColumnHeader();
			this.btn_Reload = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// LV_PortStatus
			// 
			this.LV_PortStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.LocName,
																							this.LocNum,
																							this.ComNo,
																							this.ComStatus,
																							this.BaudRate,
																							this.ByteSize,
																							this.Parity,
																							this.StopBits,
																							this.FlowControl,
																							this.ReadTimeout});
			this.LV_PortStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LV_PortStatus.HoverSelection = true;
			this.LV_PortStatus.Name = "LV_PortStatus";
			this.LV_PortStatus.Size = new System.Drawing.Size(648, 256);
			this.LV_PortStatus.TabIndex = 1;
			this.LV_PortStatus.View = System.Windows.Forms.View.Details;
			// 
			// LocName
			// 
			this.LocName.Text = "LocName";
			// 
			// LocNum
			// 
			this.LocNum.Text = "LocNum";
			// 
			// ComNo
			// 
			this.ComNo.Text = "ComNo.";
			// 
			// ComStatus
			// 
			this.ComStatus.Text = "ComStatus";
			this.ComStatus.Width = 70;
			// 
			// BaudRate
			// 
			this.BaudRate.Text = "BaudRate";
			// 
			// ByteSize
			// 
			this.ByteSize.Text = "ByteSize";
			// 
			// panel1
			// 
			this.panel1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.LV_PortStatus});
			this.panel1.Location = new System.Drawing.Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(648, 256);
			this.panel1.TabIndex = 2;
			// 
			// btn_Close
			// 
			this.btn_Close.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btn_Close.Location = new System.Drawing.Point(576, 272);
			this.btn_Close.Name = "btn_Close";
			this.btn_Close.Size = new System.Drawing.Size(75, 24);
			this.btn_Close.TabIndex = 3;
			this.btn_Close.Text = "關閉視窗";
			this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
			// 
			// Parity
			// 
			this.Parity.Text = "Parity";
			// 
			// StopBits
			// 
			this.StopBits.Text = "StopBits";
			// 
			// FlowControl
			// 
			this.FlowControl.Text = "FlowControl";
			this.FlowControl.Width = 75;
			// 
			// ReadTimeout
			// 
			this.ReadTimeout.Text = "ReadTimeout";
			this.ReadTimeout.Width = 75;
			// 
			// btn_Reload
			// 
			this.btn_Reload.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btn_Reload.Location = new System.Drawing.Point(496, 272);
			this.btn_Reload.Name = "btn_Reload";
			this.btn_Reload.Size = new System.Drawing.Size(75, 24);
			this.btn_Reload.TabIndex = 4;
			this.btn_Reload.Text = "重新整理";
			this.btn_Reload.Click += new System.EventHandler(this.btn_Reload_Click);
			// 
			// PortStatus
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(672, 302);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btn_Reload,
																		  this.btn_Close,
																		  this.panel1});
			this.Name = "PortStatus";
			this.Text = "PortStatus";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void ShowPortStatus(LoadPort[] ports)
		{
			this.LV_PortStatus.Items.Clear();
			foreach(LoadPort a in ports)
			{
				string tmpParity;
				string tmpStopBits;
				string tmpFlowControl;
				int tmpReadTimeout;

				if(a != null)
				{
					
					switch(a.Parity.ToString())
					{
						case "0":
							tmpParity = "NONE";
							break;
						case "1":
							tmpParity = "ODD";
							break;
						case "2":
							tmpParity = "EVEN";
							break;
						case "3":
							tmpParity = "MARK";
							break;
						case "4":
							tmpParity = "SPACE";
							break;
						default :
							tmpParity = "ERROR!!";
							break;
					}
					switch(a.StopBits.ToString())
					{
						case "0":
							tmpStopBits = "1";
							break;
						case "1":
							tmpStopBits = "1.5";
							break;
						case "2":
							tmpStopBits = "2";
							break;
						default :	
							tmpStopBits = "ERROR!!";
							break;
					}					
					switch(a.FlowControl.ToString())
					{
						case "0":
							tmpFlowControl = "NO";
							break;
						case "1":
							tmpFlowControl = "HW";
							break;
						case "2":
							tmpFlowControl = "SW";
							break;
						default :
							tmpFlowControl = "ERROR!!";
							break;
					}
					tmpReadTimeout = a.ReadTimeout/1000;

					string[] itemArray = {a.LocName, a.LocNum.ToString(), "COM" + a.ComPort.ToString(), a.PortStatus, a.BaudRate.ToString(),
										  a.ByteSize.ToString(), tmpParity, tmpStopBits, tmpFlowControl, tmpReadTimeout.ToString()};
					ListViewItem Item = new ListViewItem(itemArray);
					if(a.PortStatus == "Close")
					{
						Item.ForeColor = Color.Red;
					}
					else
					{
						Item.ForeColor = Color.Black;
					}
					this.LV_PortStatus.Items.Add(Item);					
				}
			}
			this.LV_PortStatus.Refresh();
		}

		private void btn_Close_Click(object sender, System.EventArgs e)
		{
			this.Hide();
		}

		private void btn_Reload_Click(object sender, System.EventArgs e)
		{
			this.OnReload_PortStatus();
		}
	}
}
