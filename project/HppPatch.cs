using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ClassToHpp.LoadHook;

namespace ClassToHpp
{
    internal static class HppPatch
    {
        public static void ApplyPatch()
        {
            LoadHook.Hook(Patch);
        }

        private static void Patch(Assembly assembly) 
        {
            if (assembly.GetName().Name != "Microsoft.VC.Wizards")
            {
                return;
            }

            Type controlEvents = assembly.GetType("Microsoft.VC.Wizards.ControlEvents");
            if (controlEvents == null) 
            { 
                LoadHook.UnHook(Patch);
                return;
            }

            MethodInfo setTextToHeader = controlEvents.GetMethod("SetTextToHeader", new Type[] { typeof(string) });
            if (setTextToHeader == null)
            {
                LoadHook.UnHook(Patch);
                return;
            }

            string dummy = null;

            s_Hook.Patch(setTextToHeader, postfix: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => SetTextToHeader("", ref dummy))));

            LoadHook.UnHook(Patch);
        }

        private static void SetTextToHeader(string text, ref string __result)
        {
            if (text.Length == 0)
            {
                return;
            }

            __result = text + ".hpp";
        }

        private static Harmony s_Hook = new Harmony("HppPatch");
    }
}
