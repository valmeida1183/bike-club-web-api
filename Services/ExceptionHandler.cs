using System;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Services
{   
    public static class ExceptionHandlerService 
    {
        public const int SqlServerViolationOfUniqueIndex = 2601;
        public const int SqlServerViolationOfUniqueConstraint = 2627;

        public static ObjectResult HandleException(Exception exception) 
        {
            var x = exception.GetType();

            switch (exception)
            {
                case DbUpdateConcurrencyException dbUpdateConcurrencyEx:
                    return new BadRequestObjectResult(new { message = "This record is already updated." });
                case DbUpdateException dbUpdateEx:
                    var sqlEx = dbUpdateEx?.InnerException as SqlException;

                    if (sqlEx != null && (sqlEx.Number == SqlServerViolationOfUniqueIndex || sqlEx.Number == SqlServerViolationOfUniqueConstraint))
                    {
                         return new ConflictObjectResult(new { message = "Cannot create a record that already exists." });
                    }

                    return new BadRequestObjectResult (new { message = "Cannot create record." });                    

                default:
                    return new BadRequestObjectResult(new { message = exception.Message });
            }
        }
    }
}