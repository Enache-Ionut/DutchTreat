using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
  [Route("api/[Controller]")]
  public class OrdersController : Controller
  {
    private readonly IDutchRepository repository;
    private readonly ILogger<OrdersController> logger;
    private readonly IMapper mapper;

    public OrdersController(IDutchRepository repository, 
      ILogger<OrdersController> logger, IMapper mapper)
    {
      this.repository = repository;
      this.logger = logger;
      this.mapper = mapper;
    }

    [HttpGet]
    public IActionResult Get(bool includeItems = true)
    {
      try
      {
        var orders = repository.GetAllOrders(includeItems);

        var result = mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(orders);

        return Ok(result);
      }
      catch (Exception ex)
      {
        logger.LogError($"Failld to get orders: {ex}");
        return BadRequest("Faild to get orders");
      }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
      try
      {
        var order = repository.GetOrderById(id);

        if (order != null)
        {
          return Ok(mapper.Map<Order, OrderViewModel>(order));
        }
        else
        {
          return NotFound();
        }

      }
      catch (Exception ex)
      {
        logger.LogError($"Failld to get orders: {ex}");
        return BadRequest("Faild to get orders");
      }
    }

    [HttpPost]
    public IActionResult Post([FromBody]OrderViewModel model)
    {
      try
      {
        if (ModelState.IsValid)
        {

          var newOrder = mapper.Map<OrderViewModel, Order>(model);

          if(newOrder.OrderDate == DateTime.MinValue)
          {
            newOrder.OrderDate = DateTime.Now;
          }

          repository.AddEntity(newOrder);

          if (repository.SaveAll())
          {
            return Created($"/api/orders/{newOrder.Id}", mapper.Map<Order, OrderViewModel>(newOrder));
          }
        }
        else
        {
          return BadRequest(ModelState);
        }
      }
      catch (Exception ex)
      {
        logger.LogError($"Failed to save new orders: {ex}");
      }

      return BadRequest("Failed to save new order");
    }

  }
}
