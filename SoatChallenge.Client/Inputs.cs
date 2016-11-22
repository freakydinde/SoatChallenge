namespace SoatChallenge.Client
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>inputs used for tests</summary>
    public static class Inputs
    {
        private static string resourcesFolder;

        /// <summary>Gets real challenge input</summary>
        public static string ChallengeInput
        {
            get
            {
                return Path.Combine(Inputs.ResourcesFolder, "ChallengeInput.txt");
            }
        }

        /// <summary>Gets output file</summary>
        public static string ChallengeOutput
        {
            get
            {
                return Path.Combine(Inputs.ResourcesFolder, "ChallengeOutput_" + DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) + ".txt");
            }
        }

        /// <summary>Gets middle size example input</summary>
        public static string ExampleInput
        {
            get
            {
                return Path.Combine(Inputs.ResourcesFolder, "ExempleInput.txt");
            }
        }

        private static string ResourcesFolder
        {
            get
            {
                if (resourcesFolder == null)
                {
                    resourcesFolder = AppDomain.CurrentDomain.BaseDirectory;

                    while (!Directory.Exists(Path.Combine(resourcesFolder, "Resources")))
                    {
                        resourcesFolder = Directory.GetParent(resourcesFolder).FullName;
                    }

                    resourcesFolder = Path.Combine(resourcesFolder, "Resources");
                }

                return resourcesFolder;
            }
        }
    }
}