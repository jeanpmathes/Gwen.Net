using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gwen.Net.Analyzers.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BindingAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor rule = new(
        "GWEN0001",
        "All value sources in bindings must be correctly referenced",
        "All value sources in bindings must be correctly referenced, GetValue() in a binding indicates an error",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        "Creating a binding from value sources will not subscribe to changes of the sources except when the sources are explicitly passed to binding creation.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [rule];
    
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocationSyntax)
            return;

        if (context.SemanticModel.GetSymbolInfo(invocationSyntax).Symbol is not IMethodSymbol methodSymbol)
            return;

        if (!IsContainingTypeBindingType(methodSymbol) && !IsReceiverBindingType(invocationSyntax, context.SemanticModel))
            return; // todo: remove New from namespace when deleting Legacy

        foreach (ArgumentSyntax argument in invocationSyntax.ArgumentList.Arguments)
        {
            if (GetValueFindingWalker.ContainsGetValueInvocation(argument.Expression, context.SemanticModel) is not {} location)
                continue;

            context.ReportDiagnostic(Diagnostic.Create(rule, location));
        }
    }
    
    private static Boolean IsContainingTypeBindingType(IMethodSymbol methodSymbol)
    {
        return IsBindingType(methodSymbol.ContainingType);
    }
    
    private static Boolean IsReceiverBindingType(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return false;

        ITypeSymbol? receiverType = semanticModel.GetTypeInfo(memberAccess.Expression).Type;
        
        return IsBindingType(receiverType);
    }
    
    private static Boolean IsBindingType(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null)
            return false;
        
        return typeSymbol.OriginalDefinition.ToDisplayString() == "Gwen.Net.New.Bindings.Binding<T>"
               || typeSymbol.OriginalDefinition.ToDisplayString() == "Gwen.Net.New.Bindings.Binding";
    }
}
