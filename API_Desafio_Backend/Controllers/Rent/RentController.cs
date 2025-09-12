using API_Desafio_Backend.Controllers.Rent.Parameters;
using Desafio_Backend.Controllers.Response;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase;
using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using Library_Desafio_Backend.Service.Rent;
using Library_Desafio_Backend.Utility;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace API_Desafio_Backend.Controllers.Rent
{
    [ApiController]
    [Route("Locacao")]
    [Tags("Locacao")]
    public class RentController : ControllerBase
    {
        private readonly MotorcycleRentDbContext _dbContext;
        public RentController(MotorcycleRentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Alugar uma moto
        /// </summary>
        /// <response code="201"></response>
        /// <response code="400">Dados Inválidos</response>       
        [HttpPost(Name = "Locacao")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public IActionResult PostRegisterLease([FromBody][Required] RentRegisterParameter rent)
        {
            try
            {
                if (rent.Data_Inicio < DateTime.Now)
                {
                    return BadRequest(new MessageResponse() { Message = "Data inicio não pode ser inferior a data atual." });
                }
                else if (rent.Data_Previsao_Termino < DateTime.Now)
                {
                    return BadRequest(new MessageResponse() { Message = "Data previsão termino não pode ser inferior a data atual." });
                }
                else if (string.IsNullOrEmpty(rent.Entregador_Id))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador do entregador inválido ou vazio." });
                }
                else if (string.IsNullOrEmpty(rent.Moto_Id))
                {
                    return BadRequest(new MessageResponse() { Message = "Identificador da moto inválido ou vazio." });
                }
                else if (rent.Plano <= 0)
                {
                    return BadRequest(new MessageResponse() { Message = "Plano inválido." });
                }
                TimeSpan dateDiff;
                dateDiff = rent.Data_Previsao_Termino - rent.Data_Inicio;
                if (dateDiff.Days < 1)
                {
                    return BadRequest(new MessageResponse() { Message = "A data previsão termino precisa ser no minimo um dia após o data inicial." });
                }
                if (rent.Data_Termino != null)
                {
                    DateTime dataNaoNullable = rent.Data_Termino ?? DateTime.MaxValue;
                    dateDiff = dataNaoNullable - rent.Data_Inicio;
                    if (dateDiff.Days < 1)
                    {
                        return BadRequest(new MessageResponse() { Message = "A data termino precisa ser no minimo um dia após o data inicial." });
                    }
                    else if (rent.Data_Termino < DateTime.Now)
                    {
                        return BadRequest(new MessageResponse() { Message = "Data termino não pode ser inferior a data atual." });
                    }
                }
                MotorcycleClass motorcycle = _dbContext.Motorcycles.FirstOrDefault(m => m.Identificador == rent.Moto_Id);
                if (motorcycle == null)
                {
                    return BadRequest(new MessageResponse() { Message = $"Moto com Identificador {rent.Moto_Id} não encontrada." });
                }

                DeliveryPersonClass deliveryPerson = _dbContext.DeliveryPerson.FirstOrDefault(p => p.Identificador == rent.Entregador_Id);
                if (deliveryPerson == null)
                {
                    return BadRequest(new MessageResponse() { Message = $"Entregador com Id {rent.Entregador_Id} não encontrada." });
                }
                else if (!deliveryPerson.Tipo_Cnh.Contains("A"))
                {
                    return BadRequest(new MessageResponse() { Message = $"Entregador não habilitado para dirigir moto." });
                }

                if (_dbContext.Rent.Any(l => l.Data_inicio <= rent.Data_Inicio && (l.Data_termino == null ? DateTime.MaxValue : l.Data_termino) >= rent.Data_Inicio && l.Moto_id == rent.Moto_Id))
                {
                    return BadRequest(new MessageResponse() { Message = $"Moto alugada durante o periodo." });
                }
                if (rent.Plano > 0)
                {
                    if (!_dbContext.RentalPlan.Any(r => r.Plano == rent.Plano))
                    {
                        return BadRequest(new MessageResponse() { Message = $"Plano não encontrado." });
                    }
                    else if (FindRentalPlan(rent.Data_Inicio, rent.Data_Previsao_Termino).Plano != rent.Plano)
                    {
                        return BadRequest(new MessageResponse() { Message = $"Plano não encontrado." });
                    }
                }
                else
                {
                    rent.Plano = FindRentalPlan(rent.Data_Inicio, rent.Data_Previsao_Termino).Plano;
                }

                RentClass rentClass = new RentClass();
                rentClass.Moto_id = rent.Moto_Id;
                rentClass.Entregador_id = rent.Entregador_Id;
                rentClass.Data_inicio = rent.Data_Inicio;
                rentClass.Data_termino = rent.Data_Termino;
                rentClass.Data_previsao_termino = rent.Data_Previsao_Termino;
                rentClass.Plano = (int)rent.Plano;
                _dbContext.Rent.Add(rentClass);
                _dbContext.SaveChanges();

                return StatusCode(201, "Created");
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }

        /// <summary>
        /// Consultar locação por id
        /// </summary>
        /// <response code="200"></response>
        /// <response code="400">Dados Inválidos</response>
        /// <response code="404">Dados não encontrados</response>
        [HttpGet(Name = "Locacao/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        [ProducesResponseType(typeof(MessageResponse), 404)]
        public IActionResult GetLease([FromQuery] string? id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                RentClass rent = _dbContext.Rent.FirstOrDefault(l => l.Identificador == id);
                if (rent != null)
                {
                    return new OkObjectResult(rent);
                }
                else
                {
                    return StatusCode(404, new MessageResponse() { Message = "Locação não encontrada." });
                }
            }
            else
            {
                IList<RentClass> rentals = _dbContext.Rent.ToList();
                if (rentals.Count == 0)
                {
                    return StatusCode(404, new MessageResponse() { Message = "Nenhuma locação não encontrada." });
                }
                else
                {
                    return new OkObjectResult(rentals);
                }
            }
        }

        /// <summary>
        /// Informar data de devolução e calcular valor
        /// </summary>
        /// <response code="200">Data de devolução informada com sucesso</response>
        /// <response code="400">Dados Inválidos</response>
        [HttpPut("{id}/devolucao")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(MessageResponse), 400)]
        public IActionResult PutReturnLease([Required] string id, [FromBody][Required] RentReturnMotorcycleParameter rentReturn)
        {
            try
            {
                RentClass rent = _dbContext.Rent.FirstOrDefault(l => l.Identificador == id);
                if (rent == null)
                {
                    return StatusCode(404, new MessageResponse() { Message = "Locação não encontrada." });
                }
                if (rent.Data_inicio >= rentReturn.Data_Termino)
                {
                    return StatusCode(404, new MessageResponse() { Message = "Data Termino inválida." });
                }
                RentalPlanClass rentalPlan = _dbContext.RentalPlan.FirstOrDefault(l => l.Plano == rent.Plano);
                if (rentalPlan == null)
                {
                    return StatusCode(404, new MessageResponse() { Message = "Locação com plano invalido." });
                }
                int plan = FindPlan(rent.Data_inicio, rent.Data_previsao_termino);
                RentCost rentCost = new RentCost();
                rent.Data_termino = rentReturn.Data_Termino;
                decimal value = rentCost.CalculateRentCost(rent, rentalPlan, rentReturn.Data_Termino);
                rent.valor = value;

                _dbContext.SaveChanges();

                return Ok($"Valor do aluguel ficou: R${Mask.MaskMoney(value)}");
            }
            catch (Exception)
            {
                return BadRequest(new MessageResponse() { Message = "Dados inválidos." });
            }
        }

        private int FindPlan(DateTime data_inicio, DateTime predictedEndDate)
        {
            DateTime startDate = data_inicio.Date;
            DateTime EndDate = predictedEndDate.Date;
            TimeSpan dateDiff;
            dateDiff = EndDate - startDate;
            return dateDiff.Days;
        }
        private RentalPlanClass FindRentalPlan(DateTime startDate, DateTime predictedEndDate)
        {
            int days = FindPlan(startDate, predictedEndDate);
            List<RentalPlanClass> plan = _dbContext.RentalPlan.ToList();
            RentalPlanClass correctPlan = plan.OrderBy(p => p.Dias).FirstOrDefault(p => days <= p.Dias);

            if (correctPlan == null)
            {
                correctPlan = plan.OrderByDescending(p => p.Dias).FirstOrDefault();
            }

            return correctPlan;
        }
    }
}
