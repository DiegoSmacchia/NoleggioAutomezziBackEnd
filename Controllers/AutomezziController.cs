using Microsoft.AspNetCore.Cors;
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
        [Route("ListDisponibili")]
        [HttpGet]
        public IActionResult ListDisponibili()
        {
            AutomezziRepository _repo = new AutomezziRepository();

            List<Automezzo> list = _repo.ListAutomezziDisponibili();

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
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(automezzo);
        }
        [Route("DeleteAutomezzo")]
        [HttpPost]
        public IActionResult DeleteAutomezzo()
        {
            AutomezziRepository _repo = new AutomezziRepository();
            int id = 0;
            int result = 0;
            try
            {
                id = int.Parse(Request.Form["id"].ToString());

                bool deleted = _repo.DeleteAutomezzo(id);
                result = deleted ? 200 : 409; //409 = Conflict, l'automezzo non può essere eliminato perché ci sono
                                              //almeno un intervento o una prenotazione collegati.

            }
            catch (OperationFailedException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception e)
            {
                return StatusCode(500); //InternalServerError
            }

            return StatusCode(result);
        }
    }
}
