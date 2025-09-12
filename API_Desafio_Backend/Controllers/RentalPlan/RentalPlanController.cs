using API_Desafio_Backend.Controllers.Motorcycle.Parameters;
using Desafio_Backend.Controllers.Response;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace API_Desafio_Backend.Controllers.RentalPlan
{

    [ApiController]
    [Route("Planos")]
    [Tags("Planos")]
    public class RentalPlanController : ControllerBase
    {
        private readonly MotorcycleRentDbContext _dbContext;
        public RentalPlanController( MotorcycleRentDbContext dbContext)
        {           
            _dbContext = dbContext;
        }
        /// <summary>
        /// Consultar planos de aluguel existentes
        /// </summary>
        /// <response code="200">Lista de planos de aluguel</response>      
        /// <response code="400">Dados inválidos</response> 
        /// <response code="404">Plano não encontrada</response>
        [HttpGet(Name = "Planos")]
        [ProducesResponseType(typeof(MotorcycleInsertParameter), 200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        public IActionResult GetRentalPlans([FromQuery] string? id, int? plano)
        {
            try
            {
                if(!string.IsNullOrEmpty(id) && plano > 0)
                {
                    RentalPlanClass rentalPlan = _dbContext.RentalPlan.FirstOrDefault(r => r.Identificador == id && r.Plano == plano);
                    if (rentalPlan != null)
                    {
                        return new OkObjectResult(rentalPlan);
                    }
                    else
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Plano não encontrado." });
                    }
                }
                else if(!string.IsNullOrEmpty(id))
                {
                    RentalPlanClass rentalPlan = _dbContext.RentalPlan.FirstOrDefault(r => r.Identificador == id);
                    if (rentalPlan != null)
                    {
                        return new OkObjectResult(rentalPlan);
                    }
                    else
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Plano não encontrado." });
                    }
                }
                else if (plano > 0)
                {
                    RentalPlanClass rentalPlan = _dbContext.RentalPlan.FirstOrDefault(r => r.Plano == plano);
                    if (rentalPlan != null)
                    {
                        return new OkObjectResult(rentalPlan);
                    }
                    else
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Plano não encontrado." });
                    }
                }
                else
                {
                    IList<RentalPlanClass> rentalPlans = _dbContext.RentalPlan.ToList();
                    if (rentalPlans.Count == 0)
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Nenhum plano não encontrado." });
                    }
                    else
                    {
                        return new OkObjectResult(rentalPlans);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse() { Message = ex.Message + " ---- " +ex.StackTrace });
                //return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
    }
}
