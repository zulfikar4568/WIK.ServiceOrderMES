using Camstar.WCF.ObjectStack;
using Camstar.WCF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.Driver.Opcenter
{
    public class MaintenanceTransaction
    {
        private readonly Helper _helper;
        public MaintenanceTransaction(Helper helper)
        {
            _helper = helper;
        }

        public WorkflowChanges WorkflowInfo(RevisionedObjectRef ObjectRevisionRef, WorkflowChanges_Info ObjectChanges, bool IgnoreException = true)
        {
            WorkflowMaintService oService = null;
            try
            {
                oService = new WorkflowMaintService(AppSettings.ExCoreUserProfile);
                WorkflowMaint oServiceObject = new WorkflowMaint();
                oServiceObject.ObjectToChange = ObjectRevisionRef;
                WorkflowMaint_Request oServiceRequest = new WorkflowMaint_Request();
                oServiceRequest.Info = new WorkflowMaint_Info();
                oServiceRequest.Info.ObjectChanges = ObjectChanges;

                ResultStatus oResultStatus = oService.Load(oServiceObject, oServiceRequest, out WorkflowMaint_Result oServiceResult);

                EventLogUtil.LogEvent(oResultStatus.Message, System.Diagnostics.EventLogEntryType.Information, 3);
                if (oServiceResult.Value.ObjectChanges != null)
                {
                    return oServiceResult.Value.ObjectChanges;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
            finally
            {
                if (!(oService is null)) oService.Close();
            }
        }
        public ERPRouteChanges ERPRouteInfo(RevisionedObjectRef ObjectRevisionRef, ERPRouteChanges_Info ObjectChanges, bool IgnoreException = true)
        {
            ERPRouteMaintService oService = null;
            try
            {
                oService = new ERPRouteMaintService(AppSettings.ExCoreUserProfile);
                ERPRouteMaint oServiceObject = new ERPRouteMaint();
                oServiceObject.ObjectToChange = ObjectRevisionRef;

                ERPRouteMaint_Request oServiceRequest = new ERPRouteMaint_Request();
                oServiceRequest.Info = new ERPRouteMaint_Info();
                oServiceRequest.Info.ObjectChanges = ObjectChanges;

                ERPRouteMaint_Result oServiceResult = null;
                ResultStatus oResultStatus = oService.Load(oServiceObject, oServiceRequest, out oServiceResult);

                EventLogUtil.LogEvent(oResultStatus.Message, System.Diagnostics.EventLogEntryType.Information, 3);
                if (oServiceResult.Value.ObjectChanges != null)
                {
                    return oServiceResult.Value.ObjectChanges;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
            finally
            {
                if (!(oService is null)) oService.Close();
            }
        }
        public ProductChanges ProductInfo(RevisionedObjectRef ObjectRevisionRef , ProductChanges_Info ObjectChanges, bool IgnoreException = true)
        {
            ProductMaintService oService = null;
            try
            {
                oService = new ProductMaintService(AppSettings.ExCoreUserProfile);
                ProductMaint oServiceObject = new ProductMaint();
                oServiceObject.ObjectToChange = ObjectRevisionRef;
                ProductMaint_Request oServiceRequest = new ProductMaint_Request();
                oServiceRequest.Info = new ProductMaint_Info();
                oServiceRequest.Info.ObjectChanges = ObjectChanges;

                ProductMaint_Result oServiceResult = null;
                ResultStatus oResultStatus = oService.Load(oServiceObject, oServiceRequest, out oServiceResult);

                EventLogUtil.LogEvent(oResultStatus.Message, System.Diagnostics.EventLogEntryType.Information, 3);
                if (oServiceResult.Value.ObjectChanges != null)
                {
                    return oServiceResult.Value.ObjectChanges;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
            finally
            {
                if (!(oService is null)) oService.Close();
            }
        }
        public MfgOrderChanges MfgOrderInfo(NamedObjectRef ObjectRef, bool IgnoreException = true)
        {
            MfgOrderMaintService oService = null;
            try
            {
                oService = new MfgOrderMaintService(AppSettings.ExCoreUserProfile);
                MfgOrderMaint oServiceObject = new MfgOrderMaint();
                oServiceObject.ObjectToChange = ObjectRef;

                MfgOrderMaint_Request oServiceRequest = new MfgOrderMaint_Request();
                oServiceRequest.Info = new MfgOrderMaint_Info();
                oServiceRequest.Info.ObjectChanges = new MfgOrderChanges_Info();
                oServiceRequest.Info.ObjectChanges.RequestValue = true;

                MfgOrderMaint_Result oServiceResult = null;
                ResultStatus oResultStatus = oService.Load(oServiceObject, oServiceRequest, out oServiceResult);

                string sMessage = "";
                if (_helper.ProcessResult(oResultStatus, ref sMessage, true))
                {
                    EventLogUtil.LogEvent(oResultStatus.Message, System.Diagnostics.EventLogEntryType.Information, 3);
                    return oServiceResult.Value.ObjectChanges;
                }
                else
                {
                    EventLogUtil.LogEvent($"{ObjectRef.Name} doesn't exists!", System.Diagnostics.EventLogEntryType.Warning, 3);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
            finally
            {
                if (!(oService is null)) oService.Close();
            }
        }
        public NamedObjectRef[] MfgOrderInfo(bool IgnoreException = true)
        {
            MfgOrderMaintService oService = null;
            try
            {
                oService = new MfgOrderMaintService(AppSettings.ExCoreUserProfile);
                MfgOrderMaint oServiceObject = new MfgOrderMaint();

                MfgOrderMaint_Request oServiceRequest = new MfgOrderMaint_Request();
                oServiceRequest.Info = new MfgOrderMaint_Info();
                oServiceRequest.Info.ObjectListInquiry = new Info(true);
                oServiceRequest.Info.ObjectListInquiry.RequestValue = true;

                MfgOrderMaint_Result oServiceResult = null;
                ResultStatus oResultStatus = oService.GetEnvironment(oServiceRequest, out oServiceResult);

                EventLogUtil.LogEvent(oResultStatus.Message, System.Diagnostics.EventLogEntryType.Information, 3);
                string sMessage = "";
                if (_helper.ProcessResult(oResultStatus, ref sMessage, true))
                {
                    return oServiceResult.Value.ObjectListInquiry;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
            finally
            {
                if (!(oService is null)) oService.Close();
            }
        }
        public bool OrderTypeTxn(OrderTypeChanges ObjectChanges, bool IgnoreException = true)
        {
            OrderTypeMaintService oService = null;
            try
            {
                OrderTypeMaint oServiceObject = null;

                //Check Object exists
                oService = new OrderTypeMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking Order Type " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new OrderTypeMaint(), ObjectChanges.Name.ToString());

                //Prepare Object
                EventLogUtil.LogEvent("Preparing Order Type " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new OrderTypeMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare Input Data
                oServiceObject = new OrderTypeMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                //Save Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Type Order " + ObjectChanges.Name.ToString(), System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Type Order " + ObjectChanges.Name.ToString(), System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusOrderType = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusOrderType;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool OrderStatusTxn(OrderStatusChanges ObjectChanges, bool IgnoreException = true)
        {
            OrderStatusMaintService oService = null;
            try
            {
                OrderStatusMaint oServiceObject = null;

                //Check Object exists
                oService = new OrderStatusMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking Order Status " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new OrderStatusMaint(), ObjectChanges.Name.ToString());

                //Prepare Object
                EventLogUtil.LogEvent("Preparing Order Status " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new OrderStatusMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare Input Data
                oServiceObject = new OrderStatusMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                //Save Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Order Status " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Order Status " + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool bStatus = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return bStatus;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool MfgLineTxn(sswMfgLineChanges ObjectChanges, bool IgnoreException = true)
        {
            sswMfgLineMaintService oService = null;
            try
            {
                sswMfgLineMaint oServiceObject = null;

                oService = new sswMfgLineMaintService(AppSettings.ExCoreUserProfile);

                //checking MfgLine exists
                EventLogUtil.LogEvent("Checking Mfg Line" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new sswMfgLineMaint(), ObjectChanges.Name.ToString());

                //Prepare Object
                EventLogUtil.LogEvent("Preparing Mfg Line" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new sswMfgLineMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare input Data
                oServiceObject = new sswMfgLineMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Mfg Line" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Mfg Line" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool bStatus = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return bStatus;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool MfgOrderTxn(MfgOrderChanges ObjectChanges, bool IgnoreException = true)
        {
            MfgOrderMaintService oService = null;
            try
            {
                MfgOrderMaint oServiceObject = null;

                //check object exists
                oService = new MfgOrderMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking Mfg Order" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new MfgOrderMaint(), ObjectChanges.Name.ToString());

                // Prepare Object
                EventLogUtil.LogEvent("Preparing Mfg Order" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new MfgOrderMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare input data
                oServiceObject = new MfgOrderMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Mfg Order" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Mfg Order" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusMfgOrder = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusMfgOrder;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool ProductTxn(ProductChanges ObjectChanges, bool IgnoreException = true)
        {
            ProductMaintService oService = null;
            try
            {
                ProductMaint oServiceObject = null;
                // CheckObject Exists
                oService = new ProductMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent($"Checking Product {ObjectChanges.Name} : {ObjectChanges.Revision}", System.Diagnostics.EventLogEntryType.Information, 3);
                bool bBaseExists = _helper.ObjectExists(oService, new ProductMaint(), ObjectChanges.Name.ToString(), "");
                bool bObjectExists = _helper.ObjectExists(oService, new ProductMaint(), ObjectChanges.Name.ToString(), ObjectChanges.Revision.ToString());
                // Prepare Object
                EventLogUtil.LogEvent($"Preparing Product {ObjectChanges.Name} : {ObjectChanges.Revision}", System.Diagnostics.EventLogEntryType.Information, 3);
                oServiceObject = new ProductMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new RevisionedObjectRef(ObjectChanges.Name.ToString(), ObjectChanges.Revision.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }
                else if (bBaseExists)
                {
                    oService.BeginTransaction();
                    oServiceObject.BaseToChange = new RevisionedObjectRef();
                    oServiceObject.BaseToChange.Name = ObjectChanges.Name.ToString();
                    oService.NewRev(oServiceObject);
                }
                // PrepareInput Data
                oServiceObject = new ProductMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent($"Updating Product {ObjectChanges.Name} : {ObjectChanges.Revision}", System.Diagnostics.EventLogEntryType.Information, 3);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else if (bBaseExists)
                {
                    EventLogUtil.LogEvent($"Creating Product {ObjectChanges.Name} : {ObjectChanges.Revision}", System.Diagnostics.EventLogEntryType.Information, 3);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent($"Creating Product {ObjectChanges.Name} : {ObjectChanges.Revision}", System.Diagnostics.EventLogEntryType.Information, 3);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusProduct = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusProduct;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool ProductFamilyTxn(ProductFamilyChanges ObjectChanges, bool IgnoreException = true)
        {
            ProductFamilyMaintService oService = null;
            try
            {
                ProductFamilyMaint oServiceObject = null;

                oService = new ProductFamilyMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking Product Family" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new ProductFamilyMaint(), ObjectChanges.Name.ToString());

                // Prepare Object
                EventLogUtil.LogEvent("Preparing Product Family" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new ProductFamilyMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare input data
                oServiceObject = new ProductFamilyMaint();
                oServiceObject.ObjectChanges = ObjectChanges;
                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Product Family" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Product Family" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusProductFamily = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusProductFamily;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool ProductTypeTxn(ProductTypeChanges ObjectChanges, bool IgnoreException = true)
        {
            ProductTypeMaintService oService = null;
            try
            {
                ProductTypeMaint oServiceObject = null;

                oService = new ProductTypeMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking Product Type" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new ProductTypeMaint(), ObjectChanges.Name.ToString());

                // Prepare Object
                EventLogUtil.LogEvent("Preparing Product Type" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new ProductTypeMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare input data
                oServiceObject = new ProductTypeMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating Product Type" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating Product Type" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusProductType = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusProductType;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
        public bool UOMTxn(UOMChanges ObjectChanges, bool IgnoreException = true)
        {
            UOMMaintService oService = null;
            try
            {
                UOMMaint oServiceObject = null;

                oService = new UOMMaintService(AppSettings.ExCoreUserProfile);
                EventLogUtil.LogEvent("Checking UOM" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                bool bObjectExists = _helper.ObjectExists(oService, new UOMMaint(), ObjectChanges.Name.ToString());

                // Prepare Object
                EventLogUtil.LogEvent("Preparing UOM" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                oServiceObject = new UOMMaint();
                if (bObjectExists)
                {
                    oServiceObject.ObjectToChange = new NamedObjectRef(ObjectChanges.Name.ToString());
                    oService.BeginTransaction();
                    oService.Load(oServiceObject);
                }

                //Prepare input data
                oServiceObject = new UOMMaint();
                oServiceObject.ObjectChanges = ObjectChanges;

                // Save the Data
                if (bObjectExists)
                {
                    EventLogUtil.LogEvent("Updating UOM" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.ExecuteTransaction(oServiceObject);
                }
                else
                {
                    EventLogUtil.LogEvent("Creating UOM" + ObjectChanges.Name, System.Diagnostics.EventLogEntryType.Information, 2);
                    oService.BeginTransaction();
                    oService.New(oServiceObject);
                    oService.ExecuteTransaction();
                }
                string sMessage = "";
                bool statusUOM = _helper.ProcessResult(oService.CommitTransaction(), ref sMessage, false);
                EventLogUtil.LogEvent(sMessage, System.Diagnostics.EventLogEntryType.Information, 2);
                return statusUOM;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
            finally
            {
                if (oService != null) oService.Close();
            }
        }
    }
}
