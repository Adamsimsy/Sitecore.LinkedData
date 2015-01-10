<%@ control language="C#" autoeventwireup="true" codebehind="LinkedData_AllTriples_Sublayout.ascx.cs" inherits="LinkedData.Website.Layouts.LinkedData_AllTriples_Sublayout" %>
<%@ import namespace="Sitecore.Links" %>
<h2>All triple data for debug</h2>

<h3>All reffered triple objects</h3>
<% foreach (var sitecoreTriple in SitecoreReferredTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectNode.Item) %>"><%= sitecoreTriple.SubjectNode.Item.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectNode.Item) %>"><%= sitecoreTriple.ObjectNode.Item.Name %></a><br />
<% } %>

<h3>All refferencing triple objects</h3>
<% foreach (var sitecoreTriple in SitecoreReferringTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectNode.Item) %>"><%= sitecoreTriple.SubjectNode.Item.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectNode.Item) %>"><%= sitecoreTriple.ObjectNode.Item.Name %></a><br />
<% } %>

<h3>All linked triple objects by predicate</h3>
<% foreach (var sitecoreTriple in SitecorePredicateTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectNode.Item) %>"><%= sitecoreTriple.SubjectNode.Item.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectNode.Item) %>"><%= sitecoreTriple.ObjectNode.Item.Name %></a><br />
<% } %>