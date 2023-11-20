using GSharp;

public class WallE
{
  private static readonly ConsoleLogger logger = new ConsoleLogger();
  private string[] args;

  public static Task Main(string[] args)
  {
    return new WallE(args).RunInternalAsync();
  }

  public WallE(string[] args)
  {
    this.args = args;
  }

  private async Task RunInternalAsync()
  {
    if (args.Length == 0)
    {
      await RunPromptAsync();
    }
    else
    {
      await RunFileAsync(args[0]);
    }
  }

  private async Task RunFileAsync(string filePath)
  {
    var source = await File.ReadAllTextAsync(filePath);

    await RunAsync(source);

    if (logger.hadError)
    {
      Environment.Exit(55);
    }

    if (logger.hadRuntimeError)
    {
      Environment.Exit(70);
    }
  }

  private Task RunPromptAsync()
  {
    while (true)
    {
      Console.Write("> ");
      string? line = Console.ReadLine();
      if (line == null) break;
      RunAsync(line);
    }

    return Task.CompletedTask;
  }

  private Task RunAsync(string source)
  {
    var scanner = new Scanner(logger, source);
    var tokens = scanner.ScanTokens();

    foreach (var item in tokens)
    {
      System.Console.WriteLine(item);
    }

    var parser = new Parser(logger, tokens);
    var stmts = parser.Parse();

    foreach (var item in stmts)
    {
      System.Console.WriteLine(item);
    }

    return Task.CompletedTask;
  }
}