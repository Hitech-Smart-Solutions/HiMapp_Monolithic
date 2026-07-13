using System.Xml.Linq;

var repoRoot = LocateRepoRoot();
var projectFiles = Directory.GetFiles(repoRoot, "*.csproj", SearchOption.AllDirectories);
var violations = new List<string>();

foreach (var projectFile in projectFiles)
{
    var relativeProject = Path.GetRelativePath(repoRoot, projectFile);
    if (!relativeProject.StartsWith(Path.Combine("src", "Modules"), StringComparison.OrdinalIgnoreCase))
    {
        continue;
    }

    var currentModule = relativeProject.Split(Path.DirectorySeparatorChar)[2];
    var document = XDocument.Load(projectFile);
    var references = document.Descendants("ProjectReference")
        .Select(reference => reference.Attribute("Include")?.Value)
        .Where(include => !string.IsNullOrWhiteSpace(include))
        .Select(include => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFile)!, include!)));

    foreach (var reference in references)
    {
        var relativeReference = Path.GetRelativePath(repoRoot, reference);
        if (!relativeReference.StartsWith(Path.Combine("src", "Modules"), StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        var parts = relativeReference.Split(Path.DirectorySeparatorChar);
        var referencedModule = parts[2];
        var referencedProject = Path.GetFileNameWithoutExtension(reference);

        if (!string.Equals(currentModule, referencedModule, StringComparison.OrdinalIgnoreCase)
            && !referencedProject.EndsWith(".Contracts", StringComparison.OrdinalIgnoreCase))
        {
            violations.Add($"{relativeProject} references forbidden module internals: {relativeReference}");
        }
    }
}

if (violations.Count > 0)
{
    Console.Error.WriteLine("Architecture boundary violations:");
    foreach (var violation in violations)
    {
        Console.Error.WriteLine($" - {violation}");
    }

    return 1;
}

Console.WriteLine("Architecture boundary rules passed.");
return 0;

static string LocateRepoRoot()
{
    var directory = AppContext.BaseDirectory;
    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory, "HiMapp_Monolithic.csproj")))
        {
            return directory;
        }

        directory = Directory.GetParent(directory)?.FullName;
    }

    throw new DirectoryNotFoundException("Could not locate repository root.");
}
