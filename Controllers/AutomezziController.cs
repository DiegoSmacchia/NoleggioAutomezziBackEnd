﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Automezzi")]
    public class AutomezziController : ControllerBase
    {

        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            AutomezziRepository _repo = new AutomezziRepository();

            List<Automezzo> list = _repo.ListAutomezzi();

            //return StatusCode(500);
            return Ok(list);
        }

        public static HttpResponseMessage CreateResponse<T>(HttpRequest requestMessage, HttpStatusCode statusCode, T content)
        {
            return new HttpResponseMessage() { StatusCode = statusCode, Content = new StringContent(JsonConvert.SerializeObject(content)) };
        }

        [Route("get/{id}")]
        [HttpGet]
        public Automezzo Get(int Id)
        {
            Automezzo result = new Automezzo();
            AutomezziRepository _repo = new AutomezziRepository();
            result = _repo.GetAutomezzoById(Id);

            return result;

        }

        [Route("Update")]
        [HttpPost]
        public IActionResult UpdateAutomezzo()
        {
            Automezzo automezzo = new Automezzo();
            try
            {
                automezzo.id = int.Parse(Request.Form["id"].ToString());
                automezzo.targa = Request.Form["targa"].ToString();
                automezzo.marca = Request.Form["marca"].ToString();
                automezzo.modello = Request.Form["modello"].ToString();
                automezzo.kmAttuali = int.Parse(Request.Form["kmAttuali"].ToString());
                automezzo.costo = int.Parse(Request.Form["costo"].ToString());

                
                AutomezziRepository _repo = new AutomezziRepository();
                if (!_repo.UpdateAutomezzo(automezzo))
                    throw new InvalidModelException();
            }
            catch (InvalidModelException e)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(automezzo);
        }
    }
}
