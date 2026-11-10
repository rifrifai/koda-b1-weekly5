using Microsoft.Data.SqlClient;

namespace KodaEWallet.Data;
public class DbConnection
{
    private readonly string _connString;
    public DbConnection(IConfiguration config)
    {
        _connString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionStrings:Default belum diisi di appsettings.json");
    }

    public async Task<SqlConnection> OpenAsync(CancellationToken ct)
    {
        var connection = new SqlConnection(_connString);
        await connection.OpenAsync(ct);
        
        return connection;
    }
}