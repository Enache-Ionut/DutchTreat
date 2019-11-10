using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
  public class DutchRepository : IDutchRepository
  {
    private readonly DutchContext context;
    private readonly ILogger<DutchRepository> logger;

    public DutchRepository(DutchContext context, ILogger<DutchRepository> logger)
    {
      this.context = context;
      this.logger = logger;
    }

    public void AddEntity(object model)
    {
      context.Add(model);
    }

    public IEnumerable<Order> GetAllOrders(bool includeItems)
    {
      try
      {
        if(includeItems)
        {

        logger.LogInformation("GetAllOrders was called");

        return context.Orders
          .Include(o => o.Items)
          .ThenInclude(i => i.Product)
          .ToList();
        }
        else
        {
          return context.Orders
            .ToList();
        }
      }
      catch (Exception ex)
      {
        logger.LogError($"Failed to get all orders: {ex}");
        return null;
      }
    }

    public IEnumerable<Product> GetAllProducts()
    {
      try
      {
        logger.LogInformation("GetAllProducts was called");

        return context.Products
          .OrderBy(p => p.Title)
          .ToList();
      }
      catch (Exception ex)
      {
        logger.LogError($"Failed to get all products: {ex}");
        return null;
      }
    }

    public Order GetOrderById(int id)
    {
      return context.Orders
          .Include(o => o.Items)
          .ThenInclude(i => i.Product)
          .Where(o => o.Id == id)
          .FirstOrDefault();
    }

    public IEnumerable<Product> GetProductsByCategory(string category)
    {
      try
      {
        return context.Products
        .Where(p => p.Category == category)
        .ToList();
      }
      catch (Exception ex)
      {
        logger.LogError($"Failed to get products by category : {ex}");
        return null;
      }
    }

    public bool SaveAll()
    {
      //try
      //{
        return context.SaveChanges() > 0;
      //}
      //catch (Exception ex)
      //{
        //logger.LogError($"Failed to save the changes: {ex}");
        //return false;
      //}
    }

  }

}
