using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// VendorIO 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class VendorLMDIO : System.Web.Services.WebService
{

    public VendorLMDIO()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 

    }

    public static System.Data.DataTable JsonToDataTable(string json)
    {
        System.Data.DataTable dataTable = new System.Data.DataTable();  //实例化
        System.Data.DataTable result;
        try
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
            ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
            if (arrayList.Count > 0)
            {
                foreach (Dictionary<string, object> dictionary in arrayList)
                {
                    if (dictionary.Keys.Count == 0)
                    {
                        result = dataTable;
                        return result;
                    }
                    //Columns
                    if (dataTable.Columns.Count == 0)
                    {
                        foreach (string current in dictionary.Keys)
                        {
                            dataTable.Columns.Add(current, dictionary[current].GetType());
                        }
                    }
                    //Rows
                    System.Data.DataRow dataRow = dataTable.NewRow();
                    foreach (string current in dictionary.Keys)
                    {
                        dataRow[current] = dictionary[current];
                    }
                    dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                }
            }
        }
        catch
        {
        }
        result = dataTable;
        return result;
    }
    public static void WriteDebug(string bugStr, string type)
    {
        FileStream fs = null;
        StreamWriter sw = null;
        try
        {
            string path = @"C:\XTWebLog\" + type + ".txt";
            bool isexists = File.Exists(path);
            if (!Directory.Exists(@"C:\XTWebLog"))
            {
                Directory.CreateDirectory(@"C:\XTWebLog");
            }
            if (File.Exists(path))
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = File.Create(path);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + bugStr);
            sw.Flush();
        }
        catch (Exception e)
        {
            throw new Exception("日志文件写入异常！" + e.ToString());
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
    }

    [WebMethod]  //加工商接收ASN单
    public string Pu_ASN_ByOtherVendor(string jsonHead, string jsonBody, string cacc_id, string cmaker)
    {
        //cacc_id   账套号
        //cmaker    操作员姓名

        //jsonHead  表头项目，栏目如下：
        //asncode    ASN单的单据号
        //asnsource  ASN单来源  可以给固定值：采购订单
        //cvencode  材料供应商编码（ASN单的供应商编码）
        //cmovencode   加工商编码（接收方)

        //jsonBody  表体项目，栏目如下：
        //autoid    ASN单的子表 ID
        //psosid    采购订单子表ID
        //cvencode  材料供应商编码（ASN单的供应商编码）
        //cinvcode  存货编码
        //iqty      ASN单的 数量
        //r_qty     实收数量
        //cbatch    送货批号
        string str_resulte = "[{\"result\":\"@@resulte@@\",\"msg\":\"@@msg@@\"}]";
        WriteDebug("Pu_ASN_ByOtherVendor：jsonHead【" + jsonHead + "】", "venlog");
        WriteDebug("Pu_ASN_ByOtherVendor：cacc_id【" + cacc_id + "】", "venlog");
        WriteDebug("Pu_ASN_ByOtherVendor：jsonBody【" + jsonBody + "】", "venlog");

        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();

        try
        {
            DataTable dtHead = JsonToDataTable(jsonHead);
            DataTable dtBody = JsonToDataTable(jsonBody);
            if (dtHead.Rows.Count == 0) throw new Exception("表头无任何行");
            if (dtBody.Rows.Count == 0) throw new Exception("表体无任何行");
            string dbname = U8Operation.GetDataString(@" select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + cacc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);

            #region //采购入库单
            DataTable dtRd01Head = new DataTable();
            dtRd01Head.Columns.Add("ASN单号"); dtRd01Head.Columns.Add("采购来源"); dtRd01Head.Columns.Add("供应商");
            dtRd01Head.Columns.Add("制单人"); dtRd01Head.Columns.Add("制单日期"); dtRd01Head.Columns.Add("仓库编码");
            dtRd01Head.Rows.Add(new object[] { "", "", "", "", "", "" });
            dtRd01Head.Rows[0]["制单人"] = cmaker;
            dtRd01Head.Rows[0]["制单日期"] = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)",Cmd);
            dtRd01Head.Rows[0]["ASN单号"] = dtHead.Rows[0]["asncode"] + "";
            dtRd01Head.Rows[0]["采购来源"] = dtHead.Rows[0]["asnsource"] + "";
            dtRd01Head.Rows[0]["供应商"] = dtHead.Rows[0]["cvencode"] + "";
            //本地虚拟接收仓库
            dtRd01Head.Rows[0]["仓库编码"] = "" + U8Operation.GetDataString("select cvalue from " + dbname + "..T_Parameter where cpid='u8ven_recieve_ware'", Cmd);
            if (dtRd01Head.Rows[0]["仓库编码"].ToString().CompareTo("") == 0) throw new Exception("U8系统参数【供应商协同平台美心虚拟接收物资仓库】没有维护");

            DataTable dtRd01Body = new DataTable();
            dtRd01Body.Columns.Add("autoid"); dtRd01Body.Columns.Add("psosid"); dtRd01Body.Columns.Add("供应商编码");
            dtRd01Body.Columns.Add("供应商名称"); dtRd01Body.Columns.Add("存货编码"); dtRd01Body.Columns.Add("存货代码");
            dtRd01Body.Columns.Add("存货名称"); dtRd01Body.Columns.Add("数量"); dtRd01Body.Columns.Add("实收数量");
            dtRd01Body.Columns.Add("扣点数量"); dtRd01Body.Columns.Add("扣点原因"); dtRd01Body.Columns.Add("批号");
            dtRd01Body.Columns.Add("规格"); dtRd01Body.Columns.Add("单位"); dtRd01Body.Columns.Add("含税单价");
            dtRd01Body.Columns.Add("税率"); dtRd01Body.Columns.Add("批次管理");
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                DataRow dr = dtRd01Body.Rows.Add(new object[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                dr["autoid"] = "" + dtBody.Rows[i]["autoid"];
                dr["psosid"] = "" + dtBody.Rows[i]["psosid"].ToString().Substring(7);
                dr["供应商编码"] = "" + dtBody.Rows[i]["cvencode"];
                dr["存货编码"] = "" + dtBody.Rows[i]["cinvcode"];
                dr["数量"] = "" + dtBody.Rows[i]["iqty"];
                dr["实收数量"] = "" + dtBody.Rows[i]["r_qty"];
                dr["扣点数量"] = "0";
                dr["批号"] = "" + dtBody.Rows[i]["cbatch"];
                dr["含税单价"] = "" + U8Operation.GetDataString("select iTaxPrice from " + dbname + "..PO_Podetails where ID=" + dr["psosid"], Cmd);
                dr["税率"] = "" + U8Operation.GetDataString("select iPerTaxRate from " + dbname + "..PO_Podetails where ID=" + dr["psosid"], Cmd);
                dr["批次管理"] = "" + U8Operation.GetDataString("select cast(bInvBatch as int) from " + dbname + "..Inventory where cinvcode='" + dr["存货编码"] + "'", Cmd);
                //dtRd01Body.Rows.Add(dr);
            }
            #endregion

            #region//调拨单
            DataTable dtTrHead = new DataTable();
            dtTrHead.Columns.Add("id"); dtTrHead.Columns.Add("仓库编码"); dtTrHead.Columns.Add("调入仓库");
            dtTrHead.Columns.Add("制单人"); dtTrHead.Columns.Add("制单日期"); dtTrHead.Columns.Add("出库类别");
            dtTrHead.Columns.Add("入库类别"); dtTrHead.Columns.Add("备注"); dtTrHead.Columns.Add("调出部门");
            dtTrHead.Columns.Add("调入部门"); dtTrHead.Columns.Add("业务员");
            dtTrHead.Rows.Add(new object[] { "", "", "", "", "", "", "", "", "", "", "" });
            dtTrHead.Rows[0]["制单人"] = cmaker;
            dtTrHead.Rows[0]["制单日期"] = dtRd01Head.Rows[0]["制单日期"];
            dtTrHead.Rows[0]["仓库编码"] = dtRd01Head.Rows[0]["仓库编码"]; //调出仓库
            //委外商虚拟仓库
            dtTrHead.Rows[0]["调入仓库"] = "" + U8Operation.GetDataString("select cVenDefine1 from " + dbname + "..vendor where cvencode='" + dtHead.Rows[0]["cmovencode"] + "'", Cmd);
            if (dtTrHead.Rows[0]["调入仓库"].ToString().CompareTo("") == 0) throw new Exception("U8委外商【供应商协同平台美心虚拟接收物资仓库】没有维护虚拟库房");
            dtTrHead.Rows[0]["入库类别"] = "" + U8Operation.GetDataString("select cvalue from " + dbname + "..T_Parameter where cpid='u8ven_db_out_rdcode'", Cmd);
            dtTrHead.Rows[0]["出库类别"] = "" + U8Operation.GetDataString("select cvalue from " + dbname + "..T_Parameter where cpid='u8ven_db_in_rdcode'", Cmd);
            if (dtTrHead.Rows[0]["入库类别"].ToString().CompareTo("") == 0) throw new Exception("U8委外商【供应商协同平台虚拟调拨入库类别编码】没有维护");
            if (dtTrHead.Rows[0]["出库类别"].ToString().CompareTo("") == 0) throw new Exception("U8委外商【供应商协同平台虚拟调拨出库类别编码】没有维护");

            DataTable dtTrBody = new DataTable();
            dtTrBody.Columns.Add("autoid"); dtTrBody.Columns.Add("存货编码"); dtTrBody.Columns.Add("存货代码");
            dtTrBody.Columns.Add("存货名称"); dtTrBody.Columns.Add("规格"); dtTrBody.Columns.Add("数量");
            dtTrBody.Columns.Add("库存数"); dtTrBody.Columns.Add("批号"); dtTrBody.Columns.Add("区域");
            dtTrBody.Columns.Add("单位"); dtTrBody.Columns.Add("供应商");
            for (int i = 0; i < dtBody.Rows.Count; i++)
            {
                DataRow dr = dtTrBody.Rows.Add(new object[] { "", "", "", "", "", "", "", "", "", "", "" });
                dr["autoid"] = "0";
                dr["存货编码"] = "" + dtBody.Rows[i]["cinvcode"];
                dr["数量"] = "" + dtBody.Rows[i]["r_qty"];
                dr["批号"] = "" + dtBody.Rows[i]["cbatch"];
                dr["供应商"] = dtHead.Rows[0]["cvencode"] + "";
                //dtTrBody.Rows.Add(dr);
            }
            #endregion

            //写入系统
            U8Operation u8 = new U8Operation();
            
            string rdpustr = u8.U8SCM_PuASN_Add(dtRd01Head, dtRd01Body, null, dbname, Cmd,false);
            string trstr = u8.U8SCM_Trans_Input(dtTrHead, dtTrBody, null, dbname, Cmd);

            Cmd.Transaction.Commit();
            str_resulte = str_resulte.Replace("@@resulte@@", "true").Replace("@@msg@@", "");
            WriteDebug("str_result【" + str_resulte + "】", "venlog");
            return str_resulte;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_resulte = str_resulte.Replace("@@resulte@@", "false").Replace("@@msg@@", ex.Message);
            WriteDebug("str_result【" + str_resulte + "】", "venlog");
            return str_resulte;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

    }


    [WebMethod]  //供应商挂账确认：自动填写发票
    public string Pu_Vendor_GZ(string idlist,string cvencode,string cacc_id, string cmaker)
    {
        //yearmonth 年度月份 如：201709
        //cvencode  供应商编码
        //cacc_id   账套号
        //cmaker    操作员
        string str_resulte = "[{\"result\":\"@@resulte@@\",\"msg\":\"@@msg@@\"}]";
        WriteDebug("Pu_Vendor_GZ：yearmonth【" + idlist + "】", "venlog");
        WriteDebug("Pu_Vendor_GZ：cvencode【" + cvencode + "】", "venlog");
        WriteDebug("Pu_Vendor_GZ：cacc_id【" + cacc_id + "】", "venlog");

        //Session.Timeout = 10;
        SqlConnection Conn = U8Operation.OpenDataConnection();
        string errmsg = "";
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.CommandTimeout = 300000;
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string cDBOwer = U8Operation.GetDataString(@"select cdatabase from UFSystem..UA_AccountDatabase where cAcc_Id='" + cacc_id + @"' and isnull(iEndYear,2099)>=YEAR(GETDATE())
	            and iBeginYear<=YEAR(GETDATE())", Cmd);
            if (cacc_id == "") throw new Exception("传递账套参数错误，未找到数据源");
            string targetAccId = cacc_id;

            DataTable dtValid = U8Operation.GetSqlDataTable("SELECT * FROM " + cDBOwer + "..T_Parameter where cPid='ven_puarr_receivetype'", "dtValid", Cmd);
            int i_rows_count = U8Operation.GetDataInt("select cValue from " + cDBOwer + @"..T_Parameter where cPid='ven_pubill_rows'", Cmd); //开票分行行数
            //string c_is_contain_koukuan = U8Operation.GetDataString("select cValue from " + cDBOwer + @"..T_Parameter where cPid='ven_pubill_contain_koukuan'", Cmd).ToLower();
            //if (c_is_contain_koukuan.CompareTo("true") != 0)  //不处理扣款，可以分行
            //{
            //    i_rows_count = U8Operation.GetDataInt("select cValue from " + cDBOwer + @"..T_Parameter where cPid='ven_pubill_rows'", Cmd);
            //}

            //获得制单人
            cmaker = U8Operation.GetDataString(@"select cValue from " + cDBOwer + @"..T_Parameter where cPid='ven_pubill_maker' ", Cmd);
            if (cmaker == "") throw new Exception("请维护参数：【供应商平台 开票制单人名称】");


            U8StandSCMBarCode.CheckVouchValid(cDBOwer, dtValid, "1" + idlist, Cmd);
            //创建临时表
            Cmd.CommandText = "if object_id('tempdb..#tmp_vouch_list') is not null begin drop table #tmp_vouch_list end";
            Cmd.ExecuteNonQuery();

            //额外
            U8StandSCMBarCode.CheckVouchValid(cDBOwer, dtValid, "1" + idlist, Cmd);

            //凭证分单
            Cmd.CommandText = @"select identity(int,1,1) rid,a.cvencode,a.iexchrate,a.cexch_name,substring(a.rdtype,4,20) rdtype,autoid,a.cbustype,
                	case when cc_gz_qty>=0 then 'blue' else 'red' end cColor,isnull(a.cptcode,'') cptcode,1 vouch_idx,b.cc_identity
                into #tmp_vouch_list
                from " + cDBOwer + @"..v_cqcc_gzd_mx_list a inner join " + cDBOwer + @"..T_CC_GZD_Pre b on a.rdtype=b.cc_rdtype and a.autoid=b.cc_autoid and a.cvencode=b.cc_vencode
                    inner join VenDB..cc_fire_pugz m on b.cc_identity=m.id
                where m.tempKey='" + idlist + "' and a.cvencode='" + cvencode + @"' and cc_HasCost='有价' and cc_kp_autoid is null  and a.rdtype not like 'sp_%'";
            Cmd.ExecuteNonQuery();

            DataTable dtmxlist = U8Operation.GetSqlDataTable(@"select cvencode+cexch_name+rdtype+cbustype+cColor+cptcode ccc_contin,rid,vouch_idx
                    from #tmp_vouch_list order by cvencode,cexch_name,rdtype,cbustype,cColor,cptcode,rid", "dtmxlist", Cmd);
            string ccc_contion = "";
            int irowidx = 0;
            int ipzid = 1;
            for (int mx = 0; mx < dtmxlist.Rows.Count; mx++)
            {
                if (ccc_contion != "" + dtmxlist.Rows[mx]["ccc_contin"])
                {
                    irowidx = 0;
                    ccc_contion = "" + dtmxlist.Rows[mx]["ccc_contin"];
                }

                Cmd.CommandText = "update #tmp_vouch_list set vouch_idx=" + ipzid + " where rid=" + dtmxlist.Rows[mx]["rid"];
                Cmd.ExecuteNonQuery();
                irowidx++;

                if (irowidx >= i_rows_count)
                {
                    ipzid++;
                    ccc_contion = "";
                }
            }

            //写发票
            DataTable dtVouch = U8Operation.GetSqlDataTable(@"select cvencode,cexch_name,rdtype,cbustype,cColor,cptcode,vouch_idx,min(autoid) autoid,iexchrate
                from #tmp_vouch_list group by cvencode,cexch_name,rdtype,cbustype,cColor,cptcode,vouch_idx,iexchrate", "dtVouch", Cmd);
            if (dtVouch.Rows.Count == 0) throw new Exception("没有获得有效数据，请确认本月是否已经确认过");

            for (int i = 0; i < dtVouch.Rows.Count; i++)
            {
                #region  //写发票主表
                KK_U8Com.U8PurBillVouch pumain = new KK_U8Com.U8PurBillVouch(Cmd, cDBOwer);
                pumain.PBVID = 1000000000 + int.Parse(U8Operation.GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PURBILL'", Cmd));
                Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PURBILL'";
                Cmd.ExecuteNonQuery();
                
                string cCodeHead = "P" + U8Operation.GetDataString("select left(replace(convert(varchar(10),getdate(),120),'-',''),6)", Cmd);
                string cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cPBVCode),4),'0000') as int)+1 as varchar(9)),4) from " + cDBOwer + "..PurBillVouch where cPBVCode like '" + cCodeHead + "%'", Cmd);
                pumain.cPBVCode = "'" + cc_mcode + "'";
                pumain.dPBVDate = "'" + U8Operation.GetDataString("select convert(varchar(10),getdate(),120)",Cmd) + "'";   //登录注册日期
                pumain.cPBVMaker = "'" + cmaker + "'";
                if (dtVouch.Rows[i]["rdtype"] + "" == "sfc")
                {
                    pumain.cPBVBillType = "'07'"; //加工发票
                    pumain.cBusType = "'工序委外'";
                    pumain.cSource = "'工序委外'";
                    pumain.iVTid = 30915;
                    pumain.cPTCode = "Null";
                }
                else
                {
                    pumain.cPBVBillType = "'01'";
                    string c_pt_code = U8Operation.GetDataString("select cptcode from " + cDBOwer + @"..v_cqcc_gzd_mx_list where substring(rdtype,4,20)='" + dtVouch.Rows[i]["rdtype"] + "' and autoid=0" + dtVouch.Rows[i]["autoid"], Cmd);
                    if (c_pt_code == "") c_pt_code = "" + dtVouch.Rows[i]["cptcode"];
                    pumain.cPTCode = (c_pt_code == "" ? "null" : "'" + c_pt_code + "'");
                    string cc_bus = U8Operation.GetDataString("select cbustype from " + cDBOwer + @"..v_cqcc_gzd_mx_list where substring(rdtype,4,20)='" + dtVouch.Rows[i]["rdtype"] + "' and autoid=0" + dtVouch.Rows[i]["autoid"], Cmd);
                    if (cc_bus == "委外加工")
                    {
                        pumain.cBusType = "'委外加工'";
                        pumain.cSource = "'委外'";
                    }
                    else
                    {
                        pumain.cBusType = "'普通采购'";
                        pumain.cSource = "'采购'";
                    }
                }

                pumain.bNegative = "0"; //红篮子发票
                if (dtVouch.Rows[i]["cColor"] + "" == "red") pumain.bNegative = "1";

                pumain.cVenCode = "'" + dtVouch.Rows[i]["cvencode"] + "'";
                pumain.cUnitCode = "'" + dtVouch.Rows[i]["cvencode"] + "'";
                pumain.cDefine3 = "'VMI_INPUT'";
                pumain.cExchRate = "" + dtVouch.Rows[i]["iexchrate"];
                pumain.cexch_name = "'" + dtVouch.Rows[i]["cexch_name"] + "'";
                pumain.iPBVTaxRate = "13";

                //供应商账期处理
                #region
                //本月 月结日期
                string c_jz_date = "" + U8Operation.GetDataString(@"SELECT convert(varchar(10),dEnd,120) FROM ufsystem.dbo.UA_Period
                    where cAcc_Id='" + cacc_id + "' and dBegin<=" + pumain.dPBVDate + " and dEnd>=" + pumain.dPBVDate, Cmd);
                if (c_jz_date.CompareTo("") == 0) throw new Exception("没有找到日期" + pumain.dPBVDate + "在账套【" + cacc_id + "】中的月份");

                DataTable dtZQ = U8Operation.GetSqlDataTable(@"select a.cVenPUOMProtocol,case when iLZYJ=4 then 1 else 0 end 是否立账依据,
		                        case when iLZFS=2 then (
			                             case when day(" + pumain.dPBVDate + " )>iday1 then convert(varchar(8),dateadd(month,1," + pumain.dPBVDate + @"),120)+cast((case when iday1=0 then 1 else iday1 end) as varchar(2))  
			                             else convert(varchar(8)," + pumain.dPBVDate + @",120)+cast(iday1 as varchar(2)) end
			                            ) 
			                         when iLZFS=1 then '" + c_jz_date + "' else " + pumain.dPBVDate + @" end  立账日期,
		                        case when iZQ=1 then isnull(a.iVenCreDate,0) else isnull(dblzqnum,0) end 账期天数
                        from " + cDBOwer + @"..vendor a inner join " + cDBOwer + @"..AA_Agreement b on a.cVenPUOMProtocol=b.ccode where a.cvencode=" + pumain.cVenCode, "dtZQ", Cmd);
                if (dtZQ.Rows.Count > 0)
                {
                    pumain.cVenPUOMProtocol = "'" + dtZQ.Rows[0]["cVenPUOMProtocol"] + "'";  //收款协议
                    string c_cur_day = "" + dtZQ.Rows[0]["立账日期"];
                    string c_new_lzr = U8Operation.GetDataString("select convert(varchar(10),dateadd(dd,-1,dateadd(m,1,'" + c_cur_day.Substring(0, 7) + "'+'-01')),120)", Cmd);//本月最后一天
                    pumain.dCreditStart = "'" + (c_cur_day.CompareTo(c_new_lzr) <= 0 ? c_cur_day : c_new_lzr) + "'";  //立账日
                    pumain.dGatheringDate = "'" + U8Operation.GetDataString("select convert(varchar(10),dateadd(day," + dtZQ.Rows[0]["账期天数"] + "," + pumain.dCreditStart + "),120)", Cmd) + "'";  //到期日
                    pumain.iCreditPeriod = "" + dtZQ.Rows[0]["账期天数"];  //账期天数
                    pumain.bCredit = dtZQ.Rows[0]["是否立账依据"] + "";   //是否立账单据
                }
                #endregion

                if (!pumain.InsertToDB(targetAccId, ref errmsg)) throw new Exception(errmsg);
                #endregion

                DataTable dtBody = U8Operation.GetSqlDataTable(@"select b.cc_identity,a.autoid,left(a.rdtype,2) cpu_type,ivalidqty pu_qty,a.iquantity iqty,ioritaxcost,iorisum,b.cc_rate itaxrate,cfp_invcode cinvcode,
                    b.cc_taxmoney q_oriSum,b.cc_taxmoney-b.cc_tax cc_imoney,ccode,kd_qty,kd_cost,b.cc_kd_money kd_money,b.cc_tax,cfree1,cfree2 
                    from " + cDBOwer + @"..v_cqcc_gzd_mx_list a inner join " + cDBOwer + @"..T_CC_GZD_Pre b on a.rdtype=b.cc_rdtype and a.autoid=b.cc_autoid and a.cvencode=b.cc_vencode
                        inner join #tmp_vouch_list m on b.cc_identity=m.cc_identity
                    where m.vouch_idx='" + dtVouch.Rows[i]["vouch_idx"] + "'  and a.cvencode='" + cvencode + "' and substring(a.rdtype,4,20)='" + dtVouch.Rows[i]["rdtype"] + @"' and cc_HasCost='有价' and cc_kp_autoid is null 
                        and m.cexch_name=" + pumain.cexch_name + " and a.cptcode='" + dtVouch.Rows[i]["cptcode"] + "' and a.cbustype='" + dtVouch.Rows[i]["cbustype"] + @"' 
                        and m.cColor='" + dtVouch.Rows[i]["cColor"] + "' order by a.autoid", "dtBody", Cmd);

                //写发票子表
                for (int l = 0; l < dtBody.Rows.Count; l++)
                {
                    KK_U8Com.U8PurBillVouchs pudetail = new KK_U8Com.U8PurBillVouchs(Cmd, cDBOwer);
                    pudetail.ID = int.Parse(U8Operation.GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='PURBILL'", Cmd));
                    Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='PURBILL'";
                    Cmd.ExecuteNonQuery();
                    pudetail.PBVID = pumain.PBVID;
                    pudetail.cInvCode = "'" + dtBody.Rows[l]["cinvcode"] + "'";
                    pudetail.ivouchrowno = l + 1;
                    pudetail.iTaxRate = "" + dtBody.Rows[l]["itaxrate"];

                    //加工费 发票和 材料发票的处理
                    #region
                    if (pumain.cPBVBillType == "'07'")  //工序加工
                    {
                        pudetail.UpSoType = "Null";
                        pudetail.iNum = pudetail.iPBVQuantity;
                        DataTable dtGZD = U8Operation.GetSqlDataTable(@"select a.autoid,b.IPROORDERAUTOID modid,b.copcode,b.cdescription,a.chyordercode,a.ihyorderdid,b.opseq 
                            from " + cDBOwer + @"..HY_UsedVouchs a inner join " + cDBOwer + @"..HY_MODetails b on a.ihyorderdid=b.autoid 
                            where a.autoid=0" + dtBody.Rows[l]["autoid"], "dtGZD", Cmd);
                        if (dtGZD.Rows.Count > 0)
                        {
                            pudetail.opseq = "'" + dtGZD.Rows[0]["opseq"] + "'";
                            pudetail.copcode = "'" + dtGZD.Rows[0]["copcode"] + "'";
                            pudetail.cdescription = "'" + dtGZD.Rows[0]["cdescription"] + "'";
                            pudetail.chyordercode = "'" + dtGZD.Rows[0]["chyordercode"] + "'";
                            pudetail.ihyorderdid = "" + dtGZD.Rows[0]["ihyorderdid"];
                        }
                    }
                    else
                    {
                        pudetail.UpSoType = "'" + dtVouch.Rows[i]["rdtype"] + "'";  //采购入库单还是挂账确认单
                    }
                    #endregion

                    //处理是否 折扣   pu代表正常业务   其他为  折扣
                    #region
                    if (dtBody.Rows[l]["cpu_type"].ToString().CompareTo("pu") == 0)
                    {
                        //正常 生单形成发票
                        pudetail.RdsId = "" + dtBody.Rows[l]["autoid"];
                        pudetail.iOriSum = "" + dtBody.Rows[l]["q_oriSum"];
                        pudetail.iOriMoney = "" + dtBody.Rows[l]["cc_imoney"];
                        pudetail.iPBVQuantity = "" + dtBody.Rows[l]["pu_qty"];
                    }
                    else
                    {
                        //扣点，折扣发票
                        pudetail.iOriSum = "" + dtBody.Rows[l]["kd_money"];
                        pudetail.iPBVQuantity = "0";       // +dtBody.Rows[l]["kd_qty"];
                    }

                    pudetail.iOriMoney = "" + (float.Parse(pudetail.iOriSum) - float.Parse("" + dtBody.Rows[l]["cc_tax"]));  //原币无税金额
                    #endregion

                    //自由项
                    pudetail.cFree1 = "'" + dtBody.Rows[l]["cfree1"] + "'";
                    pudetail.cFree2 = "'" + dtBody.Rows[l]["cfree2"] + "'";

                    pudetail.cDefine24 = "'VMI_INPUT'";
                    
                    if (!pudetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                    #region //后续逻辑关系处理
                    if (dtVouch.Rows[i]["rdtype"] + "" == "vmiused")
                    {
                        string kpall = U8Operation.GetDataString("select isnull(sum(iPBVQuantity),0) from " + cDBOwer + @"..PurBillVouchs where upsotype='vmiused' and rdsid=0" + dtBody.Rows[l]["autoid"], Cmd);
                        ////回写挂账确认单的 开票数
                        Cmd.CommandText = "update " + cDBOwer + @"..PU_T_VMIUsedVouchs set iKPQuantity=0" + kpall + "  where autoid=0" + dtBody.Rows[l]["autoid"];
                        Cmd.ExecuteNonQuery();

                        if (dtBody.Rows[l]["cpu_type"].ToString().CompareTo("pu") == 0)
                        {
                            //检验累计开票数
                            if (U8Operation.GetDataInt(@"select count(*) from " + cDBOwer + @"..PU_T_VMIUsedVouchs where autoid=0" + dtBody.Rows[l]["autoid"] +
                                " and ((iconfirmquantity>0 and isnull(iKPQuantity,0)>iconfirmquantity) or (iconfirmquantity<0 and isnull(iKPQuantity,0)<iconfirmquantity)) ", Cmd) > 0)
                            {
                                throw new Exception("确认单[" + dtBody.Rows[l]["ccode"] + "]存货[" + dtBody.Rows[l]["cinvcode"] + "]超数量开票");
                            }
                        }
                    }
                    else if (dtVouch.Rows[i]["rdtype"] + "" == "sfc")  //工序挂账确认单
                    {
                        if (dtBody.Rows[l]["cpu_type"].ToString().CompareTo("pu") == 0)
                        {
                            //更新结算计量单位
                            Cmd.CommandText = "update " + cDBOwer + @"..PurBillVouchs set cUnitID=(select max(cComUnitCode) from " + cDBOwer + @"..inventory where cinvcode=" + pudetail.cInvCode +
                                "),inattaxprice=iOriTaxCost,iexmoney=0,ilostquan=0,iNlostQuan=0,iNLostmoney=0,mNlosttax=0 where id=0" + pudetail.ID;
                            Cmd.ExecuteNonQuery();
                            
                            //写挂账单累计开票数
                            Cmd.CommandText = "update " + cDBOwer + @"..HY_UsedVouchs set iinvqty=isnull(iinvqty,0)+(0" + pudetail.iPBVQuantity +
                                "),iinvmoney=isnull(iinvmoney,0)+(" + pudetail.iSum + ")*inattaxprice where autoid=0" + dtBody.Rows[l]["autoid"];
                            Cmd.ExecuteNonQuery();
                            
                            //检查是否超开票
                            float fKpSum = float.Parse(U8Operation.GetDataString(@"select isnull(sum(b.ipbvquantity),0) ipbvqty from " + cDBOwer + @"..PurBillVouch a inner join " + cDBOwer + @"..PurBillVouchs b on a.pbvid=b.pbvid
                                where cPBVBillType ='07' and cBusType='工序委外' and b.RdsId=0" + dtBody.Rows[l]["autoid"],Cmd));//dtGZD
                            float fGzSum = float.Parse(U8Operation.GetDataString(@"select isnull(max(iquantity),0) from " + cDBOwer + @"..HY_UsedVouchs where autoid=0" + dtBody.Rows[l]["autoid"], Cmd));
                            if (fKpSum - fGzSum > 0.01)
                            {
                                throw new Exception("加工单[" + dtBody.Rows[l]["ccode"] + "] 累计开票数大于挂账数");
                            }

//                            //写外协加工明细账
//                            Cmd.CommandText = @"insert into " + cDBOwer + @"..hy_listreport(oid,cvouchertype,cheadid,cbodyid,fquantity,mReceivemoney,mUsedvouchmoney,mPurbillMoney,fjquantity,mjmoney,
//                                fzquantity,fzmoney,drmonth,dumonth,dpmonth,cbillvouchcode,modeptcode,wccode,cworkdepcode,mocode,moseq,opseq)
//                                select newid(),'发票',a.pbvid cheadid,b.id cbodyid,b.iPBVQuantity fquantity,0 mReceivemoney,0 mUsedvouchmoney,b.iOriMoney mPurbillMoney,0 fjquantity,b.iOriMoney-round(b.iPBVQuantity*isnull(t.frvprice,0),4) mjmoney,
//                                    b.iPBVQuantity fzquantity,round(b.iPBVQuantity*isnull(t.frvprice,0),4) fzMoney,t.drmonth,month(g.dDate)dumonth,month(a.dPBVDate)dpmonth,
//                                    a.cpbvcode cbillvouchcode,f.MDeptCode modeptcode,null wccode,f.MDeptCode cworkdepcode,d.cmoordercode mocode,d.imoorderseq moseq,d.opseq 
//                                    from " + cDBOwer + @"..PurBillVouch a inner join " + cDBOwer + @"..PurBillVouchs b on a.pbvid=b.pbvid
//                                    inner join " + cDBOwer + @"..HY_UsedVouchs d on b.RdsId=d.autoid
//                                    inner join " + cDBOwer + @"..HY_UsedVouch g on d.ID=g.ID
//                                    left join (select b.mocode,a.sortseq,mdeptcode from " + cDBOwer + @"..mom_orderdetail a inner join " + cDBOwer + @"..mom_order b on a.moid=b.moid) f on d.cmoordercode=f.mocode and d.imoorderseq=f.sortseq
//                                    left join (select iUsedVouchAutoId,sum(a.iQuantity) irvqty,sum(a.iQuantity*b.iUnitPrice)/sum(a.iQuantity) frvprice,min(month(c.ddate)) drmonth 
//		                                from " + cDBOwer + @"..hy_receivedetail a inner join " + cDBOwer + @"..HY_MODetails b on a.iOrderDId=b.AUTOID inner join " + cDBOwer + @"..hy_receive c on a.receiveID=c.receiveID
//		                                where isnull(iUsedVouchAutoId,0)>0 group by iUsedVouchAutoId) t on d.autoid=t.iUsedVouchAutoId
//                                where b.id=" + pudetail.ID;
//                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        ////回写采购入库单的 开票数
                        //Cmd.CommandText = "update " + cDBOwer + @"..rdrecords01 set iSumBillQuantity=isnull(iSumBillQuantity,0)+(" + pudetail.iPBVQuantity + ")  where autoid=" + dtBody.Rows[l]["autoid"];
                        //Cmd.ExecuteNonQuery();
                        if (dtBody.Rows[l]["cpu_type"].ToString().CompareTo("pu") == 0)
                        {
                            string kpall = U8Operation.GetDataString("select isnull(sum(iPBVQuantity),0) from " + cDBOwer + @"..PurBillVouchs where upsotype='rd' and rdsid=0" + dtBody.Rows[l]["autoid"], Cmd);
                            Cmd.CommandText = "update " + cDBOwer + "..rdrecords01 set iSumBillQuantity=0" + kpall + @" where autoid=0" + dtBody.Rows[l]["autoid"];
                            Cmd.ExecuteNonQuery();
                            //检验累计开票数
                            if (U8Operation.GetDataInt(@"select count(*) from " + cDBOwer + @"..rdrecords01 where autoid=0" + dtBody.Rows[l]["autoid"] + @" 
                                and ((iquantity>0 and isnull(iSumBillQuantity,0)>iquantity) or (iquantity<0 and isnull(iSumBillQuantity,0)<iquantity)) ", Cmd) > 0)
                            {
                                throw new Exception("采购入库单[" + dtBody.Rows[l]["ccode"] + "]存货[" + dtBody.Rows[l]["cinvcode"] + "]超数量开票");
                            }
                        }
                    }
                    #endregion

                    //Cmd.CommandText = "update " + cDBOwer + @"..PurBillVouch set iPBVTaxRate=" + pudetail.iTaxRate + " where PBVID=" + pudetail.PBVID;
                    //Cmd.ExecuteNonQuery();

                    //更新备挂记录
                    if (float.Parse("" + dtBody.Rows[l]["cc_identity"]) == 0)  
                    {
                        //汇总合计扣款记录写统一发票标识
                        Cmd.CommandText = "update " + cDBOwer + @"..T_CC_GZD_Pre set cc_kp_autoid=" + pudetail.ID + @" 
                            where cc_vencode='" + cvencode + "' and SUBSTRING(cc_rdtype,4,20)='" + dtVouch.Rows[i]["rdtype"] + @"' 
                                and cc_HasCost='有价' and cc_kp_autoid is null and cc_kd_money<>0 
                                and b.cc_identity in(select id from VenDB..cc_fire_pugz where tempKey='" + idlist + "')";
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Cmd.CommandText = "update " + cDBOwer + @"..T_CC_GZD_Pre set cc_kp_autoid=" + pudetail.ID + " where cc_identity=" + dtBody.Rows[l]["cc_identity"];
                        Cmd.ExecuteNonQuery();
                    }

                    Cmd.CommandText = "select '" + idlist + "-" + i + "-" + l + "'";
                    Cmd.ExecuteNonQuery();
                }


            }

            //删除临时表数据
            Cmd.CommandText = "delete from VenDB..cc_fire_pugz where tempKey='" + idlist + "'";
            Cmd.ExecuteNonQuery();

            Cmd.Transaction.Commit();
            str_resulte = str_resulte.Replace("@@resulte@@", "true").Replace("@@msg@@", "");
            WriteDebug("str_result【" + str_resulte + "】", "venlog");
            return str_resulte;
        }
        catch (Exception ex)
        {
            Cmd.Transaction.Rollback();
            str_resulte = str_resulte.Replace("@@resulte@@", "false").Replace("@@msg@@", ex.Message);
            //WriteDebug("str_result【" + str_resulte + "】", "venlog");
            return str_resulte;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }
        
    }



}

