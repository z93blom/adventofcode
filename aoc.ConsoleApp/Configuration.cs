using System.IO;
using System.Reflection;
using aoc.Puzzles.Core;
using Microsoft.Extensions.Configuration;

namespace aoc.ConsoleApp
{
    public sealed class Configuration
    {
        private readonly Fluent.IO.Path _applicationStartupDirectory;

        public string SessionCookie { get; set; }

        private Configuration(Fluent.IO.Path applicationStartupDirectory)
        {
            _applicationStartupDirectory = applicationStartupDirectory;
        }

        public static Configuration Load(Fluent.IO.Path applicationStartupDirectory)
        {
            IConfiguration builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true, true)
                .Build();

            var configuration = new Configuration(applicationStartupDirectory);
            builder.Bind(configuration);

            return configuration;
        }

        public Fluent.IO.Path GetDir(Moment moment)
        {
            return _applicationStartupDirectory.Combine(
                "..",
                "..",
                "..",
                "..",
                "aoc.Puzzles",
                $"Year{moment.Year:0000}");
        }

        public Fluent.IO.Path GetTestDir(Moment moment)
        {
            return _applicationStartupDirectory.Combine(
                "..",
                "..",
                "..",
                "..",
                "aoc.Puzzles.Test",
                $"Year{moment.Year:0000}");
        }



        public Fluent.IO.Path GetTemplateRoot()
        {
            return _applicationStartupDirectory.Combine("Templates");
        }
    }
}
