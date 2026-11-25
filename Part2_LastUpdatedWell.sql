-- Part 2: Return the last updated well for each platform
-- Database: AemEnersolDb

USE AemEnersolDb;
GO

WITH RankedWells AS (
    SELECT
        p.UniqueName AS PlatformName,
        w.Id,
        w.PlatformId,
        w.UniqueName,
        w.Latitude,
        w.Longitude,
        w.CreatedAt,
        w.UpdatedAt,
        ROW_NUMBER() OVER (
            PARTITION BY w.PlatformId
            ORDER BY w.UpdatedAt DESC
        ) AS RowNum
    FROM Well w
    INNER JOIN Platform p ON p.Id = w.PlatformId
)
SELECT PlatformName, Id, PlatformId, UniqueName, Latitude, Longitude, CreatedAt, UpdatedAt
FROM RankedWells
WHERE RowNum = 1
ORDER BY PlatformName;
