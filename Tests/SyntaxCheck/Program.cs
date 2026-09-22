using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

string project = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
string assets = Path.Combine(project, "Assets", "BH2VSQ_BASE");
int files = 0, errors = 0;
foreach (string path in Directory.EnumerateFiles(assets, "*.cs", SearchOption.AllDirectories))
{
    files++;
    string source = File.ReadAllText(path);
    var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp9), path);
    foreach (Diagnostic diagnostic in tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error))
    {
        Console.Error.WriteLine(diagnostic);
        errors++;
    }
    if (path.Contains(Path.DirectorySeparatorChar + "Scripts" + Path.DirectorySeparatorChar))
    {
        var root = tree.GetRoot();
        foreach (ClassDeclarationSyntax type in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            string bases = type.BaseList?.ToString() ?? "";
            if ((bases.Contains("UdonSharpBehaviour") || bases.Contains("ScriptableObject")) && type.Identifier.Text != Path.GetFileNameWithoutExtension(path))
            {
                Console.Error.WriteLine($"{path}: Unity component class {type.Identifier.Text} must match filename.");
                errors++;
            }
        }
    }
}
Console.WriteLine($"Parsed {files} project C# files; {errors} syntax/component filename errors.");
return errors == 0 ? 0 : 1;
