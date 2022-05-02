using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

internal static class Program
{
    private static void Main(string[] args)
    {
        // --force, -f
        // --out <file>
        // <input> 1..N
        // -h, -?, --help

        var forceOption = new Option<bool>(new[] { "--force", "-f" }, "Forces the execution.");
        var excludeOption = new Option<bool>(new[] { "--exclude", "-x" }, "Excludes something important.");
        var outOption = new Option<string>(new[] { "--out", "-o" }, "Output filename.");
        outOption.ArgumentHelpName = "path";

        outOption.IsRequired = true;
        outOption.LegalFilePathsOnly();

        var propertyOption = new Option<KeyValuePair<string, string>[]>(new[] { "-p", "/p" },
            description: "Property.",
            parseArgument: p =>
            {
                var result = new List<KeyValuePair<string, string>>();

                foreach (var t in p.Tokens)
                {
                    var parts = t.Value.Split("=");
                    if (parts.Length != 2)
                    {
                        p.ErrorMessage = $"Invalid key value pair '{t.Value}'";
                    }
                    else
                    {
                        var key = parts[0];
                        var value = parts[1];
                        var kv = KeyValuePair.Create(key, value);
                        result.Add(kv);
                    }
                }

                return result.ToArray();
            });

        var inputArguments = new Argument<FileInfo[]>("input").ExistingOnly();
        inputArguments.Arity = ArgumentArity.OneOrMore;

        var testArgument = new Argument<int?>("test");
        //testArgument.Arity = ArgumentArity.ZeroOrOne;

        var c = new RootCommand();

        var runCommand = new Command("run")
        {
            forceOption,
            excludeOption,
            outOption,
            propertyOption,
            inputArguments,
            testArgument
        };

        c.AddCommand(runCommand);

        c.SetHandler((ParseResult pr) =>
        {
            Console.WriteLine("This is the root");
        });

        runCommand.Handler = CommandHandler.Create(Run);

        //runCommand.SetHandler<bool, FileInfo[], string>(Run, forceOption, inputArguments, outOption);
        // runCommand.SetHandler((ParseResult result) =>
        // {
        //     var force = result.GetValueForOption(forceOption);
        //     Console.WriteLine($"--force : {force}");
        // 
        //     var o = result.GetValueForOption(outOption);
        //     Console.WriteLine($"--out   : {o}");
        // 
        //     var inputFiles = result.GetValueForArgument(inputArguments);
        //     foreach (var inputFile in inputFiles)
        //         Console.WriteLine($"--in    : {inputFile}");
        // 
        //     var test = result.GetValueForArgument(testArgument);
        //     Console.WriteLine($"--test  : {test}");
        // 
        //     var ps = result.GetValueForOption(propertyOption) ??
        //     Array.Empty<KeyValuePair<string, string>>();
        //     foreach (var p in ps)
        //         Console.WriteLine($"/p      : {p}");
        // });
        c.Invoke(args);

        static void Run(RunCommandOptions o)
        {
            Console.WriteLine($"force   : {o.Force}");
            Console.WriteLine($"exclude : {o.Exclude}");
            Console.WriteLine($"out     : {o.Out}");
        }
    }

    class RunCommandOptions
    {
        public bool Force { get; set; }
        public bool Exclude { get; set; }
        public FileInfo[] Input { get; set; }
        public string Out { get; set; }
    }
}
