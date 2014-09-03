using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Pagenate の概要の説明です
/// </summary>
namespace HyperEstraierSample
{
    public class PagingContainer : WebControl, INamingContainer
    {
        private ITemplate _template;
        private ITemplate _currentTemplate;

        [TemplateContainer(typeof(PageContainer))]
        public ITemplate PageTemplate
        {
            set { _template = value; }
            get { return _template; }
        }

        [TemplateContainer(typeof(PageContainer))]
        public ITemplate CurrentPageTemplate
        {
            set { _currentTemplate = value; }
            get { return _currentTemplate; }
        }

        private Int32 _currentPage;
        public Int32 CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        private Int32 _totalPages;
        public Int32 TotalPages
        {
            get { return _totalPages; }
            set { _totalPages = value; }
        }

        protected override void CreateChildControls()
        {
            for (Int32 i = 0; i < _totalPages; i++)
            {
                PageContainer pageContainer = new PageContainer();
                pageContainer.TotalPages = _totalPages;
                pageContainer.CurrentPage = _currentPage;
                pageContainer.PageNumber = i + 1;
                Controls.Add(pageContainer);

                if (_currentPage == (i + 1))
                {
                    _currentTemplate.InstantiateIn(pageContainer);
                }
                else
                {
                    _template.InstantiateIn(pageContainer);
                }
            }

            DataBind();
        }
    }
    public class PageContainer : WebControl, INamingContainer
    {
        private Int32 _page;
        public Int32 PageNumber
        {
            get { return _page; }
            set { _page = value; }
        }

        private Int32 _currentPage;
        public Int32 CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        private Int32 _totalPages;
        public Int32 TotalPages
        {
            get { return _totalPages; }
            set { _totalPages = value; }
        }
    }
}
