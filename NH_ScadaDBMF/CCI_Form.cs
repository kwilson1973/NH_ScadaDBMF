using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using TBC;
using SYS;
using System.Data.SqlClient;

using ScadaRXM.Module;


namespace SCADA_RX
{
    /// <summary>
    /// Form1 ���K�n�y�z�C
    /// </summary>
    public class CCI_Form : System.Windows.Forms.Form
    {
        #region �ŧiTaskDBGForm
        public TaskDBGForm TaskDBG;
        #endregion

        private StreamWriter LogFileWriter;  //�g LogFile �� StreamWriter
        private StreamReader PlayBackLogFile;//PlayBack��LogFile
        private StreamWriter MonFileWriter;  //�g MonFile �� StreamWriter
        private float speed;
        private int PlayBackOffset;
        private Thread PlayBackThread;
        internal string TaskName;  //TaskNaem
        internal string Filename;  //FileName
        private int LocIndexLength;
        private string LogFilePath;
        private string LogFileName;
        private string LogFileExt;
        private string LogFileFullName;
        private string MonFilePath;  //Hsisnon970528 add
        private string MonFileName;
        private string MonFileExt;
        private string MonFileFullName;
        private int MonAlarmCount = 0;
        public byte[] PollingToVPI;
        public byte[] AckToVPI;
        public byte[] RecallFromVPI;
        private int DBG_ReceiveMessageCounter;  //ReceiveDBG_Message�p�ƾ�
        private int GEN_ReceiveMessageCounter;  //ReceiveGEN_Message�p�ƾ�
        private LoadPort[] myLoadPort;  //�x�s�UComPort���}�C
        private CF_Count[] myCF_Count;  //�x�s�UComPort�O�_CF���}�C
        private PortStatus myPortStatus;
        private string tLocName;
        private int tLocNum;
        private int tComPort;
        private int tBaudRate;
        private byte tByteSize;
        private byte tParity;
        private byte tStopBits;
        private byte tFlowControl;
        private int tReadTimeout;

        public static bool bDebug { private set; get; }

        //SCADA
        private string tEncodingCode;
        //SCADA

        private TreeNode RootNode;
        private TreeNode PortNode;
        private string RAWType;
        private string SafeButtom;
        private bool AutoPolling;
        private string Simulation;
        private bool AutoPack;
        private bool AutoComPortOpen; //930628 Hsinson�s�W�ҰʧY�۰ʶ}�_Com-port�ö}�l�郞���s�u
        private Queue ReceiveMsgQue;
        private Queue SyncdSendToTBCQueue;
        private System.Timers.Timer DoMessageTimer;
        private System.Timers.Timer SendMsgToTBC_Timer;
        private int DoMsgLimite;
        private int SendToTBCLimite;
        private OpenFileDialog myOpenFileDialog;
        private string MultipleStr;
        //GSS ScadaDBMF
        //string crln="\r\n";  Hsinson modify to char array
        char[] crln = { '\r', '\n', '\0' };
        string cr = "\r";
        string ln = "\n";
        private string strSourceSQLServer_IP;
        private string strSourceSQLServer_DataBase;
        private string strSourceSQLServer_User;
        private string strSourceSQLServer_Password;
        private string strSourceSQLServer_RXTable;


        private string strLocalSQLServer_IP;
        private string strLocalSQLServer_DataBase;
        private string strLocalSQLServer_User;
        private string strLocalSQLServer_Password;
        private string strLocalSQLServer_RXTable;
        private bool blnLocalDBEnable;
        private string strLocalSQLServer_Ov_LineTable;


        // RTDB (One day)  Jacky_su 980504 add
        private string m_strLocalSQLServer_OneDay_IP;
        private string m_strLocalSQLServer_OneDay_DataBase;
        private string m_strLocalSQLServer_OneDay_User;
        private string m_strLocalSQLServer_OneDay_Password;
        private string m_strLocalSQLServer_OneDay_RXTable;
        private bool m_blnLocalDB_OneDay_Enable;

        // MainDB (One day) Jacky_su 980504 add
        private string m_strMainDBSQLServer_OneDay_IP;
        private string m_strMainDBSQLServer_OneDay_DataBase;
        private string m_strMainDBSQLServer_OneDay_User;
        private string m_strMainDBSQLServer_OneDay_Password;
        private string m_strMainDBSQLServer_OneDay_RXTable;
        private bool m_blnMainDBEnable_OneDay;


        private string strMainDBSQLServer_IP;
        private string strMainDBSQLServer_DataBase;
        private string strMainDBSQLServer_User;
        private string strMainDBSQLServer_Password;
        private string strMainDBSQLServer_RXTable;
        private bool blnMainDBEnable;

        private string strAlarmDBSQLServer_IP;
        private string strAlarmDBSQLServer_DataBase;
        private string strAlarmDBSQLServer_User;
        private string strAlarmDBSQLServer_Password;
        private string strAlarmDBSQLServer_RXTable;
        //20181025 add
        private string strAlarmDBSQLServer_RXTableTemp;
        private bool blnAlarmDBEnable;

        private string strItlDBSQLServer_IP;
        private string strItlDBSQLServer_DataBase;
        private string strItlDBSQLServer_User;
        private string strItlDBSQLServer_Password;
        private bool blnItlDBDBEnable;
        //	private bool blnItlDBEnable;

        private string strNetDCDBSQLServer_IP;
        private string strNetDCDBSQLServer_DataBase;
        private string strNetDCDBSQLServer_User;
        private string strNetDCDBSQLServer_Password;
        private string strNetDCDBSQLServer_Table;
        private string strNetDCDBSQLServer_Table_LastOne; //Hsinson960813 �s�̷s��NetDC�ˬd���G
        private bool blnNetDCDBEnable;
        private bool blnNetDCDBEnable_History; //Hsinson960813 add
        private bool blnNetDCDBEnable_LastOne; //Hsinson960813 add

        //Hsinson961009 add AtsEvent
        private string strAtsEventDBSQLServer_IP;
        private string strAtsEventDBSQLServer_DataBase;
        private string strAtsEventDBSQLServer_User;
        private string strAtsEventDBSQLServer_Password;
        private string strAtsEventDBSQLServer_Table;
        private string strAtsEventDBSQLServer_Table_ExtName; //Hsinson961017 ��ƪ��W�٤������W�r(�~���)
        private bool blnAtsEventDBEnable;
        private int AtsEventErrorCount = 0;
        private string strAtsTwcDBSQLServer_Table = "TWC";  //Hsinson970219 add
        private bool blnAtsTwcDBEnable;

        //Hsinson961227 add WinSysEvt
        //Wilson1090920 Mod for NH
        private string strWinSysEvtDBSQLServer_IP;
        private string strWinSysEvtDBSQLServer_DataBase;
        private string strWinSysEvtDBSQLServer_User;
        private string strWinSysEvtDBSQLServer_Password;
        private string strWinSysEvtDBSQLServer_Table;
        private string strWinSysEvtDBSQLServer_Table_ExtName; //Hsinson961017 ��ƪ��W�٤������W�r(�~���)
        private bool blnWinSysEvtDBEnable;
        private int WinSysEvtErrorCount = 0;
        private decimal WinSysEvtDBMaxSeq = 0;

        //Wilson1090927 add TLOS_NH
        private string strTLOS_NHDBSQLServer_IP;
        private string strTLOS_NHDBSQLServer_DataBase;
        private string strTLOS_NHDBSQLServer_User;
        private string strTLOS_NHDBSQLServer_Password;
        private string strTLOS_NHDBSQLServer_Table;
        private string strTLOS_NHDBSQLServer_Table_ExtName;
        private bool blnTLOS_NHDBEnable;
        private int WinTLOS_NHErrorCount = 0;
        private decimal TLOS_NHDBMaxSeq = 0;

        //Hsinson961227 add PMS Process Monitor Statue
        private string strPmsDBSQLServer_IP;
        private string strPmsDBSQLServer_DataBase;
        private string strPmsDBSQLServer_User;
        private string strPmsDBSQLServer_Password;
        private string strPmsDBSQLServer_Table;
        private string strPmsDBSQLServer_Table_LastOne; //Hsinson960813 �s�̷s��Pms�ˬd���G
        private bool blnPmsDBEnable;
        private bool blnPmsDBEnable_History; //Hsinson960813 add
        private bool blnPmsDBEnable_LastOne; //Hsinson960813 add

        //Kelvin, 2010/12/24
        private string strDBSpaceDBSQLServer_DataBase;
        private string strDBSpaceDBSQLServer_IP;
        private string strDBSpaceDBSQLServer_Table;
        private bool blnDBSpaceDBEnable;
        private string strDBSpaceDBSQLServer_User;
        private string strDBSpaceDBSQLServer_Password;
        private string strDBSpaceDBSQLServer_Table_LastOne;
        private bool blnDBSpaceDBEnable_History;
        private bool blnDBSpaceDBEnable_LastOne;
        private int DBSpaceErrorCount = 0;

        //SFI Add PSD Event 20121115
        private string strPSDEventDBSQLServer_IP;
        private string strPSDEventDBSQLServer_DataBase;
        private string strPSDEventDBSQLServer_User;
        private string strPSDEventDBSQLServer_Password;
        private string strPSDEventDBSQLServer_Table;
        private bool blnPSDEventDBEnable;
        //20121115 Add End

        //SFI Add EMDS CountDown 20130207
        private string strEMDSCountDown_DBSQLServer_IP;
        private string strEMDSCountDown_DBSQLServer_DataBase;
        private string strEMDSCountDown_DBSQLServer_User;
        private string strEMDSCountDown_DBSQLServer_Password;
        private string strEMDSCountDown_DBSQLServer_Table;
        private bool blnEMDSCountDown_DBEnable;
        //20130207 Add END

        //20170926 SFI Add for VehDistance
        private string strVehDistance_DBSQLServer_IP;
        private string strVehDistance_DBSQLServer_DataBase;
        private string strVehDistance_DBSQLServer_User;
        private string strVehDistance_DBSQLServer_Password;
        private string strVehDistance_DBSQLServer_Table;
        private bool blnVehDistance_DBEnable;
        //20170926 SFI Add End

        //20200826 SAMMY Add for ĵ��
        private string strWakeUpMouse_DBSQLServer_IP;
        private string strWakeUpMouse_DBSQLServer_DataBase;
        private string strWakeUpMouse_DBSQLServer_User;
        private string strWakeUpMouse_DBSQLServer_Password;
        private string strWakeUpMouse_DBSQLServer_Table;
        private bool blnWakeUpMouse_DBEnable;
        //20200826 SAMMY Add End


        //IntelligentDBSQLServer
        private string strEQDBSQLServer_IP;
        private string strEQDBSQLServer_DataBase;
        private string strEQDBSQLServer_User;
        private string strEQDBSQLServer_Password;
        private string strEQDBSQLServer_Table;
        private string strEQDBSQLServer_Table_EQ20;
        private bool blnEQDBEnable;
        private bool blnEQDBEnable_EQ20;

        //NeihuEVTDBNMoveDataOneDay
        private string m_NhEVTDBNMoveDataOneDay_IP;
        private string m_NhEVTDBNMoveDataOneDay_DaraBase;
        private string m_NhEVTDBNMoveDataOneDay_User;
        private string m_NhEVTDBNMoveDataOneDay_Password;
        private string m_NhEVTDBNMoveDataOneDay_RXTable;
        private bool m_NhEVTDBNMoveDataOneDay_Enable;
        private ArrayList m_NhEVTDBColTypList = new ArrayList();		//�s����쫬�A
        private ArrayList m_NhEVTDBColNameList = new ArrayList();		//�s�����W��
        private ArrayList m_NhEVTDBColLengthList = new ArrayList();	//�s�����j�p
        private string m_NhEVTDBstrColTyp = "";
        private string m_NhEVTDBstrColName = "";
        private string m_NhEVTDBstrColLength = "";
        private string m_NhEVTDBstrTime_LastEvent = "";
        private int m_NhEVTDBListLength = 0;					//�����Ϊ�����
        private int m_NhEVTDBListAllLength = 0;					//���Φr���`����
        private string[] m_NhEVTDBKeyWord;

        private DateTime m_bakNextCheckTime;							//�s��W�@����ƪ����


        private string m_strOvlineRTDBIP;
        private string m_strOvlineRTDB;
        private string m_strOvlineRTDBTable;
        private string m_strOvlineFrontIP;
        private string m_strOvlineFrontDB;
        private string m_strOvlineFrontTable;
        private bool m_bOvlineEnable = false;



        private SqlConnection conG9 = null;
        private SqlCommand CmdG9 = null;
        private int intTimerTimerInterval;
        private bool blnfirstTime;
        private DataTable dtIntBase;
        private DataTable dtMultipleAlarm;
        private int intIntervalGetDEDB;
        private int intIntervalGetintelligentDB;
        private int intDataQPerGet;
        private string strFormText;
        private DataTable dtSourceTables;
        internal int oldMin = -1;
        private string strLine_Code;

        private bool SkipNextCenterFlag = false; //951015Hsinson: �ΨӳB�z�a�_IDS.TXT�ᤧ�a�_�T��TRTC.TXT���P�@�զa�_

        private int intHour = -1;  //�ΨӳB�z�C�p���ܤ�
        //GSS

        private DateTime m_DTNextCheckRTDB_RX_TableTime; // �ΨӰO���U�@���n�ˬdRTDB_RX ��ƪ��O�_�n�R�����ɶ�(�R��100�ѫe����ƪ�)


        public ScadaRXM.CIntFilterType m_IntFilterType;

        private int m_nSyncTimeCount = 0;


        public string vstrContent;
        public string strA;
        public string strB;
        public string strC;
        public string strD;
        public string strNotflag;


        private int lineLimit;
        public int LineLimit	// Listbox ����ܦ�ơA���� Form ���L�աC
        {
            get { return lineLimit; }
            set { lineLimit = value; }
        }

        private string localdir;
        public string LocalDir
        {
            get
            {
                localdir = Directory.GetCurrentDirectory();
                return localdir;
            }
        }

        private string logdir;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_DBSpaceErrorCount;

        public string LogDir
        {
            get { return logdir; }
            set
            {
                string tmpdir = value;
                if (!Directory.Exists(tmpdir))
                {
                    try
                    {
                        Directory.CreateDirectory(tmpdir);
                    }
                    catch
                    {
                        return;
                    }
                }
                //Hsinson961012 �ץ�Log�ɮפ���m�A�Ͽ��ѭY�}�l�ؿ������w�Ϻнs���A�h�ϥε����}���l�ؿ��A�_�h�ϥΰ����ɩҦb�ؿ����۹��}
                //logdir = this.LocalDir + "\\" + tmpdir + "\\";
                if (tmpdir.IndexOf(':') > 0)
                    logdir = tmpdir + "\\";
                else
                    logdir = this.LocalDir + "\\" + tmpdir + "\\";
            }
        }
        private string txtdir;
        public string TxtDir
        {
            get { return txtdir; }
            set
            {
                string tmpdir = value;
                if (!Directory.Exists(tmpdir))
                {
                    try
                    {
                        Directory.CreateDirectory(tmpdir);
                    }
                    catch
                    {
                        return;
                    }
                }
                txtdir = this.LocalDir + "\\" + tmpdir + "\\";
            }
        }

        public int MIN(int a, int b)
        {
            return (a > b) ? b : a;
        }
        public int MAX(int a, int b)
        {
            return (a < b) ? b : a;
        }


        #region delegate


        public delegate void Display_StringMessage_Handler(string msg);
        public delegate void ProcessReceiveBusMessage_Handler(SYS.MSG Message);
        public delegate void ShowDBG_ReceiveData_Handler(string str, string check);
        public delegate void ShowGEN_ReceiveData_Handler(LoadPort a, string check);
        public delegate void Display_PortStatus_Handler();
        public delegate void Reload_PortStatus_Handler();
        public delegate void FHOUR_Handler();
        public delegate void FRAWC_Handler(SYS.MSG Msg);
        public delegate void ComPort_Init_Handler();
        public delegate void AddTreeView_Handler();
        public delegate void OpenComPort_Handler();
        public delegate void CloseComPort_Handler();
        public delegate void TreeView_Update_Handler(System.Windows.Forms.TreeViewEventArgs e);
        public delegate void ValidateReceiveComMessage_Handler(byte[] bCommRead, LoadPort a);
        public delegate void FB_MessageToVPI_Handler(LoadPort a);
        public delegate void FC_MessageToVPI_Handler(LoadPort a);
        public delegate void ProcessReceiveComMessage_Handler(byte[] bo, LoadPort a);
        public delegate void ProcessPlayBackMessage_Handler(byte[] bo, ushort tmpLocNum, string tmpLocName);
        public delegate void Judge_CF_Handler(LoadPort a);
        public delegate void AddToControlQueue_Handler(byte[] fc, LoadPort a);
        public delegate void SendtoQueue_Handler(SYS.MSG SendtoQueueMsg);
        public delegate void ReceiveMsg_Handler(SYS.MSG msg);

        public event Display_StringMessage_Handler OnDisplayStringMessage;
        public event ProcessReceiveBusMessage_Handler OnProcessReceiveBusMessage;
        public event Display_PortStatus_Handler OnDisplay_PortStatus;
        public event FHOUR_Handler OnFHOUR;
        public event FRAWC_Handler OnFRAWC;
        public event AddTreeView_Handler OnAddTreeView;
        public event ComPort_Init_Handler OnComPort_Init;
        public event OpenComPort_Handler OnOpenComPort;
        public event CloseComPort_Handler OnCloseComPort;
        public event TreeView_Update_Handler OnTreeView_Update;
        public event ValidateReceiveComMessage_Handler OnValidateReceiveComMessage;
        public event ProcessReceiveComMessage_Handler OnProcessReceviceComMessage;
        public event ProcessPlayBackMessage_Handler OnProcessPlayBackMessage;
        public event AddToControlQueue_Handler OnAddToControlQueue;
        public event SendtoQueue_Handler OnSendtoQueue;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem_Close;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem DBG_ReceiveData;
        private System.Windows.Forms.ImageList IL_Tree;
        private System.Windows.Forms.MenuItem DBG_GEN2MSG;
        private System.Windows.Forms.MenuItem StopAddtoLB;
        private System.Windows.Forms.MenuItem DBG_Send2Server;
        private System.Windows.Forms.Timer tmRXM;
        private System.Windows.Forms.MenuItem menuItem_ComPort;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.StatusBar statusBar1;
        private System.Windows.Forms.StatusBarPanel statusBarPanel1;
        public System.Windows.Forms.ListBox MessageListBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox grpBox_SIM;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.TextBox txb_DoMsgLimite;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        internal System.Windows.Forms.TextBox txb_DoMsgInterval;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txb_AutoCFJudge;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txb_SendtoTBCInterval;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txb_AutoReceive;
        private System.Windows.Forms.TextBox txb_AutoPolling;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txb_SendtoTBCLimite;
        private System.Windows.Forms.Button btn_NewFile;
        private System.Windows.Forms.GroupBox grpBox_FileWrite;
        public System.Windows.Forms.TextBox txb_SaveFile;
        private System.Windows.Forms.Button btn_SaveParam;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_AtsEventErrorCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_LastError;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_WinSysEvtErrorCount;
        private System.ComponentModel.IContainer components;


        #endregion

        public CCI_Form()
        {
            //
            // Windows Form �]�p�u��䴩�����n��
            //

            InitializeComponent();  //1
            myInitial_1();          //2
            //ConnectToTBS();         //3
            myInitial_Delegate();   //4

            //
            // TODO: �b InitializeComponent �I�s����[�J����غc�禡�{���X
            //

            // �]�wDBMF�{������ɡA���ˬdRTDB_RX��ƪ��O�_�ݭn�@�R��
            {
                string strTime = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                m_DTNextCheckRTDB_RX_TableTime = System.DateTime.Parse(strTime);
            }
        }

        /// <summary>
        /// �M������ϥΤ����귽�C
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region 1.Windows Form Designer generated code
        /// <summary>
        /// �����]�p�u��䴩�ҥ��ݪ���k - �ФŨϥε{���X�s�边�ק�
        /// �o�Ӥ�k�����e�C
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCI_Form));
            this.IL_Tree = new System.Windows.Forms.ImageList(this.components);
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_Close = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_ComPort = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.DBG_ReceiveData = new System.Windows.Forms.MenuItem();
            this.DBG_GEN2MSG = new System.Windows.Forms.MenuItem();
            this.DBG_Send2Server = new System.Windows.Forms.MenuItem();
            this.StopAddtoLB = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.tmRXM = new System.Windows.Forms.Timer(this.components);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.MessageListBox = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grpBox_SIM = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_DBSpaceErrorCount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_WinSysEvtErrorCount = new System.Windows.Forms.TextBox();
            this.textBox_LastError = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_AtsEventErrorCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txb_DoMsgLimite = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txb_DoMsgInterval = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txb_AutoCFJudge = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txb_SendtoTBCInterval = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txb_AutoReceive = new System.Windows.Forms.TextBox();
            this.txb_AutoPolling = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txb_SendtoTBCLimite = new System.Windows.Forms.TextBox();
            this.btn_NewFile = new System.Windows.Forms.Button();
            this.grpBox_FileWrite = new System.Windows.Forms.GroupBox();
            this.txb_SaveFile = new System.Windows.Forms.TextBox();
            this.btn_SaveParam = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.grpBox_SIM.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpBox_FileWrite.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // IL_Tree
            // 
            this.IL_Tree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL_Tree.ImageStream")));
            this.IL_Tree.TransparentColor = System.Drawing.Color.Transparent;
            this.IL_Tree.Images.SetKeyName(0, "");
            this.IL_Tree.Images.SetKeyName(1, "");
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem3,
            this.menuItem4});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Close});
            this.menuItem1.Text = "�ɮ�(&F)";
            // 
            // menuItem_Close
            // 
            this.menuItem_Close.Index = 0;
            this.menuItem_Close.Text = "����(&C)";
            this.menuItem_Close.Click += new System.EventHandler(this.menuItem_Close_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_ComPort});
            this.menuItem2.Text = "�˵�(&V)";
            // 
            // menuItem_ComPort
            // 
            this.menuItem_ComPort.Index = 0;
            this.menuItem_ComPort.Text = "ComPort���A";
            this.menuItem_ComPort.Click += new System.EventHandler(this.menuItem_ComPort_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.DBG_ReceiveData,
            this.DBG_GEN2MSG,
            this.DBG_Send2Server,
            this.StopAddtoLB});
            this.menuItem3.Text = "����(&D)";
            // 
            // DBG_ReceiveData
            // 
            this.DBG_ReceiveData.Index = 0;
            this.DBG_ReceiveData.Text = "DBG{ReceiveData}";
            this.DBG_ReceiveData.Click += new System.EventHandler(this.DBG_ReceiveData_Click);
            // 
            // DBG_GEN2MSG
            // 
            this.DBG_GEN2MSG.Index = 1;
            this.DBG_GEN2MSG.Text = "DBG{GEN2MSG}";
            this.DBG_GEN2MSG.Click += new System.EventHandler(this.DBG_GEN2MSG_Click);
            // 
            // DBG_Send2Server
            // 
            this.DBG_Send2Server.Index = 2;
            this.DBG_Send2Server.Text = "DBG{SendToServer}";
            this.DBG_Send2Server.Click += new System.EventHandler(this.DBG_Send2Server_Click);
            // 
            // StopAddtoLB
            // 
            this.StopAddtoLB.Index = 3;
            this.StopAddtoLB.Text = "�Ȱ���XListBox";
            this.StopAddtoLB.Click += new System.EventHandler(this.StopAddtoLB_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem9});
            this.menuItem4.Text = "����(&H)";
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 0;
            this.menuItem9.Text = "����ScadaDBMF(&R)";
            // 
            // tmRXM
            // 
            this.tmRXM.Enabled = true;
            this.tmRXM.Interval = 1000;
            this.tmRXM.Tick += new System.EventHandler(this.tmRXM_Tick);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusBar1);
            this.tabPage1.Controls.Add(this.MessageListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(600, 288);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "�T������";
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 272);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(600, 16);
            this.statusBar1.TabIndex = 47;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Time";
            this.statusBarPanel1.Width = 583;
            // 
            // MessageListBox
            // 
            this.MessageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageListBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessageListBox.HorizontalScrollbar = true;
            this.MessageListBox.ItemHeight = 15;
            this.MessageListBox.Location = new System.Drawing.Point(0, 0);
            this.MessageListBox.Name = "MessageListBox";
            this.MessageListBox.Size = new System.Drawing.Size(600, 288);
            this.MessageListBox.TabIndex = 46;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.grpBox_SIM);
            this.tabPage3.Location = new System.Drawing.Point(4, 21);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(600, 288);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "�t�ΰ�����";
            this.tabPage3.Visible = false;
            // 
            // grpBox_SIM
            // 
            this.grpBox_SIM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBox_SIM.Controls.Add(this.label14);
            this.grpBox_SIM.Controls.Add(this.textBox_DBSpaceErrorCount);
            this.grpBox_SIM.Controls.Add(this.label6);
            this.grpBox_SIM.Controls.Add(this.textBox_WinSysEvtErrorCount);
            this.grpBox_SIM.Controls.Add(this.textBox_LastError);
            this.grpBox_SIM.Controls.Add(this.label5);
            this.grpBox_SIM.Controls.Add(this.textBox_AtsEventErrorCount);
            this.grpBox_SIM.Controls.Add(this.label1);
            this.grpBox_SIM.Font = new System.Drawing.Font("�s�ө���", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grpBox_SIM.ForeColor = System.Drawing.Color.Black;
            this.grpBox_SIM.Location = new System.Drawing.Point(8, 8);
            this.grpBox_SIM.Name = "grpBox_SIM";
            this.grpBox_SIM.Size = new System.Drawing.Size(576, 256);
            this.grpBox_SIM.TabIndex = 27;
            this.grpBox_SIM.TabStop = false;
            this.grpBox_SIM.Text = "Event������T";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(384, 29);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(120, 16);
            this.label14.TabIndex = 7;
            this.label14.Text = "DBSpace Err Count :";
            // 
            // textBox_DBSpaceErrorCount
            // 
            this.textBox_DBSpaceErrorCount.Location = new System.Drawing.Point(504, 24);
            this.textBox_DBSpaceErrorCount.Name = "textBox_DBSpaceErrorCount";
            this.textBox_DBSpaceErrorCount.ReadOnly = true;
            this.textBox_DBSpaceErrorCount.Size = new System.Drawing.Size(56, 22);
            this.textBox_DBSpaceErrorCount.TabIndex = 6;
            this.textBox_DBSpaceErrorCount.Text = "0";
            this.textBox_DBSpaceErrorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(176, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "WinSysEvt Err Count :";
            // 
            // textBox_WinSysEvtErrorCount
            // 
            this.textBox_WinSysEvtErrorCount.Location = new System.Drawing.Point(296, 24);
            this.textBox_WinSysEvtErrorCount.Name = "textBox_WinSysEvtErrorCount";
            this.textBox_WinSysEvtErrorCount.ReadOnly = true;
            this.textBox_WinSysEvtErrorCount.Size = new System.Drawing.Size(56, 22);
            this.textBox_WinSysEvtErrorCount.TabIndex = 4;
            this.textBox_WinSysEvtErrorCount.Text = "0";
            this.textBox_WinSysEvtErrorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_LastError
            // 
            this.textBox_LastError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LastError.Location = new System.Drawing.Point(16, 72);
            this.textBox_LastError.Multiline = true;
            this.textBox_LastError.Name = "textBox_LastError";
            this.textBox_LastError.ReadOnly = true;
            this.textBox_LastError.Size = new System.Drawing.Size(544, 168);
            this.textBox_LastError.TabIndex = 3;
            this.textBox_LastError.Text = "(No Error)";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "Last Error:";
            // 
            // textBox_AtsEventErrorCount
            // 
            this.textBox_AtsEventErrorCount.Location = new System.Drawing.Point(104, 24);
            this.textBox_AtsEventErrorCount.Name = "textBox_AtsEventErrorCount";
            this.textBox_AtsEventErrorCount.ReadOnly = true;
            this.textBox_AtsEventErrorCount.Size = new System.Drawing.Size(56, 22);
            this.textBox_AtsEventErrorCount.TabIndex = 1;
            this.textBox_AtsEventErrorCount.Text = "0";
            this.textBox_AtsEventErrorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ats Err Count :";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox1);
            this.tabPage5.Controls.Add(this.btn_NewFile);
            this.tabPage5.Controls.Add(this.grpBox_FileWrite);
            this.tabPage5.Controls.Add(this.btn_SaveParam);
            this.tabPage5.Location = new System.Drawing.Point(4, 21);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(600, 288);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "�ѼƸ��";
            this.tabPage5.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txb_DoMsgLimite);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txb_DoMsgInterval);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txb_AutoCFJudge);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txb_SendtoTBCInterval);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txb_AutoReceive);
            this.groupBox1.Controls.Add(this.txb_AutoPolling);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txb_SendtoTBCLimite);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(8, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 200);
            this.groupBox1.TabIndex = 72;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "�ѼƷL��";
            // 
            // txb_DoMsgLimite
            // 
            this.txb_DoMsgLimite.Location = new System.Drawing.Point(128, 112);
            this.txb_DoMsgLimite.Name = "txb_DoMsgLimite";
            this.txb_DoMsgLimite.Size = new System.Drawing.Size(56, 22);
            this.txb_DoMsgLimite.TabIndex = 75;
            this.txb_DoMsgLimite.Visible = false;
            this.txb_DoMsgLimite.TextChanged += new System.EventHandler(this.txb_DoMsgLimite_TextChanged);
            this.txb_DoMsgLimite.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_DoMsgLimite_KeyDown);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(8, 120);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(112, 23);
            this.label15.TabIndex = 74;
            this.label15.Text = "DoMsg Limite";
            this.label15.Visible = false;
            // 
            // label16
            // 
            this.label16.ForeColor = System.Drawing.Color.Blue;
            this.label16.Location = new System.Drawing.Point(192, 96);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(168, 23);
            this.label16.TabIndex = 73;
            this.label16.Text = "�B�zTBS�e�Ӹ�Ƥ��ɶ����j";
            this.label16.Visible = false;
            // 
            // txb_DoMsgInterval
            // 
            this.txb_DoMsgInterval.Location = new System.Drawing.Point(128, 88);
            this.txb_DoMsgInterval.Name = "txb_DoMsgInterval";
            this.txb_DoMsgInterval.Size = new System.Drawing.Size(56, 22);
            this.txb_DoMsgInterval.TabIndex = 72;
            this.txb_DoMsgInterval.Visible = false;
            this.txb_DoMsgInterval.TextChanged += new System.EventHandler(this.txb_DoMsgInterval_TextChanged);
            this.txb_DoMsgInterval.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_DoMsgInterval_KeyDown);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(8, 96);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(112, 23);
            this.label17.TabIndex = 71;
            this.label17.Text = "DoMsg Interval";
            this.label17.Visible = false;
            // 
            // label18
            // 
            this.label18.ForeColor = System.Drawing.Color.Blue;
            this.label18.Location = new System.Drawing.Point(192, 120);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(168, 23);
            this.label18.TabIndex = 76;
            this.label18.Text = "�C���B�zTBS�e�Ӹ�Ƥ����";
            this.label18.Visible = false;
            // 
            // txb_AutoCFJudge
            // 
            this.txb_AutoCFJudge.Location = new System.Drawing.Point(128, 64);
            this.txb_AutoCFJudge.Name = "txb_AutoCFJudge";
            this.txb_AutoCFJudge.Size = new System.Drawing.Size(56, 22);
            this.txb_AutoCFJudge.TabIndex = 49;
            this.txb_AutoCFJudge.TextChanged += new System.EventHandler(this.txb_AutoCFJudge_TextChanged);
            this.txb_AutoCFJudge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_AutoCFJudge_KeyDown);
            // 
            // label11
            // 
            this.label11.ForeColor = System.Drawing.Color.Blue;
            this.label11.Location = new System.Drawing.Point(192, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(176, 23);
            this.label11.TabIndex = 5;
            this.label11.Text = "Ū�����z�P�_��Ƥ��ɶ����j";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "GetFEDB Interval";
            // 
            // txb_SendtoTBCInterval
            // 
            this.txb_SendtoTBCInterval.Location = new System.Drawing.Point(128, 136);
            this.txb_SendtoTBCInterval.Name = "txb_SendtoTBCInterval";
            this.txb_SendtoTBCInterval.Size = new System.Drawing.Size(56, 22);
            this.txb_SendtoTBCInterval.TabIndex = 43;
            this.txb_SendtoTBCInterval.Visible = false;
            this.txb_SendtoTBCInterval.TextChanged += new System.EventHandler(this.txb_SendtoTBCInterval_TextChanged);
            this.txb_SendtoTBCInterval.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_SendtoTBCInterval_KeyDown);
            // 
            // label10
            // 
            this.label10.ForeColor = System.Drawing.Color.Blue;
            this.label10.Location = new System.Drawing.Point(192, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(168, 23);
            this.label10.TabIndex = 4;
            this.label10.Text = "����FrontDB���ɶ����j(ms)";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 23);
            this.label9.TabIndex = 1;
            this.label9.Text = "Get Intelligent Interval";
            // 
            // txb_AutoReceive
            // 
            this.txb_AutoReceive.Location = new System.Drawing.Point(128, 40);
            this.txb_AutoReceive.Name = "txb_AutoReceive";
            this.txb_AutoReceive.Size = new System.Drawing.Size(56, 22);
            this.txb_AutoReceive.TabIndex = 3;
            this.txb_AutoReceive.TextChanged += new System.EventHandler(this.txb_AutoReceive_TextChanged);
            this.txb_AutoReceive.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_AutoReceive_KeyDown);
            // 
            // txb_AutoPolling
            // 
            this.txb_AutoPolling.Location = new System.Drawing.Point(128, 16);
            this.txb_AutoPolling.Name = "txb_AutoPolling";
            this.txb_AutoPolling.Size = new System.Drawing.Size(56, 22);
            this.txb_AutoPolling.TabIndex = 2;
            this.txb_AutoPolling.TextChanged += new System.EventHandler(this.txb_AutoPolling_TextChanged);
            this.txb_AutoPolling.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_AutoPolling_KeyDown);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(192, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 23);
            this.label4.TabIndex = 47;
            this.label4.Text = "�C��TBC�e��Ʀ�TBS�����";
            this.label4.Visible = false;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 72);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(112, 23);
            this.label13.TabIndex = 48;
            this.label13.Text = "Data Quantity";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 23);
            this.label7.TabIndex = 45;
            this.label7.Text = "SendToTBCLimite";
            this.label7.Visible = false;
            // 
            // label12
            // 
            this.label12.ForeColor = System.Drawing.Color.Blue;
            this.label12.Location = new System.Drawing.Point(192, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(192, 23);
            this.label12.TabIndex = 50;
            this.label12.Text = "�C���@�~���o����Ƶ���";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(192, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 23);
            this.label2.TabIndex = 44;
            this.label2.Text = "TBC�e��Ʀ�TBS���ɶ����j";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 23);
            this.label3.TabIndex = 42;
            this.label3.Text = "SendToTBCInterval";
            this.label3.Visible = false;
            // 
            // txb_SendtoTBCLimite
            // 
            this.txb_SendtoTBCLimite.Location = new System.Drawing.Point(128, 160);
            this.txb_SendtoTBCLimite.Name = "txb_SendtoTBCLimite";
            this.txb_SendtoTBCLimite.Size = new System.Drawing.Size(56, 22);
            this.txb_SendtoTBCLimite.TabIndex = 46;
            this.txb_SendtoTBCLimite.Visible = false;
            this.txb_SendtoTBCLimite.TextChanged += new System.EventHandler(this.txb_SendtoTBCLimite_TextChanged);
            this.txb_SendtoTBCLimite.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_SendtoTBCLimite_KeyDown);
            // 
            // btn_NewFile
            // 
            this.btn_NewFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_NewFile.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btn_NewFile.Location = new System.Drawing.Point(440, 32);
            this.btn_NewFile.Name = "btn_NewFile";
            this.btn_NewFile.Size = new System.Drawing.Size(80, 32);
            this.btn_NewFile.TabIndex = 41;
            this.btn_NewFile.Text = "���s�}����";
            this.btn_NewFile.Click += new System.EventHandler(this.btn_NewFile_Click);
            // 
            // grpBox_FileWrite
            // 
            this.grpBox_FileWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBox_FileWrite.Controls.Add(this.txb_SaveFile);
            this.grpBox_FileWrite.ForeColor = System.Drawing.Color.Black;
            this.grpBox_FileWrite.Location = new System.Drawing.Point(8, 8);
            this.grpBox_FileWrite.Name = "grpBox_FileWrite";
            this.grpBox_FileWrite.Size = new System.Drawing.Size(424, 56);
            this.grpBox_FileWrite.TabIndex = 40;
            this.grpBox_FileWrite.TabStop = false;
            this.grpBox_FileWrite.Text = "Current Log File Path";
            // 
            // txb_SaveFile
            // 
            this.txb_SaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txb_SaveFile.Enabled = false;
            this.txb_SaveFile.Location = new System.Drawing.Point(8, 16);
            this.txb_SaveFile.Name = "txb_SaveFile";
            this.txb_SaveFile.Size = new System.Drawing.Size(400, 22);
            this.txb_SaveFile.TabIndex = 32;
            // 
            // btn_SaveParam
            // 
            this.btn_SaveParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SaveParam.Location = new System.Drawing.Point(440, 171);
            this.btn_SaveParam.Name = "btn_SaveParam";
            this.btn_SaveParam.Size = new System.Drawing.Size(80, 32);
            this.btn_SaveParam.TabIndex = 9;
            this.btn_SaveParam.Text = "�x�s�Ѽ�";
            this.btn_SaveParam.Click += new System.EventHandler(this.btn_SaveParam_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 17);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(608, 313);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // CCI_Form
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(608, 313);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "CCI_Form";
            this.Text = "ScadaDBMF_NH 20210421";
            this.Activated += new System.EventHandler(this.CCI_Form_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CCI_Form_Closing);
            this.Load += new System.EventHandler(this.CCI_Form_Load);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.grpBox_SIM.ResumeLayout(false);
            this.grpBox_SIM.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpBox_FileWrite.ResumeLayout(false);
            this.grpBox_FileWrite.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region 2.myInitial_1
        public void myInitial_1()
        {
            Filename = "ScadaDBMF.ini";
            TaskName = "ScadaDBMF";
            this.RootNode = new TreeNode("AllPort");
            PollingToVPI = SYS.Tools.HexStringToByte("FB0182F000F6");  //��{��Polling���T��
            AckToVPI = SYS.Tools.HexStringToByte("FA018360F6");        //��{��Ack���T��
            RecallFromVPI = SYS.Tools.HexStringToByte("FD018150F6");   //��{��Recall���T��
            DBG_ReceiveMessageCounter = 0;
            GEN_ReceiveMessageCounter = 0;
            myOpenFileDialog = new OpenFileDialog();
            speed = 1;

            try
            {
                lineLimit = int.Parse(Profile.GetValue(Filename, "Param", "LineLimit"));
                LogDir = Profile.GetValue(Filename, "CCIFiles", "LOG");
                LogFileName = Profile.GetValue(Filename, "CCIFiles", "LogFileName");
                LogFileExt = Profile.GetValue(Filename, "CCIFiles", "LogFileExt");
                TxtDir = Profile.GetValue(Filename, "CCIFiles", "TXT");
                MonFilePath = Profile.GetValue(Filename, "CCIFiles", "MonFilePath");   //Hsisnon970528 add
                MonFileName = Profile.GetValue(Filename, "CCIFiles", "MonFileName");
                MonFileExt = Profile.GetValue(Filename, "CCIFiles", "MonFileExt");

                // GSS - SCADA
                // this.strSourceSQLServer_User =Profile.GetValue("ScadaDBMF.ini","SourceSQLServer","User");
                this.strSourceSQLServer_User = "scada";
                this.strSourceSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "SourceSQLServer", "DataBase");
                this.strSourceSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "SourceSQLServer", "IP");
                // this.strSourceSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","SourceSQLServer","Password");
                this.strSourceSQLServer_Password = "vxlsys$1";
                this.strSourceSQLServer_RXTable = Profile.GetValue("ScadaDBMF.ini", "SourceSQLServer", "RXTable");
                if (strLocalSQLServer_Password == null) strLocalSQLServer_Password = "";


                // this.strLocalSQLServer_User =Profile.GetValue("ScadaDBMF.ini","RTDBSQLServer","User");
                this.strLocalSQLServer_User = "scada";
                this.strLocalSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServer", "DataBase");
                this.strLocalSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServer", "IP");
                // this.strLocalSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","RTDBSQLServer","Password");
                this.strLocalSQLServer_Password = "vxlsys$1";
                this.strLocalSQLServer_RXTable = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServer", "RXTable");
                this.blnLocalDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServer", "DBEnable"));
                if (strLocalSQLServer_Password == null) strLocalSQLServer_Password = "";


                // RTDB (One day)  Jacky_su 980504 add
                this.m_strLocalSQLServer_OneDay_User = "scada";
                this.m_strLocalSQLServer_OneDay_Password = "vxlsys$1";
                this.m_strLocalSQLServer_OneDay_DataBase = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "DataBase");
                this.m_strLocalSQLServer_OneDay_IP = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "IP");
                //�ѩ�Table�אּ�H �����Table�W�١A�ҥH�אּ�bŪ�g��Ʈw�ɡA�~�]�w�nŪ�g��Table�W��
                // this.strLocalSQLServer_OneDay_RXTable  = Profile.GetValue("ScadaDBMF.ini","RTDBSQLServerOneDay","RXTable");
                this.m_blnLocalDB_OneDay_Enable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "DBEnable"));


                if (blnLocalDBEnable == true)
                {
                    this.strLocalSQLServer_Ov_LineTable = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServer", "OvTable");
                }
                else
                {
                    this.strLocalSQLServer_Ov_LineTable = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "OvTable");
                }




                // MainDB (One day)  Jacky_su 980504 add
                this.m_strMainDBSQLServer_OneDay_User = "scada";
                this.m_strMainDBSQLServer_OneDay_Password = "vxlsys$1";
                this.m_strMainDBSQLServer_OneDay_DataBase = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServerOneDay", "DataBase");
                this.m_strMainDBSQLServer_OneDay_IP = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServerOneDay", "IP");
                //�ѩ�Table�אּ�H �����Table�W�١A�ҥH�אּ�bŪ�g��Ʈw�ɡA�~�]�w�nŪ�g��Table�W��
                // this.m_strMainDBSQLServer_OneDay_RXTable  = Profile.GetValue("ScadaDBMF.ini","MainDBSQLServerOneDay","RXTable");
                this.m_blnMainDBEnable_OneDay = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServerOneDay", "DBEnable"));


                // this.strMainDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","MainDBSQLServer","User");
                this.strMainDBSQLServer_User = "scada";
                this.strMainDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServer", "DataBase");
                this.strMainDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServer", "IP");
                // this.strMainDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","MainDBSQLServer","Password");
                this.strMainDBSQLServer_Password = "vxlsys$1";
                this.strMainDBSQLServer_RXTable = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServer", "RXTable");
                this.blnMainDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServer", "DBEnable"));
                if (strMainDBSQLServer_Password == null) strMainDBSQLServer_Password = "";

                //this.strAlarmDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","AlarmDBSQLServer","User");
                this.strAlarmDBSQLServer_User = "scada";
                this.strAlarmDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "AlarmDBSQLServer", "DataBase");
                this.strAlarmDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "AlarmDBSQLServer", "IP");
                // this.strAlarmDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","AlarmDBSQLServer","Password");
                this.strAlarmDBSQLServer_Password = "vxlsys$1";
                this.strAlarmDBSQLServer_RXTable = Profile.GetValue("ScadaDBMF.ini", "AlarmDBSQLServer", "RXTable");
                //20181025 add
                this.strAlarmDBSQLServer_RXTableTemp = Profile.GetValue("ScadaDBMF.ini", "AlarmDBSQLServer", "RXTableTemp");
                this.blnAlarmDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "AlarmDBSQLServer", "DBEnable"));
                if (strAlarmDBSQLServer_Password == null) strAlarmDBSQLServer_Password = "";


                // this.strItlDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","IntelligentDBSQLServer","User");
                this.strItlDBSQLServer_User = "scada";
                this.strItlDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "IntelligentDBSQLServer", "DataBase");
                this.strItlDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "IntelligentDBSQLServer", "IP");
                // this.strItlDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","IntelligentDBSQLServer","Password");
                this.strItlDBSQLServer_Password = "vxlsys$1";
                if (strItlDBSQLServer_Password == null) strMainDBSQLServer_Password = "";
                this.blnItlDBDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "IntelligentDBSQLServer", "DBEnable"));

                // this.strNetDCDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","NetDCDBSQLServer","User");
                this.strNetDCDBSQLServer_User = "scada";
                this.strNetDCDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "DataBase");
                this.strNetDCDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "IP");
                // this.strNetDCDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","NetDCDBSQLServer","Password");
                this.strNetDCDBSQLServer_Password = "vxlsys$1";
                strNetDCDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "Table");
                this.blnNetDCDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "DBEnable"));

                //960813 Hsinson add
                strNetDCDBSQLServer_Table_LastOne = Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "Table_LastOne");
                this.blnNetDCDBEnable_History = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "DBEnable_History"));
                this.blnNetDCDBEnable_LastOne = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "NetDCDBSQLServer", "DBEnable_LastOne"));

                if (strNetDCDBSQLServer_Password == null) strNetDCDBSQLServer_Password = "";


                // this.strEQDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","EarthQuakeDBSQLServer","User");
                this.strEQDBSQLServer_User = "scada";
                this.strEQDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "EarthQuakeDBSQLServer", "DataBase");
                this.strEQDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "EarthQuakeDBSQLServer", "IP");
                // this.strEQDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","EarthQuakeDBSQLServer","Password");
                this.strEQDBSQLServer_Password = "vxlsys$1";
                strEQDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "EarthQuakeDBSQLServer", "Table");
                strEQDBSQLServer_Table_EQ20 = strEQDBSQLServer_Table + "_EQ20";

                this.blnEQDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "EarthQuakeDBSQLServer", "DBEnable"));
                this.blnEQDBEnable_EQ20 = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "EarthQuakeDBSQLServer", "DBEnable"));
                
                if (strEQDBSQLServer_Password == null) strEQDBSQLServer_Password = "";

                this.strFormText = Profile.GetValue("ScadaDBMF.ini", "TaskState", "TaskName");
                this.strLine_Code = Profile.GetValue("ScadaDBMF.ini", "TaskState", "LineCode");
                blnfirstTime = true;

                //Hsinson961009 add AtsEvent
                // this.strAtsEventDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","AtsEventDBSQLServer","User");
                this.strAtsEventDBSQLServer_User = "scada";
                this.strAtsEventDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "DataBase");
                this.strAtsEventDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "IP");
                // this.strAtsEventDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","AtsEventDBSQLServer","Password");
                this.strAtsEventDBSQLServer_Password = "vxlsys$1";
                this.strAtsEventDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "Table");
                this.blnAtsEventDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "DBEnable"));
                //Hsinson970219 add TWC table
                this.strAtsTwcDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "TwcTable");
                this.blnAtsTwcDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "AtsEventDBSQLServer", "TwcDBEnable"));

                //
                {
                    m_strOvlineRTDBIP = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "RTDBIP");
                    m_strOvlineRTDB = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "RTDBName");
                    m_strOvlineRTDBTable = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "RTDBTable");

                    m_strOvlineFrontIP = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "FrontDBIP");
                    m_strOvlineFrontDB = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "FrontDBName");
                    m_strOvlineFrontTable = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "FrintDBTalbe");

                    string strTemp = Profile.GetValue("ScadaDBMF.ini", "OvlineSync", "Enable");

                    if (string.Compare(strTemp.ToLower(), "true") == 0)
                    {
                        m_bOvlineEnable = true;
                    }
                    else
                    {
                        m_bOvlineEnable = false;
                    }

                }


                //Hsinson961227 add WinSysEvt
                // this.strWinSysEvtDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","WinSysEvtDBSQLServer","User");
                this.strWinSysEvtDBSQLServer_User = "scada";
                this.strWinSysEvtDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "WinSysEvtDBSQLServer", "DataBase");
                this.strWinSysEvtDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "WinSysEvtDBSQLServer", "IP");
                // this.strWinSysEvtDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","WinSysEvtDBSQLServer","Password");
                this.strWinSysEvtDBSQLServer_Password = "vxlsys$1";
                strWinSysEvtDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "WinSysEvtDBSQLServer", "Table");
                this.blnWinSysEvtDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "WinSysEvtDBSQLServer", "DBEnable"));

                //Wilson1090927 add TLOS_NH
                this.strTLOS_NHDBSQLServer_User = "scada";
                this.strTLOS_NHDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "TLOS_NH", "DataBase");
                this.strTLOS_NHDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "TLOS_NH", "IP");
                this.strTLOS_NHDBSQLServer_Password = "vxlsys$1";
                strTLOS_NHDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "TLOS_NH", "Table");
                this.blnTLOS_NHDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "TLOS_NH", "DBEnable"));

                //Hsinson961227 add PMS
                // this.strPmsDBSQLServer_User =Profile.GetValue("ScadaDBMF.ini","PmsDBSQLServer","User");
                this.strPmsDBSQLServer_User = "scada";
                this.strPmsDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "DataBase");
                this.strPmsDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "IP");
                // this.strPmsDBSQLServer_Password  =Profile.GetValue("ScadaDBMF.ini","PmsDBSQLServer","Password");
                this.strPmsDBSQLServer_Password = "vxlsys$1";
                strPmsDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "Table");
                this.blnPmsDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "DBEnable"));
                strPmsDBSQLServer_Table_LastOne = Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "Table_LastOne");
                this.blnPmsDBEnable_History = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "DBEnable_History"));
                this.blnPmsDBEnable_LastOne = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "PmsDBSQLServer", "DBEnable_LastOne"));
                if (strPmsDBSQLServer_Password == null) strPmsDBSQLServer_Password = "";

                //Kelvin, 2010/12/24, starting
                this.strDBSpaceDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "DataBase");
                this.strDBSpaceDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "IP");
                this.strDBSpaceDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "Table");
                this.blnDBSpaceDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "DBEnable"));
                this.strDBSpaceDBSQLServer_Table_LastOne = Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "Table_LastOne");
                this.blnDBSpaceDBEnable_History = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "DBEnable_History"));
                this.blnDBSpaceDBEnable_LastOne = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "DBSpaceDBSQLServer", "DBEnable_LastOne"));
                this.strDBSpaceDBSQLServer_User = "scada";
                this.strDBSpaceDBSQLServer_Password = "vxlsys$1";
                //Kelvin, 2010/12/24, ending

                //20121114 SFI Add for PSD
                this.strPSDEventDBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "PSDLogDBSQLServer", "DataBase");
                this.strPSDEventDBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "PSDLogDBSQLServer", "IP");
                this.strPSDEventDBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "PSDLogDBSQLServer", "Table");
                this.blnPSDEventDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "PSDLogDBSQLServer", "DBEnable"));
                this.strPSDEventDBSQLServer_User = "scada";
                this.strPSDEventDBSQLServer_Password = "vxlsys$1";
                //20121114 SFI Add End

                //20130207 SFI Add for EMDS CountDown
                this.strEMDSCountDown_DBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "EMDSCountDownDBSQLServer", "DataBase");
                this.strEMDSCountDown_DBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "EMDSCountDownDBSQLServer", "IP");
                this.strEMDSCountDown_DBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "EMDSCountDownDBSQLServer", "Table");
                this.blnEMDSCountDown_DBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "EMDSCountDownDBSQLServer", "DBEnable"));
                this.strEMDSCountDown_DBSQLServer_User = "scada";
                this.strEMDSCountDown_DBSQLServer_Password = "vxlsys$1";
                //20130207 SFI Add End


                //20170926 SFI Add for VehDistance
                VehDistanceMove.strVehDistance_DBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "VehDistance", "DataBase");
                VehDistanceMove.strVehDistance_DBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "VehDistance", "IP");
                VehDistanceMove.strVehDistance_DBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "VehDistance", "Table");
                VehDistanceMove.blnVehDistance_DBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "VehDistance", "DBEnable"));
                VehDistanceMove.strVehDistance_DBSQLServer_User = "scada";
                VehDistanceMove.strVehDistance_DBSQLServer_Password = "vxlsys$1";
                VehDistanceMove.callback += new Display_StringMessage_Handler(DisplayStringMessage);
                //20170926 SFI Add End

                //20200804 SFI Add for �_���x���x����
                PTTRTCDCS_EventMove.strPTTRTCDCS_Event_DBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "PTTRTCDCS", "DataBase");
                PTTRTCDCS_EventMove.strPTTRTCDCS_Event_DBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "PTTRTCDCS", "IP");
                PTTRTCDCS_EventMove.strPTTRTCDCS_Event_DBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "PTTRTCDCS", "Table");
                PTTRTCDCS_EventMove.blnPTTRTCDCS_Event_DBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "PTTRTCDCS", "DBEnable"));
                PTTRTCDCS_EventMove.strPTTRTCDCS_Event_DBSQLServer_User = "scada";
                PTTRTCDCS_EventMove.strPTTRTCDCS_Event_DBSQLServer_Password = "vxlsys$1";
                PTTRTCDCS_EventMove.callback += new Display_StringMessage_Handler(DisplayStringMessage);
                //20170926 SFI Add End

                //20200826 SAMMY Add for ĵ��
                this.strWakeUpMouse_DBSQLServer_IP = Profile.GetValue("ScadaDBMF.ini", "WAKEUPMOUSE", "IP");
                this.strWakeUpMouse_DBSQLServer_DataBase = Profile.GetValue("ScadaDBMF.ini", "WAKEUPMOUSE", "DataBase");
                this.strWakeUpMouse_DBSQLServer_Table = Profile.GetValue("ScadaDBMF.ini", "WAKEUPMOUSE", "Table");
                this.strWakeUpMouse_DBSQLServer_User = "scada";
                this.strWakeUpMouse_DBSQLServer_Password = "vxlsys$1";

                this.blnWakeUpMouse_DBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "WAKEUPMOUSE", "DBEnable"));

                //20200826 SAMMY End

                // ��l�� �L�o���� ������(�Ѻ���"����r�w�q"�s�W���L�o����)
                try
                {
                    // m_IntFilterType
                    m_IntFilterType = new ScadaRXM.CIntFilterType();

                    string strIP;
                    string strID = "scada";
                    string strPW = "vxlsys$1";
                    bool bDBEnable = false;

                    strIP = Profile.GetValue("ScadaDBMF.ini", "IntFilterType", "IP");
                    bDBEnable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "IntFilterType", "DBEnable"));

                    m_IntFilterType.m_strDBIP = strIP;
                    m_IntFilterType.m_strDBID = strID;
                    m_IntFilterType.m_strDBPW = strPW;
                    m_IntFilterType.m_bEnable = bDBEnable;

                    //m_IntFilterType.LoadDBFilterType();


                }
                catch (Exception Ex)
                {
                }


                //-----------------------------			NeihuEVTDBNMoveDataOneDay                -------------------//	
                try
                {
                    this.m_NhEVTDBNMoveDataOneDay_User = "scada";
                    this.m_NhEVTDBNMoveDataOneDay_Password = "vxlsys$1";
                    this.m_NhEVTDBNMoveDataOneDay_DaraBase = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "DataBase");
                    this.m_NhEVTDBNMoveDataOneDay_IP = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "IP");
                    this.m_NhEVTDBNMoveDataOneDay_Enable = Convert.ToBoolean(Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "DBEnable"));

                    this.m_NhEVTDBKeyWord = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "keyword").Split(new Char[] { ';' });


                    //����������������        NhEVTDBŪ��INI        ����������������//

                    if (this.m_NhEVTDBstrTime_LastEvent == "")
                    {
                        try
                        {
                            this.m_NhEVTDBColNameList.Clear();
                            this.m_NhEVTDBColLengthList.Clear();
                            this.m_NhEVTDBColTypList.Clear();
                            this.m_NhEVTDBListAllLength = 0;

                            for (this.m_NhEVTDBListLength = 0; this.m_NhEVTDBListLength < 1000; this.m_NhEVTDBListLength++)
                            {
                                this.m_NhEVTDBstrColName = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColName" + this.m_NhEVTDBListLength.ToString("000"));
                                this.m_NhEVTDBstrColTyp = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColTyp" + this.m_NhEVTDBListLength.ToString("000"));
                                this.m_NhEVTDBstrColLength = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColLength" + this.m_NhEVTDBListLength.ToString("000"));

                                if (this.m_NhEVTDBstrColName == null || this.m_NhEVTDBstrColTyp == null || this.m_NhEVTDBstrColLength == null)
                                {
                                    break;
                                }
                                this.m_NhEVTDBstrColTyp = checkColTyp(this.m_NhEVTDBstrColTyp);
                                if (this.m_NhEVTDBstrColTyp == null)
                                {
                                    this.OnDisplayStringMessage("ScadaDBMF.ini / NhEVTDBNMoveDataOneDay / ColTyp " + this.m_NhEVTDBListLength.ToString("000") + "�榡�]�w���~");
                                    break;
                                }
                                this.m_NhEVTDBColNameList.Add(this.m_NhEVTDBstrColName);
                                this.m_NhEVTDBColTypList.Add(this.m_NhEVTDBstrColTyp);
                                this.m_NhEVTDBColLengthList.Add(this.m_NhEVTDBstrColLength);
                                this.m_NhEVTDBListAllLength = this.m_NhEVTDBListAllLength + Convert.ToInt16(this.m_NhEVTDBstrColLength);
                            }
                        }
                        catch (Exception Ex)
                        {
                            this.OnDisplayStringMessage("scadaDBMF.ini ���Ū���@�~���`: " + Ex.Message);

                        }
                    }
                }
                catch (Exception)
                {
                }

                //����������������        NhEVTDBŪ��INI        ����������������//


                //this.aryLineCode=new Array(20);

                //end GSS - SCADA
            }

            catch (Exception)
            {
                lineLimit = 50;
                LogDir = "LOG";
                LogFileName = "GENCODE_";
                LogFileExt = "LOG";
                TxtDir = "TXT";
                MonFilePath = "LOG";    //Hsisnon970528 add
                MonFileName = "MonMessage_";
                MonFileExt = "TXT";
            }

            this.SetLogFileVersion(this.SetLogFileDateExt());
            SYS.LocConverter.LocTableLoad(txtdir + "\\" + "LocNumber.txt");  //�]�����O����Converter���� static method�A�ҥH���|�۰�Ū�ɡA�]������������@��
            LocIndexLength = LocConverter.LocIndex.Length;
            this.myLoadPort = new LoadPort[LocIndexLength];  //�x�s LoadPort ���}�C
            this.myCF_Count = new CF_Count[LocIndexLength];  //�x�s CF ���A���}�C
            this.myPortStatus = new PortStatus();
            //			this.txb_PortFileName.Text = txtdir + SYS.Profile.GetValue(Filename, "ComPortFile", "ComPortFile");
            try
            {
                this.SafeButtom = SYS.Profile.GetValue(Filename, "AUTO", "SafeButtom");  //Ū����ƶǰe�榡
                this.AutoPolling = bool.Parse(SYS.Profile.GetValue(Filename, "AUTO", "AutoPolling"));  //Ū����ƶǰe�榡
                this.RAWType = SYS.Profile.GetValue(Filename, "AUTO", "RAWType");  //Ū����ƶǰe�榡
                this.Simulation = SYS.Profile.GetValue(Filename, "AUTO", "Simulation");  //Ū�������榡
                this.AutoPack = bool.Parse(SYS.Profile.GetValue(Filename, "AUTO", "AutoPack"));  //Ū��Pack�榡
                this.AutoComPortOpen = bool.Parse(SYS.Profile.GetValue(Filename, "AUTO", "AutoComPortOpen"));  //Ū��AutoComPortOpen���w�]��

                this.txb_DoMsgInterval.Text = SYS.Profile.GetValue(Filename, "Param", "DoMsgInterval");
                this.txb_DoMsgLimite.Text = SYS.Profile.GetValue(Filename, "Param", "DoMsgLimite");
                this.txb_SendtoTBCInterval.Text = SYS.Profile.GetValue(Filename, "Param", "SendToTBCInterval");
                this.txb_SendtoTBCLimite.Text = SYS.Profile.GetValue(Filename, "Param", "SendToTBCLimite");
            }
            catch
            {
                this.SafeButtom = "Close";
                this.AutoPolling = false;
                this.RAWType = "FT810";
                this.Simulation = "Receive";
                this.AutoPack = true;
                this.AutoComPortOpen = true;

                this.txb_DoMsgInterval.Text = "1000";
                this.txb_DoMsgLimite.Text = "10";
                this.txb_SendtoTBCInterval.Text = "1000";
                this.txb_SendtoTBCLimite.Text = "5";
            }
            this.DoMsgLimite = Int32.Parse(this.txb_DoMsgLimite.Text);
            this.SendToTBCLimite = Int32.Parse(this.txb_SendtoTBCLimite.Text);

            this.txb_AutoReceive.Text = SYS.Profile.GetValue(Filename, "Param", "AutoReceive");
            this.txb_AutoPolling.Text = SYS.Profile.GetValue(Filename, "Param", "AutoPolling");
            this.txb_AutoCFJudge.Text = SYS.Profile.GetValue(Filename, "Param", "AutoCFJudge");
            //			this.txb_PlayBackOffset.Text = SYS.Profile.GetValue(Filename, "Param", "PlayBackOffset");
            //			this.PlayBackOffset = Int32.Parse(this.txb_PlayBackOffset.Text);

            this.ReceiveMsgQue = Queue.Synchronized(new Queue());
            this.SyncdSendToTBCQueue = Queue.Synchronized(new Queue());

            this.DoMessageTimer = new System.Timers.Timer();
            this.DoMessageTimer.Enabled = true;
            this.DoMessageTimer.Interval = Int32.Parse(this.txb_DoMsgInterval.Text);
            this.DoMessageTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoMessageTimer_Elapse);

            this.SendMsgToTBC_Timer = new System.Timers.Timer();
            this.SendMsgToTBC_Timer.Interval = Int32.Parse(this.txb_SendtoTBCInterval.Text);
            this.SendMsgToTBC_Timer.Enabled = false;
            this.SendMsgToTBC_Timer.Elapsed += new System.Timers.ElapsedEventHandler(this.SendMsgToTBC_Timer_Elapsed);


            if (this.SafeButtom == "Close")
            {
                //				this.radBtn_Safe_Close.Checked = true;
                //				this.grpBox_Polling.Enabled = this.grpBox_RawType.Enabled = this.grpBox_SIM.Enabled = false;
            }
            else if (this.SafeButtom == "Open")
            {
                //				this.radBtn_Safe_Open.Checked = true;
                //				this.grpBox_Polling.Enabled = this.grpBox_RawType.Enabled = this.grpBox_SIM.Enabled = true;
            }

            if (this.AutoPolling == true)
            {
                //				this.radBtn_Polling_Open.Checked = true;
            }
            else
            {
                //				this.radBtn_Polling_Close.Checked = true;
            }

            if (this.RAWType == "FT810")
            {
                //				this.radBtn_RawType_FT810.Checked = true;
            }
            else if (this.RAWType == "DTS")
            {
                //				this.radBtn_RawType_DTS.Checked = true;
            }

            if (this.Simulation == "Receive")
            {
                //				this.radBtn_Receive.Checked = true;
            }
            else
            {
                //				this.radBtn_Send.Checked = true;
            }

            if (this.AutoPack == true)
            {
                //				this.checkBox_CRCF6.Checked = true;
            }
            else
            {
                //				this.checkBox_CRCF6.Checked = false;
            }

            if (this.AutoComPortOpen == true)
            {
                //Hsinson 930628�s�W�ҰʧY�۰ʶ}�_Com-port�ö}�l�郞���s�u
            }
        }
        #endregion

        #region 3.ConnectToTBS TaskDBG���]�w
        public void ConnectToTBS()
        {
            TaskDBG = new TaskDBGForm(TaskName); // �䤤 TaskName ���z�ۤv�� Task �W�١A�ӷ����ӬO�z�� ini ��			
            bool AutoConnect, ShowDBGForm;
            try
            {
                AutoConnect = bool.Parse(Profile.GetValue(Filename, "Param", "AutoConnect")); // �ˬd�O�_�w�]���۰ʰ���
                ShowDBGForm = bool.Parse(Profile.GetValue(Filename, "Param", "ShowDBGForm")); // �ˬd�O�_�w�]����ܰ�������
            }
            catch (Exception)
            {
                this.TaskDBG.ClientTask.Show("ERROR   - Ū�� ini �ɮɵo�Ϳ��~�I");
                AutoConnect = false; // �p�L�kŪ�ɡA�۰ʳs�u�]�w�� false
                ShowDBGForm = false; // �p�L�kŪ�ɡA��ܰ��������]�w�� false
            }
            if (AutoConnect)
            {
                this.TaskDBG.ClientTask.Show("INFO    - Auto Connection = true");
                this.TaskDBG.ClientTask.Show("INFO    - �}�l�۰ʳs�u�@�~.............");
                TaskDBG.StartDBGFormProcess();  // �۰ʰ��� StartDBGFormProcess() �ӱҰ� DBG ���\��I
            }
            else
            {
                this.TaskDBG.ClientTask.Show("INFO    - �L�۰ʳs�u�]�w�A�Ф�ʱҰʳs�u�I");
            }
            if (ShowDBGForm)
            {
                this.TaskDBG.Visible = true;
            }
            else
            {
                this.TaskDBG.Visible = false;
            }
        }
        #endregion

        #region 4.myInitial_Delegate
        public void myInitial_Delegate()
        {
            this.OnDisplayStringMessage += new Display_StringMessage_Handler(this.DisplayStringMessage);
            //this.TaskDBG.OnDisplay_StringMessage	+= new Display_StringMessage_Handler(this.DisplayStringMessage);			
            this.OnProcessReceiveBusMessage += new ProcessReceiveBusMessage_Handler(this.ProcessReceiveBusMessage);
            //this.TaskDBG.OnShowDBG_ReceiveData		+= new ShowDBG_ReceiveData_Handler(this.ShowDBG_ReceiveData);
            this.OnDisplay_PortStatus += new Display_PortStatus_Handler(this.Display_PortStatus);
            this.myPortStatus.OnReload_PortStatus += new Reload_PortStatus_Handler(this.Reload_PortStatus);
            this.OnFHOUR += new FHOUR_Handler(this.CCI_FHOUR);
            this.OnFRAWC += new FRAWC_Handler(this.CCI_FRAWC);
            this.OnComPort_Init += new ComPort_Init_Handler(this.ComPort_Init);
            this.OnAddTreeView += new AddTreeView_Handler(this.AddTreeView);
            this.OnValidateReceiveComMessage += new ValidateReceiveComMessage_Handler(this.ValidateReceiveComMessage);
            this.OnOpenComPort += new OpenComPort_Handler(this.OpenComPort);
            this.OnCloseComPort += new CloseComPort_Handler(this.CloseComPort);
            this.OnTreeView_Update += new TreeView_Update_Handler(this.TreeViewUpdate);
            this.OnProcessReceviceComMessage += new ProcessReceiveComMessage_Handler(this.ProcessReceiveComMessage);
            this.OnProcessPlayBackMessage += new ProcessPlayBackMessage_Handler(this.ProcessPlayBackMessage);
            this.OnAddToControlQueue += new AddToControlQueue_Handler(this.AddToControlQueue);
            this.OnSendtoQueue += new SendtoQueue_Handler(this.SendtoQueue);
            //this.TaskDBG.OnReceiveMsg				+= new ReceiveMsg_Handler(this.ReceiveMsg);
        }
        #endregion

        /// <summary>
        /// ���ε{�����D�i�J�I�C
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new CCI_Form());
        }

        //----------------------------------------------

        //���{����Ϥưʧ@
        public void fnFrontendDataCopyInit()
        {
            ///  test area

            //m_IntFilterType.LoadDBFilterType();

            //int intKwType = 2;
            //vstrContent = " 4/5/2020 3:03:48 PM    VEH VEH169               3   1       X   Door Block Failure in TRN169 R/S 3/12";
            //strA = "Door Block Failure";
            //strB = "3   1       X";
            //strC = "strC";
            //strD = "strD";
            //strNotflag = "";
            //bool filter_stat;
            ////filter_stat=m_IntFilterType.fnAddDefineCheckPattern(intKwType, vstrContent, strA, strB, strC, strD, strNotflag);

            //filter_stat = fnCheckPattern(intKwType, vstrContent, strA, strB, strC, strD);
            ///



            intIntervalGetDEDB = Convert.ToInt32(this.txb_AutoPolling.Text);
            intIntervalGetintelligentDB = Convert.ToInt32(this.txb_AutoReceive.Text);
            intDataQPerGet = Convert.ToInt32(this.txb_AutoCFJudge.Text);

            //this.Text = this.strFormText + " ScadaDBMF 20200804";
            this.Text = this.strFormText;

            this.OnDisplayStringMessage("Ū���t�γ]�wini��");
            this.OnDisplayStringMessage("�ˬdRTDB�PMainDB���O�_�ۮe��FEDB");

            #region �ˬd


            this.dtSourceTables = this.fnGetSourceDBSchema();
            if (this.blnLocalDBEnable == true)
            {
                if (!fnCheckRTDBDBColumn())
                {
                    throw new Exception("RTDB   ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw ����ˬd���`");
                }

                if (!fnCheckMainDBDBColumn())
                {
                    throw new Exception("MainDB ��m(" + this.strMainDBSQLServer_IP + ") " + this.strMainDBSQLServer_RXTable + " ��Ʈw ����ˬd���`");
                }
            }

            #endregion

            // Ū����Ʈw���� ����r �L�o����
            m_IntFilterType.LoadDBFilterType();

            if (this.blnItlDBDBEnable == true)
            {
                fnGetIntBase();
                fnGetMultipleAlarm();
            }

            //this.OnDisplayStringMessage("�ƻs�Ҧ��|����s�����v���" );
            //fnFrontendDataCopy();
            this.OnDisplayStringMessage("�}�l�w�ɸ�Ƨ�s�@�~");

            this.tmRXM.Interval = intIntervalGetDEDB;
            this.tmRXM.Enabled = true;

        }


        //�ƻs��ƥD�{��
        void fnFrontendDataCopy()
        {

            decimal decSourceDBMaxSeq;
            decimal decLocalDBMaxSeq;
            decimal decSourceDBMinSeq;
            DataTable dtTmp;

            // // Ovline�P�B(�NRTDB����ƪ��A�P�B��e�ݹq��)
            if (this.m_bOvlineEnable == true)
            {
                this.m_nSyncTimeCount++;

                if (this.m_nSyncTimeCount >= 20)
                {
                    this.m_nSyncTimeCount = 0;

                    Ovline_sync OvSync = new Ovline_sync();
                    OvSync.FrontOvline_Sync(this.m_strOvlineRTDB, this.m_strOvlineRTDBTable, this.m_strOvlineRTDBIP,
                                                this.m_strOvlineFrontDB, this.m_strOvlineFrontTable, this.m_strOvlineFrontIP);
                }

            }


            //���oFrontEnd�̤j��seq
            decSourceDBMaxSeq = fnGetSourceDBMaxSeq();
            if (decSourceDBMaxSeq < 1)
            {
                this.OnDisplayStringMessage("fnFrontendDataCopy: ���oFrontEnd max seq = " + decSourceDBMaxSeq + " ���`");
                return;
            }
            decSourceDBMinSeq = fnGetSourceDBMinSeq();
            if (decSourceDBMinSeq < 1)
            {
                this.OnDisplayStringMessage("fnFrontendDataCopy: ���oFrontEnd min seq = " + decSourceDBMinSeq + " ���`");
                return;
            }

            decLocalDBMaxSeq = 0;

            // �Y���Ұ� Local database
            //if (blnLocalDBEnable == true) 
            if (blnWinSysEvtDBEnable == true && WinSysEvtDBMaxSeq == 0)
            {
                //���oRTDB�̤j��seq
                //decLocalDBMaxSeq = fnGetLocalDBMaxSeq();
                WinSysEvtDBMaxSeq = fnGetLocalDBMaxSeq();
                //if (decLocalDBMaxSeq < 1)
                if (WinSysEvtDBMaxSeq < 1)
                {
                    this.OnDisplayStringMessage("fnFrontendDataCopy: ���oFrontEnd min seq = " + decLocalDBMaxSeq + " ���`");
                    return;
                }
            }
            else if (m_blnLocalDB_OneDay_Enable == true)
            {
                //���oRTDB�̤j��seq
                decLocalDBMaxSeq = fnGetLocalDBMaxSeq_OneDay();
                if (decLocalDBMaxSeq < 1)
                {
                    this.OnDisplayStringMessage("fnFrontendDataCopy: ���oFrontEnd min seq = " + decLocalDBMaxSeq + " ���`");
                    return;
                }
            }

            //TODO: ���ըϥ�
            //980502Hsinson test ���եε{�����q,�����ջݭn�ɤ~�}�ҤU�C�T��{���X
            //decSourceDBMaxSeq = 3022;
            //decSourceDBMinSeq = decSourceDBMaxSeq - 3;
            //decLocalDBMaxSeq = decSourceDBMinSeq;


          if (decSourceDBMinSeq > decLocalDBMaxSeq) decLocalDBMaxSeq = decSourceDBMinSeq;
            
          if (this.DBG_ReceiveData.Checked)
            {
                this.OnDisplayStringMessage("fnFrontendDataCopy: ���oFrontEnd max seq (Source, Local)= (" + decSourceDBMaxSeq + ", " + decLocalDBMaxSeq + ")");
            }

            //�Y��Ƭ۵��A�h�������ΰʧ@
            //if (decSourceDBMaxSeq <= decLocalDBMaxSeq) return;
            if (decSourceDBMaxSeq <= WinSysEvtDBMaxSeq)
            {
                WinSysEvtDBMaxSeq = decSourceDBMaxSeq;
                return;
            }

            //���o�ݭnCopy�����	
            //dtTmp = fnGetDBMoveData(decLocalDBMaxSeq, decSourceDBMaxSeq);
            if (strTLOS_NHDBSQLServer_DataBase == "TLOS_NH")
                dtTmp = fnGetDBMoveData(decSourceDBMaxSeq-500, decSourceDBMaxSeq);
            else
                dtTmp = fnGetDBMoveData(WinSysEvtDBMaxSeq, decSourceDBMaxSeq);

            if (this.DBG_ReceiveData.Checked)
            {
                decimal qlength = decSourceDBMaxSeq - decLocalDBMaxSeq;
                this.OnDisplayStringMessage("fnFrontendDataCopy: �h����� " + dtTmp.Rows.Count + "/" + qlength + " ��.");
                if (qlength >= 1000)
                {
                    MonAlarmCount++;
                    if (MonAlarmCount > 32760) MonAlarmCount = 32760; //����O�@
                    if (MonAlarmCount % 600 == 3)                 //�C600+3���o�X�@���iĵ
                    {
                        WriteMonMsg("DBMF Queue Overflow��C���׷���(Queue=" + qlength + ",AlarmCount=" + MonAlarmCount + ")");
                    }
                }
                else
                {
                    MonAlarmCount = 0;
                }

            }


            if (dtTmp.Rows.Count > 0)
            {
                //�i��ƻs�@�~
                try
                {
                    //�ƻs�iAlarm DB		
                    if (this.blnAlarmDBEnable) fnSetIntellectDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("Alarm DB ��ƽƻs�@�~���`: " + Ex.Message);

                }




                //Hsinson970927 add
                try
                {
                    //�ƻs�iRTDB			//�n�g�����Ʈw���׹L�����{��
                    if (this.blnLocalDBEnable) fnSetRTDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("RTDB ��ƽƻs�@�~���`: " + Ex.Message);

                }



                // Jacky_su 980504 add
                try
                {
                    //�ƻs�iRTDB (by one day)			//�n�g�����Ʈw���׹L�����{��
                    if (this.m_blnLocalDB_OneDay_Enable) fnSetRTDBMoveDataOneDay(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("RTDB (One day table) ��ƽƻs�@�~���`: " + Ex.Message);

                }


                //Hsinson970927 add
                try
                {

                    //�ƻs�iMainDB			
                    if (this.blnMainDBEnable) fnSetMainDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("MainDB ��ƽƻs�@�~���`: " + Ex.Message);
                }

                // Jacky_su 980504 add
                try
                {

                    //�ƻs�iMainDB			
                    if (this.m_blnMainDBEnable_OneDay) fnSetMainDBMoveDataOneDay(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("MainDB ��ƽƻs�@�~���`: " + Ex.Message);
                }

                //Hsinson970927 add
                try
                {

                    //�ƻs�iNetDC
                    if (this.blnNetDCDBEnable) fnSetNetDCDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("NetDC ��ƽƻs�@�~���`: " + Ex.Message);
                }

                //����������������        NeihuEVTDBNMoveDataOneDay        ����������������//

                try
                {
                    //�ƻs�im_NhEVTDBNMoveDataOneDay			//�n�g�����Ʈw���׹L�����{��
                    if (this.m_NhEVTDBNMoveDataOneDay_Enable) fnSetNhEVTDBMoveDataOneDay(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("RTDB ��ƽƻs�@�~���`: " + Ex.Message);

                }

                //����������������        NeihuEVTDBNMoveDataOneDay    end        ����������������//


                //SAMMY20200406 ADD EQBIAS ��ƪ�
                try
                {
                    //�ƻs�iBIAS��ƪ� Select * from EQBias
                    //fnSetBiasDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("BIAS ��ƽƻs�@�~���`: " + Ex.Message);
                }


                //Hsinson970927 add
                try
                {

                    //�ƻs�iPMS: Process  Monitor Status
                    if (this.blnPmsDBEnable) fnSetPmsDBMoveData(dtTmp);

                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("PMS ��ƽƻs�@�~���`: " + Ex.Message);
                }

                //Hsinson970927 add
                try
                {
                    //�ƻs�iEQDB
                    if (this.blnEQDBEnable) fnSetEarthQuakeDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("EQDB ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //SAMMY 20191007 ADD EQ20
                try
                {
                    //�ƻs�iEQDB
                    if (this.blnEQDBEnable_EQ20) fnSetEarthQuakeDBMoveData_EQ20(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("EQDB_EQ20 ��ƽƻs�@�~���`: " + Ex.Message);
                }


                //Hsinson970927 add
                try
                {
                    //�ƻs�iAtsEventDB
                    if (this.blnAtsEventDBEnable) fnSetAtsEventDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("AtsEventDB ��ƽƻs�@�~���`: " + Ex.Message);
                }


                //Hsinson970927 add
                try
                {

                    //�ƻs�iWinSysEvtDB
                    if (this.blnWinSysEvtDBEnable) fnSetWinSysEvtDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("WinSysEvtDB ��ƽƻs�@�~���`: " + Ex.Message);
                }

                //Wilson1090927 add for NHDB
                try
                {

                    //�ƻs�iTrainLocationByTrack
                    if (this.blnTLOS_NHDBEnable) fnSetTLos_NHDBMoveDataOPCA(dtTmp);
                    if (this.blnTLOS_NHDBEnable) fnSetTLos_NHDBMoveDataOPCC(dtTmp);
                    if (this.blnTLOS_NHDBEnable) fnSetTLos_NHDBMoveDataOPCB(dtTmp);
                    
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("TLOS_NHDB ��ƽƻs�@�~���`: " + Ex.Message);
                }

                //Kelvin, 2010/12/24, starting
                try
                {
                    if (this.blnDBSpaceDBEnable) fnSetDBSpaceDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("DBSpaceDB ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //Kelvin, 2010/12/24, ending

                //SFI 20121114 Add PSD
                try
                {
                    if (this.blnPSDEventDBEnable) fnSetPSDEventDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("PSD ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //SFI 20121114 Add end

                //20130207 SFI Add for EMDS CountDown
                try
                {
                    if (this.blnEMDSCountDown_DBEnable) fnSetEMDSCountDownDBMoveData(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("EMDSCountDown ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //SFI 20130207 Add end

                //20130207 SFI Add for VehDistance
                try
                {
                    //if (VehDistanceMove.blnVehDistance_DBEnable) VehDistanceMove.Move(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("VehDistance ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //SFI 20130207 Add end
                
                //2020804 SFI Add for �_���x���x����
                try
                {
                    //if (PTTRTCDCS_EventMove.blnPTTRTCDCS_Event_DBEnable) PTTRTCDCS_EventMove.Move(dtTmp);
                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("TRTCDCS ��ƽƻs�@�~���`: " + Ex.Message);
                }
                //SFI 2020804 Add end
                
                //20200826 SAMMY Add for ĵ���iĵ
                try
                {
                    //if (this.blnWakeUpMouse_DBEnable) fnSetWakeUpMouseDBMoveData(dtTmp);

                }
                catch (Exception Ex)
                {
                    this.OnDisplayStringMessage("WakeUp_Mouse ��ƽƻs�@�~���`: " + Ex.Message);
                }



                this.OnDisplayStringMessage(" ");  //�[�L�@��ťզ�
            }

        }



        #region ��ƽƻs�@�~����function


        //�ƻs�iEarthQuakeDB
        void fnSetEarthQuakeDBMoveData(DataTable vdtData)
        {

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]EarthQuake]%' And Source = 'ID.SCA.EQU_CWB'";
            try
            {
                lock (this)
                {
                    strConn = @"server=" + this.strEQDBSQLServer_IP + ";uid=" + this.strEQDBSQLServer_User + ";pwd=" + this.strEQDBSQLServer_Password + ";database=" + this.strEQDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {

                        this.OnDisplayStringMessage("���n�g�JEarthQuakeDB  ��m(" + this.strEQDBSQLServer_IP + ") " + this.strEQDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {
                        strSQL = "";
                        strSQL = fnGetEQDBSQL(dvData[i].Row["Content"].ToString());
                        if (strSQL.Length > 0)
                        {
                            CmdDB.CommandText = strSQL;
                            CmdDB.ExecuteNonQuery();
                        }

                    }

                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JEarthQuakeDB  ��m(" + this.strEQDBSQLServer_IP + ") " + strEQDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JEarthQuakeDB  ��m(" + this.strEQDBSQLServer_IP + ") " + this.strEQDBSQLServer_Table + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }
 
        }
        void fnSetEarthQuakeDBMoveData_EQ20(DataTable vdtData)
        {

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]EarthQuake]%' And Source <> 'ID.SCA.EQU_CWB'";
            try
            {
                lock (this)
                {
                    strConn = @"server=" + this.strEQDBSQLServer_IP + ";uid=" + this.strEQDBSQLServer_User + ";pwd=" + this.strEQDBSQLServer_Password + ";database=" + this.strEQDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {

                        this.OnDisplayStringMessage("���n�g�JEarthQuakeDB  ��m(" + this.strEQDBSQLServer_IP + ") " + this.strEQDBSQLServer_Table_EQ20 + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {
                        strSQL = "";
                        strSQL = fnGetEQDBSQL_EQ20(dvData[i].Row["Content"].ToString());
                        if (strSQL.Length > 0)
                        {
                            CmdDB.CommandText = strSQL;
                            CmdDB.ExecuteNonQuery();
                        }

                    }

                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JEarthQuakeDB_EQ1O  ��m(" + this.strEQDBSQLServer_IP + ") " + strEQDBSQLServer_Table_EQ20 + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JEarthQuakeDB  ��m(" + this.strEQDBSQLServer_IP + ") " + this.strEQDBSQLServer_Table_EQ20 + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        //�ƻs�iWakeUp_Mouse ��ƪ�
        void fnSetWakeUpMouseDBMoveData(DataTable vdtData)
        {

            //SAMMY ADD WAKEUP_MOUSE ��ƪ�
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]WakeUp_Mouse]%'";
            //dvData.Sort="seq";

            int iRowUpdate; 
            try
            {
                lock (this)
                {
                    strConn = @"server=" + this.strWakeUpMouse_DBSQLServer_IP + ";uid=" + this.strWakeUpMouse_DBSQLServer_User + ";pwd=" + this.strWakeUpMouse_DBSQLServer_Password + ";database=" + this.strWakeUpMouse_DBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JWakeUp Mouse��m(" + this.strWakeUpMouse_DBSQLServer_IP + ") RTDB_SCADA.[dbo].[WakeUp_Mouse] ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }
 
                    for (int i = 0; i < dvData.Count; i++)
                    {

                        //sContent = "[WakeUp_Mouse];NodeName=ValueName;NodeTime=ValueTime;NodeMemo=ValueMemo;NodeValue=Value;
                        SortedList sltData =  fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        string NodeName = (string)sltData["NodeName"];
                        string NodeTime = (string)sltData["NodeTime"];
                        string NodeValue = (string)sltData["NodeValue"];
                        string NodeType = (string)sltData["NodeType"];
                        string NodeLocal = (string)sltData["NodeLocal"];
                        string NodeDevice = (string)sltData["NodeDevice"];
                        string NodeServer = (string)sltData["NodeServer"];
                        strSQL = "insert into WakeUp_Mouse(NodeName, NodeTime, NodeValue, NodeType, NodeLocal, NodeDevice, NodeServer)" +
                                 " values('" + NodeName + "', '" + NodeTime + "', '" + NodeValue + "','" + NodeType + "','" + NodeLocal + "','" + NodeDevice + "', '" + NodeServer + "')";

                        CmdDB.CommandText = strSQL;	//��s�ܧY�ɸ�Ʈw
                        CmdDB.ExecuteNonQuery();
                    }//for
                  
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JWakeUp_Mouse��m(" + this.strWakeUpMouse_DBSQLServer_IP + ") RTDB_SCADA.[dbo].[WakeUp_Mouse] ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                }
            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JWakeUp_Mouse��m(" + this.strWakeUpMouse_DBSQLServer_IP + ") RTDB_SCADA.[dbo].[WakeUp_Mouse] ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                 
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }
        }

        #region �B�zEQ������function
        string GetCenterRcvTime(string strConn)
        {
            string strSQL = " SELECT RcvTime FROM EarthQuake WHERE (STA = 'Center') ";
            DataSet mydataset;
            SqlConnection conG9 = null;
            SqlDataAdapter da;
            try
            {
                lock (this)
                {

                    mydataset = new DataSet();
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    //���osource�̤j��seq
                    if (mydataset.Tables[0].Rows.Count == 0)
                    {
                        return "0";
                    }
                    else
                    {
                        //return mydataset.Tables[0].Rows[0]["Alarm_ID"].ToString();
                        return mydataset.Tables[0].Rows[0][0].ToString();
                    }
                }//lock
            }//try
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("GetCenterRcvTime: ��Ʀs�����`(" + Ex.Message + "),��Ʈw�s���r��[" + strConn + "], ��ƪ��s���R�O[" + strSQL + "] ");
                if (conG9 != null)
                    conG9.Close();
                return "0";
                //throw Ex;
            }
        }//................................................fnGetOvLineData



        //�HCenter�ˬd���צA�~��DBMF�{��������
        void DoCenterWait(string strConn)
        {
            tmRXM.Enabled = false;

            string CenterRcvTime = "0";
            do
            {
                CenterRcvTime = GetCenterRcvTime(strConn);
                this.OnDisplayStringMessage("�_���o�ͦa�_�ɶ�=" + CenterRcvTime);
                if (CenterRcvTime != "0")
                {
                    this.OnDisplayStringMessage("�o�{�_���a�_�ɶ�����null�A�����ı10����,���ݦa�_�y���{�����浲�שR�O");
                    System.Threading.Thread.Sleep(10000);
                }
            } while (CenterRcvTime != "0");

            this.OnDisplayStringMessage("������a�_�y���{���w���浲�שR�O�A�G�~��פJ�a�_���v���");


            tmRXM.Enabled = true;

        }

        //���o�a�_�ninsert��TSQL
        string fnGetEQDBSQL(string vstrContent)
        {
            string[] strArytmp = vstrContent.Split(new char[] { ';' });
            string strTimeRcv = strArytmp[1];
            string strReport = strArytmp[2].Split(new char[] { '.' })[0];
            //string strSQL="";


            if (strArytmp[3].StartsWith("CENTER"))
            {
                //��H�����Ĥ@��
                // Hsinson 951017 �Y�n��DBMF�۰��ˬd�e�@���a�_��ƬO�_�w�������סA�h�N�U�C�Ʀ�{���X�Ѷ}�Y�i���ݵ��׫�A�g�JEarthQuake
                //				if (!SkipNextCenterFlag ) //951015Hsinson: �ΨӳB�z�a�_IDS.TXT�ᤧ�a�_�T��TRTC.TXT���P�@�զa�_,�G��wait����
                //				{
                //					DoCenterWait(strConn);
                //				}
                //				else
                //				{
                //					this.OnDisplayStringMessage("�x�_���B�o��IDMO�a�_�A���򱵦���H���a�_���");
                //				}
                //
                //				SkipNextCenterFlag =false;

                return fnGetCenterTitleSQL(strArytmp[3], strTimeRcv, strReport);


            }
            else
            {

                // Hsinson 951017 �Y�n��DBMF�۰��ˬd�e�@���a�_��ƬO�_�w�������סA�h�N�U�C�Ʀ�{���X�Ѷ}�Y�i���ݵ��׫�A�g�JEarthQuake
                //				if (strArytmp[3].StartsWith ("IDMO") || (strReport.StartsWith("IDS")))
                //				{
                //					SkipNextCenterFlag = true;
                //					this.OnDisplayStringMessage("�x�_���BIDS�a�_=" + strArytmp[3].ToString());
                //
                //				}
                //
                return fnGetCenterContentSQL(strArytmp[3], strTimeRcv, strReport);

            }


            //			if (strReport=="TRTCW" || strReport=="TRTCP" || strReport=="TRTC")
            //			{
            //				
            //				if (strArytmp[3].StartsWith ("CENTER"))
            //				{
            //					//center ��title
            //					return fnGetCenterTitleSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //				else
            //				{
            //					//center ��content
            //					return fnGetCenterContentSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //
            //
            //			}
            //			else if (strReport.StartsWith("EQ10"))
            //			{
            //				if (strArytmp[3].IndexOf("/")>=0)
            //				{
            //					//TRTC��title
            //					return fnGetTrtcTitleSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //				else
            //				{
            //					//TRTC�����e
            //					return fnGetTrtcContentSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //
            //			}


            //			return strSQL;

        }
        string fnGetEQDBSQL_EQ20(string vstrContent)
        {
            string[] strArytmp = vstrContent.Split(new char[] { ';' });
            string strTimeRcv = strArytmp[1];
            string strReport = strArytmp[2].Split(new char[] { '.' })[0];
            //string strSQL="";


            if (strArytmp[3].StartsWith("CENTER"))
            {
                //��H�����Ĥ@��
                // Hsinson 951017 �Y�n��DBMF�۰��ˬd�e�@���a�_��ƬO�_�w�������סA�h�N�U�C�Ʀ�{���X�Ѷ}�Y�i���ݵ��׫�A�g�JEarthQuake
                //				if (!SkipNextCenterFlag ) //951015Hsinson: �ΨӳB�z�a�_IDS.TXT�ᤧ�a�_�T��TRTC.TXT���P�@�զa�_,�G��wait����
                //				{
                //					DoCenterWait(strConn);
                //				}
                //				else
                //				{
                //					this.OnDisplayStringMessage("�x�_���B�o��IDMO�a�_�A���򱵦���H���a�_���");
                //				}
                //
                //				SkipNextCenterFlag =false;

                return fnGetCenterTitleSQL_EQ20(strArytmp[3], strTimeRcv, strReport);


            }
            else
            {

                // Hsinson 951017 �Y�n��DBMF�۰��ˬd�e�@���a�_��ƬO�_�w�������סA�h�N�U�C�Ʀ�{���X�Ѷ}�Y�i���ݵ��׫�A�g�JEarthQuake
                //				if (strArytmp[3].StartsWith ("IDMO") || (strReport.StartsWith("IDS")))
                //				{
                //					SkipNextCenterFlag = true;
                //					this.OnDisplayStringMessage("�x�_���BIDS�a�_=" + strArytmp[3].ToString());
                //
                //				}
                //
                return fnGetCenterContentSQL_EQ20(strArytmp[3], strTimeRcv, strReport);

            }


            //			if (strReport=="TRTCW" || strReport=="TRTCP" || strReport=="TRTC")
            //			{
            //				
            //				if (strArytmp[3].StartsWith ("CENTER"))
            //				{
            //					//center ��title
            //					return fnGetCenterTitleSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //				else
            //				{
            //					//center ��content
            //					return fnGetCenterContentSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //
            //
            //			}
            //			else if (strReport.StartsWith("EQ10"))
            //			{
            //				if (strArytmp[3].IndexOf("/")>=0)
            //				{
            //					//TRTC��title
            //					return fnGetTrtcTitleSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //				else
            //				{
            //					//TRTC�����e
            //					return fnGetTrtcContentSQL(strArytmp[3],strTimeRcv,strReport);
            //				}
            //
            //			}


            //			return strSQL;

        }

        //���o��H���a�_��ƪ��Ĥ@��
        string fnGetCenterTitleSQL(string vstrData, string vstrTimeRcv, string vstrReport)
        {

            int intTmp = vstrData.IndexOf(" ");
            string strStnCode = vstrData.Substring(0, intTmp).Trim();
            string strData = vstrData.Substring(intTmp + 2);
            string strDate = strData.Substring(0, 8);		//�~���
            string strTime = strData.Substring(8, 4);		//�ɤ�
            string strSecond = strData.Substring(12, 6);	//��

            //Hsinson941219�o�{�g�n���������A�ˡA�ҥH�N���������^�ӡA�ק�p�U
            //			string strPointX=strData.Substring (18,2);	//�g
            //			string strPointXm=strData.Substring (20,5);	//��
            //			string strPointY=strData.Substring (25,3);		//�n
            //			string strPointYm=strData.Substring (28,5);		//��
            string strPointY = strData.Substring(18, 2);	//�n
            string strPointYm = strData.Substring(20, 5);	//��
            string strPointX = strData.Substring(25, 3);		//�g
            string strPointXm = strData.Substring(28, 5);		//��

            string strDep = strData.Substring(33, 6);		//�`��
            string strScope = strData.Substring(39, 4);		//�W��

            float fltPointX = (float.Parse(strPointX) + float.Parse(strPointXm) / 60);
            float fltPointY = (float.Parse(strPointY) + float.Parse(strPointYm) / 60);

            string strSQL;

            //			strSQL="insert EarthQuake(TimeRcv,ReportType,PointX,PointY,Dep,Scope,TxtNum)" +
            //				"values('"+ vstrTimeRcv +"','"+ vstrReport +"','"+ fltPointX +"','"+ fltPointY +"','"+ strDep +"','"+ strScope +"','"+ vstrTimeRcv +"')";


            strSQL = "update " + this.strEQDBSQLServer_Table + " set RcvTime = '" + vstrTimeRcv + "', ReportType='" + vstrReport + "' , Lon='" + fltPointX + "'," +
                " Lat='" + fltPointY + "',Dep='" + strDep + "',Scope='" + strScope + "' " +
                "where STA='" + strStnCode + "'";


            return strSQL;

        }
        string fnGetCenterTitleSQL_EQ20(string vstrData, string vstrTimeRcv, string vstrReport)
        {

            int intTmp = vstrData.IndexOf(" ");
            string strStnCode = vstrData.Substring(0, intTmp).Trim();
            string strData = vstrData.Substring(intTmp + 2);
            string strDate = strData.Substring(0, 8);		//�~���
            string strTime = strData.Substring(8, 4);		//�ɤ�
            string strSecond = strData.Substring(12, 6);	//��

            //Hsinson941219�o�{�g�n���������A�ˡA�ҥH�N���������^�ӡA�ק�p�U
            //			string strPointX=strData.Substring (18,2);	//�g
            //			string strPointXm=strData.Substring (20,5);	//��
            //			string strPointY=strData.Substring (25,3);		//�n
            //			string strPointYm=strData.Substring (28,5);		//��
            string strPointY = strData.Substring(18, 2);	//�n
            string strPointYm = strData.Substring(20, 5);	//��
            string strPointX = strData.Substring(25, 3);		//�g
            string strPointXm = strData.Substring(28, 5);		//��

            string strDep = strData.Substring(33, 6);		//�`��
            string strScope = strData.Substring(39, 4);		//�W��

            float fltPointX = (float.Parse(strPointX) + float.Parse(strPointXm) / 60);
            float fltPointY = (float.Parse(strPointY) + float.Parse(strPointYm) / 60);

            string strSQL;

            //			strSQL="insert EarthQuake(TimeRcv,ReportType,PointX,PointY,Dep,Scope,TxtNum)" +
            //				"values('"+ vstrTimeRcv +"','"+ vstrReport +"','"+ fltPointX +"','"+ fltPointY +"','"+ strDep +"','"+ strScope +"','"+ vstrTimeRcv +"')";


            strSQL = "update " + this.strEQDBSQLServer_Table_EQ20 + " set RcvTime = '" + vstrTimeRcv + "', ReportType='" + vstrReport + "' , Lon='" + fltPointX + "'," +
                " Lat='" + fltPointY + "',Dep='" + strDep + "',Scope='" + strScope + "' " +
                "where STA='" + strStnCode + "'";


            return strSQL;

        }

        //���o��H���a�_��Ƥ��e
        string fnGetCenterContentSQL(string vstrData, string vstrTimeRcv, string vstrReport)
        {
            //int intTmp=vstrData.IndexOf(" ");
            string strSQL = "";
            //           Hsinson 951017�N�C�@���a�_��Ƨ��[�J���קP�_�A�H�K�^�����~
            //			if (vstrData.Length >2)
            if (vstrData.Length > 75)
            {
                string strStnCode = vstrData.Length > 5 ? vstrData.Substring(0, 6).Trim() : ""; //�a�_�����x
                string strGrade = vstrData.Length > 76 ? vstrData.Substring(75, 2).Trim() : ""; //�_��(�˼� �ĤG��)
                string strGal = vstrData.Length > 82 ? vstrData.Substring(77, 6).Trim() : "";   //�[�t��(�̫�@��)
                string strPGV = vstrData.Length > 87 ? vstrData.Substring(83,5).Trim(): "";     //�t��

                switch (strGrade)
                { 
                    case "-5":
                        strGrade = "5-";  break;
                    case "+5":
                        strGrade = "5+"; break;
                    case "-6":
                        strGrade = "6-"; break;
                    case "+6":
                        strGrade = "6+"; break;
                }


                //strSQL="insert EarthQuake(TimeRcv,ReportType,Grade,Gal)values('"+ vstrTimeRcv +"','"+ vstrReport +"','"+ strGrade +"','"+ strGal +"')";

                strSQL = "update " + this.strEQDBSQLServer_Table + " set RcvTime = '" + vstrTimeRcv + "' , ReportType= '" + vstrReport + 
                        "', Degree= '" + strGrade + "', Gal='" + strGal + "', PGV='" + strPGV + "'" + 
                        " Where STA='" + strStnCode + "'";
            }
            return strSQL;
        }
        string fnGetCenterContentSQL_EQ20(string vstrData, string vstrTimeRcv, string vstrReport)
        {
            //int intTmp=vstrData.IndexOf(" ");
            string strSQL = "";
            //           Hsinson 951017�N�C�@���a�_��Ƨ��[�J���קP�_�A�H�K�^�����~
            //			if (vstrData.Length >2)
            if (vstrData.Length > 75)
            {
                string strStnCode = vstrData.Length > 5 ? vstrData.Substring(0, 6).Trim() : ""; //�a�_�����x
                string strGrade = vstrData.Length > 76 ? vstrData.Substring(75, 2).Trim() : ""; //�_��(�˼� �ĤG��)
                string strGal = vstrData.Length > 82 ? vstrData.Substring(77, 6).Trim() : "";   //�[�t��(�̫�@��)
                string strPGV = vstrData.Length > 87 ? vstrData.Substring(83, 5).Trim() : "";     //�t��

                switch (strGrade)
                {
                    case "-5":
                        strGrade = "5-"; break;
                    case "+5":
                        strGrade = "5+"; break;
                    case "-6":
                        strGrade = "6-"; break;
                    case "+6":
                        strGrade = "6+"; break;
                }

                //strSQL="insert EarthQuake(TimeRcv,ReportType,Grade,Gal)values('"+ vstrTimeRcv +"','"+ vstrReport +"','"+ strGrade +"','"+ strGal +"')";

                strSQL = "update " + this.strEQDBSQLServer_Table_EQ20 + " set RcvTime = '" + vstrTimeRcv + "' , ReportType= '" + vstrReport + 
                        "', Degree= '" + strGrade + "', Gal='" + strGal + "', PGV='" + strPGV + "'" +
                        " where STA='" + strStnCode + "'";
            }
            return strSQL;
        }

        //���oTRTC�a�_��ƪ��Ĥ@��
        string fnGetTrtcTitleSQL(string vstrData, string vstrTimeRcv, string vstrReport)
        {

            string strSQL = "";
            string[] strTmp = vstrData.Split(new char[] { ',' });


            string strTimeRcv = strTmp[0];


            string strMaxGrade = strTmp[1];
            string strGroupGrade = strTmp[2];

            strSQL = "update EarthQuake set timeRcv= '" + strTimeRcv + "', MaxGrade='" + strMaxGrade + "', GroupGrade='" + strGroupGrade + "'    where stnCode in ('ID0','ID1','ID2','ID3','ID4','ID5','ID6','ID7','ID8','ID9')";

            return strSQL;
        }
        //���oTRTC_EQ20�a�_��ƪ��Ĥ@��(�L�ϥ�)
        string fnGetTrtcTitleSQL_EQ20(string vstrData, string vstrTimeRcv, string vstrReport)
        {

            string strSQL = "";
            string[] strTmp = vstrData.Split(new char[] { ',' });


            string strTimeRcv = strTmp[0];


            string strMaxGrade = strTmp[1];
            string strGroupGrade = strTmp[2];

            strSQL = "update EarthQuake_EQ20 set timeRcv= '" + strTimeRcv + "', MaxGrade='" + strMaxGrade + "', GroupGrade='" + strGroupGrade + "'    where stnCode in ('ID0','ID1','ID2','ID3','ID4','ID5','ID6','ID7','ID8','ID9','ID10','ID11','ID12','ID13','ID14','ID15','ID16','ID17','ID18','ID19')";

            return strSQL;
        }

        //���oTRTC�a�_��Ƥ��e
        string fnGetTrtcContentSQL(string vstrData, string vstrTimeRcv, string vstrReport)
        {
            string strSQL = "";
            string[] strArytmp = vstrData.Split(new char[] { ',' });
            string strStnCode = strArytmp[0];
            string strGrade = strArytmp[1];
            string strGal = strArytmp[2];

            strSQL = "update EarthQuake set Grade='" + strGrade + "', Gal='" + strGal + "' where StnCode='" + strStnCode + "'";

            return strSQL;
        }

        //���oTRTC_EQ20�a�_��Ƥ��e(�L�ϥ�)
        string fnGetTrtcContentSQL_EQ20(string vstrData, string vstrTimeRcv, string vstrReport)
        {
            string strSQL = "";
            string[] strArytmp = vstrData.Split(new char[] { ',' });
            string strStnCode = strArytmp[0];
            string strGrade = strArytmp[1];
            string strGal = strArytmp[2];

            strSQL = "update EarthQuake_EQ20 set Grade='" + strGrade + "', Gal='" + strGal + "' where StnCode='" + strStnCode + "'";

            return strSQL;
        }


        #endregion

        //�ƻs�iNetDCDB
        void fnSetNetDCDBMoveData(DataTable vdtData)
        {

            int intDiff = 0;
            string strTimeRcv;
            string strNodeName;
            string strNodeTime;
            string strCPUBusy;
            string strMEM;
            string strMEMFree;
            string strProcessSum;
            string strHDC;
            string strHDCFree;
            string strHDD;
            string strHDDFree;
            string strHDE;
            string strHDEFree;
            string strHDF;
            string strHDFFree;
            string strNetWork;
            //string strDiffTime;

            string strVersion; //Hsinson950522 add for the version of ScadaTX [WNetDc]
            string strComputerName; //SFI 960724  for WNetDc
            string strComputerIP = "0.0.0.0"; //Hsinson 960830  for WNetDc
            string strOS;// SFI 950724 for WNetDc
            string strOsRuningDay;// Hsinson 960830 for WNetDc
            string strLogonUser; //Hsinson960809 for WNetDC
            string strCpuType;//Hsinson960809 for WNetDCsfi

            string strWSocketType;
            string strNetDcCheckTimer;

            // 20090508 Jacky_su add
            string strAvgDiskQueLen = ""; // (�B�I��0.0000)�����Ϻ�Que����
            string strCurDiskQueLen = ""; // (�B�I��0.0000)���e�Ϻ�Que����
            string strPercentDiskTime = ""; // (�B�I��0.0000)�ϺЦs���ɶ���v
            string strTotalTrains = ""; // (���)�`�C���ƥ� (�w�d�Ѽ�)(�t���t�����C���AEx����u��R1-R6�C���`��)
            string strActiveTrains = ""; // (���)�D�u�W�C���ƥ� (�w�d�Ѽ�)(���t���t�Ϫ��C���ơAEx����u��R2-R5�C���`��)

            // 20090626 sean add
            string strCpuName = "";		//CUP�W��
            string strCpuVoltage = "";	//CUP�q��
            string strCpuTemp = "";		//CPU�ū�

            //20110412 SFI Add
            string strTxServerName = "";	//Tx Server �W��
            string strTxServerIP = "";	//Tx Server IP

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            string strSQL_History; //Hsinson960813 add
            string strSQL_LastOne; //Hsinson960813 add
            //SqlTransaction myTrans;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]WNetDc]%'";
            //dvData.Sort="seq";

            int iRowUpdate; //Hsinson960813 add


            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strNetDCDBSQLServer_IP + ";uid=" + this.strNetDCDBSQLServer_User + ";pwd=" + this.strNetDCDBSQLServer_Password + ";database=" + this.strNetDCDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JNetDCDB  ��m(" + this.strNetDCDBSQLServer_IP + ") " + this.strNetDCDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                        //Hsinson 960813 add
                        this.OnDisplayStringMessage("�H�Χ�sNetDCDB  ��m(" + this.strNetDCDBSQLServer_IP + ") " + this.strNetDCDBSQLServer_Table_LastOne + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }

                    //					myTrans=conDB.BeginTransaction (IsolationLevel.ReadUncommitted , "SampleTransaction");
                    //					CmdDB.Transaction = myTrans;

                    DateTime DateTimeRcv;

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;

                        //Hsinson980502�ץ��y�k
                        //sltData=fnGetArrayValuebyIndex((string)dvData[i].Row["Content"]);
                        //strTimeRcv=((string))dvData[i].Row["TimeRcv"];  

                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());

                        //string	strTemp = "[WNetDc];NodeName=ID.SCA.PCB.PC4DUTY;NodeTime=2009/05/08 14:02:42;CPUBusy=77;MEM=3669484;MEMFree=3048396;ProcessSum=41;HDC=35009;HDCFree=25130;HDD=34993;HDDFree=4924;HDE=;HDEFree=;HDF=;HDFree=;AvgDiskQueLen=0.0000;CurDiskQueLen=0.0000;PercentDiskTime=0.0000;TotalTrains=0;ActiveTrains=0;NetWork=10001;Version=ScadaTx980506;WSocketType=TcpClient;NetDcCheckTimer=160;ComputerName=H0-C-620;ComputerIP=10.253.142.130;ComputerOS=WinXP;OsRuningDay=5.66;LogonUser=VXL;CpuType=586-Intel_Pentium(2 pcs);";

                        //sltData=fnGetArrayValuebyIndex(strTemp);


                        // strTimeRcv=dvData[i].Row["TimeRcv"].ToString();
                        DateTimeRcv = DateTime.Parse(dvData[i].Row["TimeRcv"].ToString());
                        strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");


                        strNodeName = ((string)sltData["NodeName"] != null) ? (string)sltData["NodeName"] : "";
                        strNodeTime = ((string)sltData["NodeTime"] != null) ? (string)sltData["NodeTime"] : "";
                        strCPUBusy = ((string)sltData["CPUBusy"] != null) ? (string)sltData["CPUBusy"] : "";
                        strMEM = ((string)sltData["MEM"] != null) ? (string)sltData["MEM"] : "";
                        strMEMFree = ((string)sltData["MEMFree"] != null) ? (string)sltData["MEMFree"] : "";
                        strProcessSum = ((string)sltData["ProcessSum"] != null) ? (string)sltData["ProcessSum"] : "";
                        strHDC = ((string)sltData["HDC"] != null) ? (string)sltData["HDC"] : "";
                        strHDCFree = ((string)sltData["HDCFree"] != null) ? (string)sltData["HDCFree"] : "";
                        strHDD = ((string)sltData["HDD"] != null) ? (string)sltData["HDD"] : "";
                        strHDDFree = ((string)sltData["HDDFree"] != null) ? (string)sltData["HDDFree"] : "";
                        strHDE = ((string)sltData["HDE"] != null) ? (string)sltData["HDE"] : "";
                        strHDEFree = ((string)sltData["HDEFree"] != null) ? (string)sltData["HDEFree"] : "";
                        strHDF = ((string)sltData["HDF"] != null) ? (string)sltData["HDF"] : "";

                        //20130503 SFI �W�[�P�_���A�P�_F�Ѫ��r����VB���� HDFree�άO NET ���� HDFFree
                        if (sltData["HDFree"] != null)
                        {
                            strHDFFree = ((string)sltData["HDFree"] != null) ? (string)sltData["HDFree"] : "";
                        }
                        else if (sltData["HDFFree"] != null)
                        {
                            strHDFFree = ((string)sltData["HDFFree"] != null) ? (string)sltData["HDFFree"] : "";
                        }
                        else
                        {
                            strHDFFree = "";
                        }


                        //strHDFFree=((string)sltData["HDFree"] != null) ? (string)sltData["HDFree"] : "" ;
                        strNetWork = ((string)sltData["NetWork"] != null) ? (string)sltData["NetWork"] : "";

                        strAvgDiskQueLen = ((string)sltData["AvgDiskQueLen"] != null) ? (string)sltData["AvgDiskQueLen"] : "";
                        strCurDiskQueLen = ((string)sltData["CurDiskQueLen"] != null) ? (string)sltData["CurDiskQueLen"] : "";
                        strPercentDiskTime = ((string)sltData["PercentDiskTime"] != null) ? (string)sltData["PercentDiskTime"] : "";
                        strTotalTrains = ((string)sltData["TotalTrains"] != null) ? (string)sltData["TotalTrains"] : "";
                        strActiveTrains = ((string)sltData["ActiveTrains"] != null) ? (string)sltData["ActiveTrains"] : "";

                        //20090626 sean add
                        strCpuName = ((string)sltData["CpuName"] != null) ? (string)sltData["CpuName"] : "";
                        strCpuVoltage = ((string)sltData["CpuVoltage"] != null) ? (string)sltData["CpuVoltage"] : "";

                        //20140220 SFI �] ScadaTx Net ���� CpuTemp ��J�� cpuTemp �A�ҥH�n�G�ئr���i�n�եΡA����ScadaTx�����w�g�b 20140122����
                        //�ץ���CpuTemp�r��

                        if (sltData["CpuTemp"] != null)
                        {
                            strCpuTemp = ((string)sltData["CpuTemp"] != null) ? (string)sltData["CpuTemp"] : "";
                        }
                        else if (sltData["cpuTemp"] != null)
                        {
                            strCpuTemp = ((string)sltData["cpuTemp"] != null) ? (string)sltData["cpuTemp"] : "";
                        }
                        else
                        {
                            strCpuTemp = "";
                        }
                        //strCpuTemp = ((string)sltData["CpuTemp"] != null) ? (string)sltData["CpuTemp"] : "" ;

                        //Edit By SFI 20110412
                        strTxServerName = ((string)sltData["TxServerName"] != null) ? (string)sltData["TxServerName"] : "";
                        strTxServerIP = ((string)sltData["TxServerIP"] != null) ? (string)sltData["TxServerIP"] : "";


                        TimeSpan tsDiff;
                        //tsDiff=DateTime.Now.Subtract (DateTime.Parse(strDiffTime));
                        tsDiff = DateTime.Parse(strTimeRcv).Subtract(DateTime.Parse(strNodeTime));

                        strVersion = (string)sltData["Version"];  //Hsinson950522 add for the version of ScadaTX [WNetDc]
                        //SFI 960724 for WNetDc
                        strWSocketType = (string)sltData["WSocketType"]; //Winsock ���� Server Or Client
                        strNetDcCheckTimer = (string)sltData["NetDcCheckTimer"]; // NetDc �����ɶ�
                        strComputerName = (string)sltData["ComputerName"]; //�q���W��
                        strComputerIP = (string)sltData["ComputerIP"]; //960830 Hsinson add �q��IP
                        strOS = (string)sltData["ComputerOS"]; // �q���W�ϥΤ��@�~�t��

                        strOsRuningDay = (string)sltData["OsRuningDay"]; //960830Hsinson�W�[�@�~�t�ζ}�����椧�Ѽ�

                        //Hsinson960809 for WNetDc
                        strLogonUser = (string)sltData["LogonUser"]; //Winsock ���� Server Or Client
                        strCpuType = (string)sltData["CpuType"]; // NetDc �����ɶ�



                        intDiff = tsDiff.Days * 86400;
                        intDiff = intDiff + tsDiff.Hours * 3600;
                        intDiff = intDiff + tsDiff.Minutes * 60;
                        intDiff = intDiff + tsDiff.Seconds;

                        //20130314 SFI �W�[�P�_N/A���r��
                        if (strCPUBusy == "N/A")
                            strCPUBusy = "-1";
                        if (strMEM == "N/A")
                            strMEM = "-1";
                        if (strMEMFree == "N/A")
                            strMEMFree = "-1";
                        if (strProcessSum == "N/A")
                            strProcessSum = "-1";
                        if (strHDC == "N/A")
                            strHDC = "-1";
                        if (strHDCFree == "N/A")
                            strHDCFree = "-1";
                        if (strHDD == "N/A")
                            strHDD = "-1";
                        if (strHDDFree == "N/A")
                            strHDDFree = "-1";
                        if (strHDE == "N/A")
                            strHDE = "-1";
                        if (strHDEFree == "N/A")
                            strHDEFree = "-1";
                        if (strHDF == "N/A")
                            strHDF = "-1";
                        if (strHDFFree == "N/A")
                            strHDFFree = "-1";
                        if (strNetWork == "N/A")
                            strNetWork = "-1";
                        if (strAvgDiskQueLen == "N/A")
                            strAvgDiskQueLen = "-1";
                        if (strCurDiskQueLen == "N/A")
                            strCurDiskQueLen = "-1";
                        if (strPercentDiskTime == "N/A")
                            strPercentDiskTime = "-1";
                        if (strCpuVoltage == "N/A")
                            strCpuVoltage = "-1";
                        if (strCpuTemp == "N/A")
                            strCpuTemp = "-1";
                        if (strOsRuningDay == "N/A")
                            strOsRuningDay = "-1";


                        //if(sltData.Count == 30)

                        // Jacky_su 980508 add
                        strSQL = " (TimeRcv,NodeName, NodeTime, CPUBusy,MEM,MEMFree,ProcessSum,HDC,HDCFree,HDD,HDDFree,HDE,HDEFree,HDF,HDFFree,NetWork,DiffTime,Version , WSocketType , NetDcCheckTimer , ComputerName ,  ComputerIP , OS, OsRuningDay, LogonUser, CpuType, AvgDiskQueLen, CurDiskQueLen, PercentDiskTime, TotalTrains, ActiveTrains,CpuName , CpuVoltage , CpuTemp , TxServerName, TxServerIP)" +
                            " values ('" + strTimeRcv + "', '" + strNodeName + "','" + strNodeTime + "','" + strCPUBusy + "','" + strMEM + "','" + strMEMFree + "','" + strProcessSum + "','" + strHDC + "','" + strHDCFree + "' " +
                            ",'" + strHDD + "','" + strHDDFree + "','" + strHDE + "','" + strHDEFree + "','" + strHDF + "','" + strHDFFree + "','" + strNetWork + "','" + intDiff + "','" + strVersion + "','" + strWSocketType + "' " +
                            ",'" + strNetDcCheckTimer + "' ,'" + strComputerName + "' ,'" + strComputerIP + "','" + strOS + "','" + strOsRuningDay + "','" + strLogonUser + "','" + strCpuType + "'" +
                            ", '" + strAvgDiskQueLen + "'" + ", '" + strCurDiskQueLen + "'" + ", '" + strPercentDiskTime + "'" + ", '" + strTotalTrains + "'" + ", '" + strActiveTrains + "', '" + strCpuName + "', '" + strCpuVoltage + "', '" + strCpuTemp + "', '" + strTxServerName + "', '" + strTxServerIP + "')";




                        if (this.blnNetDCDBEnable_History)
                        {
                            strSQL_History = "insert into " + strNetDCDBSQLServer_Table + strSQL;

                            CmdDB.CommandText = strSQL_History; //�s�J���v��Ʈw							
                            CmdDB.ExecuteNonQuery();
                        } //if (this.blnNetDCDBEnable_History)

                        if (this.blnNetDCDBEnable_LastOne)
                        {
                            //if(sltData.Count == 30)

                            strSQL_LastOne = " update " + strNetDCDBSQLServer_Table_LastOne + " set "
                                + " TimeRcv= '" + strTimeRcv + "' "
                                //+ ", NodeName="+ strNodeName
                                + ", NodeTime= '" + strNodeTime + "' "
                                + ", CPUBusy= '" + strCPUBusy + "' "
                                + ", MEM= '" + strMEM + "' "
                                + ", MEMFree= '" + strMEMFree + "' "
                                + ", ProcessSum= '" + strProcessSum + "' "
                                + ", HDC= '" + strHDC + "' "
                                + ", HDCFree= '" + strHDCFree + "' "
                                + ", HDD= '" + strHDD + "' "
                                + ", HDDFree= '" + strHDDFree + "' "
                                + ", HDE= '" + strHDE + "' "
                                + ", HDEFree= '" + strHDEFree + "' "
                                + ", HDF= '" + strHDF + "' "
                                + ", HDFFree= '" + strHDFFree + "' "
                                + ", NetWork= '" + strNetWork + "' "
                                + ", DiffTime= '" + intDiff + "' "
                                + ", Version= '" + strVersion + "' "
                                + ", WSocketType= '" + strWSocketType + "' "
                                + ", NetDcCheckTimer= '" + strNetDcCheckTimer + "' "
                                + ", ComputerName= '" + strComputerName + "' "
                                + ", ComputerIP= '" + strComputerIP + "' "    //Hsinson960830 add
                                + ", OS= '" + strOS + "' "
                                + ", OsRuningDay= '" + strOsRuningDay + "' "   //Hsinson960830 add
                                + ", LogonUser= '" + strLogonUser + "' "
                                + ", CpuType= '" + strCpuType + "' "
                                + ", AvgDiskQueLen= '" + strAvgDiskQueLen + "' "
                                + ", CurDiskQueLen= '" + strCurDiskQueLen + "' "
                                + ", PercentDiskTime= '" + strPercentDiskTime + "' "
                                + ", TotalTrains= '" + strTotalTrains + "' "
                                + ", ActiveTrains= '" + strActiveTrains + "' "
                                + ", CpuName= '" + strCpuName + "' "
                                + ", CpuVoltage= '" + strCpuVoltage + "' "
                                + ", CpuTemp= '" + strCpuTemp + "' "
                                + ", TxServerName= '" + strTxServerName + "' "		//Add by SFI 20110412 
                                + ", TxServerIP= '" + strTxServerIP + "' "			//Add by SFI 20110412
                                + " where ( NodeName = '" + strNodeName + "') ";


                            CmdDB.CommandText = strSQL_LastOne;	//��s�ܧY�ɸ�Ʈw
                            iRowUpdate = CmdDB.ExecuteNonQuery();

                            if (iRowUpdate <= 0)  //�Y�S�����A�h�ηs�W�覡�N��ƶ�JLastOne��ƪ�
                            {
                                strSQL_LastOne = "insert into " + strNetDCDBSQLServer_Table_LastOne + strSQL;
                                CmdDB.CommandText = strSQL_LastOne;	//��s�ܧY�ɸ�Ʈw
                                iRowUpdate = CmdDB.ExecuteNonQuery();

                                strSQL_LastOne = " update " + strNetDCDBSQLServer_Table_LastOne + " set RegTime =  '" + string.Format("{0:yyyy/mm/dd hh:mm:ss}", DateTime.Now) + "' where ( NodeName = '" + strNodeName + "') ";
                                CmdDB.CommandText = strSQL_LastOne;
                                iRowUpdate = CmdDB.ExecuteNonQuery();
                            }//if
                        }//if (this.blnNetDCDBEnable_LastOne)

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JNetDCDB  ��m(" + this.strNetDCDBSQLServer_IP + ") " + strNetDCDBSQLServer_Table + "�P"
                        + this.strNetDCDBSQLServer_Table_LastOne + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JNetDCDB  ��m(" + this.strNetDCDBSQLServer_IP + ") " + this.strNetDCDBSQLServer_Table + "�P"
                    + this.strNetDCDBSQLServer_Table_LastOne + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        } //------------------------------------------------------------------------------------fnSetWinNetDcDBMoveData

        //�ƻs�iEQBIAS��ƪ� -> EQBIAS �ק�
        void fnSetBiasDBMoveData(DataTable vdtData)
        {

            //SAMMY20200406 ADD EQBIAS ��ƪ�
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]EQBIAS]%'";
            //dvData.Sort="seq";

            int iRowUpdate; //Hsinson960813 add

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strPmsDBSQLServer_IP + ";uid=" + this.strPmsDBSQLServer_User + ";pwd=" + this.strPmsDBSQLServer_Password + ";database=" + this.strPmsDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JEQ BIAS ��m(" + this.strPmsDBSQLServer_IP + ") RTDB_SCADA.[dbo].[EQBIAS] ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }

 



                    for (int i = 0; i < dvData.Count; i++)
                    {

                        string wEQName = "";
                        string wEQId = "";
                        string wEQType = "";    //1 �W�@�����A��(2�����e��T) , 2 �������ʭ�(���U)
                        string wEQTime = "";    //EQTime = 2020/01/01 12:00:00
                        string wEQBiasX = "";   //BISA X = "0.0000"
                        string wEQBiasY = "";   //BISA Y = "0.0000"
                        string wEQBiasZ = "";   //BISA Z = "0.0000"
                        //sContent = "[EQBIAS];EQNAME=" + StnName;EQID=" + stnNum;EQTYPE" + sType;EQTIME=" + sTime + ;BIASX=" + BiasX ;BIASY=" + BiasY ;BIASZ=" + BiasZ

                        //	Hsinson 980502 �ק��ন�r�ꤧ�y�k
                        //						sltData=fnGetArrayValuebyIndex((string)dvData[i].Row["Content"]);
                        //						strTimeRcv=(string)dvData[i].Row["TimeRcv"];
                        SortedList sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        wEQName = (string)sltData["EQNAME"];
                        wEQId = (string)sltData["EQID"];
                        wEQType = (string)sltData["EQTYPE"];
                        wEQTime = (string)sltData["EQTIME"];
                        wEQBiasX = (string)sltData["BIASX"];
                        wEQBiasY = (string)sltData["BIASY"];
                        wEQBiasZ = (string)sltData["BIASZ"];

                        strSQL = "insert into EQBiasData(EQName, EQId, EQType, EQDatetTime, EQBiasX, EQBiasY, EQBiasZ)" +
                                 " values('" + wEQName + "', '" + wEQId + "','" + wEQType + "','" + wEQTime + "','" + wEQBiasX + "','" + wEQBiasY + "','" + wEQBiasZ + "')";

                        CmdDB.CommandText = strSQL;	//��s�ܧY�ɸ�Ʈw
                        CmdDB.ExecuteNonQuery();

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JEQBIAS  ��m(" + this.strPmsDBSQLServer_IP + ") RTDB_SCADA.[dbo].[EQBIAS] ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JEQBIAS ��m(" + this.strPmsDBSQLServer_IP + ") RTDB_SCADA.[dbo].[EQBIAS] ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }





        }

        //�ƻs�iPmsDB Process Monitor
        void fnSetPmsDBMoveData(DataTable vdtData)
        {

            int intDiff = 0;
            string strTimeRcv;
            string strNodeName;
            string strNodeTime;
            string strAlias;
            string strMsg;
            string strPathName;
            string strShortName;
            string strPermitDate;  //���\����� -971030 Hsinson add
            string strModifyDate;  //���e�����
            string strPermitSize;  //���\���j�p
            string strFileSize;	   //���e���j�p
            string strExecuting;   //���\�����檬�A
            string strCurExeState; //���e�����檬�A

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            string strSQL_History; //Hsinson960813 add
            string strSQL_LastOne; //Hsinson960813 add
            //SqlTransaction myTrans;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]PMS]%'";
            //dvData.Sort="seq";

            int iRowUpdate; //Hsinson960813 add


            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strPmsDBSQLServer_IP + ";uid=" + this.strPmsDBSQLServer_User + ";pwd=" + this.strPmsDBSQLServer_Password + ";database=" + this.strPmsDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JPmsDB  ��m(" + this.strPmsDBSQLServer_IP + ") " + this.strPmsDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                        //Hsinson 960813 add
                        this.OnDisplayStringMessage("�H�Χ�sPmsDB  ��m(" + this.strPmsDBSQLServer_IP + ") " + this.strPmsDBSQLServer_Table_LastOne + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    }

                    //					myTrans=conDB.BeginTransaction (IsolationLevel.ReadUncommitted , "SampleTransaction");
                    //					CmdDB.Transaction = myTrans;

                    DateTime DateTimeRcv;

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;


                        //	Hsinson 980502 �ק��ন�r�ꤧ�y�k
                        //						sltData=fnGetArrayValuebyIndex((string)dvData[i].Row["Content"]);
                        //						strTimeRcv=(string)dvData[i].Row["TimeRcv"];
                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());


                        // strTimeRcv=dvData[i].Row["TimeRcv"].ToString();
                        DateTimeRcv = System.DateTime.Parse(dvData[i].Row["TimeRcv"].ToString());
                        strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");


                        strNodeName = (string)sltData["NodeName"];
                        strNodeTime = (string)sltData["NodeTime"];
                        strAlias = (string)sltData["Alias"];
                        strMsg = (string)sltData["Msg"];
                        strPathName = (string)sltData["PathName"];
                        strShortName = (string)sltData["ShortName"];
                        strPermitDate = (string)sltData["PermitDate"];
                        strModifyDate = (string)sltData["ModifyDate"];
                        strPermitSize = (string)sltData["PermitSize"];
                        strFileSize = (string)sltData["Size"];
                        strExecuting = (string)sltData["Executing"];
                        strCurExeState = (string)sltData["CurExecState"];

                        strSQL = " (TimeRcv,NodeName, NodeTime, Alias, Msg, PathName, ShortName, PermitDate ,ModifyDate, PermitSize ,FileSize, Executing, CurExeState)" +
                            " values ('" + strTimeRcv + "', '" + strNodeName + "','" + strNodeTime + "','" + strAlias + "','" + strMsg + "','" + strPathName + "','" + strShortName + "', " +
                            " '" + strPermitDate + "','" + strModifyDate + "','" + strPermitSize + "','" + strFileSize + "','" + strExecuting + "','" + strCurExeState + "')";

                        if (this.blnPmsDBEnable_History)
                        {
                            strSQL_History = "insert into " + strPmsDBSQLServer_Table + strSQL;

                            CmdDB.CommandText = strSQL_History; //�s�J���v��Ʈw							
                            CmdDB.ExecuteNonQuery();
                        } //if (this.blnPmsDBEnable_History)

                        if (this.blnPmsDBEnable_LastOne)
                        {

                            strSQL_LastOne = " update " + strPmsDBSQLServer_Table_LastOne + " set "
                                + " TimeRcv= '" + strTimeRcv + "' "
                                //+ ", NodeName="+ strNodeName
                                + ", NodeTime= '" + strNodeTime + "' "
                                + ", Alias= '" + strAlias + "' "
                                + ", Msg= '" + strMsg + "' "
                                + ", PathName= '" + strPathName + "' "
                                + ", ShortName= '" + strShortName + "' "
                                + ", PermitDate= '" + strPermitDate + "' "
                                + ", ModifyDate= '" + strModifyDate + "' "
                                + ", PermitSize= '" + strPermitSize + "' "
                                + ", FileSize= '" + strFileSize + "' "
                                + ", Executing= '" + strExecuting + "' "
                                + ", CurExeState= '" + strCurExeState + "' "
                                + " where ( NodeName = '" + strNodeName + "' and Alias='" + strAlias + "') ";
                            CmdDB.CommandText = strSQL_LastOne;	//��s�ܧY�ɸ�Ʈw
                            iRowUpdate = CmdDB.ExecuteNonQuery();
                            if (iRowUpdate <= 0)  //�Y�S�����A�h�ηs�W�覡�N��ƶ�JLastOne��ƪ�
                            {
                                strSQL_LastOne = "insert into " + strPmsDBSQLServer_Table_LastOne + strSQL;
                                CmdDB.CommandText = strSQL_LastOne;	//��s�ܧY�ɸ�Ʈw
                                iRowUpdate = CmdDB.ExecuteNonQuery();
                            }//if
                        }//if (this.blnPmsDBEnable_LastOne)

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JPmsDB  ��m(" + this.strPmsDBSQLServer_IP + ") " + strPmsDBSQLServer_Table + "�P"
                        + this.strPmsDBSQLServer_Table_LastOne + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JPmsDB  ��m(" + this.strPmsDBSQLServer_IP + ") " + this.strPmsDBSQLServer_Table + "�P"
                    + this.strPmsDBSQLServer_Table_LastOne + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        } //------------------------------------------------------------------------------------fnSetPmsDBMoveData

        string fnGetDayStr(string strTime) //961018 Hsinson add�ˬd����榡�O�_�ŦX�褸�~/��/��(yyyy/mm/dd)�榡�A�Y�O�h�Ǧ^ �褸�~���(yyyymmdd) �զ����r��
        //return yyyymmdd or null
        {
            int year = Int32.Parse(strTime.Substring(0, 4));
            int month = Int32.Parse(strTime.Substring(5, 2));
            int day = Int32.Parse(strTime.Substring(8, 2));
            //Hsinson961121�W�[�ˬd [��] �椧���r�������ťզr��
            //if (year >2000 && year<3000  &&  month >0 && month<13 && day>0 && day<32 && strTime.Substring(4,1)=="/" && strTime.Substring(7,1)=="/")
            if (year > 2000 && year < 3000 && month > 0 && month < 13 && day > 0 && day < 32 && strTime.Substring(4, 1) == "/" && strTime.Substring(7, 1) == "/" && strTime.Substring(8, 1) != " ")
            {
                //return yyyymmdd
                return strTime.Substring(0, 4) + strTime.Substring(5, 2) + strTime.Substring(8, 2);
            }
            else
                //return null;
                return DateTime.Today.ToString("yyyyMMdd"); //Hsinson961121��^�Ƿ��Ѥ��

        }//--------------------------------------------------------------------------------fnGetDayStr

        bool fnIsDateTimeStr(string strTime) //961018 Hsinson add�ˬd����榡�O�_�ŦX�褸�~/��/��(yyyy/mm/dd)�榡�A�Y�O�h�Ǧ^ �褸�~���(yyyymmdd) �զ����r��
        //return yyyymmdd or null
        {
            try
            {
                int year = Int32.Parse(strTime.Substring(0, 4));
                int month = Int32.Parse(strTime.Substring(5, 2));
                int day = Int32.Parse(strTime.Substring(8, 2));
                int hour = Int32.Parse(strTime.Substring(11, 2));
                int minute = Int32.Parse(strTime.Substring(14, 2));
                int second = Int32.Parse(strTime.Substring(17, 2));
                //Hsinson961121�W�[�ˬd [��] �椧���r�������ťզr��
                //if (year >2000 && year<3000  &&  month >0 && month<13 && day>0 && day<32 && strTime.Substring(4,1)=="/" && strTime.Substring(7,1)=="/")
                if (year > 2000 && year < 3000 && month > 0 && month < 13 && day > 0 && day < 32 && strTime.Substring(4, 1) == "/" && strTime.Substring(7, 1) == "/" && strTime.Substring(8, 1) != " "
                    && strTime.Substring(10, 1) == " "
                    && hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60 && strTime.Substring(13, 1) == ":" && strTime.Substring(16, 1) == ":"
                    && strTime.Substring(14, 1) != " " && strTime.Substring(17, 1) != " "
                    )
                {
                    return true;
                }
                else
                    //return null;
                    return false;
            }
            catch (Exception Ex)
            {
                return false;
            }

        }//--------------------------------------------------------------------------------fnGetDayStr

        string fnFixedDateTimeStr(string strTime) //961018 Hsinson add�ˬd����榡�O�_�ŦX�褸�~/��/��(yyyy/mm/dd)�榡�A�Y�O�h�Ǧ^ �褸�~���(yyyymmdd) �զ����r��
        //return yyyymmdd or null
        {
            int year = Int32.Parse(strTime.Substring(0, 4));
            int month = Int32.Parse(strTime.Substring(5, 2));
            int day = Int32.Parse(strTime.Substring(8, 2));
            int hour = Int32.Parse(strTime.Substring(11, 2));
            int minute = Int32.Parse(strTime.Substring(14, 2));
            int second = Int32.Parse(strTime.Substring(17, 2));
            //Hsinson961121�W�[�ˬd [��] �椧���r�������ťզr��
            //if (year >2000 && year<3000  &&  month >0 && month<13 && day>0 && day<32 && strTime.Substring(4,1)=="/" && strTime.Substring(7,1)=="/")
            if (year > 2000 && year < 3000 && month > 0 && month < 13 && day > 0 && day < 32 && strTime.Substring(4, 1) == "/" && strTime.Substring(7, 1) == "/" && strTime.Substring(8, 1) != " "
                && strTime.Substring(10, 1) == " "
                && hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60 && strTime.Substring(13, 1) == ":" && strTime.Substring(16, 1) == ":"
                && strTime.Substring(14, 1) != " " && strTime.Substring(17, 1) != " "
                )
            {
                return strTime;
            }
            else
                //return null;
                return DateTime.Now.ToString("yyyy/MM/dd mm:hh:ss"); //Hsinson961121��^�Ƿ��Ѥ��

        }//--------------------------------------------------------------------------------fnGetDayStr

        //�ƻs�iAtsEventDB
        void fnSetAtsEventDBMoveData(DataTable vdtData)
        {

            string strTimeRcv;
            string strNode;
            string strTime;
            string strUser;
            string strArea;			//�������X
            string strObject;
            string strObject_Type = ""; //Hsinson961121 add, �]�ƫ���
            string strObject_DevNo = ""; //Hsinson961121 add, �]�ƽs��
            string strEvent;
            string strSeverity;
            string strShortMessage;
            string strMessage;

            //Hsinson970219 add TWC 
            string strLoc = "";
            string strTrack = "";
            string strTrain = ""; //Hsinson961128 add, �C�����X
            string strBerth = "";
            string strReady = "";
            string strDoorClose = "";
            string strMotion = "";
            string strCrew = "";
            string strDest = "";


            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn = "";
            string strSQL = "";
            string strSQL_History = "";
            //20130530 ���� MARK
            SqlTransaction myTrans;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]AtsEvent]%'";
            //dvData.Sort="seq";

            int iRowUpdate; //Hsinson970219 add

            string strDebug = "";
            string strtime_LastEvent = "";//Hsinson970426�O���W�@���ƥ󤧮ɶ�

            int idxUser;

            string strAreaTmp;  //Hsinson20120715
            int idxArea = -1;     //Hsinson20120715
            int idxAreaEnd = -1;  //Hsinson20120715



            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strAtsEventDBSQLServer_IP + ";uid=" + this.strAtsEventDBSQLServer_User + ";pwd=" + this.strAtsEventDBSQLServer_Password + ";database=" + this.strAtsEventDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JAtsEventDB  ��m(" + this.strAtsEventDBSQLServer_IP + ") " + this.strAtsEventDBSQLServer_Table + "yyyymmdd ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                    }

                    //20130530 SFI ��������
                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;

                    for (int i = 0; i < dvData.Count; i++)
                    {
                        try
                        {
                            int idxTrain;
                            strDebug = "0";

                            strNode = "";
                            strTime = "";
                            strUser = "";
                            strArea = "";
                            strObject = "";
                            strObject_Type = ""; //Hsinson961121 add, �]�ƫ���
                            strObject_DevNo = ""; //Hsinson961121 add, �]�ƽs��
                            strEvent = "";
                            strSeverity = "";
                            strShortMessage = "";
                            strMessage = "";

                            //Hsinson970219 add TWC 
                            strLoc = "";
                            strTrack = "";
                            strTrain = ""; //Hsinson961128 add, �C�����X
                            strBerth = "";
                            strReady = "";
                            strDoorClose = "";
                            strMotion = "";
                            strCrew = "";
                            strDest = "";


                            SortedList sltData;
                            sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());

                            strSQL_History = dvData[i].Row["Content"].ToString();  //Hsinson970426 add for Debug
                            strTimeRcv = dvData[i].Row["TimeRcv"].ToString();

                            //�ܼƫŧi
                            //		string strTimeRcv;
                            //		string strNode;
                            //		string strTime;
                            //		string strArea;
                            //		string strObject;
                            //		string strEvent;
                            //		string strSeverity;
                            //		string strShortMessage;
                            //		string strMessage;
                            //string TimeRcv;
                            strNode = (string)sltData["Node"];
                            if (strNode == null) continue;

                            strTime = (string)sltData["Time"];
                            if (strTime == null) continue;

                            strDebug = "1";

                            if (fnIsDateTimeStr(strTime) == false)
                            {
                                strDebug = "2";
                                if (strtime_LastEvent.Length > 0)
                                {
                                    strDebug = "2-1";
                                    strTime = strtime_LastEvent;
                                }
                                else
                                {
                                    strDebug = "2-2";
                                    strTime = strTimeRcv;
                                }

                            }
                            else
                            {
                                strtime_LastEvent = strTime;
                            }
                            strTime = strTime.Substring(0, MIN(19, strTime.Length));  //Hsinson970426 Debug

                            strDebug = "3";

                            if (((string)sltData["User"]) != null)
                            {
                                strUser = ((string)sltData["User"]).ToUpper();
                            }

                            strDebug = "4";

                            idxUser = strUser.IndexOf("REFLEX");  //Hsinson970217: Alstom�s�WTWC�T����[TRN: ]�r���3�X (�`�N�A��TRN: �]���[�@�Ӫť�)
                            if (idxUser >= 0)  //Hsinson970412�NREFLEX CONTROL�令0�N��MATCH CONTROL
                            {
                                strUser = "0";
                            }


                            strArea = ((string)sltData["Area"]);
                            if (strArea == "No Group name") strArea = "";//Hsinson961121 �R�����w�W��(No Group name)�������r��

                            strObject = ((string)sltData["Object"]);

                            strDebug = "5";
                            if (strObject != null)
                            {
                                string[] split = strObject.Split(new Char[] { '_' });
                                strObject_Type = "";
                                strObject_DevNo = "";
                                if (split[0].Length < 50 && split.GetUpperBound(0) >= 1)  //���A�r����פp��50�A�B�i����2��(���@�Ӥ��j�r��)�H�W�~�}�l��Ķ����
                                {
                                    //�d��1�G[AtsEvent];...;Area=BL04;Object=RT_BL04_6_8;Event=ICONIS;Severity=0;User=;Message=RTST _ROUTE SET
                                    //�d��2�G[AtsEvent];...;Area=No Group name;Object=TCL_BL13_U1062.TWCControlsModule.TWCControlsMultiUserAlarmEventFilter.EventFilte;Event=ICONIS;Severity=125;User=;Message=PLTR _PERFORMANCE LEVEL 1  SENT TO TRAIN 285  AT BL13_UP 
                                    //�d��2�G[AtsEvent];...;Area=BL06;Object=Area_G13_UP;Event=Audibility;Severity=375;User=;Message=alarm becomes audible 
                                    strDebug = "6";

                                    if (split[0].ToLower() == "area")
                                    {
                                        strObject_Type = split[1];
                                    }
                                    else
                                    {
                                        strObject_Type = split[0];//Hsinson961121 �]�ƫ���
                                    }

                                    if (strArea.Length < 1) strArea = split[1]; //Hsinson970412 �����W��
                                    strDebug = "7";
                                    if (split.GetUpperBound(0) >= 2)  //���A�r��i����3��(���G�Ӥ��j�r��)�H�W�h�}�l�զX��Ķ����
                                    {
                                        strDebug = "8";

                                        for (int j = 2; j <= split.GetUpperBound(0); j++)  //Hsinson961121 �]�ƽs��
                                        {
                                            strDebug = "9";

                                            if (split[j] != "")
                                            {
                                                if (j == 2) strObject_DevNo = split[j];
                                                else
                                                {
                                                    strObject_DevNo += ("_" + split[j]);
                                                    strDebug = "10";

                                                }//else
                                            }//if
                                        }//for

                                        //Hsinson 20110222 �w��d��2 add: Object=TCL_BL13_U1062.TWCControlsModule.TWCControlsMultiUserAlarmEventFilter.EventFilte
                                        //                           ��'_'���}�ᤧ�ĤT�ϬqU1062.TWCControlsModule.TWCControlsMultiUserAlarmEventFilter.EventFilte
                                        //                           �r�ꤴ�Ӫ��A�d�U '.TWC'�çR�����᪺�r��
                                        strDebug = "11";
                                        int idxObject;
                                        idxObject = strObject_DevNo.IndexOf(".");  //�L�o����'.'���r��A�Ҧp".TWC....."
                                        if (idxObject >= 0)
                                        {
                                            strObject_DevNo = strObject_DevNo.Substring(0, idxObject + 4); //���'.'��@�|�X�A�A�Ҧp".TWC"
                                        }
                                    }//if ( split.GetUpperBound(0)>=2)  //���A�r��i����3��(���G�Ӥ��j�r��)�H�W�h�}�l�զX��Ķ����
                                }//if (split[0].Length<50 && split.GetUpperBound(0)>=1)  //���A�r����פp��50�A�B�i����2��(���@�Ӥ��j�r��)�H�W�~�}�l��Ķ����

                                //Hsinson 20120715, 20120716 begin-------------------------
                                //else if (strObject == "ARS") //Hsinson 20120716 modify
                                if ((strObject == "ARS") && (strArea.Length < 1))
                                {
                                    //�d��3.1�G[AtsEvent];...;Area=No Group name;Object=ARS;...;Message=Alarm state is Inactive - OffToOn unack  : Label is IDRC _ROUTE DESIGNATE CONFLICT FOR RT_G03_8_12 
                                    //�d��3.2�G[AtsEvent];...;Area=No Group name;Object=ARS;...;Message=state is Inactive - OffToOn unack  : Label is PDCM _TRAIN 171 ON TRACK TCL_G04_GCS915: DISPATCH PROHIBITED BY DESTINATION CODE MISMATCH
                                    //       Hsinson 20120715���O�G�w��Object=ARS
                                    //                             �]�L�k��'_'���}�A�䤤 Message=...IDRC _ROUTE DESIGNATE CONFLICT FOR RT_G03_8_12 
                                    //                                                 ��Message=...PDCM _TRAIN 171 ON TRACK TCL_G04_GCS915: DISPATCH PROHIBITED BY DESTINATION CODE MISMATCH
                                    //                             �]Message���t��RT_G03_8_12 �i��'_'���}
                                    //                             �G�i��Message�i��strArea���B�z
                                    if (strMessage.Length > 0)
                                    {

                                        strAreaTmp = "";
                                        idxArea = strMessage.IndexOf("RT_");                 //Area�T����[RT_]�r��_��3�ӥ�'_'�Ÿ����X�Ӫ��϶�
                                        if (idxArea < 0) idxArea = strMessage.IndexOf("TCL_");  //        ��[TCL_]�r��_��3�ӥ�'_'�Ÿ����X�Ӫ��϶�

                                        if (idxArea >= 0) //�����Area�r��
                                        {
                                            strDebug = "1101";
                                            idxAreaEnd = strMessage.IndexOf(' ', idxArea);
                                            if (idxAreaEnd <= idxArea) strAreaTmp = strMessage.Substring(idxArea);
                                            else strAreaTmp = strMessage.Substring(idxArea, idxAreaEnd - idxArea);

                                            string[] splitArea = strAreaTmp.Split(new Char[] { '_' });
                                            if (splitArea.GetUpperBound(0) >= 1)
                                            {
                                                strObject_Type = splitArea[0];//1.���o�]�ƫ���
                                                strDebug = "1102";

                                                if (strArea.Length < 1) strArea = splitArea[1]; //2.���o�����W��

                                                if (split.GetUpperBound(0) >= 2)  //���A�r��i����3��(���G�Ӥ��j�r��)�H�W�h�}�l�զX��Ķ����
                                                {
                                                    strDebug = "1103";
                                                    for (int j = 2; j <= split.GetUpperBound(0); j++)  //3.���o�]�ƽs��
                                                    {
                                                        if (split[j] != "")
                                                        {
                                                            if (j == 2) strObject_DevNo = split[j];
                                                            else
                                                            {
                                                                strDebug = "1104";
                                                                strObject_DevNo += ("_" + split[j]);

                                                            }//else
                                                        }//if
                                                    }//for
                                                    //Hsinson 20110222 �w��d��3.2  Meaasge=...TCL_G04_GCS915: DISPATCH PROHIBITED BY DESTINATION CODE MISMATCH
                                                    //                           ��'_'���}�ᤧ�ĤT�Ϭq[TCL_G04_GCS915:]�N�h�X�Ӫ��_���R��
                                                    strDebug = "1105";
                                                    int idxObject;
                                                    idxObject = strObject_DevNo.IndexOf(":");  //�L�o����':'���r��A�Ҧp"GCS915:"
                                                    if (idxObject >= 0)
                                                    {
                                                        strDebug = "1106";
                                                        strObject_DevNo = strObject_DevNo.Substring(0, idxObject); //��ܨ�':'�e�@�X�A�Ҧp"GCS915"
                                                    }

                                                }//if ( split.GetUpperBound(0)>=2)  //���A�r��i����3��(���G�Ӥ��j�r��)�H�W�h�}�l�զX��Ķ����
                                            }//split.GetUpperBound(0)>=1) 

                                        }//if idxArea>=0
                                    }//if strMessage.length>0

                                }//if ((strObject == "ARS") && (strArea.Length<1))
                                //Hsinson 20120715,20120716 end--------------------------


                                //�d��4�G[AtsEvent];...;Area=No Group name;Object=Train10.TrainDelayMultiUserMonitoredAlarm.AlarmEventFilter;Event=ICONIS;Severity=0;User=REFLEX CONTROL;Message=TOTP _TRAIN 224  (TRIP 2081 ) IS In Time 
                                //       Hsinson 20110222���O�G�w��Object=Train10.TrainDelayMultiUserMonitoredAlarm.AlarmEventFilter
                                //                             �L�k��'_'���}�A�B�r�ꤴ�Ӫ��A�]�����i�@�B�B�z

                            }//if strObject !=null
                            else
                            {
                                strObject = "";

                            }

                            strEvent = ((string)sltData["Event"]);
                            if (strEvent == null) strEvent = "";

                            strSeverity = (string)sltData["Severity"];
                            if (strSeverity == null) strSeverity = "0";

                            strMessage = (string)sltData["Message"];
                            strDebug = "11";

                            if (strMessage == null)
                            {
                                strDebug = "1101";
                                strMessage = "";
                            }

                            if (strMessage.Length > 0)
                            {
                                strDebug = "12";

                                //Hsinson 961128, add Train Number�C�����X�A
                                //    Ats Event�̡A�C�����X�� [TRAIN ]�r���3�X (�`�N�A��TRAIN�[�@�Ӫť�)
                                strTrain = "";
                                idxTrain = strMessage.IndexOf("TRN:");  //Hsinson970217: Alstom�s�WTWC�T����[TRN: ]�r���3�X (�`�N�A��TRN: �]���[�@�Ӫť�)
                                if (idxTrain >= 0)
                                {
                                    strDebug = "13";

                                    //Example:
                                    //    TWC (Track: TCL_O19_U921 TRN: 000): Train Ready On 
                                    //    TWC (Track: TCL_R22_U1073 TRN: 111): Train Berthed Off 

                                    strTrain = strMessage.Substring(idxTrain + 5, MIN(3, strMessage.Length - idxTrain - 5));

                                    //Hsinson970219 add TWC 
                                    strLoc = strArea;
                                    strTrack = strObject_DevNo;

                                }
                                else
                                {
                                    strDebug = "14";

                                    //Hsinson 20120716 add begin-------------------------
                                    //��'_NO TRAIN AVAILABLE AT" �קK�~�� strTrain = "AVA"�A�ñNstrArea�PstrObject_DevNo�Ѷ}
                                    //�d�ҡGMessage=...TNAD _NO TRAIN AVAILABLE AT G01A_GS FOR NEXT DEPARTURE 
                                    //                      012345678901234567890123456789
                                    idxTrain = strMessage.IndexOf("_NO TRAIN AVAILABLE AT");
                                    if (idxTrain >= 0)
                                    {
                                        //�d�ҡGMessage=...TNAD _NO TRAIN AVAILABLE AT G01A_GS FOR NEXT DEPARTURE 
                                        //                                         01234567890
                                        strAreaTmp = "";
                                        idxArea = strMessage.IndexOf(" AT ");
                                        if (idxArea > 0)
                                        {
                                            idxArea = idxArea + 4; //�k��4�Ӧr��
                                            idxAreaEnd = strMessage.IndexOf(' ', idxArea);
                                            if (idxAreaEnd <= idxArea) strAreaTmp = strMessage.Substring(idxArea);

                                            strAreaTmp = strMessage.Substring(idxArea, idxAreaEnd - idxArea);

                                            string[] splitArea = strAreaTmp.Split(new Char[] { '_' });
                                            if (splitArea.GetUpperBound(0) >= 1)
                                            {
                                                strDebug = "1401";
                                                //�d�ҡG G01A_GS, R33_UP,  R26X_DN ...
                                                if (strArea.Length < 1) strArea = splitArea[0];                //1.���o�����W��
                                                if (strObject_DevNo.Length < 1) strObject_DevNo = splitArea[1];//2.���o�]�ƽs��

                                            }//if ( split.GetUpperBound(0)>=2)  //���A�r��i����2��(���G�Ӥ��j�r��)�H�W�h�}�l�զX��Ķ����
                                        }//if idxArea>0
                                    } //if (idxTrain>=0) //IndexOf("_NO TRAIN AVAILABLE AT")
                                    else
                                    {
                                        //Hsinson 20120716 end-------------------------

                                        //Example:
                                        //    OCCT OCCUPIED BY TRAIN 134 
                                        //    UNOT RELEASED BY TRAIN 111 
                                        idxTrain = strMessage.IndexOf("TRAIN");
                                        if (idxTrain >= 0)
                                        {
                                            strDebug = "15";

                                            //strTrain = strMessage.Substring(idxTrain+6,3);
                                            if (strMessage.Length - idxTrain - 6 > 0)
                                            {
                                                strDebug = "16";

                                                strTrain = strMessage.Substring(idxTrain + 6, MIN(3, strMessage.Length - idxTrain - 6));

                                            }//Hsinson970426 debug
                                        }//if (idxTrain) //IndexOf("TRAIN")
                                    }//else
                                }//else  //IndexOf("TRN:")

                                strDebug = "17";
                                if (strMessage.IndexOf(' ') >= 0)
                                {
                                    strShortMessage = strMessage.Substring(0, strMessage.IndexOf(' '));
                                }

                                strDebug = "18";
                                strMessage = strMessage.Remove(0, strMessage.IndexOf(' ') + 1);//Hsinson961121 �����]�ƥN�X


                                //Hsinson970412��ĶTERRITORY
                                if (strUser.Length < 1)
                                {
                                    if (strObject_Type == "Territory")
                                    {
                                        strDebug = "19";

                                        if (strMessage.IndexOf("is assign to") >= 0)
                                        {
                                            strDebug = "20";

                                            //is assign to tc4 
                                            //01234567890123
                                            strUser = strMessage.Substring(13, 3).ToUpper();
                                        }
                                        else if (strMessage.IndexOf("is no more assign to") >= 0)
                                        //is no more assign to tc4 
                                        //0123456789012345678901
                                        {
                                            strDebug = "21";

                                            strUser = strMessage.Substring(21, 3).ToUpper();
                                        }
                                    }

                                    //Hsinson20120715 begin �W�[�^���ϥΪ̦W��
                                    //�d��5.1�G[AtsEvent];...;UserName=;Object=IconisS2K.OPCServerClientMonitor";...;Message=client operator "occ" logged off from server 1
                                    //�d��5.2�G[AtsEvent];...;UserName=;Object=IconisS2K.OPCServerClientMonitor";...;Message=client no longer connected with name "occ" to server 1
                                    //�d��5.3�G[AtsEvent];...;UserName=;Object=IconisS2K.OPCServerClientMonitor";...;Message=client connected with name "WN-RD-LC" to server 1. 
                                    else if (strObject == "IconisS2K.OPCServerClientMonitor")
                                    {
                                        strDebug = "2101";
                                        idxUser = -1;
                                        idxUser = strMessage.IndexOf("operator");
                                        if (idxUser >= 0)
                                        {
                                            strDebug = "2102";

                                            //�d��5.1�Gclient operator "occ" logged off from server 1. Client name 
                                            //                01234567890123
                                            strUser = strMessage.Substring(idxUser + 10).ToUpper();//1.2 �N��10�줸�ᤧ�r������w��strUser
                                        }//if (idxUser>=0)
                                        else
                                        {
                                            idxUser = strMessage.IndexOf("name");
                                            if (idxUser >= 0)
                                            {
                                                strDebug = "2103";

                                                //�d��5.2�Gclient no longer connected with name "occ" to server 1. 
                                                //                                         01234567890123
                                                //�d��5.3�Gclient connected with name "WN-RD-LC" to server 1. 
                                                //                               01234567890123
                                                int iTemp = (strMessage.Length - idxUser - 4) == 0 ? (idxUser + 4) : (idxUser + 6);
                                                //strUser=strMessage.Substring(idxUser+6).ToUpper(); //1.2 �N��6�줸�ᤧ�r������w��strUser
                                                strUser = strMessage.Substring(iTemp).ToUpper(); //1.2 �N��6�줸�ᤧ�r������w��strUser
                                            }//if (idxUser>=0)
                                        }//else

                                        if (strUser.Length > 0) //2.�A�NstrUser�r���["]���ΡA�B���ܤ޸�["]����
                                        {
                                            string[] split = strUser.Split(new Char[] { '"', ' ' });
                                            strDebug = "2104";
                                            strUser = split[0];
                                        }
                                        else
                                            strUser = "";

                                    }//else if (strObject == "IconisS2K.OPCServerClientMonitor" )

                                    //Hsinson20120715 end

                                }

                            }//if strMessage.length>0 //Hsinson970426 debug
                            else
                            {
                                strDebug = "22";
                                strShortMessage = "";
                            }

                            //961018 Hsinson �ˬd�ƥ����O�_��s�A�Y�O�h��s���ͷs����ƪ�
                            string sdate = fnGetDayStr(strTime);
                            if (sdate == null)
                            {
                                strDebug = "23";
                                AtsEventErrorCount++;
                                this.OnDisplayStringMessage("AtsEvent Format error, Error Count=" + AtsEventErrorCount);
                                textBox_AtsEventErrorCount.Text = AtsEventErrorCount.ToString();
                                textBox_LastError.Text = strTimeRcv + " - " + dvData[i].Row["Content"].ToString();
                            }
                            else if (strAtsEventDBSQLServer_Table_ExtName != sdate)
                            {
                                strDebug = "24";

                                strAtsEventDBSQLServer_Table_ExtName = sdate;
                                this.OnDisplayStringMessage("�ƥ����P���e��ƪ�������P�A�۰ʫإ�(Create)�s��ƪ�" + strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName);
                                strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                                strSQL = strSQL + strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                                strSQL = strSQL + " CREATE TABLE [dbo].[";
                                strSQL = strSQL + strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                strSQL = strSQL + "] (";
                                strSQL = strSQL + "[Seq] [bigint] IDENTITY (1, 1) PRIMARY KEY , ";
                                strSQL = strSQL + "[TimeRcv] [datetime] , ";
                                strSQL = strSQL + "[Node] [varchar] (50) , ";
                                strSQL = strSQL + "[EventTime] [datetime] , ";
                                strSQL = strSQL + "[UserName] [varchar] (30) , ";
                                strSQL = strSQL + "[Area] [varchar] (50) , ";
                                strSQL = strSQL + "[Object] [varchar] (200) , ";
                                strSQL = strSQL + "[Object_Type] [varchar] (50) , ";//Hsinson961121 add�]�ƫ���
                                strSQL = strSQL + "[Object_DevNo] [varchar] (150) , ";//Hsinson961121 add�]�ƽs��
                                strSQL = strSQL + "[Train] [varchar] (10) , ";//Hsinson961128 add�C�����X
                                strSQL = strSQL + "[EventType] [varchar] (50) , ";
                                strSQL = strSQL + "[Severity] [int] , ";
                                strSQL = strSQL + "[ShortMessage] [varchar] (20) , ";
                                strSQL = strSQL + "[Message] [varchar] (200) ";
                                strSQL_History = strSQL + ")";
                                CmdDB.CommandText = strSQL_History; //����إ߷s��ƪ����O
                                CmdDB.ExecuteNonQuery();
                                this.OnDisplayStringMessage("SQL���O=" + strSQL_History);

                                // �b�s�ت�Table���A�s�W���ޭ�
                                try
                                {
                                    strAtsEventDBSQLServer_Table_ExtName = sdate;

                                    string strAtsTableName = strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                    strSQL = @"if not EXISTS  (SELECT * FROM sys.indexes WHERE name='IX_ShortMessage' AND object_id = OBJECT_ID('" + strAtsTableName + "'))";

                                    strSQL = strSQL + " CREATE NONCLUSTERED INDEX [IX_ShortMessage] ON [dbo].[" + strAtsTableName + "](";
                                    strSQL = strSQL + "[ShortMessage] ASC)";
                                    strSQL = strSQL + "WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]";

                                    strSQL_History = strSQL;
                                    CmdDB.CommandText = strSQL_History; //����إ߷s��ƪ����O
                                    CmdDB.ExecuteNonQuery();
                                    this.OnDisplayStringMessage("���ƪ�" + strAtsTableName + "�۰ʫإ�(Create)���ޭ� IX_ShortMessage");
                                    this.OnDisplayStringMessage("SQL���O=" + strSQL_History);
                                }
                                catch (Exception Ex)
                                {
                                }


                                if (blnAtsTwcDBEnable)
                                {
                                    strDebug = "25";
                                    //Hsinson970219 add TWC Table
                                    strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                                    strSQL = strSQL + strAtsTwcDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                    strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                                    strSQL = strSQL + " CREATE TABLE [dbo].[";
                                    strSQL = strSQL + strAtsTwcDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                    strSQL = strSQL + "] (";
                                    strSQL = strSQL + "[Seq] [bigint] IDENTITY (1, 1) PRIMARY KEY , ";
                                    strSQL = strSQL + "[TimeRcv] [datetime] , ";
                                    strSQL = strSQL + "[EventTime] [datetime] , ";
                                    strSQL = strSQL + "[Loc] [char] (5) , ";
                                    strSQL = strSQL + "[Track] [char] (10) , ";
                                    strSQL = strSQL + "[Train] [char] (3) , ";
                                    strSQL = strSQL + "[Berth] [char] (1) , ";
                                    strSQL = strSQL + "[Ready] [char] (1) , ";
                                    strSQL = strSQL + "[DoorClose] [char] (1) , ";
                                    strSQL = strSQL + "[Motion] [char] (1) , ";
                                    strSQL = strSQL + "[Dest] [char] (2) , ";
                                    strSQL = strSQL + "[Crew] [char] (3) ";
                                    strSQL = strSQL + ")";
                                    CmdDB.CommandText = strSQL; //����إ߷s��ƪ����O
                                    CmdDB.ExecuteNonQuery();
                                    this.OnDisplayStringMessage("SQL���O=" + strSQL);

                                    // �b�s�ت�Table���A�s�W���ޭ�
                                    try
                                    {
                                        //CREATE NONCLUSTERED INDEX [IX_EventTime&Loc] ON [dbo].[TWC20150113] 
                                        //(
                                        //    [EventTime] ASC,
                                        //    [Loc] ASC,
                                        //    [Train] ASC
                                        //)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]


                                        strAtsEventDBSQLServer_Table_ExtName = sdate;

                                        string strAtsTableName = strAtsTwcDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                        strSQL = @"if not EXISTS  (SELECT * FROM sys.indexes WHERE name='IX_EventTime&Loc' AND object_id = OBJECT_ID('" + strAtsTableName + "'))";

                                        strSQL = strSQL + " CREATE NONCLUSTERED INDEX [IX_EventTime&Loc] ON [dbo].[" + strAtsTableName + "](";
                                        strSQL = strSQL + "[EventTime] ASC,";
                                        strSQL = strSQL + "[Loc] ASC,";
                                        strSQL = strSQL + "[Train] ASC";
                                        strSQL = strSQL + " ) WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]";

                                        strSQL_History = strSQL;
                                        CmdDB.CommandText = strSQL_History; //����إ߷s��ƪ����O
                                        CmdDB.ExecuteNonQuery();
                                        this.OnDisplayStringMessage("���ƪ�" + strAtsTableName + "�۰ʫإ�(Create)���ޭ� IX_EventTime&Loc");
                                        this.OnDisplayStringMessage("SQL���O=" + strSQL_History);
                                    }
                                    catch (Exception Ex)
                                    {
                                    }




                                }//if (blnAtsTwcDBEnable)
                            }//else if (strAtsEventDBSQLServer_Table_ExtName != sdate)

                            //Hsinson 961121						
                            //						strSQL= " (TimeRcv,Node, EventTime, Area,Object,EventType,Severity,ShortMessage,Message)" +
                            //							" values ('"+ strTimeRcv +"', '"+ strNode +"','" +strTime+ "','"+ strArea +"','"+strObject+"','"+strEvent+"',"+strSeverity+",'"+strShortMessage+"','"+strMessage+" ')";
                            //Hsinson 961128
                            //						strSQL= " (TimeRcv,Node, EventTime, Area,Object,Object_Type,Object_DevNo,EventType,Severity,ShortMessage,Message)" +
                            //							" values ('"+ strTimeRcv +"', '"+ strNode +"','" +strTime+ "','"+ strArea +"','"+strObject+"','"+strObject_Type+"','"+strObject_DevNo+"','"+strEvent+"',"+strSeverity+",'"+strShortMessage+"','"+strMessage+" ')";


                            strDebug = "26";
                            strSQL = " (TimeRcv,Node, EventTime, UserName, Area,Object,Object_Type,Object_DevNo, Train, EventType,Severity,ShortMessage,Message)" +
                                " values ('" + strTimeRcv + "', '" + strNode + "','" + strTime + "','" + strUser + "','" + strArea + "','" + strObject + "','" + strObject_Type + "','" + strObject_DevNo + "','" + strTrain + "','" + strEvent + "'," + strSeverity + ",'" + strShortMessage + "','" + strMessage + " ')";

                            strSQL_History = "insert into " + strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName + strSQL;

                            //970219Hsinson debug marked
                            CmdDB.CommandText = strSQL_History; //�s�J���v��Ʈw							
                            CmdDB.ExecuteNonQuery();

                            strDebug = "27";
                            //Hsinson970219
                            if ((blnAtsTwcDBEnable) && (strShortMessage.IndexOf("TWC") >= 0))
                            {
                                strBerth = "";
                                strReady = "";
                                strDoorClose = "";
                                strMotion = "";
                                strCrew = "";
                                strDest = "";

                                strSQL = " update " + strAtsTwcDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName + " set ";
                                strSQL = strSQL + " TimeRcv= '" + strTimeRcv + "' ";
                                strSQL = strSQL + ", EventTime= '" + strTime + "' ";
                                strSQL = strSQL + ", Loc= '" + strLoc + "' ";
                                strSQL = strSQL + ", Track= '" + strTrack + "' ";
                                strSQL = strSQL + ", Train= '" + strTrain + "' ";
                                idxTrain = strMessage.IndexOf("Berth");
                                strDebug = "28";
                                if (idxTrain >= 0)
                                {
                                    strDebug = "29";
                                    //(Track: TCL_R33_D1208 TRN: 118): Train Berthed On 
                                    //                                       12345678
                                    strBerth = strMessage.Substring(idxTrain + 8, 2) == "On" ? "1" : "0";
                                    strSQL = strSQL + ", Berth= '" + strBerth + "' ";
                                }//Berth
                                else
                                {
                                    strDebug = "30";
                                    idxTrain = strMessage.IndexOf("Ready");
                                    if (idxTrain >= 0)
                                    {
                                        strDebug = "31";
                                        //(Track: TCL_R33_U1208 TRN: 000): Train Ready On 
                                        //                                       123456
                                        strReady = strMessage.Substring(idxTrain + 6, 2) == "On" ? "1" : "0";
                                        strSQL = strSQL + ", Ready= '" + strReady + "' ";
                                    }//Ready
                                    else
                                    {
                                        strDebug = "32";
                                        idxTrain = strMessage.IndexOf("Close");
                                        if (idxTrain >= 0)
                                        {   //(Track: TCL_R33_D1208 TRN: 118): Doors Closed On 
                                            //                                       1234567
                                            strDebug = "33";
                                            strDoorClose = strMessage.Substring(idxTrain + 7, 2) == "On" ? "1" : "0";
                                            strSQL = strSQL + ", DoorClose= '" + strDoorClose + "' ";
                                        }//DoorClose
                                        else
                                        {
                                            strDebug = "34";

                                            idxTrain = strMessage.IndexOf("Motion");
                                            if (idxTrain >= 0)
                                            {  //(Track: TCL_R33_D1208 TRN: 116): Motion Detect On 
                                                //                                 12345678901234
                                                strDebug = "35";

                                                strMotion = strMessage.Substring(idxTrain + 14, 2) == "On" ? "1" : "0";
                                                strSQL = strSQL + ", Motion= '" + strMotion + "' ";
                                            }
                                            else
                                            {
                                                strDebug = "36";

                                                idxTrain = strMessage.IndexOf("Crew");
                                                if (idxTrain >= 0)
                                                {//(Track: TCL_R33_U1208 TRN: 101): Crew Changed to 277 
                                                    //                                 1234567890123456   
                                                    strDebug = "37";

                                                    strCrew = strMessage.Substring(idxTrain + 16, 3);
                                                    strSQL = strSQL + ", Crew= '" + strCrew + "' ";
                                                }//Crew
                                                else
                                                {
                                                    strDebug = "38";

                                                    idxTrain = strMessage.IndexOf("Dest");
                                                    if (idxTrain >= 0)
                                                    {
                                                        strDebug = "39";

                                                        //(Track: TCL_R33_U1208 TRN: 101): Destination Code Changed to 24
                                                        //                                 1234567890123456789012345678  
                                                        strDest = strMessage.Substring(idxTrain + 28, 2);
                                                        strSQL = strSQL + ", Dest= '" + strDest + "' ";
                                                    }//Dest
                                                    else
                                                    {
                                                    }//Dest
                                                }//Crew
                                            }//Motion
                                        }//DoorClose
                                    }//Ready
                                }//Berth
                                strSQL = strSQL + " where (EventTime = '" + strTime + "') and (Loc= '" + strLoc + "') and (Track= '" + strTrack + "') and (Train = '" + strTrain + "') ";
                                //							strSQL = " update " +  strAtsTwcDBSQLServer_Table +  " set ";
                                //							strSQL = strSQL	+ " TimeRcv= '"+ strTimeRcv + "' ";
                                //							strSQL = strSQL	+ ", EventTime= '"+strEventTime +"' ";
                                //							strSQL = strSQL	+ ", Loc= '"+ strLoc +"' ";
                                //							strSQL = strSQL	+ ", Track= '"+stTrack +"' ";
                                //							strSQL = strSQL	+ ", Train= '"+strTrain +"' ";
                                //							strSQL = strSQL	+ ", Berth= '"+strBerth +"' ";
                                //							strSQL = strSQL	+ ", Ready= '"+strReady +"' ";
                                //							strSQL = strSQL	+ ", DoorClose= '"+strDoorClose +"' ";
                                //							strSQL = strSQL	+ ", Motion= '"+strMotion +"' ";
                                //							strSQL = strSQL	+ ", Crew= '"+strCrew +"' ";
                                //							strSQL = strSQL	+ ", Dest= '" +strDest+"' " ;
                                //							strSQL = strSQL	+ " where ( NodeName = '" +  strNodeName + "') ";
                                CmdDB.CommandText = strSQL;	//��s�ܧY�ɸ�Ʈw
                                iRowUpdate = CmdDB.ExecuteNonQuery();
                                if (iRowUpdate <= 0)  //�Y�S�����A�h�ηs�W�覡�N��ƶ�JAtsTwc��ƪ�
                                {
                                    strSQL = "insert into " + strAtsTwcDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName;
                                    strSQL = strSQL + " (TimeRcv, EventTime, Loc,Track,Train,Berth, Ready, DoorClose,Motion,Crew,Dest) ";
                                    strSQL = strSQL + " values ('" + strTimeRcv + "', '" + strTime + "','" + strLoc + "','" + strTrack + "','" + strTrain + "','" + strBerth + "','" + strReady + "','" + strDoorClose + "','" + strMotion + "','" + strCrew + "','" + strDest + " ') ";

                                    CmdDB.CommandText = strSQL;	//��s�ܧY�ɸ�Ʈw
                                    iRowUpdate = CmdDB.ExecuteNonQuery();
                                }//if

                            }//if ((blnAtsTwcDBEnable) && (strShortMessage.IndexOf("TWC")>=0))

                        }//try 2
                        catch (Exception Ex)
                        {
                            this.OnDisplayStringMessage("Error:");
                            this.OnDisplayStringMessage(strSQL_History);
                            this.OnDisplayStringMessage(Ex.Message);
                        }//catch 2

                    }//for


                    //20130530 SFI ��������
                    myTrans.Commit();

                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JAtsEventDB  ��m(" + this.strAtsEventDBSQLServer_IP + ") " + strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }//lock

            }//try
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JAtsEventDB  ��m(" + this.strAtsEventDBSQLServer_IP + ") " + this.strAtsEventDBSQLServer_Table + strAtsEventDBSQLServer_Table_ExtName + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        } //-------------------------------------------------------------End of fnSetAtsEventDBMoveData

        //�ƻs�iWinSysEvtDB fnSetTLos_NHDBMoveData
        void fnSetWinSysEvtDBMoveData(DataTable vdtData)
        {
            int strSeq;
            string strTimeRcv;
            string strNode;
            string strTime;
            string strEvtLog;
            string strType;
            string strSource;
            string strCategory;
            string strID = ""; //Hsinson961128 add, �C�����X
            string strMessage;
            string strUserName;
            string strComputerName;


            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn = "";
            string strSQL = "";
            string strSQL_History = "";
            //SqlTransaction myTrans;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]WinSysEvt2]%'";
            //dvData.Sort="seq";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strWinSysEvtDBSQLServer_IP + ";uid=" + this.strWinSysEvtDBSQLServer_User + ";pwd=" + this.strWinSysEvtDBSQLServer_Password + ";database=" + this.strWinSysEvtDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JWinSysEvtDB  ��m(" + this.strWinSysEvtDBSQLServer_IP + ") " + this.strWinSysEvtDBSQLServer_Table + "yyyymmdd ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                    }

                    //					myTrans=conDB.BeginTransaction (IsolationLevel.ReadUncommitted , "SampleTransaction");
                    //					CmdDB.Transaction = myTrans;

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;

                        //	Hsinson 980502 �ק��ন�r�ꤧ�y�k
                        //						sltData=fnGetArrayValuebyIndex((string)dvData[i].Row["Content"]);
                        //						strTimeRcv=(string)dvData[i].Row["TimeRcv"];
                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        strTimeRcv = dvData[i].Row["TimeRcv"].ToString();
                        DateTime test = Convert.ToDateTime(strTimeRcv);
                        strTimeRcv = test.ToString("yyyy-MM-dd HH:mm:ss");

                        //
                        //[WinSysEvt]
                        //EvtLog=Security;   
                        //Type=AUDIT_FAILURE; 
                        //NodeName=ID.SCA.PCB.PC4DEV;
                        //NodeTime=2007/12/27 18:00:51;
                        //Source=Security;
                        //Category=�n�J/�n�X;
                        //ID=534;
                        //Msg=H0-1-169$;
                        //User=SYSTEM;
                        //ComputerName=H0-C-619

                        strSeq = int.Parse(dvData[i].Row["Seq"].ToString());
                        strEvtLog = ((string)sltData["EvtLog"]);
                        strType = ((string)sltData["Type"]);
                        strNode = (string)sltData["NodeName"];
                        strTime = (string)sltData["NodeTime"];
                        strSource = ((string)sltData["Source"]);
                        strCategory = ((string)sltData["Category"]);
                        strID = ((string)sltData["ID"]);
                        strMessage = (string)sltData["Msg"];
                        strUserName = (string)sltData["User"];
                        strComputerName = (string)sltData["ComputerName"];

                        //961018 Hsinson �ˬd�ƥ����O�_��s�A�Y�O�h��s���ͷs����ƪ�
                        //string sdate = fnGetDayStr(strTime);
                        //if (sdate == null)
                        //{
                        //    WinSysEvtErrorCount++;
                        //    this.OnDisplayStringMessage("WinSysEvt Format error, Error Count=" + WinSysEvtErrorCount);
                        //    textBox_WinSysEvtErrorCount.Text = WinSysEvtErrorCount.ToString();
                        //    textBox_LastError.Text = strTimeRcv + " - " + dvData[i].Row["Content"].ToString();
                        //}
                        //else if (strWinSysEvtDBSQLServer_Table_ExtName != sdate)
                        //{
                        //    //		1	seq	bigint	8	0
                        //    //		0	TimeRcv	datetime	8	1
                        //    //		0	Node	varchar	50	1
                        //    //		0	EventTime	datetime	8	1
                        //    //		0	EventLog	varchar	50	1
                        //    //		0	EventType	varchar	50	1
                        //    //		0	Source	varchar	50	1
                        //    //		0	Category	varchar	50	1
                        //    //		0	ID	varchar	10	1
                        //    //		0	Message	varchar	200	1
                        //    //		0	UserName	varchar	50	1
                        //    //		0	ComputerName	varchar	50	1

                        //    strWinSysEvtDBSQLServer_Table_ExtName = sdate;
                        //    this.OnDisplayStringMessage("�ƥ����P���e��ƪ�������P�A�۰ʫإ�(Create)�s��ƪ�" + strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName);
                        //    strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                        //    strSQL = strSQL + strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName;
                        //    strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                        //    strSQL = strSQL + " CREATE TABLE [dbo].[";
                        //    strSQL = strSQL + strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName;
                        //    strSQL = strSQL + "] (";
                        //    strSQL = strSQL + "[Seq] [bigint] IDENTITY (1, 1) PRIMARY KEY , ";
                        //    strSQL = strSQL + "[TimeRcv] [datetime] , ";
                        //    strSQL = strSQL + "[Node] [varchar] (50) , ";
                        //    strSQL = strSQL + "[EventTime] [datetime] , ";
                        //    strSQL = strSQL + "[EventLog] [varchar] (50) , ";
                        //    strSQL = strSQL + "[EventType] [varchar] (50) , ";
                        //    strSQL = strSQL + "[Source] [varchar] (50) , ";
                        //    strSQL = strSQL + "[Category] [varchar] (50) , ";
                        //    strSQL = strSQL + "[ID] [varchar] (10), ";
                        //    strSQL = strSQL + "[Message] [varchar] (200), ";
                        //    strSQL = strSQL + "[UserName] [varchar] (50), ";
                        //    strSQL = strSQL + "[ComputerName] [varchar] (50) ";
                        //    strSQL = strSQL + ")";
                        //    CmdDB.CommandText = strSQL; //����إ߷s��ƪ����O
                        //    CmdDB.ExecuteNonQuery();
                        //    this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        //}
                        strSQL = " (Seq,TimeRcv,Node, EventTime, EventLog,EventType,Source,Category, ID, Message,UserName,ComputerName)" +
                            " values ('" + strSeq + "', '"  + strTimeRcv + "', '" + strNode + "','" + strTime + "','" + strEvtLog + "','" + strType + "','" + strSource + "','" + strCategory + "','" + strID + "','" + strMessage + "','" + strUserName + "','" + strComputerName + " ')";
                        //strSQL = "insert into " + strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName + strSQL;
                        strSQL = "insert into " + strWinSysEvtDBSQLServer_Table + strSQL;
                        CmdDB.CommandText = strSQL; //�s�J��Ʈw
                        CmdDB.ExecuteNonQuery();

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JWinSysEvtDB  ��m(" + this.strWinSysEvtDBSQLServer_IP + ") " + strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JWinSysEvtDB  ��m(" + this.strWinSysEvtDBSQLServer_IP + ") " + this.strWinSysEvtDBSQLServer_Table + strWinSysEvtDBSQLServer_Table_ExtName + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        }
        //-------------------------------------------------------------End of fnSetWinSysEvtDBMoveData

        //�ƻs�iTLOS_NHDB fnSetTLos_NHDBMoveDataOPCA(StationStatus)
        void fnSetTLos_NHDBMoveDataOPCA(DataTable vdtData)
        {
            int strSeq;
            string strTimeRcv;
            string strUpdateTime;
            string strStation;
            string strPlatform;
            string strTrainID;
            string strDwellTime;
            string strArrivalDeparture;
            string strReadTime;
            string strWriteTime;


            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn = "";
            string strSQL = "";
            string strSQL_History = "";

            DataView dvData = new DataView(vdtData);
            //STATION STATUS
            //dvData.RowFilter = "Content LIKE '[[]OPCA]%'";
            dvData.RowFilter = "Content LIKE '%OPCA%'";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strTLOS_NHDBSQLServer_IP + ";uid=" + this.strTLOS_NHDBSQLServer_User + ";pwd=" + this.strTLOS_NHDBSQLServer_Password + ";database=" + this.strTLOS_NHDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n��s"+ this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "StationStatus" + "��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;

                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        strTimeRcv = dvData[i].Row["TimeRcv"].ToString();
                        DateTime test = Convert.ToDateTime(strTimeRcv);
                        strTimeRcv = test.ToString("yyyy-MM-dd HH:mm:ss");

                        strUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //OPCA Format
                        //STN=BR13;PF=D2;RT=2020/10/15 14:59:30;TID=0;DT=65;AD=;WT=2020/10/15 15:00:10

                        strSeq = int.Parse(dvData[i].Row["Seq"].ToString());
                        strStation = ((string)sltData["STN"]);
                        strPlatform = ((string)sltData["PF"]);
                        strTrainID = ((string)sltData["TID"]) != null ? ((string)sltData["TID"]).PadLeft(3,'0') : "000";
                        strDwellTime = (string)sltData["DT"];
                        strArrivalDeparture = (string)sltData["AD"];
                        strReadTime = (string)sltData["RT"];
                        strWriteTime = (string)sltData["WT"];


                        strSQL = " Update TLOS_NH.dbo.StationStatus " +
                            "set TrainID ='" + strTrainID + "',DwellTime='" + strDwellTime + "',ArrivalDeparture='" + strArrivalDeparture +
                            "',ReadTime='" + strReadTime + "',WriteTime='" + strWriteTime +"',TimeRcv='"+strTimeRcv+ "',UpdateTime='" + strUpdateTime + "',Seq='" +strSeq+"' "+
                            "Where Station='" + strStation + "' and Platform='" + strPlatform + "'";

                        CmdDB.CommandText = strSQL; //��s��Ʈw
                        CmdDB.ExecuteNonQuery();

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("��s"+this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "StationStatus" + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��s"+ this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "StationStatus" + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        }
        //-------------------------------------------------------------End of fnSetTLos_NHDBMoveData OPCA

        //�ƻs�iTLOS_NHDB fnSetTLos_NHDBMoveDataOPCB(TrainStatus)
        void fnSetTLos_NHDBMoveDataOPCB(DataTable vdtData)
        {
            string strTimeRcv;
            string strUpdateTime;
            string strTK;
            string strOffset;
            string strSpeed;
            string strTrainID;
            string strReadTime;
            string strWriteTime;
            string tempTable="";
            bool existTable = false;

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn = "";
            string strSQL = "";
            string strSQL_History = "";

            DataView dvData = new DataView(vdtData);
            //Train STATUS
            //dvData.RowFilter = "Content LIKE '[[]OPCB]%'";
            dvData.RowFilter = "Content LIKE '%OPCB%'";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strTLOS_NHDBSQLServer_IP + ";uid=" + this.strTLOS_NHDBSQLServer_User + ";pwd=" + this.strTLOS_NHDBSQLServer_Password + ";database=" + this.strTLOS_NHDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + "��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;

                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        strTimeRcv = dvData[i].Row["TimeRcv"].ToString();
                        DateTime test = Convert.ToDateTime(strTimeRcv);
                        strTimeRcv = test.ToString("yyyy-MM-dd HH:mm:ss");

                        strUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //OPCB Format
                        //[OpcB];RT=2020/10/16 08:25:33;TK=R1S165;TID=193;WT=2020/10/16 12:48:00
                        //[OpcB];RT=2020/11/30 16:24:46;TK=R2S123;TID=27;GR=101;Offs=2273;Speed=;WT=2020/11/30 16:24:57


                        strTK = ((string)sltData["TK"]);
                        strOffset = ((string)sltData["Offs"]);
                        //20210412 wilson �s�W�t��
                        strSpeed = ((string)sltData["Speed"]) != null ? ((string)sltData["Speed"]) : "";
                        strTrainID = ((string)sltData["TID"]) != null ? ((string)sltData["TID"]).PadLeft(3, '0') : "000";
                        strReadTime = (string)sltData["RT"];
                        strWriteTime = (string)sltData["WT"];


                        strSQL = " Update TLOS_NH.dbo.TrainStatus " +
                            //"set TK ='" + strTK + "',Offset ='" + ((int)float.Parse(strOffset)*0.1*0.3048).ToString()+ "',Speed ='" + strSpeed +
                            "set TK ='" + strTK + "',Speed ='" + strSpeed + "',Offset = cast(cast('" + strOffset +
                            "' as float) * 0.1 * 0.3048 as int),TKReadTime='" + strReadTime + "',WriteTime='" + strWriteTime + "',TimeRcv='" + strTimeRcv + "',UpdateTime='" + strUpdateTime + "' " +
                            "Where TrainID='" + strTrainID + "' ";

                        CmdDB.CommandText = strSQL; //��s��Ʈw
                        CmdDB.ExecuteNonQuery();

                    }//for

                    ///
                    //Wilson 20210420 �ק�
                    //�P�_��ƪ��O�_�s�b�A�s�WTrainLocationHisYYYYMM��ƪ��C
                    
                    tempTable = "TrainLocationHis" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0');

                    strSQL = "select ISNULL(COUNT(*), 0) from sysobjects where id = object_id(N'[dbo].[" + tempTable + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1 ";
                    CmdDB.CommandText = strSQL;
                    int counters = int.Parse(CmdDB.ExecuteScalar().ToString());

                    if (counters == 0)
                    {
                        strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                        strSQL = strSQL + tempTable;
                        strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                        strSQL = strSQL + " CREATE TABLE [dbo].[";
                        strSQL = strSQL + tempTable;
                        strSQL = strSQL + "] (";
                        strSQL = strSQL + "[Seq] [BIGINT] IDENTITY (1, 1) PRIMARY KEY , ";
                        strSQL = strSQL + "[TrainID] [nvarchar](5) NULL , ";
                        strSQL = strSQL + "[VehicleID] [nvarchar](5) NULL , ";
                        strSQL = strSQL + "[TK] [nvarchar](6) NULL , ";
                        strSQL = strSQL + "[Offset] [nvarchar](5) NULL , ";
                        strSQL = strSQL + "[Speed] [nvarchar](3) NULL , ";
                        strSQL = strSQL + "[OCC] [nvarchar](1) NULL , ";
                        strSQL = strSQL + "[Distance] [float] NULL , ";
                        strSQL = strSQL + "[DwellTime] [nvarchar](3) NULL , ";
                        strSQL = strSQL + "[ArrivalDeparture] [nvarchar](1) NULL , ";
                        strSQL = strSQL + "[TRReadTime] [datetime] NULL , ";
                        strSQL = strSQL + "[TKReadTime] [datetime] NULL , ";
                        strSQL = strSQL + "[WriteTime] [datetime] NULL , ";
                        strSQL = strSQL + "[TimeRcv] [datetime] NULL , ";
                        strSQL = strSQL + "[UpdateTime] [datetime] NULL ";
                        strSQL = strSQL + ")";

                        this.OnDisplayStringMessage("�s�W"+tempTable+"��ƪ��ASQL���O=" + strSQL);
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }
                    //Wilson 20210420 �ק�
                    //�N��Ƽg�JTrainLocationHisYYYYMM��ƪ��C
                    
                        strSQL = "Insert into dbo." + tempTable
                        + " ([TrainID],[VehicleID],[TK],[Offset],[Speed],[OCC],[Distance],[DwellTime],[ArrivalDeparture],[TRReadTime],[TKReadTime],[WriteTime],[TimeRcv],[UpdateTime]) "
                        + " Select [TID],[PVID],[TK],[Offset],[Speed],[OCC],[Distance],[DwellTime],[ArrivalDeparture],[TRReadTime],[TKReadTime],[WriteTime],[TimeRcv],[UpdateTime] "
                        + " FROM [TLOS_NH].[dbo].[vw_TrainLocationHis]";
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    

                    //CmdDB.CommandText = "usp_TrainLocationHis";
                    //CmdDB.CommandType = CommandType.StoredProcedure;
                    //CmdDB.ExecuteNonQuery();


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        }
        //-------------------------------------------------------------End of fnSetTLos_NHDBMoveData OPCB

        //�ƻs�iTLOS_NHDB fnSetTLos_NHDBMoveDataOPCC(VehicleStatus)
        void fnSetTLos_NHDBMoveDataOPCC(DataTable vdtData)
        {
            string strTimeRcv;
            string strUpdateTime;
            string strGID;
            string strTrainID;
            string strVehicleID;
            string strDistance;
            string strReadTime;
            string strWriteTime;


            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn = "";
            string strSQL = "";
            string strSQL_History = "";

            DataView dvData = new DataView(vdtData);
            //TRAIN STATUS
            //dvData.RowFilter = "Content LIKE '[[]OPCC]%'";
            dvData.RowFilter = "Content LIKE '%OPCC%'";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strTLOS_NHDBSQLServer_IP + ";uid=" + this.strTLOS_NHDBSQLServer_User + ";pwd=" + this.strTLOS_NHDBSQLServer_Password + ";database=" + this.strTLOS_NHDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + "��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        SortedList sltData;

                        sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                        strTimeRcv = dvData[i].Row["TimeRcv"].ToString();
                        DateTime test = Convert.ToDateTime(strTimeRcv);
                        strTimeRcv = test.ToString("yyyy-MM-dd HH:mm:ss");

                        strUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //OPCC Format
                        //[OpcC];GID=50;TID=0;VID=0;RT=2020/10/16 10:01:38;WT=2020/10/16 11:38:15
                        //[OpcC];GID=17;TID=0;VID=177;Dist=3672499200;RT=2020/11/30 16:23:54;WT=2020/11/30 16:24:57


                        strGID = ((string)sltData["GID"]);
                        strTrainID = ((string)sltData["TID"]) != null ? ((string)sltData["TID"]).PadLeft(3, '0') : "000";
                        strVehicleID = ((string)sltData["VID"]) != null ? ((string)sltData["VID"]).PadLeft(3, '0') : "000";
                        //strDistance = ((string)sltData["Dist"]) != null ? Math.Round(((float)sltData["Dist"])/3280.84,1).ToString() : "000";
                        strDistance = ((string)sltData["Dist"]) != null ? ((string)sltData["Dist"]) : "000";
                        strReadTime = (string)sltData["RT"];
                        strWriteTime = (string)sltData["WT"];


                        strSQL = " Update TLOS_NH.dbo.TrainStatus " +
                            "set TrainID ='" + strTrainID + "',VehicleID='" + strVehicleID + "',Distance='" + strDistance +
                            "',TRReadTime='" + strReadTime + "',WriteTime='" + strWriteTime + "',TimeRcv='" + strTimeRcv + "',UpdateTime='" + strUpdateTime + "' " +
                            "Where GID='" + strGID+ "' ";

                        CmdDB.CommandText = strSQL; //��s��Ʈw
                        CmdDB.ExecuteNonQuery();

                    }//for


                    //	myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��s" + this.strTLOS_NHDBSQLServer_DataBase + "��m(" + this.strTLOS_NHDBSQLServer_IP + ") " + "TrainStatus" + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }


        }
        //-------------------------------------------------------------End of fnSetTLos_NHDBMoveData OPCC

        // Kelvin 2010/12/22, starting
        void fnSetDBSpaceDBMoveData(DataTable vdtData)
        {
            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]DBSpace]%'";
            String strSQL = String.Empty;
            String strSQL_History = String.Empty;
            String strSQL_LastOne = String.Empty;
            String strTimeRcv = String.Empty;
            SqlConnection conDB = null;
            int iRowUpdate = 0;
            try
            {
                lock (this)
                {
                    string strConn = @"server=" + this.strDBSpaceDBSQLServer_IP + ";uid=" + this.strDBSpaceDBSQLServer_User + ";pwd=" + this.strDBSpaceDBSQLServer_Password + ";database=" + this.strDBSpaceDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();

                    SqlCommand CmdDB = conDB.CreateCommand();
                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JDBSpaceDB  ��m(" + this.strDBSpaceDBSQLServer_IP + ") " + this.strDBSpaceDBSQLServer_Table + "yyyymmdd ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");

                        strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                        strSQL = strSQL + this.strDBSpaceDBSQLServer_Table;
                        strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                        strSQL = strSQL + " CREATE TABLE [dbo].[";
                        strSQL = strSQL + this.strDBSpaceDBSQLServer_Table;
                        strSQL = strSQL + "]') AND OBJECTPROPERTY(ID, N'ISUSERTABLE') = 1) ";
                        strSQL = strSQL + " CREATE TABLE [dbo].[";
                        strSQL = strSQL + this.strDBSpaceDBSQLServer_Table;
                        strSQL = strSQL + "] (";
                        strSQL = strSQL + "[Seq] [BIGINT] IDENTITY (1, 1) PRIMARY KEY , ";
                        strSQL = strSQL + "[TimeRcv] [DATETIME] , ";
                        strSQL = strSQL + "[Node] [VARCHAR] (50) , ";
                        strSQL = strSQL + "[EventTime] [DATETIME] , ";
                        strSQL = strSQL + "[DatabaseName] [VARCHAR] (50) , ";
                        strSQL = strSQL + "[TotalDataSpace] [FLOAT] , ";
                        strSQL = strSQL + "[UsedDataSpace] [FLOAT] , ";
                        strSQL = strSQL + "[FreeDataSpace] [FLOAT] , ";
                        strSQL = strSQL + "[TotalLogSpace] [FLOAT] , ";
                        strSQL = strSQL + "[UsedLogSpace] [FLOAT] , ";
                        strSQL = strSQL + "[FreeLogSpace] [FLOAT] ";
                        strSQL = strSQL + ")";

                        this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();

                        strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                        strSQL = strSQL + this.strDBSpaceDBSQLServer_Table_LastOne;
                        strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                        strSQL = strSQL + " CREATE TABLE [DBO].[";
                        strSQL = strSQL + this.strDBSpaceDBSQLServer_Table_LastOne;
                        strSQL = strSQL + "] (";
                        strSQL = strSQL + "[Seq] [BIGINT] IDENTITY (1, 1) PRIMARY KEY , ";
                        strSQL = strSQL + "[TimeRcv] [DATETIME] , ";
                        strSQL = strSQL + "[Node] [VARCHAR] (50) , ";
                        strSQL = strSQL + "[EventTime] [DATETIME] , ";
                        strSQL = strSQL + "[DatabaseName] [VARCHAR] (50) , ";
                        strSQL = strSQL + "[FreeDataSpace] [float] NULL, ";
                        strSQL = strSQL + "[UsedDataSpace] [float] NULL, ";
                        strSQL = strSQL + "[TotalDataSpace] [float] NULL, ";
                        strSQL = strSQL + "[DataUsage]  AS ([scada].[fn_getPercent]([TotalDataSpace], [FreeDataSpace])), ";
                        strSQL = strSQL + "[LimitDataSpace] [float] NULL, ";
                        strSQL = strSQL + "[FreeLogSpace] [float] NULL, ";
                        strSQL = strSQL + "[UsedLogSpace] [float] NULL, ";
                        strSQL = strSQL + "[TotalLogSpace] [float] NULL, ";
                        strSQL = strSQL + "[LogUsage]  AS ([scada].[fn_getPercent]([TotalLogSpace], [FreeLogSpace])), ";
                        strSQL = strSQL + "[LimitLogSpace] [float] NULL, ";
                        strSQL = strSQL + "[LimitTotalSpace] [float] NULL, ";
                        strSQL = strSQL + "[Location] [INT] DEFAULT ((0)) ";
                        strSQL = strSQL + ")";

                        this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    for (int i = 0; i < dvData.Count; i++)
                    {
                        try
                        {
                            strTimeRcv = String.Format("{0:yyyy/MM/dd HH:mm:ss}", dvData[i].Row["TimeRcv"]);
                            SortedList sltData = fnGetArrayValuebyIndex(dvData[i].Row["Content"].ToString());
                            string strTime = (string)sltData["NodeTime"];
                            string strNode = (string)sltData["NodeName"];
                            string strDatabaseName = (string)sltData["DatabaseName"];
                            double totalDataSpace = Convert.ToDouble(sltData["TotalDataSpace"]);
                            double usedDataSpace = Convert.ToDouble(sltData["UsedDataSpace"]);
                            double freeDataSpace = Convert.ToDouble(sltData["FreeDataSpace"]);
                            double totalLogSpace = Convert.ToDouble(sltData["TotalLogSpace"]);
                            double usedLogSpace = Convert.ToDouble(sltData["UsedLogSpace"]);
                            double freeLogSpace = Convert.ToDouble(sltData["FreeLogSpace"]);

                            strSQL = " (TimeRcv, Node, EventTime, DatabaseName, TotalDataSpace, UsedDataSpace, FreeDataSpace, TotalLogSpace, UsedLogSpace, FreeLogSpace )";
                            strSQL += " values ('" + strTimeRcv + "', '" + strNode + "', '" + strTime + "', '" + strDatabaseName + "', " + totalDataSpace + ", " + usedDataSpace + ", " + freeDataSpace + ", " + totalLogSpace + ", " + usedLogSpace + ", " + freeLogSpace + ")";

                            if (this.blnDBSpaceDBEnable_History)
                            {
                                strSQL_History = " insert into " + strDBSpaceDBSQLServer_Table + strSQL;
                                //								this.OnDisplayStringMessage("SQL���O=" + strSQL_History);
                                CmdDB.CommandText = strSQL_History;
                                CmdDB.ExecuteNonQuery();
                            }

                            strSQL = " (TimeRcv, Node, EventTime, DatabaseName, TotalDataSpace, UsedDataSpace, FreeDataSpace, TotalLogSpace, UsedLogSpace, FreeLogSpace , LimitDataSpace , LimitLogSpace , LimitTotalSpace)";
                            strSQL += " values ('" + strTimeRcv + "', '" + strNode + "', '" + strTime + "', '" + strDatabaseName + "', " + totalDataSpace + ", " + usedDataSpace + ", " + freeDataSpace + ", " + totalLogSpace + ", " + usedLogSpace + ", " + freeLogSpace + ", " + totalDataSpace + ", " + totalLogSpace + ", " + (totalDataSpace + totalLogSpace) + ")";

                            if (this.blnDBSpaceDBEnable_LastOne)
                            {
                                strSQL_LastOne = " update " + strDBSpaceDBSQLServer_Table_LastOne + " set ";
                                strSQL_LastOne += " TimeRcv = '" + strTimeRcv + "'";
                                strSQL_LastOne += ", EventTime = '" + strTime + "'";
                                strSQL_LastOne += ", TotalDataSpace = " + totalDataSpace;
                                strSQL_LastOne += ", UsedDataSpace = " + usedDataSpace;
                                strSQL_LastOne += ", FreeDataSpace = " + freeDataSpace;
                                strSQL_LastOne += ", TotalLogSpace = " + totalLogSpace;
                                strSQL_LastOne += ", UsedLogSpace = " + usedLogSpace;
                                strSQL_LastOne += ", FreeLogSpace = " + freeLogSpace;
                                strSQL_LastOne += " where ( Node = '" + strNode + "'";
                                strSQL_LastOne += " and DatabaseName = '" + strDatabaseName + "'";
                                strSQL_LastOne += ") ";

                                //								this.OnDisplayStringMessage("SQL���O=" + strSQL_LastOne);
                                CmdDB.CommandText = strSQL_LastOne;
                                iRowUpdate = CmdDB.ExecuteNonQuery();
                                if (iRowUpdate < 1)
                                {
                                    strSQL_LastOne = "insert into " + strDBSpaceDBSQLServer_Table_LastOne + strSQL;
                                    //									this.OnDisplayStringMessage("SQL���O=" + strSQL_LastOne);
                                    CmdDB.CommandText = strSQL_LastOne;
                                    iRowUpdate = CmdDB.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DBSpaceErrorCount++;
                            this.OnDisplayStringMessage("DBSpace Format error, Error Count=" + DBSpaceErrorCount);
                            textBox_DBSpaceErrorCount.Text = DBSpaceErrorCount.ToString();
                            textBox_LastError.Text = strTimeRcv + " - " + dvData[i].Row["Content"].ToString();
                        }
                    }
                    this.OnDisplayStringMessage("�g�JDBSpaceDB  ��m(" + this.strDBSpaceDBSQLServer_IP + ") " + strDBSpaceDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                    if (conDB.State != System.Data.ConnectionState.Closed) conDB.Close();
                }
            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JDBSpaceDB  ��m(" + this.strDBSpaceDBSQLServer_IP + ") " + this.strDBSpaceDBSQLServer_Table + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                this.OnDisplayStringMessage(strSQL_History);
                if (conDB.State != System.Data.ConnectionState.Closed) conDB.Close();
                throw Ex;
            }
        }
        // Kelvin 2010/12/22, ending

        //�ƻs�iAlarmDB
        void fnSetIntellectDBMoveData(DataTable vdtData)
        {

            SqlConnection conDB = null;
            SqlCommand CmdDB = null;

            string strConn;
            string strSQL;
            int intCount = 0;

            int intItlDataKey = -1;

            string strTimeRcv = "";
            string strSource = "";
            string strContent = "";
            string strAlarm_level = "";
            string strwrk_dept = "";
            string strwrk_dept_name = "";
            string strBk_Fix_Id = "";
            string strBk_Fix_Code = "";
            //string strScada_ID="";
            //string strAlarm_Time="";
            string strAlarm_Ack = "";
            string strAlarm_Type = "";
            //string strrpt_dept="";
            string strequ_id = "";
            string strequ_code = "";
            string strequ_name = "";
            string strItem_Id = "";
            string strbk_fix_name = "";
            string strITEM_NO = "";

            //20181025 add strAlarmSec, strAlarmNum
            string strAlarmSec = "";
            string strAlarmNum = "";

            SqlDataAdapter daRepair;
            DataSet dsRepair = new DataSet();
            string seqTemp = "";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strAlarmDBSQLServer_IP + ";uid=" + this.strAlarmDBSQLServer_User + ";pwd=" + this.strAlarmDBSQLServer_Password + ";database=" + this.strAlarmDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP + ") " + this.strAlarmDBSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }
                    DateTime DateTimeRcv;
                    for (int i = 0; i < vdtData.Rows.Count; i++)
                    {

                        string str = MultipleCheckContent((string)vdtData.Rows[i]["Source"], vdtData.Rows[i]["Content"] == System.DBNull.Value ? "" : (string)vdtData.Rows[i]["Content"]);
                        string[] item_id = str.Split('|');
                        
                        for (int j = 0; j < item_id.Length - 1; j++)
                        {
                            intCount++;
                            DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                            strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");
                            strSource = (string)vdtData.Rows[i]["Source"];
                            strContent = (string)vdtData.Rows[i]["Content"];

                            strItem_Id = item_id[j].ToString();
                            strAlarm_level = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["Alarm_level"].Ordinal].ToString();
                            strwrk_dept = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["WRK_DEPTID"].Ordinal];
                            strwrk_dept_name = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["WRK_DEPT_NAME"].Ordinal];
                            strBk_Fix_Id = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["BK_FIX_ID"].Ordinal].ToString();
                            strBk_Fix_Code = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["Bk_Fix_Code"].Ordinal];
                            strAlarm_Ack = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["Alarm_Ack"].Ordinal].ToString();
                            strAlarm_Type = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["Alarm_Type"].Ordinal].ToString();
                            strequ_id = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["equ_id"].Ordinal].ToString();
                            strequ_code = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["EQU_CODE"].Ordinal];
                            strequ_name = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["EQU_NAME"].Ordinal];
                            strbk_fix_name = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["bk_fix_name"].Ordinal];

                            //strITEM_NO = (string)dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["ITEM_NO"].Ordinal];
                            strITEM_NO = MultipleStr;

                            strAlarmSec = dtMultipleAlarm.Select(" item_id='" + item_id[j] + "'")[0].ItemArray[dtMultipleAlarm.Columns["Alarm_Sec"].Ordinal].ToString();
                            
                            if (strAlarmSec != "0")
                            {
                                this.OnDisplayStringMessage("Multiple Alarm");
                                if (strAlarm_Ack == "1")
                                {

                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTableTemp +
                                         "       (       TimeRcv,             Source,             Content,                Alarm_Level,        RPT_DEPT_CODE,          WRK_DEPT_NAME,           BK_FIX_ID,            BK_FIX_CODE,      Alarm_Time,        Alarm_Ack,             Ack_Flag ,           Alarm_Type,            EQU_ID,            EQU_CODE,            EQU_NAME,            ITEM_ID,           BK_FIX_NAME,           ITEM_NO,            Alarm_Sec,               mark) " +
                                        "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' , GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "','" + strAlarmSec + "', '0')";
                                    CmdDB.CommandText = strSQL;
                                    CmdDB.ExecuteNonQuery();

                                    string sSql = "select top 1 seq from " + this.strAlarmDBSQLServer_RXTable + " order by seq Desc";
                                    CmdDB.CommandText = sSql;

                                    daRepair = new SqlDataAdapter(CmdDB);
                                    dsRepair.Clear();
                                    daRepair.Fill(dsRepair);
                                    if (dsRepair.Tables[0].Rows.Count > 0)
                                    {
                                        seqTemp = dsRepair.Tables[0].Rows[0].ItemArray[0].ToString();
                                    }

                                    strSQL = "if (select count(0) from RPT_SCADA_NEW where BK_FIX_ID='" + strBk_Fix_Id + "' and WS_CODE<>'F000')=0" +
                                        " begin INSERT INTO RPT_SCADA_NEW (SCADA_ID,RPT_FRM_CODE,RPT_DATE,RPT_TIME,BK_DWN_DATE,BK_DWN_TIME," +
                                        " RPT_DEPT_CODE,RPT_EMP_CODE,EQU_ID,BK_FIX_ID,BK_COND_DESC,RUN_STAT_CODE,WS_CODE) " +
                                        " values ( cast(cast(convert(char(8),getdate(),112) as bigint) * 100000 + cast(Right(cast(" + seqTemp + " as varchar),5) as int)  as char(13) )  ,'','','','" + strTimeRcv.Substring(0, 10).Replace(" ", "").Replace("/", "") + "','" + strTimeRcv.Substring(11).Replace(" ", "").Replace(":", "") + "'," +
                                        "'IDC','scada','" + strequ_id + "','" + strBk_Fix_Id + "',substring('" + strContent + "',1,200),'','')" +
                                        " end";
                                }
                                else  //�S���۰ʳ���
                                {
                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTableTemp +
                                         "       (        TimeRcv,              Source,               Content,                  Alarm_Level,            RPT_DEPT_CODE,          WRK_DEPT_NAME,              BK_FIX_ID,              BK_FIX_CODE,      Alarm_Time,          Alarm_Ack,             Alarm_Type,              EQU_ID,              EQU_CODE,              EQU_NAME,              ITEM_ID,             BK_FIX_NAME,             ITEM_NO,             Alarm_Sec,     mark) " +
                                         "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' , GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "','" + strAlarmSec + "',  '0')";
                                }
                                this.OnDisplayStringMessage("�g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP + ") " + this.strAlarmDBSQLServer_RXTableTemp + " ��Ʈw: ��Ƶ��Ʀ@ " + intCount + " ��");
                                CmdDB.CommandText = strSQL;
                                CmdDB.ExecuteNonQuery();

                                strSQL = "EXEC dbo.MultipleAlarm '" + strAlarmSec + "','" + strItem_Id + "'";
                                CmdDB.CommandText = strSQL;
                                CmdDB.ExecuteNonQuery();
                            }
                        } //end for item_id

                        intItlDataKey = fnCheckContent((string)vdtData.Rows[i]["Source"], vdtData.Rows[i]["Content"] == System.DBNull.Value ? "" : (string)vdtData.Rows[i]["Content"]);
                        if (intItlDataKey > 0)
                        {
                            intCount++;

                            DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                            strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");  //�S��
                            strSource = (string)vdtData.Rows[i]["Source"];
                            strContent = (string)vdtData.Rows[i]["Content"];

                            strItem_Id = intItlDataKey.ToString();
                            strAlarm_level = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Alarm_level"].Ordinal].ToString();
                            strwrk_dept = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["WRK_DEPTID"].Ordinal];
                            strwrk_dept_name = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["WRK_DEPT_NAME"].Ordinal];
                            strBk_Fix_Id = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["BK_FIX_ID"].Ordinal].ToString();
                            strBk_Fix_Code = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Bk_Fix_Code"].Ordinal];
                            //strScada_ID=(string)dtIntBase.Select(" item_id='"+ intItlDataKey +"'")[0].ItemArray[dtIntBase.Columns["SCADA_ID"].Ordinal ];
                            //string strAlarm_Time="";
                            strAlarm_Ack = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Alarm_Ack"].Ordinal].ToString();
                            strAlarm_Type = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Alarm_Type"].Ordinal].ToString();
                            //strrpt_dept=(string)dtIntBase.Select(" item_id='"+ intItlDataKey +"'")[0].ItemArray[dtIntBase.Columns["rpt_dept"].Ordinal ];
                            strequ_id = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["equ_id"].Ordinal].ToString();
                            strequ_code = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["EQU_CODE"].Ordinal];
                            strequ_name = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["EQU_NAME"].Ordinal];
                            strbk_fix_name = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["bk_fix_name"].Ordinal];
                            strITEM_NO = (string)dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["ITEM_NO"].Ordinal];

                            //20181025 add start
                            strAlarmSec = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Alarm_Sec"].Ordinal].ToString();
                            strAlarmNum = dtIntBase.Select(" item_id='" + intItlDataKey + "'")[0].ItemArray[dtIntBase.Columns["Alarm_Num"].Ordinal].ToString();
                          
                            if (strAlarmSec != "0" || strAlarmNum != "0")
                            {
                                this.OnDisplayStringMessage("sec+num Alarm " + strAlarmSec + "  " + strAlarmNum);
                                if (strAlarm_Ack == "1")  //2005/12/23 ken_jean,�۰ʳ���,�����N�T�{Alarm_Mark�P����Ack_Flag flag=1,�M���٭n�g�JRPT_SCADA
                                {
                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTableTemp +
                                        "       (       TimeRcv,             Source,             Content,                Alarm_Level,                  RPT_DEPT_CODE,          WRK_DEPT_NAME,           BK_FIX_ID,            BK_FIX_CODE,      Alarm_Time,        Alarm_Ack,             Ack_Flag ,           Alarm_Type,            EQU_ID,            EQU_CODE,            EQU_NAME,            ITEM_ID,           BK_FIX_NAME,           ITEM_NO,            Alarm_Sec,             Alarm_Num,   mark) " +
                                        "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' , GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "','" + strAlarmSec + "','" + strAlarmNum + "', '0')";

                                    CmdDB.CommandText = strSQL;
                                    CmdDB.ExecuteNonQuery();

                                    string sSql = "select top 1 seq from " + this.strAlarmDBSQLServer_RXTable + " order by seq Desc";
                                    CmdDB.CommandText = sSql;

                                    daRepair = new SqlDataAdapter(CmdDB);
                                    //daRepair=new SqlDataAdapter(this.strSelectSQL ,conDB); //�γo�椣��...
                                    dsRepair.Clear();
                                    daRepair.Fill(dsRepair);
                                    if (dsRepair.Tables[0].Rows.Count > 0)
                                    {
                                        seqTemp = dsRepair.Tables[0].Rows[0].ItemArray[0].ToString();
                                    }

                                    //(�۰ʳ��ױaSCADA��T)�קK���Ƴ���
                                    strSQL = "if (select count(0) from RPT_SCADA_NEW where BK_FIX_ID='" + strBk_Fix_Id + "' and WS_CODE<>'F000')=0" +
                                        " begin INSERT INTO RPT_SCADA_NEW (SCADA_ID,RPT_FRM_CODE,RPT_DATE,RPT_TIME,BK_DWN_DATE,BK_DWN_TIME," +
                                        " RPT_DEPT_CODE,RPT_EMP_CODE,EQU_ID,BK_FIX_ID,BK_COND_DESC,RUN_STAT_CODE,WS_CODE) " +
                                        //  " values ('"+seqTemp+"',"+seqTemp+",'','','','"+strTimeRcv.Substring(0,10).Replace(" ","").Replace("/","")+"','"+strTimeRcv.Substring(11).Replace(" ","").Replace(":","")+"',"+
                                        " values ( cast(cast(convert(char(8),getdate(),112) as bigint) * 100000 + cast(Right(cast(" + seqTemp + " as varchar),5) as int)  as char(13) )  ,'','','','" + strTimeRcv.Substring(0, 10).Replace(" ", "").Replace("/", "") + "','" + strTimeRcv.Substring(11).Replace(" ", "").Replace(":", "") + "'," +
                                        "'IDC','scada','" + strequ_id + "','" + strBk_Fix_Id + "',substring('" + strContent + "',1,200),'','')" +
                                        " end";
                                }
                                else  //�S���۰ʳ���
                                {
                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTableTemp +
                                         "       (        TimeRcv,              Source,               Content,                  Alarm_Level,            RPT_DEPT_CODE,          WRK_DEPT_NAME,              BK_FIX_ID,              BK_FIX_CODE,      Alarm_Time,          Alarm_Ack,             Alarm_Type,              EQU_ID,              EQU_CODE,              EQU_NAME,              ITEM_ID,             BK_FIX_NAME,             ITEM_NO,             Alarm_Sec,             Alarm_Num,    mark) " +
                                         "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' , GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "','" + strAlarmSec + "','" + strAlarmNum + "',  '0')";
                                }

                                this.OnDisplayStringMessage("�g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP + ") " + this.strAlarmDBSQLServer_RXTableTemp + " ��Ʈw: ��Ƶ��Ʀ@ " + intCount + " ��");
                                CmdDB.CommandText = strSQL;
                                CmdDB.ExecuteNonQuery();

                                strSQL = "EXEC dbo.AlarmList '" + strItem_Id + "','" + strAlarmNum + "','" + strAlarmSec + "'";
                                CmdDB.CommandText = strSQL;
                                CmdDB.ExecuteNonQuery();
                            }
                            else  //��B�z�{��
                            {
                                if (strAlarm_Ack == "1")  //2005/12/23 ken_jean,�۰ʳ���,�����N�T�{Alarm_Mark�P����Ack_Flag flag=1,�M���٭n�g�JRPT_SCADA
                                {
                                    this.OnDisplayStringMessage("Alarm");
                                    //��insert��AlarmList,�]���n���L��seq
                                    //jessica 2006/6/20 start
                                    //                        strSQL="INSERT INTO AlarmList " +
                                    //                           "       (     ITEM_NO,              ITEM_ID,            TimeRcv,                 Source,             Content,                  Alarm_Level,            BK_FIX_ID,            BK_FIX_CODE,      Alarm_Time,        Alarm_Ack,            Alarm_Type,         RPT_DEPT_CODE,         EQU_ID,            BK_COND_DES,Alarm_Mark,Ack_Flag) " +
                                    //                           "VALUES ('"+ strITEM_NO +"' ,'"+ strItem_Id +"', '"+ strTimeRcv     +"', '"+  strSource +"',  '"+ strContent +"'       , "+ strAlarm_level +" , '"+ strBk_Fix_Id +"', '"+ strBk_Fix_Code +"' , GetDate() , '"+ strAlarm_Ack +"', '"+ strAlarm_Type +"' ,'"+ strrpt_dept +"','"+ strequ_id +"' ,'"+ strbk_fix_name +"','0','1')";
                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTable +
                                        "       (       TimeRcv,             Source,             Content,                Alarm_Level,        RPT_DEPT_CODE,          WRK_DEPT_NAME,           BK_FIX_ID,            BK_FIX_CODE,      Alarm_Time,        Alarm_Ack,             Ack_Flag ,           Alarm_Type,            EQU_ID,            EQU_CODE,            EQU_NAME,            ITEM_ID,           BK_FIX_NAME,           ITEM_NO  ) " +
                                        "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' , GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "')";

                                    //jessica 2006/6/20 end

                                    CmdDB.CommandText = strSQL;
                                    CmdDB.ExecuteNonQuery();

                                    string sSql = "select top 1 seq from " + this.strAlarmDBSQLServer_RXTable + " order by seq Desc";
                                    CmdDB.CommandText = sSql;

                                    daRepair = new SqlDataAdapter(CmdDB);
                                    //daRepair=new SqlDataAdapter(this.strSelectSQL ,conDB); //�γo�椣��...
                                    dsRepair.Clear();
                                    daRepair.Fill(dsRepair);
                                    if (dsRepair.Tables[0].Rows.Count > 0)
                                    {
                                        seqTemp = dsRepair.Tables[0].Rows[0].ItemArray[0].ToString();
                                    }

                                    //(�۰ʳ��ױaSCADA��T)�קK���Ƴ���
                                    strSQL = "if (select count(0) from RPT_SCADA_NEW where BK_FIX_ID='" + strBk_Fix_Id + "' and WS_CODE<>'F000')=0" +
                                        " begin INSERT INTO RPT_SCADA_NEW (SCADA_ID,RPT_FRM_CODE,RPT_DATE,RPT_TIME,BK_DWN_DATE,BK_DWN_TIME," +
                                        " RPT_DEPT_CODE,RPT_EMP_CODE,EQU_ID,BK_FIX_ID,BK_COND_DESC,RUN_STAT_CODE,WS_CODE) " +
                                        //  " values ('"+seqTemp+"',"+seqTemp+",'','','','"+strTimeRcv.Substring(0,10).Replace(" ","").Replace("/","")+"','"+strTimeRcv.Substring(11).Replace(" ","").Replace(":","")+"',"+
                                        " values ( cast(cast(convert(char(8),getdate(),112) as bigint) * 100000 + cast(Right(cast(" + seqTemp + " as varchar),5) as int)  as char(13) )  ,'','','','" + strTimeRcv.Substring(0, 10).Replace(" ", "").Replace("/", "") + "','" + strTimeRcv.Substring(11).Replace(" ", "").Replace(":", "") + "'," +
                                        "'IDC','scada','" + strequ_id + "','" + strBk_Fix_Id + "',substring('" + strContent + "',1,200),'','')" +
                                        " end";
                                }
                                else  //�S���۰ʳ���
                                {
                                    //strSQL="INSERT INTO "+ this.strAlarmDBSQLServer_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";
                                    // jessica 2006/6/20 start
                                    //  strSQL="INSERT INTO AlarmList " +
                                    // "       (     ITEM_NO,              ITEM_ID,            TimeRcv,                 Source,             Content,                  Alarm_Level,            BK_FIX_ID,            BK_FIX_CODE,      Alarm_Time,        Alarm_Ack,            Alarm_Type,         RPT_DEPT_CODE,         EQU_ID,            BK_COND_DES) " +
                                    // "VALUES ('"+ strITEM_NO +"' ,'"+ strItem_Id +"', '"+ strTimeRcv     +"', '"+  strSource +"',  '"+ strContent +"'       , "+ strAlarm_level +" , '"+ strBk_Fix_Id +"', '"+ strBk_Fix_Code +"' , GetDate() , '"+ strAlarm_Ack +"', '"+ strAlarm_Type +"' ,'"+ strrpt_dept +"','"+ strequ_id +"' ,'"+ strbk_fix_name +"')";

                                    strSQL = "INSERT INTO " + this.strAlarmDBSQLServer_RXTable +
                                        "       (       TimeRcv,             Source,             Content,                Alarm_Level,        RPT_DEPT_CODE,          WRK_DEPT_NAME,            BK_FIX_ID,            BK_FIX_CODE,         Alarm_Time,        Alarm_Ack,            Alarm_Type,            EQU_ID,            EQU_CODE,            EQU_NAME,            ITEM_ID,           BK_FIX_NAME,           ITEM_NO  ) " +
                                        "VALUES ('" + strTimeRcv + "', '" + strSource + "' , '" + strContent + "'  ,    " + strAlarm_level + " ,'" + strwrk_dept + "', '" + strwrk_dept_name + "', '" + strBk_Fix_Id + "', '" + strBk_Fix_Code + "' ,    GetDate() , '" + strAlarm_Ack + "', '" + strAlarm_Type + "' ,'" + strequ_id + "' ,'" + strequ_code + "' ,'" + strequ_name + "' ,'" + strItem_Id + "','" + strbk_fix_name + "','" + strITEM_NO + "')";

                                    // jessica 2006/6/20 end

                                }
                                this.OnDisplayStringMessage("�g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP + ") " + this.strAlarmDBSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + intCount + " ��");
                                CmdDB.CommandText = strSQL;
                                CmdDB.ExecuteNonQuery();
                            }
                        }
                        //20181025 add end
                    } //End for vdata
                    conDB.Close();
                    //this.OnDisplayStringMessage("�g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP  + ") "+ this.strAlarmDBSQLServer_RXTable +" ��Ʈw: ��Ƶ��Ʀ@ "+ intCount +" ��" );
                }
            }
            catch (Exception Ex)
            {
                
                this.OnDisplayStringMessage(Ex.ToString());
                this.OnDisplayStringMessage("catch �g�JAlarmDB  ��m(" + this.strAlarmDBSQLServer_IP + ") " + this.strAlarmDBSQLServer_RXTable + " ��Ʈw �@�~���`: �����P�_��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ���A�P�_���߹w�g�J�� " + intCount + " ���ɵo�Ϳ��~");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }
        }


        //�ƻs�iMainDB
        void fnSetMainDBMoveData(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            int i;


            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strMainDBSQLServer_IP + ";uid=" + this.strMainDBSQLServer_User + ";pwd=" + this.strMainDBSQLServer_Password + ";database=" + this.strMainDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JMainDB     ��m(" + this.strMainDBSQLServer_IP + ") " + this.strMainDBSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;


                    DateTime DateTimeRcv;
                    string strTimeRcv;

                    string strContent = "";

                    for (i = 0; i < vdtData.Rows.Count; i++)
                    {

                        DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                        strTimeRcv = DateTimeRcv.Year.ToString("0000") + "/" + DateTimeRcv.Month.ToString("00") + "/" + DateTimeRcv.Day.ToString("00") + " " + DateTimeRcv.Hour.ToString("00") + ":" + DateTimeRcv.Minute.ToString("00") + ":" + DateTimeRcv.Second.ToString("00");

                        //strSQL="INSERT INTO "+ this.strMainDBSQLServer_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";


                        strContent = (string)vdtData.Rows[i]["Content"];
                        strContent = strContent.Replace("'", "''");

                        // strSQL="INSERT INTO "+ this.strMainDBSQLServer_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ strTimeRcv +"', '"+ vdtData.Rows[i]["Content"] +"')";
                        strSQL = "INSERT INTO " + this.strMainDBSQLServer_RXTable + " (seq, Source, TimeRcv, Content) VALUES ('" + vdtData.Rows[i]["seq"] + "', '" + vdtData.Rows[i]["Source"] + "', '" + strTimeRcv + "', '" + (string)strContent + "')";
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JMainDB  ��m(" + this.strMainDBSQLServer_IP + ") " + this.strMainDBSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JMainDB  ��m(" + this.strMainDBSQLServer_IP + ") " + this.strMainDBSQLServer_RXTable + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }


        //�ƻs�iMainDB (One day) Jacky_su 98/05/04 add
        void fnSetMainDBMoveDataOneDay(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;


            try
            {
                lock (this)
                {


                    string strTableName;

                    // Ū��Table�W��
                    strTableName = Profile.GetValue("ScadaDBMF.ini", "MainDBSQLServerOneDay", "RXTable");


                    strConn = @"server=" + this.m_strMainDBSQLServer_OneDay_IP + ";uid=" + this.m_strMainDBSQLServer_OneDay_User + ";pwd=" + this.m_strMainDBSQLServer_OneDay_Password + ";database=" + this.m_strMainDBSQLServer_OneDay_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JMainDB     ��m(" + this.strMainDBSQLServer_IP + ") " + this.m_strMainDBSQLServer_OneDay_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;



                    DateTime DateTimeRcv;
                    string strTimeRcv;
                    string strTemp;
                    string strTable_ExtName = "";
                    string strContent;

                    for (int i = 0; i < vdtData.Rows.Count; i++)
                    {

                        DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                        //strTimeRcv = DateTimeRcv.Year.ToString("0000") + "/" + DateTimeRcv.Month.ToString("00") + "/" + DateTimeRcv.Day.ToString("00") + " " + DateTimeRcv.Hour.ToString("00") + ":" + DateTimeRcv.Minute.ToString("00") + ":" + DateTimeRcv.Second.ToString("00");
                        strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");

                        strTemp = fnGetDayStr(strTimeRcv);

                        // ���O�_�n�إ߷s��Table���x�s��� (�HTimeRcv���W������ӨM�w �x�s����ƪ��W��)
                        if (strTemp != null && m_strMainDBSQLServer_OneDay_RXTable != strTableName + strTemp)
                        {
                            // �إ߷s��Table���x�s���

                            strTable_ExtName = strTemp;

                            // �]�w�s��Table�W��
                            m_strMainDBSQLServer_OneDay_RXTable = strTableName + strTable_ExtName;


                            this.OnDisplayStringMessage("�ƥ����P���e��ƪ�������P�A�۰ʫإ�(Create)�s��ƪ�" + m_strMainDBSQLServer_OneDay_RXTable);


                            strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                            strSQL = strSQL + m_strMainDBSQLServer_OneDay_RXTable;
                            strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                            strSQL = strSQL + " CREATE TABLE [dbo].[";
                            strSQL = strSQL + m_strMainDBSQLServer_OneDay_RXTable;
                            strSQL = strSQL + "] (";
                            strSQL = strSQL + "[Seq] [bigint] PRIMARY KEY , ";
                            strSQL = strSQL + "[TimeRcv] [datetime] , ";
                            strSQL = strSQL + "[Source] [varchar] (50) , ";
                            strSQL = strSQL + "[Content] [varchar] (2000) ";
                            strSQL = strSQL + ")";

                            CmdDB.CommandText = strSQL; //����إ߷s��ƪ����O
                            CmdDB.ExecuteNonQuery();

                            this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        }



                        strContent = (string)vdtData.Rows[i]["Content"];
                        strContent = strContent.Replace("'", "''");



                        // strSQL="INSERT INTO "+ this.m_strMainDBSQLServer_OneDay_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";
                        strSQL = "INSERT INTO " + this.m_strMainDBSQLServer_OneDay_RXTable + " (seq, Source, TimeRcv, Content) VALUES ('" + vdtData.Rows[i]["seq"] + "', '" + vdtData.Rows[i]["Source"] + "', '" + strTimeRcv.ToString() + "', '" + (string)strContent + "')";

                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JMainDB  ��m(" + this.m_strMainDBSQLServer_OneDay_IP + ") " + this.m_strMainDBSQLServer_OneDay_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JMainDB  ��m(" + this.m_strMainDBSQLServer_OneDay_IP + ") " + this.m_strMainDBSQLServer_OneDay_RXTable + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }


        //�ƻs�iRTDB
        void fnSetRTDBMoveData(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            string strContent;

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strLocalSQLServer_IP + ";uid=" + this.strLocalSQLServer_User + ";pwd=" + this.strLocalSQLServer_Password + ";database=" + this.strLocalSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JRTDB     ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;




                    DateTime DateTimeRcv;

                    for (int i = 0; i < vdtData.Rows.Count; i++)
                    {

                        DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                        strContent = (string)vdtData.Rows[i]["Content"];
                        strContent = strContent.Replace("'", "''");

                        //strSQL="INSERT INTO "+ this.strLocalSQLServer_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";
                        strSQL = "INSERT INTO " + this.strLocalSQLServer_RXTable + " (seq, Source, TimeRcv, Content) VALUES ('" + vdtData.Rows[i]["seq"] + "', '" + vdtData.Rows[i]["Source"] + "', '" + DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + (string)strContent + "')";
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        //�ƻs�iRTDB (One day) Jacky_su 980504 add
        void fnSetRTDBMoveDataOneDay(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;

            try
            {
                lock (this)
                {
                    // Ū��Table�W��
                    string strTableName = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "RXTable");

                    strConn = @"server=" + this.m_strLocalSQLServer_OneDay_IP + ";uid=" + this.m_strLocalSQLServer_OneDay_User + ";pwd=" + this.m_strLocalSQLServer_OneDay_Password + ";database=" + this.m_strLocalSQLServer_OneDay_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JRTDB     ��m(" + this.m_strLocalSQLServer_OneDay_IP + ") " + this.m_strLocalSQLServer_OneDay_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;


                    DateTime DateTimeRcv;
                    string strTimeRcv;
                    string strTable_ExtName = "";
                    string strTemp;
                    string strContent;

                    for (int i = 0; i < vdtData.Rows.Count; i++)
                    {

                        DateTimeRcv = System.DateTime.Parse(vdtData.Rows[i]["TimeRcv"].ToString());

                        // strTimeRcv = DateTimeRcv.Year.ToString("0000") + "/" + DateTimeRcv.Month.ToString("00") + "/" + DateTimeRcv.Day.ToString("00") + " " + DateTimeRcv.Hour.ToString("00") + ":" + DateTimeRcv.Minute.ToString("00") + ":" + DateTimeRcv.Second.ToString("00");
                        strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");

                        strTemp = fnGetDayStr(strTimeRcv);

                        // ���O�_�n�إ߷s��Table���x�s��� (�HTimeRcv���W������ӨM�w �x�s����ƪ��W��)
                        if (strTemp != null && m_strLocalSQLServer_OneDay_RXTable != strTableName + strTemp)
                        {
                            // �إ߷s��Table���x�s���

                            strTable_ExtName = strTemp;

                            // �]�w�s��Table�W��
                            m_strLocalSQLServer_OneDay_RXTable = strTableName + strTable_ExtName;


                            this.OnDisplayStringMessage("�ƥ����P���e��ƪ�������P�A�۰ʫإ�(Create)�s��ƪ�" + m_strLocalSQLServer_OneDay_RXTable);

                            strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                            strSQL = strSQL + m_strLocalSQLServer_OneDay_RXTable;
                            strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                            strSQL = strSQL + " CREATE TABLE [dbo].[";
                            strSQL = strSQL + m_strLocalSQLServer_OneDay_RXTable;
                            strSQL = strSQL + "] (";
                            strSQL = strSQL + "[Seq] [bigint] PRIMARY KEY , ";
                            strSQL = strSQL + "[TimeRcv] [datetime] , ";
                            strSQL = strSQL + "[Source] [varchar] (50) , ";
                            strSQL = strSQL + "[Content] [varchar] (2000) ";
                            strSQL = strSQL + ")";

                            CmdDB.CommandText = strSQL; //����إ߷s��ƪ����O
                            CmdDB.ExecuteNonQuery();

                            this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        }


                        strContent = (string)vdtData.Rows[i]["Content"];
                        strContent = strContent.Replace("'", "''");


                        // strSQL="INSERT INTO "+ this.m_strLocalSQLServer_OneDay_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";
                        strSQL = "INSERT INTO " + this.m_strLocalSQLServer_OneDay_RXTable + " (seq, Source, TimeRcv, Content) VALUES ('" + vdtData.Rows[i]["seq"] + "', '" + vdtData.Rows[i]["Source"] + "', '" + strTimeRcv.ToString() + "', '" + (string)strContent + "')";
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.m_strLocalSQLServer_OneDay_IP + ") " + this.m_strLocalSQLServer_OneDay_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.m_strLocalSQLServer_OneDay_IP + ") " + this.m_strLocalSQLServer_OneDay_RXTable + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        //�ƻs�iPSD DB
        void fnSetPSDEventDBMoveData(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            string strContent;

            //20121114 SFI �P�_�O�_��PSD�����
            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]PSDLog]%'";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strPSDEventDBSQLServer_IP + ";uid=" + this.strPSDEventDBSQLServer_User + ";pwd=" + this.strPSDEventDBSQLServer_Password + ";database=" + this.strPSDEventDBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();


                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JPSD DB    ��m(" + this.strPSDEventDBSQLServer_IP + ") " + this.strPSDEventDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;




                    DateTime DateTimeRcv;

                    for (int i = 0; i < dvData.Count; i++)
                    {

                        DateTimeRcv = System.DateTime.Parse(dvData[i].Row["TimeRcv"].ToString());

                        strContent = (string)dvData[i].Row["Content"];
                        strContent = strContent.Replace("'", "''");

                        //strSQL="INSERT INTO "+ this.strLocalSQLServer_RXTable  +" (seq, Source, TimeRcv, Content) VALUES ('"+ vdtData.Rows[i]["seq"] +"', '"+ vdtData.Rows[i]["Source"] +"', '"+ vdtData.Rows[i]["TimeRcv"] +"', '"+ vdtData.Rows[i]["Content"] +"')";
                        strSQL = "INSERT INTO " + this.strPSDEventDBSQLServer_Table + " (seq, Source, TimeRcv, Content) VALUES ('" + dvData[i].Row["seq"] + "', '" + dvData[i].Row["Source"] + "', '" + DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + (string)strContent + "')";
                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();
                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JPSD DB    ��m(" + this.strPSDEventDBSQLServer_IP + ") " + this.strPSDEventDBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JPSD DB    ��m(" + this.strPSDEventDBSQLServer_IP + ") " + this.strPSDEventDBSQLServer_Table + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        //�ƻs�iEMDS CountDown DB
        void fnSetEMDSCountDownDBMoveData(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            string strContent;

            //20121114 SFI �P�_�O�_��PSD�����
            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]EMDSCountDown]%'";

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strEMDSCountDown_DBSQLServer_IP + ";uid=" + this.strEMDSCountDown_DBSQLServer_User + ";pwd=" + this.strEMDSCountDown_DBSQLServer_Password + ";database=" + this.strEMDSCountDown_DBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();


                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JEMDSCountDown DB    ��m(" + this.strEMDSCountDown_DBSQLServer_IP + ") " + this.strEMDSCountDown_DBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + vdtData.Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;




                    DateTime DateTimeRcv;

                    string[] msg;
                    string Station = "";
                    string Platform = "";
                    string CountDown = "";
                    string Destination = "";
                    string UpdateTime = "";
                    SortedList list = new SortedList();


                    for (int i = 0; i < dvData.Count; i++)
                    {

                        msg = dvData[i].Row["Content"].ToString().Split(';');

                        #region �r�����
                        foreach (string m in msg)
                        {
                            string[] tmp;
                            tmp = m.Split('=');

                            //���� [EMDSCountDown] �o�ӫe�m�Ÿ�
                            int pos = 0;
                            pos = tmp[0].IndexOf("EMDSCountDown]");
                            if (pos > 0)
                            {
                                tmp[0] = tmp[0].Remove(0, "[EMDSCountDown]".Length);
                            }


                            switch (tmp[0])
                            {
                                case "Station":
                                    Station = tmp[1];
                                    break;
                                case "Platform":
                                    Platform = tmp[1];
                                    break;
                                case "Countdown":
                                    CountDown = tmp[1];
                                    break;
                                case "Destination":
                                    Destination = tmp[1];
                                    break;
                                case "UpdateTime":
                                    UpdateTime = tmp[1];
                                    break;
                                default:
                                    break;
                            }

                        }
                        #endregion
                        strSQL = string.Format("Update {0} set CountDown ='{1}' , UpdateTime = '{2}' where Station='{3}' and Destination = '{4}' ", strEMDSCountDown_DBSQLServer_Table, CountDown, UpdateTime, Station, Destination);

                        int rs = 0;
                        CmdDB.CommandText = strSQL;

                        rs = CmdDB.ExecuteNonQuery();

                        if (rs == 0)
                        {
                            strSQL = string.Format(@"insert into {0} (station,platform,Destination,CountDown,UpdateTime,CreateTime) values ('{1}' , '{2}', '{3}','{4}','{5}',getdate()) ", strEMDSCountDown_DBSQLServer_Table, Station, Platform, Destination, CountDown, UpdateTime);
                            CmdDB.CommandText = strSQL;
                            CmdDB.ExecuteNonQuery();
                        }

                    }

                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JEMDSCountDown DB    ��m(" + this.strEMDSCountDown_DBSQLServer_IP + ") " + this.strEMDSCountDown_DBSQLServer_Table + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�g�JEMDSCountDown DB    ��m(" + this.strEMDSCountDown_DBSQLServer_IP + ") " + this.strEMDSCountDown_DBSQLServer_Table + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.Count + " ��");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }



        #region �P�_�O�_�n�i�J���zDB


        //�P�_�p���X���S���n��
        int fnCheckContent(string vstrSource, string vstrContent)
        {
            //			bool btnSaveData;
            //			btnSaveData=false;
            int intSaveDataKey = -1;

            string strNotflag;
            int intKwType;

            ////  test area declation 

            
            //string strA;
            //string strB;
            //string strC;
            //string strD;

            ////




            DataView myDataView = new DataView(this.dtIntBase);
            //myDataView.RowFilter="Equ_Code='"+ vstrSource.Trim() +"'";		//�P�_�O����line
            myDataView.RowFilter = "sys_no='" + vstrSource.Trim() + "'";		//�P�_�O����line
            myDataView.Sort = "ITEM_LEVEL";									//Pattern��Ϊ�����

            for (int i = 0; i < myDataView.Count; i++)
            {
                intKwType = Convert.ToInt32(myDataView[i].Row["KEYWORD_TYPE"]);

                //if (intKwType<=0) continue;

                strA = myDataView[i].Row["ITEM_NO01"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO01"];
                strB = myDataView[i].Row["ITEM_NO02"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO02"];
                strC = myDataView[i].Row["ITEM_NO03"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO03"];
                strD = myDataView[i].Row["ITEM_NO04"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO04"];

                strNotflag = myDataView[i].Row["Notflag"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["Notflag"];
              
                //btnSaveData=fnCheckPattern(intKwType,vstrContent, strA,strB,strC,strD);
                //fnCheckPattern(9,"dhflshfhsflsahlfhsl ESP,== 100sdfssd", "ESP","99","888","");
                //if (fnCheckPattern(intKwType,vstrContent, strA,strB,strC,strD))

                // �Y���O�ϥέ�ب�L�o����
                if (intKwType > 10)
                {
                    if (m_IntFilterType.fnAddDefineCheckPattern(intKwType, vstrContent, strA, strB, strC, strD,strNotflag) == true)
                    {
                        return Convert.ToInt32(myDataView[i].Row["item_id"]);
                    }
                }
                else
                {
                    // �ϥέ�ب�L�o����
                    if (IntelligentMethod.Method.fnCheckPattern(intKwType, vstrContent, strA, strB, strC, strD))
                    {
                        return Convert.ToInt32(myDataView[i].Row["item_id"]);
                    }
                }

                //if (IntelligentMethod.Method.fnCheckPattern(intKwType,vstrContent, strA,strB,strC,strD))
                //{return Convert.ToInt32(myDataView[i].Row["item_id"]);}
            }

            //myDataView[0].Row["seq_id"]

            return intSaveDataKey;
        }

        //�h��ĵ�T�P�_
        string MultipleCheckContent(string vstrSource, string vstrContent)
        {
            string intSaveDataKey = "";

            DataView Multiple = new DataView(this.dtMultipleAlarm);
            Multiple.RowFilter = "sys_no='" + vstrSource.Trim() + "'";
            Multiple.Sort = "ITEM_LEVEL";

            MultipleStr = "";
            //�h������r�ƶq
            if (Multiple.Count > 0)
            {
                for (int Mu = 0; Mu < Multiple.Count; Mu++)
                { 
                    string ITEM_NO = "";

                    for (int no = 1; no < 21; no++)
                    {
                        ITEM_NO = Multiple[Mu].Row["ITEM_NO" + Convert.ToString(no).PadLeft(2, '0')] == System.DBNull.Value ?
                            string.Empty : (string)Multiple[Mu].Row["ITEM_NO" + Convert.ToString(no).PadLeft(2, '0')];
                        if (ITEM_NO != string.Empty)
                        {
                            if (this.DBG_ReceiveData.Checked)
                            {
                                this.DisplayStringMessage("Content[" + vstrContent + "] " +
                                                  "ITEM_NO" + (no).ToString().PadLeft(2, '0') +
                                                  "[" + ITEM_NO + "]" + " " + Mu + " bool[" +
                                (IntelligentMethod.Method.fnCheckMultiple(vstrContent, ITEM_NO)).ToString() + "]");
                            }
                            
                            if (IntelligentMethod.Method.fnCheckMultiple(vstrContent, ITEM_NO) == true & ITEM_NO != "")
                            {
                                intSaveDataKey += Multiple[Mu].Row["item_id"].ToString() + "|";
                                MultipleStr = ITEM_NO;
                                break;
                            }
                        }
                    }
                }
            }
            return intSaveDataKey;
        }

        #region ����IntelligentMethod

        //�P�_�p���X���C�@����Ʀ��S���n��
        bool fnCheckPattern(int vintKwType, string vstrContent, string vstrA, string vstrB, string vstrC, string vstrD)
        {
            bool btnReturn;
            btnReturn = false;

            switch (vintKwType)
            {
                case 1:				//1:A
                    btnReturn = fnPattern1(vstrContent, vstrA);
                    break;

                case 2:				//2:A && B
                    btnReturn = fnPattern2(vstrContent, vstrA, vstrB);
                    break;

                case 3:				//3:A || B
                    btnReturn = fnPattern3(vstrContent, vstrA, vstrB);
                    break;

                case 4:				//4:(A&&B)||C
                    btnReturn = fnPattern4(vstrContent, vstrA, vstrB, vstrC);
                    break;

                case 5:				//5:A && B &&C
                    btnReturn = fnPattern5(vstrContent, vstrA, vstrB, vstrC);
                    break;

                case 6:				//6:A || B || C
                    btnReturn = fnPattern6(vstrContent, vstrA, vstrB, vstrC);
                    break;

                case 7:				//7:(A&&B)||(C&&D)
                    btnReturn = fnPattern7(vstrContent, vstrA, vstrB, vstrC, vstrD);
                    break;

                case 8:				//8: X >= Value  A=key, B=Value
                    if (vstrA.Length != 0 && vstrB.Length != 0)
                    { btnReturn = fnPattern8(vstrContent, vstrA, vstrB); }
                    break;

                case 9:				//9: X < Value  A=key, B=Value
                    if (vstrA.Length != 0 && vstrB.Length != 0)
                    { btnReturn = fnPattern9(vstrContent, vstrA, vstrB); }
                    break;

                case 10:			//10: Value1 < X < Value2  A=key, B=Value1 , C=Value2
                    if (vstrA.Length != 0 && vstrB.Length != 0 && vstrC.Length != 0)
                    { btnReturn = fnPattern10(vstrContent, vstrA, vstrB, vstrC); }
                    break;
            }

            return btnReturn;

        }


        //�Q��key ��control�A�̬Y�W�h���ovalue string
        string fnGetKeyValue(string vstrContent, string vstrKey)
        {
            string strKey = vstrKey;
            string strContent = vstrContent;
            string strValue = "";
            //int intReturn=-1;
            bool blnEndChar = false;
            bool blnStartChar = false;

            int intKeyPos = strContent.IndexOf(strKey);
            int intTmp;

            //�T�{�Ӧr�ꤤ�O�_��key
            if (intKeyPos >= 0)
            {

                //char[] chrAryTmp=strContent.ToCharArray()
                intTmp = strContent.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }, intKeyPos + strKey.Length);

                //�T�{�Ӧr�ꤤkey����O�_���Ʀr
                if (intTmp >= 0 || intTmp > intKeyPos)
                {

                    char[] chrAryTmp = strContent.ToCharArray(intKeyPos + strKey.Length, strContent.Length - strKey.Length - intKeyPos);
                    for (int i = 0; i < chrAryTmp.Length; i++)
                    {
                        if (blnEndChar && blnStartChar) { break; }
                        if (chrAryTmp[i] == '0' || chrAryTmp[i] == '1' || chrAryTmp[i] == '2' || chrAryTmp[i] == '3' || chrAryTmp[i] == '4' || chrAryTmp[i] == '5' || chrAryTmp[i] == '6' || chrAryTmp[i] == '7' || chrAryTmp[i] == '8' || chrAryTmp[i] == '9')
                        {
                            strValue = strValue + chrAryTmp[i].ToString();
                            blnStartChar = true;
                        }
                        else
                        {
                            if (chrAryTmp[i] == ' ' || chrAryTmp[i] == ',' || chrAryTmp[i] == ';' || chrAryTmp[i] == '=')
                            { if (blnStartChar) { blnEndChar = true; } }
                            else
                            { break; }
                        }
                    }
                }
            }



            return strValue;
            //			string asdf=strValue;
            //			strValue="";
            //			intReturn= strValue.Length == 0 ? -1 : int.Parse(strValue);
            //			return intReturn;
        }



        //10: Value1 < X < Value2  A=key, B=Value1 , C=Value2
        bool fnPattern10(string vstrContent, string vstrA, string vstrB, string vstrC)
        {
            bool btnReturn;
            btnReturn = false;
            string strTmp;

            strTmp = fnGetKeyValue(vstrContent, vstrA);

            if (strTmp.Length != 0)
            {
                if (int.Parse(strTmp) > int.Parse(vstrB) && int.Parse(strTmp) < int.Parse(vstrC))
                    btnReturn = true;
            }

            return btnReturn;
        }

        //9: X < Value  A=key, B=Value
        bool fnPattern9(string vstrContent, string vstrA, string vstrB)
        {
            bool btnReturn;
            btnReturn = false;
            string strTmp;

            strTmp = fnGetKeyValue(vstrContent, vstrA);

            if (strTmp.Length != 0)
            {
                if (int.Parse(strTmp) < int.Parse(vstrB))
                    btnReturn = true;
            }

            return btnReturn;
        }

        //8: X >= Value  A=key, B=Value
        bool fnPattern8(string vstrContent, string vstrA, string vstrB)
        {
            bool btnReturn;
            btnReturn = false;
            string strTmp;

            strTmp = fnGetKeyValue(vstrContent, vstrA);

            if (strTmp.Length != 0)
            {
                if (int.Parse(strTmp) >= int.Parse(vstrB))
                    btnReturn = true;
            }

            return btnReturn;
        }


        bool fnPattern7(string vstrContent, string vstrA, string vstrB, string vstrC, string vstrD)
        {
            bool btnReturn;
            btnReturn = false;

            if ((fnPatterns(vstrContent, vstrA) && fnPatterns(vstrContent, vstrB)) || (fnPatterns(vstrContent, vstrC) && fnPatterns(vstrContent, vstrD))) ;
            {
                btnReturn = true;
            }

            return btnReturn;

        }


        bool fnPattern6(string vstrContent, string vstrA, string vstrB, string vstrC)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA) || fnPatterns(vstrContent, vstrB) || fnPatterns(vstrContent, vstrC))
            {
                btnReturn = true;
            }

            return btnReturn;

        }

        bool fnPattern5(string vstrContent, string vstrA, string vstrB, string vstrC)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA) && fnPatterns(vstrContent, vstrB) && fnPatterns(vstrContent, vstrC))
            {
                btnReturn = true;
            }

            return btnReturn;

        }


        bool fnPattern4(string vstrContent, string vstrA, string vstrB, string vstrC)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA) && fnPatterns(vstrContent, vstrB) || fnPatterns(vstrContent, vstrC))
            {
                btnReturn = true;
            }

            return btnReturn;

        }


        bool fnPattern3(string vstrContent, string vstrA, string vstrB)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA) || fnPatterns(vstrContent, vstrB))
            {
                btnReturn = true;
            }

            return btnReturn;

        }


        bool fnPattern2(string vstrContent, string vstrA, string vstrB)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA) && fnPatterns(vstrContent, vstrB))
            {
                btnReturn = true;
            }

            return btnReturn;

        }



        bool fnPattern1(string vstrContent, string vstrA)
        {
            bool btnReturn;
            btnReturn = false;

            if (fnPatterns(vstrContent, vstrA))
            {
                btnReturn = true;
            }


            return btnReturn;
        }


        bool fnPatterns(string vstrContent, string vstr)
        {
            bool btnReturn;

            if (vstrContent.IndexOf(vstr) >= 0)
            { btnReturn = true; }
            else
            { btnReturn = false; }

            return btnReturn;
        }



        #endregion



        #endregion

        //�]�wOv_Line
        void fnSetOvLine()
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;

            try
            {
                lock (this)
                {

                    strConn = @"server=" + this.strLocalSQLServer_IP + ";uid=" + this.strLocalSQLServer_User + ";pwd=" + this.strLocalSQLServer_Password + ";database=" + this.strLocalSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n��sOv_Line     ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_Ov_LineTable + " ��Ʈw�CLine+Code = " + this.strLine_Code);
                    }


                    strSQL = "UPDATE " + strLocalSQLServer_Ov_LineTable + " SET CurCnt = IniCnt, UpdatedTime= Getdate() WHERE (Line_Code = '" + this.strLine_Code + "')";
                    CmdDB.CommandText = strSQL;
                    CmdDB.ExecuteNonQuery();


                    conDB.Close();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("��sOv_Line     ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_Ov_LineTable + " ��Ʈw�����CLine_Code = " + this.strLine_Code);
                    }
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��sOv_Line    ��m(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_Ov_LineTable + " ��Ʈw �@�~���`�CLine+Code = " + this.strLine_Code);
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }


        //�qContent����������sortlist����NetDCDB�ϥ�
        SortedList fnGetArrayValuebyIndex(string vstrContent)
        {

            SortedList sltReturn = new SortedList();
            string[] strAryTmp = (vstrContent+";").Split(new Char[] { ';' });

            string strKey = "";
            string strValue = "";

            for (int i = 0; i < strAryTmp.Length-1; i++)
            {

                if ((strAryTmp[i].Length < 4)|| (strAryTmp[i].Length >= 4 && strAryTmp[i].Substring(0, 4).ToString() != "Msg="))
                {
                    string strtemp = strAryTmp[i].ToString();
                    strKey = strAryTmp[i].Split(new Char[] { '=' })[0];
                    strValue = strAryTmp[i].Split(new Char[] { '=' }).Length > 1 ? strAryTmp[i].Split(new Char[] { '=' })[1] : "";
                    sltReturn.Add(strKey, strValue);
                }
                else
                {
                    strKey = "Msg";
                    strValue = strAryTmp[i].ToString().Replace("Msg=", "");
                    sltReturn.Add(strKey, strValue);
                }

            }
            sltReturn.Remove("[OpcA]");
            return sltReturn;

        }


        //���o�h��ĵ�T����r 20181030 add
        void fnGetMultipleAlarm()
        {
            string strSQL;
            DataSet dsTmp;
            SqlDataAdapter da;
            string strConn;
            try
            {
                lock (this)
                {
                    strSQL = "select * from V_ScadaMultipleAlarm where keyword_type > 0 order by sys_no";
                    strConn = @"server=" + this.strItlDBSQLServer_IP + ";uid=" + this.strItlDBSQLServer_User + ";pwd=" + this.strItlDBSQLServer_Password + ";database=" + this.strItlDBSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    dsTmp = new DataSet();
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(dsTmp);
                    conG9.Close();

                    dtMultipleAlarm = dsTmp.Tables[0];

                    if (dtMultipleAlarm.Rows.Count <= 0)
                    { throw new Exception("�����o����h��ĵ�T���"); }
                    else
                    {
                        this.OnDisplayStringMessage("���o�h��ĵ�T��� " + dtMultipleAlarm.Rows.Count + " ��");
                    }
                }
            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage(Ex.Message);
                conG9.Close();
                throw Ex;
            }
        }


        //���o���z�P�_�̾�
        void fnGetIntBase()
        {
            string strSQL;
            DataSet dsTmp;
            SqlDataAdapter da;
            string strConn;

            try
            {
                lock (this)
                {

                    //select sys_id, m.seq_id as item_id, sys_no,bk_fix_id,bk_fix_code,bk_fix_name,item_level,keyword_type,
                    //item_no01,item_no02,item_no03,item_no04,item_nm,alarm_level,alarm_type,alarm_ack,rpt_dept
                    //from v_scadasmart
                    //order by sys_no 

                    //modify by jessica 95/6/20 start
                    //strSQL = "select Alarm_Level,wrk_deptid,wrk_dept_name,bk_fix_id,bk_fix_code,sys_id, item_id, sys_no,bk_fix_name,item_level,keyword_type, " +
                    //    "item_no01,item_no02,item_no03,item_no04,item_nm,alarm_level,alarm_type,alarm_ack,rpt_dept,equ_id,equ_code,equ_name,ITEM_NO, " +
                    //    //20181025 add AlarmSec, AlarmNum
                    //    "ISNULL(Alarm_Sec, 0) as Alarm_Sec, ISNULL(Alarm_Num, 0) as Alarm_Num " +
                    //    "from v_scadasmart_New where keyword_type > 0 " 
                    //    "order by sys_no";
                    //modify by jessica 95/6/20 end


                    //modify by peter 109/2/16 start
                    strSQL = "select Alarm_Level,wrk_deptid,wrk_dept_name,bk_fix_id,bk_fix_code,sys_id, item_id, sys_no,bk_fix_name,item_level,keyword_type, " +
                        "item_no01,item_no02,item_no03,item_no04,item_nm,alarm_level,alarm_type,alarm_ack,rpt_dept,equ_id,equ_code,equ_name,ITEM_NO, " +
                        //20181025 add AlarmSec, AlarmNum
                        "ISNULL(Alarm_Sec, 0) as Alarm_Sec, ISNULL(Alarm_Num, 0) as Alarm_Num, Notflag, Notflag_left, Notflag_right " +
                        "from v_scadasmart_New where keyword_type > 0 " +
                        "order by sys_no";
                    //modify by  peter 109/2/16 end


                    strConn = @"server=" + this.strItlDBSQLServer_IP + ";uid=" + this.strItlDBSQLServer_User + ";pwd=" + this.strItlDBSQLServer_Password + ";database=" + this.strItlDBSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    dsTmp = new DataSet();
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(dsTmp);
                    conG9.Close();

                    dtIntBase = dsTmp.Tables[0];

                    if (dsTmp.Tables[0].Rows.Count <= 0)
                    { throw new Exception("�����o���󴼼z�P�_���"); }
                    else
                    {
                        this.OnDisplayStringMessage("���o���z�P�_��� " + dsTmp.Tables[0].Rows.Count + " ��");
                    }

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage(Ex.Message);
                conG9.Close();
                throw Ex;
            }


        }


        //���o�ݭnCopy�����
        DataTable fnGetDBMoveData(decimal decStartSeq, decimal decEndSeq)
        {
            string strSQL;
            DataSet dsTmp;
            SqlDataAdapter da;
            string strConn;


            //�Y�ݭncopy�����ƶW�L�@���̤j�q�A�h�H�̤j�q���ǡC
            //			if(!blnfirstTime)
            //			{
            decEndSeq = decStartSeq + this.intDataQPerGet;
            
            WinSysEvtDBMaxSeq = decEndSeq;
            //				if ((decEndSeq>this.intDataQPerGet ) 
            //					decEndSeq=this.intDataQPerGet;
            //			}
            decStartSeq++;


            try
            {
                lock (this)
                {
                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("fnGetDBMoveData: �����h�� FrontEnd RX ��� Seq �� " + decStartSeq + " �� " + decEndSeq);
                    }

                    //SELECT * FROM  SourceTable WHERE  (seq BETWEEN 3 AND 10) order by seq
                    strSQL = "SELECT * FROM " + this.strSourceSQLServer_RXTable + " WHERE (seq BETWEEN " + decStartSeq + " AND " + decEndSeq + ") order by seq";


                    strConn = @"server=" + this.strSourceSQLServer_IP + ";uid=" + this.strSourceSQLServer_User + ";pwd=" + this.strSourceSQLServer_Password + ";database=" + this.strSourceSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    dsTmp = new DataSet();
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(dsTmp);
                    conG9.Close();

                    return dsTmp.Tables[0];

                }

            }
            catch (Exception Ex)
            {

                this.OnDisplayStringMessage("���o FrontEnd ��  IP(" + this.strSourceSQLServer_IP + ") " + this.strSourceSQLServer_RXTable + " ��Ʈw ��Ƨ@�~���`.");
                conG9.Close();
                throw Ex;

            }
            finally { }
            return null;

        }

        //���oFE�ݪ�DB Schema
        DataTable fnGetSourceDBSchema()
        {
            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 * FROM " + this.strSourceSQLServer_RXTable + " where 1<>1";
                    //					strSQL="SELECT TOP 1 * FROM " + this.strSourceSQLServer_RXTable ;
                    mydataset = new DataSet();


                    strConn = @"server=" + this.strSourceSQLServer_IP + ";uid=" + this.strSourceSQLServer_User + ";pwd=" + this.strSourceSQLServer_Password + ";database=" + this.strSourceSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    return mydataset.Tables[0].Copy();

                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("���o FrontEnd ��  IP(" + this.strSourceSQLServer_IP + ") " + this.strSourceSQLServer_RXTable + " ��Ʈw Schema: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }

            finally { }
            return null;


        }



        //���osource�̤j��seq
        decimal fnGetSourceDBMaxSeq()
        {


            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            decimal decTmp;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 * FROM " + this.strSourceSQLServer_RXTable + " Order by seq desc";
                    mydataset = new DataSet();


                    strConn = @"server=" + this.strSourceSQLServer_IP + ";uid=" + this.strSourceSQLServer_User + ";pwd=" + this.strSourceSQLServer_Password + ";database=" + this.strSourceSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    //���osource�̤j��seq
                    if (mydataset.Tables[0].Rows.Count == 0)
                    {
                        decTmp = 0;
                    }
                    else
                    {
                        decTmp = Convert.ToDecimal(mydataset.Tables[0].Rows[0]["seq"]);

                        //this.dtSourceTables =mydataset.Tables[0].Copy();
                    }
                    if (decTmp < 0)
                    { throw new Exception("seq�Ȥp��0 (seq= " + decTmp.ToString() + ")"); }

                }

                return decTmp;

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("���o FrontEnd ��  IP(" + this.strSourceSQLServer_IP + ") " + this.strSourceSQLServer_RXTable + " ��Ʈw seq �̤j�ȧ@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }
            finally
            {

            }
            return 0;

        }

        //���osource�̤p��seq
        decimal fnGetSourceDBMinSeq()
        {


            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            decimal decTmp;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 * FROM " + this.strSourceSQLServer_RXTable + " Order by seq";
                    mydataset = new DataSet();


                    strConn = @"server=" + this.strSourceSQLServer_IP + ";uid=" + this.strSourceSQLServer_User + ";pwd=" + this.strSourceSQLServer_Password + ";database=" + this.strSourceSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    //���osource�̤p��seq
                    if (mydataset.Tables[0].Rows.Count == 0)
                    {
                        decTmp = 0;
                    }
                    else
                    {
                        decTmp = Convert.ToDecimal(mydataset.Tables[0].Rows[0]["seq"]);

                        //this.dtSourceTables =mydataset.Tables[0].Copy();
                    }
                    if (decTmp < 0)
                    { throw new Exception("seq�Ȥp��0 (seq= " + decTmp.ToString() + ")"); }

                }
                return decTmp;

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("���o FrontEnd ��  IP(" + this.strSourceSQLServer_IP + ") " + this.strSourceSQLServer_RXTable + " ��Ʈw seq �̤j�ȧ@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }

            finally
            {

            }
            return 0;
        }



        //���oLocal�̤j��seq
        decimal fnGetLocalDBMaxSeq()
        {

            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            decimal decTmp = 0;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 seq FROM " + this.strLocalSQLServer_RXTable + "  Order by seq desc";
                    mydataset = new DataSet();
                    strConn = @"server=" + this.strLocalSQLServer_IP + ";uid=" + this.strLocalSQLServer_User + ";pwd=" + this.strLocalSQLServer_Password + ";database=" + this.strLocalSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    //���oLocal�̤j��seq
                    if (mydataset.Tables[0].Rows.Count == 0)
                    {
                        //decTmp = 0;
                        decTmp = 229094990;
                    }
                    else
                    {
                        decTmp = Convert.ToDecimal(mydataset.Tables[0].Rows[0]["seq"]);
                    }

                    if (decTmp < 0)
                    { throw new Exception("seq�Ȥp��0 (seq= " + decTmp.ToString() + ")"); }

                }
                return decTmp;

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("���o Local��  IP(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw seq �̤j�ȧ@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }

            finally
            {

            }
            return 0;

        }

        //���oLocal�̤j��seq (check RTDB one day table)  Jacky_su 980504 add
        decimal fnGetLocalDBMaxSeq_OneDay()
        {

            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            decimal decTmp = 0;

            try
            {
                lock (this)
                {
                    strConn = @"server=" + this.m_strLocalSQLServer_OneDay_IP + ";uid=" + this.m_strLocalSQLServer_OneDay_User + ";pwd=" + this.m_strLocalSQLServer_OneDay_Password + ";database=" + this.m_strLocalSQLServer_OneDay_DataBase;
                    conG9 = new SqlConnection(strConn);

                    string strTableName = "RTDB_FENH_RX";

                    {
                        DateTime currentTime = DateTime.Now;
                        DateTime MTime = DateTime.Now;
                        string strTemp;
                        bool bIsExistTable = false;

                        string strTable = SYS.Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "RXTable");

                        for (int i = 0; i < 10; i++)
                        {
                            strTemp = strTable + MTime.ToString("yyyyMMdd");


                            // �ˬdTable�O�_�s�b
                            if (this.IsExistTable(ref conG9, strTemp) == true)
                            {
                                // �YTable�s�b
                                bIsExistTable = true;

                                // ���oTable�W��
                                strTableName = strTemp;

                                break;
                            }

                            TimeSpan duration = new System.TimeSpan(-(i + 1), 0, 0, 0);
                            MTime = currentTime.Add(duration);
                        }


                        // �YTable ���s�b�A�h�ϥιw�]��RTDB table
                        if (bIsExistTable == false)
                        {
                            strTableName = "RTDB_FENH_RX";
                        }
                    }


                    strSQL = "SELECT TOP 1 seq FROM " + strTableName + "  Order by seq desc";
                    mydataset = new DataSet();

                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    //���oLocal�̤j��seq
                    if (mydataset.Tables[0].Rows.Count == 0)
                    {
                        decTmp = 0;
                    }
                    else
                    {
                        decTmp = Convert.ToDecimal(mydataset.Tables[0].Rows[0]["seq"]);
                    }

                    if (decTmp < 0)
                    { throw new Exception("seq�Ȥp��0 (seq= " + decTmp.ToString() + ")"); }

                }
                return decTmp;

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("���o Local��  IP(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw seq �̤j�ȧ@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }

            finally
            {

            }
            return 0;

        }

        //���RTDBColumn
        bool fnCheckRTDBDBColumn()
        {
            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            bool blnReturn = true;
            string strTmp;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 * FROM " + this.strLocalSQLServer_RXTable;
                    mydataset = new DataSet();
                    strConn = @"server=" + this.strLocalSQLServer_IP + ";uid=" + this.strLocalSQLServer_User + ";pwd=" + this.strLocalSQLServer_Password + ";database=" + this.strLocalSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();

                    strTmp = "";
                    for (int j = 0; j < mydataset.Tables[0].Columns.Count; j++)
                    {
                        strTmp = strTmp + mydataset.Tables[0].Columns[j] + " , ";
                    }


                    for (int i = 0; i < this.dtSourceTables.Columns.Count; i++)
                    {
                        if ((strTmp.ToLower()).IndexOf(this.dtSourceTables.Columns[i].Caption.ToLower()) < 0)
                        { blnReturn = false; }
                    }

                    return blnReturn;
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��� RTDB��  IP(" + this.strLocalSQLServer_IP + ") " + this.strLocalSQLServer_RXTable + " ��Ʈw�PfrontEnd ������@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }
            finally
            {

            }
            return false;
        }


        //���MainDBColumn
        bool fnCheckMainDBDBColumn()
        {
            string strSQL;
            DataSet mydataset;
            string strConn;
            SqlDataAdapter da;
            bool blnReturn = true;
            string strTmp;

            try
            {
                lock (this)
                {

                    strSQL = "SELECT TOP 1 * FROM " + this.strMainDBSQLServer_RXTable;
                    mydataset = new DataSet();
                    strConn = @"server=" + this.strMainDBSQLServer_IP + ";uid=" + this.strMainDBSQLServer_User + ";pwd=" + this.strMainDBSQLServer_Password + ";database=" + this.strMainDBSQLServer_DataBase;
                    conG9 = new SqlConnection(strConn);
                    da = new SqlDataAdapter(strSQL, conG9);
                    da.SelectCommand = new SqlCommand(strSQL, conG9);
                    da.Fill(mydataset);
                    conG9.Close();


                    strTmp = "";
                    for (int j = 0; j < mydataset.Tables[0].Columns.Count; j++)
                    {
                        strTmp = strTmp + mydataset.Tables[0].Columns[j] + " , ";
                    }


                    for (int i = 0; i < this.dtSourceTables.Columns.Count; i++)
                    {
                        if ((strTmp.ToLower()).IndexOf(this.dtSourceTables.Columns[i].Caption.ToLower()) < 0)
                        { blnReturn = false; }
                    }


                    return blnReturn;
                }

            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��� MainDB��  IP(" + this.strMainDBSQLServer_IP + ") " + this.strMainDBSQLServer_RXTable + " ��Ʈw�PfrontEnd ������@�~���`: " + Ex.Message);
                conG9.Close();
                throw Ex;
            }
            finally
            {

            }
            return false;
        }

        #endregion


        #region �����S�����ƥ�A�o��{�����ӻݭn��



        #region Display String Message �ƥ�
        private void DisplayStringMessage(string aMsg)
        {
            lock (typeof(StreamWriter))
            {
                string str = String.Format("{0:00}:{1:00}:{2:00}:{3:000} {4}",	  //�榡��			
                    System.DateTime.Now.Hour,
                    System.DateTime.Now.Minute,
                    System.DateTime.Now.Second,
                    System.DateTime.Now.Millisecond,
                    aMsg);

                if (!this.StopAddtoLB.Checked)
                {
                    this.MessageListBox.Items.Add(str);
                }
                this.LogFileWriter.WriteLine(str);
                if (this.MessageListBox.Items.Count > LineLimit)
                {
                    this.MessageListBox.Items.RemoveAt(0);
                }
                this.MessageListBox.SelectedIndex = this.MessageListBox.Items.Count - 1;
            }
        }
        #endregion

        #region ProcessReceiveBusMessage �ƥ�
        public void ProcessReceiveBusMessage(SYS.MSG Message)
        {
            if (Message.Fun == HDR.FNCEQU.FHOUR || Message.Fun == HDR.FNCEQU.FSRVDN)  //�Ȯɥ��ѨM
            {
            }
            else  //arg���������s����==>�����ˬd�O�_������m���~
            {
                if ((Message.Args[0] < 1) || (Message.Args[0] >= this.LocIndexLength)) //1.���P�_Location�s���O�_�b�d��
                {
                    this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + Message.MsgToString() + "-->Location�޼Ʀ��~"); //�x�sMessage�æP����ܦ�ListBox
                    return;
                }
                if (LocConverter.LocIndex[Message.Args[0]] == 0) //2.�A�P�_�ӽs���O�_�S���w�q--�Y�Ө����|���w�q...�p�p�n�� G12 G13/BL6
                {
                    this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + Message.MsgToString() + "-->Location���w�q"); //�x�sMessage�æP����ܦ�ListBox
                    return;
                }
            }
            //receive bus message, then do different job .....FRAWC,FHOUR.....
            switch (Message.Fun)
            {
                case HDR.FNCEQU.FRAWC: // Do FRAWC COMMAND
                    {
                        this.OnFRAWC(Message);
                        break;
                    }
                case HDR.FNCEQU.FHOUR: //Do FHOUR
                    {
                        this.OnFHOUR();
                        break;
                    }
                case HDR.FNCEQU.FSRVDN: // Do FSRVDN
                    {
                        this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "Taibus���u..."); //�x�sMessage�æP����ܦ�ListBox
                        break;
                    }
                case HDR.FNCEQU.FRSTMR: // Do FRSTMR COMMAND
                    {
                        break;
                    }
                case HDR.FNCEQU.FMONRQ: // Do FMONRQ  Recall From VPI
                    {
                        this.OnAddToControlQueue(this.RecallFromVPI, this.myLoadPort[Message.Args[0]]);
                        break;
                    }
                default:
                    {
                        break;
                    }

            }//switch
        }//ProcessReceiveBusMessage
        #endregion

        #region ShowDBG_ReceiveData �ƥ�
        public void ShowDBG_ReceiveData(string str, string check)
        {
            switch (check)
            {
                case "InQueue":
                    {
                        DBG_ReceiveMessageCounter++;
                        if (this.DBG_ReceiveData.Checked == true)
                        {
                            this.TaskDBG.ClientTask.Show("DBG_RC++=" + this.DBG_ReceiveMessageCounter.ToString() + " " + str);
                        }
                        break;
                    }
                case "DeQueue":
                    {
                        DBG_ReceiveMessageCounter--;
                        if (this.DBG_ReceiveData.Checked == true)
                        {
                            this.TaskDBG.ClientTask.Show("DBG_RC--=" + this.DBG_ReceiveMessageCounter.ToString() + " " + str);
                        }
                        break;
                    }
            }//switch			
        }
        #endregion

        #region ShowGEN_ReceiveData �ƥ�
        public void ShowGEN_ReceiveData(LoadPort a, string check)
        {
            switch (check)
            {
                case "InQueue":
                    {
                        GEN_ReceiveMessageCounter++;
                        if (this.DBG_GEN2MSG.Checked == true)
                        {
                            this.OnDisplayStringMessage("GEN_RC++=" + this.GEN_ReceiveMessageCounter.ToString() + " " + a.LocName);
                        }
                        break;
                    }
                case "DeQueue":
                    {
                        GEN_ReceiveMessageCounter--;
                        if (this.DBG_GEN2MSG.Checked == true)
                        {
                            this.OnDisplayStringMessage("GEN_RC--=" + this.GEN_ReceiveMessageCounter.ToString() + " " + a.LocName);
                        }
                        break;
                    }
            }//switch			
        }
        #endregion

        #region Display_PortStatus �ƥ�
        public void Display_PortStatus()
        {
            this.myPortStatus.ShowPortStatus(this.myLoadPort);
        }
        #endregion

        #region Reload_PortStatus �ƥ�
        public void Reload_PortStatus()
        {
            this.myPortStatus.ShowPortStatus(this.myLoadPort);
        }
        #endregion

        #region CCI_FHOUR �ƥ�
        public void CCI_FHOUR()
        {
            System.EventArgs e = new System.EventArgs();
            this.btn_NewFile_Click(this, e);
        }
        #endregion

        #region CCI_FRAWC �ƥ�
        public void CCI_FRAWC(SYS.MSG Msg)
        {
            if (this.myLoadPort == null)
            {
                this.DisplayStringMessage(this.TaskName + " " + "WARNING - " + "�|�����J�q�T���l���");
                return;
            }
            int tLocnum = Msg.Args[0];
            if ((tLocnum > this.myLoadPort.Length - 1) || (tLocnum <= 0))
            {
                this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "Unknow Location = " + tLocnum.ToString());
                return;
            }

            if (this.myLoadPort[tLocnum] != null)
            {
                byte[] b0 = this.raw2gen(Msg);  //RawData To Gensys
                byte[] fc = SYS.Gensys.Pack(SYS.CRC.PackCRC16(b0));  // CRCPack and GensysPack
                this.OnAddToControlQueue(fc, this.myLoadPort[tLocnum]);
            }
            else
            {
                this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "Unknown Location = " + tLocnum.ToString());
                return;
            }

        }
        #endregion

        #region AddToControlQueue �ƥ�
        public void AddToControlQueue(byte[] fc, LoadPort a)
        {
            lock (a.ControlFcQue.SyncRoot)
            {
                a.ControlFcQue.Enqueue(fc);  //�N���e�ܲ{����FC�T�����s�mArrayList��
            }
            this.OnDisplayStringMessage(a.LocName + " " + "GENCODE - " + SYS.Tools.ByteToHexString(fc) + " ==>to ArrayList");  //�s�ɨ���ܦ�ListBox			
        }
        #endregion

        #region ComPort_Init �ƥ�
        public void ComPort_Init()  //�q�T��-->��l��
        {
            //			string str = null;  //Ū�� ini �����r���ܼ�
            //			string PortFilename = this.txb_PortFileName.Text; //Ū���ɮפ�������|			
            //			
            //			StreamReader SRportsetting = new StreamReader(PortFilename, System.Text.Encoding.Default);
            //						
            //			//LocName	LocNum	ComPort		BaudRate	ByteSize	Parity		StopBits	FlowControl		ReadTimeout  EncodingCode
            //			//G01		31		COM1		1200		8			NONE		1			NO				30			 950
            //			//G02		30		COM2		1200		8			NONE		1			NO				30			 950
            //
            //			Regex regexLine = new Regex(
            //				@"(?<LocName>(\S)+)\s+" + 
            //				@"(?<LocNum>(\d)+)\s+" +
            //				@"(?<ComPort>(\S)+)\s+" + 
            //				@"(?<BaudRate>(\d)+)\s+" + 
            //				@"(?<ByteSize>(\d))\s+" +
            //				@"(?<Parity>(\S)+)\s+"  + 
            //				@"(?<StopBits>(\S)+)\s+" + 
            //				@"(?<FlowControl>(\S)+)\s+" +
            //		      //@"(?<ReadTimeout>(\d)+)"
            //				@"(?<ReadTimeout>(\d)+)\s+" +
            //                @"(?<EncodingCode>(\S)+)"
            //				);		
            //
            //			//Match match;
            //			while((str = SRportsetting.ReadLine()) != null)			
            //			{
            //				str = str.Trim();
            //				if (str.Length <10) //���פp��10���L
            //					continue;				
            //				if (str[0] == ';')  //���ѳ������L
            //					continue;
            //
            //				Match match = regexLine.Match(str);
            //				if(match.Success == false )
            //				{
            //					this.OnDisplayStringMessage("��l�Ƹ�Ʀ��~");  //���L�k�X��
            //					return;
            //				}
            //				
            //				tLocName = match.Groups["LocName"].ToString();
            //				tLocNum = Int32.Parse(match.Groups["LocNum"].ToString());
            //				tComPort = Int32.Parse((match.Groups["ComPort"].ToString()).Remove(0,3));
            //				try
            //				{					
            //					tBaudRate = Int32.Parse(match.Groups["BaudRate"].ToString());
            //					tByteSize = byte.Parse(match.Groups["ByteSize"].ToString());
            //					
            //					switch(match.Groups["Parity"].ToString().ToUpper())
            //					{
            //						case "NONE":
            //							tParity = 0;
            //							break;
            //						case "ODD":
            //							tParity = 1;
            //							break;
            //						case "EVEN":
            //							tParity = 2;
            //							break;
            //						case "MARK":
            //							tParity = 3;
            //							break;
            //						case "SPACE":
            //							tParity = 4;
            //							break;
            //						default :						
            //							this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + "Parity Error at " + tLocName);
            //							break;
            //					}                    
            //					switch(match.Groups["StopBits"].ToString())
            //					{
            //						case "1":
            //							tStopBits = 0;
            //							break;
            //						case "1.5":
            //							tStopBits = 1;
            //							break;
            //						case "2":
            //							tStopBits = 2;
            //							break;
            //						default :
            //							this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + "StopBits Error at " + tLocName);
            //							break;
            //					}					
            //					switch(match.Groups["FlowControl"].ToString())
            //					{
            //						case "NO":
            //							tFlowControl = 0;
            //							break;
            //						case "HW":
            //							tFlowControl = 1;
            //							break;
            //						case "SW":
            //							tFlowControl = 2;
            //							break;
            //						default :
            //							this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + "FlowControl Error at " + tLocName);						
            //							break;
            //					}
            //					tReadTimeout = Int32.Parse(match.Groups["ReadTimeout"].ToString())*1000;  //���⦨�L��	
            //
            //					//SCADA
            //					tEncodingCode = match.Groups["EncodingCode"].ToString();//�]�w��ư�X�A�Ҧp 950���j���X
            //
            //				}
            //				catch
            //				{
            //					tBaudRate = 1200;
            //					tByteSize = 8;
            //					tStopBits=0;
            //					tParity = 0;
            //					tFlowControl=0;
            //					tReadTimeout=30000;
            //
            //					//SCADA
            //					tEncodingCode ="950";
            //
            //
            //					this.OnDisplayStringMessage("All variables set to default value");
            //				}				
            //				this.myLoadPort[tLocNum] = new LoadPort(tLocName, tLocNum, tComPort, tBaudRate, tByteSize, tParity, tStopBits, tFlowControl, tReadTimeout,tEncodingCode);
            //				this.myLoadPort[tLocNum].OnValidateReceiveComMessage		+= new ValidateReceiveComMessage_Handler(this.ValidateReceiveComMessage);
            //				this.myLoadPort[tLocNum].OnFB_MessageToVPI					+= new FB_MessageToVPI_Handler(this.FB_MessageToVPI);
            //				this.myLoadPort[tLocNum].OnFC_MessageToVPI					+= new FC_MessageToVPI_Handler(this.FC_MessageToVPI);
            //				this.myLoadPort[tLocNum].OnJudge_CF							+= new Judge_CF_Handler(this.Judge_CF);
            //				this.myLoadPort[tLocNum].OnShowGEN_ReceiveData				+= new ShowGEN_ReceiveData_Handler(this.ShowGEN_ReceiveData);
            //				this.myCF_Count[tLocNum] = new CF_Count();
            //			}//While			
            //			SRportsetting.Close();  //�����ɮ�
            //			
        }// ComPort_Init()
        #endregion

        #region AddTreeView �ƥ�
        public void AddTreeView()
        {
            //			if(this.TView_Port.Nodes.IndexOf(this.RootNode) >= 0)
            //			{
            //				this.TView_Port.Nodes[this.TView_Port.Nodes.IndexOf(this.RootNode)].Nodes.Clear();				
            //				this.TView_Port.Nodes.Clear();
            //			}
            //            
            //			RootNode.ImageIndex = 0;
            //			this.TView_Port.Nodes.Add(RootNode);			
            //
            //			foreach( LoadPort a in myLoadPort)
            //			{
            //				if(a != null)
            //				{
            //					PortNode = new TreeNode(a.LocName);
            //					PortNode.ImageIndex = 1;
            //					if(a.PortStatus == "Close")
            //					{
            //						PortNode.ForeColor = Color.Red;
            //					}
            //					else
            //					{
            //						PortNode.ForeColor = Color.Black;
            //					}
            //					this.TView_Port.Nodes[this.TView_Port.Nodes.IndexOf(this.RootNode)].Nodes.Add(PortNode);					
            //				}
            //			}
        }
        #endregion

        #region TView_Port_AfterSelect �ƥ�
        private void TView_Port_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            //			this.LB_PortMes.Items.Clear();			
            //			if(e.Node == RootNode)  //����
            //			{				
            //				e.Node.SelectedImageIndex =0;
            //				for(int i=0; i<this.TView_Port.SelectedNode.Nodes.Count; i++)
            //				{
            //					this.LB_PortMes.Items.Add(this.TView_Port.SelectedNode.Nodes[i].Text.ToString());
            //				}				
            //			}
            //			else  //��毸
            //			{
            //				e.Node.SelectedImageIndex =1;
            //				LoadPort a = this.myLoadPort[LocConverter.Loc2Num(e.Node.Text)];
            //				string tmpParity;
            //				string tmpStopBits;
            //				string tmpFlowControl;
            //				int tmpReadTimeout;
            //
            //				switch(a.Parity.ToString())
            //				{
            //					case "0":
            //						tmpParity = "NONE";
            //						break;
            //					case "1":
            //						tmpParity = "ODD";
            //						break;
            //					case "2":
            //						tmpParity = "EVEN";
            //						break;
            //					case "3":
            //						tmpParity = "MARK";
            //						break;
            //					case "4":
            //						tmpParity = "SPACE";
            //						break;
            //					default :
            //						tmpParity = "ERROR!!";
            //						break;
            //				}
            //				switch(a.StopBits.ToString())
            //				{
            //					case "0":
            //						tmpStopBits = "1";
            //						break;
            //					case "1":
            //						tmpStopBits = "1.5";
            //						break;
            //					case "2":
            //						tmpStopBits = "2";
            //						break;
            //					default :	
            //						tmpStopBits = "ERROR!!";
            //						break;
            //				}					
            //				switch(a.FlowControl.ToString())
            //				{
            //					case "0":
            //						tmpFlowControl = "NO";
            //						break;
            //					case "1":
            //						tmpFlowControl = "HW";
            //						break;
            //					case "2":
            //						tmpFlowControl = "SW";
            //						break;
            //					default :
            //						tmpFlowControl = "ERROR!!";
            //						break;
            //				}
            //				tmpReadTimeout = a.ReadTimeout/1000;
            //
            //				this.LB_PortMes.Items.Add("LOC: " + a.LocName);
            //				this.LB_PortMes.Items.Add("LOC No#: " + a.LocNum);
            //				this.LB_PortMes.Items.Add("COM No#: " + "COM" + a.ComPort);					
            //				this.LB_PortMes.Items.Add("ComStatus: " + a.PortStatus);
            //				this.LB_PortMes.Items.Add("BaudRate: " + a.BaudRate);
            //				this.LB_PortMes.Items.Add("ByteSize: " + a.ByteSize);
            //				this.LB_PortMes.Items.Add("Parity: " + tmpParity);
            //				this.LB_PortMes.Items.Add("StopBits: " + tmpStopBits);
            //				this.LB_PortMes.Items.Add("FlowCtrl: " + tmpFlowControl);
            //				this.LB_PortMes.Items.Add("Timeout: " + tmpReadTimeout);				
            //			}
        }
        #endregion

        #region OpenComPort �ƥ�
        public void OpenComPort()
        {
            //			if((this.TView_Port.SelectedNode == RootNode) || (AutoComPortOpen)) //ComPort���� or �ҰʧY�۰ʶ}��
            //			{
            //				AutoComPortOpen = false;
            //				foreach( LoadPort a in this.myLoadPort)
            //				{
            //					if ( a != null && a.PortStatus == "Close")
            //					{
            //						Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ...";
            //						this.Refresh();
            //						if(true == a.hnd2com.Open())  //�N�� ComPort ���\�}��
            //						{
            //							this.OnDisplayStringMessage(this.TaskName + " " + "INFO    - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Open .......!!");
            //							a.PortStatus = "Open";
            //							this.cbBox_LocSel.Items.Add(a.LocName);
            //							Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ... Succeed !";
            //						}
            //						else  //�Y�� ComPort �}�ҥ���
            //						{
            //							this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Error! Can not be Opened!!");
            //							Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ... failure !";
            //						}
            //					}
            //					this.Refresh();
            //				}//foreach
            //			}
            //			else if(this.TView_Port.SelectedNode != null)
            //			{
            //				LoadPort a = this.myLoadPort[LocConverter.Loc2Num(this.TView_Port.SelectedNode.Text)];
            //				if(a.PortStatus == "Close")
            //				{
            //					Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ...";
            //					this.Refresh();
            //					if(true == a.hnd2com.Open())  //�N�� ComPort ���\�}��
            //					{
            //						this.OnDisplayStringMessage(this.TaskName + " " + "INFO    - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Open .......!!");
            //						a.PortStatus = "Open";
            //						this.cbBox_LocSel.Items.Add(a.LocName);
            //						Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ... Succeed !";
            //					}
            //					else  //�Y�� ComPort �}�ҥ���
            //					{
            //						this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Error! Can not be Opened!!");
            //						Port_Polling_Status.Text = "Opening COM" + a.ComPort.ToString() + " ( " + a.LocName.ToString()+" ) ... failure !";
            //					}
            //					this.Refresh();
            //				}
            //			}//if
        }
        #endregion

        #region CloseComPort �ƥ�
        public void CloseComPort()
        {
            //			if(this.TView_Port.SelectedNode == RootNode)  //ComPort����
            //			{
            //				foreach( LoadPort a in this.myLoadPort)
            //				{
            //					if ( a != null && a.PortStatus == "Open")
            //					{
            //						try  //�N�� ComPort ����
            //						{
            //							a.hnd2com.Close();
            //							this.OnDisplayStringMessage(this.TaskName + " " + "INFO    - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Close .......!!");
            //							a.PortStatus = "Close";
            //							this.cbBox_LocSel.Items.RemoveAt(this.cbBox_LocSel.Items.IndexOf(a.LocName));
            //						}
            //						catch  //�Y�� ComPort ��������
            //						{
            //							this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Error! Can not be Closed!!");
            //						}
            //					}				
            //				}//foreach
            //			}
            //			else if(this.TView_Port.SelectedNode != null)
            //			{
            //				LoadPort a = this.myLoadPort[LocConverter.Loc2Num(this.TView_Port.SelectedNode.Text)];
            //				if(a.PortStatus == "Open")
            //				{
            //					try  //�N�� ComPort ����
            //					{
            //						a.hnd2com.Close();
            //						this.OnDisplayStringMessage(this.TaskName + " " + "INFO    - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Close .......!!");
            //						a.PortStatus = "Close";
            //						this.cbBox_LocSel.Items.RemoveAt(this.cbBox_LocSel.Items.IndexOf(a.LocName));
            //					}
            //					catch  //�Y�� ComPort ��������
            //					{
            //						this.OnDisplayStringMessage(this.TaskName + " " + "ERROR   - " + a.LocName + " COM" + a.ComPort.ToString() + " (" + a.LocNum.ToString() +")"+"  Error! Can not be Closed!!");
            //					}
            //				}
            //			}//if
        }
        #endregion

        #region TreeView Update �ƥ�
        private void TreeViewUpdate(System.Windows.Forms.TreeViewEventArgs e)
        {
            //			if(this.TView_Port.InvokeRequired)
            //			{
            //				this.TView_Port.Invoke(this.OnTreeView_Update);
            //			}
            //			else
            //			{
            //				LoadPort a = this.myLoadPort[LocConverter.Loc2Num(e.Node.Text)];
            //				if(a != null)
            //				{
            //					if(a.PortStatus == "Close")
            //					{
            //						e.Node.ForeColor = Color.Red;
            //					}
            //					else
            //					{
            //						e.Node.ForeColor = Color.Black;
            //					}
            //				}
            //			}		
            //			this.TView_Port.Refresh();
        }
        #endregion


        #endregion

        #region ValidateReceiveComMessage �ƥ�
        public void ValidateReceiveComMessage(byte[] bCommRead, LoadPort a)
        {
            //			string strTmp="";
            //			byte RecvByte;


            #region "trash"
            //
            //
            //			switch (a.LocName) 
            //			{
            //				case "ECS":
            //			
            //					strECS1+=strTmp;
            //					break;
            //			
            //			}
            //
            //
            //	         int pos=0,pos1=0,pos2=0;
            //			 string line01="";
            //			pos=strECS1.IndexOf(crln);
            //			do
            //			{
            //                 
            //				if(pos!=-1)
            //				{
            //					if(pos==0)
            //					{
            //						strECS1=strECS1.Substring(2);
            //					}
            //					else
            //					{
            //						line01=strECS1.Substring(0,pos);
            //						strECS1=strECS1.Substring(pos);
            //						this.OnDisplayStringMessage(a.LocName + " " + "MESSAGE - " + line01);
            //
            //					}
            //				}
            //
            //
            //					pos=strECS1.IndexOf(crln);
            //
            //
            //			}while(pos!=-1);
            //
            //
            //	




            //			for(int i=0; i<bCommRead.Length; i++)
            //			{
            //				RecvByte = bCommRead[i];
            //
            //
            //						
            ////				this.OnDisplayStringMessage(a.LocName + " " + "MESSAGE - " + RecvByte.ToString());
            //
            //				if(RecvByte != HDR.CodeTable.H_CR || RecvByte != HDR.CodeTable.H_LF)  //CR��
            //				{
            //					a.ByteAL.Add(RecvByte);
            //				}
            //				else if(RecvByte == HDR.CodeTable.H_F2)  //�Y��F2
            //				{
            //					if(a.ByteAL.Count == 0)  //�Ĥ@��byte��F2��
            //					{
            //						a.ByteAL.Add(RecvByte);
            //					}
            //					else  //���O�Ĥ@��byte����F2,���ܤ��e���ʥ]�S��F6����,��Garbage
            //					{
            //						byte[] RecvMessage = new byte[a.ByteAL.Count];
            //						for(int j=0; j<a.ByteAL.Count; j++)
            //						{
            //							RecvMessage[j] = (byte)a.ByteAL[j];
            //						}
            //								
            //						this.OnDisplayStringMessage(a.LocName + " " + "ERROR   - " + "GENCODE Garbage " + SYS.Tools.ByteToHexString(RecvMessage));
            //		
            //						a.ByteAL.Clear();        //�N���e��ƲM��
            //						
            //						a.ByteAL.Add(RecvByte);  //�NF2�[�J
            //					}
            //				}				
            //				else if(RecvByte == HDR.CodeTable.H_LF || RecvByte == HDR.CodeTable.H_CR ||RecvByte == HDR.CodeTable.H_NUL )  //�Y��CR,LF,NUL��==>����message����
            //				{
            //					a.ByteAL.Add(RecvByte);
            //		
            //					byte[] RecvMessage = new byte[a.ByteAL.Count];
            //					for(int j=0; j<a.ByteAL.Count; j++)
            //					{
            //						RecvMessage[j] = (byte)a.ByteAL[j];
            //					}
            //					a.ByteAL.Clear();  //�M��
            //					
            //
            //					this.OnDisplayStringMessage(a.LocName + " " + "GENCODE - " + SYS.Tools.ByteToHexString(RecvMessage));  //�T����ܨæs�ɡA��DTS�榡
            //					myCF_Count[a.LocNum].Time = DateTime.Now;  //�T�{�ɶ��۴�
            //					byte[] unGensysByte = (SYS.CRC.UnpackCRC16(SYS.Gensys.Unpack(RecvMessage),1));					
            //
            //					if (RecvMessage[0] == HDR.CodeTable.H_F1)
            //					{
            //						if(true == this.AutoPolling)  //�p�G��{���@���߮� �~�ݧ@�H�UTimerReset�A��ť�ɤ��ݭ��m
            //						{
            //							a.AutoPolling_Timer.Stop();   //����惡����Polling				
            //							a.AutoPolling_Timer.Start();  //���s�}�l�惡����Polling
            //						}
            //					}
            //					else if((RecvMessage[0] == HDR.CodeTable.H_F2) && (unGensysByte != null))
            //					{		
            //						if(true == this.AutoPolling && a.hnd2com.Opened)  //�Y�Ұʹ�{�������ߥB�ӳq�T��w�}��
            //						{
            //							a.AutoPolling_Timer.Stop();   //����惡����Polling								
            //							a.hnd2com.Write(this.AckToVPI);  //�e�XFA�T�{�T���ܲ{��VPI
            //							this.OnDisplayStringMessage(a.LocName + " " + "GENTOVPI- " + SYS.Tools.ByteToHexString(this.AckToVPI));  //���FA�T�����x�s���
            //							a.AutoPolling_Timer.Start();  //���s�}�l�惡����Polling							
            //						}						
            //						this.OnProcessReceviceComMessage(unGensysByte, a);  //�e��DoComPortMessage�� Method �B�z���{����FA�T�{�^���P��½Ķ��FRAWI
            //					}
            //					else
            //					{
            //						this.OnDisplayStringMessage(a.LocName + " " + "WARNING - " + "�ӰT���S���B�z�A�гq���{���H��");  //���FA�T�����x�s���
            //					}					
            //				}//if
            //			}//for

            #endregion



        }
        #endregion

        #region �ƥ��




        #region ProcessReceiveComMessage �ƥ�
        public void ProcessReceiveComMessage(byte[] bo, LoadPort a)  //�Ω�{�����
        {
            SYS.MSG SendtoServerMsg = null;
            SYS.MSG LCINDmsg = new SYS.MSG(HDR.TSKEQU.CCI, HDR.FNCEQU.FLCIND, (ushort)((a.LocNum) & 0x00FF));

            switch (bo[0])
            {
                case HDR.CodeTable.H_F2:  //F2--�{��VPI�e�Ӥ��T��
                    {
                        byte[] data = new byte[2];          //�s��Genesys byte����m�P���
                        for (int i = 2; i < bo.Length - 2; i += 2)  //���� F2 �P 01(RTU) ���B�z, ��l�@���B�z�G��byte
                        {                                   //���ε���(=) , �]���o�˥i�H�N E005�@�֥h�� 
                            data[0] = bo[i];    //byte site
                            data[1] = bo[i + 1];  //byte value
                            SendtoServerMsg = gen2raw(data, a.LocNum);  //�ഫ�� RawData FRAWI
                            if (SendtoServerMsg != null)
                            {
                                if (this.TaskDBG.ClientTask.IsConnect == true)  //�T�{�O�_�PTBS�s�u
                                {
                                    this.OnSendtoQueue(SendtoServerMsg);  //�e��TBS ���B�i�� 1.DTS 2.FT810 �榡,�ݷ��ɿﶵ�өw
                                    this.OnSendtoQueue(LCINDmsg);
                                }
                                else
                                {
                                    this.OnDisplayStringMessage(a.LocName + " " + "WARNING - " + "���_�s�u�ɭP�L�k�e��TBS--> " + SendtoServerMsg.MsgToString());
                                }
                                SendtoServerMsg = null;
                            }
                        }
                        break;
                    }
                case HDR.CodeTable.H_F3:  //�t�λP�{�����հT���ʥ]�ǻ��O�_���T�ɥ�
                    {
                        //�̾ڲ{���[��G�A�{��VPI���Ӥ��|��F3�T���^��
                        break;
                    }
            }//switch
        }
        #endregion

        #region ProcessPlayBackMessage �ƥ�
        public void ProcessPlayBackMessage(byte[] bo, ushort tmpLocNum, string tmpLocName)
        {
            SYS.MSG SendtoServerMsg = null;
            SYS.MSG LCINDmsg = new SYS.MSG(HDR.TSKEQU.CCI, HDR.FNCEQU.FLCIND, (ushort)((tmpLocNum) & 0x00FF));
            byte[] data = new byte[2];          //�s��Genesys byte����m�P���
            for (int i = 2; i < bo.Length - 2; i += 2)  //���� F2 �P 01(RTU) ���B�z, ��l�@���B�z�G��byte
            {                                   //���ε���(=) , �]���o�˥i�H�N E005�@�֥h�� 
                data[0] = bo[i];    //byte site
                data[1] = bo[i + 1];  //byte value
                SendtoServerMsg = gen2raw(data, tmpLocNum);  //�ഫ�� RawData FRAWI
                if (SendtoServerMsg != null)
                {
                    if (this.TaskDBG.ClientTask.IsConnect == true)  //�T�{�O�_�PTBS�s�u
                    {
                        this.OnSendtoQueue(SendtoServerMsg);  //�e��TBS ���B�i�� 1.DTS 2.FT810 �榡,�ݷ��ɿﶵ�өw
                        this.OnSendtoQueue(LCINDmsg);
                    }
                    else
                    {
                        this.OnDisplayStringMessage(tmpLocName + " " + "WARNING - " + "���_�s�u�ɭP�L�k�e��TBS--> " + SendtoServerMsg.MsgToString());
                    }
                    SendtoServerMsg = null;
                }
            }
        }
        #endregion

        #region ReceiveMsg �ƥ�
        public void ReceiveMsg(SYS.MSG msg)
        {
            lock (ReceiveMsgQue.SyncRoot)
            {
                ReceiveMsgQue.Enqueue(msg);
            }
        }
        #endregion

        #region DoMessageTimer_Elapse �ƥ�
        public void DoMessageTimer_Elapse(object sender, System.Timers.ElapsedEventArgs e)
        {
            int i = 0;
            while (i++ < this.DoMsgLimite)
            {
                if (ReceiveMsgQue.Count != 0)
                {
                    lock (ReceiveMsgQue.SyncRoot)
                    {
                        SYS.MSG msg = (SYS.MSG)ReceiveMsgQue.Dequeue();
                        this.OnProcessReceiveBusMessage(msg);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        #endregion

        #region SendMsgToTBC_TimerElapsed �ƥ�
        private void SendMsgToTBC_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            //mark by SCADA
            //			int i=0;
            //			while(i++< this.SendToTBCLimite)
            //			{
            //				if(SyncdSendToTBCQueue.Count != 0)
            //				{
            //					lock(SyncdSendToTBCQueue.SyncRoot)
            //					{					
            //						SYS.MSG a = (SYS.MSG)SyncdSendToTBCQueue.Dequeue();
            //						this.TaskDBG.ClientTask.SendtoServer(a);
            //						this.TaskDBG.ClientTask.Show(a.MsgToString() + " CCI --> TBC");
            //					}
            //				}
            //				else
            //				{
            //					this.OnDisplayStringMessage("�L��ưe��TBC");
            //					break;
            //				}
            //			}
        }
        #endregion

        #region SendtoQueue �ƥ�
        public void SendtoQueue(SYS.MSG SendtoQueueMsg)
        {
            lock (SyncdSendToTBCQueue.SyncRoot)
            {
                this.SyncdSendToTBCQueue.Enqueue(SendtoQueueMsg);
            }
        }
        #endregion

        #region FB_MessageToVPI �ƥ�
        public void FB_MessageToVPI(LoadPort a)
        {
            if (a.hnd2com.Opened == true)
            {
                a.hnd2com.Write(this.PollingToVPI);  //��{���@Polling				
                this.OnDisplayStringMessage(a.LocName + " " + "GENTOVPI- " + SYS.Tools.ByteToHexString(this.PollingToVPI) + "--Polling");  //�NPolling�� FB0182F000F6 �T���e��ListBox
            }
        }
        #endregion

        #region FC_MessageToVPI �ƥ�
        public void FC_MessageToVPI(LoadPort a)
        {
            a.AutoPolling_Timer.Stop();  //����惡����Polling
            for (int i = 0; i < a.ControlFcQue.Count; i++)
            {
                lock (a.ControlFcQue.SyncRoot)
                {
                    byte[] fc = (byte[])a.ControlFcQue.Dequeue();

                    if (a.hnd2com.Opened == true)
                    {
                        a.hnd2com.Write(fc);
                        this.OnDisplayStringMessage(a.LocName + " " + "GENTOVPI- " + SYS.Tools.ByteToHexString(fc));
                    }
                    else
                    {
                        this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + a.LocName + ":�{���q�T�𥼶}�q " + SYS.Tools.ByteToHexString(fc));
                    }
                }
            }//for				
            a.AutoPolling_Timer.Start();  //���s�}�l�惡����Polling
        }
        #endregion

        #region Judge_CF �ƥ�
        public void Judge_CF(LoadPort a)
        {
            a.myTimeSpan = DateTime.Now - myCF_Count[a.LocNum].Time;  //�����T���P�W���T�����ɶ��۹j

            if (a.myTimeSpan.TotalSeconds < 30)
            {
                if (myCF_Count[a.LocNum].CFstatus != 0x0000)
                {
                    SYS.MSG myCFMsg = new SYS.MSG(HDR.TSKEQU.CCI, HDR.FNCEQU.FCDI, ushort.Parse(a.LocNum.ToString()), 0x0000, myCF_Count[a.LocNum].CFstatus);
                    this.OnSendtoQueue(myCFMsg);
                    myCF_Count[a.LocNum].CFstatus = 0x0000;
                }
            }
            else if (a.myTimeSpan.TotalSeconds >= 30 && a.myTimeSpan.TotalSeconds < 60)
            {
                if (myCF_Count[a.LocNum].CFstatus != 0x1000)
                {
                    SYS.MSG myCFMsg = new SYS.MSG(HDR.TSKEQU.CCI, HDR.FNCEQU.FCDI, ushort.Parse(a.LocNum.ToString()), 0x1000, myCF_Count[a.LocNum].CFstatus);
                    this.OnSendtoQueue(myCFMsg);
                    myCF_Count[a.LocNum].CFstatus = 0x1000;
                }
            }
            else if (a.myTimeSpan.TotalSeconds >= 60)
            {
                if (myCF_Count[a.LocNum].CFstatus != 0x5000)
                {
                    SYS.MSG myCFMsg = new SYS.MSG(HDR.TSKEQU.CCI, HDR.FNCEQU.FCDI, ushort.Parse(a.LocNum.ToString()), 0x5000, myCF_Count[a.LocNum].CFstatus);
                    this.OnSendtoQueue(myCFMsg);
                    myCF_Count[a.LocNum].CFstatus = 0x5000;
                }
            }
        }
        #endregion

        #endregion

        //----------------------------------------------

        #region SetLogFileDateExt Method
        public string SetLogFileDateExt()
        {
            DateTime a = new DateTime();
            a = DateTime.Now;
            return (a.ToString("yyyyMMdd"));
        }
        #endregion

        #region SetLogFileVersion Method
        public void SetLogFileVersion(string FileDateExt)
        {
            int i = 0;
            string temp;
            do
            {
                i++;
                temp = LogDir + LogFileName + FileDateExt + "_" + i + "." + LogFileExt;
            }
            while (File.Exists(temp));
            LogFileFullName = LogFileName + FileDateExt + "_" + i + "." + LogFileExt;
            LogFilePath = temp;
            LogFileWriter = new StreamWriter(LogFilePath, true, System.Text.Encoding.Default);
            LogFileWriter.AutoFlush = true;
            LogFileWriter.WriteLine(";" + this.TaskName + "�G" + this.LogFileFullName + " file opened at " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            this.txb_SaveFile.Text = LogFilePath;
            this.txb_SaveFile.SelectionStart = this.txb_SaveFile.Text.Length;
            return;
        }

        public void WriteMonMsg(string msg)    //Hsisnon970528 add
        {

            MonFileFullName = MonFilePath + "\\" + MonFileName + "." + MonFileExt;
            MonFileWriter = new StreamWriter(MonFileFullName, true, System.Text.Encoding.Default);
            MonFileWriter.AutoFlush = true;
            MonFileWriter.WriteLine("[Alarm];Task=" + this.strFormText + ";Time=" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ";Msg=" + msg);
            MonFileWriter.Close();
        }
        #endregion

        //960725 SFI �W�[�R�� 30�ѫe�� log file
        #region  DeleteLog
        public void DeleteLogFile()
        {
            DateTime dt;
            dt = System.DateTime.Now.Date.AddDays(-30);

            string[] files = Directory.GetFiles(LogDir, "*");

            this.OnDisplayStringMessage("�R�� 30 �ѫeLog File �@�~.");
            for (int i = 0; i < files.Length; i++)
            {
                if (DateTime.Compare(dt, Directory.GetLastWriteTime(files[i])) > 0)
                {
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch (Exception e)
                    {
                        this.OnDisplayStringMessage("�R�� Log File �@�~���`�G" + e.Message.ToString());
                    }
                }
            }
            this.OnDisplayStringMessage("�R�� 30 �ѫeLog File �@�~����.");

        }
        #endregion
        #region gen2raw & raw2gen Method
        // FT810�PDTS���t��
        //		-------------------------------------
        // FT810	8	4	2	1 |	8	4	2	1
        //		==>	8			  |		4
        //		-------------------------------------
        //			1	2	4	8 | 1	2	4	8
        //			1			  |		2			<==	DTS
        //		-------------------------------------
        //		�BFT810��byte number�n��DTS��byte number+1

        public SYS.MSG gen2raw(byte[] genData, int LocNum) //Genesys to FRAWI
        {
            //genData : FCCRCV stn Fx rtu {bn bi} cl ch Fx

            SYS.MSG msg = null;

            //�T��:f2
            if (this.RAWType == "DTS")
            {
                ushort ftsk = HDR.TSKEQU.CCI;    //From Task
                ushort fun = HDR.FNCEQU.FRAWI;  //Message 
                ushort bstn = (ushort)((LocNum) & 0x0000FFFF);  //Station
                ushort bno = genData[0];  //byte number
                ushort bnew = genData[1];  //byte value
                msg = SYS.MSG.msgPack(ftsk, fun, bstn, bno, bnew);
                return msg;  //DTS Form
            }
            else if (this.RAWType == "FT810")
            {
                ushort ftsk = HDR.TSKEQU.CCI;    //From Task
                ushort fun = HDR.FNCEQU.FRAWI;  //Message 
                ushort bstn = (ushort)((LocNum) & 0x0000FFFF);  //Station
                ushort bno = (ushort)(genData[0] + 1);  //byte number
                ushort bnew = UshortReverse(genData[1]);  //byte value
                msg = SYS.MSG.msgPack(ftsk, fun, bstn, bno, bnew);
                return msg;  //FT810 Form
            }
            else
            {
                this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "RAWType(FT810<->DTS) �榡���~");
                return null;
            }
        }

        public ushort UshortReverse(int a)
        {
            int b = 0;
            int c = 0;
            for (int i = 0; i <= 7; i++)
            {
                c = (a & 1);
                b = (b << 1) + c;
                a = a >> 1;
            }
            return (ushort)b;
        }

        public byte[] raw2gen(SYS.MSG RAWCmsg) //FRAWC to Genesys 
        {
            byte[] sendbyte = new byte[4];  //�Ȩ���FRAWC���e�|�ӰѼ�,����h���h��ѼƱ˱󤣥�
            if (RAWCmsg.Fun == HDR.FNCEQU.FRAWC)
            {
                if (this.RAWType == "DTS")
                {
                    sendbyte[0] = HDR.CodeTable.H_FC;
                    sendbyte[1] = (byte)(RAWCmsg.Args[1] & 0x00FF);		 // RTU
                    sendbyte[2] = (byte)((RAWCmsg.Args[2]) & 0x00FF);	 // Byte
                    sendbyte[3] = (byte)(RAWCmsg.Args[3] & 0x00FF);		 // Byte value
                }
                else if (this.RAWType == "FT810")
                {
                    sendbyte[0] = HDR.CodeTable.H_FC;
                    sendbyte[1] = (byte)(RAWCmsg.Args[1] & 0x00FF);		  // RTU
                    sendbyte[2] = (byte)(((RAWCmsg.Args[2]) & 0x00FF) - 1); // Byte
                    sendbyte[3] = (byte)UshortReverse((RAWCmsg.Args[3] & 0x00FF)); // Byte value
                }
            }
            return sendbyte;
        }
        #endregion

        #region PlayBack_Play Method
        public void PlayBack_Play()  // PlayBack's Play Function
        {
            //			// G01 ~ G04 �q�{��SER�Ǧ^CCER���쪺�T��
            //			// 10:17:58:758 - G04 : F2010940E005A359F6
            //			// 10:18:01:712 - G01 : F2010400E005A021F6
            //
            //			int TimeH = 0,  //��
            //				TimeM = 0,  //��
            //				TimeS = 0,  //��
            //				TimeMS = 0, //�L��
            //				SleepTime = 0,  //����Ǻ�ı�ɶ�
            //				oldtimer = 0,   //�����ƥ�ɶ�
            //				newtimer = 0;   //�U�@�Өƥ�ɶ�
            //
            //			string	tmpLocName = null,  //���W
            //					LogData = null,  //��Ƥ��e					
            //					LogHead = null,   //��ƪ��Y
            //					str = null;      //���ɮפ�Ū�i���r��
            //
            //			while( (str = this.PlayBackLogFile.ReadLine()) != null )
            //			{
            //				if(str.IndexOf("GENCODE") > 0 )  //�ȳB�z�{���Ӥ��T��
            //				{
            //					this.lab_message.Text = str;  //�N���Ū�줧��Tshow��Form�W
            //					this.OnDisplayStringMessage(str.Substring(13,str.Length-13));
            //
            //					//930826 James�ק��קK3�Ӧr�H�W�������L�kŪ�������D
            //					string [] mystring = str.Split();
            //
            //					TimeH = Int32.Parse(mystring[0].Substring(0,2));  //��
            //					TimeM = Int32.Parse(mystring[0].Substring(3,2));  //��
            //					TimeS = Int32.Parse(mystring[0].Substring(6,2));  //��
            //					TimeMS = Int32.Parse(mystring[0].Substring(9,3)); //�L��
            //
            //					//�NŪ�줧��T�[�WOffset��Ashow��Form�W
            //					this.lab_Offset.Text = new System.TimeSpan(TimeH, TimeM, (TimeS + this.PlayBackOffset)).ToString();
            //
            //					tmpLocName = mystring[1]; //���W
            //					LogHead = mystring[4].Substring(0,2);  //��ƪ��Y
            //					LogData = mystring[4];  //��Ƥ��e
            //
            //					newtimer = ((TimeH * 60 + TimeM) * 60 + TimeS) * 1000 + TimeMS;  //�N�ɶ����⦨�L�����
            //									
            //					if ((oldtimer !=0) && (speed !=0))
            //					{
            //						SleepTime = (int)((newtimer-oldtimer)/speed);
            //						if(SleepTime >=0)  //�T�O��Ѯɶ��q0�I�p���,�o�ͮɶ��p����~�����D
            //						{
            //							Thread.Sleep(SleepTime);	//�Ψ�U�@�Ӯɶ��I�A�_��
            //						}
            //						else //�]�����ɶ��۴�t��
            //						{
            //							continue;
            //						}
            //					}
            //					if(LogHead == "F2")  //���Y��F2 -->  VPI����Ʀ^�Ǯ�
            //					{
            //						byte[] PlayBack_ind = SYS.Tools.HexStringToByte(LogData);  //�N�{������ন byte[]													
            //						byte[] unGensysByte = (SYS.CRC.UnpackCRC16(SYS.Gensys.Unpack(PlayBack_ind),1));
            //						if(unGensysByte != null)
            //						{
            //							this.OnProcessPlayBackMessage(unGensysByte, SYS.LocConverter.Loc2Num(tmpLocName), tmpLocName);
            //						}						
            //					}
            //					oldtimer = newtimer;
            //				}//if
            //			}//while
            //			this.lab_status.Text = "Ū������";
            //			this.lab_status.ForeColor = Color.Red;
        }
        #endregion

        //----------------------------------------------


        #region UI��event





        private void DBG_ReceiveData_Click(object sender, System.EventArgs e)
        {
            this.DBG_ReceiveData.Checked = !DBG_ReceiveData.Checked;
            bDebug = DBG_ReceiveData.Checked;
        }

        private void DBG_GEN2MSG_Click(object sender, System.EventArgs e)
        {
            this.DBG_GEN2MSG.Checked = !this.DBG_GEN2MSG.Checked;
        }

        private void DBG_Send2Server_Click(object sender, System.EventArgs e)
        {
            DBG_Send2Server.Checked = !DBG_Send2Server.Checked;
        }

        private void StopAddtoLB_Click(object sender, System.EventArgs e)
        {
            this.StopAddtoLB.Checked = !this.StopAddtoLB.Checked;
        }

        private void menuItem_Close_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void menuItem_DBGForm_Click(object sender, System.EventArgs e)
        {
            //			this.TaskDBG.Show();
        }

        private void menuItem_ComPort_Click(object sender, System.EventArgs e)
        {
            this.OnDisplay_PortStatus();
            this.myPortStatus.Show();
        }

        private void CCI_Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.LogFileWriter.Close();
            //this.TaskDBG.Close();
        }

        private void btn_NewFile_Click(object sender, System.EventArgs e)
        {
            this.LogFileWriter.WriteLine(";" + this.TaskName + "�G" + this.LogFileFullName + " file closed at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            this.LogFileWriter.Close();
            this.SetLogFileVersion(this.SetLogFileDateExt());
        }

        private void CCI_Form_Load(object sender, System.EventArgs e)
        {
            this.OnComPort_Init();  //�N�UComPort��l��
            this.OnAddTreeView();
        }

        private void CallCOMPortsOpen()
        {
            this.OnOpenComPort();
            foreach (LoadPort a in this.myLoadPort)
            {
                if (a != null)
                {
                    if (a.PortStatus == "Open")
                    {
                        this.SendMsgToTBC_Timer.Start();
                        break;
                    }
                }//if
            }//foreach
        }//................................CallCOMPortsOpen

        private void btn_PortOpen_Click(object sender, System.EventArgs e)
        {
            //			CallCOMPortsOpen();
            //
            //			System.Windows.Forms.TreeViewEventArgs e1 = new System.Windows.Forms.TreeViewEventArgs(this.TView_Port.SelectedNode);
            //			if(e1.Node == RootNode)
            //			{
            //				for(int i=0; i<this.TView_Port.SelectedNode.Nodes.Count; i++)
            //				{
            //					System.Windows.Forms.TreeViewEventArgs a = new System.Windows.Forms.TreeViewEventArgs(this.TView_Port.SelectedNode.Nodes[i]);
            //					this.TView_Port_AfterSelect(this, a);
            //					this.OnTreeView_Update(a);
            //				}				
            //			}
            //			else
            //			{				
            //				this.TView_Port_AfterSelect(this, e1);
            //				this.OnTreeView_Update(e1);
            //			}
            //			this.TView_Port.Focus();
            //			this.OnDisplay_PortStatus();
        }

        private void btn_PortClose_Click(object sender, System.EventArgs e)
        {
            //			this.OnCloseComPort();
            //			System.Windows.Forms.TreeViewEventArgs e1 = new System.Windows.Forms.TreeViewEventArgs(this.TView_Port.SelectedNode);
            //			if(e1.Node == RootNode)
            //			{
            //				for(int i=0; i<this.TView_Port.SelectedNode.Nodes.Count; i++)
            //				{
            //					System.Windows.Forms.TreeViewEventArgs a = new System.Windows.Forms.TreeViewEventArgs(this.TView_Port.SelectedNode.Nodes[i]);
            //					this.TView_Port_AfterSelect(this, a);
            //					this.OnTreeView_Update(a);
            //				}				
            //			}
            //			else
            //			{				
            //				this.TView_Port_AfterSelect(this, e1);
            //				this.OnTreeView_Update(e1);
            //			}
            //			this.TView_Port.Focus();
            //			this.OnDisplay_PortStatus();
            //
            //			foreach( LoadPort a in this.myLoadPort )
            //			{
            //				if( a != null)
            //				{
            //					if(a.PortStatus == "Close")
            //					{
            //						if(this.SendMsgToTBC_Timer.Enabled == true)
            //							this.SendMsgToTBC_Timer.Stop();
            //					}
            //					else
            //					{
            //						if(this.SendMsgToTBC_Timer.Enabled == false)
            //						{
            //							this.SendMsgToTBC_Timer.Start();
            //							break;
            //						}
            //					}
            //				}
            //			}
        }

        private void radBtn_Safe_Open_CheckedChanged(object sender, System.EventArgs e)
        {
            //			this.grpBox_Polling.Enabled = this.grpBox_RawType.Enabled = this.grpBox_SIM.Enabled = true;
        }

        private void radBtn_Safe_Close_CheckedChanged(object sender, System.EventArgs e)
        {
            //			this.grpBox_Polling.Enabled = this.grpBox_RawType.Enabled = this.grpBox_SIM.Enabled = false;
        }

        private void radBtn_Polling_Open_CheckedChanged(object sender, System.EventArgs e)
        {
            this.AutoPolling = true;
            foreach (LoadPort a in this.myLoadPort)
            {
                if (a != null)
                {
                    a.AutoPolling_Timer.Start();
                }
            }
        }

        private void radBtn_Polling_Close_CheckedChanged(object sender, System.EventArgs e)
        {
            this.AutoPolling = false;
            foreach (LoadPort a in this.myLoadPort)
            {
                if (a != null)
                {
                    a.AutoPolling_Timer.Close();
                }
            }
        }

        private void btn_Transmit_Click(object sender, System.EventArgs e)
        {
            //			byte[] Sim_str;
            //
            //			if(checkBox_CRCF6.Checked)
            //			{
            //				Sim_str = SYS.Gensys.Pack(SYS.CRC.PackCRC16(SYS.Tools.HexStringToByte(this.txb_Sim.Text.ToUpper() + "E005")));
            //			}
            //			else
            //			{
            //				Sim_str = SYS.Tools.HexStringToByte(this.txb_Sim.Text.ToUpper());
            //			}
            //
            //			if(radBtn_Receive.Checked)  //�^��
            //			{
            //				if(this.cbBox_LocSel.SelectedItem != null)  //����ܨ�����
            //				{				
            //					this.OnValidateReceiveComMessage(Sim_str, this.myLoadPort[SYS.LocConverter.Loc2Num(this.cbBox_LocSel.SelectedItem.ToString())]);
            //				}
            //				else  //�����������
            //				{
            //					this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "�����^�ǰT����,���������");
            //				}
            //			}
            //			else  //�e�ܲ{��
            //			{
            //				if(this.cbBox_LocSel.SelectedItem != null)  //����ܨ�����
            //				{
            //                    this.OnAddToControlQueue(Sim_str, myLoadPort[SYS.LocConverter.Loc2Num(this.cbBox_LocSel.SelectedItem.ToString())]);
            //				}
            //				else  //�����������
            //				{
            //					this.OnDisplayStringMessage(this.TaskName + " " + "WARNING - " + "�����ǰT���ܲ{����,���������");
            //				}
            //			}
        }

        private void txb_AutoPolling_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_AutoPolling.ForeColor = Color.Red;
        }

        private void txb_AutoReceive_TextChanged(object sender, System.EventArgs e)
        {
            txb_AutoReceive.ForeColor = Color.Red;
        }

        private void txb_AutoCFJudge_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_AutoCFJudge.ForeColor = Color.Red;
        }

        private void txb_DoMsgInterval_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_DoMsgInterval.ForeColor = Color.Red;
        }

        private void txb_DoMsgLimite_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_DoMsgLimite.ForeColor = Color.Red;
        }

        private void txb_SendtoTBCInterval_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_SendtoTBCInterval.ForeColor = Color.Red;
        }

        private void txb_SendtoTBCLimite_TextChanged(object sender, System.EventArgs e)
        {
            this.txb_SendtoTBCLimite.ForeColor = Color.Red;
        }

        private void txb_AutoPolling_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_AutoPolling.ForeColor = Color.Blue;
                txb_AutoPolling.BackColor = Color.LightGray;

                foreach (LoadPort a in this.myLoadPort)
                {
                    if (a != null)
                    {
                        a.AutoPolling_Timer.Interval = Int32.Parse(txb_AutoPolling.Text);
                    }
                }
            }
        }

        private void txb_AutoReceive_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_AutoReceive.ForeColor = Color.Blue;
                txb_AutoReceive.BackColor = Color.LightGray;

                foreach (LoadPort a in this.myLoadPort)
                {
                    if (a != null)
                    {
                        a.AutoRecevieMessage_Timer.Interval = Int32.Parse(txb_AutoReceive.Text);
                    }
                }
            }
        }

        private void txb_AutoCFJudge_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_AutoCFJudge.ForeColor = Color.Blue;
                txb_AutoCFJudge.BackColor = Color.LightGray;
                foreach (LoadPort a in this.myLoadPort)
                {
                    if (a != null)
                    {
                        a.AutoJudgeCF_Timer.Interval = Int32.Parse(txb_AutoCFJudge.Text);
                    }
                }
                intDataQPerGet = Convert.ToInt32(this.txb_AutoCFJudge.Text); //Hsinson951017�W�[��ScadaDBMF�i�ʺA�վ㦹�Ѽ�
            }
        }

        private void txb_DoMsgInterval_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_DoMsgInterval.ForeColor = Color.Blue;
                txb_DoMsgInterval.BackColor = Color.LightGray;
                this.DoMessageTimer.Interval = Int32.Parse(txb_DoMsgInterval.Text);
            }

        }

        private void txb_DoMsgLimite_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_DoMsgLimite.ForeColor = Color.Blue;
                txb_DoMsgLimite.BackColor = Color.LightGray;
                this.DoMsgLimite = Int32.Parse(txb_DoMsgLimite.Text);
            }
        }

        private void txb_SendtoTBCInterval_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_SendtoTBCInterval.ForeColor = Color.Blue;
                txb_SendtoTBCInterval.BackColor = Color.LightGray;
                this.SendMsgToTBC_Timer.Interval = Int32.Parse(txb_SendtoTBCInterval.Text);
            }
        }

        private void txb_SendtoTBCLimite_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)  //���UEnter
            {
                txb_SendtoTBCLimite.ForeColor = Color.Blue;
                txb_SendtoTBCLimite.BackColor = Color.LightGray;
                this.SendToTBCLimite = Int32.Parse(txb_SendtoTBCLimite.Text);
            }
        }

        private void btn_SaveParam_Click(object sender, System.EventArgs e)
        {
            try
            {
                SYS.Profile.SetValue(Filename, "Param", "AutoReceive", this.txb_AutoReceive.Text);
                SYS.Profile.SetValue(Filename, "Param", "AutoPolling", this.txb_AutoPolling.Text);
                SYS.Profile.SetValue(Filename, "Param", "AutoCFJudge", this.txb_AutoCFJudge.Text);
                SYS.Profile.SetValue(Filename, "Param", "SendToTBCInterval", this.txb_SendtoTBCInterval.Text);
                SYS.Profile.SetValue(Filename, "Param", "SendToTBCLimite", this.txb_SendtoTBCLimite.Text);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }

        private void btn_SelectPlayBackFile_Click(object sender, System.EventArgs e)
        {
            //			if(myOpenFileDialog.ShowDialog() == DialogResult.OK)
            //			{
            //				this.txb_PlayBackFile.Text = myOpenFileDialog.FileName;  
            //				this.PlayBackLogFile = new StreamReader(this.txb_PlayBackFile.Text);  //�}���ɮ�
            //				this.btn_Play.Enabled = true;
            //			}
            //			else
            //			{
            //				this.txb_PlayBackFile.Text = null;
            //			}
        }

        private void comboBox_Speed_SelectedValueChanged(object sender, System.EventArgs e)
        {
            //			switch(this.comboBox_Speed.SelectedIndex)
            //			{
            //				case 0: // ~32
            //					speed = 1F/32;
            //					break;
            //				case 1: // ~16
            //					speed = 1F/16;
            //					break;
            //				case 2: // ~8
            //					speed = 1F/8;
            //					break;
            //				case 3: // ~4
            //					speed = 1F/4;
            //					break;
            //				case 4: // ~2
            //					speed = 1F/2;
            //					break;
            //				case 5: // x1
            //					speed = 1;
            //					break;
            //				case 6: // x2
            //					speed = 2;
            //					break;
            //				case 7: // x4
            //					speed = 4;
            //					break;
            //				case 8: // x8
            //					speed = 8;
            //					break;
            //				case 9: // x16
            //					speed = 16;
            //					break;
            //				case 10: // x32
            //					speed = 32;
            //					break;
            //				case 11: // x0 �̹q�����p�����t
            //					speed = 0;
            //					break;				
            //				default: //x1
            //					speed = 1;
            //					break;
            //			}//switch
        }

        private void btn_Play_Click(object sender, System.EventArgs e)
        {
            //			if(this.PlayBackThread != null && (0 != (PlayBackThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested))))
            //			{
            //				PlayBackThread.Resume();  //�~�����Play
            //			}
            //			else
            //			{
            //				this.PlayBackThread = new Thread(new ThreadStart(PlayBack_Play));
            //				this.PlayBackThread.IsBackground = true;
            //				this.PlayBackThread.Start();
            //				this.btn_Pause.Enabled = this.btn_Stop.Enabled = true;
            //			}
            //			this.lab_status.Text = "���Ū����";
            //			this.lab_status.ForeColor = Color.Blue;
            //			
            //			if(this.SendMsgToTBC_Timer.Enabled == false)
            //			{
            //				this.SendMsgToTBC_Timer.Start();
            //			}
        }

        private void btn_Pause_Click(object sender, System.EventArgs e)
        {
            //			PlayBackThread.Suspend();
            //			this.lab_status.Text = "�Ȱ�...";
            //			this.lab_status.ForeColor = Color.OrangeRed;			
        }

        private void btn_Stop_Click(object sender, System.EventArgs e)
        {
            //			if(0 != (PlayBackThread.ThreadState & ( ThreadState.Suspended | ThreadState.SuspendRequested)))
            //			{
            //				PlayBackThread.Resume();
            //			}
            //			PlayBackThread.Abort();
            //			this.PlayBackLogFile.Close();  //�����ɮ�
            //			this.txb_PlayBackFile.Text = this.lab_status.Text = this.lab_message.Text = this.lab_Offset.Text = "";
            //			this.btn_Play.Enabled = this.btn_Pause.Enabled = this.btn_Stop.Enabled = false;
            //
            //			foreach( LoadPort a in this.myLoadPort )
            //			{
            //				if( a != null)
            //				{
            //					if(a.PortStatus == "Close")
            //					{
            //						if(this.SendMsgToTBC_Timer.Enabled == true)
            //							this.SendMsgToTBC_Timer.Stop();
            //					}
            //					else
            //					{
            //						if(this.SendMsgToTBC_Timer.Enabled == false)
            //						{
            //							this.SendMsgToTBC_Timer.Start();
            //							break;
            //						}
            //					}
            //				}
            //			}
        }

        private void DBG_Test_Click(object sender, System.EventArgs e)
        {
            //			this.TaskDBG.Test();
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void CCI_Form_Activated(object sender, System.EventArgs e)
        {
            if (this.AutoComPortOpen) //930628 Hsinson�s�W�ҰʧY�۰ʶ}�_Com-port�ö}�l�郞���s�u
            {
                CallCOMPortsOpen();
            }//if

        }


        #endregion

        private void tmRXM_Tick(object sender, System.EventArgs e)
        {
            try
            {
                //���s�}����

                if (this.intHour != DateTime.Now.Hour)
                {
                    intHour = DateTime.Now.Hour;
                    btn_NewFile_Click(new object(), new EventArgs());

                    //960725 SFI �C�ѹs��R�� Log File
                    if (intHour == 00)
                        this.DeleteLogFile();
                }


                this.statusBarPanel1.Text = DateTime.Now.ToString();
                if (oldMin != DateTime.Now.Minute)
                {
                    oldMin = DateTime.Now.Minute;
                    if (this.blnfirstTime)
                    {
                        //this.tmRXM.Interval= 5000000;
                        fnFrontendDataCopyInit();
                        blnfirstTime = false;

                    }
                    else
                        this.fnGetIntBase();
                        this.fnGetMultipleAlarm();
                }

                // �Y����ˬd RTDB_RX ��ƪ����ɶ�
                // (�ˬd��ƪ��O�_�� 100 �Ѫ��R������)
                if (DateTime.Now >= m_DTNextCheckRTDB_RX_TableTime && m_blnLocalDB_OneDay_Enable == true)
                {
                    // �ˬd RTDB_RX ������ƪ��O�_�W�X�O�s����(�ܤ֫O�d100��)
                    CheckRTDB_RXByDayTable();

                    // �]�w�U���ˬd���ɶ�(�j��A�@�ˬd)
                    m_DTNextCheckRTDB_RX_TableTime = m_DTNextCheckRTDB_RX_TableTime.AddDays(1);
                }

                //�����Ʈw�ƻs�@�~
                fnFrontendDataCopy();
                //�]�wOnLine
                //fnSetOvLine();


            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("��ƽƻs�@�~���`: " + Ex.Message);
                this.tmRXM.Interval = intIntervalGetDEDB;
                this.tmRXM.Enabled = true;
            }

        }

        // ���ի��w��Table�O�_�s�b
        public bool IsExistTable(ref SqlConnection SqlConn, string strTableName)
        {
            string strSQL = "SELECT TOP 1 * FROM " + strTableName;
            bool bIsExist = false;

            DataSet mydataset = new DataSet();

            try
            {
                SqlDataAdapter da;

                da = new SqlDataAdapter(strSQL, SqlConn);
                da.SelectCommand = new SqlCommand(strSQL, SqlConn);
                da.Fill(mydataset);

                bIsExist = true;
            }
            catch (Exception Ex)
            {
                bIsExist = false;
            }

            return bIsExist;
        }

        // �ˬd RTDB_RX ������ƪ��O�_�W�X�O�s����(�ܤ֫O�d100��)
        public bool CheckRTDB_RXByDayTable()
        {
            string strTableName = Profile.GetValue("ScadaDBMF.ini", "RTDBSQLServerOneDay", "RXTable");
            string strDelTableName = "";


            DateTime stTime1;
            DateTime stTime2;
            double dDelDayNumber = -102; // �O�d100�Ѫ���� 


            stTime1 = DateTime.Now.AddDays(dDelDayNumber);

            stTime2 = stTime1;


            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;

            try
            {

                // �]�w��Ʈw�s���r��
                strConn = @"server=" + this.m_strLocalSQLServer_OneDay_IP + ";uid=" + this.m_strLocalSQLServer_OneDay_User + ";pwd=" + this.m_strLocalSQLServer_OneDay_Password + ";database=" + this.m_strLocalSQLServer_OneDay_DataBase;
                conDB = new SqlConnection(strConn);
                conDB.Open();
                CmdDB = conDB.CreateCommand();

                if (this.DBG_ReceiveData.Checked)
                {
                    this.OnDisplayStringMessage("���n�ˬdRTDB ��m (" + this.m_strLocalSQLServer_OneDay_IP + ") RX ��ƪ��O�_�W�X�O�d����(�O�d���100�Ѫ���ƪ�)");
                }

                myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                CmdDB.Transaction = myTrans;


                stTime2 = stTime1;

                bool bTableExist = false;

                for (int i = 1; i <= 10; i++)
                {
                    strDelTableName = strTableName + stTime2.ToString("yyyyMMdd");

                    bTableExist = false;

                    // ����Table�O�_�s�b
                    try
                    {
                        strSQL = "SELECT TOP 1 * FROM " + strDelTableName;

                        CmdDB.CommandText = strSQL;
                        CmdDB.ExecuteNonQuery();

                        // Table�s�b
                        bTableExist = true;
                    }
                    catch (Exception Ex)
                    {
                        // Table���s�b
                        bTableExist = false;

                    }

                    // �YTable���s�b�A�h�����ˬd
                    if (bTableExist == false)
                    {
                        break;
                    }


                    // �Y��ƪ��s�b�A�h�R����ƪ�
                    strSQL = " if exists (select * from sysobjects where id = object_id(N'[dbo].[";
                    strSQL = strSQL + strDelTableName;
                    strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                    strSQL = strSQL + " DROP TABLE [dbo].[";
                    strSQL = strSQL + strDelTableName;
                    strSQL = strSQL + "]";

                    CmdDB.CommandText = strSQL;
                    CmdDB.ExecuteNonQuery();

                    stTime2 = stTime1.AddDays(-i);


                    this.OnDisplayStringMessage("�R����ƪ�: RTDB��m(" + this.m_strLocalSQLServer_OneDay_IP + ") �R����ƪ�:" + strDelTableName + " (����ƪ��A�W�X100�Ѥ��O�d����)");
                }

                myTrans.Commit();
                conDB.Close();


            }
            catch (Exception Ex)
            {
                this.OnDisplayStringMessage("�R����ƪ����ѡG�@RTDB��m(" + this.m_strLocalSQLServer_OneDay_IP + ") ��ƪ� " + strDelTableName + "�R������");

                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;

                return false;
            }

            return true;
        }


        //-----------------------------------------------------------------------------------------------------------------------------------//

        //dvData.RowFilter="Content LIKE '[[]WNetDc]%'";



        void fnSetNhEVTDBMoveDataOneDay(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            int i;
            string seq;

            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Source LIKE 'ID.BT.OCCNeihu.EVTDBN'";

            try
            {
                lock (this)
                {
                    // Ū��Table�W��
                    string strTableName = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "RXTable");

                    strConn = @"server=" + this.m_NhEVTDBNMoveDataOneDay_IP + ";uid=" + this.m_NhEVTDBNMoveDataOneDay_User + ";pwd=" + this.m_NhEVTDBNMoveDataOneDay_Password + ";database=" + this.m_NhEVTDBNMoveDataOneDay_DaraBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();

                    if (this.DBG_ReceiveData.Checked)
                    {
                        this.OnDisplayStringMessage("���n�g�JRTDB     ��m(" + this.m_NhEVTDBNMoveDataOneDay_IP + ") " + this.m_strLocalSQLServer_OneDay_DataBase + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.ToTable().Rows.Count + " ��");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;

                    DateTime DateTimeRcv;
                    string strTimeRcv;
                    string strNodeTime;
                    string Contime = "";
                    string strCon;								//Contnt"��줺�e"
                    bool bTableExist = false;						//�ˬd��ƪ��O�_�s�b true=�s�b;false=���s�b 
                    string strDBExist = "";
                    string strNodeDay = "";							//�s���I���yyyyMMdd

                    for (i = 0; i < dvData.Count; i++)
                    {
                        seq = dvData.ToTable().Rows[i]["Seq"].ToString();

                        strCon = (string)dvData.ToTable().Rows[i]["Content"];
                        DateTimeRcv = System.DateTime.Parse(dvData.ToTable().Rows[i]["TimeRcv"].ToString());
                        strTimeRcv = DateTimeRcv.ToString("yyyy/MM/dd HH:mm:ss");

                        //���oNodeTime�ثe��m�T�w
                        try
                        {
                            //"6/15/2009 6:56:50 PM"   ----->  "2009/6/15 �U�� 06:56:50"
                            strNodeTime = DateTime.Parse(strCon.Substring(0, 23).Trim()).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        catch (Exception Ex) //�ɶ��榡����
                        {
                            continue;
                        }
                        //���oNodeTime�ثe��m�T�w����������
                        strNodeDay = fnGetDayStr(strNodeTime);
                        this.m_NhEVTDBNMoveDataOneDay_RXTable = strTableName + strNodeDay;

                        //�P�_���L����Ƨ�
                        try
                        {
                            strSQL = "SELECT TOP 1 * FROM " + this.m_NhEVTDBNMoveDataOneDay_RXTable;

                            CmdDB.CommandText = strSQL;
                            CmdDB.ExecuteNonQuery();
                            bTableExist = true;

                        }
                        catch (Exception Ex)
                        {	//�S������Ƨ�
                            bTableExist = false;
                        }

                        if (bTableExist == false)
                        {
                            this.m_NhEVTDBColNameList.Clear();
                            this.m_NhEVTDBColTypList.Clear();
                            this.m_NhEVTDBColLengthList.Clear();
                            this.m_NhEVTDBListAllLength = 0;
                            for (this.m_NhEVTDBListLength = 0; this.m_NhEVTDBListLength < 1000; this.m_NhEVTDBListLength++)
                            {

                                this.m_NhEVTDBstrColName = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColName" + this.m_NhEVTDBListLength.ToString("000"));
                                this.m_NhEVTDBstrColTyp = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColTyp" + this.m_NhEVTDBListLength.ToString("000"));
                                this.m_NhEVTDBstrColLength = Profile.GetValue("ScadaDBMF.ini", "NhEVTDBNMoveDataOneDay", "ColLength" + this.m_NhEVTDBListLength.ToString("000"));

                                //�̫�@����Ʃ�����T����
                                if (this.m_NhEVTDBstrColName == null || this.m_NhEVTDBstrColTyp == null || this.m_NhEVTDBstrColLength == null)
                                {
                                    break;
                                }
                                //�ˬd��ƫ��A
                                this.m_NhEVTDBstrColTyp = checkColTyp(this.m_NhEVTDBstrColTyp);
                                if (this.m_NhEVTDBstrColTyp == null)
                                {
                                    throw new Exception("ScadaDBMF.ini / NhEVTDBNMoveDataOneDay / ColTyp " + this.m_NhEVTDBListLength.ToString("000") + "�榡�]�w���~");
                                }
                                this.m_NhEVTDBColNameList.Add(this.m_NhEVTDBstrColName);
                                this.m_NhEVTDBColTypList.Add(this.m_NhEVTDBstrColTyp);
                                this.m_NhEVTDBColLengthList.Add(this.m_NhEVTDBstrColLength);
                                this.m_NhEVTDBListAllLength = this.m_NhEVTDBListAllLength + Convert.ToInt16(this.m_NhEVTDBstrColLength);
                            }


                            this.OnDisplayStringMessage("�ƥ����P���e��ƪ�������P�A�۰ʫإ�(Create)�s��ƪ�" + m_NhEVTDBNMoveDataOneDay_RXTable);

                            strSQL = " if not exists (select * from sysobjects where id = object_id(N'[dbo].[";
                            strSQL = strSQL + this.m_NhEVTDBNMoveDataOneDay_RXTable;
                            strSQL = strSQL + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ";
                            strSQL = strSQL + " CREATE TABLE [dbo].[";
                            strSQL = strSQL + this.m_NhEVTDBNMoveDataOneDay_RXTable;
                            strSQL = strSQL + "] (";
                            strSQL = strSQL + "[Seq] [bigint] IDENTITY(1, 1) PRIMARY KEY  , ";
                            strSQL = strSQL + "[TimeRcv] [datetime], ";
                            strSQL = strSQL + "[Source] [varchar] (50), ";

                            for (int ColIndex = 0; ColIndex < this.m_NhEVTDBListLength; ColIndex++)
                            {
                                if (this.m_NhEVTDBColTypList[ColIndex].ToString() == "datetime")
                                {
                                    strSQL = strSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "][" + this.m_NhEVTDBColTypList[ColIndex] + "],";
                                    continue;

                                }
                                strSQL = strSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "][" + this.m_NhEVTDBColTypList[ColIndex] + "](" + this.m_NhEVTDBColLengthList[ColIndex] + "),";

                            }

                            strSQL = strSQL + "[Region] [varchar] (500), ";
                            strSQL = strSQL + "[Segment] [varchar] (500), ";
                            strSQL = strSQL + "[TRN] [varchar] (500) ,";
                            strSQL = strSQL + "[VEH] [varchar] (500) ,";
                            strSQL = strSQL + "[CAR] [varchar] (500) ,";
                            strSQL = strSQL + "[ON/OFF] [tinyint] DEFAULT 1";
                            strSQL = strSQL + ")";

                            CmdDB.CommandText = strSQL; //����إ߷s��ƪ����O
                            CmdDB.ExecuteNonQuery();
                            this.OnDisplayStringMessage("SQL���O=" + strSQL);
                        }


                        //����������������        NhEVTDBColNameList ����r        ����������������//
                        string strVEH;
                        string strTRN;
                        string strREG = "";
                        string strSEG = "";
                        string strOEG = "";
                        string strCAR;
                        int ChrIndex;
                        int keybool = 1;

                        strVEH = getcontentevt(strCon, "VEH");
                        strTRN = getcontentevt(strCon, "TRN");

                        strOEG = getcontentevt(strCon, "R/S/O");
                        if (strOEG != "")
                        {

                            strREG = strOEG.Substring(0, strOEG.IndexOf("/"));
                            strSEG = strOEG.Substring(strOEG.IndexOf("/") + 1, strOEG.LastIndexOf("/") - strOEG.IndexOf("/") - 1);
                            strOEG = strOEG.Remove(0, strOEG.LastIndexOf("/") + 1);

                            for (ChrIndex = 0; ChrIndex < strOEG.Length; ChrIndex++)
                            {
                                if (char.IsDigit(strOEG, ChrIndex) != true)
                                {
                                    break;
                                }
                            }

                            strOEG = strOEG.Substring(0, ChrIndex);

                        }
                        else
                        {
                            strSEG = getcontentevt(strCon, "R/S");
                            if (strSEG != "")
                            {
                                strREG = strSEG.Substring(0, strSEG.LastIndexOf("/"));
                                strSEG = strSEG.Remove(0, strSEG.LastIndexOf("/") + 1);

                                for (ChrIndex = 0; ChrIndex < strSEG.Length; ChrIndex++)
                                {
                                    if (char.IsDigit(strSEG, ChrIndex) != true)
                                    {
                                        break;
                                    }
                                }

                                strSEG = strSEG.Substring(0, ChrIndex);
                            }
                        }
                        strCAR = getcontentevt(strCon, "Car");
                        if (strCAR == "A" || strCAR == "B") { }
                        else
                        {
                            strCAR = strCon.Remove(0, strCon.IndexOf("Car") + 3);
                            strCAR = this.getcontentevt(strCAR, "Car");
                        }

                        for (ChrIndex = 0; ChrIndex < this.m_NhEVTDBKeyWord.Length; ChrIndex++)
                        {
                            if (strCon.IndexOf(this.m_NhEVTDBKeyWord[ChrIndex]) != -1 && strCon.IndexOf(this.m_NhEVTDBKeyWord[ChrIndex]) != 0)
                            {
                                keybool = 0;
                                break;
                            }
                        }


                        //������������������        NhEVTDBColNameList ����r         ����������������//


                        //�������������W�@����������sŪ�����Τ�k
                        if (strNodeDay != this.m_NhEVTDBstrTime_LastEvent)
                        {
                            //int COLCount = this.m_NhEVTDBListLength ;  //Scada�ۭq����

                            //�j����Ƨ�������T
                            //20140220 SFI �s�W order by dbo.syscolumns.colorder
                            strSQL = "SELECT    dbo.syscolumns.name AS sColumnsName,dbo.syscolumns.prec AS iColumnsLength,dbo.systypes.name + '' AS sColumnsType  " +
                                "FROM    dbo.sysobjects INNER JOIN dbo.syscolumns ON dbo.sysobjects.id = dbo.syscolumns.id INNER JOIN  dbo.systypes ON dbo.syscolumns.xusertype = dbo.systypes.xusertype  " +
                                "WHERE    dbo.sysobjects.name = '" + this.m_NhEVTDBNMoveDataOneDay_RXTable + "' order by dbo.syscolumns.colorder ";

                            DataSet Da = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(strSQL, conDB);
                            da.SelectCommand = new SqlCommand(strSQL, conDB, myTrans);
                            da.Fill(Da);

                            if (Da.Tables[0].Rows.Count != 0)
                            {
                                this.m_NhEVTDBColNameList.Clear();
                                this.m_NhEVTDBColTypList.Clear();
                                this.m_NhEVTDBColLengthList.Clear();
                                this.m_NhEVTDBListAllLength = 0;
                                this.m_NhEVTDBListLength = 0;
                                for (int COLIdenx = 3; COLIdenx < Da.Tables[0].Rows.Count; COLIdenx++)
                                {
                                    if (Da.Tables[0].Rows[COLIdenx][0].ToString() == "Region")
                                    {
                                        break;
                                    }
                                    this.m_NhEVTDBstrColName = Da.Tables[0].Rows[COLIdenx][0].ToString();
                                    this.m_NhEVTDBstrColTyp = Da.Tables[0].Rows[COLIdenx][2].ToString();
                                    this.m_NhEVTDBstrColLength = Da.Tables[0].Rows[COLIdenx][1].ToString();

                                    this.m_NhEVTDBColNameList.Add(this.m_NhEVTDBstrColName);
                                    this.m_NhEVTDBColTypList.Add(this.m_NhEVTDBstrColTyp);
                                    this.m_NhEVTDBColLengthList.Add(this.m_NhEVTDBstrColLength);

                                    this.m_NhEVTDBListLength++;
                                    this.m_NhEVTDBListAllLength = this.m_NhEVTDBListAllLength + Convert.ToInt16(this.m_NhEVTDBstrColLength);
                                }
                            }
                        }


                        //�}�l���Φr���������������

                        //�N�r��񺡤��Φ줸
                        strCon = strCon.PadRight(this.m_NhEVTDBListAllLength + this.m_NhEVTDBListLength);

                        string FSQL = "";
                        string LSQL = "";
                        bool b = true;


                        for (int ColIndex = 0; ColIndex < this.m_NhEVTDBListLength; ColIndex++)
                        {
                            //�̫�@��
                            if (ColIndex == this.m_NhEVTDBListLength - 1 && this.m_NhEVTDBColTypList[ColIndex].ToString() != "datetime")
                            {


                                if (Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) >= strCon.Length)
                                {
                                    //�̫�@��������j��strCon�r���Ƥ����Φr��
                                    FSQL = FSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "] ,";
                                    LSQL = LSQL + strCon.Trim() + "', '";
                                }
                                else
                                {
                                    FSQL = FSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "] ,";
                                    LSQL = LSQL + strCon.Substring(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex])).Trim() + "', '";

                                    strCon = strCon.Remove(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) + 1);
                                }
                            }//���Ӥp�g���i�h
                            else if (Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) > strCon.Length && this.m_NhEVTDBColTypList[ColIndex].ToString() != "datetime")
                            {
                                this.OnDisplayStringMessage(strNodeDay + "scadaDBMF�r���]�w���~ [NhEVTDBNMoveDataOneDay] " + this.m_NhEVTDBColNameList[ColIndex] + "�����p��" + strCon.Length + "�r��");
                                b = false;
                                break;

                            }//��J�榡���ɶ��B���Φr�������T
                            else if (Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) > strCon.Length && this.m_NhEVTDBColTypList[ColIndex].ToString() == "datetime")
                            {
                                b = false;
                                break;
                            }//��J�ɶ����
                            else if (this.m_NhEVTDBColTypList[ColIndex].ToString() == "datetime")
                            {

                                try
                                {
                                    //"6/15/2009 6:56:50 PM"   ----->  "2009/6/15 �U�� 06:56:50"
                                    Contime = DateTime.Parse(strCon.Substring(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex])).Trim()).ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                catch (Exception Ex) //�ɶ��榡����
                                {
                                    b = false;
                                    break;
                                }
                                FSQL = FSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "] ,";
                                LSQL = LSQL + Contime + "', '";

                                strCon = strCon.Remove(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) + 1);
                            }
                            else
                            {
                                FSQL = FSQL + "[" + this.m_NhEVTDBColNameList[ColIndex] + "] ,";
                                LSQL = LSQL + strCon.Substring(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex])).Trim() + "', '";
                                strCon = strCon.Remove(0, Convert.ToInt32(this.m_NhEVTDBColLengthList[ColIndex]) + 1);		//�h��@�ӪŮ��
                            }
                        }
                        if (b != false)
                        {
                            strSQL = "INSERT INTO " + this.m_NhEVTDBNMoveDataOneDay_RXTable + " ( TimeRcv,Source," + FSQL + "Region,Segment,TRN,VEH,CAR,[ON/OFF])VALUES ('" + strTimeRcv + "','" + dvData.ToTable().Rows[i]["Source"] + "','" + LSQL + strREG + "', '" + strSEG + "', '" + strTRN + "', '" + strVEH + "', '" + strCAR + "', '" + keybool + "')";
                            CmdDB.CommandText = strSQL;
                            CmdDB.ExecuteNonQuery();
                            //�����g�J�ɶ�
                            this.m_NhEVTDBstrTime_LastEvent = strNodeDay;
                        }


                    }
                    strDBExist = conDB.State.ToString();
                    myTrans.Commit();
                    conDB.Close();
                    this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.m_NhEVTDBNMoveDataOneDay_IP + ") " + this.m_NhEVTDBNMoveDataOneDay_RXTable + " ��Ʈw: ��Ƶ��Ʀ@ " + dvData.ToTable().Rows.Count + " ��");

                }

            }
            catch (Exception Ex)
            {

                this.OnDisplayStringMessage("�g�JRTDB    ��m(" + this.m_NhEVTDBNMoveDataOneDay_IP + ") " + this.m_NhEVTDBNMoveDataOneDay_RXTable + " ��Ʈw �@�~���`: ��Ƶ��Ʀ@ " + dvData.ToTable().Rows.Count + " ��");
                CmdDB.Connection.Close();
                //conDB.Close();
                throw Ex;
            }

        }

        //�j���r�ꤺ������r
        string getcontentevt(string con, string evtstr)
        {
            string strevt;
            int constart;
            int evtend;
            constart = con.IndexOf(evtstr);
            if (constart == -1)
            {
                return "";
            }
            con = con.Remove(0, constart + evtstr.Length);
            con = con.Trim();
            evtend = con.IndexOf(" ");
            if (evtend == -1)
            {
                evtend = con.Length;
            }
            con = con.Substring(0, evtend);
            strevt = getcontentevt(con, evtstr);
            if (strevt == "")
            {
                return con;
            }
            else
            {
                return strevt;
            }
        }

        //�ˬd��ƫ��A
        string checkColTyp(string strColTyp)
        {
            strColTyp = strColTyp.Trim().ToLower();
            //��ƫ��A��datetime
            if (strColTyp.IndexOf("datetime") != -1)
            {
                return "datetime";
            }
            //��ƫ��A��bigint
            else if (strColTyp.IndexOf("bigint") != -1)
            {
                return "bigint";
            }
            //��ƫ��A��varchar
            else if (strColTyp.IndexOf("varchar") != -1)
            {
                return "varchar";
            }
            //��ƫ��A�W�ٿ��~
            else
            {
                return null;
            }

        }

    }//class CCI_Form
}//namespace CCI