using MAS_Sustainability.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;

namespace MAS_Sustainability.Controllers
{
    public class StaffRegistrationController : Controller
    {
        // GET: StaffRegistration
        public ActionResult Index()
        {
            return View();
        }

        // GET: StaffRegistration/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StaffRegistration/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StaffRegistration/Create
        [HttpPost]
        public ActionResult Create(StaffRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();

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
                mySqlcmd.Parameters.AddWithValue("@UserType",userRegistrationModel.UserType);
                mySqlcmd.Parameters.AddWithValue("@UserImage", "NULL");

                Session["forgotEmail"] = userRegistrationModel.UserEmail;
                Session["forgotMobile"] = Convert.ToInt32(userRegistrationModel.UserMobile);

                int mobileDigists = Convert.ToInt32(userRegistrationModel.UserMobile) % 10000;
                userRegistrationModel.SecretKey = mobileDigists + Convert.ToInt32(DateTime.Now.ToString("yymmssfff"));

                mySqlcmd.Parameters.AddWithValue("@SecretKey", userRegistrationModel.SecretKey);
                mySqlcmd.Parameters.AddWithValue("@Validation", "false");

                string UserName = "0766061689"; //acount username
                string Password = "4873"; //account password
                string PhoneNo = "94" + userRegistrationModel.UserMobile.ToString();
                string Message = "Hello " + userRegistrationModel.UserFullName + ". Welcome to MAS IMS.Your Security Code : " + userRegistrationModel.SecretKey.ToString();

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

            return RedirectToAction("SecureCode", "UserRegistration");


        }

    

        

        // GET: StaffRegistration/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StaffRegistration/Edit/5
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

        // GET: StaffRegistration/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StaffRegistration/Delete/5
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
    }
}
