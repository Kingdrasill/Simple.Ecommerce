using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class AddPhotoUserUnitOfWork : BaseUnitOfWork, IAddPhotoUserUnitOfWork
    {
        public IUserRepository Users { get; }

        public AddPhotoUserUnitOfWork(
            TesteDbContext context, 
            IUserRepository users
        ) : base(context) 
        {
            Users = users;
        }
    }
}
