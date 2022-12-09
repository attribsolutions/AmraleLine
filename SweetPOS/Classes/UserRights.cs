using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SweetPOS.Classes
{
    public class UserRights
    {
        #region Class level variables...

        Control c = null;
        EnumClass.UserRoles role = EnumClass.UserRoles.Administrator;

        #endregion

        public UserRights(EnumClass.UserRoles userRole, Control control)
        {
            c = control;
            role = userRole;

            HideMenuButtons();
        }

        private void GetUserRights()
        {

        }

        private void HideMenuButtons()
        {
            if (c.Name == "frmMain" && role== EnumClass.UserRoles.Cashier)
            {
                
            }
        }
    }
}
