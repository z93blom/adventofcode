using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Io;
using aoc.ConsoleApp.Model;
using aoc.Puzzles.Core;
using Path = Fluent.IO.Path;

namespace aoc.ConsoleApp
{
    internal class Updater
    {
        private readonly Configuration _configuration;

        public Updater(Configuration configuration)
        {
            _configuration = configuration;
        }

        public async Task Update(int year, int day)
        {
            var moment = new Moment(year, day);
            Console.WriteLine($"Updating {moment}");

            var baseAddress = new Uri("https://adventofcode.com/");
            var context = BrowsingContext.New(AngleSharp.Configuration.Default
                .WithDefaultLoader()
                .WithCss()
                .WithDefaultCookies()
            );

            context.SetCookie(new Url(baseAddress.ToString()), "session=" + _configuration.SessionCookie);

            //var calendar = await DownloadCalendar(context, baseAddress, year);
            var problem = await DownloadProblem(context, baseAddress, year, day);

            CreateSolutionTemplate(problem);
            CreateTestTemplate(problem);
            UpdateInput(problem);
        }

        static async Task<Calendar> DownloadCalendar(IBrowsingContext context, Uri baseUri, int year)
        {
            var document = await context.OpenAsync(baseUri.ToString() + year);
            var calendar = Calendar.Parse(year, document);
            return calendar;
        }

        static async Task<Problem> DownloadProblem(IBrowsingContext context, Uri baseUri, int year, int day)
        {
            var problemStatement = await context.OpenAsync(baseUri + $"{year}/day/{day}");
            var input = await context.GetService<IDocumentLoader>().FetchAsync(
                new DocumentRequest(new Url(baseUri + $"{year}/day/{day}/input"))).Task;
            var problem = Problem.Parse(
                year, day, baseUri + $"{year}/day/{day}", problemStatement,
                new StreamReader(input.Content).ReadToEnd());
            return problem;
        }

        void CreateSolutionTemplate(Problem problem)
        {
            CreateFileFromTemplate(
                problem,
                _configuration.GetDir(new Moment(problem.Year, problem.Day)),
                "Problem.template",
                $"Day{problem.Day:00}.cs");
        }

        void UpdateInput(Problem problem)
        {
            _configuration.GetDir(new Moment(problem.Year, problem.Day))
                .Combine("Input")
                .CreateFile($"Day{problem.Day:00}.txt", problem.Input, Encoding.UTF8);
        }

        void CreateTestTemplate(Problem problem)
        {
            CreateFileFromTemplate(
                problem, 
                _configuration.GetTestDir(new Moment(problem.Year, problem.Day)), 
                "Test.template",
                $"Day{problem.Day:00}.cs");
        }

        private void CreateFileFromTemplate(Problem problem, Path dir, string templateFileName, string fileName)
        {
            if (!dir.Exists || !dir.Files(fileName, false).Any())
            {
                var template = _configuration.GetTemplateRoot().Files(templateFileName, false).First().Read(Encoding.UTF8);

                var st = new Antlr4.StringTemplate.Template(template);
                st.Add("yearYYYY", $"{problem.Year:0000}");
                st.Add("year", problem.Year);
                st.Add("dayDD", $"{problem.Day:00}");
                st.Add("day", problem.Day);
                st.Add("title", $"{problem.Title}");

                var content = st.Render();
                dir.CreateFile(fileName, content, Encoding.UTF8);
            }
        }
    }
}