using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Interventi")]
    public class InterventiController : ControllerBase
    {

        [Route("ListInterventi/")]
        [HttpGet]
        public IActionResult ListInterventi()
        {
            InterventiRepository _repo = new InterventiRepository();

            List<Intervento> list = _repo.ListInterventi();

            return Ok(list);
        }
        [Route("ListInterventiByIdAutomezzo/{idAutomezzo}")]
        [HttpGet]
        public IActionResult ListInterventi(int idAutomezzo)
        {
            InterventiRepository _repo = new InterventiRepository();

            List<Intervento> list = _repo.ListInterventiByIdAutomezzo(idAutomezzo);

            return Ok(list);
        }

        [Route("UpdateIntervento")]
        [HttpPost]
        public IActionResult UpdateIntervento()
        {
            InterventiRepository _repo = new InterventiRepository();
            Intervento intervento = new Intervento();
            try
            {
                intervento.id = int.Parse(Request.Form["id"].ToString());
                intervento.automezzo.id = int.Parse(Request.Form["idAutomezzo"].ToString());
                intervento.meccanico.id = int.Parse(Request.Form["idMeccanico"].ToString());
                intervento.guasto.id = int.Parse(Request.Form["idGuasto"].ToString());
                intervento.dataInizio = DateTime.Parse(Request.Form["dataInizio"].ToString());
                intervento.dataFine = DateTime.Parse(Request.Form["dataFine"].ToString());

                _repo.UpdateIntervento(intervento);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(intervento);
        }

        [Route("ListGuasti/")]
        [HttpGet]
        public IActionResult ListGuasti()
        {
            InterventiRepository _repo = new InterventiRepository();

            List<Guasto> list = _repo.ListGuasti();

            return Ok(list);
        }

        [Route("ListGuastiByIdAutomezzo/{idAutomezzo}")]
        [HttpGet]
        public IActionResult ListGuastiByIdAutomezzo(int idAutomezzo)
        {
            InterventiRepository _repo = new InterventiRepository();

            List<Guasto> list = _repo.ListGuastiByIdAutomezzo(idAutomezzo);

            return Ok(list);
        }

        [Route("UpdateGuasto")]
        [HttpPost]
        public IActionResult UpdateGuasto()
        {
            InterventiRepository _repo = new InterventiRepository();
            Guasto guasto = new Guasto();
            try
            {
                guasto.id = int.Parse(Request.Form["id"].ToString());
                guasto.automezzo.id = int.Parse(Request.Form["idAutomezzo"].ToString());
                guasto.utente.id = int.Parse(Request.Form["idUtente"].ToString());
                guasto.descrizione = Request.Form["descrizione"].ToString();
                guasto.data = DateTime.Parse(Request.Form["data"].ToString());

                _repo.UpdateGuasto(guasto);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(guasto);
        }

    }
}
