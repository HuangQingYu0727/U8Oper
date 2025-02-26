using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;


/// <summary>
/// U8SFSOperation 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class U8SFSOperation : System.Web.Services.WebService
{

    public U8SFSOperation()  //四方所
    {
        
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }


    #region  //公共方法
    public System.Data.SqlClient.SqlConnection OpenDataConnection()
    {
        System.Data.SqlClient.SqlConnection SqlConn = new System.Data.SqlClient.SqlConnection();
        SqlConn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DB_UFSYSTEM"].ConnectionString;
        try
        {
            SqlConn.Open();
        }
        catch (Exception ex)
        {
            return null;
        }
        return SqlConn;
    }

    public string CloseDataConnection(System.Data.SqlClient.SqlConnection SqlConn)
    {
        try
        {
            if (SqlConn != null && SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }

    private string GetDataString(string sql, System.Data.SqlClient.SqlCommand cmmd)
    {
        cmmd.CommandText = sql;
        string cRdStr = "";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter();
        dpt.SelectCommand = cmmd;
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            cRdStr = dt.Rows[0][0] + "";
        }
        return cRdStr;
    }
    private string GetDataString(string sql, System.Data.SqlClient.SqlConnection Conn)
    {
        string cRet = "";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            cRet = dt.Rows[0][0] + "";
        }
        return cRet;
    }
    private int GetDataInt(string sql, System.Data.SqlClient.SqlCommand cmmd)
    {
        cmmd.CommandText = sql;
        int iRdStr = 0;
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter();
        dpt.SelectCommand = cmmd;
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            iRdStr = int.Parse(dt.Rows[0][0] + "");
        }
        return iRdStr;
    }
    private int GetDataInt(string sql, System.Data.SqlClient.SqlConnection Conn)
    {
        int iRdStr = 0;
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            iRdStr = int.Parse(dt.Rows[0][0] + "");
        }
        return iRdStr;
    }
    public System.Data.DataTable GetSqlDataTable(string sql, string tblName, System.Data.SqlClient.SqlCommand cmmd)
    {
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter();
        cmmd.CommandText = sql;
        dpt.SelectCommand = cmmd;
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds, tblName);
        return ds.Tables[tblName];
    }

    private string GetEnStr()//获得序列号
    {
        string ccomputer = GetComputerInfo();
        if (ccomputer == "") return "";

        string cRetStr = "";
        USAppReg.ClsGetComputerInfo tmp = new USAppReg.ClsGetComputerInfo();
        string tmpSn = ccomputer;
        string cClearText = "COAZHHXCOANZHANG9625GY";
        tmpSn = USAppReg.EncDec.Encrypt(cClearText, tmpSn);
        string tOldStr = tmpSn.Replace("=", "");
        int i = 0;
        tmpSn = "";
        while (i < tOldStr.Length)
        {
            i++;
            if (i > 0 && (i % 4 == 0))
            {
                tmpSn += tOldStr.Substring(i - 4, 4) + "-";
            }
        }
        if (tmpSn.Substring(tmpSn.Length - 1, 1) == "-")
        {
            tmpSn = tmpSn.Substring(0, tmpSn.Length - 1);
        }
        cRetStr = tmpSn.ToUpper();
        return cRetStr;
    }
    private string GetComputerInfo()
    {
        string ComputerMsg = "";

        try
        {
            USAppReg.ClsGetComputerInfo tmp = new USAppReg.ClsGetComputerInfo();
            string tmpSn = "" + tmp.GetCpuID();

            if (tmpSn.Trim().Length <= 3)
            {
                ComputerMsg = tmpSn;
            }
            else
            {
                ComputerMsg = tmpSn.Substring(0, 3) + tmpSn.Substring(tmpSn.Length - 3, 3) + "-";
            }


            tmpSn = tmp.GetHardDiskID();
            if (tmpSn == "")
            {
                ComputerMsg += "";
            }
            else
            {
                if (tmpSn.Trim().Length <= 3)
                {
                    ComputerMsg += tmpSn;
                }
                else
                {
                    ComputerMsg += tmpSn.Substring(0, 3) + tmpSn.Substring(tmpSn.Length - 3, 3);
                }
            }
        }
        catch
        {
        }
        return ComputerMsg;
    }
    #endregion

    #region  //U8通用功能（包括用户验证）
    [WebMethod]  //U8用户验证   返回用户名称
    public string U8UserLogin(string usercode, string pwd, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        UFSoft.U8.Framework.Login.UI.clsLogin uiLogin = new UFSoft.U8.Framework.Login.UI.clsLogin();
        if (GetDataInt("select count(*) from ufsystem..ua_user where cuser_id='" + usercode + "' and isnull(nstate,0)=0", Conn) <= 0)
        {
            ErrMsg = "没有找到对用的用户或者用户已经注销";
            CloseDataConnection(Conn);
            return "";
        }
        string cUserName = "" + GetDataString("select isnull(max(cuser_name),'') from ufsystem..ua_user where cuser_id='" + usercode +
            "' and (cPassWord='" + uiLogin.EnPassWord(pwd) + "' or isnull(cPassWord,'')='') ", Conn);
        if (cUserName == "")
        {
            ErrMsg = "用户密码错误";
            CloseDataConnection(Conn);
            return "";
        }
        ErrMsg = CloseDataConnection(Conn);
        if (ErrMsg + "" != "")
        {
            return "";
        }
        else
        {
            return cUserName;
        }

    }
    [WebMethod]  //U8用户验证,根据日期获得正确数据库连接 返回数据名称
    public string U8UserLoginDate(string usercode, string pwd, string cacc_id, string clogdate, ref string ErrMsg)
    {
        ErrMsg = "";
        string cSn = "";//加密秘钥

        //校验程序合法性


        cSn = cSn.Replace(",", "");//替换掉 英文逗号
        Application.Add("cSn", "" + cSn);

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        string CurConStr = Conn.ConnectionString;
        UFSoft.U8.Framework.Login.UI.clsLogin uiLogin = new UFSoft.U8.Framework.Login.UI.clsLogin();
        if (GetDataInt("select count(*) from ufsystem..ua_user where cuser_id='" + usercode + "' and isnull(nstate,0)=0", Conn) <= 0)
        {
            ErrMsg = "没有找到对用的用户或者用户已经注销";
            CloseDataConnection(Conn);
            return "";
        }
        string cUserName = "" + GetDataString("select isnull(max(cuser_name),'') from ufsystem..ua_user where cuser_id='" + usercode +
            "' and (cPassWord='" + uiLogin.EnPassWord(pwd) + "' or (isnull(cPassWord,'')='' and '" + pwd + "'='')) ", Conn);
        if (cUserName == "")
        {
            ErrMsg = "用户密码错误";
            CloseDataConnection(Conn);
            return "";
        }
        //获得需要连接的数据库 
        string cDataName = "";
        string cYear = "" + GetDataString("select iYear from ufsystem..UA_Period where cAcc_id='" + cacc_id + "' and dBegin<='" + clogdate + "' and dEnd>='" + clogdate + "'", Conn);
        if (cYear == "")
        {
            ErrMsg = "无法找到对应的会计期间";
            CloseDataConnection(Conn);
            return "";
        }
        if (GetDataInt("select count(*) cn from sysobjects where name='UA_AccountDatabase' and type='U'", Conn) > 0)
        {
            cDataName = "" + GetDataString("select cDatabase from UA_AccountDatabase where cacc_id='" + cacc_id + "' and iBeginYear<=" + cYear + " and isnull(iEndYear,9999)>=" + cYear, Conn);
        }
        else
        {
            cDataName = "UFData_" + cacc_id + "_" + cYear;
        }
        if (cDataName == "")
        {
            ErrMsg = "无法找到对应的数据库";
            CloseDataConnection(Conn);
            return "";
        }
        CloseDataConnection(Conn);
        return cUserName + "," + cDataName + "," + cSn;

    }
    [WebMethod]  //U8 授权帐套清单
    public System.Data.DataTable U8LoginAccount(string usercode, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtAcc;
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return null;
        }
        string cSql = @"select cAcc_id,cAcc_Name from ufsystem..UA_Account where cAcc_id in
            (SELECT distinct cAcc_id FROM ufsystem..UA_HoldAuth where cUser_ID='" + usercode + @"' and iIsUser=1
            union
            SELECT distinct cAcc_id FROM ufsystem..UA_HoldAuth where cUser_ID in(select distinct cgroup_id from ufsystem..UA_Role where cUser_ID='" + usercode + @"') and iIsUser=0)";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(cSql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtAcc = ds.Tables[0];

        ErrMsg = CloseDataConnection(Conn);
        if (ErrMsg + "" != "")
        {
            return null;
        }
        else
        {
            return dtAcc;
        }

    }

    [WebMethod]  //U8 获得员工姓名（业务员姓名）
    public string U8PersonName(string cPsnCode, string dbname, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        //select * from sfc_workshift
        string cName = "";
        if (cPsnCode.Length == 3)
        {
            cName = GetDataString("select Description from " + dbname + "..sfc_workshift where Code='" + cPsnCode + "'", Conn);
            if (cName + "" == "")
            {
                throw new Exception("班组不存在");
            }
        }
        else
        {
            cName = GetDataString("select cPersonName from " + dbname + "..Person where cPersonCode='" + cPsnCode + "'", Conn);
        }

        CloseDataConnection(Conn);
        if (cName + "" == "")
        {
            cName = cPsnCode;
            //ErrMsg = "无此人，请在U8中查看业务员档案";
            //return "";
        }
        //else
        //{
        //    return cName;
        //}
        return cName;
    }

    [WebMethod]  //U8 获得员工姓名（业务员姓名）
    public string U8PersonName_AndBanZu(string cPsnCode, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) { throw new Exception("数据库连接失败"); }

        string cName = GetDataString("select cPersonName from " + dbname + "..Person where cPersonCode='" + cPsnCode + "'", Conn);
        CloseDataConnection(Conn);
        if (cName + "" == "") { throw new Exception("不存在此员工"); }

        return cName;
    }

    [WebMethod]  //查询数据，返回数据集
    public System.Data.DataSet GetSqlDataSet(string sql, string tblName, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return null;
        }

        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds, tblName);
        if (ErrMsg != "") { CloseDataConnection(Conn); return ds; }
        ErrMsg = CloseDataConnection(Conn);
        if (ds.Tables.Count == 0) { ErrMsg = "没有查询出数据"; CloseDataConnection(Conn); return ds; }

        return ds;



    }

    [WebMethod]  //查询返回字符串数据
    public string GetSqlString(string sql, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        string cRet = "";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            cRet = dt.Rows[0][0] + "";
        }
        ErrMsg = CloseDataConnection(Conn);
        return cRet;
    }

    [WebMethod]  //查询返回字符串数据
    public bool UserExeSql(string updatesql)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        try
        {
            System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = updatesql;
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    //自动指定批号
    private System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, float iOutQty, bool bpos, System.Data.SqlClient.SqlCommand sqlcmd, string db_name)
    {
        System.Data.DataTable dtbatlist = null;
        if (bpos)
        {
            dtbatlist = GetSqlDataTable(@"select cbatch,cPosCode cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,
	                iMassDate 保质期天数,cMassUnit 保质期单位,dMadeDate 生产日期,dVDate 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,dExpirationdate 有效期计算项
                from " + db_name + @"..InvPositionSum
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "'  and iquantity>0 order by cbatch", "dtbatlist", sqlcmd);
        }
        else
        {
            dtbatlist = GetSqlDataTable(@"select cbatch,'' cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,
	                iMassDate 保质期天数,cMassUnit 保质期单位,dMdate 生产日期,dVDate 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,dExpirationdate 有效期计算项
                from " + db_name + @"..CurrentStock
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and iquantity>0 order by cbatch", "dtbatlist", sqlcmd);
        }
        System.Data.DataTable dtRet = new System.Data.DataTable("dtbat");

        for (int c = 0; c < dtbatlist.Columns.Count; c++)
        { dtRet.Columns.Add(dtbatlist.Columns[c].ColumnName); }
        //GetDataString("select 'A000000'", sqlcmd);
        for (int c = 0; c < dtbatlist.Rows.Count; c++)
        {
            float fMerOut = 0;

            if (iOutQty <= float.Parse("" + dtbatlist.Rows[c]["iquantity"]))
            {
                fMerOut = iOutQty;
                iOutQty = 0;
            }
            else
            {
                fMerOut = float.Parse("" + dtbatlist.Rows[c]["iquantity"]);
                iOutQty = iOutQty - fMerOut;
            }
            //添加行
            System.Data.DataRow dr = dtRet.NewRow();
            for (int ii = 0; ii < dtRet.Columns.Count; ii++)
            {
                dr[ii] = dtbatlist.Rows[c][ii] + "";
            }
            dr["iquantity"] = fMerOut + "";
            dtRet.Rows.Add(dr);
            if (iOutQty <= 0) break;
        }

        if (iOutQty > 0) throw new Exception("可用批次库存不够");
        return dtRet;
    }
    
    #endregion


    [WebMethod]  //U8 保存 根据采购订单 采购到货单  返回 “采购到货单ID,单据号码”
    public string U8SCM_PuArri_PO(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            KK_U8Com.U8PU_ArrivalVouch pumain = new KK_U8Com.U8PU_ArrivalVouch(Cmd, dbname);
            string cVen_name = "";
            #region  //采购到货单
            if (rd_id == 0)  //新增主表
            {
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "D";
                cc_mcode = cCodeHead + GetDataString("select right('000000000'+cast(cast(isnull(right(max(cCode),9),'000000000') as int)+1 as varchar(10)),9) from " + dbname + "..PU_ArrivalVouch where ccode like '" + cCodeHead + "%'", Cmd);
                pumain.ID = rd_id + "";
                pumain.cCode = "'" + cc_mcode + "'";
                pumain.iVTid = "8169";
                pumain.cDepCode = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : "'" + dtHead.Rows[0]["部门编码"] + "'");
                pumain.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                pumain.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                pumain.cverifier = "'" + dtHead.Rows[0]["制单人"] + "'";  //审核人
                pumain.cAuditDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                pumain.iverifystateex = "2";

                pumain.cSCCode="'01'";//发运方式
                pumain.iDiscountTaxType = "0";
                pumain.IsWfControlled = "0";  //要求审批流
                pumain.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                
                //表头项目
                System.Data.DataTable dtPUHead = GetSqlDataTable("select cVenCode,cPersonCode,cPTCode,cexch_name,nflat,iTaxRate,cpaycode cVenPUOMProtocol,cBusType from " + dbname +
                    "..PO_Pomain where cPOID='" + dtHead.Rows[0]["订单号"] + "'", "dtPUHead", Cmd);
                if (dtHead.Rows.Count > 0)
                {
                    pumain.cVenCode = "'" + dtPUHead.Rows[0]["cVenCode"] + "'";
                    cVen_name = GetDataString("select cvenname from " + dbname + "..vendor where cvencode='" + dtPUHead.Rows[0]["cVenCode"] + "'", Cmd);
                    pumain.cPersonCode = (dtPUHead.Rows[0]["cPersonCode"] + "" == "" ? "null" : "'" + dtPUHead.Rows[0]["cPersonCode"] + "'");
                    pumain.cPTCode = (dtPUHead.Rows[0]["cPTCode"] + "" == "" ? "null" : "'" + dtPUHead.Rows[0]["cPTCode"] + "'");
                    pumain.cexch_name = "'" + dtPUHead.Rows[0]["cexch_name"] + "'";
                    pumain.iExchRate = "" + dtPUHead.Rows[0]["nflat"];
                    pumain.iTaxRate = "" + dtPUHead.Rows[0]["iTaxRate"];
                    pumain.cVenPUOMProtocol = (dtPUHead.Rows[0]["cVenPUOMProtocol"] + "" == "" ? "null" : "'" + dtPUHead.Rows[0]["cVenPUOMProtocol"] + "'");
                    pumain.cBusType = "'" + dtPUHead.Rows[0]["cBusType"] + "'";
                }
                else
                {
                    throw new Exception("订单号有错误，无法找到");
                }

                if (!pumain.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
                //增加单据条码
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set csysbarcode='" + cc_mcode + "' where ID=0" + rd_id;
                Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                KK_U8Com.U8PU_ArrivalVouchs pudetail = new KK_U8Com.U8PU_ArrivalVouchs(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
                pudetail.Autoid = cAutoid + "";
                pudetail.ID = rd_id + "";
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
                Cmd.ExecuteNonQuery();
                pudetail.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                pudetail.iQuantity = dtBody.Rows[i]["数量"] + "";
                pudetail.fRealQuantity = pudetail.iQuantity;
                pudetail.fValidQuantity = pudetail.iQuantity;
                pudetail.iTaxRate = dtBody.Rows[i]["税率"] + "";
                pudetail.SoType = "0";

                //判断是否质检
                if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bPropertyCheck=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["是否质检"].ToString().CompareTo("是") == 0)
                    {
                        pudetail.bGsp = "1";
                    }
                }
                pudetail.ivouchrowno = "" + (i + 1);
                if ((dtBody.Rows[i]["含税单价"] + "").ToString().CompareTo("") != 0)  //有单价
                {
                    pudetail.iOriSum = "" + (float.Parse(dtBody.Rows[i]["数量"] + "") * float.Parse(dtBody.Rows[i]["含税单价"] + ""));
                }
                //补充订单信息
                pudetail.cordercode = "'" + dtHead.Rows[0]["订单号"] + "'";
                pudetail.bTaxCost = "1";
                pudetail.iPOsID = dtBody.Rows[i]["autoid"] + "";
                string cc_free = "" + GetDataString("select cfree1 from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["autoid"], Cmd);
                pudetail.cFree1 = (cc_free.CompareTo("") == 0 ? "null" : "'" + cc_free + "'");
                cc_free = "" + GetDataString("select cfree2 from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["autoid"], Cmd);
                pudetail.cFree2 = (cc_free.CompareTo("") == 0 ? "null" : "'" + cc_free + "'");
                pudetail.cBatch = ((dtBody.Rows[i]["批号"] + "").CompareTo("") == 0 ? "null" : "'" + dtBody.Rows[i]["批号"] + "'");

                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)+(0" + dtBody.Rows[i]["数量"] + ") where id=0" + dtBody.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtBody.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();

                if (!pudetail.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)) where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();

                //判定是否超 采购订单到货
                if (GetDataInt("select count(*) from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["autoid"] + " and isnull(iarrqty,0)>iquantity", Cmd) > 0)
                    throw new Exception(pudetail.cInvCode + "超订单到货");
            }

            #endregion

            string rd_str = rd_id + ",到货单[" + cc_mcode+"] ";
            #region   //采购入库单
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..PU_ArrivalVouchs where id=0" + rd_id + " and bGsp=0", Cmd)) > 0)
            {
                //存在，组织入库单表头   id,仓库编码,供应商编码,制单人,制单日期,来源,备注,入库类别,部门编码,采购订单,货位编码
                System.Data.DataTable dtRdHead = GetSqlDataTable("select 0 id,'" + dtHead.Rows[0]["仓库编码"] + "' 仓库编码,cvencode 供应商编码,cMaker 制单人,'" + 
                    dtHead.Rows[0]["制单日期"] + "' 制单日期,'采购到货单' 来源,'" + dtHead.Rows[0]["备注"] + "' 备注,'" + dtHead.Rows[0]["入库类别"] + 
                    "' 入库类别,cdepcode 部门编码,cCode 采购订单,'' 货位编码 from " + dbname + "..PU_ArrivalVouch where id=" + rd_id, "dtRdHead", Cmd);
                System.Data.DataTable dtRdBody = GetSqlDataTable(@"select 0 autoid,0 qc_id,autoid arrautoid,id arrid,cinvcode 存货编码,'' 存货名称,iquantity 入库数,'' 单位,
                    '' 规格型号,iquantity 原数量,'' 存货类别,'" + cc_mcode + "' 到货单号,'" + dtHead.Rows[0]["制单日期"] + @"' 到货日期,iTaxRate 税率,iOriTaxCost 含税单价,
                    iPOsID poautoid,cordercode 订单号,cbatch 批号  from " + dbname + "..PU_ArrivalVouchs where id=" + rd_id + " and bGsp=0", "dtRdBody", Cmd);
                string ccrdstr = U8_Rd01(dtRdHead, dtRdBody, dbname,Cmd);
                rd_str = rd_str + "采购入库单[" + ccrdstr.Split(',')[1] + "]";
            }
            #endregion

            #region   //来料报检
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..PU_ArrivalVouchs where id=0" + rd_id + " and bGsp=1", Cmd)) > 0)
            {
                System.Data.DataTable dtRdHead = GetSqlDataTable("select id,ccode 到货单号,cvencode 供应商编码,cMaker 制单人,'" +
                    dtHead.Rows[0]["制单日期"] + "' 制单日期,'采购到货单' 来源,'" + dtHead.Rows[0]["备注"] + @"' 备注,cdepcode 部门编码,
                    cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 
                    from " + dbname + "..PU_ArrivalVouch where id=" + rd_id, "dtRdHead", Cmd);
                System.Data.DataTable dtRdBody = GetSqlDataTable(@"select autoid arrautoid,cinvcode 存货编码,iquantity 到货数,inum,cUnitID,iinvexchrate,
                    '" + dtHead.Rows[0]["制单日期"] + @"' 到货日期,iTaxRate 税率,iOriTaxCost 含税单价,dPDate 生产日期,
                    iPOsID poautoid,cordercode 订单号,cbatch 批号,cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
                    cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37 
                    from " + dbname + "..PU_ArrivalVouchs where id=" + rd_id + " and bGsp=1", "dtRdBody", Cmd);
                string ccrdstr = U8_QC_Mer(dtRdHead, dtRdBody, dbname, Cmd);
                rd_str = rd_str + "采购入库单[" + ccrdstr.Split(',')[1] + "]";
            }
            #endregion

            tr.Commit();

            return rd_str;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg = CloseDataConnection(Conn);
        }

    }

    [WebMethod]  //U8 保存 采购入库单  返回 “采购入库单ID,单据号码”
    public string U8SCM_RD01(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            string cret=U8_Rd01(dtHead, dtBody, dbname,Cmd);

            tr.Commit();

            return cret;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }

    }

    private string U8_Rd01(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname,System.Data.SqlClient.SqlCommand Cmd)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        string cc_mcode = "";

        try
        {
            string cPosCode = "";//货位
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string targetPTRdCode = dtHead.Rows[0]["入库类别"] + "";
            string cbustype = "" + GetDataString("select isnull(cBusType,'') from " + dbname + "..PU_ArrivalVouch where id=0" + dtBody.Rows[0]["arrid"], Cmd);
            string targetPTCode = "" + GetDataString("select isnull(cPTCode,'') from " + dbname + "..PU_ArrivalVouch where id=0" + dtBody.Rows[0]["arrid"], Cmd);
            if (targetPTCode.CompareTo("") == 0)
            {
                targetPTCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetPTCode'", Cmd);//采购类型
            }
            //验证仓库编码
            if (GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "'", Cmd) == 0)
            { throw new Exception("仓库编码【" + dtHead.Rows[0]["仓库编码"] + "】不存在"); }

            bool bPosWare = false;  //代管参数
            bool bHW = false;//货位参数
            //判断仓库是否代管仓
            if (GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and bProxyWh=1", Cmd) > 0)
                bPosWare = true;
            if (GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and bWhPos=1", Cmd) > 0)
                bHW = true;

            //采购类别的赋值
            if (targetPTRdCode.CompareTo("") == 0)
            {
                if (bPosWare)
                {
                    targetPTRdCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetPTRdProxyCode'", Cmd);   //采购入库类型  代管
                }
                else
                {
                    targetPTRdCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetPTRdCode'", Cmd);   //采购入库类型
                }
            }

            if (targetPTRdCode == "") { errmsg = "没有配置采购入库类别编码"; throw new Exception(errmsg); }
            if (targetPTCode == "") { errmsg = "没有配置采购类别编码"; throw new Exception(errmsg); }

            if (bHW)
            {
                cPosCode = "" + dtHead.Rows[0]["货位编码"];
                if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + cPosCode + "'", Cmd) == 0)
                { throw new Exception("库位编码【" + cPosCode + "】不存在"); }
            }

            int iRKRowCount = 0;
            KK_U8Com.U8Rdrecord01 record01 = new KK_U8Com.U8Rdrecord01(Cmd, dbname);
            if (rd_id == 0)  //新增主表
            {
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "R" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord01 where ccode like '" + cCodeHead + "%'", Cmd);
                record01.cCode = "'" + cc_mcode + "'";
                record01.ID = rd_id;
                record01.cVouchType = "'01'";
                record01.cBusType = "'" + cbustype + "'";
                record01.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record01.cDepCode = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : "'" + dtHead.Rows[0]["部门编码"] + "'");
                record01.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record01.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                //record01.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record01.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record01.cVenCode = "'" + dtHead.Rows[0]["供应商编码"] + "'";
                record01.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                record01.cPTCode = "'" + targetPTCode + "'";
                record01.iExchRate = 1;
                record01.cExch_Name = "'人民币'";
                record01.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";

                record01.cSource = "'" + dtHead.Rows[0]["来源"] + "'";
                if (!record01.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

                //Cmd.CommandText = "update " + dbname + "..Rdrecord01 set cARVCode='" + dtHead.Rows[0]["备注"] + "' where id=" + rd_id;
                //Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (float.Parse("" + dtBody.Rows[i]["入库数"]) == 0) continue;
                iRKRowCount++;
                KK_U8Com.U8Rdrecords01 records01 = new KK_U8Com.U8Rdrecords01(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records01.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records01.ID = rd_id;
                records01.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records01.iQuantity = "" + dtBody.Rows[i]["入库数"];
                records01.cPosition = "'" + cPosCode + "'";
                records01.iMoney = "0";
                records01.iSNum = "0";
                records01.bTaxCost = 1;
                if (bPosWare)
                {
                    records01.bCosting = 0;
                    records01.bVMIUsed = "1";
                    records01.cvmivencode = "'" + dtHead.Rows[0]["供应商编码"] + "'";
                }
                //批号
                if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    records01.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                    if ((dtBody.Rows[i]["批号"] + "").CompareTo("") == 0) throw new Exception("[" + dtBody.Rows[i]["存货编码"] + "]批号不能为空");
                }
                //判断供应商是否一致
                string c_b_ven = GetDataString("select b.cvencode from " + dbname + "..PO_Podetails a inner join " + dbname + "..PO_Pomain b on a.poid=b.poid where a.id =0" + dtBody.Rows[i]["poautoid"], Cmd);
                if (c_b_ven.CompareTo(dtHead.Rows[0]["供应商编码"] + "") != 0) throw new Exception("入库单的供应商与订单的供应商不一致");

                //回写到货单累计入库数
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set fValidInQuan=isnull(fValidInQuan,0)+" + records01.iQuantity + " where autoid =0" + dtBody.Rows[i]["arrautoid"];
                Cmd.ExecuteNonQuery();
                //回写采购订单 累计入库数
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set freceivedqty=isnull(freceivedqty,0)+" + records01.iQuantity + " where id =0" + dtBody.Rows[i]["poautoid"];
                Cmd.ExecuteNonQuery();



                //records01.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records01.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records01.irowno = (i + 1);
                records01.iTaxRate = float.Parse("" + dtBody.Rows[i]["税率"]);
                records01.ioriSum = ("" + dtBody.Rows[i]["含税单价"] == "" ? "null" : "" + (float.Parse("" + dtBody.Rows[i]["入库数"]) * float.Parse("" + dtBody.Rows[i]["含税单价"])));

                //上游单据关联
                records01.iNQuantity = records01.iQuantity;
                //关联订单 到货单
                if ("" + dtBody.Rows[i]["poautoid"] != "0") records01.iPOsID = "" + dtBody.Rows[i]["poautoid"];
                if ("" + dtBody.Rows[i]["arrautoid"] != "0") records01.iArrsId = "" + dtBody.Rows[i]["arrautoid"];
                if (dtBody.Rows[i]["订单号"] + "" != "") records01.cPOID = "'" + dtBody.Rows[i]["订单号"] + "'";

                //自由项1  自由项2 的处理
                string c_free1 = "" + GetDataString("select cfree1 from " + dbname + "..PU_ArrivalVouchs where autoid =0" + dtBody.Rows[i]["arrautoid"], Cmd);
                string c_free2 = "" + GetDataString("select cfree2 from " + dbname + "..PU_ArrivalVouchs where autoid =0" + dtBody.Rows[i]["arrautoid"], Cmd);
                records01.cFree1 = (c_free1 == "" ? "null" : "'" + c_free1 + "'");
                records01.cFree2 = (c_free2 == "" ? "null" : "'" + c_free2 + "'");

                //关联检验单
                if (int.Parse("" + dtBody.Rows[i]["qc_id"]) > 0)  //存在检验单
                {
                    //records01.iCheckIds = "" + dtBody.Rows[i]["qc_id"];
                    records01.iCheckIDbaks = "" + dtBody.Rows[i]["qc_id"];
                    System.Data.DataTable dtCHecks = GetSqlDataTable("select CCHECKPERSONCODE,convert(varchar(10),DDATE,120) DDATE,CCHECKCODE,CVERIFIER,CVENCODE from " + dbname + "..QMCHECKVOUCHER where ID=0" + dtBody.Rows[i]["qc_id"], "dtCHecks", Cmd);
                    if (dtCHecks.Rows.Count == 0) throw new Exception("检验单不存在");
                    if ((dtCHecks.Rows[0]["CVERIFIER"] + "").CompareTo("") == 0) throw new Exception("检验单[" + dtCHecks.Rows[0]["CCHECKCODE"] + "]未审核");
                    records01.cCheckPersonCode = "'" + dtCHecks.Rows[0]["CCHECKPERSONCODE"] + "'";
                    records01.dCheckDate = "'" + dtCHecks.Rows[0]["DDATE"] + "'";
                    records01.cCheckCode = "'" + dtCHecks.Rows[0]["CCHECKCODE"] + "'";
                    records01.chvencode = "'" + dtCHecks.Rows[0]["CVENCODE"] + "'";
                    //回写主表  cSource
                    if (iRKRowCount == 1)
                    {
                        Cmd.CommandText = "update " + dbname + "..Rdrecord01 set cSource='来料检验单' where id=0" + record01.ID;
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string c_sorue = "" + GetDataString("select cSource from " + dbname + "..Rdrecord01 where id=0" + record01.ID, Cmd);
                        if (c_sorue.CompareTo("来料检验单") != 0) throw new Exception("检验物资能和非检验物资一起入库");
                    }

                    //回写 检验单 累计入库数
                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set fsumquantity=isnull(fsumquantity,0)+" + records01.iQuantity + " where id =0" + dtBody.Rows[i]["qc_id"];
                    Cmd.ExecuteNonQuery();
                    //判断是否超检验单
                    if (GetDataInt("select count(*) from " + dbname + "..QMCHECKVOUCHER where id=0" + dtBody.Rows[i]["qc_id"] + " and isnull(fsumquantity,0)>isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)", Cmd) > 0)
                    {
                        throw new Exception("超检验单[" + dtCHecks.Rows[0]["CCHECKCODE"] + "]入库");
                    }
                }

                //保存数据
                if (!records01.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


                if (dtBody.Rows[i]["到货日期"] + "" != "")
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbarvcode='" + dtBody.Rows[i]["到货单号"] +
                        "',dbarvdate='" + dtBody.Rows[i]["到货日期"] + "' where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }

                //是否超到货单检查
                if ("" + dtBody.Rows[i]["arrautoid"] != "0")
                {
                    float fAllarr = float.Parse(GetDataString("select isnull(sum(isnull(iQuantity,0)),0) from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname +
                            "..PU_ArrivalVouch b on a.id=b.id where a.Autoid=" + dtBody.Rows[i]["arrautoid"] + " and isnull(b.cverifier,'')<>''", Cmd));

                    float fAllRk = float.Parse(GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..rdrecords01 where iArrsId=" + dtBody.Rows[i]["arrautoid"], Cmd));
                    if (fAllRk > fAllarr)
                    {
                        errmsg = "错误." + records01.cInvCode + "超到货入库";
                        throw new Exception(errmsg);
                    }
                }

                //判断是否货位管理
                #region
                if (bHW)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cfree1,cfree2) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + cPosCode + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["入库数"] +
                        ",null,'" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'01','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] +
                        "'," + (c_free1 == "" ? "null" : "'" + c_free1 + "'") + "," + (c_free2 == "" ? "null" : "'" + c_free2 + "'") + ")";
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    Cmd.CommandText = "update " + dbname + "..rdrecords01 set iposflag=1 where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cPosCode='" + cPosCode + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + cPosCode + "','" + dtBody.Rows[i]["存货编码"] + @"',0,'',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["入库数"] +
                        ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                        "' and cPosCode='" + cPosCode + "' and cfree1='" + c_free1 + "' and cfree2='" + c_free2 + "'";
                    Cmd.ExecuteNonQuery();
                }
                #endregion
            }

            if (iRKRowCount == 0) throw new Exception("没有可以保存的入库单行");
            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //来料到货报检
    private string U8_QC_Mer(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, System.Data.SqlClient.SqlCommand Cmd)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        try
        {
            string errmsg = "";
            if (dtHead.Rows.Count == 0 || dtBody.Rows.Count == 0) throw new Exception("没有合法记录，不能报检");
            string DBName = dbname;
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            #region//写主表
            KK_U8Com.U8QMINSPECTVOUCHER istmain = new KK_U8Com.U8QMINSPECTVOUCHER(Cmd, dbname);

            string vouCode = GetDataString("select 'QD'+right('0000000000'+cast(cast(isnull(max(replace(CINSPECTCODE,'QD','')),'0') as int)+1 as varchar(10)),8) from " + DBName + "..QMINSPECTVOUCHER where cvouchtype='QM01' and CINSPECTCODE like 'QD%'", Cmd);
            istmain.CINSPECTCODE = "'" + vouCode + "'";
            string cnewid = GetDataString("select newid()", Cmd);
            istmain.INSPECTGUID = "'" + cnewid + "'";
            istmain.ID = GetDataString("select isnull(max(id),0)+1 from " + DBName + "..QMINSPECTVOUCHER", Cmd);
            istmain.CVOUCHTYPE = "'QM01'";
            istmain.CSOURCECODE = "'" + dtHead.Rows[0]["到货单号"] + "'"; //到货单号
            istmain.CSOURCEID = "" + dtHead.Rows[0]["id"];  //生产订单  主表标识
            istmain.CDEPCODE = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : dtHead.Rows[0]["部门编码"] + ""); 
            istmain.CINSPECTDEPCODE = istmain.CDEPCODE;
            istmain.CMAKER = "'" + dtHead.Rows[0]["制单人"] + "'";
            istmain.DDATE = "'" + dtHead.Rows[0]["制单日期"] + "'";
            istmain.CVERIFIER = "'" + dtHead.Rows[0]["制单人"] + "'";
            istmain.DVERIFYDATE = "'" + dtHead.Rows[0]["制单日期"] + "'";
            istmain.DARRIVALDATE = "'" + dtHead.Rows[0]["制单日期"] + "'";
            istmain.CVENCODE = "'" + dtHead.Rows[0]["供应商编码"] + "'";
            istmain.CTIME = "'" + GetDataString("select right(convert(varchar(20),getdate(),120),8)", Cmd) + "'";
            istmain.CSOURCE = "'到货单'";
            istmain.IVTID = "351";
            istmain.CCHECKTYPECODE = "'ARR'";
            istmain.DMAKETIME = "getdate()";
            istmain.DVERIFYTIME = "getdate()";
            istmain.iPrintCount = "0";

            #region //主表自定义项
            istmain.CDEFINE1 = "'" + dtHead.Rows[0]["cdefine1"] + "'";
            istmain.CDEFINE2 = "'" + dtHead.Rows[0]["cdefine2"] + "'";
            istmain.CDEFINE3 = "'" + dtHead.Rows[0]["cdefine3"] + "'";
            istmain.CDEFINE4 = ((dtHead.Rows[0]["cdefine4"] + "").CompareTo("") == 0 ? "null" : dtHead.Rows[0]["cdefine4"] + "");
            istmain.CDEFINE5 = ((dtHead.Rows[0]["cdefine5"] + "").CompareTo("") == 0 ? "0" : dtHead.Rows[0]["cdefine5"] + "");
            istmain.CDEFINE6 = ((dtHead.Rows[0]["cdefine6"] + "").CompareTo("") == 0 ? "null" : dtHead.Rows[0]["cdefine6"] + "");
            istmain.CDEFINE7 = ((dtHead.Rows[0]["cdefine7"] + "").CompareTo("") == 0 ? "0" : dtHead.Rows[0]["cdefine7"] + "");
            istmain.CDEFINE8 = "'" + dtHead.Rows[0]["cdefine8"] + "'";
            istmain.CDEFINE9 = "'" + dtHead.Rows[0]["cdefine9"] + "'";
            istmain.CDEFINE10 = "'" + dtHead.Rows[0]["cdefine10"] + "'";
            istmain.CDEFINE11 = "'" + dtHead.Rows[0]["cdefine11"] + "'";
            istmain.CDEFINE12 = "'" + dtHead.Rows[0]["cdefine12"] + "'";
            istmain.CDEFINE13 = "'" + dtHead.Rows[0]["cdefine13"] + "'";
            istmain.CDEFINE14 = "'" + dtHead.Rows[0]["cdefine14"] + "'";
            istmain.CDEFINE15 = ((dtHead.Rows[0]["cdefine15"] + "").CompareTo("") == 0 ? "0" : dtHead.Rows[0]["cdefine15"] + "");
            istmain.CDEFINE16 = ((dtHead.Rows[0]["cdefine16"] + "").CompareTo("") == 0 ? "0" : dtHead.Rows[0]["cdefine16"] + "");
            #endregion

            if (!istmain.InsertToDB(ref errmsg)) { Cmd.Transaction.Rollback(); throw new Exception(errmsg); }
            #endregion

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                KK_U8Com.U8QMINSPECTVOUCHERS istdetail = new KK_U8Com.U8QMINSPECTVOUCHERS(Cmd, dbname);
                #region  //子表
                float f_Qty_in = float.Parse(dtBody.Rows[i]["到货数"] + "");  //数量
                string cc_invcode = dtBody.Rows[i]["存货编码"] + "";

                //子表
                if (f_Qty_in <= 0) throw new Exception("数量必须大于0");
                istdetail.AUTOID = GetDataString("select isnull(max(autoid),0)+1 from " + DBName + "..QMINSPECTVOUCHERS", Cmd);
                istdetail.ID = istmain.ID;
                istdetail.SOURCEAUTOID = dtBody.Rows[i]["arrautoid"] + "";
                istdetail.ITESTSTYLE = "0";
                istdetail.CINVCODE = "'" + cc_invcode + "'";
                string cc_batch = dtBody.Rows[i]["批号"] + "";
                istdetail.FQUANTITY = "" + f_Qty_in;  //到货数量   inum,cUnitID,iinvexchrate,
                istdetail.FNUM = ((dtBody.Rows[i]["inum"] + "").CompareTo("") == 0 ? "null" : dtBody.Rows[i]["inum"] + "");
                istdetail.CUNITID = ((dtBody.Rows[i]["cUnitID"] + "").CompareTo("") == 0 ? "null" : dtBody.Rows[i]["cUnitID"] + "");
                istdetail.FCHANGRATE = ((dtBody.Rows[i]["iinvexchrate"] + "").CompareTo("") == 0 ? "null" : dtBody.Rows[i]["iinvexchrate"] + "");
                istdetail.DPRODATE = (dtBody.Rows[i]["生产日期"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["生产日期"] + "'");
                istdetail.CPOCODE = "'" + dtBody.Rows[i]["订单号"] + "'";
                istdetail.IORDERTYPE = "0";
                istdetail.BEXIGENCY = "0";

                string cToday = GetDataString("select convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120)", Cmd);
                if (dtBody.Rows[i]["生产日期"] + "" != "") cToday = dtBody.Rows[i]["生产日期"] + "";
                #region//批次管理和保质期管理
                if (int.Parse(GetDataString("select count(*) FROM " + DBName + "..inventory where cinvcode='" + cc_invcode + "' and bInvBatch=1", Cmd)) > 0)
                {
                    if (cc_batch.CompareTo("") == 0) throw new Exception("有批次管理，请输入批号");
                    istdetail.CBATCH = "'" + cc_batch + "'";
                    //生产日期 保质期
                    System.Data.DataTable dtBZQ = GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + cToday + @"' 生产日期,
                                                                        convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + cToday + @"')
					                                                                        when cMassUnit=2 then DATEADD(month,iMassDate,'" + cToday + @"')
					                                                                        else DATEADD(day,iMassDate,'" + cToday + @"') end)
                                                                        ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                                                                        from " + DBName + "..inventory i left join " + DBName + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                                                                        where cinvcode='" + cc_invcode + "' and bInvQuality=1", "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count > 0)
                    {
                        istdetail.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                        istdetail.IMASSDATE = dtBZQ.Rows[0]["保质期天数"] + "";
                        istdetail.DVDATE = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                        istdetail.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                        istdetail.DPRODATE = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                        istdetail.CMASSUNIT = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                    }
                }
                #endregion

                #region  //自由项管理
                istdetail.CDEFINE22 = "'" + dtBody.Rows[i]["cdefine22"] + "'";
                istdetail.CDEFINE23 = "'" + dtBody.Rows[i]["cdefine23"] + "'";
                istdetail.CDEFINE24 = "'" + dtBody.Rows[i]["cdefine24"] + "'";
                istdetail.CDEFINE25 = "'" + dtBody.Rows[i]["cdefine25"] + "'";
                istdetail.CDEFINE26 = ((dtBody.Rows[i]["cdefine26"] + "").CompareTo("") == 0 ? "0" : dtBody.Rows[i]["cdefine26"] + "");
                istdetail.CDEFINE27 = ((dtBody.Rows[i]["cdefine27"] + "").CompareTo("") == 0 ? "0" : dtBody.Rows[i]["cdefine27"] + "");
                istdetail.CDEFINE28 = "'" + dtBody.Rows[i]["cdefine28"] + "'";
                istdetail.CDEFINE29 = "'" + dtBody.Rows[i]["cdefine29"] + "'";
                istdetail.CDEFINE30 = "'" + dtBody.Rows[i]["cdefine30"] + "'";
                istdetail.CDEFINE31 = "'" + dtBody.Rows[i]["cdefine31"] + "'";
                istdetail.CDEFINE32 = "'" + dtBody.Rows[i]["cdefine32"] + "'";
                istdetail.CDEFINE33 = "'" + dtBody.Rows[i]["cdefine33"] + "'";
                istdetail.CDEFINE34 = ((dtBody.Rows[i]["cdefine34"] + "").CompareTo("") == 0 ? "0" : dtBody.Rows[i]["cdefine34"] + "");
                istdetail.CDEFINE35 = ((dtBody.Rows[i]["cdefine35"] + "").CompareTo("") == 0 ? "0" : dtBody.Rows[i]["cdefine35"] + "");
                istdetail.CDEFINE36 = ((dtBody.Rows[i]["cdefine36"] + "").CompareTo("") == 0 ? "null" : dtBody.Rows[i]["cdefine36"] + "");
                istdetail.CDEFINE37 = ((dtBody.Rows[i]["cdefine37"] + "").CompareTo("") == 0 ? "null" : dtBody.Rows[i]["cdefine37"] + "");
                #endregion

                if (!istdetail.InsertToDB(ref errmsg)) { Cmd.Transaction.Rollback(); throw new Exception(errmsg); }

                //回写 生产订单
                Cmd.CommandText = "update " + DBName + "..PU_ArrivalVouchs set fInspectQuantity=isnull(fInspectQuantity,0)+(0" + istdetail.FQUANTITY + @"),
                    fInspectNum=isnull(fInspectNum,0)+(" + istdetail.FNUM + @"),bInspect=1 where autoid=0" + dtBody.Rows[i]["arrautoid"];
                Cmd.ExecuteNonQuery();
                #endregion

                #region  //上下游单据关系
                //判定上下工序逻辑关系
                float f_qc_count = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select count(*) from " + dbname + @"..PU_ArrivalVouchs a 
                    where a.autoid=0" + dtBody.Rows[i]["arrautoid"] + " and isnull(fInspectQuantity,0)>iQuantity"));
                if (f_qc_count>0) throw new Exception("超出可报检数");

                #endregion
            }

            return istmain.ID + "," + vouCode;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 保存 其他入库单  返回 “其他入库单ID,单据号码”
    public string U8SCM_RD08(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            KK_U8Com.U8Rdrecord08 record08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
            int iPosSet = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "R" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord08 where ccode like '" + cCodeHead + "%'", Cmd);
                record08.cCode = "'" + cc_mcode + "'";
                record08.ID = rd_id;
                record08.cVouchType = "'08'";
                record08.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record08.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record08.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record08.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                record08.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record08.cDepCode = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : "'" + dtHead.Rows[0]["部门编码"] + "'");
                record08.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record08.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record08.iExchRate = "1";
                record08.cExch_Name = "'人民币'";

                record08.cSource = "'库存'";
                if (!record08.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                //if (iPosSet > 0)
                //{
                //    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                //    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                //}

                KK_U8Com.U8Rdrecords08 records08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records08.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records08.ID = rd_id;
                records08.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records08.iQuantity = "" + dtBody.Rows[i]["数量"];
                //if (iPosSet > 0) records08.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                //records08.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records08.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records08.irowno = (i + 1);
                records08.iNQuantity = records08.iQuantity;

                //判断是否有批次管理
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "") throw new Exception(dtBody.Rows[i]["存货编码"] + "必须输入批号");
                    records08.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                }
                //保存数据
                if (!records08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region //作废  货位管理
//                if (iPosSet > 0)
//                {
//                    //添加货位记录
//                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
//                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
//                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
//                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'08','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
//                    Cmd.ExecuteNonQuery();

//                    ////指定货位
//                    //Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
//                    //Cmd.ExecuteNonQuery();

//                    //修改货位库存
//                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
//                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
//                    {
//                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
//                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
//                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
//                        '','','','','','','','','','','','',0,0)";
//                        Cmd.ExecuteNonQuery();
//                    }

//                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] +
//                        "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
//                    Cmd.ExecuteNonQuery();

//                    //判定负库存问题
//                    if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
//                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
//                    {
//                        throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
//                    }
//                }

                #endregion

                //判定 库存账 负库存问题
                //if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                //    "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                //{
                //    throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                //}

            }
            tr.Commit();

            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg = CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //U8 保存 其他出库单  返回 “其他出库单ID,单据号码”
    public string U8SCM_RD09(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

            KK_U8Com.U8Rdrecord09 record09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);
            int iPosSet = 0;
            int iVmiSet = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                iVmiSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bProxyWh,0)=1", Cmd));
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "C" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord09 where ccode like '" + cCodeHead + "%'", Cmd);
                record09.cCode = "'" + cc_mcode + "'";
                record09.ID = rd_id;
                record09.cVouchType = "'09'";
                record09.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record09.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record09.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                record09.VT_ID = 85;
                //record09.cDefine3 = "'" + dtHead.Rows[0]["领用人"] + "'";  //领料人
                record09.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                record09.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record09.cDepCode = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : "'" + dtHead.Rows[0]["部门编码"] + "'");
                record09.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record09.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record09.iExchRate = "1";
                record09.cExch_Name = "'人民币'";

                record09.cSource = "'库存'";
                if (!record09.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }

                KK_U8Com.U8Rdrecords09 records09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records09.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records09.ID = rd_id;
                records09.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records09.iQuantity = "" + dtBody.Rows[i]["数量"];
                if (iPosSet > 0) records09.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                //records09.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records09.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records09.irowno = (i + 1);
                records09.iNQuantity = records09.iQuantity;
                //判断是否有批次管理
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "") throw new Exception(dtBody.Rows[i]["存货编码"] + "必须输入批号");
                    records09.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                }
                
                //代管仓库判定
                if (iVmiSet > 0)
                {
                    if (dtBody.Rows[i]["供应商"] + "" == "")
                    {
                        throw new Exception("代管物资必须 输入供应商信息");
                    }
                    records09.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                    records09.bVMIUsed = "1";
                }

                //保存数据
                if (!records09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                if (iPosSet > 0)
                {
                    //添加货位记录
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'09','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                    Cmd.ExecuteNonQuery();

                    ////指定货位
                    //Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                    //Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }

                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] +
                        "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
                    Cmd.ExecuteNonQuery();

                    //判定负库存问题
                    if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                    {
                        throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                    }
                }

                //判定 库存账 负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                }

            }
            tr.Commit();

            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg = CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //U8 保存 材料出库单  返回 “材料出库单ID,单据号码”
    public string U8SCM_RD11(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);
            int iPosSet = 0;
            int iVmiSet = 0;
            string modid = "";//生产订单
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                iVmiSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bProxyWh,0)=1", Cmd));
                modid = "" + dtHead.Rows[0]["modid"];
                if (modid == "") throw new Exception("没有找到生产订单，请确认 生产订单号和 行号是否存在");

                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "C" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord11 where ccode like '" + cCodeHead + "%'", Cmd);
                record11.cCode = "'" + cc_mcode + "'";
                record11.ID = rd_id;
                record11.cVouchType = "'11'";
                record11.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record11.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record11.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record11.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record11.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record11.iExchRate = "1";
                record11.cExch_Name = "'人民币'";

                record11.cSource = "'库存'";
                if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            int iAllRow = 0;
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }
                if (dtBody.Rows[i]["拣货"].ToString().CompareTo("否") == 0) continue;

                string f_stok_qty = GetDataString("select isnull(sum(iquantity),0) from " + dbname + @"..CurrentStock where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + dtBody.Rows[i]["批号"] + "'", Cmd);
                string f_rds_qty = "" + dtBody.Rows[i]["数量"];
                if (float.Parse("" + dtBody.Rows[i]["数量"]) > float.Parse(f_stok_qty))
                {
                    f_rds_qty = f_stok_qty;
                }
                if (float.Parse(f_stok_qty) == 0) continue;

                KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records11.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records11.ID = rd_id;
                records11.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records11.iQuantity = f_rds_qty;
                if (iPosSet > 0) records11.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records11.irowno = (i + 1);
                records11.iNQuantity = records11.iQuantity;
                record11.VT_ID = 65;
                //判断是否有批次管理
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "") throw new Exception(dtBody.Rows[i]["存货编码"] + "必须输入批号");
                    records11.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                }

                //代管仓库判定
                if (iVmiSet > 0)
                {
                    if (dtBody.Rows[i]["供应商"] + "" == "")
                    {
                        throw new Exception("代管物资必须 输入供应商信息");
                    }
                    records11.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                    records11.bVMIUsed = "1";
                }

                #region   //生产订单信息
                //string cmollateid = "" + GetDataString("select AllocateId from " + dbname + "..mom_moallocate where modid=0" + modid + " and WIPType=3 and invcode=" + records11.cInvCode, Cmd);
                string cmollateid = "" + GetDataString("select AllocateId from " + dbname + "..mom_moallocate where WIPType=3 and AllocateId=0" + dtBody.Rows[i]["mollateid"], Cmd);
                if (cmollateid == "") throw new Exception("生产订单无此材料[" + dtBody.Rows[i]["存货编码"] + "],请检查此材料是否为倒冲材料");
                records11.iMPoIds = cmollateid;
                records11.ipesodid = cmollateid;
                records11.imoseq = "" + dtHead.Rows[0]["行号"];
                records11.ipesoseq = "" + dtHead.Rows[0]["行号"];
                records11.cmocode = "'" + dtHead.Rows[0]["生产订单"] + "'";
                records11.cpesocode = "'" + dtHead.Rows[0]["生产订单"] + "'";
                //records11.cMoLotCode = "''";
                records11.ipesotype = "7";
                records11.invcode = "'" + GetDataString("select invcode from " + dbname + "..mom_orderdetail where modid=0" + modid, Cmd) + "'";
                #endregion

                iAllRow++;
                //保存数据
                if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region   //回写生产订单信息
                Cmd.CommandText = "update " + dbname + "..mom_moallocate set IssQty=isnull(IssQty,0)+(0" + records11.iQuantity + ") where AllocateId=0" + cmollateid;
                Cmd.ExecuteNonQuery();
                //判断是否超 领用
                if (GetDataInt("select count(*) from " + dbname + "..mom_moallocate where AllocateId=0" + cmollateid + " and isnull(IssQty,0)>Qty", Cmd) > 0)
                    throw new Exception("材料" + records11.cInvCode + "超出生产订单可领用数量");
                #endregion


                #region  //货位处理
                if (iPosSet > 0)
                {
                    //添加货位记录
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + records11.iQuantity +
                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'11','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                    Cmd.ExecuteNonQuery();

                    ////指定货位
                    //Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                    //Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }

                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + records11.iQuantity + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] +
                        "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
                    Cmd.ExecuteNonQuery();

                    //判定负库存问题
                    if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                        "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                    {
                        throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                    }
                }
                #endregion

                //判定 库存账 负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                }

            }

            if (iAllRow == 0) throw new Exception("没有成功出库，可能所有行库存都为0");

            tr.Commit();

            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg = CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //U8 保存 调拨单  返回 “调拨单ID,单据号码”
    public string U8SCM_Trans(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

            KK_U8Com.U8TransVouch dbmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
            KK_U8Com.U8Rdrecord08 rd08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
            KK_U8Com.U8Rdrecord09 rd09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);
            int iPosSet = 0;
            int iVmiSet = 0;
            int iVmiSet_In = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                iVmiSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bProxyWh,0)=1", Cmd));
                iVmiSet_In = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["调入仓库"] + "' and isnull(bProxyWh,0)=1", Cmd));
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "T" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(ctvcode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..TransVouch where ctvcode like '" + cCodeHead + "%'", Cmd);
                dbmain.cTVCode = "'" + cc_mcode + "'";
                dbmain.ID = rd_id;
                dbmain.dTVDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                dbmain.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                dbmain.dVerifyDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                dbmain.cVerifyPerson = "'" + dtHead.Rows[0]["制单人"] + "'";
                dbmain.IsWfControlled = 0;
                dbmain.cOWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                dbmain.cIWhCode = "'" + dtHead.Rows[0]["调入仓库"] + "'";
                dbmain.cORdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                dbmain.cIRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                dbmain.cODepCode = (dtHead.Rows[0]["调出部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调出部门"] + "'");
                dbmain.cIDepCode = (dtHead.Rows[0]["调入部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调入部门"] + "'");
                dbmain.cTVMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                if (!dbmain.InsertToDB(targetAccId, false, ref errmsg)) { throw new Exception(errmsg); }

                #region //审核调拨单 形成其他入库和其他出库单
                //新增其他出库单主表
                rd09.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rd09.bredvouch = "0";
                rd09.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                rd09.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                rd09.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                rd09.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                rd09.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                rd09.cCode = "'TO" + cc_mcode.Substring(1) + "'";
                rd09.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                rd09.cDepCode = (dtHead.Rows[0]["调出部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调出部门"] + "'");
                rd09.iExchRate = "1";
                rd09.cExch_Name = "'人民币'";
                rd09.cBusCode = "'" + cc_mcode + "'";
                rd09.cBusType = "'调拨出库'";
                rd09.cSource = "'调拨'";
                rd09.cDefine3 = "'AUTOINPUT'";
                //rd09.VT_ID = 131395;
                if (!rd09.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }


                //新增其他入库单主表
                rd08.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rd08.bredvouch = "0";
                rd08.cWhCode = "'" + dtHead.Rows[0]["调入仓库"] + "'";
                rd08.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                rd08.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                rd08.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                rd08.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                rd08.cCode = "'TI" + cc_mcode.Substring(1) + "'";
                rd08.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                rd08.iExchRate = "1";
                rd08.cExch_Name = "'人民币'";
                rd08.cBusCode = "'" + cc_mcode + "'";
                rd08.cBusType = "'调拨入库'";
                rd08.cSource = "'调拨'";
                rd08.cDepCode = (dtHead.Rows[0]["调入部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调入部门"] + "'");
                rd08.cDefine3 = "'AUTOINPUT'";
                if (!rd08.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
                #endregion


            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (float.Parse("" + dtBody.Rows[i]["数量"]) == 0) continue;
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }
                KK_U8Com.U8TransVouchs dbdetail = new KK_U8Com.U8TransVouchs(Cmd, dbname);
                dbdetail.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();

                dbdetail.ID = dbmain.ID;
                dbdetail.cTVCode = dbmain.cTVCode;
                dbdetail.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                dbdetail.iTVQuantity = "" + dtBody.Rows[i]["数量"];

                dbdetail.irowno = (i + 1);

                //判断是否有批次管理
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "") throw new Exception(dtBody.Rows[i]["存货编码"] + "必须输入批号");
                }

                string cccbatch = GetDataString("SELECT case when bInvBatch=1 then '" + dtBody.Rows[i]["批号"] + "' else '' end FROM " + dbname + "..Inventory where cinvcode=" + dbdetail.cInvCode, Cmd);
                dbdetail.cTVBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");


                if (iPosSet > 0) dbdetail.coutposcode = "'" + dtBody.Rows[i]["区域"] + "'";
                dbdetail.bCosting = 0;


                //代管仓库判定
                //if (iVmiSet > 0)
                //{
                //    if (dtBody.Rows[i]["供应商"] + "" == "")
                //    {
                //        throw new Exception("代管物资必须 输入供应商信息");
                //    }
                //    dbdetail.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                //}

                if (!dbdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


                //写入其他出库单
                #region
                KK_U8Com.U8Rdrecords09 rds09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);
                rds09.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds09.ID = rd09.ID;
                rds09.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                rds09.iQuantity = "" + dtBody.Rows[i]["数量"];
                rds09.irowno = (i + 1);
                rds09.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds09.iTrIds = "" + dbdetail.AutoID;
                rds09.cDefine24 = "'AUTOINPUT'";

                //代管仓库判定
                //if (iVmiSet > 0)
                //{
                //    if (dtBody.Rows[i]["供应商"] + "" == "")
                //    {
                //        throw new Exception("代管物资必须 输入供应商信息");
                //    }
                //    rds09.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                //    rds09.bVMIUsed = "1";
                //}

                if (!rds09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                //判定 库存账 负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + cccbatch + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                }



                #endregion



                //写入其他入库单
                #region
                KK_U8Com.U8Rdrecords08 rds08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);
                rds08.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds08.ID = rd08.ID;
                rds08.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                rds08.iQuantity = "" + dtBody.Rows[i]["数量"];
                rds08.irowno = (i + 1);
                rds08.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds08.iTrIds = "" + dbdetail.AutoID;
                rds08.cDefine24 = "'AUTOINPUT'";
                //代管仓库判定
                //if (iVmiSet_In > 0)
                //{
                //    if (dtBody.Rows[i]["供应商"] + "" == "")
                //    {
                //        throw new Exception("代管物资必须 输入供应商信息");
                //    }
                //    rds08.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                //    rds08.bVMIUsed = "1";
                //    rds08.bCosting = "0";
                //}

                if (!rds08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
                #endregion


            }


            tr.Commit();

            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //U8 扫描流转卡  按生产订单入库
    public string U8SCM_RD10_MOMOrder(string modid, System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string acc_id)
    {
        string pfcode = modid;
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        string errmsg = "";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        string ccRd10Code = "";
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string cToday = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
            //获得 部门编码信息
            string ccDepcode = "" + GetDataString("select b.MDeptCode from " + dbname + "..mom_orderdetail b where b.modid='" + modid + "'", Cmd);
            if (ccDepcode.CompareTo("") == 0)
            {
                throw new Exception("生产订单没有录入生产部门");
            }

            #region   //产品入库单
            KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
            //主表
            int rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            string cCodeHead = "G" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd);
            string cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord10 where ccode like '" + cCodeHead + "%'", Cmd);
            record10.cCode = "'" + cc_mcode + "'";
            ccRd10Code = cc_mcode;
            record10.ID = rd_id;
            record10.cVouchType = "'10'";
            record10.cBusType = "'成品入库'";
            record10.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
            record10.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
            record10.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            record10.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
            record10.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            record10.cDepCode = "'" + ccDepcode + "'";
            record10.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            record10.cMemo = "''";
            record10.iExchRate = "1";
            record10.cExch_Name = "'人民币'";
            record10.cSource = "'生产订单'";
            record10.cDefine3 = "'" + pfcode + "'";
            if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + record10.cWhCode + " and bInCost=1", Cmd);
            int irowno = 0;
            KK_U8Com.U8Rdrecords10 records10 = new KK_U8Com.U8Rdrecords10(Cmd, dbname);
            int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            records10.AutoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            records10.ID = rd_id;
            records10.iQuantity = "" + dtBody.Rows[0]["入库数"];
            records10.irowno = (irowno++);
            records10.iNQuantity = records10.iQuantity;
            records10.iordertype = "0";
            if (ibcostcount == 0) records10.bCosting = "0";

            //生产订单信息
            System.Data.DataTable dtfclist = GetSqlDataTable(@"select top 1 c.mocode,b.modid,b.SortSeq,MoLotCode,b.invcode,d.opseq,d.description,e.WcCode
                from " + dbname + @"..mom_orderdetail b 
                inner join " + dbname + @"..mom_order c on b.moid=c.moid
                left join " + dbname + @"..sfc_moroutingdetail d on b.modid=d.modid
                left join " + dbname + @"..sfc_workcenter e on d.wcid=e.wcid
                where b.modid='" + modid + "' order by d.opseq desc", "dtfclist", Cmd);

            if (dtfclist.Rows.Count > 0)
            {
                records10.cInvCode = "'" + dtfclist.Rows[0]["invcode"] + "'";
                records10.iMPoIds = "" + dtfclist.Rows[0]["modid"];
                records10.imoseq = "" + dtfclist.Rows[0]["SortSeq"];
                records10.cMoLotCode = "'" + dtfclist.Rows[0]["MoLotCode"] + "'";
                records10.cmocode = "'" + dtfclist.Rows[0]["mocode"] + "'";
                if ("" + dtfclist.Rows[0]["WcCode"] != "")
                {
                    records10.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
                    records10.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
                    records10.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
                }

                //批号
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    if ((dtBody.Rows[0]["入库批号"] + "").CompareTo("") == 0)
                    {
                        throw new Exception("有批次管理，批号不能为空");
                    }
                    records10.cBatch = "'" + dtBody.Rows[0]["入库批号"] + "'";
                }

                //生产日期 保质期
                System.Data.DataTable dtBZQ = GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + cToday + @"' 生产日期,
                                                                        convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + cToday + @"')
					                                                                        when cMassUnit=2 then DATEADD(month,iMassDate,'" + cToday + @"')
					                                                                        else DATEADD(day,iMassDate,'" + cToday + @"') end)
                                                                        ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                                                                        from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                                                                        where cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and bInvQuality=1", "dtBZQ", Cmd);
                if (dtBZQ.Rows.Count > 0)
                {
                    records10.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    records10.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    records10.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records10.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records10.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    records10.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //记录废品数
                float f_feipin_count = 0;

                //判断是否按照领用比例入库
                float f_Qty_in = float.Parse(records10.iQuantity);
                //总合格品入库数
                float f_qtyinall = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + dbname + "..rdrecords10 where iMPoIds=0" + dtfclist.Rows[0]["modid"] +
                    " and cinvcode='" + dtfclist.Rows[0]["invcode"] + "'", Cmd));

                //判定是否存在非倒冲零件
                int illrow = 0;
                //关键零用控制量 
                float f_ll_qty = 0;
                string ccc_inv = dtfclist.Rows[0]["invcode"] + "";
                //关键件判定
                illrow = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart=1 ", Cmd));
                //不合格数取 关键件 齐套量
                f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(IssQty*BaseQtyD/BaseQtyN+0.5,0)),0) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart=1", Cmd));

                if (f_ll_qty < f_Qty_in + f_qtyinall && illrow > 0)
                {
                    throw new Exception("不能超关键件领料入库");
                }

                //若未合格品的话，判定 所有配件
                //if ((dtHead.Rows[0]["入库类别"] + "").CompareTo("103") == 0 && ccc_inv.Substring(0, 2).CompareTo("04") == 0)
                //{
                //    illrow = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3  and b.bInvKeyPart<>1", Cmd));
                //    //合格品取 所有件 齐套量
                //    f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(IssQty*BaseQtyD/BaseQtyN+0.5,0)),0) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart<>1", Cmd));
                //    if (f_ll_qty < f_Qty_in + f_qtyinall && illrow > 0)
                //    {
                //        throw new Exception("不能超领料入库");
                //    }
                //}

                //回写生产订单信息
                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + records10.iQuantity +
                    "),Define35=isnull(Define35,0)+" + f_feipin_count + " where modid=0" + dtfclist.Rows[0]["modid"];
                Cmd.ExecuteNonQuery();

                //关闭生产订单
                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='" + dtHead.Rows[0]["制单人"] +
                    "',CloseTime=getdate(),CloseDate=convert(varchar(10),getdate(),120),Status=4 where modid=0" + dtfclist.Rows[0]["modid"] + " and isnull(QualifiedInQty,0)>=Qty";
                Cmd.ExecuteNonQuery();

            }
            else
            {
                throw new Exception("没有找到生产订单信息");
            }


            //保存数据
            if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            #endregion


            #region  //倒冲材料出库单
            //mom_moallocate
            string cOldWhcode = ""; string cNewWhcode = "";
            int irow = 0;
            string cOutRdCode = "" + GetDataString("select cvalue from " + dbname + @"..T_Parameter where cpid='cOutRdCode'", Cmd);
            System.Data.DataTable dtMolt = GetSqlDataTable("select AllocateId,WhCode,InvCode,(0" + dtBody.Rows[0]["入库数"] + ")*BaseQtyN/BaseQtyD iQty from " +
                dbname + @"..mom_moallocate where modid=0" + dtfclist.Rows[0]["modid"] + " and WIPType=1 order by WhCode,InvCode", "dtMolt", Cmd);
            int iOutRDID = 0;
            for (int k = 0; k < dtMolt.Rows.Count; k++)
            {
                bool bbpos = false;
                //写表头
                cNewWhcode = "" + dtMolt.Rows[k]["WhCode"];
                if (cOldWhcode.CompareTo(cNewWhcode) != 0)
                {
                    if (cOutRdCode == "") throw new Exception("没有设置倒冲材料出库类别编码");
                    //表头赋值
                    if (GetDataInt("select count(*) from " + dbname + @"..warehouse where cwhcode='" + cNewWhcode + "' and bWhPos=1", Cmd) > 0)
                    { bbpos = true; }
                    else { bbpos = false; }

                    KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);
                    iOutRDID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                    Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                    Cmd.ExecuteNonQuery();
                    cCodeHead = "C" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                    cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord11 where ccode like '" + cCodeHead + "%'", Cmd);
                    record11.cCode = "'" + cc_mcode + "'";
                    record11.ID = iOutRDID;
                    record11.cVouchType = "'11'";
                    record11.cWhCode = "'" + cNewWhcode + "'";
                    record11.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                    record11.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                    record11.cBusType = "'生产倒冲'";
                    record11.cBusCode = record10.cCode;
                    record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                    record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                    record11.cDepCode = "'" + ccDepcode + "'";
                    record11.cRdCode = "'" + cOutRdCode + "'";
                    record11.cMemo = "''";
                    record11.iExchRate = "1";
                    record11.cExch_Name = "'人民币'";

                    if (targetAccId.CompareTo("201") == 0)
                    {
                        record11.VT_ID = 131387;
                    }
                    else
                    {
                        record11.VT_ID = 65;
                    }

                    record11.cSource = "'产成品入库单'";
                    if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

                    //重新赋值
                    cOldWhcode = cNewWhcode;
                    irow = 0;
                }

                //写表体
                System.Data.DataTable dtMer = GetBatDTFromWare("" + cNewWhcode, "" + dtMolt.Rows[k]["InvCode"], float.Parse("" + dtMolt.Rows[k]["iQty"]), bbpos, Cmd, dbname);

                for (int r = 0; r < dtMer.Rows.Count; r++)
                {
                    irow++;

                    KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

                    cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                    records11.AutoID = cAutoid;
                    Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                    Cmd.ExecuteNonQuery();
                    records11.ID = iOutRDID;
                    records11.cInvCode = "'" + dtMolt.Rows[k]["InvCode"] + "'";
                    records11.iQuantity = "" + dtMer.Rows[r]["iquantity"];
                    if (bbpos) records11.cPosition = "'" + dtMer.Rows[r]["cposcode"] + "'";
                    //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                    //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                    records11.irowno = irow;
                    //records11.iNQuantity = records11.iQuantity;
                    records11.cBatch = "'" + dtMer.Rows[r]["cbatch"] + "'";
                    //生产订单信息
                    records11.iMPoIds = dtMolt.Rows[k]["AllocateId"] + "";
                    records11.cmocode = "'" + dtfclist.Rows[0]["mocode"] + "'";
                    records11.imoseq = "" + dtfclist.Rows[0]["SortSeq"];
                    records11.iopseq = "'0000'";
                    records11.invcode = "'" + dtfclist.Rows[0]["invcode"] + "'";

                    records11.cFree1 = (dtMer.Rows[r]["cfree1"] + "" == "" ? "null" : "'" + dtMer.Rows[r]["cfree1"] + "'");
                    records11.cFree2 = (dtMer.Rows[r]["cfree2"] + "" == "" ? "null" : "'" + dtMer.Rows[r]["cfree2"] + "'"); ;
                    //保存数据
                    if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                    //回写已领用量
                    Cmd.CommandText = "update " + dbname + @"..mom_moallocate set IssQty=isnull(IssQty,0)+(0" + records11.iQuantity + ") where AllocateId=0" + records11.iMPoIds;
                    Cmd.ExecuteNonQuery();

                    if (bbpos)
                    {
                        //添加货位记录
                        Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                            cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cfree1,cfree2) " +
                                    "Values (" + cAutoid + "," + iOutRDID + ",'" + cNewWhcode + "','" + dtMer.Rows[r]["cposcode"] + "','" + dtMolt.Rows[k]["InvCode"] + "',0" + dtMer.Rows[r]["iquantity"] +
                                    ",null,'" + dtMer.Rows[r]["cbatch"] + "','" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'11','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] +
                                    "','" + dtMer.Rows[r]["cfree1"] + "','" + dtMer.Rows[r]["cfree2"] + "')";
                        Cmd.ExecuteNonQuery();

                        ////指定货位
                        //Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                        //Cmd.ExecuteNonQuery();

                        //修改货位库存
                        if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + cNewWhcode + "' and cinvcode='" + dtMolt.Rows[k]["InvCode"] +
                            "' and cbatch='" + dtMer.Rows[r]["cbatch"] + "' and cPosCode='" + dtMer.Rows[r]["cposcode"] + "' and cfree1='" + dtMer.Rows[r]["cfree1"] +
                            "' and cfree2='" + dtMer.Rows[r]["cfree2"] + "'", Cmd) == 0)
                        {
                            Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                                cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                                values('" + cNewWhcode + "','" + dtMer.Rows[r]["cposcode"] + "','" + dtMolt.Rows[k]["InvCode"] + "',0,'" + dtMer.Rows[r]["cbatch"] + @"',
                                '','','','','','','','','','','','',0,0)";
                            Cmd.ExecuteNonQuery();
                        }

                        Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtMer.Rows[r]["iquantity"] + ") where cwhcode='" + cNewWhcode +
                            "' and cinvcode='" + dtMolt.Rows[k]["InvCode"] + "' and cbatch='" + dtMer.Rows[r]["cbatch"] + "' and cPosCode='" + dtMer.Rows[r]["cposcode"] +
                            "' and cfree1='" + dtMer.Rows[r]["cfree1"] + "' and cfree2='" + dtMer.Rows[r]["cfree2"] + "'";
                        Cmd.ExecuteNonQuery();

                        //判定负库存问题
                        if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + cNewWhcode + "' and cinvcode='" + dtMolt.Rows[k]["InvCode"] +
                            "' and cbatch='" + dtMer.Rows[r]["cbatch"] + "' and cPosCode='" + dtMer.Rows[r]["cposcode"] +
                            "' and cfree1='" + dtMer.Rows[r]["cfree1"] + "' and cfree2='" + dtMer.Rows[r]["cfree2"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                        {
                            throw new Exception("倒冲出库时【" + dtMolt.Rows[k]["InvCode"] + "】出现负库存");
                        }
                    }

                    //判定 库存账 负库存问题
                    if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode='" + cNewWhcode + "' and cinvcode='" + dtMolt.Rows[k]["InvCode"] +
                        "' and cbatch='" + dtMer.Rows[r]["cbatch"] + "' and cfree1='" + dtMer.Rows[r]["cfree1"] + "' and cfree2='" + dtMer.Rows[r]["cfree2"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                    {
                        throw new Exception("倒冲出库时【" + dtMolt.Rows[k]["InvCode"] + "】出现负库存");
                    }
                }
            }
            #endregion

            tr.Commit();

            return ccRd10Code;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg = CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //U8 保存 盘点单  返回 “盘点单ID,单据号码”
    public string U8SCM_CheckVouch(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

            KK_U8Com.U8CheckVouch check = new KK_U8Com.U8CheckVouch(Cmd, dbname);
            int iPosSet = 0;
            int iVmiSet = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode in('" + dtHead.Rows[0]["仓库编码"] + "') and isnull(bWhPos,0)=1", Cmd));
                iVmiSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode in('" + dtHead.Rows[0]["仓库编码"] + "') and isnull(bProxyWh,0)=1", Cmd));

                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='ch'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='ch'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "P" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(ccvcode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..CheckVouch where ccvcode like '" + cCodeHead + "%'", Cmd);
                check.cCVCode = "'" + cc_mcode + "'";
                check.ID = rd_id;
                if (iPosSet > 0)
                {
                    check.bPosCheck = "1";
                }
                else
                {
                    check.bPosCheck = "0";
                }

                check.cDepCode = ((dtHead.Rows[0]["部门编码"] + "").CompareTo("") == 0 ? "null" : "'" + dtHead.Rows[0]["部门编码"] + "'");
                check.cIRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                check.cORdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                check.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                check.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                check.dCVDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                check.dACDate = "'" + dtHead.Rows[0]["制单日期"] + "'";  //账面日期

                if (!check.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[i]["区域"] + "】不存在"); }
                }
                //判断批次
                if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and isnull(bInvBatch,0)=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "")
                    { throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】批号必须录入"); }
                }

                //判断代管商
                if (iVmiSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..vendor where cvencode='" + dtBody.Rows[i]["供应商"] + "'", Cmd) == 0)
                    { throw new Exception("供应商【" + dtBody.Rows[i]["供应商"] + "】不存在"); }
                }

                if (int.Parse(GetDataString("select count(*) from " + dbname + @"..CheckVouch a inner join " + dbname + @"..CheckVouchs b on a.id=b.id 
                        where b.cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and isnull(b.ccvbatch,'')='" + dtBody.Rows[i]["批号"] + "' and isnull(b.cPosition,'')='" +
                            dtBody.Rows[i]["区域"] + "' and a.cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(b.cvmivencode,'')='" + dtBody.Rows[i]["供应商"] + "' and isnull(a.caccounter,'')=''", Cmd)) > 0)
                {
                    throw new Exception("存货[" + dtBody.Rows[i]["存货编码"] + "]存在未审核的盘点单");
                }

                KK_U8Com.U8CheckVouchs checks = new KK_U8Com.U8CheckVouchs(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='ch'", Cmd));
                checks.autoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='ch'";
                Cmd.ExecuteNonQuery();

                checks.ID = rd_id;
                checks.cCVCode = "'" + cc_mcode + "'";
                checks.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                checks.cPosition = (dtBody.Rows[i]["区域"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["区域"] + "'");
                checks.cCVBatch = (dtBody.Rows[i]["批号"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["批号"] + "'");
                checks.cvmivencode = (dtBody.Rows[i]["供应商"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["供应商"] + "'");
                checks.cFree1 = (dtBody.Rows[i]["自由项1"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["自由项1"] + "'");
                checks.cFree2 = (dtBody.Rows[i]["自由项2"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["自由项2"] + "'");
                checks.iCVQuantity = "" + dtBody.Rows[i]["库存数"]; //帐目数
                checks.iCVCQuantity = "" + dtBody.Rows[i]["数量"];   //盘点数量
                checks.irowno = "" + (i + 1);
                if (!checks.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            }
            tr.Commit();

            return rd_id + "," + cc_mcode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }


    }

    [WebMethod]  //自动指定批号  按照先进进出法
    public System.Data.DataTable GetInvOutBatch(System.Data.DataTable dtBody,string cwhcode, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = null;
        //iDLsID,imain,存货编码,存货名称,规格,发货数,单位,批号
        System.Data.DataTable dt = dtBody.Clone();
        dt.Rows.Clear();
        try
        {
            Conn = OpenDataConnection();
            if (Conn == null)
            {
                throw new Exception("数据库连接失败！");
            }

            System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
            int iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode in('" + cwhcode + "') and isnull(bWhPos,0)=1", Cmd));
            bool bPos = (iPosSet > 0 ? true : false);
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                object[] obj_data=dtBody.Rows[i].ItemArray;
                System.Data.DataTable dtRet = GetBatDTFromWare(cwhcode, "" + dtBody.Rows[i]["存货编码"], float.Parse("" + dtBody.Rows[i]["发货数"]), bPos, Cmd, dbname);
                for (int k = 0; k < dtRet.Rows.Count; k++)
                {
                    System.Data.DataRow DR = dt.Rows.Add(obj_data);
                    if (k == 0)
                    {
                        DR["批号"] = "" + dtRet.Rows[k]["cbatch"];
                        DR["发货数"] = "" + dtRet.Rows[k]["iquantity"];
                    }
                    else
                    {
                        DR["imain"] = "0";
                        DR["批号"] = "" + dtRet.Rows[k]["cbatch"];
                        DR["发货数"] = "" + dtRet.Rows[k]["iquantity"];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }finally 
        {
            CloseDataConnection(Conn);
        }
        return dt;
    }

    [WebMethod]  //发货单审核
    public bool U8SCM_DipsCheck(string dlid, string cwhcode, System.Data.DataTable dtBody, string dbname, string acc_id,string username,string logdate, string SN)
    {
        System.Data.SqlClient.SqlConnection Conn =null ;
        System.Data.SqlClient.SqlTransaction tr = null;
        if (dtBody.Rows.Count == 0) throw new Exception("没有数据");
        try
        {
            Conn = OpenDataConnection();
            if (Conn == null)
            {
                throw new Exception("数据库连接失败！");
            }

            System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
            tr = Conn.BeginTransaction();
            Cmd.Transaction = tr;
            //iDLsID,imain,存货编码,存货名称,规格,发货数,单位,批号
            //修改调整原发货单
            #region
            string iDLsID = "0";
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                string ccc_batch = "";
                int icount = GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and bInvBatch=1", Cmd);
                if (icount > 0)
                {
                    ccc_batch = "'" + dtBody.Rows[i]["批号"] + "'";
                    if (ccc_batch.CompareTo("''") == 0) throw new Exception("存货【" + dtBody.Rows[i]["存货编码"] + "】必须指定批号");
                }
                else
                {
                    ccc_batch = "null";
                }
                if (int.Parse(dtBody.Rows[i]["imain"] + "") == 1)
                {
                    iDLsID = dtBody.Rows[i]["iDLsID"] + "";
                    Cmd.CommandText = "update " + dbname + "..DispatchLists set cbatch=" + ccc_batch + ",iquantity=0" + dtBody.Rows[i]["发货数"] + " where iDLsID=0" + iDLsID;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    //获得最好大号
                    string idl_maxid = "" + GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + acc_id + "' and cVouchType='Dispatch'", Cmd);
                    Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + acc_id + "' and cVouchType='Dispatch'";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "insert into " + dbname + @"..DispatchLists( DLID, iCorID, cWhCode, cInvCode, iQuantity, iNum, iQuotedPrice, iUnitPrice, iTaxUnitPrice, iMoney, 
                            iTax, iSum, iDisCount, iNatUnitPrice, iNatMoney, iNatTax, iNatSum, iNatDisCount, iSettleNum, iSettleQuantity, iBatch, 
                            cBatch, bSettleAll, cMemo, cFree1, cFree2, iTB, dvDate, TBQuantity, TBNum, iSOsID, iDLsID, KL, KL2, cInvName, 
                            iTaxRate, cDefine22, cDefine23, cDefine24, cDefine25, cDefine26, cDefine27, fOutQuantity, fOutNum, cItemCode, 
                            cItem_class, fSaleCost, fSalePrice, cVenAbbName, cItemName, cItem_CName, cFree3, cFree4, cFree5, cFree6, cFree7, 
                            cFree8, cFree9, cFree10, bIsSTQc, iInvExchRate, cUnitID, cCode, iRetQuantity, fEnSettleQuan, fEnSettleSum, 
                            iSettlePrice, cDefine28, cDefine29, cDefine30, cDefine31, cDefine32, cDefine33, cDefine34, cDefine35, cDefine36, 
                            cDefine37, dMDate, bGsp, cGspState, cSoCode, cCorCode, iPPartSeqID, iPPartID, iPPartQty, cContractID, 
                            cContractTagCode, cContractRowGuid, iMassDate, cMassUnit, bQANeedCheck, bQAUrgency, bQAChecking, bQAChecked, 
                            iQAQuantity, iQANum, cCusInvCode, cCusInvName, fsumsignquantity, fsumsignnum, cbaccounter, bcosting, cordercode, 
                            iorderrowno, fcusminprice, icostquantity, icostsum, ispecialtype, cvmivencode, iexchsum, imoneysum, irowno, 
                            frettbquantity, fretsum, iExpiratDateCalcu, dExpirationdate, cExpirationdate, cbatchproperty1, cbatchproperty2, 
                            cbatchproperty3, cbatchproperty4, cbatchproperty5, cbatchproperty6, cbatchproperty7, cbatchproperty8, cbatchproperty9, 
                            cbatchproperty10, dblPreExchMomey, dblPreMomey, idemandtype, cdemandcode, cdemandmemo, cdemandid, 
                            idemandseq, cvencode, cReasonCode, cInvSN, iInvSNCount, bneedsign, bsignover, bneedloss, flossrate, frlossqty, 
                            fulossqty, isettletype, crelacuscode, cLossMaker, dLossDate, dLossTime, icoridlsid, fretoutqty, body_outid, fVeriBillQty, 
                            fVeriBillSum, fVeriRetQty, fVeriRetSum, fLastSettleQty, fLastSettleSum, cBookWhcode, cInVouchType, cPosition, 
                            fretqtywkp, fretqtyykp, frettbqtyykp, fretsumykp, dkeepdate, cSCloser)
                        select  DLID, iCorID, cWhCode, cInvCode, " + dtBody.Rows[i]["发货数"] + @", iNum, iQuotedPrice, iUnitPrice, iTaxUnitPrice, iMoney, 
                            iTax, iSum, iDisCount, iNatUnitPrice, iNatMoney, iNatTax, iNatSum, iNatDisCount, iSettleNum, iSettleQuantity, iBatch, 
                            " + ccc_batch + ", bSettleAll, cMemo, cFree1, cFree2, iTB, dvDate, TBQuantity, TBNum, iSOsID, " + idl_maxid + @", KL, KL2, cInvName, 
                            iTaxRate, cDefine22, cDefine23, cDefine24, cDefine25, cDefine26, cDefine27, fOutQuantity, fOutNum, cItemCode, 
                            cItem_class, fSaleCost, fSalePrice, cVenAbbName, cItemName, cItem_CName, cFree3, cFree4, cFree5, cFree6, cFree7, 
                            cFree8, cFree9, cFree10, bIsSTQc, iInvExchRate, cUnitID, cCode, iRetQuantity, fEnSettleQuan, fEnSettleSum, 
                            iSettlePrice, cDefine28, cDefine29, cDefine30, cDefine31, cDefine32, cDefine33, cDefine34, cDefine35, cDefine36, 
                            cDefine37, dMDate, bGsp, cGspState, cSoCode, cCorCode, iPPartSeqID, iPPartID, iPPartQty, cContractID, 
                            cContractTagCode, cContractRowGuid, iMassDate, cMassUnit, bQANeedCheck, bQAUrgency, bQAChecking, bQAChecked, 
                            iQAQuantity, iQANum, cCusInvCode, cCusInvName, fsumsignquantity, fsumsignnum, cbaccounter, bcosting, cordercode, 
                            iorderrowno, fcusminprice, icostquantity, icostsum, ispecialtype, cvmivencode, iexchsum, imoneysum, irowno, 
                            frettbquantity, fretsum, iExpiratDateCalcu, dExpirationdate, cExpirationdate, cbatchproperty1, cbatchproperty2, 
                            cbatchproperty3, cbatchproperty4, cbatchproperty5, cbatchproperty6, cbatchproperty7, cbatchproperty8, cbatchproperty9, 
                            cbatchproperty10, dblPreExchMomey, dblPreMomey, idemandtype, cdemandcode, cdemandmemo, cdemandid, 
                            idemandseq, cvencode, cReasonCode, cInvSN, iInvSNCount, bneedsign, bsignover, bneedloss, flossrate, frlossqty, 
                            fulossqty, isettletype, crelacuscode, cLossMaker, dLossDate, dLossTime, icoridlsid, fretoutqty, body_outid, fVeriBillQty, 
                            fVeriBillSum, fVeriRetQty, fVeriRetSum, fLastSettleQty, fLastSettleSum, cBookWhcode, cInVouchType, cPosition, 
                            fretqtywkp, fretqtyykp, frettbqtyykp, fretsumykp, dkeepdate, cSCloser
                        from " + dbname + "..DispatchLists where iDLsID=0" + iDLsID;
                    Cmd.ExecuteNonQuery();
                }
            }

            Cmd.CommandText = "update " + dbname + "..DispatchList set cVerifier='" + username + "',dverifydate='" + logdate + "',dverifysystime=getdate() where dlid=0" + dlid;
            Cmd.ExecuteNonQuery();
            #endregion

            string errmsg = "";
            //新增销售出库单
            #region
            KK_U8Com.U8Rdrecord32 record32 = new KK_U8Com.U8Rdrecord32(Cmd, dbname);
            KK_U8Com.U8Rdrecords32 records32 = new KK_U8Com.U8Rdrecords32(Cmd, dbname);
            System.Data.DataTable dtHeads = KK_U8Com.U8Common.GetDataFromDB(@"select a.cdlcode,a.ccuscode,a.cdepcode,a.cpersoncode,a.cSTCode,a.iExchRate,a.cExch_Name,b.cRdCode
                from " + dbname + "..DispatchList a left join " + dbname + "..SaleType b on a.cSTCode=b.cSTCode where dlid=0" + dlid, Cmd);
            if (dtHeads.Rows.Count == 0) throw new Exception("没有找到发货单");

            //赋值
            string vouCode = KK_U8Com.U8Common.GetStringFromSql("select right('000000000'+cast(cast(isnull(right(max(cCode),9),'0000') as int)+1 as varchar(9)),9) from " + dbname + "..rdrecord11 where ccode like 'C%'", Cmd);
            record32.cCode = "'C" + vouCode + "'";
            record32.ID = 1000000000 + int.Parse(KK_U8Com.U8Common.GetStringFromSql("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + acc_id + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + acc_id + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            record32.cVouchType = "'32'";
            if(float.Parse("" + dtBody.Rows[0]["发货数"]) < 0) record32.bredvouch = 1;
            record32.cWhCode = "'" + cwhcode + "'";
            record32.cMaker = "'" + username + "'";
            record32.dDate = "'" + logdate + "'";
            record32.cHandler = "'" + username + "'";
            record32.dVeriDate = "'" + logdate + "'";

            record32.cCusCode = "'" + dtHeads.Rows[0]["ccuscode"] + "'";
            record32.cRdCode = (dtHeads.Rows[0]["cRdCode"] + "" == "" ? "null" : "'" + dtHeads.Rows[0]["cRdCode"] + "'");
            record32.cDepCode = (dtHeads.Rows[0]["cdepcode"] + "" == "" ? "null" : "'" + dtHeads.Rows[0]["cdepcode"] + "'");
            record32.cSTCode = (dtHeads.Rows[0]["cSTCode"] + "" == "" ? "null" : "'" + dtHeads.Rows[0]["cSTCode"] + "'");
            record32.cPersonCode = (dtHeads.Rows[0]["cpersoncode"] + "" == "" ? "null" : "'" + dtHeads.Rows[0]["cpersoncode"] + "'");
            record32.iExchRate = float.Parse(dtHeads.Rows[0]["iExchRate"] + "");
            record32.cExch_Name = "'" + dtHeads.Rows[0]["cExch_Name"] + "'";
            record32.cDLCode = dlid + "";
            record32.cSource = "'发货单'";
            record32.cBusCode = "'" + dtHeads.Rows[0]["cdlcode"] + "'";
            record32.cDefine3 = "'AUTOINPUT'";
            if (!record32.InsertToDB(acc_id, ref errmsg)) { return false; }
            Cmd.CommandText = "Update " + dbname + "..DispatchList SET cSaleOut=" + record32.cCode + " WHERE DLID=0" + dlid;
            Cmd.ExecuteNonQuery();

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {//iDLsID,imain,存货编码,存货名称,规格,发货数,单位,批号
                records32.AutoID = int.Parse(KK_U8Com.U8Common.GetStringFromSql("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + acc_id + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + acc_id + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                records32.ID = record32.ID;
                records32.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records32.iQuantity = "" + dtBody.Rows[i]["发货数"];
                records32.iNquantity = "" + dtBody.Rows[i]["发货数"];

                //records32.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records32.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records32.irowno = (i + 1);
                //records32.cFree1 = (dtBody.Rows[i]["cfree1"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["cfree1"] + "'");
                //records32.cFree2 = (dtBody.Rows[i]["cfree2"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["cfree2"] + "'");
                //records32.cFree3 = (dtBody.Rows[i]["cfree3"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["cfree3"] + "'");
                if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and isnull(bInvBatch,0)=1", Cmd) > 0)
                {
                    if (dtBody.Rows[i]["批号"] + "" == "")
                    { throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】批号必须录入"); }
                }
                string cccbatch = KK_U8Com.U8Common.GetStringFromSql("SELECT case when bInvBatch=1 then '" + dtBody.Rows[i]["批号"] + "' else '' end FROM " + dbname + "..Inventory where cinvcode=" + records32.cInvCode, Cmd);
                records32.cBatch = (cccbatch == "" ? "null" : "'" + cccbatch + "'");
                records32.idlsid = "" + dtBody.Rows[i]["iDLsID"];
                //订单子表ID
                records32.iorderdid = "" + KK_U8Com.U8Common.GetStringFromSql("select isnull(max(iSOsID),0) from " + dbname + "..DispatchLists where iDLsID=" + iDLsID, Cmd);
                records32.cbdlcode = "'" + dtHeads.Rows[0]["cdlcode"] + "'";
                Cmd.CommandText = "Update " + dbname + "..DispatchLists SET fOutQuantity=isnull(fOutQuantity,0)+(0" + records32.iQuantity + ") WHERE iDLsID=0" + records32.idlsid;
                Cmd.ExecuteNonQuery();

                //records32.iordertype = "1";
                //records32.iordercode = "1";

                //质保期
                //records32.iExpiratDateCalcu = (dtBody.Rows[i]["iExpiratDateCalcu"] + "" == "" ? 0 : int.Parse("" + dtBody.Rows[i]["iExpiratDateCalcu"]));
                //records32.iMassDate = (dtBody.Rows[i]["iMassDate"] + "" == "" ? "null" : "" + dtBody.Rows[i]["iMassDate"]);
                //records32.dVDate = (dtBody.Rows[i]["dVDate"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["dVDate"] + "'");
                //records32.cExpirationdate = (dtBody.Rows[i]["cExpirationdate"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["cExpirationdate"] + "'");
                //records32.dMadeDate = (dtBody.Rows[i]["dMadeDate"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["dMadeDate"] + "'");
                //records32.cMassUnit = (dtBody.Rows[i]["cMassUnit"] + "" == "" ? "null" : "'" + dtBody.Rows[i]["cMassUnit"] + "'");

                records32.cDefine24 = "'AUTOINPUT'";
                if (!records32.InsertToDB(ref errmsg)) { return false; }

            }


            #endregion

            tr.Commit();
        }
        catch (Exception ex)
        {
            if (tr != null) tr.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }


        return true;
    }
































    //自动更新
    #region
    [WebMethod(Description = "下载服务器站点文件，传递文件相对路径")]
    public byte[] DownloadFile(string strFilePath)
    {
        FileStream fs = null;
        string CurrentUploadFolderPath = HttpContext.Current.Server.MapPath("loadfiles");

        string CurrentUploadFilePath = CurrentUploadFolderPath + "\\" + strFilePath;
        if (File.Exists(CurrentUploadFilePath))
        {
            try
            {
                ///打开现有文件以进行读取。
                fs = File.OpenRead(CurrentUploadFilePath);
                int b1;
                System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
                while ((b1 = fs.ReadByte()) != -1)
                {
                    tempStream.WriteByte(((byte)b1));
                }
                return tempStream.ToArray();
            }
            catch (Exception ex)
            {
                return new byte[0];
            }
            finally
            {
                fs.Close();
            }
        }
        else
        {
            return new byte[0];
        }
    }

    [WebMethod(Description = "获得更新文件清单")]
    public string[] GetFileList()
    {
        string CurrentUploadFolderPath = HttpContext.Current.Server.MapPath("loadfiles");
        DirectoryInfo TheFolder = new DirectoryInfo(CurrentUploadFolderPath);
        System.Collections.ArrayList arrFils = new System.Collections.ArrayList();
        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            arrFils.Add(NextFile.Name);
        }
        if (arrFils.Count > 0)
        {
            string[] ret = new string[arrFils.Count];
            for (int i = 0; i < arrFils.Count; i++)
            {
                ret[i] = arrFils[i].ToString();
            }
            return ret;
        }
        else
        {
            return null;
        }
    }

    [WebMethod(Description = "获得更新文件清单")]
    public string GetVersion()
    {
        return "" + System.Configuration.ConfigurationSettings.AppSettings["appVersion"];
    }

    #endregion

}

