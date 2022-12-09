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
    /// Author : Jitendra
    /// Date   : 22 July 2015
    /// </summary>
    public class DivisionDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public BindingList<DivisionInfo> GetAllDivisionByUserID(int userId)
        {
            BindingList<DivisionInfo> retVal = new BindingList<DivisionInfo>();
            try
            {
                if (userId != 0)
                {
                    DbCommand command = null;

                    command = _db.GetSqlStringCommand("SELECT Divisions.[ID], [DivisionName] FROM [Divisions] JOIN UserDivisions On Divisions.ID = UserDivisions.DivisionID WHERE UserID = @UserID ");
                    _db.AddInParameter(command, "@UserID", DbType.Int32, userId);
                    using (IDataReader dataReader = _db.ExecuteReader(command))
                    {
                        while (dataReader.Read())
                        {
                            DivisionInfo division = new DivisionInfo();
                            division.DivisionID = Convert.ToInt32(dataReader["ID"]);
                            division.DivisionName = Convert.ToString(dataReader["DivisionName"]);

                            retVal.Add(division);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return retVal;
        }
        public BindingList<DivisionInfo> GetAllDivision()
        {
            BindingList<DivisionInfo> retVal = new BindingList<DivisionInfo>();
            DbCommand command = null;

            command = _db.GetSqlStringCommand("SELECT [ID], [DivisionName] FROM [Divisions] ");
            
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    DivisionInfo division = new DivisionInfo();
                    division.DivisionID = Convert.ToInt32(dataReader["ID"]);
                    division.DivisionName = Convert.ToString(dataReader["DivisionName"]);

                    retVal.Add(division);
                }
            }
            return retVal;
        }

    }
}
