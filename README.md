
# Advent of Code (2015-2019)
My C# solutions to the advent of code problems. (My solutions are in the "solutions" branch. The master branch contains the scaffolding).
Check out http://adventofcode.com.

## Dependencies

- This library is built for `.Net Standard 2.0`. It should work on Windows, Linux and OS X.
- `Newtonsoft.Json` for JSON parsing
- `HtmlAgilityPack` is used for problem download.

## Setting the session variable
The application depends on an environment variable. To get your identifier, you need to
look at the request headers for your daily data, and take the session identifier from
the cookie in the Request Headers (for example https://adventofcode.com/2018/day/1/input)

Then you can set it:

```
Powershell: $Env:SESSION = "..."

Command Prompt: set SESSION = ...
```

## Running

To run the project:

1. Install .NET Core
2. Clone the repo
3. Get help with `dotnet run`
```

Usage: dotnet run -p app/app.csproj [arguments]
Supported arguments:

 [year]/[day|last|all] Solve the specified problems
 [year]                Solve the whole year
 last                  Solve the last problem
 all                   Solve everything

To start working on new problems:
               
 update [year]/[day]   Prepares a folder for the given day, updates the input, 
                       the readme and creates a solution template.
 update last           Same as above, but for the current day. Works in December only.  

```
