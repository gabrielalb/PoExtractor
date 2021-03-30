using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DataAnnotationStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string ErrorMessageAttributeName = "ErrorMessage";
        private const string DisplayAttributeName = "Display";

        public DataAnnotationStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base(metadataProvider)
        {

        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is AttributeSyntax accessor && accessor.ArgumentList != null && node.ToFullString().StartsWith("Display"))
            {
                var argument = accessor.ArgumentList.Arguments
                    .Where(a => a.Expression.Parent.ToFullString().StartsWith("Name=") || a.Expression.Parent.ToFullString().StartsWith("Name ="))
                    .FirstOrDefault();
                if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    result = new LocalizableStringOccurence()
                    {
                        Text = literal.Token.ValueText,
                        Context = MetadataProvider.GetContext(node),
                        Location = MetadataProvider.GetLocation(node)
                    };

                    return true;
                }
            }

            return false;
        }
    }
}
