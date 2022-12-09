using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Message data
    /// Author	: Akash
    /// Date	: 08 June 2017 11:16:36 AM
    /// </summary>
     public class MessageInfo
    {
        public MessageInfo()
        {
        }

        System.Int32 _id=0;
        System.String _message = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
       
        

        public System.Int32 ID
        {
            get { return _id; }
            set{_id=value;}
        }
        public System.String Message
        {
            get { return _message; }
            set { _message = value; }
        
        }

        public System.Byte CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public System.DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        public System.Byte UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        public System.DateTime UpdatedOn
        {
            get { return _updatedOn; }
            set { _updatedOn = value; }
        }

  



    }

    
}
