using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Ecommerce.Contracts.ReviewContracts
{
    public record ReviewRequest
    (
        int Score,
        string? Comment,
        int UserId,
        int ProductId,
        int Id = 0
    );
}
