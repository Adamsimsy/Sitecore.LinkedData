<%@ control language="C#" autoeventwireup="true" codebehind="LinkedData_SparqlQuery_Sublayout.ascx.cs" inherits="LinkedData.Website.Layouts.LinkedData_SparqlQuery_Sublayout" %>
<%@ import namespace="Sitecore.Links" %>
<h2>
    <asp:literal runat="server" id="litTitle"></asp:literal>
</h2>
<% foreach (var sitecoreTriple in SitecoreTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectNode.Item) %>"><%= sitecoreTriple.SubjectNode.Item.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectNode.Item) %>"><%= sitecoreTriple.ObjectNode.Item.Name %></a><br />
<% } %>

<asp:literal runat="server" id="litSparqlQueryResult"></asp:literal>

