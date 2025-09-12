using API_Desafio_Backend.Controllers.DeliveryPerson.Parameters;
using API_Desafio_Backend.Controllers.Motorcycle.Parameters;
using Desafio_Backend.Controllers.Response;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using Library_Desafio_Backend.FileHandler.Image.CNH;
using Library_Desafio_Backend.Security;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_Desafio_Backend.Controllers.DeliveryPerson
{
    [ApiController]
    [Route("Entregadores")]
    [Tags("Entregadores")]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly MotorcycleRentDbContext _dbContext;
        public DeliveryPersonController( MotorcycleRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Cadastra entregador
        /// </summary>
        /// <response code="201"></response>
        /// <response code="400">Dados Inválidos</response>
        [HttpPost(Name = "Entregadores")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public IActionResult PostRegisterDeiveryPerson([FromBody] DeliveryPersonRegisterParameter deliveryPerson)
        {
            try
            {
                if (string.IsNullOrEmpty(deliveryPerson.Identificador))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador inválido ou vazio." });
                }
                else if (_dbContext.DeliveryPerson.Any(m => m.Identificador == deliveryPerson.Identificador))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador já foi cadastrado para outro entregador." });
                }
                else if (string.IsNullOrEmpty(deliveryPerson.Nome))
                {
                    return BadRequest(new MessageResponse() { Message = "Nome inválido ou vazio." });
                }
                else if (string.IsNullOrEmpty(deliveryPerson.Cnpj))
                {
                    return BadRequest(new MessageResponse() { Message = "Cnpj inválido ou vazio." });
                }
                else if (_dbContext.DeliveryPerson.Any(m => m.Cnpj == deliveryPerson.Cnpj))
                {
                    return BadRequest(new MessageResponse() { Message = "Cnpj já cadastrado." });
                }
                else if (deliveryPerson.Data_nascimento != null && deliveryPerson.Data_nascimento.Year < 1900)
                {
                    return BadRequest(new MessageResponse() { Message = "Data de nascimento inválido ou vazio." });
                }
                else if (string.IsNullOrEmpty(deliveryPerson.Numero_Cnh))
                {
                    return BadRequest(new MessageResponse() { Message = "Numero Cnh inválido ou vazio." });
                }
                else if (_dbContext.DeliveryPerson.Any(m => m.Numero_Cnh == deliveryPerson.Numero_Cnh))
                {
                    return BadRequest(new MessageResponse() { Message = "Numero Cnh inválido ou vazio." });
                }
                else if (string.IsNullOrEmpty(deliveryPerson.Tipo_Cnh) || deliveryPerson.Tipo_Cnh.Length > 2)
                {
                    return BadRequest(new MessageResponse() { Message = "Tipo Cnh já cadastrado." });
                }
                else if (!deliveryPerson.Tipo_Cnh.ToUpper().Equals("A") && !deliveryPerson.Tipo_Cnh.ToUpper().Equals("B") && !deliveryPerson.Tipo_Cnh.ToUpper().Equals("AB"))
                {
                    return BadRequest(new MessageResponse() { Message = "Tipo Cnh inválido" });

                }

                DeliveryPersonClass deliveryPersonClass = new DeliveryPersonClass();
                deliveryPersonClass.Identificador = deliveryPerson.Identificador;
                deliveryPersonClass.Nome = deliveryPerson.Nome;
                deliveryPersonClass.Cnpj = deliveryPerson.Cnpj;
                deliveryPersonClass.Data_nascimento = deliveryPerson.Data_nascimento;
                deliveryPersonClass.Numero_Cnh = deliveryPerson.Numero_Cnh;
                deliveryPersonClass.Tipo_Cnh = deliveryPerson.Tipo_Cnh.ToUpper();

                try
                {                   
                    _dbContext.DeliveryPerson.Add(deliveryPersonClass);
                    _dbContext.SaveChanges();
                    DeliveryPersonClass deliveryPersonCNH = _dbContext.DeliveryPerson.FirstOrDefault(d => d.Cnpj == deliveryPerson.Cnpj);
                    if (!string.IsNullOrEmpty(deliveryPerson.Imagem_Cnh) && deliveryPersonCNH != null)
                    {
                        deliveryPersonCNH.Imagem_Cnh = new SaveCNH().SaveImage(deliveryPerson.Imagem_Cnh, deliveryPersonCNH.Identificador);
                        _dbContext.SaveChanges();
                    }
                    else if(deliveryPersonCNH == null)
                    {
                        return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
                    }
                }
                catch (ArgumentException ex)
                {
                    DeliveryPersonClass deleteDeliveryPerson = _dbContext.DeliveryPerson.FirstOrDefault(d => d.Cnpj == deliveryPerson.Cnpj);
                    if(deleteDeliveryPerson != null)
                    {
                        _dbContext.DeliveryPerson.Remove(deleteDeliveryPerson);
                        _dbContext.SaveChanges();
                    }
                    if (ex.Message == "No images send")
                    {
                        return BadRequest(new MessageResponse() { Message = "Nenhuma CNH enviada." });
                    }
                    else if (ex.Message == "Invalid Extension") 
                    {
                        return BadRequest(new MessageResponse() { Message = "Extensão da CNH inválida, permitido apenas PNG e BMP." });
                    }
                    else if(ex.Message == "Invalid image")
                    {
                        return BadRequest(new MessageResponse() { Message = "Imagem da CNH inválida."});
                    }
                    return BadRequest(new MessageResponse() { Message = ex.Message });
                }
                catch (Exception)
                {
                    DeliveryPersonClass deleteDeliveryPerson = _dbContext.DeliveryPerson.FirstOrDefault(d => d.Cnpj == deliveryPerson.Cnpj);
                    if (deleteDeliveryPerson != null)
                    {
                        _dbContext.DeliveryPerson.Remove(deleteDeliveryPerson);
                        _dbContext.SaveChanges();
                    }
                    return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
                }
                return StatusCode(201, "Created");

            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }
        /// <summary>
        /// Enviar foto da CNH
        /// </summary>
        /// <response code="201"></response>    
        /// <response code="400">Dados Inválidos</response>
        [HttpPut("{id}/Cnh")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public IActionResult PostChangeDeliveryPersonCNHImage(string id, [FromBody][Required] CNHChangeParameter imagem_cnh)
        {
            try
            {
                DeliveryPersonClass deliveryPersonCNH = _dbContext.DeliveryPerson.FirstOrDefault(d => d.Identificador == id);
                if (deliveryPersonCNH == null)
                {
                    return BadRequest(new MessageResponse() { Message = $"Entregador com Id {id} não encontrada." });
                }
                deliveryPersonCNH.Imagem_Cnh =  new SaveCNH().SaveImage(imagem_cnh.Imagem_Cnh, id);      
                _dbContext.SaveChanges();
                return StatusCode(201);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new MessageResponse() { Message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }

        /// <summary>
        /// Consultar entregadores existentes
        /// </summary>
        /// <response code="200">Lista de entregadores</response>      
        /// <response code="404">Entregador não encontrado</response>
        [HttpGet(Name = "Entragadores")]
        [ProducesResponseType(typeof(DeliveryPersonClass), 200)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        public IActionResult GetMotorcycles([FromQuery] string? cnpj)
        {
            try
            {
                if (!string.IsNullOrEmpty(cnpj))
                {
                    DeliveryPersonClass deliveryPerson = _dbContext.DeliveryPerson.FirstOrDefault(m => m.Cnpj == cnpj);
                    if (deliveryPerson != null)
                    {
                        return new OkObjectResult(deliveryPerson);
                    }
                    else
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Entregador não encontrado." });
                    }
                }
                else
                {
                    IList<DeliveryPersonClass> deliveryPeople = _dbContext.DeliveryPerson.ToList();
                    if (deliveryPeople.Count == 0)
                    {
                        return StatusCode(404, new MessageResponse() { Message = "Nenhum entregador não encontrado." });
                    }
                    else
                    {
                        return new OkObjectResult(deliveryPeople);
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }

    }
}
