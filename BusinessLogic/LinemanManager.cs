using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;


namespace BusinessLogic
{
    /// <summary>
    /// Amol (14 Sept 2016)
    /// </summary>
    public class LinemanManager
    {

        public int AddLineman(LinemanInfo lineman)
        {
            int retval = 0;
            LinemanDAL dal = new LinemanDAL();
            retval = dal.AddLineman(lineman);
            dal = null;
            return retval;
        }

        public void UpdateLineman(LinemanInfo lineman)
        {
            LinemanDAL dal = new LinemanDAL();
            dal.UpdateLineman(lineman);
            dal = null;
        }

        public BindingList<LinemanInfo> GetLinemanByFilter(string LinemanName, int count)
        {
            BindingList<LinemanInfo> retval = new BindingList<LinemanInfo>();
            LinemanDAL dal = new LinemanDAL();
            retval = dal.GetLinemanByFilter(LinemanName, count);
            dal = null;
            return retval;
        }

        public void DeleteLineman(int linemanID)
        {
            LinemanDAL dal = new LinemanDAL();
            dal.DeleteLineman(linemanID);
            dal = null;
        }



        public BindingList<LinemanInfo> GetLinemans()
        {
            BindingList<LinemanInfo> retval = new BindingList<LinemanInfo>();
            LinemanDAL dal = new LinemanDAL();
            retval = dal.GetLinemans();
            dal = null;
            return retval;
        }

       
    }
}
