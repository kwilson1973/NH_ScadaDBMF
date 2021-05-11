using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace SCADA_RX
{
	class Ovline_sync
	{
		// 連接資料庫
		public bool ConnectSqlCeDB(ref SqlConnection SqlConn, string strIP, string strDBName)
		{
			bool bConnect = false;

			string strConnect = "data source=" + strIP + ";uid=scada;pwd=vxlsys$1;database=" + strDBName;

			try
			{
				SqlConn = new SqlConnection(strConnect);

				SqlConn.Open();

				bConnect = true;
			}
			catch (Exception ex)
			{
				bConnect = false;
			}

			return bConnect;
		}

		// 下載SQL指令
		public DataSet SqlCommand(ref SqlConnection DBSqlConnect, string strSQL)
		{
			if (DBSqlConnect == null)
			{
				return null;
			}

			DataSet DSet = null;

			try
			{
				SqlCommand selectCmd = DBSqlConnect.CreateCommand();

				selectCmd.CommandText = strSQL;

				SqlDataAdapter adp = new SqlDataAdapter(selectCmd);

				DSet = new DataSet();

				adp.Fill(DSet);
			}
			catch (Exception ex)
			{
				return null;
			}


			return DSet;
		}

		// 下載SQL指令
		public int ExecuteSqlCommand(ref SqlConnection DBSqlConnect, string strSQL)
		{
			int n = 0;

			try
			{
				SqlCommand MSqlCommand = new SqlCommand(strSQL, DBSqlConnect);

				MSqlCommand.CommandText = strSQL;

				n = MSqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				return 0;
			}

			return n;
		}


		// 將指定的前端電腦的 OvLine 資料表，和Main DB的 OvLine 資料表作同步
		public void FrontOvline_Sync(string strDesDBName, string strDesTableName, string strDesDBIP, string strSrcDBName, string strSrcTableName, string strSrcDBIP)
		{
			string strDesConnect = "data source=" + strDesDBName + ";uid=scada;pwd=vxlsys$1;database=" + strDesTableName;
			string strSrcConnect = "data source=" + strSrcDBName + ";uid=scada;pwd=vxlsys$1;database=" + strSrcTableName;


			SqlConnection DesConnection = null;
			SqlConnection SrcConnection = null;


			try
			{
				// 開啟資料庫
				bool bDesConn = ConnectSqlCeDB(ref DesConnection, strDesDBIP, strDesDBName);
				bool bSrcConn = ConnectSqlCeDB(ref SrcConnection, strSrcDBIP, strSrcDBName);

				// 若資料庫開啟失敗
				if (bDesConn == false || bSrcConn == false)
				{
					if (DesConnection != null)
					{
						DesConnection.Close();
						DesConnection.Dispose();
					}

					if (SrcConnection != null)
					{
						SrcConnection.Close();
						SrcConnection.Dispose();
					}

					return;
				}

				string strDesSQL = "";
				string strSrcSQL = "";


				strDesSQL = "Select * from OvLine";
				strSrcSQL = "Select * from OvLine";

				// Get OvLine data
				DataSet DesDSet = SqlCommand(ref DesConnection, strDesSQL);
				DataSet SrcDSet = SqlCommand(ref SrcConnection, strSrcSQL);

				if (DesDSet == null || SrcDSet == null || DesDSet.Tables.Count <= 0 || SrcDSet.Tables.Count <= 0)
				{
					if (DesDSet != null)
					{
						DesDSet.Dispose();
					}

					if (SrcDSet != null)
					{
						SrcDSet.Dispose();
					}

					if (DesConnection != null)
					{
						DesConnection.Close();
						DesConnection.Dispose();
					}

					if (SrcConnection != null)
					{
						SrcConnection.Close();
						SrcConnection.Dispose();
					}

					return;
				}



				DataView DVDes = new DataView(DesDSet.Tables[0]);
				DataView DVSrc = new DataView(SrcDSet.Tables[0]);

				if (DVDes == null || DVSrc == null)
				{

					if (DVDes != null)
					{
						DVDes.Dispose();
					}


					if (DVSrc != null)
					{
						DVSrc.Dispose();
					}


					if (DesDSet != null)
					{
						DesDSet.Dispose();
					}

					if (SrcDSet != null)
					{
						SrcDSet.Dispose();
					}

					if (DesConnection != null)
					{
						DesConnection.Close();
						DesConnection.Dispose();
					}

					if (SrcConnection != null)
					{
						SrcConnection.Close();
						SrcConnection.Dispose();
					}
				}

				bool[] bDesCheckList = new bool[DVDes.Count];
				bool[] bSrcCheckList = new bool[DVSrc.Count];
				string[] strDesDataList = new string[DVDes.Count];
				string[] strSrcDataList = new string[DVSrc.Count];

				int i = 0;
				int j = 0;

				// Load all data to memory from Destination Table and Source Table
				/* FIXME:
				 *      1) strDesDataList: It's really SOURCE db.
				 *      2) strSrcDataList: It's really DESTINATION db.
				 *      
				 *      Need to fix the typo.
				 */

				for (i = 0; i < DVDes.Count; i++)
				{
					bDesCheckList[i] = false;

					strDesDataList[i] = DataToString(ref DVDes, i);
				}

				// FIXME: 
				for (i = 0; i < DVSrc.Count; i++)
				{
					bSrcCheckList[i] = false;

					strSrcDataList[i] = DataToString(ref DVSrc, i);
				}


				// 檢查新增資料
			{
				int nIndex = 0;

				for (i = 0; i < DVDes.Count; i++)
				{
					nIndex = -1;

					for (j = 0; j < DVSrc.Count; j++)
					{
						if (string.Compare(strDesDataList[i], strSrcDataList[j]) == 0)
						{
							nIndex = 0;
							break;
						}
					}

					if (nIndex >= 0)
					{
						// 記錄DVDes所在資料有被尋到
						bDesCheckList[i] = true;
					}
				}
			}

				// 檢查刪除資料
			{
				int nIndex = 0;

				for (i = 0; i < DVSrc.Count; i++)
				{
					nIndex = -1;

					for (j = 0; j < DVDes.Count; j++)
					{
						if (string.Compare(strDesDataList[j], strSrcDataList[i]) == 0)
						{
							nIndex = 0;
							break;
						}
					}

					if (nIndex >= 0)
					{
						// 記錄DVDes所在資料有被尋到
						bSrcCheckList[i] = true;
					}
				}
			}

				// 處理資料刪除
			{
				string strSQL = "";

				for (i = 0; i < bSrcCheckList.Length; i++)
				{
					// 資料新增
					if (bSrcCheckList[i] == false)
					{
						strSQL = "DELETE FROM OvLine ";
						strSQL = strSQL + " Where Line_Code = '" + DVSrc[i].Row["Line_Code"].ToString() + "'";

						ExecuteSqlCommand(ref SrcConnection, strSQL);
					}

				}
			}


				// 處理資料新增
			{
				string strSQL = "";
				string strTime = "";
				DateTime DTime = new DateTime();

				for (i = 0; i < bDesCheckList.Length; i++)
				{
					// 資料新增
					if (bDesCheckList[i] == false)
					{
						DTime = System.DateTime.Parse(DVDes[i].Row["UpdatedTime"].ToString());

						strSQL = "INSERT INTO OvLine ";
						strSQL = strSQL + "VALUES( ";

						strSQL = strSQL + " '" + DVDes[i].Row["Line_Name"].ToString() + "',";
						strSQL = strSQL + " '" + DVDes[i].Row["Line_Code"].ToString() + "',";
						strSQL = strSQL + " " + DVDes[i].Row["Location"].ToString() + ",";
						strSQL = strSQL + " '" + DVDes[i].Row["LineN"].ToString() + "',";
						strSQL = strSQL + " " + DVDes[i].Row["IniCnt"].ToString() + ",";
						strSQL = strSQL + " " + DVDes[i].Row["IniCnt2"].ToString() + ",";
						strSQL = strSQL + " " + DVDes[i].Row["CurCnt"].ToString() + ",";
						strSQL = strSQL + " " + DVDes[i].Row["Tolerance"].ToString() + ",";
						strSQL = strSQL + " " + DVDes[i].Row["Status"].ToString() + ",";
						strSQL = strSQL + " '" + DTime.ToString("yyyy/MM/dd HH:mm:ss") + "',";
						strSQL = strSQL + " '" + DVDes[i].Row["OTHER"].ToString() + "',";
						strSQL = strSQL + " '" + DVDes[i].Row["LineLoss"].ToString() + "',";

						if (string.Compare(DVDes[i].Row["IsLFAlarm"].ToString().ToLower(), "false") == 0)
						{
							strSQL = strSQL + "0";
						}
						else
						{
							strSQL = strSQL + "1";
						}

						strSQL = strSQL + ")";


						ExecuteSqlCommand(ref SrcConnection, strSQL);
					}

				}
			}





				// Release memory
			{
				if (DVDes != null)
				{
					DVDes.Dispose();
				}


				if (DVSrc != null)
				{
					DVSrc.Dispose();
				}

				if (DesDSet != null)
				{
					DesDSet.Dispose();
				}

				if (SrcDSet != null)
				{
					SrcDSet.Dispose();
				}

				if (DesConnection != null)
				{
					DesConnection.Close();
					DesConnection.Dispose();
				}

				if (SrcConnection != null)
				{
					SrcConnection.Close();
					SrcConnection.Dispose();
				}

			}
			}
			catch (Exception ex)
			{
				return;
			}

		}


		// 將DataView資料轉換為字串
		public string DataToString(ref DataView DV, int nIndex)
		{
			string strData = "";

			try
			{
				// DateTime DTime = System.DateTime.Parse(DV[nIndex].Row["UpdatedTime"].ToString());


				strData = DV[nIndex].Row["Line_Name"].ToString() + ",";
				strData = strData + DV[nIndex].Row["Line_Code"].ToString() + ",";
				strData = strData + DV[nIndex].Row["Location"].ToString() + ",";
				strData = strData + DV[nIndex].Row["LineN"].ToString() + ",";
				strData = strData + DV[nIndex].Row["IniCnt"].ToString() + ",";
				strData = strData + DV[nIndex].Row["IniCnt2"].ToString() + ",";
				// strData = strData + DV[nIndex].Row["CurCnt"].ToString() + ",";
				strData = strData + DV[nIndex].Row["Tolerance"].ToString() + ",";
				strData = strData + DV[nIndex].Row["Status"].ToString() + ",";
				// strData = strData + DTime.ToString("yyyy/MM/dd HH:mm:ss") + ",";
				strData = strData + DV[nIndex].Row["OTHER"].ToString() + ",";
				strData = strData + DV[nIndex].Row["LineLoss"].ToString() + ",";

				if (string.Compare(DV[nIndex].Row["IsLFAlarm"].ToString().ToLower(), "false") == 0)
				{
					strData = strData + "0";
				}
				else
				{
					strData = strData + "1";
				}
			}
			catch
			{
				return "";
			}

			return strData;
		}
	}
}
