using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal sealed class CommandLineUtilities
    {
        /// <returns>
        /// Dictionary containing command line arguments passed to
        /// <see cref="Environment.GetCommandLineArgs"/>.
        /// </returns>
        internal static IDictionary<string, string> GetCommandLineArguments()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            var result = new Dictionary<string, string>();

            for (var index = 0; index < commandLineArgs.Length; index++)
            {
                var arg = commandLineArgs[index];
                var key = arg.TrimStart('-');

                if (index + 1 < commandLineArgs.Length)
                {
                    var value = commandLineArgs[index + 1];
                    if (value.StartsWith("-") == false)
                    {
                        result[key] = value;
                        index++;

                        continue;
                    }
                }

                // If the next argument starts with "-", consider it as a boolean flag
                result[key] = "true";
            }

            return result;
        }
    }
}
