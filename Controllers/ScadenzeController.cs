using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repositories;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Scadenze")]
    public class ScadenzeController : ControllerBase
    {

        [Route("ListScadenze/")]
        [HttpGet]
        public IActionResult ListScadenze()
        {
            ScadenzeRepository _repo = new ScadenzeRepository();

            List<Scadenza> list = _repo.ListScadenze();

            return Ok(list);
        }

        [Route("ListAutomezziScadenze/{idAutomezzo}")]
        [HttpGet]
        public IActionResult ListAutomezzoScadenze(int idAutomezzo)
        {
            ScadenzeRepository _repo = new ScadenzeRepository();
            int? id = idAutomezzo;
            if (id == 0)
                id = null;

            List<AutomezzoScadenza> list = _repo.ListAutomezzoScadenze(id);

            return Ok(list);
        }

        [Route("UpdateScadenza")]
        [HttpPost]
        public IActionResult UpdateScadenza()
        {
            ScadenzeRepository _repo = new ScadenzeRepository();
            Scadenza scadenza = new Scadenza();
            try
            {
                scadenza.id = int.Parse(Request.Form["id"].ToString());
                scadenza.scadenza = Request.Form["dataInizio"].ToString();

                _repo.UpdateScadenza(scadenza);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(scadenza);
        }

        [Route("UpdateAutomezzoScadenza")]
        [HttpPost]
        public IActionResult UpdateAutomezzoScadenza()
        {
            ScadenzeRepository _repo = new ScadenzeRepository();
            AutomezziRepository _repoAutomezzi = new AutomezziRepository();

            AutomezzoScadenza scadenza = new AutomezzoScadenza();
            try
            {
                scadenza.id = int.Parse(Request.Form["id"].ToString());
                scadenza.scadenza.id = int.Parse(Request.Form["idScadenza"].ToString());
                scadenza.automezzo.id = int.Parse(Request.Form["idAutomezzo"].ToString());
                scadenza.dataInizio = DateTime.Parse(Request.Form["dataInizio"].ToString());
                scadenza.dataFine = DateTime.Parse(Request.Form["dataFine"].ToString());
                scadenza.kmIniziali = int.Parse(Request.Form["kmIniziali"].ToString());
                scadenza.dataPagamento = DateTime.Parse(Request.Form["dataPagamento"].ToString());

                _repo.UpdateAutomezzoScadenza(scadenza);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(scadenza);
        }

    }
}
