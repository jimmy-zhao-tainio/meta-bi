using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateXmlNamespaces(IReadOnlyList<BuiltNode> xmlNamespaceElements)
    {
        var xmlNamespaces = new XmlNamespaces
        {
            Id = NextId(nameof(XmlNamespaces))
        };
        model.XmlNamespacesList.Add(xmlNamespaces);

        for (var ordinal = 0; ordinal < xmlNamespaceElements.Count; ordinal++)
        {
            model.XmlNamespacesXmlNamespacesElementsItemList.Add(new XmlNamespacesXmlNamespacesElementsItem
            {
                Id = NextId(nameof(XmlNamespacesXmlNamespacesElementsItem)),
                XmlNamespaces = xmlNamespaces,
                XmlNamespacesElement = xmlNamespaceElements[ordinal].GetRef<XmlNamespacesElement>(nameof(XmlNamespacesElement)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(XmlNamespaces), xmlNamespaces.Id));
    }

    public BuiltNode CreateXmlNamespacesElement(BuiltNode stringLiteral, BuiltNode? aliasIdentifier = null)
    {
        var xmlNamespacesElement = new XmlNamespacesElement
        {
            Id = NextId(nameof(XmlNamespacesElement))
        };
        model.XmlNamespacesElementList.Add(xmlNamespacesElement);
        model.XmlNamespacesElementStringLinkList.Add(new XmlNamespacesElementStringLink
        {
            Id = NextId(nameof(XmlNamespacesElementStringLink)),
            XmlNamespacesElement = xmlNamespacesElement,
            StringLiteral = stringLiteral.GetRef<StringLiteral>(nameof(StringLiteral))
        });

        if (aliasIdentifier is null)
        {
            return BuiltNode.Create((nameof(XmlNamespacesElement), xmlNamespacesElement.Id));
        }

        var aliasElement = new XmlNamespacesAliasElement
        {
            Id = NextId(nameof(XmlNamespacesAliasElement)),
            XmlNamespacesElement = xmlNamespacesElement
        };
        model.XmlNamespacesAliasElementList.Add(aliasElement);
        model.XmlNamespacesAliasElementIdentifierLinkList.Add(new XmlNamespacesAliasElementIdentifierLink
        {
            Id = NextId(nameof(XmlNamespacesAliasElementIdentifierLink)),
            XmlNamespacesAliasElement = aliasElement,
            Identifier = aliasIdentifier.GetRef<Identifier>(nameof(Identifier))
        });

        return BuiltNode.Create(
            (nameof(XmlNamespacesElement), xmlNamespacesElement.Id),
            (nameof(XmlNamespacesAliasElement), aliasElement.Id));
    }

    public BuiltNode CreateXmlNamespacesDefaultElement(BuiltNode stringLiteral)
    {
        var xmlNamespacesElement = new XmlNamespacesElement
        {
            Id = NextId(nameof(XmlNamespacesElement))
        };
        model.XmlNamespacesElementList.Add(xmlNamespacesElement);
        model.XmlNamespacesElementStringLinkList.Add(new XmlNamespacesElementStringLink
        {
            Id = NextId(nameof(XmlNamespacesElementStringLink)),
            XmlNamespacesElement = xmlNamespacesElement,
            StringLiteral = stringLiteral.GetRef<StringLiteral>(nameof(StringLiteral))
        });

        var defaultElement = new XmlNamespacesDefaultElement
        {
            Id = NextId(nameof(XmlNamespacesDefaultElement)),
            XmlNamespacesElement = xmlNamespacesElement
        };
        model.XmlNamespacesDefaultElementList.Add(defaultElement);

        return BuiltNode.Create(
            (nameof(XmlNamespacesElement), xmlNamespacesElement.Id),
            (nameof(XmlNamespacesDefaultElement), defaultElement.Id));
    }

    public BuiltNode CreateMultiPartIdentifierCallTarget(BuiltNode multiPartIdentifier)
    {
        var callTarget = new CallTarget
        {
            Id = NextId(nameof(CallTarget))
        };
        model.CallTargetList.Add(callTarget);

        var multiPartIdentifierCallTarget = new MultiPartIdentifierCallTarget
        {
            Id = NextId(nameof(MultiPartIdentifierCallTarget)),
            CallTarget = callTarget
        };
        model.MultiPartIdentifierCallTargetList.Add(multiPartIdentifierCallTarget);
        model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList.Add(new MultiPartIdentifierCallTargetMultiPartIdentifierLink
        {
            Id = NextId(nameof(MultiPartIdentifierCallTargetMultiPartIdentifierLink)),
            MultiPartIdentifierCallTarget = multiPartIdentifierCallTarget,
            MultiPartIdentifier = multiPartIdentifier.GetRef<MultiPartIdentifier>(nameof(MultiPartIdentifier))
        });

        return BuiltNode.Create(
            (nameof(CallTarget), callTarget.Id),
            (nameof(MultiPartIdentifierCallTarget), multiPartIdentifierCallTarget.Id));
    }

    public BuiltNode AttachFunctionCallCallTarget(BuiltNode functionCall, BuiltNode callTarget)
    {
        model.FunctionCallCallTargetLinkList.Add(new FunctionCallCallTargetLink
        {
            Id = NextId(nameof(FunctionCallCallTargetLink)),
            FunctionCall = functionCall.GetRef<FunctionCall>(nameof(FunctionCall)),
            CallTarget = callTarget.GetRef<CallTarget>(nameof(CallTarget))
        });

        return functionCall;
    }
}
