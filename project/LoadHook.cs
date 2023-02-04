using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HarmonyLib;
using static System.Net.Mime.MediaTypeNames;

namespace ClassToHpp
{
    internal static class LoadHook
    {
        public delegate void OnLoad(Assembly loadedAssembly);

        private static void Setup()
        {
            if (s_nLoad == null)
            {
                Assembly reflectAssembly = typeof(Assembly).Assembly;
                Type runtimeAssembly = reflectAssembly.GetType("System.Reflection.RuntimeAssembly");

                s_nLoad = runtimeAssembly.GetMethod("nLoad", BindingFlags.Static | BindingFlags.NonPublic);
            }
        }

        public static void Hook(OnLoad onLoad)
        {
            Setup();

            Assembly dummy = null;

            if (s_Routines == null)
            {
                s_Routines = onLoad;

                s_Hook.Patch(s_nLoad, postfix: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => AssemblyLoaded(ref dummy))));
            }
            else
            {
                s_Routines += onLoad;
            }
        }

        public static void UnHook(OnLoad onLoad)
        {
            s_Routines -= onLoad;
            if (s_Routines == null)
            {
                s_Hook.Unpatch(s_nLoad, HarmonyPatchType.All);
            }
        }

        private static void AssemblyLoaded(ref Assembly __result)
        {
            s_Routines(__result);
        }

        private static OnLoad s_Routines;
        private static MethodInfo s_nLoad;

        private static Harmony s_Hook = new Harmony("LoadHook");
    }
}
