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
    public class Position
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

        public Position()
        {
        }

        public Position(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var pos = (from p in entity.Positions
                           where p.ID == id
                           select p).FirstOrDefault();
                if (pos != null)
                {
                    this.ID = pos.ID;
                    this.Description = pos.Description;
                    this.ShortDescription = pos.ShortDescription;
                    this.CreatedDate = pos.CreatedDate;
                    this.CreatedBy = pos.CreatedBy;
                    this.ModifiedDate = pos.ModifiedDate;
                    this.ModifiedBy = pos.ModifiedBy;
                }
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return (from p in entity.Positions
                        where p.ID == this.ID
                        select p).Any();
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
                var pos = (from p in entity.Positions
                           where p.ID == this.ID
                           select p).FirstOrDefault();
                if (pos != null)
                {
                    pos.Description = this.Description;
                    pos.ShortDescription = this.ShortDescription;

                    if (entity.Entry(pos).State == EntityState.Modified)
                    {
                        pos.ModifiedDate = DateTime.Now;
                        pos.ModifiedBy = this.Credential?.UserID;
                        entity.SaveChanges();
                    }
                }
                else
                {
                    pos = new DataLayer.Position
                    {
                        Description = this.Description,
                        ShortDescription = this.ShortDescription,
                        CreatedDate = DateTime.Now,
                        CreatedBy = this.Credential?.UserID
                    };
                    entity.Positions.Add(pos);
                    entity.SaveChanges();
                    this.ID = pos.ID;
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var pos = (from p in entity.Positions
                               where p.ID == this.ID
                               select p).FirstOrDefault();
                    if (pos != null)
                    {
                        entity.Positions.Remove(pos);
                        if (entity.Entry(pos).State == EntityState.Deleted)
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
                                    throw new Exception("This Position is in use.");
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

        public void Delete(Credentials credential,int did)
        {
            this.Credential = credential;

            using (var entity = new WebDBEntities())
            {
                var pos = (from p in entity.Positions
                           where p.ID == this.ID
                           select p).FirstOrDefault();
                if (pos != null)
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

                    entity.Positions.Remove(pos);
                    if (entity.Entry(pos).State == EntityState.Deleted)
                    {
                        entity.SaveChanges();
                    }
                }
            }
        }

        public List<Position> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            List<Position> retval = null;
            using (var entity = new WebDBEntities())
            {
                var pl = from p in entity.Positions
                         select new Position
                         {
                             ID = p.ID,
                             Description = p.Description,
                             ShortDescription = p.ShortDescription,
                             CreatedDate = p.CreatedDate,
                             CreatedBy = p.CreatedBy,
                             ModifiedDate = p.ModifiedDate,
                             ModifiedBy = p.ModifiedBy
                         };

                if (!string.IsNullOrEmpty(filter))
                {
                    pl = from p in pl
                         where p.Description.Contains(filter) || p.ShortDescription.Contains(filter)
                         select p;
                }

                count = pl.Count();
                retval = pl.OrderBy(p => p.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
            return retval;
        }

        public List<Position> GetLookup()
        {
            List<Position> retval = null;
            using (var entity = new WebDBEntities())
            {
                var pl = from p in entity.Positions
                         select new Position
                         {
                             ID = p.ID,
                             Description = p.ShortDescription + " - " + p.Description
                         };

                retval = pl.ToList();
            }
            return retval;
        }
    }
}
