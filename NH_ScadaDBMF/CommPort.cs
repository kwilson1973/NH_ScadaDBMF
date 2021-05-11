using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SCADA_RX
{	
	public class CommPort 
	{
		public int PortNum;
		public int BaudRate;
		public byte ByteSize;
		public byte Parity;      // 0, 1, 2, 3, 4 = none,odd,even,mark,space 
		public byte StopBits;    // 0,1,2 = 1, 1.5, 2 
		public byte FlowControl; // 0,1,2 = None, Hardware(CTS/RTS), Software(Xon/Xoff)

		public CommPort()
		{
			//預設建構子
		}
		public CommPort(int PortsSetting, int RateSetting, byte ByteSizeSetting, byte ParitySetting, byte StopBitsSetting, byte FlowControlSetting) //自訂建構子
		{
			this.PortNum = PortsSetting;
			this.BaudRate = RateSetting;
			this.ByteSize = ByteSizeSetting;
			this.Parity = ParitySetting;
			this.StopBits = StopBitsSetting;
			this.FlowControl = FlowControlSetting;
		}

		public int ReadTimeout;

		//comm port win32 file handle
		public int hComm = -1;

		public bool Opened = false;
         
		//win32 api constants
		private const uint GENERIC_READ = 0x80000000;
		private const uint GENERIC_WRITE = 0x40000000;
		private const int OPEN_EXISTING = 3;
		private const int INVALID_HANDLE_VALUE = -1;
		//DCB constants
		public uint DCbtn_Binary       = 0x00000001;           
		public uint DCbtn_ParityCheck  = 0x00000002;           
		public uint DCbtn_OutxCtsFlow  = 0x00000004; 
		public uint DCbtn_OutxDsrFlow  = 0x00000008;  
		public uint DCbtn_DtrControlMask      = 0x00000030;
		public uint DCbtn_DtrControlDisable   = 0x00000000; 
		public uint DCbtn_DtrControlEnable    = 0x00000010; 
		public uint DCbtn_DtrControlHandshake = 0x00000020; 
		public uint DCbtn_DsrSensitivity      = 0x00000040; 
		public uint DCbtn_TXContinueOnXoff    = 0x00000080; 
		public uint DCbtn_OutX         = 0x00000100;    
		public uint DCbtn_InX          = 0x00000200;           
		public uint DCbtn_ErrorChar    = 0x00000400;   
		public uint DCbtn_NullStrip    = 0x00000800;  
		public uint DCbtn_RtsControlMask      = 0x00003000;
		public uint DCbtn_RtsControlDisable   = 0x00000000;
		public uint DCbtn_RtsControlEnable    = 0x00001000;
		public uint DCbtn_RtsControlHandShake = 0x00002000;
		public uint DCbtn_RtsControlTogggle   = 0x00003000;
		public uint DCbtn_fAbortOnError       = 0x00004000;
		public uint DCbtn_Reserveds    = 0xFFFF8000; 

		//COMSTAT orginal structure
		[StructLayout(LayoutKind.Sequential)]
			public struct COMSTAT
		{
			public int Flags; // 32 bits for following flag
			//				public int fCtsHold;		// 1 bit, Tx waiting for CTS signal
			//				public int fDsrHold;		// 1 bit, Tx waiting for DSR signal
			//				public int fRlsdHold;		// 1 bit, Tx waiting for RLSD signal
			//				public int fXoffHold;		// 1 bit, Tx waiting for XOFF char rec'd
			//				public int fXoffSent;		// 1 bit, Tx waiting for XOFF char sent
			//				public int fEof;		    // 1 bit, EOF character sent
			//				public int fTxim;		    // 1 bit, character waiting for Tx
			//				public int fReserved;       //25 bits,  reserved
			public int cbInQue;		//bytes in input buffer
			public int cbOutQue;	//bytes in output buffer
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct DCB 
		{
			//taken from c struct in platform sdk 
			public int DCBlength;           // sizeof(DCB) 
			public int BaudRate;            // current baud rate

			//--------- these are the c struct bit fields, bit twiddle flag to set
			public uint Flags;    // 32 bits for following flags
			//				public int fBinary;           // 1 bit,  binary mode, no EOF check 
			//				public int fParityCheck;      // 1 bit,  enable parity checking 
			//				public int fOutxCtsFlow;      // 1 bit,  CTS output flow control 
			//				public int fOutxDsrFlow;      // 1 bit,  DSR output flow control 
			//				public int fDtrControl;       // 2 bits, DTR flow control type 
			//				public int fDsrSensitivity;   // 1 bit,  DSR sensitivity 

			//				public int fTXContinueOnXoff; // 1 bit,  XOFF continues Tx 
			//				public int fOutX;             // 1 bit,  XON/XOFF out flow control 
			//				public int fInX;              // 1 bit,  XON/XOFF in flow control 
			//				public int fErrorChar;        // 1 bit,  enable error replacement 
			//				public int fNull;             // 1 bit,  enable null stripping 
			//				public int fRtsControl;       // 2 bit,  RTS flow control 
			//				public int fAbortOnError;     // 1 bit,  abort on error 
			//				public int fDummy2;           //17 bits, reserved 
			//----------End of c struct bit fields
			public ushort wReserved;        // not currently used 
			
			public ushort XonLim;           // transmit XON threshold 
			public ushort XoffLim;          // transmit XOFF threshold 
			public byte ByteSize;           // number of bits/byte, 4-8 
			public byte Parity;             // 0-4 = none,odd,even,mark,space 
			public byte StopBits;           // 0,1,2 = 1, 1.5, 2 

			public byte XonChar;            // Tx and Rx XON character 
			public byte XoffChar;           // Tx and Rx XOFF character 
			public byte ErrorChar;          // error replacement character 

			public byte EofChar;            // end of input character 
			public byte EvtChar;            // received event character 
			public ushort wReserved1;         // reserved; do not use 
		}

		[StructLayout(LayoutKind.Sequential)]
			private struct COMMTIMEOUTS 
		{
			public int ReadIntervalTimeout; 
			public int ReadTotalTimeoutMultiplier; 
			public int ReadTotalTimeoutConstant; 
			public int WriteTotalTimeoutMultiplier; 
			public int WriteTotalTimeoutConstant; 
		}     

		[StructLayout(LayoutKind.Sequential)]
			private struct OVERLAPPED 
		{ 
			public int  Internal; 
			public int  InternalHigh; 
			public int  Offset; 
			public int  OffsetHigh; 
			public int hEvent; 
		}

		[DllImport("kernel32.dll")]
		private static extern int CreateFile(
			string lpFileName,                        // file name
			uint dwDesiredAccess,                     // access mode
			int dwShareMode,                          // share mode
			int lpSecurityAttributes,                 // SD
			int dwCreationDisposition,                // how to create
			int dwFlagsAndAttributes,                 // file attributes
			int hTemplateFile                         // handle to template file
			);

		//
		// 920303 Hsinson 新增此程式 用意在可以立即丟出資料而不要存在Buffer內
		//
		[DllImport("kernel32.dll")]
		private static extern bool FlushFileBuffers(
			int hFile  // handle to file
			);
		//
		//  end
		//

		[DllImport("kernel32.dll")]
		private static extern bool GetCommState(
			int hFile,     // handle to communications device
			ref DCB lpDCB  // device-control block
			);
		[DllImport("kernel32.dll")]
		private static extern bool BuildCommDCB(
			string lpDef,    // device-control string
			ref DCB lpDCB    // device-control block
			);
		[DllImport("kernel32.dll")]
		private static extern bool SetCommState(
			int hFile,      // handle to communications device
			ref DCB lpDCB   // device-control block
			);
		[DllImport("kernel32.dll")]
		private static extern bool GetCommTimeouts(
			int hFile,                  // handle to comm device
			ref COMMTIMEOUTS lpCommTimeouts  // time-out values
			);
		[DllImport("kernel32.dll")]
		private static extern bool SetCommTimeouts(
			int hFile,                  // handle to comm device
			ref COMMTIMEOUTS lpCommTimeouts  // time-out values
			);
		[DllImport("kernel32.dll")]
		private static extern bool ReadFile(
			int hFile,                   // handle to file
			byte[] lpBuffer,             // data buffer
			int nNumberOfBytesToRead,    // number of bytes to read
			ref int lpNumberOfBytesRead, // number of bytes read
			ref OVERLAPPED lpOverlapped  // overlapped buffer
			);
		[DllImport("kernel32.dll")]
		private static extern bool WriteFile(
			int hFile,                      // handle to file
			byte[] lpBuffer,                // data buffer
			int nNumberOfBytesToWrite,      // number of bytes to write
			ref int lpNumberOfBytesWritten, // number of bytes written
			ref OVERLAPPED lpOverlapped     // overlapped buffer
			);

		[DllImport("kernel32.dll")]
		private static extern bool ClearCommError(
			int hFile,				//handle to communications device
			ref int lpErrors,		//pointer to variable to receive error codes
			ref COMSTAT lpStat		//pointer to buffer for communications status
			);

		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(
			int hObject   // handle to object
			);
		[DllImport("kernel32.dll")]
		private static extern uint GetLastError();

		[DllImport("kernel32.dll")]
		private static extern bool PurgeComm(
			int hFile,		//handle tocommunications device
			int dwFlags		//extended function to perform 
			);

		[DllImport("kernel32.dll")]
		private static extern bool EscapeCommFunction(
			int hFile,
			int dwFunc
			);

		[DllImport("kernel32.dll")]
		private static extern bool SetCommMask(
			int hFile,
			int dwEvtMask
			);

		[DllImport("kernel32.dll")]
		private static extern bool WaitCommEvent(
			int hFile,
			ref int lpEvtMask,
			ref OVERLAPPED lpOverlapped
			);

		[DllImport("kernel32.dll")]
		private static extern bool GetCommModemStatus(
			int hFiile,
			ref int lpModemStat
			);

		public bool Open()
		{
			DCB dcbCommPort = new DCB();
			COMMTIMEOUTS ctoCommPort = new COMMTIMEOUTS();			

			// OPEN THE COMM PORT.			
			hComm = CreateFile("\\\\.\\COM" + PortNum, GENERIC_READ | GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0);
		
			// IF THE PORT CANNOT BE OPENED, BAIL OUT.
			if(hComm == INVALID_HANDLE_VALUE) 
			{
				//MessageBox.Show("CreateFile: Create Comm port Error");
				return false;
				//				throw(new ApplicationException("Comm Port Can Not Be Opened"));
			}
			else
			{
			
				// SET BAUD RATE, PARITY, WORD SIZE, AND STOP BITS.
				GetCommState(hComm, ref dcbCommPort);
				dcbCommPort.BaudRate=BaudRate;

				//	dcbCommPort.Flags=0x00;
				//  Set DCB.fBinary=1;
				dcbCommPort.Flags |= DCbtn_Binary;

				//  Set dcb.fParity=1
				dcbCommPort.Flags |= DCbtn_ParityCheck;

				dcbCommPort.Parity = Parity;

				dcbCommPort.ByteSize = ByteSize;
				dcbCommPort.StopBits = StopBits;
				//設定流量管制
				switch (FlowControl)
				{
					case 0: //無NONE
						dcbCommPort.Flags &= ~DCbtn_OutX;  //Disable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_InX;   //Disable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_RtsControlHandShake; //Disable Hardware RTS control
						dcbCommPort.Flags &= ~DCbtn_OutxCtsFlow;         //Disable Hardware CTS control
						break;
					case 1: //硬體Hardware (RTS/CTS) Control
						dcbCommPort.Flags &= ~DCbtn_OutX;//Disable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_InX; //Disable Software Xon/Xoff Control
						dcbCommPort.Flags |= DCbtn_RtsControlHandShake; //Enable Hardware RTS control
						dcbCommPort.Flags |= DCbtn_OutxCtsFlow;         //Enable Hardware CTS control
					
						break;
					case 2: //軟體Software (Xon/Xoff) Control
						dcbCommPort.Flags |= DCbtn_OutX; //Enable Software Xon/Xoff Control
						dcbCommPort.Flags |= DCbtn_InX;  //Enable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_RtsControlHandShake; //Disable Hardware RTS control
						dcbCommPort.Flags &= ~DCbtn_OutxCtsFlow;         //Disable Hardware CTS control
						break;
					default:
						dcbCommPort.Flags &= ~DCbtn_OutX;  //Disable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_InX;   //Disable Software Xon/Xoff Control
						dcbCommPort.Flags &= ~DCbtn_RtsControlHandShake; //Disable Hardware RTS control
						dcbCommPort.Flags &= ~DCbtn_OutxCtsFlow;         //Disable Hardware CTS control
						break;
				}//switch (FlowControl)
			
				if (!SetCommState(hComm, ref dcbCommPort))
				{
					GetCommState(hComm, ref dcbCommPort);
					MessageBox.Show("SetCommState Error code:" + String.Format("{0}",GetLastError()));
					//				throw(new ApplicationException("SetCommState Error"));
					this.Close(); //Close hComm
				}//if (!SetCommState(hComm, ref dcbCommPort))
				else
				{
					//unremark to see if setting took correctly
					//			DCB dcbCommPort2 = new DCB();
					//			GetCommState(hComm, ref dcbCommPort2);
					Opened = true;			
				}//else if(!SetCommState(hComm, ref dcbCommPort))
			}//else if(hComm == INVALID_HANDLE_VALUE) 
			return Opened;
		}//...............................................public bool Open()


		public void Close() 
		{
			if (hComm!=INVALID_HANDLE_VALUE) 
			{
				CloseHandle(hComm);
				Opened = false;
			}
		}

		public byte[] Read() 
		{
			byte[] inbuf;
			//			byte[] OutBytes;
			//			BufBytes = new byte[NumBytes];
			int dwError=0;
			COMSTAT cs = new COMSTAT();
			ClearCommError(hComm, ref dwError, ref cs);
			if (cs.cbInQue == 0) return (null);
			inbuf = new byte[cs.cbInQue];
			if (hComm != INVALID_HANDLE_VALUE) 
			{
				OVERLAPPED ovlCommPort = new OVERLAPPED();
				int BytesRead = 0;
				ReadFile(hComm, inbuf, cs.cbInQue, ref BytesRead, ref ovlCommPort);
				//			inbuf[cs.cbInQue] = 0;
				//			OutBytes = new byte[BytesRead];
				//			Array.Copy(inbuf,OutBytes,BytesRead);
			} 
			else 
			{
				//throw(new ApplicationException("Comm Port Not Open"));
				MessageBox.Show("Read: Comm Port Not Open! " + String.Format("{0}",GetLastError()));
			}
			//			return OutBytes;
			return inbuf;
		}

		public void Write(byte[] WriteBytes) 
		{
			if (hComm!=INVALID_HANDLE_VALUE) 
			{
				OVERLAPPED ovlCommPort = new OVERLAPPED();
				int BytesWritten = 0;
				WriteFile(hComm,WriteBytes,WriteBytes.Length,ref BytesWritten,ref ovlCommPort);

				//
				//  920303 Hsinson 新增此程式 用意在可以立即丟出資料而不要存在Buffer內
				//
				FlushFileBuffers(hComm);				
			}
			else 
			{
				//throw(new ApplicationException("Comm Port Not Open"));
				MessageBox.Show("Write: Comm Port Not Open!" + String.Format("{0}",GetLastError()));
			}
		}
	}

}
