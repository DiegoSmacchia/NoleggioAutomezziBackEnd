using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repositories;
using System;
using System.Collections.Generic;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Multe")]
    public class MulteController : ControllerBase
    {

        [Route("ListMulte/")]
        [HttpGet]
        public IActionResult ListMulte()
        {
            MulteRepository _repo = new MulteRepository();

            List<Multa> list = _repo.ListMulte(null);

            return Ok(list);
        }

        [Route("ListMulteByIdUtente/")]
        [HttpGet]
        public IActionResult ListMulteByIdUtente(int idUtente)
        {
            MulteRepository _repo = new MulteRepository();

            List<Multa> list = _repo.ListMulte(idUtente);

            return Ok(list);
        }

        [Route("UpdateMulta")]
        [HttpPost]
        public IActionResult UpdateMulta()
        {
            MulteRepository _repo = new MulteRepository();
            Multa multa = new Multa();
            try
            {
                multa.id = int.Parse(Request.Form["id"].ToString());
                multa.prenotazione.id = int.Parse(Request.Form["idPrenotazione"].ToString());
                multa.importo = decimal.Parse(Request.Form["importo"].ToString());
                multa.data = DateTime.Parse(Request.Form["data"].ToString());

                _repo.UpdateMulta(multa);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(multa);
        }

    }
}
