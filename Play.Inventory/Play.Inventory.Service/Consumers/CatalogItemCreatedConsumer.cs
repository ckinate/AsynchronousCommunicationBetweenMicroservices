using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> _repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;
            var item = await _repository.GetAsync(x => x.CatalogItemId == message.ItemId);
            if (item != null)
            {
                return;
            }

            item = new CatalogItem
            {
                CatalogItemId = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };
            await _repository.CreateAsync(item);
        }
    }
}