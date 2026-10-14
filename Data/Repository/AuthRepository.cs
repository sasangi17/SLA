
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

        public UserMl? Login(string email, string password)
        {
            const string query = @"
                SELECT
                    u.Id,
                    u.StaffId,
                    u.Email,
                    u.Password,
                    u.FullName,
                    u.Position,
                    u.Department,
                    u.MobilePhone,
                    u.Telephone,
                    u.Address,
                    u.LinkedInLink,
                    r.Role
                FROM dbo.Users AS u
                INNER JOIN dbo.Roles AS r
                    ON u.Id = r.UserId
                   AND u.Email = r.Email
                WHERE u.Email = @Email
                  AND u.Password = @Password
                ORDER BY r.Role;";

            var parameters =
                new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@Email",SqlDbType.NVarChar,email), 
                    DbParametersSql.SetValues("@Password",SqlDbType.NVarChar,password)
                };

            var table = _baseSqlDL.ExecuteReader(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            UserMl? user = null;

            foreach (DataRow row in table.Rows)
            {
                if (user == null)
                {
                    user = new UserMl
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

                string role = row["Role"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(role) &&
                    !user.Roles.Contains(role,StringComparer.OrdinalIgnoreCase))
                {
                    user.Roles.Add(role);
                }
            }

            return user;
        }
    }
}
