using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAS_Sustainability.Models
{
    public class CompletedTokenModel
    {

        public int TokenAuditID { get; set; }
        public String ProblemName { get; set; }
        public String ProblemLocation { get; set; }
        public String ProblemDescription { get; set; }
        public int AttentionLevel { get; set; }
        public String ProblemCategory { get; set; }
        public String TokenAddedUserEmail { get; set; }
        public String TokenAddedDate { get; set; }
        public String TokenManagerEmail { get; set; }
        public String TokenManagerForwardDate { get; set; }
        public String DepartmentLeaderEmail { get; set; }
        public String RepairedDate { get; set; }
        public String DeadLine { get; set; }
        public String FirstImagePath { get; set; }
        public String SecondImagePath { get; set; }
        public String UserType { get; set; }
        public String RepairationDepartment { get; set; }
        public String CompleteStatus { get; set; }
        public String FinalVerification { get; set; }

        public HttpPostedFileBase CompletedImageFile { get; set; }

        public String CompletedImagePath { get; set; }

        public int SatisfactionLevel { get; set; }



    }
}