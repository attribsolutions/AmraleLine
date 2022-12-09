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
    /// <summary>
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the ReceiptPayment table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:12:58 PM
    /// </summary>
    public class ReceiptPaymentDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public ReceiptPaymentDAL()
        {

        }
        /// <summary>
        /// Gets all the ReceiptPayments from the ReceiptPayments table
        /// </summary>
        /// <returns>BindingList of ReceiptPayments</returns>
        public BindingList<ReceiptPaymentInfo> GetReceiptPaymentsAll()
        {
            BindingList<ReceiptPaymentInfo> retVal = new BindingList<ReceiptPaymentInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [TransactionDate], [ReceiptPayment], [Particulars], [PayMode], [BankName], [ChequeNo], [Amount], [ReceivedPaidBy], [PartyTransaction], [PartyID], [BillID], [DueAmount], [BalanceAmount], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ReceiptPayments ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ReceiptPaymentInfo receiptPayment = new ReceiptPaymentInfo();
                    receiptPayment.ID = Convert.ToInt64(dataReader["ID"]);
                    receiptPayment.TransactionDate = Convert.ToDateTime(dataReader["TransactionDate"]);
                    receiptPayment.ReceiptPayment = Convert.ToString(dataReader["ReceiptPayment"]);
                    receiptPayment.Particulars = Convert.ToString(dataReader["Particulars"]);
                    receiptPayment.PayMode = Convert.ToString(dataReader["PayMode"]);
                    if (dataReader["BankName"] == DBNull.Value)
                    {
                        receiptPayment.BankName = string.Empty;
                    }
                    else
                    {
                        receiptPayment.BankName = Convert.ToString(dataReader["BankName"]);
                    }
                    if (dataReader["ChequeNo"] == DBNull.Value)
                    {
                        receiptPayment.ChequeNo = string.Empty;
                    }
                    else
                    {
                        receiptPayment.ChequeNo = Convert.ToString(dataReader["ChequeNo"]);
                    }
                    receiptPayment.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    receiptPayment.ReceivedPaidBy = Convert.ToInt32(dataReader["ReceivedPaidBy"]);
                    receiptPayment.PartyTransaction = Convert.ToBoolean(dataReader["PartyTransaction"]);
                    if (dataReader["PartyID"] == DBNull.Value)
                    {
                        receiptPayment.PartyID = 0;
                    }
                    else
                    {
                        receiptPayment.PartyID = Convert.ToInt32(dataReader["PartyID"]);
                    }
                    if (dataReader["BillID"] == DBNull.Value)
                    {
                        receiptPayment.BillID = 0;
                    }
                    else
                    {
                        receiptPayment.BillID = Convert.ToInt64(dataReader["BillID"]);
                    }
                    if (dataReader["DueAmount"] == DBNull.Value)
                    {
                        receiptPayment.DueAmount = 0;
                    }
                    else
                    {
                        receiptPayment.DueAmount = Convert.ToDecimal(dataReader["DueAmount"]);
                    }
                    if (dataReader["BalanceAmount"] == DBNull.Value)
                    {
                        receiptPayment.BalanceAmount = 0;
                    }
                    else
                    {
                        receiptPayment.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    }
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        receiptPayment.Description = string.Empty;
                    }
                    else
                    {
                        receiptPayment.Description = Convert.ToString(dataReader["Description"]);
                    }
                    receiptPayment.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    receiptPayment.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    receiptPayment.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    receiptPayment.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(receiptPayment);
                }
            }
            return retVal;
        }

        public BindingList<ReceiptPaymentInfo> GetReceiptPaymentsByFilter(int searchType, string name, object sDate, object eDate)
        {
            BindingList<ReceiptPaymentInfo> retVal = new BindingList<ReceiptPaymentInfo>();
            string cmdString = "SELECT [ID], [TransactionDate], [ReceiptPayment], [Particulars], [PayMode], [BankName], [ChequeNo], [Amount], [ReceivedPaidBy], [PartyTransaction], [PartyID], [BillID], [DueAmount], [BalanceAmount], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ReceiptPayments ";
            DbCommand command = null;

            if (searchType == 0)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Particulars LIKE '" + name + "%' ORDER BY TransactionDate DESC");
            if (searchType == 1)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE TransactionDate >= @SDate AND TransactionDate <= @EDate ORDER BY TransactionDate DESC");
                _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }
            if (searchType == 2)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Amount >= @Amount ORDER BY TransactionDate DESC");
                _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
            }
            if (searchType == 3)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Amount <= @Amount ORDER BY TransactionDate DESC");
                _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
            }

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ReceiptPaymentInfo receiptPayment = new ReceiptPaymentInfo();
                    receiptPayment.ID = Convert.ToInt64(dataReader["ID"]);
                    receiptPayment.TransactionDate = Convert.ToDateTime(dataReader["TransactionDate"]);
                    receiptPayment.ReceiptPayment = Convert.ToString(dataReader["ReceiptPayment"]);
                    receiptPayment.Particulars = Convert.ToString(dataReader["Particulars"]);
                    receiptPayment.PayMode = Convert.ToString(dataReader["PayMode"]);
                    if (dataReader["BankName"] == DBNull.Value)
                    {
                        receiptPayment.BankName = string.Empty;
                    }
                    else
                    {
                        receiptPayment.BankName = Convert.ToString(dataReader["BankName"]);
                    }
                    if (dataReader["ChequeNo"] == DBNull.Value)
                    {
                        receiptPayment.ChequeNo = string.Empty;
                    }
                    else
                    {
                        receiptPayment.ChequeNo = Convert.ToString(dataReader["ChequeNo"]);
                    }
                    receiptPayment.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    receiptPayment.ReceivedPaidBy = Convert.ToInt32(dataReader["ReceivedPaidBy"]);
                    receiptPayment.PartyTransaction = Convert.ToBoolean(dataReader["PartyTransaction"]);
                    if (dataReader["PartyID"] == DBNull.Value)
                    {
                        receiptPayment.PartyID = 0;
                    }
                    else
                    {
                        receiptPayment.PartyID = Convert.ToInt32(dataReader["PartyID"]);
                    }
                    if (dataReader["BillID"] == DBNull.Value)
                    {
                        receiptPayment.BillID = 0;
                    }
                    else
                    {
                        receiptPayment.BillID = Convert.ToInt64(dataReader["BillID"]);
                    }
                    if (dataReader["DueAmount"] == DBNull.Value)
                    {
                        receiptPayment.DueAmount = 0;
                    }
                    else
                    {
                        receiptPayment.DueAmount = Convert.ToDecimal(dataReader["DueAmount"]);
                    }
                    if (dataReader["BalanceAmount"] == DBNull.Value)
                    {
                        receiptPayment.BalanceAmount = 0;
                    }
                    else
                    {
                        receiptPayment.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    }
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        receiptPayment.Description = string.Empty;
                    }
                    else
                    {
                        receiptPayment.Description = Convert.ToString(dataReader["Description"]);
                    }
                    receiptPayment.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    receiptPayment.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    receiptPayment.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    receiptPayment.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(receiptPayment);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single ReceiptPayment based on Id
        /// </summary>
        /// <param name="receiptpaymentId">Id of the ReceiptPayment the needs to be retrieved</param>
        /// <returns>Instance of ReceiptPayment</returns>
        public ReceiptPaymentInfo GetReceiptPayment(int receiptpaymentId)
        {
            ReceiptPaymentInfo retVal = new ReceiptPaymentInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [TransactionDate], [ReceiptPayment], [Particulars], [PayMode], [BankName], [ChequeNo], [Amount], [ReceivedPaidBy], [PartyTransaction], [PartyID], [BillID], [DueAmount], [BalanceAmount], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ReceiptPayments WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, receiptpaymentId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.TransactionDate = Convert.ToDateTime(dataReader["TransactionDate"]);
                    retVal.ReceiptPayment = Convert.ToString(dataReader["ReceiptPayment"]);
                    retVal.Particulars = Convert.ToString(dataReader["Particulars"]);
                    retVal.PayMode = Convert.ToString(dataReader["PayMode"]);
                    if (dataReader["BankName"] == DBNull.Value)
                    {
                        retVal.BankName = string.Empty;
                    }
                    else
                    {
                        retVal.BankName = Convert.ToString(dataReader["BankName"]);
                    }
                    if (dataReader["ChequeNo"] == DBNull.Value)
                    {
                        retVal.ChequeNo = string.Empty;
                    }
                    else
                    {
                        retVal.ChequeNo = Convert.ToString(dataReader["ChequeNo"]);
                    }
                    retVal.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    retVal.ReceivedPaidBy = Convert.ToInt32(dataReader["ReceivedPaidBy"]);
                    retVal.PartyTransaction = Convert.ToBoolean(dataReader["PartyTransaction"]);
                    if (dataReader["PartyID"] == DBNull.Value)
                    {
                        retVal.PartyID = 0;
                    }
                    else
                    {
                        retVal.PartyID = Convert.ToInt32(dataReader["PartyID"]);
                    }
                    if (dataReader["BillID"] == DBNull.Value)
                    {
                        retVal.BillID = 0;
                    }
                    else
                    {
                        retVal.BillID = Convert.ToInt64(dataReader["BillID"]);
                    }
                    if (dataReader["DueAmount"] == DBNull.Value)
                    {
                        retVal.DueAmount = 0;
                    }
                    else
                    {
                        retVal.DueAmount = Convert.ToDecimal(dataReader["DueAmount"]);
                    }
                    if (dataReader["BalanceAmount"] == DBNull.Value)
                    {
                        retVal.BalanceAmount = 0;
                    }
                    else
                    {
                        retVal.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    }
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        retVal.Description = string.Empty;
                    }
                    else
                    {
                        retVal.Description = Convert.ToString(dataReader["Description"]);
                    }
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new ReceiptPayment in to the database
        /// </summary>
        /// <param name="receiptpayment">Instance of ReceiptPayment</param>
        /// <returns>Id of the newly added ReceiptPayment</returns>
        public int AddReceiptPayment(ReceiptPaymentInfo receiptPayment)
        {
            int retval = 0;
            DbCommand commandReceiptPayment = _db.GetSqlStringCommand("INSERT INTO ReceiptPayments([TransactionDate], [ReceiptPayment], [Particulars], [PayMode], [BankName], [ChequeNo], [Amount], [ReceivedPaidBy], [PartyTransaction], [PartyID], [BillID], [DueAmount], [BalanceAmount], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@TransactionDate, @ReceiptPayment, @Particulars, @PayMode, @BankName, @ChequeNo, @Amount, @ReceivedPaidBy, @PartyTransaction, @PartyID, @BillID, @DueAmount, @BalanceAmount, @Description, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('ReceiptPayments')");

            _db.AddInParameter(commandReceiptPayment, "@TransactionDate", DbType.DateTime, receiptPayment.TransactionDate);
            _db.AddInParameter(commandReceiptPayment, "@ReceiptPayment", DbType.String, receiptPayment.ReceiptPayment);
            _db.AddInParameter(commandReceiptPayment, "@Particulars", DbType.String, receiptPayment.Particulars);
            _db.AddInParameter(commandReceiptPayment, "@PayMode", DbType.String, receiptPayment.PayMode);
            if (receiptPayment.BankName == string.Empty)
            {
                _db.AddInParameter(commandReceiptPayment, "@BankName", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@BankName", DbType.String, receiptPayment.BankName);
            }
            if (receiptPayment.ChequeNo == string.Empty)
            {
                _db.AddInParameter(commandReceiptPayment, "@ChequeNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@ChequeNo", DbType.String, receiptPayment.ChequeNo);
            }
            _db.AddInParameter(commandReceiptPayment, "@Amount", DbType.Decimal, receiptPayment.Amount);
            _db.AddInParameter(commandReceiptPayment, "@ReceivedPaidBy", DbType.Int32, receiptPayment.ReceivedPaidBy);
            _db.AddInParameter(commandReceiptPayment, "@PartyTransaction", DbType.Boolean, receiptPayment.PartyTransaction);
            if (receiptPayment.PartyID == 0)
            {
                _db.AddInParameter(commandReceiptPayment, "@PartyID", DbType.Int32, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@PartyID", DbType.Int32, receiptPayment.PartyID);
            }
            if (receiptPayment.BillID == 0)
            {
                _db.AddInParameter(commandReceiptPayment, "@BillID", DbType.Int64, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@BillID", DbType.Int64, receiptPayment.BillID);
            }
            if (receiptPayment.DueAmount == 0)
            {
                _db.AddInParameter(commandReceiptPayment, "@DueAmount", DbType.Decimal, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@DueAmount", DbType.Decimal, receiptPayment.DueAmount);
            }
            if (receiptPayment.BalanceAmount == 0)
            {
                _db.AddInParameter(commandReceiptPayment, "@BalanceAmount", DbType.Decimal, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@BalanceAmount", DbType.Decimal, receiptPayment.BalanceAmount);
            }
            if (receiptPayment.Description == string.Empty)
            {
                _db.AddInParameter(commandReceiptPayment, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandReceiptPayment, "@Description", DbType.String, receiptPayment.Description);
            }
            _db.AddInParameter(commandReceiptPayment, "@CreatedBy", DbType.Byte, receiptPayment.CreatedBy);
            _db.AddInParameter(commandReceiptPayment, "@CreatedOn", DbType.DateTime, receiptPayment.CreatedOn);
            _db.AddInParameter(commandReceiptPayment, "@UpdatedBy", DbType.Byte, receiptPayment.UpdatedBy);
            _db.AddInParameter(commandReceiptPayment, "@UpdatedOn", DbType.DateTime, receiptPayment.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    retval = Convert.ToInt32(_db.ExecuteScalar(commandReceiptPayment, transaction));

                    if (receiptPayment.PartyTransaction)
                    {
                        if (receiptPayment.ReceiptPayment.Trim() == "Payment")
                        {
                            DbCommand commandInvoice = _db.GetSqlStringCommand("UPDATE Invoices SET BalanceAmount = BalanceAmount - @BalanceAmount WHERE Id = @Id");

                            _db.AddInParameter(commandInvoice, "@Id", DbType.Int32, receiptPayment.BillID);
                            _db.AddInParameter(commandInvoice, "@BalanceAmount", DbType.Decimal, receiptPayment.Amount);

                            _db.ExecuteNonQuery(commandInvoice, transaction);
                        }
                        else
                        {
                            DbCommand commandInvoice = _db.GetSqlStringCommand("UPDATE Sales SET BalanceAmount = BalanceAmount - @BalanceAmount WHERE Id = @Id");

                            _db.AddInParameter(commandInvoice, "@Id", DbType.Int32, receiptPayment.BillID);
                            _db.AddInParameter(commandInvoice, "@BalanceAmount", DbType.Decimal, receiptPayment.Amount);

                            _db.ExecuteNonQuery(commandInvoice, transaction);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Updates the ReceiptPayment
        /// </summary>
        /// <param name="receiptpayment">Instance of ReceiptPayment class</param>
        public void UpdateReceiptPayment(ReceiptPaymentInfo receiptPayment, decimal oldAmountPaid)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE ReceiptPayments SET [TransactionDate] = @TransactionDate, [ReceiptPayment] = @ReceiptPayment, [Particulars] = @Particulars, [PayMode] = @PayMode, [BankName] = @BankName, [ChequeNo] = @ChequeNo, [Amount] = @Amount, [ReceivedPaidBy] = @ReceivedPaidBy, [PartyTransaction] = @PartyTransaction, [PartyID] = @PartyID, [BillID] = @BillID, [DueAmount] = @DueAmount, [BalanceAmount] = @BalanceAmount, [Description] = @Description, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int64, receiptPayment.ID);
            _db.AddInParameter(command, "@TransactionDate", DbType.DateTime, receiptPayment.TransactionDate);
            _db.AddInParameter(command, "@ReceiptPayment", DbType.String, receiptPayment.ReceiptPayment);
            _db.AddInParameter(command, "@Particulars", DbType.String, receiptPayment.Particulars);
            _db.AddInParameter(command, "@PayMode", DbType.String, receiptPayment.PayMode);
            if (receiptPayment.BankName == string.Empty)
            {
                _db.AddInParameter(command, "@BankName", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@BankName", DbType.String, receiptPayment.BankName);
            }
            if (receiptPayment.ChequeNo == string.Empty)
            {
                _db.AddInParameter(command, "@ChequeNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@ChequeNo", DbType.String, receiptPayment.ChequeNo);
            }
            _db.AddInParameter(command, "@Amount", DbType.Decimal, receiptPayment.Amount);
            _db.AddInParameter(command, "@ReceivedPaidBy", DbType.Int32, receiptPayment.ReceivedPaidBy);
            _db.AddInParameter(command, "@PartyTransaction", DbType.Boolean, receiptPayment.PartyTransaction);
            if (receiptPayment.PartyID == 0)
            {
                _db.AddInParameter(command, "@PartyID", DbType.Int32, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@PartyID", DbType.Int32, receiptPayment.PartyID);
            }
            if (receiptPayment.BillID == 0)
            {
                _db.AddInParameter(command, "@BillID", DbType.Int64, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@BillID", DbType.Int64, receiptPayment.BillID);
            }
            if (receiptPayment.DueAmount == 0)
            {
                _db.AddInParameter(command, "@DueAmount", DbType.Decimal, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@DueAmount", DbType.Decimal, receiptPayment.DueAmount);
            }
            if (receiptPayment.BalanceAmount == 0)
            {
                _db.AddInParameter(command, "@BalanceAmount", DbType.Decimal, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@BalanceAmount", DbType.Decimal, receiptPayment.BalanceAmount);
            }
            if (receiptPayment.Description == string.Empty)
            {
                _db.AddInParameter(command, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Description", DbType.String, receiptPayment.Description);
            }
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, receiptPayment.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, receiptPayment.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);

                    if (receiptPayment.PartyTransaction)
                    {
                        DbCommand commandPurSale = null;

                        if (receiptPayment.ReceiptPayment.Trim() == "Payment")
                            commandPurSale = _db.GetSqlStringCommand("UPDATE Invoices SET BalanceAmount = BalanceAmount + @OldAmountPaid -@Amount WHERE Id = @Id");
                        else
                            commandPurSale = _db.GetSqlStringCommand("UPDATE Sales SET BalanceAmount = BalanceAmount + @OldAmountPaid - @Amount WHERE Id = @Id");

                        _db.AddInParameter(commandPurSale, "@Id", DbType.Int32, receiptPayment.BillID);
                        _db.AddInParameter(commandPurSale, "@Amount", DbType.Decimal, receiptPayment.Amount);
                        _db.AddInParameter(commandPurSale, "@OldAmountPaid", DbType.Decimal, oldAmountPaid);

                        _db.ExecuteNonQuery(commandPurSale, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Deletes the ReceiptPayment from the database
        /// </summary>
        /// <param name="receiptpaymentId">Id of the ReceiptPayment that needs to be deleted</param>
        public void DeleteReceiptPayment(ReceiptPaymentInfo receiptPayment)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM ReceiptPayments WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, receiptPayment.ID);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);

                    if (receiptPayment.PartyTransaction)
                    {
                        DbCommand commandInvoiceSale = null;
                        if (receiptPayment.ReceiptPayment.Trim() == "Payment")
                            commandInvoiceSale = _db.GetSqlStringCommand("UPDATE Invoices SET BalanceAmount = BalanceAmount + @Amount WHERE Id = @Id");
                        else
                            commandInvoiceSale = _db.GetSqlStringCommand("UPDATE Sales SET BalanceAmount = BalanceAmount + @Amount WHERE Id = @Id");

                        _db.AddInParameter(commandInvoiceSale, "@Id", DbType.Int32, receiptPayment.BillID);
                        _db.AddInParameter(commandInvoiceSale, "@Amount", DbType.Decimal, receiptPayment.Amount);

                        _db.ExecuteNonQuery(commandInvoiceSale, transaction);

                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public bool CheckReceiptPaymentUsed(int receiptPaymentId)
        {
            //DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM ReceiptPayments WHERE PartyTransaction = 'True' AND ReceiptPayment = 'Payment' AND BillID = @InvoiceId");
            //_db.AddInParameter(command, "@Id", DbType.Int32, receiptPaymentId);

            //using (DbConnection conn = _db.CreateConnection())
            //{
            //    conn.Open();
            //    int cnt = Convert.ToInt32(_db.ExecuteScalar(command));

            //    if (cnt > 0)
            //        return true;
            //}
            return false;
        }
    }
}