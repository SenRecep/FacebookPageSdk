using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookPageSdk.Entities.Abstract
{
    public interface IPost
    {
        string Title { get; set; }
        string Date { get; set; }
        string Description { get; set; }
    }
}
