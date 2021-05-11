using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using TBC;
using SYS;
using SCADA_RX;

namespace ScadaRXM.Module
{
    public class PTTRTCDCS_Event
    {
        public string DeviceName { set; get; }
        public int Group { set; get; }
        public DateTime LogTime { set; get; }
        public string DeviceType { set; get; }
        public int SignalType { set; get; }
        public string DeviceDescription { set; get; }
        public string Enter { set; get; }
        public string Exit { set; get; }
        public int SequenceNo { set; get; }
        public int Status { set; get; }
    }

    public class PTTRTCDCS_EventMove
    {
        public static event CCI_Form.Display_StringMessage_Handler callback;
        public static string strPTTRTCDCS_Event_DBSQLServer_IP { set; get; }
        public static string strPTTRTCDCS_Event_DBSQLServer_DataBase { set; get; }
        public static string strPTTRTCDCS_Event_DBSQLServer_User { set; get; }
        public static string strPTTRTCDCS_Event_DBSQLServer_Password { set; get; }
        public static string strPTTRTCDCS_Event_DBSQLServer_Table { set; get; }
        public static bool blnPTTRTCDCS_Event_DBEnable { set; get; }

        public static bool blnDebug { private set { } get { return CCI_Form.bDebug; } }

        static object objLock = new object();


        class declare
        {
            public const string DeviceName = "DeviceName";
            public const string Group = "Group";
            public const string LogTime = "LogTime";
            public const string DeviceType = "DeviceType";
            public const string SignalType = "SignalType";
            public const string DeviceDescription = "DeviceDescription";
            public const string Enter = "Enter";
            public const string Exit = "Exit";
            public const string SequenceNo = "SequenceNo";
            public const string Status = "Status";

        }

        public static void Move(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            string strContent;

            //20121114 SFI 判斷是否有塔的資料
            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]PTTRTCDCS]%'";

            try
            {
                lock (objLock)
                {

                    strConn = @"server=" + strPTTRTCDCS_Event_DBSQLServer_IP + ";uid=" + strPTTRTCDCS_Event_DBSQLServer_User + ";pwd=" + strPTTRTCDCS_Event_DBSQLServer_Password + ";database=" + strPTTRTCDCS_Event_DBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();


                    if (blnDebug)
                    {
                        DisplayMessage("正要寫入PTTRTCDCS DB    位置(" + strPTTRTCDCS_Event_DBSQLServer_IP + ") " + strPTTRTCDCS_Event_DBSQLServer_Table + " 資料庫: 資料筆數共 " + vdtData.Rows.Count + " 筆");
                    }

                    myTrans = conDB.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");
                    CmdDB.Transaction = myTrans;


                    DateTime DateTimeRcv;

                    string[] msg;
                    //string Station = "";
                    //string Platform = "";
                    //string CountDown = "";
                    //string Destination = "";
                    //string UpdateTime = "";
                    PTTRTCDCS_Event TRTCDCS = new PTTRTCDCS_Event();
                    SortedList list = new SortedList();


                    for (int i = 0; i < dvData.Count; i++)
                    {

                        msg = dvData[i].Row["Content"].ToString().Split(';');

                        #region 字串切割
                        foreach (string m in msg)
                        {
                            string[] tmp;
                            tmp = m.Split('=');

                            //移除 [VehDistance] 這個前置符號
                            int pos = 0;
                            pos = tmp[0].IndexOf("PTTRTCDCS]");
                            if (pos > 0)
                            {
                                tmp[0] = tmp[0].Remove(0, "[PTTRTCDCS]".Length);
                            }


                            switch (tmp[0])
                            {
                                case declare.DeviceName:
                                    TRTCDCS.DeviceName = tmp[1];
                                    break;
                                case declare.Group:
                                    TRTCDCS.Group = int.Parse( tmp[1].ToString()) ;
                                    break;
                                case declare.LogTime:
                                    TRTCDCS.LogTime = DateTime.Parse(tmp[1]);
                                    break;
                                case declare.DeviceType:
                                    TRTCDCS.DeviceType = tmp[1];
                                    break;
                                case declare.SignalType:
                                    TRTCDCS.SignalType = int.Parse(tmp[1]);
                                    break;
                                case declare.DeviceDescription:
                                    TRTCDCS.DeviceDescription = tmp[1];
                                    break;
                                case declare.Enter:
                                    TRTCDCS.Enter = tmp[1];
                                    break;
                                case declare.Exit:
                                    TRTCDCS.Exit = tmp[1];
                                    break;
                                case declare.SequenceNo:
                                    TRTCDCS.SequenceNo = int.Parse( tmp[1]);
                                    break;
                                case declare.Status:
                                    TRTCDCS.Status = int.Parse(tmp[1]);
                                    break;
                                default:
                                    break;
                            }

                        }
                        #endregion


                        Insert(TRTCDCS, CmdDB);

                    }

                    myTrans.Commit();
                    conDB.Close();
                    DisplayMessage("寫入TRTCDCS DB    位置(" + strPTTRTCDCS_Event_DBSQLServer_IP + ") " + strPTTRTCDCS_Event_DBSQLServer_Table + " 資料庫: 資料筆數共 " + dvData.Count + " 筆");
                }

            }
            catch (Exception Ex)
            {
                //this.OnDisplayStringMessage("寫入EMDSCountDown DB    位置(" + this.strEMDSCountDown_DBSQLServer_IP + ") " + this.strEMDSCountDown_DBSQLServer_Table + " 資料庫 作業異常: 資料筆數共 " + dvData.Count + " 筆");
                DisplayMessage("寫入TRTCDCS DB    位置(" + strPTTRTCDCS_Event_DBSQLServer_IP + ") " + strPTTRTCDCS_Event_DBSQLServer_Table + " 資料庫 作業異常: 資料筆數共 " + dvData.Count + " 筆");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        public static bool Insert(PTTRTCDCS_Event Event , SqlCommand CmdDB)
        {

            string strSQL = @"INSERT INTO {0} ([DeviceName],[Group],[LogTime],[DeviceType],[SignalType],[DeviceDescription],[Enter],[Exit],[SequenceNo],[Status])
                                   VALUES     (@DeviceName ,@Group ,@LogTime ,@DeviceType ,@SignalType ,@DeviceDescription ,@Enter ,@Exit ,@SequenceNo ,@Status )";

            strSQL = string.Format(strSQL, strPTTRTCDCS_Event_DBSQLServer_Table);

            CmdDB.CommandText = strSQL;
            CmdDB.Parameters.Clear();
            CmdDB.Parameters.Add("@DeviceName", Event.DeviceName);
            CmdDB.Parameters.Add("@Group", Event.Group);
            CmdDB.Parameters.Add("@LogTime", Event.LogTime);
            CmdDB.Parameters.Add("@DeviceType", Event.DeviceType);
            CmdDB.Parameters.Add("@SignalType", Event.SignalType);
            CmdDB.Parameters.Add("@DeviceDescription", Event.DeviceDescription);
            CmdDB.Parameters.Add("@Enter", Event.Enter);
            CmdDB.Parameters.Add("@Exit", Event.Exit);
            CmdDB.Parameters.Add("@SequenceNo", Event.SequenceNo);
            CmdDB.Parameters.Add("@Status", Event.Status);

            CmdDB.CommandText = strSQL;

            int rs = CmdDB.ExecuteNonQuery();

            return (rs <= 0 ? true : false);
        }

        public static void DisplayMessage(string Message)
        {
            if (callback != null)
            {
                callback(Message);
            }
        }

    }



}
