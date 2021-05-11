using System;
using System.Data;
using System.Data.SqlClient;

namespace ScadaRXM
{
	/// <summary>
	/// CIntFilterType : 關鍵字濾除類型 之 管理類別
	/// </summary>
	public class CIntFilterType
	{
		string strTRUE = "55Star_TRUE";     // TRUE
		string strFALSE = "55Star_FALSE";   // FALSE

		public string m_strDBIP = "127.0.0.1";
		public string m_strDBID = "scada";
		public string m_strDBPW = "vxlsys$1";
		public bool m_bEnable = true;

		DataView    m_DVFilter = null;

		public CIntFilterType()
		{
			//
			// TODO: 在此加入建構函式的程式碼
			//
		}


		// 從資料庫讀取過濾類型
		public void LoadDBFilterType()
		{
			// 若已關閉此類別功能
			if(m_bEnable == false)
			{
				return;
			}


			DataSet DSet = GetFilterClass();

			if (DSet.Tables.Count > 0)
			{
				m_DVFilter = new DataView(DSet.Tables[0]);
				string      strTemp = "";

				for(int i=0; i < m_DVFilter.Count ; i++)
				{
					// 若 運算元1 為"OR" 或 "AND" 則轉換為 "|" 及 "&"
				{
					strTemp = (string) m_DVFilter[i].Row["Operation1"];
					strTemp = strTemp.TrimStart(' ');
					strTemp = strTemp.TrimEnd(' ');

					if(string.Compare(strTemp, "OR") == 0)
					{
						strTemp = " | ";

						m_DVFilter[i].Row["Operation1"] = strTemp;
					}
					else if(string.Compare(strTemp, "AND") == 0)
					{
						strTemp = " & ";

						m_DVFilter[i].Row["Operation1"] = strTemp;
					}
				}

					// 若 運算元2 為"OR" 或 "AND" 則轉換為 "|" 及 "&"
				{
					strTemp = (string) m_DVFilter[i].Row["Operation2"];
					strTemp = strTemp.TrimStart(' ');
					strTemp = strTemp.TrimEnd(' ');

					if(string.Compare(strTemp, "OR") == 0)
					{
						strTemp = " | ";

						m_DVFilter[i].Row["Operation2"] = strTemp;
					}
					else if(string.Compare(strTemp, "AND") == 0)
					{
						strTemp = " & ";

						m_DVFilter[i].Row["Operation2"] = strTemp;
					}
				}

					// 若 運算元3 為"OR" 或 "AND" 則轉換為 "|" 及 "&"
				{
					strTemp = (string)m_DVFilter[i].Row["Operation3"];
					strTemp = strTemp.TrimStart(' ');
					strTemp = strTemp.TrimEnd(' ');

					if(string.Compare(strTemp, "OR") == 0)
					{
						strTemp = " | ";

						m_DVFilter[i].Row["Operation3"] = strTemp;
					}
					else if(string.Compare(strTemp, "AND") == 0)
					{
						strTemp = " & ";

						m_DVFilter[i].Row["Operation3"] = strTemp;
					}
				}




				}

			}
			else
			{
				m_DVFilter = null;
			}
		}

		// 執行除原建制之 1 ~ 10 的類型外的 其它使用者從網頁 "關鍵字定義" 的過濾類型
		public bool fnAddDefineCheckPattern(int nFilterType, string vstrContent, string VA, string VB, string VC, string VD, string Notflag )
		{

            string strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1;

			// 若已關閉此類別功能
			if(m_bEnable == false)
			{
				return false;
			}


			int nTypeIndex = nFilterType - 11;

			// 若要求的濾除型態不正確
			if (nTypeIndex < 0 && nTypeIndex >= m_DVFilter.Count)
			{
				return false;
			}

			string strOpteration = "";


			// 依過濾類型 及 變數，組合成運算式
		{
			//strOpteration = ;
            // Add by peter  
            strNOT_Item1_1 = m_DVFilter[nTypeIndex].Row["Not_item1_1"].ToString();
            strNOT_Item2_1 = m_DVFilter[nTypeIndex].Row["Not_item2_1"].ToString();
            strNOT_Item3_1 = m_DVFilter[nTypeIndex].Row["Not_item3_1"].ToString();
            strNOT_Item4_1 = m_DVFilter[nTypeIndex].Row["Not_item4_1"].ToString();
            //


			strOpteration = m_DVFilter[nTypeIndex].Row["Item1_1"].ToString();

			strOpteration = strOpteration + " A ";

			strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Item1_2"].ToString();

			if (string.Compare(m_DVFilter[nTypeIndex].Row["Operation1"].ToString(), "") != 0)
			{
				// pts

				strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Operation1"].ToString() + m_DVFilter[nTypeIndex].Row["Item2_1"].ToString();

				strOpteration = strOpteration + " B ";

				strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Item2_2"].ToString();

				if (string.Compare(m_DVFilter[nTypeIndex].Row["Operation2"].ToString(), "") != 0)
				{
					strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Operation2"].ToString() + m_DVFilter[nTypeIndex].Row["Item3_1"].ToString();

					strOpteration = strOpteration + " C ";

					strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Item3_2"].ToString();

					if (string.Compare(m_DVFilter[nTypeIndex].Row["Operation3"].ToString(), "") != 0)
					{
						strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Operation3"].ToString() + m_DVFilter[nTypeIndex].Row["Item4_1"].ToString();

						strOpteration = strOpteration + " D ";

						strOpteration = strOpteration + m_DVFilter[nTypeIndex].Row["Item4_2"].ToString();
					}
				}
			}
		}
            
			//return fnCheckFilterPattern(vstrContent, strOpteration, VA, VB, VC, VD);
            // Add by peter 109/02/16  ***   notflag reverse return condition    ***** 
        if (fnCheckFilterPattern(vstrContent, strOpteration, VA, VB, VC, VD, strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1))
        {
            if (Notflag == "NOT")
                return false;
            else
                return true;
        }
        else
        {
            if (Notflag == "NOT")
                return true;
            else
                return false;
        }

		}

		// 取得新增的過濾類型
		private DataSet GetFilterClass()
		{
			string strConn = "Data Source=" + m_strDBIP + ";uid=" + m_strDBID + ";pwd=" + m_strDBPW + ";database=TRTC_ETL";

			DataSet dsTmp = null;
			SqlDataAdapter da = null;

			string strSql = "";

			strSql = "Select * from KeywordFilterType order by seq";


			try
			{
				SqlConnection SQLConnection = new SqlConnection(strConn);

				dsTmp = new DataSet();

				da = new SqlDataAdapter(strSql, SQLConnection);
				da.SelectCommand = new SqlCommand(strSql, SQLConnection);
				da.Fill(dsTmp);

				SQLConnection.Close();
				SQLConnection.Dispose();
			}
			catch (Exception Ex)
			{
			}


			return dsTmp;
		}



		// 過濾
        private bool fnCheckFilterPattern(string vstrContent, string strOpteration, string VA, string VB, string VC, string VD, string strNOT_Item1_1, string strNOT_Item2_1, string strNOT_Item3_1, string strNOT_Item4_1)
		{
			string strTemp;
			string strSubTemp;
			string strSubTemp1;
			int n = 0;
			int n1;
			int n2;
			int nSearchIndex = 0;

			string strV1;
			string strV2;
			string strOperation;

			bool b = false;

			strTemp = strOpteration;

			// 處理小括號中的運算
			while (n >= 0)
			{
				n = strTemp.IndexOf("(", nSearchIndex);

				if (n >= 0)
				{
					// 尋找左括號
					n1 = strTemp.IndexOf("(", n + 1);

					// 尋找右括號
					n2 = strTemp.IndexOf(")", n + 1);

					// 若先找到右括號，表示此運算順序較高
					if ((n1 < 0 && n2 >= 0) || (n2 < n1))
					{
						// 擷取子運算式(此次找到的高順序之運算式)
						strSubTemp = strTemp.Substring(n, n2 - n + 1);

						strSubTemp = strSubTemp.TrimStart('(');
						strSubTemp = strSubTemp.TrimStart(' ');

						strSubTemp = strSubTemp.TrimEnd(')');
						strSubTemp = strSubTemp.TrimEnd(' ');

						b = Opteration(vstrContent, strSubTemp, VA, VB, VC, VD, strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1);

						if (b)
						{
							strSubTemp = " " + strTRUE + " ";
						}
						else
						{
							strSubTemp = " " + strFALSE + " ";
						}

						strTemp = strTemp.Remove(n, n2 - n + 1);

						strTemp = strTemp.Insert(n, strSubTemp);
					}
					else // 若先找到左括號，表示n 及 n2 找到的左右括號，不是一對，因此繼續往下找
					{
						// 從下一個字元，重新尋找
						nSearchIndex = n + 1;
					}
				}

			}

            b = Opteration(vstrContent, strTemp, VA, VB, VC, VD, strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1);

			return b;
		}


		// 針對 strOpteration 作運算
        private bool Opteration(string vstrContent, string strOpteration, string VA, string VB, string VC, string VD, string strNOT_Item1_1, string strNOT_Item2_1, string strNOT_Item3_1, string strNOT_Item4_1)
		{
			string  strSub = "";
			string  strTemp = "";
			bool bResult = false;

			int nOpteration1 = 0;
			int nOpteration2 = 0;


			string strOpteration_L = "";
			string strOpteration_R = "";

            string strNOTOpteration_L = "";
            string strNOTOpteration_R = "";

			while (nOpteration1 >= 0)
			{
				strTemp = strOpteration;

				strTemp = strTemp.Replace('|', '?');
				strTemp = strTemp.Replace('&', '?');
				strTemp = strTemp.Replace('>', '?');
				strTemp = strTemp.Replace('<', '?');

				nOpteration1 = strTemp.IndexOf("?");

				// 若已無運算式，則結束運算
				if (nOpteration1 < 0)
				{
					break;
				}

				nOpteration2 = -1;

				if (nOpteration1 >= 0)
				{
					nOpteration2 = strTemp.IndexOf("?", nOpteration1 + 1);
				}

				if (nOpteration1 >= 0 && nOpteration2 >= 0)
				{
					strSub = strOpteration.Remove(nOpteration2, strTemp.Length - nOpteration2);
				}
				else
				{
					strSub = strOpteration;
				}

				strSub = strSub.TrimStart(' ');
				strSub = strSub.TrimEnd(' ');


				int n1 = 0;
				int n2 = 0;
				int n3 = 0;
				int n4 = 0;
				int n = 0;
				int nOpterationType = 0;


				n1 = strSub.IndexOf('|');
				n2 = strSub.IndexOf('&');
				n3 = strSub.IndexOf('<');
				n4 = strSub.IndexOf('>');

				if (n1 < 0)
				{
					n1 = 10000;
				}

				if (n2 < 0)
				{
					n2 = 10000;
				}

				if (n3 < 0)
				{
					n3 = 10000;
				}

				if (n4 < 0)
				{
					n4 = 10000;
				}


				n = MyMin(n1, MyMin(n2, MyMin(n3, n4)));


				// 取得運算子
			{
				strOpteration_L = strSub.Substring(0, nOpteration1 - 1);
				strOpteration_R = strSub.Substring(nOpteration1 + 1);

				strOpteration_L = strOpteration_L.TrimStart(' ');
				strOpteration_L = strOpteration_L.TrimEnd(' ');

				strOpteration_R = strOpteration_R.TrimStart(' ');
				strOpteration_R = strOpteration_R.TrimEnd(' ');

				if (strOpteration_L.CompareTo("A") == 0)
				{
					strOpteration_L = VA;
                    strNOTOpteration_L = strNOT_Item1_1;
				}
				else if (strOpteration_L.CompareTo("B") == 0)
				{
					strOpteration_L = VB;
                    strNOTOpteration_L = strNOT_Item2_1;
				}
				else if (strOpteration_L.CompareTo("C") == 0)
				{
					strOpteration_L = VC;
                    strNOTOpteration_L = strNOT_Item3_1;
				}
				else if (strOpteration_L.CompareTo("D") == 0)
				{
					strOpteration_L = VD;
                    strNOTOpteration_L = strNOT_Item4_1;
				}

				if (strOpteration_R.CompareTo("A") == 0)
				{
					strOpteration_R = VA;
                    strNOTOpteration_R = strNOT_Item1_1;
				}
				else if (strOpteration_R.CompareTo("B") == 0)
				{
					strOpteration_R = VB;
                    strNOTOpteration_R = strNOT_Item2_1;
				}
				else if (strOpteration_R.CompareTo("C") == 0)
				{
					strOpteration_R = VC;
                    strNOTOpteration_R = strNOT_Item3_1;
				}
				else if (strOpteration_R.CompareTo("D") == 0)
				{
					strOpteration_R = VD;
                    strNOTOpteration_R = strNOT_Item4_1;
				}
			}

				if (n == n1)
				{
					nOpterationType = 0; // OR

                    bResult = Opteration_A_OR_B(vstrContent, strOpteration_L, strOpteration_R, strNOTOpteration_L, strNOTOpteration_R);
				}
				else if (n == n2)
				{
					nOpterationType = 1; // AND

                    bResult = Opteration_A_AND_B(vstrContent, strOpteration_L, strOpteration_R, strNOTOpteration_L, strNOTOpteration_R);
				}
				else if (n == n3)
				{
					nOpterationType = 2; // <

                    bResult = Opteration_A_Small_B(vstrContent, strOpteration_L, strOpteration_R, strNOTOpteration_L, strNOTOpteration_R);
				}
				else if (n == n4)
				{
					nOpterationType = 3; // >

                    bResult = Opteration_A_Large_B(vstrContent, strOpteration_L, strOpteration_R, strNOTOpteration_L, strNOTOpteration_R);
				}
				else
				{
					nOpterationType = -1; // 無

					bResult = false;
				}

				// 轉換邏輯運算結果為 '1' 或 '0'
				if (bResult)
				{
					strSub = " " + strTRUE + " "; ;
				}
				else
				{
					strSub = " " + strFALSE + " ";
				}


				// 將運算結果，取代原運算式
				if (nOpteration1 >= 0 && nOpteration2 >= 0)
				{
					strOpteration = strOpteration.Remove(0, nOpteration2);

					strOpteration = strOpteration.Insert(0, strSub);
				}
				else
				{
					strOpteration = strSub;
				}
			}

			strOpteration = strOpteration.TrimEnd(' ');
			strOpteration = strOpteration.TrimStart(' ');

			// 將最後的運算結果，轉換為 布林代數(bool)
			if (string.Compare(strOpteration, strTRUE) == 0)
			{
				bResult = true;
			}
			else
			{
				bResult = false;
			}

			return bResult;
		}


		// A OR B
		// (vstrContent字串中，是否包含 A 或 B 二個子字串)
        private bool Opteration_A_OR_B(string vstrContent, string strOpteration_L, string strOpteration_R, string strNOTOpteration_L, string strNOTOpteration_R)
		{
			bool b1 = false;
			bool b2 = false;
			bool b = false;

			if (string.Compare(strOpteration_L, strTRUE) == 0)
			{
				b1 = true;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = false;

			}
			else if (string.Compare(strOpteration_L, strFALSE) == 0)
			{
				b1 = false;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = true;
			}
			else
			{
				b1 = vstrContent.IndexOf(strOpteration_L) > 0;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = !b1;
			}



            if (string.Compare(strOpteration_R, strTRUE) == 0)
			{
				b2 = true;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = false;
			}
            else if (string.Compare(strOpteration_R, strFALSE) == 0)
			{
				b2 = false;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = true;
			}
			else
			{
                b2 = vstrContent.IndexOf(strOpteration_R) > 0;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = !b1;
			}

			if (b1 || b2)
			{
				b = true;
			}

			return b;
		}

		// A AND B
		// (vstrContent字串中，是否包含 A 及 B 二個子字串)
        private bool Opteration_A_AND_B(string vstrContent, string strOpteration_L, string strOpteration_R, string strNOTOpteration_L, string strNOTOpteration_R)
		{
			bool b1 = false;
			bool b2 = false;
			bool b = false;

            if (string.Compare(strOpteration_L, strTRUE) == 0)
			{
				b1 = true;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = false;
			}
            else if (string.Compare(strOpteration_L, strFALSE) == 0)
			{
				b1 = false;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = true;
			}
			else
			{
                b1 = vstrContent.IndexOf(strOpteration_L) > 0;
                if (string.Compare(strNOTOpteration_L.ToUpper(), "NOT") == 0)
                    b1 = !b1;
			}



            if (string.Compare(strOpteration_R, strTRUE) == 0)
			{
				b2 = true;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = false;
			}
            else if (string.Compare(strOpteration_R, strFALSE) == 0)
			{
				b2 = false;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = true;
			}
			else
			{
                b2 = vstrContent.IndexOf(strOpteration_R) > 0;
                if (string.Compare(strNOTOpteration_R.ToUpper(), "NOT") == 0)
                    b2 = !b2;
			}

			if (b1 && b2)
			{
				b = true;
			}

			return b;
		}

		// A < B
        private bool Opteration_A_Small_B(string vstrContent, string strOpteration_L, string strOpteration_R, string strNOTOpteration_L, string strNOTOpteration_R)
		{
			bool b = false;

			string strTmp = fnGetKeyValue(vstrContent, strOpteration_L);

			if (int.Parse(strTmp) < int.Parse(strOpteration_R))
			{
				b = true;
			}

			return b;
		}

		// A > B
        private bool Opteration_A_Large_B(string vstrContent, string strOpteration_L, string strOpteration_R, string strNOTOpteration_L, string strNOTOpteration_R)
		{
			bool b = false;

            try
            {
                string strTmp = fnGetKeyValue(vstrContent, strOpteration_L);

                if (int.Parse(strTmp) > int.Parse(strOpteration_R))
                {
                    b = true;
                }

            }
            catch
            {
                b = false;
            }
			return b;
		}


		//利用key 跟control，依某規則取得value string
		private string fnGetKeyValue(string vstrContent, string vstrKey)
		{
			string strKey = vstrKey;
			string strContent = vstrContent;
			string strValue = "";
			//int intReturn=-1;
			bool blnEndChar = false;
			bool blnStartChar = false;

			int intKeyPos = strContent.IndexOf(strKey);
			int intTmp;

			//確認該字串中是否有key
			if (intKeyPos >= 0)
			{

				//char[] chrAryTmp=strContent.ToCharArray()
				intTmp = strContent.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }, intKeyPos + strKey.Length);

				//確認該字串中key之後是否有數字
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



		// 輸出最大
		private int MyMax(int a, int b)
		{
			if (a > b)
			{
				return a;
			}
			else
			{
				return b;
			}
		}

		// 輸出最小
		private int MyMin(int a, int b)
		{
			if (a > b)
			{
				return b;
			}
			else
			{
				return a;
			}
		}

	}
}
