<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayersInLeague.ascx.cs" Inherits="LinkedData.Website.LinkedData.PlayersInLeague" %>
<%@ Import Namespace="Sitecore.Links" %>
<h2>Players in League</h2>
<asp:Literal runat="server" ID="litRdf"></asp:Literal>

<% foreach (var sitecoreTriple in SitecoreTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectNode.Item) %>"><%= sitecoreTriple.SubjectNode.Item.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectNode.Item) %>"><%= sitecoreTriple.ObjectNode.Item.Name %></a><br />
<% } %>