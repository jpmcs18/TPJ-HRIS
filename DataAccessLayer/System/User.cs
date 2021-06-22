using System;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Security;
using SecurityLib;
using DataLayer;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace DataAccessLayer.System
{
    public class User : IEntity
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [DisplayName("User Status")]
        public int? UserStatusID { get; set; }

        [DisplayName("Prefix")]
        public string Prefix { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [DisplayName("Gender")]
        public bool? Gender { get; set; }

        public string Password { get; set; }

        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }

        [DisplayName("Home Address")]
        [Required(ErrorMessage = "Home Address is required")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public int? CityID { get; set; }
        
        public string Zip { get; set; }
        
        public int? CreatedBy { get; set; }
        
        public DateTime? DateCreated { get; set; }

        public int? SignOnAttempts { get; set; }

        public bool? Deleted { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastLogoutDate { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public bool? ForcePasswordChange { get; set; }

        public string LastSessionID { get; set; }

        public string LastInternalIPAddressUsed { get; set; }

        public string LastExternalIPAddressUsed { get; set; }

        public string LastMacAddressUsed { get; set; }

        public string LastWorkstationNameUsed { get; set; }

        public bool? LoggedOn { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? LastModifiedby { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public string PasswordSalt { get; set; }

        public bool? IsVerifiedEmailAddress { get; set; }

        public bool? IsVerifiedMobileNumber { get; set; }

        public HR.Personnel Personnel { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public List<UserRole> Roles { get; set; }

        [DisplayName("Confirm Password")]
        [Compare(nameof(Password),ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public User()
        {
        }

        public User(Credentials credential)
        {
            this.Credential = credential;
        }

        public User(int id, Credentials credential) : this(id)
        {
            this.Credential = credential;
        }

        public User(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var user = (from u in entity.Users
                            where u.ID == id
                            select u).FirstOrDefault();

                if (user != null)
                {
                    this.ID = user.ID;
                    this.UserName = user.UserName;
                    this.UserStatusID = user.UserStatusID;
                    this.Prefix = user.Prefix;
                    this.LastName = user.LastName;
                    this.FirstName = user.FirstName;
                    this.MiddleName = user.MiddleName;
                    this.Gender = user.Gender;
                    this.EmailAddress = user.EmailAddress;
                    this.MobileNumber = user.MobileNumber;
                    this.Address1 = user.Address1;
                    this.IsVerifiedMobileNumber = user.isVerifiedMobileNumber;
                    this.ForcePasswordChange = user.ForcePasswordChange;

                    this.Roles = (from r in entity.UserRoles
                                 where (r.Deleted == null || r.Deleted == false)
                                 && r.UserID == id
                                 select new UserRole
                                 {
                                     RoleID = r.RoleID
                                 }).ToList();

                    this.Personnel = (from p in entity.Personnels
                                      where p.Deleted == false
                                      && p.ID == user.Personnel_ID
                                      select new HR.Personnel
                                      {
                                          ID = p.ID,
                                          Employee_No = p.Employee_No,
                                          First_Name = p.First_Name,
                                          Middle_Name = p.Middle_Name,
                                          Last_Name = p.Last_Name
                                      }).FirstOrDefault();


                }
            }
        }

        public List<User> GetList(int pagenumber, int gridcount, string filter, int? status, out int count)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var ul = from u in entity.Users
                         join p in entity.Personnels on u.Personnel_ID equals p.ID into _up
                         from up in _up.DefaultIfEmpty()
                         select new User
                         {
                             ID = u.ID,
                             UserName = u.UserName,
                             UserStatusID = u.UserStatusID,
                             LastName = u.LastName,
                             FirstName = u.FirstName,
                             MiddleName = u.MiddleName,
                             Personnel = new HR.Personnel {
                                 Employee_No = up.Employee_No
                             }
                         };

                if (!String.IsNullOrEmpty(filter))
                {
                    ul = from u in ul
                         where u.UserName.Contains(filter) || u.FirstName.Contains(filter)
                         || u.MiddleName.Contains(filter) || u.LastName.Contains(filter)
                         select u;
                }

                if (status > 0)
                {
                    ul = from u in ul
                         where u.UserStatusID == status
                         select u;
                }

                count = ul.Count();
                return ul.OrderBy(m => m.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return entity.Users.Any(u => u.ID == this.ID);
            }
        }

        public void AddRole(int id)
        {
            if (this.Roles == null)
            {
                this.Roles = new List<UserRole>();
            }
            this.Roles.Add(new UserRole { RoleID = id });
        }

        public void RemoveRole(int id)
        {
            this.Roles.Remove(this.Roles.Where(r => r.RoleID == id).FirstOrDefault());
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
                using (var transaction = entity.Database.BeginTransaction())
                {
                    try
                    {
                        var user = (from u in entity.Users
                                    where u.ID == this.ID
                                    select u).FirstOrDefault();

                        if (user != null)
                        {
                            user.UserStatusID = this.UserStatusID;
                            user.Prefix = this.Prefix;
                            user.LastName = this.LastName;
                            user.FirstName = this.FirstName;
                            user.MiddleName = this.MiddleName;
                            user.Gender = this.Gender;
                            user.EmailAddress = this.EmailAddress;
                            user.MobileNumber = this.MobileNumber;
                            user.Address1 = this.Address1;
                            
                            if (this.Personnel?.ID > 0)
                            {
                                user.Personnel_ID = this.Personnel.ID;
                            }
                            else
                            {
                                user.Personnel_ID = null;
                            }

                            if (entity.Entry(user).Property(nameof(user.MobileNumber)).IsModified)
                            {
                                user.isVerifiedMobileNumber = false;
                            }

                            if (!string.IsNullOrEmpty(this.Password))
                            {
                                user.PasswordSalt = SecurityHash.CreateSalt();
                                user.Password = SecurityHash.Encrypt(this.Password, user.PasswordSalt);
                                user.UserStatusID = 1; //active
                                user.SignOnAttempts = 0;
                                user.ForcePasswordChange = true;
                                this.Password = null;
                            }
                            if (entity.Entry(user).State == EntityState.Modified)
                            {
                                user.LastModifiedby = Credential?.UserID;
                                user.LastUpdateDate = DateTime.Now;
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            string salt = SecurityHash.CreateSalt();
                            user = new DataLayer.User
                            {
                                UserName = this.UserName,
                                UserStatusID = this.UserStatusID,
                                Prefix = this.Prefix,
                                LastName = this.LastName,
                                FirstName = this.FirstName,
                                MiddleName = this.MiddleName,
                                Gender = this.Gender,
                                EmailAddress = this.EmailAddress,
                                MobileNumber = this.MobileNumber,
                                Address1 = this.Address1,
                                PasswordSalt = salt,
                                Password = SecurityHash.Encrypt(this.Password, salt), 
                                CreatedBy = Credential?.UserID,
                                DateCreated = DateTime.Now
                            };
                            entity.Users.Add(user);
                            entity.SaveChanges();

                            if (this.Personnel?.ID > 0)
                            {
                                user.Personnel_ID = this.Personnel.ID;
                                entity.SaveChanges();
                            }

                            this.Password = null;
                            this.ID = user.ID;
                        }

                        var url = (from r in entity.UserRoles
                                     where r.UserID == user.ID
                                     select r).ToList();

                        foreach (UserRole role in this.Roles)
                        {
                            var ur = url.Where(r => r.RoleID == role.RoleID).FirstOrDefault();

                            if (ur != null)
                            {
                                url.Remove(ur);
                            }
                            else
                            {
                                ur = new DataLayer.UserRole
                                {
                                    UserID = user.ID,
                                    RoleID = role.RoleID,
                                    CreatedBy = Credential?.UserID,
                                    DateCreated = DateTime.Now
                                };
                                entity.UserRoles.Add(ur);
                                entity.SaveChanges();
                            }
                        }
                        if (url != null && url.Count > 0)
                        {
                            foreach (DataLayer.UserRole ur in url)
                            {
                                entity.UserRoles.Remove(ur);
                                if (entity.Entry(ur).State == EntityState.Deleted)
                                {
                                    entity.SaveChanges();
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var user = (from u in entity.Users
                                where u.ID == this.ID
                                select u).FirstOrDefault();
                    if (user != null)
                    {
                        user.UserStatusID = 2; //inactive
                        if (entity.Entry(user).State == EntityState.Modified)
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
                                    throw new Exception("This User is in use.");
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

        public List<User> LookupUsers()
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var retval = from u in entity.Users
                             where u.UserStatusID != 2 //inactive
                             select new User
                             {
                                 ID = u.ID,
                                 UserName = u.UserName
                             };

                return retval.ToList();
            }
        }

        public List<User> LookupUsersNoDesignation(int? except = null)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var des_users = from d in entity.Designations
                                   select d.UserID;

                if (except > 0)
                {
                    des_users = from d in des_users
                                where d != except.Value
                                select d;
                }

                var retval = from u in entity.Users
                             where !des_users.Contains(u.ID)
                             && u.UserStatusID != 2 //inactive
                             select new User
                             {
                                 ID = u.ID,
                                 UserName = u.UserName
                             };

                return retval.ToList();
            }
        }

        public Dictionary<int, string> LookupUserStatus()
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var retval = from us in entity.UserStatus
                             select new
                             {
                                 us.ID,
                                 us.Description
                             };

                return retval.ToDictionary(us => us.ID, us => us.Description);
            }
        }

        public void ChangePassword(int id, string currpass, string newpass)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var user = entity.Users.Where(u => u.ID == id).FirstOrDefault();
                if (user != null)
                {
                    string currsaltpass = SecurityLib.SecurityHash.Encrypt(currpass, user.PasswordSalt);
                    if (user.Password == currsaltpass)
                    {
                        user.PasswordSalt = SecurityLib.SecurityHash.CreateSalt();
                        user.Password = SecurityLib.SecurityHash.Encrypt(newpass, user.PasswordSalt);
                        user.LastPasswordChange = DateTime.Now;
                        user.ForcePasswordChange = false;
                        entity.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Password incorrect.");
                    }
                }
                else
                {
                    throw new Exception("User does not exist.");
                }
            }
        }
    }
}
