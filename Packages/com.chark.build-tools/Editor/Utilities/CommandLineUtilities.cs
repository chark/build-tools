using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class CommandLineUtilities
    {
        internal sealed class CommandLineArguments
        {
            private readonly IDictionary<string, string> arguments;

            public CommandLineArguments(IDictionary<string, string> arguments)
            {
                this.arguments = arguments;
            }

            public bool TryGetValue(string key, out string value)
            {
                key.AssertNotBlank(nameof(key));

                var normalizedKey = key.Trim().ToLower();
                return arguments.TryGetValue(normalizedKey, out value);
            }
        }

        /// <returns>
        /// Dictionary containing command line arguments passed to
        /// <see cref="Environment.GetCommandLineArgs"/>.
        /// </returns>
        internal static CommandLineArguments GetCommandLineArguments()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            var result = new Dictionary<string, string>();

            for (var index = 0; index < commandLineArgs.Length; index++)
            {
                var arg = commandLineArgs[index];
                var key = arg.TrimStart('-').Trim().ToLower();

                if (index + 1 < commandLineArgs.Length)
                {
                    var value = commandLineArgs[index + 1].Trim();
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

            return new CommandLineArguments(result);
        }
    }
}
