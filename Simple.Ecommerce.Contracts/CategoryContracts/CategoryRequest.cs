using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Ecommerce.Contracts.CategoryContracts
{
    public record CategoryRequest
    (
        string Name,
        int Id = 0
    );
}
