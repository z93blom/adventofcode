using AdventOfCode.Generator;

namespace AdventOfCode.App
{
    public class ApplicationUsage: IUsageProvider
    {
        public string Usage() => $@"
               > Usage: dotnet run -p app/app.csproj [arguments]
               > Supported arguments:

               >  [year]/[day|last|all] Solve the specified problems
               >  [year]                Solve the whole year
               >  last                  Solve the last problem
               >  all                   Solve everything

               > To start working on new problems:
               
               >  update [year]/[day]   Prepares a folder for the given day, updates the input, 
               >                        the readme and creates a solution template.
               >  update last           Same as above, but for the current day. Works in December only.  
               > ".StripMargin("> ");
    }
}
