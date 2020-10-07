using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPublish.Model.Entity
{
    public class NewsComment
    {
        public int id { get; set; }
        public int Newsid { get; set; }
        public string Contents { get; set; }
        public DateTime AddTime { get; set; }
        public string Remark { get; set; }
        public virtual News News { get; set; }
    }
}
