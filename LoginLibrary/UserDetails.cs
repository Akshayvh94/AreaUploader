using LoginLibrary.AzureEF;
//using LoginLibrary.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginLibrary
{
    public class UserDetails
    {
        AreaUploaderEntities1 AreaLoaderDB = new AreaUploaderEntities1();
        public string ErrorMessage = string.Empty;
        public User GetLoginDetails(string username, string Password)
        {
            User msgStatus = new User();

            try
            {
                List<User> userList = AreaLoaderDB.Users.ToList();

                msgStatus = (from s in userList
                             where s.Username.ToLower() == username.ToLower() && Encoding.UTF8.GetString(s.Password) == Password
                             select s).FirstOrDefault();

            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return null;
            }
            return msgStatus;
        }
        public string AddUser(string userName, string email, string password)
        {
            string returnMsg = "";
            try
            {
                User objEmpDetail = new User();
                objEmpDetail.Username = userName;
               // objEmpDetail.AccountName = acctName;
                objEmpDetail.Email = email;
                objEmpDetail.Password = Encoding.UTF8.GetBytes(password);
                AreaLoaderDB.Users.Add(objEmpDetail);
                AreaLoaderDB.SaveChanges();
                returnMsg = "Success";
            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
            }
            return returnMsg;
        }
        public string GetUserExist(string sUserName, string sEmail, int iUserId = 0)
        {
            int eCount = 0;
            int uCount = 0;

            //if (aCount == 0)
            //{
            //    aCount = AreaLoaderDB.Users.Where(x => x.AccountName.ToLower().Trim() == sAccountName.ToLower().Trim()).Count();
            //}
            if (uCount == 0)
            {
                uCount = AreaLoaderDB.Users.Where(x => x.Username.ToLower().Trim() == sUserName.ToLower().Trim()).Count();
            }
            if (eCount == 0)
            {
                eCount = AreaLoaderDB.Users.Where(x => x.Email.ToLower().Trim() == sEmail.ToLower().Trim()).Count();
            }

            //if (aCount > 0)
            //{
            //    return "Accountname Found";
            //}
            if (uCount > 0)
            {
                return "Username Found";
            }
            else if (eCount > 0)
            {
                return "Email Found";
            }
            else
            {
                return "Not Found";
            }
        }
    }

}
