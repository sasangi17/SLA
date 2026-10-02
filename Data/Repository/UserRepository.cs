using System.Data;
using Niwahana_backend.Data.Repository.BaseSQLDbConnection;
using Niwahana_backend.Models.Domain;

namespace Niwahana_backend.Repository.Data
{
    public class UserRepository
    {
        private readonly BaseSqlDL _baseSqlDL;

        public UserRepository(BaseSqlDL baseSqlDL)
        {
            _baseSqlDL = baseSqlDL;
        }

    
        // GET User
        
        public UserMl? GetUserById(
            int userId)
        {
            const string query = @"
                SELECT
                    Id,
                    StaffId,
                    Email,
                    Password,
                    FullName,
                    Position,
                    Department,
                    MobilePhone,
                    Telephone,
                    Address,
                    LinkedInLink
                FROM Users
                WHERE Id = @UserId";

            var parameters =
                new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,userId)
                };

            var table = _baseSqlDL.ExecuteReader(query,parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];

            return new UserMl
            {
                Id = Convert.ToInt32(row["Id"]),
                StaffId = Convert.ToInt32(row["StaffId"]),
                Email = row["Email"]?.ToString() ?? string.Empty,
                Password = row["Password"]?.ToString() ?? string.Empty,
                FullName = row["FullName"]?.ToString() ?? string.Empty,
                Position = row["Position"]?.ToString() ?? string.Empty,
                Department = row["Department"]?.ToString() ?? string.Empty,
                MobilePhone = row["MobilePhone"]?.ToString() ?? string.Empty,
                Telephone = row["Telephone"]?.ToString() ?? string.Empty,
                Address = row["Address"]?.ToString() ?? string.Empty,
                LinkedInLink = row["LinkedInLink"]?.ToString() ?? string.Empty
            };
        }

     
        // UPDATE User

        public bool UpdateUser(
            UserMl user)
        {
            const string query = @"
                UPDATE Users
                SET
                    Email = @Email,
                    FullName = @FullName,
                    Position = @Position,
                    Department = @Department,
                    MobilePhone = @MobilePhone,
                    Telephone = @Telephone,
                    Address = @Address,
                    LinkedInLink = @LinkedInLink
                WHERE Id = @Id";

            var parameters =
                new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@Id",SqlDbType.Int,user.Id),
                    DbParametersSql.SetValues("@Email",SqlDbType.NVarChar,user.Email),
                    DbParametersSql.SetValues("@FullName",SqlDbType.NVarChar,user.FullName),
                    DbParametersSql.SetValues("@Position",SqlDbType.NVarChar,user.Position),
                    DbParametersSql.SetValues("@Department",SqlDbType.NVarChar,user.Department),
                    DbParametersSql.SetValues("@MobilePhone",SqlDbType.NVarChar,user.MobilePhone),
                    DbParametersSql.SetValues("@Telephone",SqlDbType.NVarChar,user.Telephone),
                    DbParametersSql.SetValues("@Address",SqlDbType.NVarChar,user.Address),
                    DbParametersSql.SetValues("@LinkedInLink",SqlDbType.NVarChar,user.LinkedInLink)

                };

            var rows = _baseSqlDL.ExecuteNonQuery(query,parameters);
            return rows > 0;
        }
    }
}