using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class ErrorMessageAnnotationStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string ErrorMessageAttributeName = "ErrorMessage";
        //private const string DisplayAttributeName = "Display";

        public ErrorMessageAnnotationStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base(metadataProvider)
        {

        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is AttributeArgumentSyntax argument
                && argument.Expression.Parent.ToFullString().StartsWith(ErrorMessageAttributeName)
                && node.Parent?.Parent is AttributeSyntax accessor
                //&& accessor.Name.ToString() == DisplayAttributeName
                && argument.Expression is LiteralExpressionSyntax literal
                && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                result = new LocalizableStringOccurence()
                {
                    Text = literal.Token.ValueText,
                    Context = MetadataProvider.GetContext(node),
                    Location = MetadataProvider.GetLocation(node)
                };

                return true;
            }

            return false;
        }
    }
}
