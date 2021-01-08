using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task2.Models;

namespace Task2.Controllers
{
    public class ProductsController : ApiController
    {
        static readonly IProductRepository repository = new ProductRepository();

        //Get method to retrieve all Products
        [HttpGet]
        [Route("api/products")]
        public IEnumerable<Product> GetAllProducts()
        {
            return repository.GetAll();
        }


        //Get Method to retrieve product with specific id
        [HttpGet]
        [Route("api/products/{id:int:min(1)}", Name = "getProductById")]
        public IHttpActionResult GetProduct(int id)
        {
            var product = repository.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        [Route("api/products", Name = "getProductByCategory")]
        //http://localhost:9000/api/v3/products?category=
        //http://localhost:9000/api/v3/products?category=Groceries

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return repository.GetAll().Where(
                p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }



        [HttpPost]
        [Route("api/products")]
        public HttpResponseMessage PostProduct(Product item)
        {
            if (ModelState.IsValid)
            {
                item = repository.Add(item);
                var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

                // Generate a link to the new product and set the Location header in the response.

                string uri = Url.Link("getProductById", new { id = item.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


        //[HttpPut]
        //[Route("api/products/{id:int}")]
        //public void PutProduct(int id, Product product)
        //{
        //    product.Id = id;
        //    if (!repository.Update(product))
        //    {
        //        throw new HttpResponseException(HttpStatusCode.NotFound);
        //    }
        //}


        [HttpPut]
        [Route("api/products/{id:int}")]
        public HttpResponseMessage PutProduct(int id, Product product)
        {
            product.Id = id;

            if (ModelState.IsValid)
            {
                if (!repository.Update(product))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                var response = Request.CreateResponse<Product>(HttpStatusCode.OK, product);

                // Generate a link to the new product and set the Location header in the response.

                string uri = Url.Link("getProductById", new { id = product.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpDelete]
        [Route("api/products/{id:int}")]
        public void DeleteProduct(int id)
        {
            repository.Remove(id);
        }




    }



}

