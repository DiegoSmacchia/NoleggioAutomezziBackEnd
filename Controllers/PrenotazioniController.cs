using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("API/Prenotazioni")]
    public class PrenotazioniController : ControllerBase
    {

        [Route("List/")]
        [HttpGet]
        public IActionResult ListTutte()
        {
            PrenotazioniRepository _repo = new PrenotazioniRepository();

            List<Prenotazione> list = _repo.ListPrenotazioni(null);

            return Ok(list);
        }
        [Route("List/{idUtente}")]
        [HttpGet]
        public IActionResult List(int idUtente)
        {
            PrenotazioniRepository _repo = new PrenotazioniRepository();

            List<Prenotazione> list = _repo.ListPrenotazioni(idUtente);

            return Ok(list);
        }

        [Route("Update")]
        [HttpPost]
        public IActionResult UpdatePrenotazione()
        {
            PrenotazioniRepository _repo = new PrenotazioniRepository();
            Prenotazione prenotazione = new Prenotazione();
            try
            {
                prenotazione.id = int.Parse(Request.Form["id"].ToString());
                prenotazione.utente.id = int.Parse(Request.Form["idUtente"].ToString());
                prenotazione.automezzo.id = int.Parse(Request.Form["idAutomezzo"].ToString());
                prenotazione.dataInizio = DateTime.Parse(Request.Form["dataInizio"].ToString());
                prenotazione.dataFine = DateTime.Parse(Request.Form["dataFine"].ToString());
                prenotazione.stato = int.Parse(Request.Form["stato"].ToString());

                _repo.UpdatePrenotazione(prenotazione);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (AutomezzoNonDisponibileException)
            {
                return StatusCode(406); //Conflict
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(prenotazione);
        }

        [Route("AccettaPrenotazione")]
        [HttpPost]
        public IActionResult AccettaPrenotazione()
        {
            PrenotazioniRepository _repo = new PrenotazioniRepository();
            try
            {
                int idPrenotazione = int.Parse(Request.Form["id"].ToString());

                _repo.AccettaPrenotazione(idPrenotazione);
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok();
        }
        [Route("RifiutaPrenotazione")]
        [HttpPost]
        public IActionResult RifiutaPrenotazione()
        {
            PrenotazioniRepository _repo = new PrenotazioniRepository();
            Prenotazione prenotazione = new Prenotazione();
            try
            {
                int idPrenotazione = int.Parse(Request.Form["id"].ToString());

                _repo.RifiutaPrenotazione(idPrenotazione);
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok();
        }
    }
}
