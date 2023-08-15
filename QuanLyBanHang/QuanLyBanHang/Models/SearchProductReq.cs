using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBanHang.Models
{
    public class SearchProductReq
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string Keyword { get; set; }
    }
}
