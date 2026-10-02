using System.Data;

namespace Niwahana_backend.Data.Repository.BaseSQLDbConnection
{
    public class DbParametersSql
    {
        public struct QueryParameters
        {
            public string name;

            public SqlDbType SqlDbType;

            public object? value;
        }

        public static QueryParameters SetValues(
            string name,
            SqlDbType type,
            object? value)
        {
            return new QueryParameters
            {
                name = name,
                SqlDbType = type,
                value = value
            };
        }
    }
}