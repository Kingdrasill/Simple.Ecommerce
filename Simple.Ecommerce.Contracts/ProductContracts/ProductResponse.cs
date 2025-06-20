using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Ecommerce.Contracts.ProductContracts
{
    public record ProductResponse
    (
        int Id,
        string Name,
        decimal Price,
        string Description,
        int Stock
    );
}
