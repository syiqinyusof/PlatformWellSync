using PlatformWellSync.Data;
using PlatformWellSync.Dtos;
using PlatformWellSync.Models;

namespace PlatformWellSync.Services;

public class SyncService
{
    private readonly AppDbContext _db;

    public SyncService(AppDbContext db) => _db = db;

    public async Task SyncAsync(List<PlatformDto> platforms)
    {
        int platformsInserted = 0, platformsUpdated = 0, wellsInserted = 0, wellsUpdated = 0;

        foreach (var p in platforms)
        {
            if (p.Id is null) continue;

            var platform = await _db.Platforms.FindAsync(p.Id.Value);
            if (platform is null)
            {
                platform = new Platform { Id = p.Id.Value };
                _db.Platforms.Add(platform);
                platformsInserted++;
            }
            else
            {
                platformsUpdated++;
            }

            platform.UniqueName = p.UniqueName ?? platform.UniqueName;
            platform.Latitude   = p.Latitude   ?? platform.Latitude;
            platform.Longitude  = p.Longitude  ?? platform.Longitude;
            platform.CreatedAt  = p.CreatedAt  ?? platform.CreatedAt;
            platform.UpdatedAt  = p.UpdatedAt  ?? platform.UpdatedAt;

            foreach (var w in p.Well ?? new List<WellDto>())
            {
                if (w.Id is null) continue;

                var well = await _db.Wells.FindAsync(w.Id.Value);
                if (well is null)
                {
                    well = new Well { Id = w.Id.Value };
                    _db.Wells.Add(well);
                    wellsInserted++;
                }
                else
                {
                    wellsUpdated++;
                }

                well.PlatformId = platform.Id;
                well.UniqueName = w.UniqueName ?? well.UniqueName;
                well.Latitude   = w.Latitude   ?? well.Latitude;
                well.Longitude  = w.Longitude  ?? well.Longitude;
                well.CreatedAt  = w.CreatedAt  ?? well.CreatedAt;
                well.UpdatedAt  = w.UpdatedAt  ?? well.UpdatedAt;
            }
        }

        await _db.SaveChangesAsync();

        Console.WriteLine($"Platforms: {platformsInserted} inserted, {platformsUpdated} updated");
        Console.WriteLine($"Wells:     {wellsInserted} inserted, {wellsUpdated} updated");
    }
}