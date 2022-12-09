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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the InvoiceChallan table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:08:41 PM
    /// </summary>
    public class InvoiceChallanDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public InvoiceChallanDAL()
        {

        }
        /// <summary>
        /// Gets all the InvoiceChallans from the InvoiceChallans table
        /// </summary>
        /// <returns>BindingList of InvoiceChallans</returns>
        public BindingList<InvoiceChallanInfo> GetInvoiceChallansAll()
        {
            BindingList<InvoiceChallanInfo> retVal = new BindingList<InvoiceChallanInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [InvoiceID], [ChallanID] FROM InvoiceChallans ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    InvoiceChallanInfo invoiceChallan = new InvoiceChallanInfo();
                    invoiceChallan.ID = Convert.ToInt64(dataReader["ID"]);
                    invoiceChallan.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    invoiceChallan.ChallanID = Convert.ToInt64(dataReader["ChallanID"]);

                    retVal.Add(invoiceChallan);
                }
            }
            return retVal;
        }

        public BindingList<InvoiceChallanInfo> GetInvoiceChallansByInvoiceId(int invoiceId)
        {
            BindingList<InvoiceChallanInfo> retVal = new BindingList<InvoiceChallanInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT InvoiceChallans.ID, InvoiceID, ChallanID, ChallanDate, Completed FROM InvoiceChallans JOIN Challans ON ChallanID = Challans.ID WHERE InvoiceId = @InvoiceId");
            _db.AddInParameter(command, "@InvoiceId", DbType.Int32, invoiceId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    InvoiceChallanInfo invoiceChallan = new InvoiceChallanInfo();
                    invoiceChallan.ID = Convert.ToInt64(dataReader["ID"]);
                    invoiceChallan.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    invoiceChallan.ChallanID = Convert.ToInt64(dataReader["ChallanID"]);
                    invoiceChallan.ChallanDate = Convert.ToString(Convert.ToDateTime(dataReader["ChallanDate"]).ToString("dd MMM"));
                    invoiceChallan.Completed = Convert.ToBoolean(dataReader["Completed"]);

                    retVal.Add(invoiceChallan);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single InvoiceChallan based on Id
        /// </summary>
        /// <param name="invoicechallanId">Id of the InvoiceChallan the needs to be retrieved</param>
        /// <returns>Instance of InvoiceChallan</returns>
        public InvoiceChallanInfo GetInvoiceChallan(int invoicechallanId)
        {
            InvoiceChallanInfo retVal = new InvoiceChallanInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [InvoiceID], [ChallanID] FROM InvoiceChallans WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, invoicechallanId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    retVal.ChallanID = Convert.ToInt64(dataReader["ChallanID"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new InvoiceChallan in to the database
        /// </summary>
        /// <param name="invoicechallan">Instance of InvoiceChallan</param>
        /// <returns>Id of the newly added InvoiceChallan</returns>
        public int AddInvoiceChallan(InvoiceChallanInfo invoiceChallan)
        {
            int retval = 0;
            DbCommand commandInvoiceChallan = _db.GetSqlStringCommand("INSERT INTO InvoiceChallans([InvoiceID], [ChallanID]) " +
                                                        "VALUES (@InvoiceID, @ChallanID) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('InvoiceChallans')");

            _db.AddInParameter(commandInvoiceChallan, "@InvoiceID", DbType.Int64, invoiceChallan.InvoiceID);
            _db.AddInParameter(commandInvoiceChallan, "@ChallanID", DbType.Int64, invoiceChallan.ChallanID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandInvoiceChallan));
            }
            return retval;
        }

        /// <summary>
        /// Updates the InvoiceChallan
        /// </summary>
        /// <param name="invoicechallan">Instance of InvoiceChallan class</param>
        public void UpdateInvoiceChallan(InvoiceChallanInfo invoiceChallan)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE InvoiceChallans SET [InvoiceID] = @InvoiceID, [ChallanID] = @ChallanID WHERE Id = @Id ");
            _db.AddInParameter(command, "@InvoiceID", DbType.Int64, invoiceChallan.InvoiceID);
            _db.AddInParameter(command, "@ChallanID", DbType.Int64, invoiceChallan.ChallanID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the InvoiceChallan from the database
        /// </summary>
        /// <param name="invoicechallanId">Id of the InvoiceChallan that needs to be deleted</param>
        public void DeleteInvoiceChallan(int invoicechallanId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM InvoiceChallans " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, invoicechallanId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

    }
}