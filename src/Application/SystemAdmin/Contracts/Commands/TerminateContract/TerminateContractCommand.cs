using Application.Common.Interfaces;

namespace Application.SystemAdmin.Contracts.Commands.TerminateContract;

public record TerminateContractCommand(Guid Id) : ICommand;
