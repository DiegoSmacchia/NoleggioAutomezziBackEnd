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
    [Route("API/Meccanici")]
    public class MeccaniciController : ControllerBase
    {

        [Route("ListMeccanici/")]
        [HttpGet]
        public IActionResult ListMeccanici()
        {
            MeccaniciRepository _repo = new MeccaniciRepository();

            List<Meccanico> list = _repo.ListMeccanici();

            return Ok(list);
        }

        [Route("UpdateMeccanico")]
        [HttpPost]
        public IActionResult UpdateMeccanico()
        {
            MeccaniciRepository _repo = new MeccaniciRepository();
            Meccanico meccanico = new Meccanico();
            try
            {
                meccanico.id = int.Parse(Request.Form["id"].ToString());
                meccanico.ragioneSociale = Request.Form["ragioneSociale"].ToString();
                meccanico.indirizzo = Request.Form["indirizzo"].ToString();
                meccanico.telefono = Request.Form["telefono"].ToString();

                _repo.UpdateMeccanico(meccanico);
            }
            catch (InvalidModelException)
            {
                return StatusCode(400); //Bad Request
            }
            catch (Exception)
            {
                return StatusCode(500); //InternalServerError
            }

            return Ok(meccanico);
        }

        [Route("DeleteMeccanico")]
        [HttpPost]
        public IActionResult DeleteMeccanico()
        {
            MeccaniciRepository _repo = new MeccaniciRepository();
            int id = 0;
            int result = 0;
            try
            {
                id = int.Parse(Request.Form["id"].ToString());
                

                bool deleted = _repo.DeleteMeccanico(id);
                result = deleted ? 200 : 409; //409 = Conflict, il meccanico non può essere eliminato perché c'è almeno un intervento collegato

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

    }
}
