using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    /// <summary>
    /// Author : Jitendra
    /// Date   : 22 July 2015
    /// </summary>
    public class DivisionManager
    {
        public BindingList<DivisionInfo> GetAllDivision()
        {
            DivisionDAL dal = new DivisionDAL();
            return dal.GetAllDivision();
        }

        public BindingList<DivisionInfo> GetAllDivisionByUserID(int userId)
        {
            DivisionDAL dal = new DivisionDAL();
            return dal.GetAllDivisionByUserID(userId);
        }
    }
}
