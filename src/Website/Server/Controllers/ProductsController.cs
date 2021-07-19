﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Website.Data.Repositories;
using Website.Server.Services;
using Website.Shared.Constants;
using Website.Shared.Models;

namespace Website.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsRepository productsRepository;
        private readonly DiscordService discordService;

        public ProductsController(ProductsRepository productsRepository, DiscordService discordService)
        {
            this.productsRepository = productsRepository;
            this.discordService = discordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = (await productsRepository.GetProductsAsync()).ToList();

            if (User.Identity.IsAuthenticated)
            {
                foreach (var product in products)
                {
                    if (!product.IsEnabled && product.SellerId != int.Parse(User.Identity.Name))
                    {
                        products.Remove(product);
                    }
                }
            }
            else
            {
                products.RemoveAll(x => !x.IsEnabled);
            }

            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductAsync(int productId)
        {
            int userId = User.Identity?.IsAuthenticated ?? false ? int.Parse(User.Identity.Name) : 0;
            return Ok(await productsRepository.GetProductAsync(productId, userId));
        }

        [HttpGet("{productId}/image")]
        public async Task<IActionResult> GetProductImageAsync(int productId)
        {
            int imageId = await productsRepository.GetProductImageIdAsync(productId);
            return Redirect($"/api/images/{imageId}");
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPost]
        public async Task<IActionResult> PostProductAsync([FromBody] ProductModel product)
        {
            product.SellerId = int.Parse(User.Identity.Name);
            return Ok(await productsRepository.AddProductAsync(product));
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPut]
        public async Task<IActionResult> PutProductAsync([FromBody] ProductModel product)
        {            
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductSellerAsync(product.Id, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.UpdateProductAsync(product);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPost("tabs")]
        public async Task<IActionResult> PostProductTabAsync([FromBody] ProductTabModel tab)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductSellerAsync(tab.ProductId, int.Parse(User.Identity.Name)))
                return BadRequest();

            return Ok(await productsRepository.AddProductTabAsync(tab));
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPut("tabs")]
        public async Task<IActionResult> PutProductTabAsync([FromBody] ProductTabModel tab)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductTabSellerAsync(tab.Id, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.UpdateProductTabAsync(tab);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpDelete("tabs/{tabId}")]
        public async Task<IActionResult> DeleteProductTabAsync(int tabId)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductTabSellerAsync(tabId, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.DeleteProductTabAsync(tabId);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPost("medias")]
        public async Task<IActionResult> PostProductMediaAsync([FromBody] ProductMediaModel media)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductSellerAsync(media.ProductId, int.Parse(User.Identity.Name)))
                return BadRequest();

            return Ok(await productsRepository.AddProductMediaAsync(media));
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpDelete("medias/{mediaId}")]
        public async Task<IActionResult> DeleteProductMediaAsync(int mediaId)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductMediaSellerAsync(mediaId, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.DeleteProductMediaAsync(mediaId);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpPost("customers")]
        public async Task<IActionResult> PostProductCustomerAsync([FromBody] ProductCustomerModel customer)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductSellerAsync(customer.ProductId, int.Parse(User.Identity.Name)))
                return BadRequest();

            return Ok(await productsRepository.AddProductCustomerAsync(customer));
        }

        [Authorize(Roles = RoleConstants.AdminAndSeller)]
        [HttpDelete("customers/{customerId}")]
        public async Task<IActionResult> DeleteProductCustomerAsync(int customerId)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductCustomerSellerAsync(customerId, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.DeleteProductCustomerAsync(customerId);
            return Ok();
        }
        
        [Authorize]
        [HttpPost("reviews")]
        public async Task<IActionResult> PostProductReviewAsync([FromBody] ProductReviewModel review)
        {
            review.UserId = int.Parse(User.Identity.Name);
            if (!await productsRepository.CanReviewProductAsync(review.ProductId, review.UserId))
                return BadRequest();

            review = await productsRepository.AddProductReviewAsync(review);
            await discordService.SendReviewAsync(review, Request.Headers["Origin"]);
            return Ok(review);
        }

        [Authorize]
        [HttpPut("reviews")]
        public async Task<IActionResult> PutProductReviewAsync([FromBody] ProductReviewModel review)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductReviewOwnerAsync(review.Id, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.UpdateProductReviewAsync(review);
            return Ok();
        }

        [Authorize]
        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> DeleteProductReviewAsync(int reviewId)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId) && !await productsRepository.IsProductReviewOwnerAsync(reviewId, int.Parse(User.Identity.Name)))
                return BadRequest();

            await productsRepository.DeleteProductReviewAsync(reviewId);
            return Ok();
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetUserProductsAsync()
        {
            return Ok(await productsRepository.GetUserProductsAsync(int.Parse(User.Identity.Name)));
        }
    }
}
