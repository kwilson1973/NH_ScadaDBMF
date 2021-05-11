using System;
using System.Data;

namespace IntelligentMethod
{
	/// <summary>
	/// Class1 的摘要描述。
	/// </summary>
	public class Method
	{
//		public Class1()
//		{
//			//
//			// TODO: 在此加入建構函式的程式碼
//			//
//		}

		#region 判斷是否要進入智慧DB


//		//判斷小集合有沒有要的
//		int fnCheckContent(string vstrSource, string vstrContent)
//		{
//			//			bool btnSaveData;
//			//			btnSaveData=false;
//			int intSaveDataKey=-1;
//
//
//			int intKwType;
//			string strA;
//			string strB;
//			string strC;
//			string strD;
//
//			DataView myDataView=new DataView(this.dtIntBase );
//			//myDataView.RowFilter="Equ_Code='"+ vstrSource.Trim() +"'";		//判斷是那個line
//			myDataView.RowFilter="sys_no='"+ vstrSource.Trim() +"'";		//判斷是那個line
//			myDataView.Sort="ITEM_LEVEL";									//Pattern選用的順序
//
//			for(int i=0 ; i< myDataView.Count ;i++)
//			{
//
//				intKwType= Convert.ToInt32 (myDataView[i].Row["KEYWORD_TYPE"]);
//
//				//if (intKwType<=0) continue;
//
//				strA=myDataView[i].Row["ITEM_NO01"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO01"];
//				strB=myDataView[i].Row["ITEM_NO02"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO02"];
//				strC=myDataView[i].Row["ITEM_NO03"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO03"];
//				strD=myDataView[i].Row["ITEM_NO04"] == System.DBNull.Value ? string.Empty : (string)myDataView[i].Row["ITEM_NO04"];
//
//				//btnSaveData=fnCheckPattern(intKwType,vstrContent, strA,strB,strC,strD);
//				//fnCheckPattern(9,"dhflshfhsflsahlfhsl ESP,== 100sdfssd", "ESP","99","888","");
//				if (fnCheckPattern(intKwType,vstrContent, strA,strB,strC,strD))
//				{return Convert.ToInt32(myDataView[i].Row["item_id"]);}
//			}
//
//			//myDataView[0].Row["seq_id"]
//
//
//			return intSaveDataKey;
//		}

		//判斷小集合的每一筆資料有沒有要的
		public static bool fnCheckPattern(int vintKwType ,string vstrContent, string vstrA, string vstrB, string vstrC, string vstrD)
		{
			bool btnReturn;
			btnReturn=false;
            int hit=0 ;
			switch( vintKwType)
			{
				case 1:				//1:A
					btnReturn=fnPattern1(vstrContent,vstrA);
                    if (btnReturn)
                        hit = 1;
					break;

				case 2:				//2:A && B
					btnReturn=fnPattern2(vstrContent, vstrA,vstrB);
                    if (btnReturn)
                        hit = 1;
					break;

				case 3:				//3:A || B
					btnReturn=fnPattern3(vstrContent, vstrA,vstrB);
					break;

				case 4:				//4:(A&&B)||C
					btnReturn=fnPattern4(vstrContent, vstrA,vstrB,vstrC);
					break;

				case 5:				//5:A && B &&C
					btnReturn=fnPattern5(vstrContent, vstrA,vstrB,vstrC);
					break;

				case 6:				//6:A || B || C
					btnReturn=fnPattern6(vstrContent, vstrA,vstrB,vstrC);
					break;

				case 7:				//7:(A&&B)||(C&&D)
					btnReturn=fnPattern7(vstrContent, vstrA,vstrB,vstrC,vstrD);
					break;

				case 8:				//8: X > Value  A=key, B=Value
					if(vstrA.Length!=0 && vstrB.Length!=0)
					{btnReturn=fnPattern8(vstrContent, vstrA,vstrB);}
					break;
			
				case 9:				//9: X < Value  A=key, B=Value
					if(vstrA.Length!=0 && vstrB.Length!=0)
					{btnReturn=fnPattern9(vstrContent, vstrA,vstrB);}
					break;

				case 10:			//10: Value1 < X < Value2  A=key, B=Value1 , C=Value2
					if(vstrA.Length!=0 && vstrB.Length!=0 && vstrC.Length!=0)
					{btnReturn=fnPattern10(vstrContent, vstrA,vstrB,vstrC);}
					break;
			}

			return btnReturn;
		
		}


		#region Patterns

		//利用key 跟control，依某規則取得value string
		static string fnGetKeyValue(string vstrContent, string vstrKey)
		{
			string strKey=vstrKey;
			string strContent=vstrContent;
			string strValue="";
			//int intReturn=-1;
			bool blnEndChar=false;
			bool blnStartChar=false;

			int intKeyPos=strContent.IndexOf(strKey);
			int intTmp;

			//確認該字串中是否有key
			if (intKeyPos>=0)
			{
				
				//char[] chrAryTmp=strContent.ToCharArray()
				intTmp=strContent.IndexOfAny(new char[]{'0','1','2','3','4','5','6','7','8','9'},intKeyPos+strKey.Length);

				//確認該字串中key之後是否有數字
				if (intTmp>=0 || intTmp>intKeyPos)
				{

					char[] chrAryTmp=strContent.ToCharArray(intKeyPos+strKey.Length,strContent.Length-strKey.Length-intKeyPos );
					for(int i=0; i<chrAryTmp.Length; i++)
					{
						if(blnEndChar && blnStartChar){break;}
						if (chrAryTmp[i]=='0' || chrAryTmp[i]=='1' || chrAryTmp[i]=='2' || chrAryTmp[i]=='3' || chrAryTmp[i]=='4' || chrAryTmp[i]=='5' || chrAryTmp[i]=='6' || chrAryTmp[i]=='7' || chrAryTmp[i]=='8' || chrAryTmp[i]=='9')
						{
							strValue=strValue + chrAryTmp[i].ToString();
							blnStartChar=true;
						}
						else
						{
							if (chrAryTmp[i]==' ' || chrAryTmp[i]==',' || chrAryTmp[i]==';' || chrAryTmp[i]=='=') 
							{if(blnStartChar){blnEndChar=true;}}
							else
							{break;}
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
		static bool fnPattern10(string vstrContent, string vstrA, string vstrB, string vstrC)
		{
			bool btnReturn;
			btnReturn=false;
			string strTmp;

			strTmp=fnGetKeyValue(vstrContent,vstrA);

			if (strTmp.Length!=0)
			{
				if (int.Parse (strTmp) > int.Parse(vstrB)  &&  int.Parse (strTmp) < int.Parse(vstrC) )
					btnReturn=true;
			}	

			return btnReturn;
		}

		//9: X < Value  A=key, B=Value
		static bool fnPattern9(string vstrContent, string vstrA, string vstrB)
		{
			bool btnReturn;
			btnReturn=false;
			string strTmp;

			strTmp=fnGetKeyValue(vstrContent,vstrA);

			if (strTmp.Length!=0)
			{
				if (int.Parse (strTmp) < int.Parse(vstrB))
					btnReturn=true;
			}
	
			return btnReturn;
		}

		//8: X >= Value  A=key, B=Value
		static bool fnPattern8(string vstrContent, string vstrA, string vstrB)
		{
			bool btnReturn;
			btnReturn=false;
			string strTmp;

			strTmp=fnGetKeyValue(vstrContent,vstrA);

			if (strTmp.Length!=0)
			{
				if (int.Parse (strTmp)>int.Parse(vstrB))
					btnReturn=true;
			}

			return btnReturn;
		}


		static bool fnPattern7(string vstrContent, string vstrA, string vstrB, string vstrC, string vstrD)
		{
			bool btnReturn;
			btnReturn=false;
		
			if((fnPatterns(vstrContent, vstrA) && fnPatterns(vstrContent,vstrB)) || (fnPatterns(vstrContent,vstrC)&&fnPatterns(vstrContent,vstrD)))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}


		static bool fnPattern6(string vstrContent, string vstrA, string vstrB, string vstrC)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA) || fnPatterns(vstrContent,vstrB) || fnPatterns(vstrContent,vstrC))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}

		static bool fnPattern5(string vstrContent, string vstrA, string vstrB, string vstrC)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA) && fnPatterns(vstrContent,vstrB) && fnPatterns(vstrContent,vstrC))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}


		static bool fnPattern4(string vstrContent, string vstrA, string vstrB, string vstrC)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA) && fnPatterns(vstrContent,vstrB) || fnPatterns(vstrContent,vstrC))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}


		static bool fnPattern3(string vstrContent, string vstrA, string vstrB)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA) || fnPatterns(vstrContent,vstrB))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}


		static bool fnPattern2(string vstrContent, string vstrA, string vstrB)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA) && fnPatterns(vstrContent,vstrB))
			{
				btnReturn=true;
			}

			return btnReturn;
		
		}



		static bool fnPattern1(string vstrContent, string vstrA)
		{
			bool btnReturn;
			btnReturn=false;
		
			if(fnPatterns(vstrContent,vstrA))
			{
				btnReturn=true;
			}


			return btnReturn;
		}


		static bool fnPatterns(string vstrContent, string vstr)
		{
			bool btnReturn;

			if(vstrContent.IndexOf(vstr) >= 0)
			{btnReturn=true;}
			else
			{btnReturn=false;}

			return btnReturn;
		}


		#endregion

		#endregion


        #region 多重警訊判斷

        public static bool fnCheckMultiple(string vstrContent, string vstr)
        {
            bool btnReturn = false;
            if (vstr != "") btnReturn = fnMultiplePatterns(vstrContent, vstr);
            return btnReturn;
        }

        static bool fnMultiplePatterns(string vstrContent, string vstr)
        {
            bool btnReturn;

            if (vstrContent.IndexOf(vstr) >= 0)
            { btnReturn = true; }
            else
            { btnReturn = false; }

            return btnReturn;
        }


        #endregion
    }
}
