using AutoMapper;
using Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge;


namespace Highstreetly.Payments
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map Charges
            CreateMap<Charge, Models.Stripe.Charge.Charge>();
            CreateMap<Models.Stripe.Charge.Charge, Charge>();
            
            CreateMap<BillingDetails, Models.Stripe.Charge.BillingDetails>();
            CreateMap<Models.Stripe.Charge.BillingDetails, BillingDetails>();
            
            CreateMap<Address, Models.Stripe.Charge.Address>();
            CreateMap<Models.Stripe.Charge.Address, Address>();
            
            CreateMap<Card, Models.Stripe.Charge.Card>();
            CreateMap<Models.Stripe.Charge.Card, Card>();
            
            CreateMap<Checks, Models.Stripe.Charge.Checks>();
            CreateMap<Models.Stripe.Charge.Checks, Checks>();
            
            CreateMap<Metadata, Models.Stripe.Charge.Metadata>();
            CreateMap<Models.Stripe.Charge.Metadata, Metadata>();
            
            CreateMap<Outcome, Models.Stripe.Charge.Outcome>();
            CreateMap<Models.Stripe.Charge.Outcome, Outcome>();
            
            CreateMap<Refunds, Models.Stripe.Charge.Refunds>();
            CreateMap<Models.Stripe.Charge.Refunds, Refunds>();
            
            CreateMap<FraudDetails, Models.Stripe.Charge.FraudDetails>();
            CreateMap<Models.Stripe.Charge.FraudDetails, FraudDetails>();
            
            CreateMap<TransferData, Models.Stripe.Charge.TransferData>();
            CreateMap<Models.Stripe.Charge.TransferData, TransferData>();
            
            CreateMap<PaymentMethodDetails, Models.Stripe.Charge.PaymentMethodDetails>();
            CreateMap<Models.Stripe.Charge.PaymentMethodDetails, PaymentMethodDetails>();
        }
    }
}