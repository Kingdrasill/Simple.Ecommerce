using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Ecommerce.Contracts.ReviewContracts
{
    public record ReviewResponse
    (
        int Id,
        int Score,
        string? Comment,
        int UserId,
        int ProductId
    );
}
