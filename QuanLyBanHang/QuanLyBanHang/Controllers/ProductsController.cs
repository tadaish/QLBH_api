using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuanLyBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        NorthwindContext da = new NorthwindContext();
        [HttpGet ("GetAllProduct")]
        public IActionResult GetAllProduct()
        {
            var ds = da.Products.ToList();
            return Ok(ds);
        }

        [HttpGet ("Product")]
        public IActionResult GetProductById(int id)
        {
            var ds = da.Products.FirstOrDefault(s => s.ProductId == id);
            return Ok(ds);
        }

        [HttpPost ("Add")]
        public void AddProduct([FromBody] SanPham sp)
        {
            using (var tran = da.Database.BeginTransaction())
            {
                try
                {

                    Products p = new Products();
                    p.ProductId = sp.ProductId;
                    p.ProductName = sp.ProductName;
                    p.SupplierId = sp.SupplierId;
                    p.CategoryId = sp.CategoryId;
                    p.QuantityPerUnit = sp.QuantityPerUnit;
                    p.UnitPrice = sp.UnitPrice;

                    da.Products.Add(p);
                    da.SaveChanges();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }

        }

        [HttpPut ("Update")]
        public void UpdateProduct([FromBody] SanPham sp)
        {
            Products p = da.Products.FirstOrDefault(s => s.ProductId == sp.ProductId);
            p.ProductName = sp.ProductName;
            p.SupplierId = sp.SupplierId;
            p.CategoryId = sp.CategoryId;
            p.QuantityPerUnit = sp.QuantityPerUnit;
            p.UnitPrice = sp.UnitPrice;

            da.Products.Update(p);
            da.SaveChanges();
        }

        [HttpDelete("Delete")]
        public void DeleteProduct(int id)
        {
            Products p = da.Products.FirstOrDefault(s => s.ProductId == id);

            da.Products.Remove(p);
            da.SaveChanges();
        }

        private object SearchProducts(SearchProductReq searchProductReq)
        {
            var products = da.Products.Where(s => s.ProductName.Contains(searchProductReq.Keyword));

            var offset = (searchProductReq.Page - 1) * searchProductReq.Size;
            var total = products.Count();
            var totalPage = (total % searchProductReq.Size) == 0 ? (int)(total / searchProductReq.Size) :
                (int)(1 + (total / searchProductReq.Size));

            var data = products.OrderBy(s => s.ProductId).Skip(offset).Take(searchProductReq.Size).ToList();

            var res = new
            {
                Data = data,
                TotalRecord = total,
                TotalPage = totalPage,
                Page = searchProductReq.Page,
                Size = searchProductReq.Size
            };

            return res;

        }

        [HttpPost ("search Product")]
        public IActionResult SearchProduct([FromBody] SearchProductReq searchProductReq)
        {
            var ds = SearchProducts(searchProductReq);
            return Ok(ds);
        }

        [HttpPost("cal order by customer")]

        public IActionResult CalOrderByCustomer()
        {
            var ds = da.Orders.GroupBy(s => s.CustomerId).Select(s => new { s.Key, sldh = s.Count() });
            return Ok(ds);
        }

        [HttpPost ("CalOrderByYear")]
        public IActionResult CalOrderByYear(int nam)
        {
            var ds = da.Orders.Where(s => s.OrderDate.Value.Year == nam)
                .Join(da.OrderDetails, d => d.OrderId, o => o.OrderId, (d, o) => new { nam = d.OrderDate.Value.Year, tTien = o.Quantity * o.UnitPrice })
                .GroupBy(s => s.nam).Select(g => new { g.Key, total = g.Sum(s => s.tTien) });

            return Ok(ds);
        }
    }
}
