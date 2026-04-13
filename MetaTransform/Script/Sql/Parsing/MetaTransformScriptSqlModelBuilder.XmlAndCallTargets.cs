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
                XmlNamespacesId = xmlNamespaces.Id,
                XmlNamespacesElementId = xmlNamespaceElements[ordinal].GetId(nameof(XmlNamespacesElement)),
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
            XmlNamespacesElementId = xmlNamespacesElement.Id,
            StringLiteralId = stringLiteral.GetId(nameof(StringLiteral))
        });

        if (aliasIdentifier is null)
        {
            return BuiltNode.Create((nameof(XmlNamespacesElement), xmlNamespacesElement.Id));
        }

        var aliasElement = new XmlNamespacesAliasElement
        {
            Id = NextId(nameof(XmlNamespacesAliasElement)),
            XmlNamespacesElementId = xmlNamespacesElement.Id
        };
        model.XmlNamespacesAliasElementList.Add(aliasElement);
        model.XmlNamespacesAliasElementIdentifierLinkList.Add(new XmlNamespacesAliasElementIdentifierLink
        {
            Id = NextId(nameof(XmlNamespacesAliasElementIdentifierLink)),
            XmlNamespacesAliasElementId = aliasElement.Id,
            IdentifierId = aliasIdentifier.GetId(nameof(Identifier))
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
            XmlNamespacesElementId = xmlNamespacesElement.Id,
            StringLiteralId = stringLiteral.GetId(nameof(StringLiteral))
        });

        var defaultElement = new XmlNamespacesDefaultElement
        {
            Id = NextId(nameof(XmlNamespacesDefaultElement)),
            XmlNamespacesElementId = xmlNamespacesElement.Id
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
            CallTargetId = callTarget.Id
        };
        model.MultiPartIdentifierCallTargetList.Add(multiPartIdentifierCallTarget);
        model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList.Add(new MultiPartIdentifierCallTargetMultiPartIdentifierLink
        {
            Id = NextId(nameof(MultiPartIdentifierCallTargetMultiPartIdentifierLink)),
            MultiPartIdentifierCallTargetId = multiPartIdentifierCallTarget.Id,
            MultiPartIdentifierId = multiPartIdentifier.GetId(nameof(MultiPartIdentifier))
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
            FunctionCallId = functionCall.GetId(nameof(FunctionCall)),
            CallTargetId = callTarget.GetId(nameof(CallTarget))
        });

        return functionCall;
    }
}
