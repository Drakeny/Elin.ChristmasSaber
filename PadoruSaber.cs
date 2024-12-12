using System;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Padoru;

[BepInPlugin("DrakenyDev.Elin.Padoru", "Christmas Red Saber - Custom Adventurer", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    private static Harmony harmony;

    public static string dir;
    public void OnStartCore()
    {
        dir = Path.GetDirectoryName(Info.Location);
        // var excel = dir + "/Sourcetest.xlsx";
        // var sources = Core.Instance.sources;
        // ModUtil.ImportExcel(excel, "Thing", sources.things);
        // ModUtil.ImportExcel(excel, "ThingV", sources.thingV);
        // ModUtil.ImportExcel(excel, "Element", sources.elements);
    }

    private void Start()
    {

        Log = base.Logger;
        harmony = new Harmony("DrakenyDev.Elin.Padoru");
        harmony.PatchAll();
    }
}

