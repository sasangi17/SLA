using System.Data;
using Niwahana_backend.Data.Repository.BaseSQLDbConnection;
using Niwahana_backend.Models.Domain;

namespace Niwahana_backend.Repository.Data
{
    public class BungalowsRepository
    {
        private readonly BaseSqlDL _baseSqlDL;

        public BungalowsRepository(BaseSqlDL baseSqlDL)
        {
            _baseSqlDL = baseSqlDL;
        }

        // GET ALL BUNGALOWS
        public List<BungalowsMl> GetAllBungalows()
        {
            const string query = @"
                SELECT
                    BungalowId,
                    BungalowName,
                    BungalowCode,
                    BungalowLocation,
                    IsActive,
                    CreateUser,
                    CreateDateTime,
                    UpdateUser,
                    UpdateDateTime
                FROM Bungalows
                ORDER BY BungalowId";

            var table = _baseSqlDL.ExecuteReader(
                query,
                new List<DbParametersSql.QueryParameters>()
            );

            var bungalowsList = new List<BungalowsMl>();

            foreach (DataRow row in table.Rows)
            {
                bungalowsList.Add(new BungalowsMl
                {
                    BungalowId = Convert.ToInt32(row["BungalowId"]),
                    BungalowName = row["BungalowName"]?.ToString() ?? string.Empty,
                    BungalowCode = row["BungalowCode"]?.ToString() ?? string.Empty,
                    BungalowLocation = row["BungalowLocation"]?.ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreateUser = Convert.ToInt32(row["CreateUser"]),
                    CreateDateTime = Convert.ToDateTime(row["CreateDateTime"]),
                    UpdateUser = row["UpdateUser"] == DBNull.Value ? null: Convert.ToInt32(row["UpdateUser"]),
                    UpdateDateTime = row["UpdateDateTime"] == DBNull.Value ? null : Convert.ToDateTime(row["UpdateDateTime"])
                });
            }

            return bungalowsList;
        }


        // GET BUNGALOW BY ID
        public BungalowsMl? GetBungalowsById(int bungalowsId)
        {
            const string query = @"
                SELECT
                    BungalowId,
                    BungalowName,
                    BungalowCode,
                    BungalowLocation,
                    IsActive,
                    CreateUser,
                    CreateDateTime,
                    UpdateUser,
                    UpdateDateTime
                FROM Bungalows
                WHERE BungalowId = @BungalowId";

            var parameters =
                new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@BungalowId",SqlDbType.Int,bungalowsId)
                };

            var table = _baseSqlDL.ExecuteReader(query,parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];

            return new BungalowsMl
            {
                BungalowId = Convert.ToInt32(row["BungalowId"]),
                BungalowName = row["BungalowName"]?.ToString() ?? string.Empty,
                BungalowCode = row["BungalowCode"]?.ToString() ?? string.Empty,
                BungalowLocation = row["BungalowLocation"]?.ToString() ?? string.Empty,
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreateUser = Convert.ToInt32(row["CreateUser"]),
                CreateDateTime = Convert.ToDateTime(row["CreateDateTime"]),
                UpdateUser = row["UpdateUser"] == DBNull.Value ? null: Convert.ToInt32(row["UpdateUser"]),
                UpdateDateTime = row["UpdateDateTime"] == DBNull.Value ? null : Convert.ToDateTime(row["UpdateDateTime"])
            };
        }


        // UPDATE BUNGALOWS
        public bool UpdateBungalows(
            BungalowsMl bungalows
        )
        {
            const string query = @"
                UPDATE Bungalows
                SET
                    BungalowName = @BungalowName,
                    BungalowCode = @BungalowCode,
                    BungalowLocation = @BungalowLocation,
                    IsActive = @IsActive,
                    UpdateUser = @UpdateUser,
                    UpdateDateTime = GETDATE()
                WHERE BungalowId = @BungalowId";
            var parameters =
                new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@BungalowId",SqlDbType.Int,bungalows.BungalowId),
                    DbParametersSql.SetValues("@BungalowName",SqlDbType.NVarChar,bungalows.BungalowName),
                    DbParametersSql.SetValues("@BungalowCode",SqlDbType.NVarChar,bungalows.BungalowCode),
                    DbParametersSql.SetValues("@BungalowLocation",SqlDbType.NVarChar,bungalows.BungalowLocation),
                    DbParametersSql.SetValues("@IsActive",SqlDbType.Bit,bungalows.IsActive),
                    DbParametersSql.SetValues("@UpdateUser", SqlDbType.Int, bungalows.UpdateUser)
                };

            var rows = _baseSqlDL.ExecuteNonQuery(query,parameters);

            return rows > 0;
        }
    }
}