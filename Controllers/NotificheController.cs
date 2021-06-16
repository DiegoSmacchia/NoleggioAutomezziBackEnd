using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repositories;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Notifiche")]
    public class NotificheController : ControllerBase
    {

        [Route("List/{idUtente}")]
        [HttpGet]
        public IActionResult List(int idUtente)
        {
            NotificheRepository _repo = new NotificheRepository();

            List<string> notifiche = _repo.ListNotifiche(idUtente);

            return Ok(notifiche);
        }

        [Route("ListValoriAdmin")]
        [HttpGet]
        public IActionResult ListValoriAdmin()
        {
            NotificheRepository _repo = new NotificheRepository();

            List<int> valori = _repo.ListValoriAdmin();

            return Ok(valori);
        }

        [Route("ListValoriUtente/{idUtente}")]
        [HttpGet]
        public IActionResult ListValoriUtente(int idUtente)
        {
            NotificheRepository _repo = new NotificheRepository();

            List<int> valori = _repo.ListValoriUtente(idUtente);

            return Ok(valori);
        }
    }
}
