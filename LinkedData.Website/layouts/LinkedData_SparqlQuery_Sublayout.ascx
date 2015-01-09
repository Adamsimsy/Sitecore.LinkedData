<%@ control language="C#" autoeventwireup="true" codebehind="LinkedData_SparqlQuery_Sublayout.ascx.cs" inherits="LinkedData.Website.Layouts.LinkedData_SparqlQuery_Sublayout" %>
<%@ Import Namespace="Sitecore.Links" %>
<h2>
    <asp:literal runat="server" id="litTitle"></asp:literal>
</h2>
<% foreach (var sitecoreTriple in SitecoreTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectItem) %>"><%= sitecoreTriple.SubjectItem.Name %></a>
Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectItem) %>"><%= sitecoreTriple.ObjectItem.Name %></a><br />

<% } %>

