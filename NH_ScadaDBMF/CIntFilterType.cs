using System;
using System.Data;
using System.Data.SqlClient;

namespace ScadaRXM
{
	/// <summary>
	/// CIntFilterType : ����r�o������ �� �޲z���O
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
			// TODO: �b���[�J�غc�禡���{���X
			//
		}


		// �q��ƮwŪ���L�o����
		public void LoadDBFilterType()
		{
			// �Y�w���������O�\��
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
					// �Y �B�⤸1 ��"OR" �� "AND" �h�ഫ�� "|" �� "&"
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

					// �Y �B�⤸2 ��"OR" �� "AND" �h�ഫ�� "|" �� "&"
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

					// �Y �B�⤸3 ��"OR" �� "AND" �h�ഫ�� "|" �� "&"
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

		// ���氣��ب 1 ~ 10 �������~�� �䥦�ϥΪ̱q���� "����r�w�q" ���L�o����
		public bool fnAddDefineCheckPattern(int nFilterType, string vstrContent, string VA, string VB, string VC, string VD, string Notflag )
		{

            string strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1;

			// �Y�w���������O�\��
			if(m_bEnable == false)
			{
				return false;
			}


			int nTypeIndex = nFilterType - 11;

			// �Y�n�D���o�����A�����T
			if (nTypeIndex < 0 && nTypeIndex >= m_DVFilter.Count)
			{
				return false;
			}

			string strOpteration = "";


			// �̹L�o���� �� �ܼơA�զX���B�⦡
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

		// ���o�s�W���L�o����
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



		// �L�o
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

			// �B�z�p�A�������B��
			while (n >= 0)
			{
				n = strTemp.IndexOf("(", nSearchIndex);

				if (n >= 0)
				{
					// �M�䥪�A��
					n1 = strTemp.IndexOf("(", n + 1);

					// �M��k�A��
					n2 = strTemp.IndexOf(")", n + 1);

					// �Y�����k�A���A��ܦ��B�ⶶ�Ǹ���
					if ((n1 < 0 && n2 >= 0) || (n2 < n1))
					{
						// �^���l�B�⦡(������쪺�����Ǥ��B�⦡)
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
					else // �Y����쥪�A���A���n �� n2 ��쪺���k�A���A���O�@��A�]���~�򩹤U��
					{
						// �q�U�@�Ӧr���A���s�M��
						nSearchIndex = n + 1;
					}
				}

			}

            b = Opteration(vstrContent, strTemp, VA, VB, VC, VD, strNOT_Item1_1, strNOT_Item2_1, strNOT_Item3_1, strNOT_Item4_1);

			return b;
		}


		// �w�� strOpteration �@�B��
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

				// �Y�w�L�B�⦡�A�h�����B��
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


				// ���o�B��l
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
					nOpterationType = -1; // �L

					bResult = false;
				}

				// �ഫ�޿�B�⵲�G�� '1' �� '0'
				if (bResult)
				{
					strSub = " " + strTRUE + " "; ;
				}
				else
				{
					strSub = " " + strFALSE + " ";
				}


				// �N�B�⵲�G�A���N��B�⦡
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

			// �N�̫᪺�B�⵲�G�A�ഫ�� ���L�N��(bool)
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
		// (vstrContent�r�ꤤ�A�O�_�]�t A �� B �G�Ӥl�r��)
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
		// (vstrContent�r�ꤤ�A�O�_�]�t A �� B �G�Ӥl�r��)
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


		//�Q��key ��control�A�̬Y�W�h���ovalue string
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



		// ��X�̤j
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

		// ��X�̤p
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
