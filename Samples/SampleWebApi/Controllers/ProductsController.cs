using SampleWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleWebApi.Controllers
{
    [Authorize]
    public class ProductsController : ApiController
    {
        private static List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Monitor 24''", Description = "Full HD, HDMI & VGA", Price=139 },
                new Product { Id = 2, Name = "Modem Router", Description = "4 Gigabit ports", Price=46 },
                new Product { Id = 3, Name = "Wireless keyboard", Price = 18 }
            };

        [AllowAnonymous]
        public IHttpActionResult Get() => Ok(products);

        public IHttpActionResult Get(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
                return Ok(product);

            return NotFound();
        }

        public IHttpActionResult Post(Product product)
        {
            var id = products.Max(p => p.Id) + 1;
            product.Id = id;

            products.Add(product);
            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        public IHttpActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                return StatusCode(HttpStatusCode.NoContent);
            }

            return BadRequest();
        }
    }
}
