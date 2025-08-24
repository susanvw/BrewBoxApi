using FluentValidation;
using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.UpdateOrderCommand;

public record UpdateOrderStatusRequest(string OrderId, string Status);

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator(IOrderRepository orderRepository)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("'{PropertyName}' must not be empty.")
            .MaximumLength(64).WithMessage("'{PropertyName}' must not exceed 64 characters.")
            .MustAsync(async (id, cancellation) =>
            {
                var order = await orderRepository.GetByIdAsync(id, cancellation);
                return order != null;
            }).WithMessage("Order with ID '{PropertyValue}' does not exist.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("'{PropertyName}' must not be empty.")
            .Must(status => Enum.TryParse<OrderStatus>(status, true, out _))
            .WithMessage("'{PropertyName}' must be a valid OrderStatus (e.g., Placed, InProgress, Ready, Collected, Cancelled).");
    }
}

public record MarkOrderAsPaidRequest(string OrderId);

public class MarkOrderAsPaidRequestValidator : AbstractValidator<MarkOrderAsPaidRequest>
{
    public MarkOrderAsPaidRequestValidator(IOrderRepository orderRepository)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("'{PropertyName}' must not be empty.")
            .MaximumLength(64).WithMessage("'{PropertyName}' must not exceed 64 characters.")
            .MustAsync(async (id, cancellation) =>
            {
                var order = await orderRepository.GetByIdAsync(id, cancellation);
                return order != null;
            }).WithMessage("Order with ID '{PropertyValue}' does not exist.");
    }
}