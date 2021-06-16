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
    [Route("API/Utenti")]
    public class UtentiController : ControllerBase
    {

        [Route("Login")]
        [HttpPost]
        public IActionResult Login()
        {
            Utente utente = null;
            try
            {
                string username = Request.Form["username"].ToString();
                string password = Request.Form["password"].ToString();
                UtentiRepository _repo = new UtentiRepository();
                utente = _repo.Login(username, password);
            }
            catch (LoginFailedException)
            {
                return StatusCode(401); //Unauthorized
            }
            catch(Exception)
            {
                return StatusCode(500); //InternalServerError
            }
            
            return Ok(utente);
        }

        [Route("CheckLogin")]
        [HttpPost]
        public IActionResult CheckLogin()
        {
            Utente utente = null;
            try
            {
                int userId = int.Parse(Request.Form["userId"].ToString());
                string password = Request.Form["password"].ToString();
                UtentiRepository _repo = new UtentiRepository();
                utente = _repo.CheckLogin(userId, password);
            }
            catch (LoginFailedException)
            {
                return StatusCode(401); //Unauthorized
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(utente);
        }

        [Route("Update")]
        [HttpPost]
        public IActionResult UpdateUtente()
        {
            Utente utente = new Utente();
            try
            {
                utente.id = int.Parse(Request.Form["id"].ToString());
                utente.nome = Request.Form["nome"].ToString();
                utente.cognome = Request.Form["cognome"].ToString();
                utente.username = Request.Form["username"].ToString();
                utente.password = Request.Form["password"].ToString();
                utente.dataNascita = DateTime.Parse(Request.Form["dataNascita"]);
                utente.indirizzoEmail = Request.Form["indirizzoEmail"].ToString();
                UtentiRepository _repo = new UtentiRepository();
                if (!_repo.UpdateUtente(utente))
                    throw new InvalidModelException();
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception e)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(utente);
        }

        [Route("cercaUsername/{username}")]
        [HttpGet]
        public IActionResult CercaUsername(string username)
        {
            bool found = false;
            UtentiRepository _repo = new UtentiRepository();
            found = _repo.ExistsUtenteByUsername(username);

            return Ok(found);
        }
        [Route("cercaIndirizzoEmail/{indirizzoEmail}")]
        [HttpGet]
        public IActionResult CercaIndirizzoEmail(string indirizzoEmail)
        {
            bool found = false;
            UtentiRepository _repo = new UtentiRepository();
            found = _repo.ExistsUtenteByIndirizzoEmail(indirizzoEmail);

            return Ok(found);
        }
    }
}
