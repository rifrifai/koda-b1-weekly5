using System.Data;
using Microsoft.Data.SqlClient;

namespace KodaEWallet.Data;

public static class SqlParams
{
    public static SqlParameter Money(string name, decimal value) =>
        new SqlParameter(name, SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = value };

    public static SqlParameter Text(string name, string? value) =>
        new SqlParameter(name, SqlDbType.NVarChar) { Value = (object?)value ?? DBNull.Value };
}
