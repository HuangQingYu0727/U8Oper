﻿using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// MESOperation 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class MESOperation : System.Web.Services.WebService
{

    public MESOperation()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    #region  //公共方法

    [WebMethod]
    public bool SendBaseImage(string cinvcode, byte[] bImage, string filename, string acc_id)
    {
        FileStream fs = null;
        try
        {
            string st_filepath = System.Configuration.ConfigurationManager.AppSettings["BaseImagePath"];
            st_filepath = st_filepath + "\\" + acc_id + "\\";
            if (!Directory.Exists(st_filepath)) Directory.CreateDirectory(st_filepath);
            st_filepath = st_filepath + filename;
            if (File.Exists(st_filepath)) File.Delete(st_filepath);
            fs = new FileStream(st_filepath, FileMode.Create);
            ////将字符数组转换为正确的字节格式
            //System.Drawing.Imaging.Encoder enc = System.Encoding.UTF8.GetEncoder();
            //enc.GetBytes(bImage, 0, bImage.Length, byData, 0, true);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bImage, 0, bImage.Length);
            fs.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return true;
    }

    [WebMethod]
    public byte[] GetBaseImage(string acc_id, string filename)
    {
        FileStream fs = null;
        string st_filepath = System.Configuration.ConfigurationManager.AppSettings["BaseImagePath"];

        st_filepath = st_filepath + "\\" + acc_id + "\\" + filename;
        if (File.Exists(st_filepath))
        {
            try
            {
                ///打开现有文件以进行读取。
                fs = File.OpenRead(st_filepath);
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

    [WebMethod]
    public bool CHeckValid()
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000))
            return false;
        else
            return true;
    }


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

    private string[] GetTextsFrom_FormData(System.Data.DataTable formData, string txt_fieldname)
    {
        string[] data = null;

        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null)
        {
            data = new string[4];
            data[0] = "" + DR[0];
            data[1] = "" + DR[1];
            data[2] = "" + DR[2];
            data[3] = "" + DR[3];
        }
        return data;
    }

    private string GetTextsFrom_FormData_Text(System.Data.DataTable formData, string txt_fieldname)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null) return "" + DR[3];
        return "";
    }

    private string GetTextsFrom_FormData_Tag(System.Data.DataTable formData, string txt_fieldname)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null) return "" + DR[2];
        return "";
    }

    //获得单据体具体栏目 值
    private string GetBodyValue_DTData(DataTable bodydata, int irow, string colname)
    {
        if (bodydata.Columns.Contains(colname))
        {
            return "" + bodydata.Rows[irow][colname];
        }
        else
        {
            return "";
        }
    }

    //修改原来的值
    private System.Data.DataTable SetTextsFrom_FromData(System.Data.DataTable formData, string txt_fieldname, string txt_tag, string txt_text)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        if (DR != null)
        {
            DR[2] = txt_tag;
            DR[3] = txt_text;
        }
        return formData;
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
        //获得用户
        string cpsncode = GetDataString("select cPsn_Num from " + cDataName + "..UserHrPersonContro where cUser_Id='" + usercode + "'", Conn);
        CloseDataConnection(Conn);
        return cUserName + "," + cDataName + "," + cSn + "," + cpsncode;

    }

    [WebMethod]  //U8用户验证,json 模式
    public string U8UserLoginDate_json(string usercode, string pwd, string cacc_id, string clogdate)
    {
        string cErrMsg = "";
        try
        {
            string cRet = U8UserLoginDate(usercode, pwd, cacc_id, clogdate, ref cErrMsg);
            string r_msg = "";
            if (cRet == "")
            {
                r_msg = VendorIO.SendResult("0", cErrMsg);
            }
            else
            {
                r_msg = VendorIO.SendResult("1", cRet);  //用户名,数据库名,动态序列号
            }
            return r_msg;
        }
        catch (Exception ex)
        {
            return VendorIO.SendResult("0", ex.Message);
        }
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
    public string U8PersonName(string cPsnCode, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");

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
        if (cName + "" == "") throw new Exception("无此人，请在U8中查看业务员档案");
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
    public System.Data.DataTable GetSqlDataTable(string sql, string tblName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) throw new Exception("数据库连接失败！");

        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds, tblName);

        CloseDataConnection(Conn);
        if (ds.Tables.Count == 0) throw new Exception("没有查询出数据");

        return ds.Tables[tblName];
    }

    [WebMethod]  //查询外部数据，返回数据集
    public System.Data.DataTable GetSqlDataTableOut(string sql, string tblName, string OutDBName)
    {
        System.Data.SqlClient.SqlConnection Conn = new System.Data.SqlClient.SqlConnection();
        Conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[OutDBName].ConnectionString;
        try
        {
            Conn.Open();
        }
        catch
        {
            Conn = null;
        }

        if (Conn == null) throw new Exception("数据库连接失败！");

        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(sql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds, tblName);

        CloseDataConnection(Conn);
        if (ds.Tables.Count == 0) throw new Exception("没有查询出数据");

        return ds.Tables[tblName];
    }


    [WebMethod]  //查询返回字符串数据
    public string GetSqlString(string sql)
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

    #region  //CQCC MES 供应链 对应功能(含管理U8的功能)$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    [WebMethod]  //MES 扫描MES流转卡  入库
    public string U8SCM_MOMOrder_TO_RD10_JSON(string FormDataJSON, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        DataTable FormData = VendorIO.JsonToDataTable(FormDataJSON);
        return U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, SheetID);
    }
    [WebMethod]  //MES 扫描MES流转卡  入库
    public string U8SCM_MOMOrder_TO_RD10(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
            if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
            string ccc_flowno = GetTextsFrom_FormData_Text(FormData, "txt_cardno");
            string ccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");
            if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

            if (GetDataInt(@"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + ccc_flowno + "' and ISNULL(a.CloseUser,'')<>''", Cmd) > 0)
                throw new Exception("生产订单已经关闭");
            //质量确认与废品入库的判断
            if (ccc_intype.Split(',')[0] == "2")
            {
                CheckIsCanQualityRd(ccc_cardno,dbname,Cmd);
            }
            //通过存货自定义项控制是否允许合格品通过流转卡扫码入库
            if (ccc_intype.Split(',')[0] == "1")
            {
                CheckInvNoCardRdIn(ccc_cardno, dbname, Cmd);
            }
            //检查入库单的日期和最后一次报工时间  检查入库单仓库和仓库上配置的入库类型
            CheckRdDateAndType(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string cRet = MES_OR_U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string[] cRdInfo = cRet.Split(',');  //入库单ID，入库单单号，入库单子表标识
            //建立流转卡和 入库单的关联关系 
            try
            {
                WriteFlowDataToRd10(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname, ccc_flowno);
            }
            catch (Exception ee)
            {
                throw ee;
            }

            Cmd.Transaction.Commit();

            return cRdInfo[0] + "," + cRdInfo[1];
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
    /// <summary>
    /// 按照参数配置
    /// 检查入库单的日期和最后一次报工时间
    /// 检查入库单仓库和仓库上配置的入库类型
    /// </summary>
    /// <param name="FormData"></param>
    /// <param name="dbname"></param>
    /// <param name="cUserName"></param>
    /// <param name="cLogDate"></param>
    /// <param name="SheetID"></param>
    /// <param name="Cmd"></param>
    private void CheckRdDateAndType(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID, System.Data.SqlClient.SqlCommand Cmd)
    {
        string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
        if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
        string ccccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");   //交库类型
        if (ccccc_intype == "") throw new Exception("交库类型不能为空！");
        string mes_rd_checkRdDateWithLastReport = GetDataString("select cvalue from "+dbname+"..t_parameter where cpid='mes_rd_checkRdDateWithLastReport'", Cmd);
        if (mes_rd_checkRdDateWithLastReport == "是")
        {
            string lastDate = GetDataString(@"
select CONVERT(varchar(10),max(rpt.t_maketime),120) from " + dbname + @"..T_CC_Card_List a 
inner join " +dbname+@"..T_CC_Cards_process b on a.t_card_no= b.t_card_no
inner join "+dbname+@"..T_CC_pro_report_list rpt on rpt.t_process_c_id = b.t_c_id
where a.t_card_id=" + ccc_cardno + @"
", Cmd);
            if (string.IsNullOrEmpty(lastDate))
            {
                throw new Exception("通过流转卡id"+ccc_cardno+"获取最后报工时间失败");
            }
            if (DateTime.Parse(lastDate) > DateTime.Parse(cLogDate))
            {
                throw new Exception("当前参数控制:入库日期不能早于最后一次报工时间");
            }
        }
        string[] txtdata;
        //仓库
        txtdata = GetTextsFrom_FormData(FormData, "txt_cwhcode");
        if (txtdata == null || txtdata[2].CompareTo("") == 0)
        {
            throw new Exception(txtdata[0] + " 必须录入");
        }
        else
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }
        string mes_rd_checkRdTypeWithWarehouse = GetDataString("select cvalue from " + dbname + "..t_parameter where cpid='mes_rd_checkRdTypeWithWarehouse'", Cmd);
        if (!string.IsNullOrEmpty(mes_rd_checkRdTypeWithWarehouse))
        {
            string rdTypeValue = GetDataString("select "+mes_rd_checkRdTypeWithWarehouse+" from " + dbname + "..warehouse where cwhcode='" + txtdata[2] + "'", Cmd);
            if (!rdTypeValue.Contains("合格") && ccccc_intype.Split(',')[0]=="1")
            {
                throw new Exception("当前交库类型为合格品,与对应仓库" + txtdata[2] + "的类型不一致");
            }
            else if (!rdTypeValue.Contains("废品") && ccccc_intype.Split(',')[0] == "2")
            {
                throw new Exception("当前交库类型为废品,与对应仓库" + txtdata[2] + "的类型不一致");
            }
            else if (!rdTypeValue.Contains("返修") && ccccc_intype.Split(',')[0] == "3")
            {
                throw new Exception("当前交库类型为返修,与对应仓库" + txtdata[2] + "的类型不一致");
            }
        }
    }
    private void CheckInvNoCardRdIn(string ccc_cardno, string dbname, SqlCommand Cmd)
    {
        string mes_app_invNoCardRdInDef = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid = 'mes_app_invNoCardRdInDef'", Cmd);
        if (string.IsNullOrEmpty(mes_app_invNoCardRdInDef))
        {
            return;
        }
        string b = GetDataString(@"
select i."+mes_app_invNoCardRdInDef+@" from " + dbname + @"..T_CC_Card_List a 
inner join " + dbname + @"..Inventory i on a.t_invcode = i.cInvCode
where a.t_card_id = 0" + ccc_cardno , Cmd);
        if (b == "是")
        {
            throw new Exception("当前存货参数控制,不能通过流转卡扫码入库");
        }

    }
    /// <summary>
    /// 判断质量确认是否可以入库
    /// </summary>
    /// <returns></returns>
    private void CheckIsCanQualityRd(string ccc_cardno,string dbname,SqlCommand Cmd)
    {
        if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid = 'mes_qc_QualityIsOpen'", Cmd) != "是")
        {
            // 质量确认未开启直接返回
            return ;
        }
        string mes_qc_QualityIsOpenInvDef = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid = 'mes_qc_QualityIsOpenInvDef'", Cmd);
        if (string.IsNullOrEmpty(mes_qc_QualityIsOpenInvDef))
        {
            throw new Exception("质量确认参数存货自定义项未设置");
        }
        if (GetDataString(@"
select iex." + mes_qc_QualityIsOpenInvDef + @" from " + dbname + @"..T_CC_Card_List card
inner join " + dbname + @"..mom_orderdetail md  on card.t_modid = md.MoDId
inner join " + dbname + @"..inventory i on i.cInvCode = md.InvCode
inner join " + dbname + @"..Inventory_extradefine iex on iex.cInvCode = i.cInvCode
where card.t_card_id = 0" + ccc_cardno, Cmd) == "是")
        {
            throw new Exception("当前存货需要质量确认,不能通过扫码入库的方式入库废品");
        }
    }

    [WebMethod]  //MES 扫描MES流转卡  入库 (whf -- 针对报工单质量确认,废品入库)
    public string U8SCM_MOMOrder_TO_RD10_RptQcD(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate
        , string SheetID, string rptId, string t_qc_did)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        


        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            // 废品入库,入库人员如果是编码转化为名称
            string _cUserName = GetDataString("select cUser_Name from UFSystem..UA_User where cUser_Id = '" + cUserName + "'", Cmd);
            if (!string.IsNullOrEmpty(_cUserName))
            {
                cUserName = _cUserName;
            }

            string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
            if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
            string ccc_flowno = GetTextsFrom_FormData_Text(FormData, "txt_cardno");
            string ccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");
            if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

            if (GetDataInt(@"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + ccc_flowno + "' and ISNULL(a.CloseUser,'')<>''", Cmd) > 0)
                throw new Exception("生产订单已经关闭");
            //检查入库单的日期和最后一次报工时间  检查入库单仓库和仓库上配置的入库类型
            CheckRdDateAndType(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string cRet = MES_OR_U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string[] cRdInfo = cRet.Split(',');  //入库单ID，入库单单号，入库单子表标识
            //建立流转卡和 入库单的关联关系 
            try
            {
                WriteFlowDataToRd10(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname, ccc_flowno);
                if (GetDataInt("select count(*) from " + dbname + @"..t_cqcc_Rd10_FlowCard_qcd where t_qc_did= 0" + t_qc_did, Cmd) > 0)
                    throw new Exception("当前缺陷明细已有入库记录");
                if (GetDataInt("select count(*) from " + dbname + @"..t_cqcc_Rd10_FlowCard_rpt where t_rpt_id= 0" + rptId, Cmd) > 0)
                    throw new Exception("当前报工单已有入库记录");
                Cmd.CommandText = "insert into " + dbname + @"..t_cqcc_Rd10_FlowCard_qcd(t_autoid,t_card_id,t_ctype,t_qc_did) values(" + cRdInfo[2] + "," + ccc_cardno + "," + ccc_intype.Split(',')[0] + "," + t_qc_did + ")";
                Cmd.ExecuteNonQuery();
                // 志哥说废品入库,入库单不能自动审核
                Cmd.CommandText = "update " + dbname + @"..rdrecord10 set cHandler=null,dnverifytime=null where id="+cRdInfo[0];
                Cmd.ExecuteNonQuery();
                // 写入表体扩展自定义项
                if (GetDataInt("select count(*) from " + dbname + @"..rdrecords10_extradefine where  AutoID=0" + cRdInfo[2], Cmd) == 0)
                {
                    // 扩展自定义项表对应记录不存在,进行插入
                    Cmd.CommandText = "insert into " + dbname + @"..rdrecords10_extradefine (AutoID) values (" + cRdInfo[2] + ");";
                    Cmd.ExecuteNonQuery();
                }
                // 获取要写入入库单子表扩展自定义项的数据
                DataTable qcData = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"
select TCQDD.t_defectname,op.Description opName,dept.cDepMemo,wc.Description wcName
from " + dbname + @"..T_CC_pro_report_qcdetail qcd
left join " + dbname + @"..T_CC_QC_DefectDoc TCQDD on qcd.t_defectcode = TCQDD.t_defectcode
left join " + dbname + @"..sfc_operation op on op.OpCode = qcd.t_resp_opcode
left join " + dbname + @"..v_cqcc_zr_Department zr on zr.cdepcode = qcd.t_resp_dept
left join " + dbname + @"..Department dept on dept.cDepCode = zr.cDept_num
left join " + dbname + @"..T_CC_pro_report_list rpt on rpt.t_id = qcd.t_rpt_id
left join " + dbname + @"..sfc_workcenter wc on wc.WcCode = rpt.t_wc_code
where qcd.t_qc_did=0" + t_qc_did);
                if (qcData.Rows.Count > 0)
                {
                    Cmd.CommandText = @"
update " + dbname + @"..rdrecords10_extradefine 
set cbdefine2='" + qcData.Rows[0]["t_defectname"] + @"',
    cbdefine3='" + qcData.Rows[0]["opName"] + @"',
    cbdefine4='" + qcData.Rows[0]["cDepMemo"] + @"',
    cbdefine6='" + qcData.Rows[0]["wcName"] + @"'
where   AutoID = 0" + cRdInfo[2];
                    Cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ee)
            {
                throw ee;
            }

            Cmd.Transaction.Commit();

            return cRdInfo[0] + "," + cRdInfo[1];
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


    public string U8SCM_MOMOrder_TO_QMInspect_JSON(string FormDataJSON, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        DataTable FormData = VendorIO.JsonToDataTable(FormDataJSON);
        return U8SCM_MOMOrder_TO_QMInspect(FormData, dbname, cUserName, cLogDate, SheetID);
    }
    [WebMethod]  //MES 扫描MES流转卡  报检
    public string U8SCM_MOMOrder_TO_QMInspect(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
            if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
            string ccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");
            if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

            if (GetDataInt(@"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + ccc_cardno + "' and ISNULL(a.CloseUser,'')<>''", Cmd) > 0)
                throw new Exception("生产订单已经关闭");

            string cRet = MES_OR_U8SCM_MOMOrder_TO_QMInspect(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string[] cRdInfo = cRet.Split(',');  //报检ID，报检单号，报检子表标识
            //建立流转卡和 报检的关联关系 
            try
            {
                WriteFlowDataToQMInspect(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname);
            }
            catch (Exception ee)
            {
                throw ee;
            }

            Cmd.Transaction.Commit();

            return cRdInfo[0] + "," + cRdInfo[1];
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

    [WebMethod]  //MES 批量流转卡入库
    public string U8SCM_MOMOrder_TO_PiL_RD10(System.Data.DataTable dtBody, System.Data.DataTable dtHead, string dbname, string cUserName, string cLogDate)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        if (dtBody.Rows.Count == 0) throw new Exception("没有有效数据");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        string cRetMsg = "";
        //为DataTable 建立索引
        dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        string emsg = "";
        try
        {
            string ccc_intype = GetTextsFrom_FormData_Text(dtHead, "txt_ctype");
            if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

            //DataTable Datas = new DataTable("Datas");
            //Datas.Columns.Add("LabelText"); Datas.Columns.Add("TxtName"); Datas.Columns.Add("TxtTag"); Datas.Columns.Add("TxtValue");
            GetDataString("select '行数：" + dtBody.Rows.Count + "'", Cmd);
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                string ccc_cardno = dtBody.Rows[i]["cardno"] + "";
                if (GetDataInt(@"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + ccc_cardno + "' and ISNULL(a.CloseUser,'')<>''", Cmd) > 0)
                    throw new Exception("生产订单已经关闭");

                DataRow dr = null;
                DataTable FormData = dtHead.Clone();
                for (int r = 0; r < dtHead.Rows.Count; r++)
                {
                    dr = FormData.NewRow();
                    dr["LabelText"] = dtHead.Rows[r]["LabelText"];
                    dr["TxtName"] = dtHead.Rows[r]["TxtName"];
                    dr["TxtTag"] = dtHead.Rows[r]["TxtTag"];
                    dr["TxtValue"] = dtHead.Rows[r]["TxtValue"];
                    FormData.Rows.Add(dr);
                }

                DataRow drCardNo = null;
                DataRow drMoNo = null;
                for (int r = 0; r < dtBody.Columns.Count; r++)
                {
                    dr = FormData.NewRow();
                    dr["LabelText"] = dtBody.Columns[r].Caption;
                    dr["TxtName"] = "txt_" + dtBody.Columns[r].ColumnName;
                    dr["TxtTag"] = dtBody.Rows[i][r] + "";
                    dr["TxtValue"] = dtBody.Rows[i][r] + "";
                    FormData.Rows.Add(dr);

                    if (dtBody.Columns[r].ColumnName.ToLower() == "cardno") drCardNo = dr;
                    if (dtBody.Columns[r].ColumnName.ToLower() == "mocode") drMoNo = dr;
                }
                if (drCardNo == null) throw new Exception("流转卡号必须设置为显示模式");
                if (drMoNo == null) throw new Exception("生产订单号必须设置为显示模式");
                drMoNo["TxtTag"] = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_modid from " + dbname + @"..t_cc_card_list where t_card_no='" + drCardNo["TxtTag"] + "'");

                FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
                string cRet = MES_OR_U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, "U81009", Cmd);
                string[] cRdInfo = cRet.Split(',');  //入库单ID，入库单单号，入库单子表标识
                //建立流转卡和 入库单的关联关系 
                try
                {
                    ccc_cardno = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_card_id from " + dbname + @"..t_cc_card_list where t_card_no='" + drCardNo["TxtTag"] + "'");
                    WriteFlowDataToRd10(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname, "" + drCardNo["TxtTag"]);
                }
                catch (Exception ee)
                {
                    throw ee;
                }
                cRetMsg += "," + cRdInfo[1];

            }
            Cmd.Transaction.Commit();

            if (cRetMsg.Length > 1) cRetMsg = cRetMsg.Substring(1);
            return cRetMsg;
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

    [WebMethod]  //MES 根据箱码匹配流转卡批量入库
    public string MES_Box_HX_Flow(string pinvcode, string pbatch, decimal d_qty, string cwhcode, string dbname, string cUserName, string cLogDate)
    {
        return MES_Box_HX_Flow_boxno(pinvcode, pbatch, "", d_qty, cwhcode, dbname, cUserName, cLogDate);
    }
    [WebMethod]  //MES 根据箱码匹配流转卡批量入库
    public string MES_Box_HX_Flow_boxno(string pinvcode, string pbatch, string boxno, decimal d_qty, string cwhcode, string dbname, string cUserName, string cLogDate)
    {
        return MES_Box_HX_Flow_boxno_dep(pinvcode, pbatch, boxno, d_qty, cwhcode, "%", dbname, cUserName, cLogDate);
    }
    [WebMethod]  //MES 根据箱码匹配流转卡批量入库
    public string MES_Box_HX_Flow_boxno_dep(string pinvcode, string pbatch, string boxno, decimal d_qty, string cwhcode, string cdepcode, string dbname, string cUserName, string cLogDate)
    {
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
            if (pinvcode.Trim().Length == 0) throw new Exception("产品编码不能为空");
            if (d_qty <= 0) throw new Exception("数量必须大于0");
            string theTime = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(20),getdate(),120)");
            string dprodate = theTime.Substring(0, 10);
            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "生产部门";
            dr["TxtName"] = "txt_cdepcode";
            dr["TxtTag"] = cdepcode;
            dr["TxtValue"] = cdepcode;
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "入库类别";
            dr["TxtName"] = "txt_crdcode";
            dr["TxtTag"] = "102";
            dr["TxtValue"] = "102";
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "交库类型";
            dr["TxtName"] = "txt_cdefine1";
            dr["TxtTag"] = "合格品入库";
            dr["TxtValue"] = "合格品入库";

            dr = dtHead.NewRow();
            dr["LabelText"] = "箱码";
            dr["TxtName"] = "txt_cdefine3";
            dr["TxtTag"] = boxno;
            dr["TxtValue"] = boxno;

            dtHead.Rows.Add(dr);
            #endregion

            DataTable dtBody = new DataTable("dtBody");
            dtBody.Columns.Add("modid"); dtBody.Columns.Add("cinvcode"); dtBody.Columns.Add("cbatch"); dtBody.Columns.Add("define22");
            dtBody.Columns.Add("t_card_id"); dtBody.Columns.Add("iquantity"); dtBody.Columns.Add("dprodate");
            DataTable dtFlowRdqty = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select 
                    t1.t_card_id,t1.t_card_no,t_modid,isnull(t1.t_card_overqty,0)-isnull(t2.qty,0) d_canrdqty
                from " + dbname + @"..T_CC_Card_List t1 left join (
	                select t_card_id,sum(b.iQuantity) qty from " + dbname + @"..T_CC_Rd10_FlowCard a inner join " + dbname + @"..rdrecords10 b on a.t_autoid=b.AutoID
	                where b.cInvCode='" + pinvcode + @"' and a.t_ctype=1
	                group by t_card_id
                ) t2 on t1.t_card_id=t2.t_card_id
                left join " + dbname + @"..mom_orderdetail t3 on t1.t_modid=t3.modid
                where t1.t_invcode='" + pinvcode + @"' and t3.MDeptCode like '" + cdepcode + @"' 
                    and isnull(t1.t_card_overqty,0)-isnull(t2.qty,0)>0 
                    and isnull(t3.CloseUser,'')='' ");
            foreach (DataRow flow in dtFlowRdqty.Rows)
            {
                decimal d_flow_qty = Convert.ToDecimal(flow["d_canrdqty"]);
                decimal d_rd10_qty = 0;
                if (d_flow_qty < d_qty)
                {
                    d_rd10_qty = d_flow_qty;
                    d_qty = d_qty - d_flow_qty;
                }
                else
                {
                    d_rd10_qty = d_qty;  //装箱剩余数量入库
                    d_qty = 0;
                }
                DataRow RdRow = dtBody.NewRow();
                RdRow["modid"] = flow["t_modid"];
                RdRow["define22"] = flow["t_card_no"];
                RdRow["cinvcode"] = pinvcode;
                RdRow["cbatch"] = pbatch;
                RdRow["t_card_id"] = flow["t_card_id"];
                RdRow["iquantity"] = d_rd10_qty;
                RdRow["dprodate"] = dprodate;
                dtBody.Rows.Add(RdRow);
                if (d_qty <= 0) break;
            }
            if (d_qty > 0) throw new Exception("当前流转卡未入库剩余数量不够本箱入库");
            //调用入库
            U8StandSCMBarCode u8 = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string cRet = u8.U81015(dtHead, dtBody, dbname, cUserName, cLogDate, "U81015", Cmd);
            //回写状态
            DataTable dtRds = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select b.t_card_id,a.AutoID from " + dbname + @"..rdrecords10 a 
                inner join " + dbname + @"..T_CC_Card_List b on a.cDefine22=b.t_card_no
                where a.ID=0" + cRet.Split(',')[0]);
            if (dtRds.Rows.Count == 0) throw new Exception("入库信息错误，请重新操作");
            foreach (DataRow rds in dtRds.Rows)
            {
                Cmd.CommandText = @"insert into " + dbname + @"..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) 
                    values(" + rds["AutoID"] + "," + rds["t_card_id"] + ",1)";
                Cmd.ExecuteNonQuery();
            }
            tr.Commit();
            return cRet.Split(',')[1];
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


    //根据流转卡入库（同时支持U8流转卡和MES流转卡）
    private string MES_OR_U8SCM_MOMOrder_TO_RD10(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID, System.Data.SqlClient.SqlCommand Cmd)
    {
        string errmsg = "";
        string[] txtdata = null;  //临时取数
        string ccccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");   //交库类型
        if (ccccc_intype == "") throw new Exception("交库类型不能为空！");

        #region  //逻辑检验
        txtdata = GetTextsFrom_FormData(FormData, "txt_mocode");
        if (txtdata == null) { throw new Exception("模板设置：生产订单 栏目必须设置成可视栏目"); }
        if (txtdata[2].CompareTo("") == 0) { throw new Exception("请扫描流转卡条码"); }

        string modid = txtdata[2];
        string mocode = txtdata[3];
        //判断生产订单是否关闭
        if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail(nolock) where modid=0" + modid + " and isnull(CloseUser,'')<>''", Cmd) > 0)
            throw new Exception("生产订单已经关闭");

        //判断是否勾选 质检标识
        if (Convert.ToInt32(ccccc_intype.Split(',')[0]) == 1)  //合格品入库
        {
            if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid=0" + modid + " and QcFlag=1", Cmd) > 0)
                throw new Exception("生产订单（合格品）必须走检验流程,不能采用流转卡交库");
        }
        //if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid=0" + modid + " and QcFlag=1", Cmd) > 0)
        //    throw new Exception("本生产订单必须先走检验流程");


        //业务人员
        txtdata = GetTextsFrom_FormData(FormData, "txt_t_psn_code");
        if (txtdata != null && txtdata[2].CompareTo("") != 0)
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..hr_hi_person where cPsn_Num='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }

        //仓库
        txtdata = GetTextsFrom_FormData(FormData, "txt_cwhcode");
        if (txtdata == null || txtdata[2].CompareTo("") == 0)
        {
            throw new Exception(txtdata[0] + " 必须录入");
        }
        else
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..warehouse where cwhcode='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }

        //部门
        txtdata = GetTextsFrom_FormData(FormData, "txt_cdepcode");
        if (txtdata == null || txtdata[2].CompareTo("") == 0)
        {
            throw new Exception(txtdata[0] + " 必须录入");
        }
        else
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..department where cdepcode='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }

        //入库类别
        txtdata = GetTextsFrom_FormData(FormData, "txt_crdcode");
        if (txtdata == null || txtdata[2].CompareTo("") == 0)
        {
            throw new Exception(txtdata[0] + " 必须录入");
        }
        else
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..Rd_Style where crdcode='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = GetSqlDataTable("select t_fieldname from " + dbname + "..T_CC_Base_GridCol_rule where SheetID='" + SheetID + @"' 
                and (t_fieldname like '%define%' or t_fieldname like '%free%')", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据必录项
        System.Data.DataTable dtMustInputCol = GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID=''", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
            if (txtdata == null) throw new Exception(dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项，模板必须设置成可视栏目");

            if (txtdata[3].CompareTo("") == 0 && txtdata[2].CompareTo("") != 0)  //
            {
                throw new Exception(txtdata[0] + "录入不正确 录入键值和显示值不匹配");
            }
            if (txtdata[3].CompareTo("") == 0)  //
            {
                throw new Exception(txtdata[0] + "为必录项,不能为空");
            }
        }

        //生产订单审核日期 比对，入库日期必须大于等于审核日期
        string mo_order_reldate = GetDataString("select convert(varchar(10),isnull(RelsDate,'2000-01-01'),120) from " + dbname + "..mom_orderdetail where MoDId=0" + modid, Cmd);
        if (mo_order_reldate.CompareTo(cLogDate) > 0) throw new Exception("入库日期不能小于生产订单的审核日期");

        #endregion

        //显示标题   txt_字段名称    文本Tag     文本Text值
        string[] s_txt_iquantity = GetTextsFrom_FormData(FormData, "txt_iquantity");
        if (s_txt_iquantity == null) { throw new Exception("模板设置：入库数量 栏目必须设置成可视栏目"); }

        //取产品入库单的 最小模板ID  
        int vt_id = GetDataInt("select isnull(min(DEF_ID),0) from " + dbname + "..Vouchers_base where CardNumber='0411'", Cmd);
        if (float.Parse(s_txt_iquantity[3]) <= 0) throw new Exception("入库数量必须大于0");
        string DBName = dbname;
        string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);


        #region//写主表
        KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
        KK_U8Com.U8Rdrecords10 records10 = new KK_U8Com.U8Rdrecords10(Cmd, dbname);

        //最大编号处理
        if (GetDataInt("select COUNT(*) from " + DBName + "..T_CC_Voucher_Num where voucher_name='rdrecord10'", Cmd) == 0)
        {
            Cmd.CommandText = "insert into " + DBName + "..T_CC_Voucher_Num(voucher_name,chead,cdigit_1,cdigit_2) values('rdrecord10','CR',0,0)";
            Cmd.ExecuteNonQuery();
            string c_max_code = GetDataString("select cast(isnull(max(replace(ccode,'CR','')),'0') as int)+5 from " + DBName + "..Rdrecord10 where ccode like 'CR%'", Cmd);
            Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=0" + c_max_code + " where voucher_name='rdrecord10' and chead='CR'";
            Cmd.ExecuteNonQuery();
        }

        //string vouCode = GetDataString("select 'CR'+right('0000000000'+cast(cast(isnull(max(replace(ccode,'CR','')),'0') as int)+1 as varchar(10)),8) from " + DBName + "..Rdrecord10 where ccode like 'CR%'", Cmd);
        string vouCode = GetDataString("select chead+right('0000000000'+cast(cdigit_1+1 as varchar(20)),8) from " + DBName + "..T_CC_Voucher_Num with(rowlock) where voucher_name='rdrecord10' and chead='CR'", Cmd);
        Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=cdigit_1+1 where voucher_name='rdrecord10' and chead='CR'";
        Cmd.ExecuteNonQuery();
        
        record10.VT_ID = vt_id;
        record10.cCode = "'" + vouCode + "'";
        record10.ID = 1000000000 + int.Parse(GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        string rd_max_id = record10.ID + "";
        record10.cVouchType = "'10'";
        string ccc_depp = GetTextsFrom_FormData_Tag(FormData, "txt_cdepcode");
        record10.cDepCode = ccc_depp.CompareTo("") == 0 ? "null" : "'" + ccc_depp + "'";
        string cPsnCodes = "" + GetTextsFrom_FormData_Tag(FormData, "txt_t_psn_code");
        record10.cPersonCode = (cPsnCodes.CompareTo("") == 0 ? "null" : "'" + cPsnCodes + "'");
        record10.bredvouch = "0"; //红篮子  
        record10.cWhCode = "'" + GetTextsFrom_FormData_Tag(FormData, "txt_cwhcode") + "'";
        record10.cMaker = "'" + cUserName + "'";
        record10.dDate = "'" + cLogDate + "'";

        //mes_flow_rd10_autocheck
        string rd_auto_check = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_autocheck'", Cmd);
        if (rd_auto_check.ToLower().CompareTo("true") == 0)
        {
            record10.cHandler = "'" + cUserName + "'";
            record10.dVeriDate = "'" + cLogDate + "'";
        }
        record10.cRdCode = "'" + GetTextsFrom_FormData_Tag(FormData, "txt_crdcode") + "'";
        record10.iExchRate = "1";
        record10.cExch_Name = "'人民币'";
        record10.cDefine1 = "'产品入库AUTO'";
        record10.cSource = "'生产订单'";

        #region //主表自定义项
        record10.cDefine1 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define1") + "'";
        record10.cDefine2 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define2") + "'";
        record10.cDefine3 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define3") + "'";
        record10.cDefine4 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define4") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define5");
        record10.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        record10.cDefine6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define6") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define7");
        record10.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record10.cDefine8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define8") + "'";
        record10.cDefine9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define9") + "'";
        record10.cDefine10 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define10") + "'";
        record10.cDefine11 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define11") + "'";
        record10.cDefine12 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define12") + "'";
        record10.cDefine13 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define13") + "'";
        record10.cDefine14 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define15");
        record10.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define16");
        record10.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        //查找入库单号是否重复
        if (GetDataInt("select count(*) from " + DBName + "..Rdrecord10 where ccode=" + record10.cCode, Cmd) > 0)
            throw new Exception("单据号存储冲突，请重新点击保存");

        if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

        //创建产品入库单条码
        Cmd.CommandText = @"update " + DBName + "..rdrecord10 set csysbarcode='||st10|" + record10.cCode.Replace("'", "") + "' where id=" + record10.ID;
        Cmd.ExecuteNonQuery();

        #endregion

        #region  //子表
        #region//货位控制
        bool b_Pos = false;
        if (GetDataInt("select count(*) from " + DBName + @"..warehouse where cWhCode=" + record10.cWhCode + " and bWhPos=1", Cmd) > 0)
        {
            string c_pos_code = GetTextsFrom_FormData_Tag(FormData, "txt_cposcode");
            if (c_pos_code == "") throw new Exception("请输入货位");
            records10.cPosition = "'" + c_pos_code + "'";
            b_Pos = true;
        }
        #endregion

        //判断仓库是否计入成本
        string b_costing = GetDataString("select cast(bincost as int) from " + DBName + "..warehouse where cwhcode=" + record10.cWhCode, Cmd);

        //订单类型
        int cmom_type = int.Parse(GetDataString("select MoClass from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
        //string cmom_mrp_qty = GetDataString("select isnull(max(Qty),0) from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd);//生产订单数量
        float f_Qty_in = float.Parse(s_txt_iquantity[3]);  //入库数量
        float f_qtyinall = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0", Cmd));
        string cc_invcode = GetTextsFrom_FormData_Text(FormData, "txt_cinvcode");
        if (cc_invcode.CompareTo("") == 0) { throw new Exception("模板设置：存货编码 栏目必须设置成可视栏目"); }

        #region//判断是否有关键子件
        //        //是否关键子件控制(库存 超生产订单入库 参数)
        //        string cst_option_chao = GetDataString("select cvalue from " + DBName + "..AccInformation where cname='bOverMPIn'",Cmd);
        //        int iKeysCount = int.Parse(GetDataString(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
        //                where a.MoDId=0" + modid + " and i.bInvKeyPart=1 ", Cmd));
        //        //关键领用控制量，取最小值  
        //        float f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
        //                where a.MoDId=0" + modid + " and i.bInvKeyPart=1 ", Cmd));
        //        if (f_ll_qty < f_Qty_in + f_qtyinall && iKeysCount > 0 && cst_option_chao.ToLower().CompareTo("true") == 0)
        //        {
        //            //判断是否  专员
        //            bool bExit = true;
        //            string czyuname = GetDataString("select isnull(cValue,'') from " + DBName + "..T_Parameter where cPid='mes_U81009_rd10_out_qxlist'", Cmd);
        //            if (czyuname.IndexOf(cUserName) < 0) throw new Exception("生产订单【" + mocode + "】存货【" + cc_invcode + "】超领料入库");
        //        }
        #endregion

        //子表
        records10.AutoID = int.Parse(GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
        Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        records10.ID = record10.ID;
        records10.cInvCode = "'" + cc_invcode + "'";
        //modid,mocode,soseq,cinvcode,cinvname,cinvstd,cunitname,balqualifiedqty,cbatch,bomtype
        records10.iQuantity = "" + f_Qty_in;  //入库数量
        records10.irowno = 1;
        records10.bCosting = b_costing;//是否计入成本
        records10.cvmivencode = "''";
        //上游单据关联
        records10.iNQuantity = records10.iQuantity;
        records10.iordertype = "0";
        //生产订单的生产批号
        records10.cMoLotCode = "'" + GetDataString("select MoLotCode from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd) + "'";

        System.Data.DataTable dtfclist = GetSqlDataTable(@"select top 1 a.opseq,a.description,b.WcCode from " + DBName + @"..sfc_moroutingdetail a inner join 
                " + DBName + "..sfc_workcenter b on a.wcid=b.wcid where modid=0" + modid + " order by opseq desc", "dtfclist", Cmd);
        records10.iMPoIds = "" + modid;
        records10.imoseq = "" + GetDataString("select SortSeq from " + DBName + @"..mom_orderdetail where modid=0" + modid, Cmd);
        records10.cmocode = "'" + mocode + "'";
        if (dtfclist.Rows.Count > 0)  //设置工序信息
        {
            records10.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
            records10.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
            records10.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
        }
        string cToday = GetDataString("select convert(varchar(10),'" + cLogDate + "',120)", Cmd);

        #region  //自由项管理   自定义项
        records10.cFree1 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free1") + "'";
        records10.cFree2 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free2") + "'";
        records10.cFree3 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free3") + "'";
        records10.cFree4 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free4") + "'";
        records10.cFree5 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free5") + "'";
        records10.cFree6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free6") + "'";
        records10.cFree7 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free7") + "'";
        records10.cFree8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free8") + "'";
        records10.cFree9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free9") + "'";
        records10.cFree10 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free10") + "'";

        records10.cDefine22 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define22") + "'";
        records10.cDefine23 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define23") + "'";
        records10.cDefine24 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define24") + "'";
        records10.cDefine25 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define25") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define26");
        records10.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define27");
        records10.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
        records10.cDefine28 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define28") + "'";
        records10.cDefine29 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define29") + "'";
        records10.cDefine30 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define30") + "'";
        // 沃德要求加的--扫码入库时，把生产订单的部门编码，写入入库单表体的cdefine30中
        records10.cDefine30 = record10.cDepCode;
        records10.cDefine31 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define31") + "'";
        records10.cDefine32 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define32") + "'";
        records10.cDefine33 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define33") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define34");
        records10.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define35");
        records10.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
        records10.cDefine36 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define36") + "'";
        records10.cDefine37 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define37") + "'";
        #endregion

        #region//批次管理和保质期管理
        string cc_batch = GetTextsFrom_FormData_Text(FormData, "txt_cbatch");
        if (int.Parse(GetDataString("select count(*) FROM " + DBName + "..inventory where cinvcode='" + cc_invcode + "' and bInvBatch=1", Cmd)) > 0)
        {
            if (cc_batch.CompareTo("") == 0) throw new Exception("请输入批号（有批次管理情况下，批号 必须设置成可视栏目）");
            records10.cBatch = "'" + cc_batch + "'";
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
                records10.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                records10.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                records10.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                records10.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                records10.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                records10.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
            }

            #region  //是否建立批次档案
            if (U8Operation.GetDataInt("select count(*) from " + DBName + "..Inventory_Sub where cInvSubCode=" + records10.cInvCode + " and bBatchCreate=1", Cmd) > 0)
            {
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty1");
                records10.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty2");
                records10.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty3");
                records10.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty4");
                records10.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty5");
                records10.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                records10.cBatchProperty6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty6") + "'";
                records10.cBatchProperty7 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty7") + "'";
                records10.cBatchProperty8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty8") + "'";
                records10.cBatchProperty9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty9") + "'";
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty10");
                records10.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                //继承批次档案数据
                DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + DBName + @"..AA_BatchProperty a 
                            where cInvCode=" + records10.cInvCode + " and cBatch=" + records10.cBatch + " and isnull(cFree1,'')=" + records10.cFree1 + @" 
                                and isnull(cFree2,'')=" + records10.cFree2 + " and isnull(cFree3,'')=" + records10.cFree3 + " and isnull(cFree4,'')=" + records10.cFree4 + @" 
                                and isnull(cFree5,'')=" + records10.cFree5 + " and isnull(cFree6,'')=" + records10.cFree6 + " and isnull(cFree7,'')=" + records10.cFree7 + @" 
                                and isnull(cFree8,'')=" + records10.cFree8 + " and isnull(cFree9,'')=" + records10.cFree9 + " and isnull(cFree10,'')=" + records10.cFree10, "dtBatPerp", Cmd);
                if (dtBatPerp.Rows.Count > 0)
                {
                    if (records10.cBatchProperty1 == "null")
                        records10.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                    if (records10.cBatchProperty2 == "null")
                        records10.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                    if (records10.cBatchProperty3 == "null")
                        records10.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                    if (records10.cBatchProperty4 == "null")
                        records10.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                    if (records10.cBatchProperty5 == "null")
                        records10.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                    if (records10.cBatchProperty6 == "''")
                        records10.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                    if (records10.cBatchProperty7 == "''")
                        records10.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                    if (records10.cBatchProperty8 == "''")
                        records10.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                    if (records10.cBatchProperty9 == "''")
                        records10.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                    if (records10.cBatchProperty10 == "null")
                        records10.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                }
                else  //建立档案
                {
                    Cmd.CommandText = "insert into " + DBName + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                            cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                        values(newid()," + records10.cBatchProperty1 + "," + records10.cBatchProperty2 + "," + records10.cBatchProperty3 + "," + records10.cBatchProperty4 + "," +
                         records10.cBatchProperty5 + "," + records10.cBatchProperty6 + "," + records10.cBatchProperty7 + "," + records10.cBatchProperty8 + "," +
                         records10.cBatchProperty9 + "," + records10.cBatchProperty10 + "," + records10.cInvCode + "," + records10.cBatch + "," + records10.cFree1 + "," +
                         records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," + records10.cFree6 + "," +
                         records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + ")";
                    Cmd.ExecuteNonQuery();
                }
            }

            #endregion
        }
        #endregion

        if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
        #region// 华青要求 -- 将生产订单类别写入入库单子表的自定义项
        string mes_flow_hq_inRd_ProdOrderTypeDefine = GetDataString("select cValue from "+dbname+"..t_parameter where cpid='mes_flow_hq_inRd_ProdOrderTypeDefine'", Cmd);
        if (!string.IsNullOrEmpty(mes_flow_hq_inRd_ProdOrderTypeDefine))
        {
            string motypeDes = GetDataString("select b.Description from " + dbname + "..mom_orderdetail a inner join " + dbname + "..mom_motype b on a.MoTypeId = b.MoTypeId where MoDId=0" + modid, Cmd);
            Cmd.CommandText = "update " + dbname + "..rdrecords10 set " + mes_flow_hq_inRd_ProdOrderTypeDefine + "='" + motypeDes + "' where AutoID="+records10.AutoID;
            Cmd.ExecuteNonQuery();
        }
        #endregion
        //计划价处理
        Cmd.CommandText = @"update " + DBName + "..rdrecords10 set " + DBName + "..rdrecords10.iPUnitCost=i.iInvRCost,iPPrice=" + DBName + @"..rdrecords10.iquantity*i.iInvRCost
                    from " + DBName + "..inventory i where " + DBName + "..rdrecords10.cinvcode=i.cinvcode and " + DBName + "..rdrecords10.autoid=0" + records10.AutoID;
        Cmd.ExecuteNonQuery();
        //回写 生产订单
        Cmd.CommandText = "update " + DBName + "..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + records10.iQuantity + ") where modid=0" + modid;
        Cmd.ExecuteNonQuery();


        #region//货位账务处理
        if (b_Pos)
        {
            if (U8Operation.GetDataInt("select count(*) from " + DBName + "..position where cposcode=" + records10.cPosition, Cmd) == 0)
            {
                throw new Exception("货位编码" + records10.cPosition + "不存在");
            }
            if (U8Operation.GetDataInt("select count(*) from " + DBName + "..position where cposcode=" + records10.cPosition + " and cwhcode=" + record10.cWhCode, Cmd) == 0)
            {
                throw new Exception("货位编码" + records10.cPosition + "不在仓库" + record10.cWhCode + "中");
            }

            //添加货位记录 
            Cmd.CommandText = "Insert Into " + DBName + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                        cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                "Values (" + records10.AutoID + "," + records10.ID + "," + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + "," + records10.iQuantity + "," + records10.iNum +
                ",null," + record10.dDate + ",1,'',0," + record10.cVouchType + "," + record10.dDate + "," + record10.cMaker + "," + records10.cvmivencode + "," + records10.cBatch +
                "," + records10.cFree1 + "," + records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," +
                records10.cFree6 + "," + records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + ",''," + records10.cAssUnit + @",
                    " + records10.dMadeDate + "," + records10.iMassDate + "," + records10.cMassUnit + "," + records10.iExpiratDateCalcu + "," + records10.cExpirationdate + "," + records10.dExpirationdate + ")";
            Cmd.ExecuteNonQuery();

            //指定货位
            float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
            if (fU8Version >= 11)
            {
                Cmd.CommandText = "update " + DBName + "..rdrecords10 set iposflag=1 where autoid =0" + records10.AutoID;
                Cmd.ExecuteNonQuery();
            }
            //修改货位库存
            if (U8Operation.GetDataInt("select count(*) from " + DBName + "..InvPositionSum where cwhcode=" + record10.cWhCode + " and cvmivencode=" + records10.cvmivencode + " and cinvcode=" + records10.cInvCode + @" 
                    and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + " and cfree1=" + records10.cFree1 + " and cfree2=" + records10.cFree2 + " and cfree3=" + records10.cFree3 + @" 
                    and cfree4=" + records10.cFree4 + " and cfree5=" + records10.cFree5 + " and cfree6=" + records10.cFree6 + " and cfree7=" + records10.cFree7 + @" 
                    and cfree8=" + records10.cFree8 + " and cfree9=" + records10.cFree9 + " and cfree10=" + records10.cFree10, Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + DBName + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                    values(" + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + ",0," + records10.cBatch + @",
                        " + records10.cFree1 + "," + records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," +
                    records10.cFree6 + "," + records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + "," + records10.cvmivencode + @",'',0,
                        " + records10.dMadeDate + "," + records10.iMassDate + "," + records10.cMassUnit + "," + records10.iExpiratDateCalcu + "," + records10.cExpirationdate + "," + records10.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();
            }
            Cmd.CommandText = "update " + DBName + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + records10.iQuantity + "),inum=isnull(inum,0)+(" + records10.iNum + @") 
                    where cwhcode=" + record10.cWhCode + " and cvmivencode=" + records10.cvmivencode + " and cinvcode=" + records10.cInvCode + @" 
                    and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + " and cfree1=" + records10.cFree1 + " and cfree2=" + records10.cFree2 + " and cfree3=" + records10.cFree3 + @" 
                    and cfree4=" + records10.cFree4 + " and cfree5=" + records10.cFree5 + " and cfree6=" + records10.cFree6 + " and cfree7=" + records10.cFree7 + @" 
                    and cfree8=" + records10.cFree8 + " and cfree9=" + records10.cFree9 + " and cfree10=" + records10.cFree10;
            Cmd.ExecuteNonQuery();
        }
        #endregion


        #region//是否超生产订单检查  和  领料比例控制
        float fRkqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0", Cmd));
        #region //判断是否生产订单入库  0代表不能超   1 代表可超
        if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bOverMPIn'", Cmd) == 0)
        {
            //关键领用控制量，取最小值  
            float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select qty from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
            if (f_ll_qty < fRkqty) throw new Exception("存货【" + records10.cInvCode + "】超生产订单入库");
        }
        #endregion

        #region//判断是否有关键子件  0代表不控制   1 代表控制
        int iControlType = U8Operation.GetDataInt("select CAST(cvalue as int) from " + DBName + "..AccInformation where cSysID='st' and cname='iMOProInCtrlBySet'", Cmd);
        if (iControlType == 1)  //iControlType=0 控制有无领料记录   
        {
            int iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + dbname + "..mom_moallocate a inner join " + dbname + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd);
            if (iKeysCount > 0 && U8Operation.GetDataInt("select count(*) from " + DBName + "..mom_moallocate where MoDId=" + modid + " and ByproductFlag=0 and isnull(IssQty,0)>0", Cmd) == 0)
                throw new Exception("无领料记录");
        }

        //废品是否控制子件比例 参数
        string mes_flow_rd10_feip_control = U8Operation.GetDataString(@"select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_feip_mer_bili_control'", Cmd).ToLower();
        float f_bl_rk_qty = fRkqty;  //控制领用比例 使用
        if (mes_flow_rd10_feip_control.CompareTo("true") == 0)
        {
            f_bl_rk_qty = fRkqty + float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 a inner join " + DBName + @"..T_CC_Rd10_FlowCard b on a.autoid=b.t_autoid 
                where a.iMPoIds=0" + modid + " and a.bRelated=0 and b.t_ctype not in(2,3,5)", Cmd));
        }
        string jk_type = ccccc_intype;  //交库类型
        int itype = int.Parse(jk_type.Split(',')[0]);  //2  3  和   5  代表 废品/返修入库

        //iControlType=2 按照领料比例控制
        if (iControlType == 2 & (mes_flow_rd10_feip_control.CompareTo("true") != 0 || (mes_flow_rd10_feip_control.CompareTo("true") == 0 && itype != 2 && itype != 3 && itype != 5)))
        {
            int iKeysControl = U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bControlKeyMaterial'", Cmd);
            int iKeysCount = 0;
            float iLL_Count = 0;
            string cmom_mrp_qty = U8Operation.GetDataString("select isnull(max(Qty),0) from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd);//生产订单数量
            if (iKeysControl == 1)  //控制关键材料比例
            {
                //是否关键子件控制(库存 超生产订单入库 参数)
                iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and WIPType=3 and a.ByproductFlag=0 ", Cmd);
                iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and WIPType=3 and a.ByproductFlag=0 ", Cmd));
            }
            else   //控制所有材料比例
            {
                iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd);

                iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd));
            }
            if (iKeysCount > 0 && f_bl_rk_qty > iLL_Count) throw new Exception("存货【" + records10.cInvCode + "】超领料入库");
        }
        #endregion



        #endregion

        //创建产品入库单子表条码
        Cmd.CommandText = @"update " + DBName + "..rdrecords10 set cbsysbarcode='||st10|" + record10.cCode.Replace("'", "") + "|1' where autoid=" + records10.AutoID;
        Cmd.ExecuteNonQuery();
        //更新流转卡号到入库单
        string rd_definename_to_flow = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_flow_code'", Cmd);
        if (rd_definename_to_flow.CompareTo("") != 0)
        {
            Cmd.CommandText = @"update " + DBName + "..rdrecords10 set " + rd_definename_to_flow + "='" + GetTextsFrom_FormData_Text(FormData, "txt_cardno") + "' where autoid=" + records10.AutoID;
            Cmd.ExecuteNonQuery();
        }
        //更新流转卡交库类型到入库单表头自定义项
        string c_flow_rd10_flow_type = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_flow_type'", Cmd);
        if (c_flow_rd10_flow_type.CompareTo("") != 0)
        {
            //交库类型
            if (ccccc_intype.IndexOf(',') > 0) ccccc_intype = ccccc_intype.Split(',')[1];
            Cmd.CommandText = @"update " + DBName + "..rdrecord10 set " + c_flow_rd10_flow_type + "='" + ccccc_intype + "' where id=" + records10.ID;
            Cmd.ExecuteNonQuery();
        }

        #endregion

        #region  //入库倒冲
        //废品不倒冲材料
        string fp_mes_daochong = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_feip_daochong'", Cmd);
        int ii_intype = int.Parse(GetTextsFrom_FormData_Text(FormData, "txt_ctype").Split(',')[0]);
        if (ii_intype == 1 || (fp_mes_daochong.ToLower().CompareTo("true") == 0 && ii_intype > 1))
        {
            System.Data.DataTable dtWare11 = GetSqlDataTable("select distinct WhCode from " + dbname + @"..mom_moallocate where modid=0" + modid + " and WIPType=1", "dtWare11", Cmd);
            //获得出库类别
            string cOutRdCode = "" + GetDataString("select cVRSCode from " + dbname + @"..VouchRdContrapose where cVBTID='1104'", Cmd);
            if (dtWare11.Rows.Count > 0)
            {
                if (cOutRdCode.CompareTo("") == 0) throw new Exception("倒冲出库需要 出库类别");
                //存货小数位数
                string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);

                U8StandSCMBarCode u8op = new U8StandSCMBarCode();
                for (int r = 0; r < dtWare11.Rows.Count; r++)
                {
                    if (dtWare11.Rows[r]["WhCode"] + "" == "") throw new Exception("倒冲材料出库仓库不能为空");
                    DataTable dtRdMain = GetSqlDataTable("select '" + cOutRdCode + "' crdcode,'" + dtWare11.Rows[r]["WhCode"] + "' cwhcode," + record10.cDepCode + " cdepcode," + record10.cCode + " cbuscode,'生产倒冲' cbustype", "dtRdMain", Cmd);
                    DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select b.allocateid,b.invcode cinvcode,'' cbvencode,'' cbatch,round((" + records10.iQuantity + @")*BaseQtyN/BaseQtyD," + cInv_DecDgt + @") iquantity,b.modid,'' cposcode
                        from " + dbname + @"..mom_moallocate b where modid=0" + modid + " and WhCode='" + dtWare11.Rows[r]["WhCode"] + "' and WIPType=1", "BodyData", Cmd);
                    if (dtRddetail.Rows.Count == 0) throw new Exception("无法找倒冲出库数据");
                    DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
                    SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                    u8op.U81016(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81016", Cmd);
                }
            }
        }
        #endregion

        #region //联副产品入库
        System.Data.DataTable dtWare10 = GetSqlDataTable("select distinct WhCode from " + dbname + @"..mom_moallocate where modid=0" + modid + " and WIPType=3 and ProductType=2 ", "dtWare10", Cmd);
        //获得入库类别 (todo 查看入库类别)
        string cInRdCode = "" + GetDataString("select cVRRCode from " + dbname + @"..VouchRdContrapose where cVBTID='1001'", Cmd);
        if (dtWare10.Rows.Count > 0)
        {
            if (cInRdCode.CompareTo("") == 0) throw new Exception("联产品入库需要 入库类别");
            //存货小数位数
            string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);

            U8StandSCMBarCode u8op = new U8StandSCMBarCode();
            for (int r = 0; r < dtWare10.Rows.Count; r++)
            {
                if (dtWare10.Rows[r]["WhCode"] + "" == "") throw new Exception("联产品入库 仓库不能为空");
                // todo 比对联产品产品入库单 栏目
                GetDataString("select '联产品测试'",Cmd);
                DataTable dtRdMain = GetSqlDataTable("select '" + cInRdCode + "' crdcode,'" + dtWare10.Rows[r]["WhCode"] + "' cwhcode," + record10.cDepCode + " cdepcode," + record10.cCode + " cbuscode,'生产倒冲' cbustype", "dtRdMain", Cmd);
                DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select b.allocateid impoids,b.invcode cinvcode,'' cbvencode,'"+cc_batch+"' cbatch,round((" + records10.iQuantity + @")*BaseQtyN/BaseQtyD," + cInv_DecDgt + @") iquantity,b.modid,'' cposcode
                        from " + dbname + @"..mom_moallocate b where modid=0" + modid + " and WhCode='" + dtWare10.Rows[r]["WhCode"] + "' and WIPType=3 and ProductType=2 ", "BodyData", Cmd);
                if (dtRddetail.Rows.Count == 0) throw new Exception("无法找联产品入库数据");
                DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
                SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                u8op.U81015(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81015", Cmd);
            }
        }
        #endregion

        return rd_max_id + "," + record10.cCode + "," + records10.AutoID;

    }

    //根据流转卡报检（同时支持U8流转卡和MES流转卡）
    private string MES_OR_U8SCM_MOMOrder_TO_QMInspect(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID, System.Data.SqlClient.SqlCommand Cmd)
    {
        string errmsg = "";
        string[] txtdata = null;  //临时取数
        string ccccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");   //交库类型
        if (ccccc_intype == "") throw new Exception("交库类型不能为空！");

        #region  //逻辑检验
        txtdata = GetTextsFrom_FormData(FormData, "txt_mocode");
        if (txtdata == null) { throw new Exception("模板设置：生产订单 栏目必须设置成可视栏目"); }
        if (txtdata[2].CompareTo("") == 0) { throw new Exception("请扫描流转卡条码"); }
        string card_no = GetTextsFrom_FormData_Text(FormData, "txt_cardno");
        if (card_no == "") { throw new Exception("流转卡号栏目必须设置为可视栏目"); }
        string modid = txtdata[2];
        string mocode = txtdata[3];
        string moid = GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=0" + modid, Cmd);
        //判断生产订单是否关闭
        if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid=0" + modid + " and isnull(CloseUser,'')<>''", Cmd) > 0)
            throw new Exception("生产订单已经关闭");

        //判断是否勾选 质检标识
        if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid=0" + modid + " and QcFlag=0", Cmd) > 0)
            throw new Exception("本生产订单 不能走检验流程");

        //部门
        txtdata = GetTextsFrom_FormData(FormData, "txt_cdepcode");
        if (txtdata == null || txtdata[2].CompareTo("") == 0)
        {
            throw new Exception(txtdata[0] + " 必须录入");
        }
        else
        {
            if (int.Parse(GetDataString("select count(*) from " + dbname + "..department where cdepcode='" + txtdata[2] + "'", Cmd)) == 0)
            {
                throw new Exception(txtdata[0] + " 录入不正确");
            }
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = GetSqlDataTable("select t_fieldname from " + dbname + "..T_CC_Base_GridCol_rule where SheetID='" + SheetID + @"' 
                and (t_fieldname like '%define%' or t_fieldname like '%free%')", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据必录项
        System.Data.DataTable dtMustInputCol = GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID=''", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
            if (txtdata == null) throw new Exception(dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项，模板必须设置成可视栏目");

            if (txtdata[3].CompareTo("") == 0 && txtdata[2].CompareTo("") != 0)  //
            {
                throw new Exception(txtdata[0] + "录入不正确 录入键值和显示值不匹配");
            }
            if (txtdata[3].CompareTo("") == 0)  //
            {
                throw new Exception(txtdata[0] + "为必录项,不能为空");
            }
        }

        //生产订单审核日期 比对，入库日期必须大于等于审核日期
        string mo_order_reldate = GetDataString("select convert(varchar(10),isnull(RelsDate,'2000-01-01'),120) from " + dbname + "..mom_orderdetail where MoDId=0" + modid, Cmd);
        if (mo_order_reldate.CompareTo(cLogDate) > 0) throw new Exception("报检日期不能小于生产订单的审核日期");

        #endregion

        //显示标题   txt_字段名称    文本Tag     文本Text值
        string[] s_txt_iquantity = GetTextsFrom_FormData(FormData, "txt_iquantity");
        if (s_txt_iquantity == null) { throw new Exception("模板设置：入库数量 栏目必须设置成可视栏目"); }

        //取报检单的 最小模板ID  
        int vt_id = GetDataInt("select isnull(min(DEF_ID),0) from " + dbname + "..Vouchers_base where CardNumber='QM02'", Cmd);
        if (float.Parse(s_txt_iquantity[3]) <= 0) throw new Exception("报检数量必须大于0");
        string DBName = dbname;
        string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        #region//写主表
        KK_U8Com.U8QMINSPECTVOUCHER istmain = new KK_U8Com.U8QMINSPECTVOUCHER(Cmd, dbname);
        KK_U8Com.U8QMINSPECTVOUCHERS istdetail = new KK_U8Com.U8QMINSPECTVOUCHERS(Cmd, dbname);

        //最大编号处理
        if (GetDataInt("select COUNT(*) from " + DBName + "..T_CC_Voucher_Num where voucher_name='qminspect'", Cmd) == 0)
        {
            Cmd.CommandText = "insert into " + DBName + "..T_CC_Voucher_Num(voucher_name,chead,cdigit_1,cdigit_2) values('qminspect','QR',0,0)";
            Cmd.ExecuteNonQuery();
            string c_max_code = GetDataString("select cast(isnull(max(replace(CINSPECTCODE,'QR','')),'0') as int)+5 from " + DBName + "..QMINSPECTVOUCHER where CINSPECTCODE like 'QR%'", Cmd);
            Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=0" + c_max_code + " where voucher_name='qminspect' and chead='QR'";
            Cmd.ExecuteNonQuery();
        }

        string vouCode = GetDataString("select chead+right('0000000000'+cast(cdigit_1+1 as varchar(20)),8) from " + DBName + "..T_CC_Voucher_Num with(rowlock) where voucher_name='qminspect' and chead='QR'", Cmd);
        Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=cdigit_1+1 where voucher_name='qminspect' and chead='QR'";
        Cmd.ExecuteNonQuery();
        istmain.CINSPECTCODE = "'" + vouCode + "'";
        string cnewid = GetDataString("select newid()", Cmd);
        istmain.INSPECTGUID = "'" + cnewid + "'";
        istmain.ID = GetDataString("select isnull(max(id),0)+1 from " + DBName + "..QMINSPECTVOUCHER", Cmd);
        string rd_max_id = istmain.ID + "";
        istmain.CVOUCHTYPE = "'QM02'";
        istmain.CSOORDERCODE = "'" + mocode + "'"; //生产订单号
        istmain.CSOURCEID = "" + moid;  //生产订单  主表标识
        istmain.CDEPCODE = "'" + GetTextsFrom_FormData(FormData, "txt_cdepcode")[2] + "'";
        istmain.CMAKER = "'" + cUserName + "'";
        istmain.DDATE = "'" + cLogDate + "'";
        istmain.CTIME = "'" + GetDataString("select right(convert(varchar(20),getdate(),120),8)", Cmd) + "'";
        istmain.CSOURCE = "'生产订单'";
        istmain.IVTID = vt_id + "";
        istmain.CCHECKTYPECODE = "'PRO'";
        istmain.DMAKETIME = "getdate()";
        istmain.iPrintCount = "0";

        //mes_flow_inspect_autocheck  是否自动审核报检单
        string rd_auto_check = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_inspect_autocheck'", Cmd);
        if (rd_auto_check.ToLower().CompareTo("true") == 0)
        {
            istmain.CVERIFIER = "'" + cUserName + "'";
            istmain.DVERIFYDATE = "'" + cLogDate + "'";
        }


        #region //主表自定义项
        istmain.CDEFINE1 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define1") + "'";
        istmain.CDEFINE2 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define2") + "'";
        istmain.CDEFINE3 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define3") + "'";
        istmain.CDEFINE4 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define4") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define5");
        istmain.CDEFINE5 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istmain.CDEFINE6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define6") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define7");
        istmain.CDEFINE7 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istmain.CDEFINE8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define8") + "'";
        istmain.CDEFINE9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define9") + "'";
        istmain.CDEFINE10 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define10") + "'";
        istmain.CDEFINE11 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define11") + "'";
        istmain.CDEFINE12 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define12") + "'";
        istmain.CDEFINE13 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define13") + "'";
        istmain.CDEFINE14 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define15");
        istmain.CDEFINE15 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define16");
        istmain.CDEFINE16 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        #endregion

        //查找入库单号是否重复
        if (GetDataInt("select count(*) from " + DBName + "..QMINSPECTVOUCHER where CINSPECTCODE=" + istmain.CINSPECTCODE, Cmd) > 0)
            throw new Exception("单据号存储冲突，请重新点击保存");

        if (!istmain.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

        //创建产品报检单条码
        Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHER set csysbarcode='||QMCB|" + istmain.CINSPECTCODE.Replace("'", "") + "' where id=" + istmain.ID;
        Cmd.ExecuteNonQuery();

        #endregion

        #region  //子表
        //订单类型
        int cmom_type = int.Parse(GetDataString("select MoClass from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
        //string cmom_mrp_qty = GetDataString("select isnull(max(Qty),0) from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd);//生产订单数量
        float f_Qty_in = float.Parse(s_txt_iquantity[3]);  //报检数量
        float f_qtyinall = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0", Cmd));
        string cc_invcode = GetTextsFrom_FormData_Text(FormData, "txt_cinvcode");
        if (cc_invcode.CompareTo("") == 0) { throw new Exception("模板设置：存货编码 栏目必须设置成可视栏目"); }

        istdetail.AUTOID = GetDataString("select isnull(max(autoid),0)+1 from " + DBName + "..QMINSPECTVOUCHERS", Cmd);
        istdetail.ID = istmain.ID;
        istdetail.SOURCEAUTOID = modid;
        istdetail.ITESTSTYLE = "0";
        istdetail.CINVCODE = "'" + cc_invcode + "'";
        //modid,mocode,soseq,cinvcode,cinvname,cinvstd,cunitname,balqualifiedqty,cbatch,bomtype
        istdetail.FQUANTITY = "" + f_Qty_in;  //报检数量
        istdetail.CBYPRODUCT = "'0'";
        istdetail.IORDERTYPE = "0";
        istdetail.IPROORDERID = moid;
        istdetail.CPROORDERCODE = "'" + mocode + "'";   //生产订单号
        istdetail.IPROORDERAUTOID = GetDataString("select sortseq from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd);   //生产订单行号
        istdetail.BEXIGENCY = "0";
        istdetail.ISOURCEPROORDERID = "0";
        istdetail.ISOURCEPROORDERAUTOID = "0";
        istdetail.CPROBATCH = "'" + GetDataString("select MoLotCode from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd) + "'";
        istdetail.iExpiratDateCalcu = "0";   //有效期推算方式  默认为0
        istdetail.PFCODE = "'" + card_no + "'";  //流转卡号

        #region  //自由项管理   自定义项
        istdetail.CFREE1 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free1") + "'";
        istdetail.CFREE2 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free2") + "'";
        istdetail.CFREE3 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free3") + "'";
        istdetail.CFREE4 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free4") + "'";
        istdetail.CFREE5 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free5") + "'";
        istdetail.CFREE6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free6") + "'";
        istdetail.CFREE7 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free7") + "'";
        istdetail.CFREE8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free8") + "'";
        istdetail.CFREE9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free9") + "'";
        istdetail.CFREE10 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_free10") + "'";

        istdetail.CDEFINE22 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define22") + "'";
        istdetail.CDEFINE23 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define23") + "'";
        istdetail.CDEFINE24 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define24") + "'";
        istdetail.CDEFINE25 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define25") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define26");
        istdetail.CDEFINE26 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define27");
        istdetail.CDEFINE27 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istdetail.CDEFINE28 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define28") + "'";
        istdetail.CDEFINE29 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define29") + "'";
        istdetail.CDEFINE30 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define30") + "'";
        istdetail.CDEFINE31 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define31") + "'";
        istdetail.CDEFINE32 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define32") + "'";
        istdetail.CDEFINE33 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define33") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define34");
        istdetail.CDEFINE34 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define35");
        istdetail.CDEFINE35 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istdetail.CDEFINE36 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define36") + "'";
        istdetail.CDEFINE37 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define37") + "'";
        #endregion

        string cToday = GetDataString("select convert(varchar(10),'" + cLogDate + "',120)", Cmd);
        #region//批次管理和保质期管理
        string cc_batch = GetTextsFrom_FormData(FormData, "txt_cbatch")[3];
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

            #region  //是否建立批次档案
            if (U8Operation.GetDataInt("select count(*) from " + DBName + "..Inventory_Sub where cInvSubCode=" + istdetail.CINVCODE + " and bBatchCreate=1", Cmd) > 0)
            {
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty1");
                istdetail.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty2");
                istdetail.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty3");
                istdetail.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty4");
                istdetail.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty5");
                istdetail.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                istdetail.cBatchProperty6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty6") + "'";
                istdetail.cBatchProperty7 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty7") + "'";
                istdetail.cBatchProperty8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty8") + "'";
                istdetail.cBatchProperty9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty9") + "'";
                txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_cBatchProperty10");
                istdetail.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                //继承批次档案数据
                DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + DBName + @"..AA_BatchProperty a 
                            where cInvCode=" + istdetail.CINVCODE + " and cBatch=" + istdetail.CBATCH + " and isnull(cFree1,'')=" + istdetail.CFREE1 + @" 
                                and isnull(cFree2,'')=" + istdetail.CFREE2 + " and isnull(cFree3,'')=" + istdetail.CFREE3 + " and isnull(cFree4,'')=" + istdetail.CFREE4 + @" 
                                and isnull(cFree5,'')=" + istdetail.CFREE5 + " and isnull(cFree6,'')=" + istdetail.CFREE6 + " and isnull(cFree7,'')=" + istdetail.CFREE7 + @" 
                                and isnull(cFree8,'')=" + istdetail.CFREE8 + " and isnull(cFree9,'')=" + istdetail.CFREE9 + " and isnull(cFree10,'')=" + istdetail.CFREE10, "dtBatPerp", Cmd);
                if (dtBatPerp.Rows.Count > 0)
                {
                    if (istdetail.cBatchProperty1 == "null")
                        istdetail.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                    if (istdetail.cBatchProperty2 == "null")
                        istdetail.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                    if (istdetail.cBatchProperty3 == "null")
                        istdetail.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                    if (istdetail.cBatchProperty4 == "null")
                        istdetail.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                    if (istdetail.cBatchProperty5 == "null")
                        istdetail.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                    if (istdetail.cBatchProperty6 == "''")
                        istdetail.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                    if (istdetail.cBatchProperty7 == "''")
                        istdetail.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                    if (istdetail.cBatchProperty8 == "''")
                        istdetail.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                    if (istdetail.cBatchProperty9 == "''")
                        istdetail.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                    if (istdetail.cBatchProperty10 == "null")
                        istdetail.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                }
                else  //建立档案
                {
                    Cmd.CommandText = "insert into " + DBName + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                            cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                        values(newid()," + istdetail.cBatchProperty1 + "," + istdetail.cBatchProperty2 + "," + istdetail.cBatchProperty3 + "," + istdetail.cBatchProperty4 + "," +
                         istdetail.cBatchProperty5 + "," + istdetail.cBatchProperty6 + "," + istdetail.cBatchProperty7 + "," + istdetail.cBatchProperty8 + "," +
                         istdetail.cBatchProperty9 + "," + istdetail.cBatchProperty10 + "," + istdetail.CINVCODE + "," + istdetail.CBATCH + "," + istdetail.CFREE1 + "," +
                         istdetail.CFREE2 + "," + istdetail.CFREE3 + "," + istdetail.CFREE4 + "," + istdetail.CFREE5 + "," + istdetail.CFREE6 + "," +
                         istdetail.CFREE7 + "," + istdetail.CFREE8 + "," + istdetail.CFREE9 + "," + istdetail.CFREE10 + ")";
                    Cmd.ExecuteNonQuery();
                }
            }

            #endregion

        }
        #endregion

        if (!istdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

        //回写 生产订单
        Cmd.CommandText = "update " + DBName + "..mom_orderdetail set DeclaredQty=isnull(DeclaredQty,0)+(0" + istdetail.FQUANTITY + ") where modid=0" + modid;
        Cmd.ExecuteNonQuery();

        #region//是否超生产订单检查  和  领料比例控制
        decimal fRkqty = decimal.Parse(U8Operation.GetDataString(@"select isnull(sum(a.fquantity),0) from " + dbname + @"..QMINSPECTVOUCHERS a
            inner join " + dbname + @"..QMINSPECTVOUCHER b on a.ID=b.ID and b.CVOUCHTYPE='QM02'
            where a.SOURCEAUTOID=0" + modid, Cmd));

        #region //判断是否生产订单入库  0代表不能超   1 代表可超
        if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bOverMPIn'", Cmd) == 0)
        {
            //关键领用控制量，取最小值  
            decimal f_ll_qty = decimal.Parse(U8Operation.GetDataString(@"select qty from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
            if (f_ll_qty < fRkqty) throw new Exception("存货【" + istdetail.CINVCODE + "】超生产订单报检");
        }
        #endregion

        #region//判断是否有关键子件  0代表不控制   1 代表控制
        /*   报检不控制超领料问题
        int iControlType = U8Operation.GetDataInt("select CAST(cvalue as int) from " + DBName + "..AccInformation where cSysID='st' and cname='iMOProInCtrlBySet'", Cmd);
        if (iControlType == 1)  //iControlType=0 控制有无领料记录   
        {
            int iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + dbname + "..mom_moallocate a inner join " + dbname + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd);
            if (iKeysCount > 0 && U8Operation.GetDataInt("select count(*) from " + DBName + "..mom_moallocate where MoDId=" + modid + " and ByproductFlag=0 and isnull(IssQty,0)>0", Cmd) == 0)
                throw new Exception("无领料记录");
        }

        //废品是否控制子件比例 参数
        string mes_flow_rd10_feip_control = U8Operation.GetDataString(@"select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_feip_mer_bili_control'", Cmd).ToLower();
        float f_bl_rk_qty = fRkqty;  //控制领用比例 使用
        if (mes_flow_rd10_feip_control.CompareTo("true") == 0)
        {
            f_bl_rk_qty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 a inner join " + DBName + @"..T_CC_Rd10_FlowCard b on a.autoid=b.t_autoid 
                where a.iMPoIds=0" + modid + " and a.bRelated=0 and b.t_ctype not in(2,3,5)", Cmd));
        }
        string jk_type = ccccc_intype;  //交库类型
        int itype = int.Parse(jk_type.Split(',')[0]);  //2  3  和   5  代表 废品/返修入库

        //iControlType=2 按照领料比例控制
        if (iControlType == 2 & (mes_flow_rd10_feip_control.CompareTo("true") != 0 || (mes_flow_rd10_feip_control.CompareTo("true") == 0 && itype != 2 && itype != 3 && itype != 5)))
        {
            int iKeysControl = U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bControlKeyMaterial'", Cmd);
            int iKeysCount = 0;
            float iLL_Count = 0;
            string cmom_mrp_qty = U8Operation.GetDataString("select isnull(max(Qty),0) from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd);//生产订单数量
            if (iKeysControl == 1)  //控制关键材料比例
            {
                //是否关键子件控制(库存 超生产订单入库 参数)
                iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and WIPType=3 and a.ByproductFlag=0 ", Cmd);
                iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and WIPType=3 and a.ByproductFlag=0 ", Cmd));
            }
            else   //控制所有材料比例
            {
                iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd);

                iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + modid + " and WIPType=3 and a.ByproductFlag=0", Cmd));
            }
            if (iKeysCount > 0 && f_bl_rk_qty > iLL_Count) throw new Exception("存货【" + records10.cInvCode + "】超领料入库");
        }
        */
        #endregion



        #endregion

        //创建产品入库单子表条码
        Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHERS set cbsysbarcode='||QMLB|" + istmain.CINSPECTCODE.Replace("'", "") + "|1' where autoid=" + istdetail.AUTOID;
        Cmd.ExecuteNonQuery();
        //更新流转卡号到报检单
        string rd_definename_to_flow = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_flow_code'", Cmd);
        if (rd_definename_to_flow.CompareTo("") != 0)
        {
            Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHERS set " + rd_definename_to_flow + "='" + GetTextsFrom_FormData_Text(FormData, "txt_cardno") + "' where autoid=" + istdetail.AUTOID;
            Cmd.ExecuteNonQuery();
        }
        //更新流转卡交库类型到入库单表头自定义项
        string c_flow_rd10_flow_type = "" + GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_rd10_flow_type'", Cmd);
        if (c_flow_rd10_flow_type.CompareTo("") != 0)
        {
            //交库类型
            if (ccccc_intype.IndexOf(',') > 0) ccccc_intype = ccccc_intype.Split(',')[1];
            Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHER set " + c_flow_rd10_flow_type + "='" + ccccc_intype + "' where id=" + istdetail.ID;
            Cmd.ExecuteNonQuery();
        }

        #endregion

        return rd_max_id + "," + istmain.CINSPECTCODE + "," + istdetail.AUTOID;

    }


    //回写MES流转卡 编写流转卡和入库单 对照逻辑
    // 添加多段拆分逻辑--whf 
    private void WriteFlowDataToRd10(System.Data.SqlClient.SqlCommand cmd, string rd_autoid, string card_id, string ctype, string dbname, string cc_card_no)
    {
        cmd.CommandText = "insert into " + dbname + @"..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) values(" + rd_autoid + "," + card_id + "," + ctype.Split(',')[0] + ")";
        cmd.ExecuteNonQuery();
        DataTable dtMuti = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"
select max(tprocess.t_card_seq) tseq,max(fprocess.t_card_seq) fseq
from " + dbname + @"..t_cqcc_mutilSplit a 
inner join " + dbname + @"..T_CC_Card_List fCard on a.cardNo = fCard.t_card_no
inner join " + dbname + @"..T_CC_Card_List tCard on tCard.t_father_card_id= fCard.t_card_id
inner join " + dbname + @"..T_CC_Cards_process tprocess on tprocess.t_card_no = tCard.t_card_no
inner join " + dbname + @"..T_CC_Cards_process fprocess on fprocess.t_card_no = fCard.t_card_no
where tCard.t_card_id=" + card_id);
        string c_chai_seqno = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_seqno from " + dbname + "..T_CC_FlowCard_Sp_Main where t_card_id=0" + card_id);
        #region //超入库量控制
        string cols = "";
        //判定上下工序逻辑关系
        float fRDIn = 0;
        float f_fanxiu_out_qty = 0; //本卡现场返修卡量
        int itype = int.Parse(ctype.Split(',')[0]);
        if (itype == 2 || itype == 5) cols = "t_scrap_work+t_scrap_material+t_scrap_qc";
        if (itype == 3) cols = "t_repair_qty";
        if (itype == 4) cols = "t_renew_qty";
        if (itype == 1)
        {
            if (c_chai_seqno.CompareTo("") != 0) throw new Exception("流转卡已经拆分，不能办理合格品入库");
            fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(max(t_card_overqty),0) from " + dbname + "..T_CC_Card_List where t_card_id=0" + card_id));
            if (dtMuti.Rows.Count > 0 && dtMuti.Rows[0][0] + "" != dtMuti.Rows[0][1] + "")
            {
                throw new Exception("多段拆分流转卡,只有末道工序的流转卡可入库");
            }
        }
        else
        {
            if (c_chai_seqno.CompareTo("") == 0)
            {
                //未拆分
                fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(" + cols + "),0) from " + dbname + @"..T_CC_Cards_report a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no where t_card_id=0" + card_id));
            }
            else
            {
                //非末级卡 取拆分前 非合格量  --
                fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(" + cols + @"),0) from " + dbname + @"..T_CC_Cards_report a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no 
                        where b.t_card_id=0" + card_id + " and a.t_card_seq<'" + c_chai_seqno + "'"));
            }
            if (itype == 3)  //返修
            {
                f_fanxiu_out_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(t_card_qty),0) from " + dbname + "..T_CC_Card_List where t_father_card_id=0" + card_id));
                fRDIn = fRDIn - f_fanxiu_out_qty;  //扣减现场返修量  (或者 并开 拆卡的新卡数量) 
            }
            //废品
            if (itype == 2 || itype == 5)
            {
                UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select '数据库名称:" + dbname + "'");
                //外协废品
                float f_om_feipin = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(a.t_scrop_mer+a.t_scrop_work+a.t_scrop_qc),0)
                    from " + dbname + "..T_CC_OM_Receive_Detail a inner join " + dbname + @"..T_CC_Cards_report b on a.t_rpt_c_id=b.t_c_id
                    inner join " + dbname + @"..T_CC_Card_List c on b.t_card_no =c.t_card_no
                    where c.t_card_id=0" + card_id));

                if (itype == 2) fRDIn = fRDIn - f_om_feipin;  //总废品 减去 外协废品
                if (itype == 5) fRDIn = f_om_feipin;  //可入库废品即为外协废品
            }
        }

        float f_rd10_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(b.iquantity),0) from " + dbname + @"..T_CC_Rd10_FlowCard a 
                inner join " + dbname + @"..rdrecords10 b on a.t_autoid=b.autoid and a.t_ctype=" + itype + " and b.bRelated=0 where a.t_card_id=0" + card_id));
        if (fRDIn < f_rd10_qty) throw new Exception("本流转卡“" + ctype.Split(',')[1] + "”可入库【" + fRDIn + "】");
        #endregion

        #region//关闭生产订单
        string modid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_modid from " + dbname + @"..T_CC_Card_List where t_card_id=0" + card_id);
        //缺失量
        float imissqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(a.t_miss_qty),0) from " + dbname + @"..T_CC_Cards_report a 
                inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no where b.t_modid=0" + modid));
        float f_all_rkqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(iquantity),0) from " + dbname + @"..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0"));
        float f_mo_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(Qty),0) from " + dbname + @"..mom_orderdetail where modid=0" + modid));

        string rd_mom_autoclose = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_momorder_autoclose'", cmd);
        if (f_mo_qty == f_all_rkqty + imissqty && rd_mom_autoclose.ToLower().CompareTo("true") == 0)
        {
            //关闭生产订单
            cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='自动关闭',CloseDate=convert(varchar(10),getdate(),120),CloseTime=getdate(),Status=4 where modid=0" + modid;
            cmd.ExecuteNonQuery();
        }

        if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cname='bOverMPIn'", cmd) == 0)
        {
            if (f_mo_qty < f_all_rkqty + imissqty + 0.0000001) throw new Exception("超生产订单入库 订单数[" + f_mo_qty + "]入库数[" + f_all_rkqty + "]缺失数[" + imissqty + "]");
        }
        #endregion

    }

    //回写MES流转卡 编写流转卡和报检单 对照逻辑
    private void WriteFlowDataToQMInspect(System.Data.SqlClient.SqlCommand cmd, string rd_autoid, string card_id, string ctype, string dbname)
    {
        cmd.CommandText = "insert into " + dbname + @"..T_CC_Inspect_FlowCard(t_autoid,t_card_id,t_ctype) values(" + rd_autoid + "," + card_id + "," + ctype.Split(',')[0] + ")";
        cmd.ExecuteNonQuery();

        string c_chai_seqno = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_seqno from " + dbname + "..T_CC_FlowCard_Sp_Main where t_card_id=0" + card_id);
        #region //超入库量控制
        string cols = "";
        //判定上下工序逻辑关系
        float fRDIn = 0;
        float f_fanxiu_out_qty = 0; //本卡现场返修卡量
        int itype = int.Parse(ctype.Split(',')[0]);
        if (itype == 2 || itype == 5) cols = "t_scrap_work+t_scrap_material+t_scrap_qc";
        if (itype == 3) cols = "t_repair_qty";
        if (itype == 4) cols = "t_renew_qty";
        if (itype == 1)
        {
            if (c_chai_seqno.CompareTo("") != 0) throw new Exception("流转卡已经拆分，不能办理合格品报检");
            fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(max(t_card_overqty),0) from " + dbname + "..T_CC_Card_List where t_card_id=0" + card_id));
        }
        else
        {
            if (c_chai_seqno.CompareTo("") == 0)
            {
                //未拆分
                fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(" + cols + "),0) from " + dbname + @"..T_CC_Cards_report a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no where t_card_id=0" + card_id));
            }
            else
            {
                //非末级卡 取拆分前 非合格量  --
                fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(" + cols + @"),0) from " + dbname + @"..T_CC_Cards_report a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no 
                        where b.t_card_id=0" + card_id + " and a.t_card_seq<'" + c_chai_seqno + "'"));
            }
            if (itype == 3)  //返修
            {
                f_fanxiu_out_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(t_card_qty),0) from " + dbname + "..T_CC_Card_List where t_father_card_id=0" + card_id));
                fRDIn = fRDIn - f_fanxiu_out_qty;  //扣减现场返修量  (或者 并开 拆卡的新卡数量) 
            }
            //废品
            if (itype == 2 || itype == 5)
            {
                UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select '数据库名称:" + dbname + "'");
                //外协废品
                float f_om_feipin = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(a.t_scrop_mer+a.t_scrop_work+a.t_scrop_qc),0)
                    from " + dbname + "..T_CC_OM_Receive_Detail a inner join " + dbname + @"..T_CC_Cards_report b on a.t_rpt_c_id=b.t_c_id
                    inner join " + dbname + @"..T_CC_Card_List c on b.t_card_no =c.t_card_no
                    where c.t_card_id=0" + card_id));

                if (itype == 2) fRDIn = fRDIn - f_om_feipin;  //总废品 减去 外协废品
                if (itype == 5) fRDIn = f_om_feipin;  //可入库废品即为外协废品
            }
        }

        float f_rd10_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(b.fquantity),0) from " + dbname + @"..T_CC_Inspect_FlowCard a 
                inner join " + dbname + @"..QMINSPECTVOUCHERS b on a.t_autoid=b.autoid and a.t_ctype=" + itype + " where a.t_card_id=0" + card_id));
        if (fRDIn < f_rd10_qty) throw new Exception("本流转卡“" + ctype.Split(',')[1] + "”可报检【" + fRDIn + "】");
        #endregion

        #region//关闭生产订单
        string modid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_modid from " + dbname + @"..T_CC_Card_List where t_card_id=0" + card_id);
        //缺失量
        float imissqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(a.t_miss_qty),0) from " + dbname + @"..T_CC_Cards_report a 
                inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no where b.t_modid=0" + modid));
        float f_all_rkqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(a.fquantity),0) from " + dbname + @"..QMINSPECTVOUCHERS a
            inner join " + dbname + @"..QMINSPECTVOUCHER b on a.ID=b.ID and b.CVOUCHTYPE='QM02'
            where a.SOURCEAUTOID=0" + modid));
        float f_mo_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(Qty),0) from " + dbname + @"..mom_orderdetail where modid=0" + modid));

        //string rd_mom_autoclose = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_momorder_autoclose'", cmd);
        //if (f_mo_qty == f_all_rkqty + imissqty && rd_mom_autoclose.ToLower().CompareTo("true") == 0)
        //{
        //    //关闭生产订单
        //    cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='自动关闭',CloseDate=convert(varchar(10),getdate(),120),CloseTime=getdate(),Status=4 where modid=0" + modid;
        //    cmd.ExecuteNonQuery();
        //}

        if (f_mo_qty < f_all_rkqty + imissqty) throw new Exception("超生产订单报检 订单数[" + f_mo_qty + "]报检数[" + f_all_rkqty + "]缺失数[" + imissqty + "]");

        #endregion

    }


    [WebMethod]  //U8 产品报检单   返回单据 ID,单据号
    public bool U8SCM_RD10_Check(string crdcode, string dbname, string cUserName, string cLogDate)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        try
        {
            //判断是否已经审核 
            DataTable dtRd = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select id from " + dbname + "..rdrecord10 where ccode='" + crdcode + "' and isnull(cHandler,'')<>''");
            if (dtRd.Rows.Count > 0) throw new Exception("单据已经审核");

            //审核
            Cmd.CommandText = "insert into " + dbname + "..rdrecord10 set cHandler='" + cUserName + "',dVeriDate='" + cLogDate + @"',dnverifytime=getdate() 
                where id=0" + dtRd.Rows[0]["id"];
            Cmd.ExecuteNonQuery();

            //判读是否审核修改现存量
            if (GetDataString("select cValue from " + dbname + "..AccInformation where csysid='st' and cName='bProductInCheck'", Cmd).ToLower() == "true")
            {
                dtRd = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.cwhcode,b.cinvcode,b.cbatch,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,iquantity,inum
                    from " + dbname + "..rdrecord10 a inner join " + dbname + "..rdrecords10 b on a.id=b.id where a.ccode='" + crdcode + "'");
                for (int i = 0; i < dtRd.Rows.Count; i++)
                {
                    string cfree1 = "" + dtRd.Rows[i]["cfree1"];
                    string cfree2 = "" + dtRd.Rows[i]["cfree2"];
                    string cfree3 = "" + dtRd.Rows[i]["cfree3"];
                    string cfree4 = "" + dtRd.Rows[i]["cfree4"];
                    string cfree5 = "" + dtRd.Rows[i]["cfree5"];
                    string cfree6 = "" + dtRd.Rows[i]["cfree6"];
                    string cfree7 = "" + dtRd.Rows[i]["cfree7"];
                    string cfree8 = "" + dtRd.Rows[i]["cfree8"];
                    string cfree9 = "" + dtRd.Rows[i]["cfree9"];
                    string cfree10 = "" + dtRd.Rows[i]["cfree10"];
                    string cbatch = "" + dtRd.Rows[i]["cbatch"];
                    string cinvcode = "" + dtRd.Rows[i]["cinvcode"];
                    string cwhcode = "" + dtRd.Rows[i]["cwhcode"];
                    string iqty = "" + dtRd.Rows[i]["iquantity"];
                    string inum = "" + dtRd.Rows[i]["inum"];
                    #region //修改现存量
                    string ItemId = GetDataString("select isnull(max(id),0) from " + dbname + "..SCM_Item where cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
                        " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
                        " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "'", Cmd);
                    if (ItemId == "0")
                    {
                        Cmd.CommandText = "insert into " + dbname + "..SCM_Item(cinvcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10) values('" + cinvcode + "','" + cfree1 + "','" + cfree2 + "'" +
                            ",'" + cfree3 + "','" + cfree4 + "','" + cfree5 + "','" + cfree6 + "','" + cfree7 + "','" + cfree8 + "','" + cfree9 + "','" + cfree10 + "' )";
                        Cmd.ExecuteNonQuery();
                        ItemId = GetDataString("select isnull(max(id),0) from " + dbname + "..SCM_Item where cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
                        " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
                        " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "'", Cmd);

                    }

                    string currentID = GetDataString("select isnull(max(autoid),0) from " + dbname + "..currentstock where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
                        " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
                        " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "' " +
                        " and isnull(cbatch,'')='" + cbatch + "' and iSoType=0 and iSodid='' and cVMIVenCode=''", Cmd);
                    if (currentID == "0")
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..currentstock(cWhCode,cInvCode,ItemId,cBatch, cVMIVenCode, iSoType, iSodid, iQuantity, iNum,cFree1,cFree2,fOutQuantity, fOutNum, fInQuantity, 
                        fInNum,cFree3, cFree4, cFree5, cFree6, cFree7, cFree8, cFree9, cFree10,bStopFlag, fTransInQuantity, fTransInNum, 
                        fTransOutQuantity, fTransOutNum,fPlanQuantity, fPlanNum, fDisableQuantity, fDisableNum, fAvaQuantity, fAvaNum, BGSPSTOP, 
                        cMassUnit, fStopQuantity, fStopNum, cCheckState,iExpiratDateCalcu, ipeqty,ipenum) values(
                        '" + cwhcode + "','" + cinvcode + "'," + ItemId + ",'" + cbatch + "','',0,'',0,0,'" + cfree1 + "','" + cfree2 + @"',0,0,0,0,
                        '" + cfree3 + "','" + cfree4 + "','" + cfree5 + "','" + cfree6 + "','" + cfree7 + "','" + cfree8 + "','" + cfree9 + "','" + cfree10 + @"',0,0,0,
                        0,0,0,0,0,0,0,0,0,0,0,0,'',0,0,0)";
                        Cmd.ExecuteNonQuery();

                        currentID = GetDataString("select isnull(max(autoid),0) from " + dbname + "..currentstock where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
                        " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
                        " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "' " +
                        " and isnull(cbatch,'')='" + cbatch + "' and iSoType=0 and iSodid='' and cVMIVenCode=''", Cmd);
                    }
                    //更新现存量
                    Cmd.CommandText = "update " + dbname + "..currentstock set iquantity=iquantity+(" + iqty + "),inum=inum+isnull(" + inum + ",0),fInQuantity=fInQuantity-(" + iqty + "),fInNum=fInNum-isnull(" + inum + ",0) " +
                        "where autoid=" + currentID;
                    Cmd.ExecuteNonQuery();

                    #endregion
                }
            }

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


    [WebMethod]  //U8 产品报检单   返回单据 ID,单据号
    public string U8SCM_MOMOrder_TO_INSPECT(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误");

        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };

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
            string errmsg = "";
            string[] txtdata = null;  //临时取数

            #region  //逻辑检验
            txtdata = GetTextsFrom_FormData(FormData, "txt_mocode");
            if (txtdata[2].CompareTo("") == 0) { throw new Exception("请扫描流转卡条码"); }

            string modid = txtdata[2];
            string mocode = txtdata[3];
            string moid = GetDataString("select moid from " + dbname + "..mom_orderdetail where modid=" + modid, Cmd);
            txtdata = GetTextsFrom_FormData(FormData, "txt_cardno");
            string card_no = txtdata[3];
            string card_id = txtdata[2];

            //判断是否勾选 质检标识
            if (GetDataInt("select count(*) from " + dbname + "..mom_orderdetail where modid=0" + modid + " and QcFlag=0", Cmd) > 0)
                throw new Exception("生成订单没有勾选 质检");

            //部门
            txtdata = GetTextsFrom_FormData(FormData, "txt_cdepcode");
            string txt_tag = txtdata[2];
            if (txt_tag.CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + " 必须录入");
            }
            else
            {
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..department where cdepcode='" + txt_tag + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //自定义项
            System.Data.DataTable dtDefineCheck = GetSqlDataTable("select t_fieldname from " + dbname + "..T_CC_Base_GridCol_rule where SheetID='" + SheetID + @"' 
                    and (t_fieldname like '%define%' or t_fieldname like '%free%')", "dtDefineCheck", Cmd);
            for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
            {
                string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
                txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
                if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
                {
                    throw new Exception(txtdata[0] + "录入不正确");
                }
            }
            #endregion

            //显示标题   txt_字段名称    文本Tag     文本Text值
            string[] s_txt_iquantity = GetTextsFrom_FormData(FormData, "txt_iquantity");
            if (float.Parse(s_txt_iquantity[3]) <= 0) throw new Exception("入库数量必须大于0");
            string DBName = dbname;
            string targetAccId = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            #region//写主表
            KK_U8Com.U8QMINSPECTVOUCHER istmain = new KK_U8Com.U8QMINSPECTVOUCHER(Cmd, dbname);
            KK_U8Com.U8QMINSPECTVOUCHERS istdetail = new KK_U8Com.U8QMINSPECTVOUCHERS(Cmd, dbname);

            string vouCode = GetDataString("select 'QR'+right('0000000000'+cast(cast(isnull(max(replace(CINSPECTCODE,'QR','')),'0') as int)+1 as varchar(10)),8) from " + DBName + "..QMINSPECTVOUCHER where cvouchtype='QM02' and CINSPECTCODE like 'QR%'", Cmd);
            istmain.CINSPECTCODE = "'" + vouCode + "'";
            string cnewid = GetDataString("select newid()", Cmd);
            istmain.INSPECTGUID = "'" + cnewid + "'";
            istmain.ID = GetDataString("select isnull(max(id),0)+1 from " + DBName + "..QMINSPECTVOUCHER", Cmd);
            istmain.CVOUCHTYPE = "'QM02'";
            istmain.CSOORDERCODE = "'" + mocode + "'"; //生产订单号
            istmain.CSOURCEID = "" + moid;  //生产订单  主表标识
            istmain.CDEPCODE = "'" + GetTextsFrom_FormData(FormData, "txt_cdepcode")[2] + "'";
            istmain.CMAKER = "'" + cUserName + "'";
            istmain.DDATE = "'" + cLogDate + "'";
            istmain.CVERIFIER = "'" + cUserName + "'";
            istmain.DVERIFYDATE = "'" + cLogDate + "'";
            istmain.DVERIFYTIME = "getdate()";
            istmain.CTIME = "'" + GetDataString("select right(convert(varchar(20),getdate(),120),8)", Cmd) + "'";
            istmain.CSOURCE = "'生产订单'";
            istmain.IVTID = "352";
            istmain.CCHECKTYPECODE = "'PRO'";
            istmain.DMAKETIME = "getdate()";
            istmain.iPrintCount = "0";

            #region //主表自定义项
            istmain.CDEFINE1 = "'" + GetTextsFrom_FormData(FormData, "txt_define1")[3] + "'";
            istmain.CDEFINE2 = "'" + GetTextsFrom_FormData(FormData, "txt_define2")[3] + "'";
            istmain.CDEFINE3 = "'" + GetTextsFrom_FormData(FormData, "txt_define3")[3] + "'";
            istmain.CDEFINE4 = "'" + GetTextsFrom_FormData(FormData, "txt_define4")[3] + "'";
            txtdata = GetTextsFrom_FormData(FormData, "txt_define5");
            istmain.CDEFINE5 = (txtdata[3].CompareTo("") == 0 ? "0" : txtdata[3]);
            istmain.CDEFINE6 = "'" + GetTextsFrom_FormData(FormData, "txt_define6")[3] + "'";
            txtdata = GetTextsFrom_FormData(FormData, "txt_define7");
            istmain.CDEFINE7 = (txtdata[3].CompareTo("") == 0 ? "0" : txtdata[3]);
            istmain.CDEFINE8 = "'" + GetTextsFrom_FormData(FormData, "txt_define8")[3] + "'";
            istmain.CDEFINE9 = "'" + GetTextsFrom_FormData(FormData, "txt_define9")[3] + "'";
            istmain.CDEFINE10 = "'" + GetTextsFrom_FormData(FormData, "txt_define10")[3] + "'";
            istmain.CDEFINE11 = "'" + GetTextsFrom_FormData(FormData, "txt_define11")[3] + "'";
            istmain.CDEFINE12 = "'" + GetTextsFrom_FormData(FormData, "txt_define12")[3] + "'";
            istmain.CDEFINE13 = "'" + GetTextsFrom_FormData(FormData, "txt_define13")[3] + "'";
            istmain.CDEFINE14 = "'" + GetTextsFrom_FormData(FormData, "txt_define14")[3] + "'";
            txtdata = GetTextsFrom_FormData(FormData, "txt_define15");
            istmain.CDEFINE15 = (txtdata[3].CompareTo("") == 0 ? "0" : txtdata[3]);
            txtdata = GetTextsFrom_FormData(FormData, "txt_define16");
            istmain.CDEFINE16 = (txtdata[3].CompareTo("") == 0 ? "0" : txtdata[3]);
            #endregion

            if (!istmain.InsertToDB(ref errmsg)) { Cmd.Transaction.Rollback(); throw new Exception(errmsg); }
            #endregion

            #region  //子表
            //订单类型
            int cmom_type = int.Parse(GetDataString("select MoClass from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
            float f_Qty_in = float.Parse(s_txt_iquantity[3]);  //入库数量
            float f_qtyinall = float.Parse(GetDataString("select isnull(sum(iquantity),0) from " + DBName + "..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0", Cmd));
            string cc_invcode = GetTextsFrom_FormData(FormData, "txt_cinvcode")[3];

            #region//判断是否有关键子件
            //是否关键子件控制(库存 超生产订单入库 参数)
            string cst_option_chao = GetDataString("select cvalue from " + DBName + "..AccInformation where cname='bOverMPIn'", Cmd);
            int iKeysCount = int.Parse(GetDataString(@"select count(*) cn from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                    where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and a.ByproductFlag=0 ", Cmd));
            //关键领用控制量，取最小值  
            float f_ll_qty = float.Parse(GetDataString(@"select isnull(min(round(IssQty*BaseQtyD/BaseQtyN+0.5,0)),0) from " + DBName + "..mom_moallocate a inner join " + DBName + @"..Inventory_Sub i on a.InvCode=i.cInvSubCode
                    where a.MoDId=0" + modid + " and i.bInvKeyPart=1 and a.ByproductFlag=0 ", Cmd));
            if (f_ll_qty < f_Qty_in + f_qtyinall && iKeysCount > 0 && cst_option_chao.ToLower().CompareTo("true") == 0)
            {
                //判断是否  专员
                bool bExit = true;
                string czyuname = GetDataString("select isnull(cValue,'') from " + DBName + "..T_Parameter where cPid='mes_U81009_rd10_out_qxlist'", Cmd);
                if (czyuname.IndexOf(cUserName) < 0) throw new Exception("生产订单【" + mocode + "】存货【" + cc_invcode + "】超领料入库（或无关键物料）");
            }
            #endregion

            //子表
            istdetail.AUTOID = GetDataString("select isnull(max(autoid),0)+1 from " + DBName + "..QMINSPECTVOUCHERS", Cmd);
            istdetail.ID = istmain.ID;
            istdetail.SOURCEAUTOID = modid;
            istdetail.ITESTSTYLE = "0";
            istdetail.CINVCODE = "'" + cc_invcode + "'";
            //modid,mocode,soseq,cinvcode,cinvname,cinvstd,cunitname,balqualifiedqty,cbatch,bomtype
            istdetail.FQUANTITY = "" + f_Qty_in;  //入库数量
            istdetail.CBYPRODUCT = "'0'";
            istdetail.IORDERTYPE = "0";
            istdetail.IPROORDERID = moid;
            istdetail.CPROORDERCODE = "'" + mocode + "'";   //生产订单号
            istdetail.IPROORDERAUTOID = GetDataString("select sortseq from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd);   //生产订单行号
            istdetail.BEXIGENCY = "0";
            istdetail.ISOURCEPROORDERID = "0";
            istdetail.ISOURCEPROORDERAUTOID = "0";
            istdetail.CPROBATCH = "'" + GetDataString("select MoLotCode from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd) + "'";
            istdetail.iExpiratDateCalcu = "0";   //有效期推算方式  默认为0
            istdetail.PFCODE = "'" + card_no + "'";  //流转卡号

            string cToday = GetDataString("select convert(varchar(10),'" + cLogDate + "',120)", Cmd);
            #region//批次管理和保质期管理
            string cc_batch = GetTextsFrom_FormData(FormData, "txt_cbatch")[3];
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
            istdetail.CFREE1 = "'" + GetTextsFrom_FormData(FormData, "txt_free1")[3] + "'";
            istdetail.CFREE2 = "'" + GetTextsFrom_FormData(FormData, "txt_free2")[3] + "'";
            istdetail.CFREE3 = "'" + GetTextsFrom_FormData(FormData, "txt_free3")[3] + "'";
            istdetail.CFREE4 = "'" + GetTextsFrom_FormData(FormData, "txt_free4")[3] + "'";
            istdetail.CFREE5 = "'" + GetTextsFrom_FormData(FormData, "txt_free5")[3] + "'";
            istdetail.CFREE6 = "'" + GetTextsFrom_FormData(FormData, "txt_free6")[3] + "'";
            istdetail.CFREE7 = "'" + GetTextsFrom_FormData(FormData, "txt_free7")[3] + "'";
            istdetail.CFREE8 = "'" + GetTextsFrom_FormData(FormData, "txt_free8")[3] + "'";
            istdetail.CFREE9 = "'" + GetTextsFrom_FormData(FormData, "txt_free9")[3] + "'";
            istdetail.CFREE10 = "'" + GetTextsFrom_FormData(FormData, "txt_free10")[3] + "'";

            istdetail.CDEFINE22 = "'" + GetTextsFrom_FormData(FormData, "txt_define22")[3] + "'";
            istdetail.CDEFINE23 = "'" + GetTextsFrom_FormData(FormData, "txt_define23")[3] + "'";
            istdetail.CDEFINE24 = "'" + GetTextsFrom_FormData(FormData, "txt_define24")[3] + "'";
            istdetail.CDEFINE25 = "'" + GetTextsFrom_FormData(FormData, "txt_define25")[3] + "'";
            txtdata = GetTextsFrom_FormData(FormData, "txt_define26");
            istdetail.CDEFINE26 = (txtdata[3] == "" ? "0" : txtdata[3]);
            txtdata = GetTextsFrom_FormData(FormData, "txt_define27");
            istdetail.CDEFINE27 = (txtdata[3] == "" ? "0" : txtdata[3]);
            istdetail.CDEFINE28 = "'" + GetTextsFrom_FormData(FormData, "txt_define28")[3] + "'";
            istdetail.CDEFINE29 = "'" + GetTextsFrom_FormData(FormData, "txt_define29")[3] + "'";
            istdetail.CDEFINE30 = "'" + GetTextsFrom_FormData(FormData, "txt_define30")[3] + "'";
            istdetail.CDEFINE31 = "'" + GetTextsFrom_FormData(FormData, "txt_define31")[3] + "'";
            istdetail.CDEFINE32 = "'" + GetTextsFrom_FormData(FormData, "txt_define32")[3] + "'";
            istdetail.CDEFINE33 = "'" + GetTextsFrom_FormData(FormData, "txt_define33")[3] + "'";
            txtdata = GetTextsFrom_FormData(FormData, "txt_define34");
            istdetail.CDEFINE34 = (txtdata[3] == "" ? "0" : txtdata[3]);
            txtdata = GetTextsFrom_FormData(FormData, "txt_define35");
            istdetail.CDEFINE35 = (txtdata[3] == "" ? "0" : txtdata[3]);
            istdetail.CDEFINE36 = "'" + GetTextsFrom_FormData(FormData, "txt_define36")[3] + "'";
            istdetail.CDEFINE37 = "'" + GetTextsFrom_FormData(FormData, "txt_define37")[3] + "'";
            #endregion

            if (!istdetail.InsertToDB(ref errmsg)) { Cmd.Transaction.Rollback(); throw new Exception(errmsg); }

            //回写 生产订单
            Cmd.CommandText = "update " + DBName + "..mom_orderdetail set DeclaredQty=isnull(DeclaredQty,0)+(0" + istdetail.FQUANTITY + ") where modid=0" + modid;
            Cmd.ExecuteNonQuery();
            #endregion

            #region  //上下游单据关系
            //建立流转卡和 入库单的关联关系 
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_Inspect_FlowCard(t_autoid,t_card_id) values(" + istdetail.AUTOID + "," + card_id + ")";
            Cmd.ExecuteNonQuery();

            string cols = "";
            //判定上下工序逻辑关系
            float fRDIn = 0;
            fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(max(t_card_overqty),0) from " + dbname + "..T_CC_Card_List where t_card_id=0" + card_id));

            float f_rd10_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(sum(b.fquantity),0) from " + dbname + @"..T_CC_Inspect_FlowCard a 
                inner join " + dbname + @"..QMINSPECTVOUCHERS b on a.t_autoid=b.autoid where a.t_card_id=0" + card_id));
            if (fRDIn < f_rd10_qty) throw new Exception("本流转卡可报检【" + fRDIn + "】");

            #endregion

            Cmd.Transaction.Commit();

            return istmain.ID + "," + vouCode;
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


    [WebMethod]  //MES流转卡 开卡处理
    public bool MES_Card_Start(string card_no, string dbname, string cUserName, string cLogDate)
    {
        return MES_Card_Start_qty(card_no, "", dbname, cUserName, cLogDate);
    }

    [WebMethod]  //MES流转卡 开卡处理
    public bool MES_Card_Start_qty(string card_no, string new_qty, string dbname, string cUserName, string cLogDate)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //流转卡信息
            DataTable dtCardInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select t_card_id,t_modid,t_card_qty,t_dayorderid from " + dbname + "..T_CC_Card_List where t_card_no='" + card_no + "'");
            if (dtCardInfo.Rows.Count == 0) throw new Exception("没有找到流转卡信息");

            //判断是否 非开卡状态
            if (float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select count(*) from " + dbname + "..T_CC_Card_List where t_card_no='" + card_no + "' and t_state<>'开卡'")) > 0)
                throw new Exception("不能重复开卡（非初始状态）");

            //是否领料(按卡领料控制)
            string cpvalue = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_lingliao_control'");
            if (cpvalue.Trim().CompareTo("1") == 0)
            {
                if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select count(*) from " + dbname + "..T_CC_HF_FlowMertiral where cc_pfid=0" + dtCardInfo.Rows[0]["t_card_id"])) == 0)
                    throw new Exception("[" + card_no + "]没有领料记录，不能开卡");
            }
            //按照生产订单控制领料
            if (cpvalue.Trim().CompareTo("2") == 0)
            {
                if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select COUNT(*) from " + dbname + "..mom_moallocate where modid=0" + dtCardInfo.Rows[0]["t_modid"] + " and ByproductFlag=0 and ISNULL(IssQty,0)>0")) == 0)
                    throw new Exception("[" + card_no + "]对应生产订单没有领料记录，不能开卡");
            }

            if (new_qty == "")
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_Card_List set t_state='流转',t_startdate=convert(varchar(10),getdate(),120),
                    t_enddate=case when DATEDIFF(day,t_enddate,GETDATE())>0 then convert(varchar(10),getdate(),120) else t_enddate end 
                    where t_card_id=" + dtCardInfo.Rows[0]["t_card_id"];
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_Card_List set t_state='流转',t_card_qty=0" + new_qty + @",t_startdate=convert(varchar(10),getdate(),120),
                    t_enddate=case when DATEDIFF(day,t_enddate,GETDATE())>0 then convert(varchar(10),getdate(),120) else t_enddate end 
                    where t_card_id=" + dtCardInfo.Rows[0]["t_card_id"];
                Cmd.ExecuteNonQuery();

                //是否超生产订单量
                float forderqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select qty from " + dbname + "..mom_orderdetail where modid=0" + dtCardInfo.Rows[0]["t_modid"]));
                float fpcqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(sum(t_card_qty),0) from " + dbname + "..T_CC_Card_List where t_modid=0" + dtCardInfo.Rows[0]["t_modid"]));
                if (forderqty < fpcqty) throw new Exception("不能超生产订单数量制作流转卡");

                //判断是否超排产数量  
                forderqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_qty from " + dbname + "..T_CC_MorderToDay where t_id=0" + dtCardInfo.Rows[0]["t_dayorderid"]));
                fpcqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(sum(t_card_qty),0) from " + dbname + "..T_CC_Card_List where t_dayorderid=0" + dtCardInfo.Rows[0]["t_dayorderid"]));
                if (forderqty < fpcqty) throw new Exception("不能超排产数量制作流转卡");
            }


            string flow_qty = "" + dtCardInfo.Rows[0]["t_card_qty"];
            string c_pro_cid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select top 1 t_c_id from " + dbname + "..T_CC_Cards_process where t_card_no='" + card_no + "' order by t_card_seq");
            if (c_pro_cid.CompareTo("") != 0)
            {
                string c_contiion = "";
                string cpp_pvalue = GetDataString("select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_start_auto'", Cmd);
                if (cpp_pvalue.Trim().ToLower().CompareTo("1") == 0)  //流转卡开卡时，是否自动第一道工序开工
                {
                    Cmd.CommandText = "update " + dbname + "..T_CC_Cards_process set t_starttime=getdate() where t_c_id=" + c_pro_cid;
                    Cmd.ExecuteNonQuery();
                    c_contiion = " t_starttime=getdate(),";
                }
                Cmd.CommandText = "update " + dbname + "..T_CC_Cards_report set " + c_contiion + "t_tran_in_qty=0" + flow_qty + @" 
                            where t_c_id in(select t_report_c_id from " + dbname + "..T_CC_Cards_process where t_c_id=" + c_pro_cid + ") and t_starttime is null";
                Cmd.ExecuteNonQuery();

            }

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

    [WebMethod]  //MES流转卡 工序开工处理
    public bool MES_OpCode_Start(string op_barcode, string dbname, string cUserName, string cLogDate)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //工序信息 
            DataTable dtCardInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_card_no,t_report_c_id,t_starttime,t_c_id,t_card_seq,t_opcode,t_wc_code
                from " + dbname + "..T_CC_Cards_process where t_barcode='" + op_barcode + "'");

            if (dtCardInfo.Rows.Count == 0) throw new Exception("没有找到流转卡信息");
            if (dtCardInfo.Rows[0]["t_starttime"] + "" != "") throw new Exception("工序已经开工,不能重复开工");

            //判断流转卡状态
            if (GetDataInt("select COUNT(*) from " + dbname + "..T_CC_Card_List where t_card_no='" + dtCardInfo.Rows[0]["t_card_no"] + "' and t_state<>'流转'", Cmd) > 0)
                throw new Exception("非流转状态流转卡不能 使用工序开工功能");

            //判断是否控制上下工序开工顺序
            string cpvalue = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_starttime_control'");
            if (cpvalue.Trim().ToLower().CompareTo("true") == 0)
            {
                string cUp_report_c_id = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_up_c_id FROM " + dbname + @"..T_CC_Cards_report 
                    where t_c_id=0" + dtCardInfo.Rows[0]["t_report_c_id"]);
                if (cUp_report_c_id == "") cUp_report_c_id = "0";
                //判断是否存在上报告工序报工数据
                if (int.Parse(cUp_report_c_id) > 0 && GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_pro_report_list where t_report_c_id=0" + cUp_report_c_id, Cmd) == 0)
                    throw new Exception("上工序没有报工记录，不能开工");
            }

            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_process set t_starttime=getdate() where t_c_id=" + dtCardInfo.Rows[0]["t_c_id"];
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_starttime=getdate() 
                where t_c_id in(select t_report_c_id from " + dbname + "..T_CC_Cards_process where t_c_id=" + dtCardInfo.Rows[0]["t_c_id"] + ") and t_starttime is null";
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

    private bool mesWorkStart(SqlCommand Cmd, string eq_barcode, string dbname, string cUsercode, string cUserName)
    {
        try
        {
            string cPsn_CCode = "";
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_workno_eq_by_login'", Cmd).ToLower().CompareTo("true") == 0)
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..UserHrPersonContro where cUser_Id='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前操作员无业务员编码，请设置人员档案 【是否操作员】项");
            }
            else
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..hr_hi_person where cPsn_Num='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前人员不存在");
            }

            //上岗信息 
            DataTable dtWorkInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select t_workno_time,t_workoff_time from " + dbname + @"..T_CC_Work_Time 
                where t_psncode='" + cPsn_CCode + "' and t_eq_no='" + eq_barcode + "' and t_workoff_time is null");
            if (dtWorkInfo.Rows.Count > 0) throw new Exception("未下岗，相同设备不能重复上岗");

            //判断是否扫描设备号 上岗
            string cpvalue = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_workno_eq_control'");
            if (cpvalue.Trim().ToLower().CompareTo("true") == 0)
            {
                if (eq_barcode.CompareTo("") == 0) throw new Exception("请扫码设备条码");
                if (GetDataInt("select COUNT(*) from " + dbname + @"..v_cqcc_EQ_EQData where cEQCode='" + eq_barcode + "'", Cmd) == 0)
                    throw new Exception("设备条码不存在");
            }

            //用户设备操作资质设置
            string ctypecode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cEQTypeCode from " + dbname + @"..EQ_EQData where cEQCode='" + eq_barcode + "'");
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_EQ_OperSetting where t_eqcode='" + ctypecode + "'", Cmd) > 0)
            {
                //此设备有资质管理
                if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_EQ_OperSetting where t_eqcode='" + ctypecode + "' and t_psncode='" + cPsn_CCode + "'", Cmd) == 0)
                    throw new Exception("当前员工不具备操作此设备资质");
            }

            Cmd.CommandText = "insert into " + dbname + "..T_CC_Work_Time(t_psncode,t_eq_no,t_workno_time,t_workno_log_user) values('" + cPsn_CCode + "','" + eq_barcode + "',getdate(),'" + cUserName + "')";
            Cmd.ExecuteNonQuery();

            //关联设备关停机处理  true 代表需要关联
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_workno_eq_relation_eqstate'", Cmd).ToLower().CompareTo("true") == 0)
            {
                eq_work_start(Cmd, dbname, eq_barcode, cUserName, false);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod]  //MES   上岗处理
    public bool MES_Work_Start(string eq_barcode, string dbname, string cUsercode, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();
        try
        {
            mesWorkStart(sCmd, eq_barcode, dbname, cUsercode, cUserName);
            sCmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod(Description = @"接口：扫码设备上岗。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_barcode 设备号ID；<br/>
            cUsercode 用户登录号 null<br/>
            cUserName 用户登录姓名，没有时传入 null<br/>
            ")]
    public string MES_Work_Start_Json(string dbname, string eq_barcode, string cUsercode, string cUserName)
    {
        try
        {
            MES_Work_Start(eq_barcode, dbname, cUsercode, cUserName);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    // whf
    [WebMethod(Description = @"接口：扫码设备上岗(多产品)。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_barcode 设备号ID；<br/>
            cUsercode 用户登录号 null<br/>
            cUserName 用户登录姓名，没有时传入 null<br/>
            invCodes 多产品集合,以逗号分隔<br/>
            ")]
    public string MES_Work_MultiInv_Start_Json(string dbname, string eq_barcode, string cUsercode, string cUserName, string invCodes)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            return "数据库连接失败";
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string cPsn_CCode = "";
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_workno_eq_by_login'", Cmd).ToLower().CompareTo("true") == 0)
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..UserHrPersonContro where cUser_Id='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前操作员无业务员编码，请设置人员档案 【是否操作员】项");
            }
            else
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..hr_hi_person where cPsn_Num='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前人员不存在");
            }
            //上岗信息 
            DataTable dtWorkInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select t_workno_time,t_workoff_time from " + dbname + @"..T_CC_Work_Time 
                where t_psncode='" + cPsn_CCode + "' and t_eq_no='" + eq_barcode + "' and t_workoff_time is null");
            if (dtWorkInfo.Rows.Count == 0)
            {
                // 不重复上岗(没有上岗记录才上岗)
                mesWorkStart(Cmd, eq_barcode, dbname, cUsercode, cUserName);
            }
            if (!string.IsNullOrEmpty(invCodes))
            {
                

                //throw new Exception("多产品上岗,产品记录不能为空");
                string[] arr = invCodes.Split(',');
                // 删除所有产品记录重新插入
                Cmd.CommandText = "delete from " + dbname + "..t_cqcc_MultiInvWork where eqCode='" + eq_barcode + "' and personCode = '" + cPsn_CCode + "';";
                Cmd.ExecuteNonQuery();
                for (int i = 0; i < arr.Length; i++)
                {
                    Cmd.CommandText = "insert into " + dbname + "..t_cqcc_MultiInvWork(eqCode, personCode, cInvCode) values ('" + eq_barcode + "','" + cPsn_CCode + "','" + arr[i] + "')";
                    Cmd.ExecuteNonQuery();
                }
                // 获取上岗id
                string t_autoid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_autoid from " + dbname + @"..T_CC_Work_Time where t_psncode='" + cPsn_CCode + "' and t_eq_no='" + eq_barcode + "' and t_workoff_time is null ");
                if (!string.IsNullOrEmpty(t_autoid))
                {
                    DataTable existDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select * from " + dbname + "..t_cqcc_MultiInvWorkOnSave where t_autoid="+t_autoid);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (existDt.Select(" eqCode='" + eq_barcode + "' and personCode='" + cPsn_CCode + "' and cInvCode='" + arr[i] + "'").Length > 0)
                        {
                            // 已经存在的不用插入
                            continue;
                        }
                        Cmd.CommandText = "insert into " + dbname + "..t_cqcc_MultiInvWorkOnSave(t_autoid,eqCode, personCode, cInvCode) values ("+t_autoid+",'" + eq_barcode + "','" + cPsn_CCode + "','" + arr[i] + "')";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            Cmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    private bool mesWorkOff(SqlCommand Cmd, string dbname, string cUsercode, string cUserName, string eq_barcode)
    {
        try
        {
            string cPsn_CCode = "";
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_workno_eq_by_login'", Cmd).ToLower().CompareTo("true") == 0)
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..UserHrPersonContro where cUser_Id='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前操作员无业务员编码，请设置人员档案 【是否操作员】项");
            }
            else
            {
                cPsn_CCode = "" + GetDataString("select cPsn_Num from " + dbname + "..hr_hi_person where cPsn_Num='" + cUsercode + "'", Cmd);
                if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前人员不存在");
            }

            if (cPsn_CCode.CompareTo("") == 0) throw new Exception("当前操作员无业务员编码，请设置人员档案 【是否操作员】项");
            //上岗信息 
            DataTable dtWorkInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select t_workno_time,t_workoff_time from " + dbname + "..T_CC_Work_Time where t_psncode='" + cPsn_CCode + "' and t_workoff_time is null");
            if (dtWorkInfo.Rows.Count == 0) throw new Exception("无上岗记录，不能重复下岗");

            //关联设备关停机处理  true 代表需要关联
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_workno_eq_relation_eqstate'", Cmd).ToLower().CompareTo("true") == 0)
            {
                string stop_code = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_run_stop_fromworkoff_code'", Cmd);
                if (stop_code == "") throw new Exception("请维护参数【设备停机原因-下岗时自动停机原因代码】");

                if (eq_barcode == "")
                {
                    DataTable dtEqList = GetSqlDataTable("select distinct t_eq_no from " + dbname + @"..T_CC_Work_Time 
                        where t_psncode='" + cPsn_CCode + "' and t_workoff_time is null", "dtEqList", Cmd);
                    for (int e = 0; e < dtEqList.Rows.Count; e++)
                    {
                        eq_work_stop(Cmd, dbname, "" + dtEqList.Rows[e]["t_eq_no"], stop_code, cUserName);
                    }
                }
                else
                {
                    eq_work_stop(Cmd, dbname, eq_barcode, stop_code, cUserName);
                }
            }

            //下岗处理
            if (eq_barcode == "")
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_Work_Time set t_workoff_time=getdate(),t_workoff_log_user='" + cUserName + "' where t_psncode='" + cPsn_CCode + "' and t_workoff_time is null";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_EQData where cEQCode='" + eq_barcode + "'", Cmd) == 0)
                    throw new Exception("设备[" + eq_barcode + "]不存在");

                Cmd.CommandText = "update " + dbname + "..T_CC_Work_Time set t_workoff_time=getdate(),t_workoff_log_user='" + cUserName + @"' 
                    where t_psncode='" + cPsn_CCode + "' and t_eq_no='" + eq_barcode + "' and t_workoff_time is null";
                Cmd.ExecuteNonQuery();
            }
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod]  //MES   下岗处理(所有设备下岗)
    public bool MES_Work_OFF(string dbname, string cUsercode, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();
        try
        {
            mesWorkOff(sCmd, dbname, cUsercode, cUserName, "");
            sCmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    [WebMethod]  //MES   下岗处理(多产品下岗)
    public string MES_Work_MultiInv__OFF_Json(string dbname, string cUsercode, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            return "数据库连接失败";
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();
        try
        {
            mesWorkOff(sCmd, dbname, cUsercode, cUserName, "");
            sCmd.CommandText = "delete from " + dbname + "..t_cqcc_MultiInvWork where personCode='"+cUsercode+"'";
            sCmd.ExecuteNonQuery();
            sCmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }


    [WebMethod]  //MES   下岗处理(单设备下岗)
    public bool MES_Work_OFF_eqno(string dbname, string cUsercode, string cUserName, string eq_no)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();
        try
        {
            mesWorkOff(sCmd, dbname, cUsercode, cUserName, eq_no);
            sCmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod(Description = @"接口：扫码设备下岗。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_barcode 设备号ID；<br/>
            cUsercode 用户登录号 null<br/>
            cUserName 用户登录姓名，没有时传入 null<br/>
            ")]
    public string MES_Work_OFF_Json(string dbname, string eq_barcode, string cUsercode, string cUserName)
    {
        try
        {
            MES_Work_OFF_eqno(dbname, cUsercode, cUserName, eq_barcode);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }


    [WebMethod]  //扫描流转卡 报工（单缺陷报工）
    public string MES_Report(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        return MES_Report_Multi(FormData, dbname, cUserName, cLogDate, SheetID, null);
    }

    [WebMethod]  //扫描流转卡 报工（多缺陷报工）
    public string MES_Report_Multi_JSON(string FormDataJSON, string dbname, string cUserName, string cLogDate, string SheetID, string hsValue)
    {
        DataTable FormData = VendorIO.JsonToDataTable(FormDataJSON);
        string[] hsValueArr = hsValue.Split('@');
        return MES_Report_Multi(FormData, dbname, cUserName, cLogDate, SheetID, hsValueArr);
    }

    [WebMethod]  //扫描流转卡 报工（多缺陷报工）
    public string MES_Report_Multi(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID, string[] hsValue)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            //判断是否有序列号档案
            string c_process_did = GetTextsFrom_FormData_Tag(FormData, "txt_flow_seq");  //报工流转卡明细ID
            if (c_process_did.CompareTo("") == 0) throw new Exception("没有获得工序信息");
            if (GetDataInt(@"select count(*) from " + dbname + @"..T_CC_Cards_process a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_no=b.t_card_no 
	                inner join " + dbname + @"..T_CC_Cards_Sn c on b.t_card_id=c.t_card_id
                where a.t_c_id=0" + c_process_did, Cmd) > 0)
                throw new Exception("本卡存在序列号，只能使用序列号报工");
            bool b_defect_multiInput = false;
            if (GetDataString("select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_qc_defect_inputMode'", Cmd).ToLower().CompareTo("true") == 0)
            {
                b_defect_multiInput = true;//多缺陷录入模式 
            }

            #region //流转卡报工逻辑判定
            DataTable dtProcessCardInfo = GetSqlDataTable("select t_opcode,t_wc_code,t_card_no,t_card_seq,t_report_c_id from " + dbname + @"..T_CC_Cards_process 
                where t_c_id=" + c_process_did, "dtProcessCardInfo", Cmd);
            if (dtProcessCardInfo.Rows.Count == 0) throw new Exception("流转卡信息不存在，请确认条码正确性");

            string c_process_seqno = dtProcessCardInfo.Rows[0]["t_card_seq"] + "";  //报工流转卡明细ID
            string c_t_cardno = dtProcessCardInfo.Rows[0]["t_card_no"] + "";
            // whf--检查当前报工时间不能早于流转卡制单时间以及上次报工时间
            CheckReportDate(Cmd, dbname,c_t_cardno, cLogDate);
            //流转卡状态判定
            DataTable dtCardInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_isfather_card,t_islast_card,t_state,t_soure,isnull(b.t_card_seq,'') t_chai_seqno,a.t_invcode,a.t_modid 
                from " + dbname + @"..T_CC_Card_List a inner join " + dbname + @"..t_cqcc_mutilSplit b on a.t_card_no=b.cardNo
                where a.t_card_no='" + c_t_cardno + "'");
            if (dtCardInfo.Rows.Count == 0)
            {
                dtCardInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_isfather_card,t_islast_card,t_state,t_soure,isnull(b.t_seqno,'') t_chai_seqno,a.t_invcode,a.t_modid 
                from " + dbname + @"..T_CC_Card_List a left join " + dbname + @"..T_CC_FlowCard_Sp_Main b on a.t_card_no=b.t_card_no
                where a.t_card_no='" + c_t_cardno + "'");
            }
            if (dtCardInfo.Rows.Count == 0) throw new Exception("没有找到主流转卡信息");
            if (dtCardInfo.Rows[0]["t_islast_card"] + "" != "1")
            {
                //拆分前 工序可以报工，拆分后的工序不能报工
                if (("" + dtCardInfo.Rows[0]["t_chai_seqno"]).CompareTo(c_process_seqno) <= 0)
                    throw new Exception("母卡拆分后工序不能报工");
                    //throw new Exception("非末级卡（或拆后原卡）不能报工");
            }

            string t_is_operation_qc = GetTextsFrom_FormData_Tag(FormData, "txt_is_operation_qc");  //是否送检标识 
            if (dtCardInfo.Rows[0]["t_state"] + "" != "流转" && t_is_operation_qc != "是") throw new Exception("只有 流转 状态的流转卡才能报工");
            #endregion

            string cRet_Code = "";
            string c_save_batchno = "";
            string cc_inv_code = "";//产品编码
            DataTable dtRptRecord = null; //报工行数
            string t_eqno = GetTextsFrom_FormData_Tag(FormData, "txt_t_eq_no");

            string t_opcode = dtProcessCardInfo.Rows[0]["t_opcode"] + "";
            string t_wccode = dtProcessCardInfo.Rows[0]["t_wc_code"] + "";
            string t_cardno = dtProcessCardInfo.Rows[0]["t_card_no"] + "";


            if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + t_cardno + "' and ISNULL(a.CloseUser,'')<>''") > 0)
                throw new Exception("生产订单已经关闭");

            float fvalid = 0; float fscrap_work = 0; float fscrap_material = 0; float fscrap_qc = 0; float fmiss_qty = 0; float frepair_qty = 0; float frenew_qty = 0;
            float fcompleteAll = 0;
            string c_reason_code = GetTextsFrom_FormData_Tag(FormData, "txt_t_reason_code");
            #region  //报工数据获得
            try
            {
                string cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_valid_qty");
                fvalid = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_work");
                fscrap_work = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_material");
                fscrap_material = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_qc");
                fscrap_qc = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_miss_qty");
                fmiss_qty = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_repair_qty");
                frepair_qty = float.Parse((cf_value == "" ? "0" : cf_value));
                cf_value = GetTextsFrom_FormData_Text(FormData, "txt_t_renew_qty");
                frenew_qty = float.Parse((cf_value == "" ? "0" : cf_value));

                fcompleteAll = fvalid + fscrap_work + fscrap_material + fscrap_qc + fmiss_qty + frepair_qty + frenew_qty;
                
                #region  //判断参数，当前报工数量是否与在制量一致
                if (fcompleteAll > 0 && UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from " + dbname + @"..T_Parameter 
                    where cPid='mes_flow_report_rpt_qty_control'").ToLower().CompareTo("true") == 0)
                {
                    float d_bg_qty_bal = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_working_qty+t_working_ware_qty from " + dbname + @"..T_CC_Cards_report 
                        where t_c_id=0" + dtProcessCardInfo.Rows[0]["t_report_c_id"]));
                    if (fcompleteAll != d_bg_qty_bal) throw new Exception("报工数合计不等于待加工数，请重新录入,待加工量为：" + d_bg_qty_bal);
                }
                #endregion
                #region  // 判断参数,是否控制报工废料废的角色
                DataTable scrapRoleDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select cpid,cvalue from " + dbname + "..T_parameter where cpid in ('mes_report_canReportScrapWorkRole','mes_report_canReportScrapMaterialRole')");
                foreach (DataRow row in scrapRoleDt.Rows)
                {
                    string cpid = row["cpid"] + "";
                    string cvalue = row["cvalue"] + "";
                    if (string.IsNullOrEmpty(cvalue) || 
                        UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from ufsystem..UA_Role a inner join ufsystem..UA_User u on a.cUser_Id = u.cUser_Id 
where u.cUser_Name='"+cUserName+"' and a.cGroup_Id='"+cvalue+"'")>0)
                    {
                        continue;
                    }
                    if (cpid == "mes_report_canReportScrapWorkRole" && fscrap_work>0)
                    {
                        throw new Exception("当前用户不能报工废");
                    }
                    else if (cpid == "mes_report_canReportScrapMaterialRole" && fscrap_material>0)
                    {
                        throw new Exception("当前用户不能报料废");
                    }
                }
                #endregion

                //多缺陷录入模式
                if (b_defect_multiInput)
                {
                    //获得详细输入规则
                    bool b_work_input = true; bool b_mer_input = true; bool b_qc_input = true; bool b_repair_input = true;
                    DataTable dtrule = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select input_control from " + dbname + @"..T_CC_pro_report_defect_inputRule where isnull(defect_can_input,0)=0");
                    for (int l = 0; l < dtrule.Rows.Count; l++)
                    {
                        if (dtrule.Rows[l]["input_control"] + "" == "txt_t_scrap_work") b_work_input = false;
                        if (dtrule.Rows[l]["input_control"] + "" == "txt_t_scrap_material") b_mer_input = false;
                        if (dtrule.Rows[l]["input_control"] + "" == "txt_t_scrap_qc") b_qc_input = false;
                        if (dtrule.Rows[l]["input_control"] + "" == "txt_t_repair_qty") b_repair_input = false;
                    }
                    if (b_work_input && fscrap_work > 0 && hsValue[0] == "") throw new Exception("有废品或返修数，请输入缺陷信息");
                    if (b_mer_input && fscrap_material > 0 && hsValue[1] == "") throw new Exception("有废品或返修数，请输入缺陷信息");
                    if (b_qc_input && fscrap_qc > 0 && hsValue[2] == "") throw new Exception("有废品或返修数，请输入缺陷信息");
                    if (b_repair_input && frepair_qty > 0 && hsValue[3] == "") throw new Exception("有废品或返修数，请输入缺陷信息");
                }
            }
            catch(FormatException)
            {
                throw new Exception("报工数量录入不正确，请输入数字");
            }
            #endregion

            #region  //连续制造 业务处理
            bool b_line_report = false;

            cc_inv_code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select t_invcode from " + dbname + @"..T_CC_Card_List where t_card_no='" + t_cardno + "'");
            //string is_last_opSql = "";
            //if (fvalid > 0)
            //{
            //    //or t_is_first_op='是'
            //    is_last_opSql += " and (t_is_last_op='是' ) ";
            //}
            string rptSql = @"select t_eqteam_code,t_is_first_op,t_is_last_op,t_seq_no from " + dbname + @"..T_CC_LineEQ_Manufacture 
                                where t_invcode='" + cc_inv_code + "' and t_wccode='" + t_wccode + "' and t_opcode='" + t_opcode + "' and t_eq_no='" + t_eqno + "'  order by t_seq_no";
            DataTable dtRptOpInfo = GetSqlDataTable(rptSql, "dtRptOpInfo", Cmd);
            // 获取不需要走连线的订单类型
            string noLineMoTypeCode = GetDataString("select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_noLineMoTypeCode'", Cmd);
            if (string.IsNullOrEmpty(noLineMoTypeCode))
            {
                noLineMoTypeCode += "''";
            }
            // 是否不需要走连线的订单
            bool isRepair = GetSqlDataTable(@"
select 1 from " + dbname + @"..T_CC_Card_List a
inner join " + dbname + @"..mom_orderdetail d on d.MoDId = a.t_modid
inner join " + dbname + @"..mom_motype t on t.motypeid = d.MoTypeId
where a.t_card_no='" +t_cardno+@"' and t.MotypeCode in ("+noLineMoTypeCode+@")
", "fanxiuTable", Cmd).Rows.Count>0;
            if (!isRepair)
            {
                //临时跳过连线的流转卡号
                isRepair = GetSqlDataTable(@"
select 1 from " + dbname + @"..t_cqcc_TempNoLineCardNo a
where a.t_card_no='" + t_cardno + @"' 
", "fanxiuTable", Cmd).Rows.Count > 0;
            }
            // 是否已经参与过连线
            bool isLianAlready = GetDataInt(@"select COUNT(*) from " + dbname + @"..T_CC_pro_report_list where t_report_c_id=0" + dtProcessCardInfo.Rows[0]["t_report_c_id"] + " and isnull(t_line_m_rpt,'')<>''", Cmd) > 0;
            if (isLianAlready && dtRptOpInfo.Rows.Count==0)
            {
                throw new Exception("当前工序已经报过连线,但是无法匹配连线档案,请输入正确设备");
            }
            //bool isLian = LianMate(Cmd, dbname, cc_inv_code, t_wccode, t_opcode, t_eqno, t_cardno, c_process_did, ref dtRptOpInfo, ref dtRptRecord);
            if (isLianAlready || (dtRptOpInfo.Rows.Count > 0 && !isRepair) )
            //if (isLian)
            {
                if (dtRptOpInfo.Rows.Count > 1)
                {
                    throw new Exception("当前条件匹配到多个连线分组");
                }
                b_line_report = true;
                c_save_batchno = GetDataString(@"select newid()", Cmd);
                // 获取连线工序数--whf
                string lianNum = dtRptOpInfo.Rows[0]["t_seq_no"]+"";
                //int lianNum = GetDataInt("select count(*) c from  " + dbname + @"..T_CC_LineEQ_Manufacture a where  a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "' ", Cmd);
//                int lianNum = GetDataInt(@"
//select t_seq_no from (
//select row_number() over(partition by t_card_no order by t_c_id) t_seq_no,* from " + dbname + @"..T_CC_Cards_process
//where t_card_no='" + t_cardno + @"' 
//) t where t.t_c_id =" + c_process_did, Cmd);
//                dtRptRecord = GetSqlDataTable(@"select a.t_opcode,a.t_wccode,a.t_eq_no,a.t_is_first_op,a.t_is_last_op,b.t_c_id process_did,b.t_card_seq,b.t_starttime,b.t_report_c_id 
//                                        from " + dbname + @"..T_CC_LineEQ_Manufacture a inner join 
//                                        (select row_number() over(partition by t_card_no order by t_c_id) t_seq_no,* from " + dbname + @"..T_CC_Cards_process where t_card_no='" + t_cardno + @"'
//                                            ) b 
//                                            on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode='" + cc_inv_code + @"' 
//                                        where a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "' order by a.t_seq_no", "dtRptRecord", Cmd);
                dtRptRecord = GetSqlDataTable(@"select a.t_opcode,a.t_wccode,a.t_eq_no,a.t_is_first_op,a.t_is_last_op,b.t_c_id process_did,b.t_card_seq,b.t_starttime,b.t_report_c_id 
                                                        from " + dbname + @"..T_CC_LineEQ_Manufacture a inner join 
                                                        (select row_number() over(partition by t_card_no order by t_c_id) t_seq_no,* from " + dbname + @"..T_CC_Cards_process where t_card_no='" + t_cardno + @"'
                                                          and t_c_id<=" + c_process_did + @" and t_c_id>" + c_process_did + @"-" + lianNum + @"  ) b 
                                                            on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode='" + cc_inv_code + @"' and b.t_seq_no=a.t_seq_no
                                                        where a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "' order by a.t_seq_no", "dtRptRecord", Cmd);
                if (dtRptRecord.Rows.Count == 0) throw new Exception("连续制造模式 设置错误，未找到有效连续分组数据[" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "]");
                //if (dtRptRecord.Rows[0]["process_did"] + "" == c_process_did)
                //{
                //    throw new Exception("连续制造模式 首序不能报工");
                //}
                // 参数mes_flow_lineStartTimeOpControl='是'时首序可以不开工
                string mes_flow_lineStartTimeOpControl = GetDataString("select cvalue from " + dbname + "..t_parameter where cpid = 'mes_flow_lineStartTimeOpControl'", Cmd);
                if (mes_flow_lineStartTimeOpControl == "是")
                {
                    if (dtRptRecord.Rows[0]["t_starttime"] + "" == "")
                    {
                        dtRptRecord.Rows[0]["t_starttime"] = GetDataString("select CONVERT(varchar(50),getdate(),120)", Cmd);
                        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_process set t_starttime='" + dtRptRecord.Rows[0]["t_starttime"] + @"' 
                            where t_c_id=0" + dtRptRecord.Rows[0]["process_did"];
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_starttime='" + dtRptRecord.Rows[0]["t_starttime"] + @"' 
                            where t_c_id=0" + dtRptRecord.Rows[0]["t_report_c_id"] + " and t_starttime is null";
                        Cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    //连线首工序必须 做【工序开工处理】
                    if (dtRptRecord.Rows[0]["t_starttime"] + "" == "") throw new Exception("产品连线生产 连线第一道工序必须 “工序开工处理”");
                }
                for (int s = 1; s < dtRptRecord.Rows.Count; s++)
                {
                    if (dtRptRecord.Rows[s]["t_starttime"] + "" == "")  //未开工，统一开工处理
                    {
                        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_process set t_starttime='" + dtRptRecord.Rows[0]["t_starttime"] + @"' 
                            where t_c_id=0" + dtRptRecord.Rows[s]["process_did"];
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_starttime='" + dtRptRecord.Rows[0]["t_starttime"] + @"' 
                            where t_c_id=0" + dtRptRecord.Rows[s]["t_report_c_id"] + " and t_starttime is null";
                        Cmd.ExecuteNonQuery();
                    }
                }

                //规则：A 首道报告工序的所有报工工序必须进入连线组；
                //      B 末道工序必须为报告工序；
                //      C 连线的工序数必须和流转卡的工序数相等
                //      E 连续制造工段内工序不能重复，即一个工序编码只能出现一次
                //      D 启动连续制造的情况下，只有末道工序才能报工，中间工序只能报废品
                //第一道工序的报告流转卡did
                string b_rpt_baogao_did = GetDataString(@"select t_report_c_id from " + dbname + @"..T_CC_Cards_process where t_c_id=" + dtRptRecord.Rows[0]["process_did"], Cmd);
                //最后一道工序的报告流转卡 did
                string b_rpt_baogao_last_did = GetDataString(@"select t_report_c_id from " + dbname + @"..T_CC_Cards_process where t_c_id=" + dtRptRecord.Rows[dtRptRecord.Rows.Count - 1]["process_did"], Cmd);

                //判定是否缺工序 C ：判定连线的工序数 与  流转卡的工序数 是否相同
                int iseqall_count = GetDataInt(@"select count(*) from " + dbname + @"..T_CC_Cards_process 
                        where t_card_no='" + t_cardno + "' and t_card_seq>='" + dtRptRecord.Rows[0]["t_card_seq"] + "' and t_card_seq<='" + dtRptRecord.Rows[dtRptRecord.Rows.Count - 1]["t_card_seq"] + "'", Cmd);
                if (dtRptRecord.Rows.Count != iseqall_count) throw new Exception("连续制造参数设置错误，工序顺序和工序数量不匹配,工序设备分组号[" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "]");

                //工段内 工序是否重复 E：--whf沃德这边连线生产会产生重复的
//                int iseq_notchongfu_count = int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select count(distinct a.t_opcode)
//                    from " + dbname + "..T_CC_LineEQ_Manufacture a inner join (select * from " + dbname + "..T_CC_Cards_process where t_card_no='" + t_cardno + @"') b 
//                        on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode='" + cc_inv_code + @"'
//                    where a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "'"));
//                if (dtRptRecord.Rows.Count != iseq_notchongfu_count) throw new Exception("连续制造工段内 流转卡[" + t_cardno + "]工序编码不能重复,工序设备分组号[" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "]");

                //B ：连线的末道工序是否为报告工序
                if (GetDataInt(@"select count(*) from " + dbname + @"..T_CC_Cards_report where t_c_id=" + b_rpt_baogao_last_did + @" 
                        and t_card_seq='" + dtRptRecord.Rows[dtRptRecord.Rows.Count - 1]["t_card_seq"] + "'", Cmd) == 0)
                    throw new Exception("连续制造的最后一道工序[" + dtRptRecord.Rows[dtRptRecord.Rows.Count - 1]["t_card_seq"] + "]必须为报告工序");

                //A ：第一道报告工序若有多个报工工序时，判定报工工序是否全部在本连续组中---whf 沃德工序允许重复,无法判断,暂时注释掉
//                int i_shoudao_rpt_count = GetDataInt(@"select COUNT(*) from " + dbname + @"..T_CC_Cards_process where t_report_c_id=0" + b_rpt_baogao_did, Cmd);
//                int i_shoudao_line_count = GetDataInt(@"select COUNT(*) from " + dbname + @"..T_CC_Cards_process a inner join " + dbname + @"..T_CC_LineEQ_Manufacture b 
//                            on a.t_opcode=b.t_opcode and a.t_wc_code=b.t_wccode and b.t_invcode='" + cc_inv_code + @"'
//                        where b.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "' and a.t_report_c_id=0" + b_rpt_baogao_did, Cmd);
//                if (i_shoudao_rpt_count != i_shoudao_line_count) throw new Exception("连续制造首道报告工序[" + dtRptRecord.Rows[0]["t_card_seq"] + "]的所有报工工序没有进入连续制造组[" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "]");

                //排除分拣的自动报工记录，判断是否违反连续报工规则
                if (GetDataInt(@"select COUNT(*) from " + dbname + @"..T_CC_pro_report_list a left join " + dbname + @"..T_CC_pro_report_sortdetail b on a.t_id=b.t_rptid 
                        where a.t_report_c_id=0" + b_rpt_baogao_did + " and b.t_sort_did is null and isnull(a.t_line_m_rpt,'')=''", Cmd) > 0)
                {
                    b_line_report = false;  //虽然设置满足连续制造，但已经有非连续制造的报工记录存在，自动切换到非连续制造模式
                    c_save_batchno = "";

                    throw new Exception(@"'连续制造模式 第一道工序[" + dtRptRecord.Rows[0]["t_card_seq"] + "] 存在非连线报工记录,不能进行连续制造'");//此处可以用参数控制

                }
                else
                {
                    //D ：启动连续制造的情况下，判定是否 末道工序 再报工
                    if (dtRptOpInfo.Rows[0]["t_is_last_op"] + "" != "是")
                    {
                        //合格数为0时，可以报工，否则只能末道工序报工
                        if (fvalid > 0 || fmiss_qty > 0) throw new Exception("连续制造模式下，合格数和差缺数 只有末道工序才能上报工");

                        //非末级情况下，重新构造报工清单
//                        dtRptRecord = GetSqlDataTable(@"select a.t_opcode,a.t_wccode,a.t_eq_no,a.t_is_first_op,a.t_is_last_op,b.t_c_id process_did,b.t_card_seq,b.t_starttime 
//                        from " + dbname + @"..T_CC_LineEQ_Manufacture a inner join (select * from " + dbname + @"..T_CC_Cards_process where t_card_no='" + t_cardno + @"') b 
//                            on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode='" + cc_inv_code + @"'
//                        where  a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "' and b.t_card_seq<='" + c_process_seqno + "' order by a.t_seq_no", "dtRptRecord", Cmd);
                        dtRptRecord = GetSqlDataTable(@"select a.t_opcode,a.t_wccode,a.t_eq_no,a.t_is_first_op,a.t_is_last_op,b.t_c_id process_did,b.t_card_seq,b.t_starttime 
                        from " + dbname + @"..T_CC_LineEQ_Manufacture a inner join (
                            select  row_number() over(partition by t_card_no order by t_c_id) t_seq_no,* from " + dbname + @"..T_CC_Cards_process where t_card_no='" + t_cardno + @"'
                              and t_c_id<=" + c_process_did + @" and t_c_id>" + c_process_did + @"-" + lianNum + @"  ) b 
                            on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode='" + cc_inv_code + @"' and b.t_seq_no=a.t_seq_no
                        where a.t_eqteam_code='" + dtRptOpInfo.Rows[0]["t_eqteam_code"] + "'  and b.t_card_seq<='" + c_process_seqno + "' order by a.t_seq_no", "dtRptRecord", Cmd);
                    }
                }
            }
            else
            {
                //非连线模式：判定当前工序是否存在连线报工记录
                if (GetDataInt(@"select COUNT(*) from " + dbname + @"..T_CC_pro_report_list where t_report_c_id=0" + dtProcessCardInfo.Rows[0]["t_report_c_id"] + " and isnull(t_line_m_rpt,'')<>''", Cmd) > 0)
                    throw new Exception("本工序存在连线报工记录，请扫描连线设备报工");
            }


            if (!b_line_report)  //不能满足连线制造条件，转向非连线制造模式
            {
                dtRptRecord = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select '" + t_opcode + "' t_opcode,'" + t_wccode + "' t_wccode,'" + t_eqno + @"' t_eq_no,
                        '是' t_is_first_op,'是' t_is_last_op,'" + c_process_did + "' process_did,'" + c_process_seqno + "' t_card_seq");
            }
            #endregion

            #region  //报工
            string s_psncode = GetTextsFrom_FormData_Tag(FormData, "txt_t_psn_code");
            for (int i = 0; i < dtRptRecord.Rows.Count; i++)
            {
                string c_proc_did = dtRptRecord.Rows[i]["process_did"] + "";
                FormData = SetTextsFrom_FromData(FormData, "txt_flow_seq", c_proc_did, "" + dtRptRecord.Rows[i]["t_card_seq"]);
                //连续制造末道工序才能考虑 工序送检
                if (dtRptRecord.Rows[i]["t_is_last_op"] + "" == "是")
                {
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_psn_code", "" + s_psncode, "" + s_psncode);
                    //连续制造的末工序  或者  非连续制造模式报工
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_eq_no", t_eqno, t_eqno);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_reason_code", c_reason_code, c_reason_code);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_opcode", "" + dtRptRecord.Rows[i]["t_opcode"], "" + dtRptRecord.Rows[i]["t_opcode"]);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_wc_code", "" + dtRptRecord.Rows[i]["t_wccode"], "" + dtRptRecord.Rows[i]["t_wccode"]);

                    FormData = SetTextsFrom_FromData(FormData, "txt_t_valid_qty", "", "" + fvalid);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_work", "", "" + fscrap_work);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_material", "", "" + fscrap_material);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_qc", "", "" + fscrap_qc);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_miss_qty", "", "" + fmiss_qty);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_repair_qty", "", "" + frepair_qty);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_renew_qty", "", "" + frenew_qty);

                    cRet_Code = Mes_rpt_to_db(FormData, dbname, cUserName, cLogDate, c_save_batchno, SheetID, (t_is_operation_qc == "是" ? true : false), Cmd);
                }
                else
                {
                    //连续制造模式： 非末道工序  （连续制造工段的非末道工序不能送检）
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_eq_no", "" + dtRptRecord.Rows[i]["t_eq_no"], "" + dtRptRecord.Rows[i]["t_eq_no"]);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_reason_code", "", "");
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_opcode", "" + dtRptRecord.Rows[i]["t_opcode"], "" + dtRptRecord.Rows[i]["t_opcode"]);
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_wc_code", "" + dtRptRecord.Rows[i]["t_wccode"], "" + dtRptRecord.Rows[i]["t_wccode"]);
                    // 根据设备的上岗人设置加工人员编码whf
                    string t_psncode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_psncode from "+dbname+"..T_CC_Work_Time where t_eq_no = '" + dtRptRecord.Rows[i]["t_eq_no"] + "' and t_workoff_time is null");
                    FormData = SetTextsFrom_FromData(FormData, "txt_t_psn_code", "" + t_psncode, "" + t_psncode);
                    if (i == dtRptRecord.Rows.Count - 1)
                    {
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_valid_qty", "", "" + fvalid);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_work", "", "" + fscrap_work);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_material", "", "" + fscrap_material);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_qc", "", "" + fscrap_qc);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_miss_qty", "", "" + fmiss_qty);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_repair_qty", "", "" + frepair_qty);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_renew_qty", "", "" + frenew_qty);
                    }
                    else
                    {
                        //非最后一行 报工 ，数量全部虚拟成 合格数
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_valid_qty", "", "" + fcompleteAll);
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_work", "", "0");
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_material", "", "0");
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_scrap_qc", "", "0");
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_miss_qty", "", "0");
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_repair_qty", "", "0");
                        FormData = SetTextsFrom_FromData(FormData, "txt_t_renew_qty", "", "0");
                    }
                    //连线非末道工序不能报检
                    cRet_Code = Mes_rpt_to_db(FormData, dbname, cUserName, cLogDate, c_save_batchno, SheetID, false, Cmd);
                }
            }
            #endregion

            #region  //多缺陷清单 hsValue 值顺序：hsValue[0]工废  hsValue[1]料废  hsValue[2]检废  hsValue[3]返修
            //行格式：0缺陷代号,1数量,2责任工序号,3责任部门号
            if (hsValue != null && fvalid + fscrap_work + fscrap_material + fscrap_qc + frepair_qty > 0 && b_defect_multiInput && t_is_operation_qc != "是")  //存在多缺陷
            {
                string new_rpt_id = cRet_Code.Split(',')[0];//报工单记录ID
                //创建主表信息
                #region
                string cdept_code = GetDataString("select DeptCode from " + dbname + @"..sfc_workcenter where WcCode='" + t_wccode + "'", Cmd);
                Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcsheet(t_rpt_id,t_rpt_code,t_rpt_qty,t_invcode,t_opcode,t_wccode,
                        t_deptcode,t_card_no,t_modid,t_process_id,t_is_qualified,t_memo,t_sj_psn,t_sj_date,t_qc_maker,t_qc_date,t_source,
                        t_valid,t_scrop_mer,t_scrop_work,t_scrop_qc,t_replair) 
                    select t_id,t_code," + (fvalid + fscrap_work + fscrap_material + fscrap_qc + frepair_qty) + ",'" + cc_inv_code + @"',t_opcode,t_wc_code,
                        '" + cdept_code + "','" + c_t_cardno + "'," + dtCardInfo.Rows[0]["t_modid"] + @",t_process_c_id,'是','',t_maker,t_maketime,t_maker,t_maketime,'报工',
                        0" + fvalid + ",0" + fscrap_material + ",0" + fscrap_work + ",0" + fscrap_qc + ",0" + frepair_qty + @"
                    from " + dbname + @"..t_cc_pro_report_list where t_id=" + new_rpt_id;
                Cmd.ExecuteNonQuery();
                string qc_id = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select IDENT_CURRENT( '" + dbname + @"..T_CC_pro_report_qcsheet')");
                string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
                if (qc_id != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");
                Cmd.CommandText = "update " + dbname + @"..T_CC_pro_report_qcsheet set t_qc_code=right('000000000000'+cast(t_qc_id as varchar(12)),12) where t_qc_id=" + qc_id;
                Cmd.ExecuteNonQuery();
                #endregion

                //创建子表明细
                #region
                int i_qces_row = -1;
                if (fvalid > 0)  //合格
                {
                    i_qces_row++;
                    Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcdetail(t_qc_id,t_rpt_id,t_row_no,t_valid,t_scrop_work,t_scrop_mer,t_scrop_qc,
                            t_replair,t_defectcode,t_resp_dept,t_resp_opcode,t_itemname)
                        values(" + qc_id + "," + new_rpt_id + "," + i_qces_row + "," + fvalid + ",0,0,0,0,'','','','合格')";
                    Cmd.ExecuteNonQuery();
                }
                if (fscrap_work > 0)
                {
                    string[] lines = hsValue[0].Split(';');
                    for (int l = 0; l < lines.Length; l++)
                    {
                        string[] coldata = lines[l].Split(',');
                        i_qces_row++;
                        Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcdetail(t_qc_id,t_rpt_id,t_row_no,t_valid,t_scrop_work,t_scrop_mer,t_scrop_qc,
                                t_replair,t_defectcode,t_resp_dept,t_resp_opcode,t_itemname)
                            values(" + qc_id + "," + new_rpt_id + "," + i_qces_row + ",0," + coldata[1] + ",0,0,0,'" + coldata[0] + @"',
                                '" + coldata[3] + "','" + coldata[2] + "','工废')";
                        Cmd.ExecuteNonQuery();
                    }
                }
                if (fscrap_material > 0)
                {
                    string[] lines = hsValue[1].Split(';');
                    for (int l = 0; l < lines.Length; l++)
                    {
                        string[] coldata = lines[l].Split(',');
                        i_qces_row++;
                        Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcdetail(t_qc_id,t_rpt_id,t_row_no,t_valid,t_scrop_work,t_scrop_mer,t_scrop_qc,
                                t_replair,t_defectcode,t_resp_dept,t_resp_opcode,t_itemname)
                            values(" + qc_id + "," + new_rpt_id + "," + i_qces_row + ",0,0," + coldata[1] + ",0,0,'" + coldata[0] + @"',
                                '" + coldata[3] + "','" + coldata[2] + "','料废')";
                        Cmd.ExecuteNonQuery();
                    }
                }
                if (fscrap_qc > 0)
                {
                    string[] lines = hsValue[2].Split(';');
                    for (int l = 0; l < lines.Length; l++)
                    {
                        string[] coldata = lines[l].Split(',');
                        i_qces_row++;
                        Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcdetail(t_qc_id,t_rpt_id,t_row_no,t_valid,t_scrop_work,t_scrop_mer,t_scrop_qc,
                                t_replair,t_defectcode,t_resp_dept,t_resp_opcode,t_itemname)
                            values(" + qc_id + "," + new_rpt_id + "," + i_qces_row + ",0,0,0," + coldata[1] + ",0,'" + coldata[0] + @"',
                                '" + coldata[3] + "','" + coldata[2] + "','检废')";
                        Cmd.ExecuteNonQuery();
                    }
                }
                if (frepair_qty > 0)
                {
                    string[] lines = hsValue[3].Split(';');
                    for (int l = 0; l < lines.Length; l++)
                    {
                        string[] coldata = lines[l].Split(',');
                        i_qces_row++;
                        Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_qcdetail(t_qc_id,t_rpt_id,t_row_no,t_valid,t_scrop_work,t_scrop_mer,t_scrop_qc,
                                t_replair,t_defectcode,t_resp_dept,t_resp_opcode,t_itemname)
                            values(" + qc_id + "," + new_rpt_id + "," + i_qces_row + ",0,0,0,0," + coldata[1] + ",'" + coldata[0] + @"',
                                '" + coldata[3] + "','" + coldata[2] + "','返修')";
                        Cmd.ExecuteNonQuery();
                    }
                }
                #endregion

                //主表汇总

            }
            #endregion
            #region // 判断差缺数是否需要按照比例控制--whf hq
            try
            {
                string mes_flow_missQtyControlInvDef = GetDataString("select  cvalue from " + dbname + "..T_Parameter where cpid='mes_flow_missQtyControlInvDef'", Cmd);
                if (!string.IsNullOrEmpty(mes_flow_missQtyControlInvDef) && fmiss_qty!=0)
                {
                    decimal thisMissQty = Math.Abs(decimal.Parse(fmiss_qty+""));
                    // 获取差缺控制比例
                    string mes_flow_missQtyControlInvBili = GetDataString(@"
select iex." + mes_flow_missQtyControlInvDef + " from " + dbname + @"..Inventory_extradefine iex  
inner join " + dbname + @"..t_cc_card_list cl on cl.t_invcode=iex.cInvCode         
where cl.t_card_no='" + t_cardno + "'", Cmd);
                    if (!string.IsNullOrEmpty(mes_flow_missQtyControlInvBili) && float.Parse(mes_flow_missQtyControlInvBili) > 0)
                    {
                        // 获取可能存在的母卡号
                        string fCardNo = GetDataString(@"
select fcl.t_card_no 
from " + dbname + @"..T_CC_Card_List cl
left join " + dbname + @"..t_cc_card_list fcl on fcl.t_card_id = cl.t_father_card_id
where cl.t_card_no='" + t_cardno + "'", Cmd);
                        if (string.IsNullOrEmpty(fCardNo))
                        {
                            fCardNo = t_cardno;
                        }
                        // 获得母卡差缺数汇总
                        DataTable missQty = GetSqlDataTable(@"
select isnull(sum(cp.t_miss_qty),0) totalMissQty,max(cl.t_card_qty) cardQty from " + dbname + @"..T_CC_Card_List cl
inner join " + dbname + @"..T_CC_Cards_process cp on cp.t_card_no = cl.t_card_no    
where cl.t_card_no='" + fCardNo + "' and cp.t_miss_qty<>0 ", "tb", Cmd);
                        if (missQty.Rows.Count > 0)
                        {
                            decimal totalMissQty = decimal.Parse(missQty.Rows[0][0] + "");
                            decimal cardQty = decimal.Parse(missQty.Rows[0][1] + "");
                            // 取绝对值
                            totalMissQty = Math.Abs(totalMissQty);
                            if (totalMissQty > cardQty * decimal.Parse(mes_flow_missQtyControlInvBili))
                            {
                                decimal ljcqzy = Math.Round((totalMissQty - thisMissQty) / cardQty, 3);
                                decimal bcbgcqbl = Math.Round(thisMissQty / cardQty, 3);
                                decimal yxcq = cardQty * decimal.Parse(mes_flow_missQtyControlInvBili);
                                decimal bckbcq = yxcq - totalMissQty + thisMissQty;
                                throw new Exception("此产品差缺控制比例" + mes_flow_missQtyControlInvBili + "，累计差缺占用" + ljcqzy + "比例，本次可报差缺:" + bckbcq + ",本次报工差缺比例" + bcbgcqbl + "，超过允许差缺，保存失败");
                            }
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                throw new Exception("差缺比例控制:"+ex2.Message);
            }
            #endregion
            Cmd.Transaction.Commit();
            return cRet_Code.Split(',')[0];
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

    /// <summary>
    /// 检查报工时间--whf
    /// 不能早于流转卡制单时间以及上次报工时间
    /// </summary>
    /// <param name="cardNo"></param>
    /// <param name="loginDate"></param>
    private void CheckReportDate(SqlCommand cmd,string dbname,string cardNo,string loginDate)
    {
        string mes_report_isControlReportDate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cValue from " + dbname + "..t_parameter where cpid='mes_report_isControlReportDate'");
        if (mes_report_isControlReportDate != "是")
        {
            return;
        }
        string _loginDate = DateTime.Parse(loginDate).ToString("yyyy-MM-dd");
        int c = UCGridCtl.SqlDBCommon.GetDataIntFromSelect(cmd, @"select count(*) from "+dbname+@"..T_CC_Card_List 
where t_card_no='" + cardNo + @"' and '" + _loginDate + @"'<CONVERT(varchar(10),t_makedate,120)");
        if (c > 0)
        {
            throw new Exception("当前报工时间不能早于流转卡制单时间");
        }
        c = UCGridCtl.SqlDBCommon.GetDataIntFromSelect(cmd, @"select count(*) from " + dbname + @"..T_CC_pro_report_list rpt
inner join " + dbname + @"..T_CC_Cards_process cp on cp.t_c_id = rpt.t_process_c_id
where cp.t_card_no = '" +cardNo+@"' and '"+_loginDate+@"' <CONVERT(varchar(10),rpt.t_maketime,120)");
        if (c > 0)
        {
            string t_maketime = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select CONVERT(varchar(10),max(rpt.t_maketime),120) from " + dbname + @"..T_CC_pro_report_list rpt
inner join " + dbname + @"..T_CC_Cards_process cp on cp.t_c_id = rpt.t_process_c_id
where cp.t_card_no = '" + cardNo + @"' ");
            throw new Exception("当前报工时间不能早于上次报工时间:"+t_maketime);
        }
    }

    [WebMethod]  //质量确认--whf
    public string MES_Defect_Suplement(string rptId, string dbname, string cUserName, string cLogDate, string t_qc_did, string wareCode
        // 是否确认 (是:确认,否:取消确认)
        , string confirm)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        //FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            Cmd.CommandText = @"use ["+dbname +"]";
            Cmd.ExecuteNonQuery();
            if (GetDataString("select cValue from T_Parameter where cPid = 'mes_qc_QualityIsOpen'", Cmd) != "是")
            {
                throw new Exception("当前未启用质量确认功能");
            }
            string mes_qc_QualityIsOpenInvDef = GetDataString("select cValue from T_Parameter where cPid = 'mes_qc_QualityIsOpenInvDef'", Cmd);
            if (string.IsNullOrEmpty(mes_qc_QualityIsOpenInvDef))
            {
                throw new Exception("质量确认参数存货自定义项未设置");
            }
            // 如果确认人和责任人的部门不一致不允许确认或取消确认
            DataTable qcDeptDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"
select dept.cDepCode,dept.cDepName from hr_hi_person hr 
inner join Department dept on dept.cDepCode = hr.cDept_num
inner join T_CC_pro_report_qcdetail qcd on qcd.t_resp_dept = hr.cPsn_Num
where qcd.t_qc_did = 0"+t_qc_did);
            DataTable userDeptDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"
select dept.cDepCode,dept.cDepName from UFSystem..UA_User u
inner join UserHrPersonContro uhr on uhr.cUser_Id = u.cUser_Id
inner join hr_hi_person hr on hr.cPsn_Num = uhr.cPsn_Num
inner join Department dept on dept.cDepCode = hr.cDept_num
where u.cUser_Id = '" + cUserName+"'");
            if (userDeptDt.Rows.Count <= 0)
            {
                throw new Exception("当前登录人不存在于人员档案中");
            }
            if (qcDeptDt.Rows.Count > 0 && userDeptDt.Rows.Count>0)
            {
                if (qcDeptDt.Rows[0][0] + "" != userDeptDt.Rows[0][0] + "")
                {
                    throw new Exception("责任对象部门:" + qcDeptDt.Rows[0][1] + ",与当前登录人部门:" + userDeptDt.Rows[0][1]+",不一致;不允许确认或取消确认");
                }
            }
            if (GetDataString(@"
select iex." + mes_qc_QualityIsOpenInvDef + @" from T_CC_pro_report_list rpt
inner join T_CC_Cards_process cardProcess on cardProcess.t_c_id = rpt.t_process_c_id  
inner join T_CC_Card_List card on cardProcess.t_card_no = card.t_card_no    
inner join mom_orderdetail md  on card.t_modid = md.MoDId
inner join inventory i on i.cInvCode = md.InvCode
inner join Inventory_extradefine iex on iex.cInvCode = i.cInvCode
where rpt.t_id = 0" + rptId, Cmd)!="是")
            {
                throw new Exception("当前存货不参加质量确认");
            }
            if (GetDataInt("select count(*) from t_cqcc_Rd10_FlowCard_rpt  where t_rpt_id =0" + rptId, Cmd) > 0)
            {
                throw new Exception("当前报工单已有入库记录,不能确认或取消确认");
            }
            if (GetDataInt("select count(*) from t_cqcc_Rd10_FlowCard_qcd  where t_qc_did =0" + t_qc_did, Cmd) > 0)
            {
                throw new Exception("当前缺陷明细已有入库记录,不能确认或取消确认");
            }
            string cVerifier = GetDataString("select isnull(cVerifier,'') from T_CC_pro_report_qcdetail where t_qc_did = 0" + t_qc_did, Cmd);
            if (confirm == "是")
            {
                if (!string.IsNullOrEmpty(cVerifier))
                {
                    throw new Exception("当前记录已经确认,不能重复确认");
                }
                // 设置质量确认人和质量确认时间
                Cmd.CommandText = @"update T_CC_pro_report_qcdetail set  cVerifier='" + cUserName + "' ,dverifysystime='" + cLogDate + "' ,cWhCode='" + wareCode + "' where t_qc_did=0" + t_qc_did;
                Cmd.ExecuteNonQuery();
            }
            if (confirm == "否"  )
            {
                if(cUserName!=cVerifier)
                {
                    throw new Exception("当前记录已经确认,只有确认人" + cVerifier + "能够取消确认");
                }
                // 设置质量确认人和质量确认时间
                Cmd.CommandText = @"update T_CC_pro_report_qcdetail set  cVerifier=null,dverifysystime=null ,cWhCode=null where t_qc_did=0" + t_qc_did;
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();
            return "质量确认完成";
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

    private bool LianMate(SqlCommand Cmd, string dbname, string cc_inv_code, string t_wccode, string t_opcode, string t_eqno, string t_cardno, string c_process_did
        , ref DataTable dtRptOpInfo
        , ref DataTable dtRptRecord)
    {
        DataTable processDt = GetSqlDataTable("select * from " + dbname + @"..T_CC_Cards_process where t_card_no='" + t_cardno + "' order by  t_card_seq ", "tb", Cmd);
        string rptSql = @"select t_eqteam_code,t_is_first_op,t_is_last_op from " + dbname + @"..T_CC_LineEQ_Manufacture 
                    where t_invcode='" + cc_inv_code + "' and t_wccode='" + t_wccode + "' and t_opcode='" + t_opcode + "' and t_eq_no='" + t_eqno + "'  order by t_seq_no";
        dtRptOpInfo = GetSqlDataTable(rptSql, "dtRptOpInfo", Cmd);
        dtRptRecord = GetSqlDataTable(@"select a.t_opcode,a.t_wccode,a.t_eq_no,a.t_is_first_op,a.t_is_last_op,b.t_c_id process_did,b.t_card_seq,b.t_starttime,b.t_report_c_id 
        from " + dbname + @"..T_CC_LineEQ_Manufacture a inner join (select * from " + dbname + @"..T_CC_Cards_process where t_card_no='') b 
            on a.t_opcode=b.t_opcode and a.t_wccode=b.t_wc_code and a.t_invcode=''
        where 1=2 order by a.t_seq_no", "dtRptRecord", Cmd);
        foreach (DataRow dtRptOpInfoRow in dtRptOpInfo.Rows)
        {
            DataTable LineEQDt = GetSqlDataTable("select * from " + dbname + @"..T_CC_LineEQ_Manufacture where t_eqteam_code='" + dtRptOpInfoRow["t_eqteam_code"] + "' order by t_seq_no ", "tb", Cmd);
            int len = LineEQDt.Rows.Count;
            int j = 0;
            for (int i = 0; i < processDt.Rows.Count; i++)
            {
                if (LineEQDt.Rows[j]["t_opcode"] + "" == processDt.Rows[i]["t_opcode"]+"" && LineEQDt.Rows[j]["t_wccode"] + "" == processDt.Rows[i]["t_wc_code"]+"")
                {
                    DataRow row = dtRptRecord.Rows.Add();
                    row["t_opcode"] = LineEQDt.Rows[j]["t_opcode"];
                    row["t_wccode"] = LineEQDt.Rows[j]["t_wccode"];
                    row["t_is_first_op"] = LineEQDt.Rows[j]["t_is_first_op"];
                    row["t_is_last_op"] = LineEQDt.Rows[j]["t_is_last_op"];
                    row["process_did"] = processDt.Rows[i]["t_c_id"];
                    row["t_card_seq"] = processDt.Rows[i]["t_card_seq"];
                    row["t_starttime"] = processDt.Rows[i]["t_starttime"];
                    row["t_report_c_id"] = processDt.Rows[i]["t_report_c_id"];
                    j++;
                    if (j == len)
                    {
                        dtRptOpInfo.Rows.Clear();
                        dtRptOpInfo.Rows.Add(dtRptOpInfoRow);
                        //匹配成功
                        return true;
                    }
                }
                else
                {
                    j = 0;
                    dtRptRecord.Rows.Clear();
                }
            }
        }
        return false;

    }

    private string Mes_rpt_to_db(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string csave_bat_no, string SheetID, bool bChecked, SqlCommand Cmd)
    {
        //判断是否质检，本工序
        bool bOp_checked = false;
        if (bChecked) bOp_checked = true;  //是否送检

        string c_BaoGong_did = GetTextsFrom_FormData_Tag(FormData, "txt_flow_seq");  //报工流转卡明细ID
        string c_BaoGao_did = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_report_c_id from " + dbname + @"..T_CC_Cards_process where t_c_id=" + c_BaoGong_did);//报告流转卡明细ID
        string c_father_BaoGong_did = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_father_process_c_id from " + dbname + @"..T_CC_Cards_process where t_c_id=" + c_BaoGong_did);//母卡报工明细ID
        string c_father_BaoGao_did = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_father_report_c_id from " + dbname + @"..T_CC_Cards_process where t_c_id=" + c_BaoGong_did);//母卡报告明细ID

        string[] t_data = GetTextsFrom_FormData(FormData, "txt_t_psn_code");
        string t_psn_code = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_psn_code = t_data[2];//GetTextsFrom_FormData_Tag(FormData, "txt_t_psn_code");
        }

        t_data = GetTextsFrom_FormData(FormData, "txt_t_eq_no");
        string t_eq_no = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_eq_no = t_data[2];// GetTextsFrom_FormData_Tag(FormData, "txt_t_eq_no");
        }

        t_data = GetTextsFrom_FormData(FormData, "txt_t_ven_code");
        string t_ven_code = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_ven_code = t_data[2];//GetTextsFrom_FormData_Tag(FormData, "txt_t_ven_code");
        }

        t_data = GetTextsFrom_FormData(FormData, "txt_t_reason_code");
        string t_reason_code = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_reason_code = t_data[2];//GetTextsFrom_FormData_Tag(FormData, "txt_t_reason_code");
        }

        t_data = GetTextsFrom_FormData(FormData, "txt_t_opcode");
        string t_the_opcode = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_the_opcode = t_data[2];//GetTextsFrom_FormData_Tag(FormData, "txt_t_opcode");
        }

        t_data = GetTextsFrom_FormData(FormData, "txt_t_wc_code");
        string t_the_wccode = "";
        if (t_data != null)
        {
            if ((t_data[2] == "" && t_data[3] != "") || (t_data[2] != "" && t_data[3] == "")) throw new Exception(t_data[0] + " 输入不正确，请输入此栏目后按回车");
            t_the_wccode = t_data[2];//GetTextsFrom_FormData_Tag(FormData, "txt_t_wc_code");
        }

        string t_the_opname = GetDataString("select [Description] FROM " + dbname + "..sfc_operation where OpCode='" + t_the_opcode + "'", Cmd);

        #region  //工序参数管控判定
        //判断是否 所有工序需要开工管理( 工序开工总开关)
        bool b_All_OP_Must_starttime = false;
        string cpp_pvalue = GetDataString("select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_starttime_opcontrol'", Cmd);
        if (cpp_pvalue.Trim().ToLower().CompareTo("true") == 0) b_All_OP_Must_starttime = true;
        string c_start_time = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_starttime from " + dbname + @"..T_CC_Cards_process where t_c_id=" + c_BaoGong_did);
        if (b_All_OP_Must_starttime && c_start_time.CompareTo("") == 0) throw new Exception("所有工序必须先开工 再 报工");

        DataTable dtOpSetting = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select * from " + dbname + @"..T_CC_ProCtl_Setting a 
            where t_opcode='" + t_the_opcode + "' and t_wccode='" + t_the_wccode + "'");
        if (dtOpSetting.Rows.Count > 0)
        {
            if (b_All_OP_Must_starttime == false && dtOpSetting.Rows[0]["t_is_starttime"] + "" == "是" && c_start_time.CompareTo("") == 0)  //需要采集开工时间
            {
                throw new Exception("请先进行工序 [" + t_the_opname + "] 开工处理");
            }

            if (dtOpSetting.Rows[0]["t_is_eqno"] + "" == "是")  //需要采集设备号
            {
                if (t_eq_no.CompareTo("") == 0) throw new Exception("请输入或扫描设备号");

                if (dtOpSetting.Rows[0]["t_is_workon"] + "" == "是")  //需要采集上岗记录
                {
                    if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select count(*) from " + dbname + @"..T_CC_Work_Time where t_eq_no='" + t_eq_no + "' and t_workoff_time is null")) == 0)
                        throw new Exception("当前工序 [" + t_the_opname + "] 无有效员工上岗记录");
                }
            }
        }
        #endregion

        #region //保存 报工记录
        string c_sql_1 = @"insert into " + dbname + @"..T_CC_pro_report_list(t_opcode,t_wc_code,t_psn_code,t_eq_no,t_ven_code,t_reason_code,t_process_c_id,t_report_c_id,t_father_process_c_id,t_father_report_c_id,
                    t_valid_qty,t_scrap_work,t_scrap_material,t_scrap_qc,t_miss_qty,t_repair_qty,t_renew_qty,t_maker,t_maketime,t_is_qc,uptime,t_line_m_rpt";
        string c_sql_2 = ") values('" + t_the_opcode + "','" + t_the_wccode + "','" +
            t_psn_code + "','" + t_eq_no + "','" + t_ven_code + "','" + t_reason_code + "',0" +
            c_BaoGong_did + "," + c_BaoGao_did + ",0" + c_father_BaoGong_did + ",0" + c_father_BaoGao_did +
            ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_valid_qty") + ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_work") +
            ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_material") + ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_scrap_qc") +
            ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_miss_qty") + ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_repair_qty") +
            //",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_renew_qty") + ",'" + cUserName + "',convert(varchar(10),getdate(),120),'" + (bOp_checked ? "是" : "否") + "',convert(varchar(20),getdate(),120),'" + csave_bat_no + "'";
            ",0" + GetTextsFrom_FormData_Text(FormData, "txt_t_renew_qty") + ",'" + cUserName + "','"+cLogDate+"','" + (bOp_checked ? "是" : "否") + "',convert(varchar(20),getdate(),120),'" + csave_bat_no + "'";

        for (int i = 0; i < FormData.Rows.Count; i++)
        {
            //显示标题   txt_字段名称    文本Tag     文本Text值
            string cDefineName = FormData.Rows[i]["TxtName"] + "";
            if (cDefineName.IndexOf("txt_define") > -1)
            {
                c_sql_1 = c_sql_1 + "," + cDefineName.Substring(4);
                if (cDefineName.CompareTo("txt_define14") > 0 && cDefineName.CompareTo("txt_define19") < 0)
                {
                    //数字型
                    c_sql_2 = c_sql_2 + ",0" + FormData.Rows[i]["TxtValue"];
                }
                else
                {
                    //字符型   日期
                    c_sql_2 = c_sql_2 + ",'" + FormData.Rows[i]["TxtValue"] + "'";
                }
            }
        }

        Cmd.CommandText = c_sql_1 + c_sql_2 + ")";
        Cmd.ExecuteNonQuery();

        //获得当前插入行ID
        string c_t_id = "" + GetDataString("select IDENT_CURRENT( '" + dbname + @"..T_CC_pro_report_list' )", Cmd);
        string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
        if (c_t_id != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

        //判断是否 空报工
        if (GetDataInt("select count(*) from " + dbname + @"..T_CC_pro_report_list where t_id=" + c_t_id +
            " and (t_valid_qty=0 and t_scrap_work=0 and t_scrap_material=0 and t_scrap_qc=0 and t_miss_qty=0 and t_repair_qty=0 and t_renew_qty=0)", Cmd) > 0)
        {
            throw new Exception("报工数  不能全部为0");
        }
        //判断差缺是否可以未负数
        string miss_checksql = "";
        if (GetDataString("select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_miss_qty_low_zero'", Cmd).ToLower().CompareTo("true") != 0)
            miss_checksql = "or t_miss_qty<0";

        GetDataString("select '差缺数问题查看' aa,t_valid_qty,t_scrap_work,t_scrap_material,t_scrap_qc,t_miss_qty,t_repair_qty,t_renew_qty from " + dbname + @"..T_CC_pro_report_list where t_id=" + c_t_id + " ", Cmd);
        if (GetDataInt("select count(*) from " + dbname + @"..T_CC_pro_report_list where t_id=" + c_t_id +
            " and (t_valid_qty<0 or t_scrap_work<0 or t_scrap_material<0 or t_scrap_qc<0 " + miss_checksql + " or t_repair_qty<0 or t_renew_qty<0)", Cmd) > 0)
        {
            throw new Exception("报工数必须大于0");
        }

        //本工序报工总量不能小于0
        if (decimal.Parse(GetDataString("select isnull(sum(t_valid_qty+t_scrap_work+t_scrap_material+t_scrap_qc-t_miss_qty+t_repair_qty+t_renew_qty),0) from " + dbname + @"..T_CC_pro_report_list 
            where t_process_c_id=0" + c_BaoGong_did, Cmd)) <= 0)
        {
            throw new Exception("本工序报工总量必须大于0,请确认您的差缺数是否录入正确.");
        }
        // zm要求工序总量去掉差缺数
        //        if (decimal.Parse(GetDataString("select isnull(sum(t_valid_qty+t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty),0) from " + dbname + @"..T_CC_pro_report_list 
//            where t_process_c_id=0" + c_BaoGong_did, Cmd)) <= 0)
//        {
//            throw new Exception("本工序报工总量必须大于0,请确认您的差缺数是否录入正确.");
//        }
        #endregion

        #region   //多人员操作
        if (t_eq_no.CompareTo("") != 0)
        {
            Cmd.CommandText = "delete from " + dbname + @"..t_cqcc_CardReportEqWorkOn where rptId = " + c_t_id;
            Cmd.ExecuteNonQuery();
            string cinvcode = GetDataString("select b.t_invcode from "+dbname+"..T_CC_Cards_process a inner join "+dbname+"..T_CC_Card_List b on a.t_card_no=b.t_card_no where t_c_id=0"+c_BaoGong_did, Cmd);
            DataTable dtEq_psn = GetSqlDataTable("select personCode from " + dbname + @"..t_cqcc_MultiInvWork where cInvCode='" + cinvcode + "' and eqCode='" + t_eq_no + "'","tb", Cmd);
            //获得资源清单
            if (dtEq_psn.Rows.Count == 0)
            {
                dtEq_psn = GetSqlDataTable("select t_psncode from " + dbname + @"..T_CC_Work_Time a where t_eq_no='" + t_eq_no + "' and t_workoff_time is null", "dtEq_psn", Cmd);
            }
            for (int p = 0; p < dtEq_psn.Rows.Count; p++)
            {
                Cmd.CommandText = "insert into " + dbname + @"..t_cqcc_CardReportEqWorkOn (rptId, personCode, personCount) values (" + c_t_id + ",'" + dtEq_psn.Rows[p][0] + "'," + dtEq_psn.Rows.Count + ")";
                Cmd.ExecuteNonQuery();
            }
        }// TODO
        //设备人员为空时，按照设备上岗人员来填充
        if (t_psn_code.CompareTo("") == 0 && t_eq_no.CompareTo("") != 0)
        {
            //获得参数 存放多人的 自定义项栏目清单
            string cPsn_defineCol = "" + GetDataString("select cValue from " + dbname + @"..T_Parameter where cPid='mes_workno_eq_report_psn'", Cmd);
            if (cPsn_defineCol.CompareTo("") == 0)
                cPsn_defineCol = "t_psn_code";
            else
                cPsn_defineCol = "t_psn_code," + cPsn_defineCol;
            string[] psn_cols = cPsn_defineCol.Split(',');  //可存放人员栏目清单
            //获得资源清单
            DataTable dtEq_psn = GetSqlDataTable("select t_psncode from " + dbname + @"..T_CC_Work_Time a where t_eq_no='" + t_eq_no + "' and t_workoff_time is null", "dtEq_psn", Cmd);
            for (int p = 0; p < dtEq_psn.Rows.Count; p++)
            {
                if (p < psn_cols.Length)
                {
                    Cmd.CommandText = "update " + dbname + @"..T_CC_pro_report_list set " + psn_cols[p] + "='" + dtEq_psn.Rows[p]["t_psncode"] + @"' 
                        where t_id=0" + c_t_id;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    break;
                }
            }
            
        }
        #endregion

        #region  //报告工序设置工序仓库，判断是否自动出库
        string rpt_is_ware = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select t_op_warehouse from " + dbname + @"..T_CC_Cards_report where t_c_id='" + c_BaoGao_did + "'");
        if (rpt_is_ware == "是")
        {
            string c_ware_auto_out = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_op_ware_auto_out_control'");
            if ((dtOpSetting.Rows.Count > 0 && dtOpSetting.Rows[0]["t_is_WareOut_Auto"] + "" == "是") || c_ware_auto_out.ToLower().CompareTo("true") == 0)
            {
                //判断本报告工序 是否只有一道报工工序，若是，则可以自动发料，若否则不行
                if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select COUNT(*) from " + dbname + @"..T_CC_Cards_process where t_report_c_id=" + c_BaoGao_did)) == 1)
                {
                    float f_bg_all_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select t_valid_qty+t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty 
                            from " + dbname + @"..T_CC_pro_report_list where t_id=" + c_t_id));
                    U8MES.MES.FlowOpSendOut.SaveOpSendBuss(Cmd, f_bg_all_qty, c_BaoGao_did, c_t_id, "0", "0", cUserName, dbname); //自动出库
                }
            }
        }
        #endregion

        if (bOp_checked)  //不质检的话，回写流转卡相关数据  
        {
            //写质检在制数（报告工序才能送检）
            WriteFlowQC_Data(Cmd, dbname, c_BaoGong_did, c_t_id);
        }
        else
        {
            WriteFlowData(Cmd, dbname, c_BaoGong_did);
        }

        #region   //保存后续工作：单据号，完工时间
        //获得单据号
        //string c_mocode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select right('000000000000'+cast(cast(isnull(max(t_code),'0') as int)+1 as varchar(12)),12) from T_CC_pro_report_list");
        string c_mocode = GetDataString("select right('000000000000'+cast(" + c_t_id + " as varchar(12)),12)", Cmd);
        Cmd.CommandText = "update " + dbname + @"..T_CC_pro_report_list set t_code='" + c_mocode + "' where t_id=" + c_t_id;
        Cmd.ExecuteNonQuery();

        //回写流转子卡 的完工时间
        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_process set t_endtime=getdate() where t_c_id=" + c_BaoGong_did;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_endtime=getdate() where t_c_id=" + c_BaoGao_did;
        Cmd.ExecuteNonQuery();
        #endregion

        #region  //获得流转卡补充信息

        string c_pro_batch = GetTextsFrom_FormData_Tag(FormData, "txt_t_pro_cbatch_no");
        if (c_pro_batch == "") c_pro_batch = GetTextsFrom_FormData_Text(FormData, "txt_t_pro_cbatch_no");
        string c_mer_batch = GetTextsFrom_FormData_Tag(FormData, "txt_t_cbatch_mian_mer");
        if (c_mer_batch == "") c_mer_batch = GetTextsFrom_FormData_Text(FormData, "txt_t_cbatch_mian_mer");

        if (c_mer_batch != "" || c_pro_batch != "")
        {
            string c_txt_card_no = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_card_no from " + dbname + @"..T_CC_Cards_process where t_c_id=" + c_BaoGong_did);
            if (c_pro_batch != "")
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_Card_List set t_pro_cbatch_no='" + c_pro_batch + "' where t_card_no='" + c_txt_card_no + "'";
                Cmd.ExecuteNonQuery();
            }

            if (c_mer_batch != "")
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_Card_List set t_cbatch_mian_mer='" + c_mer_batch + "' where t_card_no='" + c_txt_card_no + "'";
                Cmd.ExecuteNonQuery();
            }
        }
        #endregion

        return c_t_id + "," + c_mocode;
    }

    //回写流转卡 质检在制数
    private static void WriteFlowQC_Data(SqlCommand cmd, string dbname, string i_process_c_id, string rpt_data_id)
    {
        //报告工序才能送检
        DataTable dtProcess = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select t_c_id t_process_c_id,t_report_c_id,t_father_process_c_id,t_father_report_c_id,t_card_no,t_opcode 
            from " + dbname + @"..T_CC_Cards_process where t_c_id=0" + i_process_c_id);
        if (dtProcess.Rows.Count == 0) throw new Exception("送检：未找到送检的流转卡工序行数据");

        string cValidQty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(min(t_valid_qty),0) from " + dbname + @"..T_CC_pro_report_list where t_id=0" + rpt_data_id);
        //修改子卡的 在制质检数
        cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_working_qc_qty=isnull(t_working_qc_qty,0)+(0" + cValidQty + "),t_working_qty=t_working_qty-(0" + cValidQty + @")
            where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"];
        cmd.ExecuteNonQuery();

        //修改主卡的在制质检数
        cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_working_qc_qty=isnull(t_working_qc_qty,0)+(0" + cValidQty + "),t_working_qty=t_working_qty-(0" + cValidQty + @")
            where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"];
        cmd.ExecuteNonQuery();

        //判断在制数是否为负
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(cmd, "select count(*) from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"] + " and t_working_qty<0") > 0)
            throw new Exception("送检后，在制数为负");
    }

    //回写流转卡 相关数据   传递报工流转卡的明细ID
    // 添加多段拆卡控制--whf
    private static void WriteFlowData(SqlCommand cmd, string dbname, string i_process_c_id)
    {
        string t_working_qty = "";
        //t_valid_qty,t_scrap_work,t_scrap_material,t_scrap_qc,t_miss_qty,t_repair_qty,t_renew_qty
        DataTable dtProcess = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select t_c_id t_process_c_id,t_report_c_id,t_father_process_c_id,t_father_report_c_id,t_card_no,t_opcode 
            from " + dbname + @"..T_CC_Cards_process where t_c_id=" + i_process_c_id);
        if (dtProcess.Rows.Count == 0) return;

        #region   //报工子卡  数据回写
        //报工流转卡  取 待检状态为 否 的报工数
        DataTable dtData = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select sum(t_valid_qty)t_valid_qty,sum(t_scrap_work)t_scrap_work,sum(t_scrap_material)t_scrap_material,
                sum(t_scrap_qc)t_scrap_qc,sum(t_miss_qty)t_miss_qty,sum(t_repair_qty)t_repair_qty,sum(t_renew_qty)t_renew_qty from " + dbname + @"..T_CC_pro_report_list 
                where t_process_c_id=0" + dtProcess.Rows[0]["t_process_c_id"] + " and t_is_qc='否'");
        if (dtData.Rows.Count > 0)
        {
            cmd.CommandText = "update " + dbname + @"..T_CC_Cards_process set t_valid_qty=0" + dtData.Rows[0]["t_valid_qty"] + ",t_scrap_work=0" + dtData.Rows[0]["t_scrap_work"] +
                ",t_scrap_material=0" + dtData.Rows[0]["t_scrap_material"] + ",t_scrap_qc=0" + dtData.Rows[0]["t_scrap_qc"] + ",t_miss_qty=0" + dtData.Rows[0]["t_miss_qty"] +
                ",t_repair_qty=0" + dtData.Rows[0]["t_repair_qty"] + ",t_renew_qty=0" + dtData.Rows[0]["t_renew_qty"] +
                " where t_c_id=0" + dtProcess.Rows[0]["t_process_c_id"];
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region   //报工母卡(拆卡前的母卡)  数据回写
        //报工母卡
        if (int.Parse("" + dtProcess.Rows[0]["t_father_process_c_id"]) > 0)   //母卡 报告明细ID 为0时，无需计算 母卡的总报工数
        {
            dtData = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select sum(t_valid_qty)t_valid_qty,sum(t_scrap_work)t_scrap_work,sum(t_scrap_material)t_scrap_material,
                sum(t_scrap_qc)t_scrap_qc,sum(t_miss_qty)t_miss_qty,sum(t_repair_qty)t_repair_qty,sum(t_renew_qty)t_renew_qty from " + dbname + @"..T_CC_pro_report_list 
                where t_father_process_c_id=0" + dtProcess.Rows[0]["t_father_process_c_id"]);
            if (dtData.Rows.Count > 0)
            {
                cmd.CommandText = "update " + dbname + @"..T_CC_Cards_process set t_valid_qty=0" + dtData.Rows[0]["t_valid_qty"] + ",t_scrap_work=0" + dtData.Rows[0]["t_scrap_work"] +
                    ",t_scrap_material=0" + dtData.Rows[0]["t_scrap_material"] + ",t_scrap_qc=0" + dtData.Rows[0]["t_scrap_qc"] + ",t_miss_qty=0" + dtData.Rows[0]["t_miss_qty"] +
                    ",t_repair_qty=0" + dtData.Rows[0]["t_repair_qty"] + ",t_renew_qty=0" + dtData.Rows[0]["t_renew_qty"] +
                    " where t_c_id=0" + dtProcess.Rows[0]["t_father_process_c_id"];
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        string c_card_type = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_card_type from " + dbname + @"..T_CC_Card_List where t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'");
        string t_card_id = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_card_id from " + dbname + @"..T_CC_Card_List where t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'");
        #region   //报告子卡  数据回写
        //报告流转卡 将报工流转卡的 各项数据 按照报告卡DID汇总
        if (c_card_type == "非标返修卡")
        {
            //只处理 各工序的非合格数 与 首尾两道工序的合格数
            dtData = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select  sum(t_valid_qty) t_valid_qty,sum(t_scrap_work) t_scrap_work,sum(t_scrap_material) t_scrap_material,
                        sum(t_scrap_qc) t_scrap_qc,sum(t_miss_qty) t_miss_qty,sum(t_repair_qty) t_repair_qty,sum(t_renew_qty) t_renew_qty
                    from (
                        select 0 t_valid_qty,sum(t_scrap_work) t_scrap_work,sum(t_scrap_material) t_scrap_material,
                                            sum(t_scrap_qc) t_scrap_qc,sum(t_miss_qty) t_miss_qty,sum(t_repair_qty) t_repair_qty,sum(t_renew_qty) t_renew_qty from " + dbname + @"..T_CC_Cards_process 
                                            where t_report_c_id=0" + dtProcess.Rows[0]["t_report_c_id"] + @"
                        union all
                        select isnull(min(t_valid_qty),0) t_valid_qty,0 t_scrap_work,0 t_scrap_material,0 t_scrap_qc,0 t_miss_qty,0 t_repair_qty,0 t_renew_qty from " + dbname + @"..T_CC_Cards_process 
                        where t_report_c_id=0" + dtProcess.Rows[0]["t_report_c_id"] + @" and t_opcode in('FB01','FB02')
                    ) t ");
        }
        else
        {
            dtData = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select isnull(min(t_valid_qty),0) t_valid_qty,sum(t_scrap_work)t_scrap_work,sum(t_scrap_material)t_scrap_material,
                sum(t_scrap_qc)t_scrap_qc,sum(t_miss_qty)t_miss_qty,sum(t_repair_qty)t_repair_qty,sum(t_renew_qty)t_renew_qty from " + dbname + @"..T_CC_Cards_process 
                where t_report_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
        }
        if (dtData.Rows.Count > 0)
        {
            cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_valid_qty=0" + dtData.Rows[0]["t_valid_qty"] + ",t_scrap_work=0" + dtData.Rows[0]["t_scrap_work"] +
                ",t_scrap_material=0" + dtData.Rows[0]["t_scrap_material"] + ",t_scrap_qc=0" + dtData.Rows[0]["t_scrap_qc"] + ",t_miss_qty=0" + dtData.Rows[0]["t_miss_qty"] +
                ",t_repair_qty=0" + dtData.Rows[0]["t_repair_qty"] + ",t_renew_qty=0" + dtData.Rows[0]["t_renew_qty"] +
                " where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"];
            cmd.ExecuteNonQuery();
            //计算本工序在制数     ---------------------------------------------------------
            cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_working_qty=t_tran_in_qty-t_working_ven_qty-isnull(t_working_ware_qty,0)-t_valid_qty-t_scrap_work-t_scrap_material-t_scrap_qc-t_miss_qty-t_repair_qty-t_renew_qty   -isnull(t_working_qc_qty,0) 
                 where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"];
            cmd.ExecuteNonQuery();
            t_working_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_working_qty from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd,"select '在制数打印1:" + t_working_qty + "'");
            //写下道工序的 合格转入数 和 厂内在制数
            DataTable dtBaoGaoNextSeq = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, "select t_c_id t_next_cid,t_opcode,t_wc_code,t_op_warehouse from " + dbname + @"..T_CC_Cards_report 
                where t_up_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
            if (dtBaoGaoNextSeq.Rows.Count > 0)
            {
                //判断下道工序是否要求工序仓库
                if ((dtBaoGaoNextSeq.Rows[0]["t_op_warehouse"] + "").CompareTo("是") == 0)
                {
                    //下工序总接收数
                    string cNext_reveive_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(isnull(t_rd_in_qty,0)),0) from " + dbname + @"..T_CC_op_rd_records 
                            where t_rpt_c_id=0" + dtBaoGaoNextSeq.Rows[0]["t_next_cid"]);
                    //本工序的 【待接收量】 = 【本工序所有合格数】-【下工序所有接收数】
                    cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_worked_qty=(0" + dtData.Rows[0]["t_valid_qty"] + ")-(0" + cNext_reveive_qty + ") where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"];
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    //【上工序所有合格数 减去 本工序的 外厂在制数-各种本工序的合格数和废品数、查缺数】=本工序在制待工数 --------------------------------------------------
                    cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_tran_in_qty=0" + dtData.Rows[0]["t_valid_qty"] +
                            ",t_working_qty=(0" + dtData.Rows[0]["t_valid_qty"] + @")-t_working_ven_qty-t_valid_qty-t_scrap_work-t_scrap_material-t_scrap_qc-t_miss_qty-t_repair_qty-t_renew_qty   -isnull(t_working_qc_qty,0) 
                            where t_c_id=0" + dtBaoGaoNextSeq.Rows[0]["t_next_cid"];
                    cmd.ExecuteNonQuery();
                    t_working_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_working_qty from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtBaoGaoNextSeq.Rows[0]["t_next_cid"]);
                    UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd,"select '在制数打印2:" + t_working_qty + "'");
                    //获得下工序拆分的流转卡总数 *****************
                    string f_chai_all = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(SUM(b.t_new_card_qty),0) 
                            from " + dbname + @"..T_CC_FlowCard_Sp_Main a inner join " + dbname + @"..T_CC_FlowCard_Split b on a.t_card_id=b.t_oldcard_id
                            inner join " + dbname + @"..T_CC_Cards_report c on a.t_card_no=c.t_card_no and a.t_seqno=c.t_card_seq
                            where c.t_c_id=0" + dtBaoGaoNextSeq.Rows[0]["t_next_cid"]);
                    //下工序未设置工序仓：本工序的合格数总量 - 拆分量必须大于0
                    if (float.Parse(dtData.Rows[0]["t_valid_qty"] + "") < float.Parse(f_chai_all))
                        throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]工序[" + dtBaoGaoNextSeq.Rows[0]["t_opcode"] + "]已经拆分流转卡数量：" + f_chai_all + ",报工合格量不能小于" + f_chai_all);
                }

                //判断下道工序是否超数量  需要减去拆分的流转卡数量
                if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select count(*) from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtBaoGaoNextSeq.Rows[0]["t_next_cid"] + " and t_working_qty<0")) > 0)
                    throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]工序[" + dtBaoGaoNextSeq.Rows[0]["t_opcode"] + "]在制量为负");

            }
            else  //末道工序，回写流转卡的 完工数
            {
                cmd.CommandText = "update " + dbname + @"..T_CC_Card_List set t_card_overqty=0" + dtData.Rows[0]["t_valid_qty"] + " where t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'";
                cmd.ExecuteNonQuery();

                #region  //超入库报工检查
                //获得报工数据
                DataTable dtRpt_data = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select isnull(SUM(case when a.t_ven_code='' then a.t_scrap_material+a.t_scrap_work+a.t_scrap_qc else 0 end),0) feip_n,
                        isnull(SUM(case when a.t_ven_code<>'' then a.t_scrap_material+a.t_scrap_work+a.t_scrap_qc else 0 end),0) feip_w,
                        isnull(SUM(a.t_repair_qty),0) t_repair_qty,isnull(SUM(a.t_renew_qty),0) t_renew_qty
                    from " + dbname + @"..T_CC_pro_report_list a inner join " + dbname + @"..T_CC_Cards_report b on a.t_report_c_id=b.t_c_id
                    where b.t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'");

                //获得累计入库数
                DataTable dtRK_List = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select a.t_ctype,isnull(sum(b.iquantity),0) rk_qty
                    from " + dbname + @"..T_CC_Rd10_FlowCard a inner join " + dbname + @"..rdrecords10 b on a.t_autoid=b.autoid and b.bRelated=0 where a.t_card_id=0" + t_card_id + " group by a.t_ctype");
                for (int l = 0; l < dtRK_List.Rows.Count; l++)
                {
                    string c_data = "";
                    if (dtRK_List.Rows[l]["t_ctype"] + "" == "1")
                    {
                        c_data = dtData.Rows[0]["t_valid_qty"] + "";
                        if (c_data == "") c_data = "0";
                        if (float.Parse(c_data) < float.Parse("" + dtRK_List.Rows[l]["rk_qty"]))
                            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]超合格数据入库");
                    }
                    if (dtRK_List.Rows[l]["t_ctype"] + "" == "2")
                    {
                        if (float.Parse("" + dtRpt_data.Rows[0]["feip_n"]) < float.Parse("" + dtRK_List.Rows[l]["rk_qty"]))
                            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]超内部废品入库");
                    }
                    if (dtRK_List.Rows[l]["t_ctype"] + "" == "3")
                    {
                        if (float.Parse("" + dtRpt_data.Rows[0]["t_repair_qty"]) < float.Parse("" + dtRK_List.Rows[l]["rk_qty"]))
                            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]超返修量入库");
                    }
                    if (dtRK_List.Rows[l]["t_ctype"] + "" == "4")
                    {
                        if (float.Parse("" + dtRpt_data.Rows[0]["t_renew_qty"]) < float.Parse("" + dtRK_List.Rows[l]["rk_qty"]))
                            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]超改制品入库");
                    }
                    if (dtRK_List.Rows[l]["t_ctype"] + "" == "5")
                    {
                        if (float.Parse("" + dtRpt_data.Rows[0]["feip_w"]) < float.Parse("" + dtRK_List.Rows[l]["rk_qty"]))
                            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]超外协废品入库");
                    }
                }

                #endregion

            }
        }
        #endregion

        #region   //报告母卡（拆卡前的 报告母卡）  数据回写
        //报告母卡   将报告子卡的各项数据汇总后，回写母卡的对应数据（返修卡的 数据不能包含）
        //返修卡无需处理母卡数据
        if (c_card_type == "标准卡" && int.Parse("" + dtProcess.Rows[0]["t_father_report_c_id"]) > 0)   //母卡 报告明细ID 为0时，无需计算 母卡的总报工数
        {
            dtData = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select sum(t_working_ven_qty) t_working_ven_qty,sum(t_valid_qty) t_valid_qty,sum(t_scrap_work) t_scrap_work,
	                    sum(t_scrap_material) t_scrap_material,sum(t_scrap_qc) t_scrap_qc,sum(t_miss_qty) t_miss_qty,sum(t_repair_qty) t_repair_qty,sum(t_renew_qty) t_renew_qty,
                        sum(isnull(t_worked_qty,0)) t_worked_qty
                    from " + dbname + @"..t_cc_card_list a inner join " + dbname + @"..T_CC_Cards_report b on a.t_card_no=b.t_card_no
                    where a.t_card_type='标准卡' and b.t_father_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"]);
            if (dtData.Rows.Count > 0)
            {
                cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_valid_qty=0" + dtData.Rows[0]["t_valid_qty"] + ",t_scrap_work=0" + dtData.Rows[0]["t_scrap_work"] +
                    ",t_scrap_material=0" + dtData.Rows[0]["t_scrap_material"] + ",t_scrap_qc=0" + dtData.Rows[0]["t_scrap_qc"] + ",t_miss_qty=0" + dtData.Rows[0]["t_miss_qty"] +
                    ",t_repair_qty=0" + dtData.Rows[0]["t_repair_qty"] + ",t_renew_qty=0" + dtData.Rows[0]["t_renew_qty"] + ",t_working_ven_qty=0" + dtData.Rows[0]["t_working_ven_qty"] +
                    ",t_worked_qty=0" + dtData.Rows[0]["t_worked_qty"] + " where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"];
                cmd.ExecuteNonQuery();

                //母卡在制数处理(拆分前流转卡处理)   --------------------------------------------------------
                cmd.CommandText = @"update " + dbname + @"..T_CC_Cards_report set t_working_qty=t_tran_in_qty-t_working_ven_qty-t_valid_qty-t_scrap_work-t_scrap_material-t_scrap_qc-t_miss_qty-t_repair_qty-t_renew_qty-isnull(t_working_ware_qty,0)  -isnull(t_working_qc_qty,0)  
                    where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"];
                cmd.ExecuteNonQuery();

                //母卡下道工序转入数
                //写下道工序的 合格转入数 和 厂内在制数
                DataTable dtFatherNextSeq = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select t_c_id f_next_cid,t_opcode,t_wc_code,t_op_warehouse from " + dbname + @"..T_CC_Cards_report 
                    where t_up_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"]);
                if (dtFatherNextSeq.Rows.Count > 0)
                {
                    //判断下道工序是否要求工序仓库
                    if ((dtFatherNextSeq.Rows[0]["t_op_warehouse"] + "").CompareTo("是") != 0)//不是工序仓库时，直接更新下工序转入数
                    {
                        //【上工序所有合格数 减去 本工序的 外厂在制数-各种本工序的合格数和废品数、查缺数】=本工序在制待工数    ----------------------------------------
                        cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_tran_in_qty=0" + dtData.Rows[0]["t_valid_qty"] +
                                ",t_working_qty=(0" + dtData.Rows[0]["t_valid_qty"] + @")-t_working_ven_qty-t_valid_qty-t_scrap_work-t_scrap_material-t_scrap_qc-t_miss_qty-t_repair_qty-t_renew_qty   -isnull(t_working_qc_qty,0) 
                            where t_c_id=0" + dtFatherNextSeq.Rows[0]["f_next_cid"];
                        cmd.ExecuteNonQuery();
                        t_working_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_working_qty from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtFatherNextSeq.Rows[0]["f_next_cid"]);
                        UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select '在制数打印3:" + t_working_qty + "'");
                    }
                }
                else  //末道工序，回写流转卡的 完工数
                {
                    string father_cardno = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select t_card_no from " + dbname + @"..T_CC_Cards_report 
                        where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"]);
                    cmd.CommandText = "update " + dbname + @"..T_CC_Card_List set t_card_overqty=0" + dtData.Rows[0]["t_valid_qty"] + " where t_card_no='" + father_cardno + "'";
                    cmd.ExecuteNonQuery();
                }
            }

        }
        #endregion

        #region //超报工量控制（是否超报工了）
        //判定上下工序逻辑关系
        //1. 报告工序  本工序在制数不能 小于0
        if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select count(*) from " + dbname + @"..T_CC_Cards_report 
            where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"] + " and (t_working_qty<0 or isnull(t_worked_qty,0)<0)")) > 0)
        {
            t_working_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_working_qty from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select '在制数打印4:" + t_working_qty + "'");
            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]工序[" + dtProcess.Rows[0]["t_opcode"] + "]工序超量报工");
        }
        //2. 报工工序的所有 状态制品的 报工合计数 不能超 本工序对应的报告工序的   总转入数=总转入数 -工序库存数
        float fTranIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(max(t_tran_in_qty-isnull(t_working_ware_qty,0)),0) from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]));
        float fBaoGongAll = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select t_valid_qty+t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty
            from " + dbname + @"..T_CC_Cards_process where t_c_id=0" + i_process_c_id));
        if (fTranIn < fBaoGongAll) throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]本工序超出总报工数【" + fTranIn + "】");

        //3. 非标返修卡 中间工序（非报告工序） 能否超流转卡总量报工
        if (UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_center_opcode_rpt_control'").ToLower() == "true")
        {
            float f_ll_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_card_qty from " + dbname + @"..T_CC_Card_List where t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'"));
            float f_all_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(t_valid_qty)+sum(t_scrap_work)+sum(t_scrap_material)+sum(t_scrap_qc)+sum(t_miss_qty)+sum(t_repair_qty)+sum(t_renew_qty),0)
                from " + dbname + @"..T_CC_pro_report_list where t_process_c_id=0" + i_process_c_id));
            if (f_ll_qty < f_all_qty) throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]本工序超出总报工数【" + f_ll_qty + "】");
        }
        #endregion
        // 多段拆卡超报工量控制
        MutiSplitControl(cmd, dbname, i_process_c_id);
    }
    /// <summary>
    /// 多段拆卡超报工量控制--whf
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="dbname"></param>
    /// <param name="i_process_c_id"></param>
    private static void MutiSplitControl(SqlCommand cmd, string dbname, string i_process_c_id)
    {
        #region // 多段拆卡控制
        DataTable dtProcess = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"
select distinct a.cardNo,tCard.t_card_no,d.t_father_process_c_id ,d.t_father_report_c_id 
,d.t_opcode 
from " + dbname + @"..t_cqcc_mutilSplit a 
inner join " + dbname + @"..T_CC_Card_List fCard on a.cardNo = fCard.t_card_no
inner join " + dbname + @"..T_CC_Card_List tCard on tCard.t_father_card_id= fCard.t_card_id
inner join " + dbname + @"..T_CC_Cards_process d on d.t_card_no = tCard.t_card_no
where d.t_c_id = " + i_process_c_id);
        // 判断是否多段拆卡
        if (dtProcess.Rows.Count == 0)
        {
            return;
        }

        #endregion
        #region //多段拆卡超报工量控制（是否超报工了）
        //判定上下工序逻辑关系
        //1. 报告工序  本工序在制数不能 小于0
        if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select count(*) from " + dbname + @"..T_CC_Cards_report 
            where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"] + " and (t_working_qty<0 or isnull(t_worked_qty,0)<0)")) > 0)
        {
            throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]工序[" + dtProcess.Rows[0]["t_opcode"] + "]工序超量报工");
        }
        //2. 报工工序的所有 状态制品的 报工合计数 不能超 本工序对应的报告工序的   总转入数=总转入数 -工序库存数
        float fTranIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(max(t_tran_in_qty-isnull(t_working_ware_qty,0)),0) from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_father_report_c_id"]));
        float fBaoGongAll = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select t_valid_qty+t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty
            from " + dbname + @"..T_CC_Cards_process where t_c_id=0" + dtProcess.Rows[0]["t_father_process_c_id"]));
        if (fTranIn < fBaoGongAll) throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]本工序超出总报工数【" + fTranIn + "】");

        //3. 非标返修卡 中间工序（非报告工序） 能否超流转卡总量报工
        if (UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_center_opcode_rpt_control'").ToLower() == "true")
        {
            float f_ll_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select t_card_qty from " + dbname + @"..T_CC_Card_List where t_card_no='" + dtProcess.Rows[0]["t_card_no"] + "'"));
            float f_all_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(t_valid_qty)+sum(t_scrap_work)+sum(t_scrap_material)+sum(t_scrap_qc)+sum(t_miss_qty)+sum(t_repair_qty)+sum(t_renew_qty),0)
                from " + dbname + @"..T_CC_pro_report_list where t_process_c_id=0" + dtProcess.Rows[0]["t_father_process_c_id"]));
            if (f_ll_qty < f_all_qty) throw new Exception("流转卡[" + dtProcess.Rows[0]["t_card_no"] + "]本工序超出总报工数【" + f_ll_qty + "】");
        }
        #endregion
    }

    /// <summary>
    /// 多段开卡的获取报工 工序的 可报工量--whf
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="i_process_c_id"></param>
    /// <returns></returns>
    private static decimal GetCurrentOpCanReportQtyForMutiSplit(SqlCommand cmd, string dbname, string i_process_c_id)
    {
        #region // 多段拆卡控制
        DataTable dtMuti = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"
select distinct a.cardNo,tCard.t_card_no,d.t_father_process_c_id ,d.t_father_report_c_id 
,d.t_opcode 
from " + dbname + @"..t_cqcc_mutilSplit a 
inner join " + dbname + @"..T_CC_Card_List fCard on a.cardNo = fCard.t_card_no
inner join " + dbname + @"..T_CC_Card_List tCard on tCard.t_father_card_id= fCard.t_card_id
inner join " + dbname + @"..T_CC_Cards_process d on d.t_card_no = tCard.t_card_no
where d.t_c_id = " + i_process_c_id);
        // 判断是否多段拆卡
        if (dtMuti.Rows.Count == 0)
        {
            return -1;
        }

        #endregion
        // 获取母卡工序的可报工数量
        i_process_c_id = dtMuti.Rows[0]["t_father_process_c_id"] + "";
        DataTable dtProcess = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, @"select a.t_c_id t_process_c_id,a.t_report_c_id,a.t_father_process_c_id,a.t_father_report_c_id,
                    a.t_card_no,a.t_starttime,b.t_op_warehouse,b.t_opcode,b.t_wc_code 
                from " + dbname + @"..T_CC_Cards_process a 
inner join " + dbname + @"..T_CC_Cards_report b on a.t_report_c_id=b.t_c_id where a.t_c_id=" + i_process_c_id);
        if (dtProcess.Rows.Count == 0) return 0;

        decimal fTranIn = 0;
        //非合格数汇总
        decimal fBaoGongAll = decimal.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty),0)
                from " + dbname + @"..T_CC_Cards_process where t_report_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]));

        string c_ware_auto_out = GetMESSysParm(cmd, "mes_flow_op_ware_auto_out_control");
        string c_parm_auto_out = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select b.t_is_WareOut_Auto 
from " + dbname + @"..T_CC_Cards_report a 
inner join " + dbname + @"..T_CC_ProCtl_Setting b on a.t_opcode=b.t_opcode and a.t_wc_code=b.t_wccode where a.t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
        if (dtProcess.Rows[0]["t_op_warehouse"] + "" == "是" && (c_parm_auto_out == "是" || c_ware_auto_out.ToLower().CompareTo("true") == 0))   //获得参数信息【是否自动出库】
        {
            //工序仓自动出库：工序仓库累计收料数
            fTranIn = decimal.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(SUM(t_rd_in_qty),0) from " + dbname + @"..T_CC_op_rd_records where t_rpt_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]));
        }
        else
        {
            //正产情况：转入数-在库数-在检数
            fTranIn = decimal.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(max(t_tran_in_qty-isnull(t_working_ware_qty,0)-isnull(t_working_qc_qty,0)),0) 
                    from " + dbname + @"..T_CC_Cards_report where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]));
        }
        //本工序的合格数
        decimal fvalidAll = decimal.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select t_valid_qty from " + dbname + @"..T_CC_Cards_process where t_c_id=0" + i_process_c_id));
        decimal rtQty = fTranIn - fBaoGongAll - fvalidAll;
        return rtQty;
    }

    [WebMethod] //获得报工 工序的 可报工量
    public decimal GetCurrentOpCanReportQty(string i_process_c_id, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        try
        {
            SqlCommand cmd = Conn.CreateCommand();
            DataTable dtProcess = GetSqlDataTable(@"select a.t_c_id t_process_c_id,a.t_report_c_id,a.t_father_process_c_id,a.t_father_report_c_id,a.t_card_no,a.t_starttime,b.t_op_warehouse 
                from " + dbname + "..T_CC_Cards_process a inner join " + dbname + @"..T_CC_Cards_report b on a.t_report_c_id=b.t_c_id 
                where a.t_c_id=" + i_process_c_id, "dtProcess", cmd);
            if (dtProcess.Rows.Count == 0) return 0;
            ////判断是否开工管理
            //string cpvalue = GetDataString("select cvalue FROM " + dbname + "..T_Parameter where cPid='mes_flow_starttime_opcontrol'", cmd);
            //if (cpvalue.Trim().ToLower().CompareTo("true") == 0)
            //{
            //    if (dtProcess.Rows[0]["t_starttime"] + "" == "") throw new Exception("本工序需要先开工处理");
            //}
            // whf float对于小数精度不够 改成decimal
            decimal fTranIn = 0;
            string c_ware_auto_out = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cValue from " + dbname + @"..T_Parameter where cPid='mes_flow_op_ware_auto_out_control'");
            string c_parm_auto_out = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select b.t_is_WareOut_Auto from " + dbname + @"..T_CC_Cards_report a 
                    inner join " + dbname + @"..T_CC_ProCtl_Setting b on a.t_opcode=b.t_opcode and a.t_wc_code=b.t_wccode where a.t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]);
            if (dtProcess.Rows[0]["t_op_warehouse"] + "" == "是" && (c_parm_auto_out == "是" || c_ware_auto_out.ToLower().CompareTo("true") == 0))
            {
                //工序仓自动出库：工序仓库累计收料数
                fTranIn = decimal.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(t_rd_in_qty),0) from " + dbname + @"..T_CC_op_rd_records where t_rpt_c_id=0" + dtProcess.Rows[0]["t_report_c_id"]));
            }
            else
            {
                //正产情况：转入数-在库数-在检数
                fTranIn = decimal.Parse(GetDataString("select isnull(max(t_tran_in_qty-isnull(t_working_ware_qty,0)-isnull(t_working_qc_qty,0)),0) from " + dbname + @"..T_CC_Cards_report 
                    where t_c_id=0" + dtProcess.Rows[0]["t_report_c_id"], cmd));
            }
            GetDataString("select 'fTranIn:" + fTranIn + "'", cmd);
            decimal fBaoGongAll = decimal.Parse(GetDataString(@"select isnull(sum(t_scrap_work+t_scrap_material+t_scrap_qc+t_miss_qty+t_repair_qty+t_renew_qty),0)
                from " + dbname + @"..T_CC_Cards_process where t_report_c_id=0" + dtProcess.Rows[0]["t_report_c_id"], cmd));
            decimal fvalidAll = decimal.Parse(GetDataString(@"select t_valid_qty from " + dbname + @"..T_CC_Cards_process where t_c_id=0" + i_process_c_id, cmd));
            //return fTranIn - fBaoGongAll - fvalidAll;
            decimal rtQty = fTranIn - fBaoGongAll - fvalidAll;
            decimal mQty = GetCurrentOpCanReportQtyForMutiSplit(cmd, dbname, i_process_c_id);
            if (mQty == -1)
            {
                return rtQty;
            }
            return rtQty < mQty ? rtQty : mQty;
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

    [WebMethod]  //获得当前序列号 加工工序信息
    public DataTable MES_SnProcessInfo(string sn, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();

        try
        {
            //获得参数
            DataTable dtCard = GetSqlDataTable(@"select b.t_card_no,a.t_card_id,a.t_flow_seq
                from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    where a.t_sn_no='" + sn + "'", "dtCard", Cmd);
            if (dtCard.Rows.Count == 0) throw new Exception("没有找到流转卡信息");
            string c_flow_seq = GetDataString(@"select isnull(max(b.t_card_seq),'')
                from " + dbname + @"..T_CC_pro_report_sn a inner join " + dbname + @"..T_CC_Cards_process b on a.t_process_c_id=b.t_c_id
                where b.t_card_no='" + dtCard.Rows[0]["t_card_no"] + "'  and a.t_sn_no='" + sn + "'", Cmd);
            //获得下到工序信息
            DataTable dtOpInfo = GetSqlDataTable(@"select top 1 t_c_id,t_opcode,b.Description opname,t_card_no,t_card_seq,t_wc_code,c.Description wcname 
                from " + dbname + @"..T_CC_Cards_process a inner join " + dbname + @"..sfc_operation b on a.t_opcode=b.OpCode
                    inner join " + dbname + @"..sfc_workcenter c on a.t_wc_code=c.WcCode
                where a.t_card_no='" + dtCard.Rows[0]["t_card_no"] + "' and a.t_card_seq>'" + c_flow_seq + @"'
                order by a.t_card_seq", "dtOpInfo", Cmd);
            return dtOpInfo;

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

    [WebMethod]  //U8 产品报检单   返回单据 ID,单据号
    public bool MES_Op_Warehouse_Receive(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        System.Data.SqlClient.SqlConnection sqlCon = OpenDataConnection();
        if (sqlCon == null) throw new Exception("数据库连接失败！");

        string c_qty_value = GetTextsFrom_FormData_Text(FormData, "txt_iquantity");  //原数量
        string c_rcv_count = GetTextsFrom_FormData_Text(FormData, "txt_ireceiveqty"); //接收数量
        try
        {
            if (c_qty_value.CompareTo("") == 0) c_qty_value = "0";
            if (c_rcv_count.CompareTo("") == 0) c_rcv_count = "0";

            if (int.Parse(c_qty_value) < int.Parse(c_rcv_count) || int.Parse(c_rcv_count) <= 0)
            {
                throw new Exception("接收数量必须 大于0 并且不能超");
            }
        }
        catch
        {
            throw new Exception("接收数量必须为 整数");
        }

        SqlCommand Cmd = sqlCon.CreateCommand();
        Cmd.Transaction = sqlCon.BeginTransaction();

        try
        {
            string c_modid = GetTextsFrom_FormData_Text(FormData, "txt_modid");
            string c_next_rpt_c_id = GetTextsFrom_FormData_Tag(FormData, "txt_seqno");  //下工序
            string c_this_rpt_c_id = GetTextsFrom_FormData_Tag(FormData, "txt_t_morder_code");  //本工序
            string c_this_rpt_father_cid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select t_father_c_id from " + dbname + @"..T_CC_Cards_report a where t_c_id=" + c_this_rpt_c_id);
            string c_pos_code = GetTextsFrom_FormData_Text(FormData, "txt_tPosCode");

            DataTable dtNextInfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_card_no,t_card_seq,t_opcode,t_wc_code,t_father_c_id from " + dbname + @"..T_CC_Cards_report a 
                    where a.t_c_id=0" + c_next_rpt_c_id);
            if (dtNextInfo.Rows.Count == 0) throw new Exception("未找到下道报告工序信息");
            //获得下道工序参数信息
            DataTable dtNextOpSetting = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select * from " + dbname + @"..T_CC_ProCtl_Setting 
                    where t_wccode='" + dtNextInfo.Rows[0]["t_wc_code"] + "' and t_opcode='" + dtNextInfo.Rows[0]["t_opcode"] + "'");

            //判断是否需要货位
            if (dtNextOpSetting.Rows.Count > 0 && dtNextOpSetting.Rows[0]["t_is_position"] + "" == "是" && c_pos_code == "")
                throw new Exception("工序接收要求需要货位信息");

            //添加接收记录
            Cmd.CommandText = @"insert into " + dbname + @"..T_CC_op_rd_records(t_card_no,t_card_seq,t_opcode,t_rpt_c_id,t_op_poscode,t_rd_in_qty,t_td_out_qty,t_maker,t_makedate,t_report_list_did,t_issue_did,t_rpt_up_c_id) 
                    values('" + dtNextInfo.Rows[0]["t_card_no"] + "','" + dtNextInfo.Rows[0]["t_card_seq"] + "','" + dtNextInfo.Rows[0]["t_opcode"] + "'," + c_next_rpt_c_id + @",
                        '" + c_pos_code + "'," + c_rcv_count + ",0,'" + cUserName + "',getdate(),0,0," + c_this_rpt_c_id + ")";
            Cmd.ExecuteNonQuery();

            //修改工序仓库库存量
            if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select COUNT(*) from " + dbname + @"..T_CC_op_Stock a where t_rpt_c_id=" + c_next_rpt_c_id)) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_op_Stock(t_rpt_c_id,t_op_poscode,t_stock_qty,t_rd_in_time) values(" + c_next_rpt_c_id + ",'',0,convert(varchar(20),getdate(),120))";
                Cmd.ExecuteNonQuery();
            }
            string cStock_qty = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(sum(t_rd_in_qty)-sum(t_td_out_qty),0) from " + dbname + @"..T_CC_op_rd_records a 
                    where a.t_rpt_c_id=0" + c_next_rpt_c_id);
            Cmd.CommandText = "update " + dbname + @"..T_CC_op_Stock set t_op_poscode='" + c_pos_code + "',t_stock_qty=0" + cStock_qty + " where t_rpt_c_id=" + c_next_rpt_c_id;
            Cmd.ExecuteNonQuery();

            //转移工序待收数量 到 下工序转入数/在制库存数
            Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_worked_qty=isnull(t_worked_qty,0)-" + c_rcv_count + " where t_c_id=" + c_this_rpt_c_id;
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_tran_in_qty=t_tran_in_qty+" + c_rcv_count + ",t_working_ware_qty=isnull(t_working_ware_qty,0)+" + c_rcv_count + @" 
                    where t_c_id=" + c_next_rpt_c_id;
            Cmd.ExecuteNonQuery();

            //判断是否上工序待转量是否为负
            if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select COUNT(*) from " + dbname + @"..T_CC_Cards_report where t_c_id=" + c_this_rpt_c_id + " and isnull(t_worked_qty,0)<0")) > 0)
                throw new Exception("超本工序完工待转量");

            if (int.Parse("" + dtNextInfo.Rows[0]["t_father_c_id"]) > 0)  //存在母卡
            {
                //减少本工序母卡待收量  
                Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_worked_qty=isnull(t_worked_qty,0)-" + c_rcv_count + " where t_c_id=" + c_this_rpt_father_cid;
                Cmd.ExecuteNonQuery();
                //增加母报告卡下工序的转入数（下工序）
                Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_report set t_tran_in_qty=t_tran_in_qty+" + c_rcv_count + ",t_working_ware_qty=isnull(t_working_ware_qty,0)+" + c_rcv_count + @" 
                    where t_c_id=" + dtNextInfo.Rows[0]["t_father_c_id"];
                Cmd.ExecuteNonQuery();
            }

            Cmd.Transaction.Commit();

            return true;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            throw ex;
        }


    }
    #endregion  //                         $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    #region   //序列号处理
    [WebMethod]  //扫描序列号 报工
    public string MES_SN_Report(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        //为DataTable 建立索引
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string cbg_type = GetTextsFrom_FormData_Text(FormData, "txt_t_report_type");
            string sn = GetTextsFrom_FormData_Text(FormData, "txt_barcode");
            string flow_seq = GetTextsFrom_FormData_Text(FormData, "txt_flow_seq");
            string pro_c_id = GetTextsFrom_FormData_Tag(FormData, "txt_flow_seq");
            string t_opcode = GetTextsFrom_FormData_Tag(FormData, "txt_t_opcode");
            DataRow dr = FormData.NewRow();
            if (cbg_type.CompareTo("合格") == 0)
            {
                dr["LabelText"] = "合格数";
                dr["TxtName"] = "txt_t_valid_qty";
            }
            if (cbg_type.CompareTo("料废") == 0)
            {
                dr["LabelText"] = "料废数";
                dr["TxtName"] = "txt_t_scrap_material";
            }
            if (cbg_type.CompareTo("工废") == 0)
            {
                dr["LabelText"] = "工废数";
                dr["TxtName"] = "txt_t_scrap_work";
            }
            if (cbg_type.CompareTo("检废") == 0)
            {
                dr["LabelText"] = "检废数";
                dr["TxtName"] = "txt_t_scrap_qc";
            }

            if (dr["TxtName"] + "" == "") throw new Exception("接口调用错误，请确认条码系统版本");
            dr["TxtTag"] = "1";
            dr["TxtValue"] = "1";
            FormData.Rows.Add(dr);

            string cRet_Code = Mes_rpt_to_db(FormData, dbname, cUserName, cLogDate, "", SheetID, false, Cmd);

            //记录序列号信息
            Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_sn(t_sn_no,t_card_seq,t_opcode,t_process_c_id,t_report_list_id,t_time) 
                values('" + sn + "','" + flow_seq + "','" + t_opcode + "',0" + pro_c_id + ",0" + cRet_Code.Split(',')[0] + ",getdate())";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = @"update " + dbname + @"..T_CC_Cards_Sn set t_flow_seq='流转' where t_sn_no='" + sn + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return cRet_Code;
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

    [WebMethod]  //序列号 扫描 检查验证
    public bool MES_CHeckSn(string sn, string cardno, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        try
        {
            //获得参数
            string c_controled = GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cpid='mes_flow_checkCardFromSn'", Cmd);
            if (c_controled.ToLower().CompareTo("true") == 0)
            {
                if (GetDataInt(@"select count(*) from " + dbname + "..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    where a.t_sn_no='" + sn + "' and b.t_card_no='" + cardno + "'", Cmd) == 0)
                    throw new Exception("序列号不属于当前流转卡！");
            }

            if (GetDataInt(@"select count(*) from " + dbname + @"..T_CC_Cards_Sn a 
                    where a.t_sn_no='" + sn + "' and a.t_flow_seq='创建'", Cmd) == 0)
                throw new Exception("序列号已经张贴或不存在，不能重复操作！");

            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_flow_seq='张贴' where t_sn_no='" + sn + "' ";
            Cmd.ExecuteNonQuery();

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

    [WebMethod]  //获得序列号详细信息(入库)
    public DataTable MES_SN_Rd10_Info(string sn, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        try
        {
            SqlCommand Cmd = Conn.CreateCommand();
            DataTable dtSn = null;
            //判断条码是否箱码
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "'", Cmd) > 0)
            {
                //判断是否 存在 CCD序列号和ERP序列号不一致问题
                DataTable dtNotSn = GetSqlDataTable(@"select distinct a.csn from " + dbname + @"..T_CC_SN_Box_DZ a left join " + dbname + @"..T_CC_Cards_Sn b on a.csn=b.t_sn_no
                    where a.boxcode='" + sn + "' and b.t_card_id is null", "dtNotSn", Cmd);
                if (dtNotSn.Rows.Count > 0)
                    throw new Exception("序列号[" + dtNotSn.Rows[0]["csn"] + "]在ERP中不存在或者已经作废");

                //判断是否有已入库 序列号
                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_flow_seq,a.t_state
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where c.boxcode='" + sn + "' and a.t_flow_seq not in('创建','流转') and a.t_state<>'作废' ", "dtSn", Cmd);
                if (dtSn.Rows.Count > 0)
                    throw new Exception("序列号[" + dtSn.Rows[0]["t_sn_no"] + "]是[" + dtSn.Rows[0]["t_flow_seq"] + "]状态，不能入库。请查序列号状态表查看详细信息");

                //箱码
                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_flow_seq,a.t_state
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where c.boxcode='" + sn + "' and a.t_flow_seq in('创建','流转') and a.t_state<>'作废' ", "dtSn", Cmd);
                if (dtSn.Rows.Count == 0)  //若没有找到本箱可以入库的序列号，即判断序列号状态
                {
                    dtSn = GetSqlDataTable(@"select a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_flow_seq,a.t_state
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where c.boxcode='" + sn + "'", "dtSn", Cmd);
                    if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]箱码无装箱信息");
                    //检查序列号详细问题
                    if (dtSn.Rows[0]["t_state"] + "" == "作废")
                    {
                        throw new Exception("箱码[" + sn + "]中的序列号如[" + dtSn.Rows[0]["t_sn_no"] + "]状态为" + dtSn.Rows[0]["t_flow_seq"] + " 且 已经作废");
                    }
                    else
                    {
                        throw new Exception("箱码[" + sn + "]中的序列号如[" + dtSn.Rows[0]["t_sn_no"] + "]状态为" + dtSn.Rows[0]["t_flow_seq"]);
                    }
                }
            }
            else
            {
                ////序列号
                //if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_FlowCard_SN_Rd10 where csn='" + sn + "'", Cmd) > 0)
                //    throw new Exception("[" + sn + "]序列号已经入库");

                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_flow_seq,a.t_state
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    left join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where a.t_sn_no='" + sn + "' ", "dtSn", Cmd);
                if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]序列号 不存在");
                if (dtSn.Rows[0]["t_flow_seq"] + "" != "创建" && dtSn.Rows[0]["t_flow_seq"] + "" != "流转")
                    throw new Exception("[" + sn + "]序列号状态不是“创建”，“流转”");
                if (dtSn.Rows[0]["t_state"] + "" == "作废")
                    throw new Exception("[" + sn + "]序列号已经作废");
            }

            //校验是否存在试漏
            for (int i = 0; i < dtSn.Rows.Count; i++)
            {
                DataTable dtInvInfo = GetSqlDataTable("select cInvDefine8,cInvDefine9 from " + dbname + @"..inventory where cinvcode='" + dtSn.Rows[i]["t_invcode"] + "'", "dtInvInfo", Cmd);
                if (dtInvInfo.Rows.Count == 0) throw new Exception("序列号[" + dtSn.Rows[i]["t_sn_no"] + "]对应存货[" + dtSn.Rows[i]["t_invcode"] + "]不存在");

                dtSn.Rows[i]["cstate"] = "是";
                if (dtInvInfo.Rows[0]["cInvDefine8"] + "" == "是")
                {
                    if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_FlowCard_SN_SLRecord2 where csn='" + dtSn.Rows[i]["t_sn_no"] + "' and Result='OK'", Cmd) == 0)
                        dtSn.Rows[i]["cstate"] = "否";
                }

                if (dtInvInfo.Rows[0]["cInvDefine9"] + "" == "是")
                {
                    if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_FlowCard_SN_TQMTQLRecord2 where csn='" + dtSn.Rows[i]["t_sn_no"] + "' and Result='OK'", Cmd) == 0)
                        dtSn.Rows[i]["cstate"] = "否";
                }
            }

            return dtSn;
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

    [WebMethod]  //获得序列号详细信息(在库)
    public DataTable MES_SN_Disp_Info_ex(string sn, string dbname, bool bOnlyBox)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        try
        {
            SqlCommand Cmd = Conn.CreateCommand();
            DataTable dtSn = null;
            string ccus_code = GetDataString("select cValue from " + dbname + @"..T_Parameter where cPid='mes_sn_huawei_cuscode'", Cmd);
            bool b_only_fh = false;
            string dips_only_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_sn_only_disp'", Cmd);
            string cSnBatchNo = "a.t_cbatch";  //批号来源规则
            if (dips_only_sn.ToLower().CompareTo("true") == 0)
            {
                b_only_fh = true; //只控制发货，其他情况不控制
                cSnBatchNo = "'' t_cbatch";
            }

            //判断条码是否箱码
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "'", Cmd) > 0 || bOnlyBox)
            {
                int i_box_all = GetDataInt("select count(distinct csn) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "' and isnull(BoxState,'有效')='有效'", Cmd);
                //箱码
                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_ware_code,w.cwhname," + cSnBatchNo + @",m.cCusInvCode
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    left join " + dbname + @"..warehouse w on a.t_ware_code=w.cwhcode
                    left join " + dbname + @"..CusInvContrapose m on b.t_invcode=m.cInvCode and m.ccuscode='" + ccus_code + @"'
                    where c.boxcode='" + sn + "'" + (b_only_fh ? "" : " and a.t_ware_code<>''"), "dtSn", Cmd);

                if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]箱码无可在库序列号信息");
                if (i_box_all != dtSn.Rows.Count) throw new Exception("[" + sn + "]箱码存在部分序列号不可发货状态");
            }
            else
            {
                //序列号
                if (!b_only_fh)  //严格按照流程控制
                {
                    if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_Cards_Sn where t_sn_no='" + sn + "' and t_ware_code<>''", Cmd) == 0)
                        throw new Exception("[" + sn + "]序列号非在库状态");
                }

                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_ware_code,w.cwhname," + cSnBatchNo + @",m.cCusInvCode
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    left join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    left join " + dbname + @"..warehouse w on a.t_ware_code=w.cwhcode
                    left join " + dbname + @"..CusInvContrapose m on b.t_invcode=m.cInvCode and m.ccuscode='" + ccus_code + @"'
                    where a.t_sn_no='" + sn + "'", "dtSn", Cmd);

                if (dtSn.Rows.Count > 1) throw new Exception("[" + sn + "]序列号重复");
                if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]序列号 不存在");
            }

            return dtSn;
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

    [WebMethod]  //获得序列号详细信息(在库)
    public DataTable MES_SN_Disp_Info(string sn, string dbname)
    {
        return MES_SN_Disp_Info_ex(sn, dbname, false);
    }


    [WebMethod]  //获得序列号详细信息(销售)
    public DataTable MES_SN_Tui_Info(string sn, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        try
        {
            SqlCommand Cmd = Conn.CreateCommand();
            DataTable dtSn = null;
            //判断条码是否箱码
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "'", Cmd) > 0)
            {
                //箱码
                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,a.t_cbatch
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where c.boxcode='" + sn + "' and a.t_flow_seq='销售'", "dtSn", Cmd);

                if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]箱码无销售状态的库序列号信息");
            }
            else
            {
                //序列号
                if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_Cards_Sn where t_sn_no='" + sn + "' and t_flow_seq='销售'", Cmd) == 0)
                    throw new Exception("[" + sn + "]序列号非销售状态");

                dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,a.t_cbatch
                    from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                    left join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                    where a.t_sn_no='" + sn + "'", "dtSn", Cmd);

                if (dtSn.Rows.Count > 1) throw new Exception("[" + sn + "]序列号重复");
                if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]序列号 不存在");
            }

            return dtSn;
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

    [WebMethod]  //序列号清单入库*
    public string MES_SN_GoodIn(string cInType, System.Data.DataTable FormData, string cwhcode, string dbname, string cUserName, string cLogDate, string SheetID)
    {

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //组合入库样式（标准产品入库）
            string crdcode = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81015' and t_fieldname='crdcode'", Cmd);
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            if (crdcode == "") throw new Exception("请设置模板[U81015] 的入库类别的缺省值");
            if (cwhcode == "") throw new Exception("请选择仓库");
            #region  //产品入库
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (("" + dtSnChk.Rows[l]["t_ware_code"]).CompareTo("") != 0)
                    throw new Exception("序列号仓库发生变化，当前是在库状态，不能重复入库");
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("创建") != 0 && ("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("流转") != 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "] 非流转状态");
            }

            //流转卡号 写入 入库单表体自定义项：rd_definename_to_flow
            string rd_definename_to_flow = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_flow_code'", Cmd);
            string body_define_cardno = rd_definename_to_flow;
            rd_definename_to_flow = rd_definename_to_flow.Replace("cdefine", "define");
            if (rd_definename_to_flow == "") throw new Exception("请设置：[流转卡入库时流转卡存放表体自定义项字段名称]");

            //序列号 写入 入库单表体自定义项：rd_definename_to_sn
            string rd_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_sn_body'", Cmd);
            string body_define_sn = rd_definename_to_sn;
            rd_definename_to_sn = rd_definename_to_sn.Replace("cdefine", "define");
            if (rd_definename_to_sn == "") throw new Exception("请设置：[序列号入库 存放产品入库单表体自定义项栏目]");

            //更新流转卡交库类型到入库单表头自定义项  txt_cdefine1
            string c_flow_rd10_flow_type = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_flow_type'", Cmd);
            c_flow_rd10_flow_type = c_flow_rd10_flow_type.Replace("cdefine", "txt_cdefine");
            if (c_flow_rd10_flow_type == "") throw new Exception("请设置：[流转卡入库时交库类型存放产品入库单表头自定义项栏目]");

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "表头货位";
            dr["TxtName"] = "txt_headposcode";
            dr["TxtTag"] = "";
            dr["TxtValue"] = "";
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "入库类别";
            dr["TxtName"] = "txt_crdcode";
            dr["TxtTag"] = crdcode;
            dr["TxtValue"] = crdcode;
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "交库类型";
            dr["TxtName"] = c_flow_rd10_flow_type;
            dr["TxtTag"] = cInType;
            dr["TxtValue"] = cInType;
            dtHead.Rows.Add(dr);
            #endregion
            DataTable dtBody = GetSqlDataTable(@"select b.t_modid modid,b.t_invcode cinvcode,1 iquantity,replace(CONVERT(varchar(10),getdate(),120),'-','') cbatch,'' cposcode,
                    a.t_sn_no " + rd_definename_to_sn + ",b.t_card_no " + rd_definename_to_flow + @",b.t_card_id,isnull(ex_2,'') ex_2,'0' cmjno,'' define31
                from " + dbname + "..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                where a.t_sn_no in(" + sn_list + ")", "dtBody", Cmd);
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                string tmp_str = dtBody.Rows[i]["ex_2"] + "";
                string[] cdata = tmp_str.Split('+');  //+ 后是 模号
                string cbodybatch = "";
                //箱码
                for (int n = 0; n < FormData.Rows.Count; n++)
                {
                    if (dtBody.Rows[i][rd_definename_to_sn] + "" == FormData.Rows[n]["序列号"] + "")
                    {
                        dtBody.Rows[i]["define31"] = FormData.Rows[n]["箱码"];
                        cbodybatch = FormData.Rows[n]["箱码"] + "";//获得箱码中的日期作为批号的日期
                        string[] bdata = cbodybatch.Split('-');
                        cbodybatch = "";
                        if (bdata.Length > 1 && bdata.Length == 3) cbodybatch = bdata[1];
                        if (bdata.Length > 1 && bdata.Length == 4) cbodybatch = bdata[2];
                        break;
                    }
                }

                if (cbodybatch != "")  //更换批号，原批号为今日的日期
                {
                    dtBody.Rows[i]["cbatch"] = cbodybatch;
                }
                //批号
                if (cdata.Length > 1)
                {
                    string[] data1 = cdata[1].Split('-');
                    dtBody.Rows[i]["cbatch"] = dtBody.Rows[i]["cbatch"] + "+" + data1[0];
                    dtBody.Rows[i]["cmjno"] = data1[0];
                }

            }



            #region //补充报工记录
            //判断是否需要补充报工记录
            if (GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_sn_rd10_create_reportrecord'", Cmd).ToLower().CompareTo("true") == 0)
            {
                DataTable dtForm = new DataTable("dtForm");
                dtForm.Columns.Add("LabelText"); dtForm.Columns.Add("TxtName"); dtForm.Columns.Add("TxtTag"); dtForm.Columns.Add("TxtValue");
                for (int i = 0; i < FormData.Rows.Count; i++)
                {
                    DataTable dtProcess = null;
                    string cRpt_type = "";
                    //获得流转卡信息
                    if (cInType.Split(',')[0].CompareTo("1") == 0)
                    {
                        //合格报工
                        dtProcess = GetSqlDataTable(@"select c.t_card_seq,c.t_opcode,c.t_c_id,c.t_wc_code
                        from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                        inner join " + dbname + @"..T_CC_Cards_process c on b.t_card_no=c.t_card_no
                        where a.t_sn_no='" + FormData.Rows[i]["序列号"] + @"'
                        order by c.t_card_seq", "dtProcess", Cmd);
                        cRpt_type = "合格";
                    }
                    else if (cInType.Split(',')[0].CompareTo("2") == 0)
                    {
                        //报废报工
                        dtProcess = GetSqlDataTable(@"select top 1 c.t_card_seq,c.t_opcode,c.t_c_id,c.t_wc_code
                        from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                        inner join " + dbname + @"..T_CC_Cards_process c on b.t_card_no=c.t_card_no
                        where a.t_sn_no='" + FormData.Rows[i]["序列号"] + @"'
                        order by c.t_card_seq", "dtProcess", Cmd);
                        cRpt_type = "料废";
                    }
                    else
                    {
                        //返修报工
                        dtProcess = GetSqlDataTable(@"select top 1 c.t_card_seq,c.t_opcode,c.t_c_id,c.t_wc_code
                        from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                        inner join " + dbname + @"..T_CC_Cards_process c on b.t_card_no=c.t_card_no
                        where a.t_sn_no='" + FormData.Rows[i]["序列号"] + @"'
                        order by c.t_card_seq", "dtProcess", Cmd);
                        cRpt_type = "返修";
                    }
                    //按工序报工
                    for (int p = 0; p < dtProcess.Rows.Count; p++)
                    {
                        #region //组合报工 记录
                        dtForm.Rows.Clear();
                        dr = dtForm.NewRow();
                        dr["LabelText"] = "报工类别";
                        dr["TxtName"] = "txt_t_report_type";
                        dr["TxtTag"] = cRpt_type;
                        dr["TxtValue"] = cRpt_type;
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        dr["LabelText"] = "序列号条码";
                        dr["TxtName"] = "txt_barcode";
                        dr["TxtTag"] = FormData.Rows[i]["序列号"] + "";
                        dr["TxtValue"] = FormData.Rows[i]["序列号"] + "";
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        dr["LabelText"] = "工序行号";
                        dr["TxtName"] = "txt_flow_seq";
                        dr["TxtTag"] = dtProcess.Rows[p]["t_c_id"] + "";
                        dr["TxtValue"] = dtProcess.Rows[p]["t_card_seq"] + "";
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        dr["LabelText"] = "工序";
                        dr["TxtName"] = "txt_t_opcode";
                        dr["TxtTag"] = dtProcess.Rows[p]["t_opcode"] + "";
                        dr["TxtValue"] = dtProcess.Rows[p]["t_opcode"] + "";
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        dr["LabelText"] = "工作中心";
                        dr["TxtName"] = "txt_t_wc_code";
                        dr["TxtTag"] = dtProcess.Rows[p]["t_wc_code"] + "";
                        dr["TxtValue"] = dtProcess.Rows[p]["t_wc_code"] + "";
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        dr["LabelText"] = "设备号";
                        dr["TxtName"] = "txt_t_eq_no";
                        dr["TxtTag"] = "";
                        dr["TxtValue"] = "";
                        dtForm.Rows.Add(dr);

                        dr = dtForm.NewRow();
                        if (cRpt_type.CompareTo("合格") == 0)
                        {
                            dr["LabelText"] = "合格数";
                            dr["TxtName"] = "txt_t_valid_qty";
                        }
                        else if (cRpt_type.CompareTo("料废") == 0)
                        {
                            dr["LabelText"] = "料废数";
                            dr["TxtName"] = "txt_t_scrap_material";
                        }
                        else
                        {
                            dr["LabelText"] = "返修数";
                            dr["TxtName"] = "txt_t_repair_qty";
                        }

                        dr["TxtTag"] = "1";
                        dr["TxtValue"] = "1";
                        dtForm.Rows.Add(dr);

                        if (p == 0)  //第一道工序
                        {
                            //自定义项2
                            dr = dtForm.NewRow();
                            dr["LabelText"] = "模号";
                            dr["TxtName"] = "txt_define2";
                            dr["TxtTag"] = "";
                            dr["TxtValue"] = "";
                            string cmer_bat = GetDataString("select ex_2 from " + dbname + @"..T_CC_Cards_Sn where t_sn_no='" + FormData.Rows[i]["序列号"] + "'", Cmd);
                            string[] cdata = cmer_bat.Split('+');  //+ 后是 模号
                            if (cdata.Length > 1)
                            {
                                string[] data1 = cdata[1].Split('-');
                                dr["TxtTag"] = data1[0];
                                dr["TxtValue"] = data1[0];
                            }
                            dtForm.Rows.Add(dr);
                        }

                        #endregion
                        //序列号报工
                        dtForm.PrimaryKey = new System.Data.DataColumn[] { dtForm.Columns["TxtName"] };
                        string cRet_Code = Mes_rpt_to_db(dtForm, dbname, cUserName, cLogDate, "", "U81024", false, Cmd);

                        Cmd.CommandText = @"insert into " + dbname + @"..T_CC_pro_report_sn(t_sn_no,t_card_seq,t_opcode,t_process_c_id,t_report_list_id,t_time) 
                        values('" + FormData.Rows[i]["序列号"] + "','" + dtProcess.Rows[p]["t_card_seq"] + "','" + dtProcess.Rows[p]["t_opcode"] + @"',
                            0" + dtProcess.Rows[p]["t_c_id"] + ",0" + cRet_Code.Split(',')[0] + ",getdate())";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = @"update " + dbname + @"..T_CC_Cards_Sn set t_flow_seq='流转' where t_sn_no='" + FormData.Rows[i]["序列号"] + "' and t_flow_seq='创建'";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion



            //入库单保存
            U8StandSCMBarCode u8rd10 = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8rd10.U81015(dtHead, dtBody, dbname, cUserName, cLogDate, "U81015", Cmd);
            string rdid = crd_ret.Split(',')[0];  //入库单ID
            #endregion

            #region //建立序列号入库关系
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) 
                select a.autoid,b.t_card_id,'" + cInType.Split(',')[0] + "' from " + dbname + "..rdrecords10 a inner join " + dbname + "..T_CC_Card_List b on a." + body_define_cardno + @"=b.t_card_no 
                where id=" + rdid + " and a.bRelated=0";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_Rd10(t_autoid,t_card_id,t_ctype,csn) 
                select a.autoid,b.t_card_id,'" + cInType.Split(',')[0] + "'," + body_define_sn + @" 
                from " + dbname + "..rdrecords10 a inner join " + dbname + "..T_CC_Card_List b on a." + body_define_cardno + @"=b.t_card_no 
                where id=" + rdid + " and a.bRelated=0";
            Cmd.ExecuteNonQuery();
            #endregion

            //序列号入库记录
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_cur_seqno='',t_ware_code='" + cwhcode + @"',t_flow_seq='在库',
                    t_cbatch='" + dtBody.Rows[i]["cbatch"] + "' where t_sn_no ='" + dtBody.Rows[i][rd_definename_to_sn] + "'";
                Cmd.ExecuteNonQuery();
            }

            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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

    [WebMethod]  //序列号清单发货*
    public string MES_SN_Disp(System.Data.DataTable FormData, string cwhcode, string dbname, string cUserName, string cLogDate, string SheetID)
    {

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
                ////判断箱码是否存在
                //if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..T_CC_MLX_SN_FH_DZ where x_no='" + FormData.Rows[i]["箱码"] + "'", Cmd) > 0)
                //    throw new Exception("箱码【" + FormData.Rows[i]["箱码"] + "】已经发货");
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            if (cwhcode == "") throw new Exception("请选择仓库");
            #region  //发货
            bool b_only_fh = false;
            string dips_only_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_sn_only_disp'", Cmd);
            if (dips_only_sn.ToLower().CompareTo("true") == 0) b_only_fh = true; //只控制发货，其他情况不控制
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (b_only_fh)
                {
                    //只需要发货
                    if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("销售") == 0)
                        throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]属于已销售状态");
                }
                else
                {
                    //严格按照流程控制
                    if (cwhcode.CompareTo(dtSnChk.Rows[l]["t_ware_code"] + "") != 0)
                        throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]仓库发生变化，被别人处理,请放弃重新扫描");
                    if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("在库") != 0)
                        throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]非在库状态");
                }
            }

            //序列号 写入 入库单表体自定义项：rd_definename_to_sn
            string dips_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_disp_sn_body'", Cmd);
            if (dips_definename_to_sn == "") throw new Exception("请设置：[序列号 存放发货单表体自定义项栏目]");

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);
            #endregion

            DataTable dtBody = new DataTable("dtBody");
            #region   //组合表体栏目
            //dips_definename_to_sn  代表发货单的 存放 序列号的 自定义项
            dtBody.Columns.Add("isosid"); dtBody.Columns.Add("cbatch"); dtBody.Columns.Add("cinvcode"); dtBody.Columns.Add("iquantity");
            dtBody.Columns.Add(dips_definename_to_sn); dtBody.Columns.Add("cposcode");
            dtBody.Columns.Add("cdefine31"); dtBody.Columns.Add("cdefine32"); dtBody.Columns.Add("cdefine33");

            string _cbatno = GetDataString("select newid()", Cmd);
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                //校验存货编码是否一致项
                string c_so_inv = GetDataString("select cinvcode from " + dbname + "..SO_SODetails where iSOsID=0" + FormData.Rows[i]["isosid"], Cmd);
                if (FormData.Rows[i]["产品编码"] + "" != c_so_inv)
                {
                    throw new Exception("当前箱码对应产品[" + FormData.Rows[i]["产品编码"] + "]与订单的产品编码[" + c_so_inv + "]不一致");
                }

                #region  //汇总:临时存储  1
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_MLX_SN_FH_DZ(isosid,cbatch,cinvcode,iquantity,csn,x_no,asn_no,bat_no) 
                    values('" + FormData.Rows[i]["isosid"] + "','" + FormData.Rows[i]["t_cbatch"] + "','" + FormData.Rows[i]["产品编码"] + "',1,'" + FormData.Rows[i]["序列号"] + @"',
                        '" + FormData.Rows[i]["箱码"] + "','" + FormData.Rows[i]["ASN标签"] + "','" + _cbatno + "')";
                Cmd.ExecuteNonQuery();
                #endregion
                //原 未 汇总 代码备份
                //dr = dtBody.NewRow();
                //dr["isosid"] = FormData.Rows[i]["isosid"] + "";
                //dr["cbatch"] = FormData.Rows[i]["t_cbatch"] + "";
                //dr["cinvcode"] = FormData.Rows[i]["产品编码"] + "";
                //dr["iquantity"] = "1";
                //dr[dips_definename_to_sn] = FormData.Rows[i]["序列号"] + "";
                //dr["cdefine31"] = FormData.Rows[i]["箱码"];
                //dr["cdefine32"] = FormData.Rows[i]["ASN标签"];
                //dr["cdefine33"] = GetDataString("select cDefine10 from " + dbname + "..SO_SOMain where id in (select id from " + dbname + "..SO_SODetails where iSOsID=0" + dr["isosid"] + ")", Cmd);
                //dtBody.Rows.Add(dr);
            }

            #region  //汇总:临时存储  2
            DataTable dtDispHz = GetSqlDataTable("select isosid,cbatch,cinvcode,sum(iquantity) iquantity,bat_no from " + dbname + @"..T_CC_MLX_SN_FH_DZ 
                where bat_no='" + _cbatno + "' group by isosid,cbatch,cinvcode,bat_no", "dtDispHz", Cmd);
            for (int i = 0; i < dtDispHz.Rows.Count; i++)
            {
                dr = dtBody.NewRow();
                dr["isosid"] = dtDispHz.Rows[i]["isosid"] + "";
                dr["cbatch"] = dtDispHz.Rows[i]["cbatch"] + "";
                dr["cinvcode"] = dtDispHz.Rows[i]["cinvcode"] + "";
                dr["iquantity"] = dtDispHz.Rows[i]["iquantity"] + "";
                dr[dips_definename_to_sn] = dtDispHz.Rows[i]["bat_no"] + "";
                dr["cdefine33"] = GetDataString("select cDefine10 from " + dbname + "..SO_SOMain where id in (select id from " + dbname + "..SO_SODetails where iSOsID=0" + dr["isosid"] + ")", Cmd);
                dtBody.Rows.Add(dr);
            }
            #endregion


            #endregion

            //发货单保存
            U8StandSCMBarCode u8bcode = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8bcode.U81020(dtHead, dtBody, dbname, cUserName, cLogDate, "U81020", Cmd);
            string dlid = crd_ret.Split(',')[0];  //发货单ID
            #endregion

            #region //建立序列号发货单关系
            //新  发货单与序列号对照表
            DataTable dtDispdetail = GetSqlDataTable("select min(iDLsID) iDLsID,isosid,cinvcode from " + dbname + @"..DispatchLists
                where dlid=0" + dlid + " group by isosid,cinvcode", "dtDispdetail", Cmd);
            for (int i = 0; i < dtDispdetail.Rows.Count; i++)
            {
                //cbatch='" + dtDispdetail.Rows[i]["cbatch"] + @"' 
                Cmd.CommandText = "update " + dbname + @"..T_CC_MLX_SN_FH_DZ set t_iDLsID=0" + dtDispdetail.Rows[i]["iDLsID"] + @" 
                    where bat_no='" + _cbatno + "' and isosid=0" + dtDispdetail.Rows[i]["isosid"] + @" 
                        and cinvcode='" + dtDispdetail.Rows[i]["cinvcode"] + "'";
                Cmd.ExecuteNonQuery();
            }

            //原 发货单与序列号关系
            //            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_Dips(t_idlsid,csn) 
            //                select a.iDLsID," + dips_definename_to_sn + @" from " + dbname + "..DispatchLists a where dlid=" + dlid;
            //            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_Dips(t_idlsid,csn) 
                select a.t_iDLsID,csn from " + dbname + "..T_CC_MLX_SN_FH_DZ a where bat_no='" + _cbatno + "'";
            Cmd.ExecuteNonQuery();
            #endregion

            //序列号入库记录
            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_ware_code='',t_flow_seq='销售' where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();


            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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

    [WebMethod]  //序列号清单退货*
    public string MES_SN_Dis_Tui(System.Data.DataTable FormData, string cwhcode, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            if (cwhcode == "") throw new Exception("请选择仓库");
            #region  //退货
            //判断序列号是否已经发货
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("销售") != 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]非销售状态");
            }

            //序列号 写入 退货单表体自定义项：rd_definename_to_sn
            string dips_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_disp_sn_body'", Cmd);
            if (dips_definename_to_sn == "") throw new Exception("请设置：[序列号 存放发货单表体自定义项栏目]");

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);
            //获得仓库客户等信息
            DataTable dtOldDispMainInfo = GetSqlDataTable(@"select c.cBusType,c.cCusCode,max(c.cSTCode) cSTCode,max(c.cDepCode) cDepCode 
                from " + dbname + "..T_CC_FlowCard_SN_Dips a inner join " + dbname + @"..DispatchLists b on a.t_idlsid=b.iDLsID
	                inner join " + dbname + @"..DispatchList c on b.DLID=c.DLID
                where a.csn in(" + sn_list + @") and b.iQuantity>0
                group by c.cBusType,c.cCusCode", "dtOldDispMainInfo", Cmd);
            if (dtOldDispMainInfo.Rows.Count == 0) throw new Exception("未找到以前销售信息");
            if (dtOldDispMainInfo.Rows.Count > 1) throw new Exception("本次扫描的序列号的客户信息和业务类型不一致");

            dr = dtHead.NewRow();
            dr["LabelText"] = "业务类型";
            dr["TxtName"] = "txt_cbustype";
            dr["TxtTag"] = "" + dtOldDispMainInfo.Rows[0]["cBusType"];
            dr["TxtValue"] = "" + dtOldDispMainInfo.Rows[0]["cBusType"];
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "客户";
            dr["TxtName"] = "txt_ccuscode";
            dr["TxtTag"] = "" + dtOldDispMainInfo.Rows[0]["cCusCode"];
            dr["TxtValue"] = "" + dtOldDispMainInfo.Rows[0]["cCusCode"];
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "部门";
            dr["TxtName"] = "txt_cdepcode";
            dr["TxtTag"] = "" + dtOldDispMainInfo.Rows[0]["cDepCode"];
            dr["TxtValue"] = "" + dtOldDispMainInfo.Rows[0]["cDepCode"];
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "销售类型";
            dr["TxtName"] = "txt_cstcode";
            dr["TxtTag"] = "" + dtOldDispMainInfo.Rows[0]["cSTCode"];
            dr["TxtValue"] = "" + dtOldDispMainInfo.Rows[0]["cSTCode"];
            dtHead.Rows.Add(dr);
            #endregion

            DataTable dtBody = new DataTable("dtBody");
            #region   //组合表体栏目
            //dips_definename_to_sn  代表发货单的 存放 序列号的 自定义项
            dtBody.Columns.Add("isosid"); dtBody.Columns.Add("cbatch"); dtBody.Columns.Add("cinvcode"); dtBody.Columns.Add("iquantity");
            dtBody.Columns.Add(dips_definename_to_sn); dtBody.Columns.Add("cposcode");
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                string c_so_invcode = GetDataString("select cinvcode from " + dbname + "..SO_SODetails where isosid=0" + FormData.Rows[i]["isosid"], Cmd);
                if (c_so_invcode.CompareTo(FormData.Rows[i]["产品编码"] + "") != 0)
                    throw new Exception("销售订单编码[" + c_so_invcode + "]与序列号[" + FormData.Rows[i]["序列号"] + "]的产品不一致");
                dr = dtBody.NewRow();
                dr["isosid"] = "" + FormData.Rows[i]["isosid"];
                dr["cbatch"] = FormData.Rows[i]["批号"] + "";
                dr["cinvcode"] = FormData.Rows[i]["产品编码"] + "";
                dr["iquantity"] = "-1";
                dr[dips_definename_to_sn] = FormData.Rows[i]["序列号"] + "";
                dtBody.Rows.Add(dr);
            }
            #endregion

            //发货单保存
            U8StandSCMBarCode u8bcode = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8bcode.U81020(dtHead, dtBody, dbname, cUserName, cLogDate, "U81020", Cmd);
            string dlid = crd_ret.Split(',')[0];  //发货单ID
            #endregion

            #region //建立序列号发货单关系
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_Dips(t_idlsid,csn) 
                select a.iDLsID," + dips_definename_to_sn + @" from " + dbname + "..DispatchLists a where dlid=" + dlid;
            Cmd.ExecuteNonQuery();
            #endregion

            //序列号入库记录
            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_ware_code='" + cwhcode + "',t_flow_seq='在库' where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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


    [WebMethod]  //获得序列号详细信息(在库)
    public DataTable MES_SN_RD32_Info(string sn, string dbname, string ccus_code, string dlcode)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        try
        {
            SqlCommand Cmd = Conn.CreateCommand();
            DataTable dtSn = null;
            string dips_only_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_sn_only_disp'", Cmd);
            string cSnBatchNo = "a.t_cbatch";  //批号来源规则
            cSnBatchNo = "'' t_cbatch"; //只控制产品是否发货状态，其他情况不管

            //判断条码是否箱码
            if (GetDataInt("select COUNT(*) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "'", Cmd) == 0)
                throw new Exception("当前箱码无法找到");
            //获得箱码内产品数量
            int i_box_all = GetDataInt("select count(distinct csn) from " + dbname + @"..T_CC_SN_Box_DZ where boxcode='" + sn + "' and isnull(BoxState,'有效')='有效'", Cmd);
            //箱码
            dtSn = GetSqlDataTable(@"select distinct a.t_sn_no,c.boxcode,b.t_invcode,'' cstate,a.t_ware_code,w.cwhname," + cSnBatchNo + @",m.cCusInvCode
                from " + dbname + @"..T_CC_Cards_Sn a inner join " + dbname + @"..T_CC_Card_List b on a.t_card_id=b.t_card_id
                inner join " + dbname + @"..T_CC_SN_Box_DZ c on a.t_sn_no=c.csn and isnull(c.BoxState,'有效')='有效'
                left join " + dbname + @"..warehouse w on a.t_ware_code=w.cwhcode
                left join " + dbname + @"..CusInvContrapose m on b.t_invcode=m.cInvCode and m.ccuscode='" + ccus_code + @"'
                where c.boxcode='" + sn + "'", "dtSn", Cmd);

            if (dtSn.Rows.Count == 0) throw new Exception("[" + sn + "]箱码下无有效单件码");
            if (i_box_all != dtSn.Rows.Count) throw new Exception("[" + sn + "]箱码存在部分序列号不可发货状态");
            //判断箱码的产品与发货的产品编码是否一致
            DataTable dtInv = GetSqlDataTable(@"select distinct cinvcode from " + dbname + @"..DispatchList a 
                inner join " + dbname + @"..DispatchLists b on a.DLID=b.DLID where cdlcode='" + dlcode + "' ", "dtInv", Cmd);
            for (int i = 0; i < dtSn.Rows.Count; i++)
            {
                string c_invcode = "" + dtSn.Rows[i]["t_invcode"];
                if (dtInv.Select("cinvcode='" + c_invcode + "'").Length == 0) throw new Exception("序列号[" + c_invcode + "]的产品编码与发货单的产品不一致");
            }
            return dtSn;
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

    [WebMethod]  //序列号清单销售出库*
    public string MES_SN_RD32(System.Data.DataTable FormData, string dlcode, string cwhcode, string dbname, string cUserName, string cLogDate, string SheetID)
    {

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);
            if (cwhcode == "") throw new Exception("请选择仓库");
            //判断仓库是否发货单的仓库
            if (GetDataInt("select count(*) from " + dbname + "..DispatchList a inner join " + dbname + @"..DispatchLists b on a.DLID=b.DLID 
                where a.cdlcode='" + dlcode + "' and b.cwhcode='" + cwhcode + "'", Cmd) == 0)
            {
                throw new Exception("扫码界面的仓库[" + cwhcode + "],不是发货单的仓库");
            }
            //判断发货数量与扫码数量一致性
            int iFHAll = GetDataInt("select cast(isnull(sum(iQuantity),0) as int) from " + dbname + "..DispatchList a inner join " + dbname + @"..DispatchLists b on a.DLID=b.DLID 
                where a.cdlcode='" + dlcode + "' and b.cwhcode='" + cwhcode + "'", Cmd);
            if (iFHAll != FormData.Rows.Count)
            {
                throw new Exception("仓库[" + cwhcode + "]扫码的数量[" + FormData.Rows.Count + "]与要求的此仓库的要求发货数量[" + iFHAll + "]不一致");
            }


            #region  //发货
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("销售") == 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]属于已销售状态");
            }

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);
            #endregion

            string dips_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_disp_sn_body'", Cmd);
            if (dips_definename_to_sn == "") throw new Exception("请设置：[序列号 存放发货单表体自定义项栏目]");
            DataTable dtFH = GetSqlDataTable(@"select idlsid,cinvcode,cbatch,cast(iquantity as int) iquantity,cPosition cposcode,cdefine31,'' cdefine32,'' cdefine33
                from " + dbname + @"..DispatchList a inner join " + dbname + @"..DispatchLists b on a.DLID=b.DLID 
                where a.cdlcode='" + dlcode + "' and b.cwhcode='" + cwhcode + "'", "dtBody", Cmd);

            #region   //组合表体栏目

            string _cbatno = GetDataString("select newid()", Cmd);
            int i_form_index = 0; //序列号datatable的行号
            for (int r = 0; r < dtFH.Rows.Count; r++)
            {
                int i_qty_fh = int.Parse("" + dtFH.Rows[r]["iquantity"]);

                #region  //汇总:临时存储  1
                for (int i = 0; i < i_qty_fh; i++)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..T_CC_MLX_SN_RD32_DZ(idlsid,cbatch,cinvcode,iquantity,csn,x_no,asn_no,bat_no) 
                    values('" + dtFH.Rows[r]["idlsid"] + "','" + FormData.Rows[i_form_index]["t_cbatch"] + "','" + FormData.Rows[i_form_index]["产品编码"] + @"',1,
                        '" + FormData.Rows[i_form_index]["序列号"] + @"','" + FormData.Rows[i_form_index]["箱码"] + "','" + dtFH.Rows[r]["cdefine31"] + "','" + _cbatno + "')";
                    Cmd.ExecuteNonQuery();
                    i_form_index++;
                }
                #endregion
            }

            DataTable dtBody = dtFH.Clone();
            #region  //汇总:临时存储  2

            DataTable dtDispHz = GetSqlDataTable("select idlsid,cbatch,cinvcode,sum(iquantity) iquantity,bat_no,asn_no from " + dbname + @"..T_CC_MLX_SN_RD32_DZ 
                where bat_no='" + _cbatno + "' group by idlsid,cbatch,cinvcode,asn_no,bat_no", "dtDispHz", Cmd);
            for (int i = 0; i < dtDispHz.Rows.Count; i++)
            {
                dr = dtBody.NewRow();
                dr["idlsid"] = dtDispHz.Rows[i]["idlsid"] + "";
                dr["cbatch"] = dtDispHz.Rows[i]["cbatch"] + "";
                dr["cinvcode"] = dtDispHz.Rows[i]["cinvcode"] + "";
                dr["iquantity"] = dtDispHz.Rows[i]["iquantity"] + "";
                dr["cdefine31"] = dtDispHz.Rows[i]["asn_no"] + "";
                //dr["cdefine32"] = dtDispHz.Rows[i]["bat_no"] + "";
                dtBody.Rows.Add(dr);
            }
            #endregion


            #endregion

            //销售出库单保存
            U8StandSCMBarCode u8bcode = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8bcode.U81021_1(dtHead, dtBody, dbname, cUserName, cLogDate, "U81021", true, Cmd);
            string dlid = crd_ret.Split(',')[0];  //发货单ID

            #endregion

            #region //建立序列号发货单关系
            //销售出库单与序列号对照表   (注意：批号不控制，自动先进先出)
            DataTable dtDispdetail = GetSqlDataTable("select min(autoid) autoid,idlsid,cinvcode,'' cbatch,cdefine31 from " + dbname + @"..rdrecords32
                where id=0" + dlid + " group by idlsid,cinvcode,cbatch,cdefine31", "dtDispdetail", Cmd);
            for (int i = 0; i < dtDispdetail.Rows.Count; i++)
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_MLX_SN_RD32_DZ set t_rd32autoid=0" + dtDispdetail.Rows[i]["autoid"] + @" 
                    where bat_no='" + _cbatno + "' and idlsid=0" + dtDispdetail.Rows[i]["idlsid"] + " and cinvcode='" + dtDispdetail.Rows[i]["cinvcode"] + @"' 
                        and isnull(cbatch,'')='" + dtDispdetail.Rows[i]["cbatch"] + "' and asn_no='" + dtDispdetail.Rows[i]["cdefine31"] + "' ";
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //序列号状态调整
            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_ware_code='',t_flow_seq='销售' where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();


            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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




    [WebMethod]  //序列号清单调拨*
    public string MES_SN_DB(System.Data.DataTable FormData, string cwhcode, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (cwhcode == "") throw new Exception("请选择仓库");
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            string c_db_outware = FormData.Rows[0]["仓库编码"] + "";
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (c_db_outware.CompareTo(dtSnChk.Rows[l]["t_ware_code"] + "") != 0)
                    throw new Exception("序列号仓库发生变化，被别人处理,请放弃重新扫描");
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("在库") != 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]非在库状态");
            }

            #region  //调拨
            //序列号 写入 调拨单 表体自定义项：rd_definename_to_sn
            string dips_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_db_sn_body'", Cmd);
            if (dips_definename_to_sn == "") throw new Exception("请设置：[序列号 存放调拨单表体自定义项栏目]");

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "调入仓库";
            dr["TxtName"] = "txt_cinwhcode";
            dr["TxtTag"] = cwhcode;
            dr["TxtValue"] = cwhcode;
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "调出仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = FormData.Rows[0]["仓库编码"] + "";
            dr["TxtValue"] = FormData.Rows[0]["仓库编码"] + "";
            dtHead.Rows.Add(dr);

            //出库类别
            dr = dtHead.NewRow();
            dr["LabelText"] = "出库类别";
            dr["TxtName"] = "txt_crdcode";
            dr["TxtTag"] = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='crdcode'", Cmd);
            dr["TxtValue"] = GetDataString("select text_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='crdcode'", Cmd);
            dtHead.Rows.Add(dr);

            //入库类别
            dr = dtHead.NewRow();
            dr["LabelText"] = "入库类别";
            dr["TxtName"] = "txt_cinrdcode";
            dr["TxtTag"] = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cinrdcode'", Cmd);
            dr["TxtValue"] = GetDataString("select text_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cinrdcode'", Cmd);
            dtHead.Rows.Add(dr);

            //出库部门
            dr = dtHead.NewRow();
            dr["LabelText"] = "调出部门";
            dr["TxtName"] = "txt_cdepcode";
            dr["TxtTag"] = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cdepcode'", Cmd);
            dr["TxtValue"] = GetDataString("select text_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cdepcode'", Cmd);
            dtHead.Rows.Add(dr);

            //入库部门
            dr = dtHead.NewRow();
            dr["LabelText"] = "调入部门";
            dr["TxtName"] = "txt_cindepcode";
            dr["TxtTag"] = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cindepcode'", Cmd);
            dr["TxtValue"] = GetDataString("select text_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81017' and t_fieldname='cindepcode'", Cmd);
            dtHead.Rows.Add(dr);

            #endregion

            DataTable dtBody = new DataTable("dtBody");
            #region   //组合表体栏目
            //dips_definename_to_sn  代表发货单的 存放 序列号的 自定义项
            dtBody.Columns.Add("isosid"); dtBody.Columns.Add("cbatch"); dtBody.Columns.Add("cinvcode"); dtBody.Columns.Add("iquantity");
            dtBody.Columns.Add(dips_definename_to_sn); dtBody.Columns.Add("cposcode");
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                dr = dtBody.NewRow();
                dr["isosid"] = "";
                dr["cbatch"] = FormData.Rows[i]["批号"] + "";
                dr["cinvcode"] = FormData.Rows[i]["产品编码"] + "";
                dr["iquantity"] = "1";
                dr[dips_definename_to_sn] = FormData.Rows[i]["序列号"] + "";
                dtBody.Rows.Add(dr);
            }
            #endregion

            //发货单保存
            U8StandSCMBarCode u8bcode = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8bcode.U81017(dtHead, dtBody, dbname, cUserName, cLogDate, "U81017", Cmd);
            string ID = crd_ret.Split(',')[0];  //发货单ID
            #endregion

            #region //建立序列号调拨单关系
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_DB(t_autoid,csn) 
                select a.autoid," + dips_definename_to_sn + @" from " + dbname + "..TransVouchs a where id=" + ID;
            Cmd.ExecuteNonQuery();
            #endregion

            //序列号入库记录
            Cmd.CommandText = "update " + dbname + "..T_CC_Cards_Sn set t_ware_code='" + cwhcode + "' where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();


            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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

    [WebMethod]  //序列号清单领用*
    public string MES_SN_MerOut(System.Data.DataTable FormData, string pfcard, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            if (pfcard == "") throw new Exception("请输入流转卡信息");
            string pf_new_id = GetDataString("select t_card_id from " + dbname + @"..T_CC_Card_List where t_card_no='" + pfcard + "'", Cmd);
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq,t_card_id from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            string c_db_outware = FormData.Rows[0]["仓库编码"] + "";
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (c_db_outware.CompareTo(dtSnChk.Rows[l]["t_ware_code"] + "") != 0)
                    throw new Exception("序列号仓库发生变化，被别人处理,请放弃重新扫描");
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("在库") != 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]非在库状态");
                if (("" + dtSnChk.Rows[l]["t_card_id"]).CompareTo(pf_new_id) == 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]新旧卡相同");
            }


            #region  //领用
            //序列号 写入 调拨单 表体自定义项：rd_definename_to_sn
            string dips_definename_to_sn = "" + GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd11_sn_body'", Cmd);
            if (dips_definename_to_sn == "") throw new Exception("请设置：[序列号 存放材料出库表体自定义项栏目]");
            string rd11_bodydefinename = dips_definename_to_sn;
            dips_definename_to_sn = dips_definename_to_sn.Replace("cdefine", "define");

            DataTable dtHead = new DataTable("dtHead");
            #region  //组合表头数据
            string crdcode = GetDataString("select tag_value from " + dbname + @"..T_CC_Form_Field_Default where SheetID='U81016' and t_fieldname='crdcode'", Cmd);
            if (crdcode == "") throw new Exception("请设置模板[U81016] 的入库类别的缺省值");

            dtHead.Columns.Add("LabelText"); dtHead.Columns.Add("TxtName"); dtHead.Columns.Add("TxtTag"); dtHead.Columns.Add("TxtValue");
            DataTable dtCardInfo = GetSqlDataTable(@"select t_modid,b.MDeptCode,a.t_invcode from " + dbname + "..T_CC_Card_List a inner join " + dbname + @"..mom_orderdetail b on a.t_modid=b.MoDId 
                where t_card_no='" + pfcard + "'", "dtCardInfo", Cmd);
            if (dtCardInfo.Rows.Count == 0) throw new Exception("流转卡信息不存在");
            DataRow dr = dtHead.NewRow();
            dr["LabelText"] = "部门";
            dr["TxtName"] = "txt_cdepcode";
            dr["TxtTag"] = "" + dtCardInfo.Rows[0]["MDeptCode"];
            dr["TxtValue"] = "" + dtCardInfo.Rows[0]["MDeptCode"];
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "仓库";
            dr["TxtName"] = "txt_cwhcode";
            dr["TxtTag"] = FormData.Rows[0]["仓库编码"] + "";
            dr["TxtValue"] = FormData.Rows[0]["仓库编码"] + "";
            dtHead.Rows.Add(dr);

            dr = dtHead.NewRow();
            dr["LabelText"] = "出库类别";
            dr["TxtName"] = "txt_crdcode";
            dr["TxtTag"] = crdcode;
            dr["TxtValue"] = crdcode;
            dtHead.Rows.Add(dr);
            #endregion

            DataTable dtBody = new DataTable("dtBody");
            #region   //组合表体栏目
            //dips_definename_to_sn  代表发货单的 存放 序列号的 自定义项  
            dtBody.Columns.Add("allocateid"); dtBody.Columns.Add("cbatch"); dtBody.Columns.Add("cinvcode"); dtBody.Columns.Add("modid");
            dtBody.Columns.Add("iquantity"); dtBody.Columns.Add(dips_definename_to_sn); dtBody.Columns.Add("cposcode");

            //产品编码一致性检查
            string c_card_invcode = dtCardInfo.Rows[0]["t_invcode"] + "";
            //获得子件 用料ID
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                if (c_card_invcode.CompareTo(FormData.Rows[i]["产品编码"] + "") != 0)
                    throw new Exception("序列号[" + FormData.Rows[i]["序列号"] + "]的产品与流转卡的产品[" + FormData.Rows[i]["产品编码"] + "]不一致");


                dr = dtBody.NewRow();
                dr["allocateid"] = GetDataString("select allocateid from " + dbname + @"..mom_moallocate 
                    where MoDId=" + dtCardInfo.Rows[0]["t_modid"] + " and InvCode='" + FormData.Rows[i]["产品编码"] + "' and ByproductFlag=0", Cmd);
                dr["cbatch"] = FormData.Rows[i]["t_cbatch"] + "";
                dr["cinvcode"] = FormData.Rows[i]["产品编码"] + "";
                dr["iquantity"] = "1";
                dr["modid"] = dtCardInfo.Rows[0]["t_modid"] + "";
                dr[dips_definename_to_sn] = FormData.Rows[i]["序列号"] + "";

                if (dr["allocateid"] + "" == "") throw new Exception("存货[" + FormData.Rows[i]["产品编码"] + "]不属于流转卡[" + pfcard + "]的子件");
                dtBody.Rows.Add(dr);
            }
            #endregion

            //发货单保存
            U8StandSCMBarCode u8bcode = new U8StandSCMBarCode();
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };
            string crd_ret = u8bcode.U81016(dtHead, dtBody, dbname, cUserName, cLogDate, "U81016", Cmd);
            string ID = crd_ret.Split(',')[0];  //发货单ID
            #endregion

            #region //建立序列号材料出库单关系
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_FlowCard_SN_Mer(t_autoid,csn) 
                select a.autoid," + rd11_bodydefinename + @" from " + dbname + "..rdrecords11 a where id=" + ID;
            Cmd.ExecuteNonQuery();
            #endregion

            //序列号入库记录
            Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_Sn set t_card_old_id=t_card_id where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "update " + dbname + @"..T_CC_Cards_Sn set t_ware_code='',t_flow_seq='流转',t_card_id=0" + pf_new_id + @",t_sn_modify_count=isnull(t_sn_modify_count,0)+1 
                where t_sn_no in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();
            //清除原装箱信息
            Cmd.CommandText = "update " + dbname + "..T_CC_SN_Box_DZ set BoxState='无效' where csn in(" + sn_list + ")";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return crd_ret.Split(',')[1];
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

    [WebMethod]  //序列号在库装箱检查*
    public bool MES_SN_BoxCheck(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //组合入库样式（标准产品发货）
            string sn_list = "";
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                sn_list += ",'" + FormData.Rows[i]["序列号"] + "'";
            }
            if (sn_list.Length == 0) throw new Exception("没有传入序列号信息");
            sn_list = sn_list.Substring(1);

            #region  //检查
            //判断当前序列号状态和仓库新
            DataTable dtSnChk = GetSqlDataTable("select t_sn_no, t_ware_code,t_flow_seq from " + dbname + @"..T_CC_Cards_Sn with(rowlock) where t_sn_no in(" + sn_list + ")", "dtSnChk", Cmd);
            for (int l = 0; l < dtSnChk.Rows.Count; l++)
            {
                if (("" + dtSnChk.Rows[l]["t_flow_seq"]).CompareTo("在库") != 0)
                    throw new Exception("序列号[" + dtSnChk.Rows[l]["t_sn_no"] + "]非在库状态");
            }
            #endregion

            #region //建立检查记录
            for (int i = 0; i < FormData.Rows.Count; i++)
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_SN_Box_Check(csn,boxcode,cResult,cmaker,cmaketime) 
                    values('" + FormData.Rows[i]["序列号"] + "','" + FormData.Rows[i]["箱码"] + "','OK','" + cUserName + "',getdate())";
                Cmd.ExecuteNonQuery();
            }
            #endregion


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

    #region  //U8 车间生产制造模块改造 对应功能    ****************************************************************************************
    [WebMethod]  //U8 扫描流转卡 报工
    public string U8FC_Report(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string errmsg = "";
            string[] txtdata = null;  //临时取数
            System.Data.DataTable dtOpData = null;  //本道工序情况
            System.Data.DataTable dtNextData = null;//下道工序情况
            string cacc_id = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
            string modid = "";
            string pfdid = "";  //本工序流转卡明细ID
            string pf_nextdid = "";//下工序流转卡明细ID
            string pf_fre_did = "";//上工序流转卡明细ID
            string pfid = "";
            string PsnCode = ""; //员工代码
            string scrapreasoncode = "";//报废原因
            string refusedreasoncode = "";//拒绝原因
            string qualifiedreasoncode = "";//完工原因
            string eqid = "";//设备
            string ShiftId = ""; //班组

            bool b_qty_control = true;  //本工序是否数量管控,默认情况需要管控
            bool b_write_valid_qty = false; //要求写本工序的合格数（末道工序和外协工序的前一道工序）
            string i_qualified_qty = "";  //合格数
            string i_scrap_qty = "";//废品数
            string i_refused_qty = "";//返修数
            //U8版本信息
            float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));

            #region  //逻辑检验
            txtdata = GetTextsFrom_FormData(FormData, "txt_opseq");
            if (txtdata == null) { throw new Exception("模板设置：工序行号 栏目必须设置成可视栏目"); }
            if (txtdata[2].CompareTo("") == 0) { throw new Exception("请扫描流转卡条码"); }

            dtOpData = GetSqlDataTable(@"select b.pfdid,a.pfid,c.moid,a.modid,a.doccode,d.mocode,b.opseq,e.opcode,f.wcid,cast(f.FirstFlag as int) ifirst,
                    cast(f.LastFlag as int) ilast,cast(a.qty as float) flow_qty,c.invcode,c.MoLotCode,c.MDeptCode,isnull(b.BalMachiningQty,0) valid_qty,
                    b.MoRoutingDId,f.MoRoutingId
                from " + dbname + @"..sfc_processflow a inner join " + dbname + @"..sfc_processflowdetail b on a.pfid=b.pfid
                inner join " + dbname + @"..mom_orderdetail c on a.modid=c.modid 
                inner join " + dbname + @"..mom_order d on c.MoId=d.MoId 
                inner join " + dbname + @"..sfc_operation e on b.OperationId=e.OperationId
                inner join " + dbname + @"..sfc_moroutingdetail f on b.MoRoutingDId=f.MoRoutingDId
                where b.pfdid='" + txtdata[2] + "'", "dtOpData", Cmd);
            if (dtOpData.Rows.Count == 0) throw new Exception("条码不存在");
            modid = dtOpData.Rows[0]["modid"] + "";
            pfdid = dtOpData.Rows[0]["pfdid"] + "";
            pfid = dtOpData.Rows[0]["pfid"] + "";

            //人员
            txtdata = GetTextsFrom_FormData(FormData, "txt_employcode");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                PsnCode = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..hr_hi_person where cPsn_Num='" + PsnCode + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //报废原因
            txtdata = GetTextsFrom_FormData(FormData, "txt_scrapreasoncode");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                scrapreasoncode = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..Reason where ireasontype=14 and creasoncode='" + txtdata[2] + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //拒绝原因
            txtdata = GetTextsFrom_FormData(FormData, "txt_refusedreasoncode");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                refusedreasoncode = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..Reason where ireasontype=14 and creasoncode='" + txtdata[2] + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //完成原因
            txtdata = GetTextsFrom_FormData(FormData, "txt_qualifiedreasoncode");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                qualifiedreasoncode = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..Reason where ireasontype=14 and creasoncode='" + txtdata[2] + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //设备ID
            txtdata = GetTextsFrom_FormData(FormData, "txt_eqid");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                eqid = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..EQ_EQData where autoid='" + txtdata[2] + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //班组
            txtdata = GetTextsFrom_FormData(FormData, "txt_dutyclasscode");
            if (txtdata != null && txtdata[2].CompareTo("") != 0)
            {
                ShiftId = txtdata[2];
                if (int.Parse(GetDataString("select count(*) from " + dbname + "..hr_tm_DutyClass where cCode='" + txtdata[2] + "'", Cmd)) == 0)
                {
                    throw new Exception(txtdata[0] + " 录入不正确");
                }
            }

            //自定义项 tag值不为空，但Text为空 的情况判定
            System.Data.DataTable dtDefineCheck = GetSqlDataTable("select t_fieldname from " + dbname + "..T_CC_Base_GridCol_rule where SheetID='" + SheetID + @"' 
                    and (t_fieldname like '%define%' or t_fieldname like '%free%')", "dtDefineCheck", Cmd);
            for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
            {
                string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
                txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
                if (txtdata == null) continue;

                if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
                {
                    throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
                }
            }

            //检查单据必录项
            System.Data.DataTable dtMustInputCol = GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                    and isnull(t_must_input,0)=1 and UserID=''", "dtMustInputCol", Cmd);
            for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
            {
                string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
                txtdata = GetTextsFrom_FormData(FormData, "txt_" + cc_colname);
                if (txtdata == null) throw new Exception(dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项，模板必须设置成可视栏目");

                if (txtdata[3].CompareTo("") == 0 && txtdata[2].CompareTo("") != 0)  //
                {
                    throw new Exception(txtdata[0] + "录入不正确 录入键值和显示值不匹配");
                }
                if (txtdata[3].CompareTo("") == 0)  //
                {
                    throw new Exception(txtdata[0] + "为必录项,不能为空");
                }
            }
            #endregion

            #region  //流转卡专属  上下逻辑关系检查
            string cFirstOpSeqControl = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='cFirstOpSeqControl'", Cmd); //首道工序是否控制数量
            string cLastOpSeqControl = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='cLastOpSeqControl'", Cmd);  //末道工序是否控制数量

            dtNextData = GetSqlDataTable(@"select top 1 pfdid,OpSeq,WcId,OperationId,MoRoutingDId,cast(SubFlag as int) iSubFlag from " + dbname + @"..sfc_processflowdetail 
                where pfid=" + pfid + " and OpSeq>'" + dtOpData.Rows[0]["opseq"] + "' order by OpSeq", "dtNextData", Cmd);
            if (dtNextData.Rows.Count > 0)
                pf_nextdid = dtNextData.Rows[0]["pfdid"] + "";  //下道工序 明细ID

            if (int.Parse(dtOpData.Rows[0]["ifirst"] + "") == 1 && cFirstOpSeqControl.CompareTo("control") != 0) b_qty_control = false;
            if (int.Parse(dtOpData.Rows[0]["ilast"] + "") == 1 || dtNextData.Rows.Count == 0)  //末道工序
            {
                if (cLastOpSeqControl.CompareTo("control") != 0) b_qty_control = false;
                b_write_valid_qty = true;
            }
            else //判断下到工序是否为外协工序
            {
                if (int.Parse(dtNextData.Rows[0]["iSubFlag"] + "") == 1) b_write_valid_qty = true; //外协工序需要些 合格数
            }

            //若不管控数量，上工序必须有报工记录
            if (int.Parse(dtOpData.Rows[0]["ifirst"] + "") != 1 && !b_qty_control)
            {
                pf_fre_did = GetDataString("select top 1 pfdid from " + dbname + @"..sfc_processflowdetail where pfid=" + pfid + " and OpSeq<'" + dtOpData.Rows[0]["opseq"] + "' order by OpSeq desc", Cmd);
                if (GetDataInt("select count(*) from " + dbname + "..sfc_pfreportdetail where pfdid=" + pf_fre_did, Cmd) == 0)
                    throw new Exception("上工序没有报工");
            }

            #endregion

            #region  //报工单主表
            KK_U8Com.U8sfc_pfreport sfcMain = new KK_U8Com.U8sfc_pfreport(Cmd, dbname);
            sfcMain.PFReportId = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreport'";
            Cmd.ExecuteNonQuery();
            sfcMain.DocCode = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_pfreport where doccode like 'PDA%'", Cmd) + "'";
            sfcMain.DocDate = "convert(varchar(10),getdate(),120)";
            sfcMain.CreateUser = "'" + cUserName + "'";
            sfcMain.UpdCount = "0";
            sfcMain.PFId = "" + pfid;
            sfcMain.MoDId = GetDataString("select modid from " + dbname + "..sfc_processflow where pfid=0" + pfid, Cmd);

            //自定义项
            sfcMain.Define1 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define1") + "'";
            sfcMain.Define2 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define2") + "'";
            sfcMain.Define3 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define3") + "'";
            sfcMain.Define4 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define4") + "'";
            string txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define5");
            sfcMain.Define5 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            sfcMain.Define6 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define6") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define7");
            sfcMain.Define7 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            sfcMain.Define8 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define8") + "'";
            sfcMain.Define9 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define9") + "'";
            sfcMain.Define10 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define10") + "'";
            sfcMain.Define11 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define11") + "'";
            sfcMain.Define12 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define12") + "'";
            sfcMain.Define13 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define13") + "'";
            sfcMain.Define14 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define14") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define15");
            sfcMain.Define15 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define16");
            sfcMain.Define16 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);

            if (!sfcMain.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            #endregion

            #region  //报工单子表
            KK_U8Com.U8sfc_pfreportdetail sfcDetail = new KK_U8Com.U8sfc_pfreportdetail(Cmd, dbname);
            sfcDetail.PFReportId = sfcMain.PFReportId;
            sfcDetail.PFReportDId = GetDataInt("select isnull(max(iChildid),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=" + sfcMain.PFReportId + ",iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_pfreportdetail'";
            Cmd.ExecuteNonQuery();
            sfcDetail.PFDId = "" + pfdid;
            sfcDetail.EmployCode = (PsnCode.CompareTo("") == 0 ? "null" : "'" + PsnCode + "'");
            sfcDetail.MoRoutingDId = dtOpData.Rows[0]["MoRoutingDId"] + "";

            i_qualified_qty = GetTextsFrom_FormData_Text(FormData, "txt_qualifiedqty");
            i_scrap_qty = GetTextsFrom_FormData_Text(FormData, "txt_scrapqty");
            i_refused_qty = GetTextsFrom_FormData_Text(FormData, "txt_refusedqty");
            if (i_qualified_qty.CompareTo("") == 0) i_qualified_qty = "0";
            if (i_scrap_qty.CompareTo("") == 0) i_scrap_qty = "0";
            if (i_refused_qty.CompareTo("") == 0) i_refused_qty = "0";
            if (decimal.Parse(i_qualified_qty) < 0) throw new Exception("合格数不能为负");
            if (decimal.Parse(i_scrap_qty) < 0) throw new Exception("废品数不能为负");
            if (decimal.Parse(i_refused_qty) < 0) throw new Exception("返修数不能为负");
            if (decimal.Parse(i_qualified_qty) == 0 && decimal.Parse(i_scrap_qty) == 0 && decimal.Parse(i_refused_qty) == 0)
                throw new Exception("不能所有报工数都为0");

            sfcDetail.QualifiedQty = i_qualified_qty;  //合格数
            sfcDetail.ScrapQty = i_scrap_qty;  //废品数
            if (scrapreasoncode.CompareTo("") != 0) sfcDetail.ScrapReasonCode = "'" + scrapreasoncode + "'";
            sfcDetail.RefusedQty = i_refused_qty;   //返修数
            if (refusedreasoncode.CompareTo("") != 0) sfcDetail.RefusedReasonCode = "'" + refusedreasoncode + "'";
            sfcDetail.DeclareQty = "0";  //报检数
            if (qualifiedreasoncode.CompareTo("") != 0) sfcDetail.QualifiedReasonCode = "'" + qualifiedreasoncode + "'";
            if (eqid.CompareTo("") != 0) sfcDetail.EQId = "'" + eqid + "'";
            if (ShiftId.CompareTo("") != 0) sfcDetail.ShiftId = "'" + ShiftId + "'";  //班组

            //自定义项赋值
            sfcDetail.Define22 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define22") + "'";
            sfcDetail.Define23 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define23") + "'";
            sfcDetail.Define24 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define24") + "'";
            sfcDetail.Define25 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define25") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define26");
            sfcDetail.Define26 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define27");
            sfcDetail.Define27 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            sfcDetail.Define28 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define28") + "'";
            sfcDetail.Define29 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define29") + "'";
            sfcDetail.Define30 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define30") + "'";
            sfcDetail.Define31 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define31") + "'";
            sfcDetail.Define32 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define32") + "'";
            sfcDetail.Define33 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define33") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define34");
            sfcDetail.Define34 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            txtdata_text = GetTextsFrom_FormData_Text(FormData, "txt_define35");
            sfcDetail.Define35 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            sfcDetail.Define36 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define36") + "'";
            sfcDetail.Define37 = "'" + GetTextsFrom_FormData_Text(FormData, "txt_define37") + "'";

            if (!sfcDetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            if (fU8Version >= 11)
            {
                Cmd.CommandText = "update " + dbname + "..sfc_pfreportdetail set DueDate=" + sfcMain.DocDate + " where PFReportDId=0" + sfcDetail.PFReportDId;
                Cmd.ExecuteNonQuery();
            }
            //记录完工单的 开工 完工时间
            //Cmd.CommandText = "update " + dbname + "..sfc_pfreportdetail set StartTime=case when StartTime is null then getdate() else StartTime end,DueTime=getdate() where PFReportDId=0" + sfcDetail.PFReportDId;
            //Cmd.ExecuteNonQuery();
            #endregion

            #region //报工单保存后逻辑校验
            System.Data.DataTable dtCHeckSql = GetSqlDataTable("select t_sql_check,t_msg_show from " + dbname + @"..T_CC_Base_SheetValid_rule where SheetID='" + SheetID + "'", "dtCHeckSql", Cmd);
            for (int c = 0; c < dtCHeckSql.Rows.Count; c++)
            {
                string c_check_sql = "" + dtCHeckSql.Rows[c]["t_sql_check"];
                c_check_sql = c_check_sql.Replace("@@dbname", dbname).Replace("@@ID", sfcMain.PFReportId + "");
                if (GetDataInt(c_check_sql, Cmd) > 0) throw new Exception("错误：" + dtCHeckSql.Rows[c]["t_msg_show"]);
            }
            #endregion

            #region  //回写流转卡
            //减少本工序的在制数
            Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set CompleteQty=isnull(CompleteQty,0)+" + i_qualified_qty + "+" + i_scrap_qty + "+" + i_refused_qty +
                ",balscrapqty=isnull(balscrapqty,0)+" + i_scrap_qty + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + i_refused_qty +
                ",balmachiningqty=isnull(balmachiningqty,0)-(" + i_qualified_qty + "+" + i_scrap_qty + "+" + i_refused_qty + ")" +
                ",DueTime=getdate() where PFDId=0" + pfdid;
            Cmd.ExecuteNonQuery();

            //在制数是否为负
            if (GetDataInt("select count(*) from " + dbname + "..sfc_processflowdetail where PFDId=0" + pfdid + " and isnull(balmachiningqty,0)<0", Cmd) > 0)
                throw new Exception("本工序报工后，在制数为负，即超上工序合格数报工");

            if (b_write_valid_qty)
            {
                //累计完工数  最后一道工序(或者本工序的下一个工序未外协工序) 累计合格数量
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + i_qualified_qty + " where PFDId=0" + pfdid;
                Cmd.ExecuteNonQuery();
            }
            else //下道工序的 可用 在制数
            {
                Cmd.CommandText = "update " + dbname + "..sfc_processflowdetail set balmachiningqty=isnull(balmachiningqty,0)+" + i_qualified_qty + " where PFDId=0" + pf_nextdid;
                Cmd.ExecuteNonQuery();
            }

            #endregion

            #region  //回写工序计划
            //累计 加工数
            Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set CompleteQty=isnull(CompleteQty,0)+" + i_qualified_qty + "+" + i_scrap_qty + "+" + i_refused_qty +
                ",balscrapqty=isnull(balscrapqty,0)+" + i_scrap_qty + ",BalRefusedQty=isnull(BalRefusedQty,0)+" + i_refused_qty +
                ",balmachiningqty=isnull(balmachiningqty,0)-(" + i_qualified_qty + "+" + i_scrap_qty + "+" + i_refused_qty + ") where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
            Cmd.ExecuteNonQuery();
            if (b_write_valid_qty)
            {
                //回写工序计划 本工序 累计 加工合格数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set BalQualifiedQty=isnull(BalQualifiedQty,0)+" + i_qualified_qty + " where MoRoutingDId=0" + sfcDetail.MoRoutingDId;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //回写工序计划 中下到工序的 在制数
                Cmd.CommandText = "update " + dbname + "..sfc_moroutingdetail set balmachiningqty=isnull(balmachiningqty,0)+" + i_qualified_qty + " where MoRoutingDId=0" + dtNextData.Rows[0]["MoRoutingDId"];
                Cmd.ExecuteNonQuery();
            }

            #endregion

            #region  //生成工序转移单
            //工序计划 主表ID
            string routing_ID = "" + dtOpData.Rows[0]["MoRoutingId"];
            string cc_moid = "" + dtOpData.Rows[0]["moid"];
            //废品 本工序内转移（加工 ->  废品数） sfc_optransform
            int sfc_optransform_id = 0;
            string sfc_optransform_code = "";
            if (int.Parse(i_refused_qty) + int.Parse(i_scrap_qty) > 0)  //本工序流转
            {
                sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
                Cmd.ExecuteNonQuery();
                sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";

                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + pfdid + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + i_refused_qty + "+" + i_scrap_qty + ",0," + i_refused_qty + "," + i_scrap_qty + ",0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + cUserName + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            sfc_optransform_id = GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1,iChildid=iChildid+1 where cacc_id='" + cacc_id + "' and cVouchType='sfc_optransform'";
            Cmd.ExecuteNonQuery();
            sfc_optransform_code = "'" + GetDataString("select 'PDA'+right('000000000'+cast(isnull(cast(replace(max(doccode),'PDA','') as int),0)+1 as varchar(20)),10) from " + dbname + "..sfc_optransform where doccode like 'PDA%'", Cmd) + "'";
            //转移到下到 工序[若下到工序不存在或者 外协工序， 转移地点为： 本工序加工数 到 本工序合格数]
            if (!b_write_valid_qty)
            {
                //存在后续自制工序   合格数  本工序内转移（加工 ->  下工序加工数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + pfdid + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + dtNextData.Rows[0]["MoRoutingDId"] + @",
                        " + i_qualified_qty + ",0,0,0,0," + i_qualified_qty + @",0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + cUserName + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                //不存在后续自制工序   //合格数   本工序内转移（加工 ->  合格数）
                Cmd.CommandText = "insert into " + dbname + @"..sfc_optransform(pfreportid,pfreportdid,pfdid,moid,modid,moroutingid,moroutingdid,transformtype,opstatus,Inmoroutingdid,
                    transoutqty,qualifiedqty,scrapqty,refusedqty,declareqty,machiningqty,declaredqty,   transformid,doccode,docdate,
                    status,bfedflag,qcflag,outqcflag,createtime,doctime,createuser,createdate ,qctype,inchangerate) values
                    (" + sfcMain.PFReportId + ",0," + pfdid + "," + cc_moid + "," + sfcMain.MoDId + "," + routing_ID + "," + sfcDetail.MoRoutingDId + ",1,1," + sfcDetail.MoRoutingDId + @",
                        " + i_qualified_qty + "," + i_qualified_qty + @",0,0,0,0,0," + sfc_optransform_id + "," + sfc_optransform_code + @",convert(varchar(10),getdate(),120),
                        1,0,0,0,getdate(),getdate(),'" + cUserName + "',convert(varchar(10),getdate(),120),0,null)";
                Cmd.ExecuteNonQuery();
            }

            #endregion

            Cmd.Transaction.Commit();
            return sfcMain.PFReportId + "," + sfcMain.DocCode;
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

    [WebMethod]  //华青盛具条码 入库
    public string BarCodeGoodsIn(string dbname, string cUserName, string cLogDate
        , string wareCode, string deptCode, string barCodeDetailIdList, string RdQtyList)
    {
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
            U8StandSCMBarCode u8op = new U8StandSCMBarCode();
            Cmd.CommandText = "use " + dbname;
            Cmd.ExecuteNonQuery();
            DataTable dtRdMain = GetSqlDataTable("select '102' crdcode,'" + wareCode + "' cwhcode,'" + deptCode+ "' cdepcode,null cbuscode,'成品入库' cbustype,'盛具入库' cDefine2", "dtRdMain", Cmd);
            DataTable dtRddetail = U8Operation.GetSqlDataTable(@"
select i.cInvCode,i.cInvName,i.cInvStd,cl.t_pro_cbatch_no cbatch,b.printQty iquantity,cl.t_card_no define23,a.id define24,cl.t_modid modid,b.id barCodeDetailId from t_cqcc_barCodePrintMain a 
inner join t_cqcc_barCodePrintDetail b on a.id = b.hid
inner join T_CC_Card_List cl on cl.t_card_id = b.t_card_id
inner join inventory i on i.cInvCode = cl.t_invcode
where b.id in (" + barCodeDetailIdList + @")                                                                                                                                           
order by a.id
", "BodyData", Cmd);
            string[] barCodeDetailIdArr = barCodeDetailIdList.Split(',');
            string[] RdQtyArr = RdQtyList.Split(',');
            foreach (DataRow row in dtRddetail.Rows)
            {
                // 将前台的数量传入datatable
                string barCodeDetailId = row["barCodeDetailId"] + "";
                int index  = Array.IndexOf(barCodeDetailIdArr, barCodeDetailId);
                if (index < 0) continue;
                row["iquantity"] = RdQtyArr[index];
            }
            DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            // 构建datatable调用标准的产成品入库
            string rdRs = u8op.U81015(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81015", Cmd);
            // 构建入库单主表的id获取子表id和流转卡的关系,存储关系表
            string[] rdRsArr = rdRs.Split(',');
            // 入库单主表id
            string rdMId = rdRsArr[0];
            DataTable dt =  GetSqlDataTable(@"
select AutoID,cl.t_card_id
from rdrecords10  rds10
inner join t_cc_card_list cl on cl.t_card_no=rds10.cDefine23
where rds10.ID=0" + rdMId, "a", Cmd);
            foreach (DataRow row in dt.Rows)
            {
                //WriteU8FlowDataToRd10(Cmd, row[0] + "", row[1] + "", "1", dbname);
                
            }
            #region// 华青要求 -- 将生产订单类别写入入库单子表的自定义项
            string mes_flow_hq_inRd_ProdOrderTypeDefine = GetDataString("select cValue from " + dbname + "..t_parameter where cpid='mes_flow_hq_inRd_ProdOrderTypeDefine'", Cmd);
            if (!string.IsNullOrEmpty(mes_flow_hq_inRd_ProdOrderTypeDefine))
            {
                Cmd.CommandText = @"
update rds set rds."+mes_flow_hq_inRd_ProdOrderTypeDefine+@"=b.Description
from rdrecords10 rds
inner join mom_orderdetail a on a.MoDId= rds.iMPoIds
inner join mom_motype b on a.MoTypeId = b.MoTypeId
where rds.ID=0"+rdMId;
                Cmd.ExecuteNonQuery();
            }
            #endregion
            #region //检验是否超流转卡合格完工数入库
            string errCardNo = GetDataString(@"
select cl.t_card_no from (
select sum(iQuantity) qty,rds10.cDefine23 cardNo 
from rdrecords10 rds10
where rds10.cDefine23 in (select a.cDefine23 from rdrecords10 a where a.ID=0" + rdMId + @" and isnull(a.cDefine23,'')!='') 
and isnull(cDefine24,'')!=''
group by rds10.cDefine23 
) rd
inner join T_CC_Card_List cl on cl.t_card_no = rd.cardNo
where cl.t_card_overqty<rd.qty
", Cmd);
            if (!string.IsNullOrEmpty(errCardNo))
            {
                throw new Exception("流转卡"+errCardNo+"超合格完工数入库");
            }
            #endregion

            
            Cmd.Transaction.Commit();
            // 返回入库单id和单号
            return rdRs;
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


    [WebMethod]  //MES 扫描MES流转卡  按生产订单入库
    public string U8Flow_TO_RD10(System.Data.DataTable FormData, string dbname, string cUserName, string cLogDate, string SheetID)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }

        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        System.Data.SqlClient.SqlTransaction tr = Conn.BeginTransaction();
        Cmd.Transaction = tr;
        try
        {
            string cRet = MES_OR_U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            string[] cRdInfo = cRet.Split(',');  //入库单ID，入库单单号，入库单子表标识
            //建立流转卡和 入库单的关联关系 
            string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
            if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
            string ccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");
            if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

            try
            {
                WriteU8FlowDataToRd10(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname);
            }
            catch (Exception ee)
            {
                throw ee;
            }

            Cmd.Transaction.Commit();

            return cRdInfo[0] + "," + cRdInfo[1];
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

    //回写U8流转卡 编写流转卡和入库单 对照逻辑
    private void WriteU8FlowDataToRd10(System.Data.SqlClient.SqlCommand cmd, string rd_autoid, string card_id, string ctype, string dbname)
    {
        cmd.CommandText = "insert into " + dbname + @"..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) values(" + rd_autoid + "," + card_id + "," + ctype.Split(',')[0] + ")";
        cmd.ExecuteNonQuery();

        #region //超入库量控制
        //获得参数  u8rework_flow_goodin_control  当为control 代表控制
        string c_control_parm = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cValue FROM " + dbname + "..T_Parameter where cpid='u8rework_flow_goodin_control'");
        if (c_control_parm.ToLower().CompareTo("control") == 0)  //严格管控
        {
            string cols = "";
            //判定上下工序逻辑关系
            float fRDIn = 0;
            int itype = int.Parse(ctype.Split(',')[0]);
            if (itype == 2) cols = "balscrapqty";
            if (itype == 3) cols = "balrefusedqty";
            if (itype == 1) cols = "balqualifiedqty";
            fRDIn = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select isnull(sum(" + cols + "),0) from " + dbname + "..sfc_processflowdetail where pfid=0" + card_id));

            float f_rd10_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(b.iquantity),0) from " + dbname + @"..T_CC_Rd10_FlowCard a 
                inner join " + dbname + @"..rdrecords10 b on a.t_autoid=b.autoid and a.t_ctype=" + itype + " and b.bRelated=0 where a.t_card_id=0" + card_id));
            if (fRDIn < f_rd10_qty) throw new Exception("本流转卡“" + ctype.Split(',')[1] + "”可入库【" + fRDIn + "】");
        }
        #endregion

        #region//关闭生产订单
        string modid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select modid from " + dbname + @"..sfc_processflow where pfid=0" + card_id);

        float imissqty = 0;  //缺失量
        //已经入库量
        float f_all_rkqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(iquantity),0) from " + dbname + @"..rdrecords10 where iMPoIds=0" + modid + " and bRelated=0"));
        //生产订单量
        float f_mo_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select isnull(sum(Qty),0) from " + dbname + @"..mom_orderdetail where modid=0" + modid));
        if (f_mo_qty == f_all_rkqty + imissqty)
        {
            //关闭生产订单
            cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='自动关闭',CloseDate=convert(varchar(10),getdate(),120),CloseTime=getdate(),Status='CL' where modid=0" + modid;
            cmd.ExecuteNonQuery();
        }
        if (f_mo_qty < f_all_rkqty + imissqty) throw new Exception("超生产订单入库 订单数[" + f_mo_qty + "]入库数[" + f_all_rkqty + "]缺失数[" + imissqty + "]");

        #endregion

    }


    #region //外部系统处理
    [WebMethod]  //获得当前序列号 加工工序信息
    public string U8FC_Rd10_FromOutMES(DataTable dtDatas, string dbname, string OutDBName, string cUserName, string cLogDate, string chkSql, string updSql, string rWtrite)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.SqlClient.SqlConnection RemoteConn = new System.Data.SqlClient.SqlConnection();
        #region //连接数据库
        if (Conn == null)
        {
            throw new Exception("本地数据库连接失败！");
        }

        RemoteConn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[OutDBName].ConnectionString;
        try
        {
            RemoteConn.Open();
        }
        catch
        {
            RemoteConn = null;
        }
        if (RemoteConn == null) throw new Exception("数据库" + OutDBName + "连接失败！");
        #endregion

        SqlCommand Rmd = RemoteConn.CreateCommand();
        SqlCommand Cmd = Conn.CreateCommand();
        Rmd.Transaction = RemoteConn.BeginTransaction();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string[] rwrite = rWtrite.Split('=');
            DataTable dtRemoteRow = GetSqlDataTable(chkSql, "dtRemoteRow", Rmd);
            if (dtRemoteRow.Rows.Count == 0) throw new Exception("没有找到条码信息");
            if (rwrite.Length == 2)
            {
                if (dtRemoteRow.Rows[0][rwrite[0]] + "" == rwrite[1])
                    throw new Exception("本单据已经入库");
            }
            string c_typename = dtRemoteRow.Rows[0]["TYPENAME"] + "";  //交库类型
            string crd_code = GetDataString("select tag_value from " + dbname + "..T_CC_Form_Field_Default where SheetID='U81015' and t_fieldname='crdcode' and t_type=0", Cmd);

            #region  //调用 标准条码入库
            U8StandSCMBarCode u8 = new U8StandSCMBarCode();
            DataTable dtHead = GetSqlDataTable("select '" + dtDatas.Rows[0]["cwhcode"] + "' cwhcode,'" + crd_code + "' crdcode", "dtHead", Cmd);


            //增加自定义项
            for (int i = 0; i < dtRemoteRow.Columns.Count; i++)
            {
                string colname = dtRemoteRow.Columns[i].ColumnName.ToLower();
                if (colname.IndexOf("cdefine") > -1)
                {
                    dtHead.Columns.Add(colname);
                    dtHead.Rows[0][colname] = dtRemoteRow.Rows[0][colname];
                }
            }

            string mes_flow_rd10_feip_control = U8Operation.GetDataString(@"select cValue from " + dbname + "..T_Parameter where cPid='mes_flow_rd10_feip_mer_bili_control'", Cmd).ToLower();
            if (mes_flow_rd10_feip_control.CompareTo("true") == 0)  //参数：废品不考虑齐套
            {
                dtHead.Columns.Add("notvalid_keypart");
                if (c_typename == "内部废品入库")
                {
                    dtHead.Rows[0]["notvalid_keypart"] = "notcompute_2";  //废品不计算齐套
                }
                else if (c_typename == "返修品入库")
                {
                    dtHead.Rows[0]["notvalid_keypart"] = "notcompute_3";  //返修
                }
                else if (c_typename == "外协废品入库")
                {
                    dtHead.Rows[0]["notvalid_keypart"] = "notcompute_5";  //外协废品
                }
                else if (c_typename == "改制品入库")
                {
                    dtHead.Rows[0]["notvalid_keypart"] = "notcompute_4";  //改制
                }
                else
                {
                    dtHead.Rows[0]["notvalid_keypart"] = "notcompute_1";  //废品不计算齐套
                }
            }

            dtHead = u8.GetDtToHeadData(dtHead, 0);
            dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };

            DataTable dtBody = GetSqlDataTable("select '" + dtDatas.Rows[0]["cinvcode"] + "' cinvcode,'" + dtDatas.Rows[0]["cinvname"] + @"'cinvname,
                '" + dtDatas.Rows[0]["cinvstd"] + "' cinvstd,'" + dtDatas.Rows[0]["cbatch"] + "' cbatch,'" + dtDatas.Rows[0]["qty"] + @"' iquantity,
                '" + dtDatas.Rows[0]["modid"] + "' modid,'' cposcode", "dtBody", Cmd);

            string cRet = u8.U81015(dtHead, dtBody, dbname, cUserName, cLogDate, "U81015", Cmd);
            #endregion

            //回写标志
            if (updSql + "" != "")
            {
                Rmd.CommandText = updSql;
                Rmd.ExecuteNonQuery();
            }
            Rmd.Transaction.Commit();
            Cmd.Transaction.Commit();
            return cRet;
        }
        catch (Exception ex)
        {
            Rmd.Transaction.Rollback();
            Cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(RemoteConn);
            CloseDataConnection(Conn);
        }

    }
    #endregion


    #endregion  //                         ************************************************************************************************

    #region //设备管理
    [WebMethod]  //查询数据，返回DataTable 的json 格式
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public string GetDataJsonTableFromSQL(string sql, string tblName)
    {
        string l_sql = sql.ToLower();
        if (l_sql.IndexOf("insert") > -1 || l_sql.IndexOf("update") > -1 || l_sql.IndexOf("delete") > -1 || l_sql.IndexOf("into") > -1)
        {
            return "只能执行查询操作";
        }

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) return "数据库连接失败";

        try
        {
            System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
            System.Data.DataTable dtRet = GetSqlDataTable(sql, tblName, Cmd);

            string cjson = VendorIO.DataTableToJson(dtRet);
            return cjson;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }

    }

    #region //作业单新增
    //作业单 新增
    [WebMethod(Description = @"维修上报接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            各参数的列名请使用对应表的字段名，小写。如；<br/>
                dtHead 作业单栏目头，只能一行数据<br/>
                dtItem 作业单项目，没有时传入 null<br/>
                dtPart 作业单配件，没有时传入 null<br/>
                dtTrouble 作业单故障记录，没有时传入 null<br/>
                i_Trouble_Level 故障等级 0代表 无故障，1 代表不停机故障，其他都代表停机故障<br/>
            返回 作业单号")]
    public string EQWorkSave(string dbname, DataTable dtHead, DataTable dtItem, DataTable dtPart, DataTable dtTrouble, int i_Trouble_Level)
    {
        if (dtHead == null || dtHead.Rows.Count == 0) throw new Exception("单据头部分必需有记录");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            #region  //head 赋值
            string cWorkCode = GetBodyValue_DTData(dtHead, 0, "cworkcode");
            string cPlanCode = GetBodyValue_DTData(dtHead, 0, "cplancode");
            string cWorkTypeCode = GetBodyValue_DTData(dtHead, 0, "cworktypecode");
            string intObjType = GetBodyValue_DTData(dtHead, 0, "intobjtype");  //作业类型  1  代表维修
            string cObj = GetBodyValue_DTData(dtHead, 0, "cobj");
            string dtPlanStart = GetBodyValue_DTData(dtHead, 0, "dtplanstart");
            string dtPlanEnd = GetBodyValue_DTData(dtHead, 0, "dtplanend");
            string cworkdep = GetBodyValue_DTData(dtHead, 0, "cworkdep");
            string cusedep = GetBodyValue_DTData(dtHead, 0, "cusedep");
            string cmemo = GetBodyValue_DTData(dtHead, 0, "cmemo");
            string cmaker = GetBodyValue_DTData(dtHead, 0, "cmaker");
            string cdefine4 = GetBodyValue_DTData(dtHead, 0, "cdefine4");//日期
            string cdefine6 = GetBodyValue_DTData(dtHead, 0, "cdefine6");//日期

            if (cmaker == "") throw new Exception("制单人（上报人姓名）必须录入");
            if (cObj == "") throw new Exception("设备编码必须录入");
            if (cWorkTypeCode == "") throw new Exception("作业类型必须录入");
            // 判断设备是否存在
            string cEqCode = U8Operation.GetDataString("select cEQCode from " + dbname + "..EQ_EQData where cEQCode='" + cObj + "'", Cmd);
            if (string.IsNullOrEmpty(cObj))
            {
                throw new Exception("没有找到设备号[" + cObj + "]");
            }
            // 更新设备号为标准的设备号(避免大小写错误)
            cObj = cEqCode;
            //判断 维修作业单 一台设备不能同时存在 两张未验收的作业单
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cObj='" + cObj + "' and intObjType=2 and cWorkTypeCode='1' and ISNULL(intCheck,0)<>1 and isnull(cCloser,'')=''", Cmd) > 0)
            {
                throw new Exception("设备号[" + cObj + "]已经存在未结案的维修单，必须把前一张维修作业单处理完毕");
            }

            if (cWorkCode == "")  //重新获得单据号
            {
                string cCodeHead = "W";
                cWorkCode = cCodeHead + U8Operation.GetDataString("select right('0000000000'+cast(cast(isnull(right(max(cWorkCode),10),'00000') as int)+1 as varchar(15)),10) from " + dbname + "..EQ_Work where cWorkCode like '" + cCodeHead + "%'", Cmd);
            }
            if (cmaker == "") cmaker = "SYSTEM";
            
            if (cusedep == "")
            {
                cusedep = GetDataString("select cDepCode from " + dbname + "..EQ_EQData where cEQCode='" + cObj + "'", Cmd);
            }

            //是否自动审核
            string chekername = "null";
            string checkdate = "null";
            //是否自动确认
            if (GetDataString("select cvalue from " + dbname + "..T_Parameter where cPid='mes_eq_work_auto_check'", Cmd).ToLower().CompareTo("true") == 0)
            {
                chekername = "'" + cmaker + "'";
                checkdate = "convert(varchar(10),getdate(),120)";
            }

            //插入主表记录
            Cmd.CommandText = @"insert into " + dbname + @"..EQ_Work(cWorkCode, cPlanCode, cWorkTypeCode, intObjType, cObj,dtPlanStart, dtPlanEnd,
                  cWorkDep, cUseDep, intFlag, cMaker, dtDate, 
                  cChecker,dtAudit,cMemo, VT_ID, cDefine1, cDefine2, cDefine3, cDefine4, cDefine5, cDefine6, 
                  cDefine7, cDefine8, cDefine9, cDefine10, cDefine11, cDefine12, cDefine13, cDefine14, cDefine15, cDefine16, cSysBarCode) 
                values('" + cWorkCode + "', '" + cPlanCode + "','" + cWorkTypeCode + "',2,'" + cObj + "'," + (dtPlanStart == "" ? "null" : "'" + dtPlanStart + "'") + ", " + (dtPlanEnd == "" ? "null" : "'" + dtPlanEnd + "'") + @", 
                  " + (cworkdep == "" ? "null" : "'" + cworkdep + "'") + ", " + (cusedep == "" ? "null" : "'" + cusedep + "'") + ", 2,'" + cmaker + @"',convert(varchar(20),getdate(),120), 
                  " + chekername + "," + checkdate + @",'" + cmemo + "', 8,'" + GetBodyValue_DTData(dtHead, 0, "cdefine1") + "', '" + GetBodyValue_DTData(dtHead, 0, "cdefine2") + @"',
                  '" + GetBodyValue_DTData(dtHead, 0, "cdefine3") + "', " + (cdefine4 == "" ? "null" : "'" + cdefine4 + "'") + ",0" + GetBodyValue_DTData(dtHead, 0, "cdefine5") + ", " + (cdefine6 == "" ? "null" : "'" + cdefine6 + "'") + @",
                    0" + GetBodyValue_DTData(dtHead, 0, "cdefine7") + ",'" + GetBodyValue_DTData(dtHead, 0, "cdefine8") + "', '" + GetBodyValue_DTData(dtHead, 0, "cdefine9") + @"',
                  '" + GetBodyValue_DTData(dtHead, 0, "cdefine10") + "', '" + GetBodyValue_DTData(dtHead, 0, "cdefine11") + "', '" + GetBodyValue_DTData(dtHead, 0, "cdefine12") + @"', 
                  '" + GetBodyValue_DTData(dtHead, 0, "cdefine13") + "', '" + GetBodyValue_DTData(dtHead, 0, "cdefine14") + "', 0" + GetBodyValue_DTData(dtHead, 0, "cdefine15") + @", 
                  0" + GetBodyValue_DTData(dtHead, 0, "cdefine16") + ", '||EQ12|" + cWorkCode + "')";
            Cmd.ExecuteNonQuery();

            //获得主键ID
            string w_autoid = "" + GetDataString("select IDENT_CURRENT( '" + dbname + @"..EQ_Work' )", Cmd);
            string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
            if (w_autoid != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

            #region //添加作业单附表  (含故障原因)
            string ctrouble_type = "";
            if (!(dtTrouble == null || dtTrouble.Rows.Count == 0))
            {
                ctrouble_type = GetBodyValue_DTData(dtTrouble, 0, "ctroubletypecode");
            }
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_WorkSub(workid,cworkcode,c_trouble_level,trouble_code) 
                values(" + w_autoid + ",'" + cWorkCode + "','" + i_Trouble_Level + "','" + ctrouble_type + "')";
            Cmd.ExecuteNonQuery();

            #endregion
            #endregion

            #region  //项目清单
            if (!(dtItem == null || dtItem.Rows.Count == 0))
            {
                for (int i = 0; i < dtItem.Rows.Count; i++)
                {
                    string citemcode = GetBodyValue_DTData(dtItem, i, "citemcode");
                    string citemname = GetBodyValue_DTData(dtItem, i, "citemname");
                    if (citemcode == "") throw new Exception("未获得项目编码");
                    if (citemname == "") throw new Exception("未获得项目名称");
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkItem where cbillcode='" + cWorkCode + "' and citemcode='" + citemcode + "'", Cmd) > 0)
                    {
                        throw new Exception("项目号[" + citemcode + "]重复");
                    }

                    Cmd.CommandText = "insert into " + dbname + "..EQ_WorkItem(cbillcode,citemcode,citemname) values('" + cWorkCode + "','" + citemcode + "','" + citemname + "')";
                    Cmd.ExecuteNonQuery();

                }
            }
            #endregion

            #region  //配件清单
            if (!(dtPart == null || dtPart.Rows.Count == 0))
            {
                for (int i = 0; i < dtPart.Rows.Count; i++)
                {
                    string citemcode = GetBodyValue_DTData(dtPart, i, "citemcode");
                    string cinvcode = GetBodyValue_DTData(dtPart, i, "cinvcode");
                    string cwhcode = GetBodyValue_DTData(dtPart, i, "cwhcode");
                    string fvalue = GetBodyValue_DTData(dtPart, i, "fvalue");
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkItem where cbillcode='" + cWorkCode + "' and citemcode='" + citemcode + "'", Cmd) == 0)
                    {
                        throw new Exception("配件[" + cinvcode + "]的项目号[" + citemcode + "]不存在");
                    }
                    string cJldw = "" + U8Operation.GetDataString("select cComUnitCode from inventory where cinvcode='" + cinvcode + "'", Cmd);
                    if (cwhcode == "") cwhcode = GetDataString("select cDefWareHouse from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", Cmd);
                    Cmd.CommandText = "insert into " + dbname + @"..EQ_WorkInventory(cbillcode,citemcode,cinvcode,fValue,cwhcode,intRef,cJldw) 
                        values('" + cWorkCode + "','" + citemcode + "','" + cinvcode + "',0" + fvalue + ",'" + cwhcode + "',2,'" + cJldw + "')";
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion

            if (chekername != "null")
            {
                EQwork_Extend_1(Cmd, dbname, cWorkCode, cmaker, i_Trouble_Level, ctrouble_type);
            }

            Cmd.CommandText = "update " + dbname + @"..EQ_Work set cWorkCode='" + cWorkCode + @"',cDefine4=case when cDefine4 is null then getdate() else cDefine4 end 
                where autoid=" + w_autoid;
            Cmd.ExecuteNonQuery();
            //T_CC_EQ_State
            Cmd.Transaction.Commit();
            return cWorkCode;
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
    private void EQwork_Extend(SqlCommand Cmd, string dbname, string cworkcode, string cmaker, int i_Trouble_Level)
    {
        EQwork_Extend_1(Cmd, dbname, cworkcode, cmaker, i_Trouble_Level, "");
    }
    private void EQwork_Extend_1(SqlCommand Cmd, string dbname, string cworkcode, string cmaker, int i_Trouble_Level, string troublecode)
    {
        string w_autoid = "" + GetDataString("select AutoID from " + dbname + @"..EQ_Work where cWorkCode ='" + cworkcode + "' and intObjType=2 and cWorkTypeCode='1'", Cmd);
        if (w_autoid == "") throw new Exception("作业单【" + cworkcode + "】不存在,或作业单对象不是设备");
        string cObj = "" + GetDataString("select cObj from " + dbname + @"..EQ_Work where cWorkCode ='" + cworkcode + "'", Cmd);
        string cusedep = "" + GetDataString("select cUseDep from " + dbname + @"..EQ_Work where cWorkCode ='" + cworkcode + "'", Cmd);

        #region//判断是否自动派工
        DataTable dtPG = U8Operation.GetSqlDataTable("select b.cPsn_Num,b.cDept_num from " + dbname + "..T_CC_EQ_Setting a inner join " + dbname + @"..hr_hi_person b on a.cworkpsncode=b.cPsn_Num 
                where ceq_code='" + cObj + "' and a.ctype='1' and iautopsn='是'", "dtPG", Cmd);
        if (dtPG.Rows.Count > 0)
        {
            Cmd.CommandText = "update " + dbname + @"..EQ_Work set cWorkDep='" + dtPG.Rows[0]["cDept_num"] + "',cPlaner='" + dtPG.Rows[0]["cPsn_Num"] + "' where AutoID=0" + w_autoid;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        #region  //设备状态处理
        //添加状态记录
        if (GetDataInt("select count(*) from  " + dbname + @"..T_CC_EQ_State where cEQCode='" + cObj + "'", Cmd) == 0)
        {
            Cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_State(cEQCode, cRunState,cToubleState, iTroubleLevel, dTime, dUpMaker, state_starttime) 
                    values('" + cObj + "','停机','正常',0,getdate(),'" + cmaker + "',getdate())";
            Cmd.ExecuteNonQuery();
        }

        //更新故障状态
        // whf 修改i_Trouble_Level =>iTroubleLevel
//        Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cToubleState='故障',cworkcode='" + cworkcode + "',trouble_starttime=getdate(),dUpMaker='" + cmaker + @"',
//                    trouble_code='" + troublecode + "',i_Trouble_Level=" + i_Trouble_Level + @"
//                where cEQCode='" + cObj + "' and cToubleState='正常'";
        Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cToubleState='故障',cworkcode='" + cworkcode + "',trouble_starttime=getdate(),dUpMaker='" + cmaker + @"',
                    trouble_code='" + troublecode + "',iTroubleLevel=" + i_Trouble_Level + @"
                where cEQCode='" + cObj + "' and cToubleState='正常'";
        Cmd.ExecuteNonQuery();

        if (i_Trouble_Level > 1)  //停机故障时 需要停机处理
        {
            string stop_code = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_run_stop_from_trouble_code'", Cmd);
            if (stop_code == "") throw new Exception("请维护参数【设备停机原因-报修时自动停机原因代码】");
            eq_work_stop(Cmd, dbname, cObj, stop_code, cmaker);
        }
        #endregion

        #region//消息处理
        // 华青设备消息处理
        string cadmincode = GetDataString("select cadmincode from " + dbname + "..T_CC_EQ_Setting where ceq_code='" + cObj + "' and ctype='1'", Cmd);
        DataTable dt = GetSqlDataTable(@"select a.t_psncode from " + dbname + @"..T_CC_EQ_WorkTPerson a
inner join " + dbname + @"..T_CC_EQ_WorkTPerson b on a.t_teamcode = b.t_teamcode
where b.t_is_head=1 and b.t_psncode = '" +cadmincode+"'", "tb", Cmd);
        foreach (DataRow row in dt.Rows)
        {
            eq_send_message(Cmd, "维修上报[" + cObj + "]", cObj + "设备故障,请接单或指派人员!", "作业单", w_autoid, row[0] + "", dbname);
        }
        //string cReceiveder = GetDataString("select cworkpsncode from " + dbname + "..T_CC_EQ_Setting where ceq_code='" + cObj + "' and ctype='1'", Cmd);
        //string troubleName = GetDataString("select t_item_name from " + dbname + "..T_CC_Base_Enum where t_team_id='eq_trouble_level' and t_item_code='" + i_Trouble_Level + "'", Cmd);
        //string eqUseDep = GetDataString("select cdepname from " + dbname + "..department where cdepcode='" + cusedep + "'", Cmd);
        //if (eqUseDep == "") eqUseDep = "无";
        //if (cReceiveder != "")
        //{
        //    //有专门的设备管理员 
        //    eq_send_message(Cmd, "维修上报[" + cObj + "]", "设备[" + cObj + "]故障等级[" + troubleName + "],使用部门[" + eqUseDep + "],上报人[" + cmaker + "]", "作业单", w_autoid, cReceiveder, dbname);

        //    //设备管理员消息
        //    string cadminpsn = GetDataString("select cadmincode from " + dbname + "..T_CC_EQ_Setting where ceq_code='" + cObj + "' and ctype='1'", Cmd);
        //    if (cadminpsn != "")
        //    {
        //        eq_send_message(Cmd, "维修上报[" + cObj + "]", "设备[" + cObj + "]故障等级[" + troubleName + "],使用部门[" + eqUseDep + "],上报人[" + cmaker + "]", "作业单", w_autoid, cadminpsn, dbname);
        //    }
        //}
        //else
        //{
        //    //所有维修员得到 消息，hr_hi_person.cPsnProperty  人员属性=维修员
        //    DataTable dtRecList = GetSqlDataTable("select cpsn_num from " + dbname + "..hr_hi_person where cPsnProperty='维修员'", "dtRecList", Cmd);
        //    for (int p = 0; p < dtRecList.Rows.Count; p++)
        //    {
        //        cReceiveder = dtRecList.Rows[p]["cpsn_num"] + "";
        //        eq_send_message(Cmd, "维修上报[" + cObj + "]", "设备[" + cObj + "]故障等级[" + troubleName + "],使用部门[" + eqUseDep + "],上报人[" + cmaker + "]", "作业单", w_autoid, cReceiveder, dbname);
        //    }
        //}
        
        #endregion

    }

    [WebMethod]
    public string EQWorkSave_json(string dbname, string jsnHead, string jsnItem, string jsnPart, string jsnTrouble, int i_Trouble_Level)
    {
        DataTable dtHead = null;
        DataTable dtItem = null;
        DataTable dtPart = null;
        DataTable dtTrouble = null;
        if (jsnHead != "") dtHead = VendorIO.JsonToDataTable(jsnHead);
        if (jsnItem != "") dtItem = VendorIO.JsonToDataTable(jsnItem);
        if (jsnPart != "") dtPart = VendorIO.JsonToDataTable(jsnPart);
        if (jsnTrouble != "") dtTrouble = VendorIO.JsonToDataTable(jsnTrouble);

        try
        {
            return EQWorkSave(dbname, dtHead, dtItem, dtPart, dtTrouble, i_Trouble_Level) + ",成功";
        }
        catch (Exception ex)
        {
            return "," + dtHead.Rows.Count + ex.Message;
        }
    }
    #endregion

    #region //作业 确认
    [WebMethod(Description = @"作业单（确认）接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            cworkcode 作业单号；<br/>
            cUserName 代表派单人<br/>
            cContent  代表作业内容<br/>
            bClosed 是否终止流程  true 代表终止 <br/>
            ")] //作业单（确认）
    public bool EQWork_Check(string dbname, string cworkcode, string cUserName, string cContent, int i_Trouble_Level, bool bClosed)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            //作业单是否审核
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + cworkcode + "' and isnull(cChecker,'')<>''", Cmd) > 0)
                throw new Exception("作业单[" + cworkcode + "]已经确认");

            //记录主要维修员
            if (bClosed)
            {
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set cChecker='" + cUserName + @"',dtAudit=convert(varchar(20),getdate(),120),
                   cCloser='" + cUserName + @"',dtClose=convert(varchar(10),getdate(),120),cMemo='现场确认直接结束' where cWorkCode='" + cworkcode + "'";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set cChecker='" + cUserName + "',cdefine14=left('" + cContent + @"',120),
                    dtAudit=convert(varchar(10),getdate(),120) where cWorkCode='" + cworkcode + "'";
                Cmd.ExecuteNonQuery();

                //审核处理
                EQwork_Extend(Cmd, dbname, cworkcode, cUserName, i_Trouble_Level);
            }

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
    [WebMethod(Description = @"作业单（确认）接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            cworkcode 作业单号；<br/>
            cUserName 代表派单人<br/>
            cContent  代表作业内容<br/>
            bClosed 是否终止流程  true 代表终止 <br/>
            返回空字符串，代表成功，返回非空字符串 ，则是错误信息
            ")] //作业单（确认）
    public string EQWork_Check_json(string dbname, string cworkcode, string cUserName, string cContent, int i_Trouble_Level, bool bClosed)
    {
        try
        {
            EQWork_Check(dbname, cworkcode, cUserName, cContent, i_Trouble_Level, bClosed);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
    [WebMethod(Description = @"设备更换接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            cworkcode 作业单号；<br/>
            cUserName 代表操作员<br/>
            newEqCode 代表新设备编码<br/>
            返回空字符串，代表成功，返回非空字符串 ，则是错误信息
            ")] //设备更换
    public string EqChange_json(string dbname, string cworkcode, string cUserName,string newEqCode)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            Cmd.CommandText = "use ["+dbname+"]";
            Cmd.ExecuteNonQuery();

            if (GetDataInt("select count(*) from EQ_Work where cWorkCode = '" + cworkcode + "' and intCheck = 1", Cmd)>0)
            {
                throw new Exception("当前作业单已验收,不能再更换设备");
            }
            string srcEqCode = GetDataString("select cObj from EQ_Work where cWorkCode='"+cworkcode+"'" , Cmd);
            // 更换设备
            Cmd.CommandText = "update EQ_Work set cObj = '"+newEqCode+"' where cWorkCode='"+cworkcode+"'";
            Cmd.ExecuteNonQuery();
            #region  //设备状态处理
            // 获取老设备的状态信息
            DataTable dt = GetSqlDataTable("select trouble_code,trouble_starttime,iTroubleLevel from T_CC_EQ_State where cEQCode = '"+srcEqCode+"'", "tb", Cmd);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("获取老设备信息失败");
            }
            // 插入设备更换记录表
            Cmd.CommandText = "insert into t_cqcc_EqChange(cWorkCode,srcEqCode,newEqCode,cUser)values('" + cworkcode + "','" + srcEqCode + "','" + newEqCode + "','" + cUserName + "')";
            Cmd.ExecuteNonQuery();
            //添加状态记录
            if (GetDataInt("select count(*) from  T_CC_EQ_State where cEQCode='" + newEqCode + "'", Cmd) == 0)
            {
                Cmd.CommandText = @"insert into T_CC_EQ_State(cEQCode, cRunState,cToubleState, iTroubleLevel, dTime, dUpMaker, state_starttime) 
                    values('" + newEqCode + "','停机','正常',0,getdate(),'" + cUserName + "',getdate())";
                Cmd.ExecuteNonQuery();
            }
            string troublecode = dt.Rows[0][0]+"";
            string trouble_starttime = dt.Rows[0][1] + "";
            int i_Trouble_Level = int.Parse(dt.Rows[0][2] + "");
            //更新故障状态
            Cmd.CommandText = "update T_CC_EQ_State set cToubleState='故障',cworkcode='" + cworkcode + "',trouble_starttime='" + trouble_starttime + "',dUpMaker='" + cUserName + @"',
                    trouble_code='" + troublecode + "',iTroubleLevel=" + i_Trouble_Level + @"
                where cEQCode='" + newEqCode + "' and cToubleState='正常'";
            Cmd.ExecuteNonQuery();

            if (i_Trouble_Level > 1)  //停机故障时 需要停机处理
            {
                string stop_code = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_run_stop_from_trouble_code'", Cmd);
                if (stop_code == "") throw new Exception("请维护参数【设备停机原因-报修时自动停机原因代码】");
                //stopcause  停机原因
                //获得状态开始时间信息
                if (U8Operation.GetDataInt("select count(*) from EQ_EQData where cEQCode='" + newEqCode + "'", Cmd) == 0)
                    throw new Exception("设备[" + newEqCode + "]不存在");

                string thetime = GetDataString("select CONVERT(varchar(20),getdate(),120)", Cmd);
                DataTable dtStartEQ = GetSqlDataTable(@"select convert(varchar(20),state_starttime,120) state_starttime,cRunState,cToubleState from T_CC_EQ_State 
            where cEQCode='" + newEqCode + "' and cRunState ='运行'", "dtStartEQ", Cmd);
                if (dtStartEQ.Rows.Count > 0)
                {
                    string runtypecode = "";
                    if (dtStartEQ.Rows[0]["cToubleState"] + "" == "故障")
                    {
                        runtypecode = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_runtrouble_type_code'", Cmd);
                        if (runtypecode == "") throw new Exception("请维护参数【设备运行记录-设备带故障运行时的运行状态代码】");
                    }
                    else
                    {
                        runtypecode = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_run_type_code'", Cmd);
                        if (runtypecode == "") throw new Exception("请维护参数【设备运行记录-正常运行时的运行状态代码】");
                    }
                    if (GetDataInt("select count(*) from " + dbname + "..EQ_RunType where cRunTypeCode='" + runtypecode + "'", Cmd) == 0)
                        throw new Exception("设备运行状态码【" + runtypecode + "】不存在");

                    //添加运行记录
                    eq_work_run_record(Cmd, dbname, newEqCode, dtStartEQ.Rows[0]["state_starttime"] + "", thetime, cUserName, runtypecode);

                    //更换状态
                    Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cRunState='停机',dTime='" + thetime + "',state_starttime='" + thetime + "',dUpMaker='" + cUserName + @"',
                stop_code='" + stop_code + "' where cEQCode='" + newEqCode + "' and cRunState='运行'";
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion
            Cmd.Transaction.Commit();
            return "";
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

    #region //作业派单
    [WebMethod(Description = @"作业单（接单/派单）接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            worklist 代表作业清单，里面有两列列名，第一列：cworkcode 作业单号；<br/>
                                                   第二列：cpsncode  被指派维修的人员编码清单 逗号隔开 主要维修员放在第一个；
            cUserName 代表派单人<br/>
            ")] //作业单（接单）
    public bool EQWork_Person(string dbname, DataTable worklist, string cUserName)
    {
        if (worklist == null || worklist.Rows.Count == 0) throw new Exception("请传入作业单号清单");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            for (int i = 0; i < worklist.Rows.Count; i++)
            {
                //作业单是否审核
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and isnull(cChecker,'')=''", Cmd) > 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]没有确认");

                //已经开始维修 不能指定 人员
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and dtFactEnd is not null", Cmd) > 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]已经维修完毕，不能指定人员");

                string cpsncode = worklist.Rows[i]["cpsncode"] + "";
                string[] psnlist = cpsncode.Split(',');
                cpsncode = psnlist[0];//主要维修人

                if (GetDataInt("select count(*) from " + dbname + @"..hr_hi_person where cpsn_num='" + cpsncode + "'", Cmd) == 0)
                {
                    cpsncode = GetDataString("select cPsn_Num from " + dbname + @"..UserHrPersonContro where cUser_Id='" + cpsncode + "'", Cmd);
                }

                string cdepcode = U8Operation.GetDataString("select cDept_num from " + dbname + @"..hr_hi_person where cpsn_num='" + cpsncode + "'", Cmd);
                if (cdepcode == "") throw new Exception("人员不存在");

                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", Cmd) == 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]不存在");
                // 记录派单员
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set cDefine1='" + cUserName + "' where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
                //记录主要维修员
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set cPlaner='" + cpsncode + "' where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
                //删除原维修人员清单
                Cmd.CommandText = "delete from " + dbname + @"..T_CC_EQ_WorkPsn where t_workcode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();

                //添加维修人员清单
                for (int p = 0; p < psnlist.Length; p++)
                {
                    if (GetDataInt("select count(*) from " + dbname + @"..hr_hi_person where cpsn_num='" + psnlist[p] + "'", Cmd) == 0)
                    {
                        throw new Exception("人员[" + psnlist[p] + "]不存在");
                    }

                    Cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_WorkPsn(t_workcode,t_psncode,t_time,t_is_header) 
                         values('" + worklist.Rows[i]["cworkcode"] + "','" + psnlist[p] + "',getdate()," + (p == 0 ? 1 : 0) + ")";
                    Cmd.ExecuteNonQuery();
                }

                #region //消息处理
                DataTable dtOneWork = GetSqlDataTable(@"select a.autoid,a.cObj,b.cpsn_name,c.cadmincode,a.cMaker,t.cWorkTypeName
                    from " + dbname + @"..EQ_Work a left join " + dbname + @"..EQ_WorkType t on a.cWorkTypeCode=t.cWorkTypeCode
                        left join " + dbname + "..hr_hi_person b on a.cPlaner=b.cpsn_num left join " + dbname + @"..T_CC_EQ_Setting c on a.cObj=c.ceq_code and a.cWorkTypeCode=c.ctype
                    where a.cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "dtOneWork", Cmd);
                if (dtOneWork.Rows.Count > 0)
                {
                    //接单人获得消息
                    eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”派单[" + dtOneWork.Rows[0]["cObj"] + "]",
                        "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”已经派单，接单人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                        "作业单", dtOneWork.Rows[0]["autoid"] + "", cpsncode, dbname);

                    //设备管理员获得消息
                    if (dtOneWork.Rows[0]["cadmincode"] + "" != "")
                    {
                        eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”派单[" + dtOneWork.Rows[0]["cObj"] + "]",
                            "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”已经派单，接单人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                            "作业单", dtOneWork.Rows[0]["autoid"] + "", dtOneWork.Rows[0]["cadmincode"] + "", dbname);
                    }

                    //上报人获得消息
                    //获得上报人的 业务员编码
                    string cup_maker_code = GetDataString(@"SELECT b.cPsn_Num FROM " + dbname + @"..UserHrPersonContro b inner join UFSystem..UA_User c on b.cUser_Id=c.cUser_Id 
                        where c.cUser_Name='" + dtOneWork.Rows[0]["cMaker"] + "'", Cmd);
                    if (cup_maker_code != "")
                    {
                        eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”派单[" + dtOneWork.Rows[0]["cObj"] + "]",
                            "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”已经派单，接单人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                            "作业单", dtOneWork.Rows[0]["autoid"] + "", cup_maker_code, dbname);
                    }

                }
                #endregion
                // 接单自动现场判定
                if (GetDataString("select cvalue from "+dbname+"..T_Parameter where cpid = 'mes_eq_RecOrderAutoRepair' ", Cmd)=="是")
                {
                    string cworkcode = worklist.Rows[i]["cworkcode"]+"";
                    DataTable dtTheWork = GetSqlDataTable("select cObj,cWorkTypeCode,intObjType from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "dtTheWork", Cmd);
                    Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtFactStart=CONVERT(varchar(10),getdate(),120),tFactStart=right(CONVERT(varchar(20),getdate(),120),8) 
                    where cWorkCode='" + cworkcode + "'";
                    Cmd.ExecuteNonQuery();
                    string troubleCode = "1";
                    #region //更新设备故障原因
                    string cObj = dtTheWork.Rows[0]["cObj"] + "";
                    if (cObj != "" && dtTheWork.Rows[0]["cWorkTypeCode"] + "" == "1")  //维修作业
                    {
                        Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set trouble_code='" + troubleCode + "',cworkcode='" + cworkcode + @"',dTime=getdate()
                where cEQCode='" + cObj + "' and cToubleState='故障'";
                        Cmd.ExecuteNonQuery();
                        //记录维修故障码
                        Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_WorkSub set trouble_code='" + troubleCode + @"' where cworkcode='" + cworkcode + "'";
                        Cmd.ExecuteNonQuery();
                    }
                    #endregion
                }
            }

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
    [WebMethod]
    public string EQWork_Person_json(string dbname, string jsnworklist, string cUserName)
    {
        DataTable worklist = null;
        if (jsnworklist != "") worklist = VendorIO.JsonToDataTable(jsnworklist);

        try
        {
            EQWork_Person(dbname, worklist, cUserName);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    #endregion

    #region //作业单开始作业（或判定）
    [WebMethod(Description = @"作业单（即开始维修）接口，方式一。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            cworkcode 代表作业单号；<br/>
            dtItem 作业单项目，没有时传入 null<br/>
            dtPart 作业单配件，没有时传入 null<br/>
            troublecode 设备故障原因  <br/>
            bOutWork 是否委外维修，true 代表委外给其他厂商处理  <br/>
            ")] //作业单配件（配件、项目调整）
    public bool EQWork_WorkingOn(string dbname, string cworkcode, DataTable dtItem, DataTable dtPart, string troublecode, bool bOutWork)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            EQ_Working(dbname, sCmd, cworkcode, dtItem, dtPart, troublecode, bOutWork);
            sCmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod]
    public string EQWork_WorkingOn_json(string dbname, string cworkcode, string jsnItem, string jsnPart, string troublecode, bool bOutWork)
    {
        DataTable dtItem = null;
        DataTable dtPart = null;
        if (jsnItem != "") dtItem = VendorIO.JsonToDataTable(jsnItem);
        if (jsnPart != "") dtPart = VendorIO.JsonToDataTable(jsnPart);

        try
        {
            EQWork_WorkingOn(dbname, cworkcode, dtItem, dtPart, troublecode, bOutWork);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private bool EQ_Working(string dbname, SqlCommand Cmd, string cworkcode, DataTable dtItem, DataTable dtPart, string troubleCode, bool bOutWork)
    {
        DataTable dtTheWork = GetSqlDataTable("select cObj,cWorkTypeCode,intObjType from " + dbname + @"..EQ_Work where cWorkCode='" + cworkcode + "'", "dtTheWork", Cmd);
        if (dtTheWork.Rows.Count == 0) throw new Exception("作业单[" + cworkcode + "]不存在");
        if (dtTheWork.Rows[0]["intObjType"] + "" != "2") throw new Exception("作业单[" + cworkcode + "]对象类型不是“设备”");
        if (dtTheWork.Rows[0]["cWorkTypeCode"] + "" == "1")  //维修时 必须有 故障代码
        {
            if (troubleCode == "")
            {
                throw new Exception("作业单[" + cworkcode + "]维修故障判定时必须有 故障原因");
            }
            else
            {
                if (GetDataInt("select count(*) from " + dbname + @"..EQ_TroubleType where cTroubleTypeCode='" + troubleCode + "'", Cmd) == 0)
                    throw new Exception("故障原因码不存在");
            }
        }

        //判断
        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + cworkcode + "' and isnull(cPlaner,'')<>''", Cmd) == 0)
            throw new Exception("作业单[" + cworkcode + "]未指定人员");

        //判断是否 已经结束
        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + cworkcode + "' and intCheck=1", Cmd) > 0)
            throw new Exception("作业单[" + cworkcode + "]已经验收");

        //实际开始
        if (bOutWork)
        {
            Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtFactStart=CONVERT(varchar(10),getdate(),120),tFactStart=right(CONVERT(varchar(20),getdate(),120),8),cMemo='本单委外' 
                    where cWorkCode='" + cworkcode + "'";
            Cmd.ExecuteNonQuery();
        }
        else
        {
            Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtFactStart=CONVERT(varchar(10),getdate(),120),tFactStart=right(CONVERT(varchar(20),getdate(),120),8) 
                    where cWorkCode='" + cworkcode + "'";
            Cmd.ExecuteNonQuery();
        }


        #region  //项目清单
        if (!(dtItem == null || dtItem.Rows.Count == 0))
        {
            for (int i = 0; i < dtItem.Rows.Count; i++)
            {
                string citemcode = GetBodyValue_DTData(dtItem, i, "citemcode");
                string citemname = GetBodyValue_DTData(dtItem, i, "citemname");
                if (citemcode == "") throw new Exception("未获得项目编码");
                if (citemname == "") throw new Exception("未获得项目名称");
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkItem where cbillcode='" + cworkcode + "' and citemcode='" + citemcode + "'", Cmd) > 0)
                {
                    throw new Exception("项目号[" + citemcode + "]重复");
                }

                Cmd.CommandText = "insert into " + dbname + "..EQ_WorkItem(cbillcode,citemcode,citemname) values('" + cworkcode + "','" + citemcode + "','" + citemname + "')";
                Cmd.ExecuteNonQuery();

            }
        }

        //判断项目是否存在   默认强制增加 作业单 的项目
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkItem where cbillcode='" + cworkcode + "'", Cmd) == 0)
        {
            string c_setitemname = GetDataString("select cWorkTypeName from " + dbname + "..EQ_WorkType where cWorkTypeCode='" + dtTheWork.Rows[0]["cWorkTypeCode"] + "'", Cmd);
            Cmd.CommandText = "insert into " + dbname + "..EQ_WorkItem(cbillcode,citemcode,citemname) values('" + cworkcode + "','1','" + c_setitemname + "')";
            Cmd.ExecuteNonQuery();
        }

        #endregion

        #region  //配件清单
        if (!(dtPart == null || dtPart.Rows.Count == 0))
        {
            for (int i = 0; i < dtPart.Rows.Count; i++)
            {
                string citemcode = GetBodyValue_DTData(dtPart, i, "citemcode");
                string cinvcode = GetBodyValue_DTData(dtPart, i, "cinvcode");
                string cwhcode = GetBodyValue_DTData(dtPart, i, "cwhcode");
                string fvalue = GetBodyValue_DTData(dtPart, i, "fvalue");
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkItem where cbillcode='" + cworkcode + "' and citemcode='" + citemcode + "'", Cmd) == 0)
                {
                    throw new Exception("配件[" + cinvcode + "]的项目号[" + citemcode + "]不存在");
                }

                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..EQ_WorkInventory where cbillcode='" + cworkcode + "' and cinvcode='" + cinvcode + "'", Cmd) > 0)
                {
                    throw new Exception("作业单中已经有配件[" + cinvcode + "]，作业单中配件不能重复");
                }
                string cJldw = "" + U8Operation.GetDataString("select cComUnitCode from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", Cmd);
                if (cwhcode == "") cwhcode = GetDataString("select cDefWareHouse from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", Cmd);
                Cmd.CommandText = "insert into " + dbname + @"..EQ_WorkInventory(cbillcode,citemcode,cinvcode,fValue,cwhcode,intRef,cJldw) 
                        values('" + cworkcode + "','" + citemcode + "','" + cinvcode + "',0" + fvalue + ",'" + cwhcode + "',2,'" + cJldw + "')";
                Cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region //更新设备故障原因
        string cObj = dtTheWork.Rows[0]["cObj"] + "";
        if (cObj != "" && dtTheWork.Rows[0]["cWorkTypeCode"] + "" == "1")  //维修作业
        {
            Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set trouble_code='" + troubleCode + "',cworkcode='" + cworkcode + @"',dTime=getdate()
                where cEQCode='" + cObj + "' and cToubleState='故障'";
            Cmd.ExecuteNonQuery();
            //记录维修故障码
            Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_WorkSub set trouble_code='" + troubleCode + @"' where cworkcode='" + cworkcode + "'";
            Cmd.ExecuteNonQuery();
        }
        #endregion

        #region //消息处理
        DataTable dtOneWork = GetSqlDataTable(@"select a.autoid,a.cObj,b.cpsn_name,c.cadmincode,a.cMaker,t.cWorkTypeName
                    from " + dbname + @"..EQ_Work a  left join " + dbname + @"..EQ_WorkType t on a.cWorkTypeCode=t.cWorkTypeCode
                        left join " + dbname + "..hr_hi_person b on a.cPlaner=b.cpsn_num left join " + dbname + @"..T_CC_EQ_Setting c on a.cObj=c.ceq_code and a.cWorkTypeCode=c.ctype
                    where a.cWorkCode='" + cworkcode + "'", "dtOneWork", Cmd);
        if (dtOneWork.Rows.Count > 0)
        {
            //设备管理员获得消息
            if (dtOneWork.Rows[0]["cadmincode"] + "" != "")
            {
                eq_send_message(Cmd, "开始“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”[" + dtOneWork.Rows[0]["cObj"] + "]",
                    "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + cworkcode + "]已经开始进行“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                    "作业单", dtOneWork.Rows[0]["autoid"] + "", dtOneWork.Rows[0]["cadmincode"] + "", dbname);
            }

            //上报人获得消息
            //获得上报人的 业务员编码
            string cup_maker_code = GetDataString(@"SELECT b.cPsn_Num FROM " + dbname + @"..UserHrPersonContro b inner join UFSystem..UA_User c on b.cUser_Id=c.cUser_Id 
                        where c.cUser_Name='" + dtOneWork.Rows[0]["cMaker"] + "'", Cmd);
            if (cup_maker_code != "")
            {
                eq_send_message(Cmd, "开始“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”[" + dtOneWork.Rows[0]["cObj"] + "]",
                    "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + cworkcode + "]已经开始进行“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                    "作业单", dtOneWork.Rows[0]["autoid"] + "", cup_maker_code, dbname);
            }

        }
        #endregion


        return true;
    }

    [WebMethod(Description = @"作业单（即开始维修）接口，方式二。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            worklist  第一列 cworkcode 代表作业单号清单；<br/>
                      第一列 troubcode 代表维修原因；<br/>
            注意：批量开工 不能指定 每个作业单的 配件
            ")] //作业单配件（配件、项目调整）
    public bool EQWork_WorkingList(string dbname, DataTable worklist, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            for (int i = 0; i < worklist.Rows.Count; i++)
            {
                string cworkcode = "" + worklist.Rows[i]["cworkcode"];
                string troubCode = "" + worklist.Rows[i]["troubcode"];
                EQ_Working(dbname, sCmd, cworkcode, null, null, troubCode, false);
            }
            sCmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod(Description = @"故障原因判定修改 接口  <br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            ceq_code 设备编号；<br/>
            cworkcode 作业单编号；<br/>
            troublecode 故障原因码 可以空字符串，空时只修改处理措施<br/>
            cmemo 处理措施 可以空字符串，空时只修改故障原因<br/>")]
    public string EQWork_ToubleModify_json(string dbname, string ceq_code, string cworkcode, string troublecode, string cdefine14, string cmemo)
    {
        if (troublecode == "" && cmemo == "") return "故障原因和处理措施不能同时为空";

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            return "数据库连接失败！";
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            if (GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cworkcode='" + cworkcode + "'", Cmd) == 0)
                throw new Exception("作业单不存在");

            //判断是符合修改状态****************************************
            if (troublecode != "")
            {
                //判断是否存在故障原因
                if (GetDataInt("select count(*) from " + dbname + @"..EQ_TroubleType where cTroubleTypeCode='" + troublecode + "'", Cmd) == 0)
                    throw new Exception("故障原因码不存在");

                Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set trouble_code='" + troublecode + "',cworkcode='" + cworkcode + "' where cEQCode='" + ceq_code + "'";
                Cmd.ExecuteNonQuery();
                //记录维修故障码
                Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_WorkSub set trouble_code='" + troublecode + @"' where cworkcode='" + cworkcode + "'";
                Cmd.ExecuteNonQuery();
            }

            if (cmemo != "")
            {
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set cmemo='" + cmemo + "',cdefine14='" + cdefine14 + "' where cworkcode='" + cworkcode + "'";
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region //作业单处理完毕(或维修完毕）

    [WebMethod(Description = @"作业单  维修完工 接口。<br/>
                   dbname 代表需要获得计量单位的账套库名称；<br/>
                   worklist 代表作业单号清单，里面第一列列名：cworkcode；<br/>
                            第二列列名：cmemo；维修处理措施简要<br/>
                   cUserName 登录人的姓名；<br/>
            ")] //作业单 维修完毕上报
    public bool EQWork_WorkedOFF(string dbname, DataTable worklist, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        if (!worklist.Columns.Contains("cmemo")) throw new Exception("请传入 维修处理过程信息！");
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            for (int i = 0; i < worklist.Rows.Count; i++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", Cmd) == 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]不存在");

                //判断
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and tFactStart is not null", Cmd) == 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]未上报开始处理");

                //判断是否 已经结束
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and intCheck=1", Cmd) > 0)
                    throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]已经验收");

                Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtFactEnd=CONVERT(varchar(10),getdate(),120),tFactEnd=right(CONVERT(varchar(20),getdate(),120),8),
                        cMemo='" + worklist.Rows[i]["cmemo"] + @"'
                    where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
                //故障上报回写数据处理
                Cmd.CommandText = "delete from " + dbname + @"..t_cqcc_WorkOffInfo where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = @"
insert into " + dbname + @"..t_cqcc_WorkOffInfo(cWorkCode, TroubleType, TroubleReason, TroublePosition, Situation,notes,content) values (
'" + worklist.Rows[i]["cworkcode"] + @"','" + worklist.Rows[i]["TroubleType"] + @"','" + worklist.Rows[i]["TroubleReason"] + @"',
'" + worklist.Rows[i]["TroublePosition"] + @"','" + worklist.Rows[i]["Situation"] + @"','" + worklist.Rows[i]["Notes"] + @"','" + worklist.Rows[i]["Content"] + @"'
)
";
                Cmd.ExecuteNonQuery();
                #region //消息处理
                // 华青设备消息调整
                DataTable dtOneWork = GetSqlDataTable(@"select a.autoid,a.cObj,b.cpsn_name,c.cadmincode,a.cMaker,t.cWorkTypeName
                    from " + dbname + @"..EQ_Work a left join " + dbname + @"..EQ_WorkType t on a.cWorkTypeCode=t.cWorkTypeCode 
                        left join " + dbname + "..hr_hi_person b on a.cPlaner=b.cpsn_num left join " + dbname + @"..T_CC_EQ_Setting c on a.cObj=c.ceq_code and a.cWorkTypeCode=c.ctype
                    where a.cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "dtOneWork", Cmd);
                if (dtOneWork.Rows.Count > 0)
                {
                    //设备管理员获得消息
                    //if (dtOneWork.Rows[0]["cadmincode"] + "" != "")
                    //{
                    //    eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”完毕[" + dtOneWork.Rows[0]["cObj"] + "]",
                    //        "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]已经“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”结束，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                    //        "作业单", dtOneWork.Rows[0]["autoid"] + "", dtOneWork.Rows[0]["cadmincode"] + "", dbname);
                    //}

                    //上报人获得消息
                    //获得上报人的 业务员编码
                    string cup_maker_code = GetDataString(@"SELECT b.cPsn_Num FROM " + dbname + @"..UserHrPersonContro b inner join UFSystem..UA_User c on b.cUser_Id=c.cUser_Id 
                        where c.cUser_Name='" + dtOneWork.Rows[0]["cMaker"] + "'", Cmd);
                    if (cup_maker_code != "")
                    {
                        //eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”完毕[" + dtOneWork.Rows[0]["cObj"] + "]",
                        //    "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]已经“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”结束，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "]",
                        //    "作业单", dtOneWork.Rows[0]["autoid"] + "", cup_maker_code, dbname);
                        eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”完毕[" + dtOneWork.Rows[0]["cObj"] + "]",
                            dtOneWork.Rows[0]["cObj"] + "设备已" + dtOneWork.Rows[0]["cWorkTypeName"]+"完毕,请验收!", "作业单", dtOneWork.Rows[0]["autoid"] + "", cup_maker_code, dbname);
                    }

                }
                #endregion
            }

            //判断是否自动验收
            if (GetDataString("select cvalue from " + dbname + "..T_Parameter where cPid='mes_eq_work_auto_close'", Cmd).ToLower().CompareTo("true") == 0)
            {
                EQWork_WorkClose(Cmd, dbname, worklist, cUserName);
            }

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

    [WebMethod]
    public string EQWork_WorkedOFF_json(string dbname, string jsnworklist, string cUserName)
    {
        DataTable worklist = null;
        if (jsnworklist != "") worklist = VendorIO.JsonToDataTable(jsnworklist);

        try
        {
            EQWork_WorkedOFF(dbname, worklist, cUserName);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }
    #endregion

    #region //作业单作业 完工验收
    [WebMethod(Description = @"作业单 验收接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            worklist 代表作业单号清单，里面只有一列列名：cworkcode；<br/>
            cUserName 登录人的姓名；<br/>
            ")] //作业单 维修完毕验收
    public bool EQWork_WorkedClose(string dbname, DataTable worklist, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            EQWork_WorkClose(Cmd, dbname, worklist, cUserName);

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
    private void EQWork_WorkClose(SqlCommand Cmd, string dbname, DataTable worklist, string cUserName)
    {
        for (int i = 0; i < worklist.Rows.Count; i++)
        {
            DataTable dtTheWork = GetSqlDataTable("select cObj,cWorkTypeCode,intObjType from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "dtTheWork", Cmd);
            if (dtTheWork.Rows.Count == 0) throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]不存在");
            if (dtTheWork.Rows[0]["intObjType"] + "" != "2") throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]对象类型不是“设备”");

            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", Cmd) == 0)
                throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]不存在");

            //判断
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and tFactEnd is not null", Cmd) == 0)
                throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]未上报 处理完毕");

            //判断是否 已经结束
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_Work where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "' and intCheck=1", Cmd) > 0)
                throw new Exception("作业单[" + worklist.Rows[i]["cworkcode"] + "]已经验收");
            string qualified = worklist.Rows[i]["qualified"] + "";
            if (qualified == "不合格")
            {
                #region // 不合格消息处理
                DataTable UnqualifiedDt = GetSqlDataTable(@"
select b.cWorkTypeName,a.cObj,cPlaner,a.AutoID from "+dbname+@"..EQ_Work a 
inner join " + dbname + @"..EQ_WorkType b on a.cWorkTypeCode = b.cWorkTypeCode
where a.cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "tb", Cmd);
                eq_send_message(Cmd, "“" + UnqualifiedDt.Rows[0]["cWorkTypeName"] + "”验收[" + UnqualifiedDt.Rows[0]["cObj"] + "]",
                        UnqualifiedDt.Rows[0]["cObj"] + "设备依旧存在故障,请重新维修!",
                        "作业单", UnqualifiedDt.Rows[0]["autoid"] + "", UnqualifiedDt.Rows[0]["cPlaner"] + "", dbname);
                #endregion
                // 不合格打回到完工上报阶段
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtFactEnd=null,tFactEnd=null
                    where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
                DataTable dt = U8Operation.GetSqlDataTable(@"select * from " + dbname + @"..t_cqcc_WorkOffInfo where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "tb", Cmd);
                if (dt.Rows.Count > 0)
                {
                    //故障上报回写数据处理
                    Cmd.CommandText = "delete from " + dbname + @"..t_cqcc_WorkOffInfo where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                    Cmd.ExecuteNonQuery();
                    string b = Environment.NewLine + "   上次维修验收不合格,请重新填写:";
                    Cmd.CommandText = @"
insert into " + dbname + @"..t_cqcc_WorkOffInfo(cWorkCode, TroubleType, TroubleReason, TroublePosition, Situation,notes,content) values (
'" + dt.Rows[0]["cworkcode"] + @"','" + dt.Rows[0]["TroubleType"] + @"','" + dt.Rows[0]["TroubleReason"]+b + @"',
'" + dt.Rows[0]["TroublePosition"] + b + @"','" + dt.Rows[0]["Situation"] + b + @"','" + dt.Rows[0]["Notes"] + b + @"','" + dt.Rows[0]["Content"] + b + @"'
)
";
                    Cmd.ExecuteNonQuery();
                }
                
                return;
            }
            if (string.IsNullOrEmpty(qualified))
            {
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtCheck=getdate(),intCheck=1,cCheck='通过" + cUserName + @"验收'
                    where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + @"..EQ_Work set dtCheck=getdate(),intCheck=1,cCheck='"+qualified+@"'
                    where cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'";
                Cmd.ExecuteNonQuery();
            }
            

            #region //添加故障记录
            if (dtTheWork.Rows[0]["cWorkTypeCode"] + "" == "1")  //维修性作业单
            {
                string thetime = GetDataString("select CONVERT(varchar(20),getdate(),120)", Cmd);
                DataTable dtStartEQ = GetSqlDataTable("select convert(varchar(20),trouble_starttime,120) trouble_starttime,trouble_code from " + dbname + @"..T_CC_EQ_State 
                        where cEQCode='" + dtTheWork.Rows[0]["cObj"] + "' and cToubleState ='故障'", "dtStartEQ", Cmd);
                if (dtStartEQ.Rows.Count > 0)
                {
                    //添加故障记录
                    eq_work_trouble_record(Cmd, dbname, "" + dtTheWork.Rows[0]["cObj"], "" + dtStartEQ.Rows[0]["trouble_starttime"], thetime, cUserName,
                        "" + dtStartEQ.Rows[0]["trouble_code"], worklist.Rows[i]["cworkcode"] + "");

                    //更换故障状态
                    Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cToubleState='正常',cworkcode='',dTime='" + thetime + "',trouble_starttime='" + thetime + "',dUpMaker='" + cUserName + @"',
                            trouble_code='',iTroubleLevel=0 where cEQCode='" + dtTheWork.Rows[0]["cObj"] + "' and cToubleState='故障'";
                    Cmd.ExecuteNonQuery();

                    //是否自动开机

                }
            }
            #endregion

            #region //消息处理
            DataTable dtOneWork = GetSqlDataTable(@"select a.autoid,a.cObj,b.cpsn_name,c.cadmincode,a.cMaker,cplaner,t.cWorkTypeName
                    from " + dbname + @"..EQ_Work a left join " + dbname + @"..EQ_WorkType t on a.cWorkTypeCode=t.cWorkTypeCode 
                        left join " + dbname + "..hr_hi_person b on a.cPlaner=b.cpsn_num left join " + dbname + @"..T_CC_EQ_Setting c on a.cObj=c.ceq_code and a.cWorkTypeCode=c.ctype
                    where a.cWorkCode='" + worklist.Rows[i]["cworkcode"] + "'", "dtOneWork", Cmd);
            if (dtOneWork.Rows.Count > 0)
            {
                //设备管理员获得消息
                if (dtOneWork.Rows[0]["cadmincode"] + "" != "")
                {
                    eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”验收[" + dtOneWork.Rows[0]["cObj"] + "]",
                        "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”已验收，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "],验收人[" + cUserName + "]",
                        "作业单", dtOneWork.Rows[0]["autoid"] + "", dtOneWork.Rows[0]["cadmincode"] + "", dbname);
                }

                //维修员获得消息
                if (dtOneWork.Rows[0]["cplaner"] + "" != "")
                {
                    eq_send_message(Cmd, "“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”验收[" + dtOneWork.Rows[0]["cObj"] + "]",
                        "设备[" + dtOneWork.Rows[0]["cObj"] + "]作业单号[" + worklist.Rows[i]["cworkcode"] + "]“" + dtOneWork.Rows[0]["cWorkTypeName"] + "”已验收，处理人[" + dtOneWork.Rows[0]["cpsn_name"] + "],验收人[" + cUserName + "]",
                        "作业单", dtOneWork.Rows[0]["autoid"] + "", dtOneWork.Rows[0]["cplaner"] + "", dbname);
                }

            }

            #endregion
        }
    }

    [WebMethod]
    public string EQWork_WorkedClose_json(string dbname, string jsnworklist, string cUserName)
    {
        DataTable worklist = null;
        if (jsnworklist != "") worklist = VendorIO.JsonToDataTable(jsnworklist);

        try
        {
            EQWork_WorkedClose(dbname, worklist, cUserName);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }
    #endregion

    #region //设备开机停机
    [WebMethod(Description = @"设备停机 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号ID；<br/>
            stopcode  停机原因码 <br/>
            cUserName 登录人的姓名；<br/>
            ")] //设备停机
    public string EQWork_RunStop_json(string dbname, string eq_no, string stopcode, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            if (stopcode + "" == "") throw new Exception("停机原因必须输入");
            eq_work_stop(sCmd, dbname, eq_no, stopcode, cUserName);
            sCmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod(Description = @"设备开机 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号ID；<br/>
            cUserName 登录人的姓名；<br/>
            ")] //设备开机
    public string EQWork_RunStart_json(string dbname, string eq_no, string cUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            eq_work_start(sCmd, dbname, eq_no, cUserName, true);
            sCmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    //设备开机处理
    private void eq_work_start(SqlCommand cmd, string dbname, string eq_no, string cUserName, bool bShowMessage)
    {
        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_EQData where cEQCode='" + eq_no + "'", cmd) == 0)
            throw new Exception("设备[" + eq_no + "]不存在");

        //判断是否停机故障
        if (GetDataInt("select count(*) from  " + dbname + @"..T_CC_EQ_State where cEQCode='" + eq_no + "' and iTroubleLevel>1", cmd) > 0)
        {
            if (bShowMessage)  //提示消息
            {
                throw new Exception("设备[" + eq_no + "]属于停机故障，不能开机");
            }
            else
            {
                return;
            }
        }

        string thetime = GetDataString("select CONVERT(varchar(20),getdate(),120)", cmd);
        if (GetDataInt("select count(*) from  " + dbname + @"..T_CC_EQ_State where cEQCode='" + eq_no + "'", cmd) == 0)
        {
            cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_State(cEQCode, cRunState,cToubleState, iTroubleLevel, dTime, dUpMaker, state_starttime) 
                values('" + eq_no + "','停机','正常',0,getdate(),'" + cUserName + "',getdate())";
            cmd.ExecuteNonQuery();
        }
        else
        {
            //添加停机记录

            DataTable dtStartEQ = GetSqlDataTable("select convert(varchar(20),state_starttime,120) state_starttime,stop_code,cToubleState from " + dbname + @"..T_CC_EQ_State 
                where cEQCode='" + eq_no + "' and cRunState ='停机'", "dtStartEQ", cmd);
            if (dtStartEQ.Rows.Count > 0)
            {
                //机器停止记录
                eq_work_stop_record(cmd, dbname, eq_no, dtStartEQ.Rows[0]["stop_code"] + "", dtStartEQ.Rows[0]["state_starttime"] + "", thetime, cUserName);
            }
        }


        cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cRunState='运行',dTime=getdate(),state_starttime='" + thetime + "',dUpMaker='" + cUserName + @"',stop_code='' 
            where cEQCode='" + eq_no + "' and cRunState='停机'";
        cmd.ExecuteNonQuery();
    }

    //设备停机处理  stopcause  停机原因
    private void eq_work_stop(SqlCommand cmd, string dbname, string eq_no, string stopcause, string cUserName)
    {
        //stopcause  停机原因
        //获得状态开始时间信息
        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_EQData where cEQCode='" + eq_no + "'", cmd) == 0)
            throw new Exception("设备[" + eq_no + "]不存在");

        string thetime = GetDataString("select CONVERT(varchar(20),getdate(),120)", cmd);
        DataTable dtStartEQ = GetSqlDataTable("select convert(varchar(20),state_starttime,120) state_starttime,cRunState,cToubleState from " + dbname + @"..T_CC_EQ_State 
            where cEQCode='" + eq_no + "' and cRunState ='运行'", "dtStartEQ", cmd);
        if (dtStartEQ.Rows.Count > 0)
        {
            string runtypecode = "";
            if (dtStartEQ.Rows[0]["cToubleState"] + "" == "故障")
            {
                runtypecode = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_runtrouble_type_code'", cmd);
                if (runtypecode == "") throw new Exception("请维护参数【设备运行记录-设备带故障运行时的运行状态代码】");
            }
            else
            {
                runtypecode = GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_eq_run_type_code'", cmd);
                if (runtypecode == "") throw new Exception("请维护参数【设备运行记录-正常运行时的运行状态代码】");
            }
            if (GetDataInt("select count(*) from " + dbname + "..EQ_RunType where cRunTypeCode='" + runtypecode + "'", cmd) == 0)
                throw new Exception("设备运行状态码【" + runtypecode + "】不存在");

            //添加运行记录
            eq_work_run_record(cmd, dbname, eq_no, dtStartEQ.Rows[0]["state_starttime"] + "", thetime, cUserName, runtypecode);

            //更换状态
            cmd.CommandText = "update " + dbname + @"..T_CC_EQ_State set cRunState='停机',dTime='" + thetime + "',state_starttime='" + thetime + "',dUpMaker='" + cUserName + @"',
                stop_code='" + stopcause + "' where cEQCode='" + eq_no + "' and cRunState='运行'";
            cmd.ExecuteNonQuery();
        }
    }
    #endregion

    #region//添加运行记录
    private void eq_work_run_record(SqlCommand cmd, string dbname, string eq_no, string startdateandtime, string enddateandtime, string cusername, string runtypecode)
    {
        cmd.CommandText = "insert into " + dbname + @"..EQ_Run(cRunCode,cMaker,dtDate,cChecker,dtAudit,VT_ID) 
            values('','" + cusername + "',convert(varchar(10),getdate(),120),'" + cusername + "',convert(varchar(10),getdate(),120),30272)";
        cmd.ExecuteNonQuery();
        string w_runid = "" + GetDataString("select IDENT_CURRENT( '" + dbname + @"..EQ_Run' )", cmd);
        string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select scope_identity()");
        if (w_runid != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

        string c_runcode = GetDataString("select 'R'+right('000000000'+cast(" + w_runid + " as varchar(10)),10)", cmd);
        cmd.CommandText = "update " + dbname + @"..EQ_Run set cRunCode='" + c_runcode + "' where AutoID=0" + w_runid;
        cmd.ExecuteNonQuery();


        cmd.CommandText = "insert into " + dbname + @"..EQ_RunDetail(intLine,cRunCode,intObjType,cObj,dtStart,tStart,dtEnd,tEnd,fTimeNum,cRunTypeCode) 
            values(1,'" + c_runcode + "',2,'" + eq_no + "',left('" + startdateandtime + "',10),right('" + startdateandtime + "',8),left('" + enddateandtime + @"',10),
                right('" + enddateandtime + "',8),round(DATEDIFF(MINUTE,'" + startdateandtime + "','" + enddateandtime + "'),0),'" + runtypecode + "')";
        cmd.ExecuteNonQuery();
        // 查询设备关联的作业单
        string cWorkCode=UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, @"select cWorkCode from " + dbname + @"..EQ_Work where cObj='" + eq_no + "' and intObjType=2 and cWorkTypeCode='1' and ISNULL(intCheck,0)<>1 and isnull(cCloser,'')=''");
        string eq_eqRun_WorkCodecDefine = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select cvalue from " + dbname + "..t_parameter where cpid = 'eq_eqRun_WorkCodecDefine'");
        if (string.IsNullOrEmpty(eq_eqRun_WorkCodecDefine) || string.IsNullOrEmpty(cWorkCode))
        {
            return;
        }
        cmd.CommandText = "update " + dbname + @"..EQ_Run set " + eq_eqRun_WorkCodecDefine + "='" + cWorkCode + "' where AutoID=0" + w_runid;
        cmd.ExecuteNonQuery();

    }

    [WebMethod(Description = @"运行记录 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号ID；<br/>
            startdateandtime 开始时间<br/>
            enddateandtime 结束时间<br/>
            cusername 登录人的姓名；<br/>
            runtypecode 设备运行时的运行代码
            ")]
    public bool EQWork_Run_Recork(string dbname, string eq_no, string startdateandtime, string enddateandtime, string cusername, string runtypecode)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            eq_work_run_record(sCmd, dbname, eq_no, startdateandtime, enddateandtime, cusername, runtypecode);
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region//停机记录
    private void eq_work_stop_record(SqlCommand cmd, string dbname, string eq_no, string stopcode, string starttime, string endtime, string cusername)
    {
        cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_StopRecord( cEQCode, starttime, endtime, ihours, stop_code, cmaker, ddate) 
            values( '" + eq_no + "', '" + starttime + "', '" + endtime + "', round(DATEDIFF(MINUTE,'" + starttime + "','" + endtime + "'),0), '" + stopcode + "', '" + cusername + "', convert(varchar(10),getdate(),120))";
        cmd.ExecuteNonQuery();
    }
    [WebMethod(Description = @"停机记录 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号ID；<br/>
            stopcode  设备停机原因
            starttime 开始时间<br/>
            endtime 结束时间<br/>
            cusername 登录人的姓名；<br/>
            ")]
    public bool EQWork_Stop_Recork(string dbname, string eq_no, string stopcode, string starttime, string endtime, string cusername)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            eq_work_stop_record(sCmd, dbname, eq_no, stopcode, starttime, endtime, cusername);
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region//添加故障记录-- todo whf
    private void eq_work_trouble_record(SqlCommand cmd, string dbname, string eq_no, string startdateandtime, string enddateandtime, string cusername, string troubletypecode, string cworkcode)
    {
        cmd.CommandText = "insert into " + dbname + @"..EQ_Trouble(cTroublecode,cMaker,dtDate,cChecker,dtAudit,VT_ID) 
            values('','" + cusername + "',convert(varchar(10),getdate(),120),'" + cusername + "',convert(varchar(10),getdate(),120),30270)";
        cmd.ExecuteNonQuery();
        string w_triid = "" + GetDataString("select IDENT_CURRENT( '" + dbname + @"..EQ_Trouble' )", cmd);
        string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(cmd, "select scope_identity()");
        if (w_triid != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

        string c_tricode = GetDataString("select 'T'+right('000000000'+cast(" + w_triid + " as varchar(10)),10)", cmd);
        cmd.CommandText = "update " + dbname + @"..EQ_Trouble set cTroublecode='" + c_tricode + "' where AutoID=0" + w_triid;
        cmd.ExecuteNonQuery();
        DataTable WorkOffInfoDt = UCGridCtl.SqlDBCommon.GetDataFromDB(cmd, "select * from " + dbname + "..t_cqcc_WorkOffInfo where cWorkCode='" + cworkcode + "'");
        string TroubleReason = "";
        string TroublePosition = "";
        string Situation = "";
        if (WorkOffInfoDt.Rows.Count > 0 )
        {
            if(WorkOffInfoDt.Rows[0]["TroubleType"] + "" != "")
            {
                troubletypecode = (WorkOffInfoDt.Rows[0]["TroubleType"] + "").Split('-')[0];
            }
            TroubleReason = WorkOffInfoDt.Rows[0]["TroubleReason"]+"";
            TroublePosition = WorkOffInfoDt.Rows[0]["TroublePosition"] + "";
            Situation = WorkOffInfoDt.Rows[0]["Situation"] + "";
        }

        string cMemo = GetDataString("select cDefine14 from " + dbname + @"..EQ_Work where cWorkCode = '" + cworkcode + "'", cmd);
        cmd.CommandText = "insert into " + dbname + @"..EQ_TroubleDetail(intLine,cTroubleCode,cEQCode,dtStart,tStart,dtEnd,tEnd,FTrouble,cTroubleTypeCode,dtOver,intFlag,intOver
,cCause,cPart,cWorkThing,ctroubleMemo ) 
            values(1,'" + c_tricode + "','" + eq_no + "',left('" + startdateandtime + "',10),right('" + startdateandtime + "',8),left('" + enddateandtime + @"',10),
                right('" + enddateandtime + "',8),round(DATEDIFF(MINUTE,'" + startdateandtime + "','" + enddateandtime + "'),0),'" + troubletypecode + @"',
                left('" + enddateandtime + @"',10),3,2
,'" + TroubleReason + "','" + TroublePosition + "','" + Situation + "','" + cMemo + "')";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"update " + dbname + @"..t_cc_eq_worksub set trouble_code='" + troubletypecode + "' where cworkcode='" + cworkcode + "'";
        cmd.ExecuteNonQuery();
        if (cworkcode != "")  //添加设备故障记录与作业单的关联关系
        {
            cmd.CommandText = "delete from " + dbname + @"..EQ_WorkTroubleList where cBillCode='" + cworkcode + "'";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "insert into " + dbname + @"..EQ_WorkTroubleList(cBillCode,intLine,cTroubleCode) 
                values('" + cworkcode + "',1,'" + c_tricode + "')";
            cmd.ExecuteNonQuery();
        }
    }
    [WebMethod(Description = @"停机记录 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号ID；<br/>
            startdateandtime 开始时间<br/>
            enddateandtime 结束时间<br/>
            cusername 登录人的姓名；<br/>
            troubletypecode 设备故障代号<br/>
            cworkcode 作业单编号<br/>
            ")]
    public bool EQWork_Trouble_Recork(string dbname, string eq_no, string startdateandtime, string enddateandtime, string cusername, string troubletypecode, string cworkcode)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            throw new Exception("数据库连接失败！");
        }
        SqlCommand sCmd = Conn.CreateCommand();
        sCmd.Transaction = Conn.BeginTransaction();

        try
        {
            eq_work_trouble_record(sCmd, dbname, eq_no, startdateandtime, enddateandtime, cusername, troubletypecode, cworkcode);
            return true;
        }
        catch (Exception ex)
        {
            sCmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region//消息发送
    private void eq_send_message(SqlCommand cmd, string title, string msgtext, string souretype, string sourceid, string recever, string db_name)
    {
        int imode = GetDataInt("select isnull(max(cValue),0) from " + db_name + @"..T_Parameter where cpid='mes_eq_msg_send_by_mode'", cmd);
        string cMobile = "";
        if (imode == 1) cMobile = GetDataString("select cPsnMobilePhone from " + db_name + @"..hr_hi_person where cPsn_Num='recever'", cmd);

        if (imode == 0 || (imode == 1 && cMobile == ""))
        {
            cmd.CommandText = "insert into " + db_name + @"..T_CC_EQ_Message(t_msgtitle,t_msgtext,t_souretype,t_soureid,t_time,t_isread,t_received_personcode) 
                values('" + title + "','" + msgtext + "','" + souretype + "'," + sourceid + @",getdate(),0,'" + recever + "')";
            cmd.ExecuteNonQuery();
        }
        else if (imode == 1 && cMobile != "")
        {
            cmd.CommandText = "insert into " + db_name + @"..T_CC_Send_Message(t_sendtype,t_receive_psn,t_content,t_sendstate,t_createtime,t_createsystem,t_createpsn,t_otherparam) 
                values(1,'" + recever + "','" + msgtext + "',0,getdate(),'MES设备','作业单','" + sourceid + "')";
            cmd.ExecuteNonQuery();
        }
    }

    [WebMethod(Description = @"消息已读 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            msg_id 消息ID；<br/>
            ")] //消息读取
    public string EQWork_Message_read_json(string db_name, string msg_id)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            return "数据库连接失败！";
        }
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            Cmd.CommandText = "update " + db_name + @"..T_CC_EQ_Message set t_isread=1,t_readtime=getdate() where t_msgid=0" + msg_id;
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return "";
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            return ex.Message;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    #endregion

    #region  //设备盘点
    [WebMethod(Description = @"设备盘点新增 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号；<br/>
            ctaskno 任务号；<br/>
            dept 盘点时设备所在部门，可为空字符串；<br/>
            personcode 盘点时设备的保管人员，可为空字符串；<br/>
            eqname 盘点时设备的名称，可为空字符串，若盘点时设备未建档，需要输入设备名称；<br/>
            cusername 盘点时用户登录的姓名；
            ")] //盘点增行
    public string EQ_CheckVouch_add_json(string dbname, string eq_no, string dept, string personcode, string eqname, string ctaskno, string cusername)
    {
        if (cusername == "") return VendorIO.SendResult("0", "用户不能为空！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) return VendorIO.SendResult("0", "数据库连接失败！");

        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            //判断设备是否存在
            DataTable dtEQNo = GetSqlDataTable(@"select a.cEQCode,a.cEQName,a.cxh,a.cgg,a.cDepCode,b.cDepName,c.sKeeper,d.cPsn_Name  
                from " + dbname + @"..EQ_EQData a left join " + dbname + @"..Department b on a.cDepCode=b.cDepCode
                left join (select sAssetNum,sKeeper
                    from " + dbname + @"..fa_Cards a inner join (select MAX(scardid) scardid from " + dbname + @"..fa_Cards group by scardnum) b on a.scardid=b.scardid
                ) c on isnull(a.cAssetNum,a.cEQCode)=c.sAssetNum
                left join " + dbname + @"..hr_hi_person d on c.sKeeper=d.cPsn_Num
                where cEQCode='" + eq_no + "' or cSysBarCode='" + eq_no + "'", "dtEQNo", Cmd);
            if (dtEQNo.Rows.Count == 0)
            {
                if (eqname + "" == "") throw new Exception("未找到设备信息");
            }
            else
            {
                eq_no = dtEQNo.Rows[0]["cEQCode"] + "";
                eqname = dtEQNo.Rows[0]["cEQName"] + " " + dtEQNo.Rows[0]["cxh"];
                if (dept == "") dept = dtEQNo.Rows[0]["cDepCode"] + "";
                if (personcode == "") personcode = dtEQNo.Rows[0]["sKeeper"] + "";
            }

            //判断任务是否存在或结束
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_EQ_CheckTaskMain where ctaskno='" + ctaskno + "' and isnull(ccloser,'')=''", Cmd) == 0)
                throw new Exception("任务号不存在 或 已经终止任务");
            //判断此设备是否已经盘点
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_EQ_CheckTaskDetail where ctaskno='" + ctaskno + "' and ceqno='" + eq_no + "' and isnull(cchecker,'')<>''", Cmd) > 0)
                throw new Exception("本设备已经盘点");

            //新增盘点数据
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_EQ_CheckTaskDetail where ctaskno='" + ctaskno + "' and ceqno='" + eq_no + "'", Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_EQ_CheckTaskDetail(ctaskno,ceqno,cchecker,ccheckdate,ceqname,cq_dept,cq_personcode) 
                values('" + ctaskno + "','" + eq_no + "','" + cusername + "',getdate(),'" + eqname + "','" + dept + "','" + personcode + "')";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + @"..T_CC_EQ_CheckTaskDetail set cchecker='" + cusername + "',ccheckdate=getdate(),ceqname='" + eqname + @"',
                    cq_dept='" + dept + "',cq_personcode='" + personcode + "' where ctaskno='" + ctaskno + "' and ceqno='" + eq_no + "'";
                Cmd.ExecuteNonQuery();
            }
            Cmd.Transaction.Commit();

            //return VendorIO.SendResult("1", "成功！");
            string cjsondata = VendorIO.DataTableToJson(dtEQNo);
            string r_msg = VendorIO.SendResult("1", cjsondata);
            return r_msg;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            return VendorIO.SendResult("0", ex.Message);
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    [WebMethod(Description = @"设备查询 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号
            ")] //盘点查询
    public string EQ_CheckVouch_Query_json(string dbname, string eq_no)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) return VendorIO.SendResult("0", "数据库连接失败！");

        SqlCommand Cmd = Conn.CreateCommand();

        try
        {
            //判断设备是否存在
            DataTable dtEQNo = GetSqlDataTable(@"select a.cEQCode,a.cEQName,a.cxh,a.cgg,a.cDepCode,b.cDepName,c.sKeeper,d.cPsn_Name  
                from " + dbname + @"..EQ_EQData a left join " + dbname + @"..Department b on a.cDepCode=b.cDepCode
                left join (select sAssetNum,sKeeper
                    from " + dbname + @"..fa_Cards a inner join (select MAX(scardid) scardid from " + dbname + @"..fa_Cards group by scardnum) b on a.scardid=b.scardid
                ) c on isnull(a.cAssetNum,a.cEQCode)=c.sAssetNum
                left join " + dbname + @"..hr_hi_person d on c.sKeeper=d.cPsn_Num
                where cEQCode='" + eq_no + "' or cSysBarCode='" + eq_no + "'", "dtEQNo", Cmd);
            //if (dtEQNo.Rows.Count == 0) throw new Exception("未找到记录");

            string cjsondata = VendorIO.DataTableToJson(dtEQNo);
            string r_msg = VendorIO.SendResult("1", cjsondata);
            return r_msg;
        }
        catch (Exception ex)
        {
            return VendorIO.SendResult("0", ex.Message);
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }


    [WebMethod(Description = @"设备盘点删除 接口。<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            eq_no 设备号；<br/>
            ctaskno 任务号；<br/>
            ")] //盘点删行
    public string EQ_CheckVouch_del_json(string dbname, string eq_no, string ctaskno, string cusername)
    {
        if (cusername == "") return VendorIO.SendResult("0", "用户不能为空！");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null) return VendorIO.SendResult("0", "数据库连接失败！");

        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            //判断任务是否存在或结束
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_EQ_CheckTaskMain where ctaskno='" + ctaskno + "' and isnull(ccloser,'')=''", Cmd) == 0)
                throw new Exception("任务号不存在 或 已经终止任务");
            //判断此设备是否已经盘点
            if (GetDataInt("select count(*) from " + dbname + @"..T_CC_EQ_CheckTaskDetail where ctaskno='" + ctaskno + "' and ceqno='" + eq_no + "'", Cmd) == 0)
                throw new Exception("本设备未盘点");

            //新增盘点数据
            Cmd.CommandText = "delete from " + dbname + @"..T_CC_EQ_CheckTaskDetail where ctaskno='" + ctaskno + "' and ceqno='" + eq_no + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            return VendorIO.SendResult("1", "删除成功");
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            return VendorIO.SendResult("0", ex.Message);
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }

    #endregion

    #endregion

    #region //MES自动更新
    [WebMethod(Description = "下载服务器站点文件，传递文件相对路径")]
    public byte[] DownloadMESFile(string strFilePath)
    {
        FileStream fs = null;
        string CurrentUploadFolderPath = HttpContext.Current.Server.MapPath("mesfiles");

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

    [WebMethod(Description = "获得更新文件版本")]
    public string GetMESFileVersion()
    {
        //比对版本
        string boardversion = System.Configuration.ConfigurationSettings.AppSettings["boardVersion"];

        return boardversion;
    }

    [WebMethod(Description = "获得更新文件清单")]
    public string[] GetMESFileList()
    {
        string CurrentUploadFolderPath = HttpContext.Current.Server.MapPath("mesfiles");
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

    #endregion

    #region 沃德卡制卡
    [WebMethod]  //沃德卡制卡
    public string CardToCard(System.Data.DataTable FormData, System.Data.DataTable PCData
        , string dbname, string cUserName, string cLogDate, string SheetID)
    {
        //来源流转卡入库
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("授权序列号错误");

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
            #region 数据准备
            //为DataTable 建立索引
            FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
            string[] txtdata = null;  //临时取数
            //仓库
            txtdata = GetTextsFrom_FormData(FormData, "txt_cwhcode");
            string cwhcode = txtdata[2];
            //批号
            string cc_batch = GetTextsFrom_FormData_Text(FormData, "txt_cbatch");
            // 存货
            string cc_invcode = GetTextsFrom_FormData_Text(FormData, "txt_cinvcode");
            //数量
            string[] s_txt_iquantity = GetTextsFrom_FormData(FormData, "txt_iquantity");
            string rkQty = s_txt_iquantity[3];
            // 流转卡号
            string src_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
            // 固定数据库
            Cmd.CommandText = " USE [" + dbname + "] ";
            Cmd.ExecuteNonQuery();
            #endregion
            // 来源流转卡入库
            string rksId = CardToCard_RK(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
            // 排产
            string c_mo_day_planid = CardToCard_PC(PCData, cUserName, Cmd);
            // 出库
            string rds11Id = CardToCard_CK(cUserName, cLogDate, cc_invcode, cwhcode, cc_batch, "201", rkQty, PCData.Rows[0]["modid"] + "", Cmd);
            // 制卡
            string t_card_no = CardToCard_ZK(cUserName, cLogDate, c_mo_day_planid, rkQty, cc_batch, Cmd);
            // 记录关联关系
            Cmd.CommandText = @"insert into t_cqcc_cardToCardDetail
(thisQty,src_t_card_no,rksId,moDayPlanId,rds11Id,new_t_card_no)
values (" + rkQty + @",'" + src_cardno + @"'," + rksId + @"," + c_mo_day_planid + @"," + rds11Id + @",'" + t_card_no + @"')
";
            Cmd.ExecuteNonQuery();
            Cmd.Transaction.Commit();
            return t_card_no;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            VendorIO.WriteDebug(ex.Message, "whfdebug");
            VendorIO.WriteDebug(ex.StackTrace, "whfdebug");
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
    }
    /// <summary>
    /// 卡制卡-入库
    /// </summary>
    /// <returns></returns>
    private string CardToCard_RK(System.Data.DataTable FormData, string dbname, string cUserName
        , string cLogDate, string SheetID, System.Data.SqlClient.SqlCommand Cmd)
    {
        //为DataTable 建立索引
        FormData.PrimaryKey = new System.Data.DataColumn[] { FormData.Columns["TxtName"] };
        string ccc_cardno = GetTextsFrom_FormData_Tag(FormData, "txt_cardno");
        if (ccc_cardno.CompareTo("") == 0) throw new Exception("流转卡号 必须设置成 可视栏目");
        string ccc_intype = GetTextsFrom_FormData_Text(FormData, "txt_ctype");
        if (ccc_intype.CompareTo("") == 0) throw new Exception("交库类型 必须设置成 可视栏目");

        if (GetDataInt(@"select COUNT(*) from " + dbname + @"..mom_orderdetail a inner join " + dbname + @"..T_CC_Card_List b on a.MoDId=b.t_modid 
                    where b.t_card_no='" + ccc_cardno + "' and ISNULL(a.CloseUser,'')<>''", Cmd) > 0)
            throw new Exception("生产订单已经关闭");

        string cRet = MES_OR_U8SCM_MOMOrder_TO_RD10(FormData, dbname, cUserName, cLogDate, SheetID, Cmd);
        string[] cRdInfo = cRet.Split(',');  //入库单ID，入库单单号，入库单子表标识
        //建立流转卡和 入库单的关联关系 
        WriteFlowDataToRd10(Cmd, cRdInfo[2] + "", ccc_cardno, ccc_intype, dbname, ccc_cardno);
        return cRdInfo[2];
    }
    /// <summary>
    /// 卡制卡--排产
    /// </summary>
    private string CardToCard_PC(System.Data.DataTable PCData, string cUserName, System.Data.SqlClient.SqlCommand Cmd)
    {

        bool b_flow_simple_type = true;//流转类型，是否简单流转模式  分复杂生产和简单生产
        if (UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from T_Parameter where cPid='mes_flow_type'").ToLower().CompareTo("true") == 0) b_flow_simple_type = false;

        DataRowCollection rows = PCData.Rows;
        bool b_route_control = bool.Parse("" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from T_Parameter where cPid='mes_dayplan_route_control'"));
        // 返回的排产id
        string rtlId = "";
        for (int i = 0; i < rows.Count; i++)
        {
            decimal.Parse(rows[i]["pro_qty"] + "");  //
            DateTime.Parse(rows[i]["cprodate"] + "");
            string t_plan_code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select right('000000000'+cast(cast(isnull(max(t_plan_code),0) as int)+1 as varchar(10)),10) from T_CC_MorderToDay");

            Cmd.CommandText = @"insert into T_CC_MorderToDay(t_modid,t_qty,t_prodate,maker,makedate,uptime,t_plan_code,t_route_desc) 
                        values(" + rows[i]["modid"] + "," + rows[i]["pro_qty"] + ",'" + rows[i]["cprodate"] +
                         "','" + cUserName + "',getdate(),convert(varchar(20),getdate(),120),'" + t_plan_code + "','" + rows[i]["t_route_id"] + "')";
            Cmd.ExecuteNonQuery();
            string c_mo_day_planid = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT IDENT_CURRENT( 'T_CC_MorderToDay' )");
            string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
            if (c_mo_day_planid != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

            //判断是否超生产订单数量
            float forderqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select qty from mom_orderdetail where modid=0" + rows[i]["modid"]));
            float fpcqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(sum(t_qty),0) from T_CC_MorderToDay where t_modid=0" + rows[i]["modid"]));
            if (forderqty < fpcqty) throw new Exception("产品【" + rows[i]["invcode"] + "】超订单【" + rows[i]["mocode"] + "】数量排产");

            //创建排产计划工艺路线
            string cRoutID = (rows[i]["t_route_id"] + "").Split(',')[0];  //工艺路线ID
            if (b_route_control && cRoutID == "") throw new Exception("产品【" + rows[i]["invcode"] + "】订单【" + rows[i]["mocode"] + "】要求确定工艺路线");
            if (cRoutID != "")
            {
                DataTable dtSeqRout = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.OpSeq,b.OpCode,c.WcCode,
                            case when a.ReportFlag=1 then '是' else '否' end ReportFlag,case when a.SubFlag=1 then '是' else '否' end t_om_op,
                            ISNULL(d.t_is_warehouse,'否') t_op_warehouse,a.SVendorCode cvencode,isnull(e.t_bal_second,0) t_bal_second
                        from sfc_proutingdetail a inner join sfc_operation b on a.OperationId=b.OperationId
                            inner join sfc_workcenter c on a.wcid=c.wcid
                            left join T_CC_ProCtl_Setting d on b.OpCode=d.t_opcode and c.WcCode=d.t_wccode
                            left join (select t_opcode,t_wccode,max(t_bal_second) t_bal_second from T_CC_OP_Bal_Time 
                                where t_invcode='" + rows[i]["invcode"] + @"' group by t_opcode,t_wccode) e on b.OpCode=e.t_opcode and c.WcCode=e.t_wccode
                        where PRoutingId=0" + cRoutID + " order by a.OpSeq");
                for (int r = 0; r < dtSeqRout.Rows.Count; r++)
                {
                    if (dtSeqRout.Rows[r]["OpCode"] + "" == "FB01" || dtSeqRout.Rows[r]["OpCode"] + "" == "FB02") throw new Exception("【FB01】【FB02】 是特殊工序，不能选择");
                    if (dtSeqRout.Rows[r]["t_om_op"] + "" == "是" && dtSeqRout.Rows[r]["ReportFlag"] + "" == "否") throw new Exception("外协工序[" + dtSeqRout.Rows[r]["OpCode"] + "]必须是报告工序");
                    if (dtSeqRout.Rows[r]["t_op_warehouse"] + "" == "是" && dtSeqRout.Rows[r]["ReportFlag"] + "" == "否") throw new Exception("工序[" + dtSeqRout.Rows[r]["OpCode"] + "]有仓库接收管理必须是报告工序");
                    if (r == 0 && dtSeqRout.Rows[r]["t_op_warehouse"] + "" == "是") throw new Exception("第一道工序不能设置 接收仓库");
                    //判断是否存在外协价格表   ************************************
                    if (dtSeqRout.Rows[r]["t_om_op"] + "" == "是" && dtSeqRout.Rows[r]["cvencode"] + "" != ""
                        && UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) name from T_CC_OM_Issue_Cost a 
                                where a.cinvcode='" + rows[i]["invcode"] + "' and a.copcode='" + dtSeqRout.Rows[r]["OpCode"] + "' and a.cvencode='" + dtSeqRout.Rows[r]["cvencode"] + @"' 
                                and a.startdate<=convert(varchar(10),getdate(),120) and isnull(a.enddate,'2099-12-31')>=convert(varchar(10),getdate(),120)") == 0)
                    {
                        throw new Exception("外协工序[" + dtSeqRout.Rows[r]["OpCode"] + "]的加工商[" + dtSeqRout.Rows[r]["cvencode"] + "]无有效加工单价信息");
                    }

                    Cmd.CommandText = @"insert into T_CC_MorderDay_Process( t_m_day_id, t_card_seq, t_opcode, t_wc_code, ReportFlag, t_om_op,t_op_warehouse,t_op_vencode,t_bal_second) 
                                values(" + c_mo_day_planid + ", '" + dtSeqRout.Rows[r]["OpSeq"] + "', '" + dtSeqRout.Rows[r]["OpCode"] + "', '" + dtSeqRout.Rows[r]["WcCode"] + @"', 
                                    '" + dtSeqRout.Rows[r]["ReportFlag"] + "','" + dtSeqRout.Rows[r]["t_om_op"] + "','" + dtSeqRout.Rows[r]["t_op_warehouse"] + @"',
                                    '" + dtSeqRout.Rows[r]["cvencode"] + "'," + dtSeqRout.Rows[r]["t_bal_second"] + ")";
                    Cmd.ExecuteNonQuery();
                }
            }

            //写车间工序计划
            string sfc_modid = "" + rows[i]["modid"];
            SendMO_OpPlan_To_U8(sfc_modid, cRoutID, cUserName, c_mo_day_planid, Cmd);

            //更新排产计划（此处可以增加触发器控制逻辑关系）
            Cmd.CommandText = "update T_CC_MorderToDay set t_plan_code='" + t_plan_code + "' where t_id=" + c_mo_day_planid;
            Cmd.ExecuteNonQuery();
            rtlId = c_mo_day_planid;
            //判断是否简单模式
            if (b_flow_simple_type)
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select COUNT(*) from T_CC_MorderDay_Process where t_m_id=" + c_mo_day_planid + " and ReportFlag='否' or t_op_warehouse='是'") > 0)
                    throw new Exception("简单制造模式下不能设置并行工序，也不能设置工序仓库");
            }
        }
        return rtlId;
    }
    /// <summary>
    /// 卡制卡辅助方法SendMO_OpPlan_To_U8
    /// </summary>
    /// <param name="sfc_modid"></param>
    /// <param name="cRoutID"></param>
    /// <param name="cUserName"></param>
    /// <param name="day_planid"></param>
    /// <param name="Cmd"></param>
    public static void SendMO_OpPlan_To_U8(string sfc_modid, string cRoutID, string cUserName, string day_planid, System.Data.SqlClient.SqlCommand Cmd)
    {
        //若生成领料申请单，则不需要生成车间工序计划
        #region //领料申请单
        bool bMerApp = false;//检查参数
        if (GetMESSysParm(Cmd, "mes_dayplan_Material_App").ToLower() == "true")
        {
            DataTable dtPlaninfo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_plan_code,t_qty,DB_NAME() dbname,SUBSTRING(DB_NAME(),8,3) accid,b.MDeptCode,b.Qty,
	                    case when a.t_prodate<CONVERT(varchar(10),makedate,120) then a.t_prodate else CONVERT(varchar(10),makedate,120) end cmdate,maker,
                        t_prodate,round(t_qty/b.Qty,8) ip_bl
                    from T_CC_MorderToDay a inner join mom_orderdetail b on a.t_modid=b.MoDId where t_id=" + day_planid);
            string db_name = dtPlaninfo.Rows[0]["dbname"] + "";
            string db_accid = dtPlaninfo.Rows[0]["accid"] + "";
            string p_bl = dtPlaninfo.Rows[0]["ip_bl"] + "";  //占生产订单比例
            //存货小数位数
            string cInv_DecDgt = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT cValue FROM AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'");

            //存在合法的申请单据
            if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) 
                    from mom_moallocate a inner join mom_orderdetail b on a.MoDId=b.MoDId inner join mom_order c on b.MoId=c.MoId
                    where a.MoDId=" + sfc_modid + " and ProductType=1 and a.RequisitionFlag=1 and a.WIPType=3 and a.Qty>0 and isnull(a.Qty,0)>isnull(a.RequisitionQty,0)") > 0)
            {
                string cmessage = "";
                KK_U8Com.U8MaterialAppVouch MAppM = new KK_U8Com.U8MaterialAppVouch(Cmd, db_name);
                #region //申请单主表
                string MID = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select 1000000000+isnull(max(iFatherId),0)+1 from UFSystem..UA_Identity(rowlock)
                        where cAcc_Id='" + db_accid + "' and cVouchType='mv'");
                Cmd.CommandText = "update UFSystem..UA_Identity  set iFatherId=iFatherId+1 where cAcc_Id='" + db_accid + "' and cVouchType='mv'";
                Cmd.ExecuteNonQuery();
                string cVCode = "PC" + dtPlaninfo.Rows[0]["t_plan_code"];
                MAppM.ID = MID;
                MAppM.cCode = "'" + cVCode + "'";
                MAppM.cMaker = "'" + dtPlaninfo.Rows[0]["maker"] + "'";
                MAppM.dDate = "'" + dtPlaninfo.Rows[0]["cmdate"] + "'";
                MAppM.dnmaketime = "getdate()";

                MAppM.cHandler = "'" + dtPlaninfo.Rows[0]["maker"] + "'";
                MAppM.dVeriDate = "'" + dtPlaninfo.Rows[0]["cmdate"] + "'";
                MAppM.dnverifytime = "getdate()";

                string cMdep = "" + dtPlaninfo.Rows[0]["MDeptCode"];
                MAppM.cDepCode = (cMdep == "" ? "null" : "'" + cMdep + "'");  //部门
                MAppM.cSource = "'生产订单'";
                MAppM.imquantity = "" + dtPlaninfo.Rows[0]["Qty"];
                MAppM.csysbarcode = "'||st64|" + cVCode + "'";

                MAppM.InsertToDB(ref cmessage);
                #endregion

                #region //申请单子表
                string AutoID = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(max(iChildId),0) from UFSystem..UA_Identity(rowlock)
                        where cAcc_Id='" + db_accid + "' and cVouchType='mv'");

                Cmd.CommandText = @"insert into MaterialAppVouchs(AutoID,ID,cInvCode,cAssUnit,iinvexchrate,dDueDate,cWhCode,cbMemo,
                        iQuantity,iNum,
	                    cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree10,cDefine22,cDefine23,cDefine24,cDefine25,cDefine26,
	                    cDefine27,cDefine28,cDefine29,cDefine30,cDefine31,cDefine32,cDefine33,cDefine34,cDefine35,cDefine36,cDefine37,
	                    irowno,iMPoIds,invcode,cmocode,imoseq,iopseq,iordertype,iorderdid,isotype,
	                    ipesodid,ipesotype,cpesocode,ipesoseq) 
                    select " + AutoID + "+(ROW_NUMBER() OVER (ORDER BY a.AllocateId DESC))," + MID + @",
                        a.InvCode,a.AuxUnitCode,a.ChangeRate,'" + dtPlaninfo.Rows[0]["t_prodate"] + @"' dDueDate,a.WhCode,a.Remark,
                        case when isnull(a.Qty,0)-isnull(a.RequisitionQty,0)<round(a.Qty*" + p_bl + "," + cInv_DecDgt + ") then isnull(a.Qty,0)-isnull(a.RequisitionQty,0) else round(a.Qty*" + p_bl + "," + cInv_DecDgt + @") end,
                        round(a.AuxQty*" + p_bl + "," + cInv_DecDgt + @"),
	                    a.Free1,a.Free2,a.Free3,a.Free4,a.Free5,a.Free6,a.Free7,a.Free8,a.Free10,a.Define22,a.Define23,a.Define24,a.Define25,a.Define26,
	                    a.Define27,a.Define28,a.Define29,a.Define30,a.Define31,a.Define32,a.Define33,a.Define34,a.Define35,a.Define36,a.Define37,
	                    ROW_NUMBER() OVER (ORDER BY a.AllocateId DESC) irowno,a.AllocateId,b.InvCode,c.MoCode,b.SortSeq,a.OpSeq,0 iordertype,0 iorderdid,0 isotype,
	                    a.AllocateId ipesodid,7 ipesotype,c.MoCode cpesocode,b.SortSeq ipesoseq
                    from mom_moallocate a inner join mom_orderdetail b on a.MoDId=b.MoDId
	                    inner join mom_order c on b.MoId=c.MoId
                    where a.MoDId=" + sfc_modid + " and ProductType=1 and a.RequisitionFlag=1 and a.WIPType=3 and a.Qty>0 and isnull(a.Qty,0)>isnull(a.RequisitionQty,0)";
                int irowcount = Cmd.ExecuteNonQuery();

                Cmd.CommandText = "update UFSystem..UA_Identity  set iChildId=iChildId+" + irowcount + " where cAcc_Id='" + db_accid + "' and cVouchType='mv'";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update MaterialAppVouchs set cbsysbarcode='||st64|" + cVCode + @"|'+cast(irowno as varchar(10)),
                        iNum=case when iinvexchrate=0 then 0 else round(iQuantity/iinvexchrate," + cInv_DecDgt + @") end where id=" + MID;
                Cmd.ExecuteNonQuery();

                //回写子件用料计划申请量
                Cmd.CommandText = @"update mom_moallocate Set RequisitionQty=ISNULL(t.i_qty,0)
                        from (
	                        select iMPoIds,SUM(iQuantity) i_qty from MaterialAppVouchs 
	                        where iMPoIds in(select iMPoIds from MaterialAppVouchs where ID=" + MID + @")
	                        group by iMPoIds
                        ) t where mom_moallocate.AllocateId=t.iMPoIds
	                        and mom_moallocate.AllocateId in(select iMPoIds from MaterialAppVouchs where ID=" + MID + @")";
                Cmd.ExecuteNonQuery();

                #endregion
                //写排产与申请单对照表  T_CC_MorderDay_MAppVouch
                Cmd.CommandText = "insert into T_CC_MorderDay_MAppVouch(t_planday_id,t_MApp_id,t_plan_qty) values(" + day_planid + "," + MID + "," + dtPlaninfo.Rows[0]["t_qty"] + ")";
                Cmd.ExecuteNonQuery();

                bMerApp = true;
            }

            if (bMerApp) return;  //退出，不生成车间计划
        }
        #endregion

        #region //车间工序计划
        return;//不产生工艺计划
        //没有生产订单工艺路线表，直接退出,解决 ERP不是U8
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID('[dbo].[sfc_morouting]') AND type in ('U')") == 0)
        {
            return;
        }

        //判断是否存在工艺路线
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(*) from mom_orderdetail where MoDId=0" + sfc_modid + " and isnull(routingid,0)>0") == 0)
        {
            //生产订单没有指定工艺路线  直接退出
            return;
        }



        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(*) from sfc_morouting where MoDId=0" + sfc_modid) == 0)
        {
            string sfc_moid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select moid from mom_orderdetail where MoDId=0" + sfc_modid);
            Cmd.CommandText = @"insert into sfc_morouting(MoRoutingId, MoId, MoDId, CreateDate, CreateUser) 
                            values(" + sfc_modid + "," + sfc_moid + "," + sfc_modid + ",getdate(),'" + cUserName + "')";
            Cmd.ExecuteNonQuery();

            string cacc_id = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select substring(db_name(),8,3)");
            if (cRoutID != "")
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(*) from UFSystem..UA_Identity where cAcc_Id='" + cacc_id + "' and cVouchType='sfc_moroutingdetail'") == 0)
                {
                    Cmd.CommandText = @"insert into UFSystem..UA_Identity(cAcc_Id,cVouchType,iFatherId,iChildId) 
                                    values('" + cacc_id + "','sfc_moroutingdetail',0,0)";
                    Cmd.ExecuteNonQuery();
                }

                DataTable dtProut = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select OpSeq,cast(LastFlag as int) LastFlag,OperationId,Description,WcId from sfc_proutingdetail a
                                where PRoutingId=0" + cRoutID + " order by a.OpSeq");
                for (int ll = 0; ll < dtProut.Rows.Count; ll++)
                {
                    string i_MoRoutingDId = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(max(iChildId),0)+1 from UFSystem..UA_Identity 
                                    where cAcc_Id='" + cacc_id + "' and cVouchType='sfc_moroutingdetail'");
                    Cmd.CommandText = "update UFSystem..UA_Identity  set iChildId=" + i_MoRoutingDId + ",iFatherId=" + i_MoRoutingDId + " where cAcc_Id='" + cacc_id + "' and cVouchType='sfc_moroutingdetail'";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = @"insert into sfc_moroutingdetail(MoRoutingDId, MoRoutingId, OpSeq,LastFlag,moid,modid,   OperationId,Description,WcId,StartDate,DueDate,ChangeRate) 
                                values(" + i_MoRoutingDId + "," + sfc_modid + ",'" + dtProut.Rows[ll]["OpSeq"] + "'," + dtProut.Rows[ll]["LastFlag"] + "," + sfc_moid + "," + sfc_modid + @",
                                    " + dtProut.Rows[ll]["OperationId"] + ",'" + dtProut.Rows[ll]["Description"] + "'," + dtProut.Rows[ll]["WcId"] + @",
                                    convert(varchar(10),getdate(),120),convert(varchar(10),getdate(),120)  ,null)";
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                //无工艺路线
                string i_MoRoutingDId = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(max(iChildId),0)+1 from UFSystem..UA_Identity 
                                    where cAcc_Id='" + cacc_id + "' and cVouchType='sfc_moroutingdetail'");
                Cmd.CommandText = "update UFSystem..UA_Identity  set iChildId=" + i_MoRoutingDId + ",iFatherId=" + i_MoRoutingDId + " where cAcc_Id='" + cacc_id + "' and cVouchType='sfc_moroutingdetail'";
                Cmd.ExecuteNonQuery();

                Cmd.CommandText = @"insert into sfc_moroutingdetail(MoRoutingDId, MoRoutingId, OpSeq,LastFlag,moid,modid,ChangeRate) 
                                values(" + i_MoRoutingDId + "," + sfc_modid + ",'0010',1," + sfc_moid + "," + sfc_modid + ",null)";
                Cmd.ExecuteNonQuery();

                //更新用料表 的工序行信息
                Cmd.CommandText = "update mom_moallocate set OpSeq='0010' where modid=0" + sfc_modid;
                Cmd.ExecuteNonQuery();
            }
            Cmd.CommandText = "update mom_orderdetail set SfcFlag=1 where modid=0" + sfc_modid;
            Cmd.ExecuteNonQuery();
        }
        #endregion
    }

    /// <summary>
    /// 卡制卡-出库
    /// </summary>
    private string CardToCard_CK(string cUserName, string CurDate, string cc_invcode, string cWhCode, string cc_batch, string cRdCode
        , string rkQty, string moDid
        , System.Data.SqlClient.SqlCommand Cmd)
    {
        string cc_mcode = "";
        // 返回的出库单子表id
        string rtlId = "";
        int rd_id = 0;
        string cdepcode = "" + KK_HFCheckFlow.U8Common.GetStringFromSql("select MDeptCode from mom_orderdetail where modid=0" + moDid, Cmd);
        #region 构建出库子表需要的数据
        System.Data.DataTable ckData = KK_HFCheckFlow.U8Common.GetDataFromDB(@"
select 0 autoid,b.modid,mo.mocode,mos.SortSeq soseq,i.cinvcode,i.cinvname,i.cinvstd,u.ccomunitname cunitname
,cast(round(" + rkQty + @"*BaseQtyN/BaseQtyD,5) as float) balqualifiedqty,'" + cc_batch + @"' cbatch,
cast(st.istqty as float) stockqty,b.AllocateId allate_id,b.invcode,cast(isnull(b.IssQty,0) as float) issqty
,'' cvmivendor,'' doccode,'" + cUserName + @"' cusername 
from 
mom_moallocate b 
inner join mom_orderdetail mos on mos.MoDId = b.MoDId
inner join mom_order mo on mo.MoId = mos.MoId
inner join inventory i on b.invcode=i.cinvcode
inner join ComputationUnit u on i.cComunitCode=u.cComunitCode
left join (
	select cinvcode,sum(iquantity) istqty from CurrentStock where cwhcode='" + cWhCode + @"' group by cinvcode
) st on b.invcode=st.cinvcode
where b.modid=" + moDid + @" and b.invcode='" + cc_invcode + @"' and b.ByproductFlag=0
", Cmd);
        if (ckData.Rows.Count > 1)
        {
            throw new Exception("出库了多行,应该只有一行");
        }
        #endregion
        #region  //主表
        string dbname = KK_HFCheckFlow.U8Common.GetStringFromSql("select db_name()", Cmd);
        KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);

        string targetAccId = KK_HFCheckFlow.U8Common.GetStringFromSql("select substring('" + dbname + "',8,3)", Cmd);
        string cdate = KK_HFCheckFlow.U8Common.GetStringFromSql("select convert(varchar(10),'" + CurDate + "',120)", Cmd);
        rd_id = 1000000000 + int.Parse(KK_HFCheckFlow.U8Common.GetStringFromSql("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        //判断ID是否被使用
        int iIDUsed = int.Parse(KK_HFCheckFlow.U8Common.GetStringFromSql("select count(*) from RdRecordView where id=0" + rd_id, Cmd));
        if (iIDUsed > 0) throw new Exception("单据ID冲突，请稍等片刻再保存");

        string cCodeHead = "C" + KK_HFCheckFlow.U8Common.GetStringFromSql("select right(replace(convert(varchar(10),'" + cdate + "',120),'-',''),6)", Cmd); ;
        cc_mcode = cCodeHead + KK_HFCheckFlow.U8Common.GetStringFromSql("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from rdrecord11 where ccode like '" + cCodeHead + "%'", Cmd);
        record11.cCode = "'" + cc_mcode + "'";
        record11.ID = rd_id;
        record11.cVouchType = "'11'";
        record11.cWhCode = "'" + cWhCode + "'";
        record11.cMaker = "'" + cUserName + "'";
        record11.dDate = "'" + cdate + "'";
        record11.cHandler = "'" + cUserName + "'";
        record11.dVeriDate = "'" + cdate + "'";

        record11.cDepCode = "'" + cdepcode + "'";  //生产部门
        record11.cDefine8 = "'" + cRdCode + "'";   //班组
        record11.cRdCode = "'" + cRdCode + "'";
        record11.cMemo = "'卡制卡出库'";
        record11.iExchRate = "1";
        record11.cExch_Name = "'人民币'";

        record11.cSource = "'生产订单'";
        string errmsg = "";
        if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        #endregion


        #region  //子表
        //判断是否存在 代管仓库
        int idgcount = int.Parse("" + KK_HFCheckFlow.U8Common.GetStringFromSql("select count(*) from warehouse where cwhcode='" + cWhCode + "' and bProxyWh=1", Cmd));
        bool bNotRecords = true;
        for (int i = 0; i < ckData.Rows.Count; i++)
        {
            KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);

            int cAutoid = int.Parse(KK_HFCheckFlow.U8Common.GetStringFromSql("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd));
            records11.AutoID = cAutoid;
            rtlId = cAutoid + "";
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();

            records11.ID = rd_id;
            records11.cInvCode = "'" + ckData.Rows[i]["cinvcode"] + "'";
            records11.cBatch = "'" + ckData.Rows[i]["cbatch"] + "'";
            records11.iQuantity = "" + ckData.Rows[i]["balqualifiedqty"];
            records11.irowno = (i + 1);
            records11.iNQuantity = records11.iQuantity;
            //生产订单关联信息   autoid,modid,mocode,soseq,cinvcode,cinvname,cinvstd,cunitname,balqualifiedqty,cbatch,stockqty,allate_id
            records11.iMPoIds = "" + ckData.Rows[i]["allate_id"];
            records11.ipesodid = "" + ckData.Rows[i]["allate_id"];
            records11.imoseq = "" + ckData.Rows[i]["soseq"];
            records11.ipesoseq = "" + ckData.Rows[i]["soseq"];
            records11.cmocode = "'" + ckData.Rows[i]["mocode"] + "'";
            records11.cpesocode = "'" + ckData.Rows[i]["mocode"] + "'";
            records11.cMoLotCode = "'" + KK_HFCheckFlow.U8Common.GetStringFromSql("select MoLotCode from mom_orderdetail where modid=0" + ckData.Rows[i]["modid"], Cmd) + "'";
            records11.ipesotype = "7";
            records11.invcode = "'" + ckData.Rows[i]["invcode"] + "'";
            //计划价
            records11.iPUnitCost = "" + KK_HFCheckFlow.U8Common.GetStringFromSql("select isnull(iInvRCost,0) from inventory where cinvcode=" + records11.cInvCode, Cmd);
            records11.iPPrice = (float.Parse(records11.iPUnitCost) * float.Parse(records11.iQuantity)) + "";

            //代管
            if (idgcount > 0)
            {
                records11.cvmivencode = "'" + ckData.Rows[i]["cvmivendor"] + "'";
                records11.bVMIUsed = "1";
            }
            //              
            //保存数据
            if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            bNotRecords = false;

            Cmd.CommandText = "update mom_moallocate set IssQty=isnull(IssQty,0)+(0" + records11.iQuantity + ") where AllocateId=0" + records11.iMPoIds;
            Cmd.ExecuteNonQuery();

            //检查是否超 生产订单领用量
            if (int.Parse(KK_HFCheckFlow.U8Common.GetStringFromSql("select count(*) from mom_moallocate where AllocateId=0" + records11.iMPoIds + " and isnull(IssQty,0)-isnull(Qty,0)>1", Cmd)) > 0)
            {
                throw new Exception("【" + records11.cInvCode + "】不能超生产订单出库");
            }

            //判定 库存账 负库存问题
            string crcount = KK_HFCheckFlow.U8Common.GetStringFromSql("select count(*) from CurrentStock where cwhcode='" + cWhCode + "' and cinvcode='" +
                ckData.Rows[i]["cinvcode"] +
                "' and cbatch='" + ckData.Rows[i]["cbatch"] + "' and isnull(iquantity,0)<0", Cmd);

            if (int.Parse(crcount) > 0)
            {
                throw new Exception("【" + ckData.Rows[i]["cinvcode"] + "】出现负库存");
            }
        }
        if (bNotRecords) throw new Exception("表体无有效记录");
        #endregion
        return rtlId;
    }
    /// <summary>
    /// 卡制卡--制卡
    /// </summary>
    /// <param name="cUserName"></param>
    /// <param name="CurDate"></param>
    /// <param name="zkData"></param>
    /// <param name="Cmd"></param>
    private string CardToCard_ZK(string cUserName, string CurDate, string c_mo_day_planid, string rkQty, string cc_batch
        , System.Data.SqlClient.SqlCommand Cmd)
    {
        // 用于返回的流转卡号
        string rtlCardNo = "";

        //必须有工艺
        bool cmust_route = bool.Parse("" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from T_Parameter where cPid='mes_flow_must_route'"));
        // 查询制卡相关datatable
        string c_route_sql = @"select c.invcode,c.free1,c.free2,c.free3,c.free4,c.free5,c.free6,c.free7,c.free8,c.free9,c.free10,min(cast(a.PRoutingId as varchar(20))+','+cast(Version as varchar(10))+VersionDesc) route_ver 
                        from sfc_prouting a inner join sfc_proutingpart b on a.PRoutingId=b.PRoutingId 
                        inner join bas_part c on b.partid=c.partid where VersionEffDate<=getdate() and  VersionEndDate>=convert(varchar(10),getdate(),120)
                        group by c.invcode,c.free1,c.free2,c.free3,c.free4,c.free5,c.free6,c.free7,c.free8,c.free9,c.free10";
        if (!cmust_route)
        {
            c_route_sql = "select '' invcode,'' free1,'' free2,'' free3,'' free4,'' free5,'' free6,'' free7,'' free8,'' free9,'' free10,'' route_ver";
        }
        #region 获取field_list

        DataTable fieldDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select t_db_field from T_CC_Base_GridColShow where SheetID='U81004' and UserID='' order by t_colsort,t_colshow");
        string field_list = "";
        for (int i = 0; i < fieldDt.Rows.Count; i++)
        {
            field_list += "," + fieldDt.Rows[i]["t_db_field"].ToString().ToLower();
        }
        field_list = field_list.Substring(1);
        #endregion
        //cInvDefine12 默认流转当量
        string sql = "select " + field_list + @"
                from mom_order a inner join mom_orderdetail b on a.moid=b.moid
	                inner join mom_morder c on b.modid=c.modid
	                inner join inventory i on b.invcode=i.cinvcode
	                inner join department d on b.MDeptCode=d.cdepcode
	                inner join ComputationUnit u on i.cComunitcode=u.cComunitcode 
	                inner join T_CC_MorderToDay e on b.modid=e.t_modid 
                    left join (select t_dayorderid,sum(t_card_qty) t_card_qty from T_CC_Card_List where t_card_type='标准卡' and (t_isfather_card='母卡' or t_isfather_card='并母') group by t_dayorderid) f on e.t_id=f.t_dayorderid
                    left join (" + c_route_sql + @") p on b.invcode=p.invcode and b.free1=p.free1 and b.free2=p.free2 and b.free3=p.free3 and b.free4=p.free4 
                                            and b.free5=p.free5 and b.free6=p.free6 and b.free7=p.free7 and b.free8=p.free8 and b.free9=p.free9 and b.free10=p.free10
                where isnull(b.CloseUser,'')='' and e.t_qty-isnull(f.t_card_qty,0)>0 and e.t_id=" + c_mo_day_planid + " ";

        DataTable zkData = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, sql.ToLower());
        DataRowCollection rows = zkData.Rows;
        //参数 是否创建明细工序信息
        string cprocess_mx = "否";
        string c_cast_lists = "''";
        for (int i = 0; i < rows.Count; i++)
        {
            if (cprocess_mx.CompareTo("否") != 0 && rows[i]["t_route_id"] + "" == "") throw new Exception("工艺路线必须输入");
            string cRoutID = (rows[i]["t_route_id"] + "").Split(',')[0];  //工艺路线ID
            //若为空，获得排产的工艺主ID
            if (cRoutID == "")
            {
                cRoutID = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT t_route_desc from T_CC_MorderToDay where t_id=0" + rows[i]["t_id"]);
                cRoutID = cRoutID.Split(',')[0];
            }
            // 张数
            //int icarscount = int.Parse("" + rows[i]["cardscount"]);
            int icarscount = 1;
            decimal iall = decimal.Parse("" + rows[i]["t_last_qty"]);
            //decimal iPerQty = decimal.Parse("" + rows[i]["one_card_qty"]);
            // 单卡数量
            decimal iPerQty = decimal.Parse(rkQty);

            #region   //必录项检查
            //for (int b = 0; b < dtMustInputLM.Rows.Count; b++)
            //{
            //    if (rows[i][dtMustInputLM.Rows[b]["t_colname"] + ""] + "" == "")
            //        throw new Exception("排产号[" + rows[i]["t_plan_code"] + "]所在行[" + dtMustInputLM.Rows[b]["t_colshow"] + "]必须录入");
            //}

            #endregion

            #region  //是否采集主要原材料批号信息  (存货自定义项1)
            if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select count(*) from inventory where cinvcode='" + rows[i]["invcode"] + "' and isnull(cinvdefine1,'')='是'")) > 0)
            {
                if (rows[i]["t_cbatch_mian_mer"] + "" == "")
                    throw new Exception("产品[" + rows[i]["invcode"] + "]要求有原材料批号");
            }
            #endregion

            for (int j = 0; j < 1; j++)
            {
                if (iall <= 0) break;  //创建完毕  直接退出
                decimal i_card_flow_qty = (iall > iPerQty ? iPerQty : iall);  //流转卡数量
                if (i_card_flow_qty <= 0) continue;
                //string card_no = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select right('000000000'+cast(cast(isnull(max(t_card_no),'0') as int)+1 as varchar(10)),10) from T_CC_Card_List");
                //最大编号处理
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select COUNT(*) from T_CC_Voucher_Num where voucher_name='flowcard'") == 0)
                {
                    Cmd.CommandText = "insert into T_CC_Voucher_Num(voucher_name,chead,cdigit_1,cdigit_2) values('flowcard','',0,0)";
                    Cmd.ExecuteNonQuery();
                    //初始化最大值
                    int c_max_code = UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select cast(isnull(max(t_card_no),0) as int)+1 from T_CC_Card_list where t_card_no like '0%'");
                    Cmd.CommandText = "update T_CC_Voucher_Num set cdigit_1=0" + c_max_code + " where voucher_name='flowcard' and chead=''";
                    Cmd.ExecuteNonQuery();
                }
                string card_no = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select right('000000000'+cast(cdigit_1+1 as varchar(10)),10) 
                            from T_CC_Voucher_Num (rowlock) where voucher_name='flowcard' and chead=''");
                rtlCardNo = card_no;
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select COUNT(*) from T_CC_Card_List(nolock) where t_card_no='" + card_no + "'") > 0)
                {
                    //重置大值
                    int c_max_code = 1 + UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select cast(isnull(max(t_card_no),0) as int)+1 from T_CC_Card_list where t_card_no like '0%'");
                    Cmd.CommandText = "update T_CC_Voucher_Num set cdigit_1=0" + c_max_code + " where voucher_name='flowcard' and chead=''";
                    Cmd.ExecuteNonQuery();

                    card_no = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select right('000000000'+cast(cdigit_1+1 as varchar(10)),10) from T_CC_Voucher_Num (rowlock) where voucher_name='flowcard' and chead=''");
                }

                Cmd.CommandText = "update T_CC_Voucher_Num set cdigit_1=cdigit_1+1 where voucher_name='flowcard' and chead=''";
                Cmd.ExecuteNonQuery();

                #region //判断是否重复
                int lp = 1;
                while (lp < 100)
                {
                    lp++;
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(*) from T_CC_Card_List(nolock) where t_card_no='" + card_no + "'") > 0)
                    {
                        card_no = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select right('000000000'+cast(cdigit_1+1 as varchar(10)),10) from T_CC_Voucher_Num (rowlock) where voucher_name='flowcard' and chead=''");
                        Cmd.CommandText = "update T_CC_Voucher_Num set cdigit_1=cdigit_1+1 where voucher_name='flowcard' and chead=''";
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        break;//退出循环
                    }
                }
                #endregion

                #region   //流转卡主表
                Cmd.CommandText = @"insert into T_CC_Card_List(t_card_no, t_card_qty, t_card_overqty, t_dayorderid, t_modid, t_morder_code, t_morder_seq, 
                            t_invcode, t_startdate, t_enddate, t_maker, t_makedate, t_closer, t_closedate, t_route_mid, t_isfather_card, 
                            t_islast_card, t_soure, t_card_type, t_state, t_father_card_id,tDefine1,tDefine2,tDefine3,tDefine4,tDefine5,
                            tDefine6,tDefine7,tDefine8,tDefine9,tDefine10,
                            tDefine11,tDefine12,tDefine13,tDefine14,tDefine15,
                            tDefine16,tDefine17,tDefine18,tDefine19,tDefine20,
                            t_pro_cbatch_no,t_cbatch_mian_mer) 
                        values('" + card_no + "'," + i_card_flow_qty + @",0,
                            " + rows[i]["t_id"] + "," + rows[i]["modid"] + ",'" + rows[i]["mocode"] + "', '" + rows[i]["t_mo_seq"] + @"', 
                            '" + rows[i]["invcode"] + "','" + rows[i]["sdate"] + "','" + rows[i]["edate"] + "','" + cUserName + "',getdate(),'',null,0" + cRoutID + @",'母卡', 
                            1,'原卡','标准卡','开卡',0,'" + rows[i]["tDefine1"] + "','" + rows[i]["tDefine2"] + "','" + rows[i]["tDefine3"] + "','" + rows[i]["tDefine4"] + "','" + rows[i]["tDefine5"] + @"',
                            '" + rows[i]["tDefine6"] + "','" + rows[i]["tDefine7"] + "','" + rows[i]["tDefine8"] + "','" + rows[i]["tDefine9"] + "','" + rows[i]["tDefine10"] + @"',
                            '" + rows[i]["tDefine11"] + "','" + rows[i]["tDefine12"] + "','" + rows[i]["tDefine13"] + "','" + rows[i]["tDefine14"] + "',0" + rows[i]["tDefine15"] + @",
                            0" + rows[i]["tDefine16"] + ",0" + rows[i]["tDefine17"] + ",0" + rows[i]["tDefine18"] + ",'" + rows[i]["tDefine19"] + "','" + rows[i]["tDefine20"] + @"',
                            '" + rows[i]["t_pro_cbatch_no"] + "','" + cc_batch + "')";
                // whf 主材批号取出库和入库的批号 cc_batch 替换 (rows[i]["t_cbatch_mian_mer"] + "").Split(',')[0]
                Cmd.ExecuteNonQuery();

                //校验流转卡号重复性
                string c_t_cardid = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT IDENT_CURRENT( 'T_CC_Card_List' )");
                string check_db_identity = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
                if (c_t_cardid != check_db_identity) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select COUNT(*) from T_CC_Card_List where t_card_no='" + card_no + "' and t_card_id<>" + c_t_cardid) > 0)
                    throw new Exception("同时多人取流转卡号，出现重复，请确定后，重新保存");

                c_cast_lists = c_cast_lists + ",'" + card_no + "'";
                iall = iall - i_card_flow_qty;  //剩余数记录
                #endregion


                #region//检查是否超 日排产计划 制卡
                float f_plan_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(sum(t_qty),0) from T_CC_MorderToDay where t_id=" + rows[i]["t_id"]));
                float f_flow_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(sum(t_card_qty),0) from T_CC_Card_List where t_dayorderid=" + rows[i]["t_id"] +
                    " and t_card_type='标准卡' and (t_isfather_card='母卡' or t_isfather_card='并母')"));
                if (f_plan_qty < f_flow_qty) throw new Exception("[" + rows[i]["t_plan_code"] + "]超日排产计划制卡");

                #endregion


                if (cmust_route && cRoutID == "" && rows[i]["t_dayplan_route"] + "" == "否") throw new Exception("没有工艺路线");
                if (cRoutID == "" && rows[i]["t_dayplan_route"] + "" == "否") continue;   //没有工艺路线，不能创建流转卡明细
                #region //流转卡明细
                DataTable dtSeqRout = null;
                DataTable dt_dialog_seqData = null;
                if (dt_dialog_seqData == null)
                {
                    if (rows[i]["t_dayplan_route"] + "" == "是")
                    {
                        dtSeqRout = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.t_card_seq OpSeq,a.t_opcode OpCode,a.t_wc_code WcCode,a.ReportFlag,a.t_om_op,a.t_op_warehouse,a.t_op_vencode
                                    from T_CC_MorderDay_Process a
                                    where t_m_day_id=0" + rows[i]["t_id"] + " order by a.t_card_seq");
                    }
                    else
                    {
                        dtSeqRout = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.OpSeq,b.OpCode,c.WcCode,case when a.ReportFlag=1 then '是' else '否' end ReportFlag,
                                        case when a.SubFlag=1 then '是' else '否' end t_om_op,ISNULL(d.t_is_warehouse,'否') t_op_warehouse,a.SVendorCode t_op_vencode
                                    from sfc_proutingdetail a inner join sfc_operation b on a.OperationId=b.OperationId
	                                    inner join sfc_workcenter c on a.wcid=c.wcid
                                        left join T_CC_ProCtl_Setting d on b.OpCode=d.t_opcode and c.WcCode=d.t_wccode
                                    where PRoutingId=0" + cRoutID + " order by a.OpSeq");
                        //判断是否存在外协价格表  *********************************************************
                        if (dtSeqRout.Rows[i]["t_om_op"] + "" == "是" && dtSeqRout.Rows[i]["t_op_vencode"] + "" != ""
                            && UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) name from T_CC_OM_Issue_Cost a 
                                where a.cinvcode='" + rows[i]["invcode"] + "' and a.copcode='" + dtSeqRout.Rows[i]["OpCode"] + "' and a.cvencode='" + dtSeqRout.Rows[i]["t_op_vencode"] + @"' 
                                and a.startdate<=convert(varchar(10),getdate(),120) and isnull(a.enddate,'2099-12-31')>=convert(varchar(10),getdate(),120)") == 0)
                        {
                            throw new Exception("外协工序[" + dtSeqRout.Rows[i]["OpCode"] + "]的加工商[" + dtSeqRout.Rows[i]["t_op_vencode"] + "]无有效加工单价信息");
                        }

                    }
                }
                else
                {
                    dtSeqRout = dt_dialog_seqData;
                }

                //回写流转卡主卡 完工数，若无工序信息，则完工数=流转数，若有工序信息，则完工数=0
                Cmd.CommandText = "update T_CC_Card_List set t_card_overqty=" + (dtSeqRout.Rows.Count > 0 ? "0" : "t_card_qty") + " where t_card_no='" + card_no + "'";
                Cmd.ExecuteNonQuery();

                string t_up_c_id = "0";  //最新报告工序明细ID
                string t_pro_c_id = "0";  //报工流转卡明细ID
                for (int k = 0; k < dtSeqRout.Rows.Count; k++)
                {
                    if (dtSeqRout.Rows[k]["OpCode"] + "" == "FB01" || dtSeqRout.Rows[k]["OpCode"] + "" == "FB02")
                        throw new Exception("非标返修特殊工序[FB01][FB02]不能进入工艺路线,请调整工艺路线");
                    if (dtSeqRout.Rows[k]["t_om_op"] + "" == "是" && dtSeqRout.Rows[k]["ReportFlag"] + "" == "否") throw new Exception("外协工序[" + dtSeqRout.Rows[k]["OpCode"] + "]必须是报告工序");
                    if (dtSeqRout.Rows[k]["t_op_warehouse"] + "" == "是" && dtSeqRout.Rows[k]["ReportFlag"] + "" == "否") throw new Exception("工序[" + dtSeqRout.Rows[k]["OpCode"] + "]有接收仓库管理时必须是报告工序");
                    if (k == 0 && dtSeqRout.Rows[k]["t_op_warehouse"] + "" == "是") throw new Exception("第一道工序不能设置 接收仓库");

                    #region   //报工流转卡明细
                    Cmd.CommandText = @"insert into T_CC_Cards_process( t_card_no, t_card_seq, t_opcode, t_wc_code, t_valid_qty, t_scrap_work, t_scrap_material, t_scrap_qc, 
                                        t_miss_qty, t_repair_qty, t_renew_qty, t_report_c_id, t_father_process_c_id, t_father_report_c_id, t_is_stop) 
                                    values('" + card_no + "','" + dtSeqRout.Rows[k]["OpSeq"] + "', '" + dtSeqRout.Rows[k]["OpCode"] + "', '" + dtSeqRout.Rows[k]["WcCode"] + @"',0,0, 0,0, 
                                        0, 0, 0, 0,0, 0, 0)";
                    Cmd.ExecuteNonQuery();

                    t_pro_c_id = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT IDENT_CURRENT( 'T_CC_Cards_process' )");
                    string check_db_identity1 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
                    if (t_pro_c_id != check_db_identity1) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

                    Cmd.CommandText = @"update T_CC_Cards_process set t_barcode='1'+right('0000000000000000'+cast(t_c_id as nvarchar(20)),15) where t_c_id=0" + t_pro_c_id;
                    Cmd.ExecuteNonQuery();
                    #endregion

                    #region   //报告流转卡明细
                    //连线生产 连线最后一道工序 必须为 报告工序
                    if ("" + dtSeqRout.Rows[k]["ReportFlag"] != "是")
                    {
                        if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select count(*) cn from T_CC_LineEQ_Manufacture 
                                    where t_invcode='" + rows[i]["invcode"] + "' and t_opcode='" + dtSeqRout.Rows[k]["OpCode"] + "' and t_wccode='" + dtSeqRout.Rows[k]["WcCode"] + "' and t_is_last_op='是'")) > 0)
                            throw new Exception("工作中心【" + dtSeqRout.Rows[k]["WcCode"] + "】工序【" + dtSeqRout.Rows[k]["OpCode"] + "】已设连线生产末道工序，必须为报告工序");
                    }

                    if (k == 0 || k == dtSeqRout.Rows.Count - 1 || "" + dtSeqRout.Rows[k]["ReportFlag"] == "是")
                    {
                        Cmd.CommandText = @"insert into T_CC_Cards_report( t_card_no, t_card_seq, t_opcode, t_wc_code, t_tran_in_qty, t_working_qty,t_working_ven_qty, t_valid_qty, t_scrap_work, 
                                        t_scrap_material, t_scrap_qc, t_miss_qty, t_repair_qty, t_renew_qty, t_up_c_id, t_father_c_id, t_is_first_seq, t_is_last_seq,t_worked_qty,t_om_op,t_op_warehouse,t_op_vencode) 
                                    values('" + card_no + "','" + dtSeqRout.Rows[k]["OpSeq"] + "', '" + dtSeqRout.Rows[k]["OpCode"] + "', '" + dtSeqRout.Rows[k]["WcCode"] + @"',0,0,0, 0,0, 
                                        0, 0, 0, 0, 0, " + t_up_c_id + @", 0, " + (k == 0 ? 1 : 0) + ", " + (k == dtSeqRout.Rows.Count - 1 ? 1 : 0) + @",
                                        0,'" + dtSeqRout.Rows[k]["t_om_op"] + "','" + dtSeqRout.Rows[k]["t_op_warehouse"] + @"',
                                        '" + (dtSeqRout.Rows[k]["t_om_op"] + "" == "否" ? "" : dtSeqRout.Rows[k]["t_op_vencode"] + "") + "')";
                        Cmd.ExecuteNonQuery();

                        //连线生产 非首道工序规则：不能设置工序仓库
                        if (dtSeqRout.Rows[k]["t_op_warehouse"] + "" == "是")
                        {
                            if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select count(*) cn from T_CC_LineEQ_Manufacture 
                                        where t_invcode='" + rows[i]["invcode"] + "' and t_opcode='" + dtSeqRout.Rows[k]["OpCode"] + "' and t_wccode='" + dtSeqRout.Rows[k]["WcCode"] + "' and t_is_first_op='否'")) > 0)
                                throw new Exception("工作中心【" + dtSeqRout.Rows[k]["WcCode"] + "】工序【" + dtSeqRout.Rows[k]["OpCode"] + "】已设连线生产工序，不能设置工序仓管理");
                        }

                        //连线生产  不能外协
                        if (dtSeqRout.Rows[k]["t_om_op"] + "" == "是")
                        {
                            if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select count(*) cn from T_CC_LineEQ_Manufacture 
                                        where t_invcode='" + rows[i]["invcode"] + "' and t_opcode='" + dtSeqRout.Rows[k]["OpCode"] + "' and t_wccode='" + dtSeqRout.Rows[k]["WcCode"] + "'")) > 0)
                                throw new Exception("工作中心【" + dtSeqRout.Rows[k]["WcCode"] + "】工序【" + dtSeqRout.Rows[k]["OpCode"] + "】已设连线生产工序，不能外协");
                        }

                        t_up_c_id = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "SELECT IDENT_CURRENT( 'T_CC_Cards_report' )");//报告ID
                        string check_db_identity2 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select scope_identity()");
                        if (t_up_c_id != check_db_identity2) throw new Exception("当前数据库资源占用冲突，请等待几秒再提交");

                        //更新报工路线的  报告ID  (更新本卡所有未指定 报告ID的工序记录 )
                        Cmd.CommandText = "update T_CC_Cards_process set t_report_c_id=" + t_up_c_id + @" where t_card_no='" + card_no + "' and t_report_c_id=0";
                        Cmd.ExecuteNonQuery();
                    }
                    #endregion
                }
                #endregion


                //更新流转卡号（此处可以增加触发器 控制逻辑）
                Cmd.CommandText = "update T_CC_Card_List set t_card_no=isnull(t_card_no,'') where t_card_no='" + card_no + "'";
                Cmd.ExecuteNonQuery();
                bool b_flow_simple_type = true;//流转类型，是否简单流转模式  分复杂生产和简单生产
                if (GetMESSysParm(Cmd, "mes_flow_type").ToLower().CompareTo("true") == 0) b_flow_simple_type = false;
                if (b_flow_simple_type)
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select COUNT(*) from T_CC_Cards_process a inner join T_CC_Cards_report b on a.t_report_c_id=b.t_c_id
                                where a.t_card_no='" + card_no + "' and a.t_card_seq<>b.t_card_seq") > 0)
                        throw new Exception("简单制造模式下不能设置并行工序");

                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select COUNT(*) from T_CC_Cards_report
                                where t_card_no='" + card_no + "' and t_op_warehouse='是'") > 0)
                        throw new Exception("简单制造模式下不能设置工序仓库");
                }
            }
        }
        return rtlCardNo;
    }
    public static string GetMESSysParm(SqlCommand Cmd, string parmID)
    {
        return UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cValue from T_Parameter where cPid='" + parmID + "'");
    }
    #endregion


}

