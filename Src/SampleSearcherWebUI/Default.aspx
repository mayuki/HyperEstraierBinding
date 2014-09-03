<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="HE" Namespace="HyperEstraierSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Hyper Estraier Search</title>
    <style type="text/css">
    body
    {
        font-family: "メイリオ", sans-serif;
        font-size: 95%;
    }
    em
    {
        text-decoration: none;
        font-style: normal;
        font-weight: bold;
        color: darkred;
    }
    dt
    {
        font-size: 80%;
    }
    dd
    {
        font-size: 80%;
    }
    dd.snippet
    {
        width: 50%;
    }
    dd.attr
    {
        font-family: 'Arial', sans-serif;
        font-size: 80%;
        color: #c0c0c0;
        margin-bottom: 1em;
    }
    
    #status
    {
        font-family: 'Arial', sans-serif;
        font-size: 80%;
        color: #c0c0c0;
        text-align: right;
    }
    </style>
</head>
<body>
    <form action="">
        <p>検索: <input type="text" name="SearchPhrase" value="<%= HttpUtility.HtmlEncode(searchPhrase) %>" title="単語を空白で区切ることでAND条件、&quot;&quot;でくくるとフレーズ検索、「!」をつけるとNOT検索、「|」をつけるとOR検索。"/>
            <input type="submit" name="DoSearch" value="検索" />
        </p>
    </form>
    <form id="form" runat="server">
    <div id="resultPane" runat="server" visible="false">
        <p>「<%= HttpUtility.HtmlEncode(searchPhrase) %>」が含まれた文章が <%= resultCount %> 件見つかりました。<%= (page-1) * resultCountInPage + 1 %> 件目から <%= (resultCount < (page) * resultCountInPage) ? resultCount : (page) * resultCountInPage%> 件目までを表示しています。(最大<%= resultCountInPage %>件表示)</p>
        <dl>
        <asp:Repeater ID="searchResultList" runat="server">
            <ItemTemplate>
                <dt>
                    <a href="<%# HttpUtility.HtmlEncode(((HyperEstraier.Document)Container.DataItem).Attributes["@uri"]) %>">
                        <%# HttpUtility.HtmlEncode(String.IsNullOrEmpty(((HyperEstraier.Document)Container.DataItem).Attributes["@title"]) ? ((HyperEstraier.Document)Container.DataItem).Attributes["@uri"] : ((HyperEstraier.Document)Container.DataItem).Attributes["@title"])%>
                    </a>
                </dt>
                <dd class="snippet"><%# GetHighlightedHTMLFromSnippet((HyperEstraier.Document)Container.DataItem, 200, 40, 140)%></dd>
                <dd class="attr">
                URI: <%# HttpUtility.HtmlEncode(((HyperEstraier.Document)Container.DataItem).Attributes["@uri"])%>
                | ファイルサイズ: <%# HttpUtility.HtmlEncode(String.Format("{0:#,##}", Int32.Parse(((HyperEstraier.Document)Container.DataItem).Attributes["@size"])))%> バイト
                </dd>
            </ItemTemplate>
        </asp:Repeater>
        </dl>
        <p>
        上記の検索結果以降のページ: 
        <HE:PagingContainer runat="server" ID="pageingContainer">
            <PageTemplate><a href="Default.aspx?SearchPhrase=<%= HttpUtility.HtmlEncode(searchPhrase) %>&amp;Page=<%# DataBinder.Eval(Container, "PageNumber") %>&amp;Count=<%= resultCountInPage %>"><%# DataBinder.Eval(Container, "PageNumber") %></a> | </PageTemplate>
            <CurrentPageTemplate><%# DataBinder.Eval(Container, "PageNumber") %> | </CurrentPageTemplate>
        </HE:PagingContainer>
        </p>
    </div>
    <p id="status">
        登録文章数: <asp:Label ID="lblDatabaseDocumentCount" runat="server" Text="0"></asp:Label> 個
        |
        登録語数: <asp:Label ID="lblDatabaseWordsCount" runat="server" Text="0"></asp:Label> 個
        <asp:Label ID="lblSearchTime" runat="server" Text=""></asp:Label>
    </p>
    </form>
</body>
</html>
