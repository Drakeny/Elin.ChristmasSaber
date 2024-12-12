using System;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;

namespace Padoru;
[HarmonyPatch]
public class Padoru
{
    [HarmonyPostfix, HarmonyPatch(typeof(Chara), "GetTopicText")]
    public static void GetTopicText(Chara __instance, ref string __result, string topic)
    {
        if (__instance.source.id == "padoru" && topic == "fov")
        {
            switch (__result)
            {
                case "\"Hashire sori yo…\"":
                    SE.Play("padoru-1");
                    break;
                case "\"Kaze no you ni.\"":
                    SE.Play("padoru-2");
                    break;
                case "\"Tsukimihara wo...\"":
                    SE.Play("padoru-3");
                    break;
                case "\"PADORU PADORU\"":
                    SE.Play("padoru-4");
                    break;
                case "!ALLSONG":
                    SE.Play("padoru-5");
                    __result = "\"Hashire sori yo…\"";
                    break;
            }
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(DramaCustomSequence), "Build")]
    public static bool Build(DramaCustomSequence __instance, Chara c)
    {
        if (c.source.id == "padoru" && !CanReceivePresent())
        {
            __instance.Step("Resident");
            __instance._Talk("tg", () => "Umu~! You get a present too, Merry~!");
            DramaChoice choice = __instance.Choice2("Thank you!", "_bye");
            choice.SetOnClick(delegate
            {
                ReceivePresent();
            });

            __instance.Step("_bye");
            __instance.Method(delegate
            {
                c.Talk("Umu~");
            });
            __instance.End();
            return false;
        }
        return true;
    }

    public static void ReceivePresent()
    {
        Thing t = ThingGen.Create("padoru_gift");
        int lv = 0;
        if (EClass.player.CountKeyItem("license_adv") == 0 && !EClass.debug.enable)
        {
            lv = ((EClass.rnd(3) == 0) ? EClass.pc.LV : EClass.pc.FameLv) * (75 + EClass.rnd(50)) / 100 + EClass.rnd(EClass.rnd(10) + 1) - 3;
            if (lv >= 50)
            {
                lv = EClass.rndHalf(50);
            }
        }
        else
        {
            lv = EClass.pc.FameLv * 100 / (100 + EClass.rnd(50)) + EClass.rnd(EClass.rnd(10) + 1) - 3;
            if (EClass.rnd(10) == 0)
            {
                lv = lv * 3 / 2;
            }
            if (EClass.rnd(10) == 0)
            {
                lv /= 2;
            }
        }
        ThingGen.CreateTreasureContent(t, lv, TreasureType.BossNefia, true);
        SE.Play("dropReward");
        EClass._zone.AddCard(t, EClass.pc.pos);
    }

    public static bool CanReceivePresent()
    {
        int currentYear = EClass.world.date.year;
        string pathSave = Plugin.dir + "/" + EClass.pc.NameSimple + "_" + "padoru.json";
        if (File.Exists(pathSave))
        {

            PresentCheck check = JsonConvert.DeserializeObject<PresentCheck>(File.ReadAllText(pathSave));
            if (check.LastYearReceived != currentYear)
            {
                check.LastYearReceived = currentYear;
                File.WriteAllText(pathSave, JsonConvert.SerializeObject(check));
                return true;
            }
            return false;
        }
        else
        {
            Plugin.Log.LogMessage("Presents never received before!");
            PresentCheck check = new PresentCheck();
            check.LastYearReceived = currentYear;
            File.WriteAllText(pathSave, JsonConvert.SerializeObject(check));
            return true;
        }
    }

}

public class PresentCheck
{
    public int LastYearReceived { get; set; }
}


