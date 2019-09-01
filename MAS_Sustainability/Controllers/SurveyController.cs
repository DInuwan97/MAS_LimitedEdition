using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using MAS_Sustainability.Models;

namespace MAS_Sustainability.Controllers
{
    public class SurveyController : Controller
    {
        public ActionResult Index()
        {
            DataTable SurveyDataTable = new DataTable();
            DataTable UserDataDatatable = new DataTable();
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String UserDetails = "SELECT UserName,UserType,userid,userimage FROM users WHERE UserEmail = '" + Session["user"] + "'";


                String listOfSurveys = "select tokens.TokenID,ProblemName,AddedDate,comment.comment " +
                    "from comment,feedback,tokens, token_audit " +
                    "where tokens.tokenAuditID = token_audit.tokenauditId AND " +
                    "comment.feedbackId = feedback.feedbackId " +
                    "AND tokens.TokenID = feedback.tokenId AND " +
                    "AddedUser = '" + Session["user"] + "'";


                MySqlDataAdapter mySqlDa1 = new MySqlDataAdapter();

                MySqlCommand UserDetailsComm = new MySqlCommand(UserDetails, mySqlCon);
                MySqlCommand listOfSurveysComm = new MySqlCommand(listOfSurveys, mySqlCon);


                mySqlDa1.SelectCommand = UserDetailsComm;
                mySqlDa1.Fill(UserDataDatatable);

                mySqlDa1.SelectCommand = listOfSurveysComm;
                mySqlDa1.Fill(SurveyDataTable);
            }

            var surveyList = new List<Survey>();
            Boolean temp;

            for (int i = 0; i < SurveyDataTable.Rows.Count; i++)
            {

                if (String.IsNullOrWhiteSpace(SurveyDataTable.Rows[i][3].ToString()))
                {
                    temp = false;
                }
                else
                {
                    temp = true;
                }

                surveyList.Add(new Survey
                {
                    tokenID = Convert.ToInt32(SurveyDataTable.Rows[i][0]),
                    ProblemName = SurveyDataTable.Rows[i][1].ToString(),
                    AddedDate = SurveyDataTable.Rows[i][2].ToString(),
                    commented = temp
                });

            }

            MainModel mainModel = new MainModel();

            mainModel.SurveyList = surveyList;

            if (UserDataDatatable.Rows.Count == 1)
            {

                mainModel.LoggedUserName = UserDataDatatable.Rows[0][0].ToString();
                mainModel.LoggedUserType = UserDataDatatable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(UserDataDatatable.Rows[0][2]);
                mainModel.UserImagePath = UserDataDatatable.Rows[0][3].ToString();
            }

            return View(mainModel);
        }



        // GET: Survey/viewSurvey/?
        public ActionResult viewSurvey(int? Id)
        {

            if (!Id.HasValue)
            {
                return RedirectToAction("Index", "Survey");
            }


            DataTable UserDataDatatable = new DataTable();
            DataTable SurveyDataTable = new DataTable();

            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String listOfsurvey = "select tokens.TokenID,ProblemName,Location,Description,AddedUser,AddedDate,Category,TokenImageID,ImagePath,rating,comment " +
                                     "from comment,feedback,tokens,token_audit,users,token_image " +
                                     "where users.UserID=feedback.userId AND " +
                                     "tokens.TokenID=feedback.tokenId AND " +
                                     "tokens.tokenAuditID = token_audit.tokenauditId and " +
                                     "tokens.TokenID = '" + (int)Id + "' and " +
                                     "token_image.tokenid=tokens.tokenauditid AND " +
                                     "comment.feedbackId=feedback.feedbackId " +
                                     "AND AddedUser='" + Session["user"] + "'";

                String UserDetails = "SELECT UserName,UserType,userid,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa2 = new MySqlDataAdapter();
                MySqlCommand UserDetailsComm = new MySqlCommand(UserDetails, mySqlCon);
                MySqlCommand listOfReportsComm = new MySqlCommand(listOfsurvey, mySqlCon);
                mySqlDa2.SelectCommand = UserDetailsComm;
                mySqlDa2.Fill(UserDataDatatable);
                mySqlDa2.SelectCommand = listOfReportsComm;
                mySqlDa2.Fill(SurveyDataTable);
            }

            if (SurveyDataTable.Rows.Count == 0)
            {
                return RedirectToAction("Index", "Survey");
            }
            else
            {
                Survey s = new Survey
                {
                    tokenID = Convert.ToInt32(SurveyDataTable.Rows[0][0]),
                    ProblemName = SurveyDataTable.Rows[0][1].ToString(),
                    Location = SurveyDataTable.Rows[0][2].ToString(),
                    Description = SurveyDataTable.Rows[0][3].ToString(),
                    AddedDate = SurveyDataTable.Rows[0][5].ToString(),
                    Category = SurveyDataTable.Rows[0][6].ToString(),
                    Image1path = SurveyDataTable.Rows[0][8].ToString(),
                    Image2path = SurveyDataTable.Rows[1][8].ToString(),
                    rating = Convert.ToInt32(SurveyDataTable.Rows[1][9]),
                    comment = SurveyDataTable.Rows[1][10].ToString(),

                };

                MainModel main = new MainModel();

                main.survey = s;
                if (UserDataDatatable.Rows.Count == 1)
                {

                    main.LoggedUserName = UserDataDatatable.Rows[0][0].ToString();
                    main.LoggedUserType = UserDataDatatable.Rows[0][1].ToString();
                    main.LoggedUserID = Convert.ToInt32(UserDataDatatable.Rows[0][2]);
                    main.UserImagePath = UserDataDatatable.Rows[0][3].ToString();
                }

                return View(main);

            }
        }

        [HttpPost]
        public ActionResult SubmitSurvey(int? rating, string comment, int? tokenID)
        {
            DB dbconnection = new DB();

            using (MySqlConnection mySqlCon = dbconnection.DBConnection())
            {
                mySqlCon.Open();

                String updateQuery = "UPDATE feedback set rating = '" + rating + "' where tokenId = '" + tokenID + "'";
                MySqlCommand mySqlComm = new MySqlCommand(updateQuery, mySqlCon);
                mySqlComm.ExecuteNonQuery();

                /*
                String Like = "Update feedback set rating = '1' where userid='" +  + "' and tokenid = '" + + "'";
                MySqlCommand mySqlComm = new MySqlCommand(, mySqlCon);
                int rowAffected = mySqlComm.ExecuteNonQuery();

                    String LikeUpdate = "Insert into feedback(userid,tokenid,rating) values(" + + "," + + ",1)";
                    MySqlCommand mySqlComm2 = new MySqlCommand(LikeUpdate, mySqlCon);
                    mySqlComm2.ExecuteNonQuery();
                    */
                return RedirectToAction("Index", "Survey");
            }
        }
    }
}