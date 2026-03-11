using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gwen.Net.Analyzers.Analyzers;

public class GetValueFindingWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;

    private Location? found;

    private GetValueFindingWalker(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    public static Location? ContainsGetValueInvocation(ExpressionSyntax expression, SemanticModel semanticModel)
    {
        GetValueFindingWalker visitor = new(semanticModel);

        visitor.Visit(expression);

        return visitor.found;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        base.VisitInvocationExpression(invocationExpressionSyntax);

        if (found != null)
            return;

        if (IsGetValueInvocation(invocationExpressionSyntax, semanticModel) || IsGetValue2Invocation(invocationExpressionSyntax, semanticModel))
            found = invocationExpressionSyntax.GetLocation();
    }

    private static Boolean IsGetValueInvocation(InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel)
    {
        if (IsGetValueInvocationBase(invocationExpressionSyntax, semanticModel) is not {} methodSymbol)
            return false;

        if (methodSymbol is not {Name: "GetValue", Parameters.Length: 0})
            return false;

        return methodSymbol.ContainingType is {} containingType
               && IsOrImplementsInterface(containingType, "Gwen.Net.New.Bindings.IValueSource<T>"); // todo: remove New from namespace when deleting Legacy
    }

    private static Boolean IsGetValue2Invocation(InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel)
    {
        if (IsGetValueInvocationBase(invocationExpressionSyntax, semanticModel) is not {} methodSymbol)
            return false;

        if (methodSymbol is not {Name: "GetValue", Parameters.Length: 1})
            return false;

        return methodSymbol.ContainingType is {} containingType
               && IsOrImplementsInterface(containingType, "Gwen.Net.New.Bindings.IValueSource<TIn, TOut>"); // todo: remove New from namespace when deleting Legacy
    }

    private static Boolean IsOrImplementsInterface(ITypeSymbol typeSymbol, String interfaceDisplayName)
    {
        return typeSymbol.OriginalDefinition.ToDisplayString() == interfaceDisplayName
               || typeSymbol.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() == interfaceDisplayName);
    }

    private static IMethodSymbol? IsGetValueInvocationBase(InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel)
    {
        String? methodName = invocationExpressionSyntax.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.Text,
            MemberAccessExpressionSyntax ma => ma.Name.Identifier.Text,
            _ => null
        };

        if (methodName != "GetValue")
            return null;

        return semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
    }
}
