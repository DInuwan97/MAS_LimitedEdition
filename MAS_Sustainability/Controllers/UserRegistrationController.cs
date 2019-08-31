﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;


namespace MAS_Sustainability.Controllers
{
    public class UserRegistrationController : Controller
    {
        // GET: UserRegistration
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserRegistration/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserRegistration/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRegistration/Create
        [HttpPost]
        public ActionResult Create(UserRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();
            List<UserRegistrationModel> ForgotDetails = new List<UserRegistrationModel>();
            // String connectionString = @"server=localhost;port=3306;user id=root;database=mas_isscs;password=ThirtyFirst9731@;";


            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry = "INSERT INTO users(UserName,UserEmail,UserMobile,Password,ConfirmPassword,UserDepartment,UserType,SecretKey,Validation,UserImage) VALUES(@UserName,@UserEmail,@UserMobile,@Password,@ConfirmPassword,@UserDepartment,@UserType,@SecretKey,@Validation,@UserImage)";
                MySqlCommand mySqlcmd = new MySqlCommand(qry, mySqlCon);

             

                mySqlcmd.Parameters.AddWithValue("@UserName", userRegistrationModel.UserFullName);
                mySqlcmd.Parameters.AddWithValue("@UserEmail", userRegistrationModel.UserEmail);
                mySqlcmd.Parameters.AddWithValue("@UserMobile", userRegistrationModel.UserMobile);
                mySqlcmd.Parameters.AddWithValue("@Password", userRegistrationModel.Password);
                mySqlcmd.Parameters.AddWithValue("@ConfirmPassword", userRegistrationModel.ConfirmPassword);
                mySqlcmd.Parameters.AddWithValue("@UserDepartment", userRegistrationModel.UserDepartment);
                mySqlcmd.Parameters.AddWithValue("@UserType", "Employee");
                mySqlcmd.Parameters.AddWithValue("@UserImage", "NULL");

                Session["forgotEmail"] = userRegistrationModel.UserEmail;
                Session["forgotMobile"] = Convert.ToInt32(userRegistrationModel.UserMobile);

                int mobileDigists = Convert.ToInt32(userRegistrationModel.UserMobile) % 10000;
                userRegistrationModel.SecretKey = mobileDigists + Convert.ToInt32(DateTime.Now.ToString("yymmssfff"));

                mySqlcmd.Parameters.AddWithValue("@SecretKey", userRegistrationModel.SecretKey);
                mySqlcmd.Parameters.AddWithValue("@Validation", "false");

                string UserName = "0766061689"; //acount username
                string Password = "4873"; //account password
                string PhoneNo =  "94"+userRegistrationModel.UserMobile.ToString();
                string Message = "Hello " +userRegistrationModel.UserFullName+". Welcome to MAS IMS.Your Security Code : " + userRegistrationModel.SecretKey.ToString();

                string url = @"http://api.liyanagegroup.com/sms_api.php?sms=" + @Message + "&to=" + @PhoneNo + "&usr=" + @UserName + "&pw=" + @Password;
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string urlText = reader.ReadToEnd(); //it takes the response from your url. now you can use as your need 

                if (urlText == "OK")
                {
                    Response.Write("SMS Sent..!");
                }
                else
                {
                    Response.Write("SMS Sent Fail.!");
                }

                mySqlcmd.ExecuteNonQuery();


               


            }

            return RedirectToAction("SecureCode","UserRegistration");
        }

        // GET: UserRegistration/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserRegistration/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRegistration/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRegistration/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult VerifyRegistration()
        {

            return View();
        }


        public ActionResult ForgottenPassword(UserRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();
            DataTable dt = new DataTable();

            List<UserRegistrationModel> ForgotDetails = new List<UserRegistrationModel>();

            String userEmail, userMobile;


            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                userEmail = userRegistrationModel.UserEmail;
                userMobile = userRegistrationModel.UserMobile;
                int userID = userRegistrationModel.UserID;

                MySqlCommand mySqlCmd = mySqlCon.CreateCommand();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                mySqlCmd.CommandText = "SELECT UserID,UserEmail,UserMobile FROM users WHERE UserEmail = '"+userEmail+"'";
                mySqlCmd.ExecuteNonQuery();
               
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(mySqlCmd);
                mySqlDA.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    Session["forgotEmail"] = userRegistrationModel.UserEmail;
                    Session["forgotMobile"] = Convert.ToInt32(userRegistrationModel.UserMobile);
                    return RedirectToAction("SecureCode", "UserRegistration");
                }

            }
        

            return View();
        }


        public ActionResult SecureCode(UserRegistrationModel userRegistrationModel)
        {

            DB dbConn = new DB();
            MainModel mainModel = new MainModel();
            DataTable dt = new DataTable();

            String userEmail,userMobile;

            List<UserRegistrationModel> ForgotDetails = new List<UserRegistrationModel>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                userEmail = userRegistrationModel.UserEmail;

                String qry = "SELECT UserID,UserMobile FROM users WHERE UserEmail = '"+ Session["forgotEmail"] + "'";
                MySqlDataAdapter mySqlData_UserList = new MySqlDataAdapter(qry, mySqlCon);
                mySqlData_UserList.Fill(dt);

                if (dt.Rows.Count == 1)
                {


                    ForgotDetails.Add(new UserRegistrationModel
                    {
                        UserID = Convert.ToInt32(dt.Rows[0][0].ToString()),
                        UserMobile = dt.Rows[0][1].ToString()
                    });

                    mainModel.ForgottenDetails = ForgotDetails;

                    return View(mainModel);
                }


            }

            return View();
        }

        public ActionResult Validate(UserRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();
           

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                String UserMobile = Session["forgotMobile"].ToString();
                int UserID = userRegistrationModel.UserID;
                String SecretKey = userRegistrationModel.SecretKey.ToString();

                mySqlCon.Open();
                MySqlCommand mySqlCmd = mySqlCon.CreateCommand();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = "SELECT UserID,SecretKey,UserMobile FROM users WHERE SecretKey = '"+SecretKey+"' and UserID = '"+UserID+"'";
                mySqlCmd.ExecuteNonQuery();
                DataTable dtblToken = new DataTable();
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(mySqlCmd);
                mySqlDa.Fill(dtblToken);

            

                foreach(DataRow dr in dtblToken.Rows)
                {
                    if(dtblToken.Rows[0][1].Equals(userRegistrationModel.SecretKey))
                    {
                        String qry_update_validation = "UPDATE users SET Validation = 'true' WHERE UserID = @UserID";
                        MySqlCommand mySqlCommand_update_validity = new MySqlCommand(qry_update_validation, mySqlCon);
                        mySqlCommand_update_validity.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);
                        mySqlCommand_update_validity.Parameters.AddWithValue("@UserMobile", UserMobile);

                        mySqlCommand_update_validity.ExecuteNonQuery();

                        return RedirectToAction("Login", "UserLogin");

                    }
                    else
                    {
                        //Secret Key Mismatching
                    }
                }//invalid email
            }
            return View();    
        }


    }
}