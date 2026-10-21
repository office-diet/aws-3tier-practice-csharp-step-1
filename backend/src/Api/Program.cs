using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 接続文字列は環境変数から注入(コードに直書きしない)
var connStr = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not set");
builder.Services.AddSingleton(NpgsqlDataSource.Create(connStr));

var app = builder.Build();

// ヘルスチェック(後でALB/ECSが使う)
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

// 副作用があるのでPOST。UPSERT + RETURNING で原子的にカウントアップ
app.MapPost("/api/counter", async (NpgsqlDataSource db) =>
{
    await using var insertcmd = db.CreateCommand("""
        INSERT INTO counters DEFAULT VALUES
        RETURNING id;
        """);
    var id = await insertcmd.ExecuteScalarAsync();
    
    await using var cmd = db.CreateCommand("""
        SELECT COUNT(*) FROM counters;
        """);
    var count = (long)(await cmd.ExecuteScalarAsync())!;
    return Results.Ok(new { id, count });
});

app.MapGet("/api/counter", async (NpgsqlDataSource db) =>
{

    await using var cmd = db.CreateCommand("""
        SELECT id, accessed_at FROM counters ORDER BY accessed_at DESC;
    """);
    await using var reader = await cmd.ExecuteReaderAsync();

    var list = new List<CounterLog>();

    while ( await reader.ReadAsync() )
    {
        list.Add(new CounterLog(reader.GetGuid(0).ToString(), reader.GetDateTime(1)));
    }
    return Results.Ok( list );
});

app.Run();

record CounterLog(string Id, DateTime AccessedAt);
