using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    /// <summary>
    /// Author : Amol (13 Sept 2016)
    /// </summary>
    public class LineManager
    {
        public int AddLine(LineInfo line)
        {
            int retVal = 0;
            LineDAL dal = new LineDAL();
            retVal = dal.AddLine(line);
            dal = null;
            return retVal;
        }

        public BindingList<LineInfo> GetLinesByFilter(string lineName, int count)
        {
            BindingList<LineInfo> retval = new BindingList<LineInfo>();
            LineDAL dal = new LineDAL();
            retval = dal.GetLinesByFilter(lineName, count);
            dal = null;
            return retval;
        }

        public void UpdateLine(LineInfo line)
        {
            LineDAL dal = new LineDAL();
            dal.UpdateLine(line);
            dal = null;
        }

        public void DeleteLine(int lineID)
        {
            LineDAL dal = new LineDAL();
            dal.DeleteLine(lineID);
            dal = null;
        }

        public BindingList<LineInfo> GetLines()
        {
            BindingList<LineInfo> retval = new BindingList<LineInfo>();
            LineDAL dal = new LineDAL();
            retval = dal.GetLines();
            dal = null;
            return retval;
        }

        public bool CheckLineUsed(int lineID, out string tableName)
        {
            LineDAL dal = new LineDAL();
            return dal.CheckLineUsed(lineID, out tableName);
        }
    }
}
