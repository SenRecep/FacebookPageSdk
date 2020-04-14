using FacebookPageSdk.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookPageSdk.Entities.Concrete
{
    public class Post : IPost
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string LikeCount { get; set; }
        public string CommentCount { get; set; }
        public string ShareCount { get; set; }
        public string DetailPageLink { get; set; }
    }
}
