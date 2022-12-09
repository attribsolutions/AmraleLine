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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Counter table
    /// Author	: Kiran
    /// Date	: 01 Mar 2010 12:50:15 PM
    /// </summary>
    public class CounterDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public CounterDAL()
        {

        }
        /// <summary>
        /// Gets all the Counters from the Counters table
        /// </summary>
        /// <returns>BindingList of Counters</returns>
        public BindingList<CounterInfo> GetCountersAll()
        {
            BindingList<CounterInfo> retVal = new BindingList<CounterInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Password], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Counters ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    CounterInfo counter = new CounterInfo();
                    counter.ID = Convert.ToInt32(dataReader["ID"]);
                    counter.Name = Convert.ToString(dataReader["Name"]);
                    counter.Password = Convert.ToString(dataReader["Password"]);
                    counter.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    counter.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    counter.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    counter.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(counter);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Counter based on Id
        /// </summary>
        /// <param name="counterId">Id of the Counter the needs to be retrieved</param>
        /// <returns>Instance of Counter</returns>
        public CounterInfo GetCounter(int counterId)
        {
            CounterInfo retVal = new CounterInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Password], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Counters WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, counterId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.Password = Convert.ToString(dataReader["Password"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Counter in to the database
        /// </summary>
        /// <param name="counter">Instance of Counter</param>
        /// <returns>Id of the newly added Counter</returns>
        public int AddCounter(CounterInfo counter)
        {
            int retval = 0;
            DbCommand commandCounter = _db.GetSqlStringCommand("INSERT INTO Counters([Name], [Password], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@Name, @Password, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Counters')");

            _db.AddInParameter(commandCounter, "@Name", DbType.String, counter.Name);
            _db.AddInParameter(commandCounter, "@Password", DbType.String, counter.Password);
            _db.AddInParameter(commandCounter, "@CreatedBy", DbType.Byte, counter.CreatedBy);
            _db.AddInParameter(commandCounter, "@CreatedOn", DbType.DateTime, counter.CreatedOn);
            _db.AddInParameter(commandCounter, "@UpdatedBy", DbType.Byte, counter.UpdatedBy);
            _db.AddInParameter(commandCounter, "@UpdatedOn", DbType.DateTime, counter.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    retval = Convert.ToInt32(_db.ExecuteScalar(commandCounter, transaction));

                    //Adds column in Items table by counter name.

                    DbCommand commandAddCounterInItems = _db.GetSqlStringCommand("ALTER TABLE Items ADD " + counter.Name + " Int CONSTRAINT DF_Items_" + counter.Name + " DEFAULT ((0)) NOT NULL");
                    _db.ExecuteScalar(commandAddCounterInItems, transaction);

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
        /// Updates the Counter
        /// </summary>
        /// <param name="counter">Instance of Counter class</param>
        public void UpdateCounter(CounterInfo counter)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Counters SET [Name] = @Name, [Password] = @Password, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");
            
            _db.AddInParameter(command, "@Id", DbType.Int32, counter.ID);
            _db.AddInParameter(command, "@Name", DbType.String, counter.Name);
            _db.AddInParameter(command, "@Password", DbType.String, counter.Password);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, counter.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, counter.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
               DbTransaction transaction = null;
               try
               {
                   conn.Open();
                   transaction = conn.BeginTransaction();
                   _db.ExecuteNonQuery(command, transaction);

                   //Modifies column in Items table.

                   DbCommand commandModifyCounterInItems = _db.GetSqlStringCommand("EXEC SP_RENAME @Objname = 'Items." + counter.OldName + "', @Newname = '" + counter.Name + "', @Objtype = 'COLUMN'");
                   _db.ExecuteScalar(commandModifyCounterInItems, transaction);

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
        /// Deletes the Counter from the database
        /// </summary>
        /// <param name="counterId">Id of the Counter that needs to be deleted</param>
        public void DeleteCounter(int counterId, string counterName)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Counters WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, counterId);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);

                    //Deletes column from Items table.

                    DbCommand commandDeleteCounterInItems = _db.GetSqlStringCommand("ALTER TABLE Items DROP CONSTRAINT DF_Items_" + counterName + " ALTER TABLE Items DROP COLUMN " + counterName);
                    _db.ExecuteScalar(commandDeleteCounterInItems, transaction);

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
        /// Gets the count of Counters having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="counterName">Name of the Counter to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Counters] WHERE [Name] = @Name");
            _db.AddInParameter(command, "@Name", DbType.String, name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of Counters having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="counterName">Name of the Counter to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int counterId)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Counters] WHERE [Name] = @Name AND Id != @Id");
            _db.AddInParameter(command, "@Name", DbType.String, name);
            _db.AddInParameter(command, "@Id", DbType.Int32, counterId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }
    }
}