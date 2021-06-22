using DataAccessLayer.Security;
using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessLayer.Organization
{
    public class Designation
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User is Required")]
        public int? UserID { get; set; }

        [DisplayName("Head")]
        public int? ParentUserID { get; set; }

        [DisplayName("Position")]
        public int? PositionID { get; set; }

        [DisplayName("Department")]
        public int? DepartmentID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public Designation()
        {
        }

        public Designation(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var des = (from d in entity.Designations
                           where d.ID == id
                           select d).FirstOrDefault();
                if (des != null)
                {
                    this.ID = des.ID;
                    this.UserID = des.UserID;
                    this.ParentUserID = des.ParentUserID;
                    this.PositionID = des.PositionID;
                    this.DepartmentID = des.DepartmentID;
                    this.CreatedDate = des.CreatedDate;
                    this.CreatedBy = des.CreatedBy;
                    this.ModifiedDate = des.ModifiedDate;
                    this.ModifiedBy = des.ModifiedBy;
                }
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return (from d in entity.Designations
                        where d.ID == this.ID
                        select d).Any();
            }
        }

        public void Save(Credentials credential)
        {
            this.Credential = credential;
            Save();
        }

        public void Save()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var des = (from d in entity.Designations
                               where d.ID == this.ID
                               select d).FirstOrDefault();
                    if (des != null)
                    {
                        des.UserID = this.UserID;
                        des.ParentUserID = this.ParentUserID;
                        des.PositionID = this.PositionID;
                        des.DepartmentID = this.DepartmentID;

                        if (entity.Entry(des).State == EntityState.Modified)
                        {
                            des.ModifiedDate = DateTime.Now;
                            des.ModifiedBy = this.Credential?.UserID;
                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        des = new DataLayer.Designation
                        {
                            UserID = this.UserID,
                            ParentUserID = this.ParentUserID,
                            PositionID = this.PositionID,
                            DepartmentID = this.DepartmentID,
                            CreatedDate = DateTime.Now,
                            CreatedBy = this.Credential?.UserID
                        };
                        entity.Designations.Add(des);
                        entity.SaveChanges();
                        this.ID = des.ID;
                    }
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException)
                    {
                        if (sqlException.Errors.Count > 0)
                        {
                            switch (sqlException.Errors[0].Number)
                            {
                                case 2627: // Foreign Key violation
                                    throw new Exception("User Name is already designated.");
                                default:
                                    throw;
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                var des = (from d in entity.Designations
                           where d.ID == this.ID
                           select d).FirstOrDefault();
                if (des != null)
                {
                    entity.Designations.Remove(des);
                    if (entity.Entry(des).State == EntityState.Deleted)
                    {
                        entity.SaveChanges();
                    }
                }
            }
        }

        public List<Designation> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            List<Designation> retval = null;
            using (var entity = new WebDBEntities())
            {
                var dl = from d in entity.Designations
                         select new Designation
                         {
                             ID = d.ID,
                             UserID = d.UserID,
                             ParentUserID = d.ParentUserID,
                             PositionID = d.PositionID,
                             DepartmentID = d.DepartmentID,
                             CreatedDate = d.CreatedDate,
                             CreatedBy = d.CreatedBy,
                             ModifiedDate = d.ModifiedDate,
                             ModifiedBy = d.ModifiedBy
                         };

                if (string.IsNullOrEmpty(filter))
                {
                }

                count = dl.Count();
                retval = dl.OrderBy(d => d.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
            return retval;
        }
    }
}
