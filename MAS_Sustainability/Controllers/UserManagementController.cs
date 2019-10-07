using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Web.UI;


namespace MAS_Sustainability.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        public ActionResult Edit(int id)
        {
            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();
            MainModel mainModel = new MainModel();
            DataTable userDetailsDataTable = new DataTable();
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserMobile,UserDepartment,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);
            }

            for (int i = 0; i < userDetailsDataTable.Rows.Count; i++)
            {
                List_UserRegistration.Add(new UserRegistrationModel
                {
                    UserFullName = userDetailsDataTable.Rows[0][0].ToString(),
                    UserType = userDetailsDataTable.Rows[0][1].ToString(),
                    UserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]),
                    UserEmail = userDetailsDataTable.Rows[0][3].ToString(),
                    UserMobile = userDetailsDataTable.Rows[0][4].ToString(),
                    UserDepartment = userDetailsDataTable.Rows[0][5].ToString(),
                    UserImagePath = userDetailsDataTable.Rows[0][6].ToString()
                }
                );
            }

            if(userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][6].ToString();

                mainModel.ListUserRegistration = List_UserRegistration;
               
                return View(mainModel);
            }
            else
            {
                return View("Index");
            }



            
        }



        public ActionResult Index()
        {
            DataTable userDetailsDataTable = new DataTable();
            DataTable userListDataTable = new DataTable();
            MainModel mainModel = new MainModel();

            DataTable EmployeeCountDataTable = new DataTable();
            DataTable TokenManagerCountDataTable = new DataTable();
            DataTable DepartmentLeaderCountDataTable = new DataTable();
            DataTable FactoryManagementCountDataTable = new DataTable();

            // UserRegistrationModel userRegistrationModel = new UserRegistrationModel();

            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();

            List<UserRegistrationModel> List_UseCount = new List<UserRegistrationModel>();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserMobile,UserDepartment,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);


                String qry_listOfUsers = "SELECT * FROM users";
                MySqlDataAdapter mySqlData_UserList = new MySqlDataAdapter(qry_listOfUsers, mySqlCon);
                mySqlData_UserList.Fill(userListDataTable);

          
                String qry_get_count_Employees = "select count(UserID) FROM users WHERE UserType = 'Employee'";
                MySqlDataAdapter mySqlDataEmployee = new MySqlDataAdapter(qry_get_count_Employees,mySqlCon);
                mySqlDataEmployee.Fill(EmployeeCountDataTable);


                String qry_get_count_TokenManager = "select count(UserID) FROM users WHERE UserType = 'Token Manager'";
                MySqlDataAdapter mySqlDatTokenManager = new MySqlDataAdapter(qry_get_count_TokenManager, mySqlCon);
                mySqlDatTokenManager.Fill(TokenManagerCountDataTable);


                String qry_get_count_DepartmentLeader = "select count(UserID) FROM users WHERE UserType = 'Department Leader'";
                MySqlDataAdapter mySqlDataDepartmentLeader = new MySqlDataAdapter(qry_get_count_DepartmentLeader, mySqlCon);
                mySqlDataDepartmentLeader.Fill(DepartmentLeaderCountDataTable);


                mainModel.ArrFirstImagePath = new string[500];


            }

            if (Session["user"] == null || userDetailsDataTable.Rows[0][1].ToString() != ("Administrator"))
            {
                return RedirectToAction("Index", "Dashbord");
            }


            if (EmployeeCountDataTable.Rows.Count == 1)
            {

                List_UseCount.Add(new UserRegistrationModel
                {
                    EmployeeCount = Convert.ToInt32(EmployeeCountDataTable.Rows[0][0].ToString()),
                    TokenManagerCount = Convert.ToInt32(TokenManagerCountDataTable.Rows[0][0].ToString()),
                    DepartmentLeaderCount = Convert.ToInt32(DepartmentLeaderCountDataTable.Rows[0][0].ToString())
                });

                mainModel.UserContList = List_UseCount;


            }



            for (int i = 0; i < userListDataTable.Rows.Count; i++)
            {
               

                List_UserRegistration.Add(new UserRegistrationModel {

                    UserFullName = userListDataTable.Rows[i][1].ToString(),
                    UserType = userListDataTable.Rows[i][7].ToString(),
                    UserID = Convert.ToInt32(userListDataTable.Rows[i][0]),
                    UserEmail = userListDataTable.Rows[i][2].ToString(),
                    UserMobile = userListDataTable.Rows[i][3].ToString(),
                    UserDepartment = userListDataTable.Rows[i][6].ToString(),
                    UserImagePath = userListDataTable.Rows[i][8].ToString(),
                    UserValidity = userListDataTable.Rows[i][10].ToString()

                });

            }



            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][6].ToString();

                mainModel.ListUserRegistration = List_UserRegistration;
                //ViewBag.UserImageVariable = mainModel;

                return View(mainModel);
            }
         
            else
            {
                return View()
;           }





        }

        public ActionResult UserProfile(int id)
        {

            List<UserRegistrationModel> List_UserDetails = new List<UserRegistrationModel>();
            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();

            MainModel mainModel = new MainModel();
            DataTable userDetailsDataTable = new DataTable();
            DataTable LoggeduserDetailsDataTable = new DataTable();

            DataTable DataTablelastLoginDetails = new DataTable();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_listOfUsers = "SELECT * FROM users WHERE UserID = @UserID";
                MySqlDataAdapter mySqlData_UserList = new MySqlDataAdapter(qry_listOfUsers, mySqlCon);
                mySqlData_UserList.SelectCommand.Parameters.AddWithValue("@UserID",id);
                mySqlData_UserList.Fill(userDetailsDataTable);

                String qry_lastlogDetails = "SELECT usr.UserID,ldt.FirstLoggedDate,ldt.FirstLoggedTime,ldt.LastLoggedDate,ldt.LastLoggedTime FROM login_details ldt,users usr WHERE usr.UserEmail = ldt.UserEmail and UserID = @UserID";
                MySqlDataAdapter mySqlData_lastlogDetails = new MySqlDataAdapter(qry_lastlogDetails, mySqlCon);
                mySqlData_lastlogDetails.SelectCommand.Parameters.AddWithValue("@UserID", id);
                mySqlData_lastlogDetails.Fill(DataTablelastLoginDetails);

                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserMobile,UserDepartment,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa.Fill(LoggeduserDetailsDataTable);

            }

            if (LoggeduserDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = LoggeduserDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = LoggeduserDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(LoggeduserDetailsDataTable.Rows[0][2]);
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][6].ToString();

            }

            if (userDetailsDataTable.Rows.Count == 1 && DataTablelastLoginDetails.Rows.Count == 1)
            {

                mainModel.UserProfileImagePath = userDetailsDataTable.Rows[0][8].ToString();

                List_UserDetails.Add(
                    new UserRegistrationModel {
                        UserFullName = userDetailsDataTable.Rows[0][1].ToString(),
                        UserType = userDetailsDataTable.Rows[0][7].ToString(),
                        UserID = Convert.ToInt32(userDetailsDataTable.Rows[0][0]),
                        UserEmail = userDetailsDataTable.Rows[0][2].ToString(),
                        UserMobile = userDetailsDataTable.Rows[0][3].ToString(),
                        UserDepartment = userDetailsDataTable.Rows[0][6].ToString(),
                        FirstLoggedDate = DataTablelastLoginDetails.Rows[0][1].ToString(),
                        FirstLoggedTime = DataTablelastLoginDetails.Rows[0][2].ToString(),
                        LastLoggedDate = DataTablelastLoginDetails.Rows[0][3].ToString(),
                        LastLoggedTime = DataTablelastLoginDetails.Rows[0][4].ToString()


                    }    
                );

          


                mainModel.ListUserRegistration = List_UserDetails;
            }

                return View(mainModel);
        }


        public ActionResult UpdateUserProfile(UserRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();

            string UserImageFile = Path.GetFileNameWithoutExtension(userRegistrationModel.UserImage.FileName);
            string extension2 = Path.GetExtension(userRegistrationModel.UserImage.FileName);
            UserImageFile = UserImageFile + DateTime.Now.ToString("yymmssfff") + extension2;
            userRegistrationModel.UserImagePath = "~/userimages/" + UserImageFile;
            UserImageFile = Path.Combine(Server.MapPath("~/userimages/"), UserImageFile);
            userRegistrationModel.UserImage.SaveAs(UserImageFile);




            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();



                String qry_update_userDetails = "UPDATE users SET UserName = @UserName,UserMobile = @UserMobile,UserImage = @userImgPath WHERE UserID = @UserID";

                MySqlCommand mySqlCommand_update_userDetails = new MySqlCommand(qry_update_userDetails, mySqlCon);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserName", userRegistrationModel.UserFullName);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserMobile", userRegistrationModel.UserMobile);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@userImgPath", userRegistrationModel.UserImagePath);
                mySqlCommand_update_userDetails.ExecuteNonQuery();


            }

            return RedirectToAction("Edit", "UserManagement", new { id = userRegistrationModel.UserID });
        }




        public ActionResult UpdateMyProfile(UserRegistrationModel userRegistrationModel)
        {
            DB dbConn = new DB();



            string UserImageFile = Path.GetFileNameWithoutExtension(userRegistrationModel.UserImage.FileName);
            string extension2 = Path.GetExtension(userRegistrationModel.UserImage.FileName);
            UserImageFile = UserImageFile + DateTime.Now.ToString("yymmssfff") + extension2;
            userRegistrationModel.UserImagePath = "~/userimages/" + UserImageFile;
            UserImageFile = Path.Combine(Server.MapPath("~/userimages/"), UserImageFile);
            userRegistrationModel.UserImage.SaveAs(UserImageFile);

            String userImgPath = userRegistrationModel.UserImagePath;




            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();


                String qry_update_userDetails = "UPDATE users SET UserName = @UserName,UserMobile = @UserMobile WHERE UserID = @UserID";

                MySqlCommand mySqlCommand_update_userDetails = new MySqlCommand(qry_update_userDetails, mySqlCon);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserName", userRegistrationModel.UserFullName);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@UserMobile", userRegistrationModel.UserMobile);
                mySqlCommand_update_userDetails.Parameters.AddWithValue("@userImgPath", userRegistrationModel.UserImagePath);
                mySqlCommand_update_userDetails.ExecuteNonQuery();


            }

            return RedirectToAction("Edit", "UserManagement", new { id = userRegistrationModel.UserID });
        }

        public ActionResult EmployeesInfoReportPreview()
        {

            DataTable userDetailsDataTable = new DataTable();
            DataTable userListDataTable = new DataTable();
            MainModel mainModel = new MainModel();

            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_listOfUsers = "SELECT * FROM users";
                MySqlDataAdapter mySqlData_UserList = new MySqlDataAdapter(qry_listOfUsers, mySqlCon);
                mySqlData_UserList.Fill(userListDataTable);

            }


            for (int i = 0; i < userListDataTable.Rows.Count; i++)
            {

                mainModel.CurrentDate ="June 2018"/* DateTime.Now.ToString("dd-MM-yyyy")*/;
                mainModel.CurrentTime = "05:45PM"/* DateTime.Now.ToString("HH:mm:ss")*/;

                List_UserRegistration.Add(new UserRegistrationModel
                {

           
                    UserFullName = userListDataTable.Rows[i][1].ToString(),
                    UserType = userListDataTable.Rows[i][7].ToString(),
                    UserID = Convert.ToInt32(userListDataTable.Rows[i][0]),
                    UserEmail = userListDataTable.Rows[i][2].ToString(),
                    UserMobile = userListDataTable.Rows[i][3].ToString(),
                    UserDepartment = userListDataTable.Rows[i][6].ToString()

                });
            }
                mainModel.ListUserRegistration = List_UserRegistration;

            return View(mainModel);
        }

        public ActionResult DepartmentLeadersInfoReportPreview()
        {

            DataTable userDetailsDataTable = new DataTable();
            DataTable userListDataTable = new DataTable();
            MainModel mainModel = new MainModel();

            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_listOfUsers = "SELECT * FROM users";
                MySqlDataAdapter mySqlData_UserList = new MySqlDataAdapter(qry_listOfUsers, mySqlCon);
                mySqlData_UserList.Fill(userListDataTable);

            }


            for (int i = 0; i < userListDataTable.Rows.Count; i++)
            {

                mainModel.CurrentDate = "June 2018"/* DateTime.Now.ToString("dd-MM-yyyy")*/;
                mainModel.CurrentTime = "05:45PM"/* DateTime.Now.ToString("HH:mm:ss")*/;

                List_UserRegistration.Add(new UserRegistrationModel
                {


                    UserFullName = userListDataTable.Rows[i][1].ToString(),
                    UserType = userListDataTable.Rows[i][7].ToString(),
                    UserID = Convert.ToInt32(userListDataTable.Rows[i][0]),
                    UserEmail = userListDataTable.Rows[i][2].ToString(),
                    UserMobile = userListDataTable.Rows[i][3].ToString(),
                    UserDepartment = userListDataTable.Rows[i][6].ToString()

                });
            }
            mainModel.ListUserRegistration = List_UserRegistration;

            return View(mainModel);
        }


        public ActionResult UpdateUserDepartment(UserRegistrationModel userRegistrationModel)
        {

            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();
            MainModel mainModel = new MainModel();



            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_update_userDepartment = "UPDATE users SET UserDepartment = @UserDepartment WHERE UserID = @UserID";

                MySqlCommand mySqlCommand_update_userDepartment = new MySqlCommand(qry_update_userDepartment, mySqlCon);
                mySqlCommand_update_userDepartment.Parameters.AddWithValue("@UserDepartment", userRegistrationModel.UserDepartment);
                mySqlCommand_update_userDepartment.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);

                mySqlCommand_update_userDepartment.ExecuteNonQuery();


            }

            return RedirectToAction("Index", "UserManagement");


        }

        public ActionResult LockUser(UserRegistrationModel userRegistrationModel)
        {
            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();
            MainModel mainModel = new MainModel();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_update_user_lock = "UPDATE users SET Validation = 'false' WHERE UserID = @UserID";
                MySqlCommand mySqlCommand_update_user_lock = new MySqlCommand(qry_update_user_lock, mySqlCon);
                mySqlCommand_update_user_lock.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);
                mySqlCommand_update_user_lock.ExecuteNonQuery();


            }
            return RedirectToAction("Index", "UserManagement");
        }


        public ActionResult UnLockUser(UserRegistrationModel userRegistrationModel)
        {
            List<UserRegistrationModel> List_UserRegistration = new List<UserRegistrationModel>();
            MainModel mainModel = new MainModel();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_update_user_unlock = "UPDATE users SET Validation = 'true' WHERE UserID = @UserID";
                MySqlCommand mySqlCommand_update_user_unlock = new MySqlCommand(qry_update_user_unlock, mySqlCon);
                mySqlCommand_update_user_unlock.Parameters.AddWithValue("@UserID", userRegistrationModel.UserID);
                mySqlCommand_update_user_unlock.ExecuteNonQuery();


            }
            return RedirectToAction("Index", "UserManagement");
        }






    }
}