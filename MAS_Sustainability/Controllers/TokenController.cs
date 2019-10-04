using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using MAS_Sustainability.Models;
using System.Net;

namespace MAS_Sustainability.Controllers
{
    public class TokenController : Controller
    {
        

        [HttpGet]
        // GET: Token
        public ActionResult Index()
        {
            MainModel finalItem = new MainModel();
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }

            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable userDetailsDataTable = new DataTable();
            DataTable ForwardedTokeDataTable = new DataTable();

            DataTable ManagerStatus_pending_DataTable = new DataTable();


            MainModel mainModel = new MainModel();
            Token tokenModel = new Token();

            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            List<Token> TokenManagerPending_List = new List<Token>();


            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                string qry = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkFlow.TokenManagerStatus FROM users usr,tokens tk, token_audit tka,token_flow tkFlow WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = tkFlow.TokenAuditID";  
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry,mySqlCon);
                mySqlDA.Fill(dtblTokens);



                String qry_forwared_tokens = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkFlow.TokenManagerStatus,tkreview.SentUser " +
                    "FROM users usr,tokens tk, token_audit tka,token_flow tkFlow, token_review tkreview " +
                    "WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = tkFlow.TokenAuditID AND tk.TokenAuditID = tkreview.TokenAuditID and tkreview.Status != 'Accept'";

                MySqlDataAdapter mySqlDAForwardedTokens = new MySqlDataAdapter(qry_forwared_tokens,mySqlCon);
                mySqlDAForwardedTokens.Fill(ForwardedTokeDataTable);



                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);
                //DashbordController dashbord = new DashbordController();
                //finalItem.LoggedUserName = dashbord.setUserDetails().ToString();




            }

            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {
 
                List_Token.Add(new Token
                    { ProblemName = dtblTokens.Rows[i][1].ToString(),
                      Location = dtblTokens.Rows[i][2].ToString(),
                      AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][3]) ,
                      UserName = dtblTokens.Rows[i][4].ToString() ,
                      TokenStatus = dtblTokens.Rows[i][5].ToString(),
                      TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0]),
                      //SentUser = dtblTokens.Rows[i][6].ToString()
                     }                                  
                );

            }

            for(int i = 0; i < ForwardedTokeDataTable.Rows.Count; i++)
            {
                Token_List.Add(new Token
                {
                    ProblemName = ForwardedTokeDataTable.Rows[i][1].ToString(),
                    Location = ForwardedTokeDataTable.Rows[i][2].ToString(),
                    AttentionLevel = Convert.ToInt32(ForwardedTokeDataTable.Rows[i][3]),
                    UserName = ForwardedTokeDataTable.Rows[i][4].ToString(),
                    TokenStatus = ForwardedTokeDataTable.Rows[i][5].ToString(),
                    TokenAuditID = Convert.ToInt32(ForwardedTokeDataTable.Rows[i][0]),
                    SentUser = ForwardedTokeDataTable.Rows[i][6].ToString()

                }
                );
            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            if (ManagerStatus_pending_DataTable.Rows.Count == 1)
            {
                /*TokenManagerPending_List.Add(new Token
                {
                   TokenManagerStatusPending = 4/*Convert.ToInt32(ManagerStatus_pending_DataTable.Rows[0][0].ToString())*/
                //});

                //mainModel.TokenMaagerStatusPendingList = TokenManagerPending_List;

                mainModel.TokenManagerStatusPending = Convert.ToInt32(ManagerStatus_pending_DataTable.Rows[0][0].ToString());
            }
            

            mainModel.ListToken = List_Token;
            mainModel.ListUserLogin = List_UserLogin;
            mainModel.TokenList = Token_List;
            mainModel.TokenManagerStatusPending = TokenManagerPendingSattusCount();



            return View(mainModel);
        }


        public static int TokenManagerPendingSattusCount()
        {
            DataTable ManagerStatus_pending_DataTable = new DataTable();
            MainModel mainModel = new MainModel();
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                String qry_count_TokenManager_status_Pending = "SELECT COUNT(TokenFlowID) FROM token_flow WHERE TokenManagerStatus = 'Pending'";
                MySqlDataAdapter mySqlDa_count_TokenManager_status_Pending = new MySqlDataAdapter(qry_count_TokenManager_status_Pending, mySqlCon);
                mySqlDa_count_TokenManager_status_Pending.Fill(ManagerStatus_pending_DataTable);
            }

            if (ManagerStatus_pending_DataTable.Rows.Count == 1)
            {
                mainModel.TokenManagerStatusPending = Convert.ToInt32(ManagerStatus_pending_DataTable.Rows[0][0].ToString());
            }



                return mainModel.TokenManagerStatusPending;
        }





        // GET: Token/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Token/Create
        public ActionResult Add()
        {
            DB dbConn = new DB();
            DataTable userDetailsDataTable = new DataTable();
            MainModel mainModel = new MainModel();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);
            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][3].ToString();
            }


            return View(mainModel);
        }

        // POST: Token/Create
        [HttpPost]
        public ActionResult Add(Token tokenModel)
        {

        
            DataTable userDetailsDataTable = new DataTable();
            MainModel mainModel = new MainModel();



            //Image 01
            string first_name_of_file = Path.GetFileNameWithoutExtension(tokenModel.FirstImageFile.FileName);
            string extension1 = Path.GetExtension(tokenModel.FirstImageFile.FileName);
            first_name_of_file = first_name_of_file + DateTime.Now.ToString("yymmssfff") +extension1;
            tokenModel.FirstImagePath = "~/Image/" +first_name_of_file;
            first_name_of_file = Path.Combine(Server.MapPath("~/Image/"), first_name_of_file);
            tokenModel.FirstImageFile.SaveAs(first_name_of_file);

            String imgPath1 = tokenModel.FirstImagePath;



            //Image 02
            string second_name_of_file = Path.GetFileNameWithoutExtension(tokenModel.SecondImageFile.FileName);
            string extension2 = Path.GetExtension(tokenModel.SecondImageFile.FileName);
            second_name_of_file = second_name_of_file + DateTime.Now.ToString("yymmssfff") + extension2;
            tokenModel.SecondImagePath = "~/Image/" + second_name_of_file;
            second_name_of_file = Path.Combine(Server.MapPath("~/Image/"), second_name_of_file);
            tokenModel.SecondImageFile.SaveAs(second_name_of_file);

            String imgPath2 = tokenModel.SecondImagePath;




            String AddedUser = Session["user"].ToString();


            DB dbConn = new DB();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                // String qry = "INSERT INTO token_audit(AddedUser,Category,AddedDate)VALUES(@AddedUser,@Category,NOW())";



                MySqlCommand mySqlCmd_TokenAudit = new MySqlCommand("Proc_Store_TokenAudit", mySqlCon);
                mySqlCmd_TokenAudit.CommandType = CommandType.StoredProcedure;
                mySqlCmd_TokenAudit.Parameters.AddWithValue("_Category", tokenModel.ProblemCategory);
                mySqlCmd_TokenAudit.Parameters.AddWithValue("_AddedUser", AddedUser);


                mySqlCmd_TokenAudit.ExecuteNonQuery();


                String last_audit_id_qry = "SELECT TokenAuditID FROM token_audit ORDER BY TokenAuditID DESC LIMIT 1";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(last_audit_id_qry, mySqlCon);
                DataTable dt = new DataTable();
                mySqlDA.Fill(dt);

                int last_audit_id_num = Convert.ToInt32(dt.Rows[0][0]);

                string qry = "INSERT INTO tokens(TokenAuditID,ProblemName,Location,AttentionLevel,Description) VALUES(@TokenAuditID,@ProblemName,@Location,@AttentionLevel,@Description)";

                MySqlCommand mySqlCmd_tokenInfo = new MySqlCommand(qry, mySqlCon);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@TokenAuditID", last_audit_id_num);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@ProblemName", tokenModel.ProblemName);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@Location", tokenModel.Location);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@AttentionLevel", tokenModel.AttentionLevel);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@Description", tokenModel.Description);
                mySqlCmd_tokenInfo.ExecuteNonQuery();


                MySqlCommand mySqlCmd_ImageUpload = new MySqlCommand("Proc_Store_Images", mySqlCon);
                mySqlCmd_ImageUpload.CommandType = CommandType.StoredProcedure;
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_TokenAuditID", last_audit_id_num);
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_ImgPath1", imgPath1);
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_ImgPath2", imgPath2);
                mySqlCmd_ImageUpload.ExecuteNonQuery();



                String qryTokenStatus = "INSERT INTO token_flow(TokenAuditID,TokenManagerStatus,DeptLeaderStatus,FinalVerification,CompleteStatus,CompleteDate,VerifiedDate) values(@TokenAuditID,'Pending','Pending','Pending','Pending','Pending','Pending')";
                MySqlCommand mySqlCmd_tokenStatus = new MySqlCommand(qryTokenStatus,mySqlCon);
                mySqlCmd_tokenStatus.Parameters.AddWithValue("@TokenAuditID", last_audit_id_num);
                mySqlCmd_tokenStatus.ExecuteNonQuery();

              

            }
            // TODO: Add insert logic here


            return RedirectToAction("MyTokens", "Token");
        }



        // GET: Token/Edit/5
        public ActionResult View(int id)
        {
            MainModel mainModel = new MainModel();

            Token tokenModel1 = new Token();
            DataTable dtblToken = new DataTable();
            DataTable dtblSideList = new DataTable();

            DataTable dtblTokenImage = new DataTable();

            DataTable userDetailsDataTable = new DataTable();
            String AddedUser = Session["user"].ToString();

            DB dbConn = new DB();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry = "SELECT tka.TokenAuditID,tk.ProblemName,tka.AddedDate,tk.Location,tk.AttentionLevel,usr.UserName,tki.ImagePath,tk.Description FROM users usr,tokens tk, token_audit tka,token_image tki WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = tki.TokenID AND tk.TokenAuditID = @TokenAuditID ";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry, mySqlCon);
                mySqlDa.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa.Fill(dtblToken);


                String qry_side_token_list = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,tki.ImagePath FROM tokens tk, token_audit tka,token_image tki,token_flow tkf WHERE tk.TokenAuditID = tka.TokenAuditID  AND tk.TokenAuditID = tki.TokenID AND tkf.TokenAuditID = tk.TokenAuditID  AND tkf.TokenManagerStatus = 'Pending'";
                MySqlDataAdapter mySqlDa_sideList = new MySqlDataAdapter(qry_side_token_list, mySqlCon);
                mySqlDa_sideList.Fill(dtblSideList);



                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlData = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlData.Fill(userDetailsDataTable);


                mainModel.ArrTokenAuditID = new int[50];
                 mainModel.ArrProblemName = new string[250];
                 mainModel.ArrLocation = new string[100];
                 mainModel.ArrAttentionLevel = new int[200000];
                 mainModel.ArrFirstImagePath = new string[500];

                 mainModel.no_of_rows_side_bar = Convert.ToInt32(dtblSideList.Rows.Count);

            

            }

            for (int i = 0; i < dtblSideList.Rows.Count; i = i + 2)
            {

                 mainModel.ArrTokenAuditID[i] = Convert.ToInt32(dtblSideList.Rows[i][0]);
                 mainModel.ArrProblemName[i] = dtblSideList.Rows[i][1].ToString();
                 mainModel.ArrLocation[i] = dtblSideList.Rows[i][2].ToString();
                 mainModel.ArrAttentionLevel[i] = Convert.ToInt32(dtblSideList.Rows[i][3]);
                 mainModel.ArrFirstImagePath[i] = dtblSideList.Rows[i][4].ToString();

            }



            if (dtblToken.Rows.Count == 2 )
            {
                mainModel.TokenAuditID = Convert.ToInt32(dtblToken.Rows[0][0]);
                 mainModel.ProblemName = dtblToken.Rows[0][1].ToString();
                 mainModel.AddedDate = dtblToken.Rows[0][2].ToString();
                 mainModel.Location = dtblToken.Rows[0][3].ToString();
                 mainModel.AttentionLevel = Convert.ToInt32(dtblToken.Rows[0][4]);
                 mainModel.UserName = dtblToken.Rows[0][5].ToString();
                 mainModel.FirstImagePath = dtblToken.Rows[0][6].ToString();
                 mainModel.SecondImagePath = dtblToken.Rows[1][6].ToString();
                 mainModel.Description = dtblToken.Rows[0][7].ToString();

                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][3].ToString();

                ViewBag.TokenVariable = mainModel;
                return View(mainModel);

            }
            else
            {
                return View("MyTokens");
            }



        }

        // POST: Token/Edit/5
        [HttpPost]
        public ActionResult TokenForward(Token tokenModel)
        {
            DB dbConn = new DB();
            String ForwardUser = Session["user"].ToString();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                string qry = "INSERT INTO token_review(TokenAuditID,SpecialActs,RepairDepartment,SentDate,SentUser,Deadline,Status,Cost)VALUES(@TokenAuditID,@SpecialActs,@ReparationDepartment,NOW(),@SentUser,'null','null',0)";
                MySqlCommand mySqlCmd_TokenFoward = new MySqlCommand(qry, mySqlCon);
                mySqlCmd_TokenFoward.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCmd_TokenFoward.Parameters.AddWithValue("@SpecialActs", "Urgent");
                mySqlCmd_TokenFoward.Parameters.AddWithValue("@ReparationDepartment", tokenModel.ReparationDepartment);
                mySqlCmd_TokenFoward.Parameters.AddWithValue("@SentUser",ForwardUser);
                mySqlCmd_TokenFoward.ExecuteNonQuery();

                String update_token_status = "UPDATE token_flow SET TokenManagerStatus = @ReparationDepartment WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_update_token_status = new MySqlCommand(update_token_status,mySqlCon);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@ReparationDepartment", tokenModel.ReparationDepartment);
                mySqlCommand_update_token_status.ExecuteNonQuery();

               

            }
            return RedirectToAction("Index","Token");


        }

        // GET: Token/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Token/Delete/5
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



        public ActionResult Update(Token tokenModel)
        {
            DB dbConn = new DB();
            String ForwardUser = Session["user"].ToString();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                /*String qry_update_dept_token_flow = "UPDATE token_flow SET TokenManagerStatus = @ReparationDepartment WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_update_dept_token_flow = new MySqlCommand(qry_update_dept_token_flow, mySqlCon);
                mySqlCmd_update_dept_token_flow.Parameters.AddWithValue("@ReparationDepartment", tokenModel.TokenAuditID);
                mySqlCmd_update_dept_token_flow.Parameters.AddWithValue("@TokenAuditID", tokenModel.ReparationDepartment);
                mySqlCmd_update_dept_token_flow.ExecuteNonQuery();
                */
                String update_token_status_token_flow = "UPDATE token_flow SET TokenManagerStatus = @ReparationDepartment WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_update_token_status_token_flow = new MySqlCommand(update_token_status_token_flow, mySqlCon);
                mySqlCommand_update_token_status_token_flow.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCommand_update_token_status_token_flow.Parameters.AddWithValue("@ReparationDepartment", tokenModel.ReparationDepartment);
                mySqlCommand_update_token_status_token_flow.ExecuteNonQuery();



                String update_token_status_token_review = "UPDATE token_review SET RepairDepartment = @ReparationDepartment,SentDate = NOW() WHERE TokenAuditID = @TokenAuditID ";
                MySqlCommand mySqlCmd_update_token_status_token_review = new MySqlCommand(update_token_status_token_review,mySqlCon);
                mySqlCmd_update_token_status_token_review.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCmd_update_token_status_token_review.Parameters.AddWithValue("@ReparationDepartment", tokenModel.ReparationDepartment);
                mySqlCmd_update_token_status_token_review.ExecuteNonQuery();




                return RedirectToAction("Index");
            }



           
        }


        public ActionResult MyTokens()
        {

            MainModel finalItem = new MainModel();
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }

            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable dtbl_Expired_Tokens = new DataTable();

            DataTable userDetailsDataTable = new DataTable();
            DataTable ForwardedTokeDataTable = new DataTable();
            MainModel mainModel = new MainModel();

            Token tokenModel = new Token();

            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            List<Token> Token_Expired_List = new List<Token>();

            DateTime dateTime = DateTime.UtcNow.Date;

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                String qry_myTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka,mas_isscs.tokens tk,mas_isscs.token_flow tkf,mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and (tkf.DeptLeaderStatus >= CURDATE() OR DeptLeaderStatus = 'Pending')";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlDA.Fill(dtblTokens);


                String qry_UserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDataUserDetails = new MySqlDataAdapter(qry_UserDetails, mySqlCon);
                mySqlDataUserDetails.Fill(userDetailsDataTable);


                String qry_ExpiredTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka, mas_isscs.tokens tk, mas_isscs.token_flow tkf, mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and tkf.DeptLeaderStatus < CURDATE() and DeptLeaderStatus != 'Pending'";
                MySqlDataAdapter mySqlDA_Expired_Tokens = new MySqlDataAdapter(qry_ExpiredTokens, mySqlCon);
                mySqlDA_Expired_Tokens.Fill(dtbl_Expired_Tokens);





            }
            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }

            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {

                List_Token.Add(new Token
                {
                    CurrentDate = dateTime.ToString("dd/MM/yyyy"),
                    ProblemName = dtblTokens.Rows[i][4].ToString(),
                    ProblemCategory = dtblTokens.Rows[i][1].ToString(),
                    Location = dtblTokens.Rows[i][5].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][6]),
                    UserName = dtblTokens.Rows[i][2].ToString(),
                    TokenStatus = dtblTokens.Rows[i][7].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0]),
                    AddedDate = dtblTokens.Rows[i][3].ToString(),
                    DeadLine = dtblTokens.Rows[i][8].ToString(),
                    CompleteStatus = dtblTokens.Rows[i][9].ToString(),
                    CompleteDate = dtblTokens.Rows[i][10].ToString(),
                    FinalVerificationStatus = dtblTokens.Rows[i][11].ToString()

                    //SentUser = dtblTokens.Rows[i][6].ToString()
                }
                );

            }

            for (int i = 0; i < dtbl_Expired_Tokens.Rows.Count; i++)
            {
                Token_Expired_List.Add(new Token
                {
                    ProblemName_Expired = dtbl_Expired_Tokens.Rows[i][4].ToString(),
                    ProblemCategory_Expired = dtbl_Expired_Tokens.Rows[i][1].ToString(),
                    Location_Expired = dtbl_Expired_Tokens.Rows[i][5].ToString(),
                    AttentionLevel_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][6]),
                    UserName_Expired = dtbl_Expired_Tokens.Rows[i][2].ToString(),
                    TokenStatus_Expired = dtbl_Expired_Tokens.Rows[i][7].ToString(),
                    TokenAuditID_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][0]),
                    AddedDate_Expired = dtbl_Expired_Tokens.Rows[i][3].ToString(),
                    DeadLine_Expired = dtbl_Expired_Tokens.Rows[i][8].ToString(),
                    CompleteStatus_Expired = dtbl_Expired_Tokens.Rows[i][9].ToString(),
                    CompleteDate_Expired = dtbl_Expired_Tokens.Rows[i][10].ToString(),
                    FinalVerificationStatus_Expired = dtbl_Expired_Tokens.Rows[i][11].ToString()


                });
            }

            mainModel.ListToken = List_Token;
            mainModel.ListUserLogin = List_UserLogin;
            mainModel.TokenList = Token_List;
            mainModel.ExpiredTokensList = Token_Expired_List;
            return View(mainModel);
        }





        public ActionResult ExpiredTokens()
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();
            DataTable dtbl_Expired_Tokens = new DataTable();

            DataTable userDetailsDataTable = new DataTable();

            List<Token> Token_Expired_List = new List<Token>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_UserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDataUserDetails = new MySqlDataAdapter(qry_UserDetails, mySqlCon);
                mySqlDataUserDetails.Fill(userDetailsDataTable);

                String qry_ExpiredTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka, mas_isscs.tokens tk, mas_isscs.token_flow tkf, mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tkf.DeptLeaderStatus < CURDATE() and DeptLeaderStatus != 'Pending' AND tka.AddedUser = usr.UserEmail";
                MySqlDataAdapter mySqlDA_Expired_Tokens = new MySqlDataAdapter(qry_ExpiredTokens, mySqlCon);
                mySqlDA_Expired_Tokens.Fill(dtbl_Expired_Tokens);

            }

            for (int i = 0; i < dtbl_Expired_Tokens.Rows.Count; i++)
            {
                Token_Expired_List.Add(new Token
                {
                    ProblemName_Expired = dtbl_Expired_Tokens.Rows[i][4].ToString(),
                    ProblemCategory_Expired = dtbl_Expired_Tokens.Rows[i][1].ToString(),
                    Location_Expired = dtbl_Expired_Tokens.Rows[i][5].ToString(),
                    AttentionLevel_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][6]),
                    UserName_Expired = dtbl_Expired_Tokens.Rows[i][2].ToString(),
                    TokenStatus_Expired = dtbl_Expired_Tokens.Rows[i][7].ToString(),
                    TokenAuditID_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][0]),
                    AddedDate_Expired = dtbl_Expired_Tokens.Rows[i][3].ToString(),
                    DeadLine_Expired = dtbl_Expired_Tokens.Rows[i][8].ToString(),
                    CompleteStatus_Expired = dtbl_Expired_Tokens.Rows[i][9].ToString(),
                    CompleteDate_Expired = dtbl_Expired_Tokens.Rows[i][10].ToString(),
                    FinalVerificationStatus_Expired = dtbl_Expired_Tokens.Rows[i][11].ToString()


                });
            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            mainModel.ExpiredTokensList = Token_Expired_List;
        

            return View(mainModel);
        }




        public ActionResult TokenUpdate(int id)
        {
            DataTable userDetailsDataTable = new DataTable();

            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> Token_List = new List<Token>();
            List<Token> List_Token = new List<Token>();

            DataTable dtblTokens = new DataTable();
            MainModel mainModel = new MainModel();
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_UserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDataUserDetails = new MySqlDataAdapter(qry_UserDetails, mySqlCon);
                mySqlDataUserDetails.Fill(userDetailsDataTable);

                String qry_myTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkimg.ImagePath,tk.description " +
                "FROM token_audit tka,tokens tk,token_flow tkf,users usr,token_image tkimg WHERE " +
                "tka.TokenAuditID = tk.TokenAuditID AND tkf.TokenManagerStatus = 'Pending' and tka.TokenAuditID = tkf.TokenAuditID AND " +
                "tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and tka.TokenAuditID = @TokenAuditID and tkimg.TokenID = tk.TokenAuditID";

                MySqlDataAdapter mySqlData_TokenInfo = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlData_TokenInfo.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlData_TokenInfo.Fill(dtblTokens);

            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }

            if (dtblTokens.Rows.Count == 2)
            {

                mainModel.FirstImagePath = dtblTokens.Rows[0][8].ToString();
                mainModel.SecondImagePath = dtblTokens.Rows[1][8].ToString();

                List_Token.Add(new Token
                {
                    ProblemName = dtblTokens.Rows[0][4].ToString(),
                    ProblemCategory = dtblTokens.Rows[0][1].ToString(),
                    Location = dtblTokens.Rows[0][5].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[0][6]),
                    UserName = dtblTokens.Rows[0][2].ToString(),
                    TokenStatus = dtblTokens.Rows[0][7].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[0][0]),
                    AddedDate = dtblTokens.Rows[0][3].ToString(),
                    Description = dtblTokens.Rows[0][9].ToString()

                }
                );
            }

            mainModel.ListToken = List_Token;
            mainModel.TokenList = Token_List;

            return View(mainModel);
        }


        public ActionResult DoUpdateProcess(Token tokenModel)
        {
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();

                String update_token_details = "UPDATE tokens tk,token_audit tka SET tk.ProblemName = @ProblemName,tk.Location = @Location,tka.Category = @ProblemCategory  WHERE tka.TokenAuditID = tk.TokenAuditID and tka.TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCommand_update_token_status = new MySqlCommand(update_token_details, mySqlCon);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@Description",tokenModel.Description);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@TokenAuditID",tokenModel.TokenAuditID);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@ProblemName", tokenModel.ProblemName);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@Location", tokenModel.Location);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@ProblemCategory", tokenModel.ProblemCategory);

      


                mySqlCommand_update_token_status.ExecuteNonQuery();

            }


            return RedirectToAction("MyTokens");
        }

        public ActionResult DoUpdateProcessInDetail(Token tokenModel)
        {
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
               

                String update_token_details = "UPDATE tokens tk,token_audit tka SET tk.AttentionLevel = @AttentionLevel, tk.Description = @Description,tk.ProblemName = @ProblemName,tk.Location = @Location,tka.Category = @ProblemCategory  WHERE tka.TokenAuditID = tk.TokenAuditID and tka.TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_update_token_status = new MySqlCommand(update_token_details, mySqlCon);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@ProblemName", tokenModel.ProblemName);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@Location", tokenModel.Location);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@ProblemCategory", tokenModel.ProblemCategory);

                mySqlCommand_update_token_status.Parameters.AddWithValue("@AttentionLevel", tokenModel.AttentionLevel);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@Description", tokenModel.Description);


                mySqlCommand_update_token_status.ExecuteNonQuery();

            }


            return RedirectToAction("MyTokens");
        }


        //token Repairation//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult RepairationList()
        {
            MainModel mainModel = new MainModel();
            DB dbConn = new DB();
            Token tokenModel = new Token();

            List<UserLogin> List_UserLogin = new List<UserLogin>();

            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();
            List<Token> Token_Expired_List = new List<Token>();

            DataTable dtblRepair = new DataTable();
            DataTable dtblRepair_Expired_Deadlines = new DataTable();
            DataTable userDetailsDataTable = new DataTable();
            

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                String qry = "SELECT tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkr.SentDate,tkr.Status,tka.TokenAuditID,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus FROM token_audit tka,tokens tk,token_flow tkf,users usr,token_review tkr WHERE tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = usr.UserEmail and tk.TokenAuditID = tkf.TokenAuditID and tkr.TokenAuditID = tka.TokenAuditID  AND (tkf.DeptLeaderStatus >= CURDATE() OR tkf.DeptLeaderStatus = 'Pending')";
                MySqlDataAdapter mySqlDataRepair = new MySqlDataAdapter(qry, mySqlCon);
                mySqlDataRepair.Fill(dtblRepair);

                String qry_listOfUserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage,UserDepartment FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfUserDetails, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);


                String qry_Expired_Deadlines = "SELECT tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkr.SentDate,tkr.Status,tka.TokenAuditID,tkf.TokenManagerStatus,tkf.DeptLeaderStatus FROM token_audit tka, tokens tk,token_flow tkf, users usr,token_review tkr WHERE tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = usr.UserEmail and tk.TokenAuditID = tkf.TokenAuditID and tkr.TokenAuditID = tka.TokenAuditID AND tkf.DeptLeaderStatus < CURDATE() and tkf.DeptLeaderStatus != 'Pending'";
                MySqlDataAdapter mySqlDataAdapter_Expired_Deadlines = new MySqlDataAdapter(qry_Expired_Deadlines,mySqlCon);
                mySqlDataAdapter_Expired_Deadlines.Fill(dtblRepair_Expired_Deadlines);


            }


            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
                mainModel.LoggedUserDepartment = userDetailsDataTable.Rows[0][5].ToString();

                List_UserLogin.Add(new UserLogin
                {
                    LoggedUserType = userDetailsDataTable.Rows[0][1].ToString()
                }
                );

            }

            for (int i = 0; i < dtblRepair.Rows.Count; i++)
            {
                if (mainModel.LoggedUserDepartment == dtblRepair.Rows[i][7].ToString()) {

                    if (mainModel.LoggedUserType == "Department Leader") {

                        List_Token.Add(new Token
                        {
                            ProblemName = dtblRepair.Rows[i][0].ToString(),
                            Location = dtblRepair.Rows[i][1].ToString(),
                            AttentionLevel = Convert.ToInt32(dtblRepair.Rows[i][2]),
                            AddedUserName = dtblRepair.Rows[i][3].ToString(),
                            SentDate = dtblRepair.Rows[i][4].ToString(),
                            RecievedStatus = dtblRepair.Rows[i][5].ToString(),
                            TokenAuditID = Convert.ToInt32(dtblRepair.Rows[i][6]),
                            TokenManagerStatus = dtblRepair.Rows[i][7].ToString(),
                            DeadLine = dtblRepair.Rows[i][8].ToString(),
                            CompleteStatus = dtblRepair.Rows[i][9].ToString()

                        }
                        );


                    }//user type
                }//user dept

                if (mainModel.LoggedUserType == "Administrator")
                {
                    List_Token.Add(new Token
                    {
                        ProblemName = dtblRepair.Rows[i][0].ToString(),
                        Location = dtblRepair.Rows[i][1].ToString(),
                        AttentionLevel = Convert.ToInt32(dtblRepair.Rows[i][2]),
                        AddedUserName = dtblRepair.Rows[i][3].ToString(),
                        SentDate = dtblRepair.Rows[i][4].ToString(),
                        RecievedStatus = dtblRepair.Rows[i][5].ToString(),
                        TokenAuditID = Convert.ToInt32(dtblRepair.Rows[i][6]),
                        TokenManagerStatus = dtblRepair.Rows[i][7].ToString(),
                        DeadLine = dtblRepair.Rows[i][8].ToString(),
                        CompleteStatus = dtblRepair.Rows[i][9].ToString()

                    }
                    );

                }

            }




            //for expired list

            for (int i = 0; i < dtblRepair_Expired_Deadlines.Rows.Count; i++)
            {
                if (mainModel.LoggedUserDepartment == dtblRepair_Expired_Deadlines.Rows[i][7].ToString())
                {

                    if (mainModel.LoggedUserType == "Department Leader")
                    {

                        Token_Expired_List.Add(new Token
                        {
                            ProblemName_Expired = dtblRepair_Expired_Deadlines.Rows[i][0].ToString(),
                            Location_Expired = dtblRepair_Expired_Deadlines.Rows[i][1].ToString(),
                            AttentionLevel_Expired = Convert.ToInt32(dtblRepair_Expired_Deadlines.Rows[i][2]),
                            AddedUserName_Expired = dtblRepair_Expired_Deadlines.Rows[i][3].ToString(),
                            SentDate_Expired = dtblRepair_Expired_Deadlines.Rows[i][4].ToString(),
                            RecievedStatus_Expired = dtblRepair_Expired_Deadlines.Rows[i][5].ToString(),
                            TokenAuditID_Expired = Convert.ToInt32(dtblRepair_Expired_Deadlines.Rows[i][6]),
                            TokenManagerStatus_Expired = dtblRepair_Expired_Deadlines.Rows[i][7].ToString(),
                            DeadLine_Expired = dtblRepair_Expired_Deadlines.Rows[i][8].ToString()


                        });

                    }//user type
                }//user dept

                if (mainModel.LoggedUserType == "Administrator")
                {
                    
                    Token_Expired_List.Add(new Token
                    {
                        ProblemName_Expired = dtblRepair_Expired_Deadlines.Rows[i][0].ToString(),
                        Location_Expired = dtblRepair_Expired_Deadlines.Rows[i][1].ToString(),
                        AttentionLevel_Expired = Convert.ToInt32(dtblRepair_Expired_Deadlines.Rows[i][2]),
                        AddedUserName_Expired = dtblRepair_Expired_Deadlines.Rows[i][3].ToString(),
                        SentDate_Expired = dtblRepair_Expired_Deadlines.Rows[i][4].ToString(),
                        RecievedStatus_Expired = dtblRepair_Expired_Deadlines.Rows[i][5].ToString(),
                        TokenAuditID_Expired = Convert.ToInt32(dtblRepair_Expired_Deadlines.Rows[i][6]),
                        TokenManagerStatus_Expired = dtblRepair_Expired_Deadlines.Rows[i][7].ToString(),
                        DeadLine_Expired = dtblRepair_Expired_Deadlines.Rows[i][8].ToString()


                    });
                }

            }


            mainModel.ListToken = List_Token;
            mainModel.ListUserLogin = List_UserLogin;
            mainModel.TokenList = Token_List;
            mainModel.ExpiredTokensList = Token_Expired_List;
            ViewBag.LoggedUserVariable = mainModel;

            return View(mainModel);
        }


        public MainModel checkExpiredRepair()
        {
            MainModel mainModel = new MainModel();
            DB dbConn = new DB();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

            }


                return null;
        }





        //////////////////////////////////////////////Do not TOOUCH////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public ActionResult ViewRepairation(int id)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();

            DataTable userDetailsDataTable = new DataTable();
            DataTable singleTokenDetailsDataTable_Expired = new DataTable();

            DataTable singleTokenDetailsDataTable = new DataTable();
            DataTable multiTokenDetailsDataTable = new DataTable();

            List<UserLogin> List_UserLogin = new List<UserLogin>();

            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();



            DataTable dtblRepair = new DataTable();

            List<ReparationModel> TokenRepair_List = new List<ReparationModel>();
            List<ReparationModel> SideBarTokenRepair_List = new List<ReparationModel>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                String qry_listOfUserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage,UserDepartment,UserMobile FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfUserDetails, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);


                String qry_SingleTokenDetails = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tk.Description,tkimg.ImagePath,tkr.RepairDepartment,tkr.SentDate,tkr.SentUser,usr.UserImage,usr.UserID,tkr.Status,tkr.Deadline,tkr.Cost FROM token_audit tka,tokens tk,token_image tkimg,token_review tkr,users usr WHERE tka.TokenAuditID = tk.TokenAuditID AND tk.TokenAuditID = tkimg.TokenID AND tkimg.TokenID = tkr.TokenAuditID AND usr.UserEmail = tka.AddedUser AND tka.TokenAuditID = @TokenAuditID";
                MySqlDataAdapter mySqlDa_SingleTokenDetails = new MySqlDataAdapter(qry_SingleTokenDetails,mySqlCon);
                mySqlDa_SingleTokenDetails.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa_SingleTokenDetails.Fill(singleTokenDetailsDataTable);//for admin



                String qry_SingleTokenDetails_Expired = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tk.Description,tkimg.ImagePath,tkr.RepairDepartment,tkr.SentDate,tkr.SentUser,usr.UserImage,usr.UserID,tkr.Status,tkr.Deadline,tkr.Cost FROM token_audit tka,tokens tk,token_image tkimg,token_review tkr,users usr WHERE tka.TokenAuditID = tk.TokenAuditID AND tk.TokenAuditID = tkimg.TokenID AND tkimg.TokenID = tkr.TokenAuditID AND usr.UserEmail = tka.AddedUser AND tka.TokenAuditID = @TokenAuditID and (tkr.Deadline >= CURDATE() OR tkr.Deadline = 'null')";
                MySqlDataAdapter mySqlDa_SingleTokenDetails_Expired = new MySqlDataAdapter(qry_SingleTokenDetails_Expired, mySqlCon);
                mySqlDa_SingleTokenDetails_Expired.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa_SingleTokenDetails_Expired.Fill(singleTokenDetailsDataTable_Expired);//for Dept Leader



                String qry_MultiTokenDetails = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tk.Description,tkimg.ImagePath,tkr.RepairDepartment,tkr.SentDate,tkr.SentUser,usr.UserImage,usr.UserID FROM token_audit tka,tokens tk,token_image tkimg,token_review tkr,users usr WHERE tka.TokenAuditID = tk.TokenAuditID AND tk.TokenAuditID = tkimg.TokenID AND tkimg.TokenID = tkr.TokenAuditID AND usr.UserEmail = tka.AddedUser and tkr.Status != 'Accept' ORDER BY tka.AddedDate DESC limit 10";
                MySqlDataAdapter mySqlDa_MultiTokenDetails = new MySqlDataAdapter(qry_MultiTokenDetails, mySqlCon);
                mySqlDa_MultiTokenDetails.Fill(multiTokenDetailsDataTable);



                String qry = "SELECT tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkr.SentDate,tkr.Status,tka.TokenAuditID,tkf.TokenManagerStatus,tkimg.ImagePath FROM token_audit tka,tokens tk,token_flow tkf,users usr,token_review tkr,token_image tkimg WHERE tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = usr.UserEmail and tk.TokenAuditID = tkf.TokenAuditID and tkr.TokenAuditID = tka.TokenAuditID and tka.TokenAuditID = tkimg.TokenID and tk.TokenAuditID = tkimg.TokenID and tkr.Status != 'Accept'";
                MySqlDataAdapter mySqlDataRepair = new MySqlDataAdapter(qry, mySqlCon);
                mySqlDataRepair.Fill(dtblRepair);




            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
                mainModel.LoggedUserDepartment = userDetailsDataTable.Rows[0][5].ToString();
                mainModel.LoggedUserMobile = userDetailsDataTable.Rows[0][6].ToString();

                List_UserLogin.Add(new UserLogin
                {
                    LoggedUserType = userDetailsDataTable.Rows[0][1].ToString()
                }
                );

            }


            if (mainModel.LoggedUserType == "Administrator") {

                if (singleTokenDetailsDataTable.Rows.Count == 2)
                {
                    TokenRepair_List.Add(new ReparationModel
                    {
                        TokenAuditID = Convert.ToInt32(singleTokenDetailsDataTable.Rows[0][0].ToString()),
                        ProblemCategory = singleTokenDetailsDataTable.Rows[0][1].ToString(),
                        ProblemAddedUser = singleTokenDetailsDataTable.Rows[0][2].ToString(),
                        ProblemAddedDate = singleTokenDetailsDataTable.Rows[0][3].ToString(),
                        ProblemName = singleTokenDetailsDataTable.Rows[0][4].ToString(),
                        ProblemLocation = singleTokenDetailsDataTable.Rows[0][5].ToString(),
                        AttentionLevel = Convert.ToInt32(singleTokenDetailsDataTable.Rows[0][6].ToString()),
                        ProblemDescription = singleTokenDetailsDataTable.Rows[0][7].ToString(),
                        ProblemFirstImagePath = singleTokenDetailsDataTable.Rows[0][8].ToString(),
                        ProblemSecondImagePath = singleTokenDetailsDataTable.Rows[1][8].ToString(),
                        ReparationDepartment = singleTokenDetailsDataTable.Rows[0][9].ToString(),
                        ProblemReviewedDate = singleTokenDetailsDataTable.Rows[0][10].ToString(),
                        SentUserEmail = singleTokenDetailsDataTable.Rows[0][11].ToString(),
                        ProblemAddedUserImagePath = singleTokenDetailsDataTable.Rows[0][12].ToString(),
                        UserID = Convert.ToInt32(singleTokenDetailsDataTable.Rows[0][13].ToString()),
                        ReparationAcceptStatus = singleTokenDetailsDataTable.Rows[0][14].ToString(),
                        ReparationDeadline = singleTokenDetailsDataTable.Rows[0][15].ToString(),
                        ReparationCost = Convert.ToDouble(singleTokenDetailsDataTable.Rows[0][16].ToString())

                    }
                    );
                }

            }


            if (mainModel.LoggedUserType == "Department Leader")
            {

                if (singleTokenDetailsDataTable_Expired.Rows.Count == 2)
                {
                    TokenRepair_List.Add(new ReparationModel
                    {
                        TokenAuditID = Convert.ToInt32(singleTokenDetailsDataTable_Expired.Rows[0][0].ToString()),
                        ProblemCategory = singleTokenDetailsDataTable_Expired.Rows[0][1].ToString(),
                        ProblemAddedUser = singleTokenDetailsDataTable_Expired.Rows[0][2].ToString(),
                        ProblemAddedDate = singleTokenDetailsDataTable_Expired.Rows[0][3].ToString(),
                        ProblemName = singleTokenDetailsDataTable_Expired.Rows[0][4].ToString(),
                        ProblemLocation = singleTokenDetailsDataTable_Expired.Rows[0][5].ToString(),
                        AttentionLevel = Convert.ToInt32(singleTokenDetailsDataTable_Expired.Rows[0][6].ToString()),
                        ProblemDescription = singleTokenDetailsDataTable_Expired.Rows[0][7].ToString(),
                        ProblemFirstImagePath = singleTokenDetailsDataTable_Expired.Rows[0][8].ToString(),
                        ProblemSecondImagePath = singleTokenDetailsDataTable_Expired.Rows[1][8].ToString(),
                        ReparationDepartment = singleTokenDetailsDataTable_Expired.Rows[0][9].ToString(),
                        ProblemReviewedDate = singleTokenDetailsDataTable_Expired.Rows[0][10].ToString(),
                        SentUserEmail = singleTokenDetailsDataTable_Expired.Rows[0][11].ToString(),
                        ProblemAddedUserImagePath = singleTokenDetailsDataTable_Expired.Rows[0][12].ToString(),
                        UserID = Convert.ToInt32(singleTokenDetailsDataTable_Expired.Rows[0][13].ToString()),
                        ReparationAcceptStatus = singleTokenDetailsDataTable_Expired.Rows[0][14].ToString(),
                        ReparationDeadline = singleTokenDetailsDataTable_Expired.Rows[0][15].ToString(),
                        ReparationCost = Convert.ToDouble(singleTokenDetailsDataTable_Expired.Rows[0][16].ToString())

                    }
                    );
                }

            }






            for (int i = 0; i < multiTokenDetailsDataTable.Rows.Count; i = i + 2)
            {
                if (Convert.ToInt32(multiTokenDetailsDataTable.Rows[i][0].ToString()) != id) {

                    SideBarTokenRepair_List.Add(new ReparationModel
                    {
                        SideBarTokenAuditID = Convert.ToInt32(multiTokenDetailsDataTable.Rows[i][0].ToString()),
                        SideBarProblemCategory = multiTokenDetailsDataTable.Rows[i][1].ToString(),
                        // ProblemAddedUser = singleTokenDetailsDataTable.Rows[i][2].ToString(),
                        SideBarProblemAddedDate = multiTokenDetailsDataTable.Rows[i][3].ToString(),
                        SideBarProblemName = multiTokenDetailsDataTable.Rows[i][4].ToString(),
                        SideBarProblemLocation = multiTokenDetailsDataTable.Rows[i][5].ToString(),
                        SideBarAttentionLevel = Convert.ToInt32(multiTokenDetailsDataTable.Rows[i][6].ToString()),
                        // ProblemDescription = singleTokenDetailsDataTable.Rows[i][7].ToString(),
                        SideBarProblemFirstImagePath = multiTokenDetailsDataTable.Rows[i][8].ToString(),
                        // ProblemSecondImagePath = singleTokenDetailsDataTable.Rows[1][8].ToString(),
                        SideBarReparationDepartment = multiTokenDetailsDataTable.Rows[i][9].ToString(),
                        //ProblemReviewedDate = singleTokenDetailsDataTable.Rows[i][10].ToString(),
                        // SentUserEmail = singleTokenDetailsDataTable.Rows[i][11].ToString(),
                        // ProblemAddedUserImagePath = singleTokenDetailsDataTable.Rows[i][12].ToString(),
                        //UserID = Convert.ToInt32(singleTokenDetailsDataTable.Rows[i][13].ToString())
                    }
                    );
                }
                
            }



            for (int i = 0; i < dtblRepair.Rows.Count; i = i + 2)
            {
                if (mainModel.LoggedUserDepartment == dtblRepair.Rows[i][7].ToString())
                {

                    if (mainModel.LoggedUserType == "Department Leader" || mainModel.LoggedUserType == "Administrator")
                    {
                        if (Convert.ToInt32(dtblRepair.Rows[i][6]) != id)
                        {

                            List_Token.Add(new Token
                            {

                                ProblemName = dtblRepair.Rows[i][0].ToString(),
                                Location = dtblRepair.Rows[i][1].ToString(),
                                AttentionLevel = Convert.ToInt32(dtblRepair.Rows[i][2]),
                                AddedUserName = dtblRepair.Rows[i][3].ToString(),
                                SentDate = dtblRepair.Rows[i][4].ToString(),
                                RecievedStatus = dtblRepair.Rows[i][5].ToString(),
                                TokenAuditID = Convert.ToInt32(dtblRepair.Rows[i][6]),
                                TokenManagerStatus = dtblRepair.Rows[i][7].ToString(),
                                HoriontalFirstImagePath = dtblRepair.Rows[i][8].ToString()

                            }
                            );
                        }
                    }//user type
                }//user dept
            }

            mainModel.ListUserLogin = List_UserLogin;
            mainModel.SingleTokenReparatiDetailsList = TokenRepair_List;
            mainModel.SideBarTokenReparationDetails = SideBarTokenRepair_List;
            mainModel.ListToken = List_Token;
            return View(mainModel);


        }//end ViewRepairation





        public ActionResult UpdateTokenReparation(ReparationModel reparationModel)
        {
            DB dbConn = new DB();
            DataTable repairationAuditDataTable = new DataTable();
            UserRegistrationModel userRegistrationModel = new UserRegistrationModel();
            MainModel mainModel = new MainModel();

            List<ReparationModel> TokenRepairationAuditID_List = new List<ReparationModel>();
            //  ReparationModel reparationModel = new ReparationModel();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();


                String qry_token_raparation_update = "UPDATE token_flow tkf,token_review tkr SET tkf.DeptLeaderStatus = @Deadline,tkr.Deadline = @Deadline,tkr.Status = @DeptLeaderStatus,tkr.Cost = @Cost WHERE tkr.TokenAuditID = @id AND tkf.TokenAuditID = @id";

                MySqlCommand mySqlCommand_update_token_Reparation = new MySqlCommand(qry_token_raparation_update,mySqlCon);

                mySqlCommand_update_token_Reparation.Parameters.AddWithValue("@Deadline", reparationModel.DeadLine);
                mySqlCommand_update_token_Reparation.Parameters.AddWithValue("@Cost", reparationModel.ReparationCost);
                mySqlCommand_update_token_Reparation.Parameters.AddWithValue("@id", reparationModel.TokenAuditID);
                mySqlCommand_update_token_Reparation.Parameters.AddWithValue("@DeptLeaderStatus", "Accept");



                String qry_get_token_repair_audit_id = "SELECT TokenRepairationID FROM repairation_audit WHERE TokenAuditID = @TokenAuditID";

                MySqlDataAdapter mySqlDa_ReparationAuditDetails = new MySqlDataAdapter(qry_get_token_repair_audit_id, mySqlCon);
                mySqlDa_ReparationAuditDetails.SelectCommand.Parameters.AddWithValue("@TokenAuditID", reparationModel.TokenAuditID);
                mySqlDa_ReparationAuditDetails.Fill(repairationAuditDataTable);

                if(repairationAuditDataTable.Rows.Count == 0)
                {
                    String qry_isert_token_repairation_audit = "INSERT INTO repairation_audit(TokenAuditID,SentUser,SentDate,SentTime)VALUES(@ReparationAuditID,'" + Session["user"] + "',CURDATE(),CURTIME())";

                    MySqlCommand mySqlCmd_Reparation_Insert_AuditDetails = new MySqlCommand(qry_isert_token_repairation_audit, mySqlCon);
                    mySqlCmd_Reparation_Insert_AuditDetails.Parameters.AddWithValue("@ReparationAuditID", reparationModel.TokenAuditID);
                    mySqlCmd_Reparation_Insert_AuditDetails.ExecuteNonQuery();

                }
                else
                {
                   
                    TokenRepairationAuditID_List.Add(new ReparationModel
                    {
                        RepairationAuditID = Convert.ToInt32(repairationAuditDataTable.Rows[0][0].ToString())
                    }
                    );

                    String qry_update_token_reparation_audit = "UPDATE repairation_audit SET SentUser = '" + Session["user"] + "',SentDate = CURDATE(),SentTime = CURTIME() WHERE TokenRepairationID = @ReparationAuditID";

                    MySqlCommand mySqlCmd_Reparation_Update_AuditDetails = new MySqlCommand(qry_update_token_reparation_audit, mySqlCon);
                    mySqlCmd_Reparation_Update_AuditDetails.Parameters.AddWithValue("@ReparationAuditID", reparationModel.RepairationAuditID);
                    mySqlCmd_Reparation_Update_AuditDetails.ExecuteNonQuery();
                }

                //ReparationModel reparationModel = new ReparationModel();

              /* string UserName = "0766061689"; //acount username
                string Password = "4873"; //account password
                string PhoneNo = "94" + mainModel.LoggedUserMobile.ToString();
                string Message = "Dear " + mainModel.LoggedUserName + ",The defect "+reparationModel.ProblemName+ "that you reported,will be recoverd by " +reparationModel.ReparationDepartment+ "on or before" + reparationModel.DeadLine+ ".Track the defect on ID : " +reparationModel.TokenAuditID+ "." ;

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
                }*/

            



                mySqlCommand_update_token_Reparation.ExecuteNonQuery();






            }


            return RedirectToAction("RepairationList");
        }

        public ActionResult CompleteRepairation(ReparationModel reparationModel)
        {
            DB dbConn = new DB();
            DataTable repairationCompleteDataTable = new DataTable();
     


            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_Complete_Tokens = "UPDATE token_flow SET CompleteStatus = 'Completed' , CompleteDate = CURDATE() WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCmd_Complete_Tokens = new MySqlCommand(qry_Complete_Tokens, mySqlCon);
                mySqlCmd_Complete_Tokens.Parameters.AddWithValue("@TokenAuditID", reparationModel.TokenAuditID);
                mySqlCmd_Complete_Tokens.ExecuteNonQuery();



            }

            return RedirectToAction("RepairationList");
        }


        public ActionResult ViewCompleteToken(int id)
        {
            DB dbConn = new DB();
            DataTable userDetailsDataTable = new DataTable();
            DataTable completedTokenDetailsDataTable = new DataTable();
            String RepAssignedEmail, RepDidEmail;


            MainModel mainModel = new MainModel();
            CompletedTokenModel completedTokenModel = new CompletedTokenModel();

            List<CompletedTokenModel> CompletedTokenModel_List = new List<CompletedTokenModel>();

            List<CompletedTokenModel> CompletedTokenModelTrack_Details_List = new List<CompletedTokenModel>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_listOfUserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage,UserDepartment FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfUserDetails, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);


                String qry_CompletedTokenDetails = "SELECT * FROM tokens tk, token_audit tka,token_flow tkf, token_image tkimg,token_review tkr,repairation_audit rpa WHERE tk.TokenAuditID = tka.TokenAuditID and tka.TokenAuditID = tkf.TokenAuditID and tkf.CompleteStatus = 'Completed' and tk.TokenAuditID = @TokenAuditID and tkimg.TokenID = tka.TokenAuditID and tkimg.TokenID = tkf.TokenAuditID and tkimg.TokenID = tk.TokenAuditID and tka.AddedUser = '" + Session["user"] + "' and tkr.TokenAuditID = tka.TokenAuditID and rpa.TokenAuditID = tkr.TokenAuditID";

                MySqlDataAdapter mySqlDa_CompletedTokenDetails = new MySqlDataAdapter(qry_CompletedTokenDetails, mySqlCon);
                mySqlDa_CompletedTokenDetails.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa_CompletedTokenDetails.Fill(completedTokenDetailsDataTable);


            }


            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            if (completedTokenDetailsDataTable.Rows.Count == 2)
            {

                CompletedTokenModel_List.Add(new CompletedTokenModel {

                    TokenAuditID = Convert.ToInt32(completedTokenDetailsDataTable.Rows[0][1].ToString()),
                    ProblemName = completedTokenDetailsDataTable.Rows[0][2].ToString(),
                    ProblemLocation = completedTokenDetailsDataTable.Rows[0][3].ToString(),
                    AttentionLevel = Convert.ToInt32(completedTokenDetailsDataTable.Rows[0][4].ToString()),
                    ProblemDescription= completedTokenDetailsDataTable.Rows[0][5].ToString(),
                    TokenAddedUserEmail = completedTokenDetailsDataTable.Rows[0][7].ToString(),
                    ProblemCategory = completedTokenDetailsDataTable.Rows[0][8].ToString(),
                    RepairationDepartment = completedTokenDetailsDataTable.Rows[0][12].ToString(),
                    DeadLine = completedTokenDetailsDataTable.Rows[0][13].ToString(),
                    FinalVerification = completedTokenDetailsDataTable.Rows[0][14].ToString(),
                    CompleteStatus = completedTokenDetailsDataTable.Rows[0][15].ToString(),
                    RepairedDate = completedTokenDetailsDataTable.Rows[0][16].ToString(),
                    FirstImagePath = completedTokenDetailsDataTable.Rows[0][20].ToString(),
                    SecondImagePath = completedTokenDetailsDataTable.Rows[1][20].ToString(),
                    TokenAddedDate = completedTokenDetailsDataTable.Rows[0][9].ToString(),

                    RepairationAssignedDateTime = completedTokenDetailsDataTable.Rows[0][25].ToString(),
                    ReparationAssignedUserEmail = completedTokenDetailsDataTable.Rows[0][26].ToString(),

                    RepairationDidUserEmail = completedTokenDetailsDataTable.Rows[0][32].ToString(),
                    RepairationFinishDate= completedTokenDetailsDataTable.Rows[0][33].ToString(),
                    RepairationFinishTime= completedTokenDetailsDataTable.Rows[0][34].ToString()

                }
                );

            }

            DataTable dataRepAssign = new DataTable();
            DataTable dataRepDid = new DataTable();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_get_RepAssignedName = "SELECT UserName,UserID FROM users WHERE UserEmail = '"+ completedTokenDetailsDataTable.Rows[0][26].ToString() + "'";
                MySqlDataAdapter mySqlDa_RepAssignedName = new MySqlDataAdapter(qry_get_RepAssignedName,mySqlCon);
                mySqlDa_RepAssignedName.Fill(dataRepAssign);

                String qry_get_RepDidName = "SELECT UserName,UserID FROM users WHERE UserEmail = '"+ completedTokenDetailsDataTable.Rows[0][32].ToString() + "'";
                MySqlDataAdapter mySqlDa_RepDidName = new MySqlDataAdapter(qry_get_RepDidName, mySqlCon);
                mySqlDa_RepDidName.Fill(dataRepDid);

            }


            if(dataRepAssign.Rows.Count == 1 && dataRepDid.Rows.Count == 1)
            {
                CompletedTokenModelTrack_Details_List.Add(new CompletedTokenModel
                {
                    RepAssignUserName = dataRepAssign.Rows[0][0].ToString(),
                    RepAssignUserID = dataRepAssign.Rows[0][1].ToString(),

                    RepDidUserName = dataRepDid.Rows[0][0].ToString(),
                    RepDidUserID = dataRepDid.Rows[0][1].ToString()
                });
            }



             mainModel.CompletedTokenList = CompletedTokenModel_List;
            mainModel.CompletedToken_Track_Details_List = CompletedTokenModelTrack_Details_List;

            return View(mainModel);


        }//view Complete Token method end



        public ActionResult RejectRepairation(CompletedTokenModel completedTokenModel)
        {
            DB dbConn = new DB();
            DataTable rejectedTokenDetailsDataTable = new DataTable();

            MainModel mainModel = new MainModel();
            

            List<CompletedTokenModel> CompletedTokenModel_List = new List<CompletedTokenModel>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_add_reject_list = "INSERT INTO rejected_tokens(TokenAuditID,RejectedDate,RejectedTime,RejectedReason,RejectRepairDept)VALUES(@TokenAuditID,CURDATE(),CURTIME(),@RejectReason,@RejectRepairDept)";

                MySqlCommand mySqlCmd_add_verifited_list = new MySqlCommand(qry_add_reject_list, mySqlCon);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@TokenAuditID", completedTokenModel.TokenAuditID);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@RejectRepairDept", completedTokenModel.RepairationDepartment);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@RejectReason", "InComplete");
                mySqlCmd_add_verifited_list.ExecuteNonQuery();


                String qry_reject_token_flow = "UPDATE token_flow SET FinalVerification = 'Rejected' , VerifiedDate = CURDATE() WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCmd_reject_token_flow = new MySqlCommand(qry_reject_token_flow, mySqlCon);
                mySqlCmd_reject_token_flow.Parameters.AddWithValue("@TokenAuditID", completedTokenModel.TokenAuditID);
                mySqlCmd_reject_token_flow.ExecuteNonQuery();

            }

               return RedirectToAction("MyTokens","Token");
        }//end of reject repairation method









        public ActionResult VerifyRepairation(CompletedTokenModel completedTokenModel)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();

            string name_of_file = Path.GetFileNameWithoutExtension(completedTokenModel.CompletedImageFile.FileName);
            string extension1 = Path.GetExtension(completedTokenModel.CompletedImageFile.FileName);
            name_of_file = name_of_file + DateTime.Now.ToString("yymmssfff") + extension1;
            completedTokenModel.CompletedImagePath = "~/completedImages/" + name_of_file;
            name_of_file = Path.Combine(Server.MapPath("~/completedImages/"), name_of_file);
            completedTokenModel.CompletedImageFile.SaveAs(name_of_file);

            String imgPath = completedTokenModel.CompletedImagePath;

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_add_verifited_list = "INSERT INTO verified_tokens(TokenAuditID,VerifiedDate,VerifiedTime,Image,SatisfactionLevel,RepairedDept)VALUES(@TokenAuditID,CURDATE(),CURTIME(),@CompletedimgPath,@SatisfactionLevel,@RepairedDept)";

                MySqlCommand mySqlCmd_add_verifited_list = new MySqlCommand(qry_add_verifited_list, mySqlCon);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@TokenAuditID", completedTokenModel.TokenAuditID);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@SatisfactionLevel", completedTokenModel.SatisfactionLevel);
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@CompletedimgPath", imgPath); 
                mySqlCmd_add_verifited_list.Parameters.AddWithValue("@RepairedDept", completedTokenModel.RepairationDepartment);
                mySqlCmd_add_verifited_list.ExecuteNonQuery();


                String qry_Complete_Tokens = "UPDATE token_flow SET FinalVerification = 'Verified' , VerifiedDate = CURDATE() WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCmd_update_verifited_list = new MySqlCommand(qry_Complete_Tokens, mySqlCon);
                mySqlCmd_update_verifited_list.Parameters.AddWithValue("@TokenAuditID", completedTokenModel.TokenAuditID);
                mySqlCmd_update_verifited_list.ExecuteNonQuery();



            }

            return RedirectToAction("MyTokens");
        }




        public ActionResult ProcessingTokensPreView()
        {
            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable dtbl_Expired_Tokens = new DataTable();
            MainModel mainModel = new MainModel();

            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            List<Token> Token_Expired_List = new List<Token>();



            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                String qry_myTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka,mas_isscs.tokens tk,mas_isscs.token_flow tkf,mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and (tkf.DeptLeaderStatus >= CURDATE() OR DeptLeaderStatus = 'Pending')";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlDA.Fill(dtblTokens);


                String qry_ExpiredTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka, mas_isscs.tokens tk, mas_isscs.token_flow tkf, mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and tkf.DeptLeaderStatus < CURDATE() and DeptLeaderStatus != 'Pending'";
                MySqlDataAdapter mySqlDA_Expired_Tokens = new MySqlDataAdapter(qry_ExpiredTokens, mySqlCon);
                mySqlDA_Expired_Tokens.Fill(dtbl_Expired_Tokens);


            }

            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {

                List_Token.Add(new Token
                {

                    ProblemName = dtblTokens.Rows[i][4].ToString(),
                    ProblemCategory = dtblTokens.Rows[i][1].ToString(),
                    Location = dtblTokens.Rows[i][5].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][6]),
                    UserName = dtblTokens.Rows[i][2].ToString(),
                    TokenStatus = dtblTokens.Rows[i][7].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0]),
                    AddedDate = dtblTokens.Rows[i][3].ToString(),
                    DeadLine = dtblTokens.Rows[i][8].ToString(),
                    CompleteStatus = dtblTokens.Rows[i][9].ToString(),
                    CompleteDate = dtblTokens.Rows[i][10].ToString(),
                    FinalVerificationStatus = dtblTokens.Rows[i][11].ToString()

                    //SentUser = dtblTokens.Rows[i][6].ToString()
                }
                );

            }

            for (int i = 0; i < dtbl_Expired_Tokens.Rows.Count; i++)
            {
                Token_Expired_List.Add(new Token
                {
                    ProblemName_Expired = dtbl_Expired_Tokens.Rows[i][4].ToString(),
                    ProblemCategory_Expired = dtbl_Expired_Tokens.Rows[i][1].ToString(),
                    Location_Expired = dtbl_Expired_Tokens.Rows[i][5].ToString(),
                    AttentionLevel_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][6]),
                    UserName_Expired = dtbl_Expired_Tokens.Rows[i][2].ToString(),
                    TokenStatus_Expired = dtbl_Expired_Tokens.Rows[i][7].ToString(),
                    TokenAuditID_Expired = Convert.ToInt32(dtbl_Expired_Tokens.Rows[i][0]),
                    AddedDate_Expired = dtbl_Expired_Tokens.Rows[i][3].ToString(),
                    DeadLine_Expired = dtbl_Expired_Tokens.Rows[i][8].ToString(),
                    CompleteStatus_Expired = dtbl_Expired_Tokens.Rows[i][9].ToString(),
                    CompleteDate_Expired = dtbl_Expired_Tokens.Rows[i][10].ToString(),
                    FinalVerificationStatus_Expired = dtbl_Expired_Tokens.Rows[i][11].ToString()


                });
            }

            mainModel.ListToken = List_Token;
        
            mainModel.TokenList = Token_List;
            mainModel.ExpiredTokensList = Token_Expired_List;
            return View(mainModel);

        }

        public ActionResult TokenManagerActionsPrviewTokens()
        {
            MainModel finalItem = new MainModel();
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }

            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable userDetailsDataTable = new DataTable();
            DataTable ForwardedTokeDataTable = new DataTable();

            DataTable ManagerStatus_pending_DataTable = new DataTable();


            MainModel mainModel = new MainModel();
            Token tokenModel = new Token();

            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            List<Token> TokenManagerPending_List = new List<Token>();



            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                string qry = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkFlow.TokenManagerStatus FROM users usr,tokens tk, token_audit tka,token_flow tkFlow WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = tkFlow.TokenAuditID";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry, mySqlCon);
                mySqlDA.Fill(dtblTokens);

                String qry_forwared_tokens = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName,tkFlow.TokenManagerStatus,tkreview.SentUser " +
                    "FROM users usr,tokens tk, token_audit tka,token_flow tkFlow, token_review tkreview " +
                    "WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = tkFlow.TokenAuditID AND tk.TokenAuditID = tkreview.TokenAuditID and tkreview.Status != 'Accept'";

                MySqlDataAdapter mySqlDAForwardedTokens = new MySqlDataAdapter(qry_forwared_tokens, mySqlCon);
                mySqlDAForwardedTokens.Fill(ForwardedTokeDataTable);

            }


            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {

                List_Token.Add(new Token
                {
                    ProblemName = dtblTokens.Rows[i][1].ToString(),
                    Location = dtblTokens.Rows[i][2].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][3]),
                    UserName = dtblTokens.Rows[i][4].ToString(),
                    TokenStatus = dtblTokens.Rows[i][5].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0])
                }
                );

            }

            for (int i = 0; i < ForwardedTokeDataTable.Rows.Count; i++)
            {
                Token_List.Add(new Token
                {
                    ProblemName = ForwardedTokeDataTable.Rows[i][1].ToString(),
                    Location = ForwardedTokeDataTable.Rows[i][2].ToString(),
                    AttentionLevel = Convert.ToInt32(ForwardedTokeDataTable.Rows[i][3]),
                    UserName = ForwardedTokeDataTable.Rows[i][4].ToString(),
                    TokenStatus = ForwardedTokeDataTable.Rows[i][5].ToString(),
                    TokenAuditID = Convert.ToInt32(ForwardedTokeDataTable.Rows[i][0]),
                    SentUser = ForwardedTokeDataTable.Rows[i][6].ToString()
                }
                );
            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            if (ManagerStatus_pending_DataTable.Rows.Count == 1)
            {
                mainModel.TokenManagerStatusPending = Convert.ToInt32(ManagerStatus_pending_DataTable.Rows[0][0].ToString());
            }


            mainModel.ListToken = List_Token;
            mainModel.ListUserLogin = List_UserLogin;
            mainModel.TokenList = Token_List;
            mainModel.TokenManagerStatusPending = TokenManagerPendingSattusCount();



            return View(mainModel);

      
        }


        public ActionResult TokenManagerReverseAction(Token tokenModel)
        {

            DB dbConn = new DB();
            String ForwardUser = Session["user"].ToString();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                string qry = "DELETE FROM token_review WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_Token_Reverse = new MySqlCommand(qry, mySqlCon);
                mySqlCmd_Token_Reverse.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCmd_Token_Reverse.ExecuteNonQuery();

                String update_token_status = "UPDATE token_flow SET TokenManagerStatus = 'Pending' WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_update_token_status = new MySqlCommand(update_token_status, mySqlCon);
                mySqlCommand_update_token_status.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCommand_update_token_status.ExecuteNonQuery();

                // return RedirectToAction("index");

            }

            return RedirectToAction("index","Token");

        }

        public ActionResult RevertToken(Token tokenModel)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_revert_token_flow = "UPDATE token_flow SET TokenManagerStatus = 'Revert' WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_revert_token_flow = new MySqlCommand(qry_revert_token_flow, mySqlCon);
                mySqlCommand_revert_token_flow.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                mySqlCommand_revert_token_flow.ExecuteNonQuery();

            }

            return RedirectToAction("index", "Token");
        }

        public ActionResult ReverseTokens()
        {

            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable userDetailsDataTable = new DataTable();
            MainModel mainModel = new MainModel();

 
            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                String qry_myTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka,mas_isscs.tokens tk,mas_isscs.token_flow tkf,mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and tkf.TokenManagerStatus = 'Revert'";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlDA.Fill(dtblTokens);

                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa_userDetails = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa_userDetails.Fill(userDetailsDataTable);

            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {
                Token_List.Add(new Token
                {
                    ProblemName = dtblTokens.Rows[i][4].ToString(),
                    ProblemCategory = dtblTokens.Rows[i][1].ToString(),
                    Location = dtblTokens.Rows[i][5].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][6]),
                    UserName = dtblTokens.Rows[i][2].ToString(),
                    TokenStatus = dtblTokens.Rows[i][7].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0]),
                    AddedDate = dtblTokens.Rows[i][3].ToString()
     
                }
             );

            }

            mainModel.ListToken = Token_List;
            return View(mainModel);
        }



        public ActionResult UpdateMyTokenCategory(Token tokenModel)
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String update_token_category = "UPDATE token_audit SET Category = @ProblemCategory WHERE TokenAuditID = @TokenAuditID";

                MySqlCommand mySqlCommand_update_token_category = new MySqlCommand(update_token_category, mySqlCon);
                mySqlCommand_update_token_category.Parameters.AddWithValue("@ProblemCategory", tokenModel.ProblemCategory);
                mySqlCommand_update_token_category.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);

                mySqlCommand_update_token_category.ExecuteNonQuery();


            }

            return RedirectToAction("MyTokens", "Token");
        }




        public ActionResult DeleteMyToken(int id)
        {
            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            MainModel mainModel = new MainModel();

            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_delete_MyToken_tokens = "DELETE FROM tokens WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_delete_MyToken_tokens = new MySqlCommand(qry_delete_MyToken_tokens, mySqlCon);
                mySqlCmd_delete_MyToken_tokens.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlCmd_delete_MyToken_tokens.ExecuteNonQuery();

                String qry_delete_MyToken_token_flow = "DELETE FROM token_flow WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_delete_MyToken_token_flow = new MySqlCommand(qry_delete_MyToken_token_flow, mySqlCon);
                mySqlCmd_delete_MyToken_token_flow.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlCmd_delete_MyToken_token_flow.ExecuteNonQuery();

            }

            return RedirectToAction("MyTokens","Token");
        }



        public ActionResult DeleteReverseToken(int id)
        {
            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            MainModel mainModel = new MainModel();

            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_delete_MyToken_tokens = "DELETE FROM tokens WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_delete_MyToken_tokens = new MySqlCommand(qry_delete_MyToken_tokens, mySqlCon);
                mySqlCmd_delete_MyToken_tokens.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlCmd_delete_MyToken_tokens.ExecuteNonQuery();

                String qry_delete_MyToken_token_flow = "DELETE FROM token_flow WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_delete_MyToken_token_flow = new MySqlCommand(qry_delete_MyToken_token_flow, mySqlCon);
                mySqlCmd_delete_MyToken_token_flow.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlCmd_delete_MyToken_token_flow.ExecuteNonQuery();

            }

            return RedirectToAction("ReverseTokens", "Token");
        }





        public ActionResult RepairRejectionTokenList()
        {
            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            DataTable userDetailsDataTable = new DataTable();
            MainModel mainModel = new MainModel();


            List<UserLogin> List_UserLogin = new List<UserLogin>();
            List<Token> List_Token = new List<Token>();
            List<Token> Token_List = new List<Token>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                String qry_myTokens = "SELECT tka.TokenAuditID,tka.Category,usr.UserName,tka.AddedDate,tk.ProblemName,tk.Location,tk.AttentionLevel,tkf.TokenManagerStatus,tkf.DeptLeaderStatus,tkf.CompleteStatus,tkf.CompleteDate,tkf.FinalVerification FROM mas_isscs.token_audit tka,mas_isscs.tokens tk,mas_isscs.token_flow tkf,mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = usr.UserEmail";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlDA.Fill(dtblTokens);

                String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserEmail,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa_userDetails = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                mySqlDa_userDetails.Fill(userDetailsDataTable);

            }

            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            for (int i = 0; i < dtblTokens.Rows.Count; i++)
            {
                Token_List.Add(new Token
                {
                    
                    ProblemName = dtblTokens.Rows[i][4].ToString(),
                    ProblemCategory = dtblTokens.Rows[i][1].ToString(),
                    Location = dtblTokens.Rows[i][5].ToString(),
                    AttentionLevel = Convert.ToInt32(dtblTokens.Rows[i][6]),
                    UserName = dtblTokens.Rows[i][2].ToString(),
                    TokenStatus = dtblTokens.Rows[i][7].ToString(),
                    TokenAuditID = Convert.ToInt32(dtblTokens.Rows[i][0]),
                    AddedDate = dtblTokens.Rows[i][3].ToString(),
                    DeadLine = dtblTokens.Rows[i][8].ToString(),
                    CompleteStatus = dtblTokens.Rows[i][9].ToString(),
                    CompleteDate = dtblTokens.Rows[i][10].ToString(),
                    FinalVerificationStatus = dtblTokens.Rows[i][11].ToString()

                }
             );

            }

            mainModel.ListToken = Token_List;
            return View(mainModel);
        }



        public ActionResult RecycleRejectedToken(Token tokenModel)
        {
            MainModel mainnModel = new MainModel();
            DB dbConn = new DB();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_reject_token_flow = "UPDATE token_flow SET TokenManagerStatus = 'Pending',DeptLeaderStatus = 'Pending',FinalVerification = 'Pending',CompleteStatus = 'Pending' , CompleteDate = 'Pending' , VerifiedDate = 'Pending' WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCommand_reject_token_flow = new MySqlCommand(qry_reject_token_flow, mySqlCon);
                mySqlCommand_reject_token_flow.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
               
                string qry = "DELETE FROM token_review WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_Token_Reverse = new MySqlCommand(qry, mySqlCon);
                mySqlCmd_Token_Reverse.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);
                

                string qry_repairation_audit = "DELETE FROM repairation_audit WHERE TokenAuditID = @TokenAuditID";
                MySqlCommand mySqlCmd_repairation_audit = new MySqlCommand(qry_repairation_audit, mySqlCon);
                mySqlCmd_repairation_audit.Parameters.AddWithValue("@TokenAuditID", tokenModel.TokenAuditID);


                mySqlCommand_reject_token_flow.ExecuteNonQuery();
                mySqlCmd_Token_Reverse.ExecuteNonQuery();
                mySqlCmd_repairation_audit.ExecuteNonQuery();

            }


            return RedirectToAction("RepairRejectionTokenList", "Token");
        }


        public ActionResult TokenCompletePathPreView(int id)
        {
            DB dbConn = new DB();
            DataTable userDetailsDataTable = new DataTable();
            DataTable completedTokenDetailsDataTable = new DataTable();
            String RepAssignedEmail, RepDidEmail;


            MainModel mainModel = new MainModel();
            CompletedTokenModel completedTokenModel = new CompletedTokenModel();

            List<CompletedTokenModel> CompletedTokenModel_List = new List<CompletedTokenModel>();

            List<CompletedTokenModel> CompletedTokenModelTrack_Details_List = new List<CompletedTokenModel>();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_listOfUserDetails = "SELECT UserName,UserType,UserID,UserEmail,UserImage,UserDepartment FROM users WHERE UserEmail = '" + Session["user"] + "'";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfUserDetails, mySqlCon);
                mySqlDa.Fill(userDetailsDataTable);


                String qry_CompletedTokenDetails = "SELECT * FROM tokens tk, token_audit tka,token_flow tkf, token_image tkimg,token_review tkr,repairation_audit rpa WHERE tk.TokenAuditID = tka.TokenAuditID and tka.TokenAuditID = tkf.TokenAuditID and tkf.CompleteStatus = 'Completed' and tk.TokenAuditID = @TokenAuditID and tkimg.TokenID = tka.TokenAuditID and tkimg.TokenID = tkf.TokenAuditID and tkimg.TokenID = tk.TokenAuditID and tka.AddedUser = '" + Session["user"] + "' and tkr.TokenAuditID = tka.TokenAuditID and rpa.TokenAuditID = tkr.TokenAuditID";

                MySqlDataAdapter mySqlDa_CompletedTokenDetails = new MySqlDataAdapter(qry_CompletedTokenDetails, mySqlCon);
                mySqlDa_CompletedTokenDetails.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa_CompletedTokenDetails.Fill(completedTokenDetailsDataTable);


            }


            if (userDetailsDataTable.Rows.Count == 1)
            {
                mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                mainModel.LoggedUserEmail = userDetailsDataTable.Rows[0][3].ToString();
                mainModel.UserImagePath = userDetailsDataTable.Rows[0][4].ToString();
            }


            if (completedTokenDetailsDataTable.Rows.Count == 2)
            {

                CompletedTokenModel_List.Add(new CompletedTokenModel
                {

                    TokenAuditID = Convert.ToInt32(completedTokenDetailsDataTable.Rows[0][1].ToString()),
                    ProblemName = completedTokenDetailsDataTable.Rows[0][2].ToString(),
                    ProblemLocation = completedTokenDetailsDataTable.Rows[0][3].ToString(),
                    AttentionLevel = Convert.ToInt32(completedTokenDetailsDataTable.Rows[0][4].ToString()),
                    ProblemDescription = completedTokenDetailsDataTable.Rows[0][5].ToString(),
                    TokenAddedUserEmail = completedTokenDetailsDataTable.Rows[0][7].ToString(),
                    ProblemCategory = completedTokenDetailsDataTable.Rows[0][8].ToString(),
                    RepairationDepartment = completedTokenDetailsDataTable.Rows[0][12].ToString(),
                    DeadLine = completedTokenDetailsDataTable.Rows[0][13].ToString(),
                    FinalVerification = completedTokenDetailsDataTable.Rows[0][14].ToString(),
                    CompleteStatus = completedTokenDetailsDataTable.Rows[0][15].ToString(),
                    RepairedDate = completedTokenDetailsDataTable.Rows[0][16].ToString(),
                    FirstImagePath = completedTokenDetailsDataTable.Rows[0][20].ToString(),
                    SecondImagePath = completedTokenDetailsDataTable.Rows[1][20].ToString(),
                    TokenAddedDate = completedTokenDetailsDataTable.Rows[0][9].ToString(),

                    RepairationAssignedDateTime = completedTokenDetailsDataTable.Rows[0][25].ToString(),
                    ReparationAssignedUserEmail = completedTokenDetailsDataTable.Rows[0][26].ToString(),

                    RepairationDidUserEmail = completedTokenDetailsDataTable.Rows[0][32].ToString(),
                    RepairationFinishDate = completedTokenDetailsDataTable.Rows[0][33].ToString(),
                    RepairationFinishTime = completedTokenDetailsDataTable.Rows[0][34].ToString()

                }
                );

            }

            DataTable dataRepAssign = new DataTable();
            DataTable dataRepDid = new DataTable();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry_get_RepAssignedName = "SELECT UserName,UserID FROM users WHERE UserEmail = '" + completedTokenDetailsDataTable.Rows[0][26].ToString() + "'";
                MySqlDataAdapter mySqlDa_RepAssignedName = new MySqlDataAdapter(qry_get_RepAssignedName, mySqlCon);
                mySqlDa_RepAssignedName.Fill(dataRepAssign);

                String qry_get_RepDidName = "SELECT UserName,UserID FROM users WHERE UserEmail = '" + completedTokenDetailsDataTable.Rows[0][32].ToString() + "'";
                MySqlDataAdapter mySqlDa_RepDidName = new MySqlDataAdapter(qry_get_RepDidName, mySqlCon);
                mySqlDa_RepDidName.Fill(dataRepDid);

            }


            if (dataRepAssign.Rows.Count == 1 && dataRepDid.Rows.Count == 1)
            {
                CompletedTokenModelTrack_Details_List.Add(new CompletedTokenModel
                {
                    RepAssignUserName = dataRepAssign.Rows[0][0].ToString(),
                    RepAssignUserID = dataRepAssign.Rows[0][1].ToString(),

                    RepDidUserName = dataRepDid.Rows[0][0].ToString(),
                    RepDidUserID = dataRepDid.Rows[0][1].ToString()
                });
            }



            mainModel.CompletedTokenList = CompletedTokenModel_List;
            mainModel.CompletedToken_Track_Details_List = CompletedTokenModelTrack_Details_List;

            return View(mainModel);
        }


    }
}
 



       