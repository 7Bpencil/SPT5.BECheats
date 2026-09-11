using BepInEx;
using BepInEx.Unity.IL2CPP;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
using SPTushonka.Reflection.Patching;
using System.Reflection;

namespace SevenBoldPencil.BECheats.Client;

[BepInPlugin("7Bpencil.BECheats.Client", "7Bpencil.BECheats.Client", "0.0.1")]
public class Plugin : BasePlugin
{
	public override void Load()
	{
        new Patch_ActiveHealthController_ApplyDamage().Enable();
        new Patch_Physical_Update().Enable();
	}
}

public class Patch_ActiveHealthController_ApplyDamage : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.ApplyDamage));
	}

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController __instance, ref float __result)
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            return true;
        }

        var mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
        if (mainPlayer && mainPlayer.ActiveHealthController == __instance)
        {
            __result = 0f;
            return false;
        }

        return true;
    }
}

public class Patch_Physical_Update : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Physical), nameof(Physical.Update));
	}

    [PatchPostfix]
    public static void Postfix(Physical __instance)
    {
        var player = __instance._player;
        if (player && player.IsYourPlayer)
        {
            __instance.Stamina.Current = __instance.Stamina.TotalCapacity;
            __instance.HandsStamina.Current = __instance.HandsStamina.TotalCapacity;
            __instance.Oxygen.Current = __instance.Oxygen.TotalCapacity;
        }
    }
}
