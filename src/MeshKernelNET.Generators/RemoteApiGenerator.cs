using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MeshKernelNET.Generators;

[Generator]
public class RemoteApiGenerator : IIncrementalGenerator
{
    private const string interfaceName = "IMeshKernelApi";
    private const string implementationName = "RemoteMeshKernelApi";

    private static readonly SymbolDisplayFormat typeNameOnlyFormat = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var interfaceDeclaration = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => node is InterfaceDeclarationSyntax { Identifier.Text: interfaceName },
            transform: (ctx, _) => (InterfaceDeclaration: (InterfaceDeclarationSyntax)ctx.Node, ctx.SemanticModel)
        );

        context.RegisterSourceOutput(interfaceDeclaration, (spc, data) =>
        {
            var (interfaceSyntax, semanticModel) = data;

            if (semanticModel.GetDeclaredSymbol(interfaceSyntax) is not INamedTypeSymbol typeSymbol)
                return;

            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {typeSymbol.ContainingNamespace}");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Remote wrapper for <see cref=\"{interfaceName}\"/> that executes operations in a separate process.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    public partial class {implementationName}");
            sb.AppendLine("    {");

            foreach (IMethodSymbol method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.MethodKind != MethodKind.Ordinary)
                    continue;

                string methodName = method.Name;
                string returnType = method.ReturnType.ToDisplayString(typeNameOnlyFormat);

                string parameters = string.Join(", ", method.Parameters.Select(p =>
                    $"{GetRefModifier(p.RefKind)}{p.Type.ToDisplayString(typeNameOnlyFormat)} {p.Name}"));

                string arguments = string.Join(", ", method.Parameters.Select(p =>
                    $"{GetRefModifier(p.RefKind)}{p.Name}"));

                sb.AppendLine("        /// <inheritdoc/>");
                sb.AppendLine($"        public {returnType} {methodName}({parameters}) => api.{methodName}({arguments});");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource($"{implementationName}.g.cs", sb.ToString());
        });
    }

    private static string GetRefModifier(RefKind refKind) =>
        refKind == RefKind.None ? "" : refKind.ToString().ToLower() + " ";
}
