using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Ecommerce.Contracts.SaleContracts
{
    public record SaleResponse
    (
        int Id,
        string Name,
        int Percentage,
        int ProductId,
        string Status,
        DateTime DateStart,
        DateTime DateEnd
    );
}
