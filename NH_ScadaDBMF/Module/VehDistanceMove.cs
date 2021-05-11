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

    public class VehDistance
    {
        public DateTime NodeTime {set;get;}             //資料產生時間
        public string NodeName {set;get;}               //資料來源主機名稱
        public DateTime Time { set; get; }              //資料時間
        public int VehID {set;get;}                     //列車車號
        public float DailyDistance {set;get;}           //列車本日里程數
        public float CurrDistance { set; get; }         //列車總里程數

    }
   
    

    public static class VehDistanceMove
    {
        public static event CCI_Form.Display_StringMessage_Handler callback;

        public static string strVehDistance_DBSQLServer_IP {set;get;}
        public static string strVehDistance_DBSQLServer_DataBase {set;get;}
        public static string strVehDistance_DBSQLServer_User { set; get; }
        public static string strVehDistance_DBSQLServer_Password { set; get; }
        public static string strVehDistance_DBSQLServer_Table { set; get; }
        public static bool blnVehDistance_DBEnable { set; get; }

        public static bool blnDebug { private set { } get { return CCI_Form.bDebug; } }

        static object objLock = new object();

        //VehDistance 資料來源欄位名稱宣告
        class Declare
        {
            public const string NodeTime = "NodeTime";
            public const string NodeName = "NodeName";
            public const string Time = "Time";
            public const string VehID = "VehID";
            public const string DailyDistance = "DailyDistance";
            public const string CurrDistance = "CurrDistance";

        }

        public static void Move(DataTable vdtData)
        {
            SqlConnection conDB = null;
            SqlCommand CmdDB = null;
            string strConn;
            string strSQL;
            SqlTransaction myTrans;
            string strContent;

            //20121114 SFI 判斷是否有PSD的資料
            DataView dvData = new DataView(vdtData);
            dvData.RowFilter = "Content LIKE '[[]NHVehDistance]%'";

            try
            {
                lock (objLock)
                {

                    strConn = @"server=" + strVehDistance_DBSQLServer_IP + ";uid=" + strVehDistance_DBSQLServer_User + ";pwd=" + strVehDistance_DBSQLServer_Password + ";database=" + strVehDistance_DBSQLServer_DataBase;
                    conDB = new SqlConnection(strConn);
                    conDB.Open();
                    CmdDB = conDB.CreateCommand();


                    if (blnDebug)
                    {
                        DisplayMessage("正要寫入VehDistance DB    位置(" + strVehDistance_DBSQLServer_IP + ") " + strVehDistance_DBSQLServer_Table + " 資料庫: 資料筆數共 " + vdtData.Rows.Count + " 筆");
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
                    VehDistance vehDist = new VehDistance();
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
                            pos = tmp[0].IndexOf("VehDistance]");
                            if (pos > 0)
                            {
                                tmp[0] = tmp[0].Remove(0, "[VehDistance]".Length);
                            }


                            switch (tmp[0])
                            {
                                case Declare.NodeTime:
                                    vehDist.NodeTime =DateTime.Parse(tmp[1]);
                                    break;
                                case Declare.NodeName:
                                    vehDist.NodeName = tmp[1];
                                    break;
                                case Declare.Time:
                                    vehDist.Time = DateTime.Parse(tmp[1]);
                                    break;
                                case Declare.VehID:
                                    vehDist.VehID = int.Parse(tmp[1]);
                                    break;
                                case Declare.DailyDistance:
                                    vehDist.DailyDistance = float.Parse(tmp[1]);
                                    break;
                                case Declare.CurrDistance:
                                    vehDist.CurrDistance = float.Parse(tmp[1]);
                                    break;

                                default:
                                    break;
                            }

                        }
                        #endregion



                        if (Update(vehDist , CmdDB) )
                        {
                            Insert(vehDist, CmdDB);
                        }
                    }

                    myTrans.Commit();
                    conDB.Close();
                    DisplayMessage("寫入VehDistance DB    位置(" + strVehDistance_DBSQLServer_IP + ") " + strVehDistance_DBSQLServer_Table + " 資料庫: 資料筆數共 " + dvData.Count + " 筆");
                }

            }
            catch (Exception Ex)
            {
                //this.OnDisplayStringMessage("寫入EMDSCountDown DB    位置(" + this.strEMDSCountDown_DBSQLServer_IP + ") " + this.strEMDSCountDown_DBSQLServer_Table + " 資料庫 作業異常: 資料筆數共 " + dvData.Count + " 筆");
                DisplayMessage("寫入VehDistance DB    位置(" + strVehDistance_DBSQLServer_IP + ") " + strVehDistance_DBSQLServer_Table + " 資料庫 作業異常: 資料筆數共 " + dvData.Count + " 筆");
                CmdDB.Connection.Close();
                conDB.Close();
                throw Ex;
            }

        }

        public static bool Update(VehDistance veh, SqlCommand CmdDB)
        {
            string strSQL = @"UPDATE {0}
                                        SET [EventTime] = @EventTime
                                          ,[CurrentDistance] = @CurrentDistance
                                          ,[Distance] = @Distance
                                        WHERE [VehID] = @VehID";

            strSQL = string.Format(strSQL, strVehDistance_DBSQLServer_Table);

            CmdDB.CommandText = strSQL;
            CmdDB.Parameters.Clear();
            CmdDB.Parameters.Add("@EventTime", veh.Time);
            CmdDB.Parameters.Add("@CurrentDistance", veh.CurrDistance);
            CmdDB.Parameters.Add("@Distance", veh.DailyDistance);
            CmdDB.Parameters.Add("@VehID", veh.VehID);
            int rs = CmdDB.ExecuteNonQuery();

            return (rs <= 0 ? true : false );
        }

        public static bool Insert(VehDistance veh, SqlCommand CmdDB)
        {

            string strSQL = @"INSERT INTO {0} ([EventTime],[VehID],[CurrentDistance],[Distance])
                                        VALUES     (@EventTime,@VehID ,@CurrentDistance ,@Distance)";
            strSQL = string.Format(strSQL, strVehDistance_DBSQLServer_Table);

            CmdDB.CommandText = strSQL;
            CmdDB.Parameters.Clear();
            CmdDB.Parameters.Add("@EventTime", veh.Time);
            CmdDB.Parameters.Add("@VehID", veh.VehID);
            CmdDB.Parameters.Add("@CurrentDistance", veh.CurrDistance);
            CmdDB.Parameters.Add("@Distance", veh.DailyDistance);
            CmdDB.CommandText = strSQL;

            int rs = CmdDB.ExecuteNonQuery();

            return ( rs <= 0 ? true:false);
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
