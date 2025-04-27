using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace zachjcomer.InterfaceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IA001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Avoid exposing implementations outside the assembly",
            messageFormat: "Class '{0}' should be marked internal",
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            var isPublic = classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword);
            var isAbstract = classDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword);
            var isStatic = classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);

            if (isPublic && !isAbstract && !isStatic)
            {
                var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
