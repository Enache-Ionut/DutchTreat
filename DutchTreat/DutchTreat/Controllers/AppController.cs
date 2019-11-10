﻿using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
  public class AppController : Controller
  {
    private readonly IMailService mailService;
    private readonly DutchContext context;

    public AppController(IMailService mailService, DutchContext context)
    {
      this.mailService = mailService;
      this.context = context;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet("contact")]
    public IActionResult Contact()
    {
      return View();
    }

    [HttpPost("contact")]
    public IActionResult Contact(ContactViewModel model)
    {
      if(ModelState.IsValid)
      {
        // send the email
        mailService.SendMessage("enachestoianionut@gmail.com", model.Subject, 
          $"From: {model.Name} - {model.Email}, Message: {model.Message}");

        ViewBag.UserMessage = "Mail Sent";

        ModelState.Clear();
      }

      return View();
    }

    public IActionResult About()
    {
      ViewBag.Title = "About";
      return View();
    }

    public IActionResult Shop()
    {
      var results = context.Products
        .OrderBy(p => p.Category)
        .ToList();

      return View(results.ToList());
    }


  }
}
