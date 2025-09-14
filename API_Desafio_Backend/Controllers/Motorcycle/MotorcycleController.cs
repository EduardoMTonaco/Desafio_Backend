using API_Desafio_Backend.Controllers.Motorcycle.Parameters;
using Desafio_Backend.Controllers.Response;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using Library_Desafio_Backend.MessageBroker.Event;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_Desafio_Backend.Controllers.Motorcycle
{
    [ApiController]
    [Route("Motos")]
    [Tags("Motos")]
    public class MotorcycleController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly MotorcycleRentDbContext _dbContext;
        public MotorcycleController( IPublishEndpoint publishEndpoint, MotorcycleRentDbContext dbContext)
        {
            _publishEndpoint = publishEndpoint;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Cadastra uma nova moto
        /// </summary>
        /// <response code="201"></response>
        /// <response code="400">Dados Inválidos</response>
        [HttpPost(Name = "Motos")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public async Task<IActionResult> PostRegisterMotorcycleAsync([Required] MotorcycleInsertParameter motorcycle)
        {
            try
            {
                if (string.IsNullOrEmpty(motorcycle.Identificador))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador inválido ou vazio." });
                }
                else if (_dbContext.Motorcycles.Any(m => m.Identificador == motorcycle.Identificador))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador já foi cadastrado para outra moto." });
                }
                else if (motorcycle.Ano < 1900)
                {
                    return BadRequest(new MessageResponse() { Message = "Ano inválido." });
                }
                else if (string.IsNullOrEmpty(motorcycle.Modelo))
                {
                    return BadRequest(new MessageResponse() { Message = "Modelo inválido ou vazio." });
                }
                else if (string.IsNullOrEmpty(motorcycle.Placa))
                {
                    return BadRequest(new MessageResponse() { Message = "Placa inválido ou vazio." });
                }
                MotorcycleClass motorcycleEntity = new MotorcycleClass
                {
                    Identificador = motorcycle.Identificador,
                    Ano = motorcycle.Ano,
                    Modelo = motorcycle.Modelo,
                    Placa = motorcycle.Placa
                };
                if (_dbContext.Motorcycles.Any(m => m.Placa == motorcycleEntity.Placa))
                {
                    return BadRequest(new MessageResponse() { Message = $"Já existe uma moto cadastrada com a placa {motorcycle.Placa}" });
                }
                else
                {
                    _dbContext.Motorcycles.Add(motorcycleEntity);
                    _dbContext.SaveChanges();
                    MotorcycleClass motorcyclehistory = _dbContext.Motorcycles.FirstOrDefault(m => m.Placa == motorcycle.Placa);
                    MotorcycleHistoryClass motorcycleHistoryClass = new MotorcycleHistoryClass();
                    motorcycleHistoryClass.Moto_id = motorcyclehistory.Identificador;
                    motorcycleHistoryClass.Data_cadastro = DateTime.Now;
                    _dbContext.MotorcycleHistory.Add(motorcycleHistoryClass);
                    _dbContext.SaveChanges();
                }
#if !DEBUG
                RegisterMotorcyleEvent eventRabbit = new RegisterMotorcyleEvent
                {
                    Identificador = motorcycle.Identificador,
                    Ano = motorcycle.Ano,
                    Modelo = motorcycle.Modelo,
                    Placa = motorcycle.Placa
                };
                await _publishEndpoint.Publish<RegisterMotorcyleEvent>(eventRabbit, context => context.SetRoutingKey("register-motorcyle_queue"));
#endif
                return StatusCode(201, "Created");
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
        /// <summary>
        /// Consultar motos existentes
        /// </summary>
        /// <response code="200">Lista de motos</response>      
        /// <response code="404">Moto não encontrada</response>
        [HttpGet(Name = "Motos")]
        [ProducesResponseType(typeof(MotorcycleInsertParameter), 200)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        public IActionResult GetMotorcycles([FromQuery] string? plate)
        {
            try
            {
                if (!string.IsNullOrEmpty(plate))
                {
                    MotorcycleClass motorcycle = _dbContext.Motorcycles.FirstOrDefault(m => m.Placa == plate);
                    if (motorcycle != null)
                    {
                        return new OkObjectResult(motorcycle);
                    }
                    else
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Moto não encontrada." });
                    }
                }
                else
                {
                    IList<MotorcycleClass> motorcycles = _dbContext.Motorcycles.ToList();
                    if (motorcycles.Count == 0)
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Nenhuma moto não encontrada." });
                    }
                    else
                    {
                        return new OkObjectResult(motorcycles);
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
        /// <summary>
        /// Modificar uma placa de uma moto
        /// </summary>
        /// <response code="200">Placa modificada com sucesso</response>    
        /// <response code="400">Dados Inválidos</response>
        [HttpPut("{id}/placa")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public IActionResult PutChangeMotorcyclePlate([Required] string id, [FromBody][Required] MotorcycleChangePlateParameter plate)
        {
            try
            {
                MotorcycleClass motorcycle = _dbContext.Motorcycles.Find(id);
                if (motorcycle == null)
                {
                    return BadRequest(new MessageResponse() { Message = $"Moto com Id {id} não encontrada." });
                }
                else if (_dbContext.Motorcycles.Any(m => m.Placa == plate.Placa && m.Identificador != id))
                {
                    return BadRequest(new MessageResponse() { Message = $"Placa {plate.Placa} já existe para outra moto." });
                }
                else if (_dbContext.Rent.Any(r => r.Moto_id == motorcycle.Identificador))
                {
                    return BadRequest(new MessageResponse() { Message = $"A moto de Identificador: {motorcycle.Identificador} possui locação." });
                }
                motorcycle.Placa = plate.Placa;
                _dbContext.SaveChanges();
                return Ok("Placa modificada com sucesso.");
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
        /// <summary>
        /// Consultar motos existentes
        /// </summary>
        /// <response code="200"></response>       
        /// <response code="400">Request mal formada</response>
        /// <response code="404">Moto não encontrada</response>
        [HttpGet("{id}", Name = "Motos/")]
        [ProducesResponseType(typeof(MotorcycleInsertParameter), 200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        public IActionResult GetMotorcycleWithId([Required] string id)
        {
            try
            {
                MotorcycleClass motorcycle = _dbContext.Motorcycles.Find(id);
                if (motorcycle != null)
                {
                    return new OkObjectResult(motorcycle);
                }
                return StatusCode(404, new MessageResponse() { Message = "Moto não encontrada." });
            }
            catch (Exception)
            {

                return BadRequest(new MessageResponse() { Message = "Request mal formada." });
            }
        }

        /// <summary>
        /// Remover uma moto
        /// </summary>
        /// <response code="200"></response>
        /// <response code="400">Dados Inválidos</response>   
        /// <response code="404">Moto não encontrada</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        [HttpDelete("{id}", Name = "Motos/")]
        public IActionResult DeleteMotorcycle([Required] string id)
        {
            try
            {
                MotorcycleClass motorcycle = _dbContext.Motorcycles.Find(id);
                if (motorcycle != null)
                {
                    _dbContext.Motorcycles.Remove(motorcycle);
                    _dbContext.SaveChanges();
                    return Ok();
                }
                return StatusCode(404, new MessageResponse() { Message = "Moto não encontrada." });
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
    }
}
