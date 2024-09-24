using System.Xml;

namespace Xmlvalidator;

public class XmlNamespaceManagerBuilder
{
    private readonly XmlNamespaceManager _namespaceManager;

    public XmlNamespaceManagerBuilder()
    {
        _namespaceManager = new XmlNamespaceManager(new NameTable());
    }

    public XmlNamespaceManagerBuilder AddNamespace(string prefix, string uri)
    {
        _namespaceManager.AddNamespace(prefix, uri);
        return this;
    }

    public XmlNamespaceManager Build()
    {
        return _namespaceManager;
    }
}