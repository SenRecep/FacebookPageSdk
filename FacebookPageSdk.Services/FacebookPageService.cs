﻿using FacebookPageSdk.Entities.Concrete;
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
                    string countXPath = "article/footer/div/div[1]/a";
                    if (post.SelectSingleNode("article/footer/div/div[1]/div[1]/a") != null)
                        countXPath = "article/footer/div/div[1]/div[1]/a";
                    var likeCount = post.SelectSingleNode($"{countXPath}/div/div[1]/div");
                    var commentCount = post.SelectSingleNode($"{countXPath}/div/div[2]/span[1]");
                    var shareCount = post.SelectSingleNode($"{countXPath}/div/div[2]/span[2]");
                    var detailPageLink = post.SelectSingleNode($"{countXPath}");
                    yield return new Post()
                    {
                        Description = description == null ? "" : description.InnerText,
                        Date = date == null ? "" : date.InnerText,
                        Title = title == null ? "" : title.InnerText,
                        LikeCount = likeCount == null ? "" : likeCount.InnerText,
                        CommentCount = commentCount == null ? "" : commentCount.InnerText,
                        ShareCount = shareCount == null ? "" : shareCount.InnerText,
                        DetailPageLink = detailPageLink == null ? "" : detailPageLink.GetAttributeValue("href", string.Empty).Replace("amp;", ""),
                    };
                }

        }
    }
}
