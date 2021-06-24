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
    public class Department
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }
        
        [DisplayName("Short Description")]
        [Required(ErrorMessage = "Short Description is Required")]
        public string ShortDescription { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public Department()
        {
        }

        public Department(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var dept = (from d in entity.Departments
                            where d.ID == id
                            select d).FirstOrDefault();
                if (dept != null)
                {
                    this.ID = dept.ID;
                    this.Description = dept.Description;
                    this.ShortDescription = dept.ShortDescription;
                    this.CreatedDate = dept.CreatedDate;
                    this.CreatedBy = dept.CreatedBy;
                    this.ModifiedDate = dept.ModifiedDate;
                    this.ModifiedBy = dept.ModifiedBy;
                }
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return (from d in entity.Departments
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
                var dept = (from d in entity.Departments
                            where d.ID == this.ID
                            select d).FirstOrDefault();
                if (dept != null)
                {
                    dept.Description = this.Description;
                    dept.ShortDescription = this.ShortDescription;
                    if (entity.Entry(dept).State == EntityState.Modified)
                    {
                        dept.ModifiedDate = DateTime.Now;
                        dept.ModifiedBy = this.Credential?.UserID;
                        entity.SaveChanges();
                    }
                }
                else
                {
                    dept = new DataLayer.Department
                    {
                        Description = this.Description,
                        ShortDescription = this.ShortDescription,
                        CreatedDate = DateTime.Now,
                        CreatedBy = this.Credential?.UserID
                    };
                    entity.Departments.Add(dept);
                    entity.SaveChanges();
                    this.ID = dept.ID;
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var dept = (from d in entity.Departments
                                where d.ID == this.ID
                                select d).FirstOrDefault();
                    if (dept != null)
                    {
                        entity.Departments.Remove(dept);
                        if (entity.Entry(dept).State == EntityState.Deleted)
                        {
                            entity.SaveChanges();
                        }
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
                                case 547: // Foreign Key violation
                                    throw new Exception("This Department is in use.");
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

        public void Delete(Credentials credential, int did)
        {
            this.Credential = credential;

            using (var entity = new WebDBEntities())
            {
                var dept = (from d in entity.Departments
                            where d.ID == this.ID
                            select d).FirstOrDefault();
                if (dept != null)
                {
                    var designation = (from d in entity.Designations
                                       where d.ID == did
                                       select d).FirstOrDefault();
                    if (designation != null)
                    {
                        designation.PositionID = null;
                        if (entity.Entry(designation).State == EntityState.Modified)
                        {
                            designation.ModifiedBy = credential?.UserID;
                            designation.ModifiedDate = DateTime.Now;
                            entity.SaveChanges();
                        }
                    }

                    entity.Departments.Remove(dept);
                    if (entity.Entry(dept).State == EntityState.Deleted)
                    {
                        entity.SaveChanges();
                    }
                }
            }
        }

        public List<Department> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            List<Department> retval = null;
            using (var entity = new WebDBEntities())
            {
                var dl = from d in entity.Departments
                               select new Department
                               {
                                   ID = d.ID,
                                   Description = d.Description,
                                   ShortDescription = d.ShortDescription,
                                   CreatedDate = d.CreatedDate,
                                   CreatedBy = d.CreatedBy,
                                   ModifiedDate = d.ModifiedDate,
                                   ModifiedBy = d.ModifiedBy
                               };

                if (!string.IsNullOrEmpty(filter))
                {
                    dl = from d in dl
                         where d.Description.Contains(filter) || d.ShortDescription.Contains(filter)
                         select d;
                }

                count = dl.Count();
                retval = dl.OrderBy(d => d.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
            return retval;
        }

        public List<Department> GetLookup()
        {
            List<Department> retval = null;
            using (var entity = new WebDBEntities())
            {
                var dl = from d in entity.Departments
                         select new Department
                         {
                             ID = d.ID,
                             Description = d.ShortDescription + " - " + d.Description
                         };
                
                retval = dl.ToList();
            }
            return retval;
        }
    }
}
