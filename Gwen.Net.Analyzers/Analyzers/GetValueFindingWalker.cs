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
        var visitor = new GetValueFindingWalker(semanticModel);
        
        visitor.Visit(expression);

        return visitor.found;
    }
    
    public override void VisitInvocationExpression(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        base.VisitInvocationExpression(invocationExpressionSyntax);

        if (found != null)
            return;

        String? methodName = invocationExpressionSyntax.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.Text,
            MemberAccessExpressionSyntax ma => ma.Name.Identifier.Text,
            _ => null
        };

        if (methodName != "GetValue")
            return;

        if (semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol)
            return;

        if (methodSymbol is not {Name: "GetValue", Parameters.Length: 0})
            return;

        if (methodSymbol.ContainingType is not {} containingType)
            return;

        if (!containingType.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() == "Gwen.Net.New.Bindings.IValueSource<T>"))
            return; // todo: remove New from namespace when deleting Legacy

        found = invocationExpressionSyntax.GetLocation();
    }
}
