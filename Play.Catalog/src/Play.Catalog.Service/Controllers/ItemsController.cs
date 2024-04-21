using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extension;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await _itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(int id)
        {
            var item = await _itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.Now,
            };

            await _itemsRepository.CreateAsync(item);
            
            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemsRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            };
            existingItem.CreatedDate = DateTimeOffset.Now;
            existingItem.Price = updateItemDto.Price;
            existingItem.Description = updateItemDto.Description;
            existingItem.Name = updateItemDto.Name;

            await _itemsRepository.UpdateAsync(existingItem);
            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));



            return NoContent();

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {

            var existingItem = await _itemsRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            };

            await _itemsRepository.RemoveAsync(id);
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));


            return NoContent();

        }



    }
}