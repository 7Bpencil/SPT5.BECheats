using Spectre;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace SevenBoldPencil.BECheats.Server;

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class Plugin(
    GlobalTable globalTable,
    BotTable botTable,
    LocationTable locationTable,
    ISptLogger<Plugin> logger
) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        Location[] locations = [locationTable.Bigmap, locationTable.Woods, locationTable.Shoreline];
        foreach (var location in locations)
        {
            foreach (var bossSpawn in location.Base.BossLocationSpawn)
            {
                if (bossSpawn.BossName == "sectantPriest")
                {
                    bossSpawn.BossChance = 100;
                }
            }
        }

        return Task.CompletedTask;
    }
}
