using Application.Common.Interfaces;

namespace Application.SystemAdmin.Contracts.Commands.SendForSignature;

public record SendForSignatureCommand(Guid Id) : ICommand;
