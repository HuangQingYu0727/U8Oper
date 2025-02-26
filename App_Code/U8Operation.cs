using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class U8Operation : System.Web.Services.WebService
{
    public U8Operation()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    #region  //公共方法
    public static System.Data.SqlClient.SqlConnection OpenDataConnection()
    {
        System.Data.SqlClient.SqlConnection SqlConn = new System.Data.SqlClient.SqlConnection();
        SqlConn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DB_UFSYSTEM"].ConnectionString;
        try
        {
            SqlConn.Open();
        }
        catch (Exception ex)
        {
            throw new Exception("数据库链接失败"+ex.Message);
            return null;
        }
        return SqlConn;
    }

    public static string CloseDataConnection(System.Data.SqlClient.SqlConnection SqlConn)
    {
        try
        {
            if (SqlConn != null && SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();
                SqlConn.Dispose();
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }

    public static string GetDataString(string sql, System.Data.SqlClient.SqlCommand cmmd)
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
    public static string GetDataString(string sql, System.Data.SqlClient.SqlConnection Conn)
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
    public static int GetDataInt(string sql, System.Data.SqlClient.SqlCommand cmmd)
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
    public static int GetDataInt(string sql, System.Data.SqlClient.SqlConnection Conn)
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
    public static System.Data.DataTable GetSqlDataTable(string sql, string tblName, System.Data.SqlClient.SqlCommand cmmd)
    {
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter();
        cmmd.CommandText = sql;
        dpt.SelectCommand = cmmd;
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds, tblName);
        return ds.Tables[tblName];
    }

    private static string GetEnStr()//获得序列号
    {
        string ccomputer = GetComputerInfo();
        if (ccomputer == "") return "";

        string cRetStr = "";
        //USAppReg.ClsGetComputerInfo tmp = new USAppReg.ClsGetComputerInfo();
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
    private static string GetEnStr2()//获得序列号
    {
        string ccomputer = GetComputerInfo2();
        if (ccomputer == "") return "";

        string cRetStr = "";
        //USAppReg.ClsGetComputerInfo tmp = new USAppReg.ClsGetComputerInfo();
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
    private static string GetComputerInfo()
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
            //tmpSn = "";
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
    private static string GetComputerInfo2()
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
            tmpSn = "";
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
    public static string GetDataString(int parm1, int parm2, int parm3, int parm4, int parm5)
    {
        if (parm1 + parm2 + parm3 + parm4 + parm5 != 11111) return "ERROR";
        return GetEnStr();
    }
    public static string GetDataString2(int parm1, int parm2, int parm3, int parm4, int parm5)
    {
        if (parm1 + parm2 + parm3 + parm4 + parm5 != 11111) return "ERROR";
        return GetEnStr2();
    }

    [WebMethod]
    public string TestSn()
    {
        string st_value=System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value == GetEnStr())
            return "true";
        else
            return st_value;
    }
    [WebMethod]
    public string ComputeInfo()
    {
        string ccomputer = GetComputerInfo();
        return ccomputer;
    }

    #endregion

    #region  //U8通用功能（包括用户验证）
    [WebMethod]  //U8用户验证   返回用户名称
    public string U8UserLogin(string usercode, string pwd, ref string ErrMsg)
    {
        if (usercode == "DEMODLL" && pwd == "CQLL")
        {
            string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
            if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000))
            {
                ErrMsg = "非法";
                return "-1";
            }
            else
            {
                return "1000";
            }
        }


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
        if (GetDataInt("select count(*) cn from ufsystem..sysobjects where name='UA_AccountDatabase' and type='U'", Conn) > 0)
        {
            cDataName = "" + GetDataString("select cDatabase from ufsystem..UA_AccountDatabase where cacc_id='" + cacc_id + "' and iBeginYear<=" + cYear + " and isnull(iEndYear,9999)>=" + cYear, Conn);
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
    public string U8PersonName(string cPsnCode,string dbname, ref string ErrMsg)
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
        if (ds.Tables.Count == 0) { ErrMsg="没有查询出数据"; CloseDataConnection(Conn); return ds; }
        return ds;

        
        
    }

    [WebMethod]  //查询数据，返回DataTable
    public System.Data.DataTable GetDataTableFromSQL(string sql, string tblName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败");
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.DataTable dtRet = GetSqlDataTable(sql, tblName, Cmd);
        CloseDataConnection(Conn);
        return dtRet;

        
    }

    [WebMethod]  //查询数据，返回数据集
    public System.Data.DataTable GetDataTableFromPro(string pro_name,System.Data.DataTable dtParameters, string tblName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.CommandType = System.Data.CommandType.StoredProcedure;
        Cmd.CommandText = pro_name;
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(Cmd);
        System.Data.DataSet ds = new System.Data.DataSet();

        //dtParameters  parm_name parm_value
        for (int i = 0; i < dtParameters.Rows.Count; i++)
        {
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("" + dtParameters.Rows[i]["parm_name"], "" + dtParameters.Rows[i]["parm_value"]));
        }
        
        dpt.Fill(ds, tblName);

        CloseDataConnection(Conn);
        return ds.Tables[tblName];
    }

    [WebMethod]  //查询返回字符串数据
    public string GetSqlString(string sql, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        string cRet="";
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
    public string GetSqlStr(string sql)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        string cRet = "";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataTable dt = new System.Data.DataTable();
        dpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            cRet = dt.Rows[0][0] + "";
        }
        CloseDataConnection(Conn);
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

    #endregion
    
    #region   //车间 业务
    [WebMethod]  //U8 根据工序条码 查找工序  工序流转卡表体自定义项4（cdefine25）  条码规则 子表ID
    public System.Data.DataTable U8Mom_Process(string cbarcode, string dbname, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return null;
        }
        try
        {
            float.Parse(cbarcode);
        }
        catch
        {
            ErrMsg = "条码错误，请确认扫描了正确的条码";
            return null;
        }

        System.Data.DataTable dtPF;
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter("select DocCode,Description,OpSeq,CAST(SubFlag AS INT) SubFlag,cast(BalMachiningQty as float) balqty,b.MoDId from " + dbname + 
            "..sfc_processflowdetail a inner join " + dbname + "..sfc_processflow b on a.pfid=b.pfid where PFDId=0" + cbarcode, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtPF = ds.Tables[0];
        
        if (dtPF.Rows.Count==0)
        {
            ErrMsg = "没有找到工序流转信息";
            CloseDataConnection(Conn);
            return null;
        }
        else
        {
            if (dtPF.Rows[0]["SubFlag"] + "" == "1")
            {
                ErrMsg = "委外工序，不能通过PDA报工";
                CloseDataConnection(Conn);
                return null;
            }

            //判断是否存在  外协订单
            if (GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + cbarcode, Conn) > 0)
            {
                ErrMsg = "此工序已经下达外协计划，不能通过PDA报工";
                CloseDataConnection(Conn);
                return null;
            }

            //是否关闭生产订单
            if (GetDataString("select isnull(CloseUser,'') from " + dbname + "..mom_orderdetail where modid=0" + dtPF.Rows[0]["MoDId"], Conn) != "")
                throw new Exception("生产订单已经关闭");

            CloseDataConnection(Conn);
            return dtPF;
        }
        


    }
    [WebMethod]  //U8 根据工序条码 按照权限控制
    public System.Data.DataTable U8Mom_Process_User(string cbarcode,string userid, string dbname, ref string ErrMsg)
    {
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return null;
        }
        try
        {
            float.Parse(cbarcode);
        }
        catch
        {
            ErrMsg = "条码错误，请确认扫描了正确的条码";
            return null;
        }

        //判定是否存在 角色控制
        string cOpCode = "" + GetDataString("select c.cgroup_id from " + dbname + "..sfc_processflowdetail a inner join " + dbname + @"..sfc_operation b on a.operationid=b.operationid
            inner join ufsystem..ua_group c on b.opcode=c.cgroup_id where a.pfdid=0" + cbarcode, Conn);
        if (cOpCode.CompareTo("") != 0)//不等于空时，需要控制权限
        {
            if (GetDataInt("select count(*) from ufsystem..ua_role where cgroup_id='" + cOpCode + "' and cuser_id='" + userid + "'", Conn) <= 0)
            {
                throw new Exception("无本工序报工权限");
            }
        }


        //判断生产订单是否已经关闭
        string i_modid = GetDataString("select b.modid from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_processflow b on a.pfid=b.pfid where PFDId=0" + cbarcode, Conn);
        if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid='" + i_modid + "' and isnull(CloseUser,'')<>''", Conn) > 0)
        {
            throw new Exception("生产订单已经关闭");
        }



        System.Data.DataTable dtPF;
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter("select DocCode,Description,OpSeq,CAST(SubFlag AS INT) SubFlag,cast(BalMachiningQty as float) balqty,b.MoDId from " + dbname +
            "..sfc_processflowdetail a inner join " + dbname + "..sfc_processflow b on a.pfid=b.pfid where PFDId=0" + cbarcode, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtPF = ds.Tables[0];

        if (dtPF.Rows.Count == 0)
        {
            ErrMsg = "没有找到工序流转信息";
            CloseDataConnection(Conn);
            return null;
        }
        else
        {
            if (dtPF.Rows[0]["SubFlag"] + "" == "1")
            {
                ErrMsg = "委外工序，不能通过PDA报工";
                CloseDataConnection(Conn);
                return null;
            }

            //判断是否存在  外协订单
            if (GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + cbarcode, Conn) > 0)
            {
                ErrMsg = "此工序已经下达外协计划，不能通过PDA报工";
                CloseDataConnection(Conn);
                return null;
            }

            //是否关闭生产订单
            if (GetDataString("select isnull(CloseUser,'') from " + dbname + "..mom_orderdetail where modid=0" + dtPF.Rows[0]["MoDId"], Conn) != "")
                throw new Exception("生产订单已经关闭");


            CloseDataConnection(Conn);
            return dtPF;
        }



    }

    [WebMethod]  //U8 HF完成工序流转卡报告
    public string U8Mom_Pro_Save(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid,int iValidFeipin, string dbname,string cacc_id,string username,string SN, ref string ErrMsg)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            ErrMsg = "序列号验证错误！";
            return "";
        }
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //获得上工序合格报工数
        int iUpProCount=0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);

        //上工序ID 若为空则代表本工序为第一道工序     
        if (iupflowcount > 0)
        {   
            //判断上到工序是否为 外协工序
            int iWXcount=GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid , Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本工序的 待加工数
                //iUpProCount = GetDataInt("select cast(isnull(sum(balmachiningqty),0) as int) from " + dbname + "..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);  //本工序加工数量
                iUpProCount = GetDataInt("select cast(isnull(sum(iQualifiedQty),0) as int) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid + " and iMoRoutingDId<>0", Cmd); //上工序合格接收数量
            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }

        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (iTheProCount + iValid + iNotValid > iUpProCount)
        {
            ErrMsg = "本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】";
            return "";
        }
        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (!sfcMain.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }

            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            //sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.Define22 = "'" + PsnCode + "'";
            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid;
            sfcDetail.ScrapQty = "" + iNotValid;
            sfcDetail.RefusedQty = "" + iValidFeipin;   //合格品检验产生的废品数(拒绝数)
            sfcDetail.DeclareQty = "0";
            if (!sfcDetail.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }
            //累计完工数  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) + " where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();
            
            //写流转卡下道工序
            if (ilowcount > 0)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid= "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid + iValidFeipin > 0)  //本工序流转
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iValidFeipin) + ",0," + iNotValid + "," + iValidFeipin + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            tr.Commit();
            return sfcMain.DocCode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            ErrMsg = ex.Message;
            return "";
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 HF完成工序流转卡报告 包含报废原因
    public string U8Mom_Pro_Save_zeren(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, string bfcode, string dbname, string cacc_id, string username, string czerencode, string SN, ref string ErrMsg)
    {
        int iValidFeipin = 0;
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            ErrMsg = "序列号验证错误！";
            return "";
        }
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();

        #region  //数量逻辑关系检查
        //获得上工序合格报工数
        int iUpProCount = 0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);

        //上工序ID 若为空则代表本工序为第一道工序
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本工序的 待加工数
                //iUpProCount = GetDataInt("select cast(isnull(sum(balmachiningqty),0) as int) from " + dbname + "..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);  //本工序加工数量

                iUpProCount = GetDataInt("select cast(isnull(sum(iQualifiedQty),0) as int) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid + " and iMoRoutingDId<>0", Cmd); //上工序合格接收数量
            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }
        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (iTheProCount + iValid + iNotValid > iUpProCount)
        {
            ErrMsg = "本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】";
            return "";
        }
        #endregion

        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            #region  //检查流转卡逻辑关系
            string cc_oplist_control = GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='cc_oplist_control'", Cmd);
            if (cc_oplist_control.CompareTo("control") == 0)
            {
                string cCurDocDefine8 = GetDataString("select isnull(Define8,'') from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
                if (cCurDocDefine8.CompareTo("是") != 0)
                {
                    string cProInvCode = GetDataString("select b.invcode from " + dbname + "..sfc_processflow a inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where a.pfid=0" + pfid, Cmd);//产品编码
                    string cProOpCode = GetDataString("select b.OpCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_operation b on a.OperationId=b.OperationId where a.pfdid=0" + PFDID, Cmd);//工序编码
                    string cProSeq = "" + cSeqcode;//工序行号
                    string cProDocCode = GetDataString("select doccode from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  //流转卡号
                    string cMDeptCode = GetDataString("select b.MDeptCode from " + dbname + "..sfc_processflow a inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where a.pfid=0" + pfid, Cmd);//产品编码

                    string cup_Doc = "" + GetDataString(@"select top 1 a.doccode
                        from " + dbname + "..sfc_processflow a inner join " + dbname + @"..sfc_processflowdetail b on a.pfid=b.pfid
                        inner join " + dbname + @"..sfc_operation c on b.OperationId=c.OperationId
                        inner join " + dbname + @"..mom_orderdetail m on a.modid=m.modid
                        where m.MDeptCode+m.invcode+c.opcode+b.OpSeq='" + cMDeptCode + cProInvCode + cProOpCode + cProSeq + "' and isnull(m.CloseUser,'')='' and a.doccode<'" + cProDocCode + @"' and isnull(a.Define8,'')<>'是' and isnull(b.balmachiningqty,0)>0
                        order by a.doccode", Cmd);
                    if (cup_Doc.CompareTo("") != 0)
                    {
                        throw new Exception("流转卡[" + cup_Doc + "]未报工完毕");
                    }
                }
            }
            #endregion


            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (!sfcMain.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }

            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            //sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.Define22 = "'" + PsnCode + "'";//业务员
            sfcDetail.Define23 = "'" + czerencode + "'";//责任供应商编码


            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid;
            sfcDetail.ScrapQty = "" + iNotValid;
            sfcDetail.RefusedQty = "" + iValidFeipin;   //合格品检验产生的废品数(拒绝数)
            sfcDetail.DeclareQty = "0";
            sfcDetail.ScrapReasonCode = (bfcode + "" == "" ? "null" : "'" + bfcode + "'");
            if (!sfcDetail.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }
            //累计完工数  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) + " where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();

            //ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
            if (ilowcount > 0) //写流转卡下道工序
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid + iValidFeipin > 0)  //本工序流转
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iValidFeipin) + ",0," + iNotValid + "," + iValidFeipin + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            tr.Commit();
            return sfcMain.DocCode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            ErrMsg = ex.Message;
            return "";
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 JX完成工序流转卡报告 包含报废原因 责任单位
    public string U8Mom_Pro_Save_yy(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, string bfcode, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {
        return U8Mom_Pro_Save_zeren(PsnCode, PFDID, cSeqcode, iValid, iNotValid, bfcode, dbname, cacc_id, username, "", SN, ref ErrMsg);
    }

    [WebMethod]  //U8 Meixin完成工序流转卡报告 包含报废原因
    public string U8Mom_Pro_Save_mx(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotGongFei,int iNotLiaoFei,int iNotFanXiu, string bfcode, string dbname, string cacc_id, string username, string SN,string[] dtInfo)
    {
        string ErrMsg = "";
        int i_pw_qty = 0;
        int iNotValid = iNotGongFei + iNotLiaoFei;
        //int iValidFeipin = 0;
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误！");
        }

        if (("" + PsnCode).CompareTo("") == 0)
        {
            throw new Exception("人员不能为空！");
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //获得上工序合格报工数
        int iUpProCount = 0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);
        i_pw_qty = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量

        //上工序ID 若为空则代表本工序为第一道工序
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本工序的 待加工数
                //iUpProCount = GetDataInt("select cast(isnull(sum(balmachiningqty),0) as int) from " + dbname + "..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);  //本工序加工数量

                iUpProCount = GetDataInt("select cast(isnull(sum(iQualifiedQty),0) as int) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid + " and iMoRoutingDId<>0", Cmd); //上工序合格接收数量
            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }
        }
        else
        {
            iUpProCount = i_pw_qty;  // 流转卡数量
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        
        //生产订单MODID
        string mo_modid=GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
        //判断 生茶订单 是否关闭
        if (GetDataInt("select count(*) cn from " + dbname + "..mom_orderdetail b where b.modid=" + mo_modid + " and isnull(b.CloseUser,'')<>''", Cmd) > 0)
        {
            throw new Exception("生产订单已经关闭，不能报工");
        }

        //判断是否有领料记录（关键件）
        int iLL_count = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + mo_modid + " and WIPType=3 and b.bInvKeyPart=1 ", Cmd));
        int iLL_record = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + mo_modid + " and WIPType=3 and b.bInvKeyPart=1 and isnull(IssQty,0)>0", Cmd));
        if (iLL_count > 0 && iLL_record == 0)
        {
            throw new Exception("必须有关键件领用记录 才能报工");
        }
        //获得流转卡第一道工序序号
        string cFirstSeq = "" + GetDataString("select top 1 opseq from " + dbname + "..sfc_processflowdetail where pfid=0" + pfid + " order by opseq", Cmd);

        //参数：是否控制 废品逻辑关系数据
        string cParaNotControlFPQty = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='cParaNotControlFPQty'", Cmd);  
        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID,Cmd);

        //流转卡 总废品数和拒绝数
        int iFlowNotValid_qty = GetDataInt("select cast(isnull(BalScrapQty,0)+isnull(BalRefusedQty,0) as int) from " + dbname + "..sfc_processflowdetail where pfid=0" + pfid + " and opseq<='" + cSeqcode + "'", Cmd);
        
        //本工序已经报工合格数
        int iFlowValid_qty = GetDataInt("select isnull(sum(cast(isnull(QualifiedQty,0) as int)),0) from " + dbname + "..sfc_pfreportdetail where pfdid=0" + PFDID, Cmd);
        //流转卡数量与本工序总报工数  对比  *******************************************  新增逻辑  2019-12-07
        if (i_pw_qty < iFlowValid_qty + iNotGongFei + iNotLiaoFei + iNotFanXiu + iValid)  //投料数-总不合格品数 必须大于等于合格数
        {
            throw new Exception("本工序报工量不能超出流转卡总数（总量控制）");
        }

        if (cSeqcode.CompareTo(cFirstSeq) != 0)  //非首到工序
        {
            //工序上下 数量逻辑关系
            #region
            //非首到工序 （第二道工序开始）
            if (cParaNotControlFPQty.ToLower().CompareTo("control") != 0)
            {
                //不控制废品数
                if (iValid > 0)   //废品数暂时不控制 上下工序的数量逻辑关系  合格数严格控制
                {
                    if (iTheProCount + iValid + iNotValid > iUpProCount)
                    {
                        throw new Exception("本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】");
                    }
                }
            }
            else
            {
                //控制废品数
                if (iTheProCount + iValid + iNotValid > iUpProCount)
                {
                    throw new Exception("本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】");
                }
            }
            #endregion

            //总数量（流转卡数量）与废品数控制
            #region
            //流转卡数量
            int iFlowPF_qty = i_pw_qty; 
            

            if (iFlowPF_qty - iFlowNotValid_qty - iFlowValid_qty < iValid)  //投料数-总不合格品数 必须大于等于合格数
            {
                throw new Exception("本工序总流转数为【" + (iFlowPF_qty - iFlowNotValid_qty) + "】（总量控制）");
            }
            #endregion

            
        }
        else
        {
            //首道工序
            //控制报工超额比例  为空 代表不控制   为数字代表要控制
            //标准工序档案 Define27 代表本工序可以超的比例，空代表不控制
            #region
            string ChaoBaoGongBili = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='ChaoBaoGongBili'", Cmd); //报工可超额度
            if (ChaoBaoGongBili != "")
            {
                try
                {
                    float cbli = float.Parse(ChaoBaoGongBili);
                    if (cbli < 0)
                    {
                        ErrMsg = "参数：超报工比例 必须为大于等于的 正数";
                        throw new Exception(ErrMsg);
                    }
                }
                catch
                {
                    ErrMsg = "参数：超报工比例 设置 必须为数字";
                    throw new Exception(ErrMsg);
                }
            }

            if (ChaoBaoGongBili != "")
            {
                if (iTheProCount + iValid + iNotValid > iUpProCount * (1 + float.Parse(ChaoBaoGongBili)))
                {
                    ErrMsg = "本工序最大可报工数【" + ((iUpProCount * (1 + float.Parse(ChaoBaoGongBili))) - iTheProCount) + "】";
                    throw new Exception(ErrMsg);
                }
            }
            #endregion


            //第一道工序 控制领料比例数
            #region
            int illrow = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + mo_modid + " and WIPType=3 and b.bInvKeyPart=1 ",Cmd));
            //生产订单数量
            string i_c_mom_qty = GetDataString(@"select cast(qty as float) from " + dbname + "..mom_orderdetail where MoDId=0" + mo_modid, Cmd);
            //不合格数取 关键件 齐套量
            int f_ll_qty = int.Parse(GetDataString(@"select cast(isnull(min(round(IssQty*" + i_c_mom_qty + "/qty+0.46,0)),0) as int) from " + dbname + "..mom_moallocate a inner join " + dbname + @"..Inventory_Sub b on a.invcode=b.cInvSubCode 
                where a.MoDId=0" + mo_modid + " and WIPType=3 and b.bInvKeyPart=1 and qty>0", Cmd));
            //第一道工序的生产订单所有报工数
            int iMom_BG_qty = int.Parse(GetDataString(@"select cast(isnull(SUM(b.QualifiedQty),0) as int) from " + dbname + "..sfc_pfreport a inner join " + dbname + @"..sfc_pfreportdetail b on a.PFReportId=b.PFReportId 
                inner join  " + dbname + @"..sfc_processflowdetail c on b.PFDId=c.PFDId
                where a.MoDId=0" + mo_modid + " and c.OpSeq='" + cFirstSeq + "'", Cmd));
            if (f_ll_qty < iMom_BG_qty + iValid && illrow > 0)
            {
                throw new Exception("不能超领料数报工");
            }
            #endregion


        }

        //报工人时间管控（相同的人、相同的流转卡、不同的工序报工的时差不能小于10分钟）
        if (iupflowcount > 0)  //存在上工序  上工序ID：iUppfdid
        {
//            int i_mini_count = 5;  //两工序最小间隔时间
//            string operationid = GetDataString("select OperationId from " + dbname + @"..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);
//            if (GetDataInt("select COUNT(*) from " + dbname + "..sfc_pfreportdetail a inner join " + dbname + @"..sfc_pfreport b on a.PFReportId=b.PFReportId
//                    inner join " + dbname + @"..sfc_processflowdetail c on a.pfdid=c.pfdid 
//                where a.PFDId<>0" + PFDID + " and c.OperationId=0" + operationid + " and b.CreateUser='" + username + "' and DATEDIFF(minute,b.DocTime,GETDATE())<0" + i_mini_count, Cmd) > 0)
//                throw new Exception("两流转卡报工的最短间隔时间为" + i_mini_count + "分钟");
        }

        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            
            if (dtInfo.Length > 6)
                sfcMain.DocDate = "'" + dtInfo[6] + "'";
            else
                sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            //if (dtInfo.Length < 7) throw new Exception("请更新扫码程序");
            //sfcMain.DocDate = "'" + dtInfo[6] + "'";

            if (dtInfo.Length > 7) sfcMain.Define10 = "'" + dtInfo[7] + "'"; //供应商

            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.DocTime = "getdate()";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (!sfcMain.InsertToDB(ref ErrMsg)) { tr.Rollback(); throw new Exception(ErrMsg); }

            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid;
            sfcDetail.ScrapQty = "" + iNotValid;
            sfcDetail.RefusedQty = "" + iNotFanXiu;   //(返修拒绝数)
            sfcDetail.Define34 = "" + iNotGongFei;   //工废数
            sfcDetail.Define35 = "" + iNotLiaoFei;   //料废数
            sfcDetail.DeclareQty = "0";
            sfcDetail.Define23 = "'" + dtInfo[0] + "'";  //部门（生产线）
            sfcDetail.Define24 = "'" + dtInfo[1] + "'";
            
            if (dtInfo.Length > 2)
            {
                sfcDetail.Define25 = "'" + dtInfo[2] + "'";  //责任工序
                sfcDetail.Define28 = "'" + dtInfo[3] + "'";
            }
            sfcDetail.Define22 = "'" + dtInfo[4] + "'";//设备
            if (dtInfo.Length > 5)
            {
                sfcDetail.Define29 = "'" + dtInfo[5] + "'";  //部门属性
            }

            if (dtInfo.Length > 6)
            {
                sfcDetail.Define30 = "'" + dtInfo[6] + "'";  //责任单位
            }

            if (iNotValid > 0)
            {
                sfcDetail.ScrapReasonCode = (bfcode + "" == "" ? "null" : "'" + bfcode + "'");
            }
            if (!sfcDetail.InsertToDB(ref ErrMsg)) { tr.Rollback(); throw new Exception(ErrMsg); }
            //完善高版本字段赋值
            Cmd.CommandText = "update " + dbname + "..sfc_pfreportdetail set PFShiftId=-1 where PFReportDId=0" + sfcDetail.PFReportDId;
            Cmd.ExecuteNonQuery();

            //报工总量控制：工序所有数量不能超流转卡数量
            #region
            int i_rpt_all_qty = GetDataInt("select isnull(sum(cast(isnull(QualifiedQty,0)+isnull(ScrapQty,0)+isnull(RefusedQty,0) as int)),0) from " + dbname + "..sfc_pfreportdetail where pfdid=0" + PFDID, Cmd);
            int i_FlowPF_qty = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (i_rpt_all_qty > i_FlowPF_qty) throw new Exception("本工序所有报工数[" + i_rpt_all_qty + "]不能大于流转卡开卡数量");
            #endregion

            //累计完工数  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iNotFanXiu) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iNotFanXiu + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iNotFanXiu) + " where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();

            //写流转卡下道工序
            if (ilowcount > 0)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iNotFanXiu) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iNotFanXiu + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iNotFanXiu) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid + iNotFanXiu > 0)  //本工序流转
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iNotFanXiu) + ",0," + iNotValid + "," + iNotFanXiu + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            tr.Commit();
            return sfcMain.DocCode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            throw new Exception( ex.Message);
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }


    [WebMethod]  //U8 完成工序流转卡报告（含改制 和缺陷数据）[HL]   iValid 合格数/iNotValid 不合格数/iValidFeipin 差缺数/iNotFan 报废返修数/iNotBF  报废废品数/iNotGZ  报废改制数
    public string U8Mom_Pro_Save_fx_QC_gz(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, int iNotBF, int iNotGZ, string eq_no, string dbname, string cacc_id, string username, string SN, System.Data.DataSet dsExt)
    {
        #region   //接口合法性
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
        #endregion

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            //获得工序编码
            string cur_pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            string cur_opcode = "" + GetDataString("select b.OpCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_operation b on a.OperationId=b.OperationId where pfdid=0" + PFDID, Cmd);
            string cur_depcode = "" + GetDataString("select b.DeptCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_workcenter b on a.wcid=b.wcid where pfdid=0" + PFDID, Cmd);

            #region //增加逻辑交验：A 部门可以自己录入自己的废品  B责任部门缺陷  C栏目工序排除
            string u8_hl_not_valid = GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='u8_hl_not_valid'", Cmd);
            if (u8_hl_not_valid.ToLower() == "true")
            {
                ////A  自己可以录入废品的部门  cDepProp=Y，不能录入其他部门
                //if (iNotBF > 0)
                //{
                //    if (GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + cur_depcode + "' and cDepProp='Y'", Cmd) == 0)
                //        throw new Exception("本部门无权录入 废品数");
                //}

                //B 责任部门缺陷
                if (dsExt != null && iNotBF > 0)
                {
                    System.Data.DataTable dtExtInfo = dsExt.Tables[0];
                    //dtExtInfo.Rows[0]["cZRDept"] + "', '" + dtExtInfo.Rows[0]["cQXCode"]
                    if (dtExtInfo.Rows.Count > 0 )
                    {
                        //A  自己可以录入废品的部门  cDepProp=Y，不能录入其他部门
                        if (GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + cur_depcode + "' and cDepProp='Y'", Cmd) > 0)
                        {
                            if (cur_depcode.CompareTo(dtExtInfo.Rows[0]["cZRDept"] + "") != 0) throw new Exception("本部门的废品只能报 自己为责任部门");
                        }

                        if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_DefectDep_DZ where cdepcode='" + dtExtInfo.Rows[0]["cZRDept"] + "' and cc_defectcode='" + dtExtInfo.Rows[0]["cQXCode"] + "'", Cmd) == 0)
                            throw new Exception("本部门[" + dtExtInfo.Rows[0]["cZRDept"] + "] 无缺陷[" + dtExtInfo.Rows[0]["cQXCode"] + "] ,请做责任部门与缺陷对照");
                    }
                }

                //C 栏目工序排除   暂时不要
                System.Data.DataTable dtLM_Dz = GetSqlDataTable("select clanmu from " + dbname + "..T_CC_HL_HE_LanMuDz where cc_opcode='" + cur_opcode + "'", "dtLM_Dz", Cmd);
                for (int ll = 0; ll < dtLM_Dz.Rows.Count; ll++)
                {
                    if (dtLM_Dz.Rows[ll]["clanmu"] + "" == "1" && iValid>0) throw new Exception("当前工序不允许输入合格数");
                    if (dtLM_Dz.Rows[ll]["clanmu"] + "" == "2" && iNotBF > 0) throw new Exception("当前工序不允许输入报废数");
                    if (dtLM_Dz.Rows[ll]["clanmu"] + "" == "3" && iNotFan > 0) throw new Exception("当前工序不允许输入返修数");
                    if (dtLM_Dz.Rows[ll]["clanmu"] + "" == "4" && iNotGZ > 0) throw new Exception("当前工序不允许输入改制数");
                    if (dtLM_Dz.Rows[ll]["clanmu"] + "" == "5" && iValidFeipin > 0) throw new Exception("当前工序不允许输入差缺数");
                }
            }
            #endregion

            #region  //判断是否存放尾卡，若有，不允许报工
            if (GetDataInt("select COUNT(*) from " + dbname + "..T_HL_Card_WeiKa where cc_pfdid=0" + PFDID + " and ISNULL(cc_maker_out,'')=''", Cmd) > 0)
                throw new Exception("本工序流转卡属于临存状态，不能报工，请先【尾卡取卡】");
            #endregion

            string cRet_code=U8Mom_Pro_Save_HaiLing(PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, iNotFan, iNotBF, iNotGZ, eq_no, dbname, cacc_id, username, SN, dsExt,true ,Cmd);
            if (cRet_code.Split(',').Length > 1) cRet_code = cRet_code.Split(',')[1];
            
            //工序权限控制
            if (GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='u8_fc_op_qx'", Cmd).ToLower().CompareTo("true") == 0)
            {
                string cur_dep_code = "" + GetDataString("select b.DeptCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_workcenter b on a.WcId=b.WcId where pfdid=0" + PFDID, Cmd);
                string cur_psn_depcode = "" + GetDataString("select a.cDept_num from " + dbname + "..hr_hi_person a inner join " + dbname + @"..UserHrPersonContro b on a.cPsn_Num=b.cPsn_Num 
                inner join ufsystem..UA_User c on b.cUser_Id=c.cUser_Id where c.cUser_Name='" + username + "'", Cmd);
                if (cur_dep_code.CompareTo(cur_psn_depcode) != 0 && username.ToLower().CompareTo("demo") != 0)
                    throw new Exception("不具有本部门的报工权限");
            }


            //包装工序[清理 包装、成检外的所有工序的差缺数]
            /*   作废，改由流转卡回收
            #region
            if (cur_opcode.CompareTo("5007") == 0)
            {
                System.Data.DataTable dtFlowChaQue = GetSqlDataTable("select pfdid,opseq,cast(isnull(BalMachiningQty,0) as int) qty from " + dbname + "..sfc_processflowdetail where pfid=" + cur_pfid + " and opseq<'" + cSeqcode +
                    "' and isnull(BalMachiningQty,0)>0 and OperationId not in(select OperationId from " + dbname + "..sfc_operation where OpCode in('4016','4017','5002','5003','5004','5005','5006','5007')) order by opseq", "dtFlowChaQue", Cmd);
                for (int r = 0; r < dtFlowChaQue.Rows.Count; r++)
                {
                    int iFP_Qty = int.Parse(dtFlowChaQue.Rows[r]["qty"] + "");
                    U8Mom_Pro_Save_HaiLing(PsnCode, dtFlowChaQue.Rows[r]["pfdid"] + "", dtFlowChaQue.Rows[r]["opseq"] + "", 0, 0, iFP_Qty, 0, 0, 0, "", dbname, cacc_id, "自报差缺", SN, null,false ,Cmd);
                }
            }
            #endregion
            */
            
            tr.Commit();
            return cRet_code;
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

    private string U8Mom_Pro_Save_HaiLing(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, int iNotBF, int iNotGZ, string eq_no, string dbname, string cacc_id, string username, string SN, System.Data.DataSet dsExt,bool bPDAInput,System.Data.SqlClient.SqlCommand Cmd)
    {
        string ErrMsg = "";
        //*************************************  通用变量定义  ****
        #region   //通用变量定义
        //流转卡 主表 ID
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        //统计上工序 工序数量
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        //本工序的 上工序流转卡子表 ID
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //获得上工序合格报工数
        int iUpProCount = 0;
        //本工序 下工序 的工序数量
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        //本工序下道工序的 流转卡子表ID
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);
        string eq_valid_no = "";// 名义设备编码[符合客户要求]  与 实际加工设备eq_no  有别
        //本道工序的 工序编码
        string cc_opcode = "" + GetDataString("select b.OpCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_operation b on a.OperationId=b.OperationId where pfdid=0" + PFDID, Cmd); 
        //本道工序的 部门编码
        string cc_depcode = "" + GetDataString("select b.DeptCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + "..sfc_workcenter b on a.wcid=b.wcid where pfdid=0" + PFDID, Cmd); ;
        //产品编码
        System.Data.DataTable dt_CardInfo = GetSqlDataTable("select b.invcode,convert(varchar(10),a.CreateDate,120) CreateDate,a.doccode from " + dbname + @"..sfc_processflow a 
            inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where pfid=0" + pfid, "dt_CardInfo", Cmd);
        if (dt_CardInfo.Rows.Count == 0) throw new Exception("未找到流转卡信息！");
        string cc_invcode = dt_CardInfo.Rows[0]["invcode"] + "";


        System.Data.DataTable dtSeqParm = GetSqlDataTable("select cc_hw 货位管理,cc_xjxc 设备管理,cc_kwg 开完工管理,cc_psn 人员管理 from " + dbname +
            "..T_CC_HL_HE_Parameter where cc_depcode='" + cc_depcode + "' and cc_opcode='" + cc_opcode + "'", "dtSeqParm", Cmd);
        if (dtSeqParm.Rows.Count == 0) dtSeqParm = GetSqlDataTable("select '否' 货位管理,'否' 设备管理,'否' 开完工管理,'否' 人员管理", "dtSeqParm", Cmd);
        string imodid=GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
        #endregion
        //*********************************************************

        if (bPDAInput)  //PDA 终端传递过来的数据要求检测，系统自动传递过来的数据不检测
        {
            #region //5月分以前制作的流转卡允许报返修数
            //string card_date = dt_CardInfo.Rows[0]["CreateDate"] + "";
            //if (card_date.CompareTo("2020-05-01") < 0)
            //    throw new Exception("按生产部要求，2020年5月份以前制作的流转卡，不允许报工");
            #endregion

            //参数  dtSeqParm  货位管理  设备管理  开完工管理  人员管理
            #region //检查设备合法性、逻辑性   完工单Define25存放合法设备ID
            eq_no = eq_no + "";
            if (eq_no.CompareTo("") != 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..EQ_EQData where ceqcode='" + eq_no + "'", Cmd) == 0)
                {
                    throw new Exception("设备编号录入错误！");
                }
            }
            //需要设备管控
            if (dtSeqParm.Rows[0]["设备管理"].ToString().CompareTo("是") == 0)
            {
                if (eq_no.CompareTo("") == 0) throw new Exception("请扫描设备编码");
                System.Data.DataTable dtEqList = GetSqlDataTable("select cc_eq_id from " + dbname + "..T_CC_HL_HE_EQ where cc_invcode='" + cc_invcode + "' and cc_depcode='" + cc_depcode + "' and cc_opcode='" + cc_opcode + "'", "dtEqList", Cmd);
                if (dtEqList.Rows.Count == 0) //无名义设备要求
                {
                    if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_EQ where cc_depcode='" + cc_depcode + "' and cc_opcode='" + cc_opcode + "' and cc_eq_id='" + eq_no + "'", Cmd) == 0)
                        throw new Exception("设备不属于合法加工设备，请检查工序合法加工设备");
                    eq_valid_no = eq_no;   //加工设备即为合法设备
                }
                else  //有具体设备要求
                {
                    if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_EQ where cc_invcode='" + cc_invcode + "' and cc_depcode='" + cc_depcode + "' and cc_opcode='" + cc_opcode + "' and cc_eq_id='" + eq_no + "'", Cmd) > 0)
                    {
                        eq_valid_no = eq_no;   //加工设备即为合法设备
                    }
                    else
                    {
                        //寻找本流转卡是否有 加工设备
                        eq_valid_no = "" + GetDataString("select top 1 Define25 from " + dbname + "..sfc_pfreportdetail where pfdid=0" + PFDID, Cmd);

                        if (eq_valid_no.CompareTo("") == 0)  //寻找空闲合法设备
                        {
                            int iKongXian = 1228048251;
                            eq_valid_no = "" + dtEqList.Rows[0]["cc_eq_id"];
                            for (int s = 0; s < dtEqList.Rows.Count; s++)
                            {
                                int ieq_kongxian = GetDataInt(@"select datediff(second,isnull(max(a.DueTime),'2010-01-01'),getdate())
                                    from " + dbname + @"..sfc_pfreportdetail a 
                                    inner join " + dbname + @"..sfc_processflowdetail b on a.pfdid=b.pfdid
                                    inner join " + dbname + @"..sfc_operation c on b.OperationId=c.OperationId
                                    inner join " + dbname + @"..sfc_workcenter d on b.wcid=d.wcid
                                    inner join " + dbname + @"..sfc_processflow e on b.pfid=e.pfid
                                    inner join " + dbname + @"..mom_orderdetail f on e.modid=f.modid
                                    where f.invcode='" + cc_invcode + "' and d.DeptCode='" + cc_depcode + "' and c.opcode='" + cc_opcode + "' and a.Define25='" + dtEqList.Rows[s]["cc_eq_id"] + "'", Cmd);
                                if (iKongXian > ieq_kongxian)
                                {
                                    iKongXian = ieq_kongxian;
                                    eq_valid_no = "" + dtEqList.Rows[s]["cc_eq_id"];
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region  //检查人员合法性
            if (GetDataInt("select count(*) from " + dbname + "..Person where cpersoncode='" + PsnCode + "'", Cmd) == 0)
                throw new Exception("人员编号录入错误！");

            if (dtSeqParm.Rows[0]["人员管理"].ToString().CompareTo("是") == 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_ZiZhi where cc_depcode='" + cc_depcode + "' and cc_opcode='" + cc_opcode + "' and cc_level<>'X'", Cmd) == 0)
                    throw new Exception("人员管理不具有本工序的加工资质");
            }
            #endregion

            #region //检查流转卡是否接收，未接收不能报工
            string liuzhuan_cardno = GetDataString("select doccode from " + dbname + "..sfc_processflow where pfid="+pfid,Cmd);
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_0330_Card_State where cc_cardno='" + liuzhuan_cardno + "' and cc_top_card=0 and isnull(cc_receive_maker,'')=''", Cmd) > 0)
            {
                throw new Exception("请先接收，再报工");
            }
            #endregion

            #region  //检查货位合法性
            if (dtSeqParm.Rows[0]["货位管理"].ToString().CompareTo("是") == 0)
            {
                //接收判定
                if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Pos_TZ where cc_pfdid=0" + PFDID, Cmd) == 0)
                    throw new Exception("还未接收材料，无法进行货位管理");
                //移出判定
                if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Pos_TZ where cc_pfdid=0" + PFDID + " and isnull(cc_maker_out,'')=''", Cmd) > 0)
                    throw new Exception("材料还未移出货位，不能报工处理");
            }

            #endregion

            #region  //检查开工时间性
            if (dtSeqParm.Rows[0]["开完工管理"].ToString().CompareTo("是") == 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID + " and Define37 is not null", Cmd) == 0)
                    throw new Exception("还未开工，无法报工");
            }

            #endregion

            #region  //流转卡状态控制
            string c_flow_state = "" + GetDataString("select isnull(Define8,'正常') from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (c_flow_state.Trim().CompareTo("") == 0) c_flow_state = "正常";
            if (c_flow_state.CompareTo("正常") != 0)
            {
                if (c_flow_state.CompareTo("异常") == 0)
                {
                    throw new Exception("流转卡 [异常] 状态不能报工处理");
                }
                if (c_flow_state.CompareTo("终止") == 0)
                {
                    if (iValid != 0) throw new Exception("流转卡 [终止] 状态不能报合格数");
                }
                if (c_flow_state.CompareTo("回收") == 0)
                {
                    if (iValid != 0) throw new Exception("流转卡 已经回收");
                }
                //string cSeq_exp = "" + GetDataString("select top 1 cc_OpSeq from " + dbname + "..T_CC_HL_HE_FlowCauseList where cc_pfid=0" + pfid + " and cc_new_state='异常' order by cc_time desc", Cmd);
                //if (cSeq_exp == "" || cSeq_exp.CompareTo(cSeqcode) < 0)
                //{
                //    throw new Exception("流转卡异常，不能报工");
                //}
            }

            //生产订单是否关闭
            if (GetDataString("select isnull(CloseUser,'') from " + dbname + "..mom_orderdetail where modid=0" + imodid, Cmd) != "")
                throw new Exception("生产订单已经关闭");
            #endregion

            #region//流转卡 各上报栏目输入权限控制  [区分是否QC]
            //throw new Exception(cc_opcode + " | " + username+" | "+cc_depcode);
            if (!(cc_opcode.CompareTo("4016") == 0 || cc_opcode.CompareTo("4017") == 0 || cc_opcode.CompareTo("5002") == 0 || cc_opcode.CompareTo("5003") == 0 || cc_opcode.CompareTo("5004") == 0 || cc_opcode.CompareTo("5005") == 0 || cc_opcode.CompareTo("5006") == 0 || cc_opcode.CompareTo("5007") == 0))
            {
                //非成检  包装工序才控制
                int iqc_count = GetDataInt(@"select count(*) 
                        from ufsystem..UA_User a inner join ufsystem..UA_Role b on a.cuser_id=b.cuser_id
                        where cgroup_id = 'HLPDAQC' and a.cuser_name='" + username + "'", Cmd);
                int iqc_department = GetDataInt("select count(*) from " + dbname + "..T_CC_HL_QC_Dept where cc_depcode='" + cc_depcode + "'", Cmd);
                if (iqc_department > 0)  //要求有质检
                {
                    if (iqc_count > 0)  //质检员
                    {
                        if ((iValid > 0 || iNotFan > 0 || iNotGZ > 0)) throw new Exception("QC 只能报废品数");
                    }
                    else
                    {
                        if (iNotBF > 0) throw new Exception("废品数只能 由QC上报");
                    }
                }

            }
            #endregion
        }





        //上工序ID 若为空则代表本工序为第一道工序   
        #region  上下工序间 数量级 逻辑检查
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本工序的 待加工数
                //iUpProCount = GetDataInt("select cast(isnull(sum(balmachiningqty),0) as int) from " + dbname + "..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);  //本工序加工数量
                iUpProCount = GetDataInt("select @validqty=isnull(sum(iQualifiedQty),0) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid, Cmd); //上工序合格接收数量
            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }

        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        if (iUpProCount == 0 && (iValid > 0))  //报合格数控制
        {
            throw new Exception("上工序无报工记录，请从上工序开始报工");
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        #endregion


        #region  //控制报工超额比例  为空 代表不控制   为数字代表要控制
        string ChaoBaoGongBili = "" + GetDataString("select Define27 from " + dbname + "..sfc_operation where OpCode='" + cc_opcode + "'", Cmd);
        //string ChaoBaoGongBili = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='ChaoBaoGongBili'", Cmd); //报工可超额度
        if (ChaoBaoGongBili != "")
        {
            try
            {
                float cbli = float.Parse(ChaoBaoGongBili);
                if (cbli < 0) throw new Exception("参数：超报工比例 必须为大于等于的 正数");
            }
            catch
            {
                throw new Exception("参数：超报工比例 设置 必须为数字");
            }
        }

        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (ChaoBaoGongBili != "" && iValid > 0)
        {
            if (iTheProCount + iValid + iNotValid > iUpProCount * (1 + float.Parse(ChaoBaoGongBili)))
            {
                ErrMsg = "本工序最大可报工数【" + ((iUpProCount * (1 + float.Parse(ChaoBaoGongBili))) - iTheProCount) + "】";
                throw new Exception(ErrMsg);
            }
        }
        #endregion

        //开始保存数据 
        KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
        KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);

        #region   //保存完工单记录
        sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
        Cmd.ExecuteNonQuery();
        sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
        sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
        sfcMain.CreateUser = "'" + username + "'";
        sfcMain.UpdCount = "0";
        sfcMain.PFId = "" + pfid;
        //sfcMain.Define1 = "convert(varchar(20),getdate(),120)";
        sfcMain.Define6 = "getdate()";

        sfcMain.Define5 = "'" + cSeqcode + "'";  //工序行号
        sfcMain.Define3 = "'" + GetDataString("select Define3 from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd) + "'";  //生产批号
        sfcMain.Define10 = "'" + eq_no + "'"; //设备号码
        sfcMain.MoDId = imodid;
        
        if (!sfcMain.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }

        sfcDetail.PFReportId = sfcMain.PFReportId;
        sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
        Cmd.ExecuteNonQuery();
        sfcDetail.PFDId = "" + PFDID;
        sfcDetail.EmployCode = "'" + PsnCode + "'";
        sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        sfcDetail.QualifiedQty = "" + iValid;  //合格数
        sfcDetail.ScrapQty = "" + iNotValid;  //不合格数
        sfcDetail.RefusedQty = "" + iValidFeipin;   //差缺数
        sfcDetail.DeclareQty = "0";
        sfcDetail.Define27 = "" + iNotBF;  //报废数   表体自定义项6
        sfcDetail.Define34 = "" + iNotFan;  //返修数   表体自定义项13
        sfcDetail.Define35 = "" + iNotGZ;  //改制  表体自定义项14
        sfcDetail.Define24 = "'" + eq_no + "'";  //实际加工设备
        sfcDetail.Define25 = "'" + eq_valid_no + "'";  //名义加工设备
        //PFShiftId

        if (!sfcDetail.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }
        if (bPDAInput)
        {
            //记录完工单的 开工 完工时间
            Cmd.CommandText = "update " + dbname + @"..sfc_pfreportdetail set StartTime=case when StartTime is null or StartTime=cast(convert(varchar(10),getdate(),120) as datetime) then getdate() else StartTime end,DueTime=getdate(),PFShiftId=0 
                    where PFReportDId=0" + sfcDetail.PFReportDId;
            Cmd.ExecuteNonQuery();
        }
        else
        {
            Cmd.CommandText = "update " + dbname + @"..sfc_pfreportdetail set PFShiftId=0 
                    where PFReportDId=0" + sfcDetail.PFReportDId;
            Cmd.ExecuteNonQuery();
        }
        #endregion


        #region   //累计完工数[回写流转卡]  ,回写下工序 待加工数/合格数/返修数/报废数等
        //获得本工序的所有返工数
        string igxAllfx = "" + GetDataString("select sum(isnull(Define34,0)) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        string igxAllbf = "" + GetDataString("select sum(isnull(Define27,0)) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        string igxAllgz = "" + GetDataString("select sum(isnull(Define35,0)) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (igxAllfx.CompareTo("") == 0) igxAllfx = "0";
        if (igxAllbf.CompareTo("") == 0) igxAllbf = "0";
        if (igxAllgz.CompareTo("") == 0) igxAllgz = "0";
        Cmd.CommandText = "update " + dbname + @"..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +@",
            balscrapqty=isnull(balscrapqty,0)+(" + iNotValid + "),BalRefusedQty=isnull(BalRefusedQty,0)+(" + iValidFeipin + @"),
            balmachiningqty=isnull(balmachiningqty,0)-(" + (iValid + iNotValid + iValidFeipin) +@"),define36=convert(nvarchar(20),getdate(),120),
            define33=convert(nvarchar(20),getdate(),120),DueTime=getdate(),Define34=0" + igxAllfx +",Define27=0" + igxAllbf + ",Define35=0" + igxAllgz + " where PFDId=0" + PFDID;
        Cmd.ExecuteNonQuery();

        //写流转卡下道工序待加工数 与开工时间
        if (ilowcount > 0)
        {
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid +
                ",StartTime=getdate(),Define37=getdate() where PFDId=0" + ilowpfdid;
            Cmd.ExecuteNonQuery();
        }
        else
        {
            //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();
        }

        //本工序开工时间处理
        Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set StartTime=DATEADD(hour,-4,DueTime) where PFDId=0" + PFDID + " and datediff(hour,StartTime,DueTime)<4";
        Cmd.ExecuteNonQuery();

        #endregion


        #region  //工序 货位账出库   在货位移出中控制
        //if (dtSeqParm.Rows[0]["货位管理"].ToString().CompareTo("是") == 0)
        //{
        //    ;
        //}

        #endregion

        //回写工序计划
        string routing_did = "";//工序计划 下到工序 明细ID
        #region
        //累计 加工数
        Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+(" + (iValid + iNotValid + iValidFeipin) +
            "),balscrapqty=isnull(balscrapqty,0)+(" + iNotValid + "),BalRefusedQty=isnull(BalRefusedQty,0)+(" + iValidFeipin + "),balmachiningqty=isnull(balmachiningqty,0)-(" + (iValid + iNotValid + iValidFeipin) + ") where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
        Cmd.ExecuteNonQuery();
        if (ilowcount > 0)
        {
            //回写工序计划 中下到工序的 累计 加工数
            routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
            Cmd.ExecuteNonQuery();
        }
        else
        {
            //回写工序计划 本工序 累计 加工合格数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        //生成工序转移单
        #region
        //工序计划 主表ID
        string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
        string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
        //废品 本工序内转移（加工 ->  废品数） sfc_optransform
        int sfc_optransform_id = 0;
        string sfc_optransform_code = "";
        if (iNotValid + iValidFeipin > 0)  //本工序流转
        {
            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

            Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iValidFeipin) + ",0," + iNotValid + "," + iValidFeipin + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
            Cmd.ExecuteNonQuery();
        }

        sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
        Cmd.ExecuteNonQuery();
        sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
        //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
        if (ilowcount > 0)
        {
            //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
            Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
            Cmd.ExecuteNonQuery();
        }
        else
        {
            //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
            Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
            Cmd.ExecuteNonQuery();
        }
        #endregion

        //缺陷处理  dtExtInfo
        if (dsExt != null)
        {
            #region   //旧缺陷数据处理
            string qx_uname = "" + GetDataString("select cpsn_name from " + dbname + @"..hr_hi_person where cpsn_num='" + PsnCode + "'", Cmd);  //缺陷检验人改为 加工人员
            qx_uname = username;
            System.Data.DataTable dtExtInfo = dsExt.Tables[0];
            //流转卡号  工序代码  存货编码  发现部门代码
            System.Data.DataTable dtOpInfo = GetSqlDataTable(@"select a.DocCode,c.OpCode,m.InvCode,d.DeptCode,d.wccode from " + dbname + @"..sfc_processflow a 
                                inner join " + dbname + @"..sfc_processflowdetail b on a.PFId=b.PFid
                                inner join " + dbname + @"..mom_orderdetail m on a.modid=m.modid
                                inner join " + dbname + @"..sfc_operation c on b.OperationId =c.OperationId
                                inner join " + dbname + @"..sfc_workcenter d on b.WcId=d.WcId
                            where b.PFDId=0" + PFDID, "dtOpInfo", Cmd);
            if (dtOpInfo.Rows.Count == 0)
            {
                throw new Exception("没有找到流转卡具体信息");
            }

            //查找合格数记录
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_HL_DefectData where cc_cardno='" + dtOpInfo.Rows[0]["DocCode"] + "' and cseqno='" + cSeqcode +
                "' and copcode='" + dtOpInfo.Rows[0]["OpCode"] + "' and isnull(isys,0)=1", Cmd) == 0)  //新增合格数和交验数记录
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_DefectData( cc_cardno, cc_invcode, ibqqty, cc_res_dpt, cc_defectcode, 
                            cseqno, copcode, cc_find_dpt, validqty, wasteqty, repairqty, dmakedate, cmaker,isys,irptdid) 
                        values('" + dtOpInfo.Rows[0]["DocCode"] + "','" + dtOpInfo.Rows[0]["InvCode"] + @"',0,'', '', 
                        '" + cSeqcode + "', '" + dtOpInfo.Rows[0]["OpCode"] + "', '" + dtOpInfo.Rows[0]["DeptCode"] + "',0,0,0,convert(varchar(10),getdate(),120),'" + qx_uname + "',1,0)";
                Cmd.ExecuteNonQuery();
            }

            //更新合格数
            if (iValid > 0)
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_HL_DefectData set validqty=validqty+(0" + iValid + @") 
                        where cc_cardno='" + dtOpInfo.Rows[0]["DocCode"] + "' and cseqno='" + cSeqcode + "' and copcode='" + dtOpInfo.Rows[0]["OpCode"] + "' and isnull(isys,0)=1";
                Cmd.ExecuteNonQuery();
            }

            if (dtExtInfo.Rows.Count > 0)
            {
                //增加缺陷记录
                if (iNotBF > 0 || iNotFan > 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_DefectData( cc_cardno, cc_invcode, ibqqty, cc_res_dpt, cc_defectcode, 
                            cseqno, copcode, cc_find_dpt, validqty, wasteqty, repairqty, dmakedate, cmaker,isys,irptdid) 
                        values('" + dtOpInfo.Rows[0]["DocCode"] + "','" + dtOpInfo.Rows[0]["InvCode"] + "',0,'" + dtExtInfo.Rows[0]["cZRDept"] + "', '" + dtExtInfo.Rows[0]["cQXCode"] + @"', 
                        '" + cSeqcode + "', '" + dtOpInfo.Rows[0]["OpCode"] + "', '" + dtOpInfo.Rows[0]["DeptCode"] + "',0,0" + iNotBF + ",0" + iNotFan +
                       ",convert(varchar(10),getdate(),120),'" + qx_uname + "',0,0" + sfcDetail.PFReportDId + ")";
                    Cmd.ExecuteNonQuery();
                }
            }


            //更新交验数
            string cBQQTYAll = GetDataString("select isnull(sum(isnull(validqty,0)+isnull(wasteqty,0)+isnull(repairqty,0)),0) from " + dbname + @"..T_CC_HL_DefectData where cc_cardno='" +
                dtOpInfo.Rows[0]["DocCode"] + "' and cseqno='" + cSeqcode + "' and copcode='" + dtOpInfo.Rows[0]["OpCode"] + "' ", Cmd);
            string c_compelet_qty = GetDataString("select cast(isnull(sum(isnull(QualifiedQty,0)+isnull(ScrapQty,0)+isnull(RefusedQty,0)),0) as int) from " + dbname + @"..sfc_pfreportdetail where pfdid=0" + PFDID, Cmd);

            Cmd.CommandText = "update " + dbname + @"..T_CC_HL_DefectData set ibqqty=(0" + cBQQTYAll + @") 
                        where cc_cardno='" + dtOpInfo.Rows[0]["DocCode"] + "' and cseqno='" + cSeqcode + "' and copcode='" + dtOpInfo.Rows[0]["OpCode"] +
                                           "' and copcode in('4016','4017') and isnull(isys,0)=1";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "update " + dbname + @"..T_CC_HL_DefectData set ibqqty=(0" + c_compelet_qty + @") 
                        where cc_cardno='" + dtOpInfo.Rows[0]["DocCode"] + "' and cseqno='" + cSeqcode + "' and copcode='" + dtOpInfo.Rows[0]["OpCode"] +
                                           "' and copcode not in('4016','4017') and isnull(isys,0)=1";
            Cmd.ExecuteNonQuery();

            #endregion


            #region   //新缺陷数据处理
            float f_new_bqqty = iValid + iNotBF + iNotFan;//  报检数  合格数+废品数+返修数 
            string qx_cc_code = dtExtInfo.Rows[0]["cQXCode"] + "";
            if (iNotBF + iNotFan != 0)
            {
                if (dtExtInfo.Rows[0]["cZRDept"] + "" == "") throw new Exception("必须输入责任部门");
                if (dtExtInfo.Rows[0]["cQXCode"] + "" == "") throw new Exception("必须输入缺陷代码");
                string c_motypecode = GetDataString("select b.MotypeCode from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..mom_motype b on a.MoTypeId=b.MoTypeId where modid=" + imodid, Cmd);
                //if (qx_cc_code != "256" && qx_cc_code != "257" && qx_cc_code != "258" && qx_cc_code != "259" && qx_cc_code != "260")
                if (c_motypecode != "21" && c_motypecode != "31" && GetDataInt("select count(*) from " + dbname + @"..T_CC_HL_QueXianPaichu where cdefectcode='" + qx_cc_code + "'", Cmd) == 0)
                {
                    string num_invcode = cc_invcode.Replace("D", "").Replace("P", "").Replace("Z", "").Replace("G", "");
                    if (GetDataInt(@"select COUNT(*) from " + dbname + @"..sfc_proutingpart a inner join " + dbname + @"..sfc_proutingdetail b on a.PRoutingId=b.PRoutingId 
	                        inner join " + dbname + @"..bas_part c on a.PartId=c.PartId
	                        inner join " + dbname + @"..sfc_workcenter d on b.WcId=d.WcId
                        where c.InvCode like '%" + num_invcode + "' and d.DeptCode='" + dtExtInfo.Rows[0]["cZRDept"] + "'", Cmd) == 0)
                        throw new Exception("责任部门不是本产品的部门");
                }
            }
            if (iNotBF != 0 && iNotFan != 0) throw new Exception("返工数和报废数不同时上报");

            if (iNotBF + iNotFan == 0)  //返修与报废都为0 时，缺陷和责任部门自动清空
            {
                dtExtInfo.Rows[0]["cQXCode"] = "";
                dtExtInfo.Rows[0]["cZRDept"] = "";
            }
            

            if (dtOpInfo.Rows[0]["OpCode"] + "" == "4016" || dtOpInfo.Rows[0]["OpCode"] + "" == "4017")
            {
                if (iNotFan != 0 && qx_cc_code.Substring(0, 1) != "0") throw new Exception("返修缺陷代码必须是001-099");
                if (iNotBF != 0 && (qx_cc_code.Substring(0, 1).CompareTo("1") < 0 || qx_cc_code.Substring(0, 1).CompareTo("6") > 0))
                    throw new Exception("报废缺陷代码必须是100-699");
            }
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_Defect_NewData( cc_cardno, cc_invcode, ibqqty, cc_res_dpt, cc_defectcode, 
                    cseqno, copcode, cc_find_dpt, validqty, wasteqty, repairqty, dmakedate, cmaker,isys,irptdid,cc_find_center) 
                values('" + dtOpInfo.Rows[0]["DocCode"] + "','" + dtOpInfo.Rows[0]["InvCode"] + "',0" + f_new_bqqty + ",'" + dtExtInfo.Rows[0]["cZRDept"] + "', '" + dtExtInfo.Rows[0]["cQXCode"] + @"', 
                '" + cSeqcode + "', '" + dtOpInfo.Rows[0]["OpCode"] + "', '" + dtOpInfo.Rows[0]["DeptCode"] + "',0"+iValid+",0" + iNotBF + ",0" + iNotFan +
               ",convert(varchar(10),getdate(),120),'" + qx_uname + "',0,0" + sfcDetail.PFReportDId + ",'" + dtOpInfo.Rows[0]["wccode"] + "')";
            Cmd.ExecuteNonQuery();


            #endregion
        }

        #region//自动倒冲材料出库（工序倒冲）
        if (GetDataInt("select count(*) from " + dbname + @"..T_CC_HL_HE_DaoChongDz where cc_opcode='" + cc_opcode + "'", Cmd) > 0)
        {
            string ccc_rdcode = "203"; //材料出库类别代码
            string ccc_wccode = GetDataString("select d.wccode from " + dbname + @"..sfc_processflowdetail b inner join " + dbname + @"..sfc_workcenter d on b.WcId=d.WcId
                            where b.PFDId=0" + PFDID,Cmd);
            System.Data.DataTable dtDCCK = GetSqlDataTable("select distinct cdepcode,cwhcode,convert(varchar(10),getdate(),120) cdate from " + dbname + @"..T_CC_HL_HE_DaoChongDz where cc_opcode='" + cc_opcode + "' and wccode='" + ccc_wccode + "'", "dtDCCK", Cmd);
            if (dtDCCK.Rows.Count == 0)
            {
                string cc_opName = GetDataString("select Description from " + dbname + @"..sfc_operation where opcode='" + cc_opcode+"'", Cmd);
                throw new Exception("倒冲：工序[" + cc_opName + "]是材料倒冲工序，没有找到工作中心[" + ccc_wccode + "]的对照信息,请维护对照表");
            }

            U8StandSCMBarCode u8op = new U8StandSCMBarCode();
            string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);
            int iOut_Qty = iValid + iNotValid + iValidFeipin;
            for (int w = 0; w < dtDCCK.Rows.Count; w++)
            {
                System.Data.DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + ccc_rdcode + "' crdcode,'" + dtDCCK.Rows[w]["cwhcode"] + "' cwhcode,'" + dtDCCK.Rows[w]["cdepcode"] + "' cdepcode," + sfcMain.DocCode + " cbuscode,'生产订单' cbustype,'000' headoutposcode", "dtRdMain", Cmd);
                System.Data.DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select b.allocateid,b.invcode cinvcode,'' cbvencode,'' cbatch,round((" + iOut_Qty + ")*b.Qty/a.Qty," + cInv_DecDgt + @") iquantity,b.modid,'' cposcode
                    from " + dbname + "..mom_orderdetail a inner join " + dbname + @"..mom_moallocate b on a.modid=b.modid 
                    where b.modid=0" + imodid + " and b.invcode in(select cinvcode from " + dbname + @"..T_CC_HL_HE_DaoChongDz 
                        where cc_opcode='" + cc_opcode + "' and wccode='" + ccc_wccode + "' and cdepcode='" + dtDCCK.Rows[w]["cdepcode"] + "' and cwhcode='" + dtDCCK.Rows[w]["cwhcode"] + @"'
                        )", "dtRddetail", Cmd);
                if (dtRddetail.Rows.Count == 0) throw new Exception("无法找倒冲出库数据,请查询生产订单的子件材料不一致");
                System.Data.DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
                SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                u8op.U81016(SHeadData, dtRddetail, dbname, username, dtDCCK.Rows[w]["cdate"] + "", "U81016", Cmd);
            }
        }
        #endregion

        return sfcDetail.PFReportDId + "," + sfcMain.DocCode;
    }

    [WebMethod] //
    public bool U8Mom_HaiLin_FLow_Back(System.Data.DataTable seqdata, string doccode, string dbname, string cacc_id, string username, string SN)
    {
        #region   //接口合法性
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
        #endregion

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            System.Data.DataTable dtNewSeq = GetSqlDataTable(@"select a.pfdid,a.OpSeq 行号,cast(a.BalMachiningQty as int) 报差缺数
                from " + dbname + @"..sfc_processflowdetail a inner join " + dbname + @"..sfc_processflow b on a.pfid=b.pfid
                where b.doccode='" + doccode + "' and cast(a.BalMachiningQty as int)<>0 order by a.OpSeq", "dtNewSeq", Cmd);
            if (seqdata.Rows.Count != dtNewSeq.Rows.Count) throw new Exception("流转卡[" + doccode + "]信息发生变化");
            for (int r = 0; r < seqdata.Rows.Count; r++)
            {
                int iFP_Qty = int.Parse(seqdata.Rows[r]["报差缺数"] + "");
                U8Mom_Pro_Save_HaiLing("", seqdata.Rows[r]["pfdid"] + "", seqdata.Rows[r]["行号"] + "", 0, 0, iFP_Qty, 0, 0, 0, "", dbname, cacc_id, username+"", SN, null, false, Cmd);
            }

            Cmd.CommandText = "update " + dbname + @"..sfc_processflow set Define8='回收',define4=getdate() where doccode='" + doccode + "'";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    [WebMethod]  //U8 完成工序流转卡报告(含改制) [HL] 
    public string U8Mom_Pro_Save_fx_eq_gz(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, int iNotBF, int iNotGZ, string eq_no, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {

        return U8Mom_Pro_Save_fx_QC_gz(PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, iNotFan, iNotBF, iNotGZ, eq_no, dbname, cacc_id, username, SN, null);
    }

    [WebMethod]  //U8 完成工序流转卡报告[HL]   iValid 合格数/iNotValid 不合格数/iValidFeipin 差缺数/iNotFan 报废返修数/iNotBF  报废数   /设备编码
    public string U8Mom_Pro_Save_fx_eq(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, int iNotBF, string eq_no, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {
        return U8Mom_Pro_Save_fx_eq_gz(PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, iNotFan, iNotBF, 0, eq_no, dbname, cacc_id, username, SN, ref ErrMsg);
    }

    [WebMethod]  //U8 完成工序流转卡报告[HL]   iValid 合格数/iNotValid 不合格数/iValidFeipin 差缺数/iNotFan 报废返修数/iNotBF  报废数
    public string U8Mom_Pro_Save_fx(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, int iNotBF, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {
        return U8Mom_Pro_Save_fx_eq(PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, iNotFan, iNotBF, "", dbname, cacc_id, username, SN, ref ErrMsg);
    }

    [WebMethod]  //U8 完成工序流转卡报告 含 返修工序[HL]   iValid 合格数/iNotValid 不合格数/iValidFeipin 报废差缺数/iNotFan 拒绝返修数/cfxpfdid 返修工序/iNotBF  报废数
    public string U8Mom_Pro_Save_fanxiu(string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, int iNotFan, string cfxpfdid, int iNotBF, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            ErrMsg = "序列号验证错误！";
            return "";
        }
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            return "";
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //控制报工超额比例  为空 代表不控制   为数字代表要控制
        string ChaoBaoGongBili = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='ChaoBaoGongBili'", Cmd); //报工可超额度
        if (ChaoBaoGongBili != "")
        {
            try
            {
                float cbli = float.Parse(ChaoBaoGongBili);
                if (cbli < 0)
                {
                    ErrMsg = "参数：超报工比例 必须为大于等于的 正数";
                    return "";
                }
            }
            catch
            {
                ErrMsg = "参数：超报工比例 设置 必须为数字";
                return "";
            }
        }

        //获得上工序合格报工数
        int iUpProCount = 0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);

        //上工序ID 若为空则代表本工序为第一道工序     
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本工序的 待加工数
                //iUpProCount = GetDataInt("select cast(isnull(sum(balmachiningqty),0) as int) from " + dbname + "..sfc_processflowdetail where PFDId=0" + PFDID, Cmd);  //本工序加工数量
                iUpProCount = GetDataInt("select @validqty=isnull(sum(iQualifiedQty),0) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid, Cmd); //上工序合格接收数量
            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }

        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (ChaoBaoGongBili != "")
        {
            if (iTheProCount + iValid + iNotValid > iUpProCount)
            {
                ErrMsg = "本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】";
                return "";
            }
        }
        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (!sfcMain.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }
            //iValid 合格数/iNotValid 不合格数/iValidFeipin 报废差缺数/iNotFan 拒绝返修数/ cfxpfid 返修工序/iNotBF  报废数
            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            //sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid; //合格数
            sfcDetail.ScrapQty = "" + iNotValid;  //不合格数=报废差缺数+报废数
            sfcDetail.RefusedQty = "" + iNotFan;   //拒绝返修数
            sfcDetail.DeclareQty = "0";
            sfcDetail.Define27 = "" + iNotBF;  //报废数
            sfcDetail.Define34 = "" + iValidFeipin;  //报废差缺数

            if (!sfcDetail.InsertToDB(ref ErrMsg)) { tr.Rollback(); return ""; }
            //累计完工数[回写流转卡]  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iNotFan) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iNotFan + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iNotFan) +
                ",define36=convert(nvarchar(20),getdate(),120) where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();

            //写流转卡下道工序待加工数
            if (ilowcount > 0)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iNotFan) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iNotFan + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iNotFan) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid + iNotFan > 0)  //本工序流转
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iNotFan) + ",0," + iNotValid + "," + iNotFan + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //返修生成工序转移单
            #region
            if (iNotFan > 0)
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
                string cfanxiu_jh_id = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + cfxpfdid, Cmd);  //返修工序计划

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate,ReworkFlag) values
                    (Null,0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",2,4," + cfanxiu_jh_id + @",
                        " + (iNotFan) + ",0,0,0,0," + (iNotFan) + ",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),1,null,1)";
                Cmd.ExecuteNonQuery();

                //返工流转卡 处理
                #region
                //增加流转卡转入工序的 待加工数BalMachiningQty 和 后工序返工量PostReworkedQty
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalMachiningQty=isnull(BalMachiningQty,0)+(0" + iNotFan +
                    "),PostReworkedQty=isnull(PostReworkedQty,0)+(0" + iNotFan + ") where PFDId=0" + cfxpfdid;
                Cmd.ExecuteNonQuery();

                //减少流转卡 本工序  拒绝数
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalRefusedQty=isnull(BalRefusedQty,0)-(0" + iNotFan + ") where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();

                //减少流转卡 返工工序与本工序之间 所有工序的  完工数CompleteQty  [本工序必须在 返工工序的 后面]
                string cBenSeq = GetDataString("select OpSeq from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);   //本工序行号
                string cFUpSeq = GetDataString("select OpSeq from " + dbname + "..sfc_processflowdetail where pfdid=0" + cfxpfdid, Cmd);   //上工序行号
                System.Data.DataTable dtOpSeq = GetSqlDataTable("select pfdid from " + dbname + "..sfc_processflowdetail where pfid=" + pfid +
                    " and OpSeq>='" + cFUpSeq + "' and OpSeq<='" + cBenSeq + "'", "dtOpSeq", Cmd);
                for (int p = 0; p < dtOpSeq.Rows.Count; p++)
                {
                    Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)-(0" + iNotFan + ") where PFDId=0" + dtOpSeq.Rows[p]["pfdid"];
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                //修改工序计划
                #region
                //计划主表ID
                string cfanxiu_jh_Mid = GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + cfanxiu_jh_id, Cmd);
                System.Data.DataTable dtRouting = GetSqlDataTable("select MoRoutingDId from " + dbname + "..sfc_moroutingdetail where MoRoutingId=" + cfanxiu_jh_Mid + " order by OpSeq", "dtRouting", Cmd);
                for (int p = 0; p < dtRouting.Rows.Count; p++)
                {
                    System.Data.DataTable dtpfcount = GetSqlDataTable(@"select isnull(sum(BalRefusedQty),0) BalRefusedQty,isnull(sum(BalScrapQty),0) BalScrapQty,isnull(sum(BalMachiningQty),0) BalMachiningQty,isnull(sum(CompleteQty),0) CompleteQty,isnull(sum(BalDeclareQty),0) BalDeclareQty 
                        from " + dbname + "..sfc_processflowdetail where MoRoutingDId=0" + dtRouting.Rows[p]["MoRoutingDId"], "dtpfcount", Cmd);

                    Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalRefusedQty=" + dtpfcount.Rows[0]["BalRefusedQty"] + ",BalScrapQty=" + dtpfcount.Rows[0]["BalScrapQty"] +
                        ",BalMachiningQty=" + dtpfcount.Rows[0]["BalMachiningQty"] + ",CompleteQty=" + dtpfcount.Rows[0]["CompleteQty"] + ",BalDeclareQty=" + dtpfcount.Rows[0]["BalDeclareQty"] +
                        " where MoRoutingDId=0" + dtRouting.Rows[p]["MoRoutingDId"];
                    Cmd.ExecuteNonQuery();
                }

                #endregion
            }

            #endregion

            tr.Commit();
            return sfcMain.DocCode;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            ErrMsg = ex.Message;
            return "";
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 装箱[HL]
    public string U8Mom_ProBox_Save(string boxcode,string PsnCode, string PFDID, string cSeqcode, int iValid, int iValidChaque, int iValidFP, string dbname, string cacc_id,
        string username,string cdepcode,string crkbatch, string SN, ref string ErrMsg)
    {
        int iNotValid = iValidFP;  //不合格数
        int iValidFeipin = iValidChaque; //差缺数
        int iNotFan = 0;
        int iNotGZ = 0;
        int iNotBF = iValidFP;   //报废数

        System.Data.DataTable dtData = new System.Data.DataTable("dtData");
        #region   //接口合法性
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
        #endregion

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string cret_code = U8Mom_Pro_Save_HaiLing(PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, iNotFan, iNotBF, iNotGZ, "", dbname, cacc_id, username, SN, null, true, Cmd);
            //处理装箱事物
            #region
            string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            string ccpfDocCode = GetDataString("select doccode from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            //检查是否存在 了对应关系
            if (GetDataInt("select count(*) from " + dbname + ".. T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid, Cmd) > 0) throw new Exception("本箱码 中已经存在 这张流转卡");

            Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode,cc_cdepcode,cc_crkbatch) 
                values('" + boxcode + "'," + pfid + ",0" + iValid + ",convert(varchar(10),getdate(),120),'" + PsnCode + "','" + username + "',0" + cret_code.Split(',')[0] +
                          ",'" + ccpfDocCode + "','" + cdepcode + "','" + crkbatch + "')";
            Cmd.ExecuteNonQuery();
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into " + dbname + "..T_CC_HL_TuoBox(cc_box_code) values('" + boxcode + "')";
                Cmd.ExecuteNonQuery();
            }

            if (dtData.Rows.Count > 0)   //  行0 列0 为 托盘码
            {
                //装托处理  
                //if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "' and isnull(cc_tuo_code,'')<>''", Cmd) > 0) 
                //    throw new Exception("本箱码已经 装托，不能重复装托盘");

                Cmd.CommandText = @"update " + dbname + "..T_CC_HL_TuoBox set cc_tuo_code='" + dtData.Rows[0]["tuocode"] + "',cc_Maker='" + username + @"',cc_dDate=convert(varchar(10),getdate(),120) 
                where cc_box_code='" + boxcode + "'";
                Cmd.ExecuteNonQuery();
            }

            #endregion

            tr.Commit();
            return cret_code.Split(',')[1];
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
        //return U8Mom_ProBox_Save_tuo(boxcode, PsnCode, PFDID, cSeqcode, iValid, iNotValid, iValidFeipin, dbname, cacc_id, username, cdepcode, crkbatch, SN, dt);
    }

    [WebMethod]  //U8 装箱[HL]  
    public string U8Mom_ProBox_Save_tuo(string boxcode, string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, string dbname, string cacc_id,
        string username, string cdepcode, string crkbatch, string SN, System.Data.DataTable dtData)
    {
        string ErrMsg = "";
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
        //判断箱码是否已经存在
        //string cPFCode = "" + GetDataString("select cc_pfCode from " + dbname + "..T_CC_HL_BoxList where cc_boxcode='" + boxcode + "'", Cmd);
        //if (cPFCode != "") throw new Exception("流转卡【" + cPFCode + "】已经使用此箱码");

        //获得上工序合格报工数
        int iUpProCount = 0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        string ccpfDocCode = GetDataString("select doccode from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
        //产品编码
        string c_invcode = GetDataString("select b.invcode from " + dbname + "..sfc_processflow a inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where pfid=0" + pfid, Cmd);

        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);

        #region  //上下工序数量关系逻辑检查
        //上工序ID 若为空则代表本工序为第一道工序
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //是否 直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本上工序
                iUpProCount = GetDataInt("select @validqty=isnull(sum(iQualifiedQty),0) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid, Cmd);   //上工序合格接收数量

            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }

        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        if (iUpProCount == 0)
        {
            throw new Exception("上工序没有 报工记录，请从上工序开始报工。");
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }

        //工序超比例控制
        string ChaoBaoGongBili = "" + GetDataString("select Define27*1000 from " + dbname + @"..sfc_operation 
            where OperationId in(select OperationId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID + ")", Cmd);
        int cbli = 0;
        if (ChaoBaoGongBili != "")
        {
            try
            {
                cbli = int.Parse(ChaoBaoGongBili);
                if (cbli < 0) throw new Exception("参数：超报工比例 必须为大于等于的 正数");
            }
            catch
            {
                throw new Exception("参数：超报工比例 设置 必须为数字");
            }
        }


        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        int imaxProCount =iUpProCount * (1000 + cbli)/1000;
        if (iTheProCount + iValid + iNotValid > imaxProCount)
        {
            throw new Exception("本工序最大可报工数【" + (imaxProCount - iTheProCount) + "】");
        }

        #endregion

        //装托信息判定
        if (dtData.Rows.Count == 0) throw new Exception("请输入装托信息");
        bool b_must_box = true;
        if (boxcode == "")
        {
            b_must_box = true;
            boxcode = "X" + dtData.Rows[0]["tuocode"];  //组合虚拟装箱信息
        }

        #region //尾卡 判定处理（相同产品：1 尾卡必须先装箱  2 可以同时存多个尾卡）
        int iwei_benka = GetDataInt("select COUNT(*) from " + dbname + "..T_HL_Card_WeiKa where cc_cardno='" + ccpfDocCode + "'", Cmd);
        int iwei_cnt = GetDataInt("select COUNT(*) from " + dbname + "..T_HL_Card_WeiKa where cc_cardno='" + ccpfDocCode + "' and ISNULL(cc_maker_out,'')='' ", Cmd);
        if (iwei_cnt > 0) throw new Exception("本卡已经临存，必须先出库");
        iwei_cnt = GetDataInt("select COUNT(*) from " + dbname + "..T_HL_Card_WeiKa where cc_invcode='" + c_invcode + "' and ISNULL(cc_maker_out,'')='' ", Cmd);
        if (iwei_benka == 0 && iwei_cnt > 0) throw new Exception("本产品 临存库 存在尾卡，请先取出尾卡");

        #endregion

        #region //尾托 判定处理（相同产品：1 尾托必须先装满,一个产品只能一个尾托  2 尾托必须装满才能装其他托   3 未装满托入库后才能装其他托）
        iwei_cnt = GetDataInt("select COUNT(*) from " + dbname + "..T_HL_TUO_Weituo where cc_invcode='" + c_invcode + "' and ISNULL(cc_maker_out,'')='' ", Cmd);
        if (iwei_cnt > 0) throw new Exception("本产品 临存库 存在尾托，请先取出尾托");

        iwei_cnt = GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_TuoBox where cc_tuo_code='" + dtData.Rows[0]["tuocode"] + "' and isnull(cc_rd10autoid,0)<>0 ", Cmd);
        if (iwei_cnt > 0) throw new Exception("本托盘已经入库");

        string wei_tuocode = GetDataString(@"select a.cc_tuo_code
            from " + dbname + "..T_CC_HL_TuoBox a inner join " + dbname + @"..T_CC_HL_BoxList b on a.cc_box_code=b.cc_BoxCode 
            inner join " + dbname + @"..sfc_processflow c on b.cc_pfid=c.PFId 
            inner join " + dbname + @"..mom_orderdetail d on c.MoDId=d.MoDId
            inner join " + dbname + @"..inventory i on d.InvCode=i.cInvCode
            where d.InvCode='" + c_invcode + "' and a.cc_tuo_code<>'" + dtData.Rows[0]["tuocode"] + @"' and a.cc_rd10autoid=0 
            group by a.cc_tuo_code,i.cInvDefine12 having isnull(sum(b.cc_iqty),0)<isnull(i.cInvDefine12,0)", Cmd);
        if (wei_tuocode != "") throw new Exception("本产品 托号【" + wei_tuocode + "】未装满，必须先装");
        #endregion


        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);

            sfcMain.Define1 = "'" + boxcode + "'";//装箱码
            if (dtData.Rows.Count > 0) sfcMain.Define2 = "'" + dtData.Rows[0]["tuocode"] + "'";//托码
            sfcMain.Define3 = "'" + GetDataString("select Define3 from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd) + "'";//生产批号
            sfcMain.Define5 = cSeqcode + "";//工序行号
            sfcMain.Define12 = "'" + crkbatch + "'";//入库批号

            if (!sfcMain.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }



            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            sfcDetail.EmployCode = "'" + PsnCode + "'";
            //sfcDetail.Define23 = "'" + boxcode + "'";
            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid;
            sfcDetail.ScrapQty = "" + iNotValid;
            sfcDetail.RefusedQty = "" + iValidFeipin;   //[拒绝数需要修改成 返修数  本处为0]
            //sfcDetail.Define34 = "" + iValidFeipin;  //报废差缺数
            sfcDetail.DeclareQty = "0";
            //sfcDetail.Define22 = "'" + boxcode + "'";//装箱码

            if (!sfcDetail.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }

            Cmd.CommandText = "update " + dbname + @"..sfc_pfreportdetail set PFShiftId=0 where PFReportDId=0" + sfcDetail.PFReportDId;
            Cmd.ExecuteNonQuery();

            //累计完工数  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) +
                ",define36=convert(nvarchar(20),getdate(),120) where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();

            //写流转卡下道工序
            if (ilowcount > 0)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid + iValidFeipin) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + iValidFeipin + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid + iValidFeipin) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid + iValidFeipin > 0)
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid + iValidFeipin) + ",0," + iNotValid + "," + iValidFeipin + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //建立装箱清单
            #region
            //建表
            //CREATE TABLE [dbo].[T_CC_HL_TuoBox](	[cc_tuo_code] [nvarchar](50) NOT NULL,
            //    [cc_box_code] [nvarchar](50) NOT NULL,	[cc_Maker] [nvarchar](50) NULL,	[cc_dDate] [datetime] NULL,
            //  [cc_rd10autoid] [int] NOT NULL CONSTRAINT [DF_T_CC_HL_TuoBox_cc_rd10autoid]  DEFAULT ((0)),
            //  [cc_isChecked] [int] NULL CONSTRAINT [DF_T_CC_HL_TuoBox_cc_isChecked]  DEFAULT ((0)),
            // CONSTRAINT [PK_T_CC_HL_TuoBox] PRIMARY KEY CLUSTERED 
            //(	[cc_box_code] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            //) ON [PRIMARY]
            //GO
            //CREATE TABLE [dbo].[T_CC_HL_BoxList](
            //    [cc_BoxCode] [nvarchar](50) NOT NULL,	[cc_pfid] [int] NOT NULL,	[cc_iqty] [float] NULL,
            //    [cc_BoxDate] [datetime] NULL,	[cc_BoxPsn] [nvarchar](50) NULL,	[cc_BoxMaker] [nvarchar](50) NULL,[cc_reportdid] [int] NULL,
            // CONSTRAINT [PK_T_CC_HL_BoxList] PRIMARY KEY CLUSTERED (	[cc_BoxCode] ASC,	[cc_pfid] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            //) ON [PRIMARY]

            //检查是否存在 了对应关系
            //if (GetDataInt("select count(*) from " + dbname + ".. T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid, Cmd) > 0)
            //{
            //    string oldrkbatch = GetDataString("select cc_crkbatch from " + dbname + ".. T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid, Cmd);
            //    if (crkbatch.CompareTo(oldrkbatch) != 0) throw new Exception("本卡的入库批号已经存在，只能是[" + oldrkbatch + "]");

            //    //throw new Exception("本箱码 中已经存在 这张流转卡");
            //    Cmd.CommandText = "update " + dbname + @"..T_CC_HL_BoxList set cc_iqty=cc_iqty+(0" + iValid + ") where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid;
            //}
            //else
            //{
//            Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode,cc_cdepcode,cc_crkbatch) 
//                values('" + boxcode + "'," + pfid + ",0" + iValid + ",convert(varchar(10),getdate(),120),'" + PsnCode + "','" + username + "',0" + sfcDetail.PFReportDId +
//                          ",'" + ccpfDocCode + "','" + cdepcode + "','" + crkbatch + "')";
            //}

            System.Data.DataTable dtBoxList = GetSqlDataTable("select cc_crkbatch,ISNULL(cc_rd10autoid,0) cc_rd10autoid from " + dbname + @".. T_CC_HL_BoxList 
                where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid + " order by cc_rd10autoid desc", "dtBoxList", Cmd);
            if (dtBoxList .Rows.Count> 0)
            {
                if (int.Parse("" + dtBoxList.Rows[0]["cc_rd10autoid"]) > 0) throw new Exception("本箱子已经入库，不能再装箱");
                string oldrkbatch = "" + dtBoxList.Rows[0]["cc_crkbatch"];
                if (crkbatch.CompareTo(oldrkbatch) != 0) throw new Exception("本卡的已经使用批号[" + oldrkbatch + "]进行入库，请使用此批号");
            }
            //可以添加重复的箱码（箱码+流转卡+报工号  唯一）
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode,cc_cdepcode,cc_crkbatch) 
                values('" + boxcode + "'," + pfid + ",0" + iValid + ",convert(varchar(10),getdate(),120),'" + PsnCode + "','" + username + "',0" + sfcDetail.PFReportDId + ",'" + ccpfDocCode + "','" + cdepcode + "','" + crkbatch + "')";
            Cmd.ExecuteNonQuery();
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into " + dbname + "..T_CC_HL_TuoBox(cc_box_code) values('" + boxcode + "')";
                Cmd.ExecuteNonQuery();
            }

            if (dtData.Rows.Count > 0)   //  行0 列0 为 托盘码
            {
                if (b_must_box) //需要装箱时才控制
                {
                    //装托处理  
                    //if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "' and isnull(cc_tuo_code,'')<>''", Cmd) > 0)
                    //    throw new Exception("本箱码已经 装托，不能重复装托盘");
                }
                string cc_tuo_ccode = dtData.Rows[0]["tuocode"] + "";
                if (cc_tuo_ccode != "")
                {
                    Cmd.CommandText = @"update " + dbname + "..T_CC_HL_TuoBox set cc_tuo_code='" + cc_tuo_ccode + "',cc_Maker='" + username + @"',cc_dDate=convert(varchar(10),getdate(),120) 
                        where cc_box_code='" + boxcode + "'";
                    Cmd.ExecuteNonQuery();
                

                    //判断本箱产品一致性
                    System.Data.DataTable dtTuoList = GetSqlDataTable(@"select c.invcode,SUM(a.cc_iqty) iqty from " + dbname + @"..T_CC_HL_BoxList a 
                        inner join " + dbname + @"..sfc_processflow b on a.cc_pfid=b.PFId
                        inner join " + dbname + @"..mom_orderdetail c on b.MoDId=c.MoDId
                        inner join " + dbname + @"..T_CC_HL_TuoBox d on a.cc_BoxCode=d.cc_box_code
                        where d.cc_tuo_code='" + cc_tuo_ccode + @"'
                        group by c.invcode", "dtTuoList", Cmd);
                    if (dtTuoList.Rows.Count > 1) throw new Exception("托盘不能混装产品");
                }


            }

            #endregion

            tr.Commit();
            return sfcMain.DocCode;
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

    [WebMethod]  //U8 库内装箱装托[HL]  
    public bool U8Mom_ProBox_Tuo_2(string boxcode,string tuocode, string PFDID, int iValid, string dbname, string cacc_id,
        string username, string SN, System.Data.DataTable dtData)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        if (boxcode == "") boxcode = "X" + tuocode;

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            //检查pfdid 是否有入库记录
            if(GetDataInt(@"select count(*)
                from (select * from " + dbname + @"..T_CC_Rd10_FlowCard where t_ctype=1) c inner join " + dbname + @"..rdrecords10 d on c.t_autoid=d.AutoID
                where c.t_card_id=0" + PFDID, Cmd) == 0)
                throw new Exception("流转卡没有合格品入库记录");

            //检查是否存在 了对应关系
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "' and cc_pfid=" + PFDID, Cmd) > 0)
            {
                //throw new Exception("本箱码 中已经存在 这张流转卡");
                Cmd.CommandText = "update " + dbname + @"..T_CC_HL_BoxList set cc_iqty=cc_iqty+(0" + iValid + ") where cc_BoxCode='" + boxcode + "' and cc_pfid=" + PFDID;
            }
            else
            {
                string pf_code=GetDataString("select DocCode from " + dbname + "..sfc_processflow where PFId=0"+PFDID,Cmd);
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode,cc_cdepcode,cc_crkbatch) 
                    values('" + boxcode + "'," + PFDID + ",0" + iValid + ",convert(varchar(10),getdate(),120),'','" + username + @"',0,'" + pf_code + "','','')";
            }
            Cmd.ExecuteNonQuery();


            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into " + dbname + "..T_CC_HL_TuoBox(cc_box_code) values('" + boxcode + "')";
                Cmd.ExecuteNonQuery();
            }

            Cmd.CommandText = @"update " + dbname + "..T_CC_HL_TuoBox set cc_tuo_code='" + tuocode + "',cc_Maker='" + username + @"',cc_dDate=convert(varchar(10),getdate(),120) 
                where cc_box_code='" + boxcode + "'";
            Cmd.ExecuteNonQuery();

            //检查托盘的存货编码是否一致
            System.Data.DataTable dtRdList = GetSqlDataTable(@"select distinct e.cWhCode
                from " + dbname + @"..T_CC_HL_TuoBox a inner join " + dbname + @"..T_CC_HL_BoxList b on a.cc_box_code=b.cc_BoxCode
                inner join (select * from " + dbname + @"..T_CC_Rd10_FlowCard where t_ctype=1) c on b.cc_pfid=c.t_card_id
                inner join " + dbname + @"..rdrecords10 d on c.t_autoid=d.AutoID
                inner join " + dbname + @"..rdrecord10 e on d.ID=e.ID
                where a.cc_tuo_code='" + tuocode + "'", "dtRdList", Cmd);
            if (dtRdList.Rows.Count > 1)
            {
                string cMessage = "本托盘仓库清单";
                for (int i = 0; i < dtRdList.Rows.Count; i++)
                {
                    cMessage += "/" + dtRdList.Rows[i]["cWhCode"];
                }
                throw new Exception("托盘数据来源于多个仓库，" + cMessage);
            }

            //检查仓库是否一致
            dtRdList = GetSqlDataTable(@"select distinct d.cinvcode
                from " + dbname + @"..T_CC_HL_TuoBox a inner join " + dbname + @"..T_CC_HL_BoxList b on a.cc_box_code=b.cc_BoxCode
                inner join (select * from " + dbname + @"..T_CC_Rd10_FlowCard where t_ctype=1) c on b.cc_pfid=c.t_card_id
                inner join " + dbname + @"..rdrecords10 d on c.t_autoid=d.AutoID
                where a.cc_tuo_code='" + tuocode + "'", "dtRdList", Cmd);
            if (dtRdList.Rows.Count > 1)
            {
                string cMessage = "本托盘存货清单";
                for (int i = 0; i < dtRdList.Rows.Count; i++)
                {
                    cMessage += "/" + dtRdList.Rows[i]["cinvcode"];
                }
                throw new Exception("托盘数据来源于多个产品编码，" + cMessage);
            }


            tr.Commit();
            return true;
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

    [WebMethod]  //U8 库内装箱装托[HL]  
    public bool U8Mom_ProBox_Tuo_3(string boxcode, string tuocode,string cwhcode, string cinvcode,string cbatch, int iValid, string dbname, string cacc_id,
        string username, string SN, System.Data.DataTable dtData)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        if (boxcode == "") boxcode = "X" + tuocode;
        string errmsg = "";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            //检查产品是否存在批次管理
            if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + cinvcode + "' and bInvBatch=1", Cmd) > 0)
            {
                if (cbatch == "") throw new Exception("本产品需要批号");
            }

            int rd_id = GetDataInt(@"select isnull(max(ID),0) from " + dbname + "..TransVouch where cTVCode='" + tuocode + "'", Cmd);
            if (rd_id > 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..TransVouch where id=0" + rd_id + " and isnull(cVerifyPerson,'')<>''", Cmd) > 0)
                    throw new Exception("托盘[" + tuocode + "]已经上架（审核）");
            }

            //检查是否存在 了对应关系
            #region  //组盘
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "'", Cmd) > 0)
            {
                //throw new Exception("本箱码 中已经存在 这张流转卡");
                Cmd.CommandText = "update " + dbname + @"..T_CC_HL_BoxList set cc_iqty=cc_iqty+(0" + iValid + ") where cc_BoxCode='" + boxcode + "'";
            }
            else
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode,cc_cdepcode,cc_crkbatch,cc_invcode) 
                    values('" + boxcode + "',0,0" + iValid + ",convert(varchar(10),getdate(),120),'','" + username + @"',0,'','','" + cbatch + "','" + cinvcode + "')";
            }
            Cmd.ExecuteNonQuery();


            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into " + dbname + "..T_CC_HL_TuoBox(cc_box_code) values('" + boxcode + "')";
                Cmd.ExecuteNonQuery();
            }

            Cmd.CommandText = @"update " + dbname + "..T_CC_HL_TuoBox set cc_tuo_code='" + tuocode + "',cc_Maker='" + username + @"',cc_dDate=convert(varchar(10),getdate(),120) 
                where cc_box_code='" + boxcode + "'";
            Cmd.ExecuteNonQuery();

            //检查托盘的存货编码是否一致
            System.Data.DataTable dtRdList = GetSqlDataTable(@"select distinct b.cc_invcode
                from " + dbname + @"..T_CC_HL_TuoBox a inner join " + dbname + @"..T_CC_HL_BoxList b on a.cc_box_code=b.cc_BoxCode
                where a.cc_tuo_code='" + tuocode + "'", "dtRdList", Cmd);
            if (dtRdList.Rows.Count > 1)
            {
                string cMessage = "本托盘产品清单";
                for (int i = 0; i < dtRdList.Rows.Count; i++)
                {
                    cMessage += "/" + dtRdList.Rows[i]["cc_invcode"];
                }
                throw new Exception("托盘有多个产品，" + cMessage);
            }
            #endregion

            //创建调拨单，不审核，无货位
            #region 
            string in_whcode = "A014"; string cInv_Code = cinvcode;
            string dCurDate = GetDataString(@"select convert(varchar(10),getdate(),120)", Cmd);
            

            #region //主表
            if (rd_id ==0)
            {
                KK_U8Com.U8TransVouch dbmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='tr'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();
                dbmain.cTVCode = "'" + tuocode + "'";
                dbmain.ID = rd_id;
                dbmain.dTVDate = "'" + dCurDate + "'";
                dbmain.cMaker = "'" + username + "'";
                //dbmain.dVerifyDate = "'" + dCurDate + "'";
                //dbmain.cVerifyPerson = "'" + username + "'";
                dbmain.IsWfControlled = 0;
                dbmain.cOWhCode = "'" + cwhcode + "'";
                dbmain.cIWhCode = "'" + in_whcode + "'";
                if (!dbmain.InsertToDB(cacc_id, false, ref errmsg)) { throw new Exception(errmsg); }
            }
            #endregion

            #region //子表
            KK_U8Com.U8TransVouchs dbdetail = new KK_U8Com.U8TransVouchs(Cmd, dbname);
            dbdetail.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='tr'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='tr'";
            Cmd.ExecuteNonQuery();

            int i_row = GetDataInt("select count(*)+1 from " + dbname + "..TransVouch where cTVCode='" + tuocode + "'", Cmd);
            dbdetail.ID = rd_id;
            dbdetail.cTVCode = "'"+tuocode+"'";
            dbdetail.cInvCode = "'" + cInv_Code + "'";
            dbdetail.iTVQuantity = "" + iValid;
            dbdetail.iTVNum = "0";
            dbdetail.irowno = i_row;
            dbdetail.cTVBatch = (cbatch + "" == "" ? "null" : "'" + cbatch + "'");
            dbdetail.bCosting = 0;
            if (!dbdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            #endregion

            #endregion

            //修改调拨在途量
            #region
//            U8StandSCMBarCode.U8V10SetCurrentStockRow(Cmd, dbname + "..", cwhcode, cinvcode, "null", "null",
//                    "null", "null", "null", "null", "null",
//                    "null", "null", "null", cbatch, "null");
//            U8StandSCMBarCode.U8V10SetCurrentStockRow(Cmd, dbname + "..", in_whcode, cinvcode, "null", "null",
//                    "null", "null", "null", "null", "null",
//                    "null", "null", "null", cbatch, "null");
//            //更新数量 fTransOutNum
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)+(0" + iValid + @"),
//                                    fTransOutNum=isnull(fTransOutNum,0)+(0)
//                                where cwhcode='" + cwhcode+ "' and cinvcode='" + cinvcode + @"' and cfree1='' 
//                                    and cfree2='' and cfree3='' and cfree4='' 
//                                    and cfree5='' and cfree6='' and cfree7='' 
//                                    and cfree8='' and cfree9='' and cfree10='' 
//                                    and cbatch='" + cbatch + "' and cVMIVenCode='' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();

//            //更新数量
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)+(0" + iValid + @") ,
//                                    fTransInNum=isnull(fTransInNum,0)+(0) 
//                                where cwhcode='" + in_whcode + "' and cinvcode='" + cinvcode + @"' and cfree1='' 
//                                    and cfree2='' and cfree3='' and cfree4='' 
//                                    and cfree5='' and cfree6='' and cfree7='' 
//                                    and cfree8='' and cfree9='' and cfree10='' 
//                                    and cbatch='" + cbatch + "' and cVMIVenCode='' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();

            #endregion
            tr.Commit();
            return true;
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


    [WebMethod]  //U8 上架（货位-生成调拨单）调拨[HL]  
    public string U8Mom_ProBox_TranTo_Ware(string tuocode, string cwhcode,string cposcode, decimal iValid, string dbname, string cacc_id,
        string username, string SN,string clogDate, System.Data.DataTable dtData)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //CREATE TABLE [dbo].[T_CC_HL_Tuo_TRanDZ](	[cc_tuo_code] [nvarchar](50) NOT NULL,
        //[cc_tran_id] int NOT NULL,	[cc_Maker] [nvarchar](50) NULL,	[cc_dDate] [datetime] NULL)
        //GO

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            #region  //检查
            //货位
            if (GetDataInt("select count(*) from " + dbname + "..position where cposcode='" + cposcode + "'", Cmd) == 0) throw new Exception("货位不存在");
            //托盘是否存在
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_tuo_code='" + tuocode + "'", Cmd) == 0) throw new Exception("托盘码不存在");
            //托盘是否已经调拨
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_Tuo_TRanDZ where cc_tuo_code='" + tuocode + "'", Cmd) > 0) throw new Exception("托盘已经上架");
            #endregion
            string errmsg = "";
            #region //调拨
            string outwhcode = ""; string cInv_Code = "";
            string dCurDate = GetDataString(@"select convert(varchar(10),getdate(),120)", Cmd);

            #region   //获得托盘的数据清单
            System.Data.DataTable dtWareList = GetSqlDataTable(@"select distinct e.cWhCode,d.cinvcode
                from " + dbname + @"..T_CC_HL_TuoBox a inner join " + dbname + @"..T_CC_HL_BoxList b on a.cc_box_code=b.cc_BoxCode
                inner join (select * from " + dbname + @"..T_CC_Rd10_FlowCard where t_ctype=1) c on b.cc_pfid=c.t_card_id
                inner join " + dbname + @"..rdrecords10 d on c.t_autoid=d.AutoID
                inner join " + dbname + @"..rdrecord10 e on d.ID=e.ID
                where a.cc_tuo_code='" + tuocode + "'", "dtWareList", Cmd);
            if (dtWareList.Rows.Count  ==0) //未找到托盘对应的流转卡的入库信息
            {
                throw new Exception("未找到托盘对应的流转卡的入库信息");
            }
            outwhcode = dtWareList.Rows[0]["cWhCode"] + "";
            cInv_Code = dtWareList.Rows[0]["cinvcode"] + "";
            #endregion

            KK_U8Com.U8TransVouch dbmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
            KK_U8Com.U8Rdrecord08 rd08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
            KK_U8Com.U8Rdrecord09 rd09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);

            #region//新增主表
            int rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='tr'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='tr'";
            Cmd.ExecuteNonQuery();
            string cCodeHead = "T" + GetDataString("select right(replace(convert(varchar(10),'" + dCurDate + "',120),'-',''),6)", Cmd);
            string cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(ctvcode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..TransVouch where ctvcode like '" + cCodeHead + "%'", Cmd);
            dbmain.cTVCode = "'" + cc_mcode + "'";
            dbmain.ID = rd_id;
            dbmain.dTVDate = "'" + dCurDate + "'";
            dbmain.cMaker = "'" + username + "'";
            dbmain.dVerifyDate = "'" + dCurDate + "'";
            dbmain.cVerifyPerson = "'" + username + "'";
            dbmain.IsWfControlled = 0;
            dbmain.cOWhCode = "'" + outwhcode + "'";
            dbmain.cIWhCode = "'" + cwhcode + "'";
            //dbmain.cORdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
            //dbmain.cIRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            //dbmain.cODepCode = (dtHead.Rows[0]["调出部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调出部门"] + "'");
            //dbmain.cIDepCode = (dtHead.Rows[0]["调入部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调入部门"] + "'");
            //dbmain.cPersonCode = (dtHead.Rows[0]["业务员"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["业务员"] + "'");
            if (!dbmain.InsertToDB(cacc_id, false, ref errmsg)) { throw new Exception(errmsg); }

            #region //审核调拨单 形成其他入库和其他出库单
            //新增其他出库单主表
            rd09.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rd09.bredvouch = "0";
            rd09.cWhCode = "'" + outwhcode + "'";
            rd09.cMaker = "'" + username + "'";
            rd09.dDate = "'" + dCurDate + "'";
            rd09.cHandler = "'" + username + "'";
            rd09.dVeriDate = "'" + dCurDate + "'";
            rd09.cCode = "'TO" + cc_mcode.Substring(1) + "'";
            //rd09.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
            //rd09.cDepCode = (dtHead.Rows[0]["调出部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调出部门"] + "'");
            rd09.iExchRate = "1";
            rd09.cExch_Name = "'人民币'";
            rd09.cBusCode = "'" + cc_mcode + "'";
            rd09.cBusType = "'调拨出库'";
            rd09.cSource = "'调拨'";
            rd09.cDefine3 = "'AUTOINPUT'";
            rd09.VT_ID = 85;// 131395;
            if (!rd09.InsertToDB(cacc_id, ref errmsg)) { throw new Exception(errmsg); }


            //新增其他入库单主表
            rd08.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rd08.bredvouch = "0";
            rd08.cWhCode = "'" + cwhcode + "'";
            rd08.cMaker = "'" + username + "'";
            rd08.dDate = "'" + dCurDate + "'";
            rd08.cHandler = "'" + username + "'";
            rd08.dVeriDate = "'" + dCurDate + "'";
            rd08.cCode = "'TI" + cc_mcode.Substring(1) + "'";
            //rd08.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            rd08.iExchRate = "1";
            rd08.cExch_Name = "'人民币'";
            rd08.cBusCode = "'" + cc_mcode + "'";
            rd08.cBusType = "'调拨入库'";
            rd08.cSource = "'调拨'";
            //rd08.cDepCode = (dtHead.Rows[0]["调入部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调入部门"] + "'");
            rd08.cDefine3 = "'AUTOINPUT'";
            if (!rd08.InsertToDB(cacc_id, ref errmsg)) { throw new Exception(errmsg); }
            #endregion

            #endregion

            #region  //子表
            //获得在库批次清单
            System.Data.DataTable dtStockBatch = GetSqlDataTable(@"select cBatch,cFree1,cFree2,sum(iQuantity) iQuantity from currentstock 
                where cWhCode='" + outwhcode + "' and cInvCode='" + cInv_Code + @"' and iQuantity>0
                group by cBatch,cFree1,cFree2", "dtStockBatch",Cmd);
            for (int i = 0; i < dtStockBatch.Rows.Count; i++)
            {
                //数量控制
                decimal d_st_rowqty = decimal.Parse("" + dtStockBatch.Rows[i]["iQuantity"]);
                decimal d_rd_qty = 0;//本次调拨量
                if (d_st_rowqty >= iValid)
                {
                    d_rd_qty = iValid;
                    iValid = 0;
                }
                else
                {
                    d_rd_qty = d_st_rowqty;
                    iValid = iValid - d_st_rowqty;
                }

                //写调拨单子表
                #region
                KK_U8Com.U8TransVouchs dbdetail = new KK_U8Com.U8TransVouchs(Cmd, dbname);
                dbdetail.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='tr'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();

                dbdetail.ID = dbmain.ID;
                dbdetail.cTVCode = dbmain.cTVCode;
                dbdetail.cInvCode = "'" + cInv_Code + "'";
                dbdetail.iTVQuantity = "" + d_rd_qty;
                dbdetail.irowno = (i + 1);
                string cccbatch = "" + dtStockBatch.Rows[i]["cBatch"];
                string cccfree1 = "" + dtStockBatch.Rows[i]["cFree1"];
                string cccfree2 = "" + dtStockBatch.Rows[i]["cFree2"];
                dbdetail.cTVBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                dbdetail.cFree1 = (cccfree1 + "" == "" ? "null" : "'" + cccfree1 + "'");
                dbdetail.cFree2 = (cccfree2 + "" == "" ? "null" : "'" + cccfree2 + "'");

                dbdetail.cinposcode = "'" + cposcode + "'";
                dbdetail.bCosting = 0;
                if (!dbdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
                #endregion

                //写入其他出库单
                #region
                KK_U8Com.U8Rdrecords09 rds09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);
                rds09.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds09.ID = rd09.ID;
                rds09.cInvCode = "'" + cInv_Code + "'";
                rds09.iQuantity = "" + d_rd_qty;
                rds09.iNQuantity = rds09.iQuantity;
                rds09.irowno = (i + 1);
                rds09.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds09.cFree1 = (cccfree1 + "" == "" ? "null" : "'" + cccfree1 + "'");
                rds09.cFree2 = (cccfree2 + "" == "" ? "null" : "'" + cccfree2 + "'");
                rds09.iTrIds = "" + dbdetail.AutoID;
                rds09.cDefine24 = "'AUTOINPUT'";

                rds09.bCosting = "1";
                int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd09.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) rds09.bCosting = "0";
                rds09.isotype = "0";

                if (!rds09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


                //判定 库存账 负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode=" + rd09.cWhCode + " and cinvcode=" + rds09.cInvCode +
                    " and cbatch='" + cccbatch + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("仓库[" + outwhcode + "] 存货[" + cInv_Code + "]出现负库存");
                }

                #endregion

                //写入其他入库单
                #region
                KK_U8Com.U8Rdrecords08 rds08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);
                rds08.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds08.ID = rd08.ID;
                rds08.cInvCode = "'" + cInv_Code + "'";
                rds08.iQuantity = "" + d_rd_qty;
                rds08.irowno = (i + 1);
                rds08.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds08.cFree1 = (cccfree1 + "" == "" ? "null" : "'" + cccfree1 + "'");
                rds08.cFree2 = (cccfree2 + "" == "" ? "null" : "'" + cccfree2 + "'");

                rds08.iTrIds = "" + dbdetail.AutoID;
                rds08.cDefine24 = "'AUTOINPUT'";

                rds08.bCosting = "1";
                ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd08.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) rds08.bCosting = "0";

                rds08.cPosition = "'" + cposcode + "'";
                if (!rds08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                
                //添加货位记录
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,iExpiratdatecalcu,cMassUnit) " +
                    "Values (" + rds08.AutoID + "," + rds08.ID + "," + rd08.cWhCode + ",'" + cposcode+ "','" + cInv_Code + "',0" + rds08.iQuantity +
                    ",null,'" + cccbatch + "'," + rd08.dDate + ",1,'',0,'08'," + rd08.dDate + "," + rd08.cMaker + ",0,0)";
                Cmd.ExecuteNonQuery();

                //指定货位
                Cmd.CommandText = "update " + dbname + "..rdrecords08 set iposflag=1 where autoid =0" + rds08.AutoID;
                Cmd.ExecuteNonQuery();

                //修改货位库存
                if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + rd08.cWhCode + " and cinvcode='" + cInv_Code +
                    "' and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values(" + rd08.cWhCode + ",'" + cposcode + "','" + cInv_Code + "',0,'" + cccbatch + @"',
                        '','','','','','','','','','','','',0,0)";
                    Cmd.ExecuteNonQuery();
                }

                Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + rds08.iQuantity + ") where cwhcode=" + rd08.cWhCode +
                    " and cinvcode='" + cInv_Code + "' and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "'";
                Cmd.ExecuteNonQuery();

                ////判定负库存问题
                //if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode=" + rd08.cWhCode + " and cinvcode='" + cInv_Code +
                //    "' and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "' and isnull(iquantity,0)<0", Cmd) > 0)
                //{
                //    throw new Exception("【" + cInv_Code + "】出现负库存");
                //}
                
                #endregion

                if (iValid <= 0) break;
            }

            //判断库存是否够用
            if (iValid > 0)
            {
                throw new Exception("调出仓库[" + outwhcode + "]库存数量不够,差[" + iValid + "]");
            }
            #endregion

            //记录托盘调拨信息
            Cmd.CommandText = @"insert into T_CC_HL_Tuo_TRanDZ(cc_tuo_code,cc_tran_id,cc_Maker,cc_dDate) 
                values('" + tuocode + "',0" + rd_id + ",'" + username + "','" + dCurDate + "')";
            Cmd.ExecuteNonQuery();

            #endregion
            tr.Commit();
            return "";
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

    [WebMethod]  //U8 上架（货位-只处理货位账务）调拨[HL]  
    public string U8Mom_ProBox_TranTo_Ware_2(string tuocode, string cposcode, decimal iValid, string dbname, string cacc_id,
        string username, string SN, string clogDate, System.Data.DataTable dtData)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //CREATE TABLE [dbo].[T_CC_HL_Tuo_TRanDZ](	[cc_tuo_code] [nvarchar](50) NOT NULL,
        //[cc_tran_id] int NOT NULL,	[cc_Maker] [nvarchar](50) NULL,	[cc_dDate] [datetime] NULL)
        //GO

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            #region  //检查
            //托盘是否存在
            if (GetDataInt("select count(*) from " + dbname + "..TransVouch where cTVCode='" + tuocode + "'", Cmd) == 0) throw new Exception("托盘码为空，无装托记录");
            if (GetDataInt("select count(*) from " + dbname + "..TransVouch where cTVCode='" + tuocode + "' and isnull(cVerifyPerson,'')<>''", Cmd) > 0) throw new Exception("托盘已经上架");
            //获得调拨单数据
            System.Data.DataTable dtTranBody = GetSqlDataTable(@"select b.AutoID,a.cOWhCode,a.cIWhCode,b.cInvCode,b.cTVBatch,b.cFree1,b.cFree2,b.iTVQuantity
                from " + dbname + @"..transvouch a inner join " + dbname + @"..transvouchs b on a.ID=b.ID
                where a.cTVCode='" + tuocode + "'", "dtTranBody", Cmd);
            if (dtTranBody.Rows.Count == 0) throw new Exception("调拨单[" + tuocode + "]没有子表记录");
            
            //货位
            if (GetDataInt("select count(*) from " + dbname + "..position where cposcode='" + cposcode + "' and cwhcode='" + dtTranBody.Rows[0]["cIWhCode"] + "'", Cmd) == 0)
                throw new Exception("货位不存在,或仓库[" + dtTranBody.Rows[0]["cIWhCode"] + "]中无此货位");

            #endregion
            string errmsg = "";
            #region //调拨
            string dCurDate = GetDataString(@"select convert(varchar(10),getdate(),120)", Cmd);


            #region //主表 
            KK_U8Com.U8Rdrecord08 rd08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
            KK_U8Com.U8Rdrecord09 rd09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);

            //新增其他出库单主表
            rd09.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rd09.bredvouch = "0";
            rd09.cWhCode = "'" + dtTranBody.Rows[0]["cOWhCode"] + "'";
            rd09.cMaker = "'" + username + "'";
            rd09.dDate = "'" + dCurDate + "'";
            rd09.cHandler = "'" + username + "'";
            rd09.dVeriDate = "'" + dCurDate + "'";
            rd09.cCode = "'TO_" + tuocode + "'";
            //rd09.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
            //rd09.cDepCode = (dtHead.Rows[0]["调出部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调出部门"] + "'");
            rd09.iExchRate = "1";
            rd09.cExch_Name = "'人民币'";
            rd09.cBusCode = "'" + tuocode + "'";
            rd09.cBusType = "'调拨出库'";
            rd09.cSource = "'调拨'";
            //rd09.cDefine3 = "'AUTOINPUT'";
            rd09.VT_ID = 85;// 131395;
            if (!rd09.InsertToDB(cacc_id, ref errmsg)) { throw new Exception(errmsg); }


            //新增其他入库单主表
            rd08.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rd08.bredvouch = "0";
            rd08.cWhCode = "'" + dtTranBody.Rows[0]["cIWhCode"] + "'";
            rd08.cMaker = "'" + username + "'";
            rd08.dDate = "'" + dCurDate + "'";
            rd08.cHandler = "'" + username + "'";
            rd08.dVeriDate = "'" + dCurDate + "'";
            rd08.cCode = "'TI_" + tuocode + "'";
            //rd08.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            rd08.iExchRate = "1";
            rd08.cExch_Name = "'人民币'";
            rd08.cBusCode = "'" + tuocode + "'";
            rd08.cBusType = "'调拨入库'";
            rd08.cSource = "'调拨'";
            //rd08.cDepCode = (dtHead.Rows[0]["调入部门"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["调入部门"] + "'");
            //rd08.cDefine3 = "'AUTOINPUT'";
            if (!rd08.InsertToDB(cacc_id, ref errmsg)) { throw new Exception(errmsg); }
            #endregion

            #region  //货位账务处理
            for (int i = 0; i < dtTranBody.Rows.Count; i++)
            {
                //数量控制
                decimal d_st_rowqty = decimal.Parse("" + dtTranBody.Rows[i]["iTVQuantity"]);
                string cccbatch = "" + dtTranBody.Rows[i]["cTVBatch"];
                string cccfree1 = "" + dtTranBody.Rows[i]["cFree1"];
                string cccfree2 = "" + dtTranBody.Rows[i]["cFree2"];
                //写入其他出库单-货位帐
                #region
                KK_U8Com.U8Rdrecords09 rds09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);
                rds09.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds09.ID = rd09.ID;
                rds09.cInvCode = "'" + dtTranBody.Rows[i]["cInvCode"] + "'";
                rds09.iQuantity = "" + d_st_rowqty;
                rds09.iNQuantity = rds09.iQuantity;
                rds09.iNum = "0";
                rds09.irowno = (i + 1);
                rds09.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds09.cFree1 = (cccfree1 + "" == "" ? "null" : "'" + cccfree1 + "'");
                rds09.cFree2 = (cccfree2 + "" == "" ? "null" : "'" + cccfree2 + "'");
                rds09.iTrIds = "" + dtTranBody.Rows[i]["AutoID"];
                //rds09.cDefine24 = "'AUTOINPUT'";

                rds09.bCosting = "1";
                int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd09.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) rds09.bCosting = "0";
                rds09.isotype = "0";

                if (!rds09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                //更新现存量 待调出量
//                Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)-(0" + d_st_rowqty + @"),
//                        fTransInNum=isnull(fTransOutNum,0)-(0),iquantity=iquantity-(0" + d_st_rowqty + @"),inum=inum-(0)
//                    where cwhcode='" + dtTranBody.Rows[i]["cOWhCode"] + "' and cinvcode='" + dtTranBody.Rows[i]["cInvCode"] + "' and cfree1='" + dtTranBody.Rows[i]["cFree1"] + @"' 
//                        and cfree2='" + dtTranBody.Rows[i]["cFree2"] + @"' and cfree3='' and cfree4='' 
//                        and cfree5='' and cfree6='' and cfree7='' 
//                        and cfree8='' and cfree9='' and cfree10='' 
//                        and cbatch='" + cccbatch + "' and cVMIVenCode='' and iSoType=0 and iSodid=''";
//                Cmd.ExecuteNonQuery();

                //判定 库存账 负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..CurrentStock where cwhcode=" + rd09.cWhCode + " and cinvcode=" + rds09.cInvCode +
                    " and cbatch='" + cccbatch + "' and isnull(iquantity,0)-isnull(fTransOutQuantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("仓库[" + dtTranBody.Rows[i]["cOWhCode"] + "] 存货[" + dtTranBody.Rows[i]["cInvCode"] + "]可用量出现负库存");
                }
                #endregion


                //写入其他入库单
                #region
                KK_U8Com.U8Rdrecords08 rds08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);
                rds08.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + cacc_id + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                rds08.ID = rd08.ID;
                rds08.cInvCode = "'" + dtTranBody.Rows[i]["cInvCode"] + "'";
                rds08.iQuantity = "" + d_st_rowqty;
                rds08.iNum = "0";
                rds08.irowno = (i + 1);
                rds08.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
                rds08.cFree1 = (cccfree1 + "" == "" ? "null" : "'" + cccfree1 + "'");
                rds08.cFree2 = (cccfree2 + "" == "" ? "null" : "'" + cccfree2 + "'");

                rds08.iTrIds = "" + dtTranBody.Rows[i]["AutoID"];
                //rds08.cDefine24 = "'AUTOINPUT'";

                rds08.bCosting = "1";
                ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd08.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) rds08.bCosting = "0";

                rds08.cPosition = "'" + cposcode + "'";
                if (!rds08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                //写调拨单货位
                Cmd.CommandText = "update " + dbname + "..TransVouchs set cinposcode='" + cposcode + "' where AutoID=" + dtTranBody.Rows[i]["AutoID"];
                Cmd.ExecuteNonQuery();

                //更新现存量 待调入量
//                Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)-(0" + d_st_rowqty + @"),
//                        fTransInNum=isnull(fTransInNum,0)-(0),iquantity=iquantity+(0" + d_st_rowqty + @"),inum=inum+(0)
//                    where cwhcode='" + dtTranBody.Rows[i]["cIWhCode"] + "' and cinvcode='" + dtTranBody.Rows[i]["cInvCode"] + "' and cfree1='" + dtTranBody.Rows[i]["cFree1"] + @"' 
//                        and cfree2='" + dtTranBody.Rows[i]["cFree2"] + @"' and cfree3='' and cfree4='' 
//                        and cfree5='' and cfree6='' and cfree7='' 
//                        and cfree8='' and cfree9='' and cfree10='' 
//                        and cbatch='" + cccbatch + "' and cVMIVenCode='' and iSoType=0 and iSodid=''";
//                Cmd.ExecuteNonQuery();

                //添加货位记录
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,iExpiratdatecalcu,cMassUnit) " +
                    "Values (" + rds08.AutoID + "," + rds08.ID + "," + rd08.cWhCode + ",'" + cposcode + "'," + rds08.cInvCode + ",0" + rds08.iQuantity +
                    ",null,'" + cccbatch + "'," + rd08.dDate + ",1,'',0,'08'," + rd08.dDate + "," + rd08.cMaker + ",0,0)";
                Cmd.ExecuteNonQuery();

                //指定货位
                Cmd.CommandText = "update " + dbname + "..rdrecords08 set iposflag=1 where autoid =0" + rds08.AutoID;
                Cmd.ExecuteNonQuery();

                //修改货位库存
                if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + rd08.cWhCode + " and cinvcode=" + rds08.cInvCode + @"
                    and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values(" + rd08.cWhCode + ",'" + cposcode + "'," + rds08.cInvCode + ",0,'" + cccbatch + @"',
                        '','','','','','','','','','','','',0,0)";
                    Cmd.ExecuteNonQuery();
                }

                Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + rds08.iQuantity + ") where cwhcode=" + rd08.cWhCode +
                    " and cinvcode=" + rds08.cInvCode + " and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "'";
                Cmd.ExecuteNonQuery();

                ////判定负库存问题
                //if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode=" + rd08.cWhCode + " and cinvcode='" + cInv_Code +
                //    "' and cbatch='" + cccbatch + "' and cPosCode='" + cposcode + "' and isnull(iquantity,0)<0", Cmd) > 0)
                //{
                //    throw new Exception("【" + cInv_Code + "】出现负库存");
                //}

                #endregion

                
            }
            #endregion

            //审核调拨单
            Cmd.CommandText = "update " + dbname + "..TransVouch set cVerifyPerson='" + username + "',dVerifyDate='" + dCurDate + "' where cTVCode='" + tuocode + "'";
            Cmd.ExecuteNonQuery();


//            //记录托盘调拨信息
//            Cmd.CommandText = @"insert into T_CC_HL_Tuo_TRanDZ(cc_tuo_code,cc_tran_id,cc_Maker,cc_dDate) 
//                values('" + tuocode + "',0" + rd_id + ",'" + username + "','" + dCurDate + "')";
//            Cmd.ExecuteNonQuery();

            #endregion
            tr.Commit();
            return "";
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


    [WebMethod]  //U8 装箱[HL]    iValid 合格数   iNotValid不合格数=iValidFeipin 废品查缺数
    public string U8Mom_ProBox_Save_fanxiu(string boxcode, string PsnCode, string PFDID, string cSeqcode, int iValid, int iNotValid, int iValidFeipin, string dbname, string cacc_id, string username, string SN, ref string ErrMsg)
    {
        iNotValid = iValidFeipin;
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            ErrMsg = "序列号验证错误！";
            throw new Exception(ErrMsg);
        }
        ErrMsg = "";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            throw new Exception(ErrMsg);
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //判断箱码是否已经存在
        string cPFCode = "" + GetDataString("select cc_pfCode from " + dbname + "..T_CC_HL_BoxList where cc_boxcode='" + boxcode + "'", Cmd);
        if (cPFCode != "") throw new Exception("流转卡【" + cPFCode + "】已经使用此箱码");

        //获得上工序合格报工数
        int iUpProCount = 0;
        string pfid = GetDataString("select pfid from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
        string ccpfDocCode = GetDataString("select doccode from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);

        //本工序上工序流转卡子表 ID
        int iupflowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "'", Cmd);
        string iUppfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq<'" + cSeqcode + "' order by opseq desc", Cmd);
        //本工序下道工序 流转卡子表ID
        int ilowcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "'", Cmd);
        string ilowpfdid = "" + GetDataString("select top 1 PFDId from " + dbname + "..sfc_processflowdetail where pfid=" + pfid + " and opseq>'" + cSeqcode + "' order by opseq", Cmd);

        //上工序ID 若为空则代表本工序为第一道工序
        if (iupflowcount > 0)
        {
            //判断上到工序是否为 外协工序
            int iWXcount = GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + iUppfdid + " and subflag=1", Cmd);
            if (iWXcount == 0)
            {
                //是否 直接下达外协加工
                iWXcount = GetDataInt("select count(*) from " + dbname + "..HY_MODetails where isourceautoid=0" + iUppfdid, Cmd);
            }

            if (iWXcount > 0)
            {   //外协  取本上工序
                iUpProCount = GetDataInt("select @validqty=isnull(sum(iQualifiedQty),0) from " + dbname + "..hy_receivedetail where iMoRoutingDId=0" + iUppfdid, Cmd);   //上工序合格接收数量

            }
            else
            {
                //扣除合格 检验产出的废品
                iUpProCount = GetDataInt("select cast(isnull(sum(QualifiedQty),0) as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + iUppfdid, Cmd);  //上工序流转数量
            }

        }
        else
        {
            iUpProCount = GetDataInt("select cast(Qty as int) from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);  // 流转卡数量
        }

        //存在下到工序时，若下到工序为外协工序，视作没有此工序
        if (ilowcount > 0)
        {
            if (GetDataInt("select cast(SubFlag as int) from " + dbname + "..sfc_processflowdetail where PFDId=" + ilowpfdid, Cmd) > 0)
                ilowcount = 0; //下到工序 SubFlag=1 代表 外协工序
        }
        //获得本工序已经报工的数量  (合格数+报废数量)
        int iTheProCount = GetDataInt("select cast(  isnull(sum(QualifiedQty),0)+isnull(sum(ScrapQty),0)     as int) from " + dbname + "..sfc_pfreportdetail where PFDId=0" + PFDID, Cmd);
        if (iTheProCount + iValid + iNotValid > iUpProCount)
        {
            ErrMsg = "本工序最大可报工数【" + (iUpProCount - iTheProCount) + "】";
            return "";
        }
        //开始保存数据
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + username + "'";
            sfcMain.UpdCount = "0";
            sfcMain.Define2 = "'" + boxcode + "'";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);
            if (!sfcMain.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }

            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + PFDID;
            sfcDetail.EmployCode = "'" + PsnCode + "'";
            sfcDetail.Define23 = "'" + boxcode + "'";
            sfcDetail.MoRoutingDId = GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where pfdid=0" + PFDID, Cmd);
            sfcDetail.QualifiedQty = "" + iValid;
            sfcDetail.ScrapQty = "" + iNotValid;
            sfcDetail.RefusedQty = "0";   //[拒绝数需要修改成 返修数  本处为0]
            sfcDetail.Define34 = "" + iValidFeipin;  //报废差缺数
            sfcDetail.DeclareQty = "0";
            if (!sfcDetail.InsertToDB(ref ErrMsg)) { throw new Exception(ErrMsg); }
            //累计完工数  CompleteQty
            #region
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid) +
                ",define36=convert(nvarchar(20),getdate(),120) where PFDId=0" + PFDID;
            Cmd.ExecuteNonQuery();

            //写流转卡下道工序
            if (ilowcount > 0)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where PFDId=0" + ilowpfdid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where PFDId=0" + PFDID;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //回写工序计划
            string routing_did = "";//工序计划 下到工序 明细ID
            #region
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + (iValid + iNotValid) +
                ",balscrapqty=isnull(balscrapqty,0)+" + iNotValid + ",balmachiningqty=isnull(balmachiningqty,0)-" + (iValid + iNotValid) + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (ilowcount > 0)
            {
                //回写工序计划 中下到工序的 累计 加工数
                routing_did = "" + GetDataString("select MoRoutingDId from " + dbname + "..sfc_processflowdetail where PFDId=0" + ilowpfdid, Cmd);
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + iValid + " where MoRoutingDId=0" + routing_did;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + iValid + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //生成工序转移单
            #region
            //工序计划 主表ID
            string routing_ID = "" + GetDataString("select MoRoutingId from " + dbname + "..sfc_moroutingdetail where MoRoutingDId=0" + sfcDetail.MoRoutingDId, Cmd);
            string cc_moid = "" + GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + sfcMain.MoDId, Cmd);
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (iNotValid > 0)
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + (iNotValid ) + ",0," + iNotValid + ",0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (ilowcount > 0)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + routing_did + @",
                        " + iValid + ",0,0,0,0," + iValid + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + PFDID + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + iValid + "," + iValid + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + username + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //建立装箱清单
            #region
            //建表
            //CREATE TABLE [dbo].[T_CC_HL_TuoBox](	[cc_tuo_code] [nvarchar](50) NOT NULL,
            //    [cc_box_code] [nvarchar](50) NOT NULL,	[cc_Maker] [nvarchar](50) NULL,	[cc_dDate] [datetime] NULL,
            //  [cc_rd10autoid] [int] NOT NULL CONSTRAINT [DF_T_CC_HL_TuoBox_cc_rd10autoid]  DEFAULT ((0)),
            //  [cc_isChecked] [int] NULL CONSTRAINT [DF_T_CC_HL_TuoBox_cc_isChecked]  DEFAULT ((0)),
            // CONSTRAINT [PK_T_CC_HL_TuoBox] PRIMARY KEY CLUSTERED 
            //(	[cc_box_code] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            //) ON [PRIMARY]
            //GO
            //CREATE TABLE [dbo].[T_CC_HL_BoxList](
            //    [cc_BoxCode] [nvarchar](50) NOT NULL,	[cc_pfid] [int] NOT NULL,	[cc_iqty] [float] NULL,
            //    [cc_BoxDate] [datetime] NULL,	[cc_BoxPsn] [nvarchar](50) NULL,	[cc_BoxMaker] [nvarchar](50) NULL,[cc_reportdid] [int] NULL,
            // CONSTRAINT [PK_T_CC_HL_BoxList] PRIMARY KEY CLUSTERED (	[cc_BoxCode] ASC,	[cc_pfid] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            //) ON [PRIMARY]

            //检查是否存在 了对应关系
            if (GetDataInt("select count(*) from " + dbname + ".. T_CC_HL_BoxList where cc_BoxCode='" + boxcode + "' and cc_pfid=" + pfid, Cmd) > 0) throw new Exception("本箱码 中已经存在 这张流转卡");

            Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_BoxList(cc_BoxCode,cc_pfid,cc_iqty,cc_BoxDate,cc_BoxPsn,cc_BoxMaker,cc_reportdid,cc_pfCode) 
                values('" + boxcode + "'," + pfid + ",0" + iValid + ",convert(varchar(10),getdate(),120),'" + PsnCode + "','" + username + "',0" + sfcDetail.PFReportDId +
                          ",'" + ccpfDocCode + "')";
            Cmd.ExecuteNonQuery();
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into " + dbname + "..T_CC_HL_TuoBox(cc_box_code) values('" + boxcode + "')";
                Cmd.ExecuteNonQuery();
            }


            #endregion

            tr.Commit();
            return sfcMain.DocCode;
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

    [WebMethod]  //U8 装托盘[HL]
    public bool U8Mom_BoxTuo_Save(string tuoCode, string boxcode, string cMaker, string dbname, string SN)
    {
        
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误");
        }
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + boxcode + "' and isnull(cc_rd10autoid,0)>0 ", Cmd) > 0)
                throw new Exception("本箱码已经入库，不能重复装托盘");

            Cmd.CommandText = @"update " + dbname + "..T_CC_HL_TuoBox set cc_tuo_code='" + tuoCode + "',cc_Maker='" + cMaker + @"',cc_dDate=convert(varchar(10),getdate(),120) 
                where cc_box_code='" + boxcode + "'";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    [WebMethod]  //U8 过程质量缺陷[HL]
    public void U8Mom_QX_HLQXSave(System.Data.DataSet dst, string dbname, string accid, string cusername, string SN)
    {
         //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            throw new Exception("序列号验证错误");
        }
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败");
        }


        System.Data.DataTable dtData = dst.Tables[0];

        //获得存货编码
        string cinvcode = "" + GetDataString("select c.InvCode from " + dbname + @"..sfc_processflow a 
                inner join " + dbname + @"..sfc_processflowdetail b on a.PFId=b.PFid
                inner join " + dbname + @"..mom_orderdetail c on a.MoDId =c.MoDId
            where b.PFDId=0" + dtData.Rows[0]["pfdid"], Conn);
        //获得工序编码
        string copcode = "" + GetDataString("select c.OpCode from " + dbname + @"..sfc_processflow a 
                inner join " + dbname + @"..sfc_processflowdetail b on a.PFId=b.PFid
                inner join " + dbname + @"..sfc_operation c on b.OperationId =c.OperationId
            where b.PFDId=0" + dtData.Rows[0]["pfdid"], Conn);

        string cyzopcode = "" + GetDataString("select cValue from " + dbname + @"..T_Parameter where cPID='chengjiangongxu'", Conn);

        int ihgs = 0;
        int ijys = 0;
        int ifxs = 0;
        int ifps = 0;

        int ijyAll = 0;
        int iqtAll = 0;

        for (int i = 0; i < dtData.Rows.Count; i++)
        {
            ijys = int.Parse(dtData.Rows[i]["ijys"] + "");//交验
            ihgs = int.Parse(dtData.Rows[i]["ihgs"] + "");//合格
            ifxs = int.Parse(dtData.Rows[i]["ifxs"] + "");//返修
            ifps = int.Parse(dtData.Rows[i]["ifps"] + "");//废品

            ijyAll += ijys;
            iqtAll += (ihgs + ifxs + ifps);
        }

        if (cyzopcode.IndexOf(copcode) > -1)
        {
            //判断是否存在 逻辑数量关系
            if (ijyAll != iqtAll)
            {
                throw new Exception("要求交验数 必须等于 合格数+返修数+废品数");
            }
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //保存数据
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                ijys = int.Parse(dtData.Rows[i]["ijys"] + "");//交验
                ihgs = int.Parse(dtData.Rows[i]["ihgs"] + "");//合格
                ifxs = int.Parse(dtData.Rows[i]["ifxs"] + "");//返修
                ifps = int.Parse(dtData.Rows[i]["ifps"] + "");//废品
                
                System.Data.DataRow dr= dtData.Rows[i];   
                //插入

                Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_DefectData( cc_cardno, cc_invcode, ibqqty, cc_res_dpt, cc_defectcode, 
                        cseqno, copcode, cc_find_dpt, validqty, wasteqty, repairqty, dmakedate, cmaker) 
                    values('" +dr["pfcode"]+"','"+cinvcode+"',0"+ijys+", '"+dr["czrdepcode"]+"', '"+dr["cqxcode"]+@"', 
                        '" + dr["seq"] + "', '" + copcode + "', '" + dr["cfxdepcode"] + "',0" + ihgs + ",0" + ifps + ", 0" + ifxs + ",convert(varchar(10),getdate(),120),'"+cusername+"')";
                Cmd.ExecuteNonQuery();
            }

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
    }

    [WebMethod]  //U8 流转卡工序接收[HL]
    public bool U8Mom_FlowSeqReceived(string pfdid,float iqty,string cposcode,System.Data.DataTable dtItemData, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");
        if (iqty <= 0) throw new Exception("接收数量不允许为0");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //判定是否已经取出
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Pos_TZ where cc_pfdid=0" + pfdid + "", Cmd) > 0)
                throw new Exception("流转卡已经有货位记录，不能重复入货位");
            System.Data.DataTable dtData = GetSqlDataTable(@"select m.doccode,b.DeptCode,a.OpSeq,o.OpCode
                from " + dbname + "..sfc_processflowdetail a inner join " + dbname + @"..sfc_processflow m on a.pfid=m.pfid
                left join " + dbname + @"..sfc_workcenter b on a.wcid=b.wcid
                left join " + dbname + @"..department d on b.DeptCode=d.cdepcode
                left join " + dbname + @"..sfc_operation o on a.OperationId=o.OperationId
                where a.pfdid=0" + pfdid, "data", Cmd);
            if (dtData.Rows.Count > 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Position where cc_depcode='" + dtData.Rows[0]["DeptCode"] + "' and cc_opcode='" + dtData.Rows[0]["OpCode"] + "' and cc_ps_code='" + cposcode + "'", Cmd) == 0)
                    throw new Exception("[" + cposcode + "]不存在");

                //判断本货位是否已经存放流转卡
                string c_old_pos_card = GetDataString("select cc_cardno from " + dbname + @"..T_CC_HL_HE_Pos_TZ where cc_poscode='" + cposcode + @"' 
                    and cc_depcode='" + dtData.Rows[0]["DeptCode"] + "' and cc_opcode='" + dtData.Rows[0]["OpCode"] + "' and isnull(cc_maker_out,'')=''", Cmd);
                if (c_old_pos_card + "" != "") throw new Exception("货位【" + cposcode + "】已经存放流转卡【" + c_old_pos_card + "】");

                //更新
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_HE_Pos_TZ(cc_pfdid,cc_opseq,cc_cardno,cc_qty,cc_depcode,cc_opcode,cc_poscode,cc_maker_in,cc_in_time) 
                values(0" + pfdid + ",'" + dtData.Rows[0]["OpSeq"] + "','" + dtData.Rows[0]["doccode"] + "',0" + iqty + ",'" + dtData.Rows[0]["DeptCode"] + "','" + dtData.Rows[0]["OpCode"] +
                          "','" + cposcode + "','" + cusername + "',getdate())";
                Cmd.ExecuteNonQuery();
            }
            tr.Commit();
            return true;
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

    [WebMethod]  //U8 流转卡工序开工[HL]
    public bool U8Mom_FlowSeqStart(string pfdid, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set StartTime=getdate(),Define37=getdate() where pfdid=0" + pfdid;
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    [WebMethod]  //U8 获得需要开工的流转卡号 货位移出[HL]
    public bool U8Mom_FlowSeqOut(string pfdid, float iqty, string cposcode, System.Data.DataTable dtData, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //判定是否已经取出
            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Pos_TZ where cc_pfdid=0" + pfdid + " and isnull(cc_maker_out,'')=''", Cmd) == 0)
                throw new Exception("流转卡已经取出，不能重复取");

            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Pos_TZ where cc_pfdid=0" + pfdid + " and isnull(cc_maker_out,'')='' and cc_opcode='5007'", Cmd) > 0)
            {
                string ccc_invcode = GetDataString(@"select c.InvCode from " + dbname + "..sfc_processflowdetail a inner join " + dbname + @"..sfc_processflow b on a.PFId=b.PFId 
                    inner join " + dbname + @"..mom_orderdetail c on b.MoDId=c.MoDId
                    where a.PFDId=0"+pfdid,Cmd);
                //包装工序 必须看 尾卡
                if (GetDataInt("select count(*) from " + dbname + "..T_HL_Card_WeiKa where cc_invcode='" + ccc_invcode + "' and isnull(cc_maker_out,'')=''", Cmd) > 0)
                    throw new Exception("存在尾卡需要包装");
            }

            //更新
            Cmd.CommandText = "update " + dbname + "..T_CC_HL_HE_Pos_TZ set cc_maker_out='" + cusername + "',cc_out_time=getdate() where cc_pfdid=0" + pfdid;
            Cmd.ExecuteNonQuery();
            //移出即开工
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set StartTime=getdate(),DueTime=getdate(),Define37=getdate() where pfdid=0" + pfdid;
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    //尾卡和尾托的处理
    #region
    [WebMethod]  //U8 流转卡 尾卡存放[HL]
    public bool U8Mom_Flow_081030_Received(string pfdid, float iqty, string cposcode, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");
        if (iqty <= 0) throw new Exception("临存数量不允许为0");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            System.Data.DataTable dtData = GetSqlDataTable(@"select m.doccode,b.DeptCode,a.OpSeq,o.OpCode,k.InvCode
                from " + dbname + "..sfc_processflowdetail a inner join " + dbname + @"..sfc_processflow m on a.pfid=m.pfid
                inner join " + dbname + @"..mom_orderdetail k on m.modid=k.MoDId
                left join " + dbname + @"..sfc_workcenter b on a.wcid=b.wcid
                left join " + dbname + @"..department d on b.DeptCode=d.cdepcode
                left join " + dbname + @"..sfc_operation o on a.OperationId=o.OperationId
                where a.pfdid=0" + pfdid, "dtData", Cmd);
            if (dtData.Rows.Count == 0) throw new Exception("未找到流转卡信息");
            string c_can_save_oplist = GetDataString("select cValue from " + dbname + "..T_Parameter where cpid='u8_flow_save_card'", Cmd);
            if (c_can_save_oplist.IndexOf("" + dtData.Rows[0]["OpCode"]) < 0) throw new Exception("工序[" + dtData.Rows[0]["OpCode"] + "]不允许临存，请维护参数【临存工序】");

            //判定是否已经取出
            if (GetDataInt("select count(*) from " + dbname + "..T_HL_Card_WeiKa where cc_cardno='" + dtData.Rows[0]["doccode"] + "' and isnull(cc_maker_out,'')=''", Cmd) > 0)
                throw new Exception("流转卡已经存入尾卡库");

            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Position where cc_opcode='AAAA' and cc_ps_code='" + cposcode + "'", Cmd) == 0)
                throw new Exception("货位[" + cposcode + "]不存在");

            //判断本货位是否已经存放流转卡
            string c_old_pos_card = GetDataString("select cc_cardno from " + dbname + @"..T_HL_Card_WeiKa where cc_poscode='" + cposcode + @"' 
                    and isnull(cc_maker_out,'')=''", Cmd);
            if (c_old_pos_card + "" != "") throw new Exception("货位【" + cposcode + "】已经存放流转卡【" + c_old_pos_card + "】");

            string c_cinvcode = "" + dtData.Rows[0]["InvCode"];
            //更新
            Cmd.CommandText = "insert into " + dbname + @"..T_HL_Card_WeiKa(cc_pfdid,cc_cardno,cc_qty,cc_poscode,cc_maker_in,cc_in_time,cc_invcode) 
                values(0" + pfdid + ",'" + dtData.Rows[0]["doccode"] + "',0" + iqty + ",'" + cposcode + "','" + cusername + "',getdate(),'" + c_cinvcode + "')";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    [WebMethod]  //U8 流转卡 尾卡取出[HL]
    public bool U8Mom_Flow_081030_Out(string pfdid, float iqty, string cposcode, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");
        if (iqty <= 0) throw new Exception("出库不允许为0");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            System.Data.DataTable dtData = GetSqlDataTable(@"select m.doccode,b.DeptCode,a.OpSeq,o.OpCode,md.invcode
                from " + dbname + "..sfc_processflowdetail a inner join " + dbname + @"..sfc_processflow m on a.pfid=m.pfid
                inner join " + dbname + @"..mom_orderdetail md on m.modid=md.modid
                left join " + dbname + @"..sfc_workcenter b on a.wcid=b.wcid
                left join " + dbname + @"..department d on b.DeptCode=d.cdepcode
                left join " + dbname + @"..sfc_operation o on a.OperationId=o.OperationId
                where a.pfdid=0" + pfdid, "data", Cmd);
            if (dtData.Rows.Count == 0) throw new Exception("未找到流转卡信息");

            //判定是否已经取出
            if (GetDataInt("select count(*) from " + dbname + "..T_HL_Card_WeiKa where cc_cardno='" + dtData.Rows[0]["doccode"] + "' and isnull(cc_maker_out,'')=''", Cmd) == 0)
                throw new Exception("流转卡 没有存入尾卡库");

            //判定应该出那张卡
            string cc_first_card = GetDataString(@"select top 1 a.cc_cardno from (
	                select cc_cardno from " + dbname + @"..T_HL_Card_WeiKa where cc_invcode='" + dtData.Rows[0]["invcode"] + @"' and ISNULL(cc_maker_out,'')=''
                ) a inner join (
	                select cc_cardno,MIN(cc_in_time) in_time from " + dbname + @"..T_HL_Card_WeiKa 
	                where cc_invcode='" + dtData.Rows[0]["invcode"] + @"' group by cc_cardno
                ) b on a.cc_cardno=b.cc_cardno
                order by b.in_time", Cmd);

            if (cc_first_card.CompareTo("" + dtData.Rows[0]["doccode"]) != 0)
                throw new Exception("当前优先出库的流转卡为[" + cc_first_card + "]");

            //更新
            Cmd.CommandText = "update " + dbname + @"..T_HL_Card_WeiKa set cc_maker_out='" + cusername + @"',cc_out_time=getdate() 
                where cc_pfdid=0" + pfdid + " and isnull(cc_maker_out,'')=''";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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


    [WebMethod]  //U8 流转卡 尾托存放[HL]
    public bool U8Mom_Flow_081030_Tuo_Received(string ctuocode, string c_cinvcode, float iqty, string cposcode, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");
        if (iqty <= 0) throw new Exception("临存数量不允许为0");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_TuoBox where cc_tuo_code='" + ctuocode + "'", Cmd) == 0)
                throw new Exception("不存在托盘信息");

            if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_BoxList a inner join " + dbname + @"..T_CC_HL_TuoBox b on a.cc_BoxCode=b.cc_box_code
                where b.cc_tuo_code='" + ctuocode + "' and b.cc_rd10autoid=0", Cmd) == 0)
                throw new Exception("托盘已经入库");

            //判定是否已经存放
            if (GetDataInt("select count(*) from " + dbname + "..T_HL_TUO_Weituo where cc_tuo_code='" + ctuocode + "' and isnull(cc_maker_out,'')=''", Cmd) > 0)
                throw new Exception("托盘已经存入尾托库");

            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_HE_Position where cc_opcode='AAAA' and cc_ps_code='" + cposcode + "'", Cmd) == 0)
                throw new Exception("货位[" + cposcode + "]不存在");

            //判断本货位是否已经存放流转卡
            string c_old_pos_card = GetDataString("select cc_tuo_code from " + dbname + @"..T_HL_TUO_Weituo where cc_poscode='" + cposcode + @"' 
                    and isnull(cc_maker_out,'')=''", Cmd);
            if (c_old_pos_card + "" != "") throw new Exception("货位【" + cposcode + "】已经存放托盘【" + c_old_pos_card + "】");

            //更新
            Cmd.CommandText = "insert into " + dbname + @"..T_HL_TUO_Weituo(cc_tuo_code,cc_qty,cc_poscode,cc_maker_in,cc_in_time,cc_invcode) 
                values('" + ctuocode + "',0" + iqty + ",'" + cposcode + "','" + cusername + "',getdate(),'" + c_cinvcode + "')";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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


    [WebMethod]  //U8 流转卡 尾托取出[HL]
    public bool U8Mom_Flow_081030_Tuo_Out(string ctuocode, float iqty, string cposcode, string dbname, string accid, string cusername, string SN)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn")) throw new Exception("序列号验证错误！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");
        if (iqty <= 0) throw new Exception("发出数量不允许为0");

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_HL_TuoBox where cc_tuo_code='" + ctuocode + "'",Cmd) == 0) 
                throw new Exception("未找到托盘信息");

            //判定是否已经取出
            if (GetDataInt("select count(*) from " + dbname + "..T_HL_TUO_Weituo where cc_tuo_code='" + ctuocode + "' and isnull(cc_maker_out,'')=''", Cmd) == 0)
                throw new Exception("托盘 没有存入尾托库,或已经取出");

            //更新
            Cmd.CommandText = "update " + dbname + @"..T_HL_TUO_Weituo set cc_maker_out='" + cusername + @"',cc_out_time=getdate() 
                where cc_tuo_code='" + ctuocode + "' and isnull(cc_maker_out,'')=''";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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




    #endregion

    #region //货位管理HL
    [WebMethod]  //U8 流转卡号 货位转移[HL]
    public string U8Mom_Flow_200330_PosTrans(string doccode, string cposcode, string dbname, string accid, string cusername, string logdate)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            bool b_topcard = false;//是否新卡
            //判断当前是否为固定货位
            bool b_gd_pos = false;
            //cposcode = GetDataString("select cposcode from " + dbname + "..position where cposcode='" + cposcode + "' or  or cBarCode='" + cposcode + "'", Cmd);

            if (GetDataInt("select count(*) from " + dbname + "..T_CC_HL_0330_Position where cc_ps_code='" + cposcode + "'", Cmd) > 0) b_gd_pos = true;
            string cnew_whcode = GetDataString("select cwhcode from " + dbname + @"..Position where cposcode='" + cposcode + "'", Cmd);
            string cnew_depcode = GetDataString("select cc_depcode from " + dbname + @"..T_CC_HL_0330_Position where cc_ps_code='" + cposcode + "'", Cmd);

            System.Data.DataTable dtCardInfo = GetSqlDataTable(@"select a.cc_cardno,cc_poscode,b.cWhCode,cc_invcode,a.cc_qty,isnull(cc_is_ruku,0) cc_is_ruku,
                isnull(cc_top_card,0) cc_top_card,a.cc_rd_batchno,c.cc_mapi_cardno,a.cc_depcode,a.cc_ck_maker,d.Define11 cfree1,d.Define12 cfree2
                from " + dbname + "..T_CC_HL_0330_Card_State a inner join " + dbname + @"..Position b on a.cc_poscode=b.cPosCode
                inner join " + dbname + @"..sfc_processflow d on a.cc_cardno=d.doccode
                left join " + dbname + @"..T_CC_HL_0330_Card_DZ c on a.cc_cardno=c.cc_jj_cardno
                where a.cc_cardno='" + doccode + "'", "dtCardInfo", Cmd);
            if (dtCardInfo.Rows.Count == 0) throw new Exception("找不到流转卡的货位存放信息");
            if ("" + dtCardInfo.Rows[0]["cc_ck_maker"] != "") throw new Exception("当前卡已经出库，不能转移");
            //判断仓库一致性
            if (cnew_whcode != "" + dtCardInfo.Rows[0]["cWhCode"]) throw new Exception("新旧货位所在仓库不一致，不能转移");
            //判断当前卡是否已经入库
            if (int.Parse("" + dtCardInfo.Rows[0]["cc_is_ruku"]) == 0) throw new Exception("流转卡没有入库，不能转移");
            
            //判断当前卡是否新卡
            if (int.Parse("" + dtCardInfo.Rows[0]["cc_top_card"]) == 1 && "" + dtCardInfo.Rows[0]["cc_mapi_cardno"] == "")
            {
                b_topcard = true;  //老卡
                if (b_gd_pos) throw new Exception("旧卡（毛坯卡）不能直接进入固定货位");
            }
            else
            {
                dtCardInfo = GetSqlDataTable(@"select a.cc_cardno,'" + dtCardInfo.Rows[0]["cc_poscode"] + @"' cc_poscode,b.cWhCode,
                    cc_invcode," + dtCardInfo.Rows[0]["cc_qty"] + @" cc_qty,isnull(cc_is_ruku,0) cc_is_ruku,
                    isnull(cc_top_card,0) cc_top_card,a.cc_rd_batchno,'" + dtCardInfo.Rows[0]["cc_depcode"] + @"' cc_depcode,a.cc_ck_maker,d.Define11 cfree1,d.Define12 cfree2
                from " + dbname + "..T_CC_HL_0330_Card_State a inner join " + dbname + @"..Position b on a.cc_poscode=b.cPosCode
                inner join " + dbname + @"..sfc_processflow d on a.cc_cardno=d.doccode
                where cc_cardno='" + dtCardInfo.Rows[0]["cc_mapi_cardno"] + "'", "dtCardInfo", Cmd);
            }
            if (dtCardInfo.Rows.Count == 0) throw new Exception("当前卡为机加卡，但未找到对应的旧卡(毛坯卡)信息");

            //固定货位的情况下，判断流转卡部门与货位部门是否一致
            if (b_gd_pos && cnew_depcode != "" + dtCardInfo.Rows[0]["cc_depcode"]) throw new Exception("流转卡所在部门与当前货位部门不一致，不能转移");

            #region //货位转移单

            System.Data.DataTable dtHead = new System.Data.DataTable("dtHead");
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            System.Data.DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cnew_whcode;
            dr["TxtValue"] = cnew_whcode;
            dtHead.Rows.Add(dr);

            System.Data.DataTable dtBody = GetSqlDataTable(@"select '" + dtCardInfo.Rows[0]["cc_invcode"] + @"' cinvcode,
                '" + dtCardInfo.Rows[0]["cc_poscode"] + @"' cbposcode,'" + cposcode + @"' caposcode,'" + dtCardInfo.Rows[0]["cc_rd_batchno"] + @"' cbatch,
                " + dtCardInfo.Rows[0]["cc_qty"] + @" iquantity,'" + dtCardInfo.Rows[0]["cfree1"] + @"' cfree1,'" + dtCardInfo.Rows[0]["cfree2"] + @"' cfree2", "dtBody", Cmd);
            U8StandSCMBarCode u8rd10 = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8rd10.U81087(dtHead, dtBody, dbname, cusername, logdate, "U81087", Cmd);
            string transid = crd_ret.Split(',')[0];  //调整单ID
            string transcode = crd_ret.Split(',')[1];  //调整单号
            #endregion

            Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_poscode='" + cposcode + "' where cc_cardno='" + doccode + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return transcode;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 流转卡号 出库[HL]
    public bool U8Mom_Flow_200330_PosOut(string doccode, string dbname, string cusercode, string cusername)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            System.Data.DataTable dtCardInfo = GetSqlDataTable(@"select a.cc_cardno,isnull(cc_is_ruku,0) cc_is_ruku,a.cc_ck_maker,cc_receive_maker,ISNULL(cc_top_card,0) cc_top_card
                from " + dbname + @"..T_CC_HL_0330_Card_State a 
                where cc_cardno='" + doccode + "'", "dtCardInfo", Cmd);
            if (dtCardInfo.Rows.Count == 0) throw new Exception("找不到流转卡的货位存放信息");
            if (int.Parse("" + dtCardInfo.Rows[0]["cc_top_card"]) == 1) throw new Exception("请扫描新机加流转卡出库");
            if (int.Parse("" + dtCardInfo.Rows[0]["cc_is_ruku"]) == 0) throw new Exception("当前卡没有入库，不能出库");
            if ("" + dtCardInfo.Rows[0]["cc_ck_maker"] != "") throw new Exception("当前卡已经出库，不能重复操作");
            if ("" + dtCardInfo.Rows[0]["cc_receive_maker"] != "") throw new Exception("当前卡已经接收，不能出库");
            string cpsnnum = GetDataString(@"select a.cPsn_Num from " + dbname + @"..UserHrPersonContro a where a.cUser_Id='" + cusercode + "' ", Cmd);
            if (cpsnnum == "") throw new Exception("当前操作员没有业务员信息，请在人员档案中设置");

            Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_ck_mk_code='" + cpsnnum + "',cc_ck_maker='" + cusername + "',cc_ck_date=getdate() where cc_cardno='" + doccode + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }

    }

    [WebMethod]  //U8 流转卡送达车间确认[HL]
    public bool U8Mom_Flow_200330_CardAchieveDepartment(string doccode, string dbname, string cdepbarcode, string cusername)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            System.Data.DataTable dtCardInfo = GetSqlDataTable(@"select a.cc_cardno,isnull(cc_Achieve_maker,'') cc_Achieve_maker,a.cc_ck_maker,cc_receive_maker,ISNULL(cc_top_card,0) cc_top_card,cc_depcode
                from " + dbname + @"..T_CC_HL_0330_Card_State a 
                where cc_cardno='" + doccode + "'", "dtCardInfo", Cmd);
            if (dtCardInfo.Rows.Count == 0) throw new Exception("找不到流转卡的货位存放信息");
            if (int.Parse("" + dtCardInfo.Rows[0]["cc_top_card"]) == 1) throw new Exception("请扫描新机加流转卡出库");
            if ("" + dtCardInfo.Rows[0]["cc_ck_maker"] == "") throw new Exception("当前卡未出库");
            if ("" + dtCardInfo.Rows[0]["cc_Achieve_maker"] != "") throw new Exception("不能重复操作送达确认操作");
            if ("" + dtCardInfo.Rows[0]["cc_receive_maker"] != "") throw new Exception("当前卡已经接收，不能送达确认");

            string cdepnum = GetDataString(@"select a.cdepcode from " + dbname + @"..department a where a.vAuthorizeUnit='" + cdepbarcode + "' ", Cmd);
            if (cdepnum == "") throw new Exception("没有找到部门，或部门的条码[批准单位栏目]信息没有维护");
            if (cdepnum.CompareTo("" + dtCardInfo.Rows[0]["cc_depcode"]) != 0) throw new Exception("送达部门错误，送达部门不是本开的流向部门");

            Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_Achieve_maker='" + cusername + "',cc_Achieve_date=getdate() where cc_cardno='" + doccode + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }

    }

    [WebMethod]  //U8 流转卡号 车间接收[HL]
    public bool U8Mom_Flow_200330_DeptReceive(string doccode, string dbname, string caccid, string cusercode, string cusername)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string PFID = GetDataString("select pfid from " + dbname + @"..sfc_processflow where doccode='" + doccode + "'", Cmd);
            string c_rpt_did = "";
            string c_out_rd11code = "";
            #region  //检查
            if (PFID == "") throw new Exception("未找到流转卡");
            System.Data.DataTable dtCardInfo = GetSqlDataTable(@"select a.cc_cardno,a.cc_ck_maker,cc_receive_maker,cc_depcode,cc_ck_mk_code,cc_qty,cc_rd_batchno,
                    cc_whcode,cc_poscode,convert(varchar(10),getdate(),120) cdate,b.modid,b.Define11 cfree1,b.Define12 cfree2
                from " + dbname + @"..T_CC_HL_0330_Card_State a inner join " + dbname + @"..sfc_processflow b on a.cc_cardno=b.doccode
                where a.cc_cardno='" + doccode + "'", "dtCardInfo", Cmd);
            if (dtCardInfo.Rows.Count == 0) throw new Exception("找不到流转卡的货位存放信息");
            if ("" + dtCardInfo.Rows[0]["cc_ck_maker"] == "") throw new Exception("当前卡没有出库");
            if ("" + dtCardInfo.Rows[0]["cc_receive_maker"] != "") throw new Exception("当前卡已经接收，不能重复操作");
            //当前操作员所在部门
            string cdepcode = GetDataString(@"select b.cDept_num from " + dbname + @"..UserHrPersonContro a inner join " + dbname + @"..hr_hi_person b on a.cPsn_Num=b.cPsn_Num 
                where a.cUser_Id='" + cusercode + "' ", Cmd);
            if (cdepcode != "" + dtCardInfo.Rows[0]["cc_depcode"]) throw new Exception("接收人所在部门与流转卡的部门不一致");
            //是否已经报工，已经报工的流转卡，不能接收
            if (GetDataInt("select COUNT(*) from " + dbname + @"..sfc_pfreport where PFId=" + PFID, Cmd) > 0)
            {
                throw new Exception("本流转卡已经存在报工记录，不能接收。接收必须再报工之前");
            }
            #endregion

            //接收时，自动送达确认
            Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_receive_maker='" + cusername + @"',cc_receive_date=getdate(),
                cc_Achieve_maker=cc_ck_maker,cc_Achieve_date=getdate() where cc_cardno='" + doccode + "'";
            Cmd.ExecuteNonQuery();
            #region //自动收工序报工
            //获得首工序pfdid
            System.Data.DataTable dtSeqInfo = GetSqlDataTable("select top 1 PFDId,OpSeq from " + dbname + @"..sfc_processflowdetail where PFId=" + PFID + " order by OpSeq", "dtSeqInfo", Cmd);
            if (dtSeqInfo.Rows.Count==0) throw new Exception("本流转卡没有工序信息");
            string pfdid = dtSeqInfo.Rows[0]["PFDId"]+"";
            int ivalidqty = int.Parse("" + dtCardInfo.Rows[0]["cc_qty"]);
            string cRetMsg = U8Mom_Pro_Save_HaiLing("" + dtCardInfo.Rows[0]["cc_ck_mk_code"], pfdid, "" + dtSeqInfo.Rows[0]["OpSeq"], ivalidqty,
                0, 0, 0, 0, 0, "", dbname, caccid, cusername, "", null, false, Cmd);
            c_rpt_did = cRetMsg.Split(',')[0];  //报工单did
            #endregion

            #region //自动材料出库单
            string ccc_rdcode = "220"; //材料出库类别代码
            U8StandSCMBarCode u8op = new U8StandSCMBarCode();
            int iOut_Qty = ivalidqty;

            //获得出库的材料
            string c_out_invcode = GetDataString(@"select c.cc_invcode
                    from " + dbname + @"..T_CC_HL_0330_Card_State a inner join " + dbname + @"..T_CC_HL_0330_Card_DZ b on a.cc_cardno=b.cc_jj_cardno
                    inner join " + dbname + @"..T_CC_HL_0330_Card_State c on b.cc_mapi_cardno=c.cc_cardno
                    where a.cc_cardno='" + doccode + "'", Cmd);
            if (c_out_invcode == "") throw new Exception("未找到本卡的 拆分前流转卡信息(无毛坯卡信息)");

            System.Data.DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + ccc_rdcode + "' crdcode,'" + dtCardInfo.Rows[0]["cc_whcode"] + @"' cwhcode,
                '" + dtCardInfo.Rows[0]["cc_depcode"] + "' cdepcode,'领料' cbustype,'000' headoutposcode", "dtRdMain", Cmd);
            System.Data.DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select top 1 b.allocateid,b.invcode cinvcode,'' cbvencode,'" + dtCardInfo.Rows[0]["cc_rd_batchno"] + @"' cbatch,
                        " + iOut_Qty + @" iquantity,b.modid,'" + dtCardInfo.Rows[0]["cc_poscode"] + @"' cposcode,
                        '" + dtCardInfo.Rows[0]["cfree1"] + @"' free1,'" + dtCardInfo.Rows[0]["cfree2"] + @"' free2
                    from " + dbname + @"..mom_moallocate b 
                    where b.modid=0" + dtCardInfo.Rows[0]["modid"] + " and b.invcode ='" + c_out_invcode + @"'", "dtRddetail", Cmd);
            if (dtRddetail.Rows.Count == 0) throw new Exception("无法找到可出库的生成订单子件数据,请查询生产订单的子件材料");
            System.Data.DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            cRetMsg = u8op.U81016(SHeadData, dtRddetail, dbname, cusername, dtCardInfo.Rows[0]["cdate"] + "", "U81016", Cmd);

            c_out_rd11code = cRetMsg.Split(',')[1];
            #endregion
            Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_rpt_did=" + c_rpt_did + ",cc_receive_rd11code='" + c_out_rd11code + @"' where cc_cardno='" + doccode + "'";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "update " + dbname + "..sfc_processflow set Define1='" + c_out_rd11code + "',Define9='" + dtCardInfo.Rows[0]["cc_whcode"] + "' where pfid=" + PFID;
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //U8 流转卡号 修改流转卡数量[HL]
    public bool U8Mom_Flow_200330_CardQtyModify(string doccode, string dbname, string qty, string cusername)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string PFID = GetDataString("select pfid from " + dbname + @"..sfc_processflow where doccode='" + doccode + "'", Cmd);
            if (PFID == "") throw new Exception("未找到流转卡");
            //是否已经报工，已经报工的流转卡，不能接收
            if (GetDataInt("select COUNT(*) from " + dbname + @"..sfc_pfreport where PFId=" + PFID, Cmd) > 0)
            {
                throw new Exception("本流转卡已经存在报工记录，不能修改流转卡数量");
            }
            if (int.Parse(qty) <= 0) throw new Exception("流转卡数量必须大于0");
            Cmd.CommandText = "update " + dbname + "..sfc_processflow set Qty=" + qty + @",ModifyDate=convert(varchar(10),getdate(),120),ModifyTime=getdate(),ModifyUser='" + cusername + "' where PFId=" + PFID;
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #endregion

    #region   //小车处理

    [WebMethod]  //发车
    public void PDA_SendCar(System.Data.DataTable dtHead,System.Data.DataTable dtBody,string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_Car_records set t_in_time=getdate(),t_in_maker='修正状态' where t_car_no='" + dtBody.Rows[i]["小车条码"] + "' and isnull(t_in_maker,'')=''";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "insert into " + dbname + "..T_CC_Car_records(t_car_no,t_cuscode,t_out_maker,t_out_time) values('" + dtBody.Rows[i]["小车条码"] + @"',
                '" + dtHead.Rows[0]["客户编码"] + "','" + dtHead.Rows[0]["制单人"] + "',getdate())";
                Cmd.ExecuteNonQuery();
                if (GetDataInt("select count(*) from " + dbname + "..T_CC_Car_list where t_car_no='" + dtBody.Rows[i]["小车条码"] + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + ".. T_CC_Car_list(t_car_no,istate) values('" + dtBody.Rows[i]["小车条码"] + "',0)";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..T_CC_Car_list set istate=1 where t_car_no='" + dtBody.Rows[i]["小车条码"] + "'";
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //收车
    public void PDA_ReceiveCar(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_Car_records set t_in_time=getdate(),t_in_maker='" + dtHead.Rows[0]["制单人"] + "' where t_car_no='" + dtBody.Rows[i]["小车条码"] + "' and isnull(t_in_maker,'')=''";
                Cmd.ExecuteNonQuery();

                if (GetDataInt("select count(*) from " + dbname + "..T_CC_Car_list where t_car_no='" + dtBody.Rows[i]["小车条码"] + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + "..T_CC_Car_list(t_car_no,istate) values('" + dtBody.Rows[i]["小车条码"] + "',0)";
                    Cmd.ExecuteNonQuery();
                }

                Cmd.CommandText = "update " + dbname + "..T_CC_Car_list set istate=0 where t_car_no='" + dtBody.Rows[i]["小车条码"] + "'";
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region   //供应链业务
    [WebMethod]  //U8 保存到货单（更新 检验数） [HT]
    public string U8SCM_Update_PuArrival(System.Data.DataTable dtBody, string dbname,string username, string SN, ref string ErrMsg)
    {
        ErrMsg = "";//autoid,id,合格数,报检数,货品名称,规格型号,单位,货品编码,不合格数,累计入库数,是否修改
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            ErrMsg = "序列号验证错误！";
            throw new Exception(ErrMsg);
        }
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            ErrMsg = "数据库连接失败！";
            throw new Exception(ErrMsg);
        }

        string c_id = "0";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            int iupdaterows = 0;
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                c_id = dtBody.Rows[i]["id"] + "";
                //if (dtBody.Rows[i]["是否修改"]+"" == "0") continue;
                //核查入库情况
                if (GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs where autoid=0" + dtBody.Rows[i]["autoid"] + " and isnull(fValidInQuan,0)<>0", Cmd) > 0)
                {
                    ErrMsg = "单据已经存在入库记录,请重新扫描查看";
                    throw new Exception(ErrMsg);
                }
                //
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cDefine26=" + dtBody.Rows[i]["合格数"] + ",cDefine27=" + dtBody.Rows[i]["不合格数"] + " where autoid=" + dtBody.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();
                iupdaterows++;
            }
            //审核到货单
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set cverifier='" + username + "',cdefine3='" + username +
                "',cAuditDate=convert(varchar(10),getdate(),120),caudittime=getdate(),iverifystateex=2 where id=0" + c_id;
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return "" + iupdaterows;
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

    [WebMethod]  //U8 核对仓管员协议 [HT]
    public bool U8SCM_RD11_CheckPsn(string cinv,string cven, string dbname, string user_id, string cdate)
    {
        return true;


//        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
//        if (Conn == null)
//        {
//            throw new Exception("数据库连接失败！");
//        }

//        //if(GetDataInt("select cast(isnull(cValue,'0') as int) from " + dbname + "..T_Parameter where cPid='targetPsnQX'", Conn)!=1)
//        //    return true;

//        string constr = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='VenConnectString'", Conn);  //连接字符串
//        if (constr == "") throw new Exception("没有设置价格协议数据库连接参数");
//        System.Data.SqlClient.SqlConnection sqlC = new System.Data.SqlClient.SqlConnection();
//        sqlC.ConnectionString = constr;
//        try
//        {
//            sqlC.Open();
//            if (GetDataInt(@"select count(*) from TG_ProviderAgreement a inner join TG_ProviderAgreement_Goods b on a.编号=b.编号
//                where a.供应商编码='" + cven + "' and b.货品编码='" + cinv + "' and b.仓库管理员编码='" + user_id +
//                "' and 有效期限起<='" + cdate + "' and 有效期限止>='" + cdate + "'", sqlC) == 0)
//            {
//                return false;
//            }
//            return true;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//        finally
//        {
//            if (Conn.State == System.Data.ConnectionState.Open) Conn.Close();
//            if(sqlC.State==System.Data.ConnectionState.Open)  sqlC.Close();
//        }
    }

    [WebMethod]  //U8 保存 采购入库单  返回 “采购入库单ID,单据号码” [HT][MeiXin]
    public string U8SCM_RD01(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string SN, ref string errmsg)
    {
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            errmsg = "序列号验证错误！";
            throw new Exception(errmsg);
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            errmsg = "数据库连接失败！";
            throw new Exception(errmsg);
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        //if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;

        try
        {
            string cc_code = U8SCM_Record01_Input(dtHead, dtBody, dbname, SN, Cmd, ref errmsg);
            tr.Commit();
            return cc_code;
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

    private string U8SCM_Record01_Input(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string SN,System.Data.SqlClient.SqlCommand Cmd, ref string errmsg)
    {
        errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
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
        }//arrid


        if (targetPTRdCode == "") {errmsg = "没有配置采购入库类别编码"; throw new Exception(errmsg); }
        if (targetPTCode == "") {errmsg = "没有配置采购类别编码"; throw new Exception(errmsg); }
        
        if (bHW)
        {
            cPosCode = "" + dtHead.Rows[0]["货位编码"];
            if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + cPosCode + "'", Cmd) == 0)
            { throw new Exception("库位编码【" + cPosCode + "】不存在"); }
        }
       

        int iRKRowCount = 0;
        KK_U8Com.U8Rdrecord01 record01 = new KK_U8Com.U8Rdrecord01(Cmd, dbname);
        #region  //新增主表
        if (rd_id == 0) 
        {
            rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'",Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            string cCodeHead = "R" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord01 where ccode like '" + cCodeHead + "%'", Cmd);
            record01.cCode = "'" + cc_mcode + "'";
            record01.ID = rd_id;
            record01.cVouchType = "'01'";
            record01.cBusType = "'" + cbustype + "'";
            record01.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
            record01.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
            record01.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
            record01.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            record01.cDefine2 = "'" + dtHead.Rows[0]["销售订单"] + "'";

            //record01.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
            //record01.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

            record01.cVenCode = "'" + dtHead.Rows[0]["供应商编码"] + "'";
            if (("" + dtHead.Rows[0]["入库类别"]).CompareTo("") != 0) record01.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            record01.cPTCode = "'" + targetPTCode + "'";

            //汇率  和  币种
            if (dtBody.Rows.Count == 0) throw new Exception("表体无记录");
            if (dtBody.Rows[0]["订单号"] + "" != "")
            {
                record01.iExchRate = float.Parse(GetDataString("select nflat from " + dbname + "..po_pomain where cPOID='" + dtBody.Rows[0]["订单号"] + "'", Cmd));
                record01.cExch_Name = "'" + GetDataString("select cexch_name from " + dbname + "..po_pomain where cPOID='" + dtBody.Rows[0]["订单号"] + "'", Cmd) + "'";
            }
            else
            {
                record01.iExchRate = 1;
                record01.cExch_Name = "'人民币'";
            }

            record01.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
            record01.iTaxRate = 13;
            record01.cSource = "'" + dtHead.Rows[0]["来源"] + "'";
            if (!record01.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            Cmd.CommandText = "update " + dbname + "..Rdrecord01 set cARVCode='" + dtHead.Rows[0]["备注"] + "' where id=" + rd_id;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        for (int i = 0; i < dtBody.Rows.Count; i++)
        {
            if (float.Parse("" + dtBody.Rows[i]["入库数"]) == 0) continue;
            iRKRowCount++;
            KK_U8Com.U8Rdrecords01 records01 = new KK_U8Com.U8Rdrecords01(Cmd, dbname);
            int cAutoid = 1000000000 + int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            records01.AutoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();

            records01.ID = rd_id;
            records01.cInvCode = "'" + dtBody.Rows[i]["货品编码"] + "'";
            records01.iQuantity = "" + dtBody.Rows[i]["入库数"];
            records01.cPosition = "'" + cPosCode + "'";
            records01.iMoney = "0";
            if (bPosWare)
            {
                records01.bCosting = 0;
                records01.bVMIUsed = "1";
                records01.cvmivencode = "'" + dtHead.Rows[0]["供应商编码"] + "'";
            }
            //批号
            if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and bInvBatch=1", Cmd) > 0)
            {
                records01.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
            }

            //records01.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
            //records01.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
            records01.irowno = (i + 1);
            records01.iTaxRate = float.Parse("" + dtBody.Rows[i]["税率"]);
            records01.ioriSum = ("" + dtBody.Rows[i]["含税单价"] == "" ? "null" : "" + (float.Parse("" + dtBody.Rows[i]["入库数"]) * float.Parse("" + dtBody.Rows[i]["含税单价"])));

            //上游单据关联
            records01.iNQuantity = records01.iQuantity;
            //关联订单 到货单
            
            if ("" + dtBody.Rows[i]["arrautoid"] != "0") records01.iArrsId = "" + dtBody.Rows[i]["arrautoid"];

            //自由项1  自由项2 的处理
            string c_free1 = "" + GetDataString("select cfree1 from " + dbname + "..PU_ArrivalVouchs where autoid =0" + dtBody.Rows[i]["arrautoid"], Cmd);
            string c_free2 = "" + GetDataString("select cfree2 from " + dbname + "..PU_ArrivalVouchs where autoid =0" + dtBody.Rows[i]["arrautoid"], Cmd);
            records01.cFree1 = (c_free1 == "" ? "null" : "'" + c_free1 + "'");
            records01.cFree2 = (c_free2 == "" ? "null" : "'" + c_free2 + "'");
            //自由项2
            if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records01.cInvCode + " and bFree2=1", Cmd) > 0)
            {
                if (records01.cFree2.CompareTo("null") == 0)
                {
                    records01.cFree2 = "'" + GetDataString("select cvalue from " + dbname + "..userdefine where cid='21' and calias=" + record01.cVenCode, Cmd) + "'";
                }
            }

            //自定义项处理 扣点处理  
            if (dtBody.Columns.Contains("扣点数量"))
            {
                records01.cDefine22 = "'" + dtBody.Rows[i]["扣点原因"] + "'";
                records01.cDefine34 = int.Parse("" + dtBody.Rows[i]["扣点数量"]);
            }

            if (cbustype.CompareTo("委外加工") == 0)
            {
                //订单子表ID
                string cposid = "" + dtBody.Rows[i]["poautoid"];
                //检查是否关闭
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..OM_MODetails where MODetailsID=0" + cposid + " and isnull(cbCloser,'')<>''", Cmd) > 0)
                    throw new Exception("订单已经关闭，不能入库");
                records01.iOMoDID = cposid;
                records01.cPOID = "'" + U8Operation.GetDataString("select b.cCode from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMain b on a.MOID=b.MOID 
                    where MODetailsID=0" + cposid, Cmd) + "'";
            }
            else
            {
                if ("" + dtBody.Rows[i]["poautoid"] != "0") records01.iPOsID = "" + dtBody.Rows[i]["poautoid"];
                records01.cPOID = "'" + U8Operation.GetDataString("select b.cPOID from " + dbname + "..PO_Podetails a inner join " + dbname + @"..PO_Pomain b on a.POID=b.POID 
                    where a.ID=0" + records01.iPOsID, Cmd) + "'";
            }

            //保存数据
            if (!records01.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            //回写到货单累计入库数
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set fValidInQuan=isnull(fValidInQuan,0)+" + records01.iQuantity + " where autoid =0" + dtBody.Rows[i]["arrautoid"];
            Cmd.ExecuteNonQuery();


            //回写采购订单累计入库数
            if (cbustype.CompareTo("委外加工") == 0)
            {
                Cmd.CommandText = "update " + dbname + "..OM_MODetails set freceivedqty=isnull(freceivedqty,0)+" + records01.iQuantity + " where MODetailsID =0" + dtBody.Rows[i]["poautoid"];
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set freceivedqty=isnull(freceivedqty,0)+" + records01.iQuantity + " where id =0" + dtBody.Rows[i]["poautoid"];
                Cmd.ExecuteNonQuery();
            }

            if (dtBody.Rows[i]["到货日期"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbarvcode='" + dtBody.Rows[i]["到货单号"] +
                    "',cDefine28='" + dtBody.Rows[i]["到货单号"] + "',dbarvdate='" + dtBody.Rows[i]["到货日期"] + "' where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            #region//是否超到货单检查
            if ("" + dtBody.Rows[i]["arrautoid"] != "0")
            {
                float fAllarr = 0;
                if (GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetNotOperation'", Cmd) + "" == "")
                {
                    //参数为空，代表HengT
                    fAllarr = float.Parse(GetDataString("select isnull(sum(isnull(cDefine26,0)),0) from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname +
                        "..PU_ArrivalVouch b on a.id=b.id where a.Autoid=" + dtBody.Rows[i]["arrautoid"] + " and isnull(b.cverifier,'')<>''", Cmd));
                }
                else
                {
                    fAllarr = float.Parse(GetDataString("select isnull(sum(isnull(iQuantity,0)),0) from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname +
                        "..PU_ArrivalVouch b on a.id=b.id where a.Autoid=" + dtBody.Rows[i]["arrautoid"] + " and isnull(b.cverifier,'')<>''", Cmd));
                }
                float fAllRk = float.Parse(GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..rdrecords01 where iArrsId=" + dtBody.Rows[i]["arrautoid"], Cmd));
                if (fAllRk > fAllarr)
                {
                    errmsg = "错误." + records01.cInvCode + "超到货入库";
                    throw new Exception(errmsg);
                }
            }
            #endregion

            #region//判断是否货位管理
            if (bHW)
            {
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cfree1,cfree2) " +
                    "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + cPosCode + "','" + dtBody.Rows[i]["货品编码"] + "',0" + dtBody.Rows[i]["入库数"] +
                    ",null,'" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'01','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] +
                    "'," + (c_free1 == "" ? "null" : "'" + c_free1 + "'") + "," + (c_free2 == "" ? "null" : "'" + c_free2 + "'") + ")";
                Cmd.ExecuteNonQuery();

                //指定货位
                Cmd.CommandText = "update " + dbname + "..rdrecords01 set iposflag=1 where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();

                //修改货位库存
                if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and cPosCode='" + cPosCode + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                    cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                    values('" + dtHead.Rows[0]["仓库编码"] + "','" + cPosCode + "','" + dtBody.Rows[i]["货品编码"] + @"',0,'',
                    '','','','','','','','','','','','',0,0)";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["入库数"] + 
                    ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["货品编码"] +
                    "' and cPosCode='" + cPosCode + "' and cfree1='" + c_free1 + "' and cfree2='" + c_free2 + "'";
                Cmd.ExecuteNonQuery();
            }
            #endregion

        }
        if (iRKRowCount == 0) throw new Exception("没有可以保存的入库单行");


        #region  //委外倒冲材料出库
        if (cbustype.CompareTo("委外加工") == 0)
        {
            System.Data.DataTable dtWare11 = null;

            dtWare11 = U8Operation.GetSqlDataTable("select distinct cWhCode WhCode from " + dbname + @"..OM_MOMaterials 
                        where MoDetailsID in(select iOMoDID from " + dbname + @"..rdrecords01 where id=" + rd_id + ") and iWIPtype=1", "dtWare11", Cmd);

            string cOutRdCode = "" + U8Operation.GetDataString("select cVRSCode from " + dbname + @"..VouchRdContrapose where cVBTID='1107'", Cmd);

            if (dtWare11.Rows.Count > 0)
            {
                if (cOutRdCode.CompareTo("") == 0) throw new Exception("倒冲出库需要 出库类别");
                //存货小数位数
                string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);
                for (int r = 0; r < dtWare11.Rows.Count; r++)
                {
                    if (dtWare11.Rows[r]["WhCode"] + "" == "") throw new Exception("倒冲材料出库仓库不能为空");
                    System.Data.DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + cOutRdCode + "' crdcode,'" + dtWare11.Rows[r]["WhCode"] + @"' cwhcode,
                            " + record01.cDepCode + " cdepcode," + record01.cCode + " cbuscode,'委外倒冲' cbustype,'000' headoutposcode", "dtRdMain", Cmd);
                    System.Data.DataTable dtRddetail = null;

//                    dtRddetail = U8Operation.GetSqlDataTable(@"select b.MOMaterialsID allocateid,b.cinvcode,'' cbvencode,'' cbatch,round(sum(rd.iquantity)*b.iQuantity/a.iQuantity," + cInv_DecDgt + @") iquantity,b.MoDetailsID modid,'' cposcode
//                                from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMaterials b on a.MODetailsID=b.MODetailsID 
//                                inner join " + dbname + @"..rdrecords01 rd on b.MoDetailsID=rd.iOMoDID
//                                inner join " + dbname + @"..rdrecord01 m on rd.id=m.id彭
//                                where rd.id=" + rd_id + " and m.cWhCode='" + dtWare11.Rows[r]["WhCode"] + @"' and b.iWIPtype=1
//                                group by b.MOMaterialsID,b.cinvcode,b.MoDetailsID,b.iQuantity,a.iQuantity", "dtRddetail", Cmd);
                    //  2022-10-25 彭，去掉入库仓库和委外采购订单仓库的一致性检查。
                    dtRddetail = U8Operation.GetSqlDataTable(@"select b.MOMaterialsID allocateid,b.cinvcode,'' cbvencode,'' cbatch,round(sum(rd.iquantity)*b.iQuantity/a.iQuantity," + cInv_DecDgt + @") iquantity,b.MoDetailsID modid,'' cposcode
                                from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMaterials b on a.MODetailsID=b.MODetailsID 
                                inner join " + dbname + @"..rdrecords01 rd on b.MoDetailsID=rd.iOMoDID
                                inner join " + dbname + @"..rdrecord01 m on rd.id=m.id
                                where rd.id=" + rd_id +" and b.cWhCode='" + dtWare11.Rows[r]["WhCode"] + @"' and b.iWIPtype=1
                                group by b.MOMaterialsID,b.cinvcode,b.MoDetailsID,b.iQuantity,a.iQuantity", "dtRddetail", Cmd);
                    if (dtRddetail.Rows.Count == 0) throw new Exception("无法找倒冲出库数据");

                    U8StandSCMBarCode u8rd11 = new U8StandSCMBarCode();
                    System.Data.DataTable SHeadData = u8rd11.GetDtToHeadData(dtRdMain, 0);
                    SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                    u8rd11.U81016(SHeadData, dtRddetail, dbname, dtHead.Rows[0]["制单人"] + "", dtHead.Rows[0]["制单日期"] + "", "U81016", Cmd);
                }
            }
        }
        #endregion

        
        return rd_id + "," + cc_mcode;
    
    
        
    }

    [WebMethod]  //U8 保存 材料出库单  返回 “材料出库单ID,单据号码”[HT]
    public string U8SCM_RD11_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string SN, ref string errmsg)
    {
        errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            errmsg = "序列号验证错误！";
            throw new Exception(errmsg);
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            errmsg = "数据库连接失败！";
            throw new Exception(errmsg);
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);
            if (rd_id == 0)  //新增主表
            {
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

                //record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record11.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record11.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record11.iExchRate = "1";
                record11.cExch_Name = "'人民币'";

                record11.cSource = "'库存'";
                if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'", Cmd) == 0)
                { throw new Exception("库位编码【" + dtBody.Rows[i]["货位"] + "】不存在"); }

                KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

                int cAutoid = 1000000000 + int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records11.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records11.ID = rd_id;
                records11.cInvCode = "'" + dtBody.Rows[i]["货品编码"] + "'";
                records11.iQuantity = "" + dtBody.Rows[i]["数量"];
                records11.cPosition = "'" + dtBody.Rows[i]["货位"] + "'";
                //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records11.irowno = (i + 1);
                records11.iNQuantity = records11.iQuantity;

                //保存数据
                if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                //添加货位记录
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                    "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["货位"] + "','" + dtBody.Rows[i]["货品编码"] + "',0" + dtBody.Rows[i]["数量"] +
                    ",null,'" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'11','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                Cmd.ExecuteNonQuery();
                //指定货位
                Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();

                //修改货位库存
                if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["货位"] + "','" + dtBody.Rows[i]["货品编码"] + @"',0,'',
                        '','','','','','','','','','','','',0,0)";
                    Cmd.ExecuteNonQuery();
                }

                Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] + 
                    "' and cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'";
                Cmd.ExecuteNonQuery();

                //判定负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["货品编码"] +
                    "' and cPosCode='" + dtBody.Rows[i]["货位"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("【" + dtBody.Rows[i]["货品编码"] + "】出现负库存");
                }


            }
            tr.Commit();
            
            return rd_id + "," + cc_mcode;
        }
        catch(Exception ex)
        {
            tr.Rollback();
            throw ex;
        }
        finally
        {
            errmsg=CloseDataConnection(Conn);
        }

        
    }

    [WebMethod]  //U8 保存 材料出库单 批量【  返回 “材料出库单ID,单据号码”】,材料出库单子表有项目[HT]
    public string U8SCM_RD11_MultSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody,System.Data.DataTable dtItem, string dbname, string SN, ref string errmsg)
    {
        errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";
        //加密验证
        if (SN + "" != "" + Application.Get("cSn"))
        {
            errmsg = "序列号验证错误！";
            throw new Exception(errmsg);
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            errmsg = "数据库连接失败！";
            throw new Exception(errmsg);
        }

        if (dtBody.Rows.Count == 0) { throw new Exception("表体单据为空"); }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        if (dCurDate.CompareTo("" + dtHead.Rows[0]["制单日期"]) != 0) throw new Exception("当前登录日期与服务器不一致，请从新登录");

        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);

            bool bred = false;
            if (rd_id == 0)  //新增主表
            {
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

                if (float.Parse("" + dtBody.Rows[0]["数量"]) >= 0)
                {
                    record11.bredvouch = "0";
                }
                else
                {
                    record11.bredvouch = "1";
                    bred = true;
                }
                //record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record11.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record11.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record11.iExchRate = "1";
                record11.cExch_Name = "'人民币'";

                record11.cSource = "'库存'";
                if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (float.Parse("" + dtBody.Rows[i]["数量"]) == 0) continue; //数量为0 退出
                if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'", Cmd) == 0)
                { throw new Exception("库位编码【" + dtBody.Rows[i]["货位"] + "】不存在"); }

                int iItemCount = dtItem.Rows.Count;
                double finvall = 0;

                string ccost = "0";
                for (int cc = 0; cc < dtBody.Columns.Count; cc++)
                {
                    if (dtBody.Columns[cc].ColumnName.CompareTo("单价") == 0) ccost = dtBody.Rows[i]["单价"] + "";
                }

                if (ccost == "") ccost = "0";
                if (float.Parse(ccost) < 0) throw new Exception("单价必须大于0");

                for (int j = 0; j < dtItem.Rows.Count; j++)
                {
                    KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);
                    int cAutoid = 1000000000 + int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                    records11.AutoID = cAutoid;
                    Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                    Cmd.ExecuteNonQuery();

                    records11.ID = rd_id;
                    records11.cInvCode = "'" + dtBody.Rows[i]["货品编码"] + "'";

                    //尾差处理
                    finvall += Math.Round(float.Parse("" + dtBody.Rows[i]["数量"]) / iItemCount, 4);
                    if (j == dtItem.Rows.Count - 1)
                    {
                        records11.iQuantity = "" +
                            (Math.Round(float.Parse("" + dtBody.Rows[i]["数量"]) / iItemCount, 4) + (double.Parse("" + dtBody.Rows[i]["数量"]) - finvall));
                    }
                    else
                    {
                        records11.iQuantity = "" + Math.Round(float.Parse("" + dtBody.Rows[i]["数量"]) / iItemCount, 4);
                    }
                    string cUpdateQty = records11.iQuantity;
                    
                    if (float.Parse(ccost) != 0)
                    {
                        records11.iPrice = "" + Math.Round(float.Parse(ccost) * float.Parse(records11.iQuantity), 2);
                    }

                    records11.cPosition = "'" + dtBody.Rows[i]["货位"] + "'";
                    records11.cItem_class = "'" + dtItem.Rows[j]["大类编码"] + "'";
                    records11.cItemCName = "'" + dtItem.Rows[j]["大类名称"] + "'";
                    records11.cItemCode = "'" + dtItem.Rows[j]["项目编码"] + "'";
                    records11.cName = "'" + dtItem.Rows[j]["项目名称"] + "'";
                    //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                    //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                    records11.irowno = (i + 1);
                    //records11.iNQuantity = records11.iQuantity;

                    //保存数据
                    if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                    //添加货位记录
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["货位"] + "','" + dtBody.Rows[i]["货品编码"] + "',0" + cUpdateQty +
                        ",null,'" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'11','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                    Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["货位"] + "','" + dtBody.Rows[i]["货品编码"] + @"',0,'',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }

                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + cUpdateQty + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] +
                        "' and cinvcode='" + dtBody.Rows[i]["货品编码"] + "' and cPosCode='" + dtBody.Rows[i]["货位"] + "'";
                    Cmd.ExecuteNonQuery();

                    //判定负库存问题
                    if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] +"' and cinvcode='" + dtBody.Rows[i]["货品编码"] +
                        "' and cPosCode='" + dtBody.Rows[i]["货位"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                    {
                        throw new Exception("【" + dtBody.Rows[i]["货品编码"] + "】出现负库存");
                    }

                    //指定货位
                    Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
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
            CloseDataConnection(Conn);
        }

    }

    [WebMethod]  //U8 保存 产品入库单 批量【  返回 “单据ID,单据号码”】,产品入库单子表有项目[HL]
    public string U8SCM_RD10_Save(System.Data.DataTable dtHead, System.Data.DataTable dtBoxs, string dbname, string SN)
    {
        string errmsg = "";
        //按照流转卡 的生产订单号来存入   表体自定义项 存箱码
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
            string rd_code = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetLLY10'", Cmd); //材料领用类别
            if (rd_code == "") { throw new Exception("没有配置材料领用出库类别"); }
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            bool bPos = false;  //不参与货位管理
            string c_position = "" + dtHead.Rows[0]["货位编码"];

            KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
            if (rd_id == 0)  //新增主表
            {
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "G" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord10 where ccode like '" + cCodeHead + "%'", Cmd);
                record10.cCode = "'" + cc_mcode + "'";
                record10.ID = rd_id;
                record10.cVouchType = "'10'";
                record10.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record10.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record10.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                record10.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record10.cRdCode = "'" + rd_code + "'";
                record10.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record10.iExchRate = "1";
                record10.cExch_Name = "'人民币'";
                record10.cSource = "'生产订单'";

                if (GetDataInt("select count(*) from " + dbname + "..department where cdepcode=" + record10.cDepCode, Cmd) == 0)
                    throw new Exception("部门" + record10.cDepCode + "不存在");
                //理论部门
                if (dtHead.Columns.Contains("理论部门"))
                {
                    record10.cDefine11 = "'" + dtHead.Rows[0]["理论部门"] + "'";
                }

                if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
                
                //货位管理判定
                string ihwcount = GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and bWhPos=1", Cmd);
                if (int.Parse(ihwcount) > 0)
                {
                    bPos=true ;
                    //判定货位编码
                    if (c_position.CompareTo("") != 0)
                    {
                        if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtHead.Rows[0]["货位编码"] + "' and bPosEnd=1", Cmd) == 0)
                        {
                            throw new Exception("货位编码不存在，或不是本仓库货位，或非末级货位");
                        }
                    }
                }
            }
            //
            int irowno = 0;
            for (int k = 0; k < dtBoxs.Rows.Count; k++)  //箱码清单
            {
                //判断本箱是否已经入库
                //if (GetDataInt("select isnull(cc_rd10autoid,0) from " + dbname + "..T_CC_HL_TuoBox where cc_box_code='" + dtBoxs.Rows[k]["装箱箱码"] + "'", Cmd) > 0)
                //{ throw new Exception("箱码【" + dtBoxs.Rows[k]["装箱箱码"] + "】已经入库，不能重复入库"); }
                if (GetDataInt("select isnull(cc_rd10autoid,0) from " + dbname + "..T_CC_HL_BoxList where cc_boxcode='" + dtBoxs.Rows[k]["装箱箱码"] + @"' 
                    and cc_crkbatch='" + dtBoxs.Rows[k]["入库批号"] + "'", Cmd) > 0)
                { throw new Exception("箱码【" + dtBoxs.Rows[k]["装箱箱码"] + "】入库批号【" + dtBoxs.Rows[k]["入库批号"] + "】已经入库，不能重复入库"); }

                KK_U8Com.U8Rdrecords10 records10 = new KK_U8Com.U8Rdrecords10(Cmd, dbname);
                System.Data.DataTable dtData = GetSqlDataTable(@"select c.mocode,c.moid,b.modid,sortseq,a.Define3 molotcode,b.invcode,b.free1,b.free2,
                        b.mdeptcode,b.whcode,sum(d.cc_iqty) iqty,d.cc_boxcode,a.define14,a.doccode,a.Define10 cLuhao,a.Define2 cLuhao_2,m.Description momtype
                    from " + dbname + "..sfc_processflow a inner join " + dbname + @"..mom_orderdetail b on a.modid=b.modid
                    inner join " + dbname + @"..mom_order c on b.moid=c.moid
                    inner join " + dbname + @"..T_CC_HL_BoxList d on a.pfid=d.cc_pfid
                    left join " + dbname + @"..mom_motype m on b.MoTypeId=m.MoTypeId
                    where d.cc_boxcode='" + dtBoxs.Rows[k]["装箱箱码"] + @"' and d.cc_crkbatch='" + dtBoxs.Rows[k]["入库批号"] + @"'
                    group by c.mocode,c.moid,b.modid,sortseq,a.Define3,b.invcode,b.free1,b.free2,b.mdeptcode,b.whcode,d.cc_boxcode,a.define14,a.doccode,a.Define10,a.Define2,m.Description", "dtData", Cmd);
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    string cmotype = dtData.Rows[i]["momtype"] + "";
                    if (cmotype.Length > 4)
                    {
                        if (cmotype.Substring(0, 4) == "A025" && dtHead.Rows[0]["仓库编码"] + "" == "A024") throw new Exception("退库返检生产订单不能入仓库[A024]");
                    }
                    int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                    records10.AutoID = cAutoid;
                    Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                    Cmd.ExecuteNonQuery();

                    records10.ID = rd_id;
                    records10.cInvCode = "'" + dtData.Rows[i]["invcode"] + "'";
                    records10.iQuantity = "" + dtData.Rows[i]["iqty"];
                    records10.irowno = (irowno++);
                    records10.iNQuantity = records10.iQuantity;
                    records10.iordertype = "0";
                    //生产订单信息
                    System.Data.DataTable dtfclist = GetSqlDataTable("select top 1 a.opseq,a.description,b.WcCode from " + dbname + "..sfc_moroutingdetail a inner join " + 
                        dbname + "..sfc_workcenter b on a.wcid=b.wcid where modid=0" +
                        dtData.Rows[i]["modid"] + " order by opseq desc", "dtfclist", Cmd);
                    records10.iMPoIds = "" + dtData.Rows[i]["modid"];
                    records10.imoseq = "" + dtData.Rows[i]["sortseq"];
                    records10.cMoLotCode = "'" + dtData.Rows[i]["molotcode"] + "'";
                    records10.cmocode = "'" + dtData.Rows[i]["mocode"] + "'";
                    if (dtfclist.Rows.Count > 0)
                    {
                        records10.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
                        records10.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
                        records10.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
                    }

                    string cToday = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);

                    records10.cBatch = "'" + dtBoxs.Rows[k]["入库批号"] + "'";
                    records10.cFree1 = (dtData.Rows[i]["free1"] + "" == "" ? "null" : "'" + dtData.Rows[i]["free1"] + "'");
                    records10.cFree2 = (dtData.Rows[i]["free2"] + "" == "" ? "null" : "'" + dtData.Rows[i]["free2"] + "'");
                    records10.cDefine22 = "'" + dtData.Rows[i]["cc_boxcode"] + "'";  //装箱箱码
                    records10.cDefine23 = "'" + dtBoxs.Rows[k]["托盘条码"] + "'";
                    records10.cDefine24 = "'" + dtData.Rows[i]["doccode"] + "'";  //流转卡号
                    records10.cDefine25 = "'" + dtData.Rows[i]["define14"] + "'";  //流转卡部门

                    //生产日期 保质期
                    System.Data.DataTable dtBZQ = GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + cToday + @"' 生产日期,
                                                                        convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + cToday + @"')
					                                                                        when cMassUnit=2 then DATEADD(month,iMassDate,'" + cToday + @"')
					                                                                        else DATEADD(day,iMassDate,'" + cToday + @"') end)
                                                                        ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                                                                        from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                                                                        where cinvcode='" + dtData.Rows[i]["invcode"] + "' and bInvQuality=1", "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count > 0)
                    {
                        records10.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                        records10.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                        records10.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                        records10.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                        records10.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                        records10.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                    }

                    //生产批号 和 产品入库批号规则：生产批号+入库日期【6位】
//                    //string cbatchno = dtData.Rows[i]["molotcode"] + "-" + cToday.Replace("-", "").Substring(2);
//                    //编写批次档案
//                    if (GetDataInt("select count(*) cn from " + dbname + "..AA_BatchProperty where cbatch =" + records10.cBatch + " and cinvcode=" + records10.cInvCode + @" 
//                        and cfree1=" + records10.cFree1 + " and cfree2=" + records10.cFree2, Cmd) == 0)
//                    {
//                        string cLuhao = "" + dtData.Rows[i]["cLuhao"]; string cLuhao_2 = "" + dtData.Rows[i]["cLuhao_2"];//找流转卡信息
//                        Cmd.CommandText = @"insert into AA_BatchProperty(cbatchpropertyguid,cinvcode,cbatch,cfree1,cfree2,cbatchproperty6,cbatchproperty7,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10) 
//                            values(newid()," + records10.cInvCode + "," + records10.cBatch + "," + records10.cFree1 + "," + records10.cFree2 + ",'" + cLuhao + "','" + cLuhao_2 + "','','','','','','','','')";
//                        Cmd.ExecuteNonQuery();
//                    }

                    //货位判定
                    if (bPos && c_position.CompareTo("") != 0)
                    {
                        records10.cPosition = "'" + dtHead.Rows[0]["货位编码"] + "'";
                    }

                    //保存数据
                    if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                    if (bPos && c_position.CompareTo("") != 0)  //货位台账处理
                    {
                        #region
                        //添加货位记录 
                        Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                            cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,
                            iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate) " +
                            "Values (" + records10.AutoID + "," + records10.ID + "," + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + ",0" + records10.iQuantity +
                            ",null," + records10.cBatch + "," + record10.dDate + ",1,'',0,'10'," + record10.dDate + "," + record10.cMaker + @",
                            " + records10.iMassDate + "," + records10.cMassUnit + "," + records10.dMadeDate + "," + records10.dVDate + "," + records10.cExpirationdate + "," + records10.iExpiratDateCalcu + "," + records10.dVDate + ")";
                        Cmd.ExecuteNonQuery();

                        //指定货位
                        Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1 where autoid =0" + records10.AutoID;
                        Cmd.ExecuteNonQuery();
                        
                        //修改货位库存
                        if (int.Parse(KK_U8Com.U8Common.GetStringFromSql("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + record10.cWhCode + " and cinvcode=" +
                            records10.cInvCode + " and cbatch=isnull(" + records10.cBatch + ",'') and cPosCode=" + records10.cPosition + "",Cmd)) == 0)
                        {
                            Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                                cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                                iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate) 
                                values(" + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + ",0,isnull(" + records10.cBatch + @",''),
                                '','','','','','','','','','','','',0,
                                " + records10.iMassDate + "," + records10.cMassUnit + "," + records10.dMadeDate + "," + records10.dVDate + "," + records10.cExpirationdate + "," + records10.iExpiratDateCalcu + "," + records10.dVDate + ")";
                            Cmd.ExecuteNonQuery();
                        }
                        //修改货位账
                        Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + records10.iQuantity + @") where 
                            cwhcode=" + record10.cWhCode + " and cinvcode=" + records10.cInvCode + " and cbatch=isnull(" + records10.cBatch + ",'') and cPosCode=" + records10.cPosition;
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }

                    //记录箱 入库单记录
                    //Cmd.CommandText = "update " + dbname + "..T_CC_HL_TuoBox set cc_rd10autoid=0" + cAutoid + " where cc_box_code='" + dtData.Rows[i]["cc_boxcode"] + "'";
                    //Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..T_CC_HL_BoxList set cc_rd10autoid=0" + cAutoid + @" 
                        where cc_boxcode='" + dtData.Rows[i]["cc_boxcode"] + "' and cc_crkbatch='" + dtBoxs.Rows[k]["入库批号"] + "' and cc_pfcode='" + dtData.Rows[i]["doccode"] + "'";
                    Cmd.ExecuteNonQuery();

                    //回写生产订单已经入库数量
                    Cmd.CommandText = "update " + dbname + "..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + dtData.Rows[i]["iqty"] + @") where modid=0" + dtData.Rows[i]["modid"] + "";
                    Cmd.ExecuteNonQuery();
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

        

        return "";
    }

    [WebMethod]  //U8 审核产品入库单 [HL]
    public bool U8SCM_RD10_Check(string id,string cposcode,string dbname,string acc_id,string username,string cdate, string SN)
    {
        //按照流转卡 的生产订单号来存入   表体自定义项 存箱码
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
            #region  //检查是输入货位
            bool bCHecked = false;
            if (GetDataString("select cValue from " + dbname + "..AccInformation where csysid='st' and cName='bProductInCheck'", Cmd).ToLower() == "true") bCHecked = true;
            string cwhcode = GetDataString("select cwhcode from " + dbname + "..rdrecord10 where id=0" + id, Cmd);
            int ipos = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + cwhcode + "' and bWhPos=1", Cmd);
            int iposflag = GetDataInt("select count(*) from " + dbname + "..rdrecords10 where id=0" + id + " and isnull(iposflag,0)<>1", Cmd);
            if (ipos > 0 && iposflag > 0)
            {
                cposcode = GetDataString("select cPosCode from " + dbname + "..Position where cwhcode='" + cwhcode + "' and (cPosCode='" + cposcode + "' or cBarCode='" + cposcode + "') and bPosEnd=1", Cmd);
                if (cposcode == "")
                {
                    throw new Exception("货位编码不存在，或不是本仓库货位，或非末级货位");
                }
                //判断是否部分指定了货位
                if (GetDataInt("select count(*) from " + dbname + "..rdrecords10 where id=0" + id + " and isnull(iposflag,0)=1", Cmd) > 0)
                {
                    throw new Exception("本单据部分行已经指定货位，不能重复指定");
                }

                //货位台账
                System.Data.DataTable dtBody = GetSqlDataTable(@"select b.autoid,b.cinvcode,b.iquantity,b.cbatch,b.cfree1,b.cfree2,b.cfree3
                    from " + dbname + "..rdrecord10 a inner join " + dbname + "..rdrecords10 b on a.id=b.id  where a.id=0" + id + " and isnull(b.iposflag,0)<>1", "dtBody", Cmd);
                for (int i = 0; i < dtBody.Rows.Count; i++)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                            cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,
                            iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate,
                            cfree1,cfree2,cfree3) 
                        select b.autoid,b.id,cwhcode,'" + cposcode + @"' cposcode,cinvcode,iquantity,
                            cmemo,cbatch,ddate,brdflag,'' csource,0 itrackid,cvouchtype,a.ddate,cHandler,
                            iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate,
                            b.cfree1,b.cfree2,b.cfree3
                        from " + dbname + "..rdrecord10 a inner join " + dbname + "..rdrecords10 b on a.id=b.id  where b.autoid=0" + dtBody.Rows[i]["autoid"];
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1,cposition='" + cposcode + "' where autoid =0" + dtBody.Rows[i]["autoid"];
                    Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (int.Parse(KK_U8Com.U8Common.GetStringFromSql("select count(*) from " + dbname + @"..InvPositionSum 
                        where cwhcode='" + cwhcode + "' and cinvcode='" + dtBody.Rows[i]["cinvcode"] + "' and cbatch='" + dtBody.Rows[i]["cbatch"] + "' and cPosCode='" + cposcode + @"'
                            and cfree1='" + dtBody.Rows[i]["cfree1"] + "' and cfree2='" + dtBody.Rows[i]["cfree2"] + "' and cfree3='" + dtBody.Rows[i]["cfree3"] + "'", Cmd)) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                                cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                                iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate) 
                            select cwhcode,'" + cposcode + @"' cposcode,cinvcode,0 iquantity,isnull(cbatch,''),
                                isnull(cfree1,''),isnull(cfree2,''),isnull(cfree3,''),isnull(cfree4,''),isnull(cfree5,''),isnull(cfree6,''),isnull(cfree7,''),
                                isnull(cfree8,''),isnull(cfree9,''),isnull(cfree10,''),isnull(cvmivencode,''),isnull(cinvouchtype,''),0 iTrackid,
                                iMassDate,cMassUnit,dMadeDate,dVDate,cExpirationdate,iexpiratdatecalcu,dExpirationdate
                            from " + dbname + "..rdrecord10 a inner join " + dbname + "..rdrecords10 b on a.id=b.id  where b.autoid=0" + dtBody.Rows[i]["autoid"];
                        Cmd.ExecuteNonQuery();
                    }
                    //修改货位账
                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["iquantity"] + @") 
                            where cwhcode='" + cwhcode + "' and cinvcode='" + dtBody.Rows[i]["cinvcode"] + "' and cbatch='" + dtBody.Rows[i]["cbatch"] + @"' 
                                and cPosCode='" + cposcode + "' and cfree1='" + dtBody.Rows[i]["cfree1"] + "' and cfree2='" + dtBody.Rows[i]["cfree2"] + @"' 
                                and cfree3='" + dtBody.Rows[i]["cfree3"] + "'";
                    Cmd.ExecuteNonQuery();

                    #region  处理现存量
                    if (bCHecked)
                    {
                        //审核修改现存量
                        Cmd.CommandText = "update " + dbname + @"..currentstock set iQuantity=iQuantity+(0" + dtBody.Rows[i]["iquantity"] + "),fInQuantity=fInQuantity-(0" + dtBody.Rows[i]["iquantity"] + @") 
                            where cwhcode='" + cwhcode + "' and cinvcode='" + dtBody.Rows[i]["cinvcode"] + "' and cbatch='" + dtBody.Rows[i]["cbatch"] + @"'
                                and cfree1='" + dtBody.Rows[i]["cfree1"] + "' and cfree2='" + dtBody.Rows[i]["cfree2"] + @"' and cfree3='" + dtBody.Rows[i]["cfree3"] + "'";
                        Cmd.ExecuteNonQuery();
                    }
                    #endregion
                }
            }
            #endregion  

            Cmd.CommandText = "update " + dbname + "..rdrecord10 set cHandler='" + username + "',dVeriDate='" +
                        cdate + "',dnverifytime=getdate(),iswfcontrolled=0 where id=0" + id;
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "delete from " + dbname + "..table_task WHERE caccountid='" + acc_id + "' AND cVoucherType = '0411' and cvoucherid='" + id + "'";
            Cmd.ExecuteNonQuery();

            tr.Commit();
            return true;
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

    [WebMethod]  //U8 流转卡  完工扫描按卡入库 [MeiXin]
    public string U8Mom_CardGoodIn_Save(string pfcode, System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string acc_id)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        string errmsg = "";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string cToday = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
            //获得 部门编码信息
            string ccDepcode = "" + GetDataString("select b.MDeptCode from " + dbname + "..sfc_processflow a inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where a.DocCode='" + pfcode + "'", Cmd);
            if (ccDepcode.CompareTo("") == 0)
            {
                throw new Exception("生产订单没有录入生产部门");
            }

            KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
            //主表
            int rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            string cCodeHead = "G" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd);
            string cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord10 where ccode like '" + cCodeHead + "%'", Cmd);
            record10.cCode = "'" + cc_mcode + "'";
            record10.ID = rd_id;
            record10.cVouchType = "'10'";
            record10.cBusType = "'成品入库'";
            record10.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
            record10.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
            record10.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            record10.cDepCode = "'" + ccDepcode + "'";
            record10.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            record10.cMemo = "''";
            record10.iExchRate = "1";
            record10.cExch_Name = "'人民币'";
            record10.cSource = "'生产订单'";
            record10.cDefine3 = "'" + pfcode + "'";
            if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            int irowno =0;
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
            records10.cDefine24 = "'" + pfcode + "'";

            //生产订单信息
            System.Data.DataTable dtfclist = GetSqlDataTable(@"select top 1 c.mocode,a.modid,b.SortSeq,MoLotCode,b.invcode,d.opseq,d.description,e.WcCode
                from " + dbname + "..sfc_processflow a inner join " + dbname + @"..mom_orderdetail b on a.modid=b.modid
                inner join " + dbname + @"..mom_order c on b.moid=c.moid
                left join " + dbname + @"..sfc_moroutingdetail d on b.modid=d.modid
                left join " + dbname + @"..sfc_workcenter e on d.wcid=e.wcid
                where a.doccode='" + pfcode + "' order by d.opseq desc", "dtfclist", Cmd);

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

            }
            else
            {
                throw new Exception("没有找到生产订单信息");
            }

            
            //保存数据
            if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


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



        return "";
    }

    [WebMethod]  //U8 扫描流转卡  按生产订单入库 [MeiXin]
    public string U8Mom_CardToOrderGoodIn_Save(string modid, System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string acc_id)
    {
        string pfcode = modid;
        modid = "";
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
            modid = GetDataString("select modid from " + dbname + "..sfc_processflow where doccode='" + pfcode + "'", Cmd) + "";
            if (modid == "")
            {
                modid = GetDataString("select modid from " + dbname + "..mom_orderdetail where modid='" + pfcode + "'", Cmd) + "";
                if (modid == "") throw new Exception("没有找到生产工单信息");
            }
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string cToday = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
            //获得 部门编码信息
            string ccDepcode = "" + GetDataString("select b.MDeptCode from " + dbname + "..mom_orderdetail b where b.modid='" + modid + "'", Cmd);
            if (ccDepcode.CompareTo("") == 0)
            {
                throw new Exception("生产订单没有录入生产部门");
            }

            if (GetDataInt("select count(*) cn from " + dbname + "..mom_orderdetail b where b.modid=" + modid + " and isnull(b.CloseUser,'')<>''", Cmd) > 0)
            {
                throw new Exception("生产订单已经关闭，不能入库");
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
            records10.cDefine25 = "'" + dtBody.Rows[0]["金相数"] + "'";
            if ((dtHead.Rows[0]["是否废品"] + "").CompareTo("是") == 0) records10.cDefine35 = 1;
            if (ibcostcount == 0) records10.bCosting = "0";

            //生产订单信息
            System.Data.DataTable dtfclist = GetSqlDataTable(@"select top 1 c.mocode,b.modid,b.SortSeq,MoLotCode,b.invcode,d.opseq,d.description,e.WcCode,c.define13
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
                    if ((dtBody.Rows[0]["入库批号"]+"").CompareTo("") == 0)
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
                    " and cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and isnull(cdefine35,0)=0", Cmd));
                //总废品输入数
                float f_qtyfpall = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + dbname + "..rdrecords10 where iMPoIds=0" + dtfclist.Rows[0]["modid"] +
                   " and cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and isnull(cdefine35,0)=1", Cmd));

                if (dtHead.Rows[0]["是否废品"].ToString().CompareTo("是") == 0)
                {
                    f_feipin_count = f_Qty_in;
                    records10.cDefine24 = "'废品入库'";
                }
                else
                {
                    string c_itemcode = "" + dtfclist.Rows[0]["define13"];
                    string item_class = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mx_itempro_num'", Cmd);
                    if (c_itemcode != "" && item_class != "")
                    {
                        string iclass_name = GetDataString("select citem_name from " + dbname + "..fitemclass where citem_class='" + item_class + "'", Cmd);
                        string icitem_name = GetDataString("select citemname from " + dbname + "..fitemss" + item_class + " where citemcode='" + c_itemcode + "'", Cmd);
                        records10.cItem_class = "'"+item_class+"'";
                        records10.cItemCName = "'" + iclass_name + "'";
                        records10.cItemCode = "'" + c_itemcode + "'";
                        records10.cName = "'" + icitem_name + "'";
                    }
                }

                //判定是否存在非倒冲零件
                int illrow = 0;
                //关键零用控制量 
                float f_ll_qty = 0;
                string ccc_inv=dtfclist.Rows[0]["invcode"]+"";
                //生产订单数量
                string i_c_mom_qty = GetDataString(@"select cast(qty as float) from " + dbname + "..mom_orderdetail where MoDId=0" + dtfclist.Rows[0]["modid"], Cmd);
                //关键件判定
                illrow = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart=1 and a.Qty<>0 ", Cmd));
                //不合格数取 关键件 齐套量
                f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(IssQty*" + i_c_mom_qty + "/Qty+0.49,0)),0) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart=1 and a.Qty<>0", Cmd));

                if (f_ll_qty < f_Qty_in + f_qtyinall + f_qtyfpall && illrow > 0)
                {
                    throw new Exception("不能超关键件领料入库");
                }

                string cc_rd_code = dtHead.Rows[0]["入库类别"] + "";
                //若未合格品的话，判定 所有配件
                if ((     (targetAccId == "201" && cc_rd_code.CompareTo("103") == 0) || 
                          (targetAccId == "202" && cc_rd_code.CompareTo("104") == 0) ||
                          (targetAccId == "206" && (cc_rd_code.CompareTo("102") == 0 || cc_rd_code.CompareTo("103") == 0 || cc_rd_code.CompareTo("104") == 0 || cc_rd_code.CompareTo("110") == 0))) 
                     && ccc_inv.Substring(0, 2).CompareTo("04") == 0)
                {
                    illrow = int.Parse(GetDataString(@"select count(*) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3  and b.bInvKeyPart<>1 and a.Qty<>0", Cmd));
                    //合格品取 所有件 齐套量
                    f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(IssQty*" + i_c_mom_qty + "/Qty+0.49,0)),0) from " + dbname + "..mom_moallocate a inner join " + dbname + "..Inventory_Sub b on a.invcode=b.cInvSubCode where a.MoDId=0" + dtfclist.Rows[0]["modid"] + " and WIPType=3 and b.bInvKeyPart<>1 and a.Qty<>0", Cmd));
                    if (f_ll_qty < f_Qty_in + f_qtyinall && illrow > 0)
                    {
                        throw new Exception("不能超领料入库");
                    }
                }

                //回写生产订单信息
                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + records10.iQuantity +
                    "),Define35=isnull(Define35,0)+" + f_feipin_count + " where modid=0" + dtfclist.Rows[0]["modid"];
                Cmd.ExecuteNonQuery();

                //是否超生产订单入库
                if (GetDataInt("select count(*) from " + dbname + @"..mom_orderdetail where modid=0" + dtfclist.Rows[0]["modid"] + " and isnull(QualifiedInQty,0)>Qty", Cmd) > 0)
                    throw new Exception("不能超订单入库");

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

            ibcostcount = 1;
            string feip_daochong = "" + GetDataString("select cvalue from " + dbname + @"..T_Parameter where cpid='crd_mx_feip_daochong'", Cmd);
            if ((dtHead.Rows[0]["是否废品"] + "").CompareTo("是") != 0 || feip_daochong.ToLower().CompareTo("true")==0)  //合格品入库倒冲  或 参数废品需要倒冲
            {
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
                        ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + record11.cWhCode + " and bInCost=1", Cmd);

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
                        records11.cFree2 = (dtMer.Rows[r]["cfree2"] + "" == "" ? "null" : "'" + dtMer.Rows[r]["cfree2"] + "'");

                        if (ibcostcount == 0) records11.bCosting = "0";

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
            }

//            if ((dtHead.Rows[0]["是否废品"] + "").CompareTo("是") != 0)
//            {
//                decimal the_last_qty = decimal.Parse(GetDataString("select isnull(sum(qty_in-isnull(qty_out,0)),0) ilast from " + dbname + @"..T_CC_CQCC_AutoRd08_Relation 
//                where modid=0" + modid, Cmd));
//                decimal d_rk_qty = decimal.Parse("" + dtBody.Rows[0]["入库数"]);
//                if (the_last_qty > 0)  //存在未核销的数量
//                {
//                    decimal d_red_08 = the_last_qty >= d_rk_qty ? d_rk_qty : the_last_qty;  //核销数量
//                    dtBody.Rows[0]["入库数"] = "" + d_red_08;
//                    U81103(dtHead, dtBody, dbname, dtHead.Rows[0]["制单人"] + "", dtHead.Rows[0]["制单日期"] + "", "U81103", rd_id + "", modid, Cmd);
//                }
//            }

            if (dtHead.Columns.Contains("是否红字其他入库"))
            {
                if ((dtHead.Rows[0]["是否红字其他入库"] + "").CompareTo("是") == 0)
                {
                    U81103(dtHead, dtBody, dbname, dtHead.Rows[0]["制单人"] + "", dtHead.Rows[0]["制单日期"] + "", "U81103", rd_id + "", modid, Cmd);
                }
            }

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

    [WebMethod]  //U8 扫描流转卡  到货质量输入 [MeiXin]
    public bool U8Mom_PU_Arr_Modify(System.Data.DataTable dtData,string cUserName, string dbname)
    {
        if (dtData.Rows.Count == 0) throw new Exception("没有明细记录");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        if (dtData == null) throw new Exception("传入数据为空！");
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            int vou_id = vou_id = GetDataInt("select id from " + dbname + "..PU_ArrivalVouchs where autoid=0" + dtData.Rows[0]["autoid"], Cmd);
            if (GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..rdrecords01 b on a.autoid=b.iArrsId
                where a.id=" + vou_id, Cmd) > 0)
            {
                throw new Exception("已经入库不能调整");
            }

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cdefine23='" + dtData.Rows[i]["质量原因"] + @"',
                        cdefine24='" + dtData.Rows[i]["收货类型"] + @"',
                        cDefine33= '" + dtData.Rows[i]["粗车厂家"] + @"'
                    where autoid=0" + dtData.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();
            }

            //审核到货单
            if (GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs where id=" + vou_id + "  and (isnull(cdefine23,'')<>'' or isnull(cdefine24,'')='D')", Cmd) == 0)
            {
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set cverifier='" + cUserName + @"',cAuditDate=convert(varchar(10),getdate(),120),caudittime=getdate()
                    where id=" + vou_id + " and isnull(cverifier,'')=''";
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw new Exception(ex.Message);
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
        

    }


    //产品入库单:自动生成其他入库红字
    private string U81103(System.Data.DataTable HeadData, System.Data.DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID,string rd10code,string t_modid, System.Data.SqlClient.SqlCommand Cmd)
    {
        #region 生成其他入库单(红字)
        //表头传入入库类别,通过T_Parameter配置(未入库先发货入库类别)
        System.Data.DataTable crkTypeDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @" select top 1  t1.crdcode,t1.crdname from " + dbname + @"..Rd_style t1 
                inner join " + dbname + @"..T_Parameter t2 on t1.crdcode=t2.cValue
                where t2.cPID ='TimelycRdCode'");
        if (crkTypeDt.Rows.Count == 0) throw new Exception("未设置{未入库先发货入库类别},或参数表中没有设置{未入库先发货入库类别}TimelycRdCode");

        System.Data.DataTable dtHead = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' LabelText,'' TxtName,'' TxtTag,'' TxtValue where 1=0");
        System.Data.DataRow dr = dtHead.NewRow();
        dr["LabelText"] = "仓库";  //栏目标题
        dr["TxtName"] = "txt_cwhcode";   //txt_字段名称
        dr["TxtTag"] = HeadData.Rows[0]["仓库编码"] + "";  //标识
        dr["TxtValue"] = HeadData.Rows[0]["仓库编码"] + "";  //栏目值
        dtHead.Rows.Add(dr);
        dr = dtHead.NewRow();
        dr["LabelText"] = "入库类别";  //栏目标题
        dr["TxtName"] = "txt_crdcode";   //txt_字段名称
        dr["TxtTag"] = crkTypeDt.Rows[0][0];  //标识
        dr["TxtValue"] = crkTypeDt.Rows[0][1];  //栏目值
        dtHead.Rows.Add(dr);

        System.Data.DataTable dtBody = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' cbarcodetxt,'' cbatch,'' cinvcode,'' cinvname, '' cinvstd, '' cposcode, 0 iquantity,'' itransid where 1=0");
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            System.Data.DataRow bodydr = dtBody.NewRow();
            bodydr["cinvcode"] = BodyData.Rows[i]["产品编码"]; //存货编码
            bodydr["cposcode"] = ""; //货位必须有，值可以是空
            bodydr["cbatch"] = BodyData.Rows[i]["入库批号"] + ""; //批号最好给固定值  
            bodydr["iquantity"] = -decimal.Parse(BodyData.Rows[i]["入库数"] + ""); //数量
            dtBody.Rows.Add(bodydr);
        }
        U8StandSCMBarCode u8 = new U8StandSCMBarCode();
        string otherRkCode = u8.Test_SCM_Method(dtHead, dtBody, dbname, cUserName, cLogDate, "U81019", "U81019", Cmd);
        if (string.IsNullOrEmpty(otherRkCode))
        {
            throw new Exception("生成其他入库单失败,未能获得反馈的入库单据号");
        }

        Cmd.CommandText = "insert into " + dbname + @"..T_CC_CQCC_AutoRd08_Relation(ctype,vouchid,rd08id,qty_in,qty_out,modid) 
            values('完工'," + rd10code + "," + otherRkCode.Split(',')[0] + ",0," + BodyData.Rows[0]["入库数"] + "," + t_modid + ")";
        Cmd.ExecuteNonQuery();
        #endregion
        return otherRkCode;
    }


    [WebMethod]  //U8 流转卡  完工扫描按卡入库-半成品 [HL]
    public string U8Mom_CardGoodIn_Save_HL(string pfcode, System.Data.DataTable dtHead, System.Data.DataTable dtBody, string dbname, string acc_id)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        string errmsg = "";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            GetDataString("select 'cqcc dubug'", Cmd);
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string cToday = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
            //获得 部门编码信息
            string c_pfid = "" + GetDataString("select pfid from " + dbname + "..sfc_processflow where DocCode='" + pfcode + "'", Cmd);
            System.Data.DataTable dt_CardInfo = GetSqlDataTable("select b.invcode,convert(varchar(10),a.CreateDate,120) CreateDate,a.doccode,b.MDeptCode from " + dbname + @"..sfc_processflow a 
                inner join " + dbname + "..mom_orderdetail b on a.modid=b.modid where pfid=0" + c_pfid, "dt_CardInfo", Cmd);
            if (dt_CardInfo.Rows.Count == 0) throw new Exception("未找到流转卡信息！");
            string card_date = dt_CardInfo.Rows[0]["CreateDate"] + "";
            if (card_date.CompareTo("2020-05-01") < 0)
                throw new Exception("按生产部要求，2020年5月份以前制作的流转卡，不允许入库");

            string ccDepcode = "" + dt_CardInfo.Rows[0]["MDeptCode"];
            if (ccDepcode.CompareTo("") == 0)
            {
                //获得流转卡的领料部门
                ccDepcode = "" + GetDataString("select b.cdepcode from " + dbname + "..sfc_processflow a inner join " + dbname + "..department b on a.define14=b.cdepname where a.DocCode='" + pfcode + "'", Cmd);
                if (ccDepcode.CompareTo("") == 0)  throw new Exception("生产订单没有录入生产部门");
            }

            #region  //机加流转卡入库判定
            string b_new_card_code_rk = "";
            //if (dtBody.Columns.Contains("货位"))
            //{
            //    dtBody.Rows[0]["货位"] = GetDataString("select cposcode from " + dbname + "..position where cposcode='" + dtBody.Rows[0]["货位"] + "' or  or cBarCode='" + dtBody.Rows[0]["货位"] + "'", Cmd);
            //}
            if (dtHead.Columns.Contains("是否货位流程") && int.Parse(dtBody.Rows[0]["入库项目"] + "") == 1)
            {
                string c_jj_card = dtHead.Rows[0]["机加卡号"] + "";
                //老卡逻辑判断
                System.Data.DataTable dtOldData = GetSqlDataTable(@"select isnull(cc_is_ruku,0) cc_is_ruku,cc_qty,cc_poscode,cc_rd_batchno
                    from " + dbname + @"..T_CC_HL_0330_Card_State where cc_cardno='" + pfcode + "'", "dtOldData", Cmd);
                bool bOld_Card_Rk = false;
                if (dtOldData.Rows.Count > 0) //已经入过库的流转卡，剩余部分必须入库原卡所在货位
                {
                    if (int.Parse(dtOldData.Rows[0]["cc_is_ruku"] + "") == 1) bOld_Card_Rk = true;  //老卡已经入库
                    //判断货位等信息(若老卡已经入货位，则比对货位一致性)
                    if (dtOldData.Rows[0]["cc_poscode"] + "" != "" && dtOldData.Rows[0]["cc_poscode"] + "" != dtBody.Rows[0]["货位"] + "")
                        throw new Exception("流转卡[" + pfcode + "]只能入到货位[" + dtOldData.Rows[0]["cc_poscode"] + "]");
                    //入库批号一致性检查
                    if (dtOldData.Rows[0]["cc_rd_batchno"] + "" != dtBody.Rows[0]["入库批号"] + "")
                        throw new Exception("流转卡[" + pfcode + "]只能入库批号只能使用[" + dtOldData.Rows[0]["cc_rd_batchno"] + "]");
                }
                else
                {
                    //增加老卡记录，此处为 老卡未入库状态,数量为0
                    Cmd.CommandText = "insert into " + dbname + @"..T_CC_HL_0330_Card_State(cc_cardno, cc_invcode, cc_whcode, cc_poscode, cc_batchno, cc_luhao, cc_qty, cc_date, cc_depcode, 
                            cc_depname, cc_is_ruku, cc_ck_maker, cc_ck_date,cc_top_card,cc_cailiao,cc_rd_batchno) 
                        select a.doccode,b.invcode cc_invcode,'' cc_whcode,'' cc_poscode,a.Define3 cc_batchno,
                            a.Define10 cc_luhao,0 cc_qty,a.CreateDate cc_date,isnull(d.cdepcode,'" + ccDepcode + @"') cc_depcode,
                            a.Define14 cc_depname,0 cc_is_ruku,'' cc_ck_maker,null cc_ck_date,1 cc_top_card,a.Define13 cc_cailiao,'" + dtBody.Rows[0]["入库批号"] + @"'
                        from " + dbname + "..sfc_processflow a inner join " + dbname + @"..mom_orderdetail b on a.MoDId=b.MoDId
                        left join " + dbname + @"..Department d on a.Define14=d.cDepName
                        where DocCode='" + pfcode + "'";
                    Cmd.ExecuteNonQuery();
                }

                //按照新卡入库
                if (c_jj_card != "")
                {
                    //1    按照新卡入库
                    if (bOld_Card_Rk) throw new Exception("毛坯段流转卡（旧卡）[" + pfcode + "]已经直接扫码入库，此卡的数据只能按照老卡入库");
                    System.Data.DataTable dtNewData = GetSqlDataTable("select isnull(cc_is_ruku,0) cc_is_ruku,cc_qty from " + dbname + "..T_CC_HL_0330_Card_State where cc_cardno='" + c_jj_card + "'", "dtNewData", Cmd);
                    if (dtNewData.Rows.Count == 0) throw new Exception("机加流转卡[" + c_jj_card + "]信息不存在，可能被删除");
                    if (int.Parse("" + dtNewData.Rows[0]["cc_is_ruku"]) > 0)
                        throw new Exception("机加流转卡[" + c_jj_card + "]属于入库状态，不能重复入库");
                    //判断数量是否修改,比对数量
                    if (int.Parse("" + dtNewData.Rows[0]["cc_qty"]) != int.Parse("" + dtBody.Rows[0]["入库数"]))
                        throw new Exception("按照新机加流转卡[" + c_jj_card + "]入库时，入库数量不能修改，必须为[" + dtNewData.Rows[0]["cc_qty"] + "]");
                    //判断是否有货位信息
                    if (!(dtBody.Columns.Contains("货位")) || dtBody.Rows[0]["货位"] + "" == "") throw new Exception("按新机加流转卡入库时，请扫描货位信息");


                    ////判断货位是否为固定货位 
                    //if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_0330_Position where cc_ps_code='" + dtBody.Rows[0]["货位"] + "'", Cmd) == 0)
                    //    throw new Exception("按新机加流转卡入库时，只能进入固定货位");

                    //记录流转卡的材料批号，方便后面接收出库
                    Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_is_ruku=1,cc_rd_batchno='" + dtBody.Rows[0]["入库批号"] + @"',
                        cc_poscode='" + dtBody.Rows[0]["货位"] + "',cc_whcode='" + dtHead.Rows[0]["仓库编码"] + @"' where cc_cardno='" + c_jj_card + "'";
                    Cmd.ExecuteNonQuery();

                    b_new_card_code_rk = c_jj_card;
                }
                else
                {
                    //2    按照老卡入库
                    if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_0330_Card_State a inner join " + dbname + @"..T_CC_HL_0330_Card_DZ b on a.cc_cardno=b.cc_jj_cardno
                        where b.cc_mapi_cardno='" + pfcode + "' and isnull(a.cc_is_ruku,0)=0", Cmd) > 0)
                        throw new Exception("按旧流转卡（毛坯卡）未入库前已经拆分成新卡，请扫描新卡入库"); //新卡必须先于旧卡入库
                    if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_HL_0330_Position where cc_ps_code='" + dtBody.Rows[0]["货位"] + "'", Cmd) > 0)
                        throw new Exception("按旧流转卡（毛坯卡）入库时，只能进入临时货位");
                    //机加为空，代表按照老卡入库，则老卡为  入库状态
                    Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_is_ruku=1,cc_whcode='" + dtHead.Rows[0]["仓库编码"] + @"',
                        cc_poscode='" + dtBody.Rows[0]["货位"] + @"',cc_qty=cc_qty+(" + dtBody.Rows[0]["入库数"] + ") where cc_cardno='" + pfcode + "'";
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion


            KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
            //主表
            int rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            string cCodeHead = "G" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd);
            string cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord10 where ccode like '" + cCodeHead + "%'", Cmd);
            record10.cCode = "'" + cc_mcode + "'";
            record10.ID = rd_id;
            record10.cVouchType = "'10'";
            record10.cBusType = "'成品入库'";
            if (GetDataInt("select count(*) from " + dbname + @"..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "'", Cmd) == 0) throw new Exception("仓库不存在");
            record10.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
            record10.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
            record10.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
            //record10.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            //record10.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            record10.dVeriDate = "'" + cToday + "'";
            record10.dDate = "'" + cToday + "'";

            record10.cDepCode = "'" + ccDepcode + "'";
            record10.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
            record10.cMemo = "''";
            record10.iExchRate = "1";
            record10.cExch_Name = "'人民币'";
            record10.cSource = "'生产订单'";
            record10.cDefine3 = "'" + pfcode + "'";
            //理论部门
            if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            if (b_new_card_code_rk != "")  //记录新卡 与入库单的关联关系
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_HL_0330_Card_State set cc_rk_rd10code=" + record10.cCode + " where cc_cardno='" + b_new_card_code_rk + "'";
                Cmd.ExecuteNonQuery();
            }

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
            records10.cDefine24 = "'" + pfcode + "'";
            
            #region //货位信息判断
            bool bPos = false;
            if (GetDataInt("select count(*) from " + dbname + @"..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and bWhPos=1", Cmd) > 0)
            {
                if (!(dtBody.Columns.Contains("货位")) || dtBody.Rows[0]["货位"]+"" == "") throw new Exception("仓库有货位管理，请扫描货位信息,确保货位存在");
                if (GetDataInt("select count(*) from " + dbname + @"..position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cposcode='" + dtBody.Rows[0]["货位"] + "'", Cmd) == 0)
                {
                    throw new Exception("仓库[" + dtHead.Rows[0]["仓库编码"] + "]的货位[" + dtBody.Rows[0]["货位"] + "]不存在");
                }
                records10.cPosition = "'" + dtBody.Rows[0]["货位"] + "'";
                bPos = true;
            }
            #endregion

            //生产订单信息
            System.Data.DataTable dtfclist = GetSqlDataTable(@"select top 1 c.mocode,a.modid,b.SortSeq,MoLotCode,b.invcode,d.opseq,d.description,e.WcCode,a.Define10 cluhao,a.Define11 free1,a.Define12 free2
                from " + dbname + "..sfc_processflow a inner join " + dbname + @"..mom_orderdetail b on a.modid=b.modid
                inner join " + dbname + @"..mom_order c on b.moid=c.moid
                left join " + dbname + @"..sfc_moroutingdetail d on b.modid=d.modid
                left join " + dbname + @"..sfc_workcenter e on d.wcid=e.wcid
                where a.doccode='" + pfcode + "' order by d.opseq desc", "dtfclist", Cmd);

            if (dtfclist.Rows.Count > 0)
            {
                records10.cInvCode = "'" + dtfclist.Rows[0]["invcode"] + "'";

                if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and bFree1=1", Cmd) > 0)
                {
                    records10.cFree1 = "'" + dtfclist.Rows[0]["free1"] + "'";
                }
                else
                {
                    dtfclist.Rows[0]["free1"] = "";
                }

                if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and bFree2=1", Cmd) > 0)
                {
                    records10.cFree2 = "'" + dtfclist.Rows[0]["free2"] + "'";
                }
                else
                {
                    dtfclist.Rows[0]["free2"] = "";
                }
                records10.iMPoIds = "" + dtfclist.Rows[0]["modid"];
                records10.cDefine34 = int.Parse("" + dtfclist.Rows[0]["modid"]);
                records10.imoseq = "" + dtfclist.Rows[0]["SortSeq"];
                records10.cMoLotCode = "'" + dtfclist.Rows[0]["MoLotCode"] + "'";
                records10.cmocode = "'" + dtfclist.Rows[0]["mocode"] + "'";
                records10.cBatchProperty6 = "'" + dtfclist.Rows[0]["cluhao"] + "'";
                records10.iNum = "0";
                if ("" + dtfclist.Rows[0]["WcCode"] != "")
                {
                    records10.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
                    records10.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
                    records10.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
                }

                records10.cDefine30 = records10.cInvCode;  //存货编码
                records10.cDefine31 = "'"+GetDataString("select cinvname from " + dbname + @"..inventory where cinvcode='" + dtfclist.Rows[0]["invcode"] + "'", Cmd)+"'";

                //批号
                if (GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + dtfclist.Rows[0]["invcode"] + "' and bInvBatch=1", Cmd) > 0)
                {
                    records10.cBatch = "'" + dtBody.Rows[0]["入库批号"] + "'";
                    //批次档案 处理
                    if (int.Parse(GetDataString("select count(*) from " + dbname + @"..AA_BatchProperty where cInvCode='" + dtfclist.Rows[0]["invcode"] +
                        "' and cBatch='" + dtBody.Rows[0]["入库批号"] + "' and cfree1='" + dtfclist.Rows[0]["free1"] + "' and cfree2='" + dtfclist.Rows[0]["free2"] + "'", Cmd)) == 0)
                    {
                        Cmd.CommandText = @"insert into " + dbname + @"..AA_BatchProperty(cbatchpropertyguid,cinvcode,cbatch,cfree1,cfree2,cbatchproperty6,cbatchproperty7,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10) 
                            values(newid(),'" + dtfclist.Rows[0]["invcode"] + "','" + dtBody.Rows[0]["入库批号"] + "','" + dtfclist.Rows[0]["free1"] + "','" + dtfclist.Rows[0]["free2"] +
                                              "','" + dtfclist.Rows[0]["cluhao"] + "','','','','','','','','','')";
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (dtfclist.Rows[0]["cluhao"] + "" != "")
                        {
                            Cmd.CommandText = "update " + dbname + @"..AA_BatchProperty set cbatchproperty6='" + dtfclist.Rows[0]["cluhao"] + @"' 
                                where cInvCode='" + dtfclist.Rows[0]["invcode"] +"' and cBatch='" + dtBody.Rows[0]["入库批号"] + @"' 
                                and cfree1='" + dtfclist.Rows[0]["free1"] + "' and cfree2='" + dtfclist.Rows[0]["free2"] + "'";
                            Cmd.ExecuteNonQuery();
                        }
                    }
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

                //写流转记录明细表
                Cmd.CommandText = "insert " + dbname + "..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) values(" + records10.AutoID + "," + c_pfid + "," + dtBody.Rows[0]["入库项目"] + ") ";
                Cmd.ExecuteNonQuery();

                //回写生产订单信息
                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + records10.iQuantity +
                    ")  where modid=0" + dtfclist.Rows[0]["modid"];
                Cmd.ExecuteNonQuery();

                //获得生产订单的总查缺数
                string c_cha_que_qty = GetDataString(@"select cast(isnull(SUM(c.BalRefusedQty),0) as float) from " + dbname + @"..sfc_processflow b inner join " + dbname + @"..sfc_processflowdetail c on b.PFId=c.PFId
                    where b.MoDId=0" + dtfclist.Rows[0]["modid"], Cmd);
                //关闭生产订单
                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='" + dtHead.Rows[0]["制单人"] +
                    "',CloseTime=getdate(),CloseDate=convert(varchar(10),getdate(),120),Status=4 where modid=0" + dtfclist.Rows[0]["modid"] + " and isnull(QualifiedInQty,0)>=(Qty-(0" + c_cha_que_qty + "))";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("没有找到生产订单信息");
            }
            //保存数据
            if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            #region //是否超流转卡量入库
            if (int.Parse(dtBody.Rows[0]["入库项目"] + "") == 1)
            {
                decimal d_rpt_validqty = decimal.Parse(GetDataString("select isnull(sum(balqualifiedqty),0) from " + dbname + @"..sfc_processflowdetail where PFId=0" + c_pfid, Cmd));
                decimal d_rk_validqty = decimal.Parse(GetDataString(@"select isnull(sum(b.iquantity),0) iqty 
                    from " + dbname + "..T_CC_Rd10_FlowCard a inner join " + dbname + @"..rdrecords10 b on a.t_autoid=b.autoid where a.t_card_id=" + c_pfid + " and a.t_ctype=1", Cmd));
                if (d_rpt_validqty < d_rk_validqty) throw new Exception("超流转卡合格报工量入库");
            }            
            #endregion
            
            #region //货位账务处理
            if (bPos)
            {
                float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1 where autoid =0" + records10.AutoID;
                    Cmd.ExecuteNonQuery();
                }

                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                    cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cvmivencode,cbatch,cHandler,
                    cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                    dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                select b.autoid,b.id,a.cwhcode," + records10.cPosition + @",b.cinvcode,iquantity,inum,
                    cmemo,ddate,brdflag,'',0,cvouchtype,a.dDate,isnull(cvmivencode,''),isnull(cbatch,'')," + record10.cMaker + @",
                    isnull(cfree1,''),isnull(cfree2,''),isnull(cfree3,''),isnull(cfree4,''),isnull(cfree5,''),
                    isnull(cfree6,''),isnull(cfree7,''),isnull(cfree8,''),isnull(cfree9,''),isnull(cfree10,''),cinvouchtype,cAssUnit,
                    dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate 
                from " + dbname + @"..rdrecords10 b inner join " + dbname + @"..rdrecord10 a on a.id=b.id where autoid=" + records10.AutoID;
                Cmd.ExecuteNonQuery();


                //修改货位库存
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + record10.cWhCode + @" and cvmivencode='' 
                and cinvcode=" + records10.cInvCode + @" and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + @" 
                and cfree1='" + dtfclist.Rows[0]["free1"] + "' and cfree2='" + dtfclist.Rows[0]["free2"] + @"' and cfree3='' 
                and cfree4='' and cfree5='' and cfree6=''
                and cfree7='' and cfree8='' and cfree9='' 
                and cfree10='' ", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    values(" + record10.cWhCode + @"," + records10.cPosition + "," + records10.cInvCode + @",0," + records10.cBatch + @",
                        '" + dtfclist.Rows[0]["free1"] + "','" + dtfclist.Rows[0]["free2"] + @"','','','',
                        '','','','','','','',0," + records10.dMadeDate + "," + records10.iMassDate + @",
                        " + records10.cMassUnit + "," + records10.iExpiratDateCalcu + "," + records10.cExpirationdate + "," + records10.dVDate + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + records10.iQuantity + "),inum=isnull(inum,0)+(0" + records10.iNum + @") 
                where cwhcode=" + record10.cWhCode + @" and cvmivencode='' 
                and cinvcode=" + records10.cInvCode + @" and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + @" 
                and cfree1='" + dtfclist.Rows[0]["free1"] + "' and cfree2='" + dtfclist.Rows[0]["free2"] + @"' and cfree3='' 
                and cfree4='' and cfree5='' and cfree6='' 
                and cfree7='' and cfree8='' and cfree9='' 
                and cfree10=''";
                Cmd.ExecuteNonQuery();
            }
            #endregion

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

    #region
    #region   //采购入库单  
    [WebMethod]  //U8 保存 采购入库单  返回 “采购入库单ID,单据号码”[KL] 凯隆
    public string U8SCM_RDS_01_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            string targetPTCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetPTCode'", Cmd);//采购类型
            if (targetPTCode == "") { errmsg = "没有配置采购类别编码"; throw new Exception(errmsg); }

            KK_U8Com.U8Rdrecord01 record01 = new KK_U8Com.U8Rdrecord01(Cmd, dbname);
            int iPosSet = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "R" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord01 where ccode like '" + cCodeHead + "%'", Cmd);
                record01.cCode = "'" + cc_mcode + "'";
                record01.ID = rd_id;
                record01.cVouchType = "'01'";
                record01.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record01.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record01.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record01.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                record01.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                record01.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                record01.cVenCode = "'" + dtHead.Rows[0]["供应商编码"] + "'";
                record01.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                record01.cPTCode = "'" + targetPTCode + "'";
                record01.iExchRate = 1;
                record01.cExch_Name = "'人民币'";
                record01.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record01.cSource = "'库存'";

                if (!record01.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }
                KK_U8Com.U8Rdrecords01 records01 = new KK_U8Com.U8Rdrecords01(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records01.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records01.ID = rd_id;
                records01.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records01.iQuantity = "" + dtBody.Rows[i]["数量"];
                if (iPosSet > 0)
                {
                    if (dtBody.Rows[0]["区域"] + "" == "")
                    {
                        throw new Exception("没有输入区域");
                    }
                    records01.cPosition = "'" + dtBody.Rows[0]["区域"] + "'";
                }
                records01.iMoney = "0";
                records01.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                //records01.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records01.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records01.irowno = (i + 1);
                records01.iTaxRate = 0;
                records01.ioriSum = "" + dtBody.Rows[i]["金额"];
                //上游单据关联
                records01.iNQuantity = records01.iQuantity;

                //保存数据
                if (!records01.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                if (iPosSet > 0)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'01','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    //Cmd.CommandText = "update " + dbname + "..rdrecords01 set iposflag=1 where autoid =0" + cAutoid;
                    //Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" +
                        dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["数量"] + @") 
                        where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
                    Cmd.ExecuteNonQuery();
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
            CloseDataConnection(Conn);
        }


    }

    #endregion

    #region   //材料出库单
    [WebMethod]  //U8 保存 材料出库单  返回 “材料出库单ID,单据号码”[KL]
    public string U8SCM_RDS_11_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
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

                //record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record11.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record11.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record11.iExchRate = "1";
                record11.cExch_Name = "'人民币'";

                record11.cSource = "'库存'";
                if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }

                KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records11.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records11.ID = rd_id;
                records11.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records11.iQuantity = "" + dtBody.Rows[i]["数量"];
                if (iPosSet > 0) records11.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records11.irowno = (i + 1);
                records11.iNQuantity = records11.iQuantity;
                records11.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";

                //保存数据
                if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                if (iPosSet > 0)
                {
                    //添加货位记录
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
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

    [WebMethod]  //U8 保存 材料出库单  返回 “材料出库单ID,单据号码”[MX]
    public string U8SCM_RDS_11_MX_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
                modid = "" +GetDataString("select b.modid from " + dbname + "..mom_order a inner join " + dbname + "..mom_orderdetail b on a.moid=b.moid where a.mocode='" + dtHead.Rows[0]["生产订单"] + "' and b.SortSeq=0" + dtHead.Rows[0]["行号"], Cmd);
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

                //record11.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record11.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record11.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record11.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record11.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record11.iExchRate = "1";
                record11.cExch_Name = "'人民币'";

                record11.cSource = "'库存'";
                if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }

                KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records11.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records11.ID = rd_id;
                records11.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records11.iQuantity = "" + dtBody.Rows[i]["数量"];
                if (iPosSet > 0) records11.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                //records11.iNum = ("" + dtBody.Rows[i]["inum"] == "" ? "null" : "" + dtBody.Rows[i]["inum"]);
                //records11.iinvexchrate = ("" + dtBody.Rows[i]["iInvExchRate"] == "" ? "null" : "" + dtBody.Rows[i]["iInvExchRate"]);
                records11.irowno = (i + 1);
                records11.iNQuantity = records11.iQuantity;
                records11.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                if (targetAccId.CompareTo("201") == 0)
                {
                    record11.VT_ID = 131387;
                }
                else
                {
                    record11.VT_ID = 65;
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
                string cmollateid = "" + GetDataString("select AllocateId from " + dbname + "..mom_moallocate where modid=0" + modid + " and WIPType=3 and invcode=" + records11.cInvCode, Cmd);
                if (cmollateid == "") throw new Exception("生产订单无材料" + records11.cInvCode+" 或为倒冲材料");
                records11.iMPoIds = cmollateid;
                records11.ipesodid = cmollateid;
                records11.imoseq = "" + dtHead.Rows[0]["行号"];
                records11.ipesoseq = "" + dtHead.Rows[0]["行号"];
                records11.cmocode = "'" + dtHead.Rows[0]["生产订单"] + "'";
                records11.cpesocode = "'" + dtHead.Rows[0]["生产订单"] + "'";
                //records11.cMoLotCode = "''";
                records11.ipesotype = "7";
                records11.invcode = "'" + GetDataString("select invcode from " + dbname + "..mom_orderdetail where modid=0" + modid, Cmd) +"'";

                //部门编码控制
                string mo_depcode = GetDataString("select MDeptCode from " + dbname + "..mom_orderdetail where MoDId=0" + modid, Cmd);
                if (dtHead.Rows[0]["部门编码"] + "" != mo_depcode)
                    throw new Exception("领用部门【" + dtHead.Rows[0]["部门编码"] + "】与生产订单的生产部门【" + mo_depcode + "】不一致");
                #endregion

                int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + record11.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) records11.bCosting = "0";
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
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
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
                #endregion

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


    #endregion

    #region  //采购到货单
    [WebMethod]  //U8 保存 根据采购订单  采购到货单  返回 “采购到货单ID,单据号码”[MX]
    public string U8SCM_PuArri_MeiXin(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            if (rd_id == 0)  //新增主表
            {
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "D" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..PU_ArrivalVouch where ccode like '" + cCodeHead + "%'", Cmd);
                pumain.ID = rd_id+"";
                pumain.cCode="'"+cc_mcode+"'";
                pumain.iVTid = "8169";
                pumain.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                pumain.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                pumain.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                //表头项目
                System.Data.DataTable dtPUHead = GetSqlDataTable("select cVenCode,cPersonCode,cPTCode,cexch_name,nflat,iTaxRate,cVenPUOMProtocol,cBusType from " + dbname + 
                    "..PO_Pomain where cpoid='" + dtHead.Rows[0]["订单号"] + "'", "dtPUHead", Cmd);
                if (dtHead.Rows.Count > 0)
                {
                    pumain.cVenCode = "'" + dtPUHead.Rows[0]["cVenCode"]+"'";
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
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                KK_U8Com.U8PU_ArrivalVouchs pudetail = new KK_U8Com.U8PU_ArrivalVouchs(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
                pudetail.Autoid = cAutoid + "";
                pudetail.ID = rd_id+"";
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
                Cmd.ExecuteNonQuery();
                pudetail.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                pudetail.iPOsID = dtBody.Rows[i]["autoid"] + "";
                pudetail.iQuantity = dtBody.Rows[i]["数量"] + "";
                pudetail.bGsp = "0";//不质检
                pudetail.fRealQuantity = pudetail.iQuantity;
                pudetail.fValidQuantity = pudetail.iQuantity;
                pudetail.iTaxRate = dtBody.Rows[i]["税率"] + "";
                pudetail.SoType = "0";
                pudetail.ivouchrowno = "" + (i + 1);
                if ((dtBody.Rows[i]["含税单价"] + "").ToString().CompareTo("") != 0)  //有单价
                {
                    pudetail.iOriSum = "" + (float.Parse(dtBody.Rows[i]["数量"] + "") * float.Parse(dtBody.Rows[i]["含税单价"] + ""));
                }
                //补充订单信息
                pudetail.cordercode = "'" + dtHead.Rows[0]["订单号"] + "'";
                pudetail.bTaxCost = "1";

                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)+(0" + dtBody.Rows[i]["数量"] + ") where id=0" + dtBody.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtBody.Rows[i]["autoid"];
                Cmd.ExecuteNonQuery();
                
                if (!pudetail.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)) where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();
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

        return rd_id+","+cc_mcode;
    }

    [WebMethod]  //U8 保存 根据采购订单  采购到货单  返回 “采购到货单ID,单据号码”[MX ASN]
    public string U8SCM_PuASN_MeiXin(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            string c_retvalue = U8SCM_PuASN_Add(dtHead, dtBody, dtWidth, dbname, Cmd,true );  //ASN保存
            tr.Commit();
            return c_retvalue;
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
    public string U8SCM_PuASN_Add(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname,System.Data.SqlClient.SqlCommand Cmd,bool WriteBack)
    {

        string errmsg = "";
        string cc_mcode = "";
        int pu_id = 0;

        int i_has_queshi_row = 0;
        int i_has_valid_row = 0;

        string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        KK_U8Com.U8PU_ArrivalVouch pumain = new KK_U8Com.U8PU_ArrivalVouch(Cmd, dbname);
        #region  //到货单新增主表
        pu_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
        Cmd.ExecuteNonQuery();
        string cCodeHead = "D" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
        cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..PU_ArrivalVouch where ccode like '" + cCodeHead + "%'", Cmd);
        pumain.ID = pu_id + "";
        pumain.cCode = "'" + cc_mcode + "'";
        pumain.iVTid = "8169";
        pumain.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
        pumain.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
        pumain.cverifier = "'" + dtHead.Rows[0]["制单人"] + "'";
        pumain.cAuditDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
        pumain.cVenCode = "'" + dtHead.Rows[0]["供应商"] + "'";
        pumain.iTaxRate = "13";

        //表头项目  
        System.Data.DataTable dtPUHead = null;
        GetDataString("select 'Count[" + dtBody.Rows.Count + "]'", Cmd);
        GetDataString("select '[" + dtBody.Rows[0]["psosid"] + "]'", Cmd);
        if (dtHead.Rows[0]["采购来源"].ToString().CompareTo("采购订单") == 0)
        {
            dtPUHead = GetSqlDataTable("select top 1 a.cpoid,cVenCode,cPersonCode,cdepcode,cPTCode,cexch_name,nflat,iTaxRate,cVenPUOMProtocol,cBusType from " + dbname +
                "..PO_Pomain a inner join " + dbname + "..PO_Podetails b on a.poid=b.poid where b.id='" + dtBody.Rows[0]["psosid"] + "'", "dtPUHead", Cmd);
        }
        else
        {
            dtPUHead = GetSqlDataTable("select top 1 a.ccode cpoid,cVenCode,cPersonCode,cdepcode,cPTCode,cexch_name,nflat,iTaxRate,cVenPUOMProtocol,cBusType from " + dbname +
                "..OM_MOMain a inner join " + dbname + "..OM_MODetails b on a.moid=b.moid where b.MODetailsID='" + dtBody.Rows[0]["psosid"] + "'", "dtPUHead", Cmd);
        }
        //
        if (dtHead.Rows.Count > 0)
        {
            pumain.cDepCode = (dtPUHead.Rows[0]["cdepcode"] + "" == "" ? "null" : "'" + dtPUHead.Rows[0]["cdepcode"] + "'");
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
        pumain.IsWfControlled = "0";
        pumain.iverifystateex = "-1";  
        if (!pumain.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region  //采购到货单子表
        for (int i = 0; i < dtBody.Rows.Count; i++)
        {
            if (float.Parse(dtBody.Rows[i]["实收数量"] + "") <= 0) { i_has_queshi_row++; continue; }
            //判断ASN单是否已经 收料
            if (GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs where cDefine32 = '" + dtBody.Rows[i]["autoid"] + "'", Cmd) > 0)
                throw new Exception("[" + dtBody.Rows[i]["存货编码"] + "]已经收料，不能重复处理");

            i_has_valid_row++;
            KK_U8Com.U8PU_ArrivalVouchs pudetail = new KK_U8Com.U8PU_ArrivalVouchs(Cmd, dbname);
            int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
            pudetail.Autoid = cAutoid + "";
            pudetail.ID = pumain.ID;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
            Cmd.ExecuteNonQuery();
            pudetail.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
            pudetail.iPOsID = dtBody.Rows[i]["psosid"] + "";
            pudetail.iQuantity = dtBody.Rows[i]["实收数量"] + "";
            pudetail.fRealQuantity = pudetail.iQuantity;
            
            pudetail.iTaxRate = dtBody.Rows[i]["税率"] + "";
            pudetail.SoType = "0";
            pudetail.ivouchrowno = "" + (i + 1);
            pudetail.cDefine32 = "'" + dtBody.Rows[i]["autoid"] + "'";  //ASN单子表ID
            pudetail.cBatch = "'" + GetDataString("select case when bInvBatch=1 then '" + dtBody.Rows[i]["批号"] + @"' else '' end from " + dbname + @"..inventory 
                    where cInvCode='" + dtBody.Rows[i]["存货编码"] + "'", Cmd) + "'";

            string c_kd_qty = "" + dtBody.Rows[i]["扣点数量"];
            if (c_kd_qty == "") { c_kd_qty = "0"; dtBody.Rows[i]["扣点数量"] = "0"; }
            dtBody.Rows[i]["扣点原因"] = dtBody.Rows[i]["扣点原因"] + "";
            if (int.Parse(c_kd_qty) > 0 && dtBody.Rows[i]["扣点原因"].ToString().CompareTo("") == 0)
                throw new Exception("请输入扣点原因");

            pudetail.cDefine34 = "" + c_kd_qty;
            pudetail.cDefine22 = "'" + dtBody.Rows[i]["扣点原因"] + "'";

            if ((dtBody.Rows[i]["含税单价"] + "").ToString().CompareTo("") != 0)  //有单价
            {
                pudetail.iOriSum = "" + (float.Parse(dtBody.Rows[i]["实收数量"] + "") * float.Parse(dtBody.Rows[i]["含税单价"] + ""));
            }
            //补充订单信息
            pudetail.cordercode = "'" + dtPUHead.Rows[0]["cpoid"] + "'"; //采购订单号
            pudetail.bTaxCost = "1";

            //自由项
            if (dtHead.Rows[0]["采购来源"].ToString().CompareTo("采购订单") == 0)
            {
                pudetail.cFree1 = "'" + GetDataString("select cfree1 from " + dbname + "..PO_Podetails where id=" + dtBody.Rows[0]["psosid"], Cmd) + "'";
                pudetail.cFree2 = "'" + GetDataString("select cfree2 from " + dbname + "..PO_Podetails where id=" + dtBody.Rows[0]["psosid"], Cmd) + "'";
            }
            else
            {
                pudetail.cFree1 = "'" + GetDataString("select cfree1 from " + dbname + "..OM_MODetails where MODetailsID=" + dtBody.Rows[0]["psosid"], Cmd) + "'";
                pudetail.cFree2 = "'" + GetDataString("select cfree2 from " + dbname + "..OM_MODetails where MODetailsID=" + dtBody.Rows[0]["psosid"], Cmd) + "'";
            }
            //自由项2
            if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bFree2=1", Cmd) > 0)
            {
                if (pudetail.cFree2.CompareTo("''") == 0)
                {
                    pudetail.cFree2 = "'" + GetDataString("select cvalue from " + dbname + "..userdefine where cid='21' and calias=" + pumain.cVenCode, Cmd) + "'";
                }
            }

            //记录是否有缺失记录
            if (float.Parse(dtBody.Rows[i]["实收数量"] + "") < float.Parse(dtBody.Rows[i]["数量"] + "")) i_has_queshi_row++;
            pudetail.fValidQuantity = pudetail.iQuantity;
            
            if (!pudetail.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            if (dtHead.Rows[0]["采购来源"].ToString().CompareTo("采购订单") == 0)
            {
                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)+(0" + dtBody.Rows[i]["实收数量"] + ") where id=0" + dtBody.Rows[i]["psosid"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtBody.Rows[i]["psosid"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)) where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..OM_MODetails set iArrQTY=isnull(iArrQTY,0)+(0" + dtBody.Rows[i]["实收数量"] + ") where MODetailsID=0" + dtBody.Rows[i]["psosid"];
                Cmd.ExecuteNonQuery();
                //Cmd.CommandText = "update " + dbname + "..OM_MODetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtBody.Rows[i]["psosid"];
                //Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)),iproducttype=0 where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }

            if (WriteBack)
            {
                //回写ASN单记录
                Cmd.CommandText = "update VenDB.dbo.ASN_C set rowstate='是',arr_num=" + dtBody.Rows[i]["实收数量"] + " where ID='" + dtBody.Rows[i]["autoid"] + "'";
                Cmd.ExecuteNonQuery();
                //审批状态  spstate='3' 代表已经接收
                Cmd.CommandText = "update VenDB.dbo.ASN_H set spstate='3',realData=getdate() where ID in(select HID from VenDB.dbo.ASN_C where ID='" + dtBody.Rows[i]["autoid"] + "')";
                Cmd.ExecuteNonQuery();
                System.Data.DataTable dtASNDT = GetSqlDataTable(@"select a.asnjsjgs reciveVencode,c.id fromVencode,b.ID
                    from VenDB.dbo.ASN_H a inner join VenDB.dbo.ASN_C b on a.ID=b.HID 
                    inner join VenDB.dbo.sys_office c on a.officeID=c.code and a.accountid=c.accountid
                    where a.acttypes='02' and b.ID='" + dtBody.Rows[i]["autoid"] + "'", "dtASNDT", Cmd);
                if (dtASNDT.Rows.Count > 0)
                {
                    string c_moapi_code = GetDataString("select cinvcodeF from VenDB.dbo.Inentory_Replace where accountid='" + targetAccId + @"' and cinvcodeC='" + dtBody.Rows[i]["存货编码"] + "'", Cmd);
                    //更新库存
                    Cmd.CommandText = "update VenDB.dbo.MX_KC_MANAGER set curNum=curNum-(0" + pudetail.iQuantity + ") where accountid='" + targetAccId + @"' 
                        and fromVencode='" + dtASNDT.Rows[0]["fromVencode"] + "' and reciveVencode='" + dtASNDT.Rows[0]["reciveVencode"] + "' and cvincode='" + c_moapi_code + "'";
                    Cmd.ExecuteNonQuery();
                }

                //到货数到达 回复数量时，自动关闭采购订单/委外订单
                string cven_poid = GetDataString("select cpoid from VenDB.dbo.ASN_C where ID='" + dtBody.Rows[i]["autoid"] + "'", Cmd);
                int iHFcount = GetDataInt("select count(*) from VenDB.dbo.MX_GYSHF where accountid='" + targetAccId + "' and podid='" + cven_poid + "' and rowSpState not in(1,5)", Cmd);
                if (iHFcount == 0)
                {
                    //累计回复数
                    string hf_all_qty = GetDataString("select isnull(SUM(hfnum),0) from VenDB.dbo.MX_GYSHF where accountid='" + targetAccId + "' and podid='" + cven_poid + "' and rowSpState=1", Cmd);
                    if (dtHead.Rows[0]["采购来源"].ToString().CompareTo("采购订单") == 0)
                    {
                        //累计到货数
                        string arr_all_qty = GetDataString("select isnull(sum(iquantity),0) from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.id 
                            where iPOsID=0" + dtBody.Rows[i]["psosid"] + " and a.cBusType='普通采购'", Cmd);
                        if (float.Parse(arr_all_qty) >= float.Parse(hf_all_qty))
                        {
                            Cmd.CommandText = "update " + dbname + "..PO_Podetails set cbCloser='" + dtHead.Rows[0]["制单人"] + "',cbCloseDate=convert(varchar(10),getdate(),120) where id=0" + dtBody.Rows[i]["psosid"];
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        //累计到货数
                        string arr_all_qty = GetDataString("select isnull(sum(iquantity),0) from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.id 
                            where iPOsID=0" + dtBody.Rows[i]["psosid"] + " and a.cBusType='委外采购'", Cmd);
                        if (float.Parse(arr_all_qty) >= float.Parse(hf_all_qty))
                        {
                            Cmd.CommandText = "update " + dbname + "..OM_MODetails set cbCloser='" + dtHead.Rows[0]["制单人"] + "',dbCloseDate=convert(varchar(10),getdate(),120) where id=0" + dtBody.Rows[i]["psosid"];
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        #endregion

        if (i_has_valid_row == 0) throw new Exception("不存在合法收货记录");//没有合法到货单记录
        if (i_has_queshi_row > 0) //收料单
        {
            #region  //拒收单主表

            #endregion

            #region  //拒收单子表

            #endregion
        }

        U8Operation.GetDataString("select 'A1111'", Cmd);

        //采购入库单
        #region
        if (GetDataInt("select count(1) from " + dbname + @"..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.id 
                inner join " + dbname + @"..inventory i on b.cinvcode=i.cinvcode
                where a.id=0" + pu_id + " and isnull(i.cInvDefine1,'')='是'", Cmd) == 0)
        {
            System.Data.DataTable dtRdBody = GetSqlDataTable(@"select 0 autoid,0 id,b.autoid arrautoid,b.id arrid,b.cinvcode 货品编码,i.cinvname 货品名称,
                cast(isnull(b.iQuantity,0)-isnull(b.fValidInQuan,0) as float) 入库数,u.cComunitName 单位,isnull(i.cinvstd,'') 规格型号,
                cast(isnull(b.iQuantity,0)-isnull(b.fValidInQuan,0) as float) 原数量,0 库位库存,0 仓库库存,i.cinvccode 货品类别,a.ccode 到货单号,
                convert(varchar(10),a.ddate,120) 到货日期,cast(isnull(b.iTaxRate,0) as float) 税率,cast(isnull(b.iOriTaxCost,0) as float) 含税单价,
                b.iPOsID poautoid,b.cordercode 订单号,isnull(b.cbatch,convert(varchar(10),getdate(),120)) 批号,isnull(b.cDefine34,0) 扣点数量,b.cDefine22 扣点原因
            from " + dbname + @"..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.id
            inner join " + dbname + @"..inventory i on b.cinvcode=i.cinvcode
            inner join " + dbname + @"..ComputationUnit u on i.cComunitCode=u.cComunitCode
            where a.id=0" + pu_id + " and isnull(a.cverifier,'')<>'' and isnull(i.cInvDefine1,'')<>'是' and cast(isnull(b.iQuantity,0)-isnull(b.fValidInQuan,0) as float)>0", "dtRdBody", Cmd);

            if (dtRdBody.Rows.Count > 0)
            {
                System.Data.DataTable RdHead = new System.Data.DataTable("RdHead");
                RdHead.Columns.Add("id"); RdHead.Columns.Add("仓库编码"); RdHead.Columns.Add("供应商编码");
                RdHead.Columns.Add("制单人"); RdHead.Columns.Add("制单日期"); RdHead.Columns.Add("备注");
                RdHead.Columns.Add("入库类别"); RdHead.Columns.Add("部门编码"); RdHead.Columns.Add("销售订单");
                RdHead.Columns.Add("货位编码"); RdHead.Columns.Add("来源");
                RdHead.Rows.Add(new object[] { "", "", "", "", "", "", "", "", "", "", "" });
                RdHead.Rows[0]["id"] = "";
                RdHead.Rows[0]["仓库编码"] = dtHead.Rows[0]["仓库编码"];
                RdHead.Rows[0]["供应商编码"] = dtHead.Rows[0]["供应商"];
                RdHead.Rows[0]["制单人"] = dtHead.Rows[0]["制单人"];
                RdHead.Rows[0]["制单日期"] = dtHead.Rows[0]["制单日期"];
                RdHead.Rows[0]["备注"] = "";
                RdHead.Rows[0]["入库类别"] = GetDataString("select cRdCode from " + dbname + "..PurchaseType where cPTCode='" + dtPUHead.Rows[0]["cPTCode"] + "'", Cmd);
                RdHead.Rows[0]["部门编码"] = dtPUHead.Rows[0]["cdepcode"] + "";
                RdHead.Rows[0]["销售订单"] = "";
                RdHead.Rows[0]["货位编码"] = "";
                RdHead.Rows[0]["来源"] = "采购到货单";

                try
                {
                    cc_mcode = U8SCM_Record01_Input(RdHead, dtRdBody, dbname, "", Cmd, ref errmsg);
                }
                catch (Exception ex)
                {
                    VendorIO.WriteDebug("Pu_ASN_ByOtherVendor：step 6- U8SCM_PuASN_Add: " + ex.Message, "venlog");
                    throw ex;
                }
            }
        }
        else  //弃审到货单
        {
            Cmd.CommandText = "update " + dbname + @"..PU_ArrivalVouch set cverifier=null,cAuditDate=null where id=0" + pu_id;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        string ret_msg = "";
        if (cc_mcode.Split(',').Length > 1)
        {
            ret_msg = cc_mcode;
        }
        else
        {
            ret_msg = pu_id + "," + cc_mcode;
        }

        return ret_msg;
    }

    [WebMethod]  //U8 保存 根据ASN单 采购到货单  返回 “采购到货单ID,单据号码”[HL]
    public string U8SCM_PuArri_ASN(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            if (rd_id == 0)  //新增主表
            {
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "D" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..PU_ArrivalVouch where ccode like '" + cCodeHead + "%'", Cmd);
                pumain.ID = rd_id + "";
                pumain.cCode = "'" + cc_mcode + "'";
                pumain.iVTid = "8169";
                pumain.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                pumain.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                pumain.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                pumain.iDiscountTaxType = "0";
                pumain.IsWfControlled = "1";  //要求审批流
                pumain.iTaxRate = "13";
                //表头项目
                System.Data.DataTable dtPUHead = GetSqlDataTable("select cVenCode,cPersonCode,cPTCode,cexch_name,iExchrate nflat,iTaxRate,cpaycode cVenPUOMProtocol,cBusType from " + dbname +
                    "..PU_ASNVouch where ccode='" + dtHead.Rows[0]["订单号"] + "'", "dtPUHead", Cmd);
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
                    pumain.cDefine14 = "'" + GetDataString("select cDefine14 from " + dbname + "..PO_Pomain where cpoid='" + dtHead.Rows[0]["订单号"] + "'", Cmd) + "'";
                }
                else
                {
                    throw new Exception("订单号有错误，无法找到");
                }

                if (!pumain.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            string ASN_AutoID_List = "";
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
                if (GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bPropertyCheck=1", Cmd) > 0)
                    pudetail.bGsp = "1";

                pudetail.cBatchProperty2 = pumain.cVenCode;
                pudetail.ivouchrowno = "" + (i + 1);
                pudetail.cDefine35 = "" + dtBody.Rows[i]["autoid"];   //ASN单子表ID
                pudetail.cDefine28 = "'" + dtHead.Rows[0]["订单号"] + "'";
                if ((dtBody.Rows[i]["含税单价"] + "").ToString().CompareTo("") != 0)  //有单价
                {
                    pudetail.iOriSum = "" + (float.Parse(dtBody.Rows[i]["数量"] + "") * float.Parse(dtBody.Rows[i]["含税单价"] + ""));
                }
                //补充订单信息
                pudetail.cordercode = "'"+GetDataString("select b.cpoid from " + dbname + "..PO_Podetails a inner join " + dbname +
                    "..PO_Pomain b on a.poid=b.poid where id=0" + dtBody.Rows[i]["iPosID"], Cmd) + "'";
                pudetail.bTaxCost = "1";
                pudetail.iPOsID = dtBody.Rows[i]["iPosID"] + "";
                string cc_free = "" + GetDataString("select cfree1 from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["iPosID"], Cmd);
                pudetail.cFree1 = (cc_free.CompareTo("") == 0 ? "null" : "'" + cc_free + "'");
                cc_free = "" + GetDataString("select cfree2 from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["iPosID"], Cmd);
                pudetail.cFree2 = (cc_free.CompareTo("") == 0 ? "null" : "'" + cc_free + "'");
                pudetail.cBatchProperty6 = "'" + dtBody.Rows[i]["炉号"] + "'";
                pudetail.cBatchProperty8 = "'" + cVen_name + "'";
                pudetail.cDefine26 = "" + dtBody.Rows[i]["扫描件数"];

                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)+(0" + dtBody.Rows[i]["数量"] + ") where id=0" + dtBody.Rows[i]["iPosID"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtBody.Rows[i]["iPosID"];
                Cmd.ExecuteNonQuery();
                ////会写ASN单
                //Cmd.CommandText = "update " + dbname + "..PU_ASNVouchs set farrqty=isnull(farrqty,0)+(0" + dtBody.Rows[i]["数量"] + ") where autoid=0" + dtBody.Rows[i]["autoid"];
                //Cmd.ExecuteNonQuery();

                string asn_autoid = dtBody.Rows[i]["autoid"] + "";
                if (ASN_AutoID_List.IndexOf(asn_autoid) > -1)
                    ASN_AutoID_List = ASN_AutoID_List + "," + asn_autoid;

                if (!pudetail.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)) where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();

                

                //判定是否超 ASN单到货
                if (GetDataInt("select count(*) from " + dbname + "..PO_Podetails where id=0" + dtBody.Rows[i]["iPosID"] + " and isnull(iarrqty,0)>iquantity", Cmd) > 0) 
                    throw new Exception(pudetail.cInvCode + "超订单到货");

            }

            //校验件数
            if (ASN_AutoID_List.Length > 0)
            {
                ASN_AutoID_List = ASN_AutoID_List.Substring(1);
                string[] asnlist = ASN_AutoID_List.Split(',');
                for (int i = 0; i < asnlist.Length; i++)
                {
                    int i_pujs = GetDataInt("select sum(isnull(cDefine26,0)) from " + dbname + "..PU_ArrivalVouchs where isnull(cDefine35,0)=0" + asnlist[i], Cmd);
                    int i_asnqty = GetDataInt("select isnull(cbatch,'0') from " + dbname + "..PU_ASNVouchs where autoid=0" + asnlist[i], Cmd);
                    if (i_pujs != i_asnqty) throw new Exception("件数不匹配");
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


    #endregion

    #region   //其他入库单
    [WebMethod]  //U8 保存 其他入库单  返回 “其他入库单ID,单据号码”[MX]
    public string U8SCM_RDS_08_MX_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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

                //record08.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record08.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record08.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record08.cRdCode = "'" + dtHead.Rows[0]["出库类别"] + "'";
                record08.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record08.iExchRate = "1";
                record08.cExch_Name = "'人民币'";

                record08.cSource = "'库存'";
                if (!record08.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }

                KK_U8Com.U8Rdrecords08 records08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);

                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records08.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records08.ID = rd_id;
                records08.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records08.iQuantity = "" + dtBody.Rows[i]["数量"];
                if (iPosSet > 0) records08.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
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
                int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + record08.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) records08.bCosting = "0";

                //保存数据
                if (!records08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                if (iPosSet > 0)
                {
                    //添加货位记录
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'08','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
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

                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" + dtHead.Rows[0]["仓库编码"] +
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



    #endregion

    #region   //其他出库单
    [WebMethod]  //U8 保存 其他出库单  返回 “其他出库单ID,单据号码”[MX]
    public string U8SCM_RDS_09_MX_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
                if (targetAccId.CompareTo("201") == 0)
                {
                    record09.VT_ID = 131395;
                }
                else
                {
                    record09.VT_ID = 85;
                }

                if (targetAccId.CompareTo("206") == 0)
                {
                    record09.cDefine11 = "'" + dtHead.Rows[0]["领用人"] + "'";  //领料人
                }
                else
                {
                    record09.cDefine3 = "'" + dtHead.Rows[0]["领用人"] + "'";  //领料人
                }
                
                //record09.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record09.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";

                record09.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
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

                int cAutoid = 1000000000 + int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
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
                records09.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";
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

                int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + record09.cWhCode + " and bInCost=1", Cmd);
                if (ibcostcount == 0) records09.bCosting = "0";

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

    #endregion

    #region   //产品入库单
    [WebMethod]  //U8 保存 产品入库单  返回 “产品入库单ID,单据号码”[KL]
    public string U8SCM_RDS_10_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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

            KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
            int iPosSet = 0;
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
                rd_id = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                string cCodeHead = "G" + GetDataString("select right(replace(convert(varchar(10),'" + dtHead.Rows[0]["制单日期"] + "',120),'-',''),6)", Cmd); ;
                cc_mcode = cCodeHead + GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord10 where ccode like '" + cCodeHead + "%'", Cmd);
                record10.cCode = "'" + cc_mcode + "'";
                record10.ID = rd_id;
                record10.cVouchType = "'10'";
                record10.cWhCode = "'" + dtHead.Rows[0]["仓库编码"] + "'";
                record10.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
                record10.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                //record10.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
                //record10.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
                record10.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
                record10.cRdCode = "'" + dtHead.Rows[0]["入库类别"] + "'";
                record10.cMemo = "'" + dtHead.Rows[0]["备注"] + "'";
                record10.cDefine1 = "'" + dtHead.Rows[0]["班次"] + "'";
                record10.cDefine2 = "'" + dtHead.Rows[0]["操作员"] + "'";
                record10.iExchRate = "1";
                record10.cExch_Name = "'人民币'";
                record10.cSource = "'库存'";
                if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
            }

            int irowno = 0;
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                if (iPosSet > 0)
                {
                    if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                    { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
                }
                KK_U8Com.U8Rdrecords10 records10 = new KK_U8Com.U8Rdrecords10(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
                records10.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records10.ID = rd_id;
                records10.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                records10.iQuantity = "" + dtBody.Rows[i]["数量"];
                records10.irowno = (irowno++);
                records10.iNQuantity = records10.iQuantity;
                records10.iordertype = "0";
                if (iPosSet > 0) records10.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                records10.cBatch = "'" + dtBody.Rows[i]["批号"] + "'";

                //保存数据
                if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                if (iPosSet > 0)
                {
                    //一个批号只能存放于一个货位
                    string cOld_position = GetDataString("select top 1 cposcode from " + dbname + @"..InvPosition 
                        where cinvcode=" + records10.cInvCode + " and cbatch=" + records10.cBatch + " and brdflag=1 order by dvouchDate desc", Cmd);
                    if (cOld_position.CompareTo("") != 0 && cOld_position.CompareTo("" + dtBody.Rows[i]["区域"]) != 0)
                        throw new Exception("批号【" + dtBody.Rows[i]["批号"] + "】只能存放于货位【" + cOld_position + "】");

                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler) " +
                        "Values (" + cAutoid + "," + rd_id + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                        ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'10','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "')";
                    Cmd.ExecuteNonQuery();

                    ////指定货位
                    //Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1 where autoid =0" + cAutoid;
                    //Cmd.ExecuteNonQuery();

                    //修改货位库存
                    if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" +
                        dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
                        '','','','','','','','','','','','',0,0)";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" +
                        dtHead.Rows[0]["仓库编码"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
                    Cmd.ExecuteNonQuery();
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
            CloseDataConnection(Conn);
        }


    }

    #endregion

    #region   //调拨单
    [WebMethod]  //U8 保存 调拨单  返回 “调拨单ID,单据号码”[KL]
    public string U8SCM_Trans_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            string ret_value = U8SCM_Trans_Input(dtHead, dtBody, dtWidth, dbname, Cmd);

            tr.Commit();
            return ret_value;
            
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
    public string U8SCM_Trans_Input(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname,System.Data.SqlClient.SqlCommand Cmd)
    {
        string errmsg = "";
        int rd_id = int.Parse("0" + dtHead.Rows[0]["id"]);
        string cc_mcode = "";

        string dCurDate = GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
        string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        KK_U8Com.U8TransVouch dbmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
        KK_U8Com.U8Rdrecord08 rd08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
        KK_U8Com.U8Rdrecord09 rd09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);
        int iPosSet09 = 0;
        int iPosSet08 = 0;
        int iVmiSet = 0;
        int iVmiSet_In = 0;
        if (rd_id == 0)  //新增主表
        {
            //iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
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
            dbmain.cPersonCode = (dtHead.Rows[0]["业务员"] + "" == "" ? "null" : "'" + dtHead.Rows[0]["业务员"] + "'");
            if (!dbmain.InsertToDB(targetAccId, false, ref errmsg)) { throw new Exception(errmsg); }

            #region //审核调拨单 形成其他入库和其他出库单
            //新增其他出库单主表
            iPosSet09 = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
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
            rd09.VT_ID = 85;// 131395;
            if (!rd09.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }


            //新增其他入库单主表
            iPosSet08 = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["调入仓库"] + "' and isnull(bWhPos,0)=1", Cmd));
            rd08.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rd08.bredvouch = "0";
            rd08.cWhCode = "'" + dtHead.Rows[0]["调入仓库"] + "'";
            rd08.cMaker = "'" + dtHead.Rows[0]["制单人"] + "'";
            rd08.dDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
            //rd08.cHandler = "'" + dtHead.Rows[0]["制单人"] + "'";
            //rd08.dVeriDate = "'" + dtHead.Rows[0]["制单日期"] + "'";
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
            if (iPosSet09 > 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
                { throw new Exception("库位编码【" + dtBody.Rows[0]["区域"] + "】不存在"); }
            }
            if (iPosSet08 > 0)
            {
                if (GetDataInt("select count(*) from " + dbname + "..Position where cwhcode='" + dtHead.Rows[0]["调入仓库"] + "' and cPosCode='" + dtBody.Rows[0]["区域"] + "'", Cmd) == 0)
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
            string cccbatch = GetDataString("SELECT case when bInvBatch=1 then '" + dtBody.Rows[i]["批号"] + "' else '' end FROM " + dbname + "..Inventory where cinvcode=" + dbdetail.cInvCode, Cmd);
            dbdetail.cTVBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");


            if (iPosSet09 > 0) dbdetail.coutposcode = "'" + dtBody.Rows[i]["区域"] + "'";
            if (iPosSet08 > 0) dbdetail.cinposcode = "'" + dtBody.Rows[i]["区域"] + "'";
            dbdetail.bCosting = 0;


            //代管仓库判定
            if (iVmiSet > 0)
            {
                if (dtBody.Rows[i]["供应商"] + "" == "")
                {
                    throw new Exception("代管物资必须 输入供应商信息");
                }
                dbdetail.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
            }

            if (!dbdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


            //写入其他出库单
            #region
            KK_U8Com.U8Rdrecords09 rds09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);
            rds09.AutoID = 1000000000 + int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();
            rds09.ID = rd09.ID;
            rds09.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
            rds09.iQuantity = "" + dtBody.Rows[i]["数量"];
            rds09.iNQuantity = rds09.iQuantity;
            rds09.irowno = (i + 1);
            rds09.cBatch = (cccbatch + "" == "" ? "null" : "'" + cccbatch + "'");
            rds09.iTrIds = "" + dbdetail.AutoID;
            rds09.cDefine24 = "'AUTOINPUT'";

            //代管仓库判定
            if (iVmiSet > 0)
            {
                if (dtBody.Rows[i]["供应商"] + "" == "")
                {
                    throw new Exception("代管物资必须 输入供应商信息");
                }
                rds09.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                rds09.bVMIUsed = "1";
            }
            int ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd09.cWhCode + " and bInCost=1", Cmd);
            if (ibcostcount == 0) rds09.bCosting = "0";

            if (iPosSet09 > 0) rds09.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
            rds09.isotype = "0";
            if (!rds09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }


            if (iPosSet09 > 0)
            {
                //添加货位记录
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,iExpiratdatecalcu,cMassUnit) " +
                    "Values (" + rds09.AutoID + "," + rds09.ID + ",'" + dtHead.Rows[0]["仓库编码"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                    ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',0,'',0,'09','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "',0,0)";
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
            if (iVmiSet_In > 0)
            {
                if (dtBody.Rows[i]["供应商"] + "" == "")
                {
                    throw new Exception("代管物资必须 输入供应商信息");
                }
                rds08.cvmivencode = "'" + dtBody.Rows[i]["供应商"] + "'";
                rds08.bVMIUsed = "1";
                rds08.bCosting = "0";
            }

            ibcostcount = GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode=" + rd08.cWhCode + " and bInCost=1", Cmd);
            if (ibcostcount == 0) rds08.bCosting = "0";

            if (iPosSet08 > 0) rds08.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
            if (!rds08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            if (iPosSet08 > 0)
            {
                //添加货位记录
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,
                    cmemo,cbatch,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,iExpiratdatecalcu,cMassUnit) " +
                    "Values (" + rds08.AutoID + "," + rds08.ID + ",'" + dtHead.Rows[0]["调入仓库"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0" + dtBody.Rows[i]["数量"] +
                    ",null,'" + dtBody.Rows[i]["批号"] + "','" + dtHead.Rows[0]["制单日期"] + "',1,'',0,'08','" + dtHead.Rows[0]["制单日期"] + "','" + dtHead.Rows[0]["制单人"] + "',0,0)";
                Cmd.ExecuteNonQuery();

                ////指定货位
                //Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                //Cmd.ExecuteNonQuery();

                //修改货位库存
                if (GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + dtHead.Rows[0]["调入仓库"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,iexpiratdatecalcu) 
                        values('" + dtHead.Rows[0]["调入仓库"] + "','" + dtBody.Rows[i]["区域"] + "','" + dtBody.Rows[i]["存货编码"] + "',0,'" + dtBody.Rows[i]["批号"] + @"',
                        '','','','','','','','','','','','',0,0)";
                    Cmd.ExecuteNonQuery();
                }

                Cmd.CommandText = "update " + dbname + @"..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtBody.Rows[i]["数量"] + ") where cwhcode='" + dtHead.Rows[0]["调入仓库"] +
                    "' and cinvcode='" + dtBody.Rows[i]["存货编码"] + "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "'";
                Cmd.ExecuteNonQuery();

                //判定负库存问题
                if (GetDataInt("select count(*) from " + dbname + @"..InvPositionSum where cwhcode='" + dtHead.Rows[0]["调入仓库"] + "' and cinvcode='" + dtBody.Rows[i]["存货编码"] +
                    "' and cbatch='" + dtBody.Rows[i]["批号"] + "' and cPosCode='" + dtBody.Rows[i]["区域"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                {
                    throw new Exception("【" + dtBody.Rows[i]["存货编码"] + "】出现负库存");
                }
            }

            #endregion


        }

        return rd_id + "," + cc_mcode;
    }

    #endregion

    #region   //盘点单
    [WebMethod]  //U8 保存 盘点单  返回 “盘点单ID,单据号码”[KL]
    public string U8SCM_Check_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
            if (rd_id == 0)  //新增主表
            {
                iPosSet = int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtHead.Rows[0]["仓库编码"] + "' and isnull(bWhPos,0)=1", Cmd));
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
                check.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
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
                KK_U8Com.U8CheckVouchs checks = new KK_U8Com.U8CheckVouchs(Cmd, dbname);
                int cAutoid = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='ch'", Cmd));
                checks.autoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='ch'";
                Cmd.ExecuteNonQuery();

                checks.ID = rd_id;
                checks.cCVCode = "'" + cc_mcode + "'";
                checks.cInvCode = "'" + dtBody.Rows[i]["存货编码"] + "'";
                checks.cPosition = "'" + dtBody.Rows[i]["区域"] + "'";
                checks.cCVBatch = "'" + dtBody.Rows[i]["批号"] + "'";
                checks.iCVQuantity = "" + dtBody.Rows[i]["库存数"]; //帐目数
                checks.iCVCQuantity = "" + dtBody.Rows[i]["数量"];   //盘点数量
                checks.irowno = ""+(i + 1);
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


    [WebMethod]  //U8 保存 盘点单  返回 “盘点单ID,单据号码”[MeiXin]
    public string U8SCM_MxCheck_OneSheet(System.Data.DataTable dtHead, System.Data.DataTable dtBody, System.Data.DataTable dtWidth, string dbname, string SN)
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
                if (targetAccId.CompareTo("201")==0) check.VT_ID = "131382";
                check.cDepCode = "'" + dtHead.Rows[0]["部门编码"] + "'";
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

    #endregion


    #endregion  //KL
    [WebMethod]  //产品入库上架
    public bool U8SCM_rd10_Pos(string cc_rd_autoid, string cusername,System.Data.DataTable dtPosList,System.Data.DataTable GridWidth, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            System.Data.DataTable records10 = GetSqlDataTable(@"select a.cwhcode,b.cinvcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                        cAssUnit,dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate,b.iquantity,b.id
                    from " + dbname + @"..rdrecords10 b inner join " + dbname + @"..rdrecord10 a on a.id=b.id where autoid=" + cc_rd_autoid, "records10",Cmd);
            if (records10.Rows.Count == 0) throw new Exception("没有找到原始 入库记录");

            if (dtPosList.Rows.Count == 1)
            {
                Cmd.CommandText = "update " + dbname + "..rdrecords10 set cPosition='" + dtPosList.Rows[0]["货位条码"] + "' where autoid =0" + cc_rd_autoid;
                Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dtPosList.Rows.Count; i++)
            {
                //货位检查
                if (GetDataInt("select count(*) from " + dbname + "..Position where cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cWhCode='" + records10.Rows[0]["cwhcode"] + "'", Cmd) == 0)
                    throw new Exception("货位非法，或对应仓库错误");

                float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1 where autoid =0" + cc_rd_autoid;
                    Cmd.ExecuteNonQuery();
                }

                string f_num = GetDataString("select round(" + dtPosList.Rows[i]["数量"] + @"/iquantity*inum,5) inum from " + dbname + @"..rdrecords10 where autoid=" + cc_rd_autoid, Cmd);
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                        cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cvmivencode,cbatch,cHandler,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    select b.autoid,b.id,a.cwhcode,'" + dtPosList.Rows[i]["货位条码"] + @"',b.cinvcode," + dtPosList.Rows[i]["数量"] + ",0" + f_num + @",
                        cmemo,ddate,brdflag,'',0,cvouchtype,a.dDate,isnull(cvmivencode,''),isnull(cbatch,''),'" + cusername + @"',
                        isnull(cfree1,''),isnull(cfree2,''),isnull(cfree3,''),isnull(cfree4,''),isnull(cfree5,''),
                        isnull(cfree6,''),isnull(cfree7,''),isnull(cfree8,''),isnull(cfree9,''),isnull(cfree10,''),cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate 
                    from " + dbname + @"..rdrecords10 b inner join " + dbname + @"..rdrecord10 a on a.id=b.id where autoid=" + cc_rd_autoid;
                Cmd.ExecuteNonQuery();
                

                //修改货位库存
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + records10.Rows[0]["cwhcode"] + "' and cvmivencode='" + records10.Rows[0]["cvmivencode"] + @"' 
                    and cinvcode='" + records10.Rows[0]["cinvcode"] + @"' and cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cbatch='" + records10.Rows[0]["cbatch"] + @"' 
                    and cfree1='" + records10.Rows[0]["cfree1"] + "' and cfree2='" + records10.Rows[0]["cfree2"] + "' and cfree3='" + records10.Rows[0]["cfree3"] + @"' 
                    and cfree4='" + records10.Rows[0]["cfree4"] + "' and cfree5='" + records10.Rows[0]["cfree5"] + "' and cfree6='" + records10.Rows[0]["cfree6"] + @"'
                    and cfree7='" + records10.Rows[0]["cfree7"] + "' and cfree8='" + records10.Rows[0]["cfree8"] + "' and cfree9='" + records10.Rows[0]["cfree9"] + @"' 
                    and cfree10='" + records10.Rows[0]["cfree10"]+"' ", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    values('" + records10.Rows[0]["cwhcode"] + "','" + dtPosList.Rows[i]["货位条码"] + "','" + records10.Rows[0]["cinvcode"] + "',0,'" + records10.Rows[0]["cbatch"] + @"',
                        '" + records10.Rows[0]["cfree1"] + "','" + records10.Rows[0]["cfree2"] + "','" + records10.Rows[0]["cfree3"] + "','" + records10.Rows[0]["cfree4"] + "','" + records10.Rows[0]["cfree5"] + "','" +
                        records10.Rows[0]["cfree6"] + "','" + records10.Rows[0]["cfree7"] + "','" + records10.Rows[0]["cfree8"] + "','" + records10.Rows[0]["cfree9"] + "','" + records10.Rows[0]["cfree10"] + "','" + records10.Rows[0]["cvmivencode"] + @"','',0,
                        " + (records10.Rows[0]["dMadeDate"] + "" == "" ? "null" : "'" + records10.Rows[0]["dMadeDate"] + "'") + "," + (records10.Rows[0]["dMadeDate"] + "" == "" ? "null" : "'" + records10.Rows[0]["iMassDate"] + "'") +
                          "," + (records10.Rows[0]["cMassUnit"] + "" == "" ? "null" : "'" + records10.Rows[0]["cMassUnit"] + "'") + "," + (records10.Rows[0]["iExpiratDateCalcu"] + "" == "" ? "null" : "'" + records10.Rows[0]["iExpiratDateCalcu"] + "'") + "," +
                            (records10.Rows[0]["cExpirationdate"] + "" == "" ? "null" : "'" + records10.Rows[0]["cExpirationdate"] + "'") +
                            "," + (records10.Rows[0]["dExpirationdate"] + "" == "" ? "null" : "'" + records10.Rows[0]["dExpirationdate"] + "'") + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + dtPosList.Rows[i]["数量"] + "),inum=isnull(inum,0)+(0" + f_num + @") 
                    where cwhcode='" + records10.Rows[0]["cwhcode"] + "' and cvmivencode='" + records10.Rows[0]["cvmivencode"] + @"' 
                    and cinvcode='" + records10.Rows[0]["cinvcode"] + @"' and cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cbatch='" + records10.Rows[0]["cbatch"] + @"' 
                    and cfree1='" + records10.Rows[0]["cfree1"] + "' and cfree2='" + records10.Rows[0]["cfree2"] + "' and cfree3='" + records10.Rows[0]["cfree3"] + @"' 
                    and cfree4='" + records10.Rows[0]["cfree4"] + "' and cfree5='" + records10.Rows[0]["cfree5"] + "' and cfree6='" + records10.Rows[0]["cfree6"] + @"' 
                    and cfree7='" + records10.Rows[0]["cfree7"] + "' and cfree8='" + records10.Rows[0]["cfree8"] + "' and cfree9='" + records10.Rows[0]["cfree9"] + @"' 
                    and cfree10='" + records10.Rows[0]["cfree10"]+"'";
                Cmd.ExecuteNonQuery();

            }
            //交验数量
            float fpos_all = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + dbname + @"..InvPosition where rdsid=" +
                cc_rd_autoid + " and rdid=" + records10.Rows[0]["id"], Cmd));
            if (fpos_all != float.Parse("" + records10.Rows[0]["iquantity"])) throw new Exception("上架数量与入库数量不一致");

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
        

    }

    [WebMethod]  //其他出库单 下架
    public bool U8SCM_rd09_Pos(string cc_rd_autoid, string cusername, System.Data.DataTable dtPosList, System.Data.DataTable GridWidth, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            System.Data.DataTable records10 = GetSqlDataTable(@"select a.cwhcode,b.cinvcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                        cAssUnit,dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate,b.iquantity,b.id
                    from " + dbname + @"..rdrecords09 b inner join " + dbname + @"..rdrecord09 a on a.id=b.id where autoid=" + cc_rd_autoid, "records10",Cmd);
            if (records10.Rows.Count == 0) throw new Exception("没有找到原始 入库记录");

            if (dtPosList.Rows.Count == 1)
            {
                Cmd.CommandText = "update " + dbname + "..rdrecords09 set cPosition='" + dtPosList.Rows[0]["货位条码"] + "' where autoid =0" + cc_rd_autoid;
                Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dtPosList.Rows.Count; i++)
            {
                //货位检查
                if (GetDataInt("select count(*) from " + dbname + "..Position where cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cWhCode='" + records10.Rows[0]["cwhcode"] + "'", Cmd) == 0)
                    throw new Exception("货位非法，或对应仓库错误");

                float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords09 set iposflag=1 where autoid =0" + cc_rd_autoid;
                    Cmd.ExecuteNonQuery();
                }

                string f_num = GetDataString("select round(" + dtPosList.Rows[i]["数量"] + @"/iquantity*inum,5) inum from " + dbname + @"..rdrecords09 where autoid=" + cc_rd_autoid, Cmd);
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                        cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cvmivencode,cbatch,cHandler,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    select b.autoid,b.id,a.cwhcode," + dtPosList.Rows[i]["货位条码"] + @",b.cinvcode," + dtPosList.Rows[i]["数量"] + ",0" + f_num + @",
                        cmemo,ddate,brdflag,'',0,cvouchtype,a.dDate,isnull(cvmivencode,''),isnull(cbatch,''),'" + cusername + @"',
                        isnull(cfree1,''),isnull(cfree2,''),isnull(cfree3,''),isnull(cfree4,''),isnull(cfree5,''),
                        isnull(cfree6,''),isnull(cfree7,''),isnull(cfree8,''),isnull(cfree9,''),isnull(cfree10,''),cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate 
                    from " + dbname + @"..rdrecords09 b inner join " + dbname + @"..rdrecord09 a on a.id=b.id where autoid=" + cc_rd_autoid;
                Cmd.ExecuteNonQuery();


                //修改货位库存
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + records10.Rows[0]["cwhcode"] + "' and cvmivencode='" + records10.Rows[0]["cvmivencode"] + @"' 
                    and cinvcode='" + records10.Rows[0]["cinvcode"] + @"' and cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cbatch='" + records10.Rows[0]["cbatch"] + @"' 
                    and cfree1='" + records10.Rows[0]["cfree1"] + "' and cfree2='" + records10.Rows[0]["cfree2"] + "' and cfree3='" + records10.Rows[0]["cfree3"] + @"' 
                    and cfree4='" + records10.Rows[0]["cfree4"] + "' and cfree5='" + records10.Rows[0]["cfree5"] + "' and cfree6='" + records10.Rows[0]["cfree6"] + @"' 
                    and cfree7='" + records10.Rows[0]["cfree7"] + "' and cfree8='" + records10.Rows[0]["cfree8"] + "' and cfree9='" + records10.Rows[0]["cfree9"] + @"'
                    and cfree10='" + records10.Rows[0]["cfree10"]+"'", Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    values('" + records10.Rows[0]["cwhcode"] + "','" + dtPosList.Rows[i]["货位条码"] + "','" + records10.Rows[0]["cinvcode"] + "',0,'" + records10.Rows[0]["cbatch"] + @"',
                        '" + records10.Rows[0]["cfree1"] + "','" + records10.Rows[0]["cfree2"] + "','" + records10.Rows[0]["cfree3"] + "','" + records10.Rows[0]["cfree4"] + "','" + records10.Rows[0]["cfree5"] + "','" +
                        records10.Rows[0]["cfree6"] + "','" + records10.Rows[0]["cfree7"] + "','" + records10.Rows[0]["cfree8"] + "','" + records10.Rows[0]["cfree9"] + "','" + records10.Rows[0]["cfree10"] + "','" + records10.Rows[0]["cvmivencode"] + @"','',0,
                        " + (records10.Rows[0]["dMadeDate"] + "" == "" ? "null" : "'" + records10.Rows[0]["dMadeDate"] + "'") + "," + (records10.Rows[0]["dMadeDate"] + "" == "" ? "null" : "'" + records10.Rows[0]["iMassDate"] + "'") +
                          "," + (records10.Rows[0]["cMassUnit"] + "" == "" ? "null" : "'" + records10.Rows[0]["cMassUnit"] + "'") + "," + (records10.Rows[0]["iExpiratDateCalcu"] + "" == "" ? "null" : "'" + records10.Rows[0]["iExpiratDateCalcu"] + "'") + "," +
                            (records10.Rows[0]["cExpirationdate"] + "" == "" ? "null" : "'" + records10.Rows[0]["cExpirationdate"] + "'") +
                            "," + (records10.Rows[0]["dExpirationdate"] + "" == "" ? "null" : "'" + records10.Rows[0]["dExpirationdate"] + "'") + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + dtPosList.Rows[i]["数量"] + "),inum=isnull(inum,0)-(0" + f_num + @") 
                    where cwhcode='" + records10.Rows[0]["cwhcode"] + "' and cvmivencode='" + records10.Rows[0]["cvmivencode"] + @"' 
                    and cinvcode='" + records10.Rows[0]["cinvcode"] + @"' and cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cbatch='" + records10.Rows[0]["cbatch"] + @"' 
                    and cfree1='" + records10.Rows[0]["cfree1"] + "' and cfree2='" + records10.Rows[0]["cfree2"] + "' and cfree3='" + records10.Rows[0]["cfree3"] + @"' 
                    and cfree4='" + records10.Rows[0]["cfree4"] + "' and cfree5='" + records10.Rows[0]["cfree5"] + "' and cfree6='" + records10.Rows[0]["cfree6"] + @"' 
                    and cfree7='" + records10.Rows[0]["cfree7"] + "' and cfree8='" + records10.Rows[0]["cfree8"] + "' and cfree9='" + records10.Rows[0]["cfree9"] + @"' 
                    and cfree10='" + records10.Rows[0]["cfree10"] + "'";
                Cmd.ExecuteNonQuery();

                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode='" + records10.Rows[0]["cwhcode"] + "' and cvmivencode='" + records10.Rows[0]["cvmivencode"] + @"' 
                    and cinvcode='" + records10.Rows[0]["cinvcode"] + @"' and cPosCode='" + dtPosList.Rows[i]["货位条码"] + "' and cbatch='" + records10.Rows[0]["cbatch"] + @"' 
                    and cfree1='" + records10.Rows[0]["cfree1"] + "' and cfree2='" + records10.Rows[0]["cfree2"] + "' and cfree3='" + records10.Rows[0]["cfree3"] + @"' 
                    and cfree4='" + records10.Rows[0]["cfree4"] + "' and cfree5='" + records10.Rows[0]["cfree5"] + "' and cfree6='" + records10.Rows[0]["cfree6"] + @"' 
                    and cfree7='" + records10.Rows[0]["cfree7"] + "' and cfree8='" + records10.Rows[0]["cfree8"] + "' and cfree9='" + records10.Rows[0]["cfree9"] + @"'
                    and cfree10='" + records10.Rows[0]["cfree10"] + "' and isnull(iquantity,0)<0", Cmd) > 0)
                    throw new Exception("货位[" + dtPosList.Rows[i]["货位条码"] + "]库存不够");
            }
            //交验数量
            float fpos_all = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + dbname + @"..InvPosition where rdsid=" +
                cc_rd_autoid + " and rdid=" + records10.Rows[0]["id"], Cmd));
            if (fpos_all != float.Parse("" + records10.Rows[0]["iquantity"])) throw new Exception("上架数量与入库数量不一致");

            Cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
        

        
    }


    #endregion  //供应链

    #region  //自动匹配批号（按照批号顺序)   返回格式：批号，货位，数量，有效期
    private System.Data.DataTable GetBatDTFromWare(string cwhcode,string cinvcode,float iOutQty,bool bpos,System.Data.SqlClient.SqlCommand sqlcmd,string db_name)
    {
        System.Data.DataTable dtbatlist = null;
        if (bpos)
        {
            dtbatlist = GetSqlDataTable(@"select cbatch,cPosCode cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,
	                iMassDate 保质期天数,cMassUnit 保质期单位,dMadeDate 生产日期,dVDate 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,dExpirationdate 有效期计算项
                from " + db_name + @"..InvPositionSum
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and isnull(iquantity,0)>0 order by cbatch", "dtbatlist", sqlcmd);
        }
        else
        {
            dtbatlist = GetSqlDataTable(@"select cbatch,'' cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,
	                iMassDate 保质期天数,cMassUnit 保质期单位,dMdate 生产日期,dVDate 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,dExpirationdate 有效期计算项
                from " + db_name + @"..CurrentStock
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and isnull(iquantity,0)>0 order by cbatch", "dtbatlist", sqlcmd);
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
                iOutQty=0;
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
                dr[ii] = dtbatlist.Rows[c][ii]+"";
            }
            dr["iquantity"] = fMerOut + "";
            dtRet.Rows.Add(dr);
            if (iOutQty <= 0) break;
        }

        if (iOutQty > 0) throw new Exception("可用批次库存不够");
        return dtRet;
    }
    #endregion

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

    [WebMethod(Description = "获得更新文件版本")]
    public string GetVersion()
    {
        return "" + System.Configuration.ConfigurationSettings.AppSettings["appVersion"];
    }

    
    #endregion

}
