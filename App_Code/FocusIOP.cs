using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;


/// <summary>
/// FocusIOP 聚焦人才网U8外部同步接口
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class FocusIOP : System.Web.Services.WebService
{
    public FocusIOP()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]  //客户增加
    public string Syn_Customer(string cCusCode, string cCusName, string cCusAbbName, string cCusClass, string cPhoneNum, string cTaxNo)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from customer where ccuscode='" + cCusCode + "'", Cmd) > 0) 
            { 
                Cmd.CommandText = @"update customer set ccusname='" + cCusName + "',ccusabbname='" + cCusAbbName + "',ccccode='" + cCusClass + "',cCusPhone='" + cPhoneNum + "',cCusDefine4='" + cTaxNo + @"' 
                    where ccuscode='" + cCusCode + "'"; 
            }
            else
            {
                Cmd.CommandText = @"insert into customer(ccuscode,ccusname,ccusabbname,ccccode,cCusPhone,cCusDefine4,dcusdevdate,
                    icusdisrate,icuscreline,icuscredate,ccusheadcode,iarmoney,ilastmoney,icostgrade,cinvoicecompany,ccusexch_name,icusgsptype,bcusdomestic,ccuscreditcompany,dcuscreatedatetime,cCusMngTypeCode)
                    values('" + cCusCode + "','" + cCusName + "','" + cCusAbbName + "','" + cCusClass + "','" + cPhoneNum + "','" + cTaxNo + @"',convert(varchar(10),getdate(),120),
                    0,0,0,'" + cCusCode + "',0,0,-1,'" + cCusCode + "','人民币',0,1,'" + cCusCode + @"',getdate(),'999')";
            }
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            str_result = str_result.Replace("@@result@@", "true").Replace("@@msg@@", "");
            return str_result;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_result = str_result.Replace("@@result@@", "false").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //客户查询
    public string Syn_CustomerQuery(string cCusName)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        SqlConnection Conn = null;
        try
        {
            Conn = U8Operation.OpenDataConnection();
            SqlCommand Cmd = Conn.CreateCommand();
            if (U8Operation.GetDataInt("select count(*) from customer where ccusname='" + cCusName + "'", Cmd) > 0)
            {
                string ccusccode = U8Operation.GetDataString("select ccuscode from customer where ccusname='" + cCusName + "'", Cmd);
                str_result = str_result.Replace("@@result@@", "1").Replace("@@msg@@", ccusccode);
            }
            else
            {
                str_result = str_result.Replace("@@result@@", "0").Replace("@@msg@@", "未找到");
            }
            return str_result;
        }
        catch (Exception ex)
        {
            str_result = str_result.Replace("@@result@@", "-1").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            if(Conn!=null) U8Operation.CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //收款单增加
    public string Syn_GatherSheetAdd(string cSheetNo, string cCusCode, string cPayCode, float iPayMoney, string c_payee,
        string cPersonCode, string cDepCode, string cMaker, string dMakeDate, string pay_date)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        string cerrmsg = "";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from Ap_CloseBill where cdwcode='X" + cSheetNo + "' and cFlag='AR'", Cmd) > 0) { throw new Exception("单据已经传送，不能重复处理"); }
            if (U8Operation.GetDataInt("select count(*) from customer where ccuscode='" + cCusCode + "'", Cmd) == 0) { throw new Exception("客户编码不存在"); }
            if (U8Operation.GetDataInt("select count(*) from department where cdepcode='" + cDepCode + "'", Cmd) == 0) { throw new Exception("部门编码不存在"); }
            if (U8Operation.GetDataInt("select count(*) from SettleStyle where cSSCode='" + cPayCode + "'", Cmd) == 0) { throw new Exception("结算方式不存在"); }

            if (cPersonCode.CompareTo("") != 0)
            {
                if (U8Operation.GetDataInt("select count(*) from Person where cpersoncode='" + cPersonCode + "'", Cmd) == 0) { throw new Exception("人员编码不存在，或不是业务员"); }
            }

            float fmoney = iPayMoney;
            if (fmoney == 0) { throw new Exception("收款金额不能为0"); }
            string dbname = U8Operation.GetDataString("select db_name()", Cmd);
            string targetAccId = U8Operation.GetDataString("select substring(db_name(),8,3)", Cmd);
            KK_U8Com.U8ApCloseBill main = new KK_U8Com.U8ApCloseBill(Cmd, dbname);
            KK_U8Com.U8ApCloseBills detail = new KK_U8Com.U8ApCloseBills(Cmd, dbname);
            #region //主表
            main.iID = 1000000000 + int.Parse(U8Operation.GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='SK'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='SK'";
            Cmd.ExecuteNonQuery();
            main.cVouchID = "'X" + cSheetNo + "'";
            main.dVouchDate = "'" + dMakeDate + "'";
            main.cDwCode = "'" + cCusCode + "'";
            main.cDeptCode = "'" + cDepCode + "'";
            if (cPersonCode.CompareTo("") != 0) main.cPerson = "'" + cPersonCode + "'";
            main.cDigest = "''";
            main.cOperator = "'" + cMaker + "'";
            main.cSSCode = "'" + cPayCode + "'";
            main.iAmount = "" + iPayMoney;
            main.iAmount_s = 0;
            main.cexch_name = "'人民币'";
            main.iExchRate = 1;
            main.iPeriod = int.Parse("" + U8Operation.GetDataString("select iid from ufsystem..UA_Period where cAcc_Id='" + targetAccId + "' and dbegin<='" + dMakeDate + "' and dend>='" + dMakeDate + "'", Cmd));
            main.cFlag = "'AR'";
            main.cDefine10 = "'" + c_payee + "'";
            main.cDefine11 = "'" + pay_date + "'";  //收款时间
            if (fmoney >= 0)
            {
                main.cVouchType = "'48'";
                main.VT_ID = 8052;
            }
            else
            {
                main.cVouchType = "'49'";
                main.VT_ID = 8053;
            }
            if (!main.InsertToDB(targetAccId, ref cerrmsg)) { throw new Exception(cerrmsg); }
            #endregion

            #region  //子表
            detail.ID = 1000000000 + int.Parse(U8Operation.GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='SK'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='SK'";
            Cmd.ExecuteNonQuery();
            detail.iID = main.iID;
            detail.cCusVen = "'" + cCusCode + "'";
            detail.iAmt = "" + iPayMoney;
            detail.iAmt_s = "0";
            detail.cMemo = "''";

            if (!detail.InsertToDB(targetAccId, ref cerrmsg)) { throw new Exception(cerrmsg); }

            #endregion

            Cmd.Transaction.Commit();
            str_result = str_result.Replace("@@result@@", "true").Replace("@@msg@@", "");
            return str_result;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_result = str_result.Replace("@@result@@", "false").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //收款单删除
    public string Syn_GatherSheetDel(string cSheetNo)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from Ap_CloseBill where cVouchID ='X" + cSheetNo + "' and cFlag='AR'", Cmd) == 0) { throw new Exception("单据不存在，无法处理"); }
            if (U8Operation.GetDataInt("select count(*) from Ap_CloseBill where cVouchID ='X" + cSheetNo + "' and cFlag='AR' and isnull(cCheckMan,'')<>''", Cmd) > 0) { throw new Exception("单据已经财务审核，无法处理"); }
            Cmd.CommandText = "delete from Ap_CloseBills where iID in(select iID from Ap_CloseBill where cVouchID ='X" + cSheetNo + "' and cFlag='AR')";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "delete from Ap_CloseBill where cVouchID ='X" + cSheetNo + "' and cFlag='AR'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            str_result = str_result.Replace("@@result@@", "true").Replace("@@msg@@", "");
            return str_result;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_result = str_result.Replace("@@result@@", "false").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
    }


    [WebMethod]  //销售发票增加
    public string Syn_BillSheetAdd(string cBillNo, string c_bill_type, string cCusCode, string iTaxMoney, string invoice_title, string iTaxRate, string cGoodCode, string cPersonCode,
        string cDepCode,string cMaker,string dMakeDate)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        string cerrmsg = "";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from SaleBillVouch where cSBVCode='X" + cBillNo + "'", Cmd) > 0) { throw new Exception("单据已经传送，不能重复处理"); }
            if (U8Operation.GetDataInt("select count(*) from customer where ccuscode='" + cCusCode + "'", Cmd) == 0) { throw new Exception("客户编码不存在"); }
            if (U8Operation.GetDataInt("select count(*) from department where cdepcode='" + cDepCode + "'", Cmd) == 0) { throw new Exception("部门编码不存在"); }
            if (cPersonCode.CompareTo("") != 0)
            {
                if (U8Operation.GetDataInt("select count(*) from Person where cpersoncode='" + cPersonCode + "'", Cmd) == 0) { throw new Exception("人员编码不存在，或不是业务员"); }
            }
            float fmoney = float.Parse(iTaxMoney);
            if (fmoney == 0) { throw new Exception("发票金额不能为0"); }
            string DBName = U8Operation.GetDataString("select db_name()", Cmd);
            string accid = U8Operation.GetDataString("select substring(db_name(),8,3)", Cmd);

            KK_U8Com.U8SaleBillVouch samain = new KK_U8Com.U8SaleBillVouch(Cmd, DBName);
            //销售发票主表
            #region
            samain.SBVID = 1000000000 + int.Parse(U8Operation.GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + accid + "' and cVouchType='BILLVOUCH'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + accid + "' and cVouchType='BILLVOUCH'";
            Cmd.ExecuteNonQuery();
            samain.cSBVCode = "'X" + cBillNo + "'";
            samain.dDate = "'" + dMakeDate + "'";
            if (c_bill_type.CompareTo("专票") == 0)
            {
                samain.cVouchType = "'26'"; //专票
                samain.iVTid = 53;
            }
            else
            {
                samain.cVouchType = "'27'";//普票
                samain.iVTid = 16;
            }
            
            samain.cCusCode = "'" + cCusCode + "'";
            samain.cDepCode = "'" + cDepCode + "'";
            if (cPersonCode.CompareTo("") != 0) samain.cPersonCode = "'" + cPersonCode + "'";

            samain.cSTCode = "'01'";
            samain.cMaker = "'" + cMaker + "'";
            //samain.cChecker = "'" + dtRd01Main.Rows[0]["cmaker"] + "'";
            //samain.dverifydate = "'" + dtRd01Main.Rows[0]["cdate"] + "'";
            samain.cexch_name = "'人民币'";
            samain.iExchRate = 1;
            samain.cBusType = "null";
            samain.iDisp = 0;
            samain.cSource = "'应收'";
            samain.bReturnFlag = (fmoney < 0 ? 1 : 0);
            samain.iTaxRate = float.Parse(iTaxRate);
            samain.cDefine10 = "'" + invoice_title + "'";
            //复核人
            samain.cChecker = "'" + cMaker + "'";

            if (!samain.InsertToDB(accid, ref cerrmsg)) { throw new Exception(cerrmsg); }
            #endregion


            KK_U8Com.U8SaleBillVouchs sadetail = new KK_U8Com.U8SaleBillVouchs(Cmd, DBName);
            //销售发票子表
            #region
            sadetail.AutoID = int.Parse(U8Operation.GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + accid + "' and cVouchType='BILLVOUCH'", Cmd));
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + accid + "' and cVouchType='BILLVOUCH'";
            Cmd.ExecuteNonQuery();
            sadetail.SBVID = samain.SBVID;

            sadetail.iQuantity = "1";
            sadetail.iNum = "null";
            sadetail.iTB = 0;  //退补
            sadetail.TBQuantity = "null";
            sadetail.TBNum = "null";

            sadetail.cInvCode = "'" + cGoodCode + "'";
            sadetail.iInvExchRate = "null";
            sadetail.iSum = iTaxMoney;
            //sadetail.iTax = iTax;
            sadetail.iTaxRate = float.Parse("" + iTaxRate);
            sadetail.irowno = 1;
            sadetail.iSBVID = "0";
            if (!sadetail.InsertToDB(ref cerrmsg)) { throw new Exception(cerrmsg); }
            Cmd.CommandText = "update " + DBName + "..SaleBillVouchs set iQuantity=round(iQuantity,5),TBQuantity=round(TBQuantity,5) where AutoID=" + sadetail.AutoID;
            Cmd.ExecuteNonQuery();

            ////更新主表是否为 退货单 
            //Cmd.CommandText = "update " + DBName + ".dbo.SaleBillVouch set bReturnFlag=1 where SBVID in(select sbvid from SaleBillVouchs where sbvid=" + samain.SBVID + " and iQuantity<0)";
            //Cmd.ExecuteNonQuery();
            #endregion


            Cmd.Transaction.Commit();
            str_result = str_result.Replace("@@result@@", "true").Replace("@@msg@@", "");
            return str_result;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_result = str_result.Replace("@@result@@", "false").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
    }

    [WebMethod]  //销售发票删除
    public string Syn_BillSheetDel(string cBillNo)
    {
        string str_result = "[{\"result\":\"@@result@@\",\"msg\":\"@@msg@@\"}]";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from SaleBillVouch where cSBVCode ='X" + cBillNo + "'", Cmd) == 0) { throw new Exception("单据不存在，无法处理"); }
            if (U8Operation.GetDataInt("select count(*) from SaleBillVouch where cSBVCode ='X" + cBillNo + "' and (isnull(cVerifier,'')<>'' or isnull(cChecker,'')<>'')", Cmd) > 0) { throw new Exception("单据已经财务审核，无法处理"); }
            Cmd.CommandText = "delete from SaleBillVouchs where SBVID in(select SBVID from SaleBillVouch where cSBVCode ='X" + cBillNo + "')";
            Cmd.ExecuteNonQuery();
            Cmd.CommandText = "delete from SaleBillVouch where cSBVCode ='X" + cBillNo + "'";
            Cmd.ExecuteNonQuery();


            Cmd.Transaction.Commit();
            str_result = str_result.Replace("@@result@@", "true").Replace("@@msg@@", "");
            return str_result;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_result = str_result.Replace("@@result@@", "false").Replace("@@msg@@", ex.Message);
            return str_result;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

    }

    [WebMethod(Description = @"判定计算机 ")]  //U8 授权
    public string  ComputerKey()
    {
        return "OK";
    }

}

