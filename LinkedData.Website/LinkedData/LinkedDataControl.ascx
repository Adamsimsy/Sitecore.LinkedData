<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkedDataControl.ascx.cs" Inherits="LinkedData.Website.LinkedData.LinkedDataControl" %>
<%@ Import Namespace="Sitecore.Links" %>
<h2>Linked data here</h2>
<h3>All linked triple objects</h3>
<% foreach (var sitecoreTriple in SitecoreTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectItem) %>"><%= sitecoreTriple.SubjectItem.Name %></a>
Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectItem) %>"><%= sitecoreTriple.ObjectItem.Name %></a><br />      

   <% } %>

<h3>All linked triple objects by predicate</h3>
<asp:Literal runat="server" ID="litRdf2"></asp:Literal>