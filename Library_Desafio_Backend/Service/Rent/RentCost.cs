using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.Service.Rent
{
    public class RentCost
    {
        public decimal CalculateRentCost(RentClass rent, RentalPlanClass rentalPlan, DateTime endRentDate)
        {

            DateTime startDate = rent.Data_inicio.Date;
            DateTime predictedEndDate = rent.Data_previsao_termino.Date;
            DateTime endDate = endRentDate.Date;
            TimeSpan dateDiff;
            dateDiff = endDate - startDate;
            decimal regularValue = rentalPlan.ValorDiaria * dateDiff.Days;
            if (predictedEndDate == endDate)
            {
                return regularValue;

            }
            else if (predictedEndDate > endDate)
            {
                dateDiff = predictedEndDate - endDate;
                decimal notRegularValue = rentalPlan.ValorDiaria * dateDiff.Days * rentalPlan.Multa;
                return regularValue + notRegularValue;

            }
            else if (predictedEndDate < endDate)
            {
                dateDiff = predictedEndDate - startDate;
                regularValue = rentalPlan.ValorDiaria * dateDiff.Days;
                dateDiff = endDate - predictedEndDate;
                decimal notRegularValue = rentalPlan.ValorDiariaAposTermino * dateDiff.Days;
                return regularValue + notRegularValue;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
