using MAS_Sustainability.Controllers;
using MAS_Sustainability.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAS_Sustainability
{
    public class MainModel
    {
        //public UserLogin userLoginViewModel { get; set; }
        // public Token tokenViewModel { get; set; }

        public List<Models.Survey> SurveyList { get; set; }
        public Models.Survey survey { get; set; }
        public List<ReparationModel> SingleTokenReparatiDetailsList { get; set; }
        public List<ReparationModel> SideBarTokenReparationDetails { get; set; }
        public List<ReparationModel> ReparationAuditIDList { get; set; }
        public List<CompletedTokenModel> CompletedTokenList { get; set; }

        public List<UserRegistrationModel> UserContList { get; set; }

        public List<Token> TokenMaagerStatusPendingList { get; set; }


        public List<Token> ListToken { get; set; }
        public List<UserLogin> ListUserLogin { get; set; }
        public List<UserRegistrationModel> ListUserRegistration { get; set; }
        public List<Token> TokenList { get; set; }

         public List<Report> ReportList { get; set; }
        public Report report { get; set; }

        public List<UserRegistrationModel> ForgottenDetails { get; set; }

        public int[] ArrTokenAuditID { get; set; }
        public int[] ArrAttentionLevel { get; set; }
        public String[] ArrFirstImagePath { get; set; }
        public String[] ArrProblemName { get; set; }
        public String[] ArrLocation { get; set; }
        public String[] ArrUserName { get; set; }
        public String[] ArrTokenStatus { get; set; }   
        public int TokenAuditID { get; set; }
        public String ProblemName { get; set; }
        public String ProblemCategory { get; set; }
        public String Location { get; set; }
        public String Description { get; set; }
        public String FirstImagePath { get; set; }
        public String SecondImagePath { get; set; }
        public int AttentionLevel { get; set; }
        public string UserName { get; set; }
        public string AddedDate { get; set; }
        public String ReparationDepartment { get; set; }
        public String SpecialActs { get; set; }
        public String SentDate { get; set; }
        public int no_of_token_rows_TokenManager { get; set; }
        public String TokenStatus { get; set; }
        public int no_of_rows_side_bar { get; set; }

        public String LoggedUserName { get; set; }
        public String LoggedUserType { get; set; }

        public int LoggedUserID { get; set; }

        public String LoggedUserEmail { get; set; }

        public String LoggedUserMobile { get; set; }


        public String LoggedUserDepartment { get; set; }



        //for UserManagement Controller's UserProfile View
        public String UserImagePath { get; set; }
        public String UserProfileImagePath { get; set; }
        public String[] ArrUserImagePath { get; set; }
        //for UserManagement Controller's UserProfile View



        public int SuccesMsg { get; set; }






        public int TokenManagerStatusPending { get; set; }


        public int DepartmentReparationCount_FacEng { get; set; }
        public int DepartmentReparationCount_ProdEng { get; set; }
        public int DepartmentReparationCount_Autonomation { get; set; }
        /////////////////////////////////////////////////////////////////////////

        public int DepartmentReparationCount_MOS { get; set; }
        public int DepartmentReparationCount_RM{ get; set; }
        public int DepartmentReparationCount_Quality { get; set; }
        ////////////////////////////////////////////////////////////////////////
        ///


        public int DepartmentReparationCount_Technical { get; set; }
        public int DepartmentReparationCount_Cutting { get; set; }
        public int DepartmentReparationCount_HR { get; set; }
        public int DepartmentReparationCount_FG { get; set; }

        public int DepartmentReparationCount_Operation { get; set; }


        public int DepartmentReparationCount_VSM1 { get; set; }
        public int DepartmentReparationCount_VSM2 { get; set; }
        public int DepartmentReparationCount_VSM3 { get; set; }
        public int DepartmentReparationCount_VSM4 { get; set; }


        public int DepartmentReparationCount_PreSewing { get; set; }
        public int DepartmentReparationCount_Emblishment { get; set; }
        public int DepartmentReparationCount_IE { get; set; }









        public int DepartmentReparationCount_FacEng_SUS { get; set; }
        public int DepartmentReparationCount_ProdEng_SUS { get; set; }
        public int DepartmentReparationCount_Autonomation_SUS { get; set; }
        /////////////////////////////////////////////////////////////////////////

        public int DepartmentReparationCount_MOS_SUS { get; set; }
        public int DepartmentReparationCount_RM_SUS { get; set; }
        public int DepartmentReparationCount_Quality_SUS { get; set; }
        ////////////////////////////////////////////////////////////////////////
        ///


        public int DepartmentReparationCount_Technical_SUS { get; set; }
        public int DepartmentReparationCount_Cutting_SUS { get; set; }
        public int DepartmentReparationCount_HR_SUS { get; set; }
        public int DepartmentReparationCount_FG_SUS { get; set; }

        public int DepartmentReparationCount_Operation_SUS { get; set; }


        public int DepartmentReparationCount_VSM1_SUS { get; set; }
        public int DepartmentReparationCount_VSM2_SUS { get; set; }
        public int DepartmentReparationCount_VSM3_SUS { get; set; }
        public int DepartmentReparationCount_VSM4_SUS { get; set; }


        public int DepartmentReparationCount_PreSewing_SUS { get; set; }
        public int DepartmentReparationCount_Emblishment_SUS { get; set; }
        public int DepartmentReparationCount_IE_SUS { get; set; }








        /*public MainModel()
        {
            new Token();
            new UserLogin();
        }*/
    }
}