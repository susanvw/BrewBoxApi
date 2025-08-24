using FluentValidation;
using BrewBoxApi.Domain.Aggregates.Drinks;

namespace BrewBoxApi.Application.CQRS.Orders.CreateOrderCommand;

public record CreateDrinkRequest(DrinkType Type, DrinkSize Size, int Quantity);

public record CreateOrderRequest(DateTime PickupTime, decimal? Tip, List<CreateDrinkRequest> Drinks);

public class CreateDrinkRequestValidator : AbstractValidator<CreateDrinkRequest>
{
    private readonly IDrinkRepository _drinkRepository;

    public CreateDrinkRequestValidator(IDrinkRepository drinkRepository)
    {
        _drinkRepository = drinkRepository;

        RuleFor(d => d.Type)
            .IsInEnum().WithMessage("'{PropertyName}' must be a valid DrinkType.");

        RuleFor(d => d.Size)
            .IsInEnum().WithMessage("'{PropertyName}' must be a valid DrinkSize.");

        RuleFor(d => d.Quantity)
            .GreaterThan(0).WithMessage("'{PropertyName}' must be greater than zero.");

        RuleFor(d => d)
            .MustAsync(async (drink, cancellation) =>
            {
                var drinkPrice = await _drinkRepository.GetByTypeAndSizeAsync(drink.Type, drink.Size, cancellation);
                return drinkPrice != null;
            }).WithMessage(d => $"No price found for {d.Type}-{d.Size}.");
    }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(IDrinkRepository drinkRepository)
    {
        RuleFor(o => o.PickupTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.");

        RuleFor(o => o.Tip)
            .GreaterThanOrEqualTo(0).When(o => o.Tip.HasValue).WithMessage("'{PropertyName}' must be non-negative.");

        RuleFor(o => o.Drinks)
            .NotEmpty().WithMessage("'{PropertyName}' cannot be empty.")
            .ForEach(d => d.SetValidator(new CreateDrinkRequestValidator(drinkRepository)));
    }
}