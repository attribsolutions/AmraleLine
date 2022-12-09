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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Supplier table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:19:05 PM
    /// </summary>
    public class SupplierDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public SupplierDAL()
        {

        }
        /// <summary>
        /// Gets all the Suppliers from the Suppliers table
        /// </summary>
        /// <returns>BindingList of Suppliers</returns>
        public BindingList<SupplierInfo> GetSuppliersAll(bool showActiveOnly)
        {
            BindingList<SupplierInfo> retVal = new BindingList<SupplierInfo>();
            DbCommand command = null;
            if (showActiveOnly)
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Suppliers WHERE IsActive = 'True' ORDER BY [Name]");
            else
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Suppliers ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SupplierInfo supplier = new SupplierInfo();
                    supplier.ID = Convert.ToInt32(dataReader["ID"]);
                    supplier.Name = Convert.ToString(dataReader["Name"]);
                    supplier.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        supplier.Address = string.Empty;
                    }
                    else
                    {
                        supplier.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        supplier.City = string.Empty;
                    }
                    else
                    {
                        supplier.City = Convert.ToString(dataReader["City"]);
                    }
                    if (dataReader["State"] == DBNull.Value)
                    {
                        supplier.State = string.Empty;
                    }
                    else
                    {
                        supplier.State = Convert.ToString(dataReader["State"]);
                    }
                    if (dataReader["VatTinNo"] == DBNull.Value)
                    {
                        supplier.VatTinNo = string.Empty;
                    }
                    else
                    {
                        supplier.VatTinNo = Convert.ToString(dataReader["VatTinNo"]);
                    }
                    if (dataReader["Phone"] == DBNull.Value)
                    {
                        supplier.Phone = string.Empty;
                    }
                    else
                    {
                        supplier.Phone = Convert.ToString(dataReader["Phone"]);
                    }
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        supplier.Mobile = string.Empty;
                    }
                    else
                    {
                        supplier.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        supplier.EMail = string.Empty;
                    }
                    else
                    {
                        supplier.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    supplier.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    supplier.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    supplier.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    supplier.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    supplier.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    supplier.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(supplier);
                }
            }
            return retVal;
        }

        public BindingList<SupplierInfo> GetSuppliersByName(string supplierName, bool showActiveOnly)
        {
            BindingList<SupplierInfo> retVal = new BindingList<SupplierInfo>();
            DbCommand command = null;
            if (showActiveOnly)
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Suppliers WHERE IsActive = 'True' Name LIKE '" + supplierName + "%' ORDER BY [Name]");
            else
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Suppliers WHERE Name LIKE '" + supplierName + "%' ORDER BY [Name]");

                using (IDataReader dataReader = _db.ExecuteReader(command))
                {
                    while (dataReader.Read())
                    {
                        SupplierInfo supplier = new SupplierInfo();
                        supplier.ID = Convert.ToInt32(dataReader["ID"]);
                        supplier.Name = Convert.ToString(dataReader["Name"]);
                        supplier.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                        if (dataReader["Address"] == DBNull.Value)
                        {
                            supplier.Address = string.Empty;
                        }
                        else
                        {
                            supplier.Address = Convert.ToString(dataReader["Address"]);
                        }
                        if (dataReader["City"] == DBNull.Value)
                        {
                            supplier.City = string.Empty;
                        }
                        else
                        {
                            supplier.City = Convert.ToString(dataReader["City"]);
                        }
                        if (dataReader["State"] == DBNull.Value)
                        {
                            supplier.State = string.Empty;
                        }
                        else
                        {
                            supplier.State = Convert.ToString(dataReader["State"]);
                        }
                        if (dataReader["VatTinNo"] == DBNull.Value)
                        {
                            supplier.VatTinNo = string.Empty;
                        }
                        else
                        {
                            supplier.VatTinNo = Convert.ToString(dataReader["VatTinNo"]);
                        }
                        if (dataReader["Phone"] == DBNull.Value)
                        {
                            supplier.Phone = string.Empty;
                        }
                        else
                        {
                            supplier.Phone = Convert.ToString(dataReader["Phone"]);
                        }
                        if (dataReader["Mobile"] == DBNull.Value)
                        {
                            supplier.Mobile = string.Empty;
                        }
                        else
                        {
                            supplier.Mobile = Convert.ToString(dataReader["Mobile"]);
                        }
                        if (dataReader["EMail"] == DBNull.Value)
                        {
                            supplier.EMail = string.Empty;
                        }
                        else
                        {
                            supplier.EMail = Convert.ToString(dataReader["EMail"]);
                        }
                        supplier.Balance = Convert.ToDecimal(dataReader["Balance"]);
                        supplier.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                        supplier.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                        supplier.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                        supplier.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                        supplier.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                        retVal.Add(supplier);
                    }
                }
            return retVal;
        }

        /// <summary>
        /// Gets single Supplier based on Id
        /// </summary>
        /// <param name="supplierId">Id of the Supplier the needs to be retrieved</param>
        /// <returns>Instance of Supplier</returns>
        public SupplierInfo GetSupplier(int supplierId)
        {
            SupplierInfo retVal = new SupplierInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Suppliers WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, supplierId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        retVal.Address = string.Empty;
                    }
                    else
                    {
                        retVal.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        retVal.City = string.Empty;
                    }
                    else
                    {
                        retVal.City = Convert.ToString(dataReader["City"]);
                    }
                    if (dataReader["State"] == DBNull.Value)
                    {
                        retVal.State = string.Empty;
                    }
                    else
                    {
                        retVal.State = Convert.ToString(dataReader["State"]);
                    }
                    if (dataReader["VatTinNo"] == DBNull.Value)
                    {
                        retVal.VatTinNo = string.Empty;
                    }
                    else
                    {
                        retVal.VatTinNo = Convert.ToString(dataReader["VatTinNo"]);
                    }
                    if (dataReader["Phone"] == DBNull.Value)
                    {
                        retVal.Phone = string.Empty;
                    }
                    else
                    {
                        retVal.Phone = Convert.ToString(dataReader["Phone"]);
                    }
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        retVal.Mobile = string.Empty;
                    }
                    else
                    {
                        retVal.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        retVal.EMail = string.Empty;
                    }
                    else
                    {
                        retVal.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    retVal.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Supplier in to the database
        /// </summary>
        /// <param name="supplier">Instance of Supplier</param>
        /// <returns>Id of the newly added Supplier</returns>
        public int AddSupplier(SupplierInfo supplier)
        {
            int retval = 0;
            DbCommand commandSupplier = _db.GetSqlStringCommand("INSERT INTO Suppliers([Name], [ContactPerson], [Address], [City], [State], [VatTinNo], [Phone], [Mobile], [EMail], [Balance], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@Name, @ContactPerson, @Address, @City, @State, @VatTinNo, @Phone, @Mobile, @EMail, @Balance, @IsActive, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Suppliers')");

            _db.AddInParameter(commandSupplier, "@Name", DbType.String, supplier.Name);
            _db.AddInParameter(commandSupplier, "@ContactPerson", DbType.String, supplier.ContactPerson);
            if (supplier.Address == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@Address", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@Address", DbType.String, supplier.Address);
            }
            if (supplier.City == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@City", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@City", DbType.String, supplier.City);
            }
            if (supplier.State == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@State", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@State", DbType.String, supplier.State);
            }
            if (supplier.VatTinNo == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@VatTinNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@VatTinNo", DbType.String, supplier.VatTinNo);
            }
            if (supplier.Phone == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@Phone", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@Phone", DbType.String, supplier.Phone);
            }
            if (supplier.Mobile == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@Mobile", DbType.String, supplier.Mobile);
            }
            if (supplier.EMail == string.Empty)
            {
                _db.AddInParameter(commandSupplier, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSupplier, "@EMail", DbType.String, supplier.EMail);
            }
            _db.AddInParameter(commandSupplier, "@Balance", DbType.Decimal, supplier.Balance);
            _db.AddInParameter(commandSupplier, "@IsActive", DbType.Boolean, supplier.IsActive);
            _db.AddInParameter(commandSupplier, "@CreatedBy", DbType.Byte, supplier.CreatedBy);
            _db.AddInParameter(commandSupplier, "@CreatedOn", DbType.DateTime, supplier.CreatedOn);
            _db.AddInParameter(commandSupplier, "@UpdatedBy", DbType.Byte, supplier.UpdatedBy);
            _db.AddInParameter(commandSupplier, "@UpdatedOn", DbType.DateTime, supplier.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandSupplier));
            }
            return retval;
        }

        /// <summary>
        /// Updates the Supplier
        /// </summary>
        /// <param name="supplier">Instance of Supplier class</param>
        public void UpdateSupplier(SupplierInfo supplier)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Suppliers SET [Name] = @Name, [ContactPerson] = @ContactPerson, [Address] = @Address, [City] = @City, [State] = @State, [VatTinNo] = @VatTinNo, [Phone] = @Phone, [Mobile] = @Mobile, [EMail] = @EMail, [Balance] = @Balance, [IsActive] = @IsActive, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, supplier.ID);
            _db.AddInParameter(command, "@Name", DbType.String, supplier.Name);
            _db.AddInParameter(command, "@ContactPerson", DbType.String, supplier.ContactPerson);
            if (supplier.Address == string.Empty)
            {
                _db.AddInParameter(command, "@Address", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Address", DbType.String, supplier.Address);
            }
            if (supplier.City == string.Empty)
            {
                _db.AddInParameter(command, "@City", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@City", DbType.String, supplier.City);
            }
            if (supplier.State == string.Empty)
            {
                _db.AddInParameter(command, "@State", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@State", DbType.String, supplier.State);
            }
            if (supplier.VatTinNo == string.Empty)
            {
                _db.AddInParameter(command, "@VatTinNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@VatTinNo", DbType.String, supplier.VatTinNo);
            }
            if (supplier.Phone == string.Empty)
            {
                _db.AddInParameter(command, "@Phone", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Phone", DbType.String, supplier.Phone);
            }
            if (supplier.Mobile == string.Empty)
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, supplier.Mobile);
            }
            if (supplier.EMail == string.Empty)
            {
                _db.AddInParameter(command, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@EMail", DbType.String, supplier.EMail);
            }
            _db.AddInParameter(command, "@Balance", DbType.Decimal, supplier.Balance);
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, supplier.IsActive);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, supplier.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, supplier.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the Supplier from the database
        /// </summary>
        /// <param name="supplierId">Id of the Supplier that needs to be deleted</param>
        public void DeleteSupplier(int supplierId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Suppliers " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, supplierId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Gets the count of Suppliers having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="supplierName">Name of the Supplier to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Suppliers] WHERE [Name] = @Name");
            _db.AddInParameter(command, "@Name", DbType.String, name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of Suppliers having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="supplierName">Name of the Supplier to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int supplierId)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Suppliers] WHERE [Name] = @Name AND Id != @Id");
            _db.AddInParameter(command, "@Name", DbType.String, name);
            _db.AddInParameter(command, "@Id", DbType.Int32, supplierId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        public bool CheckSupplierUsed(int supplierId, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(SupplierID) FROM Challans WHERE SupplierID = @SupplierId");
            _db.AddInParameter(command, "@SupplierId", DbType.Int32, supplierId);
            DbCommand command1 = _db.GetSqlStringCommand("SELECT COUNT(SupplierID) FROM Invoices WHERE SupplierID = @SupplierId");
            _db.AddInParameter(command1, "@SupplierId", DbType.Int32, supplierId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    tableName = "Challans";
                    return true;
                }
                else if (Convert.ToInt32(_db.ExecuteScalar(command1)) > 0)
                {
                    tableName = "Invoices";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }
    }
}