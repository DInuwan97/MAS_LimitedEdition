using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace MAS_Sustainability.Controllers
{
    public class UserLoginController : Controller
    {
        // GET: UserLogin
        public ActionResult Login()
        {
            UserLogin userLogin = new UserLogin();
            userLogin.SuccesMsg = 0;

           // Response.Write("Invalid Credentials");
            return View(userLogin);
        }

        [HttpPost]
        public ActionResult Login(UserLogin userLogin)
        {
            userLogin.SuccesMsg = 0;
            DB dbConn = new DB();
            MySqlConnection mySqlCon = dbConn.DBConnection();

            using (mySqlCon)
            {
                mySqlCon.Open();
                userLogin.SuccesMsg = 0;
                String userEmail = userLogin.LoginUserEmail;
                String userPassword = userLogin.LoginUserPassword;
                //  String qry = "SELECT UserEmail,Password FROM users WHERE UserEmail = '"+userEmail+"' AND Password = '"+userPassword+"' ";
                MySqlCommand mySqlCmd = mySqlCon.CreateCommand();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = "SELECT UserEmail,Password FROM users WHERE UserEmail = '" + userEmail + "' AND Password = '" + userPassword + "' and Validation = 'true' ";
                mySqlCmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(mySqlCmd);
                mySqlDA.Fill(dt);

                DataTable loginDetailsDataTable = new DataTable();


                foreach (DataRow dr in dt.Rows)
                {
                    userLogin.SuccesMsg = 1;
                    //Session["user"] = new UserLogin() { Login = userLogin.LoginUserEmail, LoginUserEmail = userLogin.LoginUserEmail };
                    Session["user"] = userLogin.LoginUserEmail;


                    String qry_get_login_details = "SELECT * FROM login_details WHERE UserEmail = '" + Session["user"] + "' ";
                    MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_get_login_details, mySqlCon);
                    mySqlDa.Fill(loginDetailsDataTable);

                    if(loginDetailsDataTable.Rows.Count == 1)
                    {
                        String qry_update_login_details = "UPDATE login_details SET LastLoggedDate = CURDATE(),LastLoggedTime = CURTIME()  WHERE UserEmail = '" + Session["user"] + "'";
                        MySqlCommand mySqlCmd_update_login_details = new MySqlCommand(qry_update_login_details, mySqlCon);
                        mySqlCmd_update_login_details.ExecuteNonQuery();

                    }
                    else
                    {
                        String qry_insert_login_details = "INSERT INTO login_details(UserEmail,FirstLoggedDate,FirstLoggedtIME,LastLoggedDate,LastLoggedTime)VALUES('" + Session["user"]+ "',CURDATE(),CURTIME(),CURDATE(),CURTIME())";
                        MySqlCommand mySqlCmd_insert_login_details = new MySqlCommand(qry_insert_login_details, mySqlCon);
                        mySqlCmd_insert_login_details.ExecuteNonQuery();

                    }


                    return RedirectToAction("Index", "Dashbord");
                }

                //if(userLogin.SuccesMsg)
            }


            Response.Write("Invalid Credentials");
            return View(userLogin);
        }

        public ActionResult Logout()
        {
            UserLogin userLogin = new UserLogin();
            // Session.Clear();
            Session["user"] = null;
            userLogin.SuccesMsg = 0;
           
            return RedirectToAction("Login", "UserLogin");
        }
    }
}