using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// U8PDMInterface121 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class U8PDMInterface121 : System.Web.Services.WebService
{

    public U8PDMInterface121()
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

            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        
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


    [WebMethod(Description = @"判定计算机 授权信息 ")]  //U8 授权
    public bool ComputerKey()
    {
        if (GetEnStr().CompareTo(System.Configuration.ConfigurationManager.AppSettings["XmlSn"]) == 0)
            return true;
        else
            return false;
    }

    [WebMethod(Description = @"计量单位查询接口。<br/>
            cComUnitName=“” 代表调用出所有计量单位，如果传递具体参数名称，则返回符合条件的记录；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 字段【ccomunitcode,ccomunitname】，字段名为全小写")]  //U8 输出计量单位清单
    public System.Data.DataTable GetUnitLists(string cComUnitName, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtRet;
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        string cSql = @"select ccomunitcode,ccomunitname from " + dbname + "..ComputationUnit a inner join " + dbname + 
            "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and a.cComUnitName like '%" + cComUnitName + "%'";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(cSql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtRet = ds.Tables[0];
        dtRet.TableName = "dtRet";

        CloseDataConnection(Conn);
        return dtRet;
        
    }

    [WebMethod(Description = @"计量单位增接口。 <br/>
            cComUnitName 代表单计量单位名称；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 true 代表成功")]  //U8 新增单计量单位
    public bool AddUnitName(string cComUnitCode, string cComUnitName, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        cComUnitName=cComUnitName+"";
        if (cComUnitName.CompareTo("") == 0) throw new Exception("计量单位名称不能为空");

        //string cSql = @"select count(*) from " + dbname + "..ComputationUnit a inner join " + dbname +
        //    "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and a.cComUnitName = '" + cComUnitName + "'";
        //if (U8Operation.GetDataInt(cSql, Cmd) > 0) throw new Exception("计量单位名已经存在");
        string cSql = @"select count(*) from " + dbname + "..ComputationUnit a where cComunitCode = '" + cComUnitCode + "'";
        if (U8Operation.GetDataInt(cSql, Cmd) > 0)
        {
            Cmd.CommandText = @"update " + dbname + @"..ComputationUnit set cComUnitName='" + cComUnitName + @"'
                where cComunitCode='" + cComUnitCode + "'";
            Cmd.ExecuteNonQuery();
        }
        else
        {
            string cGroupCode = "" + U8Operation.GetDataString("select cGroupCode from " + dbname + "..ComputationGroup where iGroupType=0", Cmd);
            if (cGroupCode.CompareTo("") == 0) throw new Exception("请先在U8中建立无换算计量单位组");

//            string cMaxCode = "" + U8Operation.GetDataString("select '" + cGroupCode + @"'+right('00'+cast(cast(isnull(MAX(cComunitCode),'0') as int)+1 as varchar(3)),3)
//            from " + dbname + "..ComputationUnit where cComunitCode like '" + cGroupCode + "%'", Cmd);
            Cmd.CommandText = @"insert into " + dbname + @"..ComputationUnit(cComunitCode,cComUnitName,cGroupCode,bMainUnit,iNumber) 
            values('" + cComUnitCode + "','" + cComUnitName + "','" + cGroupCode + "',1,0)";
            Cmd.ExecuteNonQuery();
        }
        CloseDataConnection(Conn);
        return true;
    }


    [WebMethod(Description = @"存货分类查询接口。<br/>
            cInvCCName 代表末级计量单位名称，当为空字符串时，调用所有末级计量单位，如果传递具体参数名称，则返回符合条件的记录；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 字段【cinvccode,cinvcname】，字段名为全小写")]  //U8 输出存货分类
    public System.Data.DataTable GetInvClassLists(string cInvCCName, string dbname)
    {
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtRet;
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        string cSql = @"select cinvccode,cinvcname from " + dbname + "..InventoryClass where binvcend=1 and cInvCName like '%" + cInvCCName + "%'";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(cSql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtRet = ds.Tables[0];
        dtRet.TableName = "dtRet";
        CloseDataConnection(Conn);
        return dtRet;
    }


    [WebMethod(Description = @"存货分类新增接口。<br/> 
            （A：如分类0305：03父类编码，0305 代表本次需要新增的存货分类编码）；<br/> 
            （B：如分类03005：03父类编码，03005 代表本次需要新增的存货分类编码）；<br/> 
            一旦确认分类级次，级次的长度就固定了，就不能调整，以上两种情况只能选中一种长度。传递时需要从顶级分类传送<br/> 
            fatherCode 代表父类分类编码；fatherCode="" 代表顶级分类<br/>
            cInvCCode  代表分类编码；<br/>
            cInvCCName 代表单计量单位名称；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 true 代表成功")]  //U8 存货分类
    public bool AddInvClassLists(string fatherCode,string cInvCCode, string cInvCCName, string dbname)
    {
        throw new Exception("接口非法");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtRet;
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }

        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            cInvCCName = cInvCCName + ""; cInvCCode = cInvCCode + ""; fatherCode = fatherCode + "";
            if (cInvCCName.CompareTo("") == 0) throw new Exception("分类名称不能为空");
            if (cInvCCode.CompareTo("") == 0) throw new Exception("分类编码不能为空");
            if (fatherCode.CompareTo("") != 0)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..InventoryClass where cInvCCode='" + fatherCode + "'", Cmd) == 0)
                    throw new Exception("父分类编码不存在");
            }

            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..InventoryClass where cInvCCode='" + cInvCCode + "'", Cmd) > 0)
            {
                Cmd.CommandText = @"update " + dbname + @"..InventoryClass set cInvCName='" + cInvCCName + "' where cInvCCode='" + cInvCCode + "'";
                Cmd.ExecuteNonQuery();
            }
            else
            {
                if (fatherCode.CompareTo("") != 0)
                {
                    int iFatherGrade = U8Operation.GetDataInt("select iInvCGrade from " + dbname + @"..InventoryClass where cinvccode='" + fatherCode + "'", Cmd);
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..Inventory where cinvccode='" + fatherCode + "'", Cmd) > 0)
                        throw new Exception("存货分类【" + fatherCode + "】下面已经有物料，不能再其下面增加分类");

                    Cmd.CommandText = @"update " + dbname + @"..InventoryClass set bInvCEnd=0 where cinvccode='" + fatherCode + "'";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = @"insert into " + dbname + @"..InventoryClass(cInvCCode,cInvCName,iInvCGrade,bInvCEnd) values('" + cInvCCode + "','" + cInvCCName + "'," + iFatherGrade + "+1,1)";
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = @"insert into " + dbname + @"..InventoryClass(cInvCCode,cInvCName,iInvCGrade,bInvCEnd) values('" + cInvCCode + "','" + cInvCCName + "',1,1)";
                    Cmd.ExecuteNonQuery();
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

    [WebMethod(Description = @"成本查询接口，需要维护U8的供应商存货价格表。<br/>
            cinvcode代表查询具体存货编码的最新采购单价，cinvcode=“” 代表查询所有存货最新采购单价；<br/>
            cinvCCode代表查询某个具体存货分类的所有存货采购单价，cinvCCode=“” 代表查询所有分类存货最新采购单价；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 字段【cinvcode 存货编码,ccost 无税单价,ctaxcost 含税单价】，字段名为全小写")]  //U8 输出计量单位清单
    public System.Data.DataTable GetInventoryPriceLists(string cinvcode, string cinvCCode, string dbname)
    {
        throw new Exception("接口非法");

        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtRet;
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        string cSql = @"select cinvcode,(select top 1 iUnitPrice from " + dbname + @"..Ven_Inv_Price a where a.cInvCode=i.cInvCode order by a.dEnableDate desc) ccost,
	            (select top 1 iTaxUnitPrice from " + dbname + @"..Ven_Inv_Price a where a.cInvCode=i.cInvCode order by a.dEnableDate desc) ctaxcost
            from " + dbname + @"..Inventory i
            where cInvCode like '" + cinvcode + "%' and cInvCCode like '" + cinvCCode + "%'";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(cSql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtRet = ds.Tables[0];
        dtRet.TableName = "dtRet";

        CloseDataConnection(Conn);
        return dtRet;

    }

    [WebMethod(Description = @"物料档案新增接口。<br/>
            不管是修改还是新增，请统一传递，接口自动判断。<br/>
            dtItemList 存货档案列表，若清单中有多个档案，则只要有一条无法写入，代表所有档案写入失败；<br/>
            参数dtItemList传入字段说明（英文小写为字段名称，中文为描述，如调用方式 dtItemList.columns[“itemcode”]）：<br/>
            itemcode	存货编码<br/>
            itemname	存货名称<br/>
            itemstd	规格型号<br/>
            itemclasscode	存货分类编码<br/>
            unitcode	计量单位编码<br/>
            bsale	是否内销  0 代表否/1 代表 是<br/>
            bpurchase	是否采购  0 代表否/1 代表 是<br/>
            bcomsume	是否生产耗用  0 代表否/1 代表 是<br/>
            bself	是否自制  0 代表否/1 代表 是<br/>
            bproxyforeign	是否委外生产  0 代表否/1 代表 是<br/>
            bpto	PTO  0 代表否/1 代表 是<br/>
            bato	ATO  0 代表否/1 代表 是<br/>
            cdef_1	请填写：标准件/自制件  <br/>
            cdef_2	标准号    <br/>
            参数dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 写入成功与否")]  //U8 导入存货档案
    public bool SendInvBaseData(System.Data.DataTable dtItemList, string dbname)
    {
        throw new Exception("接口非法");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string cUnitCode = "";
            //获得拷贝的存货编码
            string cSourceInvCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetInventory'", Cmd);
            if (cSourceInvCode.CompareTo("") == 0) throw new Exception("U8ERP中没有设置【源存货档案编码-自制】信息");
            if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + cSourceInvCode + "'", Cmd) == 0)
            {
                throw new Exception("自制【源样板存货档案编码" + cSourceInvCode + "】不存在");
            }

            string cSourcePuInvCode = "" + GetDataString("select isnull(cValue,'') from " + dbname + "..T_Parameter where cPid='targetPuInventory'", Cmd);
            if (cSourcePuInvCode.CompareTo("") == 0) throw new Exception("U8ERP中没有设置【源存货档案编码-外购件】信息");
            if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + cSourcePuInvCode + "'", Cmd) == 0)
            {
                throw new Exception("外购【源样板存货档案编码-外购件" + cSourcePuInvCode + "】不存在");
            }

            GetDataString("select 1", Cmd);
            for (int i = 0; i < dtItemList.Rows.Count; i++)
            {
                //判断是否存在存货
                int iInvCount = GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                if (iInvCount > 0)
                {
                    cUnitCode = "" + GetDataString("select cComUnitCode from " + dbname + "..Inventory where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                    string cNewUnitCode = "" + GetDataString(@"select top 1 ccomunitcode from " + dbname + "..ComputationUnit a inner join " + dbname +
                        "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and (a.cComUnitName = '" + dtItemList.Rows[i]["unitcode"] + "' or a.ccomunitcode = '" + dtItemList.Rows[i]["unitcode"] + "')", Cmd);
                    if (cUnitCode.CompareTo(cNewUnitCode) != 0)
                    {
                        //查看是否存在入出库记录
                        iInvCount = GetDataInt("select count(*) from " + dbname + "..rdrecordsview where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                        if (iInvCount > 0) throw new Exception("存货【" + dtItemList.Rows[i]["itemcode"] + "】已经使用，不能调整计量单位，原单位编码为【" + cUnitCode + "】");
                    }

                    //修改存货档案
                    #region
                    Cmd.CommandText = "update " + dbname + "..Inventory set cinvname='" + dtItemList.Rows[i]["itemname"] + "',cinvstd='" + dtItemList.Rows[i]["itemstd"] +
                        "',cComunitCode='" + cNewUnitCode + "',cinvccode='" + dtItemList.Rows[i]["itemclasscode"] + 
                        "',bSale=0" + dtItemList.Rows[i]["bsale"] + ",bPurchase=0" + dtItemList.Rows[i]["bpurchase"] + ",bComsume=0" + dtItemList.Rows[i]["bcomsume"] +
                        ",bSelf=0" + dtItemList.Rows[i]["bself"] + ",bProxyForeign=0" + dtItemList.Rows[i]["bproxyforeign"] + ",bPTOModel=0" + dtItemList.Rows[i]["bpto"] +
                        ",bATOModel=0" + dtItemList.Rows[i]["bato"] + ",cInvDefine1='" + dtItemList.Rows[i]["cdef_1"] + "', cInvDefine2='" + dtItemList.Rows[i]["cdef_2"] + @"' 
                        where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'";
                    Cmd.ExecuteNonQuery();
                    #endregion
                }
                else
                {
                    string cCopyInv = "";
                    if (int.Parse(dtItemList.Rows[i]["bself"] + "") == 1)
                    {
                        cCopyInv = cSourceInvCode;  //自制
                    }
                    else
                    {
                        cCopyInv = cSourcePuInvCode;  //外购
                    }
                    //新增档案

                    //计量单位判断
                    string c_add_new_unit = "" + GetDataString(@"select top 1 ccomunitcode from " + dbname + "..ComputationUnit a inner join " + dbname +
                        "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and (a.cComUnitName = '" + dtItemList.Rows[i]["unitcode"] + "' or a.ccomunitcode = '" + dtItemList.Rows[i]["unitcode"] + "')", Cmd);
                    if (c_add_new_unit == "") throw new Exception("存货[" + dtItemList.Rows[i]["itemcode"] + "]没有找到[" + dtItemList.Rows[i]["unitcode"] + "]对应的计量单位");


                    #region
                    Cmd.CommandText = "insert into " + dbname + @"..Inventory(cInvCode, cInvAddCode, cInvName, cInvStd, cInvCCode, cVenCode, cReplaceItem, cPosition, bSale, bPurchase, bSelf, bComsume, bProducing, bService, bAccessary, 
                            iTaxRate, iInvWeight, iVolume, iInvRCost, iInvSPrice, iInvSCost, iInvLSCost, iInvNCost, iInvAdvance, iInvBatch, iSafeNum, iTopSum, iLowSum, iOverStock, cInvABC, 
                            bInvQuality, bInvBatch, bInvEntrust, bInvOverStock, dSDate, dEDate, bFree1, bFree2, cInvDefine1, cInvDefine2, cInvDefine3,bInvType, iInvMPCost, cQuality, 
                            iInvSaleCost, iInvSCost1, iInvSCost2, iInvSCost3, bFree3, bFree4, bFree5, bFree6, bFree7, bFree8, bFree9, bFree10, cCreatePerson, cModifyPerson, dModifyDate, 
                            fSubscribePoint, fVagQuantity, cValueType, bFixExch, fOutExcess, fInExcess, iMassDate, iWarnDays, fExpensesExch, bTrack, bSerial, bBarCode, iId, cBarCode, 
                            cInvDefine4, cInvDefine5, cInvDefine6, cInvDefine7, cInvDefine8, cInvDefine9, cInvDefine10, cInvDefine11, cInvDefine12, cInvDefine13, cInvDefine14, cInvDefine15, 
                            cInvDefine16, iGroupType, cGroupCode, cComUnitCode, cAssComUnitCode, cSAComUnitCode, cPUComUnitCode, cSTComUnitCode, cCAComUnitCode, cFrequency, 
                            iFrequency, iDays, dLastDate, iWastage, bSolitude, cEnterprise, cAddress, cFile, cLabel, cCheckOut, cLicence, bSpecialties, cDefWareHouse, iHighPrice, 
                            iExpSaleRate, cPriceGroup, cOfferGrade, iOfferRate, cMonth, iAdvanceDate, cCurrencyName, cProduceAddress, cProduceNation, cRegisterNo, cEnterNo, 
                            cPackingType, cEnglishName, bPropertyCheck, cPreparationType, cCommodity, iRecipeBatch, cNotPatentName,bPromotSales, iPlanPolicy, iROPMethod, 
                            iBatchRule, fBatchIncrement, iAssureProvideDays, iTestStyle, iDTMethod, fDTRate, fDTNum, cDTUnit, iDTStyle, iQTMethod, bPlanInv, bProxyForeign, 
                            bATOModel, bCheckItem, bPTOModel, bEquipment, cProductUnit, fOrderUpLimit, cMassUnit, fRetailPrice, cInvDepCode, iAlterAdvance, fAlterBaseNum, cPlanMethod, 
                            bMPS, bROP, bRePlan, cSRPolicy, bBillUnite, iSupplyDay, fSupplyMulti, fMinSupply, bCutMantissa, cInvPersonCode, iInvTfId, cEngineerFigNo, bInTotalCost, 
                            iSupplyType, bConfigFree1, bConfigFree2, bConfigFree3, bConfigFree4, bConfigFree5, bConfigFree6, bConfigFree7, bConfigFree8, bConfigFree9, bConfigFree10, 
                            iDTLevel, cDTAQL, bPeriodDT, cDTPeriod, iBigMonth, iBigDay, iSmallMonth, iSmallDay, bOutInvDT, bBackInvDT, iEndDTStyle, bDTWarnInv, fBackTaxRate, cCIQCode, 
                            cWGroupCode, cWUnit, fGrossW, cVGroupCode, cVUnit, fLength, fWidth, fHeight, iDTUCounter, iDTDCounter, iBatchCounter, cShopUnit, cPurPersonCode, 
                            bImportMedicine, bFirstBusiMedicine, bForeExpland, cInvPlanCode, fConvertRate, dReplaceDate, bInvModel, bKCCutMantissa, bReceiptByDT, iImpTaxRate, 
                            iExpTaxRate, bExpSale, iDrawBatch, bCheckBSATP, cInvProjectCode, iTestRule, cRuleCode, bCheckFree1, bCheckFree2, bCheckFree3, bCheckFree4, bCheckFree5, 
                            bCheckFree6, bCheckFree7, bCheckFree8, bCheckFree9, bCheckFree10, bBomMain, bBomSub, bProductBill, iCheckATP, iInvATPId, iPlanTfDay, iOverlapDay, bPiece, 
                            bSrvItem, bSrvFittings, fMaxSupply, fMinSplit, bSpecialOrder, bTrackSaleBill, cInvMnemCode, iPlanDefault, iPFBatchQty, iAllocatePrintDgt, bCheckBatch, bMngOldpart,
                            iOldpartMngRule) 
                        select '" + dtItemList.Rows[i]["itemcode"] + @"',cInvAddCode, cInvName, cInvStd, cInvCCode, cVenCode, cReplaceItem, cPosition, bSale, bPurchase, bSelf, bComsume, bProducing, bService, bAccessary, 
                            iTaxRate, iInvWeight, iVolume, iInvRCost, iInvSPrice, iInvSCost, iInvLSCost, iInvNCost, iInvAdvance, iInvBatch, iSafeNum, iTopSum, iLowSum, iOverStock, cInvABC, 
                            bInvQuality, bInvBatch, bInvEntrust, bInvOverStock, dSDate, dEDate, bFree1, bFree2, cInvDefine1, cInvDefine2, cInvDefine3,bInvType, iInvMPCost, cQuality, 
                            iInvSaleCost, iInvSCost1, iInvSCost2, iInvSCost3, bFree3, bFree4, bFree5, bFree6, bFree7, bFree8, bFree9, bFree10, cCreatePerson, cModifyPerson, dModifyDate, 
                            fSubscribePoint, fVagQuantity, cValueType, bFixExch, fOutExcess, fInExcess, iMassDate, iWarnDays, fExpensesExch, bTrack, bSerial, bBarCode, iId, cBarCode, 
                            cInvDefine4, cInvDefine5, cInvDefine6, cInvDefine7, cInvDefine8, cInvDefine9, cInvDefine10, cInvDefine11, cInvDefine12, cInvDefine13, cInvDefine14, cInvDefine15, 
                            cInvDefine16, iGroupType, cGroupCode, cComUnitCode, cAssComUnitCode, cSAComUnitCode, cPUComUnitCode, cSTComUnitCode, cCAComUnitCode, cFrequency, 
                            iFrequency, iDays, dLastDate, iWastage, bSolitude, cEnterprise, cAddress, cFile, cLabel, cCheckOut, cLicence, bSpecialties, cDefWareHouse, iHighPrice, 
                            iExpSaleRate, cPriceGroup, cOfferGrade, iOfferRate, cMonth, iAdvanceDate, cCurrencyName, cProduceAddress, cProduceNation, cRegisterNo, cEnterNo, 
                            cPackingType, cEnglishName, bPropertyCheck, cPreparationType, cCommodity, iRecipeBatch, cNotPatentName,bPromotSales, iPlanPolicy, iROPMethod, 
                            iBatchRule, fBatchIncrement, iAssureProvideDays, iTestStyle, iDTMethod, fDTRate, fDTNum, cDTUnit, iDTStyle, iQTMethod, bPlanInv, bProxyForeign, 
                            bATOModel, bCheckItem, bPTOModel, bEquipment, cProductUnit, fOrderUpLimit, cMassUnit, fRetailPrice, cInvDepCode, iAlterAdvance, fAlterBaseNum, cPlanMethod, 
                            bMPS, bROP, bRePlan, cSRPolicy, bBillUnite, iSupplyDay, fSupplyMulti, fMinSupply, bCutMantissa, cInvPersonCode, iInvTfId, cEngineerFigNo, bInTotalCost, 
                            iSupplyType, bConfigFree1, bConfigFree2, bConfigFree3, bConfigFree4, bConfigFree5, bConfigFree6, bConfigFree7, bConfigFree8, bConfigFree9, bConfigFree10, 
                            iDTLevel, cDTAQL, bPeriodDT, cDTPeriod, iBigMonth, iBigDay, iSmallMonth, iSmallDay, bOutInvDT, bBackInvDT, iEndDTStyle, bDTWarnInv, fBackTaxRate, cCIQCode, 
                            cWGroupCode, cWUnit, fGrossW, cVGroupCode, cVUnit, fLength, fWidth, fHeight, iDTUCounter, iDTDCounter, iBatchCounter, cShopUnit, cPurPersonCode, 
                            bImportMedicine, bFirstBusiMedicine, bForeExpland, cInvPlanCode, fConvertRate, dReplaceDate, bInvModel, bKCCutMantissa, bReceiptByDT, iImpTaxRate, 
                            iExpTaxRate, bExpSale, iDrawBatch, bCheckBSATP, cInvProjectCode, iTestRule, cRuleCode, bCheckFree1, bCheckFree2, bCheckFree3, bCheckFree4, bCheckFree5, 
                            bCheckFree6, bCheckFree7, bCheckFree8, bCheckFree9, bCheckFree10, bBomMain, bBomSub, bProductBill, iCheckATP, iInvATPId, iPlanTfDay, iOverlapDay, bPiece, 
                            bSrvItem, bSrvFittings, fMaxSupply, fMinSplit, bSpecialOrder, bTrackSaleBill, cInvMnemCode, iPlanDefault, iPFBatchQty, iAllocatePrintDgt, bCheckBatch, bMngOldpart,
                            iOldpartMngRule from " + dbname + "..Inventory where cinvcode='" + cCopyInv + "'";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..Inventory set cinvname='" + dtItemList.Rows[i]["itemname"] + "',cinvstd='" + dtItemList.Rows[i]["itemstd"] +
                        "',cComunitCode='" + c_add_new_unit + "',cinvccode='" + dtItemList.Rows[i]["itemclasscode"] + 
                        "',bSale=0" + dtItemList.Rows[i]["bsale"] + ",bPurchase=0" + dtItemList.Rows[i]["bpurchase"] + ",bComsume=0" + dtItemList.Rows[i]["bcomsume"] +
                        ",bSelf=0" + dtItemList.Rows[i]["bself"] + ",bProxyForeign=0" + dtItemList.Rows[i]["bproxyforeign"] + ",bPTOModel=0" + dtItemList.Rows[i]["bpto"] +
                        ",bATOModel=0" + dtItemList.Rows[i]["bato"] + @",iInvRCost=null,iInvSPrice=null,iInvSCost=null,iInvNCost=null,iSafeNum=null,iTopSum=null,iLowSum=null,
                        dSDate=convert(varchar(10),getdate(),120) ,dEDate=null,iInvMPCost=null,fMinSupply=null,cVenCode=null,cModifyPerson=null,dModifyDate=null,
                        cInvDefine1='" + dtItemList.Rows[i]["cdef_1"] + "', cInvDefine2='" + dtItemList.Rows[i]["cdef_2"] + @"'
                        where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'";
                    Cmd.ExecuteNonQuery();

                    GetDataString("select 888", Cmd);

                    Cmd.CommandText = "insert into " + dbname + @"..Inventory_Sub(cInvSubCode, fBuyExcess, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, bIsAttachFile, bInByProCheck, 
                      iRequireTrackStyle, iExpiratDateCalcu, iBOMExpandUnitType, bPurPriceFree1, bPurPriceFree2, bPurPriceFree3, bPurPriceFree4, bPurPriceFree5, bPurPriceFree6, 
                      bPurPriceFree7, bPurPriceFree8, bPurPriceFree9, bPurPriceFree10, bOMPriceFree1, bOMPriceFree2, bOMPriceFree3, bOMPriceFree4, bOMPriceFree5, 
                      bOMPriceFree6, bOMPriceFree7, bOMPriceFree8, bOMPriceFree9, bOMPriceFree10, bSalePriceFree1, bSalePriceFree2, bSalePriceFree3, bSalePriceFree4, 
                      bSalePriceFree5, bSalePriceFree6, bSalePriceFree7, bSalePriceFree8, bSalePriceFree9, bSalePriceFree10, fInvOutUpLimit, bBondedInv, bBatchCreate, 
                      bBatchProperty1, bBatchProperty2, bBatchProperty3, bBatchProperty4, bBatchProperty5, bBatchProperty6, bBatchProperty7, bBatchProperty8, bBatchProperty9, 
                      bBatchProperty10, bControlFreeRange1, bControlFreeRange2, bControlFreeRange3, bControlFreeRange4, bControlFreeRange5, bControlFreeRange6, 
                      bControlFreeRange7, bControlFreeRange8, bControlFreeRange9, bControlFreeRange10, fInvCIQExch, iWarrantyPeriod, iWarrantyUnit, bInvKeyPart, iAcceptEarlyDays, 
                      fProcessCost, fCurLLaborCost, fCurLVarManuCost, fCurLFixManuCost, fCurLOMCost, fNextLLaborCost, fNextLVarManuCost, fNextLFixManuCost, fNextLOMCost, 
                      cInvAppDocNo, bPUQuota, bInvROHS, bPrjMat, fPrjMatLimit, bInvAsset, bSrvProduct, iAcceptDelayDays, iPlanCheckDay, iMaterialsCycle, 
                      iDrawType, bSCkeyProjections, iSupplyPeriodType, iTimeBucketId, iAvailabilityDate, fMaterialCost, bImport, iNearRejectDays, bCheckSubitemCost, fRoundFactor, 
                      bConsiderFreeStock, bSuitRetail) 
                    select '" + dtItemList.Rows[i]["itemcode"] + @"', fBuyExcess, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, bIsAttachFile, bInByProCheck, 
                      iRequireTrackStyle, iExpiratDateCalcu, iBOMExpandUnitType, bPurPriceFree1, bPurPriceFree2, bPurPriceFree3, bPurPriceFree4, bPurPriceFree5, bPurPriceFree6, 
                      bPurPriceFree7, bPurPriceFree8, bPurPriceFree9, bPurPriceFree10, bOMPriceFree1, bOMPriceFree2, bOMPriceFree3, bOMPriceFree4, bOMPriceFree5, 
                      bOMPriceFree6, bOMPriceFree7, bOMPriceFree8, bOMPriceFree9, bOMPriceFree10, bSalePriceFree1, bSalePriceFree2, bSalePriceFree3, bSalePriceFree4, 
                      bSalePriceFree5, bSalePriceFree6, bSalePriceFree7, bSalePriceFree8, bSalePriceFree9, bSalePriceFree10, fInvOutUpLimit, bBondedInv, bBatchCreate, 
                      bBatchProperty1, bBatchProperty2, bBatchProperty3, bBatchProperty4, bBatchProperty5, bBatchProperty6, bBatchProperty7, bBatchProperty8, bBatchProperty9, 
                      bBatchProperty10, bControlFreeRange1, bControlFreeRange2, bControlFreeRange3, bControlFreeRange4, bControlFreeRange5, bControlFreeRange6, 
                      bControlFreeRange7, bControlFreeRange8, bControlFreeRange9, bControlFreeRange10, fInvCIQExch, iWarrantyPeriod, iWarrantyUnit, bInvKeyPart, iAcceptEarlyDays, 
                      fProcessCost, fCurLLaborCost, fCurLVarManuCost, fCurLFixManuCost, fCurLOMCost, fNextLLaborCost, fNextLVarManuCost, fNextLFixManuCost, fNextLOMCost, 
                      cInvAppDocNo, bPUQuota, bInvROHS, bPrjMat, fPrjMatLimit, bInvAsset, bSrvProduct, iAcceptDelayDays, iPlanCheckDay, iMaterialsCycle, 
                      iDrawType, bSCkeyProjections, iSupplyPeriodType, iTimeBucketId, iAvailabilityDate, fMaterialCost, bImport, iNearRejectDays, bCheckSubitemCost, fRoundFactor, 
                      bConsiderFreeStock, bSuitRetail from " + dbname + "..Inventory_Sub where cInvSubCode='" + cCopyInv + "'";
                    Cmd.ExecuteNonQuery();

                    GetDataString("select 888-1", Cmd);

                    string imaxpartid = "" + GetDataString("select max(partId)+1 from " + dbname + @"..bas_part", Cmd);
                    GetDataString("select 888-2", Cmd);
                    Cmd.CommandText = "insert into " + dbname + @"..bas_part(PartId,InvCode, Free1, Free2, Free3, Free4, Free5, Free6, Free7, Free8, Free9, Free10, SafeQty, MinQty, MulQty, FixQty, bVirtual, DrawCode, LLC, 
                      cBasEngineerFigNo, fBasMaxSupply, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, RoundingFactor, FreeStockFlag, 
                      bFreeStop) 
                    select " + imaxpartid + ",'" + dtItemList.Rows[i]["itemcode"] + @"', Free1, Free2, Free3, Free4, Free5, Free6, Free7, Free8, Free9, Free10, SafeQty, MinQty, MulQty, FixQty, bVirtual, DrawCode, LLC, 
                      cBasEngineerFigNo, fBasMaxSupply, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, RoundingFactor, FreeStockFlag, 
                      bFreeStop from " + dbname + "..bas_part where InvCode='" + cCopyInv + "'";
                    Cmd.ExecuteNonQuery();
                    GetDataString("select 888-3", Cmd);
                    #endregion
                }
                GetDataString("select 3", Cmd);

                //存货档案修改正 处理(自制件  采购件 等属性匹配问题)
                Cmd.CommandText = "update " + dbname + "..Inventory set bProductBill=1,bBomMain=1,bBomSub=1 where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "' and bSelf=1";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..Inventory set bBomSub=1,bComsume=1 where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "' and bPurchase=1";
                Cmd.ExecuteNonQuery();
            }

            GetDataString("select 9", Cmd);

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

    [WebMethod(Description = @"传递存货图片接口。<br/>
            cinvcode代表查询具体存货编码；<br/>
            bImage代表图片 字节流<br/>
            ImageType 代表图片类型，如：gif、bmp 等，不能含“.”<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 true 代表成功")]  //
    public bool SendInventoryImage(string cinvcode, byte[] bImage, string ImageType, string dbname)
    {
        throw new Exception("接口非法");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand();
        cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (bImage == null) throw new Exception("图片内容不能为空");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", cmd) == 0)
                throw new Exception("不存在此存货编码");
            string cpic_uid = U8Operation.GetDataString("select isnull(cast(PictureGUID as varchar(50)),'') from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", cmd);
            if (cpic_uid.CompareTo("") != 0)
            {
                cmd.CommandText = "delete from " + dbname + "..AA_Picture where cGUID='" + cpic_uid + "'";
                cmd.ExecuteNonQuery();
            }
            string p_guid = U8Operation.GetDataString("select newid()", cmd);
            cmd.CommandText = "update " + dbname + "..inventory set PictureGUID='" + p_guid + "' where cinvcode='" + cinvcode + "'";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "insert into " + dbname + "..AA_Picture(cGUID,cPicturetype,cTableName) values('" + p_guid + "','" + ImageType + "','Inventory')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "update " + dbname + "..AA_Picture set  Picture=@Image where cGUID='" + p_guid + "'";
            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Image", bImage));
            cmd.ExecuteNonQuery();

            cmd.Transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            cmd.Transaction.Rollback();
            throw ex;
        }
        finally
        {
            CloseDataConnection(Conn);
        }
        
    }


    [WebMethod(Description = @"仓库查询接口。<br/>
            cWareName=“” 代表调用出所有仓库，如果传递具体参数名称，则返回符合条件的记录；<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 字段【cwhcode,cwhname】，字段名为全小写")]  //U8 输出计量单位清单
    public System.Data.DataTable GetWareLists(string cWareName, string dbname)
    {
        throw new Exception("接口非法");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        System.Data.DataTable dtRet;
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        string cSql = @"select cwhcode,cwhname from " + dbname + "..warehouse a where a.cwhname like '%" + cWareName + "%'";
        System.Data.SqlClient.SqlDataAdapter dpt = new System.Data.SqlClient.SqlDataAdapter(cSql, Conn);
        System.Data.DataSet ds = new System.Data.DataSet();
        dpt.Fill(ds);
        dtRet = ds.Tables[0];
        dtRet.TableName = "dtRet";

        CloseDataConnection(Conn);
        return dtRet;

    }


    [WebMethod(Description = @"BOM新增接口。<br/>
            传递前请按照 母件编码、行号排序。不管是修改还是新增，请统一传递，接口自动判断。<br/>
            dtBomInfoList 代表代表BOM清单信息，可一次性传入多个BOM，但每个BOM必须完整<br/>
            参数dtBomInfoList传入字段说明（英文小写为字段名称，中文为描述，如调用方式 dtBomInfoList.columns[“cfathercode”]）：<br/>
            cfathercode	母件编码<br/>
            corderno	订单号  必须订单系统中存在的订单号，已经审核的订单将不允许修改<br/>
            creater	建档人<br/>
            createdate	建档日期<br/>
            cseqno	子件行号  如可用0010代表一行子件<br/>
            childcode	子件编码<br/>
            ibaseuse	用量    可精确到6位小数<br/>
            wiptype	供应类型(1 - 4 整型)  1:入库倒冲  2: 工序倒冲  3: 领料   4: 虚拟件<br/>
            warecode	供应仓库，必须是U8的仓库<br/>
            参数dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 写入成功与否")]  //U8 导入BOM
    public bool SendInvBomData(System.Data.DataTable dtBomInfoList, string dbname)
    {
        throw new Exception("接口非法");
        string oldFatherCode = "";
        if (dtBomInfoList == null) throw new Exception("没有有效数据");
        if (!dtBomInfoList.Columns.Contains("warecode")) throw new Exception("请提供列 供应仓库,字段名warecode");
        dtBomInfoList.DefaultView.Sort = "cfathercode,cseqno";
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string U8AcountID = GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        if (GetDataInt("SELECT count(*) FROM ufsystem..UA_Identity where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'", Cmd) <= 0)
        {
            new Exception("系统BOM环境没有初始化，请在U8系统中建立一个BOM样板（建完后可删除）！");
        }
       

        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //排序
            dtBomInfoList.DefaultView.Sort = "cfathercode";
            dtBomInfoList = dtBomInfoList.DefaultView.ToTable();

            //检查数据逻辑  dtBomInfoList.Rows[i][""]
            #region 
            GetDataString("select 'startBOM'", Cmd);
            GetDataString("select 1[" + dtBomInfoList.Rows.Count+ "]", Cmd);
            for (int i = 0; i < dtBomInfoList.Rows.Count; i++)
            {
                GetDataString("select 2", Cmd);
                if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtBomInfoList.Rows[i]["cfathercode"] + "' and dEDate is null and bBomMain=1", Cmd) <= 0)
                {
                    throw new Exception("编码[" + dtBomInfoList.Rows[i]["cfathercode"] + "]不存在，或已经停用，或不允许BOM母件！");
                }
                if (GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtBomInfoList.Rows[i]["childcode"] + "' and dEDate is null and bBomSub=1", Cmd) <= 0)
                {
                    throw new Exception("编码[" + dtBomInfoList.Rows[i]["childcode"] + "]不存在，或已经停用，或者不允许子件！");
                }
                if (dtBomInfoList.Rows[i]["warecode"] + "" != "")
                {
                    if (GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtBomInfoList.Rows[i]["warecode"] + "'", Cmd) <= 0)
                    {
                        throw new Exception("仓库[" + dtBomInfoList.Rows[i]["warecode"] + "]不存在");
                    }
                }
                if (dtBomInfoList.Rows[i]["wiptype"] + "" != "1" && dtBomInfoList.Rows[i]["wiptype"] + "" != "2" && 
                    dtBomInfoList.Rows[i]["wiptype"] + "" != "3" && dtBomInfoList.Rows[i]["wiptype"] + "" != "4" && 
                    dtBomInfoList.Rows[i]["wiptype"] + "" != "5")
                {
                    throw new Exception("母件[" + dtBomInfoList.Rows[i]["cfathercode"] + "]子件[" + dtBomInfoList.Rows[i]["childcode"] + "]供应类型[" + dtBomInfoList.Rows[i]["wiptype"] + "]值错误");
                }
                //判断母件是否存在BOM表  子件子件属性只能为（1入库倒冲/2工序倒冲/3 领料/4虚拟/5直接供应
                if (oldFatherCode != dtBomInfoList.Rows[i]["cfathercode"] + "")
                {
                    //检测订单号
                    GetDataString("select 3", Cmd);
                    //写主BOM
                    oldFatherCode = dtBomInfoList.Rows[i]["cfathercode"] + "";
                    if (GetDataInt("select isnull(max(a.bomid),0) from " + dbname + "..bom_parent a inner join " + dbname + "..bom_bom b on a.bomid=b.bomid " +
                        "where a.ParentId in(SELECT partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "') and b.Version='10'", Cmd) > 0)
                    {
                        GetDataString("select 3-1", Cmd);
                        string bom_part_del_id = GetDataString("SELECT top 1 partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "'", Cmd);
                        string bom_parent_del_id = GetDataString("select a.BomId from " + dbname + "..bom_parent a inner join " + dbname + @"..bom_bom b on a.BomId=b.BomId 
                            where a.ParentId=0" + bom_part_del_id + " and b.Version=10", Cmd);  //获得10版 BOMID 
                        //删除BOM
                        Cmd.CommandText = "delete from " + dbname + "..bom_opcomponentopt where Optionsid in(select OptionsId from " + dbname + "..bom_opcomponent where BomId=0" + bom_parent_del_id + ")";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "delete from " + dbname + "..bom_opcomponent where BomId=0" + bom_parent_del_id;
                        Cmd.ExecuteNonQuery();
                        
                        Cmd.CommandText = "delete from " + dbname + "..bom_parent where BomId=0" + bom_parent_del_id;
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "delete from " + dbname + "..bom_bom where BomId=0" + bom_parent_del_id;
                        Cmd.ExecuteNonQuery();
                    }
                }

                try
                {
                    Double.Parse(dtBomInfoList.Rows[i]["ibaseuse"] + "");
                }
                catch
                {
                    throw new Exception("母件[" + dtBomInfoList.Rows[i]["cfathercode"] + "]子件[" + dtBomInfoList.Rows[i]["childcode"] + "]用量必须为数字！");
                }
            }
            #endregion

            //写BOM
            #region
            string PartId = "0";  //母件
            string BomBomID = "0";
            string iChildPartId = "0";
            string iChildOpComponentId = "0";
            string iChildOptionsId = "0";
            oldFatherCode = "";
            for (int i = 0; i < dtBomInfoList.Rows.Count; i++)
            {
                if (oldFatherCode != dtBomInfoList.Rows[i]["cfathercode"] + "")
                {
                    oldFatherCode = dtBomInfoList.Rows[i]["cfathercode"] + "";
                    PartId = GetDataString("SELECT top 1 partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "'", Cmd);
                    BomBomID = GetDataString("SELECT iFatherid+1 FROM ufsystem..UA_Identity where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'", Cmd);
                    Cmd.CommandText = "update ufsystem..UA_Identity set iFatherid=" + BomBomID + " where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'";
                    Cmd.ExecuteNonQuery();
                    //新增BOM基础数据
                    Cmd.CommandText = "insert into " + dbname + "..bom_bom(BomId,BomType,Version,VersionDesc,VersionEffdate,VersionEndDate,CreateDate,createUser,createtime,vtid,Status,UpdCount,RelsUser,RelsDate,RelsTime) " +
                        "values(" + BomBomID + ",1,'10','标准','2000-01-01','2099-12-31',convert(varchar(10),getdate(),121),'admin',getdate(),30442,3,0,'admin',convert(varchar(10),getdate(),121),getdate())";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "insert into " + dbname + "..bom_parent(BomId,AutoId,ParentId,ParentScrap,SharingPartId) " +
                        "values(" + BomBomID + ",newid()," + PartId + ",0,0)";
                    Cmd.ExecuteNonQuery();
                }

                iChildPartId = GetDataString("SELECT partid FROM " + dbname + "..bas_part where invcode='" + dtBomInfoList.Rows[i]["childcode"] + "'", Cmd);
                iChildOpComponentId = GetDataString("SELECT iChildId+1 FROM ufsystem..UA_Identity where cVouchType='bom_opcomponent' and cAcc_Id='" + U8AcountID + "'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildId=" + iChildOpComponentId + " where cVouchType='bom_opcomponent' and cAcc_Id='" + U8AcountID + "'";
                Cmd.ExecuteNonQuery();
                iChildOptionsId = GetDataString("SELECT iChildId+1 FROM ufsystem..UA_Identity where cVouchType='bom_opcomponentopt' and cAcc_Id='" + U8AcountID + "'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildId=" + iChildOptionsId + " where cVouchType='bom_opcomponentopt' and cAcc_Id='" + U8AcountID + "'";
                Cmd.ExecuteNonQuery();
                //插入资料表  warecode
                Cmd.CommandText = "insert into " + dbname + "..bom_opcomponentopt(Optionsid,offset,WipType,AccuCostFlag,DrawDeptCode,whcode,OptionalFlag,MutexRule,PlanFactor) " +
                    "values(" + iChildOptionsId + ",0," + dtBomInfoList.Rows[i]["wiptype"] + ",1, null," + (dtBomInfoList.Rows[i]["warecode"] + "" == "" ? "null" : "'" + dtBomInfoList.Rows[i]["warecode"] + "'") + ",0,2,100)";
                Cmd.ExecuteNonQuery();
                //插入子件表 
                Cmd.CommandText = "insert into " + dbname + "..bom_opcomponent(OpComponentId,BomId,SortSeq,OpSeq,ComponentId,EffBegDate,EffEndDate,FvFlag,BaseQtyN,BaseQtyD,CompScrap,ByproductFlag,OptionsId,ProductType,ChangeRate) " +
                    "values(" + iChildOpComponentId + "," + BomBomID + ",'" + dtBomInfoList.Rows[i]["cseqno"] + "','0000'," + iChildPartId + ",'2000-01-01','2099-12-31',1," + dtBomInfoList.Rows[i]["ibaseuse"] + ",1,0,0," + iChildOptionsId + ",1,1)";
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


    [WebMethod(Description = @"预测订单接口。<br/>
            cinvcode 代表查询具体存货编码；<br/>
            iquantity 代表订单数量<br/>
            cdate 代表订单最迟交付日期，格式yyyy-MM-dd<br/>
            dbname 代表需要获得计量单位的账套库名称；<br/>
            返回 true 代表成功")]  
    public bool SendForeCast(string cinvcode, float iquantity, string cdate, string dbname,string cUserName)
    {
        throw new Exception("接口非法");
        System.Data.SqlClient.SqlConnection Conn = OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string U8AcountID = GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + cinvcode + "'", Cmd) == 0)
                throw new Exception("不存在此存货编码");

            string forecastid = GetDataString("SELECT iFatherid+1 FROM ufsystem..UA_Identity where cVouchType='mps_forecast' and cAcc_Id='" + U8AcountID + "'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherid=iFatherid+1 where cVouchType='mps_forecast' and cAcc_Id='" + U8AcountID + "'";
            Cmd.ExecuteNonQuery();
            string cFoCode = U8Operation.GetDataString(@"select 'Y'+right('000000000'+cast(cast(isnull(MAX(replace(FoCode,'Y','')),'0') as int)+1 as varchar(10)),10)
                from " + dbname + "..mps_forecast where FoCode like 'Y%'", Cmd);

            //参数表  T_Parameter： PDM_MRP_Version		预测版本号，必须是MRP类型
            string vversion = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='PDM_MRP_Version'", Cmd);
            string fversionid = "" + U8Operation.GetDataString("select forecastvsid from " + dbname + "..mps_forecastvs where version='" + vversion + "' and mpsflag=2", Cmd);
            if (fversionid.CompareTo("") == 0) throw new Exception("请维护正确预测版本号，要求计划类型：MRP");
            //预测主表
            Cmd.CommandText = "insert into " + dbname + @"..mps_forecast(forecastid,focode,docdate,mpsflag,fversionid,createdate,createuser,updcount,vtid,createtime,iprintcount) 
                values(" + forecastid + ",'" + cFoCode + "',convert(varchar(10),getdate(),120),2," + fversionid + ",convert(varchar(10),getdate(),120),'" + cUserName + @"',
                    0,30485,getdate(),0)";
            Cmd.ExecuteNonQuery();

            //预测子表
            string forecastdetailid = U8Operation.GetDataString("SELECT iFatherid+1 FROM ufsystem..UA_Identity where cVouchType='mps_forecastdetail' and cAcc_Id='" + U8AcountID + "'", Cmd);
            string partid = U8Operation.GetDataString("SELECT partid FROM " + dbname + @"..bas_part where invcode='" + cinvcode + "'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherid=iFatherid+1 where cVouchType='mps_forecastdetail' and cAcc_Id='" + U8AcountID + "'";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "insert into " + dbname + @"..mps_forecastdetail(forecastdid,forecastid,startdate,enddate,fqty,auxfqty,avgtype,partid,changerate,avgrounded,demandcode,status,iswfcontrolled,iverifystate,ireturncount,auditstatus) 
                values(" + forecastdetailid + "," + forecastid + ",convert(varchar(10),getdate(),120),'" + cdate + "',0" + iquantity + ",0,0," + partid + @",0,
                    2,'',1,1,0,0,1)";
            Cmd.ExecuteNonQuery();

            //预测均化表
            string forecastdataid = GetDataString("SELECT iFatherid+1 FROM ufsystem..UA_Identity where cVouchType='mps_forecastdata' and cAcc_Id='" + U8AcountID + "'", Cmd);
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherid=iFatherid+1 where cVouchType='mps_forecastdata' and cAcc_Id='" + U8AcountID + "'";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "insert into " + dbname + @"..mps_forecastdata(fdid,forecastid,forecastdid,sortseq,demdate,fqty,partid,supplydate,demandcode,custcode,status) 
                values(" + forecastdataid + "," + forecastid + "," + forecastdetailid + ",1,convert(varchar(10),getdate(),120),0" + iquantity + "," + partid + @",
                    convert(varchar(10),getdate(),120),'','',1)";
            Cmd.ExecuteNonQuery();
            
            //逻辑检查
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..mps_forecastdetail where forecastdid=0" + forecastdetailid + " and enddate<startdate", Cmd) > 0)
                throw new Exception("需求日期不能小于当前日期");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..mps_forecastdetail where forecastdid=0" + forecastdetailid + " and fqty<=0", Cmd) > 0)
                throw new Exception("需求数量不能小于0");

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

}

