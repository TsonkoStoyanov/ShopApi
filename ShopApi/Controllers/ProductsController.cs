using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Data.Context;
using ShopApi.Data.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopApiDbContext _context;

        public ProductsController(ShopApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ProductDto> Get()
        {
            //TODO: Automapper
            var products =  _context.Products.OrderByDescending(x=>x.CreatedOn);
            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var currentProductDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Discount = product.Discount,
                    Status = product.Status
                };

                productsDto.Add(currentProductDto);
            }

            return productsDto;
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<IResult> Get(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return Results.NotFound();
            }

            //TODO: Automapper
            var productDto = new ProductDto
            {   Id = product.Id,             
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Discount = product.Discount,
                Status = product.Status
            };

            return Results.Ok(productDto);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IResult> Create([FromBody] ProductDto productDto)
        {
            //TODO: Automapper
            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                ImageUrl = productDto.ImageUrl,
                Price = productDto.Price,
                Discount = productDto.Discount,
                Status = productDto.Status,
                CreatedOn = DateTime.Now
            };

            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
       

            return Results.Created("", product);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IResult> Update(Guid id, [FromBody] ProductDto productDto)
        {
            var product = await _context.FindAsync<Product>(id);

            if (product == null)
            {
                return Results.NotFound();
            }

            //TODO: Automapper
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.ImageUrl = productDto.ImageUrl;
            product.Price = productDto.Price;
            product.Discount = productDto.Discount;
            product.Status = productDto.Status;

            await _context.SaveChangesAsync();

            return Results.Ok(product);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(Guid id)
        {
            var product = await _context.FindAsync<Product>(id);

            if (product == null)
            {
                return Results.NotFound();
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();

            return Results.Ok(new Response { Status = ResponseStatus.Success, Message = "Product deleted!" });
        }
    }
}
