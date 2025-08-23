namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.GetAllPromoCodes;

public record GetAllPromoCodesRequest(
    string Code = "",
    string SearchText = "",
    decimal MinDiscountPercentage = 0,
    decimal MaxDiscountPercentage = 100,
    bool IncludeExpired = false,
    int PageNumber = 1,
    int PageSize = 10,
    string UsedByUser = "");