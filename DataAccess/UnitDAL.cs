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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Unit table
    /// Author	: Kiran
    /// Date	: 17 Apr 2010 03:17:10 PM
    /// </summary>
    public class UnitDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public UnitDAL()
        {

        }
        /// <summary>
        /// Gets all the Units from the Units table
        /// </summary>
        /// <returns>BindingList of Units</returns>
        public BindingList<UnitInfo> GetUnitsAll()
        {
            BindingList<UnitInfo> retVal = new BindingList<UnitInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name] FROM Units ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    UnitInfo unit = new UnitInfo();
                    unit.ID = Convert.ToInt32(dataReader["ID"]);
                    unit.Name = Convert.ToString(dataReader["Name"]);

                    retVal.Add(unit);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Unit based on Id
        /// </summary>
        /// <param name="unitId">Id of the Unit the needs to be retrieved</param>
        /// <returns>Instance of Unit</returns>
        public UnitInfo GetUnit(int unitId)
        {
            UnitInfo retVal = new UnitInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name] FROM Units WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, unitId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Unit in to the database
        /// </summary>
        /// <param name="unit">Instance of Unit</param>
        /// <returns>Id of the newly added Unit</returns>
        public int AddUnit(UnitInfo unit)
        {
            int retval = 0;
            DbCommand commandUnit = _db.GetSqlStringCommand("INSERT INTO Units([Name]) " +
                                                        "VALUES (@Name) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Units')");

            _db.AddInParameter(commandUnit, "@Name", DbType.String, unit.Name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandUnit));
            }
            return retval;
        }

        /// <summary>
        /// Updates the Unit
        /// </summary>
        /// <param name="unit">Instance of Unit class</param>
        public void UpdateUnit(UnitInfo unit)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Units SET [Name] = @Name WHERE Id = @Id ");
            _db.AddInParameter(command, "@Name", DbType.String, unit.Name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the Unit from the database
        /// </summary>
        /// <param name="unitId">Id of the Unit that needs to be deleted</param>
        public void DeleteUnit(int unitId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Units " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, unitId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

    }
}