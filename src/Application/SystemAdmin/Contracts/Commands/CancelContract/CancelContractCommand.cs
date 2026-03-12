using Application.Common.Interfaces;

namespace Application.SystemAdmin.Contracts.Commands.CancelContract;

public record CancelContractCommand(Guid Id) : ICommand;
