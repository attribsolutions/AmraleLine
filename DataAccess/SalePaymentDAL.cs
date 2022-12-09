using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using DataObjects;

namespace DataAccess
{
    public class SalePaymentDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public int AddSalePayment(SalePaymentInfo salePayment)
        {
            int retval = 0;
            try
            {
                DbCommand commandSale = _db.GetSqlStringCommand("INSERT INTO SalePayment([ProcessingID], [CustomerID], [PaymentMode], [PaymentDate], [OpeningBalance], [PaidAmount], [ClosingBalance], [AdjustmentAmount],[Comment] , [ChequeNo],[ReceiptNo], [CreatedBy], [CreatedOn] ) " +
                                                                "VALUES (@ProcessingID, @CustomerID, @PaymentMode, @PaymentDate, @OpeningBalance,  @PaidAmount, @ClosingBalance, @AdjustmentAmount, @Comment, @ChequeNo, @ReceiptNo, @CreatedBy, @CreatedOn) " + Environment.NewLine +
                                                                "SELECT IDENT_CURRENT('SalePayment')");

                _db.AddInParameter(commandSale, "@ProcessingID", DbType.Int32, salePayment.ProcessingID);
                _db.AddInParameter(commandSale, "@CustomerID", DbType.Int32, salePayment.CustomerID);
                _db.AddInParameter(commandSale, "@PaymentMode", DbType.Int32, salePayment.PaymentMode);
                _db.AddInParameter(commandSale, "@PaymentDate", DbType.DateTime, salePayment.PaymentDate);
                _db.AddInParameter(commandSale, "@OpeningBalance", DbType.Decimal, salePayment.OpeningBalance);
                _db.AddInParameter(commandSale, "@PaidAmount", DbType.Decimal, salePayment.PaidAmount);
                _db.AddInParameter(commandSale, "@ClosingBalance", DbType.Decimal, salePayment.ClosingBalance);
                _db.AddInParameter(commandSale, "@AdjustmentAmount", DbType.Decimal, salePayment.AdjustmentAmount);
                _db.AddInParameter(commandSale, "@Comment", DbType.String, salePayment.Comment);
                _db.AddInParameter(commandSale, "@ChequeNo",DbType.String,salePayment.ChequeNo);
                _db.AddInParameter(commandSale, "@ReceiptNo", DbType.Int32, salePayment.ReceiptNo);
                _db.AddInParameter(commandSale, "@CreatedBy", DbType.Byte, salePayment.CreatedBy);
                _db.AddInParameter(commandSale, "@CreatedOn", DbType.DateTime, salePayment.CreatedOn);

                using (DbConnection conn = _db.CreateConnection())
                {
                    conn.Open();
                    retval = Convert.ToInt32(_db.ExecuteScalar(commandSale));
                }

                DbCommand commandUpdateSalePayment = _db.GetSqlStringCommand("UPDATE SaleProcessing SET PaidAmount = @PaidAmount , ClosingBalance = @ClosingBalance, AdjustmentAmount = @AdjustmentAmount WHERE ID = @ID");
                _db.AddInParameter(commandUpdateSalePayment, "@ID", DbType.Int32, salePayment.ProcessingID);
                _db.AddInParameter(commandUpdateSalePayment, "@PaidAmount", DbType.Decimal, salePayment.PaidAmount);
                _db.AddInParameter(commandUpdateSalePayment, "@ClosingBalance", DbType.Decimal, salePayment.ClosingBalance);
                _db.AddInParameter(commandUpdateSalePayment, "@AdjustmentAmount", DbType.Decimal, salePayment.AdjustmentAmount);

                _db.ExecuteNonQuery(commandUpdateSalePayment);

            }
            catch (Exception ex)
            {
                throw;
            }
            return retval;
        }

        public BindingList<SalePaymentInfo> GetSalesPaymentByFilter(int searchType, string name, object sDate, object eDate)
        {
            BindingList<SalePaymentInfo> retVal = new BindingList<SalePaymentInfo>();

            //string cmdString = "SELECT SalePayment.ID, SalePayment.ProcessingID, SalePayment.CustomerID, Customers.Name CustomerName, SalePayment.PaymentMode, CASE WHEN(SalePayment.PaymentMode = 1 ) then 'CHEQUE' else 'CASH'  END AS PaymentModeName, SalePayment.PaymentDate,SalePayment.OpeningBalance,SalePayment.PaidAmount,SalePayment.ClosingBalance,SalePayment.AdjustmentAmount,SalePayment.Comment,SalePayment.ChequeNo FROM SalePayment JOIN Customers On SalePayment.CustomerID = Customers.ID ";
            string cmdString = " SELECT SalePayment.ID, SalePayment.ProcessingID, SalePayment.CustomerID, Customers.CustomerNumber,Lines.LineNumber, Customers.Name CustomerName, SalePayment.PaymentMode, CASE WHEN(SalePayment.PaymentMode = 1 ) then 'CHEQUE' else 'CASH'  END AS PaymentModeName, SalePayment.PaymentDate,SalePayment.OpeningBalance,SalePayment.PaidAmount,SalePayment.ClosingBalance,SalePayment.AdjustmentAmount,SalePayment.Comment,SalePayment.ChequeNo,ISNUll(ReceiptNo,0) ReceiptNo FROM   SalePayment JOIN Customers On SalePayment.CustomerID = Customers.ID join Lines on lines.ID=Customers.LineID ";

            try
            {
                DbCommand command = null;
                if (searchType == 0)
                {
                    if (name.Trim() != string.Empty)
                        command = _db.GetSqlStringCommand(cmdString + " WHERE Customers.Name LIKE '" + name + "%' ORDER BY SalePayment.ID DESC");
                    else
                        command = _db.GetSqlStringCommand(cmdString + " ORDER BY SalePayment.ID DESC");
                }
                if (searchType == 1)
                {
                    command = _db.GetSqlStringCommand(cmdString + " WHERE SalePayment.PaymentDate >= @SDate AND SalePayment.PaymentDate <= @EDate ORDER BY SalePayment.ID DESC");
                    _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                    _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
                }
                if (searchType == 2)
                {
                    command = _db.GetSqlStringCommand(cmdString + " WHERE SalePayment.PaidAmount >= @Amount ORDER BY SalePayment.ID DESC");
                    _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
                }
                if (searchType == 3)
                {
                    command = _db.GetSqlStringCommand(cmdString + " WHERE SalePayment.PaidAmount <= @Amount ORDER BY SalePayment.ID DESC");
                    _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
                }
                if (searchType == 4)
                {
                    command = _db.GetSqlStringCommand(cmdString + " WHERE Customers.CustomerNumber = @CustomerNumber ORDER BY SalePayment.ID DESC");
                    _db.AddInParameter(command, "@CustomerNumber", DbType.Int32, Convert.ToInt32(name));
                }


                using (IDataReader dataReader = _db.ExecuteReader(command))
                {
                    while (dataReader.Read())
                    {
                        SalePaymentInfo salepayment = new SalePaymentInfo();
                        salepayment.ID = Convert.ToInt32(dataReader["ID"]);
                        salepayment.ProcessingID = Convert.ToInt32(dataReader["ProcessingID"]);
                        salepayment.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                        salepayment.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                        salepayment.LineNumber = Convert.ToInt32(dataReader["LineNumber"]);
                        salepayment.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                        salepayment.PaymentMode = Convert.ToInt16(dataReader["PaymentMode"]);

                        if (salepayment.PaymentMode==1)
                            salepayment.PaymentModeName = "Cheque";
                        else if (salepayment.PaymentMode == 2)
                            salepayment.PaymentModeName = "Card";
                        else if (salepayment.PaymentMode == 3)
                            salepayment.PaymentModeName = "Paytm";
                        else if (salepayment.PaymentMode == 4)
                            salepayment.PaymentModeName = "Online";
                        else 
                            salepayment.PaymentModeName = "CASH";

                        salepayment.PaymentDate = Convert.ToDateTime(dataReader["PaymentDate"]);
                        salepayment.OpeningBalance = Convert.ToDecimal(dataReader["OpeningBalance"]);
                        salepayment.PaidAmount = Convert.ToDecimal(dataReader["PaidAmount"]);
                        salepayment.ClosingBalance = Convert.ToDecimal(dataReader["ClosingBalance"]);
                        salepayment.AdjustmentAmount = Convert.ToDecimal(dataReader["AdjustmentAmount"]);
                        salepayment.Comment = Convert.ToString(dataReader["Comment"]);
                        salepayment.ChequeNo = Convert.ToString(dataReader["ChequeNo"]);
                        salepayment.ReceiptNo = Convert.ToInt32(dataReader["ReceiptNo"]);
                      

                        retVal.Add(salepayment);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return retVal;
        }

        public void DeletePayment(int salePaymentId, int processingId)
        {
            DbCommand command = null;
            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    command = _db.GetSqlStringCommand("UPDATE SaleProcessing SET PaidAmount = PaidAmount - (SELECT PaidAmount FROM SalePayment WHERE ID = @SalePaymentId) "+
                                                      "WHERE ID = @ProcessingID");
                    _db.AddInParameter(command, "SalePaymentId", DbType.Int32, salePaymentId);
                    _db.AddInParameter(command, "ProcessingID", DbType.Int32, processingId);
                    _db.ExecuteNonQuery(command, transaction);

                    command = _db.GetSqlStringCommand("DELETE FROM SalePayment WHERE ID = @SalePaymentId");
                    _db.AddInParameter(command, "SalePaymentId", DbType.Int32, salePaymentId);
                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }



        public bool CheckReceiptNoExists(int receiptNo)
        {
            DbCommand cmd = _db.GetSqlStringCommand("SELECT COUNT(*) FROM SalePayment  WHERE ISNULL(ReceiptNo,0)=@ReceiptNo");
            _db.AddInParameter(cmd, "@ReceiptNo", DbType.Int32, receiptNo);
            int count= Convert.ToInt32(_db.ExecuteScalar(cmd));
            if (count > 0)
                return false;

            return true;
        }
    }
}
