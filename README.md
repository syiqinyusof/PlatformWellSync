# AEM Enersol – Technical Assessment

## Part 1: .NET Application

A .NET 10 console app that syncs data from the AEM REST API into SQL Server using **Entity Framework Core (Code First)**.

**What it does**
1. Logs in to the API and gets a bearer token.
2. Calls `GetPlatformWellActual` with the token.
3. Saves platforms to the **Platform** table and wells to the **Well** table.
4. Updates existing records by `Id`, inserts new ones.
5. Handles `GetPlatformWellDummy` without breaking: missing keys keep existing values, new keys are ignored.

**How to run**

Windows (LocalDB):
```powershell
$env:AEM_DB_CONNECTION="Server=(localdb)\MSSQLLocalDB;Database=AemEnersolDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet run
```

macOS (SQL Server in Docker, since LocalDB is Windows-only):
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Aem@Test123" -p 1433:1433 --name sqlserver --platform linux/amd64 -d mcr.microsoft.com/mssql/server:2022-latest
dotnet run
```

Test the dummy endpoint:
```bash
dotnet run -- dummy
```

When the app starts, it asks for the API username and password (provided in the assessment email). The password is hidden while typing.
You can also set them as environment variables to skip the prompt: `AEM_API_USERNAME` and `AEM_API_PASSWORD`.

The database and tables are created automatically on the first run.

**Note on credentials:** API credentials are not stored in the code. The database password shown above is only for a local Docker container; in production it would come from an environment variable or a secret store.

## Part 2: SQL Query

Returns the last updated well for each platform. See [`Part2_LastUpdatedWell.sql`](Part2_LastUpdatedWell.sql).

```sql
WITH RankedWells AS (
    SELECT
        p.UniqueName AS PlatformName,
        w.Id, w.PlatformId, w.UniqueName, w.Latitude, w.Longitude, w.CreatedAt, w.UpdatedAt,
        ROW_NUMBER() OVER (PARTITION BY w.PlatformId ORDER BY w.UpdatedAt DESC) AS RowNum
    FROM Well w
    INNER JOIN Platform p ON p.Id = w.PlatformId
)
SELECT PlatformName, Id, PlatformId, UniqueName, Latitude, Longitude, CreatedAt, UpdatedAt
FROM RankedWells
WHERE RowNum = 1
ORDER BY PlatformName;
```

`ROW_NUMBER()` ranks wells within each platform by `UpdatedAt` (newest = 1), and `WHERE RowNum = 1` keeps the latest one. The output matches the expected result.
