using System;

namespace ProcessLayer.Entities
{
    public sealed class PersonnelReport
    {
	    public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return String.Format("{0}{1}{2}{3}{4}", LastName
                                                      , (string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName) ? "" : ", ")
                                                      , FirstName
                                                      , (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(MiddleName) ? "" : " ")
                                                      , MiddleName);
            }
        }
        public decimal Amount { get; set; }
        public decimal PS { get; set; }
        public decimal ES { get; set; }
        public decimal EC { get; set; }
        public int Col { get; set; }
        public decimal Sum { get { return PS + ES + EC; } }
    }
}
