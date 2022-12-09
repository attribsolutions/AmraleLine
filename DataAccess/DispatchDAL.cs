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
    public class DispatchDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public BindingList<ChallanItemInfo> GetChallanItemsByDate(DateTime challanDate)
        {
            BindingList<ChallanItemInfo> retVal = new BindingList<ChallanItemInfo>();
            DbCommand command = _db.GetSqlStringCommand(" SELECT ChallanItems.ID, ChallanItems.ChallanID, ChallanItems.ItemID, Items.Name ItemName, ChallanItems.Quantity, ChallanItems.UnitID, Units.Name FROM ChallanItems JOIN Challans ON ChallanID = Challans.ID "+
                                                        " JOIN Units ON ChallanItems.UnitID = Units.ID INNER JOIN Items ON ChallanItems.ItemID = Items.ID WHERE Challans.ChallanDate = @ChallanDate ORDER BY ChallanItems.ID ");
            _db.AddInParameter(command, "@ChallanDate", DbType.DateTime, challanDate);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ChallanItemInfo challanItem = new ChallanItemInfo();
                    challanItem.ID = Convert.ToInt64(dataReader["ID"]);
                    challanItem.ChallanID = Convert.ToInt64(dataReader["ChallanID"]);
                    challanItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    challanItem.ItemName = Convert.ToString(dataReader["ItemName"]);
                    challanItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    challanItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    challanItem.UnitName = Convert.ToString(dataReader["Name"]);
                    retVal.Add(challanItem);
                }
            }
            return retVal;
        }

        //public BindingList<ChallanInfo> GetChallansByFilter(int searchType, string name, object sDate, object eDate, int DivisionID)
        //{
        //    BindingList<ChallanInfo> retVal = new BindingList<ChallanInfo>();
        //    DbCommand command = null;
        //    string cmdString = "SELECT Challans.ID, Challans.ChallanDate, Challans.ChallanNo, Challans.SupplierID, Suppliers.Name SupplierName, Challans.ReceivedDate, Challans.VehicleNo, Challans.DeliveredBy, Challans.ReceivedBy, Challans.IsConfirmed, Challans.Description,Challans.DivisionID,Divisions.DivisionName, Challans.CreatedBy, Challans.CreatedOn, Challans.UpdatedBy, Challans.UpdatedOn FROM Challans INNER JOIN Suppliers ON Challans.SupplierID = Suppliers.ID  JOIN Divisions ON Challans.DivisionID = Divisions.ID ";

        //    if (searchType == 0)
        //        if (DivisionID == 0)
        //            command = _db.GetSqlStringCommand(cmdString + "WHERE Suppliers.Name LIKE '" + name + "%' ORDER BY Suppliers.Name");
        //        else
        //            command = _db.GetSqlStringCommand(cmdString + "WHERE DivisionID = @DivisionID AND Suppliers.Name LIKE '" + name + "%' ORDER BY Suppliers.Name");
        //    if (searchType == 1)
        //    {
        //        if (DivisionID == 0)
        //            command = _db.GetSqlStringCommand(cmdString + "WHERE Challans.ChallanDate >= @SDate AND Challans.ChallanDate <= @EDate ORDER BY Challans.ChallanDate DESC");
        //        else
        //            command = _db.GetSqlStringCommand(cmdString + "WHERE DivisionID = @DivisionID AND Challans.ChallanDate >= @SDate AND Challans.ChallanDate <= @EDate ORDER BY Challans.ChallanDate DESC");

        //        _db.AddInParameter(command, "@DivisionID", DbType.Int16, DivisionID);
        //        _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
        //        _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
        //    }

        //    using (IDataReader dataReader = _db.ExecuteReader(command))
        //    {
        //        while (dataReader.Read())
        //        {
        //            ChallanInfo challan = new ChallanInfo();
        //            challan.ID = Convert.ToInt64(dataReader["ID"]);
        //            challan.ChallanDate = Convert.ToDateTime(dataReader["ChallanDate"]);
        //            challan.ChallanNo = Convert.ToString(dataReader["ChallanNo"]);
        //            challan.SupplierID = Convert.ToInt32(dataReader["SupplierID"]);
        //            challan.SupplierName = Convert.ToString(dataReader["SupplierName"]);
        //            challan.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
        //            if (dataReader["VehicleNo"] == DBNull.Value)
        //            {
        //                challan.VehicleNo = string.Empty;
        //            }
        //            else
        //            {
        //                challan.VehicleNo = Convert.ToString(dataReader["VehicleNo"]);
        //            }
        //            if (dataReader["DeliveredBy"] == DBNull.Value)
        //            {
        //                challan.DeliveredBy = string.Empty;
        //            }
        //            else
        //            {
        //                challan.DeliveredBy = Convert.ToString(dataReader["DeliveredBy"]);
        //            }
        //            challan.ReceivedBy = Convert.ToInt32(dataReader["ReceivedBy"]);
        //            challan.IsConfirmed = Convert.ToBoolean(dataReader["IsConfirmed"]);
        //            if (dataReader["Description"] == DBNull.Value)
        //            {
        //                challan.Description = string.Empty;
        //            }
        //            else
        //            {
        //                challan.Description = Convert.ToString(dataReader["Description"]);
        //            }
        //            challan.DivisionID = Convert.ToInt16(dataReader["DivisionID"]);
        //            challan.DivisionName = Convert.ToString(dataReader["DivisionName"]);
        //            challan.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
        //            challan.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
        //            challan.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
        //            challan.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

        //            retVal.Add(challan);
        //        }
        //    }
        //    return retVal;
        //}

       

    }
}
