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
                scadenza.scadenza = Request.Form["scadenza"].ToString();

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
                int km;
                DateTime datafine, datapagamento;
                scadenza.id = int.Parse(Request.Form["id"].ToString());
                scadenza.scadenza.id = int.Parse(Request.Form["idScadenza"].ToString());
                scadenza.automezzo.id = int.Parse(Request.Form["idAutomezzo"].ToString());
                scadenza.dataInizio = DateTime.Parse(Request.Form["dataInizio"].ToString());
                if (DateTime.TryParse(Request.Form["dataFine"].ToString(), out datafine))
                    scadenza.dataFine = datafine;
                else
                    scadenza.dataFine = null;
                if (int.TryParse(Request.Form["kmIniziali"].ToString(), out km))
                    scadenza.kmIniziali = km;
                else
                    scadenza.kmIniziali = null;
                if (DateTime.TryParse(Request.Form["dataPagamento"].ToString(), out datapagamento))
                    scadenza.dataPagamento = datapagamento;
                else
                    scadenza.dataPagamento = null;

                _repo.UpdateAutomezzoScadenza(scadenza);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception e)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(scadenza);
        }

        [Route("DeleteScadenza")]
        [HttpPost]
        public IActionResult DeleteScadenza()
        {
            ScadenzeRepository _repo = new ScadenzeRepository();
            int id = 0;
            int result = 0;
            try
            {
                id = int.Parse(Request.Form["id"].ToString());


                bool deleted = _repo.DeleteScadenza(id);
                result = deleted ? 200 : 409; //409 = Conflict, la scadenza non può essere eliminata perché è collegata ad un automezzo

            }
            catch (OperationFailedException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return StatusCode(result);
        }

        [Route("DeleteAutomezzoScadenza")]
        [HttpPost]
        public IActionResult DeleteAutomezzoScadenza()
        {
            ScadenzeRepository _repo = new ScadenzeRepository();
            int id = 0;
            try
            {
                id = int.Parse(Request.Form["id"].ToString());

                bool deleted = _repo.DeleteAutomezzoScadenza(id);

            }
            catch (OperationFailedException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok();
        }

    }
}
