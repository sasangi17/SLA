using System.Data;
using Niwahana_backend.Data.Repository.BaseSQLDbConnection;
using Niwahana_backend.Models.Domain;

namespace Niwahana_backend.Repository.Data
{
    public class AuthRepository
    {
        private readonly BaseSqlDL _baseSqlDL;

        public AuthRepository(BaseSqlDL baseSqlDL)
        {
            _baseSqlDL = baseSqlDL;
        }

        public UserMl? Login(
            string email,
            string password)
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
                WHERE Email = @Email
                  AND Password = @Password";

            var parameters = new List<DbParametersSql.QueryParameters>
            {
                DbParametersSql.SetValues("@Email",SqlDbType.NVarChar,email),
                DbParametersSql.SetValues("@Password",SqlDbType.NVarChar,password)
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
                Email = row["Email"]?.ToString()?? string.Empty,
                Password = row["Password"]?.ToString()?? string.Empty,
                FullName = row["FullName"]?.ToString()?? string.Empty,  
                Position = row["Position"]?.ToString()?? string.Empty,
                Department = row["Department"]?.ToString()?? string.Empty,
                MobilePhone = row["MobilePhone"]?.ToString()?? string.Empty,
                Telephone = row["Telephone"]?.ToString()?? string.Empty,
                Address = row["Address"]?.ToString()?? string.Empty,
                LinkedInLink = row["LinkedInLink"]?.ToString()?? string.Empty
            };
        }
    }
}