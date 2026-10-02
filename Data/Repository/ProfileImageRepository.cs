using System.Data;
using Niwahana_backend.Data.Repository.BaseSQLDbConnection;
using Niwahana_backend.Models.Domain;

namespace Niwahana_backend.Repository.Data
{
    public class ProfileImageRepository
    {
        private readonly BaseSqlDL _baseSqlDL;

        public ProfileImageRepository(BaseSqlDL baseSqlDL)
        {
            _baseSqlDL = baseSqlDL;
        }

        // -----------------------------------------
        // GET IMAGE
        // -----------------------------------------

        public ProfileImageMl? GetImage(int userId)
        {
            const string query = @"
                SELECT TOP 1
                    ImageId,
                    UserId,
                    ImageData,
                    ContentType
                FROM Images
                WHERE UserId = @UserId";

            var parameters = new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,userId)
                };

            var table = _baseSqlDL.ExecuteReader(query,parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];

            return new ProfileImageMl
            {
                ImageId =Convert.ToInt32(row["ImageId"]),
                UserId =Convert.ToInt32(row["UserId"]),
                ImageData = row["ImageData"] == DBNull.Value? null: (byte[])row["ImageData"],
                ContentType = row["ContentType"]?.ToString()?? "image/jpeg"
            };
        }

        // -----------------------------------------
        // CHECK IMAGE
        // -----------------------------------------

        public bool ImageExists(int userId)
        {
            const string query = @"
                SELECT COUNT(1)
                FROM Images
                WHERE UserId = @UserId";

            var parameters = new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,userId)
                };

            var result = _baseSqlDL.ExecuteScalar(query,parameters);

            return Convert.ToInt32(result) > 0;
        }

        // -----------------------------------------
        // ADD IMAGE
        // -----------------------------------------

        public bool AddImage(ProfileImageMl image)
        {
            const string query = @"
                INSERT INTO Images
                (
                    UserId,
                    ImageData,
                    ContentType
                )
                VALUES
                (
                    @UserId,
                    @ImageData,
                    @ContentType
                )";

            var parameters = new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,image.UserId),
                    DbParametersSql.SetValues("@ImageData",SqlDbType.VarBinary,image.ImageData),
                    DbParametersSql.SetValues("@ContentType",SqlDbType.NVarChar,image.ContentType)
                };

            var rows = _baseSqlDL.ExecuteNonQuery(query,parameters);

            return rows > 0;
        }

        // -----------------------------------------
        // UPDATE IMAGE
        // -----------------------------------------

        public bool UpdateImage(ProfileImageMl image)
        {
            const string query = @"
                UPDATE Images
                SET
                    ImageData = @ImageData,
                    ContentType = @ContentType
                WHERE UserId = @UserId";

            var parameters = new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,image.UserId),
                    DbParametersSql.SetValues("@ImageData",SqlDbType.VarBinary,image.ImageData),
                    DbParametersSql.SetValues("@ContentType",SqlDbType.NVarChar,image.ContentType)
                };

            var rows = _baseSqlDL.ExecuteNonQuery(query,parameters);
            return rows > 0;
        }

        // -----------------------------------------
        // DELETE IMAGE
        // -----------------------------------------

        public bool DeleteImage(int userId)
        {
            const string query = @"
                DELETE FROM Images
                WHERE UserId = @UserId";

            var parameters = new List<DbParametersSql.QueryParameters>
                {
                    DbParametersSql.SetValues("@UserId",SqlDbType.Int,userId)
                };

            var rows = _baseSqlDL.ExecuteNonQuery(query,parameters);
            return rows > 0;
        }
    }
}