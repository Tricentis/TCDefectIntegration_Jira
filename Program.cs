using System;
using System.Reflection;

namespace TCDefectIntegration {
    internal static class Program {
        private const string ArgCreateDefect = "create";

        private const string ArgOpenDefect = "open";

        private const string ArgGetStatesDefect = "getstates";

        private const string ArgGetDefectIdInfo = "getdefectidinfo";

        private static string ArgProgName {
            get {
                Assembly thisAssembly = Assembly.GetExecutingAssembly();
                return thisAssembly.ManifestModule.Name;
            }
        }

        private static int ShowUsage() {
            Console.WriteLine("Usage:");
            Console.WriteLine(ArgProgName + " " + ArgCreateDefect + " <dataFilePath> OR");
            Console.WriteLine(ArgProgName + " " + ArgOpenDefect + " <dataFilePath> OR");
            Console.WriteLine(ArgProgName + " " + ArgGetStatesDefect + " <dataFilePath> OR");
            Console.WriteLine(ArgProgName + " " + ArgGetDefectIdInfo + " <dataFilePath>");

            return -1;
        }

        private static int Main( string[] args ) {
            IntegrationManager integrationManager = new IntegrationManager();

            if (args.Length != 2) {
                return ShowUsage();
            }

            switch (args[0].ToLower()) {
                case ArgCreateDefect:
                    return integrationManager.CreateDefect(args[1]);
                case ArgOpenDefect:
                    return integrationManager.OpenDefect(args[1]);
                case ArgGetStatesDefect:
                    return integrationManager.GetStatesForDefects(args[1]);
                case ArgGetDefectIdInfo:
                    return integrationManager.GetInfosForDefects(args[1]);
                default:
                    return ShowUsage();
            }
        }
    }
}