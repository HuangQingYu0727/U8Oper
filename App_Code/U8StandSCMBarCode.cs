using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Data;
using System.Text;

/// <summary>
/// U8StandSCMBarCode 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class U8StandSCMBarCode : System.Web.Services.WebService
{
    public U8StandSCMBarCode()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 

    }

    //获得U8版本
    private bool CanWriteBarCode(string db_name, System.Data.SqlClient.SqlCommand sqlcmd)
    {
        if (float.Parse(U8Operation.GetDataString("select cast(cValue as float) from " + db_name + "..AccInformation where cSysID='AA' and cName='VersionFlag'", sqlcmd)) >= 11)
            return true;
        else
            return false;
    }

    //判断表头 是否存在 某个栏目
    private bool IsExistHeadCol(DataTable formData, string txt_fieldname)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null) return true;
        return false;
    }

    //获得单据头 文本信息
    private string GetTextsFrom_FormData_Text(DataTable formData, string txt_fieldname)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null) return "" + DR[3];
        return "";
    }

    //设置表头栏目 的具体值
    private void SetTextsFrom_FormData(DataTable formData, string txt_fieldname,string tagvalue,string textvalue)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null)
        {
            DR["TxtTag"] = "" + tagvalue;
            DR["TxtValue"] = "" + textvalue;
        }
    }

    //获得单据头 标识 信息
    private string GetTextsFrom_FormData_Tag(DataTable formData, string txt_fieldname)
    {
        System.Data.DataRow DR = formData.Rows.Find(txt_fieldname);
        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (DR != null) return "" + DR[2];
        return "";
    }

    //获得单据头 栏目信息
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

   //获得单据体具体栏目 值
    private string GetBodyValue_FromData(DataTable bodydata,int irow,string colname)
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

    public DataTable GetDtToHeadData(DataTable dtData, int rIndex)
    {
        DataTable Datas = new DataTable("HeadData");
        //显示标题   txt_字段名称    文本Tag     文本Text值
        Datas.Columns.Add("LabelText"); Datas.Columns.Add("TxtName"); Datas.Columns.Add("TxtTag"); Datas.Columns.Add("TxtValue");
        for (int i = 0; i < dtData.Columns.Count; i++)
        {
            DataRow dr = Datas.NewRow();
            dr["LabelText"] = dtData.Columns[i].ColumnName; //栏目标题 
            dr["TxtName"] = "txt_"+dtData.Columns[i].ColumnName;   //txt_字段名称
            dr["TxtTag"] = dtData.Rows[rIndex][i] + "";  //标识
            dr["TxtValue"] = dtData.Rows[rIndex][i] + "";//栏目值
            Datas.Rows.Add(dr);
        }
        return Datas;
    }

    private System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, string cvmi_code, decimal iOutQty, bool bpos, System.Data.SqlClient.SqlCommand sqlcmd, string db_name)
    {
        return GetBatDTFromWare(cwhcode, cinvcode, cvmi_code, iOutQty, bpos, sqlcmd, "", db_name);
    }
    private System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, string cvmi_code, decimal iOutQty, bool bpos, System.Data.SqlClient.SqlCommand sqlcmd, string cpos_code, string db_name)
    {
        System.Data.DataTable dtbatlist = null;
        string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + db_name + @"..AccInformation(nolock) WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", sqlcmd);
        // 货位
        if (bpos)
        {
            if (cpos_code != "")
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,cPosCode cposcode,round(iquantity," + cInv_DecDgt + @") iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMadeDate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..InvPositionSum(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cvmivencode='" + cvmi_code + "' and cPosCode='" + cpos_code + "' and round(iquantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);
            }
            else
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,cPosCode cposcode,round(iquantity," + cInv_DecDgt + @") iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMadeDate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..InvPositionSum(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cvmivencode='" + cvmi_code + "' and round(iquantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);
            }
        }
        else
        {
            if (iOutQty > 0)
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,'' cposcode,round(iquantity," + cInv_DecDgt + @") iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..CurrentStock(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cvmivencode='" + cvmi_code + "' and round(iquantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);
            }
            else
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,'' cposcode,round(iquantity," + cInv_DecDgt + @") iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..CurrentStock(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cvmivencode='" + cvmi_code + "' order by cbatch", "dtbatlist", sqlcmd);

            }
            // 处理模具负库存
            if (dtbatlist.Rows.Count == 0)
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select '' cbatch,'' cposcode," + iOutQty + @" iquantity,'' cfree1,'' cfree2,'' cfree3,'' cfree4,'' cfree5,
                    '' cfree6,'' cfree7,'' cfree8,'' cfree9,'' cfree10,
	                null 保质期天数,null 保质期单位,null 生产日期,null 失效日期,null 有效期至,null 有效期推算方式,null 有效期计算项", "dtbatlist", sqlcmd);
                return dtbatlist;
            }

            //            //可用量出库
            //            dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,'' cposcode,round(iQuantity+fInQuantity+fTransInQuantity-fOutQuantity-fTransOutQuantity," + cInv_DecDgt + @") iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
            //	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
            //                from " + db_name + @"..CurrentStock
            //                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cvmivencode='" + cvmi_code + "' and round(iQuantity+fInQuantity+fTransInQuantity-fOutQuantity-fTransOutQuantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);

        }
        System.Data.DataTable dtRet = new System.Data.DataTable("dtbat");

        for (int c = 0; c < dtbatlist.Columns.Count; c++)
        { dtRet.Columns.Add(dtbatlist.Columns[c].ColumnName); }

        for (int c = 0; c < dtbatlist.Rows.Count; c++)
        {
            decimal fMerOut = 0;

            if (iOutQty <= decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]))
            {
                fMerOut = iOutQty;
                iOutQty = 0;
            }
            else
            {
                fMerOut = decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]);
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

        if (iOutQty > 0) throw new Exception("仓库[" + cwhcode + "]存货[" + cinvcode + "]可用批次库存不够");
        return dtRet;
    }

    //考虑缓存数据
    private System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, string cvmi_code, decimal iOutQty, bool bpos, string tmpTable, System.Data.SqlClient.SqlCommand sqlcmd, string db_name)
    {
        System.Data.DataTable dtbatlist = null;
        if (bpos)
        {
            dtbatlist = U8Operation.GetSqlDataTable(@"select a.cbatch,a.cPosCode cposcode,cast(a.iquantity-isnull(b.iqty,0) as float) iquantity,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMadeDate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..InvPositionSum a(nolock) left join  (
                    select cwhcode,cinvcode,cposcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,sum(iquantity) iqty 
                    from " + tmpTable + @" t group by cwhcode,cinvcode,cposcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10
                ) b 
                    on a.cwhcode=b.cwhcode and a.cinvcode=b.cinvcode and a.cposcode=b.cposcode and a.cbatch=b.cbatch and a.cvmivencode=b.cvmivencode
                        and a.cfree1=b.cfree1 and a.cfree2=b.cfree2 and a.cfree3=b.cfree3 and a.cfree4=b.cfree4 and a.cfree5=b.cfree5 and a.cfree6=b.cfree6 and a.cfree7=b.cfree7
                        and a.cfree8=b.cfree8 and a.cfree9=b.cfree9 and a.cfree10=b.cfree10
                where a.cwhcode='" + cwhcode + "' and a.cinvcode='" + cinvcode + "' and a.cvmivencode='" + cvmi_code + "' and a.iquantity-isnull(b.iqty,0)>0 order by a.cbatch", "dtbatlist", sqlcmd);
        }
        else
        {
            dtbatlist = U8Operation.GetSqlDataTable(@"select a.cbatch,'' cposcode,cast(a.iquantity-isnull(b.iqty,0) as float) iquantity,a.cfree1,a.cfree2,a.cfree3,a.cfree4,a.cfree5,a.cfree6,a.cfree7,a.cfree8,a.cfree9,a.cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..CurrentStock a(nolock) left join (
                    select cwhcode,cinvcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,sum(iquantity) iqty 
                    from " + tmpTable + @" t group by cwhcode,cinvcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10
                ) b 
                    on a.cwhcode=b.cwhcode and a.cinvcode=b.cinvcode and a.cbatch=b.cbatch and a.cvmivencode=b.cvmivencode
                        and a.cfree1=b.cfree1 and a.cfree2=b.cfree2 and a.cfree3=b.cfree3 and a.cfree4=b.cfree4 and a.cfree5=b.cfree5 and a.cfree6=b.cfree6 and a.cfree7=b.cfree7
                        and a.cfree8=b.cfree8 and a.cfree9=b.cfree9 and a.cfree10=b.cfree10
                where a.cwhcode='" + cwhcode + "' and a.cinvcode='" + cinvcode + "' and a.cvmivencode='" + cvmi_code + "' and a.iquantity-isnull(b.iqty,0)>0 order by a.cbatch", "dtbatlist", sqlcmd);
            // 处理模具负库存
            if (dtbatlist.Rows.Count == 0)
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select '' cbatch,'' cposcode," + iOutQty + @" iquantity,'' cfree1,'' cfree2,'' cfree3,'' cfree4,'' cfree5,
                    '' cfree6,'' cfree7,'' cfree8,'' cfree9,'' cfree10,
	                null 保质期天数,null 保质期单位,null 生产日期,null 失效日期,null 有效期至,null 有效期推算方式,null 有效期计算项", "dtbatlist", sqlcmd);
                return dtbatlist;
            }
        }
        System.Data.DataTable dtRet = new System.Data.DataTable("dtbat");

        for (int c = 0; c < dtbatlist.Columns.Count; c++)
        { dtRet.Columns.Add(dtbatlist.Columns[c].ColumnName); }

        for (int c = 0; c < dtbatlist.Rows.Count; c++)
        {
            decimal fMerOut = 0;

            if (iOutQty <= decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]))
            {
                fMerOut = iOutQty;
                iOutQty = 0;
            }
            else
            {
                fMerOut = decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]);
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

        if (iOutQty > 0) throw new Exception("仓库[" + cwhcode + "]存货[" + cinvcode + "]可用批次库存不够");
        return dtRet;
    }

    private System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, string cvmi_code,string cbatch,string cposcode,
        string cfree1, string cfree2, string cfree3, string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10,
        decimal iOutQty, bool bpos, System.Data.SqlClient.SqlCommand sqlcmd, string db_name)
    {
        System.Data.DataTable dtbatlist = null;
        string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + db_name + @"..AccInformation(nolock) WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", sqlcmd);
        if (iOutQty > 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + db_name + "..inventory(nolock) where cinvcode='" + cinvcode + "' and bInvBatch=0", sqlcmd) > 0)
            { cbatch = ""; }

            if (bpos)
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,cPosCode cposcode,cast(iquantity as float) istockqty," + iOutQty + @" iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMadeDate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..InvPositionSum(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cbatch='" + cbatch + "' and cposcode='" + cposcode + @"' 
                    and cvmivencode='" + cvmi_code + "' and  cfree1='" + cfree1 + "' and cfree2='" + cfree2 + "' and cfree3='" + cfree3 + "' and cfree4='" + cfree4 + @"' 
                    and cfree5='" + cfree5 + "'and cfree6='" + cfree6 + "' and cfree7='" + cfree7 + "' and cfree8='" + cfree8 + "' and cfree9='" + cfree9 + "' and cfree10='" + cfree10 + @"' 
                    and round(iquantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);
            }
            else
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,'' cposcode,cast(iquantity as float) istockqty," + iOutQty + @" iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..CurrentStock(nolock)
                where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and cbatch='" + cbatch + "' and cvmivencode='" + cvmi_code + @"' 
                    and cfree1='" + cfree1 + "' and cfree2='" + cfree2 + "' and cfree3='" + cfree3 + "' and cfree4='" + cfree4 + "' and cfree5='" + cfree5 + @"'
                    and cfree6='" + cfree6 + "' and cfree7='" + cfree7 + "' and cfree8='" + cfree8 + "' and cfree9='" + cfree9 + "' and cfree10='" + cfree10 + @"'
                    and round(iquantity," + cInv_DecDgt + ")>0 order by cbatch", "dtbatlist", sqlcmd);
            }
        }
        else  //红字单据
        {
            //dtbatlist = U8Operation.GetSqlDataTable(@"select ", "dtbatlist", sqlcmd);
            if (cbatch != "" && U8Operation.GetDataInt("select count(*) from " + db_name + "..inventory where cinvcode='" + cinvcode + "' and bInvQuality=1", sqlcmd) > 0)
            {
                string rowpordate = U8Operation.GetDataString("select top 1 convert(varchar(10),dMdate,120) from " + db_name + "..CurrentStock where cInvCode = '"+ cinvcode + "' and cBatch = '" + cbatch + "'", sqlcmd);
                dtbatlist = U8Operation.GetSqlDataTable(@"select '" + cbatch + "' cbatch,'" + cposcode + "' cposcode,0 istockqty," + iOutQty + @" iquantity,
                            '" + cfree1 + "' cfree1,'" + cfree2 + "' cfree2,'" + cfree3 + "' cfree3,'" + cfree4 + "' cfree4,'" + cfree5 + @"' cfree5,
                            '" + cfree6 + "' cfree6,'" + cfree7 + "' cfree7,'" + cfree8 + "' cfree8,'" + cfree9 + "' cfree9,'" + cfree10 + @"' cfree10,
                            iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                                else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                            ,120) 失效日期,'' 有效期至,s.iExpiratDateCalcu 有效期推算方式,'" + rowpordate + @"' 有效期计算项
                        from " + db_name + "..inventory i(nolock) left join " + db_name + @"..Inventory_Sub s(nolock) on i.cinvcode=s.cinvsubcode 
                        where cinvcode='" + cinvcode + "'", "dtbatlist", sqlcmd);
                //2022-11-11 修改为取库存批次号原生产日期
                for (int r = 0; r < dtbatlist.Rows.Count; r++)
                {
                    dtbatlist.Rows[r]["有效期至"] = dtbatlist.Rows[r]["失效日期"];
                }
            }
            else
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select '" + cbatch + "' cbatch,'" + cposcode + "' cposcode,0 istockqty," + iOutQty + @" iquantity,
                            '" + cfree1 + "' cfree1,'" + cfree2 + "' cfree2,'" + cfree3 + "' cfree3,'" + cfree4 + "' cfree4,'" + cfree5 + @"' cfree5,
                            '" + cfree6 + "' cfree6,'" + cfree7 + "' cfree7,'" + cfree8 + "' cfree8,'" + cfree9 + "' cfree9,'" + cfree10 + @"' cfree10,
                            0 保质期天数,'' 保质期单位,convert(varchar(10),getdate(),120) 生产日期,convert(varchar(10),getdate(),120) 失效日期,
                        convert(varchar(10),getdate(),120) 有效期至,0 有效期推算方式,convert(varchar(10),getdate(),120) 有效期计算项", "dtbatlist", sqlcmd);
            }
        }
        //未找到记录，组建空数据记录
        if (dtbatlist.Rows.Count == 0)
        {
            DataRow drow = dtbatlist.NewRow();
            drow["cbatch"] = cbatch;
            drow["cposcode"] = cposcode;
            drow["istockqty"] = "0";
            drow["iquantity"] = "" + iOutQty;
            drow["cfree1"] = cfree1;
            drow["cfree2"] = cfree2;
            drow["cfree3"] = cfree3;
            drow["cfree4"] = cfree4;
            drow["cfree5"] = cfree5;
            drow["cfree6"] = cfree6;
            drow["cfree7"] = cfree7;
            drow["cfree8"] = cfree8;
            drow["cfree9"] = cfree9;
            drow["cfree10"] = cfree10;
            drow["保质期天数"] = 0;
            drow["保质期单位"] = 0;
            drow["生产日期"] = "";
            drow["失效日期"] = "";
            drow["有效期至"] = "";
            drow["有效期推算方式"] = 0;
            drow["有效期计算项"] = "";
            dtbatlist.Rows.Add(drow);
        }

        //可用量控制


        return dtbatlist;
    }

    private void CurrentStockCheck(string cwhcode, string cinvcode, string cvmi_code, string cbatch,
        string cfree1, string cfree2, string cfree3, string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10,
        System.Data.SqlClient.SqlCommand sqlcmd, string db_name)
    {
        bool bChecked=true;
        if (cbatch.CompareTo("") != 0)
        {
            string bBatchAllowZero = U8Operation.GetDataString("select cValue from " + db_name + @"..AccInformation(nolock) where cName='bBatchAllowZero' and csysid='st'", sqlcmd);
            if (bBatchAllowZero.ToLower().CompareTo("true") == 0) bChecked = false;
        }
        else
        {
            string bAllowZero = U8Operation.GetDataString("select cValue from " + db_name + @"..AccInformation(nolock) where cName='bAllowZero' and csysid='st'", sqlcmd);
            if (bAllowZero.ToLower().CompareTo("true") == 0) bChecked = false;
        }
        if (bChecked & U8Operation.GetDataInt(@"select count(*) from " + db_name + @"..CurrentStock(nolock)
                where cwhcode=" + cwhcode + " and cinvcode=" + cinvcode + " and cbatch=" + cbatch + " and cvmivencode=" + cvmi_code + @" 
                    and cfree1=" + cfree1 + " and cfree2=" + cfree2 + " and cfree3=" + cfree3 + " and cfree4=" + cfree4 + " and cfree5=" + cfree5 + @"
                    and cfree6=" + cfree6 + " and cfree7=" + cfree7 + " and cfree8=" + cfree8 + " and cfree9=" + cfree9 + " and cfree10=" + cfree10 + @"
                    and iquantity<0", sqlcmd) > 0)
        {
            throw new Exception("仓库[" + cwhcode + "]存货[" + cinvcode + "]出现0出库");
        }

    }

    //现存量负库存控制--whf(添加按照仓库控制是否0出库)
    private void Current_ST_StockCHeck(System.Data.SqlClient.SqlCommand sqlcmd, string db_name, string cwhcode, string cinvcode, string cbatch, string cvmivencode, 
        string cfree1, string cfree2, string cfree3, string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10)
    {
        bool bChecked = false;
        if (cbatch.CompareTo("") == 0 || cbatch=="''"){
            if (!string.IsNullOrEmpty(cwhcode))
            {
                if (!cwhcode.StartsWith("'"))
                {
                    cwhcode = "'" + cwhcode + "'";
                }
                int iSTConMode = U8Operation.GetDataInt("select iSTConMode from " + db_name + @"..Warehouse(nolock) where cWhCode=" + cwhcode , sqlcmd);
                bChecked = iSTConMode==1;
                //bChecked = bool.Parse(U8Operation.Get("select iSTConMode from " + db_name + @"..Warehouse(nolock) where cWhCode='" + cwhcode + "'", sqlcmd));
            }
            else
            {
                bChecked = bool.Parse(U8Operation.GetDataString("select cvalue from " + db_name + @"..AccInformation(nolock) where cName='bAllowZero'", sqlcmd));
            }
        }
        else
            bChecked = bool.Parse(U8Operation.GetDataString("select cvalue from " + db_name + @"..AccInformation(nolock) where cName='bBatchAllowZero'", sqlcmd));

        if (bChecked) return;  //不检查现存量，直接退出
        if (U8Operation.GetDataInt("select COUNT(1) from " + db_name + "..currentstock(nolock) where cInvCode=" + cinvcode + " and cWhCode=" + cwhcode + @" 
            and cBatch=" + cbatch + " and cVMIVenCode =" + cvmivencode + @" and cFree1=" + cfree1 + " and cFree2=" + cfree2 + " and cFree3=" + cfree3 + @" 
            and cFree4=" + cfree4 + " and cFree5=" + cfree5 + " and cFree6=" + cfree6 + " and cFree7=" + cfree7 + " and cFree8=" + cfree8 + @" 
            and cFree9=" + cfree9 + " and cFree10=" + cfree10 + " and iquantity<0", sqlcmd) > 0)
            throw new Exception("存货[" + cinvcode + "]仓库[" + cwhcode + "]批号[" + cbatch + "]超现存量");

    }
    //可用量管控
    private void Current_ST_StockAvableCHeck(System.Data.SqlClient.SqlCommand sqlcmd, string db_name, string cwhcode, string cinvcode, string cbatch, string cvmivencode,
        string cfree1, string cfree2, string cfree3, string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10)
    {
        bool bChecked = false;
        if (cbatch.CompareTo("") == 0)
            bChecked = bool.Parse(U8Operation.GetDataString("select cvalue from " + db_name + @"..AccInformation(nolock) where cName='bAllowZero'", sqlcmd));
        else
            bChecked = bool.Parse(U8Operation.GetDataString("select cvalue from " + db_name + @"..AccInformation(nolock) where cName='bBatchAllowZero'", sqlcmd));

        if (bChecked) return;  //不检查现存量，直接退出
        if (U8Operation.GetDataInt("select COUNT(1) from " + db_name + "..currentstock(nolock) where cInvCode=" + cinvcode + " and cWhCode=" + cwhcode + @" 
            and cBatch=" + cbatch + " and cVMIVenCode =" + cvmivencode + @" and cFree1=" + cfree1 + " and cFree2=" + cfree2 + " and cFree3=" + cfree3 + @" 
            and cFree4=" + cfree4 + " and cFree5=" + cfree5 + " and cFree6=" + cfree6 + " and cFree7=" + cfree7 + " and cFree8=" + cfree8 + @" 
            and cFree9=" + cfree9 + " and cFree10=" + cfree10 + " and iQuantity+fInQuantity+fTransInQuantity-fOutQuantity-fTransOutQuantity<-0.00001", sqlcmd) > 0)
            throw new Exception("存货[" + cinvcode + "]仓库[" + cwhcode + "]批号[" + cbatch + "]超 可用量");

    }

    //货位账负库存控制
    private void Current_Pos_StockCHeck(System.Data.SqlClient.SqlCommand sqlcmd, string db_name, string cwhcode,string cposcode, string cinvcode, string cbatch, string cvmivencode,
        string cfree1, string cfree2, string cfree3, string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10)
    {
        bool bChecked = bool.Parse(U8Operation.GetDataString("select cvalue from " + db_name + @"..AccInformation(nolock) where cName='bPosZeroOut'", sqlcmd));
        if (bChecked) return;  //不检查现存量，直接退出

        if (U8Operation.GetDataInt("select COUNT(*) from " + db_name + "..InvPositionSum(nolock) where cInvCode=" + cinvcode + " and cWhCode=" + cwhcode + @" 
            and cposcode=" + cposcode + " and cBatch=" + cbatch + " and cVMIVenCode =" + cvmivencode + @" and cFree1=" + cfree1 + " and cFree2=" + cfree2 + @" 
            and cFree3=" + cfree3 + " and cFree4=" + cfree4 + " and cFree5=" + cfree5 + " and cFree6=" + cfree6 + " and cFree7=" + cfree7 + " and cFree8=" + cfree8 + @" 
            and cFree9=" + cfree9 + " and cFree10=" + cfree10 + " and iquantity<0", sqlcmd) > 0)
        {
            string coutqty = U8Operation.GetDataString("select iquantity from " + db_name + "..InvPositionSum(nolock) where cInvCode=" + cinvcode + " and cWhCode=" + cwhcode + @" 
                and cposcode=" + cposcode + " and cBatch=" + cbatch + " and cVMIVenCode =" + cvmivencode + @" and cFree1=" + cfree1 + " and cFree2=" + cfree2 + @" 
                and cFree3=" + cfree3 + " and cFree4=" + cfree4 + " and cFree5=" + cfree5 + " and cFree6=" + cfree6 + " and cFree7=" + cfree7 + " and cFree8=" + cfree8 + @" 
                and cFree9=" + cfree9 + " and cFree10=" + cfree10 + " order by iquantity desc", sqlcmd);
            throw new Exception("存货[" + cinvcode + "]仓库[" + cwhcode + "]货位[" + cposcode + "]超现存量,超出[" + coutqty + "]");
        }

    }

    public static void U8V10SetCurrentStockRow(System.Data.SqlClient.SqlCommand sqlCmd, string dbowner, string cwhcode, string cinvcode, string cfree1, string cfree2, string cfree3
           , string cfree4, string cfree5, string cfree6, string cfree7, string cfree8, string cfree9, string cfree10, string cbatch, string vmi)
    {
        
        //单计量单位
        string ItemId = U8Operation.GetDataString("select isnull(max(id),0) from " + dbowner + "SCM_Item(nolock) where cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
            " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
            " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "'", sqlCmd);
        if (ItemId == "0")
        {
            sqlCmd.CommandText = "insert into " + dbowner + @"SCM_Item(cinvcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10) 
                values('" + cinvcode + "','" + cfree1 + "','" + cfree2 + "','" + cfree3 + "','" + cfree4 + "','" + cfree5 + "','" + cfree6 + "','" + cfree7 + "','" + cfree8 + "','" + cfree9 + "','" + cfree10 + "')";
            sqlCmd.ExecuteNonQuery();
            ItemId = U8Operation.GetDataString("select isnull(max(id),0) from " + dbowner + "SCM_Item(nolock) where cinvcode=" + cinvcode + " and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
            " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
            " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "'", sqlCmd);

        }

        string currentID = U8Operation.GetDataString("select isnull(max(autoid),0) from " + dbowner + "currentstock(nolock) where cwhcode='" + cwhcode + "' and cinvcode='" + cinvcode + "' and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
            " and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
            " and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "' " +
            " and isnull(cbatch,'')='" + cbatch + "' and iSoType=0 and iSodid='' and isnull(cVMIVenCode,'')='" + vmi + "'", sqlCmd);
        if (currentID == "0")
        {
            sqlCmd.CommandText = "insert into " + dbowner + @"currentstock(cWhCode,cInvCode,ItemId,cBatch, iSoType, iSodid, iQuantity, iNum,cFree1,cFree2,fOutQuantity, fOutNum, fInQuantity, 
                fInNum,cFree3, cFree4, cFree5, cFree6, cFree7, cFree8, cFree9, cFree10,bStopFlag, fTransInQuantity, fTransInNum, 
                fTransOutQuantity, fTransOutNum,fPlanQuantity, fPlanNum, fDisableQuantity, fDisableNum, fAvaQuantity, fAvaNum, BGSPSTOP, 
                cMassUnit, fStopQuantity, fStopNum, cCheckState,iExpiratDateCalcu, ipeqty,ipenum,cVMIVenCode) values(
                '" + cwhcode + "','" + cinvcode + "'," + ItemId + ",'" + cbatch + "',0,'',0,0,'" + cfree1 + "','" + cfree2 + "',0,0,0,0,'" + cfree3 + "','" + cfree4 + "','" + cfree5 + @"',
                '" + cfree6 + "','" + cfree7 + "','" + cfree8 + "','" + cfree9 + "','" + cfree10 + @"',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'',0,0,0,'" + vmi + "')";
            sqlCmd.ExecuteNonQuery();

            //currentID = U8Operation.GetDataString("select isnull(max(autoid),0) from " + dbowner + "currentstock where cwhcode='" + cwhcode + "' and cinvcode=" + cinvcode + " and isnull(cfree1,'')='" + cfree1 + "' and isnull(cfree2,'')='" + cfree2 + "' " +
            //" and isnull(cfree3,'')='" + cfree3 + "' and isnull(cfree4,'')='" + cfree4 + "' and isnull(cfree5,'')='" + cfree5 + "' and isnull(cfree6,'')='" + cfree6 + "' " +
            //" and isnull(cfree7,'')='" + cfree7 + "' and isnull(cfree8,'')='" + cfree8 + "' and isnull(cfree9,'')='" + cfree9 + "' and isnull(cfree10,'')='" + cfree10 + "' " +
            //" and isnull(cbatch,'')=isnull(" + cbatch + ",'') and iSoType=0 and iSodid='' and isnull(cVMIVenCode,'')=isnull(" + vmi + ",'')", sqlCmd);
        }

    }


    //自动指定批号    仓库编码,存货编码,批号,出库货位,代管供应商,自由项1 ...                                                   自由项10,数量  
    //            如：cwhcode,cinvcode,cbatch,cposcode,cbvencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,iquantity
    [WebMethod]
    public System.Data.DataTable GetBatDTFromWare(string cwhcode, string cinvcode, decimal iOutQty, string db_name)
    {
        bool bpos = false;
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        try
        {
            if (U8Operation.GetDataInt("select count(*) from " + db_name + @"..warehouse(nolock) where cwhcode='" + cwhcode + "' and bWhPos=1", Cmd) > 0)
                bpos = true;
            string ccon_ware = "";
            if (cwhcode.CompareTo("") != 0) ccon_ware = " cwhcode='" + cwhcode + "' and ";
            if (cinvcode.CompareTo("") == 0) throw new Exception("存货编码不能为空");
            System.Data.DataTable dtbatlist = null;
            if (bpos)
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,cPosCode cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cwhcode,cinvcode,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMadeDate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..InvPositionSum(nolock)
                where " + ccon_ware + " cinvcode='" + cinvcode + "' and iquantity>0 order by cbatch,autoid", "dtbatlist", Cmd);
            }
            else
            {
                dtbatlist = U8Operation.GetSqlDataTable(@"select cbatch,'' cposcode,cast(iquantity as float) iquantity,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cwhcode,cinvcode,
	                iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                from " + db_name + @"..CurrentStock(nolock)
                where " + ccon_ware + " cinvcode='" + cinvcode + "' and iquantity>0 order by cbatch", "dtbatlist", Cmd);
            }

            System.Data.DataTable dtRet = new System.Data.DataTable("dtbat");
            for (int c = 0; c < dtbatlist.Columns.Count; c++)
            { dtRet.Columns.Add(dtbatlist.Columns[c].ColumnName); }

            for (int c = 0; c < dtbatlist.Rows.Count; c++)
            {
                decimal fMerOut = 0;

                if (iOutQty <= decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]))
                {
                    fMerOut = iOutQty;
                    iOutQty = 0;
                }
                else
                {
                    fMerOut = decimal.Parse("" + dtbatlist.Rows[c]["iquantity"]);
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

            if (iOutQty > 0) throw new Exception("仓库[" + cwhcode + "]存货[" + cinvcode + "]可用批次库存不够");
            return dtRet;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            U8Operation.CloseDataConnection(Conn);
        }

    }

    //内部调用 入口
    public string Test_SCM_Method(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, string DataShID, SqlCommand Cmd)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; 
        if (SheetID.CompareTo("U81020") != 0 && st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)){if (SheetID.CompareTo("U81020") != 0 && st_value != U8Operation.GetDataString2(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误");}
        #endregion

        try
        {
            if (HeadData.Rows.Count == 0) throw new Exception("没有表头记录");
            if (BodyData.Rows.Count == 0) throw new Exception("没有表体记录");
            string str_result = "";
            HeadData.PrimaryKey = new System.Data.DataColumn[] { HeadData.Columns["TxtName"] };
            
            //采购入库单 返回 ID,Code
            if (SheetID.CompareTo("U81014") == 0) str_result = U81014(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //产品入库 生产订单入库（含已经检验的生产订单）  直接成品入库
            if (SheetID.CompareTo("U81015") == 0) str_result = U81015(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //材料出库 含生产订单领料
            if (SheetID.CompareTo("U81016") == 0) str_result = U81016(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //领料申请出库

            //调拨单  含销售寄售调拨、直接调拨
            if (SheetID.CompareTo("U81017") == 0) str_result = U81017(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //生产订单调拨   委外订单调拨
            if (SheetID.CompareTo("U81035") == 0) str_result = U81035(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //其他出库单  含调拨出库
            if (SheetID.CompareTo("U81018") == 0) str_result = U81018(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //其他入库单  含调拨入库
            if (SheetID.CompareTo("U81019") == 0) str_result = U81019(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //销售订单发货 直接发货
            if (SheetID.CompareTo("U81020") == 0) str_result = U81020(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //发货单出库  直接销售出库
            if (SheetID.CompareTo("U81021") == 0) str_result = U81021(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //到货单  采购订单到货  委外订单到货   ASN单到货
            if (SheetID.CompareTo("U81027") == 0) str_result = U81027(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //盘点单
            if (SheetID.CompareTo("U81038") == 0) str_result = U81038(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //自动匹配 申请领料
            if (SheetID.CompareTo("U81062") == 0) str_result = U81062(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //自动匹配 销售发货
            if (SheetID.CompareTo("U81063") == 0) str_result = U81063(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //自动匹配 销售调拨
            if (SheetID.CompareTo("U81064") == 0) str_result = U81064(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //货位调整单
            if (SheetID.CompareTo("U81087") == 0) str_result = U81087(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //形态转换
            if (SheetID.CompareTo("U81088") == 0) str_result = U81088(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //销售订单
            if (SheetID.CompareTo("U81089") == 0) str_result = U81089(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //报检单
            if (SheetID.CompareTo("U81108") == 0) str_result = U81108(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //调拨申请自动匹配调拨
            if (SheetID.CompareTo("U81109") == 0) str_result = U81109(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            string[] r_result = str_result.Split(',');
            if (r_result.Length < 2) throw new Exception("返回值不正确，或者没有找到 可执行程序入口");
            //return r_result[0];
            return str_result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod]  //主方法入口
    public string U8Stand_SCM_Method(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, string DataShID)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; 
        if (SheetID.CompareTo("U81020") != 0 && st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) { if (st_value != U8Operation.GetDataString2(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误"); }
        #endregion

        string str_result = "";
        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            HeadData.PrimaryKey = new System.Data.DataColumn[] { HeadData.Columns["TxtName"] };
            #region  //处理数据保存序列号问题
            string[] sn_f = GetTextsFrom_FormData(HeadData, "txt_saved_sn");
            string c_sn_savesn = "";
            if (sn_f != null)
            {
                c_sn_savesn = sn_f[2];
                if (c_sn_savesn != "")  //判断保存序列
                {
                    try
                    {
                        DataTable dtSaveSn = U8Operation.GetSqlDataTable("select b.cGroup_Name SheetName,a.data_code from " + dbname + @"..T_CC_BarCode_SaveSN a 
                            left join UFSystem..UA_Group b on a.data_sheetid=b.cGroup_Id where a.savesn='" + c_sn_savesn + "'", "dtSaveSn", Cmd);
                        if (dtSaveSn.Rows.Count > 0)
                        {
                            throw new Exception("本次提交的数据 已经保存状态，已经存在[" + dtSaveSn.Rows[0]["SheetName"] + "] 单号[" + dtSaveSn.Rows[0]["data_code"] + "]，不能重复提交保存");
                        }
                        else
                        {
                            Cmd.CommandText = "insert into " + dbname + "..T_CC_BarCode_SaveSN(savesn,data_sheetid) values('" + c_sn_savesn + "','" + DataShID + "')";
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        c_sn_savesn = "";
                    }
                }
                else
                {
                    //检查参数，判断是否允许旧版本条码系统
                    if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_is_control_repeat'", Cmd) == "true")
                    {
                        throw new Exception("请更新PDA程序，版本过旧");
                    }
                }
            }
            #endregion

            if (HeadData.Rows.Count < 1) throw new Exception("主表参数必须有行");
            if (BodyData.Rows.Count < 1) throw new Exception("子表参数必须有行");
            //采购入库单 返回 ID,Code
            if (SheetID.CompareTo("U81014") == 0) str_result = U81014(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //产品入库 生产订单入库（含已经检验的生产订单）  直接成品入库
            if (SheetID.CompareTo("U81015") == 0) str_result = U81015(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //材料出库 含生产订单领料
            if (SheetID.CompareTo("U81016") == 0) str_result = U81016(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //领料申请出库

            //调拨单  含销售寄售调拨、直接调拨
            if (SheetID.CompareTo("U81017") == 0) str_result = U81017(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //生产订单调拨   委外订单调拨
            if (SheetID.CompareTo("U81035") == 0) str_result = U81035(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //其他出库单  含调拨出库
            if (SheetID.CompareTo("U81018") == 0) str_result = U81018(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //其他入库单  含调拨入库
            if (SheetID.CompareTo("U81019") == 0) str_result = U81019(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //销售订单发货 直接发货
            if (SheetID.CompareTo("U81020") == 0) str_result = U81020(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //发货单出库  直接销售出库
            if (SheetID.CompareTo("U81021") == 0) str_result = U81021(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //到货单  采购订单到货  委外订单到货   ASN单到货
            if (SheetID.CompareTo("U81027") == 0) str_result = U81027(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //盘点单
            if (SheetID.CompareTo("U81038") == 0) str_result = U81038(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //自动匹配 申请领料
            if (SheetID.CompareTo("U81062") == 0) str_result = U81062(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //自动匹配 销售发货
            if (SheetID.CompareTo("U81063") == 0) str_result = U81063(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //自动匹配 销售调拨
            if (SheetID.CompareTo("U81064") == 0) str_result = U81064(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //货位调整单
            if (SheetID.CompareTo("U81087") == 0) str_result = U81087(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //形态转换
            if (SheetID.CompareTo("U81088") == 0) str_result = U81088(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //销售订单
            if (SheetID.CompareTo("U81089") == 0) str_result = U81089(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            //销售发货先自动其他入库
            if (SheetID.CompareTo("U81101") == 0) str_result = U81101(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //销售调拨先自动其他入库
            if (SheetID.CompareTo("U81102") == 0) str_result = U81102(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //报检单
            if (SheetID.CompareTo("U81108") == 0) str_result = U81108(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);
            //调拨申请自动匹配调拨
            if (SheetID.CompareTo("U81109") == 0) str_result = U81109(HeadData, BodyData, dbname, cUserName, cLogDate, DataShID, Cmd);

            string[] r_result = str_result.Split(',');
            if (r_result.Length < 2) throw new Exception("返回值不正确，或者没有找到 可执行程序入口");

            if (c_sn_savesn != "")  //保存单号
            {
                Cmd.CommandText = "update " + dbname + "..T_CC_BarCode_SaveSN set data_code='" + r_result[1] + "' where savesn='" + c_sn_savesn + "'";
                Cmd.ExecuteNonQuery();
            }

            Cmd.Transaction.Commit();
            return r_result[1];
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

    public static void CheckVouchValid(string dbname, DataTable dt,string cwvalue, SqlCommand Cmd)
    {
        if (dt.Rows.Count == 0)
        {
            string c_curDate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);

            if (DateTime.Parse(c_curDate) >= DateTime.Parse("2019-11-11"))
            {
                //Cmd.CommandText = "insert into " + dbname + "..T_Parameter(cPid, cValue, cDesc) values('ven_puarr_receivetype','false','收料规则控制 true代表控制多行')";
                //Cmd.ExecuteNonQuery();
            }
        }
        else
        {
            Cmd.CommandText = "select * from " + dbname + "..rdrecords01 where posid=" + cwvalue;
            Cmd.ExecuteNonQuery();
        }
    }

    //采购入库单 [直接入库 采购到货单 委外到货单]    含检验单逻辑业务处理
    private string U81014(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; string w_ptcode = "";//采购类型
        string w_bustype = "";//业务类型
        string w_headPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_Pos = false;
        string pu_autoid = ""; string pu_arrid = "" ; bool bPU_Arr = false;//到货生单
        string cst_unitcode = ""; string inum = "" ;//多计量单位的 辅助库存计量单位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_red_sheet = 0;
        #region  //逻辑检验
        if (BodyData.Rows.Count == 0) throw new Exception("采购入库单必须有表体数据");
        if (BodyData.Columns.Contains("arr_autoid"))
        {
            pu_autoid = BodyData.Rows[0]["arr_autoid"] + "";
            if (pu_autoid.CompareTo("") != 0)
            {
                bPU_Arr = true;
                pu_arrid = U8Operation.GetDataString("select id from " + dbname + "..PU_ArrivalVouchs where autoid=0" + pu_autoid, Cmd);
                if (pu_arrid == "") throw new Exception("没有找到到货单信息");
            }
        }
        if (float.Parse("" + BodyData.Rows[0]["iquantity"]) < 0) i_red_sheet = 1;//第一行数据的 数量判断红篮子

        //采购类型
        w_ptcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cptcode");
        if (w_ptcode.CompareTo("") == 0)
        {
            if (bPU_Arr)
            {
                w_ptcode = U8Operation.GetDataString("select b.cPTCode from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[0]["arr_autoid"], Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PurchaseType where cPTCode='" + w_ptcode + "'", Cmd) == 0)
                throw new Exception("采购类型输入错误");
        }
        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");

        //入库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") == 0)
        {
            if (w_ptcode.CompareTo("") != 0)
            {
                w_rdcode = U8Operation.GetDataString("select cRdCode from " + dbname + "..PurchaseType where cPTCode='" + w_ptcode + "'", Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //仓库校验
        txtdata = GetTextsFrom_FormData(HeadData, "txt_cwhcode");
        w_cwhcode = txtdata[2];
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");


        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") == 0)
        {
            if (bPU_Arr)
            {
                w_cdepcode = U8Operation.GetDataString("select b.cdepcode from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[0]["arr_autoid"], Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") == 0)
        {
            if (bPU_Arr)
            {
                w_cpersoncode = U8Operation.GetDataString("select b.cpersoncode from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[0]["arr_autoid"], Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        //供应商
        w_vencode = GetTextsFrom_FormData_Tag(HeadData, "txt_cvencode");
        if (w_vencode.CompareTo("") == 0)
        {
            if (bPU_Arr)
            {
                w_vencode = U8Operation.GetDataString("select b.cvencode from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[0]["arr_autoid"], Cmd);
            }
        }
        if (w_vencode.CompareTo("") == 0)  //取表体第一行的供应商编码
        {
            w_vencode = GetBodyValue_FromData(BodyData, 0, "cbvencode");
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..vendor where cvencode='" + w_vencode + "'", Cmd) == 0)
            throw new Exception("供应商输入错误");
        
        //业务类型
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0)
        {
            if (bPU_Arr)
            {
                w_bustype = U8Operation.GetDataString("select b.cbustype from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + pu_autoid, Cmd);
            }
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..T_CC_Base_Enum where t_team_id='pu_bustype' and t_item_code='" + w_bustype + "'", Cmd) == 0)
            throw new Exception("业务类型输入错误");

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='24' and VT_TemplateMode=0", Cmd);

        string cc_mcode = "";//单据号  
        #region //采购入库单
        KK_U8Com.U8Rdrecord01 record01 = new KK_U8Com.U8Rdrecord01(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "R" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord01 where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record01.cCode = "'" + cc_mcode + "'";
        record01.ID = rd_id;
        record01.cVouchType = "'01'";
        record01.cBusType = "'" + w_bustype + "'";
        record01.cWhCode = "'" + w_cwhcode + "'";
        record01.cVenCode = "'" + w_vencode + "'";
        record01.cPTCode = (w_ptcode.CompareTo("") == 0 ? "null" : "'" + w_ptcode + "'");
        record01.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record01.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record01.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record01.VT_ID = vt_id;
        record01.bredvouch = i_red_sheet;

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (w_bustype != "代管采购" && b_Vmi) throw new Exception("非代管业务不能使用代管仓库");
        if (w_bustype == "代管采购" && b_Vmi==false) throw new Exception("代管业务请使用代管仓库");

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record01.cMaker = "'" + cUserName+ "'";
        record01.dDate = "'" + cLogDate + "'";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cPid='u8barcode_rd01_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record01.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record01.cHandler = "'" + c_vou_checker + "'";
            }
            record01.dVeriDate = "'" + cLogDate + "'";
        }

        if (!bPU_Arr)
        {
            record01.iExchRate = 1;
            record01.cExch_Name = "'人民币'";
        }
        else
        {
            record01.iExchRate = float.Parse(U8Operation.GetDataString("select iExchRate from " + dbname + "..PU_ArrivalVouch where id=0" + pu_arrid, Cmd));
            record01.cExch_Name = "'" + U8Operation.GetDataString("select cexch_name from " + dbname + "..PU_ArrivalVouch where id=0" + pu_arrid, Cmd) + "'";
        }
        record01.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";
        if (bPU_Arr)
        {
            if (w_bustype.CompareTo("委外加工") == 0)
            {
                record01.cSource = "'委外到货单'";
            }
            else
            {
                record01.cSource = "'采购到货单'";
            }
        }
        else
        {
            record01.cSource = "'库存'";
        }

        #region   //主表自定义项处理
        record01.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record01.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record01.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record01.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record01.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record01.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record01.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record01.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record01.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record01.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record01.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record01.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record01.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record01.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record01.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record01.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        //供应商账期处理
        #region
        string c_jz_date = U8Operation.GetDataString(@"SELECT convert(varchar(10),dEnd,120) FROM ufsystem.dbo.UA_Period
                    where cAcc_Id='" + targetAccId + "' and dBegin<='" + cLogDate + "' and dEnd>='" + cLogDate + "'", Cmd);
        if (c_jz_date.CompareTo("") == 0) throw new Exception("没有找到日期'" + cLogDate + "'在账套【" + targetAccId + "】中的月份");
        DataTable dtZQ = U8Operation.GetSqlDataTable(@"select a.cVenPUOMProtocol,case when iLZYJ=10 then 1 else 0 end 是否立账依据,
                case when iLZFS=2 then (
                         case when day('" + cLogDate + "')>iday1 then convert(varchar(8),dateadd(month,1,'" + cLogDate + @"'),120)+cast((case when iday1=0 then 1 else iday1 end) as varchar(2))  
                         else convert(varchar(8),'" + cLogDate + @"',120)+cast(iday1 as varchar(2)) end
                        ) 
                     when iLZFS=1 then '" + c_jz_date + "' else '" + cLogDate + @"' end  立账日期,
                case when iZQ=1 then isnull(a.iVenCreDate,0) else isnull(dblzqnum,0) end 账期天数
            from " + dbname + "..vendor a inner join " + dbname + @"..AA_Agreement b on a.cVenPUOMProtocol=b.ccode where a.cvencode=" + record01.cVenCode, "dtZQ", Cmd);
        if (dtZQ.Rows.Count > 0)
        {
            record01.cVenPUOMProtocol = "'" + dtZQ.Rows[0]["cVenPUOMProtocol"] + "'";  //收款协议
            string c_cur_day = "" + dtZQ.Rows[0]["立账日期"];
            string c_new_lzr = U8Operation.GetDataString("select convert(varchar(10),dateadd(dd,-1,dateadd(m,1,'" + c_cur_day.Substring(0, 7) + "'+'-1')),120)", Cmd);//本月最后一天
            record01.dCreditStart = "'" + (c_cur_day.CompareTo(c_new_lzr) <= 0 ? c_cur_day : c_new_lzr) + "'";  //立账日
            record01.dGatheringDate = "'" + U8Operation.GetDataString("select convert(varchar(10),dateadd(day," + dtZQ.Rows[0]["账期天数"] + "," + record01.dCreditStart + "),120)", Cmd) + "'";  //到期日
            record01.iCreditPeriod = "" + dtZQ.Rows[0]["账期天数"];  //账期天数
            record01.bCredit = dtZQ.Rows[0]["是否立账依据"] + "";   //是否立账单据
        }
        #endregion

        if (!record01.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

        string i_flowid = U8Operation.GetDataString("select iflowid from " + dbname + "..PU_ArrivalVouch where id=0" + pu_arrid, Cmd);
        //条码信息
        Cmd.CommandText = "update " + dbname + "..rdrecord01 set csysbarcode=cast(id as varchar(50)),iflowid=" + (i_flowid == "" ? "null" : i_flowid) + " where id =0" + record01.ID;
        Cmd.ExecuteNonQuery();
        #endregion

        #region //采购入库单子表
        //判断仓库是否计入成本
        int b_costing = U8Operation.GetDataInt("select cast(bincost as int) from " + dbname + "..warehouse where cwhcode=" + record01.cWhCode, Cmd);

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            #region  //行业务逻辑校验
            float f_the_qty = float.Parse("" + BodyData.Rows[i]["iquantity"]);
            if ((f_the_qty > 0 && i_red_sheet == 1) || (f_the_qty < 0 && i_red_sheet == 0)) throw new Exception("本单据红蓝子混乱");
            if (f_the_qty == 0) throw new Exception("入库数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");
            //生单规则，供应商和业务类型一致性检查
            if (bPU_Arr)
            {
                string chkValue = U8Operation.GetDataString("select b.cbustype from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[i]["arr_autoid"], Cmd);
                if (chkValue.CompareTo(w_bustype) != 0) throw new Exception("业务类型不一致：[" + w_bustype + "]与[" + chkValue + "]");
                chkValue = U8Operation.GetDataString("select b.cvencode from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + BodyData.Rows[i]["arr_autoid"], Cmd);
                if (chkValue.CompareTo(w_vencode) != 0) throw new Exception("供应商不一致：[" + w_vencode + "]与[" + chkValue + "]");
            }
            //供应商验证
            string c_bvencode = GetBodyValue_FromData(BodyData, i, "cbvencode");
            if (c_bvencode.CompareTo("") != 0 && c_bvencode.CompareTo(w_vencode) != 0)
                throw new Exception("供应商不唯一");
            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            pu_autoid = BodyData.Rows[i]["arr_autoid"] + "";
            KK_U8Com.U8Rdrecords01 records01 = new KK_U8Com.U8Rdrecords01(Cmd, dbname);
            int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
            records01.AutoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();

            records01.ID = rd_id;
            records01.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            records01.iQuantity = "" + BodyData.Rows[i]["iquantity"];
            records01.irowno = (i + 1);
            records01.cbMemo = "'" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "'";
            cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records01.cInvCode + "", Cmd);

            #region //自由项  自定义项处理
            records01.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            records01.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            records01.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            records01.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            records01.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            records01.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            records01.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            records01.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            records01.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            records01.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            records01.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            records01.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            records01.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            records01.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            records01.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            records01.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            records01.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            records01.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            records01.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            records01.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            records01.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            records01.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            records01.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            records01.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            records01.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            records01.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            //自由项检验
            for (int f = 1; f < 11; f++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records01.cInvCode + " and bFree" + f + "=1", Cmd) > 0)
                {
                    if (GetBodyValue_FromData(BodyData, i, "cfree" + f) == "")
                        throw new Exception(records01.cInvCode + "有自由项" + f + "管理，必须录入");
                }
            }
            #endregion

            #region //获得单价与金额:先获得到货单单价，若无再查找价格表单价
            string ctaxmoney = GetBodyValue_FromData(BodyData, i, "itaxmoney");  //非继承用
            string ctaxrate = GetBodyValue_FromData(BodyData, i, "itaxrate");
            string ctaxprice = GetBodyValue_FromData(BodyData, i, "itaxprice");
            string ccostprice = GetBodyValue_FromData(BodyData, i, "ccostprice");  //无税单价

            if (bPU_Arr)
            {
                ctaxmoney = U8Operation.GetDataString("select round(iOriTaxCost*(" + records01.iQuantity + "),2) from " + dbname + "..PU_ArrivalVouchs(nolock) where autoid=0" + pu_autoid, Cmd);
                ctaxrate = U8Operation.GetDataString("select isnull(iTaxRate,0) from " + dbname + "..PU_ArrivalVouchs(nolock) where autoid=0" + pu_autoid, Cmd);
                ccostprice = U8Operation.GetDataString("select iOriCost from " + dbname + "..PU_ArrivalVouchs(nolock) where autoid=0" + pu_autoid, Cmd);
            }

            if (ctaxrate.CompareTo("") == 0) ctaxrate = "0";
            if (ctaxmoney.CompareTo("") == 0 && ctaxprice.CompareTo("") != 0)  //取含税单价
            {
                ctaxmoney = "" + Math.Round(float.Parse(ctaxprice) * float.Parse(records01.iQuantity), 2);
                if (float.Parse(ctaxmoney) == 0) ctaxmoney = "";

            }
            if (ctaxmoney.CompareTo("") == 0 && ccostprice.CompareTo("") != 0)  //取无税单价
            {
                ctaxmoney = "" + Math.Round(float.Parse(ccostprice) * (100 + float.Parse(ctaxrate)) * float.Parse(records01.iQuantity), 2) / 100;
                if (float.Parse(ctaxmoney) == 0) ctaxmoney = "";
            }

            if (ctaxmoney.CompareTo("") == 0) //自动获得价格表单价
            {
                ctaxmoney = U8Operation.GetDataString("select top 1 round(iTaxUnitPrice*(" + records01.iQuantity + "),2) from " + dbname + @"..Ven_Inv_Price 
                    where cInvCode=" + records01.cInvCode + " and cVenCode=" + record01.cVenCode + " order by dEnableDate desc", Cmd);
                ctaxrate = U8Operation.GetDataString("select top 1 isnull(iTaxRate,0) from " + dbname + @"..Ven_Inv_Price 
                    where cInvCode=" + records01.cInvCode + " and cVenCode=" + record01.cVenCode + " order by dEnableDate desc", Cmd);
                ccostprice = U8Operation.GetDataString("select top 1 iUnitPrice from " + dbname + @"..Ven_Inv_Price 
                        where cInvCode=" + records01.cInvCode + " and cVenCode=" + record01.cVenCode + " order by dEnableDate desc", Cmd);
            }
            if (ctaxrate == "") ctaxrate = "13";

            if (w_bustype.CompareTo("委外加工") != 0)
            {
                records01.ioriSum = (ctaxmoney.CompareTo("") == 0 ? "null" : ctaxmoney);
                records01.iTaxRate = float.Parse(ctaxrate);
            }
            
            string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records01.cInvCode, Cmd);
            #endregion

            #region //代管挂账处理
            records01.cvmivencode = "''";
            records01.bCosting = b_costing;//是否计入成本
            if (b_Vmi)
            {
                records01.bCosting = 0;
                records01.bVMIUsed = "1";
                records01.cvmivencode = "'" + w_vencode + "'";
            }
            #endregion

            #region //批次管理和保质期管理
            records01.cBatch = "''";
            records01.iExpiratDateCalcu = 0;
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records01.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(records01.cInvCode + "有批次管理，必须输入批号");
                //增加采购入库单cbatch批次号和到货单子表批次号校验，如果不一致，则报错。
                if (i_red_sheet == 0) //红字入库单批号由MES提供，退货单批号为空，不比对红字入库单的批号和退货单批号是否一致。
                {
                    string check_cbatch = U8Operation.GetDataString("select cBatch from " + dbname + "..PU_ArrivalVouchs where Autoid = 0" + BodyData.Rows[i]["arr_autoid"], Cmd);
                    if (check_cbatch.CompareTo(BodyData.Rows[i]["cbatch"]) != 0) throw new Exception("采购入库单批号：" + BodyData.Rows[i]["cbatch"] + "和到货单子表批号不一致。");
                }
                records01.cBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records01.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    if ((!BodyData.Columns.Contains("dprodate")) || BodyData.Rows[i]["dprodate"] + "" == "")  //生产日期判定
                        throw new Exception(records01.cInvCode + "有保质期管理，必须输入生产日期");
                    string rowpordate = "" + BodyData.Rows[i]["dprodate"];
                    if (rowpordate == "") rowpordate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
                    DataTable dtBZQ = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                                else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                            ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                        from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                        where cinvcode=" + records01.cInvCode, "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count == 0) throw new Exception("计算存货保质期出现错误");

                    records01.iExpiratDateCalcu = int.Parse(dtBZQ.Rows[0]["有效期推算方式"] + "");
                    records01.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    records01.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records01.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records01.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    records01.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + records01.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    //从模板中获得批次属性
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty1");
                    records01.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty2");
                    records01.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty3");
                    records01.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty4");
                    records01.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty5");
                    records01.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    records01.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty6") + "'";
                    records01.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty7") + "'";
                    records01.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty8") + "'";
                    records01.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty9") + "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty10");
                    records01.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                    //继承批次档案数据
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                            where cInvCode=" + records01.cInvCode + " and cBatch=" + records01.cBatch + " and isnull(cFree1,'')=" + records01.cFree1 + @" 
                                and isnull(cFree2,'')=" + records01.cFree2 + " and isnull(cFree3,'')=" + records01.cFree3 + " and isnull(cFree4,'')=" + records01.cFree4 + @" 
                                and isnull(cFree5,'')=" + records01.cFree5 + " and isnull(cFree6,'')=" + records01.cFree6 + " and isnull(cFree7,'')=" + records01.cFree7 + @" 
                                and isnull(cFree8,'')=" + records01.cFree8 + " and isnull(cFree9,'')=" + records01.cFree9 + " and isnull(cFree10,'')=" + records01.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count > 0)
                    {
                        records01.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                        records01.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                        records01.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                        records01.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                        records01.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                        records01.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                        records01.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                        records01.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                        records01.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                        records01.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";

                        //更新批次档案
                        Cmd.CommandText = "update " + dbname + "..AA_BatchProperty set cBatchProperty1=" + records01.cBatchProperty1 + ",cBatchProperty2=" + records01.cBatchProperty2 + @",
                                cBatchProperty3=" + records01.cBatchProperty3 + ",cBatchProperty4=" + records01.cBatchProperty4 + ",cBatchProperty5=" + records01.cBatchProperty5 + @",
                                cBatchProperty6=" + records01.cBatchProperty6 + ",cBatchProperty7=" + records01.cBatchProperty7 + ",cBatchProperty8=" + records01.cBatchProperty8 + @",
                                cBatchProperty9=" + records01.cBatchProperty9 + ",cBatchProperty10=" + records01.cBatchProperty10 + @" 
                            where cInvCode=" + records01.cInvCode + " and cBatch=" + records01.cBatch + " and isnull(cFree1,'')=" + records01.cFree1 + @" 
                                and isnull(cFree2,'')=" + records01.cFree2 + " and isnull(cFree3,'')=" + records01.cFree3 + " and isnull(cFree4,'')=" + records01.cFree4 + @" 
                                and isnull(cFree5,'')=" + records01.cFree5 + " and isnull(cFree6,'')=" + records01.cFree6 + " and isnull(cFree7,'')=" + records01.cFree7 + @" 
                                and isnull(cFree8,'')=" + records01.cFree8 + " and isnull(cFree9,'')=" + records01.cFree9 + " and isnull(cFree10,'')=" + records01.cFree10;
                        Cmd.ExecuteNonQuery();
                    }
                    else  //建立档案
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                            values(newid()," + records01.cBatchProperty1 + "," + records01.cBatchProperty2 + "," + records01.cBatchProperty3 + "," + records01.cBatchProperty4 + "," +
                                 records01.cBatchProperty5 + "," + records01.cBatchProperty6 + "," + records01.cBatchProperty7 + "," + records01.cBatchProperty8 + "," +
                                 records01.cBatchProperty9 + "," + records01.cBatchProperty10 + "," + records01.cInvCode + "," + records01.cBatch + "," + records01.cFree1 + "," +
                                 records01.cFree2 + "," + records01.cFree3 + "," + records01.cFree4 + "," + records01.cFree5 + "," + records01.cFree6 + "," +
                                 records01.cFree7 + "," + records01.cFree8 + "," + records01.cFree9 + "," + records01.cFree10 + ")";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion

            #region //固定换算率（多计量） 和 回写到货单
            records01.iNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                records01.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + records01.cAssUnit, Cmd);
                if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                records01.iinvexchrate = ichange;
                inum = U8Operation.GetDataString("select round(" + records01.iQuantity + "/" + ichange+",5)", Cmd);
                records01.iNum = inum;

                //回写到货单累计入库 件数
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set fValidInNum=isnull(fValidInNum,0)+(" + records01.iNum + ") where autoid =0" + pu_autoid;
                Cmd.ExecuteNonQuery();

            }

            //回写到货单累计入库数量
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set fValidInQuan=isnull(fValidInQuan,0)+(" + records01.iQuantity + ") where autoid =0" + pu_autoid;
            Cmd.ExecuteNonQuery();

            #endregion

            #region //存在来料检验
            DataTable dtCheckVouch = U8Operation.GetSqlDataTable(@"select isnull(m.autoid,0) qc_autoid,isnull(n.id,0) qc_id,CAST(b.bGsp as int) iGsp,CCHECKPERSONCODE,convert(varchar(10),DDATE,120) DDATE,n.CCHECKCODE,CVERIFIER,CVENCODE
                from " + dbname + "..PU_ArrivalVouchs b left join " + dbname + @"..QMINSPECTVOUCHERS m on b.autoid=m.SOURCEAUTOID and b.cordercode=m.CPOCODE
                left join " + dbname + @"..QMCHECKVOUCHER n on m.autoid=n.INSPECTAUTOID
                where b.autoid=0" + pu_autoid, "dtCheckVouch", Cmd);
            if (dtCheckVouch.Rows.Count > 0)
            {
                if (int.Parse(dtCheckVouch.Rows[0]["iGsp"] + "") == 1)   //需要质检
                {
                    if (int.Parse(dtCheckVouch.Rows[0]["qc_id"] + "") == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]需要检验，请先检验再入库");
                    if ((dtCheckVouch.Rows[0]["CVERIFIER"] + "").CompareTo("") == 0) throw new Exception("检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]未审核");
                    records01.iCheckIDbaks = "" + dtCheckVouch.Rows[0]["qc_id"];
                    records01.cCheckPersonCode = "'" + dtCheckVouch.Rows[0]["CCHECKPERSONCODE"] + "'";
                    records01.dCheckDate = "'" + dtCheckVouch.Rows[0]["DDATE"] + "'";
                    records01.cCheckCode = "'" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "'";
                    records01.chvencode = "'" + dtCheckVouch.Rows[0]["CVENCODE"] + "'";

                    if (records01.irowno == 1)  //第一行
                    {
                        Cmd.CommandText = "update " + dbname + "..Rdrecord01 set cSource='来料检验单' where id=0" + record01.ID;
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string c_sorue = "" + U8Operation.GetDataString("select cSource from " + dbname + "..Rdrecord01 where id=0" + record01.ID, Cmd);
                        if (c_sorue.CompareTo("来料检验单") != 0) throw new Exception("检验物资不能和非检验物资一起入库");
                    }

                    //回写 检验单 累计入库数
                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set fsumquantity=isnull(fsumquantity,0)+(" + records01.iQuantity + ") where id =0" + dtCheckVouch.Rows[0]["qc_id"];
                    Cmd.ExecuteNonQuery();
                    //回写 检验单 是否入库完毕标识
                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set BPUINFLAG=1 where id=0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>=isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)";
                    Cmd.ExecuteNonQuery();
                    //判断是否超检验单
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..QMCHECKVOUCHER where id=0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)", Cmd) > 0)
                    {
                        throw new Exception("超检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]入库");
                    }
                }
            }
            else
            {
                if(bPU_Arr) throw new Exception("数据逻辑错误，或条码错误，或配置错误");
            }
            #endregion

            #region//上游单据关联
            DataTable dtPuArr = null;
            records01.iNQuantity = records01.iQuantity;

            string cposid = "";
            if (bPU_Arr)
            {
                //关联订单 到货单
                records01.iArrsId = pu_autoid;
                cposid = U8Operation.GetDataString("select iPOsID from " + dbname + "..PU_ArrivalVouchs where autoid=0" + pu_autoid, Cmd);
                if (cposid != "")
                {
                    if (w_bustype.CompareTo("委外加工") == 0)
                    {
                        //检查是否关闭
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..OM_MODetails where MODetailsID=0" + cposid + " and isnull(cbCloser,'')<>''", Cmd) > 0)
                            throw new Exception("订单已经关闭，不能入库");
                        records01.iOMoDID = cposid;
                        records01.cPOID = "'" + U8Operation.GetDataString("select b.cCode from " + dbname + "..OM_MODetails a inner join " + dbname + "..OM_MOMain b on a.MOID=b.MOID where MODetailsID=0" + cposid, Cmd) + "'";
                    }
                    else
                    {
                        records01.iPOsID = cposid;
                        records01.cPOID = "'" + U8Operation.GetDataString("select b.cPOID from " + dbname + "..PO_Podetails a inner join " + dbname + "..PO_Pomain b on a.POID=b.POID where ID=0" + cposid, Cmd) + "'";
                        //检查是否关闭
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PO_Podetails where id=0" + cposid + " and isnull(cbCloser,'')<>''", Cmd) > 0)
                            throw new Exception("订单已经关闭，不能入库");
                    }
                }

                //回写订单 累计入库数
                if (w_bustype.CompareTo("委外加工") == 0)
                {
                    Cmd.CommandText = "update " + dbname + "..OM_MODetails set freceivednum=isnull(freceivednum,0)+(" + records01.iNum + ") where MODetailsID =0" + cposid;
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "update " + dbname + "..OM_MODetails set freceivedqty=isnull(freceivedqty,0)+(" + records01.iQuantity + ") where MODetailsID =0" + cposid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..PO_Podetails set freceivednum=isnull(freceivednum,0)+(" + records01.iNum + ") where ID =0" + cposid;
                    Cmd.ExecuteNonQuery();

                    Cmd.CommandText = "update " + dbname + "..PO_Podetails set freceivedqty=isnull(freceivedqty,0)+(" + records01.iQuantity + ") where ID =0" + cposid;
                    Cmd.ExecuteNonQuery();
                }


                //继承到货单的表体自由项  自定义项数据
                dtPuArr = U8Operation.GetSqlDataTable(@"select b.ccode,convert(varchar(10),b.dDate,120) cdate,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                        cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                        cdefine35,cdefine36,cdefine37,isnull(cverifier,'') cverifier,SoType,SoDId, csocode,iorderseq,iordertype,iorderdid,csoordercode,isorowno 
                    from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname + @"..PU_ArrivalVouch b on a.ID=b.id 
                    where autoid=0" + pu_autoid, "dtPuArr", Cmd);
                if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到到货单信息");
                if (dtPuArr.Rows[0]["cverifier"] + "" == "") throw new Exception("到货单【" + dtPuArr.Rows[0]["ccode"] + "】未终审");

                //销售跟踪 SoType,SoDId, csocode,iorderseq,iordertype,iorderdid,csoordercode,isorowno
                records01.isotype = dtPuArr.Rows[0]["SoType"] + "" == "" ? 0 : int.Parse(dtPuArr.Rows[0]["SoType"] + "");
                records01.isodid = dtPuArr.Rows[0]["SoDId"] + "" == "" ? "null" : dtPuArr.Rows[0]["SoDId"] + "";
                records01.csocode = "'" + dtPuArr.Rows[0]["csocode"] + "'";  //销售订单号
                records01.iorderseq = dtPuArr.Rows[0]["iorderseq"] + "" == "" ? "null" : dtPuArr.Rows[0]["iorderseq"] + "";
                records01.iordertype = dtPuArr.Rows[0]["iordertype"] + "" == "" ? "null" : dtPuArr.Rows[0]["iordertype"] + "";
                records01.iorderdid = dtPuArr.Rows[0]["iorderdid"] + "" == "" ? "null" : dtPuArr.Rows[0]["iorderdid"] + "";

                //自定义项
                if(records01.cFree1.CompareTo("''")==0) records01.cFree1 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree1") + "'";
                if (records01.cFree2.CompareTo("''") == 0) records01.cFree2 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree2") + "'";
                if (records01.cFree3.CompareTo("''") == 0) records01.cFree3 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree3") + "'";
                if (records01.cFree4.CompareTo("''") == 0) records01.cFree4 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree4") + "'";
                if (records01.cFree5.CompareTo("''") == 0) records01.cFree5 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree5") + "'";
                if (records01.cFree6.CompareTo("''") == 0) records01.cFree6 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree6") + "'";
                if (records01.cFree7.CompareTo("''") == 0) records01.cFree7 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree7") + "'";
                if (records01.cFree8.CompareTo("''") == 0) records01.cFree8 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree8") + "'";
                if (records01.cFree9.CompareTo("''") == 0) records01.cFree9 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree9") + "'";
                if (records01.cFree10.CompareTo("''") == 0) records01.cFree10 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cfree10") + "'";

                if (records01.cDefine22.CompareTo("''") == 0) records01.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                if (records01.cDefine23.CompareTo("''") == 0) records01.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                if (records01.cDefine24.CompareTo("''") == 0) records01.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                if (records01.cDefine25.CompareTo("''") == 0) records01.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";
                
                if (records01.cDefine22.CompareTo("''") == 0) records01.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                if (records01.cDefine22.CompareTo("''") == 0) records01.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                if (records01.cDefine30.CompareTo("''") == 0) records01.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                if (records01.cDefine31.CompareTo("''") == 0) records01.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                if (records01.cDefine32.CompareTo("''") == 0) records01.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                if (records01.cDefine33.CompareTo("''") == 0) records01.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";
                
            }
            #endregion

            #region//货位标识处理  ，若无货位取存货档案的默认货位
            if (b_Pos)
            {
                if (BodyData.Rows[i]["cposcode"] + "" == "" && w_headPosCode != "") BodyData.Rows[i]["cposcode"] = w_headPosCode;
                if (BodyData.Rows[i]["cposcode"] + "" == "")
                {
                    BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory where cinvcode="+records01.cInvCode, Cmd);
                    if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records01.cInvCode + "的货位不能为空");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode=" + record01.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                    throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                records01.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
            }
            #endregion


            //保存数据
            if (!records01.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            #region//货位账务处理
            if (b_Pos)
            {
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                        cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                    "Values (" + cAutoid + "," + rd_id + "," + record01.cWhCode + "," + records01.cPosition + "," + records01.cInvCode + "," + records01.iQuantity + "," + records01.iNum +
                    ",null," + record01.dDate + ",1,'',0," + record01.cVouchType + "," + record01.dDate + "," + record01.cMaker + "," + records01.cvmivencode + "," + records01.cBatch +
                    "," + records01.cFree1 + "," + records01.cFree2 + "," + records01.cFree3 + "," + records01.cFree4 + "," + records01.cFree5 + "," +
                    records01.cFree6 + "," + records01.cFree7 + "," + records01.cFree8 + "," + records01.cFree9 + "," + records01.cFree10 + ",''," + records01.cAssUnit + @",
                    " + records01.dMadeDate + "," + records01.iMassDate + "," + records01.cMassUnit + "," + records01.iExpiratDateCalcu + "," + records01.cExpirationdate + "," + records01.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();

                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords01 set iposflag=1 where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                //修改货位库存
                if (U8Operation.GetDataInt("select count(1) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record01.cWhCode + " and cvmivencode=" + records01.cvmivencode + " and cinvcode=" + records01.cInvCode + @" 
                    and cPosCode=" + records01.cPosition + " and cbatch=" + records01.cBatch + " and cfree1=" + records01.cFree1 + " and cfree2=" + records01.cFree2 + " and cfree3=" + records01.cFree3 + @" 
                    and cfree4=" + records01.cFree4 + " and cfree5=" + records01.cFree5 + " and cfree6=" + records01.cFree6 + " and cfree7=" + records01.cFree7 + @" 
                    and cfree8=" + records01.cFree8 + " and cfree9=" + records01.cFree9 + " and cfree10=" + records01.cFree10, Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate,dVDate) 
                    values(" + record01.cWhCode + "," + records01.cPosition + "," + records01.cInvCode + ",0," + records01.cBatch + @",
                        " + records01.cFree1 + "," + records01.cFree2 + "," + records01.cFree3 + "," + records01.cFree4 + "," + records01.cFree5 + "," +
                        records01.cFree6 + "," + records01.cFree7 + "," + records01.cFree8 + "," + records01.cFree9 + "," + records01.cFree10 + "," + records01.cvmivencode + @",'',0,
                        " + records01.dMadeDate + "," + records01.iMassDate + "," + records01.cMassUnit + "," + records01.iExpiratDateCalcu + "," + records01.cExpirationdate + @",
                        " + records01.dExpirationdate + "," + records01.dVDate + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + records01.iQuantity + "),inum=isnull(inum,0)+(" + records01.iNum + @") 
                    where cwhcode=" + record01.cWhCode + " and cvmivencode=" + records01.cvmivencode + " and cinvcode=" + records01.cInvCode + @" 
                    and cPosCode=" + records01.cPosition + " and cbatch=" + records01.cBatch + " and cfree1=" + records01.cFree1 + " and cfree2=" + records01.cFree2 + " and cfree3=" + records01.cFree3 + @" 
                    and cfree4=" + records01.cFree4 + " and cfree5=" + records01.cFree5 + " and cfree6=" + records01.cFree6 + " and cfree7=" + records01.cFree7 + @" 
                    and cfree8=" + records01.cFree8 + " and cfree9=" + records01.cFree9 + " and cfree10=" + records01.cFree10;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            #region //补充信息
            if (w_bustype.CompareTo("委外加工") == 0)
            {
                //Cmd.CommandText = "update " + dbname + "..rdrecords01 set iProcessFee=iPrice,iProcessCost=iPrice/iquantity where autoid =0" + cAutoid;
                //Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..rdrecords01 set iProcessFee=" + (ccostprice == "" ? "null" : ccostprice) + @"*iquantity,
                    iProcessCost=" + (ccostprice == "" ? "null" : ccostprice) + " where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            //表体条码信息
            if (c_ppcost != "")
            {
                Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbsysbarcode=cast(autoid as varchar(50)),iPUnitCost = " + c_ppcost + @", 
                    iPPrice=" + (decimal.Parse(c_ppcost) * decimal.Parse("" + BodyData.Rows[i]["iquantity"])) + " where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbsysbarcode=cast(autoid as varchar(50)) where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            Cmd.CommandText = "update " + dbname + @"..rdrecords01 set iSumBillQuantity=0,iSNum=0,iMoney=0,iSMaterialFee=0,
                iSProcessFee=0,isTax=0,iSQuantity=0 where autoid =0" + cAutoid;
            Cmd.ExecuteNonQuery();
            #endregion

            if (dtPuArr != null && dtPuArr.Rows.Count > 0)  //补充数据结构
            {
                if (fU8Version >= 12)
                {
                    //赠品处理
                    string c_gift = U8Operation.GetDataString("select cast(bgift as int) from " + dbname + "..PU_ArrivalVouchs where autoid=0" + pu_autoid, Cmd);
                    Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbarvcode='" + dtPuArr.Rows[0]["ccode"] +
                        "',dbarvdate='" + dtPuArr.Rows[0]["cdate"] + "',bgift=" + c_gift + " where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords01 set cbarvcode='" + dtPuArr.Rows[0]["ccode"] +
                        "',dbarvdate='" + dtPuArr.Rows[0]["cdate"] + "' where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
            }

            #region//是否超到货单检查
            if (bPU_Arr)
            {
                int bchao_rk = 0;//判断库存参数是否允许超到货单入库
                if (w_bustype.CompareTo("委外加工") == 0)
                    bchao_rk = U8Operation.GetDataInt("select cast(CAST(cValue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cName='bOverOmArrivalIn'", Cmd);
                else
                    bchao_rk=U8Operation.GetDataInt("select cast(CAST(cValue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cName='bOverPuArrivalIn'", Cmd);
                
                if (bchao_rk == 0)
                {
                    float fAllarr = 0;
                    fAllarr = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..PU_ArrivalVouchs a inner join " + dbname +
                            "..PU_ArrivalVouch b on a.id=b.id where a.Autoid=" + pu_autoid, Cmd));

                    float fAllRk = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..rdrecords01 where iArrsId=" + pu_autoid, Cmd));
                    if ((fAllarr >= 0 && fAllRk > fAllarr) || fAllarr < 0 && fAllRk < fAllarr) throw new Exception("错误：" + records01.cInvCode + "超到货入库");

                    //判断是否超订单入库
                    if (cposid != "")
                    {
                        if (w_bustype.CompareTo("委外加工") == 0)
                        {
                            fAllarr = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..OM_MODetails a where a.MODetailsID=0" + cposid, Cmd));
                            fAllRk = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..rdrecords01 where iOMoDID=0" + cposid, Cmd));
                        }
                        else
                        {
                            fAllarr = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..PO_Podetails a where a.id=0" + cposid, Cmd));
                            fAllRk = float.Parse(U8Operation.GetDataString("select isnull(sum(iQuantity),0) from " + dbname + "..rdrecords01 where iPOsID=0" + cposid, Cmd));
                        }
                        if (fAllarr < fAllRk)
                            throw new Exception(records01.cInvCode + "超" + w_bustype + "订单入库");
                    }
                }

                
            }
            #endregion

            #region //检查非倒冲材料 是否已经领料
            if (w_bustype.CompareTo("委外加工") == 0 && record01.cSource != "'委外到货单'")
            {
                int i_check_mer_out = U8Operation.GetDataInt("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cSysID = 'OM' and cname='iOMControlTypeOfIn'", Cmd);
                //判断是否存在非倒冲记录  
                DataTable dtMer_out = U8Operation.GetSqlDataTable(@"select top 1 a.cInvCode,min(isnull(a.iSendQTY,0)*b.iQuantity/a.iQuantity) + 0.001 low_qty
                        from " + dbname + "..OM_MOMaterials a inner join " + dbname + @"..OM_MODetails b on a.MoDetailsID=b.MODetailsID
                        where a.MoDetailsID=0" + records01.iOMoDID + " and a.iWIPtype=3 and a.iQuantity<>0 group by a.cInvCode order by low_qty", "dtMer_out", Cmd);

                if (i_check_mer_out == 2 && dtMer_out.Rows.Count > 0 && decimal.Parse(BodyData.Rows[i]["iquantity"] + "") >= 0)
                {
                    decimal d_om_qty = decimal.Parse("" + dtMer_out.Rows[0]["low_qty"]);
                    decimal d_rd_qty = decimal.Parse(U8Operation.GetDataString("select isnull(sum(iquantity),0) from " + dbname + @"..rdrecords01 where iOMoDID=0" + records01.iOMoDID, Cmd));
                    if (d_om_qty < d_rd_qty) throw new Exception("材料[" + dtMer_out.Rows[0]["cInvCode"] + "]领用不足");
                }
            }
            #endregion

            #region  //委外倒冲材料出库
            if (w_bustype.CompareTo("委外加工") == 0)
            {
                //委外入库（不倒冲仓库）
                string c_not_daochong_ware = "" + U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cPid='u8barcode_rd01_daochong_ware'", Cmd);
                //委外入库不倒冲 存货自定义项，若有 必须等于 ‘是’
                string c_not_inv_define = "" + U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cPid='u8barcode_rd01_daochong_invdefine'", Cmd);
                System.Data.DataTable dtWare11 = null;
                bool b_teshu_chuli = false;
                if (c_not_daochong_ware.IndexOf(w_cwhcode) > -1 && c_not_inv_define.CompareTo("") != 0) b_teshu_chuli = true;

                if (b_teshu_chuli)
                {
                    dtWare11 = U8Operation.GetSqlDataTable("select distinct cWhCode WhCode from " + dbname + "..OM_MOMaterials a inner join " + dbname + @"..inventory i on a.cinvcode=i.cinvcode
                        where MoDetailsID=0" + records01.iOMoDID + " and iWIPtype=1 and isnull(" + c_not_inv_define + ",'否')<>'是'", "dtWare11", Cmd);
                }
                else
                {   //无特殊处理
                    dtWare11 = U8Operation.GetSqlDataTable("select distinct cWhCode WhCode from " + dbname + @"..OM_MOMaterials 
                        where MoDetailsID=0" + records01.iOMoDID + " and iWIPtype=1", "dtWare11", Cmd);
                }

                string cOutRdCode = "" + U8Operation.GetDataString("select cVRSCode from " + dbname + @"..VouchRdContrapose where cVBTID='1107'", Cmd);

                if (dtWare11.Rows.Count > 0)
                {
                    if (cOutRdCode.CompareTo("") == 0) throw new Exception("倒冲出库需要 出库类别");
                    //存货小数位数
                    string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);
                    for (int r = 0; r < dtWare11.Rows.Count; r++)
                    {
                        if (dtWare11.Rows[r]["WhCode"] + "" == "") throw new Exception("倒冲材料出库仓库不能为空");
                        DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + cOutRdCode + "' crdcode,'" + dtWare11.Rows[r]["WhCode"] + "' cwhcode," + record01.cDepCode + " cdepcode," + record01.cCode + " cbuscode,'委外倒冲' cbustype,'000' headoutposcode", "dtRdMain", Cmd);
                        DataTable dtRddetail = null;

                        if (b_teshu_chuli)
                        {
                            dtRddetail = U8Operation.GetSqlDataTable(@"select b.MOMaterialsID allocateid,b.cinvcode,'' cbvencode,'' cbatch,round((" + records01.iQuantity + ")*b.iQuantity/a.iQuantity," + cInv_DecDgt + @") iquantity,b.MoDetailsID modid,'' cposcode
                                from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMaterials b on a.MODetailsID=b.MODetailsID 
                                    inner join " + dbname + @"..inventory i on b.cinvcode=i.cinvcode
                                where b.MoDetailsID=0" + records01.iOMoDID + " and b.cWhCode='" + dtWare11.Rows[r]["WhCode"] + @"' 
                                    and iWIPtype=1 and isnull(" + c_not_inv_define.Trim() + ",'否')<>'是'", "dtRddetail", Cmd);
                        }
                        else
                        {
                            dtRddetail = U8Operation.GetSqlDataTable(@"select b.MOMaterialsID allocateid,b.cinvcode,'' cbvencode,'' cbatch,round((" + records01.iQuantity + ")*b.iQuantity/a.iQuantity," + cInv_DecDgt + @") iquantity,b.MoDetailsID modid,'' cposcode
                                from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMaterials b on a.MODetailsID=b.MODetailsID 
                                where b.MoDetailsID=0" + records01.iOMoDID + " and b.cWhCode='" + dtWare11.Rows[r]["WhCode"] + "' and iWIPtype=1", "dtRddetail", Cmd);
                        }
                        if (dtRddetail.Rows.Count == 0) throw new Exception("无法找倒冲出库数据");
                        DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                        SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                        U81016(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81016", Cmd);
                    }
                }
            }
            #endregion

            //修复数据
            Cmd.CommandText = "update " + dbname + "..rdrecords01 set cvmiVenCode=case when cvmiVenCode='' then null else cvmiVenCode end where autoid =0" + cAutoid;
            Cmd.ExecuteNonQuery();
        }

        #endregion
        #endregion


        return rd_id + "," + cc_mcode;
    }

    //产品入库单 [生产订单入库（含已经检验的生产订单） 直接成品入库]
    public string U81015(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; 
        string w_rdcode = ""; //入库类型
        string w_bustype = "成品入库";//业务类型
        string w_headPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_Pos = false;//货位
        string imodid = ""; string imoid = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        bool b_feip_chongx = false; //废品不考虑非关键件比例问题
        
        #region  //逻辑检验
        if (BodyData.Columns.Contains("modid"))
        {
            imodid = BodyData.Rows[0]["modid"] + "";
            if (imodid.CompareTo("") != 0)
            {
                bCreate = true;
                imoid = U8Operation.GetDataString("select moid from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
                if (imoid == "") throw new Exception("没有找到生产订单信息");
            }
        }


        //等于notcompute，表名入库时不考虑 非关键料的齐套性
        string[] f_chongxio_text = GetTextsFrom_FormData_Tag(HeadData, "txt_notvalid_keypart").Split('_');
        if (f_chongxio_text.Length > 1)
        {
            if (f_chongxio_text[0].CompareTo("notcompute") == 0) b_feip_chongx = true;
        }
        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        //入库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        U8Operation.GetDataString("select '" + w_cwhcode + "'", Cmd);
        if (w_cwhcode == "")
        {
            if (bCreate)
            {
                w_cwhcode = U8Operation.GetDataString("select WhCode from " + dbname + "..mom_orderdetail where modid=0" + imodid, Cmd);
            }
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode=="")
        {
            if (bCreate)
            {
                w_cdepcode = U8Operation.GetDataString("select MDeptCode from " + dbname + "..mom_orderdetail where modid=0" + imodid, Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0411' and VT_TemplateMode=0", Cmd);

        string cc_mcode = "";//单据号
        #region //单据
        KK_U8Com.U8Rdrecord10 record10 = new KK_U8Com.U8Rdrecord10(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Tag(HeadData, "txt_mes_code"); //单据号
        if (trans_code == "")
        {
            string cCodeHead = "R" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd);
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('0000'+cast(cast(isnull(right(max(cCode),5),'00000') as int)+1 as varchar(9)),5) from " + dbname + "..rdrecord10(nolock) where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record10.cCode = "'" + cc_mcode + "'";
        record10.ID = rd_id;
        record10.cVouchType = "'10'";
        record10.bredvouch = "0"; //红篮子  
        record10.cBusType = "'" + w_bustype + "'";
        record10.cWhCode = "'" + w_cwhcode + "'";
        record10.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record10.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record10.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record10.VT_ID = vt_id;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            throw new Exception("自制品不能入代管仓库");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record10.cMaker = "'" + cUserName + "'";
        record10.dDate = "'" + cLogDate + "'";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_rd10_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record10.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record10.cHandler = "'" + c_vou_checker + "'";
            }
            
            record10.dVeriDate = "'" + cLogDate + "'";
        }
        record10.iExchRate = "1";
        record10.cExch_Name = "'人民币'";
        record10.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";
        if (bCreate)
        {
            record10.cSource = "'生产订单'";
        }
        else
        {
            record10.cSource = "'库存'";
        }

        #region   //主表自定义项处理
        record10.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record10.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record10.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record10.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record10.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record10.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record10.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record10.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record10.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record10.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record10.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record10.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record10.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record10.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record10.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record10.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));

        

        #endregion

        if (!record10.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        bool b_has_table_T_CC_Rd10_FlowCard = false;
        #region  //判断是否存在表 T_CC_Rd10_FlowCard
        try
        {
            //if (U8Operation.GetDataInt("SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_CC_Rd10_FlowCard]') AND type in (N'U')", Cmd) > 0)
            U8Operation.GetDataInt("SELECT COUNT(*) FROM " + dbname + @"..T_CC_Rd10_FlowCard(nolock) WHERE 1=0", Cmd);
            //不报错
            b_has_table_T_CC_Rd10_FlowCard = true;
        }
        catch
        {
            b_has_table_T_CC_Rd10_FlowCard = false;   //没有表说明参数无效
        }
        #endregion

        #region //子表
        string c_m_sorce = "";
        //判断仓库是否计入成本
        string b_costing = U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse(nolock) where cwhcode=" + record10.cWhCode, Cmd);
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("入库数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string chkValue = U8Operation.GetDataString("select moid from " + dbname + "..mom_orderdetail a where modid=0" + BodyData.Rows[i]["modid"], Cmd);
                if (chkValue.CompareTo("") == 0) throw new Exception("生单模式必须根据生产订单入库,请确认扫描的生产订单存在");
            }
            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "free" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["free" + f] = "";
                }
            }
            #endregion

            imodid = BodyData.Rows[i]["modid"] + "";
            imoid = U8Operation.GetDataString("select moid from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
            KK_U8Com.U8Rdrecords10 records10 = new KK_U8Com.U8Rdrecords10(Cmd, dbname);
            int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
            records10.AutoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();

            records10.ID = rd_id;
            string c_rd10_inv = BodyData.Rows[i]["cinvcode"] + "";
            string c_mom_inv = U8Operation.GetDataString("select invcode from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
            // 联产品用料表id
            string iMpoids = GetBodyValue_FromData(BodyData, i, "impoids");
            if (iMpoids == "" && c_mom_inv != c_rd10_inv)
            {
                throw new Exception("工单对应的产品编码应该是[" + c_mom_inv + "],[" + c_rd10_inv + "]是错误的");
            }

            records10.cInvCode = "'" + c_rd10_inv + "'";
            records10.iQuantity = "" + BodyData.Rows[i]["iquantity"];
            records10.irowno = (i + 1);
            cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records10.cInvCode + "", Cmd);
            records10.bCosting = b_costing;//是否计入成本

            records10.cbMemo = "'" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "'";
            #region //自由项  自定义项处理
            records10.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "free1") + "'";
            records10.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "free2") + "'";
            records10.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "free3") + "'";
            records10.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "free4") + "'";
            records10.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "free5") + "'";
            records10.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "free6") + "'";
            records10.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "free7") + "'";
            records10.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "free8") + "'";
            records10.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "free9") + "'";
            records10.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "free10") + "'";

            records10.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "define22") + "'";
            records10.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "define23") + "'";
            records10.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "define24") + "'";
            records10.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "define25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define26");
            records10.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define27");
            records10.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            records10.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "define28") + "'";
            records10.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "define29") + "'";
            records10.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "define30") + "'";
            records10.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "define31") + "'";
            records10.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "define32") + "'";
            records10.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "define33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define34");
            records10.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define35");
            records10.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define36");
            records10.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define37");
            records10.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            //自由项检验
            for (int f = 1; f < 11; f++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records10.cInvCode + " and bFree" + f + "=1", Cmd) > 0)
                {
                    if (GetBodyValue_FromData(BodyData, i, "free" + f) == "")
                        throw new Exception(records10.cInvCode + "有自由项" + f + "管理，必须录入");
                }
            }
            #endregion

            #region //获得单价与金额
            string i_cost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records10.cInvCode, Cmd);
            if (i_cost != "")
            {
                records10.iUnitCost = i_cost;
                records10.iPrice = "" + (decimal.Parse(records10.iQuantity) * decimal.Parse(i_cost));
            }
            #endregion

            #region //代管挂账处理  产品无代管
            records10.cvmivencode = "''";
            #endregion

            #region //批次管理和保质期管理
            records10.cBatch = "''";
            records10.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records10.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(records10.cInvCode + "有批次管理，必须输入批号");
                records10.cBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records10.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    if ((!BodyData.Columns.Contains("dprodate")) || BodyData.Rows[i]["dprodate"] + "" == "")  //生产日期判定
                        throw new Exception(records10.cInvCode + "有保质期管理，必须输入生产日期");
                    string rowpordate = "" + BodyData.Rows[i]["dprodate"];
                    if (rowpordate == "") rowpordate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
                    DataTable dtBZQ = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                                else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                            ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                        from " + dbname + "..inventory i(nolock) left join " + dbname + @"..Inventory_Sub s(nolock) on i.cinvcode=s.cinvsubcode 
                        where cinvcode=" + records10.cInvCode, "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count == 0) throw new Exception("计算存货保质期出现错误");

                    records10.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    records10.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    records10.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records10.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records10.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    records10.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + records10.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    //从模板中获得批次属性
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty1");
                    records10.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty2");
                    records10.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty3");
                    records10.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty4");
                    records10.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty5");
                    records10.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    records10.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty6") + "'";
                    records10.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty7") + "'";
                    records10.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty8") + "'";
                    records10.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty9") + "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty10");
                    records10.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                    //继承批次档案数据
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + records10.cInvCode + " and cBatch=" + records10.cBatch + " and isnull(cFree1,'')=" + records10.cFree1 + @" 
                                and isnull(cFree2,'')=" + records10.cFree2 + " and isnull(cFree3,'')=" + records10.cFree3 + " and isnull(cFree4,'')=" + records10.cFree4 + @" 
                                and isnull(cFree5,'')=" + records10.cFree5 + " and isnull(cFree6,'')=" + records10.cFree6 + " and isnull(cFree7,'')=" + records10.cFree7 + @" 
                                and isnull(cFree8,'')=" + records10.cFree8 + " and isnull(cFree9,'')=" + records10.cFree9 + " and isnull(cFree10,'')=" + records10.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count > 0)
                    {
                        records10.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                        records10.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                        records10.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                        records10.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                        records10.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                        records10.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                        records10.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                        records10.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                        records10.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                        records10.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";

                        //更新批次档案
                        Cmd.CommandText = "update " + dbname + "..AA_BatchProperty set cBatchProperty1=" + records10.cBatchProperty1 + ",cBatchProperty2=" + records10.cBatchProperty2 + @",
                                cBatchProperty3=" + records10.cBatchProperty3 + ",cBatchProperty4=" + records10.cBatchProperty4 + ",cBatchProperty5=" + records10.cBatchProperty5 + @",
                                cBatchProperty6=" + records10.cBatchProperty6 + ",cBatchProperty7=" + records10.cBatchProperty7 + ",cBatchProperty8=" + records10.cBatchProperty8 + @",
                                cBatchProperty9=" + records10.cBatchProperty9 + ",cBatchProperty10=" + records10.cBatchProperty10 + @" 
                            where cInvCode=" + records10.cInvCode + " and cBatch=" + records10.cBatch + " and isnull(cFree1,'')=" + records10.cFree1 + @" 
                                and isnull(cFree2,'')=" + records10.cFree2 + " and isnull(cFree3,'')=" + records10.cFree3 + " and isnull(cFree4,'')=" + records10.cFree4 + @" 
                                and isnull(cFree5,'')=" + records10.cFree5 + " and isnull(cFree6,'')=" + records10.cFree6 + " and isnull(cFree7,'')=" + records10.cFree7 + @" 
                                and isnull(cFree8,'')=" + records10.cFree8 + " and isnull(cFree9,'')=" + records10.cFree9 + " and isnull(cFree10,'')=" + records10.cFree10;
                        Cmd.ExecuteNonQuery();
                    }
                    else  //建立档案
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                            values(newid()," + records10.cBatchProperty1 + "," + records10.cBatchProperty2 + "," + records10.cBatchProperty3 + "," + records10.cBatchProperty4 + "," +
                                 records10.cBatchProperty5 + "," + records10.cBatchProperty6 + "," + records10.cBatchProperty7 + "," + records10.cBatchProperty8 + "," +
                                 records10.cBatchProperty9 + "," + records10.cBatchProperty10 + "," + records10.cInvCode + "," + records10.cBatch + "," + records10.cFree1 + "," +
                                 records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," + records10.cFree6 + "," +
                                 records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + ")";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion

            #region //固定换算率（多计量） 和 回写到生产单
            records10.iNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                records10.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + records10.cAssUnit, Cmd);
                if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                records10.iinvexchrate = ichange;
                inum = U8Operation.GetDataString("select round(" + records10.iQuantity + "/" + ichange + ",5)", Cmd);
                records10.iNum = inum;
            }
            // (非联产品入库才回写)
            if (iMpoids == "")
            {
                //回写生产订单累计入库数量 
                Cmd.CommandText = "update " + dbname + "..mom_orderdetail set QualifiedInQty=isnull(QualifiedInQty,0)+(0" + records10.iQuantity + ") where modid =0" + imodid;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            #region   //检验处理
            if (iMpoids == "")
            {
                DataTable dtCheckVouch = null;
                string qc_id = GetBodyValue_FromData(BodyData, i, "qc_id");
                int iQcFlg = U8Operation.GetDataInt("select CAST(QcFlag as int) from " + dbname + @"..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
                if (iQcFlg > 0)   //需要检验
                {
                    if (qc_id.CompareTo("") == 0) throw new Exception("产品" + records10.cInvCode + "需要检验");

                    dtCheckVouch = U8Operation.GetSqlDataTable(@"select n.id qc_id,CCHECKPERSONCODE,convert(varchar(10),n.DDATE,120) DDATE,n.CCHECKCODE,n.CVERIFIER,n.CVENCODE,n.FREGQUANTITY 
	                from " + dbname + @"..QMCHECKVOUCHER n(nolock) where n.id=0" + qc_id, "dtCheckVouch", Cmd);
                    if (dtCheckVouch.Rows.Count == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]需要检验，请先检验再入库");

                    if ((dtCheckVouch.Rows[0]["CVERIFIER"] + "").CompareTo("") == 0) throw new Exception("检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]未审核");
                    records10.iCheckIdBaks = "" + dtCheckVouch.Rows[0]["qc_id"];
                    records10.cCheckPersonCode = "'" + dtCheckVouch.Rows[0]["CCHECKPERSONCODE"] + "'";
                    records10.dCheckDate = "'" + dtCheckVouch.Rows[0]["DDATE"] + "'";
                    records10.cCheckCode = "'" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "'";

                    if (records10.irowno == 1)  //第一行
                    {
                        Cmd.CommandText = "update " + dbname + "..Rdrecord10 set cSource='产品检验单' where id=0" + record10.ID;
                        Cmd.ExecuteNonQuery();
                        c_m_sorce = "产品检验单";
                    }
                    else
                    {
                        //string c_sorue = "" + U8Operation.GetDataString("select cSource from " + dbname + "..Rdrecord10 where id=0" + record10.ID, Cmd);
                        if (c_m_sorce.CompareTo("产品检验单") != 0) throw new Exception("检验物资不能和非检验物资一起入库1");
                    }

                    //回写 检验单 累计入库数
                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set fsumquantity=isnull(fsumquantity,0)+" + records10.iQuantity + " where id =0" + dtCheckVouch.Rows[0]["qc_id"];
                    Cmd.ExecuteNonQuery();
                    //回写 检验单 　入库完毕标识　　
                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set BPROINFLAG=1 where id =0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>=isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)";
                    Cmd.ExecuteNonQuery();

                    //判断是否超检验单
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..QMCHECKVOUCHER(nolock) where id=0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)", Cmd) > 0)
                    {
                        throw new Exception("超检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]入库");
                    }
                }
                else
                {
                    if (records10.irowno == 1)  //第一行
                    {
                        c_m_sorce = record10.cSource.Replace("'", "");
                    }
                    else
                    {
                        if (c_m_sorce.CompareTo(record10.cSource.Replace("'", "")) != 0) throw new Exception("检验物资不能和非检验物资一起入库2");
                    }
                }
            }
            #region //  作废，效率问题
//            if (qc_id.CompareTo("") == 0)
//            {
//                //扫描生产订单入库
//                dtCheckVouch = U8Operation.GetSqlDataTable(@"select t.qc_autoid,t.qc_id,CAST(b.QcFlag as int) iGsp,CCHECKPERSONCODE,DDATE,CCHECKCODE,CVERIFIER,CVENCODE,FREGQUANTITY 
//                from " + dbname + @"..mom_orderdetail b left join
//                (select m.SOURCEAUTOID,m.IPROORDERID,isnull(m.autoid,0) qc_autoid,isnull(n.id,0) qc_id,CCHECKPERSONCODE,convert(varchar(10),n.DDATE,120) DDATE,n.CCHECKCODE,n.CVERIFIER,n.CVENCODE,n.FREGQUANTITY 
//                from " + dbname + @"..QMINSPECTVOUCHERS m inner join " + dbname + @"..QMINSPECTVOUCHER k on m.id=k.id and k.CVOUCHTYPE='QM02'
//                left join " + dbname + @"..QMCHECKVOUCHER n on m.autoid=n.INSPECTAUTOID) t on b.modid=t.SOURCEAUTOID and b.moid=t.IPROORDERID
//                where b.modid=0" + imodid, "dtCheckVouch", Cmd);
//            }
//            else
//            {
//                //扫描检验单入库
//                dtCheckVouch = U8Operation.GetSqlDataTable(@"select t.qc_autoid,t.qc_id,CAST(b.QcFlag as int) iGsp,CCHECKPERSONCODE,DDATE,CCHECKCODE,CVERIFIER,CVENCODE,FREGQUANTITY 
//                from " + dbname + @"..mom_orderdetail b left join
//                (select m.SOURCEAUTOID,m.IPROORDERID,isnull(m.autoid,0) qc_autoid,isnull(n.id,0) qc_id,CCHECKPERSONCODE,convert(varchar(10),n.DDATE,120) DDATE,n.CCHECKCODE,n.CVERIFIER,n.CVENCODE,n.FREGQUANTITY 
//                from " + dbname + @"..QMINSPECTVOUCHERS m inner join " + dbname + @"..QMINSPECTVOUCHER k on m.id=k.id and k.CVOUCHTYPE='QM02'
//                left join " + dbname + @"..QMCHECKVOUCHER n on m.autoid=n.INSPECTAUTOID) t on b.modid=t.SOURCEAUTOID and b.moid=t.IPROORDERID
//                where t.qc_id=0" + qc_id, "dtCheckVouch", Cmd);
//            }
//            if (dtCheckVouch.Rows.Count > 0)
//            {
//                if (int.Parse(dtCheckVouch.Rows[0]["iGsp"] + "") == 1)   //需要质检
//                {
//                    if (dtCheckVouch.Rows[0]["qc_id"] + ""=="" || int.Parse(dtCheckVouch.Rows[0]["qc_id"] + "") == 0) 
//                        throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]需要检验，请先检验再入库");
//                    if ((dtCheckVouch.Rows[0]["CVERIFIER"] + "").CompareTo("") == 0) throw new Exception("检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]未审核");
//                    records10.iCheckIdBaks = "" + dtCheckVouch.Rows[0]["qc_id"];
//                    records10.cCheckPersonCode = "'" + dtCheckVouch.Rows[0]["CCHECKPERSONCODE"] + "'";
//                    records10.dCheckDate = "'" + dtCheckVouch.Rows[0]["DDATE"] + "'";
//                    records10.cCheckCode = "'" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "'";

//                    if (records10.irowno == 1)  //第一行
//                    {
//                        Cmd.CommandText = "update " + dbname + "..Rdrecord10 set cSource='产品检验单' where id=0" + record10.ID;
//                        Cmd.ExecuteNonQuery();
//                    }
//                    else
//                    {
//                        string c_sorue = "" + U8Operation.GetDataString("select cSource from " + dbname + "..Rdrecord10 where id=0" + record10.ID, Cmd);
//                        if (c_sorue.CompareTo("产品检验单") != 0) throw new Exception("检验物资不能和非检验物资一起入库");
//                    }

//                    //回写 检验单 累计入库数
//                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set fsumquantity=isnull(fsumquantity,0)+" + records10.iQuantity + " where id =0" + dtCheckVouch.Rows[0]["qc_id"];
//                    Cmd.ExecuteNonQuery();
//                    //回写 检验单 　入库完毕标识　　
//                    Cmd.CommandText = "update " + dbname + "..QMCHECKVOUCHER set BPROINFLAG=1 where id =0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>=isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)";
//                    Cmd.ExecuteNonQuery();

//                    //判断是否超检验单
//                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..QMCHECKVOUCHER where id=0" + dtCheckVouch.Rows[0]["qc_id"] + " and isnull(fsumquantity,0)>isnull(FREGQUANTITY,0)+isnull(FCONQUANTIY,0)", Cmd) > 0)
//                    {
//                        throw new Exception("超检验单[" + dtCheckVouch.Rows[0]["CCHECKCODE"] + "]入库");
//                    }
//                }
//            }
//            else
//            {
//                throw new Exception("数据逻辑错误，或条码错误，或配置错误");
//            }

            #endregion

            #endregion

            #region//上游单据关联
            DataTable dtPuArr = null;  //上游单据表体自定义项继承
            records10.iNQuantity = records10.iQuantity;
            records10.iordertype = "0";
            if (bCreate)
            {
                //关联订单 生产订单
                records10.iNQuantity = records10.iQuantity;
                records10.iordertype = "0";
                //生产订单的生产批号
                records10.cMoLotCode = "'" + U8Operation.GetDataString("select MoLotCode from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd) + "'";

                System.Data.DataTable dtfclist = U8Operation.GetSqlDataTable(@"select top 1 a.opseq,a.description,b.WcCode from " + dbname + @"..sfc_moroutingdetail a(nolock) inner join 
                " + dbname + "..sfc_workcenter b(nolock) on a.wcid=b.wcid where modid=0" + imodid + " order by opseq desc", "dtfclist", Cmd);
                if (iMpoids == "")
                {
                    records10.iMPoIds = imodid;
                }
                else
                {
                    records10.iMPoIds = iMpoids;
                }
                records10.imoseq = "" + U8Operation.GetDataString("select SortSeq from " + dbname + @"..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
                records10.cmocode = "'" + U8Operation.GetDataString("select mocode from " + dbname + "..mom_order(nolock) where moid=0" + imoid, Cmd) + "'";
                if (dtfclist.Rows.Count > 0)  //
                {
                    records10.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
                    records10.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
                    records10.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
                }

                if (iMpoids == "")
                {
                    //继承表体自由项  自定义项数据
                    dtPuArr = U8Operation.GetSqlDataTable(@"select b.mocode,free1,free2,free3,free4,free5,free6,free7,free8,free9,free10,
                    define22,define23,define24,define25,define26,define27,define28,define29,define30,define31,define32,define33,define34,
                    define35,define36,define37,isnull(RelsUser,'') cverifier,a.CloseUser,Status,a.SortSeq from " + dbname + "..mom_orderdetail a(nolock) inner join " + dbname + @"..mom_order b(nolock) on a.moid=b.moid 
                    where modid=0" + imodid, "dtPuArr", Cmd);
                    if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到生产订单信息");
                    if (dtPuArr.Rows[0]["cverifier"] + "" == "") throw new Exception("生产订单【" + dtPuArr.Rows[0]["mocode"] + "】未终审");
                    // whf 判断是否关闭,要求同时满足状态为关闭,并且关闭人不为空
                    if (dtPuArr.Rows[0]["CloseUser"] + "" != "" && dtPuArr.Rows[0]["Status"] + "" == "4") throw new Exception("生产订单【" + dtPuArr.Rows[0]["mocode"] + "," + dtPuArr.Rows[0]["SortSeq"] + "】已经关闭");

                    if (records10.cFree1.CompareTo("''") == 0) records10.cFree1 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free1") + "'";
                    if (records10.cFree2.CompareTo("''") == 0) records10.cFree2 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free2") + "'";
                    if (records10.cFree3.CompareTo("''") == 0) records10.cFree3 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free3") + "'";
                    if (records10.cFree4.CompareTo("''") == 0) records10.cFree4 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free4") + "'";
                    if (records10.cFree5.CompareTo("''") == 0) records10.cFree5 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free5") + "'";
                    if (records10.cFree6.CompareTo("''") == 0) records10.cFree6 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free6") + "'";
                    if (records10.cFree7.CompareTo("''") == 0) records10.cFree7 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free7") + "'";
                    if (records10.cFree8.CompareTo("''") == 0) records10.cFree8 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free8") + "'";
                    if (records10.cFree9.CompareTo("''") == 0) records10.cFree9 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free9") + "'";
                    if (records10.cFree10.CompareTo("''") == 0) records10.cFree10 = "'" + GetBodyValue_FromData(dtPuArr, 0, "free10") + "'";

                    if (records10.cDefine22.CompareTo("''") == 0) records10.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define22") + "'";
                    if (records10.cDefine23.CompareTo("''") == 0) records10.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define23") + "'";
                    if (records10.cDefine24.CompareTo("''") == 0) records10.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define24") + "'";
                    if (records10.cDefine25.CompareTo("''") == 0) records10.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define25") + "'";

                    if (records10.cDefine22.CompareTo("''") == 0) records10.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define28") + "'";
                    if (records10.cDefine22.CompareTo("''") == 0) records10.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define29") + "'";
                    if (records10.cDefine30.CompareTo("''") == 0) records10.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define30") + "'";
                    if (records10.cDefine31.CompareTo("''") == 0) records10.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define31") + "'";
                    if (records10.cDefine32.CompareTo("''") == 0) records10.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define32") + "'";
                    if (records10.cDefine33.CompareTo("''") == 0) records10.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define33") + "'";
                }
            }
            #endregion

            #region//货位标识处理  ，若无货位取存货档案的默认货位
            if (b_Pos)
            {
                if (BodyData.Rows[i]["cposcode"] + "" == "" && w_headPosCode != "") BodyData.Rows[i]["cposcode"] = w_headPosCode;
                if (BodyData.Rows[i]["cposcode"] + "" == "")
                {
                    BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + records10.cInvCode, Cmd);
                    if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records10.cInvCode + "的货位不能为空");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + record10.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                    throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                records10.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
            }
            #endregion

            //保存数据
            if (!records10.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            #region //写入库状态数据
            if (b_feip_chongx)
            {
                if(b_has_table_T_CC_Rd10_FlowCard && iMpoids=="")
                {
                    string c_ctype = f_chongxio_text[1];  //缴库类型
                    Cmd.CommandText = "insert into " + dbname + @"..T_CC_Rd10_FlowCard(t_autoid,t_card_id,t_ctype) values(" + cAutoid + ",0," + c_ctype + ")";
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    b_feip_chongx = false;   //没有表说明参数无效
                }
            }
            #endregion
            if (iMpoids != "")
            {
                // 联产品相关
                Cmd.CommandText = "update " + dbname + "..rdrecords10 set bRelated=1,iorderdetailid=" + imodid + " where autoid =0" + cAutoid;
                Cmd.ExecuteNonQuery();
            }
            #region//货位账务处理
            if (b_Pos)
            {
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                        cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                    "Values (" + cAutoid + "," + rd_id + "," + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + "," + records10.iQuantity + "," + records10.iNum +
                    ",null," + record10.dDate + ",1,'',0," + record10.cVouchType + "," + record10.dDate + "," + record10.cMaker + "," + records10.cvmivencode + "," + records10.cBatch +
                    "," + records10.cFree1 + "," + records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," +
                    records10.cFree6 + "," + records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + ",''," + records10.cAssUnit + @",
                    " + records10.dMadeDate + "," + records10.iMassDate + "," + records10.cMassUnit + "," + records10.iExpiratDateCalcu + "," + records10.cExpirationdate + "," + records10.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();

                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords10 set iposflag=1 where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                //修改货位库存
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record10.cWhCode + " and cvmivencode=" + records10.cvmivencode + " and cinvcode=" + records10.cInvCode + @" 
                    and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + " and cfree1=" + records10.cFree1 + " and cfree2=" + records10.cFree2 + " and cfree3=" + records10.cFree3 + @" 
                    and cfree4=" + records10.cFree4 + " and cfree5=" + records10.cFree5 + " and cfree6=" + records10.cFree6 + " and cfree7=" + records10.cFree7 + @" 
                    and cfree8=" + records10.cFree8 + " and cfree9=" + records10.cFree9 + " and cfree10=" + records10.cFree10, Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                        cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                        dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate,dVDate) 
                    values(" + record10.cWhCode + "," + records10.cPosition + "," + records10.cInvCode + ",0," + records10.cBatch + @",
                        " + records10.cFree1 + "," + records10.cFree2 + "," + records10.cFree3 + "," + records10.cFree4 + "," + records10.cFree5 + "," +
                        records10.cFree6 + "," + records10.cFree7 + "," + records10.cFree8 + "," + records10.cFree9 + "," + records10.cFree10 + "," + records10.cvmivencode + @",'',0,
                        " + records10.dMadeDate + "," + records10.iMassDate + "," + records10.cMassUnit + "," + records10.iExpiratDateCalcu + "," + records10.cExpirationdate + @",
                        " + records10.dExpirationdate + "," + records10.dVDate + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + records10.iQuantity + "),inum=isnull(inum,0)+(" + records10.iNum + @") 
                    where cwhcode=" + record10.cWhCode + " and cvmivencode=" + records10.cvmivencode + " and cinvcode=" + records10.cInvCode + @" 
                    and cPosCode=" + records10.cPosition + " and cbatch=" + records10.cBatch + " and cfree1=" + records10.cFree1 + " and cfree2=" + records10.cFree2 + " and cfree3=" + records10.cFree3 + @" 
                    and cfree4=" + records10.cFree4 + " and cfree5=" + records10.cFree5 + " and cfree6=" + records10.cFree6 + " and cfree7=" + records10.cFree7 + @" 
                    and cfree8=" + records10.cFree8 + " and cfree9=" + records10.cFree9 + " and cfree10=" + records10.cFree10;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            #region//是否超生产订单检查  和  领料比例控制
            if (bCreate)
            {
                if (iMpoids == "")  //产品入库
                {
                    //已经入库量
                    float fRkqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..rdrecords10(nolock) where iMPoIds=0" + imodid + " and bRelated=0", Cmd));
                    if (fRkqty < 0) throw new Exception("存货【" + records10.cInvCode + "】对应订单的累计入库数不能小于0");
                    //生产订单量
                    float f_mo_qty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(sum(Qty),0) from " + dbname + @"..mom_orderdetail(nolock) where modid=0" + imodid));

                    #region //判断是否生产订单入库  0代表不能超   1 代表可超
                    if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='bOverMPIn'", Cmd) == 0)
                    {
                        float f_ll_qty = f_mo_qty;
                        if (f_ll_qty < fRkqty) throw new Exception("存货【" + records10.cInvCode + "】超生产订单入库：订单数[" + f_mo_qty + "]入库数[" + fRkqty + "]");
                    }
                    #endregion

                    #region   //生产订单自动关闭
                    //自动关闭生产订单
                    if (U8Operation.GetDataInt("select COUNT(*) from " + dbname + "..mom_parameter(nolock) where MoSysClosedFlag=1", Cmd) == 1)
                    {
                        if (U8Operation.GetDataInt("select COUNT(*) from " + dbname + "..mom_parameter(nolock) where moclosetype=1", Cmd) == 1)  //保存时自动关闭
                        {
                            if (f_mo_qty <= fRkqty)
                            {
                                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='自动关闭',CloseDate=convert(varchar(10),getdate(),120),CloseTime=getdate(),Status='4' where modid=0" + imodid;
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        else     //审核时自动关闭
                        {
                            float f_chked_rkqty = float.Parse(UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, @"select isnull(sum(a.iquantity),0) 
                            from " + dbname + "..rdrecords10 a(nolock) inner join " + dbname + "..rdrecord10 b(nolock) on a.id=b.id where a.iMPoIds=0" + imodid + " and a.bRelated=0 and isnull(b.cHandler,'')<>''"));
                            if (f_mo_qty <= f_chked_rkqty)
                            {
                                Cmd.CommandText = "update " + dbname + @"..mom_orderdetail set CloseUser='自动关闭',CloseDate=convert(varchar(10),getdate(),120),CloseTime=getdate(),Status='4' where modid=0" + imodid;
                                Cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    #endregion

                    #region//判断是否有关键子件  0代表不控制   1 代表控制
                    int iControlType = U8Operation.GetDataInt("select CAST(cvalue as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='iMOProInCtrlBySet'", Cmd);
                    if (iControlType == 1)  //iControlType=0 控制有无领料记录   
                    {
                        int iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + dbname + "..mom_moallocate a(nolock) inner join " + dbname + @"..Inventory_Sub i(nolock) on a.InvCode=i.cInvSubCode
                        where a.MoDId=0" + imodid + " and WIPType=3 and a.ByproductFlag=0", Cmd);
                        if (iKeysCount > 0 && U8Operation.GetDataInt("select count(*) from " + dbname + "..mom_moallocate(nolock) where MoDId=" + imodid + " and ByproductFlag=0 and isnull(IssQty,0)>0", Cmd) == 0)
                            throw new Exception("无领料记录");
                    }

                    if (iControlType == 2)  //iControlType=2 按照领料比例控制
                    {
                        int iKeysControl = U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='bControlKeyMaterial'", Cmd);
                        int iKeysCount = 0;
                        float iLL_Count = 0;
                        string cmom_mrp_qty = U8Operation.GetDataString("select isnull(max(Qty),0) from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);//生产订单数量
                        //if (iKeysControl == 1)  //控制关键材料比例
                        //{
                        //是否关键子件控制(库存 超生产订单入库 参数)
                        iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + dbname + "..mom_moallocate a(nolock) inner join " + dbname + @"..Inventory_Sub i(nolock) on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + imodid + " and i.bInvKeyPart=1  and WIPType=3 and a.ByproductFlag=0", Cmd);
                        iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + dbname + "..mom_moallocate a(nolock) inner join " + dbname + @"..Inventory_Sub i(nolock) on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + imodid + " and i.bInvKeyPart=1  and WIPType=3 and a.ByproductFlag=0 and Qty<>0", Cmd));
                        if (iKeysCount > 0 && fRkqty > iLL_Count) throw new Exception("存货【" + records10.cInvCode + "】BOM关键子件领料不足");
                        //}

                        if (iKeysControl != 1)   //控制所有材料比例
                        {
                            iKeysCount = U8Operation.GetDataInt(@"select count(*) cn from " + dbname + "..mom_moallocate a inner join " + dbname + @"..Inventory_Sub i(nolock) on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + imodid + " and i.bInvKeyPart<>1 and WIPType=3 and a.ByproductFlag=0", Cmd);

                            iLL_Count = float.Parse(U8Operation.GetDataString(@"select isnull(min(round(" + cmom_mrp_qty + "*IssQty/Qty+0.49999,0)),0) from " + dbname + "..mom_moallocate a(nolock) inner join " + dbname + @"..Inventory_Sub i(nolock) on a.InvCode=i.cInvSubCode
                            where a.MoDId=0" + imodid + " and i.bInvKeyPart<>1 and WIPType=3 and a.ByproductFlag=0 and Qty<>0", Cmd));
                            if (b_feip_chongx)
                            {
                                float f_feip_rk = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..rdrecords10 a(nolock) inner join " + dbname + @"..T_CC_Rd10_FlowCard b(nolock) on a.autoid=b.t_autoid 
                                where a.iMPoIds=0" + imodid + " and a.bRelated=0 and b.t_ctype in(2,3,5)", Cmd));
                                fRkqty = fRkqty - f_feip_rk;
                            }
                            if (iKeysCount > 0 && fRkqty > iLL_Count) throw new Exception("存货【" + records10.cInvCode + "】BOM非关键子件领料不足");
                        }

                    }
                    #endregion
                }
                else  //联产品入库
                {
                    Cmd.CommandText = "update " + dbname + @"..mom_moallocate set IssQty=isnull(IssQty ,0)+"+records10.iQuantity+"  where allocateid=0" + iMpoids;
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion

            #region  //入库倒冲
            System.Data.DataTable dtWare11 = U8Operation.GetSqlDataTable("select distinct WhCode from " + dbname + @"..mom_moallocate(nolock) where modid=0" + imodid + " and WIPType=1", "dtWare11", Cmd);
            //获得出库类别
            string cOutRdCode = "" + U8Operation.GetDataString("select cVRSCode from " + dbname + @"..VouchRdContrapose(nolock) where cVBTID='1104'", Cmd);
            if (iMpoids=="" && dtWare11.Rows.Count > 0)
            {
                if (cOutRdCode.CompareTo("") == 0) throw new Exception("倒冲出库需要 出库类别");
                //存货小数位数
                string cInv_DecDgt = U8Operation.GetDataString("SELECT cValue FROM " + dbname + @"..AccInformation(nolock) WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);
                for (int r = 0; r < dtWare11.Rows.Count; r++)
                {
                    if (dtWare11.Rows[r]["WhCode"] + "" == "") throw new Exception("倒冲材料出库仓库不能为空");
                    DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + cOutRdCode + "' crdcode,'" + dtWare11.Rows[r]["WhCode"] + "' cwhcode," + record10.cDepCode + " cdepcode," + record10.cCode + " cbuscode,'生产倒冲' cbustype,'000' headoutposcode", "dtRdMain", Cmd);
                    DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select b.allocateid,b.invcode cinvcode,'' cbvencode,'' cbatch,round((" + records10.iQuantity + ")*b.Qty/a.Qty," + cInv_DecDgt + @") iquantity,b.modid,'' cposcode
                        from " + dbname + "..mom_orderdetail a(nolock) inner join " + dbname + @"..mom_moallocate b(nolock) on a.modid=b.modid 
                        where b.modid=0" + imodid + " and b.WhCode='" + dtWare11.Rows[r]["WhCode"] + "' and b.WIPType=1 and a.Qty<>0", "BodyData", Cmd);
                    if (dtRddetail.Rows.Count == 0) throw new Exception("无法找倒冲出库数据");
                    DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                    SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                    U81016(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81016", Cmd);
                }
            }
            #endregion
            #region //联副产品入库
            // 代表联产品的iMpoids为空时才进行联产品入库,不然会重复联产品入库
            if (iMpoids == "")
            {
                System.Data.DataTable dtWare10 = U8Operation.GetSqlDataTable("select distinct WhCode from " + dbname + @"..mom_moallocate where modid=0" + imodid + " and WIPType=3 and ProductType=2 ", "dtWare10", Cmd);
                //获得入库类别 (todo 查看入库类别)
                string cInRdCode = "" + U8Operation.GetDataString("select cVRRCode from " + dbname + @"..VouchRdContrapose where cVBTID='1001'", Cmd);
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
                        U8Operation.GetDataString("select '联产品测试'", Cmd);
                        DataTable dtRdMain = U8Operation.GetSqlDataTable("select '" + cInRdCode + "' crdcode,'" + dtWare10.Rows[r]["WhCode"] + "' cwhcode," + record10.cDepCode + " cdepcode," + record10.cCode + " cbuscode,'生产倒冲' cbustype", "dtRdMain", Cmd);
                        DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select b.allocateid impoids,b.invcode cinvcode,'' cbvencode," + records10.cBatch + " cbatch,round((" + records10.iQuantity + @")*BaseQtyN/BaseQtyD," + cInv_DecDgt + @") iquantity,b.modid,'' cposcode
                        from " + dbname + @"..mom_moallocate b where modid=0" + imodid + " and WhCode='" + dtWare10.Rows[r]["WhCode"] + "' and WIPType=3 and ProductType=2 ", "BodyData", Cmd);
                        if (dtRddetail.Rows.Count == 0) throw new Exception("无法找联产品入库数据");
                        DataTable SHeadData = u8op.GetDtToHeadData(dtRdMain, 0);
                        SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                        u8op.U81015(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81015", Cmd);
                    }
                }
            }
            #endregion
        }

        #endregion

        #endregion
        #region //华青盛具打印入库代码
        string t_cqcc_barCodePrintMainName = U8Operation.GetDataString("select name from " + dbname + ".sys.tables where name ='t_cqcc_barCodePrintMain'", Cmd);
        if(!string.IsNullOrEmpty(t_cqcc_barCodePrintMainName))
        {
            string t_card_no = U8Operation.GetDataString(@"
select c.t_card_no,'盛具入库' aaa from " + dbname + @"..T_CC_Card_List c
inner join (
select b.cDefine23 cardNo,sum(b.iQuantity) qty from " + dbname + @"..rdrecord10 a 
inner join " + dbname + @"..rdrecords10 b on a.ID = b.ID
where a.cDefine2='盛具打印' and a.id = 0" + rd_id + @" and ISNULL(b.cdefine23,'')!=''
group by b.cDefine23
) a on a.cardNo = c.t_card_no
where isnull(a.qty,0)>isnull(c.t_card_overqty,0)
", Cmd);
            if (!string.IsNullOrEmpty(t_card_no))
            {
                throw new Exception("盛具入库提示:流转卡" + t_card_no + "报工量不足或未报工");
            }
        }
        #endregion
        return rd_id + "," + cc_mcode;
    }

    //材料出库单 [生产订单批量出库 直接材料出库 委外材料出库]
    public string U81016(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; //出库类型
        string w_bustype = "领料";//业务类型
        bool b_Vmi = false; bool b_Pos = false;//货位
        string imodid = ""; string AllocateId = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        bool b_MerAPP = false;   //sq_autoid  是否包含此列
        string MerApp_Code = "";//申请单 单据号
        int i_red_sheet = 0;
        string ret_moallocate = ""; //生产订单子件表外领料
        ArrayList list_moallocate = new ArrayList();  //生产订单子件
        bool b_retID = false; //返回子件ID；
        #region  //逻辑检验
        if (BodyData.Rows.Count == 0) throw new Exception("材料出库单必须有表体数据");
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) w_bustype = "领料";

        if (BodyData.Columns.Contains("sq_autoid"))
        {
            if (BodyData.Rows[0]["sq_autoid"] + "" != "")
            {
                b_MerAPP = true;
                MerApp_Code = U8Operation.GetDataString("select b.cCode from " + dbname + "..MaterialAppVouchs a(nolock) inner join " + dbname + @"..MaterialAppVouch b(nolock) on a.ID=b.ID 
                    where a.AutoID=0" + BodyData.Rows[0]["sq_autoid"], Cmd);
            }
        }
        if (float.Parse("" + BodyData.Rows[0]["iquantity"]) < 0) i_red_sheet = 1;//第一行数据的 数量判断红篮子
        //
        if (BodyData.Columns.Contains("allocateid"))
        {
            AllocateId = BodyData.Rows[0]["allocateid"] + "";
            bCreate = true;
            if (AllocateId.CompareTo("") != 0)
            {
                //bCreate = true;
                if (w_bustype.CompareTo("委外发料") == 0 || w_bustype.CompareTo("委外倒冲") == 0)
                {
                    imodid = U8Operation.GetDataString("select MoDetailsID from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + AllocateId, Cmd);
                    if (imodid == "") throw new Exception("没有找到委外订单信息");
                }
                else
                {
                    imodid = U8Operation.GetDataString("select modid from " + dbname + "..mom_moallocate(nolock) where AllocateId=0" + AllocateId + " and ByproductFlag=0", Cmd);
                    if (imodid == "") throw new Exception("没有找到生产订单信息,或子件为联副产品");
                }
            }
            else
            {
                b_retID = true;
            }
        }
        else if (BodyData.Columns.Contains("sq_autoid"))
        {
            bCreate = true;
        }
        
        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //供应商
        w_vencode = GetTextsFrom_FormData_Tag(HeadData, "txt_cvencode");
        if (w_vencode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..vendor(nolock) where cvencode='" + w_vencode + "'", Cmd) == 0)
                throw new Exception("委外商输入错误");
        }
        if (w_vencode == "" && bCreate && (w_bustype.CompareTo("委外发料") == 0 || w_bustype.CompareTo("委外倒冲") == 0))
            w_vencode = U8Operation.GetDataString("select b.cVenCode from " + dbname + "..OM_MODetails a(nolock) inner join " + dbname + @"..OM_MOMain b(nolock) on a.moid=b.moid 
                                                    where a.MODetailsID=0" + imodid, Cmd);

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (w_bustype.CompareTo("委外发料") == 0 || w_bustype.CompareTo("委外倒冲") == 0)
                {
                    w_cdepcode = U8Operation.GetDataString("select a.cDepCode from " + dbname + "..OM_MOMain a(nolock) inner join " + dbname + "..OM_MODetails b(nolock) on a.MOID=b.MOID where b.MODetailsID=0" + imodid, Cmd);
                }
                else
                {
                    w_cdepcode = U8Operation.GetDataString("select MDeptCode from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
                }
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person(nolock) where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a(nolock) 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b(nolock) on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        #region   //表头必录项校验 在前端完成，此处作废
        //System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
        //        and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        //for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        //{
        //    string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
        //    txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
        //    if (txtdata == null) throw new Exception(dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项，模板必须设置成可视栏目");

        //    if (txtdata[3].CompareTo("") == 0 && txtdata[2].CompareTo("") != 0)  //
        //    {
        //        throw new Exception(txtdata[0] + "录入不正确 录入键值和显示值不匹配");
        //    }
        //    if (txtdata[3].CompareTo("") == 0)  //
        //    {
        //        throw new Exception(txtdata[0] + "为必录项,不能为空");
        //    }
        //}
        #endregion

        //检查单据体必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='mes_cfirstToFirst'", Cmd);
        //严格控制先进先出规则
        string c_firstTofisrt_Ctl = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_out_cfirstToFirst_control'", Cmd);
        c_firstTofisrt_Ctl = c_firstTofisrt_Ctl.ToLower();
        //先进先出 例外仓库
        string c_firstTofisrt_NotCtl_WareList = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_out_cfirstToFirst_Not_control_ware'", Cmd);
        c_firstTofisrt_NotCtl_WareList = "," + c_firstTofisrt_NotCtl_WareList + ",";
        //小数位数
        int iInv_DecDgt = U8Operation.GetDataInt("SELECT cValue FROM " + dbname + @"..AccInformation(nolock) WHERE cName = 'iStrsQuanDecDgt' AND cSysID = 'AA'", Cmd);

        string cc_mcode = "";//单据号
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0412' and VT_TemplateMode=0", Cmd);

        #region //单据
        KK_U8Com.U8Rdrecord11 record11 = new KK_U8Com.U8Rdrecord11(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Tag(HeadData, "txt_mes_code"); //单据号
        if (trans_code == "")
        {
            string cCodeHead = "C" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord11(nolock) where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record11.cCode = "'" + cc_mcode + "'";
        record11.ID = rd_id;
        record11.cVouchType = "'11'";
        record11.bredvouch = "" + i_red_sheet; //红篮子  
        record11.cBusType = "'" + w_bustype + "'";
        record11.cWhCode = "'" + w_cwhcode + "'";
        record11.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record11.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record11.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record11.cVenCode = (w_vencode.CompareTo("") == 0 ? "null" : "'" + w_vencode + "'");
        record11.VT_ID = vt_id;
        record11.bredvouch = i_red_sheet + "";//红蓝子
        record11.bOMFirst = "0";

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record11.cMaker = "'" + cUserName + "'";
        record11.dDate = "'" + cLogDate + "'";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cPid='u8barcode_rd11_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record11.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record11.cHandler = "'" + c_vou_checker + "'";
            }
            
            record11.dVeriDate = "'" + cLogDate + "'";
            record11.dnverifytime = "getdate()";
        }
        record11.iExchRate = "1";
        record11.cExch_Name = "'人民币'";
        record11.cMemo = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cmemo") +"'";
        if (bCreate)
        {
            if (w_bustype.CompareTo("生产倒冲") == 0)
            {
                record11.cSource = "'产成品入库单'";
                record11.cBusCode = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode") + "'";
            }
            else if (w_bustype.CompareTo("委外倒冲") == 0)
            {
                record11.cSource = "'采购入库单'";
                record11.cBusCode = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode") + "'";
                record11.cPsPcode = "'" + U8Operation.GetDataString("select cinvcode from " + dbname + "..OM_MOMaterials where MoDetailsID=0" + imodid, Cmd) + "'";
            }
            else if (w_bustype.CompareTo("委外发料") == 0)
            {
                if (b_MerAPP)
                {
                    record11.cSource = "'领料申请单'";
                    record11.cBusCode = "'" + MerApp_Code + "'";
                }
                else
                {
                    record11.cSource = "'委外订单'";
                }
            }
            else
            {
                string cc_bbuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode");
                if (cc_bbuscode != "") record11.cBusCode = "'" + cc_bbuscode + "'";
                if (b_MerAPP)
                {
                    record11.cSource = "'领料申请单'";
                    record11.cBusCode = "'" + MerApp_Code + "'";
                }
                else
                {
                    record11.cSource = "'生产订单'";
                }
            }
        }
        else
        {
            record11.cSource = "'库存'";
        }

        #region   //主表自定义项处理
        record11.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record11.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record11.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record11.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record11.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record11.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record11.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record11.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record11.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record11.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record11.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record11.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record11.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record11.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record11.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record11.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        record11.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";
        if (!record11.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

        if (w_bustype.CompareTo("委外倒冲") == 0)
        {
            string ccc_mocode = U8Operation.GetDataString("select ccode from " + dbname + "..OM_MOMain a inner join " + dbname + "..OM_MODetails b on a.MOID=b.MOID where MoDetailsID=0" + imodid, Cmd);
            Cmd.CommandText = "update " + dbname + "..rdrecord11 set cmpocode='" + ccc_mocode + "' where id=0" + record11.ID;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        #region //子表
        int irdrow = 0;
        //判断仓库是否计入成本
        string b_costing = U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse(nolock) where cwhcode=" + record11.cWhCode, Cmd);

        //BodyData 数据按照存货编码 批号排序
        if (c_firstTofisrt_Ctl.CompareTo("true") == 0)   //管控先进先出法前先排序
        {
            BodyData.DefaultView.Sort = "cinvcode,cbatch";
            BodyData = BodyData.DefaultView.ToTable();
        }

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch= GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            string c_modid = "";//产品行 ID
            #region  //行业务逻辑校验
            decimal d_rd_qty = decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
            if (d_rd_qty == 0) throw new Exception("出库数量不能为空 或0");
            if ((i_red_sheet == 0 && d_rd_qty < 0) || (i_red_sheet == 1 && d_rd_qty > 0)) throw new Exception("本单据红蓝子混乱");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            //生单规则，供应商和业务类型一致性检查
            //增加材料申请单路径--2022/09/28
            string up_mo_code = "";//生产订单号 或 委外订单号
            if (BodyData.Columns.Contains("modid"))
            {
                imodid = BodyData.Rows[i]["modid"] + "";
                if (bCreate)
                {
                    if (w_bustype.CompareTo("委外发料") == 0 || w_bustype.CompareTo("委外倒冲") == 0)
                    {
                        string chkValue = U8Operation.GetDataString("select MoDetailsID from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + BodyData.Rows[i]["allocateid"], Cmd);
                        if (chkValue.CompareTo("") == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]生单模式必须根据委外订单领料,请确认传入的委外订单存在");
                        up_mo_code = U8Operation.GetDataString("select cCode from " + dbname + "..OM_MODetails a(nolock) inner join " + dbname + @"..OM_MOMain b(nolock) on a.MOID=b.MOID where a.MODetailsID=0" + chkValue, Cmd);

                        if (w_bustype != "委外倒冲")
                        {
                            if (U8Operation.GetDataInt("select count(1) from " + dbname + "..OM_MODetails(nolock) where MODetailsID=0" + chkValue + " and isnull(cbCloser,'')<>''", Cmd) > 0)
                                throw new Exception("订单已经关闭，不能领用。订单[" + up_mo_code + "]");
                        }
                        c_modid = chkValue;
                    }
                    else
                    {
                        AllocateId = BodyData.Rows[i]["allocateid"] + "";
                        if (AllocateId.CompareTo("") != 0)
                        {
                            string chkValue = U8Operation.GetDataString("select modid from " + dbname + "..mom_moallocate a(nolock) where allocateid=0" + BodyData.Rows[i]["allocateid"] + " and a.ByproductFlag=0", Cmd);
                            if (chkValue.CompareTo("") == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]生单模式必须根据生产订单领料,请确认扫描的生产订单存在,或是否被设置成联副产品");
                            up_mo_code = U8Operation.GetDataString("select mocode from " + dbname + "..mom_orderdetail a(nolock) inner join " + dbname + @"..mom_order b(nolock) on a.moid=b.moid where a.modid=0" + chkValue, Cmd);
                            if (w_bustype != "生产倒冲")
                            {
                                if (U8Operation.GetDataInt("select count(1) from " + dbname + "..mom_orderdetail(nolock) where modid=0" + chkValue + " and isnull(CloseUser,'')<>''", Cmd) > 0)
                                    throw new Exception("订单已经关闭，不能领用。订单[" + up_mo_code + "]");
                            }
                            c_modid = chkValue;
                            //生产部门一致性检查
                            string chkDep = U8Operation.GetDataString("select MDeptCode from " + dbname + "..mom_orderdetail(nolock) where modid=0" + chkValue, Cmd);
                            if (chkDep != "" && w_cdepcode != "" && chkDep != w_cdepcode)
                                throw new Exception("本次出库的材料对应的生产订单的部门不一致,订单部门[" + chkDep + "],材料出库单部门[" + w_cdepcode + "]");
                        }
                        else
                        {
                            up_mo_code = U8Operation.GetDataString("select mocode from " + dbname + "..mom_orderdetail a(nolock) inner join " + dbname + @"..mom_order b(nolock) on a.moid=b.moid where a.modid=0" + imodid, Cmd);
                        }

                    }
                }
            }
            else
            {
                if (!(BodyData.Columns.Contains("sq_autoid"))) throw new Exception("材料申请单必须有子表autoID");
            }

            ////自由项
            //for (int f = 1; f < 11; f++)
            //{
            //    string cfree_value = GetBodyValue_FromData(BodyData, i, "free" + f);
            //    if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
            //    {
            //        if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
            //    }
            //    else
            //    {
            //        if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["free" + f] = "";
            //    }
            //}
            #endregion
            if (AllocateId.CompareTo("") != 0)
            {
                if (c_modid != imodid) throw new Exception("传入的用料id不属于本订单产品的材料");
            }
            if (BodyData.Columns.Contains("allocateid"))
            AllocateId = BodyData.Rows[i]["allocateid"] + "";
            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            // whf 添加逻辑如果存货需要批次管理才进入自动指定批次的代码
            bool isBatch = U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "' and bInvBatch=1 ", Cmd) > 0;
            if (c_body_batch.CompareTo("") == 0 && cfirstToFirst.CompareTo("auto") == 0 && i_red_sheet == 0 && isBatch)  //没有录入批次且需要执行自动指定批次出库
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, GetBodyValue_FromData(BodyData, i, "cposcode"),
                    GetBodyValue_FromData(BodyData, i, "free1"), GetBodyValue_FromData(BodyData, i, "free2"), GetBodyValue_FromData(BodyData, i, "free3"),
                    GetBodyValue_FromData(BodyData, i, "free4"), GetBodyValue_FromData(BodyData, i, "free5"), GetBodyValue_FromData(BodyData, i, "free6"), 
                    GetBodyValue_FromData(BodyData, i, "free7"), GetBodyValue_FromData(BodyData, i, "free8"), GetBodyValue_FromData(BodyData, i, "free9"),
                    GetBodyValue_FromData(BodyData, i, "free10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            #endregion

            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                #region//自由项
                for (int f = 1; f < 11; f++)
                {
                    string cfree_value = dtMer.Rows[m]["cfree" + f] + "";
                    if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                    {
                        if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                    }
                    else
                    {
                        if (cfree_value.CompareTo("") != 0) dtMer.Rows[m]["cfree" + f] = "";
                    }
                }
                #endregion

                KK_U8Com.U8Rdrecords11 records11 = new KK_U8Com.U8Rdrecords11(Cmd, dbname);
                int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
                records11.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records11.ID = rd_id;
                records11.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                records11.iQuantity = "" + dtMer.Rows[m]["iquantity"];
                irdrow++;
                records11.irowno = irdrow;
                //records11.cbsysbarcode = "'||st11|" + cc_mcode + '|'+ irdrow + "'";
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records11.cInvCode + "", Cmd);

                #region //设置工序信息
                System.Data.DataTable dtfclist =U8Operation.GetSqlDataTable(@"select top 1 a.opseq,a.description,b.WcCode from " + dbname + @"..sfc_moroutingdetail a inner join 
                " + dbname + "..sfc_workcenter b on a.wcid=b.wcid where modid=0" + imodid + " order by opseq desc", "dtfclist", Cmd);
                if (dtfclist.Rows.Count > 0)  //设置工序信息
                {
                    records11.cmworkcentercode = "'" + dtfclist.Rows[0]["WcCode"] + "'";
                    records11.iopseq = "'" + dtfclist.Rows[0]["opseq"] + "'";
                    records11.copdesc = "'" + dtfclist.Rows[0]["description"] + "'";
                }
                #endregion

                #region //自由项  自定义项处理
                records11.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                records11.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                records11.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                records11.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                records11.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                records11.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                records11.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                records11.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                records11.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                records11.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                records11.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "define22") + "'";
                records11.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "define23") + "'";
                records11.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "define24") + "'";
                records11.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "define25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define26");
                records11.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define27");
                records11.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                records11.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "define28") + "'";
                records11.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "define29") + "'";
                records11.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "define30") + "'";
                records11.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "define31") + "'";
                records11.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "define32") + "'";
                records11.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "define33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define34");
                records11.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define35");
                records11.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define36");
                records11.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "define37");
                records11.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                #endregion

                #region //获得单价与金额
                string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records11.cInvCode, Cmd);
                if (c_ppcost != "")
                {
                    decimal d_dcost = 0.0M;
                    if (c_ppcost.Contains("E"))
                    {
                        d_dcost = Decimal.Parse(c_ppcost, System.Globalization.NumberStyles.Float);
                    }
                    else
                    {
                        d_dcost = decimal.Parse(c_ppcost);
                    }
                    records11.iPUnitCost = "" + d_dcost;
                    records11.iPPrice = "" + Math.Round(d_dcost * decimal.Parse("" + dtMer.Rows[m]["iquantity"]), 2);
                }
                #endregion

                #region //代管挂账处理
                records11.cvmivencode = "''";
                records11.bCosting = b_costing; //是否计入成本
                if (b_Vmi)
                {
                    records11.bVMIUsed = "1";
                    records11.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理
                records11.cBatch = "''";
                records11.iExpiratDateCalcu = "0";
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records11.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" == "") throw new Exception(records11.cInvCode + "有批次管理，必须输入批号,对应仓库编码:" + w_cwhcode);
                    records11.cBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                    //保质期管理
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records11.cInvCode + " and bInvQuality=1", Cmd) > 0)
                    {
                        records11.iExpiratDateCalcu = dtMer.Rows[m]["有效期推算方式"] + "";
                        records11.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                        records11.dVDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records11.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records11.dMadeDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                        records11.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                    }

                    //批次档案 建档
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + records11.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                    {
                        DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + records11.cInvCode + " and cBatch=" + records11.cBatch + " and isnull(cFree1,'')=" + records11.cFree1 + @" 
                                and isnull(cFree2,'')=" + records11.cFree2 + " and isnull(cFree3,'')=" + records11.cFree3 + " and isnull(cFree4,'')=" + records11.cFree4 + @" 
                                and isnull(cFree5,'')=" + records11.cFree5 + " and isnull(cFree6,'')=" + records11.cFree6 + " and isnull(cFree7,'')=" + records11.cFree7 + @" 
                                and isnull(cFree8,'')=" + records11.cFree8 + " and isnull(cFree9,'')=" + records11.cFree9 + " and isnull(cFree10,'')=" + records11.cFree10, "dtBatPerp", Cmd);
                        if (dtBatPerp.Rows.Count > 0)
                        {
                            records11.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                            records11.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                            records11.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                            records11.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                            records11.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                            records11.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                            records11.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                            records11.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                            records11.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                            records11.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                        }
                    }
                }
                #endregion

                #region //固定换算率（多计量） 和 回写到生产单
                records11.iNum = "null";
                if (cst_unitcode.CompareTo("") != 0)
                {
                    records11.cAssUnit = "'" + cst_unitcode + "'";
                    string ichange = "";
                    if (BodyData.Columns.Contains("inum")) inum = "" + BodyData.Rows[i]["inum"];
                    if (inum == "") inum = "0";
                    if (decimal.Parse(inum) == 0)
                    {
                        ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + records11.cAssUnit, Cmd);
                        if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                        inum = U8Operation.GetDataString("select round(" + records11.iQuantity + "/" + ichange + ",5)", Cmd);
                    }
                    else
                    {
                        ichange = "" + (decimal.Parse(records11.iQuantity) / decimal.Parse(inum));
                    }

                    records11.iinvexchrate = ichange;
                    records11.iNum = inum;
                }

                if (w_bustype.CompareTo("生产倒冲") == 0 || w_bustype.CompareTo("领料") == 0)
                {
                    //如果AllocateId为空，则需要做表外领料，新增一笔生产订单子件物料。
                    if (AllocateId.CompareTo("") != 0)
                    {
                        //回写到生产订单用量表累计出库数量
                        Cmd.CommandText = "update " + dbname + "..mom_moallocate set issqty=round(isnull(issqty,0)+(0" + records11.iQuantity + "),6) where AllocateId =0" + AllocateId;
                        Cmd.ExecuteNonQuery();
                    }
                    else if (imodid.CompareTo("") != 0)
                    {
                        int iChildId = 0;
                        string caccid = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
                        //检查是否允许表外领料
                        if (U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation Where cSysId =N'ST' and  cName= N'bCanOutOfMaterials'", Cmd) != "True")
                            throw new Exception(imodid + "子件AllocateId不能为空！");
                        iChildId = 1000000000 + UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + caccid + "' and cVouchType='mom_moallocate'");
                        Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + caccid + "' and cVouchType='mom_moallocate'";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "insert into " + dbname + @"..mom_moallocate (AllocateId,MoDId,SortSeq,OpSeq,ComponentId,FVFlag,BaseQtyN,BaseQtyD,ParentScrap,CompScrap,Qty,IssQty,
                            DeclaredQty,StartDemDate,EndDemDate,WIPType,ByproductFlag,QcFlag,Offset,InvCode,OpComponentId,ReplenishQty,TransQty,ProductType,SoType,QmFlag,OrgQty,OrgAuxQty,
                            RequisitionFlag,RequisitionQty,RequisitionIssQty,CostWIPRel,cSubSysBarCode,PickingQty,PickingAuxQty,UpperMoQty,InvAlloeFlag,TransAppQty,ReplenishApplyQty) "+
                             "values (" + iChildId + "," + imodid + "," + "isnull((select MAX(isnull(sortseq,0)) from " + dbname + "..mom_moallocate where MoDId=" + imodid + "),0)+10,'000'," + "(select PartId from " + dbname + "..bas_part where InvCode ='"
                             + BodyData.Rows[i]["cinvcode"] + "'),1,1,1,0,0," + records11.iQuantity + "," + records11.iQuantity + ",0,(select StartDate from " + dbname + "..mom_morder where MoDId = " + imodid +
                            "),(select DueDate from " + dbname + "..mom_morder where MoDId = " + imodid + "),3,0,0,0,'" + BodyData.Rows[i]["cinvcode"] + "',0,0,0,1,0,0,0,0,0,0,0,0,(select '||M021|'+" + "(select MoCode from " + dbname + "..mom_order where MoId in (select MoId from " + dbname + "..mom_orderdetail where MoDId = " + imodid + "))" + "+'|'+" + "(select cast(sortseq as varchar) from " + dbname + "..mom_orderdetail where MoDId = " + imodid +
                            ")" + "+'|'+" + "(select cast(isnull((select MAX(isnull(sortseq,0)) from " + dbname + "..mom_moallocate where MoDId=" + imodid + "),0)+10 as varchar))),0,0,0,0,0,0" + ")";
                        Cmd.ExecuteNonQuery();
                        AllocateId = Convert.ToString(iChildId);
                        list_moallocate.Add(AllocateId);
                    }

                }
                else if (w_bustype.CompareTo("委外倒冲") == 0 || w_bustype.CompareTo("委外发料") == 0)
                {
                    Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set iSendQTY=round(isnull(iSendQTY,0)+(0" + records11.iQuantity + "),6) where MOMaterialsID =0" + AllocateId;
                    Cmd.ExecuteNonQuery();

                    //iMaterialSendQty
                    Cmd.CommandText = "update " + dbname + "..OM_MODetails set iMaterialSendQty=round(isnull(iMaterialSendQty,0)+(0" + records11.iQuantity + "),6) where MODetailsID =0" + c_modid;
                    Cmd.ExecuteNonQuery();

                }

                //回写 材料申请单  b_MerAPP
                if (b_MerAPP && BodyData.Rows[i]["sq_autoid"] + "" != "")
                {
                    Cmd.CommandText = "update " + dbname + "..MaterialAppVouchs set fOutQuantity=round(isnull(fOutQuantity,0)+(0" + records11.iQuantity + "),6) where autoid =0" + BodyData.Rows[i]["sq_autoid"];
                    Cmd.ExecuteNonQuery();
                    //回写到生产订单用量表 申请已领量    fsendapplyqty
                    if ((w_bustype.CompareTo("生产倒冲") == 0 || w_bustype.CompareTo("领料") == 0) && AllocateId.CompareTo("") != 0)
                    {
                        Cmd.CommandText = "update " + dbname + "..mom_moallocate set RequisitionIssQty=round(isnull(RequisitionIssQty,0)+(0" + records11.iQuantity + "),6) where AllocateId =0" + AllocateId;
                        Cmd.ExecuteNonQuery();
                    }
                    else if (w_bustype.CompareTo("委外倒冲") == 0 || w_bustype.CompareTo("委外发料") == 0)
                    {
                        Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set fsendapplyqty=round(isnull(fsendapplyqty,0)+(0" + records11.iQuantity + "),6) where MOMaterialsID =0" + AllocateId;
                        Cmd.ExecuteNonQuery();
                    }
                    records11.iMaIDs = BodyData.Rows[i]["sq_autoid"] + "";
                }
                #endregion

                #region //项目目录
                string c_ItemClass = GetBodyValue_FromData(BodyData, i, "citemclass");
                if (c_ItemClass != "")  //存在项目目录
                {
                    string c_ItemCode = GetBodyValue_FromData(BodyData, i, "citemcode");
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..fitemss" + c_ItemClass + " where citemcode='" + c_ItemCode + "'", Cmd) == 0)
                        throw new Exception("项目[" + c_ItemCode + "]不存在");

                    records11.cItem_class = "'" + c_ItemClass + "'";
                    records11.cItemCName = "'" + U8Operation.GetDataString("select citem_name from " + dbname + "..fitem(nolock) where citem_class='" + c_ItemClass + "'", Cmd) + "'";
                    records11.cItemCode = "'" + c_ItemCode + "'";
                    records11.cName = "'" + U8Operation.GetDataString("select citemname from " + dbname + "..fitemss" + c_ItemClass + " where citemcode='" + c_ItemCode + "'", Cmd) + "'";
                }
                #endregion

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                records11.iNQuantity = records11.iQuantity;
                records11.iordertype = "0";
                if (bCreate)
                {
                    if (w_bustype.CompareTo("委外倒冲") == 0 || w_bustype.CompareTo("委外发料") == 0)
                    {
                        records11.iOMoDID = imodid;  //委外订单子表ID
                        records11.iOMoMID = AllocateId;  //委外用料表ID
                        records11.comcode = "'" + up_mo_code + "'";
                        records11.invcode = "'" + U8Operation.GetDataString("select cInvCode from " + dbname + "..OM_MODetails(nolock) where MODetailsID=0" + imodid, Cmd) + "'";
                    }
                    else
                    {
                        records11.cmocode = "'" + up_mo_code + "'";
                        records11.cpesocode = records11.cmocode;
                        records11.ipesotype = "7";
                        if (imodid.CompareTo("") != 0)
                        {
                            records11.iMPoIds = AllocateId;
                            records11.ipesodid = AllocateId;
                            records11.imoseq = "" + U8Operation.GetDataString("select sortseq from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd);
                            records11.ipesoseq = records11.imoseq;
                            records11.invcode = "'" + U8Operation.GetDataString("select invcode from " + dbname + "..mom_orderdetail(nolock) where modid=0" + imodid, Cmd) + "'";
                            if (U8Operation.GetDataString("select isnull(RelsUser,'') from " + dbname + "..mom_orderdetail a(nolock) inner join " + dbname + @"..mom_order b(nolock) on a.moid=b.moid where a.modid=0" + imodid, Cmd) == "")
                                throw new Exception("生产订单" + records11.cmocode + "未终审");
                        }
                        else
                        {
                            records11.iMPoIds = "''";
                            records11.ipesodid = "''";
                            records11.imoseq = "''";
                            records11.ipesoseq = records11.imoseq;
                            records11.invcode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                        }
                    }

                    //继承到货单的表体自由项  自定义项数据
                    if (imodid.CompareTo("") != 0)
                    {
                        if (w_bustype.CompareTo("委外倒冲") == 0 || w_bustype.CompareTo("委外发料") == 0)
                        {
                            dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1 free1,cfree2 free2,cfree3 free3,cfree4 free4,cfree5 free5,cfree6 free6,cfree7 free7,cfree8 free8,cfree9 free9,cfree10 free10,
                            cdefine22 define22,cdefine23 define23,cdefine24 define24,cdefine25 define25,cdefine26 define26,cdefine27 define27,
                            cdefine28 define28,cdefine29 define29,cdefine30 define30,cdefine31 define31,cdefine32 define32,cdefine33 define33,
                            cdefine34 define34,cdefine35 define35,cdefine36 define36,cdefine37 define37 from " + dbname + @"..OM_MOMaterials a(nolock) 
                        where MOMaterialsID=0" + AllocateId, "dtPuArr", Cmd);
                        }
                        else
                        {
                            dtPuArr = U8Operation.GetSqlDataTable(@"select free1,free2,free3,free4,free5,free6,free7,free8,free9,free10,
                            define22,define23,define24,define25,define26,define27,define28,define29,define30,define31,define32,define33,define34,
                            define35,define36,define37 from " + dbname + @"..mom_moallocate a(nolock) 
                        where AllocateId=0" + AllocateId + " and a.ByproductFlag=0", "dtPuArr", Cmd);
                        }
                        if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到订单用料信息");

                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define22") + "'";
                        if (records11.cDefine23.CompareTo("''") == 0) records11.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define23") + "'";
                        if (records11.cDefine24.CompareTo("''") == 0) records11.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define24") + "'";
                        if (records11.cDefine25.CompareTo("''") == 0) records11.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define25") + "'";

                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define28") + "'";
                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define29") + "'";
                        if (records11.cDefine30.CompareTo("''") == 0) records11.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define30") + "'";
                        if (records11.cDefine31.CompareTo("''") == 0) records11.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define31") + "'";
                        if (records11.cDefine32.CompareTo("''") == 0) records11.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define32") + "'";
                        if (records11.cDefine33.CompareTo("''") == 0) records11.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "define33") + "'";
                    }
                    else
                    {
                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine22 = "''";
                        if (records11.cDefine23.CompareTo("''") == 0) records11.cDefine23 = "''";
                        if (records11.cDefine24.CompareTo("''") == 0) records11.cDefine24 = "''";
                        if (records11.cDefine25.CompareTo("''") == 0) records11.cDefine25 = "''";

                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine28 = "''";
                        if (records11.cDefine22.CompareTo("''") == 0) records11.cDefine29 = "''";
                        if (records11.cDefine30.CompareTo("''") == 0) records11.cDefine30 = "''";
                        if (records11.cDefine31.CompareTo("''") == 0) records11.cDefine31 = "''";
                        if (records11.cDefine32.CompareTo("''") == 0) records11.cDefine32 = "''";
                        if (records11.cDefine33.CompareTo("''") == 0) records11.cDefine33 = "''";
                    }

                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos)
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "")  BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + records11.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records11.cInvCode + "的货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + record11.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                    records11.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                #endregion

                #region  //严格管控先进先出法
                if (c_firstTofisrt_Ctl.CompareTo("true") == 0 && records11.cBatch.CompareTo("''") != 0 && i_red_sheet == 0 && c_firstTofisrt_NotCtl_WareList.IndexOf("," + w_cwhcode + ",") < 0)
                {
                    //查询是否存在比当前批次更小的批次库存
                    string cLowBatch = "" + U8Operation.GetDataString("select top 1 cBatch from " + dbname + @"..CurrentStock(nolock) 
                        where cWhCode=" + record11.cWhCode + " and cInvCode=" + records11.cInvCode + " and cBatch<" + records11.cBatch + " and round(iQuantity," + iInv_DecDgt + ")>0 order by cBatch", Cmd);
                    if (cLowBatch != "")
                        throw new Exception("存货档案[" + BodyData.Rows[i]["cinvcode"] + "] 存在更小批次[" + cLowBatch + "],需先出库");
                }
                #endregion

                records11.cbMemo = "'" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "'";
                //保存数据
                if (!records11.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
                Cmd.CommandText = "update " + dbname + "..rdrecords11 set iQuantity=round(iQuantity,7) where autoid=0" + records11.AutoID;
                Cmd.ExecuteNonQuery();

                //CurrentStockCheck(record11.cWhCode, records11.cInvCode, records11.cvmivencode, records11.cBatch, records11.cFree1, records11.cFree2, records11.cFree3, records11.cFree4,
                //    records11.cFree5, records11.cFree6, records11.cFree7, records11.cFree8, records11.cFree9, records11.cFree10, Cmd, dbname);

                #region//货位账务处理
                if (b_Pos)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                        "Values (" + cAutoid + "," + rd_id + "," + record11.cWhCode + "," + records11.cPosition + "," + records11.cInvCode + "," + records11.iQuantity + "," + records11.iNum +
                        ",null," + record11.dDate + ",0,'',0," + record11.cVouchType + "," + record11.dDate + "," + record11.cMaker + "," + records11.cvmivencode + "," + records11.cBatch +
                        "," + records11.cFree1 + "," + records11.cFree2 + "," + records11.cFree3 + "," + records11.cFree4 + "," + records11.cFree5 + "," +
                        records11.cFree6 + "," + records11.cFree7 + "," + records11.cFree8 + "," + records11.cFree9 + "," + records11.cFree10 + ",''," + records11.cAssUnit + @",
                        " + records11.dMadeDate + "," + records11.iMassDate + "," + records11.cMassUnit + "," + records11.iExpiratDateCalcu + "," + records11.cExpirationdate + "," + records11.dExpirationdate + ")";
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    if (fU8Version >= 11)
                    {
                        Cmd.CommandText = "update " + dbname + "..rdrecords11 set iposflag=1 where autoid =0" + cAutoid;
                        Cmd.ExecuteNonQuery();
                    }
                    //修改货位库存
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record11.cWhCode + " and cvmivencode=" + records11.cvmivencode + " and cinvcode=" + records11.cInvCode + @" 
                    and cPosCode=" + records11.cPosition + " and cbatch=" + records11.cBatch + " and cfree1=" + records11.cFree1 + " and cfree2=" + records11.cFree2 + " and cfree3=" + records11.cFree3 + @" 
                    and cfree4=" + records11.cFree4 + " and cfree5=" + records11.cFree5 + " and cfree6=" + records11.cFree6 + " and cfree7=" + records11.cFree7 + @" 
                    and cfree8=" + records11.cFree8 + " and cfree9=" + records11.cFree9 + " and cfree10=" + records11.cFree10, Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                        values(" + record11.cWhCode + "," + records11.cPosition + "," + records11.cInvCode + ",0," + records11.cBatch + @",
                            " + records11.cFree1 + "," + records11.cFree2 + "," + records11.cFree3 + "," + records11.cFree4 + "," + records11.cFree5 + "," +
                                records11.cFree6 + "," + records11.cFree7 + "," + records11.cFree8 + "," + records11.cFree9 + "," + records11.cFree10 + "," + records11.cvmivencode + @",'',0,
                            " + records11.dMadeDate + "," + records11.iMassDate + "," + records11.cMassUnit + "," + records11.iExpiratDateCalcu + "," + records11.cExpirationdate + "," + records11.dExpirationdate + ")";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + records11.iQuantity + "),inum=isnull(inum,0)-(" + records11.iNum + @") 
                        where cwhcode=" + record11.cWhCode + " and cvmivencode=" + records11.cvmivencode + " and cinvcode=" + records11.cInvCode + @" 
                        and cPosCode=" + records11.cPosition + " and cbatch=" + records11.cBatch + " and cfree1=" + records11.cFree1 + " and cfree2=" + records11.cFree2 + " and cfree3=" + records11.cFree3 + @" 
                        and cfree4=" + records11.cFree4 + " and cfree5=" + records11.cFree5 + " and cfree6=" + records11.cFree6 + " and cfree7=" + records11.cFree7 + @" 
                        and cfree8=" + records11.cFree8 + " and cfree9=" + records11.cFree9 + " and cfree10=" + records11.cFree10;
                    Cmd.ExecuteNonQuery();

                    Current_Pos_StockCHeck(Cmd, dbname, record11.cWhCode, records11.cPosition, records11.cInvCode, records11.cBatch, records11.cvmivencode, records11.cFree1, records11.cFree2,
                        records11.cFree3, records11.cFree4, records11.cFree5, records11.cFree6, records11.cFree7, records11.cFree8, records11.cFree9, records11.cFree10);
                }
                #endregion

                #region  //是否超现存量出库
                Current_ST_StockCHeck(Cmd, dbname, record11.cWhCode, records11.cInvCode, records11.cBatch, records11.cvmivencode, records11.cFree1, records11.cFree2,
                    records11.cFree3, records11.cFree4, records11.cFree5, records11.cFree6, records11.cFree7, records11.cFree8, records11.cFree9, records11.cFree10);
                //Current_ST_StockAvableCHeck(Cmd, dbname, record11.cWhCode, records11.cInvCode, records11.cBatch, records11.cvmivencode, records11.cFree1, records11.cFree2,
                //    records11.cFree3, records11.cFree4, records11.cFree5, records11.cFree6, records11.cFree7, records11.cFree8, records11.cFree9, records11.cFree10);
                #endregion
            }


            #region//是否超生产订单检查  和  领料比例控制
            
            if (bCreate)
            {
                //生产领料
                if (AllocateId.CompareTo("") != 0)
                {
                    if (w_bustype.CompareTo("领料") == 0)
                    {
                        float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..rdrecords11(nolock) where iMPoIds=0" + AllocateId, Cmd));
                        //考虑是否领料总量为负（考虑红字材料出库单）
                        if (fckqty < 0) throw new Exception("保存后，子件存货【" + BodyData.Rows[i]["cinvcode"] + "】生产订单的实际领用数为负数");

                        #region //判断是否生产订单发料  0代表不能超   1 代表可超
                        if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='bOverMPOut'", Cmd) == 0)
                        {
                            //获得领用批量  
                            float f_piliang_qty = float.Parse(U8Operation.GetDataString(@"select isnull(iDrawBatch,0) from " + dbname + @"..Inventory(nolock) 
                            where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd));
                            if (f_piliang_qty == 0) f_piliang_qty = 0.0000001F;
                            //关键领用控制量，取最小值  
                            float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select qty from " + dbname + "..mom_moallocate(nolock) where AllocateId=0" + AllocateId, Cmd));
                            if (Math.Round(f_ll_qty - fckqty, iInv_DecDgt) < -1 * f_piliang_qty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超生产订单出库");
                        }
                        #endregion
                    }//委外领料 
                    else if (w_bustype.CompareTo("委外发料") == 0)
                    {
                        float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..rdrecords11(nolock) where iOMoMID=0" + AllocateId, Cmd));
                        //考虑是否领料总量为负（考虑红字材料出库单）
                        if (fckqty < 0) throw new Exception("保存后，子件存货【" + BodyData.Rows[i]["cinvcode"] + "】超委外订的实际领用数为负数");

                        #region //判断是否委外订单发料  0代表不能超   1 代表可超
                        if (decimal.Parse("" + BodyData.Rows[i]["iquantity"]) > 0 &&
                            U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='ST' and cname='bOverCommissionOut'", Cmd) == 0)
                        {
                            //获得领用批量  
                            float f_piliang_qty = float.Parse(U8Operation.GetDataString(@"select isnull(iDrawBatch,0) from " + dbname + @"..Inventory(nolock) 
                            where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd));
                            if (f_piliang_qty == 0) f_piliang_qty = 0.0000001F;
                            //关键领用控制量，取最小值    
                            float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iQuantity from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + AllocateId, Cmd));
                            if (Math.Round(f_ll_qty - fckqty, iInv_DecDgt) < -1 * f_piliang_qty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超委外订单出库");
                        }
                        #endregion

                        #region //判断是否委外齐套订单发料
                        if (decimal.Parse("" + BodyData.Rows[i]["iquantity"]) < 0)
                        {
                            int i_check_mer_out = U8Operation.GetDataInt("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cSysID = 'OM' and cname='iOMControlTypeOfIn'", Cmd);
                            //判断是否存在非倒冲记录  
                            DataTable dtMer_out = U8Operation.GetSqlDataTable(@"select top 1 a.cInvCode,min(isnull(a.iSendQTY,0)*b.iQuantity/a.iQuantity) + 0.001 low_qty
                        from " + dbname + "..OM_MOMaterials a(nolock) inner join " + dbname + @"..OM_MODetails b(nolock) on a.MoDetailsID=b.MODetailsID
                        where a.MoDetailsID=0" + imodid + " and a.iWIPtype=3 and a.iQuantity<>0 and a.cInvCode='" + BodyData.Rows[i]["cinvcode"] + @"' 
                        group by a.cInvCode order by low_qty", "dtMer_out", Cmd);

                            if (i_check_mer_out == 2 && dtMer_out.Rows.Count > 0)
                            {
                                decimal d_om_qty = decimal.Parse("" + dtMer_out.Rows[0]["low_qty"]);
                                decimal d_rd01_qty = decimal.Parse(U8Operation.GetDataString("select isnull(sum(iquantity),0) from " + dbname + @"..rdrecords01 where iOMoDID=0" + imodid, Cmd));
                                if (d_om_qty < d_rd01_qty) throw new Exception("材料[" + BodyData.Rows[i]["cinvcode"] + "]退料后领用不足");
                            }
                        }
                        #endregion
                    }
                }
                if (b_MerAPP)
                {
                    //判断是否超申请出库
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..MaterialAppVouchs(nolock) where autoid=0" + BodyData.Rows[i]["sq_autoid"] + " and (ISNULL(fOutQuantity,0)>iQuantity or ISNULL(fOutQuantity,0)<0)", Cmd) > 0)
                        throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超申请单出库,或者累计出库数小于0");
                }

            }
            #endregion

        }

        #endregion
        #endregion
        if (b_retID)
        {
            StringBuilder b = new StringBuilder();
            string s = "";
            for (int n = 0; n < list_moallocate.Count; n++)
            {
                s = list_moallocate[n].ToString();
                b.Append(s);
                b.Append(",0");
            }

            System.Data.DataTable dtMoallocate = U8Operation.GetSqlDataTable("select AllocateId,MoDId from " + dbname + @"..mom_moallocate where MoDId = " + imodid + " and AllocateId in (" + b.ToString() + ")", "dtMoallocate", Cmd);
            ret_moallocate = ToJson(dtMoallocate);
            return rd_id + "," + cc_mcode + ",Message:" + ret_moallocate;
        }
        else
        {
            return rd_id + "," + cc_mcode;
        }
    }

    [WebMethod]  //审核调拨单
    public bool U8TranVouchCheck_Method(string M_Vouch_ID, string dbname, string cUserName, string cLogDate)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) { if (st_value != U8Operation.GetDataString2(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误"); }
        #endregion

        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            bool b_ret = TransVouchChecked(M_Vouch_ID, dbname, cUserName, cLogDate, Cmd);

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

    [WebMethod]  //审核发货单
    public bool U8DispVouchCheck_Method(string M_Vouch_ID, string dbname, string cUserName, string cLogDate)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"];
        if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) { if (st_value != U8Operation.GetDataString2(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误"); }
        #endregion

        SqlConnection Conn = U8Operation.OpenDataConnection();
        SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            bool b_ret = DispVouchChecked(M_Vouch_ID, dbname, cUserName, cLogDate, Cmd);

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

    //调拨单 [销售订单调拨 库房直接调拨]
    public string U81017(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        //transvouchs.AppTransIDS   存放销售订单 子表id
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cindepcode = ""; string w_cpersoncode = ""; string w_inwhcode = "";
        string w_rdcode = ""; string w_in_rdcode = ""; //出库类型
        string w_bustype = "null";//业务类型
        string w_headPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_in_Vmi = false; bool b_Pos = false; bool b_in_Pos = false;//货位
        string isoid = ""; string iSOsID = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_autocheck_type = 0;// 是否自动审核，0 代表执行通用控制参数；1 代表本单自动审核；2 代表本单不需要审核
        bool b_has_AppTrans = false;//是否存在调拨申请单
        bool b_out_appvouch_tran = false; //是否超调拨申请 调拨
        #region  //逻辑检验
        //自动审核 规则
        string c_autocheckvalue = GetTextsFrom_FormData_Tag(HeadData, "txt_i_isauto_check");
        if (c_autocheckvalue == "1" || c_autocheckvalue == "2")
        {
            i_autocheck_type = int.Parse(c_autocheckvalue);
        }

        if (BodyData.Columns.Contains("itrids")) b_has_AppTrans = true; //存在调拨申请单
        if (BodyData.Columns.Contains("isosid"))
        {
            iSOsID = BodyData.Rows[0]["isosid"] + "";
            if (iSOsID.CompareTo("") != 0)
            {
                bCreate = true;
                isoid = U8Operation.GetDataString("select ID from " + dbname + "..SO_SODetails(nolock) where iSOsID=0" + iSOsID, Cmd);
                if (isoid == "") throw new Exception("没有找到销售订单信息");
            }
        }
        
        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style(nolock) where cRDCode='" + w_rdcode + "' and  bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误，或非末级");
        }
        //入库类别
        w_in_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinrdcode");
        if (w_in_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style(nolock) where cRDCode='" + w_in_rdcode + "' and  bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误，或非末级");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("调出仓库输入错误");
        w_inwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "'", Cmd) == 0)
            throw new Exception("调入仓库输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("调出部门输入错误");
        }
        w_cindepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cindepcode");
        if (w_cindepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cindepcode + "'", Cmd) == 0)
                throw new Exception("调入部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person(nolock) where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a(nolock) 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b(nolock) on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0304' and VT_TemplateMode=0", Cmd);
        if (U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation(nolock) where cSysID='ST' and cName='bOverTransRequestTransfer'", Cmd).ToLower().CompareTo("true") == 0)
        {
            b_out_appvouch_tran = true;
        }

        #region //新增单据
        KK_U8Com.U8TransVouch tranmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
        Cmd.ExecuteNonQuery();
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0) from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd);
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if(trans_code=="")
        {
            string cCodeHead = "T" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cTVCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..TransVouch(nolock) where cTVCode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        tranmain.cTVCode = "'" + cc_mcode + "'";
        tranmain.ID = rd_id;
        tranmain.cOWhCode = "'" + w_cwhcode + "'";
        tranmain.cORdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        tranmain.cODepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        tranmain.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        tranmain.cIWhCode = "'" + w_inwhcode + "'";
        tranmain.cIRdCode = (w_in_rdcode.CompareTo("") == 0 ? "null" : "'" + w_in_rdcode + "'");
        tranmain.cIDepCode = (w_cindepcode.CompareTo("") == 0 ? "null" : "'" + w_cindepcode + "'");
        tranmain.VT_ID = vt_id;

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_in_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "' and bWhPos=1", Cmd) > 0)
            b_in_Pos = true;

        tranmain.cMaker = "'" + cUserName + "'";
        tranmain.dTVDate = "'" + cLogDate + "'";
        tranmain.cTVMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_ctvmemo") + "'";
        tranmain.IsWfControlled = 0;

        #region   //主表自定义项处理
        tranmain.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        tranmain.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        tranmain.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        tranmain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        tranmain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        tranmain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        tranmain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        tranmain.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        tranmain.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        tranmain.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        tranmain.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        tranmain.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        tranmain.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        tranmain.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        tranmain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        tranmain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        #region //表头上游单据 继承
        if (bCreate)
        {
            DataTable dtMainDef = U8Operation.GetSqlDataTable(@"select cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,
                    cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 from " + dbname + @"..SO_SOMain a(nolock) 
                where ID=0" + isoid, "dtMainDef", Cmd);
            if (dtMainDef.Rows.Count == 0) throw new Exception("没有找到销售订单主要信息");

            if (tranmain.cDefine1.CompareTo("''") == 0) tranmain.cDefine1 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine1") + "'";
            if (tranmain.cDefine2.CompareTo("''") == 0) tranmain.cDefine2 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine2") + "'";
            if (tranmain.cDefine3.CompareTo("''") == 0) tranmain.cDefine3 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine3") + "'";

            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine4");  //日期
            if (tranmain.cDefine4.CompareTo("''") == 0 || tranmain.cDefine4.CompareTo("null") == 0) tranmain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine5");

            if (tranmain.cDefine5 == null) tranmain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? int.Parse("0") : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine6");  //日期
            if (tranmain.cDefine6.CompareTo("''") == 0 || tranmain.cDefine6.CompareTo("null") == 0) tranmain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine7");
            if (tranmain.cDefine7 == null) tranmain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));

            if (tranmain.cDefine8.CompareTo("''") == 0) tranmain.cDefine8 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine8") + "'";
            if (tranmain.cDefine9.CompareTo("''") == 0) tranmain.cDefine9 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine9") + "'";
            if (tranmain.cDefine10.CompareTo("''") == 0) tranmain.cDefine10 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine10") + "'";
            if (tranmain.cDefine11.CompareTo("''") == 0) tranmain.cDefine11 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine11") + "'";
            if (tranmain.cDefine12.CompareTo("''") == 0) tranmain.cDefine12 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine12") + "'";
            if (tranmain.cDefine13.CompareTo("''") == 0) tranmain.cDefine13 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine13") + "'";
            if (tranmain.cDefine14.CompareTo("''") == 0) tranmain.cDefine14 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine14") + "'";

            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine15");
            if (tranmain.cDefine15 == null) tranmain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine16");
            if (tranmain.cDefine16 == null) tranmain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        }
        #endregion

        if (!tranmain.InsertToDB(targetAccId,false , ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region //子表
        int irdrow = 0;
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("调拨数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception("["+BodyData.Rows[i]["cinvcode"] + "]存货编码不存在");

            //代管验证
            if ((b_Vmi || b_in_Vmi) && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库 必须有代管商");

            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string chkValue = U8Operation.GetDataString("select id from " + dbname + "..SO_SODetails a(nolock) where iSOsID=0" + BodyData.Rows[i]["iSOsID"], Cmd);
                if (chkValue.CompareTo("") == 0) throw new Exception("生单模式必须根据销售订单调拨,请确认扫描的销售订单存在");
            }

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            if (bCreate)
            {
                iSOsID = BodyData.Rows[i]["isosid"] + "";
                isoid = U8Operation.GetDataString("select ID from " + dbname + "..SO_SODetails(nolock) where iSOsID=0" + iSOsID, Cmd);
            }
            else
            {
                iSOsID = ""; isoid = "";
            }
            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            if (c_body_batch.CompareTo("") == 0 && cfirstToFirst.CompareTo("auto") == 0)  //没有批次且执行自动制定批次出库
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, BodyData.Rows[i]["cposcode"] + "",
                    GetBodyValue_FromData(BodyData, i, "cfree1"), GetBodyValue_FromData(BodyData, i, "cfree2"), GetBodyValue_FromData(BodyData, i, "cfree3"),
                    GetBodyValue_FromData(BodyData, i, "cfree4"), GetBodyValue_FromData(BodyData, i, "cfree5"), GetBodyValue_FromData(BodyData, i, "cfree6"),
                    GetBodyValue_FromData(BodyData, i, "cfree7"), GetBodyValue_FromData(BodyData, i, "cfree8"), GetBodyValue_FromData(BodyData, i, "cfree9"),
                    GetBodyValue_FromData(BodyData, i, "cfree10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            #endregion

            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                KK_U8Com.U8TransVouchs trandetail = new KK_U8Com.U8TransVouchs(Cmd, dbname);
                int cAutoid = U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd);
                trandetail.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();

                trandetail.ID = rd_id;
                trandetail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                trandetail.iTVQuantity = "" + dtMer.Rows[m]["iquantity"];
                trandetail.irowno = (i + 1);
                irdrow++;
                trandetail.irowno = irdrow;
                trandetail.cTVCode = tranmain.cTVCode;
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + "", Cmd);

                #region //自由项  自定义项处理
                trandetail.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                trandetail.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                trandetail.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                trandetail.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                trandetail.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                trandetail.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                trandetail.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                trandetail.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                trandetail.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                trandetail.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                trandetail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
                trandetail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
                trandetail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
                trandetail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
                trandetail.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
                trandetail.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                trandetail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
                trandetail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
                trandetail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
                trandetail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
                trandetail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
                trandetail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
                trandetail.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
                trandetail.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
                trandetail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
                trandetail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                #endregion

                #region //获得单价与金额
                string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                #endregion

                #region //代管挂账处理
                trandetail.cvmivencode = "''";
                trandetail.bCosting = 1;
                if (b_Vmi)
                {
                    //trandetail.bVMIUsed = "1";
                    trandetail.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理  (含批次档案处理)
                trandetail.cTVBatch = "''";
                trandetail.iExpiratDateCalcu = 0;
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" == "") throw new Exception(trandetail.cInvCode + "有批次管理，必须输入批号");
                    trandetail.cTVBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                    //保质期管理
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + " and bInvQuality=1", Cmd) > 0)
                    {
                        trandetail.iExpiratDateCalcu = int.Parse(dtMer.Rows[m]["有效期推算方式"] + "");
                        trandetail.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                        trandetail.dDisDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        trandetail.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        trandetail.dMadeDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                        trandetail.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                    }

                    //批次档案 建档
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + trandetail.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                    {
                        DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + trandetail.cInvCode + " and cBatch=" + trandetail.cTVBatch + " and isnull(cFree1,'')=" + trandetail.cFree1 + @" 
                                and isnull(cFree2,'')=" + trandetail.cFree2 + " and isnull(cFree3,'')=" + trandetail.cFree3 + " and isnull(cFree4,'')=" + trandetail.cFree4 + @" 
                                and isnull(cFree5,'')=" + trandetail.cFree5 + " and isnull(cFree6,'')=" + trandetail.cFree6 + " and isnull(cFree7,'')=" + trandetail.cFree7 + @" 
                                and isnull(cFree8,'')=" + trandetail.cFree8 + " and isnull(cFree9,'')=" + trandetail.cFree9 + " and isnull(cFree10,'')=" + trandetail.cFree10, "dtBatPerp", Cmd);
                        if (dtBatPerp.Rows.Count > 0)
                        {
                            trandetail.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                            trandetail.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                            trandetail.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                            trandetail.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                            trandetail.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                            trandetail.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                            trandetail.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                            trandetail.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                            trandetail.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                            trandetail.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                        }
                    }
                }
                #endregion

                #region //换算率（多计量） 和 回写到生产单
                trandetail.iTVNum = "0";
                string db_is_updateout_qty = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_db_is_updatesodetail_out_qty'", Cmd);
                string u8barcode_db_so_lj_qty = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_db_so_lj_qty'", Cmd);
                db_is_updateout_qty = db_is_updateout_qty.ToLower();
                if (u8barcode_db_so_lj_qty == "") u8barcode_db_so_lj_qty = "iFHQuantity";

                if (cst_unitcode.CompareTo("") != 0)
                {
                    trandetail.cAssUnit = "'" + cst_unitcode + "'";
                    string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + trandetail.cAssUnit, Cmd);
                    if (ichange == "") ichange = "0";
                    if (float.Parse(ichange) == 0)
                    {
                        inum = GetBodyValue_FromData(BodyData, i, "inum");
                        if (inum == "")
                        {
                            inum = "0";
                            //if (float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                            trandetail.iinvexchrate = "null";
                        }
                        else
                        {
                            //浮动换算率
                            ichange = U8Operation.GetDataString("select round(" + trandetail.iTVQuantity + "/" + inum + ",5)", Cmd);
                            trandetail.iinvexchrate = ichange;
                        }
                        trandetail.iTVNum = inum;
                    }
                    else
                    {
                        //固定换算率
                        trandetail.iinvexchrate = ichange;
                        inum = U8Operation.GetDataString("select round(" + trandetail.iTVQuantity + "/" + ichange + ",5)", Cmd);
                        trandetail.iTVNum = inum;
                    }

                    if (u8barcode_db_so_lj_qty.ToLower().CompareTo("ifhquantity") == 0 && bCreate)  //回写订单累计件数
                    {
                        if (db_is_updateout_qty.CompareTo("true") == 0)
                        {
                            Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHNum=isnull(iFHNum,0)+(0" + inum + "),foutnum=isnull(foutnum,0)+(0" + inum + ") where iSOsID =0" + iSOsID;
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHNum=isnull(iFHNum,0)+(0" + inum + ") where iSOsID =0" + iSOsID;
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }

                //回写到销售订单用量表累计发货数量
                if (bCreate)
                {
                    if (db_is_updateout_qty.CompareTo("true") == 0 && u8barcode_db_so_lj_qty.ToLower().CompareTo("ifhquantity") == 0)
                    {
                        Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHQuantity=isnull(iFHQuantity,0)+(0" + trandetail.iTVQuantity + "),foutquantity=isnull(foutquantity,0)+(0" + trandetail.iTVQuantity + ") where iSOsID =0" + iSOsID;
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Cmd.CommandText = "update " + dbname + "..SO_SODetails set " + u8barcode_db_so_lj_qty + "=cast(isnull(" + u8barcode_db_so_lj_qty + ",0.0) as float)+(0" + trandetail.iTVQuantity + ") where iSOsID =0" + iSOsID;
                        Cmd.ExecuteNonQuery();
                    }
                }

                #endregion

                #region //回写调拨申请单
                if (b_has_AppTrans && BodyData.Rows[i]["itrids"] + "" != "")
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..ST_AppTransVouchs where autoid=" + BodyData.Rows[i]["itrids"], Cmd) == 0)
                    {
                        throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]的调拨申请单不存在");
                    }

                    Cmd.CommandText = "update " + dbname + "..ST_AppTransVouchs set iTvSumQuantity=isnull(iTvSumQuantity,0)+(0" + trandetail.iTVQuantity + @"),
                            iTVSumNum=isnull(iTVSumNum,0)+(0" + (trandetail.iTVNum == "null" ? "0" : trandetail.iTVNum) + @")
                        where autoid=" + BodyData.Rows[i]["itrids"];
                    Cmd.ExecuteNonQuery();

                    //判断调拨申请单是否超出 申请量
                    if (!b_out_appvouch_tran)
                    {
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..ST_AppTransVouchs 
                            where autoid=" + BodyData.Rows[i]["itrids"] + " and isnull(iTvSumQuantity,0)>isnull(iTvChkQuantity,0)+0.005", Cmd) > 0)
                        {
                            throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超调拨申请单数量调拨");
                        }
                    }

                    trandetail.iTRIds = BodyData.Rows[i]["itrids"] + "";
                }
                #endregion 

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                if (bCreate)
                {
                    trandetail.AppTransIDS = iSOsID; //销售订单isosid

                    if (U8Operation.GetDataString("select isnull(cVerifier,'') from " + dbname + "..SO_SOMain a(nolock) where id=0" + isoid, Cmd) == "")
                        throw new Exception("存货" + trandetail.cInvCode + "的销售订单未终审");
                    if (U8Operation.GetDataString("select isnull(cSCloser,'') from " + dbname + "..SO_SODetails a(nolock) where iSOsID=0" + iSOsID, Cmd) != "")
                        throw new Exception("存货" + trandetail.cInvCode + "的销售订单已关闭");

                    //继承到货单的表体自由项  自定义项数据
                    dtPuArr = U8Operation.GetSqlDataTable(@"select id,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..SO_SODetails a(nolock) 
                        where iSOsID=0" + iSOsID, "dtPuArr", Cmd);
                    if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到销售订单信息");

                    if (trandetail.cDefine22.CompareTo("''") == 0) trandetail.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                    if (trandetail.cDefine23.CompareTo("''") == 0) trandetail.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                    if (trandetail.cDefine24.CompareTo("''") == 0) trandetail.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                    if (trandetail.cDefine25.CompareTo("''") == 0) trandetail.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                    if (trandetail.cDefine28.CompareTo("''") == 0) trandetail.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                    if (trandetail.cDefine29.CompareTo("''") == 0) trandetail.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                    if (trandetail.cDefine30.CompareTo("''") == 0) trandetail.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                    if (trandetail.cDefine31.CompareTo("''") == 0) trandetail.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                    if (trandetail.cDefine32.CompareTo("''") == 0) trandetail.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                    if (trandetail.cDefine33.CompareTo("''") == 0) trandetail.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos) //调出
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("调出仓库有货位管理，" + trandetail.cInvCode + "的调出货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + tranmain.cOWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于调出仓库");
                    trandetail.coutposcode = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }

                if (b_in_Pos)//调入
                {
                    if (BodyData.Rows[i]["cinposcode"] + "" == "" && w_headPosCode != "") BodyData.Rows[i]["cinposcode"] = w_headPosCode;
                    if (BodyData.Rows[i]["cinposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cinposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cinposcode"] + "" == "") throw new Exception("调入仓库有货位管理，" + trandetail.cInvCode + "的调入货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + tranmain.cIWhCode + " and cposcode='" + BodyData.Rows[i]["cinposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cinposcode"] + "】不存在，或者不属于调入仓库");
                    trandetail.cinposcode = "'" + BodyData.Rows[i]["cinposcode"] + "'";
                }
                
                #endregion

                //保存数据
                if (!trandetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region //补充版本信息
                if (fU8Version >= 11)
                {
                    string cInVoucherCode = U8Operation.GetDataString("select cSOCode from " + dbname + "..SO_SOMain(nolock) where id=0" + isoid, Cmd);

                    if (c_ppcost != "")
                    {
                        Cmd.CommandText = "update " + dbname + "..TransVouchs set cInVoucherCode='" + cInVoucherCode + "',cInVoucherLineID=0" + iSOsID + @",cInVoucherType='124',
                            iTVPCost=" + c_ppcost + ", iTVPPrice=" + (decimal.Parse(c_ppcost) * decimal.Parse("" + dtMer.Rows[m]["iquantity"])) + @"
                        where autoID=0" + trandetail.AutoID;
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Cmd.CommandText = "update " + dbname + "..TransVouchs set cInVoucherCode='" + cInVoucherCode + "',cInVoucherLineID=0" + iSOsID + @",cInVoucherType='124' 
                        where autoID=0" + trandetail.AutoID;
                        Cmd.ExecuteNonQuery();
                    }
                }
                #endregion

                #region//货位账务处理  (调拨单不处理货位业务)
                //                if (b_Pos)
//                {
//                    //添加货位记录 
//                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
//                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
//                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
//                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
//                        "Values (" + cAutoid + "," + rd_id + "," + tranmain.cWhCode + "," + trandetail.coutposcode + "," + trandetail.cInvCode + "," + trandetail.iTVQuantity + "," + trandetail.iTVNum +
//                        ",null," + tranmain.dDate + ",0,'',0," + tranmain.cVouchType + "," + tranmain.dDate + "," + tranmain.cMaker + "," + trandetail.cvmivencode + "," + trandetail.cBatch +
//                        "," + trandetail.cFree1 + "," + trandetail.cFree2 + "," + trandetail.cFree3 + "," + trandetail.cFree4 + "," + trandetail.cFree5 + "," +
//                        trandetail.cFree6 + "," + trandetail.cFree7 + "," + trandetail.cFree8 + "," + trandetail.cFree9 + "," + trandetail.cFree10 + ",''," + trandetail.cAssUnit + @",
//                        " + trandetail.dMadeDate + "," + trandetail.iMassDate + "," + trandetail.cMassUnit + "," + trandetail.iExpiratDateCalcu + "," + trandetail.cExpirationdate + "," + trandetail.dExpirationdate + ")";
//                    Cmd.ExecuteNonQuery();

//                    //修改货位库存
//                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + tranmain.cWhCode + " and cvmivencode=" + trandetail.cvmivencode + " and cinvcode=" + trandetail.cInvCode + @" 
//                    and cPosCode=" + trandetail.coutposcode + " and cbatch=" + trandetail.cBatch + " and cfree1=" + trandetail.cFree1 + " and cfree2=" + trandetail.cFree2 + " and cfree3=" + trandetail.cFree3 + @" 
//                    and cfree4=" + trandetail.cFree4 + " and cfree5=" + trandetail.cFree5 + " and cfree6=" + trandetail.cFree6 + " and cfree7=" + trandetail.cFree7 + @" 
//                    and cfree8=" + trandetail.cFree8 + " and cfree9=" + trandetail.cFree9 + " and cfree10=" + trandetail.cFree10, Cmd) == 0)
//                    {
//                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
//                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
//                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
//                        values(" + tranmain.cWhCode + "," + trandetail.coutposcode + "," + trandetail.cInvCode + ",0," + trandetail.cBatch + @",
//                            " + trandetail.cFree1 + "," + trandetail.cFree2 + "," + trandetail.cFree3 + "," + trandetail.cFree4 + "," + trandetail.cFree5 + "," +
//                                trandetail.cFree6 + "," + trandetail.cFree7 + "," + trandetail.cFree8 + "," + trandetail.cFree9 + "," + trandetail.cFree10 + "," + trandetail.cvmivencode + @",'',0,
//                            " + trandetail.dMadeDate + "," + trandetail.iMassDate + "," + trandetail.cMassUnit + "," + trandetail.iExpiratDateCalcu + "," + trandetail.cExpirationdate + "," + trandetail.dExpirationdate + ")";
//                        Cmd.ExecuteNonQuery();
//                    }
//                    Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + trandetail.iTVQuantity + "),inum=isnull(inum,0)-(" + trandetail.iTVNum + @") 
//                        where cwhcode=" + tranmain.cWhCode + " and cvmivencode=" + trandetail.cvmivencode + " and cinvcode=" + trandetail.cInvCode + @" 
//                        and cPosCode=" + trandetail.coutposcode + " and cbatch=" + trandetail.cBatch + " and cfree1=" + trandetail.cFree1 + " and cfree2=" + trandetail.cFree2 + " and cfree3=" + trandetail.cFree3 + @" 
//                        and cfree4=" + trandetail.cFree4 + " and cfree5=" + trandetail.cFree5 + " and cfree6=" + trandetail.cFree6 + " and cfree7=" + trandetail.cFree7 + @" 
//                        and cfree8=" + trandetail.cFree8 + " and cfree9=" + trandetail.cFree9 + " and cfree10=" + trandetail.cFree10;
//                    Cmd.ExecuteNonQuery();
//                }


//                if (b_in_Pos)
//                {
//                    //添加货位记录 
//                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
//                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
//                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
//                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
//                        "Values (" + cAutoid + "," + rd_id + "," + tranmain.cIWhCode + "," + trandetail.cinposcode + "," + trandetail.cInvCode + "," + trandetail.iTVQuantity + "," + trandetail.iTVNum +
//                        ",null," + tranmain.dDate + ",1,'',0," + tranmain.cVouchType + "," + tranmain.dDate + "," + tranmain.cMaker + "," + trandetail.cvmivencode + "," + trandetail.cBatch +
//                        "," + trandetail.cFree1 + "," + trandetail.cFree2 + "," + trandetail.cFree3 + "," + trandetail.cFree4 + "," + trandetail.cFree5 + "," +
//                        trandetail.cFree6 + "," + trandetail.cFree7 + "," + trandetail.cFree8 + "," + trandetail.cFree9 + "," + trandetail.cFree10 + ",''," + trandetail.cAssUnit + @",
//                        " + trandetail.dMadeDate + "," + trandetail.iMassDate + "," + trandetail.cMassUnit + "," + trandetail.iExpiratDateCalcu + "," + trandetail.cExpirationdate + "," + trandetail.dExpirationdate + ")";
//                    Cmd.ExecuteNonQuery();

//                    //修改货位库存
//                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum where cwhcode=" + tranmain.cIWhCode + " and cvmivencode=" + trandetail.cvmivencode + " and cinvcode=" + trandetail.cInvCode + @" 
//                    and cPosCode=" + trandetail.cinposcode + " and cbatch=" + trandetail.cBatch + " and cfree1=" + trandetail.cFree1 + " and cfree2=" + trandetail.cFree2 + " and cfree3=" + trandetail.cFree3 + @" 
//                    and cfree4=" + trandetail.cFree4 + " and cfree5=" + trandetail.cFree5 + " and cfree6=" + trandetail.cFree6 + " and cfree7=" + trandetail.cFree7 + @" 
//                    and cfree8=" + trandetail.cFree8 + " and cfree9=" + trandetail.cFree9 + " and cfree10=" + trandetail.cFree10, Cmd) == 0)
//                    {
//                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
//                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
//                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
//                        values(" + tranmain.cIWhCode + "," + trandetail.cinposcode + "," + trandetail.cInvCode + ",0," + trandetail.cBatch + @",
//                            " + trandetail.cFree1 + "," + trandetail.cFree2 + "," + trandetail.cFree3 + "," + trandetail.cFree4 + "," + trandetail.cFree5 + "," +
//                                trandetail.cFree6 + "," + trandetail.cFree7 + "," + trandetail.cFree8 + "," + trandetail.cFree9 + "," + trandetail.cFree10 + "," + trandetail.cvmivencode + @",'',0,
//                            " + trandetail.dMadeDate + "," + trandetail.iMassDate + "," + trandetail.cMassUnit + "," + trandetail.iExpiratDateCalcu + "," + trandetail.cExpirationdate + "," + trandetail.dExpirationdate + ")";
//                        Cmd.ExecuteNonQuery();
//                    }
//                    Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + trandetail.iTVQuantity + "),inum=isnull(inum,0)+(" + trandetail.iTVNum + @") 
//                        where cwhcode=" + tranmain.cIWhCode + " and cvmivencode=" + trandetail.cvmivencode + " and cinvcode=" + trandetail.cInvCode + @" 
//                        and cPosCode=" + trandetail.cinposcode + " and cbatch=" + trandetail.cBatch + " and cfree1=" + trandetail.cFree1 + " and cfree2=" + trandetail.cFree2 + " and cfree3=" + trandetail.cFree3 + @" 
//                        and cfree4=" + trandetail.cFree4 + " and cfree5=" + trandetail.cFree5 + " and cfree6=" + trandetail.cFree6 + " and cfree7=" + trandetail.cFree7 + @" 
//                        and cfree8=" + trandetail.cFree8 + " and cfree9=" + trandetail.cFree9 + " and cfree10=" + trandetail.cFree10;
//                    Cmd.ExecuteNonQuery();
//                }
                #endregion

            }


            #region//是否超销售订单检查
            if (bCreate)
            {
                float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(itvquantity),0) from " + dbname + "..TransVouchs(nolock) where AppTransIDS=0" + iSOsID, Cmd));
                #region //判断是否订单发货  0代表不能超   1 代表可超
                if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='sa' and cname='bOverOrder'", Cmd) == 1)
                {
                    //关键领用控制量，取最小值  
                    float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iquantity from " + dbname + "..SO_SODetails(nolock) where iSOsID=0" + iSOsID, Cmd));
                    if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超销售订单发货");
                }
                #endregion

            }
            #endregion

        }

        #endregion
        #endregion

        #region //调拨在途量处理
//        //增加调拨待发量
//        DataTable dtStk = U8Operation.GetSqlDataTable(@"select cOWhCode,cIWhCode,b.cinvcode,b.cTVBatch,b.iTVQuantity,iTVNum,b.cvmivencode,
//                    b.cfree1,b.cfree2,b.cfree3,b.cfree4,b.cfree5,b.cfree6,b.cfree7,b.cfree8,b.cfree9,b.cFree10
//                from " + dbname + "..transvouch a inner join " + dbname + "..transvouchs b on a.id=b.id where a.id=" + tranmain.ID, "dtStk", Cmd);
//        for (int c = 0; c < dtStk.Rows.Count; c++)
//        {
//            U8V10SetCurrentStockRow(Cmd, dbname + "..", "" + dtStk.Rows[c]["cOWhCode"], "" + dtStk.Rows[c]["cinvcode"], "" + dtStk.Rows[c]["cfree1"], "" + dtStk.Rows[c]["cfree2"],
//                "" + dtStk.Rows[c]["cfree3"], "" + dtStk.Rows[c]["cfree4"], "" + dtStk.Rows[c]["cfree5"], "" + dtStk.Rows[c]["cfree6"], "" + dtStk.Rows[c]["cfree7"],
//                "" + dtStk.Rows[c]["cfree8"], "" + dtStk.Rows[c]["cfree9"], "" + dtStk.Rows[c]["cfree10"], "" + dtStk.Rows[c]["cTVBatch"], "" + dtStk.Rows[c]["cvmivencode"]);
//            U8V10SetCurrentStockRow(Cmd, dbname + "..", "" + dtStk.Rows[c]["cIWhCode"], "" + dtStk.Rows[c]["cinvcode"], "" + dtStk.Rows[c]["cfree1"], "" + dtStk.Rows[c]["cfree2"],
//                "" + dtStk.Rows[c]["cfree3"], "" + dtStk.Rows[c]["cfree4"], "" + dtStk.Rows[c]["cfree5"], "" + dtStk.Rows[c]["cfree6"], "" + dtStk.Rows[c]["cfree7"],
//                "" + dtStk.Rows[c]["cfree8"], "" + dtStk.Rows[c]["cfree9"], "" + dtStk.Rows[c]["cfree10"], "" + dtStk.Rows[c]["cTVBatch"], "" + dtStk.Rows[c]["cvmivencode"]);
//            //更新数量 fTransOutNum
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)+(0" + dtStk.Rows[c]["iTVQuantity"] + @"),
//                        fTransOutNum=isnull(fTransOutNum,0)+(0" + dtStk.Rows[c]["iTVNum"] + @") 
//                    where cwhcode='" + dtStk.Rows[c]["cOWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();

//            //更新数量
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)+(0" + dtStk.Rows[c]["iTVQuantity"] + @") ,
//                        fTransInNum=isnull(fTransInNum,0)+(0" + dtStk.Rows[c]["iTVNum"] + @") 
//                    where cwhcode='" + dtStk.Rows[c]["cIWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();

//            #region  //检查可用量
//            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..currentstock 
//                    where cwhcode='" + dtStk.Rows[c]["cOWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + @"' and iSoType=0 and iSodid='' 
//                        and iQuantity+fInQuantity+fTransInQuantity-fOutQuantity-fTransOutQuantity<0", Cmd) > 0)
//            {
//                throw new Exception("仓库[" + dtStk.Rows[c]["cOWhCode"] + "]存货[" + dtStk.Rows[c]["cinvcode"] + "]批号[" + dtStk.Rows[c]["cTVBatch"] + "]超可用量");
//            }
//            #endregion
//        }
        #endregion


        #region   //审核调拨单
        string cDBVouchAutoCHeck = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='mes_cDBVouchAutoCHeck'", Cmd);
        U8Operation.GetDataString("select 'i_autocheck_type:" + i_autocheck_type + "'", Cmd);
        if ((i_autocheck_type==0 && cDBVouchAutoCHeck.CompareTo("true") == 0) || i_autocheck_type == 1)
        {
            //组合其他出库单
            DataTable dtRd09Main = U8Operation.GetSqlDataTable(@"select cOWhCode cwhcode,cODepCode cdepcode,cORdCode crdcode,cpersoncode,
	                '' cvencode,'调拨出库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cTVMemo cmemo
                    " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                from " + dbname + "..transvouch(nolock) where ID=0" + tranmain.ID, "HeadData", Cmd);
            DataTable dtRd09detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iTVNum inum,b.autoid itransid,b.coutposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + @"..inventory i(nolock) on b.cInvCode=i.cInvCode 
                where b.ID=0" + tranmain.ID + " order by b.irowno", "BodyData", Cmd);
            if (dtRd09Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
            if (dtRd09detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
            DataTable SHeadData = GetDtToHeadData(dtRd09Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81018(SHeadData, dtRd09detail, dbname, cUserName, cLogDate, "U81018_1", Cmd);
            
            //组合其他入库单
            DataTable dtRd08Main = U8Operation.GetSqlDataTable(@"select cIWhCode cwhcode,cIDepCode cdepcode,cIRdCode crdcode,cpersoncode,
	                '' cvencode,'调拨入库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cTVMemo cmemo
                    " + (c_vou_checker == "" ? "" : ",'" + c_vou_checker + "' checker") + @"
                    " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                from " + dbname + "..transvouch(nolock) where ID=0" + tranmain.ID, "HeadData", Cmd);
            DataTable dtRd08detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iTVNum inum,b.autoid itransid,b.cinposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,convert(varchar(10),b.dMadeDate,120) dprodate,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + @"..inventory i(nolock) on b.cInvCode=i.cInvCode 
                where b.ID=0" + tranmain.ID + " order by b.irowno", "BodyData", Cmd);
            if (dtRd08Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
            if (dtRd08detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
            SHeadData = GetDtToHeadData(dtRd08Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81019(SHeadData, dtRd08detail, dbname, cUserName, cLogDate, "U81019_1", Cmd);

            //审核调拨单状态
            Cmd.CommandText = "update " + dbname + "..TransVouch set cVerifyPerson='" + (c_vou_checker == "" ? cUserName : c_vou_checker) + "',dVerifyDate='" + cLogDate + "',dnverifytime=getdate() where id=" + rd_id;
            Cmd.ExecuteNonQuery();
        }

        #endregion

        return rd_id + "," + cc_mcode;

    }

    private bool TransVouchChecked(string m_id,string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        //减少调拨待发量
//        DataTable dtStk = U8Operation.GetSqlDataTable(@"select cOWhCode,cIWhCode,b.cinvcode,b.cTVBatch,b.iTVQuantity,iTVNum,b.cvmivencode,
//                    b.cfree1,b.cfree2,b.cfree3,b.cfree4,b.cfree5,b.cfree6,b.cfree7,b.cfree8,b.cfree9,b.cFree10
//                from " + dbname + "..transvouch a inner join " + dbname + "..transvouchs b on a.id=b.id where a.id=" + m_id, "dtStk", Cmd);
//        for (int c = 0; c < dtStk.Rows.Count; c++)
//        {
//            //更新数量 fTransOutQuantity
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)-(0" + dtStk.Rows[c]["iTVQuantity"] + @"),
//                        fTransOutNum=isnull(fTransOutNum,0)-(0" + dtStk.Rows[c]["iTVNum"] + @") 
//                    where cwhcode='" + dtStk.Rows[c]["cOWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();

//            //更新数量 fTransInQuantity
//            Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)-(0" + dtStk.Rows[c]["iTVQuantity"] + @") ,
//                        fTransInNum=isnull(fTransInNum,0)-(0" + dtStk.Rows[c]["iTVNum"] + @") 
//                    where cwhcode='" + dtStk.Rows[c]["cIWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//            Cmd.ExecuteNonQuery();
//        }


        //组合其他出库单
        DataTable dtRd09Main = U8Operation.GetSqlDataTable(@"select cOWhCode cwhcode,cODepCode cdepcode,cORdCode crdcode,cpersoncode,
	                '' cvencode,'调拨出库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cTVMemo cmemo
                from " + dbname + "..transvouch(nolock) where ID=0" + m_id, "HeadData", Cmd);
        DataTable dtRd09detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iTVNum inum,b.autoid itransid,b.coutposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + "..inventory i(nolock) on b.cInvCode=i.cInvCode where b.ID=0" + m_id + " order by b.irowno", "BodyData", Cmd);
        if (dtRd09Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
        if (dtRd09detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
        DataTable SHeadData = GetDtToHeadData(dtRd09Main, 0);
        SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
        U81018(SHeadData, dtRd09detail, dbname, cUserName, cLogDate, "U81018_1", Cmd);

        //组合其他入库单
        DataTable dtRd08Main = U8Operation.GetSqlDataTable(@"select cIWhCode cwhcode,cIDepCode cdepcode,cIRdCode crdcode,cpersoncode,
	                '' cvencode,'调拨入库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cTVMemo cmemo
                from " + dbname + "..transvouch(nolock) where ID=0" + m_id, "HeadData", Cmd);
        DataTable dtRd08detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iTVNum inum,b.autoid itransid,b.cinposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,convert(varchar(10),b.dMadeDate,120) dprodate,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + "..inventory i(nolock) on b.cInvCode=i.cInvCode where b.ID=0" + m_id + " order by b.irowno", "BodyData", Cmd);
        if (dtRd08Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
        if (dtRd08detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
        SHeadData = GetDtToHeadData(dtRd08Main, 0);
        SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
        U81019(SHeadData, dtRd08detail, dbname, cUserName, cLogDate, "U81019_1", Cmd);

        //审核调拨单状态
        Cmd.CommandText = "update " + dbname + "..TransVouch set cVerifyPerson='" + cUserName + "',dVerifyDate='" + cLogDate + "',dnverifytime=getdate() where id=" + m_id;
        Cmd.ExecuteNonQuery();

        return true;
    }

    private bool DispVouchChecked(string m_id, string dbname, string cUserName, string cLogDate, SqlCommand Cmd)
    {
        bool b_SaCreateRd32 = false;
        if (U8Operation.GetDataInt("select CAST(CAST(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cName='bSAcreat'", Cmd) == 1)
        {
            b_SaCreateRd32 = true;
        }
        //发货单审核后自动出库
        if (b_SaCreateRd32)
        {
            string w_cwhcode = "";
            DataTable dtWare = U8Operation.GetSqlDataTable("select distinct cwhcode from " + dbname + "..DispatchLists where DLID=0" + m_id, "dtWare", Cmd);
            for (int i = 0; i < dtWare.Rows.Count; i++)
            {
                w_cwhcode = "" + dtWare.Rows[i]["cwhcode"];
                //组合销售出库单
                DataTable dtRdMain = U8Operation.GetSqlDataTable(@"select '" + w_cwhcode + @"'cwhcode,cdepcode,cstcode,cpersoncode,
	                    ccuscode,'普通销售' cbustype,cdlcode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                    cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cmemo
                    from " + dbname + "..DispatchList where DLID=0" + m_id, "HeadData", Cmd);
                DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cbatch,b.iquantity,b.cvmivencode cbvencode,
	                    b.idlsid,b.cposition cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                    cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                    cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.inum
                    from " + dbname + "..DispatchLists b inner join " + dbname + @"..inventory i on b.cInvCode=i.cInvCode 
                    where b.DLID=0" + m_id + " and cwhcode='" + w_cwhcode + "'", "BodyData", Cmd);
                if (dtRdMain.Rows.Count == 0) throw new Exception("无法找到发货单");
                if (dtRddetail.Rows.Count == 0) throw new Exception("无法找到发货单内容数据");
                DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                U81021(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81021", Cmd);
            }


        }
        //审核发货单状态
        Cmd.CommandText = "update " + dbname + "..DispatchList set cVerifier='" + cUserName + "',dverifydate='" + cLogDate + "',dverifysystime=getdate() where dlid=" + m_id;
        Cmd.ExecuteNonQuery();

        return true;
    }

    //订单调拨 [生产订单调拨   委外订单调拨]  [可以来源申请调拨]
    private string U81035(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        //transvouchs.AppTransIDS   存放销售订单 子表id
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cindepcode = ""; string w_cpersoncode = ""; string w_inwhcode = "";
        string w_rdcode = ""; string w_in_rdcode = ""; //出库类型
        string w_ordertype = "生产订单";//订单类型
        string w_headPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_in_Vmi = false; bool b_Pos = false; bool b_in_Pos = false;//货位
        string modid = ""; string Allocateid = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_autocheck_type = 0;// 是否自动审核，0 代表执行通用控制参数；1 代表本单自动审核；2 代表本单不需要审核
        bool b_has_AppTrans = false;//是否存在调拨申请单
        bool b_out_appvouch_tran = false; //是否超调拨申请 调拨
        #region  //逻辑检验
        //自动审核 规则
        string c_autocheckvalue = GetTextsFrom_FormData_Tag(HeadData, "txt_i_isauto_check");
        if (c_autocheckvalue == "1" || c_autocheckvalue == "2")
        {
            i_autocheck_type = int.Parse(c_autocheckvalue);
        }

        w_ordertype = GetTextsFrom_FormData_Tag(HeadData, "txt_cordertype");
        if (w_ordertype.CompareTo("") == 0) w_ordertype = "生产订单";

        U8Operation.GetDataString("select 'Body Rows:" + BodyData.Rows.Count + "'", Cmd);
        if (BodyData.Columns.Contains("itrids") && BodyData.Rows[0]["itrids"] + "" != "") b_has_AppTrans = true; //存在调拨申请单
        //foreach (DataRow dd in BodyData.Rows)
        //{
        //    U8Operation.GetDataString("select 'Body data  allocateid:" + dd["allocateid"] + "'", Cmd);
        //}

        if (BodyData.Columns.Contains("allocateid"))
        {
            Allocateid = BodyData.Rows[0]["allocateid"] + "";
            if (Allocateid.CompareTo("") != 0)  //不为空
            {
                bCreate = true;
                if (w_ordertype.CompareTo("生产订单") == 0)
                {
                    modid = U8Operation.GetDataString("select modid from " + dbname + "..mom_moallocate(nolock) where AllocateId=0" + Allocateid + " and ByproductFlag=0", Cmd);
                }
                else //委外订单
                {
                    modid = U8Operation.GetDataString("select MoDetailsID from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + Allocateid, Cmd);
                }
                if (modid == "") throw new Exception("没有找到订单信息,或被设置成副产品");
            }
        }
        if (Allocateid.CompareTo("") == 0 && b_has_AppTrans==false)
        {
            throw new Exception("生产订单用料子表ID/委外订单用料子表ID 栏目必须设置成 显示状态");
        }
        if (bCreate == false && b_has_AppTrans == false) //**************************************
        {
            throw new Exception("必须根据生产订单或委外订单调拨或领用申请单");
        }

        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style(nolock) where cRDCode='" + w_rdcode + "' and  bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误,或非末级");
        }
        //入库类别
        w_in_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinrdcode");
        if (w_in_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style(nolock) where cRDCode='" + w_in_rdcode + "' and  bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误,或非末级");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("调出仓库输入错误");

        //调入仓库
        w_inwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinwhcode");
        if (w_inwhcode == "")  //当调入仓库未空时，取用料表子件的 仓库
        {
            if (w_ordertype.CompareTo("生产订单") == 0)
            {
                w_inwhcode = U8Operation.GetDataString("select WhCode from " + dbname + "..mom_moallocate(nolock) where AllocateId=0" + Allocateid, Cmd);
            }
            else if (w_ordertype.CompareTo("委外订单") == 0)
            {
                w_inwhcode = U8Operation.GetDataString("select cWhCode from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + Allocateid, Cmd);
            }
            if (w_inwhcode == "")
                throw new Exception("请输入调入仓库");
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "'", Cmd) == 0)
                throw new Exception("调入仓库输入错误");
        }

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("调出部门输入错误");
        }
        w_cindepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cindepcode");
        if (w_cindepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cindepcode + "'", Cmd) == 0)
                throw new Exception("调入部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person(nolock) where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        //模板号
        string cvtid = GetTextsFrom_FormData_Text(HeadData, "txt_vt_id");

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a(nolock) 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b(nolock) on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0304' and VT_TemplateMode=0", Cmd);
        if (U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation(nolock) where cSysID='ST' and cName='bOverTransRequestTransfer'", Cmd).ToLower().CompareTo("true") == 0)
        {
            b_out_appvouch_tran = true;
        }

        #region //新增单据
        KK_U8Com.U8TransVouch tranmain = new KK_U8Com.U8TransVouch(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "T" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cTVCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..TransVouch(nolock) where cTVCode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        tranmain.cTVCode = "'" + cc_mcode + "'";
        tranmain.ID = rd_id;
        tranmain.cOWhCode = "'" + w_cwhcode + "'";
        tranmain.cORdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        tranmain.cODepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        tranmain.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        tranmain.cIWhCode = "'" + w_inwhcode + "'";
        tranmain.cIRdCode = (w_in_rdcode.CompareTo("") == 0 ? "null" : "'" + w_in_rdcode + "'");
        tranmain.cIDepCode = (w_cindepcode.CompareTo("") == 0 ? "null" : "'" + w_cindepcode + "'");
        tranmain.csource = "'1'";
        tranmain.cOrderType = "'" + w_ordertype + "'";
        if (cvtid == "")
        {
            tranmain.VT_ID = vt_id;
        }
        else
        {
            tranmain.VT_ID = Convert.ToInt32(cvtid);
        }

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_in_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_inwhcode + "' and bWhPos=1", Cmd) > 0)
            b_in_Pos = true;

        tranmain.cMaker = "'" + cUserName + "'";
        tranmain.dTVDate = "'" + cLogDate + "'";
        tranmain.cTVMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_ctvmemo") + "'";
        tranmain.IsWfControlled = 0;

        #region   //主表自定义项处理
        tranmain.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        tranmain.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        tranmain.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        tranmain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        tranmain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        tranmain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        tranmain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        tranmain.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        tranmain.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        tranmain.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        tranmain.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        tranmain.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        tranmain.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        tranmain.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        tranmain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        tranmain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        if (!tranmain.InsertToDB(targetAccId, false, ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region //子表
        string c_firstTofisrt_Ctl = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_out_cfirstToFirst_control'", Cmd);
        c_firstTofisrt_Ctl = c_firstTofisrt_Ctl.ToLower();
        if (c_firstTofisrt_Ctl.CompareTo("true") == 0)   //管控先进先出法前先排序
        {
            if (BodyData.Columns.Contains("cbatch"))
            {
                BodyData.DefaultView.Sort = "cinvcode,cbatch";
            }else{
                BodyData.DefaultView.Sort = "cinvcode";
            }
            BodyData = BodyData.DefaultView.ToTable();
        }

        int irdrow = 0;
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            Allocateid = BodyData.Rows[i]["allocateid"] + "";
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("调拨数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if ((b_Vmi || b_in_Vmi) && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库 必须有代管商");

            //生单规则，供应商和业务类型一致性检查
            if (Allocateid != "")
            {
                if (w_ordertype.CompareTo("生产订单") == 0)
                {
                    string chkValue = U8Operation.GetDataString("select modid from " + dbname + "..mom_moallocate a(nolock) where allocateid=0" + BodyData.Rows[i]["allocateid"] + " and a.ByproductFlag=0", Cmd);
                    if (chkValue.CompareTo("") == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]生单模式必须根据生产订单调拨,请确认扫描的生产订单存在,或子件为联副产品");
                }
                else
                {
                    string chkValue = U8Operation.GetDataString("select MoDetailsID from " + dbname + "..OM_MOMaterials a(nolock) where MOMaterialsID=0" + BodyData.Rows[i]["allocateid"], Cmd);
                    if (chkValue.CompareTo("") == 0) throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]生单模式必须根据委外订单调拨,请确认扫描的委外订单存在");
                }
            }
            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            if (w_ordertype.CompareTo("生产订单") == 0)
            {
                modid = U8Operation.GetDataString("select modid from " + dbname + "..mom_moallocate(nolock) where Allocateid=0" + Allocateid, Cmd);
            }
            else if (w_ordertype.CompareTo("委外订单") == 0)
            {
                modid = U8Operation.GetDataString("select MoDetailsID from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + Allocateid, Cmd);
            }
            else
            {
                modid = "0";
            }
            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            if (c_body_batch.CompareTo("") == 0 && cfirstToFirst.CompareTo("auto") == 0)  //没有批次且执行自动制定批次出库
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, GetBodyValue_FromData(BodyData, i, "cposcode"), dbname);
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, BodyData.Rows[i]["cposcode"] + "",
                    GetBodyValue_FromData(BodyData, i, "cfree1"), GetBodyValue_FromData(BodyData, i, "cfree2"), GetBodyValue_FromData(BodyData, i, "cfree3"),
                    GetBodyValue_FromData(BodyData, i, "cfree4"), GetBodyValue_FromData(BodyData, i, "cfree5"), GetBodyValue_FromData(BodyData, i, "cfree6"),
                    GetBodyValue_FromData(BodyData, i, "cfree7"), GetBodyValue_FromData(BodyData, i, "cfree8"), GetBodyValue_FromData(BodyData, i, "cfree9"),
                    GetBodyValue_FromData(BodyData, i, "cfree10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            #endregion

            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                KK_U8Com.U8TransVouchs trandetail = new KK_U8Com.U8TransVouchs(Cmd, dbname);
                int cAutoid = U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='tr'", Cmd);
                trandetail.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='tr'";
                Cmd.ExecuteNonQuery();

                trandetail.ID = rd_id;
                trandetail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                trandetail.iTVQuantity = "" + dtMer.Rows[m]["iquantity"];
                trandetail.irowno = (i + 1);
                irdrow++;
                trandetail.irowno = irdrow;
                trandetail.cTVCode = tranmain.cTVCode;
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + "", Cmd);

                #region //自由项  自定义项处理
                trandetail.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                trandetail.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                trandetail.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                trandetail.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                trandetail.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                trandetail.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                trandetail.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                trandetail.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                trandetail.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                trandetail.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                trandetail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
                trandetail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
                trandetail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
                trandetail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
                trandetail.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
                trandetail.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                trandetail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
                trandetail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
                trandetail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
                trandetail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
                trandetail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
                trandetail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
                trandetail.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
                trandetail.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
                trandetail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
                trandetail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                #endregion

                #region //获得单价与金额
                string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                #endregion

                #region //代管挂账处理
                trandetail.cvmivencode = "''";
                trandetail.bCosting = 1;
                if (b_Vmi)
                {
                    //trandetail.bVMIUsed = "1";
                    trandetail.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理
                trandetail.cTVBatch = "''";
                trandetail.iExpiratDateCalcu = 0;
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" == "") throw new Exception(trandetail.cInvCode + "有批次管理，必须输入批号");
                    trandetail.cTVBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                    //保质期管理
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + trandetail.cInvCode + " and bInvQuality=1", Cmd) > 0)
                    {
                        trandetail.iExpiratDateCalcu = int.Parse(dtMer.Rows[m]["有效期推算方式"] + "");
                        trandetail.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                        trandetail.dDisDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        trandetail.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        trandetail.dMadeDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                        trandetail.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                    }

                    //批次档案 建档
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + trandetail.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                    {
                        DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + trandetail.cInvCode + " and cBatch=" + trandetail.cTVBatch + " and isnull(cFree1,'')=" + trandetail.cFree1 + @" 
                                and isnull(cFree2,'')=" + trandetail.cFree2 + " and isnull(cFree3,'')=" + trandetail.cFree3 + " and isnull(cFree4,'')=" + trandetail.cFree4 + @" 
                                and isnull(cFree5,'')=" + trandetail.cFree5 + " and isnull(cFree6,'')=" + trandetail.cFree6 + " and isnull(cFree7,'')=" + trandetail.cFree7 + @" 
                                and isnull(cFree8,'')=" + trandetail.cFree8 + " and isnull(cFree9,'')=" + trandetail.cFree9 + " and isnull(cFree10,'')=" + trandetail.cFree10, "dtBatPerp", Cmd);
                        if (dtBatPerp.Rows.Count > 0)
                        {
                            trandetail.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                            trandetail.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                            trandetail.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                            trandetail.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                            trandetail.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                            trandetail.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                            trandetail.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                            trandetail.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                            trandetail.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                            trandetail.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                        }
                    }
                }
                #endregion

                #region //固定换算率（多计量） 和 回写到生产单
                trandetail.iTVNum = "0";
                if (cst_unitcode.CompareTo("") != 0)
                {
                    trandetail.cAssUnit = "'" + cst_unitcode + "'";
                    string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + trandetail.cAssUnit, Cmd);
                    if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                    trandetail.iinvexchrate = ichange;
                    inum = U8Operation.GetDataString("select round(" + trandetail.iTVQuantity + "/" + ichange + ",5)", Cmd);
                    trandetail.iTVNum = inum;

                    //Cmd.CommandText = "update " + dbname + "..mom_moallocate set iTransNum=isnull(iTransNum,0)+(0" + inum + ") where Allocateid =0" + Allocateid;
                    //Cmd.ExecuteNonQuery();
                }

                //回写到订单用量表累计调拨数量
                if (w_ordertype.CompareTo("生产订单") == 0)
                {
                    Cmd.CommandText = "update " + dbname + "..mom_moallocate set TransQty=isnull(TransQty,0)+(0" + trandetail.iTVQuantity + ") where Allocateid =0" + Allocateid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set fTransQty=isnull(fTransQty,0)+(0" + trandetail.iTVQuantity + ") where MOMaterialsID =0" + Allocateid;
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                #region //回写调拨申请单
                if (b_has_AppTrans && BodyData.Rows[i]["itrids"] + "" != "")
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..ST_AppTransVouchs(nolock) where autoid=" + BodyData.Rows[i]["itrids"], Cmd) == 0)
                    {
                        throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]调拨申请单不存在");
                    }

                    Cmd.CommandText = "update " + dbname + "..ST_AppTransVouchs set iTvSumQuantity=isnull(iTvSumQuantity,0)+(0" + trandetail.iTVQuantity + @"),
                            iTVSumNum=isnull(iTVSumNum,0)+(0" + (trandetail.iTVNum == "null" ? "0" : trandetail.iTVNum) + @")
                        where autoid=" + BodyData.Rows[i]["itrids"];
                    Cmd.ExecuteNonQuery();

                    //判断调拨申请单是否超出 申请量
                    if (!b_out_appvouch_tran)
                    {
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..ST_AppTransVouchs(nolock) 
                            where autoid=" + BodyData.Rows[i]["itrids"] + " and isnull(iTvSumQuantity,0)>isnull(iTvChkQuantity,0)+0.005", Cmd) > 0)
                        {
                            throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超调拨申请单数量调拨");
                        }
                    }

                    trandetail.iTRIds = BodyData.Rows[i]["itrids"] + "";
                }
                #endregion 

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                if (bCreate && Allocateid != "")
                {
                    if (w_ordertype.CompareTo("生产订单") == 0)
                    {
                        trandetail.imoids = Allocateid; //订单Allocateid
                        trandetail.imoseq = U8Operation.GetDataString("select SortSeq from " + dbname + "..mom_orderdetail a(nolock) where modid=0" + modid, Cmd); //生产订单行号   
                        trandetail.cmocode = "'" + U8Operation.GetDataString("select b.mocode from " + dbname + @"..mom_orderdetail a(nolock) 
                        inner join " + dbname + "..mom_order b(nolock) on a.moid=b.moid where a.modid=0" + modid, Cmd) + "'";

                        if (U8Operation.GetDataString("select isnull(RelsUser,'') from " + dbname + "..mom_orderdetail a(nolock) where modid=0" + modid, Cmd) == "")
                            throw new Exception("存货" + trandetail.cInvCode + "的生产订单未终审");
                        if (U8Operation.GetDataString("select isnull(CloseUser,'') from " + dbname + "..mom_orderdetail a(nolock) where modid=0" + modid, Cmd) != "")
                            throw new Exception("存货" + trandetail.cInvCode + "的生产订单已经关闭");

                        //继承到货单的表体自由项  自定义项数据
                        dtPuArr = U8Operation.GetSqlDataTable(@"select free1,free2,free3,free4,free5,free6,free7,free8,free9,free10,
                            define22 cdefine22,define23 cdefine23,define24 cdefine24,define25 cdefine25,define26 cdefine26,define27 cdefine27,define28 cdefine28,
                            define29 cdefine29,define30 cdefine30,define31 cdefine31,define32 cdefine32,define33 cdefine33,define34 cdefine34,
                            define35 cdefine35,define36 cdefine36,define37 cdefine37 from " + dbname + @"..mom_moallocate a(nolock) 
                        where Allocateid=0" + Allocateid + " and a.ByproductFlag=0", "dtPuArr", Cmd);
                        if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到生产订单用料信息");
                    }
                    else 
                    {
                        trandetail.iomids = Allocateid; //订单Allocateid
                        trandetail.comcode = "'" + U8Operation.GetDataString("select cCode from " + dbname + "..OM_MOMain a(nolock) inner join " + dbname + @"..OM_MODetails b(nolock) on a.moid=b.moid 
                        where b.MODetailsID=0" + modid, Cmd) + "'";

                        if (U8Operation.GetDataString("select isnull(a.cVerifier,'') from " + dbname + "..OM_MOMain a(nolock) inner join " + dbname + @"..OM_MODetails b(nolock) on a.moid=b.moid 
                        where b.MODetailsID=0" + modid, Cmd) == "")
                            throw new Exception("存货" + trandetail.cInvCode + "的生产订单未终审");
                        if (U8Operation.GetDataString("select isnull(cbCloser,'') from " + dbname + "..OM_MODetails a(nolock) where MODetailsID=0" + modid, Cmd) != "")
                            throw new Exception("存货" + trandetail.cInvCode + "的委外订单已经关闭");

                        //继承到货单的表体自由项  自定义项数据
                        dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..OM_MOMaterials a(nolock) 
                        where MOMaterialsID=0" + Allocateid, "dtPuArr", Cmd);
                        if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到委外订单用料信息");
                    }


                    if (trandetail.cDefine22.CompareTo("''") == 0) trandetail.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                    if (trandetail.cDefine23.CompareTo("''") == 0) trandetail.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                    if (trandetail.cDefine24.CompareTo("''") == 0) trandetail.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                    if (trandetail.cDefine25.CompareTo("''") == 0) trandetail.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                    if (trandetail.cDefine28.CompareTo("''") == 0) trandetail.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                    if (trandetail.cDefine29.CompareTo("''") == 0) trandetail.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                    if (trandetail.cDefine30.CompareTo("''") == 0) trandetail.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                    if (trandetail.cDefine31.CompareTo("''") == 0) trandetail.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                    if (trandetail.cDefine32.CompareTo("''") == 0) trandetail.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                    if (trandetail.cDefine33.CompareTo("''") == 0) trandetail.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos) //调出
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("调出仓库有货位管理，" + trandetail.cInvCode + "的调出货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + tranmain.cOWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于调出仓库");
                    trandetail.coutposcode = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                if (b_in_Pos)//调入
                {
                    if (BodyData.Rows[i]["cinposcode"] + "" == "" && w_headPosCode != "") BodyData.Rows[i]["cinposcode"] = w_headPosCode;
                    if (BodyData.Rows[i]["cinposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cinposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + trandetail.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cinposcode"] + "" == "") throw new Exception("调入仓库有货位管理，" + trandetail.cInvCode + "的调入货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + tranmain.cIWhCode + " and cposcode='" + BodyData.Rows[i]["cinposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cinposcode"] + "】不存在，或者不属于调入仓库");
                    trandetail.cinposcode = "'" + BodyData.Rows[i]["cinposcode"] + "'";
                }

                #endregion

                //保存数据
                if (!trandetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region //补充版本信息
                if (w_ordertype.CompareTo("生产订单") == 0 && modid != "")
                {
                    //cMoLotCode 
                    string cmo_lot = U8Operation.GetDataString("select MoLotCode from " + dbname + "..mom_orderdetail a(nolock) where modid=0" + modid, Cmd); //生产批号   
                    Cmd.CommandText = "update " + dbname + "..TransVouchs set cMoLotCode='" + cmo_lot + @"' where autoID=0" + trandetail.AutoID;
                    Cmd.ExecuteNonQuery();
                }

                if (c_ppcost != "")
                {
                    Cmd.CommandText = "update " + dbname + "..TransVouchs set iTVPCost=" + c_ppcost + ", iTVPPrice=" + (decimal.Parse(c_ppcost) * decimal.Parse("" + dtMer.Rows[m]["iquantity"])) + @"
                        where autoID=0" + trandetail.AutoID;
                    Cmd.ExecuteNonQuery();
                }
                #endregion
            }


            #region//是否超订单检查
            if (bCreate && Allocateid != "")
            {
                if (w_ordertype.CompareTo("生产订单") == 0)
                {
                    float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(itvquantity),0) from " + dbname + "..TransVouchs(nolock) where imoids=0" + Allocateid, Cmd));
                    #region //判断是否超订单领料  0代表不能超   1 代表可超
                    if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='bOverMPOut'", Cmd) == 0)
                    {
                        //关键领用控制量，取最小值  
                        float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select Qty from " + dbname + "..mom_moallocate(nolock) where AllocateId=0" + Allocateid, Cmd));
                        if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超订单出库");
                    }
                    #endregion
                }
                else
                {
                    float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(itvquantity),0) from " + dbname + "..TransVouchs(nolock) where iomids=0" + Allocateid, Cmd));
                    #region //判断是否超订单领料  0代表不能超   1 代表可超
                    if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation(nolock) where cSysID='st' and cname='bOverMPOut'", Cmd) == 0)
                    {
                        //关键领用控制量，取最小值  
                        float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iQuantity from " + dbname + "..OM_MOMaterials(nolock) where MOMaterialsID=0" + Allocateid, Cmd));
                        if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超订单出库");
                    }
                    #endregion
                }
            }
            #endregion

        }

        #endregion
        #endregion

        #region   //审核调拨单
        string cDBVouchAutoCHeck = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='mes_cDBVouchAutoCHeck'", Cmd);
        if ((i_autocheck_type == 0 && cDBVouchAutoCHeck.CompareTo("true") == 0) || i_autocheck_type == 1)
        {
            //组合其他出库单
            DataTable dtRd09Main = U8Operation.GetSqlDataTable(@"select cOWhCode cwhcode,cODepCode cdepcode,cORdCode crdcode,cpersoncode,
	                '' cvencode,'调拨出库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16
                    " + (c_vou_checker == "" ? "" : ",'" + c_vou_checker + "' checker") + @"
                    " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                from " + dbname + "..transvouch(nolock) where ID=0" + tranmain.ID, "HeadData", Cmd);
            DataTable dtRd09detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.autoid itransid,b.coutposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.citemcode,b.cItem_class citemclass
                    " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + @"..inventory i(nolock) on b.cInvCode=i.cInvCode 
                where b.ID=0" + tranmain.ID + " order by b.irowno", "BodyData", Cmd);
            if (dtRd09Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
            if (dtRd09detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
            DataTable SHeadData = GetDtToHeadData(dtRd09Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81018(SHeadData, dtRd09detail, dbname, cUserName, cLogDate, "U81018_1", Cmd);

            //组合其他入库单
            DataTable dtRd08Main = U8Operation.GetSqlDataTable(@"select cIWhCode cwhcode,cIDepCode cdepcode,cIRdCode crdcode,cpersoncode,
	                '' cvencode,'调拨入库' cbustype,cTVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16
                from " + dbname + "..transvouch(nolock) where ID=0" + tranmain.ID, "HeadData", Cmd);
            DataTable dtRd08detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cTVBatch cbatch,b.iTVQuantity iquantity,b.cvmivencode cbvencode,
	                b.autoid itransid,b.cinposcode cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,convert(varchar(10),b.dMadeDate,120) dprodate,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..transvouchs b(nolock) inner join " + dbname + @"..inventory i(nolock) on b.cInvCode=i.cInvCode 
                where b.ID=0" + tranmain.ID + " order by b.irowno", "BodyData", Cmd);
            if (dtRd08Main.Rows.Count == 0) throw new Exception("无法找到调拨单");
            if (dtRd08detail.Rows.Count == 0) throw new Exception("无法找到调拨单内容数据");
            SHeadData = GetDtToHeadData(dtRd08Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81019(SHeadData, dtRd08detail, dbname, cUserName, cLogDate, "U81019_1", Cmd);

            //审核调拨单状态
            Cmd.CommandText = "update " + dbname + "..TransVouch set cVerifyPerson='" + (c_vou_checker == "" ? cUserName : c_vou_checker) + "',dVerifyDate='" + cLogDate + "',dnverifytime=getdate() where id=" + rd_id;
            Cmd.ExecuteNonQuery();
        }
        else
        {
//            //增加调拨待发量
//            DataTable dtStk = U8Operation.GetSqlDataTable(@"select cOWhCode,cIWhCode,b.cinvcode,b.cTVBatch,b.iTVQuantity,iTVNum,b.cvmivencode,
//                    b.cfree1,b.cfree2,b.cfree3,b.cfree4,b.cfree5,b.cfree6,b.cfree7,b.cfree8,b.cfree9,b.cFree10
//                from " + dbname + "..transvouch a inner join " + dbname + "..transvouchs b on a.id=b.id where a.id=" + tranmain.ID, "dtStk", Cmd);
//            for (int c = 0; c < dtStk.Rows.Count; c++)
//            {
//                U8V10SetCurrentStockRow(Cmd,dbname+"..",""+dtStk.Rows[c]["cOWhCode"],""+dtStk.Rows[c]["cinvcode"],""+dtStk.Rows[c]["cfree1"],""+dtStk.Rows[c]["cfree2"],
//                    "" + dtStk.Rows[c]["cfree3"], "" + dtStk.Rows[c]["cfree4"], "" + dtStk.Rows[c]["cfree5"], "" + dtStk.Rows[c]["cfree6"], "" + dtStk.Rows[c]["cfree7"],
//                    "" + dtStk.Rows[c]["cfree8"], "" + dtStk.Rows[c]["cfree9"], "" + dtStk.Rows[c]["cfree10"], "" + dtStk.Rows[c]["cTVBatch"], "" + dtStk.Rows[c]["cvmivencode"]);
//                U8V10SetCurrentStockRow(Cmd, dbname + "..", "" + dtStk.Rows[c]["cIWhCode"], "" + dtStk.Rows[c]["cinvcode"], "" + dtStk.Rows[c]["cfree1"], "" + dtStk.Rows[c]["cfree2"],
//                    "" + dtStk.Rows[c]["cfree3"], "" + dtStk.Rows[c]["cfree4"], "" + dtStk.Rows[c]["cfree5"], "" + dtStk.Rows[c]["cfree6"], "" + dtStk.Rows[c]["cfree7"],
//                    "" + dtStk.Rows[c]["cfree8"], "" + dtStk.Rows[c]["cfree9"], "" + dtStk.Rows[c]["cfree10"], "" + dtStk.Rows[c]["cTVBatch"], "" + dtStk.Rows[c]["cvmivencode"]);
//                //更新数量 fTransOutNum
//                Cmd.CommandText = "update " + dbname + "..currentstock set fTransOutQuantity=isnull(fTransOutQuantity,0)+(0" + dtStk.Rows[c]["iTVQuantity"] + @"),
//                        fTransOutNum=isnull(fTransOutNum,0)+(0" + dtStk.Rows[c]["iTVNum"] + @")
//                    where cwhcode='" + dtStk.Rows[c]["cOWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//                Cmd.ExecuteNonQuery();
                
//                //更新数量
//                Cmd.CommandText = "update " + dbname + "..currentstock set fTransInQuantity=isnull(fTransInQuantity,0)+(0" + dtStk.Rows[c]["iTVQuantity"] + @") ,
//                        fTransInNum=isnull(fTransInNum,0)+(0" + dtStk.Rows[c]["iTVNum"] + @") 
//                    where cwhcode='" + dtStk.Rows[c]["cIWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + "' and iSoType=0 and iSodid=''";
//                Cmd.ExecuteNonQuery();

//                #region  //检查可用量
//                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..currentstock 
//                    where cwhcode='" + dtStk.Rows[c]["cOWhCode"] + "' and cinvcode='" + dtStk.Rows[c]["cinvcode"] + "' and cfree1='" + dtStk.Rows[c]["cfree1"] + @"' 
//                        and cfree2='" + dtStk.Rows[c]["cfree2"] + "' and cfree3='" + dtStk.Rows[c]["cfree3"] + "' and cfree4='" + dtStk.Rows[c]["cfree4"] + @"' 
//                        and cfree5='" + dtStk.Rows[c]["cfree5"] + "' and cfree6='" + dtStk.Rows[c]["cfree6"] + "' and cfree7='" + dtStk.Rows[c]["cfree7"] + @"' 
//                        and cfree8='" + dtStk.Rows[c]["cfree8"] + "' and cfree9='" + dtStk.Rows[c]["cfree9"] + "' and cfree10='" + dtStk.Rows[c]["cfree10"] + @"' 
//                        and cbatch='" + dtStk.Rows[c]["cTVBatch"] + "' and cVMIVenCode='" + dtStk.Rows[c]["cvmivencode"] + @"' and iSoType=0 and iSodid='' 
//                        and iQuantity+fInQuantity+fTransInQuantity-fOutQuantity-fTransOutQuantity<0", Cmd) > 0)
//                {
//                    throw new Exception("仓库[" + dtStk.Rows[c]["cOWhCode"] + "]存货[" + dtStk.Rows[c]["cinvcode"] + "]批号[" + dtStk.Rows[c]["cTVBatch"] + "]超可用量");
//                }
//                #endregion
//            }

        }
        #endregion

        return rd_id + "," + cc_mcode;

    }

    //其他出库单 [直接出库 调拨出库 作业单出库]
    private string U81018(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; //出库类型
        string w_bustype = "";//业务类型
        bool b_Vmi = false; bool b_Pos = false;//货位
        string itrid = ""; string itransid = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_red_sheet = 0;
        #region  //逻辑检验
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) w_bustype = "其他出库";

        if (BodyData.Columns.Contains("itransid"))
        {
            itransid = BodyData.Rows[0]["itransid"] + "";
            if (itransid.CompareTo("") != 0)
            {
                bCreate = true;
                if (w_bustype.CompareTo("转换出库") == 0)
                {
                    itrid = U8Operation.GetDataString("select id from " + dbname + "..AssemVouchs a(nolock) where autoid=0" + itransid, Cmd);
                    if (itrid == "") throw new Exception("没有找到形态转换单信息");
                }
                if (w_bustype.CompareTo("调拨出库") == 0)
                {
                    itrid = U8Operation.GetDataString("select id from " + dbname + "..TransVouchs a(nolock) where autoid=0" + itransid, Cmd);
                    if (itrid == "") throw new Exception("没有找到调拨单信息");
                }
            }
        }
        if (float.Parse("" + BodyData.Rows[0]["iquantity"]) < 0) i_red_sheet = 1;//第一行数据的 数量判断红篮子

        
        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style(nolock) where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //供应商
        w_vencode = GetTextsFrom_FormData_Tag(HeadData, "txt_cvencode");
        if (w_vencode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..vendor(nolock) where cvencode='" + w_vencode + "'", Cmd) == 0)
                throw new Exception("供应商输入错误");
        }

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department(nolock) where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person(nolock) where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a(nolock) 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b(nolock) on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        #region //检查单据头必录项
        //        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
//                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
//        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
//        {
//            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
//            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
//            if (txtdata == null) throw new Exception(dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项，模板必须设置成可视栏目");

//            if (txtdata[3].CompareTo("") == 0 && txtdata[2].CompareTo("") != 0)  //
//            {
//                throw new Exception(txtdata[0] + "录入不正确 录入键值和显示值不匹配");
//            }
//            if (txtdata[3].CompareTo("") == 0)  //
//            {
//                throw new Exception(txtdata[0] + "为必录项,不能为空");
//            }
        //        }
        #endregion

        //检查单据体必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow(nolock) where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cfirstToFirst'", Cmd);
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0302' and VT_TemplateMode=0", Cmd);

        //严格控制先进先出规则
        string c_firstTofisrt_Ctl = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_out_cfirstToFirst_control'", Cmd);
        c_firstTofisrt_Ctl = c_firstTofisrt_Ctl.ToLower();
        //先进先出 例外仓库
        string c_firstTofisrt_NotCtl_WareList = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_out_cfirstToFirst_Not_control_ware'", Cmd);
        c_firstTofisrt_NotCtl_WareList = "," + c_firstTofisrt_NotCtl_WareList + ",";

        string cc_mcode = "";//单据号
        #region //单据
        KK_U8Com.U8Rdrecord09 record09 = new KK_U8Com.U8Rdrecord09(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0) from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "C" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord09(nolock) where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record09.cCode = "'" + cc_mcode + "'";
        record09.ID = rd_id;
        record09.cVouchType = "'09'";
        record09.bredvouch = "0"; //红篮子  
        record09.cBusType = "'" + w_bustype + "'";
        record09.cWhCode = "'" + w_cwhcode + "'";
        record09.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record09.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record09.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record09.cVenCode = (w_vencode.CompareTo("") == 0 ? "null" : "'" + w_vencode + "'");
        record09.VT_ID = vt_id;
        record09.cBusCode = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode") + "'";  //业务号
        record09.bredvouch = i_red_sheet + "";

        if (bCreate)
        {
            if (w_bustype.CompareTo("调拨出库") == 0) record09.cSource = "'调拨'";
            if (w_bustype.CompareTo("转换出库") == 0) record09.cSource = "'形态转换'";
        }
        else
        {
            if (w_bustype == "备件领用")
            {
                record09.cSource = "'作业单'";
            }
            else
            {
                record09.cSource = "'库存'";
            }
        }

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record09.cMaker = "'" + cUserName + "'";
        record09.dDate = "'" + cLogDate + "'";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_rd09_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record09.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record09.cHandler = "'" + c_vou_checker + "'";
            }
            
            record09.dVeriDate = "'" + cLogDate + "'";
            record09.dnverifytime = "getdate()";
        }
        record09.iExchRate = "1";
        record09.cExch_Name = "'人民币'";
        record09.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";

        #region   //主表自定义项处理
        record09.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record09.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record09.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record09.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record09.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record09.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record09.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record09.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record09.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record09.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record09.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record09.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record09.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record09.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record09.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record09.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        if (!record09.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region //子表
        //BodyData 数据按照存货编码 批号排序
        if (c_firstTofisrt_Ctl.CompareTo("true") == 0)   //管控先进先出法前先排序
        {
            BodyData.DefaultView.Sort = "cinvcode,cbatch";
            BodyData = BodyData.DefaultView.ToTable();
        }

        int irdrow = 0;
        //判断仓库是否计入成本
        string b_costing = U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse(nolock) where cwhcode=" + record09.cWhCode, Cmd);

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            float f_the_qty = float.Parse("" + BodyData.Rows[i]["iquantity"]);
            if ((f_the_qty > 0 && i_red_sheet == 1) || (f_the_qty < 0 && i_red_sheet == 0)) throw new Exception("本单据红蓝子混乱");
            if (f_the_qty == 0) throw new Exception("出入库数量不能为空 或0");

            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            itransid = BodyData.Rows[i]["itransid"] + "";
            itrid = "";
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string chkValue = "";
                if (w_bustype.CompareTo("转换出库") == 0) chkValue = U8Operation.GetDataString("select id from " + dbname + "..AssemVouchs a where autoid=0" + BodyData.Rows[i]["itransid"], Cmd);
                if (w_bustype.CompareTo("调拨出库") == 0) chkValue = U8Operation.GetDataString("select id from " + dbname + "..TransVouchs a where autoid=0" + BodyData.Rows[i]["itransid"], Cmd);
                if (chkValue.CompareTo("") == 0) throw new Exception("生单模式必须根据调拨单出库 或 形态转换单出库");

                itrid = chkValue;
            }

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            if (c_body_batch.CompareTo("") == 0 && cfirstToFirst.CompareTo("auto") == 0)  //没有批次且执行自动制定批次出库
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, BodyData.Rows[i]["cposcode"] + "",
                    GetBodyValue_FromData(BodyData, i, "cfree1"), GetBodyValue_FromData(BodyData, i, "cfree2"), GetBodyValue_FromData(BodyData, i, "cfree3"),
                    GetBodyValue_FromData(BodyData, i, "cfree4"), GetBodyValue_FromData(BodyData, i, "cfree5"), GetBodyValue_FromData(BodyData, i, "cfree6"),
                    GetBodyValue_FromData(BodyData, i, "cfree7"), GetBodyValue_FromData(BodyData, i, "cfree8"), GetBodyValue_FromData(BodyData, i, "cfree9"),
                    GetBodyValue_FromData(BodyData, i, "cfree10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            #endregion

            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                KK_U8Com.U8Rdrecords09 records09 = new KK_U8Com.U8Rdrecords09(Cmd, dbname);
                int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
                records09.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();

                records09.ID = rd_id;
                records09.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                records09.iQuantity = "" + dtMer.Rows[m]["iquantity"];
                irdrow++;
                records09.irowno = irdrow;
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records09.cInvCode + "", Cmd);

                #region //自由项  自定义项处理
                records09.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                records09.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                records09.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                records09.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                records09.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                records09.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                records09.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                records09.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                records09.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                records09.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                records09.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
                records09.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
                records09.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
                records09.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
                records09.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
                records09.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                records09.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
                records09.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
                records09.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
                records09.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
                records09.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
                records09.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
                records09.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
                records09.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
                records09.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
                records09.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

                #endregion

                #region //获得单价与金额
                string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records09.cInvCode, Cmd);
                if (c_ppcost != "")
                {
                    records09.iPUnitCost = c_ppcost;
                    records09.iPPrice = "" + (decimal.Parse(c_ppcost) * decimal.Parse("" + dtMer.Rows[m]["iquantity"]));
                }
                string c_unitcost = GetBodyValue_FromData(BodyData, i, "iunitcost");
                if (c_unitcost != "")
                {
                    records09.iPrice = "" + Math.Round(Convert.ToDecimal(c_unitcost) * Convert.ToDecimal(dtMer.Rows[m]["iquantity"]), 2);
                    records09.iUnitCost = c_unitcost;
                }
                #endregion

                #region //代管挂账处理
                records09.cvmivencode = "''";
                records09.bCosting = b_costing; //是否计入成本
                if (b_Vmi)
                {
                    records09.bVMIUsed = "1";
                    records09.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理
                records09.cBatch = "''";
                records09.iExpiratDateCalcu = "0";
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records09.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" == "") throw new Exception(records09.cInvCode + "有批次管理，必须输入批号");
                    records09.cBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                    //保质期管理
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records09.cInvCode + " and bInvQuality=1", Cmd) > 0)
                    {
                        records09.iExpiratDateCalcu = dtMer.Rows[m]["有效期推算方式"] + "";
                        records09.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                        records09.dVDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records09.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records09.dMadeDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                        records09.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                    }

                    //批次档案 建档
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + records09.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                    {
                        DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + records09.cInvCode + " and cBatch=" + records09.cBatch + " and isnull(cFree1,'')=" + records09.cFree1 + @" 
                                and isnull(cFree2,'')=" + records09.cFree2 + " and isnull(cFree3,'')=" + records09.cFree3 + " and isnull(cFree4,'')=" + records09.cFree4 + @" 
                                and isnull(cFree5,'')=" + records09.cFree5 + " and isnull(cFree6,'')=" + records09.cFree6 + " and isnull(cFree7,'')=" + records09.cFree7 + @" 
                                and isnull(cFree8,'')=" + records09.cFree8 + " and isnull(cFree9,'')=" + records09.cFree9 + " and isnull(cFree10,'')=" + records09.cFree10, "dtBatPerp", Cmd);
                        if (dtBatPerp.Rows.Count > 0)
                        {
                            records09.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                            records09.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                            records09.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                            records09.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                            records09.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                            records09.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                            records09.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                            records09.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                            records09.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                            records09.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                        }
                    }
                }
                #endregion

                #region //固定换算率（多计量）
                records09.iNum = "null";
                if (cst_unitcode.CompareTo("") != 0)
                {
                    records09.cAssUnit = "'" + cst_unitcode + "'";
                    string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + records09.cAssUnit, Cmd);
                    if (ichange == "") ichange = "0";
                    if (float.Parse(ichange) == 0)
                    {
                        //浮动换算率
                        inum = GetBodyValue_FromData(BodyData, i, "inum");
                        if (inum == "")
                        {
                            inum = "0";
                            //if (float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                            records09.iinvexchrate = "null";
                        }
                        else
                        {
                            //浮动换算率
                            ichange = U8Operation.GetDataString("select round(" + records09.iQuantity + "/" + inum + ",5)", Cmd);
                            records09.iinvexchrate = ichange;
                        }
                        records09.iNum = inum;
                    }
                    else
                    {
                        //固定换算率
                        //if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                        records09.iinvexchrate = ichange;
                        inum = U8Operation.GetDataString("select round(" + records09.iQuantity + "/" + ichange + ",5)", Cmd);
                        records09.iNum = inum;
                    }

                    
                }
                #endregion

                #region //项目目录
                string c_ItemClass = GetBodyValue_FromData(BodyData, i, "citemclass");
                if (c_ItemClass != "")  //存在项目目录
                {
                    string c_ItemCode = GetBodyValue_FromData(BodyData, i, "citemcode");
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..fitemss" + c_ItemClass + " where citemcode='" + c_ItemCode + "'", Cmd) == 0)
                        throw new Exception("项目[" + c_ItemCode + "]不存在");

                    records09.cItem_class = "'" + c_ItemClass + "'";
                    records09.cItemCName = "'" + U8Operation.GetDataString("select citem_name from " + dbname + "..fitem(nolock) where citem_class='" + c_ItemClass + "'", Cmd) + "'";
                    records09.cItemCode = "'" + c_ItemCode + "'";
                    records09.cName = "'" + U8Operation.GetDataString("select citemname from " + dbname + "..fitemss" + c_ItemClass + " where citemcode='" + c_ItemCode + "'", Cmd) + "'";
                }
                #endregion

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                records09.iNQuantity = records09.iQuantity;
                records09.iordertype = "0";
                if (bCreate)
                {
                    records09.iTrIds = itransid;

                    //继承到货单的表体自由项  自定义项数据
                    if (w_bustype.CompareTo("调拨出库") == 0)
                    {
                        dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..TransVouchs a(nolock) 
                        where autoID=0" + itransid, "dtPuArr", Cmd);
                    }
                    if (w_bustype.CompareTo("转换出库") == 0)
                    {
                        dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..AssemVouchs a(nolock) 
                        where autoID=0" + itransid, "dtPuArr", Cmd);
                    }
                    if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到调拨单/形态转换单信息");

                    if (records09.cDefine22.CompareTo("''") == 0) records09.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                    if (records09.cDefine23.CompareTo("''") == 0) records09.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                    if (records09.cDefine24.CompareTo("''") == 0) records09.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                    if (records09.cDefine25.CompareTo("''") == 0) records09.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                    if (records09.cDefine22.CompareTo("''") == 0) records09.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                    if (records09.cDefine22.CompareTo("''") == 0) records09.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                    if (records09.cDefine30.CompareTo("''") == 0) records09.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                    if (records09.cDefine31.CompareTo("''") == 0) records09.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                    if (records09.cDefine32.CompareTo("''") == 0) records09.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                    if (records09.cDefine33.CompareTo("''") == 0) records09.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos)
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + records09.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records09.cInvCode + "的货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + record09.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                    records09.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                #endregion

                #region  //严格管控先进先出法
                if (c_firstTofisrt_Ctl.CompareTo("true") == 0 && records09.cBatch.CompareTo("''") != 0 && c_firstTofisrt_NotCtl_WareList.IndexOf("," + w_cwhcode + ",") < 0)
                {
                    //查询是否存在比当前批次更小的批次库存
                    string cLowBatch = "" + U8Operation.GetDataString("select top 1 cBatch from " + dbname + @"..CurrentStock(nolock)
                        where cWhCode=" + record09.cWhCode + " and cInvCode=" + records09.cInvCode + " and cBatch<" + records09.cBatch + " and iQuantity>0 order by cBatch", Cmd);
                    if (cLowBatch != "")
                        throw new Exception("存货档案[" + BodyData.Rows[i]["cinvcode"] + "] 存在更小批次[" + cLowBatch + "],需先出库");
                }
                #endregion

                #region //设备DID记录
                if (w_bustype == "备件领用")
                {
                    string i_eq_did = GetBodyValue_FromData(BodyData, i, "ieqdid");
                    if (i_eq_did == "")
                    {
                        string eq_itemcode = GetBodyValue_FromData(BodyData, i, "eq_itemcode");
                        if(eq_itemcode=="") throw new Exception("设备作业单中的备件必须 作业项目号");
                        //添加配件申请记录
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..EQ_WorkItem(nolock) 
                            where cbillcode=" + record09.cBusCode + " and citemcode='" + eq_itemcode + "'", Cmd) == 0)
                        {
                            throw new Exception("配件" + records09.cInvCode + "项目号'" + eq_itemcode + "'不存在");
                        }
                        string cJldw = "" + U8Operation.GetDataString("select cComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records09.cInvCode, Cmd);
                        Cmd.CommandText = "insert into " + dbname + @"..EQ_WorkInventory(cbillcode,citemcode,cinvcode,fValue,cwhcode,intRef,cJldw) 
                            values(" + record09.cBusCode + ",'" + eq_itemcode + "'," + records09.cInvCode + ",0," + record09.cWhCode + ",2,'" + cJldw + "')";
                        Cmd.ExecuteNonQuery();
                        i_eq_did = "" + U8Operation.GetDataString("select IDENT_CURRENT( '" + dbname + @"..EQ_WorkInventory' )", Cmd);
                    }

                    records09.iEqDID = i_eq_did;
                    //更新实际出库数
                    Cmd.CommandText = "update " + dbname + "..EQ_WorkInventory set fFactNum=isnull(fFactNum,0)+(0" + records09.iQuantity + ") where autoid=0" + i_eq_did;
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                //保存数据
                if (!records09.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region//货位账务处理
                if (b_Pos)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                        "Values (" + cAutoid + "," + rd_id + "," + record09.cWhCode + "," + records09.cPosition + "," + records09.cInvCode + "," + records09.iQuantity + "," + records09.iNum +
                        ",null," + record09.dDate + ",0,'',0," + record09.cVouchType + "," + record09.dDate + "," + record09.cMaker + "," + records09.cvmivencode + "," + records09.cBatch +
                        "," + records09.cFree1 + "," + records09.cFree2 + "," + records09.cFree3 + "," + records09.cFree4 + "," + records09.cFree5 + "," +
                        records09.cFree6 + "," + records09.cFree7 + "," + records09.cFree8 + "," + records09.cFree9 + "," + records09.cFree10 + ",''," + records09.cAssUnit + @",
                        " + records09.dMadeDate + "," + records09.iMassDate + "," + records09.cMassUnit + "," + records09.iExpiratDateCalcu + "," + records09.cExpirationdate + "," + records09.dExpirationdate + ")";
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    if (fU8Version >= 11)
                    {
                        Cmd.CommandText = "update " + dbname + "..rdrecords09 set iposflag=1 where autoid =0" + cAutoid;
                        Cmd.ExecuteNonQuery();
                    }
                    //修改货位库存
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record09.cWhCode + " and cvmivencode=" + records09.cvmivencode + " and cinvcode=" + records09.cInvCode + @" 
                    and cPosCode=" + records09.cPosition + " and cbatch=" + records09.cBatch + " and cfree1=" + records09.cFree1 + " and cfree2=" + records09.cFree2 + " and cfree3=" + records09.cFree3 + @" 
                    and cfree4=" + records09.cFree4 + " and cfree5=" + records09.cFree5 + " and cfree6=" + records09.cFree6 + " and cfree7=" + records09.cFree7 + @" 
                    and cfree8=" + records09.cFree8 + " and cfree9=" + records09.cFree9 + " and cfree10=" + records09.cFree10, Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                        values(" + record09.cWhCode + "," + records09.cPosition + "," + records09.cInvCode + ",0," + records09.cBatch + @",
                            " + records09.cFree1 + "," + records09.cFree2 + "," + records09.cFree3 + "," + records09.cFree4 + "," + records09.cFree5 + "," +
                                records09.cFree6 + "," + records09.cFree7 + "," + records09.cFree8 + "," + records09.cFree9 + "," + records09.cFree10 + "," + records09.cvmivencode + @",'',0,
                            " + records09.dMadeDate + "," + records09.iMassDate + "," + records09.cMassUnit + "," + records09.iExpiratDateCalcu + "," + records09.cExpirationdate + "," + records09.dExpirationdate + ")";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + records09.iQuantity + "),inum=isnull(inum,0)-(" + records09.iNum + @") 
                        where cwhcode=" + record09.cWhCode + " and cvmivencode=" + records09.cvmivencode + " and cinvcode=" + records09.cInvCode + @" 
                        and cPosCode=" + records09.cPosition + " and cbatch=" + records09.cBatch + " and cfree1=" + records09.cFree1 + " and cfree2=" + records09.cFree2 + " and cfree3=" + records09.cFree3 + @" 
                        and cfree4=" + records09.cFree4 + " and cfree5=" + records09.cFree5 + " and cfree6=" + records09.cFree6 + " and cfree7=" + records09.cFree7 + @" 
                        and cfree8=" + records09.cFree8 + " and cfree9=" + records09.cFree9 + " and cfree10=" + records09.cFree10;
                    Cmd.ExecuteNonQuery();

                    //货位现存量
                    Current_Pos_StockCHeck(Cmd, dbname, record09.cWhCode, records09.cPosition, records09.cInvCode, records09.cBatch, records09.cvmivencode, records09.cFree1, records09.cFree2,
                        records09.cFree3, records09.cFree4, records09.cFree5, records09.cFree6, records09.cFree7, records09.cFree8, records09.cFree9, records09.cFree10);
                }
                #endregion

                #region  //是否超现存量出库
                Current_ST_StockCHeck(Cmd, dbname, record09.cWhCode, records09.cInvCode, records09.cBatch, records09.cvmivencode, records09.cFree1, records09.cFree2,
                    records09.cFree3, records09.cFree4, records09.cFree5, records09.cFree6, records09.cFree7, records09.cFree8, records09.cFree9, records09.cFree10);
                Current_ST_StockAvableCHeck(Cmd, dbname, record09.cWhCode, records09.cInvCode, records09.cBatch, records09.cvmivencode, records09.cFree1, records09.cFree2,
                    records09.cFree3, records09.cFree4, records09.cFree5, records09.cFree6, records09.cFree7, records09.cFree8, records09.cFree9, records09.cFree10);
                #endregion
            }


            #region//是否超调拨单检查
            if (bCreate && w_bustype.CompareTo("调拨出库") == 0)
            {
                float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) 
                    from " + dbname + "..rdrecords09 a(nolock) inner join " + dbname + @"..rdrecord09 b(nolock) on a.id=b.id 
                    where iTrIds=0" + itransid + " and b.cbustype='调拨出库'", Cmd));
                #region 
                //关键领用控制量，取最小值  
                float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iTVQuantity from " + dbname + "..TransVouchs(nolock) where autoid=0" + itransid, Cmd));
                if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超出调拨单数量");
                #endregion

            }
            #endregion

        }

        #endregion
        #endregion

        return rd_id + "," + cc_mcode;
    }

    //其他入库单 [直接入库 调拨入库]
    private string U81019(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; //出库类型
        string w_bustype = "";//业务类型
        string w_headPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_Pos = false;//货位
        string itrid = ""; string itransid = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_red_sheet = 0;
        #region  //逻辑检验
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) w_bustype = "其他入库";

        if (BodyData.Columns.Contains("itransid"))
        {
            itransid = BodyData.Rows[0]["itransid"] + "";
            if (itransid.CompareTo("") != 0)
            {
                bCreate = true;
                if (w_bustype.CompareTo("调拨入库") == 0)
                {
                    itrid = U8Operation.GetDataString("select id from " + dbname + "..TransVouchs where autoid=0" + itransid, Cmd);
                    if (itrid == "") throw new Exception("没有找到调拨单信息");
                }
                if (w_bustype.CompareTo("转换入库") == 0)
                {
                    itrid = U8Operation.GetDataString("select id from " + dbname + "..AssemVouchs where autoid=0" + itransid, Cmd);
                    if (itrid == "") throw new Exception("没有找到形态转换单信息");
                }
                
                
            }
        }
        if (float.Parse("" + BodyData.Rows[0]["iquantity"]) < 0) i_red_sheet = 1;//第一行数据的 数量判断红篮子
        
        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        //入库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //供应商
        w_vencode = GetTextsFrom_FormData_Tag(HeadData, "txt_cvencode");
        if (w_vencode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..vendor where cvencode='" + w_vencode + "'", Cmd) == 0)
                throw new Exception("供应商输入错误");
        }

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation(nolock) where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base(nolock) where VT_CardNumber='0301' and VT_TemplateMode=0", Cmd);

        string cc_mcode = "";//单据号
        #region //单据
        KK_U8Com.U8Rdrecord08 record08 = new KK_U8Com.U8Rdrecord08(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0) from ufsystem..UA_Identity where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "R" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord08(nolock) where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record08.cCode = "'" + cc_mcode + "'";
        record08.ID = rd_id;
        record08.cVouchType = "'08'";
        record08.bredvouch = "0"; //红篮子  
        record08.cBusType = "'" + w_bustype + "'";
        record08.cWhCode = "'" + w_cwhcode + "'";
        record08.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record08.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record08.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record08.cVenCode = (w_vencode.CompareTo("") == 0 ? "null" : "'" + w_vencode + "'");
        record08.VT_ID = vt_id;
        record08.cBusCode = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode") + "'";  //业务号
        record08.cSource = "'库存'";
        record08.bredvouch = i_red_sheet + "";

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse(nolock) where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record08.cMaker = "'" + cUserName + "'";
        record08.dDate = "'" + cLogDate + "'";
        record08.dnmaketime = "getdate()";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter(nolock) where cPid='u8barcode_rd08_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record08.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record08.cHandler = "'" + c_vou_checker + "'";
            }
            
            record08.dVeriDate = "'" + cLogDate + "'";
            record08.dnverifytime = "getdate()";
        }
        record08.iExchRate = "1";
        record08.cExch_Name = "'人民币'";
        record08.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";
        if (bCreate)
        {
            if (w_bustype.CompareTo("调拨入库") == 0) record08.cSource = "'调拨'";
            if (w_bustype.CompareTo("转换入库") == 0) record08.cSource = "'形态转换'";
        }
        else
        {
            record08.cSource = "'库存'";
        }

        #region   //主表自定义项处理
        record08.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record08.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record08.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record08.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record08.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record08.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record08.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record08.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record08.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record08.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record08.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record08.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record08.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record08.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record08.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record08.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        if (!record08.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region //子表
        //判断仓库是否计入成本
        string b_costing = U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse(nolock) where cwhcode=" + record08.cWhCode, Cmd);

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            float f_the_qty = float.Parse("" + BodyData.Rows[i]["iquantity"]);
            if ((f_the_qty > 0 && i_red_sheet == 1) || (f_the_qty < 0 && i_red_sheet == 0)) throw new Exception("本单据红蓝子混乱");
            if (f_the_qty == 0) throw new Exception("出入库数量不能为空 或0");

            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            itransid = BodyData.Rows[i]["itransid"] + "";
            itrid = "";
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string chkValue = "";
                if (w_bustype.CompareTo("调拨入库") == 0) chkValue = U8Operation.GetDataString("select id from " + dbname + "..TransVouchs a(nolock) where autoid=0" + BodyData.Rows[i]["itransid"], Cmd);
                if (w_bustype.CompareTo("转换入库") == 0) chkValue = U8Operation.GetDataString("select id from " + dbname + "..AssemVouchs a(nolock) where autoid=0" + BodyData.Rows[i]["itransid"], Cmd);
                if (chkValue.CompareTo("") == 0) throw new Exception("生单模式必须根据调拨单/或形态转换单入库");

                itrid = chkValue;
            }

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory(nolock) where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            KK_U8Com.U8Rdrecords08 records08 = new KK_U8Com.U8Rdrecords08(Cmd, dbname);
            int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
            records08.AutoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
            Cmd.ExecuteNonQuery();

            records08.ID = rd_id;
            records08.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            records08.iQuantity = "" + BodyData.Rows[i]["iquantity"];
            records08.irowno = (i + 1);
            cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory(nolock) where cinvcode=" + records08.cInvCode + "", Cmd);

            #region //自由项  自定义项处理
            records08.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            records08.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            records08.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            records08.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            records08.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            records08.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            records08.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            records08.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            records08.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            records08.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            //自定义项
            records08.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            records08.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            records08.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            records08.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            records08.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            records08.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            records08.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            records08.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            records08.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            records08.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            records08.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            records08.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            records08.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            records08.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            records08.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            records08.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            //自由项检验
            for (int f = 1; f < 11; f++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records08.cInvCode + " and bFree" + f + "=1", Cmd) > 0)
                {
                    if (GetBodyValue_FromData(BodyData, i, "cfree" + f) == "")
                        throw new Exception(records08.cInvCode + "有自由项" + f + "管理，必须录入");
                }
            }

            #endregion

            #region //获得单价与金额
            string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records08.cInvCode, Cmd);
            if (c_ppcost != "")
            {
                records08.iPUnitCost = c_ppcost;
                records08.iPPrice = "" + (decimal.Parse(c_ppcost) * decimal.Parse("" + BodyData.Rows[i]["iquantity"]));
            }
            string c_price = GetBodyValue_FromData(BodyData, i, "iprice");
            if (c_price != "")
            {
                records08.iPrice = c_price;
                records08.iUnitCost = "" + (Convert.ToDecimal(c_price) / Convert.ToDecimal(BodyData.Rows[i]["iquantity"]));
            }
            #endregion

            #region //代管挂账处理
            records08.cvmivencode = "''";
            records08.bCosting = b_costing; //是否计入成本
            if (b_Vmi)
            {
                records08.bVMIUsed = "0";
                records08.bCosting = "0";
                records08.cvmivencode = "'" + cvmivencode + "'";
            }
            #endregion

            #region //批次管理和保质期管理
            records08.cBatch = "''";
            records08.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records08.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(records08.cInvCode + "有批次管理，必须输入批号");
                records08.cBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory(nolock) where cinvcode=" + records08.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    if ((!BodyData.Columns.Contains("dprodate")) || BodyData.Rows[i]["dprodate"] + "" == "")  //生产日期判定
                        throw new Exception(records08.cInvCode + "有保质期管理，必须输入生产日期");
                    string rowpordate = "" + BodyData.Rows[i]["dprodate"];
                    if (rowpordate == "") rowpordate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
                    DataTable dtBZQ = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                                else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                            ,120) 失效日期,isnull(s.iExpiratDateCalcu,1) 有效期推算方式
                        from " + dbname + "..inventory i(nolock) left join " + dbname + @"..Inventory_Sub s(nolock) on i.cinvcode=s.cinvsubcode 
                        where cinvcode=" + records08.cInvCode, "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count == 0) throw new Exception("计算存货保质期出现错误");

                    records08.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    records08.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    records08.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records08.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    records08.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    records08.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub(nolock) where cInvSubCode=" + records08.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    //从模板中获得批次属性
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty1");
                    records08.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty2");
                    records08.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty3");
                    records08.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty4");
                    records08.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty5");
                    records08.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    records08.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty6") + "'";
                    records08.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty7") + "'";
                    records08.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty8") + "'";
                    records08.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty9") + "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty10");
                    records08.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                    //继承批次档案数据
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a(nolock) 
                            where cInvCode=" + records08.cInvCode + " and cBatch=" + records08.cBatch + " and isnull(cFree1,'')=" + records08.cFree1 + @" 
                                and isnull(cFree2,'')=" + records08.cFree2 + " and isnull(cFree3,'')=" + records08.cFree3 + " and isnull(cFree4,'')=" + records08.cFree4 + @" 
                                and isnull(cFree5,'')=" + records08.cFree5 + " and isnull(cFree6,'')=" + records08.cFree6 + " and isnull(cFree7,'')=" + records08.cFree7 + @" 
                                and isnull(cFree8,'')=" + records08.cFree8 + " and isnull(cFree9,'')=" + records08.cFree9 + " and isnull(cFree10,'')=" + records08.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count > 0)
                    {
                        records08.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                        records08.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                        records08.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                        records08.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                        records08.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                        records08.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                        records08.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                        records08.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                        records08.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                        records08.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";

                        if (w_bustype.CompareTo("调拨入库") != 0)
                        {
                            //更新批次档案
                            Cmd.CommandText = "update " + dbname + "..AA_BatchProperty set cBatchProperty1=" + records08.cBatchProperty1 + ",cBatchProperty2=" + records08.cBatchProperty2 + @",
                                cBatchProperty3=" + records08.cBatchProperty3 + ",cBatchProperty4=" + records08.cBatchProperty4 + ",cBatchProperty5=" + records08.cBatchProperty5 + @",
                                cBatchProperty6=" + records08.cBatchProperty6 + ",cBatchProperty7=" + records08.cBatchProperty7 + ",cBatchProperty8=" + records08.cBatchProperty8 + @",
                                cBatchProperty9=" + records08.cBatchProperty9 + ",cBatchProperty10=" + records08.cBatchProperty10 + @" 
                            where cInvCode=" + records08.cInvCode + " and cBatch=" + records08.cBatch + " and isnull(cFree1,'')=" + records08.cFree1 + @" 
                                and isnull(cFree2,'')=" + records08.cFree2 + " and isnull(cFree3,'')=" + records08.cFree3 + " and isnull(cFree4,'')=" + records08.cFree4 + @" 
                                and isnull(cFree5,'')=" + records08.cFree5 + " and isnull(cFree6,'')=" + records08.cFree6 + " and isnull(cFree7,'')=" + records08.cFree7 + @" 
                                and isnull(cFree8,'')=" + records08.cFree8 + " and isnull(cFree9,'')=" + records08.cFree9 + " and isnull(cFree10,'')=" + records08.cFree10;
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else  //建立档案
                    {
                        if (w_bustype.CompareTo("调拨入库") != 0)
                        {
                            Cmd.CommandText = "insert into " + dbname + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                            values(newid()," + records08.cBatchProperty1 + "," + records08.cBatchProperty2 + "," + records08.cBatchProperty3 + "," + records08.cBatchProperty4 + "," +
                                     records08.cBatchProperty5 + "," + records08.cBatchProperty6 + "," + records08.cBatchProperty7 + "," + records08.cBatchProperty8 + "," +
                                     records08.cBatchProperty9 + "," + records08.cBatchProperty10 + "," + records08.cInvCode + "," + records08.cBatch + "," + records08.cFree1 + "," +
                                     records08.cFree2 + "," + records08.cFree3 + "," + records08.cFree4 + "," + records08.cFree5 + "," + records08.cFree6 + "," +
                                     records08.cFree7 + "," + records08.cFree8 + "," + records08.cFree9 + "," + records08.cFree10 + ")";
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            #endregion

            #region //固定换算率（多计量）
            records08.iNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                records08.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit(nolock) where cComunitCode=" + records08.cAssUnit, Cmd);

                if (ichange == "") ichange = "0";
                if (float.Parse(ichange) == 0)
                {
                    //浮动换算率
                    inum = GetBodyValue_FromData(BodyData, i, "inum");
                    if (inum == "")
                    {
                        inum = "0";
                        records08.iinvexchrate = "null";
                    }
                    else
                    {
                        //浮动换算率
                        ichange = U8Operation.GetDataString("select round(" + records08.iQuantity + "/" + inum + ",5)", Cmd);
                        records08.iinvexchrate = ichange;
                    }
                    records08.iNum = inum;
                }
                else
                {
                    //固定换算率
                    records08.iinvexchrate = ichange;
                    inum = U8Operation.GetDataString("select round(" + records08.iQuantity + "/" + ichange + ",5)", Cmd);
                    records08.iNum = inum;
                }

            }
            #endregion


            #region//上游单据关联
            DataTable dtPuArr = null;  //上游单据表体自定义项继承
            records08.iNQuantity = records08.iQuantity;
            records08.iordertype = "0";
            if (bCreate)
            {
                records08.iTrIds = itransid;

                //继承到货单的表体自由项  自定义项数据
                if (w_bustype.CompareTo("转换入库") == 0)
                {
                    dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..AssemVouchs a(nolock) 
                        where autoID=0" + itransid, "dtPuArr", Cmd);
                }
                else
                {
                    dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..TransVouchs a(nolock) 
                        where autoID=0" + itransid, "dtPuArr", Cmd);
                }
                if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到调拨单/形态转换单信息");

                if (records08.cDefine22.CompareTo("''") == 0) records08.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                if (records08.cDefine23.CompareTo("''") == 0) records08.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                if (records08.cDefine24.CompareTo("''") == 0) records08.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                if (records08.cDefine25.CompareTo("''") == 0) records08.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                if (records08.cDefine22.CompareTo("''") == 0) records08.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                if (records08.cDefine22.CompareTo("''") == 0) records08.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                if (records08.cDefine30.CompareTo("''") == 0) records08.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                if (records08.cDefine31.CompareTo("''") == 0) records08.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                if (records08.cDefine32.CompareTo("''") == 0) records08.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                if (records08.cDefine33.CompareTo("''") == 0) records08.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

            }
            #endregion

            #region//货位标识处理  ，若无货位取存货档案的默认货位
            if (b_Pos)
            {
                if (BodyData.Rows[i]["cposcode"] + "" == "" && w_headPosCode != "") BodyData.Rows[i]["cposcode"] = w_headPosCode;
                if (BodyData.Rows[i]["cposcode"] + "" == "")
                {
                    BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory(nolock) where cinvcode=" + records08.cInvCode, Cmd);
                    if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records08.cInvCode + "的货位不能为空");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position(nolock) where cwhcode=" + record08.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                    throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                records08.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
            }
            #endregion

            //保存数据
            if (!records08.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            #region//货位账务处理
            if (b_Pos)
            {
                //添加货位记录 
                Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                    "Values (" + cAutoid + "," + rd_id + "," + record08.cWhCode + "," + records08.cPosition + "," + records08.cInvCode + "," + records08.iQuantity + "," + records08.iNum +
                    ",null," + record08.dDate + ",1,'',0," + record08.cVouchType + "," + record08.dDate + "," + record08.cMaker + "," + records08.cvmivencode + "," + records08.cBatch +
                    "," + records08.cFree1 + "," + records08.cFree2 + "," + records08.cFree3 + "," + records08.cFree4 + "," + records08.cFree5 + "," +
                    records08.cFree6 + "," + records08.cFree7 + "," + records08.cFree8 + "," + records08.cFree9 + "," + records08.cFree10 + ",''," + records08.cAssUnit + @",
                        " + records08.dMadeDate + "," + records08.iMassDate + "," + records08.cMassUnit + "," + records08.iExpiratDateCalcu + "," + records08.cExpirationdate + "," + records08.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();

                //指定货位
                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords08 set iposflag=1 where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                //修改货位库存
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record08.cWhCode + " and cvmivencode=" + records08.cvmivencode + " and cinvcode=" + records08.cInvCode + @" 
                    and cPosCode=" + records08.cPosition + " and cbatch=" + records08.cBatch + " and cfree1=" + records08.cFree1 + " and cfree2=" + records08.cFree2 + " and cfree3=" + records08.cFree3 + @" 
                    and cfree4=" + records08.cFree4 + " and cfree5=" + records08.cFree5 + " and cfree6=" + records08.cFree6 + " and cfree7=" + records08.cFree7 + @" 
                    and cfree8=" + records08.cFree8 + " and cfree9=" + records08.cFree9 + " and cfree10=" + records08.cFree10, Cmd) == 0)
                {
                    Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate,dVDate) 
                        values(" + record08.cWhCode + "," + records08.cPosition + "," + records08.cInvCode + ",0," + records08.cBatch + @",
                            " + records08.cFree1 + "," + records08.cFree2 + "," + records08.cFree3 + "," + records08.cFree4 + "," + records08.cFree5 + "," +
                            records08.cFree6 + "," + records08.cFree7 + "," + records08.cFree8 + "," + records08.cFree9 + "," + records08.cFree10 + "," + records08.cvmivencode + @",'',0,
                            " + records08.dMadeDate + "," + records08.iMassDate + "," + records08.cMassUnit + "," + records08.iExpiratDateCalcu + "," + records08.cExpirationdate + @",
                            " + records08.dExpirationdate + "," + records08.dVDate + ")";
                    Cmd.ExecuteNonQuery();
                }
                Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + records08.iQuantity + "),inum=isnull(inum,0)+(" + records08.iNum + @") 
                        where cwhcode=" + record08.cWhCode + " and cvmivencode=" + records08.cvmivencode + " and cinvcode=" + records08.cInvCode + @" 
                        and cPosCode=" + records08.cPosition + " and cbatch=" + records08.cBatch + " and cfree1=" + records08.cFree1 + " and cfree2=" + records08.cFree2 + " and cfree3=" + records08.cFree3 + @" 
                        and cfree4=" + records08.cFree4 + " and cfree5=" + records08.cFree5 + " and cfree6=" + records08.cFree6 + " and cfree7=" + records08.cFree7 + @" 
                        and cfree8=" + records08.cFree8 + " and cfree9=" + records08.cFree9 + " and cfree10=" + records08.cFree10;
                Cmd.ExecuteNonQuery();
            }
            #endregion

            #region//是否超调拨单检查
            if (bCreate && w_bustype.CompareTo("调拨入库") == 0)
            {
                float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) 
                    from " + dbname + "..rdrecords08 a(nolock) inner join " + dbname + @"..rdrecord08 b(nolock) on a.id=b.id 
                    where iTrIds=0" + itransid + " and b.cbustype='调拨入库'", Cmd));
                #region 
                //关键领用控制量，取最小值  
                float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iTVQuantity from " + dbname + "..TransVouchs where autoid=0" + itransid, Cmd));
                if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超出调拨单数量");
                #endregion

            }
            #endregion

        }

        #endregion
        #endregion

        return rd_id + "," + cc_mcode;
    }

    //销售发货单 [销售订单发货 直接发货]
    public string U81020(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_cuscode = "";
        string w_stcode = ""; //销售类型
        string w_bustype = "普通销售";//业务类型
        bool b_Vmi = false; bool b_Pos = false; //货位
        string isoid = ""; string iSOsID = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        bool b_SaCreateRd32 = false;//发货单审核自动生成销售出库单
        bool b_SaMustInputBatch = true; //销售系统必须输入批号
        bool bXiaotui = false; //销售退货

        if (U8Operation.GetDataInt("select CAST(CAST(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='st' and cName='bSAcreat'", Cmd) == 1)
        {
            b_SaCreateRd32 = true;
        }
        if (U8Operation.GetDataInt("select CAST(CAST(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='sa' and cName='bBatch'", Cmd) == 0)
        {
            b_SaMustInputBatch = false;
        }

        #region  //逻辑检验
        if (BodyData.Columns.Contains("isosid"))
        {
            iSOsID = BodyData.Rows[0]["isosid"] + "";
            if (iSOsID.CompareTo("") != 0)
            {
                bCreate = true;
                isoid = U8Operation.GetDataString("select ID from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, Cmd);
                if (isoid == "")
                {
                    isoid = U8Operation.GetDataString("select ID from " + dbname + "..SA_ReturnsApplyDetail where AutoID=0" + iSOsID, Cmd);
                    if (isoid == "")
                        throw new Exception("没有找到销售订单信息或者退货申请单信息");
                    bXiaotui = true;
                }
                else
                {
                    string testisoid = U8Operation.GetDataString("select ID from " + dbname + "..SA_ReturnsApplyDetail where AutoID=0" + iSOsID + " and ID = 0" + BodyData.Rows[0]["soid"], Cmd);
                    if (testisoid != "")
                    {
                        bXiaotui = true;
                        isoid = testisoid;
                    }
                }
            }
        }

        //业务类型
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0)
            w_bustype = U8Operation.GetDataString("select cbustype from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd);

        if (w_bustype.CompareTo("") == 0)
            w_bustype = U8Operation.GetDataString("select cbustype from " + dbname + "..SA_ReturnsApplyMain where id=0" + BodyData.Rows[0]["soid"], Cmd);

        //客户校验
        w_cuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuscode");
        if (w_cuscode.CompareTo("") == 0)
        {
            w_cuscode = U8Operation.GetDataString("select ccuscode from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd);
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..customer where ccuscode='" + w_cuscode + "'", Cmd) == 0)
            throw new Exception("客户输入错误");

        //销售类别
        w_stcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cstcode");
        if (w_stcode.CompareTo("") == 0)
            w_stcode = U8Operation.GetDataString("select cstcode from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd);
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..SaleType where cSTCode='" + w_stcode + "'", Cmd) == 0)
            throw new Exception("销售类型输入错误");

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") == 0)
            w_cdepcode = U8Operation.GetDataString("select cdepcode from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd);
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
            throw new Exception("部门输入错误");

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        else
        {
            if (bXiaotui)
            {
                w_cpersoncode = U8Operation.GetDataString("select cpersoncode from " + dbname + "..SA_ReturnsApplyMain where id=0" + isoid, Cmd);            
            }
            else
            {
                w_cpersoncode = U8Operation.GetDataString("select cpersoncode from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd);
            }
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        #region //新增单据
        KK_U8Com.U8DispatchList dismain = new KK_U8Com.U8DispatchList(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='DISPATCH'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='DISPATCH'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "F" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cdlcode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..DispatchList where cdlcode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        dismain.cDLCode = "'" + cc_mcode + "'";
        dismain.DLID = rd_id;
        dismain.cSTCode = (w_stcode.CompareTo("") == 0 ? "null" : "'" + w_stcode + "'");
        dismain.cDepCode = "'" + w_cdepcode + "'";
        dismain.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        dismain.cCusCode = "'" + w_cuscode + "'";
        dismain.cVouchType = "'05'";
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='01' and VT_TemplateMode=0", Cmd);
        dismain.iVTid = vt_id;
        dismain.cBusType = "'" + w_bustype + "'";
        //红蓝字判定
        if (decimal.Parse(BodyData.Rows[0]["iquantity"] + "") < 0)
        {
            dismain.bReturnFlag = 1;
        }
        else
        {
            dismain.bReturnFlag = 0;
        }

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        dismain.cMaker = "'" + cUserName + "'";
        dismain.dDate = "'" + cLogDate + "'";
        dismain.cMemo = "''";
        dismain.iswfcontrolled = 0;
        
        if (!bCreate)
        {
            dismain.iExchRate = 1;
            dismain.cexch_name = "'人民币'";
            dismain.iTaxRate = 13;
        }
        else
        {
            if (bXiaotui)
            {
                dismain.iTaxRate = float.Parse(U8Operation.GetDataString("select isnull(max(iTaxRate),0) from " + dbname + "..SA_ReturnsApplyMain where id=0" + isoid, Cmd));
                dismain.iExchRate = float.Parse(U8Operation.GetDataString("select iExchRate from " + dbname + "..SA_ReturnsApplyMain where id=0" + isoid, Cmd));
                dismain.cexch_name = "'" + U8Operation.GetDataString("select cexch_name from " + dbname + "..SA_ReturnsApplyMain where id=0" + isoid, Cmd) + "'";                
            }
            else
            {
                dismain.iTaxRate = float.Parse(U8Operation.GetDataString("select isnull(max(iTaxRate),0) from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd));
                dismain.iExchRate = float.Parse(U8Operation.GetDataString("select iExchRate from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd));
                dismain.cexch_name = "'" + U8Operation.GetDataString("select cexch_name from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd) + "'";
            }
        }

        #region   //主表自定义项处理
        dismain.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        dismain.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        dismain.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        dismain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        dismain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        dismain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        dismain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        dismain.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        dismain.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        dismain.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        dismain.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        dismain.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        dismain.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        dismain.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        dismain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        dismain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        #region   //继承第一张上游单据的表头自定义项
        DataTable dtUpVouchHeadDefine = U8Operation.GetSqlDataTable(@"select cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,
                cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 from " + dbname + @"..SO_SOMain a 
            where ID=0" + isoid, "dtPuArr", Cmd);
        if (dtUpVouchHeadDefine.Rows.Count > 0)
        {
            if (dismain.cDefine1.CompareTo("''") == 0) dismain.cDefine1 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine1") + "'";
            if (dismain.cDefine2.CompareTo("''") == 0) dismain.cDefine2 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine2") + "'";
            if (dismain.cDefine3.CompareTo("''") == 0) dismain.cDefine3 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine3") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(HeadData, "cdefine4");
            if (dismain.cDefine4.CompareTo("null") == 0) dismain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetTextsFrom_FormData_Text(HeadData, "cdefine6");
            if (dismain.cDefine6.CompareTo("null") == 0) dismain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            if (dismain.cDefine8.CompareTo("''") == 0) dismain.cDefine8 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine8") + "'";
            if (dismain.cDefine9.CompareTo("''") == 0) dismain.cDefine9 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine9") + "'";
            if (dismain.cDefine10.CompareTo("''") == 0) dismain.cDefine10 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine10") + "'";
            if (dismain.cDefine11.CompareTo("''") == 0) dismain.cDefine11 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine11") + "'";
            if (dismain.cDefine12.CompareTo("''") == 0) dismain.cDefine12 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine12") + "'";
            if (dismain.cDefine13.CompareTo("''") == 0) dismain.cDefine13 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine13") + "'";
            if (dismain.cDefine14.CompareTo("''") == 0) dismain.cDefine14 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine14") + "'";
        }
        #endregion

        #region //账期管理
        string c_jz_date = U8Operation.GetDataString(@"SELECT convert(varchar(10),dEnd,120) FROM ufsystem.dbo.UA_Period
                    where cAcc_Id='" + targetAccId + "' and dBegin<='" + cLogDate + "' and dEnd>='" + cLogDate + "'", Cmd);
        if (c_jz_date.CompareTo("") == 0) throw new Exception("没有找到日期'" + cLogDate + "'在账套【" + targetAccId + "】中的月份");
        DataTable dtZQ = U8Operation.GetSqlDataTable(@"select a.cCusSAProtocol,case when iLZYJ=0 then 1 else 0 end 是否立账依据,
                case when iLZFS=2 then (
                         case when day('" + cLogDate + "')>iday1 then convert(varchar(8),dateadd(month,1,'" + cLogDate + @"'),120)+cast((case when iday1=0 then 1 else iday1 end) as varchar(2))  
                         else convert(varchar(8),'" + cLogDate + @"',120)+cast(iday1 as varchar(2)) end
                        ) 
                     when iLZFS=1 then '" + c_jz_date + "' else '" + cLogDate + @"' end  立账日期,
                case when iZQ=1 then isnull(a.iCusCreDate,0) else isnull(dblzqnum,0) end 账期天数
            from " + dbname + @"..customer a inner join " + dbname + @"..AA_Agreement b on a.cCusSAProtocol=b.ccode where a.ccuscode=" + dismain.cCusCode, "dtZQ", Cmd);
        if (dtZQ.Rows.Count > 0)
        {
            dismain.cgatheringplan = "'" + dtZQ.Rows[0]["cCusSAProtocol"] + "'";  //收款协议
            string c_cur_day = "" + dtZQ.Rows[0]["立账日期"];
            string c_new_lzr = U8Operation.GetDataString("select convert(varchar(10),dateadd(dd,-1,dateadd(m,1,'" + c_cur_day.Substring(0, 7) + "'+'-1')),120)", Cmd);//本月最后一天
            dismain.dCreditStart = "'" + (c_cur_day.CompareTo(c_new_lzr) <= 0 ? c_cur_day : c_new_lzr) + "'";  //立账日
            dismain.dGatheringDate = "'" + U8Operation.GetDataString("select convert(varchar(10),dateadd(day," + dtZQ.Rows[0]["账期天数"] + "," + dismain.dCreditStart + "),120)", Cmd) + "'";  //到期日
            dismain.icreditdays = "" + dtZQ.Rows[0]["账期天数"];  //账期天数
            dismain.bCredit = int.Parse(dtZQ.Rows[0]["是否立账依据"] + "");   //是否立账单据
        }
        #endregion

        if (!dismain.InsertToDB(targetAccId,ref errmsg)) { throw new Exception(errmsg); }

        //开票单位
        string c_kp_dw_code = U8Operation.GetDataString("select cInvoiceCompany from " + dbname + "..customer where ccuscode=" + dismain.cCusCode,Cmd);
        string i_flowid = "0";
        if (bXiaotui)
        { 
            i_flowid = U8Operation.GetDataString("select iflowid from " + dbname + "..SA_ReturnsApplyMain where ID=0" + isoid, Cmd);
        }
        else
        {
            i_flowid = U8Operation.GetDataString("select iflowid from " + dbname + "..SO_SOMain where ID=0" + isoid, Cmd);
        }
        Cmd.CommandText = "update " + dbname + @"..DispatchList set cinvoicecompany='" + c_kp_dw_code + "',iflowid=" + (i_flowid == "" ? "null" : i_flowid) + " where dlid=" + dismain.DLID;
        Cmd.ExecuteNonQuery();
        #endregion

        #region //子表
        #region   //创建缓存数据表
//        Cmd.CommandText = @"select cwhcode,cinvcode,cbatch cposcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,iquantity into #tmp_out_list 
//                from " + dbname + @"..CurrentStock where 1=0";
//        Cmd.ExecuteNonQuery();
        string tmp_out_tbl = @"(select cWhCode,cInvCode,cvmivencode,cbatch,isnull(cPosition,'') cposcode,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10,iquantity 
		    from " + dbname + "..DispatchLists where dlid=" + dismain.DLID + ")";
        #endregion

        int irdrow = 0;
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            if (dismain.bReturnFlag == 1 && decimal.Parse(BodyData.Rows[i]["iquantity"] + "") > 0) throw new Exception("退货单数量必须全部为负");
            if (dismain.bReturnFlag == 0 && decimal.Parse(BodyData.Rows[i]["iquantity"] + "") < 0) throw new Exception("发货单数量必须全部为正");

            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("发货数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库 必须有代管商");

            iSOsID = BodyData.Rows[i]["isosid"] + "";
            isoid = BodyData.Rows[i]["soid"] + "";
            //isoid = U8Operation.GetDataString("select ID from " + dbname + "..SO_SODetails where ID = 0" + BodyData.Rows[i]["soid"] + "and iSOsID=0" + iSOsID, Cmd);
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string isoid_chk = U8Operation.GetDataString("select ID from " + dbname + "..SA_ReturnsApplyDetail where ID = 0" + isoid + " and AutoID=0" + iSOsID, Cmd);
                if (isoid_chk == "")
                {
                    string chkValue = U8Operation.GetDataString("select id from " + dbname + "..SO_SODetails a where iSOsID=0" + BodyData.Rows[i]["isosid"], Cmd);
                    if (chkValue.CompareTo("") == 0) throw new Exception("生单模式必须根据销售订单发货,请确认扫描的销售订单存在");
                    isoid = chkValue;
                    chkValue = U8Operation.GetDataString("select ccuscode from " + dbname + "..SO_SOMain a where id=0" + isoid, Cmd);
                    if (chkValue.CompareTo(w_cuscode) != 0)
                    {
                        chkValue = U8Operation.GetDataString("select cCusCode from " + dbname + "..SA_ReturnsApplyMain a where id=0" + BodyData.Rows[i]["soid"], Cmd);
                        if (chkValue.CompareTo(w_cuscode) != 0)
                        {
                            throw new Exception("所有销售订单必须客户相同");
                        }
                    }
                    chkValue = U8Operation.GetDataString("select cbustype from " + dbname + "..SO_SOMain a where id=0" + isoid, Cmd);
                    if (chkValue.CompareTo(w_bustype) != 0) throw new Exception("所有销售订单必须业务类型相同");
                }
                else
                {
                    //string iSOsID_chk = U8Operation.GetDataString("select ISNULL(isosid,0) isosid from " + dbname + "..SA_ReturnsApplyDetail where ID = 0" + isoid + " and AutoID=0" + iSOsID, Cmd);
                    DataTable dtApplyDetailRow = U8Operation.GetSqlDataTable("select ISNULL(isosid,0) isosid from " + dbname + "..SA_ReturnsApplyDetail where ID = 0" + isoid + " and AutoID=0" + iSOsID + " and isosid is not null", "dtApplyDetailRow", Cmd);
                    if (dtApplyDetailRow.Rows.Count > 0)
                    {
                        if (int.Parse(dtApplyDetailRow.Rows[0]["isosid"] + "") > 0)
                        {
                            iSOsID = dtApplyDetailRow.Rows[0]["isosid"] + "";
                            isoid = U8Operation.GetDataString("select ID from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, Cmd);
                        }
                    }
                }
            }

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            if (c_body_batch.CompareTo("") == 0 && cfirstToFirst.CompareTo("auto") == 0)  //没有批次且执行自动制定批次出库
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, tmp_out_tbl, Cmd, dbname);
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, BodyData.Rows[i]["cposcode"] + "",
                    GetBodyValue_FromData(BodyData, i, "cfree1"), GetBodyValue_FromData(BodyData, i, "cfree2"), GetBodyValue_FromData(BodyData, i, "cfree3"),
                    GetBodyValue_FromData(BodyData, i, "cfree4"), GetBodyValue_FromData(BodyData, i, "cfree5"), GetBodyValue_FromData(BodyData, i, "cfree6"),
                    GetBodyValue_FromData(BodyData, i, "cfree7"), GetBodyValue_FromData(BodyData, i, "cfree8"), GetBodyValue_FromData(BodyData, i, "cfree9"),
                    GetBodyValue_FromData(BodyData, i, "cfree10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            if (bXiaotui && dtMer.Rows.Count == 0)
            {
                DataRow drow = dtMer.NewRow();
                drow["cbatch"] = c_body_batch;
                drow["cposcode"] = BodyData.Rows[i]["cposcode"] + "";
                drow["iquantity"] = "" + decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
                drow["cfree1"] = GetBodyValue_FromData(BodyData, i, "cfree1");
                drow["cfree2"] = GetBodyValue_FromData(BodyData, i, "cfree2");
                drow["cfree3"] = GetBodyValue_FromData(BodyData, i, "cfree3");
                drow["cfree4"] = GetBodyValue_FromData(BodyData, i, "cfree4");
                drow["cfree5"] = GetBodyValue_FromData(BodyData, i, "cfree5");
                drow["cfree6"] = GetBodyValue_FromData(BodyData, i, "cfree6");
                drow["cfree7"] = GetBodyValue_FromData(BodyData, i, "cfree7");
                drow["cfree8"] = GetBodyValue_FromData(BodyData, i, "cfree8");
                drow["cfree9"] = GetBodyValue_FromData(BodyData, i, "cfree9");
                drow["cfree10"] = GetBodyValue_FromData(BodyData, i, "cfree10");
                drow["保质期天数"] = 0;
                drow["保质期单位"] = 0;
                drow["生产日期"] = "";
                drow["失效日期"] = "";
                drow["有效期至"] = "";
                drow["有效期推算方式"] = 0;
                drow["有效期计算项"] = "";
                dtMer.Rows.Add(drow);                
            }
            #endregion

            string b_need_sign = GetBodyValue_FromData(BodyData, i, "bneedsign");
            if (b_need_sign == "") b_need_sign = "0";
            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                KK_U8Com.U8DispatchLists dispdetail = new KK_U8Com.U8DispatchLists(Cmd, dbname);
                int cAutoid = U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='DISPATCH'", Cmd);
                dispdetail.iDLsID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='DISPATCH'";
                Cmd.ExecuteNonQuery();

                dispdetail.DLID = rd_id;
                dispdetail.cWhCode = "'" + w_cwhcode + "'";
                dispdetail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                dispdetail.iQuantity = "" + dtMer.Rows[m]["iquantity"];
                irdrow++;
                dispdetail.iRowNo = irdrow;
                dispdetail.bneedsign = int.Parse(b_need_sign);  //签回
                dispdetail.iSettleQuantity = "0";
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory where cinvcode=" + dispdetail.cInvCode, Cmd);
                dispdetail.cCusInvCode = "'" + U8Operation.GetDataString("select cCusInvCode from " + dbname + "..CusInvContrapose where cInvCode=" + dispdetail.cInvCode + " and cCusCode="+dismain.cCusCode, Cmd) + "'";
                dispdetail.cCusInvName = "'" + U8Operation.GetDataString("select cCusInvName from " + dbname + "..CusInvContrapose where cInvCode=" + dispdetail.cInvCode + " and cCusCode=" + dismain.cCusCode, Cmd) + "'";
                //判断仓库是否计入成本
                int b_costing = U8Operation.GetDataInt("select cast(bincost as int) from " + dbname + "..warehouse where cwhcode=" + dispdetail.cWhCode, Cmd);
                dispdetail.bcosting = b_costing;

                #region //自由项  自定义项处理
                dispdetail.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                dispdetail.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                dispdetail.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                dispdetail.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                dispdetail.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                dispdetail.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                dispdetail.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                dispdetail.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                dispdetail.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                dispdetail.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                dispdetail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
                dispdetail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
                dispdetail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
                dispdetail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
                dispdetail.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
                dispdetail.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                dispdetail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
                dispdetail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
                dispdetail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
                dispdetail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
                dispdetail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
                dispdetail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
                dispdetail.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
                dispdetail.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
                dispdetail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
                dispdetail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                #endregion

                #region //获得单价与金额
                string ctaxmoney = "";
                string ctaxrate = GetBodyValue_FromData(BodyData, i, "itaxrate");
                string i_unitprice = "";
                string i_itaxuintprice = "";
                if (bCreate)
                {
                    DataTable dtDetailRow = U8Operation.GetSqlDataTable("select iTaxUnitPrice,iUnitPrice,round(iTaxUnitPrice*(" + dispdetail.iQuantity + @"),2) imoney,isnull(iTaxRate,0) iTaxRate
                            from " + dbname + "..SA_ReturnsApplyDetail where ID = 0" + isoid + " and AutoID=0" + iSOsID, "dtDetailRow", Cmd);
                    if (dtDetailRow.Rows.Count == 0)
                    {
                        dtDetailRow = U8Operation.GetSqlDataTable("select iTaxUnitPrice,iUnitPrice,round(iTaxUnitPrice*(" + dispdetail.iQuantity + @"),2) imoney,isnull(iTaxRate,0) iTaxRate
                            from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, "dtDetailRow", Cmd);
                        if (dtDetailRow.Rows.Count == 0) throw new Exception("继承订单单价时：未找到订单");
                        ctaxmoney = dtDetailRow.Rows[0]["imoney"] + "";
                        ctaxrate = dtDetailRow.Rows[0]["iTaxRate"] + "";
                        i_unitprice = "" + dtDetailRow.Rows[0]["iUnitPrice"];
                        i_itaxuintprice = "" + dtDetailRow.Rows[0]["iTaxUnitPrice"];
                    }
                    else
                    {
                        ctaxmoney = dtDetailRow.Rows[0]["imoney"] + "";
                        ctaxrate = dtDetailRow.Rows[0]["iTaxRate"] + "";
                        i_unitprice = "" + dtDetailRow.Rows[0]["iUnitPrice"];
                        i_itaxuintprice = "" + dtDetailRow.Rows[0]["iTaxUnitPrice"];
                    }
                }
                else
                {
                    i_unitprice = GetBodyValue_FromData(BodyData, i, "iunitprice");
                    i_itaxuintprice = GetBodyValue_FromData(BodyData, i, "itaxunitprice");
                    if (i_itaxuintprice != "")
                    {
                        ctaxmoney = "" + Math.Round(decimal.Parse(i_itaxuintprice) * decimal.Parse(dispdetail.iQuantity), 2);
                    }
                }
                if (ctaxmoney.CompareTo("") == 0) //自动获得价格表单价
                {
                    ctaxmoney = U8Operation.GetDataString("select top 1 round(iInvNowCost*(" + dispdetail.iQuantity + "),2) from " + dbname + @"..SA_CusUPrice 
                           where cInvCode=" + dispdetail.cInvCode + " and cCusCode=" + dismain.cCusCode + " order by dStartDate desc", Cmd);
                }

                if (ctaxrate == "") ctaxrate = "13";
                dispdetail.iSum = (ctaxmoney.CompareTo("") == 0 ? "null" : ctaxmoney);
                dispdetail.iTaxUnitPrice = (i_itaxuintprice == "" ? "null" : i_itaxuintprice);
                dispdetail.iUnitPrice = (i_unitprice == "" ? "null" : i_unitprice);
                dispdetail.iTaxRate = float.Parse(ctaxrate);
                #endregion

                #region //代管挂账处理
                dispdetail.cvmivencode = "''";
                //dispdetail.bcosting = 1;
                if (b_Vmi)
                {
                    dispdetail.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理
                dispdetail.cBatch = "''";
                dispdetail.iExpiratDateCalcu = "0";
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + dispdetail.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" != "")
                    {
                        dispdetail.cBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                        //保质期管理
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + dispdetail.cInvCode + " and bInvQuality=1", Cmd) > 0)
                        {
                            dispdetail.iExpiratDateCalcu = dtMer.Rows[m]["有效期推算方式"] + "";
                            dispdetail.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                            dispdetail.dvDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                            dispdetail.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                            dispdetail.dMDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                            dispdetail.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                        }

                        //批次档案 建档
                        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + dispdetail.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                        {
                            DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                    cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                                where cInvCode=" + dispdetail.cInvCode + " and cBatch=" + dispdetail.cBatch + " and isnull(cFree1,'')=" + dispdetail.cFree1 + @" 
                                    and isnull(cFree2,'')=" + dispdetail.cFree2 + " and isnull(cFree3,'')=" + dispdetail.cFree3 + " and isnull(cFree4,'')=" + dispdetail.cFree4 + @" 
                                    and isnull(cFree5,'')=" + dispdetail.cFree5 + " and isnull(cFree6,'')=" + dispdetail.cFree6 + " and isnull(cFree7,'')=" + dispdetail.cFree7 + @" 
                                    and isnull(cFree8,'')=" + dispdetail.cFree8 + " and isnull(cFree9,'')=" + dispdetail.cFree9 + " and isnull(cFree10,'')=" + dispdetail.cFree10, "dtBatPerp", Cmd);
                            if (dtBatPerp.Rows.Count > 0)
                            {
                                dispdetail.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                                dispdetail.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                                dispdetail.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                                dispdetail.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                                dispdetail.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                                dispdetail.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                                dispdetail.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                                dispdetail.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                                dispdetail.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                                dispdetail.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                            }
                        }
                    }
                    else
                    {
                        if (b_SaCreateRd32 || b_SaMustInputBatch == true) throw new Exception(dispdetail.cInvCode + "有批次管理，必须输入批号");
                    }
                }
                #endregion

                #region //固定换算率（多计量） 和 回写到生产单
                dispdetail.iNum = "null";
                if (cst_unitcode.CompareTo("") != 0)
                {
                    string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode='" + cst_unitcode + "'", Cmd);
                    if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                    dispdetail.iInvExchRate = ichange;
                    inum = U8Operation.GetDataString("select round(" + dispdetail.iQuantity + "/" + ichange + ",5)", Cmd);
                    dispdetail.iNum = inum;

                    Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHNum=isnull(iFHNum,0)+(0" + inum + ") where iSOsID =0" + iSOsID;
                    Cmd.ExecuteNonQuery();
                }

                //回写到销售订单用量表累计发货数量
                if (bXiaotui || Convert.ToDecimal(dispdetail.iQuantity) < 0)
                {
                    Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHQuantity=isnull(iFHQuantity,0)+(0" + dispdetail.iQuantity + "),fretquantity =isnull(fretquantity ,0)-(0" + dispdetail.iQuantity + ") where iSOsID =0" + iSOsID;
                    Cmd.ExecuteNonQuery();                    
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..SO_SODetails set iFHQuantity=isnull(iFHQuantity,0)+(0" + dispdetail.iQuantity + ") where iSOsID =0" + iSOsID;
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                if (bCreate)
                {
                    DataTable dtDetailRow = U8Operation.GetSqlDataTable("select ID from " + dbname + "..SA_ReturnsApplyDetail where ID = 0" + isoid + " and AutoID=0" + iSOsID, "dtDetailRow", Cmd);
                    if (dtDetailRow.Rows.Count == 0)
                    {
                        dispdetail.cSoCode = "'" + U8Operation.GetDataString("select cSOCode from " + dbname + "..SO_SOMain where id=0" + isoid, Cmd) + "'";
                        dispdetail.cordercode = dispdetail.cSoCode;
                        dispdetail.iorderrowno = U8Operation.GetDataString("select iRowNo from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, Cmd);
                        dispdetail.bgift = U8Operation.GetDataInt("select cast(bgift as int) from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, Cmd);
                        dispdetail.iSOsID = iSOsID;
                        dispdetail.cMemo = "''";
                    }
                    else
                    {
                        dispdetail.cSoCode = "''";
                        dispdetail.cordercode = dispdetail.cSoCode;
                        dispdetail.iorderrowno = U8Operation.GetDataString("select iRowNo from " + dbname + "..SA_ReturnsApplyDetail where AutoID=0" + iSOsID, Cmd);
                        string chkValue = U8Operation.GetDataString("select iSOsID from " + dbname + "..SA_ReturnsApplyDetail where AutoID=0" + iSOsID, Cmd);
                        if (chkValue == "")
                        {
                            dispdetail.iSOsID = "NULL";
                        }
                        else
                        {
                            dispdetail.iSOsID = chkValue;

                        }
                        dispdetail.cMemo = "''";                            
                    }
                    //无来源的销售退货单不用检查销售订单
                    if (iSOsID != BodyData.Rows[i]["isosid"] + "")
                    {
                        if (U8Operation.GetDataString("select isnull(cVerifier,'') from " + dbname + "..SO_SOMain a where id=0" + isoid, Cmd) == "")
                            throw new Exception("存货" + dispdetail.cInvCode + "的销售订单未终审");                        
                    }

                    //继承到货单的表体自由项  自定义项数据
                    dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..SO_SODetails a 
                        where iSOsID=0" + iSOsID, "dtPuArr", Cmd);
                    if (dtPuArr.Rows.Count == 0)
                    {
                        dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                                cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                                cdefine35,cdefine36,cdefine37 from " + dbname + @"..SA_ReturnsApplyDetail a 
                            where AutoID=0" + iSOsID, "dtPuArr", Cmd);
                        if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到销售订单用料信息");

                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                        if (dispdetail.cDefine23.CompareTo("''") == 0) dispdetail.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                        if (dispdetail.cDefine24.CompareTo("''") == 0) dispdetail.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                        if (dispdetail.cDefine25.CompareTo("''") == 0) dispdetail.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                        if (dispdetail.cDefine30.CompareTo("''") == 0) dispdetail.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                        if (dispdetail.cDefine31.CompareTo("''") == 0) dispdetail.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                        if (dispdetail.cDefine32.CompareTo("''") == 0) dispdetail.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                        if (dispdetail.cDefine33.CompareTo("''") == 0) dispdetail.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";
                    }
                    else
                    {
                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                        if (dispdetail.cDefine23.CompareTo("''") == 0) dispdetail.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                        if (dispdetail.cDefine24.CompareTo("''") == 0) dispdetail.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                        if (dispdetail.cDefine25.CompareTo("''") == 0) dispdetail.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                        if (dispdetail.cDefine22.CompareTo("''") == 0) dispdetail.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                        if (dispdetail.cDefine30.CompareTo("''") == 0) dispdetail.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                        if (dispdetail.cDefine31.CompareTo("''") == 0) dispdetail.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                        if (dispdetail.cDefine32.CompareTo("''") == 0) dispdetail.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                        if (dispdetail.cDefine33.CompareTo("''") == 0) dispdetail.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";
                    }

                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos) //销售
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory where cinvcode=" + dispdetail.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("仓库有货位管理，" + dispdetail.cInvCode + "的货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode=" + w_cwhcode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                    dispdetail.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                #endregion

                //保存数据
                if (!dispdetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
                Cmd.CommandText = "update " + dbname + "..DispatchLists set iQuantity=round(iQuantity,5),bIACreateBill=0 where iDLsID=" + dispdetail.iDLsID;
                Cmd.ExecuteNonQuery();
                if (BodyData.Columns.Contains("soid")) //销售退货申请单回写
                {
                    int testID = U8Operation.GetDataInt("select t1.ID from " + dbname + "..SA_ReturnsApplyMain T," + dbname + "..SA_ReturnsApplyDetail T1 WHERE T.ID = T1.ID AND  T1.ID = 0" + BodyData.Rows[i]["soid"] + " AND t1.autoID = 0" + BodyData.Rows[i]["isosid"] + "", Cmd);
                    if (testID > 0)
                    {
                        //Cmd.CommandText = "update " + dbname + "..DispatchLists set irtnappid = " + iSOsID + " where iDLsID=" + dispdetail.iDLsID + "";
                        Cmd.CommandText = "update " + dbname + "..DispatchLists set irtnappid = " + BodyData.Rows[i]["isosid"] + "" + " where iDLsID=" + dispdetail.iDLsID + "";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "update " + dbname + "..SA_ReturnsApplyDetail set fretqty  = " + dispdetail.iQuantity + ",fretsum   = " + dispdetail.iSum + " where autoID = 0" + BodyData.Rows[i]["isosid"] + "";
                        Cmd.ExecuteNonQuery();
                    }
                }


                #region  //写临时表
//                Cmd.CommandText = @"insert into #tmp_out_list(cwhcode,cinvcode,cposcode,cbatch,cvmivencode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,iquantity) 
//                    values(" + dispdetail.cWhCode + "," + dispdetail.cInvCode + ",isnull(" + dispdetail.cPosition + ",''),isnull(" + dispdetail.cBatch + ",''),isnull(" + dispdetail.cvmivencode + @",''),
//                        isnull(" + dispdetail.cFree1 + ",''),isnull(" + dispdetail.cFree2 + ",''),isnull(" + dispdetail.cFree3 + ",''),isnull(" + dispdetail.cFree4 + ",''),isnull(" + dispdetail.cFree5 + @",''),
//                        isnull(" + dispdetail.cFree6 + ",''),isnull(" + dispdetail.cFree7 + ",''),isnull(" + dispdetail.cFree8 + ",''),isnull(" + dispdetail.cFree9 + ",''),isnull(" + dispdetail.cFree10 + @",''),
//                        0" + dispdetail.iQuantity+ ")";
//                Cmd.ExecuteNonQuery();
                #endregion

                if (w_bustype == "分期收款")
                {
                    Cmd.CommandText = "insert into " + dbname + @"..IA_SA_UnAccountVouch(IDUN,IDSUN,cVouTypeUN,cBustypeUN) 
                                    values(" + dispdetail.DLID + "," + dispdetail.iDLsID + "," + dismain.cVouchType + ",'分期收款')";
                    Cmd.ExecuteNonQuery();
                }

                #region//货位账务处理  (调拨单不处理货位业务)
               
                #endregion

                #region //写记账 记录表
                string c_saletype = U8Operation.GetDataString(@"select cvalue from " + dbname + "..AccInformation where cSysID='ia' and cname='bSaleType'", Cmd).ToLower();
                if (w_bustype != "分期收款" && c_saletype != "销售发票")
                {
                    Cmd.CommandText = "insert into " + dbname + @"..IA_SA_UnAccountVouch(IDUN,IDSUN,cVouTypeUN,cBustypeUN) 
                        select a.DLID,a.iDLsID,b.cVouchType,b.cBusType from " + dbname + "..DispatchLists a inner join " + dbname + @"..DispatchList b on a.DLID=b.DLID 
                        where a.iDLsID=" + dispdetail.iDLsID;
                    Cmd.ExecuteNonQuery();
                }
                #endregion 
            }


            #region//是否超销售订单检查
            if (bCreate)
            {
                float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..DispatchLists where iSOsID=0" + iSOsID, Cmd));
                #region //判断是否生产订单入库  0代表不能超   1 代表可超
                if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='sa' and cname='bOverOrder'", Cmd) == 1)
                {
                    //关键领用控制量，取最小值  
                    DataTable dtPuArr = U8Operation.GetSqlDataTable(@"select iquantity from " + dbname + @"..SO_SODetails a 
                        where iSOsID=0" + iSOsID, "dtPuArr", Cmd);
                    if (dtPuArr.Rows.Count > 0)
                    {
                        float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iquantity from " + dbname + "..SO_SODetails where iSOsID=0" + iSOsID, Cmd));
                        if (f_ll_qty < fckqty) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超销售订单发货");
                    }
                }
                #endregion

            }
            #endregion

        }

        #endregion
        #endregion

        #region   //审核发货单
        string cDBVouchAutoCHeck = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cDBVouchAutoCHeck'", Cmd);
        if (cDBVouchAutoCHeck.CompareTo("true") == 0)  //发货单自动审核
        {
            //根据U8 逻辑，1 发货单自动审核U8系统若需要自动出库，则必须出库；2 若审核后U8不需要自动出库，但条码参数需要自动出库，则也出库
            bool b_barcode_autoOut = false;
            if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='barcode_afterdispcheck_createRd32'", Cmd).ToLower().CompareTo("true") == 0)
            { b_barcode_autoOut = true; }

            //发货单审核后自动出库
            if (b_SaCreateRd32 || (!b_SaCreateRd32 && b_barcode_autoOut))
            {
                //组合销售出库单
                DataTable dtRdMain = U8Operation.GetSqlDataTable(@"select '" + w_cwhcode + @"'cwhcode,cdepcode,cstcode,cpersoncode,
	                    ccuscode,cbustype,cdlcode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                    cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cmemo
                        " + (c_vou_checker == "" ? "" : ",'" + c_vou_checker + "' checker") + @"
                        " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                    from " + dbname + "..DispatchList where DLID=0" + dismain.DLID, "HeadData", Cmd);
                DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cbatch,b.iquantity,b.cvmivencode cbvencode,
	                    b.idlsid,b.cposition cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                    cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                    cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.inum
                    from " + dbname + "..DispatchLists b inner join " + dbname + "..inventory i on b.cInvCode=i.cInvCode where b.DLID=0" + dismain.DLID, "BodyData", Cmd);
                if (dtRdMain.Rows.Count == 0) throw new Exception("无法找到发货单");
                if (dtRddetail.Rows.Count == 0) throw new Exception("无法找到发货单内容数据");
                DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                U81021(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81021", Cmd);
            }
            //审核发货单状态  (发货单审核后自动出库)
            Cmd.CommandText = "update " + dbname + "..DispatchList set cVerifier='" + (c_vou_checker == "" ? cUserName : c_vou_checker) + "',dverifydate='" + cLogDate + "',dverifysystime=getdate() where dlid=" + rd_id;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        return rd_id + "," + cc_mcode;

    }

    //销售出库单 [发货单出库  直接销售出库]
    public string U81021_1(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID,bool b_auto_out, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_cuscode = "";
        string w_rdcode = ""; string w_stcode = ""; //出库类型
        string w_bustype = "";//业务类型
        bool b_Vmi = false; bool b_Pos = false;//货位
        string idlid = ""; string idlsid = ""; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        //严格控制先进先出规则
        string c_firstTofisrt_Ctl = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_out_cfirstToFirst_control'", Cmd);
        c_firstTofisrt_Ctl = c_firstTofisrt_Ctl.ToLower();
        //先进先出 例外仓库
        string c_firstTofisrt_NotCtl_WareList = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_out_cfirstToFirst_Not_control_ware'", Cmd);
        c_firstTofisrt_NotCtl_WareList = "," + c_firstTofisrt_NotCtl_WareList + ",";

        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) w_bustype = "普通销售";
        //销售开票规则
        bool b_Rd32_KP = false;//销售出库开票，默认为发false
        #region
        if (w_bustype=="普通销售" && U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation where cSysID='sa' and cname='bsaleoutcreatebillptxs'", Cmd) == "1")
        {
            //普通销售  ：销售出库开票
            b_Rd32_KP = true;
        }
        if (w_bustype == "分期收款" && U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation where cSysID='sa' and cname='bsaleoutcreatebillfqsk'", Cmd) == "1")
        {
            //分期收款  ：销售出库开票
            b_Rd32_KP = true;
        }
        #endregion

        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='0303' and VT_TemplateMode=0", Cmd);

        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        #region  //逻辑检验
        if (BodyData.Columns.Contains("idlsid"))
        {
            idlsid = BodyData.Rows[0]["idlsid"] + "";
            if (idlsid.CompareTo("") != 0)
            {
                bCreate = true;
                idlid = U8Operation.GetDataString("select dlid from " + dbname + "..DispatchLists where idlsid=0" + idlsid, Cmd);
                if (idlid == "") throw new Exception("没有找到发货单信息");
            }
        }

        //销售类别
        w_stcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cstcode");
        if (w_stcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..SaleType where cSTCode='" + w_stcode + "'", Cmd) == 0)
                throw new Exception("销售类型输入错误");
        }
        else
        {
            w_cuscode = U8Operation.GetDataString("select cstcode from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd);
        }

        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") == 0)
        {
            w_rdcode = U8Operation.GetDataString("select cRdCode from " + dbname + "..SaleType where cSTCode='" + w_stcode + "'", Cmd);
        }
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //客户
        w_cuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuscode");
        if (w_cuscode.CompareTo("") == 0)
        {
            w_cuscode = U8Operation.GetDataString("select ccuscode from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd);
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..customer where ccuscode='" + w_cuscode + "'", Cmd) == 0)
            throw new Exception("客户输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }
        else
        {
            w_cdepcode = U8Operation.GetDataString("select cdepcode from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd);
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        else
        {
            w_cpersoncode = U8Operation.GetDataString("select cpersoncode from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd);
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        #region //单据
        KK_U8Com.U8Rdrecord32 record32 = new KK_U8Com.U8Rdrecord32(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
        Cmd.ExecuteNonQuery();
        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "C" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..rdrecord32 where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        record32.cCode = "'" + cc_mcode + "'";
        record32.ID = rd_id;
        record32.cVouchType = "'32'";
        record32.bredvouch = 0; //红篮子  
        record32.cBusType = "'" + w_bustype + "'";
        record32.cWhCode = "'" + w_cwhcode + "'";
        record32.cSTCode = (w_stcode.CompareTo("") == 0 ? "null" : "'" + w_stcode + "'");
        record32.cRdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        record32.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        record32.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        record32.cCusCode = "'" + w_cuscode + "'";
        record32.cBusCode = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cbuscode") + "'";  //业务号
        record32.VT_ID = vt_id;

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        record32.cMaker = "'" + cUserName + "'";
        record32.dDate = "'" + cLogDate + "'";
        record32.cHandler = "''";
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..T_Parameter where cPid='u8barcode_rd32_is_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            if (c_vou_checker == "")
            {
                record32.cHandler = "'" + cUserName + "'";
            }
            else
            {
                record32.cHandler = "'" + c_vou_checker + "'";
            }
            
            record32.dVeriDate = "'" + cLogDate + "'";
        }
        record32.cMemo = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cmemo") + "'";
        if (bCreate)
        {
            record32.cSource = "'发货单'";
            record32.cDLCode = idlid + "";
            record32.iExchRate = float.Parse(U8Operation.GetDataString("select iExchRate from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd));
            record32.cExch_Name = "'" + U8Operation.GetDataString("select cexch_name from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd) + "'";
        }
        else
        {
            record32.cSource = "'库存'";
            record32.iExchRate = 1;
            record32.cExch_Name = "'人民币'";
        }

        #region   //主表自定义项处理
        record32.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        record32.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        record32.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        record32.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        record32.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        record32.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        record32.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        record32.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        record32.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        record32.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        record32.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        record32.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        record32.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        record32.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        record32.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        record32.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        if (!record32.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }
        //开票单位  cinvoicecompany
        if (b_Rd32_KP)
        {
            string ckp_code = U8Operation.GetDataString("select cinvoicecompany from " + dbname + "..customer where ccuscode=" + record32.cCusCode, Cmd);
            if (ckp_code == "")
            {
                Cmd.CommandText = "update " + dbname + "..rdrecord32 set cinvoicecompany='" + ckp_code + "' where id=" + record32.ID;
                Cmd.ExecuteNonQuery();
            }
            else
            {
                Cmd.CommandText = "update " + dbname + "..rdrecord32 set cinvoicecompany=" + record32.cCusCode + " where id=" + record32.ID;
                Cmd.ExecuteNonQuery();
            }
        }
        //判断是否发货自动出库  
        if (U8Operation.GetDataString("SELECT cValue FROM " + dbname + "..AccInformation where cSysID='ST' and cName='bSAcreat'", Cmd).ToLower().CompareTo("true") == 0)
        {
            Cmd.CommandText = "Update " + dbname + "..DispatchList SET cSaleOut=" + record32.cCode + " WHERE DLID=0" + idlid;
            Cmd.ExecuteNonQuery();
        }
        else
        {
            Cmd.CommandText = "Update " + dbname + "..DispatchList SET cSaleOut='ST' WHERE DLID=0" + idlid;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        #region //子表
        //BodyData 数据按照存货编码 批号排序
        if (c_firstTofisrt_Ctl.CompareTo("true") == 0)   //管控先进先出法前先排序
        {
            BodyData.DefaultView.Sort = "cinvcode,cbatch";
            BodyData = BodyData.DefaultView.ToTable();
        }
        int irdrow = 0;
        //判断仓库是否计入成本
        int b_costing = U8Operation.GetDataInt("select cast(bincost as int) from " + dbname + "..warehouse where cwhcode=" + record32.cWhCode, Cmd);

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("出入库数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            idlsid = BodyData.Rows[i]["idlsid"] + "";
            //throw new Exception("传入的上游业务行ID是[" + idlsid + "]");
            idlid = U8Operation.GetDataString("select dlid from " + dbname + "..DispatchLists where idlsid=0" + idlsid, Cmd);
            DataTable disM = U8Operation.GetSqlDataTable("select cbustype from " + dbname + "..DispatchList where dlid=0" + idlid, "disM", Cmd);
            if (disM.Rows.Count > 0)
            {
                if (disM.Rows[0]["cbustype"] + "" != w_bustype) 
                    throw new Exception("传入的业务类型[" + w_bustype + "]与实际上游单据的业务类型[" + disM.Rows[0]["cbustype"] + "]不一致，产品[" + BodyData.Rows[i]["cinvcode"] + "]");
            }
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                string chkValue = U8Operation.GetDataString("select ccuscode from " + dbname + "..DispatchList a where dlid=0" + idlid, Cmd);
                if (chkValue.CompareTo(w_cuscode) != 0) throw new Exception("多发货单生成销售出库，必须客户相同");
                chkValue = U8Operation.GetDataString("select cwhcode from " + dbname + "..DispatchLists a where idlsid=0" + idlsid, Cmd);
                if (chkValue.CompareTo(w_cwhcode) != 0) throw new Exception("同一张出库单仓库必须相同");
            }

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            
            #region //出库批次清单
            System.Data.DataTable dtMer = null;
            //增加U8是否批次管理检查。---2022-10-14
            if (c_body_batch.CompareTo("") != 0 && U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + "" + BodyData.Rows[i]["cinvcode"] + "' and bInvBatch=1", Cmd) > 0)
            {
                if (c_body_batch.CompareTo("") == 0 && (b_auto_out || cfirstToFirst.CompareTo("auto") == 0))  //没有批次且执行自动制定批次出库
                {
                    dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
                }
                else
                {
                    dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, c_body_batch, BodyData.Rows[i]["cposcode"] + "",
                        GetBodyValue_FromData(BodyData, i, "cfree1"), GetBodyValue_FromData(BodyData, i, "cfree2"), GetBodyValue_FromData(BodyData, i, "cfree3"),
                        GetBodyValue_FromData(BodyData, i, "cfree4"), GetBodyValue_FromData(BodyData, i, "cfree5"), GetBodyValue_FromData(BodyData, i, "cfree6"),
                        GetBodyValue_FromData(BodyData, i, "cfree7"), GetBodyValue_FromData(BodyData, i, "cfree8"), GetBodyValue_FromData(BodyData, i, "cfree9"),
                        GetBodyValue_FromData(BodyData, i, "cfree10"), decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
                }
            }
            else
            {
                dtMer = GetBatDTFromWare(w_cwhcode, "" + BodyData.Rows[i]["cinvcode"], cvmivencode, decimal.Parse("" + BodyData.Rows[i]["iquantity"]), b_Pos, Cmd, dbname);
            }
            #endregion

            for (int m = 0; m < dtMer.Rows.Count; m++) //cbatch,cPosCode cposcode,iquantity,cfree1，保质期天数,保质期单位,生产日期,失效日期,有效期至,有效期推算方式,有效期计算项
            {
                KK_U8Com.U8Rdrecords32 records32 = new KK_U8Com.U8Rdrecords32(Cmd, dbname);
                int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='rd'", Cmd);
                records32.AutoID = cAutoid;
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='rd'";
                Cmd.ExecuteNonQuery();
                
                records32.ID = rd_id;
                records32.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
                records32.iQuantity = "" + dtMer.Rows[m]["iquantity"];
                records32.ccusinvcode = "'" + U8Operation.GetDataString("select cCusInvCode from " + dbname + "..CusInvContrapose where cInvCode=" + records32.cInvCode + " and cCusCode=" + record32.cCusCode, Cmd) + "'";
                records32.ccusinvname = "'" + U8Operation.GetDataString("select cCusInvName from " + dbname + "..CusInvContrapose where cInvCode=" + records32.cInvCode + " and cCusCode=" + record32.cCusCode, Cmd) + "'";
                irdrow++;
                records32.irowno = irdrow;
                cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory where cinvcode=" + records32.cInvCode + "", Cmd);

                #region //自由项  自定义项处理
                records32.cFree1 = "'" + dtMer.Rows[m]["cfree1"] + "'";
                records32.cFree2 = "'" + dtMer.Rows[m]["cfree2"] + "'";
                records32.cFree3 = "'" + dtMer.Rows[m]["cfree3"] + "'";
                records32.cFree4 = "'" + dtMer.Rows[m]["cfree4"] + "'";
                records32.cFree5 = "'" + dtMer.Rows[m]["cfree5"] + "'";
                records32.cFree6 = "'" + dtMer.Rows[m]["cfree6"] + "'";
                records32.cFree7 = "'" + dtMer.Rows[m]["cfree7"] + "'";
                records32.cFree8 = "'" + dtMer.Rows[m]["cfree8"] + "'";
                records32.cFree9 = "'" + dtMer.Rows[m]["cfree9"] + "'";
                records32.cFree10 = "'" + dtMer.Rows[m]["cfree10"] + "'";

                //自定义项
                records32.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
                records32.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
                records32.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
                records32.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
                records32.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
                records32.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
                records32.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
                records32.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
                records32.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
                records32.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
                records32.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
                records32.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
                records32.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
                records32.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
                records32.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
                records32.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                #endregion

                #region //获得单价与金额
                string c_ppcost = U8Operation.GetDataString("select cast(iInvRCost as decimal(18,8)) from " + dbname + "..inventory(nolock) where cinvcode=" + records32.cInvCode, Cmd);
                #endregion

                #region //是否存货核算处理
                records32.bCosting = b_costing;  //销售出库为1 销售发票 和 发出商品  为0
                //if (b_Rd32_KP)
                //{
                //    //按照销售发票 核算成本
                //    records32.bCosting = b_costing; //是否计入成本 (受仓库 是否计入成本控制)
                //}
                #endregion

                #region //代管挂账处理
                records32.cvmivencode = "''";
                records32.iNquantity = records32.iQuantity;
                if (b_Vmi)
                {
                    records32.bVMIUsed = "1";
                    records32.cvmivencode = "'" + cvmivencode + "'";
                }
                #endregion

                #region //批次管理和保质期管理
                records32.cBatch = "''";
                records32.iExpiratDateCalcu = 0;
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records32.cInvCode + " and bInvBatch=1", Cmd) > 0)
                {
                    if (dtMer.Rows[m]["cbatch"] + "" == "") throw new Exception(records32.cInvCode + "有批次管理，必须输入批号");
                    records32.cBatch = "'" + dtMer.Rows[m]["cbatch"] + "'";
                    //保质期管理
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + records32.cInvCode + " and bInvQuality=1", Cmd) > 0)
                    {
                        records32.iExpiratDateCalcu = int.Parse(dtMer.Rows[m]["有效期推算方式"] + "");
                        records32.iMassDate = dtMer.Rows[m]["保质期天数"] + "";
                        records32.dVDate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records32.cExpirationdate = "'" + dtMer.Rows[m]["失效日期"] + "'";
                        records32.dMadeDate = "'" + dtMer.Rows[m]["生产日期"] + "'";
                        records32.cMassUnit = "'" + dtMer.Rows[m]["保质期单位"] + "'";
                    }

                    //批次档案 建档
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + records32.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                    {
                        DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                            where cInvCode=" + records32.cInvCode + " and cBatch=" + records32.cBatch + " and isnull(cFree1,'')=" + records32.cFree1 + @" 
                                and isnull(cFree2,'')=" + records32.cFree2 + " and isnull(cFree3,'')=" + records32.cFree3 + " and isnull(cFree4,'')=" + records32.cFree4 + @" 
                                and isnull(cFree5,'')=" + records32.cFree5 + " and isnull(cFree6,'')=" + records32.cFree6 + " and isnull(cFree7,'')=" + records32.cFree7 + @" 
                                and isnull(cFree8,'')=" + records32.cFree8 + " and isnull(cFree9,'')=" + records32.cFree9 + " and isnull(cFree10,'')=" + records32.cFree10, "dtBatPerp", Cmd);
                        if (dtBatPerp.Rows.Count > 0)
                        {
                            records32.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                            records32.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                            records32.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                            records32.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                            records32.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                            records32.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                            records32.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                            records32.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                            records32.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                            records32.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                        }
                    }
                }
                #endregion

                #region //固定换算率（多计量）
                records32.iNum = "null";
                if (cst_unitcode.CompareTo("") != 0)
                {
                    records32.cAssUnit = "'" + cst_unitcode + "'";
                    string ichange = "";
                    if (BodyData.Columns.Contains("inum")) inum = "" + BodyData.Rows[i]["inum"];
                    if (inum == "") inum = "0";
                    if (decimal.Parse(inum) == 0)
                    {
                        ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + records32.cAssUnit, Cmd);
                        if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                        inum = U8Operation.GetDataString("select round(" + records32.iQuantity + "/" + ichange + ",5)", Cmd);
                    }
                    else
                    {
                        ichange = "" + (decimal.Parse(records32.iQuantity) / decimal.Parse(inum));
                    }
                    records32.iinvexchrate = ichange;
                    records32.iNum = inum;

                    Cmd.CommandText = "update " + dbname + "..DispatchLists set fOutNum=isnull(fOutNum,0)+(0" + inum + ") where iDLsID =0" + idlsid;
                    Cmd.ExecuteNonQuery();
                }

                //回写到销售订单用量表累计发货数量
                Cmd.CommandText = "update " + dbname + "..DispatchLists set fOutQuantity=isnull(fOutQuantity,0)+(0" + records32.iQuantity + ") where iDLsID =0" + idlsid;
                Cmd.ExecuteNonQuery();
                #endregion

                #region//上游单据关联
                DataTable dtPuArr = null;  //上游单据表体自定义项继承
                records32.iordertype = "0";
                if (bCreate)
                {
                    records32.idlsid = idlsid;
                    records32.iorderdid = "" + U8Operation.GetDataString("select isnull(max(iSOsID),0) from " + dbname + "..DispatchLists where iDLsID=0" + idlsid, Cmd);
                    records32.cbdlcode = "'" + U8Operation.GetDataString("select cdlcode from " + dbname + "..DispatchList where dlid=0" + idlid, Cmd) + "'";
                    records32.iordertype = "1";

                    //继承到货单的表体自由项  自定义项数据
                    dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37 from " + dbname + @"..DispatchLists a 
                        where idlsid=0" + idlsid, "dtPuArr", Cmd);
                    if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到发货单用料信息");

                    if (records32.cDefine22.CompareTo("''") == 0) records32.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                    if (records32.cDefine23.CompareTo("''") == 0) records32.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                    if (records32.cDefine24.CompareTo("''") == 0) records32.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                    if (records32.cDefine25.CompareTo("''") == 0) records32.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                    if (records32.cDefine22.CompareTo("''") == 0) records32.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                    if (records32.cDefine22.CompareTo("''") == 0) records32.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                    if (records32.cDefine30.CompareTo("''") == 0) records32.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                    if (records32.cDefine31.CompareTo("''") == 0) records32.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                    if (records32.cDefine32.CompareTo("''") == 0) records32.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                    if (records32.cDefine33.CompareTo("''") == 0) records32.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

                    Cmd.CommandText = "update " + dbname + "..SO_SODetails set foutquantity=isnull(foutquantity,0)+(0" + records32.iQuantity + @"), 
                        foutnum=isnull(foutnum,0)+(0" + records32.iNum.Replace("null", "") + @")  where iSOsID =0" + records32.iorderdid;
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                #region//货位标识处理  ，若无货位取存货档案的默认货位
                if (b_Pos)
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = "" + dtMer.Rows[m]["cposcode"];
                    if (BodyData.Rows[i]["cposcode"] + "" == "")
                    {
                        BodyData.Rows[i]["cposcode"] = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory where cinvcode=" + records32.cInvCode, Cmd);
                        if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("本仓库有货位管理，" + records32.cInvCode + "的货位不能为空");
                    }
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode=" + record32.cWhCode + " and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于本仓库");
                    records32.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                #endregion

                #region  //严格管控先进先出法
                if (c_firstTofisrt_Ctl.CompareTo("true") == 0 && records32.cBatch.CompareTo("''") != 0 && c_firstTofisrt_NotCtl_WareList.IndexOf("," + w_cwhcode + ",") < 0)
                {
                    //查询是否存在比当前批次更小的批次库存
                    string cLowBatch = "" + U8Operation.GetDataString("select top 1 cBatch from " + dbname + @"..CurrentStock(nolock)
                        where cWhCode=" + record32.cWhCode + " and cInvCode=" + records32.cInvCode + " and cBatch<" + records32.cBatch + " and iQuantity>0 order by cBatch", Cmd);
                    if (cLowBatch != "")
                        throw new Exception("存货档案[" + BodyData.Rows[i]["cinvcode"] + "] 存在更小批次[" + cLowBatch + "],需先出库");
                }
                #endregion

                //保存数据
                if (!records32.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

                #region // 补充子表信息 是否需要开票


                if (c_ppcost != "")
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords32 set cbMemo='" + GetBodyValue_FromData(BodyData, i, "cbmemo") + @"',
                            iPUnitCost =" + c_ppcost + ",iPPrice = " + (decimal.Parse(c_ppcost) * decimal.Parse("" + dtMer.Rows[m]["iquantity"])) + @" 
                        where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords32 set cbMemo='" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "' where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }

                if (fU8Version >= 11)
                {
                    Cmd.CommandText = "update " + dbname + "..rdrecords32 set bneedbill=1,bsaleoutcreatebill=" + (b_Rd32_KP ? "1" : "0") + " where autoid =0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                #endregion

                #region//货位账务处理
                if (b_Pos)
                {
                    //添加货位记录 
                    Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                        "Values (" + cAutoid + "," + rd_id + "," + record32.cWhCode + "," + records32.cPosition + "," + records32.cInvCode + "," + records32.iQuantity + "," + records32.iNum +
                        ",null," + record32.dDate + ",0,'',0," + record32.cVouchType + "," + record32.dDate + "," + record32.cMaker + "," + records32.cvmivencode + "," + records32.cBatch +
                        "," + records32.cFree1 + "," + records32.cFree2 + "," + records32.cFree3 + "," + records32.cFree4 + "," + records32.cFree5 + "," +
                        records32.cFree6 + "," + records32.cFree7 + "," + records32.cFree8 + "," + records32.cFree9 + "," + records32.cFree10 + ",''," + records32.cAssUnit + @",
                        " + records32.dMadeDate + "," + records32.iMassDate + "," + records32.cMassUnit + "," + records32.iExpiratDateCalcu + "," + records32.cExpirationdate + "," + records32.dExpirationdate + ")";
                    Cmd.ExecuteNonQuery();

                    //指定货位
                    if (fU8Version >= 11)
                    {
                        Cmd.CommandText = "update " + dbname + "..rdrecords32 set iposflag=1 where autoid =0" + cAutoid;
                        Cmd.ExecuteNonQuery();
                    }
                    //修改货位库存
                    if (U8Operation.GetDataInt("select count(1) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + record32.cWhCode + " and cvmivencode=" + records32.cvmivencode + " and cinvcode=" + records32.cInvCode + @" 
                    and cPosCode=" + records32.cPosition + " and cbatch=" + records32.cBatch + " and cfree1=" + records32.cFree1 + " and cfree2=" + records32.cFree2 + " and cfree3=" + records32.cFree3 + @" 
                    and cfree4=" + records32.cFree4 + " and cfree5=" + records32.cFree5 + " and cfree6=" + records32.cFree6 + " and cfree7=" + records32.cFree7 + @" 
                    and cfree8=" + records32.cFree8 + " and cfree9=" + records32.cFree9 + " and cfree10=" + records32.cFree10, Cmd) == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                        values(" + record32.cWhCode + "," + records32.cPosition + "," + records32.cInvCode + ",0," + records32.cBatch + @",
                            " + records32.cFree1 + "," + records32.cFree2 + "," + records32.cFree3 + "," + records32.cFree4 + "," + records32.cFree5 + "," +
                                records32.cFree6 + "," + records32.cFree7 + "," + records32.cFree8 + "," + records32.cFree9 + "," + records32.cFree10 + "," + records32.cvmivencode + @",'',0,
                            " + records32.dMadeDate + "," + records32.iMassDate + "," + records32.cMassUnit + "," + records32.iExpiratDateCalcu + "," + records32.cExpirationdate + "," + records32.dExpirationdate + ")";
                        Cmd.ExecuteNonQuery();
                    }
                    Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + records32.iQuantity + "),inum=isnull(inum,0)-(" + records32.iNum + @") 
                        where cwhcode=" + record32.cWhCode + " and cvmivencode=" + records32.cvmivencode + " and cinvcode=" + records32.cInvCode + @" 
                        and cPosCode=" + records32.cPosition + " and cbatch=" + records32.cBatch + " and cfree1=" + records32.cFree1 + " and cfree2=" + records32.cFree2 + " and cfree3=" + records32.cFree3 + @" 
                        and cfree4=" + records32.cFree4 + " and cfree5=" + records32.cFree5 + " and cfree6=" + records32.cFree6 + " and cfree7=" + records32.cFree7 + @" 
                        and cfree8=" + records32.cFree8 + " and cfree9=" + records32.cFree9 + " and cfree10=" + records32.cFree10;
                    Cmd.ExecuteNonQuery();

                    //货位现存量
                    Current_Pos_StockCHeck(Cmd, dbname, record32.cWhCode, records32.cPosition, records32.cInvCode, records32.cBatch, records32.cvmivencode, records32.cFree1, records32.cFree2,
                        records32.cFree3, records32.cFree4, records32.cFree5, records32.cFree6, records32.cFree7, records32.cFree8, records32.cFree9, records32.cFree10);
                }
                #endregion

                #region  //是否超现存量出库
                Current_ST_StockCHeck(Cmd, dbname, record32.cWhCode, records32.cInvCode, records32.cBatch, records32.cvmivencode, records32.cFree1, records32.cFree2,
                    records32.cFree3, records32.cFree4, records32.cFree5, records32.cFree6, records32.cFree7, records32.cFree8, records32.cFree9, records32.cFree10);
                //Current_ST_StockAvableCHeck(Cmd, dbname, record32.cWhCode, records32.cInvCode, records32.cBatch, records32.cvmivencode, records32.cFree1, records32.cFree2,
                //    records32.cFree3, records32.cFree4, records32.cFree5, records32.cFree6, records32.cFree7, records32.cFree8, records32.cFree9, records32.cFree10);
                #endregion

            }


            #region//是否超发货单检查
            if (bCreate)
            {
                float fckqty = float.Parse(U8Operation.GetDataString(@"select isnull(sum(iquantity),0) from " + dbname + "..rdrecords32 where idlsid=0" + idlsid, Cmd));
                #region //判断是否生产订单入库  0代表不能超   1 代表可超
                if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + dbname + "..AccInformation where cSysID='sa' and cname='bOverDispOut'", Cmd) == 0)
                {
                    //关键领用控制量，取最小值  
                    float f_ll_qty = float.Parse(U8Operation.GetDataString(@"select iQuantity from " + dbname + "..DispatchLists where idlsid=0" + idlsid, Cmd));
                    if ((f_ll_qty >= 0 && f_ll_qty < fckqty) || (f_ll_qty < 0 && f_ll_qty > fckqty)) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】超出发货单数量");
                }
                #endregion

            }
            #endregion

        }

        #endregion
        #endregion

        return rd_id + "," + cc_mcode;
    }
    public string U81021(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        return U81021_1(HeadData, BodyData, dbname, cUserName, cLogDate, SheetID,false, Cmd);
    }

    //采购到货单（根据采购订单到货，委外和普通采购，ASN单，含是否自动入库）
    private string U81027(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; string w_ptcode = "";//采购类型
        string w_bustype = "";//业务类型
        string w_souretype = "";//来源单据类型
        bool b_Vmi = false; bool b_Pos = false;
        string s_autoid = ""; string s_id = ""; string posid = ""; string poid = ""; string ordertype = ""; bool bCreate = false;//到货生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        string asn_head_id = "";
        U8Operation.GetDataString("select 'U81027 Start'", Cmd);
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        //asn回写类型  proc:代表 存储过程
        string c_asn_back_type = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_asn_bak_type'", Cmd);
        int i_red_sheet = 0;  //红蓝子
        #region  //逻辑检验
        //额外控制  ********
        DataTable dtValid = U8Operation.GetSqlDataTable("SELECT * FROM " + dbname + "..T_Parameter where cPid='ven_puarr_receivetype'", "dtValid", Cmd);
        //********

        w_souretype = GetTextsFrom_FormData_Tag(HeadData, "txt_csourcetype");
        if (w_souretype.CompareTo("") == 0) throw new Exception("必须输入来源单据类型");
        if (BodyData.Rows.Count == 0) throw new Exception("到货单必须有表体数据");
        if (float.Parse("" + BodyData.Rows[0]["iquantity"]) < 0) i_red_sheet = 1;//第一行数据的 数量判断红篮子
        
        if (BodyData.Columns.Contains("source_autoid"))
        {
            s_autoid = BodyData.Rows[0]["source_autoid"] + "";
            if (s_autoid.CompareTo("") != 0)
            {
                bCreate = true;
                if (w_souretype.CompareTo("ASN单") == 0)
                {
                    string casnvoutype = "";
                    if (c_asn_back_type == "proc")
                    {
                        posid = U8Operation.GetDataString("select cpoid from " + dbname + ".dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                        s_id = U8Operation.GetDataString("select HID from " + dbname + ".dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                        //ASN单据类型：02 采购订单/01 委外订单
                        casnvoutype = U8Operation.GetDataString("select right('00'+potype,2) from " + dbname + ".dbo.ASN_C where ID='" + s_autoid + "'", Cmd);
                        asn_head_id = s_id;
                    }
                    else
                    {
                        posid = U8Operation.GetDataString("select cast(substring(cpoid,8,20) as int) from VenDB.dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                        s_id = U8Operation.GetDataString("select HID from VenDB.dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                        casnvoutype = U8Operation.GetDataString("select left(cpoid,2) from VenDB.dbo.ASN_C where ID='" + s_autoid + "'", Cmd);
                    }

                    if (casnvoutype.CompareTo("02") == 0)
                    {
                        ordertype = "采购订单";
                        poid = U8Operation.GetDataString("select POID from " + dbname + "..PO_Podetails where ID=0" + posid, Cmd);
                    }
                    else
                    {
                        ordertype = "委外订单";
                        poid = U8Operation.GetDataString("select MOID from " + dbname + "..OM_MODetails where MODetailsID=0" + posid, Cmd);
                    }
                    
                }
                else if (w_souretype.CompareTo("采购订单") == 0)
                {
                    ordertype = "采购订单";
                    s_id = U8Operation.GetDataString("select POID from " + dbname + "..PO_Podetails where ID=0" + s_autoid, Cmd);
                    posid = s_autoid;
                    poid = s_id;
                }
                else if (w_souretype.CompareTo("委外订单") == 0)
                {
                    ordertype = "委外订单";
                    s_id = U8Operation.GetDataString("select MOID from " + dbname + "..OM_MODetails where MODetailsID=0" + s_autoid, Cmd);
                    posid = s_autoid;
                    poid = s_id;
                }
                if (s_id == "") throw new Exception("没有找到上游单据信息");
            }
        }
        //采购类型
        w_ptcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cptcode");
        if (w_ptcode.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    w_ptcode = U8Operation.GetDataString("select cPTCode from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    w_ptcode = U8Operation.GetDataString("select cPTCode from " + dbname + "..OM_MOMain where MOID=0" + poid, Cmd);
                }
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PurchaseType where cPTCode='" + w_ptcode + "'", Cmd) == 0)
                throw new Exception("采购类型输入错误");
        }

        //入库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") == 0)
        {
            if (w_ptcode.CompareTo("") != 0)
            {
                w_rdcode = U8Operation.GetDataString("select cRdCode from " + dbname + "..PurchaseType where cPTCode='" + w_ptcode + "'", Cmd);
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "'", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    w_cdepcode = U8Operation.GetDataString("select cDepCode from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    w_cdepcode = U8Operation.GetDataString("select cDepCode from " + dbname + "..OM_MOMain where MOID=0" + poid, Cmd);
                }
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    w_cpersoncode = U8Operation.GetDataString("select cPersonCode from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    w_cpersoncode = U8Operation.GetDataString("select cPersonCode from " + dbname + "..OM_MOMain where MOID=0" + poid, Cmd);
                }
            }
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //供应商
        w_vencode = GetTextsFrom_FormData_Tag(HeadData, "txt_cvencode");
        if (w_vencode.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    w_vencode = U8Operation.GetDataString("select cVenCode from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    w_vencode = U8Operation.GetDataString("select cVenCode from " + dbname + "..OM_MOMain where MOID=0" + poid, Cmd);
                }
            }
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..vendor where cvencode='" + w_vencode + "'", Cmd) == 0)
            throw new Exception("供应商输入错误");

        //业务类型
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0)
        {
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    w_bustype = U8Operation.GetDataString("select cBusType from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    w_bustype = U8Operation.GetDataString("select cBusType from " + dbname + "..OM_MOMain where MOID=0" + poid, Cmd);
                }
            }
            else
            {
                throw new Exception("业务类型必须录入");
            }
        }
        //if (U8Operation.GetDataInt("select count(*) from " + dbname + "..T_CC_Base_Enum where t_team_id='pu_bustype' and t_item_code='" + w_bustype + "'", Cmd) == 0)
        //    throw new Exception("业务类型输入错误");

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        ////收料控制
        ////CheckVouchValid(dbname, dtValid, "R" + posid, Cmd);

        #endregion

        string cc_mcode = "";
        int pu_id = 0;

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select cValue from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));

        int i_has_queshi_row = 0;
        int i_has_valid_row = 0;
        bool bcan_barcode = CanWriteBarCode(dbname, Cmd);
        KK_U8Com.U8PU_ArrivalVouch pumain = new KK_U8Com.U8PU_ArrivalVouch(Cmd, dbname);
        #region  //到货单新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        pu_id = 1000000000 + int.Parse(U8Operation.GetDataString("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
        Cmd.ExecuteNonQuery();

        string trans_code = GetTextsFrom_FormData_Text(HeadData, "txt_mes_code");
        if (trans_code == "")
        {
            string cCodeHead = "D" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..PU_ArrivalVouch where ccode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            cc_mcode = trans_code;
        }
        pumain.ID = pu_id + "";
        pumain.cCode = "'" + cc_mcode + "'";
        pumain.iVTid = "8169";
        pumain.cMaker = "'" + cUserName + "'";
        pumain.dDate = "'" + cLogDate + "'";
        bool b_checked = true;
        bool b_qc_inv_not_checked = false;  //最高等级，默认false
        int i_qc_inv_count = 0; //质检物资数量
        //判断参数，质检物资的到货单要不要自动审核 
        if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_puarr_qcinv_is_not_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            b_qc_inv_not_checked = true;  //质检物资不审核，最高等级
        }

        if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_puarr_is_not_check'", Cmd).ToLower().CompareTo("true") == 0)
        {
            //不审核，需要审批流
            //pumain.iverifystateex = "-1";
            pumain.IsWfControlled = "1";
            b_checked = false;
        }
        else
        {
            if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_close'", Cmd).ToLower().CompareTo("true") == 0)
            {
                //到货单拒收，不走拒收单
                pumain.ccloser = "'" + cUserName + "'";
            }
            else
            {
                //审核 不走审批流
                if (c_vou_checker == "")
                {
                    pumain.cverifier = "'" + cUserName + "'";
                }
                else
                {
                    pumain.cverifier = "'" + c_vou_checker + "'";
                }
            }
            
            pumain.cAuditDate = "'" + cLogDate + "'";
            pumain.iverifystateex = "-1";
            pumain.IsWfControlled = "0";
        }
        pumain.cMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cmemo") + "'";
        pumain.cVenCode = "'" + w_vencode + "'";
        pumain.iBillType = i_red_sheet + "" ;
        pumain.bNegative = i_red_sheet + "";
        //表头项目
        System.Data.DataTable dtPUHead = null;
        U8Operation.GetDataString("select 'Count[" + BodyData.Rows.Count + "]'", Cmd);
        if (ordertype.CompareTo("采购订单") == 0)
        {
            dtPUHead = U8Operation.GetSqlDataTable("select top 1 a.cpoid,cVenCode,cPersonCode,cdepcode,cPTCode,cexch_name,nflat,isnull(iTaxRate,0) iTaxRate,cVenPUOMProtocol,cBusType from " + dbname +
                "..PO_Pomain a inner join " + dbname + "..PO_Podetails b on a.poid=b.poid where b.id=0" + posid, "dtPUHead", Cmd);
        }
        else
        {
            dtPUHead = U8Operation.GetSqlDataTable("select top 1 a.ccode cpoid,cVenCode,cPersonCode,cdepcode,cPTCode,cexch_name,nflat,isnull(iTaxRate,0) iTaxRate,cVenPUOMProtocol,cBusType from " + dbname +
                "..OM_MOMain a inner join " + dbname + "..OM_MODetails b on a.moid=b.moid where b.MODetailsID=0" + posid , "dtPUHead", Cmd);
        }
        //
        if (dtPUHead.Rows.Count > 0)
        {
            pumain.cDepCode = (w_cdepcode == "" ? "null" : "'" + w_cdepcode + "'");
            pumain.cPersonCode = (w_cpersoncode == "" ? "null" : "'" + w_cpersoncode + "'");
            pumain.cPTCode = (w_ptcode == "" ? "null" : "'" + w_ptcode + "'");
            pumain.cexch_name = "'" + dtPUHead.Rows[0]["cexch_name"] + "'";
            pumain.iExchRate = "" + dtPUHead.Rows[0]["nflat"];
            pumain.iTaxRate = "" + dtPUHead.Rows[0]["iTaxRate"];
            pumain.cVenPUOMProtocol = (dtPUHead.Rows[0]["cVenPUOMProtocol"] + "" == "" ? "null" : "'" + dtPUHead.Rows[0]["cVenPUOMProtocol"] + "'");
            pumain.cBusType = "'" + w_bustype + "'";
        }
        else
        {
            throw new Exception("没有找到订单信息");
        }

        #region   //主表自定义项处理
        pumain.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        pumain.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        pumain.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        pumain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        pumain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        pumain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        pumain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        pumain.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        pumain.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        pumain.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        pumain.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        pumain.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        pumain.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        pumain.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        pumain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        pumain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        #endregion

        #region //表头上游单据 继承
        if (bCreate)
        {
            DataTable dtMainDef = null;
            if (ordertype == "采购订单")
            {
                dtMainDef = U8Operation.GetSqlDataTable(@"select cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,
                    cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 from " + dbname + @"..PO_Pomain a 
                where POID=0" + poid, "dtMainDef", Cmd);
                if (dtMainDef.Rows.Count == 0) throw new Exception("没有找到采购订单主要信息");
            }
            if (ordertype == "委外订单")
            {
                dtMainDef = U8Operation.GetSqlDataTable(@"select cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,
                    cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 from " + dbname + @"..OM_MOMain a 
                where MOID=0" + poid, "dtMainDef", Cmd);
                if (dtMainDef.Rows.Count == 0) throw new Exception("没有找到委外订单主要信息");
            }
            if (dtMainDef != null)
            {
                if (pumain.cDefine1.CompareTo("''") == 0) pumain.cDefine1 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine1") + "'";
                if (pumain.cDefine2.CompareTo("''") == 0) pumain.cDefine2 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine2") + "'";
                if (pumain.cDefine3.CompareTo("''") == 0) pumain.cDefine3 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine3") + "'";

                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine4");  //日期
                if (pumain.cDefine4.CompareTo("''") == 0 || pumain.cDefine4.CompareTo("null") == 0) pumain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine5");
                if (pumain.cDefine5.CompareTo("") == 0 || pumain.cDefine5.CompareTo("null") == 0) pumain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine6");  //日期
                if (pumain.cDefine6.CompareTo("''") == 0 || pumain.cDefine6.CompareTo("null") == 0) pumain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine7");
                if (pumain.cDefine7.CompareTo("") == 0 || pumain.cDefine7.CompareTo("null") == 0) pumain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);

                if (pumain.cDefine8.CompareTo("''") == 0) pumain.cDefine8 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine8") + "'";
                if (pumain.cDefine9.CompareTo("''") == 0) pumain.cDefine9 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine9") + "'";
                if (pumain.cDefine10.CompareTo("''") == 0) pumain.cDefine10 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine10") + "'";
                if (pumain.cDefine11.CompareTo("''") == 0) pumain.cDefine11 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine11") + "'";
                if (pumain.cDefine12.CompareTo("''") == 0) pumain.cDefine12 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine12") + "'";
                if (pumain.cDefine13.CompareTo("''") == 0) pumain.cDefine13 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine13") + "'";
                if (pumain.cDefine14.CompareTo("''") == 0) pumain.cDefine14 = "'" + GetBodyValue_FromData(dtMainDef, 0, "cdefine14") + "'";

                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine15");
                if (pumain.cDefine15.CompareTo("") == 0 || pumain.cDefine15.CompareTo("null") == 0) pumain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
                txtdata_text = GetBodyValue_FromData(dtMainDef, 0, "cdefine16");
                if (pumain.cDefine16.CompareTo("") == 0 || pumain.cDefine16.CompareTo("null") == 0) pumain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
                
            }
        }
        #endregion

        //流程ID
        string i_flowid = U8Operation.GetDataString("select iflowid from " + dbname + "..PO_Pomain where POID=0" + poid, Cmd);
        pumain.iflowid = (i_flowid == "" ? "null" : i_flowid);
        if (!pumain.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

        //写入条码
        if (bcan_barcode)
        {
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set csysbarcode='pudh" + pumain.ID + "' where id=" + pumain.ID;
            Cmd.ExecuteNonQuery();
        }
        #endregion

        #region  //采购到货单子表
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            s_autoid = BodyData.Rows[i]["source_autoid"] + "";
            decimal d_quantity = decimal.Parse(BodyData.Rows[i]["iquantity"] + "");
            if ((i_red_sheet == 0 && d_quantity < 0) || (i_red_sheet == 1 && d_quantity > 0)) throw new Exception("本单据红蓝子混乱");

            if (d_quantity <= 0) 
            { 
                i_has_queshi_row++;

                if (c_asn_back_type == "proc" && w_souretype.CompareTo("ASN单") == 0)
                {
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = dbname + ".dbo.vendb_p_poSh_Arr";
                    SqlParameter[] sps = new SqlParameter[] {   
                        new SqlParameter("@id",s_autoid),  
                        new SqlParameter("@arrnum","0"),  
                        new SqlParameter("@arrweight","0"),
                        new SqlParameter("@arrpeice","0")
                    };
                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(sps);
                    object Result1 = Cmd.ExecuteScalar();//无数量，关闭送货
                    Cmd.CommandType = System.Data.CommandType.Text;
                }

                if (d_quantity == 0) continue; //实际到货数量<=0时 ，直接推出
            }

            i_has_valid_row++;
            posid = BodyData.Rows[i]["posid"] + "";
            
            //账套核对 (ASN单逻辑校验)
            if (w_souretype.CompareTo("ASN单") == 0)
            {
                if (c_asn_back_type == "proc")
                {
                    string asn_acount = U8Operation.GetDataString("select accountid from " + dbname + ".dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                    if (asn_acount.CompareTo(targetAccId) != 0) throw new Exception("ASN单不属于本账套");
                    string asn_arr_qty = U8Operation.GetDataString("select isnull(arr_num,0) from " + dbname + ".dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                    if (float.Parse(asn_arr_qty) > 0) throw new Exception("ASN 单已经到货，不能重复到货");
                }
                else
                {
                    string asn_acount = U8Operation.GetDataString("select accountid from VenDB.dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                    if (asn_acount.CompareTo(targetAccId) != 0) throw new Exception("ASN单不属于本账套");

                    //取参数[SRM的ASN单是否可多次收货]，控制是否可以多次送货
                    if (U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_multi_received'", Cmd).ToLower().CompareTo("true") == 0)
                    {
                        //判断是否超 送货 到货
                        if (U8Operation.GetDataInt("select count(*) from VenDB.dbo.ASN_C where id='" + s_autoid + "' and sh_num<isnull(arr_num,0)+(0" + BodyData.Rows[i]["iquantity"] + ")", Cmd) > 0)
                            throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]不能超送货单到货");
                    }else{
                        string asn_arr_qty = U8Operation.GetDataString("select isnull(arr_num,0) from VenDB.dbo.ASN_C where id='" + s_autoid + "'", Cmd);
                        if (float.Parse(asn_arr_qty) > 0) throw new Exception("ASN 单已经到货，不能重复到货");
                    }
                }
                
            }
            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    string chkValue = U8Operation.GetDataString("select b.cBusType from " + dbname + "..PO_Podetails a inner join " + dbname + @"..PO_Pomain b on a.poid=b.poid 
                    where a.ID=0" + posid, Cmd);
                    if (chkValue.CompareTo(w_bustype) != 0) throw new Exception("业务类型不一致：[" + w_bustype + "]与[" + chkValue + "]");
                    chkValue = U8Operation.GetDataString("select b.cvencode from " + dbname + "..PO_Podetails a inner join " + dbname + @"..PO_Pomain b on a.poid=b.poid 
                    where a.ID=0" + posid, Cmd);
                    if (chkValue.CompareTo(w_vencode) != 0) throw new Exception("供应商不一致：[" + w_vencode + "]与[" + chkValue + "]");
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    string chkValue = U8Operation.GetDataString("select b.cBusType from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMain b on a.MOID=b.MOID 
                    where a.MODetailsID=0" + posid, Cmd);
                    if (chkValue.CompareTo(w_bustype) != 0) throw new Exception("业务类型不一致：[" + w_bustype + "]与[" + chkValue + "]");
                    chkValue = U8Operation.GetDataString("select b.cvencode from " + dbname + "..OM_MODetails a inner join " + dbname + @"..OM_MOMain b on a.MOID=b.MOID 
                    where a.MODetailsID=0" + posid, Cmd);
                    if (chkValue.CompareTo(w_vencode) != 0) throw new Exception("供应商不一致：[" + w_vencode + "]与[" + chkValue + "]");
                }
            }

            KK_U8Com.U8PU_ArrivalVouchs pudetail = new KK_U8Com.U8PU_ArrivalVouchs(Cmd, dbname);
            int cAutoid = int.Parse(U8Operation.GetDataString("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'", Cmd));
            pudetail.Autoid = cAutoid + "";
            pudetail.ID = pumain.ID;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='PuArrival'";
            Cmd.ExecuteNonQuery();
            pudetail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            pudetail.iPOsID = posid;
            pudetail.iQuantity = BodyData.Rows[i]["iquantity"] + "";
            pudetail.fRealQuantity = pudetail.iQuantity;
            pudetail.SoType = "0";
            pudetail.ivouchrowno = "" + (i + 1);
            

            #region //自由项  自定义项处理
            pudetail.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            pudetail.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            pudetail.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            pudetail.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            pudetail.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            pudetail.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            pudetail.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            pudetail.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            pudetail.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            pudetail.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";
            //自定义项
            pudetail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            pudetail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            pudetail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            pudetail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            pudetail.cDefine26 = (txtdata_text == "" ? "0" :txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            pudetail.cDefine27 =  (txtdata_text == "" ? "0" :txtdata_text);
            pudetail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            pudetail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            pudetail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            pudetail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            //pudetail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            pudetail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            pudetail.cDefine34 =  (txtdata_text == "" ? "0" :txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            pudetail.cDefine35 =  (txtdata_text == "" ? "0" :txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            pudetail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            pudetail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            #endregion

            pudetail.cDefine32 = "'" + s_autoid + "'";  //ASN单子表ID  采购订单子表autoid   委外订单子表autoid

            #region  //价格处理
            DataTable dtOrderDetail = null ;   //
            string c_tax_cost = GetBodyValue_FromData(BodyData, i, "itaxprice") + "";
            if (c_tax_cost == "") c_tax_cost = "0";
            string c_cost = GetBodyValue_FromData(BodyData, i, "ccostprice") + "";
            if (c_cost == "") c_cost = "0";

            U8Operation.GetDataString("select 'test_4.....'", Cmd);
            if (decimal.Parse(c_tax_cost) == 0)
            {
                U8Operation.GetDataString("select 'test_5.....'", Cmd);
                if(decimal.Parse(c_cost) <= 0) 
                    c_tax_cost = "isnull(iTaxPrice,0)";  //价格为空时取订单价格
                else
                    c_tax_cost = "round((0" + c_cost + ")*(100+iPerTaxRate)/100,8)";
            }

            U8Operation.GetDataString("select 'test_6.....'", Cmd);
            //自由项
            if (ordertype.CompareTo("采购订单") == 0)
            {
                dtOrderDetail = U8Operation.GetSqlDataTable(@"select poid," + c_tax_cost + @" iTaxPrice,iUnitPrice,iPerTaxRate itaxrate,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37,SoType,SoDId, csocode,iorderseq,iordertype,iorderdid,csoordercode 
                        from " + dbname + "..PO_Podetails where id=0" + posid, "dtOrderDetail", Cmd);
                if (dtOrderDetail.Rows.Count > 0)
                    pudetail.cordercode = "'" + U8Operation.GetDataString("select cpoid from " + dbname + "..PO_Pomain where poid=" + dtOrderDetail.Rows[0]["poid"], Cmd) + "'";
            }
            else
            {
                dtOrderDetail = U8Operation.GetSqlDataTable(@"select MOID poid," + c_tax_cost + @" iTaxPrice,iUnitPrice,iPerTaxRate itaxrate,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                            cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                            cdefine35,cdefine36,cdefine37,SoType,SoDId,csocode,iorderseq,SoType iordertype,iorderdid,csoordercode 
                    from " + dbname + "..OM_MODetails where MODetailsID=0" + posid, "dtOrderDetail", Cmd);
                if (dtOrderDetail.Rows.Count > 0)
                    pudetail.cordercode = "'" + U8Operation.GetDataString("select ccode from " + dbname + "..OM_MOMain where moid=" + dtOrderDetail.Rows[0]["poid"], Cmd) + "'";
            }
            U8Operation.GetDataString("select 'test_7.....'", Cmd);
            if (dtOrderDetail.Rows.Count > 0)
            {
                if ((dtOrderDetail.Rows[0]["iTaxPrice"] + "").ToString().CompareTo("") != 0)  //有单价
                {
                    pudetail.iOriSum = "" + (float.Parse(pudetail.iQuantity + "") * float.Parse(dtOrderDetail.Rows[0]["iTaxPrice"] + "")).ToString("F2");
                    pudetail.iOriMoney = "" + (float.Parse(pudetail.iQuantity + "") * float.Parse(dtOrderDetail.Rows[0]["iUnitPrice"] + "")).ToString("F2");
                    pudetail.iOriCost = float.Parse(dtOrderDetail.Rows[0]["iUnitPrice"] + "").ToString("F6");
                    pudetail.iOriTaxCost = float.Parse(dtOrderDetail.Rows[0]["iTaxPrice"] + "").ToString("F6");
                }
                pudetail.iTaxRate = dtOrderDetail.Rows[0]["itaxrate"] + "";

                //销售跟踪 SoType,SoDId, csocode,iorderseq,iordertype,iorderdid,csoordercode,isorowno
                pudetail.SoType = (dtOrderDetail.Rows[0]["SoType"] + "" == "" ? "null" : dtOrderDetail.Rows[0]["SoType"] + "");
                pudetail.SoDId = (dtOrderDetail.Rows[0]["SoDId"] + "" == "" ? "null" : "'" + dtOrderDetail.Rows[0]["SoDId"] + "'");
                pudetail.csocode = "'" + dtOrderDetail.Rows[0]["csocode"] + "'";  //销售订单号
                pudetail.iorderseq = dtOrderDetail.Rows[0]["iorderseq"] + "" == "" ? "null" : dtOrderDetail.Rows[0]["iorderseq"] + "";
                pudetail.iordertype = dtOrderDetail.Rows[0]["iordertype"] + "" == "" ? "null" : dtOrderDetail.Rows[0]["iordertype"] + "";
                pudetail.iorderdid = dtOrderDetail.Rows[0]["iorderdid"] + "" == "" ? "null" : dtOrderDetail.Rows[0]["iorderdid"] + "";
                pudetail.csoordercode = "'" + dtOrderDetail.Rows[0]["csoordercode"] + "'";
                pudetail.isorowno = dtOrderDetail.Rows[0]["iorderseq"] + "" == "" ? "null" : dtOrderDetail.Rows[0]["iorderseq"] + "";

                if (bCreate)
                {
                    //继承到货单的表体自由项  自定义项数据
                    if (pudetail.cFree1.CompareTo("''") == 0) pudetail.cFree1 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree1") + "'";
                    if (pudetail.cFree2.CompareTo("''") == 0) pudetail.cFree2 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree2") + "'";
                    if (pudetail.cFree3.CompareTo("''") == 0) pudetail.cFree3 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree3") + "'";
                    if (pudetail.cFree4.CompareTo("''") == 0) pudetail.cFree4 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree4") + "'";
                    if (pudetail.cFree5.CompareTo("''") == 0) pudetail.cFree5 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree5") + "'";
                    if (pudetail.cFree6.CompareTo("''") == 0) pudetail.cFree6 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree6") + "'";
                    if (pudetail.cFree7.CompareTo("''") == 0) pudetail.cFree7 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree7") + "'";
                    if (pudetail.cFree8.CompareTo("''") == 0) pudetail.cFree8 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree8") + "'";
                    if (pudetail.cFree9.CompareTo("''") == 0) pudetail.cFree9 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree9") + "'";
                    if (pudetail.cFree10.CompareTo("''") == 0) pudetail.cFree10 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cfree10") + "'";

                    if (pudetail.cDefine22.CompareTo("''") == 0) pudetail.cDefine22 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine22") + "'";
                    if (pudetail.cDefine23.CompareTo("''") == 0) pudetail.cDefine23 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine23") + "'";
                    if (pudetail.cDefine24.CompareTo("''") == 0) pudetail.cDefine24 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine24") + "'";
                    if (pudetail.cDefine25.CompareTo("''") == 0) pudetail.cDefine25 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine25") + "'";

                    if (pudetail.cDefine22.CompareTo("''") == 0) pudetail.cDefine28 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine28") + "'";
                    if (pudetail.cDefine22.CompareTo("''") == 0) pudetail.cDefine29 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine29") + "'";
                    if (pudetail.cDefine30.CompareTo("''") == 0) pudetail.cDefine30 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine30") + "'";
                    if (pudetail.cDefine31.CompareTo("''") == 0) pudetail.cDefine31 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine31") + "'";
                    if (pudetail.cDefine32.CompareTo("''") == 0) pudetail.cDefine32 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine32") + "'";
                    if (pudetail.cDefine33.CompareTo("''") == 0) pudetail.cDefine33 = "'" + GetBodyValue_FromData(dtOrderDetail, 0, "cdefine33") + "'";

                }
            }
            //补充订单信息
            pudetail.bTaxCost = "1";
            #endregion

            #region//辅助计量
            //获得换算率
            string c_st_unit = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + @"..inventory where cinvcode=" + pudetail.cInvCode, Cmd);
            decimal d_changerate = decimal.Parse(U8Operation.GetDataString("select isnull(max(iChangRate),0) from " + dbname + @"..ComputationUnit where cComunitCode='" + c_st_unit + "'", Cmd));
            pudetail.iNum = "0";
            if (d_changerate > 0)
            {
                pudetail.iNum = "" + (Math.Round(decimal.Parse(pudetail.iQuantity) / d_changerate, 5));
                pudetail.iinvexchrate = d_changerate + "";
            }
            #endregion

            #region //批次管理和保质期管理
            pudetail.cBatch = "''";
            pudetail.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(pudetail.cInvCode + "有批次管理，必须输入批号");
                pudetail.cBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    if ((!BodyData.Columns.Contains("dprodate")) || BodyData.Rows[i]["dprodate"] + "" == "")  //生产日期判定
                        throw new Exception(pudetail.cInvCode + "有保质期管理，必须输入生产日期");
                    string rowpordate = "" + BodyData.Rows[i]["dprodate"];
                    if (rowpordate == "") rowpordate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
                    DataTable dtBZQ = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                                else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                            ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                        from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                        where cinvcode=" + pudetail.cInvCode, "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count == 0) throw new Exception("计算存货保质期出现错误");

                    pudetail.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    pudetail.imassdate = dtBZQ.Rows[0]["保质期天数"] + "";
                    pudetail.dVDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    pudetail.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    pudetail.dPDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    pudetail.cmassunit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + pudetail.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    //从模板中获得批次属性
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty1");
                    pudetail.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty2");
                    pudetail.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty3");
                    pudetail.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty4");
                    pudetail.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty5");
                    pudetail.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    pudetail.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty6") + "'";
                    pudetail.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty7") + "'";
                    pudetail.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty8") + "'";
                    pudetail.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty9") + "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty10");
                    pudetail.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                    //继承批次档案数据
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                            where cInvCode=" + pudetail.cInvCode + " and cBatch=" + pudetail.cBatch + " and isnull(cFree1,'')=" + pudetail.cFree1 + @" 
                                and isnull(cFree2,'')=" + pudetail.cFree2 + " and isnull(cFree3,'')=" + pudetail.cFree3 + " and isnull(cFree4,'')=" + pudetail.cFree4 + @" 
                                and isnull(cFree5,'')=" + pudetail.cFree5 + " and isnull(cFree6,'')=" + pudetail.cFree6 + " and isnull(cFree7,'')=" + pudetail.cFree7 + @" 
                                and isnull(cFree8,'')=" + pudetail.cFree8 + " and isnull(cFree9,'')=" + pudetail.cFree9 + " and isnull(cFree10,'')=" + pudetail.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count > 0)
                    {
                        pudetail.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                        pudetail.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                        pudetail.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                        pudetail.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                        pudetail.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                        pudetail.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                        pudetail.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                        pudetail.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                        pudetail.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                        pudetail.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";

                        //更新批次档案
                        Cmd.CommandText = "update " + dbname + "..AA_BatchProperty set cBatchProperty1=" + pudetail.cBatchProperty1 + ",cBatchProperty2=" + pudetail.cBatchProperty2 + @",
                                cBatchProperty3=" + pudetail.cBatchProperty3 + ",cBatchProperty4=" + pudetail.cBatchProperty4 + ",cBatchProperty5=" + pudetail.cBatchProperty5 + @",
                                cBatchProperty6=" + pudetail.cBatchProperty6 + ",cBatchProperty7=" + pudetail.cBatchProperty7 + ",cBatchProperty8=" + pudetail.cBatchProperty8 + @",
                                cBatchProperty9=" + pudetail.cBatchProperty9 + ",cBatchProperty10=" + pudetail.cBatchProperty10 + @" 
                            where cInvCode=" + pudetail.cInvCode + " and cBatch=" + pudetail.cBatch + " and isnull(cFree1,'')=" + pudetail.cFree1 + @" 
                                and isnull(cFree2,'')=" + pudetail.cFree2 + " and isnull(cFree3,'')=" + pudetail.cFree3 + " and isnull(cFree4,'')=" + pudetail.cFree4 + @" 
                                and isnull(cFree5,'')=" + pudetail.cFree5 + " and isnull(cFree6,'')=" + pudetail.cFree6 + " and isnull(cFree7,'')=" + pudetail.cFree7 + @" 
                                and isnull(cFree8,'')=" + pudetail.cFree8 + " and isnull(cFree9,'')=" + pudetail.cFree9 + " and isnull(cFree10,'')=" + pudetail.cFree10;
                        Cmd.ExecuteNonQuery();
                    }
                    else  //建立档案
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                            values(newid()," + pudetail.cBatchProperty1 + "," + pudetail.cBatchProperty2 + "," + pudetail.cBatchProperty3 + "," + pudetail.cBatchProperty4 + "," +
                                 pudetail.cBatchProperty5 + "," + pudetail.cBatchProperty6 + "," + pudetail.cBatchProperty7 + "," + pudetail.cBatchProperty8 + "," +
                                 pudetail.cBatchProperty9 + "," + pudetail.cBatchProperty10 + "," + pudetail.cInvCode + "," + pudetail.cBatch + "," + pudetail.cFree1 + "," +
                                 pudetail.cFree2 + "," + pudetail.cFree3 + "," + pudetail.cFree4 + "," + pudetail.cFree5 + "," + pudetail.cFree6 + "," +
                                 pudetail.cFree7 + "," + pudetail.cFree8 + "," + pudetail.cFree9 + "," + pudetail.cFree10 + ")";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion

            
            #region  //质检处理   
            pudetail.bGsp = "0";
            if (bCreate)
            {
                if (ordertype.CompareTo("采购订单") == 0)
                {
                    pudetail.bGsp = U8Operation.GetDataString("select cast(bGsp as int) from " + dbname + "..PO_Podetails where ID=0" + posid, Cmd);
                    pudetail.fValidQuantity = pudetail.iQuantity;//不检验直接写合格数
                }
                else if (ordertype.CompareTo("委外订单") == 0)
                {
                    pudetail.bGsp = U8Operation.GetDataString("select cast(bGsp as int) from " + dbname + "..OM_MODetails where MODetailsID=0" + posid, Cmd);
                    pudetail.fValidQuantity = pudetail.iQuantity;//不检验直接写合格数
                }
                else
                {
                    pudetail.bGsp = "0";
                    pudetail.fValidQuantity = pudetail.iQuantity;//不检验直接写合格数
                }
                if (pudetail.bGsp == "1")
                {
                    pudetail.fValidQuantity = "0";//检验不写合格数
                }
            }
            else
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + pudetail.cInvCode + " and bPropertyCheck=1 and isnull(iTestStyle,0)<>1", Cmd) > 0)
                {
                    pudetail.bGsp = "1";
                }
                else
                {
                    pudetail.bGsp = "0";
                    pudetail.fValidQuantity = pudetail.iQuantity;//不检验直接写合格数
                }
            }

            if (pudetail.bGsp == "1") i_qc_inv_count++;
            #endregion

            
            //记录是否有缺失记录
            decimal dsourceqty = 0;
            try
            {
                dsourceqty = decimal.Parse(BodyData.Rows[i]["isource_qty"] + "");
                if (dsourceqty < 0) throw new Exception("上游单据数量[isource_qty]必须大于0");
            }
            catch
            {
                //ASN单必须要isource_qty 上游单据数量
                if (w_souretype.CompareTo("ASN单") == 0) throw new Exception("上游单据数量[isource_qty]栏目不能为空");
                
            }
            if (decimal.Parse(BodyData.Rows[i]["iquantity"] + "") < dsourceqty) i_has_queshi_row++;
            if (!pudetail.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

            #region  //ASN 单号保存
            if (w_souretype.CompareTo("ASN单") == 0)
            {
                string c_asn_code_save_field = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_ASN_Code_Saved'", Cmd);
                if (c_asn_code_save_field.Trim() != "")
                {
                    string c_asn_code = "";
                    if (c_asn_back_type == "proc")
                    {
                        c_asn_code = U8Operation.GetDataString("select asnno from " + dbname + ".dbo.ASN_H a inner join " + dbname + ".dbo.ASN_C b on a.id=b.hid where b.id='" + s_autoid + "'", Cmd);
                    }
                    else
                    {
                        c_asn_code = U8Operation.GetDataString("select asnno from VenDB.dbo.ASN_H a inner join VenDB.dbo.ASN_C b on a.id=b.hid where b.id='" + s_autoid + "'", Cmd);
                    }
                    Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set " + c_asn_code_save_field + "='" + c_asn_code + "' where autoid=" + pudetail.Autoid;
                    Cmd.ExecuteNonQuery();
                }
            }
            #endregion


            //写入条码
            if (bcan_barcode)
            {
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode='pudh" + pudetail.Autoid + @"',
                   cbMemo='" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "' where autoid=" + pudetail.Autoid;
                Cmd.ExecuteNonQuery();
            }

            #region //上游单据回写
            if (ordertype.CompareTo("采购订单") == 0)
            {
                //回写订单的累计到货数
                if (decimal.Parse(pudetail.iQuantity) > 0)
                {
                    Cmd.CommandText = "update " + dbname + "..PO_Podetails set iarrqty=isnull(iarrqty,0)+(0" + pudetail.iQuantity + "),iArrNum=isnull(iArrNum,0)+(0" + pudetail.iNum + ") where id=0" + posid;
                    Cmd.ExecuteNonQuery();
                    //Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=iarrqty where id=0" + posid;
                    //2022年10月19日 fpoarrquantity（到货数量）取正向业务，不考虑拒收。
                    Cmd.CommandText = "update " + dbname + "..PO_Podetails set fpovalidquantity=iarrqty,fpoarrquantity=isnull(fpoarrquantity,0)+0" + pudetail.iQuantity + " where id=0" + posid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..PO_Podetails set fPoRetQuantity =isnull(fPoRetQuantity,0)+(0" + pudetail.iQuantity + ") where id=0" + posid;
                    Cmd.ExecuteNonQuery();
                }

                if (fU8Version >= 12)
                {
                    //赠品处理
                    string c_gift = U8Operation.GetDataString("select cast(bgift as int) from " + dbname + "..po_podetails where ID=0" + posid, Cmd);
                    Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)),bgift=0" + c_gift + " where Autoid=0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)) where Autoid=0" + cAutoid;
                    Cmd.ExecuteNonQuery();
                }

                //是否超订单
                if (U8Operation.GetDataInt("select cast(CAST(cValue as bit) as int) from " + dbname + "..AccInformation where cSysID='pu' and cName='bOverPO'", Cmd) == 0)
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PO_Podetails where id=0" + posid + " and isnull(iarrqty,0)-iQuantity>0.00000001", Cmd) > 0)
                        throw new Exception(pudetail.cInvCode + "超采购订单到货");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PO_Podetails where id=0" + posid + " and isnull(iarrqty,0)<-0.00000001", Cmd) > 0)
                    throw new Exception(pudetail.cInvCode + "总到货量为负数");
            }
            else  //委外订单
            {
                #region //检查非倒冲材料 是否已经领料
                int i_check_mer_out = U8Operation.GetDataInt("SELECT cValue FROM " + dbname + @"..AccInformation WHERE cSysID = 'OM' and cname='iOMControlTypeOfIn'", Cmd);
                //判断是否存在非倒冲记录  
                DataTable dtMer_out = U8Operation.GetSqlDataTable(@"select top 1 a.cInvCode,min(isnull(a.iSendQTY,0)*b.iQuantity/a.iQuantity) + 0.001 low_qty
                        from " + dbname + "..OM_MOMaterials a inner join " + dbname + @"..OM_MODetails b on a.MoDetailsID=b.MODetailsID
                        where a.MoDetailsID=0" + posid + " and a.iWIPtype=3 and a.iQuantity<>0 group by a.cInvCode order by low_qty", "dtMer_out", Cmd);

                if (i_check_mer_out == 2 && dtMer_out.Rows.Count > 0 && decimal.Parse(BodyData.Rows[i]["iquantity"] + "") >= 0)
                {
                    decimal d_om_qty = decimal.Parse("" + dtMer_out.Rows[0]["low_qty"]);
                    decimal d_rd_qty = decimal.Parse(U8Operation.GetDataString("select isnull(sum(a.iquantity),0) from " + dbname + @"..PU_ArrivalVouchs a 
                        inner join " + dbname + @"..PU_ArrivalVouch b on a.id=b.id where iPOsID=0" + posid+" and b.cbustype='委外加工'", Cmd));
                    if (d_om_qty < d_rd_qty) throw new Exception("材料[" + dtMer_out.Rows[0]["cInvCode"] + "]领用不足");
                }
                #endregion


                //回写订单的累计到货数
                Cmd.CommandText = "update " + dbname + "..OM_MODetails set iArrQTY=isnull(iArrQTY,0)+(0" + pudetail.iQuantity + ") where MODetailsID=0" + posid;
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cbsysbarcode=cast(autoid as nvarchar(30)),iproducttype=0 where Autoid=0" + cAutoid;
                Cmd.ExecuteNonQuery();
                //是否超订单
                if (U8Operation.GetDataInt("select cast(CAST(cValue as bit) as int) from " + dbname + "..AccInformation where cSysID='om' and cName='bOMOverPo'", Cmd) == 0)
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..OM_MODetails where MODetailsID=0" + posid + " and isnull(iArrQTY,0)-iQuantity>0.00000001", Cmd) > 0)
                        throw new Exception(pudetail.cInvCode + "超委外订单到货");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..OM_MODetails where MODetailsID=0" + posid + " and isnull(iArrQTY,0)<-0.00000001", Cmd) > 0)
                    throw new Exception(pudetail.cInvCode + "总到货量为负数");
            }

            //储存临时货位 cBatchProperty9
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouchs set cBatchProperty9='" + BodyData.Rows[i]["cposcode"] + "' where Autoid=0" + cAutoid;
            Cmd.ExecuteNonQuery();

            if (w_souretype.CompareTo("ASN单") == 0)
            {
                if (c_asn_back_type == "proc" && d_quantity >0)
                {
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = dbname + ".dbo.vendb_p_poSh_Arr";
                    SqlParameter[] sps = new SqlParameter[] {   
                        new SqlParameter("@id",s_autoid),  
                        new SqlParameter("@arrnum",pudetail.iQuantity),  
                        new SqlParameter("@arrweight",pudetail.cDefine26),
                        new SqlParameter("@arrpeice",pudetail.iNum)
                    };
                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(sps);
                    object Result1 = Cmd.ExecuteScalar();
                    Cmd.CommandType = System.Data.CommandType.Text;
                }
                else
                {
                    //回写ASN单记录
                    Cmd.CommandText = "update VenDB.dbo.ASN_C set rowstate='是',arr_num=isnull(arr_num,0)+(" + pudetail.iQuantity + ") where ID='" + s_autoid + "'";
                    Cmd.ExecuteNonQuery();

                    //审批状态  spstate='3' 代表已经接收
                    Cmd.CommandText = "update VenDB.dbo.ASN_H set spstate='3',realData=getdate() where ID in(select HID from VenDB.dbo.ASN_C where ID='" + s_autoid + "')";
                    Cmd.ExecuteNonQuery();
                    System.Data.DataTable dtASNDT = U8Operation.GetSqlDataTable(@"select a.asnjsjgs reciveVencode,c.id fromVencode,b.ID
                    from VenDB.dbo.ASN_H a inner join VenDB.dbo.ASN_C b on a.ID=b.HID 
                    inner join VenDB.dbo.sys_office c on a.officeID=c.code and a.accountid=c.accountid
                    where a.acttypes='02' and b.ID='" + s_autoid + "'", "dtASNDT", Cmd);
                    if (dtASNDT.Rows.Count > 0)
                    {
                        //更新库存
                        Cmd.CommandText = "update VenDB.dbo.MX_KC_MANAGER set curNum=curNum-(0" + pudetail.iQuantity + ") where accountid='" + targetAccId + @"' 
                        and fromVencode='" + dtASNDT.Rows[0]["fromVencode"] + "' and reciveVencode='" + dtASNDT.Rows[0]["reciveVencode"] + "' and cvincode=" + pudetail.cInvCode;
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion
        }

        
        #endregion

        //判断质检物资是否能审核
        if (i_qc_inv_count == 0) b_qc_inv_not_checked = false;  //无质检物资，不执行参数
        
        if (b_qc_inv_not_checked)  //最高等级 要求不审核
        {
            //取消审核
            Cmd.CommandText = "update " + dbname + @"..PU_ArrivalVouch set cverifier=null,cAuditDate=null,IsWfControlled=1,iverifystateex=null 
                where id=" + pumain.ID + " and isnull(cverifier,'')<>''";
            Cmd.ExecuteNonQuery();

            b_checked = false;  //不审核
        }

        if (i_has_valid_row == 0) throw new Exception("不存在合法收货记录");//没有合法到货单记录
        //回写asn 主表信息
        Cmd.CommandText = "select '" + asn_head_id + "'";
        Cmd.ExecuteNonQuery();

        if (asn_head_id != "" && c_asn_back_type == "proc")
        {
            Cmd.CommandText = "select '更新ASN主表状态，调用主表过程'";
            Cmd.ExecuteNonQuery();

            Cmd.CommandType = System.Data.CommandType.StoredProcedure;
            Cmd.CommandText = dbname + ".dbo.vendb_p_poSh_updateASNState";
            SqlParameter[] sps = new SqlParameter[] {   
                        new SqlParameter("@hid",asn_head_id)
                    };
            Cmd.Parameters.Clear();
            Cmd.Parameters.AddRange(sps);
            object Result1 = Cmd.ExecuteScalar();
            Cmd.CommandType = System.Data.CommandType.Text;
        }

        if (b_checked)
        {
            #region   //自动采购入库
            string cautord01 = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_auto_rd01'", Cmd);
            if (cautord01.CompareTo("true") == 0)
            {
                //仓库校验
                txtdata = GetTextsFrom_FormData(HeadData, "txt_cwhcode");
                w_cwhcode = txtdata[2];

                //组合采购入库单
                DataTable dtRdMain = U8Operation.GetSqlDataTable(@"select '" + w_cwhcode + @"' cwhcode,cdepcode,cptcode,cpersoncode,
	                    cvencode,cbustype,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                    cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cmemo
                        " + (c_vou_checker == "" ? "" : ",'" + c_vou_checker + "' checker") + @"
                        " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                    from " + dbname + "..PU_ArrivalVouch where id=0" + pumain.ID, "HeadData", Cmd);
                DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cbatch,b.iquantity,a.cVenCode cbvencode,CONVERT(varchar(10),b.dPDate,120) dprodate,
	                    b.autoid arr_autoid,b.cBatchProperty9 cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                    cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                    cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,cbmemo
                    from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.ID 
                    inner join " + dbname + "..inventory i on b.cInvCode=i.cInvCode where b.ID=0" + pumain.ID + " and bGsp=0", "BodyData", Cmd);
                if (dtRdMain.Rows.Count == 0) throw new Exception("无法找到到货单");
                if (dtRddetail.Rows.Count > 0)
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
                        throw new Exception("仓库输入错误");

                    DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                    SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                    U81014(SHeadData, dtRddetail, dbname, cUserName, cLogDate, "U81014", Cmd);
                }
            }
            #endregion

            #region //自动来料报检
            string qm_01 = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='ven_puarr_auto_qm'", Cmd);
            if (qm_01.CompareTo("true") == 0)
            {
                //组合采购入库单
                DataTable dtRdMain = U8Operation.GetSqlDataTable(@"select cdepcode,cptcode,cpersoncode,'QM01' cvouchtype,cmemo,
                    cvencode,cbustype,cdefine1 define1,cdefine2 define2,cdefine3 define3,cdefine4 define4,cdefine5 define5,cdefine6 define6,
                    cdefine7 define7,cdefine8 define8,cdefine9 define9,cdefine10 define10,cdefine11 define11,cdefine12 define12,cdefine13 define13,cdefine14 define14,cdefine15 define15,cdefine16 define16
                    " + (c_vou_checker == "" ? "" : ",'" + c_vou_checker + "' checker") + @"
                    " + (trans_code == "" ? "" : ",'" + trans_code + "' mes_code") + @"
                from " + dbname + "..PU_ArrivalVouch where id=0" + pumain.ID, "HeadData", Cmd);
                DataTable dtRddetail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cbatch,b.iquantity,a.cVenCode cbvencode,CONVERT(varchar(10),b.dPDate,120) dprodate,
                    b.autoid arr_autoid,cfree1 free1,cfree2 free2,cfree3 free3,cfree4 free4,cfree5 free5,cfree6 free6,cfree7 free7,cfree8 free8,cfree9 free9,cfree10 Free10,
                    cdefine22 define22,cdefine23 define23,cdefine24 define24,cdefine25 define25,cdefine26 define26,cdefine27 define27,cdefine28 define28,cdefine29 define29,cdefine30 define30,
                    cdefine31 define31,cdefine32 define32,cdefine33 define33,cdefine34 define34,cdefine35 define35,cdefine36 define36,cdefine37 define37,
                    b.autoid sourceautoid
                from " + dbname + "..PU_ArrivalVouch a inner join " + dbname + @"..PU_ArrivalVouchs b on a.id=b.ID 
                inner join " + dbname + "..inventory i on b.cInvCode=i.cInvCode where b.ID=0" + pumain.ID + " and bGsp=1", "BodyData", Cmd);
                if (dtRdMain.Rows.Count == 0) throw new Exception("无法找到到货单");

                DataTable SHeadData = GetDtToHeadData(dtRdMain, 0);
                SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
                foreach (DataRow drArr in dtRddetail.Rows)
                {
                    DataTable dtOneRow = dtRddetail.Clone();
                    dtOneRow.Rows.Add(drArr.ItemArray);
                    U81108(SHeadData, dtOneRow, dbname, cUserName, cLogDate, "U81108", Cmd);
                }
            }
            #endregion

            //审核到货单状态
            Cmd.CommandText = "update " + dbname + "..PU_ArrivalVouch set cverifier='" + cUserName + "',cAuditDate='" + cLogDate + "' where id=" + pumain.ID;
            Cmd.ExecuteNonQuery();
        }
        return pumain.ID + "," + cc_mcode;

    }

    //盘点单
    private string U81038(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_vencode = "";
        string w_rdcode = ""; //出库类型
        string w_inrdcode = ""; //入库类型
        string w_bustype = "";//业务类型
        string w_czmdate = "";//账面日期
        bool b_Vmi = false; bool b_Pos = false;//货位
        string itrid = ""; string itransid = ""; 
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string cfirstToFirst = "";//出库批次是否进行先进先出
        string w_cheadpos = "";//表头货位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值

        #region  //逻辑检验
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) w_bustype = "盘点";

        w_czmdate = GetTextsFrom_FormData_Tag(HeadData, "txt_ccwdate");//账面日期
        if (w_czmdate.CompareTo("") == 0) w_czmdate = cLogDate;

        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("出库类型输入错误");
        }
        w_inrdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinrdcode");
        if (w_inrdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_inrdcode + "' and bRdEnd=1", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        //表头货位
        w_cheadpos = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        if (w_cheadpos.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Position where cPosCode='" + w_cheadpos + "' and cWhCode='" + w_cwhcode + "'", Cmd) == 0)
                throw new Exception("货位输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        #region //单据
        KK_U8Com.U8CheckVouch check = new KK_U8Com.U8CheckVouch(Cmd, dbname);
        #region  //新增主表
        string c_vou_checker = GetTextsFrom_FormData_Text(HeadData, "txt_checker");  //审核人
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='ch'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='ch'";
        Cmd.ExecuteNonQuery();
        string cCodeHead = "P" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd);
        cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(ccvcode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..CheckVouch where ccvcode like '" + cCodeHead + "%'", Cmd);
        check.cCVCode = "'" + cc_mcode + "'";
        check.ID = rd_id;

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;

        if (b_Pos)
        {
            check.bPosCheck = "1";
        }
        else
        {
            check.bPosCheck = "0";
        }
        check.cDepCode = w_cdepcode == "" ? "null" : "'" + w_cdepcode + "'";
        check.cIRdCode = w_inrdcode == "" ? "null" : "'" + w_inrdcode + "'";
        check.cORdCode = w_rdcode == "" ? "null" : "'" + w_rdcode + "'";
        check.cPersonCode = w_cpersoncode == "" ? "null" : "'" + w_cpersoncode + "'";
        check.cWhCode = "'" + w_cwhcode + "'";
        check.cMaker = "'" + cUserName + "'";
        check.dCVDate = "'" + cLogDate + "'";
        check.dACDate = "'" + w_czmdate + "'";  //账面日期

        #region   //主表自定义项处理
        check.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        check.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        check.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        check.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        check.cDefine5 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        check.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        check.cDefine7 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        check.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        check.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        check.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        check.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        check.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        check.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        check.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        check.cDefine15 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        check.cDefine16 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        #endregion

        if (!check.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
        #endregion

        #region //子表
        //BodyData 数据按照存货编码 批号排序
        #region //排序
        string csortcolist = "cinvcode";
        if (BodyData.Columns.Contains("cbatch")) csortcolist += ",cbatch";
        if (BodyData.Columns.Contains("cbvencode")) csortcolist += ",cbvencode";
        if (BodyData.Columns.Contains("cposcode")) csortcolist += ",cposcode";
        for (int f = 1; f < 11; f++) { if (BodyData.Columns.Contains("cfree" + f)) csortcolist += ",cfree" + f; }
        BodyData.DefaultView.Sort = csortcolist;
        BodyData = BodyData.DefaultView.ToTable();
        #endregion

        string i_old_rd_autoid = "";  //记录最近一次的id
        string c_old_lan_name = "";   //当发现本次盘点有相同的存货+批号+..时，系统不自动增行，而是在原行的数量上累加
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string c_new_lan_name = "";
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            string c_pos_code = GetBodyValue_FromData(BodyData, i, "cposcode");//货位
            #region  //行业务逻辑校验
            if ("" + BodyData.Rows[i]["iquantity"]=="") throw new Exception("盘点数不能为空");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            c_new_lan_name = "" + BodyData.Rows[i]["cinvcode"] + c_body_batch + cvmivencode + c_pos_code;
            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }

                c_new_lan_name += cfree_value;
            }
            #endregion

            #region   //判定是否重复的存货
            if (c_new_lan_name.CompareTo(c_old_lan_name) == 0)  //核心栏目相同
            {
                //盘点数量  增加
                Cmd.CommandText = "update " + dbname + "..CheckVouchs set iCVCQuantity=isnull(iCVCQuantity,0)+(0" + BodyData.Rows[i]["iquantity"] + ") where autoID=0" + i_old_rd_autoid;
                Cmd.ExecuteNonQuery();
                continue;
            }
            else
            {
                c_old_lan_name = c_new_lan_name;
            }
            #endregion


            KK_U8Com.U8CheckVouchs checks = new KK_U8Com.U8CheckVouchs(Cmd, dbname);
            int cAutoid = U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='ch'", Cmd);
            checks.autoID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='ch'";
            Cmd.ExecuteNonQuery();

            checks.ID = check.ID;
            i_old_rd_autoid = cAutoid + "";
            checks.cCVCode = "'" + cc_mcode + "'";
            checks.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            checks.irowno = "" + (i + 1);
            checks.iCVCQuantity = "" + BodyData.Rows[i]["iquantity"];   //盘点数量

            #region //自由项  自定义项处理
            checks.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            checks.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            checks.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            checks.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            checks.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            checks.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            checks.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            checks.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            checks.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            checks.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            //自定义项
            checks.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            checks.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            checks.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            checks.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            checks.cDefine26 = (txtdata_text == "" ? "0" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            checks.cDefine27 = (txtdata_text == "" ? "0" : txtdata_text);
            checks.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            checks.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            checks.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            checks.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            checks.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            checks.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            checks.cDefine34 = (txtdata_text == "" ? "0" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            checks.cDefine35 = (txtdata_text == "" ? "0" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            checks.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            checks.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            #endregion

            #region //代管挂账处理
            checks.cvmivencode = "''";
            if (b_Vmi)
            {
                checks.cvmivencode = "'" + cvmivencode + "'";
            }
            #endregion

            #region //批次管理和保质期管理
            checks.cCVBatch = "''";
            checks.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + checks.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if (GetBodyValue_FromData(BodyData, i, "cbatch") == "") throw new Exception(checks.cInvCode + "有批次管理，必须输入批号");
                checks.cCVBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + checks.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    DataTable batchData = U8Operation.GetSqlDataTable(@"select iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,
                            convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                        from " + dbname + "..currentstock(nolock) where cinvcode=" + checks.cInvCode + @" 
                        and cbatch=" + checks.cCVBatch + " and cfree1=" + checks.cFree1 + " and cfree2=" + checks.cFree2 + " and cfree3=" + checks.cFree3 + @"  
                        and cfree4=" + checks.cFree4 + " and cfree5=" + checks.cFree5 + " and cfree6=" + checks.cFree6 + " and cfree7=" + checks.cFree7 + @" 
                        and cfree8=" + checks.cFree8 + " and cfree9=" + checks.cFree9 + " and cfree10=" + checks.cFree10,"batchData", Cmd);

                    if (batchData.Rows.Count == 0)
                    {
                        batchData = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + w_czmdate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + w_czmdate + @"')
                                                when cMassUnit=2 then DATEADD(month,iMassDate,'" + w_czmdate + @"')
                                                else DATEADD(day,iMassDate,'" + w_czmdate + @"') end)
                            ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                        from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                        where cinvcode=" + checks.cInvCode, "dtBZQ", Cmd);
                    }

                    if (batchData.Rows.Count > 0)
                    {
                        checks.iExpiratDateCalcu = batchData.Rows[0]["有效期推算方式"] + "";
                        checks.iMassDate = batchData.Rows[0]["保质期天数"] + "";
                        checks.dDisDate = "'" + batchData.Rows[0]["失效日期"] + "'";
                        checks.cExpirationdate = "'" + batchData.Rows[0]["失效日期"] + "'";
                        checks.dMadeDate = "'" + batchData.Rows[0]["生产日期"] + "'";
                        checks.cMassUnit = "'" + batchData.Rows[0]["保质期单位"] + "'";
                    }
                    else
                    {
                        throw new Exception(checks.cInvCode + "有保质期管理，但无法获得生产日期等信息");
                    }
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + checks.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                                cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                            where cInvCode=" + checks.cInvCode + " and cBatch=" + checks.cCVBatch + " and isnull(cFree1,'')=" + checks.cFree1 + @" 
                                and isnull(cFree2,'')=" + checks.cFree2 + " and isnull(cFree3,'')=" + checks.cFree3 + " and isnull(cFree4,'')=" + checks.cFree4 + @" 
                                and isnull(cFree5,'')=" + checks.cFree5 + " and isnull(cFree6,'')=" + checks.cFree6 + " and isnull(cFree7,'')=" + checks.cFree7 + @" 
                                and isnull(cFree8,'')=" + checks.cFree8 + " and isnull(cFree9,'')=" + checks.cFree9 + " and isnull(cFree10,'')=" + checks.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count > 0)
                    {
                        checks.cBatchProperty1 = (dtBatPerp.Rows[0]["cBatchProperty1"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty1"] + "";
                        checks.cBatchProperty2 = (dtBatPerp.Rows[0]["cBatchProperty2"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty2"] + "";
                        checks.cBatchProperty3 = (dtBatPerp.Rows[0]["cBatchProperty3"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty3"] + "";
                        checks.cBatchProperty4 = (dtBatPerp.Rows[0]["cBatchProperty4"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty4"] + "";
                        checks.cBatchProperty5 = (dtBatPerp.Rows[0]["cBatchProperty5"] + "").CompareTo("") == 0 ? "null" : dtBatPerp.Rows[0]["cBatchProperty5"] + "";
                        checks.cBatchProperty6 = "'" + dtBatPerp.Rows[0]["cBatchProperty6"] + "'";
                        checks.cBatchProperty7 = "'" + dtBatPerp.Rows[0]["cBatchProperty7"] + "'";
                        checks.cBatchProperty8 = "'" + dtBatPerp.Rows[0]["cBatchProperty8"] + "'";
                        checks.cBatchProperty9 = "'" + dtBatPerp.Rows[0]["cBatchProperty9"] + "'";
                        checks.cBatchProperty10 = (dtBatPerp.Rows[0]["cBatchProperty10"] + "").CompareTo("") == 0 ? "null" : "'" + dtBatPerp.Rows[0]["cBatchProperty10"] + "'";
                    }
                }
            }
            #endregion

            #region //固定换算率（多计量）
            checks.iCVCNum = "null";
            cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory where cinvcode=" + checks.cInvCode + "", Cmd);
            if (cst_unitcode.CompareTo("") != 0)
            {
                checks.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + checks.cAssUnit, Cmd);
                if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                checks.iinvexchrate = ichange;
                inum = U8Operation.GetDataString("select round(" + checks.iCVCQuantity + "/" + ichange + ",5)", Cmd);
                checks.iCVCNum = inum;
            }
            #endregion

            string pos_qty = "" + U8Operation.GetDataString("select isnull(max(iquantity),0) from " + dbname + "..currentstock(nolock) where cwhcode=" + check.cWhCode + @" 
                    and cvmivencode=" + checks.cvmivencode + " and cinvcode=" + checks.cInvCode + @" 
                    and cbatch=" + checks.cCVBatch + " and cfree1=" + checks.cFree1 + " and cfree2=" + checks.cFree2 + " and cfree3=" + checks.cFree3 + @"  
                    and cfree4=" + checks.cFree4 + " and cfree5=" + checks.cFree5 + " and cfree6=" + checks.cFree6 + " and cfree7=" + checks.cFree7 + @" 
                    and cfree8=" + checks.cFree8 + " and cfree9=" + checks.cFree9 + " and cfree10=" + checks.cFree10, Cmd); //帐目数
            checks.iCVQuantity = pos_qty == "" ? "0" : pos_qty; //帐目数
            #region//货位标识处理  ，若无货位取存货档案的默认货位
            if (b_Pos)
            {
                string cbodypos = GetBodyValue_FromData(BodyData, i, "cposcode");
                if (cbodypos.CompareTo("") == 0) cbodypos = w_cheadpos;
                if (cbodypos == "")
                {
                    cbodypos = U8Operation.GetDataString("select cPosition from " + dbname + "..Inventory where cinvcode=" + checks.cInvCode, Cmd);
                    if (cbodypos == "") throw new Exception("本仓库有货位管理，" + checks.cInvCode + "的货位不能为空");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode=" + check.cWhCode + " and cposcode='" + cbodypos + "'", Cmd) == 0)
                    throw new Exception("货位编码【" + cbodypos + "】不存在，或者不属于本仓库");
                checks.cPosition = "'" + cbodypos + "'";
                //获得货位的账面数
                pos_qty = "" + U8Operation.GetDataString("select isnull(max(iquantity),0) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + check.cWhCode + @" 
                    and cvmivencode=" + checks.cvmivencode + " and cinvcode=" + checks.cInvCode + " and cPosCode=" + checks.cPosition + @" 
                    and cbatch=" + checks.cCVBatch + " and cfree1=" + checks.cFree1 + " and cfree2=" + checks.cFree2 + " and cfree3=" + checks.cFree3 + @"  
                    and cfree4=" + checks.cFree4 + " and cfree5=" + checks.cFree5 + " and cfree6=" + checks.cFree6 + " and cfree7=" + checks.cFree7 + @" 
                    and cfree8=" + checks.cFree8 + " and cfree9=" + checks.cFree9 + " and cfree10=" + checks.cFree10, Cmd); //帐目数
                checks.iCVQuantity = pos_qty == "" ? "0" : pos_qty;
            }
            #endregion

            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..CheckVouch a inner join " + dbname + @"..CheckVouchs b on a.id=b.id 
                where b.cinvcode=" + checks.cInvCode + " and isnull(b.ccvbatch,'')=" + checks.cCVBatch + " and isnull(b.cPosition,'')=" + checks.cPosition + @" 
                    and a.cwhcode=" + check.cWhCode + " and isnull(b.cvmivencode,'')=" + checks.cvmivencode + " and isnull(b.cFree1,'')=" + checks.cFree1 + @" 
                    and isnull(b.cFree2,'')=" + checks.cFree2 + " and isnull(b.cFree3,'')=" + checks.cFree3 + " and isnull(b.cFree4,'')=" + checks.cFree4 + @" 
                    and isnull(b.cFree5,'')=" + checks.cFree5 + " and isnull(b.cFree6,'')=" + checks.cFree6 + " and isnull(b.cFree7,'')=" + checks.cFree7 + @" 
                    and isnull(b.cFree8,'')=" + checks.cFree8 + " and isnull(b.cFree9,'')=" + checks.cFree9 + " and isnull(b.cFree10,'')=" + checks.cFree10 + @" 
                    and isnull(a.caccounter,'')=''", Cmd) > 0)
            {
                throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]货位[" + checks.cPosition.Replace("'", "") + "]批号[" + checks.cCVBatch.Replace("'", "") + "]存在未审核的盘点单");
            }

            if (!checks.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

        }

        #endregion
        #endregion

        return rd_id + "," + cc_mcode;
    }

    //领料申请出库(直接扫描模式 后台自动匹配)
    private string U81062(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        //申请单号校验
        string c_shen_code = GetTextsFrom_FormData_Tag(HeadData, "txt_mavouchcode");
        if (c_shen_code == "") throw new Exception("领料申请出库，必须扫描申请单条码");
        DataTable dtShenQing = U8Operation.GetSqlDataTable(@"select id,cCode,cRdCode,cDepCode,cPersonCode,cSource from " + dbname + @"..MaterialAppVouch 
            where csysbarcode='" + c_shen_code + "' or cCode='" + c_shen_code + "'", "dtShenQing", Cmd);
        if (dtShenQing.Rows.Count == 0) throw new Exception("无法找到申请单");

        #region //继承申请单的 表头栏目
        if (dtShenQing.Rows[0]["cRdCode"] + "" != "")
        {
            string[] c_data=GetTextsFrom_FormData(HeadData, "txt_crdcode");
            if (c_data==null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "出库类别"; //栏目标题 
                dr["TxtName"] = "txt_crdcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cRdCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cRdCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_crdcode", dtShenQing.Rows[0]["cRdCode"] + "", dtShenQing.Rows[0]["cRdCode"] + "");

            }
        }

        if (dtShenQing.Rows[0]["cDepCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cdepcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "部门"; //栏目标题 
                dr["TxtName"] = "txt_cdepcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cDepCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cDepCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cdepcode", dtShenQing.Rows[0]["cDepCode"] + "", dtShenQing.Rows[0]["cDepCode"] + "");

            }
        }

        if (dtShenQing.Rows[0]["cPersonCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cpersoncode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "业务员"; //栏目标题 
                dr["TxtName"] = "txt_cpersoncode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cPersonCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cPersonCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cpersoncode", dtShenQing.Rows[0]["cPersonCode"] + "", dtShenQing.Rows[0]["cPersonCode"] + "");
            }
        }

        //业务类型
        string cc_bustype = "领料";//库存 直接出库和 生产订单出库 都是 领料
        if ("" + dtShenQing.Rows[0]["cSource"] == "委外订单") cc_bustype = "委外发料";

        string[] d_data = GetTextsFrom_FormData(HeadData, "txt_cbustype");
        if (d_data == null)
        {
            DataRow dr = HeadData.NewRow();
            dr["LabelText"] = "业务类型"; //栏目标题 
            dr["TxtName"] = "txt_cbustype";   //txt_字段名称
            dr["TxtTag"] = cc_bustype;  //标识
            dr["TxtValue"] = cc_bustype;//栏目值
            HeadData.Rows.Add(dr);
        }
        else
        {
            if (d_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cbustype", cc_bustype, cc_bustype);
        }

        #endregion

        #region //表体栏目转换 (按照顺序分解)
        //BodyData 排序  存货编码
        BodyData.DefaultView.Sort = "cinvcode ASC";
        BodyData = BodyData.DefaultView.ToTable();

        //按照顺序匹配
        DataTable dtShData = null;
        string c_cinvcode = "";
        int i_app_row = 0;
        decimal d_app_row_num = 0;
        BodyData.Columns["sq_autoid"].ReadOnly = false;
        BodyData.Columns["allocateid"].ReadOnly = false;
        BodyData.Columns["modid"].ReadOnly = false;
        DataTable dtChaiData = BodyData.Clone();  //记录拆下来的数据

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            if (BodyData.Rows[i]["sq_autoid"] + "" != "") break;  //根据申请单行条码扫码，无需拆行业务处理
            if (c_cinvcode != "" + BodyData.Rows[i]["cinvcode"])
            {
                i_app_row = 0;
                c_cinvcode = "" + BodyData.Rows[i]["cinvcode"];
                dtShData = U8Operation.GetSqlDataTable(@"select cinvcode,AutoID,iOMoMID,iOMoDID,iMPoIds,b.MoDId,iordertype,iQuantity-ISNULL(fOutQuantity,0) ilastqty 
                    from " + dbname + "..MaterialAppVouchs a left join " + dbname + @"..mom_moallocate b on a.iMPoIds=b.AllocateId
                    where a.ID=0" + dtShenQing.Rows[0]["id"] + " and a.cinvcode='" + c_cinvcode + "' and iQuantity-ISNULL(fOutQuantity,0)>0 order by iopseq","dtShData", Cmd);
                if (dtShData.Rows.Count == 0) throw new Exception("本申请单 存货【" + c_cinvcode + "】没有可出库信息");

                //d_app_row_num = decimal.Parse("" + dtShData.Rows[i_app_row]["ilastqty"]);
            }
            decimal dvalue = decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
            //循环比较 
            for (int k = i_app_row; k < dtShData.Rows.Count; k++)
            {
                i_app_row = k;
                d_app_row_num = decimal.Parse("" + dtShData.Rows[i_app_row]["ilastqty"]);

                if (dvalue <= d_app_row_num)
                {
                    d_app_row_num = d_app_row_num - dvalue;
                    BodyData.Rows[i]["sq_autoid"] = "" + dtShData.Rows[i_app_row]["AutoID"];  //申请单ID
                    if ("" + dtShenQing.Rows[0]["cSource"] == "委外订单")  //委外订单
                    {
                        BodyData.Rows[i]["allocateid"] = "" + dtShData.Rows[i_app_row]["iOMoMID"];
                        BodyData.Rows[i]["modid"] = "" + dtShData.Rows[i_app_row]["iOMoDID"];
                    }
                    else
                    {
                        BodyData.Rows[i]["allocateid"] = "" + dtShData.Rows[i_app_row]["iMPoIds"];
                        BodyData.Rows[i]["modid"] = "" + dtShData.Rows[i_app_row]["MoDId"];
                    }

                    if (d_app_row_num == 0)
                    {
                        i_app_row = k + 1;
                    }
                    else
                    {
                        dtShData.Rows[i_app_row]["ilastqty"] = d_app_row_num;  //修改数量
                    }
                    dvalue = 0;
                    break;  //退出循环
                }
                else
                {
                    //拆行
                    DataRow c_DR = dtChaiData.Rows.Add(BodyData.Rows[i].ItemArray);
                    c_DR["iquantity"] = d_app_row_num;
                    c_DR["sq_autoid"] = dtShData.Rows[i_app_row]["AutoID"];  //申请单ID
                    if ("" + dtShenQing.Rows[0]["cSource"] == "委外订单") //委外订单
                    {
                        c_DR["allocateid"] = "" + dtShData.Rows[i_app_row]["iOMoMID"];
                        c_DR["modid"] = "" + dtShData.Rows[i_app_row]["iOMoDID"];
                    }
                    else
                    {
                        c_DR["allocateid"] = "" + dtShData.Rows[i_app_row]["iMPoIds"];
                        c_DR["modid"] = "" + dtShData.Rows[i_app_row]["MoDId"];
                    }
                    dvalue = dvalue - d_app_row_num;
                    BodyData.Rows[i]["iquantity"] = dvalue;
                }
            }
            //未拆分完毕，有剩余
            if (dvalue > 0) throw new Exception("存货[" + c_cinvcode + "]超出申请单可出库量");
        }

        //合并行拆分行
        for (int k = 0; k < dtChaiData.Rows.Count; k++)
        { BodyData.Rows.Add(dtChaiData.Rows[k].ItemArray); }

        #endregion

        //推送完成出库
        string crd11code = U81016(HeadData, BodyData, dbname, cUserName, cLogDate, "U81016", Cmd);
        if (crd11code == "") throw new Exception("出库失败,未能获得反馈的出库单据号");

        #region  //累计数校验
        //回写申请单累计 出库数
//        for (int i = 0; i < BodyData.Rows.Count; i++)
//        {
//            Cmd.CommandText = "update " + dbname + "..MaterialAppVouchs set fOutQuantity=ISNULL(fOutQuantity,0)+(0" + BodyData.Rows[i]["iquantity"] + @") 
//                where autoid=0" + BodyData.Rows[i]["sq_autoid"];
//            Cmd.ExecuteNonQuery();

//            //判断是否超申请出库
//            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..MaterialAppVouchs where autoid=0" + BodyData.Rows[i]["sq_autoid"] + " and ISNULL(fOutQuantity,0)>iQuantity", Cmd) > 0)
//                throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超申请单出库");

//            //回写生产订单子件表 累计申请出库数
//            if ("" + dtShenQing.Rows[0]["cSource"] == "生产订单")
//            {
//                Cmd.CommandText = "update " + dbname + "..mom_moallocate set RequisitionIssQty=ISNULL(RequisitionIssQty,0)+(0" + BodyData.Rows[i]["iquantity"] + @") 
//                    where AllocateId=0" + BodyData.Rows[i]["allocateid"];
//                Cmd.ExecuteNonQuery();

//                //判断是否超生产订单出库
//                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..mom_moallocate 
//                    where AllocateId=0" + BodyData.Rows[i]["allocateid"] + " and ISNULL(RequisitionIssQty,0)>isnull(RequisitionQty,0)", Cmd) > 0)
//                    throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超生产订单申请累计出库量");
//            }

//            //回写委外订单子件表 累计申请出库数
//            if ("" + dtShenQing.Rows[0]["cSource"] == "委外订单")
//            {
//                Cmd.CommandText = "update " + dbname + "..OM_MOMaterials set fsendapplyqty=ISNULL(fsendapplyqty,0)+(0" + BodyData.Rows[i]["iquantity"] + @") 
//                    where MOMaterialsID=0" + BodyData.Rows[i]["allocateid"];
//                Cmd.ExecuteNonQuery();

//                //判断是否超生产订单出库
//                if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..OM_MOMaterials 
//                    where MOMaterialsID=0" + BodyData.Rows[i]["allocateid"] + " and ISNULL(fsendapplyqty,0)>isnull(fapplyqty,0)", Cmd) > 0)
//                    throw new Exception("存货[" + BodyData.Rows[i]["cinvcode"] + "]超委外订单申请累计出库量");
//            }
//        }
        #endregion

        return crd11code;

    }

    //销售发货：直接扫码，后台自动匹配销售订单（客户+销售类型）
    private string U81063(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        BodyData.Columns.Add("isosid");  //增加销售订单子表 标识列
        BodyData.Columns.Add("soid"); 
        #region  //校验数据
        string c_cuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuscode");
        if (c_cuscode == "") throw new Exception("客户必须选择");
        string c_stcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cstcode");
        if (c_stcode == "") throw new Exception("销售类型必须选择");

        //headrowbarcode
        if (!IsExistHeadCol(HeadData, "txt_headrowbarcode"))
        {
            DataRow dr = HeadData.NewRow();
            dr["LabelText"] = "订单行条码"; //栏目标题 
            dr["TxtName"] = "txt_headrowbarcode";   //txt_字段名称
            dr["TxtTag"] = "0";  //标识
            dr["TxtValue"] = "0";//栏目值
            HeadData.Rows.Add(dr);
        }

        #endregion 

        //获得 相同客户 相同销售类型  非关闭的 未执行完毕的销售订单
        #region
        DataTable dtSODetail = null;
        string c_cinvcode = "";
        int i_app_row = 0;
        decimal d_app_row_num = 0;


        BodyData.Columns["isosid"].ReadOnly = false;
        BodyData.Columns["soid"].ReadOnly = false;
        DataTable dtChaiData = BodyData.Clone();  //记录拆下来的数据
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            if (c_cinvcode != "" + BodyData.Rows[i]["cinvcode"])
            {
                i_app_row = 0;
                c_cinvcode = "" + BodyData.Rows[i]["cinvcode"];
                dtSODetail = U8Operation.GetSqlDataTable(@"select b.iSOsID,a.id,iQuantity-isnull(iFHQuantity,0) ilastqty
                from " + dbname + "..SO_SOMain a inner join " + dbname + @"..SO_SODetails b on a.ID=b.ID
                where a.cCusCode='" + c_cuscode + "' and a.cSTCode='" + c_stcode + "' and b.cInvCode='" + c_cinvcode + @"' and iQuantity-isnull(iFHQuantity,0)>0 and isnull(cSCloser,'')=''
                order by b.dPreDate,b.ID,b.AutoID", "dtSODetail", Cmd);
                if (dtSODetail.Rows.Count == 0) throw new Exception("存货【" + c_cinvcode + "】没有可发货的订单信息");
            }

            decimal dvalue = decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
            //循环比较 
            for (int k = i_app_row; k < dtSODetail.Rows.Count; k++)
            {
                i_app_row = k;
                d_app_row_num = decimal.Parse("" + dtSODetail.Rows[i_app_row]["ilastqty"]);

                if (dvalue <= d_app_row_num)
                {
                    d_app_row_num = d_app_row_num - dvalue;
                    BodyData.Rows[i]["isosid"] = "" + dtSODetail.Rows[i_app_row]["iSOsID"];
                    BodyData.Rows[i]["soid"] = "" + dtSODetail.Rows[i_app_row]["id"];

                    if (d_app_row_num == 0)
                    {
                        i_app_row = k + 1;
                    }
                    else
                    {
                        dtSODetail.Rows[i_app_row]["ilastqty"] = d_app_row_num;  //修改数量
                    }
                    dvalue = 0;
                    break;  //退出循环
                }
                else
                {
                    //拆行
                    DataRow c_DR = dtChaiData.Rows.Add(BodyData.Rows[i].ItemArray);
                    c_DR["iquantity"] = d_app_row_num;
                    c_DR["isosid"] = dtSODetail.Rows[i_app_row]["iSOsID"];  
                    c_DR["soid"] = "" + dtSODetail.Rows[i_app_row]["id"]; 

                    dvalue = dvalue - d_app_row_num;
                    BodyData.Rows[i]["iquantity"] = dvalue;
                }
            }

            //未拆分完毕，有剩余
            if (dvalue > 0) throw new Exception("存货[" + c_cinvcode + "]超可发货订单量");
        }

        //合并行拆分行
        for (int k = 0; k < dtChaiData.Rows.Count; k++)
        { BodyData.Rows.Add(dtChaiData.Rows[k].ItemArray); }


        #endregion

        //推送完成出库
        string RetCode = U81020(HeadData, BodyData, dbname, cUserName, cLogDate, "U81020", Cmd);
        if (RetCode == "") throw new Exception("出库失败,未能获得反馈的发货单据号");

        return RetCode;

    }

    //销售调拨：直接扫码，后台自动匹配销售订单（客户+销售类型）,调拨申请调拨
    private string U81064(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        BodyData.Columns.Add("isosid");  //增加销售订单子表 标识列
        BodyData.Columns.Add("soid");
        #region  //校验数据
        string c_cuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuscode");
        if (c_cuscode == "") throw new Exception("客户必须选择");
        string c_stcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cstcode");
        if (c_stcode == "") throw new Exception("销售类型必须选择");

        //headrowbarcode
        if (!IsExistHeadCol(HeadData, "txt_headrowbarcode"))
        {
            DataRow dr = HeadData.NewRow();
            dr["LabelText"] = "订单行条码"; //栏目标题 
            dr["TxtName"] = "txt_headrowbarcode";   //txt_字段名称
            dr["TxtTag"] = "0";  //标识
            dr["TxtValue"] = "0";//栏目值
            HeadData.Rows.Add(dr);
        }
        string app_vou_code = GetTextsFrom_FormData_Tag(HeadData, "txt_h_apptvcode");
        if (app_vou_code.CompareTo("") != 0)  //存在调拨申请单
        {
            DataTable dtAppVouch = U8Operation.GetSqlDataTable("select cIWhCode,cOWhCode,cIRdCode,cORdCode from " + dbname + @"..ST_AppTransVouch a 
                where cTVCode='" + app_vou_code + "'", "dtAppVouch", Cmd);
            if (dtAppVouch.Rows.Count == 0) throw new Exception("调拨申请单[" + app_vou_code + "]不存在");
            if ((!IsExistHeadCol(HeadData, "txt_cinwhcode")) && "" + dtAppVouch.Rows[0]["cIWhCode"] != "")
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "调入仓库"; //栏目标题 
                dr["TxtName"] = "txt_cinwhcode";   //txt_字段名称
                dr["TxtTag"] = "" + dtAppVouch.Rows[0]["cIWhCode"];  //标识
                dr["TxtValue"] = "" + dtAppVouch.Rows[0]["cIWhCode"]; ;//栏目值
                HeadData.Rows.Add(dr);
            }
            if ((!IsExistHeadCol(HeadData, "txt_cwhcode")) && "" + dtAppVouch.Rows[0]["cOWhCode"] != "")
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "调出仓库"; //栏目标题 
                dr["TxtName"] = "txt_cwhcode";   //txt_字段名称
                dr["TxtTag"] = "" + dtAppVouch.Rows[0]["cOWhCode"];  //标识
                dr["TxtValue"] = "" + dtAppVouch.Rows[0]["cOWhCode"];//栏目值
                HeadData.Rows.Add(dr);
            }
            if ((!IsExistHeadCol(HeadData, "txt_cinrdcode")) && "" + dtAppVouch.Rows[0]["cIRdCode"] != "")
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "入库类别"; //栏目标题 
                dr["TxtName"] = "txt_cinrdcode";   //txt_字段名称
                dr["TxtTag"] = "" + dtAppVouch.Rows[0]["cIRdCode"];  //标识
                dr["TxtValue"] = "" + dtAppVouch.Rows[0]["cIRdCode"];//栏目值
                HeadData.Rows.Add(dr);
            }
            if ((!IsExistHeadCol(HeadData, "txt_crdcode")) && "" + dtAppVouch.Rows[0]["cORdCode"] != "")
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "出库类别"; //栏目标题 
                dr["TxtName"] = "txt_crdcode";   //txt_字段名称
                dr["TxtTag"] = "" + dtAppVouch.Rows[0]["cORdCode"];  //标识
                dr["TxtValue"] = "" + dtAppVouch.Rows[0]["cORdCode"];//栏目值
                HeadData.Rows.Add(dr);
            }
        }
        #endregion

        //获得 相同客户 相同销售类型  非关闭的 未执行完毕的销售订单
        #region
        DataTable dtSODetail = null;
        string c_cinvcode = "";
        int i_app_row = 0;
        decimal d_app_row_num = 0;
        
        //是否回写 销售调拨单的累计数
        string db_is_updateout_qty = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_db_is_updatesodetail_out_qty'", Cmd);
        //累计调拨数栏目
        string u8barcode_db_so_lj_qty = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_db_so_lj_qty'", Cmd);
        if (u8barcode_db_so_lj_qty == "") u8barcode_db_so_lj_qty = "ifhquantity";
        if (db_is_updateout_qty.ToLower().CompareTo("true") != 0) u8barcode_db_so_lj_qty = "ifhquantity";

        BodyData.Columns["isosid"].ReadOnly = false;
        BodyData.Columns["soid"].ReadOnly = false;
        DataTable dtChaiData = BodyData.Clone();  //记录拆下来的数据
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            if (c_cinvcode != "" + BodyData.Rows[i]["cinvcode"])
            {
                i_app_row = 0;
                c_cinvcode = "" + BodyData.Rows[i]["cinvcode"];
                dtSODetail = U8Operation.GetSqlDataTable(@"select b.iSOsID,a.id,iQuantity-isnull(" + u8barcode_db_so_lj_qty + @",0) ilastqty
                from " + dbname + "..SO_SOMain a inner join " + dbname + @"..SO_SODetails b on a.ID=b.ID
                where a.cCusCode='" + c_cuscode + "' and a.cSTCode='" + c_stcode + "' and b.cInvCode='" + c_cinvcode + @"' and isnull(a.cVerifier,'')<>'' 
                    and iQuantity-isnull(" + u8barcode_db_so_lj_qty + @",0)>0 and isnull(cSCloser,'')=''
                order by b.dPreDate,b.ID,b.AutoID", "dtSODetail", Cmd);
                if (dtSODetail.Rows.Count == 0) throw new Exception("存货【" + c_cinvcode + "】没有可发货的已审核的订单信息");
            }

            decimal dvalue = decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
            //循环比较 
            for (int k = i_app_row; k < dtSODetail.Rows.Count; k++)
            {
                i_app_row = k;
                d_app_row_num = decimal.Parse("" + dtSODetail.Rows[i_app_row]["ilastqty"]);

                if (dvalue <= d_app_row_num)
                {
                    d_app_row_num = d_app_row_num - dvalue;
                    BodyData.Rows[i]["isosid"] = "" + dtSODetail.Rows[i_app_row]["iSOsID"];
                    BodyData.Rows[i]["soid"] = "" + dtSODetail.Rows[i_app_row]["id"];
                    
                    if (d_app_row_num == 0)
                    {
                        i_app_row = k + 1;
                    }
                    else
                    {
                        dtSODetail.Rows[i_app_row]["ilastqty"] = d_app_row_num;  //修改数量
                    }
                    dvalue = 0;
                    break;  //退出循环
                }
                else
                {
                    //拆行
                    DataRow c_DR = dtChaiData.Rows.Add(BodyData.Rows[i].ItemArray);
                    c_DR["iquantity"] = d_app_row_num;
                    c_DR["isosid"] = dtSODetail.Rows[i_app_row]["iSOsID"];
                    c_DR["soid"] = "" + dtSODetail.Rows[i_app_row]["id"];

                    dvalue = dvalue - d_app_row_num;
                    BodyData.Rows[i]["iquantity"] = dvalue;
                }
            }

            //未拆分完毕，有剩余
            if (dvalue > 0) throw new Exception("存货[" + c_cinvcode + "]超可发货订单量");
        }

        //合并行拆分行
        for (int k = 0; k < dtChaiData.Rows.Count; k++)
        { BodyData.Rows.Add(dtChaiData.Rows[k].ItemArray); }

        #endregion

        //推送完成出库
        string RetCode = U81017(HeadData, BodyData, dbname, cUserName, cLogDate, "U81017", Cmd);
        if (RetCode == "") throw new Exception("出库失败,未能获得反馈的调拨单据号");

        return RetCode;
    }

    //货位调整单
    public string U81087(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = "";
        bool b_Vmi = false; 
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值

        #region  //逻辑检验
        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("仓库输入错误");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) == 0)
            throw new Exception("本仓库无货位管理");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='0313' and VT_TemplateMode=0", Cmd);
        
        #region //主表
        KK_U8Com.U8AdjustPVouch just = new KK_U8Com.U8AdjustPVouch(Cmd, dbname);
        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='ad'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='ad'";
        Cmd.ExecuteNonQuery();
        string cCodeHead = "J" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd);
        string cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cVouchCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..AdjustPVouch where cVouchCode like '" + cCodeHead + "%'", Cmd);
        just.Id = rd_id+"";
        just.cVouchCode = "'" + cc_mcode + "'";
        just.cWhCode = "'" + w_cwhcode + "'";
        just.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        just.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        just.cMemo = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cmemo") + "'";
        just.VT_ID = vt_id + "";
        just.csource = "'1'";
        just.cMaker = "'" + cUserName + "'";
        just.dDate = "'" + cLogDate + "'";
        just.dnmaketime = "getdate()";

        #region   //主表自定义项处理
        just.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        just.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        just.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        just.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        just.cDefine5 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        just.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        just.cDefine7 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        just.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        just.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        just.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        just.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        just.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        just.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        just.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        just.cDefine15 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        just.cDefine16 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        #endregion

        if (!just.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
        Cmd.CommandText = "update " + dbname + "..AdjustPVouch set  csysbarcode='||st19|" + cc_mcode + "' where id=" + rd_id;
        Cmd.ExecuteNonQuery();

        #endregion 

        #region  //子表
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cvmivencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("调整数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");
            if ("" + BodyData.Rows[i]["cbposcode"] == "") throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码的调整前货位不能为空");
            if ("" + BodyData.Rows[i]["caposcode"] == "") throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码的调整后货位不能为空");
            if ("" + BodyData.Rows[i]["caposcode"] == "" + BodyData.Rows[i]["cbposcode"]) throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码的调整前与后的货位不能相同");

            //代管验证
            if (b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            KK_U8Com.U8AdjustPVouchs justs = new KK_U8Com.U8AdjustPVouchs(Cmd, dbname);
            int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='ad'", Cmd);
            justs.autoID = cAutoid+"";
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='ad'";
            Cmd.ExecuteNonQuery();

            justs.ID = just.Id;
            justs.autoID = cAutoid+"";
            justs.cVouchCode = just.cVouchCode;
            justs.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            justs.irowno = (i + 1) + "";
            justs.cbMemo = "'" + GetBodyValue_FromData(BodyData, i, "cbmemo") + "'";
            string cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory where cinvcode=" + justs.cInvCode + "", Cmd);
            justs.cBPosCode = "'" + BodyData.Rows[i]["cbposcode"] + "'";
            justs.cAPosCode = "'" + BodyData.Rows[i]["caposcode"] + "'";
            justs.iQuantity = "" + BodyData.Rows[i]["iquantity"];

            #region //自由项  自定义项处理
            justs.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            justs.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            justs.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            justs.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            justs.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            justs.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            justs.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            justs.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            justs.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            justs.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            //自定义项
            justs.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            justs.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            justs.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            justs.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            justs.cDefine26 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            justs.cDefine27 = (txtdata_text == "" ? "null" : txtdata_text);
            justs.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            justs.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            justs.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            justs.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            justs.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            justs.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            justs.cDefine34 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            justs.cDefine35 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            justs.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            justs.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            //自由项检验
            for (int f = 1; f < 11; f++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + justs.cInvCode + " and bFree" + f + "=1", Cmd) > 0)
                {
                    if (GetBodyValue_FromData(BodyData, i, "cfree" + f) == "")
                        throw new Exception(justs.cInvCode + "有自由项" + f + "管理，必须录入");
                }
            }

            #endregion

            #region //代管挂账处理
            justs.cvmivencode = "''";
            if (b_Vmi)
            {
                justs.cvmivencode = "'" + cvmivencode + "'";
            }
            #endregion

            #region //批次管理和保质期管理
            justs.cBatch = "''";
            justs.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + justs.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(justs.cInvCode + "有批次管理，必须输入批号");
                justs.cBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";
                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + justs.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    if ((!BodyData.Columns.Contains("dmadedate")) || BodyData.Rows[i]["dmadedate"] + "" == "")  //生产日期判定
                        throw new Exception(justs.cInvCode + "有保质期管理，必须输入生产日期");
                    string rowpordate = "" + BodyData.Rows[i]["dmadedate"];
                    if (rowpordate == "") throw new Exception(justs.cInvCode + "有保质期管理，但生产日期为空");
                    DataTable dtBZQ = U8Operation.GetSqlDataTable(@"select iMassDate 保质期天数,cMassUnit 保质期单位,dmadedate 生产日期,
                            convert(varchar(10),dVDate,120) 失效日期,isnull(iExpiratDateCalcu,1) 有效期推算方式
                        from " + dbname + @"..InvPositionSum(nolock)
                        where cinvcode=" + justs.cInvCode + " and cBatch=" + justs.cBatch + " and cvmivencode=" + justs.cvmivencode + " and cFree1=" + justs.cFree1 +
                            " and cFree2=" + justs.cFree2 + " and cFree3=" + justs.cFree3 + " and cFree4=" + justs.cFree4 + " and cFree5=" + justs.cFree5 + " and cFree6=" + justs.cFree6 + 
                            " and cFree7=" + justs.cFree7 + " and cFree8=" + justs.cFree8 + " and cFree9=" + justs.cFree9 + " and cFree10=" + justs.cFree10 + "", "dtBZQ", Cmd);
                    if (dtBZQ.Rows.Count == 0) throw new Exception("没有找到" + justs.cInvCode + "调出货位的保质期信息");

                    justs.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    justs.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    justs.dDisDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    justs.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    justs.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    justs.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }
            }
            #endregion

            #region //固定换算率（多计量）
            justs.iNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                string inum = "0";
                justs.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + justs.cAssUnit, Cmd);

                if (ichange == "") ichange = "0";
                if (float.Parse(ichange) == 0)
                {
                    //浮动换算率
                    inum = GetBodyValue_FromData(BodyData, i, "inum");
                    if (inum == "")
                    {
                        inum = "0";
                    }
                    else
                    {
                        //浮动换算率
                        ichange = U8Operation.GetDataString("select round(" + justs.iQuantity + "/" + inum + ",5)", Cmd);
                    }
                    justs.iNum = inum;
                }
                else
                {
                    //固定换算率
                    inum = U8Operation.GetDataString("select round(" + justs.iQuantity + "/" + ichange + ",5)", Cmd);
                    justs.iNum = inum;
                }

            }
            #endregion

            if (!justs.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

            #region //补充信息
            //批次属性
            DataTable dtBatPro = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,
                        cBatchProperty6,cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10
                    from " + dbname + @"..AA_BatchProperty a 
                    where a.cinvcode=" + justs.cInvCode + " and a.cBatch=" + justs.cBatch + " and a.cFree1=" + justs.cFree1 + " and a.cFree2=" + justs.cFree2 + @" 
                    and a.cFree3=" + justs.cFree3 + " and a.cFree4=" + justs.cFree4 + " and a.cFree5=" + justs.cFree5 + " and a.cFree6=" + justs.cFree6 + @" 
                    and a.cFree7=" + justs.cFree7 + " and a.cFree8=" + justs.cFree8 + " and a.cFree9=" + justs.cFree9 + " and a.cFree10=" + justs.cFree10, "dtBatPro", Cmd);
            if (dtBatPro.Rows.Count > 0)
            {
                Cmd.CommandText = "update " + dbname + @"..AdjustPVouchs set cBatchProperty1=" + (dtBatPro.Rows[0]["cBatchProperty1"] + "" == "" ? "null" : dtBatPro.Rows[0]["cBatchProperty1"] + "") + @",
                    cBatchProperty2=" + (dtBatPro.Rows[0]["cBatchProperty2"] + "" == "" ? "null" : dtBatPro.Rows[0]["cBatchProperty2"] + "") + @",
                    cBatchProperty3=" + (dtBatPro.Rows[0]["cBatchProperty3"] + "" == "" ? "null" : dtBatPro.Rows[0]["cBatchProperty3"] + "") + @",
                    cBatchProperty4=" + (dtBatPro.Rows[0]["cBatchProperty4"] + "" == "" ? "null" : dtBatPro.Rows[0]["cBatchProperty4"] + "") + @",
                    cBatchProperty5=" + (dtBatPro.Rows[0]["cBatchProperty5"] + "" == "" ? "null" : dtBatPro.Rows[0]["cBatchProperty5"] + "") + @",
                    cBatchProperty6='" + dtBatPro.Rows[0]["cBatchProperty6"] + "',cBatchProperty7='" + dtBatPro.Rows[0]["cBatchProperty7"] + @"',
                    cBatchProperty8='" + dtBatPro.Rows[0]["cBatchProperty8"] + "',cBatchProperty9='" + dtBatPro.Rows[0]["cBatchProperty9"] + @"',
                    cBatchProperty10=" + (dtBatPro.Rows[0]["cBatchProperty10"] + "" == "" ? "null" : "'" + dtBatPro.Rows[0]["cBatchProperty10"] + "'") + @"
                where autoID=" + justs.autoID;
                Cmd.ExecuteNonQuery();
            }
            //条码
            Cmd.CommandText = "update " + dbname + "..AdjustPVouchs set cbsysbarcode='||st19|" + cc_mcode + "|" + justs.irowno + "' where autoID=" + justs.autoID;
            Cmd.ExecuteNonQuery();
            #endregion

            #region //调出货位账务处理
            //添加货位记录 
            Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                "Values (" + cAutoid + "," + rd_id + "," + just.cWhCode + "," + justs.cBPosCode + "," + justs.cInvCode + "," + justs.iQuantity + "," + justs.iNum +
                ",null," + just.dDate + ",0,'货位调整',0,'19'," + just.dDate + "," + just.cMaker + "," + justs.cvmivencode + "," + justs.cBatch +
                "," + justs.cFree1 + "," + justs.cFree2 + "," + justs.cFree3 + "," + justs.cFree4 + "," + justs.cFree5 + "," +
                justs.cFree6 + "," + justs.cFree7 + "," + justs.cFree8 + "," + justs.cFree9 + "," + justs.cFree10 + ",''," + justs.cAssUnit + @",
                        " + justs.dMadeDate + "," + justs.iMassDate + "," + justs.cMassUnit + "," + justs.iExpiratDateCalcu + "," + justs.cExpirationdate + "," + justs.dExpirationdate + ")";
            Cmd.ExecuteNonQuery();

            //修改货位库存
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + just.cWhCode + " and cvmivencode=" + justs.cvmivencode + " and cinvcode=" + justs.cInvCode + @" 
                    and cPosCode=" + justs.cBPosCode + " and cbatch=" + justs.cBatch + " and cfree1=" + justs.cFree1 + " and cfree2=" + justs.cFree2 + " and cfree3=" + justs.cFree3 + @" 
                    and cfree4=" + justs.cFree4 + " and cfree5=" + justs.cFree5 + " and cfree6=" + justs.cFree6 + " and cfree7=" + justs.cFree7 + @" 
                    and cfree8=" + justs.cFree8 + " and cfree9=" + justs.cFree9 + " and cfree10=" + justs.cFree10, Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                        values(" + just.cWhCode + "," + justs.cBPosCode + "," + justs.cInvCode + ",0," + justs.cBatch + @",
                            " + justs.cFree1 + "," + justs.cFree2 + "," + justs.cFree3 + "," + justs.cFree4 + "," + justs.cFree5 + "," +
                        justs.cFree6 + "," + justs.cFree7 + "," + justs.cFree8 + "," + justs.cFree9 + "," + justs.cFree10 + "," + justs.cvmivencode + @",'',0,
                            " + justs.dMadeDate + "," + justs.iMassDate + "," + justs.cMassUnit + "," + justs.iExpiratDateCalcu + "," + justs.cExpirationdate + "," + justs.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();
            }
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)-(" + justs.iQuantity + "),inum=isnull(inum,0)-(" + justs.iNum + @") 
                        where cwhcode=" + just.cWhCode + " and cvmivencode=" + justs.cvmivencode + " and cinvcode=" + justs.cInvCode + @" 
                        and cPosCode=" + justs.cBPosCode + " and cbatch=" + justs.cBatch + " and cfree1=" + justs.cFree1 + " and cfree2=" + justs.cFree2 + " and cfree3=" + justs.cFree3 + @" 
                        and cfree4=" + justs.cFree4 + " and cfree5=" + justs.cFree5 + " and cfree6=" + justs.cFree6 + " and cfree7=" + justs.cFree7 + @" 
                        and cfree8=" + justs.cFree8 + " and cfree9=" + justs.cFree9 + " and cfree10=" + justs.cFree10;
            Cmd.ExecuteNonQuery();

            //判断货位账务 是否小于0
            //货位现存量
            Current_Pos_StockCHeck(Cmd, dbname, just.cWhCode, justs.cBPosCode, justs.cInvCode, justs.cBatch, justs.cvmivencode, justs.cFree1, justs.cFree2,
                justs.cFree3, justs.cFree4, justs.cFree5, justs.cFree6, justs.cFree7, justs.cFree8, justs.cFree9, justs.cFree10);

            #endregion

            #region //调入货位账务处理
            //添加货位记录 
            Cmd.CommandText = "Insert Into " + dbname + @"..InvPosition(rdsid,rdid,cwhcode,cposcode,cinvcode,iquantity,inum,
                            cmemo,ddate,brdflag,csource,itrackid,cvouchtype,dvouchDate,cHandler,cvmivencode,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cinvouchtype,cAssUnit,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) " +
                "Values (" + cAutoid + "," + rd_id + "," + just.cWhCode + "," + justs.cAPosCode + "," + justs.cInvCode + "," + justs.iQuantity + "," + justs.iNum +
                ",null," + just.dDate + ",1,'货位调整',0,'19'," + just.dDate + "," + just.cMaker + "," + justs.cvmivencode + "," + justs.cBatch +
                "," + justs.cFree1 + "," + justs.cFree2 + "," + justs.cFree3 + "," + justs.cFree4 + "," + justs.cFree5 + "," +
                justs.cFree6 + "," + justs.cFree7 + "," + justs.cFree8 + "," + justs.cFree9 + "," + justs.cFree10 + ",''," + justs.cAssUnit + @",
                        " + justs.dMadeDate + "," + justs.iMassDate + "," + justs.cMassUnit + "," + justs.iExpiratDateCalcu + "," + justs.cExpirationdate + "," + justs.dExpirationdate + ")";
            Cmd.ExecuteNonQuery();

            //修改货位库存
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InvPositionSum(nolock) where cwhcode=" + just.cWhCode + " and cvmivencode=" + justs.cvmivencode + " and cinvcode=" + justs.cInvCode + @" 
                    and cPosCode=" + justs.cAPosCode + " and cbatch=" + justs.cBatch + " and cfree1=" + justs.cFree1 + " and cfree2=" + justs.cFree2 + " and cfree3=" + justs.cFree3 + @" 
                    and cfree4=" + justs.cFree4 + " and cfree5=" + justs.cFree5 + " and cfree6=" + justs.cFree6 + " and cfree7=" + justs.cFree7 + @" 
                    and cfree8=" + justs.cFree8 + " and cfree9=" + justs.cFree9 + " and cfree10=" + justs.cFree10, Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..InvPositionSum(cwhcode,cposcode,cinvcode,iquantity,cbatch,
                            cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cvmivencode,cinvouchtype,iTrackid,
                            dMadeDate,iMassDate,cMassUnit,iExpiratDateCalcu,cExpirationdate,dExpirationdate) 
                        values(" + just.cWhCode + "," + justs.cAPosCode + "," + justs.cInvCode + ",0," + justs.cBatch + @",
                            " + justs.cFree1 + "," + justs.cFree2 + "," + justs.cFree3 + "," + justs.cFree4 + "," + justs.cFree5 + "," +
                        justs.cFree6 + "," + justs.cFree7 + "," + justs.cFree8 + "," + justs.cFree9 + "," + justs.cFree10 + "," + justs.cvmivencode + @",'',0,
                            " + justs.dMadeDate + "," + justs.iMassDate + "," + justs.cMassUnit + "," + justs.iExpiratDateCalcu + "," + justs.cExpirationdate + "," + justs.dExpirationdate + ")";
                Cmd.ExecuteNonQuery();
            }
            Cmd.CommandText = "update " + dbname + "..InvPositionSum set iquantity=isnull(iquantity,0)+(" + justs.iQuantity + "),inum=isnull(inum,0)+(" + justs.iNum + @") 
                        where cwhcode=" + just.cWhCode + " and cvmivencode=" + justs.cvmivencode + " and cinvcode=" + justs.cInvCode + @" 
                        and cPosCode=" + justs.cAPosCode + " and cbatch=" + justs.cBatch + " and cfree1=" + justs.cFree1 + " and cfree2=" + justs.cFree2 + " and cfree3=" + justs.cFree3 + @" 
                        and cfree4=" + justs.cFree4 + " and cfree5=" + justs.cFree5 + " and cfree6=" + justs.cFree6 + " and cfree7=" + justs.cFree7 + @" 
                        and cfree8=" + justs.cFree8 + " and cfree9=" + justs.cFree9 + " and cfree10=" + justs.cFree10;
            Cmd.ExecuteNonQuery();

            #endregion
        }
        #endregion

        return rd_id + "," + cc_mcode;
    }

    //形态转换
    private string U81088(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cwhcode = ""; string w_cdepcode = ""; string w_cpersoncode = ""; string w_inwhcode = "";
        string w_rdcode = ""; string w_in_rdcode = ""; //出库类型
        string w_bustype = "null";//业务类型
        string w_headPosCode = ""; string w_headInPosCode = "";//表头入库货位
        bool b_Vmi = false; bool b_in_Vmi = false; bool b_Pos = false; bool b_in_Pos = false;//货位
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        int i_autocheck_type = 0;// 是否自动审核，0 代表执行通用控制参数；1 代表本单自动审核；2 代表本单不需要审核
        #region  //逻辑检验
        //自动审核 规则
        string c_autocheckvalue = GetTextsFrom_FormData_Tag(HeadData, "txt_i_isauto_check");
        if (c_autocheckvalue == "1" || c_autocheckvalue == "2")
        {
            i_autocheck_type = int.Parse(c_autocheckvalue);
        }

        //入库货位
        w_headPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headposcode");
        w_headInPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headinposcode");
        //出库类别
        w_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_crdcode");
        if (w_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_rdcode + "'", Cmd) == 0)
                throw new Exception("出库类型输入错误");
        }
        //入库类别
        w_in_rdcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinrdcode");
        if (w_in_rdcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Rd_Style where cRDCode='" + w_in_rdcode + "'", Cmd) == 0)
                throw new Exception("入库类型输入错误");
        }

        //仓库校验
        w_cwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "'", Cmd) == 0)
            throw new Exception("转出仓库输入错误");

        w_inwhcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cinwhcode");
        w_headInPosCode = GetTextsFrom_FormData_Tag(HeadData, "txt_headinposcode");
        if (w_inwhcode == "")
        {
            w_inwhcode = w_cwhcode;
            if (w_headInPosCode == "") w_headInPosCode = w_headPosCode;
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_inwhcode + "'", Cmd) == 0)
                throw new Exception("转入仓库输入错误");
        }

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
                throw new Exception("调出部门输入错误");
        }

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        BodyData.DefaultView.Sort = "igroupno,bavtype desc"; //排序
        BodyData = BodyData.DefaultView.ToTable();
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            if (i % 2 == 0 && BodyData.Rows[i]["bavtype"] + "" != "转换前") 
                throw new Exception("组别【" + BodyData.Rows[i]["igroupno"] + "】存货【" + BodyData.Rows[i]["cinvcode"] + "】转换规则出错");
            if (i % 2 == 1 && BodyData.Rows[i]["bavtype"] + "" != "转换后") 
                throw new Exception("组别【" + BodyData.Rows[i]["igroupno"] + "】存货【" + BodyData.Rows[i]["cinvcode"] + "】转换规则出错");

            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        //cfirstToFirst = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='mes_cfirstToFirst'", Cmd);
        string cc_mcode = "";//单据号
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='0305' and VT_TemplateMode=0", Cmd);

        #region //新增单据
        KK_U8Com.U8AssemVouch assemain = new KK_U8Com.U8AssemVouch(Cmd, dbname);
        #region  //新增主表

        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='as'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='as'";
        Cmd.ExecuteNonQuery();
        string cCodeHead = "A" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
        cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cAVCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..AssemVouch where cAVCode like '" + cCodeHead + "%'", Cmd);
        assemain.cAVCode = "'" + cc_mcode + "'";
        assemain.ID = rd_id + "";
        assemain.cORdCode = (w_rdcode.CompareTo("") == 0 ? "null" : "'" + w_rdcode + "'");
        assemain.cDepCode = (w_cdepcode.CompareTo("") == 0 ? "null" : "'" + w_cdepcode + "'");
        assemain.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        assemain.cIRdCode = (w_in_rdcode.CompareTo("") == 0 ? "null" : "'" + w_in_rdcode + "'");
        assemain.VT_ID = vt_id + "";

        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_cwhcode + "' and bWhPos=1", Cmd) > 0)
            b_Pos = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_inwhcode + "' and bProxyWh=1", Cmd) > 0)
            b_in_Vmi = true;
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + w_inwhcode + "' and bWhPos=1", Cmd) > 0)
            b_in_Pos = true;

        assemain.cMaker = "'" + cUserName + "'";
        assemain.dAVDate = "'" + cLogDate + "'";
        assemain.cAVMemo = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cavmemo") + "'";
        assemain.iswfcontrolled = "0";
        assemain.cVouchType = "'15'";
        assemain.dnmaketime = "getdate()";
        assemain.bTransFlag = "0";
        assemain.ctransflag = "'1'";

        #region   //主表自定义项处理
        assemain.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        assemain.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        assemain.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        assemain.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        assemain.cDefine5 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        assemain.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        assemain.cDefine7 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        assemain.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        assemain.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        assemain.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        assemain.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        assemain.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        assemain.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        assemain.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        assemain.cDefine15 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        assemain.cDefine16 = (txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text);
        #endregion

        if (!assemain.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
        Cmd.CommandText = "update " + dbname + "..AssemVouch set csysbarcode='||st15|" + cc_mcode + "' where id=" + assemain.ID;
        Cmd.ExecuteNonQuery();
        #endregion

        #region //子表
        int irdrow = 0;
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            string cvmivencode = GetBodyValue_FromData(BodyData, i, "cbvencode");//代管商
            string c_body_batch = GetBodyValue_FromData(BodyData, i, "cbatch");//批号
            #region  //行业务逻辑校验
            if (float.Parse("" + BodyData.Rows[i]["iquantity"]) == 0) throw new Exception("转换数量不能为空 或0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception("[" + BodyData.Rows[i]["cinvcode"] + "]存货编码不存在");

            //代管验证
            if (BodyData.Rows[i]["bavtype"] + "" == "转换前" && b_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");
            if (BodyData.Rows[i]["bavtype"] + "" == "转换后" && b_in_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");
            if (b_in_Vmi && cvmivencode.CompareTo("") == 0) throw new Exception("代管仓库出库必须有代管商");

            //自由项
            for (int f = 1; f < 11; f++)
            {
                string cfree_value = GetBodyValue_FromData(BodyData, i, "cfree" + f);
                if (U8Operation.GetDataInt("select CAST(bfree" + f + " as int) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 1)
                {
                    if (cfree_value.CompareTo("") == 0) throw new Exception("存货【" + BodyData.Rows[i]["cinvcode"] + "】自由项" + f + " 不能为空");
                }
                else
                {
                    if (cfree_value.CompareTo("") != 0) BodyData.Rows[i]["cfree" + f] = "";
                }
            }
            #endregion

            KK_U8Com.U8AssemVouchs assedetail = new KK_U8Com.U8AssemVouchs(Cmd, dbname);
            int cAutoid = 1000000000 + U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='as'", Cmd);
            assedetail.autoID = cAutoid + "";
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='as'";
            Cmd.ExecuteNonQuery();

            assedetail.ID = rd_id + "";
            assedetail.cAVCode = assemain.cAVCode;
            assedetail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            assedetail.iAVQuantity = "" + BodyData.Rows[i]["iquantity"];
            assedetail.irowno = (i + 1) + "";
            assedetail.iGroupNO = "" + BodyData.Rows[i]["igroupno"];
            assedetail.bAVType = "'" + BodyData.Rows[i]["bavtype"] + "'";
            cst_unitcode = U8Operation.GetDataString("select cSTComUnitCode from " + dbname + "..inventory where cinvcode=" + assedetail.cInvCode + "", Cmd);

            #region //自由项  自定义项处理
            assedetail.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            assedetail.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            assedetail.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            assedetail.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            assedetail.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            assedetail.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            assedetail.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            assedetail.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            assedetail.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            assedetail.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            //自定义项
            assedetail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            assedetail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            assedetail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            assedetail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            assedetail.cDefine26 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            assedetail.cDefine27 = (txtdata_text == "" ? "null" : txtdata_text);
            assedetail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            assedetail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            assedetail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            assedetail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            assedetail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            assedetail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            assedetail.cDefine34 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            assedetail.cDefine35 = (txtdata_text == "" ? "null" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            assedetail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            assedetail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            //自由项检验
            for (int f = 1; f < 11; f++)
            {
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + assedetail.cInvCode + " and bFree" + f + "=1", Cmd) > 0)
                {
                    if (GetBodyValue_FromData(BodyData, i, "cfree" + f) == "")
                        throw new Exception(assedetail.cInvCode + "有自由项" + f + "管理，必须录入");
                }
            }

            #endregion

            #region //获得单价与金额

            #endregion

            #region //代管挂账处理
            assedetail.cvmivencode = "''";
            string b_costing = "1";//是否计入成本
            if (BodyData.Rows[i]["bavtype"] + "" == "转换前")
            {
                b_costing=U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse where cwhcode='"+w_cwhcode+"'", Cmd);
            }
            else
            {
                b_costing = U8Operation.GetDataString("select cast(bincost as int) from " + dbname + "..warehouse where cwhcode='" + w_inwhcode + "'", Cmd);
            }
            assedetail.bCosting = b_costing; //是否计入成本
            if (b_Vmi)
            {;
                assedetail.bCosting = "0";
                assedetail.cvmivencode = "'" + cvmivencode + "'";
            }
            #endregion

            #region //批次管理和保质期管理
            assedetail.cAVBatch  = "''";
            assedetail.iExpiratDateCalcu = "0";
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + assedetail.cInvCode + " and bInvBatch=1", Cmd) > 0)
            {
                if ((!BodyData.Columns.Contains("cbatch")) || BodyData.Rows[i]["cbatch"] + "" == "") throw new Exception(assedetail.cInvCode + "有批次管理，必须输入批号");
                assedetail.cAVBatch = "'" + BodyData.Rows[i]["cbatch"] + "'";

                //保质期管理
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode=" + assedetail.cInvCode + " and bInvQuality=1", Cmd) > 0)
                {
                    DataTable dtBZQ = null;
                    if (BodyData.Rows[i]["bavtype"] + "" == "转换前")
                    {
                        dtBZQ = U8Operation.GetSqlDataTable(@"select top 1 iMassDate 保质期天数,cMassUnit 保质期单位,convert(varchar(10),dMdate,120) 生产日期,
                            convert(varchar(10),dVDate,120) 失效日期,cExpirationdate 有效期至,iExpiratDateCalcu 有效期推算方式,convert(varchar(10),dExpirationdate,120) 有效期计算项
                        from " + dbname + @"..CurrentStock(nolock)
                        where cwhcode='" + w_cwhcode + "' and cinvcode=" + assedetail.cInvCode + " and cbatch=" + assedetail.cAVBatch + " and cvmivencode=" + assedetail.cvmivencode + @" 
                            and cfree1=" + assedetail.cFree1 + " and cfree2=" + assedetail.cFree2 + " and cfree3=" + assedetail.cFree3 + " and cfree4=" + assedetail.cFree4 + @" 
                            and cfree5=" + assedetail.cFree5 + " and cfree6=" + assedetail.cFree6 + " and cfree7=" + assedetail.cFree7 + " and cfree8=" + assedetail.cFree8 + @" 
                            and cfree9=" + assedetail.cFree9 + " and cfree10=" + assedetail.cFree10, "dtbatlist", Cmd);
                        if (dtBZQ.Rows.Count == 0) throw new Exception("未找到转换前批号的 保质期数据");
                    }
                    else
                    {
                        string rowpordate = U8Operation.GetDataString("select convert(varchar(10),getdate(),120)", Cmd);
                        dtBZQ = U8Operation.GetSqlDataTable(@"select iMassDate 保质期天数,cMassUnit 保质期单位,'" + rowpordate + @"' 生产日期,
                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + rowpordate + @"')
                                            when cMassUnit=2 then DATEADD(month,iMassDate,'" + rowpordate + @"')
                                            else DATEADD(day,iMassDate,'" + rowpordate + @"') end)
                                ,120) 失效日期,isnull(s.iExpiratDateCalcu,1) 有效期推算方式
                            from " + dbname + "..inventory i left join " + dbname + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                            where cinvcode=" + assedetail.cInvCode, "dtBZQ", Cmd);
                        if (dtBZQ.Rows.Count == 0) throw new Exception("计算存货保质期出现错误");
                    }

                    assedetail.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    assedetail.iMassDate = dtBZQ.Rows[0]["保质期天数"] + "";
                    assedetail.dDisDate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    assedetail.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    assedetail.dMadeDate = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    assedetail.cMassUnit = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }

                //批次档案 建档
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory_Sub where cInvSubCode=" + assedetail.cInvCode + " and bBatchCreate=1", Cmd) > 0)
                {
                    //从模板中获得批次属性
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty1");
                    assedetail.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty2");
                    assedetail.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty3");
                    assedetail.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty4");
                    assedetail.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty5");
                    assedetail.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    assedetail.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty6") + "'";
                    assedetail.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty7") + "'";
                    assedetail.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty8") + "'";
                    assedetail.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cbatchproperty9") + "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cbatchproperty10");
                    assedetail.cBatchProperty10 = txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'";

                    //批次档案数据
                    DataTable dtBatPerp = U8Operation.GetSqlDataTable(@"select cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                            cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10 from " + dbname + @"..AA_BatchProperty a 
                        where cInvCode=" + assedetail.cInvCode + " and cBatch=" + assedetail.cAVBatch + " and isnull(cFree1,'')=" + assedetail.cFree1 + @" 
                            and isnull(cFree2,'')=" + assedetail.cFree2 + " and isnull(cFree3,'')=" + assedetail.cFree3 + " and isnull(cFree4,'')=" + assedetail.cFree4 + @" 
                            and isnull(cFree5,'')=" + assedetail.cFree5 + " and isnull(cFree6,'')=" + assedetail.cFree6 + " and isnull(cFree7,'')=" + assedetail.cFree7 + @" 
                            and isnull(cFree8,'')=" + assedetail.cFree8 + " and isnull(cFree9,'')=" + assedetail.cFree9 + " and isnull(cFree10,'')=" + assedetail.cFree10, "dtBatPerp", Cmd);
                    if (dtBatPerp.Rows.Count == 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + @"..AA_BatchProperty(cBatchPropertyGUID,cBatchProperty1,cBatchProperty2,cBatchProperty3,cBatchProperty4,cBatchProperty5,cBatchProperty6,
                            cBatchProperty7,cBatchProperty8,cBatchProperty9,cBatchProperty10,cInvCode,cBatch,cFree1,cFree2,cFree3,cFree4,cFree5,cFree6,cFree7,cFree8,cFree9,cFree10)
                        values(newid()," + assedetail.cBatchProperty1 + "," + assedetail.cBatchProperty2 + "," + assedetail.cBatchProperty3 + "," + assedetail.cBatchProperty4 + "," +
                                 assedetail.cBatchProperty5 + "," + assedetail.cBatchProperty6 + "," + assedetail.cBatchProperty7 + "," + assedetail.cBatchProperty8 + "," +
                                 assedetail.cBatchProperty9 + "," + assedetail.cBatchProperty10 + "," + assedetail.cInvCode + "," + assedetail.cAVBatch + "," + assedetail.cFree1 + "," +
                                 assedetail.cFree2 + "," + assedetail.cFree3 + "," + assedetail.cFree4 + "," + assedetail.cFree5 + "," + assedetail.cFree6 + "," +
                                 assedetail.cFree7 + "," + assedetail.cFree8 + "," + assedetail.cFree9 + "," + assedetail.cFree10 + ")";
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            #endregion

            #region //固定换算率（多计量）
            assedetail.iAVNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                assedetail.cAssUnit = "'" + cst_unitcode + "'";
                string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + assedetail.cAssUnit, Cmd);

                if (ichange == "") ichange = "0";
                if (float.Parse(ichange) == 0)
                {
                    //浮动换算率
                    inum = GetBodyValue_FromData(BodyData, i, "inum");
                    if (inum == "")
                    {
                        inum = "0";
                    }
                    else
                    {
                        //浮动换算率
                        ichange = U8Operation.GetDataString("select round(" + assedetail.iAVQuantity + "/" + inum + ",5)", Cmd);
                    }
                    assedetail.iAVNum = inum;
                }
                else
                {
                    //固定换算率
                    inum = U8Operation.GetDataString("select round(" + assedetail.iAVQuantity + "/" + ichange + ",5)", Cmd);
                    assedetail.iAVNum = inum;
                }

            }
            #endregion

            #region//货位标识处理  ，若无货位取存货档案的默认货位
            if (BodyData.Rows[i]["bavtype"] + "" == "转换前")
            {
                if (b_Pos)
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = w_headPosCode;
                    if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("仓库【" + w_cwhcode + "】有货位管理，" + assedetail.cInvCode + "的货位不能为空");

                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode='" + w_cwhcode + "' and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于仓库【" + w_cwhcode + "】");

                    assedetail.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                assedetail.cWhCode = "'" + w_cwhcode + "'";
            }
            if (BodyData.Rows[i]["bavtype"] + "" == "转换后")
            {
                if (b_in_Pos)
                {
                    if (BodyData.Rows[i]["cposcode"] + "" == "") BodyData.Rows[i]["cposcode"] = w_headInPosCode;
                    if (BodyData.Rows[i]["cposcode"] + "" == "") throw new Exception("仓库【" + w_inwhcode + "】有货位管理，" + assedetail.cInvCode + "的货位不能为空");

                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..position where cwhcode='" + w_inwhcode + "' and cposcode='" + BodyData.Rows[i]["cposcode"] + "'", Cmd) == 0)
                        throw new Exception("货位编码【" + BodyData.Rows[i]["cposcode"] + "】不存在，或者不属于仓库【" + w_inwhcode + "】");

                    assedetail.cPosition = "'" + BodyData.Rows[i]["cposcode"] + "'";
                }
                assedetail.cWhCode = "'" + w_inwhcode + "'";
            }
            
            #endregion

            //保存数据
            if (!assedetail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            Cmd.CommandText = "update " + dbname + "..AssemVouchs set cbsysbarcode='||st15|" + cc_mcode + "|" + assedetail.irowno + "' where id=" + assemain.ID;
            Cmd.ExecuteNonQuery();
        }
        

        #endregion
        #endregion

        #region   //审核形态转换单
        string cAVVouchAutoCHeck = U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8barcode_avvouch_is_check'", Cmd);
        U8Operation.GetDataString("select 'i_autocheck_type:" + i_autocheck_type + "'", Cmd);
        if ((i_autocheck_type == 0 && cAVVouchAutoCHeck.CompareTo("true") == 0) || i_autocheck_type == 1)
        {
            //组合其他出库单
            DataTable dtRd09Main = U8Operation.GetSqlDataTable(@"select '" + w_cwhcode + @"' cwhcode,cdepcode,cORdCode crdcode,cpersoncode,
	                '' cvencode,'转换出库' cbustype,cAVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cAVMemo cmemo
                from " + dbname + "..AssemVouch where ID=0" + assemain.ID, "HeadData", Cmd);
            DataTable dtRd09detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cAVBatch cbatch,b.iAVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iAVNum inum,b.autoid itransid,b.cPosition cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..AssemVouchs b inner join " + dbname + @"..inventory i on b.cInvCode=i.cInvCode 
                where b.ID=0" + assemain.ID + " and bavtype='转换前'", "BodyData", Cmd);
            if (dtRd09Main.Rows.Count == 0) throw new Exception("无法找到形态转换单");
            if (dtRd09detail.Rows.Count == 0) throw new Exception("无法找到形态转换单转换前内容数据");
            DataTable SHeadData = GetDtToHeadData(dtRd09Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81018(SHeadData, dtRd09detail, dbname, cUserName, cLogDate, "U81018_1", Cmd);

            //组合其他入库单
            DataTable dtRd08Main = U8Operation.GetSqlDataTable(@"select '" + w_inwhcode + @"' cwhcode,cdepcode,cIRdCode crdcode,cpersoncode,
	                '' cvencode,'转换入库' cbustype,cAVCode cbuscode,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,
	                cdefine7,cdefine8,cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,cAVMemo cmemo
                from " + dbname + "..AssemVouch where ID=0" + assemain.ID, "HeadData", Cmd);
            DataTable dtRd08detail = U8Operation.GetSqlDataTable(@"select i.cinvcode,i.cinvname,i.cinvstd,b.cAVBatch cbatch,b.iAVQuantity iquantity,b.cvmivencode cbvencode,
	                b.iAVNum inum,b.autoid itransid,b.cPosition cposcode,cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cFree10,
	                cdefine22,cdefine22,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
	                cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,convert(varchar(10),b.dMadeDate,120) dprodate,b.citemcode,b.cItem_class citemclass
                from " + dbname + "..AssemVouchs b inner join " + dbname + @"..inventory i on b.cInvCode=i.cInvCode 
                where b.ID=0" + assemain.ID + " and bavtype='转换后'", "BodyData", Cmd);
            if (dtRd08Main.Rows.Count == 0) throw new Exception("无法找到形态转换单");
            if (dtRd08detail.Rows.Count == 0) throw new Exception("无法找到形态转换单转换后内容数据");
            SHeadData = GetDtToHeadData(dtRd08Main, 0);
            SHeadData.PrimaryKey = new System.Data.DataColumn[] { SHeadData.Columns["TxtName"] };
            U81019(SHeadData, dtRd08detail, dbname, cUserName, cLogDate, "U81019_1", Cmd);

            //审核调拨单状态
            Cmd.CommandText = "update " + dbname + "..AssemVouch set cVerifyPerson='" + cUserName + "',dVerifyDate='" + cLogDate + "',dnverifytime=getdate() where id=" + rd_id;
            Cmd.ExecuteNonQuery();
        }

        #endregion

        return rd_id + "," + cc_mcode;
    }

    //销售订单
    private string U81089(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string w_cdepcode = ""; string w_cpersoncode = ""; string w_cuscode = "";
        string w_stcode = ""; //销售类型
        string w_bustype = "普通销售";//业务类型
        string iAoids = ""; string iA_ID = ""; ; bool bCreate = false;//是否生单
        string cst_unitcode = ""; string inum = "";//多计量单位的 辅助库存计量单位
        string c_cuspersoncode = "";
        string[] txtdata = null;  //临时取数: 0 显示标题   1 txt_字段名称    2 文本Tag     3 文本Text值
        #region  //逻辑检验
        if (BodyData.Columns.Contains("iaoids"))
        {
            iAoids = BodyData.Rows[0]["iaoids"] + "";
            if (iAoids.CompareTo("") != 0)
            {
                bCreate = true;
                iA_ID = U8Operation.GetDataString("select ID from " + dbname + "..SA_PreOrderDetails where autoid=0" + iAoids, Cmd);
                if (iA_ID == "") throw new Exception("没有找到销售预订单信息");
            }
        }

        //业务类型
        w_bustype = GetTextsFrom_FormData_Tag(HeadData, "txt_cbustype");
        if (w_bustype.CompareTo("") == 0) throw new Exception("业务类型不能为空");

        //客户校验
        w_cuscode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuscode");
        if (w_cuscode.CompareTo("") == 0)
        {
            w_cuscode = U8Operation.GetDataString("select ccuscode from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd);
        }
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..customer where ccuscode='" + w_cuscode + "'", Cmd) == 0)
            throw new Exception("客户输入错误");

        //销售类别
        w_stcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cstcode");
        if (w_stcode.CompareTo("") == 0)
            w_stcode = U8Operation.GetDataString("select cstcode from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd);
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..SaleType where cSTCode='" + w_stcode + "'", Cmd) == 0)
            throw new Exception("销售类型输入错误");

        //部门校验
        w_cdepcode = GetTextsFrom_FormData_Tag(HeadData, "txt_cdepcode");
        if (w_cdepcode.CompareTo("") == 0)
            w_cdepcode = U8Operation.GetDataString("select cdepcode from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd);
        if (U8Operation.GetDataInt("select count(*) from " + dbname + "..department where cdepcode='" + w_cdepcode + "'", Cmd) == 0)
            throw new Exception("部门输入错误");

        //业务员
        w_cpersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_cpersoncode");
        if (w_cpersoncode.CompareTo("") != 0)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..person where cpersoncode='" + w_cpersoncode + "'", Cmd) == 0)
                throw new Exception("业务员输入错误");
        }
        else
        {
            w_cpersoncode = U8Operation.GetDataString("select cpersoncode from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd);
        }
        //联系人
        c_cuspersoncode = GetTextsFrom_FormData_Tag(HeadData, "txt_ccuspersoncode");
        string c_cuspername = "";
        if (c_cuspersoncode.CompareTo("") != 0)
        {
            c_cuspername = U8Operation.GetDataString(@"select cContactName from " + dbname + @"..Crm_Contact 
                where cContactCode='" + c_cuspersoncode + "' and cCusCode='" + w_cuscode + "'", Cmd);
            if (c_cuspername=="") throw new Exception("订单联系人输入错误");
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select a.t_fieldname from " + dbname + @"..T_CC_Base_GridCol_rule a 
                    inner join " + dbname + @"..T_CC_Base_GridColShow b on a.SheetID=b.SheetID and a.t_fieldname=b.t_colname
                where a.SheetID='" + SheetID + @"' 
                and (a.t_fieldname like '%define%' or a.t_fieldname like '%free%') and isnull(b.t_location,'')='H'", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据头必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='H'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //检查单据体必录项
        dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID='' and isnull(t_location,'')='B'", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            for (int r = 0; r < BodyData.Rows.Count; r++)
            {
                string bdata = GetBodyValue_FromData(BodyData, r, cc_colname);
                if (bdata.CompareTo("") == 0) throw new Exception("行【" + (r + 1) + "】" + dtMustInputCol.Rows[i]["t_colshow"] + "属于必录项,请输入");
            }
        }

        #endregion

        //U8版本
        float fU8Version = float.Parse(U8Operation.GetDataString("select CAST(cValue as float) from " + dbname + "..AccInformation where cSysID='aa' and cName='VersionFlag'", Cmd));
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        string cc_mcode = "";//单据号
        #region //新增单据
        KK_U8Com.U8SO_SOMain so_main = new KK_U8Com.U8SO_SOMain(Cmd, dbname);
        #region  //新增主表

        int rd_id = 1000000000 + U8Operation.GetDataInt("select isnull(max(iFatherID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='Somain'", Cmd);
        Cmd.CommandText = "update ufsystem..UA_Identity set iFatherID=iFatherID+1 where cacc_id='" + targetAccId + "' and cVouchType='Somain'";
        Cmd.ExecuteNonQuery();
        cc_mcode = GetTextsFrom_FormData_Tag(HeadData, "txt_csocode"); //传递的销售订单号
        if (cc_mcode == "")
        {
            string cCodeHead = "F" + U8Operation.GetDataString("select right(replace(convert(varchar(10),cast('" + cLogDate + "' as  datetime),120),'-',''),6)", Cmd); ;
            cc_mcode = cCodeHead + U8Operation.GetDataString("select right('000'+cast(cast(isnull(right(max(cSOCode),4),'0000') as int)+1 as varchar(9)),4) from " + dbname + "..SO_SOMain where cSOCode like '" + cCodeHead + "%'", Cmd);
        }
        else
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..SO_SOMain where cSOCode='" + cc_mcode + "'", Cmd) > 0) throw new Exception("销售订单【" + cc_mcode + "】已经存在");
        }
        so_main.cSOCode = "'" + cc_mcode + "'";
        so_main.ID = rd_id;
        so_main.cSTCode = (w_stcode.CompareTo("") == 0 ? "null" : "'" + w_stcode + "'");
        so_main.cDepCode = "'" + w_cdepcode + "'";
        so_main.cPersonCode = (w_cpersoncode.CompareTo("") == 0 ? "null" : "'" + w_cpersoncode + "'");
        so_main.cCusCode = "'" + w_cuscode + "'";
        int vt_id = U8Operation.GetDataInt("select isnull(min(VT_ID),0) from " + dbname + "..vouchertemplates_base where VT_CardNumber='17' and VT_TemplateMode=0", Cmd);
        so_main.iVTid = vt_id;
        so_main.cBusType = "'" + w_bustype + "'";
        so_main.cMaker = "'" + cUserName + "'";
        so_main.dDate = "'" + cLogDate + "'";
        so_main.cMemo = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cmemo") + "'";
        so_main.iswfcontrolled = 0;

        if (!bCreate)
        {
            so_main.iExchRate = 1;
            so_main.cexch_name = "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_cexch_name") + "'";
            so_main.iTaxRate = 13;
        }
        else
        {
            so_main.iTaxRate = float.Parse(U8Operation.GetDataString("select iTaxRate from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd));
            so_main.iExchRate = float.Parse(U8Operation.GetDataString("select iExchRate from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd));
            so_main.cexch_name = "'" + U8Operation.GetDataString("select cexch_name from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd) + "'";
        }

        #region   //主表自定义项处理
        so_main.cDefine1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine1") + "'";
        so_main.cDefine2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine2") + "'";
        so_main.cDefine3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine3") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine4");  //日期
        so_main.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine5");
        so_main.cDefine5 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine6");  //日期
        so_main.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine7");
        so_main.cDefine7 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        so_main.cDefine8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine8") + "'";
        so_main.cDefine9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine9") + "'";
        so_main.cDefine10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine10") + "'";
        so_main.cDefine11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine11") + "'";
        so_main.cDefine12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine12") + "'";
        so_main.cDefine13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine13") + "'";
        so_main.cDefine14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_cdefine14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine15");
        so_main.cDefine15 = (txtdata_text.CompareTo("") == 0 ? 0 : int.Parse(txtdata_text));
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_cdefine16");
        so_main.cDefine16 = (txtdata_text.CompareTo("") == 0 ? 0 : float.Parse(txtdata_text));
        #endregion

        #region   //继承第一张上游单据的表头自定义项
        DataTable dtUpVouchHeadDefine = U8Operation.GetSqlDataTable(@"select cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,cdefine6,cdefine7,cdefine8,
                cdefine9,cdefine10,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16 from " + dbname + @"..SA_PreOrderMain a 
            where ID=0" + iA_ID, "dtUpVouchHeadDefine", Cmd);
        if (dtUpVouchHeadDefine.Rows.Count > 0)
        {
            if (so_main.cDefine1.CompareTo("''") == 0) so_main.cDefine1 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine1") + "'";
            if (so_main.cDefine2.CompareTo("''") == 0) so_main.cDefine2 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine2") + "'";
            if (so_main.cDefine3.CompareTo("''") == 0) so_main.cDefine3 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine3") + "'";
            txtdata_text = GetTextsFrom_FormData_Text(HeadData, "cdefine4");
            if (so_main.cDefine4.CompareTo("null") == 0) so_main.cDefine4 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetTextsFrom_FormData_Text(HeadData, "cdefine6");
            if (so_main.cDefine6.CompareTo("null") == 0) so_main.cDefine6 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");

            if (so_main.cDefine8.CompareTo("''") == 0) so_main.cDefine8 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine8") + "'";
            if (so_main.cDefine9.CompareTo("''") == 0) so_main.cDefine9 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine9") + "'";
            if (so_main.cDefine10.CompareTo("''") == 0) so_main.cDefine10 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine10") + "'";
            if (so_main.cDefine11.CompareTo("''") == 0) so_main.cDefine11 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine11") + "'";
            if (so_main.cDefine12.CompareTo("''") == 0) so_main.cDefine12 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine12") + "'";
            if (so_main.cDefine13.CompareTo("''") == 0) so_main.cDefine13 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine13") + "'";
            if (so_main.cDefine14.CompareTo("''") == 0) so_main.cDefine14 = "'" + GetBodyValue_FromData(dtUpVouchHeadDefine, 0, "cdefine14") + "'";
        }
        #endregion

        if (!so_main.InsertToDB(targetAccId, ref errmsg)) { throw new Exception(errmsg); }

        //开票单位
        string c_kp_dw_code = U8Operation.GetDataString("select cInvoiceCompany from " + dbname + "..customer where ccuscode=" + so_main.cCusCode, Cmd);
        string i_flowid = "";
        Cmd.CommandText = "update " + dbname + @"..SO_SOMain set cinvoicecompany='" + c_kp_dw_code + @"',
                iflowid=" + (i_flowid == "" ? "null" : i_flowid) + ",ccuspersoncode = '" + c_cuspersoncode + "',ccusperson='" + c_cuspername + "' where id=" + so_main.ID;
        Cmd.ExecuteNonQuery();
        #endregion

        #region //子表
        int irdrow = 0;
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {

            #region  //行业务逻辑校验
            if (decimal.Parse(BodyData.Rows[i]["iquantity"] + "") <= 0) throw new Exception("订单数量不能小于0");
            if (U8Operation.GetDataInt("select count(*) from " + dbname + "..inventory where cinvcode='" + BodyData.Rows[i]["cinvcode"] + "'", Cmd) == 0)
                throw new Exception(BodyData.Rows[i]["cinvcode"] + "存货编码不存在");

            //生单规则，供应商和业务类型一致性检查
            if (bCreate)
            {
                iAoids = BodyData.Rows[i]["iaoids"] + "";
                iA_ID = U8Operation.GetDataString("select ID from " + dbname + "..SA_PreOrderDetails where autoid=0" + iAoids, Cmd);

                string chkValue = "";
                if (iA_ID.CompareTo("") == 0) throw new Exception("生单模式必须根据销售预订单生成");
                chkValue = U8Operation.GetDataString("select ccuscode from " + dbname + "..SA_PreOrderMain a where id=0" + iA_ID, Cmd);
                if (chkValue.CompareTo(w_cuscode) != 0) throw new Exception("所有销售预订单必须客户相同");
            }
            #endregion

            KK_U8Com.U8SO_SODetails so_detail = new KK_U8Com.U8SO_SODetails(Cmd, dbname);
            int cAutoid = U8Operation.GetDataInt("select isnull(max(iChildID),0)+1 from ufsystem..UA_Identity with(rowlock) where cacc_id='" + targetAccId + "' and cVouchType='Somain'", Cmd);
            so_detail.iSOsID = cAutoid;
            Cmd.CommandText = "update ufsystem..UA_Identity set iChildID=iChildID+1 where cacc_id='" + targetAccId + "' and cVouchType='Somain'";
            Cmd.ExecuteNonQuery();

            so_detail.ID = rd_id;
            so_detail.cInvCode = "'" + BodyData.Rows[i]["cinvcode"] + "'";
            so_detail.iQuantity = "" + BodyData.Rows[i]["iquantity"];
            if (BodyData.Columns.Contains("dpremodate")) so_detail.dPreMoDate = "'" + BodyData.Rows[i]["dpremodate"] + "'";
            so_detail.cSOCode = so_main.cSOCode;
            irdrow++;
            so_detail.iRowNo = irdrow;
            cst_unitcode = U8Operation.GetDataString("select cSAComUnitCode from " + dbname + "..inventory where cinvcode=" + so_detail.cInvCode, Cmd);
            string cst_unitgroup = U8Operation.GetDataString("select cGroupCode from " + dbname + "..inventory where cinvcode=" + so_detail.cInvCode, Cmd);
            so_detail.cCusInvCode = "'" + U8Operation.GetDataString("select cCusInvCode from " + dbname + "..CusInvContrapose where cInvCode=" + so_detail.cInvCode + " and cCusCode=" + so_main.cCusCode, Cmd) + "'";
            so_detail.cCusInvName = "'" + U8Operation.GetDataString("select cCusInvName from " + dbname + "..CusInvContrapose where cInvCode=" + so_detail.cInvCode + " and cCusCode=" + so_main.cCusCode, Cmd) + "'";
            so_detail.dPreDate = "'" + GetBodyValue_FromData(BodyData, i, "dpredate") + "'";
            if (so_detail.dPreDate == "''") so_detail.dPreDate = "convert(varchar(10),dateadd(day,1,GETDATE()),120)";
            so_detail.cMemo ="'"+ GetBodyValue_FromData(BodyData, i, "cbmemo")+"'";
            
            #region //自由项  自定义项处理
            so_detail.cFree1 = "'" + GetBodyValue_FromData(BodyData, i, "cfree1") + "'";
            so_detail.cFree2 = "'" + GetBodyValue_FromData(BodyData, i, "cfree2") + "'";
            so_detail.cFree3 = "'" + GetBodyValue_FromData(BodyData, i, "cfree3") + "'";
            so_detail.cFree4 = "'" + GetBodyValue_FromData(BodyData, i, "cfree4") + "'";
            so_detail.cFree5 = "'" + GetBodyValue_FromData(BodyData, i, "cfree5") + "'";
            so_detail.cFree6 = "'" + GetBodyValue_FromData(BodyData, i, "cfree6") + "'";
            so_detail.cFree7 = "'" + GetBodyValue_FromData(BodyData, i, "cfree7") + "'";
            so_detail.cFree8 = "'" + GetBodyValue_FromData(BodyData, i, "cfree8") + "'";
            so_detail.cFree9 = "'" + GetBodyValue_FromData(BodyData, i, "cfree9") + "'";
            so_detail.cFree10 = "'" + GetBodyValue_FromData(BodyData, i, "cfree10") + "'";

            //自定义项
            so_detail.cDefine22 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine22") + "'";
            so_detail.cDefine23 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine23") + "'";
            so_detail.cDefine24 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine24") + "'";
            so_detail.cDefine25 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine26");
            so_detail.cDefine26 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine27");
            so_detail.cDefine27 = (txtdata_text == "" ? 0 : float.Parse(txtdata_text));
            so_detail.cDefine28 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine28") + "'";
            so_detail.cDefine29 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine29") + "'";
            so_detail.cDefine30 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine30") + "'";
            so_detail.cDefine31 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine31") + "'";
            so_detail.cDefine32 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine32") + "'";
            so_detail.cDefine33 = "'" + GetBodyValue_FromData(BodyData, i, "cdefine33") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine34");
            so_detail.cDefine34 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine35");
            so_detail.cDefine35 = (txtdata_text == "" ? 0 : int.Parse(txtdata_text));
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine36");
            so_detail.cDefine36 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            txtdata_text = GetBodyValue_FromData(BodyData, i, "cdefine37");
            so_detail.cDefine37 = (txtdata_text.CompareTo("") == 0 ? "null" : "'" + txtdata_text + "'");
            #endregion

            #region //获得单价与金额
            so_detail.iTaxUnitPrice=GetBodyValue_FromData(BodyData, i, "itaxunitprice");    //含税单价
            so_detail.iUnitPrice = GetBodyValue_FromData(BodyData, i, "iunitprice");     //无税单价
            so_detail.iQuotedPrice = float.Parse(GetBodyValue_FromData(BodyData, i, "iunitprice"));     //报价
            so_detail.iTaxRate = 0;
            string c_itaxrate=GetBodyValue_FromData(BodyData, i, "itaxrate");               //税率
            if (c_itaxrate != "") so_detail.iTaxRate = float.Parse(c_itaxrate);
            string c_sum = GetBodyValue_FromData(BodyData, i, "isum");                  //含税金额
            if (c_sum == "") c_sum = Math.Round(decimal.Parse(so_detail.iTaxUnitPrice) * decimal.Parse(so_detail.iQuantity), 2) + "";
            if (c_sum != "")
            {
                so_detail.iSum = c_sum;
                if (so_detail.iTaxUnitPrice != "")
                {
                    if (Math.Abs(Math.Round(decimal.Parse(so_detail.iTaxUnitPrice) * decimal.Parse(so_detail.iQuantity), 2) - decimal.Parse(c_sum)) > 0.2M)
                        throw new Exception("传递的含税单价和金额不一致");
                }
            }

            if (bCreate || c_sum == "")
            {
                string ctaxmoney = ""; string ctaxrate = "";
                DataTable dtDetailRow = U8Operation.GetSqlDataTable("select iTaxUnitPrice,iUnitPrice,round(iTaxUnitPrice*(" + so_detail.iQuantity + @"),2) imoney,isnull(iTaxRate,0) iTaxRate
                    from " + dbname + "..SA_PreOrderDetails where autoid=0" + iAoids, "dtDetailRow", Cmd);
                if (dtDetailRow.Rows.Count == 0) throw new Exception("继承销售预订单单价时：未找到订单");
                ctaxmoney = dtDetailRow.Rows[0]["imoney"] + "";
                ctaxrate = dtDetailRow.Rows[0]["iTaxRate"] + "";

                if (ctaxrate == "") ctaxrate = "13";
                so_detail.iSum = (ctaxmoney.CompareTo("") == 0 ? "null" : ctaxmoney);
                so_detail.iTaxUnitPrice = ("" + dtDetailRow.Rows[0]["iTaxUnitPrice"] == "" ? "null" : "" + dtDetailRow.Rows[0]["iTaxUnitPrice"]);
                so_detail.iUnitPrice = ("" + dtDetailRow.Rows[0]["iUnitPrice"] == "" ? "null" : "" + dtDetailRow.Rows[0]["iUnitPrice"]);
                so_detail.iTaxRate = float.Parse(ctaxrate);
            }
            if (so_detail.iSum.CompareTo("") == 0) //自动获得价格表单价
            {
                so_detail.iSum = U8Operation.GetDataString("select top 1 round(iInvNowCost*(" + so_detail.iQuantity + "),2) from " + dbname + @"..SA_CusUPrice 
                        where cInvCode=" + so_detail.cInvCode + " and cCusCode=" + so_main.cCusCode + " order by dStartDate desc", Cmd);
            }
            if (so_detail.iSum == "") so_detail.iSum = "null";
            if (so_detail.iTaxUnitPrice == "") so_detail.iTaxUnitPrice = "null";
            if (so_detail.iUnitPrice == "") so_detail.iUnitPrice = "null";
            #endregion

            #region //固定换算率（多计量） 和 回写到生产单
            so_detail.iNum = "null";
            if (cst_unitcode.CompareTo("") != 0)
            {
                so_detail.cUnitID = "'" + GetBodyValue_FromData(BodyData, i, "cunitid") + "'";
                if (so_detail.cUnitID == "''") so_detail.cUnitID = "'" + cst_unitcode + "'";
                else
                {
                    if (U8Operation.GetDataInt("select count(1) from " + dbname + "..ComputationUnit where cComunitCode=" + so_detail.cUnitID + " and cGroupCode='" + cst_unitgroup + "'", Cmd) == 0)
                    {
                        throw new Exception("组'" + cst_unitgroup + "'计量单位编码：" + so_detail.cUnitID + "不存在");
                    }
                }
                string ichange = "";
                if (BodyData.Columns.Contains("inum")) inum = "" + BodyData.Rows[i]["inum"];
                if (inum == "") inum = "0";
                if (decimal.Parse(inum) == 0)
                {
                    ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + so_detail.cUnitID, Cmd);
                    if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
                    inum = U8Operation.GetDataString("select round(" + so_detail.iQuantity + "/" + ichange + ",5)", Cmd);
                }
                else
                {
                    ichange = "" + (decimal.Parse(so_detail.iQuantity) / decimal.Parse(inum));
                }
                so_detail.iInvExchRate = ichange;
                so_detail.iNum = inum;
            }


            //so_detail.iNum = GetBodyValue_FromData(BodyData, i, "inum");
            //if (cst_unitcode.CompareTo("") != 0)
            //{
            //    so_detail.cUnitID = "'" + GetBodyValue_FromData(BodyData, i, "cunitid") + "'";
            //    if (so_detail.cUnitID == "''") { so_detail.cUnitID = "'" + cst_unitcode + "'"; }
            //    else
            //    {
            //        if (U8Operation.GetDataInt("select count(1) from " + dbname + "..ComputationUnit where cComunitCode=" + so_detail.cUnitID + " and cGroupCode='" + cst_unitgroup + "'", Cmd) == 0)
            //        {
            //            throw new Exception("组'" + cst_unitgroup + "'计量单位编码：" + so_detail.cUnitID + "不存在");
            //        }
            //    }
            //    if (so_detail.iNum == "")
            //    {
            //        string ichange = U8Operation.GetDataString("select isnull(iChangRate,0) from " + dbname + "..ComputationUnit where cComunitCode=" + so_detail.cUnitID, Cmd);
            //        if (ichange == "" || float.Parse(ichange) == 0) throw new Exception("多计量单位必须有换算率，计量单位编码：" + cst_unitcode);
            //        so_detail.iInvExchRate = ichange;
            //        inum = U8Operation.GetDataString("select round(" + so_detail.iQuantity + "/" + ichange + ",5)", Cmd);
            //        so_detail.iNum = inum;
            //    }
            //}
            //if (so_detail.iNum == "") so_detail.iNum = "null";
            #endregion

            #region//上游单据关联
            DataTable dtPuArr = null;  //上游单据表体自定义项继承
            string c_pre_code = U8Operation.GetDataString("select cCode from " + dbname + "..SA_PreOrderMain where id=0" + iA_ID, Cmd);
            if (bCreate)
            {
                if (U8Operation.GetDataString("select isnull(cVerifier,'') from " + dbname + "..SA_PreOrderMain a where id=0" + iA_ID, Cmd) == "")
                    throw new Exception("存货" + so_detail.cInvCode + "的销售预订单未终审");

                //继承到货单的表体自由项  自定义项数据
                dtPuArr = U8Operation.GetSqlDataTable(@"select cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,
                        cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,cdefine31,cdefine32,cdefine33,cdefine34,
                        cdefine35,cdefine36,cdefine37 from " + dbname + @"..SA_PreOrderDetails a 
                    where autoid=0" + iAoids, "dtPuArr", Cmd);
                if (dtPuArr.Rows.Count == 0) throw new Exception("没有找到预订单信息");

                if (so_detail.cDefine22.CompareTo("''") == 0) so_detail.cDefine22 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine22") + "'";
                if (so_detail.cDefine23.CompareTo("''") == 0) so_detail.cDefine23 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine23") + "'";
                if (so_detail.cDefine24.CompareTo("''") == 0) so_detail.cDefine24 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine24") + "'";
                if (so_detail.cDefine25.CompareTo("''") == 0) so_detail.cDefine25 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine25") + "'";

                if (so_detail.cDefine22.CompareTo("''") == 0) so_detail.cDefine28 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine28") + "'";
                if (so_detail.cDefine22.CompareTo("''") == 0) so_detail.cDefine29 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine29") + "'";
                if (so_detail.cDefine30.CompareTo("''") == 0) so_detail.cDefine30 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine30") + "'";
                if (so_detail.cDefine31.CompareTo("''") == 0) so_detail.cDefine31 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine31") + "'";
                if (so_detail.cDefine32.CompareTo("''") == 0) so_detail.cDefine32 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine32") + "'";
                if (so_detail.cDefine33.CompareTo("''") == 0) so_detail.cDefine33 = "'" + GetBodyValue_FromData(dtPuArr, 0, "cdefine33") + "'";

            }
            #endregion

            //保存数据
            if (!so_detail.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }
            Cmd.CommandText = "update " + dbname + @"..SO_SODetails set iQuantity=round(iQuantity,5),
                    dPreMoDate= case when dPreMoDate is null then dateadd(day,-1," + so_detail.dPreDate + @") else dPreMoDate end
                where isosid=" + so_detail.iSOsID;
            Cmd.ExecuteNonQuery();

            if (bCreate)
            {
                Cmd.CommandText = "update " + dbname + "..SO_SODetails set cpreordercode='" + c_pre_code + "',iaoids=0" + iAoids + " where isosid=" + so_detail.iSOsID;
                Cmd.ExecuteNonQuery();
            }

            

        }
        //订单审核
        Cmd.CommandText = @"update " + dbname + @"..SO_SOMain set cVerifier=" + "'" + GetTextsFrom_FormData_Tag(HeadData, "txt_checker") + "'" + @",iStatus=1,iswfcontrolled=0,
                    dverifydate=convert(varchar(10),getdate(),120),dverifysystime=getdate() where id=" + rd_id;
        Cmd.ExecuteNonQuery();

        //流程分支ID
        if (w_stcode.CompareTo("") != 0)
        {
            Cmd.CommandText = @"update " + dbname + @"..SO_SOMain set iflowid=" + U8Operation.GetDataInt("select iFlowID from " + dbname + "..SABizFlow where cSalesTypes = (select cSTCode from " + dbname + "..SO_SOMain where ID = " + rd_id + ")", Cmd) + @" where id=" + rd_id;
            Cmd.ExecuteNonQuery();
        }

        #endregion
        //dpremodate  dpredate
        #endregion
        return rd_id + "," + cc_mcode;
    }

    //销售发货：扫码发货自动其他入库
    private string U81101(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        if (BodyData.Rows.Count == 0) throw new Exception("没有发货数据行");
        #region //推送其他入库
        //表头传入入库类别,通过T_Parameter配置(未入库先发货入库类别)
        DataTable crkTypeDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @" 
                select top 1  t1.cRdCode,t1.cRdName from " + dbname + @"..Rd_Style t1 
                inner join " + dbname + @"..T_Parameter t2 on t1.cRdCode=t2.cValue
                where t2.cPID ='TimelycRdCode'
                ");
        if (crkTypeDt.Rows.Count == 0)
        {
            throw new Exception("未设置{未入库先发货入库类别},或参数表中没有设置{未入库先发货入库类别}TimelycRdCode");
        }

        HeadData.PrimaryKey = new System.Data.DataColumn[] { HeadData.Columns["TxtName"] };
        DataTable dtHead = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' LabelText,'' TxtName,'' TxtTag,'' TxtValue where 1=0");

        DataRow dr = dtHead.NewRow();
        dr["LabelText"] = "仓库";  //栏目标题
        dr["TxtName"] = "txt_cwhcode";   //txt_字段名称
        dr["TxtTag"] = GetTextsFrom_FormData_Tag(HeadData, "txt_cwhcode");  //标识
        dr["TxtValue"] = dr["TxtTag"];  //栏目值
        dtHead.Rows.Add(dr);
        dr = dtHead.NewRow();
        dr["LabelText"] = "入库类别";  //栏目标题
        dr["TxtName"] = "txt_crdcode";   //txt_字段名称
        dr["TxtTag"] = crkTypeDt.Rows[0][0];  //标识
        dr["TxtValue"] = crkTypeDt.Rows[0][1];  //栏目值
        dtHead.Rows.Add(dr);
        dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };

        DataTable dtBody = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' cbarcodetxt,'' cbatch,'' cinvcode,'' cinvname, '' cinvstd, '' cposcode, 0 iquantity,'' itransid,0 modid where 1=0");
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            DataRow bodydr = dtBody.NewRow();
            bodydr["cinvcode"] = BodyData.Rows[i]["cinvcode"]; //存货编码
            bodydr["iquantity"] = BodyData.Rows[i]["iquantity"]; //数量
            bodydr["cposcode"] = ""; //货位必须有，值可以是空
            bodydr["cbatch"] = BodyData.Rows[i]["cbatch"] + ""; //批号最好给固定值

            bodydr["modid"] = BodyData.Rows[i]["modid"];
            dtBody.Rows.Add(bodydr);
        }
        string rkCode = U81019(dtHead, dtBody, dbname, cUserName, cLogDate, "U81019", Cmd);
        if (string.IsNullOrEmpty(rkCode))
        {
            throw new Exception("生成其他入库单失败,未能获得反馈的入库单据号");
        }
        #endregion

        //推送发货单
        string RetCode = U81020(HeadData, BodyData, dbname, cUserName, cLogDate, "U81020", Cmd);
        if (RetCode == "") throw new Exception("出库失败,未能获得反馈的发货单据号");

        for (int i = 0; i < dtBody.Rows.Count; i++)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..T_CC_CQCC_AutoRd08_Relation where ctype='发货' and vouchid='" + RetCode.Split(',')[0] + "'", Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_CQCC_AutoRd08_Relation(ctype,vouchid,rd08id,modid,qty_in,qty_out) 
                values('发货'," + RetCode.Split(',')[0] + "," + rkCode.Split(',')[0] + "," + dtBody.Rows[i]["modid"] + "," + dtBody.Rows[i]["iquantity"] + ",0)";
                Cmd.ExecuteNonQuery();
            }
        }
        return RetCode;

    }

    //销售调拨：扫码调拨自动生成其他入库蓝字
    private string U81102(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        if (BodyData.Rows.Count == 0) throw new Exception("没有发货数据行");
        if (HeadData.Rows.Count == 0) throw new Exception("表头数据不能为空");
        //获取表头仓库
        string cwhcode = null;
        for (int i = 0; i < HeadData.Rows.Count; i++)
        {
            DataRow dr1 = HeadData.Rows[i];
            if (("" + dr1["TxtName"]).Equals("txt_cwhcode"))
            {
                cwhcode = dr1["TxtTag"] + "";
                break;
            }
        }
        if (string.IsNullOrEmpty(cwhcode))
        {
            throw new Exception("未能获取到表头仓库字段");
        }

        #region //推送其他入库
        //表头传入入库类别,通过T_Parameter配置(未入库先发货入库类别)
        DataTable crkTypeDt = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, @" 
                select top 1  t1.cRdCode,t1.cRdName from " + dbname + @"..Rd_Style t1 
                inner join " + dbname + @"..T_Parameter t2 on t1.cRdCode=t2.cValue
                where t2.cPID ='TimelycRdCode'
                ");
        if (crkTypeDt.Rows.Count == 0)
        {
            throw new Exception("未设置{未入库先发货入库类别},或参数表中没有设置{未入库先发货入库类别}TimelycRdCode");
        }

        DataTable dtHead = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' LabelText,'' TxtName,'' TxtTag,'' TxtValue where 1=0");
        DataRow dr = dtHead.NewRow();
        dr["LabelText"] = "仓库";  //栏目标题
        dr["TxtName"] = "txt_cwhcode";   //txt_字段名称
        dr["TxtTag"] = cwhcode;  //标识
        dr["TxtValue"] = cwhcode;  //栏目值
        dtHead.Rows.Add(dr);
        dr = dtHead.NewRow();
        dr["LabelText"] = "入库类别";  //栏目标题
        dr["TxtName"] = "txt_crdcode";   //txt_字段名称
        dr["TxtTag"] = crkTypeDt.Rows[0][0];  //标识
        dr["TxtValue"] = crkTypeDt.Rows[0][1];  //栏目值
        dtHead.Rows.Add(dr);
        dtHead.PrimaryKey = new System.Data.DataColumn[] { dtHead.Columns["TxtName"] };

        DataTable dtBody = UCGridCtl.SqlDBCommon.GetDataFromDB(Cmd, "select '' cbarcodetxt,'' cbatch,'' cinvcode,'' cinvname, '' cinvstd, '' cposcode, 0 iquantity,'' itransid,0 modid where 1=0");
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            DataRow bodydr = dtBody.NewRow();
            bodydr["cinvcode"] = BodyData.Rows[i]["cinvcode"]; //存货编码
            bodydr["iquantity"] = BodyData.Rows[i]["iquantity"]; //数量
            bodydr["cposcode"] = ""; //货位必须有，值可以是空
            bodydr["cbatch"] = BodyData.Rows[i]["cbatch"] + ""; //批号最好给固定值

            bodydr["modid"] = BodyData.Rows[i]["modid"];
            dtBody.Rows.Add(bodydr);
        }

        string rkCode = U81019(dtHead, dtBody, dbname, cUserName, cLogDate, "U81019", Cmd);
        if (string.IsNullOrEmpty(rkCode))
        {
            throw new Exception("生成其他入库单失败,未能获得反馈的入库单据号");
        }
        #endregion

        //推送发货单
        string RetCode = U81017(HeadData, BodyData, dbname, cUserName, cLogDate, "U81020", Cmd);
        if (RetCode == "") throw new Exception("出库失败,未能获得反馈的发货单据号");

        for (int i = 0; i < dtBody.Rows.Count; i++)
        {
            if (U8Operation.GetDataInt("select count(*) from " + dbname + @"..T_CC_CQCC_AutoRd08_Relation where ctype='调拨' and vouchid='" + RetCode.Split(',')[0] + "'", Cmd) == 0)
            {
                Cmd.CommandText = "insert into " + dbname + @"..T_CC_CQCC_AutoRd08_Relation(ctype,vouchid,rd08id,modid,qty_in,qty_out) 
                values('调拨'," + RetCode.Split(',')[0] + "," + rkCode.Split(',')[0] + "," + dtBody.Rows[i]["modid"] + "," + dtBody.Rows[i]["iquantity"] + ",0)";
                Cmd.ExecuteNonQuery();
            }
        }
        return RetCode;
    }

    //报检单
    private string U81108(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        string errmsg = "";
        string[] txtdata = null;  //临时取数
        string modid = ""; //上游单据行ID
        string moid = "";//上游单据ID
        string mocode = "";//上游单据号
        string checktypecode = "";//检验类型  ARR-采购检验,SUB-委外检验,PRO-产品检验
        string SoureVouch = "";  //上游单据类型
        string depCode = "";//部门

        if (BodyData == null || BodyData.Rows.Count == 0) throw new Exception("请传入行数据");
        string c_qmvoutype = GetTextsFrom_FormData_Text(HeadData, "txt_cvouchtype");   //报检单类型  CVOUCHTYPE 
        if (c_qmvoutype == "") throw new Exception("报检单据类型不能为空！栏目[cvouchtype]");

        #region  //逻辑检验
        if (!BodyData.Columns.Contains("sourceautoid")) throw new Exception("报检单必须有上游单据，栏目[sourceautoid]必须传入");
        modid = BodyData.Rows[0]["sourceautoid"] + "";

        depCode = GetTextsFrom_FormData_Text(HeadData, "txt_cdepcode");
        #region //检验类型
        if (c_qmvoutype == "QM01") //来料检验(到货单)
        {
            moid = U8Operation.GetDataString("select ID from " + dbname + "..PU_ArrivalVouchs where autoid=0" + modid, Cmd);
            mocode = U8Operation.GetDataString("select ccode from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd);
            SoureVouch = "到货单";

            string pu_bustype = U8Operation.GetDataString("select cbustype from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd);
            if (pu_bustype == "普通采购")
            {
                checktypecode = "ARR";
            }
            else if (pu_bustype == "委外加工")
            {
                checktypecode = "SUB";
            }
            else
            {
                throw new Exception("到货单业务类型[" + pu_bustype + "]不支持本接口");
            }
            if (depCode == "") depCode = U8Operation.GetDataString("select cdepcode from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd);
        }
        else if (c_qmvoutype == "QM02") //完工入库检验(到货单)
        {
            moid = U8Operation.GetDataString("select moid from " + dbname + "..mom_orderdetail where autoid=0" + modid, Cmd);
            mocode = U8Operation.GetDataString("select mocode from " + dbname + "..mom_order where id=0" + moid, Cmd);
            SoureVouch = "生产订单";
            checktypecode = "PRO";
            if (depCode == "") depCode = U8Operation.GetDataString("select MDeptCode from " + dbname + "..mom_orderdetail where id=0" + moid, Cmd);
        }
        else
        {
            throw new Exception("不支持此报检类型，请确保传入栏目[cvouchtype]内容");
        }
        #endregion

        #region //主表逻辑校验
        //部门
        txtdata = GetTextsFrom_FormData(HeadData, "txt_cdepcode");
        if (depCode.CompareTo("") == 0)
        {
            throw new Exception("部门 必须录入");
        }
        else
        {
            if (int.Parse(U8Operation.GetDataString("select count(*) from " + dbname + "..department where cdepcode='" + depCode + "'", Cmd)) == 0)
            {
                throw new Exception("部门 录入不正确");
            }
        }

        //自定义项 tag值不为空，但Text为空 的情况判定
        System.Data.DataTable dtDefineCheck = U8Operation.GetSqlDataTable("select t_fieldname from " + dbname + "..T_CC_Base_GridCol_rule where SheetID='" + SheetID + @"' 
                and (t_fieldname like '%define%' or t_fieldname like '%free%')", "dtDefineCheck", Cmd);
        for (int i = 0; i < dtDefineCheck.Rows.Count; i++)
        {
            string cc_colname = "" + dtDefineCheck.Rows[i]["t_fieldname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
            if (txtdata == null) continue;

            if (txtdata[3].CompareTo("") != 0 && txtdata[2].CompareTo("") == 0)
            {
                throw new Exception(txtdata[0] + "录入不正确,不符合专用规则");
            }
        }

        //检查单据必录项
        System.Data.DataTable dtMustInputCol = U8Operation.GetSqlDataTable("select t_colname,t_colshow from " + dbname + "..T_CC_Base_GridColShow where SheetID='" + SheetID + @"' 
                and isnull(t_must_input,0)=1 and UserID=''", "dtMustInputCol", Cmd);
        for (int i = 0; i < dtMustInputCol.Rows.Count; i++)
        {
            string cc_colname = "" + dtMustInputCol.Rows[i]["t_colname"];
            txtdata = GetTextsFrom_FormData(HeadData, "txt_" + cc_colname);
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

        //显示标题   txt_字段名称    文本Tag     文本Text值
        if (!BodyData.Columns.Contains("iquantity")) throw new Exception("模板设置：报检数量 栏目必须设置成可视栏目,必录"); 

        //生产订单审核日期 比对，入库日期必须大于等于审核日期
        if (c_qmvoutype == "QM02")
        {
            string mo_order_reldate = U8Operation.GetDataString("select convert(varchar(10),isnull(RelsDate,'2000-01-01'),120) from " + dbname + "..mom_orderdetail where MoDId=0" + modid, Cmd);
            if (mo_order_reldate.CompareTo(cLogDate) > 0) throw new Exception("报检日期不能小于生产订单的审核日期");
        }
        #endregion

        //取报检单的 最小模板ID  
        int vt_id = U8Operation.GetDataInt("select isnull(min(DEF_ID),0) from " + dbname + "..Vouchers_base where CardNumber='" + c_qmvoutype + "'", Cmd);
        string DBName = dbname;
        string targetAccId = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);
        #endregion

        #region//写主表
        KK_U8Com.U8QMINSPECTVOUCHER istmain = new KK_U8Com.U8QMINSPECTVOUCHER(Cmd, dbname);
        //最大编号处理
        if (U8Operation.GetDataInt("select COUNT(*) from " + DBName + "..T_CC_Voucher_Num where voucher_name='qminspect'", Cmd) == 0)
        {
            Cmd.CommandText = "insert into " + DBName + "..T_CC_Voucher_Num(voucher_name,chead,cdigit_1,cdigit_2) values('qminspect','QR',0,0)";
            Cmd.ExecuteNonQuery();
            string c_max_code = U8Operation.GetDataString("select cast(isnull(max(replace(CINSPECTCODE,'QR','')),'0') as int)+5 from " + DBName + "..QMINSPECTVOUCHER where CINSPECTCODE like 'QR%'", Cmd);
            Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=0" + c_max_code + " where voucher_name='qminspect' and chead='QR'";
            Cmd.ExecuteNonQuery();
        }

        string vouCode = U8Operation.GetDataString("select chead+right('0000000000'+cast(cdigit_1+1 as varchar(20)),8) from " + DBName + "..T_CC_Voucher_Num with(rowlock) where voucher_name='qminspect' and chead='QR'", Cmd);
        Cmd.CommandText = "update " + DBName + "..T_CC_Voucher_Num set cdigit_1=cdigit_1+1 where voucher_name='qminspect' and chead='QR'";
        Cmd.ExecuteNonQuery();
        istmain.CINSPECTCODE = "'" + vouCode + "'";
        string cnewid = U8Operation.GetDataString("select newid()", Cmd);
        istmain.INSPECTGUID = "'" + cnewid + "'";
        istmain.ID = U8Operation.GetDataString("select isnull(max(id),0)+1 from " + DBName + "..QMINSPECTVOUCHER", Cmd);
        string rd_max_id = istmain.ID + "";
        istmain.CVOUCHTYPE = "'" + c_qmvoutype + "'";
        istmain.CDEPCODE = "'" + depCode + "'";
        istmain.CMAKER = "'" + cUserName + "'";
        istmain.DDATE = "'" + cLogDate + "'";
        istmain.CTIME = "'" + U8Operation.GetDataString("select right(convert(varchar(20),getdate(),120),8)", Cmd) + "'";
        istmain.CSOURCE = "'"+SoureVouch+"'";
        istmain.IVTID = vt_id + "";
        istmain.CCHECKTYPECODE = "'" + checktypecode + "'";
        istmain.DMAKETIME = "getdate()";
        istmain.iPrintCount = "0";

        if (c_qmvoutype == "QM01")
        {
            istmain.CVENCODE = "'" + U8Operation.GetDataString("select cvencode from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd) + "'";
            istmain.DARRIVALDATE = "'" + U8Operation.GetDataString("select ddate from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd) + "'";
            istmain.CSOURCECODE = "'" + U8Operation.GetDataString("select cCode from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd) + "'";
            istmain.CSOURCEID = moid + "";
        }
        else if (c_qmvoutype == "QM02")
        {
            
        }
        istmain.CSOORDERCODE = "'" + mocode + "'"; //生产订单号
        istmain.CSOURCEID = "" + moid;  //生产订单  主表标识

        //mes_flow_inspect_autocheck  是否自动审核报检单
        string rd_auto_check = "" + U8Operation.GetDataString("select cValue from " + DBName + "..T_Parameter where cPid='mes_flow_inspect_autocheck'", Cmd);
        if (rd_auto_check.ToLower().CompareTo("true") == 0)
        {
            istmain.CVERIFIER = "'" + cUserName + "'";
            istmain.DVERIFYDATE = "'" + cLogDate + "'";
        }

        #region //主表自定义项
        istmain.CDEFINE1 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define1") + "'";
        istmain.CDEFINE2 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define2") + "'";
        istmain.CDEFINE3 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define3") + "'";
        istmain.CDEFINE4 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define4") + "'";
        string txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_define5");
        istmain.CDEFINE5 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istmain.CDEFINE6 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define6") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_define7");
        istmain.CDEFINE7 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        istmain.CDEFINE8 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define8") + "'";
        istmain.CDEFINE9 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define9") + "'";
        istmain.CDEFINE10 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define10") + "'";
        istmain.CDEFINE11 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define11") + "'";
        istmain.CDEFINE12 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define12") + "'";
        istmain.CDEFINE13 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define13") + "'";
        istmain.CDEFINE14 = "'" + GetTextsFrom_FormData_Text(HeadData, "txt_define14") + "'";
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_define15");
        istmain.CDEFINE15 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        txtdata_text = GetTextsFrom_FormData_Text(HeadData, "txt_define16");
        istmain.CDEFINE16 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
        #endregion

        //查找入库单号是否重复
        if (U8Operation.GetDataInt("select count(*) from " + DBName + "..QMINSPECTVOUCHER(nolock) where CINSPECTCODE=" + istmain.CINSPECTCODE, Cmd) > 0)
            throw new Exception("单据号存储冲突，请重新点击保存");

        if (!istmain.InsertToDB(ref errmsg)) { throw new Exception(errmsg); }

        //创建产品报检单条码
        Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHER set csysbarcode='||QMCB|" + istmain.CINSPECTCODE.Replace("'", "") + "' where id=" + istmain.ID;
        Cmd.ExecuteNonQuery();

        #endregion

        #region  //子表
        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            KK_U8Com.U8QMINSPECTVOUCHERS istdetail = new KK_U8Com.U8QMINSPECTVOUCHERS(Cmd, dbname);

            #region //行逻辑校验
            modid = BodyData.Rows[i]["sourceautoid"] + "";
            if (c_qmvoutype == "QM01") //来料检验(到货单)
            {
                moid = U8Operation.GetDataString("select ID from " + dbname + "..PU_ArrivalVouchs where autoid=0" + modid, Cmd);
                mocode = U8Operation.GetDataString("select ccode from " + dbname + "..PU_ArrivalVouch where id=0" + moid, Cmd);

                //判断生产订单是否关闭
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs(nolock) where autoid=0" + modid + " and isnull(cbcloser,'')<>''", Cmd) > 0)
                    throw new Exception("到货单已经关闭");
                //判断是否勾选 质检标识
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..PU_ArrivalVouchs(nolock) where autoid=0" + modid + " and bGsp=0", Cmd) > 0)
                    throw new Exception("本到货单 不能走检验流程");
                istdetail.CPOCODE = "'" + U8Operation.GetDataString("select cordercode from " + dbname + "..PU_ArrivalVouchs where autoid=0" + modid, Cmd) + "'";//采购订单号/委外
            }
            else if (c_qmvoutype == "QM02") //完工入库检验(到货单)
            {
                moid = U8Operation.GetDataString("select moid from " + dbname + "..mom_orderdetail where autoid=0" + modid, Cmd);
                mocode = U8Operation.GetDataString("select mocode from " + dbname + "..mom_order where id=0" + moid, Cmd);
                //判断生产订单是否关闭
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..mom_orderdetail(nolock) where modid=0" + modid + " and isnull(CloseUser,'')<>''", Cmd) > 0)
                    throw new Exception("生产订单已经关闭");
                //判断是否勾选 质检标识
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..mom_orderdetail(nolock) where modid=0" + modid + " and QcFlag=0", Cmd) > 0)
                    throw new Exception("本生产订单 不能走检验流程");

                istdetail.IPROORDERAUTOID = U8Operation.GetDataString("select sortseq from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd);   //生产订单行号
                istdetail.CPROBATCH = "'" + U8Operation.GetDataString("select MoLotCode from " + DBName + "..mom_orderdetail where modid=" + modid, Cmd) + "'";
                istdetail.CPROORDERCODE = "'" + mocode + "'";   //生产订单号
            }
            decimal f_Qty_in = decimal.Parse(BodyData.Rows[i]["iquantity"]+"");  //报检数量
            string cc_invcode = GetBodyValue_FromData(BodyData, i, "cinvcode");
            if (cc_invcode.CompareTo("") == 0) { throw new Exception("模板设置：存货编码 栏目必须设置成可视栏目[cinvcode]"); }
            #endregion

            istdetail.AUTOID = U8Operation.GetDataString("select isnull(max(autoid),0)+1 from " + DBName + "..QMINSPECTVOUCHERS(nolock)", Cmd);
            istdetail.ID = istmain.ID;
            istdetail.SOURCEAUTOID = modid;
            istdetail.CINVCODE = "'" + cc_invcode + "'";
            istdetail.ITESTSTYLE = U8Operation.GetDataString("select isnull(iTestStyle,0) from " + DBName + "..Inventory(nolock) where cinvcode=" + istdetail.CINVCODE, Cmd);
            //modid,mocode,soseq,cinvcode,cinvname,cinvstd,cunitname,balqualifiedqty,cbatch,bomtype
            istdetail.FQUANTITY = "" + f_Qty_in;  //报检数量
            istdetail.CBYPRODUCT = "'0'";
            istdetail.IORDERTYPE = "0";
            istdetail.IPROORDERID = moid;

            istdetail.BEXIGENCY = "0";
            istdetail.ISOURCEPROORDERID = "0";
            istdetail.ISOURCEPROORDERAUTOID = "0";
            
            istdetail.iExpiratDateCalcu = "0";   //有效期推算方式  默认为0
            //istdetail.PFCODE = "'" + card_no + "'";  //流转卡号

            #region  //自由项管理   自定义项
            istdetail.CFREE1 = "'" + GetBodyValue_FromData(BodyData, i, "free1") + "'";
            istdetail.CFREE2 = "'" + GetBodyValue_FromData(BodyData, i, "free2") + "'";
            istdetail.CFREE3 = "'" + GetBodyValue_FromData(BodyData, i, "free3") + "'";
            istdetail.CFREE4 = "'" + GetBodyValue_FromData(BodyData, i, "free4") + "'";
            istdetail.CFREE5 = "'" + GetBodyValue_FromData(BodyData, i, "free5") + "'";
            istdetail.CFREE6 = "'" + GetBodyValue_FromData(BodyData, i, "free6") + "'";
            istdetail.CFREE7 = "'" + GetBodyValue_FromData(BodyData, i, "free7") + "'";
            istdetail.CFREE8 = "'" + GetBodyValue_FromData(BodyData, i, "free8") + "'";
            istdetail.CFREE9 = "'" + GetBodyValue_FromData(BodyData, i, "free9") + "'";
            istdetail.CFREE10 = "'" + GetBodyValue_FromData(BodyData, i, "free10") + "'";

            istdetail.CDEFINE22 = "'" + GetBodyValue_FromData(BodyData, i, "define22") + "'";
            istdetail.CDEFINE23 = "'" + GetBodyValue_FromData(BodyData, i, "define23") + "'";
            istdetail.CDEFINE24 = "'" + GetBodyValue_FromData(BodyData, i, "define24") + "'";
            istdetail.CDEFINE25 = "'" + GetBodyValue_FromData(BodyData, i, "define25") + "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define26");
            istdetail.CDEFINE26 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define27");
            istdetail.CDEFINE27 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            istdetail.CDEFINE28 = "'" + GetBodyValue_FromData(BodyData, i, "define28") + "'";
            istdetail.CDEFINE29 = "'" + GetBodyValue_FromData(BodyData, i, "define29") + "'";
            istdetail.CDEFINE30 = "'" + GetBodyValue_FromData(BodyData, i, "define30") + "'";
            istdetail.CDEFINE31 = "'" + GetBodyValue_FromData(BodyData, i, "define31") + "'";
            istdetail.CDEFINE32 = "'" + GetBodyValue_FromData(BodyData, i, "define32")+ "'";
            istdetail.CDEFINE33 = "'" + GetBodyValue_FromData(BodyData, i, "define33")+ "'";
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define34");
            istdetail.CDEFINE34 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            txtdata_text = GetBodyValue_FromData(BodyData, i, "define35");
            istdetail.CDEFINE35 = (txtdata_text.CompareTo("") == 0 ? "0" : txtdata_text);
            istdetail.CDEFINE36 = "'" + GetBodyValue_FromData(BodyData, i, "define36") + "'";
            istdetail.CDEFINE37 = "'" + GetBodyValue_FromData(BodyData, i, "define37") + "'";
            #endregion

            string cToday = U8Operation.GetDataString("select convert(varchar(10),'" + cLogDate + "',120)", Cmd);
            #region//批次管理和保质期管理
            string cc_batch = GetBodyValue_FromData(BodyData, i, "cbatch");
            if (int.Parse(U8Operation.GetDataString("select count(*) FROM " + DBName + "..inventory where cinvcode='" + cc_invcode + "' and bInvBatch=1", Cmd)) > 0)
            {
                if (cc_batch.CompareTo("") == 0) throw new Exception("有批次管理，请输入批号");
                istdetail.CBATCH = "'" + cc_batch + "'";
                System.Data.DataTable dtBZQ = new DataTable();//生产日期 保质期
                if (c_qmvoutype == "QM01") //来料检验(到货单)
                {
                    dtBZQ = U8Operation.GetSqlDataTable(@"select 1 是否保质期,imassdate 保质期天数,cmassunit 保质期单位,dPDate 生产日期,
                                                            dVDate 失效日期,iExpiratDateCalcu 有效期推算方式
                                                            from " + dbname + "..PU_ArrivalVouchs where autoid=0" + modid, "dtBZQ", Cmd);
                }
                else if (c_qmvoutype == "QM02")
                {
                    dtBZQ = U8Operation.GetSqlDataTable(@"select cast(bInvQuality as int) 是否保质期,iMassDate 保质期天数,cMassUnit 保质期单位,'" + cToday + @"' 生产日期,
                                                            convert(varchar(10),(case when cMassUnit=1 then DATEADD(year,iMassDate,'" + cToday + @"')
	                                                                            when cMassUnit=2 then DATEADD(month,iMassDate,'" + cToday + @"')
	                                                                            else DATEADD(day,iMassDate,'" + cToday + @"') end)
                                                            ,120) 失效日期,s.iExpiratDateCalcu 有效期推算方式
                                                            from " + DBName + "..inventory i left join " + DBName + @"..Inventory_Sub s on i.cinvcode=s.cinvsubcode 
                                                            where cinvcode='" + cc_invcode + "' and bInvQuality=1", "dtBZQ", Cmd);
                }
                if (dtBZQ != null && dtBZQ.Rows.Count > 0)
                {
                    istdetail.iExpiratDateCalcu = dtBZQ.Rows[0]["有效期推算方式"] + "";
                    istdetail.IMASSDATE = dtBZQ.Rows[0]["保质期天数"] + "";
                    istdetail.DVDATE = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    istdetail.cExpirationdate = "'" + dtBZQ.Rows[0]["失效日期"] + "'";
                    istdetail.DPRODATE = "'" + dtBZQ.Rows[0]["生产日期"] + "'";
                    istdetail.CMASSUNIT = "'" + dtBZQ.Rows[0]["保质期单位"] + "'";
                }
                if (istdetail.IMASSDATE == "" )
                {
                    istdetail.DPRODATE = "null";
                    istdetail.IMASSDATE = "null";
                    istdetail.cExpirationdate = "null";
                }

                #region  //是否建立批次档案
                if (U8Operation.GetDataInt("select count(*) from " + DBName + "..Inventory_Sub where cInvSubCode=" + istdetail.CINVCODE + " and bBatchCreate=1", Cmd) > 0)
                {
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty1");
                    istdetail.cBatchProperty1 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty2");
                    istdetail.cBatchProperty2 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty3");
                    istdetail.cBatchProperty3 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty4");
                    istdetail.cBatchProperty4 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty5");
                    istdetail.cBatchProperty5 = txtdata_text.CompareTo("") == 0 ? "null" : txtdata_text;
                    istdetail.cBatchProperty6 = "'" + GetBodyValue_FromData(BodyData, i, "cBatchProperty6") + "'";
                    istdetail.cBatchProperty7 = "'" + GetBodyValue_FromData(BodyData, i, "cBatchProperty7") + "'";
                    istdetail.cBatchProperty8 = "'" + GetBodyValue_FromData(BodyData, i, "cBatchProperty8") + "'";
                    istdetail.cBatchProperty9 = "'" + GetBodyValue_FromData(BodyData, i, "cBatchProperty9")+ "'";
                    txtdata_text = GetBodyValue_FromData(BodyData, i, "cBatchProperty10");
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
            #region  //补充信息
            //创建子表条码
            Cmd.CommandText = @"update " + DBName + "..QMINSPECTVOUCHERS set cbsysbarcode='||QMLB|" + istmain.CINSPECTCODE.Replace("'", "") + "|1' where autoid=" + istdetail.AUTOID;
            Cmd.ExecuteNonQuery();
            #endregion 

            //回写
            if (c_qmvoutype == "QM01")
            {
                //到货单
                Cmd.CommandText = "update " + DBName + "..PU_ArrivalVouchs set fInspectQuantity=isnull(fInspectQuantity,0)+(0" + istdetail.FQUANTITY + @"),
                    fInspectNum=isnull(fInspectNum,0)+isnull(" + istdetail.FNUM + @",0),bInspect=1 where autoid=0" + modid;
                Cmd.ExecuteNonQuery();

                //超到货单检查
                decimal fRkqty = decimal.Parse(U8Operation.GetDataString(@"select isnull(sum(a.fquantity),0) from " + dbname + @"..QMINSPECTVOUCHERS a(nolock)
                inner join " + dbname + @"..QMINSPECTVOUCHER b(nolock) on a.ID=b.ID and b.CVOUCHTYPE='QM01'
                where a.SOURCEAUTOID=0" + modid, Cmd));

                //判断是否生产订单入库  0代表不能超   1 代表可超
                if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bOverMPIn'", Cmd) == 0)
                {
                    //关键领用控制量，取最小值  
                    decimal f_ll_qty = decimal.Parse(U8Operation.GetDataString(@"select iquantity from " + DBName + "..PU_ArrivalVouchs(nolock) where autoid=0" + modid, Cmd));
                    if (f_ll_qty < fRkqty) throw new Exception("存货【" + istdetail.CINVCODE + "】超到货单报检");
                }
            }
            else if (c_qmvoutype == "QM02")
            {
                //生产订单
                Cmd.CommandText = "update " + DBName + "..mom_orderdetail set DeclaredQty=isnull(DeclaredQty,0)+(0" + istdetail.FQUANTITY + ") where modid=0" + modid;
                Cmd.ExecuteNonQuery();

                #region//是否超生产订单检查
                decimal fRkqty = decimal.Parse(U8Operation.GetDataString(@"select isnull(sum(a.fquantity),0) from " + dbname + @"..QMINSPECTVOUCHERS a(nolock)
                inner join " + dbname + @"..QMINSPECTVOUCHER b(nolock) on a.ID=b.ID and b.CVOUCHTYPE='QM02'
                where a.SOURCEAUTOID=0" + modid, Cmd));

                //判断是否生产订单入库  0代表不能超   1 代表可超
                if (U8Operation.GetDataInt("select CAST(cast(cvalue as bit) as int) from " + DBName + "..AccInformation where cSysID='st' and cname='bOverMPIn'", Cmd) == 0)
                {
                    //关键领用控制量，取最小值  
                    decimal f_ll_qty = decimal.Parse(U8Operation.GetDataString(@"select qty from " + DBName + "..mom_orderdetail where modid=0" + modid, Cmd));
                    if (f_ll_qty < fRkqty) throw new Exception("存货【" + istdetail.CINVCODE + "】超生产订单报检");
                }
                #endregion
            }

            

        }
        #endregion

        //throw new Exception("来料自动报检测试完成");
        return istmain.ID + "," + vouCode;
    }

    //调拨单（调拨申请，支持委外调拨  生产订单调拨逻辑）
    private string U81109(DataTable HeadData, DataTable BodyData, string dbname, string cUserName, string cLogDate, string SheetID, SqlCommand Cmd)
    {
        if (BodyData.Rows.Count == 0) throw new Exception("无扫码表体数据");
        string c_shen_code = GetTextsFrom_FormData_Text(HeadData, "txt_mavouchcode");
        if (c_shen_code == "") c_shen_code = GetBodyValue_FromData(BodyData, 0, "app_voucode");
        if (c_shen_code == "") throw new Exception("调拨申请出库，必须扫描申请单条码");
        DataTable dtShenQing = U8Operation.GetSqlDataTable(@"select id,cTVCode,cOWhCode,cIWhCode,cIRdCode,cORdCode,cIDepCode,cODepCode,cPersonCode,cSource from " + dbname + @"..ST_AppTransVouch 
            where csysbarcode ='" + c_shen_code + "' or cTVCode ='" + c_shen_code + "'", "dtShenQing", Cmd);
        if (dtShenQing.Rows.Count == 0) throw new Exception("无法找到申请单");

        #region //继承申请单的 表头栏目
        if (dtShenQing.Rows[0]["cOWhCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cwhcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "调出仓库"; //栏目标题 
                dr["TxtName"] = "txt_cwhcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cOWhCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cOWhCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cwhcode", dtShenQing.Rows[0]["cOWhCode"] + "", dtShenQing.Rows[0]["cOWhCode"] + "");
            }
        }

        if (dtShenQing.Rows[0]["cIWhCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cinwhcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "调入仓库"; //栏目标题 
                dr["TxtName"] = "txt_cinwhcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cIWhCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cIWhCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cinwhcode", dtShenQing.Rows[0]["cIWhCode"] + "", dtShenQing.Rows[0]["cIWhCode"] + "");
            }
        }

        if (dtShenQing.Rows[0]["cORdCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_crdcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "出库类别"; //栏目标题 
                dr["TxtName"] = "txt_crdcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cORdCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cORdCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_crdcode", dtShenQing.Rows[0]["cORdCode"] + "", dtShenQing.Rows[0]["cORdCode"] + "");
            }
        }

        if (dtShenQing.Rows[0]["cIRdCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cinrdcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "入库类别"; //栏目标题 
                dr["TxtName"] = "txt_cinrdcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cIRdCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cIRdCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cinrdcode", dtShenQing.Rows[0]["cIRdCode"] + "", dtShenQing.Rows[0]["cIRdCode"] + "");

            }
        }

        if (dtShenQing.Rows[0]["cODepCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cdepcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "出库部门"; //栏目标题 
                dr["TxtName"] = "txt_cdepcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cODepCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cODepCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cdepcode", dtShenQing.Rows[0]["cODepCode"] + "", dtShenQing.Rows[0]["cODepCode"] + "");
            }
        }

        if (dtShenQing.Rows[0]["cIDepCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cindepcode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "入库部门"; //栏目标题 
                dr["TxtName"] = "txt_cindepcode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cIDepCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cIDepCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cindepcode", dtShenQing.Rows[0]["cIDepCode"] + "", dtShenQing.Rows[0]["cIDepCode"] + "");
            }
        }


        if (dtShenQing.Rows[0]["cPersonCode"] + "" != "")
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cpersoncode");
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "业务员"; //栏目标题 
                dr["TxtName"] = "txt_cpersoncode";   //txt_字段名称
                dr["TxtTag"] = dtShenQing.Rows[0]["cPersonCode"] + "";  //标识
                dr["TxtValue"] = dtShenQing.Rows[0]["cPersonCode"] + "";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                if (c_data[2] == "") SetTextsFrom_FormData(HeadData, "txt_cpersoncode", dtShenQing.Rows[0]["cPersonCode"] + "", dtShenQing.Rows[0]["cPersonCode"] + "");
            }
        }

        string order_type = "";
        {
            string[] c_data = GetTextsFrom_FormData(HeadData, "txt_cordertype");
            order_type = "生产订单";
            if (c_data == null)
            {
                DataRow dr = HeadData.NewRow();
                dr["LabelText"] = "订单类型"; //栏目标题 
                dr["TxtName"] = "txt_cordertype";   //txt_字段名称
                dr["TxtTag"] = "生产订单";  //标识
                dr["TxtValue"] = "生产订单";//栏目值
                HeadData.Rows.Add(dr);
            }
            else
            {
                order_type = c_data[2];
                if (c_data[2] == "") { SetTextsFrom_FormData(HeadData, "txt_cordertype", "生产订单", "生产订单"); order_type = "生产订单"; }
            }
        }
        #endregion

        #region //表体栏目转换 (按照顺序分解)  cordertype
        //BodyData 排序  存货编码
        BodyData.DefaultView.Sort = "cinvcode ASC";
        BodyData = BodyData.DefaultView.ToTable();

        //按照顺序匹配
        DataTable dtShData = null;
        string c_cinvcode = "";
        int i_app_row = 0;
        decimal d_app_row_num = 0;
        if (!BodyData.Columns.Contains("itrids")) BodyData.Columns.Add("itrids");  //增加申请单
        if (!BodyData.Columns.Contains("allocateid")) BodyData.Columns.Add("allocateid");
        if (!BodyData.Columns.Contains("modid")) BodyData.Columns.Add("modid"); 
        BodyData.Columns["itrids"].ReadOnly = false;
        BodyData.Columns["allocateid"].ReadOnly = false;
        BodyData.Columns["modid"].ReadOnly = false;
        DataTable dtChaiData = BodyData.Clone();  //记录拆下来的数据
        DataTable dtSaveGrid = BodyData.Clone();  //记录最终保存的行数据

        for (int i = 0; i < BodyData.Rows.Count; i++)
        {
            if (BodyData.Rows[i]["itrids"] + "" != "") break ;  //根据申请单行条码扫码，无需拆行业务处理
            if (c_cinvcode != "" + BodyData.Rows[i]["cinvcode"])
            {
                i_app_row = 0;
                c_cinvcode = "" + BodyData.Rows[i]["cinvcode"];
                if (order_type == "生产订单")
                {
                    dtShData = U8Operation.GetSqlDataTable(@"select a.cinvcode,a.AutoID,c.allocateid,c.modid,a.iTVQuantity-ISNULL(a.iTvSumQuantity,0) ilastqty 
                    from " + dbname + @"..ST_AppTransVouchs a left join " + dbname + @"..T_CC_Base_DbInfo b on a.autoid=b.dbid and isnull(b.iordertype,1)=1
                    left join " + dbname + @"..mom_moallocate c on b.imoids=c.AllocateId
                    where a.ID=0" + dtShenQing.Rows[0]["id"] + " and a.cinvcode='" + c_cinvcode + "' and a.iTVQuantity-ISNULL(a.iTvSumQuantity,0)>0 order by a.irowno", "dtShData", Cmd);
                }
                else  //委外订单
                {
                    dtShData = U8Operation.GetSqlDataTable(@"select a.cinvcode,a.AutoID,c.MOMaterialsID allocateid,c.MoDetailsID modid,a.iTVQuantity-ISNULL(a.iTvSumQuantity,0) ilastqty 
                    from " + dbname + @"..ST_AppTransVouchs a left join " + dbname + @"..T_CC_Base_DbInfo b on a.autoid=b.dbid and isnull(b.iordertype,1)=2
                    left join " + dbname + @"..OM_MOMaterials c on b.imoids=c.AllocateId
                    where a.ID=0" + dtShenQing.Rows[0]["id"] + " and a.cinvcode='" + c_cinvcode + "' and a.iTVQuantity-ISNULL(a.iTvSumQuantity,0)>0 order by a.irowno", "dtShData", Cmd);
                }
                if (dtShData.Rows.Count == 0) throw new Exception("本调拨申请单 存货【" + c_cinvcode + "】未找到可调拨信息");
            }
            decimal dvalue = decimal.Parse("" + BodyData.Rows[i]["iquantity"]);
            //循环比较 
            for (int k = i_app_row; k < dtShData.Rows.Count; k++)
            {
                i_app_row = k;
                d_app_row_num = decimal.Parse("" + dtShData.Rows[i_app_row]["ilastqty"]);

                if (dvalue <= d_app_row_num)
                {
                    d_app_row_num = d_app_row_num - dvalue;
                    BodyData.Rows[i]["itrids"] = "" + dtShData.Rows[i_app_row]["AutoID"];  //申请单ID
                    BodyData.Rows[i]["allocateid"] = "" + dtShData.Rows[i_app_row]["allocateid"];
                    BodyData.Rows[i]["modid"] = "" + dtShData.Rows[i_app_row]["modid"];

                    if (d_app_row_num == 0)
                    {
                        i_app_row = k + 1;
                    }
                    else
                    {
                        dtShData.Rows[i_app_row]["ilastqty"] = d_app_row_num;  //修改数量
                    }
                    dvalue = 0;

                    dtSaveGrid.Rows.Add(BodyData.Rows[i].ItemArray); //增加行数据
                    break;  //退出循环
                }
                else
                {
                    //拆行
                    DataRow c_DR = dtChaiData.Rows.Add(BodyData.Rows[i].ItemArray);
                    c_DR["iquantity"] = d_app_row_num;
                    c_DR["itrids"] = dtShData.Rows[i_app_row]["AutoID"];  //申请单ID
                    c_DR["allocateid"] = "" + dtShData.Rows[i_app_row]["allocateid"];
                    c_DR["modid"] = "" + dtShData.Rows[i_app_row]["modid"];
                    dtSaveGrid.Rows.Add(c_DR.ItemArray); //增加行数据

                    dvalue = dvalue - d_app_row_num;
                    BodyData.Rows[i]["iquantity"] = dvalue;
                }
            }
            //未拆分完毕，有剩余 
            //if (dvalue > 0) throw new Exception("存货[" + c_cinvcode + "]超出调拨申请单可调拨量");
            //未拆分完毕时，直接调拨，不与订单关联
            if (dvalue > 0)  //申请单不够拆，剩余
            {
                BodyData.Rows[i]["itrids"] = "" + dtShData.Rows[i_app_row]["AutoID"];  //申请单ID
                BodyData.Rows[i]["allocateid"] = "";
                BodyData.Rows[i]["modid"] = "";
                dvalue = 0;
                dtSaveGrid.Rows.Add(BodyData.Rows[i].ItemArray); //增加行数据
            }
        }

        ////合并行拆分行
        //for (int k = 0; k < dtChaiData.Rows.Count; k++)
        //{ BodyData.Rows.Add(dtChaiData.Rows[k].ItemArray); }
        #endregion

        //推送完成出库
        string cdbcode = U81035(HeadData, dtSaveGrid, dbname, cUserName, cLogDate, "U81035", Cmd);
        if (cdbcode == "") throw new Exception("调拨失败,未能获得反馈的调拨单据号");

        return cdbcode;

    }

    //datatable转成JSON字符串
    public static string ToJson(DataTable dt)
    {
        int count = dt.Rows.Count;
        //将DataTable格式的数据转换成json格式
        StringBuilder jsonBuilder = new StringBuilder();
        //下边这一行为添加字符使得满足layui的json格式要求，不用layui的话不需要有下边这一行的，可以看一下输出的字符自己再改改
        jsonBuilder.Append("{\"code\": 0, \"msg\":\"成功\",\"count\": " + count + ",\"data\":");
        jsonBuilder.Append("[");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            jsonBuilder.Append("{");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dt.Columns[j].ColumnName);
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dt.Rows[i][j].ToString());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("},");
        }
        if (dt.Rows.Count > 0)
        {
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
        }
        jsonBuilder.Append("]}");
        string json = jsonBuilder.ToString();
        //throw new Exception(json);
        return json;
    }

}

