namespace Simple.Ecommerce.App.Interfaces.Services.Command
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}
