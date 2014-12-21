<%@ control language="C#" autoeventwireup="true" codebehind="LinkedDataControl.ascx.cs" inherits="LinkedData.Website.LinkedData.LinkedDataControl" %>
<%@ import namespace="Sitecore.Links" %>
<h2>Linked data here</h2>

<h3>All reffered triple objects</h3>
<% foreach (var sitecoreTriple in SitecoreReferredTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectItem) %>"><%= sitecoreTriple.SubjectItem.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectItem) %>"><%= sitecoreTriple.ObjectItem.Name %></a><br />
<% } %>

<h3>All refferencing triple objects</h3>
<% foreach (var sitecoreTriple in SitecoreReferringTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectItem) %>"><%= sitecoreTriple.SubjectItem.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectItem) %>"><%= sitecoreTriple.ObjectItem.Name %></a><br />
<% } %>

<h3>All linked triple objects by predicate</h3>
<% foreach (var sitecoreTriple in SitecorePredicateTriples)
   { %>
    Subject: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.SubjectItem) %>"><%= sitecoreTriple.SubjectItem.Name %></a>
    Predicate: <a href="<%= sitecoreTriple.PredicateNode.ToString() %>"><%= sitecoreTriple.PredicateNode.ToString() %></a>
    Object: <a href="<%= LinkManager.GetItemUrl(sitecoreTriple.ObjectItem) %>"><%= sitecoreTriple.ObjectItem.Name %></a><br />
<% } %>