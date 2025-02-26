using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// PDMInterface 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PDMInterface : System.Web.Services.WebService
{

    public PDMInterface()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]  //存货档案同步
    public bool SendInvBaseData(System.Data.DataTable dtItemList, string dbname)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误");
        #endregion

        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            string cUnitCode = "";
            string cClassLen=U8Operation.GetDataString("select cValue from " + dbname + "..T_Parameter where cPid='u8pdm_inv_source_type'", Cmd);
            if (cClassLen == "") throw new Exception("请维护参数[同步源存货档案规则：维护存货分类位数，代表源档案编码]");

            int iHasext = U8Operation.GetDataInt("SELECT COUNT(*) FROM " + dbname + ".sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Inventory_extradefine]') AND type in (N'U')", Cmd);
            for (int i = 0; i < dtItemList.Rows.Count; i++)
            {
                //判断存货分类是否存在
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..InventoryClass where cinvccode='" + dtItemList.Rows[i]["itemclasscode"] + "'", Cmd) == 0)
                    throw new Exception("存货【" + dtItemList.Rows[i]["itemcode"] + "】的存货分类编码【" + dtItemList.Rows[i]["itemclasscode"] + "】不存在");

                #region //自定义项处理
                string c_def_list = ""; string c_free_list = ""; string c_batpro_list = ""; string c_Config_Free_list = ""; string c_hs_Free_list = "";
                for (int c = 0; c < dtItemList.Columns.Count; c++)
                {
                    if (dtItemList.Columns[c].ColumnName.ToLower().IndexOf("bfree") > -1)
                    {
                        c_free_list += "," + dtItemList.Columns[c].ColumnName + "='" + dtItemList.Rows[i][c] + "'";
                    }
                    else if (dtItemList.Columns[c].ColumnName.ToLower().IndexOf("bbatchproperty") > -1)
                    {
                        c_batpro_list += "," + dtItemList.Columns[c].ColumnName + "='" + dtItemList.Rows[i][c] + "'";
                    }
                    else if (dtItemList.Columns[c].ColumnName.ToLower().IndexOf("cinvdefine") > -1)
                    {
                        c_def_list += "," + dtItemList.Columns[c].ColumnName + "='" + dtItemList.Rows[i][c] + "'";
                    }
                    else if (dtItemList.Columns[c].ColumnName.ToLower().IndexOf("bconfigfree") > -1)
                    {
                        c_Config_Free_list += "," + dtItemList.Columns[c].ColumnName + "='" + dtItemList.Rows[i][c] + "'";
                    }
                    else if (dtItemList.Columns[c].ColumnName.ToLower().IndexOf("bcheckfree") > -1)
                    {
                        c_hs_Free_list += "," + dtItemList.Columns[c].ColumnName + "='" + dtItemList.Rows[i][c] + "'";
                    }
                }
                #endregion

                //判断是否存在存货
                int iInvCount = U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                if (iInvCount > 0)
                {
                    cUnitCode = "" + U8Operation.GetDataString("select cComUnitCode from " + dbname + "..Inventory where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                    string cNewUnitCode = "" + U8Operation.GetDataString(@"select top 1 ccomunitcode from " + dbname + "..ComputationUnit a inner join " + dbname +
                        "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and (a.cComUnitName = '" + dtItemList.Rows[i]["unitcode"] + "' or a.ccomunitcode = '" + dtItemList.Rows[i]["unitcode"] + "')", Cmd);
                    if (cUnitCode.CompareTo(cNewUnitCode) != 0)
                    {
                        //查看是否存在入出库记录
                        iInvCount = U8Operation.GetDataInt("select count(*) from " + dbname + "..rdrecordsview where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'", Cmd);
                        if (iInvCount > 0) throw new Exception("存货【" + dtItemList.Rows[i]["itemcode"] + "】已经使用，不能调整计量单位，原单位编码为【" + cUnitCode + "】");
                    }
                }
                else
                {
                    string cc_sorueinv = "" + dtItemList.Rows[i]["itemclasscode"]; //源存货
                    cc_sorueinv = cc_sorueinv.Substring(0, int.Parse(cClassLen));
                    cc_sorueinv = "PLM_" + cc_sorueinv; //源存货
                    
                    if (UCGridCtl.SqlDBCommon.GetDataIntFromSelect(Cmd, "select count(*) from " + dbname + "..Inventory where cinvcode='" + cc_sorueinv + "'") == 0)
                        throw new Exception("未找到分类[" + dtItemList.Rows[i]["itemclasscode"] + "]的属性源 存货编码[" + cc_sorueinv + "]");

                    //计量单位判断
                    string c_add_new_unit = "" + U8Operation.GetDataString(@"select top 1 ccomunitcode from " + dbname + "..ComputationUnit a inner join " + dbname +
                        "..ComputationGroup b on a.cGroupCode=b.cGroupCode where b.igrouptype=0 and (a.cComUnitName = '" + dtItemList.Rows[i]["unitcode"] + "' or a.ccomunitcode = '" + dtItemList.Rows[i]["unitcode"] + "')", Cmd);
                    if (c_add_new_unit == "") throw new Exception("存货[" + dtItemList.Rows[i]["itemcode"] + "]没有找到[" + dtItemList.Rows[i]["unitcode"] + "]对应的计量单位");

                    #region//复制属性
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
                            iOldpartMngRule from " + dbname + "..Inventory where cinvcode='" + cc_sorueinv + "'";
                    Cmd.ExecuteNonQuery();

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
                          bConsiderFreeStock, bSuitRetail from " + dbname + "..Inventory_Sub where cInvSubCode='" + cc_sorueinv + "'";
                    Cmd.ExecuteNonQuery();

                    string imaxpartid = "" + UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select max(partId)+1 from " + dbname + @"..bas_part");
                    Cmd.CommandText = "insert into " + dbname + @"..bas_part(PartId,InvCode, Free1, Free2, Free3, Free4, Free5, Free6, Free7, Free8, Free9, Free10, SafeQty, MinQty, MulQty, FixQty, bVirtual, DrawCode, LLC, 
                          cBasEngineerFigNo, fBasMaxSupply, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, RoundingFactor, FreeStockFlag, 
                          bFreeStop) 
                        select " + imaxpartid + ",'" + dtItemList.Rows[i]["itemcode"] + @"', Free1, Free2, Free3, Free4, Free5, Free6, Free7, Free8, Free9, Free10, SafeQty, MinQty, MulQty, FixQty, bVirtual, DrawCode, LLC, 
                          cBasEngineerFigNo, fBasMaxSupply, iSurenessType, iDateType, iDateSum, iDynamicSurenessType, iBestrowSum, iPercentumSum, RoundingFactor, FreeStockFlag, 
                          bFreeStop from " + dbname + "..bas_part where InvCode='" + cc_sorueinv + "'";
                    Cmd.ExecuteNonQuery();
                    #endregion

                    //扩展自定义项处理
                    if (iHasext > 0)
                    {
                        Cmd.CommandText = "insert into " + dbname + "..Inventory_extradefine(cInvCode) values('" + dtItemList.Rows[i]["itemcode"] + "')";
                        Cmd.ExecuteNonQuery();
                    }
                }

                //修改存货档案
                #region
                Cmd.CommandText = @"update " + dbname + "..Inventory set cinvccode='" + dtItemList.Rows[i]["itemclasscode"] + "',cinvname='" + dtItemList.Rows[i]["itemname"] + "',cinvstd='" + dtItemList.Rows[i]["itemstd"] + @"',
                        bSale=0" + dtItemList.Rows[i]["bsale"] + ",bPurchase=0" + dtItemList.Rows[i]["bpurchase"] + ",bComsume=0" + dtItemList.Rows[i]["bcomsume"] + @",
                        bSelf=0" + dtItemList.Rows[i]["bself"] + ",bProxyForeign=0" + dtItemList.Rows[i]["bproxyforeign"] + ",bPTOModel=0" + dtItemList.Rows[i]["bpto"] + @",
                        bATOModel=0" + dtItemList.Rows[i]["bato"] + ",bInvBatch=0" + dtItemList.Rows[i]["bBatch"] + c_free_list + c_def_list + c_Config_Free_list + c_hs_Free_list + @"
                    where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "'";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..Inventory_Sub set fBuyExcess=null " + c_batpro_list + " where cInvSubCode='" + dtItemList.Rows[i]["itemcode"] + "'";
                Cmd.ExecuteNonQuery();
                #endregion

                U8Operation.GetDataString("select 3", Cmd);

                //存货档案修改正 处理(自制件  采购件 等属性匹配问题)
                Cmd.CommandText = "update " + dbname + "..Inventory set bProductBill=1,bBomMain=1,bBomSub=1 where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "' and bSelf=1";
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = "update " + dbname + "..Inventory set bBomSub=1,bComsume=1 where cinvcode='" + dtItemList.Rows[i]["itemcode"] + "' and bPurchase=1";
                Cmd.ExecuteNonQuery();
            }

            U8Operation.GetDataString("select 9", Cmd);

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

    [WebMethod]  //BOM同步
    public bool SendInvBomData(System.Data.DataTable dtBomInfoList, string dbname)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误");
        #endregion

        string oldFatherCode = "";
        if (dtBomInfoList == null) throw new Exception("没有有效数据");
        if (!dtBomInfoList.Columns.Contains("warecode")) throw new Exception("请提供列 供应仓库,字段名warecode");
        bool b_has_bscount = dtBomInfoList.Columns.Contains("ibasecount");
        //dtBomInfoList.DefaultView.Sort = "cfathercode,cseqno";
        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string U8AcountID = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        if (U8Operation.GetDataInt("SELECT count(*) FROM ufsystem..UA_Identity where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'", Cmd) <= 0)
        {
            new Exception("系统BOM环境没有初始化，请在U8系统中建立一个BOM样板（建完后可删除）！");
        }


        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //排序
            dtBomInfoList.DefaultView.Sort = "cfathercode,cseqno";
            dtBomInfoList = dtBomInfoList.DefaultView.ToTable();

            //检查数据逻辑
            #region
            U8Operation.GetDataString("select 'startBOM'", Cmd);
            U8Operation.GetDataString("select 1[" + dtBomInfoList.Rows.Count + "]", Cmd);
            for (int i = 0; i < dtBomInfoList.Rows.Count; i++)
            {
                U8Operation.GetDataString("select 2", Cmd);
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtBomInfoList.Rows[i]["cfathercode"] + "' and dEDate is null and bBomMain=1", Cmd) <= 0)
                {
                    throw new Exception("编码[" + dtBomInfoList.Rows[i]["cfathercode"] + "]不存在，或已经停用，或不允许BOM母件！");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtBomInfoList.Rows[i]["childcode"] + "' and dEDate is null and bBomSub=1", Cmd) <= 0)
                {
                    throw new Exception("编码[" + dtBomInfoList.Rows[i]["childcode"] + "]不存在，或已经停用，或者不允许子件！");
                }
                if (dtBomInfoList.Rows[i]["warecode"] + "" != "")
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..warehouse where cwhcode='" + dtBomInfoList.Rows[i]["warecode"] + "'", Cmd) <= 0)
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
                    U8Operation.GetDataString("select 3", Cmd);
                    //写主BOM
                    oldFatherCode = dtBomInfoList.Rows[i]["cfathercode"] + "";
                    if (U8Operation.GetDataInt("select isnull(max(a.bomid),0) from " + dbname + "..bom_parent a inner join " + dbname + "..bom_bom b on a.bomid=b.bomid " +
                        "where a.ParentId in(SELECT partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "') and b.Version='10'", Cmd) > 0)
                    {
                        U8Operation.GetDataString("select 3-1", Cmd);
                        string bom_part_del_id = U8Operation.GetDataString("SELECT top 1 partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "'", Cmd);
                        string bom_parent_del_id = U8Operation.GetDataString("select a.BomId from " + dbname + "..bom_parent a inner join " + dbname + @"..bom_bom b on a.BomId=b.BomId 
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
                    if (decimal.Parse(dtBomInfoList.Rows[i]["ibaseuse"] + "") <= 0)
                        throw new Exception("母件[" + dtBomInfoList.Rows[i]["cfathercode"] + "]子件[" + dtBomInfoList.Rows[i]["childcode"] + "]基本用量必须大于0");
                    if (b_has_bscount && decimal.Parse(dtBomInfoList.Rows[i]["ibasecount"] + "") <= 0)
                        throw new Exception("母件[" + dtBomInfoList.Rows[i]["cfathercode"] + "]子件[" + dtBomInfoList.Rows[i]["childcode"] + "]基础用量必须大于0");
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

            System.Collections.ArrayList arrDefine = new ArrayList();
            #region  //自定义项处理
            for (int c = 0; c < dtBomInfoList.Columns.Count; c++)
            {
                if (dtBomInfoList.Columns[c].ColumnName.IndexOf("cdefine") > -1)
                {
                    string cindex = dtBomInfoList.Columns[c].ColumnName.Replace("cdefine", "");
                    try
                    {
                        int icolindex = int.Parse(cindex) + 21;
                        arrDefine.Add(icolindex);
                    }
                    catch { }
                }
            }

            #endregion

            for (int i = 0; i < dtBomInfoList.Rows.Count; i++)
            {
                if (oldFatherCode != dtBomInfoList.Rows[i]["cfathercode"] + "")
                {
                    oldFatherCode = dtBomInfoList.Rows[i]["cfathercode"] + "";
                    PartId = U8Operation.GetDataString("SELECT top 1 partid FROM " + dbname + "..bas_part where invcode='" + oldFatherCode + "'", Cmd);
                    BomBomID = U8Operation.GetDataString("SELECT iFatherid+1 FROM ufsystem..UA_Identity where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'", Cmd);
                    Cmd.CommandText = "update ufsystem..UA_Identity set iFatherid=" + BomBomID + " where cVouchType='bom_bom' and cAcc_Id='" + U8AcountID + "'";
                    Cmd.ExecuteNonQuery();
                    //新增BOM基础数据  
                    Cmd.CommandText = "insert into " + dbname + @"..bom_bom(BomId,BomType,Version,VersionDesc,VersionEffdate,VersionEndDate,CreateDate,createUser,
                            createtime,vtid,Status,UpdCount,RelsUser,RelsDate,RelsTime) " +
                        "values(" + BomBomID + ",1,'10','标准','" + dtBomInfoList.Rows[i]["dstartdate"] + "','2099-12-31','2000-01-01','" + dtBomInfoList.Rows[i]["creater"] + @"',
                            getdate(),30442,3,0,'admin',convert(varchar(10),getdate(),121),getdate())";
                    Cmd.ExecuteNonQuery();
                    Cmd.CommandText = "insert into " + dbname + "..bom_parent(BomId,AutoId,ParentId,ParentScrap,SharingPartId) " +
                        "values(" + BomBomID + ",newid()," + PartId + ",0,0)";
                    Cmd.ExecuteNonQuery();
                }

                iChildPartId = U8Operation.GetDataString("SELECT partid FROM " + dbname + "..bas_part where invcode='" + dtBomInfoList.Rows[i]["childcode"] + "'", Cmd);
                iChildOpComponentId = U8Operation.GetDataString("SELECT iChildId+1 FROM ufsystem..UA_Identity where cVouchType='bom_opcomponent' and cAcc_Id='" + U8AcountID + "'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildId=" + iChildOpComponentId + " where cVouchType='bom_opcomponent' and cAcc_Id='" + U8AcountID + "'";
                Cmd.ExecuteNonQuery();
                iChildOptionsId = U8Operation.GetDataString("SELECT iChildId+1 FROM ufsystem..UA_Identity where cVouchType='bom_opcomponentopt' and cAcc_Id='" + U8AcountID + "'", Cmd);
                Cmd.CommandText = "update ufsystem..UA_Identity set iChildId=" + iChildOptionsId + " where cVouchType='bom_opcomponentopt' and cAcc_Id='" + U8AcountID + "'";
                Cmd.ExecuteNonQuery();
                //插入资料表  warecode
                Cmd.CommandText = "insert into " + dbname + "..bom_opcomponentopt(Optionsid,offset,WipType,AccuCostFlag,DrawDeptCode,whcode,OptionalFlag,MutexRule,PlanFactor) " +
                    "values(" + iChildOptionsId + ",0," + dtBomInfoList.Rows[i]["wiptype"] + ",1, " + (dtBomInfoList.Rows[i]["cdepcode"] + "" == "" ? "null" : "'" + dtBomInfoList.Rows[i]["cdepcode"] + "'") + @",
                        " + (dtBomInfoList.Rows[i]["warecode"] + "" == "" ? "null" : "'" + dtBomInfoList.Rows[i]["warecode"] + "'") + ",0,2,100)";
                Cmd.ExecuteNonQuery();
                //插入子件表 
                string coldefine_list = ""; string colvalue_list = "";
                for (int d = 0; d < arrDefine.Count; d++)
                {
                    coldefine_list = ",Define" + arrDefine[d];
                    colvalue_list = ",'" + dtBomInfoList.Rows[i]["cdefine" + (int.Parse(arrDefine[d] + "") - 21)] + "'";
                }
                Cmd.CommandText = "insert into " + dbname + @"..bom_opcomponent(OpComponentId,BomId,SortSeq,OpSeq,ComponentId,EffBegDate,EffEndDate,FvFlag,
                        BaseQtyN,BaseQtyD,
                        CompScrap,ByproductFlag,OptionsId,ProductType,ChangeRate,remark" + coldefine_list + ") " +
                    "values(" + iChildOpComponentId + "," + BomBomID + ",'" + dtBomInfoList.Rows[i]["cseqno"] + "','0000'," + iChildPartId + ",'" + dtBomInfoList.Rows[i]["dstartdate"] + @"','2099-12-31',1,
                        " + dtBomInfoList.Rows[i]["ibaseuse"] + "," + (b_has_bscount ? "1" : dtBomInfoList.Rows[i]["ibasecount"]) + @",
                        0,0," + iChildOptionsId + ",1,1,'" + dtBomInfoList.Rows[i]["remark"] + "'" + colvalue_list + ")";
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
            U8Operation.CloseDataConnection(Conn);
        }


    }


    [WebMethod]  //工艺同步
    public bool SendInvRouteData(System.Data.DataTable dtRountInfoList, string dbname)
    {
        #region   //序列号
        string st_value = System.Configuration.ConfigurationManager.AppSettings["XmlSn"]; if (st_value != U8Operation.GetDataString(1, 10, 100, 1000, 10000)) throw new Exception("序列号错误");
        #endregion

        string oldFatherCode = "";
        if (dtRountInfoList == null) throw new Exception("没有有效数据");
        
        System.Data.SqlClient.SqlConnection Conn = U8Operation.OpenDataConnection();
        if (Conn == null)
        {
            new Exception("数据库连接失败！");
        }
        System.Data.SqlClient.SqlCommand Cmd = Conn.CreateCommand();
        string U8AcountID = U8Operation.GetDataString("select substring('" + dbname + "',8,3)", Cmd);

        if (U8Operation.GetDataInt("SELECT count(*) FROM ufsystem..UA_Identity where cVouchType='sfc_prouting' and cAcc_Id='" + U8AcountID + "'", Cmd) <= 0)
        {
            new Exception("系统工艺路线环境没有初始化，请在U8系统中建立一个工艺路线样板（建完后可删除）！");
        }


        Cmd.Transaction = Conn.BeginTransaction();
        try
        {
            //排序
            dtRountInfoList.DefaultView.Sort = "cfathercode,cseqno";//排序
            dtRountInfoList = dtRountInfoList.DefaultView.ToTable();

            //检查数据逻辑
            #region
            U8Operation.GetDataString("select 'startRount'", Cmd);
            U8Operation.GetDataString("select 1[" + dtRountInfoList.Rows.Count + "]", Cmd);

            System.Collections.ArrayList arrDefine = new ArrayList();
            #region  //自定义项处理
            for (int c = 0; c < dtRountInfoList.Columns.Count; c++)
            {
                if (dtRountInfoList.Columns[c].ColumnName.IndexOf("cdefine") > -1)
                {
                    string cindex = dtRountInfoList.Columns[c].ColumnName.Replace("cdefine", "");
                    try
                    {
                        int icolindex = int.Parse(cindex) + 21;
                        arrDefine.Add(icolindex);
                    }
                    catch { }
                }
            }

            #endregion

            string M_routingID = "";//最新主表行ID
            string PRoutingDId = "";//最新子表行ID
            for (int i = 0; i < dtRountInfoList.Rows.Count; i++)
            {
                if (dtRountInfoList.Rows[i]["cfathercode"] + "" == "") throw new Exception("工艺路线的 产品编码不能为空");
                if (dtRountInfoList.Rows[i]["cseqno"] + "" == "") throw new Exception("产品[" + dtRountInfoList.Rows[i]["cfathercode"] + "]的 的工序行号不能为空");
                U8Operation.GetDataString("select 2", Cmd);
                #region //检查错误
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..Inventory where cinvcode='" + dtRountInfoList.Rows[i]["cfathercode"] + "' and dEDate is null and bBomMain=1", Cmd) <= 0)
                {
                    throw new Exception("编码[" + dtRountInfoList.Rows[i]["cfathercode"] + "]不存在，或已经停用，或不允许BOM母件！");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..sfc_operation where opcode='" + dtRountInfoList.Rows[i]["copcode"] + "'", Cmd) <= 0)
                {
                    throw new Exception("工序编码[" + dtRountInfoList.Rows[i]["copcode"] + "]不存在");
                }
                if (U8Operation.GetDataInt("select count(*) from " + dbname + "..sfc_workcenter where wccode='" + dtRountInfoList.Rows[i]["cwccode"] + "'", Cmd) <= 0)
                {
                    throw new Exception("工作中心编码[" + dtRountInfoList.Rows[i]["cwccode"] + "]不存在");
                }
                if (dtRountInfoList.Rows[i]["rescode"] + "" != "")
                {
                    if (U8Operation.GetDataInt("select count(*) from " + dbname + "..sfc_resource where ResCode='" + dtRountInfoList.Rows[i]["rescode"] + "'", Cmd) <= 0)
                    {
                        throw new Exception("资源编码[" + dtRountInfoList.Rows[i]["rescode"] + "]不存在");
                    }
                }
                #endregion

                if (oldFatherCode != dtRountInfoList.Rows[i]["cfathercode"] + "")
                {
                    #region  //主表
                    string cdepname = U8Operation.GetDataString("select cdepname from " + dbname + @"..department 
                        where cdepcode='" + dtRountInfoList.Rows[i]["cversionname"] + "' or cdepname='" + dtRountInfoList.Rows[i]["cversionname"] + "'", Cmd);
                    if (cdepname == "") throw new Exception("工艺路线的部门[" + dtRountInfoList.Rows[i]["cversionname"] + "]不存在");

                    //处理上道产品的末道工序信息
                    if (oldFatherCode != "")
                    {
                        Cmd.CommandText = "update " + dbname + @"..sfc_proutingdetail set LastFlag=1,BFFlag=1 where PRoutingDId=" + PRoutingDId;
                        Cmd.ExecuteNonQuery();
                    }

                    //获得工艺版本信息
                    oldFatherCode = dtRountInfoList.Rows[i]["cfathercode"] + "";
                    string partid = U8Operation.GetDataString("select partid from " + dbname + @"..bas_part where invcode='" + oldFatherCode + "'", Cmd);
                    System.Data.DataTable dtOldRouteInfo = U8Operation.GetSqlDataTable(@"select b.PRoutingId from " + dbname + @"..sfc_prouting a inner join " + dbname + @"..sfc_proutingpart b on a.PRoutingId=b.PRoutingId
                        where b.PartId=0" + partid + " and a.VersionDesc='" + cdepname + "' order by a.Version desc", "dtOldRouteInfo", Cmd);
                    if (dtOldRouteInfo.Rows.Count > 0)
                    {
                        M_routingID = dtOldRouteInfo.Rows[0]["PRoutingId"] + "";
                        U8Operation.GetDataString("select 3-1", Cmd);
                        #region //删除原工艺路线
                        //删除原有的 工艺路线子表
                        Cmd.CommandText = "delete from " + dbname + "..sfc_proutingdinsp where PRoutinginspId in(select PRoutinginspId from " + dbname + @"..sfc_proutingdetail where PRoutingId=0" + M_routingID + ")";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "delete from " + dbname + "..sfc_proutingdres where PRoutingDId in(select PRoutingDId from " + dbname + @"..sfc_proutingdetail where PRoutingId=0" + M_routingID + ")";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "delete from " + dbname + "..sfc_proutingdetail where PRoutingId=0" + M_routingID;
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }
                    else
                    {
                        M_routingID = "" + GetVoucherIdentity("sfc_prouting", U8AcountID, Cmd);
                        string cVerNo = U8Operation.GetDataString(@"select isnull(MAX(a.Version),0)+10 from " + dbname + @"..sfc_prouting a inner join " + dbname + @"..sfc_proutingpart b on a.PRoutingId=b.PRoutingId
                            where b.PartId=0" + partid, Cmd);
                        string cVerName = cdepname;
                        #region //新增工艺路线主表数据
                        Cmd.CommandText = "insert into " + dbname + @"..sfc_proutingpart(Autoid,PRoutingId,PartId,SharingPartId) values(newid()," + M_routingID + "," + partid + ",0)";
                        Cmd.ExecuteNonQuery();
                        Cmd.CommandText = "insert into " + dbname + @"..sfc_prouting(PRoutingId,RountingType,Version,VersionDesc,VersionEffDate,VersionEndDate,
                                CreateDate,CreateUser,UpdCount,Vtid,Status,CreateTime,RunCardFlag) 
                            values(" + M_routingID + ",1,'" + cVerNo + "','" + cVerName + "','" + dtRountInfoList.Rows[i]["dstartdate"] + @"','2099-12-31',
                                '" + dtRountInfoList.Rows[i]["createdate"] + "','" + dtRountInfoList.Rows[i]["creater"] + "',0,'30377',1,getdate(),1)";
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }
                    #endregion
                }

                #region  子表
                string PRoutinginspId = "" + GetVoucherIdentity("sfc_proutingdinsp", U8AcountID, Cmd);
                PRoutingDId = "" + GetVoucherIdentity("sfc_proutingdetail", U8AcountID, Cmd);
                Cmd.CommandText = "insert into " + dbname + "..sfc_proutingdinsp(PRoutinginspId,QtMethod,VTid,itestrule,optranstype) values(" + PRoutinginspId + ",1,0,1,1)";
                Cmd.ExecuteNonQuery();

                string c_wcid = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select wcid from " + dbname + "..sfc_workcenter where wccode='" + dtRountInfoList.Rows[i]["cwccode"] + "'");
                if (c_wcid.CompareTo("") == 0) throw new Exception("行号[" + dtRountInfoList.Rows[i]["cwccode"] + "]没有找到工作中心信息");

                string coldefine_list = ""; string colvalue_list = "";
                for (int d = 0; d < arrDefine.Count; d++)
                {
                    coldefine_list = ",Define" + arrDefine[d];
                    colvalue_list = ",'" + dtRountInfoList.Rows[i]["cdefine" + (int.Parse(arrDefine[d] + "") - 21)] + "'";
                }

                string operationId = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select OperationId from " + dbname + @"..sfc_operation where OpCode='" + dtRountInfoList.Rows[i]["copcode"] + "'");
                string operationDes = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(Cmd, "select Description from " + dbname + @"..sfc_operation where OpCode='" + dtRountInfoList.Rows[i]["copcode"] + "'");
                Cmd.CommandText = @"insert into " + dbname + @"..sfc_proutingdetail(PRoutingDId,PRoutingId,OpSeq,PRoutinginspId,OperationId,Description,
                    WcId,EffBegDate,EffEndDate,SubFlag,RltOptionFlag,LtPercent,LastFlag,RePortFlag,BFFlag,FeeFlag,PlanSubFlag,DeliveryDays,SplitFlag,remark,ChangeRate" + coldefine_list + @") 
                    values(" + PRoutingDId + "," + M_routingID + ",'" + dtRountInfoList.Rows[i]["cseqno"] + "'," + PRoutinginspId + "," + operationId + ",'" + operationDes + @"',
                    " + c_wcid + ",'2000-01-01','2099-12-31'," + dtRountInfoList.Rows[i]["bomop"] + ", 0,0,0," + dtRountInfoList.Rows[i]["breport"] + @",
                    0,1,0,0,0,'" + dtRountInfoList.Rows[i]["remark"] + "',null" + colvalue_list + ")";
                Cmd.ExecuteNonQuery();
                #endregion
            }
            //处理末道工序
            if (oldFatherCode != "")
            {
                Cmd.CommandText = "update " + dbname + @"..sfc_proutingdetail set LastFlag=1,BFFlag=1 where PRoutingDId=" + PRoutingDId;
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
            U8Operation.CloseDataConnection(Conn);
        }


    }

    public int GetVoucherIdentity(string VouchType, string Acc_id, System.Data.SqlClient.SqlCommand sqlcmd)
    {
        //iFatherId,iChildId
        string id = UCGridCtl.SqlDBCommon.GetDataStringFromSelect(sqlcmd, "select isnull(max(iChildId),0)+1 from ufsystem..ua_identity where cacc_id='" + Acc_id + "' and cvouchtype='" + VouchType + "'");
        sqlcmd.CommandText = "update ufsystem..ua_identity set iFatherId=iFatherId+1,iChildId=iChildId+1 from ufsystem..ua_identity where cacc_id='" + Acc_id + "' and cvouchtype='" + VouchType + "'";
        sqlcmd.ExecuteNonQuery();

        return int.Parse(id);
    }


}

