using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Data;


/// <summary>
/// U8StandSCMDelete 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class U8StandSCMDelete : System.Web.Services.WebService
{

    public U8StandSCMDelete()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool Del_SCM_Method(string PK_Value, string dbname, string cUserName, string cLogDate, string InterFaceID)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) { if (st_value != U8Operation.GetDataString2(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误"); }
        #endregion

        string SheetID = "";
        if (InterFaceID.Length > 2)
        {
            SheetID = "U810" + InterFaceID;
        }
        else
        {
            SheetID = "" + InterFaceID;
        }

        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            bool str_result = false;
            //采购入库单 返回 ID,Code
            if (SheetID.CompareTo("U81014") == 0) str_result = U81014(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //产品入库 生产订单入库（含已经检验的生产订单）  直接成品入库
            if (SheetID.CompareTo("U81015") == 0) str_result = U81015(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //材料出库 含生产订单领料
            if (SheetID.CompareTo("U81016") == 0) str_result = U81016(PK_Value, dbname, cUserName, cLogDate, Cmd);

            //调拨单  含销售寄售调拨、直接调拨
            if (SheetID.CompareTo("U81017") == 0) str_result = U81017(PK_Value, dbname, cUserName, cLogDate, Cmd);
            ////生产订单调拨   委外订单调拨
            //if (SheetID.CompareTo("U81035") == 0) str_result = U81035(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //其他出库单  含调拨出库
            if (SheetID.CompareTo("U81018") == 0) str_result = U81018(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //其他入库单  含调拨入库
            if (SheetID.CompareTo("U81019") == 0) str_result = U81019(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //销售订单发货 直接发货
            if (SheetID.CompareTo("U81020") == 0) str_result = U81020(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //发货单出库  直接销售出库
            if (SheetID.CompareTo("U81021") == 0) str_result = U81021(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //到货单  采购订单到货  委外订单到货   ASN单到货
            if (SheetID.CompareTo("U81027") == 0) str_result = U81027(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //盘点单
            if (SheetID.CompareTo("U81038") == 0) str_result = U81038(PK_Value, dbname, cUserName, cLogDate, Cmd);

            //货位调整单
            if (SheetID.CompareTo("U81087") == 0) str_result = U81087(PK_Value, dbname, cUserName, cLogDate, Cmd);

            //形态转换
            if (SheetID.CompareTo("U81088") == 0) str_result = U81088(PK_Value, dbname, cUserName, cLogDate, Cmd);

            //销售订单
            if (SheetID.CompareTo("U81089") == 0) str_result = U81089(PK_Value, dbname, cUserName, cLogDate, Cmd);
            //采购订单
            if (SheetID.CompareTo("U81090") == 0) str_result = U81090(PK_Value, dbname, cUserName, cLogDate, Cmd);

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

    //采购入库单
    private bool U81014(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord01 where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords01 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PurBillVouchs 
                where UpSoType='rd' and RdsId in(select autoid from " + dbname + @"..rdrecords01 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经开票"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PurSettleVouchs 
                where cUpSoType='01' and iRdsID in(select autoid from " + dbname + @"..rdrecords01 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经结算"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..OM_MatSettleVouchs 
                where cvouchtype='01' and irdsid in(select autoid from " + dbname + @"..rdrecords01 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经委外核销"); }
        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords01", ref cmsg);
        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iPOsID,a.iOMoDID,a.iArrsId,a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords01 a inner join " + dbname + @"..rdrecord01 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["iPOsID"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set freceivedqty=isnull(freceivedqty,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        freceivednum=isnull(freceivednum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where ID =0" + dtVouchs.Rows[i]["iPOsID"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["iOMoDID"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..OM_MODetails set freceivedqty=isnull(freceivedqty,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        freceivednum=isnull(freceivednum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where MODetailsID =0" + dtVouchs.Rows[i]["iOMoDID"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["iArrsId"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set fValidInQuan=isnull(fValidInQuan,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        fValidInNum=isnull(fValidInNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where autoid =0" + dtVouchs.Rows[i]["iArrsId"];
                Cmd.ExecuteNonQuery();
            }

            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }

        //倒冲材料出库单
        string M_Code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select ccode from " + dbname + @"..rdrecord01 where id=" + pk_value);
        DataTable dtRd11 = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select id from " + dbname + @"..rdrecord11 
                where cBusCode='" + M_Code + "' and cSource = '采购入库单'");
        for (int i = 0; i < dtRd11.Rows.Count; i++)
        {
            U81016("" + dtRd11.Rows[i]["id"], dbname, cUserName, cLogDate, Cmd);
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();
        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords01 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord01 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //产品入库
    private bool U81015(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord10 
                where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords10 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords10", ref cmsg);
        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iMPoIds,a.iCheckIds,a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords10 a inner join " + dbname + @"..rdrecord10 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["iMPoIds"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)-(" + dtVouchs.Rows[i]["iquantity"] + @")
                        where ID =0" + dtVouchs.Rows[i]["iMPoIds"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["iCheckIds"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set FsumQuantity=isnull(FsumQuantity,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        FsumNum=isnull(FsumNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where ID =0" + dtVouchs.Rows[i]["iCheckIds"];
                Cmd.ExecuteNonQuery();
            }

            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }
        //倒冲材料出库单
        string M_Code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select ccode from " + dbname + @"..rdrecord10 where id=" + pk_value);
        DataTable dtRd11 = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select id from " + dbname + @"..rdrecord11 
                where cBusCode='" + M_Code + "' and cSource = '产成品入库单'");
        for (int i = 0; i < dtRd11.Rows.Count; i++)
        {
            U81016("" + dtRd11.Rows[i]["id"], dbname, cUserName, cLogDate, Cmd);
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();
        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords10 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord10 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //材料出库
    private bool U81016(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord11 where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords11 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PU_T_VMIUsedVouchs 
                where UpSoType='11' and iUpVouchsID in(select autoid from " + dbname + @"..rdrecords11 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经生成采购挂账确认单"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..OM_MatSettleVouchs 
                where cvouchtype='11' and irdsid in(select autoid from " + dbname + @"..rdrecords11 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经委外核销"); }
        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords11", ref cmsg);
        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select b.cbustype,a.iMPoIds,a.iMaIDs,a.applydid,a.iOMoMID,a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords11 a inner join " + dbname + @"..rdrecord11 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["cbustype"] + "" == "生产倒冲" || dtVouchs.Rows[i]["cbustype"] + "" == "领料")
            {
                Cmd.CommandText = "update " + dbname + "..mom_moallocate set issqty=round(isnull(issqty,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),6) 
                        where AllocateId =0" + dtVouchs.Rows[i]["iMPoIds"];
                Cmd.ExecuteNonQuery();
            }

            if (dtVouchs.Rows[i]["cbustype"] + "" == "委外倒冲" || dtVouchs.Rows[i]["cbustype"] + "" == "委外发料")
            {
                Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set iSendQTY=round(isnull(iSendQTY,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),6) 
                        where MOMaterialsID =0" + dtVouchs.Rows[i]["iOMoMID"];
                Cmd.ExecuteNonQuery();
            }

            if (dtVouchs.Rows[i]["iMaIDs"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..MaterialAppVouchs set fOutQuantity=round(isnull(fOutQuantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),6) 
                        where autoid =0" + dtVouchs.Rows[i]["iMaIDs"];
                Cmd.ExecuteNonQuery();
                //回写到生产订单用量表 申请已领量    fsendapplyqty
                if (dtVouchs.Rows[i]["cbustype"] + "" == "生产倒冲" || dtVouchs.Rows[i]["cbustype"] + "" == "领料")
                {
                    Cmd.CommandText = "update " + dbname + "..mom_moallocate set RequisitionIssQty=round(isnull(RequisitionIssQty,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),6) 
                            where AllocateId =0" + dtVouchs.Rows[i]["iMPoIds"];
                    Cmd.ExecuteNonQuery();
                }
                else if (dtVouchs.Rows[i]["cbustype"] + "" == "委外倒冲" || dtVouchs.Rows[i]["cbustype"] + "" == "委外发料")
                {
                    Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set fsendapplyqty=round(isnull(fsendapplyqty,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),6) 
                            where MOMaterialsID =0" + dtVouchs.Rows[i]["iOMoMID"];
                    Cmd.ExecuteNonQuery();
                }
            }

            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)+(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();
        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords11 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord11 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //调拨单
    private bool U81017(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),dtvdate,120) from " + dbname + @"..TransVouch where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        string M_Code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select ctvcode from " + dbname + @"..TransVouch where id=" + pk_value);
        string pk_rd09 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord09 
                where cbuscode='" + M_Code + "' and cbustype='调拨出库' and csource='调拨'");
        if (pk_rd09 != "") U81018(pk_rd09, dbname, cUserName, cLogDate, Cmd);
        string pk_rd08 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord08 
                where cbuscode='" + M_Code + "' and cbustype='调拨入库' and csource='调拨'");
        if (pk_rd08 != "") U81019(pk_rd08, dbname, cUserName, cLogDate, Cmd);

        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.itrids,a.imoids,a.iomids,a.itvquantity,a.itvnum,a.cinvcode,
                    b.cowhCode,b.ciwhCode,a.ctvbatch,a.cvmivencode,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10
                from " + dbname + @"..TransVouchs a inner join " + dbname + @"..TransVouch b on a.id=b.id
                where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            //现存量
            Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)-(0" + dtVouchs.Rows[i]["itvquantity"] + @"),
                        fTransOutNum=isnull(fTransOutNum,0)-(0" + dtVouchs.Rows[i]["itvnum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cowhCode"] + "' and cinvcode=" + dtVouchs.Rows[i]["cinvcode"] + @" 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"' 
                        and cbatch=isnull(" + dtVouchs.Rows[i]["ctvbatch"] + ",'') and isnull(cVMIVenCode,'')='" + dtVouchs.Rows[i]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)-(0" + dtVouchs.Rows[i]["itvquantity"] + @"),
                        fTransInNum=isnull(fTransInNum,0)-(0" + dtVouchs.Rows[i]["itvnum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["ciwhCode"] + "' and cinvcode=" + dtVouchs.Rows[i]["cinvcode"] + @" 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"' 
                        and cbatch=isnull(" + dtVouchs.Rows[i]["ctvbatch"] + ",'') and isnull(cVMIVenCode,'')='" + dtVouchs.Rows[i]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
            Cmd.ExecuteNonQuery();

            //回写上游
            if (dtVouchs.Rows[i]["imoids"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..mom_moallocate set TransQty=isnull(TransQty,0)-(" + dtVouchs.Rows[i]["iquantity"] + @")
                        where Allocateid =0" + dtVouchs.Rows[i]["imoids"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["iomids"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set fTransQty=isnull(fTransQty,0)-(" + dtVouchs.Rows[i]["iquantity"] + @")
                        where MOMaterialsID =0" + dtVouchs.Rows[i]["iomids"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["itrids"] + "" != "")  //调拨申请
            {
                Cmd.CommandText = "update " + dbname + "..ST_AppTransVouchs set iTvSumQuantity=isnull(iTvSumQuantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                            iTVSumNum=isnull(iTVSumNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @")
                        where autoid=" + dtVouchs.Rows[i]["itrids"];
                Cmd.ExecuteNonQuery();
            }
        }

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..TransVouchs where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..TransVouch where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //其他出库单
    private bool U81018(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord09 where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords09 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PU_T_VMIUsedVouchs 
                where UpSoType='09' and iUpVouchsID in(select autoid from " + dbname + @"..rdrecords09 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经生成采购挂账确认单"); }

        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords09", ref cmsg);

        //回写单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords09 a inner join " + dbname + @"..rdrecord09 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)+(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords09 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord09 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //其他入库单
    private bool U81019(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord08 where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords08 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PU_T_VMIUsedVouchs 
                where UpSoType='08' and iUpVouchsID in(select autoid from " + dbname + @"..rdrecords08 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经生成采购挂账确认单"); }
        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords08", ref cmsg);

        //回写单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords08 a inner join " + dbname + @"..rdrecord08 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords08 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord08 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //发货
    private bool U81020(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..DispatchList where dlid=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_SA as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..DispatchLists 
                where dlid=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..SaleBillVouchs 
                where iDLsID in(select idlsid from " + dbname + @"..DispatchLists where dlid=" + pk_value + ")") > 0)
        { throw new Exception("单据已经开票"); }

        //获得自动销售出库参数
        bool b_SaCreateRd32 = false;
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select CAST(CAST(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cName='bSAcreat'") == 1)
        {
            b_SaCreateRd32 = true;
        }
        else
        {
            if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords32 
                    where iDLsID  in(select idlsid from " + dbname + @"..DispatchLists where dlid=" + pk_value + ")") > 0)
            { throw new Exception("单据已经出库"); }
        }

        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iSOsID,a.iCorID,a.iquantity,a.inum,a.cinvcode,a.cwhcode,a.cbatch,a.cvmivencode,
                    cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10
                from " + dbname + @"..DispatchLists a
                where a.dlid=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            //修改待发货量
            string currentID = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select isnull(max(autoid),0) from " + dbname + @"..currentstock 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"' 
                        and isnull(cbatch,'')='" + dtVouchs.Rows[i]["cbatch"] + "' and iSoType=0 and iSodid='' and isnull(cVMIVenCode,'')='" + dtVouchs.Rows[i]["cvmivencode"] + "'");
            Cmd.CommandText = "update " + dbname + "..currentstock set fOutQuantity=isnull(fOutQuantity,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        fOutNum=isnull(fOutNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where autoid=" + currentID;
            Cmd.ExecuteNonQuery();

            //回写上游
            if (dtVouchs.Rows[i]["iSOsID"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHQuantity=isnull(iFHQuantity,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        iFHNum=isnull(iFHNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where iSOsID =0" + dtVouchs.Rows[i]["iSOsID"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["iCorID"] + "" != "")
            {
                throw new Exception("本单据为发货退货单据，请从界面中删除");
            }

        }
        //销售出库单删除
        if (b_SaCreateRd32)
        {
            DataTable dtRd32 = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select id from " + dbname + @"..rdrecord32 
                    where cDLCode='" + pk_value + "' and cSource = '发货单'");
            for (int i = 0; i < dtRd32.Rows.Count; i++)
            {
                U81021("" + dtRd32.Rows[i]["id"], dbname, cUserName, cLogDate, Cmd);
            }
        }

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..DispatchLists where dlid=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..DispatchList where dlid=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //销售出库
    private bool U81021(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..rdrecord32 where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords32 
                where id=" + pk_value + " and isnull(cbaccounter,'')<>''") > 0)
        { throw new Exception("单据已经记账"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PU_T_VMIUsedVouchs 
                where UpSoType='32' and iUpVouchsID in(select autoid from " + dbname + @"..rdrecords32 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经生成采购挂账确认单"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..SaleBillVouchs 
                where isaleoutid in(select autoid from " + dbname + @"..rdrecords32 where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经生成销售发票"); }
        //现存量处理
        string cmsg = "";
        KK_U8Com.U8Common.U8V10DeleteCurrentStock(Cmd, dbname + "..", pk_value, "rdrecords32", ref cmsg);

        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iDLsID,a.iquantity,a.inum,b.cwhcode,a.cvmivencode,
                    a.cinvcode,a.cbatch,a.cPosition,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10 
                from " + dbname + @"..rdrecords32 a inner join " + dbname + @"..rdrecord32 b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["iDLsID "] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..DispatchLists set fOutQuantity=isnull(fOutQuantity,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        fOutNum=isnull(fOutNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where iDLsID =0" + dtVouchs.Rows[i]["iDLsID"];
                Cmd.ExecuteNonQuery();
            }

            //货位账务处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)+(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cPosition"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }

        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource=''";
        Cmd.ExecuteNonQuery();

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..rdrecords32 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..rdrecord32 where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //采购订单到货
    private bool U81027(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..PU_ArrivalVouch where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_pu as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..QMINSPECTVOUCHERS a inner join " + dbname + @"..QMINSPECTVOUCHER b on a.id=b.id
                where b.CCHECKTYPECODE='ARR' and a.SOURCEAUTOID in(select autoid from " + dbname + @"..PU_ArrivalVouchs where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经报检"); }

        //自动采购入库单据处理
        bool AutoRD01 = false;
        string cautord01 = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_auto_rd01'", Cmd);
        if (cautord01.CompareTo("true") == 0)
        {
            DataTable dtRd01 = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select distinct a.id 
                    from " + dbname + @"..rdrecords01 a inner join " + dbname + @"..PU_ArrivalVouchs b on a.iArrsId=b.autoid where b.id=" + pk_value);
            if (dtRd01.Rows.Count == 1)
            {
                U81014("" + dtRd01.Rows[0]["id"], dbname, cUserName, cLogDate, Cmd);//删除采购入库单
                AutoRD01 = true;
            }
        }

        if (!AutoRD01)
        {
            if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords01 
                    where iArrsId in(select autoid from " + dbname + @"..PU_ArrivalVouchs where id=" + pk_value + ")") > 0)
            { throw new Exception("单据已经入库"); }
        }

        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select b.cbustype,a.iPOsID,iquantity,inum,a.iCorId 
                from " + dbname + @"..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.id=b.id where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["cbustype"] + "" == "普通采购")
            {
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                            iArrNum=isnull(iArrNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @")
                        where ID =0" + dtVouchs.Rows[i]["iPOsID"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where ID =0" + dtVouchs.Rows[i]["iPOsID"];
                Cmd.ExecuteNonQuery();
            }
            if (dtVouchs.Rows[i]["cbustype"] + "" == "委外加工")
            {
                Cmd.CommandText = "update " + dbname + @"..OM_MODetails set iArrQTY=isnull(iArrQTY,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @") 
                        where MODetailsID=0" + dtVouchs.Rows[i]["iPOsID"];
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + @"..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)),iproducttype=0 
                        where Autoid=0" + dtVouchs.Rows[i]["iPOsID"];
                Cmd.ExecuteNonQuery();
            }

            if (dtVouchs.Rows[i]["iCorId"] + "" != "") throw new Exception("本单据到货退货单，请在界面中删除");
        }

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouchs where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..PU_ArrivalVouch where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //盘点单
    private bool U81038(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),dcvdate,120) from " + dbname + @"..CheckVouch where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        string M_Code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select ccvcode from " + dbname + @"..CheckVouch where id=" + pk_value);
        string pk_rd09 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord09 
                where cbuscode='" + M_Code + "' and cbustype='盘亏出库'");
        if (pk_rd09 != "") U81018(pk_rd09, dbname, cUserName, cLogDate, Cmd);
        string pk_rd08 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord08 
                where cbuscode='" + M_Code + "' and cbustype='盘盈入库'");
        if (pk_rd08 != "") U81019(pk_rd08, dbname, cUserName, cLogDate, Cmd);

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..CheckVouchs where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..CheckVouch where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //货位调整单
    private bool U81087(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..AdjustPVouch where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        //货位账处理
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select a.iquantity,a.inum,a.cinvcode,cBPosCode,cAPosCode,
                    b.cwhcode,a.cbatch,a.cvmivencode,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10
                from " + dbname + @"..AdjustPVouchs a inner join " + dbname + @"..AdjustPVouch b on a.id=b.id
                where a.id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            //货位账处理
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)+(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cBPosCode"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(0" + dtVouchs.Rows[i]["iquantity"] + @"),
                        inum=isnull(inum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                    where cwhcode='" + dtVouchs.Rows[i]["cwhcode"] + "' and cvmivencode='" + dtVouchs.Rows[i]["cvmivencode"] + @"' 
                        and cinvcode='" + dtVouchs.Rows[i]["cinvcode"] + "' and cPosCode='" + dtVouchs.Rows[i]["cAPosCode"] + @"' 
                        and cbatch='" + dtVouchs.Rows[i]["cbatch"] + @"' 
                        and isnull(cfree1,'')='" + dtVouchs.Rows[i]["cfree1"] + "' and isnull(cfree2,'')='" + dtVouchs.Rows[i]["cfree2"] + @"' 
                        and isnull(cfree3,'')='" + dtVouchs.Rows[i]["cfree3"] + "' and isnull(cfree4,'')='" + dtVouchs.Rows[i]["cfree4"] + @"' 
                        and isnull(cfree5,'')='" + dtVouchs.Rows[i]["cfree5"] + "' and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree6"] + @"' 
                        and isnull(cfree6,'')='" + dtVouchs.Rows[i]["cfree7"] + "' and isnull(cfree8,'')='" + dtVouchs.Rows[i]["cfree8"] + @"' 
                        and isnull(cfree7,'')='" + dtVouchs.Rows[i]["cfree9"] + "' and isnull(cfree10,'')='" + dtVouchs.Rows[i]["cfree10"] + @"'";
            Cmd.ExecuteNonQuery();
        }
        //删除货位记录
        Cmd.CommandText = "delete from " + dbname + @"..InvPosition where rdid=" + pk_value + " and csource='货位调整'";
        Cmd.ExecuteNonQuery();

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..AdjustPVouchs where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..AdjustPVouch where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //形态转换
    private bool U81088(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),davdate,120) from " + dbname + @"..AssemVouch where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_ST as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        string M_Code = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select cavcode from " + dbname + @"..AssemVouch where id=" + pk_value);
        string pk_rd09 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord09 
                where cbuscode='" + M_Code + "' and cbustype='转换出库'");
        if (pk_rd09 != "") U81018(pk_rd09, dbname, cUserName, cLogDate, Cmd);
        string pk_rd08 = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select id from " + dbname + @"..rdrecord08 
                where cbuscode='" + M_Code + "' and cbustype='转换入库'");
        if (pk_rd08 != "") U81019(pk_rd08, dbname, cUserName, cLogDate, Cmd);

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..AssemVouchs where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..AssemVouch where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //销售订单
    private bool U81089(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),ddate,120) from " + dbname + @"..SO_SOMain where id=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_SA as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..DispatchLists 
                where iSOsID in(select iSOsID from " + dbname + @"..SO_SODetails where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经发货"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..SaleBillVouchs 
                where iSOsID in(select iSOsID from " + dbname + @"..SO_SODetails where id=" + pk_value + ")") > 0)
        { throw new Exception("单据已经开票"); }

        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iaoids,iquantity,inum from " + dbname + @"..SO_SODetails where id=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["iaoids"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..SA_PreOrderDetails set fdhquantity=isnull(fdhquantity,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        fdhnum=isnull(fdhnum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where autoid =0" + dtVouchs.Rows[i]["iaoids"];
                Cmd.ExecuteNonQuery();
            }
        }

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..SO_SODetails where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..SO_SOMain where id=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    //采购订单
    private bool U81090(string pk_value, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //判断是否结账
        string cdate = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select convert(varchar(10),dpodate,120) from " + dbname + @"..PO_Pomain where poid=" + pk_value);
        string cacc_id = dbname.Substring(7, 3);
        DataTable dtMonth = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iYear,iId from ufsystem..UA_Period 
                where cAcc_Id='" + cacc_id + "' and dBegin='" + cdate + "' and dEnd>='" + cdate + "'");
        if (dtMonth.Rows.Count == 0) throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]未找到有效期间");
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, @"select isnull(max(cast(bflag_pu as int)),0) from " + dbname + @"..gl_mend 
                where iyear=" + dtMonth.Rows[0]["iYear"] + " and iperiod=" + dtMonth.Rows[0]["iId"]) > 0)
            throw new Exception("账套[" + cacc_id + "] 单据日期[" + cdate + "]已经结账");

        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..rdrecords01 
                where iPOsID in(select id from " + dbname + @"..PO_Podetails where poid=" + pk_value + ")") > 0)
        { throw new Exception("单据已经入库"); }
        if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.id=b.id
                where b.cbustype<>'委外加工' and iPOsID in(select id from " + dbname + @"..PO_Podetails where poid=" + pk_value + ")") > 0)
        { throw new Exception("单据已经到货"); }

        //回写上游单据
        DataTable dtVouchs = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @"select iAppIds,iquantity,inum from " + dbname + @"..PO_Podetails where poid=" + pk_value);
        for (int i = 0; i < dtVouchs.Rows.Count; i++)
        {
            if (dtVouchs.Rows[i]["iAppIds"] + "" != "")
            {
                Cmd.CommandText = "update " + dbname + "..PU_AppVouchs set iReceivedQTY=isnull(iReceivedQTY,0)-(" + dtVouchs.Rows[i]["iquantity"] + @"),
                        iReceivedNum=isnull(iReceivedNum,0)-(0" + dtVouchs.Rows[i]["inum"] + @") 
                        where autoID =0" + dtVouchs.Rows[i]["iAppIds"];
                Cmd.ExecuteNonQuery();
            }
        }

        //删除单据
        Cmd.CommandText = "delete from " + dbname + @"..PO_Podetails where poid=" + pk_value;
        Cmd.ExecuteNonQuery();
        Cmd.CommandText = "delete from " + dbname + @"..PO_Pomain where poid=" + pk_value;
        Cmd.ExecuteNonQuery();
        return true;
    }

    [WebMethod]
    public bool SO_OrderCLose_line(string PK_line_Value, string acc_id, string cClosedUserName)
    {
        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        try
        {
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + acc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (dbname == "") throw new Exception("账套号错误，日期不在本账套[" + acc_id + "]的有效会计期间内");


            //判断是否结账
            if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(1) from " + dbname + @"..SO_SOMain 
                where id in(select id from " + dbname + @"..SO_SODetails where autoid=" + PK_line_Value + ") and isnull(cVerifier,'')=''") > 0)
            { throw new Exception("订单没有审核，不能关闭"); }


            //关闭
            Cmd.CommandText = "update " + dbname + "..SO_SODetails set cSCloser='" + cClosedUserName + @"',dbclosedate=getdate(),dbclosesystime=getdate()
            where autoid =0" + PK_line_Value;
            Cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

        return true;
    }


}

