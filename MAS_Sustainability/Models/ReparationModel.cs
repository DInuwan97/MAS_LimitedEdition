using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAS_Sustainability.Models
{
    public class ReparationModel
    {
        public int TokenAuditID { get; set; }
        public String ProblemName { get; set; }
        public String ProblemCategory { get; set; }
        public String ProblemLocation { get; set; }
        public int AttentionLevel { get; set; }
        public String ProblemAddedDate { get; set; }
        public String ProblemAddedUser { get; set; }
        public String ProblemDescription { get; set; }
        public String ReparationDepartment { get; set; }
        public String ProblemReviewedUser { get; set; }
        public String ReparationAcceptStatus { get; set; }
        public String ReparationDeadline { get; set; }
        public String ProblemFirstImagePath { get; set; }
        public String ProblemSecondImagePath { get; set; }
        public String ProblemReviewUserImagePath { get; set; }
        public String ProblemAddedUserImagePath { get; set; }
        public String ProblemReviewedDate { get; set; }
        public String SentUserEmail { get; set; }

        //side bar
        public int SideBarTokenAuditID { get; set; }
        public String SideBarProblemName { get; set; }
        public String SideBarProblemCategory { get; set; }
        public String SideBarProblemLocation { get; set; }
        public int SideBarAttentionLevel { get; set; }
        public String SideBarProblemAddedDate { get; set; }
        public String SideBarProblemFirstImagePath { get; set; }
        public String SideBarReparationDepartment { get; set; }


        ////
        public int UserID { get; set; }



    }
}