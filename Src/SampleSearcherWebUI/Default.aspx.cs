using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using HyperEstraier;
using System.Text;
using System.IO;

public partial class _Default : System.Web.UI.Page 
{
    private Database _database;
    protected String searchPhrase;
    protected Int32 resultCount;
    protected Int32 resultCountInPage;
    protected Int32 page;

    protected void Page_Load(object sender, EventArgs e)
    {
        _database = Context.Application["HyperEstraier.DatabaseInstance"] as Database;

        lblDatabaseDocumentCount.Text = _database.DocumentNumber.ToString("#,##");
        lblDatabaseWordsCount.Text = _database.WordNumber.ToString("#,##");

        // ページ内の数はデフォルト10
        if (!Int32.TryParse(Request.Params["Count"], out resultCountInPage))
            resultCountInPage = 10;

        if (!Int32.TryParse(Request.Params["Page"], out page))
            page = 1;
        if (page < 1)
            page = 1;

        if (!String.IsNullOrEmpty(Request.Params["SearchPhrase"]))
        {
            searchPhrase = Request.Params["SearchPhrase"];
            resultPane.Visible = true;
            ExecuteSearch();
        }
    }

    /// <summary>
    /// 検索とデータバインドを実行します。
    /// </summary>
    private void ExecuteSearch()
    {
        using (Condition cond = new Condition())
        {
            cond.Phrase = searchPhrase;
            //cond.Max = 10;
            cond.Options = ConditionOptions.Simple;
            cond.Order = OrderOperators.NumberAscend;

            DateTime startTime = DateTime.Now;
            using (Result result = _database.Search(cond))
            {
                lblSearchTime.Text = String.Format(" | 検索にかかった時間: {0} 秒", ((Double)((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds / 1000));
                resultCount = result.DocumentNumber;

                List<Document> results = new List<Document>();
                Int32 docCount = 0;
                Int32 start = (page - 1) * resultCountInPage;
                for (Int32 i = start; i < start + resultCountInPage && i < result.DocumentNumber; i++)
                {
                    Document doc = _database.GetDocument(result.GetDocumentId(i));
                    results.Add(doc);
                    docCount++;
                }

                startTime = DateTime.Now;
                searchResultList.DataSource = results;
                searchResultList.DataBind();
                lblSearchTime.Text += String.Format(" | データバインドにかかった時間: {0} 秒", ((Double)((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds / 1000));

                // Release Unmanaged Resources
                foreach (Document doc in results)
                    doc.Dispose();
            }
        }
        pageingContainer.TotalPages = (resultCount / resultCountInPage)+1;
        pageingContainer.CurrentPage = page;

        Page.Title += String.Format(": {0}", searchPhrase);
        //searchTimePanel.Visible = true;
    }

    /// <summary>
    /// 文章のスニペットをハイライトしてHTMLとして返します。
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="wwidth"></param>
    /// <param name="hwidth"></param>
    /// <param name="awidth"></param>
    /// <returns></returns>
    protected String GetHighlightedHTMLFromSnippet(Document doc, Int32 wwidth, Int32 hwidth, Int32 awidth)
    {
        String snippet = doc.MakeSnippet(searchPhrase.Split(new Char[] { ' ', '　' }), wwidth, hwidth, awidth);
        StringBuilder sb = new StringBuilder();
        foreach (String line in snippet.Split(new Char[] { '\n' }))
        {
            if (line.Length == 0)
            {
                sb.Append("...");
                continue;
            }

            String[] fields = line.Split(new Char[] { '\t' });
            if (fields.Length == 2)
            {
                sb.Append("<em>");
            }
            sb.Append(HttpUtility.HtmlEncode(fields[0]));
            if (fields.Length == 2)
            {
                sb.Append("</em>");
            }
        }

        return sb.ToString();
    }
}
