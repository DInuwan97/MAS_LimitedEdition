using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace MAS_Sustainability.Controllers
{


    public class DashbordController : Controller
    {
        public String SessionEmail = null;
        public String LoginUserName = null;


        public MainModel SetDashbordStatCounts()
        {
            DB dbConn = new DB();
            MainModel mainModel = new MainModel();
            DataTable dtblTokens = new DataTable();

            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                String qry_myTokens = "SELECT COUNT(*) FROM mas_isscs.token_audit tka,mas_isscs.tokens tk,mas_isscs.token_flow tkf,mas_isscs.users usr WHERE tka.TokenAuditID = tk.TokenAuditID  and tka.TokenAuditID = tkf.TokenAuditID AND tka.AddedUser = '" + Session["user"] + "' and tka.AddedUser = usr.UserEmail and tkf.TokenManagerStatus = 'Revert'";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry_myTokens, mySqlCon);
                mySqlDA.Fill(dtblTokens);

            }

            if (dtblTokens.Rows.Count == 1)
            {
                mainModel.TokenManagerRevertActions = Convert.ToInt32(dtblTokens.Rows[0][0].ToString());
            }

            return mainModel;

        }


        public String getReparationDepartmentCount(String DepartmentName,String Category)
        {
            String sql_get_reparation_count = "SELECT COUNT(*) FROM users usr,token_flow tkf,token_audit tka WHERE tkf.TokenAuditID = tka.TokenAuditID and tka.AddedUser = usr.UserEmail and tkf.FinalVerification = 'Verified' and usr.UserDepartment = '"+ DepartmentName+ "' and tka.Category = '"+Category+ "'";
            return sql_get_reparation_count;
        }


        public String getmaxReparationsDeparatment(String DepName)
        {
            String qty_max_repairation_dept = "SELECT Count(VerifiedTokenID),RepairedDept FROM verified_tokens where RepairedDept = '"+ DepName +"'";
            return qty_max_repairation_dept;
        }



        public ActionResult Index()
        {

            MainModel mainModel = new MainModel();
            HomeController home = new HomeController();  
            DataTable userDetailsDataTable = new DataTable();
            DB dbConn = new DB();

           

              if (Session["user"] == null)
              {
                  return RedirectToAction("Login", "UserLogin");
              }
              else
              {

                ///////////////////////Safety Tokens/////////////////////////////////
                DataTable dataTableRepairCount_FactoryEng = new DataTable();
                DataTable dataTableRepairCount_ProductionEng = new DataTable();
                DataTable dataTableRepairCount_Autonomation = new DataTable();

                DataTable dataTableRepairCount_MOS = new DataTable();
                DataTable dataTableRepairCount_RM = new DataTable();
                DataTable dataTableRepairCount_Quality = new DataTable();
                DataTable dataTableRepairCount_FG = new DataTable();

                DataTable dataTableRepairCount_Technical = new DataTable();
                DataTable dataTableRepairCount_Cutting = new DataTable();
                DataTable dataTableRepairCount_HR = new DataTable();

                DataTable dataTableRepairCount_Operation = new DataTable();

                DataTable dataTableRepairCount_VSM1 = new DataTable();
                DataTable dataTableRepairCount_VSM2 = new DataTable();
                DataTable dataTableRepairCount_VSM3 = new DataTable();
                DataTable dataTableRepairCount_VSM4 = new DataTable();

                DataTable dataTableRepairCount_PreSewing = new DataTable();
                DataTable dataTableRepairCount_Emblishment = new DataTable();
                DataTable dataTableRepairCount_IE = new DataTable();
                //////////////////////Sfety Tokens/////////////////////////////////


                //////////////////////Sustainability Tokens/////////////////////////////
                DataTable dataTableRepairCount_FactoryEng_SUS = new DataTable();
                DataTable dataTableRepairCount_ProductionEng_SUS = new DataTable();
                DataTable dataTableRepairCount_Autonomation_SUS = new DataTable();

                DataTable dataTableRepairCount_MOS_SUS = new DataTable();
                DataTable dataTableRepairCount_RM_SUS = new DataTable();
                DataTable dataTableRepairCount_Quality_SUS = new DataTable();
                DataTable dataTableRepairCount_FG_SUS = new DataTable();

                DataTable dataTableRepairCount_Technical_SUS = new DataTable();
                DataTable dataTableRepairCount_Cutting_SUS = new DataTable();
                DataTable dataTableRepairCount_HR_SUS = new DataTable();

                DataTable dataTableRepairCount_Operation_SUS = new DataTable();

                DataTable dataTableRepairCount_VSM1_SUS = new DataTable();
                DataTable dataTableRepairCount_VSM2_SUS = new DataTable();
                DataTable dataTableRepairCount_VSM3_SUS = new DataTable();
                DataTable dataTableRepairCount_VSM4_SUS = new DataTable();

                DataTable dataTableRepairCount_PreSewing_SUS = new DataTable();
                DataTable dataTableRepairCount_Emblishment_SUS = new DataTable();
                DataTable dataTableRepairCount_IE_SUS = new DataTable();
                //////////////////////Sustainability Tokens/////////////////////////////




                using (MySqlConnection mySqlCon = dbConn.DBConnection())
                {
                    mySqlCon.Open();

                    String qry_listOfTokens = "SELECT UserName,UserType,UserID,UserImage FROM users WHERE UserEmail = '" + Session["user"] + "'";
                    MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry_listOfTokens, mySqlCon);
                    mySqlDa.Fill(userDetailsDataTable);



                    String FacEng = getReparationDepartmentCount("Factory Engineering","Safety");
                    MySqlDataAdapter mySqlDa_FactoryEng = new MySqlDataAdapter(FacEng, mySqlCon);
                    mySqlDa_FactoryEng.Fill(dataTableRepairCount_FactoryEng);

                    String ProdEng = getReparationDepartmentCount("Production Engineering", "Safety");
                    MySqlDataAdapter mySqlDa_ProductionEng = new MySqlDataAdapter(ProdEng, mySqlCon);
                    mySqlDa_ProductionEng.Fill(dataTableRepairCount_ProductionEng);

                    String Autonomation = getReparationDepartmentCount("Autonomation","Safety");
                    MySqlDataAdapter mySqlDa_Autonomation = new MySqlDataAdapter(Autonomation, mySqlCon);
                    mySqlDa_Autonomation.Fill(dataTableRepairCount_Autonomation);


                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    String MOS = getReparationDepartmentCount("MOS","Safety");
                    MySqlDataAdapter mySqlDa_MOS = new MySqlDataAdapter(MOS, mySqlCon);
                    mySqlDa_MOS.Fill(dataTableRepairCount_MOS);
          
                    String RM = getReparationDepartmentCount("RM", "Safety");
                    MySqlDataAdapter mySqlDa_RM = new MySqlDataAdapter(RM, mySqlCon);
                    mySqlDa_RM.Fill(dataTableRepairCount_RM);

                    String Quality = getReparationDepartmentCount("Quality","Safety");
                    MySqlDataAdapter mySqlDa_Quality = new MySqlDataAdapter(Quality, mySqlCon);
                    mySqlDa_Quality.Fill(dataTableRepairCount_Quality);

                    String FG = getReparationDepartmentCount("FG","Safety");
                    MySqlDataAdapter mySqlDa_FG = new MySqlDataAdapter(FG, mySqlCon);
                    mySqlDa_FG.Fill(dataTableRepairCount_FG);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////


                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    String Technical = getReparationDepartmentCount("Technical","Safety");
                    MySqlDataAdapter mySqlDa_Technical = new MySqlDataAdapter(Technical, mySqlCon);
                    mySqlDa_Technical.Fill(dataTableRepairCount_Technical);

                    String Cutting = getReparationDepartmentCount("Cutting", "Safety");
                    MySqlDataAdapter mySqlDa_Cutting = new MySqlDataAdapter(Cutting, mySqlCon);
                    mySqlDa_Cutting.Fill(dataTableRepairCount_Cutting);

                    String HR = getReparationDepartmentCount("HR","Safety");
                    MySqlDataAdapter mySqlDa_HR = new MySqlDataAdapter(HR, mySqlCon);
                    mySqlDa_HR.Fill(dataTableRepairCount_HR);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    String Operation = getReparationDepartmentCount("Operation","Safety");
                    MySqlDataAdapter mySqlDa_Operation = new MySqlDataAdapter(Operation, mySqlCon);
                    mySqlDa_Operation.Fill(dataTableRepairCount_Operation);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///


                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    String VSM1 = getReparationDepartmentCount("Production VSM 01", "Safety");
                    MySqlDataAdapter mySqlDa_VSM1 = new MySqlDataAdapter(VSM1, mySqlCon);
                    mySqlDa_VSM1.Fill(dataTableRepairCount_VSM1);


                    String VSM2 = getReparationDepartmentCount("Production VSM 02","Safety");
                    MySqlDataAdapter mySqlDa_VSM2 = new MySqlDataAdapter(VSM2, mySqlCon);
                    mySqlDa_VSM2.Fill(dataTableRepairCount_VSM2);


                    String VSM3 = getReparationDepartmentCount("Production VSM 03","Safety");
                    MySqlDataAdapter mySqlDa_VSM3 = new MySqlDataAdapter(VSM3, mySqlCon);
                    mySqlDa_VSM3.Fill(dataTableRepairCount_VSM3);


                    String VSM4 = getReparationDepartmentCount("Production VSM 04","Safety");
                    MySqlDataAdapter mySqlDa_VSM4 = new MySqlDataAdapter(VSM4, mySqlCon);
                    mySqlDa_VSM4.Fill(dataTableRepairCount_VSM4);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////


                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    String PreSweing = getReparationDepartmentCount("Pre-Sweing", "Safety");
                    MySqlDataAdapter mySqlDa_PreSweing = new MySqlDataAdapter(PreSweing, mySqlCon);
                    mySqlDa_PreSweing.Fill(dataTableRepairCount_PreSewing);


                    String Emblishment = getReparationDepartmentCount("Emblishment","Safety");
                    MySqlDataAdapter mySqlDa_Emblishment = new MySqlDataAdapter(Emblishment, mySqlCon);
                    mySqlDa_Emblishment.Fill(dataTableRepairCount_Emblishment);


                    String IE = getReparationDepartmentCount("IE","Safety");
                    MySqlDataAdapter mySqlDa_IE = new MySqlDataAdapter(IE, mySqlCon);
                    mySqlDa_IE.Fill(dataTableRepairCount_IE);
                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    ///








                    /////////////////////////////////////////////////////////////////////Sustainability Tokens/////////////////////////////////

                    String FacEng_SUS = getReparationDepartmentCount("Factory Engineering", "Sustainability");
                    MySqlDataAdapter mySqlDa_FactoryEng_SUS = new MySqlDataAdapter(FacEng_SUS, mySqlCon);
                    mySqlDa_FactoryEng_SUS.Fill(dataTableRepairCount_FactoryEng_SUS);

                    String ProdEng_SUS = getReparationDepartmentCount("Production Engineering", "Sustainability");
                    MySqlDataAdapter mySqlDa_ProductionEng_SUS = new MySqlDataAdapter(ProdEng_SUS, mySqlCon);
                    mySqlDa_ProductionEng_SUS.Fill(dataTableRepairCount_ProductionEng_SUS);

                    String Autonomation_SUS = getReparationDepartmentCount("Autonomation", "Sustainability");
                    MySqlDataAdapter mySqlDa_Autonomation_SUS = new MySqlDataAdapter(Autonomation_SUS, mySqlCon);
                    mySqlDa_Autonomation_SUS.Fill(dataTableRepairCount_Autonomation_SUS);


                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    String MOS_SUS = getReparationDepartmentCount("MOS", "Sustainability");
                    MySqlDataAdapter mySqlDa_MOS_SUS = new MySqlDataAdapter(MOS_SUS, mySqlCon);
                    mySqlDa_MOS_SUS.Fill(dataTableRepairCount_MOS_SUS);

                    String RM_SUS = getReparationDepartmentCount("RM", "Sustainability");
                    MySqlDataAdapter mySqlDa_RM_SUS = new MySqlDataAdapter(RM_SUS, mySqlCon);
                    mySqlDa_RM_SUS.Fill(dataTableRepairCount_RM_SUS);

                    String Quality_SUS = getReparationDepartmentCount("Quality", "Sustainability");
                    MySqlDataAdapter mySqlDa_Quality_SUS = new MySqlDataAdapter(Quality_SUS, mySqlCon);
                    mySqlDa_Quality_SUS.Fill(dataTableRepairCount_Quality_SUS);

                    String FG_SUS = getReparationDepartmentCount("FG", "Sustainability");
                    MySqlDataAdapter mySqlDa_FG_SUS = new MySqlDataAdapter(FG_SUS, mySqlCon);
                    mySqlDa_FG_SUS.Fill(dataTableRepairCount_FG_SUS);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////


                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    String Technical_SUS = getReparationDepartmentCount("Technical", "Safety");
                    MySqlDataAdapter mySqlDa_Technical_SUS = new MySqlDataAdapter(Technical_SUS, mySqlCon);
                    mySqlDa_Technical_SUS.Fill(dataTableRepairCount_Technical_SUS);

                    String Cutting_SUS = getReparationDepartmentCount("Cutting", "Sustainability");
                    MySqlDataAdapter mySqlDa_Cutting_SUS = new MySqlDataAdapter(Cutting_SUS, mySqlCon);
                    mySqlDa_Cutting_SUS.Fill(dataTableRepairCount_Cutting_SUS);

                    String HR_SUS = getReparationDepartmentCount("HR", "Sustainability");
                    MySqlDataAdapter mySqlDa_HR_SUS = new MySqlDataAdapter(HR_SUS, mySqlCon);
                    mySqlDa_HR_SUS.Fill(dataTableRepairCount_HR_SUS);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    String Operation_SUS = getReparationDepartmentCount("Operation", "Sustainability");
                    MySqlDataAdapter mySqlDa_Operation_SUS = new MySqlDataAdapter(Operation_SUS, mySqlCon);
                    mySqlDa_Operation_SUS.Fill(dataTableRepairCount_Operation_SUS);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///


                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    String VSM1_SUS = getReparationDepartmentCount("Production VSM 01", "Sustainability");
                    MySqlDataAdapter mySqlDa_VSM1_SUS = new MySqlDataAdapter(VSM1_SUS, mySqlCon);
                    mySqlDa_VSM1_SUS.Fill(dataTableRepairCount_VSM1_SUS);


                    String VSM2_SUS = getReparationDepartmentCount("Production VSM 02", "Sustainability");
                    MySqlDataAdapter mySqlDa_VSM2_SUS = new MySqlDataAdapter(VSM2_SUS, mySqlCon);
                    mySqlDa_VSM2_SUS.Fill(dataTableRepairCount_VSM2_SUS);


                    String VSM3_SUS = getReparationDepartmentCount("Production VSM 03", "Sustainability");
                    MySqlDataAdapter mySqlDa_VSM3_SUS = new MySqlDataAdapter(VSM3_SUS, mySqlCon);
                    mySqlDa_VSM3_SUS.Fill(dataTableRepairCount_VSM3_SUS);


                    String VSM4_SUS = getReparationDepartmentCount("Production VSM 04", "Sustainability");
                    MySqlDataAdapter mySqlDa_VSM4_SUS = new MySqlDataAdapter(VSM4_SUS, mySqlCon);
                    mySqlDa_VSM4_SUS.Fill(dataTableRepairCount_VSM4_SUS);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////


                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    String PreSweing_SUS = getReparationDepartmentCount("Pre-Sweing", "Sustainability");
                    MySqlDataAdapter mySqlDa_PreSweing_SUS = new MySqlDataAdapter(PreSweing_SUS, mySqlCon);
                    mySqlDa_PreSweing_SUS.Fill(dataTableRepairCount_PreSewing_SUS);


                    String Emblishment_SUS = getReparationDepartmentCount("Emblishment", "Sustainability");
                    MySqlDataAdapter mySqlDa_Emblishment_SUS = new MySqlDataAdapter(Emblishment_SUS, mySqlCon);
                    mySqlDa_Emblishment_SUS.Fill(dataTableRepairCount_Emblishment_SUS);


                    String IE_SUS = getReparationDepartmentCount("IE", "Sustainability");
                    MySqlDataAdapter mySqlDa_IE_SUS = new MySqlDataAdapter(IE_SUS, mySqlCon);
                    mySqlDa_IE_SUS.Fill(dataTableRepairCount_IE_SUS);




                }

                ///////////////////////////////////////////////////////Safety//////////////////////////////////////////////////////////////////////////
                    mainModel.DepartmentReparationCount_FacEng = Convert.ToInt32(dataTableRepairCount_FactoryEng.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_ProdEng = Convert.ToInt32(dataTableRepairCount_ProductionEng.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_Autonomation = Convert.ToInt32(dataTableRepairCount_Autonomation.Rows[0][0].ToString());


                    mainModel.DepartmentReparationCount_MOS = Convert.ToInt32(dataTableRepairCount_MOS.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_RM = Convert.ToInt32(dataTableRepairCount_RM.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_Quality = Convert.ToInt32(dataTableRepairCount_Quality.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_FG = Convert.ToInt32(dataTableRepairCount_FG.Rows[0][0].ToString());


                    mainModel.DepartmentReparationCount_Technical = Convert.ToInt32(dataTableRepairCount_Technical.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_Cutting = Convert.ToInt32(dataTableRepairCount_Cutting.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_HR = Convert.ToInt32(dataTableRepairCount_HR.Rows[0][0].ToString());


                    mainModel.DepartmentReparationCount_Operation = Convert.ToInt32(dataTableRepairCount_Operation.Rows[0][0].ToString());


                    mainModel.DepartmentReparationCount_VSM1 = Convert.ToInt32(dataTableRepairCount_VSM1.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_VSM2 = Convert.ToInt32(dataTableRepairCount_VSM2.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_VSM3 = Convert.ToInt32(dataTableRepairCount_VSM3.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_VSM4 = Convert.ToInt32(dataTableRepairCount_VSM4.Rows[0][0].ToString());


                    mainModel.DepartmentReparationCount_PreSewing = Convert.ToInt32(dataTableRepairCount_PreSewing.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_Emblishment = Convert.ToInt32(dataTableRepairCount_Emblishment.Rows[0][0].ToString());
                    mainModel.DepartmentReparationCount_IE = Convert.ToInt32(dataTableRepairCount_IE.Rows[0][0].ToString());



                ///////////////////////////////////////////////////////Sustainability//////////////////////////////////////////////////////////////////////////
                mainModel.DepartmentReparationCount_FacEng_SUS = Convert.ToInt32(dataTableRepairCount_FactoryEng_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_ProdEng_SUS = Convert.ToInt32(dataTableRepairCount_ProductionEng_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_Autonomation_SUS = Convert.ToInt32(dataTableRepairCount_Autonomation_SUS.Rows[0][0].ToString());


                mainModel.DepartmentReparationCount_MOS_SUS = Convert.ToInt32(dataTableRepairCount_MOS_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_RM_SUS = Convert.ToInt32(dataTableRepairCount_RM_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_Quality_SUS = Convert.ToInt32(dataTableRepairCount_Quality_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_FG_SUS = Convert.ToInt32(dataTableRepairCount_FG_SUS.Rows[0][0].ToString());


                mainModel.DepartmentReparationCount_Technical_SUS = Convert.ToInt32(dataTableRepairCount_Technical_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_Cutting_SUS = Convert.ToInt32(dataTableRepairCount_Cutting_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_HR_SUS = Convert.ToInt32(dataTableRepairCount_HR_SUS.Rows[0][0].ToString());


                mainModel.DepartmentReparationCount_Operation_SUS = Convert.ToInt32(dataTableRepairCount_Operation_SUS.Rows[0][0].ToString());


                mainModel.DepartmentReparationCount_VSM1_SUS = Convert.ToInt32(dataTableRepairCount_VSM1_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_VSM2_SUS = Convert.ToInt32(dataTableRepairCount_VSM2_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_VSM3_SUS = Convert.ToInt32(dataTableRepairCount_VSM3_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_VSM4_SUS = Convert.ToInt32(dataTableRepairCount_VSM4_SUS.Rows[0][0].ToString());


                mainModel.DepartmentReparationCount_PreSewing_SUS = Convert.ToInt32(dataTableRepairCount_PreSewing_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_Emblishment_SUS = Convert.ToInt32(dataTableRepairCount_Emblishment_SUS.Rows[0][0].ToString());
                mainModel.DepartmentReparationCount_IE_SUS = Convert.ToInt32(dataTableRepairCount_IE_SUS.Rows[0][0].ToString());











                if (userDetailsDataTable.Rows.Count == 1)
                {
                    SetDashbordStatCounts();
                    mainModel.TokenManagerStatusPending = TokenController.TokenManagerPendingSattusCount();

                    mainModel.LoggedUserName = userDetailsDataTable.Rows[0][0].ToString();
                    mainModel.LoggedUserType = userDetailsDataTable.Rows[0][1].ToString();
                    mainModel.LoggedUserID = Convert.ToInt32(userDetailsDataTable.Rows[0][2]);
                    mainModel.UserImagePath = userDetailsDataTable.Rows[0][3].ToString();


                }




                ///////////////////////Safety Tokens/////////////////////////////////
                DataTable dataTableRepairCount_Complete_FactoryEng = new DataTable();
                DataTable dataTableRepairCount_Complete_ProductionEng = new DataTable();
                DataTable dataTableRepairCount_Complete_Autonomation = new DataTable();

                DataTable dataTableRepairCount_Complete_MOS = new DataTable();
                DataTable dataTableRepairCount_Complete_RM = new DataTable();
                DataTable dataTableRepairCount_Complete_Quality = new DataTable();
                DataTable dataTableRepairCount_Complete_FG = new DataTable();

                DataTable dataTableRepairCount_Complete_Technical = new DataTable();
                DataTable dataTableRepairCount_Complete_Cutting = new DataTable();
                DataTable dataTableRepairCount_Complete_HR = new DataTable();

                DataTable dataTableRepairCount_Complete_Operation = new DataTable();

                DataTable dataTableRepairCount_Complete_VSM1 = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM2 = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM3 = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM4 = new DataTable();

                DataTable dataTableRepairCount_Complete_PreSewing = new DataTable();
                DataTable dataTableRepairCount_Complete_Emblishment = new DataTable();
                DataTable dataTableRepairCount_Complete_IE = new DataTable();
                //////////////////////Sfety Tokens/////////////////////////////////


                //////////////////////Sustainability Tokens/////////////////////////////
                DataTable dataTableRepairCount_Complete_FactoryEng_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_ProductionEng_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_Autonomation_SUS = new DataTable();

                DataTable dataTableRepairCount_Complete_MOS_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_RM_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_Quality_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_FG_SUS = new DataTable();

                DataTable dataTableRepairCount_Complete_Technical_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_Cutting_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_HR_SUS = new DataTable();

                DataTable dataTableRepairCount_Complete_Operation_SUS = new DataTable();

                DataTable dataTableRepairCount_Complete_VSM1_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM2_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM3_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_VSM4_SUS = new DataTable();

                DataTable dataTableRepairCount_Complete_PreSewing_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_Emblishment_SUS = new DataTable();
                DataTable dataTableRepairCount_Complete_IE_SUS = new DataTable();
                //////////////////////Sustainability Tokens/////////////////////////////
























                return View(mainModel);

            }
           


        }



     

        public String getLoggedUserName() {
            return LoginUserName;

        }

        public String getSessionEmail()
        {
            return SessionEmail;
        }
    }
}