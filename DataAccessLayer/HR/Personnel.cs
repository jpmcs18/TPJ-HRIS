using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;

namespace DataAccessLayer.HR
{
    public class Personnel
    {
        public long ID { get; set; }
        public string Employee_No { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public bool? Gender { get; set; } = false;
        public DateTime? Date_Hired { get; set; }
        public int? Employment_Status_ID { get; set; }
        public DateTime? Resignation_Date { get; set; }
        public string Resignation_Remarks { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string Birth_Place { get; set; }
        public string Address { get; set; }
        public string Telephone_No { get; set; }
        public string Mobile_No { get; set; }
        public int? Civil_Status_ID { get; set; }
        public int? Religion_ID { get; set; }
        public string Other_Religion { get; set; }
        public int? Personnel_Type_ID { get; set; }
        public int? Personnel_Location_ID { get; set; }
        public int? Allowance_ID { get; set; }
        public int? Position_ID { get; set; }
        public int? Department_ID { get; set; }
        public int? Employment_Type_ID { get; set; }
        public string SSS { get; set; }
        public string Philhealth { get; set; }
        public string PAGIBIG { get; set; }
        public string TIN { get; set; }
        public string Seamans_Book_No { get; set; }
        public DateTime? Seamans_Book_Expiration_Date { get; set; }
        public string Passport_No { get; set; }
        public DateTime? Passport_Expiration_Date { get; set; }
        public string License_No { get; set; }
        public DateTime? License_Expiration_Date { get; set; }
        public string BFAR_No { get; set; }
        public DateTime? BFAR_Expiration_Date { get; set; }
        public DateTime? Yellow_Card_Expiration_Date { get; set; }
        public string Spouse_First_Name { get; set; }
        public string Spouse_Middle_Name { get; set; }
        public string Spouse_Last_Name { get; set; }
        public string Emergency_Contact_Person { get; set; }
        public string Emergency_Contact_Address { get; set; }
        public string Emergency_Contact_Number { get; set; }
        public string Email { get; set; }
        public string Referred_By { get; set; }
        public string Reference_Contact_No { get; set; }
        public bool Deleted { get; set; }
        public int? Created_By { get; set; }
        public DateTime? Created_On { get; set; }
        public int? Modified_By { get; set; }
        public DateTime? Modified_On { get; set; }

        public List<Personnel> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            List<Personnel> retval = null;
            using (var entity = new WebDBEntities())
            {
                var pl = from p in entity.Personnels
                         where p.Deleted == false
                         select new Personnel {
                             ID = p.ID,
                             Employee_No = p.Employee_No,
                             First_Name = p.First_Name,
                             Middle_Name = p.Middle_Name,
                             Last_Name = p.Last_Name
                         };

                if (!string.IsNullOrEmpty(filter))
                {
                    pl = from p in pl
                         where p.Employee_No.Contains(filter)
                         || p.First_Name.Contains(filter)
                         || p.Middle_Name.Contains(filter)
                         || p.Last_Name.Contains(filter)
                         select p;
                }

                count = pl.Count();
                retval = pl.OrderBy(p => p.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
            return retval;
        }
    }
}
