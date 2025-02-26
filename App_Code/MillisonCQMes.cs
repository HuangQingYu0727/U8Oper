using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Data;


/// <summary>
/// MillisonCQMes 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class MillisonCQMes : System.Web.Services.WebService
{
    string add_retstr = "<?xml version=\"1.0\" encoding=\"gb2312\"?><parm><result>@@result</result><msg>@@msg</msg><vouchcode>@@vcode</vouchcode></parm>";
    public MillisonCQMes()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    private void CHeckParm(DataTable dtHead, DataTable dtBody, string cUserName, string dDate)
    {
        if (dtHead == null || dtBody == null || cUserName == null || dDate == null) throw new Exception("参数：dtHead/dtBody/cUserName/dDate 都不允许为空");
        try
        {
            DateTime.Parse(dDate);
        }
        catch
        {
            throw new Exception(dDate+" 无法转化为日期");
        }

        if (dtHead.Rows.Count == 0) throw new Exception("参数：dtHead无记录行");
        if (dtHead.Rows.Count > 1) throw new Exception("参数：dtHead只允许传入一行记录");
        if (dtBody.Rows.Count == 0) throw new Exception("参数：dtBody无记录行");
        if (cUserName == "") throw new Exception("参数：cUserName 需要有效内容");
        if (!dtHead.Columns.Contains("mes_code")) throw new Exception("主表dtHead中必须包含列：mes_code，且需要有效内容");
        if (!dtHead.Columns.Contains("checker")) throw new Exception("主表dtHead中必须包含列：checker，且需要有效内容");
    }
    private string FindMesCode(DataTable dt,string codename)
    {
        if (dt.Columns.Contains(codename))
        {
            return dt.Rows[0][codename] + "";
        }
        else
        {
            return "";
        }
    }
    private string VouchAdd(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate,string app_Sheet,string data_Sheet,string Interface_num)
    {
        string ret_str = "";
        System.Data.SqlClient.SqlConnection Conn = null;
        System.Data.SqlClient.SqlTransaction tr = null;
        string mes_code = "";
        string mes_checker = "";//审核人
        string file_name = "Parameter_" + Interface_num + "_" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day;
        try
        {
            //记录日志
            string parm_in = "单据日期：" + dDate + " 账套：" + acc_id + "；" + VendorIO.DataTableToJson(dtHead) + "\r\n" + VendorIO.DataTableToJson(dtBody);
            VendorIO.WriteDebug(parm_in, file_name + "_Add");
            #region //校验
            Conn = U8Operation.OpenDataConnection();
            if (Conn == null) throw new Exception("数据库连接失败！");
            CHeckParm(dtHead, dtBody, cUserName, dDate);

            U8StandSCMBarCode u8 = new U8StandSCMBarCode();
            SqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandTimeout = 120000;//毫秒
            tr = Conn.BeginTransaction();
            Cmd.Transaction = tr;

            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (dbname == "") throw new Exception("账套号错误，日期不在本账套[" + acc_id + "]的有效会计期间内");

            string tid = "";
            string retresult = "";
            mes_code = dtHead.Rows[0]["mes_code"] + "";
            mes_checker = dtHead.Rows[0]["checker"] + "";
            if (mes_code == "") throw new Exception("MES的单据号不能为空");
            //记录日志
            //VendorIO.WriteDebug("MES单据号：" + mes_code + "；接口：" + Interface_num + "。", "Vouch_Add");
            mes_code = "" + mes_code;

            #region //检查单号重复
            //更新单据号
            if (app_Sheet == "U81014")  //采购入库单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord01 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81015")  //产品入库单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord10 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81016")  //材料出库单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord11 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81017") //调拨单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..TransVouch where cTVCode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81018")  //其他出库单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord09 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81019")  //其他入库单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord08 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81020")  //发货退货单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..DispatchList where cDLCode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81021")  //发货退货单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord32 where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81027")  //到货单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..PU_ArrivalVouch where ccode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81035") //委外-生产调拨单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..TransVouch where cTVCode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81038") //盘点单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..CheckVouch where cCVCode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81088") //形态转换单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..AssemVouch where cAVCode='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81089") //销售订单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..SO_SOMain where cSOCode ='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            else if (app_Sheet == "U81099")  //MES外协收料单
            {
                if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..T_CC_OM_Receive_Main where t_rcv_code='" + mes_code + "'") > 0)
                    throw new Exception("本单据已经存在，不能重复传入");
            }
            #endregion


            DataTable SHeadData =u8.GetDtToHeadData(dtHead, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            retresult = u8.Test_SCM_Method(SHeadData, dtBody, dbname, cUserName, dDate, app_Sheet, data_Sheet, Cmd);
            string[] r_result = retresult.Split(',');
            tid = r_result[0];
            if (tid == "") throw new Exception("执行后，获得单据ID错误");
            #endregion
            #region//更新单据号
            if (app_Sheet == "U81014")  //采购入库单
            {
                string crd01_oldcode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select ccode from " + dbname + "..Rdrecord01 where id=" + tid);

                Cmd.CommandText = "update " + dbname + "..Rdrecord01 set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();

                //修改倒冲业务号
                Cmd.CommandText = "update " + dbname + "..Rdrecord11 set cBusCode='" + mes_code + @"' 
                    where cBusCode='" + crd01_oldcode + "' and cBusType='委外倒冲' and cSource = '采购入库单'";
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81015")  //产品入库单
            {
                string crd10_oldcode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select ccode from " + dbname + "..Rdrecord10 where id=" + tid);
                Cmd.CommandText = "update " + dbname + "..Rdrecord10 set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
                //修改倒冲业务号
                Cmd.CommandText = "update " + dbname + "..Rdrecord11 set cBusCode='" + mes_code + @"' 
                    where cBusCode='" + crd10_oldcode + "' and cBusType='生产倒冲' and cSource = '产成品入库单'";
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81016")  //材料出库单
            {
                Cmd.CommandText = "update " + dbname + "..Rdrecord11 set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81017") //调拨单
            {
                Cmd.CommandText = "update " + dbname + "..TransVouchs set cTVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..TransVouch set cTVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();

                //调拨单对应的其他出库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨出库' and cSource = '调拨'";
                Cmd.ExecuteNonQuery();

                //调拨单对应的其他入库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨入库' and cSource = '调拨'";
                Cmd.ExecuteNonQuery();

            }
            else if (app_Sheet == "U81018")  //其他出库单
            {
                Cmd.CommandText = "update " + dbname + "..Rdrecord09 set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81019")  //其他入库单
            {
                Cmd.CommandText = "update " + dbname + "..Rdrecord08 set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81020")  //发货退货单
            {
                Cmd.CommandText = "update " + dbname + "..DispatchList set cDLCode='" + mes_code + "' where dlid=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81021")  //销售出库单
            {
                Cmd.CommandText = "update " + dbname + "..Rdrecord32 set cCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81027")  //到货单
            {
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set ccode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();

                tid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select a.ID from " + dbname + "..rdrecords01 a inner join " + dbname + "..PU_ArrivalVouchs b on a.iArrsId=b.Autoid where b.ID=0" + tid);
                Cmd.CommandText = "update " + dbname + "..RdRecord01 set ccode='" + mes_code + "' where id=0" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81035") //委外-生产调拨单
            {
                Cmd.CommandText = "update " + dbname + "..TransVouchs set cTVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..TransVouch set cTVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();

                //调拨单对应的其他出库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨出库'  and cSource = '调拨'";
                Cmd.ExecuteNonQuery();

                //调拨单对应的其他入库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨入库'  and cSource = '调拨'";
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81038") //盘点单
            {
                Cmd.CommandText = "update " + dbname + "..CheckVouchs set cCVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..CheckVouch set cCVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            else if (app_Sheet == "U81088") //形态转换单
            {
                Cmd.CommandText = "update " + dbname + "..AssemVouchs set cAVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..AssemVouch set cAVCode='" + mes_code + "' where id=" + tid;
                Cmd.ExecuteNonQuery();


                //调拨单对应的其他出库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..AssemVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='转换出库' and cSource = '形态转换'";
                Cmd.ExecuteNonQuery();

                //调拨单对应的其他入库单
                Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + mes_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..AssemVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='转换入库' and cSource = '形态转换'";
                Cmd.ExecuteNonQuery();


            }
            else if (app_Sheet == "U81099")  //MES外协收料单
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_OM_Receive_Main set t_rcv_code='" + mes_code + "' where t_rcv_id=" + tid;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            //ret_str = add_retstr.Replace("@@vcode", mes_code).Replace("@@msg", "成功").Replace("@@result", "1");
            if (r_result.Length > 2)
            {
                ret_str = add_retstr.Replace("@@vcode", mes_code).Replace("@@msg", "成功!" + retresult.Substring(retresult.IndexOf("Message:"))).Replace("@@result", "1");
            }
            else
            {
                ret_str = add_retstr.Replace("@@vcode", mes_code).Replace("@@msg", "成功!").Replace("@@result", "1");
            }
            tr.Commit();
        }
        catch (Exception ex)
        {
            if (tr != null) tr.Rollback();
            ret_str = add_retstr.Replace("@@vcode", mes_code).Replace("@@msg", "ERP接口：" + ex.Message).Replace("@@result", "0");
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

        VendorIO.WriteDebug(mes_code + "    " + ret_str, file_name + "_Ret");
        return ret_str;
    }

    [WebMethod]  //ASN单到货 新增
    public string VouchAdd_0(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        if (!dtBody.Columns.Contains("isource_qty"))
        {
            dtBody.Columns.Add("isource_qty");
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                dtBody.Rows[i]["isource_qty"] = dtBody.Rows[i]["iquantity"];
            }
        }

        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81027", "0");
    }

    [WebMethod]  //普通采购到货 新增
    public string VouchAdd_3(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81028", "3");
    }
    [WebMethod]
    public string VouchAdd_3_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81028", "3");
    }

    [WebMethod]  //普通采购退货 新增
    public string VouchAdd_5(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81028", "5");
    }
    [WebMethod]
    public string VouchAdd_5_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81028", "5");
    }

    [WebMethod]  //委外采购到货 新增
    public string VouchAdd_7(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81029", "7");
    }
    [WebMethod]
    public string VouchAdd_7_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81029", "7");
    }

    [WebMethod]  //委外采购退货 新增
    public string VouchAdd_9(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81029", "9");
    }
    [WebMethod]
    public string VouchAdd_9_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81027", "U81029", "9");
    }

    [WebMethod]  //库房调拨单 新增
    public string VouchAdd_11(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81017", "U81031", "11");
    }
    [WebMethod]
    public string VouchAdd_11_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81017", "U81031", "11");
    }

    [WebMethod]  //委外调拨 新增
    public string VouchAdd_12(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81035", "U81037", "12");
    }
    [WebMethod]
    public string VouchAdd_12_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81035", "U81037", "12");
    }

    [WebMethod]  //寄售调拨 新增
    public string VouchAdd_14(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81017", "U81017", "14");
    }
    [WebMethod]
    public string VouchAdd_14_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81017", "U81017", "14");
    }

    [WebMethod]  //其他入库单 新增
    public string VouchAdd_16(DataTable dtHead, DataTable dtBody,string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81019", "U81019", "16");
    }
    [WebMethod]
    public string VouchAdd_16_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81019", "U81019", "16");
    }

    [WebMethod]  //其他出库单 新增
    public string VouchAdd_18(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81018", "U81018", "18");
    }
    [WebMethod]
    public string VouchAdd_18_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81018", "U81018", "18");
    }

    [WebMethod]  //盘点单 新增
    public string VouchAdd_22(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81038", "U81038", "22");
    }
    [WebMethod]
    public string VouchAdd_22_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81038", "U81038", "22");
    }

    [WebMethod]  //委外材料出库 新增
    public string VouchAdd_32(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81016", "U81016", "32");
    }
    [WebMethod]
    public string VouchAdd_32_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81016", "U81016", "32");
    }

    [WebMethod]  //生产材料出库 新增
    public string VouchAdd_33(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81016", "U81016", "33");
    }
    [WebMethod] 
    public string VouchAdd_33_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81016", "U81016", "33");
    }

    [WebMethod]  //产成品入库单 新增
    public string VouchAdd_40(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81015", "U81015", "40");
    }
    [WebMethod] 
    public string VouchAdd_40_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81015", "U81015", "40");
    }

    [WebMethod]  //销售发货单 新增
    public string VouchAdd_43(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81020", "U81020", "43");
    }
    [WebMethod]
    public string VouchAdd_43_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81020", "U81020", "43");
    }

    [WebMethod]  //销售退货单 新增
    public string VouchAdd_45(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81020", "U81020", "45");
    }
    [WebMethod]
    public string VouchAdd_45_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81020", "U81020", "45");
    }

    [WebMethod]  //形态转换单 新增
    public string VouchAdd_47(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81088", "U81088", "47");
    }
    [WebMethod]
    public string VouchAdd_47_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81088", "U81088", "47");
    }

    [WebMethod]  //销售订单 新增
    public string VouchAdd_48(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81089", "U81089", "48");
    }
    [WebMethod]
    public string VouchAdd_48_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81089", "U81089", "48");
    }


    [WebMethod]  //工序外协收料单 新增
    public string VouchAdd_49(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        string ret_str = "";
        System.Data.SqlClient.SqlConnection Conn = null;
        System.Data.SqlClient.SqlTransaction tr = null;
        try
        {
            #region //校验
            Conn = U8Operation.OpenDataConnection();
            if (Conn == null) throw new Exception("数据库连接失败！");
            CHeckParm(dtHead, dtBody, cUserName, dDate);

            U8StandSCMBarCode u8 = new U8StandSCMBarCode();
            SqlCommand Cmd = Conn.CreateCommand(); tr = Conn.BeginTransaction();
            Cmd.Transaction = tr;
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (dbname == "") throw new Exception("账套号错误，日期不在本账套[" + acc_id + "]的有效会计期间内");

            string mes_code = dtHead.Rows[0]["mes_code"] + "";
            if (mes_code == "") throw new Exception("MES的单据号不能为空");
            //记录日志
            //VendorIO.WriteDebug("MES单据号：" + mes_code + "；接口：49。", "Vouch_Add");

            mes_code = "" + mes_code;

            string cvencode = "" + dtHead.Rows[0]["t_vencode"];
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..vendor where cvencode='" + cvencode + "'", Cmd) == 0) 
                throw new Exception("供应商编码[" + cvencode + "]ERP 中不存在");
            #endregion

            Cmd.CommandText = @"INSERT INTO " + dbname + @"..T_CC_OM_Receive_Main
                    ([t_rcv_code],[t_vencode],[t_rcv_date],[t_rcv_maker],[t_rcv_checker],[t_rcv_chkdate],[t_depcode],[t_fina_checker])
                 VALUES('" + dtHead.Rows[0]["mes_code"] + @"','" + cvencode + @"','" + dDate + @"','" + cUserName + @"',
                       '" + dtHead.Rows[0]["checker"] + "','" + dDate + "',null,null)";
            Cmd.ExecuteNonQuery();
            string tid = "" + U8Operation.GetDataString("select IDENT_CURRENT( '" + dbname + @"..T_CC_OM_Receive_Main' )", Cmd);
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                string cinvcode = "" + dtBody.Rows[i]["t_invcode"];
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..inventory where cinvcode='" + cinvcode + "'", Cmd) == 0)
                    throw new Exception("存货编码[" + cinvcode + "]ERP 中不存在");
                string copcode = "" + dtBody.Rows[i]["t_opcode"];
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..sfc_operation where OpCode='" + copcode + "'", Cmd) == 0)
                    throw new Exception("工序编码[" + copcode + "]ERP 中不存在");

                Cmd.CommandText = @"insert into " + dbname + @"..T_CC_OM_Receive_Detail (t_rcv_id,t_card_no,t_modid,t_mocode,t_invcode,t_opcode,t_valid,
                    t_flow_id,t_rpt_c_id,t_dayorderid,
                    ttaxcost ,ttaxrate , ttaxmoney,tmoney , ttaxm
                    )
                    values(
                    " + tid + ",'" + dtBody.Rows[i]["t_card_no"] + "'," + dtBody.Rows[i]["t_modid"] + ",'" + dtBody.Rows[i]["t_mocode"] + @"',
                    '" + cinvcode + "','" + copcode + "'," + dtBody.Rows[i]["t_valid"] + @",0,0,0,
                    " + dtBody.Rows[i]["ttaxcost"] + " ," + dtBody.Rows[i]["ttaxrate"] + ", " + dtBody.Rows[i]["ttaxmoney"] + "," + dtBody.Rows[i]["tmoney"] + " , " + dtBody.Rows[i]["ttaxm"] + @"
                    )";
                Cmd.ExecuteNonQuery();
            }

            ret_str = add_retstr.Replace("@@vcode", mes_code).Replace("@@msg", "成功").Replace("@@result", "1");
            tr.Commit();
        }
        catch (Exception ex)
        {
            if (tr != null) tr.Rollback();
            ret_str = add_retstr.Replace("@@vcode", "").Replace("@@msg", "ERP接口：" + ex.Message).Replace("@@result", "0");
            //记录日志
            VendorIO.WriteDebug("接口：49。" + ex.Message, "Vouch_Add");
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
        return ret_str;
    }
    [WebMethod]
    public string VouchAdd_49_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd_49(dtHead, dtBody, acc_id, cUserName, dDate);
    }

    [WebMethod]  //委外采购入库红字
    public string VouchAdd_68(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81014", "U81014", "68");
    }
    [WebMethod]
    public string VouchAdd_68_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81014", "U81014", "68");
    }

    [WebMethod]  //普通采购入库红字
    public string VouchAdd_69(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81014", "U81014", "69");
    }
    [WebMethod]
    public string VouchAdd_69_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81014", "U81014", "69");
    }

    [WebMethod]  //销售出库单
    public string VouchAdd_73(DataTable dtHead, DataTable dtBody, string acc_id, string cUserName, string dDate)
    {
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81021", "U81021", "73");
    }
    [WebMethod]
    public string VouchAdd_73_json(string Head, string Body, string acc_id, string cUserName, string dDate)
    {
        DataTable dtHead = VendorIO.JsonToDataTable(Head);
        DataTable dtBody = VendorIO.JsonToDataTable(Body);
        return VouchAdd(dtHead, dtBody, acc_id, cUserName, dDate, "U81021", "U81021", "73");
    }

    [WebMethod]  //删除接口
    public string VouchDel(string vouchcode, string acc_id, string cUserName, string dDate, int Interface_num)
    {
        string ret_msg = "[{\"result\":\"1\",\"msg\":\"OK\"}]";

        System.Data.SqlClient.SqlConnection Conn = null;
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction=Conn.BeginTransaction();
        try
        {
            #region  //校验
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (dbname == "") throw new Exception("账套号错误，日期不在本账套[" + acc_id + "]的有效会计期间内");
            #endregion

            if (Interface_num == 4) VouchDel_Arr_PT(vouchcode, cUserName, dbname, Cmd);//删除普通采购到货
            if (Interface_num == 8) VouchDel_Arr_WW(vouchcode, cUserName, dbname, Cmd);//删除委外采购到货
            if (Interface_num == 34) VouchDel_MerOut_PT(vouchcode, cUserName, dbname, Cmd);//删除生产领料
            if (Interface_num == 35) VouchDel_MerOut_WW(vouchcode, cUserName, dbname, Cmd);//删除委外领料


            Cmd.Transaction.Commit();
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            ret_msg = "[{\"result\":\"0\",\"msg\":\"" + ex.Message + "\"}]";
        }

        return ret_msg;
    }

    //普通采购到货
    private void VouchDel_Arr_PT(string vouchcode, string cUserName, string dbname, System.Data.SqlClient.SqlCommand sqlCmd)
    {
        //判断是否存在入库单
        string cautord01 = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_auto_rd01'", sqlCmd);
        if (cautord01.CompareTo("true") == 0)   //自动审核模式
        {
            DataTable dtData = U8Operation.GetSqlDataTable(@"select c.autoid,c.id,c.cbaccounter,c.iquantity,convert(varchar(10),d.ddate,120) cdate,c.cPosition
                from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID
                inner join " + dbname + @"..rdrecords01 c on b.Autoid=c.iArrsId inner join " + dbname + @"..rdrecord01 d on c.id=d.id
                where a.cCode='" + vouchcode + "'", "Table", sqlCmd);
            string ids = "0";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (dtData.Rows[i]["cbaccounter"] + "" != "") throw new Exception("到货对应的入库单财务已经记账");
                string cPeriod = U8Operation.GetDataString(@"select iId 
                    from ufsystem..UA_Period where cAcc_Id=SUBSTRING('" + dbname + "',8,3) and dBegin<='" + dtData.Rows[i]["cdate"] + "' and dEnd>='" + dtData.Rows[i]["cdate"] + "'", sqlCmd);
                if (U8Operation.GetDataInt("select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..GL_mend where iyear=year('" + dtData.Rows[i]["cdate"] + "') and iperiod=0" + cPeriod,sqlCmd) > 0) 
                    throw new Exception("到货单对应的入库单已经结账");
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..PurSettleVouchs where iRdsID=0" + dtData.Rows[i]["autoid"],sqlCmd) > 0)
                    throw new Exception("到货单对应的入库单已经结算");
                ids += "," + dtData.Rows[i]["id"];

                //修改库存
                KK_U8Com.U8Common.U8V10DeleteCurrentStockFromRdRow(sqlCmd, dbname + "..", dtData.Rows[i]["autoid"] + "", "rdrecords01");
            }

            //删除
            DataTable dtDel = U8Operation.GetSqlDataTable(@"select c.autoid,c.id from " + dbname + @"..rdrecords01 c where c.id in(" + ids + ")", "Table", sqlCmd);
            if (dtData.Rows.Count != dtDel.Rows.Count) throw new Exception("到货单未直接入库，不能自动删除");
            for (int i = 0; i < dtDel.Rows.Count; i++)
            {
                sqlCmd.CommandText = "delete from " + dbname + @"..rdrecords01 where id=" + dtDel.Rows[i]["id"];
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "delete from " + dbname + @"..rdrecord01 where id=" + dtDel.Rows[i]["id"];
                sqlCmd.ExecuteNonQuery();
            }
        }
        


        //
        if (U8Operation.GetDataInt(@"select count(*) from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID
                inner join " + dbname + "..rdrecords01 c on b.Autoid=c.iArrsId where a.cCode='" + vouchcode + "'", sqlCmd) > 0)
            throw new Exception("已经入库");

        if (U8Operation.GetDataInt(@"select count(*) from " + dbname + @"..PU_ArrivalVouchs a
                where iCorId where a.iCorId in(select b.autoid from " + dbname + @"..PU_ArrivalVouch a 
                inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID where a.ccode='" + vouchcode + "')", sqlCmd) > 0)
            throw new Exception("已经退货");

        //回写采购订单的退货数
        DataTable dtAutoid = U8Operation.GetSqlDataTable(@"select a.cbustype,b.iPOsID,b.iQuantity,b.iNum  from " + dbname + @"..PU_ArrivalVouch a 
                inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID 
                where a.ccode='" + vouchcode + "' and b.iPOsID is not null", "Table", sqlCmd);

        sqlCmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouchs where id in(select id from " + dbname + @"..PU_ArrivalVouch where ccode='" + vouchcode + "')";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouch where ccode='" + vouchcode + "'";
        sqlCmd.ExecuteNonQuery();

        for (int i = 0; i < dtAutoid.Rows.Count; i++)
        {
            if (i == 0 && dtAutoid.Rows[i]["cbustype"] + "" != "普通采购") throw new Exception("本单据非 普通采购");

            sqlCmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)-(0" + dtAutoid.Rows[i]["iQuantity"] + "),iArrNum=isnull(iArrNum,0)-(0" + dtAutoid.Rows[i]["iNum"] + @") 
                where id=0" + dtAutoid.Rows[i]["iPOsID"];
            sqlCmd.ExecuteNonQuery();
            sqlCmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + dtAutoid.Rows[i]["iPOsID"];
            sqlCmd.ExecuteNonQuery();
        }
    }

    //委外 采购到货
    private void VouchDel_Arr_WW(string vouchcode, string cUserName, string dbname, System.Data.SqlClient.SqlCommand sqlCmd)
    {
        //判断是否存在入库单
        string cautord01 = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_auto_rd01'", sqlCmd);
        if (cautord01.CompareTo("true") == 0)   //自动审核模式
        {
            DataTable dtData = U8Operation.GetSqlDataTable(@"select c.autoid,c.id,c.cbaccounter,c.iquantity,convert(varchar(10),d.ddate,120) cdate,c.cPosition
                from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID
                inner join " + dbname + @"..rdrecords01 c on b.Autoid=c.iArrsId inner join " + dbname + @"..rdrecord01 d on c.id=d.id
                where a.cCode='" + vouchcode + "'", "Table", sqlCmd);
            string ids = "0";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (dtData.Rows[i]["cbaccounter"] + "" != "") throw new Exception("到货对应的入库单财务已经记账");
                string cPeriod = U8Operation.GetDataString(@"select iId 
                    from ufsystem..UA_Period where cAcc_Id=SUBSTRING('" + dbname + "',8,3) and dBegin<='" + dtData.Rows[i]["cdate"] + "' and dEnd>='" + dtData.Rows[i]["cdate"] + "'", sqlCmd);
                if (U8Operation.GetDataInt("select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..GL_mend where iyear=year('" + dtData.Rows[i]["cdate"] + "') and iperiod=0" + cPeriod, sqlCmd) > 0)
                    throw new Exception("到货单对应的入库单已经结账");
                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..PurSettleVouchs where iRdsID=0" + dtData.Rows[i]["autoid"], sqlCmd) > 0)
                    throw new Exception("到货单对应的入库单已经结算");
                ids += "," + dtData.Rows[i]["id"];

                //修改库存
                KK_U8Com.U8Common.U8V10DeleteCurrentStockFromRdRow(sqlCmd, dbname + "..", dtData.Rows[i]["autoid"] + "", "rdrecords01");
            }

            //删除
            DataTable dtDel = U8Operation.GetSqlDataTable(@"select c.autoid,c.id from " + dbname + @"..rdrecords01 c where c.id in(" + ids + ")", "Table", sqlCmd);
            if (dtData.Rows.Count != dtDel.Rows.Count) throw new Exception("到货单未直接入库，不能自动删除");
            for (int i = 0; i < dtDel.Rows.Count; i++)
            {
                sqlCmd.CommandText = "delete from " + dbname + @"..rdrecords01 where id=" + dtDel.Rows[i]["id"];
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "delete from " + dbname + @"..rdrecord01 where id=" + dtDel.Rows[i]["id"];
                sqlCmd.ExecuteNonQuery();
            }
        }
        
        //
        if (U8Operation.GetDataInt(@"select count(*) from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID
                inner join " + dbname + "..rdrecords01 c on b.Autoid=c.iArrsId where a.cCode='" + vouchcode + "'", sqlCmd) > 0)
            throw new Exception("已经入库");

        if (U8Operation.GetDataInt(@"select count(*) from " + dbname + @"..PU_ArrivalVouchs a
                where iCorId where a.iCorId in(select b.autoid from " + dbname + @"..PU_ArrivalVouch a 
                inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID where a.ccode='" + vouchcode + "')", sqlCmd) > 0)
            throw new Exception("已经退货");

        //回写采购订单的退货数
        DataTable dtAutoid = U8Operation.GetSqlDataTable(@"select a.cbustype,b.iPOsID,b.iQuantity,b.iNum  from " + dbname + @"..PU_ArrivalVouch a 
                inner join " + dbname + @"..PU_ArrivalVouchs b on a.ID=b.ID 
                where a.ccode='" + vouchcode + "' and b.iPOsID is not null", "Table", sqlCmd);

        sqlCmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouchs where id in(select id from " + dbname + @"..PU_ArrivalVouch where ccode='" + vouchcode + "')";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouch where ccode='" + vouchcode + "'";
        sqlCmd.ExecuteNonQuery();

        for (int i = 0; i < dtAutoid.Rows.Count; i++)
        {
            if (i == 0 && dtAutoid.Rows[i]["cbustype"] + "" != "委外采购") throw new Exception("本单据非 委外业务");

            sqlCmd.CommandText = "update " + dbname + "..OM_MODetails set iArrQTY=isnull(iArrQTY,0)-(0" + dtAutoid.Rows[i]["iQuantity"] + @") 
                where MODetailsID=0" + dtAutoid.Rows[i]["iPOsID"];
            sqlCmd.ExecuteNonQuery();
        }
    }

    //生产材料出库
    private void VouchDel_MerOut_PT(string vouchcode, string cUserName, string dbname, System.Data.SqlClient.SqlCommand sqlCmd)
    {
        DataTable dtData = U8Operation.GetSqlDataTable(@"select d.cbustype,c.autoid,c.cbaccounter,c.iquantity,convert(varchar(10),d.ddate,120) cdate,c.cPosition,c.iMPoIds
                from " + dbname + @"..rdrecords11 c inner join " + dbname + @"..rdrecord11 d on c.id=d.id
                where a.cCode='" + vouchcode + "'", "Table", sqlCmd);
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
            if (dtData.Rows[i]["cbaccounter"] + "" != "") throw new Exception("出库单财务已经记账");
            if (i == 0 && dtData.Rows[i]["cbustype"] + "" != "领料") throw new Exception("本单据非 生产订单领料业务");

            if (i == 0)
            {
                string cPeriod = U8Operation.GetDataString(@"select iId 
                    from ufsystem..UA_Period where cAcc_Id=SUBSTRING('" + dbname + "',8,3) and dBegin<='" + dtData.Rows[i]["cdate"] + "' and dEnd>='" + dtData.Rows[i]["cdate"] + "'", sqlCmd);
                if (U8Operation.GetDataInt("select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..GL_mend where iyear=year('" + dtData.Rows[i]["cdate"] + "') and iperiod=0" + cPeriod, sqlCmd) > 0)
                    throw new Exception("出库单已经结账");
            }
            //回写材料表
            sqlCmd.CommandText = "update " + dbname + "..mom_moallocate set issqty=round(isnull(issqty,0)-(0" + dtData.Rows[i]["iquantity"] + "),6) where AllocateId =0" + dtData.Rows[i]["iMPoIds"];
            sqlCmd.ExecuteNonQuery();

            //修改库存
            KK_U8Com.U8Common.U8V10DeleteCurrentStockFromRdRow(sqlCmd, dbname + "..", dtData.Rows[i]["autoid"] + "", "rdrecords11");
        }

        sqlCmd.CommandText = "delete from " + dbname + @"..rdrecords11 where id in(select id from " + dbname + @"..rdrecord11 where ccode='" + vouchcode + "')";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "delete from " + dbname + @"..rdrecord11 where ccode='" + vouchcode + "'";
        sqlCmd.ExecuteNonQuery();

    }

    //委外材料出库
    private void VouchDel_MerOut_WW(string vouchcode, string cUserName, string dbname, System.Data.SqlClient.SqlCommand sqlCmd)
    {
        DataTable dtData = U8Operation.GetSqlDataTable(@"select d.cbustype,c.autoid,c.cbaccounter,c.iquantity,convert(varchar(10),d.ddate,120) cdate,c.cPosition,c.iOMoMID
                from " + dbname + @"..rdrecords11 c inner join " + dbname + @"..rdrecord11 d on c.id=d.id
                where a.cCode='" + vouchcode + "'", "Table", sqlCmd);
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
            if (dtData.Rows[i]["cbaccounter"] + "" != "") throw new Exception("出库单财务已经记账");
            if (i == 0 && dtData.Rows[i]["cbustype"] + "" != "委外发料") throw new Exception("本单据非 委外发料业务");

            if (i == 0)
            {
                string cPeriod = U8Operation.GetDataString(@"select iId 
                    from ufsystem..UA_Period where cAcc_Id=SUBSTRING('" + dbname + "',8,3) and dBegin<='" + dtData.Rows[i]["cdate"] + "' and dEnd>='" + dtData.Rows[i]["cdate"] + "'", sqlCmd);
                if (U8Operation.GetDataInt("select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..GL_mend where iyear=year('" + dtData.Rows[i]["cdate"] + "') and iperiod=0" + cPeriod, sqlCmd) > 0)
                    throw new Exception("出库单已经结账");
            }
            //回写材料表
            sqlCmd.CommandText = "update " + dbname + "..OM_MOMaterials set iSendQTY=round(isnull(iSendQTY,0)-(0" + dtData.Rows[i]["iquantity"] + "),6) where MOMaterialsID =0" + dtData.Rows[i]["iOMoMID"];
            sqlCmd.ExecuteNonQuery();

            //修改库存
            KK_U8Com.U8Common.U8V10DeleteCurrentStockFromRdRow(sqlCmd, dbname + "..", dtData.Rows[i]["autoid"] + "", "rdrecords11");
        }

        sqlCmd.CommandText = "delete from " + dbname + @"..rdrecords11 where id in(select id from " + dbname + @"..rdrecord11 where ccode='" + vouchcode + "')";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "delete from " + dbname + @"..rdrecord11 where ccode='" + vouchcode + "'";
        sqlCmd.ExecuteNonQuery();

    }

    [WebMethod]
    public string VouchAdd_json(string Head, string Body, string acc_id,string sheetid, string cUserName, string dDate)
    {
        string ret_str = "";
        System.Data.SqlClient.SqlConnection Conn = null;
        System.Data.SqlClient.SqlTransaction tr = null;
        string vou_code = "";
        string file_name = "Parameter_" + sheetid + "_" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day;
        try
        {
            //记录日志
            //string parm_in = "单据日期：" + dDate + " 账套：" + acc_id + "；" + Head + "\r\n" + Body;
            //VendorIO.WriteDebug(parm_in, file_name + "_Add");
            #region //校验
            DataTable dtHead = VendorIO.JsonToDataTable(Head);
            DataTable dtBody = VendorIO.JsonToDataTable(Body);

            Conn = U8Operation.OpenDataConnection();
            if (Conn == null) throw new Exception("数据库连接失败！");
            //CHeckParm(dtHead, dtBody, cUserName, dDate);

            U8StandSCMBarCode u8 = new U8StandSCMBarCode();
            SqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandTimeout = 120000;//毫秒
            tr = Conn.BeginTransaction();
            Cmd.Transaction = tr;

            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (dbname == "") throw new Exception("账套号错误，日期不在本账套[" + acc_id + "]的有效会计期间内");

            string tid = "";
            vou_code = dtHead.Rows[0]["vou_code"] + "";
            //if (vou_code == "") throw new Exception("MES的单据号不能为空");

            #region //检查单号重复
            string app_Sheet = sheetid;
            if (vou_code != "")
            {
                //更新单据号
                if (app_Sheet == "U81014")  //采购入库单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord01 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81015")  //产品入库单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord10 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81016")  //材料出库单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord11 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81017") //调拨单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..TransVouch where cTVCode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81018")  //其他出库单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord09 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81019")  //其他入库单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord08 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81020")  //发货退货单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..DispatchList where cDLCode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81021")  //发货退货单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..Rdrecord32 where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81027")  //到货单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..PU_ArrivalVouch where ccode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81035") //委外-生产调拨单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..TransVouch where cTVCode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81038") //盘点单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..CheckVouch where cCVCode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81088") //形态转换单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..AssemVouch where cAVCode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81089") //销售订单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..SO_SOMain where csocode='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
                else if (app_Sheet == "U81099")  //MES外协收料单
                {
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select count(*) from " + dbname + "..T_CC_OM_Receive_Main where t_rcv_code='" + vou_code + "'") > 0)
                        throw new Exception("本单据已经存在，不能重复传入");
                }
            }
            #endregion

            
            DataTable SHeadData = u8.GetDtToHeadData(dtHead, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            tid = u8.Test_SCM_Method(SHeadData, dtBody, dbname, cUserName, dDate, sheetid, sheetid, Cmd);
            if (tid == "") throw new Exception("执行后，获得单据ID错误");
            #endregion

            #region//更新单据号
            if (vou_code != "")
            {
                if (app_Sheet == "U81014")  //采购入库单
                {
                    string crd01_oldcode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select ccode from " + dbname + "..Rdrecord01 where id=" + tid);

                    Cmd.CommandText = "update " + dbname + "..Rdrecord01 set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();

                    //修改倒冲业务号
                    Cmd.CommandText = "update " + dbname + "..Rdrecord11 set cBusCode='" + vou_code + @"' 
                    where cBusCode='" + crd01_oldcode + "' and cBusType='委外倒冲' and cSource = '采购入库单'";
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81015")  //产品入库单
                {
                    string crd10_oldcode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select ccode from " + dbname + "..Rdrecord10 where id=" + tid);
                    Cmd.CommandText = "update " + dbname + "..Rdrecord10 set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                    //修改倒冲业务号
                    Cmd.CommandText = "update " + dbname + "..Rdrecord11 set cBusCode='" + vou_code + @"' 
                    where cBusCode='" + crd10_oldcode + "' and cBusType='生产倒冲' and cSource = '产成品入库单'";
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81016")  //材料出库单
                {
                    Cmd.CommandText = "update " + dbname + "..Rdrecord11 set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81017") //调拨单
                {
                    Cmd.CommandText = "update " + dbname + "..TransVouchs set cTVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..TransVouch set cTVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他出库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨出库' and cSource = '调拨'";
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他入库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨入库' and cSource = '调拨'";
                    Cmd.ExecuteNonQuery();

                }
                else if (app_Sheet == "U81018")  //其他出库单
                {
                    Cmd.CommandText = "update " + dbname + "..Rdrecord09 set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81019")  //其他入库单
                {
                    Cmd.CommandText = "update " + dbname + "..Rdrecord08 set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81020")  //发货退货单
                {
                    Cmd.CommandText = "update " + dbname + "..DispatchList set cDLCode='" + vou_code + "' where dlid=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81021")  //销售出库单
                {
                    Cmd.CommandText = "update " + dbname + "..Rdrecord32 set cCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81027")  //到货单
                {
                    Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set ccode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();

                    tid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select a.ID from " + dbname + "..rdrecords01 a inner join " + dbname + "..PU_ArrivalVouchs b on a.iArrsId=b.Autoid where b.ID=0" + tid);
                    Cmd.CommandText = "update " + dbname + "..RdRecord01 set ccode='" + vou_code + "' where id=0" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81035") //委外-生产调拨单
                {
                    Cmd.CommandText = "update " + dbname + "..TransVouchs set cTVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..TransVouch set cTVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他出库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨出库'  and cSource = '调拨'";
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他入库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..TransVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='调拨入库'  and cSource = '调拨'";
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81038") //盘点单
                {
                    Cmd.CommandText = "update " + dbname + "..CheckVouchs set cCVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..CheckVouch set cCVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81088") //形态转换单
                {
                    Cmd.CommandText = "update " + dbname + "..AssemVouchs set cAVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..AssemVouch set cAVCode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他出库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord09 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords09 a 
                    inner join " + dbname + "..AssemVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='转换出库' and cSource = '形态转换'";
                    Cmd.ExecuteNonQuery();

                    //调拨单对应的其他入库单
                    Cmd.CommandText = "update " + dbname + "..Rdrecord08 set cBusCode='" + vou_code + @"' 
                    where id in(select distinct a.id from " + dbname + @"..rdrecords08 a 
                    inner join " + dbname + "..AssemVouchs b on a.iTrIds=b.autoID and b.id=" + tid + ") and cbustype='转换入库' and cSource = '形态转换'";
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81089") //销售订单
                {
                    Cmd.CommandText = "update " + dbname + "..SO_SOMain set csocode='" + vou_code + "' where id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
                else if (app_Sheet == "U81099")  //MES外协收料单
                {
                    Cmd.CommandText = "update " + dbname + "..T_CC_OM_Receive_Main set t_rcv_code='" + vou_code + "' where t_rcv_id=" + tid;
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion

            //vou_code
            ret_str = add_retstr.Replace("@@vcode", vou_code).Replace("@@msg", "成功").Replace("@@result", "1");
            tr.Commit();
        }
        catch (Exception ex)
        {
            if (tr != null) tr.Rollback();
            ret_str = add_retstr.Replace("@@vcode", vou_code).Replace("@@msg", "ERP接口：" + ex.Message).Replace("@@result", "0");
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

        //VendorIO.WriteDebug(vou_code + "    " + ret_str, file_name + "_Ret");
        return ret_str;
    }





    //销售订单变更
    [WebMethod]
    public string SO_Order_Change_json(string Body_JSON, string acc_id, string cUserName)
    {
        if (Body_JSON == "") return add_retstr.Replace("@@vcode", "拆分销售订单").Replace("@@msg", "ERP接口：无有效传入数据").Replace("@@result", "0");

        SqlConnection sqlCon = U8Operation.OpenDataConnection();
        if (sqlCon == null) return add_retstr.Replace("@@vcode", "拆分销售订单").Replace("@@msg", "ERP接口：数据库连接失败").Replace("@@result", "0");

        SqlCommand Cmmd = sqlCon.CreateCommand();
        SqlTransaction tr = sqlCon.BeginTransaction();
        Cmmd.Transaction = tr;
        try
        {
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmmd);
            if (dbname == "") throw new Exception("账套号错误，当前日期不在本账套[" + acc_id + "]的有效会计期间内");

            DataTable dtBody = VendorIO.JsonToDataTable(Body_JSON);
            if (dtBody.Rows.Count == 0) throw new Exception("Body_JSON 无有效行参数");
            foreach (DataRow dr in dtBody.Rows)
            {
                decimal d_qty = Convert.ToDecimal(dr["ichai_qty"]);
                string cbmemo = "";
                if (dtBody.Columns.Contains("cbmemo")) cbmemo = dr["cbmemo"] + "";
                decimal d_num = 0;
                if (dtBody.Columns.Contains("ichai_num")) d_num = Convert.ToDecimal(dr["ichai_num"]);
                DataTable dtSo = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmmd, @"select b.id,iquantity,isnull(ifhquantity,0) ifhquantity,cinvcode,b.ddate,b.csocode 
                    from " + dbname + @"..SO_SODetails a inner join " + dbname + @"..SO_SOMain b on a.id=b.ID where iSOsID=0" + dr["isosid"]);
                if (dtSo.Rows.Count == 0) throw new Exception("订单行id[" + dr["isosid"] + "]无未找到");
                decimal so_qty = Convert.ToDecimal(dtSo.Rows[0]["iquantity"]);
                decimal so_fh = Convert.ToDecimal(dtSo.Rows[0]["ifhquantity"]);

                if (d_qty > so_qty - so_fh) throw new Exception("订单【" + dtSo.Rows[0]["csocode"] + "】存货【" + dtSo.Rows[0]["cinvcode"] + "】最大可拆分量为：" + (so_qty - so_fh));
                if (d_qty == so_qty)
                {
                    Cmmd.CommandText = "update " + dbname + @"..SO_SODetails set dPreDate='" + dr["newdate"] + "' where isosid=" + dr["isosid"];
                    Cmmd.ExecuteNonQuery();
                    Cmmd.CommandText = "update " + dbname + @"..SO_SOMain set cmodifier='" + cUserName + "',dmoddate=getdate() where id=" + dtSo.Rows[0]["id"];
                    Cmmd.ExecuteNonQuery();
                    continue;
                }

                string cLogDate = "" + dtSo.Rows[0]["ddate"];
                int rd_id = 1000000000 + UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmmd, "select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + acc_id + "' and cVouchType='Somain'");
                Cmmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + acc_id + "' and cVouchType='Somain'";
                Cmmd.ExecuteNonQuery();
                string cCodeHead = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)");
                string cc_mcode = cCodeHead + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select right('000'+cast(cast(isnull(right(max(cSOCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + @"..SO_SOMain where cSOCode like '" + cCodeHead + "%'");

                Cmmd.CommandText = @"insert into " + dbname + @"..SO_SOMain(cSTCode, dDate, cSOCode, cCusCode, cDepCode, cPersonCode, cSCCode, cCusOAddress, cPayCode, cexch_name, iExchRate, iTaxRate, 
                        iMoney, cMemo, iStatus, cMaker, cVerifier, cCloser, bDisFlag, cDefine1, cDefine2, cDefine3, cDefine4, cDefine5, cDefine6, cDefine7, cDefine8, cDefine9, 
                        cDefine10, bReturnFlag, cCusName, bOrder, ID, iVTid, cChanger, cBusType, cCreChpName, cDefine11, cDefine12, cDefine13, cDefine14, cDefine15, 
                        cDefine16, coppcode, cLocker, dPreMoDateBT, dPreDateBT, cgatheringplan, caddcode, iverifystate, ireturncount, iswfcontrolled, icreditstate, cmodifier, 
                        dmoddate, dverifydate, ccusperson, dcreatesystime, dverifysystime, dmodifysystime, iflowid, bcashsale, cgathingcode, cChangeVerifier, 
                        dChangeVerifyDate, dChangeVerifyTime, outid, ccuspersoncode, dclosedate, dclosesystime, iPrintCount, fbookratio, bmustbook, fbooksum, fbooknatsum, 
                        fgbooksum, fgbooknatsum, csvouchtype, cCrmPersonCode, cCrmPersonName, cMainPersonCode, cSysBarCode, ioppid, optnty_name, cCurrentAuditor, 
                        contract_status, csscode, cinvoicecompany, cAttachment, cEBTrnumber, cEBBuyer, cEBBuyerNote, ccontactname, cEBprovince, cEBcity, cEBdistrict, 
                        cmobilephone, cInvoiceCusName, cGCRouteCode) 
                        select cSTCode, dDate,  '" + cc_mcode + @"', cCusCode, cDepCode, cPersonCode,cSCCode, cCusOAddress, cPayCode, cexch_name, iExchRate, iTaxRate, 
                        iMoney, cMemo, iStatus, cMaker, cVerifier, cCloser, bDisFlag, cDefine1, cDefine2, cDefine3, cDefine4, cDefine5, cDefine6, cDefine7, cDefine8, cDefine9, 
                        cDefine10, bReturnFlag, cCusName, bOrder, " + rd_id + @", iVTid, cChanger, cBusType, cCreChpName, cDefine11, cDefine12, cDefine13, cDefine14, cDefine15, 
                        cDefine16, coppcode, cLocker, dPreMoDateBT, dPreDateBT, cgatheringplan, caddcode, iverifystate, ireturncount, 0, icreditstate, '" + cUserName + @"', 
                        getdate(), dverifydate, ccusperson, dcreatesystime, dverifysystime, getdate(), iflowid, bcashsale, cgathingcode, cChangeVerifier, 
                        dChangeVerifyDate, dChangeVerifyTime, outid, ccuspersoncode, dclosedate, dclosesystime, iPrintCount, fbookratio, bmustbook, fbooksum, fbooknatsum, 
                        fgbooksum, fgbooknatsum, csvouchtype, cCrmPersonCode, cCrmPersonName, cMainPersonCode, cSysBarCode, ioppid, optnty_name, cCurrentAuditor, 
                        contract_status, csscode, cinvoicecompany, cAttachment, cEBTrnumber, cEBBuyer, cEBBuyerNote, ccontactname, cEBprovince, cEBcity, cEBdistrict, 
                        cmobilephone, cInvoiceCusName, cGCRouteCode 
                        from " + dbname + @"..SO_SOMain where id=" + dtSo.Rows[0]["id"];
                Cmmd.ExecuteNonQuery();

                int cAutoid = UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmmd, "select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + acc_id + "' and cVouchType='Somain'");
                Cmmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + acc_id + "' and cVouchType='Somain'";
                Cmmd.ExecuteNonQuery();

                Cmmd.CommandText = @"insert into " + dbname + @"..SO_SODetails(cSOCode, cInvCode, dPreDate, iQuantity, iNum, iQuotedPrice, iUnitPrice, iTaxUnitPrice, iMoney, iTax, iSum, iDisCount, iNatUnitPrice, 
                        iNatMoney, iNatTax, iNatSum, iNatDisCount, iFHNum, iFHQuantity, iFHMoney, iKPQuantity, iKPNum, iKPMoney, cMemo, cFree1, cFree2, bFH, iSOsID, KL, 
                        KL2, cInvName, iTaxRate, cDefine22, cDefine23, cDefine24, cDefine25, cDefine26, cDefine27, cItemCode, cItem_class, cItemName, cItem_CName, cFree3, 
                        cFree4, cFree5, cFree6, cFree7, cFree8, cFree9, cFree10, iInvExchRate, cUnitID, ID, cDefine28, cDefine29, cDefine30, cDefine31, cDefine32, cDefine33, 
                        cDefine34, cDefine35, cDefine36, cDefine37, FPurQuan, fSaleCost, fSalePrice, cQuoCode, iQuoID, cSCloser, dPreMoDate, iRowNo, iCusBomID, 
                        iMoQuantity, cContractID, cContractTagCode, cContractRowGuid, iPPartSeqID, iPPartID, iPPartQty, cCusInvCode, cCusInvName, iPreKeepQuantity, 
                        iPreKeepNum, iPreKeepTotQuantity, iPreKeepTotNum, dreleasedate, fcusminprice, fimquantity, fomquantity, ballpurchase, finquantity, icostquantity, 
                        icostsum, foutquantity, foutnum, iexchsum, imoneysum,iaoids, cpreordercode, fretquantity, fretnum, dbclosedate, dbclosesystime, bOrderBOM, 
                        bOrderBOMOver, idemandtype, cdemandcode, cdemandmemo, fPurSum, fPurBillQty, fPurBillSum, iimid, ccorvouchtype, icorrowno, busecusbom, 
                        body_outid, fVeriDispQty, fVeriDispSum, bsaleprice, bgift, forecastdid, cdetailsdemandcode, cdetailsdemandmemo, fTransedQty, cbSysBarCode, fappqty, 
                        cParentCode, cChildCode, fchildqty, fchildrate, iCalcType, iCurPartid, cFactoryCode, GCSourceId, GCSourceIds)
                        select '" + cc_mcode + @"', cInvCode, '" + dr["newdate"] + "', " + d_qty + ", " + d_num + @", iQuotedPrice, iUnitPrice, iTaxUnitPrice, iMoney, iTax, iSum, iDisCount, iNatUnitPrice, 
                        iNatMoney, iNatTax, iNatSum, iNatDisCount, 0, 0, 0, 0, 0, 0, '" + cbmemo + @"', cFree1, cFree2, bFH, " + cAutoid + @", KL, 
                        KL2, cInvName, iTaxRate, cDefine22, cDefine23, cDefine24, cDefine25, cDefine26, cDefine27, cItemCode, cItem_class, cItemName, cItem_CName, cFree3, 
                        cFree4, cFree5, cFree6, cFree7, cFree8, cFree9, cFree10, iInvExchRate, cUnitID, " + rd_id + @", cDefine28, cDefine29, cDefine30, cDefine31, cDefine32, cDefine33, 
                        cDefine34, cDefine35, cDefine36, cDefine37, FPurQuan, fSaleCost, fSalePrice, cQuoCode, iQuoID, cSCloser, dPreMoDate, iRowNo, iCusBomID, 
                        0, cContractID, cContractTagCode, cContractRowGuid, iPPartSeqID, iPPartID, iPPartQty, cCusInvCode, cCusInvName, iPreKeepQuantity, 
                        iPreKeepNum, iPreKeepTotQuantity, iPreKeepTotNum, dreleasedate, fcusminprice, fimquantity, fomquantity, ballpurchase, finquantity, icostquantity, 
                        icostsum, foutquantity, foutnum, iexchsum, imoneysum,iaoids, cpreordercode, fretquantity, fretnum, dbclosedate, dbclosesystime, bOrderBOM, 
                        bOrderBOMOver, idemandtype, cdemandcode, cdemandmemo, fPurSum, fPurBillQty, fPurBillSum, iimid, ccorvouchtype, icorrowno, busecusbom, 
                        body_outid, fVeriDispQty, fVeriDispSum, bsaleprice, bgift, forecastdid, cdetailsdemandcode, cdetailsdemandmemo, fTransedQty, cbSysBarCode, fappqty, 
                        cParentCode, cChildCode, fchildqty, fchildrate, iCalcType, iCurPartid, cFactoryCode, GCSourceId, GCSourceIds
                        from " + dbname + @"..SO_SODetails where isosid=" + dr["isosid"];
                Cmmd.ExecuteNonQuery();
                //修改原订单数量
                Cmmd.CommandText = "update " + dbname + @"..SO_SODetails set iQuantity=iQuantity-(" + d_qty + @"),
                    iNum=case when iNum-(" + d_num + @")>0 then iNum-(" + d_num + @") else 0 end where isosid=" + dr["isosid"];
                Cmmd.ExecuteNonQuery();
                //更新金额等信息
                Cmmd.CommandText = @"update " + dbname + @"..SO_SODetails set iNatSum=round(iQuantity*iTaxUnitPrice,2),iSum=round(iQuantity*iTaxUnitPrice,2),
                            iNatMoney=round(iQuantity*iUnitPrice,2),iMoney=round(iQuantity*iUnitPrice,2)
                        where isosid in(" + dr["isosid"] + "," + cAutoid + ")";
                Cmmd.ExecuteNonQuery();
                Cmmd.CommandText = @"update " + dbname + @"..SO_SODetails set iNatTax=iNatSum-iNatMoney,iTax=iSum-iMoney
                        where isosid in(" + dr["isosid"] + "," + cAutoid + ")";
                Cmmd.ExecuteNonQuery();
                Cmmd.CommandText = @"update " + dbname + @"..SO_SODetails 
                            set KL=case when isnull(iQuotedPrice,0)*iquantity=0 then 100 else iSum/(iQuotedPrice*iquantity)*100 end,
                            iDisCount=case when isnull(iQuotedPrice,0)*iquantity=0 then 0 else iDisCount end,
                            iNatDisCount=case when isnull(iQuotedPrice,0)*iquantity=0 then 0 else iNatDisCount end
                        where isosid in(" + dr["isosid"] + "," + cAutoid + ")";
                Cmmd.ExecuteNonQuery();
            }
            //保存数据
            string ret_str = add_retstr.Replace("@@vcode", "拆分销售订单").Replace("@@msg", "成功").Replace("@@result", "1");
            tr.Commit();
            return ret_str;
        }
        catch (Exception ex)
        {
            tr.Rollback();
            string ret_str = add_retstr.Replace("@@vcode", "拆分销售订单").Replace("@@msg", "ERP接口：" + ex.Message).Replace("@@result", "0");
            return ret_str;
        }
        finally
        {
            U8Operation.CloseDataConnection(sqlCon);
        }

    }

    //销售订单自动审核
    [WebMethod]
    public string SoMainAutoCheck101(int soid, string acc_id, string cUserName)
    {
        SqlConnection sqlCon = U8Operation.OpenDataConnection();
        if (sqlCon == null) return add_retstr.Replace("@@vcode", "自动审核订单").Replace("@@msg", "ERP接口：数据库连接失败").Replace("@@result", "0");

        SqlCommand Cmmd = sqlCon.CreateCommand();
        try
        {
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmmd);
            if (dbname == "") throw new Exception("账套号错误，当前日期不在本账套[" + acc_id + "]的有效会计期间内");
            
            string ccuscode = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select ccuscode from " + dbname + @"..so_somain where id=" + soid);
            int i_yunshu = UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmmd, @"select count(1) from " + dbname + @"..SO_SOMain a inner join " + dbname + @"..Customer b on a.cCusCode=b.cCusCode 
                where id=" + soid + " and b.cCCCode<>'06' and a.cDefine1 like '%送货%'");
            DataTable dtSoDetail = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmmd, @"select a.cInvCode,b.cInvName,b.cInvCCode,isnull(a.iSum,0) iSum,isnull(b.iVolume,0)*a.iQuantity iVol,
	                case when b.cInvCCode in('0301','0311','0304') then 1 when b.cInvCCode in('031504','031505') then 2 else 3 end iInvType,
                    DATEDIFF(day,getdate(),a.dPreDate) iDays,cast(isnull(b.iInvWeight,0)*a.iQuantity as float) iWeight
                from " + dbname + @"..SO_SODetails a inner join " + dbname + @"..Inventory b on a.cInvCode=b.cInvCode
                where id=0" + soid);
            if (dtSoDetail.Rows.Count == 0) return add_retstr.Replace("@@vcode", "销售订单自动审核").Replace("@@msg", "订单无数据，无需审核").Replace("@@result", "0");//直接退出
            //客户信息
            DataTable dtCus = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmmd, @"select isnull(cCusDefine13,0) gongli,cCusDefine6
                from " + dbname + @"..customer where ccuscode='" + ccuscode + "'");
            if (dtCus.Rows.Count == 0) { return add_retstr.Replace("@@vcode", "销售订单自动审核").Replace("@@msg", "ERP接口：没有找到客户[" + ccuscode + "]").Replace("@@result", "0"); }
            decimal d_gongli = Convert.ToDecimal(dtCus.Rows[0]["gongli"] + "");
            string cusLevel = dtCus.Rows[0]["cCusDefine6"] + "";//客户等级


            bool AutoCheck = true; //默认自动审核
            string NotCheckMessage = "";
            if (AutoCheck)//急单判定
            {
                DataRow[] data = dtSoDetail.Select("(iInvType=2 and iDays<3) or (iInvType=3 and iDays<2)");
                if (data.Length > 0) { AutoCheck = false; NotCheckMessage = "有急单"; }
            }

            if (AutoCheck)//超期款判定
            {
                if (int.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select count(1) from " + dbname + @"..T_CC_KL_Balck where 客户编码='" + ccuscode + "' and 未收金额>0 and 是否超期='是'")) > 0)
                { AutoCheck = false; NotCheckMessage = "有超期款"; }
            }

            //总体积计算
            decimal dVolAll = 0;
            foreach (DataRow dr in dtSoDetail.Rows)
            { dVolAll += Convert.ToDecimal(dr["iVol"]); }
            //总金额计算
            decimal dMoneyAll = 0;
            foreach (DataRow dr in dtSoDetail.Rows)
            { dMoneyAll += Convert.ToDecimal(dr["iSum"]); }

            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '----1----'");

            if (AutoCheck && d_gongli > 60 && dVolAll < 40)//公里数及体积关系判定
            {
                { AutoCheck = false; if (d_gongli > 60) NotCheckMessage = "客户距离超60公里"; else NotCheckMessage = "体积小于40方"; }
            }
            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '----2----'");
            //客户等级
            if (AutoCheck)
            {
                if ((cusLevel == "A" || cusLevel == "B") && (d_gongli > 50 || dMoneyAll < 2000))
                {
                    { AutoCheck = false; NotCheckMessage = "AB类客户需要小于50公里且金额需要大于2000元"; }
                }
                if (cusLevel == "C")
                {
                    //运费检查
                    if (dMoneyAll < 5000 && i_yunshu > 0)
                    {
                        if (dtSoDetail.Select("cInvName like '%运费%'").Length == 0)
                        {
                            return add_retstr.Replace("@@vcode", "销售订单自动审核").Replace("@@msg", "ERP接口：C客户金额小于5000元需要运费").Replace("@@result", "0");
                        }
                    }
                    //UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '----3----'");
                    ////自动审核检查
                    //if (!(
                    //    (d_gongli >= 10 && d_gongli <= 50 && dMoneyAll >= 2000) ||  //距离 10-50 金额超2000
                    //    (d_gongli < 10 && dMoneyAll >= 1000)  //距离 小于10 金额超1000
                    //    ))
                    //{
                    //    { AutoCheck = false; NotCheckMessage = "C类客户10公里内金额需要超1000元，或者10-50公里且金额需要大于2000元"; }
                    //}
                }
                if (cusLevel == "D" && dMoneyAll < 800)
                {
                    { AutoCheck = false; NotCheckMessage = "D类客户（马家岩、交易城）金额需要大于800元"; }
                }
            }

            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '里程与金额控制自动审核'");
            if (AutoCheck)
            {
                if (
                    !((d_gongli <= 10 && dMoneyAll >= 500) ||
                    (d_gongli > 10 && d_gongli <= 20 && dMoneyAll >= 1000) ||
                    (d_gongli > 20 && d_gongli <= 50 && dMoneyAll >= 1800) ||
                    (d_gongli > 50 && d_gongli <= 80 && dMoneyAll >= 5000) ||
                    (d_gongli > 80 && d_gongli <= 300 && dMoneyAll >= 11000))
                    )
                {
                    AutoCheck = false; NotCheckMessage = "公里数和订单总额达不到自动审核条件";
                }
            }

            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '----7----'");
            //泡泡袋
            DataRow[] Papa = dtSoDetail.Select("cInvCCode='0305'");
            if (AutoCheck && Papa.Length > 0)
            {
                foreach (DataRow data in Papa)
                {
                    if (Convert.ToDecimal(data["iWeight"]) < 50)
                    {
                        AutoCheck = false; NotCheckMessage = "有订单行未达到泡泡袋起订量50公斤";
                        break;
                    }
                }
            }
            UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmmd, "select '----8----'");
            //自动审核
            if (AutoCheck)
            {
                Cmmd.CommandText = @"update " + dbname + @"..SO_SOMain set cVerifier=cMaker,iStatus=1,iswfcontrolled=0,
                    dverifydate=convert(varchar(10),getdate(),120),dverifysystime=getdate() where id=" + soid;
                Cmmd.ExecuteNonQuery();
            }
            else
            {
                Cmmd.CommandText = @"update " + dbname + @"..SO_SOMain set cDefine14='" + NotCheckMessage + "' where id=" + soid;
                Cmmd.ExecuteNonQuery();
            }

            return add_retstr.Replace("@@vcode", "销售订单审核").Replace("@@msg", "成功").Replace("@@result", "1");
        }
        catch (Exception ex)
        {
            return add_retstr.Replace("@@vcode", "销售订单自动审核").Replace("@@msg", "ERP接口：" + ex.Message).Replace("@@result", "0");
        }finally
        {
            U8Operation.CloseDataConnection(sqlCon);
        }

        
    }



}

