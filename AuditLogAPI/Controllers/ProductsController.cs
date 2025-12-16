using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditLogAPI.Model;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.IdentityModel.Tokens;
using AuditLogAPI.Model.Dto;
using Azure.Core.Serialization;
//using Newtonsoft.Json;
using System.Text.Json;

namespace AuditLogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        //新增search
        //GET: api/Products/search?keyword=xx&minPrice=1&maxPrice=100
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery]string? keyword, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            // 1) 準備參數（避免 SQL injection）
            var pKeyword = new SqlParameter("@keyword", (object?)keyword ?? DBNull.Value);
            var pMinPrice = new SqlParameter("@minPrice", (object?)minPrice ?? DBNull.Value);
            var pMaxPrice = new SqlParameter("@maxPrice", (object?)maxPrice ?? DBNull.Value);

            // 2) 呼叫 SP（重點：call sqlserver sp）
            // 建議你的 SP 回傳欄位別名為 Json，例如：SELECT (...) AS Json;

            var rows = await _context.JsonResultRows.FromSqlRaw("EXEC usp_Product_SearchJson @keyword, @minPrice, @maxPrice", pKeyword, pMinPrice, pMaxPrice)
                .AsNoTracking() // 取出 Json 欄位
                .ToListAsync(); // 只取第一筆結果

            // 3) 這裡：解析 JSON -> 轉成物件（重點：parse JSON）
            // SP 回來可能是 null 或空字串

            var jsonText = rows.FirstOrDefault()?.Json;

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                return Ok(new List<ProductDto>()); // 回傳空陣列
            }

            // 使用 System.Text.Json 解析 JSON
            var products = JsonSerializer.Deserialize<List<ProductDto>>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductDto>();

            // 4) 回傳結果
            return Ok(products);
        }









        #region CRUD Operations

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody]ProductPutDto dto)
        {

            var entity = await _context.Products.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = dto.Name;
            entity.Price = dto.Price;

            try
            {
                await _context.SaveChangesAsync();

                //call sp, insert log
                await InsertAuditLogAsync("/api/products/" + id, "PUT", JsonSerializer.Serialize(dto));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody]ProductPostDto dto)
        {
            var entity = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                //CreatedAt = DateTime.Now
            };


            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            //call sp, insert log
            await InsertAuditLogAsync("/api/products", "POST", JsonSerializer.Serialize(entity));

            return CreatedAtAction("GetProduct", new { id = entity.Id }, entity);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            //call sp, insert log
            await InsertAuditLogAsync("/api/products/" + id, "DELETE", requestBodyJson: null);

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        #endregion


        //新增call sp 寫log方法

        private async Task InsertAuditLogAsync(string path, string method, string? requestBodyJson)
        {

            // 產生params for prevent SQL injection
            var pPath = new SqlParameter("@path", path);
            var pMethod = new SqlParameter("@method", method);
            var pRequestBody = new SqlParameter("@requestBody", (object?)requestBodyJson ?? DBNull.Value);

            //call sp
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.usp_ApiLog_Insert @path, @method, @requestBody", pPath, pMethod, pRequestBody);

        }

        }
}
