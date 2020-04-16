using FacebookPageSdk.Entities.Concrete;
using HtmlAgilityPack;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace FacebookPageSdk.Services
{
    public class FacebookPageService
    {
        public HtmlNode FacebookPage;
        public FacebookPageService(string src)
        {
            FacebookPage = stringToHtmlNode(src);
        }
        public HtmlNode stringToHtmlNode(string src)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(src);
            return html.DocumentNode;
        }
        public HtmlNodeCollection getPostsNodes(HtmlNode html = null)
        {
            if (html == null)
                html = FacebookPage;
            return html.SelectNodes("//*[@class=\"_3drp\"]"); ;
        }
        public IEnumerable<Post> GetPosts()
        {
            var postNodes = getPostsNodes();
            if (postNodes != null)
                foreach (var post in postNodes)
                {
                    var description = post.SelectSingleNode("article/div/div[1]/span/p");
                    var date = post.SelectSingleNode("article/div/header/div[2]/div/div/div[1]/div/a/abbr");
                    var title = post.SelectSingleNode("article/div/div[2]/section/section/div/div/header/h3/span/span");
                    var likeCount = post.SelectSingleNode("article/footer/div/div[1]/div[1]/a/div/div[1]/div");
                    var commentCount = post.SelectSingleNode("article/footer/div/div[1]/div[1]/a/div/div[2]/span[1]");
                    var shareCount = post.SelectSingleNode("article/footer/div/div[1]/div[1]/a/div/div[2]/span[2]");
                    var detailPageLink = post.SelectSingleNode("article/footer/div/div[1]/div[1]/a");
                    yield return new Post()
                    {
                        Description = description?.InnerText,
                        Date = date?.InnerText,
                        Title = title?.InnerText,
                        LikeCount=likeCount?.InnerText,
                        CommentCount=commentCount?.InnerText,
                        ShareCount= shareCount?.InnerText,
                        DetailPageLink= detailPageLink?.GetAttributeValue("href",string.Empty).Replace("amp;",""),
                    };
                }

        }
    }
}
